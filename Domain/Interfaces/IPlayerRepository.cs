using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRCEntryBoard.Domain.Model;

namespace VRCEntryBoard.Domain.Interfaces
{
    internal interface IPlayerRepository
    {
        event Action OnPlayerDataChanged;
        List<Player> GetPlayers();
        Task Insert(Player player);
        Task Insert(List<Player> players);
        Task<bool> UpdateEntryStatus(Player player);
        Task<bool> UpdateStaffStatus(Player player);
        Task<bool> UpdateExpStatus(Player player);
        Task<bool> UpdateRegulationStatus(Player player);
        Task UpdateJoinStatus(List<Player> players);
        Task ResyncPlayer();
        Task SubscribeUpdates();
        Task Unsubscribe();
    }
}
