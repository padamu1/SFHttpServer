namespace SFCSharpServerLib.Common.Data
{
    [Serializable]
    public class SFServerInfo
    {
        public string Url { get; set; }
        public int Port { get; set; }
        public string ServerName { get; set; }
        public string ServerVersion { get; set; }
    }
}
