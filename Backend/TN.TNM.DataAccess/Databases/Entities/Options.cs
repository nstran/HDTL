using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class Options
    {
        public Guid Id { get; set; }
        public Guid CategoryId { get; set; }
        public Guid? CategoryUnitId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Note { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid UpdatedById { get; set; }
        public DateTime UpdatedDate { get; set; }
        public Guid TenantId { get; set; }
        public decimal? Price { get; set; }
        public Guid? ParentId { get; set; }
        public decimal? Vat { get; set; }
    }
}
