using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using LaRottaO.CSharp.Utils.RestSharpWrapper.Models;
using RestSharp;

namespace LaRottaO.CSharp.Utils.RestSharpWrapper
{
    public class RestSharpWrapper
    {
        private RestClient client;

        public CookieCollection getCookieCollection(String url)
        {
            //Only works with RestSharp up to 108.0.3
            return client.CookieContainer.GetCookies(new Uri(url));
        }

        public async Task<RestSharpResponse> makeRestRequest(RestSharpRequest request)
        {
            RestSharpResponse response = new RestSharpResponse();
            response.success = false;
            response.content = "";
            response.details = "";

            if (String.IsNullOrEmpty(request.endPointUrl))
            {
                response.success = false;
                response.details = $"HTTP REQUEST ERROR: ENDPOINT URL IS NULL";
                response.content = "";
                return response;
            }

            RestResponse restSharpRestResponse = null;

            try

            {
                var options = new RestClientOptions(request.endPointUrl)
                {
                    RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true,
                    ThrowOnAnyError = true,
                    MaxTimeout = 360000
                };

                if (client == null)
                {
                    if (!request.checkSSL)
                    {
                        client = new RestClient(options);
                    }
                    else
                    {
                        client = new RestClient();
                    }
                }

                if (request.defaultHeadersList != null)
                {
                    foreach (String[] defaultHeader in request.defaultHeadersList)
                    {
                        if (String.IsNullOrEmpty(defaultHeader[0]) || String.IsNullOrEmpty(defaultHeader[1]))
                        {
                            continue;
                        }

                        client.AddDefaultHeader(defaultHeader[0], defaultHeader[1]);
                        Debug.WriteLine($"DEBUG: ADDED DEFAULT HEADER: KEY {defaultHeader[0]} VALUE: {defaultHeader[1]}");
                    }
                }

                if (request.defaultParametersList != null)
                {
                    foreach (String[] defaultParameter in request.defaultParametersList)
                    {
                        if (String.IsNullOrEmpty(defaultParameter[0]) || String.IsNullOrEmpty(defaultParameter[1]))
                        {
                            continue;
                        }

                        client.AddDefaultParameter(defaultParameter[0], defaultParameter[1]);
                        Debug.WriteLine($"DEBUG: ADDED DEFAULT PARAMETER: KEY {defaultParameter[0]} VALUE: {defaultParameter[1]}");
                    }
                }

                RestRequest restSharpRestRequest = new RestRequest(request.endPointUrl);

                if (request.queryParametersList != null)
                {
                    foreach (String[] queryParameter in request.queryParametersList)
                    {
                        if (String.IsNullOrEmpty(queryParameter[0]) || String.IsNullOrEmpty(queryParameter[1]))
                        {
                            continue;
                        }

                        restSharpRestRequest.AddQueryParameter(queryParameter[0], queryParameter[1]);
                        Debug.WriteLine($"DEBUG: ADDED QUERY PARAMETER: KEY {queryParameter[0]} VALUE {queryParameter[1]}");
                    }
                }

                switch (request.requiredBodyType)
                {
                    case RequiredBodyType.APPLICATION_JSON:

                        restSharpRestRequest.AddOrUpdateParameter("application/json", request.body, ParameterType.RequestBody);

                        break;

                    case RequiredBodyType.APPLICATION_FORM_URL_ENCODED:

                        restSharpRestRequest.AddOrUpdateParameter("application/x-www-form-urlencoded", request.body, ParameterType.RequestBody);

                        break;

                    case RequiredBodyType.TEXT_PLAIN:

                        restSharpRestRequest.AddOrUpdateParameter("text/plain", request.body, ParameterType.RequestBody);

                        break;
                }

                try
                {
                    switch (request.requiredMethod)
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

            if (ex != null)
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