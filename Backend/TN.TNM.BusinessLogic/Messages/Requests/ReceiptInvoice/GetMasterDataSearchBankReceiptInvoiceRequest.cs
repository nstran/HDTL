using TN.TNM.DataAccess.Messages.Parameters.ReceiptInvoice;

namespace TN.TNM.BusinessLogic.Messages.Requests.ReceiptInvoice
{
    public class GetMasterDataSearchBankReceiptInvoiceRequest : BaseRequest<GetMasterDataSearchBankReceiptInvoiceParameter>
    {
        public override GetMasterDataSearchBankReceiptInvoiceParameter ToParameter() => new GetMasterDataSearchBankReceiptInvoiceParameter
        {
            UserId = UserId
        };
    }
}
