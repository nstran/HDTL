using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Project;

namespace TN.TNM.BusinessLogic.Messages.Requests.Project
{
    public class SynchronizedEvnRequest : BaseRequest<SynchronizedEvnParameter>
    {
        public override SynchronizedEvnParameter ToParameter()
        {
            return new SynchronizedEvnParameter
            {
                UserId = this.UserId
            };
        }
    }
}
