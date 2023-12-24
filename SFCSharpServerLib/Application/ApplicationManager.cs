using SFCSharpServerLib.Common.Interfaces;

namespace SFCSharpServerLib.Application
{
    public class ApplicationManager
    {
        IApplication value;

        private ApplicationManager(IApplication value)
        {
            this.value = value;

            AppDomain.CurrentDomain.ProcessExit += OnCurrentDomainExist;

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

        private void OnCurrentDomainExist(object sender, EventArgs e)
        {
            Task t1 = Task.Run(async () =>
            {
                await value.StopAsync();
            });

            t1.Wait();
        }
    }
}
