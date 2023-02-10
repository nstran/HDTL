using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Manufacture
{
    public class GetMasterDataCreateProductionOrderParameter : BaseParameter
    {
        public Guid OrderId { get; set; }
    }
}
