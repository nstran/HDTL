using System;
using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Note;
using TN.TNM.DataAccess.Messages.Parameters.Note;
using TN.TNM.DataAccess.Models.Note;
using NoteModel = TN.TNM.BusinessLogic.Models.Note.NoteModel;

namespace TN.TNM.BusinessLogic.Messages.Requests.Note
{
    public class CreateNoteForProjectResourceRequest : BaseRequest<CreateNoteForProjectResourceParameter>
    {
        public NoteModel Note { get; set; }
        public override CreateNoteForProjectResourceParameter ToParameter()
        {
            return new CreateNoteForProjectResourceParameter()
            {
                Note = Note.ToEntity(),
                UserId = UserId
            };
        }
    }
}
