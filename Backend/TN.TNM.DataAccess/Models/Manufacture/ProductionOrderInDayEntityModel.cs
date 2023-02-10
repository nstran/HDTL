using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Manufacture
{
    public class ProductionOrderInDayEntityModel
    {
        public Guid ProductionOrderId { get; set; }
        public string ProductionOrderCode { get; set; }
        public string CustomerName { get; set; }
        public double CompleteQuantity { get; set; }
        public double TotalQuantity { get; set; }
        public string TechniqueRequestCode { get; set; }
        public bool IsShow { get; set; }
        public double TotalArea { get; set; }
        public double TotalComplete { get; set; }
    }
}
