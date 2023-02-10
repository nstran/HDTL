using TN.TNM.DataAccess.Models.BankAccount;

namespace TN.TNM.DataAccess.Messages.Results.BankAccount
{
    public class GetBankAccountByIdResult : BaseResult
    {
        public BankAccountEntityModel BankAccount { get; set; }
    }
}
