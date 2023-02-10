using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Project
{
    public class ProjectTargetEntityModel
    {
        public Guid ProjectObjectId { get; set; }
        public Guid ProjectId { get; set; }
        public int? OrderNumber { get; set; }
        public Guid? ProjectObjectType { get; set; }
        public string ProjectObjectName { get; set; }
        public string TargetTypeDisplay { get; set; }
        public string TargetUnitDisplay { get; set; }
        public Guid? ProjectObjectUnit { get; set; }
        public decimal? ProjectObjectValue { get; set; }
        public string ProjectObjectDescription { get; set; }
        public Guid? TenantId { get; set; }
    }
}
