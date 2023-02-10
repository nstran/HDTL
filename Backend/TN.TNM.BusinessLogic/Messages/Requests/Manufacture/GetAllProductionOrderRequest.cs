using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Manufacture;

namespace TN.TNM.BusinessLogic.Messages.Requests.Manufacture
{
    public class GetAllProductionOrderRequest:BaseRequest<GetAllProductionOrderParameter>
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

        public override GetAllProductionOrderParameter ToParameter()
        {
            return new GetAllProductionOrderParameter()
            {
                ProductionOrderCode = ProductionOrderCode,
                CustomerName = CustomerName,
                TotalProductionOrderCode = TotalProductionOrderCode,
                StartDate = StartDate,
                EndDate = EndDate,
                ListStatusId = ListStatusId,
                TechniqueDescription = TechniqueDescription,
                Type = Type,
                ListOrgan = ListOrgan,
                IsError = IsError
            };
        }
    }
}
