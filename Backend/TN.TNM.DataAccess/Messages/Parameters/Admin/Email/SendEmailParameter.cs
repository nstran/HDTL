using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Email;

namespace TN.TNM.DataAccess.Messages.Parameters.Admin.Email
{
    public class SendEmailParameter:BaseParameter
    {
        public int SendType { get; set; } 
        public SendEmailEntityModel SendEmailEntityModel { get; set; }

        public SendEmailParameter(SendEmailEntityModel entity)
        {
            var listQuoteDetailToSendEmail = new List<QuoteDetailToSendEmailModel>();
            var newSendEmailEntityModel = entity;

            entity?.ListQuoteDetailToSendEmail?.ForEach(quoteDetail =>
            {
                listQuoteDetailToSendEmail.Add(new QuoteDetailToSendEmailModel
                {
                    DiscountValue = quoteDetail.DiscountValue,
                    ProductName = quoteDetail.ProductName,
                    ProductNameUnit = quoteDetail.ProductNameUnit,
                    Quantity = quoteDetail.Quantity,
                    SumAmount = quoteDetail.SumAmount,
                    UnitPrice = quoteDetail.UnitPrice,
                    Vat = quoteDetail.Vat
                });
            });

            if (listQuoteDetailToSendEmail.Count > 0)
            {
                newSendEmailEntityModel.ListQuoteDetailToSendEmail = listQuoteDetailToSendEmail;
            }

            this.SendEmailEntityModel = newSendEmailEntityModel;
        }
        public SendEmailParameter()
        {

        }
    }
}
