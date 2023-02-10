using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Employee;
using TN.TNM.DataAccess.Models.Folder;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class CreateCandidateParameter : BaseParameter
    {
        public CandidateEntityModel CandidateModel { get; set; }
        public Guid VacanciesId { get; set; }
        public string FolderType { get; set; }
        public List<FileUploadEntityModel> ListFile { get; set; }

    }
}
