using SFHttpServer.Core;
using SFHttpServer.Data;

namespace SFHttpServer
{
    public static class SFHttpRequestExtends
    {
        public static async Task<SFHttpResponse> RequestProcessing(this HttpApplication httpApplication, SFHttpRequestInfo request)
        {
            try
            {
                HTTP_METHOD method = HttpMethodString.GetHttpMethodEnum(request.Method);

                if (method == HTTP_METHOD.UNKNOWN)
                {
                    return null;
                }

                Dictionary<HTTP_METHOD, Dictionary<string, Func<SFHttpRequest, Task<SFHttpResponse>>>> httpMethodDic = httpApplication.GetMethodDic();

                if (httpMethodDic.ContainsKey(method))
                {
                    string path = new string(request.Path.ToCharArray()).Remove(0, 1);

                    if (httpMethodDic[method].ContainsKey(path))
                    {
                        SFHttpRequest httpRequest = new SFHttpRequest()
                        {
                            ContentType = request.ContentType,
                        };

                        httpRequest.Content = request.Content;
                        return await httpMethodDic[method][path].Invoke(httpRequest);
                    }
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
            }
            return null;
        }
    }
}
