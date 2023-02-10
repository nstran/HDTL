using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Project;

namespace TN.TNM.BusinessLogic.Messages.Requests.Project
{
    public class GetMasterProjectRequest : BaseRequest<GetMasterProjectParameter>
    {
        public override GetMasterProjectParameter ToParameter()
        {
            return new GetMasterProjectParameter() { };
        }
    }
}
