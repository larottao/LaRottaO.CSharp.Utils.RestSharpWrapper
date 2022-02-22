using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;

namespace LaRottaO.CSharp.Utils.RestSharpWrapper
{
    public class RestSharpWrapper
    {
        public IRestResponse RestRequest(Method requiredMethod, String endPoint, List<String[]> headersList, List<String[]> parametersList, string body, DataFormat requiredFormat)
        {
            try
            {
                RestClient client = new RestClient();

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

                if (parametersList != null)
                {
                    foreach (String[] parameter in parametersList)
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

                if (iRestResponse != null && iRestResponse.Content.Contains("Try again in a few seconds"))
                {
                    return RestRequest(requiredMethod, endPoint, headersList, parametersList, body, requiredFormat);
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