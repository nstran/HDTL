using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Task;

namespace TN.TNM.DataAccess.Messages.Parameters.Task
{
    public class UpdateStatusTimeSheetParameter : BaseParameter
    {
        public List<Guid> ListTimeSheetId { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public TimeSheetEntityModel TimeSheet { get; set; }

    }
}
