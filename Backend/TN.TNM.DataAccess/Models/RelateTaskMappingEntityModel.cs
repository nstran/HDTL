using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models
{
    public class RelateTaskMappingEntityModel
    {
        public Guid? RelateTaskMappingId { get; set; }
        public Guid RelateTaskId { get; set; }
        public Guid? TaskId { get; set; }

        public Guid ProjectId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? CreatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public Guid? TenantId { get; set; }
        public string TaskName { get; set; }
        public string TaskCode { get; set; }
        public string StatusCode { get; set; }
        public string StatusName { get; set; }
        public string BackgroundColorForStatus { get; set; }
        public Boolean? IsDelete { get; set; }
        public DateTime? ExpectedStartDate { get; set; }
        public DateTime? ExpectedEndDate { get; set; }

    }
}
