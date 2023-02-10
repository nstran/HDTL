using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class RecruitmentCampaign
    {
        public Guid RecruitmentCampaignId { get; set; }
        public string RecruitmentCampaignName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDateDate { get; set; }
        public Guid? PersonInChargeId { get; set; }
        public string RecruitmentCampaignDes { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }
    }
}
