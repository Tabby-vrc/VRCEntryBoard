using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using VRCEntryBoard.HMI;
using VRCEntryBoard.App.Services;
using VRCEntryBoard.App.Grouping;
using VRCEntryBoard.App.UseCase;
using VRCEntryBoard.Domain.Model;
using VRCEntryBoard.Domain.Interfaces;

namespace VRCEntryBoard.App.Controller
{
    internal class CEntryViewController
    {
        private readonly VRCDataManagementService _vrcDataManagementService;
        private CEntryView _EntryView;
        private CGroupAllocator _GroupAllocator;
        private IPlayerRepository _PlayerRepository;
        private IRegulationRepository _RegulationRepository;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CEntryViewController(VRCDataManagementService vrcDataManagementService, IPlayerRepository playerRepository, IRegulationRepository regulationRepository)
        {
            _vrcDataManagementService = vrcDataManagementService;
            _PlayerRepository = playerRepository;
            _RegulationRepository = regulationRepository;
            _PlayerRepository.SubscribeUpdates();
            _GroupAllocator = new CGroupAllocator();
        }

        public void SetView(CEntryView view)
        {
            _EntryView = view;
        }

        public async Task InitView()
        {
            _EntryView.UpdateEntryNum(new EntryNumDto());
            await _RegulationRepository.UpdateRegulations();
            _EntryView.SetRegulationName(_RegulationRepository.GetRegulations()[0].RegulationName?? "未設定",
                                         _RegulationRepository.GetRegulations()[1].RegulationName?? "未設定");
            await UpdatePlayerList();
        }

        /// <summary>
        /// プレイヤーリストの更新
        /// </summary>
        public async Task UpdatePlayerList()
        {
            await _vrcDataManagementService.UpdatePlayerList();
            
            // 表示順をステータス順に整理.
            var query = _PlayerRepository.GetPlayers().OrderBy(player =>
            {
                int order = player.EntryStatus == emEntryStatus.AskMe ? 3 : player.EntryStatus == emEntryStatus.Visiter ? 2 : 1;
                int regulation = player.RegulationStatus;
                int newUser = player.ExpStatus.HasFlag(emExpStatus.Beginner) ? 3 : player.ExpStatus.HasFlag(emExpStatus.NewUser) ? 2 : 1;
                int staff = player.StaffStatus ? 2 : 1;
                int left = player.JoinStatus ? 1 : 1000;
                return (order * 100 + staff * 10 + regulation + newUser) * left;
            }).ToList();

            _EntryView.UpdatePlayerList(query);

            EntryNumDto entryNumDto = new EntryNumDto();
            GetEntryNum(entryNumDto);
            _EntryView.UpdateEntryNum(entryNumDto);
        }

        /// <summary>
        /// エントリーステータス更新
        /// </summary>
        /// <param name="targetPlayerName">更新対象プレイヤー名</param>
        /// <param name="status">更新ステータス</param>
        public void UpdateEntryStatus(string targetPlayerName, emEntryStatus status)
        {
            var targetPlayer = _PlayerRepository.GetPlayers().FirstOrDefault(x => x.Name == targetPlayerName);
            targetPlayer.EntryStatus = status;
            _PlayerRepository.UpdateEntryStatus(targetPlayer);
            UpdateNum();
        }
        public emEntryStatus GetEntryStatus(string targetPlayerName)
        {
            return _PlayerRepository.GetPlayers().FirstOrDefault(x => x.Name == targetPlayerName).EntryStatus;
        }

        /// <summary>
        /// レギュレーションステータス更新
        /// </summary>
        /// <param name="targetPlayerName">更新対象プレイヤー名</param>
        /// <param name="regStatus">更新ステータス</param>  
        public void UpdateRegulationStatus(string targetPlayerName, int regStatus)
        {
            var targetPlayer = _PlayerRepository.GetPlayers().FirstOrDefault(x => x.Name == targetPlayerName);
            targetPlayer.RegulationStatus = regStatus;
            targetPlayer.ExpStatus &= ~emExpStatus.Beginner;
            _PlayerRepository.UpdateRegulationStatus(targetPlayer);
            UpdateNum();
        }
        public int GetRegulationStatus(string targetPlayerName)
        {
            return _PlayerRepository.GetPlayers().FirstOrDefault(x => x.Name == targetPlayerName).RegulationStatus;
        }

