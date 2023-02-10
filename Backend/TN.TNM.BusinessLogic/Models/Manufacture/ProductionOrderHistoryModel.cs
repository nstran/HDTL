using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Manufacture;

namespace TN.TNM.BusinessLogic.Models.Manufacture
{
    public class ProductionOrderHistoryModel : BaseModel<ProductionOrderHistoryEntityModel>
    {
        public Guid ProductionOrderHistoryId { get; set; }
        public Guid? ParentId { get; set; }
        public bool? ParentType { get; set; }
        public Guid ProductionOrderId { get; set; }
        public Guid ProductionOrderMappingId { get; set; }
        public Guid TechniqueRequestId { get; set; }
        public bool? CalculatorType { get; set; }
        public bool? IsError { get; set; }
        public string Description { get; set; }
        public double? QuantityUnitErr { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public bool? Thin { get; set; }
        public bool? Thick { get; set; }
        public bool? EspeciallyThin { get; set; }
        public bool? EspeciallyThick { get; set; }
        public bool? BoreholeThin { get; set; }
        public bool? BoreholeThick { get; set; }
        public bool? OriginalThin { get; set; }
        public bool? OriginalThick { get; set; }
        public bool? IsAdd { get; set; }
        public bool? IsErrorPre { get; set; }
        public Guid? OriginalId { get; set; }
        public bool? IsParent { get; set; }
        public bool? IsSubParent { get; set; }
        public Guid? IdChildrent { get; set; }
        public Guid? ParentPartId { get; set; }
        public Guid? ParentExtendId { get; set; }
        public bool? IsChildren { get; set; }
        public bool? IsAddPart { get; set; }
        public bool? Present { get; set; }

        //Số tấm còn phải cộng của một Item (Dùng cho chức năng hoàn thành nhanh)
        public double? RemainQuantity { get; set; }

        public ProductionOrderHistoryModel () { }

        public ProductionOrderHistoryModel(ProductionOrderHistoryEntityModel model)
        {
            Mapper(model, this);
        }

        public override ProductionOrderHistoryEntityModel ToEntity()
        {
            var entity = new ProductionOrderHistoryEntityModel();
            Mapper(this, entity);
            return entity;
        }
    }
}
