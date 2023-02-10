using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Manufacture;

namespace TN.TNM.BusinessLogic.Messages.Requests.Manufacture
{
    public class ChangeGroupCodeForListItemRequest : BaseRequest<ChangeGroupCodeForListItemParameter>
    {
        public Guid ProductionOrderId { get; set; }
        public string Code_11 { get; set; }
        public string Code_111 { get; set; }
        public string Code_112 { get; set; }
        public string Code_12 { get; set; }
        public string Code_121 { get; set; }
        public string Code_122 { get; set; }

        public override ChangeGroupCodeForListItemParameter ToParameter()
        {
            return new ChangeGroupCodeForListItemParameter()
            {
                ProductionOrderId = ProductionOrderId,
                Code_11 = Code_11,
                Code_111 = Code_111,
                Code_112 = Code_112,
                Code_12 = Code_12,
                Code_121 = Code_121,
                Code_122 = Code_122
            };
        }
    }
}
