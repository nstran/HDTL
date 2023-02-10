using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Manufacture
{
    public class TrackProductionReportModel
    {   
        public double? Stt { get; set; }    //STT
        public Guid? ProductionOrderId { get; set; } //id đơn
        public string ProductionOrderCode { get; set; } //mã đơn
        public double? ProductLength { get; set; } //chiều dài
        public double? ProductWidth { get; set; } //chiều rộng
        public string TechniqueDescription { get; set; } //mã hiệu
        public double? Quantity { get; set; } // số lượng
        public string CustomerName { get; set; } //khách hàng
        public string ProductColor { get; set; } //màu sắc
        public double? ProductThickness { get; set; } //chủng loại
        public DateTime? ProductionDate { get; set; } //ngày sản xuất
        public string ProductMaterial { get; set; } //thành phần
    }
}
