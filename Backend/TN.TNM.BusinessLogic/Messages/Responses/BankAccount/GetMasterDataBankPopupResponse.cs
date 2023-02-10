using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models;

namespace TN.TNM.BusinessLogic.Messages.Responses.BankAccount
{
    public class GetMasterDataBankPopupResponse : BaseResponse
    {
        public List<CategoryEntityModel> ListBank { get; set; }
    }
}
