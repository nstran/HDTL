using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Employee;
using TN.TNM.DataAccess.Models.Folder;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class UpdateCandidateParameter : BaseParameter
    {
        public Guid VacanciesId { get; set; }
        public CandidateEntityModel CandidateModel { get; set; }
        public List<FileUploadEntityModel> ListFileInFolder { get; set; }
    }

}
