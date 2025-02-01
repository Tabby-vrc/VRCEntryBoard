using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRCEntryBoard.Domain.Interfaces
{
    internal interface IEntryRecorder
    {
        void Initialize();

        List<string> GetRecotdList();
    }
}
