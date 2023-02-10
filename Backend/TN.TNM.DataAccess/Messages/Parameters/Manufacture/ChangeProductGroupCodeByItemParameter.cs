using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Manufacture
{
    public class ChangeProductGroupCodeByItemParameter : BaseParameter
    {
        public Guid ProductionOrderMappingId { get; set; }
        public string ProductGroupCode { get; set; }
    }
}
