using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Sandbox
{
    public static class Program123
    {
        private static async Task Main123f(string[] args)
        {

            var tcpListener = new TcpListener(IPAddress.IPv6Any, 1025);
            tcpListener.Server.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.IPv6Only, false);

            tcpListener.Start();
            while (true)
            {
                var connection = await tcpListener.AcceptTcpClientAsync();

                Console.WriteLine(".");
            }
        }
    }
}