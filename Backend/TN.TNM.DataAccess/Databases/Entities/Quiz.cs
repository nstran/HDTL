using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class Quiz
    {
        public Guid QuizId { get; set; }
        public Guid VacanciesId { get; set; }
        public Guid CandidateId { get; set; }
        public string QuizName { get; set; }
        public decimal? Score { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }
        public int? Status { get; set; }
    }
}
