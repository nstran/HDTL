using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Models.Vendor
{
    public class VendorOrderProductDetailProductAttributeValueEntityModel : BaseModel<Databases.Entities.VendorOrderProductDetailProductAttributeValue>
    {
        public Guid VendorOrderDetailId { get; set; }
        public Guid ProductId { get; set; }
        public Guid ProductAttributeCategoryId { get; set; }
        public Guid ProductAttributeCategoryValueId { get; set; }
        public Guid OrderProductDetailProductAttributeValueId { get; set; }
        public string NameProductAttributeCategoryValue { get; set; }
        public string NameProductAttributeCategory { get; set; }
        public string ProductAttributeCategoryName { get; set; }
        
        public List<Models.ProductAttributeCategoryValue.ProductAttributeCategoryValueEntityModel> ProductAttributeCategoryValue { get; set; }
        
        public VendorOrderProductDetailProductAttributeValueEntityModel()
        {
            this.ProductAttributeCategoryValue = new List<ProductAttributeCategoryValue.ProductAttributeCategoryValueEntityModel>();
        }

        public VendorOrderProductDetailProductAttributeValueEntityModel(Databases.Entities.VendorOrderProductDetailProductAttributeValue model)
        {
            Mapper(model, this);
        }

        public override Databases.Entities.VendorOrderProductDetailProductAttributeValue ToEntity()
        {
            var entity = new Databases.Entities.VendorOrderProductDetailProductAttributeValue();
            Mapper(this, entity);
            return entity;
        }
    }
}
