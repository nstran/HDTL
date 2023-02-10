using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Email;

namespace TN.TNM.DataAccess.Messages.Results.Admin.Email
{
    public class SearchEmailTemplateResult:BaseResult
    {
        public List<EmailTemplateEntityModel> ListEmailTemplate { get; set; }
    }
}
