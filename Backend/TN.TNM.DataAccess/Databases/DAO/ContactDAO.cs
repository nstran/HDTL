using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using TN.TNM.Common;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.Contact;
using TN.TNM.DataAccess.Messages.Results.Contact;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Address;
using TN.TNM.DataAccess.Models.Customer;
using TN.TNM.DataAccess.Models.Lead;

namespace TN.TNM.DataAccess.Databases.DAO
{
    public class ContactDAO : BaseDAO, IContactDataAccess
    {
        public ContactDAO(Databases.TNTN8Context _content, IAuditTraceDataAccess _iAuditTrace)
        {
            this.context = _content;
            this.iAuditTrace = _iAuditTrace;
        }

        public CreateContactResult CreateContact(CreateContactParameter parameter)
        {
            try
            {
                if (parameter.Contact.ContactId == null)
                {
                    var cusContact = new Contact();

                    cusContact.ContactId = Guid.NewGuid();
                    cusContact.ObjectId = parameter.Contact.ObjectId;
                    cusContact.ObjectType = parameter.Contact.ObjectType;
                    cusContact.FirstName = parameter.Contact.FirstName;
                    cusContact.LastName = parameter.Contact.LastName;
                    cusContact.Gender = parameter.Contact.Gender;
                    cusContact.Email = parameter.Contact.Email;
                    cusContact.Phone = parameter.Contact.Phone;
                    cusContact.Role = parameter.Contact.Role;
                    cusContact.Other = parameter.Contact.Other;
                    cusContact.DateOfBirth = parameter.Contact.DateOfBirth;
                    cusContact.Address = parameter.Contact.Address;
                    cusContact.ProvinceId = parameter.Contact.ProvinceId;
                    cusContact.DistrictId = parameter.Contact.DistrictId;
                    cusContact.WardId = parameter.Contact.WardId;
                    cusContact.CreatedById = parameter.UserId;
                    cusContact.CreatedDate = DateTime.Now;

                    context.Contact.Add(cusContact);
                    context.SaveChanges();
                }
                else
                {
                    var cusContact = context.Contact.FirstOrDefault(x => x.ContactId == parameter.Contact.ContactId);

                    cusContact.FirstName = parameter.Contact.FirstName;
                    cusContact.LastName = parameter.Contact.LastName;
                    cusContact.Gender = parameter.Contact.Gender;
                    cusContact.Phone = parameter.Contact.Phone;
                    cusContact.Email = parameter.Contact.Email;
                    cusContact.Role = parameter.Contact.Role;
                    cusContact.Other = parameter.Contact.Other;
                    cusContact.DateOfBirth = parameter.Contact.DateOfBirth;
                    cusContact.Address = parameter.Contact.Address;
                    cusContact.ProvinceId = parameter.Contact.ProvinceId;
                    cusContact.DistrictId = parameter.Contact.DistrictId;
                    cusContact.WardId = parameter.Contact.WardId;
                    cusContact.UpdatedById = parameter.UserId;
                    cusContact.UpdatedDate = DateTime.Now;

                    context.Contact.Update(cusContact);
                    context.SaveChanges();
                }

                var listContact = new List<CustomerOtherContactModel>();
                listContact = context.Contact
                    .Where(x => x.ObjectId == parameter.Contact.ObjectId && x.ObjectType == parameter.Contact.ObjectType)
                    .Select(y => new CustomerOtherContactModel
                    {
                        ContactId = y.ContactId,
                        ObjectId = y.ObjectId,
                        ObjectType = y.ObjectType,
                        FirstName = y.FirstName,
                        LastName = y.LastName,
                        ContactName = "",
                        Gender = y.Gender,
                        GenderName = y.Gender == "NAM" ? "Nam" : (y.Gender == "NU" ? "Nữ" : "Khác"),
                        Phone = y.Phone == null ? null : y.Phone.Trim(),
                        Email = y.Email == null ? null : y.Email.Trim(),
                        Role = y.Role == null ? null : y.Role.Trim(),
                        Other = y.Other == null ? null : y.Other.Trim(),
                        DateOfBirth = y.DateOfBirth,
                        Address = y.Address,
                        ProvinceId = y.ProvinceId,
                        DistrictId = y.DistrictId,
                        WardId = y.WardId,
                        CreatedDate = y.CreatedDate
                    }).OrderByDescending(z => z.CreatedDate).ToList();

                listContact.ForEach(item =>
                {
                    var firstName = item.FirstName == null ? "" : item.FirstName.Trim();
                    var lastName = item.LastName == null ? "" : item.LastName.Trim();
                    item.ContactName = (firstName + " " + lastName).Trim();
                });

                return new CreateContactResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = CommonMessage.Contact.CREATE_SUCCESS,
                    ListContact = listContact
                };
            }
            catch (Exception e)
            {
                return new CreateContactResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetAllContactByObjectTypeResult GetAllContactByObjectType(GetAllContactByObjectTypeParameter parameter)
        {
            try
            {
                var contactList = context.Contact.Where(c => c.ObjectType == parameter.ObjectType).ToList();
                var listContactEntityModel = new List<ContactEntityModel>();
                contactList.ForEach(item =>
                {
                    listContactEntityModel.Add(new ContactEntityModel(item));
                });
                return new GetAllContactByObjectTypeResult
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success",
                    ContactList = listContactEntityModel
                };
            }
            catch (Exception e)
            {
                return new GetAllContactByObjectTypeResult
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }

        }

