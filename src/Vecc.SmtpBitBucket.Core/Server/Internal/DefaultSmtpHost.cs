using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Vecc.SmtpBitBucket.Core.Stores;

namespace Vecc.SmtpBitBucket.Core.Server.Internal
{
    public class DefaultSmtpHost : ISmtpHost
    {
        private readonly ILogger<DefaultSmtpHost> _logger;
        private readonly ISmtpServerFactory _tcpConnectionFactory;
        private CancellationToken _hangupToken;

        public DefaultSmtpHost(ILogger<DefaultSmtpHost> logger, ISmtpServerFactory tcpConnectionFactory)
        {
            this._logger = logger;
            this._tcpConnectionFactory = tcpConnectionFactory;
        }

        public async Task RunAsync(int port, CancellationToken hangupToken)
        {
            this._logger.LogTrace("Creating listener");

            var listener = new TcpListener(IPAddress.IPv6Any, port);
            listener.Server.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.IPv6Only, false);

            var cancelRegistration = hangupToken.Register(() =>
            {
                this._logger.LogInformation("Cancellation requested");
                listener.Stop();
                this._logger.LogInformation("Listener stopped");
            });
            this._hangupToken = hangupToken;

            try
            {
                this._logger.LogTrace("Starting with a 100 connection backlog");

                await Task.Run(async () =>
                {
                    listener.Start(1);
                    this._logger.LogTrace("Listening for a connection");

                    while (!hangupToken.IsCancellationRequested)
                    {
                        var accepted = await this.StartConnectionAsync(listener);
                        if (!accepted)
                        {
                            this._logger.LogInformation("A connection was not accepted.");
                        }
                    }

                    this._logger.LogInformation("Token was cancelled, not accepting new connections");
                }, hangupToken);
            }
            catch (TaskCanceledException cancelled)
            {
                this._logger.LogInformation(cancelled, "Stopping listener due to cancellation token being cancelled");
                listener.Stop();
            }
            catch (Exception exception)
            {
                this._logger.LogError(exception, "Unable to start host.");
            }
        }

        private async Task<bool> StartConnectionAsync(TcpListener listener)
        {
            try
            {
                this._logger.LogTrace("Getting tcp client");
                TcpClient client;
                try
                {
                    client = await listener.AcceptTcpClientAsync();
                    this._logger.LogTrace("Accepted client");
                }
                catch (ObjectDisposedException exception)
                {
                    this._logger.LogError(exception, "Listener is disposed, this happens during shutdown and can safely be ignore if it is shutting down.");
                    return false;
                }

                if (client?.Client != null)
                {
                    this._logger.LogTrace("Processing connection {@RemoteEndPoint}", client.Client?.RemoteEndPoint);
                    var server = await this._tcpConnectionFactory.CreateTcpServerAsync();
                    var serverTask = Task.Run(() => server.StartAsync(client, this._hangupToken));
                    return true;
                }
                else
                {
                    this._logger.LogInformation("Client was null, connection was probably closing. Not doing anything.");
                    return false;
                }
            }
            catch (Exception exception)
            {
                this._logger.LogError(exception, "Unable to start connection.");
                return false;
            }
        }
    }
}
