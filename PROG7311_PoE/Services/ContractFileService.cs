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
        private readonly ContractService _contractService;

        public ContractFileService(IDbContextFactory<AppDbContext> contextFactory,  IWebHostEnvironment webHostEnvironment, ContractService contractService)
        {
            // Injecting the DbContext Factory Interface to create a new database connection for each method so the same connection is not used across all requests
            _contextFactory = contextFactory;
            _webHostEnvironment = webHostEnvironment;
            _contractService = contractService;
        }

        // ---------------------------------------------------------------------------------------------------------
        // Method allow user to upload a contract file in .pdf format
        public async Task<ContractFile> ContractFileUpload(int contractId, IBrowserFile contractFile)
        {
            using var dbContext = await _contextFactory.CreateDbContextAsync();

            var contract = await dbContext.Contracts
                .Include(contract => contract.LogisticsManager)
                .FirstOrDefaultAsync(contract => contract.ContractId == contractId);

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
                throw new InvalidOperationException("Invalid file type. Only .PDF files are allowed");
            }

            using (var stream = new FileStream(pathOfFile, FileMode.Create))
            {
                await contractFile.OpenReadStream(maxAllowedSize: 1024 * 1024 * 25).CopyToAsync(stream);
            }

            dbContext.ContractFiles.Add(newContractFile);

            var logisticsEmailObserver = new LogisticsManagerEmailObserver { email = contract.LogisticsManager.email };
            var logisticsAlertObserver = new LogisticsManagerAlertObserver { phoneNumber = contract.LogisticsManager.phoneNumber };

            _contractService.AttachObserver(logisticsEmailObserver);
            _contractService.AttachObserver(logisticsAlertObserver);

            _contractService.NotifyObservers($"A file has been uploaded for contract with {contractId}");

            _contractService.DetachObserver(logisticsEmailObserver);
            _contractService.DetachObserver(logisticsAlertObserver);

            await dbContext.SaveChangesAsync();

            return newContractFile;
        }

        // ---------------------------------------------------------------------------------------------------------
        // Method is a helper for the contract file upload method above
        public async Task<ContractFile> UploadContractFileHelperMethod(int contractId, IFormFile file)
        {
            using var dbContext = await _contextFactory.CreateDbContextAsync();

            var pathToUploadedContract = Path.Combine(_webHostEnvironment.WebRootPath, "uploadedContracts", "Contracts");
            if (!Directory.Exists(pathToUploadedContract))
            {
                Directory.CreateDirectory(pathToUploadedContract);
            }

            var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var pathOfFile = Path.Combine(pathToUploadedContract, uniqueFileName);

            var newContractFile = new ContractFile
            {
                ContractId = contractId,
                fileName = file.FileName,
                storedFileName = uniqueFileName,
                fileSize = file.Length,
                filePath = pathOfFile
            };

            using (var stream = new FileStream(pathOfFile, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            dbContext.ContractFiles.Add(newContractFile);
            await dbContext.SaveChangesAsync();

            return newContractFile;
        }

        // ---------------------------------------------------------------------------------------------------------
        // Method allows for the download of the contract file that was uploaded
        public async Task<(byte[] FileBytes, string FileName)> GetFileDetailsToDownload(string storedName)
        {
            using var dbContext = await _contextFactory.CreateDbContextAsync();

            var file = await dbContext.ContractFiles
                .FirstOrDefaultAsync(file => file.storedFileName == storedName);

            if (file == null)
            {
                throw new FileNotFoundException("File record not found in database.");
            }

            var path = Path.Combine(_webHostEnvironment.WebRootPath, "uploadedContracts", "Contracts", storedName);

            if (!System.IO.File.Exists(path))
            {
                throw new FileNotFoundException("Physical file not found on server.");
            }

            var fileBytes = await System.IO.File.ReadAllBytesAsync(path);

            return (fileBytes, file.fileName);
        }
    }
}
// --------------------------------------------------------000... End Of File ...000------------------------------------------------------------------
