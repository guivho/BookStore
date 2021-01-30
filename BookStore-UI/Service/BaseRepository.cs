using Blazored.LocalStorage;
using BookStore_UI.Contracts;
using Newtonsoft.Json;
using System.Collections.Generic;
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

            var client = await CreateHttpClient();
            HttpResponseMessage response = await client.SendAsync(request);
            if(response.StatusCode == System.Net.HttpStatusCode.Created)
            {
                return true;
            }
            return false; 

        }

        public async Task<bool> Delete(string url, int id)
        {
            if (id < 1)
            {
                return false;
            }
            var request = new HttpRequestMessage(HttpMethod.Delete, url+id);

            var client = await CreateHttpClient();
            HttpResponseMessage response = await client.SendAsync(request);
            if(response.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                return true;
            }
            return false; 
        }

        public async Task<T> Get(string url, int id)
        {
            if (id < 1)
            {
                return null;
            }
            var request = new HttpRequestMessage(HttpMethod.Get, url+id);

            var client = await CreateHttpClient();
            HttpResponseMessage response = await client.SendAsync(request);

            if(response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(content);
            }
            return null; 
        }

        public async Task<IList<T>> Get(string url)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);

            var client = await CreateHttpClient();
            HttpResponseMessage response = await client.SendAsync(request);

            if(response.StatusCode == System.Net.HttpStatusCode.OK)
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
            var request = new HttpRequestMessage(HttpMethod.Patch, url+id)
            {
                Content = new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json")
            };

            var client = await CreateHttpClient();
            HttpResponseMessage response = await client.SendAsync(request);
            if(response.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                return true;
            }
            return false; 
        }

        private async Task<HttpClient> CreateHttpClient()
        {
            var client = _httpClientFactory.CreateClient();
            var token = await _localStorageService.GetItemAsync<string>("authToken");
            client.DefaultRequestHeaders.Authorization
                = new AuthenticationHeaderValue("bearer", token);
            return client;
        }
    }
}
