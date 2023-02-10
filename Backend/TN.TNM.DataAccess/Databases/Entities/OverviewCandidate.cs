using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class OverviewCandidate
    {
        public Guid OverviewCandidateId { get; set; }
        public Guid CandidateId { get; set; }
        public string EducationAndWorkExpName { get; set; }
        public string CertificatePlace { get; set; }
        public string SpecializedTraining { get; set; }
        public string JobDescription { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Phone { get; set; }
        public string Type { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }
        public string ProficiencyLevel { get; set; }
        public Guid? CertificateId { get; set; }
    }
}
