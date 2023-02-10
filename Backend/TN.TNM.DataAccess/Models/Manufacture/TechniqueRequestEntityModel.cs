using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Manufacture
{
    public class TechniqueRequestEntityModel
    {
        public Guid TechniqueRequestId { get; set; }
        public Guid? ParentId { get; set; }
        public Guid? OrganizationId { get; set; }
        public string TechniqueName { get; set; }
        public string Description { get; set; }
        public short? Rate { get; set; }
        public bool? Active { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public short? TechniqueOrder { get; set; }
        public double? TechniqueValue { get; set; }
        public string TechniqueRequestCode { get; set; }

        public string ParentName { get; set; }
        public string OrganizationName { get; set; }
        public double? CompleteUnitQuantity { get; set; }
        public double? CompleteAreaInDay { get; set; } //Sản lượng hoàn thành trong ngày (Dashboard home)
    }
}
