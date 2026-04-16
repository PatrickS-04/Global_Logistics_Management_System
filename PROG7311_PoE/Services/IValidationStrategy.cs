using PROG7311_PoE.Models;

namespace PROG7311_PoE.Services
{
    public interface IValidationStrategy
    {
        bool Validate(Contract contract);
    }
}
