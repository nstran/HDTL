using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Quote
{
    public class QuotePlanEntityModel : BaseModel<Databases.Entities.QuotePlan>
    {
        public Guid PlanId { get; set; }
        public int Tt { get; set; }
        public string Finished { get; set; }
        public string ExecTime { get; set; }
        public string SumExecTime { get; set; }
        public Guid? QuoteId { get; set; }
        public Guid? TenantId { get; set; }
        public QuotePlanEntityModel() { }

        public QuotePlanEntityModel(Databases.Entities.QuotePlan model)
        {
            Mapper(model, this);
        }

        public override Databases.Entities.QuotePlan ToEntity()
        {
            var entity = new Databases.Entities.QuotePlan();
            Mapper(this, entity);
            return entity;
        }
    }
}
