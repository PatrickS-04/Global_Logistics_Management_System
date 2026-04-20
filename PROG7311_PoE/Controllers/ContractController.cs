using Microsoft.AspNetCore.Mvc;
using PROG7311_PoE.Models;
using PROG7311_PoE.Services;
using System.Security.Claims;

namespace PROG7311_PoE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContractController : ControllerBase
    {
        private readonly ContractService _contractService;
        private readonly ClientService _clientService;

        public ContractController(ContractService contractService, ClientService clientService)
        {
            _contractService = contractService;
            _clientService = clientService;
        }

        [HttpGet("activeContracts")]
        public async Task<ActionResult<List<Contract>>> GetActiveContracts()
        {
            var activeContracts = await _contractService.GetActiveContracts();
            return Ok(activeContracts);
        }

        [HttpPost("createContract")]
        public async Task<ActionResult<Contract>> CreateContract([FromBody] ContractCreate contractCreate)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User is not logged in.");
                }

                var client = await _clientService.GetClientByClientId(userId);

                if (client == null)
                {
                    return BadRequest("No client profile found for this user.");
                }

                var createContract = await _contractService.CreateNewContract(contractCreate.Type, client.ClientId,
                    contractCreate.StartDate, contractCreate.EndDate);
                return Ok(createContract);
            }
            catch(Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }

        [HttpPut("update-contract-status/{id}")]
        public async Task<IActionResult> UpdateContractStatus(int id,  [FromBody] ContractStatus contractStatus)
        {
            try
            {
                await _contractService.UpdateContractStatus(contractStatus, id);
                return NoContent();
            }
            catch(Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }
    }

    // DTO for creating a new contract method call
    public class ContractCreate()
    {
        public string Type { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
