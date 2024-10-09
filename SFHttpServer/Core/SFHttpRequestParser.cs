namespace SFHttpServer.Core
{
    public static class SFHttpRequestParser
    {
        public static SFHttpRequestInfo? Parse(string data)
        {
            SFHttpRequestInfo sfHttpRequestInfo = new SFHttpRequestInfo();

            // Check Content
            string[] splitData = data.Split("\r\n\r\n");
            if (splitData.Length >= 2 && string.IsNullOrEmpty(splitData[1]) == false)
            {
                sfHttpRequestInfo.SetContent(splitData[1]);
            }

            // Split Header
            splitData = splitData[0].Split("\r\n");

            string[] splitRequest = splitData[0].Split(' ');
            sfHttpRequestInfo.SetHeader(SFHttpHeaderNames.Method, splitRequest[0]);
            sfHttpRequestInfo.SetHeader(SFHttpHeaderNames.Path, splitRequest[1]);
            sfHttpRequestInfo.SetVersion(splitRequest[2]);

            // Set Header
            try
            {
                int offset = 1;
                while (offset < splitData.Length)
                {
                    string[] headerData = splitData[offset++].Trim().Split(':');

                    sfHttpRequestInfo.SetHeader(headerData[0], headerData[1]);
                }

                return sfHttpRequestInfo;
            }
            catch(Exception e)
            {
                Console.Error.WriteLine(e);
            }

            return null;
        }
    }
}
