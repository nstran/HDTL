using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class CreateInterviewScheduleParameter : BaseParameter
    {
        public List<InterviewScheduleEntityModel> ListInterviewSchedule { get; set; }
        public string ScreenType { get; set; }
    }

}
