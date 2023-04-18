using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
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
            RestResponse iRestResponse = null;

            RestSharpResponse response = new RestSharpResponse();
            response.success = false;
            response.content = "";
            response.details = "";

            try

            {
                var options = new RestClientOptions(endPointUrl);

                if (createNewInstanceOnEachCall)
                {
                    client = new RestClient(options);
                }

                //client.CookieContainer = new System.Net.CookieContainer();
                //Prevents the error: RestSharp Could not establish trust relationship for the SSL/TLS secure channel

                if (!checkSSL)
                {
                    ServicePointManager.ServerCertificateValidationCallback +=
                          (sender, certificate, chain, sslPolicyErrors) => true;
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
                    request.AddJsonBody(body);
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
                        iRestResponse = await client.ExecuteAsync(request, Method.Get);
                        break;

                    case RequiredHttpMethod.POST:
                        iRestResponse = await client.ExecuteAsync(request, Method.Post);
                        break;

                    case RequiredHttpMethod.PATCH:
                        iRestResponse = await client.ExecuteAsync(request, Method.Patch);
                        break;

                    case RequiredHttpMethod.DELETE:
                        iRestResponse = await client.ExecuteAsync(request, Method.Delete);
                        break;

                    case RequiredHttpMethod.PUT:
                        iRestResponse = await client.ExecuteAsync(request, Method.Put);
                        break;
                }

                if (iRestResponse == null)
                {
                    response.success = false;
                    response.details = "Received Http response was null";
                    response.content = "";
                    return response;
                }

                if (String.IsNullOrEmpty(iRestResponse.Content))
                {
                    response.success = true;
                    response.details = "Received Http response has no content";
                    response.content = "";
                    return response;
                }

                response.content = iRestResponse.Content;
                response.httpStatusCode = iRestResponse.StatusCode.ToString();

                if (!iRestResponse.StatusCode.Equals(HttpStatusCode.OK))
                {
                    response.success = false;
                    response.details = iRestResponse.StatusDescription;
                    return response;
                }

                ;

                response.success = true;
                response.details = "";

                return response;
            }
            catch (Exception ex)
            {
                response.success = false;
                response.details = ex.ToString();

                if (iRestResponse != null && !String.IsNullOrEmpty(iRestResponse.Content))
                {
                    response.content = iRestResponse.Content;
                }

                if (iRestResponse != null)
                {
                    response.httpStatusCode = iRestResponse.StatusCode.ToString();
                }

                return response;
            }
        }
    }
}