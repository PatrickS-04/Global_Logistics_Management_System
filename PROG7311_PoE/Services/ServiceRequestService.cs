using TechMove.Database.DTOs;

namespace PROG7311_PoE.Services
{
    public class ServiceRequestService
    {
        private readonly HttpClient _httpClient;

        public ServiceRequestService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // ---------------------------------------------------------------------------------------------------------
        // Method sends HTTP Post request to backend endpoint to create a service request for a contract
        public async Task<bool> CreateServiceRequestAsync(int contractId, string description, decimal cost, string status, string currency)
        {
            var dataFromForm = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("contractId", contractId.ToString()),
        new KeyValuePair<string, string>("description", description),
        new KeyValuePair<string, string>("cost", cost.ToString("F2")),
        new KeyValuePair<string, string>("status", status),
        new KeyValuePair<string, string>("currency", currency)
            });

            var response = await _httpClient.PatchAsync("api/servicerequest/createServiceRequest", dataFromForm);
            return response.IsSuccessStatusCode;
        }
    }
}
// --------------------------------------------------------000... End Of File ...000------------------------------------------------------------------
