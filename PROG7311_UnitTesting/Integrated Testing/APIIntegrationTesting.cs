using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using TechMove.Backend_API.Controllers;
using TechMove.Backend_API.Data;
using TechMove.Backend_API.Services;
using TechMove.Database.DTOs;
using TechMove.Database.Models;

namespace PROG7311_Testing.Integrated_Testing
{
    public class APIIntegrationTesting
    {
        private DbContextOptions<AppDbContext> CreateNewContextOption()
        {
            return new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public async Task Successful_GetActiveContractsAPIEndpointTest()
        {
            var mockOfContractService = new Mock<ContractService>(null, null, null);
            var mockOfContractFileService = new Mock<ContractFileService>(null, null, null);
            var mockOfClientService = new Mock<ClientService>(null);

            var testClient = new Client
            {
                clientName = "Jack Phillips",
                clientEmail = "jackPhillips@ymail.com"
            };

            var activeContracts = new List<Contract>
            {
                new StandardContract
                {
                    ContractId = 101,
                    Client = testClient,
                    startDate = DateTime.Today,
                    endDate = DateTime.Today.AddMonths(6),
                    contractStatus = ContractStatus.Active,
                    serviceLevel = "standard",
                    defaultCurrency = "ZAR"
                }
            };

            mockOfContractService
                .Setup(contractService => contractService.GetActiveContracts())
                .ReturnsAsync(activeContracts);

            var controller = new ContractController(
                (ContractService)mockOfContractService.Object,
                mockOfClientService.Object,
                mockOfContractFileService.Object
            );

            var result = await controller.GetActiveContracts();

            var httpResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedDTOs = Assert.IsAssignableFrom<List<ContractDto>>(httpResult.Value);

            Assert.Single(returnedDTOs);
            Assert.Equal(101, returnedDTOs[0].ContractId);
            Assert.Equal("Jack Phillips", returnedDTOs[0].ClientName);
            Assert.Equal("Active", returnedDTOs[0].ContractStatus);
        }

        [Fact]
        public async Task Failed_GetActiveContractsAPIEndpointTest()
        {
            var mockOfContractService = new Mock<ContractService>(null, null, null);
            var mockOfContractFileService = new Mock<ContractFileService>(null, null, null);
            var mockOfClientService = new Mock<ClientService>(null);

            string expectedExceptionMessage = "Database connection has ended or has failed to respond";

            mockOfContractService
                .Setup(contractService => contractService.GetActiveContracts())
                .ThrowsAsync(new Exception(expectedExceptionMessage));

            var controller = new ContractController(
                (ContractService)mockOfContractService.Object,
                mockOfClientService.Object,
                mockOfContractFileService.Object
            );

            var result = await controller.GetActiveContracts();

            var httpResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            string valueOfMessage = httpResult.Value?.ToString() ?? string.Empty;

            Assert.False(string.IsNullOrEmpty(valueOfMessage), "Controller did not return a text payload");
            Assert.Contains(expectedExceptionMessage.ToLower(), valueOfMessage.ToLower());
        }

        [Fact]
        public async Task Successful_CreateServiceRequestAPIEndpointTest()
        {
            var mockOfServiceRequestService = new Mock<ServiceRequestService>(null, null, null);

            mockOfServiceRequestService
                .Setup(serviceRequestService => serviceRequestService.ServiceRequestCreation(It.IsAny<ServiceRequest>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            var controller = new ServiceRequestController(mockOfServiceRequestService.Object);

            int inputContractId = 5;
            string inputDescription = "Delivery across provinces";
            decimal inputCost = 4500.00m;
            string inputStatus = "Pending Approval";
            string inputCurrency = "USD";

            var result = await controller.CreateServiceRequest(
                inputContractId,
                inputDescription,
                inputCost,
                inputStatus,
                inputCurrency
            );

            var httpResult = Assert.IsType<OkObjectResult>(result);
            var returnedDTOs = Assert.IsAssignableFrom<ServiceRequestDto>(httpResult.Value);

            Assert.Equal(inputContractId, returnedDTOs.ContractId);
            Assert.Equal(inputDescription, returnedDTOs.Description);
            Assert.Equal(inputCost, returnedDTOs.Cost);
            Assert.Equal(inputCurrency, returnedDTOs.Currency);
        }

        [Fact]
        public async Task Failed_CreateServiceRequestAPIEndpointTest()
        {
            var mockOfServiceRequestService = new Mock<ServiceRequestService>(null, null, null);

            string expectedExceptionMessage = "The API did not allow creation of a service request";

            mockOfServiceRequestService
                .Setup(serviceRequestService => serviceRequestService.ServiceRequestCreation(It.IsAny<ServiceRequest>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception(expectedExceptionMessage));

            var controller = new ServiceRequestController(mockOfServiceRequestService.Object);

            int inputContractId = 5;
            string inputDescription = "Delivery across provinces";
            decimal inputCost = 4500.00m;
            string inputStatus = "Pending Approval";
            string inputCurrency = "USD";

            var result = await controller.CreateServiceRequest(
                inputContractId,
                inputDescription,
                inputCost,
                inputStatus,
                inputCurrency
            );

            var httpResult = Assert.IsType<BadRequestObjectResult>(result);
            string valueOfMessage = httpResult.Value?.ToString() ?? string.Empty;

            Assert.False(string.IsNullOrEmpty(valueOfMessage), "Controller did not return a text payload");
            Assert.Contains(expectedExceptionMessage.ToLower(), valueOfMessage.ToLower());
        }

        [Fact]
        public async Task Successful_UpdateContractStatusAPIEndpointTest()
        {
            var mockOfContractService = new Mock<ContractService>(null, null, null);
            var mockOfContractFileService = new Mock<ContractFileService>(null, null, null);
            var mockOfClientService = new Mock<ClientService>(null);

            int testContractId = 57;
            ContractStatus newContractStatus = ContractStatus.Active;

            mockOfContractService
                .Setup(contractService => contractService.UpdateContractStatus(newContractStatus, testContractId))
                .Returns(Task.CompletedTask);

            var controller = new ContractController(
                (ContractService)mockOfContractService.Object,
                mockOfClientService.Object,
                mockOfContractFileService.Object
            );

            var result = await controller.UpdateContractStatus(testContractId, "Active");
            var httpResult = Assert.IsType<OkObjectResult>(result);

            var message = httpResult.Value.GetType().GetProperties();
            string valueOfMessage = string.Empty;

            foreach (var property in message)
            {
                if (property.PropertyType == typeof(string))
                {
                    valueOfMessage = property.GetValue(httpResult.Value, null)?.ToString() ?? string.Empty;
                    break;
                }
            }

            Assert.False(string.IsNullOrEmpty(valueOfMessage), "Controller did not return a text payload");

            Assert.Contains("updated successfully", valueOfMessage.ToLower());
        }

        [Fact]
        public async Task Failed_UpdateContractStatusAPIEndpointTest()
        {
            var mockOfContractService = new Mock<ContractService>(null, null, null);
            var mockOfContractFileService = new Mock<ContractFileService>(null, null, null);
            var mockOfClientService = new Mock<ClientService>(null);

            int testContractId = -176;
            ContractStatus newContractStatus = ContractStatus.None;

            mockOfContractService
                .Setup(contractService => contractService.UpdateContractStatus(newContractStatus, testContractId))
                .ThrowsAsync(new InvalidOperationException("Invalid contract status or contract not found"));

            var controller = new ContractController(
                (ContractService)mockOfContractService.Object,
                mockOfClientService.Object,
                mockOfContractFileService.Object
            );

            var result = await controller.UpdateContractStatus(testContractId, "None");
            var httpResult = Assert.IsType<BadRequestObjectResult>(result);

            var message = httpResult.Value.GetType().GetProperties();
            string valueOfMessage = string.Empty;

            foreach (var property in message)
            {
                if (property.PropertyType == typeof(string))
                {
                    valueOfMessage = property.GetValue(httpResult.Value, null)?.ToString() ?? string.Empty;
                    break;
                }
            }

            if (string.IsNullOrEmpty(valueOfMessage) && httpResult.Value != null)
            {
                valueOfMessage = httpResult.Value.ToString();
            }

            Assert.False(string.IsNullOrEmpty(valueOfMessage), "Controller did not return a text payload");

            Assert.Contains("invalid contract status", valueOfMessage.ToLower());
        }
    }
}
