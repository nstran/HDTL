using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Contact;

namespace TN.TNM.BusinessLogic.Messages.Requests.Contact
{
    public class GetAddressByChangeObjectRequest : BaseRequest<GetAddressByChangeObjectParameter>
    {
        public Guid ObjectId { get; set; }
        public int ObjectType { get; set; }

        public override GetAddressByChangeObjectParameter ToParameter()
        {
            return new GetAddressByChangeObjectParameter()
            {
                UserId = UserId,
                ObjectId = ObjectId,
                ObjectType = ObjectType
            };
        }
    }
}
