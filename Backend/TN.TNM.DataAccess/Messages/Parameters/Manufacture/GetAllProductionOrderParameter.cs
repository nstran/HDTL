using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Parameters.Manufacture
{
    public class GetAllProductionOrderParameter : BaseParameter
    {
        public string ProductionOrderCode { get; set; }
        public string CustomerName { get; set; }
        public string TotalProductionOrderCode { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public List<Guid> ListStatusId { get; set; }
        public string TechniqueDescription { get; set; }
        public bool? Type { get; set; }
        public List<Guid> ListOrgan { get; set; }
        public bool? IsError { get; set; }
    }
}
