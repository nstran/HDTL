using System;

namespace TN.TNM.DataAccess.Models.Employee
{
    public class EmployeeRecruitmentEntityModel
    {
        public Guid RecruitmentCampaignId { get; set; }
        public string RecruitmentCampaignName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDateDate { get; set; }
        public Guid? PersonInChargeId { get; set; }
        public string RecruitmentCampaignDes { get; set; }     
        public DateTime? CreateDate { get; set; }
        public Guid? CreateById { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateById { get; set; }
    }
}
