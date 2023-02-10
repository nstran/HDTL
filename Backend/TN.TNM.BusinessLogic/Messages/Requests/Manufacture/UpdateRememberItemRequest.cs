using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Manufacture;
using TN.TNM.DataAccess.Messages.Parameters.Manufacture;

namespace TN.TNM.BusinessLogic.Messages.Requests.Manufacture
{
    public class UpdateRememberItemRequest : BaseRequest<UpdateRememberItemParameter>
    {
        public RememberItemModel RememberItem { get; set; }
        public override UpdateRememberItemParameter ToParameter()
        {
            return new UpdateRememberItemParameter()
            {
                UserId = UserId,
                RememberItem = RememberItem.ToEntity()
            };
        }
    }
}
