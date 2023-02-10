using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models;

namespace TN.TNM.DataAccess.Messages.Parameters.Task
{
    public class CreateRelateTaskParameter : BaseParameter
    {
        public RelateTaskMappingEntityModel TaskRelate { get; set; }

    }
}
