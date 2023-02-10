using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Quote
{
    public class QuoteScopeEntityModel : BaseModel<Databases.Entities.QuoteScope>
    {
        public Guid ScopeId { get; set; }
        public string Tt { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public Guid? QuoteId { get; set; }
        public Guid? TenantId { get; set; }
        public int? Level { get; set; }
        public Guid? ParentId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? CreatedById { get; set; }
        public List<QuoteScopeEntityModel> ScopeChildList { get; set; }
        public QuoteScopeEntityModel() { }

        public QuoteScopeEntityModel(Databases.Entities.QuoteScope model)
        {
            Mapper(model, this);
        }

        public override Databases.Entities.QuoteScope ToEntity()
        {
            var entity = new Databases.Entities.QuoteScope();
            Mapper(this, entity);
            return entity;
        }
    }
}
