using log4net.Config;
using Topshelf;

namespace EventStore.RPC.Server.Console
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            XmlConfigurator.Configure();

            HostFactory.Run(x =>
            {
                x.Service<Application>(s =>
                {
                    s.ConstructUsing(name => new Application());
                    s.WhenStarted(app => app.Start());
                    s.WhenStopped(app => app.Stop());
                });

                x.RunAsLocalSystem();
                x.SetDescription("EventStore.RPC");
                x.SetDisplayName("EventStore.RPC");
                x.SetServiceName("EventStore.RPC");

                x.EnableServiceRecovery(r =>
                {
                    r.RestartService(0);
                    r.RestartComputer(0, "Restart to fix EventStore.RPC");
                    r.RestartService(0);
                    r.SetResetPeriod(1);
                });
            });
        }
    }
}