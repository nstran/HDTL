using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Manufacture;

namespace TN.TNM.BusinessLogic.Models.Manufacture
{
    public class TotalProductionOrderModel : BaseModel<TotalProductionOrderEntityModel>
    {
        public Guid TotalProductionOrderId { get; set; }
        public string Code { get; set; }
        public Guid? PeriodId { get; set; }
        public DateTime? StartDate { get; set; }
        public Guid StatusId { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? UpdatedById { get; set; }

        public string StatusName { get; set; }
        public double? TotalQuantity { get; set; }
        public double? TotalArea { get; set; }
        public DateTime? MaxEndDate { get; set; }
        public DateTime? MinEndDate { get; set; }

        public TotalProductionOrderModel() { }

        public TotalProductionOrderModel(TotalProductionOrderEntityModel model)
        {
            Mapper(model, this);
        }

        public override TotalProductionOrderEntityModel ToEntity()
        {
            var entity = new TotalProductionOrderEntityModel();
            Mapper(this, entity);
            return entity;
        }
    }
}
