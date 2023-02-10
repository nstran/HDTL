using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class ImportListCandidateParameter : BaseParameter
    {
        public List<CandidateEntityModel> ListCandidate { get; set; }
        public Guid VacanciesId { get; set; }
    }
}
