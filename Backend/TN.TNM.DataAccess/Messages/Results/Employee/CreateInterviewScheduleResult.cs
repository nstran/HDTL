using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.DataAccess.Messages.Results.Employee
{
    public class CreateInterviewScheduleResult : BaseResult
    {
        public List<CandidateEntityModel> ListCandidate { get; set; }
        public List<InterviewScheduleEntityModel> ListInterviewSchedule { get; set; }
    }
}
