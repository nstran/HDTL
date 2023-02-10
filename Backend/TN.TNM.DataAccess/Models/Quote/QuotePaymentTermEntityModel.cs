using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Quote
{
    public class QuotePaymentTermEntityModel : BaseModel<Databases.Entities.QuotePaymentTerm>
    {
        public Guid PaymentTermId { get; set; }
        public Guid QuoteId { get; set; }
        public string OrderNumber { get; set; }
        public string Milestone { get; set; }
        public string PaymentPercentage { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? CreatedById { get; set; }
        public QuotePaymentTermEntityModel() { }

        public QuotePaymentTermEntityModel(Databases.Entities.QuotePaymentTerm model)
        {
            Mapper(model, this);
        }

        public override Databases.Entities.QuotePaymentTerm ToEntity()
        {
            var entity = new Databases.Entities.QuotePaymentTerm();
            Mapper(this, entity);
            return entity;
        }
    }
}
