using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace PROG7311_PoE.Components.Pages.Admin
{
    public partial class ChangeRole : ComponentBase
    {
        [Inject]
        public HttpClient Http { get; set; } = null!;

        protected List<AdminUserDto>? Users { get; set; }
        protected string? SelectedUserId { get; set; }
        protected string? NewRole { get; set; }
        protected string? StatusMessage { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await LoadUsers();
        }

        protected async Task LoadUsers()
        {
            Users = await Http.GetFromJsonAsync<List<AdminUserDto>>("api/admin/users");
        }

        protected async Task ChangeRoleSubmit()
        {
            if (string.IsNullOrEmpty(SelectedUserId) || string.IsNullOrEmpty(NewRole))
                return;

            var payload = new { UserId = SelectedUserId, NewRole = NewRole };
            var response = await Http.PostAsJsonAsync("api/admin/changeRole", payload);
            if (response.IsSuccessStatusCode)
            {
                StatusMessage = "Role changed.";
                await LoadUsers();
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
