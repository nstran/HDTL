using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Project
{
    public class GetMasterDataCommonDashboardProjectParameter : BaseParameter
    {
        public Guid ProjectId { get; set; }
    }
}
