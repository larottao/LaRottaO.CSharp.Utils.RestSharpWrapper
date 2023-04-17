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
            RestSharpResponse response = new RestSharpResponse();

            response.success = false;

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

                RestResponse iRestResponse = null;

                switch (requiredMethod)
                {
                    case RequiredHttpMethod.GET:

                        iRestResponse = client.Get(request);

                        break;

                    case RequiredHttpMethod.POST:

                        iRestResponse = client.Post(request);

                        break;

                    case RequiredHttpMethod.PATCH:

                        iRestResponse = client.Patch(request);

                        break;

                    case RequiredHttpMethod.DELETE:

                        iRestResponse = client.Delete(request);

                        break;

                    case RequiredHttpMethod.PUT:

                        iRestResponse = client.Put(request);

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

                ;
                response.success = true;
                response.content = iRestResponse.Content;
                response.httpStatusCode = iRestResponse.StatusCode.ToString();
                response.details = "";
                return response;
            }
            catch (Exception ex)
            {
                response.success = false;
                response.content = "";
                response.details = ex.ToString();
                return response;
            }
        }
    }
}