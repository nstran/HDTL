using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Note;

namespace TN.TNM.BusinessLogic.Messages.Requests.Note
{
    public class DeleteNoteDocumentRequest : BaseRequest<DeleteNoteDocumentParameter>
    {
        public Guid NoteId { get; set; }

        public Guid NoteDocumentId { get; set; }


        public override DeleteNoteDocumentParameter ToParameter()
        {
            return new DeleteNoteDocumentParameter()
            {
                NoteDocumentId = NoteDocumentId,
                NoteId = NoteId,
                UserId = UserId
            };
        }
    }
}
