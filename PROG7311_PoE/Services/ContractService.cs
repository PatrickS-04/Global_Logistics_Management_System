using Microsoft.AspNetCore.Components.Forms;
using TechMove.Database.DTOs;

namespace PROG7311_PoE.Services
{
    public class ContractService
    {
        private readonly HttpClient _httpClient;

        public ContractService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> CreateContractAsync(string type, DateTime startDate, DateTime endDate, IBrowserFile? selectedFile)
        {
            using var content = new MultipartFormDataContent();

            content.Add(new StringContent(type ?? ""), "Type");
            content.Add(new StringContent(startDate.ToString("yyyy-MM-dd")), "StartDate");
            content.Add(new StringContent(endDate.ToString("yyyy-MM-dd")), "EndDate");

            if (selectedFile != null)
            {
                var fileStream = selectedFile.OpenReadStream(maxAllowedSize: 1024 * 1024 * 5);
                var streamContent = new StreamContent(fileStream);

                streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(selectedFile.ContentType);

                content.Add(streamContent, "ContractFile", selectedFile.Name);
            }

            var response = await _httpClient.PostAsync("api/Contract/createContract", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<List<ContractDto>> GetActiveContractsAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<ContractDto>>("api/Contract/activeContracts") ?? new List<ContractDto>();
        }

        public async Task<List<ContractDto>> GetFilteredContractsAsync(DateTime? startDate, DateTime? endDate, string? contractStatus)
        {
            var queryParams = new List<string>();

            if (startDate.HasValue)
            {
                queryParams.Add($"startDate={startDate.Value:yyyy-MM-dd}");
            }

            if (endDate.HasValue)
            {
                queryParams.Add($"endDate={endDate.Value:yyyy-MM-dd}");
            }

            if (!string.IsNullOrEmpty(contractStatus))
            {
                queryParams.Add($"contractStatus={contractStatus}");
            }

            var query = "api/Contract/filterContracts";

            if (queryParams.Any())
            {
                query += "?" + string.Join("&", queryParams);
            }

            return await _httpClient.GetFromJsonAsync<List<ContractDto>>(query) 
                ?? new List<ContractDto>();
        }

        public async Task<List<ContractDto>> ViewMyContractsAsync(string userId)
        {
            return await _httpClient.GetFromJsonAsync<List<ContractDto>>($"api/Contract/allClientContracts/{userId}") ?? new List<ContractDto>();
        }

        public async Task<List<ContractDto>> ShowAllContractsAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<ContractDto>>("api/Contract/allContracts") ?? new List<ContractDto>();
        }

        public async Task<List<ContractFileDto>> ViewContractFilesAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<ContractFileDto>>("api/Contract/allContractFiles") ?? new List<ContractFileDto>();
        }

        public async Task<HttpResponseMessage> FileDownloadAsync(string storedFileName)
        {
            return await _httpClient.GetAsync($"api/Contract/fileDownload/{storedFileName}");
        }

        public async Task<bool> UpdateContractStatusAsync(int contractId, string contractStatus)
        {
            var dataFromForm = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("contractId", contractId.ToString()),
                new KeyValuePair<string, string>("contractStatus", contractStatus)
            });

            var response = await _httpClient.PatchAsync("api/Contract/updateContractStatus", dataFromForm);
            return response.IsSuccessStatusCode;
        }
    }
}
