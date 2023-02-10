using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.AttributeConfigurationEntityModel;
using TN.TNM.DataAccess.Models.Options;

namespace TN.TNM.DataAccess.Messages.Parameters.AttributeConfiguration
{
    public class CreateOrUpdateAttributeConfigureParamenter : BaseParameter
    {
        public AttributeConfigurationEntityModel AttributeConfigurationModel { get; set; }
        public OptionsEntityModel OptionsEntityModel { get; set; }
    }
}
