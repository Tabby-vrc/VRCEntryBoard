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
        List<Player> GetPlayers();
        Task Insert(Player player);
        Task Insert(List<Player> players);
        Task UpdateEntryStatus(Player player);
        Task UpdateStaffStatus(Player player);
        Task UpdateExpStatus(Player player);
        Task UpdateRegulationStatus(Player player);
        Task UpdateJoinStatus(List<Player> players);
        Task ResyncPlayer();
        Task SubscribeUpdates();
    }
}
