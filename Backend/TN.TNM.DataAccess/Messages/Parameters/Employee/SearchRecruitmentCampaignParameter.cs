using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class SearchRecruitmentCampaignParameter : BaseParameter
    {
        public string RecruitmentCampaignName { get; set; }
        public DateTime? StartDateFrom { get; set; }
        public DateTime? StartDateTo { get; set; }
        public DateTime? EndDateFrom { get; set; }
        public DateTime? EndDateTo { get; set; }
        public List<Guid?> ListPersonInChangeId { get; set; }
        public int? RecruitmentQuantityFrom { get; set; }
        public int? RecruitmentQuantityTo { get; set; }

    }
}
