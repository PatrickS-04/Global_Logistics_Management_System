using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using TechMove.Backend_API.Data;
using TechMove.Database.Models;

namespace TechMove.Backend_API.Services
{
    public class ClientService
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        public ClientService(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        // ---------------------------------------------------------------------------------------------------------
        // Method gets client by their user Id
        public async Task<Client?> GetClientByUserId(string userId)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();

            return await context.Clients
                .FirstOrDefaultAsync(client => client.UserId == userId);
        }

        // ---------------------------------------------------------------------------------------------------------
        // Method gets clients contracts by using that user Id that was found
        public async Task<List<Contract>> GetClientsContracts(int clientId)
        {
            await using var dbContext = await _contextFactory.CreateDbContextAsync();
            return await dbContext.Contracts
                .Include(client => client.Client)
                .Where(contract => contract.ClientId == clientId).ToListAsync();
        }
    }
}
// --------------------------------------------------------000... End Of File ...000------------------------------------------------------------------
