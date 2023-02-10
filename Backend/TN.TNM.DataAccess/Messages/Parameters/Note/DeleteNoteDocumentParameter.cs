using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Note
{
    public class DeleteNoteDocumentParameter : BaseParameter
    {
        public Guid NoteId { get; set; }

        public Guid NoteDocumentId { get; set; }
    }
}
