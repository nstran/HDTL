using System.Collections.Generic;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.DataAccess.Messages.Results.Employee
{
    public class GetAllVacanciesResult : BaseResult
    {
        public List<EmployeeVacanciesEntityModel> ListViTriTuyenDung { get; set; }
        public List<CategoryEntityModel> ListViTriFilter { get; set; }
        public List<RecruitmentCampaignEntityModel> ListChienDich { get; set; }
        public List<EmployeeEntityModel> ListNguoiPT { get; set; }
        public List<CategoryEntityModel> ListLoaiCV { get; set; }
        public List<CategoryEntityModel> ListExperience { get; set; }
        public List<CategoryEntityModel> ListChanel { get; set; }
    }
}
