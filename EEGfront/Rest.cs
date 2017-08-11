using Accord.IO;
using Accord.MachineLearning.VectorMachines;
using Accord.Statistics.Kernels;
using MyObjSerial;
using Newtonsoft.Json.Linq;
using OxyPlot;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.IO.Compression;
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

        public string DecompressString(string compressedText)
        {
            byte[] gZipBuffer = Convert.FromBase64String(compressedText);
            using (var memoryStream = new MemoryStream())
            {
                int dataLength = BitConverter.ToInt32(gZipBuffer, 0);
                memoryStream.Write(gZipBuffer, 4, gZipBuffer.Length - 4);

                var buffer = new byte[dataLength];

                memoryStream.Position = 0;
                using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                {
                    gZipStream.Read(buffer, 0, buffer.Length);
                }

                return Encoding.UTF8.GetString(buffer);
            }
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
            var lel  = contentr.ToObject<byte[]>();

            string result = System.Text.Encoding.UTF8.GetString(lel);

            result = DecompressString(result);

            
            byte[] asd = Encoding.UTF8.GetBytes(result);

            using (MemoryStream s = new MemoryStream(asd))

            {
                try
                {

               



                    BinaryFormatter br = new BinaryFormatter();

                    var transvestite = Accord.IO.Serializer.Load(s, out br, SerializerCompression.None);
               

                    var trans = 0;

                }
                catch (SerializationException e)
                {
                    Console.WriteLine("Failed to deserialize. Reason: " + e.Message);
                    throw;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to deserialize. Reason: " + ex.Message);
                }
                finally
                {
                    //fs.Close();
                }
            }
            LogResponse(response);
            //Console.WriteLine(responseString);
            return responseString;
        }

        public static string CompressString(string text)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(text);
            var memoryStream = new MemoryStream();
            using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
            {
                gZipStream.Write(buffer, 0, buffer.Length);
            }

            memoryStream.Position = 0;

            var compressedData = new byte[memoryStream.Length];
            memoryStream.Read(compressedData, 0, compressedData.Length);

            var gZipBuffer = new byte[compressedData.Length + 4];
            Buffer.BlockCopy(compressedData, 0, gZipBuffer, 4, compressedData.Length);
            Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gZipBuffer, 0, 4);
            return Convert.ToBase64String(gZipBuffer);
        }


        public async Task<string> UpdateModel(string user_id, MemoryStream content)
        {
            var values = new Dictionary<string, string>();
            //content.Position = 0;
            using (StreamReader reader = new StreamReader(content, Encoding.ASCII))
            {
                string load = await reader.ReadToEndAsync();

                load = CompressString(load);


                values = new Dictionary<string, string>
                {
                    { "user_id", user_id },
                    { "current_model", load }
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