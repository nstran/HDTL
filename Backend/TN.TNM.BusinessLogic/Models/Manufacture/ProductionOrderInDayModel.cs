using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Manufacture;

namespace TN.TNM.BusinessLogic.Models.Manufacture
{
    public class ProductionOrderInDayModel : BaseModel<ProductionOrderInDayEntityModel>
    {
        public Guid ProductionOrderId { get; set; }
        public string ProductionOrderCode { get; set; }
        public string CustomerName { get; set; }
        public double CompleteQuantity { get; set; }
        public double TotalQuantity { get; set; }
        public string TechniqueRequestCode { get; set; }
        public bool IsShow { get; set; }
        public double TotalArea { get; set; }
        public double TotalComplete { get; set; }

        public ProductionOrderInDayModel() { }

        public ProductionOrderInDayModel(ProductionOrderInDayEntityModel model)
        {
            Mapper(model, this);
        }

        public override ProductionOrderInDayEntityModel ToEntity()
        {
            var entity = new ProductionOrderInDayEntityModel();
            Mapper(this, entity);
            return entity;
        }
    }
}
