using System;
using System.Collections.Concurrent;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

namespace VRCEntryBoard.Tests
{
    /// <summary>
    /// WinFormsコントロールを扱うテストをSTAスレッド上で実行する
    /// await の続きもUIスレッド（このSTAスレッド）で実行されるよう、簡易のメッセージポンプを持つ
    /// </summary>
    internal static class StaRunner
    {
        static StaRunner()
        {
            // コントロールを作ると同期コンテキストが自動で入る仕組みを止め、下記の自前のものを使う
            System.Windows.Forms.WindowsFormsSynchronizationContext.AutoInstall = false;
        }

        /// <summary>1つのスレッドへ処理を順番に流す同期コンテキスト</summary>
        private sealed class PumpContext : SynchronizationContext
        {
            private readonly BlockingCollection<(SendOrPostCallback Callback, object State)> _queue = new();

            public override void Post(SendOrPostCallback d, object state) => _queue.Add((d, state));

            public void Complete() => _queue.CompleteAdding();

            /// <summary>Completeされ、キューが空になるまで処理を実行し続ける</summary>
            public void RunLoop()
            {
                foreach (var (callback, state) in _queue.GetConsumingEnumerable())
                {
                    callback(state);
                }
            }
        }

        public static void Run(Func<Task> test)
        {
            Exception error = null;
            var thread = new Thread(() =>
            {
                try
                {
                    var context = new PumpContext();
                    SynchronizationContext.SetSynchronizationContext(context);

                    var task = test();
                    task.ContinueWith(_ => context.Complete(), TaskScheduler.Default);
                    context.RunLoop();

                    task.GetAwaiter().GetResult();
                }
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
