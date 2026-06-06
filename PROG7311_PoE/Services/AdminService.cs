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

        // ---------------------------------------------------------------------------------------------------------
        // Method sends HTTP Get request to backend endpoint to return a list of users
        public async Task<List<AdminUserDto>> GetUsersAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<AdminUserDto>>("api/Admin/getUsers") ?? new List<AdminUserDto>();
        }

        // ---------------------------------------------------------------------------------------------------------
        // Method sends HTTP Post request to backend endpoint to create a new user using email, password and role
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

        // ---------------------------------------------------------------------------------------------------------
        // Method sends HTTP Delete request to backend endpoint to delete a user by their userId
        public async Task<HttpResponseMessage> DeleteUserAsync(string userId)
        {
            return await _httpClient.DeleteAsync($"api/Admin/deleteUser/{userId}");
        }

        // ---------------------------------------------------------------------------------------------------------
        // Method sends HTTP Post request to backend endpoint to change the role of a user
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
// --------------------------------------------------------000... End Of File ...000------------------------------------------------------------------
