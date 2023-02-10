using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Task;

namespace TN.TNM.BusinessLogic.Messages.Requests.Task
{
    public class AcceptOrRejectByDayRequest : BaseRequest<AcceptOrRejectByDayParameter>
    {
        public Guid TimeSheetDetail { get; set; }
        public Boolean Check { get; set; }

        public override AcceptOrRejectByDayParameter ToParameter()
        {
            return new AcceptOrRejectByDayParameter
            {
                TimeSheetDetail = this.TimeSheetDetail,
                UserId = this.UserId,
                Check = this.Check
            };
        }
    }
}
