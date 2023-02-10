using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Contact;

namespace TN.TNM.BusinessLogic.Messages.Requests.Contact
{
    public class GetAddressByContactIdRequest : BaseRequest<GetAddressByContactIdParameter>
    {
        public Guid? ContactId { get; set; }
        public override GetAddressByContactIdParameter ToParameter()
        {
            return new GetAddressByContactIdParameter()
            {
                UserId = UserId,
                ContactId = ContactId
            };
        }
    }
}
