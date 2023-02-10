using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Employee;
using TN.TNM.DataAccess.Models.Vendor;

namespace TN.TNM.DataAccess.Messages.Results.Order
{
    public class GetVendorAndEmployeeOfPacketResult: BaseResult
    {
        public List<EmployeeEntityModel> ListEmp { get; set; }
        public List<VendorEntityModel> ListVendor { get; set; }
    }
}
