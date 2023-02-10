using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.BusinessLogic.Messages.Responses.Manufacture
{
    public class CreateItemInProductionResponse:BaseResponse
    {
        public Guid ProductionOrderMappingId { get; set; }
    }
}
