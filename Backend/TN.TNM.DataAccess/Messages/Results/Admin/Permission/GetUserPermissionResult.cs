using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Results.Admin.Permission
{
    public class GetUserPermissionResult : BaseResult
    {
        public string ListPermissionResource { get; set; }
    }
}
