using Microsoft.AspNetCore.Components.Authorization;
using Blazored.LocalStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace BookStore_UI.Providers
{
    public class ApiAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorage;
        private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler;

        public ApiAuthenticationStateProvider(ILocalStorageService localStorage
            , JwtSecurityTokenHandler jwtSecurityTokenHandler)
        {
            _localStorage = localStorage;
            _jwtSecurityTokenHandler = jwtSecurityTokenHandler;
        }
        public async override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var savedToken = await _localStorage.GetItemAsync<string>("authToken");
                if (string.IsNullOrWhiteSpace(savedToken))
                {
                    return NobodyIsLoggedIn();
                }
                var tokenContent = _jwtSecurityTokenHandler.ReadJwtToken(savedToken);
                var expiry = tokenContent.ValidTo;
                if (expiry < DateTime.Now)
                {
                    await _localStorage.RemoveItemAsync("authToken");
                    return NobodyIsLoggedIn();
                }
                // Get Claims from token and build auth user object
                var claims = ParseClaims(tokenContent);
                var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt"));

                // return authenticated user
                return new AuthenticationState(user);
            }
            catch (Exception)
            {
                return NobodyIsLoggedIn();
            }
        }

        public async Task LogIn()
        {
            var savedToken = await _localStorage.GetItemAsync<string>("authToken");
            var tokenContent = _jwtSecurityTokenHandler.ReadJwtToken(savedToken);
            var claims = ParseClaims(tokenContent);
            var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt"));
            var authState = Task.FromResult(new AuthenticationState(user));
            NotifyAuthenticationStateChanged(authState);
        }

        public void Logout()
        {
            var authState = Task.FromResult(new AuthenticationState(Nobody()));
            NotifyAuthenticationStateChanged(authState);
        }

        private ClaimsPrincipal Nobody()
        {
            return new ClaimsPrincipal(new ClaimsIdentity());
        }

        private AuthenticationState NobodyIsLoggedIn()
        {
            return new AuthenticationState(Nobody());
        }

        private IList<Claim> ParseClaims(JwtSecurityToken tokenContent)
        {
            var claims = tokenContent.Claims.ToList();
            claims.Add(new Claim(ClaimTypes.Name, tokenContent.Subject));
            return claims;
        }
    }
}
