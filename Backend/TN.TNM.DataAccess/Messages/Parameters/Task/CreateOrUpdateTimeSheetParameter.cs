using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Task;

namespace TN.TNM.DataAccess.Messages.Parameters.Task
{
    public class CreateOrUpdateTimeSheetParameter : BaseParameter
    {
        public TimeSheetEntityModel TimeSheet { get; set; }
    }
}
