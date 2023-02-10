using System;

namespace TN.TNM.BusinessLogic.Messages.Responses.Manufacture
{
    public class UpdateStatusItemWorkingResponse : BaseResponse
    {
        public Guid? ProductionOrderStatusId { get; set; }
    }
}
