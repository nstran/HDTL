using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class UpdateEmployeeThongTinChungParameter:BaseParameter
    {
        public Guid RecruitmentCampaignId { get; set; }
    }
}
