using System;

namespace TN.TNM.DataAccess.Messages.Results.Manufacture
{
    public class UpdateStatusItemWorkingResult : BaseResult
    {
        public Guid? ProductionOrderStatusId { get; set; }
    }
}
