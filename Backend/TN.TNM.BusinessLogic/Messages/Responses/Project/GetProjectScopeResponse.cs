using System;
using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Category;
using TN.TNM.BusinessLogic.Models.Project;
using TN.TNM.BusinessLogic.Models.Vendor;
using TN.TNM.DataAccess.Models.Note;

namespace TN.TNM.BusinessLogic.Messages.Responses.Project
{
    public class GetProjectScopeResponse : BaseResponse
    {      
        public ProjectModel Project { get; set; }

        public List<ProjectScopeModel> ListProjectScope { get; set; }

        public List<TaskModel> ListProjectTask { get; set; }
        public List<VendorModel> ListVendor { get; set; }
        public List<CategoryModel> ListResource { get; set; }
        public List<CategoryModel> ListStatus{ get; set; }
        public List<string> ListResourceSope { get; set; }
        public List<NoteEntityModel> ListNote { get; set; }
        public int TotalRecordsNote { get; set; }

        public List<DataAccess.Models.Project.ProjectEntityModel> listProject { get; set; }
    }
}
