using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.BankAccount;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.DataAccess.Messages.Results.BankBook
{
    public class GetMaterDataSearchBankBookResult : BaseResult
    {
        public List<BankAccountEntityModel> ListBankAccount { get; set; }
        public List<EmployeeEntityModel> ListEmployee { get; set; }
    }
}
