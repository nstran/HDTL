using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Models.Manufacture
{
    public class ProductionOrderEntityModel
    {
        public Guid ProductionOrderId { get; set; }

        //Lệnh số
        public string ProductionOrderCode { get; set; }
        public Guid? ParentId { get; set; }

        public Guid OrderId { get; set; }

        public string CustomerNumber { get; set; }

        // Tên khách hàng
        public string CustomerName { get; set; }

        public string PlaceOfDelivery { get; set; }

        // Ngày dự kiến trả
        public DateTime? ReceivedDate { get; set; }

        // Ngày sản xuất
        public DateTime? StartDate { get; set; }

        // Ngày dự kiến trả
        public DateTime? EndDate { get; set; }

        // Ghi chú
        public string Note { get; set; }

        // Ghi chú kĩ thuật
        public string NoteTechnique { get; set; }

        public bool? Especially { get; set; }

        // Trạng thái sản xuất
        public Guid StatusId { get; set; }

        public DateTime CreatedDate { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? UpdatedById { get; set; }

        public string StatusName { get; set; }
        public List<string> TotalProductionOrderCode { get; set; }

        //Số tấm còn lại (chưa hoàn thành)
        public double? RemainQuantity { get; set; }

        //Số m2 còn lại
        public double? RemainTotalArea { get; set; }

        public string StatusCode { get; set; }

        public List<TechniqueRequestEntityModel> ListTechniqueRequest { get; set; }

        public bool? IsError { get; set; } // Lệnh sản xuất có item bị lỗi hay không
        public int NumberProductionOrder { get; set; }// Số lệnh bổ sung cho lệnh sản xuất
        public string NameOrganization { get; set; } // Tên tổ gây lỗi
        public string DescriptionError { get; set; } // Mô tả lỗi
        public bool? IsIgnore { get; set; }  //true: không tính vào thống kê, false: tính vào thống kê

        //Kiểm tra lệnh sản xuất có đổi quy trình kính cắt hạ không?
        //Nếu value = true: có item cần đổi quy trình sang Cắt + Cắt = Dán;
        public bool? IsChangeTech { get; set; }
    }
}
