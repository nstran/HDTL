using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Note;
using TN.TNM.DataAccess.Models.Note;
using NoteModel = TN.TNM.BusinessLogic.Models.Note.NoteModel;

namespace TN.TNM.BusinessLogic.Messages.Responses.Note
{
    public class SearchNoteResponse : BaseResponse
    {
        public List<NoteModel> NoteList { get; set; }
        public List<NoteEntityModel> NoteEntityList { get; set; }
        public int? TotalRecordsNote { get; set; }
    }
}
