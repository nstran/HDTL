using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Employee;

namespace TN.TNM.BusinessLogic.Messages.Requests.Employee
{
    public class GetMasterDataEmployeeDetailRequest : BaseRequest<GetMasterDataEmployeeDetailParameter>
    {
        public override GetMasterDataEmployeeDetailParameter ToParameter()
        {
            return new GetMasterDataEmployeeDetailParameter()
            {
                UserId = UserId
            };
        }
    }
}
