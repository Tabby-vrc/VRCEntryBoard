using System;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

namespace VRCEntryBoard.Tests
{
    /// <summary>
    /// WinFormsコントロールを扱うテストをSTAスレッド上で実行する
    /// </summary>
    internal static class StaRunner
    {
        public static void Run(Func<Task> test)
        {
            Exception error = null;
            var thread = new Thread(() =>
            {
                try { test().GetAwaiter().GetResult(); }
                catch (Exception ex) { error = ex; }
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();
            if (error != null)
            {
                ExceptionDispatchInfo.Capture(error).Throw();
            }
        }
    }
}
