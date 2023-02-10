using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.BankAccount;

namespace TN.TNM.BusinessLogic.Messages.Requests.BankAccount
{
    public class GetMasterDataBankPopupRequest : BaseRequest<GetMasterDataBankPopupParameter>
    {
        public override GetMasterDataBankPopupParameter ToParameter()
        {
            return new GetMasterDataBankPopupParameter
            {
                UserId = this.UserId
            };
        }
    }
}
