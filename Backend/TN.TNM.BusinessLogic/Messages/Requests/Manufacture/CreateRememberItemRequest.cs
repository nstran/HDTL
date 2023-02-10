using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Manufacture;
using TN.TNM.DataAccess.Messages.Parameters.Manufacture;
using TN.TNM.DataAccess.Models.Manufacture;

namespace TN.TNM.BusinessLogic.Messages.Requests.Manufacture
{
    public class CreateRememberItemRequest : BaseRequest<CreateRememberItemParameter>
    {
        public RememberItemModel RememberItem { get; set; }

        public override CreateRememberItemParameter ToParameter()
        {
            return new CreateRememberItemParameter()
            {
                UserId = UserId,
                RememberItem = RememberItem.ToEntity()
            };
        }
    }
}
