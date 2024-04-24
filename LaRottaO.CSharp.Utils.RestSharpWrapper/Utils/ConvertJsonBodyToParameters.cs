using System;
using System.Collections.Generic;
using System.Text.Json;

namespace LaRottaO.CSharp.Utils.RestSharpWrapper
{
    public class ConvertJsonBodyToParameters
    {
        public Tuple<Boolean, String, List<string[]>> convertBodyJsonToHttpParameters(JsonElement request)
        {
            List<string[]> valueList = new List<string[]>();

            if (request.ValueKind == JsonValueKind.Null)
            { return new Tuple<Boolean, String, List<string[]>>(false, $"HTTP REQUEST ERROR: JSON IS NULL", valueList); }

            if (request.ValueKind == JsonValueKind.Undefined)
            { return new Tuple<Boolean, String, List<string[]>>(false, $"HTTP REQUEST ERROR: JSON IS MALFORMED", valueList); }

            if (String.IsNullOrEmpty(request.ToString().Trim()))
            { return new Tuple<Boolean, String, List<string[]>>(false, $"HTTP REQUEST ERROR: JSON IS EMPTY", valueList); }

            try
            {
                _ = request.GetRawText();
            }
            catch (JsonException jsonException)
            { return new Tuple<Boolean, String, List<string[]>>(false, $"HTTP REQUEST ERROR: {jsonException.Message}", valueList); }
            catch (Exception ex)
            { return new Tuple<Boolean, String, List<string[]>>(false, $"HTTP REQUEST ERROR: {ex.Message}", valueList); }

            try
            {
                List<String[]> queryParametersList = new List<String[]>();

                if (request.ValueKind == JsonValueKind.Object)
                {
                    foreach (JsonProperty property in request.EnumerateObject())
                    {
                        queryParametersList.Add(new String[] { property.Name, property.Value.ToString() });
                    }
                }

                return new Tuple<Boolean, String, List<string[]>>(true, String.Empty, valueList);
            }
            catch (Exception ex)
            {
                return new Tuple<Boolean, String, List<string[]>>(false, $"HTTP REQUEST ERROR: {ex.Message}", valueList);
            }
        }
    }
}