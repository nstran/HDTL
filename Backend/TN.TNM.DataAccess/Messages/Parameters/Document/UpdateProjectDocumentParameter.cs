using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Task;
using TN.TNM.DataAccess.Models.Note;

namespace TN.TNM.DataAccess.Messages.Parameters.Document
{
    public class UpdateProjectDocumentParameter : BaseParameter
    {
        public  Guid ProjectId { get; set; }

        public List<TaskDocumentEntityModel> ListTaskDocument { get; set; }

        public List<NoteDocumentEntityModel> ListDocument { get; set; }

    }
}
