using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Models.Email
{
    public class EmailTemplateTokenEntityModel : BaseModel<EmailTemplateToken>
    {
        public Guid EmailTemplateTokenId { get; set; }
        public string TokenLabel { get; set; }
        public string TokenCode { get; set; }
        public Guid? EmailTemplateTypeId { get; set; }
        public bool Active { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }

        public override EmailTemplateToken ToEntity()
        {
            //Code tien xu ly model truoc khi day vao DB
            var entity = new EmailTemplateToken();
            Mapper(this, entity);
            return entity;
        }

        public EmailTemplateTokenEntityModel(EmailTemplateToken emailTemplateToken)
        {
            Mapper(emailTemplateToken, this);
        }

        public EmailTemplateTokenEntityModel()
        {

        }

    }
}
