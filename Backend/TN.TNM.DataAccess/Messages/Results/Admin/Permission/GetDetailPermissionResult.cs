using System.Collections.Generic;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.ActionResource;
using TN.TNM.DataAccess.Models.MenuBuild;

namespace TN.TNM.DataAccess.Messages.Results.Admin.Permission
{
    public class GetDetailPermissionResult : BaseResult
    {
        public List<ActionResourceEntityModel> ListActionResource { get; set; }
        public List<ActionResourceEntityModel> ListCurrentActionResource { get; set; }
        public Role Role { get; set; }
        public List<MenuBuildEntityModel> ListMenuBuild { get; set; }
        public List<PermissionTempModel> Module_Mapping { get; set; }
        public List<PermissionTempModel> Resource_Mapping { get; set; }
        public List<PermissionTempModel> Action_Mapping { get; set; }
    }
}
