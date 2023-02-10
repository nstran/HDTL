using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Manufacture
{
    public class SearchTechniqueRequestParameter : BaseParameter
    {
        public string TechniqueName { get; set; }
        public Guid? OrganizationId { get; set; }
        public Guid? ParentId { get; set; }
        public bool? IsDefault { get; set; }
        public string Description { get; set; }
    }
}
