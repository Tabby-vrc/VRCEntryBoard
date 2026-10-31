using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using VRCEntryBoard.Domain.Model;
using VRCEntryBoard.HMI;
using VRCEntryBoard.HMI.CustomUserControl;
using Xunit;

namespace VRCEntryBoard.Tests
{
    /// <summary>
    /// CEntryView のステータス操作（実際のクリックハンドラ経由）
    /// DB更新失敗時に、右側のラジオボタン/チェックボックスが元の値に戻ることを確認する
    /// </summary>
    public class ViewStatusUpdateTests
    {
        private static T Find<T>(CEntryView view, string name) where T : Control
            => (T)view.Controls.Find(name, true).Single();

        /// <summary>プレイヤーを一覧に表示し、指定プレイヤーのパネルを選択した状態にする</summary>
        private static void SelectAlice(ControllerHarness h)
        {
            h.View.UpdatePlayerList(h.Repo.Players);
            var panel = (PlayerPanel)Find<FlowLayoutPanel>(h.View, "pnlPlayerList").Controls[0];
            Invoke(h.View, "PlayerPanel_Click", panel, new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
        }

        /// <summary>privateなイベントハンドラを呼ぶ（async voidだが、待機先が即時完了するため同期的に完了する）</summary>
        private static void Invoke(CEntryView view, string method, object sender, EventArgs e)
        {
            var m = typeof(CEntryView).GetMethod(method, BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.NotNull(m);
            m.Invoke(view, new[] { sender, e });
        }

        [Fact]
        public void ClickEntry_Success_KeepsRadioAndUpdatesStatus() => StaRunner.Run(() =>
        {
            var h = new ControllerHarness();
            SelectAlice(h);
            Assert.True(Find<RadioButton>(h.View, "rBtnAskMe").Checked);   // 選択直後はAskMe

            var rBtnEntry = Find<RadioButton>(h.View, "rBtnEntry");
            rBtnEntry.Checked = true;                                        // ユーザーがクリックした状態
            Invoke(h.View, "rBtnEntry_Click", rBtnEntry, EventArgs.Empty);

            Assert.Equal(emEntryStatus.Entry, h.Alice.EntryStatus);
            Assert.True(rBtnEntry.Checked);
            Assert.Equal("1", h.Reg1Num);
            return Task.CompletedTask;
        });

        [Fact]
        public void ClickEntry_Failure_RadioReturnsToPreviousValue() => StaRunner.Run(() =>
        {
            var h = new ControllerHarness();
            SelectAlice(h);
            h.Repo.UpdateResult = () => Task.FromResult(false);

            var rBtnEntry = Find<RadioButton>(h.View, "rBtnEntry");
            rBtnEntry.Checked = true;
            Invoke(h.View, "rBtnEntry_Click", rBtnEntry, EventArgs.Empty);

            Assert.Equal(emEntryStatus.AskMe, h.Alice.EntryStatus);
            Assert.False(rBtnEntry.Checked);
            Assert.True(Find<RadioButton>(h.View, "rBtnAskMe").Checked);
            Assert.Equal("0", h.Reg1Num);
            return Task.CompletedTask;
        });

        [Fact]
        public void ClickReg2_Failure_RadioReturnsToReg1() => StaRunner.Run(() =>
        {
            var h = new ControllerHarness();
            SelectAlice(h);
            Assert.True(Find<RadioButton>(h.View, "rBtnReg1").Checked);
            h.Repo.UpdateResult = () => Task.FromResult(false);

            var rBtnReg2 = Find<RadioButton>(h.View, "rBtnReg2");
            rBtnReg2.Checked = true;
            Invoke(h.View, "rBtnReg2_Click", rBtnReg2, EventArgs.Empty);

            Assert.Equal(1, h.Alice.RegulationStatus);
            Assert.False(rBtnReg2.Checked);
            Assert.True(Find<RadioButton>(h.View, "rBtnReg1").Checked);
            return Task.CompletedTask;
        });

        [Fact]
        public void ClickBeginner_Failure_RadioReturnsToReg1() => StaRunner.Run(() =>
        {
            var h = new ControllerHarness();
            SelectAlice(h);
            h.Repo.UpdateResult = () => Task.FromResult(false);

            var rBtnBeginner = Find<RadioButton>(h.View, "rBtnBeginner");
            rBtnBeginner.Checked = true;
            Invoke(h.View, "rBtnBeginner_Click", rBtnBeginner, EventArgs.Empty);

            Assert.False(h.Alice.ExpStatus.HasFlag(emExpStatus.Beginner));
            Assert.False(rBtnBeginner.Checked);
            Assert.True(Find<RadioButton>(h.View, "rBtnReg1").Checked);
            return Task.CompletedTask;
        });

        [Fact]
        public void ClickStaff_Success_StaysChecked() => StaRunner.Run(() =>
        {
            var h = new ControllerHarness();
            SelectAlice(h);

            var chk = Find<CheckBox>(h.View, "chkBoxStaff");
            chk.Checked = true;
            Invoke(h.View, "chkBoxStaff_Click", chk, EventArgs.Empty);

            Assert.True(h.Alice.StaffStatus);
            Assert.True(chk.Checked);
            Assert.Equal("1", h.StaffNum);
            return Task.CompletedTask;
        });

        [Fact]
        public void ClickStaff_Failure_CheckBoxReturnsToUnchecked() => StaRunner.Run(() =>
        {
            var h = new ControllerHarness();
            SelectAlice(h);
            h.Repo.UpdateResult = () => Task.FromResult(false);

            var chk = Find<CheckBox>(h.View, "chkBoxStaff");
            chk.Checked = true;
            Invoke(h.View, "chkBoxStaff_Click", chk, EventArgs.Empty);

            Assert.False(h.Alice.StaffStatus);
            Assert.False(chk.Checked);
            Assert.Equal("0", h.StaffNum);
            return Task.CompletedTask;
        });

        [Fact]
        public void Failure_AfterSelectionChanged_DoesNotTouchOtherPlayersUi() => StaRunner.Run(() =>
        {
            var h = new ControllerHarness();
            var bob = new Player("bob") { ID = 2 };
            bob.EntryStatus = emEntryStatus.Visiter;
            h.Repo.Players.Add(bob);
            h.View.UpdatePlayerList(h.Repo.Players);
            var panels = Find<FlowLayoutPanel>(h.View, "pnlPlayerList").Controls.Cast<PlayerPanel>().ToList();
            var alicePanel = panels.Single(p => p.PlayerName == "alice");
            var bobPanel = panels.Single(p => p.PlayerName == "bob");

            Invoke(h.View, "PlayerPanel_Click", alicePanel, new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));

            // aliceの更新を応答待ちにし、その間にbobを選択する
            var gate = new TaskCompletionSource<bool>();
            h.Repo.UpdateResult = () => gate.Task;
            var rBtnEntry = Find<RadioButton>(h.View, "rBtnEntry");
            rBtnEntry.Checked = true;
            Invoke(h.View, "rBtnEntry_Click", rBtnEntry, EventArgs.Empty);   // 応答待ちで一旦戻る
            Invoke(h.View, "PlayerPanel_Click", bobPanel, new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
            Assert.True(Find<RadioButton>(h.View, "rBtnVisiter").Checked);   // bobはVisiter

            // aliceの更新が失敗 → aliceの値は戻るが、bob表示中のUIは変えない
            gate.SetResult(false);

            Assert.Equal(emEntryStatus.AskMe, h.Alice.EntryStatus);
            Assert.True(Find<RadioButton>(h.View, "rBtnVisiter").Checked);
            return Task.CompletedTask;
        });
    }
}
