using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VRCEntryBoard.Domain.Interfaces;
using VRCEntryBoard.Domain.Model;

namespace VRCEntryBoard.Tests
{
    /// <summary>
    /// DB更新結果を制御できるプレイヤーリポジトリのFake
    /// </summary>
    internal sealed class FakePlayerRepository : IPlayerRepository
    {
#pragma warning disable CS0067
        public event Action OnPlayerDataChanged;
#pragma warning restore CS0067

        /// <summary>更新呼び出し時点のプレイヤー値のスナップショット</summary>
        public sealed record Call(string Method, emEntryStatus Entry, bool Staff, emExpStatus Exp, int Regulation);

        public List<Player> Players { get; } = new List<Player>();
        public List<Call> Calls { get; } = new List<Call>();

        /// <summary>更新の結果を決める処理（既定は即時成功）</summary>
        public Func<Task<bool>> UpdateResult { get; set; } = () => Task.FromResult(true);

        public List<Player> GetPlayers() => Players;

        private Task<bool> Record(string method, Player player)
        {
            Calls.Add(new Call(method, player.EntryStatus, player.StaffStatus, player.ExpStatus, player.RegulationStatus));
            return UpdateResult();
        }

        public Task<bool> UpdateEntryStatus(Player player) => Record(nameof(UpdateEntryStatus), player);
        public Task<bool> UpdateStaffStatus(Player player) => Record(nameof(UpdateStaffStatus), player);
        public Task<bool> UpdateExpStatus(Player player) => Record(nameof(UpdateExpStatus), player);
        public Task<bool> UpdateRegulationStatus(Player player) => Record(nameof(UpdateRegulationStatus), player);

        /// <summary>Insertされたプレイヤー（登録時点のレギュレーション値）</summary>
        public List<(string Name, int Regulation)> Inserted { get; } = new List<(string, int)>();

        public Task Insert(Player player) => Insert(new List<Player> { player });
        public Task Insert(List<Player> players)
        {
            foreach (var player in players)
            {
                Inserted.Add((player.Name, player.RegulationStatus));
                Players.Add(player);
            }
            return Task.CompletedTask;
        }
        public Task UpdateJoinStatus(List<Player> players) => Task.CompletedTask;
        public Task ResyncPlayer() => Task.CompletedTask;
        public Task SubscribeUpdates() => Task.CompletedTask;
        public Task Unsubscribe() => Task.CompletedTask;
    }
}
