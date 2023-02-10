using System;
using TN.TNM.DataAccess.Messages.Parameters.ReceiptInvoice;

namespace TN.TNM.BusinessLogic.Messages.Requests.ReceiptInvoice
{
    public class GetOrderByCustomerIdRequest : BaseRequest<GetOrderByCustomerIdParameter>
    {
        public Guid CustomerId { get; set; }

        public Guid? OrderId { get; set; }

        public override GetOrderByCustomerIdParameter ToParameter() => new GetOrderByCustomerIdParameter()
        {
            CustomerId = CustomerId,
            OrderId = OrderId,
            UserId = UserId
        };
    }
}
