using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class DeleteInterviewScheduleParameter : BaseParameter
    {
        public Guid InterviewScheduleId { get; set; }
    }

}
