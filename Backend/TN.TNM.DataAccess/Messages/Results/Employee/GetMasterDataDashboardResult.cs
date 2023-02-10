using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.DataAccess.Messages.Results.Employee
{
    public class GetMasterDataDashboardResult : BaseResult
    {
        public List<ChartCandidateFollowStatus> ListChartCandidateFollowStatus { get; set; }

        public List<NumberCandidateFollowRecruitmentChannel> ListNumberCandidateFollowRecruitmentChannel { get; set; }

        public List<NumberCandidateTurnEmployeeFollowEmployee> ListNumberCandidateTurnEmployeeFollowEmployee { get; set; }

        public List<InterviewScheduleEntityModel> ListInterviewSchedule { get; set; }
        public bool IsNguoiPhuTrach { get; set; }
        public bool IsGD { get; set; }
        public bool IsManagerOfHR { get; set; }
    }

}
