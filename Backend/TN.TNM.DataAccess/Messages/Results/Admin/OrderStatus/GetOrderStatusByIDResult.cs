using TN.TNM.DataAccess.Models.Order;

namespace TN.TNM.DataAccess.Messages.Results.Admin.OrderStatus
{
    public class GetOrderStatusByIDResult : BaseResult
    {
      public OrderStatusEntityModel orderStatus { get; set; }
    }
}
