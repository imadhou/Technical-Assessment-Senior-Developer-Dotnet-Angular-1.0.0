using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Options;
using N2J.Application.Interfaces;
using N2J.Application.Models.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N2J.Application.Services
{
    public class FileStorageService : IFileStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _containerName;
        private readonly FileStorageOptions _options;

        public FileStorageService(BlobServiceClient blobServiceClient, IOptions<FileStorageOptions> options)
        {
            _blobServiceClient = blobServiceClient;
            _options = options.Value;
            _containerName = _options.ContainerName ?? "receipts";
        }

        public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            await containerClient.CreateIfNotExistsAsync(PublicAccessType.None);

            var blobClient = containerClient.GetBlobClient(fileName);

            // Set metadata
            var metadata = new Dictionary<string, string>
        {
            { "UploadedAt", DateTime.UtcNow.ToString("O") },
            { "ContentType", contentType }
        };

            await blobClient.UploadAsync(fileStream, new BlobHttpHeaders { ContentType = contentType });
            await blobClient.SetMetadataAsync(metadata);

            return blobClient.Uri.ToString();
        }

        public async Task<Stream> DownloadFileAsync(string fileName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = containerClient.GetBlobClient(fileName);

            if (!await blobClient.ExistsAsync())
                throw new FileNotFoundException("File not found");

            var response = await blobClient.DownloadAsync();
            return response.Value.Content;
        }

        public async Task<bool> DeleteFileAsync(string fileName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = containerClient.GetBlobClient(fileName);

            return await blobClient.DeleteIfExistsAsync();
        }

        public async Task<bool> FileExistsAsync(string fileName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = containerClient.GetBlobClient(fileName);

            return await blobClient.ExistsAsync();
        }
    }
}
