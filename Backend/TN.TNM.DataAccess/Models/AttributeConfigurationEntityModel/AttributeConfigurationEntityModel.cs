using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.AttributeConfigurationEntityModel
{
    public class AttributeConfigurationEntityModel
    {
        public Guid? Id { get; set; }
        public int? DataType { get; set; }
        public string DataTypeName { get; set; }
        public Guid? CategoryId { get; set; }
        public string CategoryName { get; set; }
        public Guid? CreatedById { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }
        public decimal? ObjectType { get; set; }
        public Guid? ObjectId { get; set; }
        public string Value { get; set; }

    }
}
