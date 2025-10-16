using N2J.Application.Models.Commands;
using N2J.Application.Models.Dtos.Expense;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N2J.Application.Interfaces
{
    public interface IExpenseReceiptService
    {
        Task<ReceiptDto> UploadReceiptAsync(FileUploadCommand command, Guid userId);
        Task<Stream> DownloadReceiptAsync(Guid expenseId, Guid userId);
        Task<bool> DeleteReceiptAsync(Guid expenseId, Guid userId);
    }
}
