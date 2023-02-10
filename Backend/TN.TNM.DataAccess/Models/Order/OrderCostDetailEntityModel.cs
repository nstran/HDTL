using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Models.Order
{
    public class OrderCostDetailEntityModel : BaseModel<Databases.Entities.OrderCostDetail>
    {
        public Guid OrderCostDetailId { get; set; }
        public Guid? CostId { get; set; }
        public Guid OrderId { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? UnitPrice { get; set; }
        public string CostName { get; set; }
        public string CostCode { get; set; }
        public bool? Active { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsInclude { get; set; }
        public OrderCostDetailEntityModel(Databases.Entities.OrderCostDetail entity)
        {
            Mapper(entity, this);
        }
        public OrderCostDetailEntityModel()
        {

        }
        public override Databases.Entities.OrderCostDetail ToEntity()
        {
            var entity = new Databases.Entities.OrderCostDetail();
            Mapper(this, entity);
            return entity;
        }
    }
}
