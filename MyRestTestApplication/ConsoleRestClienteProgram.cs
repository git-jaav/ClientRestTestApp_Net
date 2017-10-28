using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Net;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

using MyRestTestApplication.EntityJson;

namespace MyRestTestApplication
{

    public class Class1
    {
        private const string URL_GEN = "http://10.10.2.77:8102/api/salud/agente/login";
        private string urlParameters = "?api_key=123";
        //private const string DATA = @"{""object"":{""name"":""Name""}}";
        private const string DATA_GEN = @"{""clave"": ""1"",""codigoAgente"": ""44488455""}";


        static void Main(string[] args)
        {

            Console.WriteLine("****INICIO TEST REST CLIENT ****");
            postRequest(URL_GEN,DATA_GEN);
            Console.WriteLine("****FIN TEST REST CLIENT ****");

            Console.WriteLine("presionar cualquier tecla para continuar");
            Console.ReadKey();
        }

        private static void getRequest(String URL)
        {

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(URL);

            // Add an Accept header for JSON format.
            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

            // List data response.           

            HttpResponseMessage response = client.GetAsync("ssa").Result;  // Blocking call!
            if (response.IsSuccessStatusCode)
            {
                // Parse the response body. Blocking!
                var dataObjects = response.Content.ReadAsAsync<IEnumerable<SG_AgenteJson>>().Result;
                if (dataObjects != null)
                {
                    foreach (var d in dataObjects)
                    {
                        Console.WriteLine("{1}", d.Nombre);
                    }
                }
            }
            else
            {
                Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
            }  
        }

        private static void postRequest(String URL, String DATA)
        {
            System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
            client.BaseAddress = new System.Uri(URL);

            //AUTH
            //byte[] cred = UTF8Encoding.UTF8.GetBytes("username:password");
            //client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(cred));

            // Add an Accept header for JSON format.
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));            

            System.Net.Http.HttpContent content = new StringContent(DATA, UTF8Encoding.UTF8, "application/json");
            HttpResponseMessage messge = client.PostAsync(URL, content).Result;
            string description = string.Empty;
            if (messge.IsSuccessStatusCode)
            {
                string result = messge.Content.ReadAsStringAsync().Result;

                description = result;
                Console.WriteLine("{0}", description);
                SG_AgenteJson agente = Deserialize<SG_AgenteJson>(result);
                if (agente !=null)
                {
                    Console.WriteLine("{1}", agente.Nombre);
                    Console.WriteLine("{2}", agente.FechaExpiracion);
                }
                
                
            }

            
        }


        private static void postRequest2(String URL, String DATA)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.ContentLength = DATA.Length;
            using (Stream webStream = request.GetRequestStream())
            using (StreamWriter requestWriter = new StreamWriter(webStream, UTF8Encoding.UTF8 ))
            //using (StreamWriter requestWriter = new StreamWriter(webStream, System.Text.Encoding.ASCII))
            {
                requestWriter.Write(DATA);
            }

            try
            {
                WebResponse webResponse = request.GetResponse();
                using (Stream webStream = webResponse.GetResponseStream())
                {
                    if (webStream != null)
                    {
                        using (StreamReader responseReader = new StreamReader(webStream))
                        {
                            string response = responseReader.ReadToEnd();
                            Console.Out.WriteLine(response);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.Out.WriteLine("-----------------");
                Console.Out.WriteLine(e.Message);
            }

        }


        public static string Serialize<T>(T obj)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
            MemoryStream ms = new MemoryStream();
            serializer.WriteObject(ms, obj);
            string retVal = Encoding.UTF8.GetString(ms.ToArray());
            return retVal;
        }

        public static T Deserialize<T>(string json)
        {
            T obj = Activator.CreateInstance<T>();
            MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(json));
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
            obj = (T)serializer.ReadObject(ms);
            ms.Close();
            return obj;
        }
    }
}
