using System;

namespace TN.TNM.BusinessLogic.Messages.Responses.Manufacture
{
    public class UpdateStatusItemCancelResponse : BaseResponse
    {
        public Guid? ProductionOrderStatusId { get; set; }
    }
}
