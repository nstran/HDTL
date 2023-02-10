using System.Collections.Generic;
using TN.TNM.DataAccess.Models.CustomerServiceLevel;

namespace TN.TNM.DataAccess.Messages.Parameters.Admin.CustomerServiceLevel
{
    public class AddLevelCustomerParameter : BaseParameter
    {
        public List<CustomerServiceLevelEntityModel> CustomerServiceLevel { get; set; }
    }
}
