using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace TN.TNM.DataAccess.Messages.Parameters.Vendor
{
    public class SendMailVendorQuoteParameter : BaseParameter
    {
        public List<string> ListEmail { get; set; }

        public List<string> ListEmailCC { get; set; }

        public List<string> ListEmailBcc { get; set; }

        public string TitleEmail { get; set; }
        public string ContentEmail { get; set; }
        public Guid SuggestedSupplierQuoteId { get; set; }
        
        //public string Base64Pdf { get; set; }

        public List<IFormFile> ListFormFile { get; set; }
    }
}
