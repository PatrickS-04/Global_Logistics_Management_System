using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;
using System.ComponentModel.DataAnnotations;

namespace PROG7311_PoE.Components.Pages.Admin
{
    public partial class CreateUser : ComponentBase
    {
        [Inject]
        public HttpClient Http { get; set; } = null!;

        protected CreateUserModel CreateModel { get; set; } = new CreateUserModel();
        protected string? StatusMessage { get; set; }

        protected async Task HandleValidSubmit()
        {
            var response = await Http.PostAsJsonAsync("api/admin/create", CreateModel);
            if (response.IsSuccessStatusCode)
            {
                StatusMessage = "User created successfully.";
                CreateModel = new CreateUserModel();
            }
            else
            {
                var txt = await response.Content.ReadAsStringAsync();
                StatusMessage = "Error: " + txt;
            }
        }

        public class CreateUserModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; } = null!;

            [Required]
            public string Password { get; set; } = null!;

            public string? Role { get; set; }
        }
    }
}
