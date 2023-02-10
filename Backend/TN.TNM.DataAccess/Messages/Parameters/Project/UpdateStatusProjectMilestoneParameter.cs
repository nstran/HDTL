using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Project
{
    public class UpdateStatusProjectMilestoneParameter : BaseParameter
    {
        public Guid ProjectMilestoneId { get; set; }
        // 0: Đóng ; 1: Mở lại
        public int Type { get; set; }
    }
}
