using System.Net;

namespace SFHttpServer.Data
{
    public class SFHttpResponse
    {
        private int statusCode;
        private string body;
        private string contentType;
        private byte[] bytes;

        public SFHttpResponse() 
        { 
            body = string.Empty;
            contentType = string.Empty;
        }

        public void SetStatus(int statusCode)
        {
            this.statusCode = statusCode;
        }

        public int GetStatusCode()
        {
            return statusCode;
        }

        public void SetContentType(string contentType)
        {
            this.contentType = contentType;
        }

        public string GetContextType()
        {
            return this.contentType;
        }

        public void SetBytes(byte[] bytes)
        {
            this.bytes = bytes;
        }

        public byte[] GetBytes()
        {
            return bytes;
        }
    }
}
