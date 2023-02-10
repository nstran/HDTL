using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Manufacture
{
    public class UpdateProductionOrderHistoryNoteQcAndErroTypeParameter: BaseParameter
    {
        public Guid ProductionOrderHistoryId { get; set; }
        public Guid? NoteQc { get; set; }
        public Guid? ErrorType { get; set; }
        public bool? IsUpdateNoteQc { get; set; }
        public bool? IsUpdateErrorType { get; set; }
    }
}
