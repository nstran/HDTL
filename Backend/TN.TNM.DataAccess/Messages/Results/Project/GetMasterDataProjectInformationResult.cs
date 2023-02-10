using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Project;

namespace TN.TNM.DataAccess.Messages.Results.Project
{
    public class GetMasterDataProjectInformationResult : BaseResult
    {
        public ProjectEntityModel Project { get; set; }

        public decimal ProjectTaskComplete { get; set; }
        
        public decimal TotalEstimateHour { get; set; }
        
        public decimal TotalHourUsed { get; set; }
        
        public decimal TotalEE { get; set; }
        
    }
}
