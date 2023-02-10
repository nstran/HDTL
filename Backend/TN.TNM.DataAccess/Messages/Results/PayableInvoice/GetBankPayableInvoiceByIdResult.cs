using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.PayableInvoice;

namespace TN.TNM.DataAccess.Messages.Results.PayableInvoice
{
    public class GetBankPayableInvoiceByIdResult : BaseResult
    {
        public BankPayableInvoiceEntityModel BankPayableInvoice { get; set; }
    }
}
