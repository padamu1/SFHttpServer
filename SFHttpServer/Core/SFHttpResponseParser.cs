using SFHttpServer.Core;
using SFHttpServer.Data;
using System.Text;

namespace SFHttpServer.Core
{
    public static class SFHttpResponseParser
    {
        public static byte[] Parse(SFHttpResponse sfHttpResponse)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("HTTP/1.1");
            sb.Append(' ');
            sb.Append(sfHttpResponse.GetStatusCode());
            sb.Append(' ');
            sb.Append("OK\r\n");
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
            sb.Append(DateTime.UtcNow.ToString("r"));
            sb.Append("\r\n");
            sb.Append("Access-Control-Allow-Origin: *\r\n");
            sb.Append("Access-Control-Allow-Methods: GET, POST, OPTIONS\r\n");
            sb.Append("Access-Control-Allow-Headers: Referer, Content-Type, User-Agent\r\n");
            sb.Append("Connection: close\r\n"); // 연결 종료 명시
            sb.Append("Cache-Control: no-cache, no-store, must-revalidate\r\n");
            sb.Append("Pragma: no-cache\r\n");
            sb.Append("Expires: 0\r\n");
            if (sfHttpResponse.GetContextType() == "application/octet-stream")
            {
                sb.Append("Content-Encoding: gzip\r\n");
            }
            sb.Append("Transfer-Encoding: identity\r\n");  // chunked 전송 해제
            sb.Append(SFHttpHeaderNames.ContentLength);
            sb.Append(": ");
            sb.Append(sfHttpResponse.GetContentLength());
            sb.Append("\r\n\r\n");

            return System.Text.Encoding.UTF8.GetBytes(sb.ToString());
        }
    }
}