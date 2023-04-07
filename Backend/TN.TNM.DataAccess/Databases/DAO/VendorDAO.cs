using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using TN.TNM.Common;
using TN.TNM.Common.NotificationSetting;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Helper;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.Vendor;
using TN.TNM.DataAccess.Messages.Results.Vendor;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Employee;
using TN.TNM.DataAccess.Models.Cost;
using TN.TNM.DataAccess.Models.Folder;
using TN.TNM.DataAccess.Models.ProcurementRequest;
using TN.TNM.DataAccess.Models.Product;
using TN.TNM.DataAccess.Models.Quote;
using TN.TNM.DataAccess.Models.Receivable;
using TN.TNM.DataAccess.Models.Vendor;
using TN.TNM.DataAccess.Models.WareHouse;
using TN.TNM.DataAccess.Models.Note;
using Microsoft.AspNetCore.Http;
using System.Text.RegularExpressions;
using System.Net;
using TN.TNM.DataAccess.Models.BankAccount;
using TN.TNM.DataAccess.Models.PurchaseOrderStatus;
using System.Dynamic;

namespace TN.TNM.DataAccess.Databases.DAO
{
    public class VendorDAO : BaseDAO, IVendorDataAsccess
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        public IConfiguration Configuration { get; }
        public static string WEB_ENDPOINT;
        public static string PrimaryDomain;
        public static int PrimaryPort;
        public static string SecondayDomain;
        public static int SecondaryPort;
        public static string Email;
        public static string Password;
        public static string BannerUrl;
        public static string Ssl;
        public static string Company;
        public static string Domain;
        public void GetConfiguration()
        {
            PrimaryDomain = Configuration["PrimaryDomain"];
            PrimaryPort = int.Parse(Configuration["PrimaryPort"]);
            SecondayDomain = Configuration["SecondayDomain"];
            SecondaryPort = int.Parse(Configuration["SecondaryPort"]);
            Email = Configuration["Email"];
            Password = Configuration["Password"];
            Ssl = Configuration["Ssl"];
            Company = Configuration["Company"];
            BannerUrl = Configuration["BannerUrl"];
            WEB_ENDPOINT = Configuration["WEB_ENDPOINT"];

            var configEntity = context.SystemParameter.ToList();
            Domain = configEntity.FirstOrDefault(w => w.SystemKey == "Domain").SystemValueString;
        }

