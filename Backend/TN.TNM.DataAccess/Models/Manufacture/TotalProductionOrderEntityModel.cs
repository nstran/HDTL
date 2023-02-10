using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Manufacture
{
    public class TotalProductionOrderEntityModel
    {
        public Guid TotalProductionOrderId { get; set; }
        public string Code { get; set; }
        public Guid? PeriodId { get; set; }
        public DateTime? StartDate { get; set; }
        public Guid StatusId { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? UpdatedById { get; set; }

        public string StatusName { get; set; }
        public double? TotalQuantity { get; set; }
        public double? TotalArea { get; set; }
        public DateTime? MaxEndDate { get; set; }
        public DateTime? MinEndDate { get; set; }
    }
}
