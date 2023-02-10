using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Employee;
using TN.TNM.DataAccess.Models.Folder;
using TN.TNM.DataAccess.Models.Note;

namespace TN.TNM.DataAccess.Messages.Results.Employee
{
    public class GetMasterRecruitmentCampaignDetailResult : BaseResult
    {
        public RecruitmentCampaignEntityModel RecruitmentCampaign { get; set; }
        public List<VacancyEntityModel> ListVacancies { get; set; }
        public List<FileInFolderEntityModel> ListFileInFolder { get; set; }
        public List<NoteEntityModel> ListNote { get; set; }
        public List<CategoryEntityModel> ListChanel { get; set; }
        public List<CategoryEntityModel> ListKinhNghiem { get; set; }
        public List<CategoryEntityModel> ListLoaiCongViec { get; set; }
        public List<EmployeeEntityModel> listEmployeePTTD { get; set; }
        public bool IsManagerOfHR { get; set; }
        public bool IsGD { get; set; }
        public bool IsNguoiPhuTrach { get; set; }
    }
}
