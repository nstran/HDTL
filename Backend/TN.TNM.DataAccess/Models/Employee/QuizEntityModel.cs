using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Employee
{
    public class QuizEntityModel
    {
        public Guid QuizId { get; set; }
        
        public Guid VacanciesId { get; set; }
        
        public Guid CandidateId { get; set; }
        
        public string QuizName { get; set; }
        
        public decimal? Score { get; set; }
        
        public int? Status { get; set; }

        public string StatusName { get; set; }
        
        public Guid CreatedById { get; set; }
        
        public DateTime CreatedDate { get; set; }
        
        public Guid? UpdatedById { get; set; }
        
        public DateTime? UpdatedDate { get; set; }
    }
}
