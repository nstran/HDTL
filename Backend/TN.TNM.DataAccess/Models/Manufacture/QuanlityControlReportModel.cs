using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Manufacture
{
    public class QuanlityControlReportModel
    {
        public Guid? ProductionOrderHistoryId { get; set; }
        public DateTime? CreatedDate { get; set; } //Ngày thánh
        public string ProductionOrderCode { get; set; } //Lệnh số
        public string CustomerName { get; set; } //Tên khách hàng
        public string ProductName { get; set; } //Sản phẩm
        public double? ProductLength { get; set; } //Kích thước: Dài
        public double? ProductWidth { get; set; } //Kích thước: Rộng
        public double? Quantity { get; set; } //Số tấm
        public double? TotalArea { get; set; } //Số m2
        public double? ProductThickness { get; set; } //Độ dày
        public string ProductColor { get; set; } //Màu
        public int? Borehole { get; set; }  //Khoan
        public int? Hole { get; set; }  //Khoét
        public string Description { get; set; } //Ghi chú tổ SX
        public Guid? NoteQc { get; set; } //Ghi chú QC
        public Guid? ErrorType { get; set; } // Loại lỗi

        public Guid? ProductionOrderId { get; set; }
    }
}
