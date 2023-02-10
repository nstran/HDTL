using System;

namespace TN.TNM.DataAccess.Messages.Results.Manufacture
{
    public class UpdateStatusItemStopResult : BaseResult
    {
        public Guid? ProductionOrderStatusId { get; set; }
    }
}
