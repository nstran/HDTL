using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Manufacture;

namespace TN.TNM.BusinessLogic.Messages.Requests.Manufacture
{
    public class GetTechniqueRequestByIdRequest : BaseRequest<GetTechniqueRequestByIdParameter>
    {
        public Guid TechniqueRequestId { get; set; }
        public override GetTechniqueRequestByIdParameter ToParameter()
        {
            return new GetTechniqueRequestByIdParameter()
            {
                UserId = UserId,
                TechniqueRequestId = TechniqueRequestId
            };
        }
    }
}
