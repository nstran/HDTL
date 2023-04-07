using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Address;
using TN.TNM.DataAccess.Models.Customer;
using TN.TNM.DataAccess.Models.Employee;
using TN.TNM.DataAccess.Models.GeographicalArea;

namespace TN.TNM.DataAccess.Messages.Results.Customer
{
    public class CreateCustomerMasterDataResult : BaseResult
    {
        public List<CategoryEntityModel> ListCustomerGroup { get; set; }
        public List<CategoryEntityModel> ListEnterPriseType { get; set; }
        public List<CategoryEntityModel> ListBusinessScale { get; set; }
        public List<CategoryEntityModel> ListPosition { get; set; }
        public List<CategoryEntityModel> ListBusinessLocal { get; set; }
        public List<CategoryEntityModel> ListMainBusiness { get; set; }
        public List<ProvinceEntityModel> ListProvinceModel { get; set; }
        public List<DistrictEntityModel> ListDistrictModel { get; set; }
        public List<WardEntityModel> ListWardModel { get; set; }
        public List<EmployeeEntityModel> ListEmployeeModel { get; set; }
        public List<string> ListCustomerCode { get; set; }
        public List<string> ListCustomerTax { get; set; }
        public List<GeographicalAreaEntityModel> ListArea { get; set; }
        public List<CustomerEntityModel> ListCustomer { get; set; }
        public List<EmployeeEntityModel> ListStaffCharge { get; set; }
    }
}
