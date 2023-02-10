using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class CreateOrUpdateCandidateAssessmentParameter : BaseParameter
    {
        public List<CandidateVacanciesMappingEntityModel> CandidateAssessmentDetail { get; set; }
        public CandidateAssessmentEntityModel CandidateAssessment { get; set; }
    }

}
