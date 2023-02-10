using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Manufacture;
using TN.TNM.DataAccess.Models.Manufacture;

namespace TN.TNM.BusinessLogic.Messages.Requests.Manufacture
{
    public class PlusListItemRequest : BaseRequest<PlusListItemParameter>
    {
        public List<ProductionOrderHistoryEntityModel> ListItem { get; set; }
        public override PlusListItemParameter ToParameter()
        {
            return new PlusListItemParameter()
            {
                UserId = UserId,
                ListItem = ListItem
            };
        }
    }
}
