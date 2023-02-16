using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using KellermanSoftware.CompareNetObjects;
using Microsoft.AspNetCore.Hosting.Internal;
using TN.TNM.Common;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.Common.NotificationSetting;
using TN.TNM.DataAccess.Databases;
using TN.TNM.DataAccess.Models.BillSale;
using TN.TNM.DataAccess.Models.ReceiptInvoice;
using TN.TNM.DataAccess.Models.Employee;
using TN.TNM.DataAccess.Models.Asset;
using TN.TNM.DataAccess.Models.Salary;
using TN.TNM.DataAccess.ConstType.Note;

namespace TN.TNM.DataAccess.Helper
{
    public static class NotificationHelper
    {
        /*
         * checkChange (true: thực hiện kiểm tra thay đổi của olModel và newModel,
         *              false: không thực hiện kiểm tra thay đổi) 
         */
        public static void AccessNotification(TNTN8Context context, string typeModel, string actionCode,
            object oldModel, object newModel, bool checkChange, object note = null, Guid? empId = new Guid?(),
            List<string> ListMailNguoiPheDuyet = null, List<Guid> lstInterviewId = null, string emailReceiver = null)
        // chuyển mail cá nhân và công ty của người phê duyệt để gửi thông báo; áp dụng cho đề xuất TL, CV, OT
        {
            if (checkChange)
            {
                var configEntity = context.SystemParameter.ToList();

                #region Kiểm tra xem đã có cấu hình cho thông báo chưa?

                var screenId = context.Screen.FirstOrDefault(x => x.ScreenCode == typeModel)?.ScreenId;
                var NotifiActionId = context.NotifiAction.FirstOrDefault(x => x.NotifiActionCode == actionCode && x.ScreenId == screenId)
                    ?.NotifiActionId;

                var notifiSetting =
                    context.NotifiSetting.FirstOrDefault(x => x.ScreenId == screenId &&
                                                              x.NotifiActionId == NotifiActionId && x.Active);

                #endregion

                if (notifiSetting == null) return;
                {
                    //Tạo mới đơn hàng
                    if (typeModel == TypeModel.CustomerOrder)
                    {
                        var _customerOrder = newModel as CustomerOrder;

                        if (_customerOrder != null)
                        {
                            #region Kiểm tra xem cấu hình có điều kiện không?

                            var listNotifiCondition = context.NotifiSettingCondition
                                .Where(x => x.NotifiSettingId == notifiSetting.NotifiSettingId).ToList();

                            #region List reference để kiểm tra điều kiện và thông tin dùng để gửi email

                            var listAllInforScreen = context.InforScreen.Where(x => x.ScreenId == screenId).ToList();
                            var listAllCondition = context.NotifiCondition.Where(x => x.ScreenId == screenId).ToList();

                            var listAllEmployee = context.Employee.Where(x => x.Active == true).ToList();
                            var listAllQuote = context.Quote.Where(x => x.Active == true).ToList();
                            var listAllOrderDetail =
                                context.CustomerOrderDetail.Where(x => x.OrderId == _customerOrder.OrderId);
                            var listAllOrderDetailId = listAllOrderDetail.Select(y => y.ProductId).Distinct().ToList();
                            var listProduct = context.Product.Where(x => listAllOrderDetailId.Contains(x.ProductId))
                                .ToList();
                            var listAllContact = context.Contact.Where(x =>
                                x.Active == true && (x.ObjectType == "CUS" ||
                                                     x.ObjectType == "CUS_CON" ||
                                                     x.ObjectType == "EMP")).ToList();

                            #endregion

                            //Nếu có điều kiện
                            if (listNotifiCondition.Count > 0)
                            {
                                var check_condition =
                                    true; //true: tất cả điều kiện đều thõa mãn, false: có điều kiện không thỏa mãn
                                for (var i = 0; i < listNotifiCondition.Count; i++)
                                {
                                    var _conditionMapping = listNotifiCondition[i];

                                    //Trường thông tin
                                    var inforScreen =
                                        listAllInforScreen.FirstOrDefault(x =>
                                            x.InforScreenId == _conditionMapping.InforScreenId);

                                    var condition = listAllCondition.FirstOrDefault(x =>
                                        x.NotifiConditionId == _conditionMapping.NotifiSettingConditionId);

                                    //Nếu có điều kiện trên db
                                    if (condition != null)
                                    {
                                        //Nhân viên bán hàng
                                        if (inforScreen?.InforScreenCode == "NVBH")
                                        {
                                            var seller = listAllEmployee.FirstOrDefault(x =>
                                                x.EmployeeId == _customerOrder?.Seller);

                                            check_condition = CheckStringCondition(seller?.EmployeeName?.Trim(),
                                                _conditionMapping.StringValue?.Trim(), condition.TypeCondition);
                                        }

                                        //Số hợp đồng
                                        if (inforScreen?.InforScreenCode == "SHD")
                                        {
                                            check_condition = CheckStringCondition(_customerOrder?.OrderCode?.Trim(),
                                                _conditionMapping.StringValue?.Trim(), condition.TypeCondition);
                                        }

                                        //Số báo giá
                                        if (inforScreen?.InforScreenCode == "SBG")
                                        {
                                            var quote = listAllQuote.FirstOrDefault(
                                                x => x.QuoteId == _customerOrder.QuoteId);

                                            if (quote == null)
                                            {
                                                check_condition = false;
                                            }
                                            else
                                            {
                                                check_condition = CheckStringCondition(quote.QuoteCode?.Trim(),
                                                    _conditionMapping.StringValue?.Trim(), condition.TypeCondition);
                                            }
                                        }

                                        //Mã sản phẩm
                                        if (inforScreen?.InforScreenCode == "MSP")
                                        {
                                            var listProductCode = listProduct.Select(y => y.ProductCode?.Trim()).ToList();
                                            check_condition = CheckListStringCondition(listProductCode,
                                                _conditionMapping.StringValue?.Trim(), condition.TypeCondition);
                                        }

                                        //Tổng thanh toán
                                        if (inforScreen?.InforScreenCode == "TTT")
                                        {
                                            var tong_thanh_toan = _customerOrder.Amount;
                                            check_condition = CheckNumberCondition(tong_thanh_toan,
                                                _conditionMapping.NumberValue.Value, condition.TypeCondition);
                                        }

                                        if (check_condition == false) break;
                                    }
                                    //Nếu không có điều kiện trên db
                                    else
                                    {
                                        check_condition = false;
                                    }
                                }

                                //Nếu tất cả điều kiện đều thỏa mãn
                                if (check_condition)
                                {
                                    //Nếu gửi nội bộ
                                    if (notifiSetting.SendInternal)
                                    {
                                        //Nếu gửi bằng email
                                        if (notifiSetting.IsEmail)
                                        {
                                            #region Lấy danh sách email cần gửi thông báo

                                            var listEmailSendTo = new List<string>();
                                            var listAllUser = context.User.ToList();

                                            #region Lấy email người tham gia (Hiện tại chưa có người tham gia)

                                            if (notifiSetting.IsParticipant)
                                            {

                                            }

                                            #endregion

                                            #region Lấy email người phê duyệt (Tạo mới thì chưa cần gửi email cho người phê duyệt)

                                            if (notifiSetting.IsApproved)
                                            {
                                                //Người phê duyệt đơn hàng là người được phân quyền dữ liệu là Quản lý
                                                var listManager = listAllEmployee.Where(x => x.IsManager)
                                                    .Select(y => y.EmployeeId).ToList();
                                                var listEmailManager = listAllContact
                                                    .Where(x => listManager.Contains(x.ObjectId) && x.ObjectType == "EMP")
                                                    .Select(y => y.Email).ToList();

                                                listEmailManager.ForEach(emailManager =>
                                                {
                                                    if (!String.IsNullOrEmpty(emailManager))
                                                    {
                                                        listEmailSendTo.Add(emailManager.Trim());
                                                    }
                                                });
                                            }

                                            #endregion

                                            #region Lấy email người tạo

                                            if (notifiSetting.IsCreated)
                                            {
                                                var Email = LayEmaiNguoiTao(listAllUser, listAllContact, _customerOrder.CreatedById);
                                                listEmailSendTo.Add(Email);
                                            }

                                            #endregion

                                            #region Lấy email người phụ trách (Nhân viên bán hàng)

                                            if (notifiSetting.IsPersonIncharge)
                                            {
                                                var email_seller = listAllContact.FirstOrDefault(x =>
                                                    x.ObjectId == _customerOrder.Seller && x.ObjectType == "EMP")?.Email;

                                                if (!String.IsNullOrEmpty(email_seller))
                                                {
                                                    listEmailSendTo.Add(email_seller.Trim());
                                                }
                                            }

                                            #endregion

                                            #region Lấy email của danh sách người đặc biệt

                                            var listEmailSpecial = LayEmaiNguoiDacBiet(listAllUser, listAllContact, context.NotifiSpecial.ToList(), notifiSetting);
                                            listEmailSpecial.ForEach(email =>
                                            {
                                                if (!String.IsNullOrEmpty(email))
                                                {
                                                    listEmailSendTo.Add(email.Trim());
                                                }
                                            });

                                            #endregion

                                            listEmailSendTo = listEmailSendTo.Distinct().ToList();

                                            #endregion

                                            #region Kiểm tra xem Gửi ngay hay Đặt lịch gửi 

                                            //Gửi ngay
                                            var subject = ReplaceTokenForContent(context, typeModel, _customerOrder,
                                                notifiSetting.EmailTitle, configEntity);
                                            var content = ReplaceTokenForContent(context, typeModel, _customerOrder,
                                                notifiSetting.EmailContent, configEntity);
                                            Emailer.SendEmail(context, listEmailSendTo, new List<string>(), new List<string>(), subject, content);

                                            //Đặt lịch gửi
                                            if (notifiSetting.ObjectBackHourInternal != null && listEmailSendTo.Count > 0)
                                            {

                                            }

                                            #endregion
                                        }
                                    }
                                }
                            }
                            //Nếu không có điều kiện
                            else
                            {
                                //Nếu gửi nội bộ
                                if (notifiSetting.SendInternal)
                                {
                                    //Nếu gửi bằng email
                                    if (notifiSetting.IsEmail)
                                    {
                                        #region Lấy danh sách email cần gửi thông báo

                                        var listEmailSendTo = new List<string>();
                                        var listAllUser = context.User.ToList();

                                        #region Lấy email người tham gia (Hiện tại chưa có người tham gia)

                                        if (notifiSetting.IsParticipant)
                                        {
                                        }

                                        #endregion

                                        #region Lấy email người phê duyệt

                                        if (notifiSetting.IsApproved)
                                        {
                                            //Người phê duyệt đơn hàng là người được phân quyền dữ liệu là Quản lý
                                            var listManager = listAllEmployee.Where(x => x.IsManager)
                                                .Select(y => y.EmployeeId).ToList();
                                            var listEmailManager = listAllContact
                                                .Where(x => listManager.Contains(x.ObjectId) && x.ObjectType == "EMP")
                                                .Select(y => y.Email).ToList();

                                            listEmailManager.ForEach(emailManager =>
                                            {
                                                if (!String.IsNullOrEmpty(emailManager))
                                                {
                                                    listEmailSendTo.Add(emailManager.Trim());
                                                }
                                            });
                                        }

                                        #endregion

                                        #region Lấy email người tạo

                                        if (notifiSetting.IsCreated)
                                        {
                                            var Email = LayEmaiNguoiTao(listAllUser, listAllContact, _customerOrder.CreatedById);
                                            listEmailSendTo.Add(Email);
                                        }

                                        #endregion

                                        #region Lấy email người phụ trách (Nhân viên bán hàng)

                                        if (notifiSetting.IsPersonIncharge)
                                        {
                                            var email_seller = listAllContact.FirstOrDefault(x =>
                                                x.ObjectId == _customerOrder.Seller && x.ObjectType == "EMP")?.Email;

                                            if (!String.IsNullOrEmpty(email_seller))
                                            {
                                                listEmailSendTo.Add(email_seller.Trim());
                                            }
                                        }

                                        #endregion

                                        #region Lấy email của danh sách người đặc biệt

                                        var listEmailSpecial = LayEmaiNguoiDacBiet(listAllUser, listAllContact, context.NotifiSpecial.ToList(), notifiSetting);
                                        listEmailSpecial.ForEach(email =>
                                        {
                                            if (!String.IsNullOrEmpty(email))
                                            {
                                                listEmailSendTo.Add(email.Trim());
                                            }
                                        });

                                        #endregion

                                        listEmailSendTo = listEmailSendTo.Distinct().ToList();

                                        #endregion

                                        #region Kiểm tra xem Gửi ngay hay Đặt lịch gửi 

                                        //Gửi ngay
                                        var subject = ReplaceTokenForContent(context, typeModel, _customerOrder,
                                            notifiSetting.EmailTitle, configEntity);
                                        var content = ReplaceTokenForContent(context, typeModel, _customerOrder,
                                            notifiSetting.EmailContent, configEntity);
                                        Emailer.SendEmail(context, listEmailSendTo, new List<string>(), new List<string>(), subject, content);

                                        //Đặt lịch gửi
                                        if (notifiSetting.ObjectBackHourInternal != null && listEmailSendTo.Count > 0)
                                        {
                                        }

                                        #endregion
                                    }
                                }
                            }

                            #endregion
                        }

                    }
                    //Chi tiết đơn hàng
                    else if (typeModel == TypeModel.CustomerOrderDetail)
                    {
                        var _customerOrder = newModel as CustomerOrder;

                        if (_customerOrder != null)
                        {
                            #region Kiểm tra xem cấu hình có điều kiện không?

                            var listNotifiCondition = context.NotifiSettingCondition
                                .Where(x => x.NotifiSettingId == notifiSetting.NotifiSettingId).ToList();

                            #region List reference để kiểm tra điều kiện và thông tin dùng để gửi email

                            var listAllInforScreen = context.InforScreen.Where(x => x.ScreenId == screenId).ToList();
                            var listAllCondition = context.NotifiCondition.Where(x => x.ScreenId == screenId).ToList();

                            var listAllEmployee = context.Employee.Where(x => x.Active == true).ToList();
                            var listAllQuote = context.Quote.Where(x => x.Active == true).ToList();
                            var listAllOrderDetail =
                                context.CustomerOrderDetail.Where(x => x.OrderId == _customerOrder.OrderId);
                            var listAllOrderDetailId = listAllOrderDetail.Select(y => y.ProductId).Distinct().ToList();
                            var listProduct = context.Product.Where(x => listAllOrderDetailId.Contains(x.ProductId))
                                .ToList();
                            var listAllContact = context.Contact.Where(x =>
                                x.Active == true && (x.ObjectType == "CUS" ||
                                                     x.ObjectType == "CUS_CON" ||
                                                     x.ObjectType == "EMP")).ToList();

                            #endregion

                            //Nếu có điều kiện
                            if (listNotifiCondition.Count > 0)
                            {
                                var check_condition =
                                    true; //true: tất cả điều kiện đều thõa mãn, false: có điều kiện không thỏa mãn
                                for (var i = 0; i < listNotifiCondition.Count; i++)
                                {
                                    var _conditionMapping = listNotifiCondition[i];

                                    //Trường thông tin
                                    var inforScreen =
                                        listAllInforScreen.FirstOrDefault(x =>
                                            x.InforScreenId == _conditionMapping.InforScreenId);

                                    var condition = listAllCondition.FirstOrDefault(x =>
                                        x.NotifiConditionId == _conditionMapping.NotifiSettingConditionId);

                                    //Nếu có điều kiện trên db
                                    if (condition != null)
                                    {
                                        //Nhân viên bán hàng
                                        if (inforScreen?.InforScreenCode == "NVBH")
                                        {
                                            var seller = listAllEmployee.FirstOrDefault(x =>
                                                x.EmployeeId == _customerOrder?.Seller);

                                            check_condition = CheckStringCondition(seller?.EmployeeName?.Trim(),
                                                _conditionMapping.StringValue?.Trim(), condition.TypeCondition);
                                        }

                                        //Số hợp đồng
                                        if (inforScreen?.InforScreenCode == "SHD")
                                        {
                                            check_condition = CheckStringCondition(_customerOrder?.OrderCode?.Trim(),
                                                _conditionMapping.StringValue?.Trim(), condition.TypeCondition);
                                        }

                                        //Số báo giá
                                        if (inforScreen?.InforScreenCode == "SBG")
                                        {
                                            var quote = listAllQuote.FirstOrDefault(
                                                x => x.QuoteId == _customerOrder.QuoteId);

                                            if (quote == null)
                                            {
                                                check_condition = false;
                                            }
                                            else
                                            {
                                                check_condition = CheckStringCondition(quote.QuoteCode?.Trim(),
                                                    _conditionMapping.StringValue?.Trim(), condition.TypeCondition);
                                            }
                                        }

                                        //Mã sản phẩm
                                        if (inforScreen?.InforScreenCode == "MSP")
                                        {
                                            var listProductCode = listProduct.Select(y => y.ProductCode?.Trim()).ToList();
                                            check_condition = CheckListStringCondition(listProductCode,
                                                _conditionMapping.StringValue?.Trim(), condition.TypeCondition);
                                        }

                                        //Tổng thanh toán
                                        if (inforScreen?.InforScreenCode == "TTT")
                                        {
                                            var tong_thanh_toan = _customerOrder.Amount;
                                            check_condition = CheckNumberCondition(tong_thanh_toan,
                                                _conditionMapping.NumberValue.Value, condition.TypeCondition);
                                        }

                                        if (check_condition == false) break;
                                    }
                                    //Nếu không có điều kiện trên db
                                    else
                                    {
                                        check_condition = false;
                                    }
                                }

                                //Nếu tất cả điều kiện đều thỏa mãn
                                if (check_condition)
                                {
                                    //Nếu gửi nội bộ
                                    if (notifiSetting.SendInternal)
                                    {
                                        #region Lấy danh sách email cần gửi thông báo

                                        var listEmailSendTo = new List<string>();
                                        var listAllUser = context.User.ToList();
                                        #region Lấy email người tham gia (Hiện tại chưa có người tham gia)

                                        if (notifiSetting.IsParticipant)
                                        {
                                        }

                                        #endregion

                                        #region Lấy email người phê duyệt được thông báo

                                        if (notifiSetting.IsApproved)
                                        {
                                            /**
                                             * Người phê duyệt đơn hàng được thông báo
                                             * là người được phân quyền dữ liệu là Quản lý
                                             * của nhân viên bán hàng của đơn hàng
                                             */

                                            // trường hợp gửi thông báo khi bình luân
                                            if (note != null)
                                            {
                                                var _note = note as Note;

                                                // lấy id của nhân viên bán hàng 
                                                var customerOrderSellerId =
                                                    context.CustomerOrder.FirstOrDefault(x => x.OrderId == _note.ObjectId)?.Seller;

                                                // lấy id phong ban thong qua id nhan vien
                                                var organizationId = listAllEmployee.FirstOrDefault(x => x.EmployeeId == customerOrderSellerId)?
                                                    .OrganizationId;

                                                // lay id nhan vien phu trach cua phong ban
                                                var listManager = listAllEmployee.Where(x => x.IsManager &&
                                                        x.OrganizationId == organizationId)
                                                    .Select(y => y.EmployeeId).ToList();

                                                // lay email cua nhan vien phu trach do
                                                var listEmailManager = listAllContact
                                                    .Where(x => listManager.Contains(x.ObjectId) && x.ObjectType == "EMP")
                                                    .Select(y => y.Email).ToList();

                                                // add email do vao list email send to
                                                listEmailManager.ForEach(emailManager =>
                                                {
                                                    if (!String.IsNullOrEmpty(emailManager))
                                                    {
                                                        listEmailSendTo.Add(emailManager.Trim());
                                                    }
                                                });
                                            }
                                            else
                                            {
                                                // lấy id phong ban thong qua id nhan vien
                                                var organizationId = listAllEmployee.FirstOrDefault(x => x.EmployeeId == empId)?
                                                    .OrganizationId;

                                                // lay id nhan vien phu trach cua phong ban
                                                var listManager = listAllEmployee.Where(x => x.IsManager &&
                                                        x.OrganizationId == organizationId)
                                                    .Select(y => y.EmployeeId).ToList();

                                                // lay email cua nhan vien phu trach do
                                                var listEmailManager = listAllContact
                                                    .Where(x => listManager.Contains(x.ObjectId) && x.ObjectType == "EMP")
                                                    .Select(y => y.Email).ToList();

                                                // add email do vao list email send to
                                                listEmailManager.ForEach(emailManager =>
                                                {
                                                    if (!String.IsNullOrEmpty(emailManager))
                                                    {
                                                        listEmailSendTo.Add(emailManager.Trim());
                                                    }
                                                });
                                            }
                                        }

                                        #endregion

                                        #region Lấy email người tạo

                                        if (notifiSetting.IsCreated)
                                        {
                                            var Email = LayEmaiNguoiTao(listAllUser, listAllContact, _customerOrder.CreatedById);
                                            listEmailSendTo.Add(Email);
                                        }

                                        #endregion

                                        #region Lấy email người phụ trách (Nhân viên bán hàng)

                                        if (notifiSetting.IsPersonIncharge)
                                        {
                                            var email_seller = listAllContact.FirstOrDefault(x =>
                                                x.ObjectId == _customerOrder.Seller && x.ObjectType == "EMP")?.Email;

                                            if (!String.IsNullOrEmpty(email_seller))
                                            {
                                                listEmailSendTo.Add(email_seller.Trim());
                                            }
                                        }

                                        #endregion

                                        #region Lấy email của danh sách người đặc biệt

                                        var listEmailSpecial = LayEmaiNguoiDacBiet(listAllUser, listAllContact, context.NotifiSpecial.ToList(), notifiSetting);
                                        listEmailSpecial.ForEach(email =>
                                        {
                                            if (!String.IsNullOrEmpty(email))
                                            {
                                                listEmailSendTo.Add(email.Trim());
                                            }
                                        });

                                        #endregion

                                        listEmailSendTo = listEmailSendTo.Distinct().ToList();

                                        #endregion

                                        #region Kiểm tra xem Gửi ngay hay Đặt lịch gửi 

                                        //Gửi ngay
                                        var subject = ReplaceTokenForContent(context, typeModel, _customerOrder,
                                            notifiSetting.EmailTitle, configEntity);
                                        var content = ReplaceTokenForContent(context, typeModel, _customerOrder,
                                            notifiSetting.EmailContent, configEntity);
                                        Emailer.SendEmail(context, listEmailSendTo, new List<string>(), new List<string>(), subject, content);

                                        //Đặt lịch gửi
                                        if (notifiSetting.ObjectBackHourInternal != null && listEmailSendTo.Count > 0)
                                        {
                                        }

                                        #endregion
                                    }
                                }
                            }
                            //Nếu không có điều kiện
                            else
                            {
                                //Nếu gửi nội bộ
                                if (notifiSetting.SendInternal)
                                {
                                    //Nếu gửi bằng email
                                    if (notifiSetting.IsEmail)
                                    {
                                        #region Lấy danh sách email cần gửi thông báo

                                        var listEmailSendTo = new List<string>();
                                        var listAllUser = context.User.ToList();

                                        #region Lấy email người tham gia (Hiện tại chưa có người tham gia)

                                        if (notifiSetting.IsParticipant)
                                        {
                                        }

                                        #endregion

                                        #region Lấy email người phê duyệt

                                        if (notifiSetting.IsApproved)
                                        {
                                            /**
                                             * Người phê duyệt đơn hàng được thông báo
                                             * là người được phân quyền dữ liệu là Quản lý
                                             * của nhân viên bán hàng của đơn hàng
                                             */

                                            // trường hợp gửi thông báo khi bình luân
                                            if (note != null)
                                            {
                                                var _note = note as Note;

                                                // lấy id của nhân viên bán hàng 
                                                var customerOrderSellerId =
                                                    context.CustomerOrder.FirstOrDefault(x => x.OrderId == _note.ObjectId)?.Seller;

                                                // lấy id phong ban thong qua id nhan vien
                                                var organizationId = listAllEmployee.FirstOrDefault(x => x.EmployeeId == customerOrderSellerId)?
                                                    .OrganizationId;

                                                // lay id nhan vien phu trach cua phong ban
                                                var listManager = listAllEmployee.Where(x => x.IsManager &&
                                                        x.OrganizationId == organizationId)
                                                    .Select(y => y.EmployeeId).ToList();

                                                // lay email cua nhan vien phu trach do
                                                var listEmailManager = listAllContact
                                                    .Where(x => listManager.Contains(x.ObjectId) && x.ObjectType == "EMP")
                                                    .Select(y => y.Email).ToList();

                                                // add email do vao list email send to
                                                listEmailManager.ForEach(emailManager =>
                                                {
                                                    if (!String.IsNullOrEmpty(emailManager))
                                                    {
                                                        listEmailSendTo.Add(emailManager.Trim());
                                                    }
                                                });
                                            }
                                            else
                                            {
                                                // lấy id phong ban thong qua id nhan vien
                                                var organizationId = listAllEmployee.FirstOrDefault(x => x.EmployeeId == empId)?
                                                    .OrganizationId;

                                                // lay id nhan vien phu trach cua phong ban
                                                var listManager = listAllEmployee.Where(x => x.IsManager &&
                                                        x.OrganizationId == organizationId)
                                                    .Select(y => y.EmployeeId).ToList();

                                                // lay email cua nhan vien phu trach do
                                                var listEmailManager = listAllContact
                                                    .Where(x => listManager.Contains(x.ObjectId) && x.ObjectType == "EMP")
                                                    .Select(y => y.Email).ToList();

                                                // add email do vao list email send to
                                                listEmailManager.ForEach(emailManager =>
                                                {
                                                    if (!String.IsNullOrEmpty(emailManager))
                                                    {
                                                        listEmailSendTo.Add(emailManager.Trim());
                                                    }
                                                });
                                            }

                                        }

                                        #endregion

                                        #region Lấy email người tạo

                                        if (notifiSetting.IsCreated)
                                        {
                                            var Email = LayEmaiNguoiTao(listAllUser, listAllContact, _customerOrder.CreatedById);
                                            listEmailSendTo.Add(Email);
                                        }

                                        #endregion

                                        #region Lấy email người phụ trách (Nhân viên bán hàng)

                                        if (notifiSetting.IsPersonIncharge)
                                        {
                                            var email_seller = listAllContact.FirstOrDefault(x =>
                                                x.ObjectId == _customerOrder.Seller && x.ObjectType == "EMP")?.Email;

                                            if (!String.IsNullOrEmpty(email_seller))
                                            {
                                                listEmailSendTo.Add(email_seller.Trim());
                                            }
                                        }

                                        #endregion

                                        #region Lấy email của danh sách người đặc biệt

                                        var listEmailSpecial = LayEmaiNguoiDacBiet(listAllUser, listAllContact, context.NotifiSpecial.ToList(), notifiSetting);
                                        listEmailSpecial.ForEach(email =>
                                        {
                                            if (!String.IsNullOrEmpty(email))
                                            {
                                                listEmailSendTo.Add(email.Trim());
                                            }
                                        });

                                        #endregion

                                        #region Lấy email người tạo bình luận

                                        if (note != null)
                                        {
                                            var _note = note as Note;
                                            var Email = LayEmaiNguoiTao(context.User.ToList(), listAllContact, _note.CreatedById);
                                            listEmailSendTo.Add(Email);
                                        }

                                        #endregion

                                        listEmailSendTo = listEmailSendTo.Distinct().ToList();

                                        #endregion

                                        #region Kiểm tra xem Gửi ngay hay Đặt lịch gửi 

                                        //Gửi ngay
                                        var subject = ReplaceTokenForContent(context, typeModel, _customerOrder,
                                            notifiSetting.EmailTitle, configEntity, note);
                                        var content = ReplaceTokenForContent(context, typeModel, _customerOrder,
                                            notifiSetting.EmailContent, configEntity, note);

                                        #region Build nội dung thay đổi

                                        //string contentModelChange = "";

                                        ////Nếu phải kiểm tra thay đổi của model
                                        //if (checkChange)
                                        //{
                                        //    contentModelChange = ContentModelChange(oldModel, newModel, typeModel);
                                        //}

                                        #endregion

                                        Emailer.SendEmail(context, listEmailSendTo, new List<string>(), new List<string>(), subject, content);

                                        //Đặt lịch gửi             
                                        if (notifiSetting.ObjectBackHourInternal != null && listEmailSendTo.Count > 0)
                                        {
                                        }

                                        #endregion
                                    }
                                }
                            }

                            #endregion
                        }
                    }
                    //Tạo báo giá
                    else if (typeModel == TypeModel.Quote)
                    {
                        var _quote = newModel as Quote;

                        if (_quote != null)
                        {
                            #region Kiểm tra xem cấu hình có điều kiện không?

                            var listNotifiCondition = context.NotifiSettingCondition
                                .Where(x => x.NotifiSettingId == notifiSetting.NotifiSettingId).ToList();

                            #region List reference để kiểm tra điều kiện và thông tin dùng để gửi email

                            var listAllEmployee = context.Employee.Where(x => x.Active == true).ToList();
                            var listAllContact = context.Contact.Where(x => x.Active == true && x.ObjectType == "EMP").ToList();

                            #endregion

                            //Nếu có điều kiện
                            if (listNotifiCondition.Count > 0)
                            {
                                //Do something...
                            }
                            //Nếu không có điều kiện
                            else
                            {
                                //Nếu gửi nội bộ
                                if (notifiSetting.SendInternal)
                                {
                                    //Nếu gửi bằng email
                                    if (notifiSetting.IsEmail)
                                    {
                                        #region Lấy danh sách email cần gửi thông báo

                                        var listEmailSendTo = new List<string>();
                                        var listAllUser = context.User.ToList();

                                        #region Lấy email người tham gia

                                        if (notifiSetting.IsParticipant)
                                        {
                                            var listParticipantId = context.QuoteParticipantMapping
                                                .Where(x => x.QuoteId == _quote.QuoteId).Select(y => y.EmployeeId).ToList();

                                            if (listParticipantId.Count > 0)
                                            {
                                                var listEmailParticipant = listAllContact.Where(x =>
                                                        listParticipantId.Contains(x.ObjectId) && x.ObjectType == "EMP")
                                                    .ToList();

                                                listEmailParticipant.ForEach(contact =>
                                                {
                                                    if (!String.IsNullOrEmpty(contact?.WorkEmail))
                                                    {
                                                        listEmailSendTo.Add(contact.WorkEmail.Trim());
                                                    }
                                                    else if (!String.IsNullOrEmpty(contact?.Email))
                                                    {
                                                        listEmailSendTo.Add(contact.Email.Trim());
                                                    }
                                                });
                                            }
                                        }

                                        #endregion

                                        #region Lấy email người phê duyệt

                                        if (notifiSetting.IsApproved)
                                        {
                                            //Người phê duyệt báo giá sẽ phải kiểm tra theo Quy trình phê duyệt báo giá

                                            var listApproved =
                                                GetListEmployeeApproved(context, "PDBG", _quote.ApprovalStep,
                                                    listAllEmployee);

                                            var listEmailManager = listAllContact
                                                .Where(x => listApproved.Contains(x.ObjectId) && x.ObjectType == "EMP")
                                                .Select(y => y.Email).ToList();

                                            listEmailManager.ForEach(emailManager =>
                                            {
                                                if (!String.IsNullOrEmpty(emailManager))
                                                {
                                                    listEmailSendTo.Add(emailManager.Trim());
                                                }
                                            });
                                        }

                                        #endregion

                                        #region Lấy email người tạo

                                        if (notifiSetting.IsCreated)
                                        {
                                            var Email = LayEmaiNguoiTao(listAllUser, listAllContact, _quote.CreatedById);
                                            listEmailSendTo.Add(Email);
                                        }

                                        #endregion

                                        #region Lấy email người phụ trách (Nhân viên bán hàng)

                                        if (notifiSetting.IsPersonIncharge)
                                        {
                                            var email_seller = listAllContact.FirstOrDefault(x =>
                                                x.ObjectId == _quote.Seller && x.ObjectType == "EMP")?.Email;

                                            if (!String.IsNullOrEmpty(email_seller))
                                            {
                                                listEmailSendTo.Add(email_seller.Trim());
                                            }
                                        }

                                        #endregion

                                        #region Lấy email của danh sách người đặc biệt

                                        var listEmailSpecial = LayEmaiNguoiDacBiet(listAllUser, listAllContact, context.NotifiSpecial.ToList(), notifiSetting);
                                        listEmailSpecial.ForEach(email =>
                                        {
                                            if (!String.IsNullOrEmpty(email))
                                            {
                                                listEmailSendTo.Add(email.Trim());
                                            }
                                        });

                                        #endregion

                                        listEmailSendTo = listEmailSendTo.Distinct().ToList();

                                        #endregion

                                        #region Kiểm tra xem Gửi ngay hay Đặt lịch gửi 

                                        //Gửi ngay
                                        var subject = ReplaceTokenForContent(context, typeModel, _quote,
                                            notifiSetting.EmailTitle, configEntity);
                                        var content = ReplaceTokenForContent(context, typeModel, _quote,
                                            notifiSetting.EmailContent, configEntity);

                                        #region Build nội dung thay đổi

                                        //string contentModelChange = "";

                                        ////Nếu phải kiểm tra thay đổi của model
                                        //if (checkChange)
                                        //{
                                        //    contentModelChange = ContentModelChange(oldModel, newModel, typeModel);
                                        //}

                                        #endregion

                                        Emailer.SendEmail(context, listEmailSendTo, new List<string>(), new List<string>(), subject, content);

                                        //Đặt lịch gửi             
                                        if (notifiSetting.ObjectBackHourInternal != null && listEmailSendTo.Count > 0)
                                        {
                                        }

                                        #endregion
                                    }
                                }
                            }

                            #endregion
                        }
                    }
                    //Chi tiết báo giá
                    else if (typeModel == TypeModel.QuoteDetail)
                    {
                        var _quote = newModel as Quote;

                        if (_quote != null)
                        {
                            #region Kiểm tra xem cấu hình có điều kiện không?

                            var listNotifiCondition = context.NotifiSettingCondition
                                .Where(x => x.NotifiSettingId == notifiSetting.NotifiSettingId).ToList();

                            #region List reference để kiểm tra điều kiện và thông tin dùng để gửi email

                            var listAllEmployee = context.Employee.Where(x => x.Active == true).ToList();
                            var listAllContact = context.Contact.Where(x => x.Active == true && x.ObjectType == "EMP").ToList();

                            #endregion

                            //Nếu có điều kiện
                            if (listNotifiCondition.Count > 0)
                            {
                                //Do something...
                            }
                            //Nếu không có điều kiện
                            else
                            {
                                //Nếu gửi nội bộ
                                if (notifiSetting.SendInternal)
                                {
                                    //Nếu gửi bằng email
                                    if (notifiSetting.IsEmail)
                                    {
                                        #region Lấy danh sách email cần gửi thông báo

                                        var listEmailSendTo = new List<string>();
                                        var listAllUser = context.User.ToList();

                                        #region Lấy email người tham gia

                                        if (notifiSetting.IsParticipant)
                                        {
                                            var listParticipantId = context.QuoteParticipantMapping
                                                .Where(x => x.QuoteId == _quote.QuoteId).Select(y => y.EmployeeId).ToList();

                                            if (listParticipantId.Count > 0)
                                            {
                                                var listEmailParticipant = listAllContact.Where(x =>
                                                        listParticipantId.Contains(x.ObjectId) && x.ObjectType == "EMP")
                                                    .ToList();

                                                listEmailParticipant.ForEach(contact =>
                                                {
                                                    if (!String.IsNullOrEmpty(contact?.WorkEmail))
                                                    {
                                                        listEmailSendTo.Add(contact.WorkEmail.Trim());
                                                    }
                                                    else if (!String.IsNullOrEmpty(contact?.Email))
                                                    {
                                                        listEmailSendTo.Add(contact.Email.Trim());
                                                    }
                                                });
                                            }
                                        }

                                        #endregion

                                        #region Lấy email người phê duyệt

                                        if (notifiSetting.IsApproved)
                                        {
                                            //Người phê duyệt báo giá sẽ phải kiểm tra theo Quy trình phê duyệt báo giá

                                            var listApproved =
                                                GetListEmployeeApproved(context, "PDBG", _quote.ApprovalStep,
                                                    listAllEmployee);

                                            var listEmailManager = listAllContact
                                                .Where(x => listApproved.Contains(x.ObjectId) && x.ObjectType == "EMP")
                                                .Select(y => y.Email).ToList();

                                            listEmailManager.ForEach(emailManager =>
                                            {
                                                if (!String.IsNullOrEmpty(emailManager))
                                                {
                                                    listEmailSendTo.Add(emailManager.Trim());
                                                }
                                            });
                                        }

                                        #endregion

                                        #region Lấy email người tạo

                                        if (notifiSetting.IsCreated)
                                        {
                                            var Email = LayEmaiNguoiTao(listAllUser, listAllContact, _quote.CreatedById);
                                            listEmailSendTo.Add(Email);
                                        }

                                        #endregion

                                        #region Lấy email người phụ trách (Nhân viên bán hàng)

                                        if (notifiSetting.IsPersonIncharge)
                                        {
                                            var email_seller = listAllContact.FirstOrDefault(x =>
                                                x.ObjectId == _quote.Seller && x.ObjectType == "EMP")?.Email;

                                            if (!String.IsNullOrEmpty(email_seller))
                                            {
                                                listEmailSendTo.Add(email_seller.Trim());
                                            }
                                        }

                                        #endregion

                                        #region Lấy email của danh sách người đặc biệt

                                        var listEmailSpecial = LayEmaiNguoiDacBiet(listAllUser, listAllContact, context.NotifiSpecial.ToList(), notifiSetting);
                                        listEmailSpecial.ForEach(email =>
                                        {
                                            if (!String.IsNullOrEmpty(email))
                                            {
                                                listEmailSendTo.Add(email.Trim());
                                            }
                                        });

                                        #endregion

                                        #region Lấy email người tạo bình luận

                                        if (note != null)
                                        {
                                            var _note = note as Note;
                                            var Email = LayEmaiNguoiTao(context.User.ToList(), listAllContact, _note.CreatedById);
                                            listEmailSendTo.Add(Email);
                                        }

                                        #endregion

                                        listEmailSendTo = listEmailSendTo.Distinct().ToList();

                                        #endregion

                                        #region Kiểm tra xem Gửi ngay hay Đặt lịch gửi 

                                        //Gửi ngay
                                        var subject = ReplaceTokenForContent(context, typeModel, _quote,
                                            notifiSetting.EmailTitle, configEntity, note);
                                        var content = ReplaceTokenForContent(context, typeModel, _quote,
                                            notifiSetting.EmailContent, configEntity, note);

                                        #region Build nội dung thay đổi

                                        //string contentModelChange = "";

                                        ////Nếu phải kiểm tra thay đổi của model
                                        //if (checkChange)
                                        //{
                                        //    contentModelChange = ContentModelChange(oldModel, newModel, typeModel);
                                        //}

                                        #endregion

                                        Emailer.SendEmail(context, listEmailSendTo, new List<string>(), new List<string>(), subject, content);

                                        //Đặt lịch gửi             
                                        if (notifiSetting.ObjectBackHourInternal != null && listEmailSendTo.Count > 0)
                                        {
                                        }

                                        #endregion
                                    }
                                }
                            }

                            #endregion
                        }
                    }
                    //Tạo Cơ hội
                    else if (typeModel == TypeModel.Lead)
                    {
                        var _lead = newModel as Lead;

                        if (_lead != null)
                        {
                            #region Kiểm tra xem cấu hình có điều kiện không?

                            var listNotifiCondition = context.NotifiSettingCondition
                                .Where(x => x.NotifiSettingId == notifiSetting.NotifiSettingId).ToList();

                            #region List reference để kiểm tra điều kiện và thông tin dùng để gửi email

                            var listAllEmployee = context.Employee.Where(x => x.Active == true).ToList();
                            var listAllContact = context.Contact.Where(x => x.Active == true && x.ObjectType == "EMP").ToList();

                            #endregion

                            //Nếu có điều kiện
                            if (listNotifiCondition.Count > 0)
                            {
                                //Do something...
                            }
                            //Nếu không có điều kiện
                            else
                            {
                                //Nếu gửi nội bộ
                                if (notifiSetting.SendInternal)
                                {
                                    //Nếu gửi bằng email
                                    if (notifiSetting.IsEmail)
                                    {
                                        #region Lấy danh sách email cần gửi thông báo

                                        var listEmailSendTo = new List<string>();
                                        var listAllUser = context.User.ToList();

                                        #region Lấy email người tham gia (Hiện tại chưa có người tham gia)

                                        if (notifiSetting.IsParticipant)
                                        {
                                        }

                                        #endregion

                                        #region Lấy email người phê duyệt

                                        if (notifiSetting.IsApproved)
                                        {
                                            //Người phê duyệt đơn hàng là người được phân quyền dữ liệu là Quản lý
                                            var listManager = listAllEmployee.Where(x => x.IsManager)
                                                .Select(y => y.EmployeeId).ToList();
                                            var listEmailManager = listAllContact
                                                .Where(x => listManager.Contains(x.ObjectId) && x.ObjectType == "EMP")
                                                .Select(y => y.Email).ToList();

                                            listEmailManager.ForEach(emailManager =>
                                            {
                                                if (!String.IsNullOrEmpty(emailManager))
                                                {
                                                    listEmailSendTo.Add(emailManager.Trim());
                                                }
                                            });
                                        }

                                        #endregion

                                        #region Lấy email người tạo

                                        if (notifiSetting.IsCreated)
                                        {
                                            var Email = LayEmaiNguoiTao(listAllUser, listAllContact, Guid.Parse(_lead.CreatedById));
                                            listEmailSendTo.Add(Email);
                                        }

                                        #endregion

                                        #region Lấy email người phụ trách (Nhân viên bán hàng)

                                        if (notifiSetting.IsPersonIncharge)
                                        {
                                            var email_seller = listAllContact.FirstOrDefault(x =>
                                                x.ObjectId == _lead.PersonInChargeId && x.ObjectType == "EMP")?.Email;

                                            if (!String.IsNullOrEmpty(email_seller))
                                            {
                                                listEmailSendTo.Add(email_seller.Trim());
                                            }
                                        }

                                        #endregion

                                        #region Lấy email của danh sách người đặc biệt

                                        var listEmailSpecial = LayEmaiNguoiDacBiet(listAllUser, listAllContact, context.NotifiSpecial.ToList(), notifiSetting);
                                        listEmailSpecial.ForEach(email =>
                                        {
                                            if (!String.IsNullOrEmpty(email))
                                            {
                                                listEmailSendTo.Add(email.Trim());
                                            }
                                        });

                                        #endregion

                                        listEmailSendTo = listEmailSendTo.Distinct().ToList();

                                        #endregion

                                        #region Kiểm tra xem Gửi ngay hay Đặt lịch gửi 

                                        //Gửi ngay
                                        var subject = ReplaceTokenForContent(context, typeModel, _lead,
                                            notifiSetting.EmailTitle, configEntity);
                                        var content = ReplaceTokenForContent(context, typeModel, _lead,
                                            notifiSetting.EmailContent, configEntity);

                                        #region Build nội dung thay đổi

                                        //string contentModelChange = "";

                                        ////Nếu phải kiểm tra thay đổi của model
                                        //if (checkChange)
                                        //{
                                        //    contentModelChange = ContentModelChange(oldModel, newModel, typeModel);
                                        //}

                                        #endregion

                                        Emailer.SendEmail(context, listEmailSendTo, new List<string>(), new List<string>(), subject, content);

                                        //Đặt lịch gửi             
                                        if (notifiSetting.ObjectBackHourInternal != null && listEmailSendTo.Count > 0)
                                        {
                                        }


                                        #endregion
                                    }
                                }
                            }

                            #endregion
                        }
                    }
                    //Chi tiết cơ hội
                    else if (typeModel == TypeModel.LeadDetail)
                    {
                        var _lead = newModel as Lead;

                        if (_lead != null)
                        {
                            #region Kiểm tra xem cấu hình có điều kiện không?

                            var listNotifiCondition = context.NotifiSettingCondition
                                .Where(x => x.NotifiSettingId == notifiSetting.NotifiSettingId).ToList();

                            #region List reference để kiểm tra điều kiện và thông tin dùng để gửi email

                            var listAllEmployee = context.Employee.Where(x => x.Active == true).ToList();
                            var listAllContact = context.Contact.Where(x => x.Active == true && x.ObjectType == "EMP").ToList();

                            #endregion

                            //Nếu có điều kiện
                            if (listNotifiCondition.Count > 0)
                            {
                                //Do something...
                            }
                            //Nếu không có điều kiện
                            else
                            {
                                //Nếu gửi nội bộ
                                if (notifiSetting.SendInternal)
                                {
                                    //Nếu gửi bằng email
                                    if (notifiSetting.IsEmail)
                                    {
                                        #region Lấy danh sách email cần gửi thông báo

                                        var listEmailSendTo = new List<string>();
                                        var listAllUser = context.User.ToList();

                                        #region Lấy email người tham gia (Hiện tại chưa có người tham gia)

                                        if (notifiSetting.IsParticipant)
                                        {
                                        }

                                        #endregion

                                        #region Lấy email người phê duyệt (Tạo mới thì chưa cần gửi email cho người phê duyệt)

                                        if (notifiSetting.IsApproved)
                                        {
                                            /**
                                         * Người phê duyệt cơ hội là người được phân quyền dữ liệu là Quản lý
                                         * của người phụ trách cơ hội
                                         */

                                            // trường hợp gửi thông báo khi bình luân
                                            if (note != null)
                                            {
                                                var _note = note as Note;

                                                // lấy id của nhân viên bán hàng 
                                                var employeeId =
                                                    context.Lead.FirstOrDefault(x => x.LeadId == _note.ObjectId)?.PersonInChargeId;

                                                // lấy id phong ban thong qua id nhan vien
                                                var organizationId = listAllEmployee.FirstOrDefault(x => x.EmployeeId == employeeId)?
                                                    .OrganizationId;

                                                // lay id nhan vien phu trach cua phong ban
                                                var listManager = listAllEmployee.Where(x => x.IsManager &&
                                                        x.OrganizationId == organizationId)
                                                    .Select(y => y.EmployeeId).ToList();

                                                // lay email cua nhan vien phu trach do
                                                var listEmailManager = listAllContact
                                                    .Where(x => listManager.Contains(x.ObjectId) && x.ObjectType == "EMP")
                                                    .Select(y => y.Email).ToList();

                                                // add email do vao list email send to
                                                listEmailManager.ForEach(emailManager =>
                                                {
                                                    if (!String.IsNullOrEmpty(emailManager))
                                                    {
                                                        listEmailSendTo.Add(emailManager.Trim());
                                                    }
                                                });
                                            }
                                            else
                                            {
                                                // lấy id phong ban thong qua id nhan vien
                                                var organizationId = listAllEmployee.FirstOrDefault(x => x.EmployeeId == empId)?
                                                    .OrganizationId;

                                                // lay id nhan vien phu trach cua phong ban
                                                var listManager = listAllEmployee.Where(x => x.IsManager &&
                                                        x.OrganizationId == organizationId)
                                                    .Select(y => y.EmployeeId).ToList();

                                                // lay email cua nhan vien phu trach do
                                                var listEmailManager = listAllContact
                                                    .Where(x => listManager.Contains(x.ObjectId) && x.ObjectType == "EMP")
                                                    .Select(y => y.Email).ToList();

                                                // add email do vao list email send to
                                                listEmailManager.ForEach(emailManager =>
                                                {
                                                    if (!String.IsNullOrEmpty(emailManager))
                                                    {
                                                        listEmailSendTo.Add(emailManager.Trim());
                                                    }
                                                });
                                            }
                                        }

                                        #endregion

                                        #region Lấy email người tạo

                                        if (notifiSetting.IsCreated)
                                        {
                                            var Email = LayEmaiNguoiTao(listAllUser, listAllContact, Guid.Parse(_lead.CreatedById));
                                            listEmailSendTo.Add(Email);
                                        }

                                        #endregion

                                        #region Lấy email người phụ trách (Nhân viên bán hàng)

                                        if (notifiSetting.IsPersonIncharge)
                                        {
                                            var email_seller = listAllContact.FirstOrDefault(x =>
                                                x.ObjectId == _lead.PersonInChargeId && x.ObjectType == "EMP")?.Email;

                                            if (!String.IsNullOrEmpty(email_seller))
                                            {
                                                listEmailSendTo.Add(email_seller.Trim());
                                            }
                                        }

                                        #endregion

                                        #region Lấy email của danh sách người đặc biệt

                                        var listEmailSpecial = LayEmaiNguoiDacBiet(listAllUser, listAllContact, context.NotifiSpecial.ToList(), notifiSetting);
                                        listEmailSpecial.ForEach(email =>
                                        {
                                            if (!String.IsNullOrEmpty(email))
                                            {
                                                listEmailSendTo.Add(email.Trim());
                                            }
                                        });

                                        #endregion

                                        #region Lấy email người tạo bình luận

                                        if (note != null)
                                        {
                                            var _note = note as Note;
                                            var Email = LayEmaiNguoiTao(context.User.ToList(), listAllContact, _note.CreatedById);
                                            listEmailSendTo.Add(Email);
                                        }

                                        #endregion

                                        listEmailSendTo = listEmailSendTo.Distinct().ToList();

                                        #endregion

                                        #region Kiểm tra xem Gửi ngay hay Đặt lịch gửi 

                                        //Gửi ngay
                                        var subject = ReplaceTokenForContent(context, typeModel, _lead,
                                            notifiSetting.EmailTitle, configEntity, note);
                                        var content = ReplaceTokenForContent(context, typeModel, _lead,
                                            notifiSetting.EmailContent, configEntity, note);

                                        #region Build nội dung thay đổi

                                        //string contentModelChange = "";

                                        ////Nếu phải kiểm tra thay đổi của model
                                        //if (checkChange)
                                        //{
                                        //    contentModelChange = ContentModelChange(oldModel, newModel, typeModel);
                                        //}

                                        #endregion

                                        Emailer.SendEmail(context, listEmailSendTo, new List<string>(), new List<string>(), subject, content);

                                        //Đặt lịch gửi             
                                        if (notifiSetting.ObjectBackHourInternal != null && listEmailSendTo.Count > 0)
                                        {
                                        }

                                        #endregion
                                    }
                                }
                            }

                            #endregion
                        }
                    }
                    //Tạo hồ sơ thầu
                    else if (typeModel == TypeModel.SaleBidding)
                    {
                        var _saleBidding = newModel as SaleBidding;

                        if (_saleBidding != null)
                        {
                            #region Kiểm tra xem cấu hình có điều kiện không?

                            var listNotifiCondition = context.NotifiSettingCondition
                                .Where(x => x.NotifiSettingId == notifiSetting.NotifiSettingId).ToList();

                            #region List reference để kiểm tra điều kiện và thông tin dùng để gửi email

                            var listAllEmployee = context.Employee.Where(x => x.Active == true).ToList();
                            var listAllContact = context.Contact.Where(x => x.Active == true && x.ObjectType == "EMP").ToList();

                            #endregion

                            //Nếu có điều kiện
                            if (listNotifiCondition.Count > 0)
                            {
                                //Do something...
                            }
                            //Nếu không có điều kiện
                            else
                            {
                                //Nếu gửi nội bộ
                                if (notifiSetting.SendInternal)
                                {
                                    //Nếu gửi bằng email
                                    if (notifiSetting.IsEmail)
                                    {
                                        #region Lấy danh sách email cần gửi thông báo

                                        var listEmailSendTo = new List<string>();
                                        var listAllUser = context.User.ToList();

                                        #region Lấy email người tham gia

                                        if (notifiSetting.IsParticipant)
                                        {
                                            var listParticipantId = context.SaleBiddingEmployeeMapping
                                                .Where(x => x.SaleBiddingId == _saleBidding.SaleBiddingId).Select(y => y.EmployeeId).ToList();

                                            if (listParticipantId.Count > 0)
                                            {
                                                var listEmailParticipant = listAllContact.Where(x =>
                                                        listParticipantId.Contains(x.ObjectId) && x.ObjectType == "EMP")
                                                    .ToList();

                                                listEmailParticipant.ForEach(contact =>
                                                {
                                                    if (!String.IsNullOrEmpty(contact?.WorkEmail))
                                                    {
                                                        listEmailSendTo.Add(contact.WorkEmail.Trim());
                                                    }
                                                    else if (!String.IsNullOrEmpty(contact?.Email))
                                                    {
                                                        listEmailSendTo.Add(contact.Email.Trim());
                                                    }
                                                });
                                            }
                                        }

                                        #endregion

                                        #region Lấy email người phê duyệt (Tạo mới thì chưa cần gửi email cho người phê duyệt)

                                        if (notifiSetting.IsApproved)
                                        {
                                            //Người phê duyệt đơn hàng là người được phân quyền dữ liệu là Quản lý
                                            var listManager = listAllEmployee.Where(x => x.IsManager)
                                                .Select(y => y.EmployeeId).ToList();
                                            var listEmailManager = listAllContact
                                                .Where(x => listManager.Contains(x.ObjectId) && x.ObjectType == "EMP")
                                                .Select(y => y.Email).ToList();

                                            listEmailManager.ForEach(emailManager =>
                                            {
                                                if (!String.IsNullOrEmpty(emailManager))
                                                {
                                                    listEmailSendTo.Add(emailManager.Trim());
                                                }
                                            });
                                        }

                                        #endregion

                                        #region Lấy email người tạo

                                        if (notifiSetting.IsCreated)
                                        {
                                            var Email = LayEmaiNguoiTao(listAllUser, listAllContact, _saleBidding.CreatedById);
                                            listEmailSendTo.Add(Email);
                                        }

                                        #endregion

                                        #region Lấy email người phụ trách (Nhân viên bán hàng)

                                        if (notifiSetting.IsPersonIncharge)
                                        {
                                            var email_seller = listAllContact.FirstOrDefault(x =>
                                                x.ObjectId == _saleBidding.PersonInChargeId && x.ObjectType == "EMP")?.Email;

                                            if (!String.IsNullOrEmpty(email_seller))
                                            {
                                                listEmailSendTo.Add(email_seller.Trim());
                                            }
                                        }

                                        #endregion

                                        #region Lấy email của danh sách người đặc biệt

                                        var listEmailSpecial = LayEmaiNguoiDacBiet(listAllUser, listAllContact, context.NotifiSpecial.ToList(), notifiSetting);
                                        listEmailSpecial.ForEach(email =>
                                        {
                                            if (!String.IsNullOrEmpty(email))
                                            {
                                                listEmailSendTo.Add(email.Trim());
                                            }
                                        });

                                        #endregion

                                        listEmailSendTo = listEmailSendTo.Distinct().ToList();

                                        #endregion

                                        #region Kiểm tra xem Gửi ngay hay Đặt lịch gửi 

                                        //Gửi ngay
                                        var subject = ReplaceTokenForContent(context, typeModel, _saleBidding,
                                            notifiSetting.EmailTitle, configEntity);
                                        var content = ReplaceTokenForContent(context, typeModel, _saleBidding,
                                            notifiSetting.EmailContent, configEntity);

                                        #region Build nội dung thay đổi

                                        //string contentModelChange = "";

                                        ////Nếu phải kiểm tra thay đổi của model
                                        //if (checkChange)
                                        //{
                                        //    contentModelChange = ContentModelChange(oldModel, newModel, typeModel);
                                        //}

                                        #endregion

                                        Emailer.SendEmail(context, listEmailSendTo, new List<string>(), new List<string>(), subject, content);

                                        //Đặt lịch gửi             
                                        if (notifiSetting.ObjectBackHourInternal != null && listEmailSendTo.Count > 0)
                                        {
                                        }

                                        #endregion
                                    }
                                }
                            }

                            #endregion
                        }
                    }
                    //Chi tiết hồ sơ thầu
                    else if (typeModel == TypeModel.SaleBiddingDetail)
                    {
                        var _saleBidding = newModel as SaleBidding;

                        if (_saleBidding != null)
                        {
                            #region Kiểm tra xem cấu hình có điều kiện không?

                            var listNotifiCondition = context.NotifiSettingCondition
                                .Where(x => x.NotifiSettingId == notifiSetting.NotifiSettingId).ToList();

                            #region List reference để kiểm tra điều kiện và thông tin dùng để gửi email

                            var listAllEmployee = context.Employee.Where(x => x.Active == true).ToList();
                            var listAllContact = context.Contact.Where(x => x.Active == true && x.ObjectType == "EMP").ToList();

                            #endregion

                            //Nếu có điều kiện
                            if (listNotifiCondition.Count > 0)
                            {
                                //Do something...
                            }
                            //Nếu không có điều kiện
                            else
                            {
                                //Nếu gửi nội bộ
                                if (notifiSetting.SendInternal)
                                {
                                    //Nếu gửi bằng email
                                    if (notifiSetting.IsEmail)
                                    {
                                        #region Lấy danh sách email cần gửi thông báo

                                        var listEmailSendTo = new List<string>();
                                        var listAllUser = context.User.ToList();

                                        #region Lấy email người tham gia

                                        if (notifiSetting.IsParticipant)
                                        {
                                            var listParticipantId = context.SaleBiddingEmployeeMapping
                                                .Where(x => x.SaleBiddingId == _saleBidding.SaleBiddingId).Select(y => y.EmployeeId).ToList();

                                            if (listParticipantId.Count > 0)
                                            {
                                                var listEmailParticipant = listAllContact.Where(x =>
                                                        listParticipantId.Contains(x.ObjectId) && x.ObjectType == "EMP")
                                                    .ToList();

                                                listEmailParticipant.ForEach(contact =>
                                                {
                                                    if (!String.IsNullOrEmpty(contact?.WorkEmail))
                                                    {
                                                        listEmailSendTo.Add(contact.WorkEmail.Trim());
                                                    }
                                                    else if (!String.IsNullOrEmpty(contact?.Email))
                                                    {
                                                        listEmailSendTo.Add(contact.Email.Trim());
                                                    }
                                                });
                                            }
                                        }

                                        #endregion

                                        #region Lấy email người phê duyệt (Tạo mới thì chưa cần gửi email cho người phê duyệt)

                                        if (notifiSetting.IsApproved)
                                        {
                                            /**
                                             * Người phê duyệt đơn hàng được thông báo
                                             * là người được phân quyền dữ liệu là Quản lý
                                             * của nhân viên bán hàng của đơn hàng
                                             */

                                            // trường hợp gửi thông báo khi bình luân
                                            if (note != null)
                                            {
                                                var _note = note as Note;

                                                // lấy id của nhân viên bán hàng 
                                                var employeeId =
                                                    context.SaleBidding.FirstOrDefault(x => x.SaleBiddingId == _note.ObjectId)?.PersonInChargeId;

                                                // lấy id phong ban thong qua id nhan vien
                                                var organizationId = listAllEmployee.FirstOrDefault(x => x.EmployeeId == employeeId)?
                                                    .OrganizationId;

                                                // lay id nhan vien phu trach cua phong ban
                                                var listManager = listAllEmployee.Where(x => x.IsManager &&
                                                        x.OrganizationId == organizationId)
                                                    .Select(y => y.EmployeeId).ToList();

                                                // lay email cua nhan vien phu trach do
                                                var listEmailManager = listAllContact
                                                    .Where(x => listManager.Contains(x.ObjectId) && x.ObjectType == "EMP")
                                                    .Select(y => y.Email).ToList();

                                                // add email do vao list email send to
                                                listEmailManager.ForEach(emailManager =>
                                                {
                                                    if (!String.IsNullOrEmpty(emailManager))
                                                    {
                                                        listEmailSendTo.Add(emailManager.Trim());
                                                    }
                                                });
                                            }
                                            else
                                            {
                                                // lấy id phong ban thong qua id nhan vien
                                                var organizationId = listAllEmployee
                                                    .FirstOrDefault(x => x.EmployeeId == empId)?
                                                    .OrganizationId;

                                                // lay id nhan vien phu trach cua phong ban
                                                var listManager = listAllEmployee.Where(x => x.IsManager &&
                                                        x.OrganizationId == organizationId)
                                                    .Select(y => y.EmployeeId).ToList();

                                                // lay email cua nhan vien phu trach do
                                                var listEmailManager = listAllContact
                                                    .Where(x => listManager.Contains(x.ObjectId) && x.ObjectType == "EMP")
                                                    .Select(y => y.Email).ToList();

                                                // add email do vao list email send to
                                                listEmailManager.ForEach(emailManager =>
                                                {
                                                    if (!String.IsNullOrEmpty(emailManager))
                                                    {
                                                        listEmailSendTo.Add(emailManager.Trim());
                                                    }
                                                });
                                            }
                                        }

                                        #endregion

                                        #region Lấy email người tạo

                                        if (notifiSetting.IsCreated)
                                        {
                                            var Email = LayEmaiNguoiTao(listAllUser, listAllContact, _saleBidding.CreatedById);
                                            listEmailSendTo.Add(Email);
                                        }

                                        #endregion

                                        #region Lấy email người phụ trách (Nhân viên bán hàng)

                                        if (notifiSetting.IsPersonIncharge)
                                        {
                                            var email_seller = listAllContact.FirstOrDefault(x =>
                                                x.ObjectId == _saleBidding.PersonInChargeId && x.ObjectType == "EMP")?.Email;

                                            if (!String.IsNullOrEmpty(email_seller))
                                            {
                                                listEmailSendTo.Add(email_seller.Trim());
                                            }
                                        }

                                        #endregion

                                        #region Lấy email của danh sách người đặc biệt

                                        var listEmailSpecial = LayEmaiNguoiDacBiet(listAllUser, listAllContact, context.NotifiSpecial.ToList(), notifiSetting);
                                        listEmailSpecial.ForEach(email =>
                                        {
                                            if (!String.IsNullOrEmpty(email))
                                            {
                                                listEmailSendTo.Add(email.Trim());
                                            }
                                        });

                                        #endregion

                                        #region Lấy email người tạo bình luận

                                        if (note != null)
                                        {
                                            var _note = note as Note;
                                            var Email = LayEmaiNguoiTao(context.User.ToList(), listAllContact, _note.CreatedById);
                                            listEmailSendTo.Add(Email);
                                        }

                                        #endregion

                                        listEmailSendTo = listEmailSendTo.Distinct().ToList();

                                        #endregion

                                        #region Kiểm tra xem Gửi ngay hay Đặt lịch gửi 

                                        //Gửi ngay
                                        var subject = ReplaceTokenForContent(context, typeModel, _saleBidding,
                                            notifiSetting.EmailTitle, configEntity, note);
                                        var content = ReplaceTokenForContent(context, typeModel, _saleBidding,
                                            notifiSetting.EmailContent, configEntity, note);

                                        #region Build nội dung thay đổi

                                        //string contentModelChange = "";

                                        ////Nếu phải kiểm tra thay đổi của model
                                        //if (checkChange)
                                        //{
                                        //    contentModelChange = ContentModelChange(oldModel, newModel, typeModel);
                                        //}

                                        #endregion

                                        Emailer.SendEmail(context, listEmailSendTo, new List<string>(), new List<string>(), subject, content);

                                        //Đặt lịch gửi             
                                        if (notifiSetting.ObjectBackHourInternal != null && listEmailSendTo.Count > 0)
                                        {
                                        }

                                        #endregion
                                    }
                                }
                            }

                            #endregion
                        }
                    }
                    //Tạo hợp đồng bán
                    else if (typeModel == TypeModel.Contract)
                    {
                        var _contract = newModel as Contract;

                        if (_contract != null)
                        {
                            #region Kiểm tra xem cấu hình có điều kiện không?

                            var listNotifiCondition = context.NotifiSettingCondition
                                .Where(x => x.NotifiSettingId == notifiSetting.NotifiSettingId).ToList();

                            #region List reference để kiểm tra điều kiện và thông tin dùng để gửi email

                            var listAllEmployee = context.Employee.Where(x => x.Active == true).ToList();
                            var listAllContact = context.Contact.Where(x => x.Active == true && x.ObjectType == "EMP").ToList();

                            #endregion

                            //Nếu có điều kiện
                            if (listNotifiCondition.Count > 0)
                            {
                                //Do something...
                            }
                            //Nếu không có điều kiện
                            else
                            {
                                //Nếu gửi nội bộ
                                if (notifiSetting.SendInternal)
                                {
                                    //Nếu gửi bằng email
                                    if (notifiSetting.IsEmail)
                                    {
                                        #region Lấy danh sách email cần gửi thông báo

                                        var listEmailSendTo = new List<string>();
                                        var listAllUser = context.User.ToList();

                                        #region Lấy email người tham gia (Hiện tại chưa có người tham gia)

                                        if (notifiSetting.IsParticipant)
                                        {
                                        }

                                        #endregion

                                        #region Lấy email người phê duyệt (Tạo mới thì chưa cần gửi email cho người phê duyệt)

                                        if (notifiSetting.IsApproved)
                                        {
                                            //Người phê duyệt đơn hàng là người được phân quyền dữ liệu là Quản lý
                                            var listManager = listAllEmployee.Where(x => x.IsManager)
                                                .Select(y => y.EmployeeId).ToList();
                                            var listEmailManager = listAllContact
                                                .Where(x => listManager.Contains(x.ObjectId) && x.ObjectType == "EMP")
                                                .Select(y => y.Email).ToList();

                                            listEmailManager.ForEach(emailManager =>
                                            {
                                                if (!String.IsNullOrEmpty(emailManager))
                                                {
                                                    listEmailSendTo.Add(emailManager.Trim());
                                                }
                                            });
                                        }

                                        #endregion

                                        #region Lấy email người tạo

                                        if (notifiSetting.IsCreated)
                                        {
                                            var Email = LayEmaiNguoiTao(listAllUser, listAllContact, _contract.CreatedById);
                                            listEmailSendTo.Add(Email);
                                        }

                                        #endregion

                                        #region Lấy email người phụ trách (Nhân viên bán hàng)

                                        if (notifiSetting.IsPersonIncharge)
                                        {
                                            var email_seller = listAllContact.FirstOrDefault(x =>
                                                x.ObjectId == _contract.EmployeeId && x.ObjectType == "EMP")?.Email;

                                            if (!String.IsNullOrEmpty(email_seller))
                                            {
                                                listEmailSendTo.Add(email_seller.Trim());
                                            }
                                        }

                                        #endregion

                                        #region Lấy email của danh sách người đặc biệt

                                        var listEmailSpecial = LayEmaiNguoiDacBiet(listAllUser, listAllContact, context.NotifiSpecial.ToList(), notifiSetting);
                                        listEmailSpecial.ForEach(email =>
                                        {
                                            if (!String.IsNullOrEmpty(email))
                                            {
                                                listEmailSendTo.Add(email.Trim());
                                            }
                                        });

                                        #endregion

                                        listEmailSendTo = listEmailSendTo.Distinct().ToList();

                                        #endregion

                                        #region Kiểm tra xem Gửi ngay hay Đặt lịch gửi 

                                        //Gửi ngay
                                        var subject = ReplaceTokenForContent(context, typeModel, _contract,
                                            notifiSetting.EmailTitle, configEntity);
                                        var content = ReplaceTokenForContent(context, typeModel, _contract,
                                            notifiSetting.EmailContent, configEntity);

                                        #region Build nội dung thay đổi

                                        //string contentModelChange = "";

                                        ////Nếu phải kiểm tra thay đổi của model
                                        //if (checkChange)
                                        //{
                                        //    contentModelChange = ContentModelChange(oldModel, newModel, typeModel);
                                        //}

                                        #endregion

                                        Emailer.SendEmail(context, listEmailSendTo, new List<string>(), new List<string>(), subject, content);

                                        //Đặt lịch gửi             
                                        if (notifiSetting.ObjectBackHourInternal != null && listEmailSendTo.Count > 0)
                                        {
                                        }

                                        #endregion
                                    }
                                }
                            }

                            #endregion
                        }
                    }
                    //Chi tiết hợp đồng bán
                    else if (typeModel == TypeModel.ContractDetail)
                    {
                        var _contract = newModel as Contract;

                        if (_contract != null)
                        {
                            #region Kiểm tra xem cấu hình có điều kiện không?

                            var listNotifiCondition = context.NotifiSettingCondition
                                .Where(x => x.NotifiSettingId == notifiSetting.NotifiSettingId).ToList();

                            #region List reference để kiểm tra điều kiện và thông tin dùng để gửi email

                            var listAllEmployee = context.Employee.Where(x => x.Active == true).ToList();
                            var listAllContact = context.Contact.Where(x => x.Active == true && x.ObjectType == "EMP").ToList();

                            #endregion

                            //Nếu có điều kiện
                            if (listNotifiCondition.Count > 0)
                            {
                                //Do something...
                            }
                            //Nếu không có điều kiện
                            else
                            {
                                //Nếu gửi nội bộ
                                if (notifiSetting.SendInternal)
                                {
                                    //Nếu gửi bằng email
                                    if (notifiSetting.IsEmail)
                                    {
                                        #region Lấy danh sách email cần gửi thông báo

                                        var listEmailSendTo = new List<string>();
                                        var listAllUser = context.User.ToList();

                                        #region Lấy email người tham gia (Hiện tại chưa có người tham gia)

                                        if (notifiSetting.IsParticipant)
                                        {
                                        }

                                        #endregion

                                        #region Lấy email người phê duyệt (Tạo mới thì chưa cần gửi email cho người phê duyệt)

                                        if (notifiSetting.IsApproved)
                                        {
                                            /**
                                             * Người phê duyệt đơn hàng được thông báo
                                             * là người được phân quyền dữ liệu là Quản lý
                                             * của nhân viên bán hàng của đơn hàng
                                             */

                                            // trường hợp gửi thông báo khi bình luân
                                            if (note != null)
                                            {
                                                var _note = note as Note;

                                                // lấy id của nhân viên bán hàng 
                                                var employeeId =
                                                    context.Contract.FirstOrDefault(x => x.ContractId == _note.ObjectId)?.EmployeeId;

                                                // lấy id phong ban thong qua id nhan vien
                                                var organizationId = listAllEmployee.FirstOrDefault(x => x.EmployeeId == employeeId)?
                                                    .OrganizationId;

                                                // lay id nhan vien phu trach cua phong ban
                                                var listManager = listAllEmployee.Where(x => x.IsManager &&
                                                        x.OrganizationId == organizationId)
                                                    .Select(y => y.EmployeeId).ToList();

                                                // lay email cua nhan vien phu trach do
                                                var listEmailManager = listAllContact
                                                    .Where(x => listManager.Contains(x.ObjectId) && x.ObjectType == "EMP")
                                                    .Select(y => y.Email).ToList();

                                                // add email do vao list email send to
                                                listEmailManager.ForEach(emailManager =>
                                                {
                                                    if (!String.IsNullOrEmpty(emailManager))
                                                    {
                                                        listEmailSendTo.Add(emailManager.Trim());
                                                    }
                                                });
                                            }
                                            else
                                            {
                                                // lấy id phong ban thong qua id nhan vien
                                                var organizationId = listAllEmployee
                                                    .FirstOrDefault(x => x.EmployeeId == empId)?
                                                    .OrganizationId;

                                                // lay id nhan vien phu trach cua phong ban
                                                var listManager = listAllEmployee.Where(x => x.IsManager &&
                                                        x.OrganizationId == organizationId)
                                                    .Select(y => y.EmployeeId).ToList();

                                                // lay email cua nhan vien phu trach do
                                                var listEmailManager = listAllContact
                                                    .Where(x => listManager.Contains(x.ObjectId) && x.ObjectType == "EMP")
                                                    .Select(y => y.Email).ToList();

                                                // add email do vao list email send to
                                                listEmailManager.ForEach(emailManager =>
                                                {
                                                    if (!String.IsNullOrEmpty(emailManager))
                                                    {
                                                        listEmailSendTo.Add(emailManager.Trim());
                                                    }
                                                });
                                            }
                                        }

                                        #endregion

                                        #region Lấy email người tạo

                                        if (notifiSetting.IsCreated)
                                        {
                                            var Email = LayEmaiNguoiTao(listAllUser, listAllContact, _contract.CreatedById);
                                            listEmailSendTo.Add(Email);
                                        }

                                        #endregion

                                        #region Lấy email người phụ trách (Nhân viên bán hàng)

                                        if (notifiSetting.IsPersonIncharge)
                                        {
                                            var email_seller = listAllContact.FirstOrDefault(x =>
                                                x.ObjectId == _contract.EmployeeId && x.ObjectType == "EMP")?.Email;

                                            if (!String.IsNullOrEmpty(email_seller))
                                            {
                                                listEmailSendTo.Add(email_seller.Trim());
                                            }
                                        }

                                        #endregion

                                        #region Lấy email của danh sách người đặc biệt

                                        var listEmailSpecial = LayEmaiNguoiDacBiet(listAllUser, listAllContact, context.NotifiSpecial.ToList(), notifiSetting);
                                        listEmailSpecial.ForEach(email =>
                                        {
                                            if (!String.IsNullOrEmpty(email))
                                            {
                                                listEmailSendTo.Add(email.Trim());
                                            }
                                        });

                                        #endregion

                                        #region Lấy email người tạo bình luận

                                        if (note != null)
                                        {
                                            var _note = note as Note;
                                            var Email = LayEmaiNguoiTao(context.User.ToList(), listAllContact, _note.CreatedById);
                                            listEmailSendTo.Add(Email);
                                        }

                                        #endregion

                                        listEmailSendTo = listEmailSendTo.Distinct().ToList();

                                        #endregion

                                        #region Kiểm tra xem Gửi ngay hay Đặt lịch gửi 

                                        //Gửi ngay
                                        var subject = ReplaceTokenForContent(context, typeModel, _contract,
                                            notifiSetting.EmailTitle, configEntity, note);
                                        var content = ReplaceTokenForContent(context, typeModel, _contract,
                                            notifiSetting.EmailContent, configEntity, note);

                                        #region Build nội dung thay đổi

                                        //string contentModelChange = "";

                                        ////Nếu phải kiểm tra thay đổi của model
                                        //if (checkChange)
                                        //{
                                        //    contentModelChange = ContentModelChange(oldModel, newModel, typeModel);
                                        //}

                                        #endregion
                                        Emailer.SendEmail(context, listEmailSendTo, new List<string>(), new List<string>(), subject, content);

                                        //Đặt lịch gửi             
                                        if (notifiSetting.ObjectBackHourInternal != null && listEmailSendTo.Count > 0)
                                        {
                                        }

                                        #endregion
                                    }
                                }
                            }

                            #endregion
                        }
                    }
                    //Tạo hóa đơn
                    else if (typeModel == TypeModel.BillSale)
                    {
                        var _bill = newModel as BillOfSale;

                        if (_bill != null)
                        {

                            #region Kiểm tra xem cấu hình có điều kiện không?

                            var listNotifiCondition = context.NotifiSettingCondition
                                .Where(x => x.NotifiSettingId == notifiSetting.NotifiSettingId).ToList();

                            #region List reference để kiểm tra điều kiện và thông tin dùng để gửi email

                            var listAllEmployee = context.Employee.Where(x => x.Active == true).ToList();
                            var listAllContact = context.Contact.Where(x => x.Active == true && x.ObjectType == "EMP").ToList();

                            #endregion

                            //Nếu có điều kiện
                            if (listNotifiCondition.Count > 0)
                            {
                                //Do something...
                            }
                            //Nếu không có điều kiện
                            else
                            {
                                //Nếu gửi nội bộ
                                if (notifiSetting.SendInternal)
                                {
                                    //Nếu gửi bằng email
                                    if (notifiSetting.IsEmail)
                                    {
                                        #region Lấy danh sách email cần gửi thông báo

                                        var listEmailSendTo = new List<string>();
                                        var listAllUser = context.User.ToList();

                                        #region Lấy email người tham gia (Hiện tại chưa có người tham gia)

                                        if (notifiSetting.IsParticipant)
                                        {
                                        }

                                        #endregion

                                        #region Lấy email người phê duyệt (Tạo mới thì chưa cần gửi email cho người phê duyệt)

                                        if (notifiSetting.IsApproved)
                                        {
                                            //Người phê duyệt đơn hàng là người được phân quyền dữ liệu là Quản lý
                                            var listManager = listAllEmployee.Where(x => x.IsManager)
                                                .Select(y => y.EmployeeId).ToList();
                                            var listEmailManager = listAllContact
                                                .Where(x => listManager.Contains(x.ObjectId) && x.ObjectType == "EMP")
                                                .Select(y => y.Email).ToList();

                                            listEmailManager.ForEach(emailManager =>
                                            {
                                                if (!String.IsNullOrEmpty(emailManager))
                                                {
                                                    listEmailSendTo.Add(emailManager.Trim());
                                                }
                                            });
                                        }

                                        #endregion

                                        #region Lấy email người tạo

                                        if (notifiSetting.IsCreated)
                                        {
                                            var Email = LayEmaiNguoiTao(listAllUser, listAllContact, _bill.CreatedById.Value);
                                            listEmailSendTo.Add(Email);
                                        }

                                        #endregion

                                        #region Lấy email người phụ trách (Nhân viên bán hàng)

                                        if (notifiSetting.IsPersonIncharge)
                                        {
                                            var email_seller = listAllContact.FirstOrDefault(x =>
                                                x.ObjectId == _bill.EmployeeId && x.ObjectType == "EMP")?.Email;

                                            if (!String.IsNullOrEmpty(email_seller))
                                            {
                                                listEmailSendTo.Add(email_seller.Trim());
                                            }
                                        }

                                        #endregion

                                        #region Lấy email của danh sách người đặc biệt

                                        var listEmailSpecial = LayEmaiNguoiDacBiet(listAllUser, listAllContact, context.NotifiSpecial.ToList(), notifiSetting);
                                        listEmailSpecial.ForEach(email =>
                                        {
                                            if (!String.IsNullOrEmpty(email))
                                            {
                                                listEmailSendTo.Add(email.Trim());
                                            }
                                        });

                                        #endregion

                                        listEmailSendTo = listEmailSendTo.Distinct().ToList();

                                        #endregion

                                        #region Kiểm tra xem Gửi ngay hay Đặt lịch gửi 

                                        //Gửi ngay
                                        var subject = ReplaceTokenForContent(context, typeModel, _bill,
                                            notifiSetting.EmailTitle, configEntity);
                                        var content = ReplaceTokenForContent(context, typeModel, _bill,
                                            notifiSetting.EmailContent, configEntity);

                                        #region Build nội dung thay đổi

                                        //string contentModelChange = "";

                                        ////Nếu phải kiểm tra thay đổi của model
                                        //if (checkChange)
                                        //{
                                        //    contentModelChange = ContentModelChange(oldModel, newModel, typeModel);
                                        //}

                                        #endregion

                                        Emailer.SendEmail(context, listEmailSendTo, new List<string>(), new List<string>(), subject, content);

                                        //Đặt lịch gửi             
                                        if (notifiSetting.ObjectBackHourInternal != null && listEmailSendTo.Count > 0)
                                        {

                                        }

                                        #endregion
                                    }
                                }
                            }

                            #endregion
                        }

                    }
                    //Chi tiết hóa đơn
                    else if (typeModel == TypeModel.BillSaleDetail)
                    {
                        var _bill = newModel as BillOfSale;

                        if (_bill != null)
                        {
                            #region Kiểm tra xem cấu hình có điều kiện không?

                            var listNotifiCondition = context.NotifiSettingCondition
                                .Where(x => x.NotifiSettingId == notifiSetting.NotifiSettingId).ToList();

                            #region List reference để kiểm tra điều kiện và thông tin dùng để gửi email

                            var listAllEmployee = context.Employee.Where(x => x.Active == true).ToList();
                            var listAllContact = context.Contact.Where(x => x.Active == true && x.ObjectType == "EMP").ToList();

                            #endregion

                            //Nếu có điều kiện
                            if (listNotifiCondition.Count > 0)
                            {
                                //Do something...
                            }
                            //Nếu không có điều kiện
                            else
                            {
                                //Nếu gửi nội bộ
                                if (notifiSetting.SendInternal)
                                {
                                    //Nếu gửi bằng email
                                    if (notifiSetting.IsEmail)
                                    {
                                        #region Lấy danh sách email cần gửi thông báo

                                        var listEmailSendTo = new List<string>();
                                        var listAllUser = context.User.ToList();

                                        #region Lấy email người tham gia (Hiện tại chưa có người tham gia)

                                        if (notifiSetting.IsParticipant)
                                        {
                                        }

                                        #endregion

                                        #region Lấy email người phê duyệt (Tạo mới thì chưa cần gửi email cho người phê duyệt)

                                        if (notifiSetting.IsApproved)
                                        {
                                            /**
                                             * Người phê duyệt đơn hàng được thông báo
                                             * là người được phân quyền dữ liệu là Quản lý
                                             * của nhân viên bán hàng của đơn hàng
                                             */

                                            // trường hợp gửi thông báo khi bình luân
                                            if (note != null)
                                            {
                                                var _note = note as Note;

                                                // lấy id của nhân viên bán hàng 
                                                var employeeId =
                                                    context.BillOfSale.FirstOrDefault(x => x.BillOfSaLeId == _note.ObjectId)?.EmployeeId;

                                                // lấy id phong ban thong qua id nhan vien
                                                var organizationId = listAllEmployee.FirstOrDefault(x => x.EmployeeId == employeeId)?
                                                    .OrganizationId;

                                                // lay id nhan vien phu trach cua phong ban
                                                var listManager = listAllEmployee.Where(x => x.IsManager &&
                                                        x.OrganizationId == organizationId)
                                                    .Select(y => y.EmployeeId).ToList();

                                                // lay email cua nhan vien phu trach do
                                                var listEmailManager = listAllContact
                                                    .Where(x => listManager.Contains(x.ObjectId) && x.ObjectType == "EMP")
                                                    .Select(y => y.Email).ToList();

                                                // add email do vao list email send to
                                                listEmailManager.ForEach(emailManager =>
                                                {
                                                    if (!String.IsNullOrEmpty(emailManager))
                                                    {
                                                        listEmailSendTo.Add(emailManager.Trim());
                                                    }
                                                });
                                            }
                                            else
                                            {
                                                // lấy id phong ban thong qua id nhan vien
                                                var organizationId = listAllEmployee
                                                    .FirstOrDefault(x => x.EmployeeId == empId)?
                                                    .OrganizationId;

                                                // lay id nhan vien phu trach cua phong ban
                                                var listManager = listAllEmployee.Where(x => x.IsManager &&
                                                        x.OrganizationId == organizationId)
                                                    .Select(y => y.EmployeeId).ToList();

                                                // lay email cua nhan vien phu trach do
                                                var listEmailManager = listAllContact
                                                    .Where(x => listManager.Contains(x.ObjectId) && x.ObjectType == "EMP")
                                                    .Select(y => y.Email).ToList();

                                                // add email do vao list email send to
                                                listEmailManager.ForEach(emailManager =>
                                                {
                                                    if (!String.IsNullOrEmpty(emailManager))
                                                    {
                                                        listEmailSendTo.Add(emailManager.Trim());
                                                    }
                                                });
                                            }
                                        }

                                        #endregion

                                        #region Lấy email người tạo

                                        if (notifiSetting.IsCreated)
                                        {
                                            var Email = LayEmaiNguoiTao(listAllUser, listAllContact, _bill.CreatedById.Value);
                                            listEmailSendTo.Add(Email);
                                        }

                                        #endregion

                                        #region Lấy email người phụ trách (Nhân viên bán hàng)

                                        if (notifiSetting.IsPersonIncharge)
                                        {
                                            var email_seller = listAllContact.FirstOrDefault(x =>
                                                x.ObjectId == _bill.EmployeeId && x.ObjectType == "EMP")?.Email;

                                            if (!String.IsNullOrEmpty(email_seller))
                                            {
                                                listEmailSendTo.Add(email_seller.Trim());
                                            }
                                        }

                                        #endregion

                                        #region Lấy email của danh sách người đặc biệt

                                        var listEmailSpecial = LayEmaiNguoiDacBiet(listAllUser, listAllContact, context.NotifiSpecial.ToList(), notifiSetting);
                                        listEmailSpecial.ForEach(email =>
                                        {
                                            if (!String.IsNullOrEmpty(email))
                                            {
                                                listEmailSendTo.Add(email.Trim());
                                            }
                                        });

                                        #endregion

                                        #region Lấy email người tạo bình luận

                                        if (note != null)
                                        {
                                            var _note = note as Note;
                                            var Email = LayEmaiNguoiTao(context.User.ToList(), listAllContact, _note.CreatedById);
                                            listEmailSendTo.Add(Email);
                                        }

                                        #endregion

                                        listEmailSendTo = listEmailSendTo.Distinct().ToList();

                                        #endregion

                                        #region Kiểm tra xem Gửi ngay hay Đặt lịch gửi 

                                        //Gửi ngay
                                        var subject = ReplaceTokenForContent(context, typeModel, _bill,
                                            notifiSetting.EmailTitle, configEntity, note);
                                        var content = ReplaceTokenForContent(context, typeModel, _bill,
                                            notifiSetting.EmailContent, configEntity, note);

                                        #region Build nội dung thay đổi

                                        //string contentModelChange = "";

                                        ////Nếu phải kiểm tra thay đổi của model
                                        //if (checkChange)
                                        //{
                                        //    contentModelChange = ContentModelChange(oldModel, newModel, typeModel);
                                        //}

                                        #endregion

                                        Emailer.SendEmail(context, listEmailSendTo, new List<string>(), new List<string>(), subject, content);

                                        //Đặt lịch gửi             
                                        if (notifiSetting.ObjectBackHourInternal != null && listEmailSendTo.Count > 0)
                                        {
                                        }

                                        #endregion
                                    }
                                }
                            }

                            #endregion
                        }
                    }
                    //Tạo đề xuất mua hàng
                    else if (typeModel == TypeModel.ProcurementRequest)
                    {
                        var _procurementRequest = newModel as ProcurementRequest;

                        if (_procurementRequest != null)
                        {
                            #region Kiểm tra xem cấu hình có điều kiện không?

                            var listNotifiCondition = context.NotifiSettingCondition
                                .Where(x => x.NotifiSettingId == notifiSetting.NotifiSettingId).ToList();

                            #region List reference để kiểm tra điều kiện và thông tin dùng để gửi email

                            var listAllEmployee = context.Employee.Where(x => x.Active == true).ToList();
                            var listAllContact = context.Contact.Where(x => x.Active == true && x.ObjectType == "EMP").ToList();

                            #endregion

                            //Nếu có điều kiện
                            if (listNotifiCondition.Count > 0)
                            {
                                //Do something...
                            }
                            //Nếu không có điều kiện
                            else
                            {
                                //Nếu gửi nội bộ
                                if (notifiSetting.SendInternal)
                                {
                                    //Nếu gửi bằng email
                                    if (notifiSetting.IsEmail)
                                    {
                                        #region Lấy danh sách email cần gửi thông báo

                                        var listEmailSendTo = new List<string>();
                                        var listAllUser = context.User.ToList();

                                        #region Lấy email người tham gia (Hiện tại chưa có người tham gia)

                                        if (notifiSetting.IsParticipant)
                                        {
                                        }

                                        #endregion

                                        #region Lấy email người phê duyệt (Tạo mới thì chưa cần gửi email cho người phê duyệt)

                                        if (notifiSetting.IsApproved)
                                        {
                                            //Người phê duyệt đơn hàng là người được phân quyền dữ liệu là Quản lý
                                            var listManager = listAllEmployee.Where(x => x.IsManager)
                                                .Select(y => y.EmployeeId).ToList();
                                            var listEmailManager = listAllContact
                                                .Where(x => listManager.Contains(x.ObjectId) && x.ObjectType == "EMP")
                                                .Select(y => y.Email).ToList();

                                            listEmailManager.ForEach(emailManager =>
                                            {
                                                if (!String.IsNullOrEmpty(emailManager))
                                                {
                                                    listEmailSendTo.Add(emailManager.Trim());
                                                }
                                            });
                                        }

                                        #endregion

                                        #region Lấy email người tạo

                                        if (notifiSetting.IsCreated)
                                        {
                                            var Email = LayEmaiNguoiTao(listAllUser, listAllContact, _procurementRequest.CreatedById.Value);
                                            listEmailSendTo.Add(Email);
                                        }

                                        #endregion

                                        #region Lấy email người phụ trách (Nhân viên bán hàng)

                                        if (notifiSetting.IsPersonIncharge)
                                        {
                                            var email_seller = listAllContact.FirstOrDefault(x =>
                                                x.ObjectId == _procurementRequest.RequestEmployeeId && x.ObjectType == "EMP")?.Email;

                                            if (!String.IsNullOrEmpty(email_seller))
                                            {
                                                listEmailSendTo.Add(email_seller.Trim());
                                            }
                                        }

                                        #endregion

                                        #region Lấy email của danh sách người đặc biệt

                                        var listEmailSpecial = LayEmaiNguoiDacBiet(listAllUser, listAllContact, context.NotifiSpecial.ToList(), notifiSetting);
                                        listEmailSpecial.ForEach(email =>
                                        {
                                            if (!String.IsNullOrEmpty(email))
                                            {
                                                listEmailSendTo.Add(email.Trim());
                                            }
                                        });

                                        #endregion

                                        listEmailSendTo = listEmailSendTo.Distinct().ToList();

                                        #endregion

                                        #region Kiểm tra xem Gửi ngay hay Đặt lịch gửi 

                                        //Gửi ngay
                                        var subject = ReplaceTokenForContent(context, typeModel, _procurementRequest,
                                            notifiSetting.EmailTitle, configEntity);
                                        var content = ReplaceTokenForContent(context, typeModel, _procurementRequest,
                                            notifiSetting.EmailContent, configEntity);

                                        #region Build nội dung thay đổi

                                        //string contentModelChange = "";

                                        ////Nếu phải kiểm tra thay đổi của model
                                        //if (checkChange)
                                        //{
                                        //    contentModelChange = ContentModelChange(oldModel, newModel, typeModel);
                                        //}

                                        #endregion

                                        Emailer.SendEmail(context, listEmailSendTo, new List<string>(), new List<string>(), subject, content);

                                        //Đặt lịch gửi             
                                        if (notifiSetting.ObjectBackHourInternal != null && listEmailSendTo.Count > 0)
                                        {
                                        }

                                        #endregion
                                    }
                                }
                            }

                            #endregion
                        }
                    }
                    //Chi tiết đề xuất mua hàng
                    else if (typeModel == TypeModel.ProcurementRequestDetail)
                    {
                        var _procurementRequest = newModel as ProcurementRequest;

                        if (_procurementRequest != null)
                        {
                            #region Kiểm tra xem cấu hình có điều kiện không?

                            var listNotifiCondition = context.NotifiSettingCondition
                                .Where(x => x.NotifiSettingId == notifiSetting.NotifiSettingId).ToList();

                            #region List reference để kiểm tra điều kiện và thông tin dùng để gửi email

                            var listAllEmployee = context.Employee.Where(x => x.Active == true).ToList();
                            var listAllContact = context.Contact.Where(x => x.Active == true && x.ObjectType == "EMP").ToList();

                            #endregion

                            //Nếu có điều kiện
                            if (listNotifiCondition.Count > 0)
                            {
                                //Do something...
                            }
                            //Nếu không có điều kiện
                            else
                            {
                                //Nếu gửi nội bộ
                                if (notifiSetting.SendInternal)
                                {
                                    //Nếu gửi bằng email
                                    if (notifiSetting.IsEmail)
                                    {
                                        #region Lấy danh sách email cần gửi thông báo

                                        var listEmailSendTo = new List<string>();
                                        var listAllUser = context.User.ToList();

                                        #region Lấy email người tham gia (Hiện tại chưa có người tham gia)

                                        if (notifiSetting.IsParticipant)
                                        {
                                        }

                                        #endregion

                                        #region Lấy email người phê duyệt (Tạo mới thì chưa cần gửi email cho người phê duyệt)

                                        if (notifiSetting.IsApproved)
                                        {
                                            /**
                                             * Người phê duyệt đơn hàng được thông báo
                                             * là người được phân quyền dữ liệu là Quản lý
                                             * của nhân viên bán hàng của đơn hàng
                                             */

                                            // trường hợp gửi thông báo khi bình luân
                                            if (note != null)
                                            {
                                                var _note = note as Note;

                                                // lấy id của nhân viên bán hàng 
                                                var employeeId =
                                                    context.ProcurementRequest.FirstOrDefault(x => x.ProcurementRequestId == _note.ObjectId)?.RequestEmployeeId;

                                                // lấy id phong ban thong qua id nhan vien
                                                var organizationId = listAllEmployee.FirstOrDefault(x => x.EmployeeId == employeeId)?
                                                    .OrganizationId;

                                                // lay id nhan vien phu trach cua phong ban
                                                var listManager = listAllEmployee.Where(x => x.IsManager &&
                                                        x.OrganizationId == organizationId)
                                                    .Select(y => y.EmployeeId).ToList();

                                                // lay email cua nhan vien phu trach do
                                                var listEmailManager = listAllContact
                                                    .Where(x => listManager.Contains(x.ObjectId) && x.ObjectType == "EMP")
                                                    .Select(y => y.Email).ToList();

                                                // add email do vao list email send to
                                                listEmailManager.ForEach(emailManager =>
                                                {
                                                    if (!String.IsNullOrEmpty(emailManager))
                                                    {
                                                        listEmailSendTo.Add(emailManager.Trim());
                                                    }
                                                });
                                            }
                                            else
                                            {
                                                // lấy id phong ban thong qua id nhan vien
                                                var organizationId = listAllEmployee
                                                    .FirstOrDefault(x => x.EmployeeId == empId)?
                                                    .OrganizationId;

                                                // lay id nhan vien phu trach cua phong ban
                                                var listManager = listAllEmployee.Where(x => x.IsManager &&
                                                        x.OrganizationId == organizationId)
                                                    .Select(y => y.EmployeeId).ToList();

                                                // lay email cua nhan vien phu trach do
                                                var listEmailManager = listAllContact
                                                    .Where(x => listManager.Contains(x.ObjectId) && x.ObjectType == "EMP")
                                                    .Select(y => y.Email).ToList();

                                                // add email do vao list email send to
                                                listEmailManager.ForEach(emailManager =>
                                                {
                                                    if (!String.IsNullOrEmpty(emailManager))
                                                    {
                                                        listEmailSendTo.Add(emailManager.Trim());
                                                    }
                                                });
                                            }
                                        }

                                        #endregion

                                        #region Lấy email người tạo

                                        if (notifiSetting.IsCreated)
                                        {
                                            var Email = LayEmaiNguoiTao(listAllUser, listAllContact, _procurementRequest.CreatedById.Value);
                                            listEmailSendTo.Add(Email);
                                        }

                                        #endregion

                                        #region Lấy email người phụ trách (Nhân viên bán hàng)

                                        if (notifiSetting.IsPersonIncharge)
                                        {
                                            var email_seller = listAllContact.FirstOrDefault(x =>
                                                x.ObjectId == _procurementRequest.RequestEmployeeId && x.ObjectType == "EMP")?.Email;

                                            if (!String.IsNullOrEmpty(email_seller))
                                            {
                                                listEmailSendTo.Add(email_seller.Trim());
                                            }
                                        }

                                        #endregion

                                        #region Lấy email của danh sách người đặc biệt

                                        var listEmailSpecial = LayEmaiNguoiDacBiet(listAllUser, listAllContact, context.NotifiSpecial.ToList(), notifiSetting);
                                        listEmailSpecial.ForEach(email =>
                                        {
                                            if (!String.IsNullOrEmpty(email))
                                            {
                                                listEmailSendTo.Add(email.Trim());
                                            }
                                        });

                                        #endregion

                                        #region Lấy email người tạo bình luận

                                        if (note != null)
                                        {
                                            var _note = note as Note;
                                            var Email = LayEmaiNguoiTao(context.User.ToList(), listAllContact, _note.CreatedById);
                                            listEmailSendTo.Add(Email);
                                        }

                                        #endregion

                                        listEmailSendTo = listEmailSendTo.Distinct().ToList();

                                        #endregion

                                        #region Kiểm tra xem Gửi ngay hay Đặt lịch gửi 

                                        //Gửi ngay
                                        var subject = ReplaceTokenForContent(context, typeModel, _procurementRequest,
                                            notifiSetting.EmailTitle, configEntity, note);
                                        var content = ReplaceTokenForContent(context, typeModel, _procurementRequest,
                                            notifiSetting.EmailContent, configEntity, note);

                                        #region Build nội dung thay đổi

                                        //string contentModelChange = "";

                                        ////Nếu phải kiểm tra thay đổi của model
                                        //if (checkChange)
                                        //{
                                        //    contentModelChange = ContentModelChange(oldModel, newModel, typeModel);
                                        //}

                                        #endregion

                                        Emailer.SendEmail(context, listEmailSendTo, new List<string>(), new List<string>(), subject, content);

                                        //Đặt lịch gửi             
                                        if (notifiSetting.ObjectBackHourInternal != null && listEmailSendTo.Count > 0)
                                        {
                                        }

                                        #endregion
                                    }
                                }
                            }

                            #endregion
                        }
                    }
                    //Tạo khách hàng tiềm năng
                    else if (typeModel == TypeModel.PotentialCustomer)
                    {
                        var _customer = newModel as Customer;

                        if (_customer != null)
                        {
                            #region Kiểm tra xem cấu hình có điều kiện không?

                            var listNotifiCondition = context.NotifiSettingCondition
                                .Where(x => x.NotifiSettingId == notifiSetting.NotifiSettingId).ToList();

                            #region List reference để kiểm tra điều kiện và thông tin dùng để gửi email

                            var listAllEmployee = context.Employee.Where(x => x.Active == true).ToList();
                            var listAllContact = context.Contact.Where(x => x.Active == true && x.ObjectType == "EMP").ToList();

                            #endregion

                            //Nếu có điều kiện
                            if (listNotifiCondition.Count > 0)
                            {
                                //Do something...
                            }
                            //Nếu không có điều kiện
                            else
                            {
                                //Nếu gửi nội bộ
                                if (notifiSetting.SendInternal)
                                {
                                    //Nếu gửi bằng email
                                    if (notifiSetting.IsEmail)
                                    {
                                        #region Lấy danh sách email cần gửi thông báo

                                        var listEmailSendTo = new List<string>();
                                        var listAllUser = context.User.ToList();

                                        #region Lấy email người tham gia (Hiện tại chưa có người tham gia)

                                        if (notifiSetting.IsParticipant)
                                        {
                                        }

                                        #endregion

                                        #region Lấy email người phê duyệt (Khách hàng tiềm năng không có action phê duyệt)

                                        if (notifiSetting.IsApproved)
                                        {
                                            //Người phê duyệt đơn hàng là người được phân quyền dữ liệu là Quản lý
                                            //var listManager = listAllEmployee.Where(x => x.IsManager)
                                            //    .Select(y => y.EmployeeId).ToList();
                                            //var listEmailManager = listAllContact
                                            //    .Where(x => listManager.Contains(x.ObjectId) && x.ObjectType == "EMP")
                                            //    .Select(y => y.Email).ToList();

                                            //listEmailManager.ForEach(emailManager =>
                                            //{
                                            //    if (!String.IsNullOrEmpty(emailManager))
                                            //    {
                                            //        listEmailSendTo.Add(emailManager.Trim());
                                            //    }
                                            //});
                                        }

                                        #endregion

                                        #region Lấy email người tạo

                                        if (notifiSetting.IsCreated)
                                        {
                                            var Email = LayEmaiNguoiTao(listAllUser, listAllContact, _customer.CreatedById);
                                            listEmailSendTo.Add(Email);
                                        }

                                        #endregion

                                        #region Lấy email người phụ trách (Nhân viên bán hàng)

                                        if (notifiSetting.IsPersonIncharge)
                                        {
                                            var email_seller = listAllContact.FirstOrDefault(x =>
                                                x.ObjectId == _customer.PersonInChargeId && x.ObjectType == "EMP")?.Email;

                                            if (!String.IsNullOrEmpty(email_seller))
                                            {
                                                listEmailSendTo.Add(email_seller.Trim());
                                            }
                                        }

                                        #endregion

                                        #region Lấy email của danh sách người đặc biệt

                                        var listEmailSpecial = LayEmaiNguoiDacBiet(listAllUser, listAllContact, context.NotifiSpecial.ToList(), notifiSetting);
                                        listEmailSpecial.ForEach(email =>
                                        {
                                            if (!String.IsNullOrEmpty(email))
                                            {
                                                listEmailSendTo.Add(email.Trim());
                                            }
                                        });

                                        #endregion

                                        listEmailSendTo = listEmailSendTo.Distinct().ToList();

                                        #endregion

                                        #region Kiểm tra xem Gửi ngay hay Đặt lịch gửi 

                                        //Gửi ngay
                                        var subject = ReplaceTokenForContent(context, typeModel, _customer,
                                            notifiSetting.EmailTitle, configEntity);
                                        var content = ReplaceTokenForContent(context, typeModel, _customer,
                                            notifiSetting.EmailContent, configEntity);

                                        #region Build nội dung thay đổi

                                        //string contentModelChange = "";

                                        ////Nếu phải kiểm tra thay đổi của model
                                        //if (checkChange)
                                        //{
                                        //    contentModelChange = ContentModelChange(oldModel, newModel, typeModel);
                                        //}

                                        #endregion

                                        Emailer.SendEmail(context, listEmailSendTo, new List<string>(), new List<string>(), subject, content);

                                        //Đặt lịch gửi 
                                        if (notifiSetting.ObjectBackHourInternal != null && listEmailSendTo.Count > 0)
                                        {

                                        }

                                        #endregion
                                    }
                                }
                            }

                            #endregion
                        }
                    }
                    //Chi tiết khách hàng tiềm năng
                    else if (typeModel == TypeModel.PotentialCustomerDetail)
                    {
                        var _customer = newModel as Customer;

                        if (_customer != null)
                        {
                            #region Kiểm tra xem cấu hình có điều kiện không?

                            var listNotifiCondition = context.NotifiSettingCondition
                                .Where(x => x.NotifiSettingId == notifiSetting.NotifiSettingId).ToList();

                            #region List reference để kiểm tra điều kiện và thông tin dùng để gửi email

                            var listAllEmployee = context.Employee.Where(x => x.Active == true).ToList();
                            var listAllContact = context.Contact.Where(x => x.Active == true && x.ObjectType == "EMP").ToList();

                            #endregion

                            //Nếu có điều kiện
                            if (listNotifiCondition.Count > 0)
                            {
                                //Do something...
                            }
                            //Nếu không có điều kiện
                            else
                            {
                                //Nếu gửi nội bộ
                                if (notifiSetting.SendInternal)
                                {
                                    //Nếu gửi bằng email
                                    if (notifiSetting.IsEmail)
                                    {
                                        #region Lấy danh sách email cần gửi thông báo

                                        var listEmailSendTo = new List<string>();
                                        var listAllUser = context.User.ToList();

                                        #region Lấy email người tham gia (Hiện tại chưa có người tham gia)

                                        if (notifiSetting.IsParticipant)
                                        {
                                        }

                                        #endregion

                                        #region Lấy email người phê duyệt (Khách hàng tiềm năng không có action phê duyệt)

                                        if (notifiSetting.IsApproved)
                                        {
                                            //Người phê duyệt đơn hàng là người được phân quyền dữ liệu là Quản lý
                                            //var listManager = listAllEmployee.Where(x => x.IsManager)
                                            //    .Select(y => y.EmployeeId).ToList();
                                            //var listEmailManager = listAllContact
                                            //    .Where(x => listManager.Contains(x.ObjectId) && x.ObjectType == "EMP")
                                            //    .Select(y => y.Email).ToList();

                                            //listEmailManager.ForEach(emailManager =>
                                            //{
                                            //    if (!String.IsNullOrEmpty(emailManager))
                                            //    {
                                            //        listEmailSendTo.Add(emailManager.Trim());
                                            //    }
                                            //});
                                        }

                                        #endregion

                                        #region Lấy email người tạo

                                        if (notifiSetting.IsCreated)
                                        {
                                            var Email = LayEmaiNguoiTao(listAllUser, listAllContact, _customer.CreatedById);
                                            listEmailSendTo.Add(Email);
                                        }

                                        #endregion

                                        #region Lấy email người phụ trách (Nhân viên bán hàng)

                                        if (notifiSetting.IsPersonIncharge)
                                        {
                                            var email_seller = listAllContact.FirstOrDefault(x =>
                                                x.ObjectId == _customer.PersonInChargeId && x.ObjectType == "EMP")?.Email;

                                            if (!String.IsNullOrEmpty(email_seller))
                                            {
                                                listEmailSendTo.Add(email_seller.Trim());
                                            }
                                        }

                                        #endregion

                                        #region Lấy email của danh sách người đặc biệt

                                        var listEmailSpecial = LayEmaiNguoiDacBiet(listAllUser, listAllContact, context.NotifiSpecial.ToList(), notifiSetting);
                                        listEmailSpecial.ForEach(email =>
                                        {
                                            if (!String.IsNullOrEmpty(email))
                                            {
                                                listEmailSendTo.Add(email.Trim());
                                            }
                                        });

                                        #endregion

                                        #region Lấy email người tạo bình luận

                                        if (note != null)
                                        {
                                            var _note = note as Note;
                                            var Email = LayEmaiNguoiTao(context.User.ToList(), listAllContact, _note.CreatedById);
                                            listEmailSendTo.Add(Email);
                                        }

                                        #endregion

                                        listEmailSendTo = listEmailSendTo.Distinct().ToList();

                                        #endregion

                                        #region Kiểm tra xem Gửi ngay hay Đặt lịch gửi 

                                        //Gửi ngay
                                        var subject = ReplaceTokenForContent(context, typeModel, _customer,
                                            notifiSetting.EmailTitle, configEntity, note);
                                        var content = ReplaceTokenForContent(context, typeModel, _customer,
                                            notifiSetting.EmailContent, configEntity, note);

                                        #region Build nội dung thay đổi

                                        //string contentModelChange = "";

                                        ////Nếu phải kiểm tra thay đổi của model
                                        //if (checkChange)
                                        //{
                                        //    contentModelChange = ContentModelChange(oldModel, newModel, typeModel);
                                        //}

                                        #endregion

                                        Emailer.SendEmail(context, listEmailSendTo, new List<string>(), new List<string>(), subject, content);

                                        //Đặt lịch gửi             
                                        if (notifiSetting.ObjectBackHourInternal != null && listEmailSendTo.Count > 0)
                                        {

                                        }

                                        #endregion
                                    }
                                }
                            }

                            #endregion
                        }
                    }
                    //Tạo khách hàng
                    else if (typeModel == TypeModel.Customer)
                    {
                        var _customer = newModel as Customer;

                        if (_customer != null)
                        {
                            #region Kiểm tra xem cấu hình có điều kiện không?

                            var listNotifiCondition = context.NotifiSettingCondition
                                .Where(x => x.NotifiSettingId == notifiSetting.NotifiSettingId).ToList();

                            #region List reference để kiểm tra điều kiện và thông tin dùng để gửi email

                            var listAllContact = context.Contact.Where(x => x.Active == true && x.ObjectType == "EMP").ToList();

                            #endregion

                            //Nếu có điều kiện
                            if (listNotifiCondition.Count > 0)
                            {
                                //Do something...
                            }
                            //Nếu không có điều kiện
                            else
                            {
                                //Nếu gửi nội bộ
                                if (notifiSetting.SendInternal)
                                {
                                    //Nếu gửi bằng email
                                    if (notifiSetting.IsEmail)
                                    {
                                        #region Lấy danh sách email cần gửi thông báo

                                        var listEmailSendTo = new List<string>();
                                        var listAllUser = context.User.ToList();

                                        #region Lấy email người tham gia (Hiện tại chưa có người tham gia)

                                        if (notifiSetting.IsParticipant)
                                        {
                                        }

                                        #endregion

                                        #region Lấy email người phê duyệt (Tạo mới thì chưa cần gửi email cho người phê duyệt)

                                        //if (notifiSetting.IsApproved)
                                        //{
                                        //    //Người phê duyệt đơn hàng là người được phân quyền dữ liệu là Quản lý
                                        //var listManager = listAllEmployee.Where(x => x.IsManager)
                                        //    .Select(y => y.EmployeeId).ToList();
                                        //var listEmailManager = listAllContact
                                        //    .Where(x => listManager.Contains(x.ObjectId) && x.ObjectType == "EMP")
                                        //    .Select(y => y.Email).ToList();

                                        //    listEmailManager.ForEach(emailManager =>
                                        //    {
                                        //        if (!String.IsNullOrEmpty(emailManager))
                                        //        {
                                        //            listEmailSendTo.Add(emailManager.Trim());
                                        //        }
                                        //    });
                                        //}

                                        #endregion

                                        #region Lấy email người tạo

                                        if (notifiSetting.IsCreated)
                                        {
                                            var Email = LayEmaiNguoiTao(listAllUser, listAllContact, _customer.CreatedById);
                                            listEmailSendTo.Add(Email);
                                        }

                                        #endregion

                                        #region Lấy email người phụ trách (Nhân viên bán hàng)

                                        if (notifiSetting.IsPersonIncharge)
                                        {
                                            var email_seller = listAllContact.FirstOrDefault(x =>
                                                x.ObjectId == _customer.PersonInChargeId && x.ObjectType == "EMP")?.Email;

                                            if (!String.IsNullOrEmpty(email_seller))
                                            {
                                                listEmailSendTo.Add(email_seller.Trim());
                                            }
                                        }

                                        #endregion

                                        #region Lấy email của danh sách người đặc biệt

                                        var listEmailSpecial = LayEmaiNguoiDacBiet(listAllUser, listAllContact, context.NotifiSpecial.ToList(), notifiSetting);
                                        listEmailSpecial.ForEach(email =>
                                        {
                                            if (!String.IsNullOrEmpty(email))
                                            {
                                                listEmailSendTo.Add(email.Trim());
                                            }
                                        });

                                        #endregion

                                        listEmailSendTo = listEmailSendTo.Distinct().ToList();

                                        #endregion

                                        #region Kiểm tra xem Gửi ngay hay Đặt lịch gửi 

                                        //Gửi ngay
                                        var subject = ReplaceTokenForContent(context, typeModel, _customer,
                                            notifiSetting.EmailTitle, configEntity);
                                        var content = ReplaceTokenForContent(context, typeModel, _customer,
                                            notifiSetting.EmailContent, configEntity);

                                        #region Build nội dung thay đổi

                                        //string contentModelChange = "";

                                        ////Nếu phải kiểm tra thay đổi của model
                                        //if (checkChange)
                                        //{
                                        //    contentModelChange = ContentModelChange(oldModel, newModel, typeModel);
                                        //}

                                        #endregion

                                        Emailer.SendEmail(context, listEmailSendTo, new List<string>(), new List<string>(), subject, content);

                                        //Đặt lịch gửi 
                                        if (notifiSetting.ObjectBackHourInternal != null && listEmailSendTo.Count > 0)
                                        {

                                        }

                                        #endregion
                                    }
                                }
                            }

                            #endregion
                        }
                    }
                    //Chi tiết khách hàng
                    else if (typeModel == TypeModel.CustomerDetail)
                    {
                        var _customer = newModel as Customer;

                        if (_customer != null)
                        {
                            #region Kiểm tra xem cấu hình có điều kiện không?

                            var listNotifiCondition = context.NotifiSettingCondition
                                .Where(x => x.NotifiSettingId == notifiSetting.NotifiSettingId).ToList();

                            #region List reference để kiểm tra điều kiện và thông tin dùng để gửi email

                            var listAllEmployee = context.Employee.Where(x => x.Active == true).ToList();
                            var listAllContact = context.Contact.Where(x => x.Active == true && x.ObjectType == "EMP").ToList();

                            #endregion

                            //Nếu có điều kiện
                            if (listNotifiCondition.Count > 0)
                            {
                                //Do something...
                            }
                            //Nếu không có điều kiện
                            else
                            {
                                //Nếu gửi nội bộ
                                if (notifiSetting.SendInternal)
                                {
                                    //Nếu gửi bằng email
                                    if (notifiSetting.IsEmail)
                                    {
                                        #region Lấy danh sách email cần gửi thông báo

                                        var listEmailSendTo = new List<string>();
                                        var listAllUser = context.User.ToList();

                                        #region Lấy email người tham gia (Hiện tại chưa có người tham gia)

                                        if (notifiSetting.IsParticipant)
                                        {
                                        }

                                        #endregion

                                        #region Lấy email người phê duyệt (Tạo mới thì chưa cần gửi email cho người phê duyệt)

                                        if (notifiSetting.IsApproved)
                                        {
                                            ////Người phê duyệt đơn hàng là người được phân quyền dữ liệu là Quản lý
                                            //var listManager = listAllEmployee.Where(x => x.IsManager)
                                            //    .Select(y => y.EmployeeId).ToList();
                                            //var listEmailManager = listAllContact
                                            //    .Where(x => listManager.Contains(x.ObjectId) && x.ObjectType == "EMP")
                                            //    .Select(y => y.Email).ToList();

                                            //listEmailManager.ForEach(emailManager =>
                                            //{
                                            //    if (!String.IsNullOrEmpty(emailManager))
                                            //    {
                                            //        listEmailSendTo.Add(emailManager.Trim());
                                            //    }
                                            //});
                                        }

                                        #endregion

                                        #region Lấy email người tạo

                                        if (notifiSetting.IsCreated)
                                        {
                                            var Email = LayEmaiNguoiTao(listAllUser, listAllContact, _customer.CreatedById);
                                            listEmailSendTo.Add(Email);
                                        }

                                        #endregion

                                        #region Lấy email người phụ trách (Nhân viên bán hàng)

                                        if (notifiSetting.IsPersonIncharge)
                                        {
                                            var email_seller = listAllContact.FirstOrDefault(x =>
                                                x.ObjectId == _customer.PersonInChargeId && x.ObjectType == "EMP")?.Email;

                                            if (!String.IsNullOrEmpty(email_seller))
                                            {
                                                listEmailSendTo.Add(email_seller.Trim());
                                            }
                                        }

                                        #endregion

                                        #region Lấy email của danh sách người đặc biệt

                                        var listEmailSpecial = LayEmaiNguoiDacBiet(listAllUser, listAllContact, context.NotifiSpecial.ToList(), notifiSetting);
                                        listEmailSpecial.ForEach(email =>
                                        {
                                            if (!String.IsNullOrEmpty(email))
                                            {
                                                listEmailSendTo.Add(email.Trim());
                                            }
                                        });

                                        #endregion

                                        #region Lấy email người tạo bình luận

                                        if (note != null)
                                        {
                                            var _note = note as Note;
                                            var Email = LayEmaiNguoiTao(context.User.ToList(), listAllContact, _note.CreatedById);
                                            listEmailSendTo.Add(Email);
                                        }

                                        #endregion

                                        listEmailSendTo = listEmailSendTo.Distinct().ToList();

                                        #endregion

                                        #region Kiểm tra xem Gửi ngay hay Đặt lịch gửi 

                                        //Gửi ngay
                                        var subject = ReplaceTokenForContent(context, typeModel, _customer,
                                            notifiSetting.EmailTitle, configEntity, note);
                                        var content = ReplaceTokenForContent(context, typeModel, _customer,
                                            notifiSetting.EmailContent, configEntity, note);

                                        #region Build nội dung thay đổi

                                        //string contentModelChange = "";

                                        ////Nếu phải kiểm tra thay đổi của model
                                        //if (checkChange)
                                        //{
                                        //    contentModelChange = ContentModelChange(oldModel, newModel, typeModel);
                                        //}

                                        #endregion

                                        Emailer.SendEmail(context, listEmailSendTo, new List<string>(), new List<string>(), subject, content);

                                        //Đặt lịch gửi             
                                        if (notifiSetting.ObjectBackHourInternal != null && listEmailSendTo.Count > 0)
                                        {

                                        }

                                        #endregion
                                    }
                                }
                            }

                            #endregion
                        }
                    }
                    //Tạo đơn hàng mua
                    else if (typeModel == TypeModel.VendorOrder)
                    {
                        var _vendorOrder = newModel as VendorOrder;

                        if (_vendorOrder != null)
                        {
                            #region Kiểm tra xem cấu hình có điều kiện không?

                            var listNotifiCondition = context.NotifiSettingCondition
                                .Where(x => x.NotifiSettingId == notifiSetting.NotifiSettingId).ToList();

                            #region List reference để kiểm tra điều kiện và thông tin dùng để gửi email

                            var listAllEmployee = context.Employee.Where(x => x.Active == true).ToList();
                            var listAllContact = context.Contact.Where(x => x.Active == true && x.ObjectType == "EMP").ToList();

                            #endregion

                            //Nếu có điều kiện
                            if (listNotifiCondition.Count > 0)
                            {
                                //Do something...
                            }
                            //Nếu không có điều kiện
                            else
                            {
                                //Nếu gửi nội bộ
                                if (notifiSetting.SendInternal)
                                {
                                    //Nếu gửi bằng email
                                    if (notifiSetting.IsEmail)
                                    {
                                        #region Lấy danh sách email cần gửi thông báo

                                        var listEmailSendTo = new List<string>();
                                        var listAllUser = context.User.ToList();

                                        #region Lấy email người tham gia (Hiện tại chưa có người tham gia)

                                        if (notifiSetting.IsParticipant)
                                        {
                                        }

                                        #endregion

                                        #region Lấy email người phê duyệt (Tạo mới thì chưa cần gửi email cho người phê duyệt)

                                        if (notifiSetting.IsApproved)
                                        {
                                            //Người phê duyệt đơn hàng là người được phân quyền dữ liệu là Quản lý
                                            var listManager = listAllEmployee.Where(x => x.IsManager)
                                                .Select(y => y.EmployeeId).ToList();
                                            var listEmailManager = listAllContact
                                                .Where(x => listManager.Contains(x.ObjectId) && x.ObjectType == "EMP")
                                                .Select(y => y.Email).ToList();

                                            listEmailManager.ForEach(emailManager =>
                                            {
                                                if (!String.IsNullOrEmpty(emailManager))
                                                {
                                                    listEmailSendTo.Add(emailManager.Trim());
                                                }
                                            });
                                        }

                                        #endregion

                                        #region Lấy email người tạo

                                        if (notifiSetting.IsCreated)
                                        {
                                            var Email = LayEmaiNguoiTao(listAllUser, listAllContact, _vendorOrder.CreatedById);
                                            listEmailSendTo.Add(Email);
                                        }

                                        #endregion

                                        #region Lấy email người phụ trách (Nhân viên bán hàng - đơn hàng mua không có người phụ trách)

                                        if (notifiSetting.IsPersonIncharge)
                                        {
                                            //var email_seller = listAllContact.FirstOrDefault(x =>
                                            //    x.ObjectId == _vendorOrder.PersonInChargeId && x.ObjectType == "EMP")?.Email;

                                            //if (!String.IsNullOrEmpty(email_seller))
                                            //{
                                            //    listEmailSendTo.Add(email_seller.Trim());
                                            //}
                                        }

                                        #endregion

                                        #region Lấy email của danh sách người đặc biệt

                                        var listEmailSpecial = LayEmaiNguoiDacBiet(listAllUser, listAllContact, context.NotifiSpecial.ToList(), notifiSetting);
                                        listEmailSpecial.ForEach(email =>
                                        {
                                            if (!String.IsNullOrEmpty(email))
                                            {
                                                listEmailSendTo.Add(email.Trim());
                                            }
                                        });

                                        #endregion

                                        listEmailSendTo = listEmailSendTo.Distinct().ToList();

                                        #endregion

                                        #region Kiểm tra xem Gửi ngay hay Đặt lịch gửi 

                                        //Gửi ngay
                                        var subject = ReplaceTokenForContent(context, typeModel, _vendorOrder,
                                            notifiSetting.EmailTitle, configEntity);
                                        var content = ReplaceTokenForContent(context, typeModel, _vendorOrder,
                                            notifiSetting.EmailContent, configEntity);

                                        #region Build nội dung thay đổi

                                        //string contentModelChange = "";

                                        ////Nếu phải kiểm tra thay đổi của model
                                        //if (checkChange)
                                        //{
                                        //    contentModelChange = ContentModelChange(oldModel, newModel, typeModel);
                                        //}

                                        #endregion

                                        Emailer.SendEmail(context, listEmailSendTo, new List<string>(), new List<string>(), subject, content);

                                        //Đặt lịch gửi 
                                        if (notifiSetting.ObjectBackHourInternal != null && listEmailSendTo.Count > 0)
                                        {

                                        }

                                        #endregion
                                    }
                                }
                            }

                            #endregion
                        }
                    }
                    //Chi tiết đơn hàng mua
                    else if (typeModel == TypeModel.VendorOrderDetail)
                    {
                        var _vendorOrder = newModel as VendorOrder;

                        if (_vendorOrder != null)
                        {
                            #region Kiểm tra xem cấu hình có điều kiện không?

                            var listNotifiCondition = context.NotifiSettingCondition
                                .Where(x => x.NotifiSettingId == notifiSetting.NotifiSettingId).ToList();

                            #region List reference để kiểm tra điều kiện và thông tin dùng để gửi email

                            var listAllEmployee = context.Employee.Where(x => x.Active == true).ToList();
                            var listAllContact = context.Contact.Where(x => x.Active == true && x.ObjectType == "EMP").ToList();

                            #endregion

                            //Nếu có điều kiện
                            if (listNotifiCondition.Count > 0)
                            {
                                //Do something...
                            }
                            //Nếu không có điều kiện
                            else
                            {
                                //Nếu gửi nội bộ
                                if (notifiSetting.SendInternal)
                                {
                                    //Nếu gửi bằng email
                                    if (notifiSetting.IsEmail)
                                    {
                                        #region Lấy danh sách email cần gửi thông báo

                                        var listEmailSendTo = new List<string>();
                                        var listAllUser = context.User.ToList();

                                        #region Lấy email người tham gia (Hiện tại chưa có người tham gia)

                                        if (notifiSetting.IsParticipant)
                                        {
                                        }

                                        #endregion

                                        #region Lấy email người phê duyệt (Tạo mới thì chưa cần gửi email cho người phê duyệt)

                                        if (notifiSetting.IsApproved)
                                        {
                                            /**
                                             * Người phê duyệt đơn hàng được thông báo
                                             * là người được phân quyền dữ liệu là Quản lý
                                             * của nhân viên bán hàng của đơn hàng
                                             */

                                            // trường hợp gửi thông báo khi bình luân
                                            if (note != null)
                                            {
                                                var _note = note as Note;

                                                // lấy id của nhân viên bán hàng 
                                                var employeeId =
                                                    context.VendorOrder.FirstOrDefault(x => x.VendorOrderId == _note.ObjectId)?.Orderer;

                                                // lấy id phong ban thong qua id nhan vien
                                                var organizationId = listAllEmployee.FirstOrDefault(x => x.EmployeeId == employeeId)?
                                                    .OrganizationId;

                                                // lay id nhan vien phu trach cua phong ban
                                                var listManager = listAllEmployee.Where(x => x.IsManager &&
                                                        x.OrganizationId == organizationId)
                                                    .Select(y => y.EmployeeId).ToList();

                                                // lay email cua nhan vien phu trach do
                                                var listEmailManager = listAllContact
                                                    .Where(x => listManager.Contains(x.ObjectId) && x.ObjectType == "EMP")
                                                    .Select(y => y.Email).ToList();

                                                // add email do vao list email send to
                                                listEmailManager.ForEach(emailManager =>
                                                {
                                                    if (!String.IsNullOrEmpty(emailManager))
                                                    {
                                                        listEmailSendTo.Add(emailManager.Trim());
                                                    }
                                                });
                                            }
                                            else
                                            {
                                                // lấy id phong ban thong qua id nhan vien
                                                var organizationId = listAllEmployee
                                                    .FirstOrDefault(x => x.EmployeeId == empId)?
                                                    .OrganizationId;

                                                // lay id nhan vien phu trach cua phong ban
                                                var listManager = listAllEmployee.Where(x => x.IsManager &&
                                                        x.OrganizationId == organizationId)
                                                    .Select(y => y.EmployeeId).ToList();

                                                // lay email cua nhan vien phu trach do
                                                var listEmailManager = listAllContact
                                                    .Where(x => listManager.Contains(x.ObjectId) && x.ObjectType == "EMP")
                                                    .Select(y => y.Email).ToList();

                                                // add email do vao list email send to
                                                listEmailManager.ForEach(emailManager =>
                                                {
                                                    if (!String.IsNullOrEmpty(emailManager))
                                                    {
                                                        listEmailSendTo.Add(emailManager.Trim());
                                                    }
                                                });
                                            }
                                        }

                                        #endregion

                                        #region Lấy email người tạo

                                        if (notifiSetting.IsCreated)
                                        {
                                            var Email = LayEmaiNguoiTao(listAllUser, listAllContact, _vendorOrder.CreatedById);
                                            listEmailSendTo.Add(Email);
                                        }

                                        #endregion

                                        #region Lấy email người phụ trách (Nhân viên bán hàng - đơn hàng mua không có người phụ trách)

                                        if (notifiSetting.IsPersonIncharge)
                                        {
                                            //var email_seller = listAllContact.FirstOrDefault(x =>
                                            //    x.ObjectId == _vendorOrder.PersonInChargeId && x.ObjectType == "EMP")?.Email;

                                            //if (!String.IsNullOrEmpty(email_seller))
                                            //{
                                            //    listEmailSendTo.Add(email_seller.Trim());
                                            //}
                                        }

                                        #endregion

                                        #region Lấy email của danh sách người đặc biệt

                                        var listEmailSpecial = LayEmaiNguoiDacBiet(listAllUser, listAllContact, context.NotifiSpecial.ToList(), notifiSetting);
                                        listEmailSpecial.ForEach(email =>
                                        {
                                            if (!String.IsNullOrEmpty(email))
                                            {
                                                listEmailSendTo.Add(email.Trim());
                                            }
                                        });

                                        #endregion

                                        #region Lấy email người tạo bình luận

                                        if (note != null)
                                        {
                                            var _note = note as Note;
                                            var Email = LayEmaiNguoiTao(context.User.ToList(), listAllContact, _note.CreatedById);
                                            listEmailSendTo.Add(Email);
                                        }

                                        #endregion

                                        listEmailSendTo = listEmailSendTo.Distinct().ToList();

                                        #endregion

                                        #region Kiểm tra xem Gửi ngay hay Đặt lịch gửi 

                                        //Gửi ngay
                                        var subject = ReplaceTokenForContent(context, typeModel, _vendorOrder,
                                            notifiSetting.EmailTitle, configEntity, note);
                                        var content = ReplaceTokenForContent(context, typeModel, _vendorOrder,
                                            notifiSetting.EmailContent, configEntity, note);

                                        #region Build nội dung thay đổi

                                        //string contentModelChange = "";

                                        ////Nếu phải kiểm tra thay đổi của model
                                        //if (checkChange)
                                        //{
                                        //    contentModelChange = ContentModelChange(oldModel, newModel, typeModel);
                                        //}

                                        #endregion

                                        Emailer.SendEmail(context, listEmailSendTo, new List<string>(), new List<string>(), subject, content);

                                        //Đặt lịch gửi             
                                        if (notifiSetting.ObjectBackHourInternal != null && listEmailSendTo.Count > 0)
                                        {

                                        }

                                        #endregion
                                    }
                                }
                            }

                            #endregion
                        }
                    }
                    //Tạo dự án
                    else if (typeModel == TypeModel.Project)
                    {
                        var _project = newModel as Project;

                        if (_project != null)
                        {
                            #region Kiểm tra xem cấu hình có điều kiện không?

                            var listNotifiCondition = context.NotifiSettingCondition
                                .Where(x => x.NotifiSettingId == notifiSetting.NotifiSettingId).ToList();

                            #region List reference để kiểm tra điều kiện và thông tin dùng để gửi email

                            var listAllEmployee = context.Employee.Where(x => x.Active == true).ToList();
                            var listAllContact = context.Contact.Where(x => x.Active == true && x.ObjectType == "EMP").ToList();

                            #endregion

                            //Nếu có điều kiện
                            if (listNotifiCondition.Count > 0)
                            {
                                //Do something...
                            }
                            //Nếu không có điều kiện
                            else
                            {
                                //Nếu gửi nội bộ
                                if (notifiSetting.SendInternal)
                                {
                                    //Nếu gửi bằng email
                                    if (notifiSetting.IsEmail)
                                    {
                                        #region Lấy danh sách email cần gửi thông báo

                                        var listEmailSendTo = new List<string>();
                                        var listAllUser = context.User.ToList();

                                        #region Lấy email người tham gia

                                        if (notifiSetting.IsParticipant)
                                        {
                                            var listParticipantId = context.ProjectResource
                                                .Where(x => x.ProjectId == _project.ProjectId).Select(y => y.ObjectId).ToList();

                                            if (listParticipantId.Count > 0)
                                            {
                                                var listEmailParticipant = listAllContact.Where(x =>
                                                        listParticipantId.Contains(x.ObjectId) && x.ObjectType == "EMP")
                                                    .ToList();

                                                listEmailParticipant.ForEach(contact =>
                                                {
                                                    if (!String.IsNullOrEmpty(contact?.WorkEmail))
                                                    {
                                                        listEmailSendTo.Add(contact.WorkEmail.Trim());
                                                    }
                                                    else if (!String.IsNullOrEmpty(contact?.Email))
                                                    {
                                                        listEmailSendTo.Add(contact.Email.Trim());
                                                    }
                                                });
                                            }
                                        }

                                        #endregion

                                        #region Lấy email người phê duyệt (dự án không có người phê duyệt)

                                        // if (notifiSetting.IsApproved)
                                        // {
                                        //Người phê duyệt báo giá sẽ phải kiểm tra theo Quy trình phê duyệt báo giá

                                        // var listApproved =
                                        //     GetListEmployeeApproved(context, "PDBG", _quote.ApprovalStep,
                                        //         listAllEmployee);
                                        //
                                        // var listEmailManager = listAllContact
                                        //     .Where(x => listApproved.Contains(x.ObjectId) && x.ObjectType == "EMP")
                                        //     .Select(y => y.Email).ToList();
                                        //
                                        // listEmailManager.ForEach(emailManager =>
                                        // {
                                        //     if (!String.IsNullOrEmpty(emailManager))
                                        //     {
                                        //         listEmailSendTo.Add(emailManager.Trim());
                                        //     }
                                        // });
                                        // }

                                        #endregion

                                        #region Lấy email người tạo

                                        if (notifiSetting.IsCreated)
                                        {
                                            var Email = LayEmaiNguoiTao(listAllUser, listAllContact, _project.CreateBy);
                                            listEmailSendTo.Add(Email);
                                        }

                                        #endregion

                                        #region Lấy email người phụ trách (Nhân viên bán hàng)

                                        if (notifiSetting.IsPersonIncharge)
                                        {

                                            var email_seller = listAllContact.FirstOrDefault(x =>
                                                x.ObjectId == _project.ProjectManagerId && x.ObjectType == "EMP")?.Email;

                                            if (!String.IsNullOrEmpty(email_seller))
                                            {
                                                listEmailSendTo.Add(email_seller.Trim());
                                            }

                                            var listEmpId = context.ProjectEmployeeMapping
                                                .Where(x => x.ProjectId == _project.ProjectId)
                                                .Select(y => y.EmployeeId).ToList();

                                            var listEmailManegement = listAllContact.Where(x =>
                                                    listEmpId.Contains(x.ObjectId) && x.ObjectType == "EMP")
                                                .Select(y => y.Email).ToList();

                                            if (listEmailManegement.Count > 0)
                                            {
                                                listEmailManegement.ForEach(item =>
                                                {
                                                    if (!String.IsNullOrEmpty(item))
                                                    {
                                                        listEmailSendTo.Add(item.Trim());
                                                    }
                                                });
                                            }
                                        }

                                        #endregion

                                        #region Lấy email của danh sách người đặc biệt

                                        var listEmailSpecial = LayEmaiNguoiDacBiet(listAllUser, listAllContact, context.NotifiSpecial.ToList(), notifiSetting);
                                        listEmailSpecial.ForEach(email =>
                                        {
                                            if (!String.IsNullOrEmpty(email))
                                            {
                                                listEmailSendTo.Add(email.Trim());
                                            }
                                        });

                                        #endregion

                                        listEmailSendTo = listEmailSendTo.Distinct().ToList();

                                        #endregion

                                        #region Kiểm tra xem Gửi ngay hay Đặt lịch gửi 

                                        //Gửi ngay
                                        listEmailSendTo?.ForEach(item =>
                                        {
                                            var subject = ReplaceTokenForContent(context, typeModel, _project,
                                            notifiSetting.EmailTitle, configEntity, emailSendTo: item);
                                            var content = ReplaceTokenForContent(context, typeModel, _project,
                                                notifiSetting.EmailContent, configEntity, emailSendTo: item);

                                            #region Build nội dung thay đổi

                                            //string contentModelChange = "";

                                            ////Nếu phải kiểm tra thay đổi của model
                                            //if (checkChange)
                                            //{
                                            //    contentModelChange = ContentModelChange(oldModel, newModel, typeModel);
                                            //}

                                            #endregion

                                            Emailer.SendEmail(context, new List<string> { item }, new List<string>(), new List<string>(), subject, content);
                                        });

                                        //Đặt lịch gửi             
                                        if (notifiSetting.ObjectBackHourInternal != null && listEmailSendTo.Count > 0)
                                        {
                                        }

                                        #endregion
                                    }
                                }
                            }

                            #endregion
                        }
                    }
                    //Chi tiết dự án
                    else if (typeModel == TypeModel.ProjectDetail)
                    {
                        var _project = newModel as Project;

                        if (_project != null)
                        {
                            #region Kiểm tra xem cấu hình có điều kiện không?

                            var listNotifiCondition = context.NotifiSettingCondition
                                .Where(x => x.NotifiSettingId == notifiSetting.NotifiSettingId).ToList();

                            #region List reference để kiểm tra điều kiện và thông tin dùng để gửi email

                            var listAllEmployee = context.Employee.Where(x => x.Active == true).ToList();
                            var listAllContact = context.Contact.Where(x => x.Active == true && x.ObjectType == "EMP").ToList();

                            #endregion

                            //Nếu có điều kiện
                            if (listNotifiCondition.Count > 0)
                            {
                                //Do something...
                            }
                            //Nếu không có điều kiện
                            else
                            {
                                //Nếu gửi nội bộ
                                if (notifiSetting.SendInternal)
                                {
                                    //Nếu gửi bằng email
                                    if (notifiSetting.IsEmail)
                                    {
                                        #region Lấy danh sách email cần gửi thông báo

                                        var listEmailSendTo = new List<string>();
                                        var listAllUser = context.User.ToList();

                                        #region Lấy email người tham gia

                                        if (notifiSetting.IsParticipant)
                                        {
                                            var listParticipantId = context.ProjectResource
                                                .Where(x => x.ProjectId == _project.ProjectId).Select(y => y.ObjectId).ToList();

                                            if (listParticipantId.Count > 0)
                                            {
                                                var listEmailParticipant = listAllContact.Where(x =>
                                                        listParticipantId.Contains(x.ObjectId) && x.ObjectType == "EMP")
                                                    .ToList();

                                                listEmailParticipant.ForEach(contact =>
                                                {
                                                    if (!String.IsNullOrEmpty(contact?.WorkEmail))
                                                    {
                                                        listEmailSendTo.Add(contact.WorkEmail.Trim());
                                                    }
                                                    else if (!String.IsNullOrEmpty(contact?.Email))
                                                    {
                                                        listEmailSendTo.Add(contact.Email.Trim());
                                                    }
                                                });
                                            }
                                        }

                                        #endregion

                                        #region Lấy email người phê duyệt (dự án không có người phê duyệt)

                                        if (notifiSetting.IsApproved)
                                        {

                                        }

                                        #endregion

                                        #region Lấy email người tạo

                                        if (notifiSetting.IsCreated)
                                        {
                                            var Email = LayEmaiNguoiTao(listAllUser, listAllContact, _project.CreateBy);
                                            listEmailSendTo.Add(Email);
                                        }

                                        #endregion

                                        #region Lấy email người phụ trách

                                        if (notifiSetting.IsPersonIncharge)
                                        {
                                            var email_seller = listAllContact.FirstOrDefault(x =>
                                                x.ObjectId == _project.ProjectManagerId && x.ObjectType == "EMP")?.Email;

                                            if (!String.IsNullOrEmpty(email_seller))
                                            {
                                                listEmailSendTo.Add(email_seller.Trim());
                                            }

                                            var listEmpId = context.ProjectEmployeeMapping
                                                .Where(x => x.ProjectId == _project.ProjectId)
                                                .Select(y => y.EmployeeId).ToList();

                                            var listEmailManegement = listAllContact.Where(x =>
                                                    listEmpId.Contains(x.ObjectId) && x.ObjectType == "EMP")
                                                .Select(y => y.Email).ToList();

                                            if (listEmailManegement.Count > 0)
                                            {
                                                listEmailManegement.ForEach(item =>
                                                {
                                                    if (!String.IsNullOrEmpty(item))
                                                    {
                                                        listEmailSendTo.Add(item.Trim());
                                                    }
                                                });
                                            }
                                        }

                                        #endregion

                                        #region Lấy email của danh sách người đặc biệt

                                        var listEmailSpecial = LayEmaiNguoiDacBiet(listAllUser, listAllContact, context.NotifiSpecial.ToList(), notifiSetting);
                                        listEmailSpecial.ForEach(email =>
                                        {
                                            if (!String.IsNullOrEmpty(email))
                                            {
                                                listEmailSendTo.Add(email.Trim());
                                            }
                                        });

                                        #endregion

                                        #region Lấy email người tạo bình luận

                                        if (note != null)
                                        {
                                            var _note = note as Note;
                                            var Email = LayEmaiNguoiTao(context.User.ToList(), listAllContact, _note.CreatedById);
                                            listEmailSendTo.Add(Email);
                                        }

                                        #endregion

                                        listEmailSendTo = listEmailSendTo.Distinct().ToList();

                                        #endregion

                                        #region Kiểm tra xem Gửi ngay hay Đặt lịch gửi 

                                        //Gửi ngay
                                        listEmailSendTo?.ForEach(item =>
                                        {
                                            var subject = ReplaceTokenForContent(context, typeModel, _project,
                                            notifiSetting.EmailTitle, configEntity, emailSendTo: item, note: note);
                                            var content = ReplaceTokenForContent(context, typeModel, _project,
                                                notifiSetting.EmailContent, configEntity, emailSendTo: item, note: note);

                                            #region Build nội dung thay đổi

                                            //string contentModelChange = "";

                                            ////Nếu phải kiểm tra thay đổi của model
                                            //if (checkChange)
                                            //{
                                            //    contentModelChange = ContentModelChange(oldModel, newModel, typeModel);
                                            //}

                                            #endregion

                                            Emailer.SendEmail(context, new List<string> { item }, new List<string>(), new List<string>(), subject, content);
                                        });


                                        //Đặt lịch gửi             
                                        if (notifiSetting.ObjectBackHourInternal != null && listEmailSendTo.Count > 0)
                                        {

                                        }

                                        #endregion
                                    }
                                }
                            }

                            #endregion
                        }
                    }
                    //Tạo công việc
                    else if (typeModel == TypeModel.ProjectTask)
                    {
                        var _task = newModel as Task;

                        if (_task != null)
                        {
                            #region Kiểm tra xem cấu hình có điều kiện không?

                            var listNotifiCondition = context.NotifiSettingCondition
                                .Where(x => x.NotifiSettingId == notifiSetting.NotifiSettingId).ToList();

                            #region List reference để kiểm tra điều kiện và thông tin dùng để gửi email

                            var listAllEmployee = context.Employee.Where(x => x.Active == true).ToList();
                            var listAllContact = context.Contact.Where(x => x.Active == true && x.ObjectType == "EMP").ToList();

                            #endregion

                            //Nếu có điều kiện
                            if (listNotifiCondition.Count > 0)
                            {
                                //Do something...
                            }
                            //Nếu không có điều kiện
                            else
                            {
                                //Nếu gửi nội bộ
                                if (notifiSetting.SendInternal)
                                {
                                    //Nếu gửi bằng email
                                    if (notifiSetting.IsEmail)
                                    {
                                        #region Lấy danh sách email cần gửi thông báo

                                        var listEmailSendTo = new List<string>();
                                        var listAllUser = context.User.ToList();

                                        #region Lấy email PM

                                        // lấy project của task 
                                        var project = context.Project.FirstOrDefault(x => x.ProjectId == _task.ProjectId);
                                        if (project != null)
                                        {
                                            var emailPM = listAllContact.FirstOrDefault(x =>
                                                x.ObjectId == project.ProjectManagerId && x.ObjectType == "EMP")?.Email;

                                            if (!String.IsNullOrEmpty(emailPM))
                                            {
                                                listEmailSendTo.Add(emailPM.Trim());
                                            }

                                            var listEmpId = context.ProjectEmployeeMapping
                                                .Where(x => x.ProjectId == project.ProjectId)
                                                .Select(y => y.EmployeeId).ToList();

                                            var listEmailManagement = listAllContact.Where(x =>
                                                    listEmpId.Contains(x.ObjectId) && x.ObjectType == "EMP")
                                                .Select(y => y.Email).ToList();

                                            if (listEmailManagement.Count > 0)
                                            {
                                                listEmailManagement.ForEach(item =>
                                                {
                                                    if (!String.IsNullOrEmpty(item))
                                                    {
                                                        listEmailSendTo.Add(item.Trim());
                                                    }
                                                });
                                            }
                                        }

                                        #endregion

                                        #region Lấy email người tham gia (người kiểm tra)

                                        if (notifiSetting.IsParticipant)
                                        {
                                            // lay ResourceID
                                            var listResourceId = context.TaskResourceMapping
                                                .Where(x => x.TaskId == _task.TaskId && x.IsChecker == true)
                                                .Select(y => y.ResourceId).ToList();
                                            if (listResourceId.Count > 0)
                                            {
                                                var listParticipantId = context.ProjectResource
                                                    .Where(x => listResourceId.Contains(x.ProjectResourceId)).Select(y => y.ObjectId).ToList();

                                                if (listParticipantId.Count > 0)
                                                {
                                                    var listEmailParticipant = listAllContact.Where(x =>
                                                            listParticipantId.Contains(x.ObjectId) && x.ObjectType == "EMP")
                                                        .ToList();

                                                    listEmailParticipant.ForEach(contact =>
                                                    {
                                                        if (!String.IsNullOrEmpty(contact?.WorkEmail))
                                                        {
                                                            listEmailSendTo.Add(contact.WorkEmail.Trim());
                                                        }
                                                        else if (!String.IsNullOrEmpty(contact?.Email))
                                                        {
                                                            listEmailSendTo.Add(contact.Email.Trim());
                                                        }
                                                    });
                                                }
                                            }
                                        }

                                        #endregion

                                        #region Lấy email người phê duyệt (dự án không có người phê duyệt)

                                        // if (notifiSetting.IsApproved)
                                        // {
                                        //Người phê duyệt báo giá sẽ phải kiểm tra theo Quy trình phê duyệt báo giá

                                        // var listApproved =
                                        //     GetListEmployeeApproved(context, "PDBG", _quote.ApprovalStep,
                                        //         listAllEmployee);
                                        //
                                        // var listEmailManager = listAllContact
                                        //     .Where(x => listApproved.Contains(x.ObjectId) && x.ObjectType == "EMP")
                                        //     .Select(y => y.Email).ToList();
                                        //
                                        // listEmailManager.ForEach(emailManager =>
                                        // {
                                        //     if (!String.IsNullOrEmpty(emailManager))
                                        //     {
                                        //         listEmailSendTo.Add(emailManager.Trim());
                                        //     }
                                        // });
                                        // }

                                        #endregion

                                        #region Lấy email người tạo

                                        if (notifiSetting.IsCreated)
                                        {
                                            var Email = LayEmaiNguoiTao(listAllUser, listAllContact, _task.CreateBy);
                                            listEmailSendTo.Add(Email);
                                        }

                                        #endregion

                                        #region Lấy email người phụ trách (Nhân viên phụ trách)

                                        if (notifiSetting.IsPersonIncharge)
                                        {

                                            // lay ResourceID
                                            var listResourceId = context.TaskResourceMapping
                                                .Where(x => x.TaskId == _task.TaskId && x.IsPersonInCharge == true)
                                                .Select(y => y.ResourceId).ToList();
                                            if (listResourceId.Count > 0)
                                            {
                                                var listParticipantId = context.ProjectResource
                                                    .Where(x => listResourceId.Contains(x.ProjectResourceId)).Select(y => y.ObjectId).ToList();

                                                if (listParticipantId.Count > 0)
                                                {
                                                    var listEmailParticipant = listAllContact.Where(x =>
                                                            listParticipantId.Contains(x.ObjectId) && x.ObjectType == "EMP")
                                                        .Select(y => y.Email)
                                                        .ToList();

                                                    listEmailParticipant.ForEach(email =>
                                                    {
                                                        if (!String.IsNullOrEmpty(email))
                                                        {
                                                            listEmailSendTo.Add(email.Trim());
                                                        }
                                                    });
                                                }
                                            }
                                        }

                                        #endregion

                                        #region Lấy email của danh sách người đặc biệt

                                        var listEmailSpecial = LayEmaiNguoiDacBiet(listAllUser, listAllContact, context.NotifiSpecial.ToList(), notifiSetting);
                                        listEmailSpecial.ForEach(email =>
                                        {
                                            if (!String.IsNullOrEmpty(email))
                                            {
                                                listEmailSendTo.Add(email.Trim());
                                            }
                                        });

                                        #endregion

                                        listEmailSendTo = listEmailSendTo.Distinct().ToList();

                                        #endregion

                                        #region Kiểm tra xem Gửi ngay hay Đặt lịch gửi 

                                        //Gửi ngay
                                        listEmailSendTo?.ForEach(item =>
                                        {
                                            var subject = ReplaceTokenForContent(context, typeModel, _task,
                                            notifiSetting.EmailTitle, configEntity, emailSendTo: item);
                                            var content = ReplaceTokenForContent(context, typeModel, _task,
                                                notifiSetting.EmailContent, configEntity, emailSendTo: item);

                                            #region Build nội dung thay đổi

                                            //string contentModelChange = "";

                                            ////Nếu phải kiểm tra thay đổi của model
                                            //if (checkChange)
                                            //{
                                            //    contentModelChange = ContentModelChange(oldModel, newModel, typeModel);
                                            //}

                                            #endregion

                                            Emailer.SendEmail(context, new List<string> { item }, new List<string>(), new List<string>(), subject, content);
                                        });

                                        //Đặt lịch gửi             
                                        if (notifiSetting.ObjectBackHourInternal != null && listEmailSendTo.Count > 0)
                                        {
                                        }

                                        #endregion
                                    }
                                }
                            }

                            #endregion
                        }
                    }
                    //Chi tiết công việc
                    else if (typeModel == TypeModel.ProjectTaskDetail)
                    {
                        var _task = newModel as Task;

                        if (_task != null)
                        {
                            #region Kiểm tra xem cấu hình có điều kiện không?

                            var listNotifiCondition = context.NotifiSettingCondition
                                .Where(x => x.NotifiSettingId == notifiSetting.NotifiSettingId).ToList();

                            #region List reference để kiểm tra điều kiện và thông tin dùng để gửi email

                            var listAllEmployee = context.Employee.Where(x => x.Active == true).ToList();
                            var listAllContact = context.Contact.Where(x => x.Active == true && x.ObjectType == "EMP").ToList();

                            #endregion

                            //Nếu có điều kiện
                            if (listNotifiCondition.Count > 0)
                            {
                                //Do something...
                            }
                            //Nếu không có điều kiện
                            else
                            {
                                //Nếu gửi nội bộ
                                if (notifiSetting.SendInternal)
                                {
                                    //Nếu gửi bằng email
                                    if (notifiSetting.IsEmail)
                                    {
                                        #region Lấy danh sách email cần gửi thông báo

                                        var listEmailSendTo = new List<string>();
                                        var listAllUser = context.User.ToList();

                                        #region Lấy email PM

                                        // lấy project của task 
                                        var project = context.Project.FirstOrDefault(x => x.ProjectId == _task.ProjectId);
                                        if (project != null)
                                        {
                                            var emailPM = listAllContact.FirstOrDefault(x =>
                                                x.ObjectId == project.ProjectManagerId && x.ObjectType == "EMP")?.Email;

                                            if (!String.IsNullOrEmpty(emailPM))
                                            {
                                                listEmailSendTo.Add(emailPM.Trim());
                                            }

                                            var listEmpId = context.ProjectEmployeeMapping
                                                .Where(x => x.ProjectId == project.ProjectId)
                                                .Select(y => y.EmployeeId).ToList();

                                            var listEmailManagement = listAllContact.Where(x =>
                                                    listEmpId.Contains(x.ObjectId) && x.ObjectType == "EMP")
                                                .Select(y => y.Email).ToList();

                                            if (listEmailManagement.Count > 0)
                                            {
                                                listEmailManagement.ForEach(item =>
                                                {
                                                    if (!String.IsNullOrEmpty(item))
                                                    {
                                                        listEmailSendTo.Add(item.Trim());
                                                    }
                                                });
                                            }
                                        }

                                        #endregion

                                        #region Lấy email người tham gia (người kiểm tra)

                                        if (notifiSetting.IsParticipant)
                                        {
                                            // lay ResourceID
                                            var listResourceId = context.TaskResourceMapping
                                                .Where(x => x.TaskId == _task.TaskId && x.IsChecker == true)
                                                .Select(y => y.ResourceId).ToList();
                                            if (listResourceId.Count > 0)
                                            {
                                                var listParticipantId = context.ProjectResource
                                                    .Where(x => listResourceId.Contains(x.ProjectResourceId)).Select(y => y.ObjectId).ToList();

                                                if (listParticipantId.Count > 0)
                                                {
                                                    var listEmailParticipant = listAllContact.Where(x =>
                                                            listParticipantId.Contains(x.ObjectId) && x.ObjectType == "EMP")
                                                        .ToList();

                                                    listEmailParticipant.ForEach(contact =>
                                                    {
                                                        if (!String.IsNullOrEmpty(contact?.WorkEmail))
                                                        {
                                                            listEmailSendTo.Add(contact.WorkEmail.Trim());
                                                        }
                                                        else if (!String.IsNullOrEmpty(contact?.Email))
                                                        {
                                                            listEmailSendTo.Add(contact.Email.Trim());
                                                        }
                                                    });
                                                }
                                            }
                                        }

                                        #endregion

                                        #region Lấy email người phê duyệt (dự án không có người phê duyệt)

                                        if (notifiSetting.IsApproved)
                                        {

                                        }

                                        #endregion

                                        #region Lấy email người tạo

                                        if (notifiSetting.IsCreated)
                                        {
                                            var Email = LayEmaiNguoiTao(listAllUser, listAllContact, _task.CreateBy);
                                            listEmailSendTo.Add(Email);
                                        }

                                        #endregion

                                        #region Lấy email người phụ trách (Nhân viên phụ trách)

                                        if (notifiSetting.IsPersonIncharge)
                                        {

                                            // lay ResourceID
                                            var listResourceId = context.TaskResourceMapping
                                                .Where(x => x.TaskId == _task.TaskId && x.IsPersonInCharge == true)
                                                .Select(y => y.ResourceId).ToList();
                                            if (listResourceId.Count > 0)
                                            {
                                                var listParticipantId = context.ProjectResource
                                                    .Where(x => listResourceId.Contains(x.ProjectResourceId)).Select(y => y.ObjectId).ToList();

                                                if (listParticipantId.Count > 0)
                                                {
                                                    var listEmailParticipant = listAllContact.Where(x =>
                                                            listParticipantId.Contains(x.ObjectId) && x.ObjectType == "EMP")
                                                        .Select(y => y.Email)
                                                        .ToList();

                                                    listEmailParticipant.ForEach(email =>
                                                    {
                                                        if (!String.IsNullOrEmpty(email))
                                                        {
                                                            listEmailSendTo.Add(email.Trim());
                                                        }
                                                    });
                                                }
                                            }
                                        }

                                        #endregion

                                        #region Lấy email của danh sách người đặc biệt

                                        var listEmailSpecial = LayEmaiNguoiDacBiet(listAllUser, listAllContact, context.NotifiSpecial.ToList(), notifiSetting);
                                        listEmailSpecial.ForEach(email =>
                                        {
                                            if (!String.IsNullOrEmpty(email))
                                            {
                                                listEmailSendTo.Add(email.Trim());
                                            }
                                        });

                                        #endregion

                                        #region Lấy email người tạo bình luận

                                        if (note != null)
                                        {
                                            var _note = note as Note;
                                            var Email = LayEmaiNguoiTao(context.User.ToList(), listAllContact, _note.CreatedById);
                                            listEmailSendTo.Add(Email);
                                        }

                                        #endregion

                                        listEmailSendTo = listEmailSendTo.Distinct().ToList();

                                        #endregion

                                        #region Kiểm tra xem Gửi ngay hay Đặt lịch gửi 

                                        //Gửi ngay
                                        listEmailSendTo?.ForEach(item =>
                                        {
                                            var subject = ReplaceTokenForContent(context, typeModel, _task,
                                            notifiSetting.EmailTitle, configEntity, emailSendTo: item, note: note);
                                            var content = ReplaceTokenForContent(context, typeModel, _task,
                                                notifiSetting.EmailContent, configEntity, emailSendTo: item, note: note);

                                            #region Build nội dung thay đổi

                                            //string contentModelChange = "";

                                            ////Nếu phải kiểm tra thay đổi của model
                                            //if (checkChange)
                                            //{
                                            //    contentModelChange = ContentModelChange(oldModel, newModel, typeModel);
                                            //}

                                            #endregion

                                            Emailer.SendEmail(context, new List<string> { item }, new List<string>(), new List<string>(), subject, content);
                                        });

                                        //Đặt lịch gửi             
                                        if (notifiSetting.ObjectBackHourInternal != null && listEmailSendTo.Count > 0)
                                        {

                                        }

                                        #endregion
                                    }
                                }
                            }

                            #endregion
                        }
                    }
                    //Hạng mục
                    else if (typeModel == TypeModel.ProjectScope)
                    {
                        var _scope = newModel as ProjectScope;

                        if (_scope != null)
                        {
                            #region Kiểm tra xem cấu hình có điều kiện không?

                            var listNotifiCondition = context.NotifiSettingCondition
                                .Where(x => x.NotifiSettingId == notifiSetting.NotifiSettingId).ToList();

                            #region List reference để kiểm tra điều kiện và thông tin dùng để gửi email

                            var listAllEmployee = context.Employee.Where(x => x.Active == true).ToList();
                            var listAllContact = context.Contact.Where(x => x.Active == true && x.ObjectType == "EMP").ToList();

                            #endregion

                            //Nếu có điều kiện
                            if (listNotifiCondition.Count > 0)
                            {
                                //Do something...
                            }
                            //Nếu không có điều kiện
                            else
                            {
                                //Nếu gửi nội bộ
                                if (notifiSetting.SendInternal)
                                {
                                    //Nếu gửi bằng email
                                    if (notifiSetting.IsEmail)
                                    {
                                        #region Lấy danh sách email cần gửi thông báo

                                        var listEmailSendTo = new List<string>();
                                        var listAllUser = context.User.ToList();

                                        #region Lấy email PM

                                        // lấy project của task 
                                        var project = context.Project.FirstOrDefault(x => x.ProjectId == _scope.ProjectId);
                                        if (project != null)
                                        {
                                            var emailPM = listAllContact.FirstOrDefault(x =>
                                                x.ObjectId == project.ProjectManagerId && x.ObjectType == "EMP")?.Email;

                                            if (!String.IsNullOrEmpty(emailPM))
                                            {
                                                listEmailSendTo.Add(emailPM.Trim());
                                            }

                                            var listEmpId = context.ProjectEmployeeMapping
                                                .Where(x => x.ProjectId == project.ProjectId)
                                                .Select(y => y.EmployeeId).ToList();

                                            var listEmailManagement = listAllContact.Where(x =>
                                                    listEmpId.Contains(x.ObjectId) && x.ObjectType == "EMP")
                                                .Select(y => y.Email).ToList();

                                            if (listEmailManagement.Count > 0)
                                            {
                                                listEmailManagement.ForEach(item =>
                                                {
                                                    if (!String.IsNullOrEmpty(item))
                                                    {
                                                        listEmailSendTo.Add(item.Trim());
                                                    }
                                                });
                                            }
                                        }

                                        #endregion

                                        #region Lấy email người tham gia (Hạng mục tạm thời chưa có người tham gia)

                                        if (notifiSetting.IsParticipant)
                                        {
                                            // lay ResourceID
                                            // var listResourceId = context.TaskResourceMapping
                                            //     .Where(x => x.TaskId == _scope.TaskId && x.IsChecker == true)
                                            //     .Select(y => y.ResourceId).ToList();
                                            // if (listResourceId.Count > 0)
                                            // {
                                            //     var listParticipantId = context.ProjectResource
                                            //         .Where(x => listResourceId.Contains(x.ProjectResourceId)).Select(y => y.ObjectId).ToList();
                                            //
                                            //     if (listParticipantId.Count > 0)
                                            //     {
                                            //         var listEmailParticipant = listAllContact.Where(x =>
                                            //                 listParticipantId.Contains(x.ObjectId) && x.ObjectType == "EMP")
                                            //             .Select(y => y.Email)
                                            //             .ToList();
                                            //
                                            //         listEmailParticipant.ForEach(email =>
                                            //         {
                                            //             if (!String.IsNullOrEmpty(email))
                                            //             {
                                            //                 listEmailSendTo.Add(email.Trim());
                                            //             }
                                            //         });
                                            //     }
                                            // }
                                        }

                                        #endregion

                                        #region Lấy email người phê duyệt (không có người phê duyệt)

                                        // if (notifiSetting.IsApproved)
                                        // {
                                        //Người phê duyệt báo giá sẽ phải kiểm tra theo Quy trình phê duyệt báo giá

                                        // var listApproved =
                                        //     GetListEmployeeApproved(context, "PDBG", _quote.ApprovalStep,
                                        //         listAllEmployee);
                                        //
                                        // var listEmailManager = listAllContact
                                        //     .Where(x => listApproved.Contains(x.ObjectId) && x.ObjectType == "EMP")
                                        //     .Select(y => y.Email).ToList();
                                        //
                                        // listEmailManager.ForEach(emailManager =>
                                        // {
                                        //     if (!String.IsNullOrEmpty(emailManager))
                                        //     {
                                        //         listEmailSendTo.Add(emailManager.Trim());
                                        //     }
                                        // });
                                        // }

                                        #endregion

                                        #region Lấy email người tạo

                                        if (notifiSetting.IsCreated)
                                        {
                                            var Email = LayEmaiNguoiTao(listAllUser, listAllContact, _scope.CreateBy);
                                            listEmailSendTo.Add(Email);
                                        }

                                        #endregion

                                        #region Lấy email người phụ trách (Không có người phụ trách)

                                        if (notifiSetting.IsPersonIncharge)
                                        {

                                            // lay ResourceID
                                            // var listResourceId = context.TaskResourceMapping
                                            //     .Where(x => x.TaskId == _scope.TaskId && x.IsPersonInCharge == true)
                                            //     .Select(y => y.ResourceId).ToList();
                                            // if (listResourceId.Count > 0)
                                            // {
                                            //     var listParticipantId = context.ProjectResource
                                            //         .Where(x => listResourceId.Contains(x.ProjectResourceId)).Select(y => y.ObjectId).ToList();
                                            //
                                            //     if (listParticipantId.Count > 0)
                                            //     {
                                            //         var listEmailParticipant = listAllContact.Where(x =>
                                            //                 listParticipantId.Contains(x.ObjectId) && x.ObjectType == "EMP")
                                            //             .Select(y => y.Email)
                                            //             .ToList();
                                            //
                                            //         listEmailParticipant.ForEach(email =>
                                            //         {
                                            //             if (!String.IsNullOrEmpty(email))
                                            //             {
                                            //                 listEmailSendTo.Add(email.Trim());
                                            //             }
                                            //         });
                                            //     }
                                            // }
                                        }

                                        #endregion

                                        #region Lấy email của danh sách người đặc biệt

                                        var listEmailSpecial = LayEmaiNguoiDacBiet(listAllUser, listAllContact, context.NotifiSpecial.ToList(), notifiSetting);
                                        listEmailSpecial.ForEach(email =>
                                        {
                                            if (!String.IsNullOrEmpty(email))
                                            {
                                                listEmailSendTo.Add(email.Trim());
                                            }
                                        });

                                        #endregion

                                        listEmailSendTo = listEmailSendTo.Distinct().ToList();

                                        #endregion

                                        #region Kiểm tra xem Gửi ngay hay Đặt lịch gửi 

                                        //Gửi ngay
                                        listEmailSendTo?.ForEach(item =>
                                        {
                                            var subject = ReplaceTokenForContent(context, typeModel, _scope,
                                            notifiSetting.EmailTitle, configEntity, emailSendTo: item, note: note);
                                            var content = ReplaceTokenForContent(context, typeModel, _scope,
                                                notifiSetting.EmailContent, configEntity, emailSendTo: item, note: note);

                                            #region Build nội dung thay đổi

                                            //string contentModelChange = "";

                                            ////Nếu phải kiểm tra thay đổi của model
                                            //if (checkChange)
                                            //{
                                            //    contentModelChange = ContentModelChange(oldModel, newModel, typeModel);
                                            //}

                                            #endregion

                                            Emailer.SendEmail(context, new List<string> { item }, new List<string>(), new List<string>(), subject, content);
                                        });

                                        //Đặt lịch gửi             
                                        if (notifiSetting.ObjectBackHourInternal != null && listEmailSendTo.Count > 0)
                                        {
                                        }

                                        #endregion
                                    }
                                }
                            }

                            #endregion
                        }
                    }
                    //Nguồn lực
                    else if (typeModel == TypeModel.ProjectResource)
                    {
                        var _resource = newModel as ProjectResource;

                        if (_resource != null)
                        {
                            #region Kiểm tra xem cấu hình có điều kiện không?

                            var listNotifiCondition = context.NotifiSettingCondition
                                .Where(x => x.NotifiSettingId == notifiSetting.NotifiSettingId).ToList();

                            #region List reference để kiểm tra điều kiện và thông tin dùng để gửi email

                            var listAllEmployee = context.Employee.Where(x => x.Active == true).ToList();
                            var listAllContact = context.Contact.Where(x => x.Active == true && x.ObjectType == "EMP").ToList();

                            #endregion

                            //Nếu có điều kiện
                            if (listNotifiCondition.Count > 0)
                            {
                                //Do something...
                            }
                            //Nếu không có điều kiện
                            else
                            {
                                //Nếu gửi nội bộ
                                if (notifiSetting.SendInternal)
                                {
                                    //Nếu gửi bằng email
                                    if (notifiSetting.IsEmail)
                                    {
                                        #region Lấy danh sách email cần gửi thông báo

                                        var listEmailSendTo = new List<string>();
                                        var listAllUser = context.User.ToList();

                                        #region Lấy email PM

                                        // lấy project của task 
                                        var project = context.Project.FirstOrDefault(x => x.ProjectId == _resource.ProjectId);
                                        if (project != null)
                                        {
                                            var emailPM = listAllContact.FirstOrDefault(x =>
                                                x.ObjectId == project.ProjectManagerId && x.ObjectType == "EMP")?.Email;

                                            if (!String.IsNullOrEmpty(emailPM))
                                            {
                                                listEmailSendTo.Add(emailPM.Trim());
                                            }

                                            var listEmpId = context.ProjectEmployeeMapping
                                                .Where(x => x.ProjectId == project.ProjectId)
                                                .Select(y => y.EmployeeId).ToList();

                                            var listEmailManagement = listAllContact.Where(x =>
                                                    listEmpId.Contains(x.ObjectId) && x.ObjectType == "EMP")
                                                .Select(y => y.Email).ToList();

                                            if (listEmailManagement.Count > 0)
                                            {
                                                listEmailManagement.ForEach(item =>
                                                {
                                                    if (!String.IsNullOrEmpty(item))
                                                    {
                                                        listEmailSendTo.Add(item.Trim());
                                                    }
                                                });
                                            }
                                        }

                                        #endregion

                                        #region Lấy email người tham gia (khong co nguoi tham gia)

                                        if (notifiSetting.IsParticipant)
                                        {
                                            // lay ResourceID
                                            // var listResourceId = context.TaskResourceMapping
                                            //     .Where(x => x.TaskId == _resource.TaskId && x.IsChecker == true)
                                            //     .Select(y => y.ResourceId).ToList();
                                            // if (listResourceId.Count > 0)
                                            // {
                                            //     var listParticipantId = context.ProjectResource
                                            //         .Where(x => listResourceId.Contains(x.ProjectResourceId)).Select(y => y.ObjectId).ToList();
                                            //
                                            //     if (listParticipantId.Count > 0)
                                            //     {
                                            //         var listEmailParticipant = listAllContact.Where(x =>
                                            //                 listParticipantId.Contains(x.ObjectId) && x.ObjectType == "EMP")
                                            //             .Select(y => y.Email)
                                            //             .ToList();
                                            //
                                            //         listEmailParticipant.ForEach(email =>
                                            //         {
                                            //             if (!String.IsNullOrEmpty(email))
                                            //             {
                                            //                 listEmailSendTo.Add(email.Trim());
                                            //             }
                                            //         });
                                            //     }
                                            // }
                                        }

                                        #endregion

                                        #region Lấy email người phê duyệt (dự án không có người phê duyệt)

                                        if (notifiSetting.IsApproved)
                                        {

                                        }

                                        #endregion

                                        #region Lấy email người tạo

                                        if (notifiSetting.IsCreated)
                                        {
                                            var Email = LayEmaiNguoiTao(listAllUser, listAllContact, _resource.CreateBy);
                                            listEmailSendTo.Add(Email);
                                        }

                                        #endregion

                                        #region Lấy email người phụ trách (email cua nguon luc dc them)

                                        if (notifiSetting.IsPersonIncharge)
                                        {

                                            var listEmailParticipant = listAllContact.Where(x =>
                                                    x.ObjectId == _resource.ObjectId && x.ObjectType == "EMP")
                                                .Select(y => y.Email)
                                                .ToList();

                                            listEmailParticipant.ForEach(email =>
                                            {
                                                if (!String.IsNullOrEmpty(email))
                                                {
                                                    listEmailSendTo.Add(email.Trim());
                                                }
                                            });
                                        }

                                        #endregion

                                        #region Lấy email của danh sách người đặc biệt

                                        var listEmailSpecial = LayEmaiNguoiDacBiet(listAllUser, listAllContact, context.NotifiSpecial.ToList(), notifiSetting);
                                        listEmailSpecial.ForEach(email =>
                                        {
                                            if (!String.IsNullOrEmpty(email))
                                            {
                                                listEmailSendTo.Add(email.Trim());
                                            }
                                        });

                                        #endregion

                                        listEmailSendTo = listEmailSendTo.Distinct().ToList();

                                        #endregion

                                        #region Kiểm tra xem Gửi ngay hay Đặt lịch gửi 

                                        //Gửi ngay
                                        listEmailSendTo?.ForEach(item =>
                                        {
                                            var subject = ReplaceTokenForContent(context, typeModel, _resource,
                                            notifiSetting.EmailTitle, configEntity, emailSendTo: item, note: note);
                                            var content = ReplaceTokenForContent(context, typeModel, _resource,
                                                notifiSetting.EmailContent, configEntity, emailSendTo: item, note: note);

                                            #region Build nội dung thay đổi

                                            //string contentModelChange = "";

                                            ////Nếu phải kiểm tra thay đổi của model
                                            //if (checkChange)
                                            //{
                                            //    contentModelChange = ContentModelChange(oldModel, newModel, typeModel);
                                            //}

                                            #endregion

                                            Emailer.SendEmail(context, new List<string> { item }, new List<string>(), new List<string>(), subject, content);
                                        });

                                        //Đặt lịch gửi             
                                        if (notifiSetting.ObjectBackHourInternal != null && listEmailSendTo.Count > 0)
                                        {

                                        }

                                        #endregion
                                    }
                                }
                            }

                            #endregion
                        }
                    }
                    //Phiếu thu
                    else if (typeModel == TypeModel.CashReceipts)
                    {
                        var _receiptInvoice = newModel as ReceiptInvoice;

                        if (_receiptInvoice != null)
                        {
                            #region Kiểm tra xem cấu hình có điều kiện không?

                            var listNotifiCondition = context.NotifiSettingCondition
                                .Where(x => x.NotifiSettingId == notifiSetting.NotifiSettingId).ToList();

                            #region List reference để kiểm tra điều kiện và thông tin dùng để gửi email

                            var listAllEmployee = context.Employee.Where(x => x.Active == true).ToList();
                            var listAllContact = context.Contact.Where(x => x.Active == true && x.ObjectType == "EMP").ToList();

                            #endregion

                            //Nếu có điều kiện
                            if (listNotifiCondition.Count > 0)
                            {
                                //Do something...
                            }
                            //Nếu không có điều kiện
                            else
                            {
                                //Nếu gửi nội bộ
                                if (notifiSetting.SendInternal)
                                {
                                    //Nếu gửi bằng email
                                    if (notifiSetting.IsEmail)
                                    {
                                        #region Lấy danh sách email cần gửi thông báo

                                        var listEmailSendTo = new List<string>();
                                        var listAllUser = context.User.ToList();

                                        #region Lấy email khách hàng

                                        var receiptInvoiceMapping = context.ReceiptInvoiceMapping.FirstOrDefault(x =>
                                            x.ReceiptInvoiceId == _receiptInvoice.ReceiptInvoiceId);

                                        if (receiptInvoiceMapping != null)
                                        {
                                            var customerId = context.Customer
                                                .FirstOrDefault(x => x.CustomerId == receiptInvoiceMapping.ObjectId)
                                                ?.CustomerId;
                                            var cusEmail = context.Contact.FirstOrDefault(x =>
                                                x.ObjectId == customerId && x.ObjectType == "CUS")?.Email;

                                            if (!String.IsNullOrEmpty(cusEmail))
                                            {
                                                listEmailSendTo.Add((cusEmail));
                                            }
                                        }

                                        #endregion

                                        #region Lấy email người tham gia (khong co nguoi tham gia)

                                        if (notifiSetting.IsParticipant)
                                        {
                                            // lay ResourceID
                                            // var listResourceId = context.TaskResourceMapping
                                            //     .Where(x => x.TaskId == _resource.TaskId && x.IsChecker == true)
                                            //     .Select(y => y.ResourceId).ToList();
                                            // if (listResourceId.Count > 0)
                                            // {
                                            //     var listParticipantId = context.ProjectResource
                                            //         .Where(x => listResourceId.Contains(x.ProjectResourceId)).Select(y => y.ObjectId).ToList();
                                            //
                                            //     if (listParticipantId.Count > 0)
                                            //     {
                                            //         var listEmailParticipant = listAllContact.Where(x =>
                                            //                 listParticipantId.Contains(x.ObjectId) && x.ObjectType == "EMP")
                                            //             .Select(y => y.Email)
                                            //             .ToList();
                                            //
                                            //         listEmailParticipant.ForEach(email =>
                                            //         {
                                            //             if (!String.IsNullOrEmpty(email))
                                            //             {
                                            //                 listEmailSendTo.Add(email.Trim());
                                            //             }
                                            //         });
                                            //     }
                                            // }
                                        }

                                        #endregion

                                        #region Lấy email người phê duyệt (không có người phê duyệt)

                                        if (notifiSetting.IsApproved)
                                        {

                                        }

                                        #endregion

                                        #region Lấy email người tạo

                                        if (notifiSetting.IsCreated)
                                        {
                                            var Email = LayEmaiNguoiTao(listAllUser, listAllContact, _receiptInvoice.CreatedById);
                                            listEmailSendTo.Add(Email);
                                        }

                                        #endregion

                                        #region Lấy email người phụ trách (khong co nguoi phu trach)

                                        // if (notifiSetting.IsPersonIncharge)
                                        // {
                                        //
                                        //     var listEmailParticipant = listAllContact.Where(x =>
                                        //             x.ObjectId == _resource.ObjectId && x.ObjectType == "EMP")
                                        //         .Select(y => y.Email)
                                        //         .ToList();
                                        //
                                        //     listEmailParticipant.ForEach(email =>
                                        //     {
                                        //         if (!String.IsNullOrEmpty(email))
                                        //         {
                                        //             listEmailSendTo.Add(email.Trim());
                                        //         }
                                        //     });
                                        // }

                                        #endregion

                                        #region Lấy email của danh sách người đặc biệt

                                        var listEmailSpecial = LayEmaiNguoiDacBiet(listAllUser, listAllContact, context.NotifiSpecial.ToList(), notifiSetting);
                                        listEmailSpecial.ForEach(email =>
                                        {
                                            if (!String.IsNullOrEmpty(email))
                                            {
                                                listEmailSendTo.Add(email.Trim());
                                            }
                                        });

                                        #endregion

                                        listEmailSendTo = listEmailSendTo.Distinct().ToList();

                                        #endregion

                                        #region Kiểm tra xem Gửi ngay hay Đặt lịch gửi 

                                        //Gửi ngay
                                        var subject = ReplaceTokenForContent(context, typeModel, _receiptInvoice,
                                            notifiSetting.EmailTitle, configEntity, note);
                                        var content = ReplaceTokenForContent(context, typeModel, _receiptInvoice,
                                            notifiSetting.EmailContent, configEntity, note);

                                        #region Build nội dung thay đổi

                                        //string contentModelChange = "";

                                        ////Nếu phải kiểm tra thay đổi của model
                                        //if (checkChange)
                                        //{
                                        //    contentModelChange = ContentModelChange(oldModel, newModel, typeModel);
                                        //}

                                        #endregion

                                        Emailer.SendEmail(context, listEmailSendTo, new List<string>(), new List<string>(), subject, content);

                                        //Đặt lịch gửi             
                                        if (notifiSetting.ObjectBackHourInternal != null && listEmailSendTo.Count > 0)
                                        {

                                        }

                                        #endregion
                                    }
                                }
                            }

                            #endregion
                        }
                    }
                    //Báo có
                    else if (typeModel == TypeModel.BankReceipts)
                    {
                        var _receiptInvoice = newModel as BankReceiptInvoice;

                        if (_receiptInvoice != null)
                        {
                            #region Kiểm tra xem cấu hình có điều kiện không?

                            var listNotifiCondition = context.NotifiSettingCondition
                                .Where(x => x.NotifiSettingId == notifiSetting.NotifiSettingId).ToList();

                            #region List reference để kiểm tra điều kiện và thông tin dùng để gửi email

                            var listAllEmployee = context.Employee.Where(x => x.Active == true).ToList();
                            var listAllContact = context.Contact.Where(x => x.Active == true && x.ObjectType == "EMP").ToList();

                            #endregion

                            //Nếu có điều kiện
                            if (listNotifiCondition.Count > 0)
                            {
                                //Do something...
                            }
                            //Nếu không có điều kiện
                            else
                            {
                                //Nếu gửi nội bộ
                                if (notifiSetting.SendInternal)
                                {
                                    //Nếu gửi bằng email
                                    if (notifiSetting.IsEmail)
                                    {
                                        #region Lấy danh sách email cần gửi thông báo

                                        var listEmailSendTo = new List<string>();
                                        var listAllUser = context.User.ToList();

                                        #region Lấy email khách hàng

                                        var receiptInvoiceMapping = context.BankPayableInvoiceMapping.FirstOrDefault(x =>
                                            x.BankPayableInvoiceId == _receiptInvoice.BankReceiptInvoiceId);

                                        if (receiptInvoiceMapping != null)
                                        {
                                            var customerId = context.Customer
                                                .FirstOrDefault(x => x.CustomerId == receiptInvoiceMapping.ObjectId)
                                                ?.CustomerId;
                                            var cusEmail = context.Contact.FirstOrDefault(x =>
                                                x.ObjectId == customerId && x.ObjectType == "CUS")?.Email;

                                            if (!string.IsNullOrEmpty(cusEmail))
                                            {
                                                listEmailSendTo.Add((cusEmail));
                                            }
                                        }

                                        #endregion

                                        #region Lấy email người tham gia (khong co nguoi tham gia)

                                        if (notifiSetting.IsParticipant)
                                        {
                                            // lay ResourceID
                                            // var listResourceId = context.TaskResourceMapping
                                            //     .Where(x => x.TaskId == _resource.TaskId && x.IsChecker == true)
                                            //     .Select(y => y.ResourceId).ToList();
                                            // if (listResourceId.Count > 0)
                                            // {
                                            //     var listParticipantId = context.ProjectResource
                                            //         .Where(x => listResourceId.Contains(x.ProjectResourceId)).Select(y => y.ObjectId).ToList();
                                            //
                                            //     if (listParticipantId.Count > 0)
                                            //     {
                                            //         var listEmailParticipant = listAllContact.Where(x =>
                                            //                 listParticipantId.Contains(x.ObjectId) && x.ObjectType == "EMP")
                                            //             .Select(y => y.Email)
                                            //             .ToList();
                                            //
                                            //         listEmailParticipant.ForEach(email =>
                                            //         {
                                            //             if (!String.IsNullOrEmpty(email))
                                            //             {
                                            //                 listEmailSendTo.Add(email.Trim());
                                            //             }
                                            //         });
                                            //     }
                                            // }
                                        }

                                        #endregion

                                        #region Lấy email người phê duyệt (không có người phê duyệt)

                                        if (notifiSetting.IsApproved)
                                        {

                                        }

                                        #endregion

                                        #region Lấy email người tạo

                                        if (notifiSetting.IsCreated)
                                        {
                                            var Email = LayEmaiNguoiTao(listAllUser, listAllContact, _receiptInvoice.CreatedById);
                                            listEmailSendTo.Add(Email);
                                        }

                                        #endregion

                                        #region Lấy email người phụ trách (khong co nguoi phu trach)

                                        // if (notifiSetting.IsPersonIncharge)
                                        // {
                                        //
                                        //     var listEmailParticipant = listAllContact.Where(x =>
                                        //             x.ObjectId == _resource.ObjectId && x.ObjectType == "EMP")
                                        //         .Select(y => y.Email)
                                        //         .ToList();
                                        //
                                        //     listEmailParticipant.ForEach(email =>
                                        //     {
                                        //         if (!String.IsNullOrEmpty(email))
                                        //         {
                                        //             listEmailSendTo.Add(email.Trim());
                                        //         }
                                        //     });
                                        // }

                                        #endregion

                                        #region Lấy email của danh sách người đặc biệt

                                        var listEmailSpecial = LayEmaiNguoiDacBiet(listAllUser, listAllContact, context.NotifiSpecial.ToList(), notifiSetting);
                                        listEmailSpecial.ForEach(email =>
                                        {
                                            if (!String.IsNullOrEmpty(email))
                                            {
                                                listEmailSendTo.Add(email.Trim());
                                            }
                                        });

                                        #endregion

                                        listEmailSendTo = listEmailSendTo.Distinct().ToList();

                                        #endregion

                                        #region Kiểm tra xem Gửi ngay hay Đặt lịch gửi 

                                        //Gửi ngay
                                        var subject = ReplaceTokenForContent(context, typeModel, _receiptInvoice,
                                            notifiSetting.EmailTitle, configEntity, note);
                                        var content = ReplaceTokenForContent(context, typeModel, _receiptInvoice,
                                            notifiSetting.EmailContent, configEntity, note);

                                        #region Build nội dung thay đổi

                                        //string contentModelChange = "";

                                        ////Nếu phải kiểm tra thay đổi của model
                                        //if (checkChange)
                                        //{
                                        //    contentModelChange = ContentModelChange(oldModel, newModel, typeModel);
                                        //}

                                        #endregion

                                        Emailer.SendEmail(context, listEmailSendTo, new List<string>(), new List<string>(), subject, content);

                                        //Đặt lịch gửi             
                                        if (notifiSetting.ObjectBackHourInternal != null && listEmailSendTo.Count > 0)
                                        {

                                        }

                                        #endregion
                                    }
                                }
                            }

                            #endregion
                        }
                    }
                    //Tạo nhân viên
                    else if (typeModel == TypeModel.Employee)
                    {
                        var _employee = newModel as Employee;

                        if (_employee != null)
                        {
                            #region Kiểm tra xem cấu hình có điều kiện không?

                            var listNotifiCondition = context.NotifiSettingCondition
                                .Where(x => x.NotifiSettingId == notifiSetting.NotifiSettingId).ToList();

                            #region List reference để kiểm tra điều kiện và thông tin dùng để gửi email

                            var listAllEmployee = context.Employee.Where(x => x.Active == true).ToList();
                            var listAllContact = context.Contact.Where(x => x.Active == true && x.ObjectType == "EMP").ToList();

                            #endregion

                            //Nếu có điều kiện
                            if (listNotifiCondition.Count > 0)
                            {
                                //Do something...
                            }
                            //Nếu không có điều kiện
                            else
                            {
                                //Nếu gửi nội bộ
                                if (notifiSetting.SendInternal)
                                {
                                    //Nếu gửi bằng email
                                    if (notifiSetting.IsEmail)
                                    {
                                        #region Lấy danh sách email cần gửi thông báo

                                        var listEmailSendTo = new List<string>();
                                        var listAllUser = context.User.ToList();

                                        if (actionCode == "CRE")
                                        {
                                            #region lấy email của nhân viên

                                            var empEmail = context.Contact.FirstOrDefault(x => x.ObjectId == _employee.EmployeeId && x.ObjectType == "EMP")?.Email;

                                            if (!string.IsNullOrEmpty(empEmail))
                                            {
                                                listEmailSendTo.Add(empEmail);
                                            }

                                            #endregion

                                            #region Lấy email người tham gia (Khong co)

                                            if (notifiSetting.IsParticipant)
                                            {
                                                //var empEmail = context.Contact.FirstOrDefault(x => x.ObjectId == _employee.EmployeeId && x.ObjectType == "EMP")?.Email;

                                                //if (!string.IsNullOrEmpty(empEmail))
                                                //{
                                                //    listEmailSendTo.Add(empEmail);
                                                //}
                                            }

                                            #endregion

                                            #region Lấy email người phê duyệt (không có người phê duyệt)

                                            if (notifiSetting.IsApproved)
                                            {

                                            }

                                            #endregion

                                            #region Lấy email người tạo

                                            if (notifiSetting.IsCreated)
                                            {
                                                var Email = LayEmaiNguoiTao(listAllUser, listAllContact, _employee.CreatedById);
                                                listEmailSendTo.Add(Email);
                                            }

                                            #endregion

                                            #region Lấy email người phụ trách (khong co nguoi phu trach)

                                            // if (notifiSetting.IsPersonIncharge)
                                            // {
                                            //
                                            //     var listEmailParticipant = listAllContact.Where(x =>
                                            //             x.ObjectId == _resource.ObjectId && x.ObjectType == "EMP")
                                            //         .Select(y => y.Email)
                                            //         .ToList();
                                            //
                                            //     listEmailParticipant.ForEach(email =>
                                            //     {
                                            //         if (!String.IsNullOrEmpty(email))
                                            //         {
                                            //             listEmailSendTo.Add(email.Trim());
                                            //         }
                                            //     });
                                            // }

                                            #endregion
                                        }

                                        #region Lấy email của danh sách người đặc biệt

                                        var listEmailSpecial = LayEmaiNguoiDacBiet(listAllUser, listAllContact, context.NotifiSpecial.ToList(), notifiSetting);
                                        listEmailSpecial.ForEach(email =>
                                        {
                                            if (!String.IsNullOrEmpty(email))
                                            {
                                                listEmailSendTo.Add(email.Trim());
                                            }
                                        });

                                        #endregion

                                        listEmailSendTo = listEmailSendTo.Distinct().ToList();

                                        #endregion

                                        #region Kiểm tra xem Gửi ngay hay Đặt lịch gửi 

                                        //Gửi ngay
                                        var subject = ReplaceTokenForContent(context, typeModel, _employee,
                                            notifiSetting.EmailTitle, configEntity, note);
                                        var content = ReplaceTokenForContent(context, typeModel, _employee,
                                            notifiSetting.EmailContent, configEntity, note);

                                        #region Build nội dung thay đổi

                                        //string contentModelChange = "";

                                        ////Nếu phải kiểm tra thay đổi của model
                                        //if (checkChange)
                                        //{
                                        //    contentModelChange = ContentModelChange(oldModel, newModel, typeModel);
                                        //}

                                        #endregion

                                        Emailer.SendEmail(context, listEmailSendTo, new List<string>(), new List<string>(), subject, content);

                                        //Đặt lịch gửi             
                                        if (notifiSetting.ObjectBackHourInternal != null && listEmailSendTo.Count > 0)
                                        {

                                        }

                                        #endregion
                                    }
                                }
                            }

                            #endregion
                        }
                    }
                    // Quên mật khẩu | Đặt lại mật khẩu
                    else if (typeModel == TypeModel.EmployeeDetail)
                    {
                        var _user = newModel as User;

                        if (_user != null)
                        {
                            #region Kiểm tra xem cấu hình có điều kiện không?

                            var listNotifiCondition = context.NotifiSettingCondition
                                .Where(x => x.NotifiSettingId == notifiSetting.NotifiSettingId).ToList();

                            #region List reference để kiểm tra điều kiện và thông tin dùng để gửi email

                            var listAllEmployee = context.Employee.Where(x => x.Active == true).ToList();
                            var listAllContact = context.Contact.Where(x => x.Active == true && x.ObjectType == "CUS").ToList();

                            #endregion

                            //Nếu có điều kiện
                            if (listNotifiCondition.Count > 0)
                            {
                                //Do something...
                            }
                            //Nếu không có điều kiện
                            else
                            {
                                //Nếu gửi nội bộ
                                if (notifiSetting.SendInternal)
                                {
                                    //Nếu gửi bằng email
                                    if (notifiSetting.IsEmail)
                                    {
                                        #region Lấy danh sách email cần gửi thông báo

                                        var listEmailSendTo = new List<string>();
                                        var listAllUser = context.User.ToList();


                                        #region lấy email của user

                                        var userEmail = context.Contact.FirstOrDefault(x => x.ObjectId == _user.EmployeeId && x.ObjectType == "EMP")?.Email;

                                        if (!string.IsNullOrEmpty(userEmail))
                                        {
                                            listEmailSendTo.Add(userEmail);
                                        }

                                        #endregion

                                        #region Lấy email người tham gia (Khong co)

                                        if (notifiSetting.IsParticipant)
                                        {
                                            //var empEmail = context.Contact.FirstOrDefault(x => x.ObjectId == _employee.EmployeeId && x.ObjectType == "EMP")?.Email;

                                            //if (!string.IsNullOrEmpty(empEmail))
                                            //{
                                            //    listEmailSendTo.Add(empEmail);
                                            //}
                                        }

                                        #endregion

                                        #region Lấy email người phê duyệt (không có người phê duyệt)

                                        if (notifiSetting.IsApproved)
                                        {

                                        }

                                        #endregion

                                        #region Lấy email người tạo

                                        if (notifiSetting.IsCreated)
                                        {
                                            var Email = LayEmaiNguoiTao(listAllUser, listAllContact, _user.CreatedById);
                                            listEmailSendTo.Add(Email);
                                        }

                                        if (!string.IsNullOrEmpty(emailReceiver))
                                        {
                                            listEmailSendTo.Add(emailReceiver);
                                        }

                                        #endregion

                                        #region Lấy email người phụ trách (khong co nguoi phu trach)

                                        // if (notifiSetting.IsPersonIncharge)
                                        // {
                                        //
                                        //     var listEmailParticipant = listAllContact.Where(x =>
                                        //             x.ObjectId == _resource.ObjectId && x.ObjectType == "EMP")
                                        //         .Select(y => y.Email)
                                        //         .ToList();
                                        //
                                        //     listEmailParticipant.ForEach(email =>
                                        //     {
                                        //         if (!String.IsNullOrEmpty(email))
                                        //         {
                                        //             listEmailSendTo.Add(email.Trim());
                                        //         }
                                        //     });
                                        // }

                                        #endregion

                                        #region Lấy email của danh sách người đặc biệt

                                        var listEmailSpecial = LayEmaiNguoiDacBiet(listAllUser, listAllContact, context.NotifiSpecial.ToList(), notifiSetting);
                                        listEmailSpecial.ForEach(email =>
                                        {
                                            if (!String.IsNullOrEmpty(email))
                                            {
                                                listEmailSendTo.Add(email.Trim());
                                            }
                                        });

                                        #endregion

                                        listEmailSendTo = listEmailSendTo.Distinct().ToList();

                                        #endregion

                                        #region Kiểm tra xem Gửi ngay hay Đặt lịch gửi 

                                        //Gửi ngay
                                        var subject = ReplaceTokenForContent(context, typeModel, _user,
                                            notifiSetting.EmailTitle, configEntity, note, actionCode);
                                        var content = ReplaceTokenForContent(context, typeModel, _user,
                                            notifiSetting.EmailContent, configEntity, note, actionCode);

                                        #region Build nội dung thay đổi

                                        //string contentModelChange = "";

                                        ////Nếu phải kiểm tra thay đổi của model
                                        //if (checkChange)
                                        //{
                                        //    contentModelChange = ContentModelChange(oldModel, newModel, typeModel);
                                        //}

                                        #endregion

                                        Emailer.SendEmail(context, listEmailSendTo, new List<string>(), new List<string>(), subject, content);

                                        //Đặt lịch gửi             
                                        if (notifiSetting.ObjectBackHourInternal != null && listEmailSendTo.Count > 0)
                                        {

                                        }

                                        #endregion
                                    }
                                }
                            }

                            #endregion
                        }
                    }
                    // Chi tiết đề xuất xin nghỉ và là phê duyệt các bước
                    else if (typeModel == TypeModel.RequestDetail && actionCode == "APPRO_REQUEST")
                    {
                        var _request = newModel as DeXuatXinNghi;

                        if (_request != null)
                        {
                            #region Kiểm tra xem cấu hình có điều kiện không?

                            var listNotifiCondition = context.NotifiSettingCondition
                                .Where(x => x.NotifiSettingId == notifiSetting.NotifiSettingId).ToList();

                            #region List reference để kiểm tra điều kiện và thông tin dùng để gửi email

                            var listAllEmployee = context.Employee.Where(x => x.Active == true).ToList();
                            var listAllContact = context.Contact.Where(x => x.Active == true && x.ObjectType == "EMP").ToList();

                            #endregion

                            //Nếu có điều kiện
                            if (listNotifiCondition.Count > 0)
                            {
                                //Do something...
                            }
                            //Nếu không có điều kiện
                            else
                            {
                                //Nếu gửi nội bộ
                                if (notifiSetting.SendInternal)
                                {
                                    //Nếu gửi bằng email
                                    if (notifiSetting.IsEmail)
                                    {
                                        #region Lấy danh sách email cần gửi thông báo

                                        var listEmailSendTo = new List<string>();
                                        var listAllUser = context.User.ToList();

                                        #region Lấy email người tham gia ( danh sách thông báo cho )

                                        if (notifiSetting.IsParticipant)
                                        {

                                        }

                                        #endregion

                                        #region Lấy email người phê duyệt

                                        if (notifiSetting.IsApproved)
                                        {
                                            if (notifiSetting.IsApproved)
                                            {
                                                listEmailSendTo.AddRange(ListMailNguoiPheDuyet);
                                            }
                                        }

                                        #endregion

                                        #region Lấy email người tạo

                                        if (notifiSetting.IsCreated)
                                        {
                                            var Email = LayEmaiNguoiTao(listAllUser, listAllContact, _request.CreatedById);
                                            listEmailSendTo.Add(Email);
                                        }

                                        #endregion

                                        #region Lấy email người phụ trách

                                        if (notifiSetting.IsPersonIncharge)
                                        {

                                        }

                                        #endregion

                                        #region Lấy email của danh sách người đặc biệt

                                        var listEmailSpecial = LayEmaiNguoiDacBiet(listAllUser, listAllContact, context.NotifiSpecial.ToList(), notifiSetting);
                                        listEmailSpecial.ForEach(email =>
                                        {
                                            if (!String.IsNullOrEmpty(email))
                                            {
                                                listEmailSendTo.Add(email.Trim());
                                            }
                                        });

                                        #endregion

                                        listEmailSendTo = listEmailSendTo.Where(x => x != null).Distinct().ToList();

                                        #endregion

                                        #region Kiểm tra xem Gửi ngay hay Đặt lịch gửi 

                                        //Gửi ngay
                                        var subject = ReplaceTokenForContent(context, typeModel, _request,
                                            notifiSetting.EmailTitle, configEntity, note, actionCode);
                                        var content = ReplaceTokenForContent(context, typeModel, _request,
                                            notifiSetting.EmailContent, configEntity, note, actionCode);

                                        #region Build nội dung thay đổi

                                        //string contentModelChange = "";

                                        ////Nếu phải kiểm tra thay đổi của model
                                        //if (checkChange)
                                        //{
                                        //    contentModelChange = ContentModelChange(oldModel, newModel, typeModel);
                                        //}

                                        #endregion

                                        Emailer.SendEmail(context, listEmailSendTo, new List<string>(), new List<string>(), subject, content);

                                        //Đặt lịch gửi             
                                        if (notifiSetting.ObjectBackHourInternal != null && listEmailSendTo.Count > 0)
                                        {

                                        }

                                        #endregion
                                    }
                                }
                            }

                            #endregion
                        }
                    }
                    // Chi tiết đề xuất xin nghỉ và là phê duyệt bước cuối
                    else if (typeModel == TypeModel.RequestDetail && actionCode == "APPRO_REQUEST_FINAL")
                    {
                        var _request = newModel as DeXuatXinNghi;

                        if (_request != null)
                        {
                            #region Kiểm tra xem cấu hình có điều kiện không?

                            var listNotifiCondition = context.NotifiSettingCondition
                                .Where(x => x.NotifiSettingId == notifiSetting.NotifiSettingId).ToList();

                            #region List reference để kiểm tra điều kiện và thông tin dùng để gửi email

                            var listAllEmployee = context.Employee.Where(x => x.Active == true).ToList();
                            var listAllContact = context.Contact.Where(x => x.Active == true && x.ObjectType == "EMP").ToList();

                            #endregion

                            //Nếu có điều kiện
                            if (listNotifiCondition.Count > 0)
                            {
                                //Do something...
                            }
                            //Nếu không có điều kiện
                            else
                            {
                                //Nếu gửi nội bộ
                                if (notifiSetting.SendInternal)
                                {
                                    //Nếu gửi bằng email
                                    if (notifiSetting.IsEmail)
                                    {
                                        #region Lấy danh sách email cần gửi thông báo

                                        var listEmailSendTo = new List<string>();
                                        var listAllUser = context.User.ToList();

                                        #region Lấy email người tham gia ( danh sách thông báo cho )

                                        if (notifiSetting.IsParticipant)
                                        {

                                        }

                                        #endregion

                                        #region Lấy email người phê duyệt

                                        if (notifiSetting.IsApproved)
                                        {

                                        }

                                        #endregion

                                        #region Lấy email người tạo

                                        if (notifiSetting.IsCreated)
                                        {
                                            var Email = LayEmaiNguoiTao(listAllUser, listAllContact, _request.CreatedById);
                                            listEmailSendTo.Add(Email);
                                        }

                                        #endregion

                                        #region Lấy email người phụ trách

                                        if (notifiSetting.IsPersonIncharge)
                                        {

                                        }

                                        #endregion

                                        #region Lấy email của danh sách người đặc biệt

                                        var listEmailSpecial = LayEmaiNguoiDacBiet(listAllUser, listAllContact, context.NotifiSpecial.ToList(), notifiSetting);
                                        listEmailSpecial.ForEach(email =>
                                        {
                                            if (!String.IsNullOrEmpty(email))
                                            {
                                                listEmailSendTo.Add(email.Trim());
                                            }
                                        });

                                        #endregion

                                        listEmailSendTo = listEmailSendTo.Where(x => x != null).Distinct().ToList();

                                        #endregion

                                        #region Kiểm tra xem Gửi ngay hay Đặt lịch gửi 

                                        //Gửi ngay
                                        var subject = ReplaceTokenForContent(context, typeModel, _request,
                                            notifiSetting.EmailTitle, configEntity, note, actionCode);
                                        var content = ReplaceTokenForContent(context, typeModel, _request,
                                            notifiSetting.EmailContent, configEntity, note, actionCode);

                                        #region Build nội dung thay đổi

                                        //string contentModelChange = "";

                                        ////Nếu phải kiểm tra thay đổi của model
                                        //if (checkChange)
                                        //{
                                        //    contentModelChange = ContentModelChange(oldModel, newModel, typeModel);
                                        //}

                                        #endregion

                                        Emailer.SendEmail(context, listEmailSendTo, new List<string>(), new List<string>(), subject, content);

                                        //Đặt lịch gửi             
                                        if (notifiSetting.ObjectBackHourInternal != null && listEmailSendTo.Count > 0)
                                        {

                                        }

                                        #endregion
                                    }
                                }
                            }

                            #endregion
                        }
                    }
                    // Chi tiết đề xuất xin nghỉ và là từ chối phê duyệt
                    else if (typeModel == TypeModel.RequestDetail && actionCode == "REJECT_REQUEST")
                    {
                        var _request = newModel as DeXuatXinNghi;

                        if (_request != null)
                        {
                            #region Kiểm tra xem cấu hình có điều kiện không?

                            var listNotifiCondition = context.NotifiSettingCondition
                                .Where(x => x.NotifiSettingId == notifiSetting.NotifiSettingId).ToList();

                            #region List reference để kiểm tra điều kiện và thông tin dùng để gửi email

                            var listAllEmployee = context.Employee.Where(x => x.Active == true).ToList();
                            var listAllContact = context.Contact.Where(x => x.Active == true && x.ObjectType == "EMP").ToList();

                            #endregion

                            //Nếu có điều kiện
                            if (listNotifiCondition.Count > 0)
                            {
                                //Do something...
                            }
                            //Nếu không có điều kiện
                            else
                            {
                                //Nếu gửi nội bộ
                                if (notifiSetting.SendInternal)
                                {
                                    //Nếu gửi bằng email
                                    if (notifiSetting.IsEmail)
                                    {
                                        #region Lấy danh sách email cần gửi thông báo

                                        var listEmailSendTo = new List<string>();
                                        var listAllUser = context.User.ToList();

                                        #region Lấy email người tham gia ( danh sách thông báo cho )

                                        if (notifiSetting.IsParticipant)
                                        {

                                        }

                                        #endregion

                                        #region Lấy email người phê duyệt

                                        if (notifiSetting.IsApproved)
                                        {

                                        }

                                        #endregion

                                        #region Lấy email người tạo

                                        if (notifiSetting.IsCreated)
                                        {
                                            var Email = LayEmaiNguoiTao(listAllUser, listAllContact, _request.CreatedById);
                                            listEmailSendTo.Add(Email);
                                        }

                                        #endregion

                                        #region Lấy email người phụ trách

                                        if (notifiSetting.IsPersonIncharge)
                                        {

                                        }

                                        #endregion

                                        #region Lấy email của danh sách người đặc biệt

                                        var listEmailSpecial = LayEmaiNguoiDacBiet(listAllUser, listAllContact, context.NotifiSpecial.ToList(), notifiSetting);
                                        listEmailSpecial.ForEach(email =>
                                        {
                                            if (!String.IsNullOrEmpty(email))
                                            {
                                                listEmailSendTo.Add(email.Trim());
                                            }
                                        });

                                        #endregion

                                        listEmailSendTo = listEmailSendTo.Where(x => x != null).Distinct().ToList();

                                        #endregion

                                        #region Kiểm tra xem Gửi ngay hay Đặt lịch gửi 

                                        //Gửi ngay
                                        var subject = ReplaceTokenForContent(context, typeModel, _request,
                                            notifiSetting.EmailTitle, configEntity, note, actionCode);
                                        var content = ReplaceTokenForContent(context, typeModel, _request,
                                            notifiSetting.EmailContent, configEntity, note, actionCode);

                                        #region Build nội dung thay đổi

                                        //string contentModelChange = "";

                                        ////Nếu phải kiểm tra thay đổi của model
                                        //if (checkChange)
                                        //{
                                        //    contentModelChange = ContentModelChange(oldModel, newModel, typeModel);
                                        //}

                                        #endregion

                                        Emailer.SendEmail(context, listEmailSendTo, new List<string>(), new List<string>(), subject, content);

                                        //Đặt lịch gửi             
                                        if (notifiSetting.ObjectBackHourInternal != null && listEmailSendTo.Count > 0)
                                        {

                                        }

                                        #endregion
                                    }
                                }
                            }

                            #endregion
                        }
                    }
                    // Chi tiết đề xuất tăng lương
                    else if (typeModel == TypeModel.DeXuatTangLuongDetail)
                    {
                        var _request = newModel as DeXuatTangLuong;

                        if (_request != null)
                        {
                            #region Kiểm tra xem cấu hình có điều kiện không?

                            var listNotifiCondition = context.NotifiSettingCondition
                                .Where(x => x.NotifiSettingId == notifiSetting.NotifiSettingId).ToList();

                            #region List reference để kiểm tra điều kiện và thông tin dùng để gửi email

                            var listAllEmployee = context.Employee.Where(x => x.Active == true).ToList();
                            var listAllContact = context.Contact.Where(x => x.Active == true && x.ObjectType == "EMP").ToList();

                            #endregion

                            //Nếu có điều kiện
                            if (listNotifiCondition.Count > 0)
                            {
                                //Do something...
                            }
                            //Nếu không có điều kiện
                            else
                            {
                                //Nếu gửi nội bộ
                                if (notifiSetting.SendInternal)
                                {
                                    //Nếu gửi bằng email
                                    if (notifiSetting.IsEmail)
                                    {
                                        #region Lấy danh sách email cần gửi thông báo

                                        var listEmailSendTo = new List<string>();
                                        var listAllUser = context.User.ToList();

                                        #region Lấy email người phê duyệt

                                        if (notifiSetting.IsApproved)
                                        {
                                            //Thêm mail cá nhân và mail công ty của người phê duyệt vào list gửi mail
                                            ListMailNguoiPheDuyet.ForEach(item =>
                                            {
                                                listEmailSendTo.Add(item.Trim());
                                            });
                                        }

                                        #endregion

                                        #region Lấy email người tạo

                                        if (notifiSetting.IsCreated)
                                        {
                                            var Email = LayEmaiNguoiTao(listAllUser, listAllContact, _request.CreatedById.Value);
                                            listEmailSendTo.Add(Email);
                                        }

                                        #endregion

                                        #region Lấy email những người tham gia

                                        if (notifiSetting.IsParticipant)
                                        {
                                            var listEmpThanGiaId = context.DeXuatTangLuongNhanVien.Where(x => x.DeXuatTangLuongId == _request.DeXuatTangLuongId && x.TrangThai == 3).Select(x => x.EmployeeId).ToList();//phê duyệt
                                            var listEmmail = listAllContact.Where(x => x.ObjectType == "EMP" && listEmpThanGiaId.Contains(x.ObjectId)).ToList();
                                            listEmmail.ForEach(email =>
                                            {
                                                if (!String.IsNullOrEmpty(email.WorkEmail))
                                                {
                                                    listEmailSendTo.Add(email.WorkEmail.Trim());
                                                }
                                                else if (!String.IsNullOrEmpty(email.Email))
                                                {
                                                    listEmailSendTo.Add(email.Email.Trim());
                                                }
                                            });
                                        }

                                        #endregion

                                        #region Lấy email của danh sách người đặc biệt

                                        var listEmailSpecial = LayEmaiNguoiDacBiet(listAllUser, listAllContact, context.NotifiSpecial.ToList(), notifiSetting);
                                        listEmailSpecial.ForEach(email =>
                                        {
                                            if (!String.IsNullOrEmpty(email))
                                            {
                                                listEmailSendTo.Add(email.Trim());
                                            }
                                        });

                                        #endregion

                                        listEmailSendTo = listEmailSendTo.Distinct().ToList();

                                        #endregion

                                        #region Kiểm tra xem Gửi ngay hay Đặt lịch gửi 

                                        //Gửi ngay
                                        var subject = ReplaceTokenForContent(context, typeModel, _request,
                                            notifiSetting.EmailTitle, configEntity, note, actionCode);
                                        var content = ReplaceTokenForContent(context, typeModel, _request,
                                            notifiSetting.EmailContent, configEntity, note, actionCode);

                                        #region Build nội dung thay đổi

                                        //string contentModelChange = "";

                                        ////Nếu phải kiểm tra thay đổi của model
                                        //if (checkChange)
                                        //{
                                        //    contentModelChange = ContentModelChange(oldModel, newModel, typeModel);
                                        //}

                                        #endregion

                                        Emailer.SendEmail(context, listEmailSendTo, new List<string>(), new List<string>(), subject, content);

                                        //Đặt lịch gửi             
                                        if (notifiSetting.ObjectBackHourInternal != null && listEmailSendTo.Count > 0)
                                        {

                                        }

                                        #endregion
                                    }
                                }
                            }

                            #endregion
                        }
                    }
                    //Chi tiết đề xuất chức vụ
                    else if (typeModel == TypeModel.DeXuatChucVuDetail)
                    {
                        var _request = newModel as DeXuatThayDoiChucVu;

                        if (_request != null)
                        {
                            #region Kiểm tra xem cấu hình có điều kiện không?

                            var listNotifiCondition = context.NotifiSettingCondition
                                .Where(x => x.NotifiSettingId == notifiSetting.NotifiSettingId).ToList();

                            #region List reference để kiểm tra điều kiện và thông tin dùng để gửi email

                            var listAllEmployee = context.Employee.Where(x => x.Active == true).ToList();
                            var listAllContact = context.Contact.Where(x => x.Active == true && x.ObjectType == "EMP").ToList();

                            #endregion

                            //Nếu có điều kiện
                            if (listNotifiCondition.Count > 0)
                            {
                                //Do something...
                            }
                            //Nếu không có điều kiện
                            else
                            {
                                //Nếu gửi nội bộ
                                if (notifiSetting.SendInternal)
                                {
                                    //Nếu gửi bằng email
                                    if (notifiSetting.IsEmail)
                                    {
                                        #region Lấy danh sách email cần gửi thông báo

                                        var listEmailSendTo = new List<string>();
                                        var listAllUser = context.User.ToList();

                                        #region Lấy email người phê duyệt

                                        if (notifiSetting.IsApproved)
                                        {
                                            //Thêm mail cá nhân và mail công ty của người phê duyệt vào list gửi mail
                                            ListMailNguoiPheDuyet.ForEach(item =>
                                            {
                                                listEmailSendTo.Add(item.Trim());
                                            });
                                        }

                                        #endregion

                                        #region Lấy email người tạo

                                        if (notifiSetting.IsCreated)
                                        {
                                            var Email = LayEmaiNguoiTao(listAllUser, listAllContact, _request.CreatedById.Value);
                                            listEmailSendTo.Add(Email);
                                        }

                                        #endregion

                                        #region Lấy email người đề xuất

                                        if (notifiSetting.IsPersonIncharge)
                                        {

                                            var empRequestEmail = context.Contact.FirstOrDefault(x => x.ObjectId == _request.NguoiDeXuatId && x.ObjectType == "EMP")?.WorkEmail;

                                            if (!String.IsNullOrEmpty(empRequestEmail))
                                            {
                                                listEmailSendTo.Add(empRequestEmail.Trim());
                                            }

                                        }
                                        #endregion

                                        #region Lấy email những người tham gia

                                        if (notifiSetting.IsParticipant)
                                        {
                                            var listEmpThanGiaId = context.NhanVienDeXuatThayDoiChucVu.Where(x => x.DeXuatThayDoiChucVuId == _request.DeXuatThayDoiChucVuId && x.TrangThai == 3).Select(x => x.EmployeeId).ToList(); // Đã duyệt
                                            var listEmmail = listAllContact.Where(x => x.ObjectType == "EMP" && listEmpThanGiaId.Contains(x.ObjectId)).ToList();
                                            listEmmail.ForEach(email =>
                                            {
                                                if (!String.IsNullOrEmpty(email.WorkEmail))
                                                {
                                                    listEmailSendTo.Add(email.WorkEmail.Trim());
                                                }
                                                else if (!String.IsNullOrEmpty(email.Email))
                                                {
                                                    listEmailSendTo.Add(email.Email.Trim());
                                                }
                                            });
                                        }

                                        #endregion

                                        #region Lấy email của danh sách người đặc biệt

                                        var listEmailSpecial = LayEmaiNguoiDacBiet(listAllUser, listAllContact, context.NotifiSpecial.ToList(), notifiSetting);
                                        listEmailSpecial.ForEach(email =>
                                        {
                                            if (!String.IsNullOrEmpty(email))
                                            {
                                                listEmailSendTo.Add(email.Trim());
                                            }
                                        });

                                        #endregion

                                        listEmailSendTo = listEmailSendTo.Distinct().ToList();

                                        #endregion

                                        #region Kiểm tra xem Gửi ngay hay Đặt lịch gửi 

                                        //Gửi ngay
                                        var subject = ReplaceTokenForContent(context, typeModel, _request,
                                            notifiSetting.EmailTitle, configEntity, note, actionCode);
                                        var content = ReplaceTokenForContent(context, typeModel, _request,
                                            notifiSetting.EmailContent, configEntity, note, actionCode);

                                        #region Build nội dung thay đổi

                                        //string contentModelChange = "";

                                        ////Nếu phải kiểm tra thay đổi của model
                                        //if (checkChange)
                                        //{
                                        //    contentModelChange = ContentModelChange(oldModel, newModel, typeModel);
                                        //}

                                        #endregion

                                        Emailer.SendEmail(context, listEmailSendTo, new List<string>(), new List<string>(), subject, content);

                                        //Đặt lịch gửi             
                                        if (notifiSetting.ObjectBackHourInternal != null && listEmailSendTo.Count > 0)
                                        {

                                        }

                                        #endregion
                                    }
                                }
                            }

                            #endregion
                        }
                    }
                    //Chi tiết đề xuất kế hoạch OT
                    else if (typeModel == TypeModel.DeXuatKeHoachOTDetail)
                    {
                        var _request = newModel as KeHoachOt;

                        if (_request != null)
                        {
                            #region Kiểm tra xem cấu hình có điều kiện không?

                            var listNotifiCondition = context.NotifiSettingCondition
                                .Where(x => x.NotifiSettingId == notifiSetting.NotifiSettingId).ToList();

                            #region List reference để kiểm tra điều kiện và thông tin dùng để gửi email

                            var listAllEmployee = context.Employee.Where(x => x.Active == true).ToList();
                            var listAllContact = context.Contact.Where(x => x.Active == true && x.ObjectType == "EMP").ToList();

                            #endregion

                            //Nếu có điều kiện
                            if (listNotifiCondition.Count > 0)
                            {
                                //Do something...
                            }
                            //Nếu không có điều kiện
                            else
                            {
                                //Nếu gửi nội bộ
                                if (notifiSetting.SendInternal)
                                {
                                    //Nếu gửi bằng email
                                    if (notifiSetting.IsEmail)
                                    {
                                        #region Lấy danh sách email cần gửi thông báo

                                        var listEmailSendTo = new List<string>();
                                        var listAllUser = context.User.ToList();

                                        #region Lấy email người phê duyệt

                                        if (notifiSetting.IsApproved)
                                        {
                                            //Thêm mail cá nhân và mail công ty của người phê duyệt vào list gửi mail
                                            ListMailNguoiPheDuyet.ForEach(item =>
                                            {
                                                listEmailSendTo.Add(item.Trim());
                                            });
                                        }

                                        #endregion

                                        #region Lấy email người tham gia ( danh sách thông báo cho )

                                        if (notifiSetting.IsParticipant)
                                        {
                                            //Gửi mail cho các trưởng bộ phận phòng ban
                                            var listOrgId = context.KeHoachOtPhongBan.Where(x => x.KeHoachOtId == _request.KeHoachOtId).Select(x => x.OrganizationId).ToList();
                                            var listTruongBoPhamEmpId = context.ThanhVienPhongBan.Where(x => listOrgId.Contains(x.OrganizationId) && x.IsManager == 1).Select(x => x.EmployeeId).ToList();
                                            var listContact = listAllContact.Where(x => x.ObjectType == "EMP" && listTruongBoPhamEmpId.Contains(x.ObjectId)).ToList();
                                            listContact.ForEach(item =>
                                            {
                                                if (!String.IsNullOrEmpty(item.WorkEmail))
                                                {
                                                    listEmailSendTo.Add(item.WorkEmail.Trim());
                                                }
                                                else if (!String.IsNullOrEmpty(item.Email))
                                                {
                                                    listEmailSendTo.Add(item.Email.Trim());
                                                }
                                            });
                                        }
                                        #endregion


                                        #region Lấy email người tạo

                                        if (notifiSetting.IsCreated)
                                        {
                                            var Email = LayEmaiNguoiTao(listAllUser, listAllContact, _request.CreatedById.Value);
                                            listEmailSendTo.Add(Email);
                                        }

                                        #endregion

                                        #region Lấy email người đề xuất

                                        if (notifiSetting.IsPersonIncharge)
                                        {

                                            var empRequestEmail = context.Contact.FirstOrDefault(x => x.ObjectId == _request.NguoiDeXuatId && x.ObjectType == "EMP")?.Email;

                                            if (!String.IsNullOrEmpty(empRequestEmail))
                                            {
                                                listEmailSendTo.Add(empRequestEmail.Trim());
                                            }

                                        }

                                        #endregion

                                        #region Lấy email của danh sách người đặc biệt

                                        var listEmailSpecial = LayEmaiNguoiDacBiet(listAllUser, listAllContact, context.NotifiSpecial.ToList(), notifiSetting);
                                        listEmailSpecial.ForEach(email =>
                                        {
                                            if (!String.IsNullOrEmpty(email))
                                            {
                                                listEmailSendTo.Add(email.Trim());
                                            }
                                        });

                                        #endregion

                                        listEmailSendTo = listEmailSendTo.Distinct().ToList();

                                        #endregion

                                        #region Kiểm tra xem Gửi ngay hay Đặt lịch gửi 

                                        //Gửi ngay
                                        var subject = ReplaceTokenForContent(context, typeModel, _request,
                                            notifiSetting.EmailTitle, configEntity, note, actionCode);
                                        var content = ReplaceTokenForContent(context, typeModel, _request,
                                            notifiSetting.EmailContent, configEntity, note, actionCode);

                                        #region Build nội dung thay đổi

                                        //string contentModelChange = "";

                                        ////Nếu phải kiểm tra thay đổi của model
                                        //if (checkChange)
                                        //{
                                        //    contentModelChange = ContentModelChange(oldModel, newModel, typeModel);
                                        //}

                                        #endregion

                                        Emailer.SendEmail(context, listEmailSendTo, new List<string>(), new List<string>(), subject, content);

                                        //Đặt lịch gửi             
                                        if (notifiSetting.ObjectBackHourInternal != null && listEmailSendTo.Count > 0)
                                        {

                                        }

                                        #endregion
                                    }
                                }
                            }

                            #endregion
                        }
                    }
                    //Chi tiết đăng ký OT
                    else if (typeModel == TypeModel.DeXuatDangKyOTDetail)
                    {
                        var _request = newModel as KeHoachOt;

                        if (_request != null)
                        {
                            #region Kiểm tra xem cấu hình có điều kiện không?

                            var listNotifiCondition = context.NotifiSettingCondition
                                .Where(x => x.NotifiSettingId == notifiSetting.NotifiSettingId).ToList();

                            #region List reference để kiểm tra điều kiện và thông tin dùng để gửi email

                            var listAllEmployee = context.Employee.Where(x => x.Active == true).ToList();
                            var listAllContact = context.Contact.Where(x => x.Active == true && x.ObjectType == "EMP").ToList();

                            #endregion

                            //Nếu có điều kiện
                            if (listNotifiCondition.Count > 0)
                            {
                                //Do something...
                            }
                            //Nếu không có điều kiện
                            else
                            {
                                //Nếu gửi nội bộ
                                if (notifiSetting.SendInternal)
                                {
                                    //Nếu gửi bằng email
                                    if (notifiSetting.IsEmail)
                                    {
                                        #region Lấy danh sách email cần gửi thông báo

                                        var listEmailSendTo = new List<string>();
                                        var listAllUser = context.User.ToList();

                                        #region Lấy email người phê duyệt

                                        if (notifiSetting.IsApproved)
                                        {
                                            //Thêm mail cá nhân và mail công ty của người phê duyệt vào list gửi mail
                                            ListMailNguoiPheDuyet.ForEach(item =>
                                            {
                                                listEmailSendTo.Add(item.Trim());
                                            });
                                        }

                                        #endregion

                                        #region Lấy email người tạo

                                        if (notifiSetting.IsCreated)
                                        {
                                            var Email = LayEmaiNguoiTao(listAllUser, listAllContact, _request.CreatedById.Value);
                                            listEmailSendTo.Add(Email);
                                        }

                                        #endregion

                                        #region Lấy email người đề xuất

                                        if (notifiSetting.IsPersonIncharge)
                                        {

                                            var empRequestEmail = context.Contact.FirstOrDefault(x => x.ObjectId == _request.NguoiDeXuatId && x.ObjectType == "EMP")?.Email;

                                            if (!String.IsNullOrEmpty(empRequestEmail))
                                            {
                                                listEmailSendTo.Add(empRequestEmail.Trim());
                                            }

                                        }

                                        #endregion

                                        #region Lấy email của danh sách người đặc biệt

                                        var listEmailSpecial = LayEmaiNguoiDacBiet(listAllUser, listAllContact, context.NotifiSpecial.ToList(), notifiSetting);
                                        listEmailSpecial.ForEach(email =>
                                        {
                                            if (!String.IsNullOrEmpty(email))
                                            {
                                                listEmailSendTo.Add(email.Trim());
                                            }
                                        });

                                        #endregion

                                        listEmailSendTo = listEmailSendTo.Distinct().ToList();

                                        #endregion

                                        #region Kiểm tra xem Gửi ngay hay Đặt lịch gửi 

                                        //Gửi ngay
                                        var subject = ReplaceTokenForContent(context, typeModel, _request,
                                            notifiSetting.EmailTitle, configEntity, note, actionCode);
                                        var content = ReplaceTokenForContent(context, typeModel, _request,
                                            notifiSetting.EmailContent, configEntity, note, actionCode);

                                        #region Build nội dung thay đổi

                                        //string contentModelChange = "";

                                        ////Nếu phải kiểm tra thay đổi của model
                                        //if (checkChange)
                                        //{
                                        //    contentModelChange = ContentModelChange(oldModel, newModel, typeModel);
                                        //}

                                        #endregion

                                        Emailer.SendEmail(context, listEmailSendTo, new List<string>(), new List<string>(), subject, content);

                                        //Đặt lịch gửi             
                                        if (notifiSetting.ObjectBackHourInternal != null && listEmailSendTo.Count > 0)
                                        {

                                        }

                                        #endregion
                                    }
                                }
                            }

                            #endregion
                        }
                    }
                    // Phỏng vấn
                    else if (typeModel == TypeModel.CandidateInterview)
                    {
                        var listNotifiCondition = context.NotifiSettingCondition
                            .Where(x => x.NotifiSettingId == notifiSetting.NotifiSettingId).ToList();

                        //Nếu có điều kiện
                        if (listNotifiCondition.Count > 0)
                        {
                            //Do something...
                        }
                        //Nếu không có điều kiện
                        else
                        {     //Nếu gửi nội bộ
                            if (notifiSetting.SendInternal)
                            {
                                //Nếu gửi bằng email
                                if (notifiSetting.IsEmail)
                                {
                                    #region Lấy danh sách email cần gửi thông báo

                                    var lstScheCandidateId = context.InterviewSchedule.Where(x => lstInterviewId.Contains(x.InterviewScheduleId)).Select(x => x.CandidateId).ToList();
                                    var lstAllCandidate = context.Candidate.Where(x => lstScheCandidateId.Contains(x.CandidateId)).ToList();

                                    // Danh sách lịch phỏng vấn chưa gửi email
                                    var lstScheInterview = context.InterviewSchedule.Where(x => lstInterviewId.Contains(x.InterviewScheduleId) && x.Status != 1).ToList();

                                    var lstInterview = context.InterviewSchedule.Where(x => lstInterviewId.Contains(x.InterviewScheduleId)).ToList();
                                    lstInterview.ForEach(scheId =>
                                    {
                                        var listEmailSendTo = new List<string>();
                                        var listEmailSendToCandidate = new List<string>();

                                        var candidate = context.Candidate.FirstOrDefault(x => x.CandidateId == lstScheInterview.FirstOrDefault(a => a.InterviewScheduleId == scheId.InterviewScheduleId).CandidateId);
                                        // Gửi email cho Ứng viên        
                                        if (!string.IsNullOrEmpty(candidate?.Email))
                                        {
                                            listEmailSendToCandidate.Add(candidate.Email);
                                            var subjectCandidate = ReplaceTokenForContent(context, typeModel, candidate,
                                           notifiSetting.EmailTitle, configEntity, note, actionCode, null, scheId.InterviewScheduleId);
                                            var contentCandidate = ReplaceTokenForContent(context, typeModel, candidate,
                                                notifiSetting.EmailContent, configEntity, note, actionCode, null, scheId.InterviewScheduleId);

                                            var sendMail = Emailer.SendMailWithIcsAttachment(context, listEmailSendToCandidate, null, subjectCandidate, contentCandidate, scheId.InterviewDate, scheId.InterviewDate.AddHours(1), Guid.Empty, scheId.Address, false);
                                            Emailer.SendEmail(context, listEmailSendToCandidate, new List<string>(), new List<string>(), subjectCandidate, contentCandidate);
                                        }

                                        // Gửi email cho người phỏng vấn
                                        var lstScheEmp = context.InterviewScheduleMapping.Where(x => x.InterviewScheduleId == scheId.InterviewScheduleId).ToList();
                                        lstScheEmp?.ForEach(scheEmp =>
                                        {
                                            var email = context.Contact.FirstOrDefault(f => f.ObjectId == scheEmp.EmployeeId)?.Email;
                                            if (lstScheInterview.Count() > 0)
                                            {
                                                //Gửi cho người phỏng vấn
                                                if (!string.IsNullOrEmpty(email))
                                                {
                                                    listEmailSendTo.Add(email.Trim());
                                                }
                                                var subject = ReplaceTokenForContent(context, typeModel, candidate,
                                                notifiSetting.EmailTitle, configEntity, note, actionCode, null, scheId.InterviewScheduleId);
                                                var content = ReplaceTokenForContent(context, typeModel, candidate,
                                                    notifiSetting.EmailContent, configEntity, note, actionCode, null, scheId.InterviewScheduleId);
                                                var sendMail = Emailer.SendMailWithIcsAttachment(context, listEmailSendToCandidate, null, subject, content, scheId.InterviewDate, scheId.InterviewDate.AddHours(1), Guid.Empty, scheId.Address, false);
                                                Emailer.SendEmail(context, listEmailSendTo, new List<string>(), new List<string>(), subject, content);
                                            }
                                        });
                                    });

                                    lstScheInterview?.ForEach(item =>
                                    {
                                        item.Status = 1; // Đã gửi email
                                    });
                                    context.InterviewSchedule.UpdateRange(lstScheInterview);
                                    context.SaveChanges();
                                }
                            }
                        }
                        #endregion
                    }
                    //Thông báo khi nhân viên sắp hết hạn hợp đồng
                    else if (typeModel == TypeModel.EmployeeInfor && actionCode != "DEADLINE_SUBMISSION")
                    {
                        var _listHopDong = newModel as List<HopDongNhanSu>;

                        if (_listHopDong.Count() > 0)
                        {
                            #region Kiểm tra xem cấu hình có điều kiện không?

                            var listNotifiCondition = context.NotifiSettingCondition
                                .Where(x => x.NotifiSettingId == notifiSetting.NotifiSettingId).ToList();

                            #region List reference để kiểm tra điều kiện và thông tin dùng để gửi email

                            var listAllEmployee = context.Employee.Where(x => x.Active == true).ToList();
                            var listAllContact = context.Contact.Where(x => x.Active == true && x.ObjectType == "EMP").ToList();

                            #endregion

                            //Nếu có điều kiện
                            if (listNotifiCondition.Count > 0)
                            {
                                //Do something...
                            }
                            //Nếu không có điều kiện
                            else
                            {
                                //Nếu gửi nội bộ
                                if (notifiSetting.SendInternal)
                                {
                                    //Nếu gửi bằng email
                                    if (notifiSetting.IsEmail)
                                    {
                                        #region Lấy danh sách email cần gửi thông báo

                                        var listEmailSendTo = new List<string>();
                                        var listAllUser = context.User.ToList();

                                        #region Lấy email của danh sách người đặc biệt

                                        var listEmailSpecial = LayEmaiNguoiDacBiet(listAllUser, listAllContact, context.NotifiSpecial.ToList(), notifiSetting);
                                        listEmailSpecial.ForEach(email =>
                                        {
                                            if (!String.IsNullOrEmpty(email))
                                            {
                                                listEmailSendTo.Add(email.Trim());
                                            }
                                        });

                                        #endregion

                                        listEmailSendTo = listEmailSendTo.Distinct().ToList();

                                        #endregion

                                        #region Kiểm tra xem Gửi ngay hay Đặt lịch gửi 

                                        //Gửi ngay
                                        var subject = ReplaceTokenForContent(context, typeModel, _listHopDong,
                                            notifiSetting.EmailTitle, configEntity, note, actionCode);
                                        var content = ReplaceTokenForContent(context, typeModel, _listHopDong,
                                            notifiSetting.EmailContent, configEntity, note, actionCode);

                                        #region Build nội dung thay đổi

                                        //string contentModelChange = "";

                                        ////Nếu phải kiểm tra thay đổi của model
                                        //if (checkChange)
                                        //{
                                        //    contentModelChange = ContentModelChange(oldModel, newModel, typeModel);
                                        //}

                                        #endregion

                                        Emailer.SendEmail(context, listEmailSendTo, new List<string>(), new List<string>(), subject, content);

                                        //Đặt lịch gửi             
                                        if (notifiSetting.ObjectBackHourInternal != null && listEmailSendTo.Count > 0)
                                        {

                                        }

                                        #endregion
                                    }
                                }
                            }

                            #endregion
                        }
                    }
                    //Thông báo khi nhân viên sắp đến hạn nộp hồ sơ
                    else if (typeModel == TypeModel.EmployeeInfor && actionCode == "DEADLINE_SUBMISSION")
                    {
                        var _listTaiLieu = newModel as List<TaiLieuNhanVien>;

                        if (_listTaiLieu.Count() > 0)
                        {
                            #region Kiểm tra xem cấu hình có điều kiện không?

                            var listNotifiCondition = context.NotifiSettingCondition
                                .Where(x => x.NotifiSettingId == notifiSetting.NotifiSettingId).ToList();

                            #region List reference để kiểm tra điều kiện và thông tin dùng để gửi email

                            var listAllEmployee = context.Employee.Where(x => x.Active == true).ToList();
                            var listAllContact = context.Contact.Where(x => x.Active == true && x.ObjectType == "EMP").ToList();

                            #endregion

                            //Nếu có điều kiện
                            if (listNotifiCondition.Count > 0)
                            {
                                //Do something...
                            }
                            //Nếu không có điều kiện
                            else
                            {
                                //Nếu gửi nội bộ
                                if (notifiSetting.SendInternal)
                                {
                                    //Nếu gửi bằng email
                                    if (notifiSetting.IsEmail)
                                    {
                                        #region Lấy danh sách email cần gửi thông báo

                                        var listEmailSendTo = new List<string>();
                                        var listAllUser = context.User.ToList();

                                        #region Lấy email của danh sách người đặc biệt

                                        var listEmailSpecial = LayEmaiNguoiDacBiet(listAllUser, listAllContact, context.NotifiSpecial.ToList(), notifiSetting);
                                        listEmailSpecial.ForEach(email =>
                                        {
                                            if (!String.IsNullOrEmpty(email))
                                            {
                                                listEmailSendTo.Add(email.Trim());
                                            }
                                        });

                                        #endregion

                                        listEmailSendTo = listEmailSendTo.Distinct().ToList();

                                        #endregion

                                        #region Kiểm tra xem Gửi ngay hay Đặt lịch gửi 

                                        //Gửi ngay
                                        var subject = ReplaceTokenForContent(context, typeModel, _listTaiLieu,
                                            notifiSetting.EmailTitle, configEntity, note, actionCode);
                                        var content = ReplaceTokenForContent(context, typeModel, _listTaiLieu,
                                            notifiSetting.EmailContent, configEntity, note, actionCode);

                                        #region Build nội dung thay đổi

                                        //string contentModelChange = "";

                                        ////Nếu phải kiểm tra thay đổi của model
                                        //if (checkChange)
                                        //{
                                        //    contentModelChange = ContentModelChange(oldModel, newModel, typeModel);
                                        //}

                                        #endregion

                                        Emailer.SendEmail(context, listEmailSendTo, new List<string>(), new List<string>(), subject, content);

                                        //Đặt lịch gửi             
                                        if (notifiSetting.ObjectBackHourInternal != null && listEmailSendTo.Count > 0)
                                        {

                                        }

                                        #endregion
                                    }
                                }
                            }

                            #endregion
                        }
                    }
                    //Thông báo khi nhân viên, quản lý, trưởng phòng hoàn thành đánh giá
                    else if (typeModel == TypeModel.ThucHienDanhGia)
                    {
                        var _request = newModel as DanhGiaNhanVien;

                        if (_request != null)
                        {
                            #region Kiểm tra xem cấu hình có điều kiện không?

                            var listNotifiCondition = context.NotifiSettingCondition
                                .Where(x => x.NotifiSettingId == notifiSetting.NotifiSettingId).ToList();

                            #region List reference để kiểm tra điều kiện và thông tin dùng để gửi email

                            var listAllEmployee = context.Employee.Where(x => x.Active == true).ToList();
                            var listAllContact = context.Contact.Where(x => x.Active == true && x.ObjectType == "EMP").ToList();

                            #endregion

                            //Nếu có điều kiện
                            if (listNotifiCondition.Count > 0)
                            {
                                //Do something...
                            }
                            //Nếu không có điều kiện
                            else
                            {
                                //Nếu gửi nội bộ
                                if (notifiSetting.SendInternal)
                                {
                                    //Nếu gửi bằng email
                                    if (notifiSetting.IsEmail)
                                    {
                                        #region Lấy danh sách email cần gửi thông báo

                                        var listEmailSendTo = new List<string>();
                                        var listAllUser = context.User.ToList();


                                        #region Lấy email người phê duyệt

                                        if (notifiSetting.IsApproved)
                                        {
                                            //Thêm mail cá nhân và mail công ty của người phê duyệt vào list gửi mail
                                            ListMailNguoiPheDuyet.ForEach(item =>
                                            {
                                                listEmailSendTo.Add(item.Trim());
                                            });
                                        }

                                        #endregion

                                        #region Lấy email người tạo

                                        if (notifiSetting.IsCreated)
                                        {
                                        }

                                        #endregion

                                        #region Lấy email người đề xuất

                                        if (notifiSetting.IsPersonIncharge)
                                        {
                                        }

                                        #endregion

                                        #region Lấy email của danh sách người đặc biệt

                                        var listEmailSpecial = LayEmaiNguoiDacBiet(listAllUser, listAllContact, context.NotifiSpecial.ToList(), notifiSetting);
                                        listEmailSpecial.ForEach(email =>
                                        {
                                            if (!String.IsNullOrEmpty(email))
                                            {
                                                listEmailSendTo.Add(email.Trim());
                                            }
                                        });

                                        #endregion

                                        listEmailSendTo = listEmailSendTo.Distinct().ToList();

                                        #endregion

                                        #region Kiểm tra xem Gửi ngay hay Đặt lịch gửi 

                                        //Gửi ngay
                                        var subject = ReplaceTokenForContent(context, typeModel, _request,
                                            notifiSetting.EmailTitle, configEntity, note, actionCode);
                                        var content = ReplaceTokenForContent(context, typeModel, _request,
                                            notifiSetting.EmailContent, configEntity, note, actionCode);

                                        #region Build nội dung thay đổi

                                        //string contentModelChange = "";

                                        ////Nếu phải kiểm tra thay đổi của model
                                        //if (checkChange)
                                        //{
                                        //    contentModelChange = ContentModelChange(oldModel, newModel, typeModel);
                                        //}

                                        #endregion

                                        Emailer.SendEmail(context, listEmailSendTo, new List<string>(), new List<string>(), subject, content);

                                        //Đặt lịch gửi             
                                        if (notifiSetting.ObjectBackHourInternal != null && listEmailSendTo.Count > 0)
                                        {

                                        }

                                        #endregion
                                    }
                                }
                            }

                            #endregion
                        }
                    }
                    //Thông báo khi tài sản gần đến hạn bảo trì bảo dưỡng
                    else if (typeModel == TypeModel.TaiSanInfor && actionCode == "TAISAN_BD")
                    {
                        var _listAsset = newModel as List<AssetEntityModel>;

                        if (_listAsset.Count() > 0)
                        {
                            #region Kiểm tra xem cấu hình có điều kiện không?

                            var listNotifiCondition = context.NotifiSettingCondition
                                .Where(x => x.NotifiSettingId == notifiSetting.NotifiSettingId).ToList();

                            #region List reference để kiểm tra điều kiện và thông tin dùng để gửi email

                            var listAllEmployee = context.Employee.Where(x => x.Active == true).ToList();
                            var listAllContact = context.Contact.Where(x => x.Active == true && x.ObjectType == "EMP").ToList();

                            #endregion

                            //Nếu có điều kiện
                            if (listNotifiCondition.Count > 0)
                            {
                                //Do something...
                            }
                            //Nếu không có điều kiện
                            else
                            {
                                //Nếu gửi nội bộ
                                if (notifiSetting.SendInternal)
                                {
                                    //Nếu gửi bằng email
                                    if (notifiSetting.IsEmail)
                                    {
                                        #region Lấy danh sách email cần gửi thông báo

                                        var listEmailSendTo = new List<string>();
                                        var listAllUser = context.User.ToList();

                                        #region Lấy email của danh sách người đặc biệt

                                        var listEmailSpecial = LayEmaiNguoiDacBiet(listAllUser, listAllContact, context.NotifiSpecial.ToList(), notifiSetting);
                                        listEmailSpecial.ForEach(email =>
                                        {
                                            if (!String.IsNullOrEmpty(email))
                                            {
                                                listEmailSendTo.Add(email.Trim());
                                            }
                                        });

                                        #endregion

                                        listEmailSendTo = listEmailSendTo.Distinct().ToList();

                                        #endregion

                                        #region Kiểm tra xem Gửi ngay hay Đặt lịch gửi 

                                        //Gửi ngay
                                        var subject = ReplaceTokenForContent(context, typeModel, _listAsset,
                                            notifiSetting.EmailTitle, configEntity, note, actionCode);
                                        var content = ReplaceTokenForContent(context, typeModel, _listAsset,
                                            notifiSetting.EmailContent, configEntity, note, actionCode);

                                        #region Build nội dung thay đổi


                                        #endregion

                                        Emailer.SendEmail(context, listEmailSendTo, new List<string>(), new List<string>(), subject, content);

                                        //Đặt lịch gửi             
                                        if (notifiSetting.ObjectBackHourInternal != null && listEmailSendTo.Count > 0)
                                        {

                                        }
                                        #endregion
                                    }
                                }
                            }
                            #endregion
                        }
                    }
                    // Chi tiết kỳ lương và là phê duyệt các bước
                    else if (typeModel == TypeModel.KyLuong && actionCode == "SEND_APPROVAL")
                    {
                        var _request = newModel as KyLuong;

                        if (_request != null)
                        {
                            #region Kiểm tra xem cấu hình có điều kiện không?

                            var listNotifiCondition = context.NotifiSettingCondition
                                .Where(x => x.NotifiSettingId == notifiSetting.NotifiSettingId).ToList();

                            #region List reference để kiểm tra điều kiện và thông tin dùng để gửi email

                            var listAllEmployee = context.Employee.Where(x => x.Active == true).ToList();
                            var listAllContact = context.Contact.Where(x => x.Active == true && x.ObjectType == "EMP").ToList();

                            #endregion

                            //Nếu có điều kiện
                            if (listNotifiCondition.Count > 0)
                            {
                                //Do something...
                            }
                            //Nếu không có điều kiện
                            else
                            {
                                //Nếu gửi nội bộ
                                if (notifiSetting.SendInternal)
                                {
                                    //Nếu gửi bằng email
                                    if (notifiSetting.IsEmail)
                                    {
                                        #region Lấy danh sách email cần gửi thông báo

                                        var listEmailSendTo = new List<string>();
                                        var listAllUser = context.User.ToList();

                                        #region Lấy email người tham gia ( danh sách thông báo cho )

                                        if (notifiSetting.IsParticipant)
                                        {

                                        }

                                        #endregion

                                        #region Lấy email người phê duyệt

                                        if (notifiSetting.IsApproved)
                                        {
                                            if (notifiSetting.IsApproved)
                                            {
                                                listEmailSendTo.AddRange(ListMailNguoiPheDuyet);
                                            }
                                        }

                                        #endregion

                                        #region Lấy email người tạo

                                        if (notifiSetting.IsCreated)
                                        {
                                            var Email = LayEmaiNguoiTao(listAllUser, listAllContact, _request.CreatedById.Value);
                                            listEmailSendTo.Add(Email);
                                        }

                                        #endregion

                                        #region Lấy email người phụ trách

                                        if (notifiSetting.IsPersonIncharge)
                                        {

                                        }

                                        #endregion

                                        #region Lấy email của danh sách người đặc biệt

                                        var listEmailSpecial = LayEmaiNguoiDacBiet(listAllUser, listAllContact, context.NotifiSpecial.ToList(), notifiSetting);
                                        listEmailSpecial.ForEach(email =>
                                        {
                                            if (!String.IsNullOrEmpty(email))
                                            {
                                                listEmailSendTo.Add(email.Trim());
                                            }
                                        });



                                        #endregion

                                        listEmailSendTo = listEmailSendTo.Where(x => x != null).Distinct().ToList();

                                        #endregion

                                        #region Kiểm tra xem Gửi ngay hay Đặt lịch gửi 

                                        //Gửi ngay
                                        var subject = ReplaceTokenForContent(context, typeModel, _request,
                                            notifiSetting.EmailTitle, configEntity, note, actionCode);
                                        var content = ReplaceTokenForContent(context, typeModel, _request,
                                            notifiSetting.EmailContent, configEntity, note, actionCode);

                                        #region Build nội dung thay đổi

                                        //string contentModelChange = "";

                                        ////Nếu phải kiểm tra thay đổi của model
                                        //if (checkChange)
                                        //{
                                        //    contentModelChange = ContentModelChange(oldModel, newModel, typeModel);
                                        //}

                                        #endregion

                                        Emailer.SendEmail(context, listEmailSendTo, new List<string>(), new List<string>(), subject, content);

                                        //Đặt lịch gửi             
                                        if (notifiSetting.ObjectBackHourInternal != null && listEmailSendTo.Count > 0)
                                        {

                                        }

                                        #endregion
                                    }
                                }
                            }

                            #endregion
                        }
                    }
                    // Chi tiết kỳ lương và là phê duyệt bước cuối
                    else if (typeModel == TypeModel.KyLuong && (actionCode == "APPRO_REQUEST_FINAL" || actionCode == "REJECT"))
                    {
                        var _request = newModel as KyLuong;

                        if (_request != null)
                        {
                            #region Kiểm tra xem cấu hình có điều kiện không?

                            var listNotifiCondition = context.NotifiSettingCondition
                                .Where(x => x.NotifiSettingId == notifiSetting.NotifiSettingId).ToList();

                            #region List reference để kiểm tra điều kiện và thông tin dùng để gửi email

                            var listAllEmployee = context.Employee.Where(x => x.Active == true).ToList();
                            var listAllContact = context.Contact.Where(x => x.Active == true && x.ObjectType == "EMP").ToList();

                            #endregion

                            //Nếu có điều kiện
                            if (listNotifiCondition.Count > 0)
                            {
                                //Do something...
                            }
                            //Nếu không có điều kiện
                            else
                            {
                                //Nếu gửi nội bộ
                                if (notifiSetting.SendInternal)
                                {
                                    //Nếu gửi bằng email
                                    if (notifiSetting.IsEmail)
                                    {
                                        #region Lấy danh sách email cần gửi thông báo

                                        var listEmailSendTo = new List<string>();
                                        var listAllUser = context.User.ToList();

                                        #region Lấy email người tham gia ( danh sách thông báo cho )

                                        if (notifiSetting.IsParticipant)
                                        {

                                        }

                                        #endregion

                                        #region Lấy email người phê duyệt

                                        if (notifiSetting.IsApproved)
                                        {

                                        }

                                        #endregion

                                        #region Lấy email người tạo

                                        if (notifiSetting.IsCreated)
                                        {
                                            var Email = LayEmaiNguoiTao(listAllUser, listAllContact, _request.CreatedById.Value);
                                            listEmailSendTo.Add(Email);
                                        }

                                        #endregion

                                        #region Lấy email người phụ trách

                                        if (notifiSetting.IsPersonIncharge)
                                        {

                                        }

                                        #endregion

                                        #region Lấy email của danh sách người đặc biệt

                                        var listEmailSpecial = LayEmaiNguoiDacBiet(listAllUser, listAllContact, context.NotifiSpecial.ToList(), notifiSetting);
                                        listEmailSpecial.ForEach(email =>
                                        {
                                            if (!String.IsNullOrEmpty(email))
                                            {
                                                listEmailSendTo.Add(email.Trim());
                                            }
                                        });



                                        #endregion

                                        listEmailSendTo = listEmailSendTo.Where(x => x != null).Distinct().ToList();

                                        #endregion

                                        #region Kiểm tra xem Gửi ngay hay Đặt lịch gửi 

                                        //Gửi ngay
                                        var subject = ReplaceTokenForContent(context, typeModel, _request,
                                            notifiSetting.EmailTitle, configEntity, note, actionCode);
                                        var content = ReplaceTokenForContent(context, typeModel, _request,
                                            notifiSetting.EmailContent, configEntity, note, actionCode);

                                        #region Build nội dung thay đổi

                                        //string contentModelChange = "";

                                        ////Nếu phải kiểm tra thay đổi của model
                                        //if (checkChange)
                                        //{
                                        //    contentModelChange = ContentModelChange(oldModel, newModel, typeModel);
                                        //}

                                        #endregion

                                        Emailer.SendEmail(context, listEmailSendTo, new List<string>(), new List<string>(), subject, content);

                                        //Đặt lịch gửi             
                                        if (notifiSetting.ObjectBackHourInternal != null && listEmailSendTo.Count > 0)
                                        {

                                        }

                                        #endregion
                                    }
                                }
                            }

                            #endregion
                        }
                    }
                    // Phiếu lương gửi nhân viên
                    else if (typeModel == TypeModel.PhieuLuong && actionCode == "SEND_EMP")
                    {
                        var _request = newModel as PhieuLuongModel;

                        if (_request != null)
                        {
                            #region Kiểm tra xem cấu hình có điều kiện không?

                            var listNotifiCondition = context.NotifiSettingCondition
                                .Where(x => x.NotifiSettingId == notifiSetting.NotifiSettingId).ToList();

                            #region List reference để kiểm tra điều kiện và thông tin dùng để gửi email

                            var listAllEmployee = context.Employee.Where(x => x.Active == true).ToList();
                            var listAllContact = context.Contact.Where(x => x.Active == true && x.ObjectType == "EMP").ToList();

                            #endregion

                            //Nếu có điều kiện
                            if (listNotifiCondition.Count > 0)
                            {
                                //Do something...
                            }
                            //Nếu không có điều kiện
                            else
                            {
                                //Nếu gửi nội bộ
                                if (notifiSetting.SendInternal)
                                {
                                    //Nếu gửi bằng email
                                    if (notifiSetting.IsEmail)
                                    {
                                        #region Lấy danh sách email cần gửi thông báo

                                        var listEmailSendTo = new List<string>();
                                        var listAllUser = context.User.ToList();

                                        #region Lấy email người tham gia ( danh sách thông báo cho )

                                        if (notifiSetting.IsParticipant)
                                        {
                                            listEmailSendTo.Add(_request.WorkEmail.Trim());
                                        }

                                        #endregion

                                        #region Lấy email người phê duyệt

                                        if (notifiSetting.IsApproved)
                                        {

                                        }

                                        #endregion

                                        #region Lấy email người tạo

                                        if (notifiSetting.IsCreated)
                                        {
                                            var Email = LayEmaiNguoiTao(listAllUser, listAllContact, _request.CreatedByEmpId.Value, 0);
                                            listEmailSendTo.Add(Email);
                                        }

                                        #endregion

                                        #region Lấy email người phụ trách

                                        if (notifiSetting.IsPersonIncharge)
                                        {

                                        }

                                        #endregion

                                        #region Lấy email của danh sách người đặc biệt

                                        var listEmailSpecial = LayEmaiNguoiDacBiet(listAllUser, listAllContact, context.NotifiSpecial.ToList(), notifiSetting);
                                        listEmailSpecial.ForEach(email =>
                                        {
                                            if (!String.IsNullOrEmpty(email))
                                            {
                                                listEmailSendTo.Add(email.Trim());
                                            }
                                        });

                                        #endregion

                                        listEmailSendTo = listEmailSendTo.Where(x => x != null).Distinct().ToList();

                                        #endregion

                                        #region Kiểm tra xem Gửi ngay hay Đặt lịch gửi 

                                        //Gửi ngay
                                        var subject = ReplaceTokenForContent(context, typeModel, _request,
                                            notifiSetting.EmailTitle, configEntity, note, actionCode);
                                        var content = ReplaceTokenForContent(context, typeModel, _request,
                                            notifiSetting.EmailContent, configEntity, note, actionCode);

                                        #region Build nội dung thay đổi

                                        //string contentModelChange = "";

                                        ////Nếu phải kiểm tra thay đổi của model
                                        //if (checkChange)
                                        //{
                                        //    contentModelChange = ContentModelChange(oldModel, newModel, typeModel);
                                        //}

                                        #endregion

                                        Emailer.SendEmail(context, listEmailSendTo, new List<string>(), new List<string>(), subject, content);

                                        //Đặt lịch gửi             
                                        if (notifiSetting.ObjectBackHourInternal != null && listEmailSendTo.Count > 0)
                                        {

                                        }

                                        #endregion
                                    }
                                }
                            }

                            #endregion
                        }
                    }
                    // Chi tiết đề xuất công tác
                    else if (typeModel == TypeModel.DeXuatCongTac)
                    {
                        var _request = newModel as DeXuatCongTac;

                        if (_request != null)
                        {
                            #region Kiểm tra xem cấu hình có điều kiện không?

                            var listNotifiCondition = context.NotifiSettingCondition
                                .Where(x => x.NotifiSettingId == notifiSetting.NotifiSettingId).ToList();

                            #region List reference để kiểm tra điều kiện và thông tin dùng để gửi email

                            var listAllEmployee = context.Employee.Where(x => x.Active == true).ToList();
                            var listAllContact = context.Contact.Where(x => x.Active == true && x.ObjectType == "EMP").ToList();

                            #endregion

                            //Nếu có điều kiện
                            if (listNotifiCondition.Count > 0)
                            {
                                //Do something...
                            }
                            //Nếu không có điều kiện
                            else
                            {
                                //Nếu gửi nội bộ
                                if (notifiSetting.SendInternal)
                                {
                                    //Nếu gửi bằng email
                                    if (notifiSetting.IsEmail)
                                    {
                                        #region Lấy danh sách email cần gửi thông báo

                                        var listEmailSendTo = new List<string>();
                                        var listAllUser = context.User.ToList();

                                        #region Lấy email người phê duyệt

                                        if (notifiSetting.IsApproved)
                                        {
                                            //Thêm mail cá nhân và mail công ty của người phê duyệt vào list gửi mail
                                            ListMailNguoiPheDuyet.ForEach(item =>
                                            {
                                                listEmailSendTo.Add(item.Trim());
                                            });
                                        }

                                        #endregion

                                        #region Lấy email người tạo

                                        if (notifiSetting.IsCreated)
                                        {
                                            var Email = LayEmaiNguoiTao(listAllUser, listAllContact, _request.CreatedById);
                                            listEmailSendTo.Add(Email);
                                        }

                                        #endregion

                                        #region Lấy email người đề xuất

                                        if (notifiSetting.IsPersonIncharge)
                                        {

                                        }

                                        #endregion

                                        #region Lấy email của danh sách người đặc biệt

                                        var listEmailSpecial = LayEmaiNguoiDacBiet(listAllUser, listAllContact, context.NotifiSpecial.ToList(), notifiSetting);
                                        listEmailSpecial.ForEach(email =>
                                        {
                                            if (!String.IsNullOrEmpty(email))
                                            {
                                                listEmailSendTo.Add(email.Trim());
                                            }
                                        });

                                        #endregion

                                        listEmailSendTo = listEmailSendTo.Distinct().ToList();

                                        #endregion

                                        #region Kiểm tra xem Gửi ngay hay Đặt lịch gửi 

                                        //Gửi ngay
                                        var subject = ReplaceTokenForContent(context, typeModel, _request,
                                            notifiSetting.EmailTitle, configEntity, note, actionCode);
                                        var content = ReplaceTokenForContent(context, typeModel, _request,
                                            notifiSetting.EmailContent, configEntity, note, actionCode);

                                        #region Build nội dung thay đổi

                                        //string contentModelChange = "";

                                        ////Nếu phải kiểm tra thay đổi của model
                                        //if (checkChange)
                                        //{
                                        //    contentModelChange = ContentModelChange(oldModel, newModel, typeModel);
                                        //}

                                        #endregion

                                        Emailer.SendEmail(context, listEmailSendTo, new List<string>(), new List<string>(), subject, content);

                                        //Đặt lịch gửi             
                                        if (notifiSetting.ObjectBackHourInternal != null && listEmailSendTo.Count > 0)
                                        {

                                        }

                                        #endregion
                                    }
                                }
                            }

                            #endregion
                        }
                    }
                    // Chi tiết đề xuất tạm ứng
                    else if (typeModel == TypeModel.DeNghiTamUng)
                    {
                        var _request = newModel as DeNghiTamHoanUng;

                        if (_request != null)
                        {
                            #region Kiểm tra xem cấu hình có điều kiện không?

                            var listNotifiCondition = context.NotifiSettingCondition
                                .Where(x => x.NotifiSettingId == notifiSetting.NotifiSettingId).ToList();

                            #region List reference để kiểm tra điều kiện và thông tin dùng để gửi email

                            var listAllEmployee = context.Employee.Where(x => x.Active == true).ToList();
                            var listAllContact = context.Contact.Where(x => x.Active == true && x.ObjectType == "EMP").ToList();

                            #endregion

                            //Nếu có điều kiện
                            if (listNotifiCondition.Count > 0)
                            {
                                //Do something...
                            }
                            //Nếu không có điều kiện
                            else
                            {
                                //Nếu gửi nội bộ
                                if (notifiSetting.SendInternal)
                                {
                                    //Nếu gửi bằng email
                                    if (notifiSetting.IsEmail)
                                    {
                                        #region Lấy danh sách email cần gửi thông báo

                                        var listEmailSendTo = new List<string>();
                                        var listAllUser = context.User.ToList();

                                        #region Lấy email người phê duyệt

                                        if (notifiSetting.IsApproved)
                                        {
                                            //Thêm mail cá nhân và mail công ty của người phê duyệt vào list gửi mail
                                            ListMailNguoiPheDuyet.ForEach(item =>
                                            {
                                                listEmailSendTo.Add(item.Trim());
                                            });
                                        }

                                        #endregion

                                        #region Lấy email người tạo

                                        if (notifiSetting.IsCreated)
                                        {
                                            var Email = LayEmaiNguoiTao(listAllUser, listAllContact, _request.CreatedById);
                                            listEmailSendTo.Add(Email);
                                        }

                                        #endregion

                                        #region Lấy email người đề xuất

                                        if (notifiSetting.IsPersonIncharge)
                                        {

                                        }

                                        #endregion


                                        #region Lấy email người tham gia (người thụ hưởng)

                                        if (notifiSetting.IsParticipant)
                                        {
                                            var contact = listAllContact.FirstOrDefault(x => x.ObjectId == _request.NguoiThuHuongId && x.ObjectType == "EMP");
                                            //Nếu không có mail cty thì gửi về mail cá nhân
                                            if (!String.IsNullOrEmpty(contact?.WorkEmail))
                                            {
                                                listEmailSendTo.Add(contact.WorkEmail.Trim());
                                            }
                                            else if (!String.IsNullOrEmpty(contact?.Email))
                                            {
                                                listEmailSendTo.Add(contact.Email.Trim());
                                            }
                                        }

                                        #endregion

                                        #region Lấy email của danh sách người đặc biệt

                                        var listEmailSpecial = LayEmaiNguoiDacBiet(listAllUser, listAllContact, context.NotifiSpecial.ToList(), notifiSetting);
                                        listEmailSpecial.ForEach(email =>
                                        {
                                            if (!String.IsNullOrEmpty(email))
                                            {
                                                listEmailSendTo.Add(email.Trim());
                                            }
                                        });

                                        #endregion

                                        listEmailSendTo = listEmailSendTo.Distinct().ToList();

                                        #endregion

                                        #region Kiểm tra xem Gửi ngay hay Đặt lịch gửi 

                                        //Gửi ngay
                                        var subject = ReplaceTokenForContent(context, typeModel, _request,
                                            notifiSetting.EmailTitle, configEntity, note, actionCode);
                                        var content = ReplaceTokenForContent(context, typeModel, _request,
                                            notifiSetting.EmailContent, configEntity, note, actionCode);

                                        #region Build nội dung thay đổi

                                        //string contentModelChange = "";

                                        ////Nếu phải kiểm tra thay đổi của model
                                        //if (checkChange)
                                        //{
                                        //    contentModelChange = ContentModelChange(oldModel, newModel, typeModel);
                                        //}

                                        #endregion

                                        Emailer.SendEmail(context, listEmailSendTo, new List<string>(), new List<string>(), subject, content);

                                        //Đặt lịch gửi             
                                        if (notifiSetting.ObjectBackHourInternal != null && listEmailSendTo.Count > 0)
                                        {

                                        }

                                        #endregion
                                    }
                                }
                            }

                            #endregion
                        }
                    }
                    // Chi tiết đề xuất hoàn ứng
                    else if (typeModel == TypeModel.DeNghiHoanUng)
                    {
                        var _request = newModel as DeNghiTamHoanUng;

                        if (_request != null)
                        {
                            #region Kiểm tra xem cấu hình có điều kiện không?

                            var listNotifiCondition = context.NotifiSettingCondition
                                .Where(x => x.NotifiSettingId == notifiSetting.NotifiSettingId).ToList();

                            #region List reference để kiểm tra điều kiện và thông tin dùng để gửi email

                            var listAllEmployee = context.Employee.Where(x => x.Active == true).ToList();
                            var listAllContact = context.Contact.Where(x => x.Active == true && x.ObjectType == "EMP").ToList();

                            #endregion

                            //Nếu có điều kiện
                            if (listNotifiCondition.Count > 0)
                            {
                                //Do something...
                            }
                            //Nếu không có điều kiện
                            else
                            {
                                //Nếu gửi nội bộ
                                if (notifiSetting.SendInternal)
                                {
                                    //Nếu gửi bằng email
                                    if (notifiSetting.IsEmail)
                                    {
                                        #region Lấy danh sách email cần gửi thông báo

                                        var listEmailSendTo = new List<string>();
                                        var listAllUser = context.User.ToList();

                                        #region Lấy email người phê duyệt

                                        if (notifiSetting.IsApproved)
                                        {
                                            //Thêm mail cá nhân và mail công ty của người phê duyệt vào list gửi mail
                                            ListMailNguoiPheDuyet.ForEach(item =>
                                            {
                                                listEmailSendTo.Add(item.Trim());
                                            });
                                        }

                                        #endregion

                                        #region Lấy email người tạo

                                        if (notifiSetting.IsCreated)
                                        {
                                            var Email = LayEmaiNguoiTao(listAllUser, listAllContact, _request.CreatedById);
                                            listEmailSendTo.Add(Email);
                                        }

                                        #endregion

                                        #region Lấy email người đề xuất

                                        if (notifiSetting.IsPersonIncharge)
                                        {

                                        }

                                        #endregion

                                        #region Lấy email người tham gia (người thụ hưởng)

                                        if (notifiSetting.IsParticipant)
                                        {
                                            var contact = listAllContact.FirstOrDefault(x => x.ObjectId == _request.NguoiThuHuongId && x.ObjectType == "EMP");
                                            //Nếu không có mail cty thì gửi về mail cá nhân
                                            if (!String.IsNullOrEmpty(contact?.WorkEmail))
                                            {
                                                listEmailSendTo.Add(contact.WorkEmail.Trim());
                                            }
                                            else if (!String.IsNullOrEmpty(contact?.Email))
                                            {
                                                listEmailSendTo.Add(contact.Email.Trim());
                                            }
                                        }

                                        #endregion

                                        #region Lấy email của danh sách người đặc biệt

                                        var listEmailSpecial = LayEmaiNguoiDacBiet(listAllUser, listAllContact, context.NotifiSpecial.ToList(), notifiSetting);
                                        listEmailSpecial.ForEach(email =>
                                        {
                                            if (!String.IsNullOrEmpty(email))
                                            {
                                                listEmailSendTo.Add(email.Trim());
                                            }
                                        });

                                        #endregion

                                        listEmailSendTo = listEmailSendTo.Distinct().ToList();

                                        #endregion

                                        #region Kiểm tra xem Gửi ngay hay Đặt lịch gửi 

                                        //Gửi ngay
                                        var subject = ReplaceTokenForContent(context, typeModel, _request,
                                            notifiSetting.EmailTitle, configEntity, note, actionCode);
                                        var content = ReplaceTokenForContent(context, typeModel, _request,
                                            notifiSetting.EmailContent, configEntity, note, actionCode);

                                        #region Build nội dung thay đổi

                                        //string contentModelChange = "";

                                        ////Nếu phải kiểm tra thay đổi của model
                                        //if (checkChange)
                                        //{
                                        //    contentModelChange = ContentModelChange(oldModel, newModel, typeModel);
                                        //}

                                        #endregion

                                        Emailer.SendEmail(context, listEmailSendTo, new List<string>(), new List<string>(), subject, content);

                                        //Đặt lịch gửi             
                                        if (notifiSetting.ObjectBackHourInternal != null && listEmailSendTo.Count > 0)
                                        {

                                        }

                                        #endregion
                                    }
                                }
                            }

                            #endregion
                        }
                    }
                    // Chi tiết yêu cầu cấp phát
                    else if (typeModel == TypeModel.DeXuatCapPhatTs)
                    {
                        var _request = newModel as YeuCauCapPhatTaiSan;

                        if (_request != null)
                        {
                            #region Kiểm tra xem cấu hình có điều kiện không?

                            var listNotifiCondition = context.NotifiSettingCondition
                                .Where(x => x.NotifiSettingId == notifiSetting.NotifiSettingId).ToList();

                            #region List reference để kiểm tra điều kiện và thông tin dùng để gửi email

                            var listAllEmployee = context.Employee.Where(x => x.Active == true).ToList();
                            var listAllContact = context.Contact.Where(x => x.Active == true && x.ObjectType == "EMP").ToList();

                            #endregion

                            //Nếu có điều kiện
                            if (listNotifiCondition.Count > 0)
                            {
                                //Do something...
                            }
                            //Nếu không có điều kiện
                            else
                            {
                                //Nếu gửi nội bộ
                                if (notifiSetting.SendInternal)
                                {
                                    //Nếu gửi bằng email
                                    if (notifiSetting.IsEmail)
                                    {
                                        #region Lấy danh sách email cần gửi thông báo

                                        var listEmailSendTo = new List<string>();
                                        var listAllUser = context.User.ToList();

                                        #region Lấy email người phê duyệt

                                        if (notifiSetting.IsApproved)
                                        {
                                            //Thêm mail cá nhân và mail công ty của người phê duyệt vào list gửi mail
                                            ListMailNguoiPheDuyet.ForEach(item =>
                                            {
                                                listEmailSendTo.Add(item.Trim());
                                            });
                                        }

                                        #endregion

                                        #region Lấy email người tạo

                                        if (notifiSetting.IsCreated)
                                        {
                                            var Email = LayEmaiNguoiTao(listAllUser, listAllContact, _request.CreatedById);
                                            listEmailSendTo.Add(Email);
                                        }

                                        #endregion

                                        #region Lấy email người đề xuất

                                        if (notifiSetting.IsPersonIncharge)
                                        {

                                        }

                                        #endregion

                                        #region Lấy email của danh sách người đặc biệt

                                        var listEmailSpecial = LayEmaiNguoiDacBiet(listAllUser, listAllContact, context.NotifiSpecial.ToList(), notifiSetting);
                                        listEmailSpecial.ForEach(email =>
                                        {
                                            if (!String.IsNullOrEmpty(email))
                                            {
                                                listEmailSendTo.Add(email.Trim());
                                            }
                                        });

                                        #endregion

                                        listEmailSendTo = listEmailSendTo.Distinct().ToList();

                                        #endregion

                                        #region Kiểm tra xem Gửi ngay hay Đặt lịch gửi 

                                        //Gửi ngay
                                        var subject = ReplaceTokenForContent(context, typeModel, _request,
                                            notifiSetting.EmailTitle, configEntity, note, actionCode);
                                        var content = ReplaceTokenForContent(context, typeModel, _request,
                                            notifiSetting.EmailContent, configEntity, note, actionCode);

                                        #region Build nội dung thay đổi

                                        //string contentModelChange = "";

                                        ////Nếu phải kiểm tra thay đổi của model
                                        //if (checkChange)
                                        //{
                                        //    contentModelChange = ContentModelChange(oldModel, newModel, typeModel);
                                        //}

                                        #endregion

                                        Emailer.SendEmail(context, listEmailSendTo, new List<string>(), new List<string>(), subject, content);

                                        //Đặt lịch gửi             
                                        if (notifiSetting.ObjectBackHourInternal != null && listEmailSendTo.Count > 0)
                                        {

                                        }

                                        #endregion
                                    }
                                }
                            }

                            #endregion
                        }
                    }
                    // Khi đẩy thông tin ứng viên từ CMS qua
                    else if (typeModel == TypeModel.CandidateCMS)
                    {
                        var _employee = newModel as Employee;
                        var _candidate = oldModel as Candidate;
                        if (_employee != null)
                        {
                            #region Kiểm tra xem cấu hình có điều kiện không?

                            var listNotifiCondition = context.NotifiSettingCondition
                                .Where(x => x.NotifiSettingId == notifiSetting.NotifiSettingId).ToList();

                            #region List reference để kiểm tra điều kiện và thông tin dùng để gửi email

                            var listAllEmployee = context.Employee.Where(x => x.Active == true).ToList();
                            var listAllContact = context.Contact.Where(x => x.Active == true && x.ObjectType == "EMP").ToList();

                            #endregion

                            //Nếu có điều kiện
                            if (listNotifiCondition.Count > 0)
                            {
                                //Do something...
                            }
                            //Nếu không có điều kiện
                            else
                            {
                                //Nếu gửi nội bộ
                                if (notifiSetting.SendInternal)
                                {
                                    //Nếu gửi bằng email
                                    if (notifiSetting.IsEmail)
                                    {
                                        #region Lấy danh sách email cần gửi thông báo

                                        var listEmailSendTo = new List<string>();
                                        var listAllUser = context.User.ToList();

                                        #region lấy email của nhân viên

                                        var empEmail = context.Contact.FirstOrDefault(x => x.ObjectId == _employee.EmployeeId && x.ObjectType == "EMP")?.Email;

                                        if (!string.IsNullOrEmpty(empEmail))
                                        {
                                            listEmailSendTo.Add(empEmail);
                                        }

                                        #endregion

                                        #region Lấy email của danh sách người đặc biệt

                                        var listEmailSpecial = LayEmaiNguoiDacBiet(listAllUser, listAllContact, context.NotifiSpecial.ToList(), notifiSetting);
                                        listEmailSpecial.ForEach(email =>
                                        {
                                            if (!String.IsNullOrEmpty(email))
                                            {
                                                listEmailSendTo.Add(email.Trim());
                                            }
                                        });

                                        #endregion

                                        listEmailSendTo = listEmailSendTo.Distinct().ToList();

                                        #endregion

                                        #region Kiểm tra xem Gửi ngay hay Đặt lịch gửi 

                                        //Gửi ngay
                                        var subject = ReplaceTokenForContent(context, typeModel, _candidate,
                                            notifiSetting.EmailTitle, configEntity, note);
                                        var content = ReplaceTokenForContent(context, typeModel, _candidate,
                                            notifiSetting.EmailContent, configEntity, note);

                                        Emailer.SendEmail(context, listEmailSendTo, new List<string>(), new List<string>(), subject, content);

                                        #endregion
                                    }
                                }
                            }

                            #endregion
                        }
                    }
                }
            }
        }

        private static string ContentModelChange(TNTN8Context context, object oldModel, object newModel, string typeModel)
        {
            string result = "";

            if (typeModel == TypeModel.CustomerOrder)
            {
                var _oldModel = oldModel as CustomerOrder;
                var _newModel = newModel as CustomerOrder;

                #region Kiểm tra thay đổi trên object

                CompareLogic compareLogic = new CompareLogic();
                var listIgnorField = new List<string> { "OrderId", "CreatedDate", "CreatedById" };
                compareLogic.Config.MembersToIgnore = listIgnorField;
                ComparisonResult compare = compareLogic.Compare(_oldModel, _newModel);

                //Nếu có thay đổi
                if (compare.AreEqual)
                {

                }

                #endregion

                #region Kiểm tra thay đổi trên list object detail

                //var _newListDetail =
                //    context.CustomerOrderDetail.Where(co => co.OrderId == _newModel.OrderId).ToList();
                //var listIgnorFieldDetail = new List<string> { "OrderDetailId", "CreatedDate", "CreatedById" };
                //compareLogic.Config.MembersToIgnore = listIgnorFieldDetail;
                //ComparisonResult detailCompare = compareLogic.Compare(_oldListDetail, _newListDetail);

                #endregion
            }
            else if (typeModel == TypeModel.Quote)
            {
                var _oldModel = oldModel as Quote;
                var _newModel = newModel as Quote;

                #region Kiểm tra thay đổi trên object

                CompareLogic compareLogic = new CompareLogic();
                var listIgnorField = new List<string> { "QuoteId", "CreatedDate", "CreatedById" };
                compareLogic.Config.MembersToIgnore = listIgnorField;
                ComparisonResult compare = compareLogic.Compare(_oldModel, _newModel);

                //Nếu có thay đổi
                if (compare.AreEqual)
                {

                }

                #endregion

                #region Kiểm tra thay đổi trên list object detail

                //var _newListDetail =
                //    context.QuoteDetail.Where(co => co.QuoteId == _newModel.QuoteId).ToList();
                //var listIgnorFieldDetail = new List<string> { "QuoteDetailId", "CreatedDate", "CreatedById" };
                //compareLogic.Config.MembersToIgnore = listIgnorFieldDetail;
                //ComparisonResult detailCompare = compareLogic.Compare(_oldListDetail, _newListDetail);

                #endregion
            }

            return result;
        }

        //param_1 là giá trị hiện tại của object, param_2 là giá trị trong setting
        private static bool CheckStringCondition(string param_1, string param_2, int typeCondition)
        {
            bool result = true;

            switch (typeCondition)
            {
                case TypeCondition.EQUAL:
                    if (param_1 != param_2) result = false;
                    break;
                case TypeCondition.NOT_EQUAL:
                    if (param_1 == param_2) result = false;
                    break;
                case TypeCondition.CONTAINS:
                    if (!param_1.Contains(param_2)) result = false;
                    break;
                case TypeCondition.NOT_CONTAINS:
                    if (param_1.Contains(param_2)) result = false;
                    break;
                case TypeCondition.NULL:
                    if (!String.IsNullOrEmpty(param_1)) result = false;
                    break;
                case TypeCondition.NOT_NULL:
                    if (String.IsNullOrEmpty(param_1)) result = false;
                    break;
            }

            return result;
        }

        //param_1 là giá trị hiện tại của object, param_2 là giá trị trong setting
        private static bool CheckListStringCondition(List<string> list_param_1, string param_2, int typeCondition)
        {
            bool result = true;

            switch (typeCondition)
            {
                case TypeCondition.EQUAL:
                    if (!list_param_1.Contains(param_2)) result = false;
                    break;
                case TypeCondition.NOT_EQUAL:
                    if (list_param_1.Contains(param_2)) result = false;
                    break;
                case TypeCondition.CONTAINS:
                    var count = list_param_1.Count;
                    var temp = 0;
                    list_param_1.ForEach(item =>
                    {
                        if (!item.Contains(param_2)) temp++;
                    });

                    if (temp == count) result = false;
                    break;
                case TypeCondition.NOT_CONTAINS:
                    list_param_1.ForEach(item =>
                    {
                        if (item.Contains(param_2)) result = false;
                    });
                    break;
            }

            return result;
        }

        //param_1 là giá trị hiện tại của object, param_2 là giá trị trong setting
        private static bool CheckNumberCondition(decimal? param_1, decimal param_2, int typeCondition)
        {
            bool result = true;

            switch (typeCondition)
            {
                case TypeCondition.EQUAL:
                    if (param_1 != param_2) result = false;
                    break;
                case TypeCondition.NOT_EQUAL:
                    if (param_1 == param_2) result = false;
                    break;
                case TypeCondition.NULL:
                    if (param_1 != null) result = false;
                    break;
                case TypeCondition.NOT_NULL:
                    if (param_1 == null) result = false;
                    break;
            }

            return result;
        }

        private static string ReplaceTokenForContent(TNTN8Context context, string typeModel, object model,
            string emailContent, List<SystemParameter> configEntity, object note = null, string actionCode = null,
            string emailSendTo = null, Guid? interviewId = null)
        {
            var result = emailContent;

            #region Common Token

            const string Logo = "[LOGO]";
            const string CustomerName = "[CUSTOMER_NAME]";
            const string EmployeeCode = "[EMPLOYEE_CODE]";
            const string EmployeeName = "[EMPLOYEE_NAME]";
            const string Url_Login = "[URL]";
            const string CreatedDate = "[CREATED_DATE]";
            const string UpdatedDate = "[UPDATED_DATE]";
            const string CommentEmployeeName = "[COMM_EMP_NAME]";
            const string CommentEmployeeCode = "[COMM_EMP_CODE]";
            const string CommentCreatedDate = "[COMM_CREATED_DATE]";
            const string CommentContent = "[COMM_CONTENT]";
            const string Description = "[DESCRIPTION]";
            const string LyDoTuChoi = "[LY_DO_TU_CHOI]";
            const string MaXacThuc = "[CODE]";
            #endregion

            #region Token CustomerOrder

            const string OrderCode = "[ORDER_CODE]";

            #endregion

            #region Token Contract

            const string ContractCode = "[CONTRACT_CODE]";
            const string ContractName = "[CONTRACT_NAME]";
            const string ContractEffective = "[CONTRACT_EFFECTIVE]";
            const string ContractTime = "[CONTRACT_TIME]";
            const string ContractExpired = "[CONTRACT_EXPIRED]";
            const string ContractAmount = "[CONTRACT_AMOUNT]";
            const string ContractDescription = "[CONTRACT_DESCRIPTION]";
            #endregion

            #region Token Quote

            const string QuoteName = "[QUOTE_NAME]";
            const string QuoteCode = "[QUOTE_CODE]";

            #endregion

            #region Token LEAD

            const string LeadName = "[LEAD_NAME]";
            const string LeadCode = "[LEAD_CODE]";

            #endregion

            #region Token SALE_BIDDING

            const string SaleBiddingName = "[SALE_BIDDING_NAME]";
            const string SaleBiddingCode = "[SALE_BIDDING_CODE]";

            #endregion

            #region BILL_SALE

            const string BillCode = "[BILL_CODE]";
            const string BillName = "[INVOICE_SYMBOL]";

            #endregion

            #region PROCUREMENT_REQUEST

            const string requestCode = "[PROCUREMENT_CODE]";
            const string requestEmployee = "[REQUEST_EMPLOYEE]";

            #endregion

            #region POTENTIAL_CUSTOMER

            const string PotentialCustomerName = "[POTENTIAL_CUSTOMER_NAME]";

            #endregion

            #region CUSTOMER

            const string CustomerCode = "[CUSTOMER_CODE]";

            #endregion

            #region VENDOR_ORDER

            const string vendorOrderCode = "[ORDER_CODE]";
            const string vendorName = "[VENDOR_NAME]";

            #endregion

            #region PROJECT MODULE

            const string accCode = "[ACCOUNT_CODE]"; //Mã của nhân viên sẽ nhận email
            const string accName = "[ACCOUNT_NAME]"; //Tên nhân viên sẽ nhận email
            const string projectType = "[PROJECT_TYPE]"; //Loại dự án
            const string projectStartDate = "[PROJECT_START_DATE]"; //Ngày bắt đầu dự kiến
            const string projectEndDate = "[PROJECT_END_DATE]"; //Ngày kết thúc dự kiến
            const string projectDescription = "[PROJECT_DESCRIPTION]"; //Mô tả dự án
            const string projectStatus = "[PROJECT_STATUS]"; //Trạng thái dự án
            const string projectCode = "[PROJECT_CODE]";
            const string projectName = "[PROJECT_NAME]";
            const string taskName = "[TASK_NAME]";
            const string taskCode = "[TASK_CODE]";
            const string taskDescription = "[TASK_DESCRIPTION]"; //Mô tả công việc
            const string taskPercentComplete = "[TASK_COMPLETED]"; // % hoàn thành công việc
            const string taskStatus = "[TASK_STATUS]"; //Trạng thái công việc
            const string scopeName = "[WORK_PACKAGE_NAME]";
            const string scopeCode = "[WORK_PACKAGE_CODE]";
            const string scopeDescription = "[WORK_PACKAGE_DESCIPTION]"; //Mô tả hạng mục dự án
            const string resourceName = "[RESOURCE_NAME]";
            const string resourceCode = "[RESOURCE_CODE]";

            #endregion

            #region Receipt Invoice

            const string totalAmountPay = "[TOTAL_AMOUNT_PAY]";
            const string companyName = "[COMPANY_NAME]";
            const string remainBalance = "[REMAIN_BALANCE]";
            const string amountPay = "[AMOUNT_PAY]";
            const string orderCode = "[ORDER_CODE]";
            const string orderDate = "[ORDER_DATE]";
            const string invoiceCode = "[INVOICE_CODE]";

            #endregion

            #region Employee

            const string Username = "[USER_NAME]";
            const string Password = "[USER_PASS]";
            const string PhongBan = "[PHONG_BAN]";
            const string ChucVu = "[CHUC_VU]";
            const string CompanyName = "[COMPANY_NAME]";
            const string ListNhanVien = "[LIST_NHAN_VIEN]";

            #endregion

            #region Đề xuất xin nghỉ

            const string RequestCode = "[REQUEST_CODE]";
            const string OfferEmployeeCode = "[OFFER_EMP_CODE]";
            const string OfferEmployeeName = "[OFFER_EMP_NAME]";
            const string TypeRequestName = "[TYPE_REQUEST_NAME]";
            const string DurationTime = "[DURATION_TIME]";
            const string Detail = "[DETAIL]";
            const string Dxxn_LyDoTuChoi = "[REJECT_REASON]";

            #endregion

            #region Đề xuất tăng lương

            //gửi email khi tạo đề xuất tăng lương
            const string DeXuatTangLuong = "[DXTL_NAME]";

            #endregion

            #region Đề xuất chức vụ

            //gửi email khi tạo đề xuất tăng lương
            const string DeXuatChucVu = "[DXCV_NAME]";

            #endregion

            #region Đề xuất kế hoạch OT và  Đề xuất đăng ký OT

            //gửi email khi tạo đề xuất tăng lương
            const string KeHoachOt = "[DXKHOT_NAME]";

            #endregion

            #region ỨNG VIÊN

            const string TenUngVien = "[CANDIDATE_NAME]";
            const string NgaySinh = "[CANDIDATE_DATEOFBIRTH]";
            const string CandidateEmail = "[CANDIDATE_EMAIL]";
            const string CandidatePhone = "[CANDIDATE_PHONE]";
            const string ViTriUngTuyen = "[VACANCIES_NAME]";
            const string ThoiGian = "[TIME]";
            const string HinhThucPV = "[INTERVIEW_TYPE]";
            const string DiaChi_Link = "[ADDRESS_LINK]";

            #endregion

            #region Tài sản đến thời hạn bảo trì, bảo dưỡng

            const string DanhSachTaiSan = "[DS_TAISAN]";

            #endregion

            #region Kỳ lương

            const string TenKyLuong = "[TEN_KY_LUONG]";
            const string KyLuong_NgayBatDau = "[NGAY_BAT_DAU]";
            const string KyLuong_NgayKetThuc = "[NGAY_KET_THUC]";
            const string KyLuong_LyDoTuChoi = "[LY_DO_TU_CHOI]";

            #endregion

            #region Phiếu lương

            const string WorkEmail = "[WORK_EMAIL]";
            const string SoNgayLamViec = "[SO_NGAY_LAM_VIEC]";

            const string CauHinhGiamTruCaNhan = "[CH_GTCN]";
            const string CauHinhGiamTruNguoiPhuThuoc = "[CH_GTNPT]";
            const string PhanTramBaoHiemCty = "[PTBH_CTY]";
            const string PhanTramBaoHiemNld = "[PTBH_NLD]";
            const string PhanTramKinhPhiCongDoanCty = "[PT_KPCD_CTY]";
            const string ThangBatDauKyLuong = "[THANG_BAT_DAU_KY_LUONG]";
            const string NamBatDauKyLuong = "[NAM_BAT_DAU_KY_LUONG]";

            const string ThangTruoc = "[THANG_TRUOC]";
            const string ThangTruocTiengAnh = "[THANG_TRUOC_TA]";
            const string NamTheoThangTruoc = "[NAM_THEO_THANG_TRUOC]";
            const string ThangKetThucKyLuong = "[THANG_KET_THUC_KY_LUONG]";
            const string NamKetThucKyLuong = "[NAM_KET_THUC_KY_LUONG]";

            const string LuongCoBan = "[LUONG_CO_BAN]";
            const string LuongCoBanSau = "[LUONG_CO_BAN_SAU]";
            const string MucDieuChinh = "[MUC_DIEU_CHINH]";
            const string NgayLamViecThucTe = "[NGAY_LV_TT]";
            const string NgayNghiPhep = "[NGAY_NGHI_PHEP]";
            const string NgayNghiLe = "[NGAY_NGHI_LE]";
            const string NgayNghiKhongLuong = "[NGAY_NGHI_KO_LUONG]";
            const string NgayDmvs = "[NGAY_DMVS]";
            const string NgayKhongHuongChuyenCan = "[NGAY_KO_HUONG_CC]";
            const string DuocHuongTroCapKpi = "[DUOC_HUONG_TC_KPI]";
            const string SoLuongDkGiamTruGiaCanh = "[SL_GTGC]";
            const string GiamTruGiaCanh = "[GTGC]";
            const string LuongTheoNgayHocViec = "[LUONG_NHV]";
            const string TroCapDiLai = "[TC_DI_LAI]";
            const string TroCapDienThoai = "[TC_DT]";
            const string TroCapAnTrua = "[TC_AT]";
            const string TroCapNhaO = "[TC_NHA_O]";
            const string TroCapChuyenCan = "[TC_CHUYEN_CAN]";
            const string ThuongKpi = "[THUONG_KPI]";
            const string ThuongCuoiNam = "[THUONG_CUOI_NAM]";
            const string TroCapTrachNhiem = "[TC_TRACH_NHIEM]";
            const string TroCapHocViec = "[TC_HOC_VIEC]";
            const string OtTinhThue = "[OT_TINH_THUE]";
            const string OtKhongTinhThue = "[OT_KO_TINH_THUE]";
            const string LuongThang13 = "[LUONG_THANG_13]";
            const string QuaBocTham = "[QUA_BOC_THAM]";
            const string TongThueTncn = "[TONG_THUE_TNCN]";
            const string BaoHiem = "[BAO_HIEM]";
            const string ThucNhan = "[THUC_NHAN]";
            const string CtyTraBh = "[CTY_TRA_BH]";
            const string KinhPhiCongDoan = "[KPCD]";
            const string TongChiPhiNhanVien = "[TCPNV]";

            #endregion

            #region Đề xuất công tác

            const string TenDeXuatCongTac = "[DXCT_NAMECODE]";

            #endregion

            #region Đề xuất tạm ứng

            const string TenDeXuatTamUng = "[DXTU_NAMECODE]";

            #endregion

            #region Đề xuất hoàn ứng

            const string TenDeXuatHoanUng = "[DXHU_NAMECODE]";

            #endregion

            #region Đề xuất cấp phát tài sản

            const string TenDeXuatCapPhatTs = "[DXCPTS_NAMECODE]";

            #endregion

            //Tạo đơn hàng
            if (typeModel == TypeModel.CustomerOrder)
            {
                var _model = model as CustomerOrder;

                if (_model != null)
                {
                    if (result.Contains(Logo))
                    {
                        var logo = configEntity.FirstOrDefault(w => w.SystemKey == "Logo").SystemValueString;

                        if (!String.IsNullOrEmpty(logo))
                        {
                            var temp_logo = "<img src=\"" + logo + "\" class=\"e - rte - image e - imginline\" alt=\"Logo TNM.png\" width=\"auto\" height=\"auto\" style=\"min - width: 0px; max - width: 750px; min - height: 0px; \">";
                            result = result.Replace(Logo, temp_logo);
                        }
                        else
                        {
                            result = result.Replace(Logo, "");
                        }
                    }

                    if (result.Contains(OrderCode) && _model.OrderCode != null)
                    {
                        result = result.Replace(OrderCode, _model.OrderCode);
                    }

                    if (result.Contains(CustomerName))
                    {
                        var _customerName = context.Customer.FirstOrDefault(x => x.CustomerId == _model.CustomerId)
                            ?.CustomerName;

                        if (!String.IsNullOrEmpty(_customerName))
                        {
                            result = result.Replace(CustomerName, _customerName);
                        }
                        else
                        {
                            result = result.Replace(CustomerName, "");
                        }
                    }

                    if (result.Contains(EmployeeCode))
                    {
                        var employeeId = context.User.FirstOrDefault(x => x.UserId == _model.CreatedById)?.EmployeeId;
                        var employeeCode = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeId)?.EmployeeCode;

                        if (!String.IsNullOrEmpty(employeeCode))
                        {
                            result = result.Replace(EmployeeCode, employeeCode);
                        }
                        else
                        {
                            result = result.Replace(EmployeeCode, "");
                        }
                    }

                    if (result.Contains(EmployeeName))
                    {
                        var employeeId = context.User.FirstOrDefault(x => x.UserId == _model.CreatedById)?.EmployeeId;
                        var employeeName = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeId)?.EmployeeName;

                        if (!String.IsNullOrEmpty(employeeName))
                        {
                            result = result.Replace(EmployeeName, employeeName);
                        }
                        else
                        {
                            result = result.Replace(EmployeeName, "");
                        }
                    }

                    if (result.Contains(CreatedDate))
                    {
                        result = result.Replace(CreatedDate, FormatDateToString(_model.CreatedDate));
                    }

                    if (result.Contains(UpdatedDate))
                    {
                        result = result.Replace(UpdatedDate, FormatDateToString(_model.UpdatedDate));
                    }

                    if (result.Contains(Url_Login))
                    {
                        var Domain = configEntity.FirstOrDefault(w => w.SystemKey == "Domain").SystemValueString;
                        var loginLink = Domain + @"/";

                        if (!String.IsNullOrEmpty(loginLink))
                        {
                            result = result.Replace(Url_Login, loginLink);
                        }
                    }
                }
            }
            //Chi tiết đơn hàng
            else if (typeModel == TypeModel.CustomerOrderDetail)
            {
                var _model = model as CustomerOrder;

                if (_model != null)
                {
                    if (result.Contains(Logo))
                    {
                        var logo = configEntity.FirstOrDefault(w => w.SystemKey == "Logo").SystemValueString;

                        if (!String.IsNullOrEmpty(logo))
                        {
                            var temp_logo = "<img src=\"" + logo + "\" class=\"e - rte - image e - imginline\" alt=\"Logo TNM.png\" width=\"auto\" height=\"auto\" style=\"min - width: 0px; max - width: 750px; min - height: 0px; \">";
                            result = result.Replace(Logo, temp_logo);
                        }
                        else
                        {
                            result = result.Replace(Logo, "");
                        }
                    }

                    if (result.Contains(OrderCode) && _model.OrderCode != null)
                    {
                        result = result.Replace(OrderCode, _model.OrderCode);
                    }

                    if (result.Contains(CustomerName))
                    {
                        var _customerName = context.Customer.FirstOrDefault(x => x.CustomerId == _model.CustomerId)
                            ?.CustomerName;

                        if (!String.IsNullOrEmpty(_customerName))
                        {
                            result = result.Replace(CustomerName, _customerName);
                        }
                        else
                        {
                            result = result.Replace(CustomerName, "");
                        }
                    }

                    if (result.Contains(EmployeeCode))
                    {
                        var employeeId = context.User.FirstOrDefault(x => x.UserId == _model.CreatedById)?.EmployeeId;
                        var employeeCode = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeId)?.EmployeeCode;

                        if (!String.IsNullOrEmpty(employeeCode))
                        {
                            result = result.Replace(EmployeeCode, employeeCode);
                        }
                        else
                        {
                            result = result.Replace(EmployeeCode, "");
                        }
                    }

                    if (result.Contains(EmployeeName))
                    {
                        var employeeId = context.User.FirstOrDefault(x => x.UserId == _model.CreatedById)?.EmployeeId;
                        var employeeName = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeId)?.EmployeeName;

                        if (!String.IsNullOrEmpty(employeeName))
                        {
                            result = result.Replace(EmployeeName, employeeName);
                        }
                        else
                        {
                            result = result.Replace(EmployeeName, "");
                        }
                    }

                    if (result.Contains(CreatedDate))
                    {
                        result = result.Replace(CreatedDate, FormatDateToString(_model.CreatedDate));
                    }

                    if (result.Contains(UpdatedDate))
                    {
                        result = result.Replace(UpdatedDate, FormatDateToString(_model.UpdatedDate));
                    }

                    if (result.Contains(Url_Login))
                    {
                        var Domain = configEntity.FirstOrDefault(w => w.SystemKey == "Domain").SystemValueString;
                        var loginLink = Domain + @"/";

                        if (!String.IsNullOrEmpty(loginLink))
                        {
                            result = result.Replace(Url_Login, loginLink);
                        }
                    }

                    if (note != null)
                    {
                        var newNote = note as Note;

                        if (result.Contains(CommentEmployeeName))
                        {
                            var commentUser = context.User.FirstOrDefault(x => x.UserId == newNote.CreatedById);
                            if (commentUser != null)
                            {
                                var commentEmployee =
                                    context.Employee.FirstOrDefault(x => x.EmployeeId == commentUser.EmployeeId);
                                var _commentEmployeeName = commentEmployee?.EmployeeName;

                                if (!String.IsNullOrEmpty(_commentEmployeeName))
                                {
                                    result = result.Replace(CommentEmployeeName, _commentEmployeeName);
                                }
                                else
                                {
                                    result = result.Replace(CommentEmployeeName, "");
                                }
                            }
                        }

                        if (result.Contains(CommentEmployeeCode))
                        {
                            var commentUser = context.User.FirstOrDefault(x => x.UserId == newNote.CreatedById);
                            if (commentUser != null)
                            {
                                var commentEmployee =
                                    context.Employee.FirstOrDefault(x => x.EmployeeId == commentUser.EmployeeId);
                                var _commentEmployeeCode = commentEmployee?.EmployeeCode;

                                if (!String.IsNullOrEmpty(_commentEmployeeCode))
                                {
                                    result = result.Replace(CommentEmployeeCode, _commentEmployeeCode);
                                }
                                else
                                {
                                    result = result.Replace(CommentEmployeeCode, "");
                                }
                            }
                        }

                        if (result.Contains(CommentCreatedDate))
                        {
                            result = result.Replace(CommentCreatedDate, FormatDateToString(newNote.CreatedDate));
                        }

                        if (result.Contains(CommentContent))
                        {
                            var _commentContent = newNote.Description?.Trim();
                            if (!String.IsNullOrEmpty(_commentContent))
                            {
                                result = result.Replace(CommentContent, _commentContent);
                            }
                            else
                            {
                                result = result.Replace(CommentContent, "");
                            }
                        }
                    }
                }
            }
            //Tạo mới báo giá
            else if (typeModel == TypeModel.Quote)
            {
                var _model = model as Quote;

                if (_model != null)
                {
                    if (result.Contains(Logo))
                    {
                        var logo = configEntity.FirstOrDefault(w => w.SystemKey == "Logo").SystemValueString;

                        if (!String.IsNullOrEmpty(logo))
                        {
                            var temp_logo = "<img src=\"" + logo + "\" class=\"e - rte - image e - imginline\" alt=\"Logo TNM.png\" width=\"auto\" height=\"auto\" style=\"min - width: 0px; max - width: 750px; min - height: 0px; \">";
                            result = result.Replace(Logo, temp_logo);
                        }
                        else
                        {
                            result = result.Replace(Logo, "");
                        }
                    }

                    if (result.Contains(QuoteName) && _model.QuoteName != null)
                    {
                        result = result.Replace(QuoteName, _model.QuoteName.Trim());
                    }

                    if (result.Contains(QuoteCode) && _model.QuoteCode != null)
                    {
                        result = result.Replace(QuoteCode, _model.QuoteCode.Trim());
                    }

                    if (result.Contains(EmployeeCode))
                    {
                        var employeeId = context.User.FirstOrDefault(x => x.UserId == _model.CreatedById)?.EmployeeId;
                        var employeeCode = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeId)?.EmployeeCode;

                        if (!String.IsNullOrEmpty(employeeCode))
                        {
                            result = result.Replace(EmployeeCode, employeeCode);
                        }
                        else
                        {
                            result = result.Replace(EmployeeCode, "");
                        }
                    }

                    if (result.Contains(EmployeeName))
                    {
                        var employeeId = context.User.FirstOrDefault(x => x.UserId == _model.CreatedById)?.EmployeeId;
                        var employeeName = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeId)?.EmployeeName;

                        if (!String.IsNullOrEmpty(employeeName))
                        {
                            result = result.Replace(EmployeeName, employeeName);
                        }
                        else
                        {
                            result = result.Replace(EmployeeName, "");
                        }
                    }

                    if (result.Contains(CreatedDate))
                    {
                        result = result.Replace(CreatedDate, FormatDateToString(_model.CreatedDate));
                    }

                    if (result.Contains(Url_Login))
                    {
                        var Domain = configEntity.FirstOrDefault(w => w.SystemKey == "Domain").SystemValueString;
                        var loginLink = Domain + @"/";

                        if (!String.IsNullOrEmpty(loginLink))
                        {
                            result = result.Replace(Url_Login, loginLink);
                        }
                    }

                    if (note != null)
                    {
                        var newNote = note as Note;

                        if (result.Contains(CommentEmployeeName))
                        {
                            var commentUser = context.User.FirstOrDefault(x => x.UserId == newNote.CreatedById);
                            if (commentUser != null)
                            {
                                var commentEmployee =
                                    context.Employee.FirstOrDefault(x => x.EmployeeId == commentUser.EmployeeId);
                                var _commentEmployeeName = commentEmployee?.EmployeeName;

                                if (!String.IsNullOrEmpty(_commentEmployeeName))
                                {
                                    result = result.Replace(CommentEmployeeName, _commentEmployeeName);
                                }
                                else
                                {
                                    result = result.Replace(CommentEmployeeName, "");
                                }
                            }
                        }

                        if (result.Contains(CommentEmployeeCode))
                        {
                            var commentUser = context.User.FirstOrDefault(x => x.UserId == newNote.CreatedById);
                            if (commentUser != null)
                            {
                                var commentEmployee =
                                    context.Employee.FirstOrDefault(x => x.EmployeeId == commentUser.EmployeeId);
                                var _commentEmployeeCode = commentEmployee?.EmployeeCode;

                                if (!String.IsNullOrEmpty(_commentEmployeeCode))
                                {
                                    result = result.Replace(CommentEmployeeCode, _commentEmployeeCode);
                                }
                                else
                                {
                                    result = result.Replace(CommentEmployeeCode, "");
                                }
                            }
                        }

                        if (result.Contains(CommentCreatedDate))
                        {
                            result = result.Replace(CommentCreatedDate, FormatDateToString(newNote.CreatedDate));
                        }

                        if (result.Contains(CommentContent))
                        {
                            var _commentContent = newNote.Description?.Trim();
                            if (!String.IsNullOrEmpty(_commentContent))
                            {
                                result = result.Replace(CommentContent, _commentContent);
                            }
                            else
                            {
                                result = result.Replace(CommentContent, "");
                            }
                        }
                    }
                }
            }
            //Chi tiết báo giá
            else if (typeModel == TypeModel.QuoteDetail)
            {
                var _model = model as Quote;

                if (_model != null)
                {
                    if (result.Contains(Logo))
                    {
                        var logo = configEntity.FirstOrDefault(w => w.SystemKey == "Logo").SystemValueString;

                        if (!String.IsNullOrEmpty(logo))
                        {
                            var temp_logo = "<img src=\"" + logo + "\" class=\"e - rte - image e - imginline\" alt=\"Logo TNM.png\" width=\"auto\" height=\"auto\" style=\"min - width: 0px; max - width: 750px; min - height: 0px; \">";
                            result = result.Replace(Logo, temp_logo);
                        }
                        else
                        {
                            result = result.Replace(Logo, "");
                        }
                    }

                    if (result.Contains(QuoteName) && _model.QuoteName != null)
                    {
                        result = result.Replace(QuoteName, _model.QuoteName.Trim());
                    }

                    if (result.Contains(QuoteCode) && _model.QuoteCode != null)
                    {
                        result = result.Replace(QuoteCode, _model.QuoteCode.Trim());
                    }

                    if (result.Contains(EmployeeCode))
                    {
                        var employeeId = context.User.FirstOrDefault(x => x.UserId == _model.UpdatedById)?.EmployeeId;
                        var employeeCode = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeId)?.EmployeeCode;

                        if (!String.IsNullOrEmpty(employeeCode))
                        {
                            result = result.Replace(EmployeeCode, employeeCode);
                        }
                        else
                        {
                            result = result.Replace(EmployeeCode, "");
                        }
                    }

                    if (result.Contains(EmployeeName))
                    {
                        var employeeId = context.User.FirstOrDefault(x => x.UserId == _model.UpdatedById)?.EmployeeId;
                        var employeeName = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeId)?.EmployeeName;

                        if (!String.IsNullOrEmpty(employeeName))
                        {
                            result = result.Replace(EmployeeName, employeeName);
                        }
                        else
                        {
                            result = result.Replace(EmployeeName, "");
                        }
                    }

                    if (result.Contains(CreatedDate))
                    {
                        result = result.Replace(CreatedDate, FormatDateToString(_model.CreatedDate));
                    }

                    if (result.Contains(UpdatedDate))
                    {
                        result = result.Replace(UpdatedDate, FormatDateToString(_model.UpdatedDate));
                    }

                    if (result.Contains(Url_Login))
                    {
                        var Domain = configEntity.FirstOrDefault(w => w.SystemKey == "Domain").SystemValueString;
                        var loginLink = Domain + @"/";

                        if (!String.IsNullOrEmpty(loginLink))
                        {
                            result = result.Replace(Url_Login, loginLink);
                        }
                    }

                    if (note != null)
                    {
                        var newNote = note as Note;

                        if (result.Contains(CommentEmployeeName))
                        {
                            var commentUser = context.User.FirstOrDefault(x => x.UserId == newNote.CreatedById);
                            if (commentUser != null)
                            {
                                var commentEmployee =
                                    context.Employee.FirstOrDefault(x => x.EmployeeId == commentUser.EmployeeId);
                                var _commentEmployeeName = commentEmployee?.EmployeeName;

                                if (!String.IsNullOrEmpty(_commentEmployeeName))
                                {
                                    result = result.Replace(CommentEmployeeName, _commentEmployeeName);
                                }
                                else
                                {
                                    result = result.Replace(CommentEmployeeName, "");
                                }
                            }
                        }

                        if (result.Contains(CommentEmployeeCode))
                        {
                            var commentUser = context.User.FirstOrDefault(x => x.UserId == newNote.CreatedById);
                            if (commentUser != null)
                            {
                                var commentEmployee =
                                    context.Employee.FirstOrDefault(x => x.EmployeeId == commentUser.EmployeeId);
                                var _commentEmployeeCode = commentEmployee?.EmployeeCode;

                                if (!String.IsNullOrEmpty(_commentEmployeeCode))
                                {
                                    result = result.Replace(CommentEmployeeCode, _commentEmployeeCode);
                                }
                                else
                                {
                                    result = result.Replace(CommentEmployeeCode, "");
                                }
                            }
                        }

                        if (result.Contains(CommentCreatedDate))
                        {
                            result = result.Replace(CommentCreatedDate, FormatDateToString(newNote.CreatedDate));
                        }

                        if (result.Contains(CommentContent))
                        {
                            var _commentContent = newNote.Description?.Trim();
                            if (!String.IsNullOrEmpty(_commentContent))
                            {
                                result = result.Replace(CommentContent, _commentContent);
                            }
                            else
                            {
                                result = result.Replace(CommentContent, "");
                            }
                        }

                        if (result.Contains(Description))
                        {
                            var _description = newNote.Description?.Trim();
                            if (!String.IsNullOrEmpty(_description))
                            {
                                result = result.Replace(Description, _description);
                            }
                            else
                            {
                                result = result.Replace(Description, "");
                            }
                        }
                    }
                }
            }
            //Tạo mới cơ hội
            else if (typeModel == TypeModel.Lead)
            {
                var _model = model as Lead;

                if (_model != null)
                {
                    if (result.Contains(Logo))
                    {
                        var logo = configEntity.FirstOrDefault(w => w.SystemKey == "Logo").SystemValueString;

                        if (!String.IsNullOrEmpty(logo))
                        {
                            var temp_logo = "<img src=\"" + logo + "\" class=\"e - rte - image e - imginline\" alt=\"Logo TNM.png\" width=\"auto\" height=\"auto\" style=\"min - width: 0px; max - width: 750px; min - height: 0px; \">";
                            result = result.Replace(Logo, temp_logo);
                        }
                        else
                        {
                            result = result.Replace(Logo, "");
                        }
                    }

                    if (result.Contains(LeadName))
                    {
                        var _leadName = context.Contact.FirstOrDefault(c => c.ObjectId == _model.LeadId && c.ObjectType == "LEA")?.FirstName;
                        if (!String.IsNullOrEmpty(_leadName))
                        {
                            result = result.Replace(LeadName, _leadName.Trim());
                        }
                    }

                    if (result.Contains(LeadCode) && _model.LeadCode != null)
                    {
                        result = result.Replace(LeadCode, _model.LeadCode.Trim());
                    }

                    if (result.Contains(EmployeeCode))
                    {
                        var employeeId = context.User.FirstOrDefault(x => x.UserId.ToString() == _model.CreatedById)?.EmployeeId;
                        var employeeCode = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeId)?.EmployeeCode;

                        if (!String.IsNullOrEmpty(employeeCode))
                        {
                            result = result.Replace(EmployeeCode, employeeCode);
                        }
                        else
                        {
                            result = result.Replace(EmployeeCode, "");
                        }
                    }

                    if (result.Contains(EmployeeName))
                    {
                        var employeeId = context.User.FirstOrDefault(x => x.UserId.ToString() == _model.CreatedById)?.EmployeeId;
                        var employeeName = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeId)?.EmployeeName;

                        if (!String.IsNullOrEmpty(employeeName))
                        {
                            result = result.Replace(EmployeeName, employeeName);
                        }
                        else
                        {
                            result = result.Replace(EmployeeName, "");
                        }
                    }

                    if (result.Contains(CreatedDate))
                    {
                        result = result.Replace(CreatedDate, FormatDateToString(_model.CreatedDate));
                    }

                    if (result.Contains(Url_Login))
                    {
                        var Domain = configEntity.FirstOrDefault(w => w.SystemKey == "Domain").SystemValueString;
                        var loginLink = Domain + @"/";

                        if (!String.IsNullOrEmpty(loginLink))
                        {
                            result = result.Replace(Url_Login, loginLink);
                        }
                    }
                }
            }
            //Chi tiết cơ hội
            else if (typeModel == TypeModel.LeadDetail)
            {
                var _model = model as Lead;

                if (_model != null)
                {
                    if (result.Contains(Logo))
                    {
                        var logo = configEntity.FirstOrDefault(w => w.SystemKey == "Logo").SystemValueString;

                        if (!String.IsNullOrEmpty(logo))
                        {
                            var temp_logo = "<img src=\"" + logo + "\" class=\"e - rte - image e - imginline\" alt=\"Logo TNM.png\" width=\"auto\" height=\"auto\" style=\"min - width: 0px; max - width: 750px; min - height: 0px; \">";
                            result = result.Replace(Logo, temp_logo);
                        }
                        else
                        {
                            result = result.Replace(Logo, "");
                        }
                    }

                    if (result.Contains(LeadName))
                    {
                        var _leadName = context.Contact
                            .FirstOrDefault(c => c.ObjectId == _model.LeadId && c.ObjectType == "LEA").FirstName;
                        if (!String.IsNullOrEmpty(_leadName))
                        {
                            result = result.Replace(LeadName, _leadName.Trim());
                        }
                    }

                    if (result.Contains(LeadCode) && _model.LeadCode != null)
                    {
                        result = result.Replace(LeadCode, _model.LeadCode.Trim());
                    }

                    if (result.Contains(EmployeeCode))
                    {
                        var employeeId = context.User.FirstOrDefault(x => x.UserId.ToString() == _model.UpdatedById)?.EmployeeId;
                        var employeeCode = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeId)?.EmployeeCode;

                        if (!String.IsNullOrEmpty(employeeCode))
                        {
                            result = result.Replace(EmployeeCode, employeeCode);
                        }
                        else
                        {
                            result = result.Replace(EmployeeCode, "");
                        }
                    }

                    if (result.Contains(EmployeeName))
                    {
                        var employeeId = context.User.FirstOrDefault(x => x.UserId.ToString() == _model.UpdatedById)?.EmployeeId;
                        var employeeName = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeId)?.EmployeeName;

                        if (!String.IsNullOrEmpty(employeeName))
                        {
                            result = result.Replace(EmployeeName, employeeName);
                        }
                        else
                        {
                            result = result.Replace(EmployeeName, "");
                        }
                    }

                    if (result.Contains(CreatedDate))
                    {
                        result = result.Replace(CreatedDate, FormatDateToString(_model.CreatedDate));
                    }

                    if (result.Contains(UpdatedDate))
                    {
                        result = result.Replace(UpdatedDate, FormatDateToString(_model.UpdatedDate));
                    }

                    if (result.Contains(Url_Login))
                    {
                        var Domain = configEntity.FirstOrDefault(w => w.SystemKey == "Domain").SystemValueString;
                        var loginLink = Domain + @"/";

                        if (!String.IsNullOrEmpty(loginLink))
                        {
                            result = result.Replace(Url_Login, loginLink);
                        }
                    }

                    if (note != null)
                    {
                        var newNote = note as Note;

                        if (result.Contains(CommentEmployeeName))
                        {
                            var commentUser = context.User.FirstOrDefault(x => x.UserId == newNote.CreatedById);
                            if (commentUser != null)
                            {
                                var commentEmployee =
                                    context.Employee.FirstOrDefault(x => x.EmployeeId == commentUser.EmployeeId);
                                var _commentEmployeeName = commentEmployee?.EmployeeName;

                                if (!String.IsNullOrEmpty(_commentEmployeeName))
                                {
                                    result = result.Replace(CommentEmployeeName, _commentEmployeeName);
                                }
                                else
                                {
                                    result = result.Replace(CommentEmployeeName, "");
                                }
                            }
                        }

                        if (result.Contains(CommentEmployeeCode))
                        {
                            var commentUser = context.User.FirstOrDefault(x => x.UserId == newNote.CreatedById);
                            if (commentUser != null)
                            {
                                var commentEmployee =
                                    context.Employee.FirstOrDefault(x => x.EmployeeId == commentUser.EmployeeId);
                                var _commentEmployeeCode = commentEmployee?.EmployeeCode;

                                if (!String.IsNullOrEmpty(_commentEmployeeCode))
                                {
                                    result = result.Replace(CommentEmployeeCode, _commentEmployeeCode);
                                }
                                else
                                {
                                    result = result.Replace(CommentEmployeeCode, "");
                                }
                            }
                        }

                        if (result.Contains(CommentCreatedDate))
                        {
                            result = result.Replace(CommentCreatedDate, FormatDateToString(newNote.CreatedDate));
                        }

                        if (result.Contains(CommentContent))
                        {
                            var _commentContent = newNote.Description?.Trim();
                            if (!String.IsNullOrEmpty(_commentContent))
                            {
                                result = result.Replace(CommentContent, _commentContent);
                            }
                            else
                            {
                                result = result.Replace(CommentContent, "");
                            }
                        }
                    }
                }
            }
            //Tạo mới hồ sơ thầu
            else if (typeModel == TypeModel.SaleBidding)
            {
                var _model = model as SaleBidding;

                if (_model != null)
                {
                    if (result.Contains(Logo))
                    {
                        var logo = configEntity.FirstOrDefault(w => w.SystemKey == "Logo").SystemValueString;

                        if (!String.IsNullOrEmpty(logo))
                        {
                            var temp_logo = "<img src=\"" + logo + "\" class=\"e - rte - image e - imginline\" alt=\"Logo TNM.png\" width=\"auto\" height=\"auto\" style=\"min - width: 0px; max - width: 750px; min - height: 0px; \">";
                            result = result.Replace(Logo, temp_logo);
                        }
                        else
                        {
                            result = result.Replace(Logo, "");
                        }
                    }

                    if (result.Contains(SaleBiddingName) && _model.SaleBiddingName != null)
                    {
                        result = result.Replace(SaleBiddingName, _model.SaleBiddingName.Trim());
                    }

                    if (result.Contains(SaleBiddingCode) && _model.SaleBiddingCode != null)
                    {
                        result = result.Replace(SaleBiddingCode, _model.SaleBiddingCode.Trim());
                    }

                    if (result.Contains(EmployeeCode))
                    {
                        var employeeId = context.User.FirstOrDefault(x => x.UserId == _model.CreatedById)?.EmployeeId;
                        var employeeCode = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeId)?.EmployeeCode;

                        if (!String.IsNullOrEmpty(employeeCode))
                        {
                            result = result.Replace(EmployeeCode, employeeCode);
                        }
                        else
                        {
                            result = result.Replace(EmployeeCode, "");
                        }
                    }

                    if (result.Contains(EmployeeName))
                    {
                        var employeeId = context.User.FirstOrDefault(x => x.UserId == _model.CreatedById)?.EmployeeId;
                        var employeeName = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeId)?.EmployeeName;

                        if (!String.IsNullOrEmpty(employeeName))
                        {
                            result = result.Replace(EmployeeName, employeeName);
                        }
                        else
                        {
                            result = result.Replace(EmployeeName, "");
                        }
                    }

                    if (result.Contains(CreatedDate))
                    {
                        result = result.Replace(CreatedDate, FormatDateToString(_model.CreatedDate));
                    }

                    if (result.Contains(Url_Login))
                    {
                        var Domain = configEntity.FirstOrDefault(w => w.SystemKey == "Domain").SystemValueString;
                        var loginLink = Domain + @"/";

                        if (!String.IsNullOrEmpty(loginLink))
                        {
                            result = result.Replace(Url_Login, loginLink);
                        }
                    }
                }
            }
            //Chi tiết hồ sơ thầu
            else if (typeModel == TypeModel.SaleBiddingDetail)
            {
                var _model = model as SaleBidding;

                if (_model != null)
                {
                    if (result.Contains(Logo))
                    {
                        var logo = configEntity.FirstOrDefault(w => w.SystemKey == "Logo").SystemValueString;

                        if (!String.IsNullOrEmpty(logo))
                        {
                            var temp_logo = "<img src=\"" + logo + "\" class=\"e - rte - image e - imginline\" alt=\"Logo TNM.png\" width=\"auto\" height=\"auto\" style=\"min - width: 0px; max - width: 750px; min - height: 0px; \">";
                            result = result.Replace(Logo, temp_logo);
                        }
                        else
                        {
                            result = result.Replace(Logo, "");
                        }
                    }

                    if (result.Contains(SaleBiddingName) && _model.SaleBiddingName != null)
                    {
                        result = result.Replace(SaleBiddingName, _model.SaleBiddingName.Trim());
                    }

                    if (result.Contains(SaleBiddingCode) && _model.SaleBiddingCode != null)
                    {
                        result = result.Replace(SaleBiddingCode, _model.SaleBiddingCode.Trim());
                    }

                    if (result.Contains(EmployeeCode))
                    {
                        var employeeId = context.User.FirstOrDefault(x => x.UserId == _model.UpdatedById)?.EmployeeId;
                        var employeeCode = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeId)?.EmployeeCode;

                        if (!String.IsNullOrEmpty(employeeCode))
                        {
                            result = result.Replace(EmployeeCode, employeeCode);
                        }
                        else
                        {
                            result = result.Replace(EmployeeCode, "");
                        }
                    }

                    if (result.Contains(EmployeeName))
                    {
                        var employeeId = context.User.FirstOrDefault(x => x.UserId == _model.UpdatedById)?.EmployeeId;
                        var employeeName = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeId)?.EmployeeName;

                        if (!String.IsNullOrEmpty(employeeName))
                        {
                            result = result.Replace(EmployeeName, employeeName);
                        }
                        else
                        {
                            result = result.Replace(EmployeeName, "");
                        }
                    }

                    if (result.Contains(CreatedDate))
                    {
                        result = result.Replace(CreatedDate, FormatDateToString(_model.CreatedDate));
                    }

                    if (result.Contains(UpdatedDate))
                    {
                        result = result.Replace(UpdatedDate, FormatDateToString(_model.UpdatedDate));
                    }

                    if (result.Contains(Url_Login))
                    {
                        var Domain = configEntity.FirstOrDefault(w => w.SystemKey == "Domain").SystemValueString;
                        var loginLink = Domain + @"/";

                        if (!String.IsNullOrEmpty(loginLink))
                        {
                            result = result.Replace(Url_Login, loginLink);
                        }
                    }

                    if (note != null)
                    {
                        var newNote = note as Note;

                        if (result.Contains(CommentEmployeeName))
                        {
                            var commentUser = context.User.FirstOrDefault(x => x.UserId == newNote.CreatedById);
                            if (commentUser != null)
                            {
                                var commentEmployee =
                                    context.Employee.FirstOrDefault(x => x.EmployeeId == commentUser.EmployeeId);
                                var _commentEmployeeName = commentEmployee?.EmployeeName;

                                if (!String.IsNullOrEmpty(_commentEmployeeName))
                                {
                                    result = result.Replace(CommentEmployeeName, _commentEmployeeName);
                                }
                                else
                                {
                                    result = result.Replace(CommentEmployeeName, "");
                                }
                            }
                        }

                        if (result.Contains(CommentEmployeeCode))
                        {
                            var commentUser = context.User.FirstOrDefault(x => x.UserId == newNote.CreatedById);
                            if (commentUser != null)
                            {
                                var commentEmployee =
                                    context.Employee.FirstOrDefault(x => x.EmployeeId == commentUser.EmployeeId);
                                var _commentEmployeeCode = commentEmployee?.EmployeeCode;

                                if (!String.IsNullOrEmpty(_commentEmployeeCode))
                                {
                                    result = result.Replace(CommentEmployeeCode, _commentEmployeeCode);
                                }
                                else
                                {
                                    result = result.Replace(CommentEmployeeCode, "");
                                }
                            }
                        }

                        if (result.Contains(CommentCreatedDate))
                        {
                            result = result.Replace(CommentCreatedDate, FormatDateToString(newNote.CreatedDate));
                        }

                        if (result.Contains(CommentContent))
                        {
                            var _commentContent = newNote.Description?.Trim();
                            if (!String.IsNullOrEmpty(_commentContent))
                            {
                                result = result.Replace(CommentContent, _commentContent);
                            }
                            else
                            {
                                result = result.Replace(CommentContent, "");
                            }
                        }

                        if (result.Contains(Description))
                        {
                            var _description = newNote.Description?.Trim();
                            if (!String.IsNullOrEmpty(_description))
                            {
                                result = result.Replace(Description, _description);
                            }
                            else
                            {
                                result = result.Replace(Description, "");
                            }
                        }
                    }
                }
            }
            //Tạo hợp đồng bán
            else if (typeModel == TypeModel.Contract)
            {
                var _model = model as Contract;

                if (_model != null)
                {
                    if (result.Contains(Logo))
                    {
                        var logo = configEntity.FirstOrDefault(w => w.SystemKey == "Logo").SystemValueString;

                        if (!String.IsNullOrEmpty(logo))
                        {
                            var temp_logo = "<img src=\"" + logo + "\" class=\"e - rte - image e - imginline\" alt=\"Logo TNM.png\" width=\"auto\" height=\"auto\" style=\"min - width: 0px; max - width: 750px; min - height: 0px; \">";
                            result = result.Replace(Logo, temp_logo);
                        }
                        else
                        {
                            result = result.Replace(Logo, "");
                        }
                    }

                    if (result.Contains(CustomerName))
                    {
                        var customerName = context.Customer.FirstOrDefault(c => c.CustomerId == _model.CustomerId)
                            .CustomerName;
                        if (!String.IsNullOrEmpty(customerName))
                        {
                            result = result.Replace(CustomerName, customerName.Trim());
                        }
                    }

                    if (result.Contains(ContractCode) && _model.ContractCode != null)
                    {
                        result = result.Replace(ContractCode, _model.ContractCode.Trim());
                    }

                    if (result.Contains(EmployeeCode))
                    {
                        var employeeId = context.User.FirstOrDefault(x => x.UserId == _model.CreatedById)?.EmployeeId;
                        var employeeCode = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeId)?.EmployeeCode;

                        if (!String.IsNullOrEmpty(employeeCode))
                        {
                            result = result.Replace(EmployeeCode, employeeCode);
                        }
                        else
                        {
                            result = result.Replace(EmployeeCode, "");
                        }
                    }

                    if (result.Contains(EmployeeName))
                    {
                        var employeeId = context.User.FirstOrDefault(x => x.UserId == _model.CreatedById)?.EmployeeId;
                        var employeeName = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeId)?.EmployeeName;

                        if (!String.IsNullOrEmpty(employeeName))
                        {
                            result = result.Replace(EmployeeName, employeeName);
                        }
                        else
                        {
                            result = result.Replace(EmployeeName, "");
                        }
                    }

                    if (result.Contains(CreatedDate))
                    {
                        result = result.Replace(CreatedDate, FormatDateToString(_model.CreatedDate));
                    }

                    if (result.Contains(Url_Login))
                    {
                        var Domain = configEntity.FirstOrDefault(w => w.SystemKey == "Domain").SystemValueString;
                        var loginLink = Domain + @"/";

                        if (!String.IsNullOrEmpty(loginLink))
                        {
                            result = result.Replace(Url_Login, loginLink);
                        }
                    }
                }
            }
            //Chi tiết hợp đồng bán
            else if (typeModel == TypeModel.ContractDetail)
            {
                var _model = model as Contract;

                if (_model != null)
                {
                    if (result.Contains(Logo))
                    {
                        var logo = configEntity.FirstOrDefault(w => w.SystemKey == "Logo").SystemValueString;

                        if (!String.IsNullOrEmpty(logo))
                        {
                            var temp_logo = "<img src=\"" + logo + "\" class=\"e - rte - image e - imginline\" alt=\"Logo TNM.png\" width=\"auto\" height=\"auto\" style=\"min - width: 0px; max - width: 750px; min - height: 0px; \">";
                            result = result.Replace(Logo, temp_logo);
                        }
                        else
                        {
                            result = result.Replace(Logo, "");
                        }
                    }

                    if (result.Contains(CustomerName))
                    {
                        var customerName = context.Customer.FirstOrDefault(c => c.CustomerId == _model.CustomerId)
                            .CustomerName;
                        if (!String.IsNullOrEmpty(customerName))
                        {
                            result = result.Replace(CustomerName, customerName.Trim());
                        }
                    }

                    if (result.Contains(ContractCode) && _model.ContractCode != null)
                    {
                        result = result.Replace(ContractCode, _model.ContractCode.Trim());
                    }
                    if (result.Contains(ContractName) && _model.ContractName != null)
                    {
                        result = result.Replace(ContractName, _model.ContractName.Trim());
                    }
                    if (result.Contains(ContractEffective) && _model.EffectiveDate != null)
                    {
                        result = result.Replace(ContractEffective, FormatDateToString(_model.EffectiveDate));
                    }
                    if (result.Contains(ContractTime) && _model.ContractTime != null)
                    {
                        result = result.Replace(ContractTime, _model.ContractTime.Value.ToString());
                    }
                    if (result.Contains(ContractExpired) && _model.ExpiredDate != null)
                    {
                        result = result.Replace(ContractExpired, FormatDateToString(_model.ExpiredDate));
                    }
                    if (result.Contains(ContractAmount) && _model.Amount != null)
                    {
                        result = result.Replace(ContractAmount, _model.Amount.ToString());
                    }

                    if (result.Contains(EmployeeCode))
                    {
                        var employeeId = context.User.FirstOrDefault(x => x.UserId == _model.UpdatedById)?.EmployeeId;
                        var employeeCode = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeId)?.EmployeeCode;

                        if (!String.IsNullOrEmpty(employeeCode))
                        {
                            result = result.Replace(EmployeeCode, employeeCode);
                        }
                        else
                        {
                            result = result.Replace(EmployeeCode, "");
                        }
                    }

                    if (result.Contains(EmployeeName))
                    {
                        var employeeId = context.User.FirstOrDefault(x => x.UserId == _model.UpdatedById)?.EmployeeId;
                        var employeeName = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeId)?.EmployeeName;

                        if (!String.IsNullOrEmpty(employeeName))
                        {
                            result = result.Replace(EmployeeName, employeeName);
                        }
                        else
                        {
                            result = result.Replace(EmployeeName, "");
                        }
                    }

                    if (result.Contains(CreatedDate))
                    {
                        result = result.Replace(CreatedDate, FormatDateToString(_model.CreatedDate));
                    }

                    if (result.Contains(UpdatedDate))
                    {
                        result = result.Replace(UpdatedDate, FormatDateToString(_model.UpdatedDate));
                    }

                    if (result.Contains(Url_Login))
                    {
                        var Domain = configEntity.FirstOrDefault(w => w.SystemKey == "Domain").SystemValueString;
                        var loginLink = Domain + @"/";

                        if (!String.IsNullOrEmpty(loginLink))
                        {
                            result = result.Replace(Url_Login, loginLink);
                        }
                    }

                    if (note != null)
                    {
                        var newNote = note as Note;

                        if (result.Contains(CommentEmployeeName))
                        {
                            var commentUser = context.User.FirstOrDefault(x => x.UserId == newNote.CreatedById);
                            if (commentUser != null)
                            {
                                var commentEmployee =
                                    context.Employee.FirstOrDefault(x => x.EmployeeId == commentUser.EmployeeId);
                                var _commentEmployeeName = commentEmployee?.EmployeeName;

                                if (!String.IsNullOrEmpty(_commentEmployeeName))
                                {
                                    result = result.Replace(CommentEmployeeName, _commentEmployeeName);
                                }
                                else
                                {
                                    result = result.Replace(CommentEmployeeName, "");
                                }
                            }
                        }

                        if (result.Contains(CommentEmployeeCode))
                        {
                            var commentUser = context.User.FirstOrDefault(x => x.UserId == newNote.CreatedById);
                            if (commentUser != null)
                            {
                                var commentEmployee =
                                    context.Employee.FirstOrDefault(x => x.EmployeeId == commentUser.EmployeeId);
                                var _commentEmployeeCode = commentEmployee?.EmployeeCode;

                                if (!String.IsNullOrEmpty(_commentEmployeeCode))
                                {
                                    result = result.Replace(CommentEmployeeCode, _commentEmployeeCode);
                                }
                                else
                                {
                                    result = result.Replace(CommentEmployeeCode, "");
                                }
                            }
                        }

                        if (result.Contains(CommentCreatedDate))
                        {
                            result = result.Replace(CommentCreatedDate, FormatDateToString(newNote.CreatedDate));
                        }

                        if (result.Contains(CommentContent))
                        {
                            var _commentContent = newNote.Description?.Trim();
                            if (!String.IsNullOrEmpty(_commentContent))
                            {
                                result = result.Replace(CommentContent, _commentContent);
                            }
                            else
                            {
                                result = result.Replace(CommentContent, "");
                            }
                        }

                        if (result.Contains(ContractDescription))
                        {
                            var _commentContent = newNote.Description?.Trim();
                            if (!String.IsNullOrEmpty(_commentContent))
                            {
                                result = result.Replace(ContractDescription, _commentContent);
                            }
                            else
                            {
                                result = result.Replace(ContractDescription, "");
                            }
                        }
                    }
                }
            }
            //Tạo hóa đơn
            else if (typeModel == TypeModel.BillSale)
            {
                var _model = model as BillOfSale;

                if (_model != null)
                {
                    if (result.Contains(Logo))
                    {
                        var logo = configEntity.FirstOrDefault(w => w.SystemKey == "Logo").SystemValueString;

                        if (!String.IsNullOrEmpty(logo))
                        {
                            var temp_logo = "<img src=\"" + logo + "\" class=\"e - rte - image e - imginline\" alt=\"Logo TNM.png\" width=\"auto\" height=\"auto\" style=\"min - width: 0px; max - width: 750px; min - height: 0px; \">";
                            result = result.Replace(Logo, temp_logo);
                        }
                        else
                        {
                            result = result.Replace(Logo, "");
                        }
                    }

                    if (result.Contains(BillName) && _model.InvoiceSymbol != null)
                    {
                        result = result.Replace(BillName, _model.InvoiceSymbol.Trim());
                    }

                    if (result.Contains(BillCode) && _model.BillOfSaLeCode != null)
                    {
                        result = result.Replace(BillCode, _model.BillOfSaLeCode.Trim());
                    }

                    if (result.Contains(EmployeeCode))
                    {
                        var employeeId = context.User.FirstOrDefault(x => x.UserId == _model.CreatedById)?.EmployeeId;
                        var employeeCode = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeId)?.EmployeeCode;

                        if (!String.IsNullOrEmpty(employeeCode))
                        {
                            result = result.Replace(EmployeeCode, employeeCode);
                        }
                        else
                        {
                            result = result.Replace(EmployeeCode, "");
                        }
                    }

                    if (result.Contains(EmployeeName))
                    {
                        var employeeId = context.User.FirstOrDefault(x => x.UserId == _model.CreatedById)?.EmployeeId;
                        var employeeName = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeId)?.EmployeeName;

                        if (!String.IsNullOrEmpty(employeeName))
                        {
                            result = result.Replace(EmployeeName, employeeName);
                        }
                        else
                        {
                            result = result.Replace(EmployeeName, "");
                        }
                    }

                    if (result.Contains(CreatedDate))
                    {
                        result = result.Replace(CreatedDate, FormatDateToString(_model.CreatedDate));
                    }

                    if (result.Contains(Url_Login))
                    {
                        var Domain = configEntity.FirstOrDefault(w => w.SystemKey == "Domain").SystemValueString;
                        var loginLink = Domain + @"/";

                        if (!String.IsNullOrEmpty(loginLink))
                        {
                            result = result.Replace(Url_Login, loginLink);
                        }
                    }
                }
            }
            //Chi tiết hóa đơn
            else if (typeModel == TypeModel.BillSaleDetail)
            {
                var _model = model as BillOfSale;

                if (_model != null)
                {
                    if (result.Contains(Logo))
                    {
                        var logo = configEntity.FirstOrDefault(w => w.SystemKey == "Logo").SystemValueString;

                        if (!String.IsNullOrEmpty(logo))
                        {
                            var temp_logo = "<img src=\"" + logo + "\" class=\"e - rte - image e - imginline\" alt=\"Logo TNM.png\" width=\"auto\" height=\"auto\" style=\"min - width: 0px; max - width: 750px; min - height: 0px; \">";
                            result = result.Replace(Logo, temp_logo);
                        }
                        else
                        {
                            result = result.Replace(Logo, "");
                        }
                    }

                    if (result.Contains(BillName) && _model.InvoiceSymbol != null)
                    {
                        result = result.Replace(BillName, _model.InvoiceSymbol.Trim());
                    }

                    if (result.Contains(BillCode) && _model.BillOfSaLeCode != null)
                    {
                        result = result.Replace(BillCode, _model.BillOfSaLeCode.Trim());
                    }

                    if (result.Contains(EmployeeCode))
                    {
                        var employeeId = context.User.FirstOrDefault(x => x.UserId == _model.UpdatedById)?.EmployeeId;
                        var employeeCode = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeId)?.EmployeeCode;

                        if (!String.IsNullOrEmpty(employeeCode))
                        {
                            result = result.Replace(EmployeeCode, employeeCode);
                        }
                        else
                        {
                            result = result.Replace(EmployeeCode, "");
                        }
                    }

                    if (result.Contains(EmployeeName))
                    {
                        var employeeId = context.User.FirstOrDefault(x => x.UserId == _model.UpdatedById)?.EmployeeId;
                        var employeeName = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeId)?.EmployeeName;

                        if (!String.IsNullOrEmpty(employeeName))
                        {
                            result = result.Replace(EmployeeName, employeeName);
                        }
                        else
                        {
                            result = result.Replace(EmployeeName, "");
                        }
                    }

                    if (result.Contains(CreatedDate))
                    {
                        result = result.Replace(CreatedDate, FormatDateToString(_model.CreatedDate));
                    }

                    if (result.Contains(UpdatedDate))
                    {
                        result = result.Replace(UpdatedDate, FormatDateToString(_model.UpdatedDate));
                    }

                    if (result.Contains(Url_Login))
                    {
                        var Domain = configEntity.FirstOrDefault(w => w.SystemKey == "Domain").SystemValueString;
                        var loginLink = Domain + @"/";

                        if (!String.IsNullOrEmpty(loginLink))
                        {
                            result = result.Replace(Url_Login, loginLink);
                        }
                    }

                    if (note != null)
                    {
                        var newNote = note as Note;

                        if (result.Contains(CommentEmployeeName))
                        {
                            var commentUser = context.User.FirstOrDefault(x => x.UserId == newNote.CreatedById);
                            if (commentUser != null)
                            {
                                var commentEmployee =
                                    context.Employee.FirstOrDefault(x => x.EmployeeId == commentUser.EmployeeId);
                                var _commentEmployeeName = commentEmployee?.EmployeeName;

                                if (!String.IsNullOrEmpty(_commentEmployeeName))
                                {
                                    result = result.Replace(CommentEmployeeName, _commentEmployeeName);
                                }
                                else
                                {
                                    result = result.Replace(CommentEmployeeName, "");
                                }
                            }
                        }

                        if (result.Contains(CommentEmployeeCode))
                        {
                            var commentUser = context.User.FirstOrDefault(x => x.UserId == newNote.CreatedById);
                            if (commentUser != null)
                            {
                                var commentEmployee =
                                    context.Employee.FirstOrDefault(x => x.EmployeeId == commentUser.EmployeeId);
                                var _commentEmployeeCode = commentEmployee?.EmployeeCode;

                                if (!String.IsNullOrEmpty(_commentEmployeeCode))
                                {
                                    result = result.Replace(CommentEmployeeCode, _commentEmployeeCode);
                                }
                                else
                                {
                                    result = result.Replace(CommentEmployeeCode, "");
                                }
                            }
                        }

                        if (result.Contains(CommentCreatedDate))
                        {
                            result = result.Replace(CommentCreatedDate, FormatDateToString(newNote.CreatedDate));
                        }

                        if (result.Contains(CommentContent))
                        {
                            var _commentContent = newNote.Description?.Trim();
                            if (!String.IsNullOrEmpty(_commentContent))
                            {
                                result = result.Replace(CommentContent, _commentContent);
                            }
                            else
                            {
                                result = result.Replace(CommentContent, "");
                            }
                        }
                    }
                }
            }
            //Tạo đề xuất mua hàng
            else if (typeModel == TypeModel.ProcurementRequest)
            {
                var _model = model as ProcurementRequest;

                if (_model != null)
                {
                    if (result.Contains(Logo))
                    {
                        var logo = configEntity.FirstOrDefault(w => w.SystemKey == "Logo").SystemValueString;

                        if (!String.IsNullOrEmpty(logo))
                        {
                            var temp_logo = "<img src=\"" + logo + "\" class=\"e - rte - image e - imginline\" alt=\"Logo TNM.png\" width=\"auto\" height=\"auto\" style=\"min - width: 0px; max - width: 750px; min - height: 0px; \">";
                            result = result.Replace(Logo, temp_logo);
                        }
                        else
                        {
                            result = result.Replace(Logo, "");
                        }
                    }

                    if (result.Contains(requestCode) && _model.ProcurementCode != null)
                    {
                        result = result.Replace(requestCode, _model.ProcurementCode.Trim());
                    }

                    if (result.Contains(requestEmployee))
                    {
                        var _requestEmployee = context.Employee
                            .FirstOrDefault(x => x.EmployeeId == _model.RequestEmployeeId).EmployeeName;
                        if (!String.IsNullOrEmpty(_requestEmployee))
                        {
                            result = result.Replace(requestEmployee, _requestEmployee);
                        }
                    }

                    if (result.Contains(EmployeeCode))
                    {
                        var employeeId = context.User.FirstOrDefault(x => x.UserId == _model.CreatedById)?.EmployeeId;
                        var employeeCode = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeId)?.EmployeeCode;

                        if (!String.IsNullOrEmpty(employeeCode))
                        {
                            result = result.Replace(EmployeeCode, employeeCode);
                        }
                        else
                        {
                            result = result.Replace(EmployeeCode, "");
                        }
                    }

                    if (result.Contains(EmployeeName))
                    {
                        var employeeId = context.User.FirstOrDefault(x => x.UserId == _model.CreatedById)?.EmployeeId;
                        var employeeName = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeId)?.EmployeeName;

                        if (!String.IsNullOrEmpty(employeeName))
                        {
                            result = result.Replace(EmployeeName, employeeName);
                        }
                        else
                        {
                            result = result.Replace(EmployeeName, "");
                        }
                    }

                    if (result.Contains(CreatedDate))
                    {
                        result = result.Replace(CreatedDate, FormatDateToString(_model.CreatedDate));
                    }

                    if (result.Contains(Url_Login))
                    {
                        var Domain = configEntity.FirstOrDefault(w => w.SystemKey == "Domain").SystemValueString;
                        var loginLink = Domain + @"/";

                        if (!String.IsNullOrEmpty(loginLink))
                        {
                            result = result.Replace(Url_Login, loginLink);
                        }
                    }
                }
            }
            //Chi tiết đề xuất mua hàng
            else if (typeModel == TypeModel.ProcurementRequestDetail)
            {
                var _model = model as ProcurementRequest;

                if (_model != null)
                {
                    if (result.Contains(Logo))
                    {
                        var logo = configEntity.FirstOrDefault(w => w.SystemKey == "Logo").SystemValueString;

                        if (!String.IsNullOrEmpty(logo))
                        {
                            var temp_logo = "<img src=\"" + logo + "\" class=\"e - rte - image e - imginline\" alt=\"Logo TNM.png\" width=\"auto\" height=\"auto\" style=\"min - width: 0px; max - width: 750px; min - height: 0px; \">";
                            result = result.Replace(Logo, temp_logo);
                        }
                        else
                        {
                            result = result.Replace(Logo, "");
                        }
                    }

                    if (result.Contains(requestCode) && _model.ProcurementCode != null)
                    {
                        result = result.Replace(requestCode, _model.ProcurementCode.Trim());
                    }

                    if (result.Contains(requestEmployee))
                    {
                        var _requestEmployee = context.Employee
                            .FirstOrDefault(x => x.EmployeeId == _model.RequestEmployeeId).EmployeeName;
                        if (!String.IsNullOrEmpty(_requestEmployee))
                        {
                            result = result.Replace(requestEmployee, _requestEmployee);
                        }
                    }

                    if (result.Contains(EmployeeCode))
                    {
                        var employeeId = context.User.FirstOrDefault(x => x.UserId == _model.UpdatedById)?.EmployeeId;
                        var employeeCode = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeId)?.EmployeeCode;

                        if (!String.IsNullOrEmpty(employeeCode))
                        {
                            result = result.Replace(EmployeeCode, employeeCode);
                        }
                        else
                        {
                            result = result.Replace(EmployeeCode, "");
                        }
                    }

                    if (result.Contains(EmployeeName))
                    {
                        var employeeId = context.User.FirstOrDefault(x => x.UserId == _model.UpdatedById)?.EmployeeId;
                        var employeeName = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeId)?.EmployeeName;

                        if (!String.IsNullOrEmpty(employeeName))
                        {
                            result = result.Replace(EmployeeName, employeeName);
                        }
                        else
                        {
                            result = result.Replace(EmployeeName, "");
                        }
                    }

                    if (result.Contains(CreatedDate))
                    {
                        result = result.Replace(CreatedDate, FormatDateToString(_model.CreatedDate));
                    }

                    if (result.Contains(UpdatedDate))
                    {
                        result = result.Replace(UpdatedDate, FormatDateToString(_model.UpdatedDate));
                    }

                    if (result.Contains(Url_Login))
                    {
                        var Domain = configEntity.FirstOrDefault(w => w.SystemKey == "Domain").SystemValueString;
                        var loginLink = Domain + @"/";

                        if (!String.IsNullOrEmpty(loginLink))
                        {
                            result = result.Replace(Url_Login, loginLink);
                        }
                    }

                    if (note != null)
                    {
                        var newNote = note as Note;

                        if (result.Contains(CommentEmployeeName))
                        {
                            var commentUser = context.User.FirstOrDefault(x => x.UserId == newNote.CreatedById);
                            if (commentUser != null)
                            {
                                var commentEmployee =
                                    context.Employee.FirstOrDefault(x => x.EmployeeId == commentUser.EmployeeId);
                                var _commentEmployeeName = commentEmployee?.EmployeeName;

                                if (!String.IsNullOrEmpty(_commentEmployeeName))
                                {
                                    result = result.Replace(CommentEmployeeName, _commentEmployeeName);
                                }
                                else
                                {
                                    result = result.Replace(CommentEmployeeName, "");
                                }
                            }
                        }

                        if (result.Contains(CommentEmployeeCode))
                        {
                            var commentUser = context.User.FirstOrDefault(x => x.UserId == newNote.CreatedById);
                            if (commentUser != null)
                            {
                                var commentEmployee =
                                    context.Employee.FirstOrDefault(x => x.EmployeeId == commentUser.EmployeeId);
                                var _commentEmployeeCode = commentEmployee?.EmployeeCode;

                                if (!String.IsNullOrEmpty(_commentEmployeeCode))
                                {
                                    result = result.Replace(CommentEmployeeCode, _commentEmployeeCode);
                                }
                                else
                                {
                                    result = result.Replace(CommentEmployeeCode, "");
                                }
                            }
                        }

                        if (result.Contains(CommentCreatedDate))
                        {
                            result = result.Replace(CommentCreatedDate, FormatDateToString(newNote.CreatedDate));
                        }

                        if (result.Contains(CommentContent))
                        {
                            var _commentContent = newNote.Description?.Trim();
                            if (!String.IsNullOrEmpty(_commentContent))
                            {
                                result = result.Replace(CommentContent, _commentContent);
                            }
                            else
                            {
                                result = result.Replace(CommentContent, "");
                            }
                        }
                    }
                }
            }
            //Tạo khách hàng tiềm năng
            else if (typeModel == TypeModel.PotentialCustomer)
            {
                var _model = model as Customer;

                if (_model != null)
                {
                    if (result.Contains(Logo))
                    {
                        var logo = configEntity.FirstOrDefault(w => w.SystemKey == "Logo").SystemValueString;

                        if (!String.IsNullOrEmpty(logo))
                        {
                            var temp_logo = "<img src=\"" + logo + "\" class=\"e - rte - image e - imginline\" alt=\"Logo TNM.png\" width=\"auto\" height=\"auto\" style=\"min - width: 0px; max - width: 750px; min - height: 0px; \">";
                            result = result.Replace(Logo, temp_logo);
                        }
                        else
                        {
                            result = result.Replace(Logo, "");
                        }
                    }

                    if (result.Contains(PotentialCustomerName) && _model.CustomerName != null)
                    {
                        result = result.Replace(PotentialCustomerName, _model.CustomerName);
                    }

                    if (result.Contains(EmployeeCode))
                    {
                        var employeeId = context.User.FirstOrDefault(x => x.UserId == _model.CreatedById)?.EmployeeId;
                        var employeeCode = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeId)?.EmployeeCode;

                        if (!String.IsNullOrEmpty(employeeCode))
                        {
                            result = result.Replace(EmployeeCode, employeeCode);
                        }
                        else
                        {
                            result = result.Replace(EmployeeCode, "");
                        }
                    }

                    if (result.Contains(EmployeeName))
                    {
                        var employeeId = context.User.FirstOrDefault(x => x.UserId == _model.CreatedById)?.EmployeeId;
                        var employeeName = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeId)?.EmployeeName;

                        if (!String.IsNullOrEmpty(employeeName))
                        {
                            result = result.Replace(EmployeeName, employeeName);
                        }
                        else
                        {
                            result = result.Replace(EmployeeName, "");
                        }
                    }

                    if (result.Contains(CreatedDate))
                    {
                        result = result.Replace(CreatedDate, FormatDateToString(_model.CreatedDate));
                    }

                    if (result.Contains(Url_Login))
                    {
                        var Domain = configEntity.FirstOrDefault(w => w.SystemKey == "Domain").SystemValueString;
                        var loginLink = Domain + @"/";

                        if (!String.IsNullOrEmpty(loginLink))
                        {
                            result = result.Replace(Url_Login, loginLink);
                        }
                    }
                }
            }
            //Chi tiết khách hàng tiềm năng
            else if (typeModel == TypeModel.PotentialCustomerDetail)
            {
                var _model = model as Customer;

                if (_model != null)
                {
                    if (result.Contains(Logo))
                    {
                        var logo = configEntity.FirstOrDefault(w => w.SystemKey == "Logo").SystemValueString;

                        if (!String.IsNullOrEmpty(logo))
                        {
                            var temp_logo = "<img src=\"" + logo + "\" class=\"e - rte - image e - imginline\" alt=\"Logo TNM.png\" width=\"auto\" height=\"auto\" style=\"min - width: 0px; max - width: 750px; min - height: 0px; \">";
                            result = result.Replace(Logo, temp_logo);
                        }
                        else
                        {
                            result = result.Replace(Logo, "");
                        }
                    }

                    if (result.Contains(PotentialCustomerName) && _model.CustomerName != null)
                    {
                        result = result.Replace(PotentialCustomerName, _model.CustomerName);
                    }

                    if (result.Contains(EmployeeCode))
                    {
                        var employeeId = context.User.FirstOrDefault(x => x.UserId == _model.UpdatedById)?.EmployeeId;
                        var employeeCode = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeId)?.EmployeeCode;

                        if (!String.IsNullOrEmpty(employeeCode))
                        {
                            result = result.Replace(EmployeeCode, employeeCode);
                        }
                        else
                        {
                            result = result.Replace(EmployeeCode, "");
                        }
                    }

                    if (result.Contains(EmployeeName))
                    {
                        var employeeId = context.User.FirstOrDefault(x => x.UserId == _model.UpdatedById)?.EmployeeId;
                        var employeeName = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeId)?.EmployeeName;

                        if (!String.IsNullOrEmpty(employeeName))
                        {
                            result = result.Replace(EmployeeName, employeeName);
                        }
                        else
                        {
                            result = result.Replace(EmployeeName, "");
                        }
                    }

                    if (result.Contains(CreatedDate))
                    {
                        result = result.Replace(CreatedDate, FormatDateToString(_model.CreatedDate));
                    }

                    if (result.Contains(UpdatedDate))
                    {
                        result = result.Replace(UpdatedDate, FormatDateToString(_model.UpdatedDate));
                    }

                    if (result.Contains(Url_Login))
                    {
                        var Domain = configEntity.FirstOrDefault(w => w.SystemKey == "Domain").SystemValueString;
                        var loginLink = Domain + @"/";

                        if (!String.IsNullOrEmpty(loginLink))
                        {
                            result = result.Replace(Url_Login, loginLink);
                        }
                    }

                    if (note != null)
                    {
                        var newNote = note as Note;

                        if (result.Contains(CommentEmployeeName))
                        {
                            var commentUser = context.User.FirstOrDefault(x => x.UserId == newNote.CreatedById);
                            if (commentUser != null)
                            {
                                var commentEmployee =
                                    context.Employee.FirstOrDefault(x => x.EmployeeId == commentUser.EmployeeId);
                                var _commentEmployeeName = commentEmployee?.EmployeeName;

                                if (!String.IsNullOrEmpty(_commentEmployeeName))
                                {
                                    result = result.Replace(CommentEmployeeName, _commentEmployeeName);
                                }
                                else
                                {
                                    result = result.Replace(CommentEmployeeName, "");
                                }
                            }
                        }

                        if (result.Contains(CommentEmployeeCode))
                        {
                            var commentUser = context.User.FirstOrDefault(x => x.UserId == newNote.CreatedById);
                            if (commentUser != null)
                            {
                                var commentEmployee =
                                    context.Employee.FirstOrDefault(x => x.EmployeeId == commentUser.EmployeeId);
                                var _commentEmployeeCode = commentEmployee?.EmployeeCode;

                                if (!String.IsNullOrEmpty(_commentEmployeeCode))
                                {
                                    result = result.Replace(CommentEmployeeCode, _commentEmployeeCode);
                                }
                                else
                                {
                                    result = result.Replace(CommentEmployeeCode, "");
                                }
                            }
                        }

                        if (result.Contains(CommentCreatedDate))
                        {
                            result = result.Replace(CommentCreatedDate, FormatDateToString(newNote.CreatedDate));
                        }

                        if (result.Contains(CommentContent))
                        {
                            var _commentContent = newNote.Description?.Trim();
                            if (!String.IsNullOrEmpty(_commentContent))
                            {
                                result = result.Replace(CommentContent, _commentContent);
                            }
                            else
                            {
                                result = result.Replace(CommentContent, "");
                            }
                        }
                    }
                }
            }
            //Tạo khách hàng
            else if (typeModel == TypeModel.Customer)
            {
                var _model = model as Customer;

                if (_model != null)
                {
                    if (result.Contains(Logo))
                    {
                        var logo = configEntity.FirstOrDefault(w => w.SystemKey == "Logo").SystemValueString;

                        if (!String.IsNullOrEmpty(logo))
                        {
                            var temp_logo = "<img src=\"" + logo + "\" class=\"e - rte - image e - imginline\" alt=\"Logo TNM.png\" width=\"auto\" height=\"auto\" style=\"min - width: 0px; max - width: 750px; min - height: 0px; \">";
                            result = result.Replace(Logo, temp_logo);
                        }
                        else
                        {
                            result = result.Replace(Logo, "");
                        }
                    }

                    if (result.Contains(CustomerCode) && _model.CustomerCode != null)
                    {
                        result = result.Replace(CustomerCode, _model.CustomerCode);
                    }

                    if (result.Contains(CustomerName) && _model.CustomerName != null)
                    {
                        result = result.Replace(CustomerName, _model.CustomerName);
                    }

                    if (result.Contains(EmployeeCode))
                    {
                        var employeeId = context.User.FirstOrDefault(x => x.UserId == _model.CreatedById)?.EmployeeId;
                        var employeeCode = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeId)?.EmployeeCode;

                        if (!String.IsNullOrEmpty(employeeCode))
                        {
                            result = result.Replace(EmployeeCode, employeeCode);
                        }
                        else
                        {
                            result = result.Replace(EmployeeCode, "");
                        }
                    }

                    if (result.Contains(EmployeeName))
                    {
                        var employeeId = context.User.FirstOrDefault(x => x.UserId == _model.CreatedById)?.EmployeeId;
                        var employeeName = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeId)?.EmployeeName;

                        if (!String.IsNullOrEmpty(employeeName))
                        {
                            result = result.Replace(EmployeeName, employeeName);
                        }
                        else
                        {
                            result = result.Replace(EmployeeName, "");
                        }
                    }

                    if (result.Contains(CreatedDate))
                    {
                        result = result.Replace(CreatedDate, FormatDateToString(_model.CreatedDate));
                    }

                    if (result.Contains(Url_Login))
                    {
                        var Domain = configEntity.FirstOrDefault(w => w.SystemKey == "Domain").SystemValueString;
                        var loginLink = Domain + @"/";

                        if (!String.IsNullOrEmpty(loginLink))
                        {
                            result = result.Replace(Url_Login, loginLink);
                        }
                    }
                }
            }
            //Chi tiết khách hàng
            else if (typeModel == TypeModel.CustomerDetail)
            {
                var _model = model as Customer;

                if (_model != null)
                {
                    if (result.Contains(Logo))
                    {
                        var logo = configEntity.FirstOrDefault(w => w.SystemKey == "Logo").SystemValueString;

                        if (!String.IsNullOrEmpty(logo))
                        {
                            var temp_logo = "<img src=\"" + logo + "\" class=\"e - rte - image e - imginline\" alt=\"Logo TNM.png\" width=\"auto\" height=\"auto\" style=\"min - width: 0px; max - width: 750px; min - height: 0px; \">";
                            result = result.Replace(Logo, temp_logo);
                        }
                        else
                        {
                            result = result.Replace(Logo, "");
                        }
                    }

                    if (result.Contains(CustomerCode) && _model.CustomerCode != null)
                    {
                        result = result.Replace(CustomerCode, _model.CustomerCode);
                    }

                    if (result.Contains(CustomerName) && _model.CustomerName != null)
                    {
                        result = result.Replace(CustomerName, _model.CustomerName);
                    }

                    if (result.Contains(EmployeeCode))
                    {
                        var employeeId = context.User.FirstOrDefault(x => x.UserId == _model.UpdatedById)?.EmployeeId;
                        var employeeCode = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeId)?.EmployeeCode;

                        if (!String.IsNullOrEmpty(employeeCode))
                        {
                            result = result.Replace(EmployeeCode, employeeCode);
                        }
                        else
                        {
                            result = result.Replace(EmployeeCode, "");
                        }
                    }

                    if (result.Contains(EmployeeName))
                    {
                        var employeeId = context.User.FirstOrDefault(x => x.UserId == _model.UpdatedById)?.EmployeeId;
                        var employeeName = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeId)?.EmployeeName;

                        if (!String.IsNullOrEmpty(employeeName))
                        {
                            result = result.Replace(EmployeeName, employeeName);
                        }
                        else
                        {
                            result = result.Replace(EmployeeName, "");
                        }
                    }

                    if (result.Contains(CreatedDate))
                    {
                        result = result.Replace(CreatedDate, FormatDateToString(_model.CreatedDate));
                    }

                    if (result.Contains(UpdatedDate))
                    {
                        result = result.Replace(UpdatedDate, FormatDateToString(_model.UpdatedDate));
                    }

                    if (result.Contains(Url_Login))
                    {
                        var Domain = configEntity.FirstOrDefault(w => w.SystemKey == "Domain").SystemValueString;
                        var loginLink = Domain + @"/";

                        if (!String.IsNullOrEmpty(loginLink))
                        {
                            result = result.Replace(Url_Login, loginLink);
                        }
                    }

                    if (note != null)
                    {
                        var newNote = note as Note;

                        if (result.Contains(CommentEmployeeName))
                        {
                            var commentUser = context.User.FirstOrDefault(x => x.UserId == newNote.CreatedById);
                            if (commentUser != null)
                            {
                                var commentEmployee =
                                    context.Employee.FirstOrDefault(x => x.EmployeeId == commentUser.EmployeeId);
                                var _commentEmployeeName = commentEmployee?.EmployeeName;

                                if (!String.IsNullOrEmpty(_commentEmployeeName))
                                {
                                    result = result.Replace(CommentEmployeeName, _commentEmployeeName);
                                }
                                else
                                {
                                    result = result.Replace(CommentEmployeeName, "");
                                }
                            }
                        }

                        if (result.Contains(CommentEmployeeCode))
                        {
                            var commentUser = context.User.FirstOrDefault(x => x.UserId == newNote.CreatedById);
                            if (commentUser != null)
                            {
                                var commentEmployee =
                                    context.Employee.FirstOrDefault(x => x.EmployeeId == commentUser.EmployeeId);
                                var _commentEmployeeCode = commentEmployee?.EmployeeCode;

                                if (!String.IsNullOrEmpty(_commentEmployeeCode))
                                {
                                    result = result.Replace(CommentEmployeeCode, _commentEmployeeCode);
                                }
                                else
                                {
                                    result = result.Replace(CommentEmployeeCode, "");
                                }
                            }
                        }

                        if (result.Contains(CommentCreatedDate))
                        {
                            result = result.Replace(CommentCreatedDate, FormatDateToString(newNote.CreatedDate));
                        }

                        if (result.Contains(CommentContent))
                        {
                            var _commentContent = newNote.Description?.Trim();
                            if (!String.IsNullOrEmpty(_commentContent))
                            {
                                result = result.Replace(CommentContent, _commentContent);
                            }
                            else
                            {
                                result = result.Replace(CommentContent, "");
                            }
                        }
                    }
                }
            }
            //Tạo đơn hàng mua
            else if (typeModel == TypeModel.VendorOrder)
            {
                var _model = model as VendorOrder;

                if (_model != null)
                {
                    if (result.Contains(Logo))
                    {
                        var logo = configEntity.FirstOrDefault(w => w.SystemKey == "Logo").SystemValueString;

                        if (!String.IsNullOrEmpty(logo))
                        {
                            var temp_logo = "<img src=\"" + logo + "\" class=\"e - rte - image e - imginline\" alt=\"Logo TNM.png\" width=\"auto\" height=\"auto\" style=\"min - width: 0px; max - width: 750px; min - height: 0px; \">";
                            result = result.Replace(Logo, temp_logo);
                        }
                        else
                        {
                            result = result.Replace(Logo, "");
                        }
                    }

                    if (result.Contains(vendorOrderCode) && _model.VendorOrderCode != null)
                    {
                        result = result.Replace(vendorOrderCode, _model.VendorOrderCode);
                    }

                    if (result.Contains(vendorName))
                    {
                        var _vendorName = context.Vendor.FirstOrDefault(x => x.VendorId == _model.VendorId).VendorName;
                        if (!String.IsNullOrEmpty(_vendorName))
                        {
                            result = result.Replace(vendorName, _vendorName);
                        }
                    }

                    if (result.Contains(EmployeeCode))
                    {
                        var employeeId = context.User.FirstOrDefault(x => x.UserId == _model.CreatedById)?.EmployeeId;
                        var employeeCode = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeId)?.EmployeeCode;

                        if (!String.IsNullOrEmpty(employeeCode))
                        {
                            result = result.Replace(EmployeeCode, employeeCode);
                        }
                        else
                        {
                            result = result.Replace(EmployeeCode, "");
                        }
                    }

                    if (result.Contains(EmployeeName))
                    {
                        var employeeId = context.User.FirstOrDefault(x => x.UserId == _model.CreatedById)?.EmployeeId;
                        var employeeName = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeId)?.EmployeeName;

                        if (!String.IsNullOrEmpty(employeeName))
                        {
                            result = result.Replace(EmployeeName, employeeName);
                        }
                        else
                        {
                            result = result.Replace(EmployeeName, "");
                        }
                    }

                    if (result.Contains(CreatedDate))
                    {
                        result = result.Replace(CreatedDate, FormatDateToString(_model.CreatedDate));
                    }

                    if (result.Contains(Url_Login))
                    {
                        var Domain = configEntity.FirstOrDefault(w => w.SystemKey == "Domain").SystemValueString;
                        var loginLink = Domain + @"/";

                        if (!String.IsNullOrEmpty(loginLink))
                        {
                            result = result.Replace(Url_Login, loginLink);
                        }
                    }
                }
            }
            //Chi tiết đơn hàng mua
            else if (typeModel == TypeModel.VendorOrderDetail)
            {
                var _model = model as VendorOrder;

                if (_model != null)
                {
                    if (result.Contains(Logo))
                    {
                        var logo = configEntity.FirstOrDefault(w => w.SystemKey == "Logo").SystemValueString;

                        if (!String.IsNullOrEmpty(logo))
                        {
                            var temp_logo = "<img src=\"" + logo + "\" class=\"e - rte - image e - imginline\" alt=\"Logo TNM.png\" width=\"auto\" height=\"auto\" style=\"min - width: 0px; max - width: 750px; min - height: 0px; \">";
                            result = result.Replace(Logo, temp_logo);
                        }
                        else
                        {
                            result = result.Replace(Logo, "");
                        }
                    }

                    if (result.Contains(vendorOrderCode) && _model.VendorOrderCode != null)
                    {
                        result = result.Replace(vendorOrderCode, _model.VendorOrderCode);
                    }

                    if (result.Contains(vendorName))
                    {
                        var _vendorName = context.Vendor.FirstOrDefault(x => x.VendorId == _model.VendorId).VendorName;
                        if (!String.IsNullOrEmpty(_vendorName))
                        {
                            result = result.Replace(vendorName, _vendorName);
                        }
                    }

                    if (result.Contains(EmployeeCode))
                    {
                        var employeeId = context.User.FirstOrDefault(x => x.UserId == _model.UpdatedById)?.EmployeeId;
                        var employeeCode = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeId)?.EmployeeCode;

                        if (!String.IsNullOrEmpty(employeeCode))
                        {
                            result = result.Replace(EmployeeCode, employeeCode);
                        }
                        else
                        {
                            result = result.Replace(EmployeeCode, "");
                        }
                    }

                    if (result.Contains(EmployeeName))
                    {
                        var employeeId = context.User.FirstOrDefault(x => x.UserId == _model.UpdatedById)?.EmployeeId;
                        var employeeName = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeId)?.EmployeeName;

                        if (!String.IsNullOrEmpty(employeeName))
                        {
                            result = result.Replace(EmployeeName, employeeName);
                        }
                        else
                        {
                            result = result.Replace(EmployeeName, "");
                        }
                    }

                    if (result.Contains(CreatedDate))
                    {
                        result = result.Replace(CreatedDate, FormatDateToString(_model.CreatedDate));
                    }

                    if (result.Contains(UpdatedDate))
                    {
                        result = result.Replace(UpdatedDate, FormatDateToString(_model.UpdatedDate));
                    }

                    if (result.Contains(Url_Login))
                    {
                        var Domain = configEntity.FirstOrDefault(w => w.SystemKey == "Domain").SystemValueString;
                        var loginLink = Domain + @"/";

                        if (!String.IsNullOrEmpty(loginLink))
                        {
                            result = result.Replace(Url_Login, loginLink);
                        }
                    }

                    if (note != null)
                    {
                        var newNote = note as Note;

                        if (result.Contains(CommentEmployeeName))
                        {
                            var commentUser = context.User.FirstOrDefault(x => x.UserId == newNote.CreatedById);
                            if (commentUser != null)
                            {
                                var commentEmployee =
                                    context.Employee.FirstOrDefault(x => x.EmployeeId == commentUser.EmployeeId);
                                var _commentEmployeeName = commentEmployee?.EmployeeName;

                                if (!String.IsNullOrEmpty(_commentEmployeeName))
                                {
                                    result = result.Replace(CommentEmployeeName, _commentEmployeeName);
                                }
                                else
                                {
                                    result = result.Replace(CommentEmployeeName, "");
                                }
                            }
                        }

                        if (result.Contains(CommentEmployeeCode))
                        {
                            var commentUser = context.User.FirstOrDefault(x => x.UserId == newNote.CreatedById);
                            if (commentUser != null)
                            {
                                var commentEmployee =
                                    context.Employee.FirstOrDefault(x => x.EmployeeId == commentUser.EmployeeId);
                                var _commentEmployeeCode = commentEmployee?.EmployeeCode;

                                if (!String.IsNullOrEmpty(_commentEmployeeCode))
                                {
                                    result = result.Replace(CommentEmployeeCode, _commentEmployeeCode);
                                }
                                else
                                {
                                    result = result.Replace(CommentEmployeeCode, "");
                                }
                            }
                        }

                        if (result.Contains(CommentCreatedDate))
                        {
                            result = result.Replace(CommentCreatedDate, FormatDateToString(newNote.CreatedDate));
                        }

                        if (result.Contains(CommentContent))
                        {
                            var _commentContent = newNote.Description?.Trim();
                            if (!String.IsNullOrEmpty(_commentContent))
                            {
                                result = result.Replace(CommentContent, _commentContent);
                            }
                            else
                            {
                                result = result.Replace(CommentContent, "");
                            }
                        }

                        if (result.Contains(Description))
                        {
                            var _description = newNote.Description?.Trim();
                            if (!String.IsNullOrEmpty(_description))
                            {
                                result = result.Replace(Description, _description);
                            }
                            else
                            {
                                result = result.Replace(Description, "");
                            }
                        }
                    }
                }
            }
            //Tạo dự án
            else if (typeModel == TypeModel.Project)
            {
                var _model = model as Project;

                if (_model != null)
                {
                    if (result.Contains(Logo))
                    {
                        var logo = configEntity.FirstOrDefault(w => w.SystemKey == "Logo").SystemValueString;

                        if (!String.IsNullOrEmpty(logo))
                        {
                            var temp_logo = "<img src=\"" + logo + "\" class=\"e - rte - image e - imginline\" alt=\"Logo TNM.png\" width=\"auto\" height=\"auto\" style=\"min - width: 0px; max - width: 750px; min - height: 0px; \">";
                            result = result.Replace(Logo, temp_logo);
                        }
                        else
                        {
                            result = result.Replace(Logo, "");
                        }
                    }

                    if (result.Contains(projectCode) && _model.ProjectCode != null)
                    {
                        result = result.Replace(projectCode, _model.ProjectCode);
                    }

                    if (result.Contains(projectName))
                    {
                        result = result.Replace(projectName, _model.ProjectName);
                    }

                    if (result.Contains(EmployeeCode))
                    {
                        var employeeId = context.User.FirstOrDefault(x => x.UserId == _model.CreateBy)?.EmployeeId;
                        var employeeCode = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeId)?.EmployeeCode;

                        if (!String.IsNullOrEmpty(employeeCode))
                        {
                            result = result.Replace(EmployeeCode, employeeCode);
                        }
                        else
                        {
                            result = result.Replace(EmployeeCode, "");
                        }
                    }

                    if (result.Contains(EmployeeName))
                    {
                        var employeeId = context.User.FirstOrDefault(x => x.UserId == _model.CreateBy)?.EmployeeId;
                        var employeeName = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeId)?.EmployeeName;

                        if (!String.IsNullOrEmpty(employeeName))
                        {
                            result = result.Replace(EmployeeName, employeeName);
                        }
                        else
                        {
                            result = result.Replace(EmployeeName, "");
                        }
                    }

                    if (result.Contains(CreatedDate))
                    {
                        result = result.Replace(CreatedDate, FormatDateToString(_model.CreateDate));
                    }

                    if (result.Contains(Url_Login))
                    {
                        var Domain = configEntity.FirstOrDefault(w => w.SystemKey == "Domain").SystemValueString;
                        var loginLink = Domain + @"/";

                        if (!String.IsNullOrEmpty(loginLink))
                        {
                            result = result.Replace(Url_Login, loginLink);
                        }
                    }

                    if (result.Contains(projectType))
                    {
                        var type = context.Category.FirstOrDefault(x => x.CategoryId == _model.ProjectType)?.CategoryName;

                        if (!string.IsNullOrEmpty(type))
                        {
                            result = result.Replace(projectType, type);
                        }
                        else
                        {
                            result = result.Replace(projectType, "");
                        }
                    }

                    if (result.Contains(projectStartDate))
                    {
                        result = result.Replace(projectStartDate, FormatDateToString(_model.ProjectStartDate));
                    }

                    if (result.Contains(projectEndDate))
                    {
                        result = result.Replace(projectEndDate, FormatDateToString(_model.ProjectEndDate));
                    }

                    if (result.Contains(projectDescription))
                    {
                        if (string.IsNullOrEmpty(_model.Description))
                        {
                            result = result.Replace(projectDescription, "");
                        }
                        else
                        {
                            result = result.Replace(projectDescription, _model.Description);
                        }
                    }

                    if (result.Contains(accName))
                    {
                        if (emailSendTo != null)
                        {
                            var empId = context.Contact.FirstOrDefault(x => x.Email.Equals(emailSendTo))?.ObjectId;
                            var empName = context.Employee.FirstOrDefault(x => x.EmployeeId == empId)?.EmployeeName;
                            if (string.IsNullOrEmpty(empName))
                            {
                                result = result.Replace(accName, "");
                            }
                            else
                            {
                                result = result.Replace(accName, empName);
                            }
                        }
                    }

                    if (result.Contains(accCode))
                    {
                        if (emailSendTo != null)
                        {
                            var empId = context.Contact.FirstOrDefault(x => x.Email.Equals(emailSendTo))?.ObjectId;
                            var empCode = context.Employee.FirstOrDefault(x => x.EmployeeId == empId)?.EmployeeCode;
                            if (string.IsNullOrEmpty(empCode))
                            {
                                result = result.Replace(accCode, "");
                            }
                            else
                            {
                                result = result.Replace(accCode, empCode);
                            }
                        }
                    }
                }
            }
            //Chi tiết dự án
            else if (typeModel == TypeModel.ProjectDetail)
            {
                var _model = model as Project;

                if (_model != null)
                {
                    if (result.Contains(Logo))
                    {
                        var logo = configEntity.FirstOrDefault(w => w.SystemKey == "Logo").SystemValueString;

                        if (!String.IsNullOrEmpty(logo))
                        {
                            var temp_logo = "<img src=\"" + logo + "\" class=\"e - rte - image e - imginline\" alt=\"Logo TNM.png\" width=\"auto\" height=\"auto\" style=\"min - width: 0px; max - width: 750px; min - height: 0px; \">";
                            result = result.Replace(Logo, temp_logo);
                        }
                        else
                        {
                            result = result.Replace(Logo, "");
                        }
                    }

                    if (result.Contains(projectCode) && _model.ProjectCode != null)
                    {
                        result = result.Replace(projectCode, _model.ProjectCode);
                    }

                    if (result.Contains(projectName))
                    {
                        result = result.Replace(projectName, _model.ProjectName);
                    }

                    if (result.Contains(EmployeeCode))
                    {
                        var employeeId = context.User.FirstOrDefault(x => x.UserId == _model.UpdateBy)?.EmployeeId;
                        var employeeCode = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeId)?.EmployeeCode;

                        if (!String.IsNullOrEmpty(employeeCode))
                        {
                            result = result.Replace(EmployeeCode, employeeCode);
                        }
                        else
                        {
                            result = result.Replace(EmployeeCode, "");
                        }
                    }

                    if (result.Contains(EmployeeName))
                    {
                        var employeeId = context.User.FirstOrDefault(x => x.UserId == _model.UpdateBy)?.EmployeeId;
                        var employeeName = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeId)?.EmployeeName;

                        if (!String.IsNullOrEmpty(employeeName))
                        {
                            result = result.Replace(EmployeeName, employeeName);
                        }
                        else
                        {
                            result = result.Replace(EmployeeName, "");
                        }
                    }

                    if (result.Contains(CreatedDate))
                    {
                        result = result.Replace(CreatedDate, FormatDateToString(_model.CreateDate));
                    }

                    if (result.Contains(UpdatedDate))
                    {
                        result = result.Replace(UpdatedDate, FormatDateToString(_model.UpdateDate));
                    }

                    if (result.Contains(Url_Login))
                    {
                        var Domain = configEntity.FirstOrDefault(w => w.SystemKey == "Domain").SystemValueString;
                        var loginLink = Domain + @"/";

                        if (!String.IsNullOrEmpty(loginLink))
                        {
                            result = result.Replace(Url_Login, loginLink);
                        }
                    }

                    if (result.Contains(projectType))
                    {
                        var type = context.Category.FirstOrDefault(x => x.CategoryId == _model.ProjectType)?.CategoryName;

                        if (!string.IsNullOrEmpty(type))
                        {
                            result = result.Replace(projectType, type);
                        }
                        else
                        {
                            result = result.Replace(projectType, "");
                        }
                    }

                    if (result.Contains(projectStatus))
                    {
                        var stt = context.Category.FirstOrDefault(x => x.CategoryId == _model.ProjectStatus)?.CategoryName;

                        if (!string.IsNullOrEmpty(stt))
                        {
                            result = result.Replace(projectStatus, stt);
                        }
                        else
                        {
                            result = result.Replace(projectStatus, "");
                        }
                    }

                    if (result.Contains(projectStartDate))
                    {
                        result = result.Replace(projectStartDate, FormatDateToString(_model.ProjectStartDate));
                    }

                    if (result.Contains(projectEndDate))
                    {
                        result = result.Replace(projectEndDate, FormatDateToString(_model.ProjectEndDate));
                    }

                    if (result.Contains(projectDescription))
                    {
                        if (string.IsNullOrEmpty(_model.Description))
                        {
                            result = result.Replace(projectDescription, "");
                        }
                        else
                        {
                            result = result.Replace(projectDescription, _model.Description);
                        }
                    }

                    if (result.Contains(accName))
                    {
                        if (emailSendTo != null)
                        {
                            var empId = context.Contact.FirstOrDefault(x => x.Email.Equals(emailSendTo))?.ObjectId;
                            var empName = context.Employee.FirstOrDefault(x => x.EmployeeId == empId)?.EmployeeName;
                            if (string.IsNullOrEmpty(empName))
                            {
                                result = result.Replace(accName, "");
                            }
                            else
                            {
                                result = result.Replace(accName, empName);
                            }
                        }
                    }

                    if (result.Contains(accCode))
                    {
                        if (emailSendTo != null)
                        {
                            var empId = context.Contact.FirstOrDefault(x => x.Email.Equals(emailSendTo))?.ObjectId;
                            var empCode = context.Employee.FirstOrDefault(x => x.EmployeeId == empId)?.EmployeeCode;
                            if (string.IsNullOrEmpty(empCode))
                            {
                                result = result.Replace(accCode, "");
                            }
                            else
                            {
                                result = result.Replace(accCode, empCode);
                            }
                        }
                    }

                    if (note != null)
                    {
                        var newNote = note as Note;

                        if (result.Contains(CommentEmployeeName))
                        {
                            var commentUser = context.User.FirstOrDefault(x => x.UserId == newNote.CreatedById);
                            if (commentUser != null)
                            {
                                var commentEmployee =
                                    context.Employee.FirstOrDefault(x => x.EmployeeId == commentUser.EmployeeId);
                                var _commentEmployeeName = commentEmployee?.EmployeeName;

                                if (!String.IsNullOrEmpty(_commentEmployeeName))
                                {
                                    result = result.Replace(CommentEmployeeName, _commentEmployeeName);
                                }
                                else
                                {
                                    result = result.Replace(CommentEmployeeName, "");
                                }
                            }
                        }

                        if (result.Contains(CommentEmployeeCode))
                        {
                            var commentUser = context.User.FirstOrDefault(x => x.UserId == newNote.CreatedById);
                            if (commentUser != null)
                            {
                                var commentEmployee =
                                    context.Employee.FirstOrDefault(x => x.EmployeeId == commentUser.EmployeeId);
                                var _commentEmployeeCode = commentEmployee?.EmployeeCode;

                                if (!String.IsNullOrEmpty(_commentEmployeeCode))
                                {
                                    result = result.Replace(CommentEmployeeCode, _commentEmployeeCode);
                                }
                                else
                                {
                                    result = result.Replace(CommentEmployeeCode, "");
                                }
                            }
                        }

                        if (result.Contains(CommentCreatedDate))
                        {
                            result = result.Replace(CommentCreatedDate, FormatDateToString(newNote.CreatedDate));
                        }

                        if (result.Contains(CommentContent))
                        {
                            var _commentContent = newNote.Description?.Trim();
                            if (!String.IsNullOrEmpty(_commentContent))
                            {
                                result = result.Replace(CommentContent, _commentContent);
                            }
                            else
                            {
                                result = result.Replace(CommentContent, "");
                            }
                        }

                        if (result.Contains(Description))
                        {
                            var _description = newNote.Description?.Trim();
                            if (!String.IsNullOrEmpty(_description))
                            {
                                result = result.Replace(Description, _description);
                            }
                            else
                            {
                                result = result.Replace(Description, "");
                            }
                        }
                    }
                }
            }
            //Tạo công việc
            else if (typeModel == TypeModel.ProjectTask)
            {
                var _model = model as Task;

                if (_model != null)
                {
                    if (result.Contains(Logo))
                    {
                        var logo = configEntity.FirstOrDefault(w => w.SystemKey == "Logo").SystemValueString;

                        if (!String.IsNullOrEmpty(logo))
                        {
                            var temp_logo = "<img src=\"" + logo + "\" class=\"e - rte - image e - imginline\" alt=\"Logo TNM.png\" width=\"auto\" height=\"auto\" style=\"min - width: 0px; max - width: 750px; min - height: 0px; \">";
                            result = result.Replace(Logo, temp_logo);
                        }
                        else
                        {
                            result = result.Replace(Logo, "");
                        }
                    }

                    if (result.Contains(projectName))
                    {
                        var name = context.Project.FirstOrDefault(x => x.ProjectId == _model.ProjectId)?.ProjectName;
                        if (string.IsNullOrEmpty(name))
                        {
                            result = result.Replace(projectName, "");
                        }
                        else
                        {
                            result = result.Replace(projectName, name);
                        }
                    }

                    if (result.Contains(taskCode) && _model.TaskCode != null)
                    {
                        result = result.Replace(taskCode, _model.TaskCode);
                    }

                    if (result.Contains(taskName))
                    {
                        result = result.Replace(taskName, _model.TaskName);
                    }

                    if (result.Contains(taskDescription))
                    {
                        if (string.IsNullOrEmpty(_model.Description))
                        {
                            result = result.Replace(taskDescription, "");
                        }
                        else
                        {
                            result = result.Replace(taskDescription, _model.Description);
                        }
                    }

                    if (result.Contains(EmployeeCode))
                    {
                        var employeeId = context.User.FirstOrDefault(x => x.UserId == _model.CreateBy)?.EmployeeId;
                        var employeeCode = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeId)?.EmployeeCode;

                        if (!String.IsNullOrEmpty(employeeCode))
                        {
                            result = result.Replace(EmployeeCode, employeeCode);
                        }
                        else
                        {
                            result = result.Replace(EmployeeCode, "");
                        }
                    }

                    if (result.Contains(EmployeeName))
                    {
                        var employeeId = context.User.FirstOrDefault(x => x.UserId == _model.CreateBy)?.EmployeeId;
                        var employeeName = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeId)?.EmployeeName;

                        if (!String.IsNullOrEmpty(employeeName))
                        {
                            result = result.Replace(EmployeeName, employeeName);
                        }
                        else
                        {
                            result = result.Replace(EmployeeName, "");
                        }
                    }

                    if (result.Contains(CreatedDate))
                    {
                        result = result.Replace(CreatedDate, FormatDateToString(_model.CreateDate));
                    }

                    if (result.Contains(Url_Login))
                    {
                        var Domain = configEntity.FirstOrDefault(w => w.SystemKey == "Domain").SystemValueString;
                        var loginLink = Domain + @"/";

                        if (!String.IsNullOrEmpty(loginLink))
                        {
                            result = result.Replace(Url_Login, loginLink);
                        }
                    }

                    if (result.Contains(projectType))
                    {

                        var typeId = context.Project.FirstOrDefault(x => x.ProjectId == _model.ProjectId)?.ProjectType;
                        var type = context.Category.FirstOrDefault(x => x.CategoryId == typeId)?.CategoryName;

                        if (!string.IsNullOrEmpty(type))
                        {
                            result = result.Replace(projectType, type);
                        }
                        else
                        {
                            result = result.Replace(projectType, "");
                        }
                    }

                    if (result.Contains(projectStatus))
                    {
                        var statusId = context.Project.FirstOrDefault(x => x.ProjectId == _model.ProjectId)?.ProjectStatus;
                        var stt = context.Category.FirstOrDefault(x => x.CategoryId == statusId)?.CategoryName;

                        if (!string.IsNullOrEmpty(stt))
                        {
                            result = result.Replace(projectStatus, stt);
                        }
                        else
                        {
                            result = result.Replace(projectStatus, "");
                        }
                    }

                    if (result.Contains(projectStartDate))
                    {
                        var startDate = context.Project.FirstOrDefault(x => x.ProjectId == _model.ProjectId)?.ProjectStartDate;
                        if (startDate != null)
                        {
                            result = result.Replace(projectStartDate, FormatDateToString(startDate));
                        }
                        else
                        {
                            result = result.Replace(projectStartDate, "");
                        }
                    }

                    if (result.Contains(projectEndDate))
                    {
                        var endDate = context.Project.FirstOrDefault(x => x.ProjectId == _model.ProjectId)?.ProjectEndDate;
                        if (endDate != null)
                        {
                            result = result.Replace(projectEndDate, FormatDateToString(endDate));
                        }
                        else
                        {
                            result = result.Replace(projectEndDate, "");
                        }
                    }

                    if (result.Contains(accName))
                    {
                        if (emailSendTo != null)
                        {
                            var empId = context.Contact.FirstOrDefault(x => x.Email.Equals(emailSendTo))?.ObjectId;
                            var empName = context.Employee.FirstOrDefault(x => x.EmployeeId == empId)?.EmployeeName;
                            if (string.IsNullOrEmpty(empName))
                            {
                                result = result.Replace(accName, "");
                            }
                            else
                            {
                                result = result.Replace(accName, empName);
                            }
                        }
                    }

                    if (result.Contains(accCode))
                    {
                        if (emailSendTo != null)
                        {
                            var empId = context.Contact.FirstOrDefault(x => x.Email.Equals(emailSendTo))?.ObjectId;
                            var empCode = context.Employee.FirstOrDefault(x => x.EmployeeId == empId)?.EmployeeCode;
                            if (string.IsNullOrEmpty(empCode))
                            {
                                result = result.Replace(accCode, "");
                            }
                            else
                            {
                                result = result.Replace(accCode, empCode);
                            }
                        }
                    }
                }
            }
            //Chi tiết công việc
            else if (typeModel == TypeModel.ProjectTaskDetail)
            {
                var _model = model as Task;

                if (_model != null)
                {
                    if (result.Contains(Logo))
                    {
                        var logo = configEntity.FirstOrDefault(w => w.SystemKey == "Logo").SystemValueString;

                        if (!String.IsNullOrEmpty(logo))
                        {
                            var temp_logo = "<img src=\"" + logo + "\" class=\"e - rte - image e - imginline\" alt=\"Logo TNM.png\" width=\"auto\" height=\"auto\" style=\"min - width: 0px; max - width: 750px; min - height: 0px; \">";
                            result = result.Replace(Logo, temp_logo);
                        }
                        else
                        {
                            result = result.Replace(Logo, "");
                        }
                    }

                    if (result.Contains(projectName))
                    {
                        var name = context.Project.FirstOrDefault(x => x.ProjectId == _model.ProjectId)?.ProjectName;
                        if (string.IsNullOrEmpty(name))
                        {
                            result = result.Replace(projectName, "");
                        }
                        else
                        {
                            result = result.Replace(projectName, name);
                        }
                    }

                    if (result.Contains(taskCode) && _model.TaskCode != null)
                    {
                        result = result.Replace(taskCode, _model.TaskCode);
                    }

                    if (result.Contains(taskName) && _model.TaskName != null)
                    {
                        result = result.Replace(taskName, _model.TaskName);
                    }

                    if (result.Contains(taskDescription))
                    {
                        if (string.IsNullOrEmpty(_model.Description))
                        {
                            result = result.Replace(taskDescription, "");
                        }
                        else
                        {
                            result = result.Replace(taskDescription, _model.Description);
                        }
                    }

                    if (result.Contains(taskPercentComplete))
                    {
                        result = result.Replace(taskPercentComplete, $"{_model.TaskComplate} %");
                    }

                    if (result.Contains(taskStatus))
                    {
                        var stt = context.Category.FirstOrDefault(x => x.CategoryId == _model.Status)?.CategoryName;

                        if (!string.IsNullOrEmpty(stt))
                        {
                            result = result.Replace(taskStatus, stt);
                        }
                        else
                        {
                            result = result.Replace(taskStatus, "");
                        }
                    }

                    if (result.Contains(EmployeeCode))
                    {
                        var employeeId = context.User.FirstOrDefault(x => x.UserId == _model.UpdateBy)?.EmployeeId;
                        var employeeCode = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeId)?.EmployeeCode;

                        if (!String.IsNullOrEmpty(employeeCode))
                        {
                            result = result.Replace(EmployeeCode, employeeCode);
                        }
                        else
                        {
                            result = result.Replace(EmployeeCode, "");
                        }
                    }

                    if (result.Contains(EmployeeName))
                    {
                        var employeeId = context.User.FirstOrDefault(x => x.UserId == _model.UpdateBy)?.EmployeeId;
                        var employeeName = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeId)?.EmployeeName;

                        if (!String.IsNullOrEmpty(employeeName))
                        {
                            result = result.Replace(EmployeeName, employeeName);
                        }
                        else
                        {
                            result = result.Replace(EmployeeName, "");
                        }
                    }

                    if (result.Contains(CreatedDate))
                    {
                        result = result.Replace(CreatedDate, FormatDateToString(_model.CreateDate));
                    }

                    if (result.Contains(UpdatedDate))
                    {
                        result = result.Replace(UpdatedDate, FormatDateToString(_model.UpdateDate));
                    }

                    if (result.Contains(Url_Login))
                    {
                        var Domain = configEntity.FirstOrDefault(w => w.SystemKey == "Domain").SystemValueString;
                        var loginLink = Domain + @"/";

                        if (!String.IsNullOrEmpty(loginLink))
                        {
                            result = result.Replace(Url_Login, loginLink);
                        }
                    }

                    if (result.Contains(projectType))
                    {

                        var typeId = context.Project.FirstOrDefault(x => x.ProjectId == _model.ProjectId)?.ProjectType;
                        var type = context.Category.FirstOrDefault(x => x.CategoryId == typeId)?.CategoryName;

                        if (!string.IsNullOrEmpty(type))
                        {
                            result = result.Replace(projectType, type);
                        }
                        else
                        {
                            result = result.Replace(projectType, "");
                        }
                    }

                    if (result.Contains(projectStatus))
                    {
                        var statusId = context.Project.FirstOrDefault(x => x.ProjectId == _model.ProjectId)?.ProjectStatus;
                        var stt = context.Category.FirstOrDefault(x => x.CategoryId == statusId)?.CategoryName;

                        if (!string.IsNullOrEmpty(stt))
                        {
                            result = result.Replace(projectStatus, stt);
                        }
                        else
                        {
                            result = result.Replace(projectStatus, "");
                        }
                    }

                    if (result.Contains(projectStartDate))
                    {
                        var startDate = context.Project.FirstOrDefault(x => x.ProjectId == _model.ProjectId)?.ProjectStartDate;
                        if (startDate != null)
                        {
                            result = result.Replace(projectStartDate, FormatDateToString(startDate));
                        }
                        else
                        {
                            result = result.Replace(projectStartDate, "");
                        }
                    }

                    if (result.Contains(projectEndDate))
                    {
                        var endDate = context.Project.FirstOrDefault(x => x.ProjectId == _model.ProjectId)?.ProjectEndDate;
                        if (endDate != null)
                        {
                            result = result.Replace(projectEndDate, FormatDateToString(endDate));
                        }
                        else
                        {
                            result = result.Replace(projectEndDate, "");
                        }
                    }

                    if (result.Contains(accName))
                    {
                        if (emailSendTo != null)
                        {
                            var empId = context.Contact.FirstOrDefault(x => x.Email.Equals(emailSendTo))?.ObjectId;
                            var empName = context.Employee.FirstOrDefault(x => x.EmployeeId == empId)?.EmployeeName;
                            if (string.IsNullOrEmpty(empName))
                            {
                                result = result.Replace(accName, "");
                            }
                            else
                            {
                                result = result.Replace(accName, empName);
                            }
                        }
                    }

                    if (result.Contains(accCode))
                    {
                        if (emailSendTo != null)
                        {
                            var empId = context.Contact.FirstOrDefault(x => x.Email.Equals(emailSendTo))?.ObjectId;
                            var empCode = context.Employee.FirstOrDefault(x => x.EmployeeId == empId)?.EmployeeCode;
                            if (string.IsNullOrEmpty(empCode))
                            {
                                result = result.Replace(accCode, "");
                            }
                            else
                            {
                                result = result.Replace(accCode, empCode);
                            }
                        }
                    }

                    if (note != null)
                    {
                        var newNote = note as Note;

                        if (result.Contains(CommentEmployeeName))
                        {
                            var commentUser = context.User.FirstOrDefault(x => x.UserId == newNote.CreatedById);
                            if (commentUser != null)
                            {
                                var commentEmployee =
                                    context.Employee.FirstOrDefault(x => x.EmployeeId == commentUser.EmployeeId);
                                var _commentEmployeeName = commentEmployee?.EmployeeName;

                                if (!String.IsNullOrEmpty(_commentEmployeeName))
                                {
                                    result = result.Replace(CommentEmployeeName, _commentEmployeeName);
                                }
                                else
                                {
                                    result = result.Replace(CommentEmployeeName, "");
                                }
                            }
                        }

                        if (result.Contains(CommentEmployeeCode))
                        {
                            var commentUser = context.User.FirstOrDefault(x => x.UserId == newNote.CreatedById);
                            if (commentUser != null)
                            {
                                var commentEmployee =
                                    context.Employee.FirstOrDefault(x => x.EmployeeId == commentUser.EmployeeId);
                                var _commentEmployeeCode = commentEmployee?.EmployeeCode;

                                if (!String.IsNullOrEmpty(_commentEmployeeCode))
                                {
                                    result = result.Replace(CommentEmployeeCode, _commentEmployeeCode);
                                }
                                else
                                {
                                    result = result.Replace(CommentEmployeeCode, "");
                                }
                            }
                        }

                        if (result.Contains(CommentCreatedDate))
                        {
                            result = result.Replace(CommentCreatedDate, FormatDateToString(newNote.CreatedDate));
                        }

                        if (result.Contains(CommentContent))
                        {
                            var _commentContent = newNote.Description?.Trim();
                            if (!String.IsNullOrEmpty(_commentContent))
                            {
                                result = result.Replace(CommentContent, _commentContent);
                            }
                            else
                            {
                                result = result.Replace(CommentContent, "");
                            }
                        }

                        if (result.Contains(Description))
                        {
                            var _description = newNote.Description?.Trim();
                            if (!String.IsNullOrEmpty(_description))
                            {
                                result = result.Replace(Description, _description);
                            }
                            else
                            {
                                result = result.Replace(Description, "");
                            }
                        }
                    }
                }
            }
            //Hạng Mục
            else if (typeModel == TypeModel.ProjectScope)
            {
                var _model = model as ProjectScope;

                if (_model != null)
                {
                    if (result.Contains(Logo))
                    {
                        var logo = configEntity.FirstOrDefault(w => w.SystemKey == "Logo").SystemValueString;

                        if (!String.IsNullOrEmpty(logo))
                        {
                            var temp_logo = "<img src=\"" + logo + "\" class=\"e - rte - image e - imginline\" alt=\"Logo TNM.png\" width=\"auto\" height=\"auto\" style=\"min - width: 0px; max - width: 750px; min - height: 0px; \">";
                            result = result.Replace(Logo, temp_logo);
                        }
                        else
                        {
                            result = result.Replace(Logo, "");
                        }
                    }

                    if (result.Contains(projectCode))
                    {
                        var code = context.Project.FirstOrDefault(x => x.ProjectId == _model.ProjectId)?.ProjectCode;
                        if (string.IsNullOrEmpty(code))
                        {
                            result = result.Replace(projectCode, "");
                        }
                        else
                        {
                            result = result.Replace(projectCode, code);
                        }
                    }

                    if (result.Contains(projectName))
                    {
                        var name = context.Project.FirstOrDefault(x => x.ProjectId == _model.ProjectId)?.ProjectName;
                        if (string.IsNullOrEmpty(name))
                        {
                            result = result.Replace(projectName, "");
                        }
                        else
                        {
                            result = result.Replace(projectName, name);
                        }
                    }

                    if (result.Contains(scopeCode) && _model.ProjectScopeCode != null)
                    {
                        result = result.Replace(scopeCode, _model.ProjectScopeCode);
                    }

                    if (result.Contains(scopeName))
                    {
                        if (string.IsNullOrEmpty(_model.ProjectScopeName))
                        {
                            result = result.Replace(scopeName, "");
                        }
                        else
                        {
                            result = result.Replace(scopeName, _model.ProjectScopeName);
                        }
                    }

                    if (result.Contains(scopeDescription))
                    {
                        if (string.IsNullOrEmpty(_model.Description))
                        {
                            result = result.Replace(scopeDescription, "");
                        }
                        else
                        {
                            result = result.Replace(scopeDescription, _model.Description);
                        }
                    }

                    if (result.Contains(EmployeeCode))
                    {
                        if (result.Contains(UpdatedDate))
                        {
                            var employeeId = context.User.FirstOrDefault(x => x.UserId == _model.UpdateBy)?.EmployeeId;
                            var employeeCode = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeId)?.EmployeeCode;

                            if (!String.IsNullOrEmpty(employeeCode))
                            {
                                result = result.Replace(EmployeeCode, employeeCode);
                            }
                            else
                            {
                                result = result.Replace(EmployeeCode, "");
                            }
                        }
                        else if (result.Contains(CreatedDate))
                        {
                            var employeeId = context.User.FirstOrDefault(x => x.UserId == _model.CreateBy)?.EmployeeId;
                            var employeeCode = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeId)?.EmployeeCode;

                            if (!String.IsNullOrEmpty(employeeCode))
                            {
                                result = result.Replace(EmployeeCode, employeeCode);
                            }
                            else
                            {
                                result = result.Replace(EmployeeCode, "");
                            }
                        }
                    }

                    if (result.Contains(EmployeeName))
                    {
                        if (result.Contains(UpdatedDate))
                        {
                            var employeeId = context.User.FirstOrDefault(x => x.UserId == _model.UpdateBy)?.EmployeeId;
                            var employeeName = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeId)?.EmployeeName;

                            if (!String.IsNullOrEmpty(employeeName))
                            {
                                result = result.Replace(EmployeeName, employeeName);
                            }
                            else
                            {
                                result = result.Replace(EmployeeName, "");
                            }
                        }
                        else if (result.Contains(CreatedDate))
                        {
                            var employeeId = context.User.FirstOrDefault(x => x.UserId == _model.CreateBy)?.EmployeeId;
                            var employeeName = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeId)?.EmployeeName;

                            if (!String.IsNullOrEmpty(employeeName))
                            {
                                result = result.Replace(EmployeeName, employeeName);
                            }
                            else
                            {
                                result = result.Replace(EmployeeName, "");
                            }
                        }
                    }

                    if (result.Contains(CreatedDate))
                    {
                        result = result.Replace(CreatedDate, FormatDateToString(_model.CreateDate));
                    }

                    if (result.Contains(UpdatedDate))
                    {
                        result = result.Replace(UpdatedDate, FormatDateToString(_model.UpdateDate));
                    }

                    if (result.Contains(Url_Login))
                    {
                        var Domain = configEntity.FirstOrDefault(w => w.SystemKey == "Domain").SystemValueString;
                        var loginLink = Domain + @"/";

                        if (!String.IsNullOrEmpty(loginLink))
                        {
                            result = result.Replace(Url_Login, loginLink);
                        }
                    }

                    if (result.Contains(projectType))
                    {

                        var typeId = context.Project.FirstOrDefault(x => x.ProjectId == _model.ProjectId)?.ProjectType;
                        var type = context.Category.FirstOrDefault(x => x.CategoryId == typeId)?.CategoryName;

                        if (!string.IsNullOrEmpty(type))
                        {
                            result = result.Replace(projectType, type);
                        }
                        else
                        {
                            result = result.Replace(projectType, "");
                        }
                    }

                    if (result.Contains(projectStatus))
                    {
                        var statusId = context.Project.FirstOrDefault(x => x.ProjectId == _model.ProjectId)?.ProjectStatus;
                        var stt = context.Category.FirstOrDefault(x => x.CategoryId == statusId)?.CategoryName;

                        if (!string.IsNullOrEmpty(stt))
                        {
                            result = result.Replace(projectStatus, stt);
                        }
                        else
                        {
                            result = result.Replace(projectStatus, "");
                        }
                    }

                    if (result.Contains(projectStartDate))
                    {
                        var startDate = context.Project.FirstOrDefault(x => x.ProjectId == _model.ProjectId)?.ProjectStartDate;
                        if (startDate != null)
                        {
                            result = result.Replace(projectStartDate, FormatDateToString(startDate));
                        }
                        else
                        {
                            result = result.Replace(projectStartDate, "");
                        }
                    }

                    if (result.Contains(projectEndDate))
                    {
                        var endDate = context.Project.FirstOrDefault(x => x.ProjectId == _model.ProjectId)?.ProjectEndDate;
                        if (endDate != null)
                        {
                            result = result.Replace(projectEndDate, FormatDateToString(endDate));
                        }
                        else
                        {
                            result = result.Replace(projectEndDate, "");
                        }
                    }

                    if (result.Contains(accName))
                    {
                        if (emailSendTo != null)
                        {
                            var empId = context.Contact.FirstOrDefault(x => x.Email.Equals(emailSendTo))?.ObjectId;
                            var empName = context.Employee.FirstOrDefault(x => x.EmployeeId == empId)?.EmployeeName;
                            if (string.IsNullOrEmpty(empName))
                            {
                                result = result.Replace(accName, "");
                            }
                            else
                            {
                                result = result.Replace(accName, empName);
                            }
                        }
                    }

                    if (result.Contains(accCode))
                    {
                        if (emailSendTo != null)
                        {
                            var empId = context.Contact.FirstOrDefault(x => x.Email.Equals(emailSendTo))?.ObjectId;
                            var empCode = context.Employee.FirstOrDefault(x => x.EmployeeId == empId)?.EmployeeCode;
                            if (string.IsNullOrEmpty(empCode))
                            {
                                result = result.Replace(accCode, "");
                            }
                            else
                            {
                                result = result.Replace(accCode, empCode);
                            }
                        }
                    }
                }
            }
            //Nguồn lực
            else if (typeModel == TypeModel.ProjectResource)
            {
                var _model = model as ProjectResource;

                if (_model != null)
                {
                    if (result.Contains(Logo))
                    {
                        var logo = configEntity.FirstOrDefault(w => w.SystemKey == "Logo").SystemValueString;

                        if (!String.IsNullOrEmpty(logo))
                        {
                            var temp_logo = "<img src=\"" + logo + "\" class=\"e - rte - image e - imginline\" alt=\"Logo TNM.png\" width=\"auto\" height=\"auto\" style=\"min - width: 0px; max - width: 750px; min - height: 0px; \">";
                            result = result.Replace(Logo, temp_logo);
                        }
                        else
                        {
                            result = result.Replace(Logo, "");
                        }
                    }

                    if (result.Contains(projectCode))
                    {
                        var code = context.Project.FirstOrDefault(x => x.ProjectId == _model.ProjectId)?.ProjectCode;
                        if (string.IsNullOrEmpty(code))
                        {
                            result = result.Replace(projectCode, "");
                        }
                        else
                        {
                            result = result.Replace(projectCode, code);
                        }
                    }

                    if (result.Contains(projectName))
                    {
                        var name = context.Project.FirstOrDefault(x => x.ProjectId == _model.ProjectId)?.ProjectName;
                        if (string.IsNullOrEmpty(name))
                        {
                            result = result.Replace(projectName, "");
                        }
                        else
                        {
                            result = result.Replace(projectName, name);
                        }
                    }

                    if (result.Contains(resourceCode))
                    {
                        var _code = context.Employee.FirstOrDefault(x => x.EmployeeId == _model.ObjectId)
                            ?.EmployeeCode;

                        if (!String.IsNullOrEmpty(_code))
                        {
                            result = result.Replace(resourceCode, _code);
                        }
                        else
                        {
                            result = result.Replace(resourceCode, "");
                        }
                    }

                    if (result.Contains(resourceName))
                    {
                        var _name = context.Employee.FirstOrDefault(x => x.EmployeeId == _model.ObjectId)
                            ?.EmployeeName;

                        if (!String.IsNullOrEmpty(_name))
                        {
                            result = result.Replace(resourceName, _name);
                        }
                        else
                        {
                            result = result.Replace(resourceName, "");
                        }
                    }

                    if (result.Contains(EmployeeCode))
                    {
                        if (result.Contains(UpdatedDate))
                        {
                            var employeeId = context.User.FirstOrDefault(x => x.UserId == _model.UpdateBy)?.EmployeeId;
                            var employeeCode = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeId)?.EmployeeCode;

                            if (!String.IsNullOrEmpty(employeeCode))
                            {
                                result = result.Replace(EmployeeCode, employeeCode);
                            }
                            else
                            {
                                result = result.Replace(EmployeeCode, "");
                            }
                        }
                        else if (result.Contains(CreatedDate))
                        {
                            var employeeId = context.User.FirstOrDefault(x => x.UserId == _model.CreateBy)?.EmployeeId;
                            var employeeCode = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeId)?.EmployeeCode;

                            if (!String.IsNullOrEmpty(employeeCode))
                            {
                                result = result.Replace(EmployeeCode, employeeCode);
                            }
                            else
                            {
                                result = result.Replace(EmployeeCode, "");
                            }
                        }

                    }

                    if (result.Contains(EmployeeName))
                    {
                        if (result.Contains(UpdatedDate))
                        {
                            var employeeId = context.User.FirstOrDefault(x => x.UserId == _model.UpdateBy)?.EmployeeId;
                            var employeeName = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeId)?.EmployeeName;

                            if (!String.IsNullOrEmpty(employeeName))
                            {
                                result = result.Replace(EmployeeName, employeeName);
                            }
                            else
                            {
                                result = result.Replace(EmployeeName, "");
                            }
                        }
                        else if (result.Contains(CreatedDate))
                        {
                            var employeeId = context.User.FirstOrDefault(x => x.UserId == _model.CreateBy)?.EmployeeId;
                            var employeeName = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeId)?.EmployeeName;

                            if (!String.IsNullOrEmpty(employeeName))
                            {
                                result = result.Replace(EmployeeName, employeeName);
                            }
                            else
                            {
                                result = result.Replace(EmployeeName, "");
                            }
                        }
                    }

                    if (result.Contains(CreatedDate))
                    {
                        result = result.Replace(CreatedDate, FormatDateToString(_model.CreateDate));
                    }

                    if (result.Contains(UpdatedDate))
                    {
                        result = result.Replace(UpdatedDate, FormatDateToString(_model.UpdateDate));
                    }

                    if (result.Contains(Url_Login))
                    {
                        var Domain = configEntity.FirstOrDefault(w => w.SystemKey == "Domain").SystemValueString;
                        var loginLink = Domain + @"/";

                        if (!String.IsNullOrEmpty(loginLink))
                        {
                            result = result.Replace(Url_Login, loginLink);
                        }
                    }

                    if (result.Contains(projectType))
                    {

                        var typeId = context.Project.FirstOrDefault(x => x.ProjectId == _model.ProjectId)?.ProjectType;
                        var type = context.Category.FirstOrDefault(x => x.CategoryId == typeId)?.CategoryName;

                        if (!string.IsNullOrEmpty(type))
                        {
                            result = result.Replace(projectType, type);
                        }
                        else
                        {
                            result = result.Replace(projectType, "");
                        }
                    }

                    if (result.Contains(projectStatus))
                    {
                        var statusId = context.Project.FirstOrDefault(x => x.ProjectId == _model.ProjectId)?.ProjectStatus;
                        var stt = context.Category.FirstOrDefault(x => x.CategoryId == statusId)?.CategoryName;

                        if (!string.IsNullOrEmpty(stt))
                        {
                            result = result.Replace(projectStatus, stt);
                        }
                        else
                        {
                            result = result.Replace(projectStatus, "");
                        }
                    }

                    if (result.Contains(projectStartDate))
                    {
                        var startDate = context.Project.FirstOrDefault(x => x.ProjectId == _model.ProjectId)?.ProjectStartDate;
                        if (startDate != null)
                        {
                            result = result.Replace(projectStartDate, FormatDateToString(startDate));
                        }
                        else
                        {
                            result = result.Replace(projectStartDate, "");
                        }
                    }

                    if (result.Contains(projectEndDate))
                    {
                        var endDate = context.Project.FirstOrDefault(x => x.ProjectId == _model.ProjectId)?.ProjectEndDate;
                        if (endDate != null)
                        {
                            result = result.Replace(projectEndDate, FormatDateToString(endDate));
                        }
                        else
                        {
                            result = result.Replace(projectEndDate, "");
                        }
                    }

                    if (result.Contains(accName))
                    {
                        if (emailSendTo != null)
                        {
                            var empId = context.Contact.FirstOrDefault(x => x.Email.Equals(emailSendTo))?.ObjectId;
                            var empName = context.Employee.FirstOrDefault(x => x.EmployeeId == empId)?.EmployeeName;
                            if (string.IsNullOrEmpty(empName))
                            {
                                result = result.Replace(accName, "");
                            }
                            else
                            {
                                result = result.Replace(accName, empName);
                            }
                        }
                    }

                    if (result.Contains(accCode))
                    {
                        if (emailSendTo != null)
                        {
                            var empId = context.Contact.FirstOrDefault(x => x.Email.Equals(emailSendTo))?.ObjectId;
                            var empCode = context.Employee.FirstOrDefault(x => x.EmployeeId == empId)?.EmployeeCode;
                            if (string.IsNullOrEmpty(empCode))
                            {
                                result = result.Replace(accCode, "");
                            }
                            else
                            {
                                result = result.Replace(accCode, empCode);
                            }
                        }
                    }
                }
            }
            //Tạo phiếu thu
            else if (typeModel == TypeModel.CashReceipts)
            {
                var _model = model as ReceiptInvoice;

                if (_model != null)
                {
                    List<Guid> listOrderId = new List<Guid>();
                    List<ReceiptOrderHistory> listOrderInReceiptOrderHistory = new List<ReceiptOrderHistory>();
                    decimal totalAmountReceivable = 0;
                    List<ReceiptInvoiceOrderModel> listReceiptInvoiceOrderModel = new List<ReceiptInvoiceOrderModel>();
                    var customerId = context.ReceiptInvoiceMapping
                        .FirstOrDefault(x => x.ReceiptInvoiceId == _model.ReceiptInvoiceId)?.ObjectId;


                    var statusInprocess = context.OrderStatus.FirstOrDefault(x => x.OrderStatusCode == "IP").OrderStatusId; //Đang xử lý
                    var statusWasSend = context.OrderStatus.FirstOrDefault(x => x.OrderStatusCode == "DLV").OrderStatusId; //Đã giao hàng
                    var statusComplete = context.OrderStatus.FirstOrDefault(x => x.OrderStatusCode == "COMP").OrderStatusId; //Đóng

                    //Lấy danh sách đơn hàng theo khách hàng
                    var listOrder = context.CustomerOrder.Where(x => (x.StatusId == statusInprocess || x.StatusId == statusWasSend || x.StatusId == statusComplete) &&
                                                                     x.CustomerId == customerId)
                        .OrderBy(y => y.OrderDate)
                        .ToList();

                    // if (parameter.OrderId != null)
                    // {
                    //     listOrder = listOrder.Where(x => x.OrderId == parameter.OrderId).ToList();
                    // }

                    if (listOrder.Count > 0)
                    {
                        listOrder.ForEach(item =>
                        {
                            if (item.OrderId != null && item.OrderId != Guid.Empty)
                                listOrderId.Add(item.OrderId);
                        });
                    }

                    if (listOrderId.Count > 0)
                    {
                        listOrderInReceiptOrderHistory = context.ReceiptOrderHistory.Where(x => listOrderId.Contains(x.OrderId)).ToList();
                        //Lấy danh sách đơn hàng đã thu tiền
                        var new_list = listOrderInReceiptOrderHistory.GroupBy(x => new { x.OrderId }).Select(y => new
                        {
                            Id = y.Key,
                            y.Key.OrderId,
                            TotalAmountCollected = y.Sum(s => s.AmountCollected)
                        }).ToList();

                        if (new_list.Count > 0)
                        {
                            listOrder.ForEach(item =>
                            {
                                var order = new_list.FirstOrDefault(x => x.OrderId == item.OrderId);
                                var totalOrder = CalculatorTotalPurchaseProduct(item.Amount.Value, item.DiscountType.Value, item.DiscountValue.Value);
                                if (order != null)
                                {
                                    //Lấy Đơn hàng chưa được thanh toán hết (Số tiền đã thanh toán < Số tiền của đơn hàng)
                                    if (order.TotalAmountCollected < totalOrder)
                                    {
                                        ReceiptInvoiceOrderModel receiptInvoiceOrder = new ReceiptInvoiceOrderModel();
                                        receiptInvoiceOrder.OrderId = order.OrderId;
                                        receiptInvoiceOrder.OrderCode = item.OrderCode;
                                        receiptInvoiceOrder.AmountCollected = totalOrder - order.TotalAmountCollected;
                                        receiptInvoiceOrder.AmountReceivable = totalOrder - order.TotalAmountCollected;
                                        receiptInvoiceOrder.Total = totalOrder;
                                        receiptInvoiceOrder.OrderDate = item.OrderDate;

                                        listReceiptInvoiceOrderModel.Add(receiptInvoiceOrder);
                                    };
                                }
                                else
                                {
                                    //Nếu đơn hàng chưa được thanh toán lần nào
                                    ReceiptInvoiceOrderModel receiptInvoiceOrder = new ReceiptInvoiceOrderModel();
                                    receiptInvoiceOrder.OrderId = item.OrderId;
                                    receiptInvoiceOrder.OrderCode = item.OrderCode;
                                    receiptInvoiceOrder.AmountCollected = totalOrder;
                                    receiptInvoiceOrder.AmountReceivable = totalOrder;
                                    receiptInvoiceOrder.Total = totalOrder;
                                    receiptInvoiceOrder.OrderDate = item.OrderDate;

                                    listReceiptInvoiceOrderModel.Add(receiptInvoiceOrder);
                                }
                            });
                        }
                        else
                        {
                            //Nếu chưa có đơn hàng nào được thanh toán
                            listOrder.ForEach(item =>
                            {
                                var totalOrder = CalculatorTotalPurchaseProduct(item.Amount.Value, item.DiscountType.Value, item.DiscountValue.Value);
                                ReceiptInvoiceOrderModel receiptInvoiceOrder = new ReceiptInvoiceOrderModel();
                                receiptInvoiceOrder.OrderId = item.OrderId;
                                receiptInvoiceOrder.OrderCode = item.OrderCode;
                                receiptInvoiceOrder.AmountCollected = totalOrder;
                                receiptInvoiceOrder.AmountReceivable = totalOrder;
                                receiptInvoiceOrder.Total = totalOrder;
                                receiptInvoiceOrder.OrderDate = item.OrderDate;

                                listReceiptInvoiceOrderModel.Add(receiptInvoiceOrder);
                            });
                        }

                        totalAmountReceivable = listReceiptInvoiceOrderModel.Sum(x => x.AmountReceivable);
                    }

                    if (result.Contains(Logo))
                    {
                        var logo = configEntity.FirstOrDefault(w => w.SystemKey == "Logo").SystemValueString;

                        if (!String.IsNullOrEmpty(logo))
                        {
                            var temp_logo = "<img src=\"" + logo + "\" class=\"e - rte - image e - imginline\" alt=\"Logo TNM.png\" width=\"auto\" height=\"auto\" style=\"min - width: 0px; max - width: 750px; min - height: 0px; \">";
                            result = result.Replace(Logo, temp_logo);
                        }
                        else
                        {
                            result = result.Replace(Logo, "");
                        }
                    }

                    if (result.Contains(CustomerName))
                    {
                        var customerName = context.Customer.FirstOrDefault(x => x.CustomerId == customerId)?.CustomerName;

                        result = result.Replace(CustomerName, !string.IsNullOrEmpty(customerName) ? customerName : "");
                    }

                    if (result.Contains(companyName))
                    {
                        var _companyName = context.Contact
                            .FirstOrDefault(x => x.ObjectId == customerId && x.ObjectType == "CUS")?.CompanyName;

                        result = result.Replace(companyName, !string.IsNullOrEmpty(_companyName) ? _companyName : "");
                    }

                    if (result.Contains(amountPay))
                    {
                        var amount = totalAmountReceivable + _model.Amount;
                        result = result.Replace(amountPay, amount.Value.ToString("C2", new CultureInfo("vi-VN")));
                    }

                    if (result.Contains(totalAmountPay) && _model.Amount != null)
                    {
                        result = result.Replace(totalAmountPay, _model.Amount != null ? _model.Amount.Value.ToString("C2", new CultureInfo("vi-VN")) : "0");
                    }

                    if (result.Contains(remainBalance))
                    {
                        if (_model.Amount == null)
                        {
                            _model.Amount = 0;
                        }
                        result = result.Replace(remainBalance, totalAmountReceivable.ToString("C2", new CultureInfo("vi-VN")));
                    }

                    if (result.Contains(orderCode))
                    {
                        listReceiptInvoiceOrderModel.ForEach(item =>
                        {
                            result = result.Replace(orderCode, item.OrderCode ?? "");
                        });
                    }

                    if (result.Contains(orderDate))
                    {
                        listReceiptInvoiceOrderModel.ForEach(item =>
                        {
                            result = result.Replace(orderDate, item.OrderDate.ToString("dd/MM/yyyy"));
                        });
                    }

                    if (result.Contains(invoiceCode))
                    {
                        result = result.Replace(invoiceCode, _model.ReceiptInvoiceCode);
                    }
                }
            }
            //Báo có
            else if (typeModel == TypeModel.BankReceipts)
            {
                var _model = model as BankReceiptInvoice;

                if (_model != null)
                {
                    List<Guid> listOrderId = new List<Guid>();
                    List<ReceiptOrderHistory> listOrderInReceiptOrderHistory = new List<ReceiptOrderHistory>();
                    decimal totalAmountReceivable = 0;
                    List<ReceiptInvoiceOrderModel> listReceiptInvoiceOrderModel = new List<ReceiptInvoiceOrderModel>();
                    var customerId = context.BankReceiptInvoiceMapping
                        .FirstOrDefault(x => x.BankReceiptInvoiceId == _model.BankReceiptInvoiceId)?.ObjectId;


                    var statusInprocess = context.OrderStatus.FirstOrDefault(x => x.OrderStatusCode == "IP").OrderStatusId; //Đang xử lý
                    var statusWasSend = context.OrderStatus.FirstOrDefault(x => x.OrderStatusCode == "DLV").OrderStatusId; //Đã giao hàng
                    var statusComplete = context.OrderStatus.FirstOrDefault(x => x.OrderStatusCode == "COMP").OrderStatusId; //Đóng

                    //Lấy danh sách đơn hàng theo khách hàng
                    var listOrder = context.CustomerOrder.Where(x => (x.StatusId == statusInprocess || x.StatusId == statusWasSend || x.StatusId == statusComplete) &&
                                                                     x.CustomerId == customerId)
                        .OrderBy(y => y.OrderDate)
                        .ToList();

                    // if (parameter.OrderId != null)
                    // {
                    //     listOrder = listOrder.Where(x => x.OrderId == parameter.OrderId).ToList();
                    // }

                    if (listOrder.Count > 0)
                    {
                        listOrder.ForEach(item =>
                        {
                            if (item.OrderId != null && item.OrderId != Guid.Empty)
                                listOrderId.Add(item.OrderId);
                        });
                    }

                    if (listOrderId.Count > 0)
                    {
                        listOrderInReceiptOrderHistory = context.ReceiptOrderHistory.Where(x => listOrderId.Contains(x.OrderId)).ToList();
                        //Lấy danh sách đơn hàng đã thu tiền
                        var new_list = listOrderInReceiptOrderHistory.GroupBy(x => new { x.OrderId }).Select(y => new
                        {
                            Id = y.Key,
                            y.Key.OrderId,
                            TotalAmountCollected = y.Sum(s => s.AmountCollected)
                        }).ToList();

                        if (new_list.Count > 0)
                        {
                            listOrder.ForEach(item =>
                            {
                                var order = new_list.FirstOrDefault(x => x.OrderId == item.OrderId);
                                var totalOrder = CalculatorTotalPurchaseProduct(item.Amount.Value, item.DiscountType.Value, item.DiscountValue.Value);
                                if (order != null)
                                {
                                    //Lấy Đơn hàng chưa được thanh toán hết (Số tiền đã thanh toán < Số tiền của đơn hàng)
                                    if (order.TotalAmountCollected < totalOrder)
                                    {
                                        ReceiptInvoiceOrderModel receiptInvoiceOrder = new ReceiptInvoiceOrderModel();
                                        receiptInvoiceOrder.OrderId = order.OrderId;
                                        receiptInvoiceOrder.OrderCode = item.OrderCode;
                                        receiptInvoiceOrder.AmountCollected = totalOrder - order.TotalAmountCollected;
                                        receiptInvoiceOrder.AmountReceivable = totalOrder - order.TotalAmountCollected;
                                        receiptInvoiceOrder.Total = totalOrder;
                                        receiptInvoiceOrder.OrderDate = item.OrderDate;

                                        listReceiptInvoiceOrderModel.Add(receiptInvoiceOrder);
                                    };
                                }
                                else
                                {
                                    //Nếu đơn hàng chưa được thanh toán lần nào
                                    ReceiptInvoiceOrderModel receiptInvoiceOrder = new ReceiptInvoiceOrderModel();
                                    receiptInvoiceOrder.OrderId = item.OrderId;
                                    receiptInvoiceOrder.OrderCode = item.OrderCode;
                                    receiptInvoiceOrder.AmountCollected = totalOrder;
                                    receiptInvoiceOrder.AmountReceivable = totalOrder;
                                    receiptInvoiceOrder.Total = totalOrder;
                                    receiptInvoiceOrder.OrderDate = item.OrderDate;

                                    listReceiptInvoiceOrderModel.Add(receiptInvoiceOrder);
                                }
                            });
                        }
                        else
                        {
                            //Nếu chưa có đơn hàng nào được thanh toán
                            listOrder.ForEach(item =>
                            {
                                var totalOrder = CalculatorTotalPurchaseProduct(item.Amount.Value, item.DiscountType.Value, item.DiscountValue.Value);
                                ReceiptInvoiceOrderModel receiptInvoiceOrder = new ReceiptInvoiceOrderModel();
                                receiptInvoiceOrder.OrderId = item.OrderId;
                                receiptInvoiceOrder.OrderCode = item.OrderCode;
                                receiptInvoiceOrder.AmountCollected = totalOrder;
                                receiptInvoiceOrder.AmountReceivable = totalOrder;
                                receiptInvoiceOrder.Total = totalOrder;
                                receiptInvoiceOrder.OrderDate = item.OrderDate;

                                listReceiptInvoiceOrderModel.Add(receiptInvoiceOrder);
                            });
                        }

                        totalAmountReceivable = listReceiptInvoiceOrderModel.Sum(x => x.AmountReceivable);
                    }

                    if (result.Contains(Logo))
                    {
                        var logo = configEntity.FirstOrDefault(w => w.SystemKey == "Logo").SystemValueString;

                        if (!String.IsNullOrEmpty(logo))
                        {
                            var temp_logo = "<img src=\"" + logo + "\" class=\"e - rte - image e - imginline\" alt=\"Logo TNM.png\" width=\"auto\" height=\"auto\" style=\"min - width: 0px; max - width: 750px; min - height: 0px; \">";
                            result = result.Replace(Logo, temp_logo);
                        }
                        else
                        {
                            result = result.Replace(Logo, "");
                        }
                    }

                    if (result.Contains(CustomerName))
                    {
                        var customerName = context.Customer.FirstOrDefault(x => x.CustomerId == customerId)?.CustomerName;

                        result = result.Replace(CustomerName, !string.IsNullOrEmpty(customerName) ? customerName : "");
                    }

                    if (result.Contains(companyName))
                    {
                        var _companyName = context.Contact
                            .FirstOrDefault(x => x.ObjectId == customerId && x.ObjectType == "CUS")?.CompanyName;

                        result = result.Replace(companyName, !string.IsNullOrEmpty(_companyName) ? _companyName : "");
                    }

                    if (result.Contains(amountPay))
                    {
                        var amount = totalAmountReceivable + _model.BankReceiptInvoiceAmount;
                        result = result.Replace(amountPay, amount.Value.ToString("C2", new CultureInfo("vi-VN")));
                    }

                    if (result.Contains(totalAmountPay) && _model.BankReceiptInvoiceAmount != null)
                    {
                        result = result.Replace(totalAmountPay, _model.BankReceiptInvoiceAmount != null ? _model.BankReceiptInvoiceAmount.Value.ToString("C2", new CultureInfo("vi-VN")) : "0");
                    }

                    if (result.Contains(remainBalance))
                    {
                        if (_model.BankReceiptInvoiceAmount == null)
                        {
                            _model.BankReceiptInvoiceAmount = 0;
                        }
                        result = result.Replace(remainBalance, totalAmountReceivable.ToString("C2", new CultureInfo("vi-VN")));
                    }

                    if (result.Contains(orderCode))
                    {
                        listReceiptInvoiceOrderModel.ForEach(item =>
                        {
                            result = result.Replace(orderCode, item.OrderCode ?? "");
                        });
                    }

                    if (result.Contains(orderDate))
                    {
                        listReceiptInvoiceOrderModel.ForEach(item =>
                        {
                            result = result.Replace(orderDate, item.OrderDate.ToString("dd/MM/yyyy"));
                        });
                    }

                    if (result.Contains(invoiceCode))
                    {
                        result = result.Replace(invoiceCode, _model.BankReceiptInvoiceCode);
                    }
                }
            }
            // TẠO NHÂN VIÊN
            else if (typeModel == TypeModel.Employee)
            {
                var _model = model as Employee;

                if (_model != null)
                {
                    if (result.Contains(Logo))
                    {
                        var logo = configEntity.FirstOrDefault(w => w.SystemKey == "Logo").SystemValueString;

                        if (!String.IsNullOrEmpty(logo))
                        {
                            var temp_logo = "<img src=\"" + logo + "\" class=\"e - rte - image e - imginline\" alt=\"Logo TNM.png\" width=\"auto\" height=\"auto\" style=\"min - width: 0px; max - width: 750px; min - height: 0px; \">";
                            result = result.Replace(Logo, temp_logo);
                        }
                        else
                        {
                            result = result.Replace(Logo, "");
                        }
                    }

                    if (result.Contains(EmployeeName) && _model.EmployeeName != null)
                    {
                        result = result.Replace(EmployeeName, _model.EmployeeName);
                    }

                    if (result.Contains(Username))
                    {
                        var _userName = context.User.FirstOrDefault(x => x.EmployeeId == _model.EmployeeId)?.UserName;

                        if (!String.IsNullOrEmpty(_userName))
                        {
                            result = result.Replace(Username, _userName);
                        }
                        else
                        {
                            result = result.Replace(Username, "");
                        }
                    }

                    if (result.Contains(Password))
                    {
                        var _password = context.SystemParameter.FirstOrDefault(x => x.SystemKey == "DefaultUserPassword")?.SystemValueString;

                        if (!String.IsNullOrEmpty(_password))
                        {
                            result = result.Replace(Password, _password);
                        }
                        else
                        {
                            result = result.Replace(Password, "");
                        }
                    }

                    if (result.Contains(Url_Login))
                    {
                        var Domain = configEntity.FirstOrDefault(w => w.SystemKey == "Domain").SystemValueString;
                        var loginLink = Domain + @"/";

                        if (!String.IsNullOrEmpty(loginLink))
                        {
                            result = result.Replace(Url_Login, loginLink);
                        }
                    }

                    if (result.Contains(CompanyName))
                    {
                        var company_name = context.CompanyConfiguration.FirstOrDefault().CompanyName;

                        if (!String.IsNullOrEmpty(company_name))
                        {
                            result = result.Replace(CompanyName, company_name);
                        }
                    }

                    if (result.Contains(EmployeeCode) && _model.EmployeeCode != null)
                    {
                        result = result.Replace(EmployeeCode, _model.EmployeeCode);
                    }

                    if (result.Contains(PhongBan))
                    {
                        var _phongBan = context.Organization.FirstOrDefault(o => o.OrganizationId == _model.OrganizationId)?.OrganizationName; ;

                        if (!String.IsNullOrEmpty(_phongBan))
                        {
                            result = result.Replace(PhongBan, _phongBan);
                        }
                        else
                        {
                            result = result.Replace(PhongBan, "");
                        }
                    }
                }
            }
            // Đặt lại mật khẩu
            else if (typeModel == TypeModel.EmployeeDetail && actionCode == "RESET")
            {
                var _model = model as User;

                if (_model != null)
                {
                    if (result.Contains(Logo))
                    {
                        var logo = configEntity.FirstOrDefault(w => w.SystemKey == "Logo").SystemValueString;

                        if (!String.IsNullOrEmpty(logo))
                        {
                            var temp_logo = "<img src=\"" + logo + "\" class=\"e - rte - image e - imginline\" alt=\"Logo TNM.png\" width=\"auto\" height=\"auto\" style=\"min - width: 0px; max - width: 750px; min - height: 0px; \">";
                            result = result.Replace(Logo, temp_logo);
                        }
                        else
                        {
                            result = result.Replace(Logo, "");
                        }
                    }

                    if (result.Contains(EmployeeName))
                    {
                        var empName = context.Employee.FirstOrDefault(x => x.EmployeeId == _model.EmployeeId)?.EmployeeName;

                        if (!string.IsNullOrEmpty(empName))
                        {
                            result = result.Replace(EmployeeName, empName);
                        }
                        else
                        {
                            result = result.Replace(EmployeeName, "");
                        }
                    }

                    if (result.Contains(Username))
                    {
                        if (!String.IsNullOrEmpty(_model.UserName))
                        {
                            result = result.Replace(Username, _model.UserName);
                        }
                        else
                        {
                            result = result.Replace(Username, "");
                        }
                    }

                    if (result.Contains(Password))
                    {
                        var _password = context.SystemParameter.FirstOrDefault(x => x.SystemKey == "DefaultUserPassword")?.SystemValueString;

                        if (!String.IsNullOrEmpty(_password))
                        {
                            result = result.Replace(Password, _password);
                        }
                        else
                        {
                            result = result.Replace(Password, "");
                        }
                    }

                    if (result.Contains(Url_Login))
                    {
                        var Domain = configEntity.FirstOrDefault(w => w.SystemKey == "Domain").SystemValueString;
                        var loginLink = Domain + @"/";

                        if (!String.IsNullOrEmpty(loginLink))
                        {
                            result = result.Replace(Url_Login, loginLink);
                        }
                    }
                }
            }
            // Quên mật khẩu
            else if (typeModel == TypeModel.EmployeeDetail && actionCode == "FORGOT")
            {
                var _model = model as User;

                if (_model != null)
                {
                    if (result.Contains(Logo))
                    {
                        var logo = configEntity.FirstOrDefault(w => w.SystemKey == "Logo").SystemValueString;

                        if (!String.IsNullOrEmpty(logo))
                        {
                            var temp_logo = "<img src=\"" + logo +
                                            "\" class=\"e - rte - image e - imginline\" alt=\"Logo TNM.png\" width=\"auto\" height=\"auto\" style=\"min - width: 0px; max - width: 750px; min - height: 0px; \">";
                            result = result.Replace(Logo, temp_logo);
                        }
                        else
                        {
                            result = result.Replace(Logo, "");
                        }
                    }

                    if (result.Contains(Username) && _model.UserName != null)
                    {
                        result = result.Replace(Username, _model.UserName);
                    }

                    if (result.Contains(EmployeeName))
                    {
                        var employeeId = context.User.FirstOrDefault(x => x.UserId == _model.UserId)?.EmployeeId;
                        var employeeName = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeId)?.EmployeeName;

                        if (!String.IsNullOrEmpty(employeeName))
                        {
                            result = result.Replace(EmployeeName, employeeName);
                        }
                        else
                        {
                            result = result.Replace(EmployeeName, "");
                        }
                    }

                    if (result.Contains(MaXacThuc))
                    {
                        Random r = new Random();
                        var maXacThuc = r.Next(100000, 999999);
                        User user = new User();

                        user = context.User.FirstOrDefault(x => x.UserId == _model.UserId);
                        user.MaXacThuc = maXacThuc;
                        context.User.Update(user);
                        context.SaveChanges();
                        result = result.Replace(MaXacThuc, maXacThuc.ToString());
                    }


                    if (result.Contains(Url_Login))
                    {
                        var Domain = configEntity.FirstOrDefault(w => w.SystemKey == "Domain").SystemValueString;
                        var loginLink = Domain + "/forgot-pass/change/" + _model.ResetCode;

                        if (!String.IsNullOrEmpty(loginLink))
                        {
                            result = result.Replace(Url_Login, loginLink);
                        }
                    }
                }
            }
            // Chi tiết đề xuất xin nghỉ
            else if (typeModel == TypeModel.RequestDetail)
            {
                var _model = model as DeXuatXinNghi;

                if (_model != null)
                {
                    if (result.Contains(Logo))
                    {
                        var logo = configEntity.FirstOrDefault(w => w.SystemKey == "Logo").SystemValueString;

                        if (!String.IsNullOrEmpty(logo))
                        {
                            var temp_logo = "<img src=\"" + logo +
                                            "\" class=\"e - rte - image e - imginline\" alt=\"Logo TNM.png\" width=\"auto\" height=\"auto\" style=\"min - width: 0px; max - width: 750px; min - height: 0px; \">";
                            result = result.Replace(Logo, temp_logo);
                        }
                        else
                        {
                            result = result.Replace(Logo, "");
                        }
                    }

                    if (result.Contains(OfferEmployeeCode))
                    {
                        var empCode = context.Employee.FirstOrDefault(x => x.EmployeeId == _model.EmployeeId)?.EmployeeCode;

                        if (!string.IsNullOrEmpty(empCode))
                        {
                            result = result.Replace(OfferEmployeeCode, empCode);
                        }
                        else
                        {
                            result = result.Replace(OfferEmployeeCode, "");
                        }
                    }

                    if (result.Contains(OfferEmployeeName))
                    {
                        var empName = context.Employee.FirstOrDefault(x => x.EmployeeId == _model.EmployeeId)?.EmployeeName;

                        if (!string.IsNullOrEmpty(empName))
                        {
                            result = result.Replace(OfferEmployeeName, empName);
                        }
                        else
                        {
                            result = result.Replace(OfferEmployeeName, "");
                        }
                    }

                    if (result.Contains(RequestCode))
                    {
                        if (!string.IsNullOrEmpty(_model.Code))
                        {
                            result = result.Replace(RequestCode, _model.Code);
                        }
                        else
                        {
                            result = result.Replace(RequestCode, "");
                        }
                    }

                    if (result.Contains(Dxxn_LyDoTuChoi))
                    {
                        if (!string.IsNullOrEmpty(_model.LyDoTuChoi))
                        {
                            result = result.Replace(Dxxn_LyDoTuChoi, _model.LyDoTuChoi);
                        }
                        else
                        {
                            result = result.Replace(Dxxn_LyDoTuChoi, "");
                        }
                    }

                    if (result.Contains(CreatedDate))
                    {
                        var createdDate = FormatDateToString(_model.CreatedDate);

                        if (!string.IsNullOrEmpty(createdDate))
                        {
                            result = result.Replace(CreatedDate, createdDate);
                        }
                        else
                        {
                            result = result.Replace(CreatedDate, "");
                        }
                    }

                    if (result.Contains(TypeRequestName))
                    {
                        var _typeRequestName = GeneralList.GetTrangThais("KyHieuChamCong")
                            .FirstOrDefault(x => x.Value == _model.LoaiDeXuatId)?.Name;

                        if (!string.IsNullOrEmpty(_typeRequestName))
                        {
                            result = result.Replace(TypeRequestName, _typeRequestName);
                        }
                        else
                        {
                            result = result.Replace(TypeRequestName, "");
                        }
                    }

                    if (result.Contains(DurationTime))
                    {
                        var listDeXuatXinNghiChiTiet = context.DeXuatXinNghiChiTiet
                            .Where(x => x.DeXuatXinNghiId == _model.DeXuatXinNghiId).ToList();

                        decimal TongNgayNghi = 0;

                        //Nếu loại đề xuất là Đi muộn hoặc Về sớm
                        if (_model.LoaiDeXuatId == 12 || _model.LoaiDeXuatId == 13)
                        {
                            TongNgayNghi = listDeXuatXinNghiChiTiet.Count;
                        }
                        else
                        {
                            TongNgayNghi = (decimal)listDeXuatXinNghiChiTiet.Count / 2;
                        }

                        result = result.Replace(DurationTime, TongNgayNghi.ToString());
                    }

                    if (result.Contains(Detail))
                    {
                        if (!string.IsNullOrEmpty(_model.LyDo))
                        {
                            result = result.Replace(Detail, _model.LyDo);
                        }
                        else
                        {
                            result = result.Replace(Detail, "");
                        }
                    }

                    if (result.Contains(Url_Login))
                    {
                        var Domain = configEntity.FirstOrDefault(w => w.SystemKey == "Domain").SystemValueString;
                        var loginLink = Domain + @"/employee/request/detail;deXuatXinNghiId=" + EncrDecrCrypto.Encrypt(_model.DeXuatXinNghiId.ToString());

                        if (!String.IsNullOrEmpty(loginLink))
                        {
                            result = result.Replace(Url_Login, loginLink);
                        }
                    }
                }
            }
            // Chi tiết đề xuất tăng lương
            else if (typeModel == TypeModel.DeXuatTangLuongDetail)
            {
                var _model = model as DeXuatTangLuong;

                if (_model != null)
                {
                    if (result.Contains(Logo))
                    {
                        var logo = configEntity.FirstOrDefault(w => w.SystemKey == "Logo").SystemValueString;

                        if (!String.IsNullOrEmpty(logo))
                        {
                            var temp_logo = "<img src=\"" + logo +
                                            "\" class=\"e - rte - image e - imginline\" alt=\"Logo TNM.png\" width=\"auto\" height=\"auto\" style=\"min - width: 0px; max - width: 750px; min - height: 0px; \">";
                            result = result.Replace(Logo, temp_logo);
                        }
                        else
                        {
                            result = result.Replace(Logo, "");
                        }
                    }
                    if (result.Contains(EmployeeCode))
                    {
                        var empName = context.Employee.FirstOrDefault(x => x.EmployeeId == _model.NguoiDeXuatId)?.EmployeeName;

                        if (!string.IsNullOrEmpty(empName))
                        {
                            result = result.Replace(EmployeeCode, empName);
                        }
                        else
                        {
                            result = result.Replace(EmployeeCode, "");
                        }
                    }
                    if (result.Contains(EmployeeName))
                    {
                        var empName = context.Employee.FirstOrDefault(x => x.EmployeeId == _model.NguoiDeXuatId)?.EmployeeCode;

                        if (!string.IsNullOrEmpty(empName))
                        {
                            result = result.Replace(EmployeeName, empName);
                        }
                        else
                        {
                            result = result.Replace(EmployeeName, "");
                        }
                    }
                    if (result.Contains(DeXuatTangLuong))
                    {
                        var TenDeXuat = _model.TenDeXuat;

                        if (!string.IsNullOrEmpty(TenDeXuat))
                        {
                            result = result.Replace(DeXuatTangLuong, TenDeXuat);
                        }
                        else
                        {
                            result = result.Replace(DeXuatTangLuong, "");
                        }
                    }

                    if (result.Contains(UpdatedDate))
                    {
                        var updatedDate = _model.UpdatedDate;
                        result = result.Replace(UpdatedDate, FormatDateToString(updatedDate));
                    }

                    if (result.Contains(Url_Login))
                    {
                        var Domain = configEntity.FirstOrDefault(w => w.SystemKey == "Domain").SystemValueString;
                        var loginLink = Domain + @"/employee/de-xuat-tang-luong-detail;deXuatTLId=" + EncrDecrCrypto.Encrypt(_model.DeXuatTangLuongId.ToString());

                        if (!String.IsNullOrEmpty(loginLink))
                        {
                            result = result.Replace(Url_Login, loginLink);
                        }
                    }
                }
            }
            // Chi tiết đề xuất chức vụ
            else if (typeModel == TypeModel.DeXuatChucVuDetail)
            {
                var _model = model as DeXuatThayDoiChucVu;

                if (_model != null)
                {
                    if (result.Contains(Logo))
                    {
                        var logo = configEntity.FirstOrDefault(w => w.SystemKey == "Logo").SystemValueString;

                        if (!String.IsNullOrEmpty(logo))
                        {
                            var temp_logo = "<img src=\"" + logo +
                                            "\" class=\"e - rte - image e - imginline\" alt=\"Logo TNM.png\" width=\"auto\" height=\"auto\" style=\"min - width: 0px; max - width: 750px; min - height: 0px; \">";
                            result = result.Replace(Logo, temp_logo);
                        }
                        else
                        {
                            result = result.Replace(Logo, "");
                        }
                    }
                    if (result.Contains(EmployeeCode))
                    {
                        var empName = context.Employee.FirstOrDefault(x => x.EmployeeId == _model.NguoiDeXuatId)?.EmployeeName;

                        if (!string.IsNullOrEmpty(empName))
                        {
                            result = result.Replace(EmployeeCode, empName);
                        }
                        else
                        {
                            result = result.Replace(EmployeeCode, "");
                        }
                    }


                    if (result.Contains(EmployeeName))
                    {
                        var empName = context.Employee.FirstOrDefault(x => x.EmployeeId == _model.NguoiDeXuatId)?.EmployeeCode;

                        if (!string.IsNullOrEmpty(empName))
                        {
                            result = result.Replace(EmployeeName, empName);
                        }
                        else
                        {
                            result = result.Replace(EmployeeName, "");
                        }
                    }

                    if (result.Contains(DeXuatChucVu))
                    {
                        var createCode = _model.TenDeXuat;

                        if (!string.IsNullOrEmpty(createCode))
                        {
                            result = result.Replace(DeXuatChucVu, createCode);
                        }
                        else
                        {
                            result = result.Replace(DeXuatChucVu, "");
                        }
                    }

                    if (result.Contains(UpdatedDate))
                    {
                        var updatedDate = _model.UpdatedDate;
                        result = result.Replace(UpdatedDate, FormatDateToString(updatedDate));
                    }


                    if (result.Contains(Url_Login))
                    {
                        var Domain = configEntity.FirstOrDefault(w => w.SystemKey == "Domain").SystemValueString;
                        var loginLink = Domain + @"/employee/de-xuat-chuc-vu-detail;deXuatTLId=" + EncrDecrCrypto.Encrypt(_model.DeXuatThayDoiChucVuId.ToString());

                        if (!String.IsNullOrEmpty(loginLink))
                        {
                            result = result.Replace(Url_Login, loginLink);
                        }
                    }
                }
            }
            // Chi tiết đề xuất kế hoạch OT
            else if (typeModel == TypeModel.DeXuatKeHoachOTDetail)
            {
                var _model = model as KeHoachOt;

                if (_model != null)
                {
                    if (result.Contains(Logo))
                    {
                        var logo = configEntity.FirstOrDefault(w => w.SystemKey == "Logo").SystemValueString;

                        if (!String.IsNullOrEmpty(logo))
                        {
                            var temp_logo = "<img src=\"" + logo +
                                            "\" class=\"e - rte - image e - imginline\" alt=\"Logo TNM.png\" width=\"auto\" height=\"auto\" style=\"min - width: 0px; max - width: 750px; min - height: 0px; \">";
                            result = result.Replace(Logo, temp_logo);
                        }
                        else
                        {
                            result = result.Replace(Logo, "");
                        }
                    }

                    if (result.Contains(EmployeeCode))
                    {
                        var empName = context.Employee.FirstOrDefault(x => x.EmployeeId == _model.NguoiDeXuatId)?.EmployeeName;

                        if (!string.IsNullOrEmpty(empName))
                        {
                            result = result.Replace(EmployeeCode, empName);
                        }
                        else
                        {
                            result = result.Replace(EmployeeCode, "");
                        }
                    }

                    if (result.Contains(EmployeeName))
                    {
                        var empName = context.Employee.FirstOrDefault(x => x.EmployeeId == _model.NguoiDeXuatId)?.EmployeeCode;

                        if (!string.IsNullOrEmpty(empName))
                        {
                            result = result.Replace(EmployeeName, empName);
                        }
                        else
                        {
                            result = result.Replace(EmployeeName, "");
                        }
                    }

                    if (result.Contains(KeHoachOt))
                    {
                        var createCode = _model.TenKeHoach;

                        if (!string.IsNullOrEmpty(createCode))
                        {
                            result = result.Replace(KeHoachOt, createCode);
                        }
                        else
                        {
                            result = result.Replace(KeHoachOt, "");
                        }
                    }

                    if (result.Contains(UpdatedDate))
                    {
                        var updatedDate = _model.UpdatedDate;
                        result = result.Replace(UpdatedDate, FormatDateToString(updatedDate));
                    }

                    if (result.Contains(Url_Login))
                    {
                        var Domain = configEntity.FirstOrDefault(w => w.SystemKey == "Domain").SystemValueString;
                        var loginLink = Domain + @"/employee/kehoach-ot-detail;deXuatOTId=" + EncrDecrCrypto.Encrypt(_model.KeHoachOtId.ToString());

                        if (!String.IsNullOrEmpty(loginLink))
                        {
                            result = result.Replace(Url_Login, loginLink);
                        }
                    }
                }
            }
            // Chi tiết đề xuất đăng ký OT
            else if (typeModel == TypeModel.DeXuatDangKyOTDetail)
            {
                var _model = model as KeHoachOt;

                if (_model != null)
                {
                    if (result.Contains(Logo))
                    {
                        var logo = configEntity.FirstOrDefault(w => w.SystemKey == "Logo").SystemValueString;

                        if (!String.IsNullOrEmpty(logo))
                        {
                            var temp_logo = "<img src=\"" + logo +
                                            "\" class=\"e - rte - image e - imginline\" alt=\"Logo TNM.png\" width=\"auto\" height=\"auto\" style=\"min - width: 0px; max - width: 750px; min - height: 0px; \">";
                            result = result.Replace(Logo, temp_logo);
                        }
                        else
                        {
                            result = result.Replace(Logo, "");
                        }
                    }

                    if (result.Contains(EmployeeCode))
                    {
                        var empName = context.Employee.FirstOrDefault(x => x.EmployeeId == _model.NguoiDeXuatId)?.EmployeeName;

                        if (!string.IsNullOrEmpty(empName))
                        {
                            result = result.Replace(EmployeeCode, empName);
                        }
                        else
                        {
                            result = result.Replace(EmployeeCode, "");
                        }
                    }

                    if (result.Contains(EmployeeName))
                    {
                        var empName = context.Employee.FirstOrDefault(x => x.EmployeeId == _model.NguoiDeXuatId)?.EmployeeCode;

                        if (!string.IsNullOrEmpty(empName))
                        {
                            result = result.Replace(EmployeeName, empName);
                        }
                        else
                        {
                            result = result.Replace(EmployeeName, "");
                        }
                    }

                    if (result.Contains(KeHoachOt))
                    {
                        var createCode = _model.TenKeHoach;

                        if (!string.IsNullOrEmpty(createCode))
                        {
                            result = result.Replace(KeHoachOt, createCode);
                        }
                        else
                        {
                            result = result.Replace(KeHoachOt, "");
                        }
                    }

                    if (result.Contains(UpdatedDate))
                    {
                        var updatedDate = _model.UpdatedDate;
                        result = result.Replace(UpdatedDate, FormatDateToString(updatedDate));
                    }

                    if (result.Contains(Url_Login))
                    {
                        var Domain = configEntity.FirstOrDefault(w => w.SystemKey == "Domain").SystemValueString;
                        var loginLink = Domain + @"/employee/kehoach-ot-detail;deXuatOTId=" + EncrDecrCrypto.Encrypt(_model.KeHoachOtId.ToString());

                        if (!String.IsNullOrEmpty(loginLink))
                        {
                            result = result.Replace(Url_Login, loginLink);
                        }
                    }
                }
            }
            // phỏng vấn
            else if (typeModel == TypeModel.CandidateInterview)
            {
                var _model = model as Candidate;

                if (_model != null)
                {
                    if (result.Contains(Logo))
                    {
                        var logo = configEntity.FirstOrDefault(w => w.SystemKey == "Logo").SystemValueString;

                        if (!String.IsNullOrEmpty(logo))
                        {
                            var temp_logo = "<img src=\"" + logo +
                                            "\" class=\"e - rte - image e - imginline\" alt=\"Logo TNM.png\" width=\"auto\" height=\"auto\" style=\"min - width: 0px; max - width: 750px; min - height: 0px; \">";
                            result = result.Replace(Logo, temp_logo);
                        }
                        else
                        {
                            result = result.Replace(Logo, "");
                        }
                    }
                    // Tên ứng viên
                    if (result.Contains(TenUngVien))
                    {
                        if (!string.IsNullOrEmpty(_model.FullName))
                        {
                            result = result.Replace(TenUngVien, _model.FullName);
                        }
                        else
                        {
                            result = result.Replace(TenUngVien, "");
                        }
                    }

                    var scheduleInter = context.InterviewSchedule.FirstOrDefault(x => x.CandidateId == _model.CandidateId && x.Status != 1 && x.InterviewScheduleId == interviewId);

                    // Vị trí ứng tuyển
                    var vacanId = context.CandidateVacanciesMapping.FirstOrDefault(x => x.CandidateId == _model.CandidateId)?.VacanciesId;
                    var vacan = context.Vacancies.FirstOrDefault(x => x.VacanciesId == vacanId);
                    if (result.Contains(ViTriUngTuyen))
                    {
                        if (!string.IsNullOrEmpty(vacan?.VacanciesName))
                        {
                            result = result.Replace(ViTriUngTuyen, vacan.VacanciesName);
                        }
                        else
                        {
                            result = result.Replace(ViTriUngTuyen, "");
                        }
                    }

                    // Thời gian
                    if (result.Contains(ThoiGian))
                    {
                        if (scheduleInter.InterviewDate != null)
                        {
                            result = result.Replace(ThoiGian, FormatDateToString(scheduleInter.InterviewDate));
                        }
                        else
                        {
                            result = result.Replace(ThoiGian, "");
                        }
                    }

                    if (result.Contains(HinhThucPV))
                    {
                        result = result.Replace(HinhThucPV, scheduleInter.InterviewScheduleType == 1 ? "Trực tiếp" : "Online");
                    }

                    if (result.Contains(DiaChi_Link))
                    {
                        var diachi_link = scheduleInter.InterviewScheduleType == 1 ? "Địa chỉ: " : "Link: ";
                        if (!string.IsNullOrEmpty(scheduleInter.Address))
                        {
                            result = result.Replace(DiaChi_Link, diachi_link + scheduleInter.Address);
                        }
                        else
                        {
                            result = result.Replace(DiaChi_Link, "");
                        }
                    }
                }
            }
            // Thông báo khi nhân viên sắp hết hạn hợp đồng
            else if (typeModel == TypeModel.EmployeeInfor && actionCode != "DEADLINE_SUBMISSION")
            {
                var _listModel = model as List<HopDongNhanSu>;

                if (_listModel.Count() > 0)
                {
                    if (result.Contains(Logo))
                    {
                        var logo = configEntity.FirstOrDefault(w => w.SystemKey == "Logo").SystemValueString;

                        if (!String.IsNullOrEmpty(logo))
                        {
                            var temp_logo = "<img src=\"" + logo + "\" class=\"e - rte - image e - imginline\" alt=\"Logo TNM.png\" width=\"auto\" height=\"auto\" style=\"min - width: 0px; max - width: 750px; min - height: 0px; \">";
                            result = result.Replace(Logo, temp_logo);
                        }
                        else
                        {
                            result = result.Replace(Logo, "");
                        }
                    }


                    if (result.Contains(CompanyName))
                    {
                        var company_name = context.CompanyConfiguration.FirstOrDefault().CompanyName;

                        if (!String.IsNullOrEmpty(company_name))
                        {
                            result = result.Replace(CompanyName, company_name);
                        }
                        else
                        {
                            result = result.Replace(Logo, "");
                        }
                    }

                    if (result.Contains(ListNhanVien))
                    {
                        var content = "";
                        var lstEmployeeId = _listModel.Select(x => x.EmployeeId).ToList();
                        var lstEmployee = context.Employee.Where(x => lstEmployeeId.Contains(x.EmployeeId)).ToList();

                        var listAllPhongBan = context.Organization.ToList();

                        int index = 1;
                        _listModel.ForEach(item =>
                        {
                            var contentEmployee = "";
                            var employee = lstEmployee.FirstOrDefault(x => x.EmployeeId == item.EmployeeId);
                            if (employee.EmployeeCode != null)
                            {
                                contentEmployee += "<p>" + index + "." + employee.EmployeeCode;
                            }
                            if (employee.EmployeeName != null)
                            {
                                contentEmployee += " - " + employee.EmployeeName;
                            }

                            var _phongBan = listAllPhongBan.FirstOrDefault(o => o.OrganizationId == employee.OrganizationId)?.OrganizationName;

                            if (!String.IsNullOrEmpty(_phongBan))
                            {
                                contentEmployee += ", " + _phongBan;
                            }

                            if (actionCode == "PROBATION_OVER")
                            {
                                contentEmployee += ", Thời gian thử việc: " + item.NgayKyHopDong.Date.ToString("dd/MM/yyyy") + " - " + item.NgayKetThucHopDong.Value.Date.ToString("dd/MM/yyyy") + "</p>";
                            }
                            else
                            {
                                contentEmployee += ", Số hợp đồng: " + item.SoHopDong;
                                contentEmployee += ", Thời gian hợp đồng: " + item.NgayKyHopDong.Date.ToString("dd/MM/yyyy") + " - " + item.NgayKetThucHopDong.Value.Date.ToString("dd/MM/yyyy") + "</p>";
                            }

                            content += contentEmployee;
                            index++;
                        });

                        if (!String.IsNullOrEmpty(content))
                        {
                            result = result.Replace(ListNhanVien, content);
                        }
                    }
                }
            }
            // Thông báo khi nhân viên sắp đến hạn nộp hồ sơ
            else if (typeModel == TypeModel.EmployeeInfor && actionCode == "DEADLINE_SUBMISSION")
            {
                var _listModel = model as List<TaiLieuNhanVien>;

                if (_listModel.Count() > 0)
                {
                    if (result.Contains(Logo))
                    {
                        var logo = configEntity.FirstOrDefault(w => w.SystemKey == "Logo").SystemValueString;

                        if (!String.IsNullOrEmpty(logo))
                        {
                            var temp_logo = "<img src=\"" + logo + "\" class=\"e - rte - image e - imginline\" alt=\"Logo TNM.png\" width=\"auto\" height=\"auto\" style=\"min - width: 0px; max - width: 750px; min - height: 0px; \">";
                            result = result.Replace(Logo, temp_logo);
                        }
                        else
                        {
                            result = result.Replace(Logo, "");
                        }
                    }


                    if (result.Contains(CompanyName))
                    {
                        var company_name = context.CompanyConfiguration.FirstOrDefault().CompanyName;

                        if (!String.IsNullOrEmpty(company_name))
                        {
                            result = result.Replace(CompanyName, company_name);
                        }
                        else
                        {
                            result = result.Replace(Logo, "");
                        }
                    }

                    if (result.Contains(ListNhanVien))
                    {
                        var content = "";
                        var lstEmployeeId = _listModel.Select(x => x.EmployeeId).Distinct().ToList();
                        var lstEmployee = context.Employee.Where(x => lstEmployeeId.Contains(x.EmployeeId)).ToList();

                        var listAllPhongBan = context.Organization.ToList();

                        int index = 1;
                        lstEmployee.ForEach(item =>
                        {
                            var contentEmployee = "";
                            var employee = lstEmployee.FirstOrDefault(x => x.EmployeeId == item.EmployeeId);
                            if (item.EmployeeCode != null)
                            {
                                contentEmployee += "<p>" + index + "." + item.EmployeeCode;
                            }
                            if (item.EmployeeName != null)
                            {
                                contentEmployee += " - " + item.EmployeeName;
                            }

                            var _phongBan = listAllPhongBan.FirstOrDefault(o => o.OrganizationId == item.OrganizationId)?.OrganizationName;

                            if (!String.IsNullOrEmpty(_phongBan))
                            {
                                contentEmployee += ", " + _phongBan;
                            }

                            var listTaiLieu = _listModel.Where(x => x.EmployeeId == item.EmployeeId).Select(x => x.TenTaiLieu).ToArray();
                            contentEmployee += ": " + String.Join(", ", listTaiLieu);

                            content += contentEmployee;
                            index++;
                        });

                        if (!String.IsNullOrEmpty(content))
                        {
                            result = result.Replace(ListNhanVien, content);
                        }
                    }
                }
            }
            // Thông báo khi đánh giá nhân viên
            else if (typeModel == TypeModel.ThucHienDanhGia)
            {
                var _model = model as DanhGiaNhanVien;
                var nhanVienKyDanHGia = context.NhanVienKyDanhGia.FirstOrDefault(x => x.NhanVienKyDanhGiaId == _model.NhanVienKyDanhGiaId);
                if (_model != null && nhanVienKyDanHGia != null)
                {
                    if (result.Contains(Logo))
                    {
                        var logo = configEntity.FirstOrDefault(w => w.SystemKey == "Logo").SystemValueString;

                        if (!String.IsNullOrEmpty(logo))
                        {
                            var temp_logo = "<img src=\"" + logo +
                                            "\" class=\"e - rte - image e - imginline\" alt=\"Logo TNM.png\" width=\"auto\" height=\"auto\" style=\"min - width: 0px; max - width: 750px; min - height: 0px; \">";
                            result = result.Replace(Logo, temp_logo);
                        }
                        else
                        {
                            result = result.Replace(Logo, "");
                        }
                    }

                    if (result.Contains(EmployeeCode))
                    {
                        var empName = context.Employee.FirstOrDefault(x => x.EmployeeId == nhanVienKyDanHGia.NguoiDuocDanhGiaId)?.EmployeeCode;

                        if (!string.IsNullOrEmpty(empName))
                        {
                            result = result.Replace(EmployeeCode, empName);
                        }
                        else
                        {
                            result = result.Replace(EmployeeCode, "");
                        }
                    }

                    if (result.Contains(EmployeeName))
                    {
                        var empName = context.Employee.FirstOrDefault(x => x.EmployeeId == nhanVienKyDanHGia.NguoiDuocDanhGiaId)?.EmployeeName;

                        if (!string.IsNullOrEmpty(empName))
                        {
                            result = result.Replace(EmployeeName, empName);
                        }
                        else
                        {
                            result = result.Replace(EmployeeName, "");
                        }
                    }


                    if (result.Contains(Url_Login))
                    {
                        var Domain = configEntity.FirstOrDefault(w => w.SystemKey == "Domain").SystemValueString;
                        var loginLink = Domain + @"/employee/thuc-hien-danh-gia;danhGiaNhanVienId=" + EncrDecrCrypto.Encrypt(_model.DanhGiaNhanVienId.ToString());

                        if (!String.IsNullOrEmpty(loginLink))
                        {
                            result = result.Replace(Url_Login, loginLink);
                        }
                    }
                }
            }
            // Thông báo khi tài sản gần đến thời gian bảo trì, bảo dưỡng
            else if (typeModel == TypeModel.TaiSanInfor)
            {
                var _model = model as List<AssetEntityModel>;
                if (_model != null)
                {
                    if (result.Contains(Logo))
                    {
                        var logo = configEntity.FirstOrDefault(w => w.SystemKey == "Logo").SystemValueString;

                        if (!String.IsNullOrEmpty(logo))
                        {
                            var temp_logo = "<img src=\"" + logo +
                                            "\" class=\"e - rte - image e - imginline\" alt=\"Logo TNM.png\" width=\"auto\" height=\"auto\" style=\"min - width: 0px; max - width: 750px; min - height: 0px; \">";
                            result = result.Replace(Logo, temp_logo);
                        }
                        else
                        {
                            result = result.Replace(Logo, "");
                        }
                    }

                    if (result.Contains(DanhSachTaiSan))
                    {
                        var dsTaiSan = "";
                        _model.ForEach(item =>
                        {
                            dsTaiSan = dsTaiSan + "<p><strong>Tên tài sản: " + item.TenTaiSan + "- Mã tài sản: " + item.MaTaiSan + "</strong></p>";
                        });
                        if (!String.IsNullOrEmpty(DanhSachTaiSan))
                        {
                            result = result.Replace(DanhSachTaiSan, dsTaiSan);
                        }
                        else
                        {
                            result = result.Replace(DanhSachTaiSan, "");
                        }
                    }

                    if (result.Contains(Url_Login))
                    {
                        var Domain = configEntity.FirstOrDefault(w => w.SystemKey == "Domain").SystemValueString;
                        var loginLink = Domain + @"/asset/list";

                        if (!String.IsNullOrEmpty(loginLink))
                        {
                            result = result.Replace(Url_Login, loginLink);
                        }
                    }
                }
            }
            // Chi tiết đề Kỳ lương
            else if (typeModel == TypeModel.KyLuong)
            {
                var _model = model as KyLuong;

                if (_model != null)
                {
                    if (result.Contains(Logo))
                    {
                        var logo = configEntity.FirstOrDefault(w => w.SystemKey == "Logo").SystemValueString;

                        if (!String.IsNullOrEmpty(logo))
                        {
                            var temp_logo = "<img src=\"" + logo +
                                            "\" class=\"e - rte - image e - imginline\" alt=\"Logo TNM.png\" width=\"auto\" height=\"auto\" style=\"min - width: 0px; max - width: 750px; min - height: 0px; \">";
                            result = result.Replace(Logo, temp_logo);
                        }
                        else
                        {
                            result = result.Replace(Logo, "");
                        }
                    }

                    if (result.Contains(TenKyLuong))
                    {
                        var tenKyLuong = _model?.TenKyLuong;

                        if (!string.IsNullOrEmpty(tenKyLuong))
                        {
                            result = result.Replace(TenKyLuong, tenKyLuong);
                        }
                        else
                        {
                            result = result.Replace(TenKyLuong, "");
                        }
                    }

                    if (result.Contains(KyLuong_NgayBatDau))
                    {
                        var ngayBatDau = _model?.TuNgay.ToString("dd/MM/yyyy");

                        if (!string.IsNullOrEmpty(ngayBatDau))
                        {
                            result = result.Replace(KyLuong_NgayBatDau, ngayBatDau);
                        }
                        else
                        {
                            result = result.Replace(KyLuong_NgayBatDau, "");
                        }
                    }

                    if (result.Contains(KyLuong_NgayKetThuc))
                    {
                        var ngayKetThuc = _model?.DenNgay.ToString("dd/MM/yyyy");

                        if (!string.IsNullOrEmpty(ngayKetThuc))
                        {
                            result = result.Replace(KyLuong_NgayKetThuc, ngayKetThuc);
                        }
                        else
                        {
                            result = result.Replace(KyLuong_NgayKetThuc, "");
                        }
                    }

                    if (result.Contains(KyLuong_LyDoTuChoi))
                    {
                        if (!string.IsNullOrEmpty(_model.LyDoTuChoi))
                        {
                            result = result.Replace(KyLuong_LyDoTuChoi, _model.LyDoTuChoi);
                        }
                        else
                        {
                            result = result.Replace(KyLuong_LyDoTuChoi, "");
                        }
                    }

                    if (result.Contains(Url_Login))
                    {
                        var Domain = configEntity.FirstOrDefault(w => w.SystemKey == "Domain").SystemValueString;
                        var loginLink = Domain + @"/salary/ky-luong-detail;kyLuongId=" + EncrDecrCrypto.Encrypt(_model.KyLuongId.ToString());

                        if (!String.IsNullOrEmpty(loginLink))
                        {
                            result = result.Replace(Url_Login, loginLink);
                        }
                    }
                }
            }
            // Phiếu lương
            else if (typeModel == TypeModel.PhieuLuong)
            {
                var _model = model as PhieuLuongModel;

                if (_model != null)
                {
                    if (result.Contains(Logo))
                    {
                        var logo = configEntity.FirstOrDefault(w => w.SystemKey == "Logo").SystemValueString;

                        if (!String.IsNullOrEmpty(logo))
                        {
                            var temp_logo = "<img src=\"" + logo +
                                            "\" class=\"e - rte - image e - imginline\" alt=\"Logo TNM.png\" width=\"auto\" height=\"auto\" style=\"min - width: 0px; max - width: 750px; min - height: 0px; \">";
                            result = result.Replace(Logo, temp_logo);
                        }
                        else
                        {
                            result = result.Replace(Logo, "");
                        }
                    }

                    if (result.Contains(EmployeeName))
                    {
                        if (!string.IsNullOrEmpty(_model?.EmployeeName))
                        {
                            result = result.Replace(EmployeeName, _model?.EmployeeName);
                        }
                        else
                        {
                            result = result.Replace(EmployeeName, "");
                        }
                    }

                    if (result.Contains(EmployeeCode))
                    {
                        if (!string.IsNullOrEmpty(_model?.EmployeeCode))
                        {
                            result = result.Replace(EmployeeCode, _model?.EmployeeCode);
                        }
                        else
                        {
                            result = result.Replace(EmployeeCode, "");
                        }
                    }

                    if (result.Contains(TenKyLuong))
                    {
                        var tenKyLuong = _model?.TenKyLuong;

                        if (!string.IsNullOrEmpty(tenKyLuong))
                        {
                            result = result.Replace(TenKyLuong, tenKyLuong);
                        }
                        else
                        {
                            result = result.Replace(TenKyLuong, "");
                        }
                    }

                    if (result.Contains(SoNgayLamViec))
                    {
                        if (!string.IsNullOrEmpty(_model?.SoNgayLamViec.ToString("#,#0.##")))
                        {
                            result = result.Replace(SoNgayLamViec, _model?.SoNgayLamViec.ToString("#,#0.##"));
                        }
                        else
                        {
                            result = result.Replace(SoNgayLamViec, "");
                        }
                    }

                    if (result.Contains(WorkEmail))
                    {
                        if (!string.IsNullOrEmpty(_model?.WorkEmail))
                        {
                            result = result.Replace(WorkEmail, _model?.WorkEmail);
                        }
                        else
                        {
                            result = result.Replace(WorkEmail, "");
                        }
                    }

                    if (result.Contains(ThangBatDauKyLuong))
                    {
                        if (!string.IsNullOrEmpty(_model?.ThangBatDauKyLuong))
                        {
                            result = result.Replace(ThangBatDauKyLuong, _model?.ThangBatDauKyLuong);
                        }
                        else
                        {
                            result = result.Replace(ThangBatDauKyLuong, "");
                        }
                    }

                    if (result.Contains(NamBatDauKyLuong))
                    {
                        if (!string.IsNullOrEmpty(_model?.NamBatDauKyLuong))
                        {
                            result = result.Replace(NamBatDauKyLuong, _model?.NamBatDauKyLuong);
                        }
                        else
                        {
                            result = result.Replace(NamBatDauKyLuong, "");
                        }
                    }

                    if (result.Contains(ThangTruoc))
                    {
                        if (!string.IsNullOrEmpty(_model?.ThangTruoc))
                        {
                            result = result.Replace(ThangTruoc, _model?.ThangTruoc);
                        }
                        else
                        {
                            result = result.Replace(ThangTruoc, "");
                        }
                    }

                    if (result.Contains(ThangTruocTiengAnh))
                    {
                        if (!string.IsNullOrEmpty(_model?.ThangTruocTiengAnh))
                        {
                            result = result.Replace(ThangTruocTiengAnh, _model?.ThangTruocTiengAnh);
                        }
                        else
                        {
                            result = result.Replace(ThangTruocTiengAnh, "");
                        }
                    }

                    if (result.Contains(NamTheoThangTruoc))
                    {
                        if (!string.IsNullOrEmpty(_model?.NamTheoThangTruoc))
                        {
                            result = result.Replace(NamTheoThangTruoc, _model?.NamTheoThangTruoc);
                        }
                        else
                        {
                            result = result.Replace(NamTheoThangTruoc, "");
                        }
                    }

                    if (result.Contains(ThangKetThucKyLuong))
                    {
                        if (!string.IsNullOrEmpty(_model?.ThangKetThucKyLuong))
                        {
                            result = result.Replace(ThangKetThucKyLuong, _model?.ThangKetThucKyLuong);
                        }
                        else
                        {
                            result = result.Replace(ThangKetThucKyLuong, "");
                        }
                    }

                    if (result.Contains(NamKetThucKyLuong))
                    {
                        if (!string.IsNullOrEmpty(_model?.NamKetThucKyLuong))
                        {
                            result = result.Replace(NamKetThucKyLuong, _model?.NamKetThucKyLuong);
                        }
                        else
                        {
                            result = result.Replace(NamKetThucKyLuong, "");
                        }
                    }

                    if (result.Contains(DuocHuongTroCapKpi))
                    {
                        if (!string.IsNullOrEmpty(_model?.DuocHuongTroCapKpi))
                        {
                            result = result.Replace(DuocHuongTroCapKpi, _model?.DuocHuongTroCapKpi);
                        }
                        else
                        {
                            result = result.Replace(DuocHuongTroCapKpi, "");
                        }
                    }

                    if (result.Contains(CauHinhGiamTruCaNhan))
                    {
                        if (!string.IsNullOrEmpty(_model?.CauHinhGiamTruCaNhan.ToString("#,#0.##")))
                        {
                            result = result.Replace(CauHinhGiamTruCaNhan, _model?.CauHinhGiamTruCaNhan.ToString("#,#0.##"));
                        }
                        else
                        {
                            result = result.Replace(CauHinhGiamTruCaNhan, "");
                        }
                    }

                    if (result.Contains(CauHinhGiamTruNguoiPhuThuoc))
                    {
                        if (!string.IsNullOrEmpty(_model?.CauHinhGiamTruNguoiPhuThuoc.ToString("#,#0.##")))
                        {
                            result = result.Replace(CauHinhGiamTruNguoiPhuThuoc, _model?.CauHinhGiamTruNguoiPhuThuoc.ToString("#,#0.##"));
                        }
                        else
                        {
                            result = result.Replace(CauHinhGiamTruNguoiPhuThuoc, "");
                        }
                    }

                    if (result.Contains(PhanTramBaoHiemCty))
                    {
                        if (!string.IsNullOrEmpty(_model?.PhanTramBaoHiemCty.ToString("#,#0.##")))
                        {
                            result = result.Replace(PhanTramBaoHiemCty, _model?.PhanTramBaoHiemCty.ToString("#,#0.##"));
                        }
                        else
                        {
                            result = result.Replace(PhanTramBaoHiemCty, "");
                        }
                    }

                    if (result.Contains(PhanTramBaoHiemNld))
                    {
                        if (!string.IsNullOrEmpty(_model?.PhanTramBaoHiemNld.ToString("#,#0.##")))
                        {
                            result = result.Replace(PhanTramBaoHiemNld, _model?.PhanTramBaoHiemNld.ToString("#,#0.##"));
                        }
                        else
                        {
                            result = result.Replace(PhanTramBaoHiemNld, "");
                        }
                    }

                    if (result.Contains(PhanTramKinhPhiCongDoanCty))
                    {
                        if (!string.IsNullOrEmpty(_model?.PhanTramKinhPhiCongDoanCty.ToString("#,#0.##")))
                        {
                            result = result.Replace(PhanTramKinhPhiCongDoanCty, _model?.PhanTramKinhPhiCongDoanCty.ToString("#,#0.##"));
                        }
                        else
                        {
                            result = result.Replace(PhanTramKinhPhiCongDoanCty, "");
                        }
                    }

                    if (result.Contains(LuongCoBan))
                    {
                        if (!string.IsNullOrEmpty(_model?.LuongCoBan.ToString("#,#0.##")))
                        {
                            result = result.Replace(LuongCoBan, _model?.LuongCoBan.ToString("#,#0.##"));
                        }
                        else
                        {
                            result = result.Replace(LuongCoBan, "");
                        }
                    }

                    if (result.Contains(LuongCoBanSau))
                    {
                        if (!string.IsNullOrEmpty(_model?.LuongCoBanSau.ToString("#,#.")))
                        {
                            result = result.Replace(LuongCoBanSau, _model?.LuongCoBanSau.ToString("#,#."));
                        }
                        else
                        {
                            result = result.Replace(LuongCoBanSau, "");
                        }
                    }

                    if (result.Contains(MucDieuChinh))
                    {
                        if (!string.IsNullOrEmpty(_model?.MucDieuChinh.ToString("#,#0.##")))
                        {
                            result = result.Replace(MucDieuChinh, _model?.MucDieuChinh.ToString("#,#0.##"));
                        }
                        else
                        {
                            result = result.Replace(MucDieuChinh, "");
                        }
                    }

                    if (result.Contains(NgayLamViecThucTe))
                    {
                        if (!string.IsNullOrEmpty(_model?.NgayLamViecThucTe.ToString("#,#0.##")))
                        {
                            result = result.Replace(NgayLamViecThucTe, _model?.NgayLamViecThucTe.ToString("#,#0.##"));
                        }
                        else
                        {
                            result = result.Replace(NgayLamViecThucTe, "");
                        }
                    }

                    if (result.Contains(NgayNghiPhep))
                    {
                        if (!string.IsNullOrEmpty(_model?.NgayNghiPhep.ToString("#,#0.##")))
                        {
                            result = result.Replace(NgayNghiPhep, _model?.NgayNghiPhep.ToString("#,#0.##"));
                        }
                        else
                        {
                            result = result.Replace(NgayNghiPhep, "");
                        }
                    }

                    if (result.Contains(NgayNghiLe))
                    {
                        if (!string.IsNullOrEmpty(_model?.NgayNghiLe.ToString("#,#0.##")))
                        {
                            result = result.Replace(NgayNghiLe, _model?.NgayNghiLe.ToString("#,#0.##"));
                        }
                        else
                        {
                            result = result.Replace(NgayNghiLe, "");
                        }
                    }

                    if (result.Contains(NgayNghiKhongLuong))
                    {
                        if (!string.IsNullOrEmpty(_model?.NgayNghiKhongLuong.ToString("#,#0.##")))
                        {
                            result = result.Replace(NgayNghiKhongLuong, _model?.NgayNghiKhongLuong.ToString("#,#0.##"));
                        }
                        else
                        {
                            result = result.Replace(NgayNghiKhongLuong, "");
                        }
                    }

                    if (result.Contains(NgayDmvs))
                    {
                        if (!string.IsNullOrEmpty(_model?.NgayDmvs.ToString("#,#0.##")))
                        {
                            result = result.Replace(NgayDmvs, _model?.NgayDmvs.ToString("#,#0.##"));
                        }
                        else
                        {
                            result = result.Replace(NgayDmvs, "");
                        }
                    }

                    if (result.Contains(NgayKhongHuongChuyenCan))
                    {
                        if (!string.IsNullOrEmpty(_model?.NgayKhongHuongChuyenCan.ToString("#,#0.##")))
                        {
                            result = result.Replace(NgayKhongHuongChuyenCan, _model?.NgayKhongHuongChuyenCan.ToString("#,#0.##"));
                        }
                        else
                        {
                            result = result.Replace(NgayKhongHuongChuyenCan, "");
                        }
                    }

                    if (result.Contains(SoLuongDkGiamTruGiaCanh))
                    {
                        if (!string.IsNullOrEmpty(_model?.SoLuongDkGiamTruGiaCanh.ToString("#,#0.##")))
                        {
                            result = result.Replace(SoLuongDkGiamTruGiaCanh, _model?.SoLuongDkGiamTruGiaCanh.ToString("#,#0.##"));
                        }
                        else
                        {
                            result = result.Replace(SoLuongDkGiamTruGiaCanh, "");
                        }
                    }

                    if (result.Contains(GiamTruGiaCanh))
                    {
                        if (!string.IsNullOrEmpty(_model?.GiamTruGiaCanh.ToString("#,#0.##")))
                        {
                            result = result.Replace(GiamTruGiaCanh, _model?.GiamTruGiaCanh.ToString("#,#0.##"));
                        }
                        else
                        {
                            result = result.Replace(GiamTruGiaCanh, "");
                        }
                    }

                    if (result.Contains(LuongTheoNgayHocViec))
                    {
                        if (!string.IsNullOrEmpty(_model?.LuongTheoNgayHocViec.ToString("#,#0.##")))
                        {
                            result = result.Replace(LuongTheoNgayHocViec, _model?.LuongTheoNgayHocViec.ToString("#,#0.##"));
                        }
                        else
                        {
                            result = result.Replace(LuongTheoNgayHocViec, "");
                        }
                    }

                    if (result.Contains(TroCapDiLai))
                    {
                        if (!string.IsNullOrEmpty(_model?.TroCapDiLai.ToString("#,#0.##")))
                        {
                            result = result.Replace(TroCapDiLai, _model?.TroCapDiLai.ToString("#,#0.##"));
                        }
                        else
                        {
                            result = result.Replace(TroCapDiLai, "");
                        }
                    }

                    if (result.Contains(TroCapDienThoai))
                    {
                        if (!string.IsNullOrEmpty(_model?.TroCapDienThoai.ToString("#,#0.##")))
                        {
                            result = result.Replace(TroCapDienThoai, _model?.TroCapDienThoai.ToString("#,#0.##"));
                        }
                        else
                        {
                            result = result.Replace(TroCapDienThoai, "");
                        }
                    }

                    if (result.Contains(TroCapAnTrua))
                    {
                        if (!string.IsNullOrEmpty(_model?.TroCapAnTrua.ToString("#,#0.##")))
                        {
                            result = result.Replace(TroCapAnTrua, _model?.TroCapAnTrua.ToString("#,#0.##"));
                        }
                        else
                        {
                            result = result.Replace(TroCapAnTrua, "");
                        }
                    }

                    if (result.Contains(TroCapNhaO))
                    {
                        if (!string.IsNullOrEmpty(_model?.TroCapNhaO.ToString("#,#0.##")))
                        {
                            result = result.Replace(TroCapNhaO, _model?.TroCapNhaO.ToString("#,#0.##"));
                        }
                        else
                        {
                            result = result.Replace(TroCapNhaO, "");
                        }
                    }

                    if (result.Contains(TroCapChuyenCan))
                    {
                        if (!string.IsNullOrEmpty(_model?.TroCapChuyenCan.ToString("#,#0.##")))
                        {
                            result = result.Replace(TroCapChuyenCan, _model?.TroCapChuyenCan.ToString("#,#0.##"));
                        }
                        else
                        {
                            result = result.Replace(TroCapChuyenCan, "");
                        }
                    }

                    if (result.Contains(ThuongKpi))
                    {
                        if (!string.IsNullOrEmpty(_model?.ThuongKpi.ToString("#,#0.##")))
                        {
                            result = result.Replace(ThuongKpi, _model?.ThuongKpi.ToString("#,#0.##"));
                        }
                        else
                        {
                            result = result.Replace(ThuongKpi, "");
                        }
                    }

                    if (result.Contains(ThuongCuoiNam))
                    {
                        if (!string.IsNullOrEmpty(_model?.ThuongCuoiNam.ToString("#,#0.##")))
                        {
                            result = result.Replace(ThuongCuoiNam, _model?.ThuongCuoiNam.ToString("#,#0.##"));
                        }
                        else
                        {
                            result = result.Replace(ThuongCuoiNam, "");
                        }
                    }

                    if (result.Contains(TroCapTrachNhiem))
                    {
                        if (!string.IsNullOrEmpty(_model?.TroCapTrachNhiem.ToString("#,#0.##")))
                        {
                            result = result.Replace(TroCapTrachNhiem, _model?.TroCapTrachNhiem.ToString("#,#0.##"));
                        }
                        else
                        {
                            result = result.Replace(TroCapTrachNhiem, "");
                        }
                    }

                    if (result.Contains(TroCapHocViec))
                    {
                        if (!string.IsNullOrEmpty(_model?.TroCapHocViec.ToString("#,#0.##")))
                        {
                            result = result.Replace(TroCapHocViec, _model?.TroCapHocViec.ToString("#,#0.##"));
                        }
                        else
                        {
                            result = result.Replace(TroCapHocViec, "");
                        }
                    }

                    if (result.Contains(OtTinhThue))
                    {
                        if (!string.IsNullOrEmpty(_model?.OtTinhThue.ToString("#,#0.##")))
                        {
                            result = result.Replace(OtTinhThue, _model?.OtTinhThue.ToString("#,#0.##"));
                        }
                        else
                        {
                            result = result.Replace(OtTinhThue, "");
                        }
                    }

                    if (result.Contains(OtKhongTinhThue))
                    {
                        if (!string.IsNullOrEmpty(_model?.OtKhongTinhThue.ToString("#,#0.##")))
                        {
                            result = result.Replace(OtKhongTinhThue, _model?.OtKhongTinhThue.ToString("#,#0.##"));
                        }
                        else
                        {
                            result = result.Replace(OtKhongTinhThue, "");
                        }
                    }

                    if (result.Contains(LuongThang13))
                    {
                        if (!string.IsNullOrEmpty(_model?.LuongThang13.ToString("#,#0.##")))
                        {
                            result = result.Replace(LuongThang13, _model?.LuongThang13.ToString("#,#0.##"));
                        }
                        else
                        {
                            result = result.Replace(LuongThang13, "");
                        }
                    }

                    if (result.Contains(QuaBocTham))
                    {
                        if (!string.IsNullOrEmpty(_model?.QuaBocTham.ToString("#,#0.##")))
                        {
                            result = result.Replace(QuaBocTham, _model?.QuaBocTham.ToString("#,#0.##"));
                        }
                        else
                        {
                            result = result.Replace(QuaBocTham, "");
                        }
                    }

                    if (result.Contains(TongThueTncn))
                    {
                        if (!string.IsNullOrEmpty(_model?.TongThueTncn.ToString("#,#0.##")))
                        {
                            result = result.Replace(TongThueTncn, _model?.TongThueTncn.ToString("#,#0.##"));
                        }
                        else
                        {
                            result = result.Replace(TongThueTncn, "");
                        }
                    }

                    if (result.Contains(BaoHiem))
                    {
                        if (!string.IsNullOrEmpty(_model?.BaoHiem.ToString("#,#0.##")))
                        {
                            result = result.Replace(BaoHiem, _model?.BaoHiem.ToString("#,#0.##"));
                        }
                        else
                        {
                            result = result.Replace(BaoHiem, "");
                        }
                    }

                    if (result.Contains(ThucNhan))
                    {
                        if (!string.IsNullOrEmpty(_model?.ThucNhan.ToString("#,#0.##")))
                        {
                            result = result.Replace(ThucNhan, _model?.ThucNhan.ToString("#,#0.##"));
                        }
                        else
                        {
                            result = result.Replace(ThucNhan, "");
                        }
                    }

                    if (result.Contains(CtyTraBh))
                    {
                        if (!string.IsNullOrEmpty(_model?.CtyTraBh.ToString("#,#0.##")))
                        {
                            result = result.Replace(CtyTraBh, _model?.CtyTraBh.ToString("#,#0.##"));
                        }
                        else
                        {
                            result = result.Replace(CtyTraBh, "");
                        }
                    }

                    if (result.Contains(KinhPhiCongDoan))
                    {
                        if (!string.IsNullOrEmpty(_model?.KinhPhiCongDoan.ToString("#,#0.##")))
                        {
                            result = result.Replace(KinhPhiCongDoan, _model?.KinhPhiCongDoan.ToString("#,#0.##"));
                        }
                        else
                        {
                            result = result.Replace(KinhPhiCongDoan, "");
                        }
                    }

                    if (result.Contains(TongChiPhiNhanVien))
                    {
                        if (!string.IsNullOrEmpty(_model?.TongChiPhiNhanVien.ToString("#,#0.##")))
                        {
                            result = result.Replace(TongChiPhiNhanVien, _model?.TongChiPhiNhanVien.ToString("#,#0.##"));
                        }
                        else
                        {
                            result = result.Replace(TongChiPhiNhanVien, "");
                        }
                    }

                    //if (result.Contains(Url_Login))
                    //{
                    //    var Domain = configEntity.FirstOrDefault(w => w.SystemKey == "Domain").SystemValueString;
                    //    var loginLink = Domain + @"/salary/phieu-luong-detail;phieuLuongId=" + EncrDecrCrypto.Encrypt(_model.PhieuLuongId.ToString());

                    //    if (!String.IsNullOrEmpty(loginLink))
                    //    {
                    //        result = result.Replace(Url_Login, loginLink);
                    //    }
                    //}
                }
            }
            // Chi tiết đề xuất công tác
            else if (typeModel == TypeModel.DeXuatCongTac)
            {
                var _model = model as DeXuatCongTac;
                if (_model != null)
                {
                    if (result.Contains(Logo))
                    {
                        var logo = configEntity.FirstOrDefault(w => w.SystemKey == "Logo").SystemValueString;

                        if (!String.IsNullOrEmpty(logo))
                        {
                            var temp_logo = "<img src=\"" + logo +
                                            "\" class=\"e - rte - image e - imginline\" alt=\"Logo TNM.png\" width=\"auto\" height=\"auto\" style=\"min - width: 0px; max - width: 750px; min - height: 0px; \">";
                            result = result.Replace(Logo, temp_logo);
                        }
                        else
                        {
                            result = result.Replace(Logo, "");
                        }
                    }

                    if (result.Contains(EmployeeName))
                    {
                        var emp = context.Employee.FirstOrDefault(x => x.EmployeeId == _model.NguoiDeXuatId);
                        var empName = emp?.EmployeeCode + " - " + emp?.EmployeeName;

                        if (!string.IsNullOrEmpty(empName))
                        {
                            result = result.Replace(EmployeeName, empName);
                        }
                        else
                        {
                            result = result.Replace(EmployeeName, "");
                        }
                    }

                    if (result.Contains(TenDeXuatCongTac))
                    {
                        var createCode = _model.MaDeXuat + " - " + _model.TenDeXuat;

                        if (!string.IsNullOrEmpty(createCode))
                        {
                            result = result.Replace(TenDeXuatCongTac, createCode);
                        }
                        else
                        {
                            result = result.Replace(KeHoachOt, "");
                        }
                    }

                    if (result.Contains(UpdatedDate))
                    {
                        var updatedDate = _model.UpdatedDate;
                        result = result.Replace(UpdatedDate, FormatDateToString(updatedDate));
                    }

                    if (result.Contains(Url_Login))
                    {
                        var Domain = configEntity.FirstOrDefault(w => w.SystemKey == "Domain").SystemValueString;
                        var loginLink = Domain + @"/employee/de-xuat-cong-tac-chi-tiet;deXuatCongTacId=" + EncrDecrCrypto.Encrypt(_model.DeXuatCongTacId.ToString());

                        if (!String.IsNullOrEmpty(loginLink))
                        {
                            result = result.Replace(Url_Login, loginLink);
                        }
                    }

                    //Lấy lý do từ chối: ( bản ghi chú mới nhất )
                    if (result.Contains(LyDoTuChoi))
                    {
                        //Lấy lý do từ chối từ note mới nhất
                        var lydotuchoi = context.Note.Where(x => x.ObjectNumber == _model.DeXuatCongTacId && x.ObjectType == NoteObjectType.DEXUATCT).OrderByDescending(x => x.CreatedDate).First().Description;
                        if (!string.IsNullOrEmpty(lydotuchoi))
                        {
                            result = result.Replace(LyDoTuChoi, lydotuchoi);
                        }
                        else
                        {
                            result = result.Replace(LyDoTuChoi, "");
                        }
                    }

                }
            }
            // Chi tiết đề xuất tạm ứng
            else if (typeModel == TypeModel.DeNghiTamUng)
            {
                var _model = model as DeNghiTamHoanUng;

                if (_model != null)
                {
                    if (result.Contains(Logo))
                    {
                        var logo = configEntity.FirstOrDefault(w => w.SystemKey == "Logo").SystemValueString;

                        if (!String.IsNullOrEmpty(logo))
                        {
                            var temp_logo = "<img src=\"" + logo +
                                            "\" class=\"e - rte - image e - imginline\" alt=\"Logo TNM.png\" width=\"auto\" height=\"auto\" style=\"min - width: 0px; max - width: 750px; min - height: 0px; \">";
                            result = result.Replace(Logo, temp_logo);
                        }
                        else
                        {
                            result = result.Replace(Logo, "");
                        }
                    }

                    if (result.Contains(EmployeeName))
                    {
                        var emp = context.Employee.FirstOrDefault(x => x.EmployeeId == _model.NguoiDeXuatId);
                        var empName = emp?.EmployeeCode + " - " + emp?.EmployeeName;

                        if (!string.IsNullOrEmpty(empName))
                        {
                            result = result.Replace(EmployeeName, empName);
                        }
                        else
                        {
                            result = result.Replace(EmployeeName, "");
                        }
                    }

                    if (result.Contains(TenDeXuatTamUng))
                    {
                        var createCode = _model.MaDeNghi;

                        if (!string.IsNullOrEmpty(createCode))
                        {
                            result = result.Replace(TenDeXuatTamUng, createCode);
                        }
                        else
                        {
                            result = result.Replace(TenDeXuatTamUng, "");
                        }
                    }

                    if (result.Contains(UpdatedDate))
                    {
                        var updatedDate = _model.UpdatedDate;
                        result = result.Replace(UpdatedDate, FormatDateToString(updatedDate));
                    }

                    if (result.Contains(Url_Login))
                    {
                        var Domain = configEntity.FirstOrDefault(w => w.SystemKey == "Domain").SystemValueString;
                        var loginLink = Domain + @"/employee/chi-tiet-de-nghi-tam-hoan-ung;deNghiTamHoanUngId=" + EncrDecrCrypto.Encrypt(_model.DeNghiTamHoanUngId.ToString()) + ";loaiDeNghi=0";

                        if (!String.IsNullOrEmpty(loginLink))
                        {
                            result = result.Replace(Url_Login, loginLink);
                        }
                    }

                    if (result.Contains(LyDoTuChoi))
                    {
                        //Lấy lý do từ chối từ note mới nhất
                        var lydotuchoi = context.Note.Where(x => x.ObjectNumber == _model.DeNghiTamHoanUngId && x.ObjectType == NoteObjectType.DENGHIHOANTAMUNG).OrderByDescending(x => x.CreatedDate).First().Description;
                        if (!string.IsNullOrEmpty(lydotuchoi))
                        {
                            result = result.Replace(LyDoTuChoi, lydotuchoi);
                        }
                        else
                        {
                            result = result.Replace(LyDoTuChoi, "");
                        }
                    }

                }
            }
            // Chi tiết đề xuất hoàn ứng
            else if (typeModel == TypeModel.DeNghiHoanUng)
            {
                var _model = model as DeNghiTamHoanUng;

                if (_model != null)
                {
                    if (result.Contains(Logo))
                    {
                        var logo = configEntity.FirstOrDefault(w => w.SystemKey == "Logo").SystemValueString;

                        if (!String.IsNullOrEmpty(logo))
                        {
                            var temp_logo = "<img src=\"" + logo +
                                            "\" class=\"e - rte - image e - imginline\" alt=\"Logo TNM.png\" width=\"auto\" height=\"auto\" style=\"min - width: 0px; max - width: 750px; min - height: 0px; \">";
                            result = result.Replace(Logo, temp_logo);
                        }
                        else
                        {
                            result = result.Replace(Logo, "");
                        }
                    }

                    if (result.Contains(EmployeeName))
                    {
                        var emp = context.Employee.FirstOrDefault(x => x.EmployeeId == _model.NguoiDeXuatId);
                        var empName = emp?.EmployeeCode + " - " + emp?.EmployeeName;

                        if (!string.IsNullOrEmpty(empName))
                        {
                            result = result.Replace(EmployeeName, empName);
                        }
                        else
                        {
                            result = result.Replace(EmployeeName, "");
                        }
                    }

                    if (result.Contains(TenDeXuatHoanUng))
                    {
                        var createCode = _model.MaDeNghi;

                        if (!string.IsNullOrEmpty(createCode))
                        {
                            result = result.Replace(TenDeXuatHoanUng, createCode);
                        }
                        else
                        {
                            result = result.Replace(TenDeXuatHoanUng, "");
                        }
                    }

                    if (result.Contains(UpdatedDate))
                    {
                        var updatedDate = _model.UpdatedDate;
                        result = result.Replace(UpdatedDate, FormatDateToString(updatedDate));
                    }

                    if (result.Contains(Url_Login))
                    {
                        var Domain = configEntity.FirstOrDefault(w => w.SystemKey == "Domain").SystemValueString;
                        var loginLink = Domain + @"/employee/chi-tiet-de-nghi-tam-hoan-ung;deNghiTamHoanUngId=" + EncrDecrCrypto.Encrypt(_model.DeNghiTamHoanUngId.ToString()) + ";loaiDeNghi=1";

                        if (!String.IsNullOrEmpty(loginLink))
                        {
                            result = result.Replace(Url_Login, loginLink);
                        }
                    }

                    if (result.Contains(LyDoTuChoi))
                    {
                        //Lấy lý do từ chối từ note mới nhất
                        var lydotuchoi = context.Note.Where(x => x.ObjectNumber == _model.DeNghiTamHoanUngId && x.ObjectType == NoteObjectType.DENGHIHOANTAMUNG).OrderByDescending(x => x.CreatedDate).First().Description;
                        if (!string.IsNullOrEmpty(lydotuchoi))
                        {
                            result = result.Replace(LyDoTuChoi, lydotuchoi);
                        }
                        else
                        {
                            result = result.Replace(LyDoTuChoi, "");
                        }
                    }
                }
            }
            // Chi tiết đề xuất cấp phát tài sản
            else if (typeModel == TypeModel.DeXuatCapPhatTs)
            {
                var _model = model as YeuCauCapPhatTaiSan;

                if (_model != null)
                {
                    if (result.Contains(Logo))
                    {
                        var logo = configEntity.FirstOrDefault(w => w.SystemKey == "Logo").SystemValueString;

                        if (!String.IsNullOrEmpty(logo))
                        {
                            var temp_logo = "<img src=\"" + logo +
                                            "\" class=\"e - rte - image e - imginline\" alt=\"Logo TNM.png\" width=\"auto\" height=\"auto\" style=\"min - width: 0px; max - width: 750px; min - height: 0px; \">";
                            result = result.Replace(Logo, temp_logo);
                        }
                        else
                        {
                            result = result.Replace(Logo, "");
                        }
                    }

                    if (result.Contains(EmployeeName))
                    {
                        var emp = context.Employee.FirstOrDefault(x => x.EmployeeId == _model.NguoiDeXuatId);
                        var empName = emp?.EmployeeCode + " - " + emp?.EmployeeName;

                        if (!string.IsNullOrEmpty(empName))
                        {
                            result = result.Replace(EmployeeName, empName);
                        }
                        else
                        {
                            result = result.Replace(EmployeeName, "");
                        }
                    }

                    if (result.Contains(TenDeXuatCapPhatTs))
                    {
                        var createCode = _model.MaYeuCau;

                        if (!string.IsNullOrEmpty(createCode))
                        {
                            result = result.Replace(TenDeXuatCapPhatTs, createCode);
                        }
                        else
                        {
                            result = result.Replace(TenDeXuatCapPhatTs, "");
                        }
                    }

                    if (result.Contains(UpdatedDate))
                    {
                        var updatedDate = _model.UpdatedDate;
                        result = result.Replace(UpdatedDate, FormatDateToString(updatedDate));
                    }

                    if (result.Contains(Url_Login))
                    {
                        var Domain = configEntity.FirstOrDefault(w => w.SystemKey == "Domain").SystemValueString;
                        var loginLink = Domain + @"/employee/chi-tiet-yeu-cau-cap-phat;requestId=" + EncrDecrCrypto.Encrypt(_model.YeuCauCapPhatTaiSanId.ToString());

                        if (!String.IsNullOrEmpty(loginLink))
                        {
                            result = result.Replace(Url_Login, loginLink);
                        }
                    }


                    if (result.Contains(LyDoTuChoi))
                    {
                        //Lấy lý do từ chối từ note mới nhất
                        var lydotuchoi = context.Note.Where(x => x.ObjectNumber == _model.YeuCauCapPhatTaiSanId && x.ObjectType == NoteObjectType.YCCAPPHAT).OrderByDescending(x => x.CreatedDate).First().Description;
                        if (!string.IsNullOrEmpty(lydotuchoi))
                        {
                            result = result.Replace(LyDoTuChoi, lydotuchoi);
                        }
                        else
                        {
                            result = result.Replace(LyDoTuChoi, "");
                        }
                    }

                }
            }
            // Chuyển ứng viên từ CMS qua
            else if (typeModel == TypeModel.CandidateCMS)
            {
                var _model = model as Candidate;

                if (_model != null)
                {
                    if (result.Contains(Logo))
                    {
                        var logo = configEntity.FirstOrDefault(w => w.SystemKey == "Logo").SystemValueString;

                        if (!String.IsNullOrEmpty(logo))
                        {
                            var temp_logo = "<img src=\"" + logo +
                                            "\" class=\"e - rte - image e - imginline\" alt=\"Logo TNM.png\" width=\"auto\" height=\"auto\" style=\"min - width: 0px; max - width: 750px; min - height: 0px; \">";
                            result = result.Replace(Logo, temp_logo);
                        }
                        else
                        {
                            result = result.Replace(Logo, "");
                        }
                    }

                    if (result.Contains(NgaySinh))
                    {
                        if (_model.DateOfBirth != null)
                        {
                            result = result.Replace(NgaySinh, _model.DateOfBirth.ToString("dd/MM/yyyy"));
                        }
                        else
                        {
                            result = result.Replace(NgaySinh, "");
                        }
                    }
                    if (result.Contains(TenUngVien))
                    {
                        if (!string.IsNullOrEmpty(_model.FullName))
                        {
                            result = result.Replace(TenUngVien, _model.FullName);
                        }
                        else
                        {
                            result = result.Replace(TenUngVien, "");
                        }
                    }
                    if (result.Contains(CandidateEmail))
                    {
                        if (!string.IsNullOrEmpty(_model.Email))
                        {
                            result = result.Replace(CandidateEmail, _model.Email);
                        }
                        else
                        {
                            result = result.Replace(CandidateEmail, "");
                        }
                    }
                    if (result.Contains(CandidatePhone))
                    {
                        if (!string.IsNullOrEmpty(_model.Phone))
                        {
                            result = result.Replace(CandidatePhone, _model.Phone);
                        }
                        else
                        {
                            result = result.Replace(CandidatePhone, "");
                        }
                    }
                }
            }
            return result;
        }

        private static string FormatDateToString(DateTime? date)
        {
            var result = "";

            if (date != null)
            {
                result = date.Value.Day.ToString("00") + "/" +
                         date.Value.Month.ToString("00") + "/" +
                         date.Value.Year.ToString("0000") + " " +
                         date.Value.Hour.ToString("00") + ":" +
                         date.Value.Minute.ToString("00");
            }

            return result;
        }

        private static List<Guid> GetListEmployeeApproved(TNTN8Context context, string WorkflowCode, int? StepNumber, List<Employee> ListAllEmployee)
        {
            var result = new List<Guid>();

            var workflow = context.WorkFlows.FirstOrDefault(x => x.WorkflowCode == WorkflowCode);

            if (workflow != null)
            {
                var workflowSteps = context.WorkFlowSteps.Where(x => x.WorkflowId == workflow.WorkFlowId).ToList();

                if (workflowSteps.Count > 0)
                {
                    //Nếu đối tượng có trạng thái Mới tạo (StepNumber == null) hoặc có trạng thái Từ chối (StepNumber == 0)
                    if (StepNumber == 0 || StepNumber == null)
                    {
                        var workflowStep = workflowSteps.FirstOrDefault(x => x.StepNumber == 2);

                        if (workflowStep != null)
                        {
                            bool approvalByPosition = workflowStep.ApprovebyPosition;

                            if (approvalByPosition)
                            {
                                #region Lấy danh sách người phê duyệt theo chức vụ

                                var positionId = workflowStep.ApproverPositionId;

                                if (positionId != null && positionId != Guid.Empty)
                                {
                                    result = ListAllEmployee.Where(x => x.PositionId == positionId).Select(y => y.EmployeeId)
                                        .ToList();
                                }

                                #endregion
                            }
                            else
                            {
                                #region Lấy người phê duyệt theo chỉ định

                                var approvedId = workflowStep.ApproverId;

                                if (approvedId != null && approvedId != Guid.Empty)
                                {
                                    result.Add(approvedId.Value);
                                }

                                #endregion
                            }
                        }
                    }
                    else
                    {
                        var workflowStep = workflowSteps.FirstOrDefault(x => x.StepNumber == StepNumber);

                        if (workflowStep != null)
                        {
                            bool approvalByPosition = workflowStep.ApprovebyPosition;

                            if (approvalByPosition)
                            {
                                #region Lấy danh sách người phê duyệt theo chức vụ

                                var positionId = workflowStep.ApproverPositionId;

                                if (positionId != null && positionId != Guid.Empty)
                                {
                                    result = ListAllEmployee.Where(x => x.PositionId == positionId).Select(y => y.EmployeeId)
                                        .ToList();
                                }

                                #endregion
                            }
                            else
                            {
                                #region Lấy người phê duyệt theo chỉ định

                                var approvedId = workflowStep.ApproverId;

                                if (approvedId != null && approvedId != Guid.Empty)
                                {
                                    result.Add(approvedId.Value);
                                }

                                #endregion
                            }
                        }
                    }
                }
            }

            return result;
        }

        private static decimal CalculatorTotalPurchaseProduct(decimal amount, bool discountType, decimal discountValue)
        {
            decimal result = 0;

            if (discountType)
            {
                //Chiết khấu được tính theo %
                result = amount - (amount * discountValue) / 100;
            }
            else
            {
                //Chiết khấu được tính theo tiền mặt
                result = amount - discountValue;
            }

            return result;
        }

        private static string LayEmaiNguoiTao(List<User> listAllUser, List<Contact> listAllContact,
           Guid CreatedById, int type = 1)
        {
            var Email = "";
            //CreatedById là EmployeeId
            if (type == 0)
            {
                if (CreatedById != null)
                {
                    var contact = listAllContact.FirstOrDefault(x => x.ObjectId == CreatedById);
                    if (!String.IsNullOrEmpty(contact.WorkEmail))
                    {
                        Email = contact.WorkEmail.Trim();
                    }
                    else if (!String.IsNullOrEmpty(contact.Email))
                    {
                        Email = contact.Email.Trim();
                    }
                }
            }
            //CreatedById là UserId
            if (type == 1)
            {
                //Người tạo
                var userId = listAllUser.FirstOrDefault(x => x.UserId == CreatedById)?.UserId;

                if (userId != null)
                {
                    var contact = listAllContact.FirstOrDefault(x => x.ObjectId == userId);
                    if (contact != null)
                    {
                        if (!String.IsNullOrEmpty(contact.WorkEmail))
                        {
                            Email = contact.WorkEmail.Trim();
                        }
                        else if (!String.IsNullOrEmpty(contact.Email))
                        {
                            Email = contact.Email.Trim();
                        }
                    }
                }
            }
            return Email;
        }

        private static List<string> LayEmaiNguoiDacBiet(List<User> listAllUser, List<Contact> listAllContact, List<NotifiSpecial> listNotifiSpecial, NotifiSetting notifiSetting)
        {
            var listEmail = new List<string>();
            var listEmployeeId = listNotifiSpecial
                                            .Where(x => x.NotifiSettingId == notifiSetting.NotifiSettingId)
                                            .Select(y => y.EmployeeId).ToList();

            var listEmailSpecial = listAllContact.Where(x =>
                    listEmployeeId.Contains(x.ObjectId) && x.ObjectType == "EMP")
                .ToList();

            listEmailSpecial.ForEach(email =>
            {
                if (!String.IsNullOrEmpty(email.WorkEmail))
                {
                    listEmail.Add(email.WorkEmail.Trim());
                }
                else if (!String.IsNullOrEmpty(email.Email))
                {
                    listEmail.Add(email.Email.Trim());
                }
            });
            return listEmail;
        }
    }
}
