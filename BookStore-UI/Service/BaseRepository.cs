using Blazored.LocalStorage;
using BookStore_UI.Contracts;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace BookStore_UI.Service
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILocalStorageService _localStorageService;
        public BaseRepository
            (IHttpClientFactory httpClientFactory, ILocalStorageService localStorageService)
        {
            _httpClientFactory = httpClientFactory;
            _localStorageService = localStorageService;
        }
        public async Task<bool> Create(string url, T obj)
        {
            if (obj == null)
            {
                return false;
            }
            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json")
            };

            var client = await CreateHttpClient("create", obj.ToString());
            HttpResponseMessage response = await client.SendAsync(request);
            DebugResponseStatusCode("create", response.StatusCode, HttpStatusCode.Created);
            return response.StatusCode == HttpStatusCode.Created;
        }

        public async Task<bool> Delete(string url, int id)
        {
            if (id < 1)
            {
                return false;
            }
            var request = new HttpRequestMessage(HttpMethod.Delete, url+id);

            var client = await CreateHttpClient("delete", id.ToString());
            HttpResponseMessage response = await client.SendAsync(request);
            DebugResponseStatusCode("delete", response.StatusCode, HttpStatusCode.NoContent);
            return response.StatusCode == HttpStatusCode.NoContent;
        }

        public async Task<T> Get(string url, int id)
        {
            if (id < 1)
            {
                return null;
            }
            var request = new HttpRequestMessage(HttpMethod.Get, url+id);

            var client = await CreateHttpClient("get", id.ToString());
            HttpResponseMessage response = await client.SendAsync(request);

            if(response.StatusCode == HttpStatusCode.OK)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(content);
            }
            return null; 
        }

        public async Task<IList<T>> Get(string url)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);

            var client = await CreateHttpClient("get", "all");
            HttpResponseMessage response = await client.SendAsync(request);

            if(response.StatusCode == HttpStatusCode.OK)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<IList<T>>(content);
            }
            return null; 
        }

        public async Task<bool> Update(string url, T obj, int id)
        {
            if (obj == null)
            {
                return false;
            }
            var request = new HttpRequestMessage(HttpMethod.Put, url+id)
            {
                Content = new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json")
            };

            var client = await CreateHttpClient("update", obj.ToString());
            HttpResponseMessage response = await client.SendAsync(request);
            DebugResponseStatusCode("update", response.StatusCode, HttpStatusCode.NoContent);
            return response.StatusCode == HttpStatusCode.NoContent;
        }

        private async Task<HttpClient> CreateHttpClient(string method, string obj)
        {
            var client = _httpClientFactory.CreateClient();
            var token = await _localStorageService.GetItemAsync<string>("authToken");
            client.DefaultRequestHeaders.Authorization
                = new AuthenticationHeaderValue("bearer", token);
            DebugAuthorizationHeader(method, obj, client);
            return client;
        }

        [ConditionalAttribute("DEBUG")]
        private void DebugResponseStatusCode(string method, HttpStatusCode httpStatusCode, HttpStatusCode requiredStatusCode)
        {
            Debug.WriteLine($"\"{method}\" response.StatusCode => " +
                $" \"{(int)httpStatusCode} {httpStatusCode}\"" +
                $" \"Ok={httpStatusCode == requiredStatusCode}\"");
        }

        [ConditionalAttribute("DEBUG")]
        private void DebugAuthorizationHeader(string method, string obj, HttpClient httpClient)
        {
            //Debug.WriteLine($"{method}: {obj} \"{httpClient.DefaultRequestHeaders.Authorization}\"");
            Debug.WriteLine($"{method}: {obj}");
        }
    }
}
