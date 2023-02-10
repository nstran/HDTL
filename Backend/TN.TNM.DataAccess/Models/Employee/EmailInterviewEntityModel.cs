using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Employee
{
    public class EmailInterviewEntityModel
    {
        public List<CandidateEmailPVModel> ListCandidate { get; set; }
        public string VancaciesName { get; set; }
        public string PersonInChagerName { get; set; }
        public string PersonInChagerPhone { get; set; }
        public string SendContent { get; set; }
        public string Subject { get; set; }
        public string WorkPlace { get; set; }
    }
    public class CandidateEmailPVModel
    {
        public string FullName { get; set; }
        public DateTime InterviewTime { get; set; }
        public string AddressOrLink { get; set; }
        public string Email { get; set; }
        public int InterviewScheduleType { get; set; }
        public List<string> ListInterviewerEmail { get; set; }
    }
}
