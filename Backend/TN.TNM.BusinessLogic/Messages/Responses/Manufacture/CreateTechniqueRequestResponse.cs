using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Manufacture;

namespace TN.TNM.BusinessLogic.Messages.Responses.Manufacture
{
    public class CreateTechniqueRequestResponse : BaseResponse
    {
        public Guid TechniqueRequestId { get; set; }
        public List<TechniqueRequestModel> ListParent { get; set; }
    }
}
