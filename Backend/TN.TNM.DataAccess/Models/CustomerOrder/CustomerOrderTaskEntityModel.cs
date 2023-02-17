using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.CustomerOrder
{
    public class CustomerOrderTaskEntityModel
    {
        public Guid Id { get; set; }
        public Guid? ServicePacketMappingOptionsId { get; set; }
        public Guid? OrderActionId { get; set; }
        public Guid? ServicePacketId { get; set; }
        public Guid? OptionId { get; set; }
        public string OptionName { get; set; }
        public int? Stt { get; set; }
        public string Path { get; set; }
        public Guid? VendorId { get; set; }
        public string VendorName { get; set; }
        public Guid? EmpId { get; set; }
        public string EmpName { get; set; }
        public List<Guid> ListEmpId { get; set; }
        public string ListEmpName { get; set; }
        public bool? isExtend { get; set; }
        public string Mission { get; set; }
        public string Note { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid UpdatedById { get; set; }
        public DateTime UpdatedDate { get; set; }
        public Guid TenantId { get; set; }
        public bool IsPersonInCharge { get; set; }
    }
}
