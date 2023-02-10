using System;
namespace TN.TNM.DataAccess.Models.Employee
{
    public class SynchronizeCandidateDataModel
    {
        public string FullName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Email { get; set; }
        public string RecruitmentCampaignYearName { get; set; }
        public string Phone { get; set; }        
        public string UrlFile { get; set; }        
        public Guid VacanciesId { get; set; }
    }
}
