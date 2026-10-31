using System.Threading.Tasks;
using VRCEntryBoard.Domain.Model;
using Xunit;

namespace VRCEntryBoard.Tests
{
    /// <summary>
    /// CEntryViewController のステータス更新（成功時／DB更新失敗時の巻き戻し）
    /// </summary>
    public class ControllerStatusUpdateTests
    {
        // ---------- エントリーステータス ----------

        [Fact]
        public void EntryStatus_Success_UpdatesPlayerCounterAndRepository() => StaRunner.Run(async () =>
        {
            var h = new ControllerHarness();

            bool ok = await h.Controller.UpdateEntryStatus("alice", emEntryStatus.Entry);

            Assert.True(ok);
            Assert.Equal(emEntryStatus.Entry, h.Alice.EntryStatus);
            Assert.Equal("1", h.Reg1Num);          // 参加 + レギュ1 としてカウントされる
            Assert.Equal("1", h.InstanceNum);
            var call = Assert.Single(h.Repo.Calls);
            Assert.Equal("UpdateEntryStatus", call.Method);
            Assert.Equal(emEntryStatus.Entry, call.Entry);  // DBには変更後の値が渡る
        });

        [Fact]
        public void EntryStatus_Failure_RevertsPlayerAndCounter() => StaRunner.Run(async () =>
        {
            var h = new ControllerHarness();
            h.Repo.UpdateResult = () => Task.FromResult(false);

            bool ok = await h.Controller.UpdateEntryStatus("alice", emEntryStatus.Entry);

            Assert.False(ok);
            Assert.Equal(emEntryStatus.AskMe, h.Alice.EntryStatus);  // 変更前(初期値)に戻る
            Assert.Equal("0", h.Reg1Num);
            Assert.Equal("0", h.InstanceNum);
        });

        [Fact]
        public void EntryStatus_ShowsNewValueWhileWaitingForDb() => StaRunner.Run(async () =>
        {
            var h = new ControllerHarness();
            var gate = new TaskCompletionSource<bool>();
            h.Repo.UpdateResult = () => gate.Task;

            var task = h.Controller.UpdateEntryStatus("alice", emEntryStatus.Entry);

            // DB応答待ちの間も画面(Player/カウンター)は新しい値を表示する
            Assert.False(task.IsCompleted);
            Assert.Equal(emEntryStatus.Entry, h.Alice.EntryStatus);
            Assert.Equal("1", h.Reg1Num);

            gate.SetResult(true);
            Assert.True(await task);
            Assert.Equal(emEntryStatus.Entry, h.Alice.EntryStatus);
        });

        [Fact]
        public void EntryStatus_FailureAfterNewerChange_DoesNotOverwriteNewerValue() => StaRunner.Run(async () =>
        {
            var h = new ControllerHarness();
            var firstGate = new TaskCompletionSource<bool>();
            h.Repo.UpdateResult = () => firstGate.Task;
            var first = h.Controller.UpdateEntryStatus("alice", emEntryStatus.Entry);   // 応答待ち

            // 待機中に別の変更(Visiter)が入り、それは成功する
            h.Repo.UpdateResult = () => Task.FromResult(true);
            Assert.True(await h.Controller.UpdateEntryStatus("alice", emEntryStatus.Visiter));

            // 最初の更新が失敗しても、新しい値(Visiter)は消さない
            firstGate.SetResult(false);
            Assert.False(await first);
            Assert.Equal(emEntryStatus.Visiter, h.Alice.EntryStatus);
        });

        // ---------- レギュレーション ----------

        [Fact]
        public void Regulation_Success_SwitchesRegulationAndClearsBeginner() => StaRunner.Run(async () =>
        {
            var h = new ControllerHarness();
            h.Alice.EntryStatus = emEntryStatus.Entry;
            h.Alice.ExpStatus = emExpStatus.Beginner;
            h.Alice.RegulationStatus = 0;

            bool ok = await h.Controller.UpdateRegulationStatus("alice", 2);

            Assert.True(ok);
            Assert.Equal(2, h.Alice.RegulationStatus);
            Assert.False(h.Alice.ExpStatus.HasFlag(emExpStatus.Beginner));
            Assert.Equal("1", h.Reg2Num);
            Assert.Equal("0", h.BeginnerNum);
            var call = Assert.Single(h.Repo.Calls);
            Assert.Equal("UpdateRegulationStatus", call.Method);
            Assert.Equal(2, call.Regulation);
            Assert.Equal(emExpStatus.None, call.Exp);
        });

