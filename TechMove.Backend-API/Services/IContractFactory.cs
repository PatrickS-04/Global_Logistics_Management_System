using TechMove.Database.Models;

namespace TechMove.Backend_API.Services
{
    public interface IContractFactory
    {
        Contract CreateContract(string type);
    }
}
