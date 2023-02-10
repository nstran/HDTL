using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Manufacture
{
    public class UpdateStatusItemStopParameter : BaseParameter
    {
        public Guid ProductionOrderMappingId { get; set; }
    }
}
