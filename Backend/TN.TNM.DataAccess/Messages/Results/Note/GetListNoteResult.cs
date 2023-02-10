using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Note;

namespace TN.TNM.DataAccess.Messages.Results.Note
{
    public class GetListNoteResult : BaseResult
    {
        public int? TotalRecordsNote { get; set; }
        public List<NoteEntityModel> ListNote { get; set; }
    }
}
