namespace SFCSharpServerLib.Common.Interfaces
{
    public interface IApplication
    {
        public Task RunAsync();
        public Task StopAsync();
    }
}
