using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Manufacture;

namespace TN.TNM.BusinessLogic.Messages.Requests.Manufacture
{
    public class GetDataReportQuanlityControlRequest : BaseRequest<GetDataReportQuanlityControlParameter>
    {
        public override GetDataReportQuanlityControlParameter ToParameter()
        {
            return new GetDataReportQuanlityControlParameter()
            {
                UserId = UserId
            };
        }
    }
}
