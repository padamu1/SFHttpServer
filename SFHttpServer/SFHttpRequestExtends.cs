using SFHttpServer.Data;
using System.Net;

namespace SFHttpServer
{
    public static class SFHttpRequestExtends
    {
        public static async Task<SFHttpResponse> RequestProcessing(this HttpApplication httpApplication, HttpListenerRequest request)
        {
            try
            {
                HTTP_METHOD method = HttpMethodString.GetHttpMethodEnum(request.HttpMethod);

                if(method == HTTP_METHOD.UNKNOWN)
                {
                    return null;
                }

                Dictionary<HTTP_METHOD, Dictionary<string, Func<SFHttpRequest, Task<SFHttpResponse>>>> httpMethodDic = httpApplication.GetMethodDic();

                if (httpMethodDic.ContainsKey(method))
                {
                    string path = new string(request.RawUrl.ToCharArray()).Remove(0, 1);

                    if (httpMethodDic[method].ContainsKey(path))
                    {
                        SFHttpRequest httpRequest = new SFHttpRequest()
                        {
                            ContentType = request.ContentType,
                        };

                        using (var reader = new StreamReader(request.InputStream))
                        {
                            httpRequest.Content = reader.ReadToEnd();
                        }
                        return await httpMethodDic[method][path].Invoke(httpRequest);
                    }
                }
            }
            catch(Exception e)
            {
                Console.Error.WriteLine(e);
            }
            return null;
        }
    }
}
