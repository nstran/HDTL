using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Task;
using TN.TNM.DataAccess.Models.Task;

namespace TN.TNM.BusinessLogic.Messages.Requests.Task
{
    public class CreateOrUpdateTimeSheetRequest : BaseRequest<CreateOrUpdateTimeSheetParameter>
    {
        public TimeSheetEntityModel TimeSheet { get; set; }
        public override CreateOrUpdateTimeSheetParameter ToParameter()
        {
            return new CreateOrUpdateTimeSheetParameter
            {
                TimeSheet = this.TimeSheet,
                UserId = this.UserId
            };
        }
    }
}
