using OfficeOpenXml.FormulaParsing.Excel.Functions.Information;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using TN.TNM.Common;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Helper;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Jwt;
using TN.TNM.DataAccess.Messages.Parameters.Admin;
using TN.TNM.DataAccess.Messages.Parameters.Admin.Permission;
using TN.TNM.DataAccess.Messages.Parameters.Users;
using TN.TNM.DataAccess.Messages.Results.Admin;
using TN.TNM.DataAccess.Messages.Results.Admin.Permission;
using TN.TNM.DataAccess.Messages.Results.Users;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.ActionResource;
using TN.TNM.DataAccess.Models.Address;
using TN.TNM.DataAccess.Models.Admin;
using TN.TNM.DataAccess.Models.FireBase;
using TN.TNM.DataAccess.Models.MenuBuild;
using TN.TNM.DataAccess.Models.Permission;
using TN.TNM.DataAccess.Models.User;

/// <summary>
/// Authentication Data Access Object
/// Use to authenticate and authorize user
/// 
/// Author: thanhhh@tringhiatech.vn
/// Date: 14/06/2018
/// </summary>
namespace TN.TNM.DataAccess.Databases.DAO
{
    public class AuthDAO : BaseDAO, IAuthDataAccess
    {
        public AuthDAO(TNTN8Context _content, IAuditTraceDataAccess _iAuditTrace)
        {
            this.context = _content;
            this.iAuditTrace = _iAuditTrace;
        }

        public GetMenuByModuleCodeResult GetMenuByModuleCode(GetMenuByModuleCodeParameter parameter)
        {
            try
            {
                ////Get list menu directly by User Id
                //var perByUser = from p in this.context.Permission
                //                join pM in this.context.PermissionMapping on p.PermissionId equals pM.PermissionId
                //                join pR in this.context.Permission on p.ParentId equals pR.PermissionId
                //                where pR.PermissionCode == parameter.ModuleCode && pM.UserId == parameter.UserId && "S".Equals(p.Type)
                //                orderby p.Sort ascending
                //                select p;

                ////Get list menu by group where user in
                //var perByGroup = from p in this.context.Permission
                //                 join pR in this.context.Permission on p.ParentId equals pR.PermissionId
                //                 join pM in this.context.PermissionMapping on p.PermissionId equals pM.PermissionId
                //                 join uG in this.context.GroupUser on pM.GroupId equals uG.GroupId
                //                 where pR.PermissionCode == parameter.ModuleCode && uG.UserId == parameter.UserId && "S".Equals(p.Type)
                //                 orderby p.Sort ascending
                //                 select p;

                //var permissions = perByUser.Union(perByGroup);
                //return new GetMenuByModuleCodeResult
                //{
                //    Permissions = permissions.ToList()
                //};

                //Lay ra permission cha theo module code truyen vao

                var parentPermissionId = context.Permission.FirstOrDefault(p => p.PermissionCode == parameter.ModuleCode) != null ? context.Permission.FirstOrDefault(p => p.PermissionCode == parameter.ModuleCode).PermissionId : Guid.Empty;

                //Lay ra cac permission con cua permission cha
                List<Permission> perChildList = new List<Permission>();
                if (parentPermissionId != Guid.Empty)
                {
                    perChildList = context.Permission.Where(p => p.ParentId == parentPermissionId && p.Type.Equals("S")).ToList();
                }

                //Lay nhom quyen cua user theo user ID
                var permissionSetOfUser = context.PermissionMapping.FirstOrDefault(pm => pm.UserId == parameter.UserId);
                if (permissionSetOfUser != null)
                {
                    var permissionSetOfUserId = permissionSetOfUser.PermissionSetId;
                    var permissionIdList = context.PermissionSet.FirstOrDefault(ps => ps.PermissionSetId == permissionSetOfUserId).PermissionId.Split(";").ToList();

                    //Kiem tra neu nhom quyen con khong ton tai trong nhom quyen cua user => remove khoi danh sach hien thi
                    if (perChildList.Count > 0)
                    {
                        List<Permission> newList = new List<Permission>();
                        List<Permission> listRemove = new List<Permission>();
                        perChildList.ForEach(perChild =>
                        {
                            if (permissionIdList.IndexOf(perChild.PermissionId.ToString()) == -1)
                            {
                                listRemove.Add(perChild);
                            }
                            else
                            {
                                newList.Add(perChild);
                            }
                        });

                        perChildList = newList;
                    }
                }
                var listPermissionEntityModel = new List<PermissionEntityModel>();
                perChildList.ForEach(item =>
                {
                    listPermissionEntityModel.Add(new PermissionEntityModel(item));
                });
                return new GetMenuByModuleCodeResult
                {
                    Permissions = listPermissionEntityModel,
                    MessageCode = "Success",
                    StatusCode = HttpStatusCode.OK
                };
            }
            catch (Exception e)
            {
                return new GetMenuByModuleCodeResult
                {
                    MessageCode = e.Message,
                    StatusCode = HttpStatusCode.ExpectationFailed
                };
            }

        }

        public LoginResult Login(LoginParameter paramater, string secretKey, string issuer, string audience)
        {
            try
            {
                User user = new User();
                paramater.User.Password = AuthUtil.GetHashingPassword(paramater.User.Password);
                user = context.User.FirstOrDefault(u =>
                    u.UserName == paramater.User.UserName && u.Password == paramater.User.Password);

                //Cập nhật deviceId của người dùng nếu có
                if(paramater.User.DeviceId != null && user != null)
                {
                    user.DeviceId = paramater.User.DeviceId.Trim();
                    context.User.Update(user);
                    context.SaveChanges();
                }
                
                // check login bằng account vendor
                var externalUser = context.ExternalUser.FirstOrDefault(u =>
                 u.UserName == paramater.User.UserName && u.Password == paramater.User.Password);

                if (externalUser != null)
                {
                    user = new User()
                    {
                        Active = externalUser.Active,
                        CreatedById = externalUser.CreatedById,
                        CreatedDate = externalUser.CreatedDate,
                        Disabled = externalUser.Disabled,
                        //Employee = context.Employee.FirstOrDefault(x => x.EmployeeId == externalUser.EmployeeId),
                        EmployeeId = externalUser.EmployeeId,
                        GroupUser = null,
                        IsAdmin = false,
                        Password = externalUser.Password,
                        PermissionMapping = null,
                        ResetCode = null,
                        ResetCodeDate = null,
                        TenantId = externalUser.TenantId,
                        UpdatedById = externalUser.UpdatedById,
                        UpdatedDate = externalUser.UpdatedDate,
                        UserId = externalUser.ExternalUserId,
                        UserName = externalUser.UserName
                    };
                }

                if (user == null)
                {
                    LogHelper.LoginAuditTrace(context, paramater.User.UserName, 0);

                    return new LoginResult()
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = CommonMessage.Login.WRONG_USER_PASSWORD
                    };
                }
                var token = new JwtTokenBuilder()
                                    .AddSecurityKey(JwtSecurityKey.Create(secretKey))
                                    .AddSubject(user.UserName)
                                    .AddIssuer(issuer)
                                    .AddAudience(audience)
                                    .AddClaim("MembershipId", user.UserId.ToString())
                                    .AddExpiry(60 * 24 * 365)
                                    .Build();
                LogHelper.LoginAuditTrace(context, paramater.User.UserName, 1);

                if (user.Active == false)
                {
                    var currentUserFail = new AuthEntityModel
                    {
                        UserId = user.UserId,
                        UserName = user.UserName,
                        EmployeeId = user.EmployeeId.Value,
                        Token = token.Value,
                        LoginTime = new DateTime(),
                    };
                    return new LoginResult()
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = CommonMessage.Login.INACTIVE_USER,
                        CurrentUser = currentUserFail
                    };
                }

                bool isAdmin = user.IsAdmin == null ? false : user.IsAdmin.Value;
                var empId = user.EmployeeId;
                string userFullName = externalUser != null ? externalUser.UserName : "";
                string userAvatar = "";
                string userEmail = "";
                bool isManager = false;
                bool isOrder = false;
                bool isCashier = false;
                Guid? positionId = Guid.Empty;
                List<string> perCodeList = new List<string>();
                List<SystemParameter> systemParameterList = new List<SystemParameter>();
                List<string> listTextActionResource = new List<string>();
                var ListMenuBuild = new List<MenuBuildEntityModel>();
                string employeeCode = "";
                string employeeName = "";
                string employeeCodeName = "";
                Guid? roleId = Guid.Empty;

