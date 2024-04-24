using System;

namespace LaRottaO.CSharp.Utils.RestSharpWrapper
{
    public class RestSharpResponse
    {
        public Boolean success { get; set; }
        public String httpStatusCode { get; set; }
        public String details { get; set; }
        public String content { get; set; }
    }
}