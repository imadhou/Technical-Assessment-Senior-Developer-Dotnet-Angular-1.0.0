using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N2J.Application.Models.Commands
{
    public class FileUploadCommand
    {
        public IFormFile File { get; set; } = null!;
        public Guid ExpenseId { get; set; }
    }
}
