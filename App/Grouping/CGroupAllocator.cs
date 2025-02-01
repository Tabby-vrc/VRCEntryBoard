using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using VRCEntryBoard.Domain.Model;

namespace VRCEntryBoard.App.Grouping
{
    /// <summary>
    /// グループ割り当ての既定クラス
    /// </summary>
    internal class CGroupAllocator
    {
        /// <summary>1グループ当たりの最大人数</summary>
        private static int MAX_GROUP_SIZE = 8;
        /// <summary>初心者インスタンス必要最低人数</summary>
        private static int BEGINERGROUP_NEEDS_LOWESTCOUNT = 4;

        public CGroupAllocator() { }

        public void Allocate(List<Player> players)
        {
            /////////////////////////////////////
            // 割り当て前
            players.ForEach(p => p.GroupNo = 0);

            
            /////////////////////////////////////
            // スタッフ割り当て準備.
            // 特になし.

            
            /////////////////////////////////////
            // 初心者割り当て準備.
            List<Player> beginnerPlayers = players.Where(p => emEntryStatus.Entry == p.EntryStatus &&
                                                              p.ExpStatus.HasFlag(emExpStatus.Beginner)).ToList();  // 初心者プレイヤーリスト.
            int beginnerPlayerCount = beginnerPlayers.Count;                                                        // 初心者数.
            bool needsBeginnerGroup = beginnerPlayerCount >= BEGINERGROUP_NEEDS_LOWESTCOUNT;
            List<int> beginnerGroupNumList = new List<int>();                                                       // 初心者インスタンス番号リスト.
            if (needsBeginnerGroup)
            {
                GroupCountCalculation(beginnerPlayerCount, out beginnerGroupNumList);
            }


            /////////////////////////////////////
            // 経験者割り当て準備.
            List<Player> normalPlayers = new List<Player>();                                                        // 経験者プレイヤーリスト.
            if (needsBeginnerGroup)
            {
                normalPlayers = players.Where(p => emEntryStatus.Entry == p.EntryStatus &&
                                                   !p.ExpStatus.HasFlag(emExpStatus.Beginner)).ToList();
            }
            else
            {
                normalPlayers = players.Where(p => emEntryStatus.Entry == p.EntryStatus).ToList();                  // エントリープレイヤーリスト.
            }
            int normalPlayerCount = normalPlayers.Count;
            List<int> normalGroupNumList = new List<int>();                                                         // 通常インスタンス番号リスト.
            GroupCountCalculation(normalPlayerCount, out normalGroupNumList);                                       // 経験者数.


            /////////////////////////////////////
            // スタッフ割り当て作業.
            // 初心者インスタンス割り当て.
            int normalMaxNum = normalGroupNumList.Max();
            beginnerGroupNumList = beginnerGroupNumList.Select(num => num += normalMaxNum).ToList();
            List<int> uniqueBeginnerNumList = ExtractAndRemoveUniqueNumbers(beginnerGroupNumList);
            foreach(Player staff in beginnerPlayers.Where(p => 0 == p.GroupNo &&
                                                               true == p.StaffStatus))
            {
                if (uniqueBeginnerNumList.Count <= 0) break;

                staff.GroupNo = uniqueBeginnerNumList[0];
                uniqueBeginnerNumList.RemoveAt(0);
            }
            // 割り当たたなかった分を返却.
            beginnerGroupNumList.AddRange(uniqueBeginnerNumList);
            // 経験者インスタンス割り当て.
            List<int> uniqueNormalNumList = ExtractAndRemoveUniqueNumbers(normalGroupNumList);
            foreach(Player staff in normalPlayers.Where(p => 0 == p.GroupNo &&
                                                             true == p.StaffStatus))
            {
                if (uniqueNormalNumList.Count <= 0) break;

                staff.GroupNo = uniqueNormalNumList[0];
                uniqueNormalNumList.RemoveAt(0);
            }
            // 割り当たたなかった分を返却.
            normalGroupNumList.AddRange(uniqueNormalNumList);


            /////////////////////////////////////
            // 初心者割り当て作業.
            Random rand = new Random();
            foreach (Player beginner in beginnerPlayers.Where(p => 0 == p.GroupNo))
            {
                if(beginnerGroupNumList.Count <= 0) break;

                int index = rand.Next(0, beginnerGroupNumList.Count);
                beginner.GroupNo = beginnerGroupNumList[index];
                beginnerGroupNumList.RemoveAt(index);
            }
            beginnerPlayers.Where(p => 0 == p.GroupNo).ToList().ForEach(p => p.GroupNo = 999);


            /////////////////////////////////////
            // 経験者割り当て作業.
            foreach (Player normal in normalPlayers.Where(p => 0 == p.GroupNo))
            {
                if (normalGroupNumList.Count <= 0) break;

                int index = rand.Next(0, normalGroupNumList.Count);
                normal.GroupNo = normalGroupNumList[index];
                normalGroupNumList.RemoveAt(index);
            }
            normalPlayers.Where(p => 0 == p.GroupNo).ToList().ForEach(p => p.GroupNo = 999);
        }

        /// <summary>
        /// グループ毎の人数算出
        /// </summary>
        /// <param name="playerCount"></param>
        /// <param name="groupCountList"></param>
        private void GroupCountCalculation(int playerCount, out List<int> groupCountList)
        {
            groupCountList = new List<int>();
            if (0 >= playerCount) return;

            // グループ数
            int groupCount = (int)Math.Ceiling( (float)playerCount / MAX_GROUP_SIZE );
            // 1グループ当たりの人数
            int playerParGroup = playerCount / groupCount;
            bool isPlayerEvenNumber = playerParGroup % 2 == 0;
            // 余り数
            int overPlayerCount = playerCount - playerParGroup * groupCount;

            // 人数が偶数でない場合は余り枠に回して、帳尻合わせする.
            if (false == isPlayerEvenNumber)
            {
                playerParGroup -= 1;
                overPlayerCount += groupCount;
                isPlayerEvenNumber = true;
            }

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


            List<int> indexes = new List<int>(); // インデックス番号を格納するリスト.

            // 配列を走査し、各要素数と一致する数だけインデックス番号を格納.
            for (int i = 0; i < groupCountList.Count; i++)
            {
                for (int j = 0; j < groupCountList[i]; j++)
                {
                    indexes.Add(i + 1);
                }
            }

            // リストを配列に変換して返す.
            groupCountList = indexes.ToList();
            return;
        }

        /// <summary>
        /// ユニーク番号を取り出し削除する処理
        /// </summary>
        /// <param name="numbers">捜索対象リスト</param>
        /// <returns>ユニーク番号リスト</returns>
        private List<int> ExtractAndRemoveUniqueNumbers(List<int> numbers)
        {
            // 数字の種類を保持するHashSet
            HashSet<int> seenNumbers = new HashSet<int>();
            List<int> uniqueNumbers = new List<int>();

            if (numbers.Count <= 0) return uniqueNumbers;

            // リストを逆順に走査することで削除操作を効率化
            for (int i = numbers.Count - 1; i >= 0; i--)
            {
                int num = numbers[i];

                // 数字がまだ見つかっていなければリストから削除して格納
                if (!seenNumbers.Contains(num))
                {
                    uniqueNumbers.Add(num);
                    seenNumbers.Add(num);
                    numbers.RemoveAt(i); // リストから削除
                }
            }

            uniqueNumbers.Reverse(); // 元の順序に戻す
            return uniqueNumbers;
        }
    }
}
