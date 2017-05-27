using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;




namespace GateKeep
{
    class Cloud
    {

        public Cloud()
        {

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

        public async Task<string> LogIn()
        {
            string html = string.Empty;
            try
            {
                string url = "https://99.242.214.17:5900/restusers/JeremiahSmith";
                
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    html = reader.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                //html = "Get Failed! Invalid Shmeltz!!! MESSAGE: " + message + " SOURCE: " + source + " STACK: " + stack + " HAS INNER EXCEPTION: " + innerStr + " HRESULT: " + hResult + " HELP LINK: " + help;
                html = ExceptionParse(e);
            }
            return html;
        }
    }
}
