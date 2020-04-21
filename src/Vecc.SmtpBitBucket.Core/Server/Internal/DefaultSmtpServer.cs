using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Vecc.SmtpBitBucket.Core.Stores;

namespace Vecc.SmtpBitBucket.Core.Server.Internal
{
    public class DefaultSmtpServer : TcpConnection<DefaultSmtpServer>, ISmtpServer
    {
        private const string AuthenticationRequired = "530 Authentication required";
        private const string AuthenticationFailed = "535 Authentication failed";
        private const string AuthenticationSuccessful = "235 Authentication successful.";
        private const string AuthenticationLoginRequestUsername = "334 VXNlcm5hbWU6"; //Username:
        private const string AuthenticationLoginRequestPassword = "334 VXNlcm5hbWU6"; //Password:
        private const string AuthenticationUnrecognizedAuthType = "504 Unknown authentication type";
        private const string GeneralBadSequenceOfCommands = "503 Bad sequence of commands";
        private const string GeneralCommandNotImplemented = "502 Command not implemented";
        private const string GeneralCommandNotRecognized = "500 Command not recognized";
        private const string GeneralOk = "250 OK";
        private const string VerifyCannotVerifyAttemptAnyways = "252 Cannot VRFY user, but will accept message and attempt delivery";
        private const string QuitGoodbye = "221 Goodbye!";

        private SmtpConnection _session;
        private bool _inData = false;
        private bool _inAuth = false;
        private readonly ServiceStore _serviceStore;
        private readonly ISmtpConfiguration _smtpConfiguration;
        private readonly bool _requireAuthentication;
        private string _authType;
        private string _authUsername;
        private bool _inUse = false;

        public DefaultSmtpServer(ILogger<DefaultSmtpServer> logger, ServiceStore serviceStore, IOptions<ServerOptions> serverOptions, IUserStore userStore)
            : base(logger, serverOptions, userStore)
        {
            this._requireAuthentication = serverOptions.Value.RequireValidUsernamePassword;
            this._serviceStore = serviceStore;
        }

        protected override async Task StartAsync()
        {
            if (this._inUse)
            {
                throw new Exception("This server is being re-used which is not allowed.");
            }

            this._inUse = true;

            this._session = await this._serviceStore.CreateNewSessionAsync(this.IpEndpoint?.Address.ToString());

            await this.SendAsync("220 " + this.ServerOptions.ServerHeloResponse);
        }

        protected override async Task ProcessIncomingLineAsync(string line)
        {
            await this._serviceStore.StoreChatterAsync(line, Direction.In, this._session.Id);

            if (this._inData)
            {
                await this.DATAAsync(line);
            }
            else if (this._inAuth)
            {
                await this.AUTHAsync(line);
            }
            else
            {
                var parts = line.Split(' ');
                switch (parts[0].ToUpper())
                {
                    case "WIPESESSION":
                        //this._authPassword = null;
                        this._authType = null;
                        this._authUsername = null;
                        this._inAuth = false;
                        this._inData = false;
                        this._session.Reset(true);

                        break;
                    case "HELO":
                        await this.HELOAsync();
                        break;
                    case "EHLO":
                        await this.EHLOAsync(parts);
                        break;
                    case "AUTH":
                        await this.AUTHAsync(parts, line);
                        break;
                    case "SEND":
                    case "SOML":
                    case "SAML":
                    case "MAIL":
                        await this.MAILAsync(parts, line);
                        break;
                    case "RCPT":
                        await this.RCPTAsync(parts, line);
                        break;
                    case "DATA":
                        await this.DATAAsync(line);
                        break;
                    case "VRFY":
                        await this.SendAsync(VerifyCannotVerifyAttemptAnyways);
                        break;
                    case "EXPN":
                        await this.SendAsync("252 Cannot expand upon list");
                        break;
                    case "RSET":
                        this._session.Reset();
                        await this.SendAsync(GeneralOk);
                        break;
                    case "NOOP":
                        await this.SendAsync(GeneralOk);
                        break;
                    case "QUIT":
                        this.Logger.LogInformation("QUIT command received.");
                        await this.SendAsync(QuitGoodbye);
                        await this.TerminateAsync();
                        break;
                    case "HELP":
                    case "TURN":
                        await this.SendAsync(GeneralCommandNotImplemented);
                        break;
                    case "":
                        break;
                    default:
                        await this.SendAsync(GeneralCommandNotRecognized);
                        break;
                }
            }
        }

