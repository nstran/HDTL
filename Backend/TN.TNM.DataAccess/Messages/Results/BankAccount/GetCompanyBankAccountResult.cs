using System.Collections.Generic;
using TN.TNM.DataAccess.Models.BankAccount;

namespace TN.TNM.DataAccess.Messages.Results.BankAccount
{
    public class GetCompanyBankAccountResult : BaseResult
    {
        public List<BankAccountEntityModel> BankList { get; set; }
    }
}