        public VendorDAO(Databases.TNTN8Context _content, IAuditTraceDataAccess _iAuditTrace, IHostingEnvironment hostingEnvironment, IConfiguration iconfiguration)
        {
            this.context = _content;
            this.iAuditTrace = _iAuditTrace;
            _hostingEnvironment = hostingEnvironment;
            this.Configuration = iconfiguration;
        }
        public CreateVendorResult CreateVendor(CreateVendorParameter parameter)
        {
            try
            {
                this.iAuditTrace.Trace(ActionName.ADD, ObjectName.VENDOR, "Create vendor", parameter.UserId);
                parameter.Vendor.VendorId = Guid.NewGuid();
                var paymentMethod = context.Category.FirstOrDefault(c => c.CategoryCode == "CASH").CategoryId;
                parameter.Vendor.PaymentId = paymentMethod;
                parameter.Vendor.CreatedDate = DateTime.Now;

                //kiểm tra vendorCode
                if (parameter.Vendor.VendorCode == null)
                {
                    return new CreateVendorResult
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = "Mã nhà cung cấp không được để trống"
                    };
                }
                else
                {
                    parameter.Vendor.VendorCode = parameter.Vendor.VendorCode.Trim();
                    if (parameter.Vendor.VendorCode == "")
                    {
                        return new CreateVendorResult
                        {
                            StatusCode = HttpStatusCode.ExpectationFailed,
                            MessageCode = "Mã nhà cung cấp không được để trống"
                        };
                    }
                    else
                    {
                        var dublicateVendor = context.Vendor.FirstOrDefault(x => x.VendorCode == parameter.Vendor.VendorCode);
                        if (dublicateVendor != null)
                        {
                            return new CreateVendorResult
                            {
                                StatusCode = HttpStatusCode.ExpectationFailed,
                                MessageCode = "Mã nhà cung cấp đã tồn tại trên hệ thống"
                            };
                        };
                    }
                }

                #region Trim data - Add by Dung
                parameter.Vendor.VendorName = parameter.Vendor.VendorName?.Trim();
                parameter.Vendor.VendorCode = parameter.Vendor.VendorCode?.Trim();
                parameter.Vendor.Active = true;
                parameter.Vendor.CreatedDate = DateTime.Now;
                parameter.Vendor.CreatedById = parameter.UserId;
                parameter.Vendor.UpdatedById = null;
                parameter.Vendor.UpdatedDate = null;

                parameter.Contact.ContactId = Guid.NewGuid();
                parameter.Contact.ObjectId = parameter.Vendor.VendorId;
                parameter.Contact.ObjectType = ObjectType.VENDOR;
                parameter.Contact.CreatedDate = DateTime.Now;
                parameter.Contact.Phone = parameter.Contact.Phone?.Trim();
                parameter.Contact.Email = parameter.Contact.Email?.Trim();
                parameter.Contact.Active = true;
                parameter.Contact.CreatedDate = DateTime.Now;
                parameter.Contact.CreatedById = parameter.UserId;
                parameter.Contact.UpdatedById = null;
                parameter.Contact.UpdatedDate = null;

                parameter.VendorContactList.ForEach(contact =>
                {
                    contact.ContactId = Guid.NewGuid();
                    contact.ObjectId = parameter.Vendor.VendorId;
                    contact.ObjectType = "VEN_CON";
                    contact.FirstName = contact.FirstName?.Trim();
                    contact.LastName = contact.LastName?.Trim();
                    contact.Phone = contact.Phone?.Trim();
                    contact.Email = contact.Email?.Trim();
                    contact.Role = contact.Role?.Trim();
                    contact.Active = true;
                    contact.CreatedDate = DateTime.Now;
                    contact.CreatedById = parameter.UserId;
                    contact.UpdatedById = null;
                    contact.UpdatedDate = null;

                    context.Contact.Add(contact.ToEntity());
                });
                #endregion
                context.Vendor.Add(parameter.Vendor.ToEntity());
                context.Contact.Add(parameter.Contact.ToEntity());
                context.SaveChanges();
                return new CreateVendorResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = CommonMessage.Vendor.CREATE_SUCCESS,
                    ContactId = parameter.Contact.ContactId,
                    VendorId = parameter.Vendor.VendorId
                };
            }
            catch (Exception ex)
            {
                return new CreateVendorResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = ex.Message
                };
            }

        }

        public SearchVendorResult SearchVendor(SearchVendorParameter parameter)
        {
            this.iAuditTrace.Trace(ActionName.SEARCH, ObjectName.VENDOR, "Search vendor", parameter.UserId);
            var commonVendor = context.Vendor.Where(v => v.Active == true).ToList();
            var commonContact = context.Contact.Where(w => w.Active == true && w.ObjectType == ObjectType.VENDOR).ToList();
            var commonCategory = context.Category.ToList();

            #region Kiểm tra điều kiện xóa: Add by Dung
            //điều kiện xóa:
            //1. không có sản phẩm
            //2. không có phiếu chi
            //3. không có báo có
            //4. không có UNC
            //5. không có phiếu thu
            var listVendorId = commonVendor.Select(w => w.VendorId).ToList();
            var commonProductVendorMapping = context.ProductVendorMapping.Where(w => listVendorId.Contains(w.VendorId)).ToList(); //product
            var commonReceiptInvoiceMapping = context.ReceiptInvoiceMapping.Where(w => listVendorId.Contains(w.ObjectId.Value)).ToList(); //phieu thu
            var commonPayableInvoiceMapping = context.PayableInvoiceMapping.Where(w => listVendorId.Contains(w.ObjectId.Value)).ToList(); //phieu chi
            var commonBankReceiptInvoiceMapping = context.BankReceiptInvoiceMapping.Where(w => listVendorId.Contains(w.ObjectId.Value)).ToList(); //bao co
            var commonBankPayableInvoiceMapping = context.BankPayableInvoiceMapping.Where(w => listVendorId.Contains(w.ObjectId.Value)).ToList(); //UNC
            #endregion

            var vendorName = parameter.VendorName?.Trim()?.ToLower() ?? "";
            var vendorCode = parameter.VendorCode?.Trim()?.ToLower() ?? "";

            #region Comment by Dung
            //var vendorList = commonVendor.Where(v => (v.VendorName == parameter.VendorName.Trim() || v.VendorName.Contains(parameter.VendorName.Trim()) || parameter.VendorName.Trim() == "" || parameter.VendorName.Trim() == null) &&
            //                                            (v.VendorCode == parameter.VendorCode.Trim() || v.VendorCode.Contains(parameter.VendorCode.Trim()) || parameter.VendorCode.Trim() == "" || parameter.VendorCode.Trim() == null) &&
            //                                            (parameter.VendorGroupIdList.Contains(v.VendorGroupId) || parameter.VendorGroupIdList.Count == 0))
            //                .Select(v => new VendorEntityModel
            //                {
            //                    VendorId = v.VendorId,
            //                    ContactId = commonContact.FirstOrDefault(c => c.ObjectId == v.VendorId && c.ObjectType == ObjectType.VENDOR).ContactId,
            //                    VendorName = v.VendorName,
            //                    VendorGroupId = v.VendorGroupId,
            //                    VendorGroupName = commonCategory.FirstOrDefault(c => c.CategoryId == v.VendorGroupId).CategoryName,
            //                    VendorCode = v.VendorCode,
            //                    TotalPurchaseValue = v.TotalPurchaseValue,
            //                    TotalPayableValue = v.TotalPayableValue,
            //                    NearestDateTransaction = v.NearestDateTransaction,
            //                    PaymentId = v.PaymentId,
            //                    CreatedById = v.CreatedById,
            //                    CreatedDate = v.CreatedDate,
            //                    UpdatedById = v.UpdatedById,
            //                    UpdatedDate = v.UpdatedDate,
            //                    Active = v.Active,
            //                    CanDelete = CheckDeleteCondition(v.VendorId, commonQuoteDetail, commonCustomerOrderDetail, commonProcurementRequestItem, commonVendorOrderDetail),
            //                    // CountVendorInformation = CountVendorInformation(v.VendorId, commonProductVendorMapping, commonCustomerOrderDetail),
            //                }).OrderByDescending(v => v.CreatedDate).ToList();
            #endregion


            //master data 
            var listOrderStatus = context.OrderStatus.ToList();
            var listVendor = context.Vendor.ToList();
            var listVendorOrder = context.VendorOrder.ToList();
            var listPayableInvoice = context.PayableInvoice.ToList();
            var listPayableInvoiceMapping = context.PayableInvoiceMapping.ToList();
            var listBankPayableInvoice = context.BankPayableInvoice.ToList();
            var listBankPayableInvoiceMapping = context.BankPayableInvoiceMapping.ToList();
            var now = DateTime.Now;
            var firstDay = new DateTime(now.Year, now.Month, 1);

            var vendorList = commonVendor.Where(w => (w.VendorName.Contains(vendorName, StringComparison.OrdinalIgnoreCase) || (string.IsNullOrWhiteSpace(w.VendorName)))
                                                   && (w.VendorCode.Contains(vendorCode, StringComparison.OrdinalIgnoreCase) || (string.IsNullOrWhiteSpace(w.VendorCode)))
                                                   && (parameter.VendorGroupIdList.Contains(w.VendorGroupId) || parameter.VendorGroupIdList.Count == 0)
                                                  )
                            .Select(v => new VendorEntityModel
                            {
                                VendorId = v.VendorId,
                                ContactId = commonContact.FirstOrDefault(c => c.ObjectId == v.VendorId && c.ObjectType == ObjectType.VENDOR).ContactId,
                                VendorName = v.VendorName,
                                VendorGroupId = v.VendorGroupId,
                                VendorGroupName = commonCategory.FirstOrDefault(c => c.CategoryId == v.VendorGroupId).CategoryName,
                                VendorCode = v.VendorCode,
                                TotalPurchaseValue = v.TotalPurchaseValue,
                                //TotalPayableValue = v.TotalPayableValue,
                                TotalPayableValue = CalculateTotalReceivable(v.VendorId, listOrderStatus, listVendor, listVendorOrder, listPayableInvoice, listPayableInvoiceMapping, listBankPayableInvoice, listBankPayableInvoiceMapping, firstDay, now),
                                NearestDateTransaction = v.NearestDateTransaction,
                                PaymentId = v.PaymentId,
                                CreatedById = v.CreatedById,
                                CreatedDate = v.CreatedDate,
                                UpdatedById = v.UpdatedById,
                                UpdatedDate = v.UpdatedDate,
                                Address = v.Address,
                                Email = v.Email,
                                PhoneNumber = v.PhoneNumber,
                                Active = v.Active,

                                CanDelete = CheckDeleteCondition(v.VendorId, commonProductVendorMapping, commonReceiptInvoiceMapping, commonPayableInvoiceMapping, commonBankReceiptInvoiceMapping, commonBankPayableInvoiceMapping),
                            }).OrderByDescending(v => v.CreatedDate).ToList();

            return new SearchVendorResult()
            {
                StatusCode = HttpStatusCode.OK,
                VendorList = vendorList
            };
        }

        public bool CheckDeleteCondition(Guid vendorId,
                                            List<ProductVendorMapping> commonProductVendorMapping,
                                            List<ReceiptInvoiceMapping> commonReceiptInvoiceMapping,
                                            List<PayableInvoiceMapping> commonPayableInvoiceMapping,
                                            List<BankReceiptInvoiceMapping> commonBankReceiptInvoiceMapping,
                                            List<BankPayableInvoiceMapping> commonBankPayableInvoiceMapping)
        {
            var hasProduct = commonProductVendorMapping.FirstOrDefault(f => f.VendorId == vendorId);
            if (hasProduct != null)
            {
                return false;
            }

            var hasReceipt = commonReceiptInvoiceMapping.FirstOrDefault(f => f.ObjectId == vendorId);
            if (hasReceipt != null)
            {
                return false;
            }

            var hasPayable = commonPayableInvoiceMapping.FirstOrDefault(f => f.ObjectId == vendorId);
            if (hasPayable != null)
            {
                return false;
            }

            var hasBankReceipt = commonBankReceiptInvoiceMapping.FirstOrDefault(f => f.ObjectId == vendorId);
            if (hasBankReceipt != null)
            {
                return false;
            }

            var hasBankPayable = commonBankPayableInvoiceMapping.FirstOrDefault(f => f.ObjectId == vendorId);
            if (hasBankPayable != null)
            {
                return false;
            }

            return true;
        }

        public GetVendorByIdResult GetVendorById(GetVendorByIdParameter parameter)
        {
            this.iAuditTrace.Trace(ActionName.GETBYID, ObjectName.VENDOR, "Get vendor by Id", parameter.UserId);
            var vendor = context.Vendor.FirstOrDefault(v => v.VendorId == parameter.VendorId);
            var contact = parameter.ContactId == Guid.Empty ? context.Contact.FirstOrDefault(c => c.ObjectId == parameter.VendorId && c.ObjectType == ObjectType.VENDOR)
                : context.Contact.FirstOrDefault(c => c.ContactId == parameter.ContactId && c.ObjectType == ObjectType.VENDOR);

            var vendorBank = new List<BankAccountEntityModel>();
            var _vendorBank = context.BankAccount.Where(vb => vb.ObjectId == vendor.VendorId).OrderBy(vb => vb.BankName).ToList();
            _vendorBank.ForEach(item =>
            {
                var _item = new BankAccountEntityModel(item);
                vendorBank.Add(_item);
            });

            var vendorContact = new List<ContactEntityModel>();
            var _vendorContact = context.Contact.Where(c => c.ObjectId == parameter.VendorId && c.ObjectType == ObjectType.VENDORCONTACT).ToList();
            _vendorContact.ForEach(item =>
            {
                var _item = new ContactEntityModel(item);
                vendorContact.Add(_item);
            });

            var commonProductVendorMapping = context.ProductVendorMapping.ToList();
            var commonCustomerOrderDetail = context.CustomerOrderDetail.ToList();
            var province = new Entities.Province();
            var district = new Entities.District();
            var ward = new Entities.Ward();
            if (contact != null)
            {
                province = context.Province.FirstOrDefault(p => p.ProvinceId == contact.ProvinceId);
                district = context.District.FirstOrDefault(d => d.DistrictId == contact.DistrictId);
                ward = context.Ward.FirstOrDefault(w => w.WardId == contact.WardId);
            }

            VendorEntityModel vm = new VendorEntityModel()
            {
                VendorId = vendor.VendorId,
                VendorName = vendor.VendorName,
                Active = vendor.Active,
                CreatedById = vendor.CreatedById,
                CreatedDate = vendor.CreatedDate,
                PaymentId = vendor.PaymentId,
                TotalPayableValue = vendor.TotalPayableValue == null ? 0 : vendor.TotalPayableValue,
                TotalPurchaseValue = vendor.TotalPurchaseValue == null ? 0 : vendor.TotalPurchaseValue,
                NearestDateTransaction = vendor.NearestDateTransaction,
                UpdatedById = vendor.UpdatedById,
                UpdatedDate = vendor.UpdatedDate,
                VendorCode = vendor.VendorCode,
                VendorGroupId = vendor.VendorGroupId,
                Price = vendor.Price,
                VendorGroupName = context.Category.FirstOrDefault(ct => ct.CategoryId == vendor.VendorGroupId).CategoryName,
                PaymentName = context.Category.FirstOrDefault(ct => ct.CategoryId == vendor.PaymentId).CategoryName,
            };
            ContactEntityModel cm = new ContactEntityModel();
            if (contact != null)
            {
                cm = new ContactEntityModel()
                {
                    Active = contact.Active,
                    ContactId = contact.ContactId,
                    FirstName = contact.FirstName,
                    CreatedById = contact.CreatedById,
                    CreatedDate = contact.CreatedDate,
                    UpdatedById = contact.UpdatedById,
                    UpdatedDate = contact.UpdatedDate,
                    Address = contact.Address,
                    Email = contact.Email,
                    ObjectId = contact.ObjectId,
                    WebsiteUrl = contact.WebsiteUrl,
                    ProvinceId = contact.ProvinceId,
                    DistrictId = contact.DistrictId,
                    WardId = contact.WardId,
                    ObjectType = contact.ObjectType,
                    Phone = contact.Phone,
                    SocialUrl = contact.SocialUrl
                };
            }
            var countVendor = CountVendorInformation(parameter.VendorId, commonProductVendorMapping, commonCustomerOrderDetail);

            #region Lấy thông tin giao dịch thanh toán bao gồm Ủy nhiệm chi và Phiếu chi
            var exchangeStatusType = context.CategoryType.FirstOrDefault(f => f.CategoryTypeCode == "TCH").CategoryTypeId;
            var exchangeStatusList = context.Category.Where(w => w.CategoryTypeId == exchangeStatusType).ToList();

            var listExchange = new List<Models.Vendor.ExchangeByVendorEntityModel>();

            //dnah sách ủy nhiệm chi
            var listBankPayableInvoiceId = context.BankPayableInvoiceMapping.Where(w => w.ObjectId == parameter.VendorId).Select(w => w.BankPayableInvoiceId).ToList() ?? new List<Guid>();
            var listBankPayableEntity = context.BankPayableInvoice.Where(w => listBankPayableInvoiceId.Contains(w.BankPayableInvoiceId)).ToList() ?? new List<BankPayableInvoice>();

            listBankPayableEntity?.ForEach(payable =>
            {
                var unitPrice = payable.BankPayableInvoicePrice ?? 1;
                var exchangeRate = payable.BankPayableInvoiceExchangeRate ?? 1;
                var exchangeValue = unitPrice * exchangeRate;

                var exchange = new ExchangeByVendorEntityModel
                {
                    ExchangeType = "UNC",
                    ExchangeName = "Ủy nhiệm chi",
                    ExchangeId = payable.BankPayableInvoiceId,
                    ExchangeDate = payable.CreatedDate,
                    ExchangeCode = payable.BankPayableInvoiceCode,
                    StatusCode = exchangeStatusList.FirstOrDefault(f => f.CategoryId == payable.StatusId).CategoryCode,
                    StatusName = exchangeStatusList.FirstOrDefault(f => f.CategoryId == payable.StatusId).CategoryName,
                    ExchangeValue = exchangeValue,
                    ExchangeDetail = payable.BankPayableInvoiceDetail
                };

                listExchange.Add(exchange);
            });

            //danh sách phiếu chi
            var listPayableInvoiceId = context.PayableInvoiceMapping.Where(w => w.ObjectId == parameter.VendorId).Select(w => w.PayableInvoiceId).ToList() ?? new List<Guid>();
            var listPayableEntity = context.PayableInvoice.Where(w => listPayableInvoiceId.Contains(w.PayableInvoiceId)).ToList() ?? new List<PayableInvoice>();

            listPayableEntity?.ForEach(payable =>
            {
                var unitPrice = payable.PayableInvoicePrice ?? 1;
                var exchangeRate = payable.ExchangeRate ?? 1;
                var exchangeValue = unitPrice * exchangeRate;

                var exchange = new ExchangeByVendorEntityModel
                {
                    ExchangeType = "PC",
                    ExchangeName = "Phiếu chi",
                    ExchangeId = payable.PayableInvoiceId,
                    ExchangeDate = payable.CreatedDate,
                    ExchangeCode = payable.PayableInvoiceCode,
                    StatusCode = exchangeStatusList.FirstOrDefault(f => f.CategoryId == payable.StatusId).CategoryCode,
                    StatusName = exchangeStatusList.FirstOrDefault(f => f.CategoryId == payable.StatusId).CategoryName,
                    ExchangeValue = exchangeValue,
                    ExchangeDetail = payable.PayableInvoiceDetail
                };

                listExchange.Add(exchange);
            });

            //danh sách phiếu thu
            var listReceiptInvoice = context.ReceiptInvoiceMapping.Where(w => w.ObjectId == parameter.VendorId).Select(w => w.ReceiptInvoiceId).ToList() ?? new List<Guid>();
            var listReceiptEntity = context.ReceiptInvoice.Where(w => listReceiptInvoice.Contains(w.ReceiptInvoiceId)).ToList() ?? new List<ReceiptInvoice>();

            listReceiptEntity?.ForEach(e =>
            {
                var unitPrice = e.UnitPrice ?? 1;
                var exchangeRate = e.ExchangeRate ?? 1;
                var exchangeValue = unitPrice * exchangeRate;

                var exchange = new ExchangeByVendorEntityModel
                {
                    ExchangeType = "PT",
                    ExchangeName = "Phiếu thu",
                    ExchangeId = e.ReceiptInvoiceId,
                    ExchangeDate = e.CreatedDate,
                    ExchangeCode = e.ReceiptInvoiceCode,
                    StatusCode = exchangeStatusList.FirstOrDefault(f => f.CategoryId == e.StatusId).CategoryCode,
                    StatusName = exchangeStatusList.FirstOrDefault(f => f.CategoryId == e.StatusId).CategoryName,
                    ExchangeValue = exchangeValue,
                    ExchangeDetail = e.ReceiptInvoiceDetail
                };

                listExchange.Add(exchange);
            });

            //danh sách báo có
            var listBankReceiptInvoice = context.BankReceiptInvoiceMapping.Where(w => w.ObjectId == parameter.VendorId).Select(w => w.BankReceiptInvoiceId).ToList() ?? new List<Guid>();
            var listBankReceiptInvoiceEntity = context.BankReceiptInvoice.Where(w => listBankReceiptInvoice.Contains(w.BankReceiptInvoiceId)).ToList() ?? new List<BankReceiptInvoice>();

            listBankReceiptInvoiceEntity?.ForEach(e =>
            {
                var unitPrice = e.BankReceiptInvoicePrice ?? 1;
                var exchangeRate = e.BankReceiptInvoiceExchangeRate ?? 1;
                var exchangeValue = unitPrice * exchangeRate;

                var exchange = new ExchangeByVendorEntityModel
                {
                    ExchangeType = "BC",
                    ExchangeName = "Báo có",
                    ExchangeId = e.BankReceiptInvoiceId,
                    ExchangeDate = e.CreatedDate,
                    ExchangeCode = e.BankReceiptInvoiceCode,
                    StatusCode = exchangeStatusList.FirstOrDefault(f => f.CategoryId == e.StatusId).CategoryCode,
                    StatusName = exchangeStatusList.FirstOrDefault(f => f.CategoryId == e.StatusId).CategoryName,
                    ExchangeValue = exchangeValue,
                    ExchangeDetail = e.BankReceiptInvoiceDetail
                };

                listExchange.Add(exchange);
            });
            #endregion

            return new GetVendorByIdResult()
            {
                StatusCode = HttpStatusCode.OK,
                Vendor = vm,
                Contact = cm,
                VendorBankAccountList = vendorBank,
                VendorContactList = vendorContact,
                CountVendorInformation = countVendor,
                FullAddress = "",
                ListExchangeByVendor = listExchange
                //FullAddress = ward?.WardType + " " + ward?.WardName + ", " + district?.DistrictType + " " + district?.DistrictName + ", " + province?.ProvinceType + " " + province?.ProvinceName
            };
        }

        public GetAllVendorCodeResult GetAllVendorCode(GetAllVendorCodeParameter parameter)
        {
            var vendorCodeList = context.Vendor.Select(v => v.VendorCode.ToLower()).ToList();
            return new GetAllVendorCodeResult()
            {
                StatusCode = HttpStatusCode.OK,
                VendorCodeList = vendorCodeList
            };
        }

        public UpdateVendorByIdResult UpdateVendorById(UpdateVendorByIdParameter parameter)
        {
            try
            {
                Entities.Vendor vendor = context.Vendor.FirstOrDefault(v => v.VendorId == parameter.Vendor.VendorId);
                //vendor.VendorId = parameter.Vendor.VendorId;
                vendor.VendorName = parameter.Vendor?.VendorName?.Trim();
                vendor.VendorCode = parameter.Vendor?.VendorCode?.Trim();
                vendor.VendorGroupId = parameter.Vendor.VendorGroupId;
                vendor.PaymentId = parameter.Vendor.PaymentId;
                vendor.Price = parameter.Vendor.Price;
                vendor.UpdatedById = parameter.UserId;
                vendor.UpdatedDate = DateTime.Now;

                Entities.Contact contact = context.Contact.FirstOrDefault(c => c.ContactId == parameter.Contact.ContactId && c.ObjectType == ObjectType.VENDOR);
                //contact.ContactId = parameter.Contact.ContactId;
                //contact.ObjectId = parameter.Contact.ObjectId;
                //contact.ObjectType = parameter.Contact.ObjectType;
                contact.Email = parameter.Contact.Email == null ? "" : parameter.Contact.Email.Trim();
                contact.ProvinceId = parameter.Contact.ProvinceId;
                contact.DistrictId = parameter.Contact.DistrictId;
                contact.WardId = parameter.Contact.WardId;
                contact.Phone = parameter.Contact.Phone == null ? "" : parameter.Contact?.Phone?.Trim();
                contact.Address = parameter.Contact.Address == null ? "" : parameter.Contact.Address?.Trim();
                contact.WebsiteUrl = parameter.Contact.WebsiteUrl == null ? "" : parameter.Contact.WebsiteUrl?.Trim();
                contact.SocialUrl = parameter.Contact.SocialUrl == null ? "" : parameter.Contact.SocialUrl?.Trim();
                contact.UpdatedById = parameter.UserId;
                contact.UpdatedDate = DateTime.Now;

                context.Vendor.Update(vendor);
                context.Contact.Update(contact);
                context.SaveChanges();

                return new UpdateVendorByIdResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = CommonMessage.Vendor.EDIT_SUCCESS
                };
            }
            catch (Exception ex)
            {
                return new UpdateVendorByIdResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = ex.Message
                };
            }

        }

        public QuickCreateVendorResult QuickCreateVendor(QuickCreateVendorParameter parameter)
        {
            try
            {
                //kiểm tra vendorCode
                if (parameter.Vendor.VendorCode == null)
                {
                    return new QuickCreateVendorResult
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = "Mã nhà cung cấp không được để trống"
                    };
                }
                else
                {
                    parameter.Vendor.VendorCode = parameter.Vendor.VendorCode.Trim();
                    if (parameter.Vendor.VendorCode == "")
                    {
                        return new QuickCreateVendorResult
                        {
                            StatusCode = HttpStatusCode.ExpectationFailed,
                            MessageCode = "Mã nhà cung cấp không được để trống"
                        };
                    }
                    else
                    {
                        var dublicateVendor = context.Vendor.FirstOrDefault(x => x.VendorCode == parameter.Vendor.VendorCode);
                        if (dublicateVendor != null)
                        {
                            return new QuickCreateVendorResult
                            {
                                StatusCode = HttpStatusCode.ExpectationFailed,
                                MessageCode = "Mã nhà cung cấp đã tồn tại trên hệ thống"
                            };
                        }
                    }
                }
                parameter.Vendor.VendorId = Guid.NewGuid();

                Contact c = new Contact()
                {
                    ContactId = Guid.NewGuid(),
                    ObjectId = parameter.Vendor.VendorId,
                    ObjectType = ObjectType.VENDOR,
                    Phone = parameter.Phone,
                    Email = parameter.Email,
                    Address = parameter.Address,
                    CreatedById = parameter.UserId,
                    CreatedDate = DateTime.Now,
                    Active = true
                };
                var payCASH = context.Category.FirstOrDefault(ct => ct.CategoryCode == "CASH");
                parameter.Vendor.PaymentId = payCASH.CategoryId;
                parameter.Vendor.UpdatedById = null;
                parameter.Vendor.CreatedById = parameter.UserId;
                parameter.Vendor.CreatedDate = DateTime.Now;
                context.Vendor.Add(parameter.Vendor);
                context.Contact.Add(c);
                context.SaveChanges();

                return new QuickCreateVendorResult
                {
                    StatusCode = HttpStatusCode.OK,
                    VendorId = parameter.Vendor.VendorId,
                };
            }
            catch (Exception ex)
            {
                return new QuickCreateVendorResult
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = ex.ToString()
                };
            }
        }

        public CreateVendorOrderResult CreateVendorOrder(CreateVendorOrderParameter parameter)
        {
            bool isValidParameterNumber = !(parameter.VendorOrder?.DiscountValue < 0);

            foreach (var item in parameter.VendorOrderDetail)
            {
                if (item?.Quantity <= 0 || item?.UnitPrice < 0 || item?.Vat < 0 || item?.DiscountValue < 0)
                {
                    isValidParameterNumber = false;
                    break;
                }
            }

            if (!isValidParameterNumber)
            {
                return new CreateVendorOrderResult()
                {
                    Status = false,
                    Message = CommonMessage.Vendor.CREATE_ORDER_FAIL
                };
            }

            try
            {
                var statusOrderNew =
                    context.PurchaseOrderStatus.FirstOrDefault(o => o.Active == true && o.PurchaseOrderStatusCode == "DRA");

                var newVendorOrderId = Guid.NewGuid();
                parameter.VendorOrder.VendorOrderId = newVendorOrderId;
                parameter.VendorOrder.CreatedDate = DateTime.Now;
                parameter.VendorOrder.StatusId = statusOrderNew.PurchaseOrderStatusId;

                parameter.VendorOrderDetail?.ForEach(detail =>
                {
                    detail.VendorOrderId = newVendorOrderId;
                    detail.VendorOrderDetailId = Guid.NewGuid();
                    detail.ProcurementRequestId = detail.ProcurementRequestId;
                    detail.Cost = detail.Cost == null ? 0 : detail.Cost;
                    detail.PriceWarehouse = detail.PriceWarehouse == null ? 0 : detail.PriceWarehouse;
                    detail.PriceValueWarehouse = detail.PriceValueWarehouse == null ? 0 : detail.PriceValueWarehouse;
                    detail.CreatedDate = DateTime.Now;
                    detail.OrderDetailType = detail.OrderDetailType;
                });

                var listVendorOrderDetail = new List<VendorOrderDetail>();
                parameter.VendorOrderDetail.ForEach(item =>
                {
                    var newItem = new VendorOrderDetail();
                    newItem = item.ToEntity();

                    if (item.VendorOrderProductDetailProductAttributeValue != null &&
                        item.VendorOrderProductDetailProductAttributeValue.Count != 0)
                    {
                        item.VendorOrderProductDetailProductAttributeValue.ForEach(_item =>
                        {
                            var _newItem = _item.ToEntity();
                            newItem.VendorOrderProductDetailProductAttributeValue.Add(_newItem);
                        });
                    }

                    listVendorOrderDetail.Add(newItem);
                });

                context.VendorOrderDetail.AddRange(listVendorOrderDetail);

                var totalVendor = context.VendorOrder.Count();
                var listVendorOrderCode = context.VendorOrder.Select(w => w.VendorOrderCode).ToList();

                // gen mã mới - dungpt
                parameter.VendorOrder.VendorOrderCode = ReGenerateorderCode(listVendorOrderCode, totalVendor);

                #region Thêm vào bảng mapping giữa Phiếu đề xuất và Đơn hàng mua

                parameter.ListVendorOrderProcurementRequestMapping?.ForEach(item =>
                {
                    var vendorOrderProcurementRequestMapping = new VendorOrderProcurementRequestMapping();
                    vendorOrderProcurementRequestMapping.VendorOrderProcurementRequestMappingId = Guid.NewGuid();
                    vendorOrderProcurementRequestMapping.VendorOrderId = newVendorOrderId;
                    vendorOrderProcurementRequestMapping.ProcurementRequestId = item.ProcurementRequestId;
                    vendorOrderProcurementRequestMapping.Active = true;
                    vendorOrderProcurementRequestMapping.CreatedById = parameter.UserId;
                    vendorOrderProcurementRequestMapping.CreatedDate = DateTime.Now;

                    context.VendorOrderProcurementRequestMapping.Add(vendorOrderProcurementRequestMapping);
                });

                #endregion

                #region Thêm vào bảng lưu chi phí cho Đơn hàng mua

                parameter.ListVendorOrderCostDetail?.ForEach(item =>
                {
                    var vendorOrderCostDetail = new VendorOrderCostDetail();
                    vendorOrderCostDetail.VendorOrderCostDetailId = Guid.NewGuid();
                    vendorOrderCostDetail.CostId = item.CostId;
                    vendorOrderCostDetail.VendorOrderId = newVendorOrderId;
                    vendorOrderCostDetail.UnitPrice = item.UnitPrice;
                    vendorOrderCostDetail.CostName = item.CostName;
                    vendorOrderCostDetail.Active = true;
                    vendorOrderCostDetail.CreatedById = parameter.UserId;
                    vendorOrderCostDetail.CreatedDate = DateTime.Now;

                    context.VendorOrderCostDetail.Add(vendorOrderCostDetail);
                });

                #endregion

                //var vendorDupblicase = context.VendorOrder.FirstOrDefault(x => x.VendorOrderCode == parameter.VendorOrder.VendorOrderCode);
                //if (vendorDupblicase != null)
                //{
                //    return new CreateVendorOrderResult
                //    {
                //        Status = false,
                //        Message = "Mã đơn đặt hàng đã tồn tại"
                //    };
                //}

                //Lưu giá trị đơn hàng vào Tổng thanh toán
                var orderSttCode = context.PurchaseOrderStatus
                    .FirstOrDefault(ord => ord.PurchaseOrderStatusId == parameter.VendorOrder.StatusId)
                    ?.PurchaseOrderStatusCode;
                var listVendorOrderStatusSales = context.SystemParameter
                    .FirstOrDefault(x => x.SystemKey == "PurchaseOrderStatus")?.SystemValueString.Split(';').ToList();
                if (listVendorOrderStatusSales != null && listVendorOrderStatusSales.Contains(orderSttCode))
                {
                    var vendor = context.Vendor.FirstOrDefault(c => c.VendorId == parameter.VendorOrder.VendorId);

                    if (vendor != null)
                    {
                        vendor.TotalPurchaseValue = vendor.TotalPurchaseValue == null
                            ? parameter.VendorOrder.Amount
                            : vendor.TotalPurchaseValue + parameter.VendorOrder.Amount;
                        vendor.TotalPayableValue = vendor.TotalPayableValue == null
                            ? parameter.VendorOrder.Amount
                            : vendor.TotalPayableValue + parameter.VendorOrder.Amount;
                        vendor.NearestDateTransaction = parameter.VendorOrder.CreatedDate;
                        context.Vendor.Update(vendor);
                    }
                }

                context.VendorOrder.Add(parameter.VendorOrder.ToEntity());
                context.SaveChanges();

                #region Gửi mail thông báo

                NotificationHelper.AccessNotification(context, TypeModel.VendorOrder, "CRE", new VendorOrder(),
                    parameter.VendorOrder, true);

                #endregion

                #region Lưu nhật ký hệ thống

                LogHelper.AuditTrace(context, ActionName.Create, ObjectName.VENDORORDER, newVendorOrderId, parameter.UserId);

                #endregion

                return new CreateVendorOrderResult()
                {
                    VendorOrderId = newVendorOrderId,
                    StatusCode = HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new CreateVendorOrderResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = ex.Message
                };
            }
        }

        public SearchVendorOrderResult SearchVendorOrder(SearchVendorOrderParameter parameter)
        {
            try
            {
                var vendorOrderStatus = context.PurchaseOrderStatus.ToList();
                var vendor = context.Vendor.ToList();
                var listIdUser = parameter.CreateyByIds;
                var user = context.User.FirstOrDefault(x => x.UserId == parameter.UserId);
                var employee = context.Employee.FirstOrDefault(x => x.EmployeeId == user.EmployeeId);
                var listCommonVendorDetail = context.VendorOrderDetail.ToList();
                var listCommonProcurement = context.ProcurementRequest.ToList();

                var lst = new List<VendorOrderEntityModel>();

                var createdByIds = new List<Guid>();

                if (listIdUser != null)
                {
                    createdByIds = context.User.Where(x => listIdUser.Contains(x.EmployeeId.Value)).Select(y => y.UserId)
                        .ToList();
                }
                else
                {
                    createdByIds = listIdUser;
                }

                var listProcurementRequestId = new List<Guid>();
                if (parameter.ListProcurementRequestId != null)
                {
                    listProcurementRequestId = parameter.ListProcurementRequestId;
                }

                var listProductId = new List<Guid>();
                if (parameter.ListProductId != null)
                {
                    listProductId = parameter.ListProductId;
                }

                if (employee.IsManager)
                {
                    //Quản lí: lấy tất cả nhân viên phòng ban đó và phòng ban dưới cấp
                    var currentOrganization = employee.OrganizationId;
                    List<Guid?> listOrganizationChildrenId = new List<Guid?>();
                    listOrganizationChildrenId.Add(currentOrganization);
                    var organizationList = context.Organization.Where(w => w.Active == true).ToList();

                    getOrganizationChildrenId(organizationList, currentOrganization, listOrganizationChildrenId);

                    var listEmployeeId = context.Employee
                        .Where(w => listOrganizationChildrenId.Contains(w.OrganizationId)).Select(y => y.EmployeeId)
                        .ToList();

                    lst = context.VendorOrder.Where(vo =>
                            (parameter.VendorIdList.Contains(vo.VendorId) || parameter.VendorIdList == null ||
                             parameter.VendorIdList.Count == 0) &&
                            (vo.VendorOrderCode.Contains(parameter.VendorModelCode) || parameter.VendorModelCode == null ||
                             parameter.VendorModelCode == "") &&
                            (parameter.VendorOrderDateFrom <= vo.VendorOrderDate || parameter.VendorOrderDateFrom == null ||
                             parameter.VendorOrderDateFrom == DateTime.MinValue) &&
                            (parameter.VendorOrderDateTo >= vo.VendorOrderDate || parameter.VendorOrderDateTo == null ||
                             parameter.VendorOrderDateTo == DateTime.MinValue) &&
                            (createdByIds == null || createdByIds.Count == 0 || createdByIds.Contains(vo.CreatedById)) &&
                            (parameter.StatusIdList.Contains(vo.StatusId) || parameter.StatusIdList == null ||
                             parameter.StatusIdList.Count == 0) &&
                            listEmployeeId.Contains(vo.Orderer.Value))
                        .Select(vo => new VendorOrderEntityModel()
                        {
                            VendorId = vo.VendorId,
                            VendorOrderId = vo.VendorOrderId,
                            VendorOrderDate = vo.VendorOrderDate,
                            VendorOrderCode = vo.VendorOrderCode,
                            Orderer = vo.Orderer,
                            Description = vo.Description,
                            Note = vo.Note,
                            VendorContactId = vo.VendorContactId,
                            PaymentMethod = vo.PaymentMethod,
                            BankAccountId = vo.BankAccountId,
                            ReceivedDate = vo.ReceivedDate,
                            ReceivedHour = vo.ReceivedHour,
                            RecipientName = vo.RecipientName,
                            LocationOfShipment = vo.LocationOfShipment,
                            ShippingNote = vo.ShippingNote,
                            RecipientPhone = vo.RecipientPhone,
                            RecipientEmail = vo.RecipientEmail,
                            PlaceOfDelivery = vo.PlaceOfDelivery,
                            Amount = vo.Amount,
                            DiscountValue = vo.DiscountValue,
                            StatusId = vo.StatusId,
                            StatusCode = vendorOrderStatus.FirstOrDefault(f => f.PurchaseOrderStatusId == vo.StatusId)
                                .PurchaseOrderStatusCode,
                            DiscountType = vo.DiscountType,
                            CreatedById = vo.CreatedById,
                            CreatedDate = vo.CreatedDate,
                            UpdatedById = vo.UpdatedById,
                            UpdatedDate = vo.UpdatedDate,
                            Active = vo.Active,
                        }).OrderByDescending(vo => vo.CreatedDate).ToList();
                }
                else
                {
                    lst = context.VendorOrder.Where(vo =>
                            (parameter.VendorIdList.Contains(vo.VendorId) || parameter.VendorIdList == null ||
                             parameter.VendorIdList.Count == 0) &&
                            (vo.VendorOrderCode.Contains(parameter.VendorModelCode) || parameter.VendorModelCode == null ||
                             parameter.VendorModelCode == "") &&
                            (parameter.VendorOrderDateFrom <= vo.VendorOrderDate || parameter.VendorOrderDateFrom == null ||
                             parameter.VendorOrderDateFrom == DateTime.MinValue) &&
                            (parameter.VendorOrderDateTo >= vo.VendorOrderDate || parameter.VendorOrderDateTo == null ||
                             parameter.VendorOrderDateTo == DateTime.MinValue) &&
                            (createdByIds == null || createdByIds.Count == 0 || createdByIds.Contains(vo.CreatedById)) &&
                            (parameter.StatusIdList.Contains(vo.StatusId) || parameter.StatusIdList == null ||
                             parameter.StatusIdList.Count == 0) &&
                            vo.Orderer.Value == employee.EmployeeId)
                        .Select(vo => new VendorOrderEntityModel()
                        {
                            VendorId = vo.VendorId,
                            VendorOrderId = vo.VendorOrderId,
                            VendorOrderDate = vo.VendorOrderDate,
                            VendorOrderCode = vo.VendorOrderCode,
                            Orderer = vo.Orderer,
                            Description = vo.Description,
                            Note = vo.Note,
                            VendorContactId = vo.VendorContactId,
                            PaymentMethod = vo.PaymentMethod,
                            BankAccountId = vo.BankAccountId,
                            ReceivedDate = vo.ReceivedDate,
                            ReceivedHour = vo.ReceivedHour,
                            RecipientName = vo.RecipientName,
                            LocationOfShipment = vo.LocationOfShipment,
                            ShippingNote = vo.ShippingNote,
                            RecipientPhone = vo.RecipientPhone,
                            RecipientEmail = vo.RecipientEmail,
                            PlaceOfDelivery = vo.PlaceOfDelivery,
                            Amount = vo.Amount,
                            DiscountValue = vo.DiscountValue,
                            StatusId = vo.StatusId,
                            StatusCode = vendorOrderStatus.FirstOrDefault(f => f.PurchaseOrderStatusId == vo.StatusId)
                                .PurchaseOrderStatusCode,
                            DiscountType = vo.DiscountType,
                            CreatedById = vo.CreatedById,
                            CreatedDate = vo.CreatedDate,
                            UpdatedById = vo.UpdatedById,
                            UpdatedDate = vo.UpdatedDate,
                            Active = vo.Active,
                        }).OrderByDescending(vo => vo.CreatedDate).ToList();
                }

                var listVendorOrderId = lst.Select(y => y.VendorOrderId).ToList();
                var listVendorOrderDetail =
                    listCommonVendorDetail.Where(x => listVendorOrderId.Contains(x.VendorOrderId)).ToList();

                var listResultVendorOrderId = listVendorOrderDetail.Where(x =>
                        (listProcurementRequestId.Count == 0 ||
                         (x.ProcurementRequestId != null &&
                          listProcurementRequestId.Contains(x.ProcurementRequestId.Value))) &&
                        (listProductId.Count == 0 || (x.ProductId != null && listProductId.Contains(x.ProductId.Value))))
                    .Select(y => y.VendorOrderId).Distinct()
                    .ToList();

                lst = lst.Where(x => listResultVendorOrderId.Contains(x.VendorOrderId))
                    .ToList();

                lst.ForEach(item =>
                {
                    var empId = context.User.FirstOrDefault(u => u.UserId == item.CreatedById).EmployeeId;
                    var contact = context.Contact.FirstOrDefault(c =>
                        c.ObjectId == empId.Value && c.ObjectType == ObjectType.EMPLOYEE);
                    var orderer = context.Contact.FirstOrDefault(x =>
                        x.ObjectId == item.Orderer && x.ObjectType == ObjectType.EMPLOYEE);
                    item.CreatedByName = contact.FirstName + " " + contact.LastName;
                    if (orderer != null) item.OrdererName = orderer.FirstName + " " + orderer.LastName;
                    item.StatusName = vendorOrderStatus.FirstOrDefault(os => os.PurchaseOrderStatusId == item.StatusId)
                                          ?.Description ?? "";
                    item.Description = item.Description ?? "";
                    item.VendorName = vendor.FirstOrDefault(c => c.VendorId == item.VendorId)?.VendorName ?? "";
                    item.VendorCode = vendor.FirstOrDefault(c => c.VendorId == item.VendorId)?.VendorCode ?? "";

                    #region Lấy danh sách phiếu đề xuất của Đơn hàng mua

                    var _listCurrentProcurementRequestId = listCommonVendorDetail
                        .Where(x => x.VendorOrderId == item.VendorOrderId)
                        .Select(y => y.ProcurementRequestId).ToList();

                    item.ListProcurementName = "";
                    if (_listCurrentProcurementRequestId.Count > 0)
                    {
                        var listProcurementRequestCode = listCommonProcurement
                            .Where(x => _listCurrentProcurementRequestId.Contains(x.ProcurementRequestId))
                            .Select(y => y.ProcurementCode).ToList();

                        item.ListProcurementName = String.Join(", ", listProcurementRequestCode);
                    }

                    #endregion
                });

                lst.ForEach(item =>
                {
                    if (item.StatusId != null && item.StatusId != Guid.Empty)
                    {
                        var status = vendorOrderStatus.FirstOrDefault(c => c.PurchaseOrderStatusId == item.StatusId);
                        switch (status?.PurchaseOrderStatusCode)
                        {
                            case "PURC":
                                item.BackgroundColorForStatus = "#007aff";
                                break;
                            case "IP":
                                item.BackgroundColorForStatus = "#ffcc00";
                                break;
                            case "RTN":
                                item.BackgroundColorForStatus = "#272909";
                                break;
                            case "CAN":
                                item.BackgroundColorForStatus = "#BB0000";
                                break;
                            case "DRA":
                                item.BackgroundColorForStatus = "#C9CAC2";
                                break;
                            case "COMP":
                                item.BackgroundColorForStatus = "#6D98E7";
                                break;
                        }
                    }
                });

                return new SearchVendorOrderResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = lst.Count == 0 ? CommonMessage.Vendor.SEARCH_ORDER_EMPTY : "",
                    VendorOrderList = lst
                };
            }
            catch (Exception ex)
            {
                return new SearchVendorOrderResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = ex.Message
                };
            }

        }

        public GetAllVendorResult GetAllVendor(GetAllVendorParameter parameter)
        {
            try
            {
                var listVendor = new List<VendorEntityModel>();
                var _listVendor = context.Vendor.ToList();
                _listVendor.ForEach(item =>
                {
                    var _item = new VendorEntityModel(item);
                    listVendor.Add(_item);
                });
                return new GetAllVendorResult
                {
                    StatusCode = HttpStatusCode.OK,
                    VendorList = listVendor
                };
            }
            catch (Exception ex)
            {
                return new GetAllVendorResult
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = ex.Message
                };
            }

        }

        public GetVendorOrderByIdResult GetVendorOrderById(GetVendorOrderByIdParameter parameter)
        {
            try
            {
                Guid vendorOrderId = Guid.Empty;
                if (parameter.VendorOrderId != Guid.Empty)
                {
                    vendorOrderId = parameter.VendorOrderId;
                }
                else
                {
                    if (parameter.CustomerOrderId != Guid.Empty)
                    {
                        vendorOrderId = context.VendorOrder.FirstOrDefault(vo => vo.CustomerOrderId == parameter.CustomerOrderId).VendorOrderId;
                    }
                }

                var vendorOrder = (from c in context.User
                                   join vor in context.VendorOrder on c.EmployeeId equals vor.Orderer
                                   where vor.VendorOrderId == vendorOrderId
                                   select new VendorOrderEntityModel
                                   {
                                       VendorOrderId = vor.VendorOrderId,
                                       VendorOrderCode = vor.VendorOrderCode,
                                       VendorOrderDate = vor.VendorOrderDate,
                                       VendorContactId = vor.VendorContactId,
                                       Active = vor.Active,
                                       Amount = vor.Amount,
                                       BankAccountId = vor.BankAccountId,
                                       CreatedById = vor.CreatedById,
                                       CreatedDate = vor.CreatedDate,
                                       Description = vor.Description,
                                       DiscountType = vor.DiscountType,
                                       VendorId = vor.VendorId,
                                       DiscountValue = vor.DiscountValue,
                                       LocationOfShipment = vor.LocationOfShipment,
                                       Note = vor.Note,
                                       Orderer = vor.Orderer,
                                       PaymentMethod = vor.PaymentMethod,
                                       PlaceOfDelivery = vor.PlaceOfDelivery,
                                       ReceivedDate = vor.ReceivedDate,
                                       ReceivedHour = vor.ReceivedHour,
                                       StatusId = vor.StatusId,
                                       RecipientEmail = vor.RecipientEmail,
                                       RecipientName = vor.RecipientName,
                                       RecipientPhone = vor.RecipientPhone,
                                       ShippingNote = vor.ShippingNote
                                   }).FirstOrDefault();

                string createdByName = null;
                if (vendorOrder != null)
                {
                    var employee = context.Employee.FirstOrDefault(e => e.EmployeeId == vendorOrder.Orderer);
                    var identityId = context.Contact.FirstOrDefault(c => c.ObjectId == employee.EmployeeId)?.IdentityId;
                    createdByName = identityId + " - " + employee?.EmployeeName;
                    vendorOrder.CreatedByName = createdByName;
                }
                else
                {
                    return new GetVendorOrderByIdResult()
                    {
                        Status = false,
                        Message = CommonMessage.Vendor.GET_ORDER_FAIL
                    };
                }

                var contactId = context.Contact.FirstOrDefault(c => c.ObjectId == vendorOrder.VendorId && c.ObjectType == ObjectType.VENDOR).ContactId;

                var listVendorOrderDetail = (from co in context.VendorOrder
                                             where co.VendorOrderId == vendorOrderId
                                             join cod in context.VendorOrderDetail on co.VendorOrderId equals cod.VendorOrderId
                                             join vendor in context.Vendor on cod.VendorId equals vendor.VendorId
                                             select new VendorOrderDetailEntityModel
                                             {
                                                 Active = cod.Active,
                                                 CreatedById = cod.CreatedById,
                                                 VendorOrderId = cod.VendorOrderId,
                                                 VendorId = cod.VendorId,
                                                 CreatedDate = cod.CreatedDate,
                                                 CurrencyUnit = cod.CurrencyUnit,
                                                 Description = cod.Description,
                                                 DiscountType = cod.DiscountType,
                                                 DiscountValue = cod.DiscountValue,
                                                 ExchangeRate = cod.ExchangeRate,
                                                 VendorOrderDetailId = cod.VendorOrderDetailId,
                                                 OrderDetailType = cod.OrderDetailType,
                                                 ProductId = cod.ProductId.Value,
                                                 UpdatedById = cod.UpdatedById,
                                                 Quantity = cod.Quantity,
                                                 UnitId = cod.UnitId,
                                                 IncurredUnit = cod.IncurredUnit,
                                                 UnitPrice = cod.UnitPrice,
                                                 UpdatedDate = cod.UpdatedDate,
                                                 Vat = cod.Vat,
                                                 NameVendor = vendor.VendorName,
                                                 NameProduct = cod.ProductId != null ? context.Product.FirstOrDefault(p => p.ProductId == cod.ProductId).ProductName : "",
                                                 NameProductUnit = cod.UnitId != null ? context.Category.FirstOrDefault(c => c.CategoryId == cod.UnitId).CategoryName : "",
                                                 NameMoneyUnit = cod.CurrencyUnit != null ? context.Category.FirstOrDefault(c => c.CategoryId == cod.CurrencyUnit).CategoryName : "",
                                                 SumAmount = SumAmount(cod.Quantity, cod.UnitPrice, cod.ExchangeRate, cod.Vat, cod.DiscountValue, cod.DiscountType)
                                             }).ToList();

                listVendorOrderDetail.ForEach(item =>
                {
                    item.NameGene = item.NameProduct + "(" + getNameGEn(item.VendorOrderDetailId) + ")";
                    item.VendorOrderProductDetailProductAttributeValue = getListOrderProductDetailProductAttributeValue(item.VendorOrderDetailId);
                });

                return new GetVendorOrderByIdResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    VendorOrder = vendorOrder,
                    VendorOrderDetailList = listVendorOrderDetail,
                    ContactId = contactId
                };
            }
            catch (Exception ex)
            {
                return new GetVendorOrderByIdResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = ex.Message
                };
            }

        }

        public UpdateVendorOrderByIdResult UpdateVendorOrderById(UpdateVendorOrderByIdParameter parameter)
        {
            try
            {
                bool isValidParameterNumber = true;
                if (parameter.VendorOrder?.DiscountValue < 0)
                {
                    isValidParameterNumber = false;
                }
                foreach (var item in parameter.VendorOrderDetail)
                {
                    if (item?.Quantity <= 0 || item?.UnitPrice < 0 || item?.Vat < 0 || item?.DiscountValue < 0)
                    {
                        isValidParameterNumber = false;
                        break;
                    }
                }

                if (!isValidParameterNumber)
                {
                    return new UpdateVendorOrderByIdResult()
                    {
                        MessageCode = CommonMessage.Vendor.EDIT_ORDER_FAIL,
                        StatusCode = HttpStatusCode.ExpectationFailed
                    };
                }

                #region Delete all item Relation 

                var oldOrder =
                    context.VendorOrder.FirstOrDefault(co => co.VendorOrderId == parameter.VendorOrder.VendorOrderId);
                parameter.VendorOrder.CreatedById = oldOrder.CreatedById;
                parameter.VendorOrder.CreatedDate = oldOrder.CreatedDate;
                parameter.VendorOrder.UpdatedById = parameter.UserId;
                parameter.VendorOrder.UpdatedDate = DateTime.Now;

                var oldAmount = oldOrder.Amount;

                var listItemInvalidModel = new List<ItemInvalidModel>();

                using (var transaction = context.Database.BeginTransaction())
                {
                    var listAllStatus = context.PurchaseOrderStatus.Where(x => x.Active == true).ToList();
                    var oldStt = listAllStatus
                        .FirstOrDefault(ord => ord.PurchaseOrderStatusId == oldOrder.StatusId)
                        .PurchaseOrderStatusCode;
                    var List_Delete_OrderProductDetailProductAttributeValue =
                        new List<VendorOrderProductDetailProductAttributeValue>();
                    var detailList = context.VendorOrderDetail
                        .Where(vod => vod.VendorOrderId == parameter.VendorOrder.VendorOrderId).ToList();

                    var List_Delete_VendorOrderDetail = new List<VendorOrderDetail>();
                    detailList.ForEach(item =>
                    {
                        if (item.VendorOrderDetailId != Guid.Empty)
                        {
                            var VendorOrderProductDetailProductAttributeValueList = context
                                .VendorOrderProductDetailProductAttributeValue.Where(OPDPAV =>
                                    OPDPAV.VendorOrderDetailId == item.VendorOrderDetailId).ToList();
                            List_Delete_OrderProductDetailProductAttributeValue.AddRange(
                                VendorOrderProductDetailProductAttributeValueList);
                            item.VendorOrderProductDetailProductAttributeValue = null;
                            List_Delete_VendorOrderDetail.Add(item);
                        }
                    });

                    context.VendorOrderProductDetailProductAttributeValue.RemoveRange(
                        List_Delete_OrderProductDetailProductAttributeValue);
                    context.SaveChanges();

                    context.VendorOrderDetail.RemoveRange(List_Delete_VendorOrderDetail);
                    context.SaveChanges();

                    var listOldCost = context.VendorOrderCostDetail
                        .Where(x => x.VendorOrderId == parameter.VendorOrder.VendorOrderId).ToList();
                    context.VendorOrderCostDetail.RemoveRange(listOldCost);
                    context.SaveChanges();

                    var listOldVendorOrderProcurementRequestMapping = context.VendorOrderProcurementRequestMapping
                        .Where(x => x.VendorOrderId == parameter.VendorOrder.VendorOrderId).ToList();
                    context.VendorOrderProcurementRequestMapping.RemoveRange(listOldVendorOrderProcurementRequestMapping);
                    context.SaveChanges();

                    context.VendorOrder.Remove(oldOrder);
                    context.SaveChanges();

                    #region Nếu Gửi phê duyệt

                    if (parameter.IsSendApproval)
                    {
                        //Lấy list phiếu đề xuất mua hàng
                        var listProcurementRequestId = parameter.VendorOrderDetail
                            .Select(y => y.ProcurementRequestId).Distinct().ToList();

                        //Lấy list item trong phiếu đề xuất mua hàng
                        var listProcurementRequestItem = context.ProcurementRequestItem
                            .Where(x => listProcurementRequestId.Contains(x.ProcurementRequestId)).ToList();

                        //Lấy list item từ phiếu đề xuất trong đơn hàng mua
                        var listRequestItemOrderDetail = parameter.VendorOrderDetail
                            .Where(x => x.ProcurementRequestItemId != null).ToList();

                        //Lấy list 
                        var listRequestItemId = listRequestItemOrderDetail.Select(y => y.ProcurementRequestItemId).ToList();

                        //Lấy các đơn hàng mua có trạng thái Đơn hàng mua và Đóng
                        var _listStatusCode = new List<string> { "PURC", "COMP" };
                        var _listStatusId = context.PurchaseOrderStatus
                            .Where(x => _listStatusCode.Contains(x.PurchaseOrderStatusCode))
                            .Select(y => y.PurchaseOrderStatusId).ToList();
                        var _listVendorOrderId = context.VendorOrder.Where(x => _listStatusId.Contains(x.StatusId))
                            .Select(y => y.VendorOrderId).ToList();

                        //Lấy các item có trong list đơn hàng mua trên
                        var listRequestItemHasUsing = context.VendorOrderDetail.Where(x =>
                                _listVendorOrderId.Contains(x.VendorOrderId) &&
                                listRequestItemId.Contains(x.ProcurementRequestItemId))
                            .GroupBy(x => new
                            {
                                x.ProcurementRequestItemId
                            })
                            .Select(y => new RequestItem
                            {
                                ProcurementRequestItemId = y.Key.ProcurementRequestItemId,
                                Quantity = y.Sum(s => s.Quantity)
                            }).ToList();

                        //Với mỗi item từ phiếu đề xuất trong đơn hàng mua => kiểm tra xem số lượng có hợp lệ hay không
                        parameter.VendorOrderDetail.ForEach(item =>
                        {
                            if (item.ProcurementRequestItemId != null)
                            {
                                var itemHasUsing =
                                    listRequestItemHasUsing.FirstOrDefault(x =>
                                        x.ProcurementRequestItemId == item.ProcurementRequestItemId);

                                var baseItem = listProcurementRequestItem.FirstOrDefault(x =>
                                    x.ProcurementRequestItemId == item.ProcurementRequestItemId);

                                if (itemHasUsing != null && baseItem != null)
                                {
                                    var currentQuantity = item.Quantity;
                                    var baseQuantity = baseItem.QuantityApproval;
                                    var usingQuantity = itemHasUsing.Quantity;
                                    var remainQuantity = baseQuantity - usingQuantity;

                                    if (currentQuantity > baseQuantity - usingQuantity)
                                    {
                                        var itemInvalid = new ItemInvalidModel();
                                        itemInvalid.ProcurementRequestItemId = item.ProcurementRequestItemId;
                                        itemInvalid.RemainQuantity = remainQuantity;
                                        listItemInvalidModel.Add(itemInvalid);
                                    }
                                }
                            }
                        });

                        //Nếu không có Item nào không hợp lệ thì đổi trạng thái Đơn hàng mua -> Chờ phê duyệt
                        if (listItemInvalidModel.Count == 0)
                        {
                            var sendApprovalId = listAllStatus
                                .FirstOrDefault(c => c.PurchaseOrderStatusCode == "IP")
                                .PurchaseOrderStatusId;
                            parameter.VendorOrder.StatusId = sendApprovalId;
                        }
                        else
                        {
                            return new UpdateVendorOrderByIdResult()
                            {
                                StatusCode = HttpStatusCode.OK,
                                MessageCode = "Has Item Invalid",
                                ListItemInvalidModel = listItemInvalidModel
                            };
                        }
                    }

                    #endregion

                    var listVendorOrderDetail = new List<VendorOrderDetail>();
                    parameter.VendorOrderDetail.ForEach(item =>
                    {
                        item.VendorOrderDetailId = Guid.NewGuid();

                        if (item.VendorId == Guid.Empty)
                        {
                            item.VendorId = parameter.VendorOrder.VendorId;
                        }

                        if (item.VendorOrderId == Guid.Empty)
                        {
                            item.VendorOrderId = parameter.VendorOrder.VendorOrderId;
                        }

                        if (item.VendorOrderProductDetailProductAttributeValue != null &&
                            item.VendorOrderProductDetailProductAttributeValue.Count != 0)
                        {
                            foreach (var itemX in item.VendorOrderProductDetailProductAttributeValue)
                            {
                                itemX.OrderProductDetailProductAttributeValueId = Guid.NewGuid();
                            }
                        }
                    });

                    parameter.VendorOrderDetail.ForEach(item =>
                    {
                        var newItem = new VendorOrderDetail();
                        newItem = item.ToEntity();

                        if (item.VendorOrderProductDetailProductAttributeValue != null &&
                            item.VendorOrderProductDetailProductAttributeValue.Count != 0)
                        {
                            item.VendorOrderProductDetailProductAttributeValue.ForEach(_item =>
                            {
                                var _newItem = _item.ToEntity();
                                newItem.VendorOrderProductDetailProductAttributeValue.Add(_newItem);
                            });
                        }

                        listVendorOrderDetail.Add(newItem);
                    });

                    context.VendorOrder.Add(parameter.VendorOrder.ToEntity());
                    context.SaveChanges();
                    context.VendorOrderDetail.AddRange(listVendorOrderDetail);
                    context.SaveChanges();

                    #region Bảng mapping giữa Chi phí và Đơn hàng mua

                    parameter.ListVendorOrderCostDetail.ForEach(item =>
                    {
                        var vendorOrderCostDetail = new VendorOrderCostDetail();
                        vendorOrderCostDetail.VendorOrderCostDetailId = Guid.NewGuid();
                        vendorOrderCostDetail.CostId = item.CostId;
                        vendorOrderCostDetail.VendorOrderId = parameter.VendorOrder.VendorOrderId;
                        vendorOrderCostDetail.UnitPrice = item.UnitPrice;
                        vendorOrderCostDetail.CostName = item.CostName;
                        vendorOrderCostDetail.Active = true;
                        vendorOrderCostDetail.CreatedById = parameter.VendorOrder.CreatedById;
                        vendorOrderCostDetail.CreatedDate = parameter.VendorOrder.CreatedDate;
                        vendorOrderCostDetail.UpdatedById = parameter.UserId;
                        vendorOrderCostDetail.UpdatedDate = DateTime.Now;

                        context.VendorOrderCostDetail.Add(vendorOrderCostDetail);
                    });

                    #endregion

                    #region Bảng mapping giữa Phiếu đề xuất và Đơn hàng mua

                    parameter.ListVendorOrderProcurementRequestMapping.ForEach(item =>
                    {
                        var vendorOrderProcurementRequestMapping = new VendorOrderProcurementRequestMapping();
                        vendorOrderProcurementRequestMapping.VendorOrderProcurementRequestMappingId = Guid.NewGuid();
                        vendorOrderProcurementRequestMapping.VendorOrderId = parameter.VendorOrder.VendorOrderId;
                        vendorOrderProcurementRequestMapping.ProcurementRequestId = item.ProcurementRequestId;
                        vendorOrderProcurementRequestMapping.Active = true;
                        vendorOrderProcurementRequestMapping.CreatedById = parameter.VendorOrder.CreatedById;
                        vendorOrderProcurementRequestMapping.CreatedDate = parameter.VendorOrder.CreatedDate;
                        vendorOrderProcurementRequestMapping.UpdatedById = parameter.UserId;
                        vendorOrderProcurementRequestMapping.UpdatedDate = DateTime.Now;

                        context.VendorOrderProcurementRequestMapping.Add(vendorOrderProcurementRequestMapping);
                    });

                    #endregion

                    #region Cập nhật lại tổng thanh toán của nhà cung cấp

                    var newOrderSttCode = context.PurchaseOrderStatus
                        .FirstOrDefault(ord => ord.PurchaseOrderStatusId == parameter.VendorOrder.StatusId)
                        .PurchaseOrderStatusCode;
                    var vendor = context.Vendor.FirstOrDefault(c => c.VendorId == parameter.VendorOrder.VendorId);
                    var totalPurchaseValue = vendor.TotalPurchaseValue;
                    var totalPayableValue = vendor.TotalPayableValue;

                    var listStatusAccept = context.SystemParameter.FirstOrDefault(x => x.SystemKey == "PurchaseOrderStatus")
                        ?.SystemValueString.Split(';').ToList();

                    if (listStatusAccept != null && listStatusAccept.Contains(oldStt))
                    {
                        totalPurchaseValue = totalPurchaseValue - oldAmount;
                        totalPayableValue = totalPayableValue - oldAmount;
                    }

                    if (listStatusAccept != null && listStatusAccept.Contains(newOrderSttCode))
                    {
                        totalPurchaseValue = totalPurchaseValue + parameter.VendorOrder.Amount;
                        totalPayableValue = totalPayableValue + parameter.VendorOrder.Amount;
                    }

                    vendor.TotalPurchaseValue = totalPurchaseValue;
                    vendor.TotalPayableValue = totalPayableValue;
                    context.Vendor.Update(vendor);
                    context.SaveChanges();

                    #endregion

                    transaction.Commit();
                }

                #endregion

                #region Gửi mail thông báo

                if (parameter.IsSendApproval)
                {
                    NotificationHelper.AccessNotification(context, TypeModel.VendorOrderDetail, "SEND_APPROVAL",
                        new VendorOrder(), parameter.VendorOrder.ToEntity(), true);
                }
                else
                {
                    NotificationHelper.AccessNotification(context, TypeModel.VendorOrderDetail, "UPD",
                        new VendorOrder(), parameter.VendorOrder.ToEntity(), true, empId: parameter.VendorOrder.Orderer);
                }

                #endregion

                #region Lưu nhật ký hệ thống

                if (!parameter.IsSendApproval)
                {
                    LogHelper.AuditTrace(context, ActionName.UPDATE, ObjectName.VENDORORDER,
                        parameter.VendorOrder.VendorOrderId, parameter.UserId);
                }

                #endregion

                return new UpdateVendorOrderByIdResult()
                {
                    MessageCode = CommonMessage.Vendor.EDIT_ORDER_SUCCESS,
                    StatusCode = HttpStatusCode.OK,
                    ListItemInvalidModel = listItemInvalidModel
                };
            }
            catch (Exception e)
            {
                return new UpdateVendorOrderByIdResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        private class RequestItem
        {
            public Guid? ProcurementRequestItemId { get; set; }
            public decimal? Quantity { get; set; }
        }

        //private string GenerateorderCode()
        //{
        //    string currentYear = DateTime.Now.Year.ToString();
        //    int count = context.VendorOrder.Count();
        //    string result = "DH-" + currentYear.Substring(currentYear.Length - 2) + returnNumberOrder(count.ToString());
        //    return result;
        //}

        private string ReGenerateorderCode(List<string> listVendorCode, int totalVendorOrder)
        {
            string currentYear = DateTime.Now.Year.ToString();
            string result = "DH-" + currentYear.Substring(currentYear.Length - 2) + returnNumberOrder(totalVendorOrder.ToString());
            var checkDuplidate = listVendorCode.FirstOrDefault(f => f == result);
            if (checkDuplidate != null) return ReGenerateorderCode(listVendorCode, totalVendorOrder + 1);
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
            }
        }

        private decimal SumAmount(decimal? Quantity, decimal? UnitPrice, decimal? ExchangeRate, decimal? Vat, decimal? DiscountValue, bool? DiscountType)
        {
            decimal result = 0;
            decimal CaculateVAT = 0;
            decimal CacuDiscount = 0;

            if (Vat != null)
            {
                CaculateVAT = (Quantity.Value * UnitPrice.Value * ExchangeRate.Value * Vat.Value) / 100;
            }
            if (DiscountValue != null)
            {
                if (DiscountType == true)
                {
                    CacuDiscount = ((Quantity.Value * UnitPrice.Value * ExchangeRate.Value * DiscountValue.Value) / 100);
                }
                else
                {
                    CacuDiscount = DiscountValue.Value;
                }
            }
            result = (Quantity.Value * UnitPrice.Value * ExchangeRate.Value) + CaculateVAT - CacuDiscount;
            return result;
        }

        public string getNameGEn(Guid vendorDetailID)
        {
            string Result = string.Empty;
            var OrderProductDetailProductAttributeValueModelList =
                (from OPDPV in context.VendorOrderProductDetailProductAttributeValue
                 join ProductAttributeCategoryV in context.ProductAttributeCategoryValue on
                     OPDPV.ProductAttributeCategoryValueId equals ProductAttributeCategoryV
                         .ProductAttributeCategoryValueId
                 where OPDPV.VendorOrderDetailId == vendorDetailID
                 select (ProductAttributeCategoryV)).ToList();

            OrderProductDetailProductAttributeValueModelList.ForEach(item =>
            {
                Result = Result + item.ProductAttributeCategoryValue1 + ";";
            });

            return Result;
        }

        public List<VendorOrderProductDetailProductAttributeValueEntityModel> getListOrderProductDetailProductAttributeValue(Guid vendorDetailID)
        {
            List<VendorOrderProductDetailProductAttributeValueEntityModel> listResult = new List<VendorOrderProductDetailProductAttributeValueEntityModel>();
            var OrderProductDetailProductAttributeValueModelList =
                (from OPDPV in context.VendorOrderProductDetailProductAttributeValue
                 join ProductAttributeC in context.ProductAttributeCategory on OPDPV.ProductAttributeCategoryId
                     equals ProductAttributeC.ProductAttributeCategoryId
                 join ProductAttributeCategoryV in context.ProductAttributeCategoryValue on
                     OPDPV.ProductAttributeCategoryValueId equals ProductAttributeCategoryV
                         .ProductAttributeCategoryValueId
                 where OPDPV.VendorOrderDetailId == vendorDetailID
                 select (new VendorOrderProductDetailProductAttributeValueEntityModel
                 {
                     VendorOrderDetailId = OPDPV.VendorOrderDetailId,
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

        public int CountVendorInformation(Guid vendorId, List<ProductVendorMapping> productVendorMappings, List<CustomerOrderDetail> customerOrderDetails)
        {
            var countVendor = productVendorMappings.Where(p => p.VendorId == vendorId).Count();
            countVendor += customerOrderDetails.Where(c => c.VendorId == vendorId).Count();

            return countVendor;
        }

        public UpdateActiveVendorResult UpdateActiveVendor(UpdateActiveVendorParameter parameter)
        {
            try
            {
                var vendorOld = context.Vendor.FirstOrDefault(v => v.VendorId == parameter.VendorId);
                vendorOld.Active = false;
                vendorOld.UpdatedById = parameter.UserId;
                vendorOld.UpdatedDate = DateTime.Now;

                #region inactive vendor contact - Add by Dung
                var listVendorContact = context.Contact.Where(w => w.ObjectId == parameter.VendorId).ToList();
                listVendorContact?.ForEach(contact =>
                {
                    contact.Active = false;
                    contact.UpdatedById = parameter.UserId;
                    contact.UpdatedDate = DateTime.Now;
                });
                #endregion

                context.Vendor.Update(vendorOld);
                context.Contact.UpdateRange(listVendorContact);
                context.SaveChanges();
                return new UpdateActiveVendorResult()
                {
                    MessageCode = CommonMessage.Vendor.DELETE_SUCCESS,
                    StatusCode = HttpStatusCode.OK
                };
            }
            catch (Exception e)
            {
                return new UpdateActiveVendorResult()
                {
                    MessageCode = CommonMessage.Vendor.DELETE_FAIL,
                    StatusCode = HttpStatusCode.ExpectationFailed
                };

            }
        }

        public QuickCreateVendorMasterdataResult QuickCreateVendorMasterdata(QuickCreateVendorMasterdataParameter parameter)
        {
            try
            {
                #region Get Master data

                var VENDOR_GROUP_CODE = "NCA";
                var vendorGroupCategoryId = context.CategoryType.Where(w => w.CategoryTypeCode == VENDOR_GROUP_CODE)
                    .FirstOrDefault().CategoryTypeId;
                var listVendorGroup = context.Category
                    .Where(w => w.CategoryTypeId == vendorGroupCategoryId && w.Active == true).Select(w => new
                    {
                        w.CategoryId,
                        w.CategoryName,
                        w.IsDefauld
                    }).ToList();
                var listVendorCodeEntity = context.Vendor.Select(w => new { w.VendorCode }).ToList();

                #endregion

                #region Response

                var listVendorCategory = new List<CategoryEntityModel>();
                listVendorGroup.ForEach(vendorGroup =>
                {
                    listVendorCategory.Add(new CategoryEntityModel
                    {
                        CategoryId = vendorGroup.CategoryId,
                        CategoryName = vendorGroup.CategoryName,
                        IsDefauld = vendorGroup.IsDefauld
                    });
                });

                listVendorCategory = listVendorCategory.OrderBy(w => w.CategoryName).ToList();

                var listVendorCode = new List<string>();
                listVendorCodeEntity.ForEach(vendor =>
                {
                    listVendorCode.Add(vendor.VendorCode);
                });

                #endregion

                return new QuickCreateVendorMasterdataResult()
                {
                    ListVendorCategory = listVendorCategory,
                    ListVendorCode = listVendorCode,
                    StatusCode = HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new QuickCreateVendorMasterdataResult()
                {
                    MessageCode = ex.ToString(),
                    StatusCode = HttpStatusCode.ExpectationFailed
                };
            }
        }

        public GetDataCreateVendorResult GetDataCreateVendor(GetDataCreateVendorParameter parameter)
        {
            try
            {
                #region Get list vendor group 
                var vendorGroupTypeId = context.CategoryType.FirstOrDefault(f => f.CategoryTypeCode == "NCA").CategoryTypeId;
                var listVendorGroup = context.Category.Where(w => w.Active == true && w.CategoryTypeId == vendorGroupTypeId).ToList();
                #endregion

                #region Get Address
                var listProvince = context.Province.Select(w => new Models.Address.ProvinceEntityModel
                {
                    ProvinceId = w.ProvinceId,
                    ProvinceName = w.ProvinceName.Trim(),
                    ProvinceCode = w.ProvinceCode.Trim(),
                    ProvinceType = w.ProvinceType,
                    Active = w.Active
                }).ToList();

                var listDistrict = context.District.Select(w => new Models.Address.DistrictEntityModel
                {
                    DistrictId = w.DistrictId,
                    ProvinceId = w.ProvinceId,
                    DistrictName = w.DistrictName.Trim(),
                    DistrictCode = w.DistrictCode.Trim(),
                    DistrictType = w.DistrictType,
                    Active = w.Active
                }).ToList();

                var listWard = context.Ward.Select(w => new Models.Address.WardEntityModel
                {
                    WardId = w.WardId,
                    DistrictId = w.DistrictId,
                    WardName = w.WardName.Trim(),
                    WardCode = w.WardCode.Trim(),
                    WardType = w.WardType,
                    Active = w.Active
                }).ToList();
                #endregion

                #region Get list vendor code
                var listVendorCode = context.Vendor.Where(w => w.Active == true && !string.IsNullOrWhiteSpace(w.VendorCode)).Select(w => w.VendorCode).ToList();
                #endregion

                var listVendorGroupResult = new List<CategoryEntityModel>();
                var _listVendorGroup = listVendorGroup.OrderBy(w => w.CategoryName).ToList();
                _listVendorGroup.ForEach(item =>
                {
                    var _item = new CategoryEntityModel(item);
                    listVendorGroupResult.Add(_item);
                });

                return new GetDataCreateVendorResult()
                {
                    ListVendorGroup = listVendorGroupResult,
                    ListProvince = listProvince?.OrderBy(w => w.ProvinceName).ToList() ?? new List<Models.Address.ProvinceEntityModel>(),
                    ListDistrict = listDistrict?.OrderBy(w => w.DistrictName).ToList() ?? new List<Models.Address.DistrictEntityModel>(),
                    ListWard = listWard?.OrderBy(w => w.WardName).ToList() ?? new List<Models.Address.WardEntityModel>(),
                    ListVendorCode = listVendorCode ?? new List<string>(),
                    StatusCode = HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new GetDataCreateVendorResult()
                {
                    MessageCode = ex.ToString(),
                    StatusCode = HttpStatusCode.ExpectationFailed
                };
            }
        }

        public GetDataSearchVendorResult GetDataSearchVendor(GetDataSearchVendorParameter parameter)
        {
            try
            {
                #region Get list vendor group 
                var vendorGroupTypeId = context.CategoryType.FirstOrDefault(f => f.CategoryTypeCode == "NCA").CategoryTypeId;
                var listVendorGroup = context.Category.Where(w => w.Active == true && w.CategoryTypeId == vendorGroupTypeId).ToList();
                #endregion
                var _listVendorGroup = listVendorGroup.OrderBy(w => w.CategoryName).ToList();
                var listVendorGroupResult = new List<CategoryEntityModel>();
                _listVendorGroup.ForEach(item =>
                {
                    var _item = new CategoryEntityModel(item);
                    listVendorGroupResult.Add(_item);
                });

                return new GetDataSearchVendorResult()
                {
                    ListVendorGroup = listVendorGroupResult,
                    StatusCode = HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new GetDataSearchVendorResult()
                {
                    MessageCode = ex.ToString(),
                    StatusCode = HttpStatusCode.ExpectationFailed
                };
            }
        }

        public GetDataEditVendorResult GetDataEditVendor(GetDataEditVendorParameter parameter)
        {
            try
            {
                #region Get Category
                var vendorGroupTypeId = context.CategoryType.FirstOrDefault(f => f.CategoryTypeCode == "NCA").CategoryTypeId;
                var listVendorGroup = context.Category.Where(w => w.Active == true && w.CategoryTypeId == vendorGroupTypeId).ToList();
                var paymentMethodTypeId = context.CategoryType.FirstOrDefault(f => f.CategoryTypeCode == "PTO").CategoryTypeId;
                var listPaymentMethod = context.Category.Where(w => w.Active == true && w.CategoryTypeId == paymentMethodTypeId).ToList();
                #endregion

                #region Get Address
                var listProvince = context.Province.Select(w => new Models.Address.ProvinceEntityModel
                {
                    ProvinceId = w.ProvinceId,
                    ProvinceName = w.ProvinceName.Trim(),
                    ProvinceCode = w.ProvinceCode.Trim(),
                    ProvinceType = w.ProvinceType,
                    Active = w.Active
                }).ToList();

                var listDistrict = context.District.Select(w => new Models.Address.DistrictEntityModel
                {
                    DistrictId = w.DistrictId,
                    ProvinceId = w.ProvinceId,
                    DistrictName = w.DistrictName.Trim(),
                    DistrictCode = w.DistrictCode.Trim(),
                    DistrictType = w.DistrictType,
                    Active = w.Active
                }).ToList();

                var listWard = context.Ward.Select(w => new Models.Address.WardEntityModel
                {
                    WardId = w.WardId,
                    DistrictId = w.DistrictId,
                    WardName = w.WardName.Trim(),
                    WardCode = w.WardCode.Trim(),
                    WardType = w.WardType,
                    Active = w.Active
                }).ToList();
                #endregion

                #region Get list vendor code
                var listVendorCode = context.Vendor.Where(w => w.Active == true && !string.IsNullOrWhiteSpace(w.VendorCode) && w.VendorId != parameter.VendorId).Select(w => w.VendorCode).ToList();
                #endregion

                #region Lấy thông tin số liệu thống kê (Tổng đặt SP/DV, Nợ phải trả, Đang xử lý)
                //Lấy ra Tổng đặt SP/DV, đang xử lý trong năm
                var currentYear = DateTime.Now.Year;
                var currentMonth = DateTime.Now.Month;
                var listVendorOrderByMonth = new List<Models.Vendor.VendorOrderByMonthModel>();
                var listVendorOrderInProcessByMonth = new List<Models.Vendor.VendorOrderByMonthModel>();
                var listReceivableByMonth = new List<Models.Vendor.VendorOrderByMonthModel>();

                //loại những trạng thái đơn hàng sau
                //    1. hoãn:'ON'
                //    2. bị trả lại: 'RTN'
                //    3. hủy: 'CAN'
                //    4. sai: 'WO'
                //    5. nháp: 'DRA'

                var listIgnoreCode = new List<string> { "ON", "RTN", "CAN", "WO", "DRA" };

                var listIgnoreStatusId = context.OrderStatus.Where(w => listIgnoreCode.Contains(w.OrderStatusCode)).Select(w => w.OrderStatusId).ToList();
                var inProcessStatusId = context.OrderStatus.FirstOrDefault(f => f.OrderStatusCode == "IP").OrderStatusId;

                //trường Amount trong db là tổng trước chiết khấu
                var listVendorOrder = context.VendorOrder.Where(w => w.Active == true
                                                            && w.VendorOrderDate.Year == currentYear
                                                            && w.VendorOrderDate.Month <= currentMonth
                                                            && w.VendorId == parameter.VendorId
                                                            && !listIgnoreStatusId.Contains(w.StatusId)
                                                            ).ToList();

                var listVendorOrderInProcess = listVendorOrder.Where(w => w.StatusId == inProcessStatusId).ToList();

                //master data
                var listPayableInvoice = context.PayableInvoice.ToList();
                var listPayableInvoiceMapping = context.PayableInvoiceMapping.ToList();
                var listBankPayableInvoice = context.BankPayableInvoice.ToList();
                var listBankPayableInvoiceMapping = context.BankPayableInvoiceMapping.ToList();
                var listVendor = context.Vendor.ToList();

                for (int i = 1; i <= currentMonth; i++)
                {
                    #region Tính tổng đơn hàng
                    var temp = new Models.Vendor.VendorOrderByMonthModel();
                    var orderbyMonth = listVendorOrder.Where(w => w.VendorOrderDate.Month == i).ToList();
                    decimal? sum = 0;
                    orderbyMonth.ForEach(order =>
                    {
                        if (order.DiscountType == true)
                        {
                            //Tính theo phần trăm
                            var discountAmount = (order.DiscountValue * order.Amount) / 100;
                            sum += order.Amount - discountAmount;
                        }
                        else
                        {
                            //Tính theo số tiền
                            sum += order.Amount - order.DiscountValue;
                        }
                    });

                    temp.Month = i;
                    temp.Amount = sum;

                    listVendorOrderByMonth.Add(temp);
                    #endregion

                    #region Tính đơn hàng đang xử lý
                    var temp_inprocess = new Models.Vendor.VendorOrderByMonthModel();
                    var orderbyMonth_inprocess = listVendorOrderInProcess.Where(w => w.VendorOrderDate.Month == i).ToList();
                    decimal? sum_inprocess = 0;
                    orderbyMonth_inprocess.ForEach(order =>
                    {
                        if (order.DiscountType == true)
                        {
                            //Tính theo phần trăm
                            var discountAmount = (order.DiscountValue * order.Amount) / 100;
                            sum_inprocess += order.Amount - discountAmount;
                        }
                        else
                        {
                            //Tính theo số tiền
                            sum_inprocess += order.Amount - order.DiscountValue;
                        }
                    });

                    temp_inprocess.Month = i;
                    temp_inprocess.Amount = sum_inprocess;

                    listVendorOrderInProcessByMonth.Add(temp_inprocess);
                    #endregion

                    #region Lấy dư nợ

                    // Nợ phát sinh trong kỳ (Tổng giá trị các đơn đặt hàng Nhà cung cấp trong kỳ)
                    var totalValueOrder = temp.Amount;

                    // Danh sách phiếu chi trong kỳ
                    var payableCashList = (from p in listPayableInvoice
                                           join pom in listPayableInvoiceMapping on p.PayableInvoiceId equals pom.PayableInvoiceId
                                           join v in listVendor on pom.ObjectId equals v.VendorId
                                           where v.VendorId == parameter.VendorId
                                                 //&& (p.PaidDate.Date >= parameter.ReceivalbeDateFrom.Date)
                                                 //&& (parameter.ReceivalbeDateTo == DateTime.MinValue ||
                                                 //    p.PaidDate.Date <= parameter.ReceivalbeDateTo.Date)
                                                 && p.PaidDate.Year == currentYear && p.PaidDate.Month == i
                                           select new ReceivableVendorReportEntityModel
                                           {
                                               CreateDateReceiptInvoice = p.PaidDate,
                                               ReceiptInvoiceValue = p.PayableInvoicePrice * (p.ExchangeRate ?? 1),
                                               DescriptionReceipt = p.PayableInvoiceDetail,
                                               ReceiptCode = p.PayableInvoiceCode,
                                               CreatedBy = p.CreatedById
                                           }).ToList();

                    // Danh sách phiếu UNC trong kỳ
                    var payableBankList = (from p in listBankPayableInvoice
                                           join pom in listBankPayableInvoiceMapping on p.BankPayableInvoiceId equals pom.BankPayableInvoiceId
                                           join v in listVendor on pom.ObjectId equals v.VendorId
                                           where v.VendorId == parameter.VendorId
                                                //&& (parameter.ReceivalbeDateFrom == DateTime.MinValue ||
                                                //    p.BankPayableInvoicePaidDate.Date >= parameter.ReceivalbeDateFrom.Date)
                                                //&& (parameter.ReceivalbeDateTo == DateTime.MinValue ||
                                                //    p.BankPayableInvoicePaidDate.Date <= parameter.ReceivalbeDateTo.Date)
                                                && p.BankPayableInvoicePaidDate.Year == currentYear
                                                && p.BankPayableInvoicePaidDate.Month == i

                                           select new ReceivableVendorReportEntityModel
                                           {
                                               CreateDateReceiptInvoice = p.BankPayableInvoicePaidDate,
                                               ReceiptInvoiceValue = p.BankPayableInvoiceAmount,
                                               DescriptionReceipt = p.BankPayableInvoiceDetail,
                                               ReceiptCode = p.BankPayableInvoiceCode,
                                               CreatedBy = p.CreatedById
                                           }).ToList();

                    var totalPayList = new List<ReceivableVendorReportEntityModel>();
                    totalPayList.AddRange(payableBankList);
                    totalPayList.AddRange(payableCashList);

                    // Thanh toán trong kỳ
                    var totalValueReceipt = totalPayList.Sum(v => v.ReceiptInvoiceValue);

                    // Danh sách đơn đặt hàng kỳ trước
                    var vendorOrder = listVendorOrder.Where(v => v.VendorId == parameter.VendorId
                                                                //&& v.CreatedDate.Date < parameter.ReceivalbeDateFrom.Date
                                                                && v.CreatedDate.Year <= currentYear
                                                                && v.CreatedDate.Month < i
                                                                ).ToList();

                    // Danh sách phiếu chi kỳ trước
                    var payCashBefore = (from p in listPayableInvoice
                                         join pom in listPayableInvoiceMapping on p.PayableInvoiceId equals pom.PayableInvoiceId
                                         where pom.ObjectId == parameter.VendorId
                                         //&&  p.PaidDate.Date < parameter.ReceivalbeDateFrom.Date
                                              && p.PaidDate.Year <= currentYear
                                              && p.PaidDate.Month < i
                                         select new ReceivableVendorReportEntityModel
                                         {
                                             ReceiptInvoiceValue = p.PayableInvoicePrice * (p.ExchangeRate ?? 1)
                                         }).ToList();

                    // Danh sách phiếu UNC kỳ trước
                    var payBankBefore = (from pb in listBankPayableInvoice
                                         join pbom in listBankPayableInvoiceMapping on pb.BankPayableInvoiceId equals pbom.BankPayableInvoiceId
                                         where pbom.ObjectId == parameter.VendorId
                                            //&& pb.BankPayableInvoicePaidDate.Date < parameter.ReceivalbeDateFrom.Date 
                                            && pb.BankPayableInvoicePaidDate.Year <= currentYear
                                              && pb.BankPayableInvoicePaidDate.Month < i
                                         select new ReceivableVendorReportEntityModel
                                         {
                                             ReceiptInvoiceValue = pb.BankPayableInvoiceAmount * (pb.BankPayableInvoiceExchangeRate ?? 1)
                                         }).ToList();

                    var receiptBefore = new List<ReceivableVendorReportEntityModel>();
                    receiptBefore.AddRange(payBankBefore);
                    receiptBefore.AddRange(payCashBefore);

                    // Tổng giá trị các đơn đặt hàng kỳ trước
                    decimal totalValueVendorOrderBefore = 0;
                    foreach (var order in vendorOrder)
                    {
                        totalValueVendorOrderBefore += CalculatorTotalPurchaseProduct(order.Amount, order.DiscountType.Value, order.DiscountValue.Value);
                    }

                    // Dư nợ đầu kỳ
                    var totalReceivableBefore = totalValueVendorOrderBefore - receiptBefore.Sum(r => r.ReceiptInvoiceValue);

                    // 
                    var totalReceivableInPeriod = totalValueOrder;

                    // Dư nợ cuối kỳ
                    var totalReceivable = totalValueOrder - totalValueReceipt + totalReceivableBefore;


                    var temp_receivable = new Models.Vendor.VendorOrderByMonthModel();
                    temp_receivable.Month = i;
                    temp_receivable.Amount = totalReceivable;
                    listReceivableByMonth.Add(temp_receivable);
                    #endregion

                }
                #endregion

                var listVendorGroupResult = new List<CategoryEntityModel>();
                var _listVendorGroup = listVendorGroup.OrderBy(w => w.CategoryName).ToList();
                _listVendorGroup.ForEach(item =>
                {
                    var _item = new CategoryEntityModel(item);
                    listVendorGroupResult.Add(_item);
                });

                var listPaymentMethodResult = new List<CategoryEntityModel>();
                var _listPaymentMethodResult = listPaymentMethod.OrderBy(w => w.CategoryName).ToList();
                _listPaymentMethodResult.ForEach(item =>
                {
                    var _item = new CategoryEntityModel(item);
                    listPaymentMethodResult.Add(_item);
                });

                return new GetDataEditVendorResult()
                {
                    ListVendorGroup = listVendorGroupResult,
                    ListPaymentMethod = listPaymentMethodResult,
                    ListProvince = listProvince?.OrderBy(w => w.ProvinceName).ToList() ?? new List<Models.Address.ProvinceEntityModel>(),
                    ListDistrict = listDistrict?.OrderBy(w => w.DistrictName).ToList() ?? new List<Models.Address.DistrictEntityModel>(),
                    ListWard = listWard?.OrderBy(w => w.WardName).ToList() ?? new List<Models.Address.WardEntityModel>(),
                    ListVendorCode = listVendorCode ?? new List<string>(),
                    ListVendorOrderByMonth = listVendorOrderByMonth,
                    ListVendorOrderInProcessByMonth = listVendorOrderInProcessByMonth,
                    ListReceivableByMonth = listReceivableByMonth,
                    StatusCode = HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new GetDataEditVendorResult()
                {
                    MessageCode = ex.ToString(),
                    StatusCode = HttpStatusCode.ExpectationFailed
                };
            }
        }

        private decimal CalculatorTotalPurchaseProduct(decimal? amount, bool? discountType, decimal? discountValue)
        {
            decimal result = 0;

            amount = amount ?? 0;
            discountType = discountType == null ? false : discountType;
            discountValue = discountValue ?? 0;

            if (discountType.Value)
            {
                //Chiết khấu được tính theo %
                result = amount.Value - (amount.Value * discountValue.Value) / 100;
            }
            else
            {
                //Chiết khấu được tính theo tiền mặt
                result = amount.Value - discountValue.Value;
            }

            return result;
        }

        public CreateVendorContactResult CreateVendorContact(CreateVendorContactParameter parameter)
        {
            try
            {

                if (parameter.IsUpdate == true)
                {
                    var vendorContact = context.Contact.FirstOrDefault(f => f.ContactId == parameter.VendorContactModel.ContactId
                                                                        && f.ObjectType == "VEN_CON");
                    if (vendorContact != null)
                    {
                        vendorContact.FirstName = parameter.VendorContactModel.FirstName;
                        vendorContact.LastName = parameter.VendorContactModel.LastName;
                        vendorContact.Gender = parameter.VendorContactModel.Gender;
                        vendorContact.Phone = parameter.VendorContactModel.Phone;
                        vendorContact.Email = parameter.VendorContactModel.Email;
                        vendorContact.Role = parameter.VendorContactModel.Role;
                        vendorContact.UpdatedDate = DateTime.Now;
                        vendorContact.UpdatedById = parameter.UserId;

                        context.Contact.Update(vendorContact);
                        context.SaveChanges();
                        return new CreateVendorContactResult()
                        {
                            ContactId = vendorContact.ContactId,
                            StatusCode = HttpStatusCode.OK
                        };
                    }
                }

                var contact = new Contact
                {
                    ContactId = Guid.NewGuid(),
                    ObjectId = parameter.VendorContactModel.ObjectId,
                    ObjectType = "VEN_CON",
                    FirstName = parameter.VendorContactModel.FirstName,
                    LastName = parameter.VendorContactModel.LastName,
                    Gender = parameter.VendorContactModel.Gender,
                    Phone = parameter.VendorContactModel.Phone,
                    Email = parameter.VendorContactModel.Email,
                    Role = parameter.VendorContactModel.Role,
                    Active = true,
                    CreatedDate = DateTime.Now,
                    CreatedById = parameter.UserId
                };

                context.Contact.Add(contact);
                context.SaveChanges();

                return new CreateVendorContactResult()
                {
                    ContactId = contact.ContactId,
                    StatusCode = HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new CreateVendorContactResult()
                {
                    MessageCode = ex.ToString(),
                    StatusCode = HttpStatusCode.ExpectationFailed
                };
            }
        }

        //tính dư nợ cuối kỳ cho mỗi nhà cung cấp
        public decimal? CalculateTotalReceivable(
            Guid vendorId,
            List<Entities.OrderStatus> listOrderStatus,
            List<Vendor> listVendor,
            List<VendorOrder> listVendorOrder,
            List<PayableInvoice> listPayableInvoice,
            List<PayableInvoiceMapping> listPayableInvoiceMapping,
            List<BankPayableInvoice> listBankPayableInvoice,
            List<BankPayableInvoiceMapping> listBankPayableInvoiceMapping,
            DateTime fromDate,
            DateTime toDate)
        {

            //loại những trạng thái đơn hàng sau
            //    1. hoãn:'ON'
            //    2. bị trả lại: 'RTN'
            //    3. hủy: 'CAN'
            //    4. sai: 'WO'
            //    5. nháp: 'DRA'

            var listIgnoreCode = new List<string> { "ON", "RTN", "CAN", "WO", "DRA" };
            var listIgnoreStatusId = listOrderStatus.Where(w => listIgnoreCode.Contains(w.OrderStatusCode)).Select(w => w.OrderStatusId).ToList();

            //trường Amount trong db là tổng trước chiết khấu
            // Danh sách đơn đặt hàng Nhà cung cấp trong kỳ
            var vendorOrderList = (from v in listVendor
                                   join vo in listVendorOrder on v.VendorId equals vo.VendorId
                                   where (vo.CreatedDate.Date >= fromDate)
                                         && (vo.CreatedDate.Date <= toDate)
                                         && vo.VendorId == vendorId
                                          //&& (vo.StatusId == inprogressId || vo.StatusId == paidId || vo.StatusId == deliveredId || vo.StatusId == completedId)
                                          && !listIgnoreStatusId.Contains(vo.StatusId)
                                   select new ReceivableVendorReportEntityModel
                                   {
                                       OrderValue = CalculatorTotalPurchaseProduct(vo.Amount, vo.DiscountType.Value, vo.DiscountValue.Value),
                                       VendorId = vo.VendorId,
                                       CreateDateOrder = vo.CreatedDate,
                                       OrderCode = vo.VendorOrderCode,
                                       CreatedBy = vo.CreatedById,
                                       VendorOrderId = vo.VendorOrderId
                                   }).OrderBy(v => v.CreateDateOrder).ToList();


            #region Lấy dư nợ
            // Nợ phát sinh trong kỳ (Tổng giá trị các đơn đặt hàng Nhà cung cấp trong kỳ)
            var totalValueOrder = vendorOrderList.Sum(v => v.OrderValue);

            // Danh sách phiếu chi trong kỳ
            var payableCashList = (from p in listPayableInvoice
                                   join pom in listPayableInvoiceMapping on p.PayableInvoiceId equals pom.PayableInvoiceId
                                   join v in listVendor on pom.ObjectId equals v.VendorId
                                   where v.VendorId == vendorId
                                         && (p.PaidDate.Date >= fromDate)
                                         && (p.PaidDate.Date <= toDate)
                                   select new ReceivableVendorReportEntityModel
                                   {
                                       CreateDateReceiptInvoice = p.PaidDate,
                                       ReceiptInvoiceValue = p.PayableInvoicePrice * (p.ExchangeRate ?? 1),
                                       DescriptionReceipt = p.PayableInvoiceDetail,
                                       ReceiptCode = p.PayableInvoiceCode,
                                       CreatedBy = p.CreatedById
                                   }).ToList();

            // Danh sách phiếu UNC trong kỳ
            var payableBankList = (from p in listBankPayableInvoice
                                   join pom in listBankPayableInvoiceMapping on p.BankPayableInvoiceId equals pom.BankPayableInvoiceId
                                   join v in listVendor on pom.ObjectId equals v.VendorId
                                   where v.VendorId == vendorId
                                        && (p.BankPayableInvoicePaidDate.Date >= fromDate)
                                        && (p.BankPayableInvoicePaidDate.Date <= toDate)
                                   select new ReceivableVendorReportEntityModel
                                   {
                                       CreateDateReceiptInvoice = p.BankPayableInvoicePaidDate,
                                       ReceiptInvoiceValue = p.BankPayableInvoiceAmount,
                                       DescriptionReceipt = p.BankPayableInvoiceDetail,
                                       ReceiptCode = p.BankPayableInvoiceCode,
                                       CreatedBy = p.CreatedById
                                   }).ToList();

            var totalPayList = new List<ReceivableVendorReportEntityModel>();
            totalPayList.AddRange(payableBankList);
            totalPayList.AddRange(payableCashList);

            // Thanh toán trong kỳ
            var totalValueReceipt = totalPayList.Sum(v => v.ReceiptInvoiceValue);

            // Danh sách đơn đặt hàng kỳ trước
            var vendorOrder = listVendorOrder.Where(v => v.VendorId == vendorId
                                                        && v.CreatedDate.Date < fromDate
                                                        ).ToList();

            // Danh sách phiếu chi kỳ trước
            var payCashBefore = (from p in listPayableInvoice
                                 join pom in listPayableInvoiceMapping on p.PayableInvoiceId equals pom.PayableInvoiceId
                                 where pom.ObjectId == vendorId
                                      && p.PaidDate.Date < fromDate
                                 select new ReceivableVendorReportEntityModel
                                 {
                                     ReceiptInvoiceValue = p.PayableInvoicePrice * (p.ExchangeRate ?? 1)
                                 }).ToList();

            // Danh sách phiếu UNC kỳ trước
            var payBankBefore = (from pb in listBankPayableInvoice
                                 join pbom in listBankPayableInvoiceMapping on pb.BankPayableInvoiceId equals pbom.BankPayableInvoiceId
                                 where pbom.ObjectId == vendorId
                                    && pb.BankPayableInvoicePaidDate.Date < fromDate
                                 select new ReceivableVendorReportEntityModel
                                 {
                                     ReceiptInvoiceValue = pb.BankPayableInvoiceAmount * (pb.BankPayableInvoiceExchangeRate ?? 1)
                                 }).ToList();

            var receiptBefore = new List<ReceivableVendorReportEntityModel>();
            receiptBefore.AddRange(payBankBefore);
            receiptBefore.AddRange(payCashBefore);

            // Tổng giá trị các đơn đặt hàng kỳ trước
            decimal totalValueVendorOrderBefore = 0;
            foreach (var order in vendorOrder)
            {
                totalValueVendorOrderBefore += CalculatorTotalPurchaseProduct(order.Amount, order.DiscountType.Value, order.DiscountValue.Value);
            }

            // Dư nợ đầu kỳ
            var totalReceivableBefore = totalValueVendorOrderBefore - receiptBefore.Sum(r => r.ReceiptInvoiceValue);

            // 
            var totalReceivableInPeriod = totalValueOrder;

            // Dư nợ cuối kỳ
            var totalReceivable = totalValueOrder - totalValueReceipt + totalReceivableBefore;

            return totalReceivable;
            #endregion
        }

        public GetDataCreateVendorOrderResult GetDataCreateVendorOrder(GetDataCreateVendorOrderParameter parameter)
        {
            try
            {
                #region Get payment method and order status

                var paymentTypeId = context.CategoryType.FirstOrDefault(f => f.CategoryTypeCode == "PTO")?.CategoryTypeId;
                var listPaymentMethod = context.Category.Where(w => w.CategoryTypeId == paymentTypeId).ToList();
                var listOrderStatus = context.PurchaseOrderStatus.Where(w => w.Active == true).ToList();

                #endregion

                #region Get Employee Create Order

                var ListEmployeeEntityModel = new List<Models.Employee.EmployeeEntityModel>();

                var listEmployeeEntity = context.Employee
                    .Where(w => w.Active == true).ToList();

                var currentEmployeeId = context.User.FirstOrDefault(f => f.UserId == parameter.UserId)?.EmployeeId;
                var employeeById = listEmployeeEntity.Where(w => w.EmployeeId == currentEmployeeId).FirstOrDefault();

                //check Is Manager
                var isManage = employeeById.IsManager;
                if (isManage == true)
                {
                    //Quản lí: lấy tất cả nhân viên phòng ban đó và phòng ban dưới cấp
                    var currentOrganization = employeeById.OrganizationId;
                    List<Guid?> listOrganizationChildrenId = new List<Guid?>();
                    listOrganizationChildrenId.Add(currentOrganization);
                    var organizationList = context.Organization.Where(w => w.Active == true).ToList();
                    getOrganizationChildrenId(organizationList, currentOrganization, listOrganizationChildrenId);
                    var listEmployeeFiltered = listEmployeeEntity
                        .Where(w => listOrganizationChildrenId.Contains(w.OrganizationId)).Select(w => new
                        {
                            EmployeeId = w.EmployeeId,
                            EmployeeName = w.EmployeeName,
                            EmployeeCode = w.EmployeeCode,
                        }).ToList();

                    listEmployeeFiltered?.ForEach(emp =>
                    {
                        ListEmployeeEntityModel.Add(new Models.Employee.EmployeeEntityModel
                        {
                            EmployeeId = emp.EmployeeId,
                            EmployeeName = emp.EmployeeName,
                            EmployeeCode = emp.EmployeeCode,
                            EmployeeCodeName = emp.EmployeeCode + " - " + emp.EmployeeName
                        });
                    });
                }
                else
                {
                    //Nhân viên: chỉ lấy nhân viên đó
                    var employeeId = listEmployeeEntity.Where(e => e.EmployeeId == currentEmployeeId).FirstOrDefault();
                    ListEmployeeEntityModel.Add(new Models.Employee.EmployeeEntityModel
                    {
                        EmployeeId = employeeId.EmployeeId,
                        EmployeeName = employeeId.EmployeeName,
                        EmployeeCode = employeeId.EmployeeCode,
                        EmployeeCodeName = employeeId.EmployeeCode + " - " + employeeId.EmployeeName
                    });
                }

                #endregion

                #region Lấy danh sách hợp đồng mua (Giang comment)

                //var contractType = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "THD");
                //var listContractStatus = context.Category.Where(x =>
                //    x.CategoryTypeId == contractType.CategoryTypeId && x.CategoryCode != "MOI" &&
                //    x.CategoryCode != "HTH" && x.CategoryCode != "HUY").Select(x => x.CategoryId).ToList();

                //var listContract = context.Contract.Where(x =>
                //        listContractStatus.Count == 0 || listContractStatus.Contains(x.StatusId.Value)).Select(y =>
                //        new ContractEntityModel
                //        {
                //            ContractId = y.ContractId,
                //            ContractCode = y.ContractCode,
                //            ListDetail = new List<ContractDetailEntityModel>()
                //        })
                //    .OrderByDescending(z => z.CreatedDate).ToList();

                //if (listContract.Count > 0)
                //{
                //    var listContractId = listContract.Select(y => y.ContractId).ToList();
                //    var listContractDetail = context.ContractDetail.Where(x => listContractId.Contains(x.ContractId))
                //        .Select(y => new ContractDetailEntityModel
                //        {
                //            ContractDetailId = y.ContractDetailId,
                //            ContractId = y.ContractId,
                //            VendorId = y.VendorId,
                //            ProductId = y.ProductId,
                //            Quantity = y.Quantity,
                //            QuantityOdered = y.QuantityOdered,
                //            UnitPrice = y.UnitPrice,
                //            CurrencyUnit = y.CurrencyUnit,
                //            ExchangeRate = y.ExchangeRate,
                //            Tax = y.Tax,
                //            GuaranteeTime = y.GuaranteeTime,
                //            DiscountType = y.DiscountType,
                //            DiscountValue = y.DiscountValue,
                //            Description = y.Description,
                //            OrderDetailType = y.OrderDetailType,
                //            UnitId = y.UnitId,
                //            IncurredUnit = y.IncurredUnit,
                //            CostsQuoteType = y.CostsQuoteType,
                //            CreatedById = y.CreatedById,
                //            CreatedDate = y.CreatedDate,
                //            ProductName = y.ProductName,
                //        });

                //    listContract.ForEach(item =>
                //    {
                //        item.ListDetail = listContractDetail.Where(x => x.ContractId == item.ContractId).ToList();
                //    });
                //}

                #endregion

                #region Lấy danh sách Kho

                var listWarehouse = context.Warehouse.Where(x => x.Active && x.WarehouseParent == null).Select(y =>
                    new WareHouseEntityModel
                    {
                        WarehouseId = y.WarehouseId,
                        WarehouseCode = y.WarehouseCode,
                        WarehouseName = y.WarehouseCode + " - " + y.WarehouseName
                    }).OrderBy(z => z.WarehouseName).ToList();

                #endregion

                #region List Vendor Create Order Infor

                var listProvinceEntity = context.Province.ToList();
                var listDistrictEntity = context.District.ToList();
                var listWardEntity = context.Ward.ToList();

                var vendorCreateOrderModel = new List<VendorCreateOrderEntityModel>();

                var listVendorEntity = context.Vendor.Where(w => w.Active == true).ToList();
                var listVendorId = listVendorEntity.Select(w => w.VendorId).ToList();
                var listVendorContact =
                    context.Contact.Where(w => listVendorId.Contains(w.ObjectId) && w.Active == true); //list vendor contact + thông tin người liên hệ của vendor
                listVendorEntity?.ForEach(e =>
                {
                    var vendorContact =
                        listVendorContact.FirstOrDefault(f => f.ObjectId == e.VendorId && f.ObjectType == "VEN");

                    var listAddress = new List<string>();
                    if (!string.IsNullOrWhiteSpace(vendorContact.Address))
                    {
                        listAddress.Add(vendorContact.Address);
                    }
                    if (vendorContact.WardId != null)
                    {
                        var _ward = listWardEntity.FirstOrDefault(f => f.WardId == vendorContact.WardId);
                        var _wardText = _ward.WardType + " " + _ward.WardName;
                        listAddress.Add(_wardText);
                    }
                    if (vendorContact.DistrictId != null)
                    {
                        var _district =
                            listDistrictEntity.FirstOrDefault(f => f.DistrictId == vendorContact.DistrictId);
                        var _districtText = _district.DistrictType + " " + _district.DistrictName;
                        listAddress.Add(_districtText);
                    }
                    if (vendorContact.ProvinceId != null)
                    {
                        var _province =
                            listProvinceEntity.FirstOrDefault(f => f.ProvinceId == vendorContact.ProvinceId);
                        var _provincetext = _province.ProvinceType + " " + _province.ProvinceName;
                        listAddress.Add(_provincetext);
                    }

                    var fullAddress = String.Join(", ", listAddress);

                    //var listVendorContact = new List<Models.ContactEntityModel>();
                    var listContactManEntity = listVendorContact.Where(w =>
                        w.Active == true && w.ObjectId == e.VendorId && w.ObjectType == "VEN_CON").ToList();
                    var listContactMan = new List<Models.ContactEntityModel>();

                    listContactManEntity?.ForEach(contact =>
                    {
                        listContactMan.Add(new ContactEntityModel()
                        {
                            ContactId = contact.ContactId,
                            FullName = contact.FirstName + " " + contact.LastName ?? "",
                            Email = contact.Email ?? "",
                            Phone = contact.Phone ?? ""
                        });
                    });

                    vendorCreateOrderModel.Add(new VendorCreateOrderEntityModel()
                    {
                        VendorId = e.VendorId,
                        VendorCode = e.VendorCode?.Trim(),
                        VendorName = e.VendorName ?? "",
                        VendorEmail = vendorContact?.Email ?? "",
                        VendorPhone = vendorContact?.Phone ?? "",
                        PaymentId = e.PaymentId,
                        FullAddressVendor = fullAddress,
                        ListVendorContact = listContactMan
                    });
                });

                #endregion

                #region get bank payment

                var listvendorId = vendorCreateOrderModel.Select(w => w.VendorId).ToList();
                var listBankAccount = context.BankAccount.Where(w => listvendorId.Contains(w.ObjectId)).ToList();

                #endregion

                #region get procurement request

                // Lấy các trạng thái đề xuất mua hàng
                var categoryTypeObj =
                    context.CategoryType.FirstOrDefault(ct => ct.Active == true && ct.CategoryTypeCode == "DDU");
                List<string> strStatus = new List<string> { "Approved" };
                var categoryObj = context.Category
                    .Where(c => c.Active == true && c.CategoryTypeId == categoryTypeObj.CategoryTypeId &&
                                strStatus.Contains(c.CategoryCode)).Select(c => c.CategoryId).ToList();

                var listProcurementItem = context.ProcurementRequestItem.ToList();
                var listProcurementRequest = context.ProcurementRequest.Where(p =>
                        p.StatusId != null && p.StatusId != Guid.Empty && categoryObj.Contains(p.StatusId.Value))
                    .Select(p => new ProcurementRequestEntityModel
                    {
                        ProcurementRequestId = p.ProcurementRequestId,
                        ProcurementCode = p.ProcurementCode,
                        ProcurementContent = p.ProcurementContent,
                        ApproverId = p.ApproverId,
                        ApproverName = "",
                        ApproverPostion = p.ApproverPostion,
                        CreatedById = p.CreatedById,
                        CreatedDate = p.CreatedDate,
                        EmployeePhone = p.EmployeePhone,
                        Explain = p.Explain,
                        Unit = p.Unit,
                        RequestEmployeeId = p.RequestEmployeeId,
                        UpdatedById = p.UpdatedById,
                        UpdatedDate = p.UpdatedDate,
                        StatusId = p.StatusId,
                        OrderId = p.OrderId,
                        TextShow = "",
                    }).ToList();

                var listProduct = context.Product.ToList();
                var listVendor = context.Vendor.ToList();
                var listCategory = context.Category.ToList();

                //Danh sách đơn vị tiền
                var currencyType = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "DTI");
                var listCurrencyUnit = context.Category.Where(x =>
                    x.Active == true && x.CategoryTypeId == currencyType.CategoryTypeId);

                //Danh sách Đơn hàng mua có trạng thái Đơn hàng mua và Đóng
                var listRequestId = listProcurementRequest.Select(y => y.ProcurementRequestId).ToList();
                var listRequestItemId = context.ProcurementRequestItem
                    .Where(x => listRequestId.Contains(x.ProcurementRequestId.Value))
                    .Select(y => y.ProcurementRequestItemId).ToList();
                var _listStatusCode = new List<string> { "PURC", "COMP" };
                var _listStatusId = context.PurchaseOrderStatus
                    .Where(x => _listStatusCode.Contains(x.PurchaseOrderStatusCode))
                    .Select(y => y.PurchaseOrderStatusId).ToList();
                var _listVendorOrderId = context.VendorOrder.Where(x => _listStatusId.Contains(x.StatusId))
                    .Select(y => y.VendorOrderId).ToList();

                //Lấy các item có trong list đơn hàng mua trên
                var listRequestItemHasUsing = context.VendorOrderDetail.Where(x =>
                        _listVendorOrderId.Contains(x.VendorOrderId) &&
                        listRequestItemId.Contains(x.ProcurementRequestItemId.Value))
                    .GroupBy(x => new
                    {
                        x.ProcurementRequestItemId
                    })
                    .Select(y => new RequestItem
                    {
                        ProcurementRequestItemId = y.Key.ProcurementRequestItemId,
                        Quantity = y.Sum(s => s.Quantity)
                    }).ToList();


                listProcurementRequest.ForEach(item =>
                {
                    if (item.ApproverId != null && item.ApproverId != Guid.Empty)
                    {
                        var _approver = listEmployeeEntity.FirstOrDefault(x => x.EmployeeId == item.ApproverId);
                        if (_approver != null)
                        {
                            item.ApproverName = _approver.EmployeeName;
                        }
                    }

                    item.TextShow = item.ProcurementCode + " - " + (item.Explain == null ? "" : item.Explain.Trim()) +
                                    " - " + item.ApproverName;

                    item.ListDetail = listProcurementItem
                        .Where(d => d.ProcurementRequestId == item.ProcurementRequestId)
                        .Select(d => new ProcurementRequestItemEntityModel
                        {
                            ProcurementRequestItemId = d.ProcurementRequestItemId,
                            ProcurementRequestId = d.ProcurementRequestId,
                            ProcurementPlanId = d.ProcurementPlanId,
                            CreatedById = d.CreatedById,
                            CreatedDate = d.CreatedDate,
                            ProductId = d.ProductId,
                            Quantity = d.Quantity,
                            UnitPrice = d.UnitPrice,
                            UpdatedById = d.UpdatedById,
                            UpdatedDate = d.UpdatedDate,
                            VendorName = listVendor.FirstOrDefault(v => v.VendorId == d.VendorId) != null
                                ? listVendor.FirstOrDefault(v => v.VendorId == d.VendorId).VendorName
                                : "",
                            UnitName = "",
                            Unit = Guid.Empty,
                            CurrencyUnit = d.CurrencyUnit,
                            CurrencyUnitName = listCurrencyUnit
                                .FirstOrDefault(x => x.CategoryId == d.CurrencyUnit.Value)?.CategoryName,
                            ProductCode = listProduct.FirstOrDefault(p => p.ProductId == d.ProductId) != null
                                ? listProduct.FirstOrDefault(p => p.ProductId == d.ProductId).ProductCode
                                : "",
                            ProductName = listProduct.FirstOrDefault(p => p.ProductId == d.ProductId) != null
                                ? listProduct.FirstOrDefault(p => p.ProductId == d.ProductId).ProductName
                                : "",
                            VendorId = d.VendorId,
                            ExchangeRate = d.ExchangeRate,
                            ProcurementCode = item.ProcurementCode,
                            QuantityApproval = d.QuantityApproval,
                            OrderDetailType = d.OrderDetailType,
                            WarehouseId = d.WarehouseId,
                            Description = d.Description,
                            IncurredUnit = d.IncurredUnit,
                        }).ToList();

                    item.ListDetail.ForEach(detail =>
                    {
                        var productObj = listProduct.FirstOrDefault(p => p.ProductId == detail.ProductId);
                        if (productObj != null)
                        {
                            var unitObj = listCategory.FirstOrDefault(c =>
                                productObj.ProductUnitId != null && productObj.ProductUnitId != Guid.Empty &&
                                c.CategoryId == productObj.ProductUnitId);
                            detail.UnitName = unitObj != null ? unitObj.CategoryName : "";
                            detail.Unit = unitObj != null ? unitObj.CategoryId : Guid.Empty;

                            detail.ProductUnitId = detail.Unit;
                            detail.ProductUnit = detail.UnitName;
                        }

                        if (detail.ProductId == null)
                        {
                            detail.ProductName = detail.Description;
                            detail.UnitName = detail.IncurredUnit;
                        }

                        //Tính lại số lượng còn lại thực tế của item trong phiếu đề xuất
                        var usingItem = listRequestItemHasUsing
                            .FirstOrDefault(x => x.ProcurementRequestItemId == detail.ProcurementRequestItemId);
                        decimal? usingQuantity = 0;
                        if (usingItem != null)
                        {
                            usingQuantity = usingItem.Quantity ?? 0;
                        }

                        var remainQuantity = detail.QuantityApproval - usingQuantity;
                        detail.QuantityApproval = remainQuantity;
                    });

                    item.ListDetail = item.ListDetail.Where(x => x.QuantityApproval != 0).ToList();
                });

                #endregion

                #region Lấy danh sách nhóm Nhà cung cấp

                var vendorGroupType = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "NCA");
                var listVendorGroup = context.Category
                    .Where(x => x.CategoryTypeId == vendorGroupType.CategoryTypeId && x.Active == true)
                    .Select(y => new CategoryEntityModel
                    {
                        CategoryId = y.CategoryId,
                        CategoryName = y.CategoryName
                    }).ToList();

                #endregion

                var listPaymentMethodResult = new List<CategoryEntityModel>();
                var _listPaymentMethod = listPaymentMethod.OrderBy(w => w.CategoryName).ToList();
                _listPaymentMethod.ForEach(item =>
                {
                    var _item = new CategoryEntityModel(item);
                    listPaymentMethodResult.Add(_item);
                });

                var listOrderStatusResult = new List<PurchaseOrderStatusEntityModel>();
                var _listOrderStatus = listOrderStatus.OrderBy(w => w.Description).ToList();
                _listOrderStatus.ForEach(item =>
                {
                    var _item = new PurchaseOrderStatusEntityModel(item);
                    listOrderStatusResult.Add(_item);
                });

                var listBankAccountResult = new List<BankAccountEntityModel>();
                var _listBankAccount = listBankAccount.OrderBy(w => w.AccountName).ToList();
                _listBankAccount.ForEach(item =>
                {
                    var _item = new BankAccountEntityModel(item);
                    listBankAccountResult.Add(_item);
                });


                return new GetDataCreateVendorOrderResult()
                {
                    ListPaymentMethod = listPaymentMethodResult,
                    ListOrderStatus = listOrderStatusResult,
                    ListEmployeeModel = ListEmployeeEntityModel.OrderBy(w => w.EmployeeName).ToList(),
                    VendorCreateOrderModel = vendorCreateOrderModel.OrderBy(w => w.VendorName).ToList(),
                    ListBankAccount = listBankAccountResult,
                    ListProcurementRequest = listProcurementRequest,
                    //ListContract = listContract,
                    ListWareHouse = listWarehouse,
                    ListVendorGroup = listVendorGroup,
                    StatusCode = HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new GetDataCreateVendorOrderResult()
                {
                    MessageCode = ex.ToString(),
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

        public GetDataAddVendorOrderDetailResult GetDataAddVendorOrderDetail(GetDataAddVendorOrderDetailParameter parameter)
        {
            try
            {
                // common
                var listProductVendorMap = context.ProductVendorMapping.Where(x => x.Active).ToList();

                #region Get Category

                var moneyTypeId = context.CategoryType.FirstOrDefault(f => f.CategoryTypeCode == "DTI")?.CategoryTypeId;
                var listMoneyUnit = context.Category.Where(w => w.CategoryTypeId == moneyTypeId && w.Active == true)
                    .Select(y => new CategoryEntityModel
                    {
                        CategoryId = y.CategoryId,
                        CategoryCode = y.CategoryCode,
                        CategoryName = y.CategoryName,
                        IsDefault = y.IsDefauld
                    }).ToList();
                var productUnitTypeId = context.CategoryType.FirstOrDefault(f => f.CategoryTypeCode == "DNH")
                    ?.CategoryTypeId;
                var listProductUnit = context.Category.Where(w => w.CategoryTypeId == productUnitTypeId).ToList();

                #endregion

                // lấy list loại hình kinh doanh: Chỉ bán ra, chỉ mua vào và cả 2.
                var loaiHinhTypeId = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "HHKD")?.CategoryTypeId;
                var listLoaiHinh = context.Category.Where(x => x.CategoryTypeId == loaiHinhTypeId).Select(c => new CategoryEntityModel()
                {
                    CategoryTypeId = c.CategoryTypeId,
                    CategoryId = c.CategoryId,
                    CategoryName = c.CategoryName,
                    CategoryCode = c.CategoryCode,
                }).ToList();

                #region Get Product

                var listProduct = new List<ProductEntityModel>();

                //var listProductMappingId =
                //    context.ProductVendorMapping.Select(w => w.ProductId).ToList() ?? new List<Guid>();

                var listProductEntity = context.Product.Where(w => w.Active == true).ToList();

                listProductEntity?.ForEach(product =>
                {
                    // Get Product Attribute Category and Product Attribute Value
                    var listProductAttributeCategoryId =
                        context.ProductAttribute.Where(w => w.ProductId == product.ProductId)
                            .Select(w => w.ProductAttributeCategoryId).ToList() ?? new List<Guid>();

                    //tên thuộc tính
                    var listProductAttributeCategory =
                        new List<Models.ProductAttributeCategory.ProductAttributeCategoryEntityModel>();
                    var listProductAttributeCategoryEntity = context.ProductAttributeCategory.Where(w =>
                            w.Active == true && listProductAttributeCategoryId.Contains(w.ProductAttributeCategoryId))
                        .ToList();

                    listProductAttributeCategoryEntity.ForEach(attributeCategory =>
                    {
                        //giá trị thuộc tính
                        var listProductAttributeCategoryValueEntityModel =
                            new List<Models.ProductAttributeCategoryValue.ProductAttributeCategoryValueEntityModel>();
                        var listProductAttributeCategoryValueEntity = context.ProductAttributeCategoryValue.Where(w =>
                            w.Active == true && w.ProductAttributeCategoryId ==
                            attributeCategory.ProductAttributeCategoryId).ToList();
                        listProductAttributeCategoryValueEntity.ForEach(value =>
                        {
                            listProductAttributeCategoryValueEntityModel.Add(
                                new Models.ProductAttributeCategoryValue.ProductAttributeCategoryValueEntityModel
                                {
                                    ProductAttributeCategoryId = value.ProductAttributeCategoryId,
                                    ProductAttributeCategoryValue1 = value.ProductAttributeCategoryValue1,
                                    ProductAttributeCategoryValueId = value.ProductAttributeCategoryValueId
                                });
                        });

                        listProductAttributeCategory.Add(
                            new Models.ProductAttributeCategory.ProductAttributeCategoryEntityModel
                            {
                                ProductAttributeCategoryId = attributeCategory.ProductAttributeCategoryId,
                                ProductAttributeCategoryName = attributeCategory.ProductAttributeCategoryName,
                                ProductAttributeCategoryValue = listProductAttributeCategoryValueEntityModel,
                            });
                    });

                    listProduct.Add(new ProductEntityModel
                    {
                        ProductId = product.ProductId,
                        ProductName = product.ProductName,
                        ProductCode = product.ProductCode,
                        Price1 = product.Price1,
                        ProductMoneyUnitId = product.ProductMoneyUnitId,
                        ListProductAttributeCategory = listProductAttributeCategory,
                        ProductUnitId = product.ProductUnitId,
                        FolowInventory = product.FolowInventory,
                        LoaiKinhDoanhCode = listLoaiHinh.FirstOrDefault(x => x.CategoryId == product.LoaiKinhDoanh)?.CategoryCode,
                        ProductUnitName = listProductUnit.FirstOrDefault(f => f.CategoryId == product.ProductUnitId)?.CategoryName ?? ""
                    });
                });

                listProduct = listProduct.Where(x => x.LoaiKinhDoanhCode == "BUYONLY" || x.LoaiKinhDoanhCode == "SALEANDBUY" || x.LoaiKinhDoanhCode == null).ToList();

                listProduct.ForEach(item =>
                {
                    item.FixedPrice = listProductVendorMap.FirstOrDefault(x =>
                        x.ProductId == item.ProductId && x.VendorId == parameter.VendorId &&
                        ((x.FromDate != null && x.ToDate != null && x.FromDate.Value.Date <= DateTime.Now &&
                          DateTime.Now <= x.ToDate.Value.Date) || (x.FromDate != null && x.ToDate == null &&
                                                                   x.FromDate.Value.Date <= DateTime.Now)))?.Price ?? 0;

                    item.MinimumInventoryQuantity = listProductVendorMap.FirstOrDefault(x =>
                        x.ProductId == item.ProductId && x.VendorId == parameter.VendorId &&
                        ((x.FromDate != null && x.ToDate != null && x.FromDate.Value.Date <= DateTime.Now &&
                          DateTime.Now <= x.ToDate.Value.Date) || (x.FromDate != null && x.ToDate == null &&
                                                                   x.FromDate.Value.Date <= DateTime.Now)))?.MiniumQuantity ?? 0;
                });

                #endregion

                #region Lấy danh sách kho
                var listWareHouse = context.Warehouse.Where(v => v.Active == true)
                    .Select(v => new WareHouseEntityModel
                    {
                        WarehouseId = v.WarehouseId,
                        WarehouseCode = v.WarehouseCode,
                        WarehouseName = v.WarehouseName,
                        WarehouseParent = v.WarehouseParent,
                        WarehouseAddress = v.WarehouseAddress,
                        WarehousePhone = v.WarehousePhone,
                        Storagekeeper = v.Storagekeeper,
                        WarehouseDescription = v.WarehouseDescription,
                        Active = v.Active,
                        WarehouseCodeName = v.WarehouseCode + " - " + v.WarehouseName,
                    }).OrderBy(c => c.WarehouseName).ToList();
                #endregion

                return new GetDataAddVendorOrderDetailResult()
                {
                    ListMoneyUnit = listMoneyUnit ?? new List<CategoryEntityModel>(),
                    ListProductByVendorId = listProduct,
                    ListWarehouse = listWareHouse,
                    StatusCode = HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new GetDataAddVendorOrderDetailResult()
                {
                    MessageCode = ex.ToString(),
                    StatusCode = HttpStatusCode.ExpectationFailed
                };
            }

        }

        public GetDataEditVendorOrderResult GetDataEditVendorOrder(GetDataEditVendorOrderParameter parameter)
        {
            try
            {
                #region Get payment method and order status

                var paymentTypeId = context.CategoryType.FirstOrDefault(f => f.CategoryTypeCode == "PTO")
                    ?.CategoryTypeId;
                var listPaymentMethod = context.Category.Where(w => w.CategoryTypeId == paymentTypeId).ToList();
                var listOrderStatus = context.PurchaseOrderStatus.Where(w => w.Active == true).ToList();
                var listInventoryReport = context.InventoryReport.ToList();

                #endregion

                #region Get Employee Create Order

                var ListEmployeeEntityModel = new List<EmployeeEntityModel>();
                var portalUserCode = "PortalUser"; //loại portalUser
                var listEmployeeEntity = context.Employee
                    .Where(w => w.Active == true && w.EmployeeCode != portalUserCode).ToList(); //loai portal user

                var currentEmployeeId = context.User.FirstOrDefault(f => f.UserId == parameter.UserId)?.EmployeeId;
                var employeeById = listEmployeeEntity.Where(w => w.EmployeeId == currentEmployeeId).FirstOrDefault();

                //check Is Manager
                var isManage = employeeById.IsManager;
                if (isManage == true)
                {
                    //Quản lí: lấy tất cả nhân viên phòng ban đó và phòng ban dưới cấp
                    var currentOrganization = employeeById.OrganizationId;
                    List<Guid?> listOrganizationChildrenId = new List<Guid?>();
                    listOrganizationChildrenId.Add(currentOrganization);
                    var organizationList = context.Organization.Where(w => w.Active == true).ToList();
                    getOrganizationChildrenId(organizationList, currentOrganization, listOrganizationChildrenId);
                    var listEmployeeFiltered = listEmployeeEntity
                        .Where(w => listOrganizationChildrenId.Contains(w.OrganizationId)).Select(w => new
                        {
                            EmployeeId = w.EmployeeId,
                            EmployeeName = w.EmployeeName,
                            EmployeeCode = w.EmployeeCode,
                        }).ToList();

                    listEmployeeFiltered?.ForEach(emp =>
                    {
                        ListEmployeeEntityModel.Add(new EmployeeEntityModel
                        {
                            EmployeeId = emp.EmployeeId,
                            EmployeeName = emp.EmployeeName,
                            EmployeeCode = emp.EmployeeCode,
                            EmployeeCodeName = emp.EmployeeCode + " - " + emp.EmployeeName
                        });
                    });
                }
                else
                {
                    //Nhân viên: chỉ lấy nhân viên đó
                    var employeeId = listEmployeeEntity.Where(e => e.EmployeeId == currentEmployeeId).FirstOrDefault();
                    ListEmployeeEntityModel.Add(new EmployeeEntityModel
                    {
                        EmployeeId = employeeId.EmployeeId,
                        EmployeeName = employeeId.EmployeeName,
                        EmployeeCode = employeeId.EmployeeCode,
                        EmployeeCodeName = employeeId.EmployeeCode + " - " + employeeId.EmployeeName
                    });
                }

                #endregion

                #region List Vendor Create Order Infor

                var listProvinceEntity = context.Province.ToList();
                var listDistrictEntity = context.District.ToList();
                var listWardEntity = context.Ward.ToList();

                var vendorCreateOrderModel = new List<VendorCreateOrderEntityModel>();

                var listVendorEntity = context.Vendor.Where(w => w.Active == true).ToList();
                var listVendorId = listVendorEntity.Select(w => w.VendorId).ToList() ?? new List<Guid>();
                var listVendorContact =
                    context.Contact.Where(w => listVendorId.Contains(w.ObjectId) && w.Active == true); //list vendor contact + thông tin người liên hệ của vendor

                listVendorEntity?.ForEach(e =>
                {
                    var vendorContact =
                        listVendorContact.FirstOrDefault(f => f.ObjectId == e.VendorId && f.ObjectType == "VEN");

                    var listAddress = new List<string>();
                    if (!string.IsNullOrWhiteSpace(vendorContact.Address))
                    {
                        listAddress.Add(vendorContact.Address);
                    }

                    if (vendorContact.WardId != null)
                    {
                        var _ward = listWardEntity.FirstOrDefault(f => f.WardId == vendorContact.WardId);
                        var _wardText = _ward.WardType + " " + _ward.WardName;
                        listAddress.Add(_wardText);
                    }

                    if (vendorContact.DistrictId != null)
                    {
                        var _district =
                            listDistrictEntity.FirstOrDefault(f => f.DistrictId == vendorContact.DistrictId);
                        var _districtText = _district.DistrictType + " " + _district.DistrictName;
                        listAddress.Add(_districtText);
                    }

                    if (vendorContact.ProvinceId != null)
                    {
                        var _province =
                            listProvinceEntity.FirstOrDefault(f => f.ProvinceId == vendorContact.ProvinceId);
                        var _provincetext = _province.ProvinceType + " " + _province.ProvinceName;
                        listAddress.Add(_provincetext);
                    }

                    var fullAddress = String.Join(", ", listAddress);

                    var listContactManEntity = listVendorContact.Where(w =>
                        w.Active == true && w.ObjectId == e.VendorId && w.ObjectType == "VEN_CON").ToList();
                    var listContactMan = new List<Models.ContactEntityModel>();

                    listContactManEntity?.ForEach(contact =>
                    {
                        listContactMan.Add(new ContactEntityModel()
                        {
                            ContactId = contact.ContactId,
                            FullName = contact.FirstName + " " + contact.LastName ?? "",
                            Email = contact.Email ?? "",
                            Phone = contact.Phone ?? ""
                        });
                    });

                    vendorCreateOrderModel.Add(new VendorCreateOrderEntityModel()
                    {
                        VendorId = e.VendorId,
                        VendorName = e.VendorName ?? "",
                        VendorEmail = vendorContact?.Email ?? "",
                        VendorPhone = vendorContact?.Phone ?? "",
                        PaymentId = e.PaymentId,
                        VendorCode = e.VendorCode ?? "",
                        FullAddressVendor = fullAddress,
                        ListVendorContact = listContactMan
                    });
                });

                #endregion

                #region get bank payment

                var listvendorId = vendorCreateOrderModel.Select(w => w.VendorId).ToList();
                var listBankAccount = context.BankAccount.Where(w => listvendorId.Contains(w.ObjectId)).ToList();

                #endregion

                #region Get vendor order by id

                var vendorOrderEntity =
                    context.VendorOrder.FirstOrDefault(f => f.VendorOrderId == parameter.VendorOrderId);

                var vendorOrderById = new VendorOrderEntityModel();
                vendorOrderById.VendorOrderId = vendorOrderEntity.VendorOrderId;
                vendorOrderById.VendorOrderCode = vendorOrderEntity.VendorOrderCode;
                vendorOrderById.VendorOrderDate = vendorOrderEntity.VendorOrderDate;
                vendorOrderById.CustomerOrderId = vendorOrderEntity.CustomerOrderId;
                vendorOrderById.Orderer = vendorOrderEntity.Orderer;
                vendorOrderById.Description = vendorOrderEntity.Description;
                vendorOrderById.Note = vendorOrderEntity.Note;
                vendorOrderById.VendorId = vendorOrderEntity.VendorId;
                vendorOrderById.VendorContactId = vendorOrderEntity.VendorContactId;
                vendorOrderById.PaymentMethod = vendorOrderEntity.PaymentMethod;
                vendorOrderById.BankAccountId = vendorOrderEntity.BankAccountId;
                vendorOrderById.ReceivedDate = vendorOrderEntity.ReceivedDate;
                vendorOrderById.ReceivedHour = vendorOrderEntity.ReceivedHour;
                vendorOrderById.RecipientName = vendorOrderEntity.RecipientName;
                vendorOrderById.LocationOfShipment = vendorOrderEntity.LocationOfShipment;
                vendorOrderById.ShippingNote = vendorOrderEntity.ShippingNote;
                vendorOrderById.RecipientPhone = vendorOrderEntity.RecipientPhone;
                vendorOrderById.RecipientEmail = vendorOrderEntity.RecipientEmail;
                vendorOrderById.PlaceOfDelivery = vendorOrderEntity.PlaceOfDelivery;
                vendorOrderById.Amount = vendorOrderEntity.Amount;
                vendorOrderById.DiscountValue = vendorOrderEntity.DiscountValue;
                vendorOrderById.StatusId = vendorOrderEntity.StatusId;
                vendorOrderById.DiscountType = vendorOrderEntity.DiscountType;
                vendorOrderById.WarehouseId = vendorOrderEntity.WarehouseId;
                vendorOrderById.ShipperName = vendorOrderEntity.ShipperName;
                vendorOrderById.TypeCost = vendorOrderEntity.TypeCost;
                vendorOrderById.UpdatedById = vendorOrderEntity.UpdatedById;
                vendorOrderById.UpdatedDate = vendorOrderEntity.UpdatedDate;

                var listVendorOrderDetailById = new List<VendorOrderDetailEntityModel>();

                var vendorOrderDetailEntity = context.VendorOrderDetail
                    .Where(f => f.VendorOrderId == parameter.VendorOrderId).OrderBy(x => x.OrderNumber).ToList();
                var listVendorOrderDetailId = vendorOrderDetailEntity.Select(w => w.VendorOrderDetailId).ToList();
                var listVendorOrderProductDetailProductAttributeValueEntity = context
                    .VendorOrderProductDetailProductAttributeValue
                    .Where(w => listVendorOrderDetailId.Contains(w.VendorOrderDetailId)).ToList();

                var listProductId = vendorOrderDetailEntity.Select(w => w.ProductId).ToList();
                var listProductEntity = context.Product.Where(w => listProductId.Contains(w.ProductId)).ToList();

                var unitTypeId = context.CategoryType.FirstOrDefault(f => f.CategoryTypeCode == "DNH").CategoryTypeId;
                var listUnitType = context.Category.Where(w => w.CategoryTypeId == unitTypeId).ToList();
                var currencyTypeId = context.CategoryType.FirstOrDefault(f => f.CategoryTypeCode == "DTI")
                    .CategoryTypeId;
                var listCurrencyType = context.Category.Where(w => w.CategoryTypeId == currencyTypeId).ToList();

                var listProductCategoryId = listVendorOrderProductDetailProductAttributeValueEntity
                    .Select(w => w.ProductAttributeCategoryId).ToList();
                var listProductAttributeId = listVendorOrderProductDetailProductAttributeValueEntity
                    .Select(w => w.ProductAttributeCategoryValueId).ToList();

                var listProductCategory = context.ProductAttributeCategory
                    .Where(w => listProductCategoryId.Contains(w.ProductAttributeCategoryId)).ToList();
                var listProductAttribute = context.ProductAttributeCategoryValue
                    .Where(w => listProductAttributeId.Contains(w.ProductAttributeCategoryValueId)).ToList();

                var listOptionAttribute = context.ProductAttributeCategoryValue
                    .Where(w => listProductCategoryId.Contains(w.ProductAttributeCategoryId)).ToList();

                var listAllProcurementRequest = context.ProcurementRequest.ToList();

                vendorOrderDetailEntity?.ForEach(orderdetail =>
                {
                    //Lấy dan sách chi tiết thuộc tính sản phẩm, nếu có
                    var listAttributeEntity = listVendorOrderProductDetailProductAttributeValueEntity
                        .Where(w => w.VendorOrderDetailId == orderdetail.VendorOrderDetailId).ToList();
                    var listAttribute =
                        new List<VendorOrderProductDetailProductAttributeValueEntityModel>();

                    listAttributeEntity?.ForEach(attri =>
                    {
                        var listProductAttributeEntity = listOptionAttribute
                            .Where(w => w.ProductAttributeCategoryId == attri.ProductAttributeCategoryId).ToList();

                        var productAttributeCategoryValue =
                            new List<Models.ProductAttributeCategoryValue.ProductAttributeCategoryValueEntityModel>();

                        listProductAttributeEntity.ForEach(e =>
                        {
                            productAttributeCategoryValue.Add(
                                new Models.ProductAttributeCategoryValue.ProductAttributeCategoryValueEntityModel
                                {
                                    ProductAttributeCategoryId = e.ProductAttributeCategoryId,
                                    ProductAttributeCategoryValueId = e.ProductAttributeCategoryValueId,
                                    ProductAttributeCategoryValue1 = e.ProductAttributeCategoryValue1
                                });
                        });

                        listAttribute.Add(new VendorOrderProductDetailProductAttributeValueEntityModel
                        {
                            OrderProductDetailProductAttributeValueId = attri.OrderProductDetailProductAttributeValueId,
                            VendorOrderDetailId = attri.VendorOrderDetailId,
                            ProductId = attri.ProductId,
                            ProductAttributeCategoryId = attri.ProductAttributeCategoryId,
                            ProductAttributeCategoryName =
                                listProductCategory
                                    .FirstOrDefault(f =>
                                        f.ProductAttributeCategoryId == attri.ProductAttributeCategoryId)
                                    ?.ProductAttributeCategoryName ?? "",
                            ProductAttributeCategoryValueId = attri.ProductAttributeCategoryValueId,
                            ProductAttributeCategoryValue = productAttributeCategoryValue
                        });
                    });

                    var listSelectedAttributeName = new List<string>();

                    listAttribute?.ForEach(e =>
                    {
                        var category = listProductCategory.FirstOrDefault(f =>
                            f.ProductAttributeCategoryId == e.ProductAttributeCategoryId);
                        var attribute = listProductAttribute.FirstOrDefault(f =>
                            f.ProductAttributeCategoryValueId == e.ProductAttributeCategoryValueId);
                        var categoryName = category?.ProductAttributeCategoryName ?? "";
                        var attributeName = attribute?.ProductAttributeCategoryValue1 ?? "";
                        var result = categoryName + ":" + attributeName;
                        listSelectedAttributeName.Add(result);
                    });

                    var selectedAttributeName = String.Join(", ", listSelectedAttributeName) ?? "";
                    var unitId = listProductEntity.FirstOrDefault(f => f.ProductId == orderdetail.ProductId)
                        ?.ProductUnitId;
                    var unitName = listUnitType.FirstOrDefault(f => f.CategoryId == unitId)?.CategoryName ?? "";

                    listVendorOrderDetailById.Add(new VendorOrderDetailEntityModel
                    {
                        VendorOrderDetailId = orderdetail.VendorOrderDetailId,
                        VendorId = orderdetail.VendorId,
                        VendorOrderId = orderdetail.VendorOrderId,
                        ProductId = orderdetail.ProductId,
                        Quantity = orderdetail.Quantity ?? 0,
                        UnitPrice = orderdetail.UnitPrice ?? 0,
                        CurrencyUnit = orderdetail.CurrencyUnit,
                        ExchangeRate = orderdetail.ExchangeRate ?? 1,
                        Vat = orderdetail.Vat ?? 0,
                        DiscountType = orderdetail.DiscountType ?? true,
                        DiscountValue = orderdetail.DiscountValue ?? 0,
                        UnitId = unitId,
                        Description = orderdetail.Description,
                        OrderDetailType = orderdetail.OrderDetailType,
                        IncurredUnit = orderdetail.IncurredUnit,
                        VendorOrderProductDetailProductAttributeValue = listAttribute,
                        //get name
                        ProductName = listProductEntity.FirstOrDefault(f => f.ProductId == orderdetail.ProductId)?.ProductName ?? "",
                        FolowInventory = listProductEntity.FirstOrDefault(f => f.ProductId == orderdetail.ProductId)?.FolowInventory ?? false,
                        VendorName = vendorCreateOrderModel.FirstOrDefault(f => f.VendorId == orderdetail.VendorId)
                                         ?.VendorName ?? "",
                        UnitName = unitName,
                        CurrencyUnitName =
                            listCurrencyType.FirstOrDefault(f => f.CategoryId == orderdetail.CurrencyUnit)
                                ?.CategoryName ?? "",
                        SelectedAttributeName = selectedAttributeName,
                        Cost = orderdetail.Cost,
                        PriceWarehouse = orderdetail.PriceWarehouse,
                        PriceValueWarehouse = orderdetail.PriceValueWarehouse,
                        ProcurementRequestId = orderdetail.ProcurementRequestId,
                        ProcurementCode = listAllProcurementRequest
                                              .FirstOrDefault(x =>
                                                  x.ProcurementRequestId == orderdetail.ProcurementRequestId)
                                              ?.ProcurementCode ?? "",
                        ProcurementRequestItemId = orderdetail.ProcurementRequestItemId,
                        IsEditCost = orderdetail.IsEditCost,
                        OrderNumber = orderdetail.OrderNumber,
                        WarehouseId = orderdetail.WarehouseId
                    });
                });

                #endregion

                #region Lấy danh sách Kho

                var listWarehouse = context.Warehouse.Where(x => x.Active).Select(y =>
                    new WareHouseEntityModel
                    {
                        WarehouseId = y.WarehouseId,
                        WarehouseCode = y.WarehouseCode,
                        WarehouseName = y.WarehouseCode + " - " + y.WarehouseName
                    }).OrderBy(z => z.WarehouseName).ToList();

                #endregion

                #region get procurement request

                // Lấy các trạng thái đề xuất mua hàng
                var categoryTypeObj =
                    context.CategoryType.FirstOrDefault(ct => ct.Active == true && ct.CategoryTypeCode == "DDU");
                List<string> strStatus = new List<string> { "Approved", "Close" };
                var categoryObj = context.Category
                    .Where(c => c.Active == true && c.CategoryTypeId == categoryTypeObj.CategoryTypeId &&
                                strStatus.Contains(c.CategoryCode)).Select(c => c.CategoryId).ToList();

                var listProcurementItem = context.ProcurementRequestItem.ToList();
                var listProcurementRequest = context.ProcurementRequest.Where(p =>
                        p.StatusId != null && p.StatusId != Guid.Empty && categoryObj.Contains(p.StatusId.Value))
                    .Select(p => new ProcurementRequestEntityModel
                    {
                        ProcurementRequestId = p.ProcurementRequestId,
                        ProcurementCode = p.ProcurementCode,
                        ProcurementContent = p.ProcurementContent,
                        ApproverId = p.ApproverId,
                        ApproverName = "",
                        ApproverPostion = p.ApproverPostion,
                        CreatedById = p.CreatedById,
                        CreatedDate = p.CreatedDate,
                        EmployeePhone = p.EmployeePhone,
                        Explain = p.Explain,
                        Unit = p.Unit,
                        RequestEmployeeId = p.RequestEmployeeId,
                        UpdatedById = p.UpdatedById,
                        UpdatedDate = p.UpdatedDate,
                        StatusId = p.StatusId,
                        OrderId = p.OrderId,
                        TextShow = ""
                    }).ToList();

                var listProduct = context.Product.ToList();
                var listVendor = context.Vendor.ToList();
                var listCategory = context.Category.ToList();

                //Danh sách đơn vị tiền
                var currencyType = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "DTI");
                var listCurrencyUnit = context.Category.Where(x =>
                    x.Active == true && x.CategoryTypeId == currencyType.CategoryTypeId);

                var listRequestId = listProcurementRequest.Select(y => y.ProcurementRequestId).ToList();
                var listRequestItemId = context.ProcurementRequestItem
                    .Where(x => listRequestId.Contains(x.ProcurementRequestId.Value))
                    .Select(y => y.ProcurementRequestItemId).ToList();
                var _listStatusCode = new List<string> { "PURC", "COMP" };
                var _listStatusId = context.PurchaseOrderStatus
                    .Where(x => _listStatusCode.Contains(x.PurchaseOrderStatusCode))
                    .Select(y => y.PurchaseOrderStatusId).ToList();
                var _listVendorOrderId = context.VendorOrder.Where(x => _listStatusId.Contains(x.StatusId))
                    .Select(y => y.VendorOrderId).ToList();

                //Lấy các item có trong list đơn hàng mua trên
                var listRequestItemHasUsing = context.VendorOrderDetail.Where(x =>
                        _listVendorOrderId.Contains(x.VendorOrderId) &&
                        listRequestItemId.Contains(x.ProcurementRequestItemId.Value))
                    .GroupBy(x => new
                    {
                        x.ProcurementRequestItemId
                    })
                    .Select(y => new RequestItem
                    {
                        ProcurementRequestItemId = y.Key.ProcurementRequestItemId,
                        Quantity = y.Sum(s => s.Quantity)
                    }).ToList();

                listProcurementRequest.ForEach(item =>
                {
                    if (item.ApproverId != null && item.ApproverId != Guid.Empty)
                    {
                        var _approver = listEmployeeEntity.FirstOrDefault(x => x.EmployeeId == item.ApproverId);
                        if (_approver != null)
                        {
                            item.ApproverName = _approver.EmployeeName;
                        }
                    }

                    item.TextShow = item.ProcurementCode + " - " + (item.Explain == null ? "" : item.Explain.Trim()) +
                                    " - " + item.ApproverName;

                    item.ListDetail = listProcurementItem
                        .Where(d => d.ProcurementRequestId == item.ProcurementRequestId)
                        .Select(d => new ProcurementRequestItemEntityModel
                        {
                            ProcurementRequestItemId = d.ProcurementRequestItemId,
                            ProcurementRequestId = d.ProcurementRequestId,
                            ProcurementPlanId = d.ProcurementPlanId,
                            CreatedById = d.CreatedById,
                            CreatedDate = d.CreatedDate,
                            ProductId = d.ProductId,
                            Quantity = d.Quantity,
                            UnitPrice = d.UnitPrice,
                            UpdatedById = d.UpdatedById,
                            UpdatedDate = d.UpdatedDate,
                            VendorName = listVendor.FirstOrDefault(v => v.VendorId == d.VendorId) != null
                                ? listVendor.FirstOrDefault(v => v.VendorId == d.VendorId).VendorName
                                : "",
                            UnitName = "",
                            Unit = Guid.Empty,
                            CurrencyUnit = d.CurrencyUnit,
                            CurrencyUnitName = listCurrencyUnit
                                .FirstOrDefault(x => x.CategoryId == d.CurrencyUnit.Value)?.CategoryName,
                            ProductCode = listProduct.FirstOrDefault(p => p.ProductId == d.ProductId) != null
                                ? listProduct.FirstOrDefault(p => p.ProductId == d.ProductId).ProductCode
                                : "",
                            ProductName = listProduct.FirstOrDefault(p => p.ProductId == d.ProductId) != null
                                ? listProduct.FirstOrDefault(p => p.ProductId == d.ProductId).ProductName
                                : "",
                            VendorId = d.VendorId,
                            ExchangeRate = d.ExchangeRate,
                            ProcurementCode = item.ProcurementCode,
                            QuantityApproval = d.QuantityApproval
                        }).ToList();

                    item.ListDetail.ForEach(detail =>
                    {
                        var productObj = listProduct.FirstOrDefault(p => p.ProductId == detail.ProductId);
                        if (productObj != null)
                        {
                            var unitObj = listCategory.FirstOrDefault(c =>
                                productObj.ProductUnitId != null && productObj.ProductUnitId != Guid.Empty &&
                                c.CategoryId == productObj.ProductUnitId);
                            detail.UnitName = unitObj != null ? unitObj.CategoryName : "";
                            detail.Unit = unitObj != null ? unitObj.CategoryId : Guid.Empty;

                            detail.ProductUnitId = detail.Unit;
                            detail.ProductUnit = detail.UnitName;
                        }

                        //Tính lại số lượng còn lại thực tế của item trong phiếu đề xuất
                        var usingItem = listRequestItemHasUsing
                            .FirstOrDefault(x => x.ProcurementRequestItemId == detail.ProcurementRequestItemId);
                        decimal? usingQuantity = 0;
                        if (usingItem != null)
                        {
                            usingQuantity = usingItem.Quantity ?? 0;
                        }

                        var remainQuantity = detail.QuantityApproval - usingQuantity;
                        detail.QuantityApproval = remainQuantity;
                    });

                    item.ListDetail = item.ListDetail.Where(x => x.QuantityApproval != 0).ToList();
                });

                #endregion

                #region Lấy danh sách Id Phiếu đề xuất gắn với Đơn hàng mua

                var listProcurementRequestId = new List<Guid?>();

                listProcurementRequestId = context.VendorOrderProcurementRequestMapping
                    .Where(x => x.VendorOrderId == parameter.VendorOrderId)
                    .Select(y => y.ProcurementRequestId).ToList();

                #endregion

                #region Lấy danh sách thông tin chi phí gắn với Đơn hàng mua

                var listCost = context.Cost.ToList();

                var listVendorOrderCostDetail = new List<VendorOrderCostDetailEntityModel>();
                listVendorOrderCostDetail = context.VendorOrderCostDetail
                    .Where(x => x.VendorOrderId == parameter.VendorOrderId).Select(y =>
                        new VendorOrderCostDetailEntityModel
                        {
                            VendorOrderCostDetailId = y.VendorOrderCostDetailId,
                            CostId = y.CostId,
                            VendorOrderId = y.VendorOrderId,
                            Active = y.Active,
                            CostName = y.CostName,
                            //CreatedById = y.CreatedById,
                            //CreatedDate = y.CreatedDate,
                            UnitPrice = y.UnitPrice,
                            CostCode = ""
                        }).ToList();

                listVendorOrderCostDetail.ForEach(item =>
                {
                    var cost = listCost.FirstOrDefault(z => z.CostId == item.CostId);

                    if (cost != null)
                    {
                        item.CostCode = cost.CostCode;
                    }
                });

                #endregion

                #region Lấy list ghi chú
                var listNote = new List<NoteEntityModel>();

                listNote = context.Note
                    .Where(x => x.ObjectId == parameter.VendorOrderId && x.ObjectType == "VENDORORDER" && x.Active == true)
                    .Select(
                        y => new NoteEntityModel
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
                    var _listAllEmployee = context.Employee.ToList();
                    var listFileNote = context.FileInFolder.Where(x => listNoteId.Contains(x.ObjectId.Value) && x.ObjectType == "NOTE").Select(
                        y => new FileInFolderEntityModel
                        {
                            FileInFolderId = y.FileInFolderId,
                            FolderId = y.FolderId,
                            ObjectId = y.ObjectId,
                            FileName = y.FileName,
                            Size = y.Size,
                            FileExtension = y.FileExtension,
                            CreatedById = y.CreatedById,
                            CreatedDate = y.CreatedDate
                            //DocumentName = y.DocumentName,
                            //DocumentSize = y.DocumentSize,
                            //DocumentUrl = y.DocumentUrl,
                            //CreatedById = y.CreatedById,
                            //CreatedDate = y.CreatedDate,
                            //UpdatedById = y.UpdatedById,
                            //UpdatedDate = y.UpdatedDate,
                            //NoteDocumentId = y.NoteDocumentId,
                            //NoteId = y.NoteId
                        }).ToList();

                    listNote.ForEach(item =>
                    {
                        var _user = listUser.FirstOrDefault(x => x.UserId == item.CreatedById);
                        var _employee = _listAllEmployee.FirstOrDefault(x => x.EmployeeId == _user.EmployeeId);
                        item.ResponsibleName = _employee.EmployeeName;
                        item.ListFile = listFileNote.Where(x => x.ObjectId.Value == item.NoteId)
                            .OrderByDescending(z => z.CreatedDate).ToList();
                    });

                    //Sắp xếp lại listNote
                    listNote = listNote.OrderByDescending(x => x.CreatedDate).ToList();
                }

                #endregion

                #region Lấy thông tin tài liệu đính kèm

                var listFile = new List<FileInFolderEntityModel>();

                var listFileInFolder = context.FileInFolder
                    .Where(x => parameter.VendorOrderId == x.ObjectId && x.ObjectType == "QLMHCON" && x.Active == true)
                    .Select(y => new FileInFolderEntityModel()
                    {
                        Size = y.Size,
                        ObjectId = y.ObjectId,
                        Active = y.Active,
                        FileExtension = y.FileExtension,
                        FileInFolderId = y.FileInFolderId,
                        FileName = y.FileName,
                        FolderId = y.FolderId,
                        ObjectType = y.ObjectType,
                        CreatedById = y.CreatedById,
                        CreatedDate = y.CreatedDate,
                        UpdatedById = y.UpdatedById,
                        UpdatedDate = y.UpdatedDate
                    }).ToList();

                listFileInFolder.ForEach(x =>
                {
                    x.UploadByName = context.User.FirstOrDefault(u => u.UserId == x.CreatedById)?.UserName;
                });

                listFile = listFileInFolder.ToList();

                #endregion

                #region Lấy tổng số tiền còn phải trả

                var paymentMethodCode = context.Category
                    .FirstOrDefault(x => x.CategoryId == vendorOrderById.PaymentMethod)?.CategoryCode;

                if (paymentMethodCode != null)
                {
                    if (paymentMethodCode.Equals("BANK"))
                    {
                        var bankPayableInvoiceList
                            = context.BankPayableInvoice.Where(x => x.ObjectId == parameter.VendorOrderId).ToList();

                        vendorOrderById.TotalPayment = 0;

                        bankPayableInvoiceList.ForEach(item =>
                        {
                            vendorOrderById.TotalPayment += item.BankPayableInvoiceAmount;
                        });
                    }
                    else
                    {
                        var payableInvoiceList
                            = context.PayableInvoice.Where(x => x.ObjectId == parameter.VendorOrderId).ToList();

                        vendorOrderById.TotalPayment = 0;

                        payableInvoiceList.ForEach(item =>
                        {
                            vendorOrderById.TotalPayment += item.Amount;
                        });
                    }
                }

                #endregion

                #region Số lượng còn dư trong kho của từng sản phẩm
                var listSucChuaSanPhamTrongKho = new List<SoDuSanPhamTrongKho>();

                var statusTypeNhapKhoId = context.CategoryType.FirstOrDefault(f => f.CategoryTypeCode == "TPH").CategoryTypeId;
                var daNhapKhoStatusId = context.Category.FirstOrDefault(f => f.CategoryTypeId == statusTypeNhapKhoId && f.CategoryCode == "NHK")?.CategoryId; // Id trạng thái đã nhập kho

                // Trạng thái phiếu xuất kho
                var statusTypeXuatKhoId = context.CategoryType.FirstOrDefault(ct => ct.CategoryTypeCode == "TPHX").CategoryTypeId;
                var daXuatKhoStatusId = context.Category.FirstOrDefault(ct => ct.CategoryCode == "NHK" && ct.CategoryTypeId == statusTypeXuatKhoId)?.CategoryId; // Id trạng thái đã xuất kho


                var listAllInventoryReceivingVoucher = context.InventoryReceivingVoucher.ToList();
                var listAllInventoryReceivingVoucherMapping = context.InventoryReceivingVoucherMapping.ToList();
                var listAllInventoryDeliveryVoucher = context.InventoryDeliveryVoucher.ToList();
                var listAllInventoryDeliveryVoucherMapping = context.InventoryDeliveryVoucherMapping.ToList();

                // Group sản phẩm theo kho => group theo sản phẩm
                var listGroupByFollowWarehouseAndProductId = listVendorOrderDetailById.Where(c => c.WarehouseId != null).GroupBy(c => new
                {
                    c.ProductId,
                    c.WarehouseId
                }).Select(m => new { m.Key.ProductId, m.Key.WarehouseId }).ToList();

                listGroupByFollowWarehouseAndProductId.ForEach(item =>
                {
                    // Nhập kho
                    var listPhieuNhapKhoId = context.InventoryReceivingVoucher.Where(c => c.StatusId == daNhapKhoStatusId)
                        .Select(m => m.InventoryReceivingVoucherId).ToList();

                    var listPhieuNhapKhoTheoWarhouseId = context.InventoryReceivingVoucherMapping.Where(c => listPhieuNhapKhoId.Contains(c.InventoryReceivingVoucherId) && item.ProductId == c.ProductId &&
                    c.WarehouseId == item.WarehouseId).ToList();
                    var listQuantityProductNhapKho = listPhieuNhapKhoTheoWarhouseId.GroupBy(c => c.ProductId)
                        .Select(m => new
                        {
                            ProductId = m.Key,
                            TotalQuantityActualNhapKho = m.Sum(g => g.QuantityActual)
                        }).ToList();

                    // Xuất kho
                    var listPhieuXuatKhoId = context.InventoryDeliveryVoucher.Where(c => c.StatusId == daXuatKhoStatusId)
                        .Select(m => m.InventoryDeliveryVoucherId).ToList();
                    var listPhieuXuatKhoTheoWarhouseId = context.InventoryDeliveryVoucherMapping.Where(c => listPhieuXuatKhoId.Contains(c.InventoryDeliveryVoucherId) && item.ProductId == c.ProductId
                     && c.WarehouseId == item.WarehouseId).ToList();

                    var listQuantityProductXuatKho = listPhieuXuatKhoTheoWarhouseId.GroupBy(c => c.ProductId)
                        .Select(m => new
                        {
                            ProductId = m.Key,
                            TotalQuantityActuralXuatKho = m.Sum(g => g.QuantityActual)
                        }).ToList();

                    var listTonKhoDauKy = context.InventoryReport.Where(c => c.WarehouseId == item.WarehouseId && c.ProductId == item.ProductId).ToList();
                    var listSoLuongTonKhoDauKyTheoSanPham = listTonKhoDauKy.GroupBy(c => c.ProductId)
                        .Select(m => new
                        {
                            ProductId = m.Key,
                            TotalQuantityTonKhoDauKi = m.Sum(g => g.StartQuantity),
                            QuantityMaximum = m.First().QuantityMaximum
                        }).ToList();

                    var nhapKho = listQuantityProductNhapKho.FirstOrDefault(c => c.ProductId == item.ProductId)?.TotalQuantityActualNhapKho ?? 0;
                    var xuatKho = listQuantityProductXuatKho.FirstOrDefault(c => c.ProductId == item.ProductId)?.TotalQuantityActuralXuatKho ?? 0;
                    var tonKho = listSoLuongTonKhoDauKyTheoSanPham.FirstOrDefault(c => c.ProductId == item.ProductId);

                    var productDetail = listProductEntity.FirstOrDefault(x => x.ProductId == item.ProductId.Value);
                    var tonKhoTheoSanPham = new SoDuSanPhamTrongKho
                    {
                        ProductId = item.ProductId.Value,
                        WarehouseId = item.WarehouseId.Value,
                        WarehouseName = listWarehouse.FirstOrDefault(x => x.WarehouseId == item.WarehouseId.Value)?.WarehouseName,
                        SucChua = tonKho?.QuantityMaximum != null ? (tonKho?.QuantityMaximum - (nhapKho - xuatKho + (tonKho?.TotalQuantityTonKhoDauKi ?? 0)))
                                : null,
                        ProductCode = productDetail?.ProductCode,
                        ProductName = productDetail?.ProductName,
                        SoLuongDat = vendorOrderDetailEntity.FirstOrDefault(x => x.ProductId == item.ProductId.Value)?.Quantity,
                        SoLuongTonKhoToiDa = listInventoryReport.FirstOrDefault(x => x.ProductId == item.ProductId.Value)?.QuantityMaximum,
                        SoLuongTonKho = nhapKho - xuatKho + (tonKho?.TotalQuantityTonKhoDauKi ?? 0),
                        //SoLuongTonKho = quantityInitial + quantityReceivingInStock - quantityDeliveryInStock; ( số lượng tồn kho + số lượng nhập - số lượng xuất)
                    };
                    listSucChuaSanPhamTrongKho.Add(tonKhoTheoSanPham);
                });

                //else
                //{
                //    // Nhập kho
                //    var listPhieuNhapKhoId = context.InventoryReceivingVoucher.Where(c => c.StatusId == daNhapKhoStatusId)
                //        .Select(m => m.InventoryReceivingVoucherId).ToList();

                //    var listPhieuNhapKhoTheoWarhouseId = context.InventoryReceivingVoucherMapping.Where(c => listPhieuNhapKhoId.Contains(c.InventoryReceivingVoucherId) && lstProductOfOrderDetailId.Contains(c.ProductId)).ToList();
                //    var listQuantityProductNhapKho = listPhieuNhapKhoTheoWarhouseId.GroupBy(c => c.ProductId)
                //        .Select(m => new
                //        {
                //            ProductId = m.Key,
                //            TotalQuantityActualNhapKho = m.Sum(g => g.QuantityActual)
                //        }).ToList();

                //    // Xuất kho
                //    var listPhieuXuatKhoId = context.InventoryDeliveryVoucher.Where(c => c.StatusId == daXuatKhoStatusId)
                //        .Select(m => m.InventoryDeliveryVoucherId).ToList();
                //    var listPhieuXuatKhoTheoWarhouseId = context.InventoryDeliveryVoucherMapping.Where(c => listPhieuXuatKhoId.Contains(c.InventoryDeliveryVoucherId) && lstProductOfOrderDetailId.Contains(c.ProductId)).ToList();
                //    var listQuantityProductXuatKho = listPhieuXuatKhoTheoWarhouseId.GroupBy(c => c.ProductId)
                //        .Select(m => new
                //        {
                //            ProductId = m.Key,
                //            TotalQuantityActuralXuatKho = m.Sum(g => g.QuantityActual)
                //        }).ToList();

                //    var listTonKhoDauKy = context.InventoryReport.ToList();
                //    var listSoLuongTonKhoDauKyTheoSanPham = listTonKhoDauKy.GroupBy(c => c.ProductId)
                //        .Select(m => new
                //        {
                //            ProductId = m.Key,
                //            TotalQuantityTonKhoDauKi = m.Sum(g => g.Quantity),
                //            QuantityMaximum = m.First().QuantityMaximum
                //        }).ToList();

                //    lstProductOfOrderDetailId.ForEach(item =>
                //    {
                //        var nhapKho = listQuantityProductNhapKho.FirstOrDefault(c => c.ProductId == item)?.TotalQuantityActualNhapKho ?? 0;
                //        var xuatKho = listQuantityProductXuatKho.FirstOrDefault(c => c.ProductId == item)?.TotalQuantityActuralXuatKho ?? 0;
                //        var tonKho = listSoLuongTonKhoDauKyTheoSanPham.FirstOrDefault(c => c.ProductId == item)?.TotalQuantityTonKhoDauKi ?? 0;

                //        var tonKhoTheoSanPham = new SoDuSanPhamTrongKho
                //        {
                //            ProductId = item.Value,
                //            SucChua = nhapKho - xuatKho + tonKho,
                //        };

                //        listSucChuaSanPhamTrongKho.Add(tonKhoTheoSanPham);
                //    });
                //}
                #endregion

                #region Lấy phiếu nhập kho theo đơn hàng mua

                var listPhieuNhapKho = new List<InventoryReceivingVoucherEntityModel>();

                var listInventoryReceivingId = context.InventoryReceivingVoucherMapping
                    .Where(x => x.ObjectId == parameter.VendorOrderId)
                    .Select(y => y.InventoryReceivingVoucherId).ToList();

                listPhieuNhapKho = context.InventoryReceivingVoucher
                    .Where(x => listInventoryReceivingId.Contains(x.InventoryReceivingVoucherId))
                    .Select(y => new InventoryReceivingVoucherEntityModel
                    {
                        InventoryReceivingVoucherId = y.InventoryReceivingVoucherId,
                        InventoryReceivingVoucherCode = y.InventoryReceivingVoucherCode,
                        StatusId = y.StatusId,
                        InventoryReceivingVoucherType = y.InventoryReceivingVoucherType,
                        WarehouseId = y.WarehouseId,
                        InventoryReceivingVoucherDate = y.InventoryReceivingVoucherDate,
                        PartnersId = y.PartnersId,
                        CreatedById = y.CreatedById,
                        CreatedDate = y.CreatedDate
                    }).ToList();

                listPhieuNhapKho.ForEach(item =>
                    {

                        #region Lấy tên loại phiếu

                        if (item.InventoryReceivingVoucherType == 1)
                        {
                            item.InventoryReceivingVoucherTypeName = "Nhập theo phiếu mua hàng";
                        }
                        else if (item.InventoryReceivingVoucherType == 2)
                        {
                            item.InventoryReceivingVoucherTypeName = "Nhập hàng bán bị trả lại";
                        }
                        else if (item.InventoryReceivingVoucherType == 3)
                        {
                            item.InventoryReceivingVoucherTypeName = "Nhập kiểm kê";
                        }
                        else if (item.InventoryReceivingVoucherType == 4)
                        {
                            item.InventoryReceivingVoucherTypeName = "Nhập điều chuyển";
                        }
                        else if (item.InventoryReceivingVoucherType == 5)
                        {
                            item.InventoryReceivingVoucherTypeName = "Nhập khác";
                        }

                        #endregion

                        #region Lấy tên người lập phiếu

                        var employeeIdCreate = context.User.FirstOrDefault(x => x.UserId == item.CreatedById)
                            ?.EmployeeId;
                        var employeeCreate = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeIdCreate);

                        item.CreatedName = employeeCreate?.EmployeeName.Trim();

                        #endregion

                        #region Lấy tên trạng thái

                        var status = context.Category.FirstOrDefault(x => x.CategoryId == item.StatusId);
                        item.StatusName = status?.CategoryName;

                        #endregion
                    });

                listPhieuNhapKho = listPhieuNhapKho.OrderByDescending(z => z.CreatedDate).ToList();

                #endregion

                var listPaymentMethodResult = new List<CategoryEntityModel>();
                var _listPaymentMethod = listPaymentMethod.OrderBy(w => w.CategoryName).ToList();
                _listPaymentMethod.ForEach(item =>
                {
                    var _item = new CategoryEntityModel(item);
                    listPaymentMethodResult.Add(_item);
                });

                var listOrderStatusResult = new List<PurchaseOrderStatusEntityModel>();
                var _listOrderStatus = listOrderStatus.OrderBy(w => w.Description).ToList();
                _listOrderStatus.ForEach(item =>
                {
                    var _item = new PurchaseOrderStatusEntityModel(item);
                    listOrderStatusResult.Add(_item);
                });

                var listBankAccountResult = new List<BankAccountEntityModel>();
                var _listBankAccount = listBankAccount.OrderBy(w => w.AccountName).ToList();
                _listBankAccount.ForEach(item =>
                {
                    var _item = new BankAccountEntityModel(item);
                    listBankAccountResult.Add(_item);
                });

                return new GetDataEditVendorOrderResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success",
                    VendorOrderById = vendorOrderById,
                    ListVendorOrderDetailById = listVendorOrderDetailById,
                    ListPaymentMethod = listPaymentMethodResult,
                    ListOrderStatus = listOrderStatusResult,
                    ListEmployeeModel = ListEmployeeEntityModel.OrderBy(w => w.EmployeeName).ToList(),
                    VendorCreateOrderModel = vendorCreateOrderModel.OrderBy(w => w.VendorName).ToList(),
                    ListBankAccount = listBankAccountResult,
                    ListProcurementRequest = listProcurementRequest,
                    //ListContract = listContract,
                    ListWareHouse = listWarehouse,
                    ListProcurementRequestId = listProcurementRequestId,
                    ListVendorOrderCostDetail = listVendorOrderCostDetail,
                    ListNote = listNote,
                    ListFile = listFile,
                    ListSucChuaSanPhamTrongKho = listSucChuaSanPhamTrongKho,
                    ListPhieuNhapKho = listPhieuNhapKho,
                };
            }
            catch (Exception ex)
            {
                return new GetDataEditVendorOrderResult()
                {
                    MessageCode = ex.ToString(),
                    StatusCode = HttpStatusCode.ExpectationFailed
                };
            }
        }

        public GetMasterDataSearchVendorOrderResult GetGetMasterDataSearchVendorOrder(GetMasterDataSearchVendorOrderParameter parameter)
        {
            try
            {
                var listProcurementRequest = new List<ProcurementRequestEntityModel>();
                var listProduct = new List<ProductEntityModel>();

                var vendors = context.Vendor.Where(c => c.Active == true).OrderBy(c => c.VendorName).ToList();
                var orderStatus = context.PurchaseOrderStatus.Where(x => x.Active).OrderBy(c => c.Description)
                    .ToList();
                var employees = context.Employee.OrderBy(c => c.EmployeeCode).ToList();

                listProcurementRequest = context.ProcurementRequest.Where(x => x.Active == true).Select(y =>
                    new ProcurementRequestEntityModel
                    {
                        ProcurementRequestId = y.ProcurementRequestId,
                        ProcurementCode = y.ProcurementCode,
                        Explain = y.Explain,
                        RequestEmployeeId = y.RequestEmployeeId,
                        TextShow = ""
                    }).OrderBy(z => z.ProcurementCode).ToList();

                listProcurementRequest.ForEach(item =>
                {
                    var employeeRequest = employees.FirstOrDefault(x => x.EmployeeId == item.RequestEmployeeId);
                    var requestEmployeeName = "";
                    if (employeeRequest != null)
                    {
                        requestEmployeeName = employeeRequest.EmployeeName.Trim();
                    }

                    var explain = item.Explain ?? "";
                    item.TextShow = item.ProcurementCode + " - " + explain + " - " + requestEmployeeName;
                });

                listProduct = context.Product.Where(x => x.Active == true).Select(y => new ProductEntityModel
                {
                    ProductId = y.ProductId,
                    ProductCodeName = y.ProductCode + " - " + y.ProductName
                }).OrderBy(z => z.ProductName).ToList();

                var companyConfigEntity = context.CompanyConfiguration.FirstOrDefault();
                var companyConfig = new DataAccess.Models.CompanyConfigEntityModel();
                companyConfig.CompanyId = companyConfigEntity.CompanyId;
                companyConfig.CompanyName = companyConfigEntity.CompanyName;
                companyConfig.Email = companyConfigEntity.Email;
                companyConfig.Phone = companyConfigEntity.Phone;
                companyConfig.TaxCode = companyConfigEntity.TaxCode;
                companyConfig.CompanyAddress = companyConfigEntity.CompanyAddress;
                companyConfig.CompanyAddress = companyConfigEntity.CompanyAddress;

                var listVendor = new List<VendorEntityModel>();
                vendors.ForEach(item =>
                {
                    var _item = new VendorEntityModel(item);
                    listVendor.Add(_item);
                });

                var listOrderStatus = new List<PurchaseOrderStatusEntityModel>();
                orderStatus.ForEach(item =>
                {
                    var _item = new PurchaseOrderStatusEntityModel(item);
                    listOrderStatus.Add(_item);
                });

                var listEmployee = new List<EmployeeEntityModel>();
                employees.ForEach(item =>
                {
                    var _item = new EmployeeEntityModel(item);
                    listEmployee.Add(_item);
                });

                return new GetMasterDataSearchVendorOrderResult
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success",
                    ListVendor = listVendor,
                    ListOrderStatus = listOrderStatus,
                    ListEmployee = listEmployee,
                    ListProcurementRequest = listProcurementRequest,
                    ListProduct = listProduct,
                    CompanyConfig = companyConfig,
                };
            }
            catch (Exception ex)
            {
                return new GetMasterDataSearchVendorOrderResult
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = ex.Message
                };
            }
        }

        public GetDataSearchVendorQuoteResult GetDataSearchVendorQuote(GetDataSearchVendorQuoteParameter parameter)
        {
            try
            {
                var vendorList = context.Vendor.ToList();
                var statusList = context.Category.ToList();
                var listProduct = context.Product.ToList();

                var statusTypeVendorQuoteId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "TBGV").CategoryTypeId;
                var statusNewVendorId = statusList.FirstOrDefault(c => c.CategoryTypeId == statusTypeVendorQuoteId && c.CategoryCode == "MOI").CategoryId;

                #region Lấy danh sách báo giá nhà cung cấp 

                var vendorQuoteList = context.SuggestedSupplierQuotes.Where(c => c.Active == true)
                    .Select(vq => new SuggestedSupplierQuotesEntityModel()
                    {
                        SuggestedSupplierQuoteId = vq.SuggestedSupplierQuoteId,
                        SuggestedSupplierQuote = vq.SuggestedSupplierQuote,
                        StatusId = vq.StatusId,
                        StatusName = "",
                        VendorId = vq.VendorId,
                        VendorCode = "",
                        VendorName = "",
                        PersonInChargeId = vq.PersonInChargeId,
                        RecommendedDate = vq.RecommendedDate,
                        QuoteTermDate = vq.QuoteTermDate,
                        ObjectType = vq.ObjectType,
                        ObjectId = vq.ObjectId,
                        Note = vq.Note,
                        Active = vq.Active,
                        CreatedDate = vq.CreatedDate,
                        CreatedById = vq.CreatedById,
                        UpdatedDate = vq.UpdatedDate,
                        UpdatedById = vq.UpdatedById,
                        CanDelete = false,
                        ProcurementRequestId = vq.ProcurementRequestId,

                        ListVendorQuoteDetail = null
                    }).OrderByDescending(vq => vq.SuggestedSupplierQuote).ToList();

                #endregion

                #region Lấy chi tiết báo giá nhà cung cấp 

                var listVendorDetail = context.SuggestedSupplierQuotesDetail.Where(c => c.Active == true)
                    .Select(m => new SuggestedSupplierQuotesDetailEntityModel
                    {
                        SuggestedSupplierQuoteDetailId = m.SuggestedSupplierQuoteDetailId,
                        SuggestedSupplierQuoteId = m.SuggestedSupplierQuoteId,
                        ProductId = m.ProductId,
                        ProductCode = "",
                        ProductName = "",
                        Quantity = m.Quantity,
                        Note = m.Note,
                        CreatedById = m.CreatedById,
                        CreatedDate = m.CreatedDate
                    }).ToList();
                listVendorDetail.ForEach(item =>
                {
                    var product = listProduct.FirstOrDefault(c => c.ProductId == item.ProductId);
                    item.ProductCode = product?.ProductCode ?? "";
                    item.ProductName = product?.ProductName ?? "";
                });

                #endregion

                vendorQuoteList.ForEach(item =>
                {
                    if (item.StatusId == statusNewVendorId)
                        item.CanDelete = true;
                    var vendor = vendorList.FirstOrDefault(v => v.VendorId == item.VendorId);
                    if (vendor != null)
                    {
                        item.VendorCode = vendor.VendorCode;
                        item.VendorName = vendor.VendorName;
                        item.VendorGroupId = vendor.VendorGroupId;
                    }
                    var status = statusList.FirstOrDefault(s => s.CategoryId == item.StatusId);
                    if (status != null)
                    {
                        item.StatusName = status.CategoryName;
                    }

                    item.ListVendorQuoteDetail = listVendorDetail
                        .Where(c => c.SuggestedSupplierQuoteId == item.SuggestedSupplierQuoteId).ToList();
                });

                vendorQuoteList = vendorQuoteList.Where(x =>
                        (parameter.VendorGroupIdList.Count == 0 ||
                         parameter.VendorGroupIdList.Contains(x.VendorGroupId.Value)) &&
                        (String.IsNullOrWhiteSpace(parameter.VendorName) ||
                         x.VendorName.ToLower().Contains(parameter.VendorName?.ToLower())) &&
                        (String.IsNullOrWhiteSpace(parameter.VendorCode) ||
                         x.VendorCode.ToLower().Contains(parameter.VendorCode?.ToLower())))
                    .OrderByDescending(z => z.CreatedDate)
                    .ToList();

                return new GetDataSearchVendorQuoteResult()
                {
                    ListVendorQuote = vendorQuoteList,
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success"
                };
            }
            catch (Exception ex)
            {
                return new GetDataSearchVendorQuoteResult()
                {
                    MessageCode = ex.Message,
                    StatusCode = HttpStatusCode.ExpectationFailed
                };
            }
        }

        public CreateVendorQuoteResult CreateVendorQuote(ListVendorQuoteParameter parameter)
        {
            try
            {
                var vendorQuote = parameter;
                var statusType = context.CategoryType.FirstOrDefault(ct => ct.CategoryTypeCode == "TBGV");
                var statusNew = context.Category.FirstOrDefault(c =>
                    c.CategoryTypeId == statusType.CategoryTypeId && c.CategoryCode == "MOI");

                vendorQuote.SuggestedSupplierQuotes.SuggestedSupplierQuoteId = Guid.NewGuid();
                vendorQuote.SuggestedSupplierQuotes.SuggestedSupplierQuote = GenerateCustomerCode(0, parameter.Index);
                vendorQuote.SuggestedSupplierQuotes.StatusId = statusNew.CategoryId;
                vendorQuote.SuggestedSupplierQuotes.CreatedDate = DateTime.Now;
                vendorQuote.SuggestedSupplierQuotes.RecommendedDate = DateTime.Now;
                vendorQuote.SuggestedSupplierQuotes.Active = true;

                context.SuggestedSupplierQuotes.Add(vendorQuote.SuggestedSupplierQuotes.ToEntity());
                context.SaveChanges();

                vendorQuote.SuggestedSupplierQuoteDetailList.ForEach(item =>
                {
                    item.SuggestedSupplierQuoteDetailId = Guid.NewGuid();
                    item.SuggestedSupplierQuoteId = vendorQuote.SuggestedSupplierQuotes.SuggestedSupplierQuoteId;
                    item.Active = true;
                    item.CreatedDate = DateTime.Now;

                    context.SuggestedSupplierQuotesDetail.Add(item.ToEntity());
                    context.SaveChanges();
                });

                return new CreateVendorQuoteResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success"
                };
            }
            catch (Exception ex)
            {
                return new CreateVendorQuoteResult()
                {
                    MessageCode = ex.ToString(),
                    StatusCode = HttpStatusCode.ExpectationFailed
                };
            }
        }
        private string GenerateCustomerCode(int maxCode, int index)
        {
            //Auto gen CustomerCode 1911190001
            int currentYear = DateTime.Now.Year % 100;
            int currentMonth = DateTime.Now.Month;
            int currentDate = DateTime.Now.Day;
            int MaxNumberCode = 0;
            var quoteVendorList = context.SuggestedSupplierQuotes.Where(cu => cu.Active == true).ToList();
            if (maxCode == 0)
            {
                var customer = quoteVendorList.OrderByDescending(or => or.CreatedDate).FirstOrDefault();
                if (customer != null)
                {
                    var customerCode = customer.SuggestedSupplierQuote;
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
            }
            else
            {
                MaxNumberCode = maxCode + 1;
            }
            var monthText = currentMonth < 10 ? ("0" + currentMonth) : currentMonth.ToString();
            var dayText = currentDate < 10 ? ("0" + currentDate) : currentDate.ToString();
            var customerCodeNew = string.Format("DNBG{0}{1}{2}{3}", currentYear, monthText, dayText, (MaxNumberCode + index).ToString("D4"));
            var customerCodeList = quoteVendorList.Where(q => q.SuggestedSupplierQuote == customerCodeNew).ToList();
            if (customerCodeList.Count > 0)
            {
                return GenerateCustomerCode(MaxNumberCode, index);
            }

            return string.Format("DNBG{0}{1}{2}{3}", currentYear, monthText, dayText, (MaxNumberCode + index).ToString("D4"));
        }

        public SearchVendorProductPriceResult SearchVendorProductPrice(SearchVendorProductPriceParameter parameter)
        {
            try
            {
                var commonCategoryType = context.CategoryType.ToList();
                var commonCategory = context.Category.ToList();

                // Đơn vị tính
                var productUnitTypeId =
                    commonCategoryType.FirstOrDefault(c => c.CategoryTypeCode == "DNH")?.CategoryTypeId ?? Guid.Empty;
                var listAllProductUnit = commonCategory.Where(c => c.CategoryTypeId == productUnitTypeId).ToList() ??
                                         new List<Category>();
                // Đơn vị tiền
                var moneyUnitTypeId = commonCategoryType.FirstOrDefault(c => c.CategoryTypeCode == "DTI")?.CategoryTypeId ??
                                      Guid.Empty;
                var listAllMoneyUnit = commonCategory.Where(c => c.CategoryTypeId == moneyUnitTypeId).ToList() ??
                                       new List<Category>();

                var commonProduct = context.Product.Where(c => c.Active == true).ToList();
                var commonVendor = context.Vendor.Where(c => c.Active == true).ToList();
                var listProductId = commonProduct.Where(c => (parameter.ProductName == null || parameter.ProductName == ""
                                                                                            || c.ProductName.ToLower()
                                                                                                .Contains(parameter
                                                                                                    .ProductName
                                                                                                    .ToLower())))
                    .Select(c => c.ProductId).ToList();

                var listVendorId = commonVendor.Where(c => (parameter.VendorName == null || parameter.VendorName == ""
                                                                                         || c.VendorName.ToLower()
                                                                                             .Contains(parameter.VendorName
                                                                                                 .ToLower())))
                    .Select(c => c.VendorId).ToList();

                var commonPriceSuggestedSupplierQuotesMapping = context.PriceSuggestedSupplierQuotesMapping.ToList();

                var listVendorProductPrice = context.ProductVendorMapping.Where(c =>
                        c.Active == true && listProductId.Contains(c.ProductId) &&
                        listVendorId.Contains(c.VendorId))
                    .Select(m => new ProductVendorMappingEntityModel
                    {
                        ProductVendorMappingId = m.ProductVendorMappingId,
                        ProductId = m.ProductId,
                        VendorId = m.VendorId,
                        MoneyUnitId = m.UnitPriceId.Value,
                        MoneyUnitName = "",
                        CreatedById = m.CreatedById,
                        CreatedDate = m.CreatedDate,
                        Active = m.Active,
                        VendorProductName = m.VendorProductName,
                        VendorProductCode = m.VendorProductCode,
                        MiniumQuantity = m.MiniumQuantity,
                        Price = m.Price,
                        ExchangeRate = m.ExchangeRate,
                        FromDate = m.FromDate,
                        ToDate = m.ToDate,
                        ProductName = "",
                        VendorName = commonVendor.FirstOrDefault(c => c.VendorId == m.VendorId).VendorName ?? "",
                        ProductCode = "",
                        ProductUnitName = "",
                        ListSuggestedSupplierQuoteId = new List<Guid?>()
                    }).OrderByDescending(c => c.CreatedDate).ToList();

                listVendorProductPrice.ForEach(item =>
                {
                    var product = commonProduct.FirstOrDefault(c => c.ProductId == item.ProductId) ?? new Product();
                    item.ProductName = product.ProductName;
                    item.ProductCode = product.ProductCode;
                    item.ProductUnitName = listAllProductUnit.FirstOrDefault(c => c.CategoryId == product.ProductUnitId)
                                               ?.CategoryName ?? "";
                    item.MoneyUnitName =
                        listAllMoneyUnit.FirstOrDefault(c => c.CategoryId == item.MoneyUnitId)?.CategoryName ?? "";

                    item.ListSuggestedSupplierQuoteId = commonPriceSuggestedSupplierQuotesMapping
                        .Where(x => x.ProductVendorMappingId == item.ProductVendorMappingId)
                        .Select(y => y.SuggestedSupplierQuoteId).ToList();
                });

                return new SearchVendorProductPriceResult
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success",
                    ListVendorProductPrice = listVendorProductPrice,
                };
            }
            catch (Exception e)
            {
                return new SearchVendorProductPriceResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public CreateVendorProductPriceResult CreateVendorProductPrice(CreateVendorProductPriceParameter parameter)
        {
            try
            {
                var listPriceSuggestedSupplierQuote = new List<PriceSuggestedSupplierQuotesMapping>();
                if (parameter.ProductVendorMapping.ProductVendorMappingId == null ||
                    parameter.ProductVendorMapping.ProductVendorMappingId == Guid.Empty)
                {
                    // Thêm mới giá nhà cung cấp
                    var productVendorMapping = new ProductVendorMapping
                    {
                        ProductVendorMappingId = Guid.NewGuid(),
                        ProductId = parameter.ProductVendorMapping.ProductId,
                        VendorId = parameter.ProductVendorMapping.VendorId,
                        VendorProductName = parameter.ProductVendorMapping.VendorProductName,
                        VendorProductCode = parameter.ProductVendorMapping.VendorProductCode,
                        MiniumQuantity = parameter.ProductVendorMapping.MiniumQuantity,
                        UnitPriceId = parameter.ProductVendorMapping.MoneyUnitId,
                        Price = parameter.ProductVendorMapping.Price,
                        FromDate = parameter.ProductVendorMapping.FromDate,
                        ToDate = parameter.ProductVendorMapping.ToDate,
                        ExchangeRate = parameter.ProductVendorMapping.ExchangeRate,
                        CreatedById = parameter.UserId,
                        CreatedDate = DateTime.Now,
                        UpdatedById = null,
                        UpdatedDate = null,
                        Active = true
                    };

                    if (parameter.ListSuggestedSupplierQuoteId != null)
                    {
                        parameter.ListSuggestedSupplierQuoteId.ForEach(item =>
                        {
                            var priceMapping = new PriceSuggestedSupplierQuotesMapping
                            {
                                PriceSuggestedSupplierQuotesMappingId = Guid.NewGuid(),
                                SuggestedSupplierQuoteId = item,
                                ProductVendorMappingId = productVendorMapping.ProductVendorMappingId,
                                CreatedDate = DateTime.Now,
                                CreatedById = parameter.UserId,
                                Active = true
                            };
                            listPriceSuggestedSupplierQuote.Add(priceMapping);
                        });
                    }

                    context.PriceSuggestedSupplierQuotesMapping.AddRange(listPriceSuggestedSupplierQuote);
                    context.ProductVendorMapping.Add(productVendorMapping);
                    context.SaveChanges();

                    return new CreateVendorProductPriceResult
                    {
                        StatusCode = HttpStatusCode.OK,
                        MessageCode = CommonMessage.VendorPrice.CREATE_SUCCESS
                    };
                }
                else
                {
                    // Update giá nhà cung cấp
                    var oldProdUctVendorMapping = context.ProductVendorMapping.FirstOrDefault(c =>
                        c.ProductVendorMappingId == parameter.ProductVendorMapping.ProductVendorMappingId);
                    if (oldProdUctVendorMapping != null)
                    {
                        using (var transaction = context.Database.BeginTransaction())
                        {
                            oldProdUctVendorMapping.ProductId = parameter.ProductVendorMapping.ProductId;
                            oldProdUctVendorMapping.VendorId = parameter.ProductVendorMapping.VendorId;
                            oldProdUctVendorMapping.VendorProductName = parameter.ProductVendorMapping.VendorProductName;
                            oldProdUctVendorMapping.VendorProductCode = parameter.ProductVendorMapping.VendorProductCode;
                            oldProdUctVendorMapping.MiniumQuantity = parameter.ProductVendorMapping.MiniumQuantity;
                            oldProdUctVendorMapping.UnitPriceId = parameter.ProductVendorMapping.MoneyUnitId;
                            oldProdUctVendorMapping.Price = parameter.ProductVendorMapping.Price;
                            oldProdUctVendorMapping.ExchangeRate = parameter.ProductVendorMapping.ExchangeRate;
                            oldProdUctVendorMapping.FromDate = parameter.ProductVendorMapping.FromDate;
                            oldProdUctVendorMapping.ToDate = parameter.ProductVendorMapping.ToDate;
                            oldProdUctVendorMapping.UpdatedById = parameter.UserId;
                            oldProdUctVendorMapping.UpdatedDate = DateTime.Now;
                            oldProdUctVendorMapping.Active = true;

                            var deletePriceSuggestedMapping = new List<PriceSuggestedSupplierQuotesMapping>();
                            deletePriceSuggestedMapping = context.PriceSuggestedSupplierQuotesMapping.Where(c =>
                                c.ProductVendorMappingId == oldProdUctVendorMapping.ProductVendorMappingId).ToList();

                            context.PriceSuggestedSupplierQuotesMapping.RemoveRange(deletePriceSuggestedMapping);

                            if (parameter.ListSuggestedSupplierQuoteId != null)
                            {
                                parameter.ListSuggestedSupplierQuoteId.ForEach(item =>
                                {
                                    var priceMapping = new PriceSuggestedSupplierQuotesMapping
                                    {
                                        PriceSuggestedSupplierQuotesMappingId = Guid.NewGuid(),
                                        SuggestedSupplierQuoteId = item,
                                        ProductVendorMappingId = oldProdUctVendorMapping.ProductVendorMappingId,
                                        CreatedDate = DateTime.Now,
                                        CreatedById = parameter.UserId,
                                        Active = true
                                    };
                                    listPriceSuggestedSupplierQuote.Add(priceMapping);
                                });
                            }

                            context.PriceSuggestedSupplierQuotesMapping.AddRange(listPriceSuggestedSupplierQuote);
                            context.ProductVendorMapping.Update(oldProdUctVendorMapping);

                            context.SaveChanges();
                            transaction.Commit();
                        }
                    }

                    return new CreateVendorProductPriceResult
                    {
                        StatusCode = HttpStatusCode.OK,
                        MessageCode = CommonMessage.VendorPrice.EDIT_SUCCESS
                    };
                }
            }
            catch (Exception ex)
            {
                return new CreateVendorProductPriceResult
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = ex.Message,
                };
            }
        }

        public DeleteVendorProductPriceResult DeleteVendorProductPrice(DeleteVendorProductPriceParameter parameter)
        {
            try
            {
                var vendorProductPrice = context.ProductVendorMapping.FirstOrDefault(c =>
                    c.ProductVendorMappingId == parameter.ProductVendorMappingId);

                if (vendorProductPrice != null)
                {
                    var listPriceSuggestedSupplierQuotesMapping = context.PriceSuggestedSupplierQuotesMapping
                        .Where(x => x.ProductVendorMappingId == vendorProductPrice.ProductVendorMappingId).ToList();

                    context.ProductVendorMapping.Remove(vendorProductPrice);
                    context.PriceSuggestedSupplierQuotesMapping.RemoveRange(listPriceSuggestedSupplierQuotesMapping);
                    context.SaveChanges();
                }

                return new DeleteVendorProductPriceResult
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = CommonMessage.VendorPrice.DELETE_SUCCESS
                };
            }
            catch (Exception ex)
            {
                return new DeleteVendorProductPriceResult
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = ex.Message
                };
            }
        }

        public DownloadTemplateVendorProductPriceResult DownloadTemplateVendorProductPrice(DownloadTemplateVendorProductPriceParameter parameter)
        {
            try
            {
                string rootFolder = _hostingEnvironment.WebRootPath + "\\ExcelTemplate";
                string fileName = @"Template_Import_Vendor_Product_Price.xlsx";

                //FileInfo file = new FileInfo(Path.Combine(rootFolder, fileName));
                string newFilePath = Path.Combine(rootFolder, fileName);
                byte[] data = File.ReadAllBytes(newFilePath);

                return new DownloadTemplateVendorProductPriceResult
                {
                    TemplateExcel = data,
                    MessageCode = string.Format("Đã dowload file Template_Import_Vendor_Product_Price"),
                    FileName = "Template_Import_Vendor_Product_Price",
                    StatusCode = HttpStatusCode.OK
                };
            }
            catch (Exception)
            {
                return new DownloadTemplateVendorProductPriceResult
                {
                    MessageCode = "Đã có lỗi xảy ra trong quá trình download",
                    StatusCode = HttpStatusCode.ExpectationFailed
                };
            }
        }

        public ImportProductVendorPriceResult ImportProductVendorPrice(ImportVendorProductPriceParameter parameter)
        {
            try
            {
                if (parameter.ListProductVendorMapping.Count > 0)
                {
                    var listProductVendorMapping = new List<ProductVendorMapping>();
                    parameter.ListProductVendorMapping.ForEach(item =>
                    {
                        var temp = new ProductVendorMapping
                        {
                            ProductVendorMappingId = Guid.NewGuid(),
                            ProductId = item.ProductId,
                            VendorId = item.VendorId,
                            VendorProductCode = item.VendorProductCode,
                            VendorProductName = item.VendorProductName,
                            MiniumQuantity = item.MiniumQuantity,
                            Price = item.Price,
                            UnitPriceId = item.MoneyUnitId,
                            FromDate = item.FromDate,
                            ToDate = item.ToDate,
                            CreatedById = parameter.UserId,
                            CreatedDate = DateTime.Now,
                            Active = true,
                        };
                        listProductVendorMapping.Add(temp);
                    });
                    context.ProductVendorMapping.AddRange(listProductVendorMapping);
                    context.SaveChanges();
                }

                return new ImportProductVendorPriceResult
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = CommonMessage.VendorPrice.IMPORT_SUCCESS
                };
            }
            catch (Exception ex)
            {
                return new ImportProductVendorPriceResult
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = ex.Message
                };
            }
        }

        public GetDataAddEditCostVendorOrderResult GetDataAddEditCostVendorOrder(GetDataAddEditCostVendorOrderParameter parameter)
        {
            try
            {
                var typteStatusId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "DSP")
                    .CategoryTypeId;
                var status = context.Category
                    .FirstOrDefault(c => c.CategoryTypeId == typteStatusId && c.CategoryCode == "DSD");

                var listCost = context.Cost.Where(c =>
                    c.Active && c.StatusId == status.CategoryId).Select(
                    y =>
                        new CostEntityModel
                        {
                            CostId = y.CostId,
                            CostCode = y.CostCode,
                            CostName = y.CostName,
                            CostCodeName = y.CostCode + " - " + y.CostName,
                            StatusId = y.StatusId,
                            OrganizationId = y.OrganizationId,
                            CreatedById = y.CreatedById,
                            CreatedDate = y.CreatedDate,
                        }).ToList();

                if (parameter.UserId != null && parameter.UserId != Guid.Empty)
                {
                    var employeeId = context.User.FirstOrDefault(u => u.UserId == parameter.UserId).EmployeeId;

                    var employees = context.Employee.Where(e => e.Active == true).ToList();

                    var employee = employees.FirstOrDefault(e => e.EmployeeId == employeeId);

                    List<Guid?> listGetAllChild = new List<Guid?>();
                    listGetAllChild.Add(employee.OrganizationId.Value);
                    listGetAllChild = getOrganizationChildrenId(employee.OrganizationId.Value, listGetAllChild);

                    listCost = listCost.Where(c => listGetAllChild.Contains(c.OrganizationId)).ToList();
                }

                return new GetDataAddEditCostVendorOrderResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success",
                    ListCost = listCost
                };
            }
            catch (Exception e)
            {
                return new GetDataAddEditCostVendorOrderResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
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

            return result;
        }

        public GetMasterDataCreateSuggestedSupplierQuoteResult GetMasterDataCreateSuggestedSupplierQuote(GetMasterDataCreateSuggestedSupplierQuoteParameter parameter)
        {
            try
            {
                var commonEmployee = context.Employee.ToList();
                var commonProduct = context.Product.ToList();
                var commonSaleBidding = context.SaleBidding.Where(c => c.Active == true).ToList();
                var commonQuote = context.Quote.Where(c => c.Active == true).ToList();
                var typeProductUnitId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "DNH")?.CategoryTypeId ?? Guid.Empty;
                var listAllProductUnit = context.Category.Where(c => c.CategoryTypeId == typeProductUnitId).ToList();

                var inforExportExcel = new InforExportExcelModel();
                var statusTypeSupplierQuoteRequestId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "TBGV")?.CategoryTypeId ?? Guid.Empty;
                var listAllStatus = context.Category.Where(c => c.CategoryTypeId == statusTypeSupplierQuoteRequestId)
                    .Select(m => new CategoryEntityModel
                    {
                        CategoryId = m.CategoryId,
                        CategoryCode = m.CategoryCode,
                        CategoryName = m.CategoryName,
                        CategoryTypeId = m.CategoryTypeId
                    }).ToList();

                var vendorQuoteEntityModel = new SuggestedSupplierQuotesEntityModel();

                if (parameter.SuggestedSupplierQuoteId != null && parameter.SuggestedSupplierQuoteId != Guid.Empty)
                {

                    var vendorQuote = context.SuggestedSupplierQuotes.FirstOrDefault(c => c.SuggestedSupplierQuoteId == parameter.SuggestedSupplierQuoteId);
                    var contact = context.Contact.FirstOrDefault(c => c.ObjectId == vendorQuote.VendorId && c.ObjectType == "VEN");
                    vendorQuoteEntityModel = new SuggestedSupplierQuotesEntityModel
                    {
                        SuggestedSupplierQuoteId = vendorQuote.SuggestedSupplierQuoteId,
                        SuggestedSupplierQuote = vendorQuote.SuggestedSupplierQuote,
                        VendorId = vendorQuote.VendorId,
                        Note = vendorQuote.Note,
                        ProcurementRequestId = vendorQuote.ProcurementRequestId,
                        PersonInChargeId = vendorQuote.PersonInChargeId,
                        RecommendedDate = vendorQuote.RecommendedDate,
                        QuoteTermDate = vendorQuote.QuoteTermDate,
                        CreatedById = vendorQuote.CreatedById,
                        CreatedDate = vendorQuote.CreatedDate,
                        StatusId = vendorQuote.StatusId,
                        ObjectId = vendorQuote.ObjectId,
                        ObjectType = vendorQuote.ObjectType,
                        SaleBiddingCode = commonSaleBidding.FirstOrDefault(c => c.SaleBiddingId == vendorQuote.ObjectId && vendorQuote.ObjectType == "SALEBIDDING")?.SaleBiddingCode ?? "",
                        QuoteCode = commonQuote.FirstOrDefault(c => c.QuoteId == vendorQuote.ObjectId && vendorQuote.ObjectType == "QUOTE")?.QuoteCode ?? "",
                        Email = contact?.Email ?? "",

                        ListVendorQuoteDetail = new List<SuggestedSupplierQuotesDetailEntityModel>(),
                    };

                    vendorQuoteEntityModel.ListVendorQuoteDetail = context.SuggestedSupplierQuotesDetail.Where(c => c.SuggestedSupplierQuoteId == parameter.SuggestedSupplierQuoteId)
                        .Select(m => new SuggestedSupplierQuotesDetailEntityModel
                        {
                            SuggestedSupplierQuoteDetailId = m.SuggestedSupplierQuoteDetailId,
                            SuggestedSupplierQuoteId = m.SuggestedSupplierQuoteId,
                            Quantity = m.Quantity,
                            Note = m.Note,
                            ProductId = m.ProductId,
                            ProductCode = "",
                            ProductName = "",
                            Active = m.Active,
                            CreatedById = m.CreatedById,
                            CreatedDate = m.CreatedDate
                        }).OrderBy(c => c.CreatedDate).ToList();
                    vendorQuoteEntityModel.ListVendorQuoteDetail.ForEach(item =>
                    {
                        var product = commonProduct.FirstOrDefault(c => c.ProductId == item.ProductId);
                        item.ProductCode = product?.ProductCode ?? "";
                        item.ProductName = product?.ProductName ?? "";
                        item.ProductUnitName = listAllProductUnit.FirstOrDefault(c => c.CategoryId == product.ProductUnitId)?.CategoryName ?? "";
                    });

                    // get dữ liệu để xuất excel
                    var company = context.CompanyConfiguration.FirstOrDefault();
                    inforExportExcel.CompanyName = company.CompanyName;
                    inforExportExcel.Address = company.CompanyAddress;
                    inforExportExcel.Phone = company.Phone;
                    inforExportExcel.Website = "";
                    inforExportExcel.Email = company.Email;
                }

                var listVendor = context.Vendor.Where(c => c.Active == true)
                    .Select(m => new VendorEntityModel
                    {
                        VendorId = m.VendorId,
                        VendorGroupId = m.VendorGroupId,
                        PaymentId = m.PaymentId,
                        VendorCode = m.VendorCode,
                        VendorName = m.VendorName,
                        Active = m.Active,
                        CreatedById = m.CreatedById,
                        CreatedDate = m.CreatedDate
                    }).OrderBy(x => x.VendorName).ToList();

                var listProcurementRequest = context.ProcurementRequest
                    .Select(m => new ProcurementRequestEntityModel
                    {
                        ProcurementRequestId = m.ProcurementRequestId,
                        ProcurementCode = m.ProcurementCode,
                        ProcurementContent = m.ProcurementContent,
                        RequestEmployeeId = m.RequestEmployeeId,
                        RequestEmployeeName = "",
                        CreatedById = m.CreatedById,
                        CreatedDate = m.CreatedDate,
                    }).OrderBy(c => c.ProcurementCode).ToList();

                listProcurementRequest.ForEach(item =>
                {
                    item.RequestEmployeeName = commonEmployee.FirstOrDefault(c => c.EmployeeId == item.RequestEmployeeId)?.EmployeeName ?? "";

                });

                var listProcurementRequestItem = context.ProcurementRequestItem
                    .Select(m => new ProcurementRequestItemEntityModel
                    {
                        ProcurementRequestItemId = m.ProcurementRequestItemId,
                        ProductId = m.ProductId,
                        ProductName = "",
                        ProductCode = "",
                        VendorId = m.VendorId,
                        VendorName = "",
                        Unit = Guid.Empty,
                        UnitPrice = m.UnitPrice,
                        UnitName = "",
                        Quantity = m.Quantity,
                        ProcurementRequestId = m.ProcurementRequestId,
                        ProcurementPlanId = m.ProcurementPlanId,
                        CurrencyUnit = m.CurrencyUnit,
                        ExchangeRate = m.ExchangeRate,
                        Amount = 0,
                        QuantityApproval = m.QuantityApproval,
                        CreatedById = m.CreatedById,
                        CreatedDate = m.CreatedDate,

                    }).ToList();

                listProcurementRequestItem.ForEach(item =>
                {
                    var product = commonProduct.FirstOrDefault(c => c.ProductId == item.ProductId);
                    item.ProductCode = product?.ProductCode;
                    item.ProductName = product?.ProductName;
                });

                var listProduct = context.Product.Where(c => c.Active == true)
                     .Select(m => new ProductEntityModel
                     {
                         ProductId = m.ProductId,
                         ProductCode = m.ProductCode,
                         ProductName = m.ProductName,
                     }).ToList();

                var listEmployee = commonEmployee.Where(c => c.Active == true)
                    .Select(m => new EmployeeEntityModel
                    {
                        EmployeeId = m.EmployeeId,
                        EmployeeCode = m.EmployeeCode,
                        EmployeeName = m.EmployeeName
                    }).ToList();

                var listProductVendorMapping = context.ProductVendorMapping.Where(c => c.Active == true)
                    .Select(m => new ProductVendorMappingEntityModel
                    {
                        ProductVendorMappingId = m.ProductVendorMappingId,
                        ProductId = m.ProductId,
                        VendorId = m.VendorId
                    }).ToList();

                return new GetMasterDataCreateSuggestedSupplierQuoteResult
                {
                    StatusCode = HttpStatusCode.OK,
                    ListVendor = listVendor,
                    ListProcurementRequest = listProcurementRequest,
                    ListProcurementRequestItem = listProcurementRequestItem,
                    ListProduct = listProduct,
                    ListEmployee = listEmployee,
                    ListStatus = listAllStatus,
                    InforExportExcel = inforExportExcel,
                    ListProductVendorMapping = listProductVendorMapping,

                    SuggestedSupplierQuotes = vendorQuoteEntityModel
                };
            }
            catch (Exception ex)
            {
                return new GetMasterDataCreateSuggestedSupplierQuoteResult
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = ex.Message
                };
            }
        }

        public CreateOrUpdateSuggestedSupplierQuoteResult CreateOrUpdateSuggestedSupplierQuote(CreateOrUpdateSuggestedSupplierQuoteParameter parameter)
        {
            try
            {
                var suggestedSupplierQuote = new SuggestedSupplierQuotes();

                var statusTypeSupplierQuoteRequestId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "TBGV")?.CategoryTypeId ?? Guid.Empty;
                var listAllStatus = context.Category.Where(c => c.CategoryTypeId == statusTypeSupplierQuoteRequestId).ToList();

                var statusNewSupplierQuoteRequestId = listAllStatus.FirstOrDefault(c => c.CategoryCode == "MOI").CategoryId;

                var listSuggestedSupplierQuoteDetail = new List<SuggestedSupplierQuotesDetail>();
                if (parameter.SuggestedSupplierQuotes.SuggestedSupplierQuoteId != null && parameter.SuggestedSupplierQuotes.SuggestedSupplierQuoteId != Guid.Empty)
                {
                    var oldSuggestedSupplierRequest = context.SuggestedSupplierQuotes.FirstOrDefault(c => c.SuggestedSupplierQuoteId == parameter.SuggestedSupplierQuotes.SuggestedSupplierQuoteId);
                    using (var trasaction = context.Database.BeginTransaction())
                    {
                        // Xóa tất cả sản phẩm trong đề nghị báo giá nhà cung cấp
                        var listDeleteSupplierRequestDetail = context.SuggestedSupplierQuotesDetail.Where(c => c.SuggestedSupplierQuoteId == oldSuggestedSupplierRequest.SuggestedSupplierQuoteId).ToList();

                        context.SuggestedSupplierQuotesDetail.RemoveRange(listDeleteSupplierRequestDetail);
                        context.SuggestedSupplierQuotes.Remove(oldSuggestedSupplierRequest);
                        context.SaveChanges();

                        suggestedSupplierQuote = new SuggestedSupplierQuotes
                        {
                            SuggestedSupplierQuoteId = oldSuggestedSupplierRequest.SuggestedSupplierQuoteId,
                            SuggestedSupplierQuote = oldSuggestedSupplierRequest.SuggestedSupplierQuote,
                            VendorId = parameter.SuggestedSupplierQuotes.VendorId,
                            PersonInChargeId = parameter.SuggestedSupplierQuotes.PersonInChargeId,
                            RecommendedDate = parameter.SuggestedSupplierQuotes.RecommendedDate,
                            QuoteTermDate = parameter.SuggestedSupplierQuotes.QuoteTermDate,
                            ObjectType = oldSuggestedSupplierRequest.ObjectType,
                            ObjectId = oldSuggestedSupplierRequest.ObjectId,
                            Note = parameter.SuggestedSupplierQuotes.Note,
                            Active = true,
                            CreatedDate = oldSuggestedSupplierRequest.CreatedDate,
                            CreatedById = oldSuggestedSupplierRequest.CreatedById,
                            UpdatedById = parameter.UserId,
                            UpdatedDate = DateTime.Now,
                            StatusId = oldSuggestedSupplierRequest.StatusId,

                            ProcurementRequestId = parameter.SuggestedSupplierQuotes.ProcurementRequestId
                        };

                        context.SuggestedSupplierQuotes.Add(suggestedSupplierQuote);

                        if (parameter.ListSuggestedSupplierQuotesDetail.Count > 0)
                        {
                            parameter.ListSuggestedSupplierQuotesDetail.ForEach(item =>
                            {
                                var suggestedSupplierQuoteDetail = new SuggestedSupplierQuotesDetail
                                {
                                    SuggestedSupplierQuoteDetailId = Guid.NewGuid(),
                                    SuggestedSupplierQuoteId = oldSuggestedSupplierRequest.SuggestedSupplierQuoteId,
                                    ProductId = item.ProductId,
                                    Quantity = item.Quantity,
                                    Note = item.Note,
                                    Active = true,
                                    CreatedById = parameter.UserId,
                                    CreatedDate = DateTime.Now
                                };
                                listSuggestedSupplierQuoteDetail.Add(suggestedSupplierQuoteDetail);
                            });
                            context.SuggestedSupplierQuotesDetail.AddRange(listSuggestedSupplierQuoteDetail);
                        }

                        context.SaveChanges();
                        trasaction.Commit();
                    }

                    return new CreateOrUpdateSuggestedSupplierQuoteResult
                    {
                        StatusCode = HttpStatusCode.OK,
                        MessageCode = CommonMessage.SuggestedSupplierQuoteRequest.EDIT_SUCCESS,
                        SuggestedSupplierQuoteId = suggestedSupplierQuote.SuggestedSupplierQuoteId,
                    };
                }
                else
                {
                    suggestedSupplierQuote = new SuggestedSupplierQuotes
                    {
                        SuggestedSupplierQuoteId = Guid.NewGuid(),
                        SuggestedSupplierQuote = GenerateoSuggestedSupplierQuoteRequestCode(),
                        VendorId = parameter.SuggestedSupplierQuotes.VendorId,
                        PersonInChargeId = parameter.SuggestedSupplierQuotes.PersonInChargeId,
                        RecommendedDate = parameter.SuggestedSupplierQuotes.RecommendedDate,
                        QuoteTermDate = parameter.SuggestedSupplierQuotes.QuoteTermDate,
                        ObjectType = null,
                        ObjectId = null,
                        Note = parameter.SuggestedSupplierQuotes.Note,
                        Active = true,
                        CreatedDate = DateTime.Now,
                        CreatedById = parameter.UserId,
                        ProcurementRequestId = parameter.SuggestedSupplierQuotes.ProcurementRequestId,
                        StatusId = statusNewSupplierQuoteRequestId,
                    };

                    context.SuggestedSupplierQuotes.Add(suggestedSupplierQuote);

                    if (parameter.ListSuggestedSupplierQuotesDetail.Count > 0)
                    {
                        parameter.ListSuggestedSupplierQuotesDetail.ForEach(item =>
                        {
                            var suggestedSupplierQuoteDetail = new SuggestedSupplierQuotesDetail
                            {
                                SuggestedSupplierQuoteDetailId = Guid.NewGuid(),
                                SuggestedSupplierQuoteId = suggestedSupplierQuote.SuggestedSupplierQuoteId,
                                ProductId = item.ProductId,
                                Quantity = item.Quantity,
                                Note = item.Note,
                                Active = true,
                                CreatedById = parameter.UserId,
                                CreatedDate = DateTime.Now
                            };
                            listSuggestedSupplierQuoteDetail.Add(suggestedSupplierQuoteDetail);
                        });
                        context.SuggestedSupplierQuotesDetail.AddRange(listSuggestedSupplierQuoteDetail);
                    }
                    context.SaveChanges();

                    return new CreateOrUpdateSuggestedSupplierQuoteResult
                    {
                        StatusCode = HttpStatusCode.OK,
                        MessageCode = CommonMessage.SuggestedSupplierQuoteRequest.CREATE_SUCCESS,
                        SuggestedSupplierQuoteId = suggestedSupplierQuote.SuggestedSupplierQuoteId,
                    };
                }
            }
            catch (Exception ex)
            {
                return new CreateOrUpdateSuggestedSupplierQuoteResult
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = ex.Message,
                };
            }
        }

        private string GenerateoSuggestedSupplierQuoteRequestCode()
        {
            // sửa định dạng gen code thành "DNBG-yyMMdd + 4 số"
            var todayQuotes = context.SuggestedSupplierQuotes.Where(w => w.CreatedDate.Date == DateTime.Now.Date)
                                                .OrderByDescending(w => w.CreatedDate)
                                                .ToList();

            var count = todayQuotes.Count() == 0 ? 0 : todayQuotes.Count();
            string currentYear = DateTime.Now.Year.ToString();
            var temp = "DNBG";
            string result = temp + currentYear.Substring(currentYear.Length - 2) + DateTime.Now.Month.ToString("00") + DateTime.Now.Day.ToString("00") + (count + 1).ToString("D4");
            return result;
        }

        public DeleteSuggestedSupplierQuoteRequestResult DeleteSuggestedSupplierQuoteRequest(DeleteSugestedSupplierQuoteRequestParameter parameter)
        {
            try
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    var deleteQuoteRequest = context.SuggestedSupplierQuotes.FirstOrDefault(c => c.SuggestedSupplierQuoteId == parameter.SuggestedSupplierQuoteId);
                    var deleteQuoteRequestItem = context.SuggestedSupplierQuotesDetail.Where(c => c.SuggestedSupplierQuoteId == parameter.SuggestedSupplierQuoteId).ToList();

                    context.SuggestedSupplierQuotesDetail.RemoveRange(deleteQuoteRequestItem);
                    context.SuggestedSupplierQuotes.Remove(deleteQuoteRequest);

                    context.SaveChanges();
                    transaction.Commit();
                }

                return new DeleteSuggestedSupplierQuoteRequestResult
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = CommonMessage.SuggestedSupplierQuoteRequest.DELETE_SUCCESS
                };
            }
            catch (Exception ex)
            {
                return new DeleteSuggestedSupplierQuoteRequestResult
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = ex.Message
                };
            }
        }

        public ChangeStatusVendorQuoteResult ChangeStatusVendorQuote(ChangeStatusVendorQuoteParameter parameter)
        {
            try
            {
                var oldSuggestedSupplierQuote = context.SuggestedSupplierQuotes.FirstOrDefault(c => c.SuggestedSupplierQuoteId == parameter.SuggestedSupplierQuoteId);

                var statusTypeSupplierQuoteRequestId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "TBGV")?.CategoryTypeId ?? Guid.Empty;

                var listAllStatus = context.Category.Where(c => c.CategoryTypeId == statusTypeSupplierQuoteRequestId).ToList();
                var statusNewStatusId = listAllStatus.FirstOrDefault(c => c.CategoryCode == "MOI").CategoryId;
                var statusSuggestionStatusId = listAllStatus.FirstOrDefault(c => c.CategoryCode == "DNG").CategoryId;
                var statusDestroyStatusId = listAllStatus.FirstOrDefault(c => c.CategoryCode == "HUY").CategoryId;

                if (parameter.StatusId == statusNewStatusId)
                {
                    if (oldSuggestedSupplierQuote.StatusId != statusDestroyStatusId)
                    {
                        return new ChangeStatusVendorQuoteResult
                        {
                            StatusCode = HttpStatusCode.ExpectationFailed,
                            MessageCode = CommonMessage.SuggestedSupplierQuoteRequest.CHANGE_STATUS_FAIL
                        };
                    }
                }

                if (parameter.StatusId == statusSuggestionStatusId)
                {
                    if (oldSuggestedSupplierQuote.StatusId != statusNewStatusId)
                    {
                        return new ChangeStatusVendorQuoteResult
                        {
                            StatusCode = HttpStatusCode.ExpectationFailed,
                            MessageCode = CommonMessage.SuggestedSupplierQuoteRequest.CHANGE_STATUS_FAIL
                        };
                    }
                }

                if (parameter.StatusId == statusDestroyStatusId)
                {
                    if (oldSuggestedSupplierQuote.StatusId != statusSuggestionStatusId)
                    {
                        return new ChangeStatusVendorQuoteResult
                        {
                            StatusCode = HttpStatusCode.ExpectationFailed,
                            MessageCode = CommonMessage.SuggestedSupplierQuoteRequest.CHANGE_STATUS_FAIL
                        };
                    }
                }

                oldSuggestedSupplierQuote.StatusId = parameter.StatusId;
                oldSuggestedSupplierQuote.UpdatedById = parameter.UserId;
                oldSuggestedSupplierQuote.UpdatedDate = DateTime.Now;

                context.SuggestedSupplierQuotes.Update(oldSuggestedSupplierQuote);
                context.SaveChanges();

                return new ChangeStatusVendorQuoteResult
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = CommonMessage.SuggestedSupplierQuoteRequest.CHANGE_STATUS_SUCCESS,
                };
            }
            catch (Exception ex)
            {
                return new ChangeStatusVendorQuoteResult
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = ex.Message
                };
            }
        }

        /// <summary>
        ///     Validate and email address
        ///     It must be follow these rules:
        ///     Has only one @ character
        ///     Has at least 3 chars after the @
        ///     Domain portion contains at least one dot
        ///     Dot can't be before or immediately after the @ character
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <returns>True: If valid, False: If not</returns>
        private static bool ValidateEmailAddress(string emailAddress)
        {
            if (string.IsNullOrEmpty(emailAddress)) return false;

            if (!Regex.IsMatch(emailAddress, "^[-A-Za-z0-9_@.]+$")) return false;

            // Search for the @ char
            var i = emailAddress.IndexOf("@", StringComparison.Ordinal);

            // There must be at least 3 chars after the @
            if (i <= 0 || i >= emailAddress.Length - 3) return false;

            // Ensure there is only one @
            if (emailAddress.IndexOf("@", i + 1, StringComparison.Ordinal) > 0) return false;


            // Check the domain portion contains at least one dot
            var j = emailAddress.LastIndexOf(".", StringComparison.Ordinal);

            // It can't be before or immediately after the @ character
            //if (j < 0 || j <= i + 1) return false;
            var before = emailAddress.Substring(0, i);
            var after = emailAddress.Substring(i + 1);
            if (before.LastIndexOf(".", StringComparison.Ordinal) == before.Length - 1) return false;
            if (after.IndexOf(".", StringComparison.Ordinal) == 0) return false;

            // EmailAddress is validated
            return true;
        }


        public SendEmailVendorQuoteResult SendEmailVendorQuote(SendMailVendorQuoteParameter parameter)
        {
            try
            {
                var suggestedSupplierQuote = context.SuggestedSupplierQuotes.FirstOrDefault(q => q.SuggestedSupplierQuoteId == parameter.SuggestedSupplierQuoteId);
                suggestedSupplierQuote.IsSend = true;

                #region Attachments file pdf tu localStorage (longhdh cmt)

                //var now = DateTime.Now;
                //var _day = now.Day.ToString("D2");
                //var _month = now.Month.ToString("D2");
                //var _year = (now.Year % 100).ToString();

                //string folderName = "Đề nghị báo giá_" + suggestedSupplierQuote.SuggestedSupplierQuote + ".pdf";
                //string webRootPath = _hostingEnvironment.WebRootPath + "\\ExportedPDFQuote\\";
                //if (!Directory.Exists(webRootPath))
                //{
                //    Directory.CreateDirectory(webRootPath);
                //}
                //string newPath = Path.Combine(webRootPath, folderName);

                //if (!File.Exists(newPath))
                //{
                //    Directory.Delete(webRootPath, true);
                //    Directory.CreateDirectory(webRootPath);

                //    byte[] imageBytes = Convert.FromBase64String(parameter.Base64Pdf);
                //    MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);

                //    using (var stream = new FileStream(newPath, FileMode.Create))
                //    {
                //        ms.CopyTo(stream);
                //    }
                //}

                //parameter.ListEmail.ForEach(item =>
                //{

                //Attachment attachment = new Attachment(newPath);

                //MailMessage mail = new MailMessage();
                //SmtpClient SmtpServer = new SmtpClient(PrimaryDomain, PrimaryPort);
                //mail.From = new MailAddress(Email, "N8");
                //mail.To.Add(item); // Email người nhận
                //mail.Subject = string.Format(parameter.TitleEmail);
                //mail.Body = parameter.ContentEmail;
                //mail.Attachments.Add(attachment);
                //mail.IsBodyHtml = true;
                //SmtpServer.Credentials = new System.Net.NetworkCredential(Email, Password);
                //SmtpServer.EnableSsl = Ssl != null ? bool.Parse(Ssl) : false;
                //SmtpServer.Send(mail);

                //Emailer.SendEmail(context, new []{item}, new List<string>(), string.Format(parameter.TitleEmail), parameter.ContentEmail);

                //});

                #endregion

                List<string> path = new List<string>();
                var folder = context.Folder.FirstOrDefault(x => x.Active == true && x.FolderType == "QLNCC");
                if (folder == null)
                {
                    return new SendEmailVendorQuoteResult()
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = "Chưa có thư mục để lưu. Bạn phải cấu hình thư mục."
                    };
                }

                if (parameter.ListFormFile != null && parameter.ListFormFile.Count > 0)
                {
                    var folderNameAttachments = folder.Url + "\\";
                    string webRootPathAttachments = _hostingEnvironment.WebRootPath;
                    string newPathAttachments = Path.Combine(webRootPathAttachments, folderNameAttachments);
                    if (!Directory.Exists(newPathAttachments))
                    {
                        Directory.CreateDirectory(newPathAttachments);
                    }
                    foreach (IFormFile file in parameter.ListFormFile)
                    {
                        if (file.Length > 0)
                        {
                            string fileName = file.FileName.Trim();

                            var fileInFolder = new FileInFolder();
                            fileInFolder.Active = true;
                            fileInFolder.CreatedById = parameter.UserId;
                            fileInFolder.CreatedDate = DateTime.Now;
                            fileInFolder.FileExtension = fileName.Substring(fileName.LastIndexOf(".") + 1);
                            fileInFolder.FileInFolderId = Guid.NewGuid();
                            fileInFolder.FileName =
                                fileName.Substring(0, fileName.LastIndexOf(".")) + "_" + Guid.NewGuid();
                            fileInFolder.FolderId = folder.FolderId;
                            fileInFolder.ObjectId = parameter.SuggestedSupplierQuoteId;
                            fileInFolder.ObjectType = "DNBGNCC";
                            fileInFolder.Size = file.Length.ToString();

                            context.FileInFolder.Add(fileInFolder);
                            fileName = fileInFolder.FileName + "." + fileInFolder.FileExtension;
                            string fullPath = Path.Combine(newPathAttachments, fileName);
                            using (var stream = new FileStream(fullPath, FileMode.Create))
                            {
                                file.CopyTo(stream);
                                path.Add(fullPath);
                            }
                        }
                    }
                }

                GetConfiguration();
                var listInvalidEmail = new List<string>();

                var listSenTo = new List<string>();
                parameter.ListEmail?.ForEach(item =>
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        if (ValidateEmailAddress(item.Trim()))
                        {
                            listSenTo.Add(item.Trim());
                        }
                        else
                        {
                            listInvalidEmail.Add(item.Trim());
                        }
                    }
                });

                var listSenToCC = new List<string>();
                parameter.ListEmailCC?.ForEach(item =>
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        if (ValidateEmailAddress(item.Trim()))
                        {
                            listSenToCC.Add(item.Trim());
                        }
                        else
                        {
                            listInvalidEmail.Add(item.Trim());
                        }
                    }
                });


                var listSenToBcc = new List<string>();
                parameter.ListEmailBcc?.ForEach(item =>
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        if (ValidateEmailAddress(item.Trim()))
                        {
                            listSenToBcc.Add(item.Trim());
                        }
                        else
                        {
                            listInvalidEmail.Add(item.Trim());
                        }
                    }
                });

                if (listSenTo.Count > 0)
                {
                    var empId = context.User.FirstOrDefault(x => x.UserId == parameter.UserId)?.EmployeeId;
                    var empEmail = context.Contact.FirstOrDefault(x => x.ObjectId == empId && x.ObjectType == "EMP")?.Email;

                    if (empEmail != null)
                    {


                        var configEntity = context.SystemParameter.ToList();

                        // replace token
                        var emailContent = ReplaceTokenForContent(context, suggestedSupplierQuote, parameter.ContentEmail, configEntity, parameter.UserId);
                        var emailTitle = ReplaceTokenForContent(context, suggestedSupplierQuote, parameter.TitleEmail, configEntity, parameter.UserId);

                        // Emailer.SendEmailWithAttachments(context, Email, listSenTo.Distinct().ToList(), listSenToCC.Distinct().ToList(), listSenToBcc.Distinct().ToList(),
                        //     string.Format(parameter.TitleEmail), parameter.ContentEmail, path);
                        Emailer.SendEmailWithAttachments(context, Email, listSenTo.Distinct().ToList(), listSenToCC.Distinct().ToList(), listSenToBcc.Distinct().ToList(),
                            emailTitle, emailContent, path);

                    }
                    else
                    {
                        return new SendEmailVendorQuoteResult
                        {
                            StatusCode = HttpStatusCode.ExpectationFailed,
                            MessageCode = "Đã có lỗi xảy ra khi lấy Email!"
                        };
                    }

                }
                else
                {
                    return new SendEmailVendorQuoteResult
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = "Gửi email không thành công. Email nhận khồng hợp lệ",
                        listInvalidEmail = listInvalidEmail,
                    };
                }



                context.SuggestedSupplierQuotes.Update(suggestedSupplierQuote);

                Note note = new Note
                {
                    NoteId = Guid.NewGuid(),
                    ObjectType = "VENDORQUOTE",
                    ObjectId = suggestedSupplierQuote.SuggestedSupplierQuoteId,
                    Type = "ADD",
                    Active = true,
                    CreatedById = parameter.UserId,
                    CreatedDate = DateTime.Now,
                    NoteTitle = "Đã thêm ghi chú",
                    Description = "Gửi mail báo giá nhà cung cấp thành công"
                };

                context.Note.Add(note);
                context.SaveChanges();

                return new SendEmailVendorQuoteResult
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Gửi mail thành công",
                    listInvalidEmail = listInvalidEmail,
                };
            }
            catch (Exception ex)
            {
                return new SendEmailVendorQuoteResult
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = ex.Message
                };
            }
        }

        public RemoveVendorOrderResult RemoveVendorOrder(RemoveVendorOrderParameter parameter)
        {
            try
            {
                var vendorOrder = context.VendorOrder.FirstOrDefault(x => x.VendorOrderId == parameter.VendorOrderId);

                if (vendorOrder == null)
                {
                    return new RemoveVendorOrderResult()
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = "Đơn hàng mua không tồn tại trên hệ thống"
                    };
                }

                var listVendorOrderDetail = context.VendorOrderDetail
                    .Where(x => x.VendorOrderId == parameter.VendorOrderId).ToList();

                var listVendorOrderDetailId = listVendorOrderDetail.Select(y => y.VendorOrderDetailId).ToList();

                var listVendorOrderProductDetailProductAttributeValue = context
                    .VendorOrderProductDetailProductAttributeValue
                    .Where(x => listVendorOrderDetailId.Contains(x.VendorOrderDetailId)).ToList();

                var listVendorOrderProcurementRequestMapping = context.VendorOrderProcurementRequestMapping
                    .Where(x => x.VendorOrderId == parameter.VendorOrderId).ToList();

                var listVendorOrderCostDetail = context.VendorOrderCostDetail
                    .Where(x => x.VendorOrderId == parameter.VendorOrderId).ToList();

                var listNote = context.Note
                    .Where(x => x.ObjectId == parameter.VendorOrderId && x.ObjectType == "VENDORORDER").ToList();

                var listNoteId = listNote.Select(y => y.NoteId).ToList();

                var listFile = context.FileInFolder
                    .Where(x => listNoteId.Contains(x.ObjectId.Value) && x.ObjectType == "NOTE").ToList();

                //Xóa data trong các bảng reference
                context.FileInFolder.RemoveRange(listFile);
                context.Note.RemoveRange(listNote);
                context.VendorOrderCostDetail.RemoveRange(listVendorOrderCostDetail);
                context.VendorOrderProcurementRequestMapping.RemoveRange(listVendorOrderProcurementRequestMapping);
                context.VendorOrderProductDetailProductAttributeValue.RemoveRange(
                    listVendorOrderProductDetailProductAttributeValue);
                context.VendorOrderDetail.RemoveRange(listVendorOrderDetail);
                context.VendorOrder.Remove(vendorOrder);

                //Lấy url vật lý của các file
                string webRootPath = _hostingEnvironment.WebRootPath;
                webRootPath = _hostingEnvironment.WebRootPath + "\\";

                var listAllFolder = context.Folder.ToList();
                var listPhysicalPathDelete = new List<string>();
                listFile.ForEach(file =>
                {
                    var urlFolder = listAllFolder.FirstOrDefault(x => x.FolderId == file.FolderId).Url;
                    var fileName = file.FileName + "." + file.FileExtension;

                    var physicalPathFolder = ConvertFolderUrl(urlFolder);
                    var physicalPathFile = Path.Combine(webRootPath, physicalPathFolder + "\\" + fileName);
                    listPhysicalPathDelete.Add(physicalPathFile);
                });

                bool isUsing = false;
                for (int i = 0; i <= listPhysicalPathDelete.Count - 1; i++)
                {
                    FileInfo fi = new FileInfo(listPhysicalPathDelete[i]);
                    isUsing = CheckFileLocked(fi);

                    if (isUsing) break;
                }

                if (isUsing)
                {
                    return new RemoveVendorOrderResult()
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = "Hãy đóng hết file đính kèm trước khi xóa"
                    };
                }

                listPhysicalPathDelete.ForEach(file =>
                {
                    File.Delete(file);
                });

                context.SaveChanges();

                #region Lưu nhật ký hệ thống

                LogHelper.AuditTrace(context, ActionName.DELETE, ObjectName.VENDORORDER, parameter.VendorOrderId, parameter.UserId);

                #endregion

                return new RemoveVendorOrderResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success"
                };
            }
            catch (Exception e)
            {
                return new RemoveVendorOrderResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public bool CheckFileLocked(FileInfo file)
        {
            try
            {
                using (FileStream stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    stream.Close();
                }
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }

            //file is not locked
            return false;
        }

        public string ConvertFolderUrl(string url)
        {
            var stringResult = url.Split(@"\");
            string result = "";
            for (int i = 0; i < stringResult.Length; i++)
            {
                result = result + stringResult[i] + "\\";
            }

            result = result.Substring(0, result.Length - 1);

            return result;
        }

        public CancelVendorOrderResult CancelVendorOrder(CancelVendorOrderParameter parameter)
        {
            try
            {
                var vendorOrder = context.VendorOrder.FirstOrDefault(x => x.VendorOrderId == parameter.VendorOrderId);

                if (vendorOrder == null)
                {
                    return new CancelVendorOrderResult()
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = "Đơn hàng mua không tồn tại trên hệ thống"
                    };
                }

                //Đổi trạng thái đơn hàng mua sang Hủy
                var cancelStatus = context.PurchaseOrderStatus.FirstOrDefault(x => x.PurchaseOrderStatusCode == "CAN")
                    .PurchaseOrderStatusId;

                vendorOrder.StatusId = cancelStatus;
                context.VendorOrder.Update(vendorOrder);
                context.SaveChanges();

                return new CancelVendorOrderResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success"
                };
            }
            catch (Exception e)
            {
                return new CancelVendorOrderResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public DraftVendorOrderResult DraftVendorOrder(DraftVendorOrderParameter parameter)
        {
            try
            {
                var vendorOrder = context.VendorOrder.FirstOrDefault(x => x.VendorOrderId == parameter.VendorOrderId);

                if (vendorOrder == null)
                {
                    return new DraftVendorOrderResult()
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = "Đơn hàng mua không tồn tại trên hệ thống"
                    };
                }

                //Đổi trạng thái đơn hàng mua sang Mới tạo
                var draftStatus = context.PurchaseOrderStatus.FirstOrDefault(x => x.PurchaseOrderStatusCode == "DRA")
                    .PurchaseOrderStatusId;

                vendorOrder.StatusId = draftStatus;
                vendorOrder.UpdatedById = parameter.UserId;
                vendorOrder.UpdatedDate = DateTime.Now;

                context.VendorOrder.Update(vendorOrder);
                context.SaveChanges();

                #region gửi mail thông báo
                if (parameter.IsCancelApproval)
                {

                    //var configEntity = context.SystemParameter.ToList();

                    //var emailTempCategoryTypeId = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "TMPE").CategoryTypeId;

                    //var listEmailTempType =
                    //    context.Category.Where(x => x.CategoryTypeId == emailTempCategoryTypeId).ToList();

                    //var emailCategoryId = listEmailTempType.FirstOrDefault(w => w.CategoryCode == "VOCA") // VENDOR_ORDER_CANCEL_APPROVAL
                    //    .CategoryId;

                    //var emailTemplate = context.EmailTemplate.FirstOrDefault(w => w.Active && w.EmailTemplateTypeId == emailCategoryId);

                    //#region List reference để kiểm tra điều kiện và thông tin dùng để gửi email

                    //var listAllEmployee = context.Employee.Where(x => x.Active == true).ToList();
                    //var listAllContact = context.Contact.Where(x => x.Active == true && x.ObjectType == "EMP").ToList();

                    //#endregion

                    //#region Lấy danh sách email cần gửi thông báo

                    //var listEmailSendTo = new List<string>();

                    //#region Lấy email người phê duyệt

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

                    //#endregion

                    //#region Lấy email người tạo

                    //var employeeId =
                    //    context.User.FirstOrDefault(x => x.UserId == vendorOrder.CreatedById)
                    //        ?.EmployeeId;

                    //var email_created = "";

                    //if (employeeId != null)
                    //{
                    //    email_created = listAllContact.FirstOrDefault(x =>
                    //        x.ObjectId == employeeId && x.ObjectType == "EMP")?.Email;

                    //    if (!String.IsNullOrEmpty(email_created))
                    //    {
                    //        listEmailSendTo.Add(email_created.Trim());
                    //    }
                    //}

                    //#endregion

                    //#region Lấy email người hủy phê duyệt

                    //var empId =
                    //    context.User.FirstOrDefault(x => x.UserId == vendorOrder.UpdatedById)
                    //        ?.EmployeeId;

                    //var email_cancel = "";

                    //if (employeeId != null)
                    //{
                    //    email_cancel = listAllContact.FirstOrDefault(x =>
                    //        x.ObjectId == employeeId && x.ObjectType == "EMP")?.Email;

                    //    if (!String.IsNullOrEmpty(email_cancel))
                    //    {
                    //        listEmailSendTo.Add(email_cancel.Trim());
                    //    }
                    //}

                    //#endregion

                    //listEmailSendTo = listEmailSendTo.Distinct().ToList();

                    //#endregion

                    //var subject = ReplaceTokenForContent(context, vendorOrder, emailTemplate.EmailTemplateTitle,
                    //    configEntity);
                    //var content = ReplaceTokenForContent(context, vendorOrder, emailTemplate.EmailTemplateContent,
                    //    configEntity);

                    //Emailer.SendEmail(context, listEmailSendTo, new List<string>(), new List<string>(), subject, content);
                    NotificationHelper.AccessNotification(context, TypeModel.VendorOrderDetail, "CANCEL_APPROVAL", new VendorOrder(),
                        vendorOrder, true);


                }
                #endregion

                return new DraftVendorOrderResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success"
                };
            }
            catch (Exception e)
            {
                return new DraftVendorOrderResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        #region Phần hỗ trợ gửi mail

        private static string ReplaceTokenForContent(TNTN8Context context, object model,
            string emailContent, List<SystemParameter> configEntity, Guid userId)
        {
            var result = emailContent;

            #region Common Token

            const string UpdatedDate = "[UPDATED_DATE]";

            const string Url_Login = "[URL]";

            const string Logo = "[LOGO]";

            const string OrderCode = "[ORDER_CODE]";

            const string EmployeeName = "[EMPLOYEE_NAME]";
            const string EmployeeCode = "[EMPLOYEE_CODE]";
            const string employeeEmail = "[EMPLOYEE_EMAIL]"; // email nhan vien
            const string employeePhone = "[EMPLOYEE_PHONE]"; // sdt nhan vien


            const string listProduct = "[LIST_PRODUCT]"; // danh sach san pham

            const string companyWebsite = "[COMPANY_WEBSITE]"; // Website công ty
            const string companyAddress = "[COMPANY_ADDRESS]"; // dia chi cong ty
            const string companyEmail = "[COMPANY_EMAIL]"; // email cong ty
            const string companyName = "[COMPANY_NAME]"; // ten cong ty
            const string companyPhone = "[COMPANY_PHONE]"; // sdt cong ty

            #endregion

            var _model = model as SuggestedSupplierQuotes;

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

            // #region replace order code
            //
            // if (result.Contains(OrderCode) && _model.OrderCode != null)
            // {
            //     result = result.Replace(OrderCode, _model.OrderCode.Trim());
            // }
            //
            // #endregion

            #region replace change employee send mail

            var employeeId = context.User.FirstOrDefault(x => x.UserId == userId)?.EmployeeId;

            if (result.Contains(EmployeeName))
            {
                var employeeName = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeId)?.EmployeeName;

                result = result.Replace(EmployeeName, !string.IsNullOrEmpty(employeeName) ? employeeName : "");
            }

            if (result.Contains(EmployeeCode))
            {
                var employeeCode = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeId)?.EmployeeCode;

                result = result.Replace(EmployeeCode, !string.IsNullOrEmpty(employeeCode) ? employeeCode : "");
            }

            if (result.Contains(employeeEmail))
            {
                var email = context.Contact.FirstOrDefault(x => x.ObjectId == employeeId && x.ObjectType == "EMP")?.Email;

                result = result.Replace(employeeEmail, !string.IsNullOrEmpty(email) ? email : "");
            }

            if (result.Contains(employeePhone))
            {
                var phone = context.Contact.FirstOrDefault(x => x.ObjectId == employeeId && x.ObjectType == "EMP")?.Phone;

                result = result.Replace(employeePhone, !string.IsNullOrEmpty(phone) ? phone : "");
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

            #region replace company infor token

            var company = context.CompanyConfiguration.FirstOrDefault();

            if (result.Contains(companyWebsite))
            {
                if (!string.IsNullOrEmpty(company?.Website))
                {
                    result = result.Replace(companyWebsite, company?.Website);
                }
                else
                {
                    result = result.Replace(companyWebsite, "");
                }
            }

            if (result.Contains(companyAddress))
            {
                if (!string.IsNullOrEmpty(company?.CompanyAddress))
                {
                    result = result.Replace(companyAddress, company?.CompanyAddress);
                }
                else
                {
                    result = result.Replace(companyAddress, "");
                }
            }

            if (result.Contains(companyEmail))
            {
                if (!string.IsNullOrEmpty(company?.Email))
                {
                    result = result.Replace(companyEmail, company?.Email);
                }
                else
                {
                    result = result.Replace(companyEmail, "");
                }
            }

            if (result.Contains(companyName))
            {
                if (!string.IsNullOrEmpty(company?.CompanyName))
                {
                    result = result.Replace(companyName, company?.CompanyName);
                }
                else
                {
                    result = result.Replace(companyName, "");
                }
            }

            if (result.Contains(companyPhone))
            {
                if (!string.IsNullOrEmpty(company?.Phone))
                {
                    result = result.Replace(companyPhone, company?.Phone);
                }
                else
                {
                    result = result.Replace(companyPhone, "");
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

        public GetMasterDataVendorOrderReportResult GetMasterDataVendorOrderReport(GetMasterDataVendorOrderReportParameter parameter)
        {
            try
            {
                var listVendor = new List<VendorEntityModel>();
                listVendor = context.Vendor.Where(x => x.Active == true).Select(y => new VendorEntityModel
                {
                    VendorId = y.VendorId,
                    VendorCode = y.VendorCode,
                    VendorName = y.VendorName
                }).OrderBy(z => z.VendorName).ToList();

                var listStatusEntityModel = new List<PurchaseOrderStatusEntityModel>();
                var listStatus = context.PurchaseOrderStatus.Where(x => x.Active).OrderBy(z => z.Description).ToList();
                listStatus.ForEach(item =>
                {
                    listStatusEntityModel.Add(new PurchaseOrderStatusEntityModel(item));
                });
                var listProcurementRequest = new List<ProcurementRequestEntityModel>();
                listProcurementRequest = context.ProcurementRequest.Where(x => x.Active == true).Select(y =>
                    new ProcurementRequestEntityModel
                    {
                        ProcurementRequestId = y.ProcurementRequestId,
                        ProcurementCode = y.ProcurementCode,
                        CreatedDate = y.CreatedDate
                    }).OrderByDescending(z => z.CreatedDate).ToList();

                var listEmployee = new List<EmployeeEntityModel>();
                listEmployee = context.Employee.Select(y => new EmployeeEntityModel
                {
                    EmployeeId = y.EmployeeId,
                    EmployeeCode = y.EmployeeCode,
                    EmployeeName = y.EmployeeName
                }).OrderBy(z => z.EmployeeName).ToList();

                return new GetMasterDataVendorOrderReportResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success",
                    ListVendor = listVendor,
                    ListStatus = listStatusEntityModel,
                    ListProcurementRequest = listProcurementRequest,
                    ListEmployee = listEmployee
                };
            }
            catch (Exception e)
            {
                return new GetMasterDataVendorOrderReportResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public SearchVendorOrderReportResult SearchVendorOrderReport(SearchVendorOrderReportParameter parameter)
        {
            try
            {
                var listVendorOrderReport = new List<VendorOrderReportEntityModel>();

                var listVendorOrder = context.VendorOrder.Where(x =>
                    (parameter.VendorOrderCode == "" || parameter.VendorOrderCode == null ||
                     x.VendorOrderCode.Contains(parameter.VendorOrderCode))
                    && (parameter.ListSelectedVendorId == null || parameter.ListSelectedVendorId.Count == 0 ||
                        parameter.ListSelectedVendorId.Contains(x.VendorId))
                    && (parameter.ListSelectedStatusId == null || parameter.ListSelectedStatusId.Count == 0 ||
                        parameter.ListSelectedStatusId.Contains(x.StatusId))
                    && (parameter.FromDate == null || parameter.FromDate == DateTime.MinValue ||
                        parameter.FromDate.Value.Date >= x.VendorOrderDate.Date)
                    && (parameter.ToDate == null || parameter.ToDate == DateTime.MinValue ||
                        parameter.ToDate.Value.Date <= x.VendorOrderDate.Date)
                    && (parameter.Description == null || parameter.Description == "" ||
                        x.Description.Contains(parameter.Description))
                    && (parameter.ListSelectedEmployeeId == null || parameter.ListSelectedEmployeeId.Count == 0 ||
                        parameter.ListSelectedEmployeeId.Contains(x.Orderer.Value))).ToList();

                //Với điều kiện Hợp đồng mua (Chưa làm hợp đồng mua nên chưa sử dụng)

                var listVendorOrderId = listVendorOrder.Select(y => y.VendorOrderId).ToList();

                var listVendorOrderDetail = context.VendorOrderDetail
                    .Where(x => listVendorOrderId.Contains(x.VendorOrderId) &&
                                (parameter.ListSelectedProcurementRequestId == null ||
                                 parameter.ListSelectedProcurementRequestId.Count == 0 ||
                                 parameter.ListSelectedProcurementRequestId.Contains(x.ProcurementRequestId.Value)))
                    .ToList();

                var listProductId = listVendorOrderDetail.Select(y => y.ProductId).Distinct().ToList();
                var listProduct = context.Product.Where(x => listProductId.Contains(x.ProductId)
                                                             && (parameter.ProductCode == null ||
                                                                 parameter.ProductCode == "" ||
                                                                 x.ProductCode.ToLower()
                                                                     .Contains(parameter.ProductCode.ToLower())))
                    .ToList();

                var listFilterProductId = new List<Guid>();

                if (parameter.ProductCode != null && parameter.ProductCode != "")
                {
                    listFilterProductId = listProduct.Select(y => y.ProductId).ToList();

                    listVendorOrderDetail = listVendorOrderDetail
                        .Where(x => listFilterProductId.Contains(x.ProductId.Value)).ToList();
                }

                var unitType = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "DNH");
                var listUnit = context.Category.Where(x => x.CategoryTypeId == unitType.CategoryTypeId).ToList();

                var listStatus = context.PurchaseOrderStatus.ToList();

                // Lấy thêm thông tin nhà cung cấp mỗi đơn hàng
                var listVendorId = listVendorOrder.Select(w => w.VendorId).Distinct() ?? new List<Guid>();
                var listVendorEntity = context.Vendor.Where(w => listVendorId.Contains(w.VendorId));

                listVendorOrderDetail.ForEach(item =>
                {
                    var vendorOrder = listVendorOrder.FirstOrDefault(x => x.VendorOrderId == item.VendorOrderId);
                    var product = listProduct.FirstOrDefault(x => x.ProductId == item.ProductId);
                    var productName = item.OrderDetailType == 1 ? item.Description : product?.ProductName;
                    var unit = listUnit.FirstOrDefault(x => x.CategoryId == item.UnitId);
                    var unitName = item.OrderDetailType == 1 ? item.IncurredUnit : unit?.CategoryName;
                    var status = listStatus.FirstOrDefault(x => x.PurchaseOrderStatusId == vendorOrder.StatusId);
                    var vendor = listVendorEntity.First(f => f.VendorId == item.VendorId);

                    var result = new VendorOrderReportEntityModel();
                    result.Stt = 0;
                    result.VendorOrderId = item.VendorOrderId;
                    result.VendorOrderCode = vendorOrder.VendorOrderCode;
                    result.VendorOrderDate = vendorOrder.VendorOrderDate;
                    result.Description = vendorOrder.Description != null ? vendorOrder.Description : "";
                    result.ProductName = productName;
                    result.ProductCode = item.OrderDetailType == 1 ? "" : product?.ProductCode;
                    result.UnitName = unitName;
                    result.Quantity = item.Quantity.Value;
                    result.StatusName = status.Description;
                    result.BackgroundColorForStatus = "";
                    result.VendorId = vendor != null ? vendor.VendorId : Guid.Empty;
                    result.VendorName = vendor != null ? vendor.VendorName : "";
                    result.PriceWarehouse = item.PriceWarehouse;
                    result.PriceValueWarehouse = item.PriceValueWarehouse;

                    switch (status.PurchaseOrderStatusCode)
                    {
                        case "PURC":
                            result.BackgroundColorForStatus = "#007aff";
                            break;
                        case "IP":
                            result.BackgroundColorForStatus = "#ffcc00";
                            break;
                        case "RTN":
                            result.BackgroundColorForStatus = "#272909";
                            break;
                        case "CAN":
                            result.BackgroundColorForStatus = "#BB0000";
                            break;
                        case "DRA":
                            result.BackgroundColorForStatus = "#C9CAC2";
                            break;
                        case "COMP":
                            result.BackgroundColorForStatus = "#6D98E7";
                            break;
                    }

                    listVendorOrderReport.Add(result);
                });

                listVendorOrderReport = listVendorOrderReport.OrderByDescending(z => z.VendorOrderDate)
                    .ThenBy(t => t.VendorOrderCode).ToList();

                int stt = 0;
                listVendorOrderReport.ForEach(item =>
                {
                    stt++;
                    item.Stt = stt;
                });

                return new SearchVendorOrderReportResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success",
                    ListVendorOrderReport = listVendorOrderReport
                };
            }
            catch (Exception e)
            {
                return new SearchVendorOrderReportResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public ApprovalOrRejectVendorOrderResult ApprovalOrRejectVendorOrder(ApprovalOrRejectVendorOrderParameter parameter)
        {
            try
            {
                // Lấy các trạng thái đơn hàng mua
                var vendorOrderStatus = context.PurchaseOrderStatus.ToList();
                var objVendorOrder =
                    context.VendorOrder.FirstOrDefault(p => p.VendorOrderId == parameter.VendorOrderId);

                var userObj = context.User.FirstOrDefault(u => u.UserId == parameter.UserId);
                var employeeObj = context.Employee.FirstOrDefault(u => u.EmployeeId == userObj.EmployeeId);

                parameter.Description = parameter.Description != null ? parameter.Description.Trim() : "";

                Note note = new Note
                {
                    NoteId = Guid.NewGuid(),
                    ObjectType = "VENDORORDER",
                    ObjectId = parameter.VendorOrderId,
                    Type = "ADD",
                    Active = true,
                    CreatedById = parameter.UserId,
                    CreatedDate = DateTime.Now,
                    NoteTitle = CommonMessage.Note.NOTE_TITLE
                };

                var listItemInvalidModel = new List<ItemInvalidModel>();
                //Phê duyệt
                if (parameter.IsAprroval)
                {
                    #region Kiểm tra các điều kiện của đơn hàng mua để chuyển sang trạng thái Đơn hàng mua

                    //Lấy list phiếu đề xuất mua hàng: VendorOrderId
                    var listVendorOrderProcurementRequestMapping = context.VendorOrderProcurementRequestMapping
                        .Where(x => x.VendorOrderId == parameter.VendorOrderId).ToList();
                    var listProcurementRequestId = listVendorOrderProcurementRequestMapping
                        .Select(y => y.ProcurementRequestId).ToList();

                    var listProcurementRequest = context.ProcurementRequest
                        .Where(x => listProcurementRequestId.Contains(x.ProcurementRequestId)).ToList();

                    //Lấy list item trong phiếu đề xuất mua hàng
                    var listProcurementRequestItem = context.ProcurementRequestItem
                        .Where(x => listProcurementRequestId.Contains(x.ProcurementRequestId)).ToList();
                    var listProcurementRequestItemId =
                        listProcurementRequestItem.Select(y => y.ProcurementRequestItemId).ToList();

                    //Lấy list item từ phiếu đề xuất trong đơn hàng mua
                    var listVendorOrderDetail = context.VendorOrderDetail
                        .Where(x => x.VendorOrderId == parameter.VendorOrderId).ToList();
                    var listRequestItemOrderDetail = listVendorOrderDetail
                        .Where(x => x.ProcurementRequestItemId != null).ToList();

                    //Lấy list 
                    var listRequestItemId = listRequestItemOrderDetail.Select(y => y.ProcurementRequestItemId).ToList();

                    //Lấy các đơn hàng mua có trạng thái Đơn hàng mua và Đóng
                    var _listStatusCode = new List<string> { "PURC", "COMP" };
                    var _listStatusId = context.PurchaseOrderStatus
                        .Where(x => _listStatusCode.Contains(x.PurchaseOrderStatusCode))
                        .Select(y => y.PurchaseOrderStatusId).ToList();
                    var _listVendorOrderId = context.VendorOrder.Where(x => _listStatusId.Contains(x.StatusId))
                        .Select(y => y.VendorOrderId).ToList();

                    //Lấy các item có trong list đơn hàng mua trên
                    var listRequestItemHasUsing = context.VendorOrderDetail.Where(x =>
                            _listVendorOrderId.Contains(x.VendorOrderId) &&
                            listRequestItemId.Contains(x.ProcurementRequestItemId))
                        .GroupBy(x => new
                        {
                            x.ProcurementRequestItemId
                        })
                        .Select(y => new RequestItem
                        {
                            ProcurementRequestItemId = y.Key.ProcurementRequestItemId,
                            Quantity = y.Sum(s => s.Quantity)
                        }).ToList();

                    //Lấy các item có trong list phê duyệt
                    var listAllRequestItemHasUsing = context.VendorOrderDetail.Where(x =>
                            _listVendorOrderId.Contains(x.VendorOrderId) &&
                            listProcurementRequestItemId.Contains(x.ProcurementRequestItemId.Value))
                        .GroupBy(x => new
                        {
                            x.ProcurementRequestItemId
                        })
                        .Select(y => new RequestItem
                        {
                            ProcurementRequestItemId = y.Key.ProcurementRequestItemId,
                            Quantity = y.Sum(s => s.Quantity)
                        }).ToList();

                    //Với mỗi item từ phiếu đề xuất trong đơn hàng mua => kiểm tra xem số lượng có hợp lệ hay không
                    listVendorOrderDetail.ForEach(item =>
                    {
                        if (item.ProcurementRequestItemId != null)
                        {
                            var itemHasUsing =
                                listRequestItemHasUsing.FirstOrDefault(x =>
                                    x.ProcurementRequestItemId == item.ProcurementRequestItemId);

                            var baseItem = listProcurementRequestItem.FirstOrDefault(x =>
                                x.ProcurementRequestItemId == item.ProcurementRequestItemId);

                            if (itemHasUsing != null && baseItem != null)
                            {
                                var currentQuantity = item.Quantity;
                                var baseQuantity = baseItem.QuantityApproval;
                                var usingQuantity = itemHasUsing.Quantity;
                                var remainQuantity = baseQuantity - usingQuantity;

                                if (currentQuantity > baseQuantity - usingQuantity)
                                {
                                    var itemInvalid = new ItemInvalidModel();
                                    itemInvalid.ProcurementRequestItemId = item.ProcurementRequestItemId;
                                    itemInvalid.RemainQuantity = remainQuantity;
                                    listItemInvalidModel.Add(itemInvalid);
                                }
                            }
                        }
                    });

                    //Nếu không có Item nào không hợp lệ thì đổi trạng thái Đơn hàng mua -> Chờ phê duyệt
                    if (listItemInvalidModel.Count == 0)
                    {
                        objVendorOrder.StatusId =
                            vendorOrderStatus.FirstOrDefault(c => c.PurchaseOrderStatusCode == "PURC")
                                .PurchaseOrderStatusId;
                        objVendorOrder.ApproverId = employeeObj.EmployeeId;
                        objVendorOrder.ApproverPostion = employeeObj.PositionId;

                        note.Description = CommonMessage.Note.NOTE_CONTENT_APPROVAL_SUCCESS + parameter.Description;

                        var categoryTypeIdProcurementRequest = context.CategoryType
                            .FirstOrDefault(x => x.CategoryTypeCode == "DDU")?.CategoryTypeId;
                        var statusClose = context.Category.FirstOrDefault(x =>
                                x.CategoryTypeId == categoryTypeIdProcurementRequest && x.CategoryCode == "Close")?
                            .CategoryId;

                        //Kiểm tra và cập nhật trạng thái Phiếu đề xuất mua hàng => Đóng
                        var listUpdateProcurementRequest = new List<ProcurementRequest>();
                        listProcurementRequest.ForEach(proRq =>
                        {
                            //Lấy list Item trong mỗi phiếu đề xuất
                            var listItem = listProcurementRequestItem
                                .Where(x => x.ProcurementRequestId == proRq.ProcurementRequestId).ToList();

                            //Với mỗi Item kiểm tra số lượng đã được mua hết chưa?
                            bool isClose = true;
                            listItem.ForEach(item =>
                            {
                                var baseQuantity = item.QuantityApproval;
                                var usingItem = listAllRequestItemHasUsing.FirstOrDefault(x =>
                                    x.ProcurementRequestItemId == item.ProcurementRequestItemId);
                                decimal? usingQuantity = 0;
                                if (usingItem != null)
                                {
                                    usingQuantity = usingItem.Quantity ?? 0;
                                }

                                var currentItem = listVendorOrderDetail.FirstOrDefault(x =>
                                    x.ProcurementRequestItemId == item.ProcurementRequestItemId);
                                decimal? currentQuantity = 0;
                                if (currentItem != null)
                                {
                                    currentQuantity = currentItem.Quantity ?? 0;
                                }

                                if (baseQuantity != usingQuantity + currentQuantity)
                                {
                                    isClose = false;
                                }
                            });

                            /*
                             * Nếu tất cả item trong phiếu đề xuất mua hàng đã được mua hết thì:
                             * Chuyển trạng thái Phiếu đề xuất => Đóng
                             */
                            if (isClose)
                            {
                                proRq.StatusId = statusClose;
                                listUpdateProcurementRequest.Add(proRq);
                            }
                        });

                        if (listUpdateProcurementRequest.Count > 0)
                        {
                            context.ProcurementRequest.UpdateRange(listUpdateProcurementRequest);
                        }

                        #region Tự động tạo phiếu nhập kho
                        /*
                         * Với mỗi mã kho trong danh dách sách sản phẩm sẽ tạo ra 1 phiếu nhập kho khác nhau
                         * Các phiếu nhập kho ở trạng thái chờ nhập kho                          
                         */
                        var statusTypeId_PNK = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "TPH")?.CategoryTypeId;
                        var statusId_SanSang_PNK = context.Category.FirstOrDefault(x => x.CategoryCode == "SAS" && x.CategoryTypeId == statusTypeId_PNK)?.CategoryId;
                        var listAllInventoryReceivingVoucher = context.InventoryReceivingVoucher.ToList();

                        // Tự động sinh mã
                        var Code = "";
                        var datenow = DateTime.Now;
                        string year = datenow.Year.ToString().Substring(datenow.Year.ToString().Length - 2, 2);
                        string month = datenow.Month < 10 ? "0" + datenow.Month.ToString() : datenow.Month.ToString();
                        string day = datenow.Day < 10 ? "0" + datenow.Day.ToString() : datenow.Day.ToString();

                        var listAllWarehouse = context.Warehouse.Where(c => c.Active == true)
                             .Select(m => new
                             {
                                 m.WarehouseId,
                                 m.WarehouseParent
                             }).ToList();

                        var listWarehouseParentId = new List<KhoChaCon>();
                        var lstGroupWarhouseId = listVendorOrderDetail.Where(c => c.WarehouseId != null && c.WarehouseId != Guid.Empty).GroupBy(c => c.WarehouseId).Select(m => m.Key).ToList();
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
                        var listCodeToDay = listAllInventoryReceivingVoucher.Where(c =>
                                Convert.ToDateTime(c.CreatedDate).Day == datenow.Day &&
                                Convert.ToDateTime(c.CreatedDate).Month == datenow.Month &&
                                Convert.ToDateTime(c.CreatedDate).Year == datenow.Year).Select(y => new
                                {
                                    InventoryReceivingVoucherCode = y.InventoryReceivingVoucherCode
                                }).ToList();
                        var index = 1;
                        listWarehouseParentId.ForEach(warehouse =>
                        {

                            if (listCodeToDay.Count == 0)
                            {
                                Code = "PN-" + year + month + day + index.ToString("D4");
                            }
                            else
                            {
                                var listNumber = new List<int>();
                                listCodeToDay.ForEach(item =>
                                {
                                    var stringNumber = item.InventoryReceivingVoucherCode.Substring(9);
                                    var number = Int32.Parse(stringNumber);
                                    listNumber.Add(number);
                                });

                                var maxNumber = listNumber.OrderByDescending(x => x).FirstOrDefault();
                                var newNumber = maxNumber + index;

                                if (newNumber > 9999)
                                {
                                    Code = "PN-" + year + month + day + newNumber;
                                }
                                else
                                {
                                    Code = "PN-" + year + month + day + newNumber.ToString("D4");
                                }
                            }

                            var inventoryReceivingVourcher = new InventoryReceivingVoucher
                            {
                                InventoryReceivingVoucherId = Guid.NewGuid(),
                                InventoryReceivingVoucherCode = Code,
                                StatusId = statusId_SanSang_PNK.Value,
                                InventoryReceivingVoucherType = 1,
                                InventoryReceivingVoucherDate = DateTime.Now,
                                InventoryReceivingVoucherTime = DateTime.Now.TimeOfDay,
                                ExpectedDate = DateTime.Now,
                                LicenseNumber = 0,
                                PartnersId = objVendorOrder.VendorId,
                                WarehouseId = warehouse.ParentId,
                                Active = true,
                                CreatedById = parameter.UserId,
                                CreatedDate = DateTime.Now
                            };
                            context.InventoryReceivingVoucher.Add(inventoryReceivingVourcher);

                            var listInventoryReceivingVoucherMapping = listVendorOrderDetail.Where(c => c.WarehouseId != null && warehouse.ListChildId.Contains(c.WarehouseId.Value))
                                .Select(m => new InventoryReceivingVoucherMapping
                                {
                                    InventoryReceivingVoucherMappingId = Guid.NewGuid(),
                                    InventoryReceivingVoucherId = inventoryReceivingVourcher.InventoryReceivingVoucherId,
                                    ObjectId = m.VendorOrderId,
                                    ObjectDetailId = m.VendorOrderDetailId,
                                    WarehouseId = m.WarehouseId.Value,
                                    ProductId = m.ProductId.Value,
                                    PriceProduct = m.UnitPrice ?? 0,
                                    QuantityActual = m.Quantity ?? 0,
                                    QuantityRequest = m.Quantity ?? 0,
                                    QuantityReservation = m.Quantity ?? 0,
                                    QuantitySerial = 0,
                                    UnitId = m.UnitId,
                                    CurrencyUnit = m.CurrencyUnit,
                                    ExchangeRate = m.ExchangeRate,
                                    Vat = m.Vat,
                                    DiscountType = m.DiscountType,
                                    DiscountValue = m.DiscountValue,
                                    PriceAverage = false,
                                    Active = true,
                                    CreatedById = parameter.UserId,
                                    CreatedDate = DateTime.Now,
                                }).ToList();

                            context.InventoryReceivingVoucherMapping.AddRange(listInventoryReceivingVoucherMapping);
                            index++;
                        });
                        #endregion
                    }
                    else
                    {
                        return new ApprovalOrRejectVendorOrderResult()
                        {
                            StatusCode = HttpStatusCode.OK,
                            MessageCode = "Has Item Invalid",
                            ListItemInvalidModel = listItemInvalidModel
                        };
                    }
                    #endregion

                    //Gửi email thồn báo
                    #region Gửi thông báo

                    var _note = new Note();
                    _note.Description = parameter.Description;
                    objVendorOrder.UpdatedById = parameter.UserId;
                    objVendorOrder.UpdatedDate = DateTime.Now;
                    NotificationHelper.AccessNotification(context, TypeModel.VendorOrderDetail, "APPROVAL", new VendorOrder(),
                        objVendorOrder, true, _note);

                    #endregion
                }
                //Từ chối
                else
                {
                    objVendorOrder.StatusId =
                        vendorOrderStatus.FirstOrDefault(c => c.PurchaseOrderStatusCode == "RTN").PurchaseOrderStatusId;
                    note.Description = employeeObj.EmployeeName + CommonMessage.Note.NOTE_CONTENT_REJECT +
                                       parameter.Description;

                    //Gửi email thồn báo
                    #region Gửi thông báo

                    var _note = new Note();
                    _note.Description = parameter.Description;
                    objVendorOrder.UpdatedById = parameter.UserId;
                    objVendorOrder.UpdatedDate = DateTime.Now;
                    NotificationHelper.AccessNotification(context, TypeModel.VendorOrderDetail, "REJECT", new VendorOrder(),
                        objVendorOrder, true, _note);

                    #endregion
                }

                context.VendorOrder.Update(objVendorOrder);
                context.Note.Add(note);
                context.SaveChanges();

                return new ApprovalOrRejectVendorOrderResult
                {
                    MessageCode = "Success",
                    StatusCode = HttpStatusCode.OK,
                    ListItemInvalidModel = listItemInvalidModel
                };
            }
            catch (Exception e)
            {
                return new ApprovalOrRejectVendorOrderResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetQuantityApprovalResult GetQuantityApproval(GetQuantityApprovalParameter paramter)
        {
            try
            {
                decimal quantityApproval = 0;

                var _listStatusCode = new List<string> { "PURC", "COMP" };
                var _listStatusId = context.PurchaseOrderStatus
                    .Where(x => _listStatusCode.Contains(x.PurchaseOrderStatusCode))
                    .Select(y => y.PurchaseOrderStatusId).ToList();
                var _listVendorOrderId = context.VendorOrder.Where(x => _listStatusId.Contains(x.StatusId))
                    .Select(y => y.VendorOrderId).ToList();

                var requestItemHasUsing = new RequestItem();
                if (paramter.VendorOrderDetailId != null && paramter.VendorOrderDetailId != Guid.Empty)
                {
                    var vendorOrderDetail = context.VendorOrderDetail.FirstOrDefault(c => c.VendorOrderDetailId == paramter.VendorOrderDetailId);
                    if (vendorOrderDetail == null)
                    {
                        return new GetQuantityApprovalResult
                        {
                            MessageCode = "Không tìm thấy bản ghi",
                            StatusCode = HttpStatusCode.ExpectationFailed
                        };
                    }
                    requestItemHasUsing = context.VendorOrderDetail.Where(c =>
                            _listVendorOrderId.Contains(c.VendorOrderId) &&
                            c.ProductId == paramter.ProductId &&
                            c.ProcurementRequestItemId == paramter.ProcurementRequestItemId &&
                            c.VendorOrderDetailId != vendorOrderDetail.VendorOrderDetailId)
                        .GroupBy(x => new
                        {
                            x.ProcurementRequestItemId
                        })
                        .Select(y => new RequestItem
                        {
                            ProcurementRequestItemId = y.Key.ProcurementRequestItemId,
                            Quantity = y.Sum(s => s.Quantity)
                        }).FirstOrDefault();
                }
                else
                {
                    requestItemHasUsing = context.VendorOrderDetail.Where(c =>
                            _listVendorOrderId.Contains(c.VendorOrderId) &&
                            c.ProductId == paramter.ProductId &&
                            c.ProcurementRequestItemId == paramter.ProcurementRequestItemId)
                        .GroupBy(x => new
                        {
                            x.ProcurementRequestItemId
                        })
                        .Select(y => new RequestItem
                        {
                            ProcurementRequestItemId = y.Key.ProcurementRequestItemId,
                            Quantity = y.Sum(s => s.Quantity)
                        }).FirstOrDefault();
                }
                //Lấy các item có trong list đơn hàng mua trên

                var procurementRequestItem = context.ProcurementRequestItem.FirstOrDefault(c => c.ProcurementRequestItemId == paramter.ProcurementRequestItemId);

                quantityApproval = requestItemHasUsing == null ? procurementRequestItem.QuantityApproval.Value :
                    (procurementRequestItem.QuantityApproval ?? 0m) - (requestItemHasUsing.Quantity ?? 0m);

                return new GetQuantityApprovalResult
                {
                    MessageCode = "Success",
                    StatusCode = HttpStatusCode.OK,
                    QuantityApproval = quantityApproval
                };
            }
            catch (Exception ex)
            {
                return new GetQuantityApprovalResult
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = ex.Message
                };
            }
        }

        public GetDashboardVendorResult GetDashboardVendor(GetDashboardVendorParamter paramter)
        {
            try
            {
                var user = context.User.FirstOrDefault(c => c.UserId == paramter.UserId);
                if (user == null)
                {
                    return new GetDashboardVendorResult
                    {
                        Message = "Nhân viên không tồn tại trong hệ thống",
                        StatusCode = HttpStatusCode.NotFound
                    };
                }
                var employee = context.Employee.FirstOrDefault(c => c.EmployeeId == user.EmployeeId);
                if (user == null)
                {
                    return new GetDashboardVendorResult
                    {
                        Message = "Nhân viên không tồn tại trong hệ thống",
                        StatusCode = HttpStatusCode.NotFound
                    };
                }

                #region Max level of product category
                var commonProductCategory = context.ProductCategory.ToList();
                var listProductCategoryLevel = (from productcategory in commonProductCategory
                                                select new
                                                { levelMaxProductCategory = productcategory.ProductCategoryLevel }).ToList();
                int? levelMaxProductCategory = listProductCategoryLevel.Max(x => x.levelMaxProductCategory);
                #endregion

                #region MasterData 
                // Trạng thái đơn hàng mua
                var listAllpurchaseOrderStatus = context.PurchaseOrderStatus.Where(c => c.Active == true).ToList();
                var lstPurchaseOrderStatusCode = context.SystemParameter.FirstOrDefault(c => c.SystemKey == "PurchaseOrderStatus").SystemValueString.Split(';').ToList();
                var lstPurchaseorderStatusId = listAllpurchaseOrderStatus.Where(c => lstPurchaseOrderStatusCode.Contains(c.PurchaseOrderStatusCode)).Select(m => m.PurchaseOrderStatusId).ToList();
                var listAllVendorOrder = context.VendorOrder.Where(c => c.Active == true && lstPurchaseorderStatusId.Contains(c.StatusId)).ToList();
                var statusWaitingForApprovalOfVendorOrder = listAllpurchaseOrderStatus.FirstOrDefault(c => c.PurchaseOrderStatusCode == "IP")?.PurchaseOrderStatusId;
                var listVendorWaitingForApproval = context.VendorOrder.Where(c => c.Active == true && c.StatusId == statusWaitingForApprovalOfVendorOrder).ToList();
                var listAllVendor = context.Vendor.Select(m => new
                {
                    m.VendorId,
                    m.VendorCode,
                    m.VendorName,
                    m.VendorGroupId
                });
                var listAllRequest = context.ProcurementRequest.Where(c => c.Active == true).ToList();
                var listAllRequestId = listAllRequest.Select(m => m.ProcurementRequestId).ToList();
                var listAllRequestItem = context.ProcurementRequestItem.Where(c => listAllRequestId.Contains(c.ProcurementRequestId.Value)).ToList();
                #endregion
                #region Default Value
                var organ = context.Organization.Where(c => c.OrganizationId == employee.OrganizationId)
                    .Select(m => new OrganizationEntityModel
                    {
                        OrganizationId = m.OrganizationId,
                        OrganizationCode = m.OrganizationCode,
                        OrganizationName = m.OrganizationName,
                        ParentId = m.ParentId
                    }).FirstOrDefault();
                var isRoot = organ.ParentId == null;
                #endregion

                #region Varibale Result
                decimal totalCost = 0;
                List<dynamic> lstResultVendorChart = new List<dynamic>();
                #endregion
                var listVendorOrderFollowOrganization = new List<VendorOrder>();
                var listRequestFollowOrganization = new List<ProcurementRequest>();
                if (employee.IsManager)
                {
                    #region Get list Child of Organization
                    List<Guid?> listGetAllChild = new List<Guid?>();
                    if (paramter.OrganizationId == null || paramter.OrganizationId == Guid.Empty)
                        paramter.OrganizationId = organ.OrganizationId;

                    listGetAllChild.Add(paramter.OrganizationId);
                    listGetAllChild = getOrganizationChildrenId(paramter.OrganizationId, listGetAllChild);
                    #endregion
                    var listEmployeeId = context.Employee.Where(c => listGetAllChild.Contains(c.OrganizationId)).Select(m => m.EmployeeId).ToList();
                    listVendorOrderFollowOrganization = listAllVendorOrder
                        .Where(c => listEmployeeId.Contains(c.Orderer.Value) &&
                                    (paramter.FromDate == null || paramter.FromDate == DateTime.MinValue || paramter.FromDate.Value.Date <= c.VendorOrderDate.Date) &&
                                    (paramter.ToDate == null || paramter.ToDate == DateTime.MinValue || paramter.ToDate.Value.Date >= c.VendorOrderDate.Date)).ToList();
                    listVendorWaitingForApproval = listVendorWaitingForApproval
                        .Where(c => listEmployeeId.Contains(c.Orderer.Value) &&
                                    (paramter.FromDate == null || paramter.FromDate == DateTime.MinValue || paramter.FromDate.Value.Date <= c.VendorOrderDate.Date) &&
                                    (paramter.ToDate == null || paramter.ToDate == DateTime.MinValue || paramter.ToDate.Value.Date >= c.VendorOrderDate.Date)).ToList();
                    // Đề xuất mua hàng
                    listRequestFollowOrganization = listAllRequest.Where(c => listEmployeeId.Contains(c.RequestEmployeeId.Value) &&
                                        (paramter.FromDate == null || paramter.FromDate == DateTime.MinValue || paramter.FromDate <= c.CreatedDate.Value.Date) &&
                                        (paramter.ToDate == null || paramter.ToDate == DateTime.MinValue || paramter.ToDate >= c.CreatedDate.Value.Date)).ToList();
                }
                else
                {
                    listVendorOrderFollowOrganization = listAllVendorOrder.Where(c => employee.EmployeeId == c.Orderer.Value &&
                                    (paramter.FromDate == null || paramter.FromDate == DateTime.MinValue || paramter.FromDate <= c.VendorOrderDate) &&
                                    (paramter.ToDate == null || paramter.ToDate == DateTime.MinValue || paramter.ToDate >= c.VendorOrderDate)).ToList();
                    listVendorWaitingForApproval = listVendorWaitingForApproval.Where(c => employee.EmployeeId == c.Orderer.Value &&
                                    (paramter.FromDate == null || paramter.FromDate == DateTime.MinValue || paramter.FromDate <= c.VendorOrderDate) &&
                                    (paramter.ToDate == null || paramter.ToDate == DateTime.MinValue || paramter.ToDate >= c.VendorOrderDate)).ToList();
                    // Đề xuất mua hàng
                    listRequestFollowOrganization = listAllRequest.Where(c => employee.EmployeeId == c.RequestEmployeeId &&
                                     (paramter.FromDate == null || paramter.FromDate == DateTime.MinValue || paramter.FromDate <= c.CreatedDate.Value.Date) &&
                                     (paramter.ToDate == null || paramter.ToDate == DateTime.MinValue || paramter.ToDate >= c.CreatedDate.Value.Date)).ToList();
                }
                #region Tổng chi phí
                totalCost = listVendorOrderFollowOrganization.Sum(c => c.Amount);
                #endregion

                #region Tỷ lệ mua hàng theo nhà cung cấp
                var listVendorGroup = listVendorOrderFollowOrganization.GroupBy(c => c.VendorId)
                    .Select(m => new
                    {
                        VendorId = m.Key,
                        Count = m.Count()
                    }).ToList();
                var total = listVendorGroup.Sum(c => c.Count);
                listVendorGroup = listVendorGroup.OrderByDescending(c => c.Count).ToList();

                //Lấy list nhóm nhà cung cấp
                var nhomNhaCungCapCategoryTypeId = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "NCA")?.CategoryTypeId;
                var listNhomNhaCungCap = context.Category.Where(x => x.CategoryTypeId == nhomNhaCungCapCategoryTypeId).ToList();
                //nhóm nhà cc
                listNhomNhaCungCap.ForEach(nhomNcc =>
                {
                    //Lấy các nhà cung cấp thuộc nhóm
                    var listAllVenDorNccId = listAllVendor.Where(x => x.VendorGroupId == nhomNcc.CategoryId).Select(x => x.VendorId).ToList();

                    var countNhomNcc = listVendorGroup.Where(x => listAllVenDorNccId.Contains(x.VendorId)).Sum(x => x.Count);

                    var sampleObject = new ExpandoObject() as IDictionary<string, Object>;
                    sampleObject.Add("VendorId", nhomNcc.CategoryId);
                    sampleObject.Add("VendorName", nhomNcc.CategoryName);
                    sampleObject.Add("Count", countNhomNcc);
                    decimal percent = 0;
                    percent = ((decimal)countNhomNcc / (decimal)total) * 100;
                    percent = Math.Round(percent, 1);
                    sampleObject.Add("Percent", percent);
                    lstResultVendorChart.Add(sampleObject);
                });

                //listVendorGroup.ForEach(item =>
                //{
                //    var vendor = listAllVendor.FirstOrDefault(c => c.VendorId == item.VendorId);
                //    var sampleObject = new ExpandoObject() as IDictionary<string, Object>;
                //    sampleObject.Add("VendorId", item.VendorId);
                //    sampleObject.Add("VendorName", vendor.VendorName);
                //    sampleObject.Add("Count", item.Count);
                //    decimal percent = 0;
                //    percent = ((decimal)item.Count / (decimal)total) * 100;
                //    percent = Math.Round(percent, 1);
                //    sampleObject.Add("Percent", percent);
                //    lstResultVendorChart.Add(sampleObject);
                //});
                #endregion

                #region Top Đơn hàng mua chờ phê duyệt
                var listVendorOrderWaitingApproval = listVendorWaitingForApproval
                    .Select(m => new VendorOrderEntityModel {
                        VendorOrderId = m.VendorOrderId,
                        VendorOrderCode = m.VendorOrderCode,
                        VendorCode =  string.Empty,
                        VendorName = string.Empty,
                        VendorOrderDate = m.VendorOrderDate,
                        Amount = m.Amount,
                        VendorId = m.VendorId
                    }).OrderByDescending(c => c.VendorOrderDate).ToList();
                listVendorOrderWaitingApproval.ForEach(item =>
                {
                    var vendor = listAllVendor.FirstOrDefault(c => c.VendorId == item.VendorId);
                    item.VendorCode = vendor?.VendorCode ?? string.Empty;
                    item.VendorName = $"{vendor?.VendorCode ?? string.Empty}-{vendor?.VendorName ?? string.Empty}";
                });
                #endregion

                #region Top để xuất mua chờ phê duyệt
                var typeRequestId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "DDU")?.CategoryTypeId ?? Guid.Empty;
                var requestWaitingForArrpvalStatusId = context.Category.FirstOrDefault(c => c.CategoryTypeId == typeRequestId && ( c.CategoryCode == "WaitForAp"))?.CategoryId ?? Guid.Empty;
                var listRequestWaitingApproval = listRequestFollowOrganization.Where(c => c.StatusId == requestWaitingForArrpvalStatusId)
                    .Select(m => new ProcurementRequestEntityModel
                    {
                        ProcurementRequestId = m.ProcurementRequestId,
                        ProcurementCode = m.ProcurementCode,
                        CreatedDate = m.CreatedDate,
                        TotalMoney = GetTotalMoneyOfProcurementRequest(m.ProcurementRequestId, listAllRequestItem)
                    }).OrderByDescending(c => c.CreatedDate).ToList();
                #endregion

                return new GetDashboardVendorResult
                {
                    TotalCost = totalCost,
                    IsRoot = isRoot,
                    Organization = organ,
                    LevelMaxProductCategory = levelMaxProductCategory.Value,
                    ListResultVendorChart = lstResultVendorChart,
                    ListVendorOrder = listVendorOrderWaitingApproval,
                    ListRequest = listRequestWaitingApproval,
                    Message = "Success",
                    StatusCode = HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new GetDashboardVendorResult
                {
                    Message = ex.Message,
                    StatusCode = HttpStatusCode.BadRequest
                };
            }
        }
        public GetProductCategoryGroupByLevelResult GetProductCategoryGroupByLevel(GetProductCategoryGroupByLevelParameter parameter)
        {
            try
            {
                var commonProductCategory = context.ProductCategory.ToList();
                #region Get employee follow organization
                // because api all after api GetDashboardVendor so used parameter.OrganizationId;
                var organizationId = parameter.OrganizationId;
                List<Guid?> listGetAllChild = new List<Guid?>();
                if (organizationId != null)
                {
                    listGetAllChild.Add(organizationId);
                    listGetAllChild = getOrganizationChildrenId(organizationId, listGetAllChild);
                }
                var commonEmployee = context.Employee.Where(x => x.OrganizationId != null && listGetAllChild.Contains(x.OrganizationId.Value)).ToList();
                var employeeIdList = commonEmployee.Select(x => x.EmployeeId).ToList();
                #endregion
                var commonOrderStatus = context.PurchaseOrderStatus.Where(w => w.PurchaseOrderStatusCode == "IP" || w.PurchaseOrderStatusCode == "PURC" || w.PurchaseOrderStatusCode == "COMP").ToList();
                var listPuOrderStatusId = commonOrderStatus.Select(m => m.PurchaseOrderStatusId).ToList();
                var commonVendorOrder = context.VendorOrder.Where(c => c.StatusId != null && listPuOrderStatusId.Contains(c.StatusId)
                              && employeeIdList.Contains(c.Orderer.Value)
                              && (parameter.VendorOrderDateStart == null || parameter.VendorOrderDateStart == DateTime.MinValue || parameter.VendorOrderDateStart.Value.Date <= c.VendorOrderDate.Date)
                              && (parameter.VendorOrderDateEnd == null || parameter.VendorOrderDateEnd == DateTime.MinValue || parameter.VendorOrderDateEnd.Value.Date >= c.VendorOrderDate.Date)).ToList();
                var vendorOrderIdList = commonVendorOrder.Select(c => c.VendorOrderId).ToList();
                var commonVendorOrderDetail = context.VendorOrderDetail.Where(c => vendorOrderIdList.Contains(c.VendorOrderId)).ToList();

                var productIdList = commonVendorOrderDetail.Select(c => c.ProductId).ToList();
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
                    List<Guid> newListProductCategoryIdChildren = new List<Guid>
                    {
                        item.ProductCategoryId
                    };
                    newListProductCategoryIdChildren = getProductCategoryChildrenId(item.ProductCategoryId, newListProductCategoryIdChildren, commonProductCategory);

                    newListProductCategory.Add(new ListCategoryId
                    {
                        ParentProductCategoryId = item.ProductCategoryId,
                        ParentProductCategoryName = item.ProductCategoryName,
                        ListChildrent = newListProductCategoryIdChildren
                    });
                });
                List<ListCategoryResult> newListProductCategoryResult = new List<ListCategoryResult>();
                var productCategoryIdList = commonProduct.Select(x => x.ProductCategoryId).ToList();
                var productCategoryList = commonProductCategory.Where(c => productCategoryIdList.Contains(c.ProductCategoryId)).ToList();
                List<ProductCategoryModel> listProductCategoryModel = new List<ProductCategoryModel>();

                commonVendorOrderDetail.ForEach(item =>
                {
                    #region viết lại công thức tính tổng tiền theo từng sản phẩm, mỗi sản phẩm phải trừ tiền chiết khấu của tổng đơn hàng mua
                    var vendorOrder = commonVendorOrder.FirstOrDefault(f => f.VendorOrderId == item.VendorOrderId);
                    decimal? discountPerOrder = 0;
                    if (vendorOrder.DiscountType == true)
                    {
                        //chiết khấu phần trăm
                        discountPerOrder = vendorOrder?.DiscountValue ?? 0;
                    }
                    else
                    {
                        //chiết khấu theo số tiền
                        discountPerOrder = (vendorOrder.DiscountValue / vendorOrder.Amount) * 100;
                    }
                    #endregion

                    var product = commonProduct.FirstOrDefault(x => x.ProductId == item.ProductId);

                    if (product != null)
                    {
                        var category = commonProductCategory.FirstOrDefault(x => x.ProductCategoryId == product.ProductCategoryId);

                        var productCategoryModel = new ProductCategoryModel
                        {
                            ProductCategoryId = product.ProductCategoryId,
                            ProductCategoryName = category.ProductCategoryName,
                            Total = ReCalculatorTotal(item.Vat.Value, item.DiscountType.Value, item.DiscountValue.Value, item.Quantity.Value, item.UnitPrice.Value, item.ExchangeRate.Value, discountPerOrder)
                        };
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
                newListProductCategoryResult = newListProductCategoryResult.OrderByDescending(c => c.Total).ToList();
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
                    LstResult = lstResult,
                    MessageCode = "Success",
                    StatusCode = HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new GetProductCategoryGroupByLevelResult
                {
                    Message = ex.Message,
                    StatusCode = HttpStatusCode.BadRequest
                };
            }
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
        private decimal ReCalculatorTotal(decimal? Vat, bool DiscountType, decimal? DiscountValue, decimal Quantity, decimal UnitPrice, decimal? ExchangeRate, decimal? DiscountPerOrder)
        {
            decimal total = (Quantity * UnitPrice * ExchangeRate.Value);
            if (Vat >= 0)
            {
                if (DiscountType == true && DiscountValue >= 0)
                {
                    //Nếu chiết khấu bằng %
                    total -= (total * DiscountValue.Value / 100);
                }
                else if (DiscountType == false && DiscountValue >= 0)
                {
                    //Nếu chiếu khấu bằng tiền
                    total -= DiscountValue.Value;
                }
                total += (total * Vat.Value / 100);
            }
            else if (Vat == null)
            {
                if (DiscountType == true && DiscountValue >= 0)
                {
                    //Nếu chiết khấu bằng %
                    total -= (total * DiscountValue.Value / 100);
                }
                else if (DiscountType == false && DiscountValue >= 0)
                {
                    //Nếu chiếu khấu bằng tiền
                    total -= DiscountValue.Value;
                }
            }

            var discountValue = (total * DiscountPerOrder.Value) / 100;

            total -= discountValue;

            return total;
        }
        public GetDataBarchartFollowMonthResult GetDataBarchartFollowMonth(GetDataBarchartFollowMonthParameter parameter)
        {
            try
            {
                var user = context.User.FirstOrDefault(c => c.UserId == parameter.UserId);
                if (user == null)
                {
                    return new GetDataBarchartFollowMonthResult
                    {
                        Message = "Nhân viên không tồn tại trong hệ thống",
                        StatusCode = HttpStatusCode.NotFound
                    };
                }
                var employee = context.Employee.FirstOrDefault(c => c.EmployeeId == user.EmployeeId);
                if (user == null)
                {
                    return new GetDataBarchartFollowMonthResult
                    {
                        Message = "Nhân viên không tồn tại trong hệ thống",
                        StatusCode = HttpStatusCode.NotFound
                    };
                }
                #region MasterData 
                // Trạng thái đơn hàng mua
                var listAllpurchaseOrderStatus = context.PurchaseOrderStatus.Where(c => c.Active == true).ToList();
                var lstPurchaseOrderStatusCode = context.SystemParameter.FirstOrDefault(c => c.SystemKey == "PurchaseOrderStatus").SystemValueString.Split(';').ToList();
                var lstPurchaseorderStatusId = listAllpurchaseOrderStatus.Where(c => lstPurchaseOrderStatusCode.Contains(c.PurchaseOrderStatusCode)).Select(m => m.PurchaseOrderStatusId).ToList();
                var listAllVendorOrder = context.VendorOrder.Where(c => c.Active == true && lstPurchaseorderStatusId.Contains(c.StatusId)).ToList();

                // Trạng thái đơn hàng bán
                var listAllOrderStatus = context.OrderStatus.Where(c => c.Active == true).ToList();
                var listOrderStatusCode = context.SystemParameter.FirstOrDefault(c => c.SystemKey == "OrderStatus").SystemValueString.Split(';').ToList();
                var listOrderStatusId = listAllOrderStatus.Where(c => listOrderStatusCode.Contains(c.OrderStatusCode)).Select(m => m.OrderStatusId).ToList();
                var listAllCustomerOrder = context.CustomerOrder.Where(c => c.Active == true && listOrderStatusId.Contains(c.StatusId.Value)).ToList();

                // Để xuất mua hàng
                var typeRequestId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "DDU")?.CategoryTypeId ?? Guid.Empty;
                var listRequestStatusId = context.Category.Where(c => c.CategoryTypeId == typeRequestId && (c.CategoryCode == "Approved" ||
                                                        c.CategoryCode == "Close" || c.CategoryCode == "WaitForAp")).Select(m => m.CategoryId).ToList();
                var listAllRequest = context.ProcurementRequest.Where(c => listRequestStatusId.Contains(c.StatusId.Value) && c.Active == true).ToList();
                var listAllRequestId = listAllRequest.Select(m => m.ProcurementRequestId).ToList();
                var listAllRequestItem = context.ProcurementRequestItem.Where(c => listAllRequestId.Contains(c.ProcurementRequestId.Value)).ToList();
                #endregion

                #region Data follow role
                var listVendorOrderFollowOrganization = new List<VendorOrder>();
                var listCustomerOrderFollowOrganization = new List<CustomerOrder>();
                var listRequestFollowOrganization = new List<ProcurementRequest>();
                var startDate = parameter.Date.AddMonths(-parameter.Month);
                if (employee.IsManager)
                {
                    #region Get list Child of Organization
                    List<Guid?> listGetAllChild = new List<Guid?>
                    {
                        parameter.OrganizationId
                    };
                    listGetAllChild = getOrganizationChildrenId(parameter.OrganizationId, listGetAllChild);
                    #endregion

                    var listEmployeeId = context.Employee.Where(c => listGetAllChild.Contains(c.OrganizationId)).Select(m => m.EmployeeId).ToList();

                    // Đơn hàng mua
                    listVendorOrderFollowOrganization = listAllVendorOrder.Where(c => listEmployeeId.Contains(c.Orderer.Value) &&
                                    (startDate == DateTime.MinValue || startDate.Date <= c.VendorOrderDate.Date) &&
                                    (parameter.Date == null || parameter.Date == DateTime.MinValue || parameter.Date.Date >= c.VendorOrderDate.Date)).ToList();
                    // Đơn hàng bán
                    listCustomerOrderFollowOrganization = listAllCustomerOrder.Where(c => listEmployeeId.Contains(c.Seller.Value) &&
                                    (startDate == DateTime.MinValue || startDate.Date <= c.OrderDate.Date) &&
                                    (parameter.Date == null || parameter.Date == DateTime.MinValue || parameter.Date.Date >= c.OrderDate.Date)).ToList();
                    // Đề xuất mua hàng
                    listRequestFollowOrganization = listAllRequest.Where(c => listEmployeeId.Contains(c.RequestEmployeeId.Value) &&
                                        (startDate == DateTime.MinValue || startDate.Date <= c.CreatedDate.Value.Date) &&
                                        (parameter.Date == null || parameter.Date == DateTime.MinValue || parameter.Date.Date >= c.CreatedDate.Value.Date)).ToList();
                }
                else
                {
                    // Đơn hàng bán
                    listVendorOrderFollowOrganization = listAllVendorOrder.Where(c => employee.EmployeeId == c.Orderer.Value &&
                                    (startDate == DateTime.MinValue || startDate.Date <= c.VendorOrderDate.Date) &&
                                    (parameter.Date == null || parameter.Date == DateTime.MinValue || parameter.Date >= c.VendorOrderDate)).ToList();
                    // Đơn hàng mua
                    listCustomerOrderFollowOrganization = listAllCustomerOrder.Where(c => employee.EmployeeId == c.Seller &&
                                    (startDate == DateTime.MinValue || startDate.Date <= c.OrderDate.Date) &&
                                    (parameter.Date == null || parameter.Date == DateTime.MinValue || parameter.Date.Date >= c.OrderDate.Date)).ToList();
                    // Đề xuất mua hàng
                    listRequestFollowOrganization = listAllRequest.Where(c => employee.EmployeeId == c.RequestEmployeeId &&
                                    (startDate == DateTime.MinValue || startDate.Date <= c.CreatedDate.Value.Date) &&
                                    (parameter.Date == null || parameter.Date == DateTime.MinValue || parameter.Date.Date >= c.CreatedDate.Value.Date)).ToList();
                }
                #endregion

                // Đơn hàng mua
                var listVendorOrderGroupMonth = listVendorOrderFollowOrganization.Select(m => new
                {
                    m.VendorOrderId,
                    m.Amount,
                    Date = new DateTime(m.VendorOrderDate.Year, m.VendorOrderDate.Month, 1),
                }).GroupBy(c => c.Date)
                .Select(m => new
                {
                    Date = m.Key,
                    DateStr = $"{m.Key.Month}/{m.Key.Year}",
                    SumAmount = m.Sum(s => s.Amount)
                }).OrderBy(c => c.Date).ToList();
                // Đơn hàng bán
                var listCustomerOrderGroupMonth = listCustomerOrderFollowOrganization.Select(m => new
                {
                    m.OrderId,
                    m.Amount,
                    Date = new DateTime(m.OrderDate.Year, m.OrderDate.Month, 1)
                }).GroupBy(c => c.Date)
                .Select(m => new
                {
                    Date = m.Key,
                    DateStr = $"{m.Key.Month}/{m.Key.Year}",
                    SumAmount = m.Sum(s => s.Amount)
                }).OrderBy(c => c.Date).ToList();
                // Đề xuất yêu cầu
                var listRequestGroupMonth = listRequestFollowOrganization.Select(m => new
                {
                    m.ProcurementRequestId,
                    Amount = GetTotalMoneyOfProcurementRequest(m.ProcurementRequestId, listAllRequestItem),
                    Date = new DateTime(m.CreatedDate.Value.Year, m.CreatedDate.Value.Month, 1)
                }).GroupBy(c => c.Date)
                .Select(m => new
                {
                    Date = m.Key,
                    DateStr = $"{m.Key.Month}/{m.Key.Year}",
                    SumAmount = m.Sum(s => s.Amount)
                }).OrderBy(c => c.Date).ToList();

                var tempDate = parameter.Date;

                #region Bar chart so sánh giá trị đề xuất và giá trị mua hàng thực tế
                var listResult3 = new List<dynamic>();
                for (int i = parameter.Month -1; i >= 0; i--)
                {
                    var dStr = tempDate.AddMonths(-i);
                    var dateStr = $"{dStr.Month}/{dStr.Year}";
                    var venTempMonth = listVendorOrderGroupMonth.FirstOrDefault(c => c.DateStr == dateStr);
                    var requestTempMonth = listRequestGroupMonth.FirstOrDefault(c => c.DateStr == dateStr);

                    var sampleObject = new ExpandoObject() as IDictionary<string, Object>;
                    sampleObject.Add("DateStr", dateStr);
                    sampleObject.Add("VendorOrderAmount", venTempMonth?.SumAmount ?? 0);
                    sampleObject.Add("RequestAmount", requestTempMonth?.SumAmount ?? 0);
                    listResult3.Add(sampleObject);
                }
                #endregion

                #region Bar chart so sánh giá trị mua hàng và bán hàng theo từng tháng, 3 tháng, 6 tháng, 12 tháng
                var listResult4 = new List<dynamic>();
                for (int i = parameter.Month - 1; i >= 0; i--)
                {
                    var dStr = tempDate.AddMonths(-i);
                    var dateStr = $"{dStr.Month}/{dStr.Year}";
                    var venTempMonth = listVendorOrderGroupMonth.FirstOrDefault(c => c.DateStr == dateStr);
                    var cusTempMonth = listCustomerOrderGroupMonth.FirstOrDefault(c => c.DateStr == dateStr);

                    var sampleObject = new ExpandoObject() as IDictionary<string, Object>;
                    sampleObject.Add("DateStr", dateStr);
                    sampleObject.Add("VendorOrderAmount", venTempMonth?.SumAmount ?? 0);
                    sampleObject.Add("CustomerOrderAmount", cusTempMonth?.SumAmount ?? 0);
                    listResult4.Add(sampleObject);
                }
                #endregion

                return new GetDataBarchartFollowMonthResult
                {
                    MonthOrderList = listResult4,
                    MonthOrderAndRequestList = listResult3,
                    Message = "Success",
                    StatusCode = HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new GetDataBarchartFollowMonthResult
                {
                    Message = ex.Message,
                    StatusCode = HttpStatusCode.BadRequest
                };
            }
        }
        public decimal GetTotalMoneyOfProcurementRequest(Guid ProcurementRequestId, List<ProcurementRequestItem> commonProcurementRequestItem)
        {
            decimal totaltMoney = 0;
            var listItem = commonProcurementRequestItem.Where(it => it.ProcurementRequestId == ProcurementRequestId).ToList();
            if (listItem != null)
            {
                listItem.ForEach(item =>
                {
                    totaltMoney += (item.UnitPrice.HasValue ? item.UnitPrice.Value : 0) * (item.Quantity.HasValue ? item.Quantity.Value : 0);
                });
            }
            return totaltMoney;
        }
    }

    public class SoDuSanPhamTrongKho
    {
        public Guid ProductId { get; set; }
        public Guid WarehouseId { get; set; }
        public decimal? SucChua { get; set; }
        public string WarehouseName { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public decimal? SoLuongTonKhoToiDa { get; set; }
        public decimal? SoLuongDat { get; set; }
        public decimal? SoLuongTonKho { get; set; }
    }

    public class VendorAndCustomerOrderFollowMonth
    {
        public string DateStr { get; set; }
        public decimal VendorOrderAmount { get; set; }
        public decimal CustomerOrderAmount { get; set; }
    }
}
