using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Manufacture;

namespace TN.TNM.BusinessLogic.Messages.Requests.Manufacture
{
    public class UpdateProductionOrderNoteRequest : BaseRequest<UpdateProductionOrderNoteParameter>
    {
        public Guid ProductionOrderId { get; set; }
        public string Note { get; set; }
        public override UpdateProductionOrderNoteParameter ToParameter()
        {
            return new UpdateProductionOrderNoteParameter()
            {
                UserId = UserId,
                ProductionOrderId = ProductionOrderId,
                Note = Note
            };
        }
    }
}
