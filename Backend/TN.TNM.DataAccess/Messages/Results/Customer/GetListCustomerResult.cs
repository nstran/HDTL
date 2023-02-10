using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Address;
using TN.TNM.DataAccess.Models.CustomerServiceLevel;

namespace TN.TNM.DataAccess.Messages.Results.Customer
{
    public class GetListCustomerResult: BaseResult
    {
        
        public List<ProvinceEntityModel> ListProvince { get; set; }
        public List<AreaEntityModel> ListAreaModel { get; set; }
        public List<CategoryEntityModel> ListSourceModel { get; set; }
        public List<CategoryEntityModel> ListStatusCareModel { get; set; }
        public List<CategoryEntityModel> ListCategoryModel { get; set; }
        public List<CustomerServiceLevelEntityModel> ListCustomerServiceLevelModel { get; set; }
    }
}
