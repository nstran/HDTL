using System.Collections.Generic;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Permission;

namespace TN.TNM.DataAccess.Messages.Results.Admin.Permission
{
    public class GetAllRoleResult : BaseResult
    {
        public List<RoleEntityModel> ListRole { get; set; }
    }
}
