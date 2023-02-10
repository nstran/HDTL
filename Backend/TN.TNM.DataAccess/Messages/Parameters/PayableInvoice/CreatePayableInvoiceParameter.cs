using TN.TNM.DataAccess.Models.PayableInvoice;

namespace TN.TNM.DataAccess.Messages.Parameters.PayableInvoice
{
    public class CreatePayableInvoiceParameter : BaseParameter
    {
        public PayableInvoiceEntityModel PayableInvoice { get; set; }
        public PayableInvoiceMappingEntityModel PayableInvoiceMapping { get; set; }
    }
}
