using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Note;

namespace TN.TNM.BusinessLogic.Messages.Responses.Note
{
    public class CreateNoteMilestoneResponse : BaseResponse
    {
        public List<NoteModel> listNote { get; set; }
    }
}
