using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading;
using System.Threading.Tasks;
using Topshelf;
using WebAppSvc.Web;

namespace WebAppSvc.WinService
{
    class Program
    {
        static void Main(string[] args)
        {
            HostFactory.Run(config =>
            {
                config.Service<WebAppService>(service =>
                {
                    service.ConstructUsing(s => new WebAppService(args));
                    service.WhenStarted(s => s.Start());
                    service.WhenStopped(s => s.Stop());
                });

                config.RunAsLocalSystem();
                config.StartAutomatically();

                config.SetDescription("My Web App Windows Service");
                config.SetDisplayName("WebAppSvc");
                config.SetServiceName("WebAppSvc");
            });
        }
    }

    public class WebAppService
    {
        private readonly string[] _args;

        private Task _webAppTask;

        private CancellationTokenSource _tokenSource;

        public WebAppService(string[] args)
        {
            this._args = args;
            _tokenSource = new CancellationTokenSource();
        }

        public void Start()
        {
            _webAppTask = Web.Program.CreateWebHostBuilder(_args)
                            .Build()
                            .RunAsync(_tokenSource.Token);
        }

        public void Stop()
        {
            _tokenSource.Cancel();
            _webAppTask.Wait();
        }
    }
}
