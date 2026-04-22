using PROG7311_PoE.Models;

namespace PROG7311_PoE.Services
{
    public class ContractFactory : IContractFactory
    {
        // ---------------------------------------------------------------------------------------------------------
        // Method implements the IContractFactory interface
        public Contract CreateContract(string type)
        {
            switch (type.ToLower())
            {
                case "standard":
                    return new StandardContract { serviceLevel = "standard" };
                case "premium":
                    return new PremiumContract { serviceLevel = "premium" };
                case "international":
                    return new InternationalContract { serviceLevel = "international" };
                default:
                    throw new ArgumentException("contract type is invalid");
            }
        }
    }
}
// --------------------------------------------------------000... End Of File ...000------------------------------------------------------------------
