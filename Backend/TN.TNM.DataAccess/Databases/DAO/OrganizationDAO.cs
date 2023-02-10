using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using TN.TNM.Common;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.Admin.Organization;
using TN.TNM.DataAccess.Messages.Results.Admin.Organization;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Address;
using TN.TNM.DataAccess.Models.GeographicalArea;
using TN.TNM.DataAccess.Models.Satellite;

namespace TN.TNM.DataAccess.Databases.DAO
{
    public class OrganizationDAO : BaseDAO, IOrganizationDataAccess
    {
        public OrganizationDAO(Databases.TNTN8Context _content, IAuditTraceDataAccess _iAuditTrace)
        {
            this.context = _content;
            this.iAuditTrace = _iAuditTrace;
        }

        public GetAllOrganizationResult GetAllOrganization(GetAllOrganizationParameter parameter)
        {
            try
            {
                var list = context.Organization.Select(o => new OrganizationEntityModel()
                {
                    OrganizationId = o.OrganizationId,
                    OrganizationName = o.OrganizationName,
                    Level = o.Level,
                    ParentId = o.ParentId,
                    IsFinancialIndependence = o.IsFinancialIndependence
                }).ToList();

                List<OrganizationEntityModel> recoreds = list.Where(l => l.ParentId == null).Select(l => new OrganizationEntityModel()
                {
                    OrganizationId = l.OrganizationId,
                    OrganizationName = l.OrganizationName,
                    Level = l.Level,
                    ParentId = l.ParentId,
                    IsFinancialIndependence = l.IsFinancialIndependence,
                    OrgChildList = GetChildren(l.OrganizationId, list)
                }).OrderBy(l => l.OrganizationName).ToList();

                var listAll = new List<OrganizationEntityModel>();
                listAll = context.Organization.Select(y => new OrganizationEntityModel
                {
                    OrganizationId = y.OrganizationId,
                    OrganizationName = y.OrganizationName,
                    Level = y.Level,
                    ParentId = y.ParentId,
                    IsFinancialIndependence = y.IsFinancialIndependence
                }).ToList();

                var listGeographicalArea = new List<GeographicalAreaEntityModel>();
                var listProvince = new List<ProvinceEntityModel>();
                var listDistrict = new List<DistrictEntityModel>();
                var listWard = new List<WardEntityModel>();
                var listSatellite = new List<SatelliteEntityModel>();

                return new GetAllOrganizationResult
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success",
                    OrganizationList = recoreds,
                    ListAll = listAll,
                    ListGeographicalArea = listGeographicalArea,
                    ListProvince = listProvince,
                    ListDistrict = listDistrict,
                    ListWard = listWard,
                    ListSatellite = listSatellite
                };
            }
            catch (Exception e)
            {
                return new GetAllOrganizationResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public CreateOrganizationResult CreateOrganization(CreateOrganizationParameter parameter)
        {
            try
            {
                var parent = context.Organization.FirstOrDefault(o => o.OrganizationId == parameter.ParentId);
                var parentLvl = parent?.Level ?? 1;

                Organization organization = new Organization()
                {
                    OrganizationId = Guid.NewGuid(),
                    OrganizationName = parameter.OrganizationName,
                    OrganizationCode = parameter.OrganizationCode,
                    Level = string.IsNullOrEmpty(parameter.ParentId.ToString()) ? parameter.Level : parentLvl + 1,
                    ParentId = parameter.ParentId,
                    IsFinancialIndependence = parameter.IsFinancialIndependence,
                    GeographicalAreaId = parameter.GeographicalAreaId,
                    ProvinceId = parameter.ProvinceId,
                    DistrictId = parameter.DistrictId,
                    WardId = parameter.WardId,
                    OrganizationOtherCode = parameter.OrganizationOtherCode,
                    CreatedById = parameter.UserId,
                    CreatedDate = DateTime.Now,
                    Active = true,
                    IsAccess = parameter.IsAccess
                };
                context.Organization.Add(organization);

                Contact contact = new Contact()
                {
                    ContactId = Guid.NewGuid(),
                    Phone = parameter.Phone,
                    Address = parameter.Address,
                    ObjectId = organization.OrganizationId,
                    ObjectType = "ORG",
                    CreatedById = parameter.UserId,
                    CreatedDate = DateTime.Now,
                    Active = true
                };
                context.Contact.Add(contact);

                //Thêm thành viên vào phòng ban
                var listThanhVienPb = new List<ThanhVienPhongBan>();
                parameter.ListThanhVienPhongBan?.ForEach(item =>
                {
                    var tv = new ThanhVienPhongBan();
                    tv.Id = Guid.NewGuid();
                    tv.EmployeeId = item.EmployeeId.Value;
                    tv.OrganizationId = organization.OrganizationId;
                    tv.IsManager = item.IsManager;

                    listThanhVienPb.Add(tv);
                });
                context.ThanhVienPhongBan.AddRange(listThanhVienPb);

                context.SaveChanges();

                return new CreateOrganizationResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = CommonMessage.Organization.CREATE_SUCCESS,
                    CreatedOrgId = organization.OrganizationId
                };
            }
            catch (Exception e)
            {
                return new CreateOrganizationResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetOrganizationByIdResult GetOrganizationById(GetOrganizationByIdParameter parameter)
        {
            try
            {
                var org = context.Organization.FirstOrDefault(o => o.OrganizationId == parameter.OrganizationId);
                var contact = context.Contact.FirstOrDefault(c => c.ObjectId == parameter.OrganizationId);
                if (org != null && contact != null)
                {
                    var parentId = org.ParentId == null ? Guid.Empty : org.ParentId;
                    var parentName = parentId == Guid.Empty ? string.Empty : GetParentName(parentId);

                    OrganizationEntityModel organization = new OrganizationEntityModel()
                    {
                        OrganizationId = org.OrganizationId,
                        OrganizationName = org.OrganizationName,
                        OrganizationCode = org.OrganizationCode,
                        Address = contact.Address,
                        Phone = contact.Phone,
                        ParentName = parentName,
                        IsFinancialIndependence = org.IsFinancialIndependence,
                        GeographicalAreaId = org.GeographicalAreaId,
                        ProvinceId = org.ProvinceId,
                        DistrictId = org.DistrictId,
                        WardId = org.WardId,
                        SatelliteId = org.SatelliteId,
                        OrganizationOtherCode = org.OrganizationOtherCode,
                        IsAccess = org.IsAccess
                    };

                    var listThanhVienPhongBan = context.ThanhVienPhongBan
                        .Where(x => x.OrganizationId == organization.OrganizationId).Select(y => new ThanhVienPhongBanModel
                        {
                            Id = y.Id,
                            EmployeeId = y.EmployeeId,
                            OrganizationId = y.OrganizationId,
                            IsManager = y.IsManager
                        }).ToList();
                    var listEmpId = listThanhVienPhongBan.Select(y => y.EmployeeId);
                    var listEmp = context.Employee.Where(x => listEmpId.Contains(x.EmployeeId)).Select(y => new
                    {
                        y.EmployeeId,
                        y.EmployeeCode,
                        y.EmployeeName
                    }).ToList();
                    listThanhVienPhongBan.ForEach(item =>
                    {
                        var emp = listEmp.FirstOrDefault(x => x.EmployeeId == item.EmployeeId);
                        item.EmployeeCodeName = emp?.EmployeeCode + " - " + emp?.EmployeeName;
                    });

                    return new GetOrganizationByIdResult()
                    {
                        StatusCode = HttpStatusCode.OK,
                        MessageCode = "Success",
                        Organization = organization,
                        ListThanhVienPhongBan = listThanhVienPhongBan
                    };
                }

                return new GetOrganizationByIdResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = CommonMessage.Organization.GET_FAIL
                };
            }
            catch (Exception e)
            {
                return new GetOrganizationByIdResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public EditOrganizationByIdResult EditOrganizationById(EditOrganizationByIdParameter parameter)
        {
            try
            {
                var organization = context.Organization.FirstOrDefault(o => o.OrganizationId == parameter.OrganizationId);
                var contact = context.Contact.FirstOrDefault(c => c.ObjectId == parameter.OrganizationId);
                var checkDif = context.Organization.FirstOrDefault(o => o.OrganizationCode == parameter.OrganizationCode && o.OrganizationId != parameter.OrganizationId);

                if (checkDif != null)
                {
                    return new EditOrganizationByIdResult()
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = CommonMessage.Organization.DUPLICATE_CODE
                    };
                }

                if (organization != null && contact != null)
                {
                    organization.OrganizationName = parameter.OrganizationName;
                    organization.OrganizationCode = parameter.OrganizationCode;
                    organization.IsFinancialIndependence = parameter.IsFinancialIndependence;
                    organization.UpdatedById = parameter.UserId;
                    organization.UpdatedDate = DateTime.Now;

                    contact.Phone = parameter.Phone;
                    contact.Address = parameter.Address;
                    contact.UpdatedById = parameter.UserId;
                    contact.UpdatedDate = DateTime.Now;

                    context.SaveChanges();

                    return new EditOrganizationByIdResult()
                    {
                        StatusCode = HttpStatusCode.OK,
                        MessageCode = CommonMessage.Organization.EDIT_SUCCESS,
                    };
                }

                return new EditOrganizationByIdResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = CommonMessage.Organization.EDIT_FAIL
                };
            }
            catch (Exception e)
            {
                return new EditOrganizationByIdResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
            
        }

        public DeleteOrganizationByIdResult DeleteOrganizationById(DeleteOrganizationByIdParameter parameter)
        {
            try
            {
                var employeeInOrganize = context.Employee.Any(e => e.OrganizationId == parameter.OrganizationId);
                var orgLevel = context.Organization.FirstOrDefault(o => o.OrganizationId == parameter.OrganizationId)
                    ?.Level;
                var count = context.ThanhVienPhongBan.Count(x => x.OrganizationId == parameter.OrganizationId);

                if (orgLevel == 0)
                {
                    return new DeleteOrganizationByIdResult()
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = CommonMessage.MasterData.HIGHEST_LEVEL,
                    };
                }

                if (count > 0)
                {
                    return new DeleteOrganizationByIdResult()
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = "Không thể xóa đã có nhân viên thuộc đơn vị",
                    };
                }

                if (employeeInOrganize)
                {
                    return new DeleteOrganizationByIdResult()
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = "Không thể xóa đã có nhân viên thuộc đơn vị",
                    };
                }

                var childOrg = context.Organization.Where(o => o.ParentId == parameter.OrganizationId).ToList();
                if (childOrg.Count > 0)
                {
                    return new DeleteOrganizationByIdResult()
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = CommonMessage.Organization.HAS_CHILD,
                    };
                }

