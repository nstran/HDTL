using System;
using System.Linq;
using TN.TNM.DataAccess.Databases;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.Common;

namespace TN.TNM.DataAccess.Helper
{
    class LogHelper
    {

        /// <summary>
        /// Log lại hành động đăng nhập của một user và trạng thái kết quả của hàng động - thành công hay thất bại
        /// </summary>
        /// <param name="context"></param>
        /// <param name="userName"> Tên đăng nhập </param>
        /// <param name="status"> trạng thái đăng nhập (thành công - 1, Thất bại - 0) </param>
        public static void LoginAuditTrace(TNTN8Context context, string userName, int status)
        {
            var user = context.User.FirstOrDefault(x => x.UserName == userName);

            if (user != null)
            {
                LoginAuditTrace loginAudit = new LoginAuditTrace();

                loginAudit.LoginAuditTraceId = Guid.NewGuid();
                loginAudit.UserId = user.UserId;
                loginAudit.LoginDate = DateTime.Now;
                loginAudit.Status = status;

                context.LoginAuditTrace.Add(loginAudit);
                context.SaveChanges();
            }
        }


        /// <summary>
        /// Log các hành động đối với một đối tượng 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="actionName"> Tên hành động </param>
        /// <param name="objectName"> Tên đối tượng </param>
        /// <param name="objectId"> id của đối tượng </param>
        /// <param name="createById"> id của người thực hiện hành động </param>
        public static void AuditTrace(TNTN8Context context, string actionName, string objectName, Guid objectId, Guid createById)
        {

            var listScreen = context.Screen.ToList();
            AuditTrace trace = new AuditTrace();

            // Tạo khách hàng tiềm năng
            if (actionName == ActionName.Create && objectName == ObjectName.POTENTIALCUSTOMER)
            {
                var screen = listScreen.FirstOrDefault(x => x.ScreenCode == "POTENTIAL_CUSTOMER");
                if (screen != null)
                {
                    trace.ScreenId = screen.ScreenId;
                }
                trace.Description = "Tạo mới khách hàng tiềm năng";
            }
            // chi tiết khách hàng tiềm năng - chỉnh sửa
            else if (actionName == ActionName.UPDATE && objectName == ObjectName.POTENTIALCUSTOMER)
            {
                var screen = listScreen.FirstOrDefault(x => x.ScreenCode == "POTENTIAL_CUSTOMER_DETAIL");
                if (screen != null)
                {
                    trace.ScreenId = screen.ScreenId;
                }
                trace.Description = "Chỉnh sửa khách hàng tiềm năng";
            }
            // chi tiết khách hàng tiềm năng - xóa
            else if (actionName == ActionName.DELETE && objectName == ObjectName.POTENTIALCUSTOMER)
            {
                var screen = listScreen.FirstOrDefault(x => x.ScreenCode == "POTENTIAL_CUSTOMER_DETAIL");
                if (screen != null)
                {
                    trace.ScreenId = screen.ScreenId;
                }
                trace.Description = "Xóa khách hàng tiềm năng";
            }
            // Tạo khách hàng
            else if (actionName == ActionName.Create && objectName == ObjectName.CUSTOMER)
            {
                var screen = listScreen.FirstOrDefault(x => x.ScreenCode == "CUSTOMER");
                if (screen != null)
                {
                    trace.ScreenId = screen.ScreenId;
                }
                trace.Description = "Tạo mới khách hàng";
            }
            // chi tiết khách hàng - chỉnh sửa
            else if (actionName == ActionName.UPDATE && objectName == ObjectName.CUSTOMER)
            {
                var screen = listScreen.FirstOrDefault(x => x.ScreenCode == "CUSTOMER_DETAIL");
                if (screen != null)
                {
                    trace.ScreenId = screen.ScreenId;
                }
                trace.Description = "Chỉnh sửa khách hàng";
            }
            // chi tiết khách hàng - xóa
            else if (actionName == ActionName.DELETE && objectName == ObjectName.CUSTOMER)
            {
                var screen = listScreen.FirstOrDefault(x => x.ScreenCode == "CUSTOMER_DETAIL");
                if (screen != null)
                {
                    trace.ScreenId = screen.ScreenId;
                }
                trace.Description = "Xóa khách hàng";
            }
            // Tạo cơ hội
            else if (actionName == ActionName.Create && objectName == ObjectName.LEAD)
            {
                var screen = listScreen.FirstOrDefault(x => x.ScreenCode == "LEAD");
                if (screen != null)
                {
                    trace.ScreenId = screen.ScreenId;
                }
                trace.Description = "Tạo mới cơ hội";
            }
            // chi tiết cơ hội - chỉnh sửa
            else if (actionName == ActionName.UPDATE && objectName == ObjectName.LEAD)
            {
                var screen = listScreen.FirstOrDefault(x => x.ScreenCode == "LEAD_DETAIL");
                if (screen != null)
                {
                    trace.ScreenId = screen.ScreenId;
                }
                trace.Description = "Chỉnh sửa cơ hội";
            }
            // chi tiết cơ hội - xóa
            else if (actionName == ActionName.DELETE && objectName == ObjectName.LEAD)
            {
                var screen = listScreen.FirstOrDefault(x => x.ScreenCode == "LEAD_DETAIL");
                if (screen != null)
                {
                    trace.ScreenId = screen.ScreenId;
                }
                trace.Description = "Xóa cơ hội";
            }
            // Tạo Hồ sơ thầu
            else if (actionName == ActionName.Create && objectName == ObjectName.SALEBIDDING)
            {
                var screen = listScreen.FirstOrDefault(x => x.ScreenCode == "SALE_BIDDING");
                if (screen != null)
                {
                    trace.ScreenId = screen.ScreenId;
                }
                trace.Description = "Tạo mới hồ sơ thầu";
            }
            // chi tiết hồ sơ thầu - chỉnh sửa
            else if (actionName == ActionName.UPDATE && objectName == ObjectName.SALEBIDDING)
            {
                var screen = listScreen.FirstOrDefault(x => x.ScreenCode == "SALE_BIDDING_DETAIL");
                if (screen != null)
                {
                    trace.ScreenId = screen.ScreenId;
                }
                trace.Description = "Chỉnh sửa hồ sơ thầu";
            }
            // Tạo báo giá
            else if (actionName == ActionName.Create && objectName == ObjectName.QUOTE)
            {
                var screen = listScreen.FirstOrDefault(x => x.ScreenCode == "QUOTE");
                if (screen != null)
                {
                    trace.ScreenId = screen.ScreenId;
                }
                trace.Description = "Tạo mới báo giá";
            }
            // chi tiết báo giá - chỉnh sửa
            else if (actionName == ActionName.UPDATE && objectName == ObjectName.QUOTE)
            {
                var screen = listScreen.FirstOrDefault(x => x.ScreenCode == "QUOTE_DETAIL");
                if (screen != null)
                {
                    trace.ScreenId = screen.ScreenId;
                }
                trace.Description = "Chỉnh sửa báo giá";
            }
            // chi tiết báo giá - xóa
            else if (actionName == ActionName.DELETE && objectName == ObjectName.QUOTE)
            {
                var screen = listScreen.FirstOrDefault(x => x.ScreenCode == "QUOTE_DETAIL");
                if (screen != null)
                {
                    trace.ScreenId = screen.ScreenId;
                }
                trace.Description = "Xóa báo giá";
            }
            // Tạo CT Chăm sóc
            else if (actionName == ActionName.Create && objectName == ObjectName.CUSTOMERCARE)
            {
                var screen = listScreen.FirstOrDefault(x => x.ScreenCode == "CUSTOMER_CARE");
                if (screen != null)
                {
                    trace.ScreenId = screen.ScreenId;
                }
                trace.Description = "Tạo CT chăm sóc";
            }
            // chi tiết CT Chăm sóc - chỉnh sửa
            else if (actionName == ActionName.UPDATE && objectName == ObjectName.CUSTOMERCARE)
            {
                var screen = listScreen.FirstOrDefault(x => x.ScreenCode == "CUSTOMER_CARE_DETAIL");
                if (screen != null)
                {
                    trace.ScreenId = screen.ScreenId;
                }
                trace.Description = "Chỉnh sửa CT chăm sóc";
            }
            // Tạo CT khuyến mại
            else if (actionName == ActionName.Create && objectName == ObjectName.PROMOTION)
            {
                var screen = listScreen.FirstOrDefault(x => x.ScreenCode == "PROMOTION");
                if (screen != null)
                {
                    trace.ScreenId = screen.ScreenId;
                }
                trace.Description = "Tạo mới CT khuyến mại";
            }
            // chi tiết CT khuyến mại - chỉnh sửa
            else if (actionName == ActionName.UPDATE && objectName == ObjectName.PROMOTION)
            {
                var screen = listScreen.FirstOrDefault(x => x.ScreenCode == "PROMOTION_DETAIL");
                if (screen != null)
                {
                    trace.ScreenId = screen.ScreenId;
                }
                trace.Description = "Chỉnh sửa CT khuyến mại";
            }
            // chi tiết CT khuyến mại - xóa
            else if (actionName == ActionName.DELETE && objectName == ObjectName.PROMOTION)
            {
                var screen = listScreen.FirstOrDefault(x => x.ScreenCode == "PROMOTION_DETAIL");
                if (screen != null)
                {
                    trace.ScreenId = screen.ScreenId;
                }
                trace.Description = "Xóa CT khuyến mại";
            }
            // Tạo hợp đồng bán
            else if (actionName == ActionName.Create && objectName == ObjectName.CONTRACT)
            {
                var screen = listScreen.FirstOrDefault(x => x.ScreenCode == "CONTRACT");
                if (screen != null)
                {
                    trace.ScreenId = screen.ScreenId;
                }
                trace.Description = "Tạo mới hợp đồng bán";
            }
            // chi tiết hợp đồng bán - chỉnh sửa
            else if (actionName == ActionName.UPDATE && objectName == ObjectName.CONTRACT)
            {
                var screen = listScreen.FirstOrDefault(x => x.ScreenCode == "CONTRACT_DETAIL");
                if (screen != null)
                {
                    trace.ScreenId = screen.ScreenId;
                }
                trace.Description = "Chỉnh sửa hợp đồng bán";
            }
            // chi tiết hợp đồng bán - xóa
            else if (actionName == ActionName.DELETE && objectName == ObjectName.CONTRACT)
            {
                var screen = listScreen.FirstOrDefault(x => x.ScreenCode == "CONTRACT_DETAIL");
                if (screen != null)
                {
                    trace.ScreenId = screen.ScreenId;
                }
                trace.Description = "Xóa hợp đồng bán";
            }
            // Tạo đơn hàng
            else if (actionName == ActionName.Create && objectName == ObjectName.CUSTOMERORDER)
            {
                var screen = listScreen.FirstOrDefault(x => x.ScreenCode == "CUSTOMER_ORDER");
                if (screen != null)
                {
                    trace.ScreenId = screen.ScreenId;
                }
                trace.Description = "Tạo mới đơn hàng";
            }
            // chi tiết đơn hàng - chỉnh sửa
            else if (actionName == ActionName.UPDATE && objectName == ObjectName.CUSTOMERORDER)
            {
                var screen = listScreen.FirstOrDefault(x => x.ScreenCode == "CUSTOMER_ORDER_DETAIL");
                if (screen != null)
                {
                    trace.ScreenId = screen.ScreenId;
                }
                trace.Description = "Chỉnh sửa đơn hàng";
            }
            // chi tiết đơn hàng - xóa
            else if (actionName == ActionName.DELETE && objectName == ObjectName.CUSTOMERORDER)
            {
                var screen = listScreen.FirstOrDefault(x => x.ScreenCode == "CUSTOMER_ORDER_DETAIL");
                if (screen != null)
                {
                    trace.ScreenId = screen.ScreenId;
                }
                trace.Description = "Xóa đơn hàng";
            }
            // Tạo hóa đơn
            else if (actionName == ActionName.Create && objectName == ObjectName.BILLSALE)
            {
                var screen = listScreen.FirstOrDefault(x => x.ScreenCode == "BILL_SALE");
                if (screen != null)
                {
                    trace.ScreenId = screen.ScreenId;
                }
                trace.Description = "Tạo mới hóa đơn";
            }
            // chi tiết hóa đơn - chỉnh sửa
            else if (actionName == ActionName.UPDATE && objectName == ObjectName.BILLSALE)
            {
                var screen = listScreen.FirstOrDefault(x => x.ScreenCode == "BILL_SALE_DETAIL");
                if (screen != null)
                {
                    trace.ScreenId = screen.ScreenId;
                }
                trace.Description = "Chỉnh sửa hóa đơn";
            }
            // chi tiết hóa đơn - xóa
            else if (actionName == ActionName.DELETE && objectName == ObjectName.BILLSALE)
            {
                var screen = listScreen.FirstOrDefault(x => x.ScreenCode == "BILL_SALE_DETAIL");
                if (screen != null)
                {
                    trace.ScreenId = screen.ScreenId;
                }
                trace.Description = "Xóa hóa đơn";
            }
            // Tạo đề xuất mua hàng
            else if (actionName == ActionName.Create && objectName == ObjectName.PROCUREMENTREQUEST)
            {
                var screen = listScreen.FirstOrDefault(x => x.ScreenCode == "PROCUREMENT_REQUEST");
                if (screen != null)
                {
                    trace.ScreenId = screen.ScreenId;
                }
                trace.Description = "Tạo mới đề xuất mua hàng";
            }
            // chi tiết hóa đơn - chỉnh sửa
            else if (actionName == ActionName.UPDATE && objectName == ObjectName.PROCUREMENTREQUEST)
            {
                var screen = listScreen.FirstOrDefault(x => x.ScreenCode == "PROCUREMENT_REQUEST_DETAIL");
                if (screen != null)
                {
                    trace.ScreenId = screen.ScreenId;
                }
                trace.Description = "Chỉnh sửa đề xuất mua hàng";
            }
            // chi tiết hóa đơn - xóa
            else if (actionName == ActionName.DELETE && objectName == ObjectName.PROCUREMENTREQUEST)
            {
                var screen = listScreen.FirstOrDefault(x => x.ScreenCode == "PROCUREMENT_REQUEST_DETAIL");
                if (screen != null)
                {
                    trace.ScreenId = screen.ScreenId;
                }
                trace.Description = "Xóa đề xuất mua hàng";
            }
            // Tạo đơn hàng mua
            else if (actionName == ActionName.Create && objectName == ObjectName.VENDORORDER)
            {
                var screen = listScreen.FirstOrDefault(x => x.ScreenCode == "VENDOR_ORDER");
                if (screen != null)
                {
                    trace.ScreenId = screen.ScreenId;
                }
                trace.Description = "Tạo mới đề xuất mua hàng";
            }
            // chi tiết hóa đơn - chỉnh sửa
            else if (actionName == ActionName.UPDATE && objectName == ObjectName.VENDORORDER)
            {
                var screen = listScreen.FirstOrDefault(x => x.ScreenCode == "VENDOR_ORDER_DETAIL");
                if (screen != null)
                {
                    trace.ScreenId = screen.ScreenId;
                }
                trace.Description = "Chỉnh sửa đề xuất mua hàng";
            }
            // chi tiết hóa đơn - xóa
            else if (actionName == ActionName.DELETE && objectName == ObjectName.VENDORORDER)
            {
                var screen = listScreen.FirstOrDefault(x => x.ScreenCode == "VENDOR_ORDER_DETAIL");
                if (screen != null)
                {
                    trace.ScreenId = screen.ScreenId;
                }
                trace.Description = "Xóa đề xuất mua hàng";
            }
            // Tạo sản phẩm dịch vụ
            else if (actionName == ActionName.Create && objectName == ObjectName.PRODUCT)
            {
                var screen = listScreen.FirstOrDefault(x => x.ScreenCode == "PRODUCT");
                if (screen != null)
                {
                    trace.ScreenId = screen.ScreenId;
                }
                trace.Description = "Tạo mới sản phẩm dịch vụ";
            }
            // chi tiết sản phẩm dịch vụ - chỉnh sửa
            else if (actionName == ActionName.UPDATE && objectName == ObjectName.PRODUCT)
            {
                var screen = listScreen.FirstOrDefault(x => x.ScreenCode == "PRODUCT_DETAIL");
                if (screen != null)
                {
                    trace.ScreenId = screen.ScreenId;
                }
                trace.Description = "Chỉnh sửa sản phẩm dịch vụ";
            }
            // chi tiết sản phẩm dịch vụ - xóa
            else if (actionName == ActionName.DELETE && objectName == ObjectName.PRODUCT)
            {
                var screen = listScreen.FirstOrDefault(x => x.ScreenCode == "PRODUCT_DETAIL");
                if (screen != null)
                {
                    trace.ScreenId = screen.ScreenId;
                }
                trace.Description = "Xóa sản phẩm dịch vụ";
            }

            trace.TraceId = Guid.NewGuid();
            trace.ActionName = actionName.ToUpper();
            trace.ObjectName = objectName.ToUpper();
            trace.CreatedById = createById;
            trace.CreatedDate = DateTime.Now;
            trace.ObjectId = objectId;

            context.AuditTrace.Add(trace);
            context.SaveChanges();
        }
    }
}
