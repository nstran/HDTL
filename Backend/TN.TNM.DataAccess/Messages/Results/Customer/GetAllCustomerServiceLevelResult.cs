using System.Collections.Generic;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.CustomerServiceLevel;

namespace TN.TNM.DataAccess.Messages.Results.Customer
{
    public class GetAllCustomerServiceLevelResult : BaseResult
    {
        public List<CustomerServiceLevelEntityModel> CustomerServiceLevelList { get; set; }
    }
}
