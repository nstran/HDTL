using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.Quote;

namespace TN.TNM.BusinessLogic.Models.Quote
{
    public class QuotePaymentTermModel : BaseModel<QuotePaymentTerm>
    {
        public Guid PaymentTermId { get; set; }
        public Guid QuoteId { get; set; }
        public string OrderNumber { get; set; }
        public string Milestone { get; set; }
        public string PaymentPercentage { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? CreatedById { get; set; }

        public QuotePaymentTermModel() { }

        public QuotePaymentTermModel(QuotePaymentTerm entity) : base(entity)
        {
            // Mapper(entity, this);
        }

        public QuotePaymentTermModel(QuotePaymentTermEntityModel model)
        {
            Mapper(model, this);
        }

        public override QuotePaymentTerm ToEntity()
        {
            var entity = new QuotePaymentTerm();
            Mapper(this, entity);
            return entity;
        }
    }
}
