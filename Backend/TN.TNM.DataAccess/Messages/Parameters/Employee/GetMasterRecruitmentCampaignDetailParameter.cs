using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class GetMasterRecruitmentCampaignDetailParameter : BaseParameter
    {
        public Guid RecruitmentCampaignId { get; set; }
    }
}
