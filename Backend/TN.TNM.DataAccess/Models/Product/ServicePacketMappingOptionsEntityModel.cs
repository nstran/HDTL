using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Product
{
    public class ServicePacketMappingOptionsEntityModel 
    {
        public Guid? Id { get; set; }
        public Guid? CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }
        public Guid? ParentId { get; set; }
        public string Name { get; set; }
        //Dành cho option
        public string NameCustom { get; set; }
        public decimal? Vat { get; set; }
        public decimal? Price { get; set; }
        public Guid? OptionId { get; set; }
        public Guid? ServicePacketId { get; set; }
        public int? SortOrder { get; set; }
        public string CategoryUnitName { get; set; }
        public string Description { get; set; }

    }
}
