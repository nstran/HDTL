using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Employee
{
    class DashboardModel
    {
    }


    /// <summary>
    /// Tỉ lệ ứng viên theo trạng thái
    /// </summary>
    public class ChartCandidateFollowStatus
    {
        public int? Status { get; set; }
        
        public string StatusName { get; set; }

        public int Count { get; set; }

        public string PercentValue { get; set; }
    }

    /// <summary>
    /// Số lượng ứng viên theo kênh tuyển dụng
    /// </summary>
    public class NumberCandidateFollowRecruitmentChannel
    {
        public Guid? RecruitmentChannelId { get; set; }
        
        public string RecruitmentChannelName { get; set; }

        public int TotalCandidateCount { get; set; }
        
        public int TotalCandidateInterviewCount { get; set; }
        
        public int TotalCandidateEmployeeCount { get; set; }
    }

    /// <summary>
    /// SỐ LƯỢNG NV/THỬ VIỆC TUYỂN ĐƯỢC THEO NHÂN VIÊN
    /// </summary>
    public class NumberCandidateTurnEmployeeFollowEmployee
    {
        public Guid? PersonInChargeId { get; set; }

        public List<Guid> ListVacanciesId { get; set; }

        public string PersonInChargeName { get; set; }
        
        public int Count { get; set; }
    }
    
}
