using Accord.IO;
using Accord.MachineLearning.VectorMachines;
using Accord.Statistics.Kernels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
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

        private void LogResponse(HttpResponseMessage resp)
        {
            Console.WriteLine("----------START----------");
            Console.WriteLine(resp.StatusCode);
            Console.WriteLine(resp.Headers);
            Console.WriteLine(resp.RequestMessage.RequestUri);
            Console.WriteLine(resp.Content.Headers);
            Console.WriteLine("-----------END-----------");
        }

        public async Task<string> Get(string url)
        {
            var response = await client.GetAsync(url);

            string responseString = await response.Content.ReadAsStringAsync();
            LogResponse(response);
            Console.WriteLine(responseString);
            return responseString;

        }
        /// <summary>PostPrev is a method in the Rest class.
        /// <para>https urlencoded POST request. The body contains user_id and returns a JSON object containing that users previous classifier binaries from the db.</para>
        /// <seealso cref="Rest.cs"/>
        /// </summary>
        public async Task<string> PostPrev(string user_id)
        {
            var values = new Dictionary<string, string>
            {
                { "user_id", user_id }
            };

            var content = new FormUrlEncodedContent(values);

            var response = await client.PostAsync("https://zotac.mikedev.ca:5900/rest/prevmodel/", content);

            string responseString = await response.Content.ReadAsStringAsync();
            LogResponse(response);
            Console.WriteLine(responseString);
            return responseString;
        }

        /// <summary>PostCurrent is a method in the Rest class.
        /// <para>https urlencoded POST request. The body contains user_id and returns a JSON object containing that users current classifier binaries from the db.</para>
        /// <seealso cref="Rest.cs"/>
        /// </summary>
        public async Task<MulticlassSupportVectorMachine<Gaussian>> PostCurrent(string user_id)
        {
            var values = new Dictionary<string, string>
            {
                { "user_id", user_id }
            };

            var content = new FormUrlEncodedContent(values);

            var response = await client.PostAsync("https://zotac.mikedev.ca:5900/rest/currentmodel/", content);

            Stream c = await response.Content.ReadAsStreamAsync();

            MulticlassSupportVectorMachine<Gaussian> classifier = Serializer.Load<MulticlassSupportVectorMachine<Gaussian>>(c);

            LogResponse(response);
            return classifier;
        }

        /// <summary>UpdateModel is a method in the Rest class.
        /// <para>https multipart/formdata POST request. 
        /// The body contains user_id and the deserialized binaries of the classifer trained during the current app session. 
        /// returns a JSON response with a success or error message.</para>
        /// <seealso cref="Rest.cs"/>
        /// </summary>
        public async Task<string> UpdateModel(string user_id, Stream content)
        {

            MultipartFormDataContent formdata = new MultipartFormDataContent();

            formdata.Add(new StringContent(user_id), "user_id");
            formdata.Add(new StreamContent(content), "current_model", "learn.zip");

            HttpResponseMessage response = await client.PostAsync("https://zotac.mikedev.ca:5900/rest/updatemodel", formdata);

            string responsestring = await response.Content.ReadAsStringAsync();
            LogResponse(response);
            return responsestring;
        }

        /// <summary>UpdateModel is a method in the Rest class.
        /// <para>https urlencoded POST request.
        /// The body contains user_id.
        /// Returns a JSON response with a success or error message and the db 2nd most current EEG data from training input manager (TIM)</para>
        /// <seealso cref="Rest.cs"/>
        /// </summary>
        public async Task<string> PostPrevRaw(string user_id)
        {
            var values = new Dictionary<string, string>
            {
                { "user_id", user_id }
            };

            var content = new FormUrlEncodedContent(values);

            var response = await client.PostAsync("https://zotac.mikedev.ca:5900/rest/prevraw/", content);

            string responseString = await response.Content.ReadAsStringAsync();
            LogResponse(response);
            Console.WriteLine(responseString);
            return responseString;
        }

        /// <summary>PostCurrentRaw is a method in the Rest class.
        /// <para>https urlencoded POST request. 
        /// The body contains user_id.
        /// Returns a JSON response with a success or error message and the db most current EEG data from training input manager (TIM).</para>
        /// <seealso cref="Rest.cs"/>
        /// </summary>
        public async Task<TrainingInputManager> PostCurrentRaw(string user_id)
        {
            IFormatter formatter = new BinaryFormatter();

            var values = new Dictionary<string, string>
            {
                { "user_id", user_id }
            };

            var content = new FormUrlEncodedContent(values);

            var response = await client.PostAsync("https://zotac.mikedev.ca:5900/rest/currentraw/", content);

            Stream c = await response.Content.ReadAsStreamAsync();

            TrainingInputManager classifier = (TrainingInputManager)formatter.Deserialize(c);

            LogResponse(response);
            return classifier;
        }

        /// <summary>UpdateModelRaw Sends the TIM Raw.
        /// <para>https multipart/formdata POST request. The body contains user_id and the deserialized binaries of the training input manager(TIM) during the current app session. returns a JSON response with a success or error message.</para>
        /// <seealso cref="Rest.cs"/>
        /// </summary>
        public async Task<string> UpdateModelRaw(string user_id, Stream content)
        {

            MultipartFormDataContent formdata = new MultipartFormDataContent();

            formdata.Add(new StringContent(user_id), "user_id");
            formdata.Add(new StreamContent(content), "current_raw", "raw.zip");

            HttpResponseMessage response = await client.PostAsync("https://zotac.mikedev.ca:5900/rest/updateraw", formdata);

            string responsestring = await response.Content.ReadAsStringAsync();
            LogResponse(response);
            return responsestring;
        }
    }
}