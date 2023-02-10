using System;

namespace TN.TNM.DataAccess.Models.ProductAttributeCategoryValue
{
    public class ProductAttributeCategoryValueEntityModel : BaseModel<Databases.Entities.ProductAttributeCategoryValue>
    {
        public Guid ProductAttributeCategoryValueId { get; set; }
        public string ProductAttributeCategoryValue1 { get; set; }
        public Guid ProductAttributeCategoryId { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool Active { get; set; }

        public ProductAttributeCategoryValueEntityModel()
        {
            
        }

        public ProductAttributeCategoryValueEntityModel(Databases.Entities.ProductAttributeCategoryValue model)
        {
            Mapper(model, this);
        }

        public override Databases.Entities.ProductAttributeCategoryValue ToEntity()
        {
            var entity = new Databases.Entities.ProductAttributeCategoryValue();
            Mapper(this, entity);
            return entity;
        }
    }
}
