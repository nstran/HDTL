using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Models.ReceiptInvoice;

namespace TN.TNM.DataAccess.Messages.Parameters.ReceiptInvoice
{
    public class CreateReceiptInvoiceParameter : BaseParameter
    {
        public ReceiptInvoiceEntityModel ReceiptInvoice { get; set; }
        public ReceiptInvoiceMappingEntityModel ReceiptInvoiceMapping { get; set; }
        public List<ReceiptHistoryEntityModel> ReceiptOrderHistory { get; set; }
        public Guid? OrderId { get; set; }
    }
}
