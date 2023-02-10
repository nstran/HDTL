using System.Collections.Generic;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Employee;
using TN.TNM.DataAccess.Models.Folder;
using TN.TNM.DataAccess.Models.Note;
using TN.TNM.DataAccess.Models.Quote;

namespace TN.TNM.DataAccess.Messages.Results.Employee
{
    public class GetMasterDataVacanciesDetailResult : BaseResult
    {
        public List<EmployeeRecruitmentEntityModel> ListEmployeeRecruit { get; set; }
        public List<EmployeeEntityModel> ListEmployeePTTD { get; set; }
        public List<CategoryEntityModel> ListLoaiCV { get; set; }
        public List<CategoryEntityModel> ListKinhNghiem { get; set; }
        public EmployeeVacanciesEntityModel ViTriTuyenDung { get; set; }
        public List<NoteEntityModel> ListNote { get; set; }
        public List<CandidateEntityModel> ListCandidate { get; set; }
        public List<CategoryEntityModel> ListChanel { get; set; }
        public List<EmployeeEntityModel> ListAllEmployee { get; set; }
        public InforExportExcelModel InforExportExcel { get; set; }
        public List<FileInFolderEntityModel> ListFileResult { get; set; }
        public bool IsManagerOfHR { get; set; }
        public bool IsGD { get; set; }
        public bool IsNguoiPhuTrach { get; set; }
    }
}
