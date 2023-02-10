using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Manufacture;
using TN.TNM.DataAccess.Messages.Parameters.Manufacture;
using TN.TNM.DataAccess.Models.Manufacture;

namespace TN.TNM.BusinessLogic.Messages.Requests.Manufacture
{
    public class CreateTechniqueRequestRequest : BaseRequest<CreateTechniqueRequestParameter>
    {
        public TechniqueRequestModel TechniqueRequest { get; set; }
        public override CreateTechniqueRequestParameter ToParameter()
        {
            return new CreateTechniqueRequestParameter()
            {
                UserId = UserId,
                TechniqueRequest = TechniqueRequest.ToEntity()
            };
        }
    }
}
