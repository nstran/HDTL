using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Task
{
    public class ChangeStatusTaskParameter : BaseParameter
    {
        public Guid TaskId { get; set; }
        public Guid StatusId { get; set; }
        //0: not care; 1 is start; 2 is Re-open
        public int Type { get; set; }
    }
}
