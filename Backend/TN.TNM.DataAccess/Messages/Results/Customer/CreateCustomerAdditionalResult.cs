using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Customer;

namespace TN.TNM.DataAccess.Messages.Results.Customer
{
    public class CreateCustomerAdditionalResult: BaseResult
    {
        public List<CustomerAdditionalInformationEntityModel> ListCustomerAdditionalInformation { get; set; }
    }
}
