using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.Quote;

namespace TN.TNM.BusinessLogic.Models.Quote
{
    public class QuotePlanModel : BaseModel<QuotePlan>
    {
        public Guid PlanId { get; set; }
        public int Tt { get; set; }
        public string Finished { get; set; }
        public string ExecTime { get; set; }
        public string SumExecTime { get; set; }
        public Guid QuoteId { get; set; }
        public Guid? TenantId { get; set; }

        public QuotePlanModel() { }

        public QuotePlanModel(QuotePlan entity) : base(entity)
        {
            // Mapper(entity, this);
        }

        public QuotePlanModel(QuotePlanEntityModel model)
        {
            Mapper(model, this);
        }
        public override QuotePlan ToEntity()
        {
            var entity = new QuotePlan();
            Mapper(this, entity);
            return entity;
        }
    }
}
