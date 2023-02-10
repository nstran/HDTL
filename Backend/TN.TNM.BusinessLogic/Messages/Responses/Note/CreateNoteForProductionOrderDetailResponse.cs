using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Note;

namespace TN.TNM.BusinessLogic.Messages.Responses.Note
{
    public class CreateNoteForProductionOrderDetailResponse : BaseResponse
    {
        public List<NoteModel> ListNote { get; set; }
    }
}
