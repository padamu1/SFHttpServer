namespace SFHttpServer.Data
{
    public enum HTTP_METHOD
    {
        GET,
        POST,
        PUT,
        DELETE,
    }

    public class HttpMethodString
    {
        public const string GET = "GET";
        public const string POST = "POST";
        public const string PUT = "PUT";
        public const string DELETE = "DELETE";

        public static HTTP_METHOD GetHttpMethodEnum(string method)
        {
            switch (method)
            {
                case GET:
                    return HTTP_METHOD.GET;
                case POST:
                    return HTTP_METHOD.POST;
                case PUT:
                    return HTTP_METHOD.PUT;
                case DELETE:
                    return HTTP_METHOD.DELETE;
                default:
                    throw new ArgumentException();
            }
        }
    }

    public class HttpContentTypeString
    {
        public const string ContentTypeJson = "application/json";
    }
}
