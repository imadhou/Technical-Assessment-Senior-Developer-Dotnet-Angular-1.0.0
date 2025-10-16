using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N2J.Application.Models.Configs
{
    public class FileStorageOptions
    {
        public string ContainerName { get; set; } = "receipts";
        public long MaxFileSize { get; set; } = 10 * 1024 * 1024; // 10MB default
        public string[] AllowedExtensions { get; set; } = { ".jpg", ".jpeg", ".png", ".pdf" };
    }
}
