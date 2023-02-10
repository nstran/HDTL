using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Manufacture
{
    public class UpdateStatusItemWorkingParameter : BaseParameter
    {
        public Guid ProductionOrderMappingId { get; set; }
    }
}
