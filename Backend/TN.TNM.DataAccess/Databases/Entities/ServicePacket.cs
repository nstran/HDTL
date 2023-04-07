using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class ServicePacket
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string AttributeName { get; set; }
        public string Message { get; set; }
        public string Status { get; set; }
        public Guid? ProductCategoryId { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid UpdatedById { get; set; }
        public DateTime UpdatedDate { get; set; }
        public Guid TenantId { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
        public int? Stt { get; set; }
        public Guid? SubjectsApplicationId { get; set; }
        public Guid? RoleId { get; set; }
        public Guid[] ProvinceIds { get; set; }
    }
}
