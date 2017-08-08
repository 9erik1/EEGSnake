using Accord.MachineLearning.VectorMachines;
using Accord.Statistics.Kernels;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
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

        private void LogResponse(HttpResponseMessage resp)
        {
            Console.WriteLine(resp.StatusCode);
            Console.WriteLine(resp.Headers);
        }

        public async Task<string> Get(string url)
        {
            var response = await client.GetAsync(url);

            string responseString = await response.Content.ReadAsStringAsync();
            LogResponse(response);
            Console.WriteLine(responseString);
            return responseString;

        }

        public async Task<string> PostPrev(string user_id)
        {
            var values = new Dictionary<string, string>
            {
                { "user_id", user_id }
            };

            var content = new FormUrlEncodedContent(values);

            var response = await client.PostAsync("https://99.224.57.104:5900/rest/prevmodel/", content);

            string responseString = await response.Content.ReadAsStringAsync();
            LogResponse(response);
            Console.WriteLine(responseString);
            return responseString;
        }
        public static Stream GenerateStreamFromString(string s)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
        public async Task<string> PostCurrent(string user_id)
        {
            var values = new Dictionary<string, string>
            {
                { "user_id", user_id }
            };

            var content = new FormUrlEncodedContent(values);

            var response = await client.PostAsync("https://99.224.57.104:5900/rest/currentmodel/", content);

            string responseString = await response.Content.ReadAsStringAsync();
            JObject json = JObject.Parse(responseString);

            JToken contentr = json["current_model"]["current_model"]["data"];
            string resp = contentr.ToString(Newtonsoft.Json.Formatting.None);

            byte[] temp = Encoding.UTF8.GetBytes(resp);
            MemoryStream s = new MemoryStream(temp);


 
             
            
            MulticlassSupportVectorMachine<Gaussian> proxyLearn = null;
            string leel;
            // Open the file containing the data that you want to deserialize.
            //Stream fs = await response.Content.ReadAsStreamAsync();
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();

                // Deserialize the hashtable from the file and 
                // assign the reference to the local variable.

                //leel = (string)formatter.Deserialize(fs);
                proxyLearn = (MulticlassSupportVectorMachine<Gaussian>)formatter.Deserialize(s);                
            }
            catch (SerializationException e)
            {
                Console.WriteLine("Failed to deserialize. Reason: " + e.Message);
                throw;
            }
            finally
            {
                //fs.Close();
            }
            
            // To prove that the table deserialized correctly, 
            // display the key/value pairs.

            //var lel = proxyLearn;

            LogResponse(response);
            //Console.WriteLine(responseString);
            return responseString;
        }

        public async Task<string> UpdateModel(string user_id, Stream content)
        {
            var values = new Dictionary<string, string>();
            content.Position = 0;
            using (StreamReader reader = new StreamReader(content, Encoding.UTF8))
            {

                values = new Dictionary<string, string>
                {
                    { "user_id", user_id },
                    { "current_model", await reader.ReadToEndAsync() }
                };
            }


            var postContent = new FormUrlEncodedContent(values);

            var response = await client.PostAsync("https://99.224.57.104:5900/rest/updatemodel/", postContent);

            string responseString = await response.Content.ReadAsStringAsync();
            LogResponse(response);
            Console.WriteLine(responseString);
            return responseString;
        }
    }
}