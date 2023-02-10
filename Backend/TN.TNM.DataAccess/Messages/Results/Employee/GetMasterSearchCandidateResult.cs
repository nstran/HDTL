using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.DataAccess.Messages.Results.Employee
{
    public class GetMasterSearchCandidateResult : BaseResult
    {
        public List<RecruitmentCampaignEntityModel> ListRecruitmentCampaign { get; set; }

        public List<VacancyEntityModel> ListVacancies { get; set; }

        public List<CategoryEntityModel> ListRecruitmentChannel { get; set; }
    }

}
