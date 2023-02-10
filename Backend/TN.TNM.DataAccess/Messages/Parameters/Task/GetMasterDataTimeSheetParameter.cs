using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Task;

namespace TN.TNM.DataAccess.Messages.Parameters.Task
{
    public class GetMasterDataTimeSheetParameter : BaseParameter
    {
        public Guid? ProjectId { get; set; }
    }
}
