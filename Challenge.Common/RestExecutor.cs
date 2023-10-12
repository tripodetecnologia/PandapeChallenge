using System.Net.Http.Headers;
using System.Net;
using System.Web;
using Newtonsoft.Json;

namespace Challenge.Common
{
    public class RestExecutor
    {
        private static HttpClient client;

        public enum Verb
        {
            Get = 0,
            Post = 1,
            Put = 2,
            Delete = 3
        }

        public static async Task<T> ExecuteRequest<T>(string urlBase,
                                                        string method,
                                                        bool validateServerCertify,
                                                        Verb restVerb,
                                                        Dictionary<string, object> headers,
                                                        object parameters)
        {

            try
            {
                ValidateParameters(urlBase, method);
                Initialize(headers, validateServerCertify);

                switch (restVerb)
                {
                    case Verb.Get:
                        return await ExecuteGet<T>(urlBase, method, parameters);
                    case Verb.Post:
                        return await ExecutePost<T>(urlBase, method, parameters);
                    case Verb.Put:
                        return await ExecutePut<T>(urlBase, method, parameters);
                    case Verb.Delete:
                        return await ExecuteDelete<T>(urlBase, method, parameters);
                    default:
                        throw new InvalidOperationException("Method or request type not valid.");
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }

        private static void Initialize(Dictionary<string, object> headers, bool validateServerCertify)
        {
            client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();

            if (validateServerCertify)
            {
                ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
            }
            if (headers != null)
            {
                foreach (KeyValuePair<string, object> _header in headers)
                {
                    if (_header.Value != null)
                    {
                        client.DefaultRequestHeaders.Add(_header.Key, Convert.ToString(_header.Value));
                    }
                }
            }
        }

        private static async Task<T> ExecuteGet<T>(string urlBase, string method, object parameters)
        {
            string urlExecute = string.Empty;
            string backSlash = urlBase.EndsWith("/") ? string.Empty : "/";

            UriBuilder uriBuilder = new UriBuilder(string.Concat(urlBase, backSlash, method));
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            if (parameters != null)
            {
                foreach (var _property in parameters.GetType().GetProperties())
                {
                    query[_property.Name] = Convert.ToString(_property.GetValue(parameters));
                }
            }
            uriBuilder.Query = query.ToString();
            urlExecute = uriBuilder.ToString();

            HttpResponseMessage response = await client.GetAsync(urlExecute);

            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync());
            }
            return default(T);
        }

        private static async Task<T> ExecutePost<T>(string urlBase, string method, object parameters)
        {
            try
            {
                client.BaseAddress = new Uri(urlBase);
                string modeloJson = JsonConvert.SerializeObject(parameters);
                StringContent content = new StringContent(modeloJson);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                HttpResponseMessage response = await client.PostAsync(method, content);

                if (response.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync());
                }

                return default(T);
            }
            catch (Exception ex)
            {
                throw;
            }


        }

        private static async Task<T> ExecutePut<T>(string urlBase, string method, object parameters)
        {
            client.BaseAddress = new Uri(urlBase);
            string modeloJson = JsonConvert.SerializeObject(parameters);
            StringContent content = new StringContent(modeloJson);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = await client.PutAsync(method, content);

            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync());
            }
            return default(T);
        }

        /// <summary>
        /// You must send a bool type to return true or false.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="urlBase"></param>
        /// <param name="method"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private static async Task<T> ExecuteDelete<T>(string urlBase, string method, object parameters)
        {
            string urlExecute = string.Empty;
            string backSlash = urlBase.EndsWith("/") ? string.Empty : "/";

            UriBuilder uriBuilder = new UriBuilder(string.Concat(urlBase, backSlash, method));
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            if (parameters != null)
            {
                foreach (var _property in parameters.GetType().GetProperties())
                {
                    query[_property.Name] = Convert.ToString(_property.GetValue(parameters));
                }
            }
            uriBuilder.Query = query.ToString();
            urlExecute = uriBuilder.ToString();

            HttpResponseMessage response = await client.DeleteAsync(urlExecute);

            return JsonConvert.DeserializeObject<T>(new { response.IsSuccessStatusCode }.ToString());

        }

        private static void ValidateParameters(string urlBase, string method)
        {
            if (string.IsNullOrWhiteSpace(urlBase))
            {
                throw new ArgumentException("Parameter urlBase is required.");
            }
            if (string.IsNullOrWhiteSpace(method))
            {
                throw new ArgumentException("Parameter method is required.");
            }
        }

    }
}
