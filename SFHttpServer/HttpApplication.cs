using SFCSharpServerLib.Common.Data;
using SFCSharpServerLib.Common.Interfaces;
using SFHttpServer.Data;
using System;
using System.Net;
using System.Text;

namespace SFHttpServer
{
    public class HttpApplication : IApplication
    {
        HttpListener httpListener;
        Dictionary<HTTP_METHOD, Dictionary<string, Func<SFHttpRequest, Task<SFHttpResponse>>>> httpMethodDic;

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

            ReceiveMessage();
        }

        private void ReceiveMessage()
        {
            Task.Run(() =>
            {
                httpListener.BeginGetContext(RequestReceive, httpListener);
            });
        }

        public void RequestReceive(IAsyncResult ar)
        {
            try
            {
                HttpListener listener = (HttpListener)ar.AsyncState;
                HttpListenerContext context = listener.EndGetContext(ar);
                HttpListenerRequest request = context.Request;
                Task.Run(async () =>
                {
                    try
                    {
                        await Process(context, request);
                    }
                    catch (Exception e)
                    {
                        Console.Error.WriteLine(e);
                    }
                });
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
            }

            ReceiveMessage();
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
                buffer = System.Text.Encoding.UTF8.GetBytes("Unknown server error");
                response.StatusCode = 404;
            }

            response.ContentLength64 = buffer.Length;
            System.IO.Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            await output.FlushAsync();
            output.Close();
        }

        public Dictionary<HTTP_METHOD, Dictionary<string, Func<SFHttpRequest, Task<SFHttpResponse>>>> GetMethodDic()
        {
            return httpMethodDic;
        }

        public async Task StopAsync()
        {
            httpListener.Stop();
            Console.WriteLine("HTTP Server Stopped");
        }

        public HttpApplication AddMethod(HTTP_METHOD method, string path, Func<SFHttpRequest, Task<SFHttpResponse>> func)
        {
            if(!httpMethodDic.ContainsKey(method))
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
