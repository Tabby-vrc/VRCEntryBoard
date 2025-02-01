using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using VRCEntryBoard.App.Grouping;
using VRCEntryBoard.Domain.Interfaces;
using VRCEntryBoard.Domain.Model;
using VRCEntryBoard.HMI;

namespace VRCEntryBoard.App.Controller
{
    internal class CEntryViewController
    {
        private IVRCDataLoder _VRCData;
        private CEntryView _EntryView;
        private PlayerRepository _PlayerRepository;
        private CGroupAllocator _GroupAllocator;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CEntryViewController(IVRCDataLoder vRCData)
        {
            _VRCData = vRCData;
            _PlayerRepository = new PlayerRepository();
            _GroupAllocator = new CGroupAllocator();
        }

        public void SetView(CEntryView view)
        {
            this._EntryView = view;
        }

        /// <summary>
        /// プレイヤーリストの更新
        /// </summary>
        public void UpdatePlayerList()
        {
            this._VRCData.UpdatePlayerList();

            _PlayerRepository.AddPlayer(_VRCData.GetPlayerList());

            // 表示順をステータス順に整理.
            var query = this._VRCData.GetPlayerList().OrderBy(player =>
            {
                int order = player.EntryStatus == emEntryStatus.AskMe ? 3 : player.EntryStatus == emEntryStatus.Visiter ? 2 : 1;
                int newUser = player.ExpStatus.HasFlag(emExpStatus.Beginner) ? 3 : player.ExpStatus.HasFlag(emExpStatus.NewUser) ? 2 : 1;
                int staff = player.StaffStatus ? 2 : 1;
                int left = player.JoinStatus ? 1 : 0;
                return (order * 100 + staff * 10 + newUser) * left;
            }).ToList();

            this._EntryView.UpdatePlayerList(query);

            GetEntryNum(out int entryNum, out int beginnerNum, out int staffNum, out int instanceNum);
            this._EntryView.UpdateEntryNum(entryNum, beginnerNum, staffNum, instanceNum);
        }

        /// <summary>
        /// エントリーステータス更新
        /// </summary>
        /// <param name="targetPlayerName">更新対象プレイヤー名</param>
        /// <param name="status">更新ステータス</param>
        public void UpdateEntryStatus(string targetPlayerName, emEntryStatus status)
        {
            this._PlayerRepository.UpdateStatus(targetPlayerName, status);
            UpdateNum();
        }
        public emEntryStatus GetEntryStatus(string targetPlayerName)
        {
            return this._PlayerRepository.GetEntryStatus(targetPlayerName);
        }

        /// <summary>
        /// 初心者ステータス更新
        /// </summary>
        public void UpdateBeginnerStatus(string targetPlayerName, bool isBeginner)
        {
            var status = _PlayerRepository.GetExpStatus(targetPlayerName);
            if (isBeginner) status |= emExpStatus.Beginner;
            else            status &= ~emExpStatus.Beginner;
            _PlayerRepository.UpdateExpStatus(targetPlayerName, status);
            UpdateNum();
        }
        public emExpStatus GetExpStatus(string targetPlayerName)
        {
            return _PlayerRepository.GetExpStatus(targetPlayerName);
        }

        /// <summary>
        /// スタッフステータス更新
        /// </summary>
        public void UpdateStaff(string targetPlayerName, bool isStaff)
        {
            this._PlayerRepository.UpdateStaff(targetPlayerName, isStaff);
            UpdateNum();
        }
        public bool GetStaff(string targetPlayerName)
        {
            return this._PlayerRepository.GetStaff(targetPlayerName);
        }

        private void UpdateNum()
        {
            GetEntryNum(out int entryNum, out int beginnerNum, out int staffNum, out int instanceNum);
            this._EntryView.UpdateEntryNum(entryNum, beginnerNum, staffNum, instanceNum);
        }

        public void GroupingPlayerList()
        {
            _GroupAllocator.Allocate(_VRCData.GetPlayerList());
            /*
            var playerList = this._VRCData.GetPlayerList();

            int entryPlayerNum = playerList.Where(player => player.EntryStatus == emEntryStatus.Entry).Count();
            List<int> groupCountList = GroupCountCalculation(entryPlayerNum);
            if (0 >= groupCountList.Count()) return;

            List<string> entryPlayerList = playerList.Where(player => player.EntryStatus == emEntryStatus.Entry)
                                                     .Select(player => player.Name).ToList();

            List<int> groupNameList = GetIndexes(groupCountList);

            int playerNum = entryPlayerList.Count;
            var rand = new Random();
            for (int i = 0; i < playerNum - 1; i++)
            {
                int nowPlayerCount = entryPlayerList.Count;
                int selectIndex = rand.Next(0, nowPlayerCount - 2);
                string selectPlayerName = entryPlayerList[selectIndex];

                playerList.First(player => player.Name == selectPlayerName).GroupNo = groupNameList[0];

                entryPlayerList[selectIndex] = entryPlayerList[nowPlayerCount - 1];
                entryPlayerList.RemoveAt(nowPlayerCount - 1);
                groupNameList.RemoveAt(0);
            }
            playerList.First(player => player.Name == entryPlayerList[0]).GroupNo = groupNameList[0];

            foreach (var nonEntryPlayer in playerList.Where(player => player.EntryStatus != emEntryStatus.Entry))
            {
                nonEntryPlayer.GroupNo = 0;
            }
            */
        }

