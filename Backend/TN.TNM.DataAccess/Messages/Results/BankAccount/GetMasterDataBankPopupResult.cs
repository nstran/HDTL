using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models;

namespace TN.TNM.DataAccess.Messages.Results.BankAccount
{
    public class GetMasterDataBankPopupResult : BaseResult
    {
        public List<CategoryEntityModel> ListBank { get; set; }
    }
}
