using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.CSharp.RuntimeBinder;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;

namespace Silasary.WikiPlugin
{
    public class XivApi
    {
        private const string URL = "http://xivapi.com/";

        private static readonly Dictionary<string, JObject> cachedResponses = new Dictionary<string, JObject>();

        public static async Task<JObject> GetContentFinderCondition(int contentFinderCondition)
        {
            return await Get("ContentFinderCondition/" + contentFinderCondition, false);
        }


        public static async Task<dynamic> Get(string endpoint, bool noCache = false, params string[] parameters)
        {
            string requestParameters = "?";
            foreach (string str in parameters)
            {
                requestParameters = requestParameters + str + "&";
            }
            if (cachedResponses.ContainsKey(endpoint + requestParameters) && !noCache)
            {
                return cachedResponses[endpoint + requestParameters];
            }
            HttpClient httpClient = new HttpClient();
            JObject jObject = JObject.Parse(await (await httpClient.PostAsync("http://xivapi.com/" + endpoint, new StringContent(requestParameters))).Content.ReadAsStringAsync());
            if (!noCache)
            {
                cachedResponses.Add(endpoint + requestParameters, jObject);
            }
            return jObject;
        }
    }

}
