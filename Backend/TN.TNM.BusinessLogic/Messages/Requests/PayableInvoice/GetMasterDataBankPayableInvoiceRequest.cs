using System;
using TN.TNM.DataAccess.Messages.Parameters.PayableInvoice;

namespace TN.TNM.BusinessLogic.Messages.Requests.PayableInvoice
{
    public class GetMasterDataBankPayableInvoiceRequest : BaseRequest<GetMasterDataBankPayableInvoiceParameter>
    {
        public Guid? VendorOrderId { get; set; }

        public override GetMasterDataBankPayableInvoiceParameter ToParameter() => new GetMasterDataBankPayableInvoiceParameter
        {
            VendorOrderId = VendorOrderId,
            UserId = UserId
        };
    }
}
