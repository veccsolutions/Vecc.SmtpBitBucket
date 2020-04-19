using System;
using System.Net;
using System.Net.Mail;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Vecc.SmtpBitBucket.Core.Server;

namespace Vecc.SmtpBitBucket.Sandbox
{
    internal class Program
    {
        private static ILogger<Program> _logger;
        private static Task _hostTask;

        private static async Task Main(string[] args)
        {

            var services = ServiceProviderFactory.GetServiceProvider();
            _logger = services.GetRequiredService<ILogger<Program>>();
            _logger.LogInformation("Getting CancellationTokenSource");
            var cancellationTokenSource = new CancellationTokenSource();

            _logger.LogInformation("Done");

            var doit = true;
            while (doit)
            {
                Console.WriteLine("-1 - Exit");
                Console.WriteLine(" 1 - Send random message");
                Console.WriteLine(" 2 - Start server");
                var option = Console.ReadLine();
                switch (option)
                {
                    case "-1":
                        doit = false;
                        break;
                    case "1":
                        await SendMessage();
                        break;
                    case "2":
                        StartServer(services, cancellationTokenSource.Token);
                        break;
                }
            }
            _logger.LogInformation("Stopping host");
            cancellationTokenSource.Cancel();

            //await _hostTask;
            _logger.LogInformation("Host stopped");
        }

        static async Task SendMessage()
        {
            _logger.LogTrace("Creating client");
            using (var smtpClient = new SmtpClient("localhost", 1225))
            {
                _logger.LogTrace("Creating mail message");
                using (var message = new MailMessage("edward@frakkingsweet.com", "edward@frakkingsweet.com"))
                {
                    message.Body = "test";
                    message.Subject = "testsubject";

                    _logger.LogTrace("Sending");
                    await smtpClient.SendMailAsync(message);
                    _logger.LogTrace("Done");
                }
            }
        }
        static void StartServer(IServiceProvider services, CancellationToken hangupToken)
        {
            _logger.LogInformation("Starting host");
            var host = services.GetRequiredService<ISmtpHost>();
            _hostTask = host.RunAsync(1225, hangupToken);
        }

        //static async Task DummyServer()
        //{

        //}
    }
}