using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using VRCEntryBoard.Domain.Model;
using VRCEntryBoard.HMI.Exception;
using VRCEntryBoard.Infra;
using VRCEntryBoard.Infra.PlayerRepository;
using Xunit;

namespace VRCEntryBoard.Tests
{
    /// <summary>
    /// SupabasePlayerRepository のテスト
    /// ・実DB系: 環境変数 VRC_TEST_SUPABASE_URL / VRC_TEST_SUPABASE_KEY が設定されている場合のみ実行（テスト用DB専用）
    /// ・再試行系: ローカルの偽サーバーに対して実行するため常に実行される
    /// 設定ファイルをテスト出力フォルダーに書き換えるため、クラス内のテストは直列実行される
    /// </summary>
    public class SupabaseRepositoryTests
    {
        private const string UrlEnv = "VRC_TEST_SUPABASE_URL";
        private const string KeyEnv = "VRC_TEST_SUPABASE_KEY";

        private static string TestUrl => Environment.GetEnvironmentVariable(UrlEnv);
        private static string TestKey => Environment.GetEnvironmentVariable(KeyEnv);
        private static readonly HttpClient Http = new HttpClient();

        private sealed class NullNotifier : IExceptionNotifier
        {
            public void NotifyFatalError(string title, string message, System.Exception ex) { }
            public bool NotifyRecoverableError(string title, string message, System.Exception ex) => true;
        }

