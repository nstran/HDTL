using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.DataAccess.Messages.Results.Employee
{
    public class GetMasterDataBaoCaoTuyenDungResult : BaseResult
    {
        public List<RecruitmentCampaignEntityModel> ListRecruitmentCampaign { get; set; }
        public List<EmployeeEntityModel> ListEmployee { get; set; }
        public List<VacancyEntityModel> ListVacancies { get; set; }
    }
}
