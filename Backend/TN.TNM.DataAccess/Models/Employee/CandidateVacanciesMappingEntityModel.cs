using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Employee
{
    public class CandidateVacanciesMappingEntityModel
    {
        public Guid CandidateVacanciesMappingId { get; set; }

        public Guid CandidateAssessmentId { get; set; }

        public Guid ReviewsSectionId { get; set; }

        public int? Rating { get; set; }
        public int SortOrder { get; set; }
        public string Review { get; set; }

        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
