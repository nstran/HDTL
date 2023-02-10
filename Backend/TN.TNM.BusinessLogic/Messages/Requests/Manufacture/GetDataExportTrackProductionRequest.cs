using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Manufacture;

namespace TN.TNM.BusinessLogic.Messages.Requests.Manufacture
{
    public class GetDataExportTrackProductionRequest : BaseRequest<GetDataExportTrackProductionParameter>
    {
        public string OrganizationCode { get; set; }
        public override GetDataExportTrackProductionParameter ToParameter()
        {
            return new GetDataExportTrackProductionParameter()
            {
                UserId = UserId,
                OrganizationCode = OrganizationCode
            };
        }
    }
}
