using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Email;

namespace TN.TNM.DataAccess.Messages.Results.Admin.Email
{
    public class CreateUpdateEmailTemplateMasterdataResult : BaseResult
    {
        public List<CategoryEntityModel> ListEmailType { get; set; }
        public List<EmailTemplateTokenEntityModel> ListToken { get; set; }
        public List<CategoryEntityModel> ListEmailStatus {get;set;}
        public List<EmailTemplateTokenEntityModel> ListEmailTemplateToken { get; set; }
        public EmailTemplateEntityModel EmailTemplateModel { get; set; }        
        public List<string> ListEmailToCC { get; set; }
    }
}
