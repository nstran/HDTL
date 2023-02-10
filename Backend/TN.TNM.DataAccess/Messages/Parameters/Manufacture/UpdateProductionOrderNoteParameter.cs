using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Manufacture
{
    public class UpdateProductionOrderNoteParameter : BaseParameter
    {
        public Guid ProductionOrderId { get; set; }
        public string Note { get; set; }
    }
}
