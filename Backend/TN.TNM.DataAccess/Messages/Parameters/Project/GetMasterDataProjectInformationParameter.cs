using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Project
{
    public class GetMasterDataProjectInformationParameter : BaseParameter
    {
        public Guid ProjectId { get; set; }
    }
}
