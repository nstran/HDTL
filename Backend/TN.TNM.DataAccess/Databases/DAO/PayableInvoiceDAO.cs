using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Hosting;
using SelectPdf;
using TN.TNM.Common;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.PayableInvoice;
using TN.TNM.DataAccess.Messages.Results.PayableInvoice;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.BankAccount;
using TN.TNM.DataAccess.Models.Customer;
using TN.TNM.DataAccess.Models.Employee;
using TN.TNM.DataAccess.Models.PayableInvoice;
using TN.TNM.DataAccess.Models.User;
using TN.TNM.DataAccess.Models.Vendor;

namespace TN.TNM.DataAccess.Databases.DAO
{
    public class PayableInvoiceDAO : BaseDAO, IPayableInvoiceDataAccess
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        public PayableInvoiceDAO(Databases.TNTN8Context _content, IAuditTraceDataAccess _iAuditTrace, IHostingEnvironment hostingEnvironment)
        {
            this.context = _content;
            this.iAuditTrace = _iAuditTrace;
            _hostingEnvironment = hostingEnvironment;
        }

        public CreatePayableInvoiceResult CreatePayableInvoice(CreatePayableInvoiceParameter parameter)
        {
            try
            {
                var newPayableInvoice = parameter.PayableInvoice;
                if (parameter.PayableInvoice.PayableInvoicePrice <= 0)
                {
                    return new CreatePayableInvoiceResult
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = "Số tiền không được là số âm hoặc bằng 0",
                    };
                }

                if (parameter.PayableInvoice.ExchangeRate <= 0)
                {
                    return new CreatePayableInvoiceResult
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = "Tỷ giá không được là số âm hoặc bằng 0",
                    };
                }
                var organizationCode = context.Organization
                    .FirstOrDefault(o => o.OrganizationId == parameter.PayableInvoice.OrganizationId)?.OrganizationCode;

                newPayableInvoice.PayableInvoiceId = Guid.NewGuid();
                newPayableInvoice.CreatedDate = DateTime.Now;
                newPayableInvoice.CreatedById = parameter.UserId;
                newPayableInvoice.PayableInvoiceCode = "PC" + "-" + organizationCode + DateTime.Now.Year
                                                       + (context.PayableInvoice.Count(p =>
                                                              p.CreatedDate.Year == DateTime.Now.Year) + 1)
                                                       .ToString("D5");
                newPayableInvoice.RegisterType = parameter.PayableInvoice.RegisterType;
                newPayableInvoice.ObjectId = parameter.PayableInvoice.ObjectId;
                var payableInvoiceDupblicase = context.PayableInvoice.FirstOrDefault(x =>
                    x.PayableInvoiceCode == newPayableInvoice.PayableInvoiceCode);

