using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Folder;
using TN.TNM.DataAccess.Models.Note;
using TN.TNM.DataAccess.Models.Project;
using TN.TNM.DataAccess.Models.Task;

namespace TN.TNM.DataAccess.Messages.Results.Project
{
    public class GetMasterProjectDocumentResult : BaseResult
    {
        public List<TaskDocumentEntityModel> ListTaskDocument { get; set; }

        public List<NoteDocumentEntityModel> ListDocument { get; set; }

        public List<FolderEntityModel> ListFolders { get; set; }

        public ProjectEntityModel Project { get; set; }

        public decimal ProjectTaskComplete { get; set; }

        public decimal TotalEstimateHour { get; set; }
        
        public decimal TotalSize { get; set; }

        public List<ProjectEntityModel> ListProject { get; set; }
    }
}
