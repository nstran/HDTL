using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Task
{
    public class AcceptOrRejectByDayParameter : BaseParameter
    {
        public Guid TimeSheetDetail { get; set; }
        public Boolean Check { get; set; }

    }
}
