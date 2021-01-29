using Blazored.LocalStorage;
using BookStore_UI.WASM.Contracts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
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

            _httpClient.DefaultRequestHeaders.Authorization
                = new AuthenticationHeaderValue("bearer", await GetBearerToken());
            DebugAuthorizationHeader("Create", obj.ToString(), _httpClient);
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync<T>(url, obj);
            DebugResponseStatusCode("Create", response.StatusCode, HttpStatusCode.Created);

            return response.StatusCode == System.Net.HttpStatusCode.Created;
        }

        public async Task<bool> Delete(string url, int id)
        {
            _httpClient.DefaultRequestHeaders.Authorization
                = new AuthenticationHeaderValue("bearer", await GetBearerToken());
            Debug.WriteLine($"Delete: id={id} \"{_httpClient.DefaultRequestHeaders.Authorization}\"");
            HttpResponseMessage response = await _httpClient.DeleteAsync(url + id);
            DebugResponseStatusCode("Delete", response.StatusCode, HttpStatusCode.NoContent);

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
            catch (Exception)
            {
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
            DebugAuthorizationHeader("Update", obj.ToString(), _httpClient);

            HttpResponseMessage response = await _httpClient.PutAsJsonAsync<T>(url + id, obj);
            DebugResponseStatusCode("Update", response.StatusCode, HttpStatusCode.NoContent);

            return response.StatusCode == HttpStatusCode.NoContent;
        }

        private async Task<string> GetBearerToken()
        {
            var token = await _localStorageService.GetItemAsync<string>("authToken");
            return token;
        }

        [ConditionalAttribute("DEBUG")]
        private void DebugResponseStatusCode(string method, HttpStatusCode httpStatusCode, HttpStatusCode requiredStatusCode)
        {
            Debug.WriteLine($"{method} response.StatusCode => " +
                $" {(int)httpStatusCode} {httpStatusCode}" +
                $" Ok={httpStatusCode == requiredStatusCode}");

        }

        [ConditionalAttribute("DEBUG")]
        private void DebugAuthorizationHeader(string method, string obj, HttpClient httpClient)
        {
            //Debug.WriteLine($"{method}: {obj} \"{_httpClient.DefaultRequestHeaders.Authorization}\"");
            Debug.WriteLine($"{method}: {obj}");
        }
    }       
}