        public SearchContactResult SearchContact(SearchContactParameter parameter)
        {
            try
            {
                var contactList = (from c in context.Contact
                                   join l in context.Lead
                                   on c.ObjectId equals l.LeadId
                                   join c1 in context.Contact on l.PersonInChargeId equals c1.ObjectId into gj
                                   from x in gj.DefaultIfEmpty()
                                   where (c.FirstName == parameter.FirstName || c.FirstName.Contains(parameter.FirstName) || parameter.FirstName == "" || parameter.FirstName == null) &&
                                         (c.LastName == parameter.LastName || c.LastName.Contains(parameter.LastName) || parameter.LastName == "" || parameter.LastName == null) &&
                                         (c.Email == parameter.Email || c.Email.Contains(parameter.Email) || parameter.Email == "" || parameter.Email == null) &&
                                         (c.Phone == parameter.Phone || c.Phone.Contains(parameter.Phone) || parameter.Phone == "" || parameter.Phone == null) &&
                                         (l.InterestedGroupId == parameter.InterestedGroupId || parameter.InterestedGroupId == null) &&
                                         (l.PersonInChargeId == parameter.PersonInChargeId || parameter.PersonInChargeId == null) &&
                                         (l.PotentialId == parameter.PotentialId || parameter.PotentialId == null) &&
                                         (l.StatusId == parameter.StatusId || parameter.StatusId == null)
                                   select new LeadEntityModel
                                   {
                                       CompanyId = l.CompanyId,
                                       FullName = c.FirstName + c.LastName,
                                       PersonInChargeFullName = x.ObjectType == ObjectType.EMPLOYEE ? x.FirstName + x.LastName : "",
                                       Email = c.Email,
                                       Phone = c.Phone
                                   }).ToList();
                return new SearchContactResult
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success",
                    ContactList = contactList
                };
            }
            catch (Exception e)
            {
                return new SearchContactResult
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }

        }

