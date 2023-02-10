using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Task;
using TN.TNM.DataAccess.Models.Folder;
using TN.TNM.DataAccess.Models.Project;
using TN.TNM.DataAccess.Models.Task;

namespace TN.TNM.BusinessLogic.Messages.Requests.Task
{
    public class CreateOrUpdateTaskRequest : BaseRequest<CreateOrUpdateTaskParameter>
    {
        public TaskEntityModel Task { get; set; }

        public string FolderType { get; set; }
        
        public List<ProjectResourceEntityModel> ListProjectResource { get; set; }

        public List<FileUploadEntityModel> ListTaskDocument { get; set; }

        public List<TaskDocumentEntityModel> ListTaskDocumentDelete { get; set; }


        public override CreateOrUpdateTaskParameter ToParameter()
        {
            return new CreateOrUpdateTaskParameter
            {
                Task = this.Task,
                FolderType = FolderType,
                ListProjectResource = this.ListProjectResource,
                ListTaskDocument = this.ListTaskDocument,
                ListTaskDocumentDelete = this.ListTaskDocumentDelete,
                UserId = this.UserId,
            };
        }
    }
}
