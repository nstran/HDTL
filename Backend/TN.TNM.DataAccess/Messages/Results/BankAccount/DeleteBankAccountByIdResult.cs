using System.Collections.Generic;
using TN.TNM.DataAccess.Models.BankAccount;

namespace TN.TNM.DataAccess.Messages.Results.BankAccount
{
    public class DeleteBankAccountByIdResult : BaseResult
    {
        public List<BankAccountEntityModel> ListBankAccount { get; set; }
    }
}
