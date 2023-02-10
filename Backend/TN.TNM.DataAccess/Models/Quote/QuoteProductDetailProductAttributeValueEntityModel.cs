using System;

namespace TN.TNM.DataAccess.Models.Quote
{
    public class QuoteProductDetailProductAttributeValueEntityModel:BaseModel<DataAccess.Databases.Entities.QuoteProductDetailProductAttributeValue>
    {
        public Guid QuoteDetailId { get; set; }
        public Guid ProductId { get; set; }
        public Guid ProductAttributeCategoryId { get; set; }
        public Guid ProductAttributeCategoryValueId { get; set; }
        public Guid QuoteProductDetailProductAttributeValueId { get; set; }

        public string NameProductAttributeCategoryValue { get; set; }
        public string NameProductAttributeCategory { get; set; }
        public QuoteProductDetailProductAttributeValueEntityModel() { }
        public QuoteProductDetailProductAttributeValueEntityModel(DataAccess.Databases.Entities.QuoteProductDetailProductAttributeValue model)
        {
            Mapper(model, this);
        }
        public override DataAccess.Databases.Entities.QuoteProductDetailProductAttributeValue ToEntity()
        {
            var entity = new DataAccess.Databases.Entities.QuoteProductDetailProductAttributeValue();
            Mapper(this, entity);
            return entity;
        }

    }
}
