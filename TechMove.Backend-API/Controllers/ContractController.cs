using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Contracts;
using System.Security.Claims;
using TechMove.Backend_API.Services;
using TechMove.Database.DTOs;
using TechMove.Database.Models;
using static System.Net.WebRequestMethods;

namespace TechMove.Backend_API.Controllers
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
        public async Task<ActionResult<List<ContractDto>>> GetActiveContracts()
        {
            var activeContracts = await _contractService.GetActiveContracts();

            var contractDTOs = activeContracts.Select(contract => new ContractDto
            {
                ContractId = contract.ContractId,
                ClientName = contract.Client?.clientName ?? "Unknown Client",
                Type = contract.GetType().Name,
                StartDate = contract.startDate,
                EndDate = contract.endDate,
                ContractStatus = contract.contractStatus.ToString(),
                DefaultCurrency = contract.defaultCurrency ?? "ZAR"
            }).ToList();

            return Ok(contractDTOs);
        }

        // ---------------------------------------------------------------------------------------------------------
        // Method creates a new user in the UI by calling the method from the service
        [HttpPost("createContract")]
        [Authorize(Roles = "Client")]
        public async Task<ActionResult> CreateContract([FromForm] ContractFormModel contractModel)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User is not authenticated");
                }

                var client = await _clientService.GetClientByUserId(userId);

                if (client == null)
                {
                    return BadRequest("Client profile could not be found");
                }

                var newContract = await _contractService.CreateNewContract(contractModel.Type, client.ClientId, contractModel.StartDate, contractModel.EndDate);

                if (contractModel.ContractFile != null && contractModel.ContractFile.Length > 0)
                {
                    await _contractFileService.UploadContractFileHelperMethod(newContract.ContractId, contractModel.ContractFile);
                }

                var responseDTO = new ContractDto
                {
                    ContractId = newContract.ContractId,
                    ClientName = client.clientName,
                    Type = contractModel.Type,
                    StartDate = newContract.startDate,
                    EndDate = newContract.endDate,
                    ContractStatus = newContract.contractStatus.ToString(),
                    DefaultCurrency = newContract.defaultCurrency ?? "ZAR"
                };

                return Ok(responseDTO);
            }
            catch(Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }

        // ---------------------------------------------------------------------------------------------------------
        // Method gets a list of all contracts belonging to a client by calling the relevant methods from the service
        [HttpGet("allClientContracts/{userId}")]
        public async Task<IActionResult> GetAllClientContracts(string userId)
        {
            var client = await _clientService.GetClientByUserId(userId);
            if (client == null) return NotFound("Client profile not found.");

            var contracts = await _clientService.GetClientsContracts(client.ClientId);

            var contractDTOs = contracts.Select(contract =>
            {
                string normalizedType = contract switch
                {
                    StandardContract => "Standard",
                    PremiumContract => "Premium",
                    InternationalContract => "International",
                    _ => "Standard"
                };

                return new ContractDto
                {
                    ContractId = contract.ContractId,
                    ClientName = contract.Client?.clientName ?? "Unknown Client",
                    Type = normalizedType,
                    StartDate = contract.startDate,
                    EndDate = contract.endDate,
                    ContractStatus = contract.contractStatus.ToString(),
                    DefaultCurrency = contract.defaultCurrency ?? "ZAR"
                };
            }).ToList();

            return Ok(contractDTOs);
        }

        // ---------------------------------------------------------------------------------------------------------
        // Method updates the contracts status by calling the relevant method from the service
        [HttpPatch("updateContractStatus")]
        [Authorize(Roles = "LogisticsManager")]
        public async Task<IActionResult> UpdateContractStatus([FromForm] int contractId, [FromForm] string contractStatus)
        {
            try
            {
                if (!Enum.TryParse<ContractStatus>(contractStatus, out var newContractStatus))
                {
                    return BadRequest($"Invalid status value: {contractStatus}");
                }

                await _contractService.UpdateContractStatus(newContractStatus, contractId);
                return Ok(new { message = "Contract status was updated successfully" });
            }
            catch(Exception exception)
            {
                return BadRequest(new { error = exception.Message });
            }
        }

        // ---------------------------------------------------------------------------------------------------------
        // Method gets a list of all contracts by calling the method from the service
        [HttpGet("allContracts")]
        public async Task<IActionResult> GetAllContracts()
        {
            var contracts = await _contractService.GetAllContracts();

            var contractDTOs = contracts.Select(contract =>
            {
                string normalizedType = contract switch
                {
                    StandardContract => "Standard",
                    PremiumContract => "Premium",
                    InternationalContract => "International",
                    _ => "Standard"
                };

                return new ContractDto
                {
                    ContractId = contract.ContractId,
                    ClientName = contract.Client?.clientName ?? "Unknown Client",
                    Type = normalizedType,
                    StartDate = contract.startDate,
                    EndDate = contract.endDate,
                    ContractStatus = contract.contractStatus.ToString(),
                    DefaultCurrency = contract.defaultCurrency ?? "ZAR"
                };
            }).ToList();

            return Ok(contractDTOs);
        }

        // ---------------------------------------------------------------------------------------------------------
        // Method filters contracts in the UI by calling the method from the service
        [HttpGet("filterContracts")]
        public async Task<ActionResult<List<ContractDto>>> filterContracts([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate, [FromQuery] ContractStatus? contractStatus)
        {
            var filteredContracts = await _contractService.GetFilteredContracts(startDate, endDate, contractStatus);

            var contractDTOs = filteredContracts.Select(contract => new ContractDto
            {
                ContractId = contract.ContractId,
                ClientName = contract.Client?.clientName ?? "Unknown Client",
                Type = contract.GetType().Name,
                StartDate = contract.startDate,
                EndDate = contract.endDate,
                ContractStatus = contract.contractStatus.ToString(),
                DefaultCurrency = contract.defaultCurrency ?? "ZAR"
            }).ToList();

            return Ok(contractDTOs);
        }

        // ---------------------------------------------------------------------------------------------------------
        // Method gets a list of all contract files in the UI by calling the method from the service
        [HttpGet("allContractFiles")]
        [AllowAnonymous]
        public async Task<ActionResult<List<ContractFileDto>>> GetAllContractFiles()
        {
            var listOfContractFiles = await _contractService.GetAllContractFiles();

            var contractFileDTOs = listOfContractFiles.Select(contractFile => new ContractFileDto
            {
                ContractId = contractFile.ContractId,
                ClientName = contractFile.Contract?.Client?.clientName ?? "N/A",
                FileName = contractFile.fileName,
                StoredFileName = contractFile.storedFileName,
                DateOfUpload = contractFile.dateOfUpload
            }).ToList();

            return Ok(contractFileDTOs);
        }

        // ---------------------------------------------------------------------------------------------------------
        // Method allows the user to download the contract file in the UI by calling the method from the service
        [HttpGet("fileDownload/{storedName}")]
        [Authorize(Roles = "LogisticsManager")]
        public async Task<IActionResult> FileDownload(string storedName)
        {
            try
            {
                (byte[] fileBytes, string fileOriginalName) = await _contractFileService.GetFileDetailsToDownload(storedName);

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

    public class ContractFormModel
    {
        public string Type { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public IFormFile? ContractFile { get; set; }
    }
}
// --------------------------------------------------------000... End Of File ...000------------------------------------------------------------------
