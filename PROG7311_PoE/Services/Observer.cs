using Microsoft.Identity.Client;

namespace PROG7311_PoE.Services
{
    public class LogisticsManagerEmailObserver : IObserver
    {
        public string email { get; set; }

        public void UpdateObservers(string message)
        {
            Console.WriteLine($"An [EMAIL] has been sent to LOGISTICS MANAGER {email}: {message}");
        }
    }

    public class ClientEmailObserver : IObserver
    {
        public string email { get; set; }

        public void UpdateObservers(string message)
        {
            Console.WriteLine($"An [EMAIL] has been sent to CLIENT {email}: {message}");
        }
    }

    public class AdminEmailObserver : IObserver
    {
        public string email { get; set; }

        public void UpdateObservers(string message)
        {
            Console.WriteLine($"An [EMAIL] has been sent to ADMIN {email}: {message}");
        }
    }

    public class LogisticsManagerAlertObserver : IObserver
    {
        public string phoneNumber { get; set; }

        public void UpdateObservers(string message)
        {
            Console.WriteLine($"An [SMS] has been sent to LOGISTICS MANAGER {phoneNumber}: {message}");
        }
    }

    public class ClientAlertObserver : IObserver
    {
        public string phoneNumber { get; set; }

        public void UpdateObservers(string message)
        {
            Console.WriteLine($"An [SMS] has been sent to CLIENT {phoneNumber}: {message}");
        }
    }

    public class AdminAlertObserver : IObserver
    {
        public string phoneNumber { get; set; }

        public void UpdateObservers(string message)
        {
            Console.WriteLine($"An [SMS] has been sent to ADMIN {phoneNumber}: {message}");
        }
    }
}
