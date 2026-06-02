using TechMove.Database.DTOs;

namespace PROG7311_PoE.Services
{
    public class AdminService
    {
        private readonly HttpClient _httpClient;

        public AdminService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<AdminUserDto>> GetUsersAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<AdminUserDto>>("api/Admin/getUsers") ?? new List<AdminUserDto>();
        }

        public async Task<HttpResponseMessage> CreateUserAsync(string email, string password, string role)
        {
            var dataFromForm = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("email", email),
                new KeyValuePair<string, string>("password", password),
                new KeyValuePair<string, string>("role", role)
            });

            return await _httpClient.PostAsync("api/Admin/createUser", dataFromForm);
        }

        public async Task<HttpResponseMessage> DeleteUserAsync(string userId)
        {
            return await _httpClient.DeleteAsync($"api/Admin/deleteUser/{userId}");
        }

        public async Task<HttpResponseMessage> ChangeUserRoleAsync(string userId, string newRole)
        {
            var dataFromForm = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("userId", userId),
                new KeyValuePair<string, string>("newRole", newRole)
            });

            return await _httpClient.PostAsync("api/Admin/changeRole", dataFromForm);
        }
    }
}