        public void OutputPlayerList()
        {
            foreach (var playerGroup in this._VRCData.GetPlayerList().GroupBy(player => player.GroupNo))
            {
                // どこにも属さないPlayerは出力しない
                if (playerGroup.Key == 0) continue;

                try
                {
                    // CSVファイルを書き出しモードで開く
                    using (StreamWriter writer = new StreamWriter(string.Format("グループ{0}.csv", playerGroup.Key), false, Encoding.GetEncoding("Shift-JIS")))
                    {
                        writer.WriteLine("プレイヤー名");

                        // 配列の各要素をCSVファイルに書き込む
                        foreach (string str in playerGroup.Select(player => player.Name))
                        {
                            writer.WriteLine(str);
                        }
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("ファイル出力に失敗しました。\n出力先ファイルを開いていないか確認してください。");
                }
            }
        }

        private static List<int> GroupCountCalculation(int playerCount)
        {
            const float _maxInstanceCount = 8;

            if (0 >= playerCount) return new List<int>();

            // グループ数
            int groupCount = (int)Math.Ceiling(playerCount / _maxInstanceCount);
            // 1グループ当たりの人数
            int playerParGroup = playerCount / groupCount;
            bool isPlayerEvenNumber = playerParGroup % 2 == 0;
            // 余り数
            int overPlayerCount = playerCount - playerParGroup * groupCount;

            if (false == isPlayerEvenNumber)
            {
                playerParGroup -= 1;
                overPlayerCount += groupCount;
                isPlayerEvenNumber = true;
            }

            List<int> groupCountList = new List<int>();
            for (int i = 0; i < groupCount; i++)
            {
                if (overPlayerCount > 0)
                {
                    if (isPlayerEvenNumber)
                    {
                        if (overPlayerCount >= 2)
                        {
                            groupCountList.Add(playerParGroup + 2);
                            overPlayerCount -= 2;
                        }
                        else
                        {
                            groupCountList.Add(playerParGroup + 1);
                            overPlayerCount -= 1;
                        }
                    }
                    else
                    {
                        groupCountList.Add(playerParGroup + 1);
                        overPlayerCount -= 1;
                    }
                }
                else
                {
                    groupCountList.Add(playerParGroup);
                }
            }
            return groupCountList;
        }

        private static List<int> GetIndexes(List<int> array)
        {
            List<int> indexes = new List<int>(); // インデックス番号を格納するリスト.

            // 配列を走査し、各要素数と一致する数だけインデックス番号を格納.
            for (int i = 0; i < array.Count; i++)
            {
                for (int j = 0; j < array[i]; j++)
                {
                    indexes.Add(i + 1);
                }
            }

            // リストを配列に変換して返す.
            return indexes.ToList();
        }

        /// <summary>
        /// プレイヤーパネルクリック処理
        /// </summary>
        /// 対象プレイヤーのステータスを更新し
        /// 現在の参加人数の再集計を実施する
        /// <param name="player">更新対象プレイヤー</param>
        public void PanelClick(Player player)
        {
            if (emEntryStatus.AskMe == player.EntryStatus ||
                emEntryStatus.Visiter == player.EntryStatus)
            {
                player.EntryStatus = emEntryStatus.Entry;
            }
            else if (emEntryStatus.Entry == player.EntryStatus)
            {
                player.EntryStatus = emEntryStatus.Visiter;
            }

            GetEntryNum(out int entryNum, out int beginnerNum, out int staffNum, out int instanceNum);
            this._EntryView.UpdateEntryNum(entryNum, beginnerNum, staffNum, instanceNum);
        }

        /// <summary>
        /// 参加人数の取得
        /// </summary>
        /// <param name="entryNum">参加人数</param>
        /// <param name="beginnerNum">初心者人数</param>
        /// <param name="staffNum">スタッフ人数</param>
        private void GetEntryNum(out int entryNum, out int beginnerNum, out int staffNum, out int instanceNum)
        {
            var playerList = this._VRCData.GetPlayerList();
            var entryList = playerList.Where(p => p.EntryStatus == emEntryStatus.Entry);

            entryNum = entryList.Count(p => !p.ExpStatus.HasFlag(emExpStatus.Beginner));
            beginnerNum = entryList.Count(p => p.ExpStatus.HasFlag(emExpStatus.Beginner));
            staffNum = playerList.Count(p => p.StaffStatus == true);
            instanceNum = (int)Math.Ceiling(entryNum / 8f) + (int)Math.Ceiling(beginnerNum / 8f);
        }
    }
}
