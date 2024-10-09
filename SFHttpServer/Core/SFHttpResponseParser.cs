using SFHttpServer.Data;
using System.Text;

namespace SFHttpServer.Core
{
    public static class SFHttpResponseParser
    {
        public static byte[] Parse(SFHttpRequestInfo requestInfo, SFHttpResponse sfHttpResponse)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(requestInfo.Version);
            sb.Append(' ');
            sb.Append(sfHttpResponse.GetStatusCode());
            sb.Append(' ');
            sb.Append(" OK\r\n");
            sb.Append(SFHttpHeaderNames.ContentType);
            sb.Append(": ");
            sb.Append(sfHttpResponse.GetContextType());
            sb.Append("\r\n");
            sb.Append(SFHttpHeaderNames.Server);
            sb.Append(": ");
            sb.Append(System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription);
            sb.Append("\r\n");
            sb.Append(SFHttpHeaderNames.Date);
            sb.Append(": ");
            sb.Append(DateTime.Now);
            sb.Append("\r\n");
            sb.Append(SFHttpHeaderNames.ContentLength);
            sb.Append(": ");
            sb.Append(sfHttpResponse.GetContentLength());
            sb.Append("\r\n\r\n");

            return System.Text.Encoding.UTF8.GetBytes(sb.ToString());
        }
    }
}
