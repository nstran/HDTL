using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.BusinessLogic.Messages.Responses.Project
{
    public class GetPermissionResponse : BaseResponse
    {
        public List<string> ListPermission { get; set; }
        public string PermissionStr { get; set; }
    }
}
