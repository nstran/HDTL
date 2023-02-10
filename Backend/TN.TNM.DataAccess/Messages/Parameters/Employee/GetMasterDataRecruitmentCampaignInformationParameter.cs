using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class GetMasterDataRecruitmentCampaignInformationParameter : BaseParameter
    {
        public Guid VacanciesId { get; set; }
        public Guid RecruitmentCampaignId { get; set; }
    }
}
