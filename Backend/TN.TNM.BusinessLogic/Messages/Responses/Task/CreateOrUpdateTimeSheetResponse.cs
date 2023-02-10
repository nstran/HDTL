using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.BusinessLogic.Messages.Responses.Task
{
    public class CreateOrUpdateTimeSheetResponse : BaseResponse
    {
        public Guid? TimeSheetId { get; set; }

    }
}
