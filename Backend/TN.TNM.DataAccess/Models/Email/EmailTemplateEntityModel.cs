using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Models.Email
{
    public class EmailTemplateEntityModel:BaseModel<EmailTemplate>
    {
        public Guid EmailTemplateId { get; set; }

        public string EmailTemplateName { get; set; }

        public string EmailTemplateTitle { get; set; }

        public string EmailTemplateContent { get; set; }

        public Guid? EmailTemplateTypeId { get; set; }

        public Guid EmailTemplateStatusId { get; set; }
        
        public string EmailTemplateStatusCode { get; set; }

        public bool Active { get; set; }

        public Guid CreatedById { get; set; }

        public DateTime CreatedDate { get; set; }

        public Guid? UpdatedById { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public bool? IsAutomatic { get; set; }
        
        public Guid? TenantId { get; set; }

        public override EmailTemplate ToEntity()
        {
            //Code tien xu ly model truoc khi day vao DB
            var entity = new EmailTemplate();
            Mapper(this, entity);
            return entity;
        }

        public EmailTemplateEntityModel(EmailTemplate emailTemplateToken)
        {
            Mapper(emailTemplateToken, this);
        }

        public EmailTemplateEntityModel()
        {

        }
    }
}
