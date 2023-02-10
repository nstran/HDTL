using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Manufacture;
using TN.TNM.DataAccess.Models.Order;

namespace TN.TNM.DataAccess.Messages.Results.Manufacture
{
    public class GetMasterDataCreateProductionOrderResult : BaseResult
    {
        public OrderGetByIdEntityModel Order { get; set; }
        public List<OrderDetailEntityModel> ListOrderDetail { get; set; }
        public List<MappingOrderTechniqueEntityModel> ListMappingOrder { get; set; }
        public List<TechniqueRequestEntityModel> ListTechniqueRequest { get; set; }
    }
}
