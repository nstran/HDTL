using System;

namespace TN.TNM.BusinessLogic.Messages.Responses.Manufacture
{
    public class UpdateStatusItemStopResponse:BaseResponse
    {
        public Guid? ProductionOrderStatusId { get; set; }
    }
}
