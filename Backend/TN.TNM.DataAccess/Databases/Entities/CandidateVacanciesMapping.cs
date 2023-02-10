using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class CandidateVacanciesMapping
    {
        public Guid CandidateVacanciesMappingId { get; set; }
        public Guid CandidateId { get; set; }
        public Guid VacanciesId { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }
    }
}
