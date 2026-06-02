namespace PROG7311_PoE.Services
{
    public class AuthorizationService
    {
        private readonly HttpClient _httpClient;

        public AuthorizationService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> LogUserInAsync(string email, string password)
        {
            var dataFromLoginForm = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("email", email),
                new KeyValuePair<string, string>("password", password)
            });

            var response = await _httpClient.PostAsync("api/account/login", dataFromLoginForm);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> LogUserOutAsync()
        {
            var response = await _httpClient.PostAsync("api/account/logout", null);
            return response.IsSuccessStatusCode;
        }
    }
}
