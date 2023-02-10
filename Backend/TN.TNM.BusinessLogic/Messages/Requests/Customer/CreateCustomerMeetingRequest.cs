using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Customer;
using TN.TNM.DataAccess.Messages.Parameters.Customer;
using TN.TNM.DataAccess.Models.Customer;

namespace TN.TNM.BusinessLogic.Messages.Requests.Customer
{
    public class CreateCustomerMeetingRequest : BaseRequest<CreateCustomerMeetingParameter>
    {
        public DataAccess.Models.Customer.CustomerMeetingModel CustomerMeeting { get; set; }
        public override CreateCustomerMeetingParameter ToParameter()
        {
            var _temp = new CustomerMeetingEntityModel();
            _temp.CustomerMeetingId = CustomerMeeting.CustomerMeetingId;
            _temp.CustomerId = CustomerMeeting.CustomerId;
            _temp.EmployeeId = CustomerMeeting.EmployeeId;
            _temp.Title = CustomerMeeting.Title;
            _temp.LocationMeeting = CustomerMeeting.LocationMeeting;
            _temp.StartDate = CustomerMeeting.StartDate;
            _temp.StartHours = CustomerMeeting.StartHours;
            _temp.Content = CustomerMeeting.Content;
            _temp.EndDate = CustomerMeeting.EndDate;
            _temp.EndHours = CustomerMeeting.EndHours;
            _temp.Participants = CustomerMeeting.Participants;
            _temp.CustomerParticipants = CustomerMeeting.CustomerParticipants;

            return new CreateCustomerMeetingParameter()
            {
                CustomerMeeting = _temp,
                UserId = UserId
            };
        }
    }
}
