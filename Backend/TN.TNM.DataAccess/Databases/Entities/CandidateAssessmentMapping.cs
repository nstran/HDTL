using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class CandidateAssessmentMapping
    {
        public Guid CandidateAssessmentMappingId { get; set; }
        public Guid CandidateAssessmentId { get; set; }
        public Guid ReviewsSectionId { get; set; }
        public int Rating { get; set; }
        public string Review { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }
    }
}
