# VRCEntryBoard
VRChatにて自身のいるインスタンスのプレイヤー名を一覧化し、ファイル出力してくれるツールです。

本ツールは特定のイベント団体の活動のために作成されており、予告なく仕様を変更する場合があります。

## 設定ファイルについて
このアプリケーションは `VRCEntryBoard-config.json` という設定ファイルを使用します。初回ビルド時には `VRCEntryBoard-config.example.json` からコピーして作成されますが、ご自身の環境に合わせて編集してください。

設定ファイルは実行ファイル（.exe）と同じディレクトリに配置する必要があります。
設定内容：
- `Url`: Supabaseの接続URL
- `Key`: SupabaseのAPIキー
- `MonitoredWorldNames`: 監視対象のワールド名リスト