                if (payableInvoiceDupblicase != null)
                {
                    return new CreatePayableInvoiceResult
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = "Mã phiếu chi đã tồn tại",
                    };
                }

                var newPayableInvoiceMapping = parameter.PayableInvoiceMapping;
                newPayableInvoiceMapping.PayableInvoiceMappingId = Guid.NewGuid();
                newPayableInvoiceMapping.CreatedById = parameter.UserId;
                newPayableInvoiceMapping.CreatedDate = DateTime.Now;
                newPayableInvoiceMapping.PayableInvoiceId = parameter.PayableInvoice.PayableInvoiceId;
                newPayableInvoiceMapping.ObjectId = parameter.PayableInvoiceMapping.ObjectId;

                var vendor = context.Vendor.FirstOrDefault(v => v.VendorId == newPayableInvoiceMapping.ObjectId);
                if (vendor != null)
                {
                    vendor.TotalPayableValue = (vendor.TotalPayableValue ?? 0) - ((newPayableInvoice.PayableInvoicePrice ?? 0) * (newPayableInvoice.ExchangeRate ?? 1));
                    context.Vendor.Update(vendor);
                }

                context.PayableInvoiceMapping.Add(newPayableInvoiceMapping.ToEntity());
                context.PayableInvoice.Add(newPayableInvoice.ToEntity());
                context.SaveChanges();

                return new CreatePayableInvoiceResult
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = CommonMessage.PayableInvoice.ADD_SUCCESS,
                };
            }
            catch (Exception e)
            {
                return new CreatePayableInvoiceResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetPayableInvoiceByIdResult GetPayableInvoiceById(GetPayableInvoiceByIdParameter parameter)
        {
            try
            {
                var reasonCategoryTypeId = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "LCH").CategoryTypeId;
                var listAllReason = context.Category.Where(x => x.CategoryTypeId == reasonCategoryTypeId).ToList();

                var statusCateoryTypeId = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "TCH").CategoryTypeId;
                var listAllStatus = context.Category.Where(x => x.CategoryTypeId == statusCateoryTypeId).ToList();

                var listAllOrg = context.Organization.ToList();

                var currencyUnitId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "DTI").CategoryTypeId;
                var listAllCurrencyUnit = context.Category.Where(x => x.CategoryTypeId == currencyUnitId).ToList();

                var payableInvoice = context.PayableInvoice.FirstOrDefault(c => parameter.PayableInvoiceId == null || parameter.PayableInvoiceId == Guid.Empty || c.PayableInvoiceId == parameter.PayableInvoiceId);

                var payableInvoiceMapping = context.PayableInvoiceMapping.FirstOrDefault(c => parameter.PayableInvoiceId == null || parameter.PayableInvoiceId == Guid.Empty || c.PayableInvoiceId == parameter.PayableInvoiceId);

                PayableInvoiceEntityModel data = new PayableInvoiceEntityModel();
                data.PayableInvoiceId = payableInvoice.PayableInvoiceId;
                data.PayableInvoiceCode = payableInvoice.PayableInvoiceCode;
                data.PayableInvoiceDetail = payableInvoice.PayableInvoiceDetail;
                data.PayableInvoicePrice = payableInvoice.PayableInvoicePrice;
                data.PayableInvoicePriceCurrency = payableInvoice.PayableInvoicePriceCurrency;
                data.PayableInvoiceReason = payableInvoice.PayableInvoiceReason;
                data.PayableInvoiceNote = payableInvoice.PayableInvoiceNote;
                data.RegisterType = payableInvoice.RegisterType;
                data.OrganizationId = payableInvoice.OrganizationId;
                data.StatusId = payableInvoice.StatusId;
                data.RecipientName = payableInvoice.RecipientName;
                data.RecipientAddress = payableInvoice.RecipientAddress;
                data.UnitPrice = payableInvoice.UnitPrice;
                data.CurrencyUnit = payableInvoice.CurrencyUnit;
                data.ExchangeRate = payableInvoice.ExchangeRate;
                data.Amount = payableInvoice.Amount;
                data.AmountText = payableInvoice.AmountText;
                data.Active = payableInvoice.Active;
                data.CreatedById = payableInvoice.CreatedById;
                data.CreatedDate = payableInvoice.CreatedDate;
                data.UpdatedById = payableInvoice.UpdatedById;
                data.UpdatedDate = payableInvoice.UpdatedDate;
                data.ObjectId = payableInvoiceMapping.ObjectId;
                data.PaidDate = payableInvoice.PaidDate;
                data.PayableInvoiceReasonText = listAllReason.FirstOrDefault(s => s.CategoryId == payableInvoice.PayableInvoiceReason).CategoryName;
                data.StatusName = listAllStatus.FirstOrDefault(c => c.CategoryId == payableInvoice.StatusId).CategoryName;

                if (data.ObjectId != null && data.ObjectId != Guid.Empty)
                {
                    data.ObjectName = GetObjectName(data.ObjectId);
                }

                if (data.CreatedById != null && data.CreatedById != Guid.Empty)
                {
                    data.CreatedByName = GetCreateByName(data.CreatedById);
                }

                if (data.OrganizationId != null && data.OrganizationId != Guid.Empty)
                {
                    data.OrganizationName = listAllOrg.FirstOrDefault(c => c.OrganizationId == data.OrganizationId).OrganizationName;
                }

                if (data.PayableInvoicePriceCurrency != null && data.PayableInvoicePriceCurrency != Guid.Empty)
                {
                    data.CurrencyUnitName = listAllCurrencyUnit.FirstOrDefault(c => c.CategoryId == data.PayableInvoicePriceCurrency).CategoryName;
                }
                data.AmountText = MoneyHelper.Convert((decimal)data.PayableInvoicePrice * (decimal)data.ExchangeRate);

                return new GetPayableInvoiceByIdResult
                {
                    PayableInvoice = data,
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success",
                };
            }
            catch (Exception ex)
            {
                return new GetPayableInvoiceByIdResult
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = ex.Message
                };
            }
        }


        public SearchPayableInvoiceResult SearchPayableInvoice(SearchPayableInvoiceParameter parameter)
        {
            try
            {
                var reasonCategoryTypeId = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "LCH").CategoryTypeId;
                var listAllReason = context.Category.Where(x => x.CategoryTypeId == reasonCategoryTypeId).ToList();

                var statusCateoryTypeId = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "TCH").CategoryTypeId;
                var listAllStatus = context.Category.Where(x => x.CategoryTypeId == statusCateoryTypeId).ToList();

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

                var lstPayableMap = context.PayableInvoiceMapping.Where(c => objectIds == null || objectIds.Count == 0 || objectIds.Contains(c.ObjectId.Value)).ToList();
                var lstPayableMapId = lstPayableMap.Select(c => c.PayableInvoiceId).ToList();

                var lst = context.PayableInvoice
                    .Where(c => (c.PayableInvoiceCode.Contains(parameter.PayableInvoiceCode) || parameter.PayableInvoiceCode == null || parameter.PayableInvoiceCode == "") &&
                                (parameter.PayableReasonIdList.Contains(c.PayableInvoiceReason.Value) || parameter.PayableReasonIdList == null || parameter.PayableReasonIdList.Count == 0) &&
                                (lstPayableMapId.Contains(c.PayableInvoiceId)) &&
                                (parameter.SttList.Contains(c.StatusId.Value) || parameter.SttList == null || parameter.SttList.Count == 0) &&
                                (createdByIds.Contains(c.CreatedById) || parameter.CreatedByIdList == null || parameter.CreatedByIdList.Count == 0) &&
                                (parameter.FromDate == null || parameter.FromDate == DateTime.MinValue || parameter.FromDate.Value.Date <= c.CreatedDate.Date) &&
                                (parameter.ToDate == null || parameter.ToDate == DateTime.MinValue || parameter.ToDate.Value.Date >= c.CreatedDate.Date))
                    .Select(m => new PayableInvoiceEntityModel
                    {
                        PayableInvoiceId = m.PayableInvoiceId,
                        PayableInvoiceCode = m.PayableInvoiceCode,
                        PayableInvoiceDetail = m.PayableInvoiceDetail,
                        PayableInvoicePrice = m.PayableInvoicePrice,
                        PayableInvoicePriceCurrency = m.PayableInvoicePriceCurrency,
                        PayableInvoiceReason = m.PayableInvoiceReason,
                        PayableInvoiceNote = m.PayableInvoiceNote,
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
                        PaidDate = m.PaidDate,
                        PayableInvoiceReasonText = listAllReason.FirstOrDefault(s => s.CategoryId == m.PayableInvoiceReason).CategoryName,
                        StatusName = listAllStatus.FirstOrDefault(c => c.CategoryId == m.StatusId).CategoryName,
                    }).ToList();
                lst.ForEach(item =>
                {
                    var temp = lstPayableMap.FirstOrDefault(c => c.PayableInvoiceId == item.PayableInvoiceId);
                    if (temp != null)
                    {
                        item.ObjectName = GetObjectName(temp.ObjectId);
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

                return new SearchPayableInvoiceResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success",
                    PayableInvList = lst,
                };
            }
            catch (Exception e)
            {
                return new SearchPayableInvoiceResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }
        public SearchCashBookPayableInvoiceResult SearchCashBookPayableInvoice(SearchCashBookPayableInvoiceParameter parameter)
        {
            try
            {
                var reasonCategoryTypeId = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "LCH")
                    ?.CategoryTypeId;
                var listAllReason = context.Category.Where(x => x.CategoryTypeId == reasonCategoryTypeId).ToList();
                var listIdUser = parameter.CreatedByIdList;
                var createdByIds = new List<Guid>();

                if (listIdUser.Count != 0)
                {
                    foreach (var item in listIdUser)
                    {
                        var temp = context.User.FirstOrDefault(u => u.EmployeeId == item)?.UserId;
                        if (temp != null)
                            createdByIds.Add(temp.Value);
                    }
                }
                else
                {
                    createdByIds = listIdUser;
                }

                var fromDate = parameter.FromPaidDate;
                var toDate = parameter.ToPaidDate;
                var organizationList = parameter.OrganizationList;
                var payableInvoiceList = context.PayableInvoice
                    .Where(c =>
                        (createdByIds == null || createdByIds.Count == 0 || createdByIds.Contains(c.CreatedById)) &&
                        (organizationList == null || organizationList.Count == 0 ||
                         organizationList.Contains(c.OrganizationId)) &&
                        (fromDate == null || fromDate == DateTime.MinValue || fromDate <= c.CreatedDate) &&
                        (toDate == null || toDate == DateTime.MinValue || toDate >= c.CreatedDate))
                    .Select(v => new PayableInvoiceEntityModel
                    {
                        Active = v.Active,
                        Amount = v.Amount,
                        AmountText = v.AmountText,
                        CreatedById = v.CreatedById,
                        CreatedDate = v.CreatedDate,
                        CurrencyUnit = v.CurrencyUnit,
                        ExchangeRate = v.ExchangeRate,
                        OrganizationId = v.OrganizationId,
                        PayableInvoiceCode = v.PayableInvoiceCode,
                        PayableInvoiceDetail = v.PayableInvoiceDetail,
                        PayableInvoiceId = v.PayableInvoiceId,
                        PayableInvoiceNote = v.PayableInvoiceNote,
                        PayableInvoiceReason = v.PayableInvoiceReason,
                        PayableInvoicePrice = v.PayableInvoicePrice,
                        PaidDate = v.PaidDate,
                        RecipientAddress = v.RecipientAddress,
                        RecipientName = v.RecipientName,
                        RegisterType = v.RegisterType,
                        StatusId = v.StatusId,
                        UnitPrice = v.UnitPrice,
                        UpdatedById = v.UpdatedById,
                        UpdatedDate = v.UpdatedDate,
                        PayableInvoiceReasonText =
                            listAllReason.FirstOrDefault(c => c.CategoryId == v.PayableInvoiceReason).CategoryName ??
                            "",
                    }).ToList();

                payableInvoiceList.ForEach(item =>
                {
                    item.CreatedByName = GetCreateByName(item.CreatedById);
                });
                payableInvoiceList.OrderByDescending(c => c.CreatedDate).ToList();

                return new SearchCashBookPayableInvoiceResult()
                {
                    StatusCode = payableInvoiceList.Count > 0 ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed,
                    MessageCode = payableInvoiceList.Count > 0 ? "" : CommonMessage.PayableInvoice.NO_INVOICE,
                    PayableInvList = payableInvoiceList,
                };
            }
            catch (Exception e)
            {
                return new SearchCashBookPayableInvoiceResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public CreateBankPayableInvoiceResult CreateBankPayableInvoice(CreateBankPayableInvoiceParameter parameter)
        {
            try
            {
                if (parameter.BankPayableInvoice.BankPayableInvoiceAmount <= 0)
                {
                    return new CreateBankPayableInvoiceResult()
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = "Số tiền cần lớn hơn 0"
                    };
                }

                if (parameter.BankPayableInvoice.BankPayableInvoicePrice <= 0)
                {
                    return new CreateBankPayableInvoiceResult()
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = "Số tiền cần lớn hơn 0"
                    };
                }

                if (parameter.BankPayableInvoice.BankPayableInvoiceExchangeRate <= 0)
                {
                    return new CreateBankPayableInvoiceResult()
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = "Tỉ giá không được âm hoặc bằng 0"
                    };
                }

                var newBankPayableInvoiceId = Guid.NewGuid();
                var organ = context.Organization.FirstOrDefault(or =>
                    or.OrganizationId == parameter.BankPayableInvoice.OrganizationId);
                parameter.BankPayableInvoice.BankPayableInvoiceId = newBankPayableInvoiceId;
                parameter.BankPayableInvoice.CreatedDate = DateTime.Now;
                parameter.BankPayableInvoice.CreatedById = parameter.UserId;
                parameter.BankPayableInvoice.BankPayableInvoiceCode =
                    "UNC" + "-" + organ.OrganizationCode + DateTime.Now.Year +
                    (context.BankPayableInvoice.Count(b => b.BankPayableInvoicePaidDate.Year == DateTime.Now.Year) + 1)
                    .ToString("D5");

                var bankPayableInvoiceDupblicase = context.BankPayableInvoice.FirstOrDefault(x =>
                    x.BankPayableInvoiceCode == parameter.BankPayableInvoice.BankPayableInvoiceCode);
                if (bankPayableInvoiceDupblicase != null)
                {
                    return new CreateBankPayableInvoiceResult
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = "Mã phiếu UNC đã tồn tại"
                    };
                }

                var newBankPayableInvoiceMapping = parameter.BankPayableInvoiceMapping;
                newBankPayableInvoiceMapping.BankPayableInvoiceMappingId = Guid.NewGuid();
                newBankPayableInvoiceMapping.CreatedById = parameter.UserId;
                newBankPayableInvoiceMapping.CreatedDate = DateTime.Now;
                newBankPayableInvoiceMapping.BankPayableInvoiceId = newBankPayableInvoiceId;
                newBankPayableInvoiceMapping.ObjectId = parameter.BankPayableInvoiceMapping.ObjectId;

                var isEmp = context.Employee.Any(e => e.EmployeeId == newBankPayableInvoiceMapping.ObjectId);
                var isCus = context.Customer.Any(c => c.CustomerId == newBankPayableInvoiceMapping.ObjectId);
                var isVen = context.Vendor.Any(v => v.VendorId == newBankPayableInvoiceMapping.ObjectId);

                if (isEmp)
                {
                    newBankPayableInvoiceMapping.ReferenceType = 1;
                }
                else if (isVen)
                {
                    newBankPayableInvoiceMapping.ReferenceType = 2;
                }
                else if (isCus)
                {
                    newBankPayableInvoiceMapping.ReferenceType = 3;
                }
                else
                {
                    newBankPayableInvoiceMapping.ReferenceType = 0;
                }

                var vedor = context.Vendor.FirstOrDefault(c => c.VendorId == newBankPayableInvoiceMapping.ObjectId);
                if (vedor != null)
                {
                    vedor.TotalPayableValue = (vedor.TotalPayableValue ?? 0) + (parameter.BankPayableInvoice.BankPayableInvoiceAmount ?? 0)
                                            * (parameter.BankPayableInvoice.BankPayableInvoiceExchangeRate ?? 1);
                    vedor.TotalPayableValue = (vedor.TotalPayableValue ?? 0) - ((parameter.BankPayableInvoice.BankPayableInvoiceAmount ?? 0)
                                            * (parameter.BankPayableInvoice.BankPayableInvoiceExchangeRate ?? 1));
                    context.Vendor.Update(vedor);
                }

                context.BankPayableInvoice.Add(parameter.BankPayableInvoice.ToEntity());
                context.BankPayableInvoiceMapping.Add(newBankPayableInvoiceMapping.ToEntity());

                context.SaveChanges();

                return new CreateBankPayableInvoiceResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = CommonMessage.BankPayableInvoice.ADD_SUCCESS
                };
            }
            catch (Exception ex)
            {
                return new CreateBankPayableInvoiceResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = ex.Message
                };
            }
        }

        public SearchBankPayableInvoiceResult SearchBankPayableInvoice(SearchBankPayableInvoiceParameter parameter)
        {
            try
            {
                var listAllUser = context.User.ToList();
                var listAllEmployee = context.Employee.ToList();

                var user = listAllUser.First(x => x.UserId == parameter.UserId && x.Active == true);
                if (user == null)
                {
                    return new SearchBankPayableInvoiceResult()
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = "User không có quyền truy xuất dữ liệu trong hệ thống",
                    };
                }

                if (user.EmployeeId == null || user.EmployeeId == Guid.Empty)
                {
                    return new SearchBankPayableInvoiceResult()
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = "Lỗi dữ liệu",
                    };
                }

                var employeeId = user.EmployeeId;
                var employee = listAllEmployee.FirstOrDefault(x => x.EmployeeId == employeeId);

                if (employee == null)
                {
                    return new SearchBankPayableInvoiceResult()
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = "Lỗi dữ liệu"
                    };
                }

                var isManager = employee.IsManager;

                var reasonCategoryTypeId = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "LCH").CategoryTypeId;
                var listAllReason = context.Category.Where(x => x.CategoryTypeId == reasonCategoryTypeId).ToList();

                var statusCateoryTypeId = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "TCH").CategoryTypeId;
                var listAllStatus = context.Category.Where(x => x.CategoryTypeId == statusCateoryTypeId).ToList();

                var bankPayableInvoiceCode = parameter.PayableInvoiceCode == null ? "" : parameter.PayableInvoiceCode.Trim();
                var bankPayableInvoiceReasonIds = parameter.PayableReasonIdList;
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
                var lstBankPayableMap = context.BankPayableInvoiceMapping.Where(c => objectIds == null || objectIds.Count == 0 || objectIds.Contains(c.ObjectId.Value)).ToList();
                var lstBankPayableMapId = lstBankPayableMap.Select(c => c.BankPayableInvoiceId).ToList();

                var lst = context.BankPayableInvoice
                    .Where(x => (bankPayableInvoiceCode == "" ||
                                 x.BankPayableInvoiceCode.Contains(bankPayableInvoiceCode)) &&
                                (bankPayableInvoiceReasonIds == null || bankPayableInvoiceReasonIds.Count == 0 ||
                                 bankPayableInvoiceReasonIds.Contains(x.BankPayableInvoiceReason.Value)) &&
                                (lstBankPayableMapId.Contains(x.BankPayableInvoiceId)) &&
                                (createdByIds == null || createdByIds.Count == 0 ||
                                 createdByIds.Contains(x.CreatedById)) &&
                                (fromDate == null || fromDate == DateTime.MinValue || fromDate <= x.CreatedDate) &&
                                (toDate == null || toDate == DateTime.MinValue || toDate >= x.CreatedDate) &&
                                (statusIds == null || statusIds.Count == 0 || statusIds.Contains(x.StatusId.Value)))
                    .Select(m => new BankPayableInvoiceEntityModel
                    {
                        BankPayableInvoiceId = m.BankPayableInvoiceId,
                        BankPayableInvoiceCode = m.BankPayableInvoiceCode,
                        BankPayableInvoiceDetail = m.BankPayableInvoiceDetail,
                        BankPayableInvoicePrice = m.BankPayableInvoicePrice,
                        BankPayableInvoicePriceCurrency = m.BankPayableInvoicePriceCurrency,
                        BankPayableInvoiceExchangeRate = m.BankPayableInvoiceExchangeRate,
                        BankPayableInvoiceReason = m.BankPayableInvoiceReason,
                        BankPayableInvoiceNote = m.BankPayableInvoiceNote,
                        BankPayableInvoiceBankAccountId = m.BankPayableInvoiceBankAccountId,
                        BankPayableInvoiceAmount = m.BankPayableInvoiceAmount,
                        BankPayableInvoiceAmountText = m.BankPayableInvoiceAmountText,
                        BankPayableInvoicePaidDate = m.BankPayableInvoicePaidDate,
                        OrganizationId = m.OrganizationId,
                        StatusId = m.StatusId,
                        ReceiveAccountNumber = m.ReceiveAccountNumber,
                        ReceiveAccountName = m.ReceiveAccountName,
                        ReceiveBankName = m.ReceiveBankName,
                        ReceiveBranchName = m.ReceiveBranchName,
                        Active = m.Active,
                        CreatedById = m.CreatedById,
                        CreatedDate = m.CreatedDate,
                        UpdatedById = m.UpdatedById,
                        UpdatedDate = m.UpdatedDate,
                        BankPayableInvoiceReasonText =
                            listAllReason.FirstOrDefault(s => s.CategoryId == m.BankPayableInvoiceReason)
                                .CategoryName ?? "",
                        StatusName = listAllStatus.FirstOrDefault(c => c.CategoryId == m.StatusId).CategoryName
                    }).ToList();
                lst.ForEach(item =>
                {
                    var temp = lstBankPayableMap.FirstOrDefault(c => c.BankPayableInvoiceId == item.BankPayableInvoiceId);
                    if (temp != null)
                    {
                        item.ObjectName = GetObjectName(temp.ObjectId);
                    }
                    item.CreatedByName = GetCreateByName(item.CreatedById);
                    item.BankPayableInvoiceDetail = item.BankPayableInvoiceDetail ?? "";
                    item.BankPayableInvoiceNote = item.BankPayableInvoiceNote ?? "";
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

                if (isManager)
                {
                    List<Guid?> listGetAllChild = new List<Guid?>();
                    if (employee.OrganizationId != null)
                    {
                        listGetAllChild.Add(employee.OrganizationId.Value);
                        listGetAllChild = GetOrganizationChildrenId(employee.OrganizationId.Value, listGetAllChild);
                    }
                    var listEmployeeInChargeByManager = listAllEmployee.Where(x => (listGetAllChild == null || listGetAllChild.Count == 0 || listGetAllChild.Contains(x.OrganizationId))).ToList();
                    List<Guid> listEmployeeInChargeByManagerId = new List<Guid>();
                    List<Guid> listUserByManagerId = new List<Guid>();

                    listEmployeeInChargeByManager.ForEach(item =>
                    {
                        if (item.EmployeeId != null && item.EmployeeId != Guid.Empty)
                            listEmployeeInChargeByManagerId.Add(item.EmployeeId);
                    });

                    // Lấy danh sách nhân viên UserId mà user phụ trách
                    listEmployeeInChargeByManagerId.ForEach(item =>
                    {
                        var user_employee = listAllUser.FirstOrDefault(x => x.EmployeeId == item);
                        if (user_employee != null)
                            listUserByManagerId.Add(user_employee.UserId);
                    });


                    lst = lst.Where(x =>
                        (listUserByManagerId.Count == 0) || listUserByManagerId == null ||
                        (x.CreatedById != null && listUserByManagerId.Contains(x.CreatedById))).ToList();
                }
                else
                {
                    lst = lst.Where(x => x.CreatedById != null && x.CreatedById == user.UserId).ToList();
                }

                lst = lst.OrderByDescending(x => x.CreatedDate).ToList();

                return new SearchBankPayableInvoiceResult
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success",
                    BankPayableInvoiceList = lst
                };
            }
            catch (Exception ex)
            {
                return new SearchBankPayableInvoiceResult
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = ex.Message
                };
            }
        }

        private List<Guid?> GetOrganizationChildrenId(Guid? id, List<Guid?> list)
        {
            var Organization = context.Organization.Where(o => o.ParentId == id).ToList();
            Organization.ForEach(item =>
            {
                list.Add(item.OrganizationId);
                GetOrganizationChildrenId(item.OrganizationId, list);
            });

            return list;
        }

        public SearchBankBookPayableInvoiceResult SearchBankBookPayableInvoice(SearchBankBookPayableInvoiceParameter parameter)
        {
            try
            {
                var reasonCategoryTypeId = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "LCH")?.CategoryTypeId;
                var listAllReason = context.Category.Where(x => x.CategoryTypeId == reasonCategoryTypeId).ToList();

                var listIdUser = parameter.ListCreateById;

                var createdByIds = new List<Guid>();

                if (listIdUser.Count != 0)
                {
                    foreach (var item in listIdUser)
                    {
                        var temp = context.User.FirstOrDefault(u => u.EmployeeId == item)?.UserId;
                        if (temp != null)
                            createdByIds.Add(temp.Value);
                    }
                }
                else
                {
                    createdByIds = listIdUser;
                }

                var fromDate = parameter.FromPaidDate;
                var toDate = parameter.ToPaidDate;

                var lst = context.BankPayableInvoice.Join(context.BankPayableInvoiceMapping, bi => bi.BankPayableInvoiceId,
                        bm => bm.BankPayableInvoiceId,
                        (bi, bm) => new {bi, bm})
                    .Where(x => (parameter.BankAccountId == null || parameter.BankAccountId.Count == 0 ||
                                 parameter.BankAccountId.Contains(x.bi.BankPayableInvoiceBankAccountId.Value)) &&
                                (createdByIds == null || createdByIds.Count == 0 ||
                                 createdByIds.Contains(x.bi.CreatedById)) &&
                                (fromDate == null || fromDate == DateTime.MinValue || fromDate <= x.bi.CreatedDate) &&
                                (toDate == null || toDate == DateTime.MinValue || toDate >= x.bi.CreatedDate))
                    .Select(m => new BankPayableInvoiceEntityModel
                    {
                        BankPayableInvoiceId = m.bi.BankPayableInvoiceId,
                        BankPayableInvoiceCode = m.bi.BankPayableInvoiceCode,
                        BankPayableInvoiceDetail = m.bi.BankPayableInvoiceDetail,
                        BankPayableInvoicePrice = m.bi.BankPayableInvoicePrice,
                        BankPayableInvoicePriceCurrency = m.bi.BankPayableInvoicePriceCurrency,
                        BankPayableInvoiceExchangeRate = m.bi.BankPayableInvoiceExchangeRate,
                        BankPayableInvoiceReason = m.bi.BankPayableInvoiceReason,
                        BankPayableInvoiceNote = m.bi.BankPayableInvoiceNote,
                        BankPayableInvoiceBankAccountId = m.bi.BankPayableInvoiceBankAccountId,
                        BankPayableInvoiceAmount = m.bi.BankPayableInvoiceAmount,
                        BankPayableInvoiceAmountText = m.bi.BankPayableInvoiceAmountText,
                        BankPayableInvoicePaidDate = m.bi.BankPayableInvoicePaidDate,
                        OrganizationId = m.bi.OrganizationId,
                        StatusId = m.bi.StatusId,
                        ReceiveAccountNumber = m.bi.ReceiveAccountNumber,
                        ReceiveAccountName = m.bi.ReceiveAccountName,
                        ReceiveBankName = m.bi.ReceiveBankName,
                        ReceiveBranchName = m.bi.ReceiveBranchName,
                        Active = m.bi.Active,
                        CreatedById = m.bi.CreatedById,
                        CreatedDate = m.bi.CreatedDate,
                        UpdatedById = m.bi.UpdatedById,
                        UpdatedDate = m.bi.UpdatedDate,
                        BankPayableInvoiceReasonText =
                            listAllReason.FirstOrDefault(s => s.CategoryId == m.bi.BankPayableInvoiceReason).CategoryName ??
                            "",
                        ObjectId = m.bm.ObjectId,
                    }).ToList();

                lst.ForEach(item =>
                {
                    item.ObjectName = GetObjectName(item.ObjectId);
                    item.CreatedByName = GetCreateByName(item.CreatedById);
                    item.BankPayableInvoiceDetail = item.BankPayableInvoiceDetail ?? "";
                    item.BankPayableInvoiceNote = item.BankPayableInvoiceNote ?? "";
                });

                return new SearchBankBookPayableInvoiceResult()
                {
                    StatusCode = lst.Count > 0 ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed,
                    MessageCode = lst.Count > 0 ? "" : CommonMessage.PayableInvoice.NO_INVOICE,
                    BankPayableInvoiceList = lst.OrderByDescending(l => l.CreatedDate).ToList(),
                };
            }
            catch (Exception e)
            {
                return new SearchBankBookPayableInvoiceResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetBankPayableInvoiceByIdResult GetBankPayableInvoiceById(GetBankPayableInvoiceByIdParameter parameter)
        {
            try
            {
                var reasonCategoryTypeId = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "LCH").CategoryTypeId;
                var listAllReason = context.Category.Where(x => x.CategoryTypeId == reasonCategoryTypeId).ToList();

                var statusCateoryTypeId = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "TCH").CategoryTypeId;
                var listAllStatus = context.Category.Where(x => x.CategoryTypeId == statusCateoryTypeId).ToList();

                var bankPay = context.BankPayableInvoice.FirstOrDefault(x => (parameter.BankPayableInvoiceId == null || parameter.BankPayableInvoiceId == Guid.Empty || parameter.BankPayableInvoiceId == x.BankPayableInvoiceId));

                var bankPayMaping = context.BankPayableInvoiceMapping.FirstOrDefault(x => (parameter.BankPayableInvoiceId == null || parameter.BankPayableInvoiceId == Guid.Empty || parameter.BankPayableInvoiceId == x.BankPayableInvoiceId));
                BankPayableInvoiceEntityModel data = new BankPayableInvoiceEntityModel();
                data.BankPayableInvoiceId = bankPay.BankPayableInvoiceId;
                data.BankPayableInvoiceCode = bankPay.BankPayableInvoiceCode;
                data.BankPayableInvoiceDetail = bankPay.BankPayableInvoiceDetail;
                data.BankPayableInvoicePrice = bankPay.BankPayableInvoicePrice;
                data.BankPayableInvoicePriceCurrency = bankPay.BankPayableInvoicePriceCurrency;
                data.BankPayableInvoiceExchangeRate = bankPay.BankPayableInvoiceExchangeRate;
                data.BankPayableInvoiceReason = bankPay.BankPayableInvoiceReason;
                data.BankPayableInvoiceNote = bankPay.BankPayableInvoiceNote;
                data.BankPayableInvoiceBankAccountId = bankPay.BankPayableInvoiceBankAccountId;
                data.BankPayableInvoiceAmount = bankPay.BankPayableInvoiceAmount;
                data.BankPayableInvoiceAmountText = MoneyHelper.Convert((decimal)data.BankPayableInvoiceAmount);
                data.BankPayableInvoicePaidDate = bankPay.BankPayableInvoicePaidDate;
                data.OrganizationId = bankPay.OrganizationId;
                data.StatusId = bankPay.StatusId;
                data.ReceiveAccountNumber = bankPay.ReceiveAccountNumber;
                data.ReceiveAccountName = bankPay.ReceiveAccountName;
                data.ReceiveBankName = bankPay.ReceiveBankName;
                data.ReceiveBranchName = bankPay.ReceiveBranchName;
                data.Active = bankPay.Active;
                data.CreatedById = bankPay.CreatedById;
                data.CreatedDate = bankPay.CreatedDate;
                data.UpdatedById = bankPay.UpdatedById;
                data.UpdatedDate = bankPay.UpdatedDate;
                data.BankPayableInvoiceReasonText = listAllReason.FirstOrDefault(s => s.CategoryId == bankPay.BankPayableInvoiceReason).CategoryName;
                data.ObjectId = bankPayMaping.ObjectId;
                data.StatusName = listAllStatus.FirstOrDefault(c => c.CategoryId == bankPay.StatusId).CategoryName;

                data.ObjectName = GetObjectName(data.ObjectId);

                data.CreatedByName = GetCreateByName(data.CreatedById);

                return new GetBankPayableInvoiceByIdResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success",
                    BankPayableInvoice = data,
                };
            }
            catch (Exception e)
            {
                return new GetBankPayableInvoiceByIdResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public ExportPayableInvoiceResult ExportPayableInvoice(ExportPayableInvoiceParameter parameter)
        {
            try
            {
                string html = ExportPdf.GetStringHtml("PayableInvTemplate.html");
                var company = context.CompanyConfiguration.FirstOrDefault(c => c.CompanyId != null);
                var invoice =
                    context.PayableInvoice.FirstOrDefault(r => r.PayableInvoiceId == parameter.PayableInvoiceId);

                if (invoice == null)
                {
                    return new ExportPayableInvoiceResult()
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = "Không tồn tại bản ghi trên hệ thống"
                    };
                }

                var reason = context.Category.FirstOrDefault(c => c.CategoryId == invoice.PayableInvoiceReason).CategoryName;
                var currency = context.Category.FirstOrDefault(c => c.CategoryId == invoice.PayableInvoicePriceCurrency).CategoryName;
                var org = context.Organization.FirstOrDefault(o => o.OrganizationId == invoice.OrganizationId).OrganizationName;
                var obj = context.PayableInvoiceMapping.FirstOrDefault(bp => bp.PayableInvoiceId == invoice.PayableInvoiceId);
                var objectId = obj == null ? Guid.Empty : obj.ObjectId;
                string objectName = GetObjectNameWithoutCode(objectId);
                html = html.Replace("[CompanyName]", company.CompanyName.ToUpper());
                html = html.Replace("[CompanyAddress]", company.CompanyAddress);
                html = html.Replace("[Code]", invoice.PayableInvoiceCode);
                html = html.Replace("[CreateDateDay]", invoice.CreatedDate.Day.ToString());
                html = html.Replace("[CreateMonth]", invoice.CreatedDate.Month.ToString());
                html = html.Replace("[CreateYear]", invoice.CreatedDate.Year.ToString());
                html = html.Replace("[Content]", invoice.PayableInvoiceDetail);
                html = html.Replace("[Price]", invoice.PayableInvoicePrice.Value.ToString("#,#."));
                html = html.Replace("[PriceString]", MoneyHelper.Convert(invoice.PayableInvoicePrice.Value));
                html = html.Replace("[Note]", invoice.PayableInvoiceNote);
                html = html.Replace("[PaidDate]", invoice.PaidDate.ToString("dd/MM/yyyy"));
                html = html.Replace("[Reason]", reason);
                html = html.Replace("[Object]", objectName);
                html = html.Replace("[Status]", invoice.PayableInvoiceNote);
                html = html.Replace("[Organization]", org);
                html = html.Replace("[CurrencyCode]", currency);
                html = html.Replace("[Name]", invoice.RecipientName);
                html = html.Replace("[Address]", invoice.RecipientAddress);

                // Export html to Pdf
                string rootFolder = _hostingEnvironment.WebRootPath + "\\ExportedPDF";
                string fileName = @"ExportedPayable.pdf";
                var bankInvoicePdf = ExportPdf.HtmlToPdfExport(html, Path.Combine(rootFolder, fileName), PdfPageSize.A5, PdfPageOrientation.Landscape, string.Empty);

                return new ExportPayableInvoiceResult()
                {
                    PayableInvoicePdf = bankInvoicePdf,
                    Code = invoice.PayableInvoiceCode,
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success"
                };
            }
            catch (Exception e)
            {
                return new ExportPayableInvoiceResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public ExportBankPayableInvoiceResult ExportBankPayableInvoice(ExportBankPayableInvoiceParameter parameter)
        {
            try
            {
                string html = ExportPdf.GetStringHtml("BankPayableInvTemplate.html");
                var company = context.CompanyConfiguration.FirstOrDefault(c => c.CompanyId != null);
                var bankInvoice =
                    context.BankPayableInvoice.FirstOrDefault(r => r.BankPayableInvoiceId == parameter.BankPayableInvId);

                if (bankInvoice == null)
                {
                    return new ExportBankPayableInvoiceResult()
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = "Bản ghi không tồn tại trên hệ thống"
                    };
                }

                var reason = context.Category.FirstOrDefault(c => c.CategoryId == bankInvoice.BankPayableInvoiceReason).CategoryName;
                var status = context.Category.FirstOrDefault(c => c.CategoryId == bankInvoice.StatusId).CategoryName;
                var currency = context.Category.FirstOrDefault(c => c.CategoryId == bankInvoice.BankPayableInvoicePriceCurrency).CategoryName;
                var org = context.Organization.FirstOrDefault(o => o.OrganizationId == bankInvoice.OrganizationId).OrganizationName;
                var obj = context.BankPayableInvoiceMapping.FirstOrDefault(bp => bp.BankPayableInvoiceId == bankInvoice.BankPayableInvoiceId);
                var objectId = obj == null ? Guid.Empty : obj.ObjectId;
                var empId = context.User.FirstOrDefault(u => u.UserId == bankInvoice.CreatedById).EmployeeId;
                var name = context.Employee.FirstOrDefault(e => e.EmployeeId == empId).EmployeeName;
                string objectName = GetObjectNameWithoutCode(objectId);

                var payerBank = context.BankAccount.FirstOrDefault(b => b.BankAccountId == bankInvoice.BankPayableInvoiceBankAccountId);

                html = html.Replace("[CompanyName]", company.CompanyName.ToUpper());
                html = html.Replace("[CompanyAddress]", company.CompanyAddress);
                html = html.Replace("[Code]", bankInvoice.BankPayableInvoiceCode);
                html = html.Replace("[CreateDateDay]", bankInvoice.CreatedDate.Day.ToString());
                html = html.Replace("[CreateMonth]", bankInvoice.CreatedDate.Month.ToString());
                html = html.Replace("[CreateYear]", bankInvoice.CreatedDate.Year.ToString());
                html = html.Replace("[Content]", bankInvoice.BankPayableInvoiceDetail);
                html = html.Replace("[Price]", bankInvoice.BankPayableInvoiceAmount.Value.ToString("#,#."));
                html = html.Replace("[PriceString]", MoneyHelper.Convert(bankInvoice.BankPayableInvoiceAmount.Value));
                html = html.Replace("[Note]", bankInvoice.BankPayableInvoiceNote);
                html = html.Replace("[PaidDate]", bankInvoice.BankPayableInvoicePaidDate.ToString("dd/MM/yyyy"));
                html = html.Replace("[Reason]", reason);
                html = html.Replace("[Object]", objectName);
                html = html.Replace("[Status]", status);
                html = html.Replace("[Organization]", org);
                html = html.Replace("[CurrencyCode]", currency);
                html = html.Replace("[CreatedBy]", name);

                if (payerBank == null)
                {
                    html = html.Replace("[PayerAccountNumber]", "");
                    html = html.Replace("[PayerAccountName]", "");
                    html = html.Replace("[PayerBankName]", "");
                    html = html.Replace("[PayerBranchName]", "");
                }
                else
                {
                    html = html.Replace("[PayerAccountNumber]", payerBank.AccountNumber);
                    html = html.Replace("[PayerAccountName]", payerBank.AccountName);
                    html = html.Replace("[PayerBankName]", payerBank.BankName);
                    html = html.Replace("[PayerBranchName]", payerBank.BranchName);
                }

                html = html.Replace("[ReceiveAccountNumber]", bankInvoice.ReceiveAccountNumber);
                html = html.Replace("[ReceiveAccountName]", bankInvoice.ReceiveAccountName);
                html = html.Replace("[ReceiveBankName]", bankInvoice.ReceiveBankName);
                html = html.Replace("[ReceiveBranchName]", bankInvoice.ReceiveBranchName);

                // Export html to Pdf
                string rootFolder = _hostingEnvironment.WebRootPath + "\\ExportedPDF";
                string fileName = @"ExportedBankPayable.pdf";
                var bankInvoicePdf = ExportPdf.HtmlToPdfExport(html, Path.Combine(rootFolder, fileName), PdfPageSize.A5, PdfPageOrientation.Landscape, string.Empty);

                return new ExportBankPayableInvoiceResult()
                {
                    BankPayableInvoicePdf = bankInvoicePdf,
                    Code = bankInvoice.BankPayableInvoiceCode,
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success"
                };
            }
            catch (Exception e)
            {
                return new ExportBankPayableInvoiceResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
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
                var con = context.Contact.FirstOrDefault(c => c.ObjectId == objId);
                var ven = context.Vendor.FirstOrDefault(e => e.VendorId == objId);
                var cus = context.Customer.FirstOrDefault(c => c.CustomerId == objId);

                if (emp != null && con != null)
                {
                    return emp.EmployeeCode + " - " + emp.EmployeeName;
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
                var con = context.Contact.FirstOrDefault(c => c.ObjectId == objId);
                var ven = context.Vendor.FirstOrDefault(e => e.VendorId == objId);
                var cus = context.Customer.FirstOrDefault(c => c.CustomerId == objId);

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

        // Create by LongNH
        public GetMasterDataPayableInvoiceResult GetMasterDataPayableInvoice(GetMasterDataPayableInvoiceParameter parameter)
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

                var listVendor = context.Vendor.ToList();
                var listAllVendorContact =
                    context.Contact.Where(x => x.ObjectType == "VEN_CON" || x.ObjectType == "VEN").ToList();

                var categoryReasonType = listCategoryType.FirstOrDefault(ct => ct.CategoryTypeCode == "LCH");
                var reasonOfPaymentList = new List<CategoryEntityModel>();
                if (categoryReasonType != null)
                {
                    reasonOfPaymentList = listCategory
                        .Where(c => c.Active == true && (c.CategoryTypeId == categoryReasonType.CategoryTypeId)).Select(c =>
                            new CategoryEntityModel()
                            {
                                CategoryTypeId = c.CategoryTypeId,
                                CategoryId = c.CategoryId,
                                CategoryName = c.CategoryName,
                                CategoryCode = c.CategoryCode,
                                IsDefault = c.IsDefauld
                            }).ToList();
                }

                var categoryStatusType = listCategoryType.FirstOrDefault(ct => ct.CategoryTypeCode == "TCH");
                var statusOfPaymentList = new List<CategoryEntityModel>();
                if (categoryStatusType != null)
                {
                    statusOfPaymentList = listCategory.Where(c => c.CategoryTypeId == categoryStatusType.CategoryTypeId)
                        .Select(c => new CategoryEntityModel()
                        {
                            CategoryTypeId = c.CategoryTypeId,
                            CategoryId = c.CategoryId,
                            CategoryName = c.CategoryName,
                            CategoryCode = c.CategoryCode,
                            IsDefault = c.IsDefauld
                        }).ToList();
                }

                var categoryType = listCategoryType.FirstOrDefault(ct => ct.CategoryTypeCode == "LSO");
                var typeOfPaymentList = new List<CategoryEntityModel>();
                if (categoryType != null)
                {
                    typeOfPaymentList = listCategory
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
                var unitMoneyOfPaymentList = new List<CategoryEntityModel>();
                if (categoryStatusType != null)
                {
                    unitMoneyOfPaymentList = listCategory
                        .Where(c => c.CategoryTypeId == categoryUnitMoneyType.CategoryTypeId).Select(c =>
                            new CategoryEntityModel()
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

                #region Nếu tạo từ đơn hàng mua

                var payableInvoice = new PayableInvoiceEntityModel();
                if (parameter.VendorOrderId != null)
                {
                    var vendorOrder = context.VendorOrder.FirstOrDefault(x => x.VendorOrderId == parameter.VendorOrderId);
                    if (vendorOrder != null)
                    {
                        var contact = listAllVendorContact.FirstOrDefault(x =>
                            x.ObjectId == vendorOrder.VendorId && x.ObjectType == "VEN");
                    
                        payableInvoice.PayableInvoiceReason =
                            reasonOfPaymentList.FirstOrDefault(x => x.CategoryCode == "CTA")?.CategoryId;
                        payableInvoice.ObjectId = listVendor
                            .FirstOrDefault(x => x.VendorId == vendorOrder.VendorId)?.VendorId;
                        payableInvoice.RecipientName = listAllVendorContact
                            .FirstOrDefault(x => x.ContactId == vendorOrder.VendorContactId && x.ObjectType == "VEN_CON")
                            ?.FirstName.Trim();
                        payableInvoice.PayableInvoiceDetail =
                            "Thanh toán tiền cho đơn hàng - " + vendorOrder.VendorOrderCode;

                        var listPayableInvoice = context.PayableInvoice.Where(x => x.ObjectId == parameter.VendorOrderId)
                            .ToList();
                        decimal? tongDaTra = 0;
                    
                        listPayableInvoice.ForEach(item =>
                        {
                            tongDaTra += item.Amount;
                        });

                        payableInvoice.PayableInvoicePrice = vendorOrder.Amount - tongDaTra;
                    
                        if (contact != null)
                            payableInvoice.RecipientAddress = BuildAddress(contact.Address, contact.ProvinceId,
                                contact.DistrictId, contact.WardId);
                    }
                }

                #endregion

                return new GetMasterDataPayableInvoiceResult
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success",
                    ReasonOfPaymentList = reasonOfPaymentList,
                    StatusOfPaymentList = statusOfPaymentList,
                    TypesOfPaymentList = typeOfPaymentList,
                    UnitMoneyList = unitMoneyOfPaymentList,
                    OrganizationList = listOrganization,
                    CustomerList = customerList,
                    PayableInvoice = payableInvoice,
                };
            }
            catch (Exception e)
            {
                return new GetMasterDataPayableInvoiceResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        private string BuildAddress(string Address, Guid? ProvinceId, Guid? DistrictId, Guid? WardId)
        {
            var result = Address?.Trim() ?? "";

            var WardName = context.Ward.FirstOrDefault(x => x.WardId == WardId)?.WardName ?? "";
            if (!string.IsNullOrEmpty(WardName)) result = result == "" ? WardName : result + ", " + WardName;
            var DistrictName = context.District.FirstOrDefault(x => x.DistrictId == DistrictId)?.DistrictName ?? "";
            if (!string.IsNullOrEmpty(DistrictName)) result = result + ", " + DistrictName;
            var ProvinceName = context.Province.FirstOrDefault(x => x.ProvinceId == ProvinceId)?.ProvinceName ?? "";
            if (!string.IsNullOrEmpty(ProvinceName)) result = result + ", " + ProvinceName;

            return result;
        }

        // Create HaiLT
        public GetMasterDataBankPayableInvoiceResult GetMasterDataBankPayableInvoice(GetMasterDataBankPayableInvoiceParameter parameter)
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

                var categoryReasonType = listCategoryType.FirstOrDefault(ct => ct.CategoryTypeCode == "LCH");
                var reasonOfPaymentList = new List<CategoryEntityModel>();
                if (categoryReasonType != null)
                {
                    reasonOfPaymentList = listCategory
                        .Where(c => c.Active == true && c.CategoryTypeId == categoryReasonType.CategoryTypeId).Select(c =>
                            new CategoryEntityModel()
                            {
                                CategoryTypeId = c.CategoryTypeId,
                                CategoryId = c.CategoryId,
                                CategoryName = c.CategoryName,
                                CategoryCode = c.CategoryCode,
                                IsDefault = c.IsDefauld
                            }).ToList();
                }

                var categoryStatusType = listCategoryType.FirstOrDefault(ct => ct.CategoryTypeCode == "TCH");
                var statusOfPaymentList = new List<CategoryEntityModel>();
                if (categoryStatusType != null)
                {
                    statusOfPaymentList = listCategory.Where(c => c.CategoryTypeId == categoryStatusType.CategoryTypeId)
                        .Select(c => new CategoryEntityModel()
                        {
                            CategoryTypeId = c.CategoryTypeId,
                            CategoryId = c.CategoryId,
                            CategoryName = c.CategoryName,
                            CategoryCode = c.CategoryCode,
                            IsDefault = c.IsDefauld
                        }).ToList();
                }

                var companyBankAccount = context.BankAccount.Where(b => b.ObjectType == ObjectType.COMPANY).ToList();
                var typeOfPaymentList = new List<BankAccountEntityModel>();
                if (companyBankAccount != null)
                {
                    typeOfPaymentList = companyBankAccount.Select(c => new BankAccountEntityModel()
                    {
                        BankAccountId = c.BankAccountId,
                        ObjectId = c.ObjectId,
                        ObjectType = c.ObjectType,
                        AccountNumber = c.AccountNumber,
                        BankName = c.BankName,
                        BankDetail = c.BankDetail,
                        BranchName = c.BranchName,
                        AccountName = c.AccountName,
                        CreatedById = c.CreatedById,
                        CreatedDate = c.CreatedDate,
                        UpdatedById = c.UpdatedById,
                        UpdatedDate = c.UpdatedDate,
                        Active = c.Active
                    }).ToList();
                }

                var categoryUnitMoneyType = listCategoryType.FirstOrDefault(ct => ct.CategoryTypeCode == "DTI");
                var unitMoneyOfPaymentList = new List<CategoryEntityModel>();
                if (categoryStatusType != null)
                {
                    unitMoneyOfPaymentList = listCategory
                        .Where(c => c.CategoryTypeId == categoryUnitMoneyType.CategoryTypeId).Select(c =>
                            new CategoryEntityModel()
                            {
                                CategoryTypeId = c.CategoryTypeId,
                                CategoryId = c.CategoryId,
                                CategoryName = c.CategoryName,
                                CategoryCode = c.CategoryCode,
                                IsDefault = c.IsDefauld
                            }).ToList();
                }
            
                var _vendorList = context.Vendor.Where(c => c.Active == true).OrderBy(x => x.VendorName).ToList();
                var vendorList = new List<VendorEntityModel>();
                _vendorList.ForEach(item =>
                {
                    var vend = new VendorEntityModel(item);
                    vendorList.Add(vend);
                });

                #region Nếu tạo từ đơn hàng mua

                var payableInvoice = new BankPayableInvoiceEntityModel();
            
                if (parameter.VendorOrderId != null)
                {
                    var vendorOrder = context.VendorOrder.FirstOrDefault(x => x.VendorOrderId == parameter.VendorOrderId);
                    if (vendorOrder != null)
                    {
                        payableInvoice.BankPayableInvoiceReason =
                            reasonOfPaymentList.FirstOrDefault(x => x.CategoryCode == "CTA")?.CategoryId;
                        payableInvoice.ObjectId = vendorList
                            .FirstOrDefault(x => x.VendorId == vendorOrder.VendorId)?.VendorId;
                    
                        payableInvoice.BankPayableInvoiceDetail =
                            "Thanh toán tiền cho đơn hàng - " + vendorOrder.VendorOrderCode;

                        payableInvoice.BankPayableInvoiceBankAccountId = vendorOrder.BankAccountId;

                        var listPayableInvoice = context.BankPayableInvoice.Where(x => x.ObjectId == parameter.VendorOrderId)
                            .ToList();
                        decimal? tongDaTra = 0;

                        listPayableInvoice.ForEach(item =>
                        {
                            tongDaTra += item.BankPayableInvoiceAmount;
                        });
                    
                        payableInvoice.BankPayableInvoiceAmount = vendorOrder.Amount - tongDaTra;
                    }
                }

                #endregion

                return new GetMasterDataBankPayableInvoiceResult
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success",
                    ReasonOfPaymentList = reasonOfPaymentList,
                    StatusOfPaymentList = statusOfPaymentList,
                    TypesOfPaymentList = typeOfPaymentList,
                    UnitMoneyList = unitMoneyOfPaymentList,
                    OrganizationList = listOrganization,
                    VendorList = vendorList,
                    BankPayableInvoice = payableInvoice
                };
            }
            catch (Exception e)
            {
                return new GetMasterDataBankPayableInvoiceResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetMasterDataPayableInvoiceSearchResult GetMasterDataPayableInvoiceSearch(GetMasterDataPayableInvoiceSearchParameter parameter)
        {
            try
            {
                var listCategoryType = context.CategoryType.ToList();
                var listCategory = context.Category.ToList();
                var _listEmployee = context.Employee.OrderBy(c => c.EmployeeCode).ToList();
                var listEmployee = new List<EmployeeEntityModel>();
                _listEmployee.ForEach(item =>
                {
                    var emp = new EmployeeEntityModel(item);
                    listEmployee.Add(emp);
                });

                var categoryReasonType = listCategoryType.FirstOrDefault(ct => ct.CategoryTypeCode == "LCH");
                var reasonOfPaymentList = new List<CategoryEntityModel>();
                if (categoryReasonType != null)
                {
                    reasonOfPaymentList = listCategory.Where(c => c.CategoryTypeId == categoryReasonType.CategoryTypeId)
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
                var statusOfPaymentList = new List<CategoryEntityModel>();
                if (categoryStatusType != null)
                {
                    statusOfPaymentList = listCategory.Where(c => c.CategoryTypeId == categoryStatusType.CategoryTypeId)
                        .Select(c => new CategoryEntityModel()
                        {
                            CategoryTypeId = c.CategoryTypeId,
                            CategoryId = c.CategoryId,
                            CategoryName = c.CategoryName,
                            CategoryCode = c.CategoryCode,
                            IsDefault = c.IsDefauld
                        }).ToList();
                }

                return new GetMasterDataPayableInvoiceSearchResult
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success",
                    ReasonOfPaymentList = reasonOfPaymentList,
                    StatusOfPaymentList = statusOfPaymentList,
                    lstUserEntityModel = listEmployee
                };
            }
            catch (Exception e)
            {
                return new GetMasterDataPayableInvoiceSearchResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetMasterDataSearchBankPayableInvoiceResult GetMasterDataSearchBankPayableInvoice(GetMasterDataSearchBankPayableInvoiceParameter parameter)
        {
            try
            {
                var listEmployee = new List<EmployeeEntityModel>();

                var listAllEmployee = context.Employee.Where(x => x.Active == true).ToList();
                var listAllUser = context.User.ToList();

                var user = listAllUser.First(x => x.UserId == parameter.UserId && x.Active == true);

                var employeeId = user.EmployeeId;
                var employee = listAllEmployee.FirstOrDefault(x => x.EmployeeId == employeeId);

                var reasonCategoryTypeId =
                    context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "LCH")?.CategoryTypeId;
                var _listAllReason = context.Category.Where(x => x.CategoryTypeId == reasonCategoryTypeId).ToList();
                var listAllReason = new List<CategoryEntityModel>();
                _listAllReason.ForEach(item =>
                {
                    var reason = new CategoryEntityModel(item);
                    listAllReason.Add(reason);
                });

                var statusCateoryTypeId =
                    context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "TCH")?.CategoryTypeId;
                var _listAllStatus = context.Category.Where(x => x.CategoryTypeId == statusCateoryTypeId).ToList();
                var listAllStatus = new List<CategoryEntityModel>();
                _listAllStatus.ForEach(item =>
                {
                    var status = new CategoryEntityModel(item);
                    listAllStatus.Add(status);
                });

                if (employee == null)
                {
                    return new GetMasterDataSearchBankPayableInvoiceResult()
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = "Lỗi dữ liệu"
                    };
                }

                var isManager = employee.IsManager;

                if (isManager)
                {
                    List<Guid?> listGetAllChild = new List<Guid?>();
                    if (employee.OrganizationId != null)
                    {
                        listGetAllChild.Add(employee.OrganizationId.Value);
                        listGetAllChild = GetOrganizationChildrenId(employee.OrganizationId.Value, listGetAllChild);
                    }

                    var _listEmployee = listAllEmployee.Where(x =>
                        (listGetAllChild == null || listGetAllChild.Count == 0 ||
                         listGetAllChild.Contains(x.OrganizationId))).ToList();
                    _listEmployee.ForEach(item =>
                    {
                        var emp = new EmployeeEntityModel(item);
                        listEmployee.Add(emp);
                    });
                }
                else
                {
                    listEmployee.Add(new EmployeeEntityModel(employee));
                }

                return new GetMasterDataSearchBankPayableInvoiceResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success",
                    ReasonOfPaymentList = listAllReason,
                    StatusOfPaymentList = listAllStatus,
                    EmployeeList = listEmployee
                };
            }
            catch (Exception e)
            {
                return new GetMasterDataSearchBankPayableInvoiceResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }
    }
}
