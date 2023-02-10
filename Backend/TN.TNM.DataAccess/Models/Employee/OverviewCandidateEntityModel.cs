using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Employee
{
    public class OverviewCandidateEntityModel
    {
        public Guid OverviewCandidateId { get; set; }
        public Guid CandidateId { get; set; }
        public string EducationAndWorkExpName { get; set; }
        public string CertificatePlace { get; set; }
        public string Certificate { get; set; }
        public string SpecializedTraining { get; set; }
        public string JobDescription { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Phone { get; set; }
        public string Type { get; set; }
        public string ProficiencyLevel { get; set; }
        public Guid? CreatedById { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? CertificateId { get; set; }
    }
}
