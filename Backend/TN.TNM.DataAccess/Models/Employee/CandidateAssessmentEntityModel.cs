using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Employee
{
    public class CandidateAssessmentEntityModel
    {
        public Guid CandidateAssessmentId { get; set; }
        
        public Guid CandidateId { get; set; }
        
        public Guid VacanciesId { get; set; }
        public int Status { get; set; }
        
        public string OtherReview { get; set; }
        
        public Guid EmployeeId { get; set; }
        
        public string EmployeeCode { get; set; }
        
        public string EmployeeName { get; set; }
        public Guid CreatedById { get; set; }
        
        public DateTime CreatedDate { get; set; }
        
        public Guid? UpdatedById { get; set; }
        
        public DateTime? UpdatedDate { get; set; }
        public List<CandidateAssessmentDetailEntityModel> CandidateAssessmentDetail { get; set; } = new List<CandidateAssessmentDetailEntityModel>();
    }
}
