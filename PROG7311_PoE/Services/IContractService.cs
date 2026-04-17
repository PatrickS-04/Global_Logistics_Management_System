namespace PROG7311_PoE.Services
{
    public interface IContractService
    {
        void AttachObserver(IObserver observer);
        void DetachObserver(IObserver observer);
        void NotifyObservers(string message);
    }
}
