using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Net.Http;
using System.Threading.Tasks;

namespace EEGfront
{
    public class Rest
    {
        private static Rest instance;
        private static readonly HttpClient client = new HttpClient();

        private Rest()
        {
            client.Timeout = new TimeSpan(0, 0, 2);

        }

        public static Rest Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Rest();
                }
                return instance;
            }
        }

        private string ExceptionParse(Exception e)
        {
            string source = e.Source;
            string message = e.Message;
            string stack = e.StackTrace;
            Exception innerEx = e.InnerException;
            string innerStr = string.Empty;
            string hResult = e.HResult.ToString();
            string help = e.HelpLink;

            if (string.IsNullOrEmpty(source))
                source = "N.A";

            if (string.IsNullOrEmpty(message))
                message = "N.A";

            if (string.IsNullOrEmpty(stack))
                stack = "N.A";

            if (innerEx == null)
                innerStr = "FALSE";
            else
                innerStr = "TRUE";

            if (string.IsNullOrEmpty(hResult))
                hResult = "N.A";

            if (string.IsNullOrEmpty(help))
                help = "N.A";

            string exc = string.Format("Get Failed! Invalid Shmeltz!!!\n\n" +
                                     "MESSAGE:\n{0}\n" +
                                     "SOURCE:\n{1}\n" +
                                     "STACK:\n{2}\n" +
                                     "HAS INNER EXCEPTION:\n{3}\n" +
                                     "HRESULT:\n{4}\n" +
                                     "HELP LINK:\n{5}\n\n",
                                     message, source, stack, innerStr, hResult, help);

            return exc;
        }

        public void OPEN()
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString =
            "Data Source=ServerName;" +
            "Initial Catalog=DataBaseName;" +
            "User id=UserName;" +
            "Password=Secret;";
            //conn.Open();
        }

        public async Task<string> Get(string url)
        {
            var response = await client.GetAsync(url);

            string responseString = await response.Content.ReadAsStringAsync();
            Console.WriteLine(responseString);
            return responseString;

        }

        public async Task<string> PostPrev(string user_id)
        {
            var values = new Dictionary<string, string>
            {
                { "user_id", user_id },
            };

            var content = new FormUrlEncodedContent(values);

            var response = await client.PostAsync("https://99.224.57.104:5900/rest/prevmodel/", content);

            string responseString = await response.Content.ReadAsStringAsync();
            Console.WriteLine(responseString);
            return responseString;
        }

        public async Task<string> PostCurrent(string user_id)
        {
            var values = new Dictionary<string, string>
            {
                { "user_id", user_id },
            };

            var content = new FormUrlEncodedContent(values);

            var response = await client.PostAsync("https://99.224.57.104:5900/rest/currentmodel/", content);

            string responseString = await response.Content.ReadAsStringAsync();
            Console.WriteLine(responseString);
            return responseString;
        }
    }
}