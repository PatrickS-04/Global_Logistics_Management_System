using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TechMove.Backend_API.Services;
using TechMove.Database.DTOs;
using TechMove.Database.Models;

namespace TechMove.Backend_API.Controllers
{
    [ApiController]
    [Route("api/servicerequest")]
    public class ServiceRequestController : ControllerBase
    {
        private readonly ServiceRequestService _serviceRequestService;

        public ServiceRequestController(ServiceRequestService serviceRequestService)
        {
            _serviceRequestService = serviceRequestService;
        }

        // ---------------------------------------------------------------------------------------------------------
        // Method creates new service request via the UI by calling the method from the service
        [HttpPost("createServiceRequest")]
        [Authorize(Roles = "LogisticsManager")]
        public async Task<IActionResult> CreateServiceRequest([FromForm] int contractId, [FromForm] string description, [FromForm] decimal cost, 
            [FromForm] string status, [FromForm] string currency)
        {
            try
            {
                var newServiceRequest = new ServiceRequest
                {
                    ContractId = contractId,
                    description = description,
                    cost = cost,
                    status = status
                };

                await _serviceRequestService.ServiceRequestCreation(newServiceRequest, currency);

                var responseDTO = new ServiceRequestDto
                {
                    ServiceRequestId = newServiceRequest.ServiceRequestId,
                    ContractId = newServiceRequest.ContractId,
                    Description = newServiceRequest.description,
                    Cost = newServiceRequest.cost,
                    Status = newServiceRequest.status,
                    Currency = currency
                };

                return Ok(responseDTO);
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }
    }
}
// --------------------------------------------------------000... End Of File ...000------------------------------------------------------------------
