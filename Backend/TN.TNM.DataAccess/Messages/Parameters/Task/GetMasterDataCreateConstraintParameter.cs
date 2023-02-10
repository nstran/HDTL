using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Task
{
    public class GetMasterDataCreateConstraintParameter : BaseParameter
    {
        public Guid? TaskId { get; set; }
        public Guid ProjectId { get; set; }
    }
}
