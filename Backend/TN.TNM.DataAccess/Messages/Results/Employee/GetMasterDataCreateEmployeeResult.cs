using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models;

namespace TN.TNM.DataAccess.Messages.Results.Employee
{
    public class GetMasterDataCreateEmployeeResult : BaseResult
    {
        //public List<EmployeeMonthySalaryEntityModel> lstEmployeeMonthySalary { get; set; }
        public List<Organization> ListOrangization { get; set; }
        public List<Position> ListPosition { get; set; }
    }
}
