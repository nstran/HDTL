using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.BusinessLogic.Messages.Responses.Vendor
{
    public class SendEmailVendorQuoteResponse : BaseResponse
    {
        public List<string> listInvalidEmail { get; set; }
    }
}