        protected override async Task TerminateAsync()
        {
            this.Logger.LogInformation("Terminating SMTP Server connection.");
            await this._serviceStore.EndSessionAsync(this._session);
            await base.TerminateAsync();
        }

        protected override async Task SendAsync(string message)
        {
            await base.SendAsync(message);
            await this._serviceStore.StoreChatterAsync(message, Direction.Out, this._session.Id);
        }

        private Task HELOAsync() => this.SendAsync(string.Format("250 {0}", Dns.GetHostName().ToLower()));

        private async Task EHLOAsync(string[] parts)
        {
            this._session.EhloHost = parts.Length < 2 ? string.Empty : parts[1];
            await this.SendAsync(string.Format("250-{0}", Dns.GetHostName().ToLower()));
            await this.SendAsync("250-8BITMIME");
            await this.SendAsync("250-AUTH LOGIN PLAIN");
            await this.SendAsync(GeneralOk);
        }

        private async Task AUTHAsync(string[] parts, string line)
        {
            if (string.IsNullOrEmpty(this._session.EhloHost))
            {
                await this.SendAsync(GeneralBadSequenceOfCommands);
                return;
            }

            if (parts.Length == 1)
            {
                await this.SendAsync(AuthenticationUnrecognizedAuthType);
            }
            else
            {
                var authType = parts[1].ToUpper();
                switch (authType)
                {
                    case "LOGIN":
                    case "PLAIN":
                        this._authType = authType;
                        break;
                    default:
                        await this.SendAsync(AuthenticationUnrecognizedAuthType);
                        return;
                }

                this.Logger.LogTrace("AuthType: <{0}>", authType);
                this._inAuth = true;

                if (authType == "LOGIN")
                {
                    if (parts.Length >= 3)
                    {
                        if (await this.HandleAuthLoginAsync(parts[2]))
                        {
                            this._inAuth = false;
                        }
                    }
                    else
                    {
                        await this.SendAsync(AuthenticationLoginRequestUsername);
                    }
                }
                else if (authType == "PLAIN")
                {
                    this._inAuth = true;
                    if (parts.Length >= 3)
                    {
                        if (await this.HandleAuthPlainAsync(parts[2]))
                        {
                            this._inAuth = false;
                        }
                    }
                    else
                    {
                        await this.SendAsync("334");
                    }
                }
            }
        }

        private async Task<bool> HandleAuthLoginAsync(string line)
        {
            var result = false;

            try
            {
                if (string.IsNullOrEmpty(this._authUsername))
                {
                    this._authUsername = Encoding.Default.GetString(Convert.FromBase64String(line));
                    this.Logger.LogTrace("_authUsername: <{0}>", this._authUsername);
                    await this.SendAsync(AuthenticationLoginRequestPassword);
                }
                else
                {
                    var authPassword = Encoding.Default.GetString(Convert.FromBase64String(line));
                    this.Logger.LogTrace("_authPassword: <{0}>", authPassword);
                    this._inAuth = false;
                    this._session.IsAuthenticated = !this._requireAuthentication || (await this.UserStore.IsValidAsync(this._authUsername, authPassword));
                    if (this._session.IsAuthenticated)
                    {
                        this._session.Username = this._authUsername;
                        await this.SendAsync(AuthenticationSuccessful);
                        await this._serviceStore.UpdateSessionUsernameAsync(this._session);
                        result = true;
                    }
                    else
                    {
                        await this.SendAsync(AuthenticationFailed);
                    }
                }
            }
            catch
            {
                this.Logger.LogWarning("Invalid data passed.");
                await this.SendAsync(AuthenticationFailed);
            }

            this.Logger.LogTrace("result: <{0}>", result);

            return result;
        }

        private async Task<bool> HandleAuthPlainAsync(string line)
        {
            var result = false;
            this._inAuth = false;

            try
            {
                var token = Encoding.Default.GetString(Convert.FromBase64String(line));
                var parts = token.Split('\0');

                if (parts.Length != 3)
                {
                    this.Logger.LogWarning("Invalid data passed.");
                    await this.SendAsync(AuthenticationFailed);
                }

                this._authUsername = parts[1];
                var authPassword = parts[2];

                this.Logger.LogTrace("_authUsername: <{0}>", this._authUsername);
                this.Logger.LogTrace("authPassword: <{0}>", authPassword);

                this._session.IsAuthenticated = !this._requireAuthentication || (await this.UserStore.IsValidAsync(this._authUsername, authPassword));
                if (this._session.IsAuthenticated)
                {
                    this._session.Username = this._authUsername;
                    await this.SendAsync(AuthenticationSuccessful);
                    await this._serviceStore.UpdateSessionUsernameAsync(this._session);
                    result = false;
                }
                else
                {
                    await this.SendAsync(AuthenticationFailed);
                }
            }
            catch (Exception exception)
            {
                this.Logger.LogWarning(exception, "Invalid data passed.");
                await this.SendAsync(AuthenticationFailed);
            }

            this.Logger.LogTrace("result: <{0}>", result);

            return result;
        }

