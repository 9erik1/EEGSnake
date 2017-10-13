using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Net.Http;
using System.Threading.Tasks;

namespace GateKeep
{
    public class Cloud
    {
        private static Cloud instance;
        private static readonly HttpClient client = new HttpClient();

        private Cloud()
        {
            client.Timeout = new TimeSpan(0, 0, 2);
        }

        public static Cloud Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Cloud();
                }
                return instance;
            }
        }

        public async Task<string> LogIn(string user, string pass)
        {
            var values = new Dictionary<string, string>
            {
                { "username", user },
                { "pass", pass }
            };

            var content = new FormUrlEncodedContent(values);

            var response = await client.PostAsync("https://zotac.mikedev.ca:5900/rest/login/",content);

            string responseString = await response.Content.ReadAsStringAsync();

            return responseString;
        }
    }
}
