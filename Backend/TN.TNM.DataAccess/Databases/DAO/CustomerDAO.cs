using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Npgsql;
using NpgsqlTypes;
using OfficeOpenXml;
using TN.TNM.Common;
using TN.TNM.Common.NotificationSetting;
using TN.TNM.DataAccess.Consts.Product;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Helper;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.Customer;
using TN.TNM.DataAccess.Messages.Parameters.Lead;
using TN.TNM.DataAccess.Messages.Results.Customer;
using TN.TNM.DataAccess.Messages.Results.Lead;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Address;
using TN.TNM.DataAccess.Models.BankAccount;
using TN.TNM.DataAccess.Models.Customer;
using TN.TNM.DataAccess.Models.CustomerServiceLevel;
using TN.TNM.DataAccess.Models.Employee;
using TN.TNM.DataAccess.Models.Folder;
using TN.TNM.DataAccess.Models.GeographicalArea;
using TN.TNM.DataAccess.Models.Lead;
using TN.TNM.DataAccess.Models.Note;
using TN.TNM.DataAccess.Models.ProductCategory;
using TN.TNM.DataAccess.Models.Quote;
using TN.TNM.DataAccess.Models.TinhHuong;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace TN.TNM.DataAccess.Databases.DAO
{
    public class CustomerDAO : BaseDAO, ICustomerDataAccess
    {
        private readonly IHostingEnvironment hostingEnvironment;
        public IConfiguration Configuration { get; }
        public static string Email;
        public TenantContext tenantContext;
        public IEmployeeDataAccess employeeDAO;

        public CustomerDAO(Databases.TNTN8Context _content, IAuditTraceDataAccess _iAuditTrace, IHostingEnvironment _hostingEnvironment, IConfiguration iconfiguration, TenantContext _tenantContext, IEmployeeDataAccess _employeeDAO)
        {
            this.context = _content;
            this.iAuditTrace = _iAuditTrace;
            this.hostingEnvironment = _hostingEnvironment;
            this.Configuration = iconfiguration;
            this.tenantContext = _tenantContext;
            this.employeeDAO = _employeeDAO;
        }

        public void GetConfiguration()
        {
            Email = Configuration["Email"];
        }

        public CreateCustomerResult CreateCustomer(CreateCustomerParameter parameter)
        {
            Customer customer = parameter.Customer.ToEntity();
            var newContactId = Guid.NewGuid();
            var newCustomerId = Guid.NewGuid();
            var duplicateContact = false;
            var ListCustomer = new List<CustomerEntityModel>();

            try
            {

                Contact contactCus = parameter.Contact.ToEntity();
                if (parameter.Customer?.TotalCapital < 0 || parameter.Customer?.TotalEmployeeParticipateSocialInsurance < 0 || parameter.Customer?.TotalRevenueLastYear < 0)
                {
                    return new CreateCustomerResult()
                    {
                        StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                        MessageCode = CommonMessage.Customer.CREATE_FAIL
                    };
                }
                var tenGoi = LocDau(parameter.Contact.FirstName);
                var hoTenDem = LocDau(parameter.Contact.LastName);
                var newUserName = hoTenDem + tenGoi;
                newUserName = String.Concat(newUserName.Where(c => !Char.IsWhiteSpace(c)));
                var cusSttId = context.CategoryType.FirstOrDefault(ct => ct.CategoryTypeCode == "THA").CategoryTypeId;

                //add by dungpt
                Guid newCusId = Guid.Empty;


                var SendEmailEntityModel = new DataAccess.Models.Email.SendEmailEntityModel();



                //Tạo khách hàng
                if (parameter.IsFromLead == false)
                {
                    #region Thêm khách hàng định danh

                    newCusId = context.Category.FirstOrDefault(c => c.CategoryTypeId == cusSttId && c.CategoryCode == "HDO")
                        .CategoryId;

                    customer.CustomerId = newCustomerId;
                    customer.StatusId = newCusId;
                    customer.CreatedDate = DateTime.Now;
                    customer.CreatedById = parameter.UserId;
                    customer.UpdatedDate = DateTime.Now;
                    customer.UpdatedById = parameter.UserId;
                    customer.CustomerGroupId = parameter.Customer.CustomerGroupId;
                    customer.StaffChargeIds = parameter.Customer.StaffChargeIds;

                    contactCus.TaxCode = parameter.Contact.TaxCode == null ? "" : parameter.Contact.TaxCode.Trim();

                    var address = parameter.Contact.Address == null ? "" : parameter.Contact.Address.Trim();
                    contactCus.Address = address; //BuildAddress(address, parameter.Contact.ProvinceId, parameter.Contact.DistrictId, parameter.Contact.WardId);
                    contactCus.ContactId = newContactId;
                    contactCus.ObjectId = newCustomerId;
                    contactCus.ObjectType = ObjectType.CUSTOMER;
                    contactCus.CreatedDate = DateTime.Now;
                    contactCus.CreatedById = parameter.UserId;
                    contactCus.UpdatedDate = DateTime.Now;
                    contactCus.UpdatedById = parameter.UserId;

                    #region Add by Dung

                    //customer.CustomerCode = parameter.Customer.CustomerCode?.Trim();
                    customer.CustomerName = parameter.Customer.CustomerName?.Trim();
                    contactCus.Email = parameter.Contact.Email?.Trim();
                    contactCus.Phone = parameter.Contact.Phone?.Trim();
                    contactCus.Note = parameter.Contact.Note?.Trim();
                    contactCus.CreatedById = parameter.UserId;
                    customer.IsFromLead = false;

                    if (!string.IsNullOrEmpty(parameter.Contact.Note))
                    {
                        var newNote = new Note();
                        newNote.NoteId = Guid.NewGuid();
                        newNote.Description = parameter.Contact.Note;
                        newNote.Type = "ADD";
                        newNote.ObjectId = newCustomerId;
                        newNote.ObjectType = "CUS";
                        newNote.Active = true;
                        newNote.CreatedById = parameter.UserId;
                        newNote.CreatedDate = DateTime.Now;
                        newNote.NoteTitle = "đã thêm ghi chú";

                        context.Note.Add(newNote);
                    }

                    #endregion

                    if (parameter.Customer.CustomerType == 1 || parameter.Customer.CustomerType == 3)
                    {
                        //Gán FirstName = CustomerName
                        contactCus.FirstName = parameter.Customer.CustomerName.Trim();
                        contactCus.FirstName = parameter.Customer.CustomerName.Trim();
                        contactCus.LastName = "";
                        contactCus.LastName = "";

                        //Thêm danh sách người liên hệ của khách hàng
                        if (parameter.CustomerContactList.Count > 0)
                        {
                            List<Contact> listContact = new List<Contact>();
                            parameter.CustomerContactList.ForEach(con =>
                            {
                                Contact contactCon = con.ToEntity();
                                contactCon.ContactId = Guid.NewGuid();
                                contactCon.ObjectId = newCustomerId;
                                contactCon.ObjectType = ObjectType.CUSTOMERCONTACT;
                                contactCon.FirstName = con.FirstName == null ? "" : con.FirstName.Trim();
                                contactCon.LastName = con.LastName == null ? "" : con.LastName.Trim();
                                contactCon.Phone = con.Phone == null ? "" : con.Phone.Trim();
                                contactCon.Email = con.Email == null ? "" : con.Email.Trim();
                                contactCon.Role = con.Role == null ? "" : con.Role.Trim();
                                contactCon.Address = con.Address; //BuildAddress(con.Address, con.ProvinceId, con.DistrictId, con.WardId);

                                contactCon.CreatedDate = DateTime.Now;
                                contactCon.UpdatedDate = null;
                                contactCon.CreatedById = parameter.UserId;
                                contactCon.UpdatedById = null;
                                listContact.Add(contactCon);
                            });

                            //FirstName, LastName, Phone không được để trống
                            bool contact_cus_con_invalid = false;
                            listContact.ForEach(item =>
                            {
                                if (string.IsNullOrEmpty(item.FirstName) || string.IsNullOrEmpty(item.LastName) || string.IsNullOrEmpty(item.Phone))
                                {
                                    contact_cus_con_invalid = true;
                                }
                            });

                            if (contact_cus_con_invalid)
                            {
                                return new CreateCustomerResult()
                                {
                                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                                    MessageCode = "Họ và Tên, Số điện thoại của người liên hệ không được để trống"
                                };
                            };

                            context.Contact.AddRange(listContact);

                            #region Warning duplicate contact customer

                            var customerContactCode = "CUS_CON";
                            var listPhoneCustomerContact = context.Contact.Where(w => w.ObjectType == customerContactCode).Select(w => new { Phone = w.Phone }).ToList();
                            var listPhone = new List<string>();
                            listPhoneCustomerContact?.ForEach(contact =>
                            {
                                listPhone.Add(contact.Phone);
                            });
                            listContact.ForEach(contact =>
                            {
                                bool duplicate = false;
                                duplicate = listPhone.Contains(contact.Phone);
                                if (duplicate == true)
                                {
                                    duplicateContact = true;
                                    return;
                                }
                            });

                            #endregion
                        }
                    }
                    else
                    {
                        //Nếu là Khách hàng cá nhân hoặc Khách hàng hộ kinh doanh
                        contactCus.FirstName = parameter.Contact.FirstName == null ? "" : parameter.Contact.FirstName.Trim();
                        contactCus.LastName = parameter.Contact.LastName == null ? "" : parameter.Contact.LastName.Trim();

                        if (string.IsNullOrEmpty(parameter.Contact.FirstName) || string.IsNullOrEmpty(parameter.Contact.LastName))
                        {
                            return new CreateCustomerResult()
                            {
                                StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                                MessageCode = "Họ và Tên người liên hệ không được để trống"
                            };
                        }

                        customer.CustomerName = parameter.Contact.FirstName.Trim() + " " + parameter.Contact.LastName.Trim();

                        customer.CustomerCode = parameter.Customer.CustomerCode == null ? "" : parameter.Customer.CustomerCode.Trim();
                        if (string.IsNullOrEmpty(parameter.Customer.CustomerCode))
                        {
                            //Nếu CustomerCode để trống thì auto generate CustomerCode
                            customer.CustomerCode = GenerateCustomerCode();
                        }
                        else
                        {
                            customer.CustomerCode = parameter.Customer.CustomerCode;
                        }
                    }

                    //Kiểm tra CustomerCode trong hệ thống
                    var dublicateCustomer = context.Customer.FirstOrDefault(x => x.CustomerCode == parameter.Customer.CustomerCode && x.Active == true);
                    if (dublicateCustomer != null)
                    {
                        return new CreateCustomerResult
                        {
                            StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                            MessageCode = "Khách hàng này đã tồn tại trên hệ thống",
                            ContactId = Guid.Empty,
                            CustomerId = Guid.Empty
                        };
                    }

                    #region Giang comment

                    //if (parameter.Customer.LeadId != null)
                    //{
                    //    //Nếu tạo khách hàng mà có thông tin trùng với một Lead thì cập nhật lại trạng thái của Lead là Ký hợp đồng
                    //    Lead lead = null;
                    //    lead = context.Lead.Where(w => w.LeadId == parameter.Customer.LeadId).FirstOrDefault();
                    //    var categoryType = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "TLE");
                    //    lead.StatusId = context.Category.Where(w => w.CategoryCode == "KHD" && w.CategoryTypeId == categoryType.CategoryTypeId).FirstOrDefault().CategoryId;
                    //    context.Lead.Update(lead);
                    //}

                    #endregion

                    #region Get Customer Infor To Send Email (Giang comment)

                    //SendEmailEntityModel.CustomerName = parameter.Contact.FirstName + " " + parameter.Contact.LastName;
                    //switch (parameter.Customer.CustomerType)
                    //{
                    //    case 1:
                    //        // khach hang doanh nghiep
                    //        SendEmailEntityModel.CustomerType = "Khách hàng doanh nghiệp";
                    //        break;
                    //    case 2:
                    //        // khach hang ca nhan
                    //        SendEmailEntityModel.CustomerType = "Khách hàng cá nhân";
                    //        break;
                    //    default:
                    //        SendEmailEntityModel.CustomerType = "";
                    //        break;
                    //}
                    //SendEmailEntityModel.CustomerGroup = context.Category.FirstOrDefault(w => w.CategoryId == parameter.Customer.CustomerGroupId)?.CategoryName ?? "";
                    //SendEmailEntityModel.CustomerCode = parameter.Customer?.CustomerCode ?? "";
                    //SendEmailEntityModel.CustomerEmail = parameter.Contact?.Email ?? "";
                    //SendEmailEntityModel.CustomerPhone = parameter.Contact?.Phone ?? "";
                    //var seller = context.Employee.FirstOrDefault(w => w.EmployeeId == parameter.Customer.PersonInChargeId)?.EmployeeName ?? "";
                    //SendEmailEntityModel.CustomerSeller = seller ?? "";
                    ////company infor
                    //var companyEntity = context.CompanyConfiguration.FirstOrDefault();
                    //SendEmailEntityModel.CompanyName = companyEntity?.CompanyName ?? "";
                    //SendEmailEntityModel.CompanyAddress = companyEntity?.CompanyAddress ?? "";
                    ////employee infor
                    //var employeeId = context.User.FirstOrDefault(e => e.UserId == parameter.UserId).EmployeeId;
                    //var emp = context.Employee.FirstOrDefault(e => e.EmployeeId == employeeId); //nhan vien tao
                    //SendEmailEntityModel.EmployeeCode = emp?.EmployeeCode ?? "";
                    //SendEmailEntityModel.EmployeeName = emp?.EmployeeName ?? "";
                    ////gửi email cho người tạo và người bán hàng
                    //var picEmail = context.Contact.Where(w => w.ObjectId == parameter.Customer.PersonInChargeId).FirstOrDefault()?.Email?.Trim();
                    //SendEmailEntityModel.ListSendToEmail.Add(picEmail);
                    //if (parameter.Customer.PersonInChargeId != employeeId)
                    //{
                    //    var empEmail = context.Contact.Where(w => w.ObjectId == emp.EmployeeId).FirstOrDefault()?.Email?.Trim();
                    //    SendEmailEntityModel.ListSendToEmail.Add(empEmail);
                    //}

                    #endregion

                    #region Check trùng email/sđt

                    if (!string.IsNullOrEmpty(parameter.Contact.Email))
                    {
                        var checkEmail = context.Contact.FirstOrDefault(x =>
                            x.Active == true &&
                            x.ObjectType == "CUS" &&
                            (x.Email ?? "").Trim().ToLower() == parameter.Contact.Email.Trim().ToLower());

                        if (checkEmail != null)
                        {
                            return new CreateCustomerResult()
                            {
                                StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                                MessageCode = "Email khách hàng đã tồn tại trên hệ thống"
                            };
                        }
                    }

                    if (!string.IsNullOrEmpty(parameter.Contact.Phone))
                    {
                        var checkPhone = context.Contact.FirstOrDefault(x =>
                            x.Active == true &&
                            x.ObjectType == "CUS" &&
                            (x.Phone ?? "").Trim().ToLower() == parameter.Contact.Phone.Trim().ToLower());

                        if (checkPhone != null)
                        {
                            return new CreateCustomerResult()
                            {
                                StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                                MessageCode = "Số điện thoại khách hàng đã tồn tại trên hệ thống"
                            };
                        }
                    }

                    #endregion

                    context.Customer.Add(customer);
                    context.Contact.Add(contactCus);
                    context.SaveChanges();

                    #endregion

                    #region Danh sách khách hàng

                    var employeeId = context.User.FirstOrDefault(x => x.UserId == parameter.UserId).EmployeeId;
                    var employee = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeId);
                    var statusCustomerType =
                        context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "THA").CategoryTypeId;
                    var statusCustomer = context.Category
                        .FirstOrDefault(x => x.CategoryCode == "HDO" && x.CategoryTypeId == statusCustomerType).CategoryId;
                    var listStatusCustomer = context.Category
                        .Where(x => x.CategoryTypeId == statusCustomerType).ToList();
                    var listEmployeeEntity = context.Employee.Where(x => x.Active == true).ToList();

                    var isManage = employee.IsManager;
                    if (isManage == true)
                    {
                        //Quản lí: lấy tất cả nhân viên phòng ban đó và phòng ban dưới cấp
                        var currentOrganization = employee.OrganizationId;
                        List<Guid?> listOrganizationChildrenId = new List<Guid?>();
                        listOrganizationChildrenId.Add(currentOrganization);
                        var organizationList = context.Organization.Where(w => w.Active == true).ToList();
                        getOrganizationChildrenId(organizationList, currentOrganization, listOrganizationChildrenId);
                        var listEmployeeId = listEmployeeEntity
                            .Where(w => listOrganizationChildrenId.Contains(w.OrganizationId)).Select(w => w.EmployeeId)
                            .ToList();

                        ListCustomer = context.Customer
                            .Where(x => x.Active == true && (x.CustomerType == 1 || x.CustomerType == 3) &&
                                        //x.StatusId == statusCustomer &&
                                        listEmployeeId.Contains(x.PersonInChargeId.Value)).Select(y =>
                                new CustomerEntityModel
                                {
                                    CustomerId = y.CustomerId,
                                    CustomerName = y.CustomerName,
                                    StatusCode = listStatusCustomer.FirstOrDefault(p => p.CategoryId == y.StatusId).CategoryCode
                                }).ToList();
                    }
                    else
                    {
                        //Nhân viên: chỉ lấy nhân viên đó
                        ListCustomer = context.Customer
                            .Where(x => x.Active == true && (x.CustomerType == 1 || x.CustomerType == 3) &&
                                        //x.StatusId == statusCustomer &&
                                        x.PersonInChargeId == employeeId).Select(y =>
                                new CustomerEntityModel
                                {
                                    CustomerId = y.CustomerId,
                                    CustomerName = y.CustomerName,
                                    StatusCode = listStatusCustomer.FirstOrDefault(p => p.CategoryId == y.StatusId).CategoryCode
                                }).ToList();
                    }

                    #endregion
                }
                //Tạo khách hàng tiềm năng
                else
                {
                    var careStateTypeId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "TTCS")
                        ?.CategoryTypeId;
                    var notCallId = context.Category
                        .FirstOrDefault(c => c.CategoryTypeId == careStateTypeId && c.CategoryCode == "CGD")?.CategoryId;

                    var supportTypeId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "TTPKHTN")
                        ?.CategoryTypeId;
                    var newSupportId = context.Category
                        .FirstOrDefault(c => c.CategoryTypeId == supportTypeId && c.CategoryCode == "A")?.CategoryId;

                    #region Thêm khách hàng tiềm năng

                    newCusId = context.Category.FirstOrDefault(c => c.CategoryTypeId == cusSttId && c.CategoryCode == "MOI").CategoryId;

                    customer.CustomerId = newCustomerId;
                    customer.StatusSuportId = newSupportId;
                    customer.StatusId = newCusId;
                    customer.CreatedDate = DateTime.Now;
                    customer.CreatedById = parameter.UserId;
                    customer.UpdatedDate = DateTime.Now;
                    customer.UpdatedById = parameter.UserId;

                    contactCus.TaxCode = parameter.Contact.TaxCode == null ? "" : parameter.Contact.TaxCode.Trim();

                    var address = parameter.Contact.Address == null ? "" : parameter.Contact.Address.Trim();
                    contactCus.Address = address; //BuildAddress(address, parameter.Contact.ProvinceId, parameter.Contact.DistrictId, parameter.Contact.WardId);

                    contactCus.CreatedDate = DateTime.Now;
                    contactCus.CreatedById = parameter.UserId;
                    contactCus.UpdatedDate = DateTime.Now;
                    contactCus.UpdatedById = parameter.UserId;
                    contactCus.ContactId = newContactId;
                    contactCus.ObjectId = newCustomerId;
                    contactCus.ObjectType = ObjectType.CUSTOMER;

                    #region Add by Dung

                    customer.CustomerCode = parameter.Customer.CustomerCode?.Trim();
                    customer.CustomerName = parameter.Customer.CustomerName?.Trim();
                    contactCus.Email = parameter.Contact.Email?.Trim();
                    contactCus.Phone = parameter.Contact.Phone?.Trim();
                    contactCus.Note = parameter.Contact.Note?.Trim();
                    contactCus.CreatedById = parameter.UserId;
                    customer.IsFromLead = true;
                    customer.CareStateId = notCallId;

                    if (!string.IsNullOrEmpty(parameter.Contact.Note))
                    {
                        var newNote = new Note();
                        newNote.NoteId = Guid.NewGuid();
                        newNote.Description = parameter.Contact.Note;
                        newNote.Type = "ADD";
                        newNote.ObjectId = newCustomerId;
                        newNote.ObjectType = "CUS";
                        newNote.Active = true;
                        newNote.CreatedById = parameter.UserId;
                        newNote.CreatedDate = DateTime.Now;
                        newNote.NoteTitle = "đã thêm ghi chú";

                        context.Note.Add(newNote);
                    }

                    #endregion

                    if (parameter.Customer.CustomerType == 1 || parameter.Customer.CustomerType == 3)
                    {
                        //Gán FirstName = CustomerName
                        contactCus.FirstName = parameter.Customer.CustomerName.Trim();
                        contactCus.LastName = "";
                        if (parameter.CustomerContactList.Count > 0)
                        {
                            List<Contact> listContact = new List<Contact>();

                            parameter.CustomerContactList.ForEach(con =>
                            {
                                Contact contactCon = con.ToEntity();
                                contactCon.ContactId = Guid.NewGuid();
                                contactCon.ObjectId = newCustomerId;
                                contactCon.ObjectType = ObjectType.CUSTOMERCONTACT;
                                contactCon.FirstName = con.FirstName == null ? "" : con.FirstName.Trim();
                                contactCon.LastName = con.LastName == null ? "" : con.LastName.Trim();
                                contactCon.Phone = con.Phone == null ? "" : con.Phone.Trim();
                                contactCon.Email = con.Email == null ? "" : con.Email.Trim();
                                contactCon.Role = con.Role == null ? "" : con.Role.Trim();

                                contactCon.CreatedDate = DateTime.Now;
                                contactCon.UpdatedDate = null;
                                contactCon.CreatedById = parameter.UserId;
                                contactCon.UpdatedById = null;
                                listContact.Add(contactCon);
                            });

                            //FirstName, LastName, Phone không được để trống
                            bool contact_cus_con_invalid = false;
                            listContact.ForEach(item =>
                            {
                                if (string.IsNullOrEmpty(item.FirstName) || string.IsNullOrEmpty(item.Phone))
                                {
                                    contact_cus_con_invalid = true;
                                }
                            });

                            if (contact_cus_con_invalid)
                            {
                                return new CreateCustomerResult()
                                {
                                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                                    MessageCode = "Họ và Tên, Số điện thoại của người liên hệ không được để trống"
                                };
                            };

                            context.Contact.AddRange(listContact);

                            #region Warning duplicate contact customer

                            var customerContactCode = "CUS_CON";
                            var listPhoneCustomerContact = context.Contact.Where(w => w.ObjectType == customerContactCode).Select(w => new { Phone = w.Phone }).ToList();
                            var listPhone = new List<string>();
                            listPhoneCustomerContact?.ForEach(contact =>
                            {
                                listPhone.Add(contact.Phone);
                            });
                            listContact.ForEach(contact =>
                            {
                                bool duplicate = false;
                                duplicate = listPhone.Contains(contact.Phone);
                                if (duplicate == true)
                                {
                                    duplicateContact = true;
                                    return;
                                }
                            });

                            #endregion
                        }
                    }
                    else
                    {
                        //Nếu là Khách hàng cá nhân
                        contactCus.FirstName = parameter.Contact.FirstName == null ? "" : parameter.Contact.FirstName.Trim();
                        contactCus.LastName = parameter.Contact.LastName == null ? "" : parameter.Contact.LastName.Trim();

                        customer.CustomerName = parameter.Contact.FirstName.Trim() + " " + parameter.Contact.LastName.Trim();

                        customer.CustomerCode = parameter.Customer.CustomerCode == null ? "" : parameter.Customer.CustomerCode.Trim();
                    }

                    //Nếu người dùng không nhập mã khách hàng => Hệ thống sẽ tự sinh mã
                    if (string.IsNullOrEmpty(parameter.Customer.CustomerCode))
                    {
                        var appName = context.SystemParameter.FirstOrDefault(x => x.SystemKey == "ApplicationName")
                            ?.SystemValueString;

                        if (appName == "VNS")
                        {
                            customer.CustomerCode = GenerateVNSCode();
                        }
                        else
                        {
                            customer.CustomerCode = this.GenerateCustomerCode(0);
                        }
                    }
                    else
                    {
                        customer.CustomerCode = parameter.Customer.CustomerCode;
                    }

                    //Kiểm tra CustomerCode trong hệ thống
                    var dublicateCustomer = context.Customer.FirstOrDefault(x => x.CustomerCode == parameter.Customer.CustomerCode && x.Active == true);
                    if (dublicateCustomer != null)
                    {
                        return new CreateCustomerResult
                        {
                            StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                            MessageCode = "Khách hàng này đã tồn tại trên hệ thống",
                            ContactId = Guid.Empty,
                            CustomerId = Guid.Empty
                        };
                    }

                    #region Check trùng email/sđt

                    if (!string.IsNullOrEmpty(parameter.Contact.Email))
                    {
                        var checkEmail = context.Contact.FirstOrDefault(x =>
                            x.ObjectType == "CUS" &&
                            (x.Email ?? "").Trim().ToLower() == parameter.Contact.Email.Trim().ToLower());

                        if (checkEmail != null)
                        {
                            return new CreateCustomerResult()
                            {
                                StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                                MessageCode = "Email khách hàng đã tồn tại trên hệ thống"
                            };
                        }
                    }

                    if (!string.IsNullOrEmpty(parameter.Contact.Phone))
                    {
                        var checkPhone = context.Contact.FirstOrDefault(x =>
                            x.ObjectType == "CUS" &&
                            (x.Phone ?? "").Trim().ToLower() == parameter.Contact.Phone.Trim().ToLower());

                        if (checkPhone != null)
                        {
                            return new CreateCustomerResult()
                            {
                                StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                                MessageCode = "Số điện thoại khách hàng đã tồn tại trên hệ thống"
                            };
                        }
                    }

                    #endregion

                    context.Customer.Add(customer);
                    context.Contact.Add(contactCus);
                    context.SaveChanges();

                    #endregion

                    #region Danh sách khách hàng tiềm năng

                    var employeeId = context.User.FirstOrDefault(x => x.UserId == parameter.UserId).EmployeeId;
                    var employee = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeId);
                    var statusCustomerType =
                        context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "THA").CategoryTypeId;
                    var statusCustomer = context.Category
                        .FirstOrDefault(x => x.CategoryCode == "MOI" && x.CategoryTypeId == statusCustomerType).CategoryId;
                    var listStatusCustomer = context.Category
                        .Where(x => x.CategoryCode == "MOI" && x.CategoryTypeId == statusCustomerType).ToList();
                    var listEmployeeEntity = context.Employee.Where(x => x.Active == true).ToList();

                    var isManage = employee.IsManager;
                    if (isManage == true)
                    {
                        //Quản lí: lấy tất cả nhân viên phòng ban đó và phòng ban dưới cấp
                        var currentOrganization = employee.OrganizationId;
                        List<Guid?> listOrganizationChildrenId = new List<Guid?>();
                        listOrganizationChildrenId.Add(currentOrganization);
                        var organizationList = context.Organization.Where(w => w.Active == true).ToList();
                        getOrganizationChildrenId(organizationList, currentOrganization, listOrganizationChildrenId);
                        var listEmployeeId = listEmployeeEntity
                            .Where(w => listOrganizationChildrenId.Contains(w.OrganizationId)).Select(w => w.EmployeeId)
                            .ToList();

                        ListCustomer = context.Customer
                            .Where(x => x.Active == true && x.CustomerType == 1 &&
                                        //x.StatusId == statusCustomer &&
                                        listEmployeeId.Contains(x.PersonInChargeId.Value)).Select(y =>
                                new CustomerEntityModel
                                {
                                    CustomerId = y.CustomerId,
                                    CustomerName = y.CustomerName,
                                    StatusCode = listStatusCustomer.FirstOrDefault(p => p.CategoryId == y.StatusId).CategoryCode
                                }).ToList();
                    }
                    else
                    {
                        //Nhân viên: chỉ lấy nhân viên đó
                        ListCustomer = context.Customer
                            .Where(x => x.Active == true && x.CustomerType == 1 &&
                                        //x.StatusId == statusCustomer &&
                                        x.PersonInChargeId == employeeId).Select(y =>
                                new CustomerEntityModel
                                {
                                    CustomerId = y.CustomerId,
                                    CustomerName = y.CustomerName,
                                    StatusCode = listStatusCustomer.FirstOrDefault(p => p.CategoryId == y.StatusId).CategoryCode
                                }).ToList();
                    }

                    #endregion
                }
                #region Tạo account cho khách hàng
                // Tao account khach hang
                using (var transaction = context.Database.BeginTransaction())
                {
                    User user = new User()
                    {
                        Disabled = parameter.User.Disabled != null ? parameter.User.Disabled.Value : false,
                        ResetCode = parameter.User.ResetCode,
                        ResetCodeDate = parameter.User.ResetCodeDate
                    };

                    var passDefault = context.SystemParameter.FirstOrDefault(x => x.SystemKey == "DefaultUserPassword")
                        ?.SystemValueString?.Trim();

                    if (string.IsNullOrWhiteSpace(passDefault))
                    {
                        return new CreateCustomerResult()
                        {
                            MessageCode = "Chưa cấu hình mật khẩu mặc định trên hệ thống",
                            StatusCode = HttpStatusCode.ExpectationFailed,
                        };
                    }


                    user.UserId = Guid.NewGuid();
                    user.EmployeeId = newCustomerId;
                    user.UserName = newUserName;
                    user.Password = AuthUtil.GetHashingPassword(passDefault);
                    user.CreatedDate = DateTime.Now;
                    user.Active = true;
                    user.CreatedById = parameter.UserId;

                    context.User.Add(user);
                    context.SaveChanges();
                    transaction.Commit();
                }
                #endregion
            }
            catch (Exception e)
            {
                return new CreateCustomerResult()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }

            if (parameter.IsFromLead == false)
            {
                #region Log

                LogHelper.AuditTrace(context, "Create", "CUSTOMER", customer.CustomerId, parameter.UserId);

                #endregion

                #region Gửi thông báo

                //NotificationHelper.AccessNotification(context, TypeModel.Customer, "CRE", new Customer(),
                //    customer, true);

                #endregion
            }
            else
            {
                #region Log

                LogHelper.AuditTrace(context, "Create", "POTENTIAL_CUSTOMER", customer.CustomerId, customer.CreatedById);

                #endregion

                #region Gửi thông báo

                NotificationHelper.AccessNotification(context, TypeModel.PotentialCustomer, "CRE", new Customer(),
                    customer, true);

                #endregion
            }

            var fullAddress = BuildAddress(parameter.Contact.Address, parameter.Contact.ProvinceId, parameter.Contact.DistrictId, parameter.Contact.WardId);

            return new CreateCustomerResult()
            {
                Message = CommonMessage.Customer.CREATE_SUCCESS,
                StatusCode = System.Net.HttpStatusCode.OK,
                MessageCode = CommonMessage.Customer.CREATE_SUCCESS,
                ContactId = newContactId,
                CustomerId = newCustomerId,
                DuplicateContact = duplicateContact,
                ListCustomer = ListCustomer,
                Address = fullAddress,
            };

        }

        private string GenerateCustomerCode()
        {
            var customerCode = "00001";
            var lastCustomerCode = context.Customer.LastOrDefault().CustomerCode;
            var isNumeric = int.TryParse(lastCustomerCode, out _);
            if (isNumeric && lastCustomerCode == customerCode)
            {
                var count = lastCustomerCode.Count();
                customerCode = (Int32.Parse(lastCustomerCode) + 1).ToString($"D{count}");
            }
            return customerCode;
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

        public SearchCustomerResult SearchCustomer(SearchCustomerParameter parameter)
        {
            try
            {
                var appName = context.SystemParameter.FirstOrDefault(x => x.SystemKey == "ApplicationName")
                    .SystemValueString;

                List<CustomerEntityModel> listCustomer = new List<CustomerEntityModel>();

                #region Referencer của CustomerId
                var contact = context.Contact.ToList();
                #endregion

                var listAllUser = context.User.ToList();
                var listAllEmployee = context.Employee.ToList();
                var listAllContact = context.Contact.ToList();

                //check isManager
                var user = listAllUser.FirstOrDefault(x => x.UserId == parameter.UserId && x.Active == true);
                if (user == null)
                {
                    return new SearchCustomerResult
                    {
                        StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                        MessageCode = "User không có quyền truy xuất dữ liệu trong hệ thống"
                    };
                }
                if (user.EmployeeId == null || user.EmployeeId == Guid.Empty)
                {
                    return new SearchCustomerResult
                    {
                        StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                        MessageCode = "Lỗi dữ liệu"
                    };
                }

                var employeeId = user.EmployeeId;
                var employee = listAllEmployee.FirstOrDefault(x => x.EmployeeId == employeeId);
                var isManager = employee.IsManager;
               
                var fromDate = parameter.FromDate;                              // Thời gian tạo từ
                var toDate = parameter.ToDate;                                  // Thời gian tạo đến    
                var fullName = (parameter.FirstName == null || parameter.FirstName == "") ? "" : parameter.FirstName.Trim();
                var phone = parameter.Phone == null ? "" : parameter.Phone.Trim();
                var email = parameter.Email == null ? "" : parameter.Email.Trim();
                var address = parameter.Address == null ? "" : parameter.Address.Trim();

                List<Customer> listAllCustomer = new List<Customer>();

                // lấy kh theo nhóm kh, nv phụ trách              
                listAllCustomer = context.Customer.Where(x => (x.Active == true) &&
                                                              (parameter.FromDate == null || parameter.FromDate.Value.Date <= x.CreatedDate.Date) &&
                                                              (parameter.ToDate == null || parameter.ToDate.Value >= x.CreatedDate.Date)
                                                        ).ToList();

                //List Customer Type: CUS and CUS_CON
                //List<string> listContactCustomerObjectType = new List<string>();
                //listContactCustomerObjectType.Add(ObjectType.CUSTOMER);
                //listContactCustomerObjectType.Add(ObjectType.CUSTOMERCONTACT);

                //Lấy tất cả contact của KH (CUS và CUS_CON)
                var listAllCustomerContact = context.Contact.Where(x => (x.Active == true) &&
                                                                        //(listContactCustomerObjectType.Contains(x.ObjectType)) &&
                                                                        (fullName == "" || ((x.FirstName ?? "").ToLower() + " " + (x.LastName ?? "").ToLower()).Contains(fullName.ToLower())) &&
                                                                        (email == "" || (x.Email != null && x.Email.ToLower().Contains(email.ToLower())) || (x.WorkEmail != null && x.WorkEmail.ToLower().Contains(email.ToLower())) || (x.OtherEmail != null && x.OtherEmail.ToLower().Contains(email.ToLower()))) &&
                                                                        (phone == "" || (x.Phone != null && x.Phone.ToLower().Contains(phone.ToLower())) || (x.WorkPhone != null && x.WorkPhone.ToLower().Contains(phone.ToLower())) || (x.OtherPhone != null && x.OtherPhone.ToLower().Contains(phone.ToLower()))) &&
                                                                        (address == "" || (x.Address != null && x.Address.ToLower().Contains(address.ToLower())))
                                                                  ).ToList();

                //Lọc ra các ObjectId bị trùng
                List<Contact> listCustomerContact = new List<Contact>();
                //List<Guid> listObjectId = new List<Guid>();
                //listAllCustomerContact.ForEach(item =>
                //{
                //    if (item.ContactId != null && item.ContactId != Guid.Empty)
                //    {
                //        var dupblicateObjectId = listObjectId.FirstOrDefault(x => x == item.ObjectId);
                //        if (dupblicateObjectId == Guid.Empty)
                //        {
                //            listObjectId.Add(item.ObjectId);
                //        }
                //    }
                //});

                //Lấy lại contact của listObjectId với ObjectType = CUS
                //if (listObjectId.Count > 0)
                //{
                listCustomerContact = listAllContact.Where(x => x.ObjectType == ObjectType.CUSTOMER).ToList();
                //}

                var listAllProvince = context.Province.ToList();
                if (isManager)
                {
                    //Lấy list phòng ban con của user
                    List<Guid?> listGetAllChild = new List<Guid?>();    //List phòng ban: chính nó và các phòng ban cấp dưới của nó
                    if (employee.OrganizationId != null && isManager)
                    {
                        listGetAllChild.Add(employee.OrganizationId.Value);
                        listGetAllChild = getOrganizationChildrenId(employee.OrganizationId.Value, listGetAllChild);
                    }
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

                    //Nếu là quản lý
                    //listAllCustomer = listAllCustomer.Where(x => (x.PersonInChargeId != null && (listEmployeeInChargeByManagerId == null || listEmployeeInChargeByManagerId.Count == 0 || listEmployeeInChargeByManagerId.FirstOrDefault(y => y.Equals(x.PersonInChargeId.Value)) != Guid.Empty)) ||
                    //                                             (x.PersonInChargeId == null && (listUserByManagerId == null || listUserByManagerId.Count == 0 || listUserByManagerId.FirstOrDefault(y => y.Equals(x.CreatedById)) != Guid.Empty))
                    //                                       ).ToList();

                    List<Customer> listAllCustomerForPersonInChargeId = new List<Customer>();
                    List<Customer> listAllCustomerForCreatedById = new List<Customer>();

                    listAllCustomer.ForEach(item =>
                    {
                        var customer_contact = listCustomerContact.FirstOrDefault(x => x.ObjectId == item.CustomerId);
                        if (customer_contact != null)
                        {
                            CustomerEntityModel customer = new CustomerEntityModel();
                            var personInCharge = listAllEmployee.FirstOrDefault(x => x.EmployeeId == item.PersonInChargeId);

                            customer.CustomerId = item.CustomerId;
                            customer.CustomerCode = item.CustomerCode != null ? item.CustomerCode.Trim() : "";
                            customer.CustomerName = item.CustomerName != null ? item.CustomerName.Trim() : "";
                            customer.ContactId = customer_contact.ContactId;
                            customer.CustomerEmail = customer_contact.Email != null ? customer_contact.Email.Trim() : "";
                            customer.CustomerPhone = customer_contact.Phone != null ? customer_contact.Phone.Trim() : "";
                            customer.CreatedDate = item.CreatedDate;
                            customer.UpdatedDate = item.UpdatedDate;
                            customer.ProvinceId = customer_contact.ProvinceId;
                            customer.ProvinceName = customer_contact.ProvinceId != null ? listAllProvince.FirstOrDefault(x => x.ProvinceId == customer_contact.ProvinceId).ProvinceName : "";
                            listCustomer.Add(customer);
                        }
                    });
                }
                else
                {
                    //Nếu không phải quản lý
                    listAllCustomer = listAllCustomer.Where(x => (x.PersonInChargeId != null && x.PersonInChargeId == employeeId) || (x.PersonInChargeId == null && x.CreatedById.Equals(user.UserId))).ToList();

                    listAllCustomer.ForEach(item =>
                    {
                        var customer_contact = listCustomerContact.FirstOrDefault(x => x.ObjectId == item.CustomerId);
                        if (customer_contact != null)
                        {
                            CustomerEntityModel customer = new CustomerEntityModel();
                            var personInCharge = listAllEmployee.FirstOrDefault(x => x.EmployeeId == item.PersonInChargeId);

                            customer.CustomerId = item.CustomerId;
                            customer.CustomerCode = item.CustomerCode != null ? item.CustomerCode.Trim() : "";
                            customer.CustomerName = item.CustomerName != null ? item.CustomerName.Trim() : "";
                            customer.ContactId = customer_contact.ContactId;
                            customer.CustomerEmail = customer_contact.Email != null ? customer_contact.Email.Trim() : "";
                            customer.CustomerPhone = customer_contact.Phone != null ? customer_contact.Phone.Trim() : "";
                            customer.CreatedDate = item.CreatedDate;
                            customer.UpdatedDate = item.UpdatedDate;
                            customer.ProvinceId = customer_contact.ProvinceId;
                            customer.ProvinceName = customer_contact.ProvinceId != null ? listAllProvince.FirstOrDefault(x => x.ProvinceId == customer_contact.ProvinceId).ProvinceName : "";
                            listCustomer.Add(customer);
                        }
                    });
                }

                if(parameter.ListProvinceId != null)
                {
                    if(parameter.ListProvinceId.Count != 0)
                    listCustomer = listCustomer.Where(x => x.ProvinceId != null && parameter.ListProvinceId.Contains(x.ProvinceId.Value)).OrderByDescending(x => x.CreatedDate).ToList();
                }


                return new SearchCustomerResult()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    ListCustomer = listCustomer,
                    MessageCode = "Success"
                };
            }
            catch (Exception)
            {
                return new SearchCustomerResult
                {
                    MessageCode = "Đã có lỗi xảy ra trong quá trình tìm kiếm",
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
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

        public GetCustomerFromOrderCreateResult GetCustomerFromOrderCreate(GetCustomerFromOrderCreateParameter parameter)
        {
            try
            {
                //var customerList = (from cus in context.Customer
                //                    join c in context.Contact on cus.CustomerId equals c.ObjectId
                //                    where
                //                    (parameter.FirstName == null || c.FirstName == parameter.FirstName.Trim() || c.FirstName.Contains(parameter.FirstName.Trim()) || cus.CustomerName.Contains(parameter.FirstName.Trim()) || parameter.FirstName.Trim() == "") &&
                //                      (parameter.LastName == null || c.LastName == parameter.LastName.Trim() || c.LastName.Contains(parameter.LastName.Trim()) || cus.CustomerName.Contains(parameter.LastName.Trim()) || parameter.LastName.Trim() == "") &&
                //                      (parameter.Email == null || c.Email == parameter.Email.Trim() || c.Email.Contains(parameter.Email.Trim()) || parameter.Email.Trim() == "") &&
                //                      (parameter.Phone == null || c.Phone == parameter.Phone.Trim() || c.Phone.Contains(parameter.Phone.Trim()) || parameter.Phone.Trim() == "") &&
                //                      (parameter.PersonInChargeIdList == null || parameter.PersonInChargeIdList.Count == 0 || parameter.PersonInChargeIdList.Contains(cus.PersonInChargeId)) &&
                //                      (parameter.CustomerGroupIdList == null || parameter.CustomerGroupIdList.Count == 0 || parameter.CustomerGroupIdList.Contains(cus.CustomerGroupId)) &&
                //                     c.ObjectType == ObjectType.CUSTOMER
                //                    select new CustomerEntityModel
                //                    {
                //                        CustomerId = cus.CustomerId,
                //                        CustomerCode = cus.CustomerCode,
                //                        ContactId = c.ContactId,
                //                        CustomerGroupId = cus.CustomerGroupId,
                //                        CustomerName = cus.CustomerName,
                //                        StatusName = context.Category.FirstOrDefault(ct => ct.CategoryId == cus.StatusId).CategoryName,
                //                        CustomerServiceLevelName = context.CustomerServiceLevel.
                //                                    FirstOrDefault(csl => csl.CustomerServiceLevelId == cus.CustomerServiceLevelId).CustomerServiceLevelName,
                //                        PersonInChargeId = cus.PersonInChargeId,
                //                        PicName = context.Employee.FirstOrDefault(e => e.EmployeeId == cus.PersonInChargeId) != null ?
                //                                    context.Employee.FirstOrDefault(e => e.EmployeeId == cus.PersonInChargeId).EmployeeName : "",
                //                        CustomerEmail = c.Email,
                //                        CustomerPhone = c.Phone,
                //                        PicAvatarUrl = context.Contact.FirstOrDefault(co => co.ObjectId == cus.PersonInChargeId) != null ?
                //                                    context.Contact.FirstOrDefault(co => co.ObjectId == cus.PersonInChargeId).AvatarUrl : "",
                //                        CusAvatarUrl = context.Contact.FirstOrDefault(co => co.ObjectId == cus.CustomerId && co.ObjectType == ObjectType.CUSTOMER) != null ?
                //                                    context.Contact.FirstOrDefault(co => co.ObjectId == cus.CustomerId && co.ObjectType == ObjectType.CUSTOMER).AvatarUrl : "",
                //                        CustomerType = cus.CustomerType,
                //                        CreatedById = cus.CreatedById,
                //                        CreatedDate = cus.CreatedDate,
                //                        StatusId = cus.StatusId,
                //                        TotalSaleValue = cus.TotalSaleValue,
                //                        CustomerServiceLevelId = null
                //                    }).OrderByDescending(date => date.CreatedDate).ToList();

                #region Edit By Hung
                var customerList = (from c in context.Contact
                                    where c.ObjectType == ObjectType.CUSTOMER
                                    select new CustomerEntityModel
                                    {
                                        CustomerId = c.ObjectId,
                                        CustomerCode = "",//cus.CustomerCode,
                                        ContactId = c.ContactId,
                                        CustomerGroupId = Guid.Empty,
                                        CustomerName = "",
                                        StatusName = "",//context.Category.FirstOrDefault(ct => ct.CategoryId == cus.StatusId).CategoryName, CustomerServiceLevelName = context.CustomerServiceLevel.FirstOrDefault(csl => csl.CustomerServiceLevelId == cus.CustomerServiceLevelId).CustomerServiceLevelName,
                                        PersonInChargeId = Guid.Empty,//cus.PersonInChargeId,
                                        PicName = "",//context.Employee.FirstOrDefault(e => e.EmployeeId == cus.PersonInChargeId) != null ? context.Employee.FirstOrDefault(e => e.EmployeeId == cus.PersonInChargeId).EmployeeName : "",
                                        CustomerEmail = c.Email,
                                        CustomerPhone = c.Phone,
                                        PicAvatarUrl = "",//context.Contact.FirstOrDefault(co => co.ObjectId == cus.PersonInChargeId) != null ? context.Contact.FirstOrDefault(co => co.ObjectId == cus.PersonInChargeId).AvatarUrl : "",
                                        CusAvatarUrl = "",// context.Contact.FirstOrDefault(co => co.ObjectId == cus.CustomerId && co.ObjectType == ObjectType.CUSTOMER) != null ? context.Contact.FirstOrDefault(co => co.ObjectId == cus.CustomerId && co.ObjectType == ObjectType.CUSTOMER).AvatarUrl : "",
                                        CustomerType = 0,//cus.CustomerType,
                                        CreatedById = Guid.Empty,//cus.CreatedById,
                                        CreatedDate = DateTime.Now,//cus.CreatedDate,
                                        StatusId = Guid.Empty,//cus.StatusId,
                                        TotalSaleValue = null,
                                        CustomerServiceLevelId = null
                                    }).OrderByDescending(date => date.CreatedDate).ToList();

                List<Guid> customerIdList = new List<Guid>();
                List<Guid> categoryIdList = new List<Guid>();
                List<Guid> employeeIdList = new List<Guid>();

                customerList.ForEach(item =>
                {
                    if (!customerIdList.Contains(item.CustomerId))
                        customerIdList.Add(item.CustomerId);
                });
                var customerTmpList = context.Customer.Where(w => customerIdList.Contains(w.CustomerId)).ToList();
                customerList.ForEach(item =>
                {
                    Customer customer = customerTmpList.FirstOrDefault(f => f.CustomerId == item.CustomerId);
                    item.CustomerCode = customer.CustomerCode;
                    item.CustomerName = customer.CustomerName;
                    item.StatusName = customer.CustomerName;
                    item.PersonInChargeId = customer.PersonInChargeId;
                    item.PicName = customer.CustomerName;
                    item.PicAvatarUrl = customer.CustomerName;
                    item.CusAvatarUrl = customer.CustomerName;
                });
                #endregion

                // Get list customer level config level
                var levelConfigs = context.CustomerServiceLevel.OrderByDescending(c => c.MinimumSaleValue).ToList();
                // Get level customer
                foreach (var level in levelConfigs)
                {
                    foreach (var customerServiceLevel in customerList)
                    {
                        if (customerServiceLevel.TotalSaleValue >= level.MinimumSaleValue)
                        {
                            customerServiceLevel.CustomerServiceLevelName = level.CustomerServiceLevelName;
                            customerServiceLevel.CustomerServiceLevelId = level.CustomerServiceLevelId;
                            break;
                        }
                        else if (string.IsNullOrEmpty(customerServiceLevel.CustomerServiceLevelName))
                        {
                            customerServiceLevel.CustomerServiceLevelName = "Chưa đạt hạng";
                            customerServiceLevel.CustomerServiceLevelId = Guid.Empty;
                        }
                    }
                }

                if (!parameter.IsBusinessCus)
                {
                    customerList = customerList.Where(cl => cl.CustomerType == 2 || cl.CustomerType == 3).ToList();
                }

                if (!parameter.IsPersonalCus)
                {
                    customerList = customerList.Where(cl => cl.CustomerType == 1 || cl.CustomerType == 3).ToList();
                }
                if (!parameter.IsHKDCus)
                {
                    customerList = customerList.Where(cl => cl.CustomerType == 1 || cl.CustomerType == 2).ToList();
                }

                if (parameter.NoPic)
                {
                    customerList = customerList.Where(cl => cl.PersonInChargeId == null || cl.PersonInChargeId == Guid.Empty).ToList();
                }

                customerList = customerList.Where(c => parameter.CustomerServiceLevelIdList.Contains(c.CustomerServiceLevelId) ||
                    parameter.CustomerServiceLevelIdList.Count == 0 && c.CustomerName != null).ToList();

                var numberOfRecord = customerList.Count;
                return new GetCustomerFromOrderCreateResult()
                {
                    StatusCode = numberOfRecord > 0 ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.OK,
                    Customer = customerList,
                    MessageCode = numberOfRecord > 0 ? "" : CommonMessage.Customer.NO_CUS
                };
            }
            catch (Exception e)
            {
                return new GetCustomerFromOrderCreateResult()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetAllCustomerServiceLevelResult GetAllCustomerServiceLevel(GetAllCustomerServiceLevelParameter parameter)
        {
            try
            {
                var customerServiceLevelList = context.CustomerServiceLevel.Select(c => new CustomerServiceLevelEntityModel()
                {
                    CustomerServiceLevelId = c.CustomerServiceLevelId,
                    CustomerServiceLevelName = c.CustomerServiceLevelName,
                    CustomerServiceLevelCode = c.CustomerServiceLevelCode,
                    MinimumSaleValue = c.MinimumSaleValue,
                    CreatedDate = c.CreatedDate,
                    UpdatedById = c.UpdatedById,
                    UpdatedDate = c.UpdatedDate,
                    Active = c.Active,
                }).ToList();
                return new GetAllCustomerServiceLevelResult()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    CustomerServiceLevelList = customerServiceLevelList
                };
            }
            catch (Exception e)
            {
                return new GetAllCustomerServiceLevelResult()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetAllCustomerCodeResult GetAllCustomerCode(GetAllCustomerCodeParameter parameter)
        {
            try
            {
                var lst = context.Customer.Select(c => c.CustomerCode.ToLower()).ToList();
                if (parameter.Mode == "edit")
                {
                    lst.RemoveAll(l => l == parameter.Code.ToLower());
                }
                return new GetAllCustomerCodeResult()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    CustomerCodeList = lst
                };
            }
            catch (Exception e)
            {
                return new GetAllCustomerCodeResult()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public GetCustomerByIdResult GetCustomerById(GetCustomerByIdParameter parameter)
        {
            try
            {
                var appName = context.SystemParameter.FirstOrDefault(x => x.SystemKey == "ApplicationName")
                    .SystemValueString;

                var listAllEmploye = context.Employee.ToList();
                var listEmployee = listAllEmploye.Where(e => e.Active == true).ToList();

                var listUser = context.User.ToList();
                var user = listUser.FirstOrDefault(x => x.UserId == parameter.UserId);
                var employee = listEmployee.FirstOrDefault(x => x.EmployeeId == user.EmployeeId);

                var listCategoryType = context.CategoryType.ToList();
                var listCategory = context.Category.ToList();

                var customer = context.Customer.FirstOrDefault(cu => cu.CustomerId == parameter.CustomerId);

                #region Lấy list phòng ban con của user

                /*
                 * List phòng ban bao gồm: chính phòng ban nó đang thuộc và các phòng ban cấp dưới của nó nếu có
                 */
                List<Guid?> listGetAllChild = new List<Guid?>();
                listGetAllChild.Add(employee.OrganizationId.Value);
                listGetAllChild = getOrganizationChildrenId(employee.OrganizationId.Value, listGetAllChild);

                #endregion

                #region Các reference của customer
                var note = context.Note.Where(cu => cu.ObjectId == parameter.CustomerId).ToList();
                var contacts = context.Contact.Where(c => (c.ObjectType == ObjectType.CUSTOMER || c.ObjectType == ObjectType.CUSTOMERCONTACT) && c.ObjectId == parameter.CustomerId).ToList();
                #endregion

                #region Lấy list ghi chú

                var listNote = new List<NoteEntityModel>();

                listNote = note
                    .Where(x => x.ObjectId == parameter.CustomerId && x.ObjectType == "CUS" && x.Active == true).Select(
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
                        var _user = listUser.FirstOrDefault(x => x.UserId == item.CreatedById);
                        var _employee = listAllEmploye.FirstOrDefault(x => x.EmployeeId == _user.EmployeeId);
                        item.ResponsibleName = _employee.EmployeeName;
                        item.NoteDocList = listNoteDocument.Where(x => x.NoteId == item.NoteId).ToList();
                    });

                    //Sắp xếp lại listNote
                    listNote = listNote.OrderByDescending(x => x.CreatedDate).ToList();
                }

                #endregion

                var cusEntityModel = new CustomerEntityModel()
                {
                    CustomerId = customer.CustomerId,
                    CustomerName = customer.CustomerName,
                    CustomerCareStaff = customer.CustomerCareStaff,
                    CustomerCode = customer.CustomerCode,
                    CustomerServiceLevelId = customer.CustomerServiceLevelId,
                    CustomerType = customer.CustomerType,
                    CustomerGroupId = customer.CustomerGroupId,
                    StatusId = customer.StatusId,
                    PersonInChargeId = customer.PersonInChargeId,
                    Active = customer.Active,
                    CreatedById = customer.CreatedById,
                    CreatedDate = customer.CreatedDate,
                    MaximumDebtDays = customer.MaximumDebtDays,
                    MaximumDebtValue = customer.MaximumDebtValue,
                    MainBusinessSector = customer.MainBusinessSector,
                    PaymentId = customer.PaymentId,
                    FieldId = customer.FieldId,
                    ScaleId = customer.ScaleId,
                    BusinessRegistrationDate = customer.BusinessRegistrationDate,
                    EnterpriseType = customer.EnterpriseType,
                    BusinessScale = customer.BusinessScale,
                    BusinessType = customer.BusinessType,
                    TotalEmployeeParticipateSocialInsurance = customer.TotalEmployeeParticipateSocialInsurance,
                    TotalCapital = customer.TotalCapital,
                    TotalRevenueLastYear = customer.TotalRevenueLastYear,
                    NearestDateTransaction = customer.NearestDateTransaction,
                    TotalReceivable = customer.TotalReceivable == null ? 0 : customer.TotalReceivable,
                    TotalSaleValue = customer.TotalSaleValue == null ? 0 : customer.TotalSaleValue,
                    IsGraduated = customer.IsGraduated,
                    IsApproval = customer.IsApproval,
                    ApprovalStep = customer.ApprovalStep,
                    Point = customer.Point ?? 0,
                    PayPoint = customer.PayPoint ?? 0,
                    StatusCareId = customer.StatusCareId,
                    KhachDuAn = customer.KhachDuAn,
                    SubjectsApplication = customer.SubjectsApplication,
                    StaffChargeIds = customer.StaffChargeIds
                };

                var contact = contacts.FirstOrDefault(c => c.ObjectId == customer.CustomerId && c.ObjectType == ObjectType.CUSTOMER);

                var conEntityModel = new ContactEntityModel()
                {
                    ContactId = contact.ContactId,
                    ObjectId = contact.ObjectId,
                    ObjectType = contact.ObjectType,
                    FirstName = contact.FirstName,
                    LastName = contact.LastName,
                    Active = contact.Active,
                    Address = contact.Address,
                    AvatarUrl = string.IsNullOrEmpty(contact.AvatarUrl) ? "" : contact.AvatarUrl,
                    CountryId = contact.CountryId,
                    AreaId = contact.GeographicalAreaId,
                    ProvinceId = contact.ProvinceId,
                    DistrictId = contact.DistrictId,
                    WardId = contact.WardId,
                    TaxCode = contact.TaxCode,
                    DateOfBirth = contact.DateOfBirth != null ? contact.DateOfBirth.Value.Date : contact.DateOfBirth,
                    IdentityId = contact.IdentityId,
                    MaritalStatusId = contact.MaritalStatusId,
                    Email = contact.Email,
                    WorkEmail = contact.WorkEmail,
                    OtherEmail = contact.OtherEmail,
                    Phone = contact.Phone,
                    WorkPhone = contact.WorkPhone,
                    OtherPhone = contact.OtherPhone,
                    Gender = contact.Gender,
                    Birthplace = contact.Birthplace,
                    Job = contact.Job,
                    Agency = contact.Agency,
                    CompanyAddress = contact.CompanyAddress,
                    CompanyName = contact.CompanyName,
                    CustomerPosition = contact.CustomerPosition,
                    CreatedById = contact.CreatedById,
                    CreatedDate = contact.CreatedDate,
                    UpdatedById = contact.UpdatedById,
                    UpdatedDate = contact.UpdatedDate,
                    Note = contact.Note,
                    Role = contact.Role,
                    SocialUrl = contact.SocialUrl,
                    PostCode = contact.PostCode,
                    WebsiteUrl = contact.WebsiteUrl,
                    Other = contact.Other,
                    Longitude = contact.Longitude,
                    Latitude = contact.Latitude
                };


                #region Lấy danh sách khu vực, tỉnh, quận, phường xã nếu có
                //Tỉnh
                var listProvince = new List<ProvinceEntityModel>();
                listProvince = context.Province.Select(x => new ProvinceEntityModel
                {
                    ProvinceId = x.ProvinceId,
                    ProvinceName = x.ProvinceName
                }).OrderBy(z => z.ProvinceName).ToList();
                #endregion

                #region Get List Customer Code
                var ListCustomerCode = new List<string>();
                var listCustomerCodeEntity = context.Customer.Where(w => w.Active == true && w.CustomerId != parameter.CustomerId).Select(w => new
                {
                    CustomerCode = w.CustomerCode
                }).Distinct().ToList();
                listCustomerCodeEntity.ForEach(code =>
                {
                    ListCustomerCode.Add(code.CustomerCode);
                });
                #endregion

                var listEmployeeEntityModel = context.Employee
                                    .Select(x => new EmployeeEntityModel
                                    {
                                        EmployeeId = x.EmployeeId,
                                        EmployeeName = x.EmployeeName
                                    }).ToList();

                var listCategoryByCustomerGroup = (from c in context.Category
                                                    join ct in context.CategoryType
                                                    on c.CategoryTypeId equals ct.CategoryTypeId into cct
                                                    from cctJoined in cct.DefaultIfEmpty()
                                                    where cctJoined.CategoryTypeCode == "NHA"
                                                    select new CategoryEntityModel
                                                    {
                                                        CategoryName = c.CategoryName,
                                                        CategoryId = c.CategoryId
                                                    }).ToList();

                return new GetCustomerByIdResult()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = "Success",
                    ListProvince = listProvince,
                    ListNote = listNote,
                    CustomerCode = ListCustomerCode,
                    Contact = conEntityModel,
                    Customer = cusEntityModel,
                    ListEmployeeEntityModel = listEmployeeEntityModel,
                    ListCategoryByCustomerGroup = listCategoryByCustomerGroup
                };
            }
            catch (Exception e)
            {
                return new GetCustomerByIdResult()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        private DateTime FirstDateOfWeek(DateTime dateNow)
        {
            var firstDate = dateNow.AddDays(1);
            var _day = firstDate.Day;
            var _month = firstDate.Month;
            var _year = firstDate.Year;
            firstDate = new DateTime(_year, _month, _day, 0, 0, 0, 0);
            return firstDate;
        }

        private DateTime LastDateOfWeek(DateTime dateNow)
        {
            DateTime dateReturn = dateNow;
            var dayNow = dateNow.DayOfWeek;
            switch (dayNow)
            {
                case DayOfWeek.Monday:
                    dateReturn = dateNow.AddDays(6);
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

            var _day = dateReturn.Day;
            var _month = dateReturn.Month;
            var _year = dateReturn.Year;
            dateReturn = new DateTime(_year, _month, _day, 23, 59, 59, 999);
            return dateReturn;
        }

        private int checkLastDateOfMonth(DateTime endDateWeek4, DateTime last_date_month)
        {
            //Kiểm tra xem ngày cuối của tuần thứ 4 có phải ngày cuối cùng của tháng hay không?
            if (endDateWeek4.Day != last_date_month.Day)
            {
                //Nếu không phải thì có tuần thứ 5
                return 2;
            }
            else
            {
                //Nếu là ngày cuối cùng của tháng
                return 1;
            }
        }

        public EditCustomerByIdResult EditCustomerById(EditCustomerByIdParameter parameter)
        {
            try
            {
                if (parameter.Customer?.MaximumDebtDays < 0 || parameter.Customer?.MaximumDebtValue < 0 || parameter.Customer?.TotalCapital < 0 || parameter.Customer?.TotalEmployeeParticipateSocialInsurance < 0 || parameter.Customer?.TotalRevenueLastYear < 0)
                {
                    return new EditCustomerByIdResult()
                    {
                        StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                        MessageCode = CommonMessage.Customer.EDIT_FAIL
                    };
                }

                var customer = context.Customer.FirstOrDefault(c => c.CustomerId == parameter.Customer.CustomerId);
                var customerCodeOther = context.Customer.FirstOrDefault(cus => cus.CustomerCode == parameter.Customer.CustomerCode && cus.CustomerId != customer.CustomerId);
                if (customerCodeOther != null)
                {
                    return new EditCustomerByIdResult()
                    {
                        StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                        MessageCode = "Mã khách hàng đã được sử dụng"
                    };
                }
                var contact = context.Contact.FirstOrDefault(c => c.ContactId == parameter.Contact.ContactId && c.ObjectType == ObjectType.CUSTOMER);
                customer.CustomerName = parameter.Customer.CustomerName;
                customer.CustomerCode = parameter.Customer.CustomerCode;

                customer.CustomerGroupId = parameter.Customer.CustomerGroupId;
                customer.StatusId = parameter.Customer.StatusId != Guid.Empty ? parameter.Customer.StatusId : customer.StatusId;
                customer.CustomerCareStaff = parameter.Customer.CustomerCareStaff;
                customer.PersonInChargeId = parameter.Customer.PersonInChargeId;
                customer.PaymentId = parameter.Customer.PaymentId;
                customer.FieldId = parameter.Customer.FieldId;
                customer.MaximumDebtDays = parameter.Customer.MaximumDebtDays;
                customer.MaximumDebtValue = parameter.Customer.MaximumDebtValue;
                customer.BusinessRegistrationDate = parameter.Customer.BusinessRegistrationDate;
                customer.EnterpriseType = parameter.Customer.EnterpriseType;
                customer.BusinessScale = parameter.Customer.BusinessScale;
                customer.BusinessType = parameter.Customer.BusinessType;
                customer.TotalEmployeeParticipateSocialInsurance = parameter.Customer.TotalEmployeeParticipateSocialInsurance;
                customer.TotalCapital = parameter.Customer.TotalCapital;
                customer.TotalRevenueLastYear = parameter.Customer.TotalRevenueLastYear;
                customer.ScaleId = parameter.Customer.ScaleId;
                customer.IsGraduated = parameter.Customer.IsGraduated;
                customer.UpdatedById = parameter.UserId;
                customer.UpdatedDate = DateTime.Now;
                customer.MainBusinessSector = parameter.Customer.MainBusinessSector;


                contact.Gender = parameter.Contact.Gender;
                contact.CountryId = parameter.Contact.CountryId;
                contact.TaxCode = parameter.Contact.TaxCode;
                contact.DateOfBirth = parameter.Contact.DateOfBirth;
                contact.IdentityId = parameter.Contact.IdentityId;
                contact.Birthplace = parameter.Contact.Birthplace;
                contact.MaritalStatusId = parameter.Contact.MaritalStatusId;
                contact.Job = parameter.Contact.Job;
                contact.Agency = parameter.Contact.Agency;
                contact.Phone = parameter.Contact.Phone;
                contact.OtherPhone = parameter.Contact.OtherPhone;
                contact.WorkPhone = parameter.Contact.WorkPhone;
                contact.Email = parameter.Contact.Email;
                contact.OtherEmail = parameter.Contact.OtherEmail;
                contact.WorkEmail = parameter.Contact.WorkEmail;
                contact.ProvinceId = parameter.Contact.ProvinceId;
                contact.DistrictId = parameter.Contact.DistrictId;
                contact.WardId = parameter.Contact.WardId;
                contact.Address = parameter.Contact.Address;
                contact.WebsiteUrl = parameter.Contact.WebsiteUrl;
                contact.UpdatedById = parameter.UserId;
                contact.UpdatedDate = DateTime.Now;
                contact.AvatarUrl = parameter.Contact.AvatarUrl;
                contact.Other = parameter.Contact.Other;
                contact.CompanyName = parameter.Contact.CompanyName;
                contact.CompanyAddress = parameter.Contact.CompanyAddress;
                contact.CustomerPosition = parameter.Contact.CustomerPosition;

                context.Customer.Update(customer);
                context.Contact.Update(contact);

                context.SaveChanges();
                return new EditCustomerByIdResult()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = CommonMessage.Customer.EDIT_SUCCESS
                };
            }
            catch (Exception e)
            {
                return new EditCustomerByIdResult()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetAllCustomerResult GetAllCustomer(GetAllCustomerParameter parameter)
        {
            try
            {
                var customerList = context.Customer.Where(x => x.Active == true)
                .OrderBy(z => z.CustomerName).Select(y => new CustomerEntityModel
                {
                    CustomerId = y.CustomerId,
                    CustomerCode = y.CustomerCode,
                    CustomerName = y.CustomerName,
                    CustomerCodeName = y.CustomerCode + " - " + y.CustomerName
                }).ToList();

                return new GetAllCustomerResult
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = "success",
                    CustomerList = customerList
                };
            }
            catch (Exception e)
            {
                return new GetAllCustomerResult()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        private string OrderByCustomerName(string CustomerName)
        {
            string customerName = CustomerName.Trim();

            if (customerName.LastIndexOf(" ") == -1)
            {
                return customerName;
            }

            customerName = customerName.Substring(customerName.LastIndexOf(" ") + 1);

            return customerName;
        }

        public QuickCreateCustomerResult QuickCreateCustomer(QuickCreateCustomerParameter parameter)
        {
            try
            {
                if (parameter.CustomerCode == null)
                {
                    return new QuickCreateCustomerResult
                    {
                        StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                        MessageCode = "Mã khách hàng không được để trống"
                    };
                }
                else if (parameter.CustomerCode.Trim() == "")
                {
                    return new QuickCreateCustomerResult
                    {
                        StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                        MessageCode = "Mã khách hàng không được để trống"
                    };
                }
                var dublicateCustomer = context.Customer.FirstOrDefault(x => x.CustomerCode == parameter.CustomerCode);
                if (dublicateCustomer != null)
                {
                    return new QuickCreateCustomerResult
                    {
                        StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                        MessageCode = "Mã khách hàng đã tồn tại trên hệ thống"
                    };
                }
                var newId = Guid.NewGuid();
                var cusSttId = context.CategoryType.FirstOrDefault(ct => ct.CategoryTypeCode == "THA").CategoryTypeId;
                var newCusId = context.Category.FirstOrDefault(c => c.CategoryTypeId == cusSttId && c.CategoryCode == "MOI").CategoryId;

                //// Ngoc add check phê duyệt khách hàng định danh
                //var approvalCus = context.SystemParameter.FirstOrDefault(sp => sp.SystemKey == "AppovalCustomer");
                //if (approvalCus != null)
                //{
                //    if (approvalCus.SystemValue == false)
                //    {
                //        newCusId = context.Category.FirstOrDefault(c => c.CategoryTypeId == cusSttId && c.CategoryCode == "HDO").CategoryId;
                //    }
                //}

                Customer cu = new Customer()
                {
                    CustomerId = newId,
                    CustomerCode = parameter.CustomerCode,
                    CustomerName = parameter.CustomerName,
                    CustomerGroupId = parameter.CustomerGroupId,
                    PaymentId = parameter.PaymentId,
                    CustomerType = parameter.CustomerType,
                    MaximumDebtDays = parameter.MaximumDebtDays,
                    MaximumDebtValue = parameter.MaximumDebtValue,
                    StatusId = newCusId,
                    Active = true,
                    CreatedById = parameter.UserId,
                    CreatedDate = DateTime.Now
                };
                Contact con = new Contact()
                {
                    ContactId = Guid.NewGuid(),
                    FirstName = parameter.CustomerName,
                    ObjectId = newId,
                    ObjectType = ObjectType.CUSTOMER,
                    Active = true,
                    CreatedById = parameter.UserId,
                    CreatedDate = DateTime.Now
                };

                context.Customer.Add(cu);
                context.Contact.Add(con);
                context.SaveChanges();

                return new QuickCreateCustomerResult
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    CustomerID = cu.CustomerId
                };
            }
            catch (Exception e)
            {
                return new QuickCreateCustomerResult()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public ImportCustomerResult ImportCustomer(ImportCustomerParameter parameter)
        {
            using (var dbcxtransaction = context.Database.BeginTransaction())
            {
                try
                {
                    List<Customer> lstCustomer = new List<Customer>();
                    List<Contact> lstcontact = new List<Contact>();
                    bool checkIsDublicate = false;
                    var listDupblicateCustomerString = "";
                    List<CustomerEntityModel> lstcontactCustomerDuplicate = new List<CustomerEntityModel>();  //Danh sách customer bị trùng trên server
                    List<ContactEntityModel> lstcontactContactDuplicate = new List<ContactEntityModel>(); //Danh sách contact của các customer bị trùng trên server
                    List<ContactEntityModel> lstcontactContact_CON_Duplicate = new List<ContactEntityModel>();    //Danh sách người liên hệ của các customer bị trùng trên server

                    List<string> dulicateTaxcode = new List<string>();
                    List<string> dulicateEmail = new List<string>();
                    List<string> dulicatePhone = new List<string>();

                    #region comment by Giang
                    //int countDulicateEmail = 1;
                    //int countDulicatePhone = 1;
                    #endregion

                    if (parameter.FileList != null && parameter.FileList.Count > 0)
                    {
                        using (var stream = new MemoryStream())
                        {
                            parameter.FileList[0].CopyTo(stream);
                            using (ExcelPackage package = new ExcelPackage(stream))
                            {


                                var listProvince = context.Province.ToList();
                                var listDistrict = context.District.ToList();
                                var listWard = context.Ward.ToList();
                                //Loai doanh nghiep 
                                var listBusinessType = (from categoryT in context.CategoryType
                                                        join category in context.Category on categoryT.CategoryTypeId equals category.CategoryTypeId
                                                        where categoryT.CategoryTypeCode == "LNG"
                                                        select category).ToList();
                                //lĩnh vực SX/KD
                                var listCategoryTypeField = (from categoryT in context.CategoryType
                                                             join category in context.Category on categoryT.CategoryTypeId equals category.CategoryTypeId
                                                             where categoryT.CategoryTypeCode == "LDO"
                                                             select category).ToList();
                                //loại hình doanh nghiệp EnterpriseType
                                var listEnterpriseType = (from categoryT in context.CategoryType
                                                          join category in context.Category on categoryT.CategoryTypeId equals category.CategoryTypeId
                                                          where categoryT.CategoryTypeCode == "LHI"
                                                          select category).ToList();
                                //ngành nghề kinh doanh chính
                                var listMainBusinessSector = (from categoryT in context.CategoryType
                                                              join category in context.Category on categoryT.CategoryTypeId equals category.CategoryTypeId
                                                              where categoryT.CategoryTypeCode == "NCH"
                                                              select category).ToList();

                                //Quy mo
                                var listBusinessScale = (from categoryT in context.CategoryType
                                                         join category in context.Category on categoryT.CategoryTypeId equals category.CategoryTypeId
                                                         where categoryT.CategoryTypeCode == "QNG"
                                                         select category).ToList();
                                //Group ID
                                var listCustomerGroup = (from categoryT in context.CategoryType
                                                         join category in context.Category on categoryT.CategoryTypeId equals category.CategoryTypeId
                                                         where categoryT.CategoryTypeCode == "NHA"
                                                         select category).ToList();
                                //Status Customer
                                var listCustomerStatus = (from categoryT in context.CategoryType
                                                          join category in context.Category on categoryT.CategoryTypeId equals category.CategoryTypeId
                                                          where categoryT.CategoryTypeCode == "THA"
                                                          select category).ToList();
                                //Customer Postion
                                var listCustomerPostion = (from categoryT in context.CategoryType
                                                           join category in context.Category on categoryT.CategoryTypeId equals category.CategoryTypeId
                                                           where categoryT.CategoryTypeCode == "CVU"
                                                           select category).ToList();

                                if (parameter.CustomerType == 1)
                                {
                                    ExcelWorksheet worksheet = package.Workbook.Worksheets["DN"];

                                    if (worksheet == null)
                                    {
                                        return new ImportCustomerResult
                                        {
                                            MessageCode = "File excel không đúng theo template",
                                            StatusCode = System.Net.HttpStatusCode.ExpectationFailed
                                        };
                                    }
                                    //Group cells by row
                                    var rowcellgroups = worksheet.Cells["A:Y"].GroupBy(c => c.Start.Row);
                                    //Loại bỏ 1 dòng tiêu đề
                                    var groups = rowcellgroups.Skip(1);

                                    //Group theo từng ngày
                                    var cv = (from item in groups
                                              group item by new
                                              {
                                                  item.First().Value,
                                              } into gcs
                                              select gcs).ToList();

                                    //var cv2 = (from item in cv
                                    //           group item by new
                                    //          {
                                    //              Email =item.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "K")).DefaultIfEmpty(null).FirstOrDefault().Value
                                    //          } into gcs
                                    //          select gcs).ToList();

                                    if (cv.Count == 0)
                                    {
                                        return new ImportCustomerResult
                                        {
                                            MessageCode = "File excel không tồn tại bản ghi nào!",
                                            StatusCode = System.Net.HttpStatusCode.ExpectationFailed
                                        };
                                    }

                                    bool checkValidSTT = false;
                                    bool checkIsNull = false;
                                    cv.ForEach(g =>
                                    {
                                        if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "A")).First() == null
                                        || g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "C")).First() == null
                                        || g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "L")).First() == null
                                        || g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "Q")).First() == null)
                                        {
                                            checkIsNull = true;
                                        }
                                        else
                                        {
                                            if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "A")).First().Value == null
                                           || g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "C")).First().Value == null
                                           || g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "L")).First().Value == null
                                           || g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "Q")).First().Value == null
                                           || g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "A")).First().Text.Replace(" ", "") == ""
                                           || g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "C")).First().Text.Replace(" ", "") == ""
                                           || g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "L")).First().Text.Replace(" ", "") == ""
                                           || g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "Q")).First().Text.Replace(" ", "") == ""
                                           )
                                            {
                                                checkIsNull = true;
                                            }
                                            else
                                            {
                                                int result;
                                                bool parsedSuccessfully = int.TryParse(g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "A")).First().Text.Trim(), out result);
                                                if (parsedSuccessfully == false)
                                                {
                                                    checkValidSTT = true;
                                                }
                                            }
                                        }
                                    });

                                    if (checkIsNull)
                                    {
                                        return new ImportCustomerResult
                                        {
                                            MessageCode = "Import không thành công, một số trường bắt buộc chưa nhập",
                                            StatusCode = System.Net.HttpStatusCode.ExpectationFailed
                                        };
                                    }
                                    if (checkValidSTT)
                                    {
                                        return new ImportCustomerResult
                                        {
                                            MessageCode = "Import không thành công, trường STT phải nhập số",
                                            StatusCode = System.Net.HttpStatusCode.ExpectationFailed
                                        };
                                    }

                                    cv.ForEach(g =>
                                    {
                                        if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "B")).First() != null)
                                        {
                                            dulicateTaxcode.Add(g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "B")).First().Text.Trim());
                                        }

                                        if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "K")).First() != null)
                                        {
                                            dulicateEmail.Add(g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "K")).First().Text.Trim());
                                        }

                                        if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "L")).First() != null)
                                        {
                                            dulicatePhone.Add(g.Select(o => o.FirstOrDefault(rc => rc.Start.Address.Substring(0, 1) == "L")).First().Text.Trim());
                                        }
                                    });

                                    #region Kiểm tra trùng taxcode, email và sđt trong file
                                    List<string> lstDupblicateTaxCode = new List<string>();
                                    List<string> lstDublicateEmail = new List<string>();
                                    List<string> lstDublicatePhone = new List<string>();
                                    List<string> stt = new List<string>();

                                    //Danh sách taxcode != ""
                                    var lstTaxcodeNotNullOrEmpty = dulicateTaxcode.Where(x => x.Trim() != "").ToList();
                                    //Danh sách taxcode không có giá trị bị trùng
                                    var lstDistinctTaxcode = lstTaxcodeNotNullOrEmpty.Distinct().ToList();
                                    //Danh sách taxcode bị trùng
                                    lstDupblicateTaxCode = lstTaxcodeNotNullOrEmpty.GroupBy(x => x)
                                                          .Where(g => g.Count() > 1)
                                                          .Select(y => y.Key)
                                                          .ToList();

                                    //Lọc các giá trị email != ""
                                    dulicateEmail = dulicateEmail.Where(x => x.Trim() != "").ToList();
                                    //Danh sách email không có giá trị bị trùng
                                    var lstDistinctEmail = dulicateEmail.Distinct().ToList();
                                    //Danh sách email bị trùng
                                    lstDublicateEmail = dulicateEmail.GroupBy(x => x)
                                                          .Where(g => g.Count() > 1)
                                                          .Select(y => y.Key)
                                                          .ToList();

                                    //Danh sách số điện thoại không có giá trị bị trùng
                                    var lstDistinctPhone = dulicatePhone.Distinct().ToList();
                                    //Danh sách số điện thoại bị trùng
                                    lstDublicatePhone = dulicatePhone.GroupBy(x => x)
                                                          .Where(g => g.Count() > 1)
                                                          .Select(y => y.Key)
                                                          .ToList();

                                    if (lstDupblicateTaxCode.Count > 0 || lstDublicateEmail.Count > 0 || lstDublicatePhone.Count > 0)
                                    {
                                        checkIsDublicate = true;

                                        //Lấy stt của row bị trùng
                                        if (lstDupblicateTaxCode.Count > 0)
                                        {
                                            cv.ForEach(item =>
                                            {
                                                if (item.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "B")).First() != null)
                                                {
                                                    var currentTaxcode = item.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "B")).First().Text.Trim();
                                                    if (lstDupblicateTaxCode.FirstOrDefault(x => x == currentTaxcode) != null)
                                                    {
                                                        stt.Add(item.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "A")).First().Text.Trim());
                                                    }
                                                }
                                            });
                                        }

                                        if (lstDublicateEmail.Count > 0)
                                        {
                                            cv.ForEach(item =>
                                            {
                                                if (item.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "K")).First() != null)
                                                {
                                                    var currentEmail = item.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "K")).First().Text.Trim();
                                                    if (lstDublicateEmail.FirstOrDefault(x => x == currentEmail) != null)
                                                    {
                                                        stt.Add(item.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "A")).First().Text.Trim());
                                                    }
                                                }
                                            });
                                        }

                                        if (lstDublicatePhone.Count > 0)
                                        {
                                            cv.ForEach(item =>
                                            {
                                                if (item.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "L")).First() != null)
                                                {
                                                    var currentPhone = item.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "L")).First().Text.Trim();
                                                    if (lstDublicatePhone.FirstOrDefault(x => x == currentPhone) != null)
                                                    {
                                                        stt.Add(item.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "A")).First().Text.Trim());
                                                    }
                                                }
                                            });
                                        }
                                    }

                                    stt = stt.Distinct().ToList();
                                    #endregion

                                    int currentYear = DateTime.Now.Year % 100;
                                    int currentMonth = DateTime.Now.Month;
                                    int currentDate = DateTime.Now.Day;
                                    int MaxNumberCode = 0;

                                    cv.ForEach(g =>
                                    {
                                        if (g.Key.Value == null) return;
                                        string Email = string.Empty;

                                        #region comment by giang
                                        //countDulicateEmail = 0;
                                        //countDulicatePhone = 0;
                                        #endregion

                                        if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "K")).First() != null)
                                        {
                                            if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "K")).First().Value != null)
                                            {
                                                Email = g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "K")).First().Value.ToString().Trim();
                                            }
                                        }
                                        string Phone = string.Empty;
                                        if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "L")).First() != null)
                                        {
                                            if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "L")).First().Value != null)
                                            {
                                                Phone = g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "L")).First().Value.ToString().Trim();
                                            }
                                        }
                                        string TaxCode = string.Empty;
                                        if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "B")).First() != null)
                                        {
                                            if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "B")).First().Value != null)
                                            {
                                                TaxCode = g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "B")).First().Value.ToString().Trim();
                                            }
                                        }

                                        Contact contactEntityCheckEmailDuplicate = null;
                                        Contact contactEntityCheckPhoneDuplicate = null;
                                        Contact contactEntityCheckTaxCodeDuplicate = null;

                                        #region check dupblicase email, phone (comment by giang)
                                        //for (int i = 0; i < dulicateEmail.Count; i++)
                                        //{
                                        //    if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "K")).First().Value == dulicateEmail[i])
                                        //    {
                                        //        countDulicateEmail++;
                                        //    }
                                        //}
                                        //for (int i = 0; i < dulicatePhone.Count; i++)
                                        //{
                                        //    if (g.Select(o => o.FirstOrDefault(rc => rc.Start.Address.Substring(0, 1) == "L")).First().Value.ToString().Trim() == dulicatePhone[i])
                                        //    {
                                        //        countDulicatePhone++;
                                        //    }
                                        //}
                                        #endregion

                                        #region Kiểm tra email, phone, taxcode đã tồn tại trên server chưa
                                        if (!string.IsNullOrEmpty(Email))
                                        {
                                            contactEntityCheckEmailDuplicate = context.Contact.Where(w => w.Email.ToLower() == Email.Trim().ToLower() && w.Active == true).FirstOrDefault();
                                        }

                                        if (!string.IsNullOrEmpty(Phone))
                                        {
                                            contactEntityCheckPhoneDuplicate = context.Contact.Where(w => w.Phone.ToLower() == Phone.Trim().ToLower() && w.Active == true).FirstOrDefault();
                                        }

                                        if (!string.IsNullOrEmpty(TaxCode))
                                        {
                                            contactEntityCheckTaxCodeDuplicate = context.Contact.Where(w => w.TaxCode.ToLower() == TaxCode.Trim().ToLower() && w.Active == true).FirstOrDefault();
                                        }
                                        #endregion

                                        if (contactEntityCheckTaxCodeDuplicate == null && contactEntityCheckEmailDuplicate == null && contactEntityCheckPhoneDuplicate == null)
                                        {
                                            //Nếu email, phone, taxcode đều chưa tồn tại trên server thì:

                                            //var BusinessType = g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "I")).First();
                                            Customer CreateNewCustomer = new Customer();

                                            //Nếu thuộc các trường hợp bị trùng taxcode, email, số điện thoại thì không Add
                                            if (stt.FirstOrDefault(x => x == g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "A")).First().Text.Trim()) == null)
                                            {
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "Q")).First() != null)
                                                {
                                                    if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "Q")).First().Value != null)
                                                    {
                                                        string TextExcel = g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "Q")).First().Value.ToString();
                                                        var objectCustomerGroup = listCustomerGroup.Where(w => w.CategoryCode == TextExcel).FirstOrDefault();
                                                        CreateNewCustomer.CustomerGroupId = objectCustomerGroup != null ? objectCustomerGroup.CategoryId : Guid.Empty; //listCustomerGroup.Where(w => w.CategoryCode == "DN").Select(d => (Guid?)d.CategoryId).DefaultIfEmpty(null).FirstOrDefault().Value;
                                                    }
                                                }

                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "B")).First() != null)
                                                {
                                                    if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "B")).First().Value != null)
                                                    {
                                                        CreateNewCustomer.CustomerCode = g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "B")).First().Value.ToString().Trim();
                                                    }
                                                }

                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "C")).First() != null)
                                                {
                                                    if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "C")).First().Value != null)
                                                    {
                                                        CreateNewCustomer.CustomerName = g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "C")).First().Value.ToString().Trim();
                                                    }
                                                }

                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "R")).First() != null)
                                                {
                                                    if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "R")).First().Value != null)
                                                    {
                                                        CreateNewCustomer.TotalCapital = Convert.ToDecimal(g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "R")).First().Value.ToString());
                                                    }
                                                }

                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "S")).First() != null)
                                                {
                                                    if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "S")).First().Value != null)
                                                    {
                                                        CreateNewCustomer.BusinessRegistrationDate = Convert.ToDateTime(DateTime.ParseExact(g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "S")).First().Text.ToString().Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture));
                                                    }
                                                }

                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "H")).First() != null)
                                                {
                                                    if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "H")).First().Value != null)
                                                    {
                                                        CreateNewCustomer.EnterpriseType = (listEnterpriseType.Where(x => x.CategoryCode == g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "H")).First().Value.ToString()).Select(d => (Guid?)d.CategoryId).DefaultIfEmpty(null).FirstOrDefault());
                                                    }
                                                }

                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "T")).First() != null)
                                                {
                                                    if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "T")).First().Value != null)
                                                    {
                                                        CreateNewCustomer.TotalEmployeeParticipateSocialInsurance = int.Parse(g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "T")).First().Value.ToString());
                                                    }
                                                }

                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "U")).First() != null)
                                                {
                                                    if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "U")).First().Value != null)
                                                    {
                                                        CreateNewCustomer.BusinessType = (listBusinessType.Where(x => x.CategoryCode == g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "U")).First().Value.ToString().Trim()).Select(d => (Guid?)d.CategoryId).DefaultIfEmpty(null).FirstOrDefault());
                                                    }
                                                }

                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "I")).First() != null)
                                                {
                                                    if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "I")).First().Value != null)
                                                    {
                                                        CreateNewCustomer.FieldId = (listCategoryTypeField.Where(x => x.CategoryCode == g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "I")).First().Value.ToString()).Select(d => (Guid?)d.CategoryId).DefaultIfEmpty(null).FirstOrDefault());
                                                    }
                                                }

                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "J")).First() != null)
                                                {
                                                    if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "J")).First().Value != null)
                                                    {
                                                        CreateNewCustomer.MainBusinessSector = (listMainBusinessSector.Where(x => x.CategoryCode == g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "J")).First().Value.ToString()).Select(d => (Guid?)d.CategoryId).DefaultIfEmpty(null).FirstOrDefault());
                                                    }
                                                }

                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "W")).First() != null)
                                                {
                                                    if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "W")).First().Value != null)
                                                    {
                                                        CreateNewCustomer.TotalRevenueLastYear = Convert.ToDecimal(g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "W")).First().Value.ToString());
                                                    }
                                                }

                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "X")).First() != null)
                                                {
                                                    if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "X")).First().Value != null)
                                                    {
                                                        CreateNewCustomer.BusinessScale = (listBusinessScale.Where(x => x.CategoryCode == g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "X")).First().Value.ToString()).Select(d => (Guid?)d.CategoryId).DefaultIfEmpty(null).FirstOrDefault());
                                                    }
                                                }

                                                CreateNewCustomer.CustomerId = Guid.NewGuid();
                                                CreateNewCustomer.CustomerType = 1;
                                                CreateNewCustomer.StatusId = listCustomerStatus.Where(w => w.CategoryCode == "MOI").Select(d => (Guid?)d.CategoryId).DefaultIfEmpty(null).FirstOrDefault().Value;
                                                CreateNewCustomer.CreatedById = parameter.UserId;
                                                CreateNewCustomer.CreatedDate = DateTime.Now;

                                                //thêm khách hàng
                                                if (CreateNewCustomer.CustomerCode == null)
                                                {
                                                    MaxNumberCode++;
                                                    CreateNewCustomer.CustomerCode = string.Format("CTM{0}{1}{2}{3}", currentYear, currentMonth, currentDate, (MaxNumberCode).ToString("D4"));
                                                }

                                                lstCustomer.Add(CreateNewCustomer);

                                                #region comment by giang
                                                //if (countDulicatePhone <= 1 && countDulicateEmail <= 1)
                                                //{
                                                //    if (CreateNewCustomer.CustomerCode == null)
                                                //    {
                                                //        MaxNumberCode++;
                                                //        CreateNewCustomer.CustomerCode = string.Format("CTM{0}{1}{2}{3}", currentYear, currentMonth, currentDate, (MaxNumberCode).ToString("D4"));
                                                //    }
                                                //    context.Customer.Add(CreateNewCustomer);
                                                //    context.SaveChanges();
                                                //}
                                                #endregion

                                                Contact CreateContact = new Contact();
                                                CreateContact.ObjectId = CreateNewCustomer.CustomerId;
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "B")).First() != null)
                                                {
                                                    if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "B")).First().Value != null)
                                                    {
                                                        CreateContact.TaxCode = g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "B")).First().Value.ToString().Trim();
                                                    }
                                                }

                                                CreateContact.ObjectType = "CUS";
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "E")).First() != null)
                                                {
                                                    if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "E")).First().Value != null)
                                                    {
                                                        CreateContact.ProvinceId = (listProvince.Where(x => x.ProvinceCode == g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "E")).First().Value.ToString()).Select(d => (Guid?)d.ProvinceId).DefaultIfEmpty(null).FirstOrDefault());
                                                    }
                                                }

                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "F")).First() != null)
                                                {
                                                    if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "F")).First().Value != null)
                                                    {
                                                        CreateContact.DistrictId = (listDistrict.Where(x => x.DistrictCode == g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "F")).First().Value.ToString()).Select(d => (Guid?)d.DistrictId).DefaultIfEmpty(null).FirstOrDefault());
                                                    }
                                                }

                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "G")).First() != null)
                                                {
                                                    if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "G")).First().Value != null)
                                                    {
                                                        CreateContact.WardId = (listWard.Where(x => x.WardCode == g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "G")).First().Value.ToString()).Select(d => (Guid?)d.WardId).DefaultIfEmpty(null).FirstOrDefault());
                                                    }
                                                }

                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "D")).First() != null)
                                                {
                                                    if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "D")).First().Value != null)
                                                    {
                                                        CreateContact.Address = g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "D")).First().Value.ToString().Trim();
                                                    }
                                                }

                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "K")).First() != null)
                                                {
                                                    if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "K")).First().Value != null)
                                                    {
                                                        CreateContact.Email = g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "K")).First().Value.ToString().Trim();
                                                    }
                                                }

                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "L")).First() != null)
                                                {
                                                    if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "L")).First().Value != null)
                                                    {
                                                        CreateContact.Phone = g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "L")).First().Value.ToString().Trim();
                                                    }
                                                }

                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "Y")).First() != null)
                                                {
                                                    if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "Y")).First().Value != null)
                                                    {
                                                        CreateContact.Note = g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "Y")).First().Value.ToString();
                                                    }
                                                }

                                                CreateContact.CreatedById = parameter.UserId;
                                                CreateContact.CreatedDate = DateTime.Now;
                                                CreateContact.Phone = CreateContact.Phone == null ? null : CreateContact.Phone.Replace(" ", "");
                                                lstcontact.Add(CreateContact);

                                                //Tạo ra người liên hệ
                                                if ((g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "M")).First() == null
                                                && g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "N")).First() == null
                                                && g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "O")).First() == null
                                                && g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "P")).First() == null)
                                                || (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "M")).First().Value == null
                                                && g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "N")).First().Value == null
                                                && g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "O")).First().Value == null
                                                && g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "P")).First().Value == null)
                                                || (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "M")).First().Text.Replace(" ", "") == ""
                                                && g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "N")).First().Text.Replace(" ", "") == ""
                                                && g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "O")).First().Text.Replace(" ", "") == ""
                                                && g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "P")).First().Text.Replace(" ", "") == "")
                                                )
                                                {

                                                }
                                                else
                                                {
                                                    string FullNameContact_COn = string.Empty;
                                                    if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "M")).First() != null)
                                                    {
                                                        if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "M")).First().Value != null)
                                                        {
                                                            FullNameContact_COn = g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "M")).First().Value.ToString().Trim();
                                                        }
                                                    }

                                                    string[] array = FullNameContact_COn.Split(' ');
                                                    string FirstName = array[0];
                                                    string LastName = string.Empty;
                                                    for (int i = 1; i < array.Length; ++i)
                                                    {
                                                        LastName = LastName + " " + array[i];
                                                    };
                                                    Contact CreateContact_Con = new Contact();
                                                    CreateContact_Con.ObjectId = CreateNewCustomer.CustomerId;
                                                    CreateContact_Con.ObjectType = "CUS_CON";
                                                    CreateContact_Con.FirstName = FirstName;
                                                    CreateContact_Con.LastName = LastName;
                                                    if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "N")).First() != null)
                                                    {
                                                        if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "N")).First().Value != null)
                                                        {
                                                            CreateContact_Con.Role = g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "N")).First().Value.ToString().Trim();
                                                        }
                                                    }
                                                    if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "P")).First() != null)
                                                    {
                                                        if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "P")).First().Value != null)
                                                        {
                                                            CreateContact_Con.Email = g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "P")).First().Value.ToString().Trim();
                                                        }
                                                    }
                                                    if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "O")).First() != null)
                                                    {
                                                        if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "O")).First().Value != null)
                                                        {
                                                            CreateContact_Con.Phone = g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "O")).First().Value.ToString().Trim();
                                                        }
                                                    }

                                                    CreateContact_Con.CreatedById = parameter.UserId;
                                                    CreateContact_Con.CreatedDate = DateTime.Now;
                                                    CreateContact_Con.Phone = CreateContact_Con.Phone == null ? null : CreateContact_Con.Phone.Replace(" ", "");
                                                    lstcontact.Add(CreateContact_Con);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            //Nếu email, phone, taxcode có trường hợp đã tồn tại trên server thì:
                                            Contact contact;
                                            contact = contactEntityCheckEmailDuplicate != null ? contactEntityCheckEmailDuplicate : (contactEntityCheckPhoneDuplicate != null ? contactEntityCheckPhoneDuplicate : contactEntityCheckTaxCodeDuplicate);

                                            //khách hàng
                                            CustomerEntityModel CustomerDuplicate = new CustomerEntityModel();
                                            CustomerDuplicate.CustomerId = contact.ObjectId;
                                            //CustomerDuplicate.CustomerGroupId = listCustomerGroup.Where(w => w.CategoryCode == "DN").Select(d => (Guid?)d.CategoryId).DefaultIfEmpty(null).FirstOrDefault().Value;
                                            if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "Q")).First() != null)
                                            {
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "Q")).First().Value != null)
                                                {
                                                    string TextExcel = g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "Q")).First().Value.ToString();
                                                    var objectCustomerGroup = listCustomerGroup.Where(w => w.CategoryCode == TextExcel).FirstOrDefault();
                                                    CustomerDuplicate.CustomerGroupId = objectCustomerGroup != null ? objectCustomerGroup.CategoryId : Guid.Empty; //listCustomerGroup.Where(w => w.CategoryCode == "DN").Select(d => (Guid?)d.CategoryId).DefaultIfEmpty(null).FirstOrDefault().Value;
                                                }
                                            }

                                            if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "B")).First() != null)
                                            {
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "B")).First().Value != null)
                                                {
                                                    CustomerDuplicate.CustomerCode = g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "B")).First().Value.ToString().Trim();
                                                }
                                            }

                                            if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "C")).First() != null)
                                            {
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "C")).First().Value != null)
                                                {
                                                    CustomerDuplicate.CustomerName = g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "C")).First().Value.ToString().Trim();
                                                }
                                            }
                                            if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "R")).First() != null)
                                            {
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "R")).First().Value != null)
                                                {
                                                    CustomerDuplicate.TotalCapital = Convert.ToDecimal(g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "R")).First().Value.ToString());
                                                }
                                            }
                                            if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "S")).First() != null)
                                            {
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "S")).First().Value != null)
                                                {
                                                    CustomerDuplicate.BusinessRegistrationDate = Convert.ToDateTime(DateTime.ParseExact(g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "S")).First().Text.ToString().Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture));
                                                }
                                            }
                                            if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "H")).First() != null)
                                            {
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "H")).First().Value != null)
                                                {
                                                    CustomerDuplicate.EnterpriseType = (listEnterpriseType.Where(x => x.CategoryCode == g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "H")).First().Value.ToString()).Select(d => (Guid?)d.CategoryId).DefaultIfEmpty(null).FirstOrDefault());
                                                }
                                            }
                                            if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "T")).First() != null)
                                            {
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "T")).First().Value != null)
                                                {
                                                    CustomerDuplicate.TotalEmployeeParticipateSocialInsurance = int.Parse(g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "T")).First().Value.ToString());
                                                }
                                            }

                                            if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "U")).First() != null)
                                            {
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "U")).First().Value != null)
                                                {
                                                    CustomerDuplicate.BusinessType = (listBusinessType.Where(x => x.CategoryCode == g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "U")).First().Value.ToString()).Select(d => (Guid?)d.CategoryId).DefaultIfEmpty(null).FirstOrDefault());
                                                }
                                            }
                                            if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "I")).First() != null)
                                            {
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "I")).First().Value != null)
                                                {
                                                    CustomerDuplicate.FieldId = (listCategoryTypeField.Where(x => x.CategoryCode == g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "I")).First().Value.ToString()).Select(d => (Guid?)d.CategoryId).DefaultIfEmpty(null).FirstOrDefault());
                                                }
                                            }

                                            if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "J")).First() != null)
                                            {
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "J")).First().Value != null)
                                                {
                                                    CustomerDuplicate.MainBusinessSector = (listMainBusinessSector.Where(x => x.CategoryCode == g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "J")).First().Value.ToString()).Select(d => (Guid?)d.CategoryId).DefaultIfEmpty(null).FirstOrDefault());
                                                }
                                            }

                                            if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "W")).First() != null)
                                            {
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "W")).First().Value != null)
                                                {
                                                    CustomerDuplicate.TotalRevenueLastYear = Convert.ToDecimal(g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "W")).First().Value.ToString());
                                                }
                                            }
                                            if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "X")).First() != null)
                                            {
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "X")).First().Value != null)
                                                {
                                                    CustomerDuplicate.BusinessScale = (listBusinessScale.Where(x => x.CategoryCode == g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "X")).First().Value.ToString()).Select(d => (Guid?)d.CategoryId).DefaultIfEmpty(null).FirstOrDefault());
                                                }
                                            }

                                            lstcontactCustomerDuplicate.Add(CustomerDuplicate);

                                            //thông tin chi tiết của khách hàng
                                            ContactEntityModel ContactDuplicate = new ContactEntityModel();
                                            ContactDuplicate.ObjectId = contact.ObjectId;
                                            ContactDuplicate.ContactId = contact.ContactId;
                                            if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "B")).First() != null)
                                            {
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "B")).First().Value != null)
                                                {
                                                    ContactDuplicate.TaxCode = g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "B")).First().Value.ToString().Trim();
                                                }
                                            }
                                            ContactDuplicate.ObjectType = "CUS";
                                            if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "E")).First() != null)
                                            {
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "E")).First().Value != null)
                                                {
                                                    ContactDuplicate.ProvinceId = (listProvince.Where(x => x.ProvinceCode == g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "E")).First().Value.ToString()).Select(d => (Guid?)d.ProvinceId).DefaultIfEmpty(null).FirstOrDefault());
                                                }
                                            }
                                            if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "F")).First() != null)
                                            {
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "F")).First().Value != null)
                                                {
                                                    ContactDuplicate.DistrictId = (listDistrict.Where(x => x.DistrictCode == g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "F")).First().Value.ToString()).Select(d => (Guid?)d.DistrictId).DefaultIfEmpty(null).FirstOrDefault());
                                                }
                                            }

                                            if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "G")).First() != null)
                                            {
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "G")).First().Value != null)
                                                {
                                                    ContactDuplicate.WardId = (listWard.Where(x => x.WardCode == g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "G")).First().Value.ToString()).Select(d => (Guid?)d.WardId).DefaultIfEmpty(null).FirstOrDefault());
                                                }
                                            }

                                            if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "D")).First() != null)
                                            {
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "D")).First().Value != null)
                                                {
                                                    ContactDuplicate.Address = g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "D")).First().Value.ToString().Trim();
                                                }
                                            }
                                            if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "K")).First() != null)
                                            {
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "K")).First().Value != null)
                                                {
                                                    ContactDuplicate.Email = g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "K")).First().Value.ToString().Trim();
                                                }
                                            }
                                            if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "L")).First() != null)
                                            {
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "L")).First().Value != null)
                                                {
                                                    ContactDuplicate.Phone = g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "L")).First().Value.ToString().Trim();
                                                }
                                            }
                                            if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "Y")).First() != null)
                                            {
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "Y")).First().Value != null)
                                                {
                                                    ContactDuplicate.Note = g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "Y")).First().Value.ToString();
                                                }
                                            }
                                            lstcontactContactDuplicate.Add(ContactDuplicate);

                                            //Tạo ra người liên hệ
                                            string FullNameContact_COn = string.Empty;
                                            if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "M")).First() != null)
                                            {
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "M")).First().Value != null)
                                                {
                                                    FullNameContact_COn = g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "M")).First().Value.ToString().Trim();
                                                }
                                            }
                                            string[] array = FullNameContact_COn.Split(' ');
                                            string FirstName = array[0];
                                            string LastName = string.Empty;
                                            for (int i = 1; i < array.Length; ++i)
                                            {
                                                LastName = LastName + " " + array[i];
                                            };

                                            ContactEntityModel CreateContact_Con = new ContactEntityModel();
                                            CreateContact_Con.ObjectId = contact.ObjectId;
                                            CreateContact_Con.ObjectType = "CUS_CON";
                                            CreateContact_Con.FirstName = FirstName;
                                            CreateContact_Con.LastName = LastName;
                                            if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "N")).First() != null)
                                            {
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "N")).First().Value != null)
                                                {
                                                    CreateContact_Con.Role = g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "N")).First().Value.ToString();
                                                }
                                            }
                                            if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "P")).First() != null)
                                            {
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "P")).First().Value != null)
                                                {
                                                    CreateContact_Con.Email = g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "P")).First().Value.ToString().Trim();
                                                }
                                            }
                                            if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "O")).First() != null)
                                            {
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "O")).First().Value != null)
                                                {
                                                    CreateContact_Con.Phone = g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "O")).First().Value.ToString().Trim();
                                                }
                                            }
                                            CreateContact_Con.CreatedById = parameter.UserId;
                                            CreateContact_Con.CreatedDate = DateTime.Now;

                                            lstcontactContact_CON_Duplicate.Add(CreateContact_Con);
                                        }
                                    });

                                    #region comment by Giang
                                    //if (countDulicateEmail <= 1 && countDulicatePhone <= 1)
                                    //{
                                    //    context.Contact.AddRange(lstcontact);
                                    //    context.SaveChanges();
                                    //}
                                    #endregion

                                    if (checkIsDublicate)
                                    {
                                        listDupblicateCustomerString = "";
                                        stt.ForEach(item =>
                                        {
                                            listDupblicateCustomerString += item + ", ";
                                        });
                                    }

                                    context.Customer.AddRange(lstCustomer);
                                    context.Contact.AddRange(lstcontact);
                                    context.SaveChanges();
                                }
                                else if (parameter.CustomerType == 2)
                                {
                                    ExcelWorksheet worksheet = package.Workbook.Worksheets["CN"];

                                    if (worksheet == null)
                                    {
                                        return new ImportCustomerResult
                                        {
                                            MessageCode = "File excel không đúng theo template",
                                            StatusCode = System.Net.HttpStatusCode.ExpectationFailed
                                        };
                                    }
                                    //Group cells by row
                                    var rowcellgroups = worksheet.Cells["A:Q"].GroupBy(c => c.Start.Row);
                                    //Loại bỏ 2 dòng tiêu đề
                                    var groups = rowcellgroups.Skip(1);
                                    //Group theo từng ngày
                                    var cv = (from item in groups
                                              group item by new
                                              {
                                                  item.First().Value,
                                              } into gcs
                                              select gcs).ToList();
                                    if (cv.Count == 0)
                                    {
                                        return new ImportCustomerResult
                                        {
                                            MessageCode = "File excel không tồn tại bản ghi nào!",
                                            StatusCode = System.Net.HttpStatusCode.ExpectationFailed
                                        };
                                    }
                                    bool checkIsNull = false;
                                    cv.ForEach(g =>
                                    {
                                        if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "A")).First() == null
                                        || g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "C")).First() == null
                                        || g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "I")).First() == null
                                        || g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "L")).First() == null)
                                        {
                                            checkIsNull = true;
                                        }
                                        else
                                        {
                                            if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "A")).First().Value == null
                                           || g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "C")).First().Value == null
                                           || g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "I")).First().Value == null
                                           || g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "L")).First().Value == null
                                           || g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "A")).First().Text.Replace(" ", "") == ""
                                           || g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "C")).First().Text.Replace(" ", "") == ""
                                           || g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "L")).First().Text.Replace(" ", "") == ""
                                           || g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "I")).First().Text.Replace(" ", "") == ""
                                           )
                                            {
                                                checkIsNull = true;
                                            }
                                        }
                                    });

                                    if (checkIsNull)
                                    {
                                        return new ImportCustomerResult
                                        {
                                            MessageCode = "Import không thành công, một số trường bắt buộc chưa nhập",
                                            StatusCode = System.Net.HttpStatusCode.ExpectationFailed
                                        };
                                    }
                                    //Auto gen CustomerCode
                                    #region comment By Hung
                                    //int currentYear = DateTime.Now.Year % 100;
                                    //int currentMonth = DateTime.Now.Month;
                                    //int currentDate = DateTime.Now.Day;
                                    //var lstRequestPayment = context.Customer.Where(w => w.YearCode == currentYear && w.MonthCode == currentMonth && w.DateCode == currentDate).Select(s => s.NumberCode.Value).ToList();
                                    //int MaxNumberCode = 0;
                                    //if (lstRequestPayment.Count > 0)
                                    //{
                                    //    MaxNumberCode = lstRequestPayment.Max();
                                    //}
                                    #endregion
                                    int MaxNumberCode = 0;
                                    cv.ForEach(g =>
                                    {
                                        if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "B")).First() != null)
                                        {
                                            dulicateTaxcode.Add(g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "B")).First().Text);
                                        }
                                        if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "J")).First() != null)
                                        {
                                            dulicateEmail.Add(g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "J")).First().Text);
                                        }
                                        if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "I")).First() != null)
                                        {
                                            dulicatePhone.Add(g.Select(o => o.FirstOrDefault(rc => rc.Start.Address.Substring(0, 1) == "I")).First().Text);
                                        }
                                    });

                                    //Kiểm tra trùng email và sđt trong file
                                    List<string> lstDublicateTaxcode = new List<string>();
                                    List<string> lstDublicateEmail = new List<string>();
                                    List<string> lstDublicatePhone = new List<string>();
                                    List<string> stt = new List<string>();

                                    //Lọc các giá trị taxcode != ""
                                    dulicateTaxcode = dulicateTaxcode.Where(x => x.Trim() != "").ToList();
                                    //Danh sách taxcode không có giá trị bị trùng
                                    var lstDistinctTaxcode = dulicateTaxcode.Distinct().ToList();
                                    //Danh sách taxcode bị trùng
                                    lstDublicateTaxcode = dulicateTaxcode.GroupBy(x => x)
                                                          .Where(g => g.Count() > 1)
                                                          .Select(y => y.Key)
                                                          .ToList();

                                    //Lọc các giá trị email != ""
                                    dulicateEmail = dulicateEmail.Where(x => x.Trim() != "").ToList();
                                    //Danh sách email không có giá trị bị trùng
                                    var lstDistinctEmail = dulicateEmail.Distinct().ToList();
                                    //Danh sách email bị trùng
                                    lstDublicateEmail = dulicateEmail.GroupBy(x => x)
                                                          .Where(g => g.Count() > 1)
                                                          .Select(y => y.Key)
                                                          .ToList();

                                    //Danh sách số điện thoại không có giá trị bị trùng
                                    var lstDistinctPhone = dulicatePhone.Distinct().ToList();
                                    //Danh sách số điện thoại bị trùng
                                    lstDublicatePhone = dulicatePhone.GroupBy(x => x)
                                                          .Where(g => g.Count() > 1)
                                                          .Select(y => y.Key)
                                                          .ToList();

                                    if (lstDublicateTaxcode.Count > 0 || lstDublicateEmail.Count > 0 || lstDublicatePhone.Count > 0)
                                    {
                                        checkIsDublicate = true;

                                        //Lấy stt của row bị trùng
                                        if (lstDublicateTaxcode.Count > 0)
                                        {
                                            cv.ForEach(item =>
                                            {
                                                if (item.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "B")).First() != null)
                                                {
                                                    var currenTaxcode = item.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "B")).First().Text.Trim();
                                                    if (lstDublicateTaxcode.FirstOrDefault(x => x == currenTaxcode) != null)
                                                    {
                                                        stt.Add(item.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "A")).First().Text.Trim());
                                                    }
                                                }
                                            });
                                        }

                                        if (lstDublicateEmail.Count > 0)
                                        {
                                            cv.ForEach(item =>
                                            {
                                                if (item.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "J")).First() != null)
                                                {
                                                    var currentEmail = item.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "J")).First().Text.Trim();
                                                    if (lstDublicateEmail.FirstOrDefault(x => x == currentEmail) != null)
                                                    {
                                                        stt.Add(item.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "A")).First().Text.Trim());
                                                    }
                                                }
                                            });
                                        }

                                        if (lstDublicatePhone.Count > 0)
                                        {
                                            cv.ForEach(item =>
                                            {
                                                if (item.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "I")).First() != null)
                                                {
                                                    var currentPhone = item.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "I")).First().Text.Trim();
                                                    if (lstDublicatePhone.FirstOrDefault(x => x == currentPhone) != null)
                                                    {
                                                        stt.Add(item.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "A")).First().Text.Trim());
                                                    }
                                                }
                                            });
                                        }
                                    }

                                    stt = stt.Distinct().ToList();

                                    cv.ForEach(g =>
                                    {
                                        if (g.Key.Value == null) return;

                                        #region comment by giang
                                        //countDulicateEmail = 0;
                                        //countDulicatePhone = 0;
                                        #endregion

                                        string Taxcode = string.Empty;
                                        if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "B")).First() != null)
                                        {
                                            if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "B")).First().Value != null)
                                            {
                                                Taxcode = g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "B")).First().Value.ToString();
                                            }
                                        }
                                        string Email = string.Empty;
                                        if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "J")).First() != null)
                                        {
                                            if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "J")).First().Value != null)
                                            {
                                                Email = g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "J")).First().Value.ToString();
                                            }
                                        }
                                        string Phone = string.Empty;
                                        if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "I")).First() != null)
                                        {
                                            if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "I")).First().Value != null)
                                            {
                                                Phone = g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "I")).First().Value.ToString();
                                            }
                                        }
                                        Contact contactEntityCheckTaxcodeDuplicate = null;
                                        Contact contactEntityCheckEmailDuplicate = null;
                                        Contact contactEntityCheckPhoneDuplicate = null;

                                        if (!string.IsNullOrEmpty(Taxcode))
                                        {
                                            contactEntityCheckTaxcodeDuplicate = context.Contact.Where(w => w.TaxCode.ToLower() == Taxcode.Trim().ToLower()).FirstOrDefault();
                                        }

                                        if (!string.IsNullOrEmpty(Email))
                                        {
                                            contactEntityCheckEmailDuplicate = context.Contact.Where(w => w.Email.ToLower() == Email.Trim().ToLower() && w.Active == true).FirstOrDefault();
                                        }

                                        if (!string.IsNullOrEmpty(Phone))
                                        {
                                            contactEntityCheckPhoneDuplicate = context.Contact.Where(w => w.Phone.ToLower() == Phone.Trim().ToLower() && w.Active == true).FirstOrDefault();
                                        }

                                        if (contactEntityCheckEmailDuplicate == null && contactEntityCheckPhoneDuplicate == null && contactEntityCheckTaxcodeDuplicate == null)
                                        {
                                            //Nếu Mã số thuế, Email hoặc Phone không bị trùng trên server thì:
                                            if (stt.FirstOrDefault(x => x == g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "A")).First().Text.Trim()) == null)
                                            {
                                                //Nếu không nằm trong danh sách các khách hàng bị trùng email hoặc số điện thoại trong file thì:
                                                //khách hàng
                                                string CustomerNameV = string.Empty;
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "C")).First() != null)
                                                {
                                                    if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "C")).First().Value != null)
                                                    {
                                                        CustomerNameV = g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "C")).First().Value.ToString().Trim();
                                                    }
                                                }

                                                Customer CreateNewCustomer = new Customer
                                                {
                                                    CustomerId = Guid.NewGuid(),
                                                    CustomerCode = this.GenerateCustomerCode(MaxNumberCode),    //Edit By Hung//string.Format("CTM{0}{1}{2}{3}", currentYear, currentMonth, currentDate, (MaxNumberCode).ToString("D4")),
                                                    CustomerName = CustomerNameV,
                                                    CustomerType = 2,
                                                    StatusId = listCustomerStatus.Where(w => w.CategoryCode == "MOI").Select(d => (Guid?)d.CategoryId).DefaultIfEmpty(null).FirstOrDefault().Value,
                                                    CreatedById = parameter.UserId,
                                                    CreatedDate = DateTime.Now
                                                };
                                                MaxNumberCode++;
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "L")).First() != null)
                                                {
                                                    if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "L")).First().Value != null)
                                                    {
                                                        string TextExcel = g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "L")).First().Value.ToString();
                                                        var objectCustomerGroup = listCustomerGroup.Where(w => w.CategoryCode == TextExcel).FirstOrDefault();
                                                        CreateNewCustomer.CustomerGroupId = objectCustomerGroup != null ? objectCustomerGroup.CategoryId : Guid.Empty;
                                                    }
                                                }

                                                #region comment by giang
                                                //for (int i = 0; i < dulicateEmail.Count; i++)
                                                //{
                                                //    if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "J")).First().Value == dulicateEmail[i])
                                                //    {
                                                //        countDulicateEmail++;
                                                //    }
                                                //}
                                                //for (int i = 0; i < dulicatePhone.Count; i++)
                                                //{
                                                //    if (g.Select(o => o.FirstOrDefault(rc => rc.Start.Address.Substring(0, 1) == "I")).First().Value == dulicatePhone[i])
                                                //    {
                                                //        countDulicatePhone++;
                                                //    }
                                                //}


                                                //if (countDulicateEmail <= 1 && countDulicatePhone <= 1)
                                                //{
                                                //    context.Customer.Add(CreateNewCustomer);
                                                //    context.SaveChanges();
                                                //}
                                                #endregion

                                                lstCustomer.Add(CreateNewCustomer);

                                                //thông tin chi tiết của khách hàng
                                                string FullNameContact_COn = string.Empty;
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "C")).First() != null)
                                                {
                                                    if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "C")).First().Value != null)
                                                    {
                                                        FullNameContact_COn = g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "C")).First().Value.ToString().Trim();
                                                    }
                                                }
                                                string[] array = FullNameContact_COn.Split(' ');
                                                string FirstName = array[0];
                                                string LastName = string.Empty;
                                                for (int i = 1; i < array.Length; ++i)
                                                {
                                                    LastName = LastName + " " + array[i];
                                                };

                                                Contact CreateContact = new Contact();
                                                CreateContact.ObjectId = CreateNewCustomer.CustomerId;
                                                CreateContact.FirstName = FirstName;
                                                CreateContact.LastName = LastName;
                                                CreateContact.ObjectType = "CUS";

                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "B")).First() != null)
                                                {
                                                    if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "B")).First().Value != null)
                                                    {
                                                        CreateContact.TaxCode = g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "B")).First().Value.ToString().Trim();
                                                    }
                                                }
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "F")).First() != null)
                                                {
                                                    if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "F")).First().Value != null)
                                                    {
                                                        CreateContact.ProvinceId = (listProvince.Where(x => x.ProvinceCode == g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "F")).First().Value.ToString()).Select(d => (Guid?)d.ProvinceId).DefaultIfEmpty(null).FirstOrDefault());
                                                    }
                                                }
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "G")).First() != null)
                                                {
                                                    if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "G")).First().Value != null)
                                                    {
                                                        CreateContact.DistrictId = (listDistrict.Where(x => x.DistrictCode == g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "G")).First().Value.ToString()).Select(d => (Guid?)d.DistrictId).DefaultIfEmpty(null).FirstOrDefault());
                                                    }
                                                }
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "H")).First() != null)
                                                {
                                                    if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "H")).First().Value != null)
                                                    {
                                                        CreateContact.WardId = (listWard.Where(x => x.WardCode == g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "H")).First().Value.ToString()).Select(d => (Guid?)d.WardId).DefaultIfEmpty(null).FirstOrDefault());
                                                    }
                                                }
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "E")).First() != null)
                                                {
                                                    if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "E")).First().Value != null)
                                                    {
                                                        CreateContact.Address = g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "E")).First().Value.ToString().Trim();
                                                    }
                                                }
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "J")).First() != null)
                                                {
                                                    if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "J")).First().Value != null)
                                                    {
                                                        CreateContact.Email = g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "J")).First().Value.ToString().Trim();
                                                    }
                                                }
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "I")).First() != null)
                                                {
                                                    if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "I")).First().Value != null)
                                                    {
                                                        CreateContact.Phone = g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "I")).First().Value.ToString().Trim();
                                                    }
                                                }
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "K")).First() != null)
                                                {
                                                    if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "K")).First().Value != null)
                                                    {
                                                        CreateContact.Note = g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "K")).First().Value.ToString();
                                                    }
                                                }
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "D")).First() != null)
                                                {
                                                    if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "D")).First().Value != null)
                                                    {
                                                        CreateContact.Gender = g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "D")).First().Value.ToString();
                                                    }
                                                }
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "M")).First() != null)
                                                {
                                                    if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "M")).First().Value != null)
                                                    {
                                                        CreateContact.IdentityId = g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "M")).First().Value.ToString();
                                                    }
                                                }
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "N")).First() != null)
                                                {
                                                    if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "N")).First().Value != null)
                                                    {
                                                        CreateContact.DateOfBirth = Convert.ToDateTime(DateTime.ParseExact(g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "N")).First().Value.ToString().Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture));
                                                    }
                                                }
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "O")).First() != null)
                                                {
                                                    if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "O")).First().Value != null)
                                                    {
                                                        CreateContact.CompanyName = g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "O")).First().Value.ToString();
                                                    }
                                                }
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "P")).First() != null)
                                                {
                                                    if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "P")).First().Value != null)
                                                    {
                                                        CreateContact.CompanyAddress = g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "P")).First().Value.ToString();
                                                    }
                                                }

                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "Q")).First() != null)
                                                {
                                                    if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "Q")).First().Value != null)
                                                    {
                                                        CreateContact.CustomerPosition = (listCustomerPostion.Where(x => x.CategoryCode == g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "Q")).First().Value.ToString()).Select(d => (Guid?)d.CategoryId).DefaultIfEmpty(null).FirstOrDefault());
                                                    }
                                                }

                                                CreateContact.CreatedById = parameter.UserId;
                                                CreateContact.CreatedDate = DateTime.Now;
                                                lstcontact.Add(CreateContact);
                                            }
                                        }
                                        else
                                        {
                                            //Nếu có Email hoặc Phone bị trùng trên server thì:
                                            Contact contact;
                                            contact = contactEntityCheckEmailDuplicate != null ? contactEntityCheckEmailDuplicate : contactEntityCheckPhoneDuplicate;
                                            string CustomerNameV = string.Empty;
                                            if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "C")).First() != null)
                                            {
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "C")).First().Value != null)
                                                {
                                                    CustomerNameV = g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "C")).First().Value.ToString();
                                                }
                                            }

                                            CustomerEntityModel CustomerDuplicate = new CustomerEntityModel
                                            {
                                                CustomerId = contact.ObjectId,
                                                CustomerName = CustomerNameV,
                                            };
                                            if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "L")).First() != null)
                                            {
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "L")).First().Value != null)
                                                {
                                                    string TextExcel = g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "L")).First().Value.ToString();
                                                    var objectCustomerGroup = listCustomerGroup.Where(w => w.CategoryCode == TextExcel).FirstOrDefault();
                                                    CustomerDuplicate.CustomerGroupId = objectCustomerGroup != null ? objectCustomerGroup.CategoryId : Guid.Empty;
                                                }
                                            }
                                            lstcontactCustomerDuplicate.Add(CustomerDuplicate);

                                            //thông tin chi tiết của khách hàng
                                            string FullNameContact_COn = string.Empty;
                                            if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "C")).First() != null)
                                            {
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "C")).First().Value != null)
                                                {
                                                    FullNameContact_COn = g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "C")).First().Value.ToString();
                                                }
                                            }
                                            string[] array = FullNameContact_COn.Split(' ');
                                            string FirstName = array[0];
                                            string LastName = string.Empty;
                                            for (int i = 1; i < array.Length; ++i)
                                            {
                                                LastName = LastName + " " + array[i];
                                            };


                                            ContactEntityModel ContactDuplicate = new ContactEntityModel();
                                            ContactDuplicate.ObjectId = contact.ObjectId;
                                            ContactDuplicate.ContactId = contact.ContactId;
                                            ContactDuplicate.FirstName = FirstName;
                                            ContactDuplicate.LastName = LastName;

                                            if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "B")).First() != null)
                                            {
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "B")).First().Value != null)
                                                {
                                                    ContactDuplicate.TaxCode = g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "B")).First().Value.ToString().Trim();
                                                }
                                            }

                                            if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "F")).First() != null)
                                            {
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "F")).First().Value != null)
                                                {
                                                    ContactDuplicate.ProvinceId = (listProvince.Where(x => x.ProvinceCode == g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "F")).First().Value.ToString()).Select(d => (Guid?)d.ProvinceId).DefaultIfEmpty(null).FirstOrDefault());
                                                }
                                            }
                                            if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "G")).First() != null)
                                            {
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "G")).First().Value != null)
                                                {
                                                    ContactDuplicate.DistrictId = (listDistrict.Where(x => x.DistrictCode == g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "G")).First().Value.ToString()).Select(d => (Guid?)d.DistrictId).DefaultIfEmpty(null).FirstOrDefault());
                                                }
                                            }
                                            if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "H")).First() != null)
                                            {
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "H")).First().Value != null)
                                                {
                                                    ContactDuplicate.WardId = (listWard.Where(x => x.WardCode == g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "H")).First().Value.ToString()).Select(d => (Guid?)d.WardId).DefaultIfEmpty(null).FirstOrDefault());
                                                }
                                            }
                                            if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "E")).First() != null)
                                            {
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "E")).First().Value != null)
                                                {
                                                    ContactDuplicate.Address = g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "E")).First().Value.ToString();
                                                }
                                            }
                                            if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "J")).First() != null)
                                            {
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "J")).First().Value != null)
                                                {
                                                    ContactDuplicate.Email = g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "J")).First().Value.ToString().Trim();
                                                }
                                            }
                                            if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "I")).First() != null)
                                            {
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "I")).First().Value != null)
                                                {
                                                    ContactDuplicate.Phone = g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "I")).First().Value.ToString().Trim();
                                                }
                                            }
                                            if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "K")).First() != null)
                                            {
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "K")).First().Value != null)
                                                {
                                                    ContactDuplicate.Note = g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "K")).First().Value.ToString();
                                                }
                                            }
                                            if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "D")).First() != null)
                                            {
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "D")).First().Value != null)
                                                {
                                                    ContactDuplicate.Gender = g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "D")).First().Value.ToString();
                                                }
                                            }
                                            if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "M")).First() != null)
                                            {
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "M")).First().Value != null)
                                                {
                                                    ContactDuplicate.IdentityId = g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "M")).First().Value.ToString();
                                                }
                                            }
                                            if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "N")).First() != null)
                                            {
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "N")).First().Value != null)
                                                {
                                                    ContactDuplicate.DateOfBirth = Convert.ToDateTime(DateTime.ParseExact(g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "N")).First().Value.ToString().Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture));
                                                }
                                            }

                                            if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "O")).First() != null)
                                            {
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "O")).First().Value != null)
                                                {
                                                    ContactDuplicate.CompanyName = g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "O")).First().Value.ToString().Trim();
                                                }
                                            }
                                            if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "P")).First() != null)
                                            {
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "P")).First().Value != null)
                                                {
                                                    ContactDuplicate.CompanyAddress = g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "P")).First().Value.ToString();
                                                }
                                            }

                                            if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "Q")).First() != null)
                                            {
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "Q")).First().Value != null)
                                                {
                                                    ContactDuplicate.CustomerPosition = (listCustomerPostion.Where(x => x.CategoryCode == g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "Q")).First().Value.ToString()).Select(d => (Guid?)d.CategoryId).DefaultIfEmpty(null).FirstOrDefault());
                                                }
                                            }

                                            lstcontactContactDuplicate.Add(ContactDuplicate);
                                        }
                                    });

                                    if (checkIsDublicate)
                                    {
                                        listDupblicateCustomerString = "";
                                        stt.ForEach(item =>
                                        {
                                            listDupblicateCustomerString += item + ", ";
                                        });
                                    }

                                    context.Customer.AddRange(lstCustomer);
                                    context.Contact.AddRange(lstcontact);
                                    context.SaveChanges();
                                }
                                else
                                {
                                    ExcelWorksheet worksheet = package.Workbook.Worksheets["HKD"];

                                    if (worksheet == null)
                                    {
                                        return new ImportCustomerResult
                                        {
                                            MessageCode = "File excel không đúng theo template",
                                            StatusCode = System.Net.HttpStatusCode.ExpectationFailed
                                        };
                                    }
                                    //Group cells by row
                                    var rowcellgroups = worksheet.Cells["A:N"].GroupBy(c => c.Start.Row);
                                    //Loại bỏ 2 dòng tiêu đề
                                    var groups = rowcellgroups.Skip(1);
                                    //Group theo từng ngày
                                    var cv = (from item in groups
                                              group item by new
                                              {
                                                  item.First().Value,
                                              } into gcs
                                              select gcs).ToList();
                                    if (cv.Count == 0)
                                    {
                                        return new ImportCustomerResult
                                        {
                                            MessageCode = "File excel không tồn tại bản ghi nào!",
                                            StatusCode = System.Net.HttpStatusCode.ExpectationFailed
                                        };
                                    }

                                    //Auto gen CustomerCode
                                    int currentYear = DateTime.Now.Year % 100;
                                    int currentMonth = DateTime.Now.Month;
                                    int currentDate = DateTime.Now.Day;
                                    var lstRequestPayment = context.Customer.Where(w => w.YearCode == currentYear && w.MonthCode == currentMonth && w.DateCode == currentDate).Select(s => s.NumberCode.Value).ToList();
                                    int MaxNumberCode = 0;
                                    if (lstRequestPayment.Count > 0)
                                    {
                                        MaxNumberCode = lstRequestPayment.Max();
                                    }

                                    bool checkIsNull = false;
                                    cv.ForEach(g =>
                                    {
                                        if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "A")).First() == null
                                        || g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "C")).First() == null
                                        || g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "I")).First() == null
                                        || g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "L")).First() == null)
                                        {
                                            checkIsNull = true;
                                        }
                                        else
                                        {
                                            if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "A")).First().Value == null
                                           || g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "C")).First().Value == null
                                           || g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "I")).First().Value == null
                                           || g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "L")).First().Value == null
                                           || g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "A")).First().Text.Replace(" ", "") == ""
                                           || g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "C")).First().Text.Replace(" ", "") == ""
                                           || g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "L")).First().Text.Replace(" ", "") == ""
                                           || g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "I")).First().Text.Replace(" ", "") == ""
                                           )
                                            {
                                                checkIsNull = true;
                                            }
                                        }
                                    });

                                    if (checkIsNull)
                                    {
                                        return new ImportCustomerResult
                                        {
                                            MessageCode = "Import không thành công, một số trường bắt buộc chưa nhập",
                                            StatusCode = System.Net.HttpStatusCode.ExpectationFailed
                                        };
                                    }

                                    cv.ForEach(g =>
                                    {
                                        if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "B")).First() != null)
                                        {
                                            dulicateTaxcode.Add(g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "B")).First().Text.Trim());
                                        }

                                        if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "J")).First() != null)
                                        {
                                            dulicateEmail.Add(g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "J")).First().Text);
                                        }

                                        if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "I")).First() != null)
                                        {
                                            dulicatePhone.Add(g.Select(o => o.FirstOrDefault(rc => rc.Start.Address.Substring(0, 1) == "I")).First().Text);
                                        }
                                    });

                                    #region Kiểm tra trùng taxcode, email và sđt trong file
                                    List<string> lstDupblicateTaxCode = new List<string>();
                                    List<string> lstDublicateEmail = new List<string>();
                                    List<string> lstDublicatePhone = new List<string>();
                                    List<string> stt = new List<string>();

                                    //Danh sách taxcode != ""
                                    var lstTaxcodeNotNullOrEmpty = dulicateTaxcode.Where(x => x.Trim() != "").ToList();
                                    //Danh sách taxcode không có giá trị bị trùng
                                    var lstDistinctTaxcode = lstTaxcodeNotNullOrEmpty.Distinct().ToList();
                                    //Danh sách email bị trùng
                                    lstDupblicateTaxCode = lstTaxcodeNotNullOrEmpty.GroupBy(x => x)
                                                          .Where(g => g.Count() > 1)
                                                          .Select(y => y.Key)
                                                          .ToList();

                                    //Lọc các giá trị email != ""
                                    dulicateEmail = dulicateEmail.Where(x => x.Trim() != "").ToList();
                                    //Danh sách email không có giá trị bị trùng
                                    var lstDistinctEmail = dulicateEmail.Distinct().ToList();
                                    //Danh sách email bị trùng
                                    lstDublicateEmail = dulicateEmail.GroupBy(x => x)
                                                          .Where(g => g.Count() > 1)
                                                          .Select(y => y.Key)
                                                          .ToList();

                                    //Danh sách số điện thoại không có giá trị bị trùng
                                    var lstDistinctPhone = dulicatePhone.Distinct().ToList();
                                    //Danh sách số điện thoại bị trùng
                                    lstDublicatePhone = dulicatePhone.GroupBy(x => x)
                                                          .Where(g => g.Count() > 1)
                                                          .Select(y => y.Key)
                                                          .ToList();

                                    if (lstDupblicateTaxCode.Count > 0 || lstDublicateEmail.Count > 0 || lstDublicatePhone.Count > 0)
                                    {
                                        checkIsDublicate = true;

                                        //Lấy stt của row bị trùng
                                        if (lstDupblicateTaxCode.Count > 0)
                                        {
                                            cv.ForEach(item =>
                                            {
                                                if (item.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "B")).First() != null)
                                                {
                                                    var currentTaxcode = item.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "B")).First().Text.Trim();
                                                    if (lstDupblicateTaxCode.FirstOrDefault(x => x == currentTaxcode) != null)
                                                    {
                                                        stt.Add(item.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "A")).First().Text.Trim());
                                                    }
                                                }
                                            });
                                        }

                                        if (lstDublicateEmail.Count > 0)
                                        {
                                            cv.ForEach(item =>
                                            {
                                                if (item.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "J")).First() != null)
                                                {
                                                    var currentEmail = item.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "J")).First().Text.Trim();
                                                    if (lstDublicateEmail.FirstOrDefault(x => x == currentEmail) != null)
                                                    {
                                                        stt.Add(item.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "A")).First().Text.Trim());
                                                    }
                                                }
                                            });
                                        }

                                        if (lstDublicatePhone.Count > 0)
                                        {
                                            cv.ForEach(item =>
                                            {
                                                if (item.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "I")).First() != null)
                                                {
                                                    var currentPhone = item.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "I")).First().Text.Trim();
                                                    if (lstDublicatePhone.FirstOrDefault(x => x == currentPhone) != null)
                                                    {
                                                        stt.Add(item.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "A")).First().Text.Trim());
                                                    }
                                                }
                                            });
                                        }
                                    }

                                    stt = stt.Distinct().ToList();
                                    #endregion

                                    cv.ForEach(g =>
                                    {
                                        if (g.Key.Value == null) return;
                                        string Email = string.Empty;

                                        #region comment by giang
                                        //countDulicateEmail = 0;
                                        //countDulicatePhone = 0;
                                        //for (int i = 0; i < dulicateEmail.Count; i++)
                                        //{
                                        //    if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "J")).First().Value == dulicateEmail[i])
                                        //    {
                                        //        countDulicateEmail++;
                                        //    }
                                        //}
                                        //for (int i = 0; i < dulicatePhone.Count; i++)
                                        //{
                                        //    if (g.Select(o => o.FirstOrDefault(rc => rc.Start.Address.Substring(0, 1) == "I")).First().Value == dulicatePhone[i])
                                        //    {
                                        //        countDulicatePhone++;
                                        //    }
                                        //}
                                        #endregion

                                        if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "J")).First() != null)
                                        {
                                            if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "J")).First().Value != null)
                                            {
                                                Email = g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "J")).First().Value.ToString();
                                            }
                                        }
                                        string Phone = string.Empty;
                                        if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "I")).First() != null)
                                        {
                                            if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "I")).First().Value != null)
                                            {
                                                Phone = g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "I")).First().Value.ToString();
                                            }
                                        }
                                        Contact contactEntityCheckEmailDuplicate = null;
                                        Contact contactEntityCheckPhoneDuplicate = null;

                                        if (!string.IsNullOrEmpty(Email))
                                        {
                                            contactEntityCheckEmailDuplicate = context.Contact.Where(w => w.Email.ToLower() == Email.Trim().ToLower() && w.Active == true).FirstOrDefault();
                                        }

                                        if (!string.IsNullOrEmpty(Phone))
                                        {
                                            contactEntityCheckPhoneDuplicate = context.Contact.Where(w => w.Phone.ToLower() == Phone.Trim().ToLower() && w.Active == true).FirstOrDefault();
                                        }

                                        if (contactEntityCheckEmailDuplicate == null && contactEntityCheckPhoneDuplicate == null)
                                        {
                                            //Nếu email, số điện thoại chưa tồn tại trên server thì:
                                            if (stt.FirstOrDefault(x => x == g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "A")).First().Text.Trim()) == null)
                                            {
                                                MaxNumberCode = MaxNumberCode + 1;
                                                //khách hàng

                                                string CustomerNameV = string.Empty;
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "C")).First() != null)
                                                {
                                                    if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "C")).First().Value != null)
                                                    {
                                                        CustomerNameV = g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "C")).First().Value.ToString();
                                                    }
                                                }

                                                Customer CreateNewCustomer = new Customer
                                                {
                                                    CustomerId = Guid.NewGuid(),
                                                    //Comment by Giang: CustomerCode = string.Format("CTM{0}{1}{2}{3}", currentYear, currentMonth, currentDate, (MaxNumberCode).ToString("D4")),
                                                    CustomerName = CustomerNameV,
                                                    CustomerType = 3,
                                                    NumberCode = MaxNumberCode,
                                                    YearCode = currentYear,
                                                    MonthCode = currentMonth,
                                                    DateCode = currentDate,
                                                    StatusId = listCustomerStatus.Where(w => w.CategoryCode == "MOI").Select(d => (Guid?)d.CategoryId).DefaultIfEmpty(null).FirstOrDefault().Value,
                                                    CreatedById = parameter.UserId,
                                                    CreatedDate = DateTime.Now
                                                };

                                                #region Tạo CustomerCode (add by giang)
                                                //Nếu có taxcode thì lấy taxcode:
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "B")).First() != null)
                                                {
                                                    if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "B")).First().Value != null)
                                                    {
                                                        CreateNewCustomer.CustomerCode = g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "B")).First().Value.ToString().Trim();
                                                    }
                                                }

                                                //Nếu không có taxcode thì hệ thống tự set:
                                                if (CreateNewCustomer.CustomerCode == null)
                                                {
                                                    CreateNewCustomer.CustomerCode = string.Format("CTM{0}{1}{2}{3}", currentYear, currentMonth, currentDate, (MaxNumberCode).ToString("D4"));
                                                }
                                                #endregion

                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "L")).First() != null)
                                                {
                                                    if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "L")).First().Value != null)
                                                    {
                                                        string TextExcel = g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "L")).First().Value.ToString();
                                                        var objectCustomerGroup = listCustomerGroup.Where(w => w.CategoryCode == TextExcel).FirstOrDefault();
                                                        CreateNewCustomer.CustomerGroupId = objectCustomerGroup != null ? objectCustomerGroup.CategoryId : Guid.Empty;
                                                    }
                                                }
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "N")).First() != null)
                                                {
                                                    if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "N")).First().Value != null)
                                                    {
                                                        CreateNewCustomer.MainBusinessSector = (listMainBusinessSector.Where(x => x.CategoryCode == g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "N")).First().Value.ToString()).Select(d => (Guid?)d.CategoryId).DefaultIfEmpty(null).FirstOrDefault());
                                                    }
                                                }

                                                #region comment by Giang
                                                //if (countDulicateEmail <= 1 && countDulicatePhone <= 1)
                                                //{
                                                //    context.Customer.Add(CreateNewCustomer);
                                                //    context.SaveChanges();
                                                //}
                                                #endregion

                                                lstCustomer.Add(CreateNewCustomer);

                                                //thông tin chi tiết của khách hàng
                                                string FullNameContact_COn = string.Empty;
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "C")).First() != null)
                                                {
                                                    if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "C")).First().Value != null)
                                                    {
                                                        FullNameContact_COn = g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "C")).First().Value.ToString().Trim();
                                                    }
                                                }
                                                string[] array = FullNameContact_COn.Split(' ');
                                                string FirstName = array[0];
                                                string LastName = string.Empty;
                                                for (int i = 1; i < array.Length; ++i)
                                                {
                                                    LastName = LastName + " " + array[i];
                                                };

                                                Contact CreateContact = new Contact();
                                                CreateContact.ObjectId = CreateNewCustomer.CustomerId;
                                                CreateContact.FirstName = FirstName;
                                                CreateContact.LastName = LastName;
                                                CreateContact.ObjectType = "CUS";

                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "B")).First() != null)
                                                {
                                                    if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "B")).First().Value != null)
                                                    {
                                                        CreateContact.TaxCode = g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "B")).First().Value.ToString().Trim();
                                                    }
                                                }

                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "F")).First() != null)
                                                {
                                                    if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "F")).First().Value != null)
                                                    {
                                                        CreateContact.ProvinceId = (listProvince.Where(x => x.ProvinceCode == g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "F")).First().Value.ToString()).Select(d => (Guid?)d.ProvinceId).DefaultIfEmpty(null).FirstOrDefault());
                                                    }
                                                }

                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "G")).First() != null)
                                                {
                                                    if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "G")).First().Value != null)
                                                    {
                                                        CreateContact.DistrictId = (listDistrict.Where(x => x.DistrictCode == g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "G")).First().Value.ToString()).Select(d => (Guid?)d.DistrictId).DefaultIfEmpty(null).FirstOrDefault());
                                                    }
                                                }

                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "H")).First() != null)
                                                {
                                                    if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "H")).First().Value != null)
                                                    {
                                                        CreateContact.WardId = (listWard.Where(x => x.WardCode == g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "H")).First().Value.ToString()).Select(d => (Guid?)d.WardId).DefaultIfEmpty(null).FirstOrDefault());
                                                    }
                                                }

                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "E")).First() != null)
                                                {
                                                    if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "E")).First().Value != null)
                                                    {
                                                        CreateContact.Address = g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "E")).First().Value.ToString();
                                                    }
                                                }

                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "J")).First() != null)
                                                {
                                                    if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "J")).First().Value != null)
                                                    {
                                                        CreateContact.Email = g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "J")).First().Value.ToString().Trim();
                                                    }
                                                }
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "I")).First() != null)
                                                {
                                                    if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "I")).First().Value != null)
                                                    {
                                                        CreateContact.Phone = g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "I")).First().Value.ToString().Trim();
                                                    }
                                                }
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "K")).First() != null)
                                                {
                                                    if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "K")).First().Value != null)
                                                    {
                                                        CreateContact.Note = g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "K")).First().Value.ToString().Trim();
                                                    }
                                                }

                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "D")).First() != null)
                                                {
                                                    if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "D")).First().Value != null)
                                                    {
                                                        CreateContact.Gender = g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "D")).First().Value.ToString();
                                                    }
                                                }
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "M")).First() != null)
                                                {
                                                    if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "M")).First().Value != null)
                                                    {
                                                        CreateContact.IdentityId = g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "M")).First().Value.ToString();
                                                    }
                                                }

                                                CreateContact.CreatedById = parameter.UserId;
                                                CreateContact.CreatedDate = DateTime.Now;

                                                lstcontact.Add(CreateContact);

                                                #region comment by giang
                                                //if (countDulicatePhone <= 1 && countDulicateEmail <= 1)
                                                //{
                                                //    lstcontact.Add(CreateContact);
                                                //}
                                                #endregion
                                            }
                                        }
                                        else
                                        {
                                            //Nếu email, số điện thoại đã tồn tại trên server thì:
                                            Contact contact;
                                            contact = contactEntityCheckEmailDuplicate != null ? contactEntityCheckEmailDuplicate : contactEntityCheckPhoneDuplicate;
                                            string CustomerNameV = string.Empty;
                                            if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "C")).First() != null)
                                            {
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "C")).First().Value != null)
                                                {
                                                    CustomerNameV = g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "C")).First().Value.ToString();
                                                }
                                            }

                                            CustomerEntityModel CustomerDuplicate = new CustomerEntityModel
                                            {
                                                CustomerId = contact.ObjectId,
                                                CustomerName = CustomerNameV,
                                            };
                                            if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "L")).First() != null)
                                            {
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "L")).First().Value != null)
                                                {
                                                    string TextExcel = g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "L")).First().Value.ToString();
                                                    var objectCustomerGroup = listCustomerGroup.Where(w => w.CategoryCode == TextExcel).FirstOrDefault();
                                                    CustomerDuplicate.CustomerGroupId = objectCustomerGroup != null ? objectCustomerGroup.CategoryId : Guid.Empty;
                                                }
                                            }
                                            if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "N")).First() != null)
                                            {
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "N")).First().Value != null)
                                                {
                                                    CustomerDuplicate.MainBusinessSector = (listMainBusinessSector.Where(x => x.CategoryCode == g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "N")).First().Value.ToString()).Select(d => (Guid?)d.CategoryId).DefaultIfEmpty(null).FirstOrDefault());
                                                }
                                            }

                                            lstcontactCustomerDuplicate.Add(CustomerDuplicate);

                                            //thông tin chi tiết của khách hàng
                                            string FullNameContact_COn = string.Empty;
                                            if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "C")).First() != null)
                                            {
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "C")).First().Value != null)
                                                {
                                                    FullNameContact_COn = g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "C")).First().Value.ToString().Trim();
                                                }
                                            }
                                            string[] array = FullNameContact_COn.Split(' ');
                                            string FirstName = array[0];
                                            string LastName = string.Empty;
                                            for (int i = 1; i < array.Length; ++i)
                                            {
                                                LastName = LastName + " " + array[i];
                                            };


                                            ContactEntityModel ContactDuplicate = new ContactEntityModel();
                                            ContactDuplicate.ObjectId = contact.ObjectId;
                                            ContactDuplicate.ContactId = contact.ContactId;
                                            ContactDuplicate.FirstName = FirstName;
                                            ContactDuplicate.LastName = LastName;

                                            if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "B")).First() != null)
                                            {
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "B")).First().Value != null)
                                                {
                                                    ContactDuplicate.TaxCode = g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "B")).First().Value.ToString().Trim();
                                                }
                                            }

                                            if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "F")).First() != null)
                                            {
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "F")).First().Value != null)
                                                {
                                                    ContactDuplicate.ProvinceId = (listProvince.Where(x => x.ProvinceCode == g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "F")).First().Value.ToString()).Select(d => (Guid?)d.ProvinceId).DefaultIfEmpty(null).FirstOrDefault());
                                                }
                                            }
                                            if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "G")).First() != null)
                                            {
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "G")).First().Value != null)
                                                {
                                                    ContactDuplicate.DistrictId = (listDistrict.Where(x => x.DistrictCode == g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "G")).First().Value.ToString()).Select(d => (Guid?)d.DistrictId).DefaultIfEmpty(null).FirstOrDefault());
                                                }
                                            }
                                            if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "E")).First() != null)
                                            {
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "E")).First().Value != null)
                                                {
                                                    ContactDuplicate.Address = g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "E")).First().Value.ToString();
                                                }
                                            }
                                            if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "J")).First() != null)
                                            {
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "J")).First().Value != null)
                                                {
                                                    ContactDuplicate.Email = g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "J")).First().Value.ToString().Trim();
                                                }
                                            }
                                            if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "I")).First() != null)
                                            {
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "I")).First().Value != null)
                                                {
                                                    ContactDuplicate.Phone = g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "I")).First().Value.ToString().Trim();
                                                }
                                            }
                                            if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "K")).First() != null)
                                            {
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "K")).First().Value != null)
                                                {
                                                    ContactDuplicate.Note = g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "K")).First().Value.ToString().Trim();
                                                }
                                            }
                                            if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "D")).First() != null)
                                            {
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "D")).First().Value != null)
                                                {
                                                    ContactDuplicate.Gender = g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "D")).First().Value.ToString();
                                                }
                                            }
                                            if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "M")).First() != null)
                                            {
                                                if (g.Select(o => o.FirstOrDefault(rc => rc.Address.Substring(0, 1) == "M")).First().Value != null)
                                                {
                                                    ContactDuplicate.IdentityId = g.Select(o => o.First(rc => rc.Start.Address.Substring(0, 1) == "M")).First().Value.ToString();
                                                }
                                            }
                                            lstcontactContactDuplicate.Add(ContactDuplicate);
                                        }
                                    });

                                    if (checkIsDublicate)
                                    {
                                        listDupblicateCustomerString = "";
                                        stt.ForEach(item =>
                                        {
                                            listDupblicateCustomerString += item + ", ";
                                        });
                                    }

                                    context.Customer.AddRange(lstCustomer);
                                    context.Contact.AddRange(lstcontact);
                                    context.SaveChanges();
                                }
                            }
                        }
                    }

                    string Message = string.Empty;
                    #region comment by Giang (Kiểm tra trùng email, phone trong file)
                    //for (int i = 0; i < dulicateEmail.Count; i++)
                    //{
                    //    countDulicateEmail = 1;
                    //    for (int j = i + 1; j < dulicateEmail.Count; j++)
                    //    {
                    //        if (dulicateEmail[i] == dulicateEmail[j])
                    //        {
                    //            countDulicateEmail++;
                    //        }
                    //    }
                    //    if (countDulicateEmail > 1) break;
                    //}
                    //for (int i = 0; i < dulicatePhone.Count; i++)
                    //{
                    //    countDulicatePhone = 1;
                    //    for (int j = i + 1; j < dulicatePhone.Count; j++)
                    //    {
                    //        if (dulicatePhone[i] == dulicatePhone[j])
                    //        {
                    //            countDulicatePhone++;
                    //        }
                    //    }
                    //    if (countDulicatePhone > 1) break;
                    //}

                    //if (lstcontactContactDuplicate.Count > 0 || lstcontactContact_CON_Duplicate.Count > 0 || lstcontactCustomerDuplicate.Count > 0 || countDulicatePhone > 1 || countDulicateEmail > 1)
                    //{
                    //    Message = "Đã import thành công,một số thông tin đã bị lặp";
                    //}
                    //else
                    //{
                    //    Message = "Đã import thành công";
                    //}
                    #endregion

                    if (checkIsDublicate)
                        listDupblicateCustomerString = listDupblicateCustomerString.Substring(0, listDupblicateCustomerString.LastIndexOf(","));

                    if (lstcontactContactDuplicate.Count > 0 || lstcontactContact_CON_Duplicate.Count > 0 || lstcontactCustomerDuplicate.Count > 0 || checkIsDublicate == true)
                    {
                        if (checkIsDublicate && lstcontactContactDuplicate.Count == 0 && lstcontactContact_CON_Duplicate.Count == 0 && lstcontactCustomerDuplicate.Count == 0)
                        {
                            Message = "Đã import thành công, một số thông tin đã bị lặp trong file Excel tại hàng có STT: " + listDupblicateCustomerString;
                        }
                        else if (checkIsDublicate && (lstcontactContactDuplicate.Count > 0 || lstcontactContact_CON_Duplicate.Count > 0 || lstcontactCustomerDuplicate.Count > 0))
                        {
                            Message = "Đã import thành công, một số thông tin đã tồn tại trên hệ thống. Một số thông tin đã bị lặp trong file Excel tại hàng có STT: " + listDupblicateCustomerString;
                        }
                        else if (!checkIsDublicate && (lstcontactContactDuplicate.Count > 0 || lstcontactContact_CON_Duplicate.Count > 0 || lstcontactCustomerDuplicate.Count > 0))
                        {
                            Message = "Đã import thành công, một số thông tin đã bị lặp";
                        }
                    }
                    else
                    {
                        Message = "Đã import thành công";
                    }

                    dbcxtransaction.Commit();
                    return new ImportCustomerResult
                    {
                        lstcontactContactDuplicate = lstcontactContactDuplicate,    //Contact của các customer trùng
                        lstcontactContact_CON_Duplicate = lstcontactContact_CON_Duplicate,  //Người liên hệ của các customer trùng
                        lstcontactCustomerDuplicate = lstcontactCustomerDuplicate,  //Các customer trùng
                        isDupblicateInFile = checkIsDublicate,
                        MessageCode = Message,
                        StatusCode = System.Net.HttpStatusCode.OK
                    };
                }
                catch (Exception ex)
                {
                    dbcxtransaction.Rollback();
                    return new ImportCustomerResult
                    {
                        MessageCode = "Đã có lỗi xảy ra trong quá trình import",
                        StatusCode = System.Net.HttpStatusCode.ExpectationFailed
                    };
                }
            }
        }

        public DownloadTemplateCustomerResult DownloadTemplateCustomer(DownloadTemplateCustomerParameter parameter)
        {
            try
            {
                string rootFolder = hostingEnvironment.WebRootPath + "\\ExcelTemplate";
                string fileName = @"Template_Customer_DN.xlsm";
                if (parameter.CustomerType == 1)
                {
                    fileName = @"Template_Customer_DN.xlsm";
                }
                else if (parameter.CustomerType == 2)
                {
                    fileName = @"Template_Customer_CN.xlsm";
                }
                else
                {
                    fileName = @"Template_Customer_HKD.xlsm";
                }
                //FileInfo file = new FileInfo(Path.Combine(rootFolder, fileName));
                string newFilePath = Path.Combine(rootFolder, fileName);
                byte[] data = File.ReadAllBytes(newFilePath);

                return new DownloadTemplateCustomerResult
                {
                    ExcelFile = data,
                    MessageCode = string.Format("Đã dowload file {0}", parameter.CustomerType == 1 ? "Template_Customer_DN" : (parameter.CustomerType == 2 ? "Template_Customer_CN" : "Template_Customer_HKD")),
                    NameFile = parameter.CustomerType == 1 ? "Template_Customer_DN" : (parameter.CustomerType == 2 ? "Template_Customer_CN" : "Template_Customer_HKD"),
                    StatusCode = System.Net.HttpStatusCode.OK
                };

            }
            catch (Exception)
            {
                return new DownloadTemplateCustomerResult
                {
                    MessageCode = "Đã có lỗi xảy ra trong quá trình download",
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed
                };
            }
        }

        public UpdateCustomerDuplicateResult UpdateCustomerDuplicate(UpdateCustomerDuplicateParameter parameter)
        {
            try
            {
                if (parameter.lstcontactCustomerDuplicate.Count > 0)
                {
                    parameter.lstcontactCustomerDuplicate.ForEach(item =>
                    {

                        var customerUpdate = context.Customer.FirstOrDefault(w => w.CustomerId == item.CustomerId);
                        if (customerUpdate != null)
                        {
                            customerUpdate.CustomerId = item.CustomerId;
                            if (item.CustomerGroupId != Guid.Empty)
                            {
                                if (item.CustomerGroupId != null)
                                {
                                    customerUpdate.CustomerGroupId = item.CustomerGroupId;
                                }
                            }
                            if (!string.IsNullOrEmpty(item.CustomerCode))
                            {
                                if (item.CustomerCode != null)
                                {
                                    customerUpdate.CustomerCode = item.CustomerCode;
                                }
                            }
                            customerUpdate.MainBusinessSector = item.MainBusinessSector;
                            customerUpdate.CustomerName = item.CustomerName;
                            customerUpdate.TotalCapital = item.TotalCapital;
                            customerUpdate.BusinessRegistrationDate = item.BusinessRegistrationDate;
                            customerUpdate.EnterpriseType = item.EnterpriseType;
                            customerUpdate.TotalEmployeeParticipateSocialInsurance = item.TotalEmployeeParticipateSocialInsurance;
                            customerUpdate.BusinessType = item.BusinessType;
                            if (item.FieldId != Guid.Empty)
                            {
                                if (item.FieldId != null)
                                {
                                    customerUpdate.FieldId = item.FieldId;
                                }
                            }
                            customerUpdate.TotalRevenueLastYear = item.TotalRevenueLastYear;
                            customerUpdate.BusinessScale = item.BusinessScale;
                            customerUpdate.UpdatedById = parameter.UserId;
                            customerUpdate.UpdatedDate = DateTime.Now;

                            context.Customer.Update(customerUpdate);
                            context.SaveChanges();
                        }
                    });

                }
                if (parameter.lstcontactContactDuplicate.Count > 0)
                {
                    parameter.lstcontactContactDuplicate.ForEach(item =>
                    {
                        var orderUpdate = context.Contact.FirstOrDefault(w => w.ContactId == item.ContactId);
                        if (orderUpdate != null)
                        {
                            orderUpdate.TaxCode = item.TaxCode;
                            if (item.ProvinceId != Guid.Empty)
                            {
                                if (item.ProvinceId != null)
                                {
                                    orderUpdate.ProvinceId = item.ProvinceId;
                                }
                            }
                            if (item.DistrictId != Guid.Empty)
                            {
                                if (item.DistrictId != null)
                                {
                                    orderUpdate.DistrictId = item.DistrictId;
                                }
                            }
                            orderUpdate.FirstName = item.FirstName;
                            orderUpdate.LastName = item.LastName;
                            orderUpdate.Address = item.Address;
                            orderUpdate.Email = item.Email;
                            orderUpdate.Phone = item.Phone;
                            orderUpdate.Note = item.Note;
                            orderUpdate.UpdatedById = parameter.UserId;
                            orderUpdate.UpdatedDate = DateTime.Now;

                            context.Contact.Update(orderUpdate);
                            context.SaveChanges();
                        }
                    });
                }

                if (parameter.lstcontactContact_CON_Duplicate.Count > 0)
                {
                    parameter.lstcontactContact_CON_Duplicate.ForEach(item =>
                    {
                        var contact_Con = context.Contact.Where(w => w.ObjectId == item.ObjectId && w.ObjectType == "CUS_CON").ToList();
                        if (contact_Con.Count > 0)
                        {
                            context.Contact.RemoveRange(contact_Con);
                            context.SaveChanges();
                        }
                    });
                    context.Contact.AddRange(parameter.lstcontactContact_CON_Duplicate);
                    context.SaveChanges();
                }
                return new UpdateCustomerDuplicateResult
                {
                    MessageCode = "Đã chỉnh sửa lại khách hàng",
                    StatusCode = System.Net.HttpStatusCode.OK
                };

            }
            catch (Exception ex)
            {
                return new UpdateCustomerDuplicateResult
                {
                    MessageCode = "Đã có lỗi xảy ra trong quá trình cập nhật lại khách hàng",
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed
                };
            }
        }

        public GetStatisticCustomerForDashboardResult GetStatisticCustomerForDashboard(GetStatisticCustomerForDashboardParameter parameter)
        {
            try
            {
                var employees = context.Employee.ToList();
                var organizations = context.Organization.ToList();
                var contacts = context.Contact.Where(w => w.ObjectType == ObjectType.CUSTOMER).ToList();
                var users = context.User.ToList();
                var customers = context.Customer.Where(w => w.Active == true).ToList();
                var orderStatus = context.OrderStatus.ToList();
                var customerOrders = context.CustomerOrder.ToList();
                var productCategorys = context.ProductCategory.ToList();
                var products = context.Product.ToList();
                var customerOrderDetails = context.CustomerOrderDetail.ToList();

                var currentEmp = employees.FirstOrDefault(emp => emp.EmployeeId == users.FirstOrDefault(u => u.UserId == parameter.UserId).EmployeeId);
                var orgRoot = organizations.FirstOrDefault(o => o.OrganizationId == currentEmp.OrganizationId);
                var listOrg = ListChildOfParamToSearch(orgRoot.OrganizationId, organizations);

                var _listCusIsNewest = (from cus in customers
                                            //join con in context.Contact on cus.CustomerId equals con.ObjectId
                                        join userCre in users on cus.CreatedById equals userCre.UserId
                                        join empCre in employees on userCre.EmployeeId equals empCre.EmployeeId
                                        //join empPic in context.Employee on cus.PersonInChargeId equals empPic.EmployeeId
                                        where cus.Active == true && cus.CustomerName != null && cus.CustomerName.ToLower().Contains(parameter.KeyName.ToLower()) &&
                                            (
                                                (!currentEmp.IsManager && (cus.CreatedById == parameter.UserId || (cus.PersonInChargeId != null && cus.PersonInChargeId.Value == currentEmp.EmployeeId)))
                                                ||
                                                (currentEmp.IsManager && (empCre.OrganizationId.Value == orgRoot.OrganizationId || listOrg.Contains(empCre.OrganizationId.Value) || (cus.PersonInChargeId != null && (employees.FirstOrDefault(emp => emp.EmployeeId == cus.PersonInChargeId).OrganizationId.Value == orgRoot.OrganizationId || listOrg.Contains(employees.FirstOrDefault(emp => emp.EmployeeId == cus.PersonInChargeId).OrganizationId.Value)))))
                                            )
                                        select new CustomerEntityModel()
                                        {
                                            CusAvatarUrl = contacts.FirstOrDefault(f => f.ObjectId == cus.CustomerId) == null ? "" : contacts.FirstOrDefault(f => f.ObjectId == cus.CustomerId).AvatarUrl,
                                            CustomerName = cus.CustomerName,
                                            CustomerId = cus.CustomerId,
                                            CustomerEmail = contacts.FirstOrDefault(f => f.ObjectId == cus.CustomerId) == null ? "" : contacts.FirstOrDefault(f => f.ObjectId == cus.CustomerId).Email,
                                            ContactId = contacts.FirstOrDefault(f => f.ObjectId == cus.CustomerId) == null ? Guid.Empty : contacts.FirstOrDefault(f => f.ObjectId == cus.CustomerId).ContactId,
                                            CreatedDate = cus.CreatedDate,
                                            CustomerPhone = contacts.FirstOrDefault(f => f.ObjectId == cus.CustomerId) == null ? "" : contacts.FirstOrDefault(f => f.ObjectId == cus.CustomerId).Phone,
                                            PicName = employees.FirstOrDefault(emp => emp.EmployeeId == cus.PersonInChargeId) == null ? "" : employees.FirstOrDefault(emp => emp.EmployeeId == cus.PersonInChargeId).EmployeeName,
                                        }).OrderByDescending(date => date.CreatedDate).ToList();

                var lstOrderStt = new List<string>() { "IP", "COMP", "DLV", "PD" };
                var _listCusIsNewBought = (from order in customerOrders
                                           join cus in customers on order.CustomerId equals cus.CustomerId
                                           //join con in context.Contact on cus.CustomerId equals con.ObjectId
                                           join stt in orderStatus on order.StatusId equals stt.OrderStatusId
                                           join userCre in users on cus.CreatedById equals userCre.UserId
                                           join empCre in employees on userCre.EmployeeId equals empCre.EmployeeId
                                           //join empPic in context.Employee on cus.PersonInChargeId equals empPic.EmployeeId
                                           where
                                                cus.Active == true && lstOrderStt.Contains(stt.OrderStatusCode) && cus.CustomerName.ToLower().Contains(parameter.KeyName.ToLower()) &&
                                                (
                                                    (!currentEmp.IsManager && (cus.CreatedById == parameter.UserId || (cus.PersonInChargeId != null && cus.PersonInChargeId.Value == currentEmp.EmployeeId))) ||
                                                    (currentEmp.IsManager && (empCre.OrganizationId.Value == orgRoot.OrganizationId || listOrg.Contains(empCre.OrganizationId.Value) || (cus.PersonInChargeId != null && (employees.FirstOrDefault(emp => emp.EmployeeId == cus.PersonInChargeId).OrganizationId.Value == orgRoot.OrganizationId || listOrg.Contains(employees.FirstOrDefault(emp => emp.EmployeeId == cus.PersonInChargeId).OrganizationId.Value)))))
                                                )
                                           select new CustomerEntityModel()
                                           {
                                               CustomerName = cus.CustomerName,
                                               CusAvatarUrl = contacts.FirstOrDefault(f => f.ObjectId == cus.CustomerId).AvatarUrl,
                                               CustomerEmail = contacts.FirstOrDefault(f => f.ObjectId == cus.CustomerId).Email,
                                               CustomerId = cus.CustomerId,
                                               ContactId = contacts.FirstOrDefault(f => f.ObjectId == cus.CustomerId).ContactId,
                                               CustomerPhone = contacts.FirstOrDefault(f => f.ObjectId == cus.CustomerId).Phone,
                                               PicName = employees.FirstOrDefault(emp => emp.EmployeeId == cus.PersonInChargeId) == null ? "" : employees.FirstOrDefault(emp => emp.EmployeeId == cus.PersonInChargeId).EmployeeName,
                                               TotalSaleValue = order.DiscountType == true ? order.Amount - order.Amount * order.DiscountValue : order.Amount - order.DiscountValue,
                                               CreatedDate = order.CreatedDate
                                           }).OrderByDescending(date => date.CreatedDate).ToList();

                #region comment

                //var _listCusSaleTop = (from or in customerOrders
                //                       join cus in customers on or.CustomerId equals cus.CustomerId
                //                       //join con in context.Contact on or.CustomerId equals con.ObjectId
                //                       join stt in orderStatus on or.StatusId equals stt.OrderStatusId
                //                       join userCre in users on cus.CreatedById equals userCre.UserId
                //                       join empCre in employees on userCre.EmployeeId equals empCre.EmployeeId
                //                       //join empPic in context.Employee on cus.PersonInChargeId equals empPic.EmployeeId
                //                       where
                //                                  lstOrderStt.Contains(stt.OrderStatusCode) &&
                //                                  //con.ObjectType == ObjectType.CUSTOMER &&
                //                                  cus.CustomerName.ToLower().Contains(parameter.KeyName.ToLower())
                //                                  &&
                //                                  (
                //                                    (!currentEmp.IsManager && (cus.CreatedById == parameter.UserId || (cus.PersonInChargeId != null && cus.PersonInChargeId.Value == currentEmp.EmployeeId))) ||
                //                                    (currentEmp.IsManager &&
                //                                        (empCre.OrganizationId.Value == orgRoot.OrganizationId ||
                //                                         listOrg.Contains(empCre.OrganizationId.Value) ||
                //                                         (cus.PersonInChargeId != null &&
                //                                          (
                //                                           employees.FirstOrDefault(emp => emp.EmployeeId == cus.PersonInChargeId).OrganizationId.Value == orgRoot.OrganizationId ||
                //                                           listOrg.Contains(employees.FirstOrDefault(emp => emp.EmployeeId == cus.PersonInChargeId).OrganizationId.Value)
                //                                          )
                //                                         )
                //                                        )
                //                                    )
                //                                  )
                //                       select new CustomerEntityModel()
                //                       {
                //                           CustomerName = cus.CustomerName,
                //                           CustomerCode = cus.CustomerCode,
                //                           CustomerId = cus.CustomerId,
                //                           CusAvatarUrl = contacts.FirstOrDefault(f => f.ObjectId == cus.CustomerId).AvatarUrl,
                //                           CustomerEmail = contacts.FirstOrDefault(f => f.ObjectId == cus.CustomerId).Email,
                //                           PicName = employees.FirstOrDefault(emp => emp.EmployeeId == cus.PersonInChargeId) == null ? "" : employees.FirstOrDefault(emp => emp.EmployeeId == cus.PersonInChargeId).EmployeeName,
                //                           CustomerPhone = contacts.FirstOrDefault(f => f.ObjectId == cus.CustomerId).Phone,
                //                           TotalSaleValue = or.DiscountType == true ? or.Amount - or.Amount * or.DiscountValue : or.Amount - or.DiscountValue,
                //                           CreatedDate = or.CreatedDate
                //                       }).OrderBy(o => o.CreatedDate.Date).ToList();

                #endregion

                var _listCusFollowProduct = (from orderDetail in customerOrderDetails
                                             join order in customerOrders on orderDetail.OrderId equals order.OrderId
                                             join product in products on orderDetail.ProductId equals product.ProductId
                                             join stt in orderStatus on order.StatusId equals stt.OrderStatusId
                                             join proCate in productCategorys on product.ProductCategoryId equals proCate.ProductCategoryId
                                             where
                                                  lstOrderStt.Contains(stt.OrderStatusCode)
                                             select new ProductCategoryEntityModel()
                                             {
                                                 ProductCategoryId = proCate.ProductCategoryId
                                             }).OrderByDescending(code => code.ProductCategoryId).ToList();
                _listCusFollowProduct.ForEach(item =>
                {
                    item.ProductCategoryName = GetProductCategoryParent(item.ProductCategoryId, productCategorys).ProductCategoryName;
                    item.ProductCategoryCode = GetProductCategoryParent(item.ProductCategoryId, productCategorys).ProductCategoryCode;
                });
                var _listTopPic = (from cus in customers
                                   where cus.PersonInChargeId != null
                                   join emp in employees on cus.PersonInChargeId equals emp.EmployeeId
                                   where emp != null &&
                                        (
                                            (currentEmp.IsManager && (emp.OrganizationId == orgRoot.OrganizationId || listOrg.Contains(emp.OrganizationId.Value))) ||
                                            (!currentEmp.IsManager && emp.EmployeeId == currentEmp.EmployeeId)
                                        )
                                   select new CustomerEntityModel()
                                   {
                                       PicName = emp.EmployeeName,
                                       PersonInChargeId = emp.EmployeeId
                                   }).OrderByDescending(id => id.PersonInChargeId).ToList();
                var _listCusCreatedInThisYear = (from cus in customers
                                                 where cus.CreatedDate.Year == DateTime.Now.Year
                                                 select new CustomerEntityModel()
                                                 {
                                                     CreatedDate = cus.CreatedDate,
                                                     CustomerName = cus.CustomerName,
                                                     MaximumDebtDays = cus.CreatedDate.Month
                                                 }).OrderBy(c => c.CreatedDate.Month).ToList();

                return new GetStatisticCustomerForDashboardResult()
                {
                    MessageCode = "Success",
                    StatusCode = System.Net.HttpStatusCode.OK,
                    ListCusCreatedInThisYear = _listCusCreatedInThisYear,
                    ListCusFollowProduct = _listCusFollowProduct,
                    ListCusIsNewBought = _listCusIsNewBought,
                    ListCusIsNewest = _listCusIsNewest,
                    //ListCusSaleTop = _listCusSaleTop,
                    ListTopPic = _listTopPic
                };
            }
            catch (Exception ex)
            {
                return new GetStatisticCustomerForDashboardResult
                {
                    MessageCode = ex.Message,
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed
                };
            }

        }
        public GetListCustomeSaleToprForDashboardResult GetListCustomeSaleToprForDashboard(GetListCustomeSaleToprForDashboardParameter parameter)
        {
            try
            {
                var employees = context.Employee.ToList();
                var organizations = context.Organization.ToList();
                var contacts = context.Contact.Where(w => w.ObjectType == ObjectType.CUSTOMER).ToList();
                var users = context.User.ToList();
                var customers = context.Customer.Where(w => w.Active == true).ToList();
                var orderStatus = context.OrderStatus.ToList();
                var customerOrders = context.CustomerOrder.ToList();

                var currentEmp = employees.FirstOrDefault(emp => emp.EmployeeId == users.FirstOrDefault(u => u.UserId == parameter.UserId).EmployeeId);
                var orgRoot = organizations.FirstOrDefault(o => o.OrganizationId == currentEmp.OrganizationId);
                var listOrg = ListChildOfParamToSearch(orgRoot.OrganizationId, organizations);
                var lstOrderStt = new List<string>() { "IP", "COMP", "DLV", "PD" };
                var _listCusSaleTop = (from or in customerOrders
                                       join cus in customers on or.CustomerId equals cus.CustomerId
                                       //join con in context.Contact on or.CustomerId equals con.ObjectId
                                       join stt in orderStatus on or.StatusId equals stt.OrderStatusId
                                       join userCre in users on cus.CreatedById equals userCre.UserId
                                       join empCre in employees on userCre.EmployeeId equals empCre.EmployeeId
                                       //join empPic in context.Employee on cus.PersonInChargeId equals empPic.EmployeeId
                                       where
                                                  lstOrderStt.Contains(stt.OrderStatusCode) &&
                                                  //con.ObjectType == ObjectType.CUSTOMER &&
                                                  cus.CustomerName.ToLower().Contains(parameter.KeyName.ToLower())
                                                  &&
                                                  (
                                                    (!currentEmp.IsManager && (cus.CreatedById == parameter.UserId || (cus.PersonInChargeId != null && cus.PersonInChargeId.Value == currentEmp.EmployeeId))) ||
                                                    (currentEmp.IsManager &&
                                                        (empCre.OrganizationId.Value == orgRoot.OrganizationId ||
                                                         listOrg.Contains(empCre.OrganizationId.Value) ||
                                                         (cus.PersonInChargeId != null &&
                                                          (
                                                           employees.FirstOrDefault(emp => emp.EmployeeId == cus.PersonInChargeId).OrganizationId.Value == orgRoot.OrganizationId ||
                                                           listOrg.Contains(employees.FirstOrDefault(emp => emp.EmployeeId == cus.PersonInChargeId).OrganizationId.Value)
                                                          )
                                                         )
                                                        )
                                                    )
                                                  )
                                                  &&
                                                  //(or.CreatedDate.Month == parameter.Month)
                                                  (or.OrderDate.Month == parameter.Month)
                                                  &&
                                                  //(or.CreatedDate.Year == parameter.Year)
                                                  (or.OrderDate.Year == parameter.Year)
                                       select new CustomerEntityModel()
                                       {
                                           CustomerName = cus.CustomerName,
                                           CustomerCode = cus.CustomerCode,
                                           CustomerId = cus.CustomerId,
                                           CusAvatarUrl = contacts.FirstOrDefault(f => f.ObjectId == cus.CustomerId).AvatarUrl,
                                           CustomerEmail = contacts.FirstOrDefault(f => f.ObjectId == cus.CustomerId).Email,
                                           PicName = employees.FirstOrDefault(emp => emp.EmployeeId == cus.PersonInChargeId) == null ? "" : employees.FirstOrDefault(emp => emp.EmployeeId == cus.PersonInChargeId).EmployeeName,
                                           CustomerPhone = contacts.FirstOrDefault(f => f.ObjectId == cus.CustomerId).Phone,
                                           TotalSaleValue = (or.DiscountType == true) ? (or.Amount - or.Amount * or.DiscountValue / 100) : (or.Amount - or.DiscountValue),
                                           CreatedDate = or.CreatedDate
                                       }).OrderBy(o => o.CreatedDate.Date).ToList();

                return new GetListCustomeSaleToprForDashboardResult()
                {
                    MessageCode = "Success",
                    StatusCode = System.Net.HttpStatusCode.OK,
                    ListCusSaleTop = _listCusSaleTop,
                };
            }
            catch (Exception ex)
            {
                return new GetListCustomeSaleToprForDashboardResult
                {
                    MessageCode = ex.Message,
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed
                };
            }
        }

        private List<Guid> ListChildOfParamToSearch(Guid orgId, List<Organization> organizations)
        {
            //var orgParam = context.Organization.FirstOrDefault(org => org.OrganizationId == orgId);
            var _listOrgIdChild = organizations.Where(o => o.ParentId == orgId).Select(id => id.OrganizationId).ToList();
            var _tmpOrgId = new List<Guid>();
            _listOrgIdChild.ForEach(_orgId =>
            {
                _tmpOrgId.Add(_orgId);
                ListChildOfParamToSearch(_orgId, organizations).ForEach(child =>
                {
                    _tmpOrgId.Add(child);
                });
            });
            return _tmpOrgId;
        }

        private ProductCategory GetProductCategoryParent(Guid proCateId, List<ProductCategory> productCategorys)
        {
            var target = productCategorys.FirstOrDefault(p => p.ProductCategoryId == proCateId);
            while (target.ParentId != null)
            {
                target = productCategorys.FirstOrDefault(p => p.ProductCategoryId == target.ParentId);
            }
            return target;

        }

        public CheckDuplicateCustomerResult CheckDuplicateCustomerPhoneOrEmail(CheckDuplicateCustomerLeadParameter parameter)
        {
            try
            {
                Contact contact = null;
                //CustomerOrderDAO obj = new CustomerOrderDAO(this.context, this.iAuditTrace, this.hostingEnvironment);
                bool isDuplicate = false;
                if (!string.IsNullOrEmpty(parameter.Email))
                {
                    contact = context.Contact.Where(w => w.Email.ToLower() == parameter.Email.Trim().ToLower() && w.ObjectType == ObjectType.CUSTOMER).FirstOrDefault();
                    if (contact != null)
                    {
                        var customer = context.Customer.FirstOrDefault(cus => cus.CustomerId == contact.ObjectId);
                        if (customer != null)
                        {
                            isDuplicate = customer.Active == true ? true : false;
                            contact = null;
                        }
                    }
                }

                if (!string.IsNullOrEmpty(parameter.Phone) && contact == null)
                {
                    contact = context.Contact.Where(w => w.Phone.ToLower() == parameter.Phone.Trim().ToLower() && w.ObjectType == ObjectType.CUSTOMER).FirstOrDefault();
                    if (contact != null)
                    {
                        var customer = context.Customer.FirstOrDefault(cus => cus.CustomerId == contact.ObjectId);
                        if (customer != null)
                        {
                            isDuplicate = customer.Active == true ? true : false;
                            contact = null;
                        }
                    }
                }

                if (parameter.LeadId != null && contact == null)
                {
                    isDuplicate = false;
                    contact = context.Contact.FirstOrDefault(f => f.ObjectId == parameter.LeadId && f.ObjectType == ObjectType.LEAD);
                    if (contact != null)
                    {
                        #region Update Lead thành ký hợp đồng và tạo 1 khách hàng mới
                        var lead = context.Lead.FirstOrDefault(f => f.LeadId == parameter.LeadId);
                        var lead_contact = context.Contact.FirstOrDefault(f => f.ObjectId == lead.LeadId && f.ObjectType == "LEA");
                        if (lead != null && lead_contact != null)
                        {
                            var categoryTypeStatusLead = context.CategoryType.FirstOrDefault(f => f.CategoryTypeCode == "TLE"); //trạng thái lead
                            var categoryTypStatusCustomer = context.CategoryType.FirstOrDefault(f => f.CategoryTypeCode == "THA"); //trạng thái khách hàng     

                            var statusLead = context.Category.FirstOrDefault(f => f.CategoryTypeId == categoryTypeStatusLead.CategoryTypeId && f.CategoryCode == "KHD");
                            if (statusLead != null)
                            {
                                lead.StatusId = statusLead.CategoryId;
                                context.Lead.Update(lead);
                            }

                            var newcustomer = new Customer();
                            newcustomer.CustomerId = Guid.NewGuid();
                            newcustomer.CustomerCode = this.GenerateCustomerCode(0);

                            if (lead.LeadGroupId != null)
                            {
                                newcustomer.CustomerGroupId = lead.LeadGroupId;
                            }
                            else
                            {
                                //nếu không có lead group id thì set theo giá trị mặc định
                                var groupCodeType = context.CategoryType.FirstOrDefault(f => f.CategoryTypeCode == "NHA").CategoryTypeId;
                                var defaultGroupCode = context.Category.FirstOrDefault(f => f.CategoryTypeId == groupCodeType && f.IsDefauld == true) ?? context.Category.FirstOrDefault(f => f.CategoryTypeId == groupCodeType);
                                newcustomer.CustomerGroupId = defaultGroupCode.CategoryId;
                            }

                            newcustomer.CustomerName = lead_contact.FirstName + " " + lead_contact.LastName;
                            newcustomer.LeadId = lead.LeadId;
                            newcustomer.StatusId = context.Category.FirstOrDefault(f => f.CategoryTypeId == categoryTypStatusCustomer.CategoryTypeId && f.CategoryCode == "MOI").CategoryId;
                            newcustomer.PersonInChargeId = lead.PersonInChargeId;
                            newcustomer.PaymentId = lead.PaymentMethodId;
                            //chuyển loại lead => loại khách hàng
                            var leadTypeId = context.Category.FirstOrDefault(f => f.CategoryId == lead.LeadTypeId);
                            if (leadTypeId == null)
                            {
                                newcustomer.CustomerType = 2;
                            }
                            else
                            {
                                switch (leadTypeId.CategoryCode)
                                {
                                    case "KPL"://Khách hàng cá nhân
                                        newcustomer.CustomerType = 2;
                                        break;
                                    case "KCL"://Khách hàng doanh nghiệp
                                        newcustomer.CustomerType = 1;
                                        break;
                                    case "KHDL"://Khách hàng đại lý
                                        newcustomer.CustomerType = 3;
                                        break;
                                    default:
                                        newcustomer.CustomerType = 2;
                                        break;
                                }
                            }
                            newcustomer.Active = true;
                            newcustomer.CreatedDate = DateTime.Now;
                            newcustomer.CreatedById = parameter.UserId;
                            newcustomer.UpdatedById = null;
                            newcustomer.UpdatedDate = null;

                            context.Customer.Add(newcustomer);

                            var newCustomerContact = new Contact();
                            newCustomerContact = lead_contact;
                            newCustomerContact.ContactId = Guid.NewGuid();
                            newCustomerContact.ObjectId = newcustomer.CustomerId;
                            newCustomerContact.ObjectType = "CUS";
                            newCustomerContact.FirstName = newcustomer.CustomerName;
                            newCustomerContact.LastName = "";

                            //công ty khách hàng = công ty lead (nếu có)
                            if (lead.CompanyId != null)
                            {
                                var companyName = context.Company.FirstOrDefault(f => f.CompanyId == lead.CompanyId)?.CompanyName ?? "";
                                newCustomerContact.CompanyName = companyName;
                            }

                            context.Contact.Add(newCustomerContact);
                        }
                        #endregion
                    }
                    context.SaveChanges();
                }

                if (parameter.IsUpdateLead)
                {
                    // Edit by Ngoc
                    //var customer = context.Customer.FirstOrDefault(f => f.CustomerId == contact.ObjectId);
                    var customer = context.Customer.FirstOrDefault(f => f.CustomerId == contact.ObjectId && f.Active == true);
                    if (customer.LeadId == null || customer.LeadId == Guid.Empty)
                    {
                        customer.LeadId = parameter.LeadId;
                        context.Customer.Update(customer);
                        context.SaveChanges();
                    }
                    //obj.UpdateStatusLead(parameter.LeadId);
                }

                return new CheckDuplicateCustomerResult
                {
                    IsDuplicate = isDuplicate,
                    ContactId = contact != null ? contact.ContactId : Guid.Empty,
                    CustomerId = contact != null ? contact.ObjectId : Guid.Empty,
                    StatusCode = System.Net.HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new CheckDuplicateCustomerResult
                {
                    MessageCode = ex.Message,
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed
                };
            }

        }

        public CheckDuplicateCustomerResult CheckDuplicateCustomer(CheckDuplicateCustomerParameter parameter)
        {
            try
            {

                Contact contactEntityCheckEmailDuplicateByCustomer = null;
                Contact contactEntityCheckPhoneDuplicateByCustomer = null;
                bool IsDuplicate = false;
                var categoryKHDId = context.Category.Where(w => w.CategoryCode == "KHD").FirstOrDefault().CategoryId;
                var categoryNDO = context.Category.Where(w => w.CategoryCode == "NDO").FirstOrDefault().CategoryId;
                dynamic entityCheckEmailLead = null;
                dynamic entityCheckPhoneLead = null;

                if (parameter.CheckByEmail)
                {
                    if (!string.IsNullOrEmpty(parameter.Email))
                    {
                        if (parameter.LeadId == null)
                        {
                            entityCheckEmailLead = (from l in context.Lead
                                                    join ct in context.Contact on l.LeadId equals ct.ObjectId
                                                    where (l.StatusId != categoryKHDId && l.StatusId != categoryNDO && ct.Email.ToLower() == parameter.Email.Trim().ToLower())
                                                    select new
                                                    {
                                                        LeadId = l.LeadId
                                                    }).FirstOrDefault();
                        }
                        else
                        {
                            entityCheckEmailLead = (from l in context.Lead
                                                    join ct in context.Contact on l.LeadId equals ct.ObjectId
                                                    where (l.StatusId != categoryKHDId && l.StatusId != categoryNDO && l.LeadId != parameter.LeadId && ct.Email.ToLower() == parameter.Email.Trim().ToLower())
                                                    select new
                                                    {
                                                        LeadId = l.LeadId
                                                    }).FirstOrDefault();
                        }
                        contactEntityCheckEmailDuplicateByCustomer = context.Contact.Where(w => w.ObjectType == "CUS" && w.Email.ToLower() == parameter.Email.Trim().ToLower()).FirstOrDefault();
                    }
                }

                if (parameter.CheckByPhone)
                {
                    if (!string.IsNullOrEmpty(parameter.Phone))
                    {
                        if (parameter.LeadId == null)
                        {
                            entityCheckPhoneLead = (from l in context.Lead
                                                    join ct in context.Contact on l.LeadId equals ct.ObjectId
                                                    where (l.StatusId != categoryKHDId && l.StatusId != categoryNDO && ct.Phone.ToLower() == parameter.Phone.Trim().ToLower())
                                                    select new
                                                    {
                                                        LeadId = l.LeadId
                                                    }).FirstOrDefault();
                        }
                        else
                        {
                            entityCheckPhoneLead = (from l in context.Lead
                                                    join ct in context.Contact on l.LeadId equals ct.ObjectId
                                                    where (l.StatusId != categoryKHDId && l.StatusId != categoryNDO && l.LeadId != parameter.LeadId && ct.Phone.ToLower() == parameter.Phone.Trim().ToLower())
                                                    select new
                                                    {
                                                        LeadId = l.LeadId
                                                    }).FirstOrDefault();
                        }
                        contactEntityCheckPhoneDuplicateByCustomer = context.Contact.Where(w => w.ObjectType == "CUS" && w.Phone.ToLower() == parameter.Phone.Trim().ToLower()).FirstOrDefault();
                    }
                }

                Guid LeadId = Guid.Empty;
                if (entityCheckEmailLead != null)
                {
                    LeadId = entityCheckEmailLead.LeadId;
                }
                else if (entityCheckPhoneLead != null)
                {
                    LeadId = entityCheckPhoneLead.LeadId;
                }

                Guid CustomerId = Guid.Empty;
                Guid ContactId = Guid.Empty;
                if (contactEntityCheckEmailDuplicateByCustomer != null)
                {
                    CustomerId = contactEntityCheckEmailDuplicateByCustomer.ObjectId;
                    ContactId = contactEntityCheckEmailDuplicateByCustomer.ContactId;
                }
                else if (contactEntityCheckPhoneDuplicateByCustomer != null)
                {
                    CustomerId = contactEntityCheckPhoneDuplicateByCustomer.ObjectId;
                    ContactId = contactEntityCheckPhoneDuplicateByCustomer.ContactId;
                }
                return new CheckDuplicateCustomerResult
                {
                    IsDuplicate = IsDuplicate,
                    IsDuplicateByEmailLead = entityCheckEmailLead != null ? true : false,
                    IsDuplicateByPhoneLead = entityCheckPhoneLead != null ? true : false,
                    IsDuplicateByEmailCustomer = contactEntityCheckEmailDuplicateByCustomer != null ? true : false,
                    IsDuplicateByPhoneCustomer = contactEntityCheckPhoneDuplicateByCustomer != null ? true : false,
                    LeadId = LeadId,
                    CustomerId = CustomerId,
                    ContactId = ContactId,
                    StatusCode = System.Net.HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new CheckDuplicateCustomerResult
                {
                    MessageCode = ex.Message,
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed
                };
            }
        }

        public CheckDuplicatePersonalCustomerResult CheckDuplicatePersonalCustomer(CheckDuplicatePersonalCustomerParameter parameter)
        {
            try
            {
                var categoryKHDId = context.Category.Where(w => w.CategoryCode == "KHD").FirstOrDefault().CategoryId;
                var categoryNDO = context.Category.Where(w => w.CategoryCode == "NDO").FirstOrDefault().CategoryId;
                dynamic entityCheckDuplicatePhoneLeadByPhone = null;
                dynamic entityCheckDuplicatePhoneLeadByWorkPhone = null;
                dynamic entityCheckDuplicatePhoneLeadByOtherPhone = null;
                dynamic entityCheckDuplicateEmailLeadByEmail = null;
                dynamic entityCheckDuplicateEmailLeadByWorkEmail = null;
                dynamic entityCheckDuplicateEmailLeadByOtherEmail = null;
                //Contact contactEntityCheckDuplicatePhoneLeadByPhone = null;
                //Contact contactEntityCheckDuplicatePhoneLeadByWorkPhone = null;
                //Contact contactEntityCheckDuplicatePhoneLeadByOtherPhone = null;
                //Contact contactEntityCheckDuplicateEmailLeadByEmail = null;
                //Contact contactEntityCheckDuplicateEmailLeadByWorkEmail = null;
                //Contact contactEntityCheckDuplicateEmailLeadByOtherEmail = null;
                Contact contactEntityCheckDuplicatePhoneCustomer = null;
                Contact contactEntityCheckDuplicateWorkPhoneCustomer = null;
                Contact contactEntityCheckDuplicateOtherPhoneCustomer = null;
                Contact contactEntityCheckDuplicateEmailCustomer = null;
                Contact contactEntityCheckDuplicateWorkEmailCustomer = null;
                Contact contactEntityCheckDuplicateOtherEmailCustomer = null;

                bool IsDuplicateCustomerByPhone = false;
                bool IsDuplicateCustomerByEmail = false;
                bool IsDuplicateLead = false;

                if (!string.IsNullOrEmpty(parameter.Contact.Phone))
                {
                    if (parameter.Customer.LeadId == null)
                    {
                        //contactEntityCheckDuplicatePhoneLeadByPhone = context.Contact.Where(w => w.ObjectType == "LEA" && w.Phone.Trim() == parameter.Contact.Phone.ToLower().Trim()).FirstOrDefault();
                        entityCheckDuplicatePhoneLeadByPhone = (from l in context.Lead
                                                                join ct in context.Contact on l.LeadId equals ct.ObjectId
                                                                where (l.StatusId != categoryKHDId && l.StatusId != categoryNDO && ct.Phone.Trim() == parameter.Contact.Phone.ToLower().Trim())
                                                                select new
                                                                {
                                                                    LeadId = l.LeadId
                                                                }).FirstOrDefault();
                    }
                    else
                    {
                        //contactEntityCheckDuplicatePhoneLeadByPhone= context.Contact.Where(w => w.ObjectType == "LEA" && w.ObjectId != parameter.Customer.LeadId && w.Phone.Trim() == parameter.Contact.Phone.ToLower().Trim()).FirstOrDefault();
                        entityCheckDuplicatePhoneLeadByPhone = (from l in context.Lead
                                                                join ct in context.Contact on l.LeadId equals ct.ObjectId
                                                                where (l.StatusId != categoryKHDId && l.StatusId != categoryNDO && ct.ObjectId != parameter.Customer.LeadId && ct.Phone.Trim() == parameter.Contact.Phone.ToLower().Trim())
                                                                select new
                                                                {
                                                                    LeadId = l.LeadId
                                                                }).FirstOrDefault();
                    }
                    contactEntityCheckDuplicatePhoneCustomer = context.Contact.Where(w => w.ObjectType == "CUS" && (w.Phone.Trim() == parameter.Contact.Phone.ToLower().Trim())).FirstOrDefault();
                }
                if (!string.IsNullOrEmpty(parameter.Contact.WorkPhone))
                {
                    if (parameter.Customer.LeadId == null)
                    {
                        //contactEntityCheckDuplicatePhoneLeadByWorkPhone = context.Contact.Where(w => w.ObjectType == "LEA" && w.Phone.Trim() == parameter.Contact.WorkPhone.ToLower().Trim()).FirstOrDefault();
                        entityCheckDuplicatePhoneLeadByWorkPhone = (from l in context.Lead
                                                                    join ct in context.Contact on l.LeadId equals ct.ObjectId
                                                                    where (l.StatusId != categoryKHDId && l.StatusId != categoryNDO && ct.Phone.Trim() == parameter.Contact.WorkPhone.ToLower().Trim())
                                                                    select new
                                                                    {
                                                                        LeadId = l.LeadId
                                                                    }).FirstOrDefault();
                    }
                    else
                    {
                        //contactEntityCheckDuplicatePhoneLeadByWorkPhone = context.Contact.Where(w => w.ObjectType == "LEA" && w.ObjectId != parameter.Customer.LeadId && w.Phone.Trim() == parameter.Contact.WorkPhone.ToLower().Trim()).FirstOrDefault();
                        entityCheckDuplicatePhoneLeadByWorkPhone = (from l in context.Lead
                                                                    join ct in context.Contact on l.LeadId equals ct.ObjectId
                                                                    where (l.StatusId != categoryKHDId && l.StatusId != categoryNDO && ct.ObjectId != parameter.Customer.LeadId && ct.Phone.Trim() == parameter.Contact.WorkPhone.ToLower().Trim())
                                                                    select new
                                                                    {
                                                                        LeadId = l.LeadId
                                                                    }).FirstOrDefault();
                    }
                    contactEntityCheckDuplicateWorkPhoneCustomer = context.Contact.Where(w => w.ObjectType == "CUS" && w.WorkPhone.Trim() == parameter.Contact.WorkPhone.ToLower().Trim()).FirstOrDefault();
                }
                if (!string.IsNullOrEmpty(parameter.Contact.OtherPhone))
                {
                    if (parameter.Customer.LeadId == null)
                    {
                        //contactEntityCheckDuplicatePhoneLeadByOtherPhone = context.Contact.Where(w => w.ObjectType == "LEA" && w.Phone.Trim() == parameter.Contact.OtherPhone.ToLower().Trim()).FirstOrDefault();
                        entityCheckDuplicatePhoneLeadByOtherPhone = (from l in context.Lead
                                                                     join ct in context.Contact on l.LeadId equals ct.ObjectId
                                                                     where (l.StatusId != categoryKHDId && l.StatusId != categoryNDO && ct.Phone.Trim() == parameter.Contact.OtherPhone.ToLower().Trim())
                                                                     select new
                                                                     {
                                                                         LeadId = l.LeadId
                                                                     }).FirstOrDefault();
                    }
                    else
                    {
                        //contactEntityCheckDuplicatePhoneLeadByOtherPhone = context.Contact.Where(w => w.ObjectType == "LEA" && w.ObjectId != parameter.Customer.LeadId && w.Phone.Trim() == parameter.Contact.OtherPhone.ToLower().Trim()).FirstOrDefault();
                        entityCheckDuplicatePhoneLeadByOtherPhone = (from l in context.Lead
                                                                     join ct in context.Contact on l.LeadId equals ct.ObjectId
                                                                     where (l.StatusId != categoryKHDId && l.StatusId != categoryNDO && ct.ObjectId != parameter.Customer.LeadId && ct.Phone.Trim() == parameter.Contact.OtherPhone.ToLower().Trim())
                                                                     select new
                                                                     {
                                                                         LeadId = l.LeadId
                                                                     }).FirstOrDefault();
                    }
                    contactEntityCheckDuplicateOtherPhoneCustomer = context.Contact.Where(w => w.ObjectType == "CUS" && w.OtherPhone.Trim() == parameter.Contact.OtherPhone.ToLower().Trim()).FirstOrDefault();
                }
                if (!string.IsNullOrEmpty(parameter.Contact.Email))
                {
                    if (parameter.Customer.LeadId == null)
                    {
                        //contactEntityCheckDuplicateEmailLeadByEmail = context.Contact.Where(w => w.ObjectType == "LEA" && w.Email.Trim() == parameter.Contact.Email.ToLower().Trim()).FirstOrDefault();
                        entityCheckDuplicateEmailLeadByEmail = (from l in context.Lead
                                                                join ct in context.Contact on l.LeadId equals ct.ObjectId
                                                                where (l.StatusId != categoryKHDId && l.StatusId != categoryNDO && ct.Email.Trim() == parameter.Contact.Email.ToLower().Trim())
                                                                select new
                                                                {
                                                                    LeadId = l.LeadId
                                                                }).FirstOrDefault();
                    }
                    else
                    {
                        //contactEntityCheckDuplicateEmailLeadByEmail = context.Contact.Where(w => w.ObjectType == "LEA" && w.ObjectId != parameter.Customer.LeadId && w.Email.Trim() == parameter.Contact.Email.ToLower().Trim()).FirstOrDefault();
                        entityCheckDuplicateEmailLeadByEmail = (from l in context.Lead
                                                                join ct in context.Contact on l.LeadId equals ct.ObjectId
                                                                where (l.StatusId != categoryKHDId && l.StatusId != categoryNDO && ct.ObjectId != parameter.Customer.LeadId && ct.Email.Trim() == parameter.Contact.Email.ToLower().Trim())
                                                                select new
                                                                {
                                                                    LeadId = l.LeadId
                                                                }).FirstOrDefault();
                    }
                    contactEntityCheckDuplicateEmailCustomer = context.Contact.Where(w => w.ObjectType == "CUS" && w.Email.Trim() == parameter.Contact.Email.ToLower().Trim()).FirstOrDefault();
                }
                if (!string.IsNullOrEmpty(parameter.Contact.WorkEmail))
                {
                    if (parameter.Customer.LeadId == null)
                    {
                        //contactEntityCheckDuplicateEmailLeadByWorkEmail = context.Contact.Where(w => w.ObjectType == "LEA" && w.Email.Trim() == parameter.Contact.WorkEmail.ToLower().Trim()).FirstOrDefault();
                        entityCheckDuplicateEmailLeadByWorkEmail = (from l in context.Lead
                                                                    join ct in context.Contact on l.LeadId equals ct.ObjectId
                                                                    where (l.StatusId != categoryKHDId && l.StatusId != categoryNDO && ct.Email.Trim() == parameter.Contact.WorkEmail.ToLower().Trim())
                                                                    select new
                                                                    {
                                                                        LeadId = l.LeadId
                                                                    }).FirstOrDefault();
                    }
                    else
                    {
                        //contactEntityCheckDuplicateEmailLeadByWorkEmail = context.Contact.Where(w => w.ObjectType == "LEA" &&w.ObjectId != parameter.Customer.LeadId && w.Email.Trim() == parameter.Contact.WorkEmail.ToLower().Trim()).FirstOrDefault();
                        entityCheckDuplicateEmailLeadByWorkEmail = (from l in context.Lead
                                                                    join ct in context.Contact on l.LeadId equals ct.ObjectId
                                                                    where (l.StatusId != categoryKHDId && l.StatusId != categoryNDO && ct.ObjectId != parameter.Customer.LeadId && ct.Email.Trim() == parameter.Contact.WorkEmail.ToLower().Trim())
                                                                    select new
                                                                    {
                                                                        LeadId = l.LeadId
                                                                    }).FirstOrDefault();
                    }
                    contactEntityCheckDuplicateWorkEmailCustomer = context.Contact.Where(w => w.ObjectType == "CUS" && w.WorkEmail.Trim() == parameter.Contact.WorkEmail.ToLower().Trim()).FirstOrDefault();
                }
                if (!string.IsNullOrEmpty(parameter.Contact.OtherEmail))
                {
                    if (parameter.Customer.LeadId == null)
                    {
                        //contactEntityCheckDuplicateEmailLeadByOtherEmail = context.Contact.Where(w => w.ObjectType == "LEA" && w.Email.Trim() == parameter.Contact.OtherEmail.ToLower().Trim()).FirstOrDefault();
                        entityCheckDuplicateEmailLeadByOtherEmail = (from l in context.Lead
                                                                     join ct in context.Contact on l.LeadId equals ct.ObjectId
                                                                     where (l.StatusId != categoryKHDId && l.StatusId != categoryNDO && ct.Email.Trim() == parameter.Contact.OtherEmail.ToLower().Trim())
                                                                     select new
                                                                     {
                                                                         LeadId = l.LeadId
                                                                     }).FirstOrDefault();
                    }
                    else
                    {
                        //contactEntityCheckDuplicateEmailLeadByOtherEmail = context.Contact.Where(w => w.ObjectType == "LEA" && w.ObjectId != parameter.Customer.LeadId && w.Email.Trim() == parameter.Contact.OtherEmail.ToLower().Trim()).FirstOrDefault();
                        entityCheckDuplicateEmailLeadByOtherEmail = (from l in context.Lead
                                                                     join ct in context.Contact on l.LeadId equals ct.ObjectId
                                                                     where (l.StatusId != categoryKHDId && l.StatusId != categoryNDO && ct.ObjectId != parameter.Customer.LeadId && ct.Email.Trim() == parameter.Contact.OtherEmail.ToLower().Trim())
                                                                     select new
                                                                     {
                                                                         LeadId = l.LeadId
                                                                     }).FirstOrDefault();
                    }
                    contactEntityCheckDuplicateOtherEmailCustomer = context.Contact.Where(w => w.ObjectType == "CUS" && w.OtherEmail.Trim() == parameter.Contact.OtherEmail.ToLower().Trim()).FirstOrDefault();
                }

                if (entityCheckDuplicatePhoneLeadByPhone != null || entityCheckDuplicatePhoneLeadByWorkPhone != null || entityCheckDuplicatePhoneLeadByOtherPhone != null || entityCheckDuplicateEmailLeadByEmail != null || entityCheckDuplicateEmailLeadByWorkEmail != null || entityCheckDuplicateEmailLeadByOtherEmail != null)
                {
                    IsDuplicateLead = true;
                }

                if (contactEntityCheckDuplicatePhoneCustomer != null || contactEntityCheckDuplicateWorkPhoneCustomer != null || contactEntityCheckDuplicateOtherPhoneCustomer != null)
                {
                    IsDuplicateCustomerByPhone = true;
                }

                if (contactEntityCheckDuplicateEmailCustomer != null || contactEntityCheckDuplicateWorkEmailCustomer != null || contactEntityCheckDuplicateOtherEmailCustomer != null)
                {
                    IsDuplicateCustomerByEmail = true;
                }

                Contact customer = null;

                if (contactEntityCheckDuplicatePhoneCustomer != null)
                {
                    customer = contactEntityCheckDuplicatePhoneCustomer;
                }
                else if (contactEntityCheckDuplicateWorkPhoneCustomer != null)
                {
                    customer = contactEntityCheckDuplicateWorkPhoneCustomer;
                }
                else if (contactEntityCheckDuplicateOtherPhoneCustomer != null)
                {
                    customer = contactEntityCheckDuplicateOtherPhoneCustomer;
                }
                else if (contactEntityCheckDuplicateEmailCustomer != null)
                {
                    customer = contactEntityCheckDuplicateEmailCustomer;
                }
                else if (contactEntityCheckDuplicateWorkEmailCustomer != null)
                {
                    customer = contactEntityCheckDuplicateWorkEmailCustomer;
                }
                else if (contactEntityCheckDuplicateOtherEmailCustomer != null)
                {
                    customer = contactEntityCheckDuplicateOtherEmailCustomer;
                }

                Guid LeadId = Guid.Empty;

                if (entityCheckDuplicatePhoneLeadByPhone != null)
                {
                    LeadId = entityCheckDuplicatePhoneLeadByPhone.LeadId;
                }
                else if (entityCheckDuplicatePhoneLeadByWorkPhone != null)
                {
                    LeadId = entityCheckDuplicatePhoneLeadByPhone.LeadId;
                }
                else if (entityCheckDuplicatePhoneLeadByOtherPhone != null)
                {
                    LeadId = entityCheckDuplicatePhoneLeadByOtherPhone.LeadId;
                }
                else if (entityCheckDuplicateEmailLeadByEmail != null)
                {
                    LeadId = entityCheckDuplicateEmailLeadByEmail.LeadId;
                }
                else if (entityCheckDuplicateEmailLeadByWorkEmail != null)
                {
                    LeadId = entityCheckDuplicateEmailLeadByWorkEmail.LeadId;
                }
                else if (entityCheckDuplicateEmailLeadByOtherEmail != null)
                {
                    LeadId = entityCheckDuplicateEmailLeadByOtherEmail.LeadId;
                }
                return new CheckDuplicatePersonalCustomerResult
                {
                    IsDuplicateCustomerByPhone = IsDuplicateCustomerByPhone,
                    IsDuplicateCustomerByEmail = IsDuplicateCustomerByEmail,
                    IsDuplicateLead = IsDuplicateLead,
                    LeadId = LeadId,
                    CustomerId = customer != null ? customer.ObjectId : Guid.Empty,
                    ContactId = customer != null ? customer.ContactId : Guid.Empty,
                    StatusCode = System.Net.HttpStatusCode.OK
                };
            }
            catch (Exception)
            {
                return new CheckDuplicatePersonalCustomerResult
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                };
            }
        }

        public CheckDuplicatePersonalCustomerByEmailOrPhoneResult CheckDuplicatePersonalCustomerByEmailOrPhone(CheckDuplicatePersonalCustomerByEmailOrPhoneParameter parameter)
        {
            try
            {
                var categoryKHDId = context.Category.Where(w => w.CategoryCode == "KHD").FirstOrDefault().CategoryId;
                var categoryNDO = context.Category.Where(w => w.CategoryCode == "NDO").FirstOrDefault().CategoryId;
                dynamic entityCheckDuplicatePhoneLead = null;
                dynamic entityCheckDuplicateEmailLead = null;
                //Contact contactEntityCheckDuplicatePhoneLead = null;
                //Contact contactEntityCheckDuplicateEmailLead = null;
                Contact contactEntityCheckDuplicatePhoneCustomer = null;
                Contact contactEntityCheckDuplicateWorkPhoneCustomer = null;
                Contact contactEntityCheckDuplicateOtherPhoneCustomer = null;
                Contact contactEntityCheckDuplicateEmailCustomer = null;
                Contact contactEntityCheckDuplicateWorkEmailCustomer = null;
                Contact contactEntityCheckDuplicateOtherEmailCustomer = null;
                bool IsDuplicateCustomer = false;
                bool IsDuplicateLead = false;

                if (parameter.CheckByEmail && !string.IsNullOrEmpty(parameter.Email))
                {
                    if (parameter.LeadId == null)
                    {
                        //contactEntityCheckDuplicateEmailLead = context.Contact.Where(w => w.ObjectType == "LEA" && w.Email.Trim() == parameter.Email.Trim().ToLower()).FirstOrDefault();
                        entityCheckDuplicateEmailLead = (from l in context.Lead
                                                         join ct in context.Contact on l.LeadId equals ct.ObjectId
                                                         where (l.StatusId != categoryKHDId && l.StatusId != categoryNDO && ct.Email.Trim() == parameter.Email.ToLower().Trim())
                                                         select new
                                                         {
                                                             LeadId = l.LeadId
                                                         }).FirstOrDefault();
                    }
                    else
                    {
                        //contactEntityCheckDuplicateEmailLead = context.Contact.Where(w => w.ObjectType == "LEA" && w.ObjectId != parameter.LeadId && w.Email.Trim() == parameter.Email.Trim().ToLower()).FirstOrDefault();
                        entityCheckDuplicateEmailLead = (from l in context.Lead
                                                         join ct in context.Contact on l.LeadId equals ct.ObjectId
                                                         where (l.StatusId != categoryKHDId && l.StatusId != categoryNDO && ct.ObjectId != parameter.LeadId && ct.Email.Trim() == parameter.Email.ToLower().Trim())
                                                         select new
                                                         {
                                                             LeadId = l.LeadId
                                                         }).FirstOrDefault();
                    }
                    contactEntityCheckDuplicateEmailCustomer = context.Contact.Where(w => w.ObjectType == "CUS" && w.Email.Trim() == parameter.Email.Trim().ToLower()).FirstOrDefault();
                }

                if (parameter.CheckByWorkEmail && !string.IsNullOrEmpty(parameter.Email))
                {
                    if (parameter.LeadId == null)
                    {
                        //contactEntityCheckDuplicateEmailLead = context.Contact.Where(w => w.ObjectType == "LEA" && w.Email.Trim() == parameter.Email.Trim().ToLower()).FirstOrDefault();
                        entityCheckDuplicateEmailLead = (from l in context.Lead
                                                         join ct in context.Contact on l.LeadId equals ct.ObjectId
                                                         where (l.StatusId != categoryKHDId && l.StatusId != categoryNDO && ct.Email.Trim() == parameter.Email.ToLower().Trim())
                                                         select new
                                                         {
                                                             LeadId = l.LeadId
                                                         }).FirstOrDefault();
                    }
                    else
                    {
                        //contactEntityCheckDuplicateEmailLead = context.Contact.Where(w => w.ObjectType == "LEA" && w.ObjectId != parameter.LeadId && w.Email.Trim() == parameter.Email.Trim().ToLower()).FirstOrDefault();
                        entityCheckDuplicateEmailLead = (from l in context.Lead
                                                         join ct in context.Contact on l.LeadId equals ct.ObjectId
                                                         where (l.StatusId != categoryKHDId && l.StatusId != categoryNDO && ct.ObjectId != parameter.LeadId && ct.Email.Trim() == parameter.Email.ToLower().Trim())
                                                         select new
                                                         {
                                                             LeadId = l.LeadId
                                                         }).FirstOrDefault();
                    }
                    contactEntityCheckDuplicateWorkEmailCustomer = context.Contact.Where(w => w.ObjectType == "CUS" && w.WorkEmail.Trim() == parameter.Email.Trim().ToLower()).FirstOrDefault();
                }

                if (parameter.CheckByOtherEmail && !string.IsNullOrEmpty(parameter.Email))
                {
                    if (parameter.LeadId == null)
                    {
                        //contactEntityCheckDuplicateEmailLead = context.Contact.Where(w => w.ObjectType == "LEA" && w.Email.Trim() == parameter.Email.Trim().ToLower()).FirstOrDefault();
                        entityCheckDuplicateEmailLead = (from l in context.Lead
                                                         join ct in context.Contact on l.LeadId equals ct.ObjectId
                                                         where (l.StatusId != categoryKHDId && l.StatusId != categoryNDO && ct.Email.Trim() == parameter.Email.ToLower().Trim())
                                                         select new
                                                         {
                                                             LeadId = l.LeadId
                                                         }).FirstOrDefault();
                    }
                    else
                    {
                        //contactEntityCheckDuplicateEmailLead = context.Contact.Where(w => w.ObjectType == "LEA" && w.ObjectId != parameter.LeadId && w.Email.Trim() == parameter.Email.Trim().ToLower()).FirstOrDefault();
                        entityCheckDuplicateEmailLead = (from l in context.Lead
                                                         join ct in context.Contact on l.LeadId equals ct.ObjectId
                                                         where (l.StatusId != categoryKHDId && l.StatusId != categoryNDO && ct.ObjectId != parameter.LeadId && ct.Email.Trim() == parameter.Email.ToLower().Trim())
                                                         select new
                                                         {
                                                             LeadId = l.LeadId
                                                         }).FirstOrDefault();
                    }
                    contactEntityCheckDuplicateOtherEmailCustomer = context.Contact.Where(w => w.ObjectType == "CUS" && w.OtherEmail == parameter.Email.Trim().ToLower()).FirstOrDefault();
                }

                if (parameter.CheckByPhone && !string.IsNullOrEmpty(parameter.Phone))
                {
                    if (parameter.LeadId == null)
                    {
                        //contactEntityCheckDuplicatePhoneLead = context.Contact.Where(w => w.ObjectType == "LEA" && w.Phone.Trim() == parameter.Phone.Trim().ToLower()).FirstOrDefault();
                        entityCheckDuplicatePhoneLead = (from l in context.Lead
                                                         join ct in context.Contact on l.LeadId equals ct.ObjectId
                                                         where (l.StatusId != categoryKHDId && l.StatusId != categoryNDO && ct.Phone.Trim() == parameter.Phone.ToLower().Trim())
                                                         select new
                                                         {
                                                             LeadId = l.LeadId
                                                         }).FirstOrDefault();
                    }
                    else
                    {
                        //contactEntityCheckDuplicatePhoneLead = context.Contact.Where(w => w.ObjectType == "LEA" && w.ObjectId != parameter.LeadId && w.Phone.Trim() == parameter.Phone.Trim().ToLower()).FirstOrDefault();
                        entityCheckDuplicatePhoneLead = (from l in context.Lead
                                                         join ct in context.Contact on l.LeadId equals ct.ObjectId
                                                         where (l.StatusId != categoryKHDId && l.StatusId != categoryNDO && ct.ObjectId != parameter.LeadId && ct.Phone.Trim() == parameter.Phone.ToLower().Trim())
                                                         select new
                                                         {
                                                             LeadId = l.LeadId
                                                         }).FirstOrDefault();
                    }
                    contactEntityCheckDuplicatePhoneCustomer = context.Contact.Where(w => w.ObjectType == "CUS" && w.Phone.Trim() == parameter.Phone.Trim().ToLower()).FirstOrDefault();
                }

                if (parameter.CheckByWorkPhone && !string.IsNullOrEmpty(parameter.Phone))
                {
                    if (parameter.LeadId == null)
                    {
                        //contactEntityCheckDuplicatePhoneLead = context.Contact.Where(w => w.ObjectType == "LEA" && w.Phone.Trim() == parameter.Phone.Trim().ToLower()).FirstOrDefault();
                        entityCheckDuplicatePhoneLead = (from l in context.Lead
                                                         join ct in context.Contact on l.LeadId equals ct.ObjectId
                                                         where (l.StatusId != categoryKHDId && l.StatusId != categoryNDO && ct.Phone.Trim() == parameter.Phone.ToLower().Trim())
                                                         select new
                                                         {
                                                             LeadId = l.LeadId
                                                         }).FirstOrDefault();
                    }
                    else
                    {
                        //contactEntityCheckDuplicatePhoneLead = context.Contact.Where(w => w.ObjectType == "LEA" && w.ObjectId != parameter.LeadId && w.Phone.Trim() == parameter.Phone.Trim().ToLower()).FirstOrDefault();
                        entityCheckDuplicatePhoneLead = (from l in context.Lead
                                                         join ct in context.Contact on l.LeadId equals ct.ObjectId
                                                         where (l.StatusId != categoryKHDId && l.StatusId != categoryNDO && ct.ObjectId != parameter.LeadId && ct.Phone.Trim() == parameter.Phone.ToLower().Trim())
                                                         select new
                                                         {
                                                             LeadId = l.LeadId
                                                         }).FirstOrDefault();
                    }
                    contactEntityCheckDuplicateWorkPhoneCustomer = context.Contact.Where(w => w.ObjectType == "CUS" && w.WorkPhone.Trim() == parameter.Phone.Trim().ToLower()).FirstOrDefault();
                }

                if (parameter.CheckByOtherPhone && !string.IsNullOrEmpty(parameter.Phone))
                {
                    if (parameter.LeadId == null)
                    {
                        //contactEntityCheckDuplicatePhoneLead = context.Contact.Where(w => w.ObjectType == "LEA" && w.Phone.Trim() == parameter.Phone.Trim().ToLower()).FirstOrDefault();
                        entityCheckDuplicatePhoneLead = (from l in context.Lead
                                                         join ct in context.Contact on l.LeadId equals ct.ObjectId
                                                         where (l.StatusId != categoryKHDId && l.StatusId != categoryNDO && ct.Phone.Trim() == parameter.Phone.ToLower().Trim())
                                                         select new
                                                         {
                                                             LeadId = l.LeadId
                                                         }).FirstOrDefault();
                    }
                    else
                    {
                        //contactEntityCheckDuplicatePhoneLead = context.Contact.Where(w => w.ObjectType == "LEA" && w.ObjectId != parameter.LeadId && w.Phone.Trim() == parameter.Phone.Trim().ToLower()).FirstOrDefault();
                        entityCheckDuplicatePhoneLead = (from l in context.Lead
                                                         join ct in context.Contact on l.LeadId equals ct.ObjectId
                                                         where (l.StatusId != categoryKHDId && l.StatusId != categoryNDO && ct.ObjectId != parameter.LeadId && ct.Phone.Trim() == parameter.Phone.ToLower().Trim())
                                                         select new
                                                         {
                                                             LeadId = l.LeadId
                                                         }).FirstOrDefault();
                    }
                    contactEntityCheckDuplicateOtherPhoneCustomer = context.Contact.Where(w => w.ObjectType == "CUS" && w.OtherPhone.Trim() == parameter.Phone.Trim().ToLower()).FirstOrDefault();
                }

                if (entityCheckDuplicatePhoneLead != null || entityCheckDuplicateEmailLead != null)
                {
                    IsDuplicateLead = true;
                }

                if (contactEntityCheckDuplicatePhoneCustomer != null || contactEntityCheckDuplicateWorkPhoneCustomer != null || contactEntityCheckDuplicateOtherPhoneCustomer != null || contactEntityCheckDuplicateEmailCustomer != null || contactEntityCheckDuplicateWorkEmailCustomer != null || contactEntityCheckDuplicateOtherEmailCustomer != null)
                {
                    IsDuplicateCustomer = true;
                }

                Contact customer = null;
                Guid LeadId = Guid.Empty;

                if (entityCheckDuplicatePhoneLead != null)
                {
                    LeadId = entityCheckDuplicatePhoneLead.LeadId;
                }
                else if (entityCheckDuplicateEmailLead != null)
                {
                    LeadId = entityCheckDuplicateEmailLead.LeadId;
                }

                if (contactEntityCheckDuplicatePhoneCustomer != null)
                {
                    customer = contactEntityCheckDuplicatePhoneCustomer;
                }
                else if (contactEntityCheckDuplicateWorkPhoneCustomer != null)
                {
                    customer = contactEntityCheckDuplicateWorkPhoneCustomer;
                }
                else if (contactEntityCheckDuplicateOtherPhoneCustomer != null)
                {
                    customer = contactEntityCheckDuplicateOtherPhoneCustomer;
                }
                else if (contactEntityCheckDuplicateEmailCustomer != null)
                {
                    customer = contactEntityCheckDuplicateEmailCustomer;
                }
                else if (contactEntityCheckDuplicateWorkEmailCustomer != null)
                {
                    customer = contactEntityCheckDuplicateWorkEmailCustomer;
                }
                else if (contactEntityCheckDuplicateOtherEmailCustomer != null)
                {
                    customer = contactEntityCheckDuplicateOtherEmailCustomer;
                }

                return new CheckDuplicatePersonalCustomerByEmailOrPhoneResult
                {
                    IsDuplicateCustomer = IsDuplicateCustomer,
                    IsDuplicateLead = IsDuplicateLead,
                    CustomerId = customer != null ? customer.ObjectId : Guid.Empty,
                    ContactId = customer != null ? customer.ContactId : Guid.Empty,
                    LeadId = LeadId,
                    StatusCode = System.Net.HttpStatusCode.OK,
                };
            }
            catch (Exception)
            {
                return new CheckDuplicatePersonalCustomerByEmailOrPhoneResult
                {
                    MessageCode = "Check Duplicate Personal Customer By Email Or Phone  Fail",
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                };
            }
        }

        public GetAllCustomerAdditionalByCustomerIdResult GetAllCustomerAdditionalByCustomerId(GetAllCustomerAdditionalByCustomerIdParameter parameter)
        {
            try
            {
                var customerAdditionalInformationList = context.CustomerAdditionalInformation.Where(cusAdd => cusAdd.CustomerId == parameter.CustomerId).ToList();

                List<dynamic> lstResult = new List<dynamic>();
                customerAdditionalInformationList.ForEach(item =>
                {
                    var sampleObject = new ExpandoObject() as IDictionary<string, Object>;
                    sampleObject.Add("customerAdditionalInformationId", item.CustomerAdditionalInformationId);
                    sampleObject.Add("customerId", item.CustomerId);
                    sampleObject.Add("question", item.Question);
                    sampleObject.Add("answer", item.Answer);
                    sampleObject.Add("active", item.Active);
                    sampleObject.Add("createdById", item.CreatedById);
                    sampleObject.Add("createdDate", item.CreatedDate);
                    sampleObject.Add("updatedById", item.UpdatedById);
                    sampleObject.Add("updatedDate", item.UpdatedDate);
                    lstResult.Add(sampleObject);
                });

                return new GetAllCustomerAdditionalByCustomerIdResult
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    CustomerAdditionalInformationList = lstResult
                };
            }
            catch (Exception)
            {
                return new GetAllCustomerAdditionalByCustomerIdResult
                {
                    MessageCode = "Không lấy được thông tin của câu hỏi và trả lời",
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                };
            }
        }

        public CreateCustomerAdditionalResult CreateCustomerAdditional(CreateCustomerAdditionalParameter parameter)
        {
            try
            {
                if (parameter.CustomerAdditionalInformationId != null && parameter.CustomerAdditionalInformationId != Guid.Empty)
                {
                    var cusAdd = context.CustomerAdditionalInformation.FirstOrDefault(x =>
                        x.CustomerAdditionalInformationId == parameter.CustomerAdditionalInformationId);

                    cusAdd.Question = parameter.Question;
                    cusAdd.Answer = parameter.Answer;

                    context.CustomerAdditionalInformation.Update(cusAdd);
                    context.SaveChanges();
                }
                else
                {
                    var customerAdditionalInformation = new CustomerAdditionalInformation();
                    customerAdditionalInformation.CustomerAdditionalInformationId = Guid.NewGuid();
                    customerAdditionalInformation.Question = parameter.Question;
                    customerAdditionalInformation.Answer = parameter.Answer;
                    customerAdditionalInformation.Active = true;
                    customerAdditionalInformation.CustomerId = parameter.CustomerId;
                    customerAdditionalInformation.CreatedById = parameter.UserId;
                    customerAdditionalInformation.CreatedDate = DateTime.Now;

                    context.CustomerAdditionalInformation.Add(customerAdditionalInformation);
                    context.SaveChanges();
                }

                var listCustomerAdditionalInformation = new List<CustomerAdditionalInformationEntityModel>();
                listCustomerAdditionalInformation = context.CustomerAdditionalInformation
                    .Where(x => x.CustomerId == parameter.CustomerId)
                    .Select(y => new CustomerAdditionalInformationEntityModel
                    {
                        CustomerAdditionalInformationId = y.CustomerAdditionalInformationId,
                        CustomerId = y.CustomerId,
                        Question = y.Question,
                        Answer = y.Answer,
                        Active = y.Active,
                        CreatedById = y.CreatedById,
                        CreatedDate = y.CreatedDate,
                        UpdatedById = y.UpdatedById,
                        UpdatedDate = y.UpdatedDate,
                    }).OrderByDescending(z => z.CreatedDate).ToList();

                return new CreateCustomerAdditionalResult
                {
                    ListCustomerAdditionalInformation = listCustomerAdditionalInformation,
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = "Lưu thành công"
                };
            }
            catch (Exception e)
            {
                return new CreateCustomerAdditionalResult
                {
                    MessageCode = e.Message,
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                };
            }
        }

        public DeleteCustomerAdditionalResult DeleteCustomerAdditional(DeleteCustomerAdditionalParameter parameter)
        {
            try
            {
                var customerAdditionalInformation = context.CustomerAdditionalInformation.FirstOrDefault(cusAdd => cusAdd.CustomerAdditionalInformationId == parameter.CustomerAdditionalInformationId);

                if (customerAdditionalInformation != null)
                {
                    context.CustomerAdditionalInformation.Remove(customerAdditionalInformation);
                    context.SaveChanges();
                }

                var listCustomerAdditionalInformation = new List<CustomerAdditionalInformationEntityModel>();
                listCustomerAdditionalInformation = context.CustomerAdditionalInformation
                    .Where(x => x.CustomerId == parameter.CustomerId).
                    Select(y => new CustomerAdditionalInformationEntityModel
                    {
                        CustomerAdditionalInformationId = y.CustomerAdditionalInformationId,
                        CustomerId = y.CustomerId,
                        Question = y.Question,
                        Answer = y.Answer,
                        Active = y.Active,
                        CreatedById = y.CreatedById,
                        CreatedDate = y.CreatedDate,
                        UpdatedById = y.UpdatedById,
                        UpdatedDate = y.UpdatedDate,
                    }).OrderByDescending(z => z.CreatedDate).ToList();

                return new DeleteCustomerAdditionalResult
                {
                    ListCustomerAdditionalInformation = listCustomerAdditionalInformation,
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = "xóa câu hỏi - trả lời thành công"
                };
            }
            catch (Exception e)
            {
                return new DeleteCustomerAdditionalResult
                {
                    MessageCode = e.Message,
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                };
            }
        }

        public EditCustomerAdditionalResult EditCustomerAdditional(EditCustomerAdditionalParameter parameter)
        {
            try
            {
                var customerAdditionalInformation = context.CustomerAdditionalInformation.FirstOrDefault(cusAdd => cusAdd.CustomerAdditionalInformationId == parameter.CustomerAdditionalInformationId);

                if (customerAdditionalInformation != null)
                {
                    customerAdditionalInformation.Question = parameter.Question;
                    customerAdditionalInformation.Answer = parameter.Answer;
                    customerAdditionalInformation.UpdatedById = parameter.UserId;
                    customerAdditionalInformation.UpdatedDate = DateTime.Now;
                    context.CustomerAdditionalInformation.Update(customerAdditionalInformation);
                    context.SaveChanges();
                }

                return new EditCustomerAdditionalResult
                {
                    CustomerAdditionalInformationId = customerAdditionalInformation.CustomerAdditionalInformationId,
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = "Sửa câu hỏi - trả lời thành công"
                };
            }
            catch (Exception e)
            {
                return new EditCustomerAdditionalResult
                {
                    MessageCode = "Sửa câu hỏi - trả lời thất bại",
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                };
            }
        }

        public CreateListQuestionResult CreateListQuestion(CreateListQuestionParameter parameter)
        {
            try
            {
                List<CustomerAdditionalInformation> questionList = new List<CustomerAdditionalInformation>();

                var categoryTypeId = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "BHO")
                    .CategoryTypeId;
                List<string> listQuestion = new List<string>();
                listQuestion = context.Category.Where(x => x.CategoryTypeId == categoryTypeId && x.Active == true)
                    .Select(x => x.CategoryName).ToList();

                listQuestion.ForEach(item =>
                {
                    var customerAdditionalInformation = new CustomerAdditionalInformation();
                    customerAdditionalInformation.CustomerAdditionalInformationId = Guid.NewGuid();
                    customerAdditionalInformation.Question = item;
                    customerAdditionalInformation.Answer = "";
                    customerAdditionalInformation.Active = true;
                    customerAdditionalInformation.CustomerId = parameter.CustomerId;
                    customerAdditionalInformation.CreatedById = parameter.UserId;
                    customerAdditionalInformation.CreatedDate = DateTime.Now;
                    questionList.Add(customerAdditionalInformation);
                });

                context.CustomerAdditionalInformation.AddRange(questionList);
                context.SaveChanges();

                var listCustomerAdditionalInformation = new List<CustomerAdditionalInformationEntityModel>();
                listCustomerAdditionalInformation = context.CustomerAdditionalInformation
                    .Where(x => x.CustomerId == parameter.CustomerId)
                    .Select(y => new CustomerAdditionalInformationEntityModel
                    {
                        CustomerAdditionalInformationId = y.CustomerAdditionalInformationId,
                        CustomerId = y.CustomerId,
                        Question = y.Question,
                        Answer = y.Answer,
                        Active = y.Active,
                        CreatedById = y.CreatedById,
                        CreatedDate = y.CreatedDate,
                        UpdatedById = y.UpdatedById,
                        UpdatedDate = y.UpdatedDate,
                    }).ToList();

                return new CreateListQuestionResult
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    ListCustomerAdditionalInformation = listCustomerAdditionalInformation,
                    MessageCode = "Thêm câu hỏi - trả lời thành công"
                };
            }
            catch (Exception e)
            {
                return new CreateListQuestionResult
                {
                    MessageCode = e.Message,
                    StatusCode = System.Net.HttpStatusCode.OK
                };
            }
        }

        public GetListQuestionAnswerBySearchResult GetListQuestionAnswerBySearch(GetListQuestionAnswerBySearchParameter parameter)
        {
            try
            {
                var listCustomerAdditionalInformation = context.CustomerAdditionalInformation.Where(cusAdd => cusAdd.CustomerId == parameter.CustomerId && (cusAdd.Question.Contains(parameter.TextSearch) || cusAdd.Answer.Contains(parameter.TextSearch))).ToList();

                List<dynamic> lstResult = new List<dynamic>();
                listCustomerAdditionalInformation.ForEach(item =>
                {
                    var sampleObject = new ExpandoObject() as IDictionary<string, Object>;
                    sampleObject.Add("customerAdditionalInformationId", item.CustomerAdditionalInformationId);
                    sampleObject.Add("customerId", item.CustomerId);
                    sampleObject.Add("question", item.Question);
                    sampleObject.Add("answer", item.Answer);
                    sampleObject.Add("active", item.Active);
                    sampleObject.Add("createdById", item.CreatedById);
                    sampleObject.Add("createdDate", item.CreatedDate);
                    sampleObject.Add("updatedById", item.UpdatedById);
                    sampleObject.Add("updatedDate", item.UpdatedDate);
                    lstResult.Add(sampleObject);
                });

                return new GetListQuestionAnswerBySearchResult
                {
                    CustomerAdditionalInformationList = lstResult,
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = "Tìm kiếm câu hỏi - trả lời thành công"
                };
            }
            catch (Exception e)
            {
                return new GetListQuestionAnswerBySearchResult
                {
                    MessageCode = "Tìm kiếm câu hỏi - trả lời thất bại",
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                };
            }
        }

        public GetAllHistoryProductByCustomerIdResult GetAllHistoryProductByCustomerId(GetAllHistoryProductByCustomerIdParameter parameter)
        {
            try
            {
                var listProduct = context.CustomerOrder.Where(cus => cus.CustomerId == parameter.CustomerId)
                                         .OrderByDescending(x => x.OrderDate).ToList();
                List<dynamic> lstResult = new List<dynamic>();
                #region Edit By Hung
                if (listProduct.Count > 0)
                {
                    List<Guid> listSellerId = new List<Guid>();
                    List<Guid> listStatusId = new List<Guid>();
                    listProduct.ForEach(item =>
                    {
                        if (item.Seller != null && !listSellerId.Contains(item.Seller.Value))
                        {
                            listSellerId.Add(item.Seller.Value);
                        }
                        if (item.StatusId != null && !listStatusId.Contains(item.StatusId.Value))
                        {
                            listStatusId.Add(item.StatusId.Value);
                        }
                        if (item.DiscountValue != null || item.DiscountValue != 0)
                        {
                            if (item.DiscountType == true)
                            {
                                //Nếu chiết khấu theo %
                                item.Amount = item.Amount - (item.Amount * item.DiscountValue.Value / 100);
                            }
                            else
                            {
                                //Nếu chiết khấu theo số tiền
                                item.Amount = item.Amount - item.DiscountValue.Value;
                            }
                        }
                    });
                    var listSeller = context.Employee.Where(w => listSellerId.Contains(w.EmployeeId)).ToList();
                    var listStatus = context.OrderStatus.Where(os => listStatusId.Contains(os.OrderStatusId)).ToList();
                    listProduct.ForEach(item =>
                    {
                        var seller = listSeller.FirstOrDefault(emp => emp.EmployeeId == item.Seller);
                        string sellerName = "";
                        if (seller != null && seller.EmployeeName != null)
                        {
                            sellerName = seller.EmployeeName.Trim();
                        }
                        var statusName = "";
                        var orderStatus = listStatus.FirstOrDefault(os => os.OrderStatusId == item.StatusId);
                        if (orderStatus != null && orderStatus.Description != null)
                        {
                            statusName = orderStatus.Description.Trim();
                        }
                        var sampleObject = new ExpandoObject() as IDictionary<string, Object>;
                        sampleObject.Add("orderId", item.OrderId);
                        sampleObject.Add("orderCode", item.OrderCode);
                        sampleObject.Add("seller", item.Seller);
                        sampleObject.Add("sellerName", sellerName);
                        sampleObject.Add("amount", item.Amount);
                        sampleObject.Add("statusId", item.StatusId);
                        sampleObject.Add("statusName", statusName);
                        sampleObject.Add("createdDate", item.CreatedDate);
                        sampleObject.Add("orderDate", item.OrderDate);
                        lstResult.Add(sampleObject);
                    });
                }
                #endregion
                return new GetAllHistoryProductByCustomerIdResult
                {
                    listProduct = lstResult,
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = "Lấy thông tin lịch sử sản phẩm/dịch vụ thành công"
                };
            }
            catch (Exception e)
            {
                return new GetAllHistoryProductByCustomerIdResult
                {
                    MessageCode = "Lấy thông tin lịch sử sản phẩm/dịch vụ thất bại",
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed
                };
            }
        }

        public CreateCustomerFromProtalResult CreateCustomerFromProtal(CreateCustomerFromProtalParameter parameter)
        {
            if (parameter.TokenId == "6db0e752-1908-41eb-a8c1-97ae079b3e61")
            {
                try
                {
                    string Email = parameter.Email;
                    string Phone = parameter.Phone;
                    Contact contactEntityCheckEmailDuplicate = null;
                    Contact contactEntityCheckPhoneDuplicate = null;

                    if (!string.IsNullOrEmpty(Email))
                    {
                        contactEntityCheckEmailDuplicate = context.Contact.Where(w => w.Email.ToLower() == Email.Trim().ToLower()).FirstOrDefault();
                    }
                    else
                    {
                        return new CreateCustomerFromProtalResult
                        {
                            MessageCode = "Không được để trống Email",
                            StatusCode = System.Net.HttpStatusCode.ExpectationFailed
                        };
                    }

                    if (!string.IsNullOrEmpty(Phone))
                    {
                        contactEntityCheckPhoneDuplicate = context.Contact.Where(w => w.Phone.ToLower() == Phone.Trim().ToLower()).FirstOrDefault();
                    }
                    else
                    {
                        return new CreateCustomerFromProtalResult
                        {
                            MessageCode = "Không được để trống số điện thoại",
                            StatusCode = System.Net.HttpStatusCode.ExpectationFailed
                        };

                    }
                    if (contactEntityCheckEmailDuplicate == null && contactEntityCheckPhoneDuplicate == null)
                    {
                        //Status Customer
                        var listCustomerStatus = (from categoryT in context.CategoryType
                                                  join category in context.Category on categoryT.CategoryTypeId equals category.CategoryTypeId
                                                  where categoryT.CategoryTypeCode == "THA"
                                                  select category).ToList();
                        //Loai doanh nghiep 
                        var listBusinessType = (from categoryT in context.CategoryType
                                                join category in context.Category on categoryT.CategoryTypeId equals category.CategoryTypeId
                                                where categoryT.CategoryTypeCode == "LNG"
                                                select category).ToList();
                        //lĩnh vực SX/KD
                        var listCategoryTypeField = (from categoryT in context.CategoryType
                                                     join category in context.Category on categoryT.CategoryTypeId equals category.CategoryTypeId
                                                     where categoryT.CategoryTypeCode == "LDO"
                                                     select category).ToList();
                        //loại hình doanh nghiệp EnterpriseType
                        var listEnterpriseType = (from categoryT in context.CategoryType
                                                  join category in context.Category on categoryT.CategoryTypeId equals category.CategoryTypeId
                                                  where categoryT.CategoryTypeCode == "LHI"
                                                  select category).ToList();

                        //Quy mo
                        var listBusinessScale = (from categoryT in context.CategoryType
                                                 join category in context.Category on categoryT.CategoryTypeId equals category.CategoryTypeId
                                                 where categoryT.CategoryTypeCode == "QNG"
                                                 select category).ToList();
                        //Group ID
                        var listCustomerGroup = (from categoryT in context.CategoryType
                                                 join category in context.Category on categoryT.CategoryTypeId equals category.CategoryTypeId
                                                 where categoryT.CategoryTypeCode == "NHA"
                                                 select category).ToList();

                        //Auto gen CustomerCode
                        #region Comment By Hung
                        //int currentYear = DateTime.Now.Year % 100;
                        //int currentMonth = DateTime.Now.Month;
                        //int currentDate = DateTime.Now.Day;
                        //var lstRequestPayment = context.Customer.Where(w => w.YearCode == currentYear && w.MonthCode == currentMonth && w.DateCode == currentDate).Select(s => s.NumberCode.Value).ToList();
                        //int MaxNumberCode = 0;
                        //if (lstRequestPayment.Count > 0)
                        //{
                        //    MaxNumberCode = lstRequestPayment.Max();
                        //}
                        #endregion
                        var CustomerGroupId = listCustomerGroup.Where(x => x.CategoryCode == "POR").Select(d => (Guid)d.CategoryId).FirstOrDefault();

                        if (CustomerGroupId == null)
                        {
                            return new CreateCustomerFromProtalResult
                            {
                                MessageCode = "Chưa tồn tại mã nhóm khách hàng Portal trên CRM",
                                StatusCode = System.Net.HttpStatusCode.ExpectationFailed
                            };
                        }
                        var PortalUser = context.User.Where(w => w.UserName == "PortalUser").Select(s => s.UserId).FirstOrDefault();
                        if (PortalUser == null)
                        {
                            return new CreateCustomerFromProtalResult
                            {
                                MessageCode = "Chưa tồn tại mã user Portal trên CRM",
                                StatusCode = System.Net.HttpStatusCode.ExpectationFailed
                            };
                        }
                        Customer CreateNewCustomer = new Customer
                        {
                            CustomerGroupId = CustomerGroupId,
                            CustomerType = 1,
                            StatusId = listCustomerStatus.Where(w => w.CategoryCode == "MOI").Select(d => (Guid?)d.CategoryId).DefaultIfEmpty(null).FirstOrDefault().Value,
                            CreatedById = PortalUser,
                            CreatedDate = DateTime.Now
                        };
                        if (!string.IsNullOrEmpty(parameter.TaxCode))
                        {
                            CreateNewCustomer.CustomerCode = parameter.TaxCode;
                        }
                        else
                        {
                            CreateNewCustomer.CustomerCode = this.GenerateCustomerCode(0); //Edit by Hung//Format("CTM{0}{1}{2}{3}", currentYear, currentMonth, currentDate, (MaxNumberCode + 1).ToString("D4"));
                            //CreateNewCustomer.NumberCode = MaxNumberCode;
                            //CreateNewCustomer.YearCode = currentYear;
                            //CreateNewCustomer.MonthCode = currentMonth;
                            //CreateNewCustomer.DateCode = currentDate;
                        }
                        CreateNewCustomer.CustomerName = parameter.CustomerName;
                        if (parameter.BusinessRegistrationDate != null)
                        {
                            CreateNewCustomer.BusinessRegistrationDate = parameter.BusinessRegistrationDate;
                        }
                        if (!string.IsNullOrEmpty(parameter.EnterpriseType))
                        {
                            CreateNewCustomer.EnterpriseType = (listEnterpriseType.Where(x => x.CategoryCode == parameter.EnterpriseType).Select(d => (Guid?)d.CategoryId).DefaultIfEmpty(null).FirstOrDefault());
                        }
                        if (!string.IsNullOrEmpty(parameter.FieldId))
                        {
                            CreateNewCustomer.FieldId = (listCategoryTypeField.Where(x => x.CategoryCode == parameter.FieldId).Select(d => (Guid?)d.CategoryId).DefaultIfEmpty(null).FirstOrDefault());
                        }
                        if (!string.IsNullOrEmpty(parameter.BusinessScale))
                        {
                            CreateNewCustomer.BusinessScale = (listBusinessScale.Where(x => x.CategoryCode == parameter.BusinessScale).Select(d => (Guid?)d.CategoryId).DefaultIfEmpty(null).FirstOrDefault());
                        }
                        CreateNewCustomer.TotalEmployeeParticipateSocialInsurance = parameter.TotalEmployeeParticipateSocialInsurance;
                        CreateNewCustomer.TotalCapital = parameter.TotalCapital;
                        CreateNewCustomer.TotalRevenueLastYear = parameter.TotalRevenueLastYear;
                        CreateNewCustomer.PortalId = parameter.PortalID;
                        //khách hàng
                        context.Customer.Add(CreateNewCustomer);
                        context.SaveChanges();

                        Contact CreateContact = new Contact();
                        CreateContact.ObjectId = CreateNewCustomer.CustomerId;
                        CreateContact.ObjectType = "CUS";
                        CreateContact.TaxCode = parameter.TaxCode;
                        CreateContact.Address = parameter.Address;
                        CreateContact.Email = parameter.Email;
                        CreateContact.Phone = parameter.Phone;
                        CreateContact.CreatedById = PortalUser;
                        CreateContact.CreatedDate = DateTime.Now;
                        context.Contact.Add(CreateContact);
                        context.SaveChanges();

                        return new CreateCustomerFromProtalResult
                        {
                            MessageCode = "Đã tạo mới khách hàng",
                            StatusCode = System.Net.HttpStatusCode.OK
                        };
                    }
                    else
                    {
                        return new CreateCustomerFromProtalResult
                        {
                            MessageCode = "Khách hàng đã tồn tại",
                            StatusCode = System.Net.HttpStatusCode.ExpectationFailed
                        };

                    }
                }
                catch (Exception ex)
                {

                    return new CreateCustomerFromProtalResult
                    {
                        MessageCode = ex.ToString(),
                        StatusCode = System.Net.HttpStatusCode.ExpectationFailed
                    };
                }
            }
            else
            {
                return new CreateCustomerFromProtalResult
                {
                    MessageCode = "Invalid Token",
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed
                };
            }
        }
        /// <summary>
        /// When maxCode=0 them check CustomerCode
        /// when maxCode>0 them number in CustomerCode + 1
        /// </summary>
        /// <param name="maxCode"></param>
        /// <returns></returns>
        private string GenerateCustomerCode(int maxCode)
        {
            //Auto gen CustomerCode 1911190001
            int currentYear = DateTime.Now.Year % 100;
            int currentMonth = DateTime.Now.Month;
            int currentDate = DateTime.Now.Day;
            int MaxNumberCode = 0;
            var customerList = context.Customer.Where(cu => cu.Active == true).ToList();
            if (maxCode == 0)
            {
                var customer = customerList.OrderByDescending(or => or.CreatedDate).FirstOrDefault();
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
            }
            else
            {
                MaxNumberCode = maxCode + 1;
            }
            var customerCodeNew = string.Format("CTM{0}{1}{2}{3}", currentYear, currentMonth, currentDate, (MaxNumberCode).ToString("D4"));
            var customerCodeList = customerList.Where(c => c.CustomerCode == customerCodeNew).ToList();
            if (customerCodeList.Count > 0)
            {
                return GenerateCustomerCode(MaxNumberCode);
            }

            return string.Format("CTM{0}{1}{2}{3}", currentYear, currentMonth, currentDate, (MaxNumberCode).ToString("D4"));
        }

        public ChangeCustomerStatusToDeleteResult ChangeCustomerStatusToDelete(ChangeCustomerStatusToDeleteParameter parameter)
        {
            try
            {
                var cusomer = context.Customer.FirstOrDefault(l => l.CustomerId == parameter.CustomerId);

                #region Kiểm tra các liên kết trước khi xóa

                var quote = context.Quote.FirstOrDefault(x => x.ObjectTypeId == cusomer.CustomerId && x.Active == true);

                if (quote != null)
                {
                    return new ChangeCustomerStatusToDeleteResult()
                    {
                        StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                        MessageCode = "Không thể xóa khách hàng đã phát sinh báo giá"
                    };
                }

                #endregion

                cusomer.Active = false;
                cusomer.UpdatedById = parameter.UserId;
                cusomer.UpdatedDate = DateTime.Now;
                context.Customer.Update(cusomer);

                var listContact = context.Contact.Where(c =>
                    c.ObjectId == parameter.CustomerId &&
                    (c.ObjectType == ObjectType.CUSTOMER || c.ObjectType == ObjectType.CUSTOMERCONTACT)).ToList();

                listContact.ForEach(item =>
                {
                    item.UpdatedById = parameter.UserId;
                    item.Active = false;
                    item.UpdatedDate = DateTime.Now;
                });
                context.Contact.UpdateRange(listContact);

                context.SaveChanges();

                #region Log

                var categoryTypeId = context.CategoryType.FirstOrDefault(x => x.Active == true && x.CategoryTypeCode == "THA")?.CategoryTypeId;
                var statusId = context.Category.FirstOrDefault(x => x.Active == true && x.CategoryTypeId == categoryTypeId && x.CategoryCode == "MOI")?.CategoryId;

                LogHelper.AuditTrace(context, "Delete",
                    cusomer.StatusId == statusId ? "POTENTIAL_CUSTOMER" : "CUSTOMER", cusomer.CustomerId,
                    parameter.UserId);

                #endregion

                return new ChangeCustomerStatusToDeleteResult()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = CommonMessage.Customer.DELETE_SUCCESS
                };
            }
            catch (Exception e)
            {
                return new ChangeCustomerStatusToDeleteResult()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = CommonMessage.Customer.DELETE_FAIL
                };
            }
        }

        public GetDashBoardCustomerResult GetDashBoardCustomer(GetDashBoardCustomerParameter parameter)
        {
            try
            {
                var appName = context.SystemParameter.FirstOrDefault(x => x.SystemKey == "ApplicationName")?.SystemValueString;
                var user = context.User.FirstOrDefault(x => x.UserId == parameter.UserId);

                var lstOrderStt = new List<string>() { "IP", "COMP", "DLV", "PD" };
                var listOrderStatusAccept = context.OrderStatus
                    .Where(x => x.Active == true && lstOrderStt.Contains(x.OrderStatusCode)).ToList();
                var listOrderStatusIdAccept = listOrderStatusAccept.Select(x => x.OrderStatusId).ToList();

                var listOrderDetail = context.CustomerOrderDetail.Where(x => x.Active == true).ToList();
                var listProductCategory = context.ProductCategory.Where(x => x.Active == true).ToList();

                var listEmployee = context.Employee.ToList(); //Active = false ?
                var listContact = context.Contact.Where(x => x.ObjectType == "CUS" || x.ObjectType == "EMP").ToList();

                var listCusFollowProduct = new List<ProductCategoryEntityModel>();
                var listTopPic = new List<CustomerEntityModel>();
                var listCusCreatedInThisYear = new List<CustomerEntityModel>();
                var listCusIsNewest = new List<CustomerEntityModel>();
                var listCusIsNewBought = new List<CustomerEntityModel>();
                var listCusTopRevenueInMonth = new List<CustomerEntityModel>();
                var listCusIdentification = new List<CustomerEntityModel>();
                var listCusFree = new List<CustomerEntityModel>();
                var statusCusTypeId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "THA").CategoryTypeId;
                var listAllStatus = context.Category.Where(c => c.CategoryTypeId == statusCusTypeId).ToList();

                var listOrderCost = context.OrderCostDetail.ToList();
                var listPromotionObjectApply = context.PromotionObjectApply.ToList();

                //chỉ lấy khách hàng định danh - add by dungpt
                var HDOStatusId = listAllStatus.FirstOrDefault(f => f.CategoryCode == "HDO").CategoryId;

                var employee = listEmployee.FirstOrDefault(x => x.EmployeeId == user.EmployeeId);

                if (employee.IsManager)
                {
                    /*
                     * Lấy list phòng ban con của user
                     * List phòng ban bao gồm: chính phòng ban nó đang thuộc và các phòng ban cấp dưới của nó nếu có
                     */
                    List<Guid?> listGetAllChild = new List<Guid?>();
                    listGetAllChild.Add(employee.OrganizationId.Value);
                    listGetAllChild = getOrganizationChildrenId(employee.OrganizationId.Value, listGetAllChild);

                    listEmployee = listEmployee
                        .Where(x => listGetAllChild.Count == 0 || listGetAllChild.Contains(x.OrganizationId)).ToList();

                    var listEmployeeId = listEmployee.Select(y => y.EmployeeId).ToList();

                    //Lấy list UserId theo list EmployeeId
                    var listUser = context.User.Where(x => listEmployeeId.Contains(x.EmployeeId.Value)).ToList();

                    var listUserId = listUser.Select(y => y.UserId).ToList();

                    //Lấy list Customer theo list nhân viên phụ trách
                    //var listCustomer1 = context.Customer.Where(x => x.PersonInChargeId != null && x.Active == true).ToList(); //comment by dungpt
                    var listCustomer1 = context.Customer.Where(x => x.PersonInChargeId != null
                                                             && x.Active == true
                                                             && x.StatusId == HDOStatusId
                                                             ).ToList(); //add by dungpt

                    listCustomer1 = listCustomer1.Where(x => listEmployeeId.Contains(x.PersonInChargeId.Value))
                        .ToList();

                    #region Lấy list khách hàng theo người phụ trách

                    listTopPic = (from cus in listCustomer1
                                  join emp in listEmployee on cus.PersonInChargeId equals emp.EmployeeId
                                  select new CustomerEntityModel()
                                  {
                                      PicName = emp.EmployeeName,
                                      PersonInChargeId = emp.EmployeeId
                                  }).OrderByDescending(id => id.PersonInChargeId).ToList();

                    #endregion

                    var listSubCustomer = context.Customer
                        .Where(x => x.PersonInChargeId == null && x.StatusId == HDOStatusId && listUserId.Contains(x.CreatedById) && x.Active == true).ToList();

                    //Ghép 2 list customer lại với nhau
                    listCustomer1.AddRange(listSubCustomer);

                    var listCustomerId = listCustomer1.Select(x => x.CustomerId).ToList();

                    #region Lấy list khách hàng được tạo trong năm

                    var statusCusId = context.Category.FirstOrDefault(c => c.CategoryCode == "HDO" && c.CategoryTypeId == statusCusTypeId).CategoryId;

                    listCusCreatedInThisYear = (from cus in listCustomer1
                                                where cus.CreatedDate.Year == DateTime.Now.Year && cus.StatusId == statusCusId
                                                select new CustomerEntityModel()
                                                {
                                                    CreatedDate = cus.CreatedDate,
                                                    CustomerName = cus.CustomerName,
                                                    MaximumDebtDays = cus.CreatedDate.Month,
                                                }).OrderBy(c => c.CreatedDate.Month).ToList();

                    #endregion

                    #region Lấy list khách hàng định danh
                    listCusIdentification = listCustomer1.Where(c => c.StatusId == statusCusId)
                                                            .Select(y => new CustomerEntityModel
                                                            {
                                                                CustomerId = y.CustomerId,
                                                                CustomerName = y.CustomerName,
                                                                ContactId = Guid.Empty,
                                                                CustomerEmail = "",
                                                                CustomerPhone = "",
                                                                PersonInChargeId = y.PersonInChargeId,
                                                                PicName = "",
                                                                PicContactId = Guid.Empty,
                                                                CreatedById = y.CreatedById,
                                                                StatusId = y.StatusId,
                                                                CreatedDate = y.CreatedDate
                                                            }).OrderByDescending(m => m.CreatedDate).Take(5).ToList();
                    var listCusIdentificationId = listCusIdentification.Select(x => x.CustomerId).ToList();
                    if (listCusIdentificationId.Count > 0)
                    {
                        var listCusContact = listContact.Where(x => listCusIdentificationId.Contains(x.ObjectId) && x.ObjectType == "CUS").ToList();
                        listCusIdentification.ForEach(item =>
                        {
                            var cusContact = listCusContact.FirstOrDefault(x => x.ObjectId == item.CustomerId);
                            var email = cusContact != null ? (cusContact.Email != null ? cusContact.Email : "") : "";
                            var phone = cusContact != null ? (cusContact.Phone != null ? cusContact.Phone : "") : "";
                            var contacId = cusContact != null
                                ? (cusContact.ContactId != null ? cusContact.ContactId : Guid.Empty)
                                : Guid.Empty;

                            item.CustomerEmail = email;
                            item.CustomerPhone = phone;
                            item.ContactId = contacId;
                            item.StatusName = listAllStatus.FirstOrDefault(c => c.CategoryId == item.StatusId).CategoryName;
                            var statusCode = listAllStatus.FirstOrDefault(c => c.CategoryId == item.StatusId).CategoryCode;
                            switch (statusCode)
                            {
                                case "HDO":
                                    item.BackgroupStatus = "#0f62fe";
                                    break;
                                case "MOI":
                                    item.BackgroupStatus = "#ff3b30";
                                    break;
                            }

                            if (item.PersonInChargeId != null)
                            {
                                var picInfor = listEmployee.FirstOrDefault(x => x.EmployeeId == item.PersonInChargeId);
                                var picName = picInfor != null ? (picInfor.EmployeeName != null ? picInfor.EmployeeName : "") : "";
                                var picContact = listContact.FirstOrDefault(x => x.ObjectId == item.PersonInChargeId && x.ObjectType == "EMP");
                                item.PicName = picName;
                                item.PicContactId = picContact != null ? (picContact.ContactId != null ? picContact.ContactId : Guid.Empty) : Guid.Empty;
                            }
                        });
                    }
                    #endregion

                    //Lọc list Order theo list customer và trạng thái đơn hàng
                    if (listCustomerId.Count > 0)
                    {
                        var listOrder = context.CustomerOrder.Where(x =>
                                x.Active == true &&
                                listCustomerId.Contains(x.CustomerId.Value) &&
                                listOrderStatusIdAccept.Contains(x.StatusId.Value))
                            .ToList();

                        //Lấy list Order Detail theo list Order
                        var listOrderId = listOrder.Select(x => x.OrderId).ToList();

                        if (listOrderId.Count > 0)
                        {
                            #region Lấy list khách hàng có doanh thu cao nhất trong tháng hiện tại

                            var newListOrder = listOrder.Where(x => x.OrderDate.Month == DateTime.Now.Month && x.OrderDate.Year == DateTime.Now.Year)
                                .Select(y => new
                                {
                                    CustomerId = y.CustomerId,
                                    Total = appName == "VNS" ? CalculateTotalAmountAfterVatOrder(y.OrderId, y.QuoteId, y.DiscountType, y.DiscountValue, y.Vat, listOrderDetail,
                                        listOrderCost, listPromotionObjectApply) : CalculatorAmount(y.DiscountType.Value, y.DiscountValue.Value, y.Amount.Value)
                                }).ToList();

                            newListOrder = newListOrder.GroupBy(x => x.CustomerId).Select(y => new
                            {
                                CustomerId = y.First().CustomerId,
                                Total = y.Sum(s => s.Total)
                            }).OrderByDescending(z => z.Total).Take(5).ToList();

                            var listCusIdTopRevenueInMonth =
                                newListOrder.Select(x => x.CustomerId).ToList();

                            listCusTopRevenueInMonth = listCustomer1
                                .Where(x => listCusIdTopRevenueInMonth.Contains(x.CustomerId)).Select(y => new CustomerEntityModel
                                {
                                    CustomerId = y.CustomerId,
                                    CustomerName = y.CustomerName,
                                    ContactId = Guid.Empty,
                                    CustomerEmail = "",
                                    CustomerPhone = "",
                                    PersonInChargeId = y.PersonInChargeId,
                                    PicName = "",
                                    PicContactId = Guid.Empty,
                                    CreatedById = y.CreatedById
                                }).ToList();

                            var listCusTopRevenueContact = listContact.Where(x =>
                                listCusIdTopRevenueInMonth.Contains(x.ObjectId) && x.ObjectType == "CUS").ToList();
                            listCusTopRevenueInMonth.ForEach(item =>
                            {
                                var cusContact =
                                    listCusTopRevenueContact.FirstOrDefault(x => x.ObjectId == item.CustomerId);
                                var email = cusContact != null ? (cusContact.Email != null ? cusContact.Email : "") : "";
                                var phone = cusContact != null ? (cusContact.Phone != null ? cusContact.Phone : "") : "";
                                var contacId = cusContact != null
                                    ? (cusContact.ContactId != null ? cusContact.ContactId : Guid.Empty)
                                    : Guid.Empty;

                                item.CustomerEmail = email;
                                item.CustomerPhone = phone;
                                item.ContactId = contacId;

                                if (item.PersonInChargeId != null)
                                {
                                    var picInfor = listEmployee.FirstOrDefault(x => x.EmployeeId == item.PersonInChargeId);
                                    var picName = picInfor != null ? (picInfor.EmployeeName != null ? picInfor.EmployeeName : "") : "";
                                    var picContact = listContact.FirstOrDefault(x => x.ObjectId == item.PersonInChargeId && x.ObjectType == "EMP");
                                    item.PicName = picName;
                                    item.PicContactId = picContact != null ? (picContact.ContactId != null ? picContact.ContactId : Guid.Empty) : Guid.Empty;
                                }

                                //Doanh thu
                                item.TotalSaleValue = newListOrder.FirstOrDefault(x => x.CustomerId == item.CustomerId)
                                    .Total;
                            });

                            listCusTopRevenueInMonth = listCusTopRevenueInMonth.OrderByDescending(x => x.TotalSaleValue)
                                .Take(5).ToList();

                            #endregion

                            listOrderDetail = listOrderDetail.Where(x => listOrderId.Contains(x.OrderId)).ToList();

                            //Lấy list ProductId theo list OrderDetail
                            var listProductId = listOrderDetail
                                .Where(x => x.ProductId != null && x.ProductId != Guid.Empty).Select(y => y.ProductId)
                                .Distinct()
                                .ToList();

                            //Lấy list Product theo list ProductId
                            var listProduct = context.Product.Where(x => listProductId.Contains(x.ProductId)).ToList();

                            //listCusFollowProduct
                            listCusFollowProduct = (from orderDetail in listOrderDetail
                                                    join order in listOrder on orderDetail.OrderId equals order.OrderId
                                                    join product in listProduct on orderDetail.ProductId equals product.ProductId
                                                    join stt in listOrderStatusAccept on order.StatusId equals stt.OrderStatusId
                                                    join proCate in listProductCategory on product.ProductCategoryId equals proCate.ProductCategoryId
                                                    group new { orderDetail, order, product, stt, proCate } by new { order.CustomerId, proCate.ProductCategoryId } into temp
                                                    select new ProductCategoryEntityModel
                                                    {
                                                        CustomerId = temp.Key.CustomerId,
                                                        ProductCategoryId = temp.Key.ProductCategoryId
                                                    }).ToList();

                            listCusFollowProduct.ForEach(item =>
                            {
                                item.ProductCategoryName = GetProductCategoryParent(item.ProductCategoryId, listProductCategory).ProductCategoryName;
                                item.ProductCategoryCode = GetProductCategoryParent(item.ProductCategoryId, listProductCategory).ProductCategoryCode;
                            });

                            listCusFollowProduct = (from lst in listCusFollowProduct
                                                    group lst by new { lst.ProductCategoryCode, lst.CustomerId } into temp
                                                    select new ProductCategoryEntityModel
                                                    {
                                                        CustomerId = temp.Key.CustomerId,
                                                        ProductCategoryCode = temp.Key.ProductCategoryCode,
                                                        ProductCategoryName = temp.First().ProductCategoryName
                                                    }).ToList();

                            listCusFollowProduct = (from temp in listCusFollowProduct
                                                    group temp by temp.ProductCategoryCode into qe
                                                    select new ProductCategoryEntityModel
                                                    {
                                                        ProductCategoryCode = qe.Key,
                                                        ProductCategoryName = qe.First().ProductCategoryName,
                                                        CountCustomer = qe.Count(),
                                                    }).ToList();
                        }
                    }
                }
                else
                {
                    var picInfor = listEmployee.FirstOrDefault(x => x.EmployeeId == employee.EmployeeId);
                    var picContact =
                        listContact.FirstOrDefault(x => x.ObjectId == picInfor.EmployeeId && x.ObjectType == "EMP");

                    var listCustomer1 = context.Customer
                        .Where(x => x.Active == true && x.StatusId == HDOStatusId && x.PersonInChargeId == user.EmployeeId).ToList(); //add by dungpt

                    #region Lấy list khách hàng theo người phụ trách

                    listTopPic = (from cus in listCustomer1
                                  select new CustomerEntityModel()
                                  {
                                      PicName = employee.EmployeeName,
                                      PersonInChargeId = employee.EmployeeId
                                  }).OrderByDescending(id => id.PersonInChargeId).ToList();

                    #endregion

                    var listCustomer2 = context.Customer.Where(x =>
                        x.Active == true && x.StatusId == HDOStatusId && x.PersonInChargeId == null && x.CreatedById == user.UserId).ToList();//add by dungpt

                    listCustomer1.AddRange(listCustomer2);

                    var listCustomerId = listCustomer1.Select(x => x.CustomerId).ToList();

                    #region Lấy list khách hàng được tạo trong năm

                    var statusCusId = context.Category.FirstOrDefault(c => c.CategoryCode == "HDO").CategoryId;

                    listCusCreatedInThisYear = (from cus in listCustomer1
                                                where cus.CreatedDate.Year == DateTime.Now.Year && cus.StatusId == statusCusId
                                                select new CustomerEntityModel()
                                                {
                                                    CreatedDate = cus.CreatedDate,
                                                    CustomerName = cus.CustomerName,
                                                    MaximumDebtDays = cus.CreatedDate.Month
                                                }).OrderBy(c => c.CreatedDate.Month).ToList();

                    #endregion

                    #region Lấy list khách hàng định danh

                    listCusIdentification = listCustomer1.Where(c => c.StatusId == statusCusId)
                                                            .Select(y => new CustomerEntityModel
                                                            {
                                                                CustomerId = y.CustomerId,
                                                                CustomerName = y.CustomerName,
                                                                ContactId = Guid.Empty,
                                                                CustomerEmail = "",
                                                                CustomerPhone = "",
                                                                PersonInChargeId = y.PersonInChargeId,
                                                                PicName = "",
                                                                PicContactId = Guid.Empty,
                                                                CreatedById = y.CreatedById,
                                                                StatusId = y.StatusId,
                                                                CreatedDate = y.CreatedDate
                                                            }).OrderByDescending(m => m.CreatedDate).Take(5).ToList();
                    var listCusIdentificationId = listCusIdentification.Select(x => x.CustomerId).ToList();
                    if (listCusIdentificationId.Count > 0)
                    {
                        var listCusContact = listContact.Where(x => listCusIdentificationId.Contains(x.ObjectId) && x.ObjectType == "CUS").ToList();
                        listCusIdentification.ForEach(item =>
                        {
                            var cusContact = listCusContact.FirstOrDefault(x => x.ObjectId == item.CustomerId);
                            var email = cusContact != null ? (cusContact.Email != null ? cusContact.Email : "") : "";
                            var phone = cusContact != null ? (cusContact.Phone != null ? cusContact.Phone : "") : "";
                            var contacId = cusContact != null
                                ? (cusContact.ContactId != null ? cusContact.ContactId : Guid.Empty)
                                : Guid.Empty;

                            item.CustomerEmail = email;
                            item.CustomerPhone = phone;
                            item.ContactId = contacId;
                            item.StatusName = listAllStatus.FirstOrDefault(c => c.CategoryId == item.StatusId).CategoryName;
                            var statusCode = listAllStatus.FirstOrDefault(c => c.CategoryId == item.StatusId).CategoryCode;
                            switch (statusCode)
                            {
                                case "HDO":
                                    item.BackgroupStatus = "#0f62fe";
                                    break;
                                case "MOI":
                                    item.BackgroupStatus = "#ff3b30";
                                    break;
                            }
                        });
                    }

                    #endregion

                    //Lọc list Order theo list customer và trạng thái đơn hàng
                    if (listCustomerId.Count > 0)
                    {
                        var listOrder = context.CustomerOrder.Where(x =>
                                x.Active == true &&
                                listCustomerId.Contains(x.CustomerId.Value) &&
                                listOrderStatusIdAccept.Contains(x.StatusId.Value))
                            .ToList();

                        //Lấy list Order Detail theo list Order
                        var listOrderId = listOrder.Select(x => x.OrderId).ToList();

                        if (listOrderId.Count > 0)
                        {
                            #region Lấy list khách hàng có doanh thu cao nhất trong tháng hiện tại

                            var newListOrder = listOrder.Where(x => x.OrderDate.Month == DateTime.Now.Month && x.OrderDate.Year == DateTime.Now.Year)
                                .Select(y => new
                                {
                                    CustomerId = y.CustomerId,
                                    Total = appName == "VNS" ? CalculateTotalAmountAfterVatOrder(y.OrderId, y.QuoteId, y.DiscountType, y.DiscountValue,
                                        y.Vat, listOrderDetail, listOrderCost, listPromotionObjectApply) : CalculatorAmount(y.DiscountType.Value, y.DiscountValue.Value, y.Amount.Value)
                                }).ToList();

                            newListOrder = newListOrder.GroupBy(x => x.CustomerId).Select(y => new
                            {
                                CustomerId = y.First().CustomerId,
                                Total = y.Sum(s => s.Total)
                            }).OrderByDescending(z => z.Total).Take(5).ToList();

                            var listCusIdTopRevenueInMonth =
                                newListOrder.Select(x => x.CustomerId).ToList();

                            listCusTopRevenueInMonth = listCustomer1
                                .Where(x => listCusIdTopRevenueInMonth.Contains(x.CustomerId)).Select(y => new CustomerEntityModel
                                {
                                    CustomerId = y.CustomerId,
                                    CustomerName = y.CustomerName,
                                    ContactId = Guid.Empty,
                                    CustomerEmail = "",
                                    CustomerPhone = "",
                                    PersonInChargeId = y.PersonInChargeId,
                                    PicName = y.PersonInChargeId != null ? picInfor.EmployeeName : "",
                                    PicContactId = y.PersonInChargeId != null ? picContact.ContactId : Guid.Empty,
                                    CreatedById = y.CreatedById
                                }).ToList();

                            var listCusTopRevenueContact = listContact.Where(x =>
                                listCusIdTopRevenueInMonth.Contains(x.ObjectId) && x.ObjectType == "CUS").ToList();

                            listCusTopRevenueInMonth.ForEach(item =>
                            {
                                var cusContact =
                                    listCusTopRevenueContact.FirstOrDefault(x => x.ObjectId == item.CustomerId);
                                var email = cusContact != null ? (cusContact.Email != null ? cusContact.Email : "") : "";
                                var phone = cusContact != null ? (cusContact.Phone != null ? cusContact.Phone : "") : "";
                                var contacId = cusContact != null
                                    ? (cusContact.ContactId != null ? cusContact.ContactId : Guid.Empty)
                                    : Guid.Empty;

                                item.CustomerEmail = email;
                                item.CustomerPhone = phone;
                                item.ContactId = contacId;

                                //Doanh thu
                                item.TotalSaleValue = newListOrder.FirstOrDefault(x => x.CustomerId == item.CustomerId)
                                    .Total;
                            });

                            listCusTopRevenueInMonth = listCusTopRevenueInMonth.OrderByDescending(x => x.TotalSaleValue)
                                .Take(5).ToList();

                            #endregion

                            listOrderDetail = listOrderDetail.Where(x => listOrderId.Contains(x.OrderId)).ToList();

                            //Lấy list ProductId theo list OrderDetail
                            var listProductId = listOrderDetail
                                .Where(x => x.ProductId != null && x.ProductId != Guid.Empty).Select(y => y.ProductId)
                                .Distinct()
                                .ToList();

                            //Lấy list Product theo list ProductId
                            var listProduct = context.Product.Where(x => listProductId.Contains(x.ProductId)).ToList();

                            listCusFollowProduct = (from orderDetail in listOrderDetail
                                                    join order in listOrder on orderDetail.OrderId equals order.OrderId
                                                    join product in listProduct on orderDetail.ProductId equals product.ProductId
                                                    join stt in listOrderStatusAccept on order.StatusId equals stt.OrderStatusId
                                                    join proCate in listProductCategory on product.ProductCategoryId equals proCate.ProductCategoryId
                                                    group new { orderDetail, order, product, stt, proCate } by new { order.CustomerId, proCate.ProductCategoryId } into temp
                                                    select new ProductCategoryEntityModel
                                                    {
                                                        CustomerId = temp.Key.CustomerId,
                                                        ProductCategoryId = temp.Key.ProductCategoryId
                                                    }).ToList();

                            listCusFollowProduct.ForEach(item =>
                            {
                                item.ProductCategoryName = GetProductCategoryParent(item.ProductCategoryId, listProductCategory).ProductCategoryName;
                                item.ProductCategoryCode = GetProductCategoryParent(item.ProductCategoryId, listProductCategory).ProductCategoryCode;
                            });

                            listCusFollowProduct = (from lst in listCusFollowProduct
                                                    group lst by new { lst.ProductCategoryCode, lst.CustomerId } into temp
                                                    select new ProductCategoryEntityModel
                                                    {
                                                        CustomerId = temp.Key.CustomerId,
                                                        ProductCategoryCode = temp.Key.ProductCategoryCode,
                                                        ProductCategoryName = temp.First().ProductCategoryName
                                                    }).ToList();

                            listCusFollowProduct = (from temp in listCusFollowProduct
                                                    group temp by temp.ProductCategoryCode into qe
                                                    select new ProductCategoryEntityModel
                                                    {
                                                        ProductCategoryCode = qe.Key,
                                                        ProductCategoryName = qe.First().ProductCategoryName,
                                                        CountCustomer = qe.Count(),
                                                    }).ToList();
                        }
                    }
                }

                if (parameter.CustomerName != null && parameter.CustomerName != "")
                {
                    listCusIsNewest = listCusIsNewest.Where(x =>
                            x.CustomerName.ToLower().Trim().Contains(parameter.CustomerName.ToLower().Trim()))
                        .ToList();
                    listCusTopRevenueInMonth = listCusTopRevenueInMonth
                        .Where(x => x.CustomerName.ToLower().Trim().Contains(parameter.CustomerName.ToLower().Trim()))
                        .ToList();
                    listCusIdentification = listCusIdentification
                        .Where(x => x.CustomerName.ToLower().Trim().Contains(parameter.CustomerName.ToLower().Trim()))
                        .ToList();
                }

                listCusIdentification = listCusIdentification.OrderByDescending(c => c.CreatedDate).ToList();
                return new GetDashBoardCustomerResult()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = "Success",
                    ListCusFollowProduct = listCusFollowProduct,
                    ListTopPic = listTopPic,
                    ListCusCreatedInThisYear = listCusCreatedInThisYear,
                    ListCusTopRevenueInMonth = listCusTopRevenueInMonth,
                    ListCusIdentification = listCusIdentification,
                    ListCusFree = listCusFree //khách hàng tự do
                };
            }
            catch (Exception e)
            {
                return new GetDashBoardCustomerResult()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        //Tổng tiền sau thuế = Tổng GTHH bán ra + Tổng thành tiền nhân công - Tổng chiết khấu - Tổng khuyến mại + Tổng thuế VAT + Tổng chi phí
        private decimal? CalculateTotalAmountAfterVatOrder(Guid orderId, Guid? quoteId, bool? discountType,
            decimal? discountValue, decimal vat,
            List<CustomerOrderDetail> listCustomerOrderDetails,
            List<OrderCostDetail> listOrderCostDetails,
            List<PromotionObjectApply> listPromotionObjectApply)
        {
            decimal? result = 0;
            decimal? amount = 0;
            decimal? totalSumAmountLabor = 0;
            decimal? totalAmountDiscount = 0;
            decimal? totalAmountPromotion = 0;
            decimal? totalAmountVat = 0;
            decimal? amountPriceCost = 0;
            bool hasDiscount = false;
            bool hasVat = false;

            var quoteDetailList = listCustomerOrderDetails.Where(x => x.Active == true && x.OrderId == orderId).ToList();

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
            if (quoteId != null)
            {
                var promotionObjectApplyList = listPromotionObjectApply.Where(x => x.ObjectId == quoteId && x.ObjectType == "QUOTE").ToList();

                promotionObjectApplyList.ForEach(x =>
                {
                    if (x.ProductId == null)
                    {
                        totalAmountPromotion += x.Amount;
                    }
                });
            }
            else
            {
                totalAmountPromotion = 0;
            }

            //Tổng chi phí
            var quoteCostDetailList = listOrderCostDetails.Where(x => x.Active == true && x.OrderId == orderId).ToList();

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

            return result;
        }

        private decimal CalculatorAmount(bool DiscountType, decimal DiscountValue, decimal Amount)
        {
            decimal result;
            if (DiscountType)
            {
                result = Amount - (Amount * DiscountValue) / 100;
                return result;
            }

            result = Amount - DiscountValue;
            return result;
        }

        public int CheckCountInformationCustomer(
                Guid? CustomerId,
                List<Lead> lead,
                List<Quote> quote,
                List<CustomerOrder> customerOrder,
                List<ReceiptInvoiceMapping> receiptInvoiceMapping,
                List<BankReceiptInvoiceMapping> bankReceiptInvoiceMapping,
                List<CustomerCareCustomer> customerCareCustomer,
                List<PayableInvoiceMapping> payableInvoiceMapping,
                List<BankPayableInvoiceMapping> bankPayableInvoiceMapping
            )
        {
            var customerCount = quote.Count(l => l.ObjectTypeId == CustomerId);
            customerCount += lead.Count(x => x.CustomerId == CustomerId);
            //customerCount += note.Where(n => n.ObjectId == CustomerId).Count();
            customerCount += customerOrder.Count(c => c.CustomerId == CustomerId);
            customerCount += receiptInvoiceMapping.Count(c => c.ObjectId == CustomerId);
            customerCount += bankReceiptInvoiceMapping.Count(c => c.ObjectId == CustomerId);
            customerCount += payableInvoiceMapping.Count(c => c.ObjectId == CustomerId);
            customerCount += bankPayableInvoiceMapping.Count(c => c.ObjectId == CustomerId);
            customerCount += customerCareCustomer.Count(c => c.CustomerId == CustomerId);

            return customerCount;
        }

        public int CheckCustomerReference(
            List<LeadEntityModel> lead,
            List<QuoteEntityModel> quote
        )
        {
            var customerCount = 0;

            if (lead.Count > 0)
            {
                customerCount++;
            }

            if (quote.Count > 0)
            {
                customerCount++;
            }

            return customerCount;
        }

        public GetListCustomerResult GetListCustomer(GetListCustomerParameter parameter)
        {
            try
            {
                var listCategoryEntityModel = new List<CategoryEntityModel>();
                var listSourceEntityModel = new List<CategoryEntityModel>();
                var listStatusCareEntityModel = new List<CategoryEntityModel>();
                var listCustomerServiceLevel = new List<CustomerServiceLevelEntityModel>();
                var listAreaEntityModel = new List<AreaEntityModel>();


                var listAreaEntity = context.GeographicalArea.Where(x => x.Active).ToList();
                listAreaEntity.ForEach(x =>
                {
                    listAreaEntityModel.Add(new AreaEntityModel
                    {
                        AreaId = x.GeographicalAreaId,
                        AreaCode = x.GeographicalAreaCode,
                        AreaName = x.GeographicalAreaName
                    });

                });

                var categoryTypeId = context.CategoryType.FirstOrDefault(w => w.CategoryTypeCode == parameter.CategoryTypeCode)?.CategoryTypeId;
                var listCategoryEntity = context.Category.Where(w => w.CategoryTypeId == categoryTypeId).ToList();
                listCategoryEntity?.ForEach(x =>
                {
                    listCategoryEntityModel.Add(new CategoryEntityModel
                    {
                        CategoryTypeId = x.CategoryTypeId,
                        CategoryId = x.CategoryId,
                        CategoryName = x.CategoryName,
                        CategoryCode = x.CategoryCode,
                        IsDefault = x.IsDefauld
                    });
                });

                var sourceCategoryTypeId = context.CategoryType.FirstOrDefault(w => w.CategoryTypeCode == "IVF")?.CategoryTypeId;
                var listSourceCategoryEntity = context.Category.Where(w => w.CategoryTypeId == sourceCategoryTypeId).ToList();
                listSourceCategoryEntity?.ForEach(x =>
                {
                    listSourceEntityModel.Add(new CategoryEntityModel
                    {
                        CategoryTypeId = x.CategoryTypeId,
                        CategoryId = x.CategoryId,
                        CategoryName = x.CategoryName,
                        CategoryCode = x.CategoryCode,
                        IsDefault = x.IsDefauld,
                    });
                });

                var listCustomerServiceLevelEntity = context.CustomerServiceLevel
                    .Select(y => new CustomerServiceLevelEntityModel
                    {
                        CustomerServiceLevelId = y.CustomerServiceLevelId,
                        CustomerServiceLevelName = y.CustomerServiceLevelName,
                        CustomerServiceLevelCode = y.CustomerServiceLevelCode,
                        MinimumSaleValue = y.MinimumSaleValue,
                        CreatedById = y.CreatedById,
                        CreatedDate = y.CreatedDate,
                        UpdatedDate = y.UpdatedDate,
                        Active = y.Active,
                    }).ToList();

                listCustomerServiceLevelEntity?.ForEach(level =>
                {
                    listCustomerServiceLevel.Add(new CustomerServiceLevelEntityModel
                    {
                        CustomerServiceLevelId = level.CustomerServiceLevelId,
                        CustomerServiceLevelName = level.CustomerServiceLevelName
                    });
                });

                var statusCareTypeId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "TTCSKH")?.CategoryTypeId;
                var listStatusCare = context.Category.Where(c => c.CategoryTypeId == statusCareTypeId).ToList();
                listStatusCare.ForEach(x =>
                {
                    listStatusCareEntityModel.Add(new CategoryEntityModel
                    {
                        CategoryId = x.CategoryId,
                        CategoryName = x.CategoryName,
                        CategoryCode = x.CategoryCode,
                        CategoryTypeId = x.CategoryTypeId,
                        IsDefault = x.IsDefauld,
                    });
                });

                var listProvince = context.Province.Select(x => new ProvinceEntityModel { 
                    ProvinceId = x.ProvinceId,
                    ProvinceName = x.ProvinceName
                }).ToList();

                return new GetListCustomerResult()
                {
                    ListProvince = listProvince,
                    ListAreaModel = listAreaEntityModel,
                    ListSourceModel = listSourceEntityModel,
                    ListStatusCareModel = listStatusCareEntityModel,
                    ListCategoryModel = listCategoryEntityModel,
                    ListCustomerServiceLevelModel = listCustomerServiceLevel,
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = "OK"
                };
            }
            catch (Exception e)
            {
                return new GetListCustomerResult()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public CreateCustomerMasterDataResult CreateCustomerMasterData(CreateCustomerMasterDataParameter parameter)
        {
            try
            {
                //result
                var ListCustomerGroupEntity = new List<CategoryEntityModel>();
                var ListEnterPriseTypeEntity = new List<CategoryEntityModel>();
                var ListBusinessScaleEntity = new List<CategoryEntityModel>();
                var ListPositionEntity = new List<CategoryEntityModel>();
                var ListBusinessLocalEntity = new List<CategoryEntityModel>();
                var ListMainBusinessEntity = new List<CategoryEntityModel>();
                var ListProvinceEntityModel = new List<DataAccess.Models.Address.ProvinceEntityModel>();
                var ListDistrictEntityModel = new List<DataAccess.Models.Address.DistrictEntityModel>();
                var ListWardEntityModel = new List<DataAccess.Models.Address.WardEntityModel>();
                var ListEmployeeEntityModel = new List<DataAccess.Models.Employee.EmployeeEntityModel>();
                var ListCustomerCode = new List<string>();
                var ListCustomerTax = new List<string>();
                var listParticipants = new List<EmployeeEntityModel>();

                var categoryTypeList = context.CategoryType.Where(cty => cty.Active == true).ToList();
                var categoryList = context.Category.Where(ct => ct.Active == true).ToList();
                var employeeList = context.Employee.Where(emp => emp.Active == true).ToList();
                var districsList = context.District.Where(emp => emp.Active == true).ToList();
                var wardsList = context.Ward.Where(emp => emp.Active == true).ToList();

                #region Define Category Code
                var listCategoryCode = new List<string>();
                var customerGroupCode = "NHA"; //nhóm khách hàng
                listCategoryCode.Add(customerGroupCode);
                var portalUserCode = "PortalUser"; //loại portalUser
                #endregion

                #region Get data from Category table
                var listCategoryTypeEntity = categoryTypeList.Where(w => listCategoryCode.Contains(w.CategoryTypeCode) && w.Active == true).ToList();
                var listCateTypeId = new List<Guid>();
                listCategoryTypeEntity?.ForEach(type =>
                {
                    listCateTypeId.Add(type.CategoryTypeId);
                });
                var listCategoryEntity = categoryList.Where(w => listCateTypeId.Contains(w.CategoryTypeId) && w.Active == true).ToList(); //list master data của category

                //get customer group
                var customerGroupTypeId = listCategoryTypeEntity.Where(w => w.CategoryTypeCode == customerGroupCode).FirstOrDefault()?.CategoryTypeId;
                var listCustomerGroupEntity = listCategoryEntity.Where(w => w.CategoryTypeId == customerGroupTypeId && w.CategoryCode != "POR").ToList(); //loại Khách hàng Portal                                              
                listCustomerGroupEntity?.ForEach(group =>
                {
                    ListCustomerGroupEntity.Add(new CategoryEntityModel
                    {
                        CategoryId = group.CategoryId,
                        CategoryName = group.CategoryName,
                        CategoryCode = group.CategoryCode,
                        CategoryTypeId = group.CategoryTypeId,
                        IsDefault = group.IsDefauld
                    });
                });
                #endregion

                #region Get Employee Care Staff List

                var listEmployeeEntity = employeeList.Where(w => w.Active == true && w.EmployeeCode != portalUserCode).ToList(); //loai portal user

                var employeeById = listEmployeeEntity.Where(w => w.EmployeeId == parameter.EmployeeId).FirstOrDefault();
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
                    var listEmployeeFiltered = listEmployeeEntity.Where(w => listOrganizationChildrenId.Contains(w.OrganizationId)).Select(w => new
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
                            EmployeeCode = emp.EmployeeCode
                        });
                    });
                }
                else
                {
                    //Nhân viên: chỉ lấy nhân viên đó
                    var employeeId = listEmployeeEntity.Where(e => e.EmployeeId == parameter.EmployeeId).FirstOrDefault();
                    ListEmployeeEntityModel.Add(new Models.Employee.EmployeeEntityModel
                    {
                        EmployeeId = employeeId.EmployeeId,
                        EmployeeName = employeeId.EmployeeName,
                        EmployeeCode = employeeId.EmployeeCode
                    });
                }

                #endregion

                #region Get List Customer Code
                var listCustomerCodeEntity = context.Customer.Where(w => w.Active == true).Select(w => new
                {
                    CustomerCode = w.CustomerCode
                }).Distinct().ToList();
                listCustomerCodeEntity.ForEach(code =>
                {
                    ListCustomerCode.Add(code.CustomerCode);
                });
                #endregion

                #region Get Province
                ListProvinceEntityModel = context.Province.Select(p => new ProvinceEntityModel()
                {
                    ProvinceId = p.ProvinceId,
                    ProvinceName = p.ProvinceName,
                    ProvinceCode = p.ProvinceCode,
                    ProvinceType = p.ProvinceType
                }).OrderBy(p => p.ProvinceName).ToList();
                #endregion

                #region Get Position
                //var categoryTypeId_9 = categoryTypeList.FirstOrDefault(x => x.CategoryTypeCode == "CVU").CategoryTypeId;
                //ListPositionEntity = categoryList.Where(x => x.CategoryTypeId == categoryTypeId_9).Select(y =>
                //    new CategoryEntityModel
                //    {
                //        CategoryId = y.CategoryId,
                //        CategoryCode = y.CategoryCode,
                //        CategoryName = y.CategoryName
                //    }).ToList();
                #endregion

                #region Get List Customer Code
                var listCustomerTaxEntity = context.Contact.Where(w => w.Active == true && w.ObjectType == "CUS" && w.TaxCode != null && w.TaxCode.Trim() != "").Select(w => new
                {
                    TaxCode = w.TaxCode
                }).Distinct().ToList();
                listCustomerTaxEntity.ForEach(code =>
                {
                    ListCustomerTax.Add(code.TaxCode);
                });
                #endregion

                ListProvinceEntityModel.ForEach(p =>
                {
                    var districtList = districsList.Where(d => d.ProvinceId == p.ProvinceId)
                        .Select(d => new DistrictEntityModel()
                        {
                            DistrictId = d.DistrictId,
                            DistrictName = d.DistrictName,
                            DistrictCode = d.DistrictCode,
                            DistrictType = d.DistrictType,
                            ProvinceId = d.ProvinceId
                        }).OrderBy(d => d.DistrictName).ToList();

                    districtList.ForEach(d =>
                    {
                        var wardList = wardsList.Where(w => w.DistrictId == d.DistrictId).Select(w =>
                            new WardEntityModel()
                            {
                                WardId = w.WardId,
                                WardName = w.WardName,
                                WardCode = w.WardCode,
                                WardType = w.WardType,
                                DistrictId = w.DistrictId
                            }).OrderBy(w => w.WardName).ToList();
                        d.WardList = wardList;
                    });

                    p.DistrictList = districtList;
                });

                var listArea = context.GeographicalArea.Where(c => c.Active == true)
                    .Select(m => new GeographicalAreaEntityModel
                    {
                        GeographicalAreaId = m.GeographicalAreaId,
                        GeographicalAreaCode = m.GeographicalAreaCode,
                        GeographicalAreaName = m.GeographicalAreaName
                    }).ToList();

                #region Danh sách khách hàng

                var ListCustomer = new List<CustomerEntityModel>();
                var statusCustomerType =
                    context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "THA").CategoryTypeId;
                var statusCustomer = context.Category
                    .FirstOrDefault(x => x.CategoryCode == "HDO" && x.CategoryTypeId == statusCustomerType).CategoryId;
                var listStatusCustomer = context.Category
                    .Where(x => x.CategoryTypeId == statusCustomerType).ToList();

                if (isManage == true)
                {
                    //Quản lí: lấy tất cả nhân viên phòng ban đó và phòng ban dưới cấp
                    var currentOrganization = employeeById.OrganizationId;
                    List<Guid?> listOrganizationChildrenId = new List<Guid?>();
                    listOrganizationChildrenId.Add(currentOrganization);
                    var organizationList = context.Organization.Where(w => w.Active == true).ToList();
                    getOrganizationChildrenId(organizationList, currentOrganization, listOrganizationChildrenId);
                    var listEmployeeId = listEmployeeEntity
                        .Where(w => listOrganizationChildrenId.Contains(w.OrganizationId)).Select(w => w.EmployeeId)
                        .ToList();

                    ListCustomer = context.Customer
                        .Where(x => x.Active == true && x.CustomerType == 1 &&
                                    //x.StatusId == statusCustomer &&
                                    listEmployeeId.Contains(x.PersonInChargeId.Value)).Select(y =>
                            new CustomerEntityModel
                            {
                                CustomerId = y.CustomerId,
                                CustomerName = y.CustomerName,
                                StatusCode = listStatusCustomer.FirstOrDefault(p => p.CategoryId == y.StatusId).CategoryCode
                            }).ToList();
                }
                else
                {
                    //Nhân viên: chỉ lấy nhân viên đó
                    ListCustomer = context.Customer
                        .Where(x => x.Active == true && x.CustomerType == 1 &&
                                    //x.StatusId == statusCustomer &&
                                    x.PersonInChargeId == parameter.EmployeeId).Select(y =>
                            new CustomerEntityModel
                            {
                                CustomerId = y.CustomerId,
                                CustomerName = y.CustomerName,
                                StatusCode = listStatusCustomer.FirstOrDefault(p => p.CategoryId == y.StatusId).CategoryCode
                            }).ToList();
                }

                var listStaffCharge = employeeList
                                        .Select(x => new EmployeeEntityModel
                                        {
                                            EmployeeId = x.EmployeeId,
                                            EmployeeName = x.EmployeeName
                                        }).ToList();

                #endregion

                return new CreateCustomerMasterDataResult()
                {
                    ListCustomerGroup = ListCustomerGroupEntity.OrderBy(lg => lg.CategoryName).ToList(),
                    ListEnterPriseType = ListEnterPriseTypeEntity,
                    ListBusinessScale = ListBusinessScaleEntity,
                    ListPosition = ListPositionEntity,
                    ListBusinessLocal = ListBusinessLocalEntity,
                    ListMainBusiness = ListMainBusinessEntity,
                    ListProvinceModel = ListProvinceEntityModel,
                    ListDistrictModel = ListDistrictEntityModel,
                    ListWardModel = ListWardEntityModel,
                    ListEmployeeModel = ListEmployeeEntityModel,
                    ListCustomerCode = ListCustomerCode,
                    ListCustomerTax = ListCustomerTax,
                    ListArea = listArea,
                    ListCustomer = ListCustomer,
                    ListStaffCharge = listStaffCharge,
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = "OK",
                };
            }
            catch (Exception e)
            {
                return new CreateCustomerMasterDataResult()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
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

        public CheckDuplicateCustomerAllTypeResult CheckDuplicateCustomerAllType(CheckDuplicateCustomerAllTypeParameter request)
        {
            try
            {
                var isDuplicateLead = false;
                var isDuplicateCustomer = false;
                var duplicateLeadModel = new LeadEntityModel();
                var duplicateLeadContactModel = new ContactEntityModel();
                var duplicateCustomerModel = new CustomerEntityModel();
                var duplicateCustomerContactModel = new ContactEntityModel();

                var KHDCode = "KHD"; //Mã Category lead đã ký hợp đồng
                var NDOCode = "NDO"; //Mã Category lead ngừng theo dõi
                var leadKHDId = context.Category.Where(w => w.CategoryCode == KHDCode).FirstOrDefault().CategoryId; //Id Lead đã ký hợp đồng
                var leadNDOId = context.Category.Where(w => w.CategoryCode == NDOCode).FirstOrDefault().CategoryId;//Id Lead ngừng theo dõi

                //master data
                var listLeadEntity = context.Lead.Where(w => w.StatusId != leadKHDId && w.StatusId != leadNDOId && w.Active == true).ToList();
                var listContact = context.Contact.Where(w => w.Active == true).ToList();
                //&& !string.IsNullOrWhiteSpace(w.Email)
                //&& !string.IsNullOrWhiteSpace(w.Phone)).ToList();
                var listLeadId = new List<Guid?>();

                listLeadEntity.ForEach(lead =>
                {
                    listLeadId.Add(lead.LeadId);
                });

                //filtered
                var listLeadContactEntity = listContact.Where(w => listLeadId.Contains(w.ObjectId)).ToList(); //list contact theo lead

                var listCustomerContact = listContact.Where(w => w.ObjectType == "CUS").ToList();
                Guid leadId = Guid.Empty;
                Guid customerId = Guid.Empty;

                #region Check On Save Customer
                if (request.IsCheckOnSave == true)
                {
                    var duplicateLead = listLeadContactEntity.Where(w => w.Phone == request.ContactModel.Phone.Trim()
                                                                      || w.Email == request.ContactModel.Email.Trim()).FirstOrDefault();

                    // remove lead if already checked
                    if (request.IsCheckedLead == true)
                    {
                        duplicateLead = listLeadContactEntity.Where(w => (w.Phone == request.ContactModel.Phone.Trim()
                                                                     || w.Email == request.ContactModel.Email.Trim())
                                                                     && w.ObjectId != request.ContactModel.ObjectId).FirstOrDefault();
                    }

                    if (duplicateLead != null)
                    {
                        isDuplicateLead = true;
                        leadId = duplicateLead.ObjectId;
                        var duplicateLeadModelResult = listLeadEntity.Where(w => w.LeadId == leadId).FirstOrDefault();
                        duplicateLeadModel = new LeadEntityModel(duplicateLeadModelResult);
                        var picId = duplicateLeadModel?.PersonInChargeId;
                        //nếu không có nhân viên phụ trách và  người đăng nhập khác người tạo thì báo lỗi
                        var user = context.User.Where(w => w.EmployeeId == request.EmployeeId).FirstOrDefault();//người đang đăng nhập
                        if (picId == null && duplicateLead.CreatedById != user.UserId)
                        {
                            var createdUser = context.User.Where(w => w.UserId.ToString() == duplicateLeadModel.CreatedById).FirstOrDefault();
                            if (createdUser != null)
                            {
                                var employee = context.Employee.FirstOrDefault(f => f.EmployeeId == createdUser.EmployeeId);
                                return new CheckDuplicateCustomerAllTypeResult
                                {
                                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                                    MessageCode = "Khách hàng đã tồn tại trên hệ thống và được tạo bởi " + employee.EmployeeCode + "- " + employee.EmployeeName
                                };
                            }
                        }
                        //nếu nhân viên phụ trách khác nhân viên đang đăng nhập thì báo lỗi
                        if (picId != null && picId != request.EmployeeId)
                        {
                            var employee = context.Employee.FirstOrDefault(f => f.EmployeeId == picId);
                            return new CheckDuplicateCustomerAllTypeResult
                            {
                                StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                                MessageCode = "Khách hàng đã tồn tại trên hệ thống và đang được phụ trách bởi " + employee.EmployeeCode + "- " + employee.EmployeeName
                            };
                        }

                        var duplicateLeadContactModelEntity = listLeadContactEntity.Where(w => w.ObjectId == leadId).FirstOrDefault();

                        duplicateLeadContactModel.ContactId = duplicateLeadContactModelEntity.ContactId;
                        duplicateLeadContactModel.ObjectId = duplicateLeadContactModelEntity.ObjectId;
                        duplicateLeadContactModel.CompanyName = duplicateLeadContactModelEntity.CompanyName ?? "";
                        duplicateLeadContactModel.Email = duplicateLeadContactModelEntity.Email ?? "";
                        duplicateLeadContactModel.WorkEmail = duplicateLeadContactModelEntity.WorkEmail ?? "";
                        duplicateLeadContactModel.OtherEmail = duplicateLeadContactModelEntity.OtherEmail ?? "";
                        duplicateLeadContactModel.Phone = duplicateLeadContactModelEntity.Phone ?? "";
                        duplicateLeadContactModel.WorkPhone = duplicateLeadContactModelEntity.WorkPhone ?? "";
                        duplicateLeadContactModel.OtherPhone = duplicateLeadContactModelEntity.OtherPhone ?? "";
                        duplicateLeadContactModel.ProvinceId = duplicateLeadContactModelEntity.ProvinceId;
                        duplicateLeadContactModel.DistrictId = duplicateLeadContactModelEntity.DistrictId;
                        duplicateLeadContactModel.WardId = duplicateLeadContactModelEntity.WardId;
                        duplicateLeadContactModel.Address = duplicateLeadContactModelEntity.Address ?? "";
                        duplicateLeadContactModel.FirstName = duplicateLeadContactModelEntity.FirstName ?? "";
                        duplicateLeadContactModel.LastName = duplicateLeadContactModelEntity.LastName ?? "";
                        duplicateLeadContactModel.Gender = duplicateLeadContactModelEntity.Gender ?? "NAM";

                        var company = context.Company.Where(w => w.CompanyId == duplicateLeadModel.CompanyId).FirstOrDefault()?.CompanyName;

                        duplicateLeadContactModel.CompanyName = company ?? "";

                        return new CheckDuplicateCustomerAllTypeResult()
                        {
                            IsDuplicateLead = isDuplicateLead,
                            IsDuplicateCustomer = isDuplicateCustomer,
                            DuplicateLeadModel = duplicateLeadModel,
                            DuplicateLeadContactModel = duplicateLeadContactModel,
                            StatusCode = System.Net.HttpStatusCode.OK,
                            MessageCode = "OK"
                        };
                    }

                    var duplicateCustomer = listCustomerContact.Where(w => w.Phone == request.ContactModel.Phone.Trim()
                                                                        || w.Email == request.ContactModel.Email.Trim()).FirstOrDefault();
                    if (duplicateCustomer != null)
                    {
                        isDuplicateCustomer = true;
                        customerId = duplicateCustomer.ObjectId;

                        var duplicateCustomerEntityModel = listContact.Where(w => w.ObjectId == customerId).FirstOrDefault();
                        if (duplicateCustomerEntityModel != null)
                        {
                            var customer = context.Customer.Where(w => w.CustomerId == duplicateCustomerEntityModel.ObjectId).FirstOrDefault();
                            var picId = customer?.PersonInChargeId;
                            //nếu không có nhân viên phụ trách và  người đăng nhập khác người tạo thì báo lỗi
                            var user = context.User.Where(w => w.EmployeeId == request.EmployeeId).FirstOrDefault();
                            if (picId == null && customer.CreatedById != user.UserId)
                            {
                                var createdUser = context.User.Where(w => w.UserId == customer.CreatedById).FirstOrDefault();
                                if (createdUser != null)
                                {
                                    var employee = context.Employee.FirstOrDefault(f => f.EmployeeId == createdUser.EmployeeId);
                                    return new CheckDuplicateCustomerAllTypeResult
                                    {
                                        StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                                        MessageCode = "Khách hàng đã tồn tại trên hệ thống và được tạo bởi " + employee.EmployeeCode + "- " + employee.EmployeeName
                                    };
                                }
                            }
                            //nếu nhân viên bán hàng khác nhân viên đang đăng nhập thì báo lỗi                        
                            if (picId != null && picId != request.EmployeeId)
                            {
                                var employee = context.Employee.FirstOrDefault(f => f.EmployeeId == picId);
                                return new CheckDuplicateCustomerAllTypeResult
                                {
                                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                                    MessageCode = "Khách hàng đã tồn tại trên hệ thống và đang được phụ trách bởi " + employee.EmployeeCode + "- " + employee.EmployeeName
                                };
                            }

                            duplicateCustomerContactModel.ContactId = duplicateCustomerEntityModel.ContactId;
                            duplicateCustomerContactModel.ObjectId = duplicateCustomerEntityModel.ObjectId;
                        }

                        return new CheckDuplicateCustomerAllTypeResult()
                        {
                            IsDuplicateLead = isDuplicateLead,
                            IsDuplicateCustomer = isDuplicateCustomer,
                            DuplicateLeadModel = duplicateLeadModel,
                            DuplicateLeadContactModel = duplicateLeadContactModel,
                            DuplicateCustomerContactModel = duplicateCustomerContactModel,
                            StatusCode = System.Net.HttpStatusCode.OK,
                            MessageCode = "OK"
                        };
                    }

                    return new CheckDuplicateCustomerAllTypeResult()
                    {
                        IsDuplicateLead = isDuplicateLead,
                        IsDuplicateCustomer = isDuplicateCustomer,
                        DuplicateLeadModel = duplicateLeadModel,
                        DuplicateLeadContactModel = duplicateLeadContactModel,
                        DuplicateCustomerContactModel = duplicateCustomerContactModel,
                        StatusCode = System.Net.HttpStatusCode.OK,
                        MessageCode = "OK",
                    };

                }
                #endregion

                #region Check Duplicate Customer      
                var listCustomerType = context.Customer.Where(c => c.Active == true && c.CustomerType == request.CustomerType).Select(c => c.CustomerId).ToList();
                var listCustomerTypeContactEntity = listContact.Where(w => listCustomerType.Contains(w.ObjectId) && w.ObjectType == "CUS").ToList(); //list contact theo lead
                if (!string.IsNullOrWhiteSpace(request.ContactModel.Phone))
                {
                    var duplicate = listCustomerTypeContactEntity.Where(w => w.Phone == request.ContactModel.Phone.Trim()).FirstOrDefault();
                    if (duplicate != null)
                    {
                        isDuplicateCustomer = true;
                        customerId = duplicate.ObjectId;
                    }
                }
                if (!string.IsNullOrWhiteSpace(request.ContactModel.OtherPhone))
                {
                    var duplicate = listCustomerTypeContactEntity.Where(w => w.OtherPhone == request.ContactModel.OtherPhone.Trim()).FirstOrDefault();
                    if (duplicate != null)
                    {
                        isDuplicateCustomer = true;
                        customerId = duplicate.ObjectId;
                    }
                }
                if (!string.IsNullOrWhiteSpace(request.ContactModel.WorkPhone))
                {
                    var duplicate = listCustomerTypeContactEntity.Where(w => w.WorkPhone == request.ContactModel.WorkPhone.Trim()).FirstOrDefault();
                    if (duplicate != null)
                    {
                        isDuplicateCustomer = true;
                        customerId = duplicate.ObjectId;
                    }
                }
                if (!string.IsNullOrWhiteSpace(request.ContactModel.Email))
                {
                    var duplicate = listCustomerTypeContactEntity.Where(w => w.Email == request.ContactModel.Email.Trim()).FirstOrDefault();
                    if (duplicate != null)
                    {
                        isDuplicateCustomer = true;
                        customerId = duplicate.ObjectId;
                    }
                }
                if (!string.IsNullOrWhiteSpace(request.ContactModel.OtherEmail))
                {
                    var duplicate = listCustomerTypeContactEntity.Where(w => w.OtherEmail == request.ContactModel.OtherEmail.Trim()).FirstOrDefault();
                    if (duplicate != null)
                    {
                        isDuplicateCustomer = true;
                        customerId = duplicate.ObjectId;
                    }
                }
                if (!string.IsNullOrWhiteSpace(request.ContactModel.WorkEmail))
                {
                    var duplicate = listCustomerTypeContactEntity.Where(w => w.WorkEmail == request.ContactModel.WorkEmail.Trim()).FirstOrDefault();
                    if (duplicate != null)
                    {
                        isDuplicateCustomer = true;
                        customerId = duplicate.ObjectId;
                    }
                }

                if (isDuplicateCustomer == true)
                {
                    var duplicateCustomerEntityModel = listContact.Where(w => w.ObjectId == customerId).FirstOrDefault();
                    if (duplicateCustomerEntityModel != null)
                    {
                        var customer = context.Customer.Where(w => w.CustomerId == duplicateCustomerEntityModel.ObjectId).FirstOrDefault();
                        var picId = customer?.PersonInChargeId;
                        //nếu không có nhân viên phụ trách và  người đăng nhập khác người tạo thì báo lỗi
                        var user = context.User.Where(w => w.EmployeeId == request.EmployeeId).FirstOrDefault();
                        if (picId == null && customer.CreatedById != user.UserId)
                        {
                            var createdUser = context.User.Where(w => w.UserId == customer.CreatedById).FirstOrDefault();
                            if (createdUser != null)
                            {
                                var employee = context.Employee.FirstOrDefault(f => f.EmployeeId == createdUser.EmployeeId);
                                return new CheckDuplicateCustomerAllTypeResult
                                {
                                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                                    MessageCode = "Khách hàng đã tồn tại trên hệ thống và được tạo bởi " + employee.EmployeeCode + "- " + employee.EmployeeName
                                };
                            }
                        }
                        //nếu nhân viên bán hàng khác nhân viên đang đăng nhập thì báo lỗi                     
                        if (picId != null && picId != request.EmployeeId)
                        {
                            var employee = context.Employee.FirstOrDefault(f => f.EmployeeId == picId);
                            return new CheckDuplicateCustomerAllTypeResult
                            {
                                StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                                MessageCode = "Khách hàng đã tồn tại trên hệ thống và đang được phụ trách bởi " + employee.EmployeeCode + "- " + employee.EmployeeName
                            };
                        }

                        duplicateCustomerContactModel.ContactId = duplicateCustomerEntityModel.ContactId;
                        duplicateCustomerContactModel.ObjectId = duplicateCustomerEntityModel.ObjectId;
                    }
                }
                #endregion

                #region Check Duplicate Lead
                if (!string.IsNullOrWhiteSpace(request.ContactModel.Phone))
                {
                    var duplicate = listLeadContactEntity.Where(w => w.Phone == request.ContactModel.Phone.Trim()).FirstOrDefault();
                    if (duplicate != null)
                    {
                        isDuplicateLead = true;
                        leadId = duplicate.ObjectId;
                    }
                }

                if (!string.IsNullOrWhiteSpace(request.ContactModel.Email))
                {
                    var duplicate = listLeadContactEntity.Where(w => w.Email == request.ContactModel.Email.Trim()).FirstOrDefault();
                    if (duplicate != null)
                    {
                        isDuplicateLead = true;
                        leadId = duplicate.ObjectId;
                    }
                }

                if (isDuplicateLead == true)
                {
                    var duplicateLeadModelResult1 = listLeadEntity.Where(w => w.LeadId == leadId).FirstOrDefault();
                    duplicateLeadModel = new LeadEntityModel(duplicateLeadModelResult1);

                    var picId = duplicateLeadModel?.PersonInChargeId;
                    //nếu không có nhân viên phụ trách và  người đăng nhập khác người tạo thì báo lỗi
                    var user = context.User.Where(w => w.EmployeeId == request.EmployeeId).FirstOrDefault();
                    if (picId == null && duplicateLeadModel.CreatedById != user.UserId.ToString())
                    {
                        var createdUser = context.User.Where(w => w.UserId.ToString() == duplicateLeadModel.CreatedById).FirstOrDefault();
                        if (createdUser != null)
                        {
                            var employee = context.Employee.FirstOrDefault(f => f.EmployeeId == createdUser.EmployeeId);
                            return new CheckDuplicateCustomerAllTypeResult
                            {
                                StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                                MessageCode = "Khách hàng đã tồn tại trên hệ thống và được tạo bởi " + employee.EmployeeCode + "- " + employee.EmployeeName
                            };
                        }
                    }
                    //nếu nhân viên phụ trách khác nhân viên đang đăng nhập thì báo lỗi             
                    if (picId != null && picId != request.EmployeeId)
                    {
                        var employee = context.Employee.FirstOrDefault(f => f.EmployeeId == picId);
                        return new CheckDuplicateCustomerAllTypeResult
                        {
                            StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                            MessageCode = "Khách hàng đã tồn tại trên hệ thống và đang được phụ trách bởi " + employee.EmployeeCode + "- " + employee.EmployeeName
                        };
                    }

                    var duplicateLeadContactModelEntity = listLeadContactEntity.Where(w => w.ObjectId == leadId).FirstOrDefault();
                    duplicateLeadContactModel.ContactId = duplicateLeadContactModelEntity.ContactId;
                    duplicateLeadContactModel.ObjectId = duplicateLeadContactModelEntity.ObjectId;
                    duplicateLeadContactModel.CompanyName = duplicateLeadContactModelEntity.CompanyName ?? "";
                    duplicateLeadContactModel.Email = duplicateLeadContactModelEntity.Email ?? "";
                    duplicateLeadContactModel.WorkEmail = duplicateLeadContactModelEntity.WorkEmail ?? "";
                    duplicateLeadContactModel.OtherEmail = duplicateLeadContactModelEntity.OtherEmail ?? "";
                    duplicateLeadContactModel.Phone = duplicateLeadContactModelEntity.Phone ?? "";
                    duplicateLeadContactModel.WorkPhone = duplicateLeadContactModelEntity.WorkPhone ?? "";
                    duplicateLeadContactModel.OtherPhone = duplicateLeadContactModelEntity.OtherPhone ?? "";
                    duplicateLeadContactModel.ProvinceId = duplicateLeadContactModelEntity.ProvinceId;
                    duplicateLeadContactModel.DistrictId = duplicateLeadContactModelEntity.DistrictId;
                    duplicateLeadContactModel.WardId = duplicateLeadContactModelEntity.WardId;
                    duplicateLeadContactModel.Address = duplicateLeadContactModelEntity.Address ?? "";
                    duplicateLeadContactModel.FirstName = duplicateLeadContactModelEntity.FirstName ?? "";
                    duplicateLeadContactModel.LastName = duplicateLeadContactModelEntity.LastName ?? "";
                    duplicateLeadContactModel.Gender = duplicateLeadContactModelEntity.Gender ?? "NAM";

                    var company = context.Company.Where(w => w.CompanyId == duplicateLeadModel.CompanyId).FirstOrDefault()?.CompanyName;

                    duplicateLeadContactModel.CompanyName = company ?? "";

                    return new CheckDuplicateCustomerAllTypeResult()
                    {
                        IsDuplicateLead = isDuplicateLead,
                        IsDuplicateCustomer = isDuplicateCustomer,
                        DuplicateLeadModel = duplicateLeadModel,
                        DuplicateLeadContactModel = duplicateLeadContactModel,
                        StatusCode = System.Net.HttpStatusCode.OK,
                        MessageCode = "OK",
                    };
                }
                #endregion

                return new CheckDuplicateCustomerAllTypeResult()
                {
                    IsDuplicateLead = isDuplicateLead,
                    IsDuplicateCustomer = isDuplicateCustomer,
                    DuplicateLeadModel = duplicateLeadModel,
                    DuplicateLeadContactModel = duplicateLeadContactModel,
                    DuplicateCustomerContactModel = duplicateCustomerContactModel,
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = "OK",

                };
            }
            catch (Exception e)
            {
                return new CheckDuplicateCustomerAllTypeResult()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message,
                };
            }
        }

        public UpdateCustomerByIdResult UpdateCustomerById(UpdateCustomerByIdParameter parameter)
        {
            var customer = new Customer();

            try
            {
                //Kiểm tra trùng số điện thoại với khách hàng doanh nghiệp
                customer = context.Customer.FirstOrDefault(x => x.CustomerId == parameter.CustomerModel.CustomerId);
                var customerContact =
                    context.Contact.FirstOrDefault(x => x.ObjectId == customer.CustomerId && x.ObjectType == "CUS");
                customer.CustomerCode = parameter.CustomerModel.CustomerCode;
                customer.CustomerName = parameter.CustomerModel.CustomerName;
                customer.UpdatedById = parameter.UserId;
                customer.UpdatedDate = DateTime.Now;
                customer.KhachDuAn = parameter.CustomerModel.KhachDuAn ?? false;
                customer.SubjectsApplication = parameter.CustomerModel.SubjectsApplication;
                customer.CustomerType = parameter.CustomerModel.CustomerType;
                customer.CustomerGroupId = parameter.CustomerModel.CustomerGroupId;
                customer.StaffChargeIds = parameter.CustomerModel.StaffChargeIds;

                customerContact.FirstName = parameter.ContactModel.FirstName;
                customerContact.LastName = parameter.ContactModel.LastName;

                customerContact.UpdatedById = parameter.UserId;
                customerContact.UpdatedDate = DateTime.Now;

                if (customer.CustomerType == 2 )
                {
                    customerContact.Gender = parameter.ContactModel.Gender;
                    customerContact.DateOfBirth = parameter.ContactModel.DateOfBirth;
                    customerContact.Phone = parameter.ContactModel.Phone;
                    customerContact.Email = parameter.ContactModel.Email;   
                    customerContact.ProvinceId = parameter.ContactModel.ProvinceId; 
                    customerContact.Address = parameter.ContactModel.Address; 
                }
          
                context.Customer.Update(customer);
                context.Contact.Update(customerContact);
                context.SaveChanges();
            }
            catch (Exception e)
            {
                return new UpdateCustomerByIdResult()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }

            #region Log

            LogHelper.AuditTrace(context, "Update", "CUSTOMER", customer.CustomerId, parameter.UserId);

            #endregion

            #region Gửi mail thông báo

            //NotificationHelper.AccessNotification(context, TypeModel.CustomerDetail, "UPD", new Customer(),
            //    customer, true);

            #endregion

            return new UpdateCustomerByIdResult()
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                MessageCode = "Success",
            };
        }

        public GetCustomerImportDetailResult GetCustomerImportDetail(GetCustomerImportDetailParameter parameter)
        {
            try
            {
                //response variable
                var listCustomerCompanyCode = new List<string>();
                var listCustomerGroup = new List<CategoryEntityModel>();
                var listEmail = new List<string>();
                var listPhone = new List<string>();
                //nhóm khách hàng
                var CUSTOME_GROUP_CODE = "NHA";
                var customerGroupCodeId = context.CategoryType.Where(w => w.CategoryTypeCode == CUSTOME_GROUP_CODE).FirstOrDefault().CategoryTypeId;
                var customerGroupListEntity = context.Category.Where(w => w.CategoryTypeId == customerGroupCodeId).ToList();
                var listCustomerGroupResult = customerGroupListEntity.Where(w => w.CategoryCode != "POR").ToList();//loại khách hàng portal
                listCustomerGroupResult.ForEach(item =>
                {
                    listCustomerGroup.Add(new CategoryEntityModel(item));
                });
                //get phone and email Lead
                var KHDCode = "KHD"; //Mã Category lead đã ký hợp đồng
                var NDOCode = "NDO"; //Mã Category lead ngừng theo dõi
                var leadKHDId = context.Category.Where(w => w.CategoryCode == KHDCode).FirstOrDefault().CategoryId; //Id Lead đã ký hợp đồng
                var leadNDOId = context.Category.Where(w => w.CategoryCode == NDOCode).FirstOrDefault().CategoryId;//Id Lead ngừng theo dõi
                var listLeadEntity = context.Lead.Where(w => w.StatusId != leadKHDId && w.StatusId != leadNDOId && w.Active == true).ToList();
                var listContact = context.Contact.ToList();
                var listLeadId = new List<Guid>();
                listLeadEntity.ForEach(lead =>
                {
                    listLeadId.Add(lead.LeadId);
                });
                var listLeadContactEntity = listContact.Where(w => listLeadId.Contains(w.ObjectId) && w.Active == true).Select(w => new { Phone = w.Phone, Email = w.Email }).ToList();
                listLeadContactEntity.ForEach(leadContact =>
                {
                    if (!listEmail.Contains(leadContact.Email))
                    {
                        listEmail.Add(leadContact.Email);
                    }
                    if (!listPhone.Contains(leadContact.Phone))
                    {
                        listPhone.Add(leadContact.Phone);
                    }
                });
                //get phone and email Customer
                var listCustomerEntity = context.Customer.Where(w => w.Active == true).Select(w => new { w.CustomerId, w.CustomerCode }).ToList();
                var listCustomerId = new List<Guid>();
                listCustomerEntity.ForEach(customer =>
                {
                    //add customer code to response
                    if (customer.CustomerCode?.Trim() != "")
                    {
                        listCustomerCompanyCode.Add(customer.CustomerCode);
                    }
                    //get list customer Id
                    listCustomerId.Add(customer.CustomerId);
                });
                var listCustomerContact = listContact.Where(w => listCustomerId.Contains(w.ObjectId))
                    .Select(w => new { w.Email, w.WorkEmail, w.OtherEmail, w.Phone, w.WorkPhone, w.OtherPhone })
                    .ToList();
                listCustomerContact.ForEach(customerContact =>
                {
                    if (!listEmail.Contains(customerContact.Email) && !string.IsNullOrWhiteSpace(customerContact.Email))
                    {
                        listEmail.Add(customerContact.Email);
                    }
                    if (!listEmail.Contains(customerContact.WorkEmail) && !string.IsNullOrWhiteSpace(customerContact.WorkEmail))
                    {
                        listEmail.Add(customerContact.WorkEmail);
                    }
                    if (!listEmail.Contains(customerContact.OtherEmail) && !string.IsNullOrWhiteSpace(customerContact.OtherEmail))
                    {
                        listEmail.Add(customerContact.OtherEmail);
                    }

                    if (!listPhone.Contains(customerContact.Phone) && !string.IsNullOrWhiteSpace(customerContact.Phone))
                    {
                        listPhone.Add(customerContact.Phone);
                    }
                    if (!listPhone.Contains(customerContact.WorkPhone) && !string.IsNullOrWhiteSpace(customerContact.WorkPhone))
                    {
                        listPhone.Add(customerContact.WorkPhone);
                    }
                    if (!listPhone.Contains(customerContact.OtherPhone) && !string.IsNullOrWhiteSpace(customerContact.OtherPhone))
                    {
                        listPhone.Add(customerContact.OtherPhone);
                    }
                });

                return new GetCustomerImportDetailResult()
                {
                    ListCustomerCompanyCode = listCustomerCompanyCode,
                    ListCustomerGroup = listCustomerGroup,
                    ListEmail = listEmail,
                    ListPhone = listPhone,
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = "Success"
                };
            }
            catch (Exception e)
            {
                return new GetCustomerImportDetailResult()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public ImportListCustomerResult ImportListCustomer(ImportListCustomerParameter parameter)
        {
            try
            {
                //get status new
                var cusSttId = context.CategoryType.FirstOrDefault(ct => ct.CategoryTypeCode == "THA").CategoryTypeId;
                var newCusId = context.Category.FirstOrDefault(c => c.CategoryTypeId == cusSttId && c.CategoryCode == "HDO").CategoryId; //khách hàng định danh
                var potentialCustomerId = context.Category.FirstOrDefault(c => c.CategoryTypeId == cusSttId && c.CategoryCode == "MOI").CategoryId; //khách hàng tiềm năng

                var listCustomer = new List<DataAccess.Databases.Entities.Customer>();
                var listContact = new List<DataAccess.Databases.Entities.Contact>();

                var currentEmployee = context.User.FirstOrDefault(f => f.UserId == parameter.UserId).EmployeeId;

                // Trạng thái chăm sóc khách hàng tiềm năng
                var typeID = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "TTCS")?.CategoryTypeId;
                var statusCGD = context.Category
                    .FirstOrDefault(x => x.CategoryTypeId == typeID && x.CategoryCode == "CGD")?.CategoryId;

                // Trạng thái phụ của khách hàng tiềm năng
                var categotyTypeID = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "TTPKHTN")?.CategoryTypeId;
                var statusNew = context.Category
                    .FirstOrDefault(x => x.CategoryTypeId == categotyTypeID && x.CategoryCode == "A")?.CategoryId;

                //create list guild
                var newListGuid = new List<Guid>();
                for (int i = 0; i < parameter.ListCustomer.Count(); i++)
                {
                    newListGuid.Add(Guid.NewGuid());
                }

                for (int i = 0; i < parameter.ListCustomer.Count(); i++)
                {
                    var newCustomer = new DataAccess.Databases.Entities.Customer();
                    newCustomer.CustomerId = newListGuid[i];

                    #region dungpt comment
                    //khách hàng cá nhân thì generate customer code
                    //cắt họ và tên của khách hàng cá nhân
                    //if (parameter.ListCustomer[i].CustomerType == 1)
                    //{
                    //    var customerCodeValue = parameter.ListCustomer[i].CustomerCode.Trim();
                    //    if (customerCodeValue == "")
                    //    {
                    //        newCustomer.CustomerCode = this.GenerateCustomerCode(0);

                    //    }
                    //    else
                    //    {
                    //        newCustomer.CustomerCode = customerCodeValue;
                    //    }
                    //}
                    //else
                    //{
                    //    newCustomer.CustomerCode = this.GenerateCustomerCode(0);
                    //}
                    #endregion

                    #region add by dungpt
                    newCustomer.CustomerCode = parameter.ListCustomer[i].CustomerCode?.Trim() ?? "";
                    #endregion

                    newCustomer.CustomerName = parameter.ListCustomer[i].CustomerName.Trim();
                    newCustomer.CustomerGroupId = parameter.ListCustomer[i].CustomerGroupId;
                    newCustomer.LeadId = null;
                    newCustomer.StatusId = parameter.IsPotentialCustomer == false ? newCusId : potentialCustomerId;
                    newCustomer.CustomerServiceLevelId = null;
                    newCustomer.PersonInChargeId = currentEmployee;
                    newCustomer.CustomerType = parameter.ListCustomer[i].CustomerType;
                    newCustomer.CreatedById = parameter.UserId;
                    newCustomer.Active = true;
                    newCustomer.CreatedDate = DateTime.Now;
                    newCustomer.CareStateId = statusCGD != null ? statusCGD : Guid.Empty;
                    newCustomer.StatusSuportId = statusNew != null ? statusNew : Guid.Empty;

                    listCustomer.Add(newCustomer);
                }

                //thông tin liên hệ khách hàng
                for (int i = 0; i < parameter.ListContact.Count(); i++)
                {
                    var newContact = new DataAccess.Databases.Entities.Contact();
                    newContact.ContactId = Guid.NewGuid();
                    newContact.ObjectId = newListGuid[i];
                    newContact.ObjectType = "CUS";
                    newContact.FirstName = parameter.ListContact[i].FirstName?.Trim();
                    newContact.LastName = parameter.ListContact[i].LastName?.Trim();
                    newContact.Gender = parameter.ListContact[i].Gender;
                    newContact.DateOfBirth = null;
                    newContact.Phone = parameter.ListContact[i].Phone?.Trim();
                    newContact.Email = parameter.ListContact[i].Email?.Trim();
                    newContact.OptionPosition = parameter.ListContact[i].OptionPosition?.Trim();
                    newContact.Address = parameter.ListContact[i].Address?.Trim();
                    newContact.TaxCode = parameter.ListContact[i].TaxCode?.Trim();
                    newContact.Note = parameter.ListContact[i].Note?.Trim();

                    newContact.Active = true;
                    newContact.CreatedById = parameter.UserId;
                    newContact.CreatedDate = DateTime.Now;

                    listContact.Add(newContact);
                }

                //danh sách người liên hệ
                for (int i = 0; i < parameter.ListContactAdditional.Count(); i++)
                {
                    if (!string.IsNullOrWhiteSpace(parameter.ListContactAdditional[i].FirstName?.Trim()))
                    {
                        var newContact = new DataAccess.Databases.Entities.Contact();
                        newContact.ContactId = Guid.NewGuid();
                        newContact.ObjectId = newListGuid[i];
                        newContact.ObjectType = "CUS_CON";
                        newContact.FirstName = parameter.ListContactAdditional[i].FirstName?.Trim();
                        newContact.LastName = parameter.ListContactAdditional[i].LastName?.Trim();
                        newContact.Gender = parameter.ListContactAdditional[i].Gender;
                        newContact.DateOfBirth = null;
                        newContact.Phone = parameter.ListContactAdditional[i].Phone?.Trim();
                        newContact.Email = parameter.ListContactAdditional[i].Email?.Trim();
                        newContact.Active = true;
                        newContact.CreatedById = parameter.UserId;
                        newContact.CreatedDate = DateTime.Now;

                        listContact.Add(newContact);
                    }
                }

                context.Customer.AddRange(listCustomer);
                context.Contact.AddRange(listContact);
                context.SaveChanges();

                return new ImportListCustomerResult()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = "Success"
                };
            }
            catch (Exception e)
            {
                return new ImportListCustomerResult()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public DeleteListCustomerAdditionalResult DeleteListCustomerAdditional(DeleteListCustomerAdditionalParameter parameter)
        {
            try
            {
                var listCustomerAdditionalInformation = new List<CustomerAdditionalInformationEntityModel>();

                if (parameter.ListCusAddInfId != null && parameter.ListCusAddInfId.Count > 0)
                {
                    var list = context.CustomerAdditionalInformation.Where(x =>
                        parameter.ListCusAddInfId.Contains(x.CustomerAdditionalInformationId)).ToList();
                    context.CustomerAdditionalInformation.RemoveRange(list);
                    context.SaveChanges();
                }

                var listCustomerAdditionalInformationResult = context.CustomerAdditionalInformation
                    .Where(x => x.CustomerId == parameter.CustomerId).OrderByDescending(z => z.CreatedDate).ToList();
                listCustomerAdditionalInformationResult.ForEach(item =>
                {
                    listCustomerAdditionalInformation.Add(new CustomerAdditionalInformationEntityModel(item));
                });

                return new DeleteListCustomerAdditionalResult()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = "Success",
                    ListCustomerAdditionalInformation = listCustomerAdditionalInformation
                };
            }
            catch (Exception e)
            {
                return new DeleteListCustomerAdditionalResult()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetHistoryCustomerCareResult GetHistoryCustomerCare(GetHistoryCustomerCareParameter parameter)
        {
            try
            {
                var listEmployeePosition = context.Position.ToList();
                var listCategoryType = context.CategoryType.ToList();
                var listCategory = context.Category.ToList();
                var listEmployee = context.Employee.ToList();

                //Hình thức
                var customerCareCategoryTypeId =
                    listCategoryType.FirstOrDefault(x => x.CategoryTypeCode == "HCS").CategoryTypeId;
                var listCustomerCareCategory =
                    listCategory.Where(x => x.CategoryTypeId == customerCareCategoryTypeId).ToList();
                var listTypeOfCustomerCare1 = listCustomerCareCategory
                    .Where(x => x.CategoryCode == "Gift" || x.CategoryCode == "CallPhone").Select(y => y.CategoryId)
                    .ToList();
                var listTypeOfCustomerCare2 = listCustomerCareCategory
                    .Where(x => x.CategoryCode == "Email" || x.CategoryCode == "SMS").Select(y => y.CategoryId)
                    .ToList();

                //Trạng thái
                var statusOfCustomerCareCategoryId =
                    listCategoryType.FirstOrDefault(x => x.CategoryTypeCode == "TCS").CategoryTypeId;
                var statusActiveOfCustomerCare = listCategory.FirstOrDefault(x =>
                    x.CategoryTypeId == statusOfCustomerCareCategoryId && x.CategoryCode == "Active").CategoryId;

                var listCustomerCareInfor = new List<CustomerCareInforModel>();
                var listAllCustomerCareForWeek = new List<CustomerCareForWeekModel>();

                /*
                 * Lọc ra các chương trình CSKH thõa mãn các điều kiện:
                 * - Hình thức là Tặng quà và Gọi điện
                 * - Có ngày kích hoạt trong tháng, năm được chọn
                 */
                var listCustomerCare1 = context.CustomerCare.Where(x =>
                    listTypeOfCustomerCare1.Contains(x.CustomerCareContactType.Value) &&
                    x.ActiveDate.Value.Month == parameter.Month && x.ActiveDate.Value.Year == parameter.Year).ToList();

                //Lọc ra các chương trình CSKH mà có Khách hàng hiện tại tham gia
                var listCustomerCareId1 = listCustomerCare1.Select(x => x.CustomerCareId).ToList();

                if (listCustomerCareId1.Count > 0)
                {
                    var listCustomerCareIdForCustomer = context.CustomerCareCustomer
                        .Where(x => listCustomerCareId1.Contains(x.CustomerCareId.Value) &&
                                    x.CustomerId == parameter.CustomerId).Select(x => x.CustomerCareId).ToList();

                    if (listCustomerCareIdForCustomer.Count > 0)
                    {
                        listCustomerCare1 = listCustomerCare1
                            .Where(x => listCustomerCareIdForCustomer.Contains(x.CustomerCareId)).ToList();
                    }
                    else
                    {
                        listCustomerCare1 = new List<CustomerCare>();
                    }
                }

                /*
                 * Lấy list CustomerCareId trong bảng Queue thõa màn các điều kiện sau:
                 * - IsSend = true (Đã gửi)
                 * - Là gửi Email hoặc SMS
                 * - Có ngày gửi (SenDate) trong tháng, năm được chọn
                 */
                var listQueueCustomerCare2 = context.Queue.Where(x =>
                    x.IsSend == true &&
                    (x.Method == "Email" || x.Method == "SMS") && x.SenDate.Value.Month == parameter.Month &&
                    x.SenDate.Value.Year == parameter.Year && x.CustomerId == parameter.CustomerId).ToList();

                var listQueueCustomerCare2Id = listQueueCustomerCare2.Select(y => y.CustomerCareId).Distinct().ToList();

                var listCustomerCare2 = new List<CustomerCare>();
                if (listQueueCustomerCare2Id.Count > 0)
                {
                    listCustomerCare2 = context.CustomerCare
                        .Where(x => listQueueCustomerCare2Id.Contains(x.CustomerCareId)).ToList();
                }

                //merge 2 list CustomerCare
                listCustomerCare1.AddRange(listCustomerCare2);

                var listEmployeeId = new List<Guid>();
                if (listCustomerCare1.Count > 0)
                {
                    var listCustomerCareId = listCustomerCare1.Select(y => y.CustomerCareId).ToList();
                    var listAllFeedBack = context.CustomerCareFeedBack
                        .Where(x => listCustomerCareId.Contains(x.CustomerCareId.Value) && x.CustomerId == parameter.CustomerId).ToList();
                    listEmployeeId = listCustomerCare1.Select(y => y.EmployeeCharge.Value).Distinct().ToList();

                    listEmployeeId.ForEach(employeeId =>
                    {
                        var customerCareInfor = new CustomerCareInforModel();
                        var emp = listEmployee.FirstOrDefault(x => x.EmployeeId == employeeId);
                        customerCareInfor.EmployeeCharge = emp.EmployeeId;
                        customerCareInfor.EmployeeName = emp.EmployeeName;
                        customerCareInfor.EmployeePosition = listEmployeePosition
                            .FirstOrDefault(x => x.PositionId == emp.PositionId).PositionName;

                        listCustomerCareInfor.Add(customerCareInfor);
                    });

                    listCustomerCare1.ForEach(item =>
                    {
                        var customerCareForWeek = new CustomerCareForWeekModel();
                        customerCareForWeek.CustomerCareId = item.CustomerCareId;
                        customerCareForWeek.EmployeeCharge = item.EmployeeCharge.Value;
                        customerCareForWeek.Title = listCustomerCareCategory
                            .FirstOrDefault(x => x.CategoryId == item.CustomerCareContactType).CategoryName;

                        /*
                         * Gửi SMS: 1
                         * Gửi email: 2
                         * Tặng quà: 3
                         * Gọi điện: 4
                         */
                        var customerCareCategoryCode = listCustomerCareCategory
                            .FirstOrDefault(x => x.CategoryId == item.CustomerCareContactType).CategoryCode;

                        switch (customerCareCategoryCode)
                        {
                            case "SMS":
                                customerCareForWeek.Type = 1;
                                customerCareForWeek.SubTitle = "Xem chi tiết";
                                customerCareForWeek.FeedBackStatus = 0;
                                customerCareForWeek.Background = "#fbe8ba";
                                customerCareForWeek.ActiveDate = listQueueCustomerCare2
                                    .FirstOrDefault(x => x.CustomerCareId == item.CustomerCareId).SenDate.Value;
                                break;
                            case "Email":
                                customerCareForWeek.Type = 2;
                                customerCareForWeek.SubTitle = "Xem chi tiết";
                                customerCareForWeek.FeedBackStatus = 0;
                                customerCareForWeek.Background = "#e5cbf2";
                                customerCareForWeek.ActiveDate = listQueueCustomerCare2
                                    .FirstOrDefault(x => x.CustomerCareId == item.CustomerCareId).SenDate.Value;
                                customerCareForWeek.Title = context.Queue.FirstOrDefault(x => x.CustomerId == parameter.CustomerId && x.CustomerCareId == item.CustomerCareId)?.Title;
                                if (customerCareForWeek.Title.Contains("-"))
                                {
                                    customerCareForWeek.Title = customerCareForWeek.Title.Substring(0,
                                        customerCareForWeek.Title.LastIndexOf("-") - 1);
                                }
                                break;
                            case "Gift":
                                customerCareForWeek.Type = 3;
                                var checkFeedBackGift = listAllFeedBack.FirstOrDefault(x => x.CustomerCareId == item.CustomerCareId);
                                customerCareForWeek.SubTitle = checkFeedBackGift != null
                                    ? (checkFeedBackGift.FeedBackCode != null ? "Đã phản hồi" : "Chưa phản hồi")
                                    : "Chưa phản hồi";
                                customerCareForWeek.FeedBackStatus = checkFeedBackGift != null ? 1 : 2;
                                customerCareForWeek.Background = "#cfdefa";
                                customerCareForWeek.ActiveDate = item.ActiveDate.Value;
                                break;
                            case "CallPhone":
                                customerCareForWeek.Type = 4;
                                var checkFeedBackCallPhone = listAllFeedBack.FirstOrDefault(x => x.CustomerCareId == item.CustomerCareId);
                                customerCareForWeek.SubTitle = checkFeedBackCallPhone != null
                                    ? (checkFeedBackCallPhone.FeedBackCode != null ? "Đã phản hồi" : "Chưa phản hồi")
                                    : "Chưa phản hồi";
                                customerCareForWeek.FeedBackStatus = checkFeedBackCallPhone != null ? 1 : 2;
                                customerCareForWeek.Background = "#f4d4e4";
                                customerCareForWeek.ActiveDate = item.ActiveDate.Value;
                                break;
                        }

                        listAllCustomerCareForWeek.Add(customerCareForWeek);
                    });

                    #region Nhóm theo nhân viên CSKH và theo tuần

                    var current_month = parameter.Month;
                    var current_year = parameter.Year;
                    //Ngày đầu tiên của tháng
                    var first_date_month = new DateTime(current_year, current_month, 1, 0, 0, 0, 0);
                    //Ngày cuối cùng của tháng
                    var last_date_month = first_date_month.AddMonths(1).AddDays(-1);
                    last_date_month = last_date_month.AddHours(23).AddMinutes(59).AddSeconds(59).AddMilliseconds(999);

                    //Ngày bắt đầu tuần 1 và Ngày cuối cùng tuần 1
                    var startDateWeek1 = first_date_month;
                    var endDateWeek1 = LastDateOfWeek(startDateWeek1);

                    //Ngày bắt đầu tuần 2 và Ngày cuối cùng tuần 2
                    var startDateWeek2 = FirstDateOfWeek(endDateWeek1);
                    var endDateWeek2 = LastDateOfWeek(startDateWeek2);

                    //Ngày bắt đầu tuần 3 và Ngày cuối cùng tuần 3
                    var startDateWeek3 = FirstDateOfWeek(endDateWeek2);
                    var endDateWeek3 = LastDateOfWeek(startDateWeek3);

                    //Ngày bắt đầu tuần 4 và Ngày cuối cùng tuần 4
                    var startDateWeek4 = FirstDateOfWeek(endDateWeek3);
                    var endDateWeek4 = LastDateOfWeek(startDateWeek4);

                    //Ngày bắt đầu tuần 5 và Ngày cuối cùng tuần 5
                    DateTime? startDateWeek5 = FirstDateOfWeek(endDateWeek4);
                    DateTime? endDateWeek5 = last_date_month;

                    int check = checkLastDateOfMonth(endDateWeek4, last_date_month);

                    if (check == 1)
                    {
                        //Nếu là ngày cuối cùng của tháng
                        endDateWeek4 = last_date_month;
                        startDateWeek5 = null;
                        endDateWeek5 = null;
                    }

                    var listWeek1 = new List<CustomerCareForWeekModel>();
                    var listWeek2 = new List<CustomerCareForWeekModel>();
                    var listWeek3 = new List<CustomerCareForWeekModel>();
                    var listWeek4 = new List<CustomerCareForWeekModel>();
                    var listWeek5 = new List<CustomerCareForWeekModel>();
                    listCustomerCareInfor.ForEach(item =>
                    {
                        listWeek1 = listAllCustomerCareForWeek.Where(x =>
                            x.EmployeeCharge == item.EmployeeCharge && x.ActiveDate >= startDateWeek1 &&
                            x.ActiveDate < endDateWeek1).OrderBy(z => z.ActiveDate).ToList();
                        item.Week1 = listWeek1;

                        listWeek2 = listAllCustomerCareForWeek.Where(x =>
                            x.EmployeeCharge == item.EmployeeCharge && x.ActiveDate >= startDateWeek2 &&
                            x.ActiveDate < endDateWeek2).OrderBy(z => z.ActiveDate).ToList();
                        item.Week2 = listWeek2;

                        listWeek3 = listAllCustomerCareForWeek.Where(x =>
                            x.EmployeeCharge == item.EmployeeCharge && x.ActiveDate >= startDateWeek3 &&
                            x.ActiveDate < endDateWeek3).OrderBy(z => z.ActiveDate).ToList();
                        item.Week3 = listWeek3;

                        listWeek4 = listAllCustomerCareForWeek.Where(x =>
                            x.EmployeeCharge == item.EmployeeCharge && x.ActiveDate >= startDateWeek4 &&
                            x.ActiveDate < endDateWeek4).OrderBy(z => z.ActiveDate).ToList();
                        item.Week4 = listWeek4;

                        if (check != 1)
                        {
                            listWeek5 = listAllCustomerCareForWeek.Where(x =>
                                x.EmployeeCharge == item.EmployeeCharge && x.ActiveDate >= startDateWeek5 &&
                                x.ActiveDate < endDateWeek5).OrderBy(z => z.ActiveDate).ToList();
                            item.Week5 = listWeek5;
                        }
                    });

                    #endregion
                }

                return new GetHistoryCustomerCareResult()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = "Success",
                    ListCustomerCareInfor = listCustomerCareInfor
                };
            }
            catch (Exception e)
            {
                return new GetHistoryCustomerCareResult()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetDataPreviewCustomerCareResult GetDataPreviewCustomerCare(GetDataPreviewCustomerCareParameter parameter)
        {
            try
            {
                var effecttiveFromDate = DateTime.Now;
                var effecttiveToDate = DateTime.Now;
                var sendDate = DateTime.Now;
                var statusName = "";
                var previewEmailContent = "";
                var previewEmailTo = "";
                var previewEmailCC = "";
                var previewEmailBcc = "";
                var previewEmailName = "";
                var previewEmailTitle = "";
                var previewSmsPhone = "";
                var previewSmsContent = "";

                var listPreviewFile = new List<FileInFolderEntityModel>();

                var customerCare =
                    context.CustomerCare.FirstOrDefault(x => x.CustomerCareId == parameter.CustomerCareId);
                effecttiveFromDate = customerCare.EffecttiveFromDate.Value;
                effecttiveToDate = customerCare.EffecttiveToDate.Value;

                var queue = context.Queue.FirstOrDefault(x =>
                    x.CustomerId == parameter.CustomerId && x.CustomerCareId == parameter.CustomerCareId);

                var categoryTypeId = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "TCS").CategoryTypeId;
                statusName = context.Category
                    .FirstOrDefault(x => x.CategoryTypeId == categoryTypeId && x.CategoryId == customerCare.StatusId)
                    .CategoryName;

                switch (parameter.Mode)
                {
                    case "Email":
                        previewEmailTitle = queue.Title;
                        previewEmailContent = queue.SendContent;
                        previewEmailTo = queue?.SendTo;
                        previewEmailCC = queue?.Cc;
                        previewEmailBcc = queue?.Bcc;
                        sendDate = queue.SenDate.Value;
                        break;

                    case "SMS":
                        previewSmsContent = queue.SendContent;
                        sendDate = queue.SenDate.Value;
                        break;
                }

                if (parameter.Mode.Equals("Email"))
                {
                    listPreviewFile = context.FileInFolder.Where(x =>
                            x.ObjectId == parameter.CustomerCareId && x.ObjectType == "QLKHCRM")
                        .Select(y => new FileInFolderEntityModel
                        {
                            FileName = y.FileName,
                            FileExtension = y.FileExtension,
                            CreatedDate = y.CreatedDate,
                            CreatedById = y.CreatedById,
                            Size = y.Size,
                            FileInFolderId = y.FileInFolderId,
                            FolderId = y.FolderId,
                        }).ToList();

                    var webRootPath = hostingEnvironment.WebRootPath + "\\";

                    listPreviewFile.ForEach(item =>
                    {
                        var empId = context.User.FirstOrDefault(x => x.UserId == item.CreatedById)?.EmployeeId;
                        item.CreatedByName = context.Employee.FirstOrDefault(x => x.EmployeeId == empId)?.EmployeeName;

                        item.FileFullName = $"{item.FileName}.{item.FileExtension}";
                        var folderUrl = context.Folder.FirstOrDefault(x => x.FolderId == item.FolderId)?.Url;
                        item.FileUrl = Path.Combine(webRootPath, folderUrl, item.FileFullName);
                    });
                }

                return new GetDataPreviewCustomerCareResult()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = "Success",
                    EffecttiveFromDate = effecttiveFromDate,
                    EffecttiveToDate = effecttiveToDate,
                    SendDate = sendDate,
                    StatusName = statusName,
                    PreviewEmailContent = previewEmailContent,
                    PreviewEmailTo = previewEmailTo,
                    PreviewEmailCC = previewEmailCC,
                    PreviewEmailBcc = previewEmailBcc,
                    PreviewEmailName = previewEmailName,
                    PreviewEmailTitle = previewEmailTitle,
                    PreviewSmsPhone = previewSmsPhone,
                    PreviewSmsContent = previewSmsContent,
                    ListPreviewFile = listPreviewFile,
                };
            }
            catch (Exception e)
            {
                return new GetDataPreviewCustomerCareResult()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetDataCustomerCareFeedBackResult GetDataCustomerCareFeedBack(GetDataCustomerCareFeedBackParameter parameter)
        {
            try
            {
                var name = "";
                var typeName = "";
                Guid? feedBackCode = null;
                var feedBackContent = "";
                var listFeedBackCode = new List<CategoryEntityModel>();

                var feedBack = context.CustomerCareFeedBack.FirstOrDefault(x =>
                    x.CustomerId == parameter.CustomerId && x.CustomerCareId == parameter.CustomerCareId);

                var customerCare =
                    context.CustomerCare.FirstOrDefault(x => x.CustomerCareId == parameter.CustomerCareId);
                name = customerCare.CustomerCareTitle;

                var categoryTypeId1 = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "HCS")
                    .CategoryTypeId;
                var fromDate = customerCare.EffecttiveFromDate;
                var toDate = customerCare.EffecttiveToDate;
                typeName = context.Category.FirstOrDefault(x =>
                        x.CategoryId == customerCare.CustomerCareContactType.Value &&
                        x.CategoryTypeId == categoryTypeId1)
                    .CategoryName;

                var categoryTypeId = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "MHO").CategoryTypeId;
                listFeedBackCode = context.Category.Where(x => x.CategoryTypeId == categoryTypeId).Select(y =>
                    new CategoryEntityModel
                    {
                        CategoryId = y.CategoryId,
                        CategoryCode = y.CategoryCode,
                        CategoryName = y.CategoryName,
                        IsDefault = y.IsDefauld
                    }).ToList();

                if (feedBack != null)
                {
                    feedBackCode = feedBack.FeedBackCode;
                    feedBackContent = feedBack.FeedBackContent;
                }

                return new GetDataCustomerCareFeedBackResult()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = "Success",
                    Name = name,
                    FromDate = fromDate.Value,
                    ToDate = toDate.Value,
                    TypeName = typeName,
                    FeedBackCode = feedBackCode,
                    FeedBackContent = feedBackContent,
                    ListFeedBackCode = listFeedBackCode
                };
            }
            catch (Exception e)
            {
                return new GetDataCustomerCareFeedBackResult()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public SaveCustomerCareFeedBackResult SaveCustomerCareFeedBack(SaveCustomerCareFeedBackParameter parameter)
        {
            try
            {
                var feedBack = context.CustomerCareFeedBack.FirstOrDefault(x =>
                    x.CustomerId == parameter.CustomerCareFeedBack.CustomerId &&
                    x.CustomerCareId == parameter.CustomerCareFeedBack.CustomerCareId);
                var customerCare = context.CustomerCare.FirstOrDefault(x =>
                    x.CustomerCareId == parameter.CustomerCareFeedBack.CustomerCareId);
                var feedBackFromDate = customerCare.EffecttiveFromDate;
                var feedBackToDate = customerCare.EffecttiveToDate;
                var feedBackType = customerCare.CustomerCareContactType;
                var feedBackCode = parameter.CustomerCareFeedBack.FeedBackCode;
                var feedBackContent = parameter.CustomerCareFeedBack.FeedBackContent;

                if (feedBack == null)
                {
                    var newFeedBack = new CustomerCareFeedBack();
                    newFeedBack.CustomerCareFeedBackId = Guid.NewGuid();
                    newFeedBack.FeedBackFromDate = feedBackFromDate;
                    newFeedBack.FeedBackToDate = feedBackToDate;
                    newFeedBack.FeedbackType = feedBackType;
                    newFeedBack.FeedBackCode = feedBackCode;
                    newFeedBack.FeedBackContent = feedBackContent;
                    newFeedBack.CustomerId = parameter.CustomerCareFeedBack.CustomerId;
                    newFeedBack.CustomerCareId = parameter.CustomerCareFeedBack.CustomerCareId;
                    newFeedBack.CreateDate = DateTime.Now;
                    newFeedBack.CreateById = parameter.UserId;

                    context.CustomerCareFeedBack.Add(newFeedBack);
                    context.SaveChanges();
                }
                else
                {
                    feedBack.FeedBackCode = feedBackCode;
                    feedBack.FeedBackContent = feedBackContent;
                    feedBack.UpdateDate = DateTime.Now;
                    feedBack.UpdateById = parameter.UserId;

                    context.CustomerCareFeedBack.Update(feedBack);
                    context.SaveChanges();
                }

                return new SaveCustomerCareFeedBackResult()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = "Success"
                };
            }
            catch (Exception e)
            {
                return new SaveCustomerCareFeedBackResult()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetDataCustomerMeetingByIdResult GetDataCustomerMeetingById(GetDataCustomerMeetingByIdParameter parameter)
        {
            try
            {
                var customerMeeting = new CustomerMeetingEntityModel();

                var customerContact = new List<ContactEntityModel>();


                if (parameter.CustomerMeetingId != null)
                {
                    customerMeeting =
                        context.CustomerMeeting.Where(x => x.CustomerMeetingId == parameter.CustomerMeetingId).Select(
                            y => new CustomerMeetingEntityModel
                            {
                                CustomerMeetingId = y.CustomerMeetingId,
                                CustomerId = y.CustomerId,
                                EmployeeId = y.EmployeeId,
                                Title = y.Title,
                                LocationMeeting = y.LocationMeeting,
                                StartDate = y.StartDate,
                                //StartHours = y.StartHours,
                                EndDate = y.EndDate,
                                Content = y.Content,
                                Participants = y.Participants,
                                IsCreateByUser = y.CreatedById == parameter.UserId,
                                CustomerParticipants = y.CustomerEmail,
                            }).FirstOrDefault();

                    if (parameter.CustomerId != null)
                    {
                        customerContact = context.Contact.Where(x =>
                                x.Active == true && x.ObjectType == "CUS_CON" && x.ObjectId == parameter.CustomerId)
                            .Select(y => new ContactEntityModel
                            {
                                ContactId = y.ContactId,
                                ObjectId = y.ObjectId,
                                ObjectType = y.ObjectType,
                                FirstName = y.FirstName,
                                LastName = y.LastName,
                                Gender = y.Gender,
                                Phone = y.Phone,
                                Email = y.Email,
                                Role = y.Role,
                            }).ToList();
                    }
                }
                else
                {
                    if (parameter.CustomerId != null)
                    {
                        customerContact = context.Contact.Where(x =>
                                x.Active == true && x.ObjectType == "CUS_CON" && x.ObjectId == parameter.CustomerId)
                            .Select(y => new ContactEntityModel
                            {
                                ContactId = y.ContactId,
                                ObjectId = y.ObjectId,
                                ObjectType = y.ObjectType,
                                FirstName = y.FirstName,
                                LastName = y.LastName,
                                Gender = y.Gender,
                                Phone = y.Phone,
                                Email = y.Email,
                                Role = y.Role,
                            }).ToList();
                    }
                }

                return new GetDataCustomerMeetingByIdResult()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = "Success",
                    CustomerMeeting = customerMeeting,
                    CustomerContact = customerContact,
                };
            }
            catch (Exception e)
            {
                return new GetDataCustomerMeetingByIdResult()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public CreateCustomerMeetingResult CreateCustomerMeeting(CreateCustomerMeetingParameter parameter)
        {
            try
            {
                var today = DateTime.Today;
                if (parameter.CustomerMeeting.StartDate < today)
                {
                    return new CreateCustomerMeetingResult()
                    {
                        StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                        MessageCode = CommonMessage.Customer.TIME_ERROR
                    };
                }

                GetConfiguration();
                var user = context.User.FirstOrDefault(x => x.UserId == parameter.UserId);
                var employeeContact =
                    context.Contact.FirstOrDefault(x => x.ObjectId == user.EmployeeId && x.ObjectType == "EMP");
                var listEmployeeId = new List<string>();
                var listEmployeeContact = new List<Contact>();

                if (parameter.CustomerMeeting.Participants != null && parameter.CustomerMeeting.Participants != $"")
                {
                    listEmployeeId = parameter.CustomerMeeting.Participants.Split(';').ToList();
                    listEmployeeContact = context.Contact.Where(c => listEmployeeId.Contains(c.ObjectId.ToString()) && c.ObjectType == "EMP").ToList();
                }

                if (parameter.CustomerMeeting.CustomerMeetingId != null)
                {
                    var customerMeeting = context.CustomerMeeting.FirstOrDefault(x =>
                        x.CustomerMeetingId == parameter.CustomerMeeting.CustomerMeetingId);

                    customerMeeting.Title = parameter.CustomerMeeting.Title;
                    customerMeeting.LocationMeeting = parameter.CustomerMeeting.LocationMeeting;
                    customerMeeting.StartDate = SetDate(parameter.CustomerMeeting.StartDate, parameter.CustomerMeeting.StartHours.Value.TimeOfDay);
                    customerMeeting.StartHours = parameter.CustomerMeeting.StartHours.Value.TimeOfDay;
                    customerMeeting.Content = parameter.CustomerMeeting.Content;
                    customerMeeting.UpdatedById = parameter.UserId;
                    customerMeeting.UpdatedDate = DateTime.Now;
                    if (parameter.CustomerMeeting.EndDate != null && parameter.CustomerMeeting.EndHours != null)
                    {
                        customerMeeting.EndDate = SetDate(parameter.CustomerMeeting.EndDate, parameter.CustomerMeeting.EndHours.Value.TimeOfDay);
                        customerMeeting.EndHours = parameter.CustomerMeeting.EndHours.Value.TimeOfDay;
                    }

                    if (parameter.CustomerMeeting.EndHours != null && parameter.CustomerMeeting.EndDate == null)
                    {
                        customerMeeting.EndDate = SetDate(parameter.CustomerMeeting.StartDate, parameter.CustomerMeeting.EndHours.Value.TimeOfDay);
                        customerMeeting.EndHours = parameter.CustomerMeeting.EndHours.Value.TimeOfDay;
                    }
                    customerMeeting.Participants = parameter.CustomerMeeting.Participants;
                    customerMeeting.CustomerEmail = parameter.CustomerMeeting.CustomerParticipants;
                    context.CustomerMeeting.Update(customerMeeting);

                    var listEmail = new List<string>();
                    listEmployeeContact.ForEach(item =>
                    {
                        if (item.Email != "")
                        {
                            listEmail.Add(item.Email.Trim());
                        }
                    });

                    var listCustomerEmail = parameter.CustomerMeeting.CustomerParticipants.Split(';');
                    foreach (string email in listCustomerEmail)
                    {
                        if (email != "")
                        {
                            listEmail.Add(email.Trim());
                        }
                    }

                    // lấy nhân viên tham giá
                    var listEmpId = customerMeeting.Participants.Split(';').ToList();
                    var listAllEmp = context.Employee.ToList();
                    StringBuilder employeeName = new StringBuilder();
                    listEmpId.ForEach(item =>
                    {
                        var name = listAllEmp.FirstOrDefault(x => item != null && x.EmployeeId == Guid.Parse(item))?.EmployeeName;
                        employeeName.Append(name + "; ");
                    });
                    string empName = employeeName.ToString().Substring(0, length: employeeName.ToString().LastIndexOf(";", StringComparison.Ordinal));


                    // lấy khách hàng tham giá
                    string customerName = "";
                    var customerEmails = customerMeeting.CustomerEmail;
                    if (!string.IsNullOrEmpty(customerEmails))
                    {
                        var listCusEmail = customerEmails.Split(";").ToList();
                        var listAllContact = context.Contact.Where(x => x.ObjectType == "CUS_CON").OrderByDescending(y => y.CreatedDate).ToList();

                        listCusEmail.ForEach(item =>
                        {
                            var customerContact = listAllContact.FirstOrDefault(x => item != null && x.Email == item.Trim() && x.ObjectId == parameter.CustomerMeeting.CustomerId);
                            if (customerContact != null && customerContact.FirstName == null && customerContact.LastName != null)
                            {
                                customerName = customerName + customerContact.LastName + "; ";
                            }
                            else if (customerContact != null && customerContact.LastName == null && customerContact.FirstName != null)
                            {
                                customerName = customerName + customerContact.FirstName + "; ";
                            }
                            else if (customerContact != null && customerContact.LastName != null && customerContact.FirstName != null)
                            {
                                customerName = customerName + customerContact.FirstName + " " + customerContact.LastName + "; ";
                            }
                            else if (customerContact == null)
                            {
                                customerName = customerName + item + "; ";
                            }
                        });

                        customerName = customerName.Substring(0, customerName.LastIndexOf(";", StringComparison.Ordinal));

                    }


                    var title =
                        $"{customerMeeting.Title} - lúc {customerMeeting.StartHours.Value:hh\\:mm} ngày {customerMeeting.StartDate.Value:dd/MM/yyyy}";

                    var body =
                        $"<p><strong>Thời gian:</strong> {customerMeeting.StartHours.Value:hh\\:mm} ngày {customerMeeting.StartDate.Value:dd/MM/yyyy}</p>" +
                        $"<p><strong>Địa điểm:</strong> {customerMeeting.LocationMeeting}</p>" +
                        $"<p><strong>Nhân viên tham gia:</strong> {empName}</p>" +
                        $"<p><strong>Khách hàng tham gia:</strong> {customerName}</p>" +
                        $"<p>-----------------------</p>" +
                        $"<p><strong>Nội dung họp:</strong></p>" +
                        $"<p>{customerMeeting.Content}</p>";
                    var sendMail = Emailer.SendMailWithIcsAttachment(context, listEmail, null, title, body, customerMeeting.StartDate ?? DateTime.Now, customerMeeting.EndDate, customerMeeting.CustomerMeetingId, customerMeeting.LocationMeeting, false);
                    if (!sendMail.Status)
                    {
                        return new CreateCustomerMeetingResult()
                        {
                            StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                            MessageCode = sendMail.Message
                        };
                    }
                }
                else
                {
                    var customerMeeting = new CustomerMeeting();

                    customerMeeting.CustomerMeetingId = Guid.NewGuid();
                    customerMeeting.CustomerId = parameter.CustomerMeeting.CustomerId;
                    customerMeeting.EmployeeId = user.EmployeeId.Value;
                    customerMeeting.Title = parameter.CustomerMeeting.Title;
                    customerMeeting.LocationMeeting = parameter.CustomerMeeting.LocationMeeting;
                    customerMeeting.StartDate = SetDate(parameter.CustomerMeeting.StartDate, parameter.CustomerMeeting.StartHours.Value.TimeOfDay);
                    customerMeeting.StartHours = parameter.CustomerMeeting.StartHours.Value.TimeOfDay;
                    if (parameter.CustomerMeeting.EndDate != null && parameter.CustomerMeeting.EndHours != null)
                    {
                        customerMeeting.EndDate = SetDate(parameter.CustomerMeeting.EndDate, parameter.CustomerMeeting.EndHours.Value.TimeOfDay);
                        customerMeeting.EndHours = parameter.CustomerMeeting.EndHours.Value.TimeOfDay;
                    }

                    if (parameter.CustomerMeeting.EndHours != null && parameter.CustomerMeeting.EndDate == null)
                    {
                        customerMeeting.EndDate = SetDate(parameter.CustomerMeeting.StartDate, parameter.CustomerMeeting.EndHours.Value.TimeOfDay);
                        customerMeeting.EndHours = parameter.CustomerMeeting.EndHours.Value.TimeOfDay;
                    }
                    customerMeeting.Content = parameter.CustomerMeeting.Content;
                    customerMeeting.Active = true;
                    customerMeeting.CreatedById = parameter.UserId;
                    customerMeeting.CreatedDate = DateTime.Now;
                    customerMeeting.Participants = parameter.CustomerMeeting.Participants;
                    customerMeeting.CustomerEmail = parameter.CustomerMeeting.CustomerParticipants;
                    context.CustomerMeeting.Add(customerMeeting);

                    #region Đổi trạng thái của khách hàng tiềm năng

                    var customer =
                        context.Customer.FirstOrDefault(x => x.CustomerId == customerMeeting.CustomerId);
                    var categoryTypeId = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "TTCS")?.CategoryTypeId;
                    var listAllCategory = context.Category.ToList();

                    var statusNotCallYet = listAllCategory
                        .FirstOrDefault(x => x.CategoryTypeId == categoryTypeId && x.CategoryCode == "CGD")?.CategoryId;
                    var statusCalled = listAllCategory
                        .FirstOrDefault(x => x.CategoryTypeId == categoryTypeId && x.CategoryCode == "DGD")?.CategoryId;
                    var statusNotMeetYet = listAllCategory
                        .FirstOrDefault(x => x.CategoryTypeId == categoryTypeId && x.CategoryCode == "CG")?.CategoryId;
                    var statusDHL = listAllCategory
                        .FirstOrDefault(x => x.CategoryTypeId == categoryTypeId && x.CategoryCode == "DHL")?.CategoryId;

                    if (customer != null)
                    {
                        if (customer.CareStateId == statusNotMeetYet || customer.CareStateId == statusNotCallYet ||
                            customer.CareStateId == statusCalled)
                        {
                            customer.CareStateId = statusDHL;
                            context.Customer.Update(customer);
                        }
                    }

                    #endregion

                    //var sendDate = DateTime.Now;    

                    //var timeNow = DateTime.Now;
                    //timeNow = timeNow.AddMinutes(60);
                    //var compareTime = DateTime.Compare(timeNow, customerMeeting.StartDate.Value);
                    //if (compareTime <= 0)
                    //{
                    //    sendDate = customerMeeting.StartDate.Value.AddMinutes(-60);
                    //}

                    var listEmail = new List<string>();
                    listEmployeeContact.ForEach(item =>
                    {
                        if (item.Email != "")
                        {
                            listEmail.Add(item.Email.Trim());
                        }
                    });

                    var listCustomerEmail = parameter.CustomerMeeting.CustomerParticipants.Split(';');
                    foreach (string email in listCustomerEmail)
                    {
                        if (email != "")
                        {
                            listEmail.Add(email.Trim());
                        }
                    }

                    // lấy nhân viên tham giá
                    var listEmpId = customerMeeting.Participants.Split(';').ToList();
                    var listAllEmp = context.Employee.ToList();
                    StringBuilder employeeName = new StringBuilder();
                    listEmpId.ForEach(item =>
                    {
                        var name = listAllEmp.FirstOrDefault(x => item != null && x.EmployeeId == Guid.Parse(item))?.EmployeeName;
                        employeeName.Append(name + "; ");
                    });
                    string empName = employeeName.ToString().Substring(0, length: employeeName.ToString().LastIndexOf(";", StringComparison.Ordinal));



                    // lấy khách hàng tham giá
                    string customerName = "";
                    var customerEmails = customerMeeting.CustomerEmail;
                    if (!string.IsNullOrEmpty(customerEmails))
                    {
                        var listCusEmail = customerEmails.Split(";").ToList();
                        var listAllContact = context.Contact.Where(x => x.ObjectType == "CUS_CON").OrderByDescending(y => y.CreatedDate).ToList();

                        listCusEmail.ForEach(item =>
                        {
                            var customerContact = listAllContact.FirstOrDefault(x => item != null && x.Email == item.Trim() && x.ObjectId == parameter.CustomerMeeting.CustomerId);
                            if (customerContact != null && customerContact.FirstName == null && customerContact.LastName != null)
                            {
                                customerName = customerName + customerContact.LastName + "; ";
                            }
                            else if (customerContact != null && customerContact.LastName == null && customerContact.FirstName != null)
                            {
                                customerName = customerName + customerContact.FirstName + "; ";
                            }
                            else if (customerContact != null && customerContact.LastName != null && customerContact.FirstName != null)
                            {
                                customerName = customerName + customerContact.FirstName + " " + customerContact.LastName + "; ";
                            }
                            else if (customerContact == null)
                            {
                                customerName = customerName + item + "; ";
                            }
                        });

                        customerName = customerName.Substring(0, customerName.LastIndexOf(";", StringComparison.Ordinal));

                    }

                    var title =
                        $"{customerMeeting.Title} - lúc {customerMeeting.StartHours.Value:hh\\:mm} ngày {customerMeeting.StartDate.Value:dd/MM/yyyy}";

                    var body =
                        $"<p><strong>Thời gian:</strong> {customerMeeting.StartHours.Value:hh\\:mm} ngày {customerMeeting.StartDate.Value:dd/MM/yyyy}</p>" +
                        $"<p><strong>Địa điểm:</strong> {customerMeeting.LocationMeeting}</p>" +
                        $"<p><strong>Nhân viên tham gia:</strong> {empName}</p>" +
                        $"<p><strong>Khách hàng tham gia:</strong> {customerName}</p>" +
                        $"<p>-----------------------</p>" +
                        $"<p><strong>Nội dung họp:</strong></p>" +
                        $"<p>{customerMeeting.Content}</p>";

                    var sendMail = Emailer.SendMailWithIcsAttachment(context, listEmail, null, title, body, customerMeeting.StartDate ?? DateTime.Now, customerMeeting.EndDate, customerMeeting.CustomerMeetingId, customerMeeting.LocationMeeting, false);
                    if (!sendMail.Status)
                    {
                        return new CreateCustomerMeetingResult()
                        {
                            StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                            MessageCode = sendMail.Message
                        };
                    }
                }

                context.SaveChanges();

                return new CreateCustomerMeetingResult()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = "Success"
                };
            }
            catch (Exception e)
            {
                return new CreateCustomerMeetingResult()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetHistoryCustomerMeetingResult GetHistoryCustomerMeeting(GetHistoryCustomerMeetingParameter parameter)
        {
            try
            {
                var user = context.User.FirstOrDefault(x => x.UserId == parameter.UserId);
                var employee = context.Employee.FirstOrDefault(x => x.EmployeeId == user.EmployeeId);
                var listEmployeePosition = context.Position.ToList();

                var customerMeetingInfor = new CustomerMeetingInforModel();
                customerMeetingInfor.EmployeeId = employee.EmployeeId;
                customerMeetingInfor.EmployeeName = employee.EmployeeName;
                customerMeetingInfor.EmployeePosition = listEmployeePosition
                    .FirstOrDefault(x => x.PositionId == employee.PositionId).PositionName;

                var listAllCustomerMeetingForWeek = new List<CustomerMeetingForWeekModel>();

                var listAllCustomerMeeting = context.CustomerMeeting.Where(x =>
                        x.EmployeeId == employee.EmployeeId && x.CustomerId == parameter.CustomerId &&
                        x.StartDate.Value.Month == parameter.Month &&
                        x.StartDate.Value.Year == parameter.Year)
                    .ToList();

                listAllCustomerMeeting.ForEach(item =>
                {
                    var customerMeetingForWeek = new CustomerMeetingForWeekModel();
                    customerMeetingForWeek.CustomerMeetingId = item.CustomerMeetingId;
                    customerMeetingForWeek.EmployeeId = item.EmployeeId;
                    customerMeetingForWeek.Title = item.Title;
                    customerMeetingForWeek.Subtitle = item.StartDate.Value.ToString("dd/MM/yyyy") + " - " + item.StartDate.Value.ToString("HH:mm");
                    customerMeetingForWeek.Background = "#ffcc00";
                    customerMeetingForWeek.StartDate = item.StartDate;
                    customerMeetingForWeek.StartHours = item.StartHours;
                    listAllCustomerMeetingForWeek.Add(customerMeetingForWeek);
                });

                var current_month_meeting = parameter.Month;
                var current_year_meeting = parameter.Year;
                //Ngày đầu tiên của tháng
                var first_date_month_meeting = new DateTime(current_year_meeting, current_month_meeting, 1, 0, 0, 0, 0);
                //Ngày cuối cùng của tháng
                var last_date_month_meeting = first_date_month_meeting.AddMonths(1).AddDays(-1);
                last_date_month_meeting = last_date_month_meeting.AddHours(23).AddMinutes(59).AddSeconds(59).AddMilliseconds(999);

                //Ngày bắt đầu tuần 1 và Ngày cuối cùng tuần 1
                var startDateMeetingWeek1 = first_date_month_meeting;
                var endDateMeetingWeek1 = LastDateOfWeek(startDateMeetingWeek1);

                //Ngày bắt đầu tuần 2 và Ngày cuối cùng tuần 2
                var startDateMeetingWeek2 = FirstDateOfWeek(endDateMeetingWeek1);
                var endDateMeetingWeek2 = LastDateOfWeek(startDateMeetingWeek2);

                //Ngày bắt đầu tuần 3 và Ngày cuối cùng tuần 3
                var startDateMeetingWeek3 = FirstDateOfWeek(endDateMeetingWeek2);
                var endDateMeetingWeek3 = LastDateOfWeek(startDateMeetingWeek3);

                //Ngày bắt đầu tuần 4 và Ngày cuối cùng tuần 4
                var startDateMeetingWeek4 = FirstDateOfWeek(endDateMeetingWeek3);
                var endDateMeetingWeek4 = LastDateOfWeek(startDateMeetingWeek4);

                //Ngày bắt đầu tuần 5 và Ngày cuối cùng tuần 5
                DateTime? startDateMeetingWeek5 = FirstDateOfWeek(endDateMeetingWeek4);
                DateTime? endDateMeetingWeek5 = last_date_month_meeting;

                int checkMeeting = checkLastDateOfMonth(endDateMeetingWeek4, last_date_month_meeting);

                if (checkMeeting == 1)
                {
                    //Nếu là ngày cuối cùng của tháng
                    endDateMeetingWeek4 = last_date_month_meeting;
                    startDateMeetingWeek5 = null;
                    endDateMeetingWeek5 = null;
                }

                var week1 = new List<CustomerMeetingForWeekModel>();
                var week2 = new List<CustomerMeetingForWeekModel>();
                var week3 = new List<CustomerMeetingForWeekModel>();
                var week4 = new List<CustomerMeetingForWeekModel>();
                var week5 = new List<CustomerMeetingForWeekModel>();

                week1 = listAllCustomerMeetingForWeek
                    .Where(x => x.StartDate >= startDateMeetingWeek1 && x.StartDate < endDateMeetingWeek1)
                    .OrderBy(z => z.StartDate).ToList();

                week2 = listAllCustomerMeetingForWeek
                    .Where(x => x.StartDate >= startDateMeetingWeek2 && x.StartDate < endDateMeetingWeek2)
                    .OrderBy(z => z.StartDate).ToList();

                week3 = listAllCustomerMeetingForWeek
                    .Where(x => x.StartDate >= startDateMeetingWeek3 && x.StartDate < endDateMeetingWeek3)
                    .OrderBy(z => z.StartDate).ToList();

                week4 = listAllCustomerMeetingForWeek
                    .Where(x => x.StartDate >= startDateMeetingWeek4 && x.StartDate < endDateMeetingWeek4)
                    .OrderBy(z => z.StartDate).ToList();

                if (checkMeeting != 1)
                {
                    week5 = listAllCustomerMeetingForWeek
                        .Where(x => x.StartDate >= startDateMeetingWeek5 && x.StartDate < endDateMeetingWeek5)
                        .OrderBy(z => z.StartDate).ToList();
                }

                customerMeetingInfor.Week1 = week1;
                customerMeetingInfor.Week2 = week2;
                customerMeetingInfor.Week3 = week3;
                customerMeetingInfor.Week4 = week4;
                customerMeetingInfor.Week5 = week5;
                return new GetHistoryCustomerMeetingResult()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = "Succees",
                    CustomerMeetingInfor = customerMeetingInfor
                };
            }
            catch (Exception e)
            {
                return new GetHistoryCustomerMeetingResult()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        private DateTime SetDate(DateTime? SendEmailDate, TimeSpan? SendEmailHour)
        {
            DateTime newDate = DateTime.Now;
            if (SendEmailDate.HasValue && SendEmailHour.HasValue)
            {
                int Year = SendEmailDate.Value.Year;
                int Month = SendEmailDate.Value.Month;
                int Day = SendEmailDate.Value.Day;

                newDate = new DateTime(Year, Month, Day) + SendEmailHour.Value;
            }
            return newDate;
        }

        public SendApprovalResult SendApproval(SendApprovalParameter parameter)
        {
            try
            {
                var approvalCus = context.SystemParameter.FirstOrDefault(sp => sp.SystemKey == "AppovalCustomer");
                if (approvalCus.SystemValue == true)
                {
                    var customerById = context.Customer.FirstOrDefault(c => c.CustomerId == parameter.CustomerId);
                    if (customerById != null)
                    {
                        customerById.IsApproval = true;
                        customerById.ApprovalStep = 2;
                        context.Customer.Update(customerById);
                        context.SaveChanges();

                        return new SendApprovalResult()
                        {
                            StatusCode = System.Net.HttpStatusCode.OK,
                            MessageCode = "Success"
                        };
                    }
                }
                else
                {
                    var cusSttId = context.CategoryType.FirstOrDefault(ct => ct.CategoryTypeCode == "THA").CategoryTypeId;
                    var newCusStatustId = context.Category.FirstOrDefault(c => c.CategoryTypeId == cusSttId && c.CategoryCode == "HDO").CategoryId;

                    var customerById = context.Customer.FirstOrDefault(c => c.CustomerId == parameter.CustomerId);
                    if (customerById != null)
                    {
                        customerById.StatusId = newCusStatustId;
                        customerById.CreatedDate = DateTime.Now;
                        context.Customer.Update(customerById);
                        context.SaveChanges();

                        return new SendApprovalResult()
                        {
                            StatusCode = System.Net.HttpStatusCode.OK,
                            MessageCode = "Success"
                        };
                    }
                }
                return new SendApprovalResult()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = "Success"
                };
            }
            catch (Exception e)
            {
                return new SendApprovalResult()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public SearchCustomerResult GetListCustomerRequestApproval(GetListCustomerRequestApprovalParameter parameter)
        {
            try
            {
                #region search customer
                List<CustomerEntityModel> listCustomer = new List<CustomerEntityModel>();

                #region Referencer của CustomerId

                var lead = context.Lead.ToList(); // cơ hội
                var quote = context.Quote.ToList(); // báo giá
                var customerOrder = context.CustomerOrder.ToList();// đơn hàng
                var receiptInvoiceMapping = context.ReceiptInvoiceMapping.ToList();     //Phiếu thu
                var bankReceiptInvoiceMapping = context.BankReceiptInvoiceMapping.ToList();     //Báo có
                var payableInvoiceMapping = context.PayableInvoiceMapping.ToList();   //Phiếu chi
                var bankPayableInvoiceMapping = context.BankPayableInvoiceMapping.ToList();   //Phiếu ủy nhiệm chi
                var customerCareCustomer = context.CustomerCareCustomer.ToList();

                #endregion

                var listAllUser = context.User.ToList();
                var listAllEmployee = context.Employee.ToList();
                var listAllContact = context.Contact.ToList();

                //check isManager
                var user = listAllUser.FirstOrDefault(x => x.UserId == parameter.UserId && x.Active == true);
                if (user == null)
                {
                    return new SearchCustomerResult
                    {
                        StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                        MessageCode = "User không có quyền truy xuất dữ liệu trong hệ thống"
                    };
                }
                if (user.EmployeeId == null || user.EmployeeId == Guid.Empty)
                {
                    return new SearchCustomerResult
                    {
                        StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                        MessageCode = "Lỗi dữ liệu"
                    };
                }

                var employeeId = user.EmployeeId;
                var employee = listAllEmployee.FirstOrDefault(x => x.EmployeeId == employeeId);
                var isManager = employee.IsManager;

                var fullName = (parameter.FirstName == null || parameter.FirstName == "") ? "" : parameter.FirstName.Trim();
                var email = parameter.Email == null ? "" : parameter.Email.Trim();
                var phone = parameter.Phone == null ? "" : parameter.Phone.Trim();
                var listServiceLevelId = parameter.CustomerServiceLevelIdList;                      //Hạng (Hiện tại chưa dùng)
                var listGroupIdList = parameter.CustomerGroupIdList;                                //Nhóm khách hàng
                var listPersonInChargeId = parameter.PersonInChargeIdList;                          //Người phụ trách
                var customerCode = parameter.CustomerCode == null ? "" : parameter.CustomerCode.Trim();       //Mã khách hàng
                var taxCode = parameter.TaxCode == null ? "" : parameter.TaxCode.Trim();            //Mã số thuế

                var personInChargeIdIsNull = parameter.NoPic;                   //Có người phụ trách?
                var businessOnly = parameter.IsBusinessCus;                     //Chỉ lấy khách hàng doanh nghiệp?
                var personalOnly = parameter.IsPersonalCus;                     //Chỉ lấy khách hàng cá nhân? 
                var HKDOnly = parameter.IsHKDCus;                               //Chỉ lấy hộ kinh doanh?
                var identificationOnly = parameter.IsIdentificationCus;         //Chỉ lấy khách hàng định danh
                var freeOnly = parameter.IsFreeCus;                             //Chỉ lấy khách hàng tự do
                var statusCusTypeId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "THA").CategoryTypeId;
                var listAllStatus = context.Category.Where(c => c.CategoryTypeId == statusCusTypeId).ToList();

                List<Customer> listAllCustomer = new List<Customer>();

                listAllCustomer = context.Customer.Where(x => (x.Active == true) &&
                                                              (listGroupIdList == null || listGroupIdList.Count == 0 || listGroupIdList.Contains(x.CustomerGroupId)) &&
                                                              (listPersonInChargeId == null || listPersonInChargeId.Count == 0 || listPersonInChargeId.Contains(x.PersonInChargeId)) &&
                                                              (customerCode == "" || x.CustomerCode.ToLower().Contains(customerCode.ToLower()))
                                                        ).ToList();

                if (personInChargeIdIsNull)
                {
                    //Nếu lấy những KH chưa có người phụ trách
                    listAllCustomer = listAllCustomer.Where(x => x.PersonInChargeId == null).ToList();
                }
                if (!businessOnly)
                {
                    //Chỉ lấy khách hàng doanh nghiệp
                    listAllCustomer = listAllCustomer.Where(x => x.CustomerType == 2 || x.CustomerType == 3).ToList();
                }
                if (!personalOnly)
                {
                    //Chỉ lấy khách hàng cá nhân
                    listAllCustomer = listAllCustomer.Where(x => x.CustomerType == 1 || x.CustomerType == 3).ToList();
                }
                if (!HKDOnly)
                {
                    //Chỉ lấy khách hàng hộ kinh doanh
                    listAllCustomer = listAllCustomer.Where(x => x.CustomerType == 1 || x.CustomerType == 2).ToList();
                }
                if (identificationOnly && !freeOnly)
                {
                    var stausCusIdenId = listAllStatus.FirstOrDefault(c => c.CategoryCode == "HDO").CategoryId;
                    listAllCustomer = listAllCustomer.Where(x => x.StatusId == stausCusIdenId).ToList();
                }
                if (freeOnly && !identificationOnly)
                {
                    var statusCusFreeId = listAllStatus.FirstOrDefault(c => c.CategoryCode == "MOI").CategoryId;
                    listAllCustomer = listAllCustomer.Where(x => x.StatusId == statusCusFreeId).ToList();
                }
                if (!identificationOnly && !freeOnly)
                {
                    var stausCusIdenId = listAllStatus.FirstOrDefault(c => c.CategoryCode == "HDO").CategoryId;
                    var statusCusFreeId = listAllStatus.FirstOrDefault(c => c.CategoryCode == "MOI").CategoryId;
                    listAllCustomer = listAllCustomer.Where(x => x.StatusId != statusCusFreeId && x.StatusId != stausCusIdenId).ToList();
                }
                //List Customer Type: CUS and CUS_CON
                List<string> listContactCustomerObjectType = new List<string>();
                listContactCustomerObjectType.Add(ObjectType.CUSTOMER);
                listContactCustomerObjectType.Add(ObjectType.CUSTOMERCONTACT);

                //Lấy tất cả contact của KH (CUS và CUS_CON)
                var listAllCustomerContact = context.Contact.Where(x => (x.Active == true) &&
                                                                        (listContactCustomerObjectType.Contains(x.ObjectType)) &&
                                                                        (fullName == "" || ((x.FirstName ?? "").ToLower() + " " + (x.LastName ?? "").ToLower()).Contains(fullName.ToLower())) &&
                                                                        (email == "" || (x.Email != null && x.Email.ToLower().Contains(email.ToLower())) || (x.WorkEmail != null && x.WorkEmail.ToLower().Contains(email.ToLower())) || (x.OtherEmail != null && x.OtherEmail.ToLower().Contains(email.ToLower()))) &&
                                                                        (phone == "" || (x.Phone != null && x.Phone.ToLower().Contains(phone.ToLower())) || (x.WorkPhone != null && x.WorkPhone.ToLower().Contains(phone.ToLower())) || (x.OtherPhone != null && x.OtherPhone.ToLower().Contains(phone.ToLower()))) &&
                                                                        (taxCode == "" || (x.TaxCode ?? "").ToLower().Contains(taxCode.ToLower()))
                                                                  ).ToList();

                //Lọc ra các ObjectId bị trùng
                List<Contact> listCustomerContact = new List<Contact>();
                List<Guid> listObjectId = new List<Guid>();
                listAllCustomerContact.ForEach(item =>
                {
                    if (item.ContactId != null && item.ContactId != Guid.Empty)
                    {
                        var dupblicateObjectId = listObjectId.FirstOrDefault(x => x == item.ObjectId);
                        if (dupblicateObjectId == Guid.Empty)
                        {
                            listObjectId.Add(item.ObjectId);
                        }
                    }
                });

                //Lấy lại contact của listObjectId với ObjectType = CUS
                if (listObjectId.Count > 0)
                {
                    listCustomerContact = listAllContact.Where(x => x.ObjectType == ObjectType.CUSTOMER && (listObjectId == null || listObjectId.Count == 0 || listObjectId.Contains(x.ObjectId))).ToList();
                }

                if (isManager)
                {
                    //Lấy list phòng ban con của user
                    List<Guid?> listGetAllChild = new List<Guid?>();    //List phòng ban: chính nó và các phòng ban cấp dưới của nó
                    if (employee.OrganizationId != null && isManager)
                    {
                        listGetAllChild.Add(employee.OrganizationId.Value);
                        listGetAllChild = getOrganizationChildrenId(employee.OrganizationId.Value, listGetAllChild);
                    }
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

                    //Nếu là quản lý
                    listAllCustomer = listAllCustomer.Where(x => (x.PersonInChargeId != null && (listEmployeeInChargeByManagerId == null || listEmployeeInChargeByManagerId.Count == 0 || listEmployeeInChargeByManagerId.FirstOrDefault(y => y.Equals(x.PersonInChargeId.Value)) != Guid.Empty)) ||
                                                                 (x.PersonInChargeId == null && (listUserByManagerId == null || listUserByManagerId.Count == 0 || listUserByManagerId.FirstOrDefault(y => y.Equals(x.CreatedById)) != Guid.Empty))
                                                           ).ToList();

                    List<Customer> listAllCustomerForPersonInChargeId = new List<Customer>();
                    List<Customer> listAllCustomerForCreatedById = new List<Customer>();

                    listAllCustomer.ForEach(item =>
                    {
                        var customer_contact = listCustomerContact.FirstOrDefault(x => x.ObjectId == item.CustomerId);
                        if (customer_contact != null)
                        {
                            CustomerEntityModel customer = new CustomerEntityModel();
                            var personInCharge = listAllEmployee.FirstOrDefault(x => x.EmployeeId == item.PersonInChargeId);

                            customer.CustomerId = item.CustomerId;
                            customer.CustomerCode = item.CustomerCode != null ? item.CustomerCode.Trim() : "";
                            customer.CustomerName = item.CustomerName != null ? item.CustomerName.Trim() : "";
                            customer.ContactId = customer_contact.ContactId;
                            customer.CustomerEmail = customer_contact.Email != null ? customer_contact.Email.Trim() : "";
                            customer.CustomerPhone = customer_contact.Phone != null ? customer_contact.Phone.Trim() : "";
                            customer.PicName = (personInCharge != null ? (personInCharge.EmployeeName != null ? personInCharge.EmployeeName.Trim() : "") : "");
                            customer.CountCustomerInfo = CheckCountInformationCustomer(item.CustomerId, lead, quote, customerOrder, receiptInvoiceMapping, bankReceiptInvoiceMapping, customerCareCustomer, payableInvoiceMapping, bankPayableInvoiceMapping);
                            customer.StatusId = item.StatusId;
                            customer.CusAvatarUrl = "";
                            customer.PicAvatarUrl = "";
                            customer.IsApproval = item.IsApproval;
                            customer.ApprovalStep = item.ApprovalStep;
                            customer.CreatedDate = item.CreatedDate;

                            listCustomer.Add(customer);
                        }
                    });
                }
                else
                {
                    //Nếu không phải quản lý
                    listAllCustomer = listAllCustomer.Where(x => (x.PersonInChargeId != null && x.PersonInChargeId == employeeId) || (x.PersonInChargeId == null && x.CreatedById.Equals(user.UserId))).ToList();

                    listAllCustomer.ForEach(item =>
                    {
                        var customer_contact = listCustomerContact.FirstOrDefault(x => x.ObjectId == item.CustomerId);
                        if (customer_contact != null)
                        {
                            CustomerEntityModel customer = new CustomerEntityModel();
                            var personInCharge = listAllEmployee.FirstOrDefault(x => x.EmployeeId == item.PersonInChargeId);

                            customer.CustomerId = item.CustomerId;
                            customer.CustomerCode = item.CustomerCode != null ? item.CustomerCode.Trim() : "";
                            customer.CustomerName = item.CustomerName != null ? item.CustomerName.Trim() : "";
                            customer.ContactId = customer_contact.ContactId;
                            customer.CustomerEmail = customer_contact.Email != null ? customer_contact.Email.Trim() : "";
                            customer.CustomerPhone = customer_contact.Phone != null ? customer_contact.Phone.Trim() : "";
                            customer.PicName = (personInCharge != null ? (personInCharge.EmployeeName != null ? personInCharge.EmployeeName.Trim() : "") : "");
                            customer.CountCustomerInfo = CheckCountInformationCustomer(item.CustomerId, lead, quote, customerOrder, receiptInvoiceMapping, bankReceiptInvoiceMapping, customerCareCustomer, payableInvoiceMapping, bankPayableInvoiceMapping);
                            customer.StatusId = item.StatusId;
                            customer.CusAvatarUrl = "";
                            customer.PicAvatarUrl = "";
                            customer.IsApproval = item.IsApproval;
                            customer.ApprovalStep = item.ApprovalStep;
                            customer.CreatedDate = item.CreatedDate;

                            listCustomer.Add(customer);
                        }
                    });
                }
                listCustomer.ForEach(item =>
                {
                    item.StatusName = listAllStatus.FirstOrDefault(c => c.CategoryId == item.StatusId).CategoryName;
                    var statusCode = listAllStatus.FirstOrDefault(c => c.CategoryId == item.StatusId).CategoryCode;
                    switch (statusCode)
                    {
                        case "HDO":
                            item.BackgroupStatus = "#0f62fe";
                            break;
                        case "MOI":
                            item.BackgroupStatus = "#ff3b30";
                            break;
                    }
                });
                listCustomer = listCustomer.OrderByDescending(x => x.CreatedDate).ToList();
                #endregion search customer

                // List khách hàng đang chờ phê duyệt
                List<CustomerEntityModel> listCustomerApproval = new List<CustomerEntityModel>();

                if (freeOnly)
                {
                    // Lấy khách hàng đang chờ phê duyệt
                    var customerApproval = listCustomer.Where(c => c.IsApproval == true).ToList();
                    // Lấy phê duyệt khách hàng định danh
                    var workFlowsCustomerDD = context.WorkFlows.FirstOrDefault(wf => wf.WorkflowCode == "PDKHDD");
                    // Lấy các bước phê duyệt khách hàng định danh
                    var workFlowSteps = context.WorkFlowSteps.Where(wf => wf.WorkflowId == workFlowsCustomerDD.WorkFlowId).ToList();

                    workFlowSteps.ForEach(item =>
                    {
                        if (item.StepNumber != 1)
                        {
                            if ((item.ApprovebyPosition && item.ApproverPositionId == employee.PositionId)
                            || (!item.ApprovebyPosition && item.ApproverId == employee.EmployeeId))
                            {
                                var customerStep = customerApproval.Where(ca => ca.ApprovalStep == item.StepNumber).ToList();
                                customerStep.ForEach(cusStep =>
                                {
                                    listCustomerApproval.Add(cusStep);
                                });
                            }
                        }
                    });
                }
                if (identificationOnly)
                {
                    listCustomerApproval = listCustomer.Where(d => d.CountCustomerInfo == 0).ToList();
                }

                return new SearchCustomerResult()
                {
                    ListCustomer = listCustomerApproval,
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = "Success"
                };
            }
            catch (Exception e)
            {
                return new SearchCustomerResult()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public SendApprovalResult ApprovalOrRejectCustomer(ApprovalOrRejectCustomerParameter parameter)
        {
            try
            {
                var message = "";
                var customers = context.Customer.Where(c => c.Active == true).ToList();
                var categoryType = context.CategoryType.FirstOrDefault(ca => ca.CategoryTypeCode == "THA");
                var categoryDD = context.Category.FirstOrDefault(ca => ca.CategoryTypeId == categoryType.CategoryTypeId && ca.CategoryCode == "HDO");
                var categoryNew = context.Category.FirstOrDefault(ca => ca.CategoryTypeId == categoryType.CategoryTypeId && ca.CategoryCode == "MOI");
                if (parameter.IsFreeCus)
                {
                    // Lấy phê duyệt khách hàng định danh
                    var workFlowsCustomerDD = context.WorkFlows.FirstOrDefault(wf => wf.WorkflowCode == "PDKHDD");
                    // Lấy các bước phê duyệt khách hàng định danh
                    var workFlowSteps = context.WorkFlowSteps.Where(wf => wf.WorkflowId == workFlowsCustomerDD.WorkFlowId).ToList();

                    if (parameter.IsApproval)
                    {
                        parameter.ListCustomerId.ForEach(item =>
                        {
                            var customer = customers.FirstOrDefault(cu => cu.CustomerId == item);
                            var stepNext = workFlowSteps.FirstOrDefault(w => w.StepNumber == customer.ApprovalStep);
                            customer.ApprovalStep = stepNext.NextStepNumber;
                            message = "Gửi phê duyệt định danh thành công";
                            if (customer.ApprovalStep == 0)
                            {
                                customer.StatusId = categoryDD.CategoryId;
                                customer.IsApproval = false;
                                customer.CreatedDate = DateTime.Now;
                                message = "Chuyển định danh thành công";
                            }
                            context.Customer.Update(customer);
                        });
                    }
                    else
                    {
                        parameter.ListCustomerId.ForEach(item =>
                        {
                            var customer = customers.FirstOrDefault(cu => cu.CustomerId == item);
                            var stepNext = workFlowSteps.FirstOrDefault(w => w.StepNumber == customer.ApprovalStep);
                            customer.ApprovalStep = stepNext.BackStepNumber;

                            if (customer.ApprovalStep < 2)
                            {
                                customer.IsApproval = false;
                            }

                            message = "Từ chối phê duyệt định danh thành công";
                            context.Customer.Update(customer);
                        });
                    }
                }
                else
                {
                    parameter.ListCustomerId.ForEach(item =>
                    {
                        var customer = customers.FirstOrDefault(cu => cu.CustomerId == item);
                        customer.StatusId = categoryNew.CategoryId;
                        message = "Chuyển tự do thành công";
                    });
                }
                context.SaveChanges();
                return new SendApprovalResult()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = message
                };
            }
            catch (Exception e)
            {
                return new SendApprovalResult()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetDataCreatePotentialCustomerResult GetDataCreatePotentialCustomer(GetDataCreatePotentialCustomerParameter parameter)
        {
            try
            {
                //result
                var ListInvestFund = new List<CategoryEntityModel>();
                var ListGroupCus = new List<CategoryEntityModel>();
                var ListEmployeeEntityModel = new List<DataAccess.Models.Employee.EmployeeEntityModel>();

                var categoryTypeList = context.CategoryType.Where(cty => cty.Active == true).ToList();
                var categoryList = context.Category.Where(ct => ct.Active == true).ToList();
                var employeeList = context.Employee.Where(emp => emp.Active == true).ToList();

                var ListProvinceEntityModel = new List<DataAccess.Models.Address.ProvinceEntityModel>();
                var ListDistrictEntityModel = new List<DataAccess.Models.Address.DistrictEntityModel>();
                var ListWardEntityModel = new List<DataAccess.Models.Address.WardEntityModel>();
                var ListCustomerCode = new List<string>();
                var ListCustomerTax = new List<string>();

                var districsList = context.District.Where(emp => emp.Active == true).ToList();
                var wardsList = context.Ward.Where(emp => emp.Active == true).ToList();

                #region Nhóm khu vực
                var listArea = context.GeographicalArea.Where(c => c.Active == true)
                    .Select(m => new GeographicalAreaEntityModel
                    {
                        GeographicalAreaId = m.GeographicalAreaId,
                        GeographicalAreaCode = m.GeographicalAreaCode,
                        GeographicalAreaName = m.GeographicalAreaName
                    }).ToList();
                #endregion

                #region Define Category Code
                var listCategoryCode = new List<string>();
                var customerGroupCode = "IVF"; //nguon tiem nang
                listCategoryCode.Add(customerGroupCode);
                var portalUserCode = "PortalUser"; //loại portalUser
                #endregion

                #region Get data from Category table
                var listCategoryTypeEntity = categoryTypeList.Where(w => listCategoryCode.Contains(w.CategoryTypeCode) && w.Active == true).ToList();
                var listCateTypeId = new List<Guid>();
                listCategoryTypeEntity?.ForEach(type =>
                {
                    listCateTypeId.Add(type.CategoryTypeId);
                });
                var listCategoryEntity = categoryList.Where(w => listCateTypeId.Contains(w.CategoryTypeId) && w.Active == true).ToList(); //list master data của category

                //get customer group
                var customerGroupTypeId = listCategoryTypeEntity.Where(w => w.CategoryTypeCode == customerGroupCode).FirstOrDefault()?.CategoryTypeId;
                var listCustomerGroupEntity = listCategoryEntity.Where(w => w.CategoryTypeId == customerGroupTypeId && w.CategoryCode != "POR").ToList(); //loại Khách hàng Portal                                              
                listCustomerGroupEntity?.ForEach(group =>
                {
                    ListInvestFund.Add(new CategoryEntityModel
                    {
                        CategoryId = group.CategoryId,
                        CategoryName = group.CategoryName,
                        CategoryCode = group.CategoryCode,
                        CategoryTypeId = group.CategoryTypeId,
                        IsDefault = group.IsDefauld
                    });
                });
                #endregion

                #region Get Employee Care Staff List

                var listEmployeeEntity = employeeList.Where(w => w.Active == true && w.EmployeeCode != portalUserCode).ToList(); //loai portal user

                var employeeById = listEmployeeEntity.Where(w => w.EmployeeId == parameter.EmployeeId).FirstOrDefault();
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
                    var listEmployeeFiltered = listEmployeeEntity.Where(w => listOrganizationChildrenId.Contains(w.OrganizationId)).Select(w => new
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
                            EmployeeCode = emp.EmployeeCode
                        });
                    });
                }
                else
                {
                    //Nhân viên: chỉ lấy nhân viên đó
                    var employeeId = listEmployeeEntity.Where(e => e.EmployeeId == parameter.EmployeeId).FirstOrDefault();
                    ListEmployeeEntityModel.Add(new Models.Employee.EmployeeEntityModel
                    {
                        EmployeeId = employeeId.EmployeeId,
                        EmployeeName = employeeId.EmployeeName,
                        EmployeeCode = employeeId.EmployeeCode
                    });
                }

                #endregion

                #region Get Position
                //var categoryTypeId_9 = categoryTypeList.FirstOrDefault(x => x.CategoryTypeCode == "CVU").CategoryTypeId;
                //ListPositionEntity = categoryList.Where(x => x.CategoryTypeId == categoryTypeId_9).Select(y =>
                //    new CategoryEntityModel
                //    {
                //        CategoryId = y.CategoryId,
                //        CategoryCode = y.CategoryCode,
                //        CategoryName = y.CategoryName
                //    }).ToList();
                #endregion

                ListProvinceEntityModel = context.Province.Select(p => new ProvinceEntityModel()
                {
                    ProvinceId = p.ProvinceId,
                    ProvinceName = p.ProvinceName,
                    ProvinceCode = p.ProvinceCode,
                    ProvinceType = p.ProvinceType
                }).OrderBy(p => p.ProvinceName).ToList();

                #region Get List Customer Code
                var listCustomerTaxEntity = context.Contact.Where(w => w.Active == true && w.ObjectType == "CUS" && w.TaxCode != null && w.TaxCode.Trim() != "").Select(w => new
                {
                    TaxCode = w.TaxCode
                }).Distinct().ToList();
                listCustomerTaxEntity.ForEach(code =>
                {
                    ListCustomerTax.Add(code.TaxCode);
                });
                #endregion

                ListProvinceEntityModel.ForEach(p =>
                {
                    var districtList = districsList.Where(d => d.ProvinceId == p.ProvinceId)
                        .Select(d => new DistrictEntityModel()
                        {
                            DistrictId = d.DistrictId,
                            DistrictName = d.DistrictName,
                            DistrictCode = d.DistrictCode,
                            DistrictType = d.DistrictType,
                            ProvinceId = d.ProvinceId
                        }).OrderBy(d => d.DistrictName).ToList();

                    districtList.ForEach(d =>
                    {
                        var wardList = wardsList.Where(w => w.DistrictId == d.DistrictId).Select(w =>
                            new WardEntityModel()
                            {
                                WardId = w.WardId,
                                WardName = w.WardName,
                                WardCode = w.WardCode,
                                WardType = w.WardType,
                                DistrictId = w.DistrictId
                            }).OrderBy(w => w.WardName).ToList();
                        d.WardList = wardList;
                    });

                    p.DistrictList = districtList;
                });

                #region Nhóm khách hàng

                var categoryTypeId = categoryTypeList.FirstOrDefault(x => x.CategoryTypeCode == "NHA")?.CategoryTypeId;
                var ListGroupCustomer = categoryList.Where(x => x.CategoryTypeId == categoryTypeId).ToList();
                ListGroupCustomer?.ForEach(group =>
                {
                    ListGroupCus.Add(new CategoryEntityModel
                    {
                        CategoryId = group.CategoryId,
                        CategoryName = group.CategoryName,
                        CategoryCode = group.CategoryCode,
                        CategoryTypeId = group.CategoryTypeId,
                        IsDefault = group.IsDefauld
                    });
                });


                #endregion

                #region Danh sách khách hàng tiềm năng

                var ListCustomer = new List<CustomerEntityModel>();
                var _employeeId = context.User.FirstOrDefault(x => x.UserId == parameter.UserId).EmployeeId;
                var employee = context.Employee.FirstOrDefault(x => x.EmployeeId == _employeeId);
                var statusCustomerType =
                    context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "THA").CategoryTypeId;
                var statusCustomer = context.Category
                    .FirstOrDefault(x => x.CategoryCode == "MOI" && x.CategoryTypeId == statusCustomerType).CategoryId;

                if (isManage == true)
                {
                    //Quản lí: lấy tất cả nhân viên phòng ban đó và phòng ban dưới cấp
                    var currentOrganization = employee.OrganizationId;
                    List<Guid?> listOrganizationChildrenId = new List<Guid?>();
                    listOrganizationChildrenId.Add(currentOrganization);
                    var organizationList = context.Organization.Where(w => w.Active == true).ToList();
                    getOrganizationChildrenId(organizationList, currentOrganization, listOrganizationChildrenId);
                    var listEmployeeId = listEmployeeEntity
                        .Where(w => listOrganizationChildrenId.Contains(w.OrganizationId)).Select(w => w.EmployeeId)
                        .ToList();

                    ListCustomer = context.Customer
                        .Where(x => x.Active == true && x.CustomerType == 1 &&
                                    x.StatusId == statusCustomer &&
                                    listEmployeeId.Contains(x.PersonInChargeId.Value)).Select(y =>
                            new CustomerEntityModel
                            {
                                CustomerId = y.CustomerId,
                                CustomerName = y.CustomerName
                            }).ToList();
                }
                else
                {
                    //Nhân viên: chỉ lấy nhân viên đó
                    ListCustomer = context.Customer
                        .Where(x => x.Active == true && x.CustomerType == 1 &&
                                    x.StatusId == statusCustomer &&
                                    x.PersonInChargeId == _employeeId).Select(y =>
                            new CustomerEntityModel
                            {
                                CustomerId = y.CustomerId,
                                CustomerName = y.CustomerName
                            }).ToList();
                }

                #endregion

                #region Danh sách nhân viên take care

                var listEmpoloyeeTakeCare = context.Employee.Where(c => c.Active == true && c.IsTakeCare == true)
                    .Select(m => new EmployeeEntityModel
                    {
                        EmployeeId = m.EmployeeId,
                        EmployeeCode = m.EmployeeCode,
                        EmployeeName = m.EmployeeName,
                        EmployeeCodeName = m.EmployeeCode + " - " + m.EmployeeName
                    }).OrderBy(m => m.EmployeeName).ToList();

                #endregion



                return new GetDataCreatePotentialCustomerResult()
                {
                    ListInvestFund = ListInvestFund ?? new List<CategoryEntityModel>(),
                    ListEmployeeModel = ListEmployeeEntityModel ?? new List<EmployeeEntityModel>(),
                    ListGroupCustomer = ListGroupCus,
                    ListArea = listArea,
                    ListProvinceEntityModel = ListProvinceEntityModel,
                    ListDistrictEntityModel = ListDistrictEntityModel,
                    ListWardEntityModel = ListWardEntityModel,
                    ListCustomer = ListCustomer,
                    ListEmployeeTakeCare = listEmpoloyeeTakeCare,
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = "OK"
                };
            }
            catch (Exception e)
            {
                return new GetDataCreatePotentialCustomerResult()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message,
                };
            }
        }

        public GetDataDetailPotentialCustomerResult GetDataDetailPotentialCustomer(GetDataDetailPotentialCustomerParameter parameter)
        {
            try
            {
                //result
                var ListInvestFund = new List<CategoryEntityModel>();
                var ListEmployeeEntityModel = new List<EmployeeEntityModel>();

                var categoryTypeList = context.CategoryType.Where(cty => cty.Active == true).ToList();
                var categoryList = context.Category.Where(ct => ct.Active == true).ToList();
                var employeeList = context.Employee.Where(emp => emp.Active == true).ToList();
                var note = context.Note.Where(cu => cu.ObjectId == parameter.CustomerId).ToList();

                #region Define Category Code

                var listCategoryCode = new List<string>();
                var customerGroupCode = "IVF"; //nguon tiem nang
                listCategoryCode.Add(customerGroupCode);
                var portalUserCode = "PortalUser"; //loại portalUser

                #endregion

                #region Get data from Category table

                var listCategoryTypeEntity = categoryTypeList.Where(w => listCategoryCode.Contains(w.CategoryTypeCode) && w.Active == true).ToList();
                var listCateTypeId = new List<Guid>();
                listCategoryTypeEntity?.ForEach(type =>
                {
                    listCateTypeId.Add(type.CategoryTypeId);
                });
                var listCategoryEntity = categoryList.Where(w => listCateTypeId.Contains(w.CategoryTypeId) && w.Active == true).ToList(); //list master data của category

                //get customer group
                var customerGroupTypeId = listCategoryTypeEntity.Where(w => w.CategoryTypeCode == customerGroupCode).FirstOrDefault()?.CategoryTypeId;
                var listCustomerGroupEntity = listCategoryEntity.Where(w => w.CategoryTypeId == customerGroupTypeId && w.CategoryCode != "POR").ToList(); //loại Khách hàng Portal                                              
                listCustomerGroupEntity?.ForEach(group =>
                {
                    ListInvestFund.Add(new CategoryEntityModel
                    {
                        CategoryId = group.CategoryId,
                        CategoryName = group.CategoryName,
                        CategoryCode = group.CategoryCode,
                        CategoryTypeId = group.CategoryTypeId,
                        IsDefault = group.IsDefauld
                    });
                });

                #endregion

                #region Lấy danh sách người phụ trách

                var listEmployeeEntity = employeeList.Where(w => w.Active == true && w.EmployeeCode != portalUserCode).ToList(); //loai portal user

                var employeeIdUser = context.User.FirstOrDefault(w => w.UserId == parameter.UserId).EmployeeId;

                var employeeById = listEmployeeEntity.Where(w => w.EmployeeId == employeeIdUser).FirstOrDefault();

                #region Lấy thông tin chi tiết khách hàng tiềm năng

                var customerResult = context.Customer.FirstOrDefault(f => f.CustomerId == parameter.CustomerId && f.Active == true);
                //chuyển về entitymodel
                var customer = new CustomerEntityModel(customerResult);

                var contactResult = context.Contact.FirstOrDefault(f =>
                    f.ObjectId == parameter.CustomerId && f.Active == true && f.ObjectType == "CUS");
                var contact = new ContactEntityModel(contactResult);

                #endregion

                //Kiểm tra xem user đang đăng nhập có phải nhân viên take care của khách hàng ko?
                bool isEmployeeTakeCare = false;
                if (customer.EmployeeTakeCareId == employeeById.EmployeeId)
                {
                    isEmployeeTakeCare = true;
                }

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
                            OrganizationId = w.OrganizationId
                        }).ToList();

                    if (isEmployeeTakeCare)
                    {
                        var existsEmp =
                            listEmployeeFiltered.FirstOrDefault(x => x.EmployeeId == customer.PersonInChargeId);

                        if (existsEmp == null)
                        {
                            var empExtend =
                                listEmployeeEntity.Where(x => x.EmployeeId == customer.PersonInChargeId).Select(w => new
                                {
                                    EmployeeId = w.EmployeeId,
                                    EmployeeName = w.EmployeeName,
                                    EmployeeCode = w.EmployeeCode,
                                    OrganizationId = w.OrganizationId
                                }).FirstOrDefault();

                            if (empExtend != null)
                            {
                                listEmployeeFiltered.Add(empExtend);
                            }
                        }
                    }

                    listEmployeeFiltered?.ForEach(emp =>
                    {
                        ListEmployeeEntityModel.Add(new EmployeeEntityModel
                        {
                            EmployeeId = emp.EmployeeId,
                            EmployeeName = emp.EmployeeName,
                            EmployeeCode = emp.EmployeeCode,
                            OrganizationId = emp.OrganizationId
                        });
                    });
                }
                else
                {
                    //Nhân viên: chỉ lấy nhân viên đó
                    var employeeId = listEmployeeEntity.Where(e => e.EmployeeId == employeeIdUser).FirstOrDefault();

                    if (isEmployeeTakeCare)
                    {
                        if (employeeId.EmployeeId != customer.PersonInChargeId)
                        {
                            var empExtend =
                                listEmployeeEntity.Where(x => x.EmployeeId == customer.PersonInChargeId).Select(w => new
                                {
                                    EmployeeId = w.EmployeeId,
                                    EmployeeName = w.EmployeeName,
                                    EmployeeCode = w.EmployeeCode,
                                    OrganizationId = w.OrganizationId
                                }).FirstOrDefault();

                            if (empExtend != null)
                            {
                                ListEmployeeEntityModel.Add(new EmployeeEntityModel
                                {
                                    EmployeeId = empExtend.EmployeeId,
                                    EmployeeName = empExtend.EmployeeName,
                                    EmployeeCode = empExtend.EmployeeCode,
                                    OrganizationId = empExtend.OrganizationId
                                });
                            }
                        }
                    }

                    ListEmployeeEntityModel.Add(new EmployeeEntityModel
                    {
                        EmployeeId = employeeId.EmployeeId,
                        EmployeeName = employeeId.EmployeeName,
                        EmployeeCode = employeeId.EmployeeCode,
                        OrganizationId = employeeId.OrganizationId
                    });
                }

                #endregion

                #region Lấy danh sách liên hệ
                var listContactResult = context.Contact.Where(w => w.ObjectId == parameter.CustomerId && w.Active == true && w.ObjectType == "CUS_CON").ToList() ?? new List<Contact>();
                //Chuyển về entitymodel
                var listContact = new List<ContactEntityModel>();
                listContactResult.ForEach(item =>
                {
                    listContact.Add(new ContactEntityModel(item));
                });
                #endregion

                #region Lấy thông tin File đính kèm
                var listFileDocument = context.FileInFolder.Where(w => w.Active == true && w.ObjectId == parameter.CustomerId && w.ObjectType == "QLKHTN").ToList() ?? new List<FileInFolder>();

                var listUser = context.User.ToList();

                var listFileByPotentialCustomer = new List<DataAccess.Models.Folder.FileInFolderEntityModel>();
                listFileDocument.ForEach(file =>
                {
                    //convert name of file
                    List<string> fullnameArr = file.FileName?.Split('_')?.ToList() ?? new List<string>();
                    if (fullnameArr.Any()) //prevent IndexOutOfRangeException for empty list
                    {
                        fullnameArr.RemoveAt(fullnameArr.Count - 1);
                    }
                    string fullName = string.Join("", fullnameArr);
                    var fileExtension = file.FileExtension;
                    var createByUserName = listUser.FirstOrDefault(f => f.UserId == file.CreatedById)?.UserName ?? "";

                    var newItem = new DataAccess.Models.Folder.FileInFolderEntityModel();
                    newItem.FileInFolderId = file.FileInFolderId;
                    newItem.FolderId = file.FolderId;
                    newItem.FileName = fullName + "." + fileExtension;
                    newItem.ObjectId = file.ObjectId;
                    newItem.ObjectType = file.ObjectType;
                    newItem.Size = file.Size;
                    newItem.Active = file.Active;
                    newItem.FileExtension = file.FileExtension;
                    newItem.CreatedById = file.CreatedById;
                    newItem.CreatedDate = file.CreatedDate;
                    newItem.UpdatedById = file.UpdatedById;
                    newItem.UpdatedDate = file.UpdatedDate;
                    newItem.CreatedByName = createByUserName;

                    listFileByPotentialCustomer.Add(newItem);
                });
                #endregion

                #region Get list link dinh kem`
                var ListLinkOfDocument = new List<DataAccess.Models.Document.LinkOfDocumentEntityModel>();
                var listLinkOfDocEntity = context.LinkOfDocument.Where(w => w.ObjectId == parameter.CustomerId && w.Active == true && w.ObjectType == "QLKHTN").ToList();
                listLinkOfDocEntity?.ForEach(item =>
                {
                    ListLinkOfDocument.Add(new Models.Document.LinkOfDocumentEntityModel
                    {
                        LinkOfDocumentId = item.LinkOfDocumentId,
                        LinkName = item.LinkName,
                        LinkValue = item.LinkValue,
                        CreatedByName = listUser.FirstOrDefault(f => f.UserId == item.CreatedById)?.UserName ?? "",
                        CreatedById = item.CreatedById,
                        CreatedDate = item.CreatedDate
                    });
                });
                #endregion

                #region danh sách sản phẩm(hàng hóa) master data
                var listProduct = context.Product.Where(w => w.Active == true).ToList() ?? new List<Product>();
                var listPriceProduct = context.PriceProduct.Where(w => w.Active == true).ToList() ?? new List<PriceProduct>();
                var listProductWithFixedPrice = new List<DataAccess.Models.Product.ProductEntityModel>();

                listProduct.ForEach(product =>
                {
                    //tìm giá niêm yết của sản phẩm
                    var currentTime = DateTime.Now;
                    var maxFixedPrice = listPriceProduct.Where(w => w.ProductId == product.ProductId && w.EffectiveDate <= product.CreatedDate)
                                                             .Select(w => w.PriceVnd)
                                                             .DefaultIfEmpty(0)
                                                              .Max();
                    //decimal maxFixedPrice = listPriceOfProduct.Max();

                    listProductWithFixedPrice.Add(new Models.Product.ProductEntityModel
                    {
                        ProductId = product.ProductId,
                        ProductName = product.ProductName,
                        ProductCode = product.ProductCode,
                        FixedPrice = maxFixedPrice
                    });
                });
                #endregion

                #region Danh sách sản phẩm theo từng khách hàng
                var listPotentialCustomerProductEntity = context.PotentialCustomerProduct.Where(w => w.CustomerId == parameter.CustomerId && w.Active == true).ToList() ?? new List<PotentialCustomerProduct>();
                var listPotentialCustomerProduct = new List<DataAccess.Models.Customer.PotentialCustomerProductEntityModel>();

                listPotentialCustomerProductEntity?.ForEach(product =>
                {
                    listPotentialCustomerProduct.Add(new PotentialCustomerProductEntityModel
                    {
                        PotentialCustomerProductId = product.PotentialCustomerProductId,
                        ProductId = product.ProductId,
                        CustomerId = product.CustomerId,
                        IsInTheSystem = product.IsInTheSystem,
                        ProductName = product.ProductName,
                        //ProductUnit = product.ProductUnit,
                        ProductFixedPrice = product.ProductFixedPrice ?? 0,
                        ProductUnitPrice = product.ProductUnitPrice ?? 0,
                        ProductNote = product.ProductNote
                    });
                });
                #endregion

                #region Lấy danh sách người tham gia
                var listParticipants = employeeList.Select(
                  c => new EmployeeEntityModel
                  {
                      EmployeeId = c.EmployeeId,
                      EmployeeCode = c.EmployeeCode,
                      EmployeeName = c.EmployeeName
                  }).ToList();
                #endregion

                #region Lấy list thông tin CSKH theo tháng hiện tại
                var customerCares = context.CustomerCare.Where(cc => cc.ActiveDate != null).ToList();
                var customerCareCustomer = context.CustomerCareCustomer.Where(cu => cu.CustomerId == parameter.CustomerId).ToList();
                var listEmployee = context.Employee.Where(e => e.Active == true).ToList();
                var listEmployeePosition = context.Position.ToList();

                var user = listUser.FirstOrDefault(x => x.UserId == parameter.UserId);
                var employee = listEmployee.FirstOrDefault(x => x.EmployeeId == user.EmployeeId);

                //Hình thức
                var customerCareCategoryTypeId =
                    context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "HCS").CategoryTypeId;
                var listCustomerCareCategory =
                    context.Category.Where(x => x.CategoryTypeId == customerCareCategoryTypeId).ToList();
                var listTypeOfCustomerCare1 = listCustomerCareCategory
                    .Where(x => x.CategoryCode == "Gift" || x.CategoryCode == "CallPhone").Select(y => y.CategoryId)
                    .ToList();
                var listTypeOfCustomerCare2 = listCustomerCareCategory
                    .Where(x => x.CategoryCode == "Email" || x.CategoryCode == "SMS").Select(y => y.CategoryId)
                    .ToList();

                //Trạng thái
                var statusOfCustomerCareCategoryId =
                    context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "TCS").CategoryTypeId;
                var statusActiveOfCustomerCare = context.Category.FirstOrDefault(x =>
                    x.CategoryTypeId == statusOfCustomerCareCategoryId && x.CategoryCode == "Active").CategoryId;

                var listCustomerCareInfor = new List<CustomerCareInforModel>();
                var listAllCustomerCareForWeek = new List<CustomerCareForWeekModel>();

                /*
                 * Lọc ra các chương trình CSKH thõa mãn các điều kiện:
                 * - Hình thức là Tặng quà và Gọi điện
                 * - Có ngày kích hoạt trong tháng, năm được chọn
                 */
                var listCustomerCare1 = customerCares.Where(x =>
                        listTypeOfCustomerCare1.Contains(x.CustomerCareContactType.Value) &&
                        x.ActiveDate.Value.Month == (DateTime.Now).Month &&
                        x.ActiveDate.Value.Year == (DateTime.Now).Year)
                    .ToList();

                //Lọc ra các chương trình CSKH mà có Khách hàng hiện tại tham gia
                var listCustomerCareId1 = listCustomerCare1.Select(x => x.CustomerCareId).ToList();

                if (listCustomerCareId1.Count > 0)
                {
                    var listCustomerCareIdForCustomer = customerCareCustomer
                        .Where(x => listCustomerCareId1.Contains(x.CustomerCareId.Value) &&
                                    x.CustomerId == parameter.CustomerId).Select(x => x.CustomerCareId).ToList();

                    if (listCustomerCareIdForCustomer.Count > 0)
                    {
                        listCustomerCare1 = listCustomerCare1
                            .Where(x => listCustomerCareIdForCustomer.Contains(x.CustomerCareId)).ToList();
                    }
                    else
                    {
                        listCustomerCare1 = new List<CustomerCare>();
                    }
                }

                /*
                 * Lấy list CustomerCareId trong bảng Queue thõa màn các điều kiện sau:
                 * - IsSend = true (Đã gửi)
                 * - Là gửi Email hoặc SMS
                 * - Có ngày gửi (SenDate) trong tháng, năm được chọn
                 */
                var listQueueCustomerCare2 = context.Queue.Where(x =>
                    x.IsSend == true &&
                    (x.Method == "Email" || x.Method == "SMS") && x.SenDate.Value.Month == (DateTime.Now).Month &&
                    x.SenDate.Value.Year == (DateTime.Now).Year && x.CustomerId == parameter.CustomerId).ToList();

                var listQueueCustomerCare2Id = listQueueCustomerCare2.Select(y => y.CustomerCareId).Distinct().ToList();

                var listCustomerCare2 = new List<CustomerCare>();
                if (listQueueCustomerCare2Id.Count > 0)
                {
                    listCustomerCare2 = customerCares
                        .Where(x => listQueueCustomerCare2Id.Contains(x.CustomerCareId)).ToList();
                }

                //merge 2 list CustomerCare
                listCustomerCare1.AddRange(listCustomerCare2);

                var listEmployeeId = new List<Guid>();
                if (listCustomerCare1.Count > 0)
                {
                    var listCustomerCareId = listCustomerCare1.Select(y => y.CustomerCareId).ToList();
                    var listAllFeedBack = context.CustomerCareFeedBack
                        .Where(x => listCustomerCareId.Contains(x.CustomerCareId.Value) && x.CustomerId == parameter.CustomerId).ToList();
                    listEmployeeId = listCustomerCare1.Select(y => y.EmployeeCharge.Value).Distinct().ToList();

                    listEmployeeId.ForEach(employeeId =>
                    {
                        var customerCareInfor = new CustomerCareInforModel();
                        var emp = listEmployee.FirstOrDefault(x => x.EmployeeId == employeeId);
                        customerCareInfor.EmployeeCharge = emp.EmployeeId;
                        customerCareInfor.EmployeeName = emp.EmployeeName;
                        customerCareInfor.EmployeePosition = listEmployeePosition
                            .FirstOrDefault(x => x.PositionId == emp.PositionId).PositionName;

                        listCustomerCareInfor.Add(customerCareInfor);
                    });

                    listCustomerCare1.ForEach(item =>
                    {
                        var customerCareForWeek = new CustomerCareForWeekModel();
                        customerCareForWeek.CustomerCareId = item.CustomerCareId;
                        customerCareForWeek.EmployeeCharge = item.EmployeeCharge.Value;
                        customerCareForWeek.Title = listCustomerCareCategory
                            .FirstOrDefault(x => x.CategoryId == item.CustomerCareContactType).CategoryName;

                        /*
                         * Gửi SMS: 1
                         * Gửi email: 2
                         * Tặng quà: 3
                         * Gọi điện: 4
                         */
                        var customerCareCategoryCode = listCustomerCareCategory
                            .FirstOrDefault(x => x.CategoryId == item.CustomerCareContactType).CategoryCode;

                        switch (customerCareCategoryCode)
                        {
                            case "SMS":
                                customerCareForWeek.Type = 1;
                                customerCareForWeek.SubTitle = "Xem chi tiết";
                                customerCareForWeek.FeedBackStatus = 0;
                                customerCareForWeek.Background = "#fbe8ba";
                                customerCareForWeek.ActiveDate = listQueueCustomerCare2
                                    .FirstOrDefault(x => x.CustomerCareId == item.CustomerCareId).SenDate.Value;
                                break;
                            case "Email":
                                customerCareForWeek.Type = 2;
                                customerCareForWeek.SubTitle = "Xem chi tiết";
                                customerCareForWeek.FeedBackStatus = 0;
                                customerCareForWeek.Background = "#e5cbf2";
                                customerCareForWeek.ActiveDate = listQueueCustomerCare2
                                    .FirstOrDefault(x => x.CustomerCareId == item.CustomerCareId).SenDate.Value;
                                customerCareForWeek.Title = context.Queue.FirstOrDefault(x => x.CustomerId == parameter.CustomerId && x.CustomerCareId == item.CustomerCareId)?.Title;
                                if (customerCareForWeek.Title.Contains("-"))
                                {
                                    customerCareForWeek.Title = customerCareForWeek.Title.Substring(0,
                                        customerCareForWeek.Title.LastIndexOf("-") - 1);
                                }
                                break;
                            case "Gift":
                                customerCareForWeek.Type = 3;
                                var checkFeedBackGift = listAllFeedBack.FirstOrDefault(x => x.CustomerCareId == item.CustomerCareId);
                                customerCareForWeek.SubTitle = checkFeedBackGift != null
                                    ? (checkFeedBackGift.FeedBackCode != null ? "Đã phản hồi" : "Chưa phản hồi")
                                    : "Chưa phản hồi";
                                customerCareForWeek.FeedBackStatus = checkFeedBackGift != null ? 1 : 2;
                                customerCareForWeek.Background = "#cfdefa";
                                customerCareForWeek.ActiveDate = item.ActiveDate.Value;
                                break;
                            case "CallPhone":
                                customerCareForWeek.Type = 4;
                                var checkFeedBackCallPhone = listAllFeedBack.FirstOrDefault(x => x.CustomerCareId == item.CustomerCareId);
                                customerCareForWeek.SubTitle = checkFeedBackCallPhone != null
                                    ? (checkFeedBackCallPhone.FeedBackCode != null ? "Đã phản hồi" : "Chưa phản hồi")
                                    : "Chưa phản hồi";
                                customerCareForWeek.FeedBackStatus = checkFeedBackCallPhone != null ? 1 : 2;
                                customerCareForWeek.Background = "#f4d4e4";
                                customerCareForWeek.ActiveDate = item.ActiveDate.Value;
                                break;
                        }

                        listAllCustomerCareForWeek.Add(customerCareForWeek);
                    });

                    #region Nhóm theo nhân viên CSKH và theo tuần

                    var current_month = (DateTime.Now).Month;
                    var current_year = (DateTime.Now).Year;
                    //Ngày đầu tiên của tháng
                    var first_date_month = new DateTime(current_year, current_month, 1, 0, 0, 0, 0);
                    //Ngày cuối cùng của tháng
                    var last_date_month = first_date_month.AddMonths(1).AddDays(-1);
                    last_date_month = last_date_month.AddHours(23).AddMinutes(59).AddSeconds(59).AddMilliseconds(999);

                    //Ngày bắt đầu tuần 1 và Ngày cuối cùng tuần 1
                    var startDateWeek1 = first_date_month;
                    var endDateWeek1 = LastDateOfWeek(startDateWeek1);

                    //Ngày bắt đầu tuần 2 và Ngày cuối cùng tuần 2
                    var startDateWeek2 = FirstDateOfWeek(endDateWeek1);
                    var endDateWeek2 = LastDateOfWeek(startDateWeek2);

                    //Ngày bắt đầu tuần 3 và Ngày cuối cùng tuần 3
                    var startDateWeek3 = FirstDateOfWeek(endDateWeek2);
                    var endDateWeek3 = LastDateOfWeek(startDateWeek3);

                    //Ngày bắt đầu tuần 4 và Ngày cuối cùng tuần 4
                    var startDateWeek4 = FirstDateOfWeek(endDateWeek3);
                    var endDateWeek4 = LastDateOfWeek(startDateWeek4);

                    //Ngày bắt đầu tuần 5 và Ngày cuối cùng tuần 5
                    DateTime? startDateWeek5 = FirstDateOfWeek(endDateWeek4);
                    DateTime? endDateWeek5 = last_date_month;

                    int check = checkLastDateOfMonth(endDateWeek4, last_date_month);

                    if (check == 1)
                    {
                        //Nếu là ngày cuối cùng của tháng
                        endDateWeek4 = last_date_month;
                        startDateWeek5 = null;
                        endDateWeek5 = null;
                    }

                    var listWeek1 = new List<CustomerCareForWeekModel>();
                    var listWeek2 = new List<CustomerCareForWeekModel>();
                    var listWeek3 = new List<CustomerCareForWeekModel>();
                    var listWeek4 = new List<CustomerCareForWeekModel>();
                    var listWeek5 = new List<CustomerCareForWeekModel>();
                    listCustomerCareInfor.ForEach(item =>
                    {
                        listWeek1 = listAllCustomerCareForWeek.Where(x =>
                            x.EmployeeCharge == item.EmployeeCharge && x.ActiveDate >= startDateWeek1 &&
                            x.ActiveDate < endDateWeek1).OrderBy(z => z.ActiveDate).ToList();
                        item.Week1 = listWeek1;

                        listWeek2 = listAllCustomerCareForWeek.Where(x =>
                            x.EmployeeCharge == item.EmployeeCharge && x.ActiveDate >= startDateWeek2 &&
                            x.ActiveDate < endDateWeek2).OrderBy(z => z.ActiveDate).ToList();
                        item.Week2 = listWeek2;

                        listWeek3 = listAllCustomerCareForWeek.Where(x =>
                            x.EmployeeCharge == item.EmployeeCharge && x.ActiveDate >= startDateWeek3 &&
                            x.ActiveDate < endDateWeek3).OrderBy(z => z.ActiveDate).ToList();
                        item.Week3 = listWeek3;

                        listWeek4 = listAllCustomerCareForWeek.Where(x =>
                            x.EmployeeCharge == item.EmployeeCharge && x.ActiveDate >= startDateWeek4 &&
                            x.ActiveDate < endDateWeek4).OrderBy(z => z.ActiveDate).ToList();
                        item.Week4 = listWeek4;

                        if (check != 1)
                        {
                            listWeek5 = listAllCustomerCareForWeek.Where(x =>
                                x.EmployeeCharge == item.EmployeeCharge && x.ActiveDate >= startDateWeek5 &&
                                x.ActiveDate < endDateWeek5).OrderBy(z => z.ActiveDate).ToList();
                            item.Week5 = listWeek5;
                        }
                    });

                    #endregion

                }

                #endregion

                #region Lấy list thông tin lịch hẹn theo tháng hiện tại

                var customerMeetingInforModel = new CustomerMeetingInforModel();
                customerMeetingInforModel.EmployeeId = employee.EmployeeId;
                customerMeetingInforModel.EmployeeName = employee.EmployeeName;
                customerMeetingInforModel.EmployeePosition = listEmployeePosition
                    .FirstOrDefault(x => x.PositionId == employee.PositionId) != null ? listEmployeePosition
                    .FirstOrDefault(x => x.PositionId == employee.PositionId).PositionName : string.Empty;

                var listAllCustomerMeetingForWeek = new List<CustomerMeetingForWeekModel>();

                var listAllCustomerMeeting = context.CustomerMeeting.Where(x =>
                        x.EmployeeId == employee.EmployeeId && x.CustomerId == parameter.CustomerId &&
                        x.StartDate.Value.Month == (DateTime.Now).Month &&
                        x.StartDate.Value.Year == (DateTime.Now).Year)
                    .ToList();

                listAllCustomerMeeting.ForEach(item =>
                {
                    var customerMeetingForWeek = new CustomerMeetingForWeekModel();
                    customerMeetingForWeek.CustomerMeetingId = item.CustomerMeetingId;
                    customerMeetingForWeek.EmployeeId = item.EmployeeId;
                    customerMeetingForWeek.Title = item.Title;
                    customerMeetingForWeek.Subtitle = item.StartDate.Value.ToString("dd/MM/yyyy") + " - " + item.StartDate.Value.ToString("HH:mm");
                    customerMeetingForWeek.Background = "#ffcc00";
                    customerMeetingForWeek.StartDate = item.StartDate;
                    customerMeetingForWeek.StartHours = item.StartHours;
                    listAllCustomerMeetingForWeek.Add(customerMeetingForWeek);
                });

                var current_month_meeting = (DateTime.Now).Month;
                var current_year_meeting = (DateTime.Now).Year;
                //Ngày đầu tiên của tháng
                var first_date_month_meeting = new DateTime(current_year_meeting, current_month_meeting, 1, 0, 0, 0, 0);
                //Ngày cuối cùng của tháng
                var last_date_month_meeting = first_date_month_meeting.AddMonths(1).AddDays(-1);
                last_date_month_meeting = last_date_month_meeting.AddHours(23).AddMinutes(59).AddSeconds(59).AddMilliseconds(999);

                //Ngày bắt đầu tuần 1 và Ngày cuối cùng tuần 1
                var startDateMeetingWeek1 = first_date_month_meeting;
                var endDateMeetingWeek1 = LastDateOfWeek(startDateMeetingWeek1);

                //Ngày bắt đầu tuần 2 và Ngày cuối cùng tuần 2
                var startDateMeetingWeek2 = FirstDateOfWeek(endDateMeetingWeek1);
                var endDateMeetingWeek2 = LastDateOfWeek(startDateMeetingWeek2);

                //Ngày bắt đầu tuần 3 và Ngày cuối cùng tuần 3
                var startDateMeetingWeek3 = FirstDateOfWeek(endDateMeetingWeek2);
                var endDateMeetingWeek3 = LastDateOfWeek(startDateMeetingWeek3);

                //Ngày bắt đầu tuần 4 và Ngày cuối cùng tuần 4
                var startDateMeetingWeek4 = FirstDateOfWeek(endDateMeetingWeek3);
                var endDateMeetingWeek4 = LastDateOfWeek(startDateMeetingWeek4);

                //Ngày bắt đầu tuần 5 và Ngày cuối cùng tuần 5
                DateTime? startDateMeetingWeek5 = FirstDateOfWeek(endDateMeetingWeek4);
                DateTime? endDateMeetingWeek5 = last_date_month_meeting;

                int checkMeeting = checkLastDateOfMonth(endDateMeetingWeek4, last_date_month_meeting);

                if (checkMeeting == 1)
                {
                    //Nếu là ngày cuối cùng của tháng
                    endDateMeetingWeek4 = last_date_month_meeting;
                    startDateMeetingWeek5 = null;
                    endDateMeetingWeek5 = null;
                }

                var week1 = new List<CustomerMeetingForWeekModel>();
                var week2 = new List<CustomerMeetingForWeekModel>();
                var week3 = new List<CustomerMeetingForWeekModel>();
                var week4 = new List<CustomerMeetingForWeekModel>();
                var week5 = new List<CustomerMeetingForWeekModel>();

                week1 = listAllCustomerMeetingForWeek
                    .Where(x => x.StartDate >= startDateMeetingWeek1 && x.StartDate < endDateMeetingWeek1)
                    .OrderBy(z => z.StartDate).ToList();

                week2 = listAllCustomerMeetingForWeek
                    .Where(x => x.StartDate >= startDateMeetingWeek2 && x.StartDate < endDateMeetingWeek2)
                    .OrderBy(z => z.StartDate).ToList();

                week3 = listAllCustomerMeetingForWeek
                    .Where(x => x.StartDate >= startDateMeetingWeek3 && x.StartDate < endDateMeetingWeek3)
                    .OrderBy(z => z.StartDate).ToList();

                week4 = listAllCustomerMeetingForWeek
                    .Where(x => x.StartDate >= startDateMeetingWeek4 && x.StartDate < endDateMeetingWeek4)
                    .OrderBy(z => z.StartDate).ToList();

                if (checkMeeting != 1)
                {
                    week5 = listAllCustomerMeetingForWeek
                        .Where(x => x.StartDate >= startDateMeetingWeek5 && x.StartDate < endDateMeetingWeek5)
                        .OrderBy(z => z.StartDate).ToList();
                }

                customerMeetingInforModel.Week1 = week1;
                customerMeetingInforModel.Week2 = week2;
                customerMeetingInforModel.Week3 = week3;
                customerMeetingInforModel.Week4 = week4;
                customerMeetingInforModel.Week5 = week5;

                #endregion

                #region Lấy thông tin báo giá
                var quoteCategoryTypeId = context.CategoryType.FirstOrDefault(f => f.CategoryTypeCode == "TGI")?.CategoryTypeId;
                var listQuoteStatus = context.Category.Where(w => w.CategoryTypeId == quoteCategoryTypeId).ToList() ?? new List<Category>();
                var listQuoteEntity = context.Quote.Where(w => w.Active == true && w.ObjectTypeId == parameter.CustomerId && w.ObjectType == "CUSTOMER").ToList() ?? new List<Quote>();
                var listQuoteDetail = context.QuoteDetail.Where(x => x.Active == true).ToList();
                var listQuoteCostDetail = context.QuoteCostDetail.Where(x => x.Active == true).ToList();
                var listPromotionObjectApply = context.PromotionObjectApply.ToList();
                var listQuote = new List<DataAccess.Models.Quote.QuoteEntityModel>();
                var appName = context.SystemParameter.FirstOrDefault(x => x.SystemKey == "ApplicationName")
                    .SystemValueString;

                listQuoteEntity?.ForEach(quote =>
                {
                    var quoteStatusName = listQuoteStatus.FirstOrDefault(f => f.CategoryId == quote.StatusId)?.CategoryName ?? "";
                    listQuote.Add(new Models.Quote.QuoteEntityModel
                    {
                        QuoteId = quote.QuoteId,
                        QuoteCode = quote.QuoteCode,
                        QuoteDate = quote.QuoteDate,
                        EffectiveQuoteDate = quote.EffectiveQuoteDate,
                        Amount = quote.Amount,
                        QuoteStatusName = quoteStatusName,
                        Note = quote.Note,
                        QuoteName = quote.QuoteName,
                        DiscountType = quote.DiscountType,
                        DiscountValue = quote.DiscountValue,
                        TotalAmountAfterVat = CalculateTotalAmountAfterVat(quote.QuoteId, quote.DiscountType, quote.DiscountValue, quote.Vat, listQuoteDetail, listQuoteCostDetail, listPromotionObjectApply, appName)
                    });
                });

                listQuote.ForEach(x =>
                {
                    x.TotalAmount = CalculateTotalAmount(x.QuoteId, x.DiscountType, x.DiscountValue,
                        x.TotalAmountAfterVat, listPromotionObjectApply);
                });
                #endregion

                #region Lấy thông tin danh sách cơ hội

                var leadCategoryTypeId =
                    context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "CHS")?.CategoryTypeId;
                var listLeadStatus = context.Category.Where(x => x.CategoryTypeId == leadCategoryTypeId).ToList() ?? new List<Category>();

                var listLeadEntity =
                    context.Lead.Where(x => x.Active == true && x.CustomerId == parameter.CustomerId).ToList() ??
                    new List<Lead>();
                var ListLead = new List<DataAccess.Models.Lead.LeadEntityModel>();

                listLeadEntity?.ForEach(lead =>
                {
                    var statusName = listLeadStatus.FirstOrDefault(x => x.CategoryId == lead.StatusId)?.CategoryName ??
                                     "";
                    var leadName =
                        context.Contact.FirstOrDefault(x => x.ObjectId == lead.LeadId && x.ObjectType == "LEA");
                    var pic = context.Employee.FirstOrDefault(x => x.EmployeeId == lead.PersonInChargeId);
                    ListLead.Add(new Models.Lead.LeadEntityModel
                    {
                        LeadId = lead.LeadId,
                        LeadCode = lead.LeadCode,
                        FullName = leadName.FirstName + " " + leadName.LastName,
                        StatusName = statusName,
                        RequirementDetail = lead.RequirementDetail,
                        ExpectedSale = lead.ExpectedSale,
                        PersonInChargeFullName = pic.EmployeeCode + " - " + pic.EmployeeName,
                    });
                });

                #endregion

                #region Lấy list ghi chú

                var listNote = new List<NoteEntityModel>();

                listNote = note
                    .Where(x => x.ObjectId == parameter.CustomerId && x.ObjectType == "CUS" && x.Active == true).Select(
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
                        var _user = listUser.FirstOrDefault(x => x.UserId == item.CreatedById);
                        var _employee = listEmployee.FirstOrDefault(x => x.EmployeeId == _user.EmployeeId);
                        item.ResponsibleName = _employee.EmployeeName;
                        item.NoteDocList = listNoteDocument.Where(x => x.NoteId == item.NoteId).ToList();
                    });

                    //Sắp xếp lại listNote
                    listNote = listNote.OrderByDescending(x => x.CreatedDate).ToList();
                }

                #endregion

                #region Khu vực
                var listArea = context.GeographicalArea.Where(c => c.Active == true)
                    .Select(m => new GeographicalAreaEntityModel
                    {
                        GeographicalAreaId = m.GeographicalAreaId,
                        GeographicalAreaCode = m.GeographicalAreaCode,
                        GeographicalAreaName = m.GeographicalAreaName
                    }).ToList();
                #endregion

                #region Nhóm khách hàng

                var cusGroupId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "NHA")?.CategoryTypeId;
                var listCusGroup = context.Category.Where(c => c.CategoryTypeId == cusGroupId)
                    .Select(m => new CategoryEntityModel
                    {
                        CategoryId = m.CategoryId,
                        CategoryCode = m.CategoryCode,
                        CategoryName = m.CategoryName
                    }).ToList();

                #endregion

                #region Trạng thái phụ

                var statusCustomerTypeId = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "THA")
                    ?.CategoryTypeId;
                var statusCustomerCode = context.Category
                    .FirstOrDefault(x => x.CategoryTypeId == statusCustomerTypeId && x.CategoryId == customer.StatusId)
                    ?.CategoryCode;

                var statusSupportType = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "TTPKHTN")
                    ?.CategoryTypeId;
                var ListStatusSupport = context.Category.Where(x => x.CategoryTypeId == statusSupportType).Select(y =>
                        new CategoryEntityModel
                        {
                            CategoryId = y.CategoryId,
                            CategoryCode = y.CategoryCode,
                            CategoryName = y.CategoryName
                        })
                    .OrderBy(z => z.CategoryCode).ToList();

                #endregion

                #region Danh sách nhân viên take care

                var listEmpTakeCare = context.Employee.Where(c => c.Active == true && c.IsTakeCare == true)
                    .Select(m => new EmployeeEntityModel
                    {
                        EmployeeId = m.EmployeeId,
                        EmployeeCode = m.EmployeeCode,
                        EmployeeName = m.EmployeeName,
                        EmployeeCodeName = m.EmployeeCode + " - " + m.EmployeeName
                    }).OrderBy(c => c.EmployeeName).ToList();

                //Nếu nhân viên đã nghỉ việc thì thêm nhân viên đó vào danh sách
                if (customer.EmployeeTakeCareId != null)
                {
                    var existsEMP = listEmpTakeCare.FirstOrDefault(x => x.EmployeeId == customer.EmployeeTakeCareId);

                    if (existsEMP == null)
                    {
                        var empTakeCare = context.Employee.Where(x => x.EmployeeId == customer.EmployeeTakeCareId)
                            .Select(y => new EmployeeEntityModel
                            {
                                EmployeeId = y.EmployeeId,
                                EmployeeCode = y.EmployeeCode,
                                EmployeeName = y.EmployeeName,
                                EmployeeCodeName = y.EmployeeCode + " - " + y.EmployeeName
                            }).FirstOrDefault();

                        if (empTakeCare != null)
                        {
                            listEmpTakeCare.Add(empTakeCare);
                        }
                    }
                }

                #endregion

                #region Thêm nv phụ trách đã được lưu nhưng không có trong list nv phụ trách hiện tại

                //ListEmployeeEntityModel
                if (customer.PersonInChargeId != null)
                {
                    var empExists =
                        ListEmployeeEntityModel.FirstOrDefault(x => x.EmployeeId == customer.PersonInChargeId);

                    if (empExists == null)
                    {
                        var empPER = context.Employee.Where(x => x.EmployeeId == customer.PersonInChargeId).Select(y =>
                            new EmployeeEntityModel
                            {
                                EmployeeId = y.EmployeeId,
                                EmployeeName = y.EmployeeName,
                                EmployeeCode = y.EmployeeCode,
                                OrganizationId = y.OrganizationId
                            }).FirstOrDefault();

                        if (empPER != null)
                        {
                            ListEmployeeEntityModel.Add(empPER);
                        }
                    }
                }

                #endregion

                #region Lấy reference của khách hàng

                var countCustomerReference = CheckCustomerReference(ListLead, listQuote);

                #endregion

                return new GetDataDetailPotentialCustomerResult()
                {
                    ListInvestFund = ListInvestFund ?? new List<CategoryEntityModel>(),
                    ListPersonalInChange = ListEmployeeEntityModel ?? new List<EmployeeEntityModel>(),
                    PotentialCustomerModel = customer,
                    PotentialCustomerContactModel = contact,
                    ListContact = listContact,
                    ListFileByPotentialCustomer = listFileByPotentialCustomer,
                    ListLinkOfDocument = ListLinkOfDocument ?? new List<Models.Document.LinkOfDocumentEntityModel>(),
                    ListProduct = listProductWithFixedPrice ?? new List<Models.Product.ProductEntityModel>(),
                    ListPotentialCustomerProduct = listPotentialCustomerProduct ?? new List<PotentialCustomerProductEntityModel>(),
                    ListQuoteByPotentialCustomer = listQuote ?? new List<Models.Quote.QuoteEntityModel>(),
                    ListLeadByPotentialCustomer = ListLead ?? new List<Models.Lead.LeadEntityModel>(),
                    ListCustomerCareInfor = listCustomerCareInfor ?? new List<CustomerCareInforModel>(),
                    CustomerMeetingInfor = customerMeetingInforModel,
                    ListParticipants = listParticipants ?? new List<EmployeeEntityModel>(),
                    ListNote = listNote,
                    ListArea = listArea,
                    ListCusGroup = listCusGroup,
                    ListStatusSupport = ListStatusSupport,
                    StatusSupportId = customer.StatusSuportId,
                    StatusCustomerCode = statusCustomerCode,
                    ListEmpTakeCare = listEmpTakeCare,
                    CountCustomerReference = countCustomerReference,
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = "OK"
                };
            }
            catch (Exception e)
            {
                return new GetDataDetailPotentialCustomerResult()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
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
            decimal? amountNotInCluse = 0;
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
                        var sumLabor = x.UnitLaborPrice * x.UnitLaborNumber * x.ExchangeRate;
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
                            amountNotInCluse += price;
                        }
                        amountPriceCost += price;
                    });


                    result = amount + totalAmountVat + amountPriceCost;
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

        public UpdatePotentialCustomerResult UpdatePotentialCustomer(UpdatePotentialCustomerParameter parameter)
        {
            var customer = new Customer();
            var listNote = new List<NoteEntityModel>();
            var listFileByPotentialCustomer = new List<DataAccess.Models.Folder.FileInFolderEntityModel>();
            var ListLinkOfDocument = new List<DataAccess.Models.Document.LinkOfDocumentEntityModel>();
            var listEmpTakeCare = new List<EmployeeEntityModel>();
            var ListPersonalInChange = new List<EmployeeEntityModel>();

            try
            {
                var listUser = context.User.ToList();
                var user = context.User.FirstOrDefault(x => x.UserId == parameter.UserId);
                customer = context.Customer.FirstOrDefault(f => f.CustomerId == parameter.Customer.CustomerId);
                var contact = context.Contact.First(f => f.ContactId == parameter.Contact.ContactId);
                var employee = context.Employee.ToList() ?? new List<Employee>();
                var listUsersEntity = context.User.ToList() ?? new List<User>();

                var investTypeId = context.CategoryType.FirstOrDefault(f => f.CategoryTypeCode == "IVF").CategoryTypeId;
                var listInvestCategory = context.Category.Where(w => w.Active == true && w.CategoryTypeId == investTypeId).ToList() ?? new List<Category>();

                var oldInvestmentFund = listInvestCategory.FirstOrDefault(f => f.CategoryId == customer.InvestmentFundId);
                var oldPersonInCharge = employee.FirstOrDefault(f => f.EmployeeId == customer.PersonInChargeId);
                var employeeLogin = employee.FirstOrDefault(f => f.EmployeeId == user.EmployeeId);
                var oldCustomer = new ComparePotentialCustomerModel()
                {
                    Email = contact.Email,
                    WorkEmail = contact.WorkEmail,
                    Phone = contact.Phone,
                    WorkPhone = contact.WorkPhone,
                    OtherPhone = contact.OtherPhone,
                    InvestmentFund = oldInvestmentFund,
                    PersonInCharge = oldPersonInCharge,
                };

                customer.CustomerName = parameter.Customer.CustomerName?.Trim() ?? "";
                customer.PersonInChargeId = parameter.Customer.PersonInChargeId;
                customer.UpdatedById = parameter.UserId;
                customer.UpdatedDate = DateTime.Now;
                customer.AllowSendEmail = parameter.Customer.AllowSendEmail;
                customer.AllowCall = parameter.Customer.AllowCall;
                customer.InvestmentFundId = parameter.Customer.InvestmentFundId;
                customer.CareStateId = parameter.Customer.CareStateId;
                customer.CustomerGroupId = parameter.Customer.CustomerGroupId;
                customer.EmployeeTakeCareId = parameter.Customer.EmployeeTakeCareId;
                customer.ContactDate = parameter.Customer.ContactDate;
                customer.EvaluateCompany = parameter.Customer.EvaluateCompany;
                customer.SalesUpdate = parameter.Customer.SalesUpdate;
                customer.SalesUpdateAfterMeeting = parameter.Customer.SalesUpdateAfterMeeting;
                customer.PotentialId = parameter.Customer.PotentialId;
                customer.KhachDuAn = parameter.Customer.KhachDuAn.Value;

                contact.FirstName = parameter.Contact.FirstName?.Trim() ?? "";
                contact.LastName = parameter.Contact.LastName?.Trim() ?? "";
                contact.Gender = parameter.Contact.Gender?.Trim() ?? "";
                contact.Phone = parameter.Contact.Phone?.Trim() ?? "";
                contact.WorkPhone = parameter.Contact.WorkPhone?.Trim() ?? "";
                contact.OtherPhone = parameter.Contact.OtherPhone?.Trim() ?? "";
                contact.Email = parameter.Contact.Email?.Trim() ?? "";
                contact.OtherEmail = parameter.Contact.OtherEmail?.Trim() ?? "";
                contact.WorkEmail = parameter.Contact.WorkEmail?.Trim() ?? "";
                contact.Role = parameter.Contact.Role?.Trim() ?? "";
                contact.Address = parameter.Contact.Address?.Trim() ?? "";
                contact.GeographicalAreaId = parameter.Contact.GeographicalAreaId;
                contact.SocialUrl = parameter.Contact.SocialUrl?.Trim() ?? "";
                contact.TaxCode = parameter.Contact.TaxCode?.Trim() ?? "";
                contact.CustomerPosition = parameter.Contact.CustomerPosition;
                contact.PotentialCustomerPosition = parameter.Contact.PotentialCustomerPosition?.Trim() ?? "";
                contact.Note = parameter.Contact.Note ?? "";

                if (parameter.Contact.Latitude != null)
                    contact.Latitude = Math.Round(parameter.Contact.Latitude.Value, 6);
                if (parameter.Contact.Longitude != null)
                    contact.Longitude = Math.Round(parameter.Contact.Longitude.Value, 6);

                contact.UpdatedById = parameter.UserId;
                contact.UpdatedDate = DateTime.Now;

                #region Check trùng email/sđt

                if (!string.IsNullOrEmpty(contact.Email))
                {
                    var checkEmail = context.Contact.FirstOrDefault(x =>
                        x.ObjectType == "CUS" && x.ObjectId != customer.CustomerId &&
                        (x.Email ?? "").Trim().ToLower() == contact.Email.Trim().ToLower());

                    if (checkEmail != null)
                    {
                        return new UpdatePotentialCustomerResult()
                        {
                            StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                            MessageCode = "Email khách hàng đã tồn tại trên hệ thống"
                        };
                    }
                }

                if (!string.IsNullOrEmpty(contact.Phone))
                {
                    var checkPhone = context.Contact.FirstOrDefault(x =>
                        x.ObjectType == "CUS" && x.ObjectId != customer.CustomerId &&
                        (x.Phone ?? "").Trim().ToLower() == contact.Phone.Trim().ToLower());

                    if (checkPhone != null)
                    {
                        return new UpdatePotentialCustomerResult()
                        {
                            StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                            MessageCode = "Số điện thoại khách hàng đã tồn tại trên hệ thống"
                        };
                    }
                }

                #endregion

                context.Customer.Update(customer);
                context.Contact.Update(contact);

                #region Remove list document

                parameter.ListDocumentIdNeedRemove = parameter.ListDocumentIdNeedRemove.Where(w => w != Guid.Empty && w != null).ToList();

                var listOldDocument = context.FileInFolder.Where(w => parameter.ListDocumentIdNeedRemove.Contains(w.FileInFolderId)).ToList();
                context.FileInFolder.RemoveRange(listOldDocument);

                #endregion

                #region Remove old Link of document and add new 

                var listOldLinkOfDocument = context.LinkOfDocument.Where(w => w.ObjectId == parameter.Customer.CustomerId && w.ObjectType == "QLKHTN").ToList();
                context.LinkOfDocument.RemoveRange(listOldLinkOfDocument);

                var listNewLinkOfDocument = new List<Entities.LinkOfDocument>();
                parameter.ListLinkOfDocument?.ForEach(linkOfDoc =>
                {
                    var newItem = new Entities.LinkOfDocument();
                    newItem.LinkOfDocumentId = Guid.NewGuid();
                    newItem.LinkName = linkOfDoc.LinkName;
                    newItem.LinkValue = linkOfDoc.LinkValue;
                    newItem.ObjectType = "QLKHTN";
                    newItem.ObjectId = parameter.Customer.CustomerId;
                    newItem.Active = true;
                    newItem.CreatedById = parameter.UserId;
                    newItem.CreatedDate = DateTime.Now;
                    newItem.UpdatedById = null;
                    newItem.UpdatedDate = null;

                    listNewLinkOfDocument.Add(newItem);
                });
                context.LinkOfDocument.AddRange(listNewLinkOfDocument);

                #endregion

                #region Update danh sách sản phẩm hàng hóa

                //xóa bản ghi cũ
                var listOldProduct = context.PotentialCustomerProduct.Where(w => w.CustomerId == parameter.Customer.CustomerId).ToList();
                context.PotentialCustomerProduct.RemoveRange(listOldProduct);

                var listNewProduct = new List<Entities.PotentialCustomerProduct>();
                parameter.ListCustomerProduct?.ForEach(item =>
                {
                    listNewProduct.Add(new PotentialCustomerProduct
                    {
                        PotentialCustomerProductId = Guid.NewGuid(),
                        ProductId = item.ProductId,
                        CustomerId = parameter.Customer.CustomerId,
                        IsInTheSystem = item.IsInTheSystem,
                        ProductName = item.ProductName,
                        ProductUnit = item.ProductUnit,
                        ProductFixedPrice = item.ProductFixedPrice,
                        ProductUnitPrice = item.ProductUnitPrice,
                        ProductNote = item.ProductNote,
                        Active = true,
                        CreatedById = parameter.UserId,
                        CreatedDate = DateTime.Now,
                    });
                });
                context.PotentialCustomerProduct.AddRange(listNewProduct);

                #endregion

                #region Update danh sách người liên hệ

                var listOldContact = context.Contact.Where(w => w.ObjectId == parameter.Customer.CustomerId && w.ObjectType == "CUS_CON").ToList();

                var listOldContactId = listOldContact.Select(x => x.ContactId).ToList();

                var listNewContact = new List<Contact>();
                parameter.ListContact?.ForEach(_contact =>
                {
                    var newContact = new Contact();
                    newContact.ContactId = _contact.ContactId;
                    newContact.ObjectId = parameter.Customer.CustomerId;
                    newContact.ObjectType = "CUS_CON";
                    newContact.FirstName = _contact.FirstName;
                    newContact.LastName = _contact.LastName;
                    newContact.Gender = _contact.Gender;
                    newContact.Phone = _contact.Phone;
                    //newContact.WorkPhone = _contact.WorkPhone;
                    newContact.Email = _contact.Email;
                    //newContact.WorkEmail = _contact.WorkEmail;
                    newContact.Address = _contact.Address;
                    newContact.Role = _contact.Role;
                    newContact.LinkFace = _contact.LinkFace;
                    newContact.EvaluateContactPeople = _contact.EvaluateContactPeople;
                    newContact.DateOfBirth = _contact.DateOfBirth;

                    newContact.Active = true;
                    newContact.UpdatedById = parameter.UserId;
                    newContact.UpdatedDate = DateTime.Now;

                    if (listOldContactId.Contains(newContact.ContactId))
                    {
                        newContact.CreatedById = _contact.CreatedById;
                        newContact.CreatedDate = _contact.CreatedDate;
                    }
                    else
                    {
                        newContact.ContactId = Guid.NewGuid();
                        newContact.CreatedById = parameter.UserId;
                        newContact.CreatedDate = DateTime.Now;
                    }

                    listNewContact.Add(newContact);
                });

                context.Contact.RemoveRange(listOldContact);
                context.Contact.AddRange(listNewContact);

                #endregion

                #region Thêm ghi chú nếu những có thay đổi

                var newInvestmentFund = listInvestCategory.FirstOrDefault(f => f.CategoryId == customer.InvestmentFundId);
                var newPersonInCharge = employee.FirstOrDefault(f => f.EmployeeId == customer.PersonInChargeId);

                var newCustomer = new ComparePotentialCustomerModel()
                {
                    Email = contact.Email,
                    WorkEmail = contact.WorkEmail,
                    Phone = contact.Phone,
                    WorkPhone = contact.WorkPhone,
                    OtherPhone = contact.OtherPhone,
                    InvestmentFund = newInvestmentFund,
                    PersonInCharge = newPersonInCharge
                };

                //tìm ra danh sách link bị xóa
                var listOldDocumentId = parameter.ListLinkOfDocument.Where(w => w.LinkOfDocumentId != Guid.Empty).Select(w => w.LinkOfDocumentId).ToList(); //danh sach document cu~
                //listOldLinkOfDocument
                var listDeletedDocument = listOldLinkOfDocument.Where(w => !listOldDocumentId.Contains(w.LinkOfDocumentId)).ToList() ?? new List<LinkOfDocument>();

                var isDifference = ComparePotentialCustomer(oldCustomer, newCustomer, parameter.ListLinkOfDocument, listDeletedDocument);

                if (isDifference == false)
                {
                    var note = new Note();
                    note.NoteId = Guid.NewGuid();
                    note.Description = GetNoteDescription(oldCustomer, newCustomer, parameter.ListLinkOfDocument, listDeletedDocument, employeeLogin);
                    note.Type = "SYS";
                    note.ObjectId = parameter.Customer.CustomerId;
                    note.ObjectType = "CUS";
                    note.Active = true;
                    note.CreatedById = parameter.UserId;
                    note.CreatedDate = DateTime.Now;
                    note.NoteTitle = "đã chỉnh sửa";

                    context.Note.Add(note);
                }

                #endregion

                //lưu ghi chú dữ liệu thay đổi và document đi kèm
                context.SaveChanges();

                #region Log

                LogHelper.AuditTrace(context, "Update", "POTENTIAL_CUSTOMER", parameter.Customer.CustomerId, parameter.UserId);

                #endregion

                #region Lấy list ghi chú sau khi update


                listNote = context.Note
                    .Where(x => x.ObjectId == parameter.Customer.CustomerId && x.ObjectType == "CUS" && x.Active == true).Select(
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
                        var _user = listUsersEntity.FirstOrDefault(x => x.UserId == item.CreatedById);
                        var _employee = employee.FirstOrDefault(x => x.EmployeeId == _user.EmployeeId);
                        item.ResponsibleName = _employee.EmployeeName;
                        item.NoteDocList = listNoteDocument.Where(x => x.NoteId == item.NoteId).ToList();
                    });

                    //Sắp xếp lại listNote
                    listNote = listNote.OrderByDescending(x => x.CreatedDate).ToList();
                }

                #endregion

                #region Lấy thông tin File đính kèm

                var listFileDocument =
                    context.FileInFolder.Where(w =>
                            w.Active == true && w.ObjectId == parameter.Customer.CustomerId && w.ObjectType == "QLKHTN")
                        .ToList() ?? new List<FileInFolder>();


                listFileDocument.ForEach(file =>
                {
                    //convert name of file
                    List<string> fullnameArr = file.FileName?.Split('_')?.ToList() ?? new List<string>();
                    if (fullnameArr.Any()) //prevent IndexOutOfRangeException for empty list
                    {
                        fullnameArr.RemoveAt(fullnameArr.Count - 1);
                    }

                    string fullName = string.Join("", fullnameArr);
                    var fileExtension = file.FileExtension;
                    var createByUserName = listUser.FirstOrDefault(f => f.UserId == file.CreatedById)?.UserName ?? "";

                    var newItem = new DataAccess.Models.Folder.FileInFolderEntityModel();
                    newItem.FileInFolderId = file.FileInFolderId;
                    newItem.FolderId = file.FolderId;
                    newItem.FileName = fullName + "." + fileExtension;
                    newItem.ObjectId = file.ObjectId;
                    newItem.ObjectType = file.ObjectType;
                    newItem.Size = file.Size;
                    newItem.Active = file.Active;
                    newItem.FileExtension = file.FileExtension;
                    newItem.CreatedById = file.CreatedById;
                    newItem.CreatedDate = file.CreatedDate;
                    newItem.UpdatedById = file.UpdatedById;
                    newItem.UpdatedDate = file.UpdatedDate;
                    newItem.CreatedByName = createByUserName;

                    listFileByPotentialCustomer.Add(newItem);
                });

                #endregion

                #region Get list link dinh kem


                var listLinkOfDocEntity = context.LinkOfDocument.Where(w =>
                        w.ObjectId == parameter.Customer.CustomerId && w.Active == true && w.ObjectType == "QLKHTN")
                    .ToList();
                listLinkOfDocEntity?.ForEach(item =>
                {
                    ListLinkOfDocument.Add(new Models.Document.LinkOfDocumentEntityModel
                    {
                        LinkOfDocumentId = item.LinkOfDocumentId,
                        LinkName = item.LinkName,
                        LinkValue = item.LinkValue,
                        CreatedByName = listUser.FirstOrDefault(f => f.UserId == item.CreatedById)?.UserName ?? "",
                        CreatedById = item.CreatedById,
                        CreatedDate = item.CreatedDate
                    });
                });

                #endregion

                #region Lấy lại list nhân viên take care

                listEmpTakeCare = context.Employee.Where(c => c.Active == true && c.IsTakeCare == true)
                    .Select(m => new EmployeeEntityModel
                    {
                        EmployeeId = m.EmployeeId,
                        EmployeeCode = m.EmployeeCode,
                        EmployeeName = m.EmployeeName,
                        EmployeeCodeName = m.EmployeeCode + " - " + m.EmployeeName
                    }).OrderBy(c => c.EmployeeName).ToList();

                //Nếu nhân viên đã nghỉ việc thì thêm nhân viên đó vào danh sách
                if (customer.EmployeeTakeCareId != null)
                {
                    var existsEMP = listEmpTakeCare.FirstOrDefault(x => x.EmployeeId == customer.EmployeeTakeCareId);

                    if (existsEMP == null)
                    {
                        var empTakeCare = context.Employee.Where(x => x.EmployeeId == customer.EmployeeTakeCareId)
                            .Select(y => new EmployeeEntityModel
                            {
                                EmployeeId = y.EmployeeId,
                                EmployeeCode = y.EmployeeCode,
                                EmployeeName = y.EmployeeName,
                                EmployeeCodeName = y.EmployeeCode + " - " + y.EmployeeName
                            }).FirstOrDefault();

                        if (empTakeCare != null)
                        {
                            listEmpTakeCare.Add(empTakeCare);
                        }
                    }
                }

                #endregion

                #region Thêm nv phụ trách đã được lưu nhưng không có trong list nv phụ trách hiện tại


                var current_employee = context.Employee.FirstOrDefault(x => x.EmployeeId == user.EmployeeId);
                var listEmployeeEntity = context.Employee.Where(w => w.Active == true).ToList();


                //Kiểm tra xem user đang đăng nhập có phải nhân viên take care của khách hàng ko?
                bool isEmployeeTakeCare = false;
                if (customer.EmployeeTakeCareId == current_employee.EmployeeId)
                {
                    isEmployeeTakeCare = true;
                }

                //check Is Manager
                var isManage = current_employee.IsManager;
                if (isManage == true)
                {
                    //Quản lí: lấy tất cả nhân viên phòng ban đó và phòng ban dưới cấp
                    var currentOrganization = current_employee.OrganizationId;
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
                            OrganizationId = w.OrganizationId
                        }).ToList();

                    //Nếu chưa có trong list nhân viên phụ trách khách hàng thì thêm vào
                    if (isEmployeeTakeCare)
                    {
                        var existsEmp =
                            listEmployeeFiltered.FirstOrDefault(x => x.EmployeeId == customer.PersonInChargeId);

                        if (existsEmp == null)
                        {
                            var empExtend =
                                listEmployeeEntity.Where(x => x.EmployeeId == customer.PersonInChargeId).Select(w => new
                                {
                                    EmployeeId = w.EmployeeId,
                                    EmployeeName = w.EmployeeName,
                                    EmployeeCode = w.EmployeeCode,
                                    OrganizationId = w.OrganizationId
                                }).FirstOrDefault();

                            if (empExtend != null)
                            {
                                listEmployeeFiltered.Add(empExtend);
                            }
                        }
                    }

                    listEmployeeFiltered?.ForEach(emp =>
                    {
                        ListPersonalInChange.Add(new EmployeeEntityModel
                        {
                            EmployeeId = emp.EmployeeId,
                            EmployeeName = emp.EmployeeName,
                            EmployeeCode = emp.EmployeeCode,
                            OrganizationId = emp.OrganizationId
                        });
                    });
                }
                else
                {
                    //Nhân viên: chỉ lấy nhân viên đó
                    var employeeId = listEmployeeEntity.Where(e => e.EmployeeId == current_employee.EmployeeId)
                        .FirstOrDefault();

                    if (isEmployeeTakeCare)
                    {
                        if (employeeId.EmployeeId != customer.PersonInChargeId)
                        {
                            var empExtend =
                                listEmployeeEntity.Where(x => x.EmployeeId == customer.PersonInChargeId).Select(w => new
                                {
                                    EmployeeId = w.EmployeeId,
                                    EmployeeName = w.EmployeeName,
                                    EmployeeCode = w.EmployeeCode,
                                    OrganizationId = w.OrganizationId
                                }).FirstOrDefault();

                            if (empExtend != null)
                            {
                                ListPersonalInChange.Add(new EmployeeEntityModel
                                {
                                    EmployeeId = empExtend.EmployeeId,
                                    EmployeeName = empExtend.EmployeeName,
                                    EmployeeCode = empExtend.EmployeeCode,
                                    OrganizationId = empExtend.OrganizationId
                                });
                            }
                        }
                    }

                    ListPersonalInChange.Add(new EmployeeEntityModel
                    {
                        EmployeeId = employeeId.EmployeeId,
                        EmployeeName = employeeId.EmployeeName,
                        EmployeeCode = employeeId.EmployeeCode,
                        OrganizationId = employeeId.OrganizationId
                    });
                }

                if (customer.PersonInChargeId != null)
                {
                    var empExists =
                        ListPersonalInChange.FirstOrDefault(x => x.EmployeeId == customer.PersonInChargeId);

                    if (empExists == null)
                    {
                        var empPER = context.Employee.Where(x => x.EmployeeId == customer.PersonInChargeId).Select(y =>
                            new EmployeeEntityModel
                            {
                                EmployeeId = y.EmployeeId,
                                EmployeeName = y.EmployeeName,
                                EmployeeCode = y.EmployeeCode,
                                OrganizationId = y.OrganizationId
                            }).FirstOrDefault();

                        if (empPER != null)
                        {
                            ListPersonalInChange.Add(empPER);
                        }
                    }
                }

                #endregion

            }
            catch (Exception e)
            {
                return new UpdatePotentialCustomerResult()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }

            #region Gửi mail thông báo

            NotificationHelper.AccessNotification(context, TypeModel.PotentialCustomerDetail, "UPD", new Customer(),
                customer, true);

            #endregion

            return new UpdatePotentialCustomerResult()
            {
                ListNote = listNote ?? new List<NoteEntityModel>(),
                ListFileByPotentialCustomer = listFileByPotentialCustomer,
                ListLinkOfDocument = ListLinkOfDocument ?? new List<Models.Document.LinkOfDocumentEntityModel>(),
                ListEmpTakeCare = listEmpTakeCare,
                ListPersonalInChange = ListPersonalInChange,
                StatusCode = System.Net.HttpStatusCode.OK,
                MessageCode = "OK",
            };
        }

        public GetDataSearchPotentialCustomerResult GetDataSearchPotentialCustomer(GetDataSearchPotentialCustomerParameter parameter)
        {
            try
            {
                //result
                var ListInvestFund = new List<CategoryEntityModel>();
                var ListEmployeeEntityModel = new List<DataAccess.Models.Employee.EmployeeEntityModel>();

                var categoryTypeList = context.CategoryType.Where(cty => cty.Active == true).ToList();
                var categoryList = context.Category.Where(ct => ct.Active == true).ToList();
                var employeeList = context.Employee.Where(emp => emp.Active == true).ToList();

                var cusTypeId = context.CategoryType.FirstOrDefault(w => w.CategoryTypeCode == "LHL").CategoryTypeId;
                var listCusType = context.Category.Where(w => w.CategoryTypeId == cusTypeId).Select(w => new Models.CategoryEntityModel
                {
                    CategoryId = w.CategoryId,
                    CategoryName = w.CategoryName,
                    CategoryCode = w.CategoryCode,
                    IsDefault = w.IsDefauld
                }).ToList();

                // Nhóm khách hàng
                var cusGroupTypeId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "NHA")?.CategoryTypeId;
                var listCusGroup = context.Category.Where(c => c.CategoryTypeId == cusGroupTypeId)
                    .Select(w => new Models.CategoryEntityModel
                    {
                        CategoryId = w.CategoryId,
                        CategoryName = w.CategoryName,
                        CategoryCode = w.CategoryCode,
                        IsDefault = w.IsDefauld
                    }).ToList();

                // Khu vực
                var listArea = context.GeographicalArea.Where(c => c.Active == true)
                    .Select(m => new GeographicalAreaEntityModel
                    {
                        GeographicalAreaId = m.GeographicalAreaId,
                        GeographicalAreaCode = m.GeographicalAreaCode,
                        GeographicalAreaName = m.GeographicalAreaName
                    }).ToList();

                #region Define Category Code
                var listCategoryCode = new List<string>();
                var customerGroupCode = "IVF"; //nguon tiem nang
                listCategoryCode.Add(customerGroupCode);
                var portalUserCode = "PortalUser"; //loại portalUser
                #endregion

                #region Get data from Category table
                var listCategoryTypeEntity = categoryTypeList.Where(w => listCategoryCode.Contains(w.CategoryTypeCode) && w.Active == true).ToList();
                var listCateTypeId = new List<Guid>();
                listCategoryTypeEntity?.ForEach(type =>
                {
                    listCateTypeId.Add(type.CategoryTypeId);
                });
                var listCategoryEntity = categoryList.Where(w => listCateTypeId.Contains(w.CategoryTypeId) && w.Active == true).ToList(); //list master data của category

                //get customer group
                var customerGroupTypeId = listCategoryTypeEntity.Where(w => w.CategoryTypeCode == customerGroupCode).FirstOrDefault()?.CategoryTypeId;
                var listCustomerGroupEntity = listCategoryEntity.Where(w => w.CategoryTypeId == customerGroupTypeId && w.CategoryCode != "POR").ToList(); //loại Khách hàng Portal                                              
                listCustomerGroupEntity?.ForEach(group =>
                {
                    ListInvestFund.Add(new CategoryEntityModel
                    {
                        CategoryId = group.CategoryId,
                        CategoryName = group.CategoryName,
                        CategoryCode = group.CategoryCode,
                        CategoryTypeId = group.CategoryTypeId,
                        IsDefault = group.IsDefauld
                    });
                });

                var careStateTypeId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "TTCS")?.CategoryTypeId;
                var listCareState = context.Category.Where(c => c.CategoryTypeId == careStateTypeId)
                    .Select(m => new CategoryEntityModel
                    {
                        CategoryId = m.CategoryId,
                        CategoryName = m.CategoryName,
                        CategoryCode = m.CategoryCode,
                        CategoryTypeId = m.CategoryTypeId,
                        IsDefault = m.IsDefauld
                    }).ToList();
                #endregion

                #region Get Employee Care Staff List
                var listEmployeeEntity = employeeList.Where(w => w.Active == true && w.EmployeeCode != portalUserCode).ToList(); //loai portal user

                var employeeById = listEmployeeEntity.Where(w => w.EmployeeId == parameter.EmployeeId).FirstOrDefault();
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
                    var listEmployeeFiltered = listEmployeeEntity.Where(w => listOrganizationChildrenId.Contains(w.OrganizationId)).Select(w => new
                    {
                        EmployeeId = w.EmployeeId,
                        EmployeeName = w.EmployeeName,
                        EmployeeCode = w.EmployeeCode,
                        OrganizationId = w.OrganizationId
                    }).ToList();

                    listEmployeeFiltered?.ForEach(emp =>
                    {
                        ListEmployeeEntityModel.Add(new Models.Employee.EmployeeEntityModel
                        {
                            EmployeeId = emp.EmployeeId,
                            EmployeeName = emp.EmployeeName,
                            EmployeeCode = emp.EmployeeCode,
                            OrganizationId = emp.OrganizationId,
                        });
                    });
                }
                else
                {
                    //Nhân viên: chỉ lấy nhân viên đó
                    var employeeId = listEmployeeEntity.Where(e => e.EmployeeId == parameter.EmployeeId).FirstOrDefault();
                    ListEmployeeEntityModel.Add(new Models.Employee.EmployeeEntityModel
                    {
                        EmployeeId = employeeId.EmployeeId,
                        EmployeeName = employeeId.EmployeeName,
                        EmployeeCode = employeeId.EmployeeCode,
                        OrganizationId = employeeId.OrganizationId,
                    });
                }
                #endregion

                #region Danh sách nhân viên take care

                var listEmpTakeCare = context.Employee.Where(c => c.Active == true && c.IsTakeCare == true)
                    .Select(m => new EmployeeEntityModel
                    {
                        EmployeeId = m.EmployeeId,
                        EmployeeCode = m.EmployeeCode,
                        EmployeeName = m.EmployeeName,
                        EmployeeCodeName = m.EmployeeCode + " - " + m.EmployeeName
                    }).OrderBy(c => c.EmployeeName).ToList();

                #endregion


                return new GetDataSearchPotentialCustomerResult()
                {
                    ListInvestFund = ListInvestFund ?? new List<CategoryEntityModel>(),
                    ListEmployeeModel = ListEmployeeEntityModel ?? new List<EmployeeEntityModel>(),
                    ListCareState = listCareState,
                    ListArea = listArea,
                    ListCusGroup = listCusGroup,
                    ListCusType = listCusType,
                    ListEmpTakeCare = listEmpTakeCare,
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = "OK"
                };
            }
            catch (Exception e)
            {
                return new GetDataSearchPotentialCustomerResult()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public SearchPotentialCustomerResult SearchPotentialCustomer(SearchPotentialCustomerParameter parameter)
        {
            try
            {
                var appName = context.SystemParameter.FirstOrDefault(x => x.SystemKey == "ApplicationName")
                    .SystemValueString;

                #region master data

                var listAllUser = context.User.ToList();
                var listAllEmployee = context.Employee.ToList();
                var statusCusTypeId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "THA").CategoryTypeId;
                var listAllStatus = context.Category.Where(c => c.CategoryTypeId == statusCusTypeId).ToList();
                var MOIStatusId = listAllStatus.FirstOrDefault(f => f.CategoryCode == "MOI").CategoryId;
                var HDOStatusId = listAllStatus.FirstOrDefault(f => f.CategoryCode == "HDO").CategoryId;
                var listAllCustomerEntity = context.Customer.Where(w => w.Active == true).ToList() ?? new List<Customer>();
                var listCustomerId = listAllCustomerEntity.Select(w => w.CustomerId).ToList() ?? new List<Guid>();
                var listContact = context.Contact.Where(w => w.Active == true && listCustomerId.Contains(w.ObjectId) && w.ObjectType == "CUS").ToList() ?? new List<Contact>();
                var listCustomerContact = context.Contact.Where(w => w.Active == true && listCustomerId.Contains(w.ObjectId) && w.ObjectType == "CUS_CON").OrderByDescending(x => x.CreatedDate).ToList();
                var versionName = context.SystemParameter.FirstOrDefault(x => x.SystemKey == "ApplicationName").SystemValueString;
                var listAllLead = context.Lead.Where(x => x.Active == true).ToList();
                var listAllQuote = context.Quote.Where(x => x.Active == true).ToList();


                var careStateTypeId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "TTCS")?.CategoryTypeId;
                var listAllCareState = context.Category.Where(c => c.CategoryTypeId == careStateTypeId).ToList();

                var supportStatusTypeId =
                    context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "TTPKHTN")?.CategoryTypeId;
                var listAllStatusSupport =
                    context.Category.Where(x => x.CategoryTypeId == supportStatusTypeId).ToList();

                var _khachDuAn = parameter.KhachDuAn;

                var _khachBanLe = parameter.KhachBanLe;

                //danh sách khách hàng tiềm năng và khách hàng định danh đã chuyển đổi
                var listAllCustomer =
                    listAllCustomerEntity.Where(w =>
                        (w.StatusId == MOIStatusId) || (w.StatusId == HDOStatusId && w.IsConverted == true)).ToList() ??
                    new List<Customer>();

                // chỉ lấy kh bán lẻ
                if (!_khachDuAn && _khachBanLe)
                {
                    listAllCustomer = listAllCustomer.Where(x => x.KhachDuAn == false).ToList() ?? new List<Customer>();
                }

                // chỉ lấy kh dự án
                if (_khachDuAn && !_khachBanLe)
                {
                    listAllCustomer = listAllCustomer.Where(x => x.KhachDuAn == true).ToList() ?? new List<Customer>();
                }

                #endregion

                #region Check permision: manager

                var user = listAllUser.FirstOrDefault(x => x.UserId == parameter.UserId && x.Active == true);
                if (user == null)
                {
                    return new SearchPotentialCustomerResult
                    {
                        StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                        MessageCode = "User không có quyền truy xuất dữ liệu trong hệ thống"
                    };
                }
                if (user.EmployeeId == null || user.EmployeeId == Guid.Empty)
                {
                    return new SearchPotentialCustomerResult
                    {
                        StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                        MessageCode = "Lỗi dữ liệu"
                    };
                }
                var employeeId = user.EmployeeId;
                var employee = listAllEmployee.FirstOrDefault(x => x.EmployeeId == employeeId);
                var isManager = employee.IsManager;

                #endregion

                #region Get list customer reponse

                var listCustomerIdForSale = context.Customer.Where(x => x.EmployeeTakeCareId == employeeId)
                    .Select(y => y.CustomerId).ToList();

                List<CustomerEntityModel> listCustomer = new List<CustomerEntityModel>();
                if (isManager)
                {
                    //Lấy list phòng ban con của user
                    List<Guid?> listGetAllChild = new List<Guid?>();    //List phòng ban: chính nó và các phòng ban cấp dưới của nó
                    if (employee.OrganizationId != null && isManager)
                    {
                        listGetAllChild.Add(employee.OrganizationId.Value);
                        listGetAllChild = getOrganizationChildrenId(employee.OrganizationId.Value, listGetAllChild);
                    }
                    //Lấy danh sách nhân viên EmployyeeId mà user phụ trách
                    var listEmployeeInChargeByManager = listAllEmployee.Where(x =>
                        (listGetAllChild == null || listGetAllChild.Count == 0 ||
                         listGetAllChild.Contains(x.OrganizationId))).ToList();
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

                    //Nếu là quản lý
                    listAllCustomer = listAllCustomer.Where(x =>
                        ((x.PersonInChargeId != null && (listEmployeeInChargeByManagerId == null ||
                                                        listEmployeeInChargeByManagerId.Count == 0 ||
                                                        listEmployeeInChargeByManagerId.FirstOrDefault(y =>
                                                            y.Equals(x.PersonInChargeId.Value)) != Guid.Empty)) ||
                        (x.PersonInChargeId == null && (listUserByManagerId == null || listUserByManagerId.Count == 0 ||
                                                        listUserByManagerId.FirstOrDefault(y =>
                                                            y.Equals(x.CreatedById)) != Guid.Empty))) ||
                        listCustomerIdForSale.Contains(x.CustomerId)
                    ).ToList();

                    List<Customer> listAllCustomerForPersonInChargeId = new List<Customer>();
                    List<Customer> listAllCustomerForCreatedById = new List<Customer>();

                    listAllCustomer.ForEach(item =>
                    {
                        var customer_contact = listContact.FirstOrDefault(x => x.ObjectId == item.CustomerId);
                        var contact = listCustomerContact.FirstOrDefault(x => x.ObjectId == item.CustomerId);
                        if (customer_contact != null)
                        {
                            CustomerEntityModel customer = new CustomerEntityModel();
                            var personInCharge =
                                listAllEmployee.FirstOrDefault(x => x.EmployeeId == item.PersonInChargeId);

                            customer.IsConverted = item.IsConverted;
                            customer.CustomerId = item.CustomerId;
                            customer.CustomerCode = item.CustomerCode != null ? item.CustomerCode.Trim() : "";
                            customer.CustomerName = item.CustomerName != null ? item.CustomerName.Trim() : "";
                            customer.ContactId = customer_contact.ContactId;

                            // Neu la phien ban cho VNS
                            if (versionName == "VNS")
                            {
                                // doi voi kh la doanh nghiep
                                if (item.CustomerType == 1)
                                {
                                    if (contact != null)
                                    {
                                        customer.ContactName = contact.FirstName + " " + contact.LastName;
                                        customer.CustomerEmail =
                                            contact.Email != null ? contact.Email.Trim() : "";
                                        customer.CustomerPhone =
                                            contact.Phone != null ? contact.Phone.Trim() : "";
                                    }
                                }
                                // KH đại lý
                                if (item.CustomerType == 3)
                                {
                                    if (contact != null)
                                    {
                                        customer.ContactName = contact.FirstName + " " + contact.LastName;
                                        customer.CustomerEmail =
                                            contact.Email != null ? contact.Email.Trim() : "";
                                        customer.CustomerPhone =
                                            contact.Phone != null ? contact.Phone.Trim() : "";
                                    }
                                }
                                //KH cá nhân
                                if (item.CustomerType == 2)
                                {
                                    customer.CustomerEmail =
                                        customer_contact.Email != null ? customer_contact.Email.Trim() : "";
                                    customer.CustomerPhone =
                                        customer_contact.Phone != null ? customer_contact.Phone.Trim() : "";
                                }
                            }
                            else
                            {
                                customer.CustomerEmail = customer_contact.Email != null ? customer_contact.Email.Trim() : "";
                                customer.CustomerPhone = customer_contact.Phone != null ? customer_contact.Phone.Trim() : "";
                            }

                            customer.FullAddress =
                                customer_contact.Address != null ? customer_contact.Address.Trim() : "";
                            customer.PicName = (personInCharge != null
                                ? (personInCharge.EmployeeName != null ? personInCharge.EmployeeName.Trim() : "")
                                : "");
                            customer.PersonInChargeId = item.PersonInChargeId;
                            //customer.CountCustomerInfo = CheckCountInformationCustomer(item.CustomerId, quote, customerOrder, receiptInvoiceMapping, bankReceiptInvoiceMapping, customerCareCustomer, payableInvoiceMapping, bankPayableInvoiceMapping);
                            customer.InvestmentFundId = item.InvestmentFundId;
                            customer.StatusId = item.StatusId;
                            customer.CusAvatarUrl = "";
                            customer.PicAvatarUrl = "";
                            customer.CareStateName =
                                listAllCareState.FirstOrDefault(c => c.CategoryId == item.CareStateId)?.CategoryName ??
                                "";
                            customer.CareSateCode =
                                listAllCareState.FirstOrDefault(c => c.CategoryId == item.CareStateId)?.CategoryCode ??
                                "";
                            customer.StatusSuportId = item.StatusSuportId;
                            customer.StatusSupportName =
                                listAllStatusSupport.FirstOrDefault(x => x.CategoryId == item.StatusSuportId)
                                    ?.CategoryName ?? "";
                            customer.CareStateId = item.CareStateId;
                            customer.CustomerGroupId = item.CustomerGroupId;
                            customer.CustomerType = item.CustomerType;
                            customer.CreatedDate = item.CreatedDate;
                            customer.UpdatedDate = item.UpdatedDate;
                            customer.Longitude = customer_contact.Longitude;
                            customer.Latitude = customer_contact.Latitude;
                            customer.PotentialId = item.PotentialId;
                            customer.EmployeeTakeCareId = item.EmployeeTakeCareId;

                            listCustomer.Add(customer);
                        }
                    });
                }
                else
                {
                    //Nếu không phải quản lý
                    listAllCustomer = listAllCustomer.Where(x =>
                        (x.PersonInChargeId != null && x.PersonInChargeId == employeeId) ||
                        (x.PersonInChargeId == null && x.CreatedById.Equals(user.UserId)) ||
                        listCustomerIdForSale.Contains(x.CustomerId)).ToList();

                    listAllCustomer.ForEach(item =>
                    {
                        var customer_contact = listContact.FirstOrDefault(x => x.ObjectId == item.CustomerId);
                        var contact = listCustomerContact.FirstOrDefault(x => x.ObjectId == item.CustomerId);
                        if (customer_contact != null)
                        {
                            CustomerEntityModel customer = new CustomerEntityModel();
                            var personInCharge = listAllEmployee.FirstOrDefault(x => x.EmployeeId == item.PersonInChargeId);

                            customer.IsConverted = item.IsConverted;
                            customer.CustomerId = item.CustomerId;
                            customer.CustomerCode = item.CustomerCode != null ? item.CustomerCode.Trim() : "";
                            customer.CustomerName = item.CustomerName != null ? item.CustomerName.Trim() : "";
                            customer.ContactId = customer_contact.ContactId;
                            // Neu la phien ban cho VNS
                            if (versionName == "VNS")
                            {
                                // doi voi kh la doanh nghiep
                                if (item.CustomerType == 1)
                                {
                                    if (contact != null)
                                    {
                                        customer.ContactName = contact.FirstName + " " + contact.LastName;
                                        customer.CustomerEmail =
                                            contact.Email != null ? contact.Email.Trim() : "";
                                        customer.CustomerPhone =
                                            contact.Phone != null ? contact.Phone.Trim() : "";
                                    }
                                }
                                else
                                {
                                    customer.CustomerEmail =
                                        customer_contact.Email != null ? customer_contact.Email.Trim() : "";
                                    customer.CustomerPhone =
                                        customer_contact.Phone != null ? customer_contact.Phone.Trim() : "";
                                }
                            }
                            else
                            {
                                customer.CustomerEmail = customer_contact.Email != null ? customer_contact.Email.Trim() : "";
                                customer.CustomerPhone = customer_contact.Phone != null ? customer_contact.Phone.Trim() : "";
                            }
                            customer.FullAddress = customer_contact.Address != null ? customer_contact.Address.Trim() : "";
                            customer.PicName = (personInCharge != null
                                ? (personInCharge.EmployeeName != null ? personInCharge.EmployeeName.Trim() : "")
                                : "");
                            customer.PersonInChargeId = item.PersonInChargeId;
                            //customer.CountCustomerInfo = CheckCountInformationCustomer(item.CustomerId, quote, customerOrder, receiptInvoiceMapping, bankReceiptInvoiceMapping, customerCareCustomer, payableInvoiceMapping, bankPayableInvoiceMapping);
                            customer.InvestmentFundId = item.InvestmentFundId;
                            customer.StatusId = item.StatusId;
                            customer.CusAvatarUrl = "";
                            customer.PicAvatarUrl = "";
                            customer.CreatedDate = item.CreatedDate;
                            customer.CareStateName =
                                listAllCareState.FirstOrDefault(c => c.CategoryId == item.CareStateId)?.CategoryName ??
                                "";
                            customer.CareSateCode =
                                listAllCareState.FirstOrDefault(c => c.CategoryId == item.CareStateId)?.CategoryCode ??
                                "";
                            customer.CareStateId = item.CareStateId;
                            customer.StatusSuportId = item.StatusSuportId;
                            customer.StatusSupportName =
                                listAllStatusSupport.FirstOrDefault(x => x.CategoryId == item.StatusSuportId)
                                    ?.CategoryName ?? "";
                            customer.CustomerGroupId = item.CustomerGroupId;
                            customer.CustomerType = item.CustomerType;
                            customer.CreatedDate = item.CreatedDate;
                            customer.UpdatedDate = item.UpdatedDate;
                            customer.Longitude = customer_contact.Longitude;
                            customer.Latitude = customer_contact.Latitude;
                            customer.PotentialId = item.PotentialId;

                            listCustomer.Add(customer);
                        }
                    });
                }

                //filter
                if (!String.IsNullOrWhiteSpace(parameter.FullName))
                {
                    listCustomer = listCustomer.Where(w => w.CustomerName != null && w.CustomerName.Trim().ToLower().Contains(parameter.FullName.Trim().ToLower())).ToList();
                }

                if (!String.IsNullOrWhiteSpace(parameter.Phone))
                {
                    listCustomer = listCustomer.Where(w => w.CustomerPhone != null && w.CustomerPhone.Trim().ToLower().Contains(parameter.Phone.Trim().ToLower())).ToList();
                }

                if (!String.IsNullOrWhiteSpace(parameter.Email))
                {
                    listCustomer = listCustomer.Where(w => w.CustomerEmail != null && w.CustomerEmail.Trim().ToLower().Contains(parameter.Email.Trim().ToLower())).ToList();
                }

                if (!String.IsNullOrWhiteSpace(parameter.Adress))
                {
                    listCustomer = listCustomer.Where(w => w.FullAddress != null && w.FullAddress.Trim().ToLower().Contains(parameter.Adress.Trim().ToLower())).ToList();
                }

                if (parameter.PersonInChargeId.Count() > 0)
                {
                    listCustomer = listCustomer.Where(w => parameter.PersonInChargeId.Contains(w.PersonInChargeId)).ToList();
                }

                if (parameter.EmployeeTakeCare.Count() > 0)
                {
                    listCustomer = listCustomer.Where(w => parameter.EmployeeTakeCare.Contains(w.EmployeeTakeCareId)).ToList();
                }

                if (parameter.InvestmentFundId.Count() > 0)
                {
                    listCustomer = listCustomer.Where(w => parameter.InvestmentFundId.Contains(w.InvestmentFundId)).ToList();
                }

                if (parameter.ListCareStateId.Count() > 0)
                {
                    listCustomer = listCustomer.Where(w => parameter.ListCareStateId.Contains(w.CareStateId)).ToList();
                }

                if (parameter.ListPotentialId.Count() > 0)
                {
                    listCustomer = listCustomer.Where(w => parameter.ListPotentialId.Contains(w.PotentialId)).ToList();
                }

                if (parameter.ListAreaId.Count() > 0)
                {
                    var listCusId = listContact.Where(c => parameter.ListAreaId.Contains(c.GeographicalAreaId)).Select(c => c.ObjectId).ToList();
                    listCustomer = listCustomer.Where(c => listCusId.Contains(c.CustomerId)).ToList();
                }

                if (parameter.ListCusGroupId.Count() > 0)
                {
                    listCustomer = listCustomer.Where(c => parameter.ListCusGroupId.Contains(c.CustomerGroupId)).ToList();
                }
                if (parameter.ListCusTypeId.Count() > 0)
                {
                    var cusTypeId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "LHL")?.CategoryTypeId;
                    var cusType1 = context.Category.FirstOrDefault(c => c.CategoryTypeId == cusTypeId && c.CategoryCode == "KCL").CategoryId;
                    var cusType2 = context.Category.FirstOrDefault(c => c.CategoryTypeId == cusTypeId && c.CategoryCode == "KPL").CategoryId;
                    var cusType3 = context.Category.FirstOrDefault(c => c.CategoryTypeId == cusTypeId && c.CategoryCode == "KHDL").CategoryId;
                    var lst = new List<short>();
                    if (parameter.ListCusTypeId.Contains(cusType1)) lst.Add(1);
                    if (parameter.ListCusTypeId.Contains(cusType2)) lst.Add(2);
                    if (parameter.ListCusTypeId.Contains(cusType3)) lst.Add(3);

                    listCustomer = listCustomer.Where(c => lst.Contains(c.CustomerType.Value)).ToList();
                }
                if (parameter.StartDate != null && parameter.StartDate != DateTime.MinValue)
                {
                    listCustomer = listCustomer.Where(c => c.CreatedDate >= parameter.StartDate).ToList();
                }
                if (parameter.EndDate != null && parameter.EndDate != DateTime.MinValue)
                {
                    listCustomer = listCustomer.Where(c => c.CreatedDate <= parameter.EndDate).ToList();
                }

                if (parameter.IsConverted == false)
                {
                    //khách hàng chưa chuyển đổi
                    listCustomer = listCustomer.Where(w => w.IsConverted != true).ToList();
                }
                else
                {
                    //khách hàng đã chuyển đổi
                    listCustomer = listCustomer.Where(w => w.IsConverted == true).ToList();
                }

                //VNS
                if (appName == "VNS")
                {
                    listCustomer = listCustomer.OrderByDescending(o => o.UpdatedDate).ToList();
                }
                //Không phải VNS
                else
                {
                    listCustomer = listCustomer.OrderByDescending(o => o.CreatedDate).ToList();
                }

                #endregion

                #region Check reference of customer

                listCustomer.ForEach(item =>
                {
                    var listLead = listAllLead.Where(x => x.CustomerId == item.CustomerId).ToList();
                    var listQuote = listAllQuote.Where(x => x.ObjectTypeId == item.CustomerId).ToList();

                    if (listLead.Count > 0)
                    {
                        item.CountCustomerReference++;
                    }
                    if (listQuote.Count > 0)
                    {
                        item.CountCustomerReference++;
                    }
                });

                #endregion

                return new SearchPotentialCustomerResult()
                {
                    ListPotentialCustomer = listCustomer,
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = "OK"
                };
            }
            catch (Exception e)
            {
                return new SearchPotentialCustomerResult()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetDataDashboardPotentialCustomerResult GetDataDashboardPotentialCustomer(GetDataDashboardPotentialCustomerParameter parameter)
        {
            try
            {
                #region master data
                var listAllUser = context.User.ToList();
                var listAllEmployee = context.Employee.ToList();
                var listCategoryType = context.CategoryType.ToList();
                var listCategory = context.Category.ToList();
                var listOrderStatus = context.OrderStatus.Where(w => w.Active == true).ToList();

                var statusCusTypeId = listCategoryType.FirstOrDefault(c => c.CategoryTypeCode == "THA").CategoryTypeId;
                var listAllStatus = listCategory.Where(c => c.CategoryTypeId == statusCusTypeId).ToList();

                var MOIStatusId = listAllStatus.FirstOrDefault(f => f.CategoryCode == "MOI").CategoryId; //status khach hang tiem nang
                var HDOStatusId = listAllStatus.FirstOrDefault(f => f.CategoryCode == "HDO").CategoryId; //status khach hang dinh danh

                var listCustomerMasterdata = context.Customer.Where(w => w.Active == true).ToList();

                var listAllCustomer = listCustomerMasterdata.Where(w => w.StatusId == MOIStatusId).ToList() ?? new List<Customer>(); //danh sach co trang thai khach hang tiem nang
                var listCustomerId = listAllCustomer.Select(w => w.CustomerId).ToList() ?? new List<Guid>();

                var listAllCustomerHDO = listCustomerMasterdata.Where(w => w.StatusId == HDOStatusId).ToList() ?? new List<Customer>(); //danh sach co trang thai khach hang dinh danh
                var listCustomerHDOId = listAllCustomerHDO.Select(w => w.CustomerId).ToList() ?? new List<Guid>();

                var listContact = context.Contact.Where(w => w.Active == true && (listCustomerId.Contains(w.ObjectId) || listCustomerHDOId.Contains(w.ObjectId)) && w.ObjectType == "CUS").ToList() ?? new List<Contact>();
                #endregion

                #region Check permision: manager
                var user = listAllUser.FirstOrDefault(x => x.UserId == parameter.UserId && x.Active == true);
                if (user == null)
                {
                    return new GetDataDashboardPotentialCustomerResult
                    {
                        StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                        MessageCode = "User không có quyền truy xuất dữ liệu trong hệ thống"
                    };
                }
                if (user.EmployeeId == null || user.EmployeeId == Guid.Empty)
                {
                    return new GetDataDashboardPotentialCustomerResult
                    {
                        StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                        MessageCode = "Lỗi dữ liệu"
                    };
                }
                var employeeId = user.EmployeeId;
                var employee = listAllEmployee.FirstOrDefault(x => x.EmployeeId == employeeId);
                var isManager = employee.IsManager;
                #endregion

                #region Get list customer reponse
                if (isManager)
                {
                    //Lấy list phòng ban con của user
                    List<Guid?> listGetAllChild = new List<Guid?>();    //List phòng ban: chính nó và các phòng ban cấp dưới của nó
                    if (employee.OrganizationId != null && isManager)
                    {
                        listGetAllChild.Add(employee.OrganizationId.Value);
                        listGetAllChild = getOrganizationChildrenId(employee.OrganizationId.Value, listGetAllChild);
                    }
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

                    //Nếu là quản lý
                    listAllCustomer = listAllCustomer.Where(x => (x.PersonInChargeId != null && (listEmployeeInChargeByManagerId == null || listEmployeeInChargeByManagerId.Count == 0 || listEmployeeInChargeByManagerId.FirstOrDefault(y => y.Equals(x.PersonInChargeId.Value)) != Guid.Empty)) ||
                                                                 (x.PersonInChargeId == null && (listUserByManagerId == null || listUserByManagerId.Count == 0 || listUserByManagerId.FirstOrDefault(y => y.Equals(x.CreatedById)) != Guid.Empty))
                                                           ).ToList();
                    //listAllCustomerHDO
                    listAllCustomerHDO = listAllCustomerHDO.Where(x => (x.PersonInChargeId != null && (listEmployeeInChargeByManagerId == null || listEmployeeInChargeByManagerId.Count == 0 || listEmployeeInChargeByManagerId.FirstOrDefault(y => y.Equals(x.PersonInChargeId.Value)) != Guid.Empty)) ||
                                                                (x.PersonInChargeId == null && (listUserByManagerId == null || listUserByManagerId.Count == 0 || listUserByManagerId.FirstOrDefault(y => y.Equals(x.CreatedById)) != Guid.Empty))
                                                          ).ToList();
                }
                else
                {
                    //Nếu không phải quản lý
                    listAllCustomer = listAllCustomer.Where(x => (x.PersonInChargeId != null && x.PersonInChargeId == employeeId) || (x.PersonInChargeId == null && x.CreatedById.Equals(user.UserId))).ToList();
                    listAllCustomerHDO = listAllCustomerHDO.Where(x => (x.PersonInChargeId != null && x.PersonInChargeId == employeeId) || (x.PersonInChargeId == null && x.CreatedById.Equals(user.UserId))).ToList();

                }

                listAllCustomer = listAllCustomer.OrderByDescending(o => o.CreatedDate).ToList();
                #endregion

                #region Lấy top 5 danh sách khách hàng tiềm năng mới nhất
                var topNewestPotentialCustomerEntity = listAllCustomer.Take(5).ToList();
                List<CustomerEntityModel> listCustomer = new List<CustomerEntityModel>();
                topNewestPotentialCustomerEntity.ForEach(item =>
                {
                    var customer_contact = listContact.FirstOrDefault(x => x.ObjectId == item.CustomerId);
                    if (customer_contact != null)
                    {
                        CustomerEntityModel customer = new CustomerEntityModel();
                        var personInCharge = listAllEmployee.FirstOrDefault(x => x.EmployeeId == item.PersonInChargeId);

                        customer.CustomerId = item.CustomerId;
                        customer.CustomerCode = item.CustomerCode != null ? item.CustomerCode.Trim() : "";
                        customer.CustomerName = item.CustomerName != null ? item.CustomerName.Trim() : "";
                        customer.ContactId = customer_contact.ContactId;
                        customer.CustomerEmail = customer_contact.Email != null ? customer_contact.Email.Trim() : "";
                        customer.CustomerPhone = customer_contact.Phone != null ? customer_contact.Phone.Trim() : "";
                        customer.PicName = (personInCharge != null ? (personInCharge.EmployeeName != null ? personInCharge.EmployeeName.Trim() : "") : "");
                        customer.StatusId = item.StatusId;
                        customer.CusAvatarUrl = "";
                        customer.PicAvatarUrl = "";
                        customer.CreatedDate = item.CreatedDate;

                        listCustomer.Add(customer);
                    }
                });
                #endregion

                #region Lấy top 5 danh sách khách hàng đã chuyển đổi mới nhất

                var topNewestPotentialCustomerConvertedEntity = listCustomerMasterdata
                    .Where(w => w.IsConverted == true && w.StatusId == HDOStatusId)
                    .OrderByDescending(x => x.UpdatedDate).Take(5).ToList();
                List<CustomerEntityModel> listCustomerConverted = new List<CustomerEntityModel>();
                topNewestPotentialCustomerConvertedEntity.ForEach(item =>
                {
                    var customer_contact = listContact.FirstOrDefault(x => x.ObjectId == item.CustomerId);
                    if (customer_contact != null)
                    {
                        CustomerEntityModel customer = new CustomerEntityModel();
                        var personInCharge = listAllEmployee.FirstOrDefault(x => x.EmployeeId == item.PersonInChargeId);

                        customer.CustomerId = item.CustomerId;
                        customer.CustomerCode = item.CustomerCode != null ? item.CustomerCode.Trim() : "";
                        customer.CustomerName = item.CustomerName != null ? item.CustomerName.Trim() : "";
                        customer.ContactId = customer_contact.ContactId;
                        customer.CustomerEmail = customer_contact.Email != null ? customer_contact.Email.Trim() : "";
                        customer.CustomerPhone = customer_contact.Phone != null ? customer_contact.Phone.Trim() : "";
                        customer.PicName = (personInCharge != null ? (personInCharge.EmployeeName != null ? personInCharge.EmployeeName.Trim() : "") : "");
                        customer.StatusId = item.StatusId;
                        customer.CusAvatarUrl = "";
                        customer.PicAvatarUrl = "";
                        customer.CreatedDate = item.CreatedDate;

                        listCustomerConverted.Add(customer);
                    }
                });
                #endregion

                #region Lấy số lượng tiềm năng theo nguồn gốc
                var investTypeId = context.CategoryType.FirstOrDefault(f => f.CategoryTypeCode == "IVF").CategoryTypeId;
                var listInvestCategory = context.Category.Where(w => w.CategoryTypeId == investTypeId && w.Active == true).ToList() ?? new List<Category>();

                var listInvestmentFund = new List<DataAccess.Models.Customer.ItemInvestmentChartPotentialCustomerDashboard>();

                var totalInvestmentFundRecord = listAllCustomer.Count();
                var totalValidCate = 0;
                if (totalInvestmentFundRecord > 0)
                {
                    listInvestCategory.ForEach(cate =>
                    {
                        var countOfSingleCate = listAllCustomer.Where(w => w.InvestmentFundId == cate.CategoryId).Count();
                        totalValidCate += countOfSingleCate;
                        var item = new DataAccess.Models.Customer.ItemInvestmentChartPotentialCustomerDashboard();
                        item.CategoryName = cate.CategoryName;
                        item.Value = countOfSingleCate;
                        item.PercentValue = item.Value / totalInvestmentFundRecord;

                        listInvestmentFund.Add(item);
                    });

                    //lấy tỉ lệ những bản ghi không có nguồn
                    if (totalInvestmentFundRecord != totalValidCate)
                    {
                        var item = new DataAccess.Models.Customer.ItemInvestmentChartPotentialCustomerDashboard();
                        item.CategoryName = "Không xác định";
                        item.Value = totalInvestmentFundRecord - totalValidCate;
                        item.PercentValue = (totalInvestmentFundRecord - totalValidCate) / totalInvestmentFundRecord;

                        listInvestmentFund.Add(item);
                    }
                }
                #endregion

                #region Phễu cơ hội
                //listAllCustomer
                var listCustomerForFunnel = listAllCustomer.ToList().Concat(listAllCustomerHDO?.Where(x => x.IsFromLead == true).ToList());

                //var listAllCustomerHDOId = listAllCustomerHDO.Select(w => w.CustomerId).ToList() ?? new List<Guid>();
                //listCustomerForFunnel.AddRange(listAllCustomerHDO);

                //danh sách khách hàng tiềm năng đã chuyển đổi
                var listAllConvertedCustomer = listAllCustomerHDO.ToList() ?? new List<Customer>();
                var listAllConvertedCustomerId = listAllConvertedCustomer.Select(w => w.CustomerId).ToList() ?? new List<Guid>();

                //danh sách cơ hội từ khách hàng tiềm năng - trạng thái khác hủy - các cơ hội trước khi chuyển đổi
                var leadStatusTypeId = listCategoryType.FirstOrDefault(f => f.CategoryTypeCode == "CHS").CategoryTypeId;
                var leadCancelId = listCategory.FirstOrDefault(f => f.CategoryTypeId == leadStatusTypeId && f.CategoryCode == "CANC").CategoryId;

                var listLead = context.Lead.Where(w => w.Active == true && listCustomerId.Contains(w.CustomerId.Value) && w.StatusId != leadCancelId).ToList() ?? new List<Lead>();

                var listLeadPotential = context.Lead.Where(w => w.Active == true && listCustomerId.Contains(w.CustomerId.Value) && w.StatusId != leadCancelId).ToList() ?? new List<Lead>();
                var listLeadPotentialId = listLeadPotential.Select(w => w.LeadId).ToList() ?? new List<Guid>();

                // Lead của KH tiềm năng đã chuyển đổi
                var listLeadHDO = context.Lead.Where(w => w.Active == true && listAllConvertedCustomerId.Contains(w.CustomerId.Value) && w.StatusId != leadCancelId).ToList() ?? new List<Lead>();
                var listLeadIdHDO = listLeadHDO.Select(w => w.LeadId).ToList() ?? new List<Guid>();

                listAllCustomerHDO.ForEach(cus =>
                {
                    var lstLead = listLeadHDO.Where(x => x.CustomerId == cus.CustomerId && cus.IsFromLead == true && (x.CreatedDate <= cus.PotentialConversionDate || cus.PotentialConversionDate == null)).ToList();
                    if (lstLead != null && lstLead.Count() > 0)
                    {
                        listLead.AddRange(lstLead);
                    }
                });


                var listLeadId = listLead.Select(w => w.LeadId).ToList() ?? new List<Guid>();
                //danh sách báo giá tạo từ cơ hội - trạng thái khác hủy
                var quoteStatusTypeId = listCategoryType.FirstOrDefault(f => f.CategoryTypeCode == "TGI").CategoryTypeId;
                var quoteCancelId = listCategory.FirstOrDefault(f => f.CategoryTypeId == quoteStatusTypeId && f.CategoryCode == "HUY").CategoryId;
                var listQuote = context.Quote.Where(w => w.Active == true && listLeadId.Contains(w.LeadId.Value) && w.StatusId != quoteCancelId).ToList() ?? new List<Quote>();
                var listQuoteId = listQuote.Select(w => w.QuoteId).ToList() ?? new List<Guid>();


                //danh sách đơn hàng được tạo từ báo giá của KH tiềm năng - trạng thái khách hủy
                var listCustomerAfterConvert = listCustomerForFunnel.Where(w => w.IsFromLead == true).ToList() ?? new List<Customer>();
                var listCustomerAfterConvertId = listCustomerAfterConvert.Select(w => w.CustomerId).ToList() ?? new List<Guid>();

                var orderStatusTypeId = listOrderStatus.FirstOrDefault(f => f.OrderStatusCode == "CAN").OrderStatusId;
                var listCustomerOrder = context.CustomerOrder.Where(w => w.Active == true
                                                                    && listQuoteId.Contains(w.QuoteId.Value)
                                                                    //|| listCustomerAfterConvertId.Contains(w.CustomerId.Value))                                                                  
                                                                    && w.StatusId != orderStatusTypeId).ToList() ?? new List<CustomerOrder>();

                var funnelChartPotentialDasboard = new DataAccess.Models.Customer.FunnelChartPotentialDasboardModel();
                funnelChartPotentialDasboard.TotalPotentialCustomerConverted = listCustomerForFunnel.Count();
                funnelChartPotentialDasboard.TotalLead = listLead.Count();
                funnelChartPotentialDasboard.TotalQuote = listQuote.Count();
                funnelChartPotentialDasboard.TotalCustomerOrder = listCustomerOrder.Count();

                var percentPotentialToLead = (funnelChartPotentialDasboard.TotalLead > 0 && funnelChartPotentialDasboard.TotalPotentialCustomerConverted != 0) ? (decimal)(funnelChartPotentialDasboard.TotalLead / funnelChartPotentialDasboard.TotalPotentialCustomerConverted) * 100 : 0;
                var percentLeadToQuote = (funnelChartPotentialDasboard.TotalQuote > 0 && funnelChartPotentialDasboard.TotalPotentialCustomerConverted != 0) ? (decimal)(funnelChartPotentialDasboard.TotalQuote / funnelChartPotentialDasboard.TotalPotentialCustomerConverted) * 100 : 0;
                var percentQuoteToOrder = (funnelChartPotentialDasboard.TotalCustomerOrder > 0 && funnelChartPotentialDasboard.TotalPotentialCustomerConverted != 0) ? (decimal)(funnelChartPotentialDasboard.TotalCustomerOrder / funnelChartPotentialDasboard.TotalPotentialCustomerConverted) * 100 : 0;

                percentPotentialToLead = Math.Round(percentPotentialToLead, 2);
                percentLeadToQuote = Math.Round(percentLeadToQuote, 2);
                percentQuoteToOrder = Math.Round(percentQuoteToOrder, 2);

                funnelChartPotentialDasboard.PercentPotentialToLead = percentPotentialToLead.ToString() + "%";
                funnelChartPotentialDasboard.PercentLeadToQuote = percentLeadToQuote.ToString() + "%";
                funnelChartPotentialDasboard.PercentQuoteToOrder = percentQuoteToOrder.ToString() + "%";
                #endregion


                return new GetDataDashboardPotentialCustomerResult()
                {
                    ListInvestmentFundDasboard = listInvestmentFund ?? new List<ItemInvestmentChartPotentialCustomerDashboard>(),
                    TopNewestCustomer = listCustomer ?? new List<CustomerEntityModel>(),
                    TopNewestCustomerConverted = listCustomerConverted ?? new List<CustomerEntityModel>(),
                    FunnelChartPotentialDasboardModel = funnelChartPotentialDasboard,
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = "OK"
                };
            }
            catch (Exception e)
            {
                return new GetDataDashboardPotentialCustomerResult()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public ConvertPotentialCustomerResult ConvertPotentialCustomer(ConvertPotentialCustomerParameter parameter)
        {
            var potentialCustomer = new Customer();

            try
            {
                potentialCustomer = context.Customer.FirstOrDefault(f => f.CustomerId == parameter.CustomerId);
                var potentialCustomerContact =
                    context.Contact.FirstOrDefault(f => f.ObjectId == parameter.CustomerId && f.ObjectType == "CUS");

                #region Tạo khách hàng (chuyển tiềm năng => định danh)

                if (parameter.IsCreateCustomer == true)
                {
                    #region comment by dungpt
                    //var newCustomer = new Customer();
                    //newCustomer.CustomerId = Guid.NewGuid();
                    //newCustomer.CustomerCode = this.GenerateCustomerCode(0);
                    //newCustomer.CustomerGroupId = potentialCustomer.CustomerGroupId;
                    //newCustomer.CustomerName = potentialCustomer.CustomerName;
                    //newCustomer.LeadId = potentialCustomer.LeadId;
                    //newCustomer.StatusId = newCusId;
                    //newCustomer.PersonInChargeId = parameter.PersonalInChargeId;
                    //newCustomer.CustomerType = potentialCustomer.CustomerType;

                    //newCustomer.CustomerServiceLevel = potentialCustomer.CustomerServiceLevel;
                    //newCustomer.PaymentId = potentialCustomer.PaymentId;
                    //newCustomer.FieldId = potentialCustomer.FieldId;
                    //newCustomer.ScaleId = potentialCustomer.ScaleId;
                    //newCustomer.MaximumDebtDays = potentialCustomer.MaximumDebtDays;
                    //newCustomer.MaximumDebtValue = potentialCustomer.MaximumDebtValue;
                    //newCustomer.TotalSaleValue = potentialCustomer.TotalSaleValue;
                    //newCustomer.TotalReceivable = potentialCustomer.TotalReceivable;

                    //newCustomer.Active = true;
                    //newCustomer.CreatedById = parameter.UserId;
                    //newCustomer.CreatedDate = DateTime.Now;
                    //newCustomer.InvestmentFundId = potentialCustomer.InvestmentFundId;
                    //newCustomer.IsFromLead = false;
                    //newCustomer.PotentialCustomerId = parameter.CustomerId;

                    //var newCustomerContact = new Contact();
                    //newCustomerContact.ContactId = Guid.NewGuid();
                    //newCustomerContact.ObjectId = newCustomer.CustomerId;
                    //newCustomerContact.ObjectType = "CUS";

                    //newCustomerContact.FirstName = potentialCustomerContact.FirstName;
                    //newCustomerContact.LastName = potentialCustomerContact.LastName;
                    //newCustomerContact.Gender = potentialCustomerContact.Gender;
                    //newCustomerContact.DateOfBirth = potentialCustomerContact.DateOfBirth;
                    //newCustomerContact.Phone = potentialCustomerContact.Phone;
                    //newCustomerContact.WorkPhone = potentialCustomerContact.WorkPhone;
                    //newCustomerContact.OtherPhone = potentialCustomerContact.OtherPhone;
                    //newCustomerContact.Email = potentialCustomerContact.Email;
                    //newCustomerContact.WorkEmail = potentialCustomerContact.WorkEmail;
                    //newCustomerContact.OtherEmail = potentialCustomerContact.OtherEmail;
                    //newCustomerContact.AvatarUrl = potentialCustomerContact.AvatarUrl;
                    //newCustomerContact.IdentityId = potentialCustomerContact.IdentityId;
                    //newCustomerContact.Address = potentialCustomerContact.Address;
                    //newCustomerContact.ProvinceId = potentialCustomerContact.ProvinceId;
                    //newCustomerContact.DistrictId = potentialCustomerContact.DistrictId;
                    //newCustomerContact.WardId = potentialCustomerContact.WardId;
                    //newCustomerContact.PostCode = potentialCustomerContact.PostCode;
                    //newCustomerContact.WebsiteUrl = potentialCustomerContact.WebsiteUrl;
                    //newCustomerContact.SocialUrl = potentialCustomerContact.SocialUrl;
                    //newCustomerContact.Note = potentialCustomerContact.Note;
                    //newCustomerContact.Role = potentialCustomerContact.Role;
                    //newCustomerContact.TaxCode = potentialCustomerContact.TaxCode;

                    //newCustomerContact.Active = true;
                    //newCustomerContact.CreatedById = parameter.UserId;
                    //newCustomerContact.CreatedDate = DateTime.Now;

                    //context.Customer.Add(newCustomer);
                    //context.Contact.Add(newCustomerContact);
                    #endregion

                    #region Add by dungpt

                    var cusSttId = context.CategoryType.FirstOrDefault(ct => ct.CategoryTypeCode == "THA")
                        .CategoryTypeId;
                    var newCusId = context.Category
                        .FirstOrDefault(c => c.CategoryTypeId == cusSttId && c.CategoryCode == "HDO").CategoryId; //khách hàng  định danh
                    potentialCustomer.StatusId = newCusId;
                    potentialCustomer.PersonInChargeId = parameter.PersonalInChargeId;
                    potentialCustomer.UpdatedById = parameter.UserId;
                    potentialCustomer.UpdatedDate = DateTime.Now;

                    #endregion
                }

                #endregion

                #region Tạo cơ hội

                if (!string.IsNullOrEmpty(parameter.LeadName))
                {
                    var newLead = new Lead();
                    newLead.LeadId = Guid.NewGuid();
                    newLead.PersonInChargeId = potentialCustomer.PersonInChargeId;
                    newLead.CustomerId = potentialCustomer.CustomerId;
                    newLead.LeadCode = GenerateCode();

                    //mặc định trạng thái là NHÁP
                    var leadStatusTypeId = context.CategoryType.FirstOrDefault(f => f.CategoryTypeCode == "CHS")
                        .CategoryTypeId;
                    var leadStatusDraftId =
                        context.Category
                            .FirstOrDefault(f => f.CategoryTypeId == leadStatusTypeId && f.CategoryCode == "DRAFT")
                            ?.CategoryId ?? new Guid();

                    newLead.StatusId = leadStatusDraftId;

                    //Lấy loại khách hàng: Doanh nghiệp, Cá nhân
                    var leadTypeCategoryType = context.CategoryType.FirstOrDefault(w => w.CategoryTypeCode == "LHL")
                        .CategoryTypeId;

                    //Khách hàng doanh nghiệp
                    if (potentialCustomer.CustomerType == 1)
                    {
                        var leadTypeId = context.Category.FirstOrDefault(x =>
                            x.CategoryTypeId == leadTypeCategoryType && x.CategoryCode == "KCL")?.CategoryId;

                        newLead.LeadTypeId = leadTypeId;
                    }
                    //Khách hàng cá nhân
                    else if (potentialCustomer.CustomerType == 2)
                    {
                        var leadTypeId = context.Category.FirstOrDefault(x =>
                            x.CategoryTypeId == leadTypeCategoryType && x.CategoryCode == "KPL")?.CategoryId;

                        newLead.LeadTypeId = leadTypeId;
                    }

                    newLead.CreatedDate = DateTime.Now;
                    newLead.CreatedById = parameter.UserId.ToString();

                    context.Lead.Add(newLead);

                    #region Lấy liên hệ của Khách hàng tiềm năng sang Cơ hội

                    var customerContact = context.Contact.FirstOrDefault(x =>
                        x.ObjectType == "CUS" && x.ObjectId == potentialCustomer.CustomerId);

                    if (customerContact != null)
                    {
                        customerContact.ContactId = Guid.NewGuid();
                        customerContact.ObjectId = newLead.LeadId;
                        customerContact.ObjectType = "LEA";
                        customerContact.FirstName = parameter.LeadName;
                        customerContact.LastName = "";
                        customerContact.CreatedDate = DateTime.Now;
                        customerContact.CreatedById = parameter.UserId;

                        context.Contact.Add(customerContact);
                    }

                    var listContactCon = context.Contact
                        .Where(x => x.ObjectType == "CUS_CON" && x.ObjectId == potentialCustomer.CustomerId).ToList();

                    if (listContactCon.Count > 0)
                    {
                        listContactCon.ForEach(item =>
                        {
                            item.ContactId = Guid.NewGuid();
                            item.ObjectType = "LEA_CON";
                            item.ObjectId = newLead.LeadId;
                            item.CreatedDate = DateTime.Now;
                            item.CreatedById = parameter.UserId;
                        });

                        context.Contact.AddRange(listContactCon);
                    }

                    #endregion

                    #region Thay đổi trạng thái phụ của Khách hàng tiềm năng thành Chuyển đổi

                    var statusSupportType = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "TTPKHTN")
                        ?.CategoryTypeId;
                    var statusSupportId = context.Category
                        .FirstOrDefault(x => x.CategoryTypeId == statusSupportType && x.CategoryCode == "E")
                        ?.CategoryId;

                    potentialCustomer.StatusSuportId = statusSupportId;

                    #endregion

                    #region Giang comment

                    if (parameter.IsCreateLead == true)
                    {
                        //var newLead = new Lead();
                        //newLead.LeadId = Guid.NewGuid();

                        ////mặc định trạng thái là NHÁP
                        //var leadStatusTypeId = context.CategoryType.FirstOrDefault(f => f.CategoryTypeCode == "CHS")
                        //    .CategoryTypeId;
                        //var leadStatusDraftId =
                        //    context.Category
                        //        .FirstOrDefault(f => f.CategoryTypeId == leadStatusTypeId && f.CategoryCode == "DRAFT")
                        //        ?.CategoryId ?? new Guid();

                        //newLead.RequirementDetail = parameter.LeadModel.RequirementDetail != null
                        //    ? parameter.LeadModel.RequirementDetail.Trim()
                        //    : parameter.LeadModel.RequirementDetail;
                        //newLead.PotentialId = parameter.LeadModel.PotentialId;
                        //newLead.InterestedGroupId = parameter.LeadModel.InterestedGroupId;
                        //newLead.PersonInChargeId = parameter.LeadModel.PersonInChargeId;
                        //newLead.CompanyId = parameter.LeadModel.CompanyId;
                        //newLead.StatusId = leadStatusDraftId;
                        //newLead.Role = parameter.LeadModel.Role;
                        //newLead.WaitingForApproval = parameter.LeadModel.WaitingForApproval;
                        //newLead.LeadTypeId = parameter.LeadModel.LeadTypeId;
                        //newLead.LeadGroupId = parameter.LeadModel.LeadGroupId;
                        //newLead.CustomerId = parameter.LeadModel.CustomerId;
                        //newLead.BusinessTypeId = parameter.LeadModel.BusinessTypeId;
                        //newLead.InvestmentFundId = parameter.LeadModel.InvestmentFundId;
                        //newLead.ExpectedSale = parameter.LeadModel.ExpectedSale;
                        //newLead.ProbabilityId = parameter.LeadModel.ProbabilityId;
                        //newLead.LeadCode = GenerateCode();
                        //newLead.CloneCount = 0;
                        //newLead.Active = true;
                        //newLead.CreatedDate = DateTime.Now;
                        //newLead.CreatedById = parameter.UserId.ToString();

                        //var newLeadContact = new Contact();
                        //newLeadContact.ContactId = Guid.NewGuid();
                        //newLeadContact.ObjectId = newLead.LeadId;
                        //newLeadContact.ObjectType = ObjectType.LEAD;
                        //newLeadContact.FirstName = parameter.ContactModel.FirstName.Trim();
                        //newLeadContact.LastName = parameter.ContactModel.LastName != null
                        //    ? parameter.ContactModel.LastName.Trim()
                        //    : parameter.ContactModel.LastName;
                        //newLeadContact.Gender = parameter.ContactModel.Gender;
                        //newLeadContact.Address = parameter.ContactModel.Address != null
                        //    ? parameter.ContactModel.Address.Trim()
                        //    : parameter.ContactModel.Address;
                        //newLeadContact.Email = parameter.ContactModel.Email != null
                        //    ? parameter.ContactModel.Email.Trim()
                        //    : parameter.ContactModel.Email;
                        //newLeadContact.DateOfBirth = parameter.ContactModel.DateOfBirth;
                        //newLeadContact.Phone = parameter.ContactModel.Phone;
                        //newLeadContact.WorkPhone = parameter.ContactModel.WorkPhone;
                        //newLeadContact.OtherPhone = parameter.ContactModel.OtherPhone;
                        //newLeadContact.WorkEmail = parameter.ContactModel.WorkEmail;
                        //newLeadContact.OtherEmail = parameter.ContactModel.OtherEmail;
                        //newLeadContact.AvatarUrl = parameter.ContactModel.AvatarUrl;
                        //newLeadContact.IdentityId = parameter.ContactModel.IdentityId;
                        //newLeadContact.ProvinceId = parameter.ContactModel.ProvinceId;
                        //newLeadContact.DistrictId = parameter.ContactModel.DistrictId;
                        //newLeadContact.WardId = parameter.ContactModel.WardId;
                        //newLeadContact.PostCode = parameter.ContactModel.PostCode;
                        //newLeadContact.WebsiteUrl = parameter.ContactModel.WebsiteUrl;
                        //newLeadContact.SocialUrl = parameter.ContactModel.SocialUrl;
                        //newLeadContact.Note = parameter.ContactModel.Note;
                        //newLeadContact.Role = parameter.ContactModel.Role;
                        //newLeadContact.TaxCode = parameter.ContactModel.TaxCode;
                        //newLeadContact.CreatedDate = DateTime.Now;
                        //newLeadContact.CreatedById = parameter.UserId;
                        //newLeadContact.Active = true;

                        ////thêm danh sách nhu cầu SP/DV của cơ hội
                        //var listInterested = new List<LeadInterestedGroupMapping>();
                        //parameter.ListInterestedGroupId?.ForEach(item =>
                        //{
                        //    listInterested.Add(new LeadInterestedGroupMapping
                        //    {
                        //        LeadInterestedGroupMappingId = Guid.NewGuid(),
                        //        LeadId = newLead.LeadId,
                        //        InterestedGroupId = item.Value,
                        //        Active = true,
                        //        CreatedById = parameter.UserId,
                        //        CreatedDate = DateTime.Now
                        //    });
                        //});

                        //context.Lead.Add(newLead);
                        //context.Contact.Add(newLeadContact);
                        //context.LeadInterestedGroupMapping.AddRange(listInterested);
                    }

                    #endregion
                }

                #endregion

                //đánh dấu là đã chuyển đổi(chỉ convert 1 lần)
                potentialCustomer.IsConverted = true;
                context.Customer.Update(potentialCustomer);

                context.SaveChanges();
            }
            catch (Exception e)
            {
                return new ConvertPotentialCustomerResult()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }

            #region Gửi mail thông báo

            NotificationHelper.AccessNotification(context, TypeModel.PotentialCustomerDetail, "CONVERT", new Customer(), potentialCustomer, true);

            #endregion

            return new ConvertPotentialCustomerResult()
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                MessageCode = "OK"
            };
        }

        public DownloadTemplatePotentialCustomerResult DownloadTemplatePotentialCustomer(DownloadTemplatePotentialCustomerParameter parameter)
        {
            try
            {
                string rootFolder = hostingEnvironment.WebRootPath + "\\ExcelTemplate";
                string fileName = @"TemplateImportPotentialCustomer.xlsx";
                string newFilePath = Path.Combine(rootFolder, fileName);
                byte[] data = File.ReadAllBytes(newFilePath);

                return new DownloadTemplatePotentialCustomerResult
                {
                    ExcelFile = data,
                    MessageCode = "",
                    NameFile = "TemplateImportPotentialCustomer",
                    StatusCode = System.Net.HttpStatusCode.OK,
                };
            }
            catch (Exception e)
            {
                return new DownloadTemplatePotentialCustomerResult()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetDataImportPotentialCustomerResult GetDataImportPotentialCustomer(GetDataImportPotentialCustomerParameter parameter)
        {
            try
            {
                //master data
                var categoryTypeList = context.CategoryType.Where(cty => cty.Active == true).ToList();
                var categoryList = context.Category.Where(ct => ct.Active == true).ToList();
                var employeeList = context.Employee.Where(emp => emp.Active == true).ToList();
                var portalUserCode = "PortalUser"; //loại portalUser

                #region Define Category Code
                var listCategoryCode = new List<string>();
                var customerGroupCode = "IVF"; //nguon tiem nang
                listCategoryCode.Add(customerGroupCode);
                #endregion

                #region Lấy danh sách người phụ trách
                var ListEmployeeEntityModel = new List<DataAccess.Models.Employee.EmployeeEntityModel>();
                var listEmployeeEntity = employeeList.Where(w => w.Active == true && w.EmployeeCode != portalUserCode).ToList(); //loai portal user

                var employeeIdUser = context.User.FirstOrDefault(w => w.UserId == parameter.UserId).EmployeeId;

                var employeeById = listEmployeeEntity.Where(w => w.EmployeeId == employeeIdUser).FirstOrDefault();
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
                    var listEmployeeFiltered = listEmployeeEntity.Where(w => listOrganizationChildrenId.Contains(w.OrganizationId)).Select(w => new
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
                            EmployeeCode = emp.EmployeeCode
                        });
                    });
                }
                else
                {
                    //Nhân viên: chỉ lấy nhân viên đó
                    var employeeId = listEmployeeEntity.Where(e => e.EmployeeId == employeeIdUser).FirstOrDefault();
                    ListEmployeeEntityModel.Add(new Models.Employee.EmployeeEntityModel
                    {
                        EmployeeId = employeeId.EmployeeId,
                        EmployeeName = employeeId.EmployeeName,
                        EmployeeCode = employeeId.EmployeeCode
                    });
                }
                #endregion

                #region Lấy danh sách nguồn gốc
                var ListInvestFund = new List<CategoryEntityModel>();
                var listCategoryTypeEntity = categoryTypeList.Where(w => listCategoryCode.Contains(w.CategoryTypeCode) && w.Active == true).ToList();
                var listCateTypeId = new List<Guid>();
                listCategoryTypeEntity?.ForEach(type =>
                {
                    listCateTypeId.Add(type.CategoryTypeId);
                });
                var listCategoryEntity = categoryList.Where(w => listCateTypeId.Contains(w.CategoryTypeId) && w.Active == true).ToList(); //list master data của category

                //get customer group
                var customerGroupTypeId = listCategoryTypeEntity.Where(w => w.CategoryTypeCode == customerGroupCode).FirstOrDefault()?.CategoryTypeId;
                var listCustomerGroupEntity = listCategoryEntity.Where(w => w.CategoryTypeId == customerGroupTypeId && w.CategoryCode != "POR").ToList(); //loại Khách hàng Portal                                              
                listCustomerGroupEntity?.ForEach(group =>
                {
                    ListInvestFund.Add(new CategoryEntityModel
                    {
                        CategoryId = group.CategoryId,
                        CategoryName = group.CategoryName,
                        CategoryCode = group.CategoryCode,
                        CategoryTypeId = group.CategoryTypeId,
                        IsDefault = group.IsDefauld
                    });
                });
                #endregion

                #region Lấy danh sách email và sdt khách hàng
                var listContactEntity = context.Contact.Where(w => w.Active == true && (w.ObjectType == "CUS")).ToList() ?? new List<Contact>();
                var listEmail = listContactEntity.Where(w => !string.IsNullOrWhiteSpace(w.Email)).Select(w => w.Email).Distinct().ToList() ?? new List<string>();
                var listPhone = listContactEntity.Where(w => !string.IsNullOrWhiteSpace(w.Phone)).Select(w => w.Phone).Distinct().ToList() ?? new List<string>();
                #endregion

                return new GetDataImportPotentialCustomerResult
                {
                    ListPersonalInChange = ListEmployeeEntityModel ?? new List<EmployeeEntityModel>(),
                    ListInvestFund = ListInvestFund ?? new List<CategoryEntityModel>(),
                    ListEmail = listEmail,
                    ListPhone = listPhone,
                    MessageCode = "",
                    StatusCode = System.Net.HttpStatusCode.OK
                };
            }
            catch (Exception e)
            {
                return new GetDataImportPotentialCustomerResult()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        private bool ComparePotentialCustomer(ComparePotentialCustomerModel oldCustomer, ComparePotentialCustomerModel newCustomer, List<DataAccess.Models.Document.LinkOfDocumentEntityModel> listLinkOfDocument,
                        List<LinkOfDocument> ListOldLink)
        {
            //true: không thay đổi; false: có thay đổi
            var result = false;

            var listNewLink = listLinkOfDocument.Where(w => w.IsNewLink == true).ToList() ?? new List<Models.Document.LinkOfDocumentEntityModel>();


            if (oldCustomer.Phone == newCustomer.Phone
                && oldCustomer.WorkPhone == newCustomer.WorkPhone
                && oldCustomer.OtherPhone == newCustomer.OtherPhone
                && oldCustomer.Email == newCustomer.Email
                && oldCustomer.WorkEmail == newCustomer.WorkEmail
                && oldCustomer.PersonInCharge?.EmployeeId == newCustomer.PersonInCharge?.EmployeeId
                && oldCustomer.InvestmentFund?.CategoryId == newCustomer.InvestmentFund?.CategoryId
                && listNewLink.Count() == 0
                && ListOldLink.Count() == 0
            ) return true;

            return result;
        }

        private bool CompareCustomer(CompareCustomerModel oldCustomer, CompareCustomerModel newCustomer)
        {
            //true: không thay đổi; false: có thay đổi
            var result = false;
            if (oldCustomer.PersonInCharge?.EmployeeId == newCustomer.PersonInCharge?.EmployeeId)
                return true;

            return result;
        }

        private string GetNoteDescriptionCustomer(CompareCustomerModel oldCustomer, CompareCustomerModel newCustomer, Employee employee)
        {
            var result = "<ul>";
            //thêm ghi chú cho người phụ trách
            if (oldCustomer.PersonInCharge?.EmployeeId != newCustomer.PersonInCharge?.EmployeeId)
            {
                if (oldCustomer.PersonInCharge?.EmployeeId == null)
                {
                    var newEmployeeName = newCustomer.PersonInCharge?.EmployeeName ?? "";
                    result += "<li>" + "Đã thêm người phụ trách " + "<b>" + newEmployeeName + "</b>" + "</li>";
                }
                else if (newCustomer.PersonInCharge?.EmployeeId == null)
                {
                    result += "<li>" + "Đã hủy người phụ trách" + "</li>";
                }
                else
                {
                    var oldEmployeeName = oldCustomer.PersonInCharge?.EmployeeName ?? "";
                    var newEmployeeName = newCustomer.PersonInCharge?.EmployeeName ?? "";
                    //[Mã nhân viên] - [Tên nhân viên] đã đổi người phụ trách / nhân viên bán hàng từ[Tên Người phụ trách cũ] thành[Tên Người phụ trách mới].
                    result += "<li>" + "<b>" + employee.EmployeeCode + " - " + employee.EmployeeName + "</b>" + " đã đổi người phụ trách từ " + "<b>" + oldEmployeeName + "</b>" + " thành " + "<b>" + newEmployeeName ?? "" + "</b>" + "</li>";
                }
            }
            result += "</ul>";

            return result;
        }

        private string GetNoteDescription(ComparePotentialCustomerModel oldCustomer, ComparePotentialCustomerModel newCustomer, List<DataAccess.Models.Document.LinkOfDocumentEntityModel> listLinkOfDocument,
            List<LinkOfDocument> ListOldLink, Employee employee)
        {
            var result = "<ul>";

            //format:<li>text</li>
            //thêm ghi chú cho thay đổi số điện thoại
            if (oldCustomer.Phone != newCustomer.Phone)
            {
                if (string.IsNullOrEmpty(oldCustomer.Phone))
                {
                    result += "<li>" + "Đã thêm số điện thoại" + "<b>" + newCustomer.Phone + "</b>" + "</li>";
                }
                else
                {
                    result += "<li>" + "Đã chỉnh sửa số điện thoại " + "<b>" + oldCustomer.Phone + "</b>" + " thành " + "<b>" + newCustomer.Phone + "</b>" + "</li>";
                }
            }

            //thêm ghi chú cho thay đổi số điện thoại cơ quan
            if (newCustomer.WorkPhone != "" && oldCustomer.WorkPhone != newCustomer.WorkPhone)
            {
                if (string.IsNullOrEmpty(oldCustomer.WorkPhone))
                {
                    result += "<li>" + "Đã thêm số điện thoại cơ quan " + "<b>" + newCustomer.WorkPhone + "</b>" + "</li>";
                }
                else
                {
                    result += "<li>" + "Đã chỉnh sửa số điện thoại cơ quan " + "<b>" + oldCustomer.WorkPhone + "</b>" + " thành " + "<b>" + newCustomer.WorkPhone + "</b>" + "</li>";
                }
            }

            //thêm ghi chú cho thay đổi số điện thoại khác
            if (newCustomer.OtherPhone != "" && oldCustomer.OtherPhone != newCustomer.OtherPhone)
            {
                if (string.IsNullOrEmpty(oldCustomer.OtherPhone))
                {
                    result += "<li>" + "Đã thêm số điện thoại khác " + "<b>" + newCustomer.OtherPhone + "</b>" + "</li>";
                }
                else
                {
                    result += "<li>" + "Đã chỉnh sửa số điện thoại khác " + "<b>" + oldCustomer.OtherPhone + "</b>" + " thành " + "<b>" + newCustomer.OtherPhone + "</b>" + "</li>";
                }
            }

            //thêm ghi chú cho thay đổi email cá nhân
            if (newCustomer.Email != "" && oldCustomer.Email != newCustomer.Email)
            {
                if (string.IsNullOrEmpty(oldCustomer.Email))
                {
                    result += "<li>" + "Đã thêm email cá nhân " + "<b>" + newCustomer.Email + "</b>" + "</li>";
                }
                else
                {
                    result += "<li>" + "Đã chỉnh sửa email cá nhân " + "<b>" + oldCustomer.Email + "</b>" + " thành " + "<b>" + newCustomer.Email + "</b>" + "</li>";
                }
            }

            //thêm ghi chú cho thay đổi email cơ quan
            if (newCustomer.WorkEmail != "" && oldCustomer.WorkEmail != newCustomer.WorkEmail)
            {
                if (string.IsNullOrEmpty(oldCustomer.WorkEmail))
                {
                    result += "<li>" + "Đã thêm email cơ quan " + "<b>" + newCustomer.WorkEmail + "</b>" + "</li>";
                }
                else
                {
                    result += "<li>" + "Đã chỉnh sửa email cơ quan " + "<b>" + oldCustomer.WorkEmail + "</b>" + " thành " + "<b>" + newCustomer.WorkEmail + "</b>" + "</li>";
                }
            }

            //thêm ghi chú cho người phụ trách
            if (oldCustomer.PersonInCharge?.EmployeeId != newCustomer.PersonInCharge?.EmployeeId)
            {
                if (oldCustomer.PersonInCharge?.EmployeeId == null)
                {
                    var newEmployeeName = newCustomer.PersonInCharge?.EmployeeName ?? "";
                    result += "<li>" + "Đã thêm người phụ trách " + "<b>" + newEmployeeName + "</b>" + "</li>";
                }
                else if (newCustomer.PersonInCharge?.EmployeeId == null)
                {
                    result += "<li>" + "Đã hủy người phụ trách" + "</li>";
                }
                else
                {
                    var oldEmployeeName = oldCustomer.PersonInCharge?.EmployeeName ?? "";
                    var newEmployeeName = newCustomer.PersonInCharge?.EmployeeName ?? "";
                    //[Mã nhân viên] - [Tên nhân viên] đã đổi người phụ trách / nhân viên bán hàng từ[Tên Người phụ trách cũ] thành[Tên Người phụ trách mới].
                    result += "<li>" + "<b>" + employee.EmployeeCode + " - " + employee.EmployeeName + "</b>" + " đã đổi người phụ trách / nhân viên bán hàng từ " + "<b>" + oldEmployeeName + "</b>" + " thành " + "<b>" + newEmployeeName ?? "" + "</b>" + "</li>";
                }
            }

            //thêm ghi chú cho thay đổi nguồn tiềm năng
            if (oldCustomer.InvestmentFund?.CategoryId != newCustomer.InvestmentFund?.CategoryId)
            {
                if (oldCustomer.InvestmentFund?.CategoryId == null)
                {
                    var newCate = newCustomer.InvestmentFund?.CategoryName ?? "";
                    result += "<li>" + "Đã thêm nguồn tiềm năng " + "<b>" + newCate + "</b>" + "</li>";
                }
                else if (newCustomer.InvestmentFund?.CategoryId == null)
                {
                    result += "<li>" + "Đã hủy nguồn tiềm năng" + "</li>";
                }
                else
                {
                    var oldCate = oldCustomer.InvestmentFund?.CategoryName ?? "";
                    var newCate = newCustomer.InvestmentFund?.CategoryName ?? "";
                    result += "<li>" + "Đã chỉnh sửa nguồn tiềm năng " + "<b>" + oldCate + "</b>" + " thành " + "<b>" + newCate ?? "" + "</b>" + "</li>";
                }
            }

            //thêm ghi chú cho thay đổi tài liệu đính kèm
            //1.link đính kèm
            var listNewLink = listLinkOfDocument.Where(w => w.IsNewLink == true).ToList() ?? new List<Models.Document.LinkOfDocumentEntityModel>();
            var newLinkText = GetListHrefString(listNewLink);

            if (listNewLink.Count() > 0)
            {
                result += "<li>" + "Đã thêm liên kết " + newLinkText + "</li>";
            }
            //2.link đính kèm đã bị xóa
            var listOldLink = new List<Models.Document.LinkOfDocumentEntityModel>();
            ListOldLink.ForEach(item =>
            {
                listOldLink.Add(new Models.Document.LinkOfDocumentEntityModel
                {
                    LinkName = item.LinkName,
                    LinkValue = item.LinkValue
                });
            });
            var oldLinkText = GetListHrefString(listOldLink);
            if (oldLinkText.Count() > 0)
            {
                result += "<li>" + "Đã xóa liên kết " + oldLinkText + "</li>";
            }

            result += "</ul>";

            return result;
        }

        private string GetListHrefString(List<DataAccess.Models.Document.LinkOfDocumentEntityModel> listLinkOfDocument)
        {
            //thêm link đính kèm cho note
            var listString = new List<string>();
            listLinkOfDocument.ForEach(item =>
            {
                // < a href = "../html-link.htm" target = "_blank" > Open page in new window</ a >
                var temp = $"<a href='{item.LinkValue}' target ='_blank'>{item.LinkName}</a>";
                listString.Add(temp);
            });

            return string.Join(",", listString);
        }

        public DownloadTemplateImportCustomerResult DownloadTemplateImportCustomer(DownloadTemplateImportCustomerParameter parameter)
        {
            try
            {
                string rootFolder = hostingEnvironment.WebRootPath + "\\ExcelTemplate";
                string fileName = @"Template_Import_Customers.xls";

                string newFilePath = Path.Combine(rootFolder, fileName);
                byte[] data = File.ReadAllBytes(newFilePath);
                return new DownloadTemplateImportCustomerResult
                {
                    TemplateExcel = data,
                    MessageCode = string.Format("Đã dowload file Template_Import_Customerse"),
                    FileName = "Template_Import_Customers",
                    StatusCode = System.Net.HttpStatusCode.OK
                };
            }
            catch (Exception)
            {
                return new DownloadTemplateImportCustomerResult
                {
                    MessageCode = "Đã có lỗi xảy ra trong quá trình download",
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed
                };
            }
        }

        public SearchContactCustomerResult SearchContactCustomer(SearchContactCustomerParameter parameter)
        {
            try
            {
                var listAllUser = context.User.ToList();
                var listAllEmployee = context.Employee.ToList();
                var listAllContact = context.Contact.ToList();
                var listAllCustomer = context.Customer.Where(x => x.Active == true).ToList();
                List<Guid> listAllCustomerId = new List<Guid>();

                var user = context.User.FirstOrDefault(x => x.UserId == parameter.UserId);
                var employeeId = user.EmployeeId;
                var employee = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeId);
                var isManager = employee.IsManager;


                var positionTypeId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "CVU")
                    ?.CategoryTypeId;
                var listPosition = context.Category.Where(c => c.CategoryTypeId == positionTypeId).ToList();
                var listCommonCus = context.Customer.Where(c => c.Active == true).ToList();
                var statusCustomerType =
                    context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "THA").CategoryTypeId;
                var listStatusCustomer = context.Category.Where(x => x.CategoryTypeId == statusCustomerType).ToList();
                var listContact = new List<ContactEntityModel>();

                if (isManager)
                {
                    //Lấy list phòng ban con của user
                    List<Guid?> listGetAllChild = new List<Guid?>();    //List phòng ban: chính nó và các phòng ban cấp dưới của nó
                    if (employee.OrganizationId != null && isManager)
                    {
                        listGetAllChild.Add(employee.OrganizationId.Value);
                        listGetAllChild = getOrganizationChildrenId(employee.OrganizationId.Value, listGetAllChild);
                    }
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

                    //Nếu là quản lý
                    listAllCustomer = listAllCustomer.Where(x => (x.PersonInChargeId != null && (listEmployeeInChargeByManagerId == null || listEmployeeInChargeByManagerId.Count == 0 || listEmployeeInChargeByManagerId.FirstOrDefault(y => y.Equals(x.PersonInChargeId.Value)) != Guid.Empty)) ||
                                                                 (x.PersonInChargeId == null && (listUserByManagerId == null || listUserByManagerId.Count == 0 || listUserByManagerId.FirstOrDefault(y => y.Equals(x.CreatedById)) != Guid.Empty))
                                                           ).ToList();

                    listAllCustomerId = listAllCustomer.Select(x => x.CustomerId).ToList();
                }
                else
                {

                    var statusCusTypeId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "THA").CategoryTypeId;
                    var listAllStatus = context.Category.Where(c => c.CategoryTypeId == statusCusTypeId).ToList();
                    var MOIStatusId = listAllStatus.FirstOrDefault(f => f.CategoryCode == "MOI").CategoryId;

                    //Nếu không phải quản lý
                    listAllCustomer = listAllCustomer.Where(x => (x.PersonInChargeId != null && x.PersonInChargeId == employeeId) ||
                                                                 (x.EmployeeTakeCareId != null && x.EmployeeTakeCareId == employeeId && x.StatusId == MOIStatusId)).ToList();

                    listAllCustomerId = listAllCustomer.Select(x => x.CustomerId).ToList();
                }

                listContact = context.Contact
                    .Where(c => c.Active == true && c.ObjectType == ObjectType.CUSTOMERCONTACT && listAllCustomerId.Contains(c.ObjectId))
                    .Select(m => new ContactEntityModel
                    {
                        ContactId = m.ContactId,
                        ObjectId = m.ObjectId,
                        ObjectType = m.ObjectType,
                        FirstName = m.FirstName ?? "",
                        LastName = m.LastName ?? "",
                        Email = m.Email,
                        Phone = m.Phone,
                        CustomerPosition = m.CustomerPosition,
                        CustomerName = "",
                        Role = m.Role,
                        CreatedDate = m.CreatedDate,
                        StatusCustomer = ""
                    }).ToList();


                listContact.ForEach(item =>
                {
                    var customer = listCommonCus.FirstOrDefault(c => c.CustomerId == item.ObjectId);

                    if (customer != null)
                    {
                        var statusCode = listStatusCustomer.FirstOrDefault(x => x.CategoryId == customer.StatusId)
                            .CategoryCode;

                        if (statusCode == "HDO")
                        {
                            item.StatusCustomer = "HDO";
                        }
                        else if (statusCode == "MOI")
                        {
                            item.StatusCustomer = "MOI";
                        }
                    }

                    item.CustomerName = customer?.CustomerName;
                    item.FullName = item.FirstName + " " + item.LastName;
                    if (item.CustomerPosition != null && item.CustomerPosition != Guid.Empty)
                    {
                        item.Role = listPosition.FirstOrDefault(c => c.CategoryId == item.CustomerPosition)
                                        ?.CategoryName ?? "";
                    }
                });

                listContact = listContact.OrderByDescending(c => c.CreatedDate).ToList();

                return new SearchContactCustomerResult
                {
                    ListContact = listContact,
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = "Success"
                };
            }
            catch (Exception ex)
            {
                return new SearchContactCustomerResult
                {
                    MessageCode = ex.Message,
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                };
            }
        }

        public CheckDuplicateInforCustomerResult CheckDuplicateInforCustomer(CheckDuplicateInforCustomerParameter parameter)
        {
            try
            {
                bool Valid = true;

                //Nếu check email
                if (parameter.CheckType == 1)
                {
                    if (!string.IsNullOrEmpty(parameter.Email))
                    {
                        //Nếu check khi tạo mới
                        if (parameter.CustomerId == null)
                        {
                            var duplicate = context.Contact.FirstOrDefault(x =>
                                (x.ObjectType == "CUS") &&
                                x.Active == true &&
                                x.Email != null &&
                                x.Email.Trim().ToLower() == parameter.Email.Trim().ToLower());

                            if (duplicate != null)
                            {
                                Valid = false;
                            }
                        }
                        //Nếu check khi ở chi tiết
                        else
                        {
                            var duplicate = context.Contact.FirstOrDefault(x =>
                                x.ObjectId != parameter.CustomerId &&
                                (x.ObjectType == "CUS") &&
                                x.Active == true &&
                                x.Email != null &&
                                x.Email.Trim().ToLower() == parameter.Email.Trim().ToLower());

                            if (duplicate != null)
                            {
                                Valid = false;
                            }
                        }
                    }
                }
                //Nếu check số điện thoại
                else if (parameter.CheckType == 2)
                {
                    if (!string.IsNullOrEmpty(parameter.Phone))
                    {
                        //Nếu check khi tạo mới
                        if (parameter.CustomerId == null)
                        {
                            var duplicate = context.Contact.FirstOrDefault(x =>
                                (x.ObjectType == "CUS") &&
                                x.Active == true &&
                                x.Phone != null &&
                                x.Phone.Trim().ToLower() == parameter.Phone.Trim().ToLower());

                            if (duplicate != null)
                            {
                                Valid = false;
                            }
                        }
                        //Nếu check khi ở chi tiết
                        else
                        {
                            var duplicate = context.Contact.FirstOrDefault(x =>
                                x.ObjectId != parameter.CustomerId &&
                                (x.ObjectType == "CUS") &&
                                x.Active == true &&
                                x.Phone != null &&
                                x.Phone.Trim().ToLower() == parameter.Phone.Trim().ToLower());

                            if (duplicate != null)
                            {
                                Valid = false;
                            }
                        }
                    }
                }

                return new CheckDuplicateInforCustomerResult()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = "Success",
                    Valid = Valid
                };
            }
            catch (Exception e)
            {
                return new CheckDuplicateInforCustomerResult()
                {
                    StatusCode = System.Net.HttpStatusCode.BadRequest,
                    MessageCode = e.Message,
                };
            }
        }

        public ChangeStatusSupportResult ChangeStatusSupport(ChangeStatusSupportParameter parameter)
        {
            try
            {
                var customer = context.Customer.FirstOrDefault(x => x.CustomerId == parameter.CustomerId);

                if (customer == null)
                {
                    return new ChangeStatusSupportResult()
                    {
                        StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                        MessageCode = "Khách hàng không tồn tại trên hệ thống"
                    };
                }

                customer.StatusSuportId = parameter.StatusSupportId;
                context.Customer.Update(customer);
                context.SaveChanges();

                return new ChangeStatusSupportResult()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = "Success",
                };
            }
            catch (Exception e)
            {
                return new ChangeStatusSupportResult()
                {
                    MessageCode = e.Message,
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                };
            }
        }

        //Generate code theo VNS
        private string GenerateVNSCode()
        {
            var result = "";
            var listCustomerCode = context.Customer.Where(x => x.CustomerCode.IndexOf("VNS-") == 0)
                .Select(y => y.CustomerCode).ToList();

            if (listCustomerCode.Count > 0)
            {
                var listPostfix = listCustomerCode.Select(y => y.Substring(4)).ToList();
                var listNumber = new List<Int32>();
                listPostfix.ForEach(item =>
                {
                    var temp = Int32.Parse(item);
                    listNumber.Add(temp);
                });

                var maxCurrent = listNumber.OrderByDescending(z => z).FirstOrDefault();
                var nextNumber = maxCurrent + 1;

                result = "VNS-" + nextNumber.ToString();
                return result;
            }
            else
            {
                result = "VNS-1001";
                return result;
            }
        }

        // Generate code cho lead
        private string GenerateCode()
        {
            var leadTemp = context.Lead.Where(z => z.CreatedDate.Date == DateTime.Now.Date && z.LeadCode != null).OrderByDescending(x => x.CreatedDate).ToList().FirstOrDefault();
            var year = DateTime.Now.Year.ToString().Substring(2);
            var month = DateTime.Now.Month < 10 ? "0" + DateTime.Now.Month.ToString() : DateTime.Now.Month.ToString();
            var day = DateTime.Now.Day < 10 ? "0" + DateTime.Now.Day.ToString() : DateTime.Now.Day.ToString();
            if (leadTemp == null)
            {
                return "LEAD-" + year + month + day + "0001";
            }
            else
            {
                var code = leadTemp.LeadCode?.Substring(leadTemp.LeadCode.Length - 4);
                int temp = Convert.ToInt32(code);
                temp++;
                string identity = "";
                if (temp < 10)
                {
                    identity = "000" + temp;
                }
                else if (temp < 100)
                {
                    identity = "00" + temp;
                }
                else if (temp < 1000)
                {
                    identity = "0" + temp;
                }
                else if (temp < 10000)
                {
                    identity = temp.ToString();
                }
                return "LEAD-" + year + month + day + identity;
            }
        }

        public CreatePotentialCutomerFromWebResult CreatePotentialCutomerFromWeb(CreatePotentialCutomerFromWebParameter parameter)
        {
            var customer = new Customer();
            try
            {
                var tenantId = tenantContext.Tenants.FirstOrDefault(c => c.TenantHost == parameter.TenantHost)?.TenantId;
                var token = tenantContext.SystemParameter.FirstOrDefault(c => c.SystemKey == "TokenId" && tenantId == c.TenantId)?.SystemValueString;
                if (token == parameter.TokenId)
                {
                    using (var transaction = tenantContext.Database.BeginTransaction())
                    {

                        var user = tenantContext.User.FirstOrDefault(c => c.IsAdmin == true && c.TenantId == tenantId);

                        var cusSttId = tenantContext.CategoryType.FirstOrDefault(ct => ct.CategoryTypeCode == "THA" && ct.TenantId == tenantId).CategoryTypeId;
                        var careStateTypeId = tenantContext.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "TTCS" && c.TenantId == tenantId)
                            ?.CategoryTypeId;
                        var notCallId = tenantContext.Category
                            .FirstOrDefault(c => c.CategoryTypeId == careStateTypeId && c.CategoryCode == "CGD" && c.TenantId == tenantId)?.CategoryId;

                        var supportTypeId = tenantContext.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "TTPKHTN" && c.TenantId == tenantId)
                            ?.CategoryTypeId;
                        var newSupportId = tenantContext.Category
                            .FirstOrDefault(c => c.CategoryTypeId == supportTypeId && c.CategoryCode == "A" && c.TenantId == tenantId)?.CategoryId;

                        #region Nguồn tiềm năng
                        var nguonTiemNangId = tenantContext.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "IVF" && c.TenantId == tenantId)?.CategoryTypeId;
                        var websiteId = tenantContext.Category.FirstOrDefault(c => c.CategoryTypeId == nguonTiemNangId && c.CategoryCode == "WEB" && c.TenantId == tenantId)?.CategoryId;
                        #endregion

                        #region Thêm khách hàng tiềm năng
                        var potentialCutomerId = tenantContext.Category.FirstOrDefault(c => c.CategoryTypeId == cusSttId && c.CategoryCode == "MOI" && c.TenantId == tenantId).CategoryId;

                        customer = new Customer
                        {
                            CustomerId = Guid.NewGuid(),
                            StatusSuportId = newSupportId,
                            StatusId = potentialCutomerId,
                            CustomerName = parameter.CustomerName,
                            CreatedDate = DateTime.Now,
                            CustomerType = 1,
                            InvestmentFundId = websiteId,
                            CreatedById = user.UserId,
                            Active = true,
                            TenantId = tenantId
                        };

                        var contact = new Contact
                        {
                            ContactId = Guid.NewGuid(),
                            CreatedDate = DateTime.Now,
                            ObjectId = customer.CustomerId,
                            ObjectType = ObjectType.CUSTOMER,
                            Email = parameter.Email.Trim(),
                            Phone = parameter.Phone?.Trim(),
                            Address = parameter.Address?.Trim(),
                            CreatedById = user.UserId,
                            Note = parameter.Description?.Trim(),
                            Active = true,
                            TenantId = tenantId
                        };

                        if (customer.CustomerType == 1)
                        {
                            contact.FirstName = parameter.CustomerName?.Trim();
                            contact.LastName = "";
                            var con = new Contact
                            {
                                ContactId = Guid.NewGuid(),
                                ObjectId = customer.CustomerId,
                                ObjectType = ObjectType.CUSTOMERCONTACT,
                                FirstName = parameter.Deputy?.Trim(),
                                LastName = "",
                                Phone = parameter.Phone?.Trim(),
                                Email = parameter.Email?.Trim(),
                                Address = parameter.Address?.Trim(),
                                Role = parameter.Role?.Trim(),
                                CreatedDate = DateTime.Now,
                                UpdatedDate = null,
                                CreatedById = user.UserId,
                                UpdatedById = null,
                                TenantId = tenantId
                            };

                            //FirstName, Phone không được để trống
                            bool contact_cus_con_invalid = false;
                            if (string.IsNullOrEmpty(con.FirstName) || string.IsNullOrEmpty(con.Phone))
                            {
                                contact_cus_con_invalid = true;
                            }

                            if (contact_cus_con_invalid)
                            {
                                return new CreatePotentialCutomerFromWebResult()
                                {
                                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                                    MessageCode = "Họ và Tên, Số điện thoại của người liên hệ không được để trống"
                                };
                            };
                            tenantContext.Contact.Add(con);

                            #region Warning duplicate contact customer

                            var customerContactCode = "CUS_CON";
                            var listPhoneCustomerContact = tenantContext.Contact.Where(w => w.ObjectType == customerContactCode && w.TenantId == tenantId)
                                .Select(w => new { Phone = w.Phone }).ToList();
                            var listPhone = new List<string>();

                            listPhoneCustomerContact?.ForEach(a =>
                            {
                                listPhone.Add(a.Phone);
                            });
                            #endregion
                        }

                        if (string.IsNullOrEmpty(customer.CustomerCode))
                        {
                            //Nếu CustomerCode để trống thì auto generate CustomerCode
                            customer.CustomerCode = this.GenerateCustomerCode(0);
                        }

                        #region Check trùng email/sđt
                        if (!string.IsNullOrEmpty(parameter.Email))
                        {
                            var checkEmail = tenantContext.Contact.FirstOrDefault(x =>
                                x.ObjectType == "CUS" &&
                                (x.Email ?? "").Trim().ToLower() == parameter.Email.Trim().ToLower() &&
                                x.TenantId == tenantId);

                            if (checkEmail != null)
                            {
                                return new CreatePotentialCutomerFromWebResult()
                                {
                                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                                    MessageCode = "Email khách hàng đã tồn tại trên hệ thống"
                                };
                            }
                        }

                        if (!string.IsNullOrEmpty(parameter.Phone))
                        {
                            var checkPhone = tenantContext.Contact.FirstOrDefault(x =>
                                x.ObjectType == "CUS" &&
                                (x.Phone ?? "").Trim().ToLower() == parameter.Phone.Trim().ToLower() &&
                                x.TenantId == tenantId);

                            if (checkPhone != null)
                            {
                                return new CreatePotentialCutomerFromWebResult()
                                {
                                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                                    MessageCode = "Số điện thoại khách hàng đã tồn tại trên hệ thống"
                                };
                            }
                        }
                        #endregion

                        tenantContext.Customer.Add(customer);
                        tenantContext.Contact.Add(contact);
                        #endregion

                        var listProduct = tenantContext.Product.ToList();
                        // Thêm sản phẩm dịch vụ
                        var listNewProduct = new List<Entities.PotentialCustomerProduct>();
                        parameter.ListProductCode?.ForEach(item =>
                        {
                            var product = listProduct.FirstOrDefault(c => c.ProductCode == item);
                            listNewProduct.Add(new PotentialCustomerProduct
                            {
                                PotentialCustomerProductId = Guid.NewGuid(),
                                ProductId = product.ProductId,
                                CustomerId = customer.CustomerId,
                                IsInTheSystem = false,
                                ProductName = product.ProductName,
                                ProductUnit = "",
                                ProductFixedPrice = product.Price1,
                                ProductUnitPrice = product.Price1,
                                ProductNote = "",
                                Active = true,
                                CreatedById = user.UserId,
                                CreatedDate = DateTime.Now,
                                TenantId = tenantId
                            });
                        });
                        tenantContext.PotentialCustomerProduct.AddRange(listNewProduct);
                        tenantContext.SaveChanges();
                        transaction.Commit();
                    }
                }
                else
                {
                    return new CreatePotentialCutomerFromWebResult
                    {
                        MessageCode = "error",
                        StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    };
                }
            }
            catch (Exception ex)
            {
                return new CreatePotentialCutomerFromWebResult
                {
                    MessageCode = ex.Message,
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                };
            }


            #region Log

            LogHelper.AuditTrace(context, "Create", "POTENTIAL_CUSTOMER", customer.CustomerId, customer.CreatedById);

            #endregion

            #region Gửi thông báo

            NotificationHelper.AccessNotification(context, TypeModel.PotentialCustomer, "CRE", new Customer(),
                customer, true);

            #endregion

            return new CreatePotentialCutomerFromWebResult
            {
                MessageCode = "Success",
                StatusCode = System.Net.HttpStatusCode.OK,
            };
        }

        public KichHoatTinhHuongResult KichHoatTinhHuong(KichHoatTinhHuongParameter parameter)
        {
            var listPhone = parameter.ListPhone.Split(";");
            if (listPhone.Length == 0)
            {
                return new KichHoatTinhHuongResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = "Thành viên tổ khẩn nguy không được để trống"
                };
            }

            //Tạo data
            var tinhHuong = new KichHoatTinhHuong();
            tinhHuong.Id = Guid.NewGuid();
            tinhHuong.NoiDung = parameter.NoiDung;
            tinhHuong.SoLuongNguoi = listPhone.Length;
            tinhHuong.ThoiDiemKichHoat = DateTime.Now;
            context.KichHoatTinhHuong.Add(tinhHuong);

            var listKichHoatTinhHuongChiTiet = new List<KichHoatTinhHuongChiTiet>();

            foreach (var phone in listPhone)
            {
                var _item = new KichHoatTinhHuongChiTiet();
                _item.Id = Guid.NewGuid();
                _item.KichHoatTinhHuongId = tinhHuong.Id;
                _item.Sdt = phone;
                _item.NoiDung = parameter.NoiDung;

                var item = new KichHoatRequestModel();
                item.campaign_id = 706;
                item.phone_number = phone;
                item.content = parameter.NoiDung;
                item.tts_voice = "banmai";

                var res = SendRequestHelper.KichHoatTinhHuong(item);
                if (res.error == 0)
                {
                    _item.Session = res.message.session;
                    Thread.Sleep(1000);
                }

                listKichHoatTinhHuongChiTiet.Add(_item);
            }

            //for (int i = 0; i < listPhone.Length; i++)
            //{
            //    var _item = new KichHoatTinhHuongChiTiet();
            //    _item.Id = Guid.NewGuid();
            //    _item.KichHoatTinhHuongId = tinhHuong.Id;
            //    _item.Sdt = listPhone[i];
            //    _item.NoiDung = parameter.NoiDung;

            //    var item = new KichHoatRequestModel();
            //    item.campaign_id = 706;
            //    item.phone_number = listPhone[i];
            //    item.content = parameter.NoiDung;
            //    item.tts_voice = "banmai";

            //    var res = SendRequestHelper.KichHoatTinhHuong(item);
            //    if (res.error == 0)
            //    {
            //        _item.Session = res.message.session;
            //    }

            //    listKichHoatTinhHuongChiTiet.Add(_item);
            //}

            context.KichHoatTinhHuongChiTiet.AddRange(listKichHoatTinhHuongChiTiet);
            context.SaveChanges();

            return new KichHoatTinhHuongResult()
            {
                StatusCode = HttpStatusCode.OK,
                MessageCode = "Kích hoạt thành công"
            };
        }

        public GetListTinhHuongResult GetListTinhHuong(GetListTinhHuongParameter parameter)
        {
            try
            {
                var listData = context.KichHoatTinhHuong.Select(y => new KichHoatTinhHuongModel
                {
                    Id = y.Id,
                    NoiDung = y.NoiDung,
                    ThoiDiemKichHoat = y.ThoiDiemKichHoat,
                    SoLuongNguoi = y.SoLuongNguoi
                }).OrderByDescending(z => z.ThoiDiemKichHoat).ToList();

                return new GetListTinhHuongResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success",
                    ListData = listData
                };
            }
            catch (Exception e)
            {
                return new GetListTinhHuongResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public async Task<GetChiTietTinhHuongResult> GetChiTietTinhHuong(GetChiTietTinhHuongParameter parameter)
        {
            var listKichHoatTinhHuongChiTiet = context.KichHoatTinhHuongChiTiet
                .Where(x => x.KichHoatTinhHuongId == parameter.Id)
                .ToList();

            for (int i = 0; i < listKichHoatTinhHuongChiTiet.Count; i++)
            {
                var item = listKichHoatTinhHuongChiTiet[i];

                item.PhanHoi = SendRequestHelper.GetKichHoatTinhHuongResult(item.Session);
            }

            context.KichHoatTinhHuongChiTiet.UpdateRange(listKichHoatTinhHuongChiTiet);
            context.SaveChanges();

            var listDataDetail = listKichHoatTinhHuongChiTiet.Select(y => new KichHoatTinhHuongChiTietModel
            {
                Id = y.Id,
                KichHoatTinhHuongId = y.KichHoatTinhHuongId,
                HoVaTen = y.HoVaTen,
                NoiDung = y.NoiDung,
                Sdt = y.Sdt,
                PhanHoi = y.PhanHoi,
            }).ToList();

            return new GetChiTietTinhHuongResult()
            {
                StatusCode = HttpStatusCode.OK,
                MessageCode = "Success",
                ListDataDetail = listDataDetail
            };
        }
        private static readonly string[] VietNamChar = new string[]
       {
            "aAeEoOuUiIdDyY",
            "áàạảãâấầậẩẫăắằặẳẵ",
            "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",
            "éèẹẻẽêếềệểễ",
            "ÉÈẸẺẼÊẾỀỆỂỄ",
            "óòọỏõôốồộổỗơớờợởỡ",
            "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",
            "úùụủũưứừựửữ",
            "ÚÙỤỦŨƯỨỪỰỬỮ",
            "íìịỉĩ",
            "ÍÌỊỈĨ",
            "đ",
            "Đ",
            "ýỳỵỷỹ",
            "ÝỲỴỶỸ"
       };

        public static string LocDau(string str)
        {
            //Thay thế và lọc dấu từng char      
            for (int i = 1; i < VietNamChar.Length; i++)
            {
                for (int j = 0; j < VietNamChar[i].Length; j++)
                    str = str.Replace(VietNamChar[i][j], VietNamChar[0][i - 1]);
            }
            return str;
        }
    }

    public class ComparePotentialCustomerModel
    {
        public string Phone { get; set; } //số điện thoại
        public string WorkPhone { get; set; } //số điện thoại cơ quan
        public string OtherPhone { get; set; } //số điện thoại khác
        public string Email { get; set; } //email cá nhân
        public string WorkEmail { get; set; } //email cơ quan
        public Category InvestmentFund { get; set; }
        public Employee PersonInCharge { get; set; }
    }

    public class CompareCustomerModel
    {
        public Employee PersonInCharge { get; set; }
        public Employee Employee { get; set; }
    }
}
