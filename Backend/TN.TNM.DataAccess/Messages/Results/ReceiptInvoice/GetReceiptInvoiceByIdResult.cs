using TN.TNM.DataAccess.Models.ReceiptInvoice;

namespace TN.TNM.DataAccess.Messages.Results.ReceiptInvoice
{
    public class GetReceiptInvoiceByIdResult : BaseResult
    {
        public ReceiptInvoiceEntityModel ReceiptInvoice { get; set; }
    }
}
