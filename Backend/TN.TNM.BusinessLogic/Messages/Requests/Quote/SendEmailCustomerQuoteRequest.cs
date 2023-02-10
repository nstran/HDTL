using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using TN.TNM.DataAccess.Messages.Parameters.Quote;
using TN.TNM.DataAccess.Models.Folder;

namespace TN.TNM.BusinessLogic.Messages.Requests.Quote
{
    public class SendEmailCustomerQuoteRequest : BaseRequest<SendEmailCustomerQuoteParameter>
    {
        public List<string> ListEmail { get; set; }

        public List<string> ListEmailCC { get; set; }

        public List<string> ListEmailBcc { get; set; }

        public string TitleEmail { get; set; }
        public string ContentEmail { get; set; }
        public Guid QuoteId { get; set; }
        public List<IFormFile> ListFormFile { get; set; }

        public override SendEmailCustomerQuoteParameter ToParameter()
        {
            return new SendEmailCustomerQuoteParameter
            {
                ListEmail = this.ListEmail,
                ListEmailCC = this.ListEmailCC,
                ListEmailBcc = this.ListEmailBcc,
                TitleEmail = this.TitleEmail,
                ContentEmail = this.ContentEmail,
                QuoteId = this.QuoteId,
                ListFormFile = ListFormFile,
                UserId = this.UserId
            };
        }
    }
}
