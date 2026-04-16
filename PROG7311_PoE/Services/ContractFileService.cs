using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using PROG7311_PoE.Data;
using PROG7311_PoE.Models;

namespace PROG7311_PoE.Services
{
    public class ContractFileService
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ContractFileService(IDbContextFactory<AppDbContext> contextFactory,  IWebHostEnvironment webHostEnvironment)
        {
            // Injecting the DbContext Factory Interface to create a new database connection for each method so the same connection is not used across all requests
            _contextFactory = contextFactory;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<ContractFile> ContractFileUpload(int contractId, IBrowserFile contractFile)
        {
            using var dbContext = await _contextFactory.CreateDbContextAsync();

            var contract = await dbContext.Contracts.FindAsync(contractId);
            if (contract == null)
            {
                throw new Exception("Contract not found");
            }

            var pathToUploadedContract = Path.Combine(_webHostEnvironment.WebRootPath, "uploadedContracts", "Contracts");
            if (!Directory.Exists(pathToUploadedContract))
            {
                Directory.CreateDirectory(pathToUploadedContract);
            }

            var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(contractFile.Name)}";
            var pathOfFile = Path.Combine(pathToUploadedContract, uniqueFileName);

            var newContractFile = new ContractFile
            {
                ContractId = contractId,
                fileName = contractFile.Name,
                storedFileName = uniqueFileName,
                fileSize = contractFile.Size,
                filePath = pathOfFile
            };

            contract.File = newContractFile;

            var contractFileTypeValidation = new CheckFileTypeStrategy();
            if (!contractFileTypeValidation.Validate(contract)) 
            {
                throw new InvalidOperationException("Invalid file type. Only PDF files are allowed");
            }

            using (var stream = new FileStream(pathOfFile, FileMode.Create))
            {
                await contractFile.OpenReadStream(maxAllowedSize: 1024 * 1024 * 25).CopyToAsync(stream);
            }

            dbContext.ContractFiles.Add(newContractFile);
            await dbContext.SaveChangesAsync();

            return newContractFile;
        }
    }
}
