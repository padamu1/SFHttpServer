using System.Net;
using System.Net.Sockets;

namespace SFTcpServer.Core
{
    public abstract class SocketListener
    {
        private CancellationTokenSource cancellationTokenSource;
        private Socket listenerSocket;

        private IPAddress ipAddr;
        private int port;

        protected SocketListener(IPAddress ipAddr, int port)
        {
            this.ipAddr = ipAddr;
            this.port = port;
        }

        public void StartServer()
        {
            cancellationTokenSource = new CancellationTokenSource();
            Task.Run(() => HandShake(cancellationTokenSource.Token));
        }

        public void StopServer()
        {
            cancellationTokenSource.Cancel();
            listenerSocket.Close();
        }

        private async Task HandShake(CancellationToken cancellationToken)
        {
            IPEndPoint localEndPoint = new IPEndPoint(ipAddr, port);
            listenerSocket = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                // 소켓을 로컬 엔드포인트에 바인드하고 연결 요청을 듣기 시작
                listenerSocket.Bind(localEndPoint);
                listenerSocket.Listen(10);
            }
            catch (Exception ex)
            {
                // 여기서 초기 바인딩 및 리스닝 중 발생한 예외 처리
                Console.Error.WriteLine($"Exception during socket setup: {ex.Message}");
                return;
            }

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    Console.WriteLine("Waiting for a connection...");
                    // 비동기적으로 클라이언트 연결 수락
                    Socket handlerSocket = await listenerSocket.AcceptAsync();

                    if (handlerSocket != null)
                    {
                        // 연결된 소켓을 처리하는 작업을 별도의 태스크로 실행
                        _ = Task.Run(async () => await OnSocketAccept(handlerSocket), cancellationToken);
                    }
                }
                catch (OperationCanceledException)
                {
                    // 작업 취소 요청 처리
                    Console.WriteLine("Cancellation requested, shutting down handshake listener.");
                    break;
                }
                catch (Exception ex)
                {
                    // 연결 수립 중 발생한 예외 처리
                    Console.Error.WriteLine($"Exception accepting client: {ex.Message}");
                    // 필요한 경우 연결 시도를 계속할 수 있도록 여기서는 계속 진행
                }
            }
        }

        protected abstract Task OnSocketAccept(Socket socket);
    }
}
