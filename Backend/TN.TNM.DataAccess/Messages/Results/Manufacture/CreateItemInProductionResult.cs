using System;

namespace TN.TNM.DataAccess.Messages.Results.Manufacture
{
    public class CreateItemInProductionResult : BaseResult
    {
        public Guid ProductionOrderMappingId { get; set; }
    }
}
