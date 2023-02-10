using System;
using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Quote;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Messages.Parameters.Quote;
using TN.TNM.DataAccess.Models.Promotion;
using TN.TNM.DataAccess.Models.Quote;

namespace TN.TNM.BusinessLogic.Messages.Requests.Quote
{
    public class CreateQuoteRequest : BaseRequest<CreateQuoteParameter>
    {
        public QuoteModel Quote { get; set; }
        public List<QuoteDetailModel> QuoteDetail { get; set; }
        public List<QuoteCostDetailModel> QuoteCostDetail { get; set; }
        public int TypeAccount { get; set; }
        public List<QuoteDocumentModel> FileList { get; set; }
        public List<AdditionalInformationModel> ListAdditionalInformation { get; set; }
        public bool isClone { get; set; }
        public Guid QuoteIdClone { get; set; }
        public List<Guid> ListParticipant { get; set; }
        public List<PromotionObjectApplyEntityModel> ListPromotionObjectApply { get; set; }
        public List<QuotePlanModel> ListQuotePlan { get; set; }
        public List<QuoteScopeModel> ListQuoteScope { get; set; }

        public List<QuotePaymentTermModel> ListQuotePaymentTerm { get; set; }

        public override CreateQuoteParameter ToParameter()
        {
            List<QuoteDetail> ListQuoteDetail = new List<QuoteDetail>();
            QuoteDetail?.ForEach(item =>
            {
                var quoteDetailObject = new QuoteDetail();
                quoteDetailObject = item.ToEntity();
                List<QuoteProductDetailProductAttributeValue> QuoteProductDetailProductAttributeValueList = new List<QuoteProductDetailProductAttributeValue>();
                if (item.QuoteProductDetailProductAttributeValue != null)
                {
                    item.QuoteProductDetailProductAttributeValue.ForEach(itemX =>
                    {
                        QuoteProductDetailProductAttributeValueList.Add(itemX.ToEntity());
                    });
                    quoteDetailObject.QuoteProductDetailProductAttributeValue = QuoteProductDetailProductAttributeValueList;
                }
                ListQuoteDetail.Add(quoteDetailObject);
            });

            List<QuoteCostDetail> ListQuoteCostDetail = new List<QuoteCostDetail>();
            QuoteCostDetail?.ForEach(item =>
            {
                var quoteDetailObject = new QuoteCostDetail();
                quoteDetailObject = item.ToEntity();
                ListQuoteCostDetail.Add(quoteDetailObject);
            });

            List<QuotePlan> ListQuotePlanEntity = new List<QuotePlan>();
            ListQuotePlan?.ForEach(item =>
            {
                var quotePlan = new QuotePlan();
                quotePlan = item.ToEntity();
                ListQuotePlanEntity.Add(quotePlan);
            });
            
            
            List<QuoteScope> ListQuoteScopeEntity = new List<QuoteScope>();
            ListQuoteScope?.ForEach(item =>
            {
                var quoteScope = new QuoteScope();
                quoteScope = item.ToEntity();
                ListQuoteScopeEntity.Add(quoteScope);
            });


            List<QuoteDocument> ListQuoteDocument = new List<QuoteDocument>();
            if (FileList != null)
            {
                FileList.ForEach(item =>
                {
                    ListQuoteDocument.Add(item.ToEntity());
                });
            }

            List<AdditionalInformation> newListAdditionalInformation = new List<AdditionalInformation>();
            if (ListAdditionalInformation != null)
            {
                ListAdditionalInformation.ForEach(item =>
                {
                    newListAdditionalInformation.Add(item.ToEntity());
                });
            }

            List<QuotePaymentTerm> quotePaymentTerms = new List<QuotePaymentTerm>();
            if (ListQuotePaymentTerm != null)
                ListQuotePaymentTerm.ForEach(item =>
                {
                    quotePaymentTerms.Add(item.ToEntity());
                });

            return new CreateQuoteParameter
            {
                //Quote = Quote.ToEntity(),
                //QuoteDetail = ListQuoteDetail,
                //QuoteCostDetail = ListQuoteCostDetail,
                //TypeAccount = TypeAccount,
                //QuoteDocument = ListQuoteDocument,
                //ListAdditionalInformation = newListAdditionalInformation,
                isClone = isClone,
                QuoteIdClone = QuoteIdClone,
                UserId = this.UserId,
                ListParticipant = ListParticipant,
                ListPromotionObjectApply = ListPromotionObjectApply,
                //QuotePlans = ListQuotePlanEntity,
                //QuoteScopes = ListQuoteScopeEntity,
                //ListQuotePaymentTerm = quotePaymentTerms,
            };

        }
    }
}
