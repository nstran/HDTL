using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Employee
{
    public class InterviewScheduleMappingEntityModel
    {
        public Guid InterviewScheduleMappingId { get; set; }
        public Guid InterviewScheduleId { get; set; }
        
        public Guid EmployeeId { get; set; }
        
        public string EmployeeName { get; set; }
        
        public string EmployeeCode { get; set; }

        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
