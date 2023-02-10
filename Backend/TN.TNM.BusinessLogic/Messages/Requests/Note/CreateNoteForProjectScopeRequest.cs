using System;
using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Note;
using TN.TNM.DataAccess.Messages.Parameters.Note;
using TN.TNM.DataAccess.Models.Note;
using NoteModel = TN.TNM.BusinessLogic.Models.Note.NoteModel;

namespace TN.TNM.BusinessLogic.Messages.Requests.Note
{
    public class CreateNoteForProjectScopeRequest : BaseRequest<CreateNoteForProjectScopeParameter>
    {
        public NoteModel Note { get; set; }

        public override CreateNoteForProjectScopeParameter ToParameter()
        {
            return new CreateNoteForProjectScopeParameter()
            {
                Note = Note.ToEntity(),
                UserId = UserId
            };
        }
    }
}
