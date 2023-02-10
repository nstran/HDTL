using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.Admin.OrderStatus;
using TN.TNM.DataAccess.Messages.Results.Admin.OrderStatus;
using TN.TNM.DataAccess.Models.Order;

namespace TN.TNM.DataAccess.Databases.DAO
{
    public class OrderStatusDAO : BaseDAO, IOrderStatusDataAccess
    {
        public OrderStatusDAO(Databases.TNTN8Context _content, IAuditTraceDataAccess _iAuditTrace)
        {
            this.context = _content;
            this.iAuditTrace = _iAuditTrace;
        }

        public GetAllOrderStatusResult GetAllOrderStatus(GetAllOrderStatusParameter parameter)
        {
            try
            {
                var listOrderStatus = context.OrderStatus.ToList();
                var list = new List<OrderStatusEntityModel>();
                listOrderStatus.ForEach(item =>
                {
                    var _item = new OrderStatusEntityModel(item);
                    list.Add(_item);
                });

                return new GetAllOrderStatusResult
                {
                    listOrderStatus = list,
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success",
                };
            }
            catch (Exception ex)
            {
                return new GetAllOrderStatusResult
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = ex.Message,
                };
            }
        }

        public GetOrderStatusByIDResult GetOrderStatusByID(GetOrderStatusByIDParameter parameter)
        {
            try
            {
                var orderStatusobject =
                    context.OrderStatus.FirstOrDefault(item => item.OrderStatusId == parameter.OderStatusId);

                if (orderStatusobject == null)
                {
                    return new GetOrderStatusByIDResult()
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = "Trạng thái không tồn tại trên hệ thống",
                    };
                }

                return new GetOrderStatusByIDResult
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success",
                    orderStatus = new OrderStatusEntityModel(orderStatusobject),
                };
            }
            catch (Exception ex)
            {
                return new GetOrderStatusByIDResult
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = ex.Message,
                };
            }
        }

    }
}
