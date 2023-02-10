using System;

namespace TN.TNM.DataAccess.Messages.Results.Manufacture
{
    public class UpdateStatusItemCancelResult : BaseResult
    {
        public Guid? ProductionOrderStatusId { get; set; }
    }
}