        private async Task AUTHAsync(string line)
        {
            if (this._authType == "LOGIN")
            {
                await this.HandleAuthLoginAsync(line);
            }
            else if (this._authType == "PLAIN")
            {
                await this.HandleAuthPlainAsync(line);
            }
            else
            {
                await this.SendAsync(AuthenticationFailed);
            }
        }

        private async Task MAILAsync(string[] parts, string line)
        {
            // Check for the right number of parameters
            if (parts.Length < 2)
            {
                await this.SendAsync(GeneralCommandNotImplemented);
                return;
            }

            // Check for the ":"
            if (!parts[1].ToUpper().StartsWith("FROM") || !line.Contains(":"))
            {
                await this.SendAsync(GeneralCommandNotImplemented);
                return;
            }

            // Check request order
            if (this._session.EhloHost == null)
            {
                await this.SendAsync(GeneralBadSequenceOfCommands);
                return;
            }

            if (this._requireAuthentication && !this._session.IsAuthenticated)
            {
                await this.SendAsync(AuthenticationRequired);
                return;
            }

            // Set the from settings
            this._session.Reset();

            var address =
                line.Substring(line.IndexOf(":") + 1)
                    .Replace("<", string.Empty)
                    .Replace(">", string.Empty)
                    .Trim();

            this._session.MailFrom = address;

            // Check for encoding
            foreach (var part in parts.Where(part => part.ToUpper().StartsWith("BODY=")))
            {
                switch (part.ToUpper().Replace("BODY=", string.Empty).Trim())
                {
                    case "8BITMIME":
                        this._session.UseUtf8 = true;
                        break;
                    default:
                        this._session.UseUtf8 = false;
                        break;
                }

                break;
            }

            await this.SendAsync(string.Format("250 <{0}> OK", address));
        }

        private async Task RCPTAsync(string[] parts, string line)
        {
            // Check for the ":"
            if (!line.ToUpper().StartsWith("RCPT TO") || !line.Contains(":"))
            {
                await this.SendAsync(GeneralCommandNotImplemented);
                return;
            }

            // Check request order
            if (this._session.MailFrom == null)
            {
                await this.SendAsync(GeneralBadSequenceOfCommands);
                return;
            }

            var address = line.Substring(line.IndexOf(":") + 1)
                    .Replace("<", string.Empty)
                    .Replace(">", string.Empty)
                    .Trim();

            if (!this._session.Recipients.Contains(address))
            {
                this._session.Recipients.Add(address);
            }

            await this.SendAsync(string.Format("250 <{0}> OK", address));
        }

        private async Task DATAAsync(string line)
        {
            if (this._session.Recipients.Count == 0)
            {
                await this.SendAsync(GeneralBadSequenceOfCommands);
                return;
            }
            if (!this._inData)
            {
                await this.SendAsync("354 Start mail input; end with <CRLF>.<CRLF>");
                this._inData = true;
                return;
            }
            if (line == ".")
            {
                this._inData = false;
                //parse the message here
                var bytes = Encoding.UTF8.GetBytes(string.Join("\r\n", this._session.Data));
                using (var stream = new MemoryStream(bytes))
                {
                    var x = new MimeKit.MimeParser(stream);
                    var headers = await x.ParseHeadersAsync();
                    var subject = headers[MimeKit.HeaderId.Subject];
                    this.Logger.LogInformation("Message received: From={from} To={to} Subject={subject}", this._session.MailFrom, string.Join(",", this._session.Recipients), subject);
                    this._session.Subject = subject ?? string.Empty;
                }

                await this.SendAsync(GeneralOk);
                this.Logger.LogInformation("Message received!");

                await this._serviceStore.StoreMessageAsync(this._session);
            }
            else
            {
                this._session.Data.Add(line);
            }
        }
    }
}
