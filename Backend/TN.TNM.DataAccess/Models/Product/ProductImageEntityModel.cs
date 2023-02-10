using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Product
{
    public class ProductImageEntityModel : BaseModel<Databases.Entities.ProductImage>
    {
        public Guid? ProductImageId { get; set; }
        public Guid? ProductId { get; set; }
        public string ImageName { get; set; }
        public string ImageSize { get; set; }
        public string ImageUrl { get; set; }
        public bool? Active { get; set; }
        public Guid? CreatedById { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }

        public ProductImageEntityModel()
        {
        }

        public ProductImageEntityModel(Databases.Entities.ProductImage entity)
        {
            Mapper(entity, this);
        }

        public override Databases.Entities.ProductImage ToEntity()
        {
            var entity = new Databases.Entities.ProductImage();
            Mapper(this, entity);
            return entity;
        }
    }
}
