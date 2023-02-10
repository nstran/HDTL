using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Note;

namespace TN.TNM.BusinessLogic.Messages.Responses.Note
{
    public class CreateNoteForProjectScopeResponse : BaseResponse
    {
        public List<NoteModel> listNote { get; set; }
    }
}
