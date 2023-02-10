using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Manufacture
{
    public class UpdateStatusItemCancelParameter:BaseParameter
    {
        public Guid ProductionOrderMappingId { get; set; }
    }
}
