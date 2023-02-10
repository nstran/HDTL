using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Note
{
    public class CreateNoteMilestoneParameter : BaseParameter
    {
        public Databases.Entities.Note Note { get; set; }
        public bool IsSendEmail { get; set; }
    }
}
