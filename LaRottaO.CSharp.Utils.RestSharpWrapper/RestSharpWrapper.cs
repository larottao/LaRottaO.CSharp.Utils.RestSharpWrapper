using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using RestSharp;

/*
RestSharpResponse wsResponse = await new RestSharpWrapper().restRequest(
RestSharpWrapper.RequiredHttpMethod.GET, //REQUIRED METHOD
url, //ENDPOINT URL
null, //DEFAULT HEADERS
null, //DEFAULT PARAMETERS
null, //QUERY PARAMETERS
null, //BODY
RequiredBodyType.NO_BODY, //REQUIRED BODY TYPE
false, //CHECK SSL
true, //CREATE NEW INSTANCE
10000, //TIMEOUT
false); //SWALLOW ERRORS
*/

namespace LaRottaO.CSharp.Utils.RestSharpWrapper
{
    public class RestSharpWrapper
    {
        private RestClient client = new RestClient();

        public CookieContainer getCookieContainer()
        {
            return client.CookieContainer;
        }

        public enum RequiredHttpMethod
        { GET, POST, PATCH, PUT, DELETE }

        public enum RequiredBodyType
        { NO_BODY, APPLICATION_FORM_URL_ENCODED, APPLICATION_JSON, TEXT_PLAIN }

        public async Task<RestSharpResponse> restRequest(RequiredHttpMethod requiredMethod, String endPointUrl, List<String[]> defaultHeadersList, List<String[]> defaultParametersList, List<String[]> queryParametersList, string body, RequiredBodyType requiredBodyType, Boolean checkSSL, int argTimeOutMs = 10000, Boolean argSwallowAnyErrors = false)
        {
            RestSharpResponse response = new RestSharpResponse();
            response.success = false;
            response.content = "";
            response.details = "";

            if (String.IsNullOrEmpty(endPointUrl))
            {
                response.success = false;
                response.details = "Received Http response was null";
                response.content = "";
                return response;
            }

            RestResponse restSharpRestResponse = null;

            try

            {
                client.Options.BaseUrl = new Uri(endPointUrl);

                if (!checkSSL)
                {
                    client.Options.RemoteCertificateValidationCallback  = (sender, certificate, chain, sslPolicyErrors) => true;
                }

                client.Options.ThrowOnAnyError = argSwallowAnyErrors;
                client.Options.MaxTimeout = argTimeOutMs;

                if (defaultHeadersList != null)
                {
                    foreach (String[] defaultHeader in defaultHeadersList)
                    {
                        if (String.IsNullOrEmpty(defaultHeader[0]) || String.IsNullOrEmpty(defaultHeader[1]))
                        {
                            continue;
                        }

                        client.AddDefaultHeader(defaultHeader[0], defaultHeader[1]);
                        Debug.WriteLine("Added defaultHeader: " + defaultHeader[0] + " " + defaultHeader[1]);
                    }
                }

                if (defaultParametersList != null)
                {
                    foreach (String[] defaultParameter in defaultParametersList)
                    {
                        if (String.IsNullOrEmpty(defaultParameter[0]) || String.IsNullOrEmpty(defaultParameter[1]))
                        {
                            continue;
                        }

                        var existingParameter = client.DefaultParameters.FirstOrDefault(p => p.Name.Equals(defaultParameter[0]));

                        if (existingParameter != null)
                        {
                            client.DefaultParameters.RemoveParameter(existingParameter);
                        }

                        client.AddDefaultParameter(defaultParameter[0], defaultParameter[1]);
                        Debug.WriteLine("Added defaultParameter: " + defaultParameter[0] + " " + defaultParameter[1]);
                    }
                }

                RestRequest restSharpRestRequest = new RestRequest();

                if (queryParametersList != null)
                {
                    foreach (String[] queryParameter in queryParametersList)
                    {
                        if (String.IsNullOrEmpty(queryParameter[0]) || String.IsNullOrEmpty(queryParameter[1]))
                        {
                            continue;
                        }

                        restSharpRestRequest.AddQueryParameter(queryParameter[0], queryParameter[1]);
                        Debug.WriteLine("Added queryParameter: " + queryParameter[0] + " " + queryParameter[1]);
                    }
                }

                switch (requiredBodyType)
                {
                    case RequiredBodyType.APPLICATION_JSON:

                        restSharpRestRequest.AddParameter("application/json", body, ParameterType.RequestBody);

                        break;

                    case RequiredBodyType.APPLICATION_FORM_URL_ENCODED:

                        restSharpRestRequest.AddParameter("application/x-www-form-urlencoded", body, ParameterType.RequestBody);

                        break;

                    case RequiredBodyType.TEXT_PLAIN:

                        restSharpRestRequest.AddParameter("text/plain", body, ParameterType.RequestBody);

                        break;
                }

                try
                {
                    switch (requiredMethod)
                    {
                        case RequiredHttpMethod.GET:
                            restSharpRestResponse = await client.ExecuteAsync(restSharpRestRequest, Method.Get);
                            break;

                        case RequiredHttpMethod.POST:
                            restSharpRestResponse = await client.ExecuteAsync(restSharpRestRequest, Method.Post);
                            break;

                        case RequiredHttpMethod.PATCH:
                            restSharpRestResponse = await client.ExecuteAsync(restSharpRestRequest, Method.Patch);
                            break;

                        case RequiredHttpMethod.DELETE:
                            restSharpRestResponse = await client.ExecuteAsync(restSharpRestRequest, Method.Delete);
                            break;

                        case RequiredHttpMethod.PUT:
                            restSharpRestResponse = await client.ExecuteAsync(restSharpRestRequest, Method.Put);
                            break;

                        default:
                            response.success = false;
                            response.details = requiredMethod + " is not a valid RequiredHttpMethod";
                            response.content = "";
                            return response;
                    }
                }
                catch (Exception ex)
                {
                    return errorHandling(restSharpRestResponse, ex);
                }

                if (restSharpRestResponse == null)
                {
                    return errorHandling(restSharpRestResponse);
                }

                if (restSharpRestResponse.ErrorException != null && !String.IsNullOrEmpty(restSharpRestResponse.ErrorException.ToString()))
                {
                    return errorHandling(restSharpRestResponse);
                }

                response.success = true;
                response.details = "";

                response.content = restSharpRestResponse.Content;
                response.httpStatusCode = restSharpRestResponse.StatusCode.ToString();

                return response;
            }
            catch (Exception ex)
            {
                return errorHandling(restSharpRestResponse, ex);
            }
        }

        private RestSharpResponse errorHandling(RestResponse restSharpRestResponse, Exception ex = null)
        {
            RestSharpResponse response = new RestSharpResponse();

            response.success = false;

            if (ex!=null)
            {
                response.details = ex.ToString();
            }

            if (restSharpRestResponse != null &&
                restSharpRestResponse.ErrorException != null &&
                !String.IsNullOrEmpty(restSharpRestResponse.ErrorException.ToString()))
            {
                response.success = false;
                response.details = restSharpRestResponse.ErrorException.ToString();
            }

            if (restSharpRestResponse != null &&
                !String.IsNullOrEmpty(restSharpRestResponse.Content))
            {
                response.content = restSharpRestResponse.Content;
            }

            if (restSharpRestResponse != null)
            {
                response.httpStatusCode = restSharpRestResponse.StatusCode.ToString();
            }

            return response;
        }
    }
}