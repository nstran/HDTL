using System.Collections.Generic;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Note;
using TN.TNM.DataAccess.Models.Project;
using TN.TNM.DataAccess.Models.Task;
using TN.TNM.DataAccess.Models.Vendor;

namespace TN.TNM.DataAccess.Messages.Results.Project
{
    public class GetProjectScopeResult : BaseResult
    {   
        public ProjectEntityModel Project { get; set; }
        public List<ProjectScopeModel> ListProjectScope { get; set; }
        public List<TaskEntityModel> ListProjectTask { get; set; }
        public List<VendorEntityModel> ListVendor { get; set; }
        public List<CategoryEntityModel> ListResource { get; set; }
        public List<CategoryEntityModel> ListStatus { get; set; }
        public List<NoteEntityModel> ListNote { get; set; }
        public int TotalRecordsNote { get; set; }

        public List<ProjectEntityModel> listProject { get; set; }
    }
}
