using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Note;

namespace TN.TNM.DataAccess.Messages.Results.Note
{
    public class SearchNoteResult : BaseResult
    {
        public List<NoteEntityModel> NoteEntityList { get; set; }
        public int? TotalRecordsNote { get; set; }
        public List<NoteEntityModel> NoteList { get; set; }
    }
}
