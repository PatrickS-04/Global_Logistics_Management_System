using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using PROG7311_PoE.Data;
using PROG7311_PoE.Models;

namespace PROG7311_PoE.Services
{
    public class ClientService
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        public ClientService(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<Client?> GetClientByClientId(string userId)
        {
            using var context = _contextFactory.CreateDbContext();

            return await context.Clients
                .FirstOrDefaultAsync(client => client.UserId == userId);
        }
    }
}
