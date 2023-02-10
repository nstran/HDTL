using System.Collections.Generic;
using TN.TNM.DataAccess.Models;

namespace TN.TNM.DataAccess.Messages.Results.Admin.Organization
{
    public class GetFinancialindependenceOrgResult : BaseResult
    {
        public List<OrganizationEntityModel> ListOrg { get; set; }
    }
}