        /// <summary>
        /// 初心者ステータス更新
        /// </summary>
        /// <param name="targetPlayerName">更新対象プレイヤー名</param>
        /// <param name="isBeginner">更新ステータス</param>
        public void UpdateBeginnerStatus(string targetPlayerName, bool isBeginner)
        {
            var targetPlayer = _PlayerRepository.GetPlayers().FirstOrDefault(x => x.Name == targetPlayerName);
            if (isBeginner) targetPlayer.ExpStatus |= emExpStatus.Beginner;
            else            targetPlayer.ExpStatus &= ~emExpStatus.Beginner;
            targetPlayer.RegulationStatus = 0;
            _PlayerRepository.UpdateExpStatus(targetPlayer);
            UpdateNum();
        }
        public emExpStatus GetExpStatus(string targetPlayerName)
        {
            return _PlayerRepository.GetPlayers().FirstOrDefault(x => x.Name == targetPlayerName).ExpStatus;
        }

        /// <summary>
        /// スタッフステータス更新
        /// </summary>
        public void UpdateStaff(string targetPlayerName, bool isStaff)
        {
            var targetPlayer = _PlayerRepository.GetPlayers().FirstOrDefault(x => x.Name == targetPlayerName);
            targetPlayer.StaffStatus = isStaff;
            _PlayerRepository.UpdateStaffStatus(targetPlayer);
            UpdateNum();
        }
        public bool GetStaff(string targetPlayerName)
        {
            return _PlayerRepository.GetPlayers().FirstOrDefault(x => x.Name == targetPlayerName).StaffStatus;
        }

        private void UpdateNum()
        {
            EntryNumDto entryNumDto = new EntryNumDto();
            GetEntryNum(entryNumDto);
            this._EntryView.UpdateEntryNum(entryNumDto);
        }

        public void GroupingPlayerList()
        {
            _GroupAllocator.Allocate(_PlayerRepository.GetPlayers());
        }

        public void OutputPlayerList()
        {
            foreach (var playerGroup in _PlayerRepository.GetPlayers().GroupBy(player => player.GroupNo))
            {
                // どこにも属さないPlayerは出力しない
                if (playerGroup.Key == 0) continue;

                try
                {
                    // CSVファイルを書き出しモードで開く
                    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
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

            EntryNumDto entryNumDto = new EntryNumDto();
            GetEntryNum(entryNumDto);
            this._EntryView.UpdateEntryNum(entryNumDto);
        }

        /// <summary>
        /// 参加人数の取得
        /// </summary>
        /// <param name="entryNum">参加人数</param>
        /// <param name="beginnerNum">初心者人数</param>
        /// <param name="staffNum">スタッフ人数</param>
        private void GetEntryNum(EntryNumDto entryNumDto)
        {
            var playerList = _PlayerRepository.GetPlayers();
            var entryList = playerList.Where(p => p.EntryStatus == emEntryStatus.Entry);

            entryNumDto.Reg1Num = entryList.Count(p => p.RegulationStatus == 1);
            entryNumDto.Reg2Num = entryList.Count(p => p.RegulationStatus == 2);
            entryNumDto.BeginnerNum = entryList.Count(p => p.ExpStatus.HasFlag(emExpStatus.Beginner));
            entryNumDto.StaffNum = playerList.Count(p => p.StaffStatus == true);
            entryNumDto.InstanceNum = (int)Math.Ceiling(entryNumDto.Reg1Num / 8f)
                                    + (int)Math.Ceiling(entryNumDto.Reg2Num / 8f)
                                    + (int)Math.Ceiling(entryNumDto.BeginnerNum / 16f);
        }
    }
}
