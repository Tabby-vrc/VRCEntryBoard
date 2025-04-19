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
        /// <summary>通常インスタンス当たりの最大人数</summary>
        private static int MAX_GROUP_SIZE = 8;
        /// <summary>初心者インスタンス当たりの最大人数</summary>
        private static int MAX_BEGINNER_GROUP_SIZE = 16;
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
            List<Player> beginnerPlayers = players.Where(p => p.EntryStatus == emEntryStatus.Entry &&
                                                              p.ExpStatus.HasFlag(emExpStatus.Beginner)).ToList();  // 初心者プレイヤーリスト.
            int beginnerPlayerCount = beginnerPlayers.Count;                                                        // 初心者数.
            bool needsBeginnerGroup = beginnerPlayerCount >= BEGINERGROUP_NEEDS_LOWESTCOUNT;
            List<int> beginnerGroupNumList = new List<int>();                                                       // 初心者インスタンス番号リスト.
            if (needsBeginnerGroup)
            {
                GroupCountCalculation(beginnerPlayerCount, MAX_BEGINNER_GROUP_SIZE, out beginnerGroupNumList);
            }


            /////////////////////////////////////
            // レギュレーション1割り当て準備.
            List<Player> reg1Players = new List<Player>();                                                        // 経験者プレイヤーリスト.
            if (needsBeginnerGroup)
            {
                reg1Players = players.Where(p => p.EntryStatus == emEntryStatus.Entry &&
                                                 !p.ExpStatus.HasFlag(emExpStatus.Beginner) &&
                                                 p.RegulationStatus == 1).ToList();
            }
            else
            {
                reg1Players = players.Where(p => p.EntryStatus == emEntryStatus.Entry &&
                                                 p.RegulationStatus == 1).ToList();
            }
            int reg1PlayerCount = reg1Players.Count;
            List<int> reg1GroupNumList = new List<int>();                                                         // 通常インスタンス番号リスト.
            GroupCountCalculation(reg1PlayerCount, MAX_GROUP_SIZE, out reg1GroupNumList);                                         // 経験者数.


            /////////////////////////////////////
            // レギュレーション2割り当て準備.
            List<Player> reg2Players = new List<Player>();                                                        // 経験者プレイヤーリスト.
            if (needsBeginnerGroup)
            {
                reg2Players = players.Where(p => p.EntryStatus == emEntryStatus.Entry &&
                                                 !p.ExpStatus.HasFlag(emExpStatus.Beginner) &&
                                                 p.RegulationStatus == 2).ToList();
            }
            else
            {
                reg2Players = players.Where(p => p.EntryStatus == emEntryStatus.Entry &&
                                                 p.RegulationStatus == 2).ToList();
            }
            int reg2PlayerCount = reg2Players.Count;
            List<int> reg2GroupNumList = new List<int>();                                                         // 通常インスタンス番号リスト.
            GroupCountCalculation(reg2PlayerCount, MAX_GROUP_SIZE, out reg2GroupNumList);                                         // 経験者数.

            /////////////////////////////////////
            // スタッフ割り当て作業.
            
            // レギュ2以降のインスタンス番号を更新.
            int reg1MaxNum = reg1GroupNumList.Max();
            int reg2MaxNum = reg2GroupNumList.Max();
            reg2GroupNumList = reg2GroupNumList.Select(num => num += reg1MaxNum).ToList();
            beginnerGroupNumList = beginnerGroupNumList.Select(num => num += reg1MaxNum + reg2MaxNum).ToList();
            
            // 初心者インスタンス割り当て.
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
            // レギュ1インスタンス割り当て.
            List<int> uniqueReg1NumList = ExtractAndRemoveUniqueNumbers(reg1GroupNumList);
            foreach(Player staff in reg1Players.Where(p => 0 == p.GroupNo &&
                                                             true == p.StaffStatus))
            {
                if (uniqueReg1NumList.Count <= 0) break;

                staff.GroupNo = uniqueReg1NumList[0];
                uniqueReg1NumList.RemoveAt(0);
            }
            // 割り当たたなかった分を返却.
            reg1GroupNumList.AddRange(uniqueReg1NumList);
            // レギュ2インスタンス割り当て.
            List<int> uniqueReg2NumList = ExtractAndRemoveUniqueNumbers(reg2GroupNumList);
            foreach(Player staff in reg2Players.Where(p => 0 == p.GroupNo &&
                                                             true == p.StaffStatus))
            {
                if (uniqueReg2NumList.Count <= 0) break;

                staff.GroupNo = uniqueReg2NumList[0];
                uniqueReg2NumList.RemoveAt(0);
            }
            // 割り当たたなかった分を返却.
            reg2GroupNumList.AddRange(uniqueReg2NumList);


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
            // レギュ1割り当て作業.
            foreach (Player normal in reg1Players.Where(p => 0 == p.GroupNo))
            {
                if (reg1GroupNumList.Count <= 0) break;

                int index = rand.Next(0, reg1GroupNumList.Count);
                normal.GroupNo = reg1GroupNumList[index];
                reg1GroupNumList.RemoveAt(index);
            }
            reg1Players.Where(p => 0 == p.GroupNo).ToList().ForEach(p => p.GroupNo = 999);


            /////////////////////////////////////
            // レギュ2割り当て作業.
            foreach (Player normal in reg2Players.Where(p => 0 == p.GroupNo))
            {
                if (reg2GroupNumList.Count <= 0) break;

                int index = rand.Next(0, reg2GroupNumList.Count);
                normal.GroupNo = reg2GroupNumList[index];
                reg2GroupNumList.RemoveAt(index);
            }
            reg2Players.Where(p => 0 == p.GroupNo).ToList().ForEach(p => p.GroupNo = 999);

            // 割り当てなかったプレイヤーを999にする.
            players.Where(p => p.EntryStatus == emEntryStatus.Entry &&
                               p.GroupNo == 0).ToList().ForEach(p => p.GroupNo = 999);
        }

        /// <summary>
        /// グループ毎の人数算出
        /// </summary>
        /// <param name="playerCount"></param>
        /// <param name="groupCountList"></param>
        private void GroupCountCalculation(int playerCount, int maxGroupSize, out List<int> groupCountList)
        {
            groupCountList = new List<int>();
            if (0 >= playerCount) return;

            // グループ数
            int groupCount = (int)Math.Ceiling( (float)playerCount / maxGroupSize );
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
