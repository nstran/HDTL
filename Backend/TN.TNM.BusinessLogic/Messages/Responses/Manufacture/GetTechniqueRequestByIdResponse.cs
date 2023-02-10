using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Admin;
using TN.TNM.BusinessLogic.Models.Manufacture;

namespace TN.TNM.BusinessLogic.Messages.Responses.Manufacture
{
    public class GetTechniqueRequestByIdResponse : BaseResponse
    {
        public TechniqueRequestModel TechniqueRequest { get; set; }
        public List<OrganizationModel> ListOrganization { get; set; }
        public List<TechniqueRequestModel> ListParent { get; set; }
    }
}
