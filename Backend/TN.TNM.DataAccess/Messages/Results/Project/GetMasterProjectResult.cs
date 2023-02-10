using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Contract;
using TN.TNM.DataAccess.Models.Customer;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.DataAccess.Messages.Results.Project
{
    public class GetMasterProjectResult : BaseResult
    {
        public List<EmployeeEntityModel> ListEmployee { get; set; }
        public List<CustomerEntityModel> ListCustomer { get; set; }
        public List<CategoryEntityModel> ListProjectType { get; set; }
        public List<CategoryEntityModel> ListProjectScope { get; set; }
        public List<CategoryEntityModel> ListProjectStatus { get; set; }
        public List<ContractEntityModel> ListContract { get; set; }
        public List<CategoryEntityModel> ListTargetType { get; set; }
        public List<CategoryEntityModel> ListTargetUnit { get; set; }
    }
}
