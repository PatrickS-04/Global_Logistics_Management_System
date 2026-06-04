using TechMove.Database.Models;

namespace TechMove.Backend_API.Services
{
    public interface IValidationStrategy
    {
        bool Validate(Contract contract);
    }
}
