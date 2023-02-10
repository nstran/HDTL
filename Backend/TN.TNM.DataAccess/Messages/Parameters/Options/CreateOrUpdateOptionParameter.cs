using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.AttributeConfigurationEntityModel;
using TN.TNM.DataAccess.Models.MilestoneConfiguration;
using TN.TNM.DataAccess.Models.Options;

namespace TN.TNM.DataAccess.Messages.Parameters.Options
{
    public class CreateOrUpdateOptionParameter : BaseParameter
    {
        public OptionsEntityModel OptionsEntityModel { get; set; }
        public CategoryEntityModel CategoryEntityModel { get; set; }
        public CategoryTypeEntityModel CategoryTypeEntityModel { get; set; }
        public AttributeConfigurationEntityModel AttributeConfigurationEntityModel { get; set; }
        public MilestoneConfigurationEntityModel MilestoneConfigurationEntityModel { get; set; }
    }
}
