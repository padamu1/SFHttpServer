using SFCSharpServerLib.Common.Data;
using SFCSharpServerLib.Common.Interfaces;
using SFHttpServer.Data;
using System.Net;

namespace SFHttpServer
{
    public class HttpApplication : IApplication
    {
        HttpListener httpListener;
        Dictionary<HTTP_METHOD, Dictionary<string, Func<SFHttpRequest, Task<SFHttpResponse>>>> httpMethodDic;
        CancellationTokenSource cancelToken;

        public HttpApplication(SFServerInfo sfServerInfo)
        {
            httpListener = new HttpListener();
            httpMethodDic = new Dictionary<HTTP_METHOD, Dictionary<string, Func<SFHttpRequest, Task<SFHttpResponse>>>>();
            httpListener.Prefixes.Add($"http://{sfServerInfo.Url}:{sfServerInfo.Port}/");
            httpListener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
        }
        public async Task RunAsync()
        {
            Console.WriteLine("HTTP Server Run");

            Publish();
        }

        public void Publish()
        {
            httpListener.Start();

            cancelToken = new CancellationTokenSource();
            Task.Run(ReceiveMessage);
        }

        private async Task ReceiveMessage()
        {
            var token = cancelToken.Token;
            while (!token.IsCancellationRequested)
            {
                try
                {
                    HttpListenerContext context = await httpListener.GetContextAsync();

                    _ = Task.Run(() => RequestReceive(context)).ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine(e);
                }
            }
        }

        public async Task RequestReceive(HttpListenerContext context)
        {
            try
            {
                HttpListenerRequest request = context.Request;

                await Process(context, request);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private async Task Process(HttpListenerContext context, HttpListenerRequest request)
        {
            SFHttpResponse sfHttpResponse = await this.RequestProcessing(request);

            HttpListenerResponse response = context.Response;

            byte[] buffer;
            if (sfHttpResponse != null)
            {
                response.StatusCode = sfHttpResponse.GetStatusCode();
                response.ContentType = sfHttpResponse.GetContextType();
                buffer = sfHttpResponse.GetBytes();
            }
            else
            {
                buffer = System.Text.Encoding.UTF8.GetBytes("Request Error");
                response.StatusCode = 404;
            }

            System.IO.Stream output = response.OutputStream;
            if (buffer != null)
            {
                response.ContentLength64 = buffer.Length;
                output.Write(buffer, 0, buffer.Length);
            }
            else
            {
                response.ContentLength64 = 0;
            }
            await output.FlushAsync();
            output.Close();
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

            httpListener.Stop();
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
