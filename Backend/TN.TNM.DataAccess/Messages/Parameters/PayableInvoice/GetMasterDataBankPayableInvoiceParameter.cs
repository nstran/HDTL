using System;

namespace TN.TNM.DataAccess.Messages.Parameters.PayableInvoice
{
    public class GetMasterDataBankPayableInvoiceParameter : BaseParameter
    {
        public Guid? VendorOrderId { get; set; }
    }
}
