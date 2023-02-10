using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.ReceiptInvoice
{
    public class ConfirmPaymentParameter : BaseParameter
    {
        public Guid ReceiptInvoiceId { get; set; }

        public string Type { get; set; }
    }
}
