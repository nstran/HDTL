using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Contract;
using TN.TNM.DataAccess.Models.Customer;
using TN.TNM.DataAccess.Models.Employee;
using TN.TNM.DataAccess.Models.Note;
using TN.TNM.DataAccess.Models.Project;

namespace TN.TNM.DataAccess.Messages.Results.Project
{
    public class GetMasterUpdateProjectResult : BaseResult
    {
        public List<EmployeeEntityModel> ListEmployee { get; set; }
        public List<CustomerEntityModel> ListCustomer { get; set; }
        public List<CategoryEntityModel> ListProjectType { get; set; }
        public List<CategoryEntityModel> ListProjectScope { get; set; }
        public List<CategoryEntityModel> ListProjectStatus { get; set; }
        public List<ContractEntityModel> ListContract { get; set; }
        public List<CategoryEntityModel> ListTargetType { get; set; }
        public List<CategoryEntityModel> ListTargetUnit { get; set; }
        public List<NoteEntityModel> ListNote { get; set; }

        public List<ProjectTargetEntityModel> ListProjectTarget { get; set; }
        public ProjectEntityModel Project { get; set; }
        public string Role { get; set; }
        public bool HasTaskInProgress { get; set; }
        public int? TotalRecordsNote { get; set; }

        public List<ProjectEntityModel> ListProject { get; set; }
        public bool IsShowGiaBanTheoGio { get; set; }
    }
}
