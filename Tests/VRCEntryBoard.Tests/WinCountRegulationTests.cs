using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VRCEntryBoard.App.Controller;
using VRCEntryBoard.App.Services;
using VRCEntryBoard.Domain.Interfaces;
using VRCEntryBoard.Domain.Model;
using VRCEntryBoard.HMI;
using VRCEntryBoard.Infra;
using VRCEntryBoard.Infra.VRChat;
using Xunit;

namespace VRCEntryBoard.Tests
{
    /// <summary>
    /// 優勝回数に応じた初期レギュレーションの設定
    /// （ログからの新規プレイヤー作成 → レギュレーション決定 → 登録、および起動時の読み込み）
    /// </summary>
    public class WinCountRegulationTests
    {
        private const string World = "test-world";

        private const string RankingJson = @"[
            {""Name"":""champ"",""Count"":5,""Ranking"":1},
            {""Name"":""twice"",""Count"":2,""Ranking"":2},
            {""Name"":""once"",""Count"":1,""Ranking"":3}
        ]";

        // ---- テスト用の部品 ----

        /// <summary>返すイベントを差し替えられるログローダーのFake</summary>
        private sealed class FakeLoader : IVRCDataLoder
        {
            public List<VRChatLogFileModel> Events { get; set; } = new List<VRChatLogFileModel>();
            public List<VRChatLogFileModel> GetVRCData() => Events;
        }

        private sealed class FakeRegulationRepository : IRegulationRepository
        {
            public Task UpdateRegulations() => Task.CompletedTask;
            public List<Regulation> GetRegulations() => new List<Regulation>
            {
                new Regulation(1, "reg1"), new Regulation(2, "reg2")
            };
        }

        /// <summary>優勝回数を固定値で返すFake。LoadAsyncの呼び出し回数を数え、完了を制御できる</summary>
        private sealed class FakeWinCountProvider : IWinCountProvider
        {
            private readonly Dictionary<string, int> _counts;
            public int LoadCalls;
            public Task Gate { get; set; } = Task.CompletedTask;

            public FakeWinCountProvider(Dictionary<string, int> counts) { _counts = counts; }

            public async Task LoadAsync()
            {
                Interlocked.Increment(ref LoadCalls);
                await Gate;
            }

            public int GetWinCount(string playerName) =>
                _counts.TryGetValue(playerName, out var c) ? c : 0;
        }

        /// <summary>リクエスト回数を数える偽サーバー</summary>
        private sealed class CountingServer : IDisposable
        {
            private readonly HttpListener _listener = new HttpListener();
            public string Url { get; }
            public int Requests;

            public CountingServer(int status, string body)
            {
                var port = new Random().Next(20000, 60000);
                Url = $"http://127.0.0.1:{port}/";
                _listener.Prefixes.Add(Url);
                _listener.Start();
                Task.Run(async () =>
                {
                    while (_listener.IsListening)
                    {
                        HttpListenerContext ctx;
                        try { ctx = await _listener.GetContextAsync(); } catch { return; }
                        Interlocked.Increment(ref Requests);
                        ctx.Response.StatusCode = status;
                        var bytes = Encoding.UTF8.GetBytes(body);
                        ctx.Response.OutputStream.Write(bytes, 0, bytes.Length);
                        ctx.Response.Close();
                    }
                });
            }

            public void Dispose() => _listener.Stop();
        }

        private static VRChatLogFileModel WorldMove() =>
            new VRChatLogFileModel { EventType = EventType.WorldMove, WorldName = World };

        private static VRChatLogFileModel Join(string name) =>
            new VRChatLogFileModel { EventType = EventType.PlayerJoin, PlayerName = name };

        private static VRCDataManagementService CreateService(
            FakeLoader loader, FakePlayerRepository repo, IWinCountProvider provider) =>
            new VRCDataManagementService(loader, repo, provider, new List<string> { World });

        // ---- サービス単体（レギュレーションの決まり方） ----

        [Fact]
        public async Task NewPlayers_GetRegulationByWinCount_BoundaryIsTwo()
        {
            var repo = new FakePlayerRepository();
            var loader = new FakeLoader
            {
                Events = { WorldMove(), Join("champ"), Join("twice"), Join("once"), Join("nobody") }
            };
            var provider = new FakeWinCountProvider(new Dictionary<string, int>
            {
                ["champ"] = 5, ["twice"] = 2, ["once"] = 1
            });

            await CreateService(loader, repo, provider).UpdatePlayerList();

            Assert.Equal(2, repo.Inserted.Single(p => p.Name == "champ").Regulation);
            Assert.Equal(2, repo.Inserted.Single(p => p.Name == "twice").Regulation);   // ちょうど2回は対象
            Assert.Equal(1, repo.Inserted.Single(p => p.Name == "once").Regulation);    // 1回は対象外
            Assert.Equal(1, repo.Inserted.Single(p => p.Name == "nobody").Regulation);  // 未登録は1
        }

