namespace TechMove.Backend_API.Services
{
    public interface IContractService
    {
        void AttachObserver(IObserver observer);
        void DetachObserver(IObserver observer);
        void NotifyObservers(string message);
    }
}
