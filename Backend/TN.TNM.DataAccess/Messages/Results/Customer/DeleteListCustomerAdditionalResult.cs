using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Customer;

namespace TN.TNM.DataAccess.Messages.Results.Customer
{
    public class DeleteListCustomerAdditionalResult : BaseResult
    {
        public List<CustomerAdditionalInformationEntityModel> ListCustomerAdditionalInformation { get; set; }

    }
}
