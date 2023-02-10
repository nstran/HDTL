using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Cost;

namespace TN.TNM.DataAccess.Messages.Parameters.Quote
{
    public class UpdateCostParameter : BaseParameter
    {
        public CostEntityModel Cost { get; set; }
    }
}
