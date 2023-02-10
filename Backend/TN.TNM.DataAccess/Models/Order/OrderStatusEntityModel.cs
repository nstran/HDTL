using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Models.Order
{
    public class OrderStatusEntityModel: BaseModel<OrderStatus>
    {
        public Guid OrderStatusId { get; set; }
        public string OrderStatusCode { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public OrderStatusEntityModel()
        {

        }

        public OrderStatusEntityModel(OrderStatus entity)
        {
            Mapper(entity, this);
        }

        public override OrderStatus ToEntity()
        {
            var entity = new OrderStatus();
            Mapper(this, entity);
            return entity;
        }
    }
}
