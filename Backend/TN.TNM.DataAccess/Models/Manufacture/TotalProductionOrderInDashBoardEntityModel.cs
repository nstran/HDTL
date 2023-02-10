using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Manufacture
{
    public class TotalProductionOrderInDashBoardEntityModel
    {
        public List<TotalQuantityByTechniqueRequestModel> ListTotalQuantityByTechniqueRequest { get; set; }

        public Guid? StatusId { get; set; }
        public string StatusName { get; set; }
        public Guid? ProductionOrderId { get; set; }
        public string ProductionOrderCode { get; set; }
        public string CustomerName { get; set; }
        public DateTime? EndDate { get; set; }
        public string StatusCode { get; set; }

        public TotalProductionOrderInDashBoardEntityModel()
        {
            this.ListTotalQuantityByTechniqueRequest = new List<TotalQuantityByTechniqueRequestModel>();
        }
    }

    public class TotalQuantityByTechniqueRequestModel
    {
       public  Guid? TechniqueRequestId { get; set; }
       public double? TotalQuantityCompleted { get; set; }
       public double? TotalQuantityHaveToComplete { get; set; }
    }
}
