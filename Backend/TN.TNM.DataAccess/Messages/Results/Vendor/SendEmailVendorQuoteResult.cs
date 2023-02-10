using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Results.Vendor
{
    public class SendEmailVendorQuoteResult : BaseResult
    {
        public List<string> listInvalidEmail { get; set; }
    }
}
