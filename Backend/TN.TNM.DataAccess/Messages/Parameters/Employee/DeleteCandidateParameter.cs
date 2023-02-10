using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class DeleteCandidateParameter : BaseParameter
    {
        public Guid CandidateId { get; set; }
        public Guid VacanciesId { get; set; }
    }

}
