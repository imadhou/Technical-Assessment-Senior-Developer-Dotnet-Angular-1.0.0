using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using N2J.Application.Interfaces;
using N2J.Application.Models.Commands;
using N2J.Application.Models.Configs;
using N2J.Application.Models.Dtos.Expense;
using N2J.Application.Models.Exceptions;
using N2J.Domain.Entities;
using N2J.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N2J.Application.Services
{
    public class ReceiptService : IReceiptService
    {
        private readonly IFileStorageService _fileStorageService;
        private readonly IExpenseRepository _expenseRepository;
        private readonly IUserRepository _userRepository;
        private readonly FileStorageOptions _options;

        public ReceiptService(
            IFileStorageService fileStorageService,
            IExpenseRepository expenseRepository,
            IUserRepository userRepository,
            IOptions<FileStorageOptions> options)
        {
            _fileStorageService = fileStorageService;
            _expenseRepository = expenseRepository;
            _userRepository = userRepository;
            _options = options.Value;
        }

        public async Task<ReceiptDto> UploadReceiptAsync(FileUploadCommand request, Guid userId)
        {
            ValidateFile(request.File);

            var expense = await _expenseRepository.GetByIdAsync(request.ExpenseId, e => e.User);
            if (expense == null)
            {
                throw new EntityNotFoundException("Expnese not found");
            }

            // Generate unique file name
            var fileExtension = Path.GetExtension(request.File.FileName).ToLowerInvariant();
            var fileName = $"/user/{userId}/expenses/{request.ExpenseId}/{Guid.NewGuid()}{fileExtension}";

            // Upload file
            using var stream = request.File.OpenReadStream();
            var fileUrl = await _fileStorageService.UploadFileAsync(stream, fileName, request.File.ContentType);

            // Update expense with receipt info
            expense.ReceiptUrl = fileName;
            expense.ReceiptFileName = request.File.FileName;
            expense.ReceiptFileSize = request.File.Length;
            expense.ReceiptUploadedAt = DateTime.UtcNow;

            await _expenseRepository.UpdateAsync(expense);

            return new ReceiptDto
            {
                FileName = request.File.FileName,
                FileUrl = fileUrl,
                FileSize = request.File.Length,
                ContentType = request.File.ContentType,
                UploadedAt = DateTime.UtcNow,
            };
        }

        public async Task<Stream> DownloadReceiptAsync(Guid expenseId, Guid userId)
        {
            var expense = await _expenseRepository.GetByIdAsync(expenseId);
            if (expense == null || expense.UserId != userId || string.IsNullOrEmpty(expense.ReceiptUrl))
            {
                throw new FileNotFoundException("Receipt not found");
            }

            // Extract file name from URL
            var stream = await _fileStorageService.DownloadFileAsync(expense.ReceiptUrl);
            return stream;
        }

        public async Task<bool> DeleteReceiptAsync(Guid expenseId, Guid userId)
        {
            var expense = await _expenseRepository.GetByIdAsync(expenseId);
            if (expense == null || expense.UserId != userId || string.IsNullOrEmpty(expense.ReceiptUrl))
            {
                return false;
            }

            var deleted = await _fileStorageService.DeleteFileAsync(expense.ReceiptUrl);

            if (deleted)
            {
                expense.ReceiptUrl = null;
                expense.ReceiptFileName = null;
                expense.ReceiptFileSize = null;
                expense.ReceiptUploadedAt = null;

                await _expenseRepository.UpdateAsync(expense);
            }

            return deleted;
        }


        private void ValidateFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty");

            if (file.Length > _options.MaxFileSize)
                throw new ArgumentException($"File size exceeds maximum allowed size of {_options.MaxFileSize} bytes");

            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!_options.AllowedExtensions.Contains(fileExtension))
                throw new ArgumentException($"File type not allowed. Allowed types: {string.Join(", ", _options.AllowedExtensions)}");
        }
    }
}
