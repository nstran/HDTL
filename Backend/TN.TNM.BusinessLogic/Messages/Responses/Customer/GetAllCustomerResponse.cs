using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Customer;
using TN.TNM.DataAccess.Models.Customer;

namespace TN.TNM.BusinessLogic.Messages.Responses.Customer
{
    public class GetAllCustomerResponse : BaseResponse
    {
        public List<CustomerEntityModel> CustomerList { get; set; }
    }
}
