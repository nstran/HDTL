using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Vendor
{
    public class SuggestedSupplierQuotesDetailEntityModel : BaseModel<Databases.Entities.SuggestedSupplierQuotesDetail>
    {
        public Guid SuggestedSupplierQuoteDetailId { get; set; }
        public Guid SuggestedSupplierQuoteId { get; set; }
        public Guid? ProductId { get; set; }
        public decimal? Quantity { get; set; }
        public string Note { get; set; }
        public bool Active { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? UpdatedById { get; set; }

        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string ProductUnitName { get; set; }

        public SuggestedSupplierQuotesDetailEntityModel()
        {

        }

        public SuggestedSupplierQuotesDetailEntityModel(Databases.Entities.SuggestedSupplierQuotesDetail entity)
        {
            Mapper(entity, this);
        }

        public override Databases.Entities.SuggestedSupplierQuotesDetail ToEntity()
        {
            var entity = new Databases.Entities.SuggestedSupplierQuotesDetail();
            Mapper(this, entity);
            return entity;
        }
    }
}
