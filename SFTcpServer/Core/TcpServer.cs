using System.Net;
using System.Net.Sockets;

namespace SFTcpServer.Core
{
    public class TcpServer : SocketListener
    {
        protected Func<Socket, Task> socketAcceptCallback;

        protected TcpServer(IPAddress ipAddr, int port, Func<Socket, Task> socketAcceptCallback) : base (ipAddr, port)
        {
            this.socketAcceptCallback = socketAcceptCallback;
        }

        public static TcpServer SetServer(IPAddress ipAddr, int port, Func<Socket, Task> socketAcceptCallback)
        {
            return new TcpServer(ipAddr, port, socketAcceptCallback);
        }

        protected override async Task OnSocketAccept(Socket socket)
        {
            await socketAcceptCallback(socket);
        }
    }
}
