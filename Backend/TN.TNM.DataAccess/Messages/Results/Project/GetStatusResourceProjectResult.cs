using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Contract;
using TN.TNM.DataAccess.Models.Customer;
using TN.TNM.DataAccess.Models.Employee;
using TN.TNM.DataAccess.Models.Project;
using TN.TNM.DataAccess.Models.Vendor;

namespace TN.TNM.DataAccess.Messages.Results.Project
{
    public class GetStatusResourceProjectResult : BaseResult
    {             
        public List<CategoryEntityModel> ListResourceType { get; set; }
        public List<CategoryEntityModel> ListResourceRole { get; set; }
        public List<CategoryEntityModel> ListVendorGroup { get; set; }
        public List<EmployeeEntityModel> ListEmployee { get; set; }
        public List<CategoryEntityModel> ListMachine { get; set; }
        public List<CategoryEntityModel> ListOther { get; set; }
        public List<VendorEntityModel> ListVendor { get; set; }
        public List<CategoryEntityModel> ListResourceSource { get; set; }
        public ProjectResourceEntityModel ProjectResource { get; set; }
    }
}
