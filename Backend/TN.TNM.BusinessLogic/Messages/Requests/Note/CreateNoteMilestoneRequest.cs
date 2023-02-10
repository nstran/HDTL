using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Note;
using TN.TNM.DataAccess.Messages.Parameters.Note;

namespace TN.TNM.BusinessLogic.Messages.Requests.Note
{
    public class CreateNoteMilestoneRequest : BaseRequest<CreateNoteMilestoneParameter>
    {
        public NoteModel Note { get; set; }
        public bool IsSendEmail { get; set; }
        public override CreateNoteMilestoneParameter ToParameter()
        {
            return new CreateNoteMilestoneParameter
            {
                Note = this.Note.ToEntity(),
                UserId = this.UserId,
                IsSendEmail = this.IsSendEmail
            };
        }
    }
}
