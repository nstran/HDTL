using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Project
{
    public class UpdateProjectStatusParameter : BaseParameter
    {      
        public Guid ProjectId { get; set; }
        public string Status { get; set; }        
    }
}
