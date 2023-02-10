using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.MenuBuild
{
    public class UpdateIsShowParameter : BaseParameter
    {
        public Guid MenuBuildId { get; set; }
        public bool IsShow { get; set; }
    }
}