                var contact = context.Contact.FirstOrDefault(c => c.ObjectId == parameter.OrganizationId);
                var org = context.Organization.FirstOrDefault(o => o.OrganizationId == parameter.OrganizationId);

                if (contact != null)
                {
                    context.Contact.Remove(contact);
                }

                if (org != null)
                {
                    context.Organization.Remove(org);
                }

                context.SaveChanges();

                return new DeleteOrganizationByIdResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = CommonMessage.Organization.DELETE_SUCCESS,
                };
            }
            catch (Exception e)
            {
                return new DeleteOrganizationByIdResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetAllOrganizationCodeResult GetAllOrganizationCode(GetAllOrganizationCodeParameter parameter)
        {
            try
            {
                var orgList = context.Organization.ToList();
                List<string> codeList = new List<string>();
                orgList.ForEach(o =>
                {
                    codeList.Add(o.OrganizationCode.ToLower());
                });

                return new GetAllOrganizationCodeResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success",
                    OrgCodeList = codeList
                };
            }
            catch (Exception e)
            {
                return new GetAllOrganizationCodeResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetFinancialindependenceOrgResult GetFinancialindependenceOrg(GetFinancialindependenceOrgParameter parameter)
        {
            try
            {
                var lst = context.Organization.Where(o => o.IsFinancialIndependence.Value).ToList();
                var list = new List<OrganizationEntityModel>();
                lst.ForEach(item =>
                {
                    var _item = new OrganizationEntityModel(item);
                    list.Add(_item);
                });

                return new GetFinancialindependenceOrgResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success",
                    ListOrg = list
                };
            }
            catch (Exception e)
            {
                return new GetFinancialindependenceOrgResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        private List<OrganizationEntityModel> GetChildren(Guid? id, List<OrganizationEntityModel> list)
        {
            return list.Where(l => l.ParentId == id)
                .Select(l => new OrganizationEntityModel()
                {
                    OrganizationId = l.OrganizationId,
                    OrganizationName = l.OrganizationName,
                    Level = l.Level,
                    ParentId = l.ParentId,
                    IsFinancialIndependence = l.IsFinancialIndependence,
                    OrgChildList = GetChildren(l.OrganizationId, list)
                }).OrderBy(l => l.OrganizationName).ToList();
        }

        public string GetParentName(Guid? parentId)
        {
            if (parentId != null)
            {
                var objectOrganization = context.Organization.FirstOrDefault(o => o.OrganizationId == parentId);
                if (objectOrganization.ParentId != Guid.Empty)
                {
                    string equal = GetParentName(objectOrganization.ParentId);
                    string Uio = string.IsNullOrEmpty(equal) ? equal : string.Format(" - {0}", equal);
                    return objectOrganization.OrganizationName + Uio;
                }
                else
                {
                    return "";
                }
            }
            else
            {
                return "";
            }
        }

        public GetChildrenOrganizationByIdResult GetChildrenOrganizationById(GetChildrenOrganizationByIdParameter parameter)
        {
            try
            {
                var organization = context.Employee.FirstOrDefault(e => e.EmployeeId == parameter.EmployeeId);
                var organizationId = organization.OrganizationId;
                if (organizationId == null)
                {
                    return new GetChildrenOrganizationByIdResult()
                    {
                        Status = false,
                        Message = CommonMessage.Organization.GET_FAIL
                    };
                }
                var list = context.Organization.Select(o => new OrganizationEntityModel()
                {
                    OrganizationId = o.OrganizationId,
                    OrganizationName = o.OrganizationName,
                    Level = o.Level,
                    ParentId = o.ParentId
                }).ToList();
                List<OrganizationEntityModel> recoreds = list.Where(l => l.ParentId == organizationId).Select(l => new OrganizationEntityModel()
                {
                    OrganizationId = l.OrganizationId,
                    OrganizationName = l.OrganizationName,
                    Level = l.Level,
                    ParentId = l.ParentId,
                    OrgChildList = GetChildren(l.OrganizationId, list)
                }).OrderBy(l => l.OrganizationName).ToList();

                List<dynamic> lstResult = new List<dynamic>();
                list.ForEach(item =>
                {
                    var sampleObject = new ExpandoObject() as IDictionary<string, Object>;
                    if (item.OrganizationId == organizationId)
                    {
                        sampleObject.Add("OrganizationId", item.OrganizationId);
                        sampleObject.Add("OrganizationName", item.OrganizationName);
                        sampleObject.Add("Level", item.Level);
                        sampleObject.Add("ParentId", item.ParentId);
                        lstResult.Add(sampleObject);
                    }
                });

                return new GetChildrenOrganizationByIdResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success",
                    listOrganization = recoreds,
                    organizationParent = lstResult,
                    isManager = organization.IsManager
                };
            }
            catch (Exception e)
            {
                return new GetChildrenOrganizationByIdResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetOrganizationByEmployeeIdResult GetOrganizationByEmployeeId(GetOrganizationByEmployeeIdParameter parameter)
        {
            try
            {
                var emp = context.Employee.FirstOrDefault(e => e.EmployeeId == parameter.EmployeeId);
                var org = context.Organization.FirstOrDefault(o => o.OrganizationId == emp.OrganizationId);
            
                if (org != null || emp != null)
                {
                    OrganizationEntityModel organization = new OrganizationEntityModel()
                    {
                        OrganizationId = org.OrganizationId,
                        OrganizationName = org.OrganizationName,
                        OrganizationCode = org.OrganizationCode,
                        IsFinancialIndependence = org.IsFinancialIndependence
                    };

                    return new GetOrganizationByEmployeeIdResult()
                    {
                        StatusCode = HttpStatusCode.OK,
                        MessageCode = "Success",
                        Organization = organization
                    };
                }

                return new GetOrganizationByEmployeeIdResult()
                {
                    Organization = null,
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = CommonMessage.Organization.GET_FAIL
                };
            }
            catch (Exception e)
            {
                return new GetOrganizationByEmployeeIdResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetChildrenByOrganizationIdResult GetChildrenByOrganizationId(GetChildrenByOrganizationIdParameter parameter)
        {
            try
            {
                var org = context.Organization.FirstOrDefault(o => o.OrganizationId == parameter.OrganizationId);

                var list = context.Organization.Select(o => new OrganizationEntityModel()
                {
                    OrganizationId = o.OrganizationId,
                    OrganizationName = o.OrganizationName,
                    Level = o.Level,
                    ParentId = o.ParentId
                }).ToList();

                List<OrganizationEntityModel> recoreds = list.Where(l => l.ParentId == org.OrganizationId).Select(l => new OrganizationEntityModel()
                {
                    OrganizationId = l.OrganizationId,
                    OrganizationName = l.OrganizationName,
                    Level = l.Level,
                    ParentId = l.ParentId,
                    OrgChildList = GetChildren(l.OrganizationId, list)
                }).OrderBy(l => l.OrganizationName).ToList();

                OrganizationEntityModel organization = new OrganizationEntityModel()
                {
                    OrganizationId = org.OrganizationId,
                    OrganizationName = org.OrganizationName,
                    Level = org.Level,
                    ParentId = org.ParentId,
                    OrgChildList = recoreds
                };

                List<OrganizationEntityModel> list_final = new List<OrganizationEntityModel>();
                list_final.Add(organization);

                return new GetChildrenByOrganizationIdResult
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success",
                    OrganizationList = list_final
                };
            }
            catch (Exception e)
            {
                return new GetChildrenByOrganizationIdResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public UpdateOrganizationByIdResult UpdateOrganizationById(UpdateOrganizationByIdParameter parameter)
        {
            try
            {
                var org = context.Organization.FirstOrDefault(x => x.OrganizationId == parameter.OrganizationId);

                org.OrganizationCode = parameter.OrganizationCode;
                org.OrganizationName = parameter.OrganizationName;
                org.IsFinancialIndependence = parameter.IsFinancialIndependence;
                org.GeographicalAreaId = parameter.GeographicalAreaId;
                org.ProvinceId = parameter.ProvinceId;
                org.DistrictId = parameter.DistrictId;
                org.WardId = parameter.WardId;
                org.SatelliteId = parameter.SatelliteId;
                org.OrganizationOtherCode = parameter.OrganizationOtherCode;
                org.IsAccess = parameter.IsAccess;

                context.Organization.Update(org);

                var org_contact = context.Contact.FirstOrDefault(x =>
                    x.ObjectType == "ORG" && x.ObjectId == parameter.OrganizationId);

                org_contact.Phone = parameter.OrganizationPhone;
                org_contact.Address = parameter.OrganizationAddress;

                context.Contact.Update(org_contact);

                //Thêm nhân viên trong phòng ban
                var listOld = context.ThanhVienPhongBan.Where(x => x.OrganizationId == org.OrganizationId).ToList();
                context.ThanhVienPhongBan.RemoveRange(listOld);

                var listThanhVienPb = new List<ThanhVienPhongBan>();
                parameter.ListThanhVienPhongBan?.ForEach(item =>
                {
                    var emp = new ThanhVienPhongBan();
                    emp.Id = Guid.NewGuid();
                    emp.EmployeeId = item.EmployeeId.Value;
                    emp.OrganizationId = org.OrganizationId;
                    emp.IsManager = item.IsManager;

                    listThanhVienPb.Add(emp);
                });
                context.ThanhVienPhongBan.AddRange(listThanhVienPb);

                context.SaveChanges();

                return new UpdateOrganizationByIdResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = "Success"
                };
            }
            catch (Exception e)
            {
                return new UpdateOrganizationByIdResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetOrganizationByUserResult GetOrganizationByUser(GetOrganizationByUserParameter parameter)
        {
            try
            {
                var listOrganizationentity = context.Organization.Where(w => w.Active == true)
                    .OrderBy(w => w.OrganizationName).ToList();
                var user = context.User.FirstOrDefault(x => x.UserId == parameter.UserId && x.Active == true);
                var employeeId = user.EmployeeId;
                var employee = context.Employee.FirstOrDefault(x => x.EmployeeId == employeeId);
                var isManager = employee.IsManager;

                List<Guid?> listGetAllChild = new List<Guid?>(); //List phòng ban: chính nó và các phòng ban cấp dưới của nó
         
                var currentEmployeeOrganizationId = employee.OrganizationId;

                List<Guid?> listGetAllParent = new List<Guid?>();

                var listAllParentId = getOrganizationParentId(currentEmployeeOrganizationId, listOrganizationentity, listGetAllParent); //List phòng ban cấp trên

                if (isManager)
                {
                    //Lấy list phòng ban con của user
                    if (employee.OrganizationId != null && isManager)
                    {
                        listGetAllChild.Add(employee.OrganizationId.Value);
                        listGetAllChild = getOrganizationChildrenId(employee.OrganizationId.Value, listGetAllChild, listOrganizationentity);
                    }
                }
                else
                {
                    //Nếu không phải quản lý
                    listGetAllChild.Add(employee.OrganizationId.Value);
                }

                var listOrganization = new List<DataAccess.Models.OrganizationEntityModel>();

                //gộp danh sách (phòng ban user hiện tại + cấp dưới) và  danh sách (phòng ban cấp trên)

                var listAllOrganization = new List<Guid?>();
                listAllOrganization.AddRange(listAllParentId);
                listAllOrganization.AddRange(listGetAllChild);

                listAllOrganization?.ForEach(orgId =>
                {
                    var org = listOrganizationentity.FirstOrDefault(f => f.OrganizationId == orgId);

                    if (org != null)
                    {
                        listOrganization.Add(new OrganizationEntityModel
                        {
                            OrganizationId = org.OrganizationId,
                            OrganizationName = org.OrganizationName,
                            OrganizationCode = org.OrganizationCode,
                            ParentId = org.ParentId,
                            Level = org.Level
                        });
                    }
                });

                //Lấy id phòng ban con của từng phòng ban
                listOrganization?.ForEach(org =>
                {
                    var hasChild = listOrganization.Where(w => w != org).FirstOrDefault(f => f.ParentId == org.OrganizationId);

                    if (hasChild != null)
                    {
                        org.HasChildren = true;
                    } else
                    {
                        org.HasChildren = false;
                    }
                });

                #region Lấy danh sách chọn phòng ban hợp lệ
                /*
                 *  Nếu là admin: phòng ban của nó và cấp dươi
                 *  Nếu là nhân viên: phòng ban chính nó
                 */

                var listValidSelectionOrganization = new List<Guid?>();
                if (isManager == true)
                {
                    listValidSelectionOrganization.AddRange(listGetAllChild);
                } else
                {
                    listValidSelectionOrganization.Add(currentEmployeeOrganizationId);
                }
                #endregion

                return new GetOrganizationByUserResult()
                {
                    ListOrganization = listOrganization,
                    ListValidSelectionOrganization = listValidSelectionOrganization,
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success",
                };
            }
            catch (Exception e)
            {
                return new GetOrganizationByUserResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public DeleteNhanVienThuocDonViResult DeleteNhanVienThuocDonVi(DeleteNhanVienThuocDonViParameter parameter)
        {
            try
            {
                var emp = context.ThanhVienPhongBan.FirstOrDefault(x => x.Id == parameter.Id);
                if (emp == null)
                {
                    return new DeleteNhanVienThuocDonViResult()
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = "Nhân viên không tồn tại trong đơn vị",
                    };
                }

                var count = context.ThanhVienPhongBan.Count(x => x.EmployeeId == emp.EmployeeId);
                if (count == 1)
                {
                    return new DeleteNhanVienThuocDonViResult()
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = "Nhân viên cần thuộc ít nhất một đơn vị",
                    };
                }

                return new DeleteNhanVienThuocDonViResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Xóa thành công",
                };
            }
            catch (Exception e)
            {
                return new DeleteNhanVienThuocDonViResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        //Function lấy phòng ban cấp dưới
        private List<Guid?> getOrganizationChildrenId(Guid? id, List<Guid?> list, List<Organization> ListOrganization)
        {
            var Organization = ListOrganization.Where(o => o.ParentId == id).ToList();
            Organization.ForEach(item =>
            {
                list.Add(item.OrganizationId);
                getOrganizationChildrenId(item.OrganizationId, list, ListOrganization);
            });

            return list;
        }

        //Function lấy Phòng ban cấp trên
        private List<Guid?> getOrganizationParentId(Guid? currentOrganization , List<Organization> ListOrganization, List<Guid?> ListGetAllParent)
        {
            var result = ListGetAllParent;

            //phòng ban hiện tại
            var currentOrg = ListOrganization.FirstOrDefault(f => f.OrganizationId == currentOrganization);

            //Lấy phòng cha
            var parentOrganization = ListOrganization.FirstOrDefault(f => f.OrganizationId == currentOrg.ParentId);
            if (parentOrganization != null)
            {
                result.Add(parentOrganization.OrganizationId);
                //đệ quy tìm phòng ban cha
                getOrganizationParentId(parentOrganization.OrganizationId, ListOrganization, result);
            }

            return result;
        }
    }
}
