using SFCSharpServerLib.Common.Interfaces;

namespace SFCSharpServerLib.Application
{
    public class ApplicationManager
    {
        IApplication value;

        private ApplicationManager(IApplication value)
        {
            this.value = value;

            Initialized();
        }

        public static ApplicationManager RegistServer(IApplication value)
        {
            return new ApplicationManager(value);
        }

        private void Initialized()
        {
            Task.Run(async () =>
            {
                await value.RunAsync();
            });
        }
    }
}
