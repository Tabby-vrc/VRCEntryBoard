using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

using VRCEntryBoard.Domain.Interfaces;
using VRCEntryBoard.Domain.Model;
using VRCEntryBoard.Infra.Logger;

namespace VRCEntryBoard.Infra
{
    /// <summary>
    /// 優勝回数ランキング（JSON）から優勝回数を取得する
    /// 形式: [{ "Name": "プレイヤー名", "Count": 優勝回数, "Ranking": 順位 }, ...]
    /// </summary>
    internal class GistWinCountProvider : IWinCountProvider
    {
        private static readonly TimeSpan RequestTimeout = TimeSpan.FromSeconds(5);

        private readonly ILogger _logger = LogManager.GetLogger("WinCount");
        private readonly string _url;
        private readonly string _configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "VRCEntryBoard-config.json");
        private Dictionary<string, int> _winCounts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        private bool _isLoaded = false;

        public GistWinCountProvider()
        {
            _url = ReadUrlFromConfig();
        }

        /// <summary>テスト・検証用に取得先URLを指定する</summary>
        internal GistWinCountProvider(string url)
        {
            _url = url;
        }

        /// <inheritdoc/>
        public async Task LoadAsync()
        {
            if (_isLoaded) return;
            if (string.IsNullOrWhiteSpace(_url))
            {
                // 空文字の指定は機能の無効化として扱う
                _isLoaded = true;
                return;
            }

            try
            {
                using var client = new HttpClient { Timeout = RequestTimeout };
                var json = await client.GetStringAsync(_url);
                var entries = JsonSerializer.Deserialize<List<WinCountEntry>>(json);

                var winCounts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
                foreach (var entry in entries ?? new List<WinCountEntry>())
                {
                    if (string.IsNullOrEmpty(entry.Name)) continue;
                    // 同名が複数ある場合は大きい方を採用する
                    if (!winCounts.TryGetValue(entry.Name, out var current) || entry.Count > current)
                    {
                        winCounts[entry.Name] = entry.Count;
                    }
                }
                _winCounts = winCounts;
                _isLoaded = true;
                _logger.LogInformation("優勝回数を読み込みました: {0}件", _winCounts.Count);
            }
            catch (Exception ex)
            {
                // 取得できなくても動作は継続する（次回の更新で再試行する）
                _logger.LogWarning(ex, "優勝回数の読み込みに失敗しました。優勝回数は0として扱います。");
            }
        }

        /// <inheritdoc/>
        public int GetWinCount(string playerName)
        {
            if (playerName == null) return 0;
            return _winCounts.TryGetValue(playerName, out var count) ? count : 0;
        }

        /// <summary>設定ファイルから取得先URLを読む（未指定の場合はnull = 機能無効）</summary>
        private string ReadUrlFromConfig()
        {
            try
            {
                if (!File.Exists(_configPath)) return null;
                var config = JsonSerializer.Deserialize<VRCEntryBoardConfig>(File.ReadAllText(_configPath));
                return config?.WinCountRankingUrl;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "設定ファイルの読み込みに失敗したため、優勝回数の取得は行いません。");
                return null;
            }
        }

        private sealed class WinCountEntry
        {
            public string Name { get; set; }
            public int Count { get; set; }
        }
    }
}
