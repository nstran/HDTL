using System.Collections.Generic;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.DataAccess.Messages.Results.Employee
{
    public class GetMasterDataRecruitmentCampaignInformationResult : BaseResult
    {
        public RecruitmentCampaignEntityModel RecruitmentCampaign { get; set; }
    }
}
