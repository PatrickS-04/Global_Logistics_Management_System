using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PROG7311_PoE.Components;
using PROG7311_PoE.Services;

namespace PROG7311_PoE
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

            builder.Services.AddScoped(sp => new HttpClient
            {
                BaseAddress = new Uri("http://backend:8080/")
            });

            builder.Services.AddScoped<ContractService>();
            builder.Services.AddScoped<ServiceRequestService>();
            builder.Services.AddScoped<AdminService>();
            builder.Services.AddScoped<AuthorizationService>();

            builder.Services.AddCascadingAuthenticationState();

            builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {

                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseAntiforgery();
            app.UseStatusCodePagesWithReExecute("/not-found");

            app.MapStaticAssets();
            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            await app.RunAsync();
        }
    }
}
