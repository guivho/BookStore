using BookStore_UI.Contracts;
using BookStore_UI.Static;
using BookStore_UI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using BookStore_UI.Providers;

namespace BookStore_UI.Service
{
    public class AuthenticationRepository : IAuthenticationRepository
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILocalStorageService _localStorageServive;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        public AuthenticationRepository(IHttpClientFactory httpClientFactory
            , ILocalStorageService localStorageService
            , AuthenticationStateProvider authenticationStateProvider)
        {
            _httpClientFactory = httpClientFactory;
            _localStorageServive = localStorageService;
            _authenticationStateProvider = authenticationStateProvider;
        }

        public async Task<bool> Login(LoginModel user)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, Endpoints.LoginEndpoint)
            {
                Content = new StringContent(JsonConvert.SerializeObject(user)
                , Encoding.UTF8, "application/json")
            };
            var client = _httpClientFactory.CreateClient();
            HttpResponseMessage response = await client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                return false;
            }
            var content = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(content);

            // Store token
            await _localStorageServive.SetItemAsync("authToken", tokenResponse.Token);

            // Change Authentication State
            await ((ApiAuthenticationStateProvider)_authenticationStateProvider).LogIn();

            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", tokenResponse.Token);
            return true;
        }

        public async Task Logout()
        {
            await _localStorageServive.RemoveItemAsync("authToken");
            //((ApiAuthenticationStateProvider)_authenticationStateProvider).Logout();
        }

        public async Task<bool> Register(RegistrationModel user)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, Endpoints.RegisterEndpoint)
            {
                Content = new StringContent(JsonConvert.SerializeObject(user)
                , Encoding.UTF8, "application/json")
            };
            var client = _httpClientFactory.CreateClient();
            HttpResponseMessage response = await client.SendAsync(request);
            return response.IsSuccessStatusCode;
        }
    }
}