                var emp = context.Employee.FirstOrDefault(e => e.EmployeeId == empId);
                //Nếu là nhân viên
                if (emp != null)
                {
                    employeeCode = emp?.EmployeeCode;
                    employeeName = emp?.EmployeeName;
                    employeeCodeName = employeeCode + " - " + employeeName;
                    userFullName = emp?.EmployeeName;
                    userAvatar = context.Contact.FirstOrDefault(c => c.ObjectId == empId && c.ObjectType == "EMP")?
                        .AvatarUrl;
                    userEmail = context.Contact.FirstOrDefault(c => c.ObjectId == empId && c.ObjectType == "EMP")?.Email;
                    var manager = context.Employee.FirstOrDefault(e => e.EmployeeId == empId);
                    if (manager != null)
                    {
                        isManager = manager.IsManager;
                        positionId = manager.PositionId;
                        isOrder = manager.IsOrder ?? false;
                        isCashier = manager.IsCashier ?? false;
                    }

                    if (user != null)
                    {
                        var permissionSetOfUser = context.PermissionMapping.FirstOrDefault(pm => pm.UserId == user.UserId);
                        if (permissionSetOfUser != null)
                        {
                            var permissionSetOfUserId = permissionSetOfUser.PermissionSetId;
                            var permissionIdList = context.PermissionSet
                                .FirstOrDefault(ps => ps.PermissionSetId == permissionSetOfUserId).PermissionId.Split(";")
                                .ToList();
                            permissionIdList.ForEach(perId =>
                            {
                                if (!string.IsNullOrEmpty(perId))
                                {
                                    var perCode = context.Permission.FirstOrDefault(p => p.PermissionId == Guid.Parse(perId))
                                        .PermissionCode;
                                    perCodeList.Add(perCode);
                                }
                            });
                        }
                    }

                    systemParameterList = context.SystemParameter.ToList();

                    //Lấy list User Role
                    var listUserRole = context.UserRole.Where(e => e.UserId == user.UserId).ToList();
                    List<Guid> listRoleId = new List<Guid>();
                    if (listUserRole.Count > 0)
                    {
                        listUserRole.ForEach(item =>
                        {
                            listRoleId.Add(item.RoleId.Value);
                        });
                    }

                    //Lấy list Action Resource Id
                    var listActionResource =
                        context.RoleAndPermission.Where(e => listRoleId.Contains(e.RoleId.Value)).ToList();
                    List<Guid> listActionResourceId = new List<Guid>();
                    if (listActionResource.Count > 0)
                    {
                        listActionResource.ForEach(item =>
                        {
                            listActionResourceId.Add(item.ActionResourceId.Value);
                        });
                    }

                    //Lấy list text action resource
                    listTextActionResource = context.ActionResource
                        .Where(e => listActionResourceId.Contains(e.ActionResourceId)).Select(x => x.ActionResource1)
                        .ToList();

                    #region Lấy list MenuBuid

                    ListMenuBuild = context.MenuBuild.Where(x => x.IsShow == true).Select(y => new MenuBuildEntityModel
                    {
                        MenuBuildId = y.MenuBuildId,
                        ParentId = y.ParentId,
                        Name = y.Name,
                        Code = y.Code,
                        CodeParent = y.CodeParent,
                        Path = y.Path,
                        NameIcon = y.NameIcon,
                        Level = y.Level,
                        IndexOrder = y.IndexOrder,
                        IsPageDetail = y.IsPageDetail
                    }).OrderBy(z => z.IndexOrder).ToList();

                    //Lấy list đường dẫn mặc định gắn với nhóm quyền
                    //Nếu user được phân duy nhất 1 quyền
                    if (listRoleId.Count == 1)
                    {
                        roleId = listRoleId.FirstOrDefault();

                        var listRoleAndMenuBuild = context.RoleAndMenuBuild.Where(x => x.RoleId == roleId).ToList();
                        ListMenuBuild.ForEach(item =>
                        {
                            //Lấy ra các sub menu module
                            if (item.Level == 1)
                            {
                                var existsDefaultPath = listRoleAndMenuBuild.FirstOrDefault(x => x.Code == item.Code);

                                //Nếu tồn tại đường dẫn mặc định đã được cấu hình
                                if (existsDefaultPath != null)
                                {
                                    item.Path = existsDefaultPath.Path;
                                }
                            }
                        });
                    }

                    #endregion

                }
                //Nếu là KH
                else
                {
                    var cus = context.Customer.FirstOrDefault(e => e.CustomerId == empId);
                    employeeCode = cus?.CustomerCode;
                    employeeName = cus?.CustomerName;
                    employeeCodeName = employeeCode + " - " + employeeName;
                    userFullName = cus?.CustomerName;
                    var contact = context.Contact.FirstOrDefault(c => c.ObjectId == empId && c.ObjectType == "CUS");
                    userAvatar = contact?.AvatarUrl;
                    userEmail = contact?.Email;
                }
                var contactEntityModel = context.Contact
                              .Where(x => x.ObjectId == empId)
                              .Select(x => new ContactEntityModel
                              {
                                  Address = x.Address,
                                  Phone = x.Phone,
                                  Email = x.Email,
                                  LastName = x.LastName,
                                  FirstName = x.FirstName,
                                  Gender = x.Gender,
                                  DateOfBirth = x.DateOfBirth,
                                  ObjectType = x.ObjectType,
                                  ProvinceId = x.ProvinceId,
                              }).FirstOrDefault();

                var currentUser = new AuthEntityModel
                {
                    UserId = user.UserId,
                    UserName = user.UserName,
                    EmployeeId = user.EmployeeId.Value,
                    Token = token.Value,
                    LoginTime = new DateTime(),
                    PositionId = positionId
                };

                if (contactEntityModel.ObjectType == "CUS")
                {
                    return new LoginResult
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = "Bạn không có quyền truy cập!"
                    };
                }

