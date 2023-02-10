using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Manufacture;

namespace TN.TNM.BusinessLogic.Messages.Requests.Manufacture
{
    public class ChangeProductGroupCodeByItemRequest : BaseRequest<ChangeProductGroupCodeByItemParameter>
    {
        public Guid ProductionOrderMappingId { get; set; }
        public string ProductGroupCode { get; set; }

        public override ChangeProductGroupCodeByItemParameter ToParameter()
        {
            return new ChangeProductGroupCodeByItemParameter()
            {
                ProductionOrderMappingId = ProductionOrderMappingId,
                ProductGroupCode = ProductGroupCode,
                UserId = UserId
            };
        }
    }
}
