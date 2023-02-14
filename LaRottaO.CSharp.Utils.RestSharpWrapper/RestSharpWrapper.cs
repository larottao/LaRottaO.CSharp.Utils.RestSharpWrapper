using System;
using System.Collections.Generic;
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

        public async Task<IRestResponse> restRequest(Method requiredMethod, String endPoint, List<String[]> headersList, List<String[]> defaultParametersList, List<String[]> queryParametersList, string body, DataFormat requiredFormat, Boolean checkSSL = false, Boolean createNewInstanceOnEachCall = true)
        {
            try
            {
                if (createNewInstanceOnEachCall)
                {
                    client = new RestClient();
                }

                client.CookieContainer = new System.Net.CookieContainer();

                //Prevents the error: RestSharp Could not establish trust relationship for the SSL/TLS secure channel

                if (!checkSSL)
                {
                    ServicePointManager.ServerCertificateValidationCallback +=
                          (sender, certificate, chain, sslPolicyErrors) => true;
                }

                //Exceute request

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

                client.BaseUrl = new Uri(endPoint);

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

                IRestResponse iRestResponse;

                switch (requiredMethod)
                {
                    case Method.GET:

                        iRestResponse = client.Get(request);

                        break;

                    case Method.POST:

                        iRestResponse = client.Post(request);

                        break;

                    case Method.PATCH:

                        iRestResponse = client.Patch(request);

                        break;

                    default:

                        Console.WriteLine("Unimplemented Method: " + requiredMethod);
                        iRestResponse = null;

                        break;
                }

                return iRestResponse;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                return null;
            }
        }
    }
}