                return new LoginResult
                {
                    StatusCode = HttpStatusCode.OK,
                    RoleId = roleId,
                    CurrentUser = currentUser,
                    UserFullName = userFullName,
                    UserAvatar = userAvatar,
                    UserEmail = userEmail,
                    IsManager = isManager,
                    PermissionList = perCodeList,
                    PositionId = positionId,
                    ListPermissionResource = listTextActionResource,
                    IsAdmin = isAdmin,
                    SystemParameterList = systemParameterList,
                    IsOrder = isOrder,
                    IsCashier = isCashier,
                    ListMenuBuild = ListMenuBuild,
                    EmployeeCode = employeeCode,
                    EmployeeName = employeeName,
                    EmployeeCodeName = employeeCodeName,
                    ContactEntityModel = contactEntityModel
                };
            }
            catch (Exception e)
            {
                return new LoginResult
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public LoginResult LoginWithDeviceId(LoginParameter paramater, string secretKey, string issuer, string audience)
        {
            try
            {
                User user = new User();
                paramater.User.Password = AuthUtil.GetHashingPassword(paramater.User.Password);
                user = context.User.FirstOrDefault(u =>
                    u.UserName == paramater.User.UserName && u.Password == paramater.User.Password);

                //Cập nhật deviceId của người dùng nếu có
                if (paramater.User.DeviceId != null && user != null)
                {
                    user.DeviceId = paramater.User.DeviceId.Trim();
                    context.User.Update(user);
                    context.SaveChanges();
                }

                // check login bằng account vendor
                var externalUser = context.ExternalUser.FirstOrDefault(u =>
                 u.UserName == paramater.User.UserName && u.Password == paramater.User.Password);

                if (externalUser != null)
                {
                    user = new User()
                    {
                        Active = externalUser.Active,
                        CreatedById = externalUser.CreatedById,
                        CreatedDate = externalUser.CreatedDate,
                        Disabled = externalUser.Disabled,
                        //Employee = context.Employee.FirstOrDefault(x => x.EmployeeId == externalUser.EmployeeId),
                        EmployeeId = externalUser.EmployeeId,
                        GroupUser = null,
                        IsAdmin = false,
                        Password = externalUser.Password,
                        PermissionMapping = null,
                        ResetCode = null,
                        ResetCodeDate = null,
                        TenantId = externalUser.TenantId,
                        UpdatedById = externalUser.UpdatedById,
                        UpdatedDate = externalUser.UpdatedDate,
                        UserId = externalUser.ExternalUserId,
                        UserName = externalUser.UserName
                    };
                }

                if (user == null)
                {
                    LogHelper.LoginAuditTrace(context, paramater.User.UserName, 0);

                    return new LoginResult()
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = CommonMessage.Login.WRONG_USER_PASSWORD
                    };
                }
                var token = new JwtTokenBuilder()
                                    .AddSecurityKey(JwtSecurityKey.Create(secretKey))
                                    .AddSubject(user.UserName)
                                    .AddIssuer(issuer)
                                    .AddAudience(audience)
                                    .AddClaim("MembershipId", user.UserId.ToString())
                                    .AddExpiry(60 * 24 * 365)
                                    .Build();
                LogHelper.LoginAuditTrace(context, paramater.User.UserName, 1);

                if (user.Active == false)
                {
                    var currentUserFail = new AuthEntityModel
                    {
                        UserId = user.UserId,
                        UserName = user.UserName,
                        EmployeeId = user.EmployeeId.Value,
                        Token = token.Value,
                        LoginTime = new DateTime(),
                    };
                    return new LoginResult()
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = CommonMessage.Login.INACTIVE_USER,
                        CurrentUser = currentUserFail
                    };
                }

                bool isAdmin = user.IsAdmin == null ? false : user.IsAdmin.Value;
                var empId = user.EmployeeId;
                string userFullName = externalUser != null ? externalUser.UserName : "";
                string userAvatar = "";
                string userEmail = "";
                bool isManager = false;
                bool isOrder = false;
                bool isCashier = false;
                Guid? positionId = Guid.Empty;
                List<string> perCodeList = new List<string>();
                List<SystemParameter> systemParameterList = new List<SystemParameter>();
                List<string> listTextActionResource = new List<string>();
                var ListMenuBuild = new List<MenuBuildEntityModel>();
                string employeeCode = "";
                string employeeName = "";
                string employeeCodeName = "";
                Guid? roleId = Guid.Empty;

                var emp = context.Employee.FirstOrDefault(e => e.EmployeeId == empId);
                //Nếu là nhân viên
                if (emp != null)
                {
                    employeeCode = emp?.EmployeeCode;
                    employeeName = emp?.EmployeeName;
                    employeeCodeName = employeeCode + " - " + employeeName;
                    userFullName = emp?.EmployeeName;
                    userAvatar = context.Contact.FirstOrDefault(c => c.ObjectId == empId && c.ObjectType == "EMP")?
                        .AvatarUrl;
                    userEmail = context.Contact.FirstOrDefault(c => c.ObjectId == empId && c.ObjectType == "EMP")?.Email;
                    var manager = context.Employee.FirstOrDefault(e => e.EmployeeId == empId);
                    if (manager != null)
                    {
                        isManager = manager.IsManager;
                        positionId = manager.PositionId;
                        isOrder = manager.IsOrder ?? false;
                        isCashier = manager.IsCashier ?? false;
                    }

                    if (user != null)
                    {
                        var permissionSetOfUser = context.PermissionMapping.FirstOrDefault(pm => pm.UserId == user.UserId);
                        if (permissionSetOfUser != null)
                        {
                            var permissionSetOfUserId = permissionSetOfUser.PermissionSetId;
                            var permissionIdList = context.PermissionSet
                                .FirstOrDefault(ps => ps.PermissionSetId == permissionSetOfUserId).PermissionId.Split(";")
                                .ToList();
                            permissionIdList.ForEach(perId =>
                            {
                                if (!string.IsNullOrEmpty(perId))
                                {
                                    var perCode = context.Permission.FirstOrDefault(p => p.PermissionId == Guid.Parse(perId))
                                        .PermissionCode;
                                    perCodeList.Add(perCode);
                                }
                            });
                        }
                    }

                    systemParameterList = context.SystemParameter.ToList();

                    //Lấy list User Role
                    var listUserRole = context.UserRole.Where(e => e.UserId == user.UserId).ToList();
                    List<Guid> listRoleId = new List<Guid>();
                    if (listUserRole.Count > 0)
                    {
                        listUserRole.ForEach(item =>
                        {
                            listRoleId.Add(item.RoleId.Value);
                        });
                    }

                    //Lấy list Action Resource Id
                    var listActionResource =
                        context.RoleAndPermission.Where(e => listRoleId.Contains(e.RoleId.Value)).ToList();
                    List<Guid> listActionResourceId = new List<Guid>();
                    if (listActionResource.Count > 0)
                    {
                        listActionResource.ForEach(item =>
                        {
                            listActionResourceId.Add(item.ActionResourceId.Value);
                        });
                    }

                    //Lấy list text action resource
                    listTextActionResource = context.ActionResource
                        .Where(e => listActionResourceId.Contains(e.ActionResourceId)).Select(x => x.ActionResource1)
                        .ToList();

                    #region Lấy list MenuBuid

                    ListMenuBuild = context.MenuBuild.Where(x => x.IsShow == true).Select(y => new MenuBuildEntityModel
                    {
                        MenuBuildId = y.MenuBuildId,
                        ParentId = y.ParentId,
                        Name = y.Name,
                        Code = y.Code,
                        CodeParent = y.CodeParent,
                        Path = y.Path,
                        NameIcon = y.NameIcon,
                        Level = y.Level,
                        IndexOrder = y.IndexOrder,
                        IsPageDetail = y.IsPageDetail
                    }).OrderBy(z => z.IndexOrder).ToList();

                    //Lấy list đường dẫn mặc định gắn với nhóm quyền
                    //Nếu user được phân duy nhất 1 quyền
                    if (listRoleId.Count == 1)
                    {
                        roleId = listRoleId.FirstOrDefault();

                        var listRoleAndMenuBuild = context.RoleAndMenuBuild.Where(x => x.RoleId == roleId).ToList();
                        ListMenuBuild.ForEach(item =>
                        {
                            //Lấy ra các sub menu module
                            if (item.Level == 1)
                            {
                                var existsDefaultPath = listRoleAndMenuBuild.FirstOrDefault(x => x.Code == item.Code);

                                //Nếu tồn tại đường dẫn mặc định đã được cấu hình
                                if (existsDefaultPath != null)
                                {
                                    item.Path = existsDefaultPath.Path;
                                }
                            }
                        });
                    }

                    #endregion

                }
                //Nếu là KH
                else
                {
                    var cus = context.Customer.FirstOrDefault(e => e.CustomerId == empId);
                    employeeCode = cus?.CustomerCode;
                    employeeName = cus?.CustomerName;
                    employeeCodeName = employeeCode + " - " + employeeName;
                    userFullName = cus?.CustomerName;
                    var contact = context.Contact.FirstOrDefault(c => c.ObjectId == empId && c.ObjectType == "CUS");
                    userAvatar = contact?.AvatarUrl;
                    userEmail = contact?.Email;
                }
                var contactEntityModel = context.Contact
                              .Where(x => x.ObjectId == empId)
                              .Select(x => new ContactEntityModel
                              {
                                  Address = x.Address,
                                  Phone = x.Phone,
                                  Email = x.Email,
                                  LastName = x.LastName,
                                  FirstName = x.FirstName,
                                  Gender = x.Gender,
                                  DateOfBirth = x.DateOfBirth,
                                  ObjectType = x.ObjectType,
                                  ProvinceId = x.ProvinceId,
                              }).FirstOrDefault();

                var currentUser = new AuthEntityModel
                {
                    UserId = user.UserId,
                    UserName = user.UserName,
                    EmployeeId = user.EmployeeId.Value,
                    Token = token.Value,
                    LoginTime = new DateTime(),
                    PositionId = positionId
                };

                return new LoginResult
                {
                    StatusCode = HttpStatusCode.OK,
                    RoleId = roleId,
                    CurrentUser = currentUser,
                    UserFullName = userFullName,
                    UserAvatar = userAvatar,
                    UserEmail = userEmail,
                    IsManager = isManager,
                    PermissionList = perCodeList,
                    PositionId = positionId,
                    ListPermissionResource = listTextActionResource,
                    IsAdmin = isAdmin,
                    SystemParameterList = systemParameterList,
                    IsOrder = isOrder,
                    IsCashier = isCashier,
                    ListMenuBuild = ListMenuBuild,
                    EmployeeCode = employeeCode,
                    EmployeeName = employeeName,
                    EmployeeCodeName = employeeCodeName,
                    ContactEntityModel = contactEntityModel
                };
            }
            catch (Exception e)
            {
                return new LoginResult
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetUserPermissionResult GetUserPermission(GetUserPermissionParameter parameter)
        {
            try
            {
                //Lấy list User Role
                List<Guid> listRoleId = context.UserRole.Where(e => e.UserId == parameter.UserId)
                    .Select(y => y.RoleId.Value).ToList();

                //Lấy list Action Resource Id
                List<Guid> listActionResourceId = context.RoleAndPermission.Where(e => listRoleId.Contains(e.RoleId.Value))
                    .Select(y => y.ActionResourceId.Value).ToList();

                //Lấy list text action resource
                var _listActionResource = context.ActionResource
                    .Where(e => listActionResourceId.Contains(e.ActionResourceId)).Select(x => x.ActionResource1)
                    .ToList();

                string[] array = _listActionResource.ToArray();
                string listPermissionResource = String.Join(", ", array);

                return new GetUserPermissionResult()
                {
                    ListPermissionResource = listPermissionResource,
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success",
                };
            }
            catch (Exception e)
            {
                return new GetUserPermissionResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public CreatePermissionResult CreatePermission(CreatePermissionParameter parameter)
        {
            try
            {
                string permissionList = string.Join(";", parameter.PermissionIdList);

                parameter.PermissionIdList.ToList().ForEach(permissionId =>
                {
                    var permission = context.Permission.FirstOrDefault(p => p.PermissionId == Guid.Parse(permissionId) && p.ParentId == null);
                    if (permission != null)
                    {
                        var permissionAsMenu = context.Permission.Where(p => p.ParentId == Guid.Parse(permissionId) && p.Type == "S").Select(p => p.PermissionId).ToList();
                        string permissionAsMenuString = string.Join(";", permissionAsMenu);
                        permissionList += ";" + permissionAsMenuString;
                    }
                });

                PermissionSet ps = new PermissionSet()
                {
                    PermissionSetId = Guid.NewGuid(),
                    PermissionSetName = parameter.PermissionSetName.Trim(),
                    PermissionSetCode = parameter.PermissionSetCode.Trim(),
                    PermissionSetDescription = parameter.PermissionSetDescription.Trim(),
                    PermissionId = permissionList,
                    CreatedById = parameter.UserId,
                    CreatedDate = DateTime.Now,
                    Active = true
                };

                context.PermissionSet.Add(ps);
                context.SaveChanges();
                return new CreatePermissionResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = CommonMessage.Permission.CREATE_SUCCESS
                };
            }
            catch (Exception e)
            {
                return new CreatePermissionResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }

        }

        public EditPermissionByIdResult EditPermissionById(EditPermissionByIdParameter parameter)
        {
            try
            {
                var permission = context.PermissionSet.FirstOrDefault(p => p.PermissionSetId == parameter.PermissionSetId);
                permission.PermissionSetName = parameter.PermissionSetName.Trim();
                permission.PermissionSetDescription = parameter.PermissionSetDescription.Trim();
                permission.PermissionSetCode = parameter.PermissionSetCode.Trim();
                permission.PermissionId = string.Join(";", parameter.PermissionIdList.ToList());

                context.PermissionSet.Update(permission);
                context.SaveChanges();
                return new EditPermissionByIdResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = CommonMessage.Permission.EDIT_SUCCESS
                };
            }
            catch (Exception e)
            {
                return new EditPermissionByIdResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetAllPermissionResult GetAllPermission(GetAllPermissionParameter parameter)
        {
            try
            {
                var perLst = context.PermissionSet.Select(p => new PermissionSetEntityModel()
                {
                    PermissionSetId = p.PermissionSetId,
                    PermissionSetName = p.PermissionSetName,
                    PermissionSetCode = p.PermissionSetCode,
                    PermissionSetDescription = p.PermissionSetDescription,
                    NumberOfUserHasPermission = context.PermissionMapping.Where(pm => pm.PermissionSetId == p.PermissionSetId).Count(),
                    CreatedById = p.CreatedById,
                    CreatedDate = p.CreatedDate
                }).OrderByDescending(ps => ps.CreatedDate).ToList();

                return new GetAllPermissionResult
                {
                    PermissionList = perLst,
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success"
                };
            }
            catch (Exception e)
            {
                return new GetAllPermissionResult
                {
                    MessageCode = e.Message,
                    StatusCode = HttpStatusCode.ExpectationFailed
                };
            }
        }

        public GetPermissionByIdResult GetPermissionById(GetPermissionByIdParameter parameter)
        {
            try
            {
                var permissionSet = context.PermissionSet.FirstOrDefault(ps => ps.PermissionSetId == parameter.PermissionSetId);
                var permissionIdList = permissionSet.PermissionId.Split(";").ToList();
                List<PermissionEntityModel> permissionList = new List<PermissionEntityModel>();
                permissionIdList.ForEach(permissionId =>
                {
                    var permission = context.Permission.FirstOrDefault(p => p.PermissionId.ToString() == permissionId);

                    if (permission != null)
                    {
                        PermissionEntityModel pem = new PermissionEntityModel()
                        {
                            PermissionId = permission.PermissionId,
                            PermissionName = permission.PermissionName,
                            PermissionCode = permission.PermissionCode,
                            ParentId = permission.ParentId
                        };

                        permissionList.Add(pem);
                    }
                });

                PermissionSetEntityModel psem = new PermissionSetEntityModel()
                {
                    PermissionSetId = permissionSet.PermissionSetId,
                    PermissionSetName = permissionSet.PermissionSetName,
                    PermissionSetDescription = permissionSet.PermissionSetDescription,
                    PermissionSetCode = permissionSet.PermissionSetCode,
                    NumberOfUserHasPermission = context.PermissionMapping.Where(pm => pm.PermissionSetId == permissionSet.PermissionSetId).Count(),
                    PermissionList = permissionList
                };

                return new GetPermissionByIdResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success",
                    Permission = psem
                };
            }
            catch (Exception e)
            {
                return new GetPermissionByIdResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message,
                };
            }
        }

        public DeletePermissionByIdResult DeletePermissionById(DeletePermissionByIdParameter parameter)
        {
            try
            {
                var permissionMapping = context.PermissionMapping.Where(pm => pm.PermissionSetId == parameter.PermissionSetId).ToList();
                if (permissionMapping.Count > 0)
                {
                    return new DeletePermissionByIdResult()
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = CommonMessage.Permission.HAS_USER
                    };
                }

                var permission = context.PermissionSet.FirstOrDefault(p => p.PermissionSetId == parameter.PermissionSetId);
                if (permission == null)
                {
                    return new DeletePermissionByIdResult()
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = CommonMessage.Permission.NOT_EXIST
                    };
                }

                context.PermissionSet.Remove(permission);
                context.SaveChanges();
                return new DeletePermissionByIdResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = CommonMessage.Permission.DELETE_SUCCESS
                };
            }
            catch (Exception e)
            {
                return new DeletePermissionByIdResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public ChangePasswordResult ChangePassword(ChangePasswordParameter parameter)
        {
            try
            {
                var user = context.User.FirstOrDefault(u => u.UserId == parameter.UserId);
                var currentPass = user.Password;
                var oldPass = AuthUtil.GetHashingPassword(parameter.OldPassword);

                if (oldPass != currentPass)
                {
                    return new ChangePasswordResult()
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = CommonMessage.Password.NOT_CORRECT
                    };
                }

                var newPass = AuthUtil.GetHashingPassword(parameter.NewPassword);
                if (newPass == currentPass)
                {
                    return new ChangePasswordResult()
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = CommonMessage.Password.DUPLICATE
                    };
                }

                user.Password = newPass;

                context.User.Update(user);
                context.SaveChanges();

                return new ChangePasswordResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = CommonMessage.Password.CHANGE_SUCCESS
                };
            }
            catch (Exception e)
            {
                return new ChangePasswordResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public ChangePasswordResult ChangePasswordForgot(ChangePasswordForgotParameter parameter)
        {
            try
            {
                if(parameter.NewPassword.Trim() != parameter.ConfirmPassword.Trim())
                {
                    return new ChangePasswordResult
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        Message = "Mật khâu không khớp"
                    };
                }

                var user = context.User.FirstOrDefault(u => u.UserName == parameter.UserName);
                if (user == null)
                {
                    return new ChangePasswordResult
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        Message = "Không tồn tại tên đăng nhập"
                    };
                }

                if (user.MaXacThuc.ToString() != parameter.Code.Trim())
                {
                    return new ChangePasswordResult
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        Message = "Sai mã xác thực"
                    };
                }
                user.Password = AuthUtil.GetHashingPassword(parameter.NewPassword);

                context.User.Update(user);
                context.SaveChanges();

                return new ChangePasswordResult
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = "Đổi mật khẩu thành công"
                };
            }
            catch (Exception e)
            {
                return new ChangePasswordResult
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    Message = "Đổi mật khẩu thất bại"
                };
            }
        }

        public GetPermissionByCodeResult GetPermissionByCode(GetPermissionByCodeParameter parameter)
        {
            try
            {
                var parentPermission = context.Permission.Where(p => parameter.PerCode.IndexOf(p.PermissionCode) > -1).Select(parent => new PermissionEntityModel()
                {
                    PermissionId = parent.PermissionId,
                    PermissionName = parent.PermissionName,
                    PermissionChildList = context.Permission.Where(p => p.ParentId == parent.PermissionId && p.Type == "P").Select(p => new PermissionEntityModel()
                    {
                        PermissionId = p.PermissionId,
                        PermissionName = p.PermissionName,
                        PermissionCode = p.PermissionCode,
                        ParentId = p.ParentId,
                        PermissionDescription = p.PermissionDescription,
                        Sort = p.Sort
                    }).OrderBy(p => p.Sort).ToList()
                }).ToList();
                return new GetPermissionByCodeResult()
                {
                    PermissionList = parentPermission,
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success"
                };
            }
            catch (Exception e)
            {
                return new GetPermissionByCodeResult()
                {
                    MessageCode = e.Message,
                    StatusCode = HttpStatusCode.ExpectationFailed
                };
            }

        }

        private List<PermissionEntityModel> GetChildren(Guid? id, List<PermissionEntityModel> list)
        {
            return list.Where(p => p.ParentId == id)
                .Select(p => new PermissionEntityModel()
                {
                    PermissionId = p.PermissionId,
                    PermissionName = p.PermissionName,
                    PermissionCode = p.PermissionCode,
                    ParentId = p.ParentId,
                    PermissionDescription = p.PermissionDescription,
                    PermissionChildList = GetChildren(p.PermissionId, list)
                }).ToList();
        }

        public EditUserProfileResult EditUserProfile(EditUserProfileParameter parameter)
        {
            try
            {
                var user = context.User.FirstOrDefault(u => u.UserId == parameter.UserId);
                var employee = context.Employee.FirstOrDefault(e => e.EmployeeId == user.EmployeeId);
                var contact = context.Contact.FirstOrDefault(c => c.ObjectId == employee.EmployeeId);

                user.UserName = parameter.Username;
                contact.FirstName = parameter.FirstName;
                contact.LastName = parameter.LastName;
                contact.Email = parameter.Email;
                contact.AvatarUrl = parameter.AvatarUrl;

                context.User.Update(user);
                context.Contact.Update(contact);
                context.SaveChanges();
                return new EditUserProfileResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = CommonMessage.User.CHANGE_SUCCESS
                };
            }
            catch (Exception e)
            {
                return new EditUserProfileResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = e.Message
                };
            }

        }

        public GetUserProfileResult GetUserProfile(GetUserProfileParameter parameter)
        {
            try
            {
                var user = context.User.FirstOrDefault(u => u.UserId == parameter.UserId);
                var employee = context.Employee.FirstOrDefault(e => e.EmployeeId == user.EmployeeId);
                var contact = context.Contact.FirstOrDefault(c => c.ObjectId == employee.EmployeeId);
                return new GetUserProfileResult()
                {
                    Status = true,
                    AvatarUrl = contact.AvatarUrl,
                    Email = contact.Email,
                    FirstName = contact.FirstName,
                    LastName = contact.LastName,
                    Username = user.UserName,
                    MessageCode = "Success",
                    StatusCode = HttpStatusCode.OK
                };
            }
            catch (Exception e)
            {
                return new GetUserProfileResult()
                {
                    MessageCode = e.Message,
                    StatusCode = HttpStatusCode.ExpectationFailed
                };
            }
        }

        public GetUserProfileByRoomNameResult GetUserProfileByRoomName(GetUserProfileByRoomNameParameter parameter)
        {
            try
            {
                var listUserId = context.FireBase
                                .Where(x => parameter.ListRoomName.Any(y => y == x.RoomName))
                                .Select(x => new FireBaseEntityModel
                                {
                                    UserId = (Guid)(x.UserId == parameter.UserId ? x.OtherId : x.UserId)
                                }).ToList();

                var listUserProfile = context.Contact
                                     .Where(x => listUserId.Any(y => y.UserId == x.ObjectId))
                                     .Select(x => new GetUserProfileResult
                                     {
                                         ObjectId = x.ObjectId,
                                         AvatarUrl = x.AvatarUrl
                                     }).ToList();

                return new GetUserProfileByRoomNameResult()
                {
                    ListUserProfileResult = listUserProfile,
                    Message = "Success",
                    StatusCode = HttpStatusCode.OK
                };
            }
            catch (Exception e)
            {
                return new GetUserProfileByRoomNameResult()
                {
                    MessageCode = e.Message,
                    StatusCode = HttpStatusCode.ExpectationFailed
                };
            }
        }


        public GetUserProfileByEmailResult GetUserProfileByEmail(GetUserProfileByEmailParameter parameter)
        {
            try
            {
                var contact = context.Contact.FirstOrDefault(c => c.Email == parameter.UserEmail
            || c.OtherEmail == parameter.UserEmail
            || c.WorkEmail == parameter.UserEmail);
                var employee = context.Employee.FirstOrDefault(e => e.EmployeeId == contact.ObjectId);
                var user = context.User.FirstOrDefault(u => u.EmployeeId == employee.EmployeeId);

                if (user.Active == false)
                {
                    return new GetUserProfileByEmailResult()
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = CommonMessage.Login.INACTIVE_USER,
                        AvatarUrl = contact.AvatarUrl,
                        Email = contact.Email,
                        FirstName = contact.FirstName,
                        LastName = contact.LastName,
                        UserName = user.UserName,
                        FullName = employee.EmployeeName,
                        UserId = user.UserId,
                        EmployeeId = employee.EmployeeId,
                        PermissionList = new List<string>(),
                        IsManager = false
                    };
                }

                var empId = user.EmployeeId;
                bool isManager = false;
                Guid? positionId = Guid.Empty;
                List<string> perCodeList = new List<string>();
                if (empId != null)
                {
                    isManager = employee.IsManager;
                    positionId = employee.PositionId;

                    var permissionSetOfUser = context.PermissionMapping.FirstOrDefault(pm => pm.UserId == user.UserId);
                    if (permissionSetOfUser != null)
                    {
                        var permissionSetOfUserId = permissionSetOfUser.PermissionSetId;
                        var permissionIdList = context.PermissionSet.FirstOrDefault(ps => ps.PermissionSetId == permissionSetOfUserId).PermissionId.Split(";").ToList();
                        permissionIdList.ForEach(perId =>
                        {
                            if (!string.IsNullOrEmpty(perId))
                            {
                                var perCode = context.Permission.FirstOrDefault(p => p.PermissionId == Guid.Parse(perId)).PermissionCode;
                                var parentId = context.Permission.FirstOrDefault(p => p.PermissionId == Guid.Parse(perId)).ParentId;

                                if (parentId != null)
                                {
                                    var parentCode = context.Permission.FirstOrDefault(p => p.PermissionId == Guid.Parse(parentId.ToString())).PermissionCode;
                                    if (perCodeList.IndexOf(parentCode.ToString()) == -1)
                                    {
                                        perCodeList.Add(parentCode.ToString());
                                    }
                                }
                                perCodeList.Add(perCode);
                            }
                        });
                    }
                }
                this.iAuditTrace.Trace(ActionName.LOGIN, ObjectName.USER, $"User {user.UserName} login", user.UserId);

                return new GetUserProfileByEmailResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success",
                    AvatarUrl = contact.AvatarUrl,
                    Email = contact.Email,
                    FirstName = contact.FirstName,
                    LastName = contact.LastName,
                    UserName = user.UserName,
                    FullName = employee.EmployeeName,
                    UserId = user.UserId,
                    PermissionList = perCodeList,
                    IsManager = isManager,
                    PositionId = positionId,
                    EmployeeId = employee.EmployeeId,
                };
            }
            catch (Exception e)
            {
                return new GetUserProfileByEmailResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }

        }

        public GetModuleByPermissionSetIdResult GetModuleByPermissionSetId(GetModuleByPermissionSetIdParameter parameter)
        {
            try
            {
                var permissionSet = context.PermissionSet.FirstOrDefault(ps => ps.PermissionSetId == parameter.PermissionSetId);
                List<PermissionEntityModel> permissionList = new List<PermissionEntityModel>();
                if (permissionSet != null)
                {
                    var permissionIdList = permissionSet.PermissionId.Split(";").ToList();

                    if (permissionIdList.Count > 0)
                    {
                        permissionIdList.ForEach(perId =>
                        {
                            var parent = context.Permission.FirstOrDefault(p => p.PermissionId == Guid.Parse(perId) && p.ParentId == null);
                            if (parent != null)
                            {
                                PermissionEntityModel pm = new PermissionEntityModel()
                                {
                                    PermissionId = parent.PermissionId,
                                    PermissionName = parent.PermissionName
                                };
                                permissionList.Add(pm);
                            }
                        });
                    }
                }

                return new GetModuleByPermissionSetIdResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success",
                    PermissionListAsModule = permissionList
                };
            }
            catch (Exception e)
            {
                return new GetModuleByPermissionSetIdResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetAllPermissionSetNameAndCodeResult GetAllPermissionSetNameAndCode(
            GetAllPermissionSetNameAndCodeParameter parameter)
        {
            try
            {
                List<string> nameList = context.PermissionSet.Select(ps => ps.PermissionSetName.ToLower()).Distinct().ToList();
                List<string> codeList = context.PermissionSet.Select(ps => ps.PermissionSetCode.ToLower()).Distinct().ToList();

                return new GetAllPermissionSetNameAndCodeResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    PermissionSetNameList = nameList,
                    PermissionSetCodeList = codeList,
                    MessageCode = "Success"
                };
            }
            catch (Exception e)
            {
                return new GetAllPermissionSetNameAndCodeResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetAllUserResult GetAllUser(GetAllUserParameter parameter)
        {
            try
            {
                var ListUserObject = (from u in context.User
                                      join e in context.Employee on u.EmployeeId equals e.EmployeeId
                                      where e.Active == true
                                      select new UserEntityModel
                                      {
                                          UserId = u.UserId,
                                          UserName = e.EmployeeName,
                                          EmployeeId = e.EmployeeId
                                      }).ToList();

                return new GetAllUserResult
                {
                    MessageCode = "Sucess",
                    StatusCode = HttpStatusCode.OK,
                    lstUserEntityModel = ListUserObject
                };
            }
            catch (Exception ex)
            {

                return new GetAllUserResult
                {
                    MessageCode = ex.ToString(),
                    StatusCode = HttpStatusCode.ExpectationFailed
                };
            }
        }

        public GetCheckUserNameResult GetCheckUserName(GetCheckUserNameParameter parameter)
        {
            try
            {
                parameter.UserName = parameter.UserName.Trim();
                var user = context.User.FirstOrDefault(u => u.UserName == parameter.UserName);

                if (user == null)
                {
                    return new GetCheckUserNameResult()
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = CommonMessage.Login.USER_NOT_EXIST
                    };
                }

                if (user.Active == false)
                {
                    return new GetCheckUserNameResult()
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = CommonMessage.Login.INACTIVE_USER
                    };
                }

                //var emp = context.Employee.FirstOrDefault(e => e.EmployeeId == user.EmployeeId);
                var contact = context.Contact.FirstOrDefault(c => c.ObjectId == user.UserId);
                var Email = contact.Email;
                if (Email != null)
                {
                    Email = Email.Trim();
                }
                //if (string.IsNullOrEmpty(Email))
                //{
                //    Email = contact.WorkEmail.Trim();
                //    if (string.IsNullOrEmpty(Email))
                //    {
                //        Email = contact.OtherEmail.Trim();
                //    }
                //}
                //if (Email == null || Email.Trim() == "")
                //{

                //}
                if (string.IsNullOrEmpty(Email))
                {
                    return new GetCheckUserNameResult()
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = CommonMessage.Contact.EMAIL_DOES_NOT_EXIST
                    };
                }

                return new GetCheckUserNameResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success",
                    UserId = user.UserId,
                    UserName = user.UserName,
                    EmailAddress = Email
                };
            }
            catch (Exception e)
            {
                return new GetCheckUserNameResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetCheckResetCodeUserResult GetCheckResetCodeUser(GetCheckResetCodeUserParameter parameter)
        {
            try
            {
                var user = context.User.FirstOrDefault(u => u.ResetCode == parameter.Code);

                if (user == null)
                {
                    return new GetCheckResetCodeUserResult()
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = CommonMessage.Login.RESET_CODE_ERR
                    };
                }
                else if (user.Active == false)
                {
                    return new GetCheckResetCodeUserResult()
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = CommonMessage.Login.INACTIVE_USER
                    };
                }
                var curr_time = DateTime.Now;
                TimeSpan range_time = Convert.ToDateTime(curr_time) - Convert.ToDateTime(user.ResetCodeDate);
                int sum_day = range_time.Days;
                if (sum_day > 2)
                {
                    return new GetCheckResetCodeUserResult()
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = CommonMessage.Login.DATELINE_RESET_PASS
                    };
                }

                return new GetCheckResetCodeUserResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success",
                    UserId = user.UserId,
                    UserName = user.UserName
                };
            }
            catch (Exception e)
            {
                return new GetCheckResetCodeUserResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }

        }

        public ResetPasswordResult ResetPassword(ResetPasswordParameter parameter)
        {
            try
            {
                var user = context.User.FirstOrDefault(u => u.UserId == parameter.UserId);

                if (user == null)
                {
                    return new ResetPasswordResult()
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = CommonMessage.Login.USER_NOT_EXIST
                    };
                }
                else if (user.Active == false)
                {
                    return new ResetPasswordResult()
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = CommonMessage.Login.INACTIVE_USER
                    };
                }

                var currentPass = user.Password;
                var newPass = AuthUtil.GetHashingPassword(parameter.Password);

                if (currentPass == newPass)
                {
                    return new ResetPasswordResult()
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = CommonMessage.Password.DUPLICATE
                    };
                }

                user.ResetCode = null;
                user.ResetCodeDate = null;
                user.UpdatedById = parameter.UserId;
                user.UpdatedDate = DateTime.Now;
                user.Password = newPass;
                context.User.Update(user);
                context.SaveChanges();

                return new ResetPasswordResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success"
                };
            }
            catch (Exception e)
            {
                return new ResetPasswordResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetPositionCodeByPositionIdResult GetPositionCodeByPositionId(GetPositionCodeByPositionIdParameter parameter)
        {
            try
            {
                var position = context.Position.FirstOrDefault(p => p.PositionId == parameter.PositionId);
                var employee = context.Employee.FirstOrDefault(e => e.EmployeeId == parameter.EmployeeId);
                var organization = context.Organization.FirstOrDefault(o => o.OrganizationId == employee.OrganizationId);
                var OrganizationId = organization.OrganizationId;
                var OrganizationName = organization.OrganizationName;
                var organizationIdList = (from or in context.Organization
                                          select new
                                          {
                                              or.OrganizationId,
                                              or.OrganizationCode,
                                              or.OrganizationName
                                          }
                                          ).ToList();

                List<dynamic> lstResult = new List<dynamic>();
                organizationIdList.ForEach(item =>
                {
                    var sampleObject = new ExpandoObject() as IDictionary<string, Object>;
                    sampleObject.Add("OrganizationId", item.OrganizationId);
                    sampleObject.Add("OrganizationCode", item.OrganizationCode);
                    sampleObject.Add("OrganizationName", item.OrganizationName);
                    lstResult.Add(sampleObject);
                });

                if (position == null)
                {
                    return new GetPositionCodeByPositionIdResult()
                    {
                        StatusCode = HttpStatusCode.OK,
                        MessageCode = CommonMessage.Login.POSITION_NOT_EXIST
                    };
                }

                var PositionCode = position.PositionCode;
                if (string.IsNullOrEmpty(PositionCode))
                {
                    return new GetPositionCodeByPositionIdResult()
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = CommonMessage.Login.POSITION_NOT_EXIST
                    };
                }

                return new GetPositionCodeByPositionIdResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success",
                    PositionCode = PositionCode,
                    OrganizationId = OrganizationId,
                    OrganizationName = OrganizationName,
                    lstResult = lstResult
                };
            }
            catch (Exception e)
            {
                return new GetPositionCodeByPositionIdResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }

        }

        public GetAllRoleResult GetAllRole(GetAllRoleParameter parameter)
        {
            try
            {
                var listUserRole = context.UserRole.ToList();
                List<RoleEntityModel> listRoleResult = new List<RoleEntityModel>();
                var listRole = context.Role.ToList();
                if (listRole.Count > 0)
                {
                    listRole.ForEach(item =>
                    {
                        RoleEntityModel role = new RoleEntityModel();
                        role.RoleId = item.RoleId;
                        role.RoleValue = item.RoleValue;
                        role.Description = item.Description;
                        var userNumber = listUserRole.Where(e => e.RoleId == item.RoleId).ToList().Count();
                        role.UserNumber = userNumber;
                        listRoleResult.Add(role);
                    });
                }

                return new GetAllRoleResult
                {
                    ListRole = listRoleResult,
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Lấy dữ liệu thành công"
                };
            }
            catch (Exception e)
            {
                return new GetAllRoleResult
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetCreatePermissionResult GetCreatePermission(GetCreatePermissionParameter parameter)
        {
            try
            {
                List<ActionResourceEntityModel> listActionResourceEntityModel = new List<ActionResourceEntityModel>();

                var listActionResource = context.ActionResource.ToList();
                listActionResource.ForEach(item =>
                {
                    listActionResourceEntityModel.Add(new ActionResourceEntityModel(item));
                });

                var ListMenuBuild = new List<MenuBuildEntityModel>();

                #region Lấy Module_Mapping, Resource_Mapping, Action_Mapping

                GetPermissionMapping(out List<PermissionTempModel> module_Mapping,
                    out List<PermissionTempModel> resource_Mapping,
                    out List<PermissionTempModel> action_Mapping);

                #endregion

                return new GetCreatePermissionResult
                {
                    ListActionResource = listActionResourceEntityModel,
                    ListMenuBuild = ListMenuBuild,
                    Module_Mapping = module_Mapping,
                    Resource_Mapping = resource_Mapping,
                    Action_Mapping = action_Mapping,
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Lấy dữ liệu thành công"
                };
            }
            catch (Exception e)
            {
                return new GetCreatePermissionResult
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public CreateRoleAndPermissionResult CreateRoleAndPermission(CreateRoleAndPermissionParameter parameter)
        {
            try
            {
                //Tạo Role
                Role role = new Role();
                role.RoleId = Guid.NewGuid();
                role.RoleValue = parameter.RoleValue;
                role.Description = parameter.Description;
                context.Role.Add(role);

                //Lấy list ActionResourceId từ mảng
                var listActionResourceId = context.ActionResource
                    .Where(e => parameter.ListActionResource.Contains(e.ActionResource1)).ToList();

                if (listActionResourceId.Count > 0)
                {
                    List<RoleAndPermission> listRoleAndPermission = new List<RoleAndPermission>();
                    listActionResourceId.ForEach(item =>
                    {
                        RoleAndPermission roleAndPermission = new RoleAndPermission();
                        roleAndPermission.RoleAndPermissionId = Guid.NewGuid();
                        roleAndPermission.ActionResourceId = item.ActionResourceId;
                        roleAndPermission.RoleId = role.RoleId;
                        listRoleAndPermission.Add(roleAndPermission);
                    });
                    context.RoleAndPermission.AddRange(listRoleAndPermission);
                }

                #region Lưu đường dẫn mặc định của Sub menu module tương ứng với Role

                if (parameter.ListMenuBuild.Count > 0)
                {
                    var listAllMenuBuild = context.MenuBuild.ToList();

                    var ListRoleAndMenuBuild = new List<RoleAndMenuBuild>();
                    parameter.ListMenuBuild.ForEach(item =>
                    {
                        var RoleAndMenuBuild = new RoleAndMenuBuild();
                        RoleAndMenuBuild.RoleAndMenuBuildId = Guid.NewGuid();

                        var subMenu = listAllMenuBuild.FirstOrDefault(x => x.Code == item.Code);
                        RoleAndMenuBuild.MenuBuildId = subMenu.MenuBuildId;
                        RoleAndMenuBuild.RoleId = role.RoleId;
                        RoleAndMenuBuild.Code = item.Code;
                        RoleAndMenuBuild.Path = item.Path;

                        ListRoleAndMenuBuild.Add(RoleAndMenuBuild);
                    });

                    context.RoleAndMenuBuild.AddRange(ListRoleAndMenuBuild);
                }

                #endregion

                context.SaveChanges();

                return new CreateRoleAndPermissionResult
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Lưu thành công"
                };
            }
            catch (Exception e)
            {
                return new CreateRoleAndPermissionResult
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetDetailPermissionResult GetDetailPermission(GetDetailPermissionParameter parameter)
        {
            try
            {
                var role = context.Role.FirstOrDefault(e => e.RoleId == parameter.RoleId);
                if (role == null)
                {
                    return new GetDetailPermissionResult
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = "Không tồn tại nhóm quyền này"
                    };
                }

                List<ActionResourceEntityModel> listActionResourceEntityModel = new List<ActionResourceEntityModel>();
                List<ActionResourceEntityModel> listCurrentActionResourceEntityModel = new List<ActionResourceEntityModel>();
                var listActionResource = context.ActionResource.ToList();
                listActionResource.ForEach(item =>
                {
                    listActionResourceEntityModel.Add(new ActionResourceEntityModel(item));
                });
                List<RoleAndPermission> listRoleAndPermission = new List<RoleAndPermission>();
                listRoleAndPermission = context.RoleAndPermission.Where(e => e.RoleId == role.RoleId).ToList();

                if (listRoleAndPermission.Count > 0)
                {
                    List<Guid> listActionResourceId = new List<Guid>();
                    listRoleAndPermission.ForEach(item =>
                    {
                        if (item.ActionResourceId != Guid.Empty && item.ActionResourceId != null)
                        {
                            listActionResourceId.Add(item.ActionResourceId.Value);
                        }
                    });

                    if (listActionResourceId.Count > 0)
                    {
                        var listCurrentActionResource = context.ActionResource.Where(e => listActionResourceId.Contains(e.ActionResourceId)).ToList();
                        listCurrentActionResource.ForEach(item =>
                        {
                            listCurrentActionResourceEntityModel.Add(new ActionResourceEntityModel(item));
                        });
                    }
                }

                #region Lấy list MenuBuid -> Giang comment: ko sử dụng nữa

                var ListMenuBuild = new List<MenuBuildEntityModel>();

                #endregion

                #region Lấy Module_Mapping, Resource_Mapping, Action_Mapping

                GetPermissionMapping(out List<PermissionTempModel> module_Mapping,
                    out List<PermissionTempModel> resource_Mapping,
                    out List<PermissionTempModel> action_Mapping);

                #endregion

                return new GetDetailPermissionResult
                {
                    Role = role,
                    ListActionResource = listActionResourceEntityModel,
                    ListCurrentActionResource = listCurrentActionResourceEntityModel,
                    ListMenuBuild = ListMenuBuild,
                    Module_Mapping = module_Mapping,
                    Resource_Mapping = resource_Mapping,
                    Action_Mapping = action_Mapping,
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Lấy dữ liệu thành công"
                };
            }
            catch (Exception e)
            {
                return new GetDetailPermissionResult
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public EditRoleAndPermissionResult EditRoleAndPermission(EditRoleAndPermissionParameter parameter)
        {
            try
            {
                var role = context.Role.FirstOrDefault(e => e.RoleId == parameter.RoleId);
                if (role == null)
                {
                    return new EditRoleAndPermissionResult
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = "Không tồn tại nhóm quyền"
                    };
                }
                role.RoleValue = parameter.RoleValue;
                role.Description = parameter.Description;
                context.Update(role);

                var listActionResource = context.ActionResource.Where(e => parameter.ListActionResource.Contains(e.ActionResource1)).ToList();
                List<Guid> listActionResourceId = new List<Guid>();
                if (listActionResource.Count > 0)
                {
                    listActionResource.ForEach(item =>
                    {
                        listActionResourceId.Add(item.ActionResourceId);
                    });
                }
                //Xóa trong bảng RoleAndPermission
                var listRoleAndPermissionOld = context.RoleAndPermission.Where(e => e.RoleId == role.RoleId).ToList();
                context.RoleAndPermission.RemoveRange(listRoleAndPermissionOld);

                //Thêm mới trong bảng RoleAndPermission
                List<RoleAndPermission> listRoleAndPermission = new List<RoleAndPermission>();
                if (listActionResourceId.Count > 0)
                {
                    listActionResourceId.ForEach(item =>
                    {
                        RoleAndPermission roleAndPermission = new RoleAndPermission();
                        roleAndPermission.RoleAndPermissionId = Guid.NewGuid();
                        roleAndPermission.ActionResourceId = item;
                        roleAndPermission.RoleId = role.RoleId;
                        listRoleAndPermission.Add(roleAndPermission);
                    });
                    context.RoleAndPermission.AddRange(listRoleAndPermission);
                }

                #region Lưu đường dẫn mặc định của Sub menu module tương ứng với Role

                //Xóa dữ liệu mặc định cũ
                var listOldRoleAndMenuBuild =
                    context.RoleAndMenuBuild.Where(x => x.RoleId == parameter.RoleId).ToList();
                context.RoleAndMenuBuild.RemoveRange(listOldRoleAndMenuBuild);

                if (parameter.ListMenuBuild.Count > 0)
                {
                    var listAllMenuBuild = context.MenuBuild.ToList();

                    var ListRoleAndMenuBuild = new List<RoleAndMenuBuild>();
                    parameter.ListMenuBuild.ForEach(item =>
                    {
                        var RoleAndMenuBuild = new RoleAndMenuBuild();
                        RoleAndMenuBuild.RoleAndMenuBuildId = Guid.NewGuid();

                        var subMenu = listAllMenuBuild.FirstOrDefault(x => x.Code == item.Code);
                        RoleAndMenuBuild.MenuBuildId = subMenu.MenuBuildId;
                        RoleAndMenuBuild.RoleId = role.RoleId;
                        RoleAndMenuBuild.Code = item.Code;
                        RoleAndMenuBuild.Path = item.Path;

                        ListRoleAndMenuBuild.Add(RoleAndMenuBuild);
                    });

                    context.RoleAndMenuBuild.AddRange(ListRoleAndMenuBuild);
                }

                #endregion

                context.SaveChanges();

                return new EditRoleAndPermissionResult
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Lưu thành công"
                };
            }
            catch (Exception e)
            {
                return new EditRoleAndPermissionResult
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public AddUserRoleResult AddUserRole(AddUserRoleParameter parameter)
        {
            try
            {
                var user = context.User.FirstOrDefault(e => e.EmployeeId == parameter.EmployeeId);
                var role = context.Role.FirstOrDefault(e => e.RoleId == parameter.RoleId);
                if (user == null)
                {
                    return new AddUserRoleResult
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = "Không tồn tại User này trên hệ thống"
                    };
                }
                if (role == null)
                {
                    return new AddUserRoleResult
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = "Không tồn tại Nhóm quyền này trên hệ thống"
                    };
                }
                var listUserRoleOld = context.UserRole.Where(e => e.UserId == user.UserId).ToList();
                if (listUserRoleOld.Count > 0)
                {
                    context.UserRole.RemoveRange(listUserRoleOld);
                }

                //Add lại role cho user
                //Hiện tại chỉ là 1:1
                UserRole userRole = new UserRole();
                userRole.UserRoleId = Guid.NewGuid();
                userRole.UserId = user.UserId;
                userRole.RoleId = role.RoleId;
                context.UserRole.Add(userRole);
                context.SaveChanges();

                return new AddUserRoleResult
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Lưu thành công"
                };
            }
            catch (Exception e)
            {
                return new AddUserRoleResult
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public DeleteRoleResult DeleteRole(DeleteRoleParameter parameter)
        {
            try
            {
                if (parameter.RoleId == new Guid("214d7013-4940-4101-9393-d500988a8ca0") ||
                    parameter.RoleId == new Guid("a749f1a7-1cbe-41c2-a1a7-2c035db603ca") ||
                    parameter.RoleId == new Guid("b21aa322-1404-4591-b856-0a0ac36e8c82") ||
                    parameter.RoleId == new Guid("a37c9152-478d-417f-b60f-e74403966fe5")
                    )
                {
                    return new DeleteRoleResult
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = "Nhóm quyền này mặc định. Không thể thực hiện thao tác xóa!"
                    };
                }

                //Delete Role
                var role = context.Role.FirstOrDefault(e => e.RoleId == parameter.RoleId);
                if (role == null)
                {
                    return new DeleteRoleResult
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = "Nhóm quyền này không tồn tại"
                    };
                }
                context.Role.Remove(role);

                //Delete RoleAndPermission
                List<RoleAndPermission> listRoleAndPermission = new List<RoleAndPermission>();
                listRoleAndPermission = context.RoleAndPermission.Where(e => e.RoleId == parameter.RoleId).ToList();
                if (listRoleAndPermission.Count > 0)
                {
                    context.RoleAndPermission.RemoveRange(listRoleAndPermission);
                }

                //Delete User Role
                List<UserRole> listUserRole = new List<UserRole>();
                listUserRole = context.UserRole.Where(e => e.RoleId == parameter.RoleId).ToList();
                if (listUserRole.Count > 0)
                {
                    context.UserRole.RemoveRange(listUserRole);
                }

                context.SaveChanges();
                return new DeleteRoleResult
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Xóa nhóm quyền thành công"
                };
            }
            catch (Exception e)
            {
                return new DeleteRoleResult
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        private void GetPermissionMapping(out List<PermissionTempModel> module_Mapping,
         out List<PermissionTempModel> resource_Mapping, out List<PermissionTempModel> action_Mapping)
        {
            module_Mapping = new List<PermissionTempModel>()
            {
                new PermissionTempModel()
                {
                    Key = "sys",
                    Name = "Module Quản lý hệ thống"
                },
                new PermissionTempModel()
                {
                    Key = "crm",
                    Name = "Module Quản trị quan hệ khách hàng"
                },
                new PermissionTempModel()
                {
                    Key = "sal",
                    Name = "Module Quản lý dịch vụ"
                },
                new PermissionTempModel()
                {
                    Key = "buy",
                    Name = "Module Quản lý nhà cung cấp"
                },
                new PermissionTempModel()
                {
                    Key = "hrm",
                    Name = "Module Quản trị nhân sự"
                },
                 new PermissionTempModel()
                {
                    Key = "cusOrder",
                    Name = "Module Quản trị đặt dịch vụ"
                },
            };

            resource_Mapping = new List<PermissionTempModel>()
            {
                new PermissionTempModel()
                {
                    Key = "customer/create",
                    Name = "Tạo mới khách hàng"
                },
                new PermissionTempModel()
                {
                    Key = "customer/list",
                    Name = "Tìm kiếm khách hàng"
                },
                new PermissionTempModel()
                {
                    Key = "customer/detail",
                    Name = "Chi tiết khách hàng"
                },
                new PermissionTempModel()
                {
                    Key = "admin/list-product-category",
                    Name = "Quản lý danh mục"
                },
                new PermissionTempModel()
                {
                    Key = "order/create",
                    Name = "Tạo mới phiếu yêu cầu dịch vụ"
                },
                new PermissionTempModel()
                {
                    Key = "order/list",
                    Name = "Tìm kiếm phiếu yêu cầu dịch vụ"
                },
                new PermissionTempModel()
                {
                    Key = "employee/create",
                    Name = "Tạo mới nhân viên"
                },
                new PermissionTempModel()
                {
                    Key = "employee/list",
                    Name = "Tìm kiếm nhân viên"
                },
                new PermissionTempModel()
                {
                    Key = "employee/employee-quit-work",
                    Name = "Danh sách nhân viên đã nghỉ việc"
                },
                new PermissionTempModel()
                {
                    Key = "employee/detail",
                    Name = "Chi tiết nhân viên"
                },
                new PermissionTempModel()
                {
                    Key = "vendor/create",
                    Name = "Tạo mới nhà cung cấp"
                },
                new PermissionTempModel()
                {
                    Key = "vendor/list",
                    Name = "Tìm kiếm nhà cung cấp"
                },
                new PermissionTempModel()
                {
                    Key = "vendor/detail",
                    Name = "Xem chi tiết nhà cung cấp"
                },
                
                new PermissionTempModel()
                {
                    Key = "admin/company-config",
                    Name = "Cấu hình hệ thống"
                },
                new PermissionTempModel()
                {
                    Key = "admin/system-parameter",
                    Name = "Tham số hệ thống"
                },
                new PermissionTempModel()
                {
                    Key = "admin/organization",
                    Name = "Quản lý tổ chức"
                },
                new PermissionTempModel()
                {
                    Key = "admin/masterdata",
                    Name = "Quản lý danh mục dữ liệu"
                },
                new PermissionTempModel()
                {
                    Key = "admin/permission",
                    Name = "Quản lý nhóm quyền"
                },
                new PermissionTempModel()
                {
                    Key = "admin/permission-create",
                    Name = "Tạo mới nhóm quyền"
                },
                new PermissionTempModel()
                {
                    Key = "admin/permission-detail",
                    Name = "Xem chi tiết thông tin nhóm quyền"
                },
                 new PermissionTempModel()
                {
                    Key = "order/orderProcess",
                    Name = "Tạo mới quy trình dịch vụ"
                },
                   new PermissionTempModel()
                {
                    Key = "order/orderProcessList",
                    Name = "Tìm kiếm quy trình dịch vụ"
                },
                    new PermissionTempModel()
                {
                    Key = "order/orderAction",
                    Name = "Tạo mới phiếu hỗ trợ"
                },
                   new PermissionTempModel()
                {
                    Key = "order/orderActionList",
                    Name = "Tìm kiếm phiếu hỗ trợ"
                },

                   new PermissionTempModel()
                {
                    Key = "product/product-option-detail",
                    Name = "Tạo mới tùy chọn dịch vụ"
                },
                   new PermissionTempModel()
                {
                    Key = "product/product-option-list",
                    Name = "Tìm kiếm tùy chọn dịch vụ"
                },
                   new PermissionTempModel()
                {
                    Key = "product/product-packet-createOrUpdate",
                    Name = "Tạo mới gói dịch vụ"
                },
                   new PermissionTempModel()
                {
                    Key = "product/list-product-packet",
                    Name = "Danh sách gói dịch vụ"
                },
            };

            action_Mapping = new List<PermissionTempModel>()
            {
                new PermissionTempModel()
                {
                    Key = "view",
                    Name = "Xem"
                },
                new PermissionTempModel()
                {
                    Key = "add",
                    Name = "Thêm"
                },
                new PermissionTempModel()
                {
                    Key = "edit",
                    Name = "Sửa"
                },
                new PermissionTempModel()
                {
                    Key = "delete",
                    Name = "Xóa"
                },
                new PermissionTempModel()
                {
                    Key = "import",
                    Name = "Tải lên"
                },
                new PermissionTempModel()
                {
                    Key = "download",
                    Name = "Tải xuống"
                },
                new PermissionTempModel()
                {
                    Key = "sms",
                    Name = "Gửi sms"
                },
                new PermissionTempModel()
                {
                    Key = "email",
                    Name = "Gửi email"
                },
                new PermissionTempModel()
                {
                    Key = "approve",
                    Name = "Phê duyệt"
                },
                new PermissionTempModel()
                {
                    Key = "send_approve",
                    Name = "Gửi phê duyệt"
                },
                new PermissionTempModel()
                {
                    Key = "reject",
                    Name = "Từ chối"
                },
                new PermissionTempModel()
                {
                    Key = "confirm",
                    Name = "Xác nhận"
                },
                new PermissionTempModel()
                {
                    Key = "re-pass",
                    Name = "Đặt lại mật khẩu"
                },
                new PermissionTempModel()
                {
                    Key = "delete-file",
                    Name = "Xóa File"
                },
                new PermissionTempModel()
                {
                    Key = "payment",
                    Name = "Thanh toán"
                },
                new PermissionTempModel()
                {
                    Key = "editnote",
                    Name = "Sửa/Xóa ghi chú"
                },
                new PermissionTempModel()
                {
                    Key = "copyscope",
                    Name = "Copy hạng mục"
                },
                new PermissionTempModel()
                {
                    Key = "cap-phat",
                    Name = "Cấp phát"
                },
                new PermissionTempModel()
                {
                    Key = "thu-hoi",
                    Name = "Thu hồi"
                },
                new PermissionTempModel()
                {
                    Key = "complete",
                    Name = "Hoàn thành"
                },
                 new PermissionTempModel()
                {
                    Key = "action",
                    Name = "Kích hoạt"
                },
            };
        }

        public RegisterResult Register(RegisterParameter parameter)
        {
            try
            {
                if(parameter.CustomerId == null)
                {
                    User user = new User();
                    user.UserId = Guid.NewGuid();

                    Customer customer = new Customer();
                    customer.CustomerId = Guid.NewGuid();
                    customer.CustomerName = parameter.FirstNameLastName;
                    customer.CustomerCode = GenerateCustomerCode();
                    customer.CustomerType = 2;
                    customer.StatusId = context.Category.FirstOrDefault(c => c.CategoryCode == "HDO").CategoryId;
                    customer.Active = true;
                    customer.CreatedDate = DateTime.Now;
                    customer.CreatedById = user.UserId;
                    var duplicateCustomer = context.Contact.FirstOrDefault(x => x.Phone == parameter.PhoneNumber.Trim());
                    if (duplicateCustomer != null)
                    {
                        return new RegisterResult
                        {
                            StatusCode = HttpStatusCode.ExpectationFailed,
                            MessageCode = "Tên người dùng đã tồn tại"
                        };
                    }
                    context.Customer.Add(customer);

                    user.EmployeeId = customer.CustomerId;
                    user.UserName = parameter.PhoneNumber;
                    user.Password = AuthUtil.GetHashingPassword(parameter.Password);
                    user.CreatedDate = DateTime.Now;
                    user.CreatedById = user.UserId;
                    user.Active = true;
                    var duplicateUser = context.User.FirstOrDefault(x => x.UserName == parameter.PhoneNumber.Trim());
                    if (duplicateUser != null)
                    {
                        return new RegisterResult
                        {
                            StatusCode = HttpStatusCode.ExpectationFailed,
                            MessageCode = "Tên tài khoản đã tồn tại"
                        };
                    }
                    context.User.Add(user);

                    Contact contact = new Contact();
                    contact.ContactId = Guid.NewGuid();
                    contact.FirstName = GetFirstName(parameter.FirstNameLastName.Trim());
                    contact.LastName = GetLastName(parameter.FirstNameLastName.Trim());
                    contact.Gender = parameter.Gender.Trim();
                    contact.Email = parameter.Email;
                    contact.ObjectId = customer.CustomerId;
                    contact.AvatarUrl = parameter.AvatarUrl;
                    contact.ObjectType = ObjectType.CUSTOMER;
                    contact.CreatedDate = DateTime.Now;
                    contact.CreatedById = user.UserId;
                    contact.Address = parameter.Address;
                    contact.ProvinceId = parameter.ProvinceId;
                    contact.Phone = parameter.PhoneNumber;

                    if(parameter.UserId != null && parameter.UserId != Guid.Empty && parameter.CustomerId != null && parameter.CustomerId != Guid.Empty)
                    {
                        var existUser = context.User.FirstOrDefault(x => x.UserName == parameter.PhoneNumber);
                        existUser.UserName = parameter.PhoneNumber;
                        context.User.AddRange(existUser);
                    }

                    contact.Active = true;
                    var duplicatePhone = context.Contact.FirstOrDefault(x => x.Phone == parameter.PhoneNumber.Trim());
                    if (duplicatePhone != null)
                    {
                        return new RegisterResult
                        {
                            StatusCode = HttpStatusCode.ExpectationFailed,
                            MessageCode = "Trùng số điện thoại"
                        };
                    }

                    var duplicateEmail = context.Contact.FirstOrDefault(x => x.Email == parameter.Email.Trim());
                    if (duplicateEmail != null)
                    {
                        return new RegisterResult
                        {
                            StatusCode = HttpStatusCode.ExpectationFailed,
                            MessageCode = "Trùng email"
                        };
                    }
                    context.Contact.Add(contact);
                }
                else
                {
                    var customer = context.Customer.FirstOrDefault(x => x.CustomerId == parameter.CustomerId);
                    if (customer == null)
                    {
                        return new RegisterResult
                        {
                            StatusCode = HttpStatusCode.ExpectationFailed,
                            MessageCode = "Khách hàng không tồn tại trên hệ thống!"
                        };
                    }

                    customer.CustomerName = parameter.FirstNameLastName;
                    customer.CustomerType = 2;
                    customer.StatusId = context.Category.FirstOrDefault(c => c.CategoryCode == "HDO").CategoryId;
                    customer.Active = true;
                    customer.UpdatedDate = DateTime.Now;
                    customer.UpdatedById = parameter.UserId;
                    context.Customer.Update(customer);

                    Contact contact = context.Contact.FirstOrDefault(x => x.ObjectId == customer.CustomerId);
                    if (contact == null)
                    {
                        return new RegisterResult
                        {
                            StatusCode = HttpStatusCode.ExpectationFailed,
                            MessageCode = "Không tồn tại contact của khách hàng!"
                        };
                    }

                    contact.FirstName = GetFirstName(parameter.FirstNameLastName.Trim());
                    contact.LastName = GetLastName(parameter.FirstNameLastName.Trim());
                    contact.Gender = parameter.Gender.Trim();
                    contact.Email = parameter.Email;
                    contact.ObjectId = customer.CustomerId;
                    contact.AvatarUrl = parameter.AvatarUrl;
                    contact.ObjectType = ObjectType.CUSTOMER;
                    contact.UpdatedDate = DateTime.Now;
                    contact.UpdatedById = parameter.UserId;
                    contact.Address = parameter.Address;
                    contact.ProvinceId = parameter.ProvinceId;
                    contact.Phone = parameter.PhoneNumber;
                    contact.Active = true;
                    var duplicatePhone = context.Contact.FirstOrDefault(x => x.Phone == parameter.PhoneNumber.Trim() && x.ObjectId != customer.CustomerId);
                    if (duplicatePhone != null)
                    {
                        return new RegisterResult
                        {
                            StatusCode = HttpStatusCode.ExpectationFailed,
                            MessageCode = "Trùng số điện thoại"
                        };
                    }

                    var duplicateEmail = context.Contact.FirstOrDefault(x => x.Email == parameter.Email.Trim() && x.ObjectId != customer.CustomerId);
                    if (duplicateEmail != null)
                    {
                        return new RegisterResult
                        {
                            StatusCode = HttpStatusCode.ExpectationFailed,
                            MessageCode = "Trùng email"
                        };
                    }
                    context.Contact.Update(contact);
                }

                context.SaveChanges();
                return new RegisterResult
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = parameter.CustomerId != null ? "Cập nhật thông tin thành công!": "Đăng ký thành công!"
                };
            }
            catch (Exception e)
            {
                return new RegisterResult
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    Message = e.Message
                };
            }
        }

        private string GetLastName(string fullName)
        {
            var parts = fullName.Split(' ');
            var lastName = parts.LastOrDefault();
            return lastName;
        }

        private string GetFirstName(string fullName)
        {
            var parts = fullName.Split(' ');
            var firstName = string.Join(" ", parts.Take(parts.Length - 1));
            return firstName;
        }

        private string GenerateCustomerCode()
        {
            var customerCode = "00001";
            var lastCustomerCode = context.Customer.LastOrDefault().CustomerCode;
            var isNumeric = int.TryParse(lastCustomerCode, out _);
            if (isNumeric)
            {
                var count = lastCustomerCode.Count();
                customerCode = (Int32.Parse(lastCustomerCode) + 1).ToString($"D{count}");
            }
            return customerCode;
        }

        public GetListProvinceResult TakeListProvince(GetListProvinceParameter parameter)
        {
            try
            {
                var listProvince = context.Province
                               .Select(x => new ProvinceEntityModel
                               {
                                   ProvinceId = x.ProvinceId,
                                   ProvinceName = x.ProvinceName,
                                   ProvinceCode = x.ProvinceCode,
                               }).ToList();

                return new GetListProvinceResult
                {
                    StatusCode = HttpStatusCode.OK,
                    ListProvinceEntityModel = listProvince
                };
            }
            catch (Exception e)
            {
                return new GetListProvinceResult
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    Message = e.Message
                };
            }
        }
        public RemoveDeviceIdResult RemoveDeviceId(RemoveDeviceIdParameter parameter)
        {
            try
            {
                var user = context.User.FirstOrDefault(x => x.UserId == parameter.UserId);
                if(user == null)
                {
                    return new RemoveDeviceIdResult
                    {
                        StatusCode = HttpStatusCode.OK,
                        MessageCode = "Người dùng không tồn tại trên hệ thống!"
                    };
                }
                user.DeviceId = null;
                context.User.Update(user);
                context.SaveChanges();

                return new RemoveDeviceIdResult
                {
                    StatusCode = HttpStatusCode.OK,
                };
            }
            catch (Exception e)
            {
                return new RemoveDeviceIdResult
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    Message = e.Message
                };
            }
        }
    }
}
