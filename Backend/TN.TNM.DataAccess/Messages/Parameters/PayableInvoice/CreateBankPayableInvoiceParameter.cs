using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.PayableInvoice;

namespace TN.TNM.DataAccess.Messages.Parameters.PayableInvoice
{
    public class CreateBankPayableInvoiceParameter : BaseParameter
    {
        public BankPayableInvoiceEntityModel BankPayableInvoice { get; set; }
        public BankPayableInvoiceMappingEntityModel BankPayableInvoiceMapping { get; set; }
    }
}
