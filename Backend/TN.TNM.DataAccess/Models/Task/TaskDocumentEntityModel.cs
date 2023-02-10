using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Task
{
    public class TaskDocumentEntityModel
    {
        public Guid TaskDocumentId { get; set; }
        public Guid TaskId { get; set; }
        public string DocumentName { get; set; }
        public string documentNameWithoutExtension { get; set; }
        public string DocumentSize { get; set; }
        public string DocumentUrl { get; set; }
        public bool? Active { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid CreatedById { get; set; }
        public Guid FolderId { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? UpdatedById { get; set; }

        public string CreateByName { get; set; }

        public string TaskCodeName { get; set; }


        public Guid ObjectId { get; set; }
        public string ObjectType { get; set; }
    }
}
