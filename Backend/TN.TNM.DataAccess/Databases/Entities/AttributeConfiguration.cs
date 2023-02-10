using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class AttributeConfiguration
    {
        public Guid Id { get; set; }
        public int DataType { get; set; }
        public Guid CategoryId { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid UpdatedById { get; set; }
        public DateTime UpdatedDate { get; set; }
        public Guid TenantId { get; set; }
        public int ObjectType { get; set; }
        public bool? IsUsing { get; set; }
        public Guid ObjectId { get; set; }
    }
}
