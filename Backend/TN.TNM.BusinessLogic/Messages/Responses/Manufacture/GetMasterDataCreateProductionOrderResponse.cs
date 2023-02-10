using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Manufacture;
using TN.TNM.BusinessLogic.Models.Order;

namespace TN.TNM.BusinessLogic.Messages.Responses.Manufacture
{
    public class GetMasterDataCreateProductionOrderResponse : BaseResponse
    {
        public CustomerOrderModel CustomerOrderObject { get; set; }
        public List<CustomerOrderDetailModel> ListCustomerOrderDetail { get; set; }
        public List<MappingOrderTechniqueModel> ListMappingOrder { get; set; }
        public List<TechniqueRequestModel> ListTechniqueRequest { get; set; }
    }
}
