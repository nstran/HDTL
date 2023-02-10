using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models;

namespace TN.TNM.DataAccess.Messages.Results.Admin.Organization
{
    public class GetOrganizationByUserResult: BaseResult
    {
        public List<OrganizationEntityModel> ListOrganization { get; set; }
        public List<Guid?> ListValidSelectionOrganization { get; set; }
    }
}