        public GetContactByIdResult GetContactById(GetContactByIdParameter parameter)
        {
            try
            {
                var contact = context.Contact.FirstOrDefault(c => c.ContactId == parameter.ContactId);
                var province = new Entities.Province();
                var district = new Entities.District();
                var ward = new Entities.Ward();

                if (contact != null)
                {
                    province = context.Province.FirstOrDefault(p => p.ProvinceId == contact.ProvinceId);
                    district = context.District.FirstOrDefault(d => d.DistrictId == contact.DistrictId);
                    ward = context.Ward.FirstOrDefault(w => w.WardId == contact.WardId);
                }

                return new GetContactByIdResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode="Success",
                    Contact = new ContactEntityModel(contact),
                    FullAddress = ward?.WardType + " " + ward?.WardName + ", " + district?.DistrictType + " " + district?.DistrictName + ", " + province?.ProvinceType + " " + province?.ProvinceName
                };
            }
            catch (Exception e)
            {
                return new GetContactByIdResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetContactByIdResult GetContactByObjectId(GetContactByObjectIdParameter parameter)
        {
            try
            {
                var contact = context.Contact.FirstOrDefault(c => c.ObjectId == parameter.ObjectId);
                var province = new Entities.Province();
                var district = new Entities.District();
                var ward = new Entities.Ward();

                if (contact != null)
                {
                    province = context.Province.FirstOrDefault(p => p.ProvinceId == contact.ProvinceId);
                    district = context.District.FirstOrDefault(d => d.DistrictId == contact.DistrictId);
                    ward = context.Ward.FirstOrDefault(w => w.WardId == contact.WardId);
                }

                return new GetContactByIdResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode="Success",
                    Contact = new ContactEntityModel(contact),
                    FullAddress = ward?.WardType + " " + ward?.WardName + ", " + district?.DistrictType + " " + district?.DistrictName + ", " + province?.ProvinceType + " " + province?.ProvinceName
                };
            }
            catch (Exception e)
            {
                return new GetContactByIdResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode=e.Message
                };
            }
        }

        public DeleteContactByIdResult DeleteContactById(DeleteContactByIdParameter parameter)
        {
            try
            {
                var contact = context.Contact.FirstOrDefault(c => c.ContactId == parameter.ContactId);
                context.Contact.Remove(contact);
                context.SaveChanges();

                var listContact = new List<CustomerOtherContactModel>();
                listContact = context.Contact
                    .Where(x => x.ObjectId == parameter.ObjectId && x.ObjectType == parameter.ObjectType)
                    .Select(y => new CustomerOtherContactModel
                    {
                        ContactId = y.ContactId,
                        ObjectId = y.ObjectId,
                        ObjectType = y.ObjectType,
                        FirstName = y.FirstName,
                        LastName = y.LastName,
                        ContactName = "",
                        Gender = y.Gender,
                        GenderName = y.Gender == "NAM" ? "Nam" : (y.Gender == "NU" ? "Nữ" : "Khác"),
                        Phone = y.Phone == null ? null : y.Phone.Trim(),
                        Email = y.Email == null ? null : y.Email.Trim(),
                        Role = y.Role == null ? null : y.Role.Trim(),
                        Other = y.Other == null ? null : y.Other.Trim(),
                        CreatedDate = y.CreatedDate,
                        ProvinceId = y.ProvinceId,
                        DistrictId = y.DistrictId,
                        WardId = y.WardId
                    }).OrderByDescending(z => z.CreatedDate).ToList();

                listContact.ForEach(item =>
                {
                    var firstName = item.FirstName == null ? "" : item.FirstName.Trim();
                    var lastName = item.LastName == null ? "" : item.LastName.Trim();
                    item.ContactName = (firstName + " " + lastName).Trim();
                });

                return new DeleteContactByIdResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = CommonMessage.Contact.DELETE_SUCCESS,
                    ListContact = listContact
                };
            }
            catch (Exception e)
            {
                return new DeleteContactByIdResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public EditContactByIdResult EditContactById(EditContactByIdParameters parameter)
        {
            try
            {
                parameter.Contact.UpdatedById = parameter.UserId;
                parameter.Contact.UpdatedDate = DateTime.Now;
                context.Contact.Update(parameter.Contact);
                context.SaveChanges();

                return new EditContactByIdResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = CommonMessage.Contact.EDIT_SUCCESS
                };
            }
            catch(Exception e)
            {
                return new EditContactByIdResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }           
        }

        public UpdatePersonalCustomerContactResult UpdatePersonalCustomerContact(UpdatePersonalCustomerContactParameter parameter)
        {
            try
            {
                var contact = context.Contact.FirstOrDefault(x => x.ContactId == parameter.Contact.ContactId);

                #region Kiểm tra trùng email, phone

                //var email = parameter.Contact.Email;
                //var workEmail = parameter.Contact.WorkEmail;
                //var otherEmail = parameter.Contact.OtherEmail;

                //var phone = parameter.Contact.Phone;
                //var workPhone = parameter.Contact.WorkPhone;
                //var otherPhone = parameter.Contact.OtherPhone;

                //if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(workEmail))
                //{
                //    if (email == workEmail)
                //    {
                //        return new UpdatePersonalCustomerContactResult()
                //        {
                //            Status = false,
                //            Message = "Email cá nhân đang trùng với Email công việc"
                //        };
                //    }
                //}

                //if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(otherEmail))
                //{
                //    if (email == otherEmail)
                //    {
                //        return new UpdatePersonalCustomerContactResult()
                //        {
                //            Status = false,
                //            Message = "Email cá nhân đang trùng với Email khác"
                //        };
                //    }
                //}

                //if (!string.IsNullOrEmpty(workEmail) && !string.IsNullOrEmpty(otherEmail))
                //{
                //    if (workEmail == otherEmail)
                //    {
                //        return new UpdatePersonalCustomerContactResult()
                //        {
                //            Status = false,
                //            Message = "Email công việc đang trùng với Email khác"
                //        };
                //    }
                //}

                //if (!string.IsNullOrEmpty(email))
                //{
                //    var checkEmail = context.Contact.FirstOrDefault(x =>
                //        (x.ObjectType == "CUS" || x.ObjectType == "CUS_CON") && x.ObjectId != contact.ObjectId &&
                //        ((x.Email ?? "").Trim().ToLower() == email.Trim().ToLower() ||
                //         (x.WorkEmail ?? "").Trim().ToLower() == email.Trim().ToLower() ||
                //         (x.OtherEmail ?? "").Trim().ToLower() == email.Trim().ToLower()));

                //    if (checkEmail != null)
                //    {
                //        return new UpdatePersonalCustomerContactResult()
                //        {
                //            Status = false,
                //            Message = "Email cá nhân đang trùng với Email của khách hàng khác trong hệ thống"
                //        };
                //    }
                //}

                //if (!string.IsNullOrEmpty(workEmail))
                //{
                //    var checkWorkEmail = context.Contact.FirstOrDefault(x =>
                //        (x.ObjectType == "CUS" || x.ObjectType == "CUS_CON") && x.ObjectId != contact.ObjectId &&
                //        ((x.Email ?? "").Trim().ToLower() == workEmail.Trim().ToLower() ||
                //         (x.WorkEmail ?? "").Trim().ToLower() == workEmail.Trim().ToLower() ||
                //         (x.OtherEmail ?? "").Trim().ToLower() == workEmail.Trim().ToLower()));

                //    if (checkWorkEmail != null)
                //    {
                //        return new UpdatePersonalCustomerContactResult()
                //        {
                //            Status = false,
                //            Message = "Email công việc đang trùng với Email của khách hàng khác trong hệ thống"
                //        };
                //    }
                //}

                //if (!string.IsNullOrEmpty(otherEmail))
                //{
                //    var checkOtherEmail = context.Contact.FirstOrDefault(x =>
                //        (x.ObjectType == "CUS" || x.ObjectType == "CUS_CON") && x.ObjectId != contact.ObjectId &&
                //        ((x.Email ?? "").Trim().ToLower() == otherEmail.Trim().ToLower() ||
                //         (x.WorkEmail ?? "").Trim().ToLower() == otherEmail.Trim().ToLower() ||
                //         (x.OtherEmail ?? "").Trim().ToLower() == otherEmail.Trim().ToLower()));

                //    if (checkOtherEmail != null)
                //    {
                //        return new UpdatePersonalCustomerContactResult()
                //        {
                //            Status = false,
                //            Message = "Email khác đang trùng với Email của khách hàng khác trong hệ thống"
                //        };
                //    }
                //}

                //if (!string.IsNullOrEmpty(phone) && !string.IsNullOrEmpty(workPhone))
                //{
                //    if (phone == workPhone)
                //    {
                //        return new UpdatePersonalCustomerContactResult()
                //        {
                //            Status = false,
                //            Message = "Số điện thoại cá nhân đang trùng với Số điện thoại công việc"
                //        };
                //    }
                //}

                //if (!string.IsNullOrEmpty(phone) && !string.IsNullOrEmpty(otherPhone))
                //{
                //    if (phone == otherPhone)
                //    {
                //        return new UpdatePersonalCustomerContactResult()
                //        {
                //            Status = false,
                //            Message = "Số điện thoại cá nhân đang trùng với Số điện thoại khác"
                //        };
                //    }
                //}

                //if (!string.IsNullOrEmpty(workPhone) && !string.IsNullOrEmpty(otherPhone))
                //{
                //    if (workPhone == otherPhone)
                //    {
                //        return new UpdatePersonalCustomerContactResult()
                //        {
                //            Status = false,
                //            Message = "Số điện thoại công việc đang trùng với Số điện thoại khác"
                //        };
                //    }
                //}

                //if (!string.IsNullOrEmpty(phone))
                //{
                //    var checkPhone = context.Contact.FirstOrDefault(x =>
                //        (x.ObjectType == "CUS" || x.ObjectType == "CUS_CON") && x.ObjectId != contact.ObjectId &&
                //        ((x.Phone ?? "").Trim().ToLower() == phone.Trim().ToLower() ||
                //         (x.WorkPhone ?? "").Trim().ToLower() == phone.Trim().ToLower() ||
                //         (x.OtherPhone ?? "").Trim().ToLower() == phone.Trim().ToLower()));

                //    if (checkPhone != null)
                //    {
                //        return new UpdatePersonalCustomerContactResult()
                //        {
                //            Status = false,
                //            Message = "Số điện thoại cá nhân đang trùng với Số điện thoại của khách hàng khác trong hệ thống"
                //        };
                //    }
                //}

                //if (!string.IsNullOrEmpty(workPhone))
                //{
                //    var checkWorkPhone = context.Contact.FirstOrDefault(x =>
                //        (x.ObjectType == "CUS" || x.ObjectType == "CUS_CON") && x.ObjectId != contact.ObjectId &&
                //        ((x.Phone ?? "").Trim().ToLower() == workPhone.Trim().ToLower() ||
                //         (x.WorkPhone ?? "").Trim().ToLower() == workPhone.Trim().ToLower() ||
                //         (x.OtherPhone ?? "").Trim().ToLower() == workPhone.Trim().ToLower()));

                //    if (checkWorkPhone != null)
                //    {
                //        return new UpdatePersonalCustomerContactResult()
                //        {
                //            Status = false,
                //            Message = "Số điện thoại công việc đang trùng với Số điện thoại của khách hàng khác trong hệ thống"
                //        };
                //    }
                //}

                //if (!string.IsNullOrEmpty(otherPhone))
                //{
                //    var checkOtherPhone = context.Contact.FirstOrDefault(x =>
                //        (x.ObjectType == "CUS" || x.ObjectType == "CUS_CON") && x.ObjectId != contact.ObjectId &&
                //        ((x.Phone ?? "").Trim().ToLower() == otherPhone.Trim().ToLower() ||
                //         (x.WorkPhone ?? "").Trim().ToLower() == otherPhone.Trim().ToLower() ||
                //         (x.OtherPhone ?? "").Trim().ToLower() == otherPhone.Trim().ToLower()));

                //    if (checkOtherPhone != null)
                //    {
                //        return new UpdatePersonalCustomerContactResult()
                //        {
                //            Status = false,
                //            Message = "Số điện thoại khác đang trùng với Số điện thoại của khách hàng khác trong hệ thống"
                //        };
                //    }
                //}

                #endregion

                contact.Email = parameter.Contact.Email;
                contact.WorkEmail = parameter.Contact.WorkEmail;
                contact.OtherEmail = parameter.Contact.OtherEmail;
                contact.Phone = parameter.Contact.Phone;
                contact.WorkPhone = parameter.Contact.WorkPhone;
                contact.OtherPhone = parameter.Contact.OtherPhone;
                contact.GeographicalAreaId = parameter.Contact.AreaId;
                contact.ProvinceId = parameter.Contact.ProvinceId;
                contact.DistrictId = parameter.Contact.DistrictId;
                contact.WardId = parameter.Contact.WardId;
                contact.Address = parameter.Contact.Address;
                contact.Other = parameter.Contact.Other;
                contact.Longitude = parameter.Contact.Longitude;
                contact.Latitude = parameter.Contact.Latitude;

                context.Contact.Update(contact);
                context.SaveChanges();

                return new UpdatePersonalCustomerContactResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success"
                };
            }
            catch (Exception e)
            {
                return new UpdatePersonalCustomerContactResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetAddressByObjectResult GetAddressByObject(GetAddressByObjectParameter parameter)
        {
            try
            {
                var contact = context.Contact.FirstOrDefault(x =>
                    x.ObjectId == parameter.ObjectId && x.ObjectType == parameter.ObjectType);
                var address = contact == null
                    ? ""
                    : (contact.Address == null ? "" : (contact.Address.Trim()));

                var province = "";
                var district = "";
                var ward = "";

                if (contact != null)
                {
                    var wardId = contact.WardId;
                    if (wardId != null && wardId != Guid.Empty)
                    {
                        var _ward = context.Ward.FirstOrDefault(x => x.WardId == wardId);
                        ward = _ward == null
                            ? ""
                            : (_ward.WardName == null ? "" : (_ward.WardType.Trim() + " " + _ward.WardName.Trim()));
                    }

                    if (!string.IsNullOrEmpty(ward)) address = address == "" ? ward : address + ", " + ward;

                    var districtId = contact.DistrictId;
                    if (districtId != null && districtId != Guid.Empty)
                    {
                        var _district = context.District.FirstOrDefault(x => x.DistrictId == districtId);
                        district = _district == null
                            ? ""
                            : (_district.DistrictName == null
                                ? ""
                                : (_district.DistrictType.Trim() + " " + _district.DistrictName.Trim()));
                    }
                    if (!string.IsNullOrEmpty(district)) address = address == "" ? district : address + ", " + district;

                    var provinceId = contact.ProvinceId;
                    if (provinceId != null && provinceId != Guid.Empty)
                    {
                        var _province = context.Province.FirstOrDefault(x => x.ProvinceId == provinceId);
                        province = _province == null
                            ? ""
                            : (_province.ProvinceName == null
                                ? ""
                                : (_province.ProvinceType.Trim() + " " + _province.ProvinceName.Trim()));
                    }
                    if (!string.IsNullOrEmpty(province)) address = address == "" ? province : address + ", " + province;
                }

                return new GetAddressByObjectResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success",
                    Address = address
                };
            }
            catch (Exception e)
            {
                return new GetAddressByObjectResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message,
                };
            }
        }

        public GetAddressByContactIdResult GetAddressByContactId(GetAddressByContactIdParameter parameter)
        {
            try
            {
                var ListProvince = new List<ProvinceEntityModel>();
                var ListDistrict = new List<DistrictEntityModel>();
                var ListWard = new List<WardEntityModel>();

                ListProvince = context.Province.Select(y => new ProvinceEntityModel
                {
                    ProvinceId = y.ProvinceId,
                    ProvinceName = y.ProvinceName
                }).ToList();

                //Nếu có contact
                if (parameter.ContactId != null)
                {
                    var contact = context.Contact.FirstOrDefault(x => x.ContactId == parameter.ContactId);

                    if (contact.ProvinceId != null && contact.ProvinceId != Guid.Empty)
                    {
                        ListDistrict = context.District.Where(x => x.ProvinceId == contact.ProvinceId).Select(y =>
                            new DistrictEntityModel
                            {
                                DistrictId = y.DistrictId,
                                DistrictName = y.DistrictName,
                                ProvinceId = y.ProvinceId
                            }).ToList();

                        if (contact.DistrictId != null && contact.DistrictId != Guid.Empty)
                        {
                            ListWard = context.Ward.Where(x => x.DistrictId == contact.DistrictId).Select(y =>
                                new WardEntityModel
                                {
                                    WardId = y.WardId,
                                    WardName = y.WardName,
                                    DistrictId = y.DistrictId
                                }).ToList();
                        }
                    }
                }

                return new GetAddressByContactIdResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success",
                    ListProvince = ListProvince,
                    ListDistrict = ListDistrict,
                    ListWard = ListWard
                };
            }
            catch (Exception e)
            {
                return new GetAddressByContactIdResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetAddressByChangeObjectResult GetAddressByChangeObject(GetAddressByChangeObjectParameter parameter)
        {
            try
            {
                var ListDistrict = new List<DistrictEntityModel>();
                var ListWard = new List<WardEntityModel>();

                //Lấy Huyện
                if (parameter.ObjectType == 1)
                {
                    ListDistrict = context.District.Where(x => x.ProvinceId == parameter.ObjectId).Select(y =>
                        new DistrictEntityModel
                        {
                            DistrictId = y.DistrictId,
                            DistrictName = y.DistrictName,
                            ProvinceId = y.ProvinceId
                        }).ToList();
                }
                //Lấy Phường
                else if (parameter.ObjectType == 2)
                {
                    ListWard = context.Ward.Where(x => x.DistrictId == parameter.ObjectId).Select(y =>
                        new WardEntityModel
                        {
                            WardId = y.WardId,
                            WardName = y.WardName,
                            DistrictId = y.DistrictId
                        }).ToList();
                }

                return new GetAddressByChangeObjectResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success",
                    ListDistrict = ListDistrict,
                    ListWard = ListWard
                };
            }
            catch (Exception e)
            {
                return new GetAddressByChangeObjectResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }
    }
}
