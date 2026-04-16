using Microsoft.EntityFrameworkCore;
using PROG7311_PoE.Data;
using PROG7311_PoE.Models;

namespace PROG7311_PoE.Services
{
    public class ServiceRequestService
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly IConfiguration _configuration;

        public ServiceRequestService(IDbContextFactory<AppDbContext> contextFactory, IConfiguration configuration)
        {
            // Injecting the DbContext Factory Interface to create a new database connection for each method so the same connection is not used across all requests
            _contextFactory = contextFactory;
            _configuration = configuration;
        }

        // ---------------------------------------------------------------------------------------------------------
        // Method returns a boolean value indicating whether a service request can be created for a given contract based on its status
        public async Task<bool> IsContractEligibleForServiceRequestCreation(int contractId)
        {
            using var contextFactoryDb = _contextFactory.CreateDbContext();
            var contract = await contextFactoryDb.Contracts.FindAsync(contractId);

            if (contract == null)
            {
                return false;
            }

            if (contract.contractStatus == ContractStatus.Expired || contract.contractStatus == ContractStatus.OnHold)
            {
                return false;
            }

            var currencyConversionValidation = new CheckCurrencyConversion();
            if (!currencyConversionValidation.Validate(contract))
            {
                return false;
            }
            return true;
        }

        // ---------------------------------------------------------------------------------------------------------
        // Method creates a new service request after validating the contracts eligibility and then converting the cost to ZAR
        public async Task ServiceRequestCreation(ServiceRequest newServiceRequest, string chosenCurrency)
        {
            bool isContractEligible = await IsContractEligibleForServiceRequestCreation(newServiceRequest.ContractId);

            if (!isContractEligible)
            {
                throw new InvalidOperationException("Service Requests cannot be created for contracts with a status of 'Expired' or 'On Hold'");
            }

            decimal randAmount = await CurrencyConversion(newServiceRequest.cost, chosenCurrency);
            newServiceRequest.cost = randAmount;

            using var contextFactoryDb = _contextFactory.CreateDbContext();
            contextFactoryDb.ServiceRequests.Add(newServiceRequest);
            await contextFactoryDb.SaveChangesAsync();
        }

        // ---------------------------------------------------------------------------------------------------------
        // Method returns a decimal value from the API for the exchange rate between the chosen currency and ZAR
        public async Task<decimal> CurrencyConversion(decimal currencyAmount, string currencyConversionFrom)
        {
            string currencyConversionTo = "ZAR";
            string apiKeyUrl = _configuration["ConnectionStrings:API-Key"];

            using var httpClient = new HttpClient();
            {
                try
                {
                    string url = $"{apiKeyUrl}/{currencyConversionFrom}/{currencyConversionTo}";

                    var response = await httpClient.GetFromJsonAsync<ExchangeRateDetails>(url);

                    if (response != null && response.conversionResult == "success")
                    {
                        return currencyAmount * response.exchangeRate;
                    }

                    throw new Exception("The API did not produce a successful result");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"API Currency Error: {ex.Message}");

                    decimal estimatedExchangeRate;
                    switch (currencyConversionFrom)
                    {
                        case "USD":
                            estimatedExchangeRate = 17.00m;
                            break;
                        case "GBP":
                            estimatedExchangeRate = 22.50m;
                            break;
                        case "AUD":
                            estimatedExchangeRate = 11.50m;
                            break;
                        case "EUR":
                            estimatedExchangeRate = 19.50m;
                            break;
                        default:
                            estimatedExchangeRate = 5.00m;
                            break;
                    }

                    Console.WriteLine($"API Not working. Using Estimated exchange rates for {currencyConversionFrom} : {estimatedExchangeRate}");
                    return currencyAmount * estimatedExchangeRate;
                }
            }
        }
    }
}
// --------------------------------------------------------000... End Of File ...000------------------------------------------------------------------
