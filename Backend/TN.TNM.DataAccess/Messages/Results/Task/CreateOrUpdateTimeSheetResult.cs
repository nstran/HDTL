using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Results.Task
{
    public class CreateOrUpdateTimeSheetResult : BaseResult
    {
        public Guid? TimeSheetId { get; set; }
    }
}
