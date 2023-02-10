using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Manufacture;

namespace TN.TNM.BusinessLogic.Messages.Requests.Manufacture
{
    public class GetReportManuFactureByMonthRequest : BaseRequest<GetReportManuFactureByMonthParameter>
    {
        public Guid? TechniqueRequestId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public override GetReportManuFactureByMonthParameter ToParameter()
        {
            return new GetReportManuFactureByMonthParameter()
            {
                TechniqueRequestId = TechniqueRequestId,
                FromDate = FromDate,
                ToDate = ToDate,
                UserId = UserId
            };
        }
    }
}
