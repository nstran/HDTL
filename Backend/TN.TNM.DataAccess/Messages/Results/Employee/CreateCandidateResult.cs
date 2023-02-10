using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.DataAccess.Messages.Results.Employee
{
    public class CreateCandidateResult : BaseResult
    {
        public Guid CandidateId { get; set; }
        public List<CandidateEntityModel> ListCandidate { get; set; }
        public List<EmployeeVacanciesEntityModel> ListViTriTuyenDung { get; set; }
    }
}
