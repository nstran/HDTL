using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class UpdateCandidateStatusFromVacanciesParameter : BaseParameter
    {
        public List<Guid> ListCandidate { get; set; }
        public int Status { get; set; }
        public Guid VacanciesId { get; set; }
    }

}
