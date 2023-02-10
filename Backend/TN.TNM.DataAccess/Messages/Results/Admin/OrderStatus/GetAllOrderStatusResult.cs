using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Order;

namespace TN.TNM.DataAccess.Messages.Results.Admin.OrderStatus
{
    public class GetAllOrderStatusResult: BaseResult
    {
      public List<OrderStatusEntityModel> listOrderStatus { get; set; }
    }
}
