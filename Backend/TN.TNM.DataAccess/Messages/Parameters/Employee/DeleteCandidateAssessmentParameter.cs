using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class DeleteCandidateAssessmentParameter : BaseParameter
    {
        public Guid CandidateAssessmentId { get; set; }
    }

}
