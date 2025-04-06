namespace VRCEntryBoard.Infra.VRChat
{
    internal enum EventType
    {
        PlayerJoin,
        PlayerLeft,
        WorldMove
    }

    /// <summary>
    /// VRChatのログファイルモデル
    /// </summary>
    internal sealed class VRChatLogFileModel
    {
        public EventType EventType { get; set; }
        public string PlayerName { get; set; }
        public string WorldName { get; set; }
    }
}
