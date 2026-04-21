using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace PROG7311_PoE.Components.Pages.Admin
{
    public partial class DeleteUser : ComponentBase
    {
        [Inject]
        public HttpClient Http { get; set; } = null!;

        protected List<AdminUserDto>? Users { get; set; }
        protected string? SelectedUserId { get; set; }
        protected string? StatusMessage { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await LoadUsers();
        }

        protected async Task LoadUsers()
        {
            Users = await Http.GetFromJsonAsync<List<AdminUserDto>>("api/admin/users");
        }

        protected async Task DeleteSelected()
        {
            if (string.IsNullOrEmpty(SelectedUserId))
                return;

            var response = await Http.DeleteAsync($"api/admin/delete/{SelectedUserId}");
            if (response.IsSuccessStatusCode)
            {
                StatusMessage = "User deleted.";
                await LoadUsers();
                SelectedUserId = null;
            }
            else
            {
                StatusMessage = "Error: " + await response.Content.ReadAsStringAsync();
            }
        }

        public class AdminUserDto
        {
            public string Id { get; set; } = null!;
            public string Email { get; set; } = null!;
            public string Role { get; set; } = null!;
        }
    }
}
