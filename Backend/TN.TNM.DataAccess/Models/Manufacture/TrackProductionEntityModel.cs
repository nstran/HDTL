using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Manufacture
{
    public class TrackProductionEntityModel
    {
        public Guid ProductionOrderMappingId { get; set; }
        public Guid? ParentId { get; set; }
        public bool? ParentType { get; set; }
        public Guid TechniqueRequestId { get; set; }
        public Guid ProductionOrderId { get; set; }
        public DateTime? EndDate { get; set; }
        public double? ProductLength { get; set; }
        public double? ProductWidth { get; set; }
        public double? ProductThickness { get; set; }
        public string Code { get; set; }
        public string ProductName { get; set; }
        public string TechniqueDescription { get; set; }
        public Guid? StatusItemId { get; set; }
        public string StatusCode { get; set; }
        public string StatusItemName { get; set; }
        public Guid? OriginalId { get; set; }
        public short? Rate { get; set; } //Hệ số của tiến trình hiện tại
        public double? PreCompleteQuantity { get; set; } //Số tấm đã hoàn thành của tiến trình trước
        public double? Quantity { get; set; } //Số tấm thành phẩm
        public double? CompleteQuantity { get; set; } //Số tấm đã hoàn thành
        public double? UnitQuantity { get; set; }   //Số tấm phải làm
        public double? ActionQuantity { get; set; } //Tổng số lần nhấn cộng và trừ
        public bool IsShow { get; set; } //True: là item thuộc tiến trình hiện tại, False: là item không thuộc tiến trình hiện tại
        public int TotalErrPre { get; set; }    //Tổng số tấm lỗi của tiến trình trước
        public short? TechniqueOrder { get; set; }    //Số thứ tự của Tiến trình hiện tại
        public bool? IsParent { get; set; }
        public bool? IsSubParent { get; set; }
        public Guid? ParentPartId { get; set; }
        public int Type { get; set; }   //1: Item cha, 0: Không phải Item cha
        public Guid? ParentExtendId { get; set; }
        public int? Borehole { get; set; }
        public int? Hole { get; set; }
        public string ProductColor { get; set; }
        public bool? IsAddPart { get; set; }
        public string TextColorMode { get; set; }
        public string Note { get; set; }
        public bool? Present { get; set; }

        //1: Item thuộc lệnh sản xuất ưu tiên, 2: Item thuộc lệnh sản 
        //3: Item thuộc lệnh sản xuất không ưu tiên
        public int OrderType { get; set; }


        public List<double> ListRateChild { get; set; }
        public List<TechniqueRequestEntityModel> ListTechniqueRequest { get; set; }
    }
}
