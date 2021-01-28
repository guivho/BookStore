using Blazored.LocalStorage;
using BookStore_UI.WASM.Contracts;
using BookStore_UI.WASM.Models;
using BookStore_UI.WASM.Providers;
using BookStore_UI.WASM.Static;
using Microsoft.AspNetCore.Components.Authorization;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace BookStore_UI.WASM.Service
{
    public class AuthenticationRepository : IAuthenticationRepository
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorageService;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        public AuthenticationRepository(HttpClient httpClient
            , ILocalStorageService localStorageService
            , AuthenticationStateProvider authenticationStateProvider)
        {
            _httpClient = httpClient;
            _localStorageService = localStorageService;
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
            await _localStorageService.SetItemAsync("authToken", tokenResponse.Token);

            // Change Authentication State
            await ((ApiAuthenticationStateProvider)_authenticationStateProvider).LogIn();

            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", tokenResponse.Token);
            return true;
        }

        public async Task Logout()
        {
            await _localStorageService.RemoveItemAsync("authToken");
            ((ApiAuthenticationStateProvider)_authenticationStateProvider).Logout();
        }

        public async Task<bool> Register(RegistrationModel user)
        {
            var response = await _httpClient.PostAsJsonAsync(Endpoints.RegisterEndpoint, user);
            return response.IsSuccessStatusCode;
        }
    }
}
