using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.Email;

namespace TN.TNM.DataAccess.Messages.Results.Admin.Email
{
    public class GetTokenForEmailTypeIdResult : BaseResult
    {
        public List<EmailTemplateTokenEntityModel> ListEmailTemplateToken { get; set; }
    }
}
