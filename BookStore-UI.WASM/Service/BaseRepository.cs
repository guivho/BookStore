using Blazored.LocalStorage;
using BookStore_UI.WASM.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace BookStore_UI.WASM.Service
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorageService;
        public BaseRepository
            (HttpClient httpClient, ILocalStorageService localStorageService)
        {
            _httpClient = httpClient;
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

        public async Task<IList<T>> Get(string url)
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

        private async Task<string> GetBearerToken()
        {
            return await _localStorageService.GetItemAsync<string>("authToken");
        }
    }
}
