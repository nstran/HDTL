using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Project;
using TN.TNM.DataAccess.Messages.Parameters.Project;

namespace TN.TNM.BusinessLogic.Messages.Requests.Project
{
    public class CreateOrUpdateProjectResourceRequest : BaseRequest<CreateOrUpdateProjectResourceParameter>
    {       
        public ProjectResourceModel ProjectResource { get; set; }
        public override CreateOrUpdateProjectResourceParameter ToParameter()
        {
            return new CreateOrUpdateProjectResourceParameter() {
                //ProjectResource = ProjectResource.ToEntity(),
                UserId = UserId
            };
        }
    }
}
