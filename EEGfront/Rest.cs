using Accord.IO;
using Accord.MachineLearning.VectorMachines;
using Accord.Statistics.Kernels;
using MyObjSerial;
using Newtonsoft.Json.Linq;
using OxyPlot;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
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

            var response = await client.PostAsync("http://192.168.0.101:3222/currentmodel/", content);

            //string responseString = await response.Content.ReadAsStringAsync();
            //JObject json = JObject.Parse(responseString);

            //JToken contentr = json["current_model"]["current_model"]["data"];
            //var lel  = contentr.ToObject<byte[]>();

            //string result = Encoding.UTF8.GetString(lel);
            Stream c = await response.Content.ReadAsStreamAsync();

            MulticlassSupportVectorMachine<Gaussian> asd = Serializer.Load<MulticlassSupportVectorMachine<Gaussian>>(c);

            //byte[] asd = Encoding.UTF8.GetBytes(result);

            //using (MemoryStream s = new MemoryStream(asd))

            //{
            //    try
            //    {

            //        //MulticlassSupportVectorMachine<Gaussian> asd = Serializer.Load<MulticlassSupportVectorMachine<Gaussian>>(payload);



            //        BinaryFormatter br = new BinaryFormatter();

            //        var transvestite = Accord.IO.Serializer.Load(s, out br, SerializerCompression.None);
               

            //        var trans = 0;

            //    }
            //    catch (SerializationException e)
            //    {
            //        Console.WriteLine("Failed to deserialize. Reason: " + e.Message);
            //        throw;
            //    }
            //    catch (Exception ex)
            //    {
            //        Console.WriteLine("Failed to deserialize. Reason: " + ex.Message);
            //    }
            //    finally
            //    {
            //        //fs.Close();
            //    }
            //}
            LogResponse(response);
            //Console.WriteLine(responseString);
            return "";
        }

        public async Task<string> UpdateModel(string user_id, Stream content)
        { 

            MultipartFormDataContent formdata = new MultipartFormDataContent();

            formdata.Add(new StringContent(user_id), "user_id");
            //var cont = new StreamContent(content);
            formdata.Add(new StreamContent(content), "current_model", "learn.zip");

            HttpResponseMessage response = await client.PostAsync("http://192.168.0.101:3222/updatemodel", formdata);

            string responsestring = await response.Content.ReadAsStringAsync();
            LogResponse(response);
            Console.WriteLine("1: " + responsestring);

            return responsestring;
        }
    }
}