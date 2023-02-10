using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SelectPdf;
using TN.TNM.Common;
using TN.TNM.Common.NotificationSetting;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Helper;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.ReceiptInvoice;
using TN.TNM.DataAccess.Messages.Results.ReceiptInvoice;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Customer;
using TN.TNM.DataAccess.Models.Employee;
using TN.TNM.DataAccess.Models.Order;
using TN.TNM.DataAccess.Models.ReceiptInvoice;
using SystemParameter = TN.TNM.DataAccess.Databases.Entities.SystemParameter;

//using Syncfusion.Pdf;

namespace TN.TNM.DataAccess.Databases.DAO
{
    public class ReceiptInvoiceDAO : BaseDAO, IReceiptInvoiceDataAccess
    {
        private IConverter _converter;
        private readonly IHostingEnvironment _hostingEnvironment;
        public ReceiptInvoiceDAO(TNTN8Context _content, IAuditTraceDataAccess _iAuditTrace, IConverter converter, IHostingEnvironment hostingEnvironment)
        {
            context = _content;
            iAuditTrace = _iAuditTrace;
            iAuditTrace = _iAuditTrace;
            converter = _converter;
            _hostingEnvironment = hostingEnvironment;
        }

        public CreateReceiptInvoiceResult CreateReceiptInvoice(CreateReceiptInvoiceParameter parameter)
        {

            if (parameter.ReceiptInvoice.Amount <= 0)
            {
                return new CreateReceiptInvoiceResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = "Thành tiền không được âm hoặc bằng 0"
                };
            }
            if (parameter.ReceiptInvoice.UnitPrice <= 0)
            {
                return new CreateReceiptInvoiceResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = "Số tiền không được âm hoặc bằng 0"
                };
            }
            if (parameter.ReceiptInvoice.ExchangeRate <= 0)
            {
                return new CreateReceiptInvoiceResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = "Tỉ giá không được âm hoặc bằng 0"
                };
            }

            var cusEmail = context.Contact.FirstOrDefault(x =>
                x.ObjectId == parameter.ReceiptInvoiceMapping.ObjectId && x.ObjectType == "CUS")?.Email;
            if (string.IsNullOrEmpty(cusEmail) && parameter.ReceiptInvoice.IsSendMail == true)
            {
                return new CreateReceiptInvoiceResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = "Vui lòng cập nhật thông tin Email doanh nghiệp hoặc bỏ tick checkbox 'Thông báo cho khách hàng'."
                };
            }

            var newReceiptInvoice = parameter.ReceiptInvoice;
            var newReceiptInvoiceMapping = parameter.ReceiptInvoiceMapping;
            var organizationCode = context.Organization
                .FirstOrDefault(o => o.OrganizationId == parameter.ReceiptInvoice.OrganizationId)?.OrganizationCode;

            if (newReceiptInvoice.UnitPrice <= 0)
            {
                return new CreateReceiptInvoiceResult
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = "Số tiền không được là số âm hoặc bằng 0"
                };
            }
            if (newReceiptInvoice.ExchangeRate != null)
            {
                if (newReceiptInvoice.ExchangeRate <= 0)
                {
                    return new CreateReceiptInvoiceResult
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = "Tỷ giá không được là số âm hoặc bằng 0"
                    };
                }
            }

            newReceiptInvoice.ReceiptInvoiceId = Guid.NewGuid();
            newReceiptInvoice.CreatedById = parameter.UserId;
            newReceiptInvoice.CreatedDate = DateTime.Now;
            newReceiptInvoice.ReceiptInvoiceCode = "PT" + "-" + organizationCode + DateTime.Now.Year
                + (context.ReceiptInvoice.Count(r => r.CreatedDate.Year == DateTime.Now.Year) + 1).ToString("D5");
            var receiptInvoiceDupblicase =
                context.ReceiptInvoice.FirstOrDefault(x =>
                    x.ReceiptInvoiceCode == newReceiptInvoice.ReceiptInvoiceCode);
            if (receiptInvoiceDupblicase != null)
            {
                return new CreateReceiptInvoiceResult
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = "Mã phiếu thu đã tồn tại"
                };
            }
            newReceiptInvoiceMapping.ReceiptInvoiceMappingId = Guid.NewGuid();
            newReceiptInvoiceMapping.ReceiptInvoiceId = parameter.ReceiptInvoice.ReceiptInvoiceId.Value;
            newReceiptInvoiceMapping.CreatedById = parameter.UserId;
            newReceiptInvoiceMapping.CreatedDate = DateTime.Now;
            newReceiptInvoiceMapping.ObjectId = parameter.ReceiptInvoiceMapping.ObjectId;

            var statusIP = context.OrderStatus.FirstOrDefault(o => o.OrderStatusCode == "COMP").OrderStatusId;

            parameter.ReceiptOrderHistory.ForEach(item =>
            {
                if (item.AmountCollected > 0)
                {
                    var orderHistory = new ReceiptOrderHistory()
                    {
                        ReceiptOrderHistoryId = Guid.NewGuid(),
                        ObjectId = newReceiptInvoice.ReceiptInvoiceId.Value,
                        ObjectType = "THU",
                        OrderId = item.OrderId,
                        AmountCollected = item.AmountCollected,
                        CreatedDate = DateTime.Now,
                        CreatedById = parameter.UserId
                    };
                    context.ReceiptOrderHistory.Add(orderHistory);

                    if (item.AmountCollected == item.Amount)
                    {
                        var customerOrder = context.CustomerOrder.FirstOrDefault(c => c.OrderId == item.OrderId);
                        if (customerOrder != null)
                        {
                            customerOrder.StatusId = statusIP;
                            context.CustomerOrder.Update(customerOrder);
                        }
                    }
                }
            });

            context.ReceiptInvoiceMapping.Add(newReceiptInvoiceMapping.ToEntity());
            context.ReceiptInvoice.Add(newReceiptInvoice.ToEntity());

            context.SaveChanges();

            #region Thong bao mail cho khach hang neu co tick checkbox

            if (parameter.ReceiptInvoice.IsSendMail == true)
            {
                if (parameter.ReceiptOrderHistory.Count <= 1)
                {
                    Helper.NotificationHelper.AccessNotification(context, TypeModel.CashReceipts,
                        parameter.OrderId == null ? "PAY_CRE" : "PAY_ORDER_CRE", new BankReceiptInvoice(),
                        newReceiptInvoice.ToEntity(),
                        true);
                }
                else
                {
                    var configEntity = context.SystemParameter.ToList();

                    #region Kiểm tra xem đã có cấu hình cho thông báo chưa?

                    var screenId = context.Screen.FirstOrDefault(x => x.ScreenCode == TypeModel.BankReceipts)?.ScreenId;
                    var NotiActionId = context.NotifiAction.FirstOrDefault(x => x.NotifiActionCode == "PAY_ORDER_MULTI" && x.ScreenId == screenId)
                        ?.NotifiActionId;

                    var notifiSetting =
                        context.NotifiSetting.FirstOrDefault(x => x.ScreenId == screenId &&
                                                                  x.NotifiActionId == NotiActionId && x.Active);

                    #endregion

                    if (notifiSetting != null)
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

                                    #region Lấy email khách hàng

                                    if (!string.IsNullOrEmpty(cusEmail))
                                    {
                                        listEmailSendTo.Add((cusEmail));
                                    }

                                    #endregion

                                    #region Lấy email người tham gia (khong co nguoi tham gia)

                                    if (notifiSetting.IsParticipant)
                                    {

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
                                        //Người tạo
                                        var employeeId =
                                            context.User.FirstOrDefault(x => x.UserId == parameter.ReceiptInvoice.CreatedById)
                                                ?.EmployeeId;

                                        var email_created = "";

                                        if (employeeId != null)
                                        {
                                            email_created = listAllContact.FirstOrDefault(x =>
                                                x.ObjectId == employeeId && x.ObjectType == "EMP")?.Email;

                                            if (!String.IsNullOrEmpty(email_created))
                                            {
                                                listEmailSendTo.Add(email_created.Trim());
                                            }
                                        }
                                    }

                                    #endregion

                                    #region Lấy email người phụ trách (khong co nguoi phu trach)



                                    #endregion

                                    #region Lấy email của danh sách người đặc biệt

                                    var listEmployeeId = context.NotifiSpecial
                                        .Where(x => x.NotifiSettingId == notifiSetting.NotifiSettingId)
                                        .Select(y => y.EmployeeId).ToList();

                                    var listEmailSpecial = listAllContact.Where(x =>
                                            listEmployeeId.Contains(x.ObjectId) && x.ObjectType == "EMP")
                                        .Select(y => y.Email)
                                        .ToList();

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
                                    var subject = ReplaceTokenForContent(context, TypeModel.CashReceipts, newReceiptInvoice.ToEntity(),
                                        notifiSetting.EmailTitle, configEntity, parameter.ReceiptOrderHistory);
                                    var content = ReplaceTokenForContent(context, TypeModel.CashReceipts, newReceiptInvoice.ToEntity(),
                                        notifiSetting.EmailContent, configEntity, parameter.ReceiptOrderHistory);

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
            }

            #endregion

            return new CreateReceiptInvoiceResult
            {
                StatusCode = HttpStatusCode.OK,
                MessageCode = CommonMessage.ReceiptInvoice.ADD_SUCCESS
            };
        }

        public GetReceiptInvoiceByIdResult GetReceiptInvoiceById(GetReceiptInvoiceByIdParameter parameter)
        {
            try
            {
                var reasonCategoryTypeId =
                    context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "LTH").CategoryTypeId;
                var listAllReason = context.Category.Where(c => c.CategoryTypeId == reasonCategoryTypeId).ToList();

                var statusCateoryTypeId =
                    context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "TCH").CategoryTypeId;
                var listAllStatus = context.Category.Where(x => x.CategoryTypeId == statusCateoryTypeId).ToList();

                var listAllOrg = context.Organization.ToList();

                var currencyUnitId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "DTI").CategoryTypeId;
                var listAllCurrencyUnit = context.Category.Where(x => x.CategoryTypeId == currencyUnitId).ToList();

                var receiptInvoice = context.ReceiptInvoice.FirstOrDefault(c =>
                    parameter.ReceiptInvoiceId == null || parameter.ReceiptInvoiceId == Guid.Empty ||
                    c.ReceiptInvoiceId == parameter.ReceiptInvoiceId);

                var receiptInvoiceMapping = context.ReceiptInvoiceMapping.FirstOrDefault(c =>
                    parameter.ReceiptInvoiceId == null || parameter.ReceiptInvoiceId == Guid.Empty ||
                    c.ReceiptInvoiceId == parameter.ReceiptInvoiceId);

                ReceiptInvoiceEntityModel data = new ReceiptInvoiceEntityModel();
                data.ReceiptInvoiceId = receiptInvoice.ReceiptInvoiceId;
                data.ReceiptInvoiceCode = receiptInvoice.ReceiptInvoiceCode;
                data.ReceiptInvoiceDetail = receiptInvoice.ReceiptInvoiceDetail;
                data.ReceiptInvoiceReason = receiptInvoice.ReceiptInvoiceReason;
                data.ReceiptInvoiceNote = receiptInvoice.ReceiptInvoiceNote;
                data.RegisterType = receiptInvoice.RegisterType;
                data.OrganizationId = receiptInvoice.OrganizationId;
                data.StatusId = receiptInvoice.StatusId;
                data.RecipientName = receiptInvoice.RecipientName;
                data.RecipientAddress = receiptInvoice.RecipientAddress;
                data.UnitPrice = receiptInvoice.UnitPrice;
                data.CurrencyUnit = receiptInvoice.CurrencyUnit;
                data.ExchangeRate = receiptInvoice.ExchangeRate;
                data.Amount = receiptInvoice.Amount;
                data.AmountText = receiptInvoice.AmountText;
                data.Active = receiptInvoice.Active;
                data.CreatedById = receiptInvoice.CreatedById;
                data.CreatedDate = receiptInvoice.CreatedDate;
                data.UpdatedById = receiptInvoice.UpdatedById;
                data.UpdatedDate = receiptInvoice.UpdatedDate;
                data.ObjectId = receiptInvoiceMapping?.ObjectId;
                data.ReceiptDate = receiptInvoice.ReceiptDate;
                data.IsSendMail = receiptInvoice.IsSendMail;
                data.StatusName = listAllStatus.FirstOrDefault(c => c.CategoryId == receiptInvoice.StatusId)?.CategoryName;
                data.StatusCode = listAllStatus.FirstOrDefault(c => c.CategoryId == receiptInvoice.StatusId)?.CategoryCode;
                data.AmountText = MoneyHelper.Convert((decimal)data.Amount);
                data.NameCreateBy = GetCreateByName(receiptInvoice.CreatedById);
                data.NameObjectReceipt = GetObjectName(data.ObjectId);
                if (data.ReceiptInvoiceReason != null && data.ReceiptInvoiceReason != Guid.Empty)
                {
                    data.NameReceiptInvoiceReason = listAllReason.FirstOrDefault(c => c.CategoryId == receiptInvoice.ReceiptInvoiceReason).CategoryName;
                }
                if (data.OrganizationId != null && data.OrganizationId != Guid.Empty)
                {
                    data.OrganizationName = listAllOrg.FirstOrDefault(c => c.OrganizationId == data.OrganizationId).OrganizationName;
                }
                if (data.CurrencyUnit != null && data.CurrencyUnit != Guid.Empty)
                {
                    data.CurrencyUnitName = listAllCurrencyUnit.FirstOrDefault(c => c.CategoryId == data.CurrencyUnit).CategoryName;
                }

                return new GetReceiptInvoiceByIdResult
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success",
                    ReceiptInvoice = data
                };
            }
            catch (Exception e)
            {
                return new GetReceiptInvoiceByIdResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public SearchReceiptInvoiceResult SearchReceiptInvoice(SearchReceiptInvoiceParameter parameter)
        {
            try
            {
                var reasonCategoryTypeId =
                    context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "LTH").CategoryTypeId;
                var listAllReason = context.Category.Where(c => c.CategoryTypeId == reasonCategoryTypeId).ToList();

                var statusCategoryTypeId =
                    context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "TCH").CategoryTypeId;
                var listAllStatus = context.Category.Where(c => c.CategoryTypeId == statusCategoryTypeId).ToList();

                var receiptInvoiceCode =
                    parameter.ReceiptInvoiceCode == null ? "" : parameter.ReceiptInvoiceCode.Trim();
                var receiptInvoiceReason = parameter.ReceiptInvoiceReason;
                var objectIds = parameter.ObjectReceipt;
                var listIdUser = parameter.CreateById;
                var createdByIds = new List<Guid>();

                if (listIdUser != null)
                {
                    foreach (var item in listIdUser)
                    {
                        var temp = context.User.FirstOrDefault(u => u.EmployeeId == item).UserId;
                        createdByIds.Add(temp);
                    }
                }
                else
                {
                    createdByIds = listIdUser;
                }

                var fromDate = parameter.CreateDateFrom;
                var toDate = parameter.CreateDateTo;
                var statusIds = parameter.Status;

                var lstReceiptMap = context.ReceiptInvoiceMapping.Where(c =>
                    objectIds == null || objectIds.Count == 0 || objectIds.Contains(c.ObjectId)).ToList();
                var lstReceiptMapId = lstReceiptMap.Select(c => c.ReceiptInvoiceId).ToList();

                var lst = context.ReceiptInvoice
                    .Where(x => (receiptInvoiceCode == "" || x.ReceiptInvoiceCode.Contains(receiptInvoiceCode)) &&
                                (receiptInvoiceReason == null || receiptInvoiceReason.Count == 0 ||
                                 receiptInvoiceReason.Contains(x.ReceiptInvoiceReason.Value)) &&
                                (lstReceiptMapId.Contains(x.ReceiptInvoiceId)) &&
                                (createdByIds == null || createdByIds.Count == 0 ||
                                 createdByIds.Contains(x.CreatedById)) &&
                                (fromDate == null || fromDate == DateTime.MinValue || fromDate <= x.CreatedDate) &&
                                (toDate == null || toDate == DateTime.MinValue || toDate >= x.CreatedDate) &&
                                (statusIds == null || statusIds.Count == 0 || statusIds.Contains(x.StatusId)))
                    .Select(m => new ReceiptInvoiceEntityModel
                    {
                        ReceiptInvoiceId = m.ReceiptInvoiceId,
                        ReceiptInvoiceCode = m.ReceiptInvoiceCode,
                        ReceiptInvoiceDetail = m.ReceiptInvoiceDetail,
                        ReceiptInvoiceReason = m.ReceiptInvoiceReason,
                        ReceiptInvoiceNote = m.ReceiptInvoiceNote,
                        RegisterType = m.RegisterType,
                        OrganizationId = m.OrganizationId,
                        StatusId = m.StatusId,
                        RecipientName = m.RecipientName,
                        RecipientAddress = m.RecipientAddress,
                        UnitPrice = m.UnitPrice,
                        CurrencyUnit = m.CurrencyUnit,
                        ExchangeRate = m.ExchangeRate,
                        Amount = m.Amount,
                        AmountText = m.AmountText,
                        Active = m.Active,
                        CreatedById = m.CreatedById,
                        CreatedDate = m.CreatedDate,
                        UpdatedById = m.UpdatedById,
                        UpdatedDate = m.UpdatedDate,
                        NameReceiptInvoiceReason = listAllReason
                            .FirstOrDefault(s => s.CategoryId == m.ReceiptInvoiceReason).CategoryName,
                        ReceiptDate = m.ReceiptDate,
                        StatusName = listAllStatus.FirstOrDefault(c => c.CategoryId == m.StatusId).CategoryName,
                    }).ToList();

                lst.ForEach(item =>
                {
                    var temp = lstReceiptMap.FirstOrDefault(c => c.ReceiptInvoiceId == item.ReceiptInvoiceId);
                    if (temp != null)
                    {
                        item.ObjectId = temp.ObjectId;
                        item.NameObjectReceipt = GetObjectName(temp.ObjectId);
                    }
                    item.NameCreateBy = GetCreateByName(item.CreatedById);
                    if (item.StatusId != null && item.StatusId != Guid.Empty)
                    {
                        var status = listAllStatus.FirstOrDefault(c => c.CategoryId == item.StatusId);
                        switch (status.CategoryCode)
                        {
                            case "CSO":
                                item.BackgroundColorForStatus = "#ffcc00";
                                break;
                            case "DSO":
                                item.BackgroundColorForStatus = "#007aff";
                                break;
                        }
                    }
                });

                lst = lst.OrderByDescending(x => x.CreatedDate).ToList();

                return new SearchReceiptInvoiceResult
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success",
                    lstReceiptInvoiceEntity = lst
                };
            }
            catch (Exception ex)
            {
                return new SearchReceiptInvoiceResult
                {
                    MessageCode = ex.Message,
                    StatusCode = HttpStatusCode.ExpectationFailed
                };
            }
        }

        public SearchCashBookReceiptInvoiceResult SearchCashBookReceiptInvoice(SearchCashBookReceiptInvoiceParameter parameter)
        {
            try
            {
                var reasonCategoryTypeId =
                    context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "LTH").CategoryTypeId;
                var listAllReason = context.Category.Where(c => c.CategoryTypeId == reasonCategoryTypeId);

                var listIdUser = parameter.CreateById;
                var createdByIds = new List<Guid>();

                if (listIdUser != null)
                {
                    foreach (var item in listIdUser)
                    {
                        var temp = context.User.FirstOrDefault(u => u.EmployeeId == item).UserId;
                        createdByIds.Add(temp);
                    }
                }
                else
                {
                    createdByIds = listIdUser;
                }
                var fromDate = parameter.ReceiptDateFrom;
                var toDate = parameter.ReceiptDateTo;
                var organizations = parameter.OrganizationList;
                var receiptInvoiceList = context.ReceiptInvoice
                    .Where(c =>
                        (createdByIds == null || createdByIds.Count == 0 || createdByIds.Contains(c.CreatedById)) &&
                        (organizations == null || organizations.Count == 0 ||
                         organizations.Contains(c.OrganizationId)) &&
                        (fromDate == null || fromDate == DateTime.MinValue || fromDate <= c.CreatedDate) &&
                        (toDate == null || toDate == DateTime.MinValue || toDate >= c.CreatedDate))
                    .Select(v => new ReceiptInvoiceEntityModel
                    {
                        Active = v.Active,
                        Amount = v.Amount,
                        CreatedById = v.CreatedById,
                        CreatedDate = v.CreatedDate,
                        CurrencyUnit = v.CurrencyUnit,
                        ExchangeRate = v.ExchangeRate,
                        OrganizationId = v.OrganizationId,
                        ReceiptInvoiceCode = v.ReceiptInvoiceCode,
                        ReceiptInvoiceDetail = v.ReceiptInvoiceDetail,
                        ReceiptInvoiceId = v.ReceiptInvoiceId,
                        ReceiptInvoiceNote = v.ReceiptInvoiceNote ?? "",
                        ReceiptInvoiceReason = v.ReceiptInvoiceReason,
                        ReceiptDate = v.ReceiptDate,
                        RecipientAddress = v.RecipientAddress ?? "",
                        RecipientName = v.RecipientName ?? "",
                        RegisterType = v.RegisterType,
                        StatusId = v.StatusId,
                        UnitPrice = v.UnitPrice,
                        UpdatedById = v.UpdatedById,
                        UpdatedDate = v.UpdatedDate,
                        NameReceiptInvoiceReason =
                            listAllReason.FirstOrDefault(c => c.CategoryId == v.ReceiptInvoiceReason).CategoryName ?? ""
                    }).ToList();

                receiptInvoiceList.ForEach(item =>
                {
                    item.NameCreateBy = GetCreateByName(item.CreatedById);
                });

                receiptInvoiceList.OrderByDescending(c => c.CreatedDate).ToList();

                return new SearchCashBookReceiptInvoiceResult
                {
                    MessageCode = "Success!",
                    StatusCode = HttpStatusCode.OK,
                    lstReceiptInvoiceEntity = receiptInvoiceList,
                };
            }
            catch (Exception ex)
            {
                return new SearchCashBookReceiptInvoiceResult
                {
                    MessageCode = ex.Message,
                    StatusCode = HttpStatusCode.ExpectationFailed
                };
            }
        }

        public SearchBankReceiptInvoiceResult SearchBankReceiptInvoice(SearchBankReceiptInvoiceParameter parameter)
        {
            try
            {
                var reasonCategoryTypeId =
                    context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "LTH").CategoryTypeId;
                var listAllReason = context.Category.Where(c => c.CategoryTypeId == reasonCategoryTypeId);

                var statusCateoryTypeId =
                    context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "TCH").CategoryTypeId;
                var listAllStatus = context.Category.Where(x => x.CategoryTypeId == statusCateoryTypeId).ToList();

                var bankReceiptInvoice = parameter.ReceiptInvoiceCode == null ? "" : parameter.ReceiptInvoiceCode;
                var bankReceiptInvoiceReasonIds = parameter.ReceiptReasonIdList;
                var objectIds = parameter.ObjectIdList;
                var listIdUser = parameter.CreatedByIdList;
                var createdByIds = new List<Guid>();

                if (listIdUser != null)
                {
                    foreach (var item in listIdUser)
                    {
                        var temp = context.User.FirstOrDefault(u => u.EmployeeId == item).UserId;
                        createdByIds.Add(temp);
                    }
                }
                else
                {
                    createdByIds = listIdUser;
                }

                var fromDate = parameter.FromDate;
                var toDate = parameter.ToDate;
                var statusIds = parameter.SttList;
                var lstBankReceiptMap = context.BankReceiptInvoiceMapping
                    .Where(c => objectIds == null || objectIds.Count == 0 || objectIds.Contains(c.ObjectId.Value)).ToList();
                var lstBankReceiptMapId = lstBankReceiptMap.Select(c => c.BankReceiptInvoiceId).ToList();

                var lst = context.BankReceiptInvoice
                    .Where(x => (bankReceiptInvoice == "" || x.BankReceiptInvoiceCode.Contains(bankReceiptInvoice)) &&
                                (bankReceiptInvoiceReasonIds == null || bankReceiptInvoiceReasonIds.Count == 0 ||
                                 bankReceiptInvoiceReasonIds.Contains(x.BankReceiptInvoiceReason.Value)) &&
                                (lstBankReceiptMapId.Contains(x.BankReceiptInvoiceId)) &&
                                (createdByIds == null || createdByIds.Count == 0 ||
                                 createdByIds.Contains(x.CreatedById)) &&
                                (fromDate == null || fromDate == DateTime.MinValue || fromDate <= x.CreatedDate) &&
                                (toDate == null || toDate == DateTime.MinValue || toDate >= x.CreatedDate) &&
                                (statusIds == null || statusIds.Count == 0 || statusIds.Contains(x.StatusId.Value)))
                    .Select(m => new BankReceiptInvoiceEntityModel
                    {
                        BankReceiptInvoiceId = m.BankReceiptInvoiceId,
                        BankReceiptInvoiceCode = m.BankReceiptInvoiceCode,
                        BankReceiptInvoiceDetail = m.BankReceiptInvoiceDetail,
                        BankReceiptInvoicePrice = m.BankReceiptInvoicePrice,
                        BankReceiptInvoicePriceCurrency = m.BankReceiptInvoicePriceCurrency,
                        BankReceiptInvoiceExchangeRate = m.BankReceiptInvoiceExchangeRate,
                        BankReceiptInvoiceReason = m.BankReceiptInvoiceReason,
                        BankReceiptInvoiceNote = m.BankReceiptInvoiceNote,
                        BankReceiptInvoiceBankAccountId = m.BankReceiptInvoiceBankAccountId,
                        BankReceiptInvoiceAmount = m.BankReceiptInvoiceAmount,
                        BankReceiptInvoiceAmountText = m.BankReceiptInvoiceAmountText,
                        BankReceiptInvoicePaidDate = m.BankReceiptInvoicePaidDate,
                        OrganizationId = m.OrganizationId,
                        StatusId = m.StatusId,
                        Active = m.Active,
                        CreatedById = m.CreatedById,
                        CreatedDate = m.CreatedDate,
                        UpdatedById = m.UpdatedById,
                        UpdatedDate = m.UpdatedDate,
                        BankReceiptInvoiceReasonText = listAllReason
                            .FirstOrDefault(c => c.CategoryId == m.BankReceiptInvoiceReason).CategoryName,
                        StatusName = listAllStatus.FirstOrDefault(c => c.CategoryId == m.StatusId).CategoryName,
                    }).ToList();

                lst.ForEach(item =>
                {
                    var tmp = lstBankReceiptMap.FirstOrDefault(c => c.BankReceiptInvoiceId == item.BankReceiptInvoiceId);
                    if (tmp != null)
                    {
                        item.ObjectId = tmp.ObjectId;
                        item.ObjectName = GetObjectName(item.ObjectId);
                    }
                    item.CreatedByName = GetCreateByName(item.CreatedById);
                    if (item.StatusId != null && item.StatusId != Guid.Empty)
                    {
                        var status = listAllStatus.FirstOrDefault(c => c.CategoryId == item.StatusId);
                        switch (status.CategoryCode)
                        {
                            case "CSO":
                                item.BackgroundColorForStatus = "#ffcc00";
                                break;
                            case "DSO":
                                item.BackgroundColorForStatus = "#007aff";
                                break;
                        }
                    }
                });

                lst = lst.OrderByDescending(x => x.CreatedDate).ToList();

                return new SearchBankReceiptInvoiceResult
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success",
                    BankReceiptInvoiceList = lst
                };
            }
            catch (Exception ex)
            {
                return new SearchBankReceiptInvoiceResult
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = ex.Message
                };
            }
        }
        public CreateBankReceiptInvoiceResult CreateBankReceiptInvoice(CreateBankReceiptInvoiceParameter parameter)
        {
            try
            {
                if (parameter.BankReceiptInvoice.BankReceiptInvoiceAmount <= 0)
                {
                    return new CreateBankReceiptInvoiceResult()
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = "Thành tiền không được âm hoặc bằng 0"
                    };
                }
                if (parameter.BankReceiptInvoice.BankReceiptInvoicePrice <= 0)
                {
                    return new CreateBankReceiptInvoiceResult()
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = "Số tiền không được âm hoặc bằng 0"
                    };
                }
                if (parameter.BankReceiptInvoice.BankReceiptInvoiceExchangeRate <= 0)
                {
                    return new CreateBankReceiptInvoiceResult()
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = "Tỉ giá không được âm hoặc bằng 0"
                    };
                }

                var cusEmail = context.Contact.FirstOrDefault(x =>
                    x.ObjectId == parameter.BankReceiptInvoiceMapping.ObjectId && x.ObjectType == "CUS")?.Email;
                if (string.IsNullOrEmpty(cusEmail) && parameter.BankReceiptInvoice.IsSendMail == true)
                {
                    return new CreateBankReceiptInvoiceResult()
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode =
                            "Vui lòng cập nhật thông tin Email doanh nghiệp hoặc bỏ tick checkbox 'Thông báo cho khách hàng'."
                    };
                }

                var newBankReceiptInvoiceId = Guid.NewGuid();
                var organ = context.Organization.FirstOrDefault(or =>
                    or.OrganizationId == parameter.BankReceiptInvoice.OrganizationId);
                parameter.BankReceiptInvoice.BankReceiptInvoiceId = newBankReceiptInvoiceId;
                parameter.BankReceiptInvoice.CreatedDate = DateTime.Now;
                parameter.BankReceiptInvoice.CreatedById = parameter.UserId;
                parameter.BankReceiptInvoice.BankReceiptInvoiceCode =
                    "BC" + "-" + organ.OrganizationCode + DateTime.Now.Year +
                    (context.BankReceiptInvoice.Count(b => b.CreatedDate.Year == DateTime.Now.Year) + 1).ToString("D5");
                var bankReceiptInvoiceDupblicase = context.BankReceiptInvoice.FirstOrDefault(x =>
                    x.BankReceiptInvoiceCode == parameter.BankReceiptInvoice.BankReceiptInvoiceCode);
                if (bankReceiptInvoiceDupblicase != null)
                {
                    return new CreateBankReceiptInvoiceResult
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = "Mã báo có đã tồn tại"
                    };
                }

                var newBankReceiptInvoiceMapping = parameter.BankReceiptInvoiceMapping;
                newBankReceiptInvoiceMapping.BankReceiptInvoiceMappingId = Guid.NewGuid();
                newBankReceiptInvoiceMapping.CreatedById = parameter.UserId;
                newBankReceiptInvoiceMapping.CreatedDate = DateTime.Now;
                newBankReceiptInvoiceMapping.BankReceiptInvoiceId = newBankReceiptInvoiceId;
                newBankReceiptInvoiceMapping.ObjectId = parameter.BankReceiptInvoiceMapping.ObjectId;

                var isEmp = context.Employee.Any(e => e.EmployeeId == newBankReceiptInvoiceMapping.ObjectId);
                var isCus = context.Customer.Any(c => c.CustomerId == newBankReceiptInvoiceMapping.ObjectId);
                var isVen = context.Vendor.Any(v => v.VendorId == newBankReceiptInvoiceMapping.ObjectId);

                if (isEmp)
                {
                    newBankReceiptInvoiceMapping.ReferenceType = 1;
                }
                else if (isVen)
                {
                    newBankReceiptInvoiceMapping.ReferenceType = 2;
                }
                else if (isCus)
                {
                    newBankReceiptInvoiceMapping.ReferenceType = 3;
                }
                else
                {
                    newBankReceiptInvoiceMapping.ReferenceType = 0;
                }

                var statusPD = context.OrderStatus.FirstOrDefault(o => o.OrderStatusCode == "COMP")?.OrderStatusId;

                parameter.ReceiptOrderHistory.ForEach(item =>
                {
                    if (item.AmountCollected > 0)
                    {
                        var orderHistory = new ReceiptOrderHistory()
                        {
                            ReceiptOrderHistoryId = Guid.NewGuid(),
                            ObjectId = parameter.BankReceiptInvoice.BankReceiptInvoiceId,
                            ObjectType = "BAOCO",
                            OrderId = item.OrderId,
                            AmountCollected = item.AmountCollected,
                            CreatedDate = DateTime.Now,
                            CreatedById = parameter.UserId
                        };
                        context.ReceiptOrderHistory.Add(orderHistory);

                        if (item.AmountCollected == item.Amount)
                        {
                            var customerOrder = context.CustomerOrder.FirstOrDefault(c => c.OrderId == item.OrderId);
                            if (customerOrder != null)
                            {
                                customerOrder.StatusId = statusPD;
                                context.CustomerOrder.Update(customerOrder);
                            }
                        }
                    }
                });

                context.BankReceiptInvoice.Add(parameter.BankReceiptInvoice.ToEntity());
                context.BankReceiptInvoiceMapping.Add(newBankReceiptInvoiceMapping.ToEntity());
                context.SaveChanges();

                #region Thong bao mail cho khach hang neu co tick checkbox

                if (parameter.BankReceiptInvoice.IsSendMail == true)
                {
                    if (parameter.ReceiptOrderHistory.Count <= 1)
                    {
                        NotificationHelper.AccessNotification(context, TypeModel.BankReceipts,
                            parameter.OrderId == null ? "PAY_CRE" : "PAY_ORDER_CRE", new BankReceiptInvoice(),
                            parameter.BankReceiptInvoice.ToEntity(),
                            true);
                    }
                    else
                    {
                        var configEntity = context.SystemParameter.ToList();

                        #region Kiểm tra xem đã có cấu hình cho thông báo chưa?

                        var screenId = context.Screen.FirstOrDefault(x => x.ScreenCode == TypeModel.BankReceipts)?.ScreenId;
                        var NotiActionId = context.NotifiAction.FirstOrDefault(x =>
                                x.NotifiActionCode == "PAY_ORDER_MULTI" && x.ScreenId == screenId)
                            ?.NotifiActionId;

                        var notifiSetting =
                            context.NotifiSetting.FirstOrDefault(x => x.ScreenId == screenId &&
                                                                      x.NotifiActionId == NotiActionId && x.Active);

                        #endregion

                        if (notifiSetting != null)
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

                                        #region Lấy email khách hàng

                                        if (!string.IsNullOrEmpty(cusEmail))
                                        {
                                            listEmailSendTo.Add((cusEmail));
                                        }

                                        #endregion

                                        #region Lấy email người tham gia (khong co nguoi tham gia)

                                        if (notifiSetting.IsParticipant)
                                        {

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
                                            //Người tạo
                                            var employeeId =
                                                context.User.FirstOrDefault(x => x.UserId == parameter.BankReceiptInvoice.CreatedById)
                                                    ?.EmployeeId;

                                            var email_created = "";

                                            if (employeeId != null)
                                            {
                                                email_created = listAllContact.FirstOrDefault(x =>
                                                    x.ObjectId == employeeId && x.ObjectType == "EMP")?.Email;

                                                if (!String.IsNullOrEmpty(email_created))
                                                {
                                                    listEmailSendTo.Add(email_created.Trim());
                                                }
                                            }
                                        }

                                        #endregion

                                        #region Lấy email người phụ trách (khong co nguoi phu trach)


                                        #endregion

                                        #region Lấy email của danh sách người đặc biệt

                                        var listEmployeeId = context.NotifiSpecial
                                            .Where(x => x.NotifiSettingId == notifiSetting.NotifiSettingId)
                                            .Select(y => y.EmployeeId).ToList();

                                        var listEmailSpecial = listAllContact.Where(x =>
                                                listEmployeeId.Contains(x.ObjectId) && x.ObjectType == "EMP")
                                            .Select(y => y.Email)
                                            .ToList();

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
                                        var subject = ReplaceTokenForContent(context, TypeModel.BankReceipts, parameter.BankReceiptInvoice,
                                            notifiSetting.EmailTitle, configEntity, parameter.ReceiptOrderHistory);
                                        var content = ReplaceTokenForContent(context, TypeModel.BankReceipts, parameter.BankReceiptInvoice,
                                            notifiSetting.EmailContent, configEntity, parameter.ReceiptOrderHistory);

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
                }

                #endregion

                return new CreateBankReceiptInvoiceResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = CommonMessage.BankReceiptInvoice.ADD_SUCCESS
                };
            }
            catch (Exception e)
            {
                return new CreateBankReceiptInvoiceResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        private static string ReplaceTokenForContent(TNTN8Context context, string typeModel, object model,
            string emailContent, List<SystemParameter> configEntity, List<ReceiptHistoryEntityModel> receiptOrderHistory)
        {
            var result = emailContent;

            var listOrder = context.CustomerOrder.Select(x => new CustomerOrderEntityModel
            {
                OrderId = x.OrderId,
                OrderCode = x.OrderCode,
                OrderDate = x.OrderDate,
            }).OrderBy(y => y.OrderCode).ToList();
            var listOrderId = receiptOrderHistory.Select(x => x.OrderId).ToList();
            listOrder = listOrder.Where(x => listOrderId.Contains(x.OrderId.Value)).ToList();

            #region Common Token

            const string Logo = "[LOGO]";
            const string CustomerName = "[CUSTOMER_NAME]";

            #endregion

            #region Receipt Invoice

            const string totalAmountPay = "[TOTAL_AMOUNT_PAY]";
            const string companyName = "[COMPANY_NAME]";
            const string remainBalance = "[REMAIN_BALANCE]";
            const string amountPay = "[AMOUNT_PAY]";
            const string orderCode = "[ORDER_CODE]";
            const string orderDate = "[ORDER_DATE]";
            const string invoiceCode = "[INVOICE_CODE]";
            const string orderNumber = "[ORDER_NUMBER]";

            #endregion

            //Tạo phiếu thu
            if (typeModel == TypeModel.CashReceipts)
            {
                var _model = model as ReceiptInvoice;

                var customerId = context.ReceiptInvoiceMapping
                    .FirstOrDefault(x => x.ReceiptInvoiceId == _model.ReceiptInvoiceId)?.ObjectId;

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

                if (result.Contains(orderNumber))
                {
                    var str = "";
                    for (var i = 0; i < receiptOrderHistory.Count; i++)
                    {
                        var stt = i + 1;
                        str += "<p>" + stt.ToString() + "</p>";
                    }

                    result = result.Replace(orderNumber, str);
                }

                if (result.Contains(amountPay))
                {
                    var amountPayString = "";
                    receiptOrderHistory.ForEach(item =>
                    {
                        var amount = item.Amount.ToString("C2", new CultureInfo("vi-VN"));
                        amountPayString += $"<p>{amount}</p>";
                    });

                    result = result.Replace(amountPay, amountPayString);
                }

                if (result.Contains(totalAmountPay) && _model.Amount != null)
                {
                    var totalAmountPayString = "";
                    receiptOrderHistory.ForEach(item =>
                    {
                        var amount = item.AmountCollected.ToString("C2", new CultureInfo("vi-VN"));
                        totalAmountPayString += $"<p>{amount}</p>";
                    });

                    result = result.Replace(totalAmountPay, totalAmountPayString);
                }

                if (result.Contains(remainBalance) && _model.Amount != null)
                {
                    var remainBalanceString = "";
                    receiptOrderHistory.ForEach(item =>
                    {
                        var amount = item.Amount - item.AmountCollected;
                        remainBalanceString += $"<p>{amount.ToString("C2", new CultureInfo("vi-VN"))}</p>";
                    });
                    result = result.Replace(remainBalance, remainBalanceString);
                }

                if (result.Contains(orderCode) && result.Contains("<p>"))
                {
                    var orderCodeString = "";
                    listOrder.ForEach(item =>
                    {
                        var code = item.OrderCode;
                        orderCodeString += $"<p>{code}</p>";
                    });

                    result = result.Replace(orderCode, orderCodeString);
                }
                else if (result.Contains(orderCode) && !result.Contains("<p>"))
                {
                    var orderCodeString = "";
                    listOrder.ForEach(item =>
                    {
                        var code = item.OrderCode;
                        orderCodeString += $"{code}, ";
                    });
                    result = result.Replace(orderCode, orderCodeString.Substring(0, orderCodeString.Length - 2));
                }

                if (result.Contains(orderDate))
                {
                    var orderDateString = "";
                    listOrder.ForEach(item =>
                    {
                        var date = item.OrderDate.Value.ToString("dd/MM/yyyy");
                        orderDateString += $"<p>{date}</p>";
                    });

                    result = result.Replace(orderDate, orderDateString);
                }
            }
            //Báo có
            else if (typeModel == TypeModel.BankReceipts)
            {
                var _model = model as BankReceiptInvoice;

                var customerId = context.BankReceiptInvoiceMapping
                    .FirstOrDefault(x => x.BankReceiptInvoiceId == _model.BankReceiptInvoiceId)?.ObjectId;

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

                if (result.Contains(orderNumber))
                {
                    var str = "";
                    for (var i = 0; i < receiptOrderHistory.Count; i++)
                    {
                        var stt = i + 1;
                        str += "<p>" + stt.ToString() + "</p>";
                    }

                    result = result.Replace(orderNumber, str);
                }

                if (result.Contains(amountPay))
                {
                    var amountPayString = "";
                    receiptOrderHistory.ForEach(item =>
                    {
                        var amount = item.Amount.ToString("C2", new CultureInfo("vi-VN"));
                        amountPayString += $"<p>{amount}</p>";
                    });

                    result = result.Replace(amountPay, amountPayString);
                }

                if (result.Contains(totalAmountPay) && _model.BankReceiptInvoiceAmount != null)
                {
                    var totalAmountPayString = "";
                    receiptOrderHistory.ForEach(item =>
                    {
                        var amount = item.AmountCollected.ToString("C2", new CultureInfo("vi-VN"));
                        totalAmountPayString += $"<p>{amount}</p>";
                    });

                    result = result.Replace(totalAmountPay, totalAmountPayString);
                }

                if (result.Contains(remainBalance) && _model.BankReceiptInvoiceAmount != null)
                {
                    var remainBalanceString = "";
                    receiptOrderHistory.ForEach(item =>
                    {
                        var amount = item.Amount - item.AmountCollected;
                        remainBalanceString += $"<p>{amount.ToString("C2", new CultureInfo("vi-VN"))}</p>";
                    });
                    result = result.Replace(remainBalance, remainBalanceString);
                }

                if (result.Contains(orderCode) && result.Contains("<p>"))
                {
                    var orderCodeString = "";
                    listOrder.ForEach(item =>
                    {
                        var code = item.OrderCode;
                        orderCodeString += $"<p>{code}</p>";
                    });

                    result = result.Replace(orderCode, orderCodeString);
                }
                else if (result.Contains(orderCode) && !result.Contains("<p>"))
                {
                    var orderCodeString = "";
                    listOrder.ForEach(item =>
                    {
                        var code = item.OrderCode;
                        orderCodeString += $"{code}, ";
                    });
                    result = result.Replace(orderCode, orderCodeString.Substring(0, orderCodeString.Length - 2));
                }

                if (result.Contains(orderDate))
                {
                    var orderDateString = "";
                    listOrder.ForEach(item =>
                    {
                        var date = item.OrderDate.Value.ToString("dd/MM/yyyy");
                        orderDateString += $"<p>{date}</p>";
                    });

                    result = result.Replace(orderDate, orderDateString);
                }
            }


            return result;
        }

        public SearchBankBookReceiptResult SearchBankBookReceipt(SearchBankBookReceiptParameter parameter)
        {
            try
            {
                var reasonCategoryTypeId =
                    context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "LTH").CategoryTypeId;
                var listAllReason = context.Category.Where(c => c.CategoryTypeId == reasonCategoryTypeId);

                var listIdUser = parameter.ListCreateById;
                var createdByIds = new List<Guid>();

                if (listIdUser != null)
                {
                    foreach (var item in listIdUser)
                    {
                        var temp = context.User.FirstOrDefault(u => u.EmployeeId == item).UserId;
                        createdByIds.Add(temp);
                    }
                }
                else
                {
                    createdByIds = listIdUser;
                }

                var fromDate = parameter.FromPaidDate;
                var toDate = parameter.ToPaidDate;

                var lst = context.BankReceiptInvoice.Join(context.BankReceiptInvoiceMapping,
                        bi => bi.BankReceiptInvoiceId,
                        bm => bm.BankReceiptInvoiceId,
                        (bi, bm) => new {bi, bm})
                    .Where(x => (parameter.BankAccountId == null || parameter.BankAccountId.Count == 0 ||
                                 parameter.BankAccountId.Contains(x.bi.BankReceiptInvoiceBankAccountId.Value)) &&
                                (createdByIds == null || createdByIds.Count == 0 ||
                                 createdByIds.Contains(x.bi.CreatedById)) &&
                                (fromDate == null || fromDate == DateTime.MinValue || fromDate <= x.bi.CreatedDate) &&
                                (toDate == null || toDate == DateTime.MinValue || toDate >= x.bi.CreatedDate))
                    .Select(m => new BankReceiptInvoiceEntityModel
                    {
                        BankReceiptInvoiceId = m.bi.BankReceiptInvoiceId,
                        BankReceiptInvoiceCode = m.bi.BankReceiptInvoiceCode,
                        BankReceiptInvoiceDetail = m.bi.BankReceiptInvoiceDetail,
                        BankReceiptInvoicePrice = m.bi.BankReceiptInvoicePrice,
                        BankReceiptInvoicePriceCurrency = m.bi.BankReceiptInvoicePriceCurrency,
                        BankReceiptInvoiceExchangeRate = m.bi.BankReceiptInvoiceExchangeRate,
                        BankReceiptInvoiceReason = m.bi.BankReceiptInvoiceReason,
                        BankReceiptInvoiceNote = m.bi.BankReceiptInvoiceNote,
                        BankReceiptInvoiceBankAccountId = m.bi.BankReceiptInvoiceBankAccountId,
                        BankReceiptInvoiceAmount = m.bi.BankReceiptInvoiceAmount,
                        BankReceiptInvoiceAmountText = m.bi.BankReceiptInvoiceAmountText,
                        BankReceiptInvoicePaidDate = m.bi.BankReceiptInvoicePaidDate,
                        OrganizationId = m.bi.OrganizationId,
                        StatusId = m.bi.StatusId,
                        Active = m.bi.Active,
                        CreatedById = m.bi.CreatedById,
                        CreatedDate = m.bi.CreatedDate,
                        UpdatedById = m.bi.UpdatedById,
                        UpdatedDate = m.bi.UpdatedDate,
                        BankReceiptInvoiceReasonText =
                            listAllReason.FirstOrDefault(c => c.CategoryId == m.bi.BankReceiptInvoiceReason)
                                .CategoryName ??
                            "",
                        ObjectId = m.bm.ObjectId,
                        StatusName = "",
                        BackgroundColorForStatus = ""
                    }).ToList();

                lst.ForEach(item =>
                {
                    item.ObjectName = GetObjectName(item.ObjectId);
                    item.CreatedByName = GetCreateByName(item.CreatedById);
                    item.BankReceiptInvoiceDetail = item.BankReceiptInvoiceDetail ?? "";
                    item.BankReceiptInvoiceNote = item.BankReceiptInvoiceNote ?? "";
                });

                lst = lst.OrderByDescending(x => x.CreatedDate).ToList();

                return new SearchBankBookReceiptResult()
                {
                    StatusCode = lst.Count > 0 ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed,
                    MessageCode = lst.Count > 0 ? "" : CommonMessage.BankReceiptInvoice.NO_INVOICE,
                    BankReceiptInvoiceList = lst.OrderByDescending(l => l.CreatedDate).ToList(),
                };
            }
            catch (Exception e)
            {
                return new SearchBankBookReceiptResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetBankReceiptInvoiceByIdResult GetBankReceiptInvoiceById(GetBankReceiptInvoiceByIdParameter parameter)
        {
            try
            {
                var bri = context.BankReceiptInvoice.FirstOrDefault(b =>
                    b.BankReceiptInvoiceId == parameter.BankReceiptInvoiceId);
                var brim = context.BankReceiptInvoiceMapping
                    .FirstOrDefault(b => b.BankReceiptInvoiceId == parameter.BankReceiptInvoiceId).ObjectId;
                var reasontext = context.Category.FirstOrDefault(rt => rt.CategoryId == bri.BankReceiptInvoiceReason);
                var org = context.Organization.FirstOrDefault(o => o.OrganizationId == bri.OrganizationId);
                var status = context.Category.FirstOrDefault(st => st.CategoryId == bri.StatusId);
                var pricecrr = context.Category.FirstOrDefault(pr => pr.CategoryId == bri.BankReceiptInvoicePriceCurrency);
                var empId = context.User.FirstOrDefault(u => u.UserId == bri.CreatedById).EmployeeId;
                var createdName = context.Employee.FirstOrDefault(e => e.EmployeeId == empId).EmployeeName;
                var objectName = GetObjectName(brim);
                var bankaccount =
                    context.BankAccount.FirstOrDefault(ba => ba.BankAccountId == bri.BankReceiptInvoiceBankAccountId);

                bri.BankReceiptInvoiceAmountText = MoneyHelper.Convert(bri.BankReceiptInvoiceAmount.Value);

                return new GetBankReceiptInvoiceByIdResult()
                {
                    BankReceiptInvoice = new BankReceiptInvoiceEntityModel(bri),
                    BankReceiptInvoiceReasonText = reasontext.CategoryName,
                    BankReceiptTypeText = (bankaccount != null) ? bankaccount.BankName : "",
                    OrganizationText = org.OrganizationName,
                    StatusText = status.CategoryName,
                    PriceCurrencyText = pricecrr.CategoryName,
                    CreateName = createdName,
                    ObjectName = objectName,
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success"
                };
            }
            catch (Exception e)
            {
                return new GetBankReceiptInvoiceByIdResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public ExportReceiptinvoiceResult ExportPdfReceiptInvoice(ExportReceiptInvoiceParameter parameter)
        {
            try
            {
                string html = ExportPdf.GetStringHtml("ReceiptInvoiceTemplatePDF.html");
                string css = ExportPdf.GetstrgCss("bootstrap.min.css");
                var company = context.CompanyConfiguration.FirstOrDefault(c => c.CompanyId != null);
                var receiptInvoice =
                    context.ReceiptInvoice.FirstOrDefault(r => r.ReceiptInvoiceId == parameter.ReceiptInvoiceId);

                if (receiptInvoice == null)
                {
                    return new ExportReceiptinvoiceResult()
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = "Bản ghi không tồn tại trên hệ thống"
                    };
                }

                var reason = context.Category.FirstOrDefault(rs => rs.CategoryId == receiptInvoice.ReceiptInvoiceReason)
                    .CategoryName;
                html = html.Replace("[CompanyName]", company.CompanyName.ToUpper());
                html = html.Replace("[CompanyAddress]", company.CompanyAddress);
                html = html.Replace("[ReceiptInvCode]", receiptInvoice.ReceiptInvoiceCode);
                html = html.Replace("[Date]", receiptInvoice.CreatedDate.Day.ToString());
                html = html.Replace("[Month]", receiptInvoice.CreatedDate.Month.ToString());
                html = html.Replace("[Year]", receiptInvoice.CreatedDate.Year.ToString());
                html = html.Replace("[RecipientName]", receiptInvoice.RecipientName);
                if (receiptInvoice.RecipientAddress != null)
                    html = html.Replace("[ReceipientAddress]", receiptInvoice.RecipientAddress);
                html = html.Replace("[ReceiptInvReason]", reason);
                html = html.Replace("[ReceiptInvDetail]", receiptInvoice.ReceiptInvoiceDetail);
                decimal price = (decimal)((receiptInvoice.ExchangeRate != null)
                    ? receiptInvoice.ExchangeRate * receiptInvoice.UnitPrice
                    : receiptInvoice.UnitPrice);
                html = html.Replace("[ReceiptInvPrice]", price.ToString("#,#."));
                html = html.Replace("[ReceiptInvPriceText]", MoneyHelper.Convert(price));
                html = html.Replace("[Note]",
                    receiptInvoice.ReceiptInvoiceNote == null ? "" : receiptInvoice.ReceiptInvoiceNote);

                // Export html to Pdf
                string rootFolder = _hostingEnvironment.WebRootPath + "\\ExportedPDF";
                string fileName = @"ExportedReceipt.pdf";
                var receiptInvoicePdf = ExportPdf.HtmlToPdfExport(html, Path.Combine(rootFolder, fileName),
                    PdfPageSize.A5, PdfPageOrientation.Landscape, string.Empty);

                return new ExportReceiptinvoiceResult
                {
                    ReceiptInvoicePdf = receiptInvoicePdf,
                    Code = receiptInvoice.ReceiptInvoiceCode,
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success"
                };
            }
            catch (Exception e)
            {
                return new ExportReceiptinvoiceResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public ExportBankReceiptInvoiceResult ExportBankReceiptInvoice(ExportBankReceiptInvoiceParameter parameter)
        {
            string html = ExportPdf.GetStringHtml("BankReceiptInvTemplate.html");
            string css = ExportPdf.GetstrgCss("bootstrap.min.css");
            var company = context.CompanyConfiguration.FirstOrDefault(c => c.CompanyId != null);
            var bankInvoice =
                context.BankReceiptInvoice.FirstOrDefault(r => r.BankReceiptInvoiceId == parameter.BankReceiptInvoiceId);

            if (bankInvoice == null)
            {
                return new ExportBankReceiptInvoiceResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = "Bản ghi không tồn tại trên hệ thống"
                };
            }

            var reason = context.Category.FirstOrDefault(c => c.CategoryId == bankInvoice.BankReceiptInvoiceReason)
                .CategoryName;
            var status = context.Category.FirstOrDefault(c => c.CategoryId == bankInvoice.StatusId).CategoryName;
            var currency = context.Category
                .FirstOrDefault(c => c.CategoryId == bankInvoice.BankReceiptInvoicePriceCurrency).CategoryName;
            var org = context.Organization.FirstOrDefault(o => o.OrganizationId == bankInvoice.OrganizationId)
                .OrganizationName;
            var obj = context.BankReceiptInvoiceMapping.FirstOrDefault(bp =>
                bp.BankReceiptInvoiceId == bankInvoice.BankReceiptInvoiceId);
            var objectId = obj == null ? Guid.Empty : obj.ObjectId;
            var empId = context.User.FirstOrDefault(u => u.UserId == bankInvoice.CreatedById).EmployeeId;
            var name = context.Employee.FirstOrDefault(e => e.EmployeeId == empId).EmployeeName;
            string objectName = GetObjectNameWithoutCode(objectId);
            html = html.Replace("[CompanyName]", company.CompanyName.ToUpper());
            html = html.Replace("[CompanyAddress]", company.CompanyAddress);
            html = html.Replace("[Code]", bankInvoice.BankReceiptInvoiceCode);
            html = html.Replace("[CreateDateDay]", bankInvoice.CreatedDate.Day.ToString());
            html = html.Replace("[CreateMonth]", bankInvoice.CreatedDate.Month.ToString());
            html = html.Replace("[CreateYear]", bankInvoice.CreatedDate.Year.ToString());
            html = html.Replace("[Content]", bankInvoice.BankReceiptInvoiceDetail);
            html = html.Replace("[Price]", bankInvoice.BankReceiptInvoiceAmount.Value.ToString("#,#."));
            html = html.Replace("[PriceString]", MoneyHelper.Convert(bankInvoice.BankReceiptInvoiceAmount.Value));
            html = html.Replace("[Note]", bankInvoice.BankReceiptInvoiceNote);
            html = html.Replace("[PaidDate]", bankInvoice.BankReceiptInvoicePaidDate.ToString("dd/MM/yyyy"));
            html = html.Replace("[Reason]", reason);
            html = html.Replace("[Object]", objectName);
            html = html.Replace("[Status]", status);
            html = html.Replace("[Organization]", org);
            html = html.Replace("[CurrencyCode]", currency);
            html = html.Replace("[CreatedBy]", name);

            // Export html to Pdf
            string rootFolder = _hostingEnvironment.WebRootPath + "\\ExportedPDF";
            string fileName = @"ExportedBankReceipt.pdf";
            var bankInvoicePdf = ExportPdf.HtmlToPdfExport(html, Path.Combine(rootFolder, fileName), PdfPageSize.A5,
                PdfPageOrientation.Landscape, string.Empty);

            return new ExportBankReceiptInvoiceResult()
            {
                BankReceiptInvoicePdf = bankInvoicePdf,
                Code = bankInvoice.BankReceiptInvoiceCode,
                StatusCode = HttpStatusCode.OK,
                MessageCode = "Success"
            };
        }

        private string GetCreateByName(Guid? createById)
        {
            if (createById != null && createById != Guid.Empty)
            {
                var empId = context.User.FirstOrDefault(u => u.UserId == createById).EmployeeId;

                if (empId != null && empId != Guid.Empty)
                {
                    var emp = context.Employee.FirstOrDefault(x => x.EmployeeId == empId);

                    if (emp != null)
                    {
                        return emp.EmployeeCode + " - " + emp.EmployeeName;
                    }
                }
            }
            return "";
        }

        private string GetObjectName(Guid? objId)
        {
            if (objId != null && objId != Guid.Empty)
            {
                var emp = context.Employee.FirstOrDefault(e => e.EmployeeId == objId);
                var cus = context.Customer.FirstOrDefault(cu => cu.CustomerId == objId);
                var con = context.Contact.FirstOrDefault(c => c.ObjectId == objId);
                var ven = context.Vendor.FirstOrDefault(e => e.VendorId == objId);

                if (emp != null && con != null)
                {
                    return con.IdentityId + " - " + emp.EmployeeName;
                }

                if (ven != null)
                {
                    return ven.VendorCode + " - " + ven.VendorName;
                }
                if (cus != null)
                {
                    return cus.CustomerCode + " - " + cus.CustomerName;
                }

                return "";
            }

            return "";
        }

        private string GetObjectNameWithoutCode(Guid? objId)
        {
            if (objId != null && objId != Guid.Empty)
            {
                var emp = context.Employee.FirstOrDefault(e => e.EmployeeId == objId);
                var cus = context.Customer.FirstOrDefault(cu => cu.CustomerId == objId);
                var con = context.Contact.FirstOrDefault(c => c.ObjectId == objId);
                var ven = context.Vendor.FirstOrDefault(e => e.VendorId == objId);

                if (emp != null && con != null)
                {
                    return emp.EmployeeName;
                }

                if (ven != null)
                {
                    return ven.VendorName;
                }
                if (cus != null)
                {
                    return cus.CustomerName;
                }

                return "";
            }

            return "";
        }

        public GetOrderByCustomerIdResult GetOrderByCustomerId(GetOrderByCustomerIdParameter parameter)
        {
            try
            {
                List<Guid> listOrderId = new List<Guid>();
                List<ReceiptOrderHistory> listOrderInReceiptOrderHistory = new List<ReceiptOrderHistory>();
                decimal totalAmountReceivable = 0;
                List<ReceiptInvoiceOrderModel> listReceiptInvoiceOrderModel = new List<ReceiptInvoiceOrderModel>();

                var statusInprocess = context.OrderStatus.FirstOrDefault(x => x.OrderStatusCode == "IP").OrderStatusId; //Đang xử lý
                var statusWasSend = context.OrderStatus.FirstOrDefault(x => x.OrderStatusCode == "DLV").OrderStatusId; //Đã giao hàng

                //Lấy danh sách đơn hàng theo khách hàng
                var listOrder = context.CustomerOrder.Where(x =>
                        (x.StatusId == statusInprocess || x.StatusId == statusWasSend) &&
                        x.CustomerId == parameter.CustomerId)
                    .OrderBy(y => y.OrderDate)
                    .ToList();

                if (parameter.OrderId != null)
                {
                    listOrder = listOrder.Where(x => x.OrderId == parameter.OrderId).ToList();
                }

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
                    listOrderInReceiptOrderHistory = context.ReceiptOrderHistory
                        .Where(x => listOrderId.Contains(x.OrderId)).ToList();
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
                            var totalOrder = CalculatorTotalPurchaseProduct(item.Amount.Value, item.DiscountType.Value,
                                item.DiscountValue.Value);
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
                                }
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
                            var totalOrder = CalculatorTotalPurchaseProduct(item.Amount.Value, item.DiscountType.Value,
                                item.DiscountValue.Value);
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

                return new GetOrderByCustomerIdResult
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Lấy danh sách đơn hàng thành công",
                    listOrder = listReceiptInvoiceOrderModel,
                    totalAmountReceivable = totalAmountReceivable
                };
            }
            catch (Exception e)
            {
                return new GetOrderByCustomerIdResult
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
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

        public GetMasterDataSearchBankReceiptInvoiceResult GetMasterDataSearchBankReceiptInvoice(GetMasterDataSearchBankReceiptInvoiceParameter parameter)
        {
            try
            {
                var reasonCategoryTypeId =
                    context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "LTH").CategoryTypeId;
                var _listAllReason = context.Category.Where(c => c.CategoryTypeId == reasonCategoryTypeId).ToList();
                var listAllReason = new List<CategoryEntityModel>();
                _listAllReason.ForEach(item =>
                {
                    var reason = new CategoryEntityModel(item);
                    listAllReason.Add(reason);
                });

                var statusCateoryTypeId =
                    context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "TCH").CategoryTypeId;
                var _listAllStatus = context.Category.Where(x => x.CategoryTypeId == statusCateoryTypeId).ToList();
                var listAllStatus = new List<CategoryEntityModel>();
                _listAllStatus.ForEach(item =>
                {
                    var status = new CategoryEntityModel(item);
                    listAllStatus.Add(status);
                });

                var _listEmpployee = context.Employee.Where(x => x.Active == true).ToList();
                var listEmpployee = new List<EmployeeEntityModel>();
                _listEmpployee.ForEach(item =>
                {
                    var emp = new EmployeeEntityModel(item);
                    listEmpployee.Add(emp);
                });

                return new GetMasterDataSearchBankReceiptInvoiceResult
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success",
                    ReasonOfReceiptList = listAllReason,
                    StatusOfReceiptList = listAllStatus,
                    EmployeeList = listEmpployee
                };
            }
            catch (Exception ex)
            {
                return new GetMasterDataSearchBankReceiptInvoiceResult
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = ex.Message
                };
            }
        }

        public GetMasterDataReceiptInvoiceResult GetMasterDataReceiptInvoice(GetMasterDataReceiptInvoiceParameter parameter)
        {
            try
            {
                var listCategoryType = context.CategoryType.ToList();
                var listCategory = context.Category.ToList();
                var _listOrganization = context.Organization.Where(o => o.IsFinancialIndependence.Value).ToList();
                var listOrganization = new List<OrganizationEntityModel>();
                _listOrganization.ForEach(item =>
                {
                    var org = new OrganizationEntityModel(item);
                    listOrganization.Add(org);
                });

                var categoryReasonType = listCategoryType.FirstOrDefault(ct => ct.CategoryTypeCode == "LTH");
                var reasonReceiptList = new List<CategoryEntityModel>();

                if (categoryReasonType != null)
                {
                    reasonReceiptList = listCategory
                        .Where(ct => ct.Active == true && ct.CategoryTypeId == categoryReasonType.CategoryTypeId)
                        .Select(c => new CategoryEntityModel()
                        {
                            CategoryTypeId = c.CategoryTypeId,
                            CategoryId = c.CategoryId,
                            CategoryName = c.CategoryName,
                            CategoryCode = c.CategoryCode,
                            IsDefault = c.IsDefauld
                        }).ToList();
                }

                var categoryStatusType = listCategoryType.FirstOrDefault(ct => ct.CategoryTypeCode == "TCH");
                var statusOfReceiptList = new List<CategoryEntityModel>();
                if (categoryStatusType != null)
                {
                    statusOfReceiptList = listCategory
                        .Where(c => c.Active == true && c.CategoryTypeId == categoryStatusType.CategoryTypeId).Select(
                            c => new CategoryEntityModel()
                            {
                                CategoryTypeId = c.CategoryTypeId,
                                CategoryId = c.CategoryId,
                                CategoryName = c.CategoryName,
                                CategoryCode = c.CategoryCode,
                                IsDefault = c.IsDefauld
                            }).ToList();
                }

                var categoryType = listCategoryType.FirstOrDefault(ct => ct.CategoryTypeCode == "LSO");
                var typeOfReceiptList = new List<CategoryEntityModel>();
                if (categoryType != null)
                {
                    typeOfReceiptList = listCategory
                        .Where(c => c.Active == true && c.CategoryTypeId == categoryType.CategoryTypeId).Select(c =>
                            new CategoryEntityModel()
                            {
                                CategoryTypeId = c.CategoryTypeId,
                                CategoryId = c.CategoryId,
                                CategoryName = c.CategoryName,
                                CategoryCode = c.CategoryCode,
                                IsDefault = c.IsDefauld
                            }).ToList();
                }

                var categoryUnitMoneyType = listCategoryType.FirstOrDefault(ct => ct.CategoryTypeCode == "DTI");
                var unitMoneyOfReceiptList = new List<CategoryEntityModel>();
                if (categoryStatusType != null)
                {
                    unitMoneyOfReceiptList = listCategory
                        .Where(c => c.Active == true && c.CategoryTypeId == categoryUnitMoneyType.CategoryTypeId)
                        .Select(c => new CategoryEntityModel()
                        {
                            CategoryTypeId = c.CategoryTypeId,
                            CategoryId = c.CategoryId,
                            CategoryName = c.CategoryName,
                            CategoryCode = c.CategoryCode,
                            IsDefault = c.IsDefauld
                        }).ToList();
                }

                var _customerList = context.Customer.Where(c => c.Active == true).OrderBy(x => x.CustomerName).ToList();
                var customerList = new List<CustomerEntityModel>();
                _customerList.ForEach(item =>
                {
                    var cus = new CustomerEntityModel(item);
                    customerList.Add(cus);
                });

                return new GetMasterDataReceiptInvoiceResult
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success",
                    ListReason = reasonReceiptList,
                    ListStatus = statusOfReceiptList,
                    TypesOfReceiptList = typeOfReceiptList,
                    UnitMoneyList = unitMoneyOfReceiptList,
                    OrganizationList = listOrganization,
                    CustomerList = customerList
                };
            }
            catch (Exception ex)
            {
                return new GetMasterDataReceiptInvoiceResult
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = ex.Message
                };
            }
        }

        public GetMasterDataSearchReceiptInvoiceResult GetMasterDataSearchReceiptInvoice(GetMasterDataSearchReceiptInvoiceParameter parameter)
        {
            try
            {
                var listCategoryType = context.CategoryType.ToList();
                var listCategory = context.Category.ToList();

                var categoryReasonType = listCategoryType.FirstOrDefault(ct => ct.CategoryTypeCode == "LTH");
                var reasonReceiptList = new List<CategoryEntityModel>();

                if (categoryReasonType != null)
                {
                    reasonReceiptList = listCategory
                        .Where(ct => ct.Active == true && ct.CategoryTypeId == categoryReasonType.CategoryTypeId)
                        .Select(c => new CategoryEntityModel()
                        {
                            CategoryTypeId = c.CategoryTypeId,
                            CategoryId = c.CategoryId,
                            CategoryName = c.CategoryName,
                            CategoryCode = c.CategoryCode,
                            IsDefault = c.IsDefauld
                        }).ToList();
                }

                var categoryStatusType = listCategoryType.FirstOrDefault(ct => ct.CategoryTypeCode == "TCH");
                var statusOfReceiptList = new List<CategoryEntityModel>();
                if (categoryStatusType != null)
                {
                    statusOfReceiptList = listCategory.Where(c => c.CategoryTypeId == categoryStatusType.CategoryTypeId)
                        .Select(c => new CategoryEntityModel()
                        {
                            CategoryTypeId = c.CategoryTypeId,
                            CategoryId = c.CategoryId,
                            CategoryName = c.CategoryName,
                            CategoryCode = c.CategoryCode,
                            IsDefault = c.IsDefauld
                        }).ToList();
                }

                var _listEmployee = context.Employee.Where(e => e.Active == true).ToList();
                var listEmployee = new List<EmployeeEntityModel>();
                _listEmployee.ForEach(item =>
                {
                    var emp = new EmployeeEntityModel(item);
                    listEmployee.Add(emp);
                });

                return new GetMasterDataSearchReceiptInvoiceResult
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success",
                    ListReason = reasonReceiptList,
                    ListStatus = statusOfReceiptList,
                    ListEmployee = listEmployee,
                };
            }
            catch (Exception ex)
            {
                return new GetMasterDataSearchReceiptInvoiceResult
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = ex.Message
                };
            }
        }

        public ConfirmPaymentResult ConfirmPayment(ConfirmPaymentParameter parameter)
        {
            try
            {
                //Phiếu thu
                if (parameter.Type == "cash")
                {
                    var receiptInvoice =
                        context.ReceiptInvoice.FirstOrDefault(x => x.ReceiptInvoiceId == parameter.ReceiptInvoiceId);

                    var maDoiTuong =
                        context.Category.FirstOrDefault(x => x.CategoryId == receiptInvoice.ReceiptInvoiceReason);

                    //Nếu đôi tượng là Khách hàng
                    if (maDoiTuong?.CategoryCode == "THA")
                    {
                        #region Lấy email khách hàng

                        var receiptInvoiceMapping = context.ReceiptInvoiceMapping.FirstOrDefault(x =>
                            x.ReceiptInvoiceId == parameter.ReceiptInvoiceId);

                        if (receiptInvoiceMapping != null)
                        {
                            var customerId = context.Customer
                                .FirstOrDefault(x => x.CustomerId == receiptInvoiceMapping.ObjectId)
                                ?.CustomerId;
                            var cusEmail = context.Contact.FirstOrDefault(x =>
                                x.ObjectId == customerId && x.ObjectType == "CUS")?.Email;

                            if (string.IsNullOrEmpty(cusEmail))
                            {
                                return new ConfirmPaymentResult()
                                {
                                    StatusCode = HttpStatusCode.ExpectationFailed,
                                    MessageCode = "Gửi thông báo thất bại, vui lòng cập nhật thông tin email doanh nghiệp."
                                };
                            }

                            // Thong bao 
                            NotificationHelper.AccessNotification(context, TypeModel.CashReceipts, "PAY_CRE", new ReceiptInvoice(), receiptInvoice, true);
                        }

                        #endregion
                    }

                    receiptInvoice.IsSendMail = true;

                    context.Update(receiptInvoice);
                    context.SaveChanges();

                    return new ConfirmPaymentResult()
                    {
                        StatusCode = HttpStatusCode.OK,
                        MessageCode = "Gửi thông báo xác nhận thành công"
                    };
                }
                //Phiếu UNC
                else
                {
                    var bankReceiptInvoice =
                        context.BankReceiptInvoice.FirstOrDefault(x => x.BankReceiptInvoiceId == parameter.ReceiptInvoiceId);

                    var maDoiTuong =
                        context.Category.FirstOrDefault(x => x.CategoryId == bankReceiptInvoice.BankReceiptInvoiceReason);

                    //Nếu đôi tượng là Khách hàng
                    if (maDoiTuong?.CategoryCode == "THA")
                    {
                        #region Lấy email khách hàng

                        var receiptInvoiceMapping = context.BankReceiptInvoiceMapping.FirstOrDefault(x =>
                            x.BankReceiptInvoiceId == parameter.ReceiptInvoiceId);

                        if (receiptInvoiceMapping != null)
                        {
                            var customerId = context.Customer
                                .FirstOrDefault(x => x.CustomerId == receiptInvoiceMapping.ObjectId)
                                ?.CustomerId;
                            var cusEmail = context.Contact.FirstOrDefault(x =>
                                x.ObjectId == customerId && x.ObjectType == "CUS")?.Email;

                            if (string.IsNullOrEmpty(cusEmail))
                            {
                                return new ConfirmPaymentResult()
                                {
                                    StatusCode = HttpStatusCode.ExpectationFailed,
                                    MessageCode = "Gửi thông báo thất bại, vui lòng cập nhật thông tin email doanh nghiệp."
                                };
                            }

                            // Thong bao 
                            NotificationHelper.AccessNotification(context, TypeModel.BankReceipts, "PAY_CRE", new BankReceiptInvoice(), bankReceiptInvoice, true);
                        }

                        #endregion
                    }

                    bankReceiptInvoice.IsSendMail = true;

                    context.Update(bankReceiptInvoice);
                    context.SaveChanges();

                    return new ConfirmPaymentResult()
                    {
                        StatusCode = HttpStatusCode.OK,
                        MessageCode = "Gửi thông báo xác nhận thành công"
                    };
                }

            }
            catch (Exception e)
            {
                return new ConfirmPaymentResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }
    }
}
