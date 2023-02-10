using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Manufacture;

namespace TN.TNM.DataAccess.Messages.Parameters.Manufacture
{
    public class UpdateTotalProductionOrderParameter : BaseParameter
    {
        public TotalProductionOrderEntityModel TotalProductionOrder { get; set; }
        public List<Guid> ListProductionOrderId { get; set; }
        public string OldStatusCodeFe { get; set; }
    }
}
