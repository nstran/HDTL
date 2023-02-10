using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Manufacture
{
    public class DeleteItemInProductionParameter : BaseParameter
    {
        public Guid ProductionOrderMappingId { get; set; }
    }
}
