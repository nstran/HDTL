using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Category;
using TN.TNM.BusinessLogic.Models.Contract;
using TN.TNM.BusinessLogic.Models.Customer;
using TN.TNM.BusinessLogic.Models.Employee;

namespace TN.TNM.BusinessLogic.Messages.Responses.Project
{
    public class GetMasterProjectResponse : BaseResponse
    {
        public List<EmployeeModel> ListEmployee { get; set; }
        public List<CustomerModel> ListCustomer { get; set; }
        public List<CategoryModel> ListProjectType { get; set; }
        public List<CategoryModel> ListProjectScope { get; set; }
        public List<CategoryModel> ListProjectStatus { get; set; }
        public List<ContractModel> ListContract { get; set; }
        public List<CategoryModel> ListTargetType { get; set; }
        public List<CategoryModel> ListTargetUnit { get; set; }
    }
}
