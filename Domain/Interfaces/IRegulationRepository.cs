using System.Collections.Generic;
using System.Threading.Tasks;

using VRCEntryBoard.Domain.Model;

namespace VRCEntryBoard.Domain.Interfaces
{
    internal interface IRegulationRepository
    {
        Task UpdateRegulations();
        List<Regulation> GetRegulations();
    }
}