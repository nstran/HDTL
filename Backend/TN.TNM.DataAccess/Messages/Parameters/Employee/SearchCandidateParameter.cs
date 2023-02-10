using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class SearchCandidateParameter : BaseParameter
    {
        public string FullName { get; set; }
        public List<Guid?> ListRecruitmentCampaignId { get; set; }
        public List<Guid?> ListVacanciesId { get; set; }
        public DateTime? ApplicationDateFrom { get; set; }
        public DateTime? ApplicationDateTo { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public List<Guid?> ListRecruitmentChannelId { get; set; }
        public List<int?> ListStatus { get; set; }
    }

}
