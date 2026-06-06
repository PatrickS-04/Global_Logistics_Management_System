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

        // ---------------------------------------------------------------------------------------------------------
        // Method sends HTTP Get request to backend endpoint to check whether a user is logged in or not
        // When a user logs in, a claimsidentity is built with name, id and role and then the api checks whether the user is logged in or not
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

                if (root.TryGetProperty("id", out var idElement))
                {
                    identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, idElement.GetString() ?? ""));
                }

                foreach (var role in root.GetProperty("roles").EnumerateArray())
                {
                    identity.AddClaim(new Claim(ClaimTypes.Role, role.GetString() ?? ""));
                }

                return new AuthenticationState(new ClaimsPrincipal(identity));
            }
            catch
            {
                return anonymousState;
            }
        }

        // ---------------------------------------------------------------------------------------------------------
        // Method tells the application to re-check the authentication status right after a user has logged in or logged out
        public void NotifyUserAuthenticationStateChanged()
        {
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        // ---------------------------------------------------------------------------------------------------------
        // Method clears out the claimsidentity and authentication state and logs user out
        public void NotifyUserLoggedOut()
        {
            var anonymousPrincipal = new ClaimsPrincipal(new ClaimsIdentity());
            var anonymousState = Task.FromResult(new AuthenticationState(anonymousPrincipal));

            NotifyAuthenticationStateChanged(anonymousState);
        }
    }
}
// --------------------------------------------------------000... End Of File ...000------------------------------------------------------------------
