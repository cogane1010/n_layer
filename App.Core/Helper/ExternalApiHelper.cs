using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using RestSharp;


namespace App.Core.Helper
{
    public class ExternalApiHelper
    {
       //public static TResult PostBodyJson<TBody, TResult>(string url, Dictionary<string, string> headerParams, TBody body, int timeout = -1)
       //     where TBody : class
       //     where TResult : class
       // {
       //     var client = new RestClient(url);
       //     client.Timeout = timeout;
       //     var request = new RestRequest(Method.POST);
       //     foreach (KeyValuePair<string, string> p in headerParams)
       //     {
       //         request.AddHeader(p.Key, p.Value);
       //     }

       //     var bodyString = JsonConvert.SerializeObject(body);
       //     request.AddParameter("application/json", bodyString, ParameterType.RequestBody);
       //     IRestResponse response = client.Execute(request);
       //     var result = JsonConvert.DeserializeObject<TResult>(response.Content);
       //     return result;

       // }
    }
}
