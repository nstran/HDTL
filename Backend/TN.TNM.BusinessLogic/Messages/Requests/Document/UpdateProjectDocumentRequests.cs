using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Document;
using TN.TNM.DataAccess.Models.Note;
using TN.TNM.DataAccess.Models.Task;

namespace TN.TNM.BusinessLogic.Messages.Requests.Document
{
    public class UpdateProjectDocumentRequests: BaseRequest<UpdateProjectDocumentParameter>
    {
        public Guid ProjectId { get; set; }

        public List<TaskDocumentEntityModel> ListTaskDocument { get; set; }

        public List<NoteDocumentEntityModel> ListDocument { get; set; }


        public override UpdateProjectDocumentParameter ToParameter()
        {
            return new UpdateProjectDocumentParameter()
            {
                ProjectId = ProjectId,
                ListTaskDocument = ListTaskDocument,
                ListDocument = ListDocument,
                UserId = this.UserId
            };
        }
    }
}
