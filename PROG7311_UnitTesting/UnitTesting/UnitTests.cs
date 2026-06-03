using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using TechMove.Backend_API.Data;
using TechMove.Database.Models;
using TechMove.Backend_API.Services;
using System.Diagnostics.Contracts;
using Contract = TechMove.Database.Models.Contract;

namespace PROG7311_Testing.UnitTesting
{
    public class UnitTests
    {
        [Fact]
        public async Task CurrencyConversionUnitTest()
        {
            var mockOfDbFactory = new Mock<IDbContextFactory<AppDbContext>>();
            var mockOfConfiguration = new Mock<IConfiguration>();

            mockOfConfiguration.Setup(contract => contract["ConnectionStrings:API-Key"]).Returns("https://v6.exchangerate-api.com/v6/unused");

            var requestService = new ServiceRequestService(
                mockOfDbFactory.Object,
                mockOfConfiguration.Object,
                null
            );

            decimal AmountInputted = 46m;
            string currencyConversionFrom = "GBP";

            decimal expectedConversion = 1035m;

            decimal result = await requestService.CurrencyConversion(AmountInputted, currencyConversionFrom);

            Assert.Equal(expectedConversion, result);
        }

        [Fact]
        public async Task ContractFileUploadTest()
        {
            var dbOptions = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var contextOfDb = new AppDbContext(dbOptions);

            var logisticsManager = new LogisticsManager
            {
                LogisticsManagerId = 1,
                name = "John Doe",
                phoneNumber = "1234567890",
                email = "jDoe@test.com"
            };

            var client = new Client
            {
                ClientId = 1,
                UserId = "test-user-id",
                clientName = "Harry Doyle",
                clientEmail = "hDoyle@test.com",
                clientPhoneNumber = "0437583386"
            };

            var contractForTesting = new Contract { ContractId = 1, ClientId = 1, Client = client, startDate = DateTime.Now, 
                endDate = DateTime.Now.AddMonths(2), contractStatus = ContractStatus.Active, serviceLevel = "premium", 
                LogisticsManager = logisticsManager, defaultCurrency = "ZAR" };
            contextOfDb.Contracts.Add(contractForTesting);
            await contextOfDb.SaveChangesAsync();

            var mockOfDbFactory = new Mock<IDbContextFactory<AppDbContext>>();
            mockOfDbFactory.Setup(factory => factory.CreateDbContextAsync(default)).ReturnsAsync(() => new AppDbContext(dbOptions));

            var mockOfWebEnvironment = new Mock<IWebHostEnvironment>();
            mockOfWebEnvironment.Setup(mock => mock.WebRootPath).Returns(Directory.GetCurrentDirectory());

            var contractFileService = new ContractFileService(
                mockOfDbFactory.Object,
                mockOfWebEnvironment.Object,
                null
            );

            var mockOfFile = new Mock<IBrowserFile>();
            mockOfFile.Setup(file => file.Name).Returns("test_file.png");
            mockOfFile.Setup(file => file.Size).Returns(1024);

            var error = await Assert.ThrowsAsync<InvalidOperationException>(() => contractFileService.ContractFileUpload(1, mockOfFile.Object));

            Assert.Equal("Invalid file type. Only .PDF files are allowed", error.Message);
        }

        [Fact]
        public async Task ServiceRequestCreationTest()
        {
            var dbOptions = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var contextOfDb = new AppDbContext(dbOptions);

            var logisticsManager = new LogisticsManager
            {
                LogisticsManagerId = 1,
                name = "John Doe",
                phoneNumber = "1234567890",
                email = "jDoe@test.com"
            };

            var client = new Client
            {
                ClientId = 1,
                UserId = "test-user-id",
                clientName = "Harry Doyle",
                clientEmail = "hDoyle@test.com",
                clientPhoneNumber = "0437583386"
            };

            var contractForTesting = new Contract
            {
                ContractId = 1,
                ClientId = 1,
                Client = client,
                startDate = DateTime.Now,
                endDate = DateTime.Now.AddMonths(2),
                contractStatus = ContractStatus.Active,
                serviceLevel = "standard",
                LogisticsManager = logisticsManager,
                defaultCurrency = "ZAR"
            };
            contextOfDb.Contracts.Add(contractForTesting);
            await contextOfDb.SaveChangesAsync();

            var mockOfDbFactory = new Mock<IDbContextFactory<AppDbContext>>();
            mockOfDbFactory.Setup(factory => factory.CreateDbContext()).Returns(() => new AppDbContext(dbOptions));

            var mockOfContractFactory = new Mock<IContractFactory>();
            var mockOfObserverList = new Mock<IObserver>();

            var mockOfConfiguration = new Mock<IConfiguration>();
            mockOfConfiguration.Setup(contract => contract["ConnectionStrings:API-Key"]).Returns("https://v6.exchangerate-api.com/v6/unused");

            var actualContractService = new ContractService(
                mockOfDbFactory.Object,
                mockOfContractFactory.Object,
                new List<IObserver>()
            );

            actualContractService.AttachObserver(mockOfObserverList.Object);

            var requestService = new ServiceRequestService(
                mockOfDbFactory.Object,
                mockOfConfiguration.Object,
                actualContractService
            );

            var serviceRequest = new ServiceRequest
            {
                ServiceRequestId = 2,
                ContractId = 1,
                description = "Maintenance",
                cost = 214.22m,
                status = "Expired"
            };

            await requestService.ServiceRequestCreation(serviceRequest, "GBP");

            mockOfObserverList.Verify(observer => observer.UpdateObservers(It.IsAny<string>()), Times.AtLeastOnce());

            var createdServiceRequest = await contextOfDb.ServiceRequests.FindAsync(2);
            Assert.NotNull(createdServiceRequest);
        }
    }
}
