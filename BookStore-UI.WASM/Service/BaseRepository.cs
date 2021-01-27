using Blazored.LocalStorage;
using BookStore_UI.WASM.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
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
            _httpClient.DefaultRequestHeaders.Authorization
                = new AuthenticationHeaderValue("bearer", await GetBearerToken());
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync<T>(url, obj);
            return response.StatusCode == System.Net.HttpStatusCode.Created;
        }

        public async Task<bool> Delete(string url, int id)
        {
            _httpClient.DefaultRequestHeaders.Authorization
                = new AuthenticationHeaderValue("bearer", await GetBearerToken());
            HttpResponseMessage response = await _httpClient.DeleteAsync(url + id);
            return response.StatusCode == System.Net.HttpStatusCode.NoContent;
        }

        public async Task<T> Get(string url, int id)
        {
            _httpClient.DefaultRequestHeaders.Authorization
                = new AuthenticationHeaderValue("bearer", await GetBearerToken());
            return await _httpClient.GetFromJsonAsync<T>(url + id);
        }

        public async Task<IList<T>> Get(string url)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization
                    = new AuthenticationHeaderValue("bearer", await GetBearerToken());
                return await _httpClient.GetFromJsonAsync<IList<T>>(url);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return null;
            }
        }

        public async Task<bool> Update(string url, T obj, int id)
        {
            if (obj == null)
            {
                return false;
            }
            _httpClient.DefaultRequestHeaders.Authorization
                = new AuthenticationHeaderValue("bearer", await GetBearerToken());
            HttpResponseMessage response = await _httpClient.PutAsJsonAsync<T>(url + id, obj);
            return response.StatusCode == System.Net.HttpStatusCode.NoContent;
        }

        private async Task<string> GetBearerToken()
        {
            return await _localStorageService.GetItemAsync<string>("authToken");
        }
    }
}
