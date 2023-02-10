using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Note;
using TN.TNM.DataAccess.Models.Note;

namespace TN.TNM.BusinessLogic.Messages.Responses.Note
{
    public class CreateNoteForAllRecruitmentCampaignResponse : BaseResponse
    {
        public List<NoteEntityModel> ListNote { get; set; }
    }
}
