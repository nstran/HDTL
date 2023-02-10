using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.MilestoneConfiguration
{
    public class GetListMilestoneConfigurationByOptionIdParameter : BaseParameter
    {
        public Guid Id { get; set; }
    }
}
