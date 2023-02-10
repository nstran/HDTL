using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Results.MilestoneConfiguration
{
    public class GetListMilestoneConfigurationResult : BaseResult
    {
        public List<ListMilestoneConfiguration> ListMilestoneConfiguration { get; set; }
    }
    public class ListMilestoneConfiguration
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int SortOrder { get; set; }

    }
}
