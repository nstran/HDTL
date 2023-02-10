using System.Collections.Generic;
using TN.TNM.DataAccess.Models;

namespace TN.TNM.DataAccess.Messages.Results.Admin
{
    public class GetMenuByModuleCodeResult: BaseResult
    {
        public ICollection<PermissionEntityModel> Permissions { get; set; }
        //public List<PermissionEntityModel> Permissions { get; set; }
    }
}
