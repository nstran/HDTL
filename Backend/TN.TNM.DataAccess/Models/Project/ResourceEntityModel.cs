using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Project
{
    public class ResourceEntityModel
    {
        public Guid ResourceId { get; set; }
        public Guid ResourceName { get; set; }
        public string NameResources { get; set; }
        public string ResourceDescription { get; set; }
        public Guid CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public Guid? UpdateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? TenantId { get; set; }
    }
}
