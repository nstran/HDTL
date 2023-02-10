using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.Promotion;
using TN.TNM.DataAccess.Models.Quote;

namespace TN.TNM.DataAccess.Messages.Parameters.Quote
{
    public class CreateQuoteParameter:BaseParameter
    {
        public QuoteEntityModel Quote { get; set; }
        public List<QuoteDetailEntityModel> QuoteDetail { get; set; }
        public List<QuoteCostDetailEntityModel> QuoteCostDetail { get; set; }
        public int TypeAccount { get; set; }
        public List<QuoteDocumentEntityModel> QuoteDocument { get; set; }
        public List<AdditionalInformationEntityModel> ListAdditionalInformation { get; set; }
        public bool isClone { get; set; }
        public Guid QuoteIdClone { get; set; }
        public List<Guid> ListParticipant { get; set; }
        public List<PromotionObjectApplyEntityModel> ListPromotionObjectApply { get; set; }
        public List<QuotePlanEntityModel> QuotePlans { get; set; }
        public List<QuoteScopeEntityModel> QuoteScopes { get; set; }

        public List<QuotePaymentTermEntityModel> ListQuotePaymentTerm { get; set; }
    }
}
