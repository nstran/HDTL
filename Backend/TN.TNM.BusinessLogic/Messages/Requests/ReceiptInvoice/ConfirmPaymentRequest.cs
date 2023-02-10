using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.ReceiptInvoice;

namespace TN.TNM.BusinessLogic.Messages.Requests.ReceiptInvoice
{
    public class ConfirmPaymentRequest : BaseRequest<ConfirmPaymentParameter>
    {
        public Guid ReceiptInvoiceId { get; set; }

        public string Type { get; set; }


        public override ConfirmPaymentParameter ToParameter()
        {
            return new ConfirmPaymentParameter()
            {
                ReceiptInvoiceId = ReceiptInvoiceId,
                Type = Type,
                UserId = UserId
            };
        }
    }
}
