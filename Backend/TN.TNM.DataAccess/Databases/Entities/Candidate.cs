using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class Candidate
    {
        public Guid CandidateId { get; set; }
        public string FullName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Avatar { get; set; }
        public string Email { get; set; }
        public int? Sex { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }
        public DateTime? ApplicationDate { get; set; }
        public Guid? RecruitmentChannelId { get; set; }
        public int? Status { get; set; }
        public string TomTatHocVan { get; set; }
        public Guid? PhuongThucTuyenDungId { get; set; }
        public decimal? MucPhi { get; set; }
    }
}
