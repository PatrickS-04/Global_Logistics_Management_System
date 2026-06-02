using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.Text.Json;

namespace PROG7311_PoE.Services
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly HttpClient _httpClient;

        public CustomAuthStateProvider(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var anonymousState = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

            try
            {
                var response = await _httpClient.GetAsync("api/account/userState");

                if (!response.IsSuccessStatusCode)
                {
                    return anonymousState;
                }

                using var doc = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
                var root = doc.RootElement;

                if (!root.GetProperty("isAuthenticated").GetBoolean())
                {
                    return anonymousState;
                }

                var identity = new ClaimsIdentity("BackendCookieAuth");
                identity.AddClaim(new Claim(ClaimTypes.Name, root.GetProperty("name").GetString() ?? ""));

                foreach (var roleElement in root.GetProperty("roles").EnumerateArray())
                {
                    identity.AddClaim(new Claim(ClaimTypes.Role, roleElement.GetString() ?? ""));
                }

                return new AuthenticationState(new ClaimsPrincipal(identity));
            }
            catch
            {
                return anonymousState;
            }
        }

        public void NotifyUserAuthenticationStateChanged()
        {
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public void NotifyUserLoggedOut()
        {
            var anonymousPrincipal = new ClaimsPrincipal(new ClaimsIdentity());
            var anonymousState = Task.FromResult(new AuthenticationState(anonymousPrincipal));

            NotifyAuthenticationStateChanged(anonymousState);
        }
    }
}
