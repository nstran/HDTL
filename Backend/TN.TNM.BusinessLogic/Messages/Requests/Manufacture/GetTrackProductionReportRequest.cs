using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Manufacture;

namespace TN.TNM.BusinessLogic.Messages.Requests.Manufacture
{
    public class GetTrackProductionReportRequest : BaseRequest<GetTrackProductionReportParameter>
    {
        public string OrganizationCode { get; set; }
        public List<Guid?> ListProductionOrderId { get; set; }
        public override GetTrackProductionReportParameter ToParameter()
        {
            return new GetTrackProductionReportParameter()
            {
                OrganizationCode = OrganizationCode,
                ListProductionOrderId = ListProductionOrderId,
                UserId = UserId,
            };
        }
    }
}
