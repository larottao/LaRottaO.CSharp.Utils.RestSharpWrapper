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
        public IRestResponse MakeRestRequest(Method requiredMethod, String endPoint, List<String[]> headersList, List<String[]> parametersList, string body, DataFormat requiredFormat)
        {
            try
            {
                RestClient client = new RestClient();

                #region agrega headers

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
                    foreach (String[] parametro in parametersList)
                    {
                        if (String.IsNullOrEmpty(parametro[0]) || String.IsNullOrEmpty(parametro[1]))
                        {
                            continue;
                        }

                        client.AddDefaultParameter(parametro[0], parametro[1]);
                    }
                }

                #endregion agrega headers

                client.BaseUrl = new Uri(endPoint);

                var request = new RestRequest();

                if (body != null)
                {
                    request.AddJsonBody(body);
                }

                IRestResponse respuestaConsulta;

                switch (requiredMethod)
                {
                    case Method.GET:

                        respuestaConsulta = client.Get(request);

                        break;

                    case Method.POST:

                        respuestaConsulta = client.Post(request);

                        break;

                    case Method.PATCH:

                        respuestaConsulta = client.Patch(request);

                        break;

                    default:

                        Console.WriteLine("Unimplemented Method: " + requiredMethod);
                        respuestaConsulta = null;

                        break;
                }

                if (respuestaConsulta != null && respuestaConsulta.Content.Contains("Try again in a few seconds"))
                {
                    return MakeRestRequest(requiredMethod, endPoint, headersList, parametersList, body, requiredFormat);
                }

                return respuestaConsulta;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                return null;
            }
        }
    }
}