using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TN.TNM.BusinessLogic.Interfaces.Admin.OrderStatus;
using TN.TNM.BusinessLogic.Messages.Requests.Admin.OrderStatus;
using TN.TNM.BusinessLogic.Messages.Responses.Admin.OrderStatus;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.Admin.OrderStatus;
using TN.TNM.DataAccess.Messages.Results.Admin.OrderStatus;

namespace TN.TNM.Api.Controllers
{
    public class OrderStatusController : Controller
    {
        private readonly IOrderStatusDataAccess iOrderStatusDataAccess;
        public OrderStatusController(IOrderStatusDataAccess _iOrderStatusDataAccess)
        {
            this.iOrderStatusDataAccess = _iOrderStatusDataAccess;
        }

        [HttpPost]
        [Route("api/orderstatus/getAllOrderStatus")]
        [Authorize(Policy = "Member")]
        public GetAllOrderStatusResult GetAllOrderStatus([FromBody]GetAllOrderStatusParameter request)
        {
            return this.iOrderStatusDataAccess.GetAllOrderStatus(request);
        }

        [HttpPost]
        [Route("api/orderstatus/getOrderStatusByID")]
        [Authorize(Policy = "Member")]
        public GetOrderStatusByIDResult GetOrderStatusByID([FromBody] GetOrderStatusByIDParameter request)
        {
            return this.iOrderStatusDataAccess.GetOrderStatusByID(request);
        }


    }
}