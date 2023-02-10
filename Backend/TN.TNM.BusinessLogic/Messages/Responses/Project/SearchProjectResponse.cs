using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Category;
using TN.TNM.BusinessLogic.Models.Employee;
using TN.TNM.BusinessLogic.Models.Project;


namespace TN.TNM.BusinessLogic.Messages.Responses.Project
{
    public class SearchProjectResponse : BaseResponse
    {
        public List<ProjectModel> ListProject { get; set; }
        public List<CategoryModel> ListStatus { get; set; }
        public List<CategoryModel> ListProjectType { get; set; }
        public List<EmployeeModel> ListEmployee { get; set; }
    }
}
