using System.Collections.Specialized;

#pragma warning disable CS8604
#pragma warning disable CS8603
namespace SFHttpServer.Core
{
    public class SFHttpRequestInfo
    {
        public string Path => header[SFHttpHeaderNames.Path];

        public string Method => header[SFHttpHeaderNames.Method];

        public int ContentLength => int.Parse(header[SFHttpHeaderNames.ContentLength]);

        public string ContentType => header[SFHttpHeaderNames.ContentType];

        public string Version => version;

        public string Content => content;

        private NameValueCollection header;
        private string version;
        private string content;

        public SFHttpRequestInfo()
        {
            header = new NameValueCollection();
            version = string.Empty;
            content = string.Empty;

            header.Add(SFHttpHeaderNames.Path, string.Empty);
            header.Add(SFHttpHeaderNames.Method, string.Empty);
            header.Add(SFHttpHeaderNames.ContentLength, "0");
            header.Add(SFHttpHeaderNames.ContentType, string.Empty);
        }

        public void SetVersion(string version)
        {
            this.version = version;
        }

        public void SetHeader(string key, string value)
        {
            header[key] = value;
        }

        public string GetHeaderValue(string headerKey)
        {
            if (header.Get(headerKey) != null)
            {
                return header[headerKey];
            }

            return string.Empty;
        }

        public void SetContent(string content)
        {
            this.content = content;
        }
    }
}

#pragma warning restore CS8604
#pragma warning restore CS8603