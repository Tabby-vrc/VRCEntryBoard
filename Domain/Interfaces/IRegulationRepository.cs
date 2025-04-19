using System.Collections.Generic;

using VRCEntryBoard.Domain.Model;

namespace VRCEntryBoard.Domain.Interfaces
{
    internal interface IRegulationRepository
    {
        List<Regulation> GetRegulations();
    }
}