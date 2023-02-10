using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.MilestoneConfiguration;
using TN.TNM.DataAccess.Models.ProductAttributeCategory;

namespace TN.TNM.DataAccess.Messages.Parameters.MilestoneConfiguration
{
    public class CreateOrUpdateMilestoneConfigurationParameter : BaseParameter
    {
        public MilestoneConfigurationEntityModel MilestoneConfigurationEntityModel { get; set; }
    }
}
