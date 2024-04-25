using System;
using System.Collections.Generic;

namespace LaRottaO.CSharp.Utils.RestSharpWrapper.Models
{
    public enum RequiredHttpMethod
    { GET, POST, PATCH, PUT, DELETE }

    public enum RequiredBodyType
    { NO_BODY, APPLICATION_FORM_URL_ENCODED, APPLICATION_JSON, TEXT_PLAIN }

    public class RestSharpRequest
    {
        public RequiredHttpMethod requiredMethod { get; set; }
        public String endPointUrl { get; set; }
        public List<String[]> defaultHeadersList { get; set; } = new List<String[]>();
        public List<String[]> defaultParametersList { get; set; } = new List<String[]>();
        public List<String[]> parametersList { get; set; } = new List<String[]>();
        public List<String[]> queryParametersList { get; set; } = new List<String[]>();
        public string body { get; set; }
        public RequiredBodyType requiredBodyType { get; set; }
        public Boolean checkSSL { get; set; }
        public int maxTimeout { get; set; }
    }
}