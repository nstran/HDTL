using System;

namespace TN.TNM.DataAccess.Models.Order
{
    public class OrderProductDetailProductAttributeValueEntityModel:BaseModel<Databases.Entities.OrderProductDetailProductAttributeValue>
    {
        public Guid OrderDetailId { get; set; }
        public Guid ProductId { get; set; }
        public Guid ProductAttributeCategoryId { get; set; }
        public Guid ProductAttributeCategoryValueId { get; set; }
        public Guid OrderProductDetailProductAttributeValueId { get; set; }

        public string NameProductAttributeCategoryValue { get; set; }
        public string NameProductAttributeCategory {get;set;}
        public OrderProductDetailProductAttributeValueEntityModel()
        {

        }

        public OrderProductDetailProductAttributeValueEntityModel(Databases.Entities.OrderProductDetailProductAttributeValue model)
        {
            Mapper(model, this);
        }

        public override Databases.Entities.OrderProductDetailProductAttributeValue ToEntity()
        {
            var entity = new Databases.Entities.OrderProductDetailProductAttributeValue();
            Mapper(this, entity);
            return entity;
        }
    }
}
