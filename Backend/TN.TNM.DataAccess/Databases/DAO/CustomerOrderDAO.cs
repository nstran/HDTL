using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using KellermanSoftware.CompareNetObjects;
using Microsoft.AspNetCore.Hosting;
using TN.TNM.Common;
using TN.TNM.Common.NotificationSetting;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.Order;
using TN.TNM.DataAccess.Messages.Results.Order;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.BankAccount;
using TN.TNM.DataAccess.Models.BillSale;
using TN.TNM.DataAccess.Models.Contract;
using TN.TNM.DataAccess.Models.Customer;
using TN.TNM.DataAccess.Models.Employee;
using TN.TNM.DataAccess.Models.Lead;
using TN.TNM.DataAccess.Models.Note;
using TN.TNM.DataAccess.Models.Order;
using TN.TNM.DataAccess.Models.Product;
using TN.TNM.DataAccess.Models.Quote;
using TN.TNM.DataAccess.Models.Vendor;
using TN.TNM.DataAccess.Models.WareHouse;
using OrderStatus = TN.TNM.DataAccess.Databases.Entities.OrderStatus;
using TN.TNM.DataAccess.Helper;
using TN.TNM.DataAccess.Models.CustomerOrder;
using TN.TNM.DataAccess.Models.Folder;
using TN.TNM.DataAccess.Models.LocalAddress;
using TN.TNM.DataAccess.Models.LocalPoint;
using TN.TNM.DataAccess.Models.ProductCategory;
using CustomerOrderEntityModel = TN.TNM.DataAccess.Models.Order.CustomerOrderEntityModel;
using System.Net;
using TN.TNM.DataAccess.Models.KiemTraTonKho;
using TN.TNM.DataAccess.Models.AttributeConfigurationEntityModel;
using TN.TNM.DataAccess.Messages.Results.Options;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TN.TNM.DataAccess.Consts.Product;
using TN.TNM.DataAccess.Models.Options;
using AutoMapper;
using TN.TNM.DataAccess.ConstType.Contact;
using TN.TNM.DataAccess.Models.PermissionConfiguration;
using TN.TNM.DataAccess.Messages.Results;
using TN.TNM.DataAccess.Models.Entities;
using static iTextSharp.text.pdf.AcroFields;
using System.Net.WebSockets;

namespace TN.TNM.DataAccess.Databases.DAO
{
    public class CustomerOrderDAO : BaseDAO, ICustomerOrderDataAccess
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IMapper _mapper;

        public CustomerOrderDAO(Databases.TNTN8Context _content, IAuditTraceDataAccess _iAuditTrace, IHostingEnvironment hostingEnvironment, IMapper mapper)
        {
            this.context = _content;
            this.iAuditTrace = _iAuditTrace;
            _hostingEnvironment = hostingEnvironment;
            _mapper = mapper;

        }

        public CreateCustomerOrderResult CreateCustomerOrder(CreateCustomerOrderParameter parameter)
        {
            var customerOrder = new CustomerOrder();
            var mess = "";
            try
            {
                //Tạo mới 
                if (parameter.OrderId == null)
                {
                    mess = "Tạo mới phiếu yêu cầu dịch vụ thành công!";
                    customerOrder.OrderId = Guid.NewGuid();
                    customerOrder.OrderCode = GenerateOrderCodeNew(parameter, null , false, null, null);

                    //Kiểm tra trường hợp đã tồn tại Mã đơn hàng
                    var duplicateOrder =
                        context.CustomerOrder.FirstOrDefault(x => x.OrderCode == customerOrder.OrderCode);
                    if (duplicateOrder != null)
                    {
                        return new CreateCustomerOrderResult
                        {
                            StatusCode = HttpStatusCode.ExpectationFailed,
                            MessageCode = CommonMessage.Order.ORDER_EXIST
                        };
                    }

                    var statusNewCustomerOrder = GeneralList.GetTrangThais("CustomerOrder").FirstOrDefault(x => x.Value == 1).Value;
                    customerOrder.CustomerId = parameter.CusOrder.CustomerId;
                    customerOrder.Vat = parameter.Vat.value;
                    customerOrder.DiscountType = parameter.DiscountType == 1 ? false : true ; // false: theo %, true: Theo số tiền
                    customerOrder.DiscountValue = parameter.DiscountValue;
                    customerOrder.OrderType = parameter.CusOrder.OrderType;
                    customerOrder.ObjectId = parameter.CusOrder.ObjectId;
                    customerOrder.StatusOrder = statusNewCustomerOrder;
                    customerOrder.IsOrderAction = false;
                    customerOrder.CreatedById = parameter.UserId;
                    customerOrder.CreatedDate = DateTime.Now;
                    customerOrder.UpdatedById = parameter.UserId;
                    customerOrder.UpdatedDate = DateTime.Now;
                    customerOrder.OrderProcessId = parameter.OrderProcessId;
                    customerOrder.ServicePacketId = parameter.ServicePacketId;
                    customerOrder.PaymentMethod = parameter.CusOrder.PaymentMethod;
                    customerOrder.PaymentContent = parameter.CusOrder.PaymentContent;
                    customerOrder.PaymentMethodNote = parameter.CusOrder.PaymentMethodNote;

                    //Nếu tạo phiếu yêu cầu => lấy OrderProcessId của phiếu cũ để mapp vào trường orderprocessId
                    if (parameter.CusOrder.OrderType == 2)
                    {
                        var oldCustomerOrder = context.CustomerOrder.FirstOrDefault(x => x.OrderId == parameter.CusOrder.ObjectId);
                        if (oldCustomerOrder != null)
                        {
                            customerOrder.OrderProcessId = oldCustomerOrder.OrderProcessId;
                        }
                    }


                    //Lưu thuộc tính của gói và tùy chọn dịch vụ
                    var listAttrDetail = new List<CustomerOrderExtension>();
                    //Lưu thông tin các tùy chọn dịch vụ
                    var listOrderDetail = new List<CustomerOrderDetail>();
                    //Lưu thông tin tùy chọn dịch vụ phát sinh
                    var listOrderDetailExten = new List<CustomerOrderDetailExten>();


                    genCustomerOrder(out listAttrDetail, out listOrderDetail, out listOrderDetailExten, parameter, customerOrder, true);

                    //Nếu tạo từ mobile => tạo thêm quy trình cho phiếu
                    if(parameter.IsMobile != null)
                    {     
                        if (parameter.IsMobile.Value == true)
                        {
                            var param = new CreateOrUpdateCustomerOrderProcessParameter();
                            var newOrderProcess = new OrderProcessEntityModel();
                            newOrderProcess.CustomerId = parameter.CusOrder.CustomerId;
                            newOrderProcess.ServicePacketId = parameter.ServicePacketId;
                            newOrderProcess.ProductCategoryId = context.ServicePacket.FirstOrDefault(x => x.Id == parameter.ServicePacketId)?.ProductCategoryId;
                            param.UserId = parameter.UserId;

                            param.OrderProcess = newOrderProcess;
                            var result1 = CreateOrUpdateCustomerOrderProcess(param);

                            //Nếu tạo orderProcessFail
                            if(result1.StatusCode == System.Net.HttpStatusCode.ExpectationFailed)
                            {
                                return new CreateCustomerOrderResult
                                {
                                    StatusCode = HttpStatusCode.ExpectationFailed,
                                    MessageCode = result1.MessageCode,
                                };
                            }
                            customerOrder.OrderProcessId = result1.Id;
                        }
                    }

                    context.CustomerOrder.Add(customerOrder);
                    context.CustomerOrderDetail.AddRange(listOrderDetail);
                    context.CustomerOrderExtension.AddRange(listAttrDetail);
                    context.CustomerOrderDetailExten.AddRange(listOrderDetailExten);

                    //Cập nhật bước quy trình: OrderProcessDetail
                    var result = UpdateOrderProcess(ProductConsts.CategoryCodeCreateStep, customerOrder.OrderProcessId, customerOrder.OrderType.Value,
                        false, customerOrder.OrderId, null);

                    //Nếu có lỗi thì báo lỗi
                    if (result.StatusCode == System.Net.HttpStatusCode.ExpectationFailed)
                    {
                        return new CreateCustomerOrderResult
                        {
                            StatusCode = HttpStatusCode.ExpectationFailed,
                            MessageCode = result.Message,
                        };
                    }

                    //Nếu từ mobile thì cập nhật các bước trước bước tạo phiếu yêu cầu thành hoàn thành
                    if (parameter.IsMobile != null)
                    {
                        if (parameter.IsMobile.Value == true)
                        {
                            //Lấy categoryId của bước tạo yêu cầu + xác nhận thanh toán
                            var cateTypeIdOfStep = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == ProductConsts.CategoryTypeCodeActionStep).CategoryTypeId;
                            var createStepId = context.Category.FirstOrDefault(x => x.CategoryTypeId == cateTypeIdOfStep && x.CategoryCode == ProductConsts.CategoryCodeCreateStep).CategoryId;
                            var listAllProcessDetail = context.OrderProcessDetail.Where(x => x.OrderProcessId == customerOrder.OrderProcessId).OrderByDescending(x => x.StepId).ToList();

                            var createOrderStepId = listAllProcessDetail.FirstOrDefault(x => x.CategoryId == createStepId).StepId;

                            //Lấy các bước trước nó và đnáh hoàn thành (3)
                            var listPreStep = listAllProcessDetail.Where(x => x.StepId < createOrderStepId).ToList();
                            listPreStep.ForEach(item => {
                                item.Status = 3;
                            });
                            context.OrderProcessDetail.UpdateRange(listPreStep);
                        }
                    }

                    //Cập nhật orderprocess thành đang thực hiện: 2
                    var orderProcess = context.OrderProcess.FirstOrDefault(x => x.Id == customerOrder.OrderProcessId);
                    if(orderProcess != null)
                    {
                        orderProcess.Status = 2; //Đang thực hiện
                        context.OrderProcess.Update(orderProcess);
                    }

                    context.SaveChanges();
                }
                //Cập nhật
                else
                {
                    mess = "Cập nhật phiếu yêu cầu dịch vụ thành công!";
                    customerOrder = context.CustomerOrder.FirstOrDefault(x => x.OrderId == parameter.OrderId);
                    if(customerOrder == null)
                    {
                        return new CreateCustomerOrderResult
                        {
                            StatusCode = HttpStatusCode.ExpectationFailed,
                            MessageCode = "Phiếu yêu cầu hỗ trợ không tồn tại trên hệ thống!"
                        };
                    }

                    customerOrder.UpdatedById = parameter.UserId;
                    customerOrder.UpdatedDate = DateTime.Now;
                    context.CustomerOrder.Update(customerOrder);

                    //Lấy các detail cũ và xóa
                    var listDetail = context.CustomerOrderDetail.Where(x => x.OrderId == customerOrder.OrderId).ToList();
                    var listDetailId = listDetail.Select(x => x.OrderDetailId).ToList();
                    var listAttrOptionAndPacket = context.CustomerOrderExtension
                        .Where(x => listDetailId.Contains(x.OrderDetailId.Value) || x.OrderDetailId == customerOrder.OrderId) // Thuộc tính của option || thuộc tính của packet trong phiếu
                        .ToList();
                    var listOrderDetailExtenOld = context.CustomerOrderDetailExten.Where(x => x.OrderId == parameter.OrderId).ToList();
                    context.CustomerOrderDetail.RemoveRange(listDetail);
                    context.CustomerOrderExtension.RemoveRange(listAttrOptionAndPacket);
                    context.CustomerOrderDetailExten.RemoveRange(listOrderDetailExtenOld);

                    customerOrder.CustomerId = parameter.CusOrder.CustomerId;
                    customerOrder.Vat = parameter.Vat.value;
                    customerOrder.DiscountType = parameter.DiscountType == 1 ? false : true; // false: theo %, true: Theo số tiền
                    customerOrder.StatusOrder = parameter.CusOrder.StatusOrder;
                    customerOrder.OrderType = parameter.CusOrder.OrderType;
                    customerOrder.ObjectId = parameter.CusOrder.ObjectId;
                    customerOrder.DiscountValue = parameter.DiscountValue;
                    customerOrder.IsOrderAction = false;
                    customerOrder.UpdatedById = parameter.UserId;
                    customerOrder.UpdatedDate = DateTime.Now;
                    customerOrder.PaymentMethod = parameter.CusOrder.PaymentMethod;
                    customerOrder.PaymentContent = parameter.CusOrder.PaymentContent;
                    customerOrder.PaymentMethodNote = parameter.CusOrder.PaymentMethodNote;
                    parameter.OrderProcessId = customerOrder.OrderProcessId;
                    context.CustomerOrder.Update(customerOrder);

                    //Lưu thuộc tính của gói và tùy chọn dịch vụ
                    var listAttrDetail = new List<CustomerOrderExtension>();
                    //Lưu thông tin các tùy chọn dịch vụ
                    var listOrderDetail = new List<CustomerOrderDetail>();
                    //Lưu thông tin tùy chọn dịch vụ phát sinh
                    var listOrderDetailExten = new List<CustomerOrderDetailExten>();

                    genCustomerOrder(out listAttrDetail, out listOrderDetail, out listOrderDetailExten, parameter, customerOrder, false);

                    context.CustomerOrderDetail.AddRange(listOrderDetail);
                    context.CustomerOrderExtension.AddRange(listAttrDetail);
                    context.CustomerOrderDetailExten.AddRange(listOrderDetailExten);

                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                return new CreateCustomerOrderResult
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message,
                };
            }
            #region lưu nhật ký hệ thống

            //LogHelper.AuditTrace(context, ActionName.Create, ObjectName.CUSTOMERORDER, parameter.CustomerOrder.OrderId, parameter.UserId);

            #endregion
            
            return new CreateCustomerOrderResult
            {
                StatusCode = HttpStatusCode.OK,
                MessageCode = mess,
                CustomerOrderID = customerOrder.OrderId,
                CustomerOrderCode = customerOrder.OrderCode,
            };
        }

        private void genCustomerOrder(out List<CustomerOrderExtension> listAttrDetail, out List<CustomerOrderDetail> listOrderDetail,
            out List<CustomerOrderDetailExten>  listOrderDetailExten,
            CreateCustomerOrderParameter parameter,CustomerOrder customerOrder,bool isCreateOrder)
        {
            listAttrDetail = new List<CustomerOrderExtension>();
            listOrderDetail = new List<CustomerOrderDetail>();
            listOrderDetailExten = new List<CustomerOrderDetailExten>();

            var listAttrDetailTemp = new List<CustomerOrderExtension>();
            var listOrderDetailTemp = new List<CustomerOrderDetail>();
            var listOrderDetailExtenTemp = new List<CustomerOrderDetailExten>();

            //Thuộc tính của gói
            var listAttrPack = parameter.ListAttrPackAndOption.Where(x => x.ObjectType == "2").ToList();
            listAttrPack.ForEach(item =>
            {
                var newExten = _mapper.Map<CustomerOrderExtension>(item);
                newExten.Id = Guid.NewGuid();
                newExten.OrderDetailId = customerOrder.OrderId; //1 phiếu nhiều gói dịch vụ
                listAttrDetailTemp.Add(newExten);
            });

            var listTreeOptionOfPack = context.ServicePacketMappingOptions.Where(x =>x.ServicePacketId == parameter.ServicePacketId).ToList();

            //Tùy chọn và thuộc tính của tùy chọn dịch vụ
            dynamic indexDetail = 1;
            parameter.ListCustomerDetail.ForEach(item =>
            {
                var newDetail = _mapper.Map<CustomerOrderDetail>(item);
                newDetail.OrderDetailId = Guid.NewGuid();
                newDetail.OrderId = customerOrder.OrderId;
                newDetail.PacketServiceId = item.ServicePacketId;
                newDetail.Stt = indexDetail;
                newDetail.Vat = item.Vat;
                listOrderDetailTemp.Add(newDetail);
                indexDetail++;
                //Thuộc tính của tùy chọn
                var optionId = listTreeOptionOfPack.FirstOrDefault(x => x.Id == item.OptionId).OptionId;
                var listAttrOp = parameter.ListAttrPackAndOption.Where(x => x.ObjectType == "1" && x.ObjectId == optionId && x.ServicePacketMappingOptionsId == item.OptionId).ToList();
                listAttrOp.ForEach(attrOp =>
                {
                    var newExten = _mapper.Map<CustomerOrderExtension>(attrOp);
                    newExten.Id = Guid.NewGuid();
                    newExten.OrderDetailId = newDetail.OrderDetailId;// 1 gói có nhiều tùy chọn
                    newExten.ServicePacketMappingOptionsId = newDetail.OptionId;
                    listAttrDetailTemp.Add(newExten);
                });
            });

            var statusAcceptOrderDetailExten = GeneralList.GetTrangThais("OrderDetailExten").FirstOrDefault(x => x.Value == 3).Value;
            //Dịch vụ phát sinh
            dynamic indexDetailExten = 1;

            parameter.ListOrderDetailExten.ForEach(item =>
            {
                var newDetail = _mapper.Map<CustomerOrderDetailExten>(item);
                newDetail.Id = Guid.NewGuid();
                newDetail.OrderId = customerOrder.OrderId;
                newDetail.Status = statusAcceptOrderDetailExten;
                newDetail.ServicePacketId = parameter.ServicePacketId;
                newDetail.Stt = indexDetailExten;
                if (isCreateOrder == false)
                {
                    newDetail.Status = item.Status != null ? item.Status.Value : statusAcceptOrderDetailExten;
                }
                listOrderDetailExtenTemp.Add(newDetail);
            });
            listAttrDetail = listAttrDetailTemp;
            listOrderDetail = listOrderDetailTemp;
            listOrderDetailExten = listOrderDetailExtenTemp;
        }


        private bool UpdateStatusQuote(Guid _QuoteId)
        {
            var quote = context.Quote.FirstOrDefault(f => f.QuoteId == _QuoteId);
            if (quote != null)
            {
                var categoryTypeQuote = context.CategoryType.FirstOrDefault(f => f.CategoryTypeCode == "TGI");
                if (categoryTypeQuote != null)
                {
                    var statusQuote = context.Category.FirstOrDefault(f => f.CategoryTypeId == categoryTypeQuote.CategoryTypeId && f.CategoryCode == "DTH");
                    if (statusQuote != null)
                    {
                        quote.StatusId = statusQuote.CategoryId;
                        context.Quote.Update(quote);
                        context.SaveChanges();
                        return true;
                    }
                }
            }
            return false;
        }

        public bool UpdateStatusLead(Guid leadId)
        {
            var lead = context.Lead.FirstOrDefault(f => f.LeadId == leadId);
            if (lead != null)
            {
                var categoryTypeLead = context.CategoryType.FirstOrDefault(f => f.CategoryTypeCode == "TLE");
                if (categoryTypeLead != null)
                {
                    var statusLead = context.Category.FirstOrDefault(f => f.CategoryTypeId == categoryTypeLead.CategoryTypeId && f.CategoryCode == "KHD");
                    if (statusLead != null)
                    {
                        lead.StatusId = statusLead.CategoryId;
                        context.Lead.Update(lead);
                        context.SaveChanges();
                        return true;
                    }
                }
            }
            return false;
        }

        private string GenerateCustomerCode()
        {
            //Auto gen CustomerCode 1911190001
            int currentYear = DateTime.Now.Year % 100;
            int currentMonth = DateTime.Now.Month;
            int currentDate = DateTime.Now.Day;
            var customer = context.Customer.OrderByDescending(or => or.CreatedDate).FirstOrDefault();
            int MaxNumberCode = 0;
            if (customer != null)
            {
                var customerCode = customer.CustomerCode;
                if (customerCode.Contains(currentYear.ToString()) && customerCode.Contains(currentMonth.ToString()) && customerCode.Contains(currentDate.ToString()))
                {
                    try
                    {
                        customerCode = customerCode.Substring(customerCode.Length - 4);
                        if (customerCode != "")
                        {
                            MaxNumberCode = Convert.ToInt32(customerCode) + 1;
                        }
                        else
                        {
                            MaxNumberCode = 1;
                        }
                    }
                    catch
                    {
                        MaxNumberCode = 1;
                    }

                }
                else
                {
                    MaxNumberCode = 1;
                }
            }
            else
            {
                MaxNumberCode = 1;
            }
            return string.Format("CTM{0}{1}{2}{3}", currentYear, currentMonth, currentDate, (MaxNumberCode).ToString("D4"));
        }
        public Contact InsertLeadToCustomer(Contact _Contact, Guid _UserId)
        {
            if (_Contact != null)
            {
                var statusType = context.CategoryType.FirstOrDefault(f => f.CategoryTypeCode == "THA");
                var groupType = context.CategoryType.FirstOrDefault(f => f.CategoryTypeCode == "NHA");
                var StatusCustomer = context.Category.FirstOrDefault(f =>
                    f.CategoryTypeId == statusType.CategoryTypeId && f.CategoryCode == "MOI");
                var groupCusstomer =
                    context.Category.FirstOrDefault(f =>
                        f.CategoryTypeId == groupType.CategoryTypeId && f.IsDefauld == true) ??
                    context.Category.FirstOrDefault(f => f.CategoryTypeId == groupType.CategoryTypeId);
                var typeLead = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "LHL");
                var listTypeLead =
                    context.Category.Where(x => x.Active == true && x.CategoryTypeId == typeLead.CategoryTypeId)
                        .ToList();
                var typeLeadId = context.Lead.FirstOrDefault(x => x.LeadId == _Contact.ObjectId).LeadTypeId;

                short customerType = 2;   //Khách hàng cá nhân
                if (typeLeadId != null)
                {
                    var code = listTypeLead.FirstOrDefault(x => x.CategoryId == typeLeadId).CategoryCode;
                    if (code == "KPL")
                    {
                        customerType = 2;   //Khách hàng cá nhân
                    }
                    else if (code == "KCL")
                    {
                        customerType = 1;   //Khách hàng doanh nghiệp
                    }
                }

                Customer newCustomer = new Customer()
                {
                    CustomerId = Guid.NewGuid(),
                    CustomerCode = GenerateCustomerCode(),
                    CustomerName = _Contact.FirstName + " " + _Contact.LastName,
                    CustomerGroupId = groupCusstomer != null ? groupCusstomer.CategoryId : Guid.Empty,
                    LeadId = _Contact.ObjectId,
                    StatusId = StatusCustomer != null ? StatusCustomer.CategoryId : Guid.Empty,
                    CustomerServiceLevelId = null,
                    PersonInChargeId = null,
                    CustomerType = customerType,
                    PaymentId = null,
                    FieldId = null,
                    ScaleId = null,
                    MaximumDebtValue = 0,
                    MaximumDebtDays = 0,
                    TotalSaleValue = 0,
                    TotalReceivable = 0,
                    NearestDateTransaction = null,
                    TotalCapital = 0,
                    BusinessRegistrationDate = null,
                    EnterpriseType = null,
                    TotalEmployeeParticipateSocialInsurance = 0,
                    TotalRevenueLastYear = 0,
                    BusinessType = null,
                    BusinessScale = null,
                    NumberCode = null,
                    YearCode = null,
                    MonthCode = null,
                    DateCode = null,
                    MainBusinessSector = null,
                    CustomerCareStaff = null,
                    Active = true,
                    CreatedById = _UserId,
                    CreatedDate = DateTime.Now,
                    UpdatedById = null,
                    UpdatedDate = null
                };

                context.Customer.Add(newCustomer);

                Contact newContact = _Contact;
                newContact.ContactId = Guid.NewGuid();
                newContact.ObjectId = newCustomer.CustomerId;
                newContact.ObjectType = "CUS";
                newContact.CreatedById = _UserId;
                newContact.CreatedDate = DateTime.Now;

                context.Contact.Add(newContact);
                context.SaveChanges();

                return newContact;
            }
            return null;
        }

        public GetAllCustomerOrderResult GetAllCustomerOrder(GetAllCustomerOrderParameter parameter)
        {
            try
            {
                List<CustomerOrderEntityModel> listOrder = new List<CustomerOrderEntityModel>();
                List<CustomerOrderEntityModel> newListOrderBy = new List<CustomerOrderEntityModel>();
                var listAllUser = context.User.ToList();
                var listAllEmployee = context.Employee.ToList();
                var listAllStatus = context.OrderStatus.ToList();
                var listAllContact = context.Contact.ToList();

                //check isManager
                var user = listAllUser.FirstOrDefault(x => x.UserId == parameter.UserId && x.Active == true);
                if (user == null)
                {
                    return new GetAllCustomerOrderResult()
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = "User không có quyền truy xuất dữ liệu trong hệ thống"
                    };
                }
                if (user.EmployeeId == null || user.EmployeeId == Guid.Empty)
                {
                    return new GetAllCustomerOrderResult()
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = "Lỗi dữ liệu"
                    };
                }

                var employeeId = user.EmployeeId;
                var employee = listAllEmployee.FirstOrDefault(x => x.EmployeeId == employeeId);
                if (employee == null)
                {
                    return new GetAllCustomerOrderResult()
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = "Lỗi dữ liệu"
                    };
                }
                var isManager = employee.IsManager;

                var orderCode = parameter.OrderCode == null ? "" : parameter.OrderCode.Trim();
                var customerName = parameter.CustomerName == null ? "" : parameter.CustomerName.Trim();
                var phone = parameter.Phone == null ? "" : parameter.Phone.Trim();
                var fromDate = parameter.OrderDateStart;
                var toDate = parameter.OrderDateEnd;
                var listStatusCode = new List<string>()
            {
                "DLV", // Đơn hàng bán
                "DRA", // Mới
                "IP", // Chờ phê duyệt
                "COMP", // Đóng
                "RTN", // Từ chối
                "CAN", // Hủy
            };
                int vat = parameter.Vat;

                var lstOrderBy = context.CustomerOrder.Join(context.Customer, or => or.CustomerId, cus => cus.CustomerId,
                        (or, cus) => new { or, cus })
                    .Where(x => x.or.Active == true && (customerName == "" || x.cus.CustomerName.Contains(customerName)) &&
                                (orderCode == "" || x.or.OrderCode.Contains(orderCode)) &&
                                (listStatusCode == null || listStatusCode.Count == 0 ) &&
                                (fromDate == null || fromDate == DateTime.MinValue || fromDate <= x.or.OrderDate) &&
                                (toDate == null || toDate == DateTime.MinValue || toDate >= x.or.OrderDate))
                    .Select(m => new CustomerOrderEntityModel
                    {
                        OrderId = m.or.OrderId,
                        OrderCode = m.or.OrderCode,
                        OrderDate = m.or.OrderDate,
                        Seller = m.or.Seller,
                        SellerName = listAllEmployee.FirstOrDefault(e => e.EmployeeId == m.or.Seller) == null ? "" : listAllEmployee.FirstOrDefault(e => e.EmployeeId == m.or.Seller).EmployeeName,
                        Description = m.or.Description,
                        Note = m.or.Note,
                        CustomerId = m.or.CustomerId.Value,
                        CustomerName = m.cus.CustomerName,
                        CustomerContactId = Guid.Empty,
                        PaymentMethod = m.or.PaymentMethod,
                        DaysAreOwed = m.or.DaysAreOwed,
                        MaxDebt = m.or.MaxDebt,
                        ReceivedDate = m.or.ReceivedDate,
                        ReceivedHour = m.or.ReceivedHour,
                        RecipientName = m.or.RecipientName,
                        LocationOfShipment = m.or.LocationOfShipment,
                        ShippingNote = m.or.ShippingNote,
                        RecipientPhone = m.or.RecipientPhone,
                        RecipientEmail = m.or.RecipientEmail,
                        PlaceOfDelivery = m.or.PlaceOfDelivery,
                        Amount = (decimal)((m.or.DiscountType == true)
                            ? (m.or.Amount * (1 - (m.or.DiscountValue / 100)))
                            : (m.or.Amount - m.or.DiscountValue)),
                        DiscountValue = m.or.DiscountValue,
                        StatusId = m.or.StatusId,
                        OrderStatusName = listAllStatus.FirstOrDefault(s => s.OrderStatusId == m.or.StatusId).Description,
                        CreatedById = m.or.CreatedById,
                        CreatedDate = m.or.CreatedDate,
                        UpdatedById = m.or.UpdatedById,
                        UpdatedDate = m.or.UpdatedDate,
                        Active = m.or.Active,
                        DiscountType = m.or.DiscountType,
                        SellerAvatarUrl = "",
                        SellerFirstName = "",
                        SellerLastName = "",
                        ListOrderDetail = ""
                    }).ToList();

                #region Kiểu LinQ
                //var lstOrderByParam = (from order in context.CustomerOrder
                //                       join customer in context.Customer on order.CustomerId equals customer.CustomerId
                //                       where order.Active == true &&
                //                             (orderCode == "" || order.OrderCode.Contains(orderCode)) &&
                //                             (customerName == "" || customer.CustomerName.Contains(customerName)) &&
                //                             (listStatusId == null || listStatusId.Count == 0 || listStatusId.Contains(order.StatusId)) &&
                //                             (fromDate == null || fromDate == DateTime.MinValue || fromDate <= order.OrderDate) &&
                //                             (toDate == null || toDate == DateTime.MinValue || toDate >= order.OrderDate)
                //                       select new CustomerOrderEntityModel
                //                       {
                //                           OrderId = order.OrderId,
                //                           OrderCode = order.OrderCode,
                //                           OrderDate = order.OrderDate,
                //                           Seller = order.Seller,
                //                           SellerName = listAllEmployee.FirstOrDefault(e => e.EmployeeId == order.Seller) == null ? "" : listAllEmployee.FirstOrDefault(e => e.EmployeeId == order.Seller).EmployeeName,
                //                           Description = order.Description,
                //                           Note = order.Note,
                //                           CustomerId = order.CustomerId,
                //                           CustomerName = customer.CustomerName,
                //                           CustomerContactId = Guid.Empty,
                //                           PaymentMethod = order.PaymentMethod,
                //                           DaysAreOwed = order.DaysAreOwed,
                //                           MaxDebt = order.MaxDebt,
                //                           ReceivedDate = order.ReceivedDate,
                //                           ReceivedHour = order.ReceivedHour,
                //                           RecipientName = order.RecipientName,
                //                           LocationOfShipment = order.LocationOfShipment,
                //                           ShippingNote = order.ShippingNote,
                //                           RecipientPhone = order.RecipientPhone,
                //                           RecipientEmail = order.RecipientEmail,
                //                           PlaceOfDelivery = order.PlaceOfDelivery,
                //                           Amount = (decimal)((order.DiscountType == true)
                //                               ? (order.Amount * (1 - (order.DiscountValue / 100)))
                //                               : (order.Amount - order.DiscountValue)),
                //                           DiscountValue = order.DiscountValue,
                //                           StatusId = order.StatusId,
                //                           OrderStatusName = listAllStatus.FirstOrDefault(x => x.OrderStatusId == order.StatusId).Description,
                //                           CreatedById = order.CreatedById,
                //                           CreatedDate = order.CreatedDate,
                //                           UpdatedById = order.UpdatedById,
                //                           UpdatedDate = order.UpdatedDate,
                //                           Active = order.Active,
                //                           DiscountType = order.DiscountType,
                //                           SellerAvatarUrl = "",
                //                           SellerFirstName = "",
                //                           SellerLastName = "",
                //                           ListOrderDetail = ""
                //                       }).ToList();
                #endregion

                var lstCustomerId = new List<Guid>();
                lstOrderBy.ForEach(item =>
                {
                    var dupblicate = lstCustomerId.FirstOrDefault(x => x == item.CustomerId);
                    if (dupblicate == Guid.Empty)
                    {
                        lstCustomerId.Add(item.CustomerId.Value);
                    }
                });

                var lstContact = context.Contact.Where(x =>
                    (x.ObjectType == ObjectType.CUSTOMER || x.ObjectType == ObjectType.CUSTOMERCONTACT) &&
                    (lstCustomerId == null || lstCustomerId.Count == 0 || lstCustomerId.Contains(x.ObjectId)) &&
                    (phone == "" || (x.Phone != null && x.Phone.ToLower().Contains(phone.ToLower())) ||
                     (x.WorkPhone != null && x.WorkPhone.ToLower().Contains(phone.ToLower())) ||
                     (x.OtherPhone != null && x.OtherPhone.ToLower().Contains(phone.ToLower())))).ToList();

                if (isManager)
                {
                    //Nếu là quản lý

                    //List phòng ban: chính nó và các phòng ban cấp dưới của nó
                    List<Guid?> listGetAllChild = new List<Guid?>();
                    if (employee.OrganizationId != null)
                    {
                        listGetAllChild.Add(employee.OrganizationId.Value);
                        listGetAllChild = getOrganizationChildrenId(employee.OrganizationId.Value, listGetAllChild);
                    }
                    //Lấy danh sách nhân viên EmployyeeId mà user phụ trách
                    var listEmployeeInChargeByManager = listAllEmployee.Where(x => (listGetAllChild == null || listGetAllChild.Count == 0 || listGetAllChild.Contains(x.OrganizationId))).ToList();
                    List<Guid> listEmployeeInChargeByManagerId = new List<Guid>();

                    listEmployeeInChargeByManager.ForEach(item =>
                    {
                        if (item.EmployeeId != Guid.Empty)
                            listEmployeeInChargeByManagerId.Add(item.EmployeeId);
                    });

                    lstOrderBy = lstOrderBy.Where(x =>
                        (listEmployeeInChargeByManagerId.Count == 0 ||
                         (x.Seller != null && listEmployeeInChargeByManagerId.Contains(x.Seller.Value)))).ToList();

                    List<Guid> lstOrderId = new List<Guid>();
                    lstOrderBy.ForEach(item =>
                    {
                        lstOrderId.Add(item.OrderId.Value);
                    });
                    var listOrderDetail = context.CustomerOrderDetail.Where(x => (lstOrderId == null || lstOrderId.Count == 0 || lstOrderId.Contains(x.OrderId))).ToList();
                    List<Guid> lstProductId = new List<Guid>();
                    listOrderDetail.ForEach(item =>
                    {
                        var dublicateProduct = lstProductId.FirstOrDefault(x => x == item.ProductId);
                        if (dublicateProduct == Guid.Empty)
                        {
                            if (item.ProductId != null)
                                lstProductId.Add(item.ProductId.Value);
                        }
                    });
                    var listProduct = context.Product.Where(x => (lstProductId == null || lstProductId.Count == 0 || lstProductId.Contains(x.ProductId))).ToList();

                    lstOrderBy.ForEach(item =>
                    {
                        var contact = lstContact.FirstOrDefault(x => x.ObjectId == item.CustomerId);
                        if (contact != null)
                        {
                            var contactCustomer = lstContact.FirstOrDefault(x => x.ObjectId == item.CustomerId && x.ObjectType == ObjectType.CUSTOMER);
                            var contactSeller = listAllContact.FirstOrDefault(x => x.ObjectId == item.Seller && x.ObjectType == ObjectType.EMPLOYEE);

                            item.CustomerContactId = contactCustomer == null ? Guid.Empty : contactCustomer.ContactId;
                            item.SellerFirstName = contactSeller == null ? "" : contactSeller.FirstName;
                            item.SellerLastName = contactSeller == null ? "" : contactSeller.LastName;

                            var orderDetail = listOrderDetail.Where(e => e.OrderId == item.OrderId).ToList();
                            if (orderDetail.Count > 0)
                            {
                                orderDetail.ForEach(currentOrderDetail =>
                                {
                                    var productName = "";
                                    var product = listProduct.FirstOrDefault(p => p.ProductId == currentOrderDetail.ProductId);
                                    if (product != null)
                                    {
                                        productName = product.ProductName + "; ";
                                    }
                                    item.ListOrderDetail += productName;
                                });
                            }
                            else
                            {
                                item.ListOrderDetail = "";
                            }

                            newListOrderBy.Add(item);
                        }
                    });
                }
                else
                {
                    //Nếu là nhân viên
                    lstOrderBy = lstOrderBy.Where(x => x.Seller == employeeId).ToList();

                    List<Guid> lstOrderId = new List<Guid>();
                    lstOrderBy.ForEach(item =>
                    {
                        lstOrderId.Add(item.OrderId.Value);
                    });
                    var listOrderDetail = context.CustomerOrderDetail.Where(x => (lstOrderId == null || lstOrderId.Count == 0 || lstOrderId.Contains(x.OrderId))).ToList();
                    List<Guid> lstProductId = new List<Guid>();
                    listOrderDetail.ForEach(item =>
                    {
                        var dublicateProduct = lstProductId.FirstOrDefault(x => x == item.ProductId);
                        if (dublicateProduct == Guid.Empty)
                        {
                            if (item.ProductId != null)
                                lstProductId.Add(item.ProductId.Value);
                        }
                    });
                    var listProduct = context.Product.Where(x => (lstProductId == null || lstProductId.Count == 0 || lstProductId.Contains(x.ProductId))).ToList();

                    lstOrderBy.ForEach(item =>
                    {
                        var contact = lstContact.FirstOrDefault(x => x.ObjectId == item.CustomerId);
                        if (contact != null)
                        {
                            var contactCustomer = lstContact.FirstOrDefault(x => x.ObjectId == item.CustomerId && x.ObjectType == ObjectType.CUSTOMER);
                            var contactSeller = listAllContact.FirstOrDefault(x => x.ObjectId == item.Seller && x.ObjectType == ObjectType.EMPLOYEE);

                            item.CustomerContactId = contactCustomer == null ? Guid.Empty : contactCustomer.ContactId;
                            item.SellerFirstName = contactSeller == null ? "" : contactSeller.FirstName;
                            item.SellerLastName = contactSeller == null ? "" : contactSeller.LastName;

                            var orderDetail = listOrderDetail.Where(e => e.OrderId == item.OrderId).ToList();
                            if (orderDetail.Count > 0)
                            {
                                orderDetail.ForEach(currentOrderDetail =>
                                {
                                    var productName = "";
                                    var product = listProduct.FirstOrDefault(p => p.ProductId == currentOrderDetail.ProductId);
                                    if (product != null)
                                    {
                                        productName = product.ProductName + "; ";
                                    }
                                    item.ListOrderDetail += productName;
                                });
                            }
                            else
                            {
                                item.ListOrderDetail = "";
                            }

                            newListOrderBy.Add(item);
                        }
                    });
                }

                #region comment by Giang
                //parameter.CustomerName = parameter.CustomerName == null ? parameter.CustomerName : parameter.CustomerName.Trim();
                //parameter.OrderCode = parameter.OrderCode == null ? parameter.OrderCode : parameter.OrderCode.Trim();
                //var commonOrganization = context.Organization.ToList();
                //int organizationCount = parameter.ListOrganizationId == null ? 0 : parameter.ListOrganizationId.Count;

                //if (parameter.ListOrganizationId != null && parameter.ListOrganizationId.Count == 1)
                //{
                //    parameter.ListOrganizationId = getOrganizationChildrenId(commonOrganization, parameter.ListOrganizationId[0], parameter.ListOrganizationId);
                //}
                #endregion

                #region By Hung code
                //var orderList = (from or in context.CustomerOrder
                //                 join employee in context.Employee on or.Seller equals employee.EmployeeId
                //                 join organization in context.Organization on employee.OrganizationId equals organization.OrganizationId
                //                 where (parameter.OrderStatusId==null||parameter.OrderStatusId.Contains(or.StatusId) || parameter.OrderStatusId.Count == 0)
                //                  && (parameter.Seller == null || parameter.Seller == or.Seller)
                //                  && (string.IsNullOrEmpty(parameter.OrderCode) || or.OrderCode.Contains(parameter.OrderCode))
                //                  //&& (string.IsNullOrEmpty(parameter.CustomerName) || cus.CustomerName.Contains(parameter.CustomerName))
                //                  && (parameter.OrderDateStart == null || parameter.OrderDateStart == DateTime.MinValue || parameter.OrderDateStart.Value <= or.OrderDate)
                //                  && (parameter.OrderDateEnd == null || parameter.OrderDateEnd == DateTime.MinValue || parameter.OrderDateEnd.Value >= or.OrderDate)
                //                  && (parameter.ListOrganizationId.Contains(organization.OrganizationId) || organizationCount == 0)
                //                 select new CustomerOrderEntityModel
                //                 {
                //                     OrderId = or.OrderId,
                //                     OrderCode = or.OrderCode,
                //                     OrderDate = or.OrderDate,
                //                     Seller = or.Seller,
                //                     Description = or.Description,
                //                     Note = or.Note,
                //                     CustomerId = or.CustomerId,
                //                     CustomerContactId = Guid.Empty,    //add by Giang
                //                     PaymentMethod = or.PaymentMethod,
                //                     DaysAreOwed = or.DaysAreOwed,
                //                     MaxDebt = or.MaxDebt,
                //                     ReceivedDate = or.ReceivedDate,
                //                     ReceivedHour = or.ReceivedHour,
                //                     RecipientName = or.RecipientName,
                //                     LocationOfShipment = or.LocationOfShipment,
                //                     ShippingNote = or.ShippingNote,
                //                     RecipientPhone = or.RecipientPhone,
                //                     RecipientEmail = or.RecipientEmail,
                //                     PlaceOfDelivery = or.PlaceOfDelivery,
                //                     Amount = (decimal)((or.DiscountType == true) ? (or.Amount * (1 - (or.DiscountValue / 100))) : (or.Amount - or.DiscountValue)),
                //                     DiscountValue = or.DiscountValue,
                //                     StatusId = or.StatusId,
                //                     CreatedById = or.CreatedById,
                //                     CreatedDate = or.CreatedDate,
                //                     UpdatedById = or.UpdatedById,
                //                     UpdatedDate = or.UpdatedDate,
                //                     Active = or.Active,
                //                     DiscountType = or.DiscountType,
                //                     SellerAvatarUrl = "",
                //                     SellerFirstName = "",
                //                     SellerLastName = "",
                //                     OrderStatusName = "",
                //                     CustomerName = ""
                //                 }).ToList();
                //List<Guid> listCustomerId = new List<Guid>();
                //List<Guid> listSeller = new List<Guid>();
                //List<Guid> listOrderStatusId = new List<Guid>();
                //List<Guid> listEmployeeId = new List<Guid>();
                //List<Guid> listOrderId = new List<Guid>();  //add by Giang

                //orderList.ForEach(item =>
                //{
                //    if (item.CustomerId != null)
                //        listCustomerId.Add(item.CustomerId);
                //    if (item.Seller != null)
                //    {
                //        listSeller.Add(item.Seller.Value);
                //        listEmployeeId.Add(item.Seller.Value);
                //    }
                //    if (item.StatusId != null)
                //        listOrderStatusId.Add(item.StatusId.Value);
                //    if (item.OrderId != null)               //add by Giang
                //        listOrderId.Add(item.OrderId);

                //});
                //var commonContact = context.Contact.Where(w => w.ObjectType == ObjectType.CUSTOMER || w.ObjectType == ObjectType.EMPLOYEE).ToList();
                //var listContact = commonContact.Where(e => listSeller.Contains(e.ObjectId)).ToList();
                //var listContactCustomer = commonContact.Where(e => listCustomerId.Contains(e.ObjectId) && e.ObjectType == "CUS").ToList();    //add by Giang
                //var listCustomer = context.Customer.Where(e => listCustomerId.Contains(e.CustomerId)).ToList();
                //var listOrderStatus = context.OrderStatus.Where(e => listOrderStatusId.Contains(e.OrderStatusId)).ToList();
                //var listEmployee = context.Employee.Where(e => listEmployeeId.Contains(e.EmployeeId)).ToList();
                //var listOrderDetail = context.CustomerOrderDetail.Where(e => listOrderId.Contains(e.OrderId)).ToList();
                //var listProduct = context.Product.ToList();

                //orderList.ForEach(item=> {
                //    if (item.Seller != null) {
                //        //item.SellerAvatarUrl = listContact.FirstOrDefault(e=>e.ObjectId==item.Seller).AvatarUrl;
                //        item.SellerFirstName = listEmployee.FirstOrDefault(e => e.EmployeeId == item.Seller).EmployeeName;
                //        item.SellerLastName = listContact.FirstOrDefault(e => e.ObjectId == item.Seller).LastName;
                //    }
                //    if (item.StatusId != null)
                //        item.OrderStatusName = listOrderStatus.FirstOrDefault(e => e.OrderStatusId == item.StatusId).Description;
                //    if (item.CustomerId != null)
                //    {
                //        var cus = listCustomer.FirstOrDefault(e => e.CustomerId == item.CustomerId);
                //        if (cus != null)
                //            item.CustomerName = cus.CustomerName;
                //        else
                //            item.CustomerName = "";
                //        var con = listContactCustomer.FirstOrDefault(e => e.ObjectId == item.CustomerId);
                //        if (con != null)
                //            item.CustomerContactId = con.ContactId;
                //        else
                //            item.CustomerContactId = Guid.Empty;
                //    }
                //    if (item.OrderId != null)
                //    {
                //        var orderDetail = listOrderDetail.Where(e => e.OrderId == item.OrderId).ToList();
                //        if (orderDetail.Count > 0)
                //        {
                //            orderDetail.ForEach(currentOrderDetail =>
                //            {
                //                var productName = listProduct.FirstOrDefault(p => p.ProductId == currentOrderDetail.ProductId) != null ?
                //                                    (listProduct.FirstOrDefault(p => p.ProductId == currentOrderDetail.ProductId).ProductName + "; ") : "";
                //                item.ListOrderDetail += productName;
                //            });
                //        } else
                //        {
                //            item.ListOrderDetail = "";
                //        }
                //    }
                //});
                //orderList = orderList.Where(e=> string.IsNullOrEmpty(parameter.CustomerName) || e.CustomerName.Contains(parameter.CustomerName)).OrderByDescending(or => or.CreatedDate).ToList();
                #endregion

                #region Comment By Hung
                //var orderList = (from c in context.Contact
                //                 join or in context.CustomerOrder on c.ObjectId equals or.Seller
                //                 join cus in context.Customer on or.CustomerId equals cus.CustomerId
                //                 join os in context.OrderStatus on or.StatusId equals os.OrderStatusId
                //                 join e in context.Employee on or.Seller equals e.EmployeeId
                //                 where (parameter.OrderStatusId.Contains(or.StatusId) || parameter.OrderStatusId.Count == 0)
                //                 && (parameter.Seller == null || parameter.Seller == or.Seller)
                //                 && (string.IsNullOrEmpty(parameter.OrderCode) || or.OrderCode.Contains(parameter.OrderCode))
                //                 && (string.IsNullOrEmpty(parameter.CustomerName) || cus.CustomerName.Contains(parameter.CustomerName))
                //                 && (parameter.OrderDateStart == null || parameter.OrderDateStart == DateTime.MinValue || parameter.OrderDateStart.Value <= or.OrderDate)
                //                 && (parameter.OrderDateEnd == null || parameter.OrderDateEnd == DateTime.MinValue || parameter.OrderDateEnd.Value >= or.OrderDate)
                //                 select new CustomerOrderEntityModel
                //                 {
                //                     OrderId = or.OrderId,
                //                     OrderCode = or.OrderCode,
                //                     OrderDate = or.OrderDate,
                //                     Description = or.Description,
                //                     Note = or.Note,
                //                     CustomerId = or.CustomerId,
                //                     PaymentMethod = or.PaymentMethod,
                //                     DaysAreOwed = or.DaysAreOwed,
                //                     MaxDebt = or.MaxDebt,
                //                     ReceivedDate = or.ReceivedDate,
                //                     ReceivedHour = or.ReceivedHour,
                //                     RecipientName = or.RecipientName,
                //                     LocationOfShipment = or.LocationOfShipment,
                //                     ShippingNote = or.ShippingNote,
                //                     RecipientPhone = or.RecipientPhone,
                //                     RecipientEmail = or.RecipientEmail,
                //                     PlaceOfDelivery = or.PlaceOfDelivery,
                //                     Amount = or.Amount,
                //                     DiscountValue = or.DiscountValue,
                //                     StatusId = or.StatusId,
                //                     CreatedById = or.CreatedById,
                //                     CreatedDate = or.CreatedDate,
                //                     UpdatedById = or.UpdatedById,
                //                     UpdatedDate = or.UpdatedDate,
                //                     Active = or.Active,
                //                     DiscountType = or.DiscountType,
                //                     SellerAvatarUrl = c.AvatarUrl,
                //                     SellerFirstName = e.EmployeeName,
                //                     SellerLastName = c.LastName,
                //                     OrderStatusName = os.Description,
                //                     CustomerName = cus.CustomerName
                //                 }
                //                 ).OrderByDescending(or => or.CreatedDate).ToList();
                #endregion

                #region Tìm những đơn hàng liên quan đến hóa đơn VAT
                if (parameter.Vat == 1)
                {
                    //Đơn hàng có cả hóa đơn VAT và không có hóa đơn VAT
                    listOrder = newListOrderBy;
                }
                else if (parameter.Vat == 2)
                {
                    //Đơn hàng có hóa đơn VAT
                    List<Guid> lstOrderId = new List<Guid>();
                    newListOrderBy.ForEach(item =>
                    {
                        lstOrderId.Add(item.OrderId.Value);
                    });
                    var listOrderDetail = context.CustomerOrderDetail.Where(x => (lstOrderId == null || lstOrderId.Count == 0 || lstOrderId.Contains(x.OrderId))).ToList();

                    newListOrderBy.ForEach(item =>
                    {
                        int countVat = listOrderDetail.Where(c => c.OrderId == item.OrderId && c.Vat > 0).Count();
                        if (countVat > 0)
                        {
                            listOrder.Add(item);
                        }
                    });
                }
                else if (parameter.Vat == 3)
                {
                    //Đơn hàng không có hóa đơn VAT
                    List<Guid> lstOrderId = new List<Guid>();
                    newListOrderBy.ForEach(item =>
                    {
                        lstOrderId.Add(item.OrderId.Value);
                    });
                    var listOrderDetail = context.CustomerOrderDetail.Where(x => (lstOrderId == null || lstOrderId.Count == 0 || lstOrderId.Contains(x.OrderId))).ToList();

                    newListOrderBy.ForEach(item =>
                    {
                        var countVat = listOrderDetail.Where(c => c.OrderId == item.OrderId && c.Vat > 0).Count();
                        if (countVat == 0)
                        {
                            listOrder.Add(item);
                        }
                    });
                }
                #endregion

                listOrder = listOrder.OrderByDescending(x => x.OrderDate).ToList();

                if (parameter.Top3NewOrder != null)
                {
                    var orderTop3List = listOrder.OrderByDescending(or => or.OrderCode).Take(5).ToList();
                    return new GetAllCustomerOrderResult()
                    {
                        StatusCode = HttpStatusCode.OK,
                        MessageCode = "Success",
                        OrderList = listOrder,
                        OrderTop5List = new List<CustomerOrderEntityModel>()
                    };
                }

                return new GetAllCustomerOrderResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success",
                    OrderList = listOrder,
                    OrderTop5List= new List<CustomerOrderEntityModel>()
                };
            }
            catch (Exception e)
            {
                return new GetAllCustomerOrderResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }

        }

        public ProfitAccordingCustomersResult SearchProfitAccordingCustomers(ProfitAccordingCustomersParameter parameter)
        {
            try
            {
                var listOrder = new List<ProfitAccordingCustomersModel>();

                var listAllUser = context.User.ToList();
                var listAllEmployee = context.Employee.ToList();

                //check isManager
                var user = listAllUser.FirstOrDefault(x => x.UserId == parameter.UserId && x.Active == true);
                var employeeId = user.EmployeeId;
                var employee = listAllEmployee.FirstOrDefault(x => x.EmployeeId == employeeId);

                var isManager = employee.IsManager;

                var orderCode = parameter.OrderCode == null ? "" : parameter.OrderCode.Trim();
                var customerName = parameter.CustomerName == null ? "" : parameter.CustomerName.Trim();
                var fromDate = parameter.FromDate;
                var toDate = parameter.ToDate;
                var listStatusId = parameter.ListStatusId;

                var allStatusOrder = context.OrderStatus.Where(c => c.Active == true).ToList();
                var commonOrderStatus = allStatusOrder.Where(w =>
                    w.OrderStatusCode == "DLV" ||
                    w.OrderStatusCode == "COMP").Select(w => w.OrderStatusId).ToList();

                #region Thêm điều kiện lọc sản phẩm
                List<Guid> lstOrdId = new List<Guid>();
                if (parameter.ProductId != null && parameter.ProductId != Guid.Empty)
                {
                    lstOrdId = context.CustomerOrderDetail.Where(co => co.ProductId == parameter.ProductId).Select(co => co.OrderId).ToList();
                }
                #endregion

                var listOrderDetail = context.CustomerOrder.Join(context.CustomerOrderDetail, or => or.OrderId, od => od.OrderId,
                    (or, od) => new { or, od })
                .Where(x => x.or.Active == true &&
                            (orderCode == "" || x.or.OrderCode.Contains(orderCode)) &&
                            (listStatusId == null || listStatusId.Count == 0 || listStatusId.Contains(x.or.StatusId.Value)) &&
                            (commonOrderStatus == null || commonOrderStatus.Count == 0 || commonOrderStatus.Contains(x.or.StatusId.Value)) &&
                            (lstOrdId == null || lstOrdId.Count == 0 || lstOrdId.Contains(x.or.OrderId)) &&
                            (parameter.QuoteId == null || parameter.QuoteId == Guid.Empty || x.or.QuoteId == parameter.QuoteId) &&
                            (parameter.Seller == null || parameter.Seller == Guid.Empty || x.or.Seller == parameter.Seller) &&
                            (fromDate == null || fromDate == DateTime.MinValue || fromDate <= x.or.OrderDate) &&
                            (toDate == null || toDate == DateTime.MinValue || toDate >= x.or.OrderDate))
                        .Select(m => new ProfitAccordingCustomersModel
                        {
                            CustomerId = m.or.CustomerId.Value,
                            CustomerName = null,
                            CustomerCode = null,
                            CapitalMoney = m.od.PriceInitial == null ? 0 : m.od.PriceInitial.Value,
                            GrossCapitalMoney = 0,
                            GrossProfit = 0,
                            GrossProfitMoney = 0,
                            ProfitMoney = m.or.Amount.Value - (m.or.DiscountType == true ? (m.or.Amount.Value * m.or.DiscountValue.Value / 100) : m.or.DiscountValue.Value),
                            STT = 0
                        }).ToList();

                listOrder = listOrderDetail.Join(context.Customer, or => or.CustomerId, cus => cus.CustomerId,
                    (or, cus) => new { or, cus })
                .Where(x => (customerName == "" || x.cus.CustomerName.Contains(customerName)))
                .Select(m => new ProfitAccordingCustomersModel
                {
                    CustomerId = m.or.CustomerId,
                    CustomerName = m.cus.CustomerName,
                    CustomerCode = m.cus.CustomerCode,
                    CapitalMoney = m.or.CapitalMoney,
                    GrossCapitalMoney = m.or.GrossCapitalMoney,
                    GrossProfit = m.or.GrossProfit,
                    GrossProfitMoney = m.or.GrossProfitMoney,
                    ProfitMoney = m.or.ProfitMoney,
                    STT = m.or.STT
                }).ToList();

                // lấy các phiếu nhập hàng bị trả lại
                var inventoryReceiving = context.InventoryReceivingVoucher.Where(i => i.Active == true && i.InventoryReceivingVoucherType == 2).ToList();
                listOrder = listOrder.GroupBy(x => new { x.CustomerId, x.CustomerName, x.CustomerCode }).Select(y => new ProfitAccordingCustomersModel
                {
                    CustomerId = y.Key.CustomerId,
                    CustomerName = y.Key.CustomerName,
                    CustomerCode = y.Key.CustomerCode,
                    CapitalMoney = y.Sum(s => s.CapitalMoney),
                    GrossCapitalMoney = 0,
                    GrossProfit = 0,
                    GrossProfitMoney = 0,
                    ProfitMoney = y.Sum(s => s.ProfitMoney),
                    STT = 0
                }).ToList();

                int index = 0;

                decimal SumProfitMoney = 0;
                decimal SumCapitalMoney = 0;
                decimal SumGrossProfit = 0;
                decimal SumGrossCapitalMoney = 0;
                decimal SumGrossProfitMoney = 0;

                listOrder.ForEach((item) =>
                {
                    index++;
                    item.STT = index;
                    item.GrossProfit = item.ProfitMoney - item.CapitalMoney;
                    item.GrossProfitMoney = item.ProfitMoney == 0 ? 100 : Math.Round((item.GrossProfit / item.ProfitMoney), 2);
                    item.GrossCapitalMoney = item.CapitalMoney == 0 ? 100 : Math.Round((item.GrossProfit / item.CapitalMoney), 2);
                    SumProfitMoney = SumProfitMoney + item.ProfitMoney;
                    SumCapitalMoney = SumCapitalMoney + item.CapitalMoney;
                    SumGrossProfit = SumGrossProfit + item.GrossProfit;
                    SumGrossCapitalMoney = SumGrossCapitalMoney + item.GrossCapitalMoney;
                    SumGrossProfitMoney = SumGrossProfitMoney + item.GrossProfitMoney;
                });

                return new ProfitAccordingCustomersResult
                {
                    MessageCode = "Success",
                    ListProfitAccordingCustomers = listOrder,
                    StatusCode = HttpStatusCode.OK,
                    SumCapitalMoney = SumCapitalMoney,
                    SumGrossProfit = SumGrossProfit,
                    SumGrossCapitalMoney = SumGrossCapitalMoney,
                    SumGrossProfitMoney = SumGrossProfitMoney,
                    SumProfitMoney = SumProfitMoney
                };
            }
            catch (Exception e)
            {
                return new ProfitAccordingCustomersResult
                {
                    MessageCode = e.Message,
                    StatusCode = HttpStatusCode.ExpectationFailed
                };
            }
        }

        private List<Guid?> getOrganizationChildrenId(Guid? id, List<Guid?> list)
        {
            var Organization = context.Organization.Where(o => o.ParentId == id).ToList();
            Organization.ForEach(item =>
            {
                list.Add(item.OrganizationId);
                getOrganizationChildrenId(item.OrganizationId, list);
            });

            return list;
        }

        public GetCustomerOrderByIDResult GetCustomerOrderByID(GetCustomerOrderByIDParameter parameter)
        {
            try
            {
                #region By Hung Edit code

                var customerOrderObject = (from or in context.CustomerOrder
                                           where or.OrderId == parameter.CustomerOrderId
                                           select new CustomerOrderEntityModel
                                           {
                                               OrderId = or.OrderId,
                                               BankAccountId = or.BankAccountId,
                                               OrderCode = or.OrderCode,
                                               OrderDate = or.OrderDate,
                                               Description = or.Description,
                                               Note = or.Note,
                                               CustomerId = or.CustomerId.Value,
                                               PaymentMethod = or.PaymentMethod,
                                               DaysAreOwed = or.DaysAreOwed,
                                               MaxDebt = or.MaxDebt,
                                               ReceivedDate = or.ReceivedDate,
                                               ReceivedHour = or.ReceivedHour,
                                               RecipientName = or.RecipientName,
                                               LocationOfShipment = or.LocationOfShipment,
                                               ShippingNote = or.ShippingNote,
                                               RecipientPhone = or.RecipientPhone,
                                               RecipientEmail = or.RecipientEmail,
                                               PlaceOfDelivery = or.PlaceOfDelivery,
                                               ReceiptInvoiceAmount = or.ReceiptInvoiceAmount,
                                               Amount = or.Amount.Value,
                                               Seller = or.Seller,
                                               CustomerContactId = or.CustomerContactId,
                                               DiscountValue = or.DiscountValue,
                                               StatusId = or.StatusId,
                                               CreatedById = or.CreatedById,
                                               CreatedDate = or.CreatedDate,
                                               UpdatedById = or.UpdatedById,
                                               UpdatedDate = or.UpdatedDate,
                                               Active = or.Active,
                                               DiscountType = or.DiscountType,
                                               SellerAvatarUrl = "",
                                               SellerFirstName = "",
                                               SellerLastName = "",
                                               ReasonCancel = or.ReasonCancel,
                                               CustomerAddress = or.CustomerAddress
                                           }).FirstOrDefault();

                var contact = context.Contact.Where(e => e.ObjectId == customerOrderObject.Seller).FirstOrDefault();
                if (contact != null)
                {
                    customerOrderObject.SellerAvatarUrl = contact.AvatarUrl;
                    customerOrderObject.SellerFirstName = contact.FirstName;
                    customerOrderObject.SellerLastName = contact.LastName;
                }

                #endregion

                var statusList = context.OrderStatus.ToList();
                customerOrderObject.StatusCode =
                    statusList.FirstOrDefault(x => x.OrderStatusId == customerOrderObject.StatusId) != null
                        ? statusList.FirstOrDefault(x => x.OrderStatusId == customerOrderObject.StatusId)
                            .OrderStatusCode
                        : "";

                if (customerOrderObject.CustomerId == getIDKHL001())
                {
                    customerOrderObject.TypeAccount = 1;
                }
                else
                {
                    customerOrderObject.TypeAccount = 2;
                }

                #region By Hung Edit Code

                var listCustomerOrderObjectType0 = (from cod in context.CustomerOrderDetail
                                                    where cod.OrderId == parameter.CustomerOrderId && cod.OrderDetailType == 0
                                                    select (new CustomerOrderDetailEntityModel
                                                    {
                                                        Active = cod.Active,
                                                        CreatedById = cod.CreatedById,
                                                        OrderId = cod.OrderId,
                                                        VendorId = cod.VendorId,
                                                        CreatedDate = cod.CreatedDate,
                                                        CurrencyUnit = cod.CurrencyUnit,
                                                        Description = cod.Description,
                                                        DiscountType = cod.DiscountType,
                                                        DiscountValue = cod.DiscountValue,
                                                        ExchangeRate = cod.ExchangeRate,
                                                        OrderDetailId = cod.OrderDetailId,
                                                        OrderDetailType = cod.OrderDetailType,
                                                        ProductId = cod.ProductId.Value,
                                                        UpdatedById = cod.UpdatedById,
                                                        Quantity = cod.Quantity,
                                                        UnitId = cod.UnitId,
                                                        IncurredUnit = cod.IncurredUnit,
                                                        UnitPrice = cod.UnitPrice,
                                                        UpdatedDate = cod.UpdatedDate,
                                                        GuaranteeTime = cod.GuaranteeTime,
                                                        GuaranteeDatetime = cod.GuaranteeDatetime != null
                                                            ? cod.GuaranteeDatetime
                                                            : customerOrderObject.ReceivedDate.Value.AddMonths(cod.GuaranteeTime.Value),
                                                        ExpirationDate = cod.ExpirationDate,
                                                        Vat = cod.Vat,
                                                        NameVendor = "",
                                                        NameProduct = "",
                                                        NameProductUnit = "",
                                                        NameMoneyUnit = "",
                                                        SumAmount = SumAmount(cod.Quantity, cod.UnitPrice, cod.Vat, cod.DiscountValue, cod.DiscountType,
                                                            cod.ExchangeRate, cod.UnitLaborPrice, cod.UnitLaborNumber),
                                                        OrderNumber = cod.OrderNumber,
                                                        UnitLaborPrice = cod.UnitLaborPrice,
                                                        UnitLaborNumber = cod.UnitLaborNumber
                                                    })).ToList();

                if (listCustomerOrderObjectType0.Count > 0)
                {
                    List<Guid> listProductId = new List<Guid>();
                    List<Guid> listVendorId = new List<Guid>();
                    List<Guid> listCurrencyUnitId = new List<Guid>();
                    List<Guid> listUnitId = new List<Guid>();
                    listCustomerOrderObjectType0.ForEach(item =>
                    {
                        if (item.ProductId != null)
                            listProductId.Add(item.ProductId.Value);
                        if (item.VendorId != null)
                            listVendorId.Add(item.VendorId.Value);
                        if (item.CurrencyUnit != null)
                            listCurrencyUnitId.Add(item.CurrencyUnit.Value);
                        if (item.UnitId != null)
                            listUnitId.Add(item.UnitId.Value);
                    });

                    var listProduct = context.Product.Where(e => listProductId.Contains(e.ProductId)).ToList();
                    var listVendor = context.Vendor.Where(e => listVendorId.Contains(e.VendorId)).ToList();
                    var listCurrencyUnit =
                        context.Category.Where(e => listCurrencyUnitId.Contains(e.CategoryId)).ToList();
                    var listUnit = context.Category.Where(e => listUnitId.Contains(e.CategoryId)).ToList();
                    listCustomerOrderObjectType0.ForEach(item =>
                    {
                        if (item.VendorId != null)
                            item.NameVendor = listVendor.FirstOrDefault(e => e.VendorId == item.VendorId).VendorName;
                        if (item.ProductId != null)
                        {
                            item.NameProduct = listProduct.FirstOrDefault(e => e.ProductId == item.ProductId)
                                .ProductName;
                            item.ProductCode = listProduct.FirstOrDefault(e => e.ProductId == item.ProductId)
                                .ProductCode;
                        }

                        if (item.CurrencyUnit != null)
                            item.NameProductUnit =
                                listUnit.FirstOrDefault(e => e.CategoryId == item.UnitId).CategoryName;
                        if (item.UnitId != null)
                            item.NameMoneyUnit = listCurrencyUnit.FirstOrDefault(e => e.CategoryId == item.CurrencyUnit)
                                .CategoryName;
                    });
                }

                #endregion

                #region By Hung Edit Code

                var listCustomerOrderObjectType1 = (from cod in context.CustomerOrderDetail
                                                    where cod.OrderId == parameter.CustomerOrderId && cod.OrderDetailType == 1
                                                    select (new CustomerOrderDetailEntityModel
                                                    {
                                                        Active = cod.Active,
                                                        CreatedById = cod.CreatedById,
                                                        OrderId = cod.OrderId,
                                                        VendorId = cod.VendorId,
                                                        CreatedDate = cod.CreatedDate,
                                                        CurrencyUnit = cod.CurrencyUnit,
                                                        Description = cod.Description,
                                                        DiscountType = cod.DiscountType,
                                                        DiscountValue = cod.DiscountValue,
                                                        ExchangeRate = cod.ExchangeRate,
                                                        OrderDetailId = cod.OrderDetailId,
                                                        OrderDetailType = cod.OrderDetailType,
                                                        ProductId = cod.ProductId.Value,
                                                        UpdatedById = cod.UpdatedById,
                                                        Quantity = cod.Quantity,
                                                        UnitId = cod.UnitId,
                                                        IncurredUnit = cod.IncurredUnit,
                                                        UnitPrice = cod.UnitPrice,
                                                        UpdatedDate = cod.UpdatedDate,
                                                        GuaranteeTime = cod.GuaranteeTime,
                                                        GuaranteeDatetime = cod.GuaranteeDatetime != null
                                                            ? cod.GuaranteeDatetime
                                                            : customerOrderObject.ReceivedDate.Value.AddMonths(cod.GuaranteeTime.Value),
                                                        ExpirationDate = cod.ExpirationDate,
                                                        Vat = cod.Vat,
                                                        NameVendor = "",
                                                        NameProduct = "",
                                                        NameProductUnit = "",
                                                        NameMoneyUnit = "",
                                                        SumAmount = SumAmount(cod.Quantity, cod.UnitPrice, cod.Vat, cod.DiscountValue, cod.DiscountType,
                                                            cod.ExchangeRate, 0, 0),
                                                        OrderNumber = cod.OrderNumber,
                                                        UnitLaborPrice = cod.UnitLaborPrice,
                                                        UnitLaborNumber = cod.UnitLaborNumber
                                                    })).ToList();

                if (listCustomerOrderObjectType1.Count > 0)
                {
                    List<Guid> listCurrencyUnitId = new List<Guid>();

                    listCustomerOrderObjectType1.ForEach(item =>
                    {
                        if (item.CurrencyUnit != null)
                            listCurrencyUnitId.Add(item.CurrencyUnit.Value);
                    });

                    var listCurrencyUnit = context.Category.Where(e => listCurrencyUnitId.Contains(e.CategoryId)).ToList();
                    listCustomerOrderObjectType1.ForEach(item =>
                    {
                        if (item.CurrencyUnit != null)
                            item.NameMoneyUnit = listCurrencyUnit.FirstOrDefault(e => e.CategoryId == item.CurrencyUnit)
                                .CategoryName;
                    });
                }

                #endregion

                listCustomerOrderObjectType0.ForEach(item =>
                {
                    item.NameGene = item.NameProduct + "(" + getNameGEn(item.OrderDetailId.Value) + ")";
                    item.OrderProductDetailProductAttributeValue =
                        getListOrderProductDetailProductAttributeValue(item.OrderDetailId.Value);
                });

                listCustomerOrderObjectType0.AddRange(listCustomerOrderObjectType1);

                listCustomerOrderObjectType0 = listCustomerOrderObjectType0.OrderBy(z => z.OrderNumber).ToList();

                #region Lấy list ghi chú

                var listNote = new List<NoteEntityModel>();

                listNote = context.Note.Where(x => x.ObjectId == parameter.CustomerOrderId && x.ObjectType == "ORDER" && x.Active == true).Select(y => new NoteEntityModel
                {
                    NoteId = y.NoteId,
                    Description = y.Description,
                    Type = y.Type,
                    ObjectId = y.ObjectId,
                    ObjectType = y.ObjectType,
                    NoteTitle = y.NoteTitle,
                    Active = y.Active,
                    CreatedById = y.CreatedById,
                    CreatedDate = y.CreatedDate,
                    UpdatedById = y.UpdatedById,
                    UpdatedDate = y.UpdatedDate,
                    ResponsibleName = "",
                    ResponsibleAvatar = "",
                    NoteDocList = new List<NoteDocumentEntityModel>()
                }).ToList();

                if (listNote.Count > 0)
                {
                    var listNoteId = listNote.Select(x => x.NoteId).ToList();
                    var listUser = context.User.ToList();
                    var listEmployee = context.Employee.ToList();
                    var listNoteDocument = context.NoteDocument.Where(x => listNoteId.Contains(x.NoteId)).Select(
                        y => new NoteDocumentEntityModel
                        {
                            DocumentName = y.DocumentName,
                            DocumentSize = y.DocumentSize,
                            DocumentUrl = y.DocumentUrl,
                            CreatedById = y.CreatedById,
                            NoteDocumentId = y.NoteDocumentId,
                            NoteId = y.NoteId
                        }).ToList();
                    listNote.ForEach(item =>
                    {
                        var user = listUser.FirstOrDefault(x => x.UserId == item.CreatedById);
                        var employee = listEmployee.FirstOrDefault(x => x.EmployeeId == user.EmployeeId);
                        item.ResponsibleName = employee.EmployeeName;
                        item.NoteDocList = listNoteDocument.Where(x => x.NoteId == item.NoteId).ToList();
                    });

                    //Sắp xếp lại listNote
                    listNote = listNote.OrderByDescending(x => x.CreatedDate).ToList();
                }

                #endregion

                return new GetCustomerOrderByIDResult
                {
                    CustomerOrderObject = customerOrderObject,
                    ListCustomerOrderDetail = listCustomerOrderObjectType0,
                    ListNote = listNote,
                    MessageCode = "Success",
                    StatusCode = HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new GetCustomerOrderByIDResult
                {
                    MessageCode = ex.Message,
                    StatusCode = HttpStatusCode.ExpectationFailed
                };
            }
        }

        private Guid getIDKHL001()
        {
            Guid result = Guid.Empty;
            var AccountKHL = context.Customer.Where(item => item.CustomerCode == "KHL001").FirstOrDefault();
            if (AccountKHL != null)
                result = AccountKHL.CustomerId;
            return result;
        }

        private decimal SumAmount(decimal? Quantity, decimal? UnitPrice, decimal? Vat, decimal? DiscountValue, bool? DiscountType, decimal? ExChangeRate, decimal unitLaborPrice, int unitLaborNumber)
        {
            decimal result = 0;
            decimal CaculateVAT = 0;
            decimal CacuDiscount = 0;
            decimal SumAmount = Quantity.Value * UnitPrice.Value * ExChangeRate.Value;
            decimal calculateUnitLabor = unitLaborNumber * unitLaborPrice * (ExChangeRate ?? 1);


            if (DiscountValue != null)
            {
                if (DiscountType == true)
                {
                    CacuDiscount = (((SumAmount + calculateUnitLabor) * DiscountValue.Value) / 100);
                }
                else
                {
                    CacuDiscount = DiscountValue.Value;
                }
            }

            if (Vat != null)
            {
                CaculateVAT = ((SumAmount + calculateUnitLabor - CacuDiscount) * Vat.Value) / 100;
            }

            result = SumAmount + calculateUnitLabor + CaculateVAT - CacuDiscount;

            return result;
        }

        public string getNameGEn(Guid CustomerDetailID)
        {
            string Result = string.Empty;
            List<OrderProductDetailProductAttributeValueEntityModel> listResult = new List<OrderProductDetailProductAttributeValueEntityModel>();
            var OrderProductDetailProductAttributeValueModelList = (from OPDPV in context.OrderProductDetailProductAttributeValue
                                                                    join ProductAttributeCategoryV in context.ProductAttributeCategoryValue on OPDPV.ProductAttributeCategoryValueId equals ProductAttributeCategoryV.ProductAttributeCategoryValueId
                                                                    where OPDPV.OrderDetailId == CustomerDetailID
                                                                    select (ProductAttributeCategoryV)).ToList();

            OrderProductDetailProductAttributeValueModelList.ForEach(item => { Result = Result + item.ProductAttributeCategoryValue1 + ";"; });

            return Result;

        }

        public List<OrderProductDetailProductAttributeValueEntityModel> getListOrderProductDetailProductAttributeValue(Guid CustomerDetailID)
        {
            List<OrderProductDetailProductAttributeValueEntityModel> listResult = new List<OrderProductDetailProductAttributeValueEntityModel>();
            var OrderProductDetailProductAttributeValueModelList = (from OPDPV in context.OrderProductDetailProductAttributeValue
                                                                    join ProductAttributeC in context.ProductAttributeCategory on OPDPV.ProductAttributeCategoryId equals ProductAttributeC.ProductAttributeCategoryId
                                                                    join ProductAttributeCategoryV in context.ProductAttributeCategoryValue on OPDPV.ProductAttributeCategoryValueId equals ProductAttributeCategoryV.ProductAttributeCategoryValueId
                                                                    where OPDPV.OrderDetailId == CustomerDetailID
                                                                    select (new OrderProductDetailProductAttributeValueEntityModel
                                                                    {
                                                                        OrderDetailId = OPDPV.OrderDetailId,
                                                                        OrderProductDetailProductAttributeValueId = OPDPV.OrderProductDetailProductAttributeValueId,
                                                                        ProductAttributeCategoryId = OPDPV.ProductAttributeCategoryId,
                                                                        ProductId = OPDPV.ProductId,
                                                                        ProductAttributeCategoryValueId = OPDPV.ProductAttributeCategoryValueId,
                                                                        NameProductAttributeCategory = ProductAttributeC.ProductAttributeCategoryName,
                                                                        NameProductAttributeCategoryValue = ProductAttributeCategoryV.ProductAttributeCategoryValue1
                                                                    })).ToList();
            listResult = OrderProductDetailProductAttributeValueModelList;
            return listResult;
        }

        private decimal GetDiscountValue(decimal quantity, decimal unitPrice, decimal exchangeRate, bool discountType, decimal discountValue)
        {
            if (discountType == true)
            {
                //Nếu chiết khấu là %
                var amount = quantity * unitPrice * exchangeRate;
                var newDiscountValue = Math.Round(((amount * discountValue) / 100), 0);
                return newDiscountValue; //.ToString("#,#.");
            }
            else
            {
                //Nếu chiết khấu là tiền mặt
                return discountValue; //.ToString("#,#.");
            }
        }

        public ExportCustomerOrderPDFResult ExportPdfCustomerOrder(ExportCustomerOrderPDFParameter parameter)
        {
            try
            {
                GetCustomerOrderByIDResult resultCustomerOrderId = new GetCustomerOrderByIDResult();
                GetCustomerOrderByIDParameter customerOrderByIDParameter = new GetCustomerOrderByIDParameter();
                customerOrderByIDParameter.CustomerOrderId = parameter.CustomerOrderId;
                customerOrderByIDParameter.UserId = parameter.UserId;
                resultCustomerOrderId = this.GetCustomerOrderByID(customerOrderByIDParameter);
                var customerOrder = resultCustomerOrderId.CustomerOrderObject;
                var companyConfig = context.CompanyConfiguration.FirstOrDefault();
                var customer = context.Customer.Where(item => item.CustomerId == customerOrder.CustomerId).FirstOrDefault();
                Guid contactID = context.Contact
                    .Where(c => c.ObjectId == customer.CustomerId && c.ObjectType != ObjectType.CUSTOMERCONTACT)
                    .FirstOrDefault().ContactId;
                var contact = context.Contact.FirstOrDefault(c => c.ContactId == contactID);
                var PaymentMethodObj =
                    context.Category.FirstOrDefault(item => item.CategoryId == customerOrder.PaymentMethod);

                if (customerOrder != null)
                {
                    #region add by Giang

                    PDFOrderModel PDFOrder = new PDFOrderModel();
                    PDFOrder.OrderDate = "Ngày " + customerOrder.OrderDate.Value.Day + " tháng " +
                                         customerOrder.OrderDate.Value.Month + " năm " + customerOrder.OrderDate.Value.Year;
                    PDFOrder.CompanyName = companyConfig.CompanyName == null ? "" : companyConfig.CompanyName;
                    PDFOrder.Website = "";
                    PDFOrder.TaxCode = companyConfig.TaxCode == null ? "" : companyConfig.TaxCode;
                    PDFOrder.CompanyPhone = companyConfig.Phone == null ? "" : companyConfig.Phone;
                    PDFOrder.CompanyEmail = companyConfig.Email == null ? "" : companyConfig.Email;
                    PDFOrder.CompanyAddress = companyConfig.CompanyAddress == null ? "" : companyConfig.CompanyAddress;
                    PDFOrder.OrderCode = customerOrder.OrderCode;
                    PDFOrder.Seller = (customerOrder.SellerFirstName == null ? "" : customerOrder.SellerFirstName) + ' ' +
                                      (customerOrder.SellerLastName == null ? "" : customerOrder.SellerLastName);
                    PDFOrder.LocationOfShipment =
                        customerOrder.LocationOfShipment == null ? "" : customerOrder.LocationOfShipment;
                    PDFOrder.Description = customerOrder.Description == null ? "" : customerOrder.Description;
                    PDFOrder.CustomerName = customer.CustomerName == null ? "" : customer.CustomerName;
                    PDFOrder.CustomerCode = customer.CustomerCode == null ? "" : customer.CustomerCode;
                    PDFOrder.CustomerPhone = contact.Phone == null ? "" : contact.Phone;
                    PDFOrder.CustomerAddress = contact.Address == null ? "" : contact.Address;
                    PDFOrder.CustomerTaxCode = contact.TaxCode == null ? "" : contact.TaxCode;
                    if (PaymentMethodObj != null)
                    {
                        PDFOrder.CustomerPaymentMethod =
                            PaymentMethodObj.CategoryName == null ? "" : PaymentMethodObj.CategoryName;
                    }
                    else
                    {
                        PDFOrder.CustomerPaymentMethod = "";
                    }
                    PDFOrder.RecipientName = customerOrder.RecipientName == null ? "" : customerOrder.RecipientName;
                    PDFOrder.PlaceOfDelivery = customerOrder.PlaceOfDelivery == null ? "" : customerOrder.PlaceOfDelivery;
                    string displayReceivedHour = string.Empty;
                    if (customerOrder.ReceivedHour.HasValue == false)
                    {
                        displayReceivedHour = "";
                    }
                    else
                    {
                        DateTime timeReceivedHour = DateTime.Today.Add(customerOrder.ReceivedHour.Value);
                        displayReceivedHour = timeReceivedHour.ToString("hh:mm tt");
                    }

                    PDFOrder.ReceivedDate =
                        (customerOrder.ReceivedDate.HasValue == false
                            ? ""
                            : customerOrder.ReceivedDate.Value.ToString("dd/M/yyyy")) + " - " + displayReceivedHour;
                    PDFOrder.ShippingNote = customerOrder.ShippingNote == null ? "" : customerOrder.ShippingNote;

                    var lstCustomerOrderDetail = resultCustomerOrderId.ListCustomerOrderDetail;
                    List<PDFOrderAttributeModel> lstPDFOrderAttribute = new List<PDFOrderAttributeModel>();
                    List<PDFOrderAttributeModel> lstPDFOrderAttributeOther = new List<PDFOrderAttributeModel>();
                    decimal totalAmountBeforVat = 0;
                    decimal totalAmountVat = 0;
                    decimal totalDiscountValue = 0;
                    int stt = 1;
                    int sttOther = 1;
                    for (int i = 0; i < lstCustomerOrderDetail.Count; ++i)
                    {
                        PDFOrderAttributeModel PDFOrderAttribute = new PDFOrderAttributeModel();
                        if (lstCustomerOrderDetail[i].OrderDetailType == 0)
                        {
                            var quantity = lstCustomerOrderDetail[i].Quantity == null ? 0 : lstCustomerOrderDetail[i].Quantity;
                            var unitPrice = lstCustomerOrderDetail[i].UnitPrice == null ? 0 : lstCustomerOrderDetail[i].UnitPrice;
                            var exchangeRate = lstCustomerOrderDetail[i].ExchangeRate == null ? 0 : lstCustomerOrderDetail[i].ExchangeRate;

                            var discountValue = GetDiscountValue(
                                lstCustomerOrderDetail[i].Quantity == null
                                    ? 0
                                    : lstCustomerOrderDetail[i].Quantity.Value,
                                lstCustomerOrderDetail[i].UnitPrice == null
                                    ? 0
                                    : lstCustomerOrderDetail[i].UnitPrice.Value,
                                lstCustomerOrderDetail[i].ExchangeRate == null
                                    ? 1
                                    : lstCustomerOrderDetail[i].ExchangeRate.Value,
                                lstCustomerOrderDetail[i].DiscountType == null
                                    ? false
                                    : lstCustomerOrderDetail[i].DiscountType.Value,
                                lstCustomerOrderDetail[i].DiscountValue == null
                                    ? 0
                                    : lstCustomerOrderDetail[i].DiscountValue.Value
                            );

                            var amount = (quantity * unitPrice * exchangeRate - discountValue);
                            var vatAmount = amount * lstCustomerOrderDetail[i].Vat / 100;
                            totalAmountBeforVat = totalAmountBeforVat + amount.Value;
                            totalDiscountValue = totalDiscountValue + discountValue;
                            totalAmountVat = totalAmountVat + vatAmount.Value;

                            //Không có chi phí phát sinh
                            PDFOrderAttribute.Stt = (stt++).ToString("#,#.");
                            PDFOrderAttribute.ProductName = lstCustomerOrderDetail[i].NameProduct == null
                                ? ""
                                : lstCustomerOrderDetail[i].NameProduct;
                            PDFOrderAttribute.ProductCode = lstCustomerOrderDetail[i].ProductCode == null
                                ? ""
                                : lstCustomerOrderDetail[i].ProductCode;
                            PDFOrderAttribute.UnitName = lstCustomerOrderDetail[i].NameProductUnit == null
                                ? ""
                                : lstCustomerOrderDetail[i].NameProductUnit;
                            PDFOrderAttribute.Quantity = lstCustomerOrderDetail[i].Quantity == null
                                ? ""
                                : lstCustomerOrderDetail[i].Quantity.Value.ToString("#,#.");
                            PDFOrderAttribute.UnitPrice = lstCustomerOrderDetail[i].UnitPrice == null
                                ? ""
                                : lstCustomerOrderDetail[i].UnitPrice.Value.ToString("#,#.");
                            PDFOrderAttribute.ExchangeRate = lstCustomerOrderDetail[i].ExchangeRate.HasValue == false
                                ? ""
                                : (lstCustomerOrderDetail[i].ExchangeRate == 1
                                    ? ""
                                    : lstCustomerOrderDetail[i].ExchangeRate.Value.ToString("#,#."));
                            PDFOrderAttribute.VAT = lstCustomerOrderDetail[i].Vat.HasValue == false
                                ? "0%"
                                : (lstCustomerOrderDetail[i].Vat == null || lstCustomerOrderDetail[i].Vat == 0
                                    ? "0%"
                                    : lstCustomerOrderDetail[i].Vat.Value.ToString("#,#.") + "%");
                            PDFOrderAttribute.DiscountValue = GetDiscountValue(
                                lstCustomerOrderDetail[i].Quantity == null
                                    ? 0
                                    : lstCustomerOrderDetail[i].Quantity.Value,
                                lstCustomerOrderDetail[i].UnitPrice == null
                                    ? 0
                                    : lstCustomerOrderDetail[i].UnitPrice.Value,
                                lstCustomerOrderDetail[i].ExchangeRate == null
                                    ? 1
                                    : lstCustomerOrderDetail[i].ExchangeRate.Value,
                                lstCustomerOrderDetail[i].DiscountType == null
                                    ? false
                                    : lstCustomerOrderDetail[i].DiscountType.Value,
                                lstCustomerOrderDetail[i].DiscountValue == null
                                    ? 0
                                    : lstCustomerOrderDetail[i].DiscountValue.Value
                            ).ToString("#,#.");
                            //PDFOrderAttribute.Amount = lstCustomerOrderDetail[i].SumAmount.ToString("#,#.");
                            PDFOrderAttribute.Amount = amount.Value.ToString("#,#.");
                            lstPDFOrderAttribute.Add(PDFOrderAttribute);
                        }
                        else
                        {
                            var quantity = lstCustomerOrderDetail[i].Quantity == null ? 0 : lstCustomerOrderDetail[i].Quantity;
                            var unitPrice = lstCustomerOrderDetail[i].UnitPrice == null ? 0 : lstCustomerOrderDetail[i].UnitPrice;
                            var exchangeRate = lstCustomerOrderDetail[i].ExchangeRate == null ? 0 : lstCustomerOrderDetail[i].ExchangeRate;

                            var discountValue = GetDiscountValue(
                                lstCustomerOrderDetail[i].Quantity == null
                                    ? 0
                                    : lstCustomerOrderDetail[i].Quantity.Value,
                                lstCustomerOrderDetail[i].UnitPrice == null
                                    ? 0
                                    : lstCustomerOrderDetail[i].UnitPrice.Value,
                                lstCustomerOrderDetail[i].ExchangeRate == null
                                    ? 1
                                    : lstCustomerOrderDetail[i].ExchangeRate.Value,
                                lstCustomerOrderDetail[i].DiscountType == null
                                    ? false
                                    : lstCustomerOrderDetail[i].DiscountType.Value,
                                lstCustomerOrderDetail[i].DiscountValue == null
                                    ? 0
                                    : lstCustomerOrderDetail[i].DiscountValue.Value
                            );

                            var amount = (quantity * unitPrice * exchangeRate - discountValue);
                            var vatAmount = amount * lstCustomerOrderDetail[i].Vat / 100;
                            totalAmountBeforVat = totalAmountBeforVat + amount.Value;
                            totalDiscountValue = totalDiscountValue + discountValue;
                            totalAmountVat = totalAmountVat + vatAmount.Value;

                            //Có chi phí phát sinh
                            PDFOrderAttribute.Stt = (sttOther++).ToString("#,#.");
                            PDFOrderAttribute.ProductName = lstCustomerOrderDetail[i].NameProduct == null
                                ? ""
                                : lstCustomerOrderDetail[i].NameProduct;
                            PDFOrderAttribute.ProductCode = lstCustomerOrderDetail[i].ProductCode == null
                                ? ""
                                : lstCustomerOrderDetail[i].ProductCode;
                            PDFOrderAttribute.UnitName = lstCustomerOrderDetail[i].NameProductUnit == null
                                ? ""
                                : lstCustomerOrderDetail[i].NameProductUnit;
                            PDFOrderAttribute.Quantity = lstCustomerOrderDetail[i].Quantity == null
                                ? ""
                                : lstCustomerOrderDetail[i].Quantity.Value.ToString("#,#.");
                            PDFOrderAttribute.UnitPrice = lstCustomerOrderDetail[i].UnitPrice == null
                                ? ""
                                : lstCustomerOrderDetail[i].UnitPrice.Value.ToString("#,#.");
                            PDFOrderAttribute.ExchangeRate = lstCustomerOrderDetail[i].ExchangeRate.HasValue == false
                                ? ""
                                : (lstCustomerOrderDetail[i].ExchangeRate == 1
                                    ? ""
                                    : lstCustomerOrderDetail[i].ExchangeRate.Value.ToString("#,#."));
                            PDFOrderAttribute.VAT = lstCustomerOrderDetail[i].Vat.HasValue == false
                                ? "0%"
                                : (lstCustomerOrderDetail[i].Vat == null || lstCustomerOrderDetail[i].Vat == 0
                                    ? "0%"
                                    : lstCustomerOrderDetail[i].Vat.Value.ToString("#,#.") + "%");
                            PDFOrderAttribute.DiscountValue = GetDiscountValue(
                                lstCustomerOrderDetail[i].Quantity == null
                                    ? 0
                                    : lstCustomerOrderDetail[i].Quantity.Value,
                                lstCustomerOrderDetail[i].UnitPrice == null
                                    ? 0
                                    : lstCustomerOrderDetail[i].UnitPrice.Value,
                                lstCustomerOrderDetail[i].ExchangeRate == null
                                    ? 1
                                    : lstCustomerOrderDetail[i].ExchangeRate.Value,
                                lstCustomerOrderDetail[i].DiscountType == null
                                    ? false
                                    : lstCustomerOrderDetail[i].DiscountType.Value,
                                lstCustomerOrderDetail[i].DiscountValue == null
                                    ? 0
                                    : lstCustomerOrderDetail[i].DiscountValue.Value
                            ).ToString("#,#.");
                            //PDFOrderAttribute.Amount = lstCustomerOrderDetail[i].SumAmount.ToString("#,#.");
                            PDFOrderAttribute.Amount = amount.Value.ToString("#,#.");
                            lstPDFOrderAttributeOther.Add(PDFOrderAttribute);
                        }
                    }

                    PDFOrder.ListPDFOrderAttribute = lstPDFOrderAttribute;
                    PDFOrder.ListPDFOrderAttributeOther = lstPDFOrderAttributeOther;
                    PDFOrder.TotalVat = totalAmountVat.ToString("#,#.");
                    PDFOrder.TotalBeforVat = totalAmountBeforVat.ToString("#,#.");
                    PDFOrder.TotalDiscountValue = totalDiscountValue.ToString("#,#.");

                    PDFOrder.TotalAmount = customerOrder.Amount.Value.ToString("#,#.");
                    if (customerOrder.DiscountType == true)
                    {
                        //Nếu chiết khấu là %
                        var DiscountValueOrder =
                            Math.Round(
                                (customerOrder.Amount.Value * (customerOrder.DiscountValue == null
                                     ? 0
                                     : customerOrder.DiscountValue.Value)) / 100, 0);
                        PDFOrder.DiscountValue = DiscountValueOrder.ToString("#,#.");
                        var TotalAmountAfter = (customerOrder.Amount - DiscountValueOrder);
                        PDFOrder.TotalAmountAfter = TotalAmountAfter.Value.ToString("#,#.");
                        PDFOrder.TotalAmountAfterText = MoneyHelper.Convert(TotalAmountAfter.Value);
                    }
                    else
                    {
                        //Nếu chiết khấu là tiền mặt
                        PDFOrder.DiscountValue = customerOrder.DiscountValue == null
                            ? "0"
                            : customerOrder.DiscountValue.Value.ToString("#,#.");
                        var TotalAmountAfter = (customerOrder.Amount.Value -
                                                (customerOrder.DiscountValue == null
                                                    ? 0
                                                    : customerOrder.DiscountValue.Value));
                        PDFOrder.TotalAmountAfter = TotalAmountAfter.ToString("#,#.");
                        PDFOrder.TotalAmountAfterText = MoneyHelper.Convert(TotalAmountAfter);
                    }

                    #endregion

                    return new ExportCustomerOrderPDFResult
                    {
                        StatusCode = HttpStatusCode.OK,
                        MessageCode = "Success",
                        Code = customerOrder.OrderCode,
                        PDFOrder = PDFOrder
                    };
                }

                return new ExportCustomerOrderPDFResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = "Không tồn tại đơn hàng"
                };
            }
            catch (Exception e)
            {
                return new ExportCustomerOrderPDFResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public UpdateCustomerOrderResult UpdateCustomerOrder(UpdateCustomerOrderParameter parameter)
        {
            var vendorOrder = new VendorOrder();
            try
            {
                bool isValidParameterNumber = true;

                if (parameter.CustomerOrder?.DaysAreOwed < 0 || parameter.CustomerOrder?.MaxDebt < 0 ||
                    parameter.CustomerOrder?.DiscountValue < 0)
                {
                    isValidParameterNumber = false;
                }

                parameter.CustomerOrderDetail.ForEach(item =>
                {
                    if (item?.Quantity <= 0 || item?.ExchangeRate <= 0 || item?.UnitPrice < 0 ||
                        item?.GuaranteeTime < 0 || item?.Vat < 0 || item?.DiscountValue < 0)
                    {
                        isValidParameterNumber = false;
                    }
                });

                if (!isValidParameterNumber)
                {
                    return new UpdateCustomerOrderResult
                    {
                        MessageCode = CommonMessage.Order.EDIT_ORDER_FAIL,
                        StatusCode = HttpStatusCode.ExpectationFailed
                    };
                }

                var oldOrder =
                    context.CustomerOrder.FirstOrDefault(co => co.OrderId == parameter.CustomerOrder.OrderId);

                if (oldOrder == null)
                {
                    return new UpdateCustomerOrderResult()
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = CommonMessage.Order.ORDER_NO_EXIST
                    };
                }

                #region Kiểm tra đơn hàng có thay đổi khách hàng hay không

                if (oldOrder.CustomerId != parameter.CustomerOrder.CustomerId)
                {
                    //Nếu đơn hàng thay đổi khách hàng thì kiểm tra xem đơn hàng đã phát sinh thanh toán chưa?
                    var order_reciept = context.ReceiptOrderHistory.FirstOrDefault(x => x.OrderId == oldOrder.OrderId);
                    if (order_reciept != null)
                    {
                        //Nếu đơn hàng đã phát sinh thanh toán thì không cho thay đổi khách hàng
                        return new UpdateCustomerOrderResult()
                        {
                            StatusCode = HttpStatusCode.ExpectationFailed,
                            MessageCode = CommonMessage.Order.ORDER_NO_PAY
                        };
                    }
                }

                #endregion

                #region Kiểm tra trạng thái đơn hàng có thỏa mãn điều kiện cập nhật đơn hàng hay không

                var oldStatus = context.OrderStatus.FirstOrDefault(x => x.OrderStatusId == oldOrder.StatusId);
                var oldStatusCode = oldStatus?.OrderStatusCode;

                var newStatus =
                    context.OrderStatus.FirstOrDefault(x => x.OrderStatusId == parameter.CustomerOrder.StatusId);
                var newStatusCode = newStatus?.OrderStatusCode;

                switch (oldStatusCode)
                {
                    case "DRA":
                        if (newStatusCode != "DRA" && newStatusCode != "IP")
                        {
                            return new UpdateCustomerOrderResult()
                            {
                                StatusCode = HttpStatusCode.ExpectationFailed,
                                MessageCode = CommonMessage.Order.ORDER_NO_CHANGE_STATUS + newStatus.Description
                            };
                        }
                        break;
                    case "IP":
                        if (newStatusCode != "IP" && newStatusCode != "PD" && newStatusCode != "DLV" &&
                            newStatusCode != "COMP" && newStatusCode != "ON" && newStatusCode != "RTN" &&
                            newStatusCode != "CAN")
                        {
                            return new UpdateCustomerOrderResult()
                            {
                                StatusCode = HttpStatusCode.ExpectationFailed,
                                MessageCode = CommonMessage.Order.ORDER_NO_CHANGE_STATUS + newStatus.Description
                            };
                        }
                        break;
                    case "ON":
                        if (newStatusCode != "ON" && newStatusCode != "IP" && newStatusCode != "PD" &&
                            newStatusCode != "DLV" && newStatusCode != "COMP" && newStatusCode != "RTN" &&
                            newStatusCode != "CAN")
                        {
                            return new UpdateCustomerOrderResult()
                            {
                                StatusCode = HttpStatusCode.ExpectationFailed,
                                MessageCode = CommonMessage.Order.ORDER_NO_CHANGE_STATUS + newStatus.Description
                            };
                        }
                        break;
                    case "DLV":
                        if (newStatusCode != "DLV" && newStatusCode != "RTN" && newStatusCode != "PD" &&
                            newStatusCode != "CAN" && newStatusCode != "COMP")
                        {
                            return new UpdateCustomerOrderResult()
                            {
                                StatusCode = HttpStatusCode.ExpectationFailed,
                                MessageCode = CommonMessage.Order.ORDER_NO_CHANGE_STATUS + newStatus.Description
                            };
                        }
                        break;
                    case "PD":
                        if (newStatusCode != "PD" && newStatusCode != "DLV" && newStatusCode != "COMP" &&
                            newStatusCode != "CAN")
                        {
                            return new UpdateCustomerOrderResult()
                            {
                                StatusCode = HttpStatusCode.ExpectationFailed,
                                MessageCode = CommonMessage.Order.ORDER_NO_CHANGE_STATUS + newStatus.Description
                            };
                        }
                        break;
                    case "COMP":
                        return new UpdateCustomerOrderResult()
                        {
                            StatusCode = HttpStatusCode.ExpectationFailed,
                            MessageCode = CommonMessage.Order.ORDER_NO_CHANGE
                        };
                    case "CAN":
                        return new UpdateCustomerOrderResult()
                        {
                            StatusCode = HttpStatusCode.ExpectationFailed,
                            MessageCode = CommonMessage.Order.ORDER_NO_CHANGE
                        };
                    case "RTN":
                        return new UpdateCustomerOrderResult()
                        {
                            StatusCode = HttpStatusCode.ExpectationFailed,
                            MessageCode = CommonMessage.Order.ORDER_NO_CHANGE
                        };
                }

                #endregion

                using (var transaction = context.Database.BeginTransaction())
                {
                    #region Delete all item Relation

                    var List_Delete_OrderProductDetailProductAttributeValue =
                        new List<OrderProductDetailProductAttributeValue>();
                    var List_Delete_CustomerOrderDetail = new List<CustomerOrderDetail>();
                    var List_Delete_OrderCostDetail = new List<OrderCostDetail>();

                    List_Delete_CustomerOrderDetail = context.CustomerOrderDetail
                        .Where(item => item.OrderId == parameter.CustomerOrder.OrderId).ToList();

                    List_Delete_CustomerOrderDetail.ForEach(item =>
                    {
                        if (item.OrderDetailId != Guid.Empty)
                        {
                            var OrderProductDetailProductAttributeValueList = context
                                .OrderProductDetailProductAttributeValue
                                .Where(OPDPAV => OPDPAV.OrderDetailId == item.OrderDetailId).ToList();
                            List_Delete_OrderProductDetailProductAttributeValue.AddRange(
                                OrderProductDetailProductAttributeValueList);
                        }
                    });

                    List_Delete_OrderCostDetail = context.OrderCostDetail
                        .Where(item => item.OrderId == parameter.CustomerOrder.OrderId).ToList();

                    context.OrderProductDetailProductAttributeValue.RemoveRange(
                        List_Delete_OrderProductDetailProductAttributeValue);
                    context.SaveChanges();
                    context.CustomerOrderDetail.RemoveRange(List_Delete_CustomerOrderDetail);
                    context.SaveChanges();
                    context.OrderCostDetail.RemoveRange(List_Delete_OrderCostDetail);
                    context.SaveChanges();

                    context.CustomerOrder.Remove(oldOrder);
                    context.SaveChanges();

                    #endregion

                    #region Create new Order base on Old Order

                    parameter.CustomerOrderDetail.ForEach(item =>
                    {
                        //Nếu là chi phí khác thì tên sản phẩm dịch vụ sẽ bằng trường Description
                        if (item.OrderDetailType == 1)
                        {
                            item.ProductName = item.Description;
                        }

                        item.OrderDetailId = Guid.NewGuid();
                        if (item.OrderProductDetailProductAttributeValue != null)
                        {
                            foreach (var itemX in item.OrderProductDetailProductAttributeValue)
                            {
                                itemX.OrderProductDetailProductAttributeValueId = Guid.NewGuid();
                            }
                        }
                    });

                    parameter.OrderCostDetail.ForEach(item =>
                    {
                        item.OrderCostDetailId = Guid.NewGuid();
                        item.CreatedById = parameter.UserId;
                        item.CreatedDate = DateTime.Now;
                        item.UpdatedById = parameter.UserId;
                        item.UpdatedDate = DateTime.Now;
                    });

                    #region Kiểm tra có phải là khách lẻ không?

                    if (parameter.TypeAccount == 1)
                    {
                        var customer = context.Customer.FirstOrDefault(x => x.CustomerCode == "KHL001");

                        parameter.CustomerOrder.CustomerId = customer.CustomerId;
                        parameter.CustomerOrder.PaymentMethod = null;
                        parameter.CustomerOrder.DaysAreOwed = null;
                        parameter.CustomerOrder.MaxDebt = null;
                        parameter.CustomerOrder.BankAccountId = null;
                    }

                    #endregion


                    parameter.CustomerOrder.CreatedById = oldOrder.CreatedById;
                    parameter.CustomerOrder.CreatedDate = oldOrder.CreatedDate;
                    parameter.CustomerOrder.UpdatedById = parameter.UserId;
                    parameter.CustomerOrder.UpdatedDate = DateTime.Now;
                    var customerOrder = parameter.CustomerOrder.ToEntity();
                    var listCustomerOrderDetail = new List<CustomerOrderDetail>();
                    parameter.CustomerOrderDetail.ForEach(item =>
                    {
                        var newItem = new CustomerOrderDetail();
                        newItem = item.ToEntity();

                        if (item.OrderProductDetailProductAttributeValue != null &&
                            item.OrderProductDetailProductAttributeValue.Count != 0)
                        {
                            item.OrderProductDetailProductAttributeValue.ForEach(_item =>
                            {
                                var _newItem = _item.ToEntity();
                                newItem.OrderProductDetailProductAttributeValue.Add(_newItem);
                            });
                        }

                        listCustomerOrderDetail.Add(newItem);
                    });
                    var listOrderCostDetail = new List<OrderCostDetail>();
                    parameter.OrderCostDetail.ForEach(item =>
                    {
                        listOrderCostDetail.Add(item.ToEntity());
                    });

                    context.CustomerOrder.Add(customerOrder);
                    context.SaveChanges();

                    transaction.Commit();

                    #endregion

                    #region Kiểm tra xem đơn hàng đã được sử dụng để tạo Đơn đặt hàng Nhà cung cấp hay chưa?

                    vendorOrder = context.VendorOrder.Where(item => item.CustomerOrderId == parameter.CustomerOrder.OrderId).FirstOrDefault();

                    #endregion

                }
            }
            catch (Exception ex)
            {
                return new UpdateCustomerOrderResult
                {
                    MessageCode = CommonMessage.Order.EDIT_ORDER_FAIL,
                    StatusCode = HttpStatusCode.ExpectationFailed,
                };
            }


            #region Gửi thông báo

            NotificationHelper.AccessNotification(context, TypeModel.CustomerOrderDetail, "UPD", new CustomerOrder(),
                parameter.CustomerOrder, true, empId: parameter.CustomerOrder.Seller);

            #endregion

            #region lưu nhật ký hệ thống

            LogHelper.AuditTrace(context, ActionName.UPDATE, ObjectName.CUSTOMERORDER, parameter.CustomerOrder.OrderId.Value, parameter.UserId);

            #endregion
            
            return new UpdateCustomerOrderResult
            {
                MessageCode = CommonMessage.Order.EDIT_ORDER_SUCCESS,
                CustomerOrderID = parameter.CustomerOrder.OrderId.Value,
                StatusCode = HttpStatusCode.OK,
                VendorOrderID = vendorOrder?.VendorOrderId
            };
        }
        public UpdateCustomerOrderResult UpdateStatusOrder(UpdateStatusOrderParameter parameter)
        {
            var customerOrderObj = new CustomerOrder();
            Guid? ProcurementRequestId = null;
            var StatusId = Guid.Empty;
            var message = "";

            try
            {
                var employeeId = context.User.FirstOrDefault(u => u.UserId == parameter.UserId).EmployeeId;
                var employee = context.Employee.FirstOrDefault(u => u.EmployeeId == employeeId);
                var employeeContact = context.Contact.FirstOrDefault(u => u.ObjectId == employeeId && u.ObjectType == "EMP");
                var statusOrder = context.OrderStatus.Where(s => s.Active == true).ToList();
                customerOrderObj = context.CustomerOrder.FirstOrDefault(c => c.OrderId == parameter.CustomerOrderId);
                var objDetail = context.CustomerOrderDetail.Where(d => d.OrderId == customerOrderObj.OrderId).ToList();
                var warehouseObj = context.Warehouse.FirstOrDefault(w => w.Active == true && w.WarehouseParent == null);
                StatusId = customerOrderObj.StatusId.Value;

                Note note = new Note();
                note.NoteId = Guid.NewGuid();
                note.ObjectType = "ORDER";
                note.ObjectId = parameter.CustomerOrderId;
                note.Type = "ADD";
                note.Active = true;
                note.CreatedById = parameter.UserId;
                note.CreatedDate = DateTime.Now;
                note.NoteTitle = CommonMessage.Note.NOTE_TITLE;

                
                switch (parameter.ObjectType)
                {
                    case "SEND_APPROVAL":
                        var statusIP = statusOrder.FirstOrDefault(s => s.OrderStatusCode == "IP");
                        customerOrderObj.StatusId = statusIP.OrderStatusId;
                        StatusId = statusIP.OrderStatusId;
                        customerOrderObj.UpdatedById = parameter.UserId;
                        customerOrderObj.UpdatedDate = DateTime.Now;
                        if (customerOrderObj.OrderContractId != null && customerOrderObj.OrderContractId != Guid.Empty)
                        {
                            var THDStatusType = context.CategoryType.FirstOrDefault(ct => ct.CategoryTypeCode == "THD" && ct.Active == true);
                            var DTHStatus = context.Category.FirstOrDefault(ct => ct.CategoryTypeId == THDStatusType.CategoryTypeId && ct.Active == true && ct.CategoryCode == "DTH");
                            var contractObj = context.Contract.FirstOrDefault(ctr => ctr.ContractId == customerOrderObj.OrderContractId);
                            contractObj.StatusId = DTHStatus.CategoryId;
                            context.Contract.Update(contractObj);
                            context.SaveChanges();
                        }
                        message = CommonMessage.Order.EDIT_ORDER_SUCCESS;
                        note.Description = CommonMessage.Note.NOTE_CONTENT_SEND_APPROVAL;
                        break;
                    case "APPROVAL":
                        var statusDLV = statusOrder.FirstOrDefault(s => s.OrderStatusCode == "DLV");
                        customerOrderObj.StatusId = statusDLV.OrderStatusId;
                        StatusId = statusDLV.OrderStatusId;
                        customerOrderObj.UpdatedById = parameter.UserId;
                        customerOrderObj.UpdatedDate = DateTime.Now;

                        #region Tự động sinh phiếu xuất kho

                        /*
                         * Phiếu xuất kho tự động sẽ sinh theo Mã kho theo từng sản phẩm
                         */
                        var listAllWarehouse = context.Warehouse.Where(c => c.Active == true)
                            .Select(m => new
                            {
                                m.WarehouseId,
                                m.WarehouseParent
                            }).ToList();

                        var listWarehouseParentId = new List<KhoChaCon>();

                        var lstGroupWarhouseId = objDetail.Where(c => c.WarehouseId != null && c.WarehouseId != Guid.Empty).GroupBy(c => c.WarehouseId).Select(m => m.Key).ToList();
                        var lstGroupWarehouseParentId = listAllWarehouse.Where(c => lstGroupWarhouseId.Contains(c.WarehouseId)).ToList();

                        // Đệ quy để tính ra Id, Cha - Con => cần đệ quy trong vòng while. Chưa tìm được thuật toán tối ưu hơn
                        lstGroupWarehouseParentId.ForEach(item =>
                        {
                            // Để quy để lấy parent
                            var parent = listAllWarehouse.FirstOrDefault(c => c.WarehouseId == item.WarehouseParent);
                            while (true)
                            {
                                if (parent?.WarehouseParent == null) break;
                                parent = listAllWarehouse.FirstOrDefault(c => c.WarehouseId == parent.WarehouseParent);
                            }

                            // Check xem parentId đã tồn tại trong listChaCon
                            var contain = listWarehouseParentId.FirstOrDefault(c => c.ParentId == parent?.WarehouseId);
                            if (parent == null)
                            {
                                // Chưa tồn tại khởi tạo đối tượng và add vào list
                                var khoChaCon = new KhoChaCon
                                {
                                    ParentId = item.WarehouseId,
                                    ListChildId = new List<Guid>
                                    {
                                        item.WarehouseId
                                    }
                                };
                                listWarehouseParentId.Add(khoChaCon);
                            }
                            else if (contain == null)
                            {
                                // Chưa tồn tại khởi tạo đối tượng và add vào list
                                var khoChaCon = new KhoChaCon
                                {
                                    ParentId = parent.WarehouseId,
                                    ListChildId = new List<Guid>
                                    {
                                        item.WarehouseId
                                    }
                                };
                                listWarehouseParentId.Add(khoChaCon);
                            }
                            else if (contain != null)
                            {
                                // Tộn tại xóa đối tượng, thêm Id con và add lại vào listChaCon
                                listWarehouseParentId.Remove(contain);
                                contain.ListChildId.Add(item.WarehouseId);
                                listWarehouseParentId.Add(contain);
                            }
                        });

                        // Tự động sinh Phiếu xuất kho
                        var datenow = DateTime.Now;
                        TimeSpan today = new TimeSpan(datenow.Hour, datenow.Minute, datenow.Second);
                        var totalInvertoryCreate = context.InventoryDeliveryVoucher.Where(c => Convert.ToDateTime(c.CreatedDate).Day == datenow.Day && Convert.ToDateTime(c.CreatedDate).Month == datenow.Month && Convert.ToDateTime(c.CreatedDate).Year == datenow.Year).Count();
                        var statusInventoryDeliveryVoucher = context.CategoryType.FirstOrDefault(f => f.CategoryTypeCode == "TPHX");
                        var statusInventoryDeliveryVoucherCXL = context.Category.FirstOrDefault(f => f.CategoryCode == "CXK" && f.CategoryTypeId == statusInventoryDeliveryVoucher.CategoryTypeId);

                        var index = 1;
                        listWarehouseParentId.ForEach(warehouse =>
                        {
                            var inventoryDeliveryVoucher = new InventoryDeliveryVoucher
                            {
                                InventoryDeliveryVoucherId = Guid.NewGuid(),
                                InventoryDeliveryVoucherCode = "PX-" + ConverCreateId(totalInvertoryCreate + index),
                                StatusId = statusInventoryDeliveryVoucherCXL.CategoryId,
                                InventoryDeliveryVoucherType = 1,
                                WarehouseId = warehouse.ParentId,
                                ObjectId = customerOrderObj.OrderId,
                                Receiver = customerOrderObj.RecipientName,
                                InventoryDeliveryVoucherDate = datenow,
                                InventoryDeliveryVoucherTime = today,
                                LicenseNumber = 1,
                                Active = true,
                                CreatedDate = datenow,
                                CreatedById = parameter.UserId
                            };
                            context.InventoryDeliveryVoucher.Add(inventoryDeliveryVoucher);

                            var lstInventoryDeliveryVoucherDetail = objDetail.Where(c => c.WarehouseId != null && warehouse.ListChildId.Contains(c.WarehouseId.Value))
                                .Select(m => new InventoryDeliveryVoucherMapping
                                {
                                    InventoryDeliveryVoucherMappingId = Guid.NewGuid(),
                                    InventoryDeliveryVoucherId = inventoryDeliveryVoucher.InventoryDeliveryVoucherId,
                                    ProductId = m.ProductId.Value,
                                    QuantityRequest = m.Quantity.Value,
                                    QuantityInventory = 0,
                                    QuantityActual = m.Quantity.Value,
                                    PriceProduct = m.UnitPrice.Value,
                                    WarehouseId = m.WarehouseId.Value,
                                    Description = m.Description,
                                    Active = true,
                                    CreatedDate = datenow,
                                    CreatedById = parameter.UserId,
                                    CurrencyUnit = m.CurrencyUnit,
                                    UnitId = m.UnitId,
                                    ExchangeRate = m.ExchangeRate,
                                    Vat = m.Vat,
                                    DiscountType = m.DiscountType,
                                    DiscountValue = m.DiscountValue,
                                }).ToList();

                            context.InventoryDeliveryVoucherMapping.AddRange(lstInventoryDeliveryVoucherDetail);
                            index++;
                        });

                        #endregion

                        context.SaveChanges();
                        message = CommonMessage.Order.EDIT_ORDER_SUCCESS;
                        note.Description = employee.EmployeeName + CommonMessage.Note.NOTE_CONTENT_APPROVAL;
                        break;
                    case "REJECT":
                        var statusRTN = statusOrder.FirstOrDefault(s => s.OrderStatusCode == "RTN");
                        StatusId = statusRTN.OrderStatusId;
                        customerOrderObj.StatusId = statusRTN.OrderStatusId;
                        customerOrderObj.UpdatedById = parameter.UserId;
                        customerOrderObj.UpdatedDate = DateTime.Now;

                        message = CommonMessage.Order.EDIT_ORDER_SUCCESS;
                        note.Description = employee.EmployeeName + CommonMessage.Note.NOTE_CONTENT_REJECT + parameter.Description;
                        break;
                    case "CANCEL":
                        var statusCAN = statusOrder.FirstOrDefault(s => s.OrderStatusCode == "CAN");
                        customerOrderObj.StatusId = statusCAN.OrderStatusId;
                        StatusId = statusCAN.OrderStatusId;
                        customerOrderObj.UpdatedById = parameter.UserId;
                        customerOrderObj.UpdatedDate = DateTime.Now;

                        message = CommonMessage.Order.EDIT_ORDER_SUCCESS;
                        note.Description = employee.EmployeeName + CommonMessage.Note.NOTE_CONTENT_CANCEL;// + parameter.Description;
                        break;
                    case "EDIT_NEW":
                        var statusNEW = statusOrder.FirstOrDefault(s => s.OrderStatusCode == "DRA");
                        customerOrderObj.StatusId = statusNEW.OrderStatusId;
                        StatusId = statusNEW.OrderStatusId;
                        customerOrderObj.UpdatedById = parameter.UserId;
                        customerOrderObj.UpdatedDate = DateTime.Now;

                        message = CommonMessage.Order.EDIT_ORDER_SUCCESS;
                        note.Description = CommonMessage.Note.NOTE_CONTENT_EDIT_NEW;
                        break;
                    case "PAY_ORDER":
                        // Đề xuất mua hàng
                        var datetimenow = DateTime.Now;
                        TimeSpan timetoday = new TimeSpan(datetimenow.Hour, datetimenow.Minute, datetimenow.Second);
                        var numberOfPr = context.ProcurementRequest.Count();
                        var code = "RQP" + datetimenow.ToString("yy") + (numberOfPr + 1).ToString("D5");

                        var statusProcurementRequest = context.CategoryType.FirstOrDefault(f => f.CategoryTypeCode == "DDU");
                        var statusProcurementRequestCXL = context.Category.FirstOrDefault(f => f.CategoryCode == "DR" && f.CategoryTypeId == statusProcurementRequest.CategoryTypeId);

                        var procurementRequest = new ProcurementRequest();
                        procurementRequest.ProcurementRequestId = Guid.NewGuid();
                        procurementRequest.OrderId = parameter.CustomerOrderId;
                        procurementRequest.ProcurementCode = code;
                        procurementRequest.ProcurementContent = "Đặt hàng cho đơn hàng bán " + customerOrderObj.OrderCode + ", ngày " + customerOrderObj.OrderDate.ToString("dd/MM/yyy");
                        procurementRequest.RequestEmployeeId = employeeId;
                        procurementRequest.EmployeePhone = employeeContact.Phone;
                        procurementRequest.Unit = employee.OrganizationId;
                        procurementRequest.ApproverPostion = employee.PositionId;
                        procurementRequest.StatusId = statusProcurementRequestCXL.CategoryId;
                        procurementRequest.CreatedById = parameter.UserId;
                        procurementRequest.CreatedDate = datetimenow;
                        procurementRequest.Active = true;

                        context.ProcurementRequest.Add(procurementRequest);



                        objDetail.ForEach(item =>
                        {
                            var procurementRequestDetail = new ProcurementRequestItem();
                            procurementRequestDetail.ProcurementRequestItemId = Guid.NewGuid();
                            procurementRequestDetail.ProductId = item.ProductId;
                            procurementRequestDetail.VendorId = item.VendorId;
                            procurementRequestDetail.Quantity = item.Quantity;
                            procurementRequestDetail.ProcurementRequestId = procurementRequest.ProcurementRequestId;
                            //procurementRequestDetail.ProcurementPlanId = item.ProcurementPlanId;
                            procurementRequestDetail.Vat = item.Vat;
                            procurementRequestDetail.DiscountType = item.DiscountType;
                            procurementRequestDetail.DiscountValue = item.DiscountValue;
                            procurementRequestDetail.IncurredUnit = item.IncurredUnit;
                            procurementRequestDetail.Description = item.Description;
                            procurementRequestDetail.CreatedById = parameter.UserId;
                            procurementRequestDetail.CreatedDate = datetimenow;
                            procurementRequestDetail.CurrencyUnit = item.CurrencyUnit;
                            procurementRequestDetail.ExchangeRate = item.ExchangeRate;
                            procurementRequestDetail.OrderDetailType = item.OrderDetailType;
                            procurementRequestDetail.WarehouseId = item.WarehouseId;

                            // bảng giá ncc
                            var _today = DateTime.Now;
                            var productVendorMapping = context.ProductVendorMapping
                                .Where(x => x.Active == true && (x.MiniumQuantity <= item.Quantity) &&
                                            (x.ProductId == item.ProductId) &&
                                            ((x.ToDate != null && x.FromDate.Value.Date <= _today.Date && _today.Date <= x.ToDate.Value.Date) ||
                                             (x.ToDate.Value.Date == null && x.FromDate.Value.Date <= _today.Date)))
                                .OrderBy(y => y.Price)
                                .ToList();

                            if (productVendorMapping.Count < 1 && item.VendorId == null)
                            {
                                procurementRequestDetail.UnitPrice = 0;
                            }
                            else if (productVendorMapping.Count > 0 && item.VendorId == null)
                            {
                                procurementRequestDetail.UnitPrice = productVendorMapping[0].Price;
                            }


                            context.ProcurementRequestItem.Add(procurementRequestDetail);
                        });
                        ProcurementRequestId = procurementRequest.ProcurementRequestId;
                        break;
                    //case "CLONE":
                    //    var statusNEWClone = statusOrder.FirstOrDefault(s => s.OrderStatusCode == "DRA");
                    //    int count = customerOrderObj.CloneCount == null ? 1 : customerOrderObj.CloneCount.Value + 1;
                    //    var cloneObj = customerOrderObj;
                    //    cloneObj.OrderId = Guid.NewGuid();
                    //    cloneObj.OrderCode = GenerateOrderCode(1);

                    //    customerOrderObj.CloneCount = count;

                    //    message = CommonMessage.Order.EDIT_ORDER_SUCCESS;
                    //    note.Description = CommonMessage.Note.NOTE_CONTENT_EDIT_NEW;
                    //    break;
                    case "CANCEL_APPROVAL":
                        var statusNew = statusOrder.FirstOrDefault(s => s.OrderStatusCode == "DRA");
                        if (customerOrderObj != null)
                        {
                            if (statusNew != null) customerOrderObj.StatusId = statusNew.OrderStatusId;
                            StatusId = statusNew.OrderStatusId;
                            customerOrderObj.UpdatedById = parameter.UserId;
                            customerOrderObj.UpdatedDate = DateTime.Now;
                        }

                        message = CommonMessage.Order.CANCEL_APPROVAL_SUCCESS;
                        note.Description = "Đơn hàng đã được hủy yêu cầu phê duyệt bởi nhân viên: " + employee.EmployeeCode + " - " + employee.EmployeeName;
                        break;
                }

                if (parameter.ObjectType != "PAY_ORDER")
                {
                    context.CustomerOrder.Update(customerOrderObj);
                    context.Note.Add(note);
                }
                context.SaveChanges();

            }
            catch (Exception ex)
            {
                return new UpdateCustomerOrderResult
                {
                    MessageCode = ex.Message,
                    StatusCode = HttpStatusCode.ExpectationFailed
                };
            }
            
            #region gửi mail thông báo

            switch (parameter.ObjectType)
            {
                case "CANCEL_APPROVAL":
                    NotificationHelper.AccessNotification(context, TypeModel.CustomerOrderDetail, "CANCEL_APPROVAL", new CustomerOrder(),
                        customerOrderObj, true, empId: customerOrderObj.Seller);
                    break;
            }

            #endregion

            return new UpdateCustomerOrderResult
            {
                ProcurementRequestId = ProcurementRequestId,
                StatusId = StatusId,
                MessageCode = message,
                StatusCode = HttpStatusCode.OK
            };
        }

        #region Phần hỗ trợ gửi mail

        private static string ReplaceTokenForContent(TNTN8Context context, object model,
            string emailContent, List<SystemParameter> configEntity)
        {
            var result = emailContent;

            #region Common Token

            const string Logo = "[LOGO]";
            const string OrderCode = "[ORDER_CODE]";
            const string EmployeeName = "[EMPLOYEE_NAME]";
            const string EmployeeCode = "[EMPLOYEE_CODE]";
            const string UpdatedDate = "[UPDATED_DATE]";
            const string Url_Login = "[URL]";

            #endregion

            var _model = model as CustomerOrder;

            #region Replace token

            #region replace logo

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

            #endregion

            #region replace order code

            if (result.Contains(OrderCode) && _model.OrderCode != null)
            {
                result = result.Replace(OrderCode, _model.OrderCode.Trim());
            }

            #endregion

            #region replaca change employee code

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

            #endregion

            #region replace change employee name

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

            #endregion

            #region replace updated date

            if (result.Contains(UpdatedDate))
            {
                result = result.Replace(UpdatedDate, FormatDateToString(_model.UpdatedDate));
            }

            #endregion

            #region replace url 

            if (result.Contains(Url_Login))
            {
                var Domain = configEntity.FirstOrDefault(w => w.SystemKey == "Domain").SystemValueString;
                var loginLink = Domain + @"/login?returnUrl=%2Fhome";

                if (!String.IsNullOrEmpty(loginLink))
                {
                    result = result.Replace(Url_Login, loginLink);
                }
            }

            #endregion

            #endregion

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

        #endregion

        public string ConverCreateId(int totalRecordCreate)
        {
            var datenow = DateTime.Now;
            string year = datenow.Year.ToString().Substring(datenow.Year.ToString().Length - 2, 2);
            string month = datenow.Month < 10 ? "0" + datenow.Month.ToString() : datenow.Month.ToString();
            string day = datenow.Day < 10 ? "0" + datenow.Day.ToString() : datenow.Day.ToString();
            string total = "";
            if (totalRecordCreate > 999)
            {
                total = totalRecordCreate.ToString();
            }
            else if (totalRecordCreate > 99 && totalRecordCreate < 1000)
            {
                total = "0" + totalRecordCreate.ToString();
            }
            else if (totalRecordCreate > 9 && totalRecordCreate < 100)
            {
                total = "00" + totalRecordCreate.ToString();
            }
            else
            {
                total = "000" + totalRecordCreate.ToString();
            }
            var result = year + month + day + total;
            var code = "PX-" + result;
            var InventoryDeliveryVoucherObj = context.InventoryDeliveryVoucher.FirstOrDefault(i => i.InventoryDeliveryVoucherCode == code);
            if (InventoryDeliveryVoucherObj != null)
            {
                ConverCreateId(totalRecordCreate + 1);
            }
            return result;
        }

        public CheckBeforCreateOrUpdateOrderResult CheckBeforCreateOrUpdateOrder(CheckBeforCreateOrUpdateOrderParameter parameter)
        {
            try
            {
                bool isCheck = false;
                if (parameter.MaxDebt != 0)
                {
                    var statusDBH =
                        context.OrderStatus.FirstOrDefault(o => o.OrderStatusCode == "DLV" && o.Active == true);

                    if (statusDBH != null)
                    {
                        //Lấy những Đơn hàng có trạng thái Đơn hàng bán của khách hàng
                        var orderByCustomer = context.CustomerOrder.Where(co =>
                            co.CustomerId == parameter.CustomerId && co.StatusId == statusDBH.OrderStatusId).ToList();
                        var listOrderId = orderByCustomer.Select(y => y.OrderId).ToList();

                        //Lấy tổng thanh toán một phần của các đơn hàng trên nếu có
                        var tongThanhToanMotPhan = context.ReceiptOrderHistory
                            .Where(x => listOrderId.Contains(x.OrderId)).Sum(y => y.AmountCollected);

                        var amountOrderCustomer = parameter.AmountOrder; //Tổng thanh toán của đơn hàng hiện tại
                        orderByCustomer.ForEach(item =>
                        {
                            var discountMoney = item.DiscountType == true
                                ? (item.Amount * item.DiscountValue / 100)
                                : item.DiscountValue;
                            amountOrderCustomer =
                                amountOrderCustomer + item.Amount.Value - decimal.Parse(discountMoney.ToString());
                        });

                        if ((parameter.MaxDebt - amountOrderCustomer + tongThanhToanMotPhan) < 0)
                        {
                            isCheck = true;
                        }
                    }
                }
                return new CheckBeforCreateOrUpdateOrderResult
                {
                    isCheckMaxDebt = isCheck,
                    MessageCode = CommonMessage.Order.EDIT_ORDER_SUCCESS,
                    StatusCode = HttpStatusCode.OK,
                };
            }
            catch (Exception ex)
            {
                return new CheckBeforCreateOrUpdateOrderResult
                {
                    MessageCode = CommonMessage.Order.EDIT_ORDER_FAIL,
                    StatusCode = HttpStatusCode.ExpectationFailed,
                };
            }
        }

        private string GenerateOrderCode(int maxIndex)
        {
            var orderCode = "";
            var prefix = "SO";
            var now = DateTime.Now;
            var _day = now.Day.ToString("D2");
            var _month = now.Month.ToString("D2");
            var _year = (now.Year % 100).ToString();

            var new_final_order = context.CustomerOrder.OrderByDescending(z => z.CreatedDate)
                .FirstOrDefault(x => x.OrderCode.Contains("_" + _day + _month + _year + ""));

            if (new_final_order != null)
            {
                var currentOrderCode = new_final_order.OrderCode;
                //var currentMaxIndex = currentOrderCode.Substring(currentOrderCode.LastIndexOf("_") + 1);
                //maxIndex = Convert.ToInt32(currentMaxIndex);
                //maxIndex = maxIndex + 1;
                orderCode = prefix + "_" + _year + _month + _day + maxIndex.ToString("D4");
                var duplicateOrderFinal = context.CustomerOrder.FirstOrDefault(x => x.OrderCode == orderCode);
                if (duplicateOrderFinal != null)
                {
                    orderCode = GenerateOrderCode(maxIndex + 1);
                }
                return orderCode;
            }

            //maxIndex = maxIndex + 1;
            orderCode = prefix + "_" + _year + _month + _day + maxIndex.ToString("D4");
            var duplicateOrder = context.CustomerOrder.FirstOrDefault(x => x.OrderCode == orderCode);
            if (duplicateOrder != null)
            {
                orderCode = GenerateOrderCode(maxIndex + 1);
            }
            return orderCode;
        }

        private string GenerateOrderCodeNew(CreateCustomerOrderParameter parameterOrder, CreateOrUpdateCustomerOrderActionParameter parameterOrderAction,
             bool isOrderAction, bool? isOrderProcess, CreateOrUpdateCustomerOrderProcessParameter parameterOrderProcess)
        {
            var orderCode = "";

            if (isOrderProcess == true)
            {
                //Loại phiếu yêu cầu
                var statusDone = GeneralList.GetTrangThais("OrderProcess").FirstOrDefault(x => x.Value == 3).Value; // Hoàn thành 

                var cusCode = context.Customer.FirstOrDefault(x => x.CustomerId == parameterOrderProcess.OrderProcess.CustomerId)?.CustomerCode;
                var servicePacketCode = context.ServicePacket.FirstOrDefault(x => x.Id == parameterOrderProcess.OrderProcess.ServicePacketId)?.Code;

                //Số phiếu yêu cầu hỗ trợ KH đã đặt
                var countCustomerOrder = context.OrderProcess.Where(x => x.CustomerId == parameterOrderProcess.OrderProcess.CustomerId).Count();
                return orderCode = cusCode + "-" + servicePacketCode + "-" + (countCustomerOrder + 1).ToString();
            }

            if(isOrderAction == false)
            {
                //Loại phiếu yêu cầu
                var listOrderType = GeneralList.GetTrangThais("OrderType").ToList();
                var extenType = listOrderType.FirstOrDefault(x => x.Value == 2).Value; // Bổ sung
                var normalType = listOrderType.FirstOrDefault(x => x.Value == 1).Value; // Phiếu yêu cầu bt

                var pre = "YC";
                var cusCode = context.Customer.FirstOrDefault(x => x.CustomerId == parameterOrder.CusOrder.CustomerId)?.CustomerCode;
                var servicePacketCode = context.ServicePacket.FirstOrDefault(x => x.Id == parameterOrder.ServicePacketId)?.Code;

                //Số phiếu yêu cầu hỗ trợ KH đã đặt
                var countCustomerOrder = context.CustomerOrder.Where(x => x.CustomerId == parameterOrder.CusOrder.CustomerId 
                                && x.IsOrderAction == false && x.OrderType == normalType).Count().ToString();
                orderCode = pre + "-" + cusCode + "-" + servicePacketCode + "-" + countCustomerOrder;
                
                if (parameterOrder.CusOrder.OrderType == extenType)
                {
                    //Số phiếu yêu cầu hỗ trợ bổ sung KH đã đặt
                    var countCustomerOrderExten = context.CustomerOrder.Where(x => x.CustomerId == parameterOrder.CusOrder.CustomerId 
                                && x.IsOrderAction == false && x.OrderType == extenType).Count().ToString();
                    orderCode = "YCBS" + "-" + countCustomerOrderExten + "_" + orderCode;
                }
            }
            else
            {
                var pre = "HT";
                //Lấy code của phiếu yêu cầu gán với phiếu hỗ trợ
                var customerOrderCode = context.CustomerOrder.FirstOrDefault(x => x.OrderId == parameterOrderAction.CustomerOrderId && x.IsOrderAction == false);
                orderCode = pre + "_" + customerOrderCode?.OrderCode;
            }
            return orderCode;
        }

        private string GenerateorderCode()
        {
            string currentYear = DateTime.Now.Year.ToString();
            int count = context.CustomerOrder.Count();
            string result = "DH-" + currentYear.Substring(currentYear.Length - 2) + returnNumberOrder(count.ToString());
            return result;
        }

        private string returnNumberOrder(string number)
        {
            switch (number.Length)
            {
                case 1: { return "000" + number; }
                case 2: { return "00" + number; }
                case 3: { return "0" + number; }
                default:
                    return number;
                    break;
            }
        }

        private string returnOrderDetail(string parameter0, string parameter1, string parameter2, string parameter3, string parameter4, string parameter5, string parameter6, string parameter7, string parameter8)
        {
            string Result = string.Format(@"<tr><td class='title'>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td><td>{5}</td><td>{6}</td><td>{7}</td><td>{8}</td></tr>",
                parameter0, parameter1, parameter2, parameter3, parameter4, parameter5, parameter6, parameter7, parameter8);
            return Result;
        }

        public GetCustomerOrderBySellerResult GetCustomerOrderBySeller(GetCustomerOrderBySellerParameter parameter)
        {
            try
            {
                this.iAuditTrace.Trace(ActionName.GETALL, ObjectName.ORDER, "GetAllOrderBySeller", parameter.UserId);

                var commonProductCategory = context.ProductCategory.ToList();
                var commonProducts = context.Product.ToList();
                var commonProductCategories = context.ProductCategory.ToList();
                var commonCustomers = context.Customer.ToList();
                var commonOrderStatus = context.OrderStatus.Where(x => x.OrderStatusCode == "IP" || x.OrderStatusCode == "DLV" || x.OrderStatusCode == "PD" || x.OrderStatusCode == "COMP").ToList();
                var orderStatusIdList = commonOrderStatus.Select(o => o.OrderStatusId).ToList();
                var custormerIdList = commonCustomers.Select(c => c.CustomerId).ToList();
                var commonCustomerOrder = context.CustomerOrder.Where(c => c.StatusId != null && orderStatusIdList.Contains((Guid)c.StatusId)
                                              && parameter.Seller == c.Seller
                                              && (parameter.OrderDateStart == null || parameter.OrderDateStart == DateTime.MinValue || parameter.OrderDateStart.Value.Date <= c.OrderDate.Date)
                                              && (parameter.OrderDateEnd == null || parameter.OrderDateEnd == DateTime.MinValue || parameter.OrderDateEnd.Value.Date >= c.OrderDate.Date)).ToList();

                var custormerOrderIdList = commonCustomerOrder.Select(c => c.OrderId).ToList();
                var commonCustomerOrderDetails = context.CustomerOrderDetail.Where(c => custormerOrderIdList.Contains(c.OrderId)).ToList();
                var productIdList = commonCustomerOrderDetails.Select(c => c.ProductId).ToList();
                var commonProduct = context.Product.Where(c => productIdList.Contains(c.ProductId)).ToList();

                #region Lấy list đơn hàng mới nhất

                #region LongNH comment code

                //var commonProductCategory = context.ProductCategory.ToList();
                //var commonCustomerOrderDetails = context.CustomerOrderDetail.ToList();
                //var commonProducts = context.Product.ToList();
                //var commonProductCategories = context.ProductCategory.ToList();
                //var commonCustomerOrder = context.CustomerOrder.ToList();
                //var commonCustomers = context.Customer.ToList();
                //var commonOrderStatus = context.OrderStatus.Where(x => x.OrderStatusCode == "IP" || x.OrderStatusCode == "DLV" || x.OrderStatusCode == "PD" || x.OrderStatusCode == "COMP").ToList();     
                //var orderListAll = (from or in commonCustomerOrder
                //                    join cus in commonCustomers on or.CustomerId equals cus.CustomerId
                //                    join orst in commonOrderStatus on or.StatusId equals orst.OrderStatusId
                //                    where parameter.Seller == or.Seller
                //                    && (orst.OrderStatusCode == "IP" || orst.OrderStatusCode == "DLV" || orst.OrderStatusCode == "PD" || orst.OrderStatusCode == "COMP")
                //                    && (parameter.OrderDateStart == null || parameter.OrderDateStart == DateTime.MinValue || parameter.OrderDateStart.Value.Date <= or.OrderDate.Date)
                //                    && (parameter.OrderDateEnd == null || parameter.OrderDateEnd == DateTime.MinValue || parameter.OrderDateEnd.Value.Date >= or.OrderDate.Date)
                //                    select new CustomerOrderEntityModel
                //                    {
                //                        OrderId = or.OrderId,
                //                        OrderCode = or.OrderCode,
                //                        DiscountType = or.DiscountType,
                //                        DiscountValue = or.DiscountValue,
                //                        Amount = (decimal)((or.DiscountType == true) ? (or.Amount * (1 - (or.DiscountValue / 100))) : (or.Amount - or.DiscountValue)),
                //                        CreatedDate = or.CreatedDate,
                //                        CustomerName = cus.CustomerName
                //                    }
                //                 ).OrderByDescending(or => or.CreatedDate).ToList();

                #endregion

                var orderListAll = new List<CustomerOrderEntityModel>();
                commonCustomerOrder.ForEach(item =>
                {
                    var customer = commonCustomers.FirstOrDefault(c => c.CustomerId == item.CustomerId);
                    if (customer != null)
                    {
                        var customerOrder = new CustomerOrderEntityModel();
                        customerOrder.OrderId = item.OrderId;
                        customerOrder.OrderCode = item.OrderCode;
                        customerOrder.DiscountType = item.DiscountType;
                        customerOrder.DiscountValue = item.DiscountValue;
                        customerOrder.Amount = (decimal)((item.DiscountType == true) ? (item.Amount * (1 - (item.DiscountValue / 100))) : (item.Amount - item.DiscountValue));
                        customerOrder.CreatedDate = item.CreatedDate;
                        customerOrder.CustomerName = customer.CustomerName;
                        orderListAll.Add(customerOrder);
                    }
                });

                var orderList = orderListAll.OrderByDescending(c => c.CreatedDate).Take(5).ToList();

                #endregion

                #region Lấy tỉ lệ doanh số theo nhóm sản phẩm

                var listProductCaterogyFirstParent = (from productcategory in commonProductCategory
                                                      where productcategory.ProductCategoryLevel == 0
                                                      select new
                                                      {
                                                          productcategory.ProductCategoryId,
                                                          productcategory.ProductCategoryName
                                                      }).ToList();

                int? levelMaxProductCategory = commonProductCategory.Max(x => x.ProductCategoryLevel);

                List<ListCategoryId> newListProductCategory = new List<ListCategoryId>();
                listProductCaterogyFirstParent.ForEach(item =>
                {
                    List<Guid> newListProductCategoryIdChildren = new List<Guid>();
                    newListProductCategoryIdChildren.Add(item.ProductCategoryId);
                    newListProductCategoryIdChildren = getProductCategoryChildrenId(item.ProductCategoryId, newListProductCategoryIdChildren, commonProductCategory);

                    var listCategoryId = new ListCategoryId();
                    listCategoryId.ParentProductCategoryId = item.ProductCategoryId;
                    listCategoryId.ParentProductCategoryName = item.ProductCategoryName;
                    listCategoryId.ListChildrent = newListProductCategoryIdChildren;
                    newListProductCategory.Add(listCategoryId);
                });

                #region Long comment

                //newListProductCategory.ForEach(item =>
                //{

                //    var newProductCategoryList = (from customerOderDetail in commonCustomerOrderDetails
                //                                  join product in commonProducts on customerOderDetail.ProductId equals product.ProductId
                //                                  join productCategory in commonProductCategories on product.ProductCategoryId equals productCategory.ProductCategoryId
                //                                  join customerOrder in commonCustomerOrder on customerOderDetail.OrderId equals customerOrder.OrderId
                //                                  join orderStatus in commonOrderStatus on customerOrder.StatusId equals orderStatus.OrderStatusId
                //                                  where parameter.Seller == customerOrder.Seller
                //                                  && (item.ListChildrent.Contains(productCategory.ProductCategoryId) || item.ListChildrent.Count == 0)
                //                                  && (orderStatus.OrderStatusCode == "IP" || orderStatus.OrderStatusCode == "DLV" || orderStatus.OrderStatusCode == "PD" || orderStatus.OrderStatusCode == "COMP")
                //                                  && (parameter.OrderDateStart == null || parameter.OrderDateStart == DateTime.MinValue || parameter.OrderDateStart.Value.Date <= customerOrder.OrderDate.Date)
                //                                  && (parameter.OrderDateEnd == null || parameter.OrderDateEnd == DateTime.MinValue || parameter.OrderDateEnd.Value.Date >= customerOrder.OrderDate.Date)
                //                                  select new
                //                                  {
                //                                      productCategory.ProductCategoryId,
                //                                      productCategory.ProductCategoryName,
                //                                      Total = CalculatorTotal(customerOderDetail.Vat.Value, customerOderDetail.DiscountType.Value, customerOderDetail.DiscountValue.Value, customerOderDetail.Quantity.Value, customerOderDetail.UnitPrice.Value, customerOderDetail.ExchangeRate.Value)
                //                                  }
                //                           ).ToList();

                //    var new_ListProductCategory = newProductCategoryList.GroupBy(x => new { x.ProductCategoryId, x.ProductCategoryName }).Select(y => new
                //    {
                //        Id = y.Key,
                //        y.Key.ProductCategoryId,
                //        y.Key.ProductCategoryName,
                //        Total = y.Sum(s => s.Total)
                //    }).OrderByDescending(x => x.Total).ToList();

                //    var listtt = new ListCategoryResult();
                //    if (new_ListProductCategory.Count > 0)
                //    {
                //        decimal total_money = 0;
                //        listtt.ParentProductCategoryId = item.ParentProductCategoryId;
                //        listtt.ParentProductCategoryName = item.ParentProductCategoryName;

                //        new_ListProductCategory.ForEach(element =>
                //        {
                //            total_money += element.Total;
                //        });
                //        listtt.Total = total_money;
                //        newListProductCategoryResult.Add(listtt);
                //    }
                //});

                #endregion

                #region Lấy ra danh sách tất cả danh mục bán được sản phẩm

                List<ListCategoryResult> newListProductCategoryResult = new List<ListCategoryResult>();
                var productCategoryIdList = commonProduct.Select(x => x.ProductCategoryId).ToList();
                var productCategoryList = commonProductCategory.Where(c => productCategoryIdList.Contains(c.ProductCategoryId)).ToList();
                List<ProductCategoryModel> listProductCategoryModel = new List<ProductCategoryModel>();
                commonCustomerOrderDetails.ForEach(item =>
                {
                    #region Add by Dung: viết lại công thức tính tổng tiền theo từng sản phẩm, mỗi sản phẩm phải trừ tiền chiết khấu của tổng đơn hàng
                    var customerOrder = commonCustomerOrder.FirstOrDefault(f => f.OrderId == item.OrderId);
                    decimal? discountPerOrder = 0;
                    if (customerOrder.DiscountType == true)
                    {
                        //chiết khấu phần trăm
                        discountPerOrder = customerOrder?.DiscountValue ?? 0;
                    }
                    else
                    {
                        //chiết khấu theo số tiền
                        discountPerOrder = (customerOrder.DiscountValue / customerOrder.Amount) * 100;
                    }
                    #endregion

                    var product = commonProduct.FirstOrDefault(x => x.ProductId == item.ProductId);

                    if (product != null)
                    {
                        var category = commonProductCategory.FirstOrDefault(x => x.ProductCategoryId == product.ProductCategoryId);

                        var productCategoryModel = new ProductCategoryModel();
                        productCategoryModel.ProductCategoryId = product.ProductCategoryId;
                        productCategoryModel.ProductCategoryName = category.ProductCategoryName;
                        //productCategoryModel.Total = CalculatorTotal(item.Vat.Value, item.DiscountType.Value, item.DiscountValue.Value, item.Quantity.Value, item.UnitPrice.Value, item.ExchangeRate.Value);
                        productCategoryModel.Total = ReCalculatorTotal(item.Vat.Value, item.DiscountType.Value, item.DiscountValue.Value, item.Quantity.Value, item.UnitPrice.Value, item.ExchangeRate.Value, discountPerOrder);
                        listProductCategoryModel.Add(productCategoryModel);
                    }
                });

                #endregion

                // Gom nhóm theo danh mục và tính tổng hóa đơn theo từng danh mục đó
                var new_ListProductCategory = listProductCategoryModel.GroupBy(x => new { x.ProductCategoryId, x.ProductCategoryName }).Select(y => new
                {
                    Id = y.Key,
                    y.Key.ProductCategoryId,
                    y.Key.ProductCategoryName,
                    Total = y.Sum(s => s.Total)
                }).OrderByDescending(x => x.Total).ToList();

                // Tỉnh tổng hóa đơn theo danh mục gốc(danh mục cấp 0)
                newListProductCategory.ForEach(item =>
                {
                    var newProductCategoryId = productCategoryList.Where(c => item.ListChildrent.Contains(c.ProductCategoryId)).Select(c => c.ProductCategoryId).ToList();

                    // Nếu danh mục nào bán được sản phẩm thì tính tổng theo danh mục đó và add vào newListProductCategoryResult
                    if (newProductCategoryId.Count != 0)
                    {
                        var listCategoryResult = new ListCategoryResult();
                        var total = new_ListProductCategory.Where(x => newProductCategoryId.Contains((Guid)x.ProductCategoryId)).Sum(y => y.Total);
                        listCategoryResult.Total = total;
                        listCategoryResult.ParentProductCategoryName = item.ParentProductCategoryName;
                        listCategoryResult.ParentProductCategoryId = item.ParentProductCategoryId;
                        newListProductCategoryResult.Add(listCategoryResult);
                    }
                });

                //Tinh tong sau chiet khau
                decimal total_money_all = 0;
                orderListAll.ForEach(element =>
                {
                    total_money_all += element.Amount.Value;
                });

                //Tinh tong gia tri cua cac Nhom san pham
                //decimal total_product = 0;
                //newListProductCategoryResult.ForEach(item =>
                //{
                //    total_product += item.Total;
                //});

                List<dynamic> lstResult = new List<dynamic>();
                newListProductCategoryResult.ForEach(item =>
                {
                    var sampleObject = new ExpandoObject() as IDictionary<string, Object>;
                    sampleObject.Add("ProductCategoryId", item.ParentProductCategoryId);
                    sampleObject.Add("ProductCategoryName", item.ParentProductCategoryName);
                    sampleObject.Add("Total", item.Total);
                    decimal percent = 0;
                    percent = (item.Total / total_money_all) * 100;
                    percent = Math.Round(percent, 1);
                    sampleObject.Add("Percent", percent);
                    lstResult.Add(sampleObject);
                });

                #endregion

                return new GetCustomerOrderBySellerResult()
                {
                    OrderList = orderList,
                    lstResult = lstResult,
                    totalProduct = total_money_all,
                    levelMaxProductCategory = levelMaxProductCategory,
                    MessageCode = "Success",
                    StatusCode = HttpStatusCode.OK
                };
            }
            catch (Exception e)
            {
                return new GetCustomerOrderBySellerResult()
                {
                    MessageCode = e.Message,
                    StatusCode = HttpStatusCode.ExpectationFailed
                };
            }
            
        }

        private decimal CalculatorTotal(decimal? Vat, bool DiscountType, decimal? DiscountValue, decimal Quantity, decimal UnitPrice, decimal? ExchangeRate)
        {
            decimal total = (Quantity * UnitPrice * ExchangeRate.Value);
            if (Vat >= 0)
            {
                if (DiscountType == true && DiscountValue >= 0)
                {
                    //Nếu chiết khấu bằng %
                    total = total - (total * DiscountValue.Value / 100);
                }
                else if (DiscountType == false && DiscountValue >= 0)
                {
                    //Nếu chiếu khấu bằng tiền
                    total = total - DiscountValue.Value;
                }
                total = total + (total * Vat.Value / 100);
            }
            else if (Vat == null)
            {
                if (DiscountType == true && DiscountValue >= 0)
                {
                    //Nếu chiết khấu bằng %
                    total = total - (total * DiscountValue.Value / 100);
                }
                else if (DiscountType == false && DiscountValue >= 0)
                {
                    //Nếu chiếu khấu bằng tiền
                    total = total - DiscountValue.Value;
                }
            }
            return total;
        }

        //add by dung
        private decimal ReCalculatorTotal(decimal? Vat, bool DiscountType, decimal? DiscountValue, decimal Quantity, decimal UnitPrice, decimal? ExchangeRate, decimal? DiscountPerOrder)
        {
            decimal total = (Quantity * UnitPrice * ExchangeRate.Value);
            if (Vat >= 0)
            {
                if (DiscountType == true && DiscountValue >= 0)
                {
                    //Nếu chiết khấu bằng %
                    total = total - (total * DiscountValue.Value / 100);
                }
                else if (DiscountType == false && DiscountValue >= 0)
                {
                    //Nếu chiếu khấu bằng tiền
                    total = total - DiscountValue.Value;
                }
                total = total + (total * Vat.Value / 100);
            }
            else if (Vat == null)
            {
                if (DiscountType == true && DiscountValue >= 0)
                {
                    //Nếu chiết khấu bằng %
                    total = total - (total * DiscountValue.Value / 100);
                }
                else if (DiscountType == false && DiscountValue >= 0)
                {
                    //Nếu chiếu khấu bằng tiền
                    total = total - DiscountValue.Value;
                }
            }

            var discountValue = (total * DiscountPerOrder.Value) / 100;

            total = total - discountValue;

            return total;
        }

        private List<Guid> getProductCategoryChildrenId(Guid? Id, List<Guid> lst, List<ProductCategory> productCategoryList)
        {
            var ProductCategory = productCategoryList.Where(m => m.ParentId == Id).ToList();
            ProductCategory.ForEach(item =>
            {
                lst.Add(item.ProductCategoryId);
                getProductCategoryChildrenId(item.ProductCategoryId, lst, productCategoryList);
            });
            return lst;
        }

        public GetProductCategoryGroupByLevelResult GetProductCategoryGroupByLevel(GetProductCategoryGroupByLevelParameter parameter)
        {
            try
            {
                var commonProductCategory = context.ProductCategory.ToList();
                var commonOrderStatus = context.OrderStatus.Where(x => x.OrderStatusCode == "IP" || x.OrderStatusCode == "DLV" || x.OrderStatusCode == "PD" || x.OrderStatusCode == "COMP").ToList();
                var orderStatusIdList = commonOrderStatus.Select(o => o.OrderStatusId).ToList();

                var commonCustomerOrder = context.CustomerOrder.Where(c => c.StatusId != null && orderStatusIdList.Contains((Guid)c.StatusId)
                                              && parameter.Seller == c.Seller
                                              && (parameter.OrderDateStart == null || parameter.OrderDateStart == DateTime.MinValue || parameter.OrderDateStart.Value.Date <= c.OrderDate.Date)
                                              && (parameter.OrderDateEnd == null || parameter.OrderDateEnd == DateTime.MinValue || parameter.OrderDateEnd.Value.Date >= c.OrderDate.Date)).ToList();
                var custormerOrderIdList = commonCustomerOrder.Select(c => c.OrderId).ToList();
                var commonCustomerOrderDetails = context.CustomerOrderDetail.Where(c => custormerOrderIdList.Contains(c.OrderId)).ToList();
                var productIdList = commonCustomerOrderDetails.Select(c => c.ProductId).ToList();
                var commonProduct = context.Product.Where(c => productIdList.Contains(c.ProductId)).ToList();

                var listProductCaterogyFirstParent = (from productcategory in commonProductCategory
                                                      where productcategory.ProductCategoryLevel == parameter.ProductCategoryLevel
                                                      select new
                                                      {
                                                          productcategory.ProductCategoryId,
                                                          productcategory.ProductCategoryName
                                                      }).ToList();

                List<ListCategoryId> newListProductCategory = new List<ListCategoryId>();
                listProductCaterogyFirstParent.ForEach(item =>
                {
                    List<Guid> newListProductCategoryIdChildren = new List<Guid>();
                    newListProductCategoryIdChildren.Add(item.ProductCategoryId);
                    newListProductCategoryIdChildren = getProductCategoryChildrenId(item.ProductCategoryId, newListProductCategoryIdChildren, commonProductCategory);

                    var listCategoryId = new ListCategoryId();
                    listCategoryId.ParentProductCategoryId = item.ProductCategoryId;
                    listCategoryId.ParentProductCategoryName = item.ProductCategoryName;
                    listCategoryId.ListChildrent = newListProductCategoryIdChildren;
                    newListProductCategory.Add(listCategoryId);
                });

                #region LongNH comment

                //newListProductCategory.ForEach(item =>
                //{
                //    var newProductCategoryList = (from customerOderDetail in commonCustomerOrderDetails
                //                                  join product in commonProducts on customerOderDetail.ProductId equals product.ProductId
                //                                  join productCategory in commonProductCategories on product.ProductCategoryId equals productCategory.ProductCategoryId
                //                                  where (item.ListChildrent.Contains(productCategory.ProductCategoryId) || item.ListChildrent.Count == 0)
                //                                  select new
                //                                  {
                //                                      productCategory.ProductCategoryId,
                //                                      productCategory.ProductCategoryName,
                //                                      Total = (decimal)(customerOderDetail.Quantity * customerOderDetail.UnitPrice)
                //                                  }
                //                           ).ToList();
                //    var new_List = newProductCategoryList.GroupBy(x => new { x.ProductCategoryName, x.ProductCategoryId }).Select(y => new
                //    {
                //        Id = y.Key,
                //        y.Key.ProductCategoryName,
                //        y.Key.ProductCategoryId,
                //        Total = y.Sum(s => s.Total)
                //    }).OrderByDescending(x => x.Total).ToList();
                //    var listtt = new ListCategoryResult();
                //    if (new_List.Count > 0)
                //    {
                //        decimal total_money = 0;
                //        listtt.ParentProductCategoryId = item.ParentProductCategoryId;
                //        listtt.ParentProductCategoryName = item.ParentProductCategoryName;
                //        new_List.ForEach(element =>
                //        {
                //            total_money += element.Total;
                //        });
                //        listtt.Total = total_money;
                //        newListProductCategoryResult.Add(listtt);
                //    }
                //});

                #endregion

                List<ListCategoryResult> newListProductCategoryResult = new List<ListCategoryResult>();
                var productCategoryIdList = commonProduct.Select(x => x.ProductCategoryId).ToList();
                var productCategoryList = commonProductCategory.Where(c => productCategoryIdList.Contains(c.ProductCategoryId)).ToList();
                List<ProductCategoryModel> listProductCategoryModel = new List<ProductCategoryModel>();


                commonCustomerOrderDetails.ForEach(item =>
                {
                    #region Add by Dung: viết lại công thức tính tổng tiền theo từng sản phẩm, mỗi sản phẩm phải trừ tiền chiết khấu của tổng đơn hàng
                    var customerOrder = commonCustomerOrder.FirstOrDefault(f => f.OrderId == item.OrderId);
                    decimal? discountPerOrder = 0;
                    if (customerOrder.DiscountType == true)
                    {
                        //chiết khấu phần trăm
                        discountPerOrder = customerOrder?.DiscountValue ?? 0;
                    }
                    else
                    {
                        //chiết khấu theo số tiền
                        discountPerOrder = (customerOrder.DiscountValue / customerOrder.Amount) * 100;
                    }
                    #endregion

                    var product = commonProduct.FirstOrDefault(x => x.ProductId == item.ProductId);

                    if (product != null)
                    {
                        var category = commonProductCategory.FirstOrDefault(x => x.ProductCategoryId == product.ProductCategoryId);

                        var productCategoryModel = new ProductCategoryModel();
                        productCategoryModel.ProductCategoryId = product.ProductCategoryId;
                        productCategoryModel.ProductCategoryName = category.ProductCategoryName;
                        //productCategoryModel.Total = CalculatorTotal(item.Vat.Value, item.DiscountType.Value, item.DiscountValue.Value, item.Quantity.Value, item.UnitPrice.Value, item.ExchangeRate.Value);
                        productCategoryModel.Total = ReCalculatorTotal(item.Vat.Value, item.DiscountType.Value, item.DiscountValue.Value, item.Quantity.Value, item.UnitPrice.Value, item.ExchangeRate.Value, discountPerOrder);
                        listProductCategoryModel.Add(productCategoryModel);
                    }
                });

                // Gom nhóm theo danh mục và tính tổng hóa đơn theo từng danh mục đó
                var new_ListProductCategory = listProductCategoryModel.GroupBy(x => new { x.ProductCategoryId, x.ProductCategoryName }).Select(y => new
                {
                    Id = y.Key,
                    y.Key.ProductCategoryId,
                    y.Key.ProductCategoryName,
                    Total = y.Sum(s => s.Total)
                }).OrderByDescending(x => x.Total).ToList();

                // Tỉnh tổng hóa đơn theo danh mục gốc(danh mục cấp 0)
                newListProductCategory.ForEach(item =>
                {
                    var newProductCategoryId = productCategoryList.Where(c => item.ListChildrent.Contains(c.ProductCategoryId)).Select(c => c.ProductCategoryId).ToList();

                    // Nếu danh mục nào bán được sản phẩm thì tính tổng theo danh mục đó và add vào newListProductCategoryResult
                    if (newProductCategoryId.Count != 0)
                    {
                        var listCategoryResult = new ListCategoryResult();
                        var total = new_ListProductCategory.Where(x => newProductCategoryId.Contains((Guid)x.ProductCategoryId)).Sum(y => y.Total);
                        listCategoryResult.Total = total;
                        listCategoryResult.ParentProductCategoryName = item.ParentProductCategoryName;
                        listCategoryResult.ParentProductCategoryId = item.ParentProductCategoryId;
                        newListProductCategoryResult.Add(listCategoryResult);
                    }
                });

                //Tinh tong gia tri cua cac Nhom san pham
                decimal total_product = 0;
                newListProductCategoryResult.ForEach(item =>
                {
                    total_product += item.Total;
                });
                newListProductCategoryResult.OrderByDescending(c => c.Total);
                List<dynamic> lstResult = new List<dynamic>();
                newListProductCategoryResult.ForEach(item =>
                {
                    var sampleObject = new ExpandoObject() as IDictionary<string, Object>;
                    sampleObject.Add("ProductCategoryId", item.ParentProductCategoryId);
                    sampleObject.Add("ProductCategoryName", item.ParentProductCategoryName);
                    sampleObject.Add("Total", item.Total);
                    decimal percent = 0;
                    percent = (item.Total / total_product) * 100;
                    percent = Math.Round(percent, 1);
                    sampleObject.Add("Percent", percent);
                    lstResult.Add(sampleObject);
                });

                return new GetProductCategoryGroupByLevelResult()
                {
                    lstResult = lstResult,
                    MessageCode = "Success",
                    StatusCode = HttpStatusCode.OK
                };
            }
            catch(Exception e)
            {
                return new GetProductCategoryGroupByLevelResult()
                {
                    MessageCode = e.Message,
                    StatusCode = HttpStatusCode.ExpectationFailed
                };
            }       
        }

        public GetEmployeeListByOrganizationIdResult GetEmployeeListByOrganizationId(GetEmployeeListByOrganizationIdParameter parameter)
        {
            try
            {
                var user = context.User.FirstOrDefault(x => x.UserId == parameter.UserId);
                var employee = context.Employee.FirstOrDefault(u => u.EmployeeId == user.EmployeeId);
                var commonProductCategory = context.ProductCategory.ToList();
                var allStatusOrder = context.OrderStatus.Where(c => c.Active == true).ToList();
                var organizationId = parameter.OrganizationId;
                List<Guid?> listGetAllChild = new List<Guid?>();
                if (organizationId != null)
                {
                    listGetAllChild.Add(organizationId);
                    listGetAllChild = getOrganizationChildrenId(organizationId, listGetAllChild);
                }

                var commonOrganization = context.Organization
                    .Where(x => listGetAllChild.Contains(x.OrganizationId)).ToList();
                var organizationIdList = commonOrganization.Select(x => x.OrganizationId).ToList();
                var commonEmployee = context.Employee
                    .Where(x => x.OrganizationId != null && organizationIdList.Contains((Guid)x.OrganizationId)).ToList();
                var employeeIdList = commonEmployee.Select(x => x.EmployeeId).ToList();
                var customerOrderAllStatus = context.CustomerOrder.Where(c =>
                    c.StatusId != null
                    && (parameter.OrderDateStart == null ||
                        parameter.OrderDateStart == DateTime.MinValue ||
                        parameter.OrderDateStart.Value.Date <= c.OrderDate.Date)
                    && (parameter.OrderDateEnd == null || parameter.OrderDateEnd == DateTime.MinValue ||
                        parameter.OrderDateEnd.Value.Date >= c.OrderDate.Date)).ToList();
                if (employee.IsManager)
                {
                    customerOrderAllStatus =
                        customerOrderAllStatus.Where(c => employeeIdList.Contains(c.Seller.Value)).ToList();
                }
                else
                {
                    customerOrderAllStatus =
                        customerOrderAllStatus.Where(c => c.Seller.Value == employee.EmployeeId).ToList();
                }
                var listOrderCode = context.SystemParameter.FirstOrDefault(x => x.SystemKey == "OrderStatus")
                    .SystemValueString.Split(';').ToList();
                var commonOrderStatus = allStatusOrder.Where(x => listOrderCode.Contains(x.OrderStatusCode)).ToList();
                var orderStatusIdList = commonOrderStatus.Select(o => o.OrderStatusId).ToList();
                var commonCustomerOrder = customerOrderAllStatus.Where(c =>
                    c.StatusId != null && orderStatusIdList.Contains((Guid)c.StatusId)
                ).ToList();

                var custormerOrderIdList = commonCustomerOrder.Select(c => c.OrderId).ToList();
                var commonCustomerOrderDetail = context.CustomerOrderDetail
                    .Where(c => custormerOrderIdList.Contains(c.OrderId)).ToList();
                var productIdList = commonCustomerOrderDetail.Select(c => c.ProductId).ToList();
                var commonProduct = context.Product.Where(c => productIdList.Contains(c.ProductId)).ToList();

                var listProductCaterogyFirstParent = (from productcategory in commonProductCategory
                                                      where productcategory.ProductCategoryLevel == 0
                                                      select new
                                                      {
                                                          productcategory.ProductCategoryId,
                                                          productcategory.ProductCategoryName
                                                      }).ToList();
                var listProductCategoryLevel = (from productcategory in commonProductCategory
                                                select new
                                                { levelMaxProductCategory = productcategory.ProductCategoryLevel }).ToList();
                int? levelMaxProductCategory = listProductCategoryLevel.Max(x => x.levelMaxProductCategory);

                #region Lấy danh sách 5 nhân viên bán hàng đạt doanh số cao nhất

                var employeeList = new List<EmployeeModel>();

                commonCustomerOrder.ForEach(item =>
                {
                    var employeeSeller = commonEmployee.FirstOrDefault(c => c.EmployeeId == item.Seller);
                    if (employeeSeller != null)
                    {
                        var newOrganization =
                            commonOrganization.FirstOrDefault(c => c.OrganizationId == employeeSeller.OrganizationId);
                        if (newOrganization != null)
                        {
                            var employeeTemp = new EmployeeModel();
                            employeeTemp.EmployeeId = employeeSeller.EmployeeId;
                            employeeTemp.EmployeeName = employeeSeller.EmployeeName;
                            employeeTemp.OrganizationName = newOrganization.OrganizationName;
                            employeeTemp.Amount = item.Amount.Value;
                            employeeTemp.DiscountType = item.DiscountType;
                            employeeTemp.DiscountValue = item.DiscountValue;
                            employeeList.Add(employeeTemp);
                        }
                    }
                });

                var newList = (employeeList.GroupBy(x => new { x.EmployeeId, x.EmployeeName, x.OrganizationName }).Select(y =>
                      new
                      {
                          Id = y.Key,
                          y.Key.EmployeeId,
                          y.Key.EmployeeName,
                          y.Key.OrganizationName,
                          Total = y.Sum(s =>
                              (s.DiscountType == true)
                                  ? (s.Amount * (1 - s.DiscountValue / 100))
                                  : (s.Amount - s.DiscountValue))
                      }).OrderByDescending(x => x.Total)).Take(5).ToList();

                #endregion

                #region Lấy danh sách đơn hàng 

                List<ListCategoryId> newListProductCategory = new List<ListCategoryId>();
                listProductCaterogyFirstParent.ForEach(item =>
                {
                    List<Guid> newListProductCategoryIdChildren = new List<Guid>();
                    newListProductCategoryIdChildren.Add(item.ProductCategoryId);
                    newListProductCategoryIdChildren = getProductCategoryChildrenId(item.ProductCategoryId,
                        newListProductCategoryIdChildren, commonProductCategory);

                    var listCategoryId = new ListCategoryId();
                    listCategoryId.ParentProductCategoryId = item.ProductCategoryId;
                    listCategoryId.ParentProductCategoryName = item.ProductCategoryName;
                    listCategoryId.ListChildrent = newListProductCategoryIdChildren;
                    newListProductCategory.Add(listCategoryId);
                });

                List<ListCategoryResult> newListProductCategoryResult = new List<ListCategoryResult>();
                var productCategoryIdList = commonProduct.Select(x => x.ProductCategoryId).ToList();
                var productCategoryList = commonProductCategory
                    .Where(c => productCategoryIdList.Contains(c.ProductCategoryId)).ToList();

                List<ProductCategoryModel> listProductCategoryModel = new List<ProductCategoryModel>();
                commonCustomerOrderDetail.ForEach(item =>
                {
                    #region Add by Dung: viết lại công thức tính tổng tiền theo từng sản phẩm, mỗi sản phẩm phải trừ tiền chiết khấu của tổng đơn hàng

                    var customerOrder = commonCustomerOrder.FirstOrDefault(f => f.OrderId == item.OrderId);
                    decimal? discountPerOrder = 0;
                    if (customerOrder.DiscountType == true)
                    {
                        //chiết khấu phần trăm
                        discountPerOrder = customerOrder?.DiscountValue ?? 0;
                    }
                    else
                    {
                        //chiết khấu theo số tiền
                        discountPerOrder = (customerOrder.DiscountValue / customerOrder.Amount) * 100;
                    }

                    #endregion

                    var product = commonProduct.FirstOrDefault(x => x.ProductId == item.ProductId);

                    if (product != null)
                    {
                        var category =
                            commonProductCategory.FirstOrDefault(x => x.ProductCategoryId == product.ProductCategoryId);

                        var productCategoryModel = new ProductCategoryModel();
                        productCategoryModel.ProductCategoryId = product.ProductCategoryId;
                        productCategoryModel.ProductCategoryName = category.ProductCategoryName;
                        productCategoryModel.Total = ReCalculatorTotal(item.Vat.Value, item.DiscountType.Value,
                            item.DiscountValue.Value, item.Quantity.Value, item.UnitPrice.Value, item.ExchangeRate.Value,
                            discountPerOrder);
                        listProductCategoryModel.Add(productCategoryModel);
                    }
                });

                #endregion

                // Gom nhóm theo danh mục và tính tổng hóa đơn theo từng danh mục đó
                var new_ListProductCategory = listProductCategoryModel
                    .GroupBy(x => new { x.ProductCategoryId, x.ProductCategoryName }).Select(y => new
                    {
                        Id = y.Key,
                        y.Key.ProductCategoryId,
                        y.Key.ProductCategoryName,
                        Total = y.Sum(s => s.Total)
                    }).OrderByDescending(x => x.Total).ToList();

                // Tỉnh tổng hóa đơn theo danh mục gốc(danh mục cấp 0)
                newListProductCategory.ForEach(item =>
                {
                    var newProductCategoryId = productCategoryList
                        .Where(c => item.ListChildrent.Contains(c.ProductCategoryId)).Select(c => c.ProductCategoryId)
                        .ToList();

                    // Nếu danh mục nào bán được sản phẩm thì tính tổng theo danh mục đó và add vào newListProductCategoryResult
                    if (newProductCategoryId.Count != 0)
                    {
                        var listCategoryResult = new ListCategoryResult();
                        var total = new_ListProductCategory.Where(x => newProductCategoryId.Contains((Guid)x.ProductCategoryId))
                            .Sum(y => y.Total);
                        listCategoryResult.Total = total;
                        listCategoryResult.ParentProductCategoryName = item.ParentProductCategoryName;
                        listCategoryResult.ParentProductCategoryId = item.ParentProductCategoryId;
                        newListProductCategoryResult.Add(listCategoryResult);
                    }
                });

                decimal total_money_search = 0;
                employeeList.ForEach(item =>
                {
                    total_money_search = total_money_search + (decimal)((item.DiscountType == true)
                                             ? (item.Amount * (1 - item.DiscountValue / 100))
                                             : (item.Amount - item.DiscountValue));
                });

                List<dynamic> lstResultEmployee = new List<dynamic>();
                newList.ForEach(item =>
                {
                    var sampleObject = new ExpandoObject() as IDictionary<string, Object>;
                    sampleObject.Add("employeeId", item.EmployeeId);
                    sampleObject.Add("employeeName", item.EmployeeName);
                    sampleObject.Add("avatarUrl", "");
                    sampleObject.Add("organizationName", item.OrganizationName);
                    sampleObject.Add("total", item.Total);
                    sampleObject.Add("totalSearch", total_money_search);
                    lstResultEmployee.Add(sampleObject);
                });

                List<dynamic> lstResult = new List<dynamic>();
                newListProductCategoryResult.ForEach(item =>
                {
                    var sampleObject = new ExpandoObject() as IDictionary<string, Object>;
                    sampleObject.Add("ProductCategoryId", item.ParentProductCategoryId);
                    sampleObject.Add("ProductCategoryName", item.ParentProductCategoryName);
                    sampleObject.Add("Total", item.Total);
                    decimal percent = 0;

                    percent = (item.Total / total_money_search) * 100;

                    percent = Math.Round(percent, 1);
                    sampleObject.Add("Percent", percent);
                    lstResult.Add(sampleObject);
                });

                #region Danh sách đơn hàng chờ xuất kho

                // Lấy đơn hàng có phiếu xuất kho ở trạng thái Chờ xuất
                var statusInventoryDeliveryVoucher = context.CategoryType.FirstOrDefault(f => f.CategoryTypeCode == "TPHX");
                var statusInventoryDeliveryVoucherCXL = context.Category.FirstOrDefault(f =>
                    f.CategoryCode == "CXK" && f.CategoryTypeId == statusInventoryDeliveryVoucher.CategoryTypeId);
                var listOrderId = context.InventoryDeliveryVoucher
                    .Where(i => i.StatusId == statusInventoryDeliveryVoucherCXL.CategoryId).Select(i => i.ObjectId)
                    .ToList();

                var lstOrderInventoryDelivery =
                    customerOrderAllStatus.Where(ci => listOrderId.Contains(ci.OrderId)).ToList();
                var lstOrderInventoryDeliveryEntityModel = new List<CustomerOrderEntityModel>();
                lstOrderInventoryDelivery.ForEach(item =>
                {
                    lstOrderInventoryDeliveryEntityModel.Add(new CustomerOrderEntityModel(item));
                });
                #endregion

                #region Danh sách đơn hàng chờ xuất hóa đơn

                //Lấy các trạng thái ở Đơn hàng bán
                var orderStatusView = allStatusOrder.Where(w =>
                   w.OrderStatusCode == "DLV").ToList();
                var orderStatusIdViewList = orderStatusView.Select(o => o.OrderStatusId).ToList();
                var lstOrderBill = customerOrderAllStatus.Where(c =>
                    c.StatusId != null && orderStatusIdViewList.Contains((Guid)c.StatusId)).ToList();
                // Lấy các hóa đơn ở trạng thái mới
                var categoryTypeBill =
                    context.CategoryType.FirstOrDefault(ct => ct.CategoryTypeCode == "BILL" && ct.Active == true);
                var statusNew = context.Category.FirstOrDefault(c =>
                    c.CategoryTypeId == categoryTypeBill.CategoryTypeId && c.Active == true && c.CategoryCode == "NEW");

                var billNotOrder = context.BillOfSale.Where(b => b.Active == true && b.StatusId != statusNew.CategoryId)
                    .Select(b => b.OrderId).ToList();
                lstOrderBill = lstOrderBill.Where(ob =>
                    billNotOrder == null || billNotOrder.Count == 0 || !billNotOrder.Contains(ob.OrderId)).ToList();
                var lstOrderBillEntityModel = new List<CustomerOrderEntityModel>();
                lstOrderBill.ForEach(item =>
                {
                    lstOrderBillEntityModel.Add(new CustomerOrderEntityModel(item));
                });
                #endregion

                #region Giá trị đơn hàng theo trạng thái

                List<StatusOrderModel> statustList = new List<StatusOrderModel>();
                customerOrderAllStatus.ForEach(item =>
                {
                    if (allStatusOrder != null)
                    {
                        var statusTemp = new StatusOrderModel();
                        statusTemp.StatusId = item.StatusId.Value;
                        statusTemp.StatusName =
                            allStatusOrder.FirstOrDefault(s => s.OrderStatusId == item.StatusId).Description;
                        statusTemp.Amount = item.Amount.Value;
                        statusTemp.DiscountType = item.DiscountType;
                        statusTemp.DiscountValue = item.DiscountValue;
                        statustList.Add(statusTemp);
                    }
                });

                var statusOrderList = (statustList.GroupBy(x => new { x.StatusId, x.StatusName }).Select(y => new
                {
                    Id = y.Key,
                    y.Key.StatusId,
                    y.Key.StatusName,
                    Total = y.Sum(s =>
                        (s.DiscountType == true) ? (s.Amount * (1 - s.DiscountValue / 100)) : (s.Amount - s.DiscountValue))
                }).OrderByDescending(x => x.Total)).ToList();

                List<dynamic> lstResultStatusOrder = new List<dynamic>();
                statusOrderList.ForEach(item =>
                {
                    var sampleObject = new ExpandoObject() as IDictionary<string, Object>;
                    sampleObject.Add("statusId", item.StatusId);
                    sampleObject.Add("statusName", item.StatusName);
                    sampleObject.Add("total", item.Total);
                    lstResultStatusOrder.Add(sampleObject);
                });

                #endregion

                #region Giá trị đơn hàng giữa các tháng

                List<MonthOrderModel> monthList = new List<MonthOrderModel>();
                var customerMonth = commonCustomerOrder.OrderBy(cm => cm.OrderDate).ToList();
                var montStart = parameter.OrderDateStart.Value.Month + 1;
                customerMonth.ForEach(item =>
                {
                    var monthTemp = new MonthOrderModel();
                    monthTemp.STT = montStart - item.OrderDate.Month;
                    monthTemp.MonthName = "Tháng " + item.OrderDate.Month + " Năm " + item.OrderDate.Year;
                    monthTemp.TimeNode = new DateTime(item.OrderDate.Year, item.OrderDate.Month, 1, 0, 0, 0, 0);
                    monthTemp.Amount = item.Amount.Value;
                    monthTemp.DiscountType = item.DiscountType;
                    monthTemp.DiscountValue = item.DiscountValue;
                    monthList.Add(monthTemp);
                });
                var monthOrderList = (monthList.GroupBy(x => new { x.MonthName, x.TimeNode }).Select(y => new
                {
                    Id = y.Key,
                    y.Key.MonthName,
                    y.Key.TimeNode,
                    Total = y.Sum(s =>
                        (s.DiscountType == true) ? (s.Amount * (1 - s.DiscountValue / 100)) : (s.Amount - s.DiscountValue))
                })).OrderBy(x => x.TimeNode).ToList();

                List<dynamic> lstResultMonthOrder = new List<dynamic>();
                monthOrderList.ForEach(item =>
                {
                    var sampleObject = new ExpandoObject() as IDictionary<string, Object>;
                    sampleObject.Add("monthName", item.MonthName);
                    sampleObject.Add("total", item.Total);
                    lstResultMonthOrder.Add(sampleObject);
                });

                #endregion

                return new GetEmployeeListByOrganizationIdResult()
                {
                    employeeList = lstResultEmployee,
                    lstOrderBill = lstOrderBillEntityModel,
                    lstOrderInventoryDelivery = lstOrderInventoryDeliveryEntityModel,
                    lstResult = lstResult,
                    statusOrderList = lstResultStatusOrder,
                    monthOrderList = lstResultMonthOrder,
                    levelMaxProductCategory = levelMaxProductCategory,
                    MessageCode = "Success",
                    StatusCode = HttpStatusCode.OK
                };
            }
            catch(Exception e)
            {
                return new GetEmployeeListByOrganizationIdResult()
                {
                    MessageCode = e.Message,
                    StatusCode = HttpStatusCode.ExpectationFailed
                };
            }      
        }

        public GetMonthsListResult GetMonthsList(GetMonthsListParameter parameter)
        {
            try
            {
                var user = context.User.FirstOrDefault(x => x.UserId == parameter.UserId);
                var employee = context.Employee.FirstOrDefault(u => u.EmployeeId == user.EmployeeId);

                var organizationId = parameter.OrganizationId;
                List<Guid?> listGetAllChild = new List<Guid?>();
                listGetAllChild.Add(organizationId);
                listGetAllChild = getOrganizationChildrenId(organizationId, listGetAllChild);
                var commonOrganization = context.Organization
                    .Where(x => listGetAllChild.Contains(x.OrganizationId)).ToList();
                var organizationIdList = commonOrganization.Select(x => x.OrganizationId).ToList();
                if (organizationId != null)
                {
                    listGetAllChild.Add(organizationId);
                    listGetAllChild = getOrganizationChildrenId(organizationId, listGetAllChild);
                }
                var commonEmployee = context.Employee
                    .Where(x => x.OrganizationId != null && organizationIdList.Contains((Guid)x.OrganizationId)).ToList();
                var employeeIdList = commonEmployee.Select(x => x.EmployeeId).ToList();
                var allStatusOrder = context.OrderStatus.Where(c => c.Active == true).ToList();

                var dateFrom = parameter.OrderDateTo.Value.Date.AddMonths(-parameter.MonthAdd + 1);
                dateFrom = new DateTime(dateFrom.Year, dateFrom.Month, 1, 0, 0, 0, 0);
                var customerOrderAllStatus = context.CustomerOrder.Where(c =>
                    c.StatusId != null
                    && (parameter.OrderDateTo != null) && (dateFrom.Date <= c.OrderDate.Date && c.OrderDate.Date <= parameter.OrderDateTo.Value.Date)).ToList();

                customerOrderAllStatus = employee.IsManager ? customerOrderAllStatus.Where(c => employeeIdList.Contains(c.Seller.Value)).ToList() : customerOrderAllStatus.Where(c => c.Seller.Value == employee.EmployeeId).ToList();

                var listOrderCode = context.SystemParameter.FirstOrDefault(x => x.SystemKey == "OrderStatus")
                    .SystemValueString.Split(';').ToList();
                var commonOrderStatus = allStatusOrder.Where(x => listOrderCode.Contains(x.OrderStatusCode)).ToList();
                var orderStatusIdList = commonOrderStatus.Select(o => o.OrderStatusId).ToList();
                var commonCustomerOrder = customerOrderAllStatus.Where(c =>
                    c.StatusId != null && orderStatusIdList.Contains((Guid)c.StatusId)
                ).ToList();





                #region Giá trị đơn hàng giữa các tháng

                List<MonthOrderModel> monthList = new List<MonthOrderModel>();
                var customerMonth = commonCustomerOrder.OrderBy(cm => cm.OrderDate).ToList();
                if (parameter.OrderDateTo != null)
                {
                    var montStart = dateFrom.Month;
                    customerMonth.ForEach(item =>
                    {
                        var monthTemp = new MonthOrderModel();
                        monthTemp.STT = montStart - item.OrderDate.Month;
                        monthTemp.MonthName = item.OrderDate.Month.ToString("d2") + "/" + item.OrderDate.Year;
                        monthTemp.TimeNode = new DateTime(item.OrderDate.Year, item.OrderDate.Month, 1, 0, 0, 0, 0);
                        monthTemp.Amount = item.Amount.Value;
                        monthTemp.DiscountType = item.DiscountType;
                        monthTemp.DiscountValue = item.DiscountValue;
                        monthList.Add(monthTemp);
                    });

                }

                var startDate = parameter.OrderDateTo.Value.Date.AddMonths(-parameter.MonthAdd + 1);
                var endDate = parameter.OrderDateTo.Value;

                while (startDate <= endDate)
                {
                    var monthName = startDate.Month.ToString("d2") + "/" + startDate.Year;
                    var count = 0;
                    monthList.ForEach(x =>
                    {
                        if (x.MonthName == monthName)
                        {
                            count++;
                        }
                    });
                    if (count == 0)
                    {
                        var monthTemp = new MonthOrderModel();
                        monthTemp.STT = -1;
                        monthTemp.MonthName = monthName;
                        monthTemp.TimeNode = new DateTime(startDate.Year, startDate.Month, 1, 0, 0, 0, 0);
                        monthTemp.Amount = 0;
                        monthTemp.DiscountType = true;
                        monthTemp.DiscountValue = 0;
                        monthList.Add(monthTemp);
                    }

                    startDate = startDate.AddMonths(1);
                }

                var monthOrderList = (monthList.GroupBy(x => new { x.MonthName, x.TimeNode }).Select(y => new
                {
                    Id = y.Key,
                    y.Key.MonthName,
                    y.Key.TimeNode,
                    Total = y.Sum(s =>
                        (s.DiscountType == true) ? (s.Amount * (1 - s.DiscountValue / 100)) : (s.Amount - s.DiscountValue))
                })).OrderBy(x => x.TimeNode).ToList();

                List<dynamic> lstResultMonthOrder = new List<dynamic>();
                monthOrderList.ForEach(item =>
                {
                    var sampleObject = new ExpandoObject() as IDictionary<string, Object>;
                    sampleObject.Add("monthName", item.MonthName);
                    sampleObject.Add("total", item.Total);
                    lstResultMonthOrder.Add(sampleObject);
                });

                #endregion


                return new GetMonthsListResult()
                {
                    monthOrderList = lstResultMonthOrder,
                    MessageCode="Success",
                    StatusCode=HttpStatusCode.OK
                };
            }
            catch(Exception e)
            {
                return new GetMonthsListResult()
                {                
                    MessageCode = e.Message,
                    StatusCode = HttpStatusCode.ExpectationFailed
                };
            } 
        }

        private List<Guid?> getOrganizationChildrenId(List<Organization> organizationList, Guid? id, List<Guid?> list)
        {
            var organizations = organizationList.Where(o => o.ParentId == id).ToList();
            organizations.ForEach(item =>
            {
                list.Add(item.OrganizationId);
                getOrganizationChildrenId(organizationList, item.OrganizationId, list);
            });

            return list;
        }

        public GetProductCategoryGroupByManagerResult GetProductCategoryGroupByManager(GetProductCategoryGroupByManagerParameter parameter)
        {
            try
            {
                var commonProductCategory = context.ProductCategory.ToList();

                var organizationId = parameter.OrganizationId;
                List<Guid?> listGetAllChild = new List<Guid?>();
                if (organizationId != null)
                {
                    listGetAllChild.Add(organizationId);
                    listGetAllChild = getOrganizationChildrenId(organizationId, listGetAllChild);
                }

                var commonOrganization = context.Organization.Where(c => listGetAllChild.Contains(c.OrganizationId)).ToList();
                var organizationIdList = commonOrganization.Select(c => c.OrganizationId).ToList();
                var commonEmployee = context.Employee.Where(x => x.OrganizationId != null && organizationIdList.Contains((Guid)x.OrganizationId)).ToList();
                var employeeIdList = commonEmployee.Select(x => x.EmployeeId).ToList();

                var commonOrderStatus = context.OrderStatus.Where(w => w.OrderStatusCode == "IP" || w.OrderStatusCode == "DLV" || w.OrderStatusCode == "PD" || w.OrderStatusCode == "COMP").ToList();
                var orderStatusIdList = commonOrderStatus.Select(o => o.OrderStatusId).ToList();
                var commonCustomerOrder = context.CustomerOrder.Where(c => c.StatusId != null && orderStatusIdList.Contains((Guid)c.StatusId)
                                              && employeeIdList.Contains((Guid)c.Seller)
                                              && (parameter.OrderDateStart == null || parameter.OrderDateStart == DateTime.MinValue || parameter.OrderDateStart.Value.Date <= c.OrderDate.Date)
                                              && (parameter.OrderDateEnd == null || parameter.OrderDateEnd == DateTime.MinValue || parameter.OrderDateEnd.Value.Date >= c.OrderDate.Date)).ToList();
                var custormerOrderIdList = commonCustomerOrder.Select(c => c.OrderId).ToList();
                var commonCustomerOrderDetail = context.CustomerOrderDetail.Where(c => custormerOrderIdList.Contains(c.OrderId)).ToList();
                var productIdList = commonCustomerOrderDetail.Select(c => c.ProductId).ToList();
                var commonProduct = context.Product.Where(c => productIdList.Contains(c.ProductId)).ToList();

                var listProductCaterogyFirstParent = (from productcategory in commonProductCategory
                                                      where productcategory.ProductCategoryLevel == parameter.ProductCategoryLevel
                                                      select new
                                                      {
                                                          productcategory.ProductCategoryId,
                                                          productcategory.ProductCategoryName
                                                      }).ToList();
                List<ListCategoryId> newListProductCategory = new List<ListCategoryId>();
                listProductCaterogyFirstParent.ForEach(item =>
                {
                    List<Guid> newListProductCategoryIdChildren = new List<Guid>();
                    newListProductCategoryIdChildren.Add(item.ProductCategoryId);
                    newListProductCategoryIdChildren = getProductCategoryChildrenId(item.ProductCategoryId, newListProductCategoryIdChildren, commonProductCategory);

                    var listCategoryId = new ListCategoryId();
                    listCategoryId.ParentProductCategoryId = item.ProductCategoryId;
                    listCategoryId.ParentProductCategoryName = item.ProductCategoryName;
                    listCategoryId.ListChildrent = newListProductCategoryIdChildren;
                    newListProductCategory.Add(listCategoryId);
                });

                List<ListCategoryResult> newListProductCategoryResult = new List<ListCategoryResult>();
                var productCategoryIdList = commonProduct.Select(x => x.ProductCategoryId).ToList();
                var productCategoryList = commonProductCategory.Where(c => productCategoryIdList.Contains(c.ProductCategoryId)).ToList();

                List<ProductCategoryModel> listProductCategoryModel = new List<ProductCategoryModel>();
                commonCustomerOrderDetail.ForEach(item =>
                {
                    #region Add by Dung: viết lại công thức tính tổng tiền theo từng sản phẩm, mỗi sản phẩm phải trừ tiền chiết khấu của tổng đơn hàng
                    var customerOrder = commonCustomerOrder.FirstOrDefault(f => f.OrderId == item.OrderId);
                    decimal? discountPerOrder = 0;
                    if (customerOrder.DiscountType == true)
                    {
                        //chiết khấu phần trăm
                        discountPerOrder = customerOrder?.DiscountValue ?? 0;
                    }
                    else
                    {
                        //chiết khấu theo số tiền
                        discountPerOrder = (customerOrder.DiscountValue / customerOrder.Amount) * 100;
                    }
                    #endregion

                    var product = commonProduct.FirstOrDefault(x => x.ProductId == item.ProductId);

                    if (product != null)
                    {
                        var category = commonProductCategory.FirstOrDefault(x => x.ProductCategoryId == product.ProductCategoryId);

                        var productCategoryModel = new ProductCategoryModel();
                        productCategoryModel.ProductCategoryId = product.ProductCategoryId;
                        productCategoryModel.ProductCategoryName = category.ProductCategoryName;
                        //productCategoryModel.Total = SumAmount(item.Quantity, item.UnitPrice, item.Vat, item.DiscountValue, item.DiscountType, item.ExchangeRate);
                        productCategoryModel.Total = ReCalculatorTotal(item.Vat.Value, item.DiscountType.Value, item.DiscountValue.Value, item.Quantity.Value, item.UnitPrice.Value, item.ExchangeRate.Value, discountPerOrder);
                        listProductCategoryModel.Add(productCategoryModel);
                    }
                });


                // Gom nhóm theo danh mục và tính tổng hóa đơn theo từng danh mục đó
                var new_ListProductCategory = listProductCategoryModel.GroupBy(x => new { x.ProductCategoryId, x.ProductCategoryName }).Select(y => new
                {
                    Id = y.Key,
                    y.Key.ProductCategoryId,
                    y.Key.ProductCategoryName,
                    Total = y.Sum(s => s.Total)
                }).OrderByDescending(x => x.Total).ToList();

                // Tỉnh tổng hóa đơn theo danh mục gốc(danh mục cấp 0)
                newListProductCategory.ForEach(item =>
                {
                    var newProductCategoryId = productCategoryList.Where(c => item.ListChildrent.Contains(c.ProductCategoryId)).Select(c => c.ProductCategoryId).ToList();

                    // Nếu danh mục nào bán được sản phẩm thì tính tổng theo danh mục đó và add vào newListProductCategoryResult
                    if (newProductCategoryId.Count != 0)
                    {
                        var listCategoryResult = new ListCategoryResult();
                        var total = new_ListProductCategory.Where(x => newProductCategoryId.Contains((Guid)x.ProductCategoryId)).Sum(y => y.Total);
                        listCategoryResult.Total = total;
                        listCategoryResult.ParentProductCategoryName = item.ParentProductCategoryName;
                        listCategoryResult.ParentProductCategoryId = item.ParentProductCategoryId;
                        newListProductCategoryResult.Add(listCategoryResult);
                    }
                });

                //Tinh tong gia tri cua cac Nhom san pham
                decimal total_product = 0;
                newListProductCategoryResult.ForEach(item =>
                {
                    total_product += item.Total;
                });
                List<dynamic> lstResult = new List<dynamic>();
                newListProductCategoryResult.ForEach(item =>
                {
                    var sampleObject = new ExpandoObject() as IDictionary<string, Object>;
                    sampleObject.Add("ProductCategoryId", item.ParentProductCategoryId);
                    sampleObject.Add("ProductCategoryName", item.ParentProductCategoryName);
                    sampleObject.Add("Total", item.Total);
                    decimal percent = 0;
                    percent = (item.Total / total_product) * 100;
                    percent = Math.Round(percent, 1);
                    sampleObject.Add("Percent", percent);
                    lstResult.Add(sampleObject);
                });

                return new GetProductCategoryGroupByManagerResult()
                {
                    lstResult = lstResult,
                    MessageCode="Success",
                    StatusCode=HttpStatusCode.OK
                };
            }
            catch(Exception e)
            {
                return new GetProductCategoryGroupByManagerResult()
                {
                    MessageCode = e.Message,
                    StatusCode = HttpStatusCode.ExpectationFailed
                };
            }         
        }

        public GetMasterDataOrderSearchResult GetMasterDataOrderSearch(GetMasterDataOrderSearchParameter parameter)
        {
            try 
            {
                //nếu là phiếu yêu cầu 
                if (parameter.IsOrderAction == false && parameter.IsOrderProcess == false)
                {
                    // list gói dịch vụ
                    var listPacketService = context.ServicePacket.Select(x => new ServicePacketEntityModel
                    {
                        Name = x.Name,
                        Id = x.Id
                    }).ToList();

                    //list người tạo
                    var listCreator = context.Employee.Select(x => new EmployeeEntityModel
                    {
                        EmployeeId = x.EmployeeId,
                        EmployeeCodeName = x.EmployeeCode + " - " + x.EmployeeName,
                    }).ToList();

                    //list trạng thái của phiếu yêu cầu
                    var listStatus = GeneralList.GetTrangThais("CustomerOrder").ToList();

                    return new GetMasterDataOrderSearchResult()
                    {
                        ListStatus = listStatus,
                        ListPacketService = listPacketService,
                        ListCreator = listCreator,
                        StatusCode = HttpStatusCode.OK,
                        MessageCode = "Success",

                    };
                }
                //Nếu là phiếu hỗ trợ
                else if (parameter.IsOrderAction == true && parameter.IsOrderProcess == false)
                {
                    //Lấy list khách hàng của phiếu
                    var listCusId = context.CustomerOrder.Where(x => x.IsOrderAction == true).Select(x => x.CustomerId).ToList();
                    var listCus = context.Customer.Where(x => listCusId.Contains(x.CustomerId)).Select(x => new CustomerEntityModel
                    {
                        CustomerId = x.CustomerId,
                        CustomerCode = x.CustomerCode,
                        CustomerName = x.CustomerName,
                        CustomerCodeName = x.CustomerCode + " - " + x.CustomerName
                    }).ToList();

                    //list trạng thái của phiếu hỗ trợ
                    var listStatus = GeneralList.GetTrangThais("CustomerOrderAction").ToList();

                    return new GetMasterDataOrderSearchResult()
                    {
                        ListCus = listCus,
                        ListStatus = listStatus,
                        StatusCode = HttpStatusCode.OK,
                        MessageCode = "Success",

                    };
                }

                //Nếu là quy trình đặt dịch vụ
                if (parameter.IsOrderProcess == true)
                {
                    var listCusId = context.OrderProcess.Select(x => x.CustomerId).Distinct().ToList();
                    var listCus = context.Customer.Where(x => listCusId.Contains(x.CustomerId)).Select(x => new CustomerEntityModel {
                        CustomerId = x.CustomerId,
                        CustomerCodeName = x.CustomerCode + "-" + x.CustomerName,
                    }).ToList();

                    var listAllPacket = context.ServicePacket.Select(x => new ServicePacketEntityModel
                    {
                        Id = x.Id,
                        Code = x.Name
                    }).ToList();

                    var listAllProductCategory = context.ProductCategory.Select(x => new ProductCategoryEntityModel
                    {
                        ProductCategoryId = x.ProductCategoryId,
                        ProductCategoryName = x.ProductCategoryName,
                        ProductCategoryCode = x.ProductCategoryCode,
                    }).ToList();

                    var listStatus = GeneralList.GetTrangThais("OrderProcess").ToList();

                    return new GetMasterDataOrderSearchResult()
                    {
                        ListCus = listCus,
                        ListStatus = listStatus,
                        StatusCode = HttpStatusCode.OK,
                        MessageCode = "Success",
                        ListPacketService = listAllPacket,
                        ListAllProductCategory = listAllProductCategory,
                    };
                }

                return new GetMasterDataOrderSearchResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = "Lỗi dữ liệu",
                };
            }
            catch (Exception e)
            {
                return new GetMasterDataOrderSearchResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public SearchOrderResult SearchOrder(SearchOrderParameter parameter)
        {
            try
            {
                var listAllUser = context.User.ToList();
                var listAllEmp = context.Employee.ToList();
                var listAllManagerMappingPack = context.ManagerPacketService.ToList();
                var userLogin = listAllUser.FirstOrDefault(x => x.UserId == parameter.UserId);
                var empLogin = listAllEmp.FirstOrDefault(x => x.EmployeeId == userLogin.EmployeeId);

                var listOrder = new List<CustomerOrderEntityModel>();
                var listOrderProcess = new List<OrderProcessEntityModel>();

                var user = listAllUser.FirstOrDefault(x => x.UserId == parameter.UserId && x.Active == true);
                if (user == null)
                {
                    return new SearchOrderResult()
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = "User không có quyền truy xuất dữ liệu trong hệ thống"
                    };
                }
                if (user.EmployeeId == null || user.EmployeeId == Guid.Empty)
                {
                    return new SearchOrderResult()
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = "Lỗi dữ liệu"
                    };
                }

                //phiếu yêu cầu
                if(parameter.IsOrderAction == false && parameter.IsOrderProcess == false)
                {
                    var listOrderType = GeneralList.GetTrangThais("OrderType").ToList();

                    //Lọc theo ngày và trạng thái
                    listOrder = context.CustomerOrder.Where(x =>
                        x.IsOrderAction == parameter.IsOrderAction &&
                        (parameter.FromDate == null || parameter.FromDate.Value.Date <= x.CreatedDate.Date) &&
                        (parameter.ToDate == null || parameter.ToDate.Value.Date >= x.CreatedDate.Date) &&
                        (parameter.ListStatusSelected == null || parameter.ListStatusSelected.Count == 0 || parameter.ListStatusSelected.Contains(x.StatusOrder.Value))
                    ).Select(x => new CustomerOrderEntityModel
                    {
                        OrderId = x.OrderId,
                        OrderCode = x.OrderCode,
                        CustomerId = x.CustomerId,
                        StatusOrder = x.StatusOrder,
                        CreatedDate = x.CreatedDate,
                        CreatedById = x.CreatedById,
                        DiscountType = x.DiscountType,
                        DiscountValue = x.DiscountValue,
                        ServicePacketId = x.ServicePacketId,
                        Vat = x.Vat,
                        OrderTypeName = listOrderType.FirstOrDefault(y => y.Value == x.OrderType).Name,
                    }).ToList();

                    //list trạng thái của phiếu yêu cầu
                    var listStatus = GeneralList.GetTrangThais("CustomerOrder").ToList();

                    //List khách hàng
                    var listAllCusId = listOrder.Select(x => x.CustomerId).ToList();

                    //List người tạo
                    var listAllUserId = listOrder.Select(x => x.CreatedById).ToList();
                    var listAllEmpId = listAllUser.Where(x => listAllUserId.Contains(x.UserId)).Select(x => x.EmployeeId).ToList();

                    var listAllCus = context.Customer.Where(x => listAllCusId.Contains(x.CustomerId)).ToList();
                    var listAllCreator = context.Employee.Where(x => listAllEmpId.Contains(x.EmployeeId)).ToList();

                    //list gói dịch vụ mapp của tất cả phiếu
                    var listOrderId = listOrder.Select(x => x.OrderId).ToList();

                    var listOrderDetail = context.CustomerOrderDetail.Where(x => listOrderId.Contains(x.OrderId)).ToList();

                    var listPacketId = listOrderDetail.Select(x => x.PacketServiceId).ToList();
                    var listPacketService = context.ServicePacket.Where(x => listPacketId.Any(y => y == x.Id)).ToList();

                    //Lấy categoryId của bước tạo yêu cầu + xác nhận thanh toán
                    var cateTypeIdOfStep = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == ProductConsts.CategoryTypeCodeActionStep).CategoryTypeId;
                    var createOrderStepId = context.Category.FirstOrDefault(x => x.CategoryTypeId == cateTypeIdOfStep && x.CategoryCode == ProductConsts.CategoryCodeCreateStep).CategoryId;
                    var cofirmCostStepId = context.Category.FirstOrDefault(x => x.CategoryTypeId == cateTypeIdOfStep && x.CategoryCode == ProductConsts.CategoryCodeConfirmStep).CategoryId;


                    //Lấy list các bước tạo  phiếu yêu cầu và xác nhận thanh toán của tất cả phiếu 
                    var listAllStepPack = context.PermissionConfiguration
                            .Where(x => x.CategoryId == createOrderStepId || x.CategoryId  == cofirmCostStepId).ToList();

                    var listAllStepPackId = listAllStepPack.Select(x => x.Id).ToList();
                    //Lấy list all emp mapping bước 
                    var listAllEmpPermissionConfigure = context.EmployeeMappingPermissionConfiguration.Where(x => listAllStepPackId.Contains(x.PermissionConfigurationId)).ToList();


                    //Lấy list dịch vụ phát sinh
                    var listCustomerOrderDetailExten = context.CustomerOrderDetailExten.Where(x => listOrderId.Contains(x.OrderId)).ToList();

                    listOrder.ForEach(item => {
                        item.CustomerName = listAllCus.FirstOrDefault(x => x.CustomerId == item.CustomerId)?.CustomerName;

                        var empCreator = listAllUser.FirstOrDefault(y => y.UserId == item.CreatedById);
                        if (empCreator != null)
                        {
                            var emp = listAllCreator.FirstOrDefault(x => x.EmployeeId == empCreator.EmployeeId);
                            item.Creatorname = emp == null ? item.CustomerName : emp?.EmployeeCode + " - " + emp?.EmployeeName;
                            item.CreatorId = emp?.EmployeeId;
                        }

                        //Lấy các  gói dịch vụ của phiếu
                        item.ListPacketServiceName = "";
                        item.ListPacketServiceId = new List<Guid>();
                        var listPacketServiceId = listOrderDetail.Where(x => x.OrderId == item.OrderId).Select(x => x.PacketServiceId).ToList();
                        var listPack = listPacketService.Where(x => listPacketServiceId.Contains(x.Id)).ToList();
                        listPack.ForEach(pack => {
                            item.ListPacketServiceName = item.ListPacketServiceName + ", " + pack.Name;
                            item.ListPacketServiceId.Add(pack.Id);
                        });
                        item.OrderStatusName = listStatus.FirstOrDefault(x => x.Value == item.StatusOrder).Name;

                        #region
                        //Tổng tiền trước thuế
                        decimal? totalCostBeforeTax = 0;
                        //Tổng tiền thuế
                        decimal? totalCostTax = 0;
                        //Tổng tiền chiết khấu
                        decimal? totalDiscountCost = 0;
                        // Tổng tiền phải trả
                        decimal? totalCostPay = 0;

                        var listOrderDetailCurrent = listOrderDetail.Where(x => x.OrderId == item.OrderId).ToList();

                        listOrderDetailCurrent.ForEach(currentDetail => {
                            totalCostBeforeTax += currentDetail.Quantity.Value * currentDetail.PriceInitial.Value;
                            totalCostTax += currentDetail.Quantity * currentDetail.PriceInitial / 100 * ((currentDetail.Vat == -1 || currentDetail.Vat == 0) ? 0 : currentDetail.Vat);
                        });

                        var listOderDetailExtenCurrent = listCustomerOrderDetailExten.Where(x => x.OrderId == item.OrderId).ToList();
                        //Nếu có phát sinh
                        if (listOderDetailExtenCurrent.Count > 0 && item.StatusOrder != 1)
                        {
                            listOderDetailExtenCurrent.ForEach(extenCur => {
                                if (extenCur.Price != null && extenCur.Status == 2)
                                {
                                    totalCostBeforeTax += extenCur.Quantity * extenCur.Price;
                                    totalCostTax += extenCur.Quantity * extenCur.Price / 100 * ((item.Vat == -1 || item.Vat == 0) ? 0 : item.Vat);
                                }
                            });
                        }

                        //Chiết khấu theo %

                        // false: theo %, true: Theo số tiền

                        if (!item.DiscountType.Value)
                        {
                            totalDiscountCost = totalCostBeforeTax * item.DiscountValue / 100;
                        }
                        //Chiết khấu theo số tiền
                        else if (item.DiscountType.Value)
                        {
                            totalDiscountCost = item.DiscountValue;
                        }
                        totalCostPay = totalCostBeforeTax + totalCostTax - totalDiscountCost;

                        item.Amount = totalCostPay;
                        #endregion

                        #region Lấy list người tạo + người phụ trách bước tạo phiếu + xác nhận phiếu
                        item.ListPersonInCharge = new List<Guid>();
                        item.ListPersonInCharge.Add(empLogin.EmployeeId);

                        var packetIdOfOrder = listOrderDetail.FirstOrDefault(x => x.OrderId == item.OrderId)?.PacketServiceId;
                        if(packetIdOfOrder != null)
                        {
                            //các nhân viên được truy cập phiêu yêu cầu
                            var listStepIdOfOrder = listAllStepPack.Where(x => x.ServicePacketId == packetIdOfOrder).Select(x => x.Id).ToList();
                            var listEmpId = listAllEmpPermissionConfigure.Where(x => listStepIdOfOrder.Contains(x.PermissionConfigurationId)).Select(x => x.EmployeeId).Distinct().ToList();
                            item.ListPersonInCharge.AddRange(listEmpId);
                        }
                        //Lấy ra tất cả người phụ trách gói
                        item.ListPersonInCharge.AddRange(listAllManagerMappingPack.Where(x => listPacketServiceId.Contains(x.PackId)).Select(x => x.EmployeeId).ToList());
                        item.ListPersonInCharge = item.ListPersonInCharge.Distinct().ToList();
                        #endregion
                    });

                    //lọc theo gói và người tạo và người được truy cập
                    listOrder = listOrder.Where(x =>
                      (parameter.ListCreatorIdSelected == null || parameter.ListCreatorIdSelected.Count == 0 || parameter.ListCreatorIdSelected.Contains(x.CreatorId.Value) &&
                      (parameter.ListPacketIdSelected == null || parameter.ListPacketIdSelected.Count == 0 || parameter.ListPacketIdSelected.Intersect(x.ListPacketServiceId).Any()) &&
                      (x.ListPersonInCharge == null || x.ListPersonInCharge.Count == 0 || x.ListPersonInCharge.Contains(empLogin.EmployeeId))
                      )).ToList();
                }
               //Phiếu hỗ trợ
                else if (parameter.IsOrderAction == true && parameter.IsOrderProcess == false)
                {
                    //Lọc theo ngày và trạng thái
                    listOrder = context.CustomerOrder.Where(x =>
                        x.IsOrderAction == parameter.IsOrderAction &&
                        (parameter.FromDate == null || parameter.FromDate.Value.Date <= x.CreatedDate.Date) &&
                        (parameter.ToDate == null || parameter.ToDate.Value.Date >= x.CreatedDate.Date) &&
                        (parameter.ListStatusSelected == null || parameter.ListStatusSelected.Count == 0 || parameter.ListStatusSelected.Contains(x.StatusOrder.Value)) &&
                        (parameter.ListCusId == null || parameter.ListCusId.Count == 0 || parameter.ListCusId.Contains(x.CustomerId.Value))
                        ).Select(x => new CustomerOrderEntityModel
                        {
                            OrderId = x.OrderId,
                            OrderCode = x.OrderCode,
                            CustomerId = x.CustomerId,
                            StatusOrder = x.StatusOrder,
                            CreatedDate = x.CreatedDate,
                            CreatedById = x.CreatedById,
                            DiscountType = x.DiscountType,
                            DiscountValue = x.DiscountValue,
                            Vat = x.Vat,
                            ObjectId = x.ObjectId,
                            ServicePacketId = x.ServicePacketId,
                        }).ToList();

                    var listPacketServiceId = listOrder.Select(x => x.ServicePacketId).ToList();

                    var listOrderId = listOrder.Select(x => x.OrderId).ToList();
                    var listOrderDetail = context.CustomerOrderDetail.Where(x => listOrderId.Contains(x.OrderId)).ToList();


                    //List khách hàng
                    var listAllCusId = listOrder.Select(x => x.CustomerId).ToList();
                    var listAllCus = context.Customer.Where(x => listAllCusId.Contains(x.CustomerId)).ToList();

                    //List trạng thái của phiếu hõ trợ
                    var listStatus = GeneralList.GetTrangThais("CustomerOrderAction").ToList();

                    //list phiếu yêu cầu
                    var listCustomerOrderId = listOrder.Select(x => x.ObjectId).ToList();
                    var listCustomerOrder = context.CustomerOrder.Where(x => listCustomerOrderId.Contains(x.OrderId) && x.IsOrderAction == false).ToList();


                    //Lấy categoryId của bước tạo yêu cầu + xác nhận thanh toán
                    var cateTypeIdOfStep = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == ProductConsts.CategoryTypeCodeActionStep).CategoryTypeId;
                    var createOrderActionStepId = context.Category.FirstOrDefault(x => x.CategoryTypeId == cateTypeIdOfStep && x.CategoryCode == ProductConsts.CategoryCodeCforderStep).CategoryId;
                    var reportStepId = context.Category.FirstOrDefault(x => x.CategoryTypeId == cateTypeIdOfStep && x.CategoryCode == ProductConsts.CategoryCodeSupportStep).CategoryId;


                    //Lấy list các bước tạo phiếu hỗ trợ và báo cáo của tất cả phiếu 
                    var listAllStepPack = context.PermissionConfiguration
                            .Where(x => x.CategoryId == createOrderActionStepId || x.CategoryId == reportStepId).ToList();

                    var listAllStepPackId = listAllStepPack.Select(x => x.Id).ToList();
                    //Lấy list all emp mapping bước 
                    var listAllEmpPermissionConfigure = context.EmployeeMappingPermissionConfiguration.Where(x => listAllStepPackId.Contains(x.PermissionConfigurationId)).ToList();

                    listOrder.ForEach(item => {
                        item.CustomerName = listAllCus.FirstOrDefault(x => x.CustomerId == item.CustomerId)?.CustomerName;
                        item.OrderStatusName = listStatus.FirstOrDefault(x => x.Value == item.StatusOrder).Name;
                        item.OrderRequireCode = listCustomerOrder.FirstOrDefault(x => x.OrderId == item.ObjectId)?.OrderCode;


                        #region Lấy list người tạo + người phụ trách bước tạo phiếu + xác nhận phiếu
                        item.ListPersonInCharge = new List<Guid>();
                        item.ListPersonInCharge.Add(empLogin.EmployeeId);

                        var packetIdOfOrder = listOrderDetail.FirstOrDefault(x => x.OrderId == item.OrderId)?.PacketServiceId;
                        if (packetIdOfOrder != null)
                        {
                            //các nhân viên được truy cập phiêu yêu cầu
                            var listStepIdOfOrder = listAllStepPack.Where(x => x.ServicePacketId == packetIdOfOrder).Select(x => x.Id).ToList();
                            var listEmpId = listAllEmpPermissionConfigure.Where(x => listStepIdOfOrder.Contains(x.PermissionConfigurationId)).Select(x => x.EmployeeId).Distinct().ToList();
                            item.ListPersonInCharge.AddRange(listEmpId);
                        }
                        //Lấy ra tất cả người phụ trách gói
                        item.ListPersonInCharge.AddRange(listAllManagerMappingPack.Where(x => listPacketServiceId.Contains(x.PackId)).Select(x => x.EmployeeId).ToList());
                        item.ListPersonInCharge = item.ListPersonInCharge.Distinct().ToList();
                        #endregion
                    });

                    //lọc theo gói và người tạo và người được truy cập
                    listOrder = listOrder.Where(x =>
                      x.ListPersonInCharge == null || x.ListPersonInCharge.Count == 0 || x.ListPersonInCharge.Contains(empLogin.EmployeeId)).ToList();
                }

                //Search cho quy trình đặt dịch vụ
                if(parameter.IsOrderProcess == true)
                {
                    //Lọc theo ngày và trạng thái
                    listOrderProcess = context.OrderProcess.Where(x =>
                        (parameter.FromDate == null || parameter.FromDate.Value.Date <= x.CreatedDate.Date) &&
                        (parameter.ToDate == null || parameter.ToDate.Value.Date >= x.CreatedDate.Date) &&
                        (parameter.ListStatusSelected == null || parameter.ListStatusSelected.Count == 0 || parameter.ListStatusSelected.Contains(x.Status.Value)) &&
                        (parameter.ListCusId == null || parameter.ListCusId.Count == 0 || parameter.ListCusId.Contains(x.CustomerId.Value)) &&
                        (parameter.ListPacketIdSelected == null || parameter.ListPacketIdSelected.Count == 0 || parameter.ListPacketIdSelected.Contains(x.ServicePacketId.Value)) &&
                        (parameter.ListProductCateSelected == null || parameter.ListProductCateSelected.Count == 0 || parameter.ListProductCateSelected.Contains(x.ProductCategoryId.Value))
                        ).Select(x => new OrderProcessEntityModel
                        {
                            Id = x.Id,
                            ServicePacketId = x.ServicePacketId,
                            OrderProcessCode = x.OrderProcessCode,
                            ProductCategoryId = x.ProductCategoryId,
                            CustomerId = x.CustomerId,
                            CreatedById = x.CreatedById,
                            CreatedDate = x.CreatedDate,
                            Status = x.Status,
                        }).ToList();

                    var listPacketServiceId = listOrder.Select(x => x.ServicePacketId).ToList();
                    //List khách hàng
                    var listAllCusId = listOrderProcess.Select(x => x.CustomerId).ToList();
                    var listAllCus = context.Customer.Where(x => listAllCusId.Contains(x.CustomerId)).ToList();

                    //List trạng thái của quy trình đặt dịch vụ
                    var listStatus = GeneralList.GetTrangThais("OrderProcess").ToList();

                    var listAllPacket = context.ServicePacket.ToList();
                    var listAllProductCategory = context.ProductCategory.ToList();


                    //Lấy categoryId của Xác nhận hoàn thành hỗ trợ dịch vụ + các bước k mặc định
                    var listCateFixed = new List<string>() { 
                        ProductConsts.CategoryCodeCreateStep,
                        ProductConsts.CategoryCodeConfirmStep,
                        ProductConsts.CategoryCodeCforderStep,
                        ProductConsts.CategoryCodeSupportStep,
                        ProductConsts.CategoryCodeDoneStep
                        };
                    var cateTypeIdOfStep = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == ProductConsts.CategoryTypeCodeActionStep).CategoryTypeId;
                    var listCategoryInOrderProcess = context.Category.Where(x => x.CategoryTypeId == cateTypeIdOfStep && 
                                       (x.CategoryCode == ProductConsts.CategoryCodeDoneStep || !listCateFixed.Contains(x.CategoryCode))).Select(x => x.CategoryId).ToList();

                    //Lấy list các bước tạo phiếu hỗ trợ và báo cáo của tất cả phiếu 
                    var listAllStepPack = context.PermissionConfiguration
                            .Where(x => listCategoryInOrderProcess.Contains(x.CategoryId.Value)).ToList();

                    var listAllStepPackId = listAllStepPack.Select(x => x.Id).ToList();
                    //Lấy list all emp mapping bước 
                    var listAllEmpPermissionConfigure = context.EmployeeMappingPermissionConfiguration.Where(x => listAllStepPackId.Contains(x.PermissionConfigurationId)).ToList();

                    listOrderProcess.ForEach(item => {
                        item.CustomerName = listAllCus.FirstOrDefault(x => x.CustomerId == item.CustomerId)?.CustomerName;
                        item.StatusName = listStatus.FirstOrDefault(x => x.Value == item.Status).Name;
                        item.ProductCategoryName = listAllProductCategory.FirstOrDefault(x => x.ProductCategoryId == item.ProductCategoryId)?.ProductCategoryName;
                        item.ServicePacketName = listAllPacket.FirstOrDefault(x => x.Id == item.ServicePacketId)?.Name;

                        #region Lấy list người tạo + người phụ trách bước tạo phiếu + xác nhận phiếu
                        item.ListPersonInCharge = new List<Guid>();
                        item.ListPersonInCharge.Add(empLogin.EmployeeId);

                        //các nhân viên được truy cập phiêu yêu cầu
                        var listStepIdOfOrder = listAllStepPack.Where(x => x.ServicePacketId == item.ServicePacketId).Select(x => x.Id).ToList();
                        var listEmpId = listAllEmpPermissionConfigure.Where(x => listStepIdOfOrder.Contains(x.PermissionConfigurationId)).Select(x => x.EmployeeId).Distinct().ToList();
                        item.ListPersonInCharge.AddRange(listEmpId);

                        //Lấy ra tất cả người phụ trách gói
                        item.ListPersonInCharge.AddRange(listAllManagerMappingPack.Where(x => listPacketServiceId.Contains(x.PackId)).Select(x => x.EmployeeId).ToList());
                        item.ListPersonInCharge = item.ListPersonInCharge.Distinct().ToList();

                        #endregion
                    });

                    //Lọc theo các nhận viên được truy cập vào quy trình đặt dịch vụ của KH
                    listOrderProcess = listOrderProcess.Where(x =>
                       x.ListPersonInCharge == null || x.ListPersonInCharge.Count == 0 || x.ListPersonInCharge.Contains(empLogin.EmployeeId)).ToList();
                }

                return new SearchOrderResult()
                {
                    ListOrder = listOrder.OrderByDescending(x => x.CreatedDate).ToList(),
                    ListOrderProcess = listOrderProcess.OrderByDescending(x => x.CreatedDate).ToList(),
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success",
                };
            }
            catch (Exception e)
            {
                return new SearchOrderResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetMasterDataOrderCreateResult GetMasterDataOrderCreate(GetMasterDataOrderCreateParameter parameter)
        {
            try
            {
                var user = context.User.FirstOrDefault(x => x.UserId == parameter.UserId);
                var employee = context.Employee.FirstOrDefault(x => x.EmployeeId == user.EmployeeId);
                var empLoginName = employee.EmployeeName;

                var listOrderType = GeneralList.GetTrangThais("OrderType").ToList();

                var listOrderStatus = new List<OrderStatus>();
                var listOrderStatusEntityModel = new List<OrderStatusEntityModel>();
                var listCustomer = new List<CustomerEntityModel>();
                var listCustomerBankAccount = new List<BankAccountEntityModel>();
                var listPaymentMethod = new List<PaymentMethodConfigureEntityModel>();
                var listCustomerCode = new List<string>();

                #region Lấy List Customer
                var listAllContact = context.Contact.Where(x => x.Active == true && x.ObjectType == "CUS").ToList();
                var listAllCustomer = context.Customer.Where(x => x.Active == true).Select(
                    y => new CustomerEntityModel
                    {
                        CustomerId = y.CustomerId,
                        CustomerCode = y.CustomerCode,
                        CustomerName = y.CustomerName,
                        CustomerGroupId = y.CustomerGroupId,
                        CustomerEmail = "",
                        CustomerPhone = "",
                        FullAddress = "",
                        PaymentId = y.PaymentId,
                        PersonInChargeId = y.PersonInChargeId,
                        CustomerCodeName = y.CustomerCode + " - " + y.CustomerName,
                    }).ToList();

                listAllCustomer.ForEach(item =>
                {
                    var contact = listAllContact.FirstOrDefault(x => x.ObjectId == item.CustomerId && x.ObjectType == "CUS");
                    var email = contact == null ? "" : (contact.Email == null ? "" : contact.Email.Trim());
                    var phone = contact == null ? "" : (contact.Phone == null ? "" : contact.Phone.Trim());
                    var taxCode = contact == null
                        ? ""
                        : (contact.TaxCode == null ? "" : contact.TaxCode.Trim());
                    var address = contact == null ? "" : (contact.Address == null ? "" : contact.Address.Trim());
                    item.CustomerEmail = email;
                    item.CustomerPhone = phone;
                    item.TaxCode = taxCode;
                    item.FullAddress = address;
                });

                #endregion

                #region Lấy List Customer Code

                listCustomerCode = listAllCustomer.Where(x => (x.CustomerCode ?? "").Trim() != "")
                    .Select(y => y.CustomerCode.ToLower()).ToList();

                #endregion

                //Lấy tất cả các phiếu yêu cầu
                var listAllCustomerOrder = context.CustomerOrder.Where(x => x.IsOrderAction == false).Select(x => new CustomerOrderEntityModel()
                {
                    OrderId = x.OrderId,
                    OrderCode = x.OrderCode,
                    StatusOrder = x.StatusOrder,
                    CreatedById = x.CreatedById,
                    CreatedDate = x.CreatedDate,
                    CustomerId = x.CustomerId,
                    Vat = x.Vat,
                    DiscountType = x.DiscountType,
                    DiscountValue = x.DiscountValue,
                    OrderType = x.OrderType,
                    ObjectId = x.ObjectId,
                    IsOrderAction = x.IsOrderAction
                }).ToList();


                var paymentMethodCateTypeId = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "PaymentMethod").CategoryTypeId;

                var listPayMentCategory = context.Category.Where(x => x.CategoryTypeId == paymentMethodCateTypeId)
                        .Select(y => new CategoryEntityModel
                        {
                            CategoryId = y.CategoryId,
                            CategoryName = y.CategoryName,
                            CategoryCode = y.CategoryCode
                        }).OrderBy(z => z.CategoryName).ToList();

                listPaymentMethod = context.PaymentMethodConfigure.Select(x => new PaymentMethodConfigureEntityModel()
                {
                    Id = x.Id,
                    CategoryId = x.CategoryId,
                    CategoryName = listPayMentCategory.FirstOrDefault(y => y.CategoryId == x.CategoryId).CategoryName,
                    CategoryCode = listPayMentCategory.FirstOrDefault(y => y.CategoryId == x.CategoryId).CategoryCode,
                    Content = x.Content,
                    CreatedById = x.CreatedById,
                }).ToList();

                return new GetMasterDataOrderCreateResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success",
                    ListOrderStatus = listOrderStatusEntityModel,
                    ListCustomer = listAllCustomer,
                    ListCustomerBankAccount = listCustomerBankAccount,
                    ListPaymentMethod = listPaymentMethod,
                    ListCustomerCode = listCustomerCode,
                    EmpLoginName = empLoginName,
                    ListOrderType = listOrderType,
                    ListAllCustomerOrder = listAllCustomerOrder,
                };
            }
            catch (Exception e)
            {
                return new GetMasterDataOrderCreateResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public string GetContractCodeName(string code, string name)
        {
            string codeName;

            if (name != null)
            {
                codeName = code + " - " + name;
            }
            else
            {
                codeName = code;
            }

            return codeName;
        }

        public GetMasterDataOrderDetailDialogResult GetMasterDataOrderDetailDialog(GetMasterDataOrderDetailDialogParameter parameter)
        {
            try
            {
                var listPacketService = new List<ServicePacketEntityModel>();
                //Lấy các các gọi dịch vụ
                listPacketService = context.ServicePacket.Select(x => new ServicePacketEntityModel
                {
                    Name = x.Name,
                    Id = x.Id
                }).ToList();

                return new GetMasterDataOrderDetailDialogResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success",
                    ListPacketService = listPacketService,
                };
            }
            catch (Exception e)
            {
                return new GetMasterDataOrderDetailDialogResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetVendorByProductIdResult GetVendorByProductId(GetVendorByProductIdParameter parameter)
        {
            try
            {
                var listVendorId = context.ProductVendorMapping.Where(x => x.ProductId == parameter.ProductId)
                    .Select(y => y.VendorId).ToList();

                var listVendor = new List<VendorEntityModel>();

                if (listVendorId.Count > 0)
                {
                    listVendor = context.Vendor
                        .Where(x => listVendorId.Contains(x.VendorId) && x.Active == true)
                        .Select(y => new VendorEntityModel
                        {
                            VendorId = y.VendorId,
                            VendorCode = y.VendorCode,
                            VendorName = y.VendorName
                        }).ToList();
                }

                #region Lấy list thuộc tính của sản phẩm

                var listObjectAttributeNameProduct = new List<ObjectAttributeNameProductModel>();
                var listObjectAttributeValueProduct = new List<ObjectAttributeValueProductModel>();

                var listProductAttribute =
                    context.ProductAttribute.Where(x => x.ProductId == parameter.ProductId).ToList();

                List<Guid> listProductAttributeCategoryId = new List<Guid>();
                listProductAttribute.ForEach(item =>
                {
                    listProductAttributeCategoryId.Add(item.ProductAttributeCategoryId);
                });

                if (listProductAttributeCategoryId.Count > 0)
                {
                    listObjectAttributeNameProduct = context.ProductAttributeCategory
                        .Where(x => listProductAttributeCategoryId.Contains(x.ProductAttributeCategoryId))
                        .Select(y => new ObjectAttributeNameProductModel
                        {
                            ProductAttributeCategoryId = y.ProductAttributeCategoryId,
                            ProductAttributeCategoryName = y.ProductAttributeCategoryName
                        })
                        .ToList();

                    listObjectAttributeValueProduct = context.ProductAttributeCategoryValue
                        .Where(x => listProductAttributeCategoryId.Contains(x.ProductAttributeCategoryId))
                        .Select(y => new ObjectAttributeValueProductModel
                        {
                            ProductAttributeCategoryValueId = y.ProductAttributeCategoryValueId,
                            ProductAttributeCategoryValue = y.ProductAttributeCategoryValue1,
                            ProductAttributeCategoryId = y.ProductAttributeCategoryId
                        })
                        .ToList();
                }

                #endregion

                decimal priceProduct = 0;

                if (parameter.CustomerGroupId != null && parameter.CustomerGroupId != Guid.Empty)
                {
                    var listPriceProduct = context.PriceProduct
                        .Where(x => x.Active && x.EffectiveDate.Date <= parameter.OrderDate.Date &&
                                    x.ProductId == parameter.ProductId &&
                                    x.CustomerGroupCategory == parameter.CustomerGroupId)
                        .OrderByDescending(z => z.EffectiveDate)
                        .ToList();

                    var price = listPriceProduct.FirstOrDefault();

                    if (price != null)
                    {
                        priceProduct = price.PriceVnd;
                    }
                }

                return new GetVendorByProductIdResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success",
                    PriceProduct = priceProduct,
                    ListVendor = listVendor,
                    ListObjectAttributeValueProduct = listObjectAttributeValueProduct,
                    ListObjectAttributeNameProduct = listObjectAttributeNameProduct
                };
            }
            catch (Exception e)
            {
                return new GetVendorByProductIdResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetMasterDataOrderDetailResult GetMasterDataOrderDetail(GetMasterDataOrderDetailParameter parameter)
        {
            try
            {
                var listAllCustomerOrder = context.CustomerOrder.Select(x => new CustomerOrderEntityModel()
                {
                    OrderId = x.OrderId,
                    OrderCode = x.OrderCode,
                    StatusOrder = x.StatusOrder,
                    CreatedById = x.CreatedById,
                    CreatedDate = x.CreatedDate,
                    CustomerId = x.CustomerId,
                    Vat = x.Vat,
                    DiscountType = x.DiscountType,
                    DiscountValue = x.DiscountValue,
                    OrderType = x.OrderType,
                    ObjectId = x.ObjectId,
                    IsOrderAction = x.IsOrderAction,
                    OrderProcessId = x.OrderProcessId,
                    ServicePacketId = x.ServicePacketId,
                    PaymentContent = x.PaymentContent,
                    PaymentMethod = x.PaymentMethod,
                    PaymentMethodOrder = x.PaymentMethodOrder,
                    PaymentMethodNote = x.PaymentMethodNote
                }).ToList();

                var customerOrder = listAllCustomerOrder.FirstOrDefault(x => x.OrderId == parameter.OrderId);
                var listManager = context.ManagerPacketService.Where(x => x.PackId == customerOrder.ServicePacketId).Select(x => x.EmployeeId).ToList();
                bool isShowButtonDelete = false;
                var listUserId = context.User.Where(x => listManager.Any(y => y == x.EmployeeId)).Select(x => x.UserId).ToList();
                if (listUserId.Contains(parameter.UserId))
                {
                    isShowButtonDelete = true;
                }

                if (customerOrder == null)
                {
                    return new GetMasterDataOrderDetailResult()
                    {
                        StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                        MessageCode = "Không tồn tại phiếu yêu cầu dịch vụ trên hệ thống!",
                    };
                }
                customerOrder.StatusOrderName = GeneralList.GetTrangThais("CustomerOrder").FirstOrDefault(x => x.Value == customerOrder.StatusOrder).Name;
                var creatorUser = context.User.FirstOrDefault(x => x.UserId == customerOrder.CreatedById);
                var empNameCreator = context.Employee.FirstOrDefault(x => x.EmployeeId == creatorUser.EmployeeId)?.EmployeeName;

                if (string.IsNullOrEmpty(empNameCreator))
                {
                    var customerOrderDetailId = context.CustomerOrder.Where(x => x.OrderId == parameter.OrderId).Select(x => x.CustomerId)?.FirstOrDefault();
                    empNameCreator = context.Customer.FirstOrDefault(x => x.CustomerId == customerOrderDetailId)?.CustomerName;
                }

                #region phân quyền

                var userLogin = context.User.FirstOrDefault(x => x.UserId == parameter.UserId);
                var empLogin = context.Employee.FirstOrDefault(x => x.EmployeeId == userLogin.EmployeeId);

                var isCurrentStep = false;

                if (empLogin != null)
                {
                    //Lấy categoryId của bước tạo yêu cầu + xác nhận thanh toán
                    var cateTypeIdOfStep = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == ProductConsts.CategoryTypeCodeActionStep).CategoryTypeId;
                    var createOrderStepId = context.Category.FirstOrDefault(x => x.CategoryTypeId == cateTypeIdOfStep && x.CategoryCode == ProductConsts.CategoryCodeCreateStep).CategoryId;
                    var cofirmCostStepId = context.Category.FirstOrDefault(x => x.CategoryTypeId == cateTypeIdOfStep && x.CategoryCode == ProductConsts.CategoryCodeConfirmStep).CategoryId;

                    //Lấy list các bước tạo  phiếu yêu cầu và xác nhận thanh toán của gói
                    var listAllStepPack = context.PermissionConfiguration
                            .Where(x => x.ServicePacketId == customerOrder.ServicePacketId).OrderBy(x => x.StepId).ToList();

                    var listAllStepPackId = listAllStepPack.Select(x => x.Id).ToList();
                    var listAllEmpPermissionConfigure = context.EmployeeMappingPermissionConfiguration.Where(x => listAllStepPackId.Contains(x.PermissionConfigurationId)).ToList();

                    //Bước tạo
                    var createStep = listAllStepPack.FirstOrDefault(x => x.CategoryId == createOrderStepId);
                    var listEmpIdCreateId = listAllEmpPermissionConfigure.Where(x => x.PermissionConfigurationId == createStep.Id).Select(x => x.EmployeeId).ToList();
                    if (listEmpIdCreateId.Contains(empLogin.EmployeeId))
                    {
                        customerOrder.IsCreate = true;
                    }
                    //Bước xác nhận thanh toán
                    var confirmStep = listAllStepPack.FirstOrDefault(x => x.CategoryId == cofirmCostStepId);
                    var listEmpIdConfirm = listAllEmpPermissionConfigure.Where(x => x.PermissionConfigurationId == confirmStep.Id).Select(x => x.EmployeeId).ToList();
                    if (listEmpIdConfirm.Contains(empLogin.EmployeeId))
                    {
                        customerOrder.IsConfirm = true;
                    }

                    //Kiểm tra xem đã đến bước xác nhận thanh toán chưa ( phiếu yêu cầu không phải là phiếu bổ sung)
                    if (customerOrder.OrderType == 1)
                    {
                        //Thông tin các bước của phiếu
                        var listAllStep = context.OrderProcessDetail.Where(x => x.OrderProcessId == customerOrder.OrderProcessId).OrderBy(x => x.StepId).ToList();
                        var confirmStepOrder = listAllStep.FirstOrDefault(x => x.CategoryId == cofirmCostStepId);
                        //vị trí của bước xác nhận thanh toán
                        var indexConfirmStep = listAllStep.IndexOf(confirmStepOrder);
                        //Xem bước trước nó đã hoàn thành chưa. Nếu hoàn thành rồi thì xác nhận thanh toán là bước hiện tại
                        if (listAllStep[indexConfirmStep - 1].Status == 3)
                        {
                            isCurrentStep = true;
                        }
                    }

                    //Phiếu bổ sung sẽ không theo luồng từ bảng orderprocess
                    if (customerOrder.OrderType == 2)
                    {
                        isCurrentStep = true;
                    }

                }

                #endregion

                //Lấy các detail của phiếu
                var listDetail = context.CustomerOrderDetail.Where(x => x.OrderId == customerOrder.OrderId).Select(x => new CustomerOrderDetailEntityModel
                {
                    OrderDetailId = x.OrderDetailId,
                    OrderId = x.OrderId,
                    Quantity = x.Quantity,
                    PriceInitial = x.PriceInitial,
                    OptionId = x.OptionId,
                    ServicePacketId = x.PacketServiceId,
                    Stt = x.Stt,
                    CreatedDate = x.CreatedDate,
                    Vat = x.Vat,
                }).OrderBy(x => x.Stt).ToList();

                //Lấy các thuộc tính của gói và tùy chọn dịch vụ

                var listDetailId = listDetail.Select(x => x.OrderDetailId).ToList();

                var listPacketServiceId = listDetail.Select(x => x.ServicePacketId).Distinct().ToList();
                var listServicePacket = context.ServicePacket.Where(x => listPacketServiceId.Contains(x.Id)).Select(x => new ServicePacketEntityModel
                {
                    Id = x.Id,
                    Name = x.Name
                }).ToList();


                //Lấy sơ đầu tùy chọn dịch vụ của gói
                var listPacketId = listServicePacket.Select(x => x.Id).ToList();
                var listTreeOptionOfPack = context.ServicePacketMappingOptions.Where(x => listPacketId.Contains(x.ServicePacketId)).ToList();

                var listOptionId = listTreeOptionOfPack.Select(x => x.OptionId).ToList();
                var listAllOption = context.Options.Where(x => listOptionId.Contains(x.Id)).ToList();

                //Thuộc tính gói
                var listAtrrPacket = context.CustomerOrderExtension.Where(x => x.OrderDetailId == customerOrder.OrderId  && x.ObjectType == "2" && listPacketServiceId.Contains(x.ObjectId))
                                        .Select(x => new CustomerOrderExtensionEntityModel
                                        {
                                            Id = x.Id,
                                            OrderDetailId = x.OrderDetailId,
                                            AttributeConfigurationId = x.AttributeConfigurationId,
                                            ObjectId = x.ObjectId,
                                            ObjectType = x.ObjectType,
                                            Value = x.Value,
                                            DataType = x.DataType,
                                            ServicePacketMappingOptionsId = x.ServicePacketMappingOptionsId
                                        }).ToList();

                //Thuộc tính tùy chọn dịch vụ
                var listAtrrOption = context.CustomerOrderExtension.Where(x => listDetailId.Contains(x.OrderDetailId) && x.ObjectType == "1" && listOptionId.Contains(x.ObjectId))
                                        .Select(x => new CustomerOrderExtensionEntityModel
                                        {
                                            Id = x.Id,
                                            OrderDetailId = x.OrderDetailId,
                                            AttributeConfigurationId = x.AttributeConfigurationId,
                                            ObjectId = x.ObjectId,
                                            ObjectType = x.ObjectType,
                                            Value = x.Value,
                                            DataType = x.DataType,
                                            ServicePacketMappingOptionsId = x.ServicePacketMappingOptionsId
                                        }).ToList();


                //Hoàn thiện thông tin cho detail
                listDetail.ForEach(item =>
                {
                    var mappingOption = listTreeOptionOfPack.FirstOrDefault(y => y.Id == item.OptionId && y.ServicePacketId == item.ServicePacketId);
                    item.SortOrder = mappingOption.SortOrder;
                    var option = listAllOption.FirstOrDefault(y => y.Id == mappingOption.OptionId);
                    if (option != null)
                    {
                        if (mappingOption.ParentId != null)
                        {
                            item.OptionName = GetPathOption(option.Name, mappingOption.ParentId.Value, listTreeOptionOfPack);
                        }
                        else
                        {
                            item.OptionName = option.Name;
                        }
                    }
                });

                var listStatusOrderDetailExten = GeneralList.GetTrangThais("OrderDetailExten").ToList();

                //Lấy các dịch vụ phát sinh của gói
                var listOptionExten = context.CustomerOrderDetailExten.Where(x => x.OrderId == parameter.OrderId)
                                      .Select(x => new CustomerOrderDetailExtenEntityModel
                                      {
                                          Id = x.Id,
                                          OrderId = x.OrderId,
                                          Name = x.Name,
                                          Quantity = x.Quantity,
                                          Price = x.Price,
                                          Status = x.Status,
                                          StatusName = listStatusOrderDetailExten.FirstOrDefault(y => y.Value == x.Status).Name,
                                          StatusObject = listStatusOrderDetailExten.FirstOrDefault(y => y.Value == x.Status),
                                          Edit = false,
                                          Stt = x.Stt,
                                      }).OrderBy(x => x.Stt).ToList();

                //ListAll phiếu yêu cầu bổ sung
                var listAllOrderExten = new List<CustomerOrderEntityModel>();
                GetListCustomerOrder(customerOrder, listAllOrderExten, listAllCustomerOrder);
                var listCustomerOrderId = listAllOrderExten.Select(x => x.OrderId).ToList();
                listCustomerOrderId.Add(parameter.OrderId);
                //list phiếu hỗ trợ
                var listCustomerOrderAction = listAllCustomerOrder.Where(x => x.IsOrderAction == true
                                        && listCustomerOrderId.Contains(x.ObjectId)).ToList();
                //Lấy thêm thông tin cho loại phiếu trên
                var listUserIdSupport = listAllOrderExten.Select(x => x.CreatedById).ToList();
                var listUser = context.User.Where(x => listUserIdSupport.Contains(x.UserId)).ToList();
                var listEmpIdSupport = listUser.Select(x => x.EmployeeId).ToList();
                var listEmp = context.Employee.Where(x => listEmpIdSupport.Contains(x.EmployeeId)).ToList();

                var listCusId = listAllOrderExten.Select(x => x.CustomerId).ToList();
                var listCus = context.Customer.Where(x => listCusId.Contains(x.CustomerId)).ToList();

                var listStatusOrderExten = GeneralList.GetTrangThais("CustomerOrder").ToList();
                listAllOrderExten.ForEach(item =>
                {
                    item.CustomerName = listCus.FirstOrDefault(x => x.CustomerId == item.CustomerId)?.CustomerName;
                    var user = listUser.FirstOrDefault(x => x.UserId == item.CreatedById);
                    if(user != null)
                    {
                        item.SupporterName = listEmp.FirstOrDefault(x => x.EmployeeId == user.EmployeeId)?.EmployeeName;
                    }
                    item.OrderStatusName = listStatusOrderExten.FirstOrDefault(x => x.Value == item.StatusOrder)?.Name;
                });

                var listStatusOrderAction = GeneralList.GetTrangThais("CustomerOrderAction").ToList();
                listCustomerOrderAction.ForEach(item =>
                {
                    item.CustomerName = listCus.FirstOrDefault(x => x.CustomerId == item.CustomerId)?.CustomerName;
                    item.OrderStatusName = listStatusOrderAction.FirstOrDefault(x => x.Value == item.StatusOrder)?.Name;
                });

                var orderActionId = context.CustomerOrder.FirstOrDefault(x => x.ObjectId == parameter.OrderId && x.IsOrderAction == true)?.OrderId;
                var doiTuongApDung = GeneralList.GetTrangThais("DoiTuongApDungQuyTrinhPheDuyet").FirstOrDefault(x => x.Value == 31).Value;

            
                var isShowXacNhan = false;
                var isShowTuChoi = false;

                #region  Phân quyền hiển thị nút từ chối/Phê duyệt cho quy trình phê duyệt phát sinh || phê duyệt phiếu yêu cầu bổ sung
                if ((customerOrder.StatusOrder == 11 || customerOrder.StatusOrder == 12) && empLogin != null) // chờ phê duyệt
                {
                    var buocHienTai = context.CacBuocApDung.Where(x => x.ObjectId == customerOrder.OrderId &&
                                                                           x.DoiTuongApDung == doiTuongApDung &&
                                                                           x.TrangThai == 0)
                            .OrderByDescending(z => z.Stt)
                            .FirstOrDefault();

                    //Nếu là phê duyệt trưởng bộ phận
                    if (buocHienTai?.LoaiPheDuyet == 1)
                    {
                        var listDonViId_NguoiPhuTrach = context.ThanhVienPhongBan
                            .Where(x => x.EmployeeId == creatorUser.EmployeeId)
                            .Select(y => y.OrganizationId).ToList();

                        var countPheDuyet = context.ThanhVienPhongBan.Count(x =>
                            x.EmployeeId == empLogin.EmployeeId &&
                            x.IsManager == 1 &&
                            listDonViId_NguoiPhuTrach.Contains(
                                x.OrganizationId));

                        if (countPheDuyet > 0)
                        {
                            isShowXacNhan = true;
                            isShowTuChoi = true;
                        }
                    }
                    //Nếu là phòng ban phê duyệt
                    else if (buocHienTai?.LoaiPheDuyet == 2)
                    {
                        //Lấy list Phòng ban đã phê duyệt ở bước hiện tại
                        var listDonViIdDaPheDuyet = context.PhongBanApDung
                            .Where(x => x.CacBuocApDungId == buocHienTai.Id &&
                                        x.CacBuocQuyTrinhId == buocHienTai.CacBuocQuyTrinhId)
                            .Select(y => y.OrganizationId).ToList();

                        //Lấy list Phòng ban chưa phê duyệt ở bước hiện tại
                        var listDonViId = context.PhongBanTrongCacBuocQuyTrinh
                            .Where(x => x.CacBuocQuyTrinhId == buocHienTai.CacBuocQuyTrinhId &&
                                        !listDonViIdDaPheDuyet.Contains(x.OrganizationId))
                            .Select(y => y.OrganizationId).ToList();

                        var countPheDuyet = context.ThanhVienPhongBan.Count(x =>
                            x.EmployeeId == empLogin.EmployeeId &&
                            x.IsManager == 1 &&
                            listDonViId.Contains(
                                x.OrganizationId));

                        if (countPheDuyet > 0)
                        {
                            isShowXacNhan = true;
                            isShowTuChoi = true;
                        }
                    }
                    //Nếu là quản lý gói phê duyệt
                    if (buocHienTai?.LoaiPheDuyet == 3)
                    {
                        var listManagerPacket = context.ManagerPacketService.Where(x => x.PackId == customerOrder.ServicePacketId).Select(x => x.EmployeeId).ToList();
                        if (listManagerPacket.Contains(empLogin.EmployeeId))
                        {
                            isShowXacNhan = true;
                            isShowTuChoi = true;
                        }
                    }
                }
                #endregion 


                //Lấy quản lý gói để chat
                Guid? quanLyGoi_UserId = null;
                Guid? quanLyGoi_EmpId = null;
                String quanLyGoi_Name = "";
                var quanLyGoi = context.ManagerPacketService.FirstOrDefault(x => x.PackId == customerOrder.ServicePacketId);
                
                if(quanLyGoi != null)
                {
                    quanLyGoi_EmpId = quanLyGoi.EmployeeId;
                    quanLyGoi_UserId = context.User.FirstOrDefault(x => x.EmployeeId == quanLyGoi.EmployeeId)?.UserId;
                    quanLyGoi_Name = context.Employee.FirstOrDefault(x => x.EmployeeId == quanLyGoi.EmployeeId)?.EmployeeName;
                }

                return new GetMasterDataOrderDetailResult()
                {
                    EmpNameCreator = empNameCreator,
                    QuanLyGoi_UserId = quanLyGoi_UserId,
                    QuanLyGoi_EmpId = quanLyGoi_EmpId,
                    QuanLyGoi_Name = quanLyGoi_Name,
                    CustomerOrder = customerOrder,
                    ListDetail = listDetail,
                    ListAtrrPacket = listAtrrPacket,
                    ListAtrrOption = listAtrrOption,
                    ListServicePacket = listServicePacket,
                    ListOptionExten = listOptionExten,
                    ListAllOrderExten = listAllOrderExten,
                    ListCustomerOrderAction = listCustomerOrderAction,
                    OrderActionId = orderActionId,
                    IsCurrentStep = isCurrentStep,
                    IsShowXacNhan = isShowXacNhan,
                    IsShowTuChoi = isShowTuChoi,
                    IsShowButtonDelete = isShowButtonDelete,
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = "Success",
                };
            }
            catch (Exception e)
            {
                return new GetMasterDataOrderDetailResult()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message,
                };
            }
        }

        public DeleteOrderResult DeleteOrder(DeleteOrderParameter parameter)
        {
            try
            {
                var order = context.CustomerOrder.FirstOrDefault(x => x.OrderId == parameter.OrderId);

                if (order == null)
                {
                    return new DeleteOrderResult()
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = "Không tồn tại đơn hàng này trên hệ thống"
                    };
                }

                #region Kiểm tra đơn hàng có phải trạng thái Nháp không

                var status = context.OrderStatus.FirstOrDefault(x => x.OrderStatusId == order.StatusId);

                if (status.OrderStatusCode != "DRA")
                {
                    var description = status.Description;
                    return new DeleteOrderResult()
                    {
                        Status = false,
                        Message = "Không thể xóa đơn hàng có trạng thái " + description
                    };
                }

                #endregion

                #region Xóa NoteDocument và Note của đơn hàng

                var listNote = context.Note.Where(x => x.ObjectId == parameter.OrderId && x.ObjectType == "DH")
                    .ToList();

                var listNoteId = listNote.Select(x => x.NoteId).ToList();
                if (listNoteId.Count > 0)
                {
                    var listNoteDocument = context.NoteDocument.Where(x => listNoteId.Contains(x.NoteId)).ToList();
                    context.NoteDocument.RemoveRange(listNoteDocument);
                }

                context.Note.RemoveRange(listNote);

                #endregion

                #region Xóa đơn hàng và các reference của nó

                var listOrderDetail = context.CustomerOrderDetail.Where(x => x.OrderId == parameter.OrderId).ToList();

                if (listOrderDetail.Count > 0)
                {
                    var listOrderDetailId = listOrderDetail.Select(x => x.OrderDetailId).ToList();
                    var listOrderProductDetailProductAttributeValue = context.OrderProductDetailProductAttributeValue
                        .Where(x => listOrderDetailId.Contains(x.OrderDetailId)).ToList();

                    context.OrderProductDetailProductAttributeValue.RemoveRange(
                        listOrderProductDetailProductAttributeValue);

                    context.CustomerOrderDetail.RemoveRange(listOrderDetail);
                }

                var listOrderCostDetail = context.OrderCostDetail.Where(x => x.OrderId == parameter.OrderId).ToList();
                context.OrderCostDetail.RemoveRange(listOrderCostDetail);

                var listOrderLocalPointMapping = context.CustomerOrderLocalPointMapping
                    .Where(x => x.OrderId == parameter.OrderId).ToList();
                context.CustomerOrderLocalPointMapping.RemoveRange(listOrderLocalPointMapping);

                //Cập nhật lại trạng thái các Điểm gắn với Đơn hàng thành Sẵn sàng
                var listLocalPointId = listOrderLocalPointMapping.Select(y => y.LocalPointId).ToList();
                var listLocalPoint = context.LocalPoint.Where(x => listLocalPointId.Contains(x.LocalPointId)).ToList();
                listLocalPoint.ForEach(item => { item.StatusId = 1; });
                context.LocalPoint.UpdateRange(listLocalPoint);

                context.CustomerOrder.Remove(order);
                context.SaveChanges();

                #endregion

                #region lưu nhật ký hệ thống

                LogHelper.AuditTrace(context, ActionName.DELETE, ObjectName.CUSTOMERORDER, order.OrderId, parameter.UserId);

                #endregion

                return new DeleteOrderResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success"
                };
            }
            catch (Exception e)
            {
                return new DeleteOrderResult()
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
                var empId = context.User.FirstOrDefault(u => u.UserId == createById) != null ? context.User.FirstOrDefault(u => u.UserId == createById).EmployeeId : null;

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

        public GetDataDashboardHomeResult GetDataDashboardHome(GetDataDashboardHomeParameter parameter)
        {
            try
            {
                var user = context.User.FirstOrDefault(x => x.UserId == parameter.UserId);
                var employee = context.Employee.FirstOrDefault(x => x.EmployeeId == user.EmployeeId);

                var listCommonOrganization = context.Organization.ToList();
                var listCommonPosition = context.Position.ToList();
                var listCommonCustomer = context.Customer.ToList();
                var listCommonContact = context.Contact.ToList();
                var listCommonLead = context.Lead.ToList();
                var listCommonUser = context.User.ToList();
                var listCommonEmployee = context.Employee.ToList();
                var listCommonOrder = context.CustomerOrder.ToList();
                var listCommonOrderStatus = context.OrderStatus.Where(x => x.Active == true).ToList();
                var listQuoteDetail = context.QuoteDetail.Where(x => x.Active == true).ToList();
                var listQuoteCostDetail = context.QuoteCostDetail.Where(x => x.Active == true).ToList();
                var listPromotionObjectApply = context.PromotionObjectApply.ToList();
                var appName = context.SystemParameter.FirstOrDefault(x => x.SystemKey == "ApplicationName")
                    .SystemValueString;


                //var listOrderCode = new List<string> { "IP", "PD", "DLV", "COMP" };
                var listOrderCode = context.SystemParameter.FirstOrDefault(x => x.SystemKey == "OrderStatus")
                    .SystemValueString.Split(';').ToList();
                var listOrderStatusId = listCommonOrderStatus.Where(x => listOrderCode.Contains(x.OrderStatusCode))
                    .Select(y => y.OrderStatusId);

                DateTime firstDateOfWeek = FirstDateOfWeek();
                DateTime lastDateOfWeek = LastDateOfWeek();
                DateTime firstDateOfMonth = FirstDateOfMonth();
                DateTime lastDateOfMonth = LastDateOfMonth();
                var dateOfQuarter = GetDateOfQuarter(DateTime.Now);
                var dateOfQuarterPress = GetDateOfQuarter(DateTime.Now.AddMonths(-4));

                decimal totalSalesOfWeek = 0;   //Tổng doanh thu của các đơn hàng trong tuần
                decimal totalSalesOfMonth = 0;   //Tổng doanh thu của các đơn hàng trong tháng
                decimal totalSalesOfQuarter = 0;   //Tổng doanh thu của các đơn hàng trong Qúy
                decimal totalSalesOfYear = 0; // Tổng doanh thu của các đơn hàng trong năm

                decimal chiTieuDoanhThuTuan = 0;
                decimal chiTieuDoanhThuThang = 0;
                decimal chiTieuDanhThuQuy = 0;
                decimal chiTieuDoanhThuNam = 0;

                decimal totalSalesOfWeekPress = 0;   //Tổng doanh thu của các đơn hàng trong tuần trước
                decimal totalSalesOfMonthPress = 0;   //Tổng doanh thu của các đơn hàng trong tháng trước
                decimal totalSalesOfQuarterPress = 0;   //Tổng doanh thu của các đơn hàng trong Qúy trước
                decimal totalSalesOfYearPress = 0; //Tổng doanh thu của các đơn hàng trong Qúy trước

                var listQuote = new List<QuoteEntityModel>();
                var listCustomer = new List<CustomerEntityModel>();
                var listOrderNew = new List<CustomerOrderEntityModel>();
                var listCustomerMeeting = new List<CustomerMeetingEntityModel>();
                var listLeadMeeting = new List<LeadMeetingEntityModel>();
                var listCusBirthdayOfWeek_1 = new List<CustomerEntityModel>();
                var listEmployeeBirthDayOfWeek = new List<EmployeeEntityModel>();

                // Lịch hẹn khách hàng
                listCustomerMeeting = context.CustomerMeeting
                    .Where(x => x.Participants.Contains(employee.EmployeeId.ToString())
                                && x.Active == true
                                ).Select(y =>
                        new CustomerMeetingEntityModel
                        {
                            CustomerMeetingId = y.CustomerMeetingId,
                            CustomerId = y.CustomerId,
                            EmployeeId = y.EmployeeId,
                            StartDate = y.StartDate,
                            EndDate = y.EndDate,
                            Title = y.Title,
                            Content = y.Content,
                            LocationMeeting = y.LocationMeeting,
                            CreatedById = y.CreatedById,
                            CustomerName = "",
                            EmployeeName = "",
                            CreateByName = "",
                            IsShowLink = false,
                            Participants = y.Participants,
                            IsCreateByUser = y.CreatedById == user.UserId
                        }).OrderByDescending(z => z.StartDate).ToList();

                // Lịch hẹn cơ hội
                //  c.StartDate >= firstDateOfWeek && c.StartDate <= lastDateOfMonth
                listLeadMeeting = context.LeadMeeting.Where(c => c.Participant.Contains(employee.EmployeeId.ToString())
                                                 && c.Active == true)
                                                 .Select(m => new LeadMeetingEntityModel
                                                 {
                                                     LeadMeetingId = m.LeadMeetingId,
                                                     LeadId = m.LeadId,
                                                     EmployeeId = m.EmployeeId,
                                                     StartDate = m.StartDate,
                                                     EndDate = m.EndDate,
                                                     Title = m.Title,
                                                     Content = m.Content,
                                                     LocationMeeting = m.LocationMeeting,
                                                     Active = m.Active,
                                                     CreatedById = m.CreatedById,
                                                     CreateByName = "",
                                                     LeadName = "",
                                                     EmployeeName = "",
                                                     IsShowLink = false,
                                                     IsCreateByUser = m.CreatedById == user.UserId
                                                 }).OrderByDescending(z => z.StartDate).ToList();

                var listAllOrganization = context.Organization.ToList();

                if (employee.IsManager)
                {
                    //List phòng ban: chính nó và các phòng ban cấp dưới của nó
                    List<Guid?> listGetAllChild = new List<Guid?>();
                    if (employee.OrganizationId != null)
                    {
                        listGetAllChild.Add(employee.OrganizationId.Value);
                        listGetAllChild = getOrganizationChildrenId(employee.OrganizationId.Value, listGetAllChild);
                    }

                    //List nhân viên thuộc quyền của quản lý
                    var listAllEmp = listCommonEmployee.Where(x => listGetAllChild.Contains(x.OrganizationId)).ToList();
                    var listAllEmpId = listAllEmp.Where(x => x.EmployeeId != null && x.EmployeeId != Guid.Empty)
                        .Select(y => y.EmployeeId).ToList();
                    var listAllUserId = context.User.Where(x => listAllEmpId.Contains(x.EmployeeId.Value))
                        .Select(y => y.UserId).ToList();
                    var _listCommonCustomer = listCommonCustomer.Where(x =>
                        (x.PersonInChargeId != null && listAllEmpId.Contains(x.PersonInChargeId.Value)) ||
                        (x.PersonInChargeId == null && listAllUserId.Contains(x.CreatedById))).ToList();


                    var _listCommonOrder = listCommonOrder.Where(x => listAllEmpId.Contains(x.Seller.Value)).ToList();

                    var listChildId = new List<Guid>();
                    if (employee.OrganizationId != null)
                    {
                        listChildId.Add(employee.OrganizationId.Value);
                        var listChildOrganizationId = listAllOrganization.Where(c => c.ParentId == employee.OrganizationId).Select(m => m.OrganizationId).ToList();
                        while (listChildOrganizationId.Count() != 0)
                        {
                            listChildId.AddRange(listChildOrganizationId);
                            listChildOrganizationId = listAllOrganization.Where(c => c.ParentId != null && listChildOrganizationId.Contains(c.ParentId.Value)).Select(m => m.OrganizationId).ToList();
                        }
                    }

                    var listBusinessId = context.BusinessGoals.Where(c => listChildId.Contains(c.OrganizationId) && c.Year == DateTime.Now.Year.ToString()).Select(m => m.BusinessGoalsId).ToList();
                    var listBusinessGoalsDetail = context.BusinessGoalsDetail.Where(c => listBusinessId.Contains(c.BusinessGoalsId)).ToList();
                    var currentMonth = DateTime.Now.Month;

                    // Chỉ tiêu các tháng

                    var chiTieuDoanhThuThangT1 = listBusinessGoalsDetail.Where(c => c.BusinessGoalsType == "REVENUE").Sum(c => c.January.Value);
                    var chiTieuDoanhThuThangT2 = listBusinessGoalsDetail.Where(c => c.BusinessGoalsType == "REVENUE").Sum(c => c.February.Value);
                    var chiTieuDoanhThuThangT3 = listBusinessGoalsDetail.Where(c => c.BusinessGoalsType == "REVENUE").Sum(c => c.March.Value);
                    var chiTieuDoanhThuThangT4 = listBusinessGoalsDetail.Where(c => c.BusinessGoalsType == "REVENUE").Sum(c => c.April.Value);
                    var chiTieuDoanhThuThangT5 = listBusinessGoalsDetail.Where(c => c.BusinessGoalsType == "REVENUE").Sum(c => c.May.Value);
                    var chiTieuDoanhThuThangT6 = listBusinessGoalsDetail.Where(c => c.BusinessGoalsType == "REVENUE").Sum(c => c.June.Value);
                    var chiTieuDoanhThuThangT7 = listBusinessGoalsDetail.Where(c => c.BusinessGoalsType == "REVENUE").Sum(c => c.July.Value);
                    var chiTieuDoanhThuThangT8 = listBusinessGoalsDetail.Where(c => c.BusinessGoalsType == "REVENUE").Sum(c => c.August.Value);
                    var chiTieuDoanhThuThangT9 = listBusinessGoalsDetail.Where(c => c.BusinessGoalsType == "REVENUE").Sum(c => c.September.Value);
                    var chiTieuDoanhThuThangT10 = listBusinessGoalsDetail.Where(c => c.BusinessGoalsType == "REVENUE").Sum(c => c.October.Value);
                    var chiTieuDoanhThuThangT11 = listBusinessGoalsDetail.Where(c => c.BusinessGoalsType == "REVENUE").Sum(c => c.November.Value);
                    var chiTieuDoanhThuThangT12 = listBusinessGoalsDetail.Where(c => c.BusinessGoalsType == "REVENUE").Sum(c => c.December.Value);


                    #region Doanh thu bán hàng trong tuần

                    var listOrderOfWeek = _listCommonOrder.Where(x =>
                            listOrderStatusId.Contains(x.StatusId.Value) && x.OrderDate >= firstDateOfWeek &&
                            x.OrderDate <= lastDateOfWeek)
                        .ToList();

                    listOrderOfWeek.ForEach(item =>
                    {
                        decimal sales = 0; //Doanh số của đơn hàng

                        if (item.DiscountType == true)
                        {
                            sales = item.Amount.Value - ((item.Amount.Value * item.DiscountValue.Value) / 100);
                        }
                        else
                        {
                            sales = item.Amount.Value - item.DiscountValue.Value;
                        }

                        totalSalesOfWeek += sales;
                    });
                    switch (currentMonth)
                    {
                        case 1:
                            chiTieuDoanhThuTuan = chiTieuDoanhThuThangT1 / 4;
                            break;
                        case 2:
                            chiTieuDoanhThuTuan = chiTieuDoanhThuThangT2 / 4;
                            break;
                        case 3:
                            chiTieuDoanhThuTuan = chiTieuDoanhThuThangT3 / 4;
                            break;
                        case 4:
                            chiTieuDoanhThuTuan = chiTieuDoanhThuThangT4 / 4;
                            break;
                        case 5:
                            chiTieuDoanhThuTuan = chiTieuDoanhThuThangT5 / 4;
                            break;
                        case 6:
                            chiTieuDoanhThuTuan = chiTieuDoanhThuThangT6 / 4;
                            break;
                        case 7:
                            chiTieuDoanhThuTuan = chiTieuDoanhThuThangT7 / 4;
                            break;
                        case 8:
                            chiTieuDoanhThuTuan = chiTieuDoanhThuThangT8 / 4;
                            break;
                        case 9:
                            chiTieuDoanhThuTuan = chiTieuDoanhThuThangT9 / 4;
                            break;
                        case 10:
                            chiTieuDoanhThuTuan = chiTieuDoanhThuThangT10 / 4;
                            break;
                        case 11:
                            chiTieuDoanhThuTuan = chiTieuDoanhThuThangT11 / 4;
                            break;
                        case 12:
                            chiTieuDoanhThuTuan = chiTieuDoanhThuThangT12 / 4;
                            break;

                    }
                    #endregion

                    #region Doanh thu bán hàng trong tuần trước

                    var listOrderOfWeekPress = _listCommonOrder.Where(x =>
                            listOrderStatusId.Contains(x.StatusId.Value) && x.OrderDate >= firstDateOfWeek.AddDays(-7) &&
                            x.OrderDate <= lastDateOfWeek.AddDays(-7))
                        .ToList();

                    listOrderOfWeekPress.ForEach(item =>
                    {
                        decimal sales = 0; //Doanh số của đơn hàng

                        if (item.DiscountType == true)
                        {
                            sales = item.Amount.Value - ((item.Amount.Value * item.DiscountValue.Value) / 100);
                        }
                        else
                        {
                            sales = item.Amount.Value - item.DiscountValue.Value;
                        }

                        totalSalesOfWeekPress += sales;
                    });


                    #endregion

                    #region Doanh thu bán hàng trong tháng

                    var listOrderOfMonth = _listCommonOrder.Where(x =>
                            listOrderStatusId.Contains(x.StatusId.Value) && x.OrderDate >= firstDateOfMonth &&
                            x.OrderDate <= lastDateOfMonth)
                        .ToList();

                    listOrderOfMonth.ForEach(item =>
                    {
                        decimal sales = 0; //Doanh số của đơn hàng

                        if (item.DiscountType == true)
                        {
                            sales = item.Amount.Value - ((item.Amount.Value * item.DiscountValue.Value) / 100);
                        }
                        else
                        {
                            sales = item.Amount.Value - item.DiscountValue.Value;
                        }

                        totalSalesOfMonth = totalSalesOfMonth + sales;
                    });

                    switch (currentMonth)
                    {
                        case 1:
                            chiTieuDoanhThuThang = chiTieuDoanhThuThangT1;
                            break;
                        case 2:
                            chiTieuDoanhThuThang = chiTieuDoanhThuThangT2;
                            break;
                        case 3:
                            chiTieuDoanhThuThang = chiTieuDoanhThuThangT3;
                            break;
                        case 4:
                            chiTieuDoanhThuThang = chiTieuDoanhThuThangT4;
                            break;
                        case 5:
                            chiTieuDoanhThuThang = chiTieuDoanhThuThangT5;
                            break;
                        case 6:
                            chiTieuDoanhThuThang = chiTieuDoanhThuThangT6;
                            break;
                        case 7:
                            chiTieuDoanhThuThang = chiTieuDoanhThuThangT7;
                            break;
                        case 8:
                            chiTieuDoanhThuThang = chiTieuDoanhThuThangT8;
                            break;
                        case 9:
                            chiTieuDoanhThuThang = chiTieuDoanhThuThangT9;
                            break;
                        case 10:
                            chiTieuDoanhThuThang = chiTieuDoanhThuThangT10;
                            break;
                        case 11:
                            chiTieuDoanhThuThang = chiTieuDoanhThuThangT11;
                            break;
                        case 12:
                            chiTieuDoanhThuThang = chiTieuDoanhThuThangT12;
                            break;
                    }

                    #endregion

                    #region Doanh thu bán hàng trong tháng trước

                    var listOrderOfMonthPress = _listCommonOrder.Where(x =>
                            listOrderStatusId.Contains(x.StatusId.Value) && x.OrderDate >= firstDateOfMonth.AddMonths(-1) &&
                            x.OrderDate <= firstDateOfMonth.AddDays(-1).AddHours(23).AddMinutes(59).AddSeconds(59).AddTicks(999))
                        .ToList();

                    listOrderOfMonthPress.ForEach(item =>
                    {
                        decimal sales = 0; //Doanh số của đơn hàng

                        if (item.DiscountType == true)
                        {
                            sales = item.Amount.Value - ((item.Amount.Value * item.DiscountValue.Value) / 100);
                        }
                        else
                        {
                            sales = item.Amount.Value - item.DiscountValue.Value;
                        }

                        totalSalesOfMonthPress = totalSalesOfMonthPress + sales;
                    });

                    #endregion

                    #region Doanh thu bán hàng trong Qúy

                    var listOrderOfQuarter = _listCommonOrder.Where(x =>
                            listOrderStatusId.Contains(x.StatusId.Value) &&
                            x.OrderDate >= dateOfQuarter.FirstDateOfQuarter &&
                            x.OrderDate <= dateOfQuarter.LastDateOfQuarter)
                        .ToList();

                    listOrderOfQuarter.ForEach(item =>
                    {
                        decimal sales = 0; //Doanh số của đơn hàng

                        if (item.DiscountType == true)
                        {
                            sales = item.Amount.Value - ((item.Amount.Value * item.DiscountValue.Value) / 100);
                        }
                        else
                        {
                            sales = item.Amount.Value - item.DiscountValue.Value;
                        }

                        totalSalesOfQuarter = totalSalesOfQuarter + sales;
                    });

                    if (currentMonth == 1 || currentMonth == 2 || currentMonth == 3)
                    {
                        chiTieuDanhThuQuy = chiTieuDoanhThuThangT1 + chiTieuDoanhThuThangT2 + chiTieuDoanhThuThangT3;
                    }
                    else if (currentMonth == 4 || currentMonth == 5 || currentMonth == 6)
                    {
                        chiTieuDanhThuQuy = chiTieuDoanhThuThangT4 + chiTieuDoanhThuThangT5 + chiTieuDoanhThuThangT6;
                    }
                    else if (currentMonth == 7 || currentMonth == 8 || currentMonth == 9)
                    {
                        chiTieuDanhThuQuy = chiTieuDoanhThuThangT7 + chiTieuDoanhThuThangT8 + chiTieuDoanhThuThangT9;
                    }
                    else if (currentMonth == 10 || currentMonth == 11 || currentMonth == 12)
                    {
                        chiTieuDanhThuQuy = chiTieuDoanhThuThangT10 + chiTieuDoanhThuThangT11 + chiTieuDoanhThuThangT12;
                    }

                    #endregion

                    #region Doanh thu bán hàng trong Qúy Trước

                    var listOrderOfQuarterPress = _listCommonOrder.Where(x =>
                            listOrderStatusId.Contains(x.StatusId.Value) &&
                            x.OrderDate >= dateOfQuarterPress.FirstDateOfQuarter &&
                            x.OrderDate <= dateOfQuarterPress.LastDateOfQuarter)
                        .ToList();

                    listOrderOfQuarterPress.ForEach(item =>
                    {
                        decimal sales = 0; //Doanh số của đơn hàng

                        if (item.DiscountType == true)
                        {
                            sales = item.Amount.Value - ((item.Amount.Value * item.DiscountValue.Value) / 100);
                        }
                        else
                        {
                            sales = item.Amount.Value - item.DiscountValue.Value;
                        }

                        totalSalesOfQuarterPress = totalSalesOfQuarterPress + sales;
                    });

                    #endregion

                    #region Doanh thu bán hàng trong Năm
                    var listOrderYear = _listCommonOrder.Where(x =>
                            listOrderStatusId.Contains(x.StatusId.Value) && x.OrderDate.Year == DateTime.Now.Year)
                        .ToList();

                    listOrderYear.ForEach(item =>
                    {
                        decimal sales = 0; //Doanh thu của đơn hàng

                        if (item.DiscountType == true)
                        {
                            sales = item.Amount.Value - ((item.Amount.Value * item.DiscountValue.Value) / 100);
                        }
                        else
                        {
                            sales = item.Amount.Value - item.DiscountValue.Value;
                        }

                        totalSalesOfYear += sales;
                    });

                    chiTieuDoanhThuNam = chiTieuDoanhThuThangT1 + chiTieuDoanhThuThangT2 + chiTieuDoanhThuThangT3 + chiTieuDoanhThuThangT4 + chiTieuDoanhThuThangT5 + chiTieuDoanhThuThangT6
                        + chiTieuDoanhThuThangT7 + chiTieuDoanhThuThangT8 + chiTieuDoanhThuThangT9 + chiTieuDoanhThuThangT10 + chiTieuDoanhThuThangT11 + chiTieuDoanhThuThangT12;
                    #endregion

                    #region Doanh thu bán hàng trong Năm trước
                    var listOrderYearPress = _listCommonOrder.Where(x => listOrderStatusId.Contains(x.StatusId.Value)
                            && x.OrderDate.Year == (DateTime.Now.Year - 1)).ToList();

                    listOrderYearPress.ForEach(item =>
                    {
                        decimal sales = 0; //Doanh thu của đơn hàng

                        if (item.DiscountType == true)
                        {
                            sales = item.Amount.Value - ((item.Amount.Value * item.DiscountValue.Value) / 100);
                        }
                        else
                        {
                            sales = item.Amount.Value - item.DiscountValue.Value;
                        }

                        totalSalesOfYearPress += sales;
                    });
                    #endregion

                    #region Lấy 5 báo giá mới

                    var categoryTypeCodeOfQuote = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "TGI");
                    var quoteStatusNewId = context.Category.FirstOrDefault(x =>
                        x.CategoryTypeId == categoryTypeCodeOfQuote.CategoryTypeId && x.CategoryCode == "MTA").CategoryId;

                    // id user dang signin
                    var employeeId = employee.EmployeeId;

                    var listQuoteParticipantMappingId =
                        context.QuoteParticipantMapping.Where(x => x.EmployeeId == employeeId).Select(y => y.QuoteId).ToList();

                    listQuote = context.Quote.Where(x =>
                            x.StatusId == quoteStatusNewId &&
                            (listAllEmpId.Contains(x.Seller.Value) || listAllEmpId.Contains(x.PersonInChargeId.Value)) &&
                            x.Active == true ||
                            (listQuoteParticipantMappingId.Contains(x.QuoteId)))
                        .OrderByDescending(z => z.QuoteDate).Take(5).Select(y => new QuoteEntityModel
                        {
                            QuoteId = y.QuoteId,
                            QuoteCode = y.QuoteCode,
                            Amount = AmountAfterDiscount(y.Amount, y.DiscountType, y.DiscountValue),
                            ObjectTypeId = y.ObjectTypeId,
                            ObjectType = y.ObjectType,
                            CustomerName = "",
                            CustomerContactId = null,
                            Seller = y.Seller,
                            SellerName = "",
                            QuoteDate = y.QuoteDate,
                            DiscountType = y.DiscountType,
                            DiscountValue = y.DiscountValue,
                            TotalAmountAfterVat = CalculateTotalAmountAfterVat(y.QuoteId, y.DiscountType, y.DiscountValue, y.Vat, listQuoteDetail, listQuoteCostDetail, listPromotionObjectApply, appName)
                        }).ToList();

                    listQuote.ForEach(item =>
                    {
                        if (item.ObjectType == "LEAD")
                        {
                            var lead = listCommonContact.FirstOrDefault(x =>
                                x.ObjectId == item.ObjectTypeId && x.ObjectType == "LEA");
                            item.CustomerName = lead == null
                                ? ""
                                : ((lead.FirstName == null ? "" : lead.FirstName.Trim()) + " " +
                                   (lead.LastName == null ? "" : lead.LastName.Trim()));
                            item.CustomerContactId = lead == null ? Guid.Empty : (lead.ContactId);
                        }
                        else if (item.ObjectType == "CUSTOMER")
                        {
                            var cus = listCommonContact.FirstOrDefault(x =>
                                x.ObjectId == item.ObjectTypeId && x.ObjectType == "CUS");
                            item.CustomerName = cus == null
                                ? ""
                                : ((cus.FirstName == null ? "" : cus.FirstName.Trim()) + " " +
                                   (cus.LastName == null ? "" : cus.LastName.Trim()));
                            item.CustomerContactId = cus == null ? Guid.Empty : (cus.ContactId);
                        }
                    });

                    var listSellerId = listQuote.Select(x => x.Seller).ToList();
                    if (listSellerId.Count > 0)
                    {
                        var listSellerOfQuote =
                            listCommonEmployee.Where(x => listSellerId.Contains(x.EmployeeId)).ToList();
                        listQuote.ForEach(item =>
                        {
                            var emp = listSellerOfQuote.FirstOrDefault(x => x.EmployeeId == item.Seller);
                            if (emp != null)
                            {
                                item.SellerName = emp.EmployeeName.Trim();
                            }
                        });
                    }

                    #endregion

                    #region Lấy 5 khách hàng mới

                    var categoryTypeCodeCustomer =
                        context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "THA");
                    var statusCustomerNewId = context.Category.FirstOrDefault(x =>
                            x.CategoryTypeId == categoryTypeCodeCustomer.CategoryTypeId && x.CategoryCode == "HDO")
                        .CategoryId;

                    listCustomer = _listCommonCustomer.Where(x =>
                            x.StatusId == statusCustomerNewId && x.Active == true)
                        .OrderByDescending(z => z.CreatedDate).Take(5).Select(y => new CustomerEntityModel
                        {
                            CustomerId = y.CustomerId,
                            CustomerName = y.CustomerName,
                            CustomerPhone = "",
                            CustomerEmail = "",
                            PersonInChargeId = y.PersonInChargeId,
                            PicName = "",
                            PicContactId = null
                        }).ToList();

                    var listCustomerId = listCustomer.Select(x => x.CustomerId).ToList();
                    var listEmployeeId = listCustomer.Select(x => x.PersonInChargeId).ToList();

                    if (listCustomerId.Count > 0)
                    {
                        var listContactCustomer = listCommonContact
                            .Where(x => listCustomerId.Contains(x.ObjectId) && x.ObjectType == "CUS").ToList();

                        listCustomer.ForEach(item =>
                        {
                            var cus_contact = listContactCustomer.FirstOrDefault(x => x.ObjectId == item.CustomerId);
                            item.CustomerPhone = cus_contact == null
                                ? ""
                                : (cus_contact.Phone == null ? "" : cus_contact.Phone.Trim());
                            item.CustomerEmail = cus_contact == null
                                ? ""
                                : (cus_contact.Email == null ? "" : cus_contact.Email.Trim());
                        });
                    }

                    if (listEmployeeId.Count > 0)
                    {
                        var listContactEmployee = listCommonContact
                            .Where(x => listEmployeeId.Contains(x.ObjectId) && x.ObjectType == "EMP").ToList();

                        listCustomer.ForEach(item =>
                        {
                            var emp_contact =
                                listContactEmployee.FirstOrDefault(x => x.ObjectId == item.PersonInChargeId);
                            item.PicName = emp_contact == null
                                ? ""
                                : (emp_contact.FirstName ?? "" + emp_contact.LastName ?? "");
                            if (emp_contact != null)
                            {
                                item.PicContactId = emp_contact.ContactId;
                            }
                        });
                    }

                    #endregion

                    #region Lấy 5 đơn hàng mới

                    listOrderNew = _listCommonOrder.Where(x =>
                            listOrderStatusId.Contains(x.StatusId.Value) && x.Active == true).OrderByDescending(z => z.OrderDate).Take(5)
                        .Select(y => new CustomerOrderEntityModel
                        {
                            OrderId = y.OrderId,
                            OrderCode = y.OrderCode,
                            CustomerId = y.CustomerId,
                            CustomerName = "",
                            Amount = AmountAfterDiscount(y.Amount, y.DiscountType, y.DiscountValue),
                            Seller = y.Seller,
                            SellerName = "",
                            SellerContactId = Guid.Empty
                        }).ToList();

                    var listEmloyeeIdOfOrder = listOrderNew.Select(x => x.Seller).ToList();
                    if (listEmloyeeIdOfOrder.Count > 0)
                    {
                        var listEmployeeOfOrder = listCommonEmployee
                            .Where(x => listEmloyeeIdOfOrder.Contains(x.EmployeeId))
                            .ToList();

                        listOrderNew.ForEach(item =>
                        {
                            var emp = listEmployeeOfOrder.FirstOrDefault(x => x.EmployeeId == item.Seller);
                            item.SellerName = emp == null ? "" : emp.EmployeeName.Trim();
                        });
                    }

                    var listCustomerIdOfOrder = listOrderNew.Select(x => x.CustomerId).ToList();
                    if (listCustomerIdOfOrder.Count > 0)
                    {
                        var listCustomerOfOrder = listCommonCustomer
                            .Where(x => listCustomerIdOfOrder.Contains(x.CustomerId)).ToList();

                        listOrderNew.ForEach(item =>
                        {
                            var customer = listCustomerOfOrder.FirstOrDefault(x => x.CustomerId == item.CustomerId);
                            item.CustomerName = customer == null ? "" : customer.CustomerName.Trim();
                        });
                    }

                    #endregion

                    #region Các khách hàng có sinh nhật trong tuần hiện tại

                    #region Khách hàng doanh nghiệp lấy theo ngày cấp (Ngày thành lập)

                    var _yearNow = DateTime.Now.Year;
                    listCusBirthdayOfWeek_1 = _listCommonCustomer.Where(x =>
                        x.CustomerType == 1 && x.BusinessRegistrationDate != null && (new DateTime(_yearNow,
                            x.BusinessRegistrationDate.Value.Month, x.BusinessRegistrationDate.Value.Day)) >=
                        firstDateOfWeek &&
                        (new DateTime(_yearNow, x.BusinessRegistrationDate.Value.Month,
                            x.BusinessRegistrationDate.Value.Day)) <= lastDateOfWeek).Select(y => new CustomerEntityModel
                            {
                                CustomerId = y.CustomerId,
                                CustomerName = y.CustomerName,
                                CustomerPhone = "",
                                CustomerEmail = "",
                                PersonInChargeId = y.PersonInChargeId,
                                PicName = "",
                                PicContactId = null,
                                DateOfBirth = y.BusinessRegistrationDate
                            }).ToList();
                    #endregion

                    #region Khách hàng cá nhân lấy theo ngày sinh nhật

                    var listCusBirthdayOfWeek_2 = new List<CustomerEntityModel>();
                    listCusBirthdayOfWeek_2 =
                        _listCommonCustomer.Where(x => x.CustomerType == 2 && x.Active == true).Select(y =>
                            new CustomerEntityModel
                            {
                                CustomerId = y.CustomerId,
                                CustomerName = y.CustomerName,
                                CustomerPhone = "",
                                CustomerEmail = "",
                                PersonInChargeId = y.PersonInChargeId,
                                PicName = "",
                                PicContactId = null,
                                DateOfBirth = null
                            }).ToList();
                    var listCusId = listCusBirthdayOfWeek_2.Select(x => x.CustomerId).ToList();

                    if (listCusId.Count > 0)
                    {
                        var yearNow = DateTime.Now.Year;
                        var listCusContact = listCommonContact.Where(x =>
                                listCusId.Contains(x.ObjectId) && x.DateOfBirth != null &&
                                (new DateTime(yearNow, x.DateOfBirth.Value.Month, x.DateOfBirth.Value.Day)) >=
                                firstDateOfWeek &&
                                (new DateTime(yearNow, x.DateOfBirth.Value.Month, x.DateOfBirth.Value.Day)) <=
                                lastDateOfWeek)
                            .ToList();
                        var listCusIdByContact = listCusContact.Select(x => x.ObjectId).Distinct().ToList();

                        if (listCusIdByContact.Count > 0)
                        {
                            listCusBirthdayOfWeek_2 = listCusBirthdayOfWeek_2
                                .Where(x => listCusIdByContact.Contains(x.CustomerId)).ToList();

                            listCusBirthdayOfWeek_2.ForEach(item =>
                            {
                                var cus_contact = listCusContact.FirstOrDefault(x => x.ObjectId == item.CustomerId);
                                item.DateOfBirth = cus_contact == null ? null : cus_contact.DateOfBirth;
                            });
                        }
                        else
                        {
                            listCusBirthdayOfWeek_2 = new List<CustomerEntityModel>();
                        }
                    }

                    #endregion

                    #region Ghép 2 nhóm khách hàng lại với nhau

                    listCusBirthdayOfWeek_1.AddRange(listCusBirthdayOfWeek_2);

                    listCusBirthdayOfWeek_1 =
                        listCusBirthdayOfWeek_1.OrderByDescending(x => x.DateOfBirth).ToList();

                    listCusBirthdayOfWeek_1.ForEach(item =>
                    {
                        var cus_cont = listCommonContact.FirstOrDefault(x =>
                            x.ObjectId == item.CustomerId && x.ObjectType == "CUS");
                        var emp = listCommonEmployee.FirstOrDefault(x => x.EmployeeId == item.PersonInChargeId);

                        item.CustomerPhone =
                            cus_cont == null ? "" : (cus_cont.Phone == null ? "" : (cus_cont.Phone.Trim()));
                        item.CustomerEmail =
                            cus_cont == null ? "" : (cus_cont.Email == null ? "" : (cus_cont.Email.Trim()));
                        item.PicName = emp == null ? "" : emp.EmployeeName.Trim();
                    });

                    #endregion

                    #endregion

                    #region Các nhân viên có sinh nhật trong tuần

                    listEmployeeBirthDayOfWeek = listCommonEmployee.Where(x => x.Active == true).Select(y =>
                        new EmployeeEntityModel
                        {
                            EmployeeId = y.EmployeeId,
                            EmployeeName = y.EmployeeName,
                            OrganizationId = y.OrganizationId,
                            OrganizationName = "",
                            PositionId = y.PositionId,
                            PositionName = "",
                            DateOfBirth = null,
                            ContactId = null
                        }).ToList();

                    var listEmpId = listEmployeeBirthDayOfWeek.Select(x => x.EmployeeId).ToList();
                    if (listEmpId.Count > 0)
                    {
                        var _year = DateTime.Now.Year;
                        var listEmpContact = listCommonContact
                            .Where(x => listEmpId.Contains(x.ObjectId) && x.ObjectType == "EMP" && x.DateOfBirth != null &&
                                        (new DateTime(_year, x.DateOfBirth.Value.Month, x.DateOfBirth.Value.Day)) >=
                                        firstDateOfWeek &&
                                        (new DateTime(_year, x.DateOfBirth.Value.Month, x.DateOfBirth.Value.Day)) <=
                                        lastDateOfWeek).ToList();
                        var listEmpContactId = listEmpContact.Select(x => x.ObjectId).Distinct().ToList();

                        if (listEmpContactId.Count > 0)
                        {
                            listEmployeeBirthDayOfWeek = listEmployeeBirthDayOfWeek
                                .Where(x => listEmpContactId.Contains(x.EmployeeId.Value)).ToList();

                            listEmployeeBirthDayOfWeek.ForEach(item =>
                            {
                                var emp_cont = listEmpContact.FirstOrDefault(x => x.ObjectId == item.EmployeeId);
                                item.DateOfBirth = emp_cont == null ? null : emp_cont.DateOfBirth;
                                item.ContactId = emp_cont.ContactId;

                                var organization =
                                    listCommonOrganization.FirstOrDefault(x => x.OrganizationId == item.OrganizationId);
                                var position = listCommonPosition.FirstOrDefault(x => x.PositionId == item.PositionId);

                                item.OrganizationName = organization == null ? "" : (organization.OrganizationName.Trim());
                                item.PositionName = position == null ? "" : (position.PositionName.Trim());
                            });

                            listEmployeeBirthDayOfWeek =
                                listEmployeeBirthDayOfWeek.OrderByDescending(z => z.DateOfBirth).ToList();
                        }
                        else
                        {
                            listEmployeeBirthDayOfWeek = new List<EmployeeEntityModel>();
                        }
                    }

                    #endregion

                    #region Các lịch hẹn trong tuần
                    listCustomerMeeting.ForEach(item =>
                    {
                        var cus = listCommonCustomer.FirstOrDefault(x => x.CustomerId == item.CustomerId);
                        var emp = listCommonEmployee.FirstOrDefault(x => x.EmployeeId == item.EmployeeId);

                        item.CustomerName = cus == null ? "" : (cus.CustomerName.Trim());
                        item.EmployeeName = emp == null ? "" : (emp.EmployeeName.Trim());
                        item.CreateByName = GetCreateByName(item.CreatedById);
                        var cusPersonInChargeId = listCommonCustomer.FirstOrDefault(c => c.CustomerId == item.CustomerId)?.PersonInChargeId ?? Guid.Empty;
                        if (listAllUserId.Contains(item.CreatedById) || listAllEmpId.Contains(cusPersonInChargeId))
                            item.IsShowLink = true;
                    });

                    listLeadMeeting.ForEach(item =>
                    {
                        item.LeadName = listCommonContact.FirstOrDefault(c => c.ObjectId == item.LeadId && c.ObjectType == "LEA")?.FirstName ?? "";
                        item.EmployeeName = listCommonEmployee.FirstOrDefault(x => x.EmployeeId == item.EmployeeId)?.EmployeeName ?? "";
                        item.CreateByName = GetCreateByName(item.CreatedById);
                        var leadPersonInChargeId = listCommonLead.FirstOrDefault(c => c.LeadId == item.LeadId)?.PersonInChargeId ?? Guid.Empty;
                        if (listAllUserId.Contains(item.CreatedById) || listAllEmpId.Contains(leadPersonInChargeId))
                            item.IsShowLink = true;
                    });
                    #endregion
                }
                else
                {
                    var _listCommonCustomer = listCommonCustomer.Where(x =>
                        (x.PersonInChargeId != null && x.PersonInChargeId == employee.EmployeeId) ||
                        (x.PersonInChargeId == null && x.CreatedById == user.UserId)).ToList();
                    var _listCommonOrder = listCommonOrder.Where(x => x.Seller == employee.EmployeeId).ToList();

                    #region Lấy 5 báo giá mới

                    var categoryTypeCodeOfQuote = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "TGI");
                    var quoteStatusNewId = context.Category.FirstOrDefault(x =>
                        x.CategoryTypeId == categoryTypeCodeOfQuote.CategoryTypeId && x.CategoryCode == "MTA").CategoryId;

                    // id user dang signin
                    var employeeId = employee.EmployeeId;

                    var listQuoteParticipantMappingId =
                        context.QuoteParticipantMapping.Where(x => x.EmployeeId == employeeId).Select(y => y.QuoteId).ToList();

                    listQuote = context.Quote.Where(x =>
                            x.StatusId == quoteStatusNewId &&
                            (x.Seller == employee.EmployeeId || x.PersonInChargeId == employee.EmployeeId) &&
                            x.Active == true ||
                            (listQuoteParticipantMappingId.Contains(x.QuoteId)))
                        .OrderByDescending(z => z.QuoteDate).Take(5).Select(y => new QuoteEntityModel
                        {
                            QuoteId = y.QuoteId,
                            QuoteCode = y.QuoteCode,
                            Amount = AmountAfterDiscount(y.Amount, y.DiscountType, y.DiscountValue),
                            ObjectTypeId = y.ObjectTypeId,
                            ObjectType = y.ObjectType,
                            CustomerName = "",
                            CustomerContactId = null,
                            Seller = y.Seller,
                            SellerName = "",
                            QuoteDate = y.QuoteDate,
                            DiscountType = y.DiscountType,
                            DiscountValue = y.DiscountValue,
                            TotalAmountAfterVat = CalculateTotalAmountAfterVat(y.QuoteId, y.DiscountType, y.DiscountValue, y.Vat, listQuoteDetail, listQuoteCostDetail, listPromotionObjectApply, appName)
                        }).ToList();

                    listQuote.ForEach(item =>
                    {
                        if (item.ObjectType == "LEAD")
                        {
                            var lead = listCommonContact.FirstOrDefault(x =>
                                x.ObjectId == item.ObjectTypeId && x.ObjectType == "LEA");
                            item.CustomerName = lead == null
                                ? ""
                                : ((lead.FirstName == null ? "" : lead.FirstName.Trim()) + " " +
                                   (lead.LastName == null ? "" : lead.LastName.Trim()));
                            item.CustomerContactId = lead == null ? Guid.Empty : (lead.ContactId);
                        }
                        else if (item.ObjectType == "CUSTOMER")
                        {
                            var cus = listCommonContact.FirstOrDefault(x =>
                                x.ObjectId == item.ObjectTypeId && x.ObjectType == "CUS");
                            item.CustomerName = cus == null
                                ? ""
                                : ((cus.FirstName == null ? "" : cus.FirstName.Trim()) + " " +
                                   (cus.LastName == null ? "" : cus.LastName.Trim()));
                            item.CustomerContactId = cus == null ? Guid.Empty : (cus.ContactId);
                        }
                    });

                    var listSellerId = listQuote.Select(x => x.Seller).ToList();
                    if (listSellerId.Count > 0)
                    {
                        var listSellerOfQuote =
                            listCommonEmployee.Where(x => listSellerId.Contains(x.EmployeeId)).ToList();
                        listQuote.ForEach(item =>
                        {
                            var emp = listSellerOfQuote.FirstOrDefault(x => x.EmployeeId == item.Seller);
                            if (emp != null)
                            {
                                item.SellerName = emp.EmployeeName.Trim();
                            }
                        });
                    }

                    #endregion

                    #region Lấy 5 khách hàng mới

                    var categoryTypeCodeCustomer =
                        context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "THA");
                    var statusCustomerNewId = context.Category.FirstOrDefault(x =>
                            x.CategoryTypeId == categoryTypeCodeCustomer.CategoryTypeId && x.CategoryCode == "HDO")
                        .CategoryId;

                    listCustomer = _listCommonCustomer.Where(x =>
                            x.StatusId == statusCustomerNewId && x.Active == true)
                        .OrderByDescending(z => z.CreatedDate).Take(5).Select(y => new CustomerEntityModel
                        {
                            CustomerId = y.CustomerId,
                            CustomerName = y.CustomerName,
                            CustomerPhone = "",
                            CustomerEmail = "",
                            PersonInChargeId = y.PersonInChargeId,
                            PicName = "",
                            PicContactId = null
                        }).ToList();

                    var listCustomerId = listCustomer.Select(x => x.CustomerId).ToList();
                    var listEmployeeId = listCustomer.Select(x => x.PersonInChargeId).ToList();

                    if (listCustomerId.Count > 0)
                    {
                        var listContactCustomer = listCommonContact
                            .Where(x => listCustomerId.Contains(x.ObjectId) && x.ObjectType == "CUS").ToList();

                        listCustomer.ForEach(item =>
                        {
                            var cus_contact = listContactCustomer.FirstOrDefault(x => x.ObjectId == item.CustomerId);
                            item.CustomerPhone = cus_contact == null
                                ? ""
                                : (cus_contact.Phone == null ? "" : cus_contact.Phone.Trim());
                            item.CustomerEmail = cus_contact == null
                                ? ""
                                : (cus_contact.Email == null ? "" : cus_contact.Email.Trim());
                        });
                    }

                    if (listEmployeeId.Count > 0)
                    {
                        var listContactEmployee = listCommonContact
                            .Where(x => listEmployeeId.Contains(x.ObjectId) && x.ObjectType == "EMP").ToList();

                        listCustomer.ForEach(item =>
                        {
                            var emp_contact =
                                listContactEmployee.FirstOrDefault(x => x.ObjectId == item.PersonInChargeId);
                            item.PicName = emp_contact == null
                                ? ""
                                : (emp_contact.FirstName ?? "" + emp_contact.LastName ?? "");
                            if (emp_contact != null)
                            {
                                item.PicContactId = emp_contact.ContactId;
                            }
                        });
                    }

                    #endregion

                    #region Lấy 5 đơn hàng mới

                    listOrderNew = _listCommonOrder.Where(x =>
                            listOrderStatusId.Contains(x.StatusId.Value) && x.Active == true).OrderByDescending(z => z.OrderDate).Take(5)
                        .Select(y => new CustomerOrderEntityModel
                        {
                            OrderId = y.OrderId,
                            OrderCode = y.OrderCode,
                            CustomerId = y.CustomerId.Value,
                            CustomerName = "",
                            Amount = AmountAfterDiscount(y.Amount, y.DiscountType, y.DiscountValue),
                            Seller = y.Seller,
                            SellerName = "",
                            SellerContactId = Guid.Empty
                        }).ToList();

                    var listEmloyeeIdOfOrder = listOrderNew.Select(x => x.Seller).ToList();
                    if (listEmloyeeIdOfOrder.Count > 0)
                    {
                        var listEmployeeOfOrder = listCommonEmployee
                            .Where(x => listEmloyeeIdOfOrder.Contains(x.EmployeeId))
                            .ToList();

                        listOrderNew.ForEach(item =>
                        {
                            var emp = listEmployeeOfOrder.FirstOrDefault(x => x.EmployeeId == item.Seller);
                            item.SellerName = emp == null ? "" : emp.EmployeeName.Trim();
                        });
                    }

                    var listCustomerIdOfOrder = listOrderNew.Select(x => x.CustomerId).ToList();
                    if (listCustomerIdOfOrder.Count > 0)
                    {
                        var listCustomerOfOrder = listCommonCustomer
                            .Where(x => listCustomerIdOfOrder.Contains(x.CustomerId)).ToList();

                        listOrderNew.ForEach(item =>
                        {
                            var customer = listCustomerOfOrder.FirstOrDefault(x => x.CustomerId == item.CustomerId);
                            item.CustomerName = customer == null ? "" : customer.CustomerName.Trim();
                        });
                    }

                    #endregion

                    #region Các lịch hẹn trong tuần
                    listCustomerMeeting.ForEach(item =>
                    {
                        var cus = listCommonCustomer.FirstOrDefault(x => x.CustomerId == item.CustomerId);
                        var emp = listCommonEmployee.FirstOrDefault(x => x.EmployeeId == item.EmployeeId);

                        item.CustomerName = cus == null ? "" : (cus.CustomerName.Trim());
                        item.EmployeeName = emp == null ? "" : (emp.EmployeeName.Trim());
                        item.CreateByName = GetCreateByName(item.CreatedById);
                        var cusPersonInChargeId = listCommonCustomer.FirstOrDefault(c => c.CustomerId == item.CustomerId)?.PersonInChargeId ?? Guid.Empty;
                        if (item.CreatedById == user.UserId || employee.EmployeeId == cusPersonInChargeId)
                            item.IsShowLink = true;
                    });

                    listLeadMeeting.ForEach(item =>
                    {
                        item.LeadName = listCommonContact.FirstOrDefault(c => c.ObjectId == item.LeadId && c.ObjectType == "LEA")?.FirstName ?? "";
                        item.EmployeeName = listCommonEmployee.FirstOrDefault(x => x.EmployeeId == item.EmployeeId)?.EmployeeName ?? "";
                        var leadPersonInChargeId = listCommonLead.FirstOrDefault(c => c.LeadId == item.LeadId)?.PersonInChargeId ?? Guid.Empty;
                        item.CreateByName = GetCreateByName(item.CreatedById);
                        if (item.CreatedById == user.UserId || employee.EmployeeId == leadPersonInChargeId)
                            item.IsShowLink = true;
                    });
                    #endregion
                }

                listQuote.ForEach(x =>
                {
                    x.TotalAmount = CalculateTotalAmount(x.QuoteId, x.DiscountType, x.DiscountValue,
                        x.TotalAmountAfterVat, listPromotionObjectApply);
                });

                var listParticipants = context.Employee.Where(c => c.Active == true).Select(
                    c => new EmployeeEntityModel
                    {
                        EmployeeId = c.EmployeeId,
                        EmployeeCode = c.EmployeeCode,
                        EmployeeName = c.EmployeeName
                    }).ToList();

                var listEmployee = listCommonEmployee.Where(x => x.Active == true).Select(y => new EmployeeEntityModel
                {
                    EmployeeId = y.EmployeeId,
                    EmployeeName = y.EmployeeName,
                    EmployeeCode = y.EmployeeCode,
                    EmployeeCodeName = y.EmployeeCode + " - " + y.EmployeeName
                }).OrderBy(z => z.EmployeeName).ToList();

                return new GetDataDashboardHomeResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success",
                    TotalSalesOfWeek = totalSalesOfWeek,
                    TotalSalesOfMonth = totalSalesOfMonth,
                    TotalSalesOfQuarter = totalSalesOfQuarter,
                    TotalSalesOfWeekPress = totalSalesOfWeekPress,
                    TotalSalesOfMonthPress = totalSalesOfMonthPress,
                    TotalSalesOfQuarterPress = totalSalesOfQuarterPress,
                    TotalSalesOfYear = totalSalesOfYear,
                    TotalSalesOfYearPress = totalSalesOfYearPress,
                    ListQuote = listQuote,
                    ListCustomer = listCustomer,
                    ListOrderNew = listOrderNew,
                    ListCustomerMeeting = listCustomerMeeting,
                    ListCusBirthdayOfWeek = listCusBirthdayOfWeek_1,
                    ListEmployeeBirthDayOfWeek = listEmployeeBirthDayOfWeek,
                    ListLeadMeeting = listLeadMeeting,
                    ListParticipants = listParticipants,
                    ListEmployee = listEmployee,
                    ChiTieuDoanhThuTuan = chiTieuDoanhThuTuan,
                    ChiTieuDoanhThuThang = chiTieuDoanhThuThang,
                    ChiTieuDoanhThuQuy = chiTieuDanhThuQuy,
                    ChiTieuDoanhThuName = chiTieuDoanhThuNam
                };
            }
            catch (Exception e)
            {
                return new GetDataDashboardHomeResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        //Tổng tiền sau thuế = Tổng GTHH bán ra + Tổng thành tiền nhân công - Tổng chiết khấu - Tổng khuyến mại + Tổng thuế VAT + Tổng chi phí
        //Tổng tiền sau thuế = Tổng GTHH bán ra  + Tổng thuế VAT + Tổng chi phí
        private decimal? CalculateTotalAmountAfterVat(Guid quoteId, bool? discountType, decimal? discountValue, decimal? vat, List<QuoteDetail> listQuoteDetail, List<QuoteCostDetail> listQuoteCostDetail, List<PromotionObjectApply> listPromotionObjectApply, string appName)
        {
            decimal? result = 0;
            decimal? amount = 0;
            decimal? totalSumAmountLabor = 0;
            decimal? totalAmountDiscount = 0;
            decimal? totalAmountPromotion = 0;
            decimal? totalAmountVat = 0;
            decimal? amountPriceCost = 0;
            decimal? amountCostNotInclude = 0;
            bool hasDiscount = false;
            bool hasVat = false;

            if (appName != null)
            {
                if (appName == "VNS")
                {
                    var quoteDetailList = listQuoteDetail.Where(x => x.Active == true && x.QuoteId == quoteId).ToList();

                    quoteDetailList.ForEach(x =>
                    {
                        if (x.DiscountValue > 0)
                        {
                            hasDiscount = true;
                        }

                        if (x.Vat > 0)
                        {
                            hasVat = true;
                        }

                        var price = x.Quantity * x.UnitPrice * x.ExchangeRate;
                        decimal? amountDiscount = 0;
                        var sumAmountLabor = x.UnitLaborPrice * x.Quantity;

                        if (x.DiscountType == true)
                        {
                            amountDiscount = price * x.DiscountValue / 100;
                        }
                        else
                        {
                            amountDiscount = x.DiscountValue;
                        }

                        var amountVAT = (price - amountDiscount + sumAmountLabor) * x.Vat / 100;

                        //Tổng GTHH bán ra
                        amount += x.Quantity * x.UnitPrice * x.ExchangeRate;
                        //Tổng thành tiền nhân công
                        totalSumAmountLabor += sumAmountLabor;
                        //Tổng chiết khấu
                        totalAmountDiscount += amountDiscount;
                        //Tổng thuế VAT
                        totalAmountVat += amountVAT;
                    });

                    // Tổng khuyến mại
                    var promotionObjectApplyList = listPromotionObjectApply.Where(x => x.ObjectId == quoteId && x.ObjectType == "QUOTE").ToList();

                    promotionObjectApplyList.ForEach(x =>
                    {
                        if (x.ProductId == null)
                        {
                            totalAmountPromotion += x.Amount;
                        }
                    });

                    //Tổng chi phí
                    var quoteCostDetailList = listQuoteCostDetail.Where(x => x.Active == true && x.QuoteId == quoteId).ToList();

                    quoteCostDetailList.ForEach(item =>
                    {
                        var price = item.UnitPrice * item.Quantity;
                        amountPriceCost += price;
                    });

                    if (!hasDiscount)
                    {
                        /*Tổng thành tiền chiết khấu*/
                        if (discountType == true)
                        {
                            totalAmountDiscount = amount * discountValue / 100;
                        }
                        else
                        {
                            totalAmountDiscount = discountValue;
                        }
                        /*End*/
                    }

                    if (!hasVat)
                    {
                        totalAmountVat = (amount + totalSumAmountLabor - totalAmountDiscount - totalAmountPromotion) * vat / 100;
                    }

                    result = amount + totalSumAmountLabor - totalAmountDiscount - totalAmountPromotion +
                             totalAmountVat + amountPriceCost;
                }
                else
                {
                    var quoteDetailList = listQuoteDetail.Where(x => x.Active == true && x.QuoteId == quoteId).ToList();

                    quoteDetailList.ForEach(x =>
                    {

                        var price = x.Quantity * x.UnitPrice * x.ExchangeRate;
                        var sumLabor = x.UnitLaborNumber * x.UnitLaborPrice * x.ExchangeRate;
                        decimal? amountDiscount = 0;

                        if (x.DiscountType == true)
                        {
                            amountDiscount = (price + sumLabor) * x.DiscountValue / 100;
                        }
                        else
                        {
                            amountDiscount = x.DiscountValue;
                        }

                        var amountVAT = (price + sumLabor - amountDiscount) * x.Vat / 100;

                        //Tổng GTHH bán ra
                        amount += ((x.Quantity * x.UnitPrice * x.ExchangeRate + sumLabor) - amountDiscount);
                        //Tổng thuế VAT
                        totalAmountVat += amountVAT;
                    });


                    //Tổng chi phí
                    var quoteCostDetailList = listQuoteCostDetail.Where(x => x.Active == true && x.QuoteId == quoteId).ToList();

                    quoteCostDetailList.ForEach(item =>
                    {
                        var price = item.UnitPrice * item.Quantity;
                        if (item.IsInclude == false)
                        {
                            amountCostNotInclude += price;
                        }
                        amountPriceCost += price;
                    });


                    result = amount + totalAmountVat + amountCostNotInclude;
                }
            }

            return result;
        }

        //Tổng tiền thanh toán = Tổng tiền sau thuế - Tổng Thành tiền chiết khấu - Tổng Thành tiền của Phiếu giảm giá tại tab Khuyến mãi
        private decimal? CalculateTotalAmount(Guid quoteId, bool? discountType, decimal? discountValue, decimal? totalAmountAfterVat, List<PromotionObjectApply> listPromotionObjectApply)
        {
            decimal? totalAmountPromotion = 0;
            decimal? discountVal = 0;

            var listPromotion = listPromotionObjectApply.Where(x => x.ObjectId == quoteId).ToList();


            listPromotion.ForEach(x =>
            {
                if (x.ProductId == null)
                {
                    if (!x.LoaiGiaTri)
                    {
                        totalAmountPromotion += x.GiaTri * x.SoLuongTang;
                    }
                    else
                    {
                        totalAmountPromotion += (totalAmountAfterVat * x.GiaTri / 100) * x.SoLuongTang;
                    }
                }
            });

            if (discountType == true)
            {
                discountVal = (totalAmountAfterVat * discountValue) / 100;
            }
            else
            {
                discountVal = discountValue;
            }

            return totalAmountAfterVat - discountVal - totalAmountPromotion;
        }

        private DateTime FirstDateOfWeek()
        {
            DateTime dateNow = DateTime.Now;
            DateTime dateReturn = dateNow;
            var dayNow = dateNow.DayOfWeek;
            switch (dayNow)
            {
                case DayOfWeek.Monday:
                    dateReturn = dateNow;
                    break;
                case DayOfWeek.Tuesday:
                    dateReturn = dateNow.AddDays(-1);
                    break;
                case DayOfWeek.Wednesday:
                    dateReturn = dateNow.AddDays(-2);
                    break;
                case DayOfWeek.Thursday:
                    dateReturn = dateNow.AddDays(-3);
                    break;
                case DayOfWeek.Friday:
                    dateReturn = dateNow.AddDays(-4);
                    break;
                case DayOfWeek.Saturday:
                    dateReturn = dateNow.AddDays(-5);
                    break;
                case DayOfWeek.Sunday:
                    dateReturn = dateNow.AddDays(-6);
                    break;
            }
            int hour = dateNow.Hour;
            int minute = dateNow.Minute;
            int second = dateNow.Second;
            dateReturn = dateReturn.AddHours(-hour).AddMinutes(-minute).AddSeconds(-second);
            var _day = dateReturn.Day;
            var _month = dateReturn.Month;
            var _year = dateReturn.Year;
            dateReturn = new DateTime(_year, _month, _day, 0, 0, 0, 0);
            return dateReturn;
        }

        private DateTime LastDateOfWeek()
        {
            DateTime dateNow = DateTime.Now;
            DateTime dateReturn = dateNow;
            var dayNow = dateNow.DayOfWeek;
            switch (dayNow)
            {
                case DayOfWeek.Monday:
                    dateReturn = dateNow.AddDays(7);
                    break;
                case DayOfWeek.Tuesday:
                    dateReturn = dateNow.AddDays(5);
                    break;
                case DayOfWeek.Wednesday:
                    dateReturn = dateNow.AddDays(4);
                    break;
                case DayOfWeek.Thursday:
                    dateReturn = dateNow.AddDays(3);
                    break;
                case DayOfWeek.Friday:
                    dateReturn = dateNow.AddDays(2);
                    break;
                case DayOfWeek.Saturday:
                    dateReturn = dateNow.AddDays(1);
                    break;
                case DayOfWeek.Sunday:
                    dateReturn = dateNow;
                    break;
            }
            int hour = 23 - dateNow.Hour;
            int minute = 59 - dateNow.Minute;
            int second = 59 - dateNow.Second;
            dateReturn = dateReturn.AddHours(hour).AddMinutes(minute).AddSeconds(second);
            var _day = dateReturn.Day;
            var _month = dateReturn.Month;
            var _year = dateReturn.Year;
            dateReturn = new DateTime(_year, _month, _day, 23, 59, 59, 999);
            return dateReturn;
        }

        private DateTime FirstDateOfMonth()
        {
            DateTime dateNow = DateTime.Now;
            DateTime dateReturn = dateNow;

            dateReturn = new DateTime(dateNow.Year, dateNow.Month, 1, 0, 0, 0, 0);

            return dateReturn;
        }

        private DateTime LastDateOfMonth()
        {
            DateTime dateNow = DateTime.Now;
            DateTime dateReturn = dateNow;

            var firstDateOfMonth = FirstDateOfMonth();
            dateReturn = firstDateOfMonth.AddMonths(1).AddDays(-1);
            dateReturn = new DateTime(dateReturn.Year, dateReturn.Month, dateReturn.Day, 23, 59, 59, 999);

            return dateReturn;
        }

        private DateOfQuarter GetDateOfQuarter(DateTime dateNow)
        {
            //DateTime dateNow = DateTime.Now;
            DateOfQuarter dateOfQuarter = new DateOfQuarter();

            int quarter = 0;

            //Tìm quý hiện tại
            var month = dateNow.Month;

            if (month == 1 || month == 2 || month == 3)
            {
                quarter = 1;
            }
            else if (month == 4 || month == 5 || month == 6)
            {
                quarter = 2;
            }
            else if (month == 7 || month == 8 || month == 9)
            {
                quarter = 3;
            }
            else
            {
                quarter = 4;
            }

            switch (quarter)
            {
                case 1:
                    dateOfQuarter.FirstDateOfQuarter = new DateTime(dateNow.Year, 1, 1, 0, 0, 0, 0);
                    dateOfQuarter.LastDateOfQuarter = new DateTime(dateNow.Year, 4, 1, 23, 59, 59, 999);
                    dateOfQuarter.LastDateOfQuarter = dateOfQuarter.LastDateOfQuarter.AddDays(-1);
                    break;

                case 2:
                    dateOfQuarter.FirstDateOfQuarter = new DateTime(dateNow.Year, 4, 1, 0, 0, 0, 0);
                    dateOfQuarter.LastDateOfQuarter = new DateTime(dateNow.Year, 7, 1, 23, 59, 59, 999);
                    dateOfQuarter.LastDateOfQuarter = dateOfQuarter.LastDateOfQuarter.AddDays(-1);
                    break;

                case 3:
                    dateOfQuarter.FirstDateOfQuarter = new DateTime(dateNow.Year, 7, 1, 0, 0, 0, 0);
                    dateOfQuarter.LastDateOfQuarter = new DateTime(dateNow.Year, 10, 1, 23, 59, 59, 999);
                    dateOfQuarter.LastDateOfQuarter = dateOfQuarter.LastDateOfQuarter.AddDays(-1);
                    break;

                case 4:
                    dateOfQuarter.FirstDateOfQuarter = new DateTime(dateNow.Year, 10, 1, 0, 0, 0, 0);
                    dateOfQuarter.LastDateOfQuarter = new DateTime(dateNow.Year + 1, 1, 1, 23, 59, 59, 999);
                    dateOfQuarter.LastDateOfQuarter = dateOfQuarter.LastDateOfQuarter.AddDays(-1);
                    break;
            }

            return dateOfQuarter;
        }

        private decimal AmountAfterDiscount(decimal? Amount, bool? DiscountType, decimal? DiscountValue)
        {
            decimal value;

            if (DiscountType.Value == true)
            {
                value = Amount.Value - ((Amount.Value * DiscountValue.Value) / 100);
            }
            else
            {
                value = Amount.Value - DiscountValue.Value;
            }

            return value;
        }

        public CheckReceiptOrderHistoryResult CheckReceiptOrderHistory(CheckReceiptOrderHistoryParameter parameter)
        {
            try
            {
                var orderById = context.CustomerOrder.FirstOrDefault(c => c.OrderId == parameter.OrderId);
                if (orderById != null)
                {
                    //var listReceiptInvoiceMapping = context.ReceiptInvoiceMapping.Where(r => r.ObjectId == orderById.CustomerId).Select(r=>r.ReceiptInvoiceId).ToList();

                    //var receiptOrder = context.ReceiptOrderHistory.FirstOrDefault(ro => ro.OrderId == parameter.OrderId && listReceiptInvoiceMapping.Contains(ro.ObjectId));
                    //if (receiptOrder != null)
                    //{
                    //    if (parameter.MoneyOrder <= receiptOrder.AmountCollected)
                    //    {
                    //        return new CheckReceiptOrderHistoryResult()
                    //        {
                    //            Status = true,
                    //            CheckReceiptOrderHistory = false,
                    //            Message = ""
                    //        };
                    //    }
                    //}
                    var receiptOrder = context.ReceiptOrderHistory.Where(ro => ro.OrderId == parameter.OrderId).Sum(ro => ro.AmountCollected);
                    if (parameter.MoneyOrder <= receiptOrder)
                    {
                        return new CheckReceiptOrderHistoryResult()
                        {
                            StatusCode = HttpStatusCode.OK,
                            CheckReceiptOrderHistory = false,
                            MessageCode = ""
                        };
                    }

                }
                return new CheckReceiptOrderHistoryResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    CheckReceiptOrderHistory = true,
                    MessageCode = ""
                };
            }
            catch (Exception e)
            {
                return new CheckReceiptOrderHistoryResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetMasterDataOrderServiceCreateResult GetMasterDataOrderServiceCreate(GetMasterDataOrderServiceCreateParameter parameter)
        {
            try
            {
                var listProductCategory = new List<ProductCategoryEntityModel>();
                var listProduct = new List<ProductEntityModel>();
                var listLocalAddress = new List<LocalAddressEntityModel>();

                var user = context.User.FirstOrDefault(x => x.UserId == parameter.UserId);
                var employee = context.Employee.FirstOrDefault(x => x.EmployeeId == user.EmployeeId);

                //Lấy list Khu vực theo nhân viên đăng nhập
                var listComonLocalPoint = context.LocalPoint.ToList();
                listLocalAddress = context.LocalAddress.Where(x => x.BranchId == employee.BranchId).Select(y =>
                    new LocalAddressEntityModel
                    {
                        LocalAddressId = y.LocalAddressId,
                        LocalAddressCode = y.LocalAddressCode,
                        LocalAddressName = y.LocalAddressName,

                        //Lấy list Điểm thuộc khu vực
                        ListLocalPoint = listComonLocalPoint.Where(p => p.LocalAddressId == y.LocalAddressId).Select(
                            q => new LocalPointEntityModel
                            {
                                LocalPointId = q.LocalPointId,
                                LocalPointCode = q.LocalPointCode,
                                LocalPointName = q.LocalPointName,
                                StatusId = q.StatusId,
                                LocalAddressId = q.LocalAddressId,
                                StatusName = q.StatusId == 1 ? "Sẵn sàng" : "Đang dùng"
                            }).OrderBy(z => z.LocalPointCode).ToList()
                    }).ToList();

                //Lấy list trạng thái của Đơn hàng
                var listOrderStatus = context.OrderStatus.Where(x => x.Active == true).Select(y => new OrderStatus
                {
                    OrderStatusId = y.OrderStatusId,
                    OrderStatusCode = y.OrderStatusCode
                }).ToList();
                var listOrderStatusEntityModel = new List<OrderStatusEntityModel>();
                listOrderStatus.ForEach(item =>
                {
                    listOrderStatusEntityModel.Add(new OrderStatusEntityModel(item));
                });
                //Lấy list Đơn vị tiền của hệ thống
                var moneyUnitType = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "DTI")?.CategoryTypeId;
                var listMoneyUnit = context.Category.Where(x => x.Active == true && x.CategoryTypeId == moneyUnitType)
                    .Select(y => new CategoryEntityModel
                    {
                        CategoryId = y.CategoryId,
                        CategoryCode = y.CategoryCode
                    }).ToList();

                //Lấy nhóm khách hàng điềm đạm
                var customerGroupCategoryType = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "NHA")
                    ?.CategoryTypeId;
                var customerGroupId = context.Category.FirstOrDefault(x =>
                    x.CategoryCode == "TPH" && x.CategoryTypeId == customerGroupCategoryType)?.CategoryId;

                var listPriceProduct = context.PriceProduct.Where(x =>
                        x.Active && x.EffectiveDate.Date <= DateTime.Now.Date &&
                        x.CustomerGroupCategory == customerGroupId)
                    .ToList();

                //list đơn vị tính
                var productUnitCategoryType = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "DNH")
                    ?.CategoryTypeId;
                var listProductUnit = context.Category.Where(x => x.CategoryTypeId == productUnitCategoryType).ToList();

                listProductCategory = context.ProductCategory.Where(x => x.Active == true).Select(y =>
                    new ProductCategoryEntityModel
                    {
                        ProductCategoryId = y.ProductCategoryId,
                        ProductCategoryCode = y.ProductCategoryCode,
                        ProductCategoryName = y.ProductCategoryName,
                        ProductCategoryLevel = y.ProductCategoryLevel
                    }).ToList();

                listProduct = context.Product.Where(x => x.Active == true).Select(y => new ProductEntityModel
                {
                    ProductId = y.ProductId,
                    ProductCode = y.ProductCode,
                    ProductName = y.ProductName,
                    ProductCategoryId = y.ProductCategoryId,
                    ProductUnitId = y.ProductUnitId
                }).ToList();

                //Lấy giá bán của các sản phẩm
                listProduct.ForEach(item =>
                {
                    decimal priceProduct = 0;

                    var listCurrentPriceProduct = listPriceProduct
                        .Where(x => x.ProductId == item.ProductId)
                        .OrderByDescending(z => z.EffectiveDate)
                        .ToList();

                    var price = listCurrentPriceProduct.FirstOrDefault();

                    if (price != null)
                    {
                        priceProduct = price.PriceVnd;
                    }

                    item.Price1 = priceProduct;

                    //Lấy tên đơn vị tính
                    item.ProductUnitName = listProductUnit.FirstOrDefault(x => x.CategoryId == item.ProductUnitId)?
                        .CategoryName;
                });

                return new GetMasterDataOrderServiceCreateResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success",
                    ListOrderStatus = listOrderStatusEntityModel,
                    ListMoneyUnit = listMoneyUnit,
                    ListProductCategory = listProductCategory,
                    ListProduct = listProduct,
                    ListLocalAddress = listLocalAddress
                };
            }
            catch (Exception e)
            {
                return new GetMasterDataOrderServiceCreateResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public CreateOrderServiceResult CreateOrderService(CreateOrderServiceParameter parameter)
        {
            try
            {
                bool isValidParameterNumber = true;
                if (parameter.CustomerOrder?.DaysAreOwed < 0 || parameter.CustomerOrder?.MaxDebt < 0 ||
                    parameter.CustomerOrder?.DiscountValue < 0)
                {
                    isValidParameterNumber = false;
                }
                parameter.ListCustomerOrderDetail.ForEach(item =>
                {
                    if (item.Quantity <= 0 || item.UnitPrice < 0 || item?.Vat < 0 || item?.DiscountValue < 0 ||
                        item?.ExchangeRate <= 0 || item?.GuaranteeTime < 0)
                    {
                        isValidParameterNumber = false;
                    }
                });

                if (!isValidParameterNumber)
                {
                    return new CreateOrderServiceResult
                    {
                        MessageCode = CommonMessage.Order.CREATE_FAIL,
                        StatusCode = HttpStatusCode.ExpectationFailed
                    };
                }

                //Kiểm tra chiết khấu của đơn hàng
                if (parameter.CustomerOrder.DiscountValue == null)
                {
                    parameter.CustomerOrder.DiscountValue = 0;
                }

                //Kiểm tra chiết khấu của sản phẩm
                if (parameter.ListCustomerOrderDetail.Count > 0)
                {
                    var listProduct = parameter.ListCustomerOrderDetail.ToList();
                    listProduct.ForEach(item =>
                    {
                        if (item.DiscountValue == null)
                        {
                            item.DiscountValue = 0;
                        }
                    });
                }

                var AccountKHL = context.Customer.Where(item => item.CustomerCode == "KHL001").SingleOrDefault();
                parameter.CustomerOrder.CustomerId = AccountKHL.CustomerId;

                parameter.ListCustomerOrderDetail.ForEach(item =>
                {
                    item.CreatedById = parameter.UserId;
                    item.CreatedDate = DateTime.Now;
                    item.Active = true;
                    item.OrderDetailId = Guid.NewGuid();
                    foreach (var itemX in item.OrderProductDetailProductAttributeValue)
                    {
                        itemX.OrderProductDetailProductAttributeValueId = Guid.NewGuid();
                    }
                });

                parameter.CustomerOrder.OrderId = Guid.NewGuid();
                parameter.CustomerOrder.OrderCode = GenerateOrderCode(1);

                //Kiểm tra trường hợp đã tồn tại Mã đơn hàng
                var duplicateOrder =
                    context.CustomerOrder.FirstOrDefault(x => x.OrderCode == parameter.CustomerOrder.OrderCode);
                if (duplicateOrder != null)
                {
                    return new CreateOrderServiceResult
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = CommonMessage.Order.ORDER_EXIST
                    };
                }

                #region Thêm Điểm vào bảng mapping với Đơn hàng và kiểm tra Điểm đã được đặt chưa

                bool hasUsing = false;
                var listLocalPoint = context.LocalPoint.ToList();
                var listMapping = new List<CustomerOrderLocalPointMapping>();
                var listUpdateLocalPoint = new List<LocalPoint>();
                parameter.ListLocalPointId.ForEach(item =>
                {
                    var localPoint = listLocalPoint.FirstOrDefault(x => x.LocalPointId == item);
                    listUpdateLocalPoint.Add(localPoint);

                    //Nếu trong list Điểm có Điểm đang được sử dụng thì báo lỗi
                    if (localPoint.StatusId != 1)
                    {
                        hasUsing = true;
                    }

                    var mapping = new CustomerOrderLocalPointMapping();
                    mapping.CustomerOrderLocalPointMappingId = Guid.NewGuid();
                    mapping.OrderId = parameter.CustomerOrder.OrderId;
                    mapping.LocalPointId = item;

                    listMapping.Add(mapping);
                });

                //Nếu có Điểm đang được sử dụng thì báo lỗi
                if (hasUsing)
                {
                    return new CreateOrderServiceResult()
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = "Có bàn đang dùng vui lòng kiểm tra lại các bàn đã đặt"
                    };
                }
                //Nếu tất cả Điểm đều thỏa mãn thì đổi trạng thái các Điểm sang Đang dùng
                else
                {
                    listUpdateLocalPoint.ForEach(item => { item.StatusId = 0; });
                    context.LocalPoint.UpdateRange(listUpdateLocalPoint);
                }

                context.CustomerOrderLocalPointMapping.AddRange(listMapping);

                #endregion

                parameter.CustomerOrder.CreatedById = parameter.UserId;
                parameter.CustomerOrder.CreatedDate = DateTime.Now;
                var customerOrder = parameter.CustomerOrder.ToEntity();
                var listCustomerOrderDetail = new List<CustomerOrderDetail>();
                parameter.ListCustomerOrderDetail.ForEach(item =>
                {
                    listCustomerOrderDetail.Add(item.ToEntity());
                });
                context.CustomerOrder.Add(customerOrder);
                context.SaveChanges();

            }
            catch (Exception e)
            {
                return new CreateOrderServiceResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
            
            #region Gửi thông báo

            NotificationHelper.AccessNotification(context, TypeModel.CustomerOrder, "CRE", new CustomerOrder(),
                parameter.CustomerOrder, true);

            #endregion

            return new CreateOrderServiceResult()
            {
                StatusCode = HttpStatusCode.OK,
                MessageCode = "Success"
            };
        }

        public GetMasterDataPayOrderServiceResult GetMasterDataPayOrderService(GetMasterDataPayOrderServiceParameter parameter)
        {
            try
            {
                var listLocalAddress = new List<LocalAddressEntityModel>();
                var listLocalPoint = new List<LocalPointEntityModel>();

                //Lấy tỷ lệ quy đổi Tiền thành Điểm của khách hàng
                var pointRateString = context.SystemParameter.FirstOrDefault(x => x.SystemKey == "PointRate")
                    .SystemValueString;
                decimal pointRate = decimal.Parse(pointRateString);

                //Lấy tỷ lệ quy đổi Điểm thành Tiền của khách hàng
                var moneyRateString = context.SystemParameter.FirstOrDefault(x => x.SystemKey == "MoneyRate")
                    .SystemValueString;
                decimal moneyRate = decimal.Parse(moneyRateString);

                //Lấy list khu vực theo Branch của nhân viên đăng nhập
                var user = context.User.FirstOrDefault(x => x.UserId == parameter.UserId);
                var employee = context.Employee.FirstOrDefault(x => x.EmployeeId == user.EmployeeId);

                listLocalAddress = context.LocalAddress.Where(x => x.BranchId == employee.BranchId).Select(y =>
                    new LocalAddressEntityModel
                    {
                        LocalAddressId = y.LocalAddressId,
                        LocalAddressCode = y.LocalAddressCode,
                        LocalAddressName = y.LocalAddressName
                    }).OrderBy(z => z.LocalAddressCode).ToList();

                var listLocalAddressId = listLocalAddress.Select(y => y.LocalAddressId).ToList();
                listLocalPoint = context.LocalPoint.Where(x => listLocalAddressId.Contains(x.LocalAddressId)).Select(
                    y => new LocalPointEntityModel
                    {
                        LocalPointId = y.LocalPointId,
                        LocalPointCode = y.LocalPointCode,
                        LocalPointName = y.LocalPointName,
                        LocalAddressId = y.LocalAddressId,
                        StatusId = y.StatusId
                    }).OrderBy(z => z.LocalPointCode).ToList();

                return new GetMasterDataPayOrderServiceResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success",
                    ListLocalAddress = listLocalAddress,
                    ListLocalPoint = listLocalPoint,
                    PointRate = pointRate,
                    MoneyRate = moneyRate
                };
            }
            catch (Exception e)
            {
                return new GetMasterDataPayOrderServiceResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetListOrderByLocalPointResult GetListOrderByLocalPoint(GetListOrderByLocalPointParameter parameter)
        {
            try
            {
                var listOrder = new List<CustomerOrderEntityModel>();
                var listOrderStatus = context.OrderStatus.Where(x => x.Active == true).ToList();
                var statusDRA = listOrderStatus.FirstOrDefault(x => x.OrderStatusCode == "DRA");

                #region Lấy những đơn hàng thuộc Điểm có trạng thái Nháp

                var listOrderId = context.CustomerOrderLocalPointMapping
                    .Where(x => x.LocalPointId == parameter.LocalPointId).Select(y => y.OrderId).Distinct().ToList();

                listOrder = context.CustomerOrder
                    .Where(x => listOrderId.Contains(x.OrderId) && x.StatusId == statusDRA.OrderStatusId).Select(y =>
                        new CustomerOrderEntityModel
                        {
                            OrderId = y.OrderId,
                            OrderCode = y.OrderCode,
                            OrderDate = y.OrderDate,
                            Amount = y.Amount.Value
                        }).ToList();

                #endregion

                return new GetListOrderByLocalPointResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success",
                    ListOrder = listOrder
                };
            }
            catch (Exception e)
            {
                return new GetListOrderByLocalPointResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public PayOrderByLocalPointResult PayOrderByLocalPoint(PayOrderByLocalPointParameter parameter)
        {
            try
            {
                //Lấy list Khách hàng
                var listCustomer = context.Customer.Where(x => x.Active == true).ToList();
                var customer = listCustomer.FirstOrDefault(x => x.CustomerId == parameter.CustomerId);

                //Lấy nhóm khách hàng điềm đạm
                var customerGroupCategoryType = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "NHA")
                    ?.CategoryTypeId;
                var customerGroupId = context.Category.FirstOrDefault(x =>
                    x.CategoryCode == "TPH" && x.CategoryTypeId == customerGroupCategoryType)?.CategoryId;

                //Trạng thái khách hàng
                var customerStatusType = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "THA");
                var customerStatusHDO = context.Category.FirstOrDefault(x =>
                    x.CategoryCode == "HDO" && x.CategoryTypeId == customerStatusType.CategoryTypeId);

                var user = context.User.FirstOrDefault(x => x.UserId == parameter.UserId);
                var employee = context.Employee.FirstOrDefault(x => x.EmployeeId == user.EmployeeId);

                //Lấy tỷ lệ quy đổi điểm của khách hàng
                var pointRateString = context.SystemParameter.FirstOrDefault(x => x.SystemKey == "PointRate")
                    .SystemValueString;

                decimal pointRate = decimal.Parse(pointRateString);

                var listLocalPointId = new List<Guid?>();
                DateTime? orderDate = null;
                string customerName = "";
                string customerPhone = "";
                string cashierName = "";

                Guid? newCustomerId = customer?.CustomerId;
                using (var transaction = context.Database.BeginTransaction())
                {
                    var listOrderStatus = context.OrderStatus.Where(x => x.Active == true).ToList();
                    var statusCOMP = listOrderStatus.FirstOrDefault(x => x.OrderStatusCode == "COMP");

                    var listOrder = context.CustomerOrder.Where(x => parameter.ListOrderId.Contains(x.OrderId)).ToList();

                    #region Kiểm tra xem khách hàng mới hay khách hàng cũ

                    //Nếu bỏ trống Số điện thoại thì lấy khách lẻ
                    if (String.IsNullOrEmpty(parameter.CustomerPhone))
                    {
                        var customerPersonal = context.Customer.FirstOrDefault(x => x.CustomerCode == "KHL001");
                        newCustomerId = customerPersonal.CustomerId;
                        parameter.CustomerName = customerPersonal.CustomerName;
                    }
                    //Nếu là khách hàng mới thì tạo khách hàng
                    else if (customer.CustomerCode == "KHL001")
                    {
                        var newCustomer = new Customer();
                        newCustomer.CustomerId = Guid.NewGuid();
                        newCustomer.CustomerCode = GenerateCustomerCodeByOrderService();
                        newCustomer.CustomerGroupId = customerGroupId;
                        newCustomer.CustomerName = parameter.CustomerName?.Trim() ?? "";
                        newCustomer.StatusId = customerStatusHDO.CategoryId;
                        newCustomer.PersonInChargeId = user.EmployeeId; //Người phụ trách khách hàng mới tạo sẽ là nhân viên Thanh toán
                        newCustomer.CustomerType = 2; //Khách hàng cá nhân
                        newCustomer.MaximumDebtValue = 0;
                        newCustomer.MaximumDebtDays = 0;
                        newCustomer.TotalSaleValue = 0;
                        newCustomer.TotalReceivable = 0;
                        newCustomer.TotalCapital = 0;
                        newCustomer.Active = true;
                        newCustomer.CreatedById = parameter.UserId;
                        newCustomer.CreatedDate = DateTime.Now;

                        #region Tính điểm cho khách hàng

                        var totalAmout = listOrder.Sum(x => x.Amount);
                        var point = Math.Round(totalAmout.Value / pointRate);
                        newCustomer.Point = point;
                        newCustomer.PayPoint = 0;

                        #endregion

                        context.Customer.Add(newCustomer);

                        newCustomerId = newCustomer.CustomerId;

                        //Tạo contact
                        var newContact = new Contact();
                        newContact.ContactId = Guid.NewGuid();
                        newContact.ObjectId = newCustomer.CustomerId;
                        newContact.ObjectType = "CUS";
                        newContact.FirstName = parameter.CustomerName?.Trim() ?? "";
                        newContact.Gender = "NAM";
                        newContact.Phone = parameter.CustomerPhone.Trim();
                        newContact.Email = "";
                        newContact.Active = true;
                        newContact.OptionPosition = "CUS";
                        newContact.CreatedById = parameter.UserId;
                        newContact.CreatedDate = DateTime.Now;

                        context.Contact.Add(newContact);
                    }
                    //Nếu khách hàng đã tồn tại trên hệ thống thì cập nhật lại tên khách hàng
                    else
                    {
                        if (!String.IsNullOrEmpty(parameter.CustomerName))
                        {
                            customer.CustomerName = parameter.CustomerName;

                            #region Tính điểm cho khách hàng

                            customer.Point = parameter.Point;
                            var currentPayPoint = customer.PayPoint ?? 0;
                            customer.PayPoint = currentPayPoint + parameter.PayPoint;

                            #endregion

                            context.Customer.Update(customer);

                            var _contact = context.Contact.FirstOrDefault(x =>
                                x.ObjectId == customer.CustomerId && x.ObjectType == "CUS");
                            _contact.FirstName = parameter.CustomerName;
                            _contact.LastName = "";
                            context.Contact.Update(_contact);
                        }
                    }

                    #endregion

                    //Lấy phòng ban
                    var organization = context.Organization.FirstOrDefault();

                    //Lấy danh sách lý do thu
                    var reasonType = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "LTH");
                    //Thu tiền khách hàng
                    var reason = context.Category.FirstOrDefault(x =>
                        x.CategoryCode == "THA" && x.CategoryTypeId == reasonType.CategoryTypeId);

                    //Trạng thái phiếu thu
                    var statusType = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "TCH");
                    var status = context.Category.FirstOrDefault(x =>
                        x.CategoryCode == "DSO" && x.CategoryTypeId == statusType.CategoryTypeId);

                    //Lấy đơn vị tiền
                    var unitPriceType = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "DTI");
                    //VND
                    var unitPrice = context.Category.FirstOrDefault(x =>
                        x.CategoryCode == "VND" && x.CategoryTypeId == unitPriceType.CategoryTypeId);

                    var listReceiptInvoice = new List<ReceiptInvoice>();
                    var listReceiptInvoiceMapping = new List<ReceiptInvoiceMapping>();
                    //Đổi trạng thái list Order sang Đóng
                    listOrder.ForEach(item =>
                    {
                        item.StatusId = statusCOMP.OrderStatusId;
                        item.CustomerId = newCustomerId.Value;
                        item.CustomerName = parameter.CustomerName?.Trim() ?? "";
                        item.DiscountType = parameter.DiscountType;
                        item.DiscountValue = parameter.DiscountValue;
                        item.UpdatedById = parameter.UserId;
                        item.UpdatedDate = DateTime.Now;

                        //Với mỗi đơn hàng tạo ra một phiếu thu tiền mặt
                        var receiptInvoice = new ReceiptInvoice();
                        receiptInvoice.ReceiptInvoiceId = Guid.NewGuid();
                        receiptInvoice.ReceiptInvoiceCode =
                            "PT" + "-" + organization.OrganizationCode + DateTime.Now.Year
                            + (context.ReceiptInvoice.Count(r =>
                                   r.CreatedDate.Year == DateTime.Now.Year) + 1)
                            .ToString("D5");
                        receiptInvoice.ReceiptInvoiceDetail = "Thanh toán";
                        receiptInvoice.ReceiptInvoiceReason = reason.CategoryId;
                        receiptInvoice.ReceiptInvoiceNote = "";
                        receiptInvoice.OrganizationId = organization.OrganizationId;
                        receiptInvoice.StatusId = status.CategoryId;
                        receiptInvoice.RecipientName = parameter.CustomerName?.Trim() ?? "";
                        receiptInvoice.RecipientAddress = "";
                        decimal? amout = 0;
                        if (item.DiscountType == true)
                        {
                            amout = item.Amount - item.Amount * (item.DiscountValue ?? 0) / 100;
                        }
                        else
                        {
                            amout = item.Amount - (item.DiscountValue ?? 0);
                        }
                        receiptInvoice.UnitPrice = amout;
                        receiptInvoice.CurrencyUnit = unitPrice.CategoryId;
                        receiptInvoice.ExchangeRate = 1;
                        receiptInvoice.Amount = amout;
                        receiptInvoice.ReceiptDate = DateTime.Now;
                        receiptInvoice.VouchersDate = DateTime.Now;
                        receiptInvoice.Active = true;
                        receiptInvoice.CreatedById = parameter.UserId;
                        receiptInvoice.CreatedDate = DateTime.Now;

                        listReceiptInvoice.Add(receiptInvoice);

                        //mapping
                        var receiptInvoiceMapping = new ReceiptInvoiceMapping();
                        receiptInvoiceMapping.ReceiptInvoiceMappingId = Guid.NewGuid();
                        receiptInvoiceMapping.ReceiptInvoiceId = receiptInvoice.ReceiptInvoiceId;
                        receiptInvoiceMapping.ObjectId = newCustomerId;
                        receiptInvoiceMapping.CreatedById = parameter.UserId;
                        receiptInvoiceMapping.CreatedDate = DateTime.Now;

                        listReceiptInvoiceMapping.Add(receiptInvoiceMapping);
                    });

                    context.CustomerOrder.UpdateRange(listOrder);
                    context.ReceiptInvoice.AddRange(listReceiptInvoice);
                    context.ReceiptInvoiceMapping.AddRange(listReceiptInvoiceMapping);

                    //Đổi trạng thái của các Điểm gắn với đơn hàng
                    listLocalPointId = context.CustomerOrderLocalPointMapping
                        .Where(x => parameter.ListOrderId.Contains(x.OrderId.Value)).Select(y => y.LocalPointId).ToList();
                    var listLocalPoint = context.LocalPoint.Where(x => listLocalPointId.Contains(x.LocalPointId)).ToList();
                    listLocalPoint.ForEach(item => { item.StatusId = 1; });
                    context.LocalPoint.UpdateRange(listLocalPoint);

                    var order = listOrder.FirstOrDefault();
                    orderDate = order.OrderDate;
                    customerName = order.CustomerName;
                    if (String.IsNullOrEmpty(parameter.CustomerPhone))
                    {
                        customerPhone = "";
                    }
                    else
                    {
                        customerPhone = parameter.CustomerPhone.Trim();
                    }
                    cashierName = employee.EmployeeName.Trim();

                    context.SaveChanges();

                    transaction.Commit();
                }

                return new PayOrderByLocalPointResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success",
                    ListLocalPointId = listLocalPointId,
                    OrderDate = orderDate,
                    CustomerName = customerName,
                    CustomerPhone = customerPhone,
                    CashierName = cashierName
                };
            }
            catch (Exception e)
            {
                return new PayOrderByLocalPointResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public CheckExistsCustomerByPhoneResult CheckExistsCustomerByPhone(CheckExistsCustomerByPhoneParameter parameter)
        {
            try
            {
                var customer = new CustomerEntityModel();

                var contact =
                    context.Contact.FirstOrDefault(x => x.Phone == parameter.CustomerPhone && x.ObjectType == "CUS");

                //Lấy tỷ lệ quy đổi điểm của khách hàng
                var pointRateString = context.SystemParameter.FirstOrDefault(x => x.SystemKey == "PointRate")
                    .SystemValueString;

                decimal pointRate = decimal.Parse(pointRateString);

                //Nếu đã tồn tại khách hàng
                if (contact != null)
                {
                    customer = context.Customer.Where(x => x.CustomerId == contact.ObjectId).Select(y =>
                        new CustomerEntityModel
                        {
                            CustomerId = y.CustomerId,
                            CustomerCode = y.CustomerCode,
                            CustomerName = y.CustomerName,
                            Point = y.Point,
                            PayPoint = y.PayPoint
                        }).FirstOrDefault();
                }
                //Nếu chưa tồn tại khách hàng thì trả về khách lẻ
                else
                {
                    customer = context.Customer.Where(x => x.CustomerCode == "KHL001").Select(y =>
                        new CustomerEntityModel
                        {
                            CustomerId = y.CustomerId,
                            CustomerCode = y.CustomerCode,
                            CustomerName = y.CustomerName,
                            Point = 0,
                            PayPoint = 0
                        }).FirstOrDefault();
                }

                return new CheckExistsCustomerByPhoneResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success",
                    Customer = customer,
                    PointRate = pointRate
                };
            }
            catch (Exception e)
            {
                return new CheckExistsCustomerByPhoneResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public RefreshLocalPointResult RefreshLocalPoint(RefreshLocalPointParameter parameter)
        {
            try
            {
                var listLocalAddress = new List<LocalAddressEntityModel>();

                var user = context.User.FirstOrDefault(x => x.UserId == parameter.UserId);
                var employee = context.Employee.FirstOrDefault(x => x.EmployeeId == user.EmployeeId);

                //Lấy list Khu vực theo nhân viên đăng nhập
                var listComonLocalPoint = context.LocalPoint.ToList();
                listLocalAddress = context.LocalAddress.Where(x => x.BranchId == employee.BranchId).Select(y =>
                    new LocalAddressEntityModel
                    {
                        LocalAddressId = y.LocalAddressId,
                        LocalAddressCode = y.LocalAddressCode,
                        LocalAddressName = y.LocalAddressName,

                        //Lấy list Điểm thuộc khu vực
                        ListLocalPoint = listComonLocalPoint.Where(p => p.LocalAddressId == y.LocalAddressId).Select(
                            q => new LocalPointEntityModel
                            {
                                LocalPointId = q.LocalPointId,
                                LocalPointCode = q.LocalPointCode,
                                LocalPointName = q.LocalPointName,
                                StatusId = q.StatusId,
                                LocalAddressId = q.LocalAddressId,
                                StatusName = q.StatusId == 1 ? "Sẵn sàng" : "Đang dùng"
                            }).OrderBy(z => z.LocalPointCode).ToList()
                    }).ToList();

                return new RefreshLocalPointResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success",
                    ListLocalAddress = listLocalAddress
                };
            }
            catch (Exception e)
            {
                return new RefreshLocalPointResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetLocalPointByLocalAddressResult GetLocalPointByLocalAddress(GetLocalPointByLocalAddressParameter parameter)
        {
            try
            {
                var listLocalAddress = new List<LocalAddressEntityModel>();
                var listLocalPoint = new List<LocalPointEntityModel>();

                //Lấy list khu vực theo Branch của nhân viên đăng nhập
                var user = context.User.FirstOrDefault(x => x.UserId == parameter.UserId);
                var employee = context.Employee.FirstOrDefault(x => x.EmployeeId == user.EmployeeId);

                listLocalAddress = context.LocalAddress.Where(x => x.BranchId == employee.BranchId).Select(y =>
                    new LocalAddressEntityModel
                    {
                        LocalAddressId = y.LocalAddressId,
                        LocalAddressCode = y.LocalAddressCode,
                        LocalAddressName = y.LocalAddressName
                    }).OrderBy(z => z.LocalAddressCode).ToList();

                //Nếu lấy tất cả
                if (parameter.LocalAddressId == Guid.Empty)
                {
                    var listLocalAddressId = listLocalAddress.Select(y => y.LocalAddressId).ToList();
                    listLocalPoint = context.LocalPoint.Where(x => listLocalAddressId.Contains(x.LocalAddressId))
                        .Select(
                            y => new LocalPointEntityModel
                            {
                                LocalPointId = y.LocalPointId,
                                LocalPointCode = y.LocalPointCode,
                                LocalPointName = y.LocalPointName,
                                LocalAddressId = y.LocalAddressId,
                                StatusId = y.StatusId
                            }).OrderBy(z => z.LocalPointCode).ToList();
                }
                //Nếu lấy theo 1 Khu vực xác định
                else
                {
                    listLocalPoint = context.LocalPoint.Where(x => x.LocalAddressId == parameter.LocalAddressId).Select(
                        y => new LocalPointEntityModel
                        {
                            LocalPointId = y.LocalPointId,
                            LocalPointCode = y.LocalPointCode,
                            LocalPointName = y.LocalPointName,
                            LocalAddressId = y.LocalAddressId,
                            StatusId = y.StatusId
                        }).OrderBy(z => z.LocalPointCode).ToList();
                }

                return new GetLocalPointByLocalAddressResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success",
                    ListLocalPoint = listLocalPoint
                };
            }
            catch (Exception e)
            {
                return new GetLocalPointByLocalAddressResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        private string GenerateCustomerCodeByOrderService()
        {
            var result = "";

            var now = DateTime.Now;
            var hour = now.Hour.ToString("00");
            var minute = now.Minute.ToString("00");
            var second = now.Second.ToString("00");
            var day = now.Day.ToString("00");
            var month = now.Month.ToString("00");
            var year = now.Year.ToString("0000");

            result = day + month + year + hour + minute + second;

            return result;
        }

        public GetDataSearchTopReVenueResult GetDataSearchTopReVenue(GetDataSearchTopReVenueParameter parameter)
        {
            try
            {
                #region Lấy danh sách nhân viên bán hàng
                var listEmployeeEntity = context.Employee.ToList();

                var listEmployeeResult = new List<DataAccess.Models.Employee.EmployeeEntityModel>();
                listEmployeeEntity?.ForEach(emp =>
                {
                    listEmployeeResult.Add(new EmployeeEntityModel
                    {
                        EmployeeId = emp.EmployeeId,
                        EmployeeName = emp.EmployeeName,
                        EmployeeCode = emp.EmployeeCode
                    });
                });
                #endregion

                #region lấy ID phòng ban hiện tại của nhân viên đang đăng nhập
                var currentEmpId = context.User.FirstOrDefault(f => f.UserId == parameter.UserId).EmployeeId;
                var currentOrganizationId = context.Employee.FirstOrDefault(f => f.EmployeeId == currentEmpId).OrganizationId;
                var currentOrganizationEntity = context.Organization.FirstOrDefault(f => f.OrganizationId == currentOrganizationId);
                var currentOrganization = new DataAccess.Models.OrganizationEntityModel()
                {
                    OrganizationId = currentOrganizationEntity.OrganizationId,
                    OrganizationName = currentOrganizationEntity.OrganizationName
                };
                #endregion

                #region Lấy danh sách khách hàng
                var listCustomerEntity = context.Customer.Where(w => w.Active == true).ToList();
                var listCustomer = new List<Models.Customer.CustomerEntityModel>();
                listCustomerEntity?.ForEach(cus =>
                {
                    listCustomer.Add(new CustomerEntityModel
                    {
                        CustomerId = cus.CustomerId,
                        CustomerName = cus.CustomerName,
                        CustomerCode = cus.CustomerCode
                    });
                });
                #endregion

                #region Nhóm khách hàng
                var customerGroupTypeId = context.CategoryType.FirstOrDefault(f => f.CategoryTypeCode == "NHA").CategoryTypeId;
                var customerGroupEntity = context.Category.Where(w => w.Active == true && w.CategoryTypeId == customerGroupTypeId).ToList();
                var listCustomerGroupCategory = new List<Models.CategoryEntityModel>();
                customerGroupEntity?.ForEach(cate =>
                {
                    listCustomerGroupCategory.Add(new CategoryEntityModel
                    {
                        CategoryId = cate.CategoryId,
                        CategoryCode = cate.CategoryCode,
                        CategoryName = cate.CategoryName
                    });
                });
                #endregion

                #region Nhóm sản phẩm, dịch vụ
                var listProductCategoryEntity = context.ProductCategory.Where(w => w.Active == true).ToList();
                var listProductCategory = new List<Models.ProductCategory.ProductCategoryEntityModel>();
                listProductCategoryEntity?.ForEach(cate =>
                {
                    listProductCategory.Add(new ProductCategoryEntityModel
                    {
                        ProductCategoryId = cate.ProductCategoryId,
                        ProductCategoryName = cate.ProductCategoryName,
                        ProductCategoryCode = cate.ProductCategoryCode
                    });
                });
                #endregion

                #region Thông tin xuất excel
                var inforExportExcel = new InforExportExcelModel();
                // get dữ liệu để xuất excel
                var company = context.CompanyConfiguration.FirstOrDefault();
                inforExportExcel.CompanyName = company.CompanyName;
                inforExportExcel.Address = company.CompanyAddress;
                inforExportExcel.Phone = company.Phone;
                inforExportExcel.Website = "";
                inforExportExcel.Email = company.Email;
                #endregion

                return new GetDataSearchTopReVenueResult()
                {
                    ListEmployee = listEmployeeResult.OrderBy(o => o.EmployeeName).ToList(),
                    CurrentOrganization = currentOrganization,
                    ListCustomer = listCustomer.OrderBy(o => o.CustomerName).ToList(),
                    ListCustomerGroupCategory = listCustomerGroupCategory.OrderBy(o => o.CategoryName).ToList(),
                    ListProductCategory = listProductCategory.OrderBy(o => o.ProductCategoryName).ToList(),
                    InforExportExcel = inforExportExcel,
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = ""
                };
            }
            catch (Exception e)
            {
                return new GetDataSearchTopReVenueResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public SearchTopReVenueResult SearchTopReVenue(SearchTopReVenueParameter parameter)
        {
            try
            {
                #region Master data và  Phân quyền
                var listOrganizationentity = context.Organization.Where(w => w.Active == true).OrderBy(w => w.OrganizationName).ToList();
                var orderStatusEntity = context.OrderStatus.Where(w => w.Active == true).ToList();
                var listStatusIdValid = orderStatusEntity.Where(w => (w.OrderStatusCode == "DLV" || w.OrderStatusCode == "COMP") && w.Active == true)
                                                         .Select(w => w.OrderStatusId)
                                                         .ToList();
                var listAllUser = context.User.ToList();
                var listAllEmployee = context.Employee.ToList();
                var listEmployeeAuthorization = new List<Employee>();
                var listAllCustomer = context.Customer.ToList();
                var listProductEntity = context.Product.ToList();
                var listProductCategoryEntity = context.ProductCategory.ToList();

                var user = listAllUser.FirstOrDefault(x => x.UserId == parameter.UserId && x.Active == true);
                var employeeId = user.EmployeeId;
                var employee = listAllEmployee.FirstOrDefault(x => x.EmployeeId == employeeId);
                var isManager = employee.IsManager;

                List<Guid?> listGetAllChild = new List<Guid?>();    //List phòng ban: chính nó và các phòng ban cấp dưới của nó
                if (isManager)
                {
                    //Lấy list phòng ban con của user
                    if (employee.OrganizationId != null && isManager)
                    {
                        listGetAllChild.Add(employee.OrganizationId.Value);
                        listGetAllChild = GetOrganizationChildren(employee.OrganizationId.Value, listGetAllChild, listOrganizationentity);
                        //Lấy danh sách nhân viên EmployyeeId mà user phụ trách
                        var listEmployeeInChargeByManager = listAllEmployee.Where(x => (listGetAllChild == null || listGetAllChild.Count == 0 || listGetAllChild.Contains(x.OrganizationId))).ToList();
                        List<Guid> listEmployeeInChargeByManagerId = new List<Guid>();
                        List<Guid> listUserByManagerId = new List<Guid>();

                        listEmployeeInChargeByManager.ForEach(item =>
                        {
                            if (item.EmployeeId != null && item.EmployeeId != Guid.Empty)
                                listEmployeeInChargeByManagerId.Add(item.EmployeeId);
                        });

                        //Lấy danh sách nhân viên UserId mà user phụ trách
                        listEmployeeInChargeByManagerId.ForEach(item =>
                        {
                            var user_employee = listAllUser.FirstOrDefault(x => x.EmployeeId == item);
                            if (user_employee != null)
                                listUserByManagerId.Add(user_employee.UserId);
                        });

                        var listEmpByUserId = listAllUser.Where(w => listUserByManagerId.Contains(w.UserId)).Select(w => w.EmployeeId).ToList();
                        listEmployeeAuthorization = listAllEmployee.Where(w => listEmpByUserId.Contains(w.EmployeeId)).ToList();
                    }
                }
                else
                {
                    //Nếu không phải quản lý
                    listEmployeeAuthorization = listAllEmployee.Where(w => w.EmployeeId == employeeId).ToList();
                }
                #endregion

                #region Lấy thông tin để filter
                //nếu có điều kiện tìm theo khách hàng (loại khách hàng, khách hàng , nhóm khách hàng)
                //loại khách hàng
                var listCustomerFilter = listAllCustomer.ToList();
                if (parameter.ListCustomerType.Count != 0)
                {
                    listCustomerFilter = listCustomerFilter.Where(w => w.CustomerType != null && parameter.ListCustomerType.Contains(w.CustomerType)).ToList();
                }

                //khách hàng 
                if (parameter.ListCustomer.Count != 0)
                {
                    listCustomerFilter = listCustomerFilter.Where(w => w.CustomerId != null && parameter.ListCustomer.Contains(w.CustomerId)).ToList();
                }

                //nhóm khách hàng
                if (parameter.ListCustomerGroup.Count != 0)
                {
                    listCustomerFilter = listCustomerFilter.Where(w => w.CustomerGroupId != null && parameter.ListCustomerGroup.Contains(w.CustomerGroupId)).ToList();
                }

                var listCustomerFilterId = listCustomerFilter.Select(w => w.CustomerId).ToList();
                #endregion

                #region master data

                var listCustomerOrderDetailEntity = context.CustomerOrderDetail.Where(w => w.Active == true).ToList();

                //chỉ lấy những đơn hàng có trạng thái đóng hoặc đơn hàng bán
                var listCustomerOrderEntity = context.CustomerOrder
                                                     .Where(w => w.Active == true
                                                                 && w.Seller != null && listEmployeeAuthorization.Select(emp => emp.EmployeeId).Contains(w.Seller.Value) //phân quyền dữ liệu
                                                                 && listStatusIdValid.Contains(w.StatusId.Value) //filter theo trạng thái
                                                                 && (parameter.ListSellerId.Count == 0 || (parameter.ListSellerId.Count != 0 && parameter.ListSellerId.Contains(w.Seller))) //filter theo người bán hàng
                                                                 && (listCustomerFilterId.Count == 0 || (listCustomerFilterId.Count != 0 && listCustomerFilterId.Contains(w.CustomerId.Value))) //filter khách hàng
                                                                 && (parameter.OrderFromDate == null || (parameter.OrderFromDate != null && parameter.OrderFromDate <= w.OrderDate)) //filter theo ngày bán hàng
                                                                 && (parameter.OrderToDate == null || (parameter.OrderToDate != null && w.OrderDate <= parameter.OrderToDate)) //filter theo ngày bán hàng
                                                                 )
                                                     .ToList();

                //tìm theo nhóm hàng
                if (parameter.ListProductGroup.Count != 0)
                {
                    var listProductFilter = listProductEntity.Where(w => parameter.ListProductGroup.Contains(w.ProductCategoryId)).ToList();
                    var listProductFilterId = listProductFilter.Select(w => w.ProductId).ToList();
                    var listCustomerOrderDetailFilterByProduct = context.CustomerOrderDetail.Where(w => listProductFilterId.Contains(w.ProductId.Value)).ToList();
                    var listOrderFilter = listCustomerOrderDetailFilterByProduct.Select(w => w.OrderId).ToList() ?? new List<Guid>();
                    listCustomerOrderEntity = listCustomerOrderEntity.Where(w => listOrderFilter.Contains(w.OrderId)).ToList();
                }

                //nếu có tìm theo mã hàng hóa sản phẩm
                if (!String.IsNullOrWhiteSpace(parameter.ProductCode))
                {
                    var listProductFilter = listProductEntity.Where(w => w.ProductCode.ToLower().Contains(parameter.ProductCode.ToLower())).ToList();
                    var listProductFilterId = listProductFilter.Select(w => w.ProductId).ToList();
                    var listCustomerOrderDetailFilterByProduct = context.CustomerOrderDetail.Where(w => listProductFilterId.Contains(w.ProductId.Value)).ToList();
                    var listOrderFilter = listCustomerOrderDetailFilterByProduct.Select(w => w.OrderId).ToList() ?? new List<Guid>();
                    listCustomerOrderEntity = listCustomerOrderEntity.Where(w => listOrderFilter.Contains(w.OrderId)).ToList();
                }

                //là quản lý + nếu chọn phòng ban -> lấy phòng ban đó và phòng ban dưới thuộc phân quyền
                if (isManager == true && parameter.OrganizationId != null)
                {
                    var listEmpByOrganizationIdFilter = new List<Guid>();

                    if (listGetAllChild.Contains(parameter.OrganizationId))
                    {
                        //phòng ban được chọn thuộc danh sách phòng ban trong phân quyền
                        var listGetAllChildFilter = new List<Guid?>();
                        listGetAllChildFilter.Add(parameter.OrganizationId);
                        listGetAllChildFilter = GetOrganizationChildren(parameter.OrganizationId, listGetAllChildFilter, listOrganizationentity);

                        //Lấy danh sách nhân viên EmployyeeId mà user phụ trách
                        var listEmployeeInChargeByManager = listAllEmployee.Where(x => (listGetAllChildFilter == null || listGetAllChildFilter.Contains(x.OrganizationId))).ToList();
                        List<Guid> listEmployeeInChargeByManagerId = new List<Guid>();
                        List<Guid> listUserByManagerId = new List<Guid>();

                        listEmployeeInChargeByManager.ForEach(item =>
                        {
                            if (item.EmployeeId != null && item.EmployeeId != Guid.Empty)
                                listEmployeeInChargeByManagerId.Add(item.EmployeeId);
                        });

                        //Lấy danh sách nhân viên UserId mà user phụ trách
                        listEmployeeInChargeByManagerId.ForEach(item =>
                        {
                            var user_employee = listAllUser.FirstOrDefault(x => x.EmployeeId == item);
                            if (user_employee != null)
                                listUserByManagerId.Add(user_employee.UserId);
                        });

                        var listEmpByUserId = listAllUser.Where(w => listUserByManagerId.Contains(w.UserId)).Select(w => w.EmployeeId).ToList();
                        var listEmployeeAuthorizationFilter = listAllEmployee.Where(w => listEmpByUserId.Contains(w.EmployeeId)).ToList();
                        var listEmployeeAuthorizationFilterId = listEmployeeAuthorizationFilter.Select(w => w.EmployeeId).ToList();

                        listCustomerOrderEntity = listCustomerOrderEntity.Where(w => listEmployeeAuthorizationFilterId.Contains(w.Seller.Value)).ToList();
                    }
                    else
                    {
                        listCustomerOrderEntity = new List<CustomerOrder>();
                    }
                }
                #endregion

                #region get list Top ReVenue result
                var listTopEmployeeRevenue = new List<DataAccess.Models.Order.TopEmployeeRevenueEntityModel>();

                //lấy danh sách nhân viên bán hàng theo danh sách order đã lọc
                var listSellerIdFilter = listCustomerOrderEntity.Where(w => w.Seller != null).Select(w => w.Seller).ToList();
                var listSeller = listAllEmployee.Where(w => listSellerIdFilter.Contains(w.EmployeeId)).ToList();

                listSeller?.ForEach(emp =>
                {
                    decimal? amount = 0; //doanh thu
                    decimal? totalProductInOrder = 0;   //số lượng bán
                    var totalOrder = 0; //số lượng đơn hàng

                    var listCustomerOrderBySeller = listCustomerOrderEntity.Where(w => w.Seller == emp.EmployeeId).ToList() ?? new List<CustomerOrder>();
                    var listOrderId = listCustomerOrderBySeller.Select(w => w.OrderId).ToList() ?? new List<Guid>();
                    var listCustomerOrderDetailBySeller = listCustomerOrderDetailEntity.Where(w => listOrderId.Contains(w.OrderId)).ToList() ?? new List<CustomerOrderDetail>();

                    totalOrder = listCustomerOrderBySeller.Count();
                    totalProductInOrder = listCustomerOrderDetailBySeller.Select(w => w.Quantity).DefaultIfEmpty(0).Sum();

                    //Amount(tiền hàng trước thuế) = SUM(đơn giá sp * số lượng sp * tỷ giá sp - chiết khấu) của sản phẩm trong đơn hàng
                    listCustomerOrderDetailBySeller?.ForEach(item =>
                    {
                        decimal? price = item.UnitPrice * item.Quantity * item.ExchangeRate;
                        decimal? discount = 0;
                        if (item.DiscountType == true)
                        {
                            discount = price * item.DiscountValue / 100;
                        }
                        else
                        {
                            discount = item.DiscountValue;
                        }

                        amount += price - discount;
                    });

                    listTopEmployeeRevenue.Add(new TopEmployeeRevenueEntityModel
                    {
                        EmployeeId = emp.EmployeeId,
                        EmployeeCode = emp.EmployeeCode,
                        EmployeeName = emp.EmployeeName,
                        TotalOrder = totalOrder,
                        Amount = amount,
                        TotalProductInOrder = totalProductInOrder
                    });
                });

                #region Filter sau khi tính toán dữ liệu
                //doanh thu
                if (parameter.AmountFrom != null)
                {
                    listTopEmployeeRevenue = listTopEmployeeRevenue.Where(w => parameter.AmountFrom <= w.Amount).ToList();
                }
                if (parameter.AmountTo != null)
                {
                    listTopEmployeeRevenue = listTopEmployeeRevenue.Where(w => w.Amount <= parameter.AmountTo).ToList();
                }
                //số lượng bán
                if (parameter.TotalProductInOrderFrom != null)
                {
                    listTopEmployeeRevenue = listTopEmployeeRevenue.Where(w => parameter.TotalProductInOrderFrom <= w.TotalProductInOrder).ToList();
                }
                if (parameter.TotalProductInOrderTo != null)
                {
                    listTopEmployeeRevenue = listTopEmployeeRevenue.Where(w => w.TotalProductInOrder <= parameter.TotalProductInOrderTo).ToList();
                }
                //số lượng đơn hàng
                if (parameter.TotalOrderFrom != null)
                {
                    listTopEmployeeRevenue = listTopEmployeeRevenue.Where(w => parameter.TotalOrderFrom <= w.TotalOrder).ToList();
                }
                if (parameter.TotalOrderTo != null)
                {
                    listTopEmployeeRevenue = listTopEmployeeRevenue.Where(w => w.TotalOrder <= parameter.TotalOrderTo).ToList();
                }
                #endregion

                listTopEmployeeRevenue = listTopEmployeeRevenue.OrderBy(w => w.EmployeeCode.ToLower()).ToList();
                #endregion

                return new SearchTopReVenueResult()
                {
                    ListTopEmployeeRevenue = listTopEmployeeRevenue,
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = ""
                };
            }
            catch (Exception e)
            {
                return new SearchTopReVenueResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetDataSearchRevenueProductResult GetDataSearchRevenueProduct(GetDataSearchRevenueProductParameter parameter)
        {
            try
            {
                #region danh sách nhóm sản phẩm
                var productCategoryEntity = context.ProductCategory.Where(w => w.Active == true).ToList() ?? new List<ProductCategory>();
                var productCategoryResult = new List<DataAccess.Models.ProductCategory.ProductCategoryEntityModel>();

                productCategoryEntity?.ForEach(cate =>
                {
                    productCategoryResult.Add(new Models.ProductCategory.ProductCategoryEntityModel
                    {
                        ProductCategoryId = cate.ProductCategoryId,
                        ProductCategoryName = cate.ProductCategoryName,
                        ProductCategoryCode = cate.ProductCategoryCode
                    });
                });
                #endregion

                #region Kho hàng
                var listWarehouseEntity = context.Warehouse.Where(w => w.Active == true).ToList();
                var listWarehouse = new List<Models.WareHouse.WareHouseEntityModel>();
                listWarehouseEntity?.ForEach(warehouse =>
                {
                    listWarehouse.Add(new WareHouseEntityModel
                    {
                        WarehouseId = warehouse.WarehouseId,
                        WarehouseCode = warehouse.WarehouseCode,
                        WarehouseName = warehouse.WarehouseName,

                    });
                });
                #endregion

                #region Lấy danh sách nhân viên bán hàng
                var listEmployeeEntity = context.Employee.ToList();

                var listEmployeeResult = new List<DataAccess.Models.Employee.EmployeeEntityModel>();
                listEmployeeEntity?.ForEach(emp =>
                {
                    listEmployeeResult.Add(new EmployeeEntityModel
                    {
                        EmployeeId = emp.EmployeeId,
                        EmployeeName = emp.EmployeeName,
                        EmployeeCode = emp.EmployeeCode
                    });
                });
                #endregion

                #region Nhóm khách hàng
                var customerGroupTypeId = context.CategoryType.FirstOrDefault(f => f.CategoryTypeCode == "NHA").CategoryTypeId;
                var customerGroupEntity = context.Category.Where(w => w.Active == true && w.CategoryTypeId == customerGroupTypeId).ToList();
                var listCustomerGroupCategory = new List<Models.CategoryEntityModel>();
                customerGroupEntity?.ForEach(cate =>
                {
                    listCustomerGroupCategory.Add(new CategoryEntityModel
                    {
                        CategoryId = cate.CategoryId,
                        CategoryCode = cate.CategoryCode,
                        CategoryName = cate.CategoryName
                    });
                });
                #endregion

                #region Lấy danh sách khách hàng
                var listCustomerEntity = context.Customer.Where(w => w.Active == true).ToList();
                var listCustomer = new List<Models.Customer.CustomerEntityModel>();
                listCustomerEntity?.ForEach(cus =>
                {
                    listCustomer.Add(new CustomerEntityModel
                    {
                        CustomerId = cus.CustomerId,
                        CustomerName = cus.CustomerName,
                        CustomerCode = cus.CustomerCode
                    });
                });
                #endregion

                #region Thông tin xuất excel
                var inforExportExcel = new InforExportExcelModel();
                // get dữ liệu để xuất excel
                var company = context.CompanyConfiguration.FirstOrDefault();
                inforExportExcel.CompanyName = company.CompanyName;
                inforExportExcel.Address = company.CompanyAddress;
                inforExportExcel.Phone = company.Phone;
                inforExportExcel.Website = "";
                inforExportExcel.Email = company.Email;
                #endregion

                return new GetDataSearchRevenueProductResult()
                {
                    ListEmployee = listEmployeeResult,
                    ListProductCategory = productCategoryResult,
                    ListWarehouse = listWarehouse.OrderBy(o => o.WarehouseName).ToList(),
                    ListCustomerGroupCategory = listCustomerGroupCategory.OrderBy(o => o.CategoryName).ToList(),
                    ListCustomer = listCustomer.OrderBy(o => o.CustomerName).ToList(),
                    InforExportExcel = inforExportExcel,
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = ""
                };
            }
            catch (Exception e)
            {
                return new GetDataSearchRevenueProductResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public SearchRevenueProductResult SearchRevenueProduct(SearchRevenueProductParameter parameter)
        {
            try
            {
                #region master data và  Phân quyền
                var listProductEntity = context.Product.Where(w => w.Active == true).ToList() ?? new List<Product>();
                var unitProductTypeId = context.CategoryType.FirstOrDefault(f => f.CategoryTypeCode == "DNH").CategoryTypeId;
                var listProductUnit = context.Category.Where(w => w.Active == true && w.CategoryTypeId == unitProductTypeId).ToList();

                var orderStatusEntity = context.OrderStatus.Where(w => w.Active == true).ToList();
                var listStatusIdValid = orderStatusEntity.Where(w => (w.OrderStatusCode == "DLV" || w.OrderStatusCode == "COMP") && w.Active == true)
                                                         .Select(w => w.OrderStatusId)
                                                         .ToList();

                var listAllUser = context.User.ToList();
                var listAllEmployee = context.Employee.ToList();
                var listEmployeeAuthorization = new List<Employee>();
                var listAllCustomer = context.Customer.ToList();

                var user = listAllUser.FirstOrDefault(x => x.UserId == parameter.UserId && x.Active == true);
                var employeeId = user.EmployeeId;
                var employee = listAllEmployee.FirstOrDefault(x => x.EmployeeId == employeeId);
                var isManager = employee.IsManager;

                List<Guid?> listGetAllChild = new List<Guid?>();    //List phòng ban: chính nó và các phòng ban cấp dưới của nó
                if (isManager)
                {
                    //Lấy list phòng ban con của user
                    if (employee.OrganizationId != null && isManager)
                    {
                        listGetAllChild.Add(employee.OrganizationId.Value);
                        listGetAllChild = getOrganizationChildrenId(employee.OrganizationId.Value, listGetAllChild);
                        //Lấy danh sách nhân viên EmployyeeId mà user phụ trách
                        var listEmployeeInChargeByManager = listAllEmployee.Where(x => (listGetAllChild == null || listGetAllChild.Count == 0 || listGetAllChild.Contains(x.OrganizationId))).ToList();
                        List<Guid> listEmployeeInChargeByManagerId = new List<Guid>();
                        List<Guid> listUserByManagerId = new List<Guid>();

                        listEmployeeInChargeByManager.ForEach(item =>
                        {
                            if (item.EmployeeId != null && item.EmployeeId != Guid.Empty)
                                listEmployeeInChargeByManagerId.Add(item.EmployeeId);
                        });

                        //Lấy danh sách nhân viên UserId mà user phụ trách
                        listEmployeeInChargeByManagerId.ForEach(item =>
                        {
                            var user_employee = listAllUser.FirstOrDefault(x => x.EmployeeId == item);
                            if (user_employee != null)
                                listUserByManagerId.Add(user_employee.UserId);
                        });

                        var listEmpByUserId = listAllUser.Where(w => listUserByManagerId.Contains(w.UserId)).Select(w => w.EmployeeId).ToList();
                        listEmployeeAuthorization = listAllEmployee.Where(w => listEmpByUserId.Contains(w.EmployeeId)).ToList();
                    }
                }
                else
                {
                    //Nếu không phải quản lý
                    listEmployeeAuthorization = listAllEmployee.Where(w => w.EmployeeId == employeeId).ToList();
                }

                var listCustomerOrderDetailEntity = context.CustomerOrderDetail.Where(w => w.Active == true && w.ProductId != null).ToList(); //chỉ lấy những đơn hàng có sản phẩm
                var listCustomerOrderDetail = listCustomerOrderDetailEntity.ToList();

                //filter theo mã hàng hóa sản phẩm
                var listProductFilter = listProductEntity.ToList();
                var listOrderIdFilterByDetail = new List<Guid>();
                if (!string.IsNullOrWhiteSpace(parameter.ProductCode))
                {

                    listProductFilter = listProductEntity.Where(w => w.ProductCode != null && w.ProductCode.ToLower().Contains(parameter.ProductCode.ToLower())).ToList();
                    var listProductFilterId = listProductFilter.Select(w => w.ProductId).ToList();
                    listCustomerOrderDetail = listCustomerOrderDetail.Where(w => listProductFilterId.Contains(w.ProductId.Value)).ToList();
                    listOrderIdFilterByDetail = listCustomerOrderDetail.Select(w => w.OrderId).ToList();
                }
                //filter theo nhóm hàng
                var listOrderFilterByProductCategory = new List<Guid>();
                if (parameter.ProductCategory.Count != 0)
                {
                    listProductFilter = listProductEntity.Where(w => w.ProductCategoryId != null && parameter.ProductCategory.Contains(w.ProductCategoryId)).ToList();
                    var listProductFilterId = listProductFilter.Select(w => w.ProductId).ToList();
                    listCustomerOrderDetail = listCustomerOrderDetail.Where(w => listProductFilterId.Contains(w.ProductId.Value)).ToList();
                    listOrderFilterByProductCategory = listCustomerOrderDetail.Select(w => w.OrderId).ToList();
                }

                #region Lấy thông tin để filter
                //nếu có điều kiện tìm theo khách hàng (loại khách hàng, khách hàng , nhóm khách hàng)
                //loại khách hàng
                var listCustomerFilter = listAllCustomer.ToList();
                if (parameter.CustomerType.Count != 0)
                {
                    listCustomerFilter = listCustomerFilter.Where(w => w.CustomerType != null && parameter.CustomerType.Contains(w.CustomerType)).ToList();
                }

                //khách hàng 
                if (parameter.Customer.Count != 0)
                {
                    listCustomerFilter = listCustomerFilter.Where(w => w.CustomerId != null && parameter.Customer.Contains(w.CustomerId)).ToList();
                }

                //nhóm khách hàng
                if (parameter.CustomerGroup.Count != 0)
                {
                    listCustomerFilter = listCustomerFilter.Where(w => w.CustomerGroupId != null && parameter.CustomerGroup.Contains(w.CustomerGroupId)).ToList();
                }

                var listCustomerFilterId = listCustomerFilter.Select(w => w.CustomerId).ToList();
                #endregion

                var listCustomerOrderEntity = context.CustomerOrder
                                                  .Where(
                                                         w => w.Active == true
                                                              && w.Seller != null && listEmployeeAuthorization.Select(emp => emp.EmployeeId).Contains(w.Seller.Value) //phân quyền dữ liệu
                                                              && listStatusIdValid.Contains(w.StatusId.Value) //filter theo trạng thái
                                                              && (listOrderIdFilterByDetail.Count == 0 || (listOrderIdFilterByDetail.Count != 0 && listOrderIdFilterByDetail.Contains(w.OrderId))) //filter theo mã hàng hóa, sản phẩm
                                                              && (listOrderFilterByProductCategory.Count == 0 || (listOrderFilterByProductCategory.Count != 0 && listOrderFilterByProductCategory.Contains(w.OrderId))) //filter theo nhóm hàng
                                                              && (parameter.Seller == null || (parameter.Seller != null && parameter.Seller == w.Seller)) //filter theo người bán hàng
                                                              && (listCustomerFilterId.Count == 0 || (listCustomerFilterId.Count != 0 && listCustomerFilterId.Contains(w.CustomerId.Value))) //filter khách hàng
                                                              && (parameter.OrderFromDate == null || (parameter.OrderFromDate != null && parameter.OrderFromDate <= w.OrderDate)) //filter theo ngày bán hàng
                                                              && (parameter.OrderToDate == null || (parameter.OrderToDate != null && w.OrderDate <= parameter.OrderToDate)) //filter theo ngày bán hàng
                                                              && (parameter.Warehouse.Count == 0 || (w.WarehouseId != null && parameter.Warehouse.Count != 0 && parameter.Warehouse.Contains(w.WarehouseId))) //filter warehouse
                                                              )
                                                  .ToList();
                var listOrderIdFilter = listCustomerOrderEntity.Select(w => w.OrderId).ToList();
                listCustomerOrderDetail = listCustomerOrderDetail.Where(w => listOrderIdFilter.Contains(w.OrderId)).ToList();
                #endregion

                #region Lấy dữ liệu báo cáo theo từng sản phẩm
                var listProductRevenue = new List<DataAccess.Models.Order.ProductRevenueEntityModel>();

                //chỉ hiển thị những sản phẩm có phát sinh đơn hàng bán
                var listProductIdInOrder = listCustomerOrderDetail.Where(w => w.ProductId != null).Select(w => w.ProductId).Distinct().ToList();
                var listProductInOrder = listProductEntity.Where(w => listProductIdInOrder.Contains(w.ProductId)).ToList();

                listProductInOrder?.ForEach(product =>
                {
                    //lấy danh chi tiết đơn hàng theo từng sản phẩm
                    var listCustomerOrderDetailByProduct = listCustomerOrderDetail.Where(w => w.ProductId == product.ProductId).ToList() ?? new List<CustomerOrderDetail>();
                    //lấy danh sách đơn hàng theo từng sản phẩm
                    var listUniqueOrderId = listCustomerOrderDetailByProduct.Select(w => w.OrderId).Distinct().ToList() ?? new List<Guid>();
                    var listCustomerOrderByProduct = listCustomerOrderEntity.Where(w => listUniqueOrderId.Contains(w.OrderId)).ToList() ?? new List<CustomerOrder>();

                    var getSaleRevenueByProduct = GetSaleRevenueByProduct(listCustomerOrderDetailByProduct);

                    var unitName = listProductUnit.FirstOrDefault(f => f.CategoryId == product.ProductUnitId)?.CategoryName ?? "";

                    var productRevenue = new DataAccess.Models.Order.ProductRevenueEntityModel();
                    productRevenue.ProductName = product.ProductName; //Mã sản phẩm
                    productRevenue.ProductCode = product.ProductCode; //Tên sản phẩm
                    productRevenue.UnitName = unitName; //tên đơn vị tính
                    productRevenue.OrderCount = listCustomerOrderByProduct.Count(); //Số lượng đơn hàng
                    productRevenue.ProductInOrderCount = listCustomerOrderDetailByProduct.Select(w => w.Quantity).DefaultIfEmpty(0).Sum() ?? 0; //Số lượng bán
                    productRevenue.SaleRevenue = getSaleRevenueByProduct.SaleRevenueByProduct; //Doanh thu
                    productRevenue.TotalDiscount = getSaleRevenueByProduct.AmountDiscountProduct; //Chiết khấu
                    productRevenue.TotalPriceInitial = getSaleRevenueByProduct.AmountPriceInitial; // Tiền vốn

                    /* vì pending phần phiếu nhập hàng bán trả lại nên số liệu liên quan phần này = 0 */
                    productRevenue.ProductRefundCount = 0; //Số lượng trả lại
                    productRevenue.SaleRevenueRefund = 0; //Doanh thu trả lại
                    productRevenue.TotalPriceInitialRefund = 0; //Tiền vốn trả lại

                    productRevenue.TotalGrossProfit = (productRevenue.SaleRevenue - productRevenue.SaleRevenueRefund - productRevenue.TotalDiscount - productRevenue.TotalPriceInitial + productRevenue.TotalPriceInitialRefund); //Lãi gộp = Doanh thu bán-Doanh thu trả lại- chiết khấu- Tiền vốn+Tiền vốn trả lại

                    if (productRevenue.SaleRevenue - productRevenue.SaleRevenueRefund == 0)
                    {
                        productRevenue.TotalProfitPerSaleRevenue = 0;
                    }
                    else
                    {
                        productRevenue.TotalProfitPerSaleRevenue = (productRevenue.TotalGrossProfit) / (productRevenue.SaleRevenue - productRevenue.SaleRevenueRefund) * 100; //% lãi/DT =Lãi gộp/(Doanh thu bán- Doanh thu trả lại)
                    }

                    if (productRevenue.TotalPriceInitial - productRevenue.TotalPriceInitialRefund == 0)
                    {
                        productRevenue.TotalProfitPerPriceInitial = 0;
                    }
                    else
                    {
                        productRevenue.TotalProfitPerPriceInitial = (productRevenue.TotalGrossProfit) / (productRevenue.TotalPriceInitial - productRevenue.TotalPriceInitialRefund) * 100; //% lãi/Giá vốn =Lãi gộp/(Tiền vốn- Tiền vốn trả lại)
                    }

                    listProductRevenue.Add(productRevenue);
                });

                listProductRevenue = listProductRevenue.OrderBy(w => w.ProductCode).ToList();
                #endregion

                #region Nếu filter theo doanh thu, số lượng bán, số lượng đơn hàng, số lượng trả lại
                //doanh thu
                if (parameter.SaleRevenueFrom != null)
                {
                    listProductRevenue = listProductRevenue.Where(w => parameter.SaleRevenueFrom <= w.SaleRevenue).ToList();
                }
                if (parameter.SaleRevenueTo != null)
                {
                    listProductRevenue = listProductRevenue.Where(w => w.SaleRevenue <= parameter.SaleRevenueFrom).ToList();
                }
                //số lượng bán
                if (parameter.ProductInOrderCountFrom != null)
                {
                    listProductRevenue = listProductRevenue.Where(w => parameter.ProductInOrderCountFrom <= w.ProductInOrderCount).ToList();
                }

                if (parameter.ProductInOrderCountTo != null)
                {
                    listProductRevenue = listProductRevenue.Where(w => w.ProductInOrderCount <= parameter.ProductInOrderCountTo).ToList();
                }
                //số lượng đơn hàng
                if (parameter.OrderCountFrom != null)
                {
                    listProductRevenue = listProductRevenue.Where(w => parameter.OrderCountFrom <= w.OrderCount).ToList();
                }

                if (parameter.OrderCountTo != null)
                {
                    listProductRevenue = listProductRevenue.Where(w => w.OrderCount <= parameter.OrderCountTo).ToList();
                }

                // số lượng trả lại
                if (parameter.ProductRefundCountFrom != null)
                {
                    listProductRevenue = listProductRevenue.Where(w => parameter.ProductRefundCountFrom <= w.ProductRefundCount).ToList();
                }

                if (parameter.ProductRefundCountTo != null)
                {
                    listProductRevenue = listProductRevenue.Where(w => w.ProductRefundCount <= parameter.ProductRefundCountTo).ToList();
                }
                #endregion

                return new SearchRevenueProductResult()
                {
                    ListProductRevenue = listProductRevenue,
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = ""
                };
            }
            catch (Exception e)
            {
                return new SearchRevenueProductResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        //hàm tính tổng doanh số theo từng sản phẩm
        private SaleRevenueByProductModel GetSaleRevenueByProduct(List<CustomerOrderDetail> listCustomerOrderDetailByProduct)
        {
            var result = new SaleRevenueByProductModel();

            listCustomerOrderDetailByProduct?.ForEach(order_detail =>
            {
                //decimal? amountProduct = 0;
                //decimal? amountDiscountProduct = 0;
                //decimal? amountVatProduct = 0;

                decimal? price = order_detail.UnitPrice * order_detail.Quantity;
                decimal? discount = 0;
                if (order_detail.DiscountType == true)
                {
                    discount = price * order_detail.DiscountValue / 100;
                }
                else
                {
                    discount = order_detail.DiscountValue;
                }

                result.SaleRevenueByProduct += price; //tổng tiền theo từng sản phẩm
                result.AmountDiscountProduct += discount; //tổng chiết khấu
                result.AmountPriceInitial += order_detail.PriceInitial.HasValue ? order_detail.PriceInitial * order_detail.Quantity : 0; //tổng tiền vốn ban đầu
            });

            return result;
        }

        public GetListOrderDetailByOrderResult GetListOrderDetailByOrder(GetListOrderDetailByOrderParameter parameter)
        {
            try
            {
                var listOrderDetail = new List<CustomerOrderDetailEntityModel>();
                listOrderDetail = context.CustomerOrderDetail.Where(x => x.OrderId == parameter.OrderId).Select(y =>
                    new CustomerOrderDetailEntityModel
                    {
                        OrderDetailId = y.OrderDetailId,
                        OrderId = y.OrderId,
                        ProductId = y.ProductId,
                        ProductName = "",
                        ExchangeRate = y.ExchangeRate,
                        Quantity = y.Quantity,
                        UnitPrice = y.UnitPrice,
                        SumAmount = y.Quantity.Value * y.UnitPrice.Value * y.ExchangeRate.Value
                    }).ToList();

                var listAllProduct = context.Product.Where(x => x.Active == true).ToList();
                listOrderDetail.ForEach(item =>
                {
                    var product = listAllProduct.FirstOrDefault(x => x.ProductId == item.ProductId);
                    item.ProductName = product?.ProductName ?? "";
                });

                return new GetListOrderDetailByOrderResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success",
                    ListOrderDetail = listOrderDetail
                };
            }
            catch (Exception e)
            {
                return new GetListOrderDetailByOrderResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetListProductWasOrderResult GetListProductWasOrder(GetListProductWasOrderParameter parameter)
        {
            try
            {
                var listProductWasOrder = new List<ProductEntityModel>();

                //Lấy ra các orderId gắn với Điểm
                var listOrderId = context.CustomerOrderLocalPointMapping
                    .Where(x => x.LocalPointId == parameter.LocalPointId).Select(y => y.OrderId).Distinct().ToList();

                var listOrderStatus = context.OrderStatus.Where(x => x.Active == true).ToList();
                var statusDRA = listOrderStatus.FirstOrDefault(x => x.OrderStatusCode == "DRA");

                //Lấy ra Order có trạng thái Nháp ở thời điểm hiện tại
                var order = context.CustomerOrder.FirstOrDefault(x =>
                    listOrderId.Contains(x.OrderId) && x.StatusId == statusDRA.OrderStatusId);

                //Lấy list sản phẩm theo order
                listProductWasOrder = context.CustomerOrderDetail.Where(x => x.OrderId == order.OrderId).Select(y =>
                    new ProductEntityModel
                    {
                        ProductId = y.ProductId.Value,
                        Quantity = y.Quantity
                    }).ToList();

                return new GetListProductWasOrderResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success",
                    ListProductWasOrder = listProductWasOrder,
                    OrderId = order.OrderId
                };
            }
            catch (Exception e)
            {
                return new GetListProductWasOrderResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public UpdateCustomerServiceResult UpdateCustomerService(UpdateCustomerServiceParameter parameter)
        {
            try
            {
                var order = context.CustomerOrder.FirstOrDefault(x => x.OrderId == parameter.OrderId);

                var listOrderStatus = context.OrderStatus.Where(x => x.Active == true).ToList();
                var statusDRA = listOrderStatus.FirstOrDefault(x => x.OrderStatusCode == "DRA");

                if (order.StatusId != statusDRA.OrderStatusId)
                {
                    return new UpdateCustomerServiceResult()
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = "Đơn hàng đã được thanh toán"
                    };
                }

                var listOrderDetail = context.CustomerOrderDetail.Where(x => x.OrderId == parameter.OrderId).ToList();

                context.CustomerOrderDetail.RemoveRange(listOrderDetail);
                context.SaveChanges();

                decimal totalAmount = 0;
                parameter.ListCustomerOrderDetail.ForEach(item =>
                {
                    item.CreatedById = order.CreatedById;
                    item.CreatedDate = order.CreatedDate;
                    item.UpdatedById = parameter.UserId;
                    item.UpdatedDate = DateTime.Now;
                    item.Active = true;
                    item.OrderDetailId = Guid.NewGuid();
                    foreach (var itemX in item.OrderProductDetailProductAttributeValue)
                    {
                        itemX.OrderProductDetailProductAttributeValueId = Guid.NewGuid();
                    }

                    totalAmount += item.Quantity.Value * item.UnitPrice.Value * item.ExchangeRate.Value;
                });

                order.Amount = totalAmount;

                context.CustomerOrder.Update(order);
                context.SaveChanges();
                var listCustomerOrderDetail = new List<CustomerOrderDetail>();
                parameter.ListCustomerOrderDetail.ForEach(item =>
                {
                    listCustomerOrderDetail.Add(item.ToEntity());
                });
                context.CustomerOrderDetail.AddRange(listCustomerOrderDetail);
                context.SaveChanges();

                return new UpdateCustomerServiceResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success"
                };
            }
            catch (Exception e)
            {
                return new UpdateCustomerServiceResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetDataProfitByCustomerResult GetDataProfitByCustomer(GetDataProfitByCustomerParameter parameter)
        {
            try
            {
                #region Lấy danh sách khách hàng
                var listCustomerEntity = context.Customer.Where(w => w.Active == true).ToList();
                var listCustomer = new List<Models.Customer.CustomerEntityModel>();
                listCustomerEntity?.ForEach(cus =>
                {
                    listCustomer.Add(new CustomerEntityModel
                    {
                        CustomerId = cus.CustomerId,
                        CustomerName = cus.CustomerName,
                        CustomerCode = cus.CustomerCode
                    });
                });
                #endregion

                #region Nhóm khách hàng
                var customerGroupTypeId = context.CategoryType.FirstOrDefault(f => f.CategoryTypeCode == "NHA").CategoryTypeId;
                var customerGroupEntity = context.Category.Where(w => w.Active == true && w.CategoryTypeId == customerGroupTypeId).ToList();
                var listCustomerGroupCategory = new List<Models.CategoryEntityModel>();
                customerGroupEntity?.ForEach(cate =>
                {
                    listCustomerGroupCategory.Add(new CategoryEntityModel
                    {
                        CategoryId = cate.CategoryId,
                        CategoryCode = cate.CategoryCode,
                        CategoryName = cate.CategoryName
                    });
                });
                #endregion

                #region Nhóm sản phẩm, dịch vụ
                var listProductCategoryEntity = context.ProductCategory.Where(w => w.Active == true).ToList();
                var listProductCategory = new List<Models.ProductCategory.ProductCategoryEntityModel>();
                listProductCategoryEntity?.ForEach(cate =>
                {
                    listProductCategory.Add(new ProductCategoryEntityModel
                    {
                        ProductCategoryId = cate.ProductCategoryId,
                        ProductCategoryName = cate.ProductCategoryName,
                        ProductCategoryCode = cate.ProductCategoryCode
                    });
                });
                #endregion

                #region Thông tin xuất excel
                var inforExportExcel = new InforExportExcelModel();
                // get dữ liệu để xuất excel
                var company = context.CompanyConfiguration.FirstOrDefault();
                inforExportExcel.CompanyName = company.CompanyName;
                inforExportExcel.Address = company.CompanyAddress;
                inforExportExcel.Phone = company.Phone;
                inforExportExcel.Website = "";
                inforExportExcel.Email = company.Email;
                #endregion

                #region Lấy danh sách hợp đồng
                var contractStatusTypeId = context.CategoryType.FirstOrDefault(f => f.CategoryTypeCode == "THD").CategoryTypeId;
                var listContractStatus = context.Category.Where(w => w.Active == true && w.CategoryTypeId == contractStatusTypeId).ToList();
                var listInvalidStatus = new List<string>();
                listInvalidStatus.Add("MOI"); //loại những hợp đồng có trạng thái mới
                var listContractStatusValid = listContractStatus.Where(w => !listInvalidStatus.Contains(w.CategoryCode)).ToList();
                var listContractStatusValidId = listContractStatusValid.Select(w => w.CategoryId).ToList();

                var listContractEntity = context.Contract.Where(w => w.Active == true && w.StatusId != null && listContractStatusValidId.Contains(w.StatusId.Value)).ToList();
                var listContract = new List<DataAccess.Models.Contract.ContractEntityModel>();
                listContractEntity?.ForEach(contract =>
                {
                    listContract.Add(new ContractEntityModel
                    {
                        ContractId = contract.ContractId,
                        ContractCode = contract.ContractCode,
                        Amount = contract.Amount
                    });
                });
                #endregion

                return new GetDataProfitByCustomerResult()
                {
                    ListCustomer = listCustomer.OrderBy(w => w.CustomerName).ToList(),
                    ListCustomerGroupCategory = listCustomerGroupCategory.OrderBy(w => w.CategoryName).ToList(),
                    ListProductCategory = listProductCategory.OrderBy(w => w.ProductCategoryName).ToList(),
                    InforExportExcel = inforExportExcel,
                    ListContract = listContract.OrderBy(w => w.ContractCode).ToList(),
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success"
                };
            }
            catch (Exception e)
            {
                return new GetDataProfitByCustomerResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public SearchProfitCustomerResult SearchProfitCustomer(SearchProfitCustomerParameter parameter)
        {
            try
            {
                #region master data
                var orderStatusEntity = context.OrderStatus.Where(w => w.Active == true).ToList();
                var listStatusIdValid = orderStatusEntity.Where(w => (w.OrderStatusCode == "DLV" || w.OrderStatusCode == "COMP") && w.Active == true)
                                                        .Select(w => w.OrderStatusId)
                                                        .ToList();
                var listAllUser = context.User.ToList();
                var listAllEmployee = context.Employee.ToList();
                var listEmployeeAuthorization = new List<Employee>();
                var listAllCustomer = context.Customer.ToList();

                var listProductEntity = context.Product.ToList();
                var listProductCategoryEntity = context.ProductCategory.ToList();

                var listCustomerOrderDetailEntity = context.CustomerOrderDetail.Where(w => w.Active == true).ToList();
                #endregion

                #region phân quyền dữ liệu

                var user = listAllUser.FirstOrDefault(x => x.UserId == parameter.UserId && x.Active == true);
                var employeeId = user.EmployeeId;
                var employee = listAllEmployee.FirstOrDefault(x => x.EmployeeId == employeeId);
                var isManager = employee.IsManager;

                List<Guid?> listGetAllChild = new List<Guid?>();    //List phòng ban: chính nó và các phòng ban cấp dưới của nó

                if (isManager)
                {
                    //Lấy list phòng ban con của user
                    if (employee.OrganizationId != null && isManager)
                    {
                        listGetAllChild.Add(employee.OrganizationId.Value);
                        listGetAllChild = getOrganizationChildrenId(employee.OrganizationId.Value, listGetAllChild);
                        //Lấy danh sách nhân viên EmployyeeId mà user phụ trách
                        var listEmployeeInChargeByManager = listAllEmployee.Where(x => (listGetAllChild == null || listGetAllChild.Count == 0 || listGetAllChild.Contains(x.OrganizationId))).ToList();
                        List<Guid> listEmployeeInChargeByManagerId = new List<Guid>();
                        List<Guid> listUserByManagerId = new List<Guid>();

                        listEmployeeInChargeByManager.ForEach(item =>
                        {
                            if (item.EmployeeId != null && item.EmployeeId != Guid.Empty)
                                listEmployeeInChargeByManagerId.Add(item.EmployeeId);
                        });

                        //Lấy danh sách nhân viên UserId mà user phụ trách
                        listEmployeeInChargeByManagerId.ForEach(item =>
                        {
                            var user_employee = listAllUser.FirstOrDefault(x => x.EmployeeId == item);
                            if (user_employee != null)
                                listUserByManagerId.Add(user_employee.UserId);
                        });

                        var listEmpByUserId = listAllUser.Where(w => listUserByManagerId.Contains(w.UserId)).Select(w => w.EmployeeId).ToList();
                        listEmployeeAuthorization = listAllEmployee.Where(w => listEmpByUserId.Contains(w.EmployeeId)).ToList();
                    }
                }
                else
                {
                    //Nếu không phải quản lý
                    listEmployeeAuthorization = listAllEmployee.Where(w => w.EmployeeId == employeeId).ToList();
                }

                #endregion

                #region nếu có điều kiện tìm theo khách hàng (loại khách hàng, khách hàng , nhóm khách hàng)

                //loại khách hàng
                var listCustomerFilter = listAllCustomer.ToList();
                if (parameter.ListCustomerType.Count != 0)
                {
                    listCustomerFilter = listCustomerFilter.Where(w => w.CustomerType != null && parameter.ListCustomerType.Contains(w.CustomerType)).ToList();
                }

                //khách hàng 
                if (parameter.ListCustomer.Count != 0)
                {
                    listCustomerFilter = listCustomerFilter.Where(w => w.CustomerId != null && parameter.ListCustomer.Contains(w.CustomerId)).ToList();
                }

                //nhóm khách hàng
                if (parameter.ListCustomerGroup.Count != 0)
                {
                    listCustomerFilter = listCustomerFilter.Where(w => w.CustomerGroupId != null && parameter.ListCustomerGroup.Contains(w.CustomerGroupId)).ToList();
                }

                var listCustomerFilterId = listCustomerFilter.Select(w => w.CustomerId).ToList();

                #endregion

                #region lọc ra customer order theo điều kiện tìm kiếm

                //chỉ lấy những đơn hàng có trạng thái đóng hoặc đơn hàng bán
                var listCustomerOrderEntity = context.CustomerOrder
                    .Where(
                        w => w.Active == true
                             && w.Seller != null &&
                             listEmployeeAuthorization.Select(emp => emp.EmployeeId)
                                 .Contains(w.Seller.Value) //phân quyền dữ liệu
                             && listStatusIdValid.Contains(w.StatusId.Value) //filter theo trạng thái
                             && (listCustomerFilterId.Count == 0 ||
                                 (listCustomerFilterId.Count != 0 && listCustomerFilterId.Contains(w.CustomerId.Value))
                             ) //filter khách hàng
                             && (string.IsNullOrWhiteSpace(parameter.OrderCode) ||
                                 (!string.IsNullOrWhiteSpace(parameter.OrderCode) &&
                                  !string.IsNullOrWhiteSpace(w.OrderCode) &&
                                  w.OrderCode.ToLower().Trim().Contains(parameter.OrderCode.ToLower().Trim()))
                             ) //filter theo mã đơn hàng
                             && (parameter.OrderFromDate == null ||
                                 (parameter.OrderFromDate != null && parameter.OrderFromDate <= w.OrderDate)
                             ) //filter theo ngày bán hàng
                             && (parameter.OrderToDate == null ||
                                 (parameter.OrderToDate != null &&
                                  w.OrderDate <= parameter.OrderToDate)) //filter theo ngày bán hàng
                    )
                    .ToList();

                //nếu có điều kiện tìm theo nhóm hàng
                if (parameter.ListProductCategory.Count != 0)
                {
                    var listProductFilter = listProductEntity.Where(w =>
                             parameter.ListProductCategory.Contains(w.ProductCategoryId))
                        .ToList();
                    var listProductFilterId = listProductFilter.Select(w => w.ProductId).ToList();
                    var listCustomerOrderDetailFilterByProduct = context.CustomerOrderDetail
                        .Where(w => listProductFilterId.Contains(w.ProductId.Value)).ToList();
                    var listOrderFilter = listCustomerOrderDetailFilterByProduct.Select(w => w.OrderId).ToList() ??
                                          new List<Guid>();
                    listCustomerOrderEntity =
                        listCustomerOrderEntity.Where(w => listOrderFilter.Contains(w.OrderId)).ToList();
                }

                //nếu có điều kiện tìm theo mã hàng hóa, sản phẩm
                if (!string.IsNullOrWhiteSpace(parameter.ProductCode))
                {
                    var listProductFilter = listProductEntity.Where(w =>
                        w.ProductCode != null && w.ProductCode.ToLower().Trim()
                            .Contains(parameter.ProductCode.ToLower().Trim())).ToList();
                    var listProductFilterId = listProductFilter.Select(w => w.ProductId).ToList();
                    var listCustomerOrderDetailFilterByProduct = context.CustomerOrderDetail
                        .Where(w => listProductFilterId.Contains(w.ProductId.Value)).ToList();
                    var listOrderFilter = listCustomerOrderDetailFilterByProduct.Select(w => w.OrderId).ToList() ??
                                          new List<Guid>();
                    listCustomerOrderEntity =
                        listCustomerOrderEntity.Where(w => listOrderFilter.Contains(w.OrderId)).ToList();
                }

                //nếu có điều kiện tìm theo mã hợp đồng
                if (parameter.ListContract.Count != 0)
                {
                    var listContractIdFilter = context.Contract
                        .Where(w => w.Active == true && parameter.ListContract.Contains(w.ContractId))
                        .Select(w => w.ContractId).ToList();
                    listCustomerOrderEntity = listCustomerOrderEntity.Where(w =>
                        w.OrderContractId != null && listContractIdFilter.Contains(w.OrderContractId.Value)).ToList();
                }

                //nếu tìm theo số báo giá
                if (!string.IsNullOrWhiteSpace(parameter.QuoteCode))
                {
                    var listQuoteIdFilter = context.Quote
                        .Where(w => w.Active == true && w.QuoteCode != null && w.QuoteCode.ToLower().Trim()
                                        .Contains(parameter.QuoteCode.ToLower().Trim())).Select(w => w.QuoteId)
                        .ToList();
                    listCustomerOrderEntity = listCustomerOrderEntity
                        .Where(w => w.QuoteId != null && listQuoteIdFilter.Contains(w.QuoteId.Value)).ToList();
                }

                #endregion

                #region get list doanh số theo nhân viên

                var listSearchProfitCustomer = new List<DataAccess.Models.CustomerOrder.SearchProfitCustomerModel>();
                //chỉ hiển thị nhân viên có customer order
                var listCustomerInOrderId = listCustomerOrderEntity.Where(w => w.CustomerId != null)
                    .Select(w => w.CustomerId).Distinct().ToList();
                var listCustomerInOrder =
                    listAllCustomer.Where(w => listCustomerInOrderId.Contains(w.CustomerId)).ToList();

                var listCustomerOrderId = listCustomerOrderEntity.Select(w => w.OrderId).ToList();
                var listOrderDetail = context.CustomerOrderDetail
                    .Where(w => w.Active == true && listCustomerOrderId.Contains(w.OrderId)).ToList(); //lấy danh sách chi tiết đơn hàng sau khi filter


                listCustomerInOrder.ForEach(customer =>
                {
                    var listOrderByCustomer = listCustomerOrderEntity
                        .Where(w => w.CustomerId != null && w.CustomerId == customer.CustomerId).ToList();
                    var listOrderByCustomerId = listOrderByCustomer.Select(w => w.OrderId).ToList();
                    var listOrderDetailByCustomer =
                        listOrderDetail.Where(w => listOrderByCustomerId.Contains(w.OrderId)).ToList();

                    var saleRevenueByCustomer = GetSaleRevenueByCustomer(listOrderByCustomer, listOrderDetailByCustomer);

                    listSearchProfitCustomer.Add(new Models.CustomerOrder.SearchProfitCustomerModel
                    {
                        CustomerName = customer.CustomerName?.Trim() ?? "",
                        CustomerCode = customer.CustomerCode?.Trim() ?? "",
                        SaleRevenue = saleRevenueByCustomer.SaleRevenue,
                        TotalPriceInitial = saleRevenueByCustomer.TotalPriceInitial,
                        TotalGrossProfit = saleRevenueByCustomer.TotalGrossProfit,
                        TotalProfitPerSaleRevenue = saleRevenueByCustomer.TotalProfitPerSaleRevenue,
                        TotalProfitPerPriceInitial = saleRevenueByCustomer.TotalProfitPerPriceInitial
                    });
                });

                //filter by doanh thu
                if (parameter.SaleRevenueFrom != null)
                {
                    listSearchProfitCustomer = listSearchProfitCustomer
                        .Where(w => parameter.SaleRevenueFrom <= w.SaleRevenue).ToList();
                }
                if (parameter.SaleRevenueTo != null)
                {
                    listSearchProfitCustomer = listSearchProfitCustomer
                        .Where(w => w.SaleRevenue <= parameter.SaleRevenueTo).ToList();
                }

                //sắp xếp theo mã khách hàng
                listSearchProfitCustomer = listSearchProfitCustomer.OrderBy(w => w.CustomerCode).ToList();

                #endregion

                return new SearchProfitCustomerResult()
                {
                    ListSearchProfitCustomer = listSearchProfitCustomer,
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success"
                };
            }
            catch (Exception e)
            {
                return new SearchProfitCustomerResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        //hàm tính doanh số theo từng khách hàng
        public SaleRevenueByCustomerModel GetSaleRevenueByCustomer(List<CustomerOrder> ListCustomerOrder, List<CustomerOrderDetail> ListCustomerOrderDetail)
        {
            decimal? saleRevenue = 0; //doanh thu
            decimal? totalPriceInitial = 0; //Tiền vốn

            //Tổng tiền hàng trước thuế: TotalSumAmountProduct = SUM(đơn giá sp * số lượng sp * tỷ giá sp - chiết khấu)
            //chỉ lấy những chi tiết có số lượng và giá vốn hợp lệ
            ListCustomerOrderDetail = ListCustomerOrderDetail.Where(w => w.Quantity != null && w.PriceInitial != null).ToList();

            ListCustomerOrderDetail?.ForEach(order_detail =>
            {
                //tiền vốn
                totalPriceInitial += order_detail.Quantity * order_detail.PriceInitial;

                //Doanh thu (Tổng tiền vốn): TotalPriceInitial = SUM(số lượng sp * giá vốn) của chi tiết đơn hàng
                decimal? price = order_detail.UnitPrice * order_detail.Quantity;
                decimal? discount = 0;
                if (order_detail.DiscountType == true)
                {
                    discount = price * order_detail.DiscountValue / 100;
                }
                else
                {
                    discount = order_detail.DiscountValue;
                }

                saleRevenue += price;
            });

            //Lãi gộp = Doanh thu - Tiền vốn
            var totalGrossProfit = saleRevenue - totalPriceInitial;
            // %lãi/DT = = Lãi gộp/ Doanh thu
            decimal? totalProfitPerSaleRevenue = 0;
            if (saleRevenue == 0)
            {
                totalProfitPerSaleRevenue = 0;
            }
            else
            {
                totalProfitPerSaleRevenue = (totalGrossProfit / saleRevenue) * 100;
            }

            //%Lãi/Giá vốn = Lãi gộp/ Tiền vốn
            decimal? totalProfitPerPriceInitial = 0;
            if (totalPriceInitial == 0)
            {
                totalProfitPerPriceInitial = 0;
            }
            else
            {
                totalProfitPerPriceInitial = (totalGrossProfit / totalPriceInitial) * 100;
            }

            var saleRevenueByCustomer = new SaleRevenueByCustomerModel();
            saleRevenueByCustomer.SaleRevenue = saleRevenue;
            saleRevenueByCustomer.TotalPriceInitial = totalPriceInitial;
            saleRevenueByCustomer.TotalGrossProfit = totalGrossProfit;
            saleRevenueByCustomer.TotalProfitPerSaleRevenue = totalProfitPerSaleRevenue;
            saleRevenueByCustomer.TotalProfitPerPriceInitial = totalProfitPerPriceInitial;

            return saleRevenueByCustomer;
        }

        //Function lấy phòng ban cấp dưới
        private List<Guid?> GetOrganizationChildren(Guid? id, List<Guid?> list, List<Organization> ListOrganization)
        {
            var Organization = ListOrganization.Where(o => o.ParentId == id).ToList();
            Organization.ForEach(item =>
            {
                list.Add(item.OrganizationId);
                GetOrganizationChildren(item.OrganizationId, list, ListOrganization);
            });

            return list;
        }

        public GetInventoryNumberResult GetInventoryNumber(GetInventoryNumberParameter parameter)
        {
            try
            {

                var listWareHouseId = new List<Guid>();
                var listAllWarhouse = context.Warehouse.Where(c => c.Active == true).ToList();
                if (parameter.WareHouseId == null)
                {
                    listWareHouseId = listAllWarhouse.Select(m => m.WarehouseId).ToList();
                }
                else
                {
                    listWareHouseId.Add(parameter.WareHouseId.Value);
                    var listChildId = listAllWarhouse.Where(c => c.WarehouseParent == parameter.WareHouseId).Select(c => c.WarehouseId).ToList();
                    while (listChildId.Count() > 0)
                    {
                        listWareHouseId.AddRange(listChildId);
                        listChildId = listAllWarhouse.Where(c => c.WarehouseParent != null && listChildId.Contains(c.WarehouseParent.Value)).Select(c => c.WarehouseId).ToList();
                    }
                }
                #region Lấy tất cả phiếu nhập kho theo điều kiện
                /*
                 * - Kho được chọn (WarehouseId)
                 * - Có trạng thái Nhập kho
                 */
                var statusTypeId_PNK = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "TPH")?.CategoryTypeId;
                var statusId_PNK = context.Category.FirstOrDefault(x => x.CategoryCode == "NHK" && x.CategoryTypeId == statusTypeId_PNK)?.CategoryId;
                var listInventoryReceivingVoucher = context.InventoryReceivingVoucher.Where(x => x.Active && x.StatusId == statusId_PNK).ToList();

                #region Lấy tất cả sản phẩm đã nhập kho theo các phiếu nhập kho bên trên
                var listInventoryReceivingVoucherId = listInventoryReceivingVoucher.Select(y => y.InventoryReceivingVoucherId).ToList();
                var listProductReceivingVoucher = context.InventoryReceivingVoucherMapping
                    .Where(x => listWareHouseId.Contains(x.WarehouseId) && listInventoryReceivingVoucherId.Contains(x.InventoryReceivingVoucherId)).ToList();
                #endregion

                #endregion

                #region Lấy tất cả phiếu xuất kho theo điều kiện
                /*
                 * - Kho được chọn (WarehouseId)
                 * - Có trạng thái Xuất kho
                 */
                var statusTypeId_PXK = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "TPHX")?.CategoryTypeId;
                var statusId_PXK = context.Category.FirstOrDefault(x => x.CategoryCode == "NHK" && x.CategoryTypeId == statusTypeId_PXK)?.CategoryId;
                var listInventoryDeliveryVoucher = context.InventoryDeliveryVoucher.Where(x => x.Active && x.StatusId == statusId_PXK).ToList();

                #region Lấy tất cả sản phẩm đã xuất kho theo các phiếu xuất kho bên trên
                var listInventoryDeliveryVoucherId = listInventoryDeliveryVoucher
                    .Select(y => y.InventoryDeliveryVoucherId).ToList();
                var listProductDeliveryVoucher = context.InventoryDeliveryVoucherMapping
                    .Where(x => listWareHouseId.Contains(x.WarehouseId) && listInventoryDeliveryVoucherId.Contains(x.InventoryDeliveryVoucherId)).ToList();
                #endregion

                #endregion

                // Sản phẩm nhập kho
                listProductReceivingVoucher = listProductReceivingVoucher.Where(x => parameter.ProductId == x.ProductId).ToList();

                // Sản phẩm xuất kho
                listProductDeliveryVoucher = listProductDeliveryVoucher.Where(x => parameter.ProductId == x.ProductId).ToList();


                /*Sô tồn tho kho thực tế*/

                #region Số tồn kho ban đầu
                //Kiểm tra trong bảng InventoryReport
                var quantityInitialReport = context.InventoryReport.Where(x => listWareHouseId.Contains(x.WarehouseId) && x.ProductId == parameter.ProductId)
                    .Sum(y => y.StartQuantity);
                var quantityInitial = quantityInitialReport != null ? (quantityInitialReport ?? 0) : 0;
                #endregion

                #region Số tồn kho tối đa
                var quantityMaximumReport = context.InventoryReport.Where(x => listWareHouseId.Contains(x.WarehouseId) && x.ProductId == parameter.ProductId)
                    .Sum(y => y.QuantityMaximum);

                var quantityMaximum = quantityMaximumReport != null ? (quantityMaximumReport ?? 0) : 0;
                #endregion

                //Số lượng nhập kho của sản phẩm
                decimal quantityReceivingInStock = listProductReceivingVoucher.Where(x => x.ProductId == parameter.ProductId)
                    .Sum(y => y.QuantityActual);

                //Số lượng xuất kho của sản phẩm
                decimal quantityDeliveryInStock = listProductDeliveryVoucher.Where(x => x.ProductId == parameter.ProductId)
                    .Sum(y => y.QuantityActual);

                //Số tồn kho = Số tồn kho ban đầu + Số lượng nhập kho - Số lượng xuất kho
                var inventoryNumber = quantityInitial + quantityReceivingInStock - quantityDeliveryInStock;

                return new GetInventoryNumberResult
                {
                    InventoryNumber = inventoryNumber,
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success",
                };
            }
            catch (Exception ex)
            {
                return new GetInventoryNumberResult
                {
                    MessageCode = ex.Message,
                    StatusCode = HttpStatusCode.ExpectationFailed
                };
            }
        }

        public CheckTonKhoSanPhamResult CheckTonKhoSanPham(CheckTonKhoSanPhamParameter parameter)
        {
            try
            {
                int ktTonKho = 1;
                //var checkTonKho = true;
                var systemParTonKho = context.SystemParameter.FirstOrDefault(c => c.SystemKey == "HIM");
                var listProduct = context.Product.ToList();
                var listInventoryReport = context.InventoryReport.ToList();
                var listAllWarhouse = context.Warehouse.Where(c => c.Active == true).ToList();
                var commonCategory = context.Category.ToList();
                var commonCategoryType = context.CategoryType.ToList();
                var listAllInventoryReceivingVoucher = context.InventoryReceivingVoucher.Where(c => c.Active == true).ToList();
                var listAllInventoryReceivingVoucherMapping = context.InventoryReceivingVoucherMapping.ToList();
                var listAllInventoryDeliveryVoucher = context.InventoryDeliveryVoucher.Where(c => c.Active == true).ToList();
                var listAllInventoryDeliveryVoucherMapping = context.InventoryDeliveryVoucherMapping.ToList();
                var statusTypeId_PXK = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "TPHX")?.CategoryTypeId;
                var statusId_PXK = context.Category
                    .FirstOrDefault(x => x.CategoryCode == "NHK" && x.CategoryTypeId == statusTypeId_PXK)?.CategoryId;
                var statusTypeId_PNK = commonCategoryType.FirstOrDefault(x => x.CategoryTypeCode == "TPH")?.CategoryTypeId;
                var statusId_PNK = commonCategory
                    .FirstOrDefault(x => x.CategoryCode == "NHK" && x.CategoryTypeId == statusTypeId_PNK)?.CategoryId;
                var customerOrder = context.CustomerOrder.FirstOrDefault(c => c.OrderId == parameter.OrderId);
                var listKiemTraTonKho = new List<KiemTraTonKhoEntityModel>();

                if (customerOrder == null)
                {
                    return new CheckTonKhoSanPhamResult()
                    {
                        StatusCode = HttpStatusCode.OK,
                        MessageCode = "Order không tồn tại trong hệ thống"
                    };
                }; ;
                if (systemParTonKho != null)
                {
                    if (!Int32.TryParse(systemParTonKho.SystemValueString, out ktTonKho))
                    {
                        ktTonKho = 1;
                    }
                }

                //ProductId
                //WarehoureId
                //Quantity = Sum
                var listGroupOrderDetail = parameter.CustomerOrderDetail.GroupBy(g => new { g.ProductId, g.WarehouseId })
                    .Select(y => new
                    {
                        ProductId = y.Key.ProductId,
                        WarehouseId = y.Key.WarehouseId,
                        Quantity = y.Sum(s => s.Quantity)
                    }).ToList();
                var index = 0;
                var customerOrderDetails = new List<CustomerOrderDetailEntityModel>();
                if (parameter.CustomerOrderDetail != null && parameter.CustomerOrderDetail.Count > 0)
                {
                    customerOrderDetails = parameter.CustomerOrderDetail.Where(x => x.FolowInventory.Value).ToList();
                }
                if (customerOrderDetails != null && customerOrderDetails.Count() > 0)
                {
                    customerOrderDetails.ForEach(customerOrderDetail =>
                    {


                        index++;
                        var warehouse = listAllWarhouse.FirstOrDefault(c => c.WarehouseId == customerOrderDetail.WarehouseId.Value);

                        #region Lấy tất cả phiếu nhập kho theo điều kiện

                        /*
                         * - Kho được chọn (WarehouseId)
                         * - Có trạng thái Nhập kho
                         */

                        var listInventoryReceivingVoucher = listAllInventoryReceivingVoucher.Where(x => x.Active && x.StatusId == statusId_PNK).ToList();

                        #region Lấy tất cả sản phẩm đã nhập kho theo các phiếu nhập kho bên trên

                        var listInventoryReceivingVoucherId = listInventoryReceivingVoucher.Select(y => y.InventoryReceivingVoucherId).ToList();
                        var listProductReceivingVoucher = listAllInventoryReceivingVoucherMapping
                            .Where(x => x.WarehouseId == customerOrderDetail.WarehouseId
                            && listInventoryReceivingVoucherId.Contains(x.InventoryReceivingVoucherId)).ToList();

                        #endregion

                        #endregion

                        #region Lấy tất cả phiếu xuất kho theo điều kiện
                        /*
                         * - Kho được chọn (WarehouseId)
                         * - Có trạng thái Xuất kho
                         */

                        var listInventoryDeliveryVoucher = listAllInventoryDeliveryVoucher.Where(x => x.Active && x.StatusId == statusId_PXK).ToList();

                        #region Lấy tất cả sản phẩm đã xuất kho theo các phiếu xuất kho bên trên

                        var listInventoryDeliveryVoucherId = listInventoryDeliveryVoucher
                        .Select(y => y.InventoryDeliveryVoucherId).ToList();
                        var listProductDeliveryVoucher = listAllInventoryDeliveryVoucherMapping
                            .Where(x => x.WarehouseId == customerOrderDetail.WarehouseId &&
                                        listInventoryDeliveryVoucherId.Contains(x.InventoryDeliveryVoucherId)).ToList();
                        #endregion

                        #endregion

                        // Sản phẩm nhập kho
                        listProductReceivingVoucher = listProductReceivingVoucher
                        .Where(x => customerOrderDetail.ProductId == x.ProductId).ToList();

                        // Sản phẩm xuất kho
                        listProductDeliveryVoucher = listProductDeliveryVoucher
                        .Where(x => customerOrderDetail.ProductId == x.ProductId).ToList();

                        /*Sô tồn tho kho thực tế*/

                        #region Số tồn kho ban đầu

                        //Kiểm tra trong bảng InventoryReport
                        var quantityInitialReport = listInventoryReport
                        .Where(x => x.WarehouseId == customerOrderDetail.WarehouseId && x.ProductId == customerOrderDetail.ProductId)
                        .Sum(y => y.StartQuantity);
                        var quantityInitial = quantityInitialReport != null ? (quantityInitialReport ?? 0) : 0;

                        #endregion

                        //Số lượng nhập kho của sản phẩm
                        decimal quantityReceivingInStock = listProductReceivingVoucher
                        .Where(x => x.ProductId == customerOrderDetail.ProductId)
                        .Sum(y => y.QuantityActual);

                        //Số lượng xuất kho của sản phẩm
                        decimal quantityDeliveryInStock = listProductDeliveryVoucher
                        .Where(x => x.ProductId == customerOrderDetail.ProductId)
                        .Sum(y => y.QuantityActual);

                        //Lấy số lượng sản phẩm giống nhau có trong list đã duyệt có cùng wareHouse
                        var quantityDaDuyetReport = listGroupOrderDetail.FirstOrDefault(c => c.ProductId == customerOrderDetail.ProductId
                     && c.WarehouseId == customerOrderDetail.WarehouseId.Value).Quantity;
                        var quantityDaDuyet = quantityDaDuyetReport != null ? (quantityDaDuyetReport ?? 0) : 0;

                        //Số tồn kho = Số tồn kho ban đầu + Số lượng nhập kho - Số lượng xuất kho - số lượng sản phẩm - số lượng sản phẩm đã duyệt          
                        var inventoryNumber = quantityInitial + quantityReceivingInStock
                        - quantityDeliveryInStock - customerOrderDetail.Quantity - quantityDaDuyet;

                        if (inventoryNumber < 0)
                        {
                            //checkTonKho = false;
                            var kiemTraTonKho = new KiemTraTonKhoEntityModel();
                            kiemTraTonKho.ProductCode = customerOrderDetail.ProductCode;
                            kiemTraTonKho.ProductName = customerOrderDetail.ProductName;
                            kiemTraTonKho.WarehouseName = warehouse.WarehouseName;
                            kiemTraTonKho.Stt = index;
                            kiemTraTonKho.SoLuongDat = customerOrderDetail.Quantity.Value;
                            kiemTraTonKho.SoLuongTonKho = quantityInitial + quantityReceivingInStock
                                            - quantityDeliveryInStock;
                            kiemTraTonKho.SoLuongTonKhoToiThieu = listInventoryReport.FirstOrDefault(x => x.ProductId == customerOrderDetail.ProductId)?.QuantityMinimum;
                            listKiemTraTonKho.Add(kiemTraTonKho);
                        }
                    });
            
                }
               
                return new CheckTonKhoSanPhamResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success",
                    KTTonKho = ktTonKho,
                    ListKiemTraTonKho = listKiemTraTonKho
                };
            }
            catch (Exception e)
            {
                return new CheckTonKhoSanPhamResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public UpdateCustomerOrderTonKhoResult UpdateCustomerOrderTonKho(UpdateCustomerOrderTonKhoParameter parameter)
        {
            try
            {
                var oldOrder =
                    context.CustomerOrder.FirstOrDefault(co => co.OrderId == parameter.CustomerOrder.OrderId);
                var employeeId = context.User.FirstOrDefault(u => u.UserId == parameter.UserId).EmployeeId;
                var employee = context.Employee.FirstOrDefault(u => u.EmployeeId == employeeId);

                Note note = new Note();
                note.NoteId = Guid.NewGuid();
                note.ObjectType = "ORDER";
                note.ObjectId = parameter.CustomerOrder.OrderId.Value;
                note.Type = "ADD";
                note.Active = true;
                note.CreatedById = parameter.UserId;
                note.CreatedDate = DateTime.Now;
                note.NoteTitle = CommonMessage.Note.NOTE_TITLE;

                bool isValidParameterNumber = true;
                var commonOrderStatus = context.OrderStatus.Where(c => c.Active == true);
                if (parameter.CustomerOrder?.DaysAreOwed < 0 || parameter.CustomerOrder?.MaxDebt < 0 ||
                    parameter.CustomerOrder?.DiscountValue < 0)
                {
                    isValidParameterNumber = false;
                }
                var newStatus = new OrderStatus();
                parameter.CustomerOrderDetail.ForEach(item =>
                {
                    if (item?.Quantity <= 0 || item?.ExchangeRate <= 0 || item?.UnitPrice < 0 ||
                        item?.GuaranteeTime < 0 || item?.Vat < 0 || item?.DiscountValue < 0)
                    {
                        isValidParameterNumber = false;
                    }
                });

                if (!isValidParameterNumber)
                {
                    return new UpdateCustomerOrderTonKhoResult
                    {
                        MessageCode = CommonMessage.Order.EDIT_ORDER_FAIL,
                        StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    };
                }
                switch (parameter.StatusType)
                {
                    case "SEND_APPROVAL":
                        newStatus = commonOrderStatus.FirstOrDefault(s => s.OrderStatusCode == "IP");
                        note.Description = employee.EmployeeName +  " " + CommonMessage.Note.NOTE_CONTENT_SEND_APPROVAL;

                        //Nếu Đơn hàng gắn với Hợp đồng thì cập nhật trạng thái hợp đồng sang Đang thực hiện
                        if (oldOrder.OrderContractId != null && oldOrder.OrderContractId != Guid.Empty)
                        {
                            var THDStatusType = context.CategoryType.FirstOrDefault(ct =>
                                ct.CategoryTypeCode == "THD" && ct.Active == true);
                            var DTHStatus = context.Category.FirstOrDefault(ct =>
                                ct.CategoryTypeId == THDStatusType.CategoryTypeId && ct.Active == true &&
                                ct.CategoryCode == "DTH");
                            var contractObj = context.Contract.FirstOrDefault(ctr =>
                                ctr.ContractId == oldOrder.OrderContractId);

                            contractObj.StatusId = DTHStatus.CategoryId;
                            context.Contract.Update(contractObj);
                            context.SaveChanges();
                        }

                        break;
                    case "APPROVAL":
                        newStatus = commonOrderStatus.FirstOrDefault(s => s.OrderStatusCode == "DLV");
                        note.Description = employee.EmployeeName + CommonMessage.Note.NOTE_CONTENT_APPROVAL;

                        break;
                }

                if (oldOrder == null)
                {
                    return new UpdateCustomerOrderTonKhoResult()
                    {
                        StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                        MessageCode = CommonMessage.Order.ORDER_NO_EXIST
                    };
                }

                #region Kiểm tra đơn hàng có thay đổi khách hàng hay không

                if (oldOrder.CustomerId != parameter.CustomerOrder.CustomerId)
                {
                    //Nếu đơn hàng thay đổi khách hàng thì kiểm tra xem đơn hàng đã phát sinh thanh toán chưa?
                    var order_reciept = context.ReceiptOrderHistory.FirstOrDefault(x => x.OrderId == oldOrder.OrderId);
                    if (order_reciept != null)
                    {
                        //Nếu đơn hàng đã phát sinh thanh toán thì không cho thay đổi khách hàng
                        return new UpdateCustomerOrderTonKhoResult()
                        {
                            StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                            MessageCode = CommonMessage.Order.ORDER_NO_PAY
                        };
                    }
                }

                #endregion

                #region Kiểm tra trạng thái đơn hàng có thỏa mãn điều kiện cập nhật đơn hàng hay không

                var oldStatus = context.OrderStatus.FirstOrDefault(x => x.OrderStatusId == oldOrder.StatusId);
                var oldStatusCode = oldStatus?.OrderStatusCode;

                var newStatusCode = newStatus?.OrderStatusCode;

                switch (oldStatusCode)
                {
                    case "DRA":
                        if (newStatusCode != "DRA" && newStatusCode != "IP")
                        {
                            return new UpdateCustomerOrderTonKhoResult()
                            {
                                StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                                MessageCode = CommonMessage.Order.ORDER_NO_CHANGE_STATUS + newStatus.Description
                            };
                        }
                        break;
                    case "IP":
                        if (newStatusCode != "IP" && newStatusCode != "PD" && newStatusCode != "DLV" &&
                            newStatusCode != "COMP" && newStatusCode != "ON" && newStatusCode != "RTN" &&
                            newStatusCode != "CAN")
                        {
                            return new UpdateCustomerOrderTonKhoResult()
                            {
                                StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                                MessageCode = CommonMessage.Order.ORDER_NO_CHANGE_STATUS + newStatus.Description
                            };
                        }
                        break;
                    case "ON":
                        if (newStatusCode != "ON" && newStatusCode != "IP" && newStatusCode != "PD" &&
                            newStatusCode != "DLV" && newStatusCode != "COMP" && newStatusCode != "RTN" &&
                            newStatusCode != "CAN")
                        {
                            return new UpdateCustomerOrderTonKhoResult()
                            {
                                StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                                MessageCode = CommonMessage.Order.ORDER_NO_CHANGE_STATUS + newStatus.Description
                            };
                        }
                        break;
                    case "DLV":
                        if (newStatusCode != "DLV" && newStatusCode != "RTN" && newStatusCode != "PD" &&
                            newStatusCode != "CAN" && newStatusCode != "COMP")
                        {
                            return new UpdateCustomerOrderTonKhoResult()
                            {
                                StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                                MessageCode = CommonMessage.Order.ORDER_NO_CHANGE_STATUS + newStatus.Description
                            };
                        }
                        break;
                    case "PD":
                        if (newStatusCode != "PD" && newStatusCode != "DLV" && newStatusCode != "COMP" &&
                            newStatusCode != "CAN")
                        {
                            return new UpdateCustomerOrderTonKhoResult()
                            {
                                StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                                MessageCode = CommonMessage.Order.ORDER_NO_CHANGE_STATUS + newStatus.Description
                            };
                        }
                        break;
                    case "COMP":
                        return new UpdateCustomerOrderTonKhoResult()
                        {
                            StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                            MessageCode = CommonMessage.Order.ORDER_NO_CHANGE
                        };
                    case "CAN":
                        return new UpdateCustomerOrderTonKhoResult()
                        {
                            StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                            MessageCode = CommonMessage.Order.ORDER_NO_CHANGE
                        };
                    case "RTN":
                        return new UpdateCustomerOrderTonKhoResult()
                        {
                            StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                            MessageCode = CommonMessage.Order.ORDER_NO_CHANGE
                        };
                }

                #endregion

                using (var transaction = context.Database.BeginTransaction())
                {
                    #region Delete all item Relation

                    var List_Delete_OrderProductDetailProductAttributeValue =
                        new List<OrderProductDetailProductAttributeValue>();
                    var List_Delete_CustomerOrderDetail = new List<CustomerOrderDetail>();
                    var List_Delete_OrderCostDetail = new List<OrderCostDetail>();

                    List_Delete_CustomerOrderDetail = context.CustomerOrderDetail
                        .Where(item => item.OrderId == parameter.CustomerOrder.OrderId).ToList();

                    List_Delete_CustomerOrderDetail.ForEach(item =>
                    {
                        if (item.OrderDetailId != Guid.Empty)
                        {
                            var OrderProductDetailProductAttributeValueList = context
                                .OrderProductDetailProductAttributeValue
                                .Where(OPDPAV => OPDPAV.OrderDetailId == item.OrderDetailId).ToList();
                            List_Delete_OrderProductDetailProductAttributeValue.AddRange(
                                OrderProductDetailProductAttributeValueList);
                        }
                    });

                    List_Delete_OrderCostDetail = context.OrderCostDetail
                        .Where(item => item.OrderId == parameter.CustomerOrder.OrderId).ToList();

                    context.OrderProductDetailProductAttributeValue.RemoveRange(
                        List_Delete_OrderProductDetailProductAttributeValue);
                    context.SaveChanges();
                    context.CustomerOrderDetail.RemoveRange(List_Delete_CustomerOrderDetail);
                    context.SaveChanges();
                    context.OrderCostDetail.RemoveRange(List_Delete_OrderCostDetail);
                    context.SaveChanges();

                    context.CustomerOrder.Remove(oldOrder);
                    context.SaveChanges();

                    #endregion

                    #region Create new Order base on Old Order

                    parameter.CustomerOrderDetail.ForEach(item =>
                    {
                        //Nếu là chi phí khác thì tên sản phẩm dịch vụ sẽ bằng trường Description
                        if (item.OrderDetailType == 1)
                        {
                            item.ProductName = item.Description;
                        }

                        item.OrderDetailId = Guid.NewGuid();
                        if (item.OrderProductDetailProductAttributeValue != null)
                        {
                            foreach (var itemX in item.OrderProductDetailProductAttributeValue)
                            {
                                itemX.OrderProductDetailProductAttributeValueId = Guid.NewGuid();
                            }
                        }
                    });

                    parameter.OrderCostDetail.ForEach(item =>
                    {
                        item.OrderCostDetailId = Guid.NewGuid();
                        item.CreatedById = parameter.UserId;
                        item.CreatedDate = DateTime.Now;
                        item.UpdatedById = parameter.UserId;
                        item.UpdatedDate = DateTime.Now;
                    });

                    #region Kiểm tra có phải là khách lẻ không?

                    if (parameter.TypeAccount == 1)
                    {
                        var customer = context.Customer.FirstOrDefault(x => x.CustomerCode == "KHL001");

                        parameter.CustomerOrder.CustomerId = customer.CustomerId;
                        parameter.CustomerOrder.PaymentMethod = null;
                        parameter.CustomerOrder.DaysAreOwed = null;
                        parameter.CustomerOrder.MaxDebt = null;
                        parameter.CustomerOrder.BankAccountId = null;
                    }

                    #endregion

                    parameter.CustomerOrder.CreatedById = oldOrder.CreatedById;
                    parameter.CustomerOrder.CreatedDate = oldOrder.CreatedDate;
                    parameter.CustomerOrder.StatusId = newStatus.OrderStatusId;
                    parameter.CustomerOrder.UpdatedById = parameter.UserId;
                    parameter.CustomerOrder.UpdatedDate = DateTime.Now;
                    var customerOrder = parameter.CustomerOrder.ToEntity();
                    var listCustomerOrderDetail = new List<CustomerOrderDetail>();
                    parameter.CustomerOrderDetail.ForEach(item =>
                    {
                        var newItem = new CustomerOrderDetail();
                        newItem = item.ToEntity();

                        if (item.OrderProductDetailProductAttributeValue != null &&
                            item.OrderProductDetailProductAttributeValue.Count != 0)
                        {
                            item.OrderProductDetailProductAttributeValue.ForEach(_item =>
                            {
                                var _newItem = _item.ToEntity();
                                newItem.OrderProductDetailProductAttributeValue.Add(_newItem);
                            });
                        }

                        listCustomerOrderDetail.Add(newItem);
                    });
                    var listOrderCostDetail = new List<OrderCostDetail>();
                    parameter.OrderCostDetail.ForEach(item =>
                    {
                        listOrderCostDetail.Add(item.ToEntity());
                    });

                    context.CustomerOrder.Add(customerOrder);
                    context.Note.Add(note);
                    context.SaveChanges();

                    //Nếu phê duyệt đơn hàng
                    if (parameter.StatusType == "APPROVAL")
                    {
                        #region Tự động sinh phiếu xuất kho

                        var objDetail = context.CustomerOrderDetail
                            .Where(x => x.OrderId == parameter.CustomerOrder.OrderId).ToList();

                        /*
                         * Phiếu xuất kho tự động sẽ sinh theo Mã kho theo từng sản phẩm
                         */
                        var listAllWarehouse = context.Warehouse.Where(c => c.Active == true)
                            .Select(m => new
                            {
                                m.WarehouseId,
                                m.WarehouseParent
                            }).ToList();

                        var listWarehouseParentId = new List<KhoChaCon>();

                        var lstGroupWarhouseId = objDetail
                            .Where(c => c.WarehouseId != null && c.WarehouseId != Guid.Empty)
                            .GroupBy(c => c.WarehouseId).Select(m => m.Key).ToList();
                        var lstGroupWarehouseParentId = listAllWarehouse
                            .Where(c => lstGroupWarhouseId.Contains(c.WarehouseId)).ToList();

                        // Đệ quy để tính ra Id, Cha - Con => cần đệ quy trong vòng while. Chưa tìm được thuật toán tối ưu hơn
                        lstGroupWarehouseParentId.ForEach(item =>
                        {
                            // Để quy để lấy parent
                            var parent = listAllWarehouse.FirstOrDefault(c => c.WarehouseId == item.WarehouseParent);
                            while (true)
                            {
                                if (parent?.WarehouseParent == null) break;
                                parent = listAllWarehouse.FirstOrDefault(c => c.WarehouseId == parent.WarehouseParent);
                            }

                            // Check xem parentId đã tồn tại trong listChaCon
                            var contain = listWarehouseParentId.FirstOrDefault(c => c.ParentId == parent?.WarehouseId);
                            if (parent == null)
                            {
                                // Chưa tồn tại khởi tạo đối tượng và add vào list
                                var khoChaCon = new KhoChaCon
                                {
                                    ParentId = item.WarehouseId,
                                    ListChildId = new List<Guid>
                                    {
                                        item.WarehouseId
                                    }
                                };
                                listWarehouseParentId.Add(khoChaCon);
                            }
                            else if (contain == null)
                            {
                                // Chưa tồn tại khởi tạo đối tượng và add vào list
                                var khoChaCon = new KhoChaCon
                                {
                                    ParentId = parent.WarehouseId,
                                    ListChildId = new List<Guid>
                                    {
                                        item.WarehouseId
                                    }
                                };
                                listWarehouseParentId.Add(khoChaCon);
                            }
                            else if (contain != null)
                            {
                                // Tộn tại xóa đối tượng, thêm Id con và add lại vào listChaCon
                                listWarehouseParentId.Remove(contain);
                                contain.ListChildId.Add(item.WarehouseId);
                                listWarehouseParentId.Add(contain);
                            }
                        });

                        // Tự động sinh Phiếu xuất kho
                        var datenow = DateTime.Now;
                        TimeSpan today = new TimeSpan(datenow.Hour, datenow.Minute, datenow.Second);
                        var totalInvertoryCreate = context.InventoryDeliveryVoucher.Where(c =>
                            Convert.ToDateTime(c.CreatedDate).Day == datenow.Day &&
                            Convert.ToDateTime(c.CreatedDate).Month == datenow.Month &&
                            Convert.ToDateTime(c.CreatedDate).Year == datenow.Year).Count();
                        var statusInventoryDeliveryVoucher =
                            context.CategoryType.FirstOrDefault(f => f.CategoryTypeCode == "TPHX");
                        var statusInventoryDeliveryVoucherCXL = context.Category.FirstOrDefault(f =>
                            f.CategoryCode == "CXK" &&
                            f.CategoryTypeId == statusInventoryDeliveryVoucher.CategoryTypeId);

                        var index = 1;
                        listWarehouseParentId.ForEach(warehouse =>
                        {
                            var inventoryDeliveryVoucher = new InventoryDeliveryVoucher
                            {
                                InventoryDeliveryVoucherId = Guid.NewGuid(),
                                InventoryDeliveryVoucherCode = "PX-" + ConverCreateId(totalInvertoryCreate + index),
                                StatusId = statusInventoryDeliveryVoucherCXL.CategoryId,
                                InventoryDeliveryVoucherType = 1,
                                WarehouseId = warehouse.ParentId,
                                ObjectId = customerOrder.OrderId,
                                Receiver = customerOrder.RecipientName,
                                InventoryDeliveryVoucherDate = datenow,
                                InventoryDeliveryVoucherTime = today,
                                LicenseNumber = 1,
                                Active = true,
                                CreatedDate = datenow,
                                CreatedById = parameter.UserId
                            };
                            context.InventoryDeliveryVoucher.Add(inventoryDeliveryVoucher);

                            var lstInventoryDeliveryVoucherDetail = objDetail.Where(c =>
                                    c.WarehouseId != null && warehouse.ListChildId.Contains(c.WarehouseId.Value))
                                .Select(m => new InventoryDeliveryVoucherMapping
                                {
                                    InventoryDeliveryVoucherMappingId = Guid.NewGuid(),
                                    InventoryDeliveryVoucherId = inventoryDeliveryVoucher.InventoryDeliveryVoucherId,
                                    ProductId = m.ProductId.Value,
                                    QuantityRequest = m.Quantity.Value,
                                    QuantityInventory = 0,
                                    QuantityActual = m.Quantity.Value,
                                    PriceProduct = m.UnitPrice.Value,
                                    WarehouseId = m.WarehouseId.Value,
                                    Description = m.Description,
                                    Active = true,
                                    CreatedDate = datenow,
                                    CreatedById = parameter.UserId,
                                    CurrencyUnit = m.CurrencyUnit,
                                    UnitId = m.UnitId,
                                    ExchangeRate = m.ExchangeRate,
                                    Vat = m.Vat,
                                    DiscountType = m.DiscountType,
                                    DiscountValue = m.DiscountValue,
                                }).ToList();

                            context.InventoryDeliveryVoucherMapping.AddRange(lstInventoryDeliveryVoucherDetail);
                            index++;
                        });

                        #endregion
                    }

                    transaction.Commit();

                    #endregion

                    #region Kiểm tra xem đơn hàng đã được sử dụng để tạo Đơn đặt hàng Nhà cung cấp hay chưa?

                    var vendorOrder = context.VendorOrder.Where(item => item.CustomerOrderId == parameter.CustomerOrder.OrderId).FirstOrDefault();

                    #endregion
                }
            }
            catch (Exception ex)
            {
                return new UpdateCustomerOrderTonKhoResult
                {
                    MessageCode = CommonMessage.Order.EDIT_ORDER_FAIL,
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                };
            }
            
            #region Gửi thông báo

            NotificationHelper.AccessNotification(context, TypeModel.CustomerOrderDetail, "UPD", new CustomerOrder(),
                parameter.CustomerOrder, true, empId: parameter.CustomerOrder.Seller);

            #endregion

            #region lưu nhật ký hệ thống

            LogHelper.AuditTrace(context, ActionName.UPDATE, ObjectName.CUSTOMERORDER, parameter.CustomerOrder.OrderId.Value, parameter.UserId);

            #endregion
            
            return new UpdateCustomerOrderTonKhoResult
            {
                MessageCode = CommonMessage.Order.EDIT_ORDER_SUCCESS,
                StatusCode = System.Net.HttpStatusCode.OK,
            };
        }

        public GetServicePacketOrderByIdResult GetServicePacketOrderById(GetServicePacketOrderByIdParameter parameter)
        {
            try
            {
                var cateTypeAtrrId = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "ATTRIBUTE").CategoryTypeId;
                var listCateArr = context.Category.Where(x => x.CategoryTypeId == cateTypeAtrrId).ToList();
                var listDataType = GeneralList.GetTrangThais("DataTypeAttr").ToList();

                //Lấy thuộc tính của gói
                var listAtrrPacket = context.AttributeConfiguration.Where(x => x.ObjectType == 2 && x.ObjectId == parameter.ServicePacketId).Select(
                        x => new AttributeConfigurationEntityModel
                        {
                            DataType = x.DataType,
                            DataTypeName = listDataType.FirstOrDefault(y => y.Value == x.DataType).Name,
                            CategoryId = x.CategoryId,
                            CategoryName = listCateArr.FirstOrDefault(y => y.CategoryId == x.CategoryId).CategoryName,
                        }).ToList();

                //Lấy các product của gói
                var listProductId = context.ServicePacketMappingOptions.Where(x => x.ServicePacketId == parameter.ServicePacketId).Select(x => x.OptionId).ToList();

                var listProduct = context.Product.Where(x => listProductId.Contains(x.ProductId)).Select(x => new ProductEntityModel
                {
                    ProductId = x.ProductId,
                    ProductName = x.ProductName
                }).ToList();


                //lấy ra các tùy chọn của dịch vụ
                var listProductOption = context.ProductMappingOptions.Where(x => listProductId.Contains(x.ProductId)).Select(x => new ProductMappingOptionsEnityModel
                {
                    Id = x.Id,
                    OptionId = x.OptionId,
                    ProductId = x.ProductId,
                }).ToList();


                //Cac thuoc tinh cua tuy chon dich vu
                var listProductOptionId = listProductOption.Select(x => x.OptionId).ToList();
                var listAttrProductOptions = context.AttributeConfiguration.Where(x => x.ObjectType == 1 && listProductOptionId.Contains(x.ObjectId)).Select(
                        x => new AttributeConfigurationEntityModel
                        {
                            ObjectType = x.ObjectType,
                            ObjectId = x.ObjectId,
                            DataType = x.DataType,
                            DataTypeName = listDataType.FirstOrDefault(y => y.Value == x.DataType).Name,
                            CategoryId = x.CategoryId,
                            CategoryName = listCateArr.FirstOrDefault(y => y.CategoryId == x.CategoryId).CategoryName,
                        }).ToList();


                //khoi tao lien ket tra con cho tree
                var listTreeOrderPacketService = new List<TreeOrderPacketService>();

                //Dich vu
                listProduct.ForEach(pro => {

                    //Tuy chon cua dich vu
                    var listProductOp = listProductOption.Where(x => x.ProductId == pro.ProductId).ToList();
                    listProductOp.ForEach(op =>
                    {

                        //Thuoc tinh cua dich vu
                        var listAttr = listAttrProductOptions.Where(x => x.ObjectId == op.OptionId).ToList();
                        listAttr.ForEach(attrOp => { 
                        
                        });
                    });
                });



                return new GetServicePacketOrderByIdResult
            {
                StatusCode = HttpStatusCode.OK,
            };
            }
            catch (Exception e)
            {
                return new GetServicePacketOrderByIdResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public async Task<SearchOptionOfPacketServiceResult> SearchOptionOfPacketService(SearchOptionOfPacketServiceParameter parameter)
        {
            try
            {
                #region dịch vụ
                //Lấy gói dịch vụ
                var packetService = context.ServicePacket.FirstOrDefault(x => x.Id == parameter.PacketServiceId);
                if(packetService == null)
                {
                    return new SearchOptionOfPacketServiceResult()
                    {
                        MessageCode = "Gói dịch vụ không tồn tại trên hệ thống!",
                        StatusCode = HttpStatusCode.OK
                    };
                }

                var attrCateTypeId = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == ProductConsts.CategoryTypeCodeATR).CategoryTypeId;
                var listAttr = context.Category.Where(x => x.CategoryTypeId == attrCateTypeId).Select(x => new CategoryEntityModel {
                    CategoryId = x.CategoryId,
                    CategoryName = x.CategoryName,
                    CategoryCode = x.CategoryCode,
                }).ToList();

                //Lấy list Attr của gói dịch vụ
                var ObjectTypePacket = 2;
                var listAttrPacket = context.AttributeConfiguration.Where(x => x.ObjectId == packetService.Id && x.ObjectType == ObjectTypePacket)
                    .Select(x => new AttributeConfigurationEntityModel
                    {
                        Id = x.Id,
                        DataType = x.DataType,
                        CategoryId = x.CategoryId,
                        CategoryName = listAttr.FirstOrDefault(y => y.CategoryId == x.CategoryId).CategoryName,
                        ObjectType = x.ObjectType,
                        ObjectId = x.ObjectId
                        }).ToList();

                #endregion


                #region Tùy chọn dịch vụ

                var listTreeMapping = context.ServicePacketMappingOptions.Where(x => x.ServicePacketId == parameter.PacketServiceId)
                    .Select(x => new ServicePacketMappingOptionsEntityModel {
                        Id = x.Id,
                        Name = x.Name,
                        ParentId = x.ParentId,
                        ServicePacketId = x.ServicePacketId,
                        OptionId = x.OptionId,
                        SortOrder = x.SortOrder
                    }).OrderBy(x => x.SortOrder).ToList();

                //Lấy id các dịch vụ lv0 gán với gói
                var listOptionPackId = listTreeMapping.Select(x => x.OptionId).ToList();

                var listOptionCategoryUnit = await (from c in context.Category
                                                    join ct in context.CategoryType
                                                    on c.CategoryTypeId equals ct.CategoryTypeId
                                                    where ct.CategoryTypeCode == ProductConsts.CategoryTypeCodeUnit
                                                    select new OptionCategory
                                                    {
                                                        CategoryName = c.CategoryName,
                                                        CategoryId = c.CategoryId
                                                    }).ToListAsync();

                //lấy list dịch vụ
                var listOption = await (from o in context.Options
                                    where listOptionPackId.Contains(o.Id)
                                    select new OptionsEntityModel
                                    {
                                        Name = o.Name,
                                        NameCustom = o.Price != null ? o.Name + "   -   " + "Đơn giá: " + String.Format("{0:N0}", o.Price) : o.Name,
                                        Price = o.Price,
                                        Id = o.Id,
                                        ParentId = o.ParentId,
                                        Description = o.Description,
                                        VAT = o.Vat,
                                        CategoryUnitId = o.CategoryUnitId,
                                        CategoryUnitName = listOptionCategoryUnit.FirstOrDefault(x => x.CategoryId == o.CategoryUnitId) != null ? listOptionCategoryUnit.FirstOrDefault(x => x.CategoryId == o.CategoryUnitId).CategoryName : ""
                                    }).ToListAsync();

                listTreeMapping.ForEach(item =>
                {
                    //Nếu có thông tin về option
                    if(item.OptionId != null)
                    {
                        var option = listOption.FirstOrDefault(x => x.Id == item.OptionId);
                        if(option != null)
                        {
                            item.Name = option.Name;
                            item.NameCustom = option.NameCustom;
                            item.Price = option.Price;
                            item.Vat = option.VAT;
                            item.CategoryUnitName = option.CategoryUnitName;
                        }
                    }
                    else
                    {
                        item.NameCustom = item.Name;
                    }
                });

                //Lấy list Attr của từng dịch vụ
                var ObjectTypeOption = 1;
                var listOptionAttr = context.AttributeConfiguration.Where(x => listOptionPackId.Contains(x.ObjectId) && x.ObjectType == ObjectTypeOption)
                     .Select(x => new AttributeConfigurationEntityModel
                     {
                         Id = x.Id,
                         DataType = x.DataType,
                         CategoryId = x.CategoryId,
                         CategoryName = listAttr.FirstOrDefault(y => y.CategoryId == x.CategoryId).CategoryName,
                         ObjectType = x.ObjectType,
                         ObjectId = x.ObjectId
                     }).ToList();
                #endregion


                #region thông tin cấu hình

                var listDataType = GeneralList.GetTrangThais("DataTypeAttr").ToList();

                #endregion

                return new SearchOptionOfPacketServiceResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    ListAttrPacket = listAttrPacket,
                    ListOptionAttr = listOptionAttr,
                    ListOption = listTreeMapping,
                    ListDataType = listDataType
                };
            }
            catch (Exception e)
            {
                return new SearchOptionOfPacketServiceResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public async Task<ChangeStatusCustomerOrderResult> ChangeStatusCustomerOrder(ChangeStatusCustomerOrderParameter parameter)
        {
            try
            {
                var customerOrder = context.CustomerOrder.FirstOrDefault(x => x.OrderId == parameter.OrderId);
                if(customerOrder == null)
                {
                    return new ChangeStatusCustomerOrderResult()
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = "Không tìm thấy phiếu yêu cầu trên hệ thống!"
                    };
                }
                if(parameter.PaymentMethod != null)
                {
                    customerOrder.PaymentMethod = parameter.PaymentMethod.Value;
                }
                var listEmp = new List<Guid>();
                //Lấy list thông báo
                var notificationCateTypeId = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == ProductConsts.CategoryTypeCodeNotificationConfig).CategoryTypeId;
                var listNotifi = context.Category.Where(x => x.CategoryTypeId == notificationCateTypeId).ToList();

                var listNotificationConfiguration = context.NotificationConfiguration.Where(x => x.ServicePacketId == customerOrder.ServicePacketId).ToList();
                var listNotificationConfigurationId = listNotificationConfiguration.Select(x => x.Id).ToList();

                var listEmpNoti = context.EmployeeMappingNotificationConfiguration.Where(x => listNotificationConfigurationId.Contains(x.NotificationConfigurationId.Value)).ToList();

                var messagae = "";
               
                if (parameter.StatusOrder == 1)
                {
                    //Nếu là phiếu yêu cầu bình thường
                    if(customerOrder.OrderType == 1)
                    {
                        //Nếu trạng thái gửi về là  trạng thái mới => check xem có phát sinh không
                        //Nếu có chuyển sang quản lý xác nhận phát sinh ( 1 => 2 )
                        //Nếu không chuyển sang chờ thanh toán (1 => 4)
                        var listDetailExten = context.CustomerOrderDetailExten.FirstOrDefault(x => x.OrderId == parameter.OrderId);
                        if (listDetailExten != null)
                        {
                            customerOrder.StatusOrder = GeneralList.GetTrangThais("CustomerOrder").FirstOrDefault(x => x.Value == 2).Value;
                            messagae = "Đặt dịch vụ thành công!";                        }
                        else
                        {
                            customerOrder.StatusOrder = GeneralList.GetTrangThais("CustomerOrder").FirstOrDefault(x => x.Value == 4).Value;
                            messagae = "Đặt dịch vụ thành công!";
                        }
                    }
                    //Nếu là phiếu yêu cầu bổ sung => chuyển sang trạng thái chờ phê duyệt phiếu yêu cầu bổ sung
                    else
                    {
                        customerOrder.StatusOrder = GeneralList.GetTrangThais("CustomerOrder").FirstOrDefault(x => x.Value == 10).Value;
                        messagae = "Đặt dịch vụ thành công!";
                    }

                    //Lấy ra người cần nhận thông báo ở sự kiện đặt dịch vụ
                    var datDichVuId = listNotifi.FirstOrDefault(x => x.CategoryCode == "OrService").CategoryId;
                    var listEmpId = new List<Guid>();
                    var notiConfigure = listNotificationConfiguration.FirstOrDefault(x => x.CategoryId == datDichVuId);
                    listEmp = listEmpNoti.Where(x => x.NotificationConfigurationId == notiConfigure.Id).Select(x => x.EmployeeId.Value).ToList();
                    //Thông báo
                    var notificationReceiver = CommonHelper.GetListEmployeeIdNotifi(context, customerOrder, notiConfigure, null, null);
                    var getListEmpId = notificationReceiver.ListEmployeeId;
                    var deviceId = context.User.Where(x => x.EmployeeId == notificationReceiver.CustomerId)?.FirstOrDefault()?.DeviceId;
                    listEmp.AddRange(getListEmpId);
                    if (deviceId != null)
                    {
                        var body = "Phiếu " + customerOrder.OrderCode + ": " + messagae;
                        var title = "Phiếu " + customerOrder.OrderCode;
                        var type = 1; //1 : order, 2: orderAction

                        CommonHelper.PushNotificationToDevice(deviceId, title, body, type.ToString(), customerOrder.OrderId.ToString());
                    }
                }
                //Nếu trạng thái gửi về là quản lý phê duyệt (2)
                else if (parameter.StatusOrder == 2)
                {
                    //Nếu là phiếu yêu cầu chuyển sang KH xác nhận(3) khi không có phát sinh.
                    if (customerOrder.OrderType == 1)
                    {
                        var customerOrderDetailExten = context.CustomerOrderDetailExten.FirstOrDefault(x => x.OrderId == parameter.OrderId);
                        if (customerOrderDetailExten == null)
                        {
                            customerOrder.StatusOrder = GeneralList.GetTrangThais("CustomerOrder").FirstOrDefault(x => x.Value == 3).Value;
                            messagae = "Đã gửi yêu cầu xác nhận chi phí phát sinh!";
                        }
                        else
                        {
                            customerOrder.StatusOrder = GeneralList.GetTrangThais("CustomerOrder").FirstOrDefault(x => x.Value == 11).Value;
                            messagae = "Đã gửi yêu cầu phê duyệt dịch vụ phát sinh!";
                        }
                    }
                    //Nếu là phiếu yêu cầu bổ sung => chuyển sang trạng thái: Quản lý phê duyệt phiếu yêu cầu bổ sung (12)
                    else
                    {
                        customerOrder.StatusOrder = GeneralList.GetTrangThais("CustomerOrder").FirstOrDefault(x => x.Value == 12).Value;
                        messagae = "Đã gửi yêu cầu phê duyệt phiếu yêu cầu bổ sung!";
                    }

                    //Lấy ra người cần nhận thông báo ở sự kiện gửi phê duyệt
                    var datDichVuId = listNotifi.FirstOrDefault(x => x.CategoryCode == "Request").CategoryId;
                    var listEmpId = new List<Guid>();
                    var notiConfigure = listNotificationConfiguration.FirstOrDefault(x => x.CategoryId == datDichVuId);
                    listEmp = listEmpNoti.Where(x => x.NotificationConfigurationId == notiConfigure.Id).Select(x => x.EmployeeId.Value).ToList();
                    //Thông báo
                    var notificationReceiver = CommonHelper.GetListEmployeeIdNotifi(context, customerOrder, notiConfigure, null, null);
                    var getListEmpId = notificationReceiver.ListEmployeeId;
                    var deviceId = context.User.Where(x => x.EmployeeId == notificationReceiver.CustomerId)?.FirstOrDefault()?.DeviceId;
                    listEmp.AddRange(getListEmpId);
                    if (deviceId != null)
                    {
                        var body = "Phiếu " + customerOrder.OrderCode + ": " + messagae;
                        var title = "Phiếu " + customerOrder.OrderCode;
                        var type = 1; //1 : order, 2: orderAction

                        CommonHelper.PushNotificationToDevice(deviceId, title, body, type.ToString(), customerOrder.OrderId.ToString());
                    }

                }
                //Nếu trạng thái  gửi về là KH xác nhận (3) => chuyển sang chờ thanh toán (4)
                else if (parameter.StatusOrder == 3)
                {
                    customerOrder.StatusOrder = GeneralList.GetTrangThais("CustomerOrder").FirstOrDefault(x => x.Value == 4).Value;
                    messagae = "Đặt dịch vụ thành công!";

                    //Lấy ra người cần nhận thông báo ở sự kiện KH xác nhận chi phí dịch vụ bổ sung
                    var datDichVuId = listNotifi.FirstOrDefault(x => x.CategoryCode == "C").CategoryId;
                    var listEmpId = new List<Guid>();
                    var notiConfigure = listNotificationConfiguration.FirstOrDefault(x => x.CategoryId == datDichVuId);
                    listEmp = listEmpNoti.Where(x => x.NotificationConfigurationId == notiConfigure.Id).Select(x => x.EmployeeId.Value).ToList();
                    //Thông báo
                    var notificationReceiver = CommonHelper.GetListEmployeeIdNotifi(context, customerOrder, notiConfigure, null, null);
                    var getListEmpId = notificationReceiver.ListEmployeeId;
                    var deviceId = context.User.Where(x => x.EmployeeId == notificationReceiver.CustomerId)?.FirstOrDefault()?.DeviceId;
                    listEmp.AddRange(getListEmpId);
                    if (deviceId != null)
                    {
                        var body = "Phiếu " + customerOrder.OrderCode + ": " + messagae;
                        var title = "Phiếu " + customerOrder.OrderCode;
                        var type = 1; //1 : order, 2: orderAction

                        CommonHelper.PushNotificationToDevice(deviceId, title, body, type.ToString(), customerOrder.OrderId.ToString());
                    }
                }
                //Nếu trạng thái  gửi về là chờ thanh toán (4) => chuyển sang trạng thái Đã thành toán (5)
                else if (parameter.StatusOrder == 4)
                {
                    customerOrder.StatusOrder = GeneralList.GetTrangThais("CustomerOrder").FirstOrDefault(x => x.Value == 5).Value;
                    messagae = "Đã xác nhận thanh toán!";

                    //Cập nhật bước quy trình: OrderProcessDetail
                    var result = UpdateOrderProcess(ProductConsts.CategoryCodeConfirmStep, customerOrder.OrderProcessId, customerOrder.OrderType.Value,
                        false, customerOrder.OrderId, null);

                    //Nếu có lỗi thì báo lỗi
                    if (result.StatusCode == System.Net.HttpStatusCode.ExpectationFailed)
                    {
                        return new ChangeStatusCustomerOrderResult
                        {
                            StatusCode = HttpStatusCode.ExpectationFailed,
                            MessageCode = result.Message,
                        };
                    }

                    //Lấy ra người cần nhận thông báo ở sự kiện nhân viên xác nhận thanh toán
                    var datDichVuId = listNotifi.FirstOrDefault(x => x.CategoryCode == "D").CategoryId;
                    var listEmpId = new List<Guid>();
                    var notiConfigure = listNotificationConfiguration.FirstOrDefault(x => x.CategoryId == datDichVuId);
                    listEmp = listEmpNoti.Where(x => x.NotificationConfigurationId == notiConfigure.Id).Select(x => x.EmployeeId.Value).ToList();
                    //Thông báo
                    var notificationReceiver = CommonHelper.GetListEmployeeIdNotifi(context, customerOrder, notiConfigure, null, null);
                    var getListEmpId = notificationReceiver.ListEmployeeId;
                    var deviceId = context.User.Where(x => x.EmployeeId == notificationReceiver.CustomerId)?.FirstOrDefault()?.DeviceId;
                    listEmp.AddRange(getListEmpId);
                    if (deviceId != null)
                    {
                        var body = "Phiếu " + customerOrder.OrderCode + ": " + messagae;
                        var title = "Phiếu " + customerOrder.OrderCode;
                        var type = 1; //1 : order, 2: orderAction

                        CommonHelper.PushNotificationToDevice(deviceId, title, body, type.ToString(), customerOrder.OrderId.ToString());
                    }
                }
                //Nếu trạng thái gửi về là hủy (6) => chuyển sang trạng thái hủy (6)
                else if (parameter.StatusOrder == 6)
                {
                    customerOrder.StatusOrder = GeneralList.GetTrangThais("CustomerOrder").FirstOrDefault(x => x.Value == 6).Value;
                    messagae = "Đã hủy đặt dịch vụ!";

                    //Lấy ra người cần nhận thông báo ở sự kiện nhân viên hoặc KH hủy dịch vụ
                    var datDichVuId = listNotifi.FirstOrDefault(x => x.CategoryCode == "E").CategoryId;
                    var listEmpId = new List<Guid>();
                    var notiConfigure = listNotificationConfiguration.FirstOrDefault(x => x.CategoryId == datDichVuId);
                    listEmp = listEmpNoti.Where(x => x.NotificationConfigurationId == notiConfigure.Id).Select(x => x.EmployeeId.Value).ToList();
                    var notificationReceiver = CommonHelper.GetListEmployeeIdNotifi(context, customerOrder, notiConfigure, null, null);
                    var getListEmpId = notificationReceiver.ListEmployeeId;
                    var deviceId = context.User.Where(x => x.EmployeeId == notificationReceiver.CustomerId)?.FirstOrDefault()?.DeviceId;
                    listEmp.AddRange(getListEmpId);
                    if (deviceId != null)
                    {
                        var body = "Phiếu " + customerOrder.OrderCode + ": " + messagae;
                        var title = "Phiếu " + customerOrder.OrderCode;
                        var type = 1; //1 : order, 2: orderAction

                        CommonHelper.PushNotificationToDevice(deviceId, title, body, type.ToString(), customerOrder.OrderId.ToString());
                    }

                    //Nếu là hủy phiếu hỗ trợ bổ sung => kiểm tra xem còn công việc gì cần hỗ trợ k. Nếu không còn thì hoàn thành bước hỗ trợ dịch vụ
                    if (customerOrder.OrderType == 2) //Phiếu bổ sung
                    {
                        var listAllOrder = context.CustomerOrder.Where(x => x.OrderProcessId == customerOrder.OrderProcessId).ToList();
                        //Kiểm tra xem có các phiếu yêu cầu bổ sung phát sinh ở trạng thái khác hủy hoặc đã từ chối phát sinh, từ chối chi phí, từ chối phiếu bổ sung (6,7,8, 9)
                        var listOrderExten = listAllOrder.Where(x =>x.OrderProcessId == customerOrder.OrderProcessId && x.StatusOrder != 6 && x.StatusOrder != 7 && x.StatusOrder != 8 && x.StatusOrder != 9 && x.OrderType == 2).ToList();

                        //Các phiếu đã đươc chuyển sang phiễu hỗ trợ và ở trạng thái hoàn thành
                        var listOrderExtenId = listOrderExten.Select(x => x.OrderId).ToList();
                        var listOrderAction = listAllOrder.Where(x => x.StatusOrder == 3 && x.ObjectId != null && listOrderExtenId.Contains(x.ObjectId.Value) && x.IsOrderAction == true && x.StatusOrder == 3).ToList();
                        //Nếu số lượng phiếu yêu cầu phát sinh so với phiếu hỗ trợ đã hoàn thành tương ứng bằng nhau và phiếu chính cũng hoàn thành => hoàn thành hỗ trợ dịch vụ
                        if (listOrderExten.Count - 1 == listOrderAction.Count)
                        {
                            //Kiểm tra phiếu gốc. Nếu các điểm báo cáo của phiếu gốc đều hoàn thành => hoàn thành hỗ trợ dịch vụ
                            var customerOrderActionRoot = listAllOrder.FirstOrDefault(x => x.OrderType == 1 && x.IsOrderAction == true && x.StatusOrder == 2);
                            if (customerOrderActionRoot != null)
                            {
                                var listReportPoint = context.ReportPoint.Where(x => x.OrderActionId == customerOrderActionRoot.OrderId);
                                if (listReportPoint.Count() == listReportPoint.Where(x => x.Status == 3).Count())
                                {
                                    //Lấy categoryId của bước tạo yêu cầu + xác nhận thanh toán
                                    var cateTypeIdOfStep = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == ProductConsts.CategoryTypeCodeActionStep).CategoryTypeId;
                                    var reportStepId = context.Category.FirstOrDefault(x => x.CategoryTypeId == cateTypeIdOfStep && x.CategoryCode == ProductConsts.CategoryCodeSupportStep).CategoryId;

                                    //Cập nhật bước hỗ trợ dịch vụ trong orderprocess là hoàn thành
                                    var orderProcessDetail = context.OrderProcessDetail.FirstOrDefault(x => x.OrderProcessId == customerOrder.OrderProcessId
                                                                    && x.CategoryId == reportStepId);
                                    if (orderProcessDetail != null)
                                    {
                                        orderProcessDetail.Status = 3;//Hoàn thành
                                        context.OrderProcessDetail.Update(orderProcessDetail);
                                    }
                                }
                            }
                        }
                    }
                    //Nếu hủy phiếu yêu cầu bình thường => chuyển trạng thái của orderProcess thành Hủy đặt dịch vụ
                    else
                    {
                        //Chuyển orderprocess gán với order thành đã từ chối dịch vụ phát sinh
                        var deniedStatus = GeneralList.GetTrangThais("OrderProcess").FirstOrDefault(x => x.Value == 5).Value;
                        var orderProcess = context.OrderProcess.FirstOrDefault(x => x.Id == customerOrder.OrderProcessId);
                        if (orderProcess != null)
                        {
                            orderProcess.Status = deniedStatus;
                            context.OrderProcess.Update(orderProcess);
                            context.SaveChanges();
                        }
                    }
                }
                //Nếu trạng thái gửi về là Quản lý Từ chối dịch vụ phát sinh (7) => chuyển sang trạng thái Quản lý Từ chối dịch vụ phát sinh (7)
                else if (parameter.StatusOrder == 7)
                {
                    customerOrder.StatusOrder = GeneralList.GetTrangThais("CustomerOrder").FirstOrDefault(x => x.Value == 7).Value;
                    messagae = "Đã từ chối các dịch vụ phát sinh!";
                }
                //Nếu trạng thái gửi về là KH Từ chối dịch vụ phát sinh (8) => chuyển sang trạng thái KH Từ chối dịch vụ phát sinh (8)
                else if (parameter.StatusOrder == 8)
                {
                    customerOrder.StatusOrder = GeneralList.GetTrangThais("CustomerOrder").FirstOrDefault(x => x.Value == 8).Value;
                    messagae = "Đã từ chối các chi phí phát sinh!";

                    //Nếu là hủy phiếu hỗ trợ bổ sung => kiểm tra xem còn công việc gì cần hỗ trợ k. Nếu không còn thì hoàn thành bước hỗ trợ dịch vụ
                    if (customerOrder.OrderType == 2) //Phiếu bổ sung
                    {
                        var listAllOrder = context.CustomerOrder.Where(x => x.OrderProcessId == customerOrder.OrderProcessId).ToList();
                        //Kiểm tra xem có các phiếu yêu cầu bổ sung phát sinh ở trạng thái khác hủy hoặc đã từ chối phát sinh, từ chối chi phí, từ chối phiếu bổ sung (6,7,8, 9)
                        var listOrderExten = listAllOrder.Where(x => x.OrderProcessId == customerOrder.OrderProcessId && x.StatusOrder != 6 && x.StatusOrder != 7 && x.StatusOrder != 8 && x.StatusOrder != 9 && x.OrderType == 2).ToList();

                        //Các phiếu đã đươc chuyển sang phiễu hỗ trợ và ở trạng thái hoàn thành
                        var listOrderExtenId = listOrderExten.Select(x => x.OrderId).ToList();
                        var listOrderAction = listAllOrder.Where(x => x.StatusOrder == 3 && x.ObjectId != null && listOrderExtenId.Contains(x.ObjectId.Value) && x.IsOrderAction == true && x.StatusOrder == 3).ToList();
                        //Nếu số lượng phiếu yêu cầu phát sinh so với phiếu hỗ trợ đã hoàn thành tương ứng bằng nhau và phiếu chính cũng hoàn thành => hoàn thành hỗ trợ dịch vụ
                        if (listOrderExten.Count - 1 == listOrderAction.Count)
                        {
                            //Kiểm tra phiếu gốc. Nếu các điểm báo cáo của phiếu gốc đều hoàn thành => hoàn thành hỗ trợ dịch vụ
                            var customerOrderActionRoot = listAllOrder.FirstOrDefault(x => x.OrderType == 1 && x.IsOrderAction == true && x.StatusOrder == 2);
                            if (customerOrderActionRoot != null)
                            {
                                var listReportPoint = context.ReportPoint.Where(x => x.OrderActionId == customerOrderActionRoot.OrderId);
                                if (listReportPoint.Count() == listReportPoint.Where(x => x.Status == 3).Count())
                                {
                                    //Lấy categoryId của bước tạo yêu cầu + xác nhận thanh toán
                                    var cateTypeIdOfStep = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == ProductConsts.CategoryTypeCodeActionStep).CategoryTypeId;
                                    var reportStepId = context.Category.FirstOrDefault(x => x.CategoryTypeId == cateTypeIdOfStep && x.CategoryCode == ProductConsts.CategoryCodeSupportStep).CategoryId;

                                    //Cập nhật bước hỗ trợ dịch vụ trong orderprocess là hoàn thành
                                    var orderProcessDetail = context.OrderProcessDetail.FirstOrDefault(x => x.OrderProcessId == customerOrder.OrderProcessId
                                                                    && x.CategoryId == reportStepId);
                                    if (orderProcessDetail != null)
                                    {
                                        orderProcessDetail.Status = 3;//Hoàn thành
                                        context.OrderProcessDetail.Update(orderProcessDetail);
                                    }
                                }
                            }
                        }
                    }
                    //Nếu hủy phiếu yêu cầu bình thường => chuyển trạng thái của orderProcess thành KH từ chối chi phí phát sinh
                    else
                    {
                        //Chuyển orderprocess gán với order thành đã từ chối dịch vụ phát sinh
                        var deniedStatus = GeneralList.GetTrangThais("OrderProcess").FirstOrDefault(x => x.Value == 6).Value;
                        var orderProcess = context.OrderProcess.FirstOrDefault(x => x.Id == customerOrder.OrderProcessId);
                        if (orderProcess != null)
                        {
                            orderProcess.Status = deniedStatus;
                            context.OrderProcess.Update(orderProcess);
                            context.SaveChanges();
                        }
                    }

                    //Lấy ra người cần nhận thông báo ở sự kiện từ chối dịch vụ bổ sung hoặc thường
                    var datDichVuId = listNotifi.FirstOrDefault(x => x.CategoryCode == "B").CategoryId;
                    var listEmpId = new List<Guid>();
                    var notiConfigure = listNotificationConfiguration.FirstOrDefault(x => x.CategoryId == datDichVuId);
                    listEmp = listEmpNoti.Where(x => x.NotificationConfigurationId == notiConfigure.Id).Select(x => x.EmployeeId.Value).ToList();
                    var notificationReceiver = CommonHelper.GetListEmployeeIdNotifi(context, customerOrder, notiConfigure, null, null);
                    var getListEmpId = notificationReceiver.ListEmployeeId;
                    var deviceId = context.User.Where(x => x.EmployeeId == notificationReceiver.CustomerId)?.FirstOrDefault()?.DeviceId;
                    listEmp.AddRange(getListEmpId);
                    if (deviceId != null)
                    {
                        var body = "Phiếu " + customerOrder.OrderCode + ": " + messagae;
                        var title = "Phiếu " + customerOrder.OrderCode;
                        var type = 1; //1 : order, 2: orderAction

                        CommonHelper.PushNotificationToDevice(deviceId, title, body, type.ToString(), customerOrder.OrderId.ToString());
                    }
                }
                customerOrder.UpdatedById = parameter.UserId;
                customerOrder.UpdatedDate = DateTime.Now;
                context.CustomerOrder.Update(customerOrder);
                context.SaveChanges();


                return new ChangeStatusCustomerOrderResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = messagae,
                    ListEmpId = listEmp.Distinct().ToList(),
                };
            }
            catch (Exception e)
            {
                return new ChangeStatusCustomerOrderResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }


        private string GetPathOption(string pathName, Guid parentOptionId, List<ServicePacketMappingOptions> listTreeOptionOfPack)
        {
            var parentOption = listTreeOptionOfPack.FirstOrDefault(x => x.Id == parentOptionId);
            if(parentOption != null)
            {
                pathName = parentOption.Name + "--->" + pathName;
                if(parentOption.ParentId != null)
                {
                    pathName = GetPathOption(pathName, parentOption.ParentId.Value, listTreeOptionOfPack);
                }
            }
            return pathName;
        }

        public GetMasterDataCreateOrderActionResult GetMasterDataCreateOrderAction(GetMasterDataCreateOrderActionParameter parameter)
        {
            try
            {
                //Một phiếu hỗ trợ chỉ gán với 1 phiếu yêu cầu
                var user = context.User.FirstOrDefault(x => x.UserId == parameter.UserId);
                var creatorName = context.Employee.FirstOrDefault(x => x.EmployeeId == user.EmployeeId)?.EmployeeName;
                //Trạng thái đã thanh toán
                var statusOrderConfirm = GeneralList.GetTrangThais("CustomerOrder").FirstOrDefault(x => x.Value == 5).Value;
                var listAllCustomerOrder = context.CustomerOrder.ToList();
                //List id các phiếu yêu cầu đã có phiếu hỗ trợ
                var listCusOrderIdIsChose = listAllCustomerOrder.Where(x => x.IsOrderAction == true).Select(x => x.ObjectId).ToList();
                var listCustomerOrder = listAllCustomerOrder.Where(x => x.StatusOrder == statusOrderConfirm 
                                                                && x.IsOrderAction == false && !listCusOrderIdIsChose.Contains(x.OrderId))
                    .Select(x => new CustomerOrderEntityModel
                    {
                        OrderId = x.OrderId,
                        OrderCode = x.OrderCode,
                        CustomerId = x.CustomerId,
                        CusOrderDate = x.CreatedDate
                    }).ToList();

                var listEmployeeEntityModel = (from e in context.Employee
                                               join c in context.Contact on e.EmployeeId equals c.ObjectId
                                               into ec from ecJoined in ec.DefaultIfEmpty()
                                               where e.Active == true
                                               select new EmployeeEntityModel
                                               {
                                                   EmployeeId = e.EmployeeId,
                                                   EmployeeName = e.EmployeeName,
                                                   Phone = ecJoined.Phone
                                               }).ToList();

                return new GetMasterDataCreateOrderActionResult
                {
                    ListEmployeeEntityModel = listEmployeeEntityModel,
                    ListCustomerOrder = listCustomerOrder,
                    CreatorName = creatorName,
                    StatusCode = HttpStatusCode.OK,
                };
            }
            catch (Exception e)
            {
                return new GetMasterDataCreateOrderActionResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public ChangeCustomerOrderResult ChangeCustomerOrder(ChangeCustomerOrderParameter parameter)
        {
            try
            {
                var customerOrder = context.CustomerOrder.FirstOrDefault(x => x.OrderId == parameter.OrderId);
                if (customerOrder == null)
                {
                    return new ChangeCustomerOrderResult()
                    {
                        StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                        MessageCode = "Không tồn tại phiếu yêu cầu dịch vụ trên hệ thống!",

                    };
                }

                //Lấy các detail của phiếu
                var listDetail = context.CustomerOrderDetail.Where(x => x.OrderId == customerOrder.OrderId).Select(x => new CustomerOrderDetailEntityModel
                {
                    OrderDetailId = x.OrderDetailId,
                    OrderId = x.OrderId,
                    Quantity = x.Quantity,
                    PriceInitial = x.PriceInitial,
                    OptionId = x.OptionId,
                    ServicePacketId = x.PacketServiceId,
                    Stt = x.Stt,
                }).OrderBy(x => x.Stt).ToList();
                
                var listDetailId = listDetail.Select(x => x.OrderDetailId).ToList();
                var listPacketServiceId = listDetail.Select(x => x.ServicePacketId).Distinct().ToList();
                var listServicePacket = context.ServicePacket.Where(x => listPacketServiceId.Contains(x.Id)).Select(x => new ServicePacketEntityModel
                {
                    Id = x.Id,
                    Name = x.Name
                }).ToList();

                //Lấy sơ đầu tùy chọn dịch vụ của gói
                var listPacketId = listServicePacket.Select(x => x.Id).ToList();
                var listTreeOptionOfPack = context.ServicePacketMappingOptions.Where(x => listPacketId.Contains(x.ServicePacketId)).ToList();

                var listOptionId = listTreeOptionOfPack.Select(x => x.OptionId).ToList();
                var listAllOption = context.Options.Where(x => listOptionId.Contains(x.Id)).ToList();

                //Hoàn thiện thông tin cho detail
                listDetail.ForEach(item =>
                {
                    var mappingOption = listTreeOptionOfPack.FirstOrDefault(y => y.Id == item.OptionId && y.ServicePacketId == item.ServicePacketId);
                    item.OptionTempId = mappingOption.OptionId;
                    var option = listAllOption.FirstOrDefault(y => y.Id == mappingOption.OptionId);
                    if (option != null)
                    {
                        if (mappingOption.ParentId != null)
                        {
                            item.OptionName = GetPathOption(option.Name, mappingOption.ParentId.Value, listTreeOptionOfPack);
                        }
                        else
                        {
                            item.OptionName = option.Name;
                        }
                    }
                });

                //Lấy các dịch vụ phát sinh của gói
                var listOptionExten = context.CustomerOrderDetailExten.Where(x => x.OrderId == parameter.OrderId)
                                      .Select(x => new CustomerOrderDetailExtenEntityModel
                                      {
                                          Id = x.Id,
                                          OrderId = x.OrderId,
                                          Name = x.Name,
                                          Quantity = x.Quantity,
                                          Price = x.Price,
                                          Status = x.Status,
                                          Edit = false,
                                          ServicePacketId = x.ServicePacketId,
                                          Stt = x.Stt,
                                      }).OrderBy(x => x.Stt).ToList();

                //Thuộc tính tùy chọn dịch vụ
                var listAtrrOption = context.CustomerOrderExtension.Where(x => listDetailId.Contains(x.OrderDetailId) && x.ObjectType == "1" && listOptionId.Contains(x.ObjectId))
                                        .Select(x => new CustomerOrderExtensionEntityModel
                                        {
                                            Id = x.Id,
                                            OrderDetailId = x.OrderDetailId,
                                            AttributeConfigurationId = x.AttributeConfigurationId,
                                            ObjectId = x.ObjectId,
                                            ObjectType = x.ObjectType,
                                            Value = x.Value,
                                            DataType = x.DataType,
                                            ServicePacketMappingOptionsId = x.ServicePacketMappingOptionsId,
                                        }).ToList();

                //Lấy cấu hình thuộc tính của tất cả tùy chọn
                var listAttrConfiId = listAtrrOption.Select(x => x.AttributeConfigurationId).ToList();
                var listAllAttributeConfiguration = context.AttributeConfiguration.Where(x => listAttrConfiId.Contains(x.Id)).ToList();

                //Lấy danh mục thuộc tính
                var attrCateTypeId = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == ProductConsts.CategoryTypeCodeATR).CategoryTypeId;
                var listAttr = context.Category.Where(x => x.CategoryTypeId == attrCateTypeId).Select(x => new CategoryEntityModel
                {
                    CategoryId = x.CategoryId,
                    CategoryName = x.CategoryName,
                    CategoryCode = x.CategoryCode,
                }).ToList();

                listAtrrOption.ForEach(item =>
                {
                    var attributeConfigurationItem = listAllAttributeConfiguration.FirstOrDefault(x => x.Id == item.AttributeConfigurationId);
                    if(attributeConfigurationItem != null)
                    {
                        item.AttrName = listAttr.FirstOrDefault(x => x.CategoryId == attributeConfigurationItem.CategoryId)?.CategoryName;
                    }
                });


                var cusName = "";
                var cusAddress = "";
                var cusPhone = "";
                var cusNote = "";

                var customer = context.Customer.FirstOrDefault(x => x.CustomerId == customerOrder.CustomerId);
                if(customer != null)
                {
                     var contactCus = context.Contact.FirstOrDefault(x => x.ObjectId == customer.CustomerId);
                     cusName = customer.CustomerName;
                     cusAddress = contactCus?.Address;
                     cusPhone = contactCus?.Phone;
                     cusNote = "";
                }
                var cusOrderDate = customerOrder.CreatedDate;

                return new ChangeCustomerOrderResult()
                {
                    ListOrderDetail = listDetail,
                    ListOrderDetailExten = listOptionExten,
                    ListAtrrOption = listAtrrOption,
                    CusName = cusName,
                    CusAddress = cusAddress,
                    CusPhone = cusPhone,
                    CusNote = cusNote,
                    CusOrderDate = cusOrderDate,
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = "Success",
                };
            }
            catch (Exception e)
            {
                return new ChangeCustomerOrderResult()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message,
                };
            }
        }
        public GetVendorAndEmployeeOfPacketResult GetVendorAndEmployeeOfPacket(GetVendorAndEmployeeOfPacketParameter parameter)
        {
            try
            {
                var servicePacket = context.ServicePacket.FirstOrDefault(x => x.Id == parameter.PacketServiceId);
                if (servicePacket == null)
                {
                    return new GetVendorAndEmployeeOfPacketResult()
                    {
                        StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                        MessageCode = "Gói dịch vụ không tồn tại trên hệ thống!",
                    };
                }

                //Danh sách nhiệm vụ nhân viên 
                var empMissionTypeCodeId = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == ProductConsts.CategoryTypeCodeEmpMission).CategoryTypeId;
                var listMission = context.Category.Where(x => x.CategoryTypeId == empMissionTypeCodeId).Select(x => new CategoryEntityModel
                {
                    CategoryId = x.CategoryId,
                    CategoryName = x.CategoryName
                }).ToList();

                //Lấy các nhân viện tại bước hộ trợ dịch vụ: 
                var cateTypeIdOfStep = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == ProductConsts.CategoryTypeCodeActionStep).CategoryTypeId;
                var supportStepId = context.Category.FirstOrDefault(x => x.CategoryTypeId == cateTypeIdOfStep && x.CategoryCode == ProductConsts.CategoryCodeSupportStep).CategoryId;

                var listConfigurePermissionId = context.PermissionConfiguration.FirstOrDefault(x => x.ServicePacketId == parameter.PacketServiceId
                 && x.CategoryId == supportStepId).Id;

                var listEmpId = context.EmployeeMappingPermissionConfiguration.Where(x => listConfigurePermissionId == x.PermissionConfigurationId).Select(x => x.EmployeeId).ToList();

                var listEmp = context.Employee.Where(x => listEmpId.Contains(x.EmployeeId)).Select(x => new EmployeeEntityModel
                {
                    EmployeeId = x.EmployeeId,
                    EmployeeCode = x.EmployeeCode,
                    EmployeeName = x.EmployeeName,
                    EmployeeCodeName = x.EmployeeCode + " - " + x.EmployeeName,
                    Mission = x.Mission != null ? listMission.FirstOrDefault(y => y.CategoryId == x.Mission).CategoryName : ""
                }).ToList();

                //Lấy list nhà cung cấp
                var listOptionId = context.ServicePacketMappingOptions.Where(x => x.OptionId != null && x.ServicePacketId == parameter.PacketServiceId).Select(x => x.OptionId).ToList();
                var listVendorMappingOption = context.VendorMappingOption.Where(x => listOptionId.Contains(x.OptionId)).ToList();
                var listVendorId = listVendorMappingOption.Select(x => x.VendorId).ToList();

                var listVendor = context.Vendor.Where(x => listVendorId.Contains(x.VendorId)).Select(x => new VendorEntityModel
                {
                    VendorId = x.VendorId,
                    VendorName = x.VendorName,
                    ListoptionId = listVendorMappingOption.Where(y => y.VendorId == x.VendorId).Select(z => z.OptionId).ToList()
                }).ToList();

                return new GetVendorAndEmployeeOfPacketResult()
                {
                    ListEmp = listEmp,
                    ListVendor = listVendor,
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = "Success",
                };
            }
            catch (Exception e)
            {
                return new GetVendorAndEmployeeOfPacketResult()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message,
                };
            }
        }
        public CreateOrUpdateCustomerOrderActionResult CreateOrUpdateCustomerOrderAction(CreateOrUpdateCustomerOrderActionParameter parameter)
        {
            try
            {
                var mess = "";
                var listEmp = new List<Guid>();
                //Tạo mới
                var listStatusOrderAction = GeneralList.GetTrangThais("CustomerOrderAction").ToList();
                if(parameter.CustomerOrderActionId == null)
                {
                    mess = "Tạo phiếu hỗ trợ thành công!";
                    //Tìm phiếu yêu cầu dịch vụ
                    var customerOrder = context.CustomerOrder.FirstOrDefault(x => x.OrderId == parameter.CustomerOrderId);
                    //Tạo phiếu hỗ trợ (clone từ phiếu yêu cầu nhưng đổi isOrderAction = true và id)
                    var customerOrderAction = customerOrder;
                    customerOrderAction.OrderId = Guid.NewGuid();
                    customerOrderAction.ObjectId = parameter.CustomerOrderId;
                    customerOrder.OrderCode = GenerateOrderCodeNew(null, parameter, true, null , null);
                    customerOrderAction.IsOrderAction = true;
                    customerOrderAction.StatusOrder = listStatusOrderAction.FirstOrDefault(x => x.Value == 1).Value; // mới
                    customerOrderAction.CreatedById = parameter.UserId;
                    customerOrderAction.CreatedDate = DateTime.Now;
                    context.CustomerOrder.Add(customerOrderAction);
                    parameter.CustomerOrderActionId = customerOrderAction.OrderId;

                    //Lưu thông tin về các task kèm theo nhân viên phụ trách và điểm báo cáo

                    //Lưu các điểm báo cáo trong tùy chọn dịch vụ vào phiếu
                    var listServiceMappingOptionId = context.CustomerOrderDetail.Where(x => x.OrderId == parameter.CustomerOrderId).OrderBy(x => x.Stt).Select(x => x.OptionId).ToList();
                    //Lấy các optionId
                    var listServiceMappingOption = context.ServicePacketMappingOptions.Where(x => listServiceMappingOptionId.Contains(x.Id)).ToList();
                    var listOptionId = listServiceMappingOption.Select(x => x.OptionId).ToList();
                    //Lấy cái điểm báo cáo
                    var listReportOption = context.MilestoneConfiguration.Where(x => listOptionId.Contains(x.OptionId)).ToList();

                    var listReportPoint = new List<ReportPoint>();
                    var listSettingEmpToTask = new List<CustomerOrderTask>();
                    var listMappingEmpTask = new List<OrderTaskMappingEmp>();

                    dynamic index = 1;
                    var indexReport = 1;
                    parameter.ListSettingEmpToTask.ForEach(obj =>
                    {
                        var newObj = _mapper.Map<CustomerOrderTask>(obj);
                        newObj.Id = Guid.NewGuid();
                        newObj.Stt = index;
                        newObj.OrderActionId = customerOrderAction.OrderId;
                        index++;
                        listSettingEmpToTask.Add(newObj);
                        var optionId = listServiceMappingOption.FirstOrDefault(x => x.Id == obj.ServicePacketMappingOptionsId)?.OptionId;
                        if (optionId != null)
                        {
                            var listReport = listReportOption.Where(x => x.OptionId == optionId).ToList();
                            listReport.ForEach(item =>
                            {
                                var reportPoint = new ReportPoint();
                                reportPoint.Id = Guid.NewGuid();
                                reportPoint.Name = item.Name;
                                reportPoint.OptionId = item.OptionId;
                                reportPoint.OrderActionId = customerOrderAction.OrderId;
                                reportPoint.Order = indexReport;
                                reportPoint.EmpId = Guid.Empty;
                                reportPoint.IsCusView = false;
                                reportPoint.ServicePacketMappingOptionsId = obj.ServicePacketMappingOptionsId;
                                reportPoint.Content = "";
                                reportPoint.Status = GeneralList.GetTrangThais("ReportPoint").FirstOrDefault(x => x.Value == 1).Value;
                                reportPoint.CreatedDate = DateTime.Now;
                                reportPoint.CreatedById = parameter.UserId;
                                reportPoint.UpdatedById = parameter.UserId;
                                listReportPoint.Add(reportPoint);
                                indexReport++;
                            });
                        }
                        //Thêm nhân viên hỗ trợ mapping task
                        obj.ListEmpId.ForEach(emp => { 
                             var mappingEmpTask = new OrderTaskMappingEmp();
                            mappingEmpTask.Id = Guid.NewGuid();
                            mappingEmpTask.EmployeeId = emp;
                            mappingEmpTask.CustomerOrderTaskId = newObj.Id;
                            listMappingEmpTask.Add(mappingEmpTask);
                        });
                    });

                    //Cập nhật bước quy trình: OrderProcessDetail
                    var result = UpdateOrderProcess(ProductConsts.CategoryCodeCforderStep, customerOrderAction.OrderProcessId, customerOrderAction.OrderType.Value,
                        true, customerOrder.OrderId, null);

                    //Nếu có lỗi thì báo lỗi
                    if (result.StatusCode == System.Net.HttpStatusCode.ExpectationFailed)
                    {
                        return new CreateOrUpdateCustomerOrderActionResult
                        {
                            StatusCode = HttpStatusCode.ExpectationFailed,
                            MessageCode = result.Message,
                        };
                    }

                    context.CustomerOrderTask.AddRange(listSettingEmpToTask);
                    context.OrderTaskMappingEmp.AddRange(listMappingEmpTask);
                    context.ReportPoint.AddRange(listReportPoint);
                    context.SaveChanges();

                    var eventCode = "F";

                    //Lấy list thông báo
                    var notificationCateTypeId = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == ProductConsts.CategoryTypeCodeNotificationConfig).CategoryTypeId;
                    var listNotifi = context.Category.Where(x => x.CategoryTypeId == notificationCateTypeId).ToList();

                    var listNotificationConfiguration = context.NotificationConfiguration.Where(x => x.ServicePacketId == customerOrderAction.ServicePacketId).ToList();
                    var listNotificationConfigurationId = listNotificationConfiguration.Select(x => x.Id).ToList();
                    var listEmpNoti = context.EmployeeMappingNotificationConfiguration.Where(x => listNotificationConfigurationId.Contains(x.NotificationConfigurationId.Value)).ToList();

                    var datDichVuId = listNotifi.FirstOrDefault(x => x.CategoryCode == eventCode).CategoryId;
                    var listEmpId = new List<Guid>();
                    var notiConfigure = listNotificationConfiguration.FirstOrDefault(x => x.CategoryId == datDichVuId);
                    listEmp = listEmpNoti.Where(x => x.NotificationConfigurationId == notiConfigure.Id).Select(x => x.EmployeeId.Value).ToList();

                    var notificationReceiver = CommonHelper.GetListEmployeeIdNotifi(context, customerOrder, notiConfigure, null, null);
                    var getListEmpId = notificationReceiver.ListEmployeeId;
                    var deviceId = context.User.Where(x => x.EmployeeId == notificationReceiver.CustomerId)?.FirstOrDefault()?.DeviceId;
                    listEmp.AddRange(getListEmpId);
                    if (deviceId != null)
                    {
                        var body = "Phiếu " + customerOrder.OrderCode + ": " + mess;
                        var title = "Phiếu " + customerOrder.OrderCode;
                        var type = 2; //1 : order, 2: orderAction

                        CommonHelper.PushNotificationToDevice(deviceId, title, body, type.ToString(), customerOrder.OrderId.ToString());
                    }
                }
                //Cập nhật
                else
                {
                    mess = "Cập nhật phiếu hỗ trợ thành công!";
                    //Tìm phiếu hỗ trợ
                    var customerOrderAction = context.CustomerOrder.FirstOrDefault(x => x.OrderId == parameter.CustomerOrderActionId
                                            && x.IsOrderAction == true);
                    if (customerOrderAction == null)
                    {
                        return new CreateOrUpdateCustomerOrderActionResult()
                        {
                            StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                            MessageCode = "Không tìm thấy phiếu hỗ trợ trên hệ thống!",
                        };
                    }

                    //Lấy ra người cần nhận thông báo ở sự kiện hoàn thành cấu hình và chuyển sang thực hiện
                    if (parameter.StatusOrderAction == 2)
                    {
                        //check phải có ít nhất 1 điểm báo cáo mới cho chuyển sang trạng thái thực hiện
                        var listReportPoint = context.ReportPoint.Where(x => x.OrderActionId == customerOrderAction.OrderId).ToList();
                        if (listReportPoint.Count() == 0)
                        {
                            return new CreateOrUpdateCustomerOrderActionResult()
                            {
                                StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                                MessageCode = "Phải có ít nhất điểm báo cáo!",
                                CustomerOrderActionId = parameter.CustomerOrderActionId,
                            };
                        }
                    }

                    customerOrderAction.ObjectId = parameter.CustomerOrderId;
                    customerOrderAction.UpdatedById = parameter.UserId;
                    customerOrderAction.UpdatedDate = DateTime.Now;
                    if(parameter.StatusOrderAction != null) customerOrderAction.StatusOrder = GeneralList.GetTrangThais("CustomerOrderAction").FirstOrDefault(x => x.Value == parameter.StatusOrderAction).Value; 
                    context.CustomerOrder.Update(customerOrderAction);
                    parameter.CustomerOrderActionId = customerOrderAction.OrderId;

                    //Xóa thông tin cũ 
                    var listOldTaskEmp = context.CustomerOrderTask.Where(x => x.OrderActionId == customerOrderAction.OrderId).ToList();


                    var listOldTaskEmpId = listOldTaskEmp.Select(x => x.Id).ToList();
                    var listOldEmpMappingTask = context.OrderTaskMappingEmp.Where(x => listOldTaskEmpId.Contains(x.CustomerOrderTaskId)).ToList();
                    context.CustomerOrderTask.RemoveRange(listOldTaskEmp);
                    context.OrderTaskMappingEmp.RemoveRange(listOldEmpMappingTask);

                    //Lưu thông tin về các task kèm theo nhân viên phụ trách
                    var listSettingEmpToTask = new List<CustomerOrderTask>();
                    var listMappingEmpTask = new List<OrderTaskMappingEmp>();
                    dynamic index = 1;
                    parameter.ListSettingEmpToTask.ForEach(item =>
                    {
                        var newObj = _mapper.Map<CustomerOrderTask>(item);
                        newObj.Id = Guid.NewGuid();
                        newObj.Stt = index;
                        newObj.OrderActionId = customerOrderAction.OrderId;
                        index++;
                        listSettingEmpToTask.Add(newObj);

                        //Thêm nhân viên hỗ trợ mapping task
                        item.ListEmpId.ForEach(emp => {
                            var mappingEmpTask = new OrderTaskMappingEmp();
                            mappingEmpTask.Id = Guid.NewGuid();
                            mappingEmpTask.EmployeeId = emp;
                            mappingEmpTask.CustomerOrderTaskId = newObj.Id;
                            listMappingEmpTask.Add(mappingEmpTask);
                        });

                    });

                    //Nếu là phiếu bổ sung thì k cần cập nhật orderprocess vì k có
                    if (parameter.StatusOrderAction != null && customerOrderAction.OrderType == 1) 
                    {
                        var stepCode = ProductConsts.CategoryCodeSupportStep;
                        //Cập nhật bước quy trình: OrderProcessDetail
                        var result = UpdateOrderProcess(stepCode, customerOrderAction.OrderProcessId, customerOrderAction.OrderType.Value,
                            true, customerOrderAction.OrderId , parameter.StatusOrderAction);
                        //Nếu có lỗi thì báo lỗi
                        if (result.StatusCode == System.Net.HttpStatusCode.ExpectationFailed)
                        {
                            return new CreateOrUpdateCustomerOrderActionResult
                            {
                                StatusCode = HttpStatusCode.ExpectationFailed,
                                MessageCode = result.Message,
                            };
                        }

                        if (parameter.StatusOrderAction == 3)
                        {
                            stepCode = ProductConsts.CategoryCodeDoneStep;
                            //Cập nhật bước quy trình: OrderProcessDetail
                            var result1 = UpdateOrderProcess(stepCode, customerOrderAction.OrderProcessId, customerOrderAction.OrderType.Value,
                                true, customerOrderAction.OrderId, parameter.StatusOrderAction);

                            //Nếu có lỗi thì báo lỗi
                            if (result1.StatusCode == System.Net.HttpStatusCode.ExpectationFailed)
                            {
                                return new CreateOrUpdateCustomerOrderActionResult
                                {
                                    StatusCode = HttpStatusCode.ExpectationFailed,
                                    MessageCode = result1.Message,
                                };
                            }

                        }
                    }
                    //Nếu từ phát sinh và muốn xác nhận hoàn thành
                    else if (parameter.StatusOrderAction == 3 && customerOrderAction.OrderType == 2)
                    {
                        //Kiểm tra xem các điểm báo cáo đã hoàn thành chưa => chưa thì trả về cảnh báo
                        var reportPointDoneCount = context.ReportPoint.Where(x => x.OrderActionId == parameter.CustomerOrderActionId && x.Status != 3).Count();
                        if(reportPointDoneCount != 0)
                        {
                            return new CreateOrUpdateCustomerOrderActionResult()
                            {
                                StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                                MessageCode = "Hãy hoàn thành các điểm báo cáo trước khi xác nhận hoàn thành phiếu!",
                                CustomerOrderActionId = parameter.CustomerOrderActionId,
                            };
                        }
                        //Kiểm tra các phiếu khác gán với orderProcess xem đã hoàn thành hết chưa. Nếu hoàn thành thì chuyển trạng thái hỗ trợ thành hoàn thành.
                        else{
                            var listAllOrder = context.CustomerOrder.Where(x => x.OrderProcessId == customerOrderAction.OrderProcessId).ToList();
                            //Kiểm tra xem có các phiếu yêu cầu bổ sung phát sinh ở trạng thái khác hủy hoặc đã từ chối phát sinh, từ chối chi phí, từ chối phiếu bổ sung (6,7,8, 9)
                            var listOrderExten = listAllOrder.Where(x =>x.OrderProcessId == customerOrderAction.OrderProcessId &&  x.StatusOrder != 6 && x.StatusOrder != 7 && x.StatusOrder != 8 && x.StatusOrder != 9  && x.OrderType == 2).ToList();
                            
                            //Các phiếu đã đươc chuyển sang phiễu hỗ trợ và ở trạng thái hoàn thành
                            var listOrderExtenId = listOrderExten.Select(x => x.OrderId).ToList();
                            var listOrderAction = listAllOrder.Where(x => x.StatusOrder == 3 && x.ObjectId != null && listOrderExtenId.Contains(x.ObjectId.Value) && x.IsOrderAction == true && x.StatusOrder == 3).ToList();
                            //Nếu số lượng phiếu yêu cầu phát sinh so với phiếu hỗ trợ đã hoàn thành tương ứng bằng nhau và phiếu chính cũng hoàn thành => hoàn thành hỗ trợ dịch vụ
                            if (listOrderExten.Count - 1 == listOrderAction.Count )
                            {
                                //Kiểm tra phiếu gốc. Nếu các điểm báo cáo của phiếu gốc đều hoàn thành => hoàn thành hỗ trợ dịch vụ
                                var customerOrderActionRoot = listAllOrder.FirstOrDefault(x => x.OrderType == 1 && x.IsOrderAction == true && x.StatusOrder == 2);
                                if (customerOrderActionRoot != null)
                                {
                                    var listReportPoint = context.ReportPoint.Where(x => x.OrderActionId == customerOrderActionRoot.OrderId);
                                    if (listReportPoint.Count() == listReportPoint.Where(x => x.Status == 3).Count())
                                    {
                                        //Lấy categoryId của bước tạo yêu cầu + xác nhận thanh toán
                                        var cateTypeIdOfStep = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == ProductConsts.CategoryTypeCodeActionStep).CategoryTypeId;
                                        var reportStepId = context.Category.FirstOrDefault(x => x.CategoryTypeId == cateTypeIdOfStep && x.CategoryCode == ProductConsts.CategoryCodeSupportStep).CategoryId;

                                        //Cập nhật bước hỗ trợ dịch vụ trong orderprocess là hoàn thành
                                        var orderProcessDetail = context.OrderProcessDetail.FirstOrDefault(x => x.OrderProcessId == customerOrderAction.OrderProcessId
                                                                        && x.CategoryId == reportStepId);
                                        if (orderProcessDetail != null)
                                        {
                                            orderProcessDetail.Status = 3;//Hoàn thành
                                            context.OrderProcessDetail.Update(orderProcessDetail);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    var eventCode = "";
                    //Cập nhật
                    if (parameter.StatusOrderAction == null)
                    {
                        eventCode = "I";
                    }

                    //Lấy ra người cần nhận thông báo ở sự kiện hoàn thành cấu hình và chuyển sang thực hiện
                    if (parameter.StatusOrderAction == 2)
                    {
                        eventCode = "I";
                        //check phải có ít nhất 1 điểm báo cáo mới cho chuyển sang trạng thái thực hiện
                        var listReportPoint = context.ReportPoint.Where(x => x.OrderActionId == customerOrderAction.OrderId).ToList();
                        if(listReportPoint.Count() == 0)
                        {
                            return new CreateOrUpdateCustomerOrderActionResult()
                            {
                                StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                                MessageCode = "Phải có ít nhất điểm báo cáo!",
                                CustomerOrderActionId = parameter.CustomerOrderActionId,
                            };
                        }
                    }
                    //Hoàn thành yêu cầu
                    if (parameter.StatusOrderAction == 3)
                    {
                        mess = "Xác nhận hoàn thành dịch vụ!";
                        eventCode = "DoneSer";
                    }

                    context.OrderTaskMappingEmp.AddRange(listMappingEmpTask);
                    context.CustomerOrderTask.AddRange(listSettingEmpToTask);
                    context.SaveChanges();

                    //Lấy list thông báo
                    var notificationCateTypeId = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == ProductConsts.CategoryTypeCodeNotificationConfig).CategoryTypeId;
                    var listNotifi = context.Category.Where(x => x.CategoryTypeId == notificationCateTypeId).ToList();

                    var listNotificationConfiguration = context.NotificationConfiguration.Where(x => x.ServicePacketId == customerOrderAction.ServicePacketId).ToList();
                    var listNotificationConfigurationId = listNotificationConfiguration.Select(x => x.Id).ToList();
                    var listEmpNoti = context.EmployeeMappingNotificationConfiguration.Where(x => listNotificationConfigurationId.Contains(x.NotificationConfigurationId.Value)).ToList();

                    var datDichVuId = listNotifi.FirstOrDefault(x => x.CategoryCode == eventCode).CategoryId;
                    var listEmpId = new List<Guid>();
                    var notiConfigure = listNotificationConfiguration.FirstOrDefault(x => x.CategoryId == datDichVuId);
                    listEmp = listEmpNoti.Where(x => x.NotificationConfigurationId == notiConfigure.Id).Select(x => x.EmployeeId.Value).ToList();
                    var notificationReceiver = CommonHelper.GetListEmployeeIdNotifi(context, customerOrderAction, notiConfigure, null, null);
                    var getListEmpId = notificationReceiver.ListEmployeeId;
                    var deviceId = context.User.Where(x => x.EmployeeId == notificationReceiver.CustomerId)?.FirstOrDefault()?.DeviceId;
                    listEmp.AddRange(getListEmpId);
                    if (deviceId != null)
                    {
                        var body = "Phiếu " + customerOrderAction.OrderCode + ": " + mess;
                        var title = "Phiếu " + customerOrderAction.OrderCode;
                        var type = 2; //1 : order, 2: orderAction

                        CommonHelper.PushNotificationToDevice(deviceId, title, body, type.ToString(), customerOrderAction.OrderId.ToString());
                    }
                }

                return new CreateOrUpdateCustomerOrderActionResult()
                {
                   
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = mess,
                    CustomerOrderActionId = parameter.CustomerOrderActionId,
                    ListEmpId = listEmp.Distinct().ToList(),
                };
            }
            catch (Exception e)
            {
                return new CreateOrUpdateCustomerOrderActionResult()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message,
                };
            }
        }
        public GetMasterDataOrderActionDetailResult GetMasterDataOrderActionDetail(GetMasterDataOrderActionDetailParameter parameter)
        {
            try
            {
                var userLogin = context.User.FirstOrDefault(x => x.UserId == parameter.UserId);
                var empLogin = context.Employee.FirstOrDefault(x => x.EmployeeId == userLogin.EmployeeId);

                var listAllCusOrder = context.CustomerOrder.ToList();
                var listStatusOrderAction = GeneralList.GetTrangThais("CustomerOrderAction").ToList();
                var customerOrderAction = listAllCusOrder.Where(x => x.OrderId == parameter.OrderActionId && x.IsOrderAction == true).Select(x => new CustomerOrderEntityModel()
                {
                    OrderId = x.OrderId,
                    OrderCode = x.OrderCode,
                    OrderType = x.OrderType,
                    StatusOrder = x.StatusOrder,
                    OrderProcessId = x.OrderProcessId,
                    CreatedById = x.CreatedById,
                    CreatedDate = x.CreatedDate,
                    CustomerId = x.CustomerId,
                    ServicePacketId = x.ServicePacketId,
                    ObjectId = x.ObjectId,
                    IsOrderAction = x.IsOrderAction,
                    StatusOrderName = listStatusOrderAction.FirstOrDefault(y => y.Value == x.StatusOrder).Name
                }).FirstOrDefault();

                if (customerOrderAction == null)
                {
                    return new GetMasterDataOrderActionDetailResult()
                    {
                        StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                        MessageCode = "Không tồn tại phiếu hỗ trợ dịch vụ trên hệ thống!",

                    };
                }

                var creatorUser = context.User.FirstOrDefault(x => x.UserId == customerOrderAction.CreatedById);
                var empNameCreator = context.Employee.FirstOrDefault(x => x.EmployeeId == creatorUser.EmployeeId)?.EmployeeName;

                #region phân quyền 
                //Lấy categoryId của bước tạo yêu cầu + xác nhận thanh toán
                var cateTypeIdOfStep = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == ProductConsts.CategoryTypeCodeActionStep).CategoryTypeId;
                var createOrderActionStepId = context.Category.FirstOrDefault(x => x.CategoryTypeId == cateTypeIdOfStep && x.CategoryCode == ProductConsts.CategoryCodeCforderStep).CategoryId;
                var reportStepId = context.Category.FirstOrDefault(x => x.CategoryTypeId == cateTypeIdOfStep && x.CategoryCode == ProductConsts.CategoryCodeSupportStep).CategoryId;
                var doneOrderActionStepId = context.Category.FirstOrDefault(x => x.CategoryTypeId == cateTypeIdOfStep && x.CategoryCode == ProductConsts.CategoryCodeDoneStep).CategoryId;

                //Lấy list các bước tạo phiếu hỗ trợ và báo cáo của phiếu 
                var listAllStepPack = context.PermissionConfiguration
                        .Where(x => (x.CategoryId == createOrderActionStepId || x.CategoryId == reportStepId || x.CategoryId == doneOrderActionStepId) && x.ServicePacketId == customerOrderAction.ServicePacketId).ToList();

                var listAllStepPackId = listAllStepPack.Select(x => x.Id).ToList();
                var listAllEmpPermissionConfigure = context.EmployeeMappingPermissionConfiguration.Where(x => listAllStepPackId.Contains(x.PermissionConfigurationId)).ToList();

                //Bước tạo phiếu hỗ trợ
                var createStep = listAllStepPack.FirstOrDefault(x => x.CategoryId == createOrderActionStepId);
                var listEmpIdCreateId = listAllEmpPermissionConfigure.Where(x => x.PermissionConfigurationId == createStep.Id).Select(x => x.EmployeeId).ToList();
                if(empLogin != null) {
                    if (listEmpIdCreateId.Contains(empLogin.EmployeeId))
                    {
                        customerOrderAction.IsCreateAction = true;
                    }
                    //Bước thực hiện báo cáo
                    var confirmStep = listAllStepPack.FirstOrDefault(x => x.CategoryId == reportStepId);
                    var listEmpIdConfirm = listAllEmpPermissionConfigure.Where(x => x.PermissionConfigurationId == confirmStep.Id).Select(x => x.EmployeeId).ToList();
                    if (listEmpIdConfirm.Contains(empLogin.EmployeeId))
                    {
                        customerOrderAction.IsReport = true;
                    }
                    //Bước thực xác nhận hoàn thành hỗ trợ
                    var doneStep = listAllStepPack.FirstOrDefault(x => x.CategoryId == doneOrderActionStepId);
                    var listEmpIdDone = listAllEmpPermissionConfigure.Where(x => x.PermissionConfigurationId == doneStep.Id).Select(x => x.EmployeeId).ToList();
                    if (listEmpIdDone.Contains(empLogin.EmployeeId))
                    {
                        customerOrderAction.IsComplete = true;
                    }
                }
                //Kiểm tra xem đã đến bước thực hiện hỗ trợ chưa ( phiếu yêu cầu không phải là phiếu bổ sung)
                var isActionStep = false;
                var isConfirmStep = false;
                if (customerOrderAction.OrderType == 1)
                {
                    //Thông tin các bước của phiếu
                    var listAllStep = context.OrderProcessDetail.Where(x => x.OrderProcessId == customerOrderAction.OrderProcessId).OrderBy(x => x.StepId).ToList();
                   
                    //vị trí của bước thực hiện hỗ trợ dịch vụ
                    var reportStepOrder = listAllStep.FirstOrDefault(x => x.CategoryId == reportStepId);
                    var indexReportOrder = listAllStep.IndexOf(reportStepOrder);
                    //Xem bước trước nó đã hoàn thành chưa. Nếu hoàn thành rồi thì xác nhận thanh toán là bước hiện tại
                    if (listAllStep[indexReportOrder - 1].Status == 3)
                    {
                        isActionStep = true;
                    }

                    //vị trí của bước xác nhận hoàn thành hỗ trợ
                    var confirmReportOrder = listAllStep.FirstOrDefault(x => x.CategoryId == doneOrderActionStepId);
                    var indexConfirmReportStep = listAllStep.IndexOf(confirmReportOrder);
                    //Xem bước trước nó đã hoàn thành chưa. Nếu hoàn thành rồi thì xác nhận thanh toán là bước hiện tại
                    if (listAllStep[indexConfirmReportStep - 1].Status == 3)
                    {
                        isConfirmStep = true;
                    }
                }
                //Phiếu bổ sung sẽ không theo luồng từ bảng orderprocess
                if (customerOrderAction.OrderType == 2)
                {
                    isActionStep = true;
                    isConfirmStep = true;
                }
                #endregion

                //Danh sách nhiệm vụ nhân viên
                var empMissionTypeCodeId = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == ProductConsts.CategoryTypeCodeEmpMission).CategoryTypeId;
                var listMission = context.Category.Where(x => x.CategoryTypeId == empMissionTypeCodeId).Select(x => new CategoryEntityModel
                {
                    CategoryId = x.CategoryId,
                    CategoryName = x.CategoryName
                }).ToList();

                //Lấy các list task và emp của phiếu hỗ trợ
                 var listCustomerOrderTask = context.CustomerOrderTask
                    .Where(x => x.OrderActionId == parameter.OrderActionId)
                    .Select(x => new CustomerOrderTaskEntityModel
                    {
                        Id = x.Id,
                        ServicePacketMappingOptionsId = x.ServicePacketMappingOptionsId,
                        VendorId = x.VendorId,
                        EmpId = x.EmpId,
                        Note = x.Note,
                        isExtend = x.IsExtend,
                        Path = x.Path,
                        OrderActionId = x.OrderActionId,
                        CreatedDate = x.CreatedDate,
                        Stt = x.Stt,
                    }).OrderBy(x => x.Stt).ToList();

                foreach (var item in listCustomerOrderTask)
                {
                    item.IsPersonInCharge = ((empLogin == null) ? false : (item.EmpId == empLogin.EmployeeId ? true : false));
                }

                //Nhân viên
                var listOrderTaskId = listCustomerOrderTask.Select(x => x.Id).ToList();
                var listAllMapping = context.OrderTaskMappingEmp.Where(x => listOrderTaskId.Contains(x.CustomerOrderTaskId)).ToList();
                var listAllEmpId = listAllMapping.Select(x => x.EmployeeId).ToList();
                var listAllEmp = context.Employee.Where(x => listAllEmpId.Contains(x.EmployeeId)).ToList();
                var listAllEmpContact = context.Contact.Where(x => listAllEmpId.Contains(x.ObjectId) && x.ObjectType == "EMP").ToList();
                //Nhà cung cấp
                var listAllVendorId = listCustomerOrderTask.Select(x => x.VendorId).Distinct().ToList();
                var listAllVendor = context.Vendor.Where(x => listAllVendorId.Contains(x.VendorId)).ToList();
                //Các gói dịch vụ
                var listAllServicePacketMappingOptionsId = listCustomerOrderTask.Select(x => x.ServicePacketMappingOptionsId).Distinct().ToList();
                var listAllServicePacketMappingOptions = context.ServicePacketMappingOptions.Where(x => listAllServicePacketMappingOptionsId.Contains(x.Id)).ToList();

                listCustomerOrderTask.ForEach(item =>
                {
                    var listMapping = listAllMapping.Where(x => x.CustomerOrderTaskId == item.Id).Select(x => x.EmployeeId).ToList();

                    var emp = listAllEmp.Where(x => listMapping.Contains(x.EmployeeId)).ToList();
                    var contact = listAllEmpContact.Where(x => listMapping.Contains(x.ObjectId)).ToList();
                    var servicePacketMappingOptionsItem = listAllServicePacketMappingOptions.FirstOrDefault(x => x.Id == item.ServicePacketMappingOptionsId);
                    item.OptionId = servicePacketMappingOptionsItem?.OptionId;
                    item.ServicePacketId = customerOrderAction.ServicePacketId;
                    item.VendorName = listAllVendor.FirstOrDefault(x => x.VendorId == item.VendorId)?.VendorName;
                    item.ListEmpId = new List<Guid>();
                    item.ListEmpName = "";
                    emp.ForEach(obj => {
                        item.EmpPhone = contact.FirstOrDefault(x => x.ObjectId == obj.EmployeeId)?.Phone;
                        item.ListEmpId.Add(obj.EmployeeId);
                        item.ListEmpName += ", " + obj.EmployeeName;
                    });
                });

                //Lấy phiếu hỗ trợ của chính nó để add vào list chose
                var customerOrder = listAllCusOrder.Where(x => x.OrderId == customerOrderAction.ObjectId)
                 .Select(x => new CustomerOrderEntityModel
                 {
                     OrderId = x.OrderId,
                     OrderCode = x.OrderCode,
                     CustomerId = x.CustomerId,
                     CusOrderDate = x.CreatedDate
                 }).First();


                //Lấy các nhân viện tại bước hộ trợ dịch vụ: 
                var listConfigurePermissionId = context.PermissionConfiguration.FirstOrDefault(x => x.ServicePacketId == customerOrderAction.ServicePacketId
                 && x.CategoryId == reportStepId).Id;

                var listEmpId = context.EmployeeMappingPermissionConfiguration.Where(x => listConfigurePermissionId == x.PermissionConfigurationId).Select(x => x.EmployeeId).ToList();

                var listEmp = context.Employee.Where(x => listEmpId.Contains(x.EmployeeId)).Select(x => new EmployeeEntityModel
                {
                    EmployeeId = x.EmployeeId,
                    EmployeeCode = x.EmployeeCode,
                    EmployeeName = x.EmployeeName,
                    EmployeeCodeName = x.EmployeeCode + " - " + x.EmployeeName,
                }).ToList();

                //Thông tin các gói dịch vụ

                var listOptionId = listAllServicePacketMappingOptions.Select(x => x.OptionId).ToList();
                var listOption = context.Options.Where(x => listOptionId.Contains(x.Id)).ToList();

                var listStatuReport = GeneralList.GetTrangThais("ReportPoint").ToList();
                //Lấy các điểm báo cáo
                var listReportPoint = context.ReportPoint.Where(x => x.OrderActionId == parameter.OrderActionId).Select(x => new ReportPointEntityModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    OptionName = listCustomerOrderTask.FirstOrDefault(y => y.ServicePacketMappingOptionsId == x.ServicePacketMappingOptionsId).Path,
                    OrderActionId = x.OrderActionId,
                    ServicePacketMappingOptionsId = x.ServicePacketMappingOptionsId,
                    Order = x.Order,
                    EmpId = x.EmpId,
                    EmpName = listEmp.FirstOrDefault(y => y.EmployeeId == x.EmpId).EmployeeCodeName,
                    Deadline = x.Deadline,
                    IsCusView = x.IsCusView,
                    Content = x.Content,
                    Status = x.Status,
                    StatusName = listStatuReport.FirstOrDefault(y => y.Value == x.Status).Name,
                    UpdatedDate = x.UpdatedDate

                }).OrderBy(x => x.Order).ToList();

                foreach (var item in listReportPoint)
                {
                    item.IsReporter = ((empLogin == null) ? false : (item.EmpId == empLogin.EmployeeId ? true : false));
                }

                return new GetMasterDataOrderActionDetailResult()
                {
                    EmpNameCreator = empNameCreator,
                    IsActionStep = isActionStep,
                    IsConfirmStep = isConfirmStep,
                    ListReportPoint = listReportPoint,
                    CustomerOrderAction = customerOrderAction,
                    ListCustomerOrderTask = listCustomerOrderTask,
                    CustomerOrder = customerOrder,
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = "Success",
                };
            }
            catch (Exception e)
            {
                return new GetMasterDataOrderActionDetailResult()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message,
                };
            }
        }
        private List<CustomerOrderEntityModel> GetListCustomerOrder(CustomerOrderEntityModel customerOrder, List<CustomerOrderEntityModel> list, List<CustomerOrderEntityModel> listAllCustomerOrder)
        {
            //Tìm ra phiếu yêu cầu bổ sung gán với nó
            var orderExten = listAllCustomerOrder.Where(x => x.IsOrderAction == false && x.ObjectId == customerOrder.OrderId).ToList();
            list.AddRange(orderExten);
            if (orderExten.Count > 0)
            {
                orderExten.ForEach(item =>
                {
                    GetListCustomerOrder(item, list, listAllCustomerOrder);
                });
            }
            return list;
        }
        public CreateOrUpdateCustomerReportPointResult CreateOrUpdateCustomerReportPoint(CreateOrUpdateCustomerReportPointParameter parameter)
        {
            try
            {
                var mess = "";
                //Tạo mới
                if (parameter.ReportPoint.Id == null)
                {
                    mess = "Tạo điểm báo cáo thành công!";
                    var newObj = _mapper.Map<ReportPoint>(parameter.ReportPoint);
                    newObj.Id = Guid.NewGuid();
                    newObj.OrderActionId = parameter.OrderActionId;
                    newObj.CreatedDate = DateTime.Now;
                    newObj.CreatedById = parameter.UserId;
                    newObj.Status = GeneralList.GetTrangThais("ReportPoint").FirstOrDefault(x => x.Value == 1).Value; //Mới
                    context.ReportPoint.Add(newObj);
                    parameter.ReportPoint.Id = newObj.Id;

                    #region set số thứ tự
                    var listAllReportPoint = context.ReportPoint.Where(x => x.OrderActionId == parameter.OrderActionId).ToList();

                    if (newObj.Order >= parameter.ReportPoint.Order)
                    {
                        //Lấy tất cả điểm báo cáo của phiếu trừ chính nó và có số thứ tự >= số thứ tự mới nhập để set số thứ tự
                        var listSetOrder = listAllReportPoint.Where(x => x.Order >= parameter.ReportPoint.Order).OrderBy(x => x.Order).ToList();
                        var order = parameter.ReportPoint.Order + 1;
                        listSetOrder.ForEach(item =>
                        {
                            item.Order = order.Value;
                            order++;
                        });
                    }
                    #endregion


                    context.SaveChanges();
                }
                //Cập nhật
                else
                {
                    var listAllReportPoint = context.ReportPoint.Where(x => x.OrderActionId == parameter.OrderActionId).ToList();
                    mess = "Cập nhật điểm báo cáo  thành công!";
                    var reportPoint = listAllReportPoint.FirstOrDefault(x => x.Id == parameter.ReportPoint.Id);
                    if (reportPoint == null)
                    {
                        return new CreateOrUpdateCustomerReportPointResult()
                        {
                            StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                            MessageCode = "Không tìm thấy điểm báo cáo trên hệ thống!",
                        };
                    }

                    #region set số thứ tự
                    //Nếu số thứ tự cũ > số thứ tự mới
                    if(reportPoint.Order >= parameter.ReportPoint.Order)
                    {
                        //Lấy tất cả điểm báo cáo của phiếu trừ chính nó và có số thứ tự >= số thứ tự mới nhập để set số thứ tự
                        var listSetOrder = listAllReportPoint.Where(x => x.Id != reportPoint.Id && x.Order >= parameter.ReportPoint.Order).OrderBy(x => x.Order).ToList();
                        var order = parameter.ReportPoint.Order + 1;
                        listSetOrder.ForEach(item =>
                        {
                            item.Order = order.Value;
                            order++;
                        });
                    }
                    //Nếu số thứ tự cũ < số thứ tự mới
                    else
                    {
                        //Lấy khoảng điểm báo cáo có điều kiện: Stt lớn hơn điểm báo cáo đang sửa và nhỏ hơn số thứ tự mới của báo cáo đang sửa
                        var listSetOrder = listAllReportPoint.Where(x => x.Id != reportPoint.Id && x.Order > reportPoint.Order
                                                                    && x.Order <= parameter.ReportPoint.Order
                                                                    ).OrderBy(x => x.Order).ToList();
                        listSetOrder.ForEach(item => {
                            item.Order = item.Order -1 ;
                        });
                    }
                    #endregion


                    reportPoint.Name = parameter.ReportPoint.Name;
                    reportPoint.OptionId = parameter.ReportPoint.OptionId;
                    reportPoint.Order = parameter.ReportPoint.Order.Value;
                    reportPoint.EmpId = parameter.ReportPoint.EmpId.Value;
                    reportPoint.Deadline = parameter.ReportPoint.Deadline;
                    reportPoint.IsCusView = parameter.ReportPoint.IsCusView;
                    reportPoint.ServicePacketMappingOptionsId = parameter.ReportPoint.ServicePacketMappingOptionsId;
                    reportPoint.Content = parameter.ReportPoint.Content;
                    reportPoint.UpdatedDate = DateTime.Now;
                    reportPoint.UpdatedById = parameter.UserId;

                  

                    context.ReportPoint.Update(reportPoint);
                    context.SaveChanges();
                }
                return new CreateOrUpdateCustomerReportPointResult()
                {

                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = mess,
                    ReportPointId = parameter.ReportPoint.Id.Value,
                };
            }
            catch (Exception e)
            {
                return new CreateOrUpdateCustomerReportPointResult()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message,
                };
            }
        }
        public GetOptionAndEmpOfOrderActionResult GetOptionAndEmpOfOrderAction(GetOptionAndEmpOfOrderActionParameter parameter)
        {
            try
            {

                var orderAction = context.CustomerOrder.FirstOrDefault(x => x.OrderId == parameter.OrderActionId && x.IsOrderAction == true);
                if (orderAction == null)
                {
                    return new GetOptionAndEmpOfOrderActionResult()
                    {
                        StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                        MessageCode = "Phiếu hỗ trợ không tồn tại trên hệ thống!",
                    };
                }

                
                //Lấy các dịch vụ của phiếu hỗ trợ
                var listOption = context.CustomerOrderTask.Where(x => x.OrderActionId == orderAction.OrderId).Select(x => new CustomerOrderTaskEntityModel 
                { 
                    Id = x.Id,
                    ServicePacketMappingOptionsId = x.ServicePacketMappingOptionsId,
                    Path = x.Path,
                }).ToList();

                //Danh sách nhiệm vụ nhân viên
                var empMissionTypeCodeId = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == ProductConsts.CategoryTypeCodeEmpMission).CategoryTypeId;
                var listMission = context.Category.Where(x => x.CategoryTypeId == empMissionTypeCodeId).Select(x => new CategoryEntityModel
                {
                    CategoryId = x.CategoryId,
                    CategoryName = x.CategoryName
                }).ToList();


                var listEmp = new List<EmployeeEntityModel>();
                var listEmpReportPoint = context.ReportPoint.Where(x => x.Status == 2 && x.EmpId != Guid.Empty && x.EmpId != null).Select(x => x.EmpId).Distinct().ToList();
                if (parameter.ServicePacketMappingOptionsId != null)
                {
                    var customerOrderTask = context.CustomerOrderTask.FirstOrDefault(x => x.ServicePacketMappingOptionsId == parameter.ServicePacketMappingOptionsId && x.OrderActionId == parameter.OrderActionId);
                    if(customerOrderTask != null)
                    {
                        var listEmpId = context.OrderTaskMappingEmp.Where(x => x.CustomerOrderTaskId == customerOrderTask.Id).Select(x => x.EmployeeId).ToList();
                        listEmp = context.Employee.Where(x => listEmpId.Contains(x.EmployeeId)).Select(x => new EmployeeEntityModel
                        {
                            EmployeeId = x.EmployeeId,
                            EmployeeCode = x.EmployeeCode,
                            EmployeeName = x.EmployeeName,
                            Mission = x.Mission != null ? listMission.FirstOrDefault(y => y.CategoryId == x.Mission).CategoryName : "",
                            EmployeeCodeName = x.EmployeeCode + " - " + x.EmployeeName,
                            IsDisable = listEmpReportPoint != null ? listEmpReportPoint.Contains(x.EmployeeId) ? true : false : false
                        }).ToList();

                    }
                }
                else
                {
                    //Lấy các nhân viện tại bước hộ trợ dịch vụ: 
                    var cateTypeIdOfStep = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == ProductConsts.CategoryTypeCodeActionStep).CategoryTypeId;
                    var supportStepId = context.Category.FirstOrDefault(x => x.CategoryTypeId == cateTypeIdOfStep && x.CategoryCode == ProductConsts.CategoryCodeSupportStep).CategoryId;

                    var listConfigurePermissionId = context.PermissionConfiguration.FirstOrDefault(x => x.ServicePacketId == orderAction.ServicePacketId
                     && x.CategoryId == supportStepId).Id;
                    var listEmpId = context.EmployeeMappingPermissionConfiguration.Where(x => listConfigurePermissionId == x.PermissionConfigurationId).Select(x => x.EmployeeId).ToList();
                    listEmp = context.Employee.Where(x => listEmpId.Contains(x.EmployeeId)).Select(x => new EmployeeEntityModel
                    {
                        EmployeeId = x.EmployeeId,
                        EmployeeCode = x.EmployeeCode,
                        EmployeeName = x.EmployeeName,
                        Mission = x.Mission != null ? listMission.FirstOrDefault(y => y.CategoryId == x.Mission).CategoryName : "",
                        EmployeeCodeName = x.EmployeeCode + " - " + x.EmployeeName,
                        IsDisable = listEmpReportPoint != null ? listEmpReportPoint.Contains(x.EmployeeId) ? true : false : false
                    }).ToList();
                }

                return new GetOptionAndEmpOfOrderActionResult()
                {
                    ListSupporter = listEmp,
                    ListOption = listOption,
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = "Success",
                };
            }
            catch (Exception e)
            {
                return new GetOptionAndEmpOfOrderActionResult()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message,
                };
            }
        }
        public ChangeStatusReportPointResult ChangeStatusReportPoint(ChangeStatusReportPointParameter parameter)
        {
            try
            {
                var reportPoint = context.ReportPoint.FirstOrDefault(x => x.Id == parameter.ReportPointId);
                if (reportPoint == null)
                {
                    return new ChangeStatusReportPointResult()
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = "Không tìm thấy điểm báo cáo trên hệ thống!"
                    };
                }
                reportPoint.Status = parameter.Status;
                context.ReportPoint.Update(reportPoint);

                var note = new Note();
                note.NoteId = Guid.NewGuid();
                note.Description = parameter.Status == 2 ? "Đổi trạng thái sang đang thực hiện" : "Đổi trạng thái sang hoàn thành";
                note.Type = "SYS";
                note.ObjectId = parameter.ReportPointId;
                note.ObjectNumber = null;
                note.ObjectType = "Report_Point";
                note.Active = true;
                note.CreatedById = parameter.UserId;
                note.CreatedDate = DateTime.Now;
                note.NoteTitle = "Đã thêm ghi chú";
                context.Note.Add(note);

                var listAllCustomerOrder = context.CustomerOrder.ToList();
                var listAllReport = context.ReportPoint.Where(x => x.OrderActionId == reportPoint.OrderActionId).ToList();
                //Lấy phiếu yêu cầu gốc
                var orderAction = listAllCustomerOrder.FirstOrDefault(x => x.OrderType == 1 && x.OrderId == reportPoint.OrderActionId);
                var listEmp = new List<Guid>();
                //Nếu điểm báo cáo ở phiếu hỗ trợ có nguồn gốc từ phiếu phiếu yêu cầu ( không phải phát sinh: OrderType == 2) => cập nhật trạng thái trong OrderprocessDetail
                if (orderAction != null)
                {
                    //Nếu tất cả điểm báo cáo ở trạng thái hoàn thành => cập nhật orderprocess. Chỉ áp dụng cho orderType == 1 (phiếu yêu cầu bình thưởng)
                    if ((listAllReport.Count()) == listAllReport.Where(x => x.Status == 3).Count())
                    {
                        //Lấy categoryId của bước hỗ trợ dịch vụ
                        var cateTypeIdOfStep = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == ProductConsts.CategoryTypeCodeActionStep).CategoryTypeId;
                        var reportStepId = context.Category.FirstOrDefault(x => x.CategoryTypeId == cateTypeIdOfStep && x.CategoryCode == ProductConsts.CategoryCodeSupportStep).CategoryId;

                        //Kiểm tra xem có các phiếu yêu cầu bổ sung phát sinh trong quy trình hỗ trợ ở trạng thái khác hủy hoặc đã từ chối phát sinh, từ chối chi phí, từ chối phiếu yêu cầu bổ sung (6,7,8, 9)
                        var listOrderExten = listAllCustomerOrder.Where(x => x.OrderProcessId == orderAction.OrderProcessId && x.StatusOrder != 6 && x.StatusOrder != 7 && x.StatusOrder != 8 && x.StatusOrder != 9 && x.OrderType == 2 && x.IsOrderAction == false).ToList();
                        //Các phiếu đã đươc chuyển sang phiễu hỗ trợ và ở trạng thái hoàn thành
                        var listOrderExtenId = listOrderExten.Select(x => x.OrderId).ToList();
                        var listOrderAction = listAllCustomerOrder.Where(x => x.StatusOrder == 3 && x.ObjectId != null && listOrderExtenId.Contains(x.ObjectId.Value) && x.IsOrderAction == true && x.StatusOrder == 3).ToList();

                        //Nếu số lượng phiếu yêu cầu phát sinh so với phiếu hỗ trợ đã hoàn thành tương ứng không bằng nhau => chưa hoàn thành hết toàn bộ việc hỗ trợ
                        if (listOrderExten.Count == listOrderAction.Count)
                        {
                            //Cập nhật bước hỗ trợ dịch vụ trong orderprocess là hoàn thành
                            var orderProcessDetail = context.OrderProcessDetail.FirstOrDefault(x => x.OrderProcessId == orderAction.OrderProcessId
                                                            && x.CategoryId == reportStepId);
                            if (orderProcessDetail != null)
                            {
                                orderProcessDetail.Status = 3;//Hoàn thành
                                context.OrderProcessDetail.Update(orderProcessDetail);
                            }
                        }
                    }
                }
                else
                {
                    var currentOrder = listAllCustomerOrder.FirstOrDefault(x =>  x.OrderId == reportPoint.OrderActionId);
                    //Lấy phiếu yêu cầu gốc
                    orderAction = listAllCustomerOrder.FirstOrDefault(x => x.OrderType == 1 && x.OrderProcessId == currentOrder.OrderProcessId && x.IsOrderAction == true);
                }

                //Lấy list thông báo
                var notificationCateTypeId = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == ProductConsts.CategoryTypeCodeNotificationConfig).CategoryTypeId;
                var listNotifi = context.Category.Where(x => x.CategoryTypeId == notificationCateTypeId).ToList();

                var listNotificationConfiguration = context.NotificationConfiguration.Where(x => x.ServicePacketId == orderAction.ServicePacketId).ToList();
                var listNotificationConfigurationId = listNotificationConfiguration.Select(x => x.Id).ToList();
                var listEmpNoti = context.EmployeeMappingNotificationConfiguration.Where(x => listNotificationConfigurationId.Contains(x.NotificationConfigurationId.Value)).ToList();

                //Lấy ra người cần nhận thông báo ở sự kiện ấn nút thay đổi trạng thái các điểm báo cáo
                var datDichVuId = listNotifi.FirstOrDefault(x => x.CategoryCode == "G").CategoryId;
                var listEmpId = new List<Guid>();
                var notiConfigure = listNotificationConfiguration.FirstOrDefault(x => x.CategoryId == datDichVuId);
                listEmp = listEmpNoti.Where(x => x.NotificationConfigurationId == notiConfigure.Id).Select(x => x.EmployeeId.Value).ToList();
                var notificationReceiver = CommonHelper.GetListEmployeeIdNotifi(context, orderAction, notiConfigure, null, null);
                var getListEmpId = notificationReceiver.ListEmployeeId;
                var deviceId = context.User.Where(x => x.EmployeeId == notificationReceiver.CustomerId)?.FirstOrDefault()?.DeviceId;
                listEmp.AddRange(getListEmpId);
                if (deviceId != null)
                {
                    var body = "Phiếu " + orderAction.OrderCode + ": " + "thay đổi trạng thái";
                    var title = "Phiếu " + orderAction.OrderCode;
                    var type = 2; //1 : order, 2: orderAction

                    CommonHelper.PushNotificationToDevice(deviceId, title, body, type.ToString(), orderAction.OrderId.ToString());
                }

                context.SaveChanges();
                return new ChangeStatusReportPointResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    ListEmpId = listEmp.Distinct().ToList(),
                    MessageCode = parameter.Status == 2 ? "Đang thực hiện!" : "Hoàn thành!"
                };
            }
            catch (Exception e)
            {
                return new ChangeStatusReportPointResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }
        public GetMasterDataCreateOrderProcessResult GetMasterDataCreateOrderProcess(GetMasterDataCreateOrderProcessParameter parameter)
        {
            try
            {
                //Danh sách loại gói dịch vụ
                var listServiceType = context.ProductCategory.Where(x => x.Active == true).Select(x => new ProductCategoryEntityModel {
                    ProductCategoryId = x.ProductCategoryId,
                    ParentId = x.ParentId,
                    ProductCategoryCode = x.ProductCategoryCode,
                    ProductCategoryName = x.ProductCategoryName,
                }).ToList();

                //Danh sách gói dịch vụ
                var listServicePacket = context.ServicePacket.Select(x => new ServicePacketEntityModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    ProductCategoryId = x.ProductCategoryId,
                }).ToList();

                //Danh sách tên các bước bổ sung
                var stepCateTypeId = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == ProductConsts.CategoryTypeCodeActionStep).CategoryTypeId;
                var listStep = context.Category.Where(x => x.CategoryTypeId == stepCateTypeId).ToList();
                //Danh sách qui trình của các gói dịch vụ
                var listProcessOfPack = context.PermissionConfiguration.Select(x => new PermissionConfigurationEntityModel
                {
                    Id = x.Id,
                    StepId = x.StepId,
                    ServicePacketId = x.ServicePacketId,
                    CategoryId = x.CategoryId.Value,
                    CategoryName = listStep.FirstOrDefault(y => y.CategoryId == x.CategoryId).CategoryName,
                    CategoryCode = listStep.FirstOrDefault(y => y.CategoryId == x.CategoryId).CategoryCode
                }).OrderBy(x => x.StepId).ToList();

                //Danh sách khách hàng
                var listCustomer = context.Customer.Where(x => x.Active == true).Select(x => new CustomerEntityModel
                        {
                            CustomerId = x.CustomerId,
                            CustomerName = x.CustomerName,
                        }).ToList();

                var listAllContactCus = context.Contact.Where(x => x.ObjectType == ContactObjectType.CUS).ToList();
                listCustomer.ForEach(item =>
                {
                    var contactCus = listAllContactCus.FirstOrDefault(x => x.ObjectId == item.CustomerId);
                    if (contactCus != null)
                    {
                        item.CustomerPhone = contactCus.Phone;
                        item.FullAddress = contactCus.Address;
                        item.CustomerEmail = contactCus.Email;
                    }
                });


                return new GetMasterDataCreateOrderProcessResult
                {
                    ListServiceType = listServiceType,
                    ListServicePacket = listServicePacket,
                    ListCustomer = listCustomer,
                    ListProcessOfPack = listProcessOfPack,
                    StatusCode = HttpStatusCode.OK,
                };
            }
            catch (Exception e)
            {
                return new GetMasterDataCreateOrderProcessResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }
        public CreateOrUpdateCustomerOrderProcessResult CreateOrUpdateCustomerOrderProcess(CreateOrUpdateCustomerOrderProcessParameter parameter)
        {
            try
            {
                var mess = "";
                //Tạo mới
                var listStatusOrderProcess = GeneralList.GetTrangThais("OrderProcess").ToList();
                var listStatusOrderProcessDetail = GeneralList.GetTrangThais("OrderProcessDetail").ToList();
                if (parameter.OrderProcess.Id == null)
                {
                    mess = "Tạo quy trình đặt dịch vụ thành công!";
                    var newOrderProcess = _mapper.Map<OrderProcess>(parameter.OrderProcess);
                    newOrderProcess.Id = Guid.NewGuid();
                    newOrderProcess.CreatedById = parameter.UserId;
                    newOrderProcess.Status = listStatusOrderProcess.FirstOrDefault(x => x.Value == 1).Value; // mới
                    newOrderProcess.RateStar = 0;
                    newOrderProcess.CreatedDate = DateTime.Now;
                    newOrderProcess.UpdatedById = parameter.UserId;
                    newOrderProcess.UpdatedDate = DateTime.Now;
                    newOrderProcess.OrderProcessCode = GenerateOrderCodeNew(null, null, true, true, parameter);
                    context.OrderProcess.Add(newOrderProcess);
                    parameter.OrderProcess.Id = newOrderProcess.Id;

                    //Lưu thông tin quy trình từ gói vào phiếu cho khách hàng
                    var servicePacket = context.ServicePacket.FirstOrDefault(x => x.Id == parameter.OrderProcess.ServicePacketId);
                    if(servicePacket == null)
                    {
                        return new CreateOrUpdateCustomerOrderProcessResult()
                        {
                            StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                            MessageCode = "Không tìm thấy gói dịch vụ trên hệ thống!",
                        };
                    }

                    //Lấy quy trình 
                    var listProcessPacket = context.PermissionConfiguration.Where(x => x.ServicePacketId == parameter.OrderProcess.ServicePacketId).ToList();
                    var listOrderProcessDetail = new List<OrderProcessDetail>();
                    listProcessPacket.ForEach(item =>
                    {
                        var newObj = new OrderProcessDetail();
                        newObj.Id = Guid.NewGuid();
                        newObj.OrderProcessId = newOrderProcess.Id;
                        newObj.StepId = item.StepId;
                        newObj.ServicePacketId = item.ServicePacketId;
                        newObj.CreatedById = parameter.UserId;
                        newObj.CreatedDate = DateTime.Now;
                        newObj.CategoryId = item.CategoryId;
                        newObj.Status = listStatusOrderProcessDetail.FirstOrDefault(x => x.Value == 1).Value; // mới
                        listOrderProcessDetail.Add(newObj);
                    });
                    context.OrderProcessDetail.AddRange(listOrderProcessDetail);
                    context.SaveChanges();
                }
                //Cập nhật
                else
                {
                    mess = "Cập nhật quy trình đặt dịch vụ thành công!";
                    var orderProcess = context.OrderProcess.FirstOrDefault(x => x.Id == parameter.OrderProcess.Id);
                    if(orderProcess == null)
                    {
                        return new CreateOrUpdateCustomerOrderProcessResult()
                        {
                            StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                            MessageCode = "Không tìm thấy quy trình trên hệ thống!",
                        };
                    }
                    orderProcess.CustomerId = parameter.OrderProcess.CustomerId;
                    orderProcess.ProductCategoryId = parameter.OrderProcess.ProductCategoryId;
                    orderProcess.ServicePacketId = parameter.OrderProcess.ServicePacketId;
                    orderProcess.UpdatedById = parameter.UserId;
                    orderProcess.UpdatedDate = DateTime.Now;

                    //Xóa quy trình cũ 
                    var listOldOrderProcessDetail = context.OrderProcessDetail.Where(x => x.OrderProcessId == orderProcess.Id).ToList();
                    context.OrderProcessDetail.RemoveRange(listOldOrderProcessDetail);

                    //Lấy quy trình 
                    var listProcessPacket = context.PermissionConfiguration.Where(x => x.ServicePacketId == parameter.OrderProcess.ServicePacketId).ToList();
                    var listOrderProcessDetail = new List<OrderProcessDetail>();
                    listProcessPacket.ForEach(item =>
                    {
                        var newObj = new OrderProcessDetail();
                        newObj.Id = Guid.NewGuid();
                        newObj.OrderProcessId = orderProcess.Id;
                        newObj.StepId = item.StepId;
                        newObj.ServicePacketId = item.ServicePacketId;
                        newObj.CreatedById = parameter.UserId;
                        newObj.CreatedDate = DateTime.Now;
                        newObj.CategoryId = item.CategoryId;
                        newObj.Status = listStatusOrderProcessDetail.FirstOrDefault(x => x.Value == 1).Value; // mới
                        listOrderProcessDetail.Add(newObj);
                    });
                    context.OrderProcessDetail.AddRange(listOrderProcessDetail);

                    context.OrderProcess.Update(orderProcess);
                    context.SaveChanges();
                }
                return new CreateOrUpdateCustomerOrderProcessResult()
                {

                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = mess,
                    Id = parameter.OrderProcess.Id.Value
                };
            }
            catch (Exception e)
            {
                return new CreateOrUpdateCustomerOrderProcessResult()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message,
                };
            }
        }
        public GetDetailOrderProcessResult GetDetailOrderProcess(GetDetailOrderProcessParameter parameter)
        {
            try
            {
                var userLogin = context.User.FirstOrDefault(x => x.UserId == parameter.UserId);
                var empLogin = context.Employee.FirstOrDefault(x => x.EmployeeId == userLogin.EmployeeId);

                var listStatusOrderProcess = GeneralList.GetTrangThais("OrderProcess").ToList();
                var listStatusOrderProcessDetail = GeneralList.GetTrangThais("OrderProcessDetail").ToList();
                var orderProcessTemp = context.OrderProcess.Where(x => x.Id == parameter.Id).Select(x => new OrderProcessEntityModel {
                    Id = x.Id,
                    OrderProcessCode = x.OrderProcessCode,
                    RateStar = x.RateStar,
                    RateContent = x.RateContent,
                    CustomerId = x.CustomerId,
                    ProductCategoryId = x.ProductCategoryId,
                    ServicePacketId = x.ServicePacketId,
                    Status = x.Status,
                    StatusName = listStatusOrderProcess.FirstOrDefault(y => y.Value == x.Status).Name,
                    IsCreator = x.CreatedById == parameter.UserId ? true : false
                }).ToList();

                if (orderProcessTemp.Count == 0)
                {
                    return new GetDetailOrderProcessResult()
                    {
                        StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                        MessageCode = "Không tìm thấy quy trình trên hệ thống!",
                    };
                }

                var orderProcess = orderProcessTemp.First();

                var listAllOrder = context.CustomerOrder.ToList();
                //Lấy phiếu yêu cầu gán với quy trình
                var orderId = listAllOrder.FirstOrDefault(x => x.OrderProcessId == parameter.Id && x.IsOrderAction == false && x.ObjectId == null)?.OrderId;
                //Lấy phiếu hỗ trợ gán với quy trình
                var orderActionId = listAllOrder.FirstOrDefault(x => x.OrderProcessId == parameter.Id && x.IsOrderAction == true && x.ObjectId == orderId)?.OrderId;
                var listExtenOrder = listAllOrder.Where(x => x.OrderProcessId == parameter.Id && x.IsOrderAction == false)
                                        .Select(x => new CustomerOrderEntityModel {
                                            OrderId = x.OrderId,
                                            OrderCode = x.OrderCode,
                                            OrderType = x.OrderType,
                                        }).OrderBy(x => x.OrderType).ToList();

                //Lấy những phiếu phát sinh gán với quy trình
                listExtenOrder.ForEach(item =>
                {
                    var orderAction = listAllOrder.FirstOrDefault(x => x.ObjectId == item.OrderId && x.IsOrderAction == true);
                    if(orderAction != null)
                    {
                        item.OrderActionId = orderAction.OrderId;
                        item.OrderActionCode = orderAction.OrderCode;
                    }
                });

                var stepCateTypeId = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == ProductConsts.CategoryTypeCodeActionStep).CategoryTypeId;
                var listStep = context.Category.Where(x => x.CategoryTypeId == stepCateTypeId).ToList();
                var listOrderProcessDetail = context.OrderProcessDetail.Where(x => x.OrderProcessId == parameter.Id).Select(x => new OrderProcessDetailEntityModel
                {
                    Id = x.Id,
                    StepId = x.StepId,
                    ServicePacketId = x.ServicePacketId,
                    CategoryId = x.CategoryId, //Bước trong category
                    CategoryName = listStep.FirstOrDefault(y => y.CategoryId == x.CategoryId).CategoryName,
                    CategoryCode = listStep.FirstOrDefault(y => y.CategoryId == x.CategoryId).CategoryCode,
                    OrderProcessId = x.OrderProcessId,
                    Status = x.Status,
                    StatusName = listStatusOrderProcessDetail.FirstOrDefault(y => y.Value == x.Status).Name,
                    OrderId = orderId,
                    OrderActionId = orderActionId,
                    ListExtenOrder = listExtenOrder,
                }).OrderBy(x => x.StepId).ToList();


                //Phần quyền cho từng bước trong quy trình
                var listAllStepOfPack = context.PermissionConfiguration.Where(x => x.ServicePacketId == orderProcess.ServicePacketId).ToList();
                var listAllStepIdOfPack = listAllStepOfPack.Select(x => x.Id).ToList();
                var listAllStepMappingEmp = context.EmployeeMappingPermissionConfiguration.Where(x => listAllStepIdOfPack.Contains(x.PermissionConfigurationId)).ToList();

                listOrderProcessDetail.ForEach(item =>
                {
                    item.IsAction = false;
                    var currentStepId = listAllStepOfPack.Where(x => x.CategoryId == item.CategoryId).Select(x => x.Id).ToList();
                    var listEmpIdCurrentStep = listAllStepMappingEmp.Where(x => currentStepId.Contains(x.PermissionConfigurationId)).Select(x => x.EmployeeId).ToList();
                    if (listEmpIdCurrentStep.Contains(empLogin.EmployeeId))
                    {
                        item.IsAction = true;
                    }
                });


                context.SaveChanges();
                return new GetDetailOrderProcessResult()
                {
                    OrderProcess = orderProcess,
                    ListOrderProcessDetail = listOrderProcessDetail,
                    StatusCode = System.Net.HttpStatusCode.OK,
                };
            }
            catch (Exception e)
            {
                return new GetDetailOrderProcessResult()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message,
                };
            }
        }
        //Code bước, Quy trình id, loại phiếu (1: yêu cầu, 2: bổ sung), là phiếu yêu cầu hay bổ sung (false | true)
        private BaseResult UpdateOrderProcess(string StepCode, Guid? OrderProcessId,int OrderType, bool isExten, Guid OrderId, int? StatusOrder)
        {
            var result = new BaseResult();
            result.StatusCode = System.Net.HttpStatusCode.OK;
            result.Message = "Cập nhật quy trình thành công!";
            //nếu loại phiếu là phiếu yêu cầu thì mới cập nhật
            if (OrderType == 1)
            {
                //Lấy quy trình
                var orderProcess = context.OrderProcess.FirstOrDefault(x => x.Id == OrderProcessId);
                if (orderProcess == null)
                {
                    result.StatusCode = System.Net.HttpStatusCode.ExpectationFailed;
                    result.Message = "Không tìm thấy quy trình trên hệ thống!";
                    return result;
                }

                //Trạng thái của các bước
                var listStatus = GeneralList.GetTrangThais("OrderProcessDetail").ToList();

                var listAllProcessDetail = context.OrderProcessDetail.Where(x => x.OrderProcessId == OrderProcessId).OrderByDescending(x => x.StepId).ToList();
                //Nếu là bước k mặc định
                if (String.IsNullOrEmpty(StepCode))
                {
                    //Lấy bước trước đó 
                    var lastStep = listAllProcessDetail.Where(x => x.OrderProcessId == OrderProcessId && x.Status == 3).First();
                    var indexOfLastStep = listAllProcessDetail.IndexOf(lastStep);
                    var currentStep = listAllProcessDetail[indexOfLastStep + 1];

                    //và cập nhật thành hoàn thành(status == 3: Hoàn thành)
                    currentStep.Status = listStatus.FirstOrDefault(x => x.Value == 3).Value;
                    context.OrderProcessDetail.Update(currentStep);
                }
                //Nếu là bước mặc định
                else
                {
                    //Lấy ra các bước
                    var cateTypeIdOfStep = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == ProductConsts.CategoryTypeCodeActionStep).CategoryTypeId;
                    var currentStepId = context.Category.FirstOrDefault(x => x.CategoryTypeId == cateTypeIdOfStep && x.CategoryCode == StepCode).CategoryId;

                    //Lấy bước hiện tại và cập nhật thành hoàn thành
                    var currentStep = listAllProcessDetail.FirstOrDefault(x => x.CategoryId == currentStepId);

                    //Nếu là bước khác hỗ trợ dịch vụ mặc định => Chuyển sạng hoàn thành (3)
                    if (StepCode != ProductConsts.CategoryCodeSupportStep)
                    {
                        currentStep.Status = listStatus.FirstOrDefault(x => x.Value == 3).Value;
                    }
                    //Nếu là bước hỗ trợ dịch vụ mặc định => Cập nhật theo biến chuyền vào
                    else if(StepCode == ProductConsts.CategoryCodeSupportStep && isExten)
                    {
                        currentStep.Status = listStatus.FirstOrDefault(x => x.Value == StatusOrder).Value;
                    }
                    context.OrderProcessDetail.Update(currentStep);
                }

                //Cập nhật trạng thái của quy trình (OrderProcess): Nếu tất cả các bước là hoàn thành (3) => trạng thái quy trình chuyển thành hoàn thành
                if (listAllProcessDetail.Where(x => x.Status == 3).Count() == (listAllProcessDetail.Count() - 1))
                {
                    var statusDoneOrderProcess = GeneralList.GetTrangThais("OrderProcess").FirstOrDefault(x => x.Value == 3).Value;
                    orderProcess.Status = statusDoneOrderProcess;
                    context.OrderProcess.Update(orderProcess);
                }

                context.SaveChanges();

            }

            return result;
        }
        public UpdateOrderProcessResult UpdateOrderProcess(UpdateOrderProcessParameter parameter)
        {
            try
            {
                var statusDoneOrderProcess = GeneralList.GetTrangThais("OrderProcess").FirstOrDefault(x => x.Value == 3).Value;
                var statusDoneOrderProcessDetail = GeneralList.GetTrangThais("OrderProcessDetail").FirstOrDefault(x => x.Value == 3).Value;
                var orderProcess = context.OrderProcess.FirstOrDefault(x => x.Id == parameter.OrderProcessId);
                if (orderProcess == null)
                {
                    return new UpdateOrderProcessResult()
                    {
                        StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                        MessageCode = "Không tìm thấy quy trình trên hệ thống!",
                    };
                }

                var mess = "Cập nhật trạng thái thành công!";
                var listOrderProcessDetail = context.OrderProcessDetail.Where(x => x.OrderProcessId == parameter.OrderProcessId).ToList();
                var listEmp = new List<Guid>();
                //Cập nhật trạng thái bước
                if (parameter.OrderProcessDetailId != null)
                {
                    //Mỗi quy trình chỉ có 1 phiếu hỗ trợ dạng không bổ sung
                    var orderAction = context.CustomerOrder.FirstOrDefault(x => x.OrderType == 1
                                    && x.OrderProcessId == parameter.OrderProcessId && x.IsOrderAction == true);

                    var orderProcessDetail = listOrderProcessDetail.FirstOrDefault(x => x.Id == parameter.OrderProcessDetailId);
                    orderProcessDetail.Status = statusDoneOrderProcessDetail;
                   
                    var eventCode = "";
                    //Nếu là bước hoàn thành thì cập nhật phiếu hỗ trợ sang trạng thái hoàn thành (3)
                    var statusCode = context.Category.FirstOrDefault(x => x.CategoryId == orderProcessDetail.CategoryId).CategoryCode;
                    if (statusCode == ProductConsts.CategoryCodeDoneStep)
                    {
                        if (orderAction != null)
                        {
                            orderAction.StatusOrder = 3;
                            context.CustomerOrder.Update(orderAction);
                            mess = "Xác nhận hoàn thành dịch vụ!";

                        }
                        eventCode = "DoneSer";
                    }
                    else
                    {
                        eventCode = "K";
                        mess = "Hoàn thành bước thực hiện trong quy trình!";
                    }
                    //Lấy list thông báo
                    var notificationCateTypeId = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == ProductConsts.CategoryTypeCodeNotificationConfig).CategoryTypeId;
                    var listNotifi = context.Category.Where(x => x.CategoryTypeId == notificationCateTypeId).ToList();

                    var listNotificationConfiguration = context.NotificationConfiguration.Where(x => x.ServicePacketId == orderProcess.ServicePacketId).ToList();
                    var listNotificationConfigurationId = listNotificationConfiguration.Select(x => x.Id).ToList();
                    var listEmpNoti = context.EmployeeMappingNotificationConfiguration.Where(x => listNotificationConfigurationId.Contains(x.NotificationConfigurationId.Value)).ToList();

                    //Lấy ra người cần nhận thông báo ở sự kiện ấn nút thay đổi trạng thái các điểm báo cáo
                    var datDichVuId = listNotifi.FirstOrDefault(x => x.CategoryCode == eventCode).CategoryId;
                    var listEmpId = new List<Guid>();
                    var notiConfigure = listNotificationConfiguration.FirstOrDefault(x => x.CategoryId == datDichVuId);
                    listEmp = listEmpNoti.Where(x => x.NotificationConfigurationId == notiConfigure.Id).Select(x => x.EmployeeId.Value).ToList();
                    var notificationReceiver = CommonHelper.GetListEmployeeIdNotifi(context, orderAction, notiConfigure, null, null);
                    var getListEmpId = notificationReceiver.ListEmployeeId;
                    var deviceId = context.User.Where(x => x.EmployeeId == notificationReceiver.CustomerId)?.FirstOrDefault()?.DeviceId;
                    listEmp.AddRange(getListEmpId);
                    if (deviceId != null)
                    {
                        var body = "Phiếu " + orderAction.OrderCode + ": " + mess;
                        var title = "Phiếu " + orderAction.OrderCode;
                        var type = 2; //1 : order, 2: orderAction

                        CommonHelper.PushNotificationToDevice(deviceId, title, body, type.ToString(), orderAction.OrderId.ToString());
                    }

                }
                //Cập nhật trạng thái của quy trình (OrderProcess): Nếu tất cả các bước là hoàn thành (3) => trạng thái quy trình chuyển thành hoàn thành
                if (listOrderProcessDetail.Where(x => x.Status == 3).Count() == listOrderProcessDetail.Count())
                {
                    orderProcess.Status = statusDoneOrderProcess;
                    orderProcess.UpdatedById = parameter.UserId;
                    orderProcess.UpdatedDate = DateTime.Now;
                    context.OrderProcess.Update(orderProcess);
                }

                context.SaveChanges();
                return new UpdateOrderProcessResult()
                {
                    MessageCode = mess,
                    StatusCode = System.Net.HttpStatusCode.OK,
                    ListEmpId = listEmp.Distinct().ToList(),
                };
            }
            catch (Exception e)
            {
                return new UpdateOrderProcessResult()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message,
                };
            }
        }
        public GetListOrderOfCusResult GetListOrderOfCus(GetListOrderOfCusParameter parameter)
        {
            try
            {
                var listOrderStatus = GeneralList.GetTrangThais("CustomerOrder").ToList();
                var listCustomerOrderStatus = GeneralList.GetTrangThais("CustomerOrderAction").ToList();
                var listOrderProcess = context.OrderProcess.Where(x => x.CustomerId == parameter.CustomerId).ToList();

                var listOrderProcessId = listOrderProcess.Select(x => x.Id).ToList();

                var listAllOrderOfCus = context.CustomerOrder.Where(x => listOrderProcessId.Contains(x.OrderProcessId.Value)).Select(x => new CustomerOrderEntityModel { 
                    OrderId = x.OrderId,
                    OrderType = x.OrderType,
                    IsOrderAction = x.IsOrderAction,
                    OrderCode = x.OrderCode,
                    ServicePacketId = x.ServicePacketId,
                    StatusOrder = x.StatusOrder,
                    CreatedDate = x.CreatedDate,
                    OrderProcesId = x.OrderProcessId
                }).ToList();


                //List phiếu yêu cầu
                var listOrder = listAllOrderOfCus.Where(x => x.OrderType == 1 && x.IsOrderAction == false).ToList();


                var listServicePacketId = listOrder.Select(x => x.ServicePacketId).Distinct().ToList();
                var listPack = context.ServicePacket.Where(x => listServicePacketId.Contains(x.Id)).ToList();

                var listOrderId = listOrder.Select(x => x.OrderId).ToList();
                var listOrderDetail = context.CustomerOrderDetail.Where(x => listOrderId.Contains(x.OrderId)).Select(x => new CustomerOrderDetailEntityModel { 
                    OrderDetailId = x.OrderDetailId,
                    OrderId = x.OrderId,
                    PriceInitial = x.PriceInitial,
                    OptionId = x.OptionId,
                    Quantity = x.Quantity
                }).ToList();
                var listServiceTreeId = listOrderDetail.Select(x => x.OptionId).ToList();
                var listServiceTree = context.ServicePacketMappingOptions.Where(x => listServiceTreeId.Contains(x.Id)).ToList();
                var listOptionId = listServiceTree.Select(x => x.OptionId).ToList();
                var listOption = context.Options.Where(x => listOptionId.Contains(x.Id)).Distinct().ToList();

                var listAllCustomerOrderDetailExten = context.CustomerOrderDetailExten.ToList();

                listOrder.ForEach(item =>
                {
                    var orderProcess = listOrderProcess.FirstOrDefault(x => x.Id == item.OrderProcesId);
                    if(orderProcess != null)
                    {
                        item.RateStar = orderProcess.RateStar;
                        item.OrderProcessId = orderProcess.Id;
                        item.RateConent = orderProcess.RateContent;
                    }

                    item.ListPacketServiceName = listPack.FirstOrDefault(x => x.Id == item.ServicePacketId)?.Name;
                    item.OrderStatusName = listOrderStatus.FirstOrDefault(x => x.Value == item.StatusOrder).Name;

                    //Tìm phiếu hỗ trợ
                    var orderAction = listAllOrderOfCus.FirstOrDefault(x => x.ObjectId == item.OrderId && x.IsOrderAction == true);
                    if(orderAction != null)
                    {
                        item.OrderActionId = orderAction.OrderId;
                        item.OrderActionCode = orderAction.OrderCode;
                        item.StatusOrderActionName = listCustomerOrderStatus.FirstOrDefault(x => x.Value == orderAction.StatusOrder).Name;
                    }

                    //Lấy các dịch vụ có trong phiếu
                    var listOrderDetail_Current = listOrderDetail.Where(x => x.OrderId == item.OrderId).ToList();
                    listOrderDetail_Current.ForEach(obj =>
                    {
                        var serviceTree_Current = listServiceTree.FirstOrDefault(x => x.Id == obj.OptionId);
                        if(serviceTree_Current != null)
                        {
                            var option = listOption.FirstOrDefault(x => x.Id == serviceTree_Current.OptionId);
                            obj.OptionName = option?.Name;
                            obj.Vat = option?.Vat;
                        }
                    });
                    item.ListCustomerOrderDetail = listOrderDetail_Current;

                    //Lấy các phiếu phát sinh của phiếu
                    item.ListCustomerOrderDetailExten = listAllCustomerOrderDetailExten.Where(x => x.OrderId == item.OrderId).Select(x => new CustomerOrderDetailExtenEntityModel
                    {
                        Id = x.Id,
                        Price = x.Price,
                        Quantity = x.Quantity,
                        Name = x.Name,
                        Status = x.Status,
                        StatusName = x.Status == 1 ? "Từ chối" : "Đã phê duyệt"
                    }).ToList();

                    //Lấy các phiếu yêu cầu bổ sung dịch vụ
                    //OrderType == 2 : Phiểu yêu cầu bổ sung, OrderType == 1 : Phiểu yêu cầu, 
                    var listOrderExten = listAllOrderOfCus.Where(x => x.OrderType == 2 && x.IsOrderAction == false).ToList();
                    listOrderExten.ForEach(obj =>
                    {
                        obj.OrderStatusName = listOrderStatus.FirstOrDefault(x => x.Value == obj.StatusOrder).Name;

                        //Tìm phiếu hỗ trợ
                        var orderActionExtend = listAllOrderOfCus.FirstOrDefault(x => x.ObjectId == obj.OrderId && x.IsOrderAction == true);
                        if (orderActionExtend != null)
                        {
                            obj.OrderActionId = orderActionExtend.OrderId;
                            obj.OrderActionCode = orderActionExtend.OrderCode;
                            obj.StatusOrderActionName = listCustomerOrderStatus.FirstOrDefault(x => x.Value == orderActionExtend.StatusOrder).Name;
                        }
                    });
                    item.ListOrderExtend = listOrderExten;
                });

                context.SaveChanges();
                return new GetListOrderOfCusResult()
                {
                    ListOrder = listOrder,
                    MessageCode = "",
                    StatusCode = System.Net.HttpStatusCode.OK,
                };
            }
            catch (Exception e)
            {
                return new GetListOrderOfCusResult()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message,
                };
            }
        }
        public RatingOrderResult RatingOrder(RatingOrderParameter parameter)
        {
            try
            {
                var order = context.CustomerOrder.FirstOrDefault(x => x.OrderId == parameter.OrderId);
                if(order == null)
                {
                    return new RatingOrderResult()
                    {
                        StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                        MessageCode = "Không tìm thấy phiếu trên hệ thống!",
                    };
                }
                var orderProcess = context.OrderProcess.FirstOrDefault(x => x.Id == order.OrderProcessId);
                if (orderProcess == null)
                {
                    return new RatingOrderResult()
                    {
                        StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                        MessageCode = "Không tìm thấy phiếu trên hệ thống!",
                    };
                }
                orderProcess.RateStar = parameter.RateStar;
                orderProcess.RateContent = parameter.RateContent;
                context.OrderProcess.Update(orderProcess);
                context.SaveChanges();
                return new RatingOrderResult()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = "Đánh giá thành công!",
                };
            }
            catch (Exception e)
            {
                return new RatingOrderResult()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message,
                };
            }
        }
        public DeleteOrderOptionByCusResult DeleteOrderOptionByCus(DeleteOrderOptionByCusParameter parameter)
        {
            try
            {
                //Nếu là dịch vụ phát sinh
                if(parameter.IsExtend == true)
                {
                    var optionExtend = context.CustomerOrderDetailExten.FirstOrDefault(x => x.Id == parameter.Id);
                    if (optionExtend == null)
                    {
                        return new DeleteOrderOptionByCusResult()
                        {
                            StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                            MessageCode = "Không tìm thấy dịch vụ trên hệ thống!",
                        };
                    }
                    context.CustomerOrderDetailExten.Remove(optionExtend);
                }
                else
                {
                    var orderDetail = context.CustomerOrderDetail.FirstOrDefault(x => x.OrderDetailId == parameter.Id);
                    var listAttrOptionAndPacket = context.CustomerOrderExtension
                        .Where(x => x.ObjectId == orderDetail.OrderDetailId && x.ObjectType == "1")
                        .ToList();
                    context.CustomerOrderExtension.RemoveRange(listAttrOptionAndPacket);

                    if (orderDetail == null)
                    {
                        return new DeleteOrderOptionByCusResult()
                        {
                            StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                            MessageCode = "Không tìm thấy dịch vụ trên hệ thống!",
                        };
                    }
                    context.CustomerOrderDetail.Remove(orderDetail);
                }
                context.SaveChanges();
                return new DeleteOrderOptionByCusResult()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = "Xóa thành công!",
                };
            }
            catch (Exception e)
            {
                return new DeleteOrderOptionByCusResult()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message,
                };
            }
        }

        public UpdateCustomerOrderDetailExtendResult UpdateCustomerOrderDetailExtend(UpdateCustomerOrderDetailExtendParameter parameter)
        {
            try
            {
                var customerOrder = context.CustomerOrder.FirstOrDefault(x => x.OrderId == parameter.OrderId);
                if (customerOrder == null)
                {
                    return new UpdateCustomerOrderDetailExtendResult
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = "Phiếu yêu cầu hỗ trợ không tồn tại trên hệ thống!"
                    };
                }

                customerOrder.UpdatedById = parameter.UserId;
                customerOrder.UpdatedDate = DateTime.Now;
                context.CustomerOrder.Update(customerOrder);

                //Lấy các detail cũ và xóa
                var listOrderDetailExtenOld = context.CustomerOrderDetailExten.Where(x => x.OrderId == parameter.OrderId).ToList();
                context.CustomerOrderDetailExten.RemoveRange(listOrderDetailExtenOld);

                //Lưu thông tin tùy chọn dịch vụ phát sinh
                var listOrderDetailExten = new List<CustomerOrderDetailExten>();
                var index = 1;
                parameter.ListOrderDetailExten.ForEach(item =>
                {
                    var newDetail = _mapper.Map<CustomerOrderDetailExten>(item);
                    newDetail.ServicePacketId = customerOrder.ServicePacketId;
                    newDetail.Stt = index;
                    index++;
                    listOrderDetailExten.Add(newDetail);
                });
                context.CustomerOrderDetailExten.AddRange(listOrderDetailExten);
                context.SaveChanges();

            }
            catch (Exception e)
            {
                return new UpdateCustomerOrderDetailExtendResult
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message,
                };
            }

            return new UpdateCustomerOrderDetailExtendResult
            {
                StatusCode = HttpStatusCode.OK,
            };
        }
        public DeleteOrderResult DeleteCustomerOrder(DeleteOrderParameter parameter)
        {
            try
            {
                var customerOrder = context.CustomerOrder.FirstOrDefault(x => x.OrderId == parameter.OrderId);
                if (customerOrder == null)
                {
                    return new DeleteOrderResult
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = "Phiếu yêu cầu không tồn tại trên hệ thống!"
                    };
                }

                //Lấy các detail và xóa
                var listDetail = context.CustomerOrderDetail.Where(x => x.OrderId == customerOrder.OrderId).ToList();
                var listDetailId = listDetail.Select(x => x.OrderDetailId).ToList();
                var listAttrOptionAndPacket = context.CustomerOrderExtension
                    .Where(x => listDetailId.Contains(x.OrderDetailId.Value) || x.OrderDetailId == customerOrder.OrderId) // Thuộc tính của option || thuộc tính của packet trong phiếu
                    .ToList();
                var listOrderDetailExtenOld = context.CustomerOrderDetailExten.Where(x => x.OrderId == parameter.OrderId).ToList();
                context.CustomerOrderDetail.RemoveRange(listDetail);
                context.CustomerOrderExtension.RemoveRange(listAttrOptionAndPacket);
                context.CustomerOrderDetailExten.RemoveRange(listOrderDetailExtenOld);

                var orderProcess = context.OrderProcess.FirstOrDefault(x => x.Id == customerOrder.OrderProcessId);
                var listOrderProcess = context.OrderProcessDetail.Where(x => x.OrderProcessId == orderProcess.Id).ToList();
                context.OrderProcess.RemoveRange(orderProcess);
                context.OrderProcessDetail.RemoveRange(listOrderProcess);
                context.CustomerOrder.Remove(customerOrder);
                context.SaveChanges();
                return new DeleteOrderResult
                {
                    StatusCode = HttpStatusCode.OK,
                };
            }
            catch (Exception e)
            {
                return new DeleteOrderResult
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message,
                };
            }

            
        }
        public CheckTaskWithReportPointExtendResult CheckTaskWithReportPointExtend(CheckTaskWithReportPointExtendParameter parameter)
        {
            try
            {
                var customerOrderTask = context.CustomerOrderTask.FirstOrDefault(x => x.Id == parameter.CustomerOrderTaskId);
                if (customerOrderTask == null)
                {
                    return new CheckTaskWithReportPointExtendResult
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = "Cấu hình dịch vụ không tồn tại trên hệ thống!"
                    };
                }

                //Lấy các reportPoint có khác mới có liên quan tới orderTask
                var listReportPoint = context.ReportPoint.Where(x => x.ServicePacketMappingOptionsId == customerOrderTask.ServicePacketMappingOptionsId && x.Status != 1
                                                        && x.OrderActionId == customerOrderTask.OrderActionId).ToList();

                //Lấy listEmp đang thực hiện điểm báo cáo
                var listEmpId = listReportPoint.Select(x => x.EmpId).ToList();

                //So sánh vs listEmp cập nhật mới
                //Nếu trong list Mới k có listEmpId đang thực hiện báo cáo => trả về lỗi
                var check = false;
                listEmpId.ForEach(item => {
                    if (!parameter.ListEmpId.Contains(item))
                    {
                        check = true;
                    }
                });

                if(check == true)
                {
                    return new CheckTaskWithReportPointExtendResult
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = "Danh sách nhân viên mới không khớp với điểm báo cáo tương ứng",
                    };
                }

                return new CheckTaskWithReportPointExtendResult
                {
                    StatusCode = HttpStatusCode.OK,
                };
            }
            catch (Exception e)
            {
                return new CheckTaskWithReportPointExtendResult
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message,
                };
            }

            
        }
    }





    public class ListCategoryId
    {
        public Guid ParentProductCategoryId { get; set; }
        public string ParentProductCategoryName { get; set; }
        public List<Guid> ListChildrent { get; set; }
    }

    public class ListCategoryResult
    {
        public Guid ParentProductCategoryId { get; set; }
        public string ParentProductCategoryName { get; set; }
        public decimal Total { get; set; }
    }

    public class EmployeeModel
    {
        public Guid EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string OrganizationName { get; set; }
        public decimal Amount { get; set; }
        public bool? DiscountType { get; set; }
        public decimal? DiscountValue { get; set; }
        public Guid UserId { get; set; }
    }

    public class StatusOrderModel
    {
        public Guid StatusId { get; set; }
        public string StatusName { get; set; }
        public decimal Amount { get; set; }
        public bool? DiscountType { get; set; }
        public decimal? DiscountValue { get; set; }
    }

    public class MonthOrderModel
    {
        public int STT { get; set; }
        public string MonthName { get; set; }
        public decimal Amount { get; set; }
        public bool? DiscountType { get; set; }
        public decimal? DiscountValue { get; set; }
        public DateTime TimeNode { get; set; }
    }

    public class ProductCategoryModel
    {
        public Guid? ProductCategoryId { get; set; }
        public string ProductCategoryName { get; set; }
        public decimal Total { get; set; }
    }

    public class DateOfQuarter
    {
        public DateTime FirstDateOfQuarter { get; set; }
        public DateTime LastDateOfQuarter { get; set; }
    }

    public class SaleRevenueByProductModel
    {
        public decimal? SaleRevenueByProduct { get; set; } //Doanh thu
        public decimal? AmountDiscountProduct { get; set; } //Chiết khấu
        public decimal? AmountPriceInitial { get; set; } //tổng giá vốn

        public SaleRevenueByProductModel()
        {
            SaleRevenueByProduct = 0;
            AmountDiscountProduct = 0;
            AmountPriceInitial = 0;
        }
    }

    public class SaleRevenueByCustomerModel
    {
        public decimal? SaleRevenue { get; set; } //Doanh thu
        public decimal? TotalPriceInitial { get; set; } //Tiền vốn
        public decimal? TotalGrossProfit { get; set; } //Lãi gộp
        public decimal? TotalProfitPerSaleRevenue { get; set; } // % (lãi/doanh thu)
        public decimal? TotalProfitPerPriceInitial { get; set; } // % (lãi/giá vốn)

        public SaleRevenueByCustomerModel()
        {
            SaleRevenue = 0;
            TotalPriceInitial = 0;
            TotalGrossProfit = 0;
            TotalProfitPerSaleRevenue = 0;
            TotalProfitPerPriceInitial = 0;
        }
    }


    public class TonKhoTheoSanPham
    {
        public Guid ProductId { get; set; }
        public Guid WarehouseId { get; set; }
        public string WarehouseName { get; set; }
        public decimal? TonKho { get; set; }
    }

    public class KhoChaCon
    {
        public Guid ParentId { get; set; }
        public List<Guid> ListChildId { get; set; }
    }
}
