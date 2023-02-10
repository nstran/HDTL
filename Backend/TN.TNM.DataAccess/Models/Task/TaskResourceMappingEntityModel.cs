using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Task
{
    public class TaskResourceMappingEntityModel
    {
        public Guid TaskResourceMappingId { get; set; }
        public Guid TaskId { get; set; }
        public Guid ResourceId { get; set; }
        public decimal? Hours { get; set; }
        public bool? IsPersonInCharge { get; set; }
        public bool? IsChecker { get; set; }
    }
}
