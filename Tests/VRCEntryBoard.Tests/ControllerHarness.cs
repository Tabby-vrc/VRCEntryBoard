using System.Linq;
using System.Windows.Forms;
using VRCEntryBoard.App.Controller;
using VRCEntryBoard.Domain.Model;
using VRCEntryBoard.HMI;

namespace VRCEntryBoard.Tests
{
    /// <summary>
    /// Controller・View・Fakeリポジトリを組み合わせたテスト用の土台
    /// </summary>
    internal sealed class ControllerHarness
    {
        public FakePlayerRepository Repo { get; } = new FakePlayerRepository();
        public CEntryViewController Controller { get; }
        public CEntryView View { get; }
        public Player Alice { get; }

        public ControllerHarness()
        {
            // VRCDataManagementService / IRegulationRepository はステータス更新では使われない
            Controller = new CEntryViewController(null, Repo, null);
            View = new CEntryView(Controller);

            Alice = new Player("alice") { ID = 1 };
            Repo.Players.Add(Alice);
        }

        private string Label(string name) => ((Label)View.Controls.Find(name, true).Single()).Text;

        public string Reg1Num => Label("lblReg1Num");
        public string Reg2Num => Label("lblReg2Num");
        public string BeginnerNum => Label("lblBeginnerNum");
        public string StaffNum => Label("lblStaffNum");
        public string InstanceNum => Label("lblInstanceNum");
    }
}
