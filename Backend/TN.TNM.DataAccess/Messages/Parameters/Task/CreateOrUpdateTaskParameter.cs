using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Folder;
using TN.TNM.DataAccess.Models.Project;
using TN.TNM.DataAccess.Models.Task;

namespace TN.TNM.DataAccess.Messages.Parameters.Task
{
    public class CreateOrUpdateTaskParameter : BaseParameter
    {
        public TaskEntityModel Task { get; set; }

        public string FolderType { get; set; }

        public List<ProjectResourceEntityModel> ListProjectResource { get; set; }
        public List<FileUploadEntityModel> ListTaskDocument { get; set; }

        public List<TaskDocumentEntityModel> ListTaskDocumentDelete { get; set; }
        public List<RelateTaskMappingEntityModel> ListTaskRelate { get; set; }
    }
}
