using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.DataAccess.Messages.Results.CashBook
{
    public class GetDataSearchCashBookResult : BaseResult
    {
        public List<EmployeeEntityModel> ListEmployee { get; set; }
        public List<OrganizationEntityModel> ListOrganization { get; set; }
    }
}
