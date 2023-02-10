using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Manufacture;

namespace TN.TNM.DataAccess.Messages.Parameters.Manufacture
{
    public class CreateAllBTPParameter:BaseParameter
    {
        public Guid ProductionOrderId { get; set; }
        public ProductionOrderMappingEntityModel BTP { get; set; }
        public List<ProductionOrderMappingEntityModel> ListBTP1 { get; set; }
        public List<ProductionOrderMappingEntityModel> ListBTP2 { get; set; }
    }
}
