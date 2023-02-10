using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Category;
using TN.TNM.BusinessLogic.Models.Contract;
using TN.TNM.BusinessLogic.Models.Customer;
using TN.TNM.BusinessLogic.Models.Employee;
using TN.TNM.BusinessLogic.Models.Vendor;
using TN.TNM.DataAccess.Models.Project;

namespace TN.TNM.BusinessLogic.Messages.Responses.Project
{
    public class GetStatusResourceProjectResponse : BaseResponse
    {
        public List<CategoryModel> ListResourceType { get; set; }
        public List<CategoryModel> ListResourceRole { get; set; }
        public List<CategoryModel> ListVendorGroup { get; set; }
        public List<EmployeeModel> ListEmployee { get; set; }
        public List<VendorModel> ListVendor { get; set; }
        public List<CategoryModel> ListResourceSource { get; set; }
        public List<CategoryModel> ListMachine { get; set; }
        public List<CategoryModel> ListOther { get; set; }
        public ProjectResourceEntityModel ProjectResource { get; set; }
    }
}
