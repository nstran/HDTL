using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Manufacture;
using TN.TNM.DataAccess.Messages.Parameters.Manufacture;

namespace TN.TNM.BusinessLogic.Messages.Requests.Manufacture
{
    public class UpdateTechniqueRequestRequest : BaseRequest<UpdateTechniqueRequestParameter>
    {
        public TechniqueRequestModel TechniqueRequest { get; set; }
        public override UpdateTechniqueRequestParameter ToParameter()
        {
            return new UpdateTechniqueRequestParameter()
            {
                TechniqueRequest = TechniqueRequest.ToEntity(),
                UserId = UserId
            };
        }
    }
}
