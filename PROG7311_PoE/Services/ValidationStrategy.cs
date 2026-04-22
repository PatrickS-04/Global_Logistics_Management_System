using PROG7311_PoE.Models;

namespace PROG7311_PoE.Services
{
    public class CheckContractStatus : IValidationStrategy
    {
        // ---------------------------------------------------------------------------------------------------------
        // Method implements the IValidationStrategy interface
        public bool Validate(Contract contract)
        {
            return contract.contractStatus != ContractStatus.None;
        }
    }

    public class CheckFileTypeStrategy : IValidationStrategy
    {
        // ---------------------------------------------------------------------------------------------------------
        // Method implements the IValidationStrategy interface
        public bool Validate(Contract contract)
        {
            if (contract.File != null)
            {
                var nameOfFile = contract.File.fileName.ToLower();
                return nameOfFile.EndsWith(".pdf");
            }
            return false;
        }
    }

    public class CheckCurrencyConversion : IValidationStrategy
    {
        // ---------------------------------------------------------------------------------------------------------
        // Method implements the IValidationStrategy interface
        public bool Validate(Contract contract)
        {
            if (contract == null)
            {
                return false;
            }

            if (contract is InternationalContract)
            {
                if (string.IsNullOrWhiteSpace(contract.defaultCurrency) || contract.defaultCurrency == "None")
                {
                    return false;
                }
                return true;
            }
            return true;
        }
    }
}
// --------------------------------------------------------000... End Of File ...000------------------------------------------------------------------
