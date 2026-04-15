using PROG7311_PoE.Models;

namespace PROG7311_PoE.Services
{
    public interface IContractFactory
    {
        Contract CreateContract(string type);
    }
}
