using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Employee
{
    public class VacancyEntityModel
    {
        public Guid VacanciesId { get; set; }
        public Guid RecruitmentCampaignId { get; set; }
        public string VacanciesName { get; set; }
        public int Quantity { get; set; }
        
        public int Priority { get; set; }
        
        public string PriorityName { get; set; }
        
        public Guid? PersonInChargeId { get; set; }
        
        public string PersonInChargeCode { get; set; }
        
        public string PersonInChargeName { get; set; }
        
        
        public Guid? TypeOfWork { get; set; }
        
        public string TypeOfWorkCode { get; set; }
        
        public string TypeOfWorkName { get; set; }
        
        public string PlaceOfWork { get; set; }
        
        public Guid? ExperienceId { get; set; }
        
        public string ExperienceCode { get; set; }
        
        public string ExperienceName { get; set; }
        
        public string Currency { get; set; }
        public int? SalarType { get; set; }
        public decimal? SalaryFrom { get; set; }
        public decimal? SalaryTo { get; set; }
        public string SalaryLable { get; set; }
        public string VacanciesDes { get; set; }
        public string ProfessionalRequirements { get; set; }
        public string CandidateBenefits { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public List<CandidateEntityModel> ListCandidate { get; set; }
        public decimal HieuSuat { get; set; }
        public decimal PTDat { get; set; }
    }
}
