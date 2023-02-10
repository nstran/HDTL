using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Customer;

namespace TN.TNM.DataAccess.Messages.Results.Customer
{
    public class GetAllCustomerResult : BaseResult
    {
        public List<CustomerEntityModel> CustomerList { get; set; }
    }
}
