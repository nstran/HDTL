using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Models.Employee
{
    public class CandidateEntityModel
    {
        public Guid CandidateId { get; set; }
        public string FullName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Avatar { get; set; }
        public string Email { get; set; }
        public Guid? RecruitmentChannel { get; set; }
        public string RecruitmentChannelName { get; set; }
        public Guid? RecruitmentChannelId { get; set; }
        public int? Sex { get; set; }
        public int? StatusId { get; set; }
        public string StatusName { get; set; }

        public Guid? VacanciesId { get; set; }

        public string VacanciesName { get; set; }

        public int? Status { get; set; }

        public Guid? RecruitmentCampaignId { get; set; }

        public string RecruitmentCampaignName { get; set; }
        public string StatusCode { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime? ApplicationDate { get; set; }
        public string RecruitmentChannelCode { get; set; }
        public string PersonInChargeName { get; set; }
        public string PersonInChargePhone { get; set; }
        public string PlaceOfWork { get; set; }
        public string TomTatHocVan { get; set; }
        public Guid? PhuongThucTuyenDungId { get; set; }
        public decimal? MucPhi { get; set; }

    }
}
