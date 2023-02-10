using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.DataAccess.Messages.Results.Employee
{
    public class GetCandidateImportDetaiResult : BaseResult
    {
        public List<CandidateEntityModel> ListCandidate { get; set; }
        public List<EmployeeEntityModel> ListEmp { get; set; }
        public List<EmployeeEntityModel> ListEmpNghiViec { get; set; }
    }
}
