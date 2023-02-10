using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Manufacture;

namespace TN.TNM.BusinessLogic.Models.Manufacture
{
    public class RememberItemModel : BaseModel<RememberItemEntityModel>
    {
        public Guid RememberItemId { get; set; }
        public Guid? ProductionOrderId { get; set; }
        public Guid? ProductionOrderMappingId { get; set; }
        public double? Quantity { get; set; }
        public string Description { get; set; }
        public bool? IsCheck { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? UpdatedById { get; set; }

        public string ProductionOrderCode { get; set; }
        public string ProductName { get; set; }
        public double? ProductThickness { get; set; }
        public double? ProductLength { get; set; }
        public double? ProductWidth { get; set; }

        public RememberItemModel() { }

        public RememberItemModel(RememberItemEntityModel model)
        {
            Mapper(model, this);
        }

        public override RememberItemEntityModel ToEntity()
        {
            var entity = new RememberItemEntityModel();
            Mapper(this, entity);
            return entity;
        }
    }
}
