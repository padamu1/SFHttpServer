using SFTcpServer.Core;
using System.Net;
using System.Net.Sockets;

namespace SFHttpServer.Core
{
    public class SFHttpListener : SocketListener
    {
        private Func<Socket, Task> onSocketAccept;

        protected SFHttpListener(IPAddress ipAddr, int port, Func<Socket, Task> onSocketAccept) : base(ipAddr, port)
        {
            this.onSocketAccept = onSocketAccept;
        }

        public static SFHttpListener SetServer(IPAddress ipAddr, int port, Func<Socket, Task> onSocketAccept)
        {
            return new SFHttpListener(ipAddr, port, onSocketAccept);
        }

        protected override async Task OnSocketAccept(Socket socket)
        {
            await onSocketAccept(socket);
        }
    }
}
