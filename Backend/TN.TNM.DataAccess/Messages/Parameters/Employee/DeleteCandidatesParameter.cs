using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class DeleteCandidatesParameter : BaseParameter
    {
        public List<Guid> ListCandidateId { get; set; }
        public Guid VacanciesId { get; set; }
    }
}
