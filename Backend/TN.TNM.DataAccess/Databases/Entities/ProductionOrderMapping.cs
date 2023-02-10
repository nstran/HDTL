using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class ProductionOrderMapping
    {
        public Guid ProductionOrderMappingId { get; set; }
        public Guid? ParentId { get; set; }
        public Guid ProductionOrderId { get; set; }
        public Guid? ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public string ProductColor { get; set; }
        public string ProductColorCode { get; set; }
        public double? ProductThickness { get; set; }
        public double? ProductLength { get; set; }
        public double? ProductWidth { get; set; }
        public double? Quantity { get; set; }
        public double? TotalArea { get; set; }
        public string TechniqueDescription { get; set; }
        public Guid StatusId { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public Guid? TenantId { get; set; }
        public Guid? ProductOrderWorkflowId { get; set; }
        public bool? ParentType { get; set; }
        public int? Borehole { get; set; }
        public int? Hole { get; set; }
        public string ProductGroupCode { get; set; }
        public bool? Especially { get; set; }
        public Guid? OriginalId { get; set; }
        public bool? IsParent { get; set; }
        public bool? IsSubParent { get; set; }
        public Guid? StartId { get; set; }
        public Guid? ParentPartId { get; set; }
        public double? Grind { get; set; }
        public Guid? ParentExtendId { get; set; }
        public double? Stt { get; set; }
        public bool? IsAddPart { get; set; }
        public bool? Present { get; set; }
    }
}
