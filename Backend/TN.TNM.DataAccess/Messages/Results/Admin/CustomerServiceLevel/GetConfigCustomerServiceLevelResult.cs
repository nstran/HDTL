using System.Collections.Generic;
using TN.TNM.DataAccess.Models.CustomerServiceLevel;

namespace TN.TNM.DataAccess.Messages.Results.Admin.CustomerServiceLevel
{
    public class GetConfigCustomerServiceLevelResult : BaseResult
    {
        public List<CustomerServiceLevelEntityModel> CustomerServiceLevel { get; set; }
    }
}
