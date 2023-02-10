using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Models.Manufacture
{
    public class ProductionOrderMappingEntityModel
    {
        public Guid ProductionOrderMappingId { get; set; }
        public Guid? ParentId { get; set; }
        public Guid ProductionOrderId { get; set; }
        public Guid? ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductColor { get; set; }
        public string ProductColorCode { get; set; }
        public double? ProductThickness { get; set; }
        public double? ProductLength { get; set; }
        public double? ProductWidth { get; set; }
        public double? Quantity { get; set; }
        public double? TotalArea { get; set; }
        public string TechniqueDescription { get; set; }
        public Guid StatusId { get; set; }
        public string StatusCode { get; set; }
        public string Index { get; set; }
        public string ParentIndex { get; set; }
        public string ProductCode { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public bool? ParentType { get; set; }
        public Guid? ProductOrderWorkflowId { get; set; }
        public int? NumberTechniqueSpecial { get; set; }
        public int? Borehole { get; set; }
        public int? Hole { get; set; }
        public string ProductGroupCode { get; set; }

        public List<TechniqueRequestEntityModel> ListTechnique { get; set; }
        public string WorkflowName { get; set; } //Tên quy trình
        public double? CompleteQuantity { get; set; } //Số tấm còn lại (chưa làm xong)
        public double? CompleteTotalArea { get; set; } //Số m2 còn lại (chưa làm xong)

        public string Description { get; set; }// Mô tả lỗi
        public string NameNest { get; set; } // Tên tổ
        public Guid? ProductionOrderHistoryId { get; set; } // Id bản lịch sử
        public short? TechniqueOrder { get; set; } // Order tiến trình lỗi
        public Guid? OriginalId { get; set; }// Tiến trình lỗi
        public Guid? ParentPartId { get; set; }// Id bán thành phẩm cha

        public double? Grind { get; set; } //Mài
        public double? Stt { get; set; }    //Số thứ tự
        public string Note { get; set; }
        public string StatusName { get; set; }
        public string ProductOrderWorkflowName { get; set; }
        public bool? IsAddItem { get; set; }
        public bool? IsCreated { get; set; }
        public bool? IsOriginal { get; set; }
    }
}
