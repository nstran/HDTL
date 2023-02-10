using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.DataAccess.Messages.Results.Admin.Product
{
    public class GetListEmployeeByRoleIdResult : BaseResult
    {
        public List<EmployeeEntityModel> ListEmployeeEntityModel { get; set; }     
    }
}
