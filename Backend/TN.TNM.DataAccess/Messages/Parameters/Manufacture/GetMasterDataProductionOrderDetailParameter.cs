using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Manufacture
{
    public class GetMasterDataProductionOrderDetailParameter : BaseParameter
    {
        public Guid ProductionOrderId { get; set; }
    }
}
