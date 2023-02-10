using System;
using System.Linq;
using TN.TNM.Common;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.Admin;
using TN.TNM.DataAccess.Messages.Parameters.Admin.AuditTrace;
using TN.TNM.DataAccess.Messages.Results.Admin;
using TN.TNM.DataAccess.Messages.Results.Admin.AuditTrace;
using TN.TNM.DataAccess.Models.AuditTrace;
using TN.TNM.DataAccess.Models.Employee;
using TN.TNM.DataAccess.Models.User;

namespace TN.TNM.DataAccess.Databases.DAO
{
    public class AuditTraceDAO : BaseDAO, IAuditTraceDataAccess
    {
        public AuditTraceDAO(TNTN8Context _content)
        {
            this.context = _content;
        }

        public void Trace(string actionName, string objectName, string description, Guid createById)
        {
            //var trace = new AuditTrace
            //{                
            //    ActionName = actionName,
            //    ObjectName = objectName,
            //    CreatedById = createById,
            //    CreatedDate = DateTime.Now,
            //    Description = description
            //};
            //this.context.Add(trace);
            //this.context.SaveChanges();
        }

        public GetMasterDataTraceResult GetMasterDataTrace(GetMasterDataTraceParameter parameter)
        {
            try
            {
                // common
                var listAllUser = context.User.ToList();
                var listAllEmp = context.Employee.ToList();

                #region Lấy danh sách nhân viên

                var lstEmp = listAllEmp.Select(item => new EmployeeEntityModel
                {
                    EmployeeId = item.EmployeeId,
                    EmployeeName = item.EmployeeName,
                    EmployeeCodeName = item.EmployeeCode + " - " + item.EmployeeName,
                }).ToList();

                #endregion

                #region Lấy danh sách user

                var lstUser = listAllUser.Select(item => new UserEntityModel()
                {
                    UserId = item.UserId,
                    UserName = item.UserName,
                }).ToList();

                #endregion

                return new GetMasterDataTraceResult()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = "Success",
                    ListEmp = lstEmp,
                    ListUser = lstUser,
                };
            }
            catch (Exception e)
            {
                return new GetMasterDataTraceResult()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public SearchTraceResult SearchTrace(SearchTraceParameter parameter)
        {
            try
            {
                // common
                var listAllLoginTraces = context.LoginAuditTrace.ToList();
                var listAllAuditTraces = context.AuditTrace.ToList();
                var listAllUser = context.User.ToList();
                var listAllEmp = context.Employee.ToList();
                int pageSize = parameter.PageSize;
                int pageIndex = parameter.PageIndex;
                int totalRecordsLoginTrace = 0;
                int totalRecordsAuditTrace = 0;

                #region Nhật ký đăng nhập

                var listLoginTrace = listAllLoginTraces.Select(item => new LoginTraceEntityModel()
                {
                    UserId = item.UserId,
                    LoginDate = item.LoginDate,
                    StatusCode = item.Status,
                }).OrderByDescending(w => w.LoginDate).ToList();

                listLoginTrace.ForEach(item =>
                {
                    var user = listAllUser.FirstOrDefault(x => x.UserId == item.UserId);
                    var emp = listAllEmp.FirstOrDefault(x => user != null && x.EmployeeId == user.EmployeeId);

                    if (user != null)
                    {

                        item.UserName = user.UserName;
                        item.EmployeeId = user.EmployeeId;
                        if (emp != null) item.EmployeeCode = emp.EmployeeCode;
                        if (emp != null) item.EmployeeName = emp.EmployeeName;

                    }
                });

                /* FILTER */
                if (parameter.IsSelectedLoginAudit)
                {
                    listLoginTrace = listLoginTrace.Where(x =>
                            (parameter.ListUserId.Count == 0 || parameter.ListUserId.Contains(x.UserId)) &&
                            (parameter.ListEmployeeId.Count == 0 || parameter.ListEmployeeId.Contains(x.EmployeeId)) &&
                            (parameter.ListStatus.Count == 0 || parameter.ListStatus.Contains((int)x.StatusCode)) &&
                            (parameter.SearchDate == null || parameter.SearchDate.Value.Date == x.LoginDate.Value.Date))
                        .ToList();
                }

                
                totalRecordsLoginTrace = listLoginTrace.Count;

                listLoginTrace = listLoginTrace
                    .Skip(pageSize * (pageIndex - 1))
                    .Take(pageSize).ToList();

                #endregion

                #region Nhật ký hệ thống

                var listAuditTrace = listAllAuditTraces.Select(item => new TraceEntityModel()
                {
                    UserId = item.CreatedById,
                    UserName = listAllUser.FirstOrDefault(x => x.UserId == item.CreatedById)?.UserName,
                    CreateDate = item.CreatedDate,
                    Description = item.Description,
                    ActionType = item.ActionName,
                    ObjectName = item.ObjectName
                }).OrderByDescending(y => y.CreateDate).ToList();

                listAuditTrace.ForEach(item =>
                {
                    var user = listAllUser.FirstOrDefault(x => x.UserId == item.UserId);
                    var emp = listAllEmp.FirstOrDefault(x => user != null && x.EmployeeId == user.EmployeeId);

                    if (user != null)
                    {
                        item.UserName = user.UserName;
                        if (emp != null) item.EmployeeName = emp.EmployeeName;
                        if (emp != null) item.EmployeeId = emp.EmployeeId;
                    }

                    if (item.ActionType == ActionName.Create.ToUpper())
                    {
                        item.ActionName = "Tạo mới";
                    }
                    else if (item.ActionType == ActionName.UPDATE.ToUpper())
                    {
                        item.ActionName = "Sửa";
                    }
                    else if (item.ActionType == ActionName.DELETE.ToUpper())
                    {
                        item.ActionName = "Xóa";
                    }
                });

                if (parameter.IsSelectedAuditTrace)
                {
                    listAuditTrace = listAuditTrace.Where(x =>
                            (parameter.ListUserId.Count == 0 || parameter.ListUserId.Contains(x.UserId)) &&
                            (parameter.ListEmployeeId.Count == 0 || parameter.ListEmployeeId.Contains(x.EmployeeId)) &&
                            (parameter.ListActionName.Count == 0 || parameter.ListActionName.Contains(x.ActionType)) &&
                            (parameter.ListObjectType.Count == 0 || parameter.ListObjectType.Contains(x.ObjectName)) &&
                            (parameter.SearchDate == null || parameter.SearchDate.Value.Date == x.CreateDate.Date))
                        .ToList();
                }

                totalRecordsAuditTrace = listAuditTrace.Count;

                listAuditTrace = listAuditTrace
                    .Skip(pageSize * (pageIndex - 1))
                    .Take(pageSize).ToList();

                #endregion

                return new SearchTraceResult()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = "Success",
                    ListAuditTrace = listAuditTrace,
                    ListLoginTrace = listLoginTrace,
                    TotalRecordsLoginTrace = totalRecordsLoginTrace,
                    TotalRecordsAuditTrace = totalRecordsAuditTrace,
                };
            }
            catch (Exception e)
            {
                return new SearchTraceResult()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }
    }
}
