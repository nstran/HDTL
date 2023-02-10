using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Manufacture;

namespace TN.TNM.BusinessLogic.Messages.Requests.Manufacture
{
    public class UpdateProductionOrderHistoryNoteQcAndErroTypeRequest : BaseRequest<UpdateProductionOrderHistoryNoteQcAndErroTypeParameter>
    {
        public Guid ProductionOrderHistoryId { get; set; }
        public Guid? NoteQc { get; set; }
        public Guid? ErrorType { get; set; }
        public bool? IsUpdateNoteQc { get; set; }
        public bool? IsUpdateErrorType { get; set; }

        public override UpdateProductionOrderHistoryNoteQcAndErroTypeParameter ToParameter()
        {
            return new UpdateProductionOrderHistoryNoteQcAndErroTypeParameter()
            {
                ProductionOrderHistoryId = ProductionOrderHistoryId,
                NoteQc = NoteQc,
                ErrorType = ErrorType,
                IsUpdateNoteQc = IsUpdateNoteQc,
                IsUpdateErrorType = IsUpdateErrorType
            };
        }
    }
}
