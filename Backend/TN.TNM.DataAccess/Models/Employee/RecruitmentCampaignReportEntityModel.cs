using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Models.Employee
{
    public class RecruitmentCampaignReportEntityModel
    {
        public Guid RecruitmentCampaignId { get; set; }
        public string RecruitmentCampaignName { get; set; }
        public string RecruitmentCampaignYearName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDateDate { get; set; }
       public List<VacancyEntityModel> ListVacancies { get; set; }
    }
}
