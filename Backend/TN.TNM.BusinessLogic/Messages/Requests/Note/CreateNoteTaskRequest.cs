using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Note;
using TN.TNM.DataAccess.Messages.Parameters.Note;

namespace TN.TNM.BusinessLogic.Messages.Requests.Note
{
    public class CreateNoteTaskRequest : BaseRequest<CreateNoteTaskParameter>
    {
        public NoteModel Note { get; set; }
        public override CreateNoteTaskParameter ToParameter()
        {
            return new CreateNoteTaskParameter
            {
                Note = this.Note.ToEntity(),
                UserId = this.UserId
            };
        }
    }
}
