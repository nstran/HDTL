using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Manufacture;

namespace TN.TNM.BusinessLogic.Messages.Requests.Manufacture
{
    public class GetDataReportManufactureRequest: BaseRequest<GetDataReportManufactureParameter>
    {
        public override GetDataReportManufactureParameter ToParameter()
        {
            return new GetDataReportManufactureParameter()
            {
                UserId = UserId
            };
        }
    }
}
