using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PROG7311_PoE.Models;
using PROG7311_PoE.Services;
using System.Security.Claims;
using static System.Net.WebRequestMethods;

namespace PROG7311_PoE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContractController : ControllerBase
    {
        private readonly ContractService _contractService;
        private readonly ClientService _clientService;
        private readonly ContractFileService _contractFileService;

        public ContractController(ContractService contractService, ClientService clientService, ContractFileService contractFileService)
        {
            _contractService = contractService;
            _clientService = clientService;
            _contractFileService = contractFileService;
        }

        // ---------------------------------------------------------------------------------------------------------
        // Method gets a list of all active contracts in the UI by calling the method from the service
        [HttpGet("activeContracts")]
        [IgnoreAntiforgeryToken]
        public async Task<ActionResult<List<Contract>>> GetActiveContracts()
        {
            var activeContracts = await _contractService.GetActiveContracts();
            return Ok(activeContracts);
        }

        // ---------------------------------------------------------------------------------------------------------
        // Method creates a new user in the UI by calling the method from the service
        [HttpPost("createContract")]
        [IgnoreAntiforgeryToken]
        [Authorize(Roles = "Client")]
        public async Task<ActionResult<Contract>> CreateContract([FromForm] string type, [FromForm] DateTime startDate, [FromForm] DateTime endDate, 
            [FromForm] IFormFile? contractFile)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userId))
                {
                    return Redirect("/Login");
                }

                var client = await _clientService.GetClientByUserId(userId);

                var newContract = await _contractService.CreateNewContract(type, client.ClientId, startDate, endDate);

                if (contractFile != null && contractFile.Length > 0)
                {
                    await _contractFileService.UploadContractFileHelperMethod(newContract.ContractId, contractFile);
                }

                return Redirect("/Home");
            }
            catch(Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }

        // ---------------------------------------------------------------------------------------------------------
        // Method gets a list of all contracts belonging to a client by calling the relevant methods from the service
        [HttpGet("allClientContracts/{userId}")]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> GetAllClientContracts(string userId)
        {
            var client = await _clientService.GetClientByUserId(userId);
            if (client == null) return NotFound("Client profile not found.");

            var contracts = await _clientService.GetClientsContracts(client.ClientId);
            return Ok(contracts);
        }

        // ---------------------------------------------------------------------------------------------------------
        // Method updates the contracts status by calling the relevant method from the service
        [HttpPost("updateContractStatus")]
        [IgnoreAntiforgeryToken]
        [Authorize(Roles = "LogisticsManager")]
        public async Task<IActionResult> UpdateContractStatus([FromForm] int contractId, [FromForm] string contractStatus)
        {
            try
            {
                if (!Enum.TryParse<ContractStatus>(contractStatus, out var newContractStatus))
                {
                    return Redirect("/update-contract-status");
                }

                await _contractService.UpdateContractStatus(newContractStatus, contractId);
                return Redirect("/Home");
            }
            catch(Exception exception)
            {
                return Redirect("/update-contract-status");
            }
        }

        // ---------------------------------------------------------------------------------------------------------
        // Method gets a list of all contracts by calling the method from the service
        [HttpGet("allContracts")]
        public async Task<IActionResult> GetAllContracts()
        {
            var contracts = await _contractService.GetAllContracts();
            return Ok(contracts);
        }

        // ---------------------------------------------------------------------------------------------------------
        // Method filters contracts in the UI by calling the method from the service
        [HttpGet("filterContracts")]
        public async Task<ActionResult<List<Contract>>> filterContracts([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate, [FromQuery] ContractStatus? contractStatus)
        {
            var filteredContracts = await _contractService.GetFilteredContracts(startDate, endDate, contractStatus);
            return Ok(filteredContracts);
        }

        // ---------------------------------------------------------------------------------------------------------
        // Method gets a list of all contract files in the UI by calling the method from the service
        [HttpGet("allContractFiles")]
        [AllowAnonymous]
        public async Task<ActionResult<List<ContractFile>>> GetAllContractFiles()
        {
            var listOfContractFiles = await _contractService.GetAllContractFiles();
            return Ok(listOfContractFiles);
        }

        // ---------------------------------------------------------------------------------------------------------
        // Method allows the user to download the contract file in the UI by calling the method from the service
        [HttpGet("fileDownload/{storedName}")]
        [Authorize(Roles = "LogisticsManager")]
        public async Task<IActionResult> FileDownload(string storedName)
        {
            try
            {
                var (fileBytes, fileOriginalName) = await _contractFileService.GetFileDetailsToDownload(storedName);

                return File(fileBytes, "application/pdf", fileOriginalName);
            }
            catch (FileNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred during download: {ex.Message}");
            }
        }
    }
}
// --------------------------------------------------------000... End Of File ...000------------------------------------------------------------------
