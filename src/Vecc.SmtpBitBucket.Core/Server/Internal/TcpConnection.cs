using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Vecc.SmtpBitBucket.Core.Stores;

namespace Vecc.SmtpBitBucket.Core.Server.Internal
{
    public abstract class TcpConnection<T>
    {
        private CancellationToken _hangupToken;
        private NetworkStream _networkStream;
        private Task _readTask { get; set; }
        private TcpClient _tcpClient;

        public TcpConnection(ILogger<T> logger, IOptions<ServerOptions> serverOptions, IUserStore userStore)
        {
            this.Logger = logger;
            this.ServerOptions = serverOptions.Value;
            this.UserStore = userStore;
        }

        protected ServerOptions ServerOptions { get; }
        protected IUserStore UserStore { get; }
        protected ILogger<T> Logger { get; }
        protected bool Terminated { get; set; }
        protected IPEndPoint IpEndpoint { get; set; }

        public virtual async Task StartAsync(TcpClient tcpClient, CancellationToken hangupToken)
        {
            await Task.Yield();
            try
            {
                var ipEndpoint = tcpClient.Client.RemoteEndPoint as IPEndPoint;
                var buffer = new byte[1024];

                this._hangupToken = hangupToken;
                this._tcpClient = tcpClient;
                this._networkStream = this._tcpClient.GetStream();
                this._readTask = this.ReadAsync();
                //this._networkStream.BeginRead(buffer, 0, 1024, this.Read, buffer);

                await this.StartAsync();
            }
            catch (Exception exception)
            {
                this.Logger.LogError(exception, "Unable to start server.");
            }
        }

        protected abstract Task StartAsync();

        protected async Task ReadAsync()
        {
            try
            {
                var bytesRead = 0;
                var buffer = new byte[1024];
                var stringBuffer = string.Empty;

                while ((bytesRead = (await this._networkStream.ReadAsync(buffer, 0, 1024, this._hangupToken))) > 0)
                {

                    stringBuffer += Encoding.Default.GetString(buffer, 0, bytesRead);

                    var linePointer = -1;
                    while (stringBuffer != null && (linePointer = stringBuffer.IndexOf("\r\n")) >= 0)
                    {
                        var line1 = stringBuffer.Substring(0, linePointer);

                        this.Logger.LogTrace("Handling line: {0}", line1);
                        stringBuffer = stringBuffer.Substring(linePointer + 2);

                        await this.ProcessIncomingLineAsync(line1);
                    }

                }

                await this.TerminateAsync();
            }
            catch (Exception exception)
            {
                this.Logger.LogError(exception, "Error while reading from socket.");
                await this.TerminateAsync();
            }
        }

        protected virtual async Task SendAsync(string message)
        {
            try
            {
                this.Logger.LogTrace("Sending Data: {0}", message);
                var buffer = Encoding.Default.GetBytes(message + "\r\n");

                await this._networkStream.WriteAsync(buffer, 0, buffer.Length, this._hangupToken);
            }
            catch (Exception exception)
            {
                this.Logger.LogError(exception, "Error while writing to socket.");
                await this.TerminateAsync();
            }
        }

        protected abstract Task ProcessIncomingLineAsync(string line);

        protected virtual Task TerminateAsync()
        {
            try
            {
                if (this._networkStream != null)
                {
                    this._networkStream.Dispose();
                }
            }
            catch { }
            try
            {
                if (this._tcpClient != null)
                {
                    this._tcpClient.Close();
                }
            }
            catch { }

            //release the resources.
            this._networkStream = null;
            this._tcpClient = null;
            this.Terminated = true;

            return Task.CompletedTask;
        }
    }
}
