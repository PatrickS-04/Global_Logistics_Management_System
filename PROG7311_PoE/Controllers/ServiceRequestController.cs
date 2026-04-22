using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PROG7311_PoE.Models;
using PROG7311_PoE.Services;
using System.Security.Claims;

namespace PROG7311_PoE.Controllers
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
        [IgnoreAntiforgeryToken]
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
                return Redirect("/Home");
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }
    }
}
// --------------------------------------------------------000... End Of File ...000------------------------------------------------------------------
