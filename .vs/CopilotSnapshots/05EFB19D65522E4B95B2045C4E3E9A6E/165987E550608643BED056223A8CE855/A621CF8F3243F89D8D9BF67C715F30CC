using Microsoft.EntityFrameworkCore;
using PROG7311_PoE.Data;
using PROG7311_PoE.Models;
using System;
using System.Diagnostics.Contracts;
using Contract = PROG7311_PoE.Models.Contract;

namespace PROG7311_PoE.Services
{
    public class ContractService : IContractService
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly IContractFactory _contractFactory;
        private readonly List<IObserver> _observers = new List<IObserver>();

        public ContractService(IDbContextFactory<AppDbContext> contextFactory, IContractFactory contractFactory, List<IObserver> observers)
        {
            // Injecting the DbContext Factory Interface to create a new database connection for each method so the same connection is not used across all requests
            _contextFactory = contextFactory;
            // Injecting the Contract Factory Interface for abstraction and to follow the Factory Design Pattern that i chose in Part 1
            _contractFactory = contractFactory;      
            _observers = observers;
        }

        public void AttachObserver(IObserver observer)
        {
            _observers.Add(observer);
        }

        public void DetachObserver(IObserver observer)
        {
            _observers.Remove(observer);
        }

        public void NotifyObservers(string message)
        {
            foreach (var observer in _observers)
            {
                observer.UpdateObservers(message);
            }
        }

        // ---------------------------------------------------------------------------------------------------------
        // Method returns a list of all Active Contracts
        public async Task<List<Contract>> GetActiveContracts()
        {
            using var contextFactoryDb = _contextFactory.CreateDbContext();
            return await contextFactoryDb.Contracts
                .Include(client => client.Client)
                .Where(contract => contract.contractStatus == ContractStatus.Active).ToListAsync();
        }

        // ---------------------------------------------------------------------------------------------------------
        // Method creates a new contract of either Standard, Premium or International type and saves it to the database
        public async Task<Contract> CreateNewContract(string type, int clientId, DateTime startDate, DateTime endDate)
        {
            var contract = _contractFactory.CreateContract(type);

            contract.ClientId = clientId;
            contract.startDate = startDate;
            contract.endDate = endDate;
            contract.contractStatus = ContractStatus.Draft;

            using var contextFactoryDb = _contextFactory.CreateDbContext();

            contextFactoryDb.Contracts.Add(contract);
            await contextFactoryDb.SaveChangesAsync();

            var newContract = await contextFactoryDb.Contracts
                .Include(c => c.Client)
                .FirstOrDefaultAsync(c => c.ContractId == contract.ContractId);

            if (newContract?.Client != null)
            {
                // Admins as well
                var clientEmailObserver = new ClientEmailObserver { email = contract.Client.clientEmail };
                var clientAlertObserver = new ClientAlertObserver { phoneNumber = contract.Client.clientPhoneNumber };

                AttachObserver(clientEmailObserver);
                AttachObserver(clientAlertObserver);

                NotifyObservers("A new contract has been created!");

                DetachObserver(clientEmailObserver);
                DetachObserver(clientAlertObserver);
            }

            return contract;
        }

        // ---------------------------------------------------------------------------------------------------------
        // Method returns a filtered list of contracts by date range and contract status
        public async Task<List<Contract>> GetFilteredContracts(DateTime? startDate, DateTime? endDate, ContractStatus? contractStatus)
        {
            using var contextFactoryDb = _contextFactory.CreateDbContext();
            var linqQuery = contextFactoryDb.Contracts.AsQueryable();

            if (startDate.HasValue)
            {
                linqQuery = linqQuery.Where(contract => contract.startDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                linqQuery = linqQuery.Where(contract => contract.endDate <= endDate.Value);
            }

            if (contractStatus.HasValue)
            {
                linqQuery = linqQuery.Where(contract => contract.contractStatus == contractStatus.Value);
            }

            return await linqQuery.ToListAsync();
        }

        // ---------------------------------------------------------------------------------------------------------
        // Method updated contract statuses and saves it to the database
        public async Task UpdateContractStatus(ContractStatus updatedContractStatus, int contractId)
        {
            using var contextFactoryDb = _contextFactory.CreateDbContext();

            var contract = await contextFactoryDb.Contracts
                .Include(contract => contract.Client)
                .Include(contract => contract.LogisticsManager)
                .FirstOrDefaultAsync(contract => contract.ContractId == contractId);

            if (contract != null)
            {
                var logisticsEmailObserver = new LogisticsManagerEmailObserver { email = contract.LogisticsManager.email};
                var clientEmailObserver = new ClientEmailObserver { email = contract.Client.clientEmail };
                var logisticsAlertObserver = new LogisticsManagerAlertObserver { phoneNumber = contract.LogisticsManager.phoneNumber };
                var clientAlertObserver = new ClientAlertObserver { phoneNumber = contract.Client.clientPhoneNumber };

                AttachObserver(logisticsEmailObserver);
                AttachObserver(clientEmailObserver);
                AttachObserver(logisticsAlertObserver);
                AttachObserver(clientAlertObserver);

                contract.contractStatus = updatedContractStatus;

                var contractStatusValidation = new CheckContractStatus();
                
                if (!contractStatusValidation.Validate(contract))
                {
                    throw new InvalidOperationException("Invalid contract status");
                }

                NotifyObservers($"The status for contract with {contractId} has been changed to {updatedContractStatus}");

                DetachObserver(logisticsEmailObserver);
                DetachObserver(clientEmailObserver);
                DetachObserver(logisticsAlertObserver);
                DetachObserver(clientAlertObserver);

                await contextFactoryDb.SaveChangesAsync();
            }
        }
    }
}
// --------------------------------------------------------000... End Of File ...000------------------------------------------------------------------