        [Fact]
        public void Regulation_Failure_RestoresRegulationAndBeginner() => StaRunner.Run(async () =>
        {
            var h = new ControllerHarness();
            h.Alice.EntryStatus = emEntryStatus.Entry;
            h.Alice.ExpStatus = emExpStatus.Beginner;
            h.Alice.RegulationStatus = 0;
            h.Repo.UpdateResult = () => Task.FromResult(false);

            bool ok = await h.Controller.UpdateRegulationStatus("alice", 2);

            Assert.False(ok);
            Assert.Equal(0, h.Alice.RegulationStatus);
            Assert.True(h.Alice.ExpStatus.HasFlag(emExpStatus.Beginner));  // 初心者フラグも戻る
            Assert.Equal("0", h.Reg2Num);
            Assert.Equal("1", h.BeginnerNum);
        });

        // ---------- 初心者 ----------

        [Fact]
        public void Beginner_Success_SetsFlagAndResetsRegulation() => StaRunner.Run(async () =>
        {
            var h = new ControllerHarness();
            h.Alice.EntryStatus = emEntryStatus.Entry;   // レギュ1で参加中

            bool ok = await h.Controller.UpdateBeginnerStatus("alice", true);

            Assert.True(ok);
            Assert.True(h.Alice.ExpStatus.HasFlag(emExpStatus.Beginner));
            Assert.Equal(0, h.Alice.RegulationStatus);
            Assert.Equal("1", h.BeginnerNum);
            Assert.Equal("0", h.Reg1Num);
            var call = Assert.Single(h.Repo.Calls);
            Assert.Equal("UpdateExpStatus", call.Method);
            Assert.Equal(emExpStatus.Beginner, call.Exp);
            Assert.Equal(0, call.Regulation);
        });

        [Fact]
        public void Beginner_Failure_RestoresFlagAndRegulation() => StaRunner.Run(async () =>
        {
            var h = new ControllerHarness();
            h.Alice.EntryStatus = emEntryStatus.Entry;
            h.Repo.UpdateResult = () => Task.FromResult(false);

            bool ok = await h.Controller.UpdateBeginnerStatus("alice", true);

            Assert.False(ok);
            Assert.False(h.Alice.ExpStatus.HasFlag(emExpStatus.Beginner));
            Assert.Equal(1, h.Alice.RegulationStatus);   // 変更前のレギュ1に戻る
            Assert.Equal("0", h.BeginnerNum);
            Assert.Equal("1", h.Reg1Num);
        });

        // ---------- スタッフ ----------

        [Fact]
        public void Staff_Success_UpdatesPlayerCounterAndRepository() => StaRunner.Run(async () =>
        {
            var h = new ControllerHarness();

            bool ok = await h.Controller.UpdateStaff("alice", true);

            Assert.True(ok);
            Assert.True(h.Alice.StaffStatus);
            Assert.Equal("1", h.StaffNum);
            var call = Assert.Single(h.Repo.Calls);
            Assert.Equal("UpdateStaffStatus", call.Method);
            Assert.True(call.Staff);
        });

        [Fact]
        public void Staff_Failure_Reverts() => StaRunner.Run(async () =>
        {
            var h = new ControllerHarness();
            h.Repo.UpdateResult = () => Task.FromResult(false);

            bool ok = await h.Controller.UpdateStaff("alice", true);

            Assert.False(ok);
            Assert.False(h.Alice.StaffStatus);
            Assert.Equal("0", h.StaffNum);
        });

        // ---------- 共通 ----------

        [Fact]
        public void UnknownPlayer_ReturnsFalseWithoutCallingRepository() => StaRunner.Run(async () =>
        {
            var h = new ControllerHarness();

            Assert.False(await h.Controller.UpdateEntryStatus("nobody", emEntryStatus.Entry));
            Assert.False(await h.Controller.UpdateRegulationStatus("nobody", 2));
            Assert.False(await h.Controller.UpdateBeginnerStatus("nobody", true));
            Assert.False(await h.Controller.UpdateStaff("nobody", true));
            Assert.Empty(h.Repo.Calls);
        });
    }
}
