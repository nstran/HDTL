using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Manufacture;

namespace TN.TNM.BusinessLogic.Models.Manufacture
{
    public class ProductionOrderModel : BaseModel<ProductionOrderEntityModel>
    {
        public Guid ProductionOrderId { get; set; }

        //Lệnh số
        public string ProductionOrderCode { get; set; }

        public Guid OrderId { get; set; }

        public string CustomerNumber { get; set; }

        // Tên khách hàng
        public string CustomerName { get; set; }

        public string OrderCustomerCode { get; set; }

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
        public double? RemainQuantity { get; set; }
        public double? RemainTotalArea { get; set; }
        public string StatusCode { get; set; }
        public bool? IsError { get; set; } // Lệnh sản xuất có item bị lỗi hay không
        public int NumberProductionOrder { get; set; }// Số lệnh bổ sung cho lệnh sản xuất
        public Guid? ParentId { get; set; }
        public string NameOrganization { get; set; } // Tên tổ gây lỗi
        public string DescriptionError { get; set; } // Mô tả lỗi
        public bool? IsIgnore { get; set; }  //true: không tính vào thống kê, false: tính vào thống kê
        public bool? IsChangeTech { get; set; }

        public List<TechniqueRequestModel> ListTechniqueRequest { get; set; }

        public ProductionOrderModel() { }

        public ProductionOrderModel(ProductionOrderEntityModel model)
        {
            Mapper(model, this);
            if (model.ListTechniqueRequest != null)
            {
                var cList = new List<TechniqueRequestModel>();
                model.ListTechniqueRequest.ForEach(child =>
                {
                    cList.Add(new TechniqueRequestModel(child));
                });
                this.ListTechniqueRequest = cList;
            }
        }

        public override ProductionOrderEntityModel ToEntity()
        {
            var entity = new ProductionOrderEntityModel();
            Mapper(this, entity);
            return entity;
        }
    }
}
