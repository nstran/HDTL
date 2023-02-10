using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Customer;

namespace TN.TNM.DataAccess.Messages.Results.Customer
{
    public class GetDataCustomerMeetingByIdResult : BaseResult
    {
        public CustomerMeetingEntityModel CustomerMeeting { get; set; }

        public List<ContactEntityModel> CustomerContact { get; set; }
    }
}