        [Fact]
        public async Task ExistingPlayer_IsNotChanged_EvenWithManyWins()
        {
            var repo = new FakePlayerRepository();
            repo.Players.Add(new Player("champ") { ID = 1, RegulationStatus = 1 });   // 手動で1にしている想定
            var loader = new FakeLoader { Events = { WorldMove(), Join("champ") } };
            var provider = new FakeWinCountProvider(new Dictionary<string, int> { ["champ"] = 5 });

            await CreateService(loader, repo, provider).UpdatePlayerList();

            Assert.Empty(repo.Inserted);
            Assert.Equal(1, repo.Players.Single().RegulationStatus);
        }

        [Fact]
        public async Task NoNewPlayers_DoesNotTouchWinCountProvider()
        {
            var repo = new FakePlayerRepository();
            repo.Players.Add(new Player("champ") { ID = 1 });
            var loader = new FakeLoader { Events = { WorldMove(), Join("champ") } };
            var provider = new FakeWinCountProvider(new Dictionary<string, int>());

            await CreateService(loader, repo, provider).UpdatePlayerList();

            Assert.Equal(0, provider.LoadCalls);
        }

        [Fact]
        public async Task NotMonitoredWorld_PlayersAreNotCreated()
        {
            var repo = new FakePlayerRepository();
            var loader = new FakeLoader
            {
                Events = { new VRChatLogFileModel { EventType = EventType.WorldMove, WorldName = "other" }, Join("champ") }
            };
            var provider = new FakeWinCountProvider(new Dictionary<string, int> { ["champ"] = 5 });

            await CreateService(loader, repo, provider).UpdatePlayerList();

            Assert.Empty(repo.Inserted);
        }

        [Fact]
        public async Task GistUnavailable_NewPlayersFallBackToRegulation1()
        {
            using var server = new CountingServer(500, "error");
            var repo = new FakePlayerRepository();
            var loader = new FakeLoader { Events = { WorldMove(), Join("champ") } };

            await CreateService(loader, repo, new GistWinCountProvider(server.Url)).UpdatePlayerList();

            Assert.Equal(1, repo.Inserted.Single().Regulation);
        }

        // ---- 起動時の読み込み（InitView経由） ----

        private static CEntryViewController CreateController(
            FakeLoader loader, FakePlayerRepository repo, IWinCountProvider provider, out CEntryView view)
        {
            var service = CreateService(loader, repo, provider);
            var controller = new CEntryViewController(service, repo, new FakeRegulationRepository());
            view = new CEntryView(controller);
            return controller;
        }

        [Fact]
        public void InitView_LoadsRankingOnce_AndNewPlayersGetRegulation2() => StaRunner.Run(async () =>
        {
            using var server = new CountingServer(200, RankingJson);
            var repo = new FakePlayerRepository();
            var loader = new FakeLoader { Events = { WorldMove(), Join("champ"), Join("once") } };
            var controller = CreateController(loader, repo, new GistWinCountProvider(server.Url), out _);

            await controller.InitView();

            Assert.Equal(2, repo.Players.Single(p => p.Name == "champ").RegulationStatus);
            Assert.Equal(1, repo.Players.Single(p => p.Name == "once").RegulationStatus);
            Assert.Equal(1, server.Requests);

            // 更新ボタン相当を繰り返し、新規プレイヤーが増えても再取得しない（キャッシュ）
            loader.Events.Add(Join("twice"));
            await controller.UpdatePlayerList();
            await controller.UpdatePlayerList();

            Assert.Equal(2, repo.Players.Single(p => p.Name == "twice").RegulationStatus);
            Assert.Equal(1, server.Requests);
        });

        [Fact]
        public void InitView_WaitsForRankingBeforeCreatingPlayers() => StaRunner.Run(async () =>
        {
            var gate = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            var provider = new FakeWinCountProvider(new Dictionary<string, int> { ["champ"] = 5 })
            {
                Gate = gate.Task
            };
            var repo = new FakePlayerRepository();
            var loader = new FakeLoader { Events = { WorldMove(), Join("champ") } };
            var controller = CreateController(loader, repo, provider, out _);

            var init = controller.InitView();
            await Task.Delay(300);

            // 読み込み中は、プレイヤー作成（レギュレーション確定）に進まない
            Assert.False(init.IsCompleted);
            Assert.Empty(repo.Inserted);

            gate.SetResult(true);
            // 止まった場合にテスト全体がハングしないよう、待ち時間に上限を付ける
            var finished = await Task.WhenAny(init, Task.Delay(TimeSpan.FromSeconds(10)));
            Assert.True(finished == init, "読み込み完了後もInitViewが完了しない");
            await init;

            Assert.Equal(2, repo.Inserted.Single().Regulation);
        });

        [Fact]
        public void InitView_GistUnavailable_StillStarts_WithRegulation1() => StaRunner.Run(async () =>
        {
            using var server = new CountingServer(500, "error");
            var repo = new FakePlayerRepository();
            var loader = new FakeLoader { Events = { WorldMove(), Join("champ") } };
            var controller = CreateController(loader, repo, new GistWinCountProvider(server.Url), out _);

            await controller.InitView();   // 例外にならず起動できる

            Assert.Equal(1, repo.Players.Single().RegulationStatus);
        });
    }
}
