using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Note;

namespace TN.TNM.BusinessLogic.Messages.Responses.Note
{
    public class CreateNoteForProjectResourceResponse : BaseResponse
    {
        public List<NoteModel> listNote { get; set; }
        public int TotalRecordsNote { get; set; }
    }
}