        /// <summary>指定の接続先で本物のリポジトリを作る（接続先は設定ファイル経由で渡される）</summary>
        private static SupabasePlayerRepository CreateRepository(string url, string key)
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "VRCEntryBoard-config.json");
            File.WriteAllText(path, JsonSerializer.Serialize(new
            {
                Url = url,
                Key = key,
                MonitoredWorldNames = new[] { "test-world" }
            }));
            var notifier = new NullNotifier();
            return new SupabasePlayerRepository(new SupabaseClient(notifier), notifier);
        }

        // ---------- 実DB（テスト用DB）で確認する ----------

        private static void RequireTestDb() =>
            Skip.If(string.IsNullOrEmpty(TestUrl) || string.IsNullOrEmpty(TestKey),
                $"{UrlEnv} / {KeyEnv} が未設定のためスキップ");

        private static HttpRequestMessage Request(HttpMethod method, string pathAndQuery, string json = null)
        {
            var req = new HttpRequestMessage(method, $"{TestUrl}/rest/v1/{pathAndQuery}");
            req.Headers.Add("apikey", TestKey);
            req.Headers.Add("Authorization", "Bearer " + TestKey);
            if (json != null) req.Content = new StringContent(json, Encoding.UTF8, "application/json");
            return req;
        }

        /// <summary>リポジトリを介さず、DBに保存されている行を直接読む</summary>
        private static async Task<JsonElement> ReadRow(string name)
        {
            var res = await Http.SendAsync(Request(HttpMethod.Get, $"Player?Name=eq.{name}"));
            res.EnsureSuccessStatusCode();
            using var doc = JsonDocument.Parse(await res.Content.ReadAsStringAsync());
            Assert.Equal(1, doc.RootElement.GetArrayLength());
            return doc.RootElement[0].Clone();
        }

        private static async Task DirectPatch(int id, string json)
        {
            var res = await Http.SendAsync(Request(new HttpMethod("PATCH"), $"Player?ID=eq.{id}", json));
            res.EnsureSuccessStatusCode();
        }

        private static string NewName() => "itest-" + Guid.NewGuid().ToString("N").Substring(0, 12);

        private static async Task<(SupabasePlayerRepository repo, Player player)> InsertNewPlayer()
        {
            var repo = CreateRepository(TestUrl, TestKey);
            var player = new Player(NewName());
            await repo.Insert(player);
            Assert.True(player.ID > 0, "Insert後にIDが採番されること");
            return (repo, player);
        }

        [SkippableFact]
        public async Task Insert_PersistsPlayer()
        {
            RequireTestDb();
            var (_, player) = await InsertNewPlayer();

            var row = await ReadRow(player.Name);
            Assert.Equal(player.ID, row.GetProperty("ID").GetInt32());
            Assert.Equal((int)emEntryStatus.AskMe, row.GetProperty("EntryStatus").GetInt32());
        }

        [SkippableFact]
        public async Task UpdateEntryStatus_Persists_AndLeavesOtherColumnsUntouched()
        {
            RequireTestDb();
            var (repo, player) = await InsertNewPlayer();

            // 他のPC等でスタッフ=trueに更新された状態を作る（このPCのplayerオブジェクトは古いまま）
            await DirectPatch(player.ID, "{\"StaffStatus\":true,\"RegulationStatus\":2}");

            player.EntryStatus = emEntryStatus.Entry;
            Assert.True(await repo.UpdateEntryStatus(player));

            var row = await ReadRow(player.Name);
            Assert.Equal((int)emEntryStatus.Entry, row.GetProperty("EntryStatus").GetInt32());
            Assert.True(row.GetProperty("StaffStatus").GetBoolean());           // 古い値で上書きされない
            Assert.Equal(2, row.GetProperty("RegulationStatus").GetInt32());    // 同上
        }

        [SkippableFact]
        public async Task UpdateStaffStatus_Persists()
        {
            RequireTestDb();
            var (repo, player) = await InsertNewPlayer();

            player.StaffStatus = true;
            Assert.True(await repo.UpdateStaffStatus(player));
            Assert.True((await ReadRow(player.Name)).GetProperty("StaffStatus").GetBoolean());

            player.StaffStatus = false;
            Assert.True(await repo.UpdateStaffStatus(player));
            Assert.False((await ReadRow(player.Name)).GetProperty("StaffStatus").GetBoolean());
        }

        [SkippableFact]
        public async Task UpdateExpStatus_Persists_ExpAndRegulation()
        {
            RequireTestDb();
            var (repo, player) = await InsertNewPlayer();

            player.ExpStatus = emExpStatus.Beginner;
            player.RegulationStatus = 0;
            Assert.True(await repo.UpdateExpStatus(player));

            var row = await ReadRow(player.Name);
            Assert.Equal((int)emExpStatus.Beginner, row.GetProperty("ExpStatus").GetInt32());
            Assert.Equal(0, row.GetProperty("RegulationStatus").GetInt32());
        }

        [SkippableFact]
        public async Task UpdateRegulationStatus_Persists_ExpAndRegulation()
        {
            RequireTestDb();
            var (repo, player) = await InsertNewPlayer();

            player.ExpStatus = emExpStatus.None;
            player.RegulationStatus = 2;
            Assert.True(await repo.UpdateRegulationStatus(player));

            var row = await ReadRow(player.Name);
            Assert.Equal(2, row.GetProperty("RegulationStatus").GetInt32());
            Assert.Equal(0, row.GetProperty("ExpStatus").GetInt32());
        }

        [SkippableFact]
        public async Task UpdateJoinStatus_PersistsForEveryPlayer()
        {
            RequireTestDb();
            var (repo, p1) = await InsertNewPlayer();
            var p2 = new Player(NewName());
            await repo.Insert(p2);

            p1.JoinStatus = false;
            p2.JoinStatus = true;
            await repo.UpdateJoinStatus(new List<Player> { p1, p2 });

            Assert.False((await ReadRow(p1.Name)).GetProperty("JoinStatus").GetBoolean());
            Assert.True((await ReadRow(p2.Name)).GetProperty("JoinStatus").GetBoolean());
        }

        [SkippableFact]
        public async Task Update_ForNonexistentId_ReturnsTrue_AndDoesNotThrow()
        {
            RequireTestDb();
            var repo = CreateRepository(TestUrl, TestKey);
            var ghost = new Player("itest-ghost") { ID = 987654321 };

            // 該当行が無くてもSupabaseはエラーを返さない（0件更新）。例外にならず成功扱いになる
            Assert.True(await repo.UpdateEntryStatus(ghost));
        }

        // ---------- 偽サーバーで再試行を確認する（常に実行） ----------

        /// <summary>先頭 failCount 回は500を返し、その後は成功(200 [])を返す偽のPostgREST</summary>
        private sealed class FlakyServer : IDisposable
        {
            private readonly HttpListener _listener = new HttpListener();
            private readonly int _failCount;
            private int _requests;
            public string Url { get; }
            public int Requests => Volatile.Read(ref _requests);

            public FlakyServer(int failCount)
            {
                _failCount = failCount;
                var port = GetFreePort();
                Url = $"http://127.0.0.1:{port}";
                _listener.Prefixes.Add(Url + "/");
                _listener.Start();
                _ = Task.Run(Loop);
            }

            private static int GetFreePort()
            {
                var l = new System.Net.Sockets.TcpListener(IPAddress.Loopback, 0);
                l.Start();
                int port = ((IPEndPoint)l.LocalEndpoint).Port;
                l.Stop();
                return port;
            }

            private async Task Loop()
            {
                while (_listener.IsListening)
                {
                    HttpListenerContext ctx;
                    try { ctx = await _listener.GetContextAsync(); }
                    catch { return; }

                    int n = Interlocked.Increment(ref _requests);
                    bool fail = n <= _failCount;
                    ctx.Response.StatusCode = fail ? 500 : 200;
                    ctx.Response.ContentType = "application/json";
                    var body = Encoding.UTF8.GetBytes(fail ? "{\"message\":\"boom\"}" : "[]");
                    await ctx.Response.OutputStream.WriteAsync(body, 0, body.Length);
                    ctx.Response.Close();
                }
            }

            public void Dispose() => _listener.Close();
        }

        private static Player SamplePlayer() => new Player("retry-test") { ID = 1, EntryStatus = emEntryStatus.Entry };

        [Fact]
        public async Task Update_SucceedsFirstTime_SendsOneRequest()
        {
            using var server = new FlakyServer(failCount: 0);
            var repo = CreateRepository(server.Url, "dummy-key");

            Assert.True(await repo.UpdateEntryStatus(SamplePlayer()));
            Assert.Equal(1, server.Requests);
        }

        [Fact]
        public async Task Update_FailsTwiceThenSucceeds_RetriesAndReturnsTrue()
        {
            using var server = new FlakyServer(failCount: 2);
            var repo = CreateRepository(server.Url, "dummy-key");

            var sw = Stopwatch.StartNew();
            bool ok = await repo.UpdateEntryStatus(SamplePlayer());
            sw.Stop();

            Assert.True(ok);
            Assert.Equal(3, server.Requests);                           // 初回 + 再試行2回
            Assert.True(sw.ElapsedMilliseconds >= 1400, $"待機時間が短すぎる: {sw.ElapsedMilliseconds}ms");  // 500ms + 1000ms
        }

        [Fact]
        public async Task Update_AlwaysFails_GivesUpAfterThreeAttemptsAndReturnsFalse()
        {
            using var server = new FlakyServer(failCount: int.MaxValue);
            var repo = CreateRepository(server.Url, "dummy-key");

            bool ok = await repo.UpdateEntryStatus(SamplePlayer());

            Assert.False(ok);
            Assert.Equal(3, server.Requests);                           // 無限に再試行しない
        }

        [Fact]
        public async Task Update_ServerUnreachable_ReturnsFalseWithoutThrowing()
        {
            // 待ち受けのないポート(接続拒否)
            int closedPort;
            var l = new System.Net.Sockets.TcpListener(IPAddress.Loopback, 0);
            l.Start();
            closedPort = ((IPEndPoint)l.LocalEndpoint).Port;
            l.Stop();
            var repo = CreateRepository($"http://127.0.0.1:{closedPort}", "dummy-key");

            Assert.False(await repo.UpdateStaffStatus(SamplePlayer()));
            Assert.False(await repo.UpdateExpStatus(SamplePlayer()));
            Assert.False(await repo.UpdateRegulationStatus(SamplePlayer()));
        }

        [Fact]
        public async Task Update_NullPlayer_ReturnsFalse()
        {
            using var server = new FlakyServer(failCount: 0);
            var repo = CreateRepository(server.Url, "dummy-key");

            Assert.False(await repo.UpdateEntryStatus(null));
            Assert.Equal(0, server.Requests);
        }
    }
}
