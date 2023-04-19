using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using RestSharp;

namespace LaRottaO.CSharp.Utils.RestSharpWrapper
{
    public class RestSharpWrapper
    {
        private RestClient client = new RestClient();

        public async Task<CookieContainer> getCookieContainer()
        {
            return client.CookieContainer;
        }

        public enum RequiredHttpMethod
        { GET, POST, PATCH, PUT, DELETE }

        public async Task<RestSharpResponse> restRequest(RequiredHttpMethod requiredMethod, String endPointUrl, List<String[]> headersList, List<String[]> defaultParametersList, List<String[]> queryParametersList, string body, Boolean checkSSL = false, Boolean createNewInstanceOnEachCall = true)
        {
            RestResponse RestResponse = null;

            RestSharpResponse response = new RestSharpResponse();
            response.success = false;
            response.content = "";
            response.details = "";

            try

            {
                RestClientOptions options;

                if (checkSSL)
                {
                    options = new RestClientOptions(endPointUrl)
                    {
                        ThrowOnAnyError = true,
                        MaxTimeout = -1
                    };
                }
                else
                {
                    options = new RestClientOptions(endPointUrl)
                    {
                        ThrowOnAnyError = true,
                        MaxTimeout = -1,
                        RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true
                    };
                }

                if (createNewInstanceOnEachCall)
                {
                    client = new RestClient(options);
                }

                if (headersList != null)
                {
                    foreach (String[] header in headersList)
                    {
                        if (String.IsNullOrEmpty(header[0]) || String.IsNullOrEmpty(header[1]))
                        {
                            continue;
                        }
                        client.AddDefaultHeader(header[0], header[1]);
                    }
                }

                if (defaultParametersList != null)
                {
                    foreach (String[] parameter in defaultParametersList)
                    {
                        if (String.IsNullOrEmpty(parameter[0]) || String.IsNullOrEmpty(parameter[1]))
                        {
                            continue;
                        }

                        client.AddDefaultParameter(parameter[0], parameter[1]);
                    }
                }

                var request = new RestRequest();
                if (body != null)
                {
                    request.AddParameter("application/json", body, ParameterType.RequestBody);
                }

                if (queryParametersList != null)
                {
                    foreach (String[] parameter in queryParametersList)
                    {
                        if (String.IsNullOrEmpty(parameter[0]) || String.IsNullOrEmpty(parameter[1]))
                        {
                            continue;
                        }

                        request.AddQueryParameter(parameter[0], parameter[1]);
                    }
                }

                switch (requiredMethod)
                {
                    case RequiredHttpMethod.GET:
                        RestResponse = await client.ExecuteAsync(request, Method.Get);
                        break;

                    case RequiredHttpMethod.POST:
                        RestResponse = await client.ExecuteAsync(request, Method.Post);
                        break;

                    case RequiredHttpMethod.PATCH:
                        RestResponse = await client.ExecuteAsync(request, Method.Patch);
                        break;

                    case RequiredHttpMethod.DELETE:
                        RestResponse = await client.ExecuteAsync(request, Method.Delete);
                        break;

                    case RequiredHttpMethod.PUT:
                        RestResponse = await client.ExecuteAsync(request, Method.Put);
                        break;
                }

                if (RestResponse == null)
                {
                    response.success = false;
                    response.details = "Received Http response was null";
                    response.content = "";
                    return response;
                }

                if (RestResponse.ErrorException != null && !String.IsNullOrEmpty(RestResponse.ErrorException.ToString()))
                {
                    response.success = false;
                    response.details = RestResponse.ErrorException.ToString();
                }
                else
                {
                    response.success = true;
                    response.details = "";
                }

                response.content = RestResponse.Content;
                response.httpStatusCode = RestResponse.StatusCode.ToString();

                return response;
            }
            catch (Exception ex)
            {
                response.success = false;
                response.details = ex.ToString();

                if (RestResponse != null && !String.IsNullOrEmpty(RestResponse.Content))
                {
                    response.content = RestResponse.Content;
                }

                if (RestResponse != null)
                {
                    response.httpStatusCode = RestResponse.StatusCode.ToString();
                }

                return response;
            }
        }
    }
}