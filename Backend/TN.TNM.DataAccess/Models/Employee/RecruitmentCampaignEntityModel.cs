using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Employee
{
    public class RecruitmentCampaignEntityModel
    {
        public Guid RecruitmentCampaignId { get; set; }
        
        public string RecruitmentCampaignName { get; set; }
        public string RecruitmentCampaignYearName { get; set; }
        public DateTime StartDate { get; set; }
        
        public DateTime EndDateDate { get; set; }
        
        public string StatusName { get; set; }

        public Guid? PersonInChargeId { get; set; }
        
        public string PersonInChargeName { get; set; }
        
        public string PersonInChargeCode { get; set; }
        
        public string PersonInChargeCodeName { get; set; }
        
        public string RecruitmentCampaignDes { get; set; }

        public int RecruitmentQuantity { get; set; }

        public int SortOrder { get; set; }


        public Guid? CreatedById { get; set; }
        
        public DateTime? CreatedDate { get; set; }
        
        public Guid? UpdatedById { get; set; }
        
        public DateTime? UpdatedDate { get; set; }
        public int QuantityVacancies { get; set; }
    }
}
