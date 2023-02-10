using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Employee
{
    public class InterviewScheduleEntityModel
    {
        public Guid InterviewScheduleId { get; set; }
        
        public Guid VacanciesId { get; set; }
        
        public Guid CandidateId { get; set; }
        
        public string InterviewTitle { get; set; }
        
        public DateTime InterviewDate { get; set; }
        
        public string Address { get; set; }
        
        public int? InterviewTime { get; set; }
        
        public int? SortOrder { get; set; }
        
        public int Status { get; set; }
        
        public string StatusName { get; set; }
  
        public List<InterviewScheduleMappingEntityModel> ListInterviewScheduleMapping { get; set; }
        public Guid StatusId { get; set; }
        public int InterviewScheduleType { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public List<Guid> ListEmployeeId { get; set; }
    }
}
