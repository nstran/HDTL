using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class InterviewSchedule
    {
        public Guid InterviewScheduleId { get; set; }
        public Guid VacanciesId { get; set; }
        public Guid CandidateId { get; set; }
        public string InterviewTitle { get; set; }
        public DateTime InterviewDate { get; set; }
        public int? InterviewTime { get; set; }
        public int? SortOrder { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }
        public string Address { get; set; }
        public string TypeOfInterview { get; set; }
        public int? Status { get; set; }
        public int InterviewScheduleType { get; set; }
    }
}
