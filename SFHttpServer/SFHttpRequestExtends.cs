﻿using SFHttpServer.Data;
using System.Net;

namespace SFHttpServer
{
    public static class SFHttpRequestExtends
    {
        public static SFHttpResponse RequestProcessing(this HttpApplication httpApplication, HttpListenerRequest request)
        {
            HTTP_METHOD method = HttpMethodString.GetHttpMethodEnum(request.HttpMethod);

            Dictionary<HTTP_METHOD, Dictionary<string, Func<SFHttpRequest, SFHttpResponse>>> httpMethodDic = httpApplication.GetMethodDic();

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
                    return httpMethodDic[method][path].Invoke(httpRequest);
                }
            }
            return null;
        }
    }
}