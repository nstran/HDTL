using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.MilestoneConfiguration;

namespace TN.TNM.DataAccess.Messages.Parameters.MilestoneConfiguration
{
    public class UpdateListMilestoneConfigurationParameter
    {
        public List<MilestoneConfigurationEntityModel> ListMilestoneConfigurationEntityModel { get; set; }
    }
}
