using System.Collections.Generic;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Employee;
using TN.TNM.DataAccess.Models.Project;


namespace TN.TNM.DataAccess.Messages.Results.Project
{
    public class SearchProjectResult : BaseResult
    {
        public List<ProjectEntityModel> ListProject { get; set; }
        public List<CategoryEntityModel> ListStatus { get; set; }
        public List<CategoryEntityModel> ListProjectType { get; set; }
        public List<EmployeeEntityModel> ListEmployee { get; set; }
    }
}
