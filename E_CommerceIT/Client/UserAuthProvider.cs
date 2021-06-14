using Blazored.LocalStorage;
using E_CommerceIT.Shared.Models;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace E_CommerceIT.Client
{
    public class UserAuthProvider : AuthenticationStateProvider
    {
        public static AuthenticationState _authState;
        private readonly HttpClient _http;

        public UserAuthProvider(HttpClient httpClient)
        {
            _http = httpClient;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {

            Users user = new();
            user = await _http.GetFromJsonAsync<Users>("User/GetGlobalUser");

            if (user.IsAdmin == 1 && user.IsCustomer == 0)
            {
                var adminClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.UserEmail),
                    new Claim(ClaimTypes.Name, user.UserEmail),
                    new Claim(ClaimTypes.Role, "isAdmin")
                };
                var adminClaimsId = new ClaimsIdentity(adminClaims, CookieAuthenticationDefaults.AuthenticationScheme);
                var adminPrincipal = new ClaimsPrincipal(adminClaimsId);
                AuthenticationState state = new AuthenticationState(adminPrincipal);

                NotifyAuthenticationStateChanged(Task.FromResult(state));

                return state;
            }
            else if(user.IsAdmin == 0 && user.IsCustomer == 1)
            {
                var customerClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.UserEmail),
                    new Claim(ClaimTypes.Name, user.UserEmail),
                    new Claim(ClaimTypes.Role, "isCustomer")
                };
                var customerClaimsId = new ClaimsIdentity(customerClaims, CookieAuthenticationDefaults.AuthenticationScheme);
                var customerPrincipal = new ClaimsPrincipal(customerClaimsId);
                AuthenticationState state = new AuthenticationState(customerPrincipal);

                NotifyAuthenticationStateChanged(Task.FromResult(state));

                return state;
            }
            else
            {
                var readonlyClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, "readonlyuser"),
                    new Claim(ClaimTypes.Name, ""),
                    new Claim(ClaimTypes.Role, "isReadOnly")
                };
                var readonlyClaimsId = new ClaimsIdentity(readonlyClaims, CookieAuthenticationDefaults.AuthenticationScheme);
                var readonlyPrincipal = new ClaimsPrincipal(readonlyClaimsId);
                AuthenticationState state = new AuthenticationState(readonlyPrincipal);

                NotifyAuthenticationStateChanged(Task.FromResult(state));

                return state;
            }  
        }
    }
}
