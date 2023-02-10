using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Project;
using TN.TNM.DataAccess.Messages.Parameters.Project;

namespace TN.TNM.BusinessLogic.Messages.Requests.Project
{
    public class UpdateProjectVendorRequest : BaseRequest<UpdateProjectVendorParameter>
    {
        public ProductVendorModel ProjectVendor { get; set; } 
        public override UpdateProjectVendorParameter ToParameter()
        {
            return new UpdateProjectVendorParameter()
            {
                //ProjectVendor = ProjectVendor.ToEntity(),
                UserId = UserId,
            };
        }
    }
}
