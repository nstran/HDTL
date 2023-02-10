using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Models.Manufacture
{
    public class MappingOrderTechniqueEntityModel
    {
        public Guid ProductOrderWorkflowId { get; set; }
        public string Name { get; set; }
        public bool? IsDefault { get; set; }

        public Guid? ParentId { get; set; }

        public List<TechniqueRequestEntityModel> ListTechniqueRequest { get; set; }
    }
}
