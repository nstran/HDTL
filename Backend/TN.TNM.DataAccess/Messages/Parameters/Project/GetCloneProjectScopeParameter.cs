using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Project
{
    public class GetCloneProjectScopeParameter : BaseParameter
    {
        public Guid NewProjectId { get; set; }
        public Guid OldProjectId { get; set; }
    }
}
