using SFCSharpServerLib.Common.Data;
using SFCSharpServerLib.Common.Interfaces;
using SFHttpServer.Core;
using SFHttpServer.Data;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SFHttpServer
{
    public class HttpApplication : IApplication
    {
        SFHttpListener httpListener;
        Dictionary<HTTP_METHOD, Dictionary<string, Func<SFHttpRequest, Task<SFHttpResponse>>>> httpMethodDic;
        CancellationTokenSource cancelToken;

        public HttpApplication(SFServerInfo sfServerInfo)
        {
            httpMethodDic = new Dictionary<HTTP_METHOD, Dictionary<string, Func<SFHttpRequest, Task<SFHttpResponse>>>>();
            httpListener = SFHttpListener.SetServer(IPAddress.Any, sfServerInfo.Port, OnSocketAccept);
        }

        public async Task RunAsync()
        {
            Console.WriteLine("HTTP Server Run");

            Publish();
        }

        public void Publish()
        {
            httpListener.StartServer();
        }

        private async Task OnSocketAccept(Socket socket)
        {
            byte[] bytes = new byte[1024];
            string data = null;
            try
            {
                int numByte = await socket.ReceiveAsync(bytes, SocketFlags.None);
                data += Encoding.ASCII.GetString(bytes, 0, numByte);
                while (socket.Available > 0)
                {
                    numByte = socket.Receive(bytes);
                    data += Encoding.ASCII.GetString(bytes, 0, numByte);
                    if (numByte <= 0)
                    {
                        break;
                    }
                }

                Console.Write(data);
                await Process(socket, data);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                socket.Close();
            }
        }

        private async Task Process(Socket socket, string data)
        {
            SFHttpRequestInfo? sfHttpRequestInfo;
            try
            {
                sfHttpRequestInfo = SFHttpRequestParser.Parse(data);
            }
            catch(Exception e)
            {
                sfHttpRequestInfo = null;
            }

            SFHttpResponse sfHttpResponse = null;
            if (sfHttpRequestInfo != null)
            {
                sfHttpResponse = await this.RequestProcessing(sfHttpRequestInfo);
            }

            if (sfHttpResponse == null)
            {
                sfHttpResponse = new SFHttpResponse();
                sfHttpResponse.SetContentType("text/plain");
                sfHttpResponse.SetBytes(System.Text.Encoding.UTF8.GetBytes("Request Error"));
                sfHttpResponse.SetStatus(404);
            }

            socket.Send(SFHttpResponseParser.Parse(sfHttpRequestInfo, sfHttpResponse));

            if (sfHttpResponse.GetContentLength() > 0)
            {
                socket.Send(sfHttpResponse.GetBytes());
            }

            socket.Close();
        }

        public Dictionary<HTTP_METHOD, Dictionary<string, Func<SFHttpRequest, Task<SFHttpResponse>>>> GetMethodDic()
        {
            return httpMethodDic;
        }

        public async Task StopAsync()
        {
            if (cancelToken != null && cancelToken.IsCancellationRequested == false)
            {
                cancelToken.Cancel();
            }

            httpListener.StopServer();
            Console.WriteLine("HTTP Server Stopped");
        }

        public HttpApplication AddMethod(HTTP_METHOD method, string path, Func<SFHttpRequest, Task<SFHttpResponse>> func)
        {
            if (!httpMethodDic.ContainsKey(method))
            {
                httpMethodDic.Add(method, new Dictionary<string, Func<SFHttpRequest, Task<SFHttpResponse>>>());
            }

            if (httpMethodDic[method].ContainsKey(path))
            {
                throw new Exception($"{method.ToString()} - {path} already exist!");
            }

            httpMethodDic[method].Add(path, func);

            return this;
        }
    }
}
