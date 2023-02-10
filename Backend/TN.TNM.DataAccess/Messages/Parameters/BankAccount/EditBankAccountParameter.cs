using TN.TNM.DataAccess.Models.BankAccount;

namespace TN.TNM.DataAccess.Messages.Parameters.BankAccount
{
    public class EditBankAccountParameter : BaseParameter
    {
        public BankAccountEntityModel BankAccount { get; set; }
    }
}
