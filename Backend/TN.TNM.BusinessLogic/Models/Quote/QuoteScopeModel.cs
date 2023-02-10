using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.Quote;

namespace TN.TNM.BusinessLogic.Models.Quote
{
    public class QuoteScopeModel : BaseModel<QuoteScope>
    {
        public Guid ScopeId { get; set; }
        public string Tt { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public Guid? QuoteId { get; set; }
        public Guid? TenantId { get; set; }
        public int Level { get; set; }
        public Guid? ParentId { get; set; }
        public List<QuoteScopeModel> ScopeChildList { get; set; }

        public QuoteScopeModel() { }

        public QuoteScopeModel(QuoteScope entity) : base(entity)
        {
            // Mapper(entity, this);
        }

        public QuoteScopeModel(QuoteScopeEntityModel model)
        {
            Mapper(model, this);
        }
        public override QuoteScope ToEntity()
        {
            var entity = new QuoteScope();
            Mapper(this, entity);
            return entity;
        }
    }
}
