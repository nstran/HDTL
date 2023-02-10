using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.CustomerOrder
{
    public class CustomerOrderExtensionEntityModel
    {
        public Guid? Id { get; set; }
        public Guid? AttributeConfigurationId { get; set; }
        public Guid? ObjectId { get; set; }
        public string AttrName { get; set; }
        public string ObjectType { get; set; }
        public string Value { get; set; }
        public int? DataType { get; set; }
        public Guid? CreatedById { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }
        public Guid? OrderDetailId { get; set; }
        public Guid? ServicePacketMappingOptionsId { get; set; }
    }
}
