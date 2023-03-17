using System.Collections.Generic;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Address;
using TN.TNM.DataAccess.Models.BankAccount;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.DataAccess.Messages.Results.Asset
{
    public class TakeDataSwipeResult : BaseResult
    {
        public string MId { get; set; }

        public string CardId { get; set; }

        public string Time { get; set; }
    }
}
