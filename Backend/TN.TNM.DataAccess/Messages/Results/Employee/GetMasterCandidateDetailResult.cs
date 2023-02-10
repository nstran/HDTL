using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Employee;
using TN.TNM.DataAccess.Models.Folder;
using TN.TNM.DataAccess.Models.Note;

namespace TN.TNM.DataAccess.Messages.Results.Employee
{
    public class GetMasterCandidateDetailResult : BaseResult
    {
        public CandidateEntityModel CandidateModel { get; set; }
        public VacancyEntityModel VacancyModel { get; set; }
        public RecruitmentCampaignEntityModel RecruitmentCampaignModel { get; set; }
        public List<FileInFolderEntityModel> ListFileInFolder { get; set; }
        public List<NoteEntityModel> ListNote { get; set; }
        public List<OverviewCandidateEntityModel> ListOverviewCandidate { get; set; }
        public List<InterviewScheduleEntityModel> ListInterviewSchedule { get; set; }
        public List<QuizEntityModel> ListQuiz { get; set; }
        public List<CandidateAssessmentEntityModel> ListCandidateAssessment { get; set; }
        public List<CategoryEntityModel> ListSessionReview { get; set; }
        public List<EmployeeEntityModel> ListEmployeeModels { get; set; }
        public bool IsNguoiPhongVan { get; set; }
        public bool IsNVPhuTrach { get; set; }
        public bool IsTruongBoPhan { get; set; }
        public Guid LoginEmp { get; set; }

    }
}
