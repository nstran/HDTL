using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Manufacture
{
    public class UpdateProductionOrderEspeciallyParameter : BaseParameter
    {
        public Guid ProductionOrderId { get; set; }
        public bool Especially { get; set; }
    }
}
