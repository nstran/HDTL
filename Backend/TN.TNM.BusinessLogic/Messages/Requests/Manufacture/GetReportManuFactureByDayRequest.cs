using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Manufacture;

namespace TN.TNM.BusinessLogic.Messages.Requests.Manufacture
{
    public class GetReportManuFactureByDayRequest : BaseRequest<GetReportManuFactureByDayParameter>
    {
        public Guid? TechniqueRequestId { get; set; }
        public int? Shift { get; set; } //1: Ca ngày; 2: Ca đêm
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public override GetReportManuFactureByDayParameter ToParameter()
        {
            return new GetReportManuFactureByDayParameter()
            {
                TechniqueRequestId = TechniqueRequestId,
                Shift = Shift,
                FromDate = FromDate,
                ToDate = ToDate,
                UserId = UserId
            };
        }
    }
}
