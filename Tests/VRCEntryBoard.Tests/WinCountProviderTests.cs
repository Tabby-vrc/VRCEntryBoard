using System;
using System.Net;
using System.Threading.Tasks;
using VRCEntryBoard.Infra;
using VRCEntryBoard.App.Services;
using Xunit;

namespace VRCEntryBoard.Tests
{
    /// <summary>
    /// 優勝回数の取得（GistWinCountProvider）
    /// 偽のHTTPサーバーに向けて実行する
    /// </summary>
    public class WinCountProviderTests
    {
        private const string Json = @"[
            {""Name"":""champ"",""Count"":5,""Ranking"":1},
            {""Name"":""Twice"",""Count"":2,""Ranking"":2},
            {""Name"":""once"",""Count"":1,""Ranking"":3},
            {""Name"":""dup"",""Count"":1,""Ranking"":3},
            {""Name"":""dup"",""Count"":4,""Ranking"":2}
        ]";

        /// <summary>指定の応答を返すサーバーを立てて、処理を実行する</summary>
        private static async Task WithServer(int status, string body, Func<string, Task> test)
        {
            var port = new Random().Next(20000, 40000);
            var prefix = $"http://127.0.0.1:{port}/";
            using var listener = new HttpListener();
            listener.Prefixes.Add(prefix);
            listener.Start();
            var serve = Task.Run(async () =>
            {
                while (listener.IsListening)
                {
                    HttpListenerContext ctx;
                    try { ctx = await listener.GetContextAsync(); } catch { return; }
                    ctx.Response.StatusCode = status;
                    var bytes = System.Text.Encoding.UTF8.GetBytes(body);
                    ctx.Response.OutputStream.Write(bytes, 0, bytes.Length);
                    ctx.Response.Close();
                }
            });
            try { await test(prefix); }
            finally { listener.Stop(); }
        }

        [Fact]
        public async Task Load_ReadsWinCounts_CaseInsensitive_AndUsesMaxForDuplicates()
        {
            await WithServer(200, Json, async url =>
            {
                var provider = new GistWinCountProvider(url);
                await provider.LoadAsync();

                Assert.Equal(5, provider.GetWinCount("champ"));
                Assert.Equal(2, provider.GetWinCount("twice"));   // 大文字小文字は区別しない
                Assert.Equal(1, provider.GetWinCount("once"));
                Assert.Equal(4, provider.GetWinCount("dup"));     // 同名は大きい方
                Assert.Equal(0, provider.GetWinCount("unknown"));
                Assert.Equal(0, provider.GetWinCount(null));
            });
        }

        [Fact]
        public async Task Threshold_BoundaryIsTwoWins()
        {
            await WithServer(200, Json, async url =>
            {
                var provider = new GistWinCountProvider(url);
                await provider.LoadAsync();

                Assert.True(provider.GetWinCount("twice") >= VRCDataManagementService.Reg2WinCountThreshold);
                Assert.False(provider.GetWinCount("once") >= VRCDataManagementService.Reg2WinCountThreshold);
            });
        }

        [Fact]
        public async Task Load_ServerError_DoesNotThrow_AndTreatsAsZero()
        {
            await WithServer(500, "error", async url =>
            {
                var provider = new GistWinCountProvider(url);
                await provider.LoadAsync();
                Assert.Equal(0, provider.GetWinCount("champ"));
            });
        }

        [Fact]
        public async Task Load_InvalidJson_DoesNotThrow()
        {
            await WithServer(200, "not json", async url =>
            {
                var provider = new GistWinCountProvider(url);
                await provider.LoadAsync();
                Assert.Equal(0, provider.GetWinCount("champ"));
            });
        }

        [Fact]
        public async Task Load_RetriesOnNextCall_AfterFailure()
        {
            var port = new Random().Next(40001, 60000);
            var url = $"http://127.0.0.1:{port}/";
            var provider = new GistWinCountProvider(url);

            await provider.LoadAsync();                       // サーバー無し → 失敗
            Assert.Equal(0, provider.GetWinCount("champ"));

            using var listener = new HttpListener();
            listener.Prefixes.Add(url);
            listener.Start();
            var serve = Task.Run(async () =>
            {
                var ctx = await listener.GetContextAsync();
                var bytes = System.Text.Encoding.UTF8.GetBytes(Json);
                ctx.Response.OutputStream.Write(bytes, 0, bytes.Length);
                ctx.Response.Close();
            });
            await provider.LoadAsync();                       // 復旧後は読み込める
            Assert.Equal(5, provider.GetWinCount("champ"));
            await serve;
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task Load_UrlNotConfigured_DisablesFeature(string url)
        {
            var provider = new GistWinCountProvider(url);
            await provider.LoadAsync();
            Assert.Equal(0, provider.GetWinCount("champ"));
        }

        [Fact]
        public async Task Load_DummyUrlFromExampleConfig_DoesNotThrow()
        {
            var provider = new GistWinCountProvider("your-win-count-ranking-url");
            await provider.LoadAsync();
            Assert.Equal(0, provider.GetWinCount("champ"));
        }

        /// <summary>
        /// 実際のランキングの形式を読めること
        /// 環境変数 VRC_TEST_WIN_COUNT_URL と VRC_TEST_WIN_COUNT_NAME（優勝2回以上の名前）が設定されている場合のみ実行
        /// </summary>
        [SkippableFact]
        public async Task Load_RealRanking_ParsesTotalRanking()
        {
            var url = Environment.GetEnvironmentVariable("VRC_TEST_WIN_COUNT_URL");
            var name = Environment.GetEnvironmentVariable("VRC_TEST_WIN_COUNT_NAME");
            Skip.If(string.IsNullOrEmpty(url) || string.IsNullOrEmpty(name), "環境変数が未設定のためスキップ");

            var provider = new GistWinCountProvider(url);
            await provider.LoadAsync();
            Assert.True(provider.GetWinCount(name) >= 2);
        }
    }
}
