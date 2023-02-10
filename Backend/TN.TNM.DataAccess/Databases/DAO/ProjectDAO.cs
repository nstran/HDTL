using KellermanSoftware.CompareNetObjects;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using TN.TNM.Common;
using TN.TNM.Common.NotificationSetting;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Helper;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.Note;
using TN.TNM.DataAccess.Messages.Parameters.Project;
using TN.TNM.DataAccess.Messages.Parameters.Task;
using TN.TNM.DataAccess.Messages.Results.Note;
using TN.TNM.DataAccess.Messages.Results.Project;
using TN.TNM.DataAccess.Messages.Results.Task;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Contract;
using TN.TNM.DataAccess.Models.Customer;
using TN.TNM.DataAccess.Models.Employee;
using TN.TNM.DataAccess.Models.Folder;
using TN.TNM.DataAccess.Models.Note;
using TN.TNM.DataAccess.Models.Project;
using TN.TNM.DataAccess.Models.Task;
using TN.TNM.DataAccess.Models.Vendor;
using System.Net;
using TN.TNM.DataAccess.Helper;
using TN.TNM.DataAccess.Models.DynamicColumnTable;

namespace TN.TNM.DataAccess.Databases.DAO
{
    public class ProjectDAO : BaseDAO, IProjectDataAccess
    {
        private readonly IHostingEnvironment iHostingEnvironment;
        private List<ProjectScopeModel> lstScopeNew = new List<ProjectScopeModel>();
        public IConfiguration Configuration { get; }

        public ProjectDAO(Databases.TNTN8Context _content, IAuditTraceDataAccess _iAuditTrace, IHostingEnvironment _hostingEnvironment, IConfiguration iconfiguration)
        {
            this.context = _content;
            this.iAuditTrace = _iAuditTrace;
            this.iHostingEnvironment = _hostingEnvironment;
            this.Configuration = iconfiguration;
        }

        public GetMasterProjectResult GetMasterProjectCreate(GetMasterProjectParameter parameter)
        {
            try
            {
                var listEmployee = new List<EmployeeEntityModel>();
                var listCustomer = new List<CustomerEntityModel>();
                var listContract = new List<ContractEntityModel>();
                var listProjectType = new List<CategoryEntityModel>();
                var listProjectScope = new List<CategoryEntityModel>();
                var listProjectStatus = new List<CategoryEntityModel>();
                var listTargetType = new List<CategoryEntityModel>();
                var listTargetUnit = new List<CategoryEntityModel>();

                // chỉ lấy khách hàng định danh
                var HDOStatusId = context.Category.FirstOrDefault(f => f.CategoryCode == "HDO")?.CategoryId;

                var listAllCustomer = context.Customer.Where(x => (x.Active == true) && (x.StatusId == HDOStatusId)).ToList();

                var listCategoryType = context.CategoryType.Where(x => x.Active == true).ToList();
                var listCategory = context.Category.Where(x => x.Active == true).ToList();
                var listAllEmployee = context.Employee.ToList();
                var listAllContract = context.Contract.Where(x => x.Active == true).ToList();
                var listOrganization = context.Organization.ToList();

                #region Lấy List Customer

                var listEmployeeId = listEmployee.Select(x => x.EmployeeId).ToList();
                var categoryTypeTHA = context.CategoryType.FirstOrDefault(ct => ct.Active == true && ct.CategoryTypeCode == "THA");
                var categoryNew = context.Category.FirstOrDefault(c =>
                    c.Active == true && c.CategoryCode == "MOI" && c.CategoryTypeId == categoryTypeTHA.CategoryTypeId);
                var categoryHDO = context.Category.FirstOrDefault(c =>
                    c.Active == true && c.CategoryCode == "HDO" && c.CategoryTypeId == categoryTypeTHA.CategoryTypeId);

                #region Lấy danh sách nhân viên

                listEmployee = listAllEmployee.Select(y =>
                           new EmployeeEntityModel
                           {
                               EmployeeId = y.EmployeeId,
                               EmployeeCode = y.EmployeeCode,
                               EmployeeName = y.EmployeeName,
                               EmployeeCodeName = y.EmployeeCode + " - " + y.EmployeeName,
                               OrganizationId = y.OrganizationId,
                               IsManager = y.IsManager,
                               Active = y.Active
                           }).ToList();

                listEmployee.ForEach(item =>
                {
                    var empOrganization = listOrganization.FirstOrDefault(x => x.OrganizationId == item.OrganizationId);
                    if (empOrganization != null)
                    {
                        item.OrganizationLevel = empOrganization.Level;
                    }

                });

                #endregion

                //if (listEmployeeId.Count > 0)
                //{
                var listUserId = context.User.Where(x => listEmployeeId.Contains(x.EmployeeId))
                        .Select(y => y.UserId);
                listCustomer = listAllCustomer.Select(
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
                        MaximumDebtDays = y.MaximumDebtDays,
                        MaximumDebtValue = y.MaximumDebtValue,
                        CustomerCodeName = y.CustomerCode + " - " + y.CustomerName,
                    }).ToList();
                //}

                #endregion

                #region Lấy List trạng thái dự án

                listContract = listAllContract
                  .Select(y => new ContractEntityModel
                  {
                      ContractId = y.ContractId,
                      QuoteId = y.QuoteId,
                      CustomerId = y.CustomerId,
                      ContractCode = y.ContractCode,
                      ContractTypeId = y.ContractTypeId,
                      EmployeeId = y.EmployeeId,
                      MainContractId = y.MainContractId,
                      ContractNote = y.ContractNote,
                      ContractDescription = y.ContractDescription,
                      ValueContract = y.ValueContract,
                      PaymentMethodId = y.PaymentMethodId,
                      BankAccountId = y.BankAccountId,
                      EffectiveDate = y.EffectiveDate,
                      Active = y.Active,
                      CreatedById = y.CreatedById,
                      CreatedDate = y.CreatedDate,
                      UpdatedById = y.UpdatedById,
                      UpdatedDate = y.UpdatedDate,
                      TenantId = y.TenantId,
                      DiscountType = y.DiscountType,
                      DiscountValue = y.DiscountValue,
                      Amount = y.Amount,
                      StatusId = y.StatusId,
                      ListDetail = null,
                      ContractName = y.ContractName,
                      ContractCodeName = GetContractCodeName(y.ContractCode, y.ContractName),
                  }).ToList();

                #endregion

                #region Lấy List trạng thái dự án

                var categoryTypeStatus = listCategoryType.FirstOrDefault(x => x.CategoryTypeCode == "DAT").CategoryTypeId;
                listProjectStatus = listCategory.Where(x => x.Active == true && x.CategoryTypeId == categoryTypeStatus)
                    .Select(y => new CategoryEntityModel
                    {
                        CategoryId = y.CategoryId,
                        CategoryCode = y.CategoryCode,
                        CategoryName = y.CategoryName
                    }).ToList();

                #endregion

                #region Lấy List Quy mô dự án

                var categoryTypeScope = listCategoryType.FirstOrDefault(x => x.CategoryTypeCode == "PSC").CategoryTypeId;
                listProjectScope = listCategory.Where(x => x.Active == true && x.CategoryTypeId == categoryTypeScope)
                    .Select(y => new CategoryEntityModel
                    {
                        CategoryId = y.CategoryId,
                        CategoryCode = y.CategoryCode,
                        CategoryName = y.CategoryName
                    }).ToList();

                #endregion

                #region Lấy List Loai dự án

                var categoryTypeProject = listCategoryType.FirstOrDefault(x => x.CategoryTypeCode == "LDA").CategoryTypeId;
                listProjectType = listCategory.Where(x => x.Active == true && x.CategoryTypeId == categoryTypeProject)
                    .Select(y => new CategoryEntityModel
                    {
                        CategoryId = y.CategoryId,
                        CategoryCode = y.CategoryCode,
                        CategoryName = y.CategoryName
                    }).ToList();

                #endregion

                #region Lấy List mục tiêu dự án

                var categoryTargetType = listCategoryType.FirstOrDefault(x => x.CategoryTypeCode == "LMT").CategoryTypeId;
                listTargetType = listCategory.Where(x => x.Active == true && x.CategoryTypeId == categoryTargetType)
                    .Select(y => new CategoryEntityModel
                    {
                        CategoryId = y.CategoryId,
                        CategoryCode = y.CategoryCode,
                        CategoryName = y.CategoryName
                    }).ToList();

                #endregion

                #region Lấy List đơn vị mục tiêu

                var categoryTargetUnit = listCategoryType.FirstOrDefault(x => x.CategoryTypeCode == "LDV").CategoryTypeId;
                listTargetUnit = listCategory.Where(x => x.Active == true && x.CategoryTypeId == categoryTargetUnit)
                    .Select(y => new CategoryEntityModel
                    {
                        CategoryId = y.CategoryId,
                        CategoryCode = y.CategoryCode,
                        CategoryName = y.CategoryName
                    }).ToList();

                #endregion

                return new GetMasterProjectResult
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "",
                    ListEmployee = listEmployee,
                    ListCustomer = listCustomer,
                    ListContract = listContract,
                    ListProjectType = listProjectType,
                    ListProjectScope = listProjectScope,
                    ListProjectStatus = listProjectStatus,
                    ListTargetType = listTargetType,
                    ListTargetUnit = listTargetUnit
                };
            }
            catch (Exception e)
            {
                return new GetMasterProjectResult
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }

        }
        public SearchProjectResult SearchProject(SearchProjectParameter parameter)
        {
            try
            {
                var user = context.User.FirstOrDefault(c => c.UserId == parameter.UserId);
                if (user == null)
                {
                    return new SearchProjectResult
                    {
                        MessageCode = "Nhân viên không tồn tại trong hệ thống",
                        StatusCode = HttpStatusCode.ExpectationFailed
                    };
                }
                var employee = context.Employee.FirstOrDefault(c => c.EmployeeId == user.EmployeeId);
                if (employee == null)
                {
                    return new SearchProjectResult
                    {
                        MessageCode = "Nhân viên không tồn tại trong hệ thống",
                        StatusCode = HttpStatusCode.ExpectationFailed
                    };
                }

                #region Khai báo và common data
                var listProject = new List<ProjectEntityModel>();
                var listAllProject = context.Project.Select(y => new ProjectEntityModel
                {
                    ProjectId = y.ProjectId,
                    ProjectCode = y.ProjectCode,
                    ProjectName = y.ProjectName,
                    ProjectType = y.ProjectType,
                    ProjectStatus = y.ProjectStatus,
                    ProjectStartDate = y.ProjectStartDate,
                    ProjectManagerId = y.ProjectManagerId,
                    ProjectEndDate = y.ProjectEndDate,
                    ActualStartDate = y.ActualStartDate,
                    ActualEndDate = y.ActualEndDate,
                    ProjectSize = y.ProjectSize,
                    EstimateCompleteTime = 0,
                    TaskComplate = 0,
                    UpdateDate = y.UpdateDate,
                    CreateBy = y.CreateBy,
                    CreateDate = y.CreateDate,
                    LastChangeActivityDate = y.LastChangeActivityDate
                }).OrderByDescending(z => z.LastChangeActivityDate).ThenByDescending(z => z.UpdateDate).ToList();

                var listEmployee = new List<EmployeeEntityModel>();
                var listCategoryTypeCodes = new List<string> { "DAT", "PSC", "LDA" };
                var listAllTask = context.Task.ToList();
                var listCategory = context.Category.Where(x => listCategoryTypeCodes.Contains(x.CategoryType.CategoryTypeCode) && x.Active == true).Select(y =>
                                   new CategoryEntityModel
                                   {
                                       CategoryId = y.CategoryId,
                                       CategoryName = y.CategoryName,
                                       CategoryCode = y.CategoryCode,
                                       CategoryTypeId = Guid.Empty,
                                       CreatedById = Guid.Empty,
                                       CategoryTypeCode = y.CategoryType.CategoryTypeCode,
                                       CountCategoryById = 0
                                   }).ToList();
                #endregion

                #region Phân quyền
                var position = context.Position.FirstOrDefault(c => c.PositionId == employee.PositionId);
                if (position != null && position.PositionCode == "GD")
                {
                    var isRoot = context.Organization.FirstOrDefault(c => c.OrganizationId == employee.OrganizationId).ParentId == null;
                    if (isRoot)
                    {
                        // Giám đốc được set đơn vị cao nhất trong tổ chức - Get All
                    }
                    else
                    {
                        // Lấy những bản ghi là quản lý, quản lý cấp cao, subPM - trong nguồn lực
                        // Những dự án có trong nguồn lực
                        var listProjectFollowResourceId = context.ProjectResource.Where(c => c.ObjectId == employee.EmployeeId).Select(m => m.ProjectId).ToList();
                        // Những dự án là quản lý, quản lý cấp cao, đồng quản lý
                        var listProjectFollowManagerId = context.ProjectEmployeeMapping.Where(c => c.EmployeeId == employee.EmployeeId).Select(c => c.ProjectId).ToList();

                        var listId = new List<Guid>();
                        listId.AddRange(listProjectFollowResourceId);
                        listId.AddRange(listProjectFollowManagerId);

                        listAllProject = listAllProject.Where(c => listId.Contains(c.ProjectId) || c.ProjectManagerId == employee.EmployeeId || c.CreateBy == user.UserId).ToList();
                    }
                }
                else
                {
                    // Những dự án có trong nguồn lực
                    var listProjectFollowResourceId = context.ProjectResource.Where(c => c.ObjectId == employee.EmployeeId).Select(m => m.ProjectId).ToList();
                    // Những dự án là quản lý, quản lý cấp cao, đồng quản lý
                    var listProjectFollowManagerId = context.ProjectEmployeeMapping.Where(c => c.EmployeeId == employee.EmployeeId).Select(c => c.ProjectId).ToList();

                    var listId = new List<Guid>();
                    listId.AddRange(listProjectFollowResourceId);
                    listId.AddRange(listProjectFollowManagerId);

                    listAllProject = listAllProject.Where(c => listId.Contains(c.ProjectId) || c.ProjectManagerId == employee.EmployeeId || c.CreateBy == user.UserId).ToList();
                }
                #endregion 

                listProject = listAllProject.Where(x =>
                        (parameter.ProjectName == null || parameter.ProjectName == "" || x.ProjectName.Contains(parameter.ProjectName)) &&
                        (parameter.ProjectCode == null || parameter.ProjectCode == "" || x.ProjectCode.Contains(parameter.ProjectCode)) &&
                        (parameter.ProjectStartS == null || parameter.ProjectStartS == DateTime.MinValue || x.ProjectStartDate.Value.Date >= parameter.ProjectStartS.Value.Date) &&
                        (parameter.ProjectStartE == null || parameter.ProjectStartE == DateTime.MinValue || x.ProjectStartDate.Value.Date <= parameter.ProjectStartE.Value.Date) &&
                        (parameter.ProjectEndS == null || parameter.ProjectEndS == DateTime.MinValue || x.ProjectEndDate.Value.Date >= parameter.ProjectEndS.Value.Date) &&
                        (parameter.ProjectEndE == null || parameter.ProjectEndE == DateTime.MinValue || x.ProjectEndDate.Value.Date <= parameter.ProjectEndE.Value.Date) &&
                        (parameter.ActualStartS == null || parameter.ActualStartS == DateTime.MinValue || x.ActualStartDate.Value.Date >= parameter.ActualStartS.Value.Date) &&
                        (parameter.ActualStartE == null || parameter.ActualStartE == DateTime.MinValue || x.ActualStartDate.Value.Date <= parameter.ActualStartE.Value.Date) &&
                        (parameter.ActualEndS == null || parameter.ActualEndS == DateTime.MinValue || x.ActualEndDate.Value.Date >= parameter.ActualEndS.Value.Date) &&
                        (parameter.ActualEndE == null || parameter.ActualEndE == DateTime.MinValue || x.ActualEndDate.Value.Date <= parameter.ActualEndE.Value.Date) &&
                        (parameter.ListProjectType == null || parameter.ListProjectType.Count == 0 || parameter.ListProjectType.Contains(x.ProjectType)) &&
                        (parameter.ListEmployee == null || parameter.ListEmployee.Count == 0 || parameter.ListEmployee.Contains(x.ProjectManagerId)) &&
                        (parameter.ListStatusProject == null || parameter.ListStatusProject.Count == 0 || parameter.ListStatusProject.Contains(x.ProjectStatus)))
                   .ToList();

                listProject.ForEach(p =>
                {
                    // tính thời gian dự kiến thực hiện
                    // tính ước tính tỉ lệ hoàn thành
                    decimal? _estimateCompleteTime = 0;
                    decimal? _taskComplete = 0;
                    var listProjectTask = listAllTask.Where(x => x.ProjectId == p.ProjectId).ToList();
                    if (listProjectTask.Count > 0)
                    {
                        listProjectTask.ForEach(item =>
                        {
                            if (item.EstimateHour != null) _estimateCompleteTime += item.EstimateHour;
                            if (item.TaskComplate != null && item.EstimateHour != null)
                                _taskComplete += ((item.TaskComplate) * item.EstimateHour);
                        });
                    }

                    p.EstimateCompleteTime = _estimateCompleteTime;
                    if (_estimateCompleteTime != 0 && _estimateCompleteTime != null && _taskComplete != null)
                        p.TaskComplate = Math.Round((decimal)(_taskComplete / _estimateCompleteTime), 2);

                    p.ProjectTypeName = listCategory.FirstOrDefault(c => c.CategoryId == p.ProjectType)?.CategoryName;
                    p.ProjectStatusName = listCategory.FirstOrDefault(c => c.CategoryId == p.ProjectStatus)?.CategoryName;
                    p.ProjectStatusCode = listCategory.FirstOrDefault(c => c.CategoryId == p.ProjectStatus)?.CategoryCode;
                    p.EmployeeName = context.Employee.FirstOrDefault(e => e.EmployeeId == p.ProjectManagerId)?.EmployeeName ?? string.Empty;

                    if (p.ProjectStatusCode != null || p.ProjectStatusCode != string.Empty)
                    {

                        switch (p.ProjectStatusCode)
                        {
                            case "MOI":
                                p.BackgroundColorForStatus = "#0F62FE";
                                break;
                            case "DTK":
                                p.BackgroundColorForStatus = "#FFC000";
                                break;
                            case "TDU":
                                p.BackgroundColorForStatus = "#FFC000";
                                break;
                            case "HTH":
                                p.BackgroundColorForStatus = "#63B646";
                                break;
                            case "DON":
                                p.BackgroundColorForStatus = "#9C00FF";
                                break;
                            case "HUY":
                                p.BackgroundColorForStatus = "#797979";
                                break;
                        }
                    }
                });

                listProject = listProject.Where(item =>
                    (parameter.EstimateCompleteTimeS == null || parameter.EstimateCompleteTimeS == 0 ||
                     parameter.EstimateCompleteTimeS <= item.EstimateCompleteTime) &&
                    (parameter.EstimateCompleteTimeE == null || parameter.EstimateCompleteTimeE == 0 ||
                     parameter.EstimateCompleteTimeE >= item.EstimateCompleteTime)).ToList();

                listProject = listProject.Where(x =>
                    (parameter.CompleteRateS == null || parameter.CompleteRateS == 0 ||
                     parameter.CompleteRateS <= x.TaskComplate) &&
                    (parameter.CompleteRateE == null || parameter.CompleteRateE == 0 ||
                     parameter.CompleteRateE >= x.TaskComplate)).ToList();

                #region Lấy danh sách nhân viên

                listEmployee = context.Employee.Select(y =>
                           new EmployeeEntityModel
                           {
                               EmployeeId = y.EmployeeId,
                               EmployeeCode = y.EmployeeCode,
                               EmployeeName = y.EmployeeName,
                               EmployeeCodeName = y.EmployeeCode + " - " + y.EmployeeName,
                               OrganizationId = y.OrganizationId,
                               IsManager = y.IsManager,
                               Active = y.Active
                           }).ToList();

                #endregion

                return new SearchProjectResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success",
                    ListProject = listProject,
                    ListStatus = listCategory.Where(c => c.CategoryTypeCode == "DAT").ToList(),
                    ListProjectType = listCategory.Where(c => c.CategoryTypeCode == "LDA").ToList(),
                    ListEmployee = listEmployee
                };
            }
            catch (Exception e)
            {
                return new SearchProjectResult()
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

        public UpdateProjectStatusResult UpdateProjectStatus(UpdateProjectStatusParameter parameter)
        {
            try
            {
                bool checkEndDate = false;
                bool startProject = false;
                var listAllUser = context.User.ToList();
                var status = new Category();
                Guid? statusId = Guid.Empty;

                var categoryTypeId = context.CategoryType.FirstOrDefault(ct => ct.CategoryTypeCode == "DAT")?.CategoryTypeId;

                switch (parameter.Status)
                {
                    case "STA":
                        status = context.Category.FirstOrDefault(c =>
                            c.CategoryCode == "DTK" && c.CategoryTypeId == categoryTypeId);
                        startProject = true;
                        break;
                    case "DEL":
                        DeleteProject(parameter.ProjectId);
                        break;
                    case "STO":
                        status = context.Category.FirstOrDefault(c =>
                            c.CategoryCode == "TDU" && c.CategoryTypeId == categoryTypeId);
                        break;
                    case "CLO":
                        status = context.Category.FirstOrDefault(c =>
                            c.CategoryCode == "DON" && c.CategoryTypeId == categoryTypeId);
                        break;
                    case "OPE":
                        checkEndDate = true;
                        status = context.Category.FirstOrDefault(c =>
                            c.CategoryCode == "DTK" && c.CategoryTypeId == categoryTypeId);
                        break;
                    case "HUY":
                        status = context.Category.FirstOrDefault(c =>
                            c.CategoryCode == "HUY" && c.CategoryTypeId == categoryTypeId);
                        break;
                    case "COM":
                        status = context.Category.FirstOrDefault(x =>
                            x.CategoryCode == "HTH" && x.CategoryTypeId == categoryTypeId);
                        break;
                }

                var project = context.Project.FirstOrDefault(p => p.ProjectId == parameter.ProjectId);

                if (project != null && status != null)
                {
                    statusId = status.CategoryId;
                    project.ProjectStatus = statusId;
                    switch (status.CategoryCode)
                    {
                        case "DTK":

                            if (startProject)
                            {
                                project.ActualStartDate = DateTime.Now;
                            }

                            if (checkEndDate)
                            {
                                if (project.ActualEndDate != null)
                                {
                                    var today = DateTime.Now;
                                    if (project.ActualEndDate.Value.Date <= today.Date)
                                    {
                                        project.ActualEndDate = null;
                                    }
                                }
                            }

                            break;
                        case "DON":
                            project.ActualEndDate = DateTime.Now;
                            break;
                    }

                    project.UpdateDate = DateTime.Now;
                    project.UpdateBy = parameter.UserId;

                    context.Update(project);
                    context.SaveChanges();

                    #region Thông báo

                    NotificationHelper.AccessNotification(context, TypeModel.ProjectDetail, "UPD", new Project(),
                        project, true);

                    #endregion

                    CreateNoteStatus(project.ProjectId, parameter.UserId, listAllUser, status.CategoryCode);
                }



                return new UpdateProjectStatusResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success",
                    StatusId = statusId
                };
            }
            catch (Exception e)
            {
                return new UpdateProjectStatusResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        private void CreateNoteStatus(Guid projectId, Guid userId, List<User> listAllUser, string statusCode)
        {
            Note note = new Note
            {
                NoteId = Guid.NewGuid(),
                Type = "SYS",
                ObjectId = projectId,
                ObjectType = "Project",
                Active = true,
                CreatedById = userId,
                CreatedDate = DateTime.Now,
                NoteTitle = "đã thêm ghi chú"
            };

            var user = listAllUser.FirstOrDefault(x => x.UserId == userId);
            if (user != null)
            {
                switch (statusCode)
                {
                    case "DTK":
                        note.Description = "<p><strong>" + user.UserName + "</strong>" + " thay đổi trạng thái dự án thành: " +
                                           "<strong>Đang triển khai</strong></p>";
                        break;
                    case "TDU":
                        note.Description = "<p><strong>" + user.UserName + "</strong>" + " thay đổi trạng thái dự án thành: " +
                                           "<strong>Tạm dừng</strong></p>";
                        break;
                    case "DON":
                        note.Description = "<p><strong>" + user.UserName + "</strong>" + " thay đổi trạng thái dự án thành: " +
                                           "<strong>Đóng</strong></p>";
                        break;
                    case "HUY":
                        note.Description = "<p><strong>" + user.UserName + "</strong>" + " thay đổi trạng thái dự án thành: " +
                                           "<strong>Hủy</strong></p>";
                        break;
                    case "HTH":
                        note.Description = "<p><strong>" + user.UserName + "</strong>" + " thay đổi trạng thái dự án thành: " +
                                           "<strong>Hoàn thành</strong></p>";
                        break;
                }

            }
            else
            {
                switch (statusCode)
                {
                    case "DTK":
                        note.Description = "<p>Thay đổi trạng thái dự án thành: <strong>Đang triển khai</strong></p>";
                        break;
                    case "TDU":
                        note.Description = "<p>Thay đổi trạng thái dự án thành: <strong>Tạm dừng</strong></p>";
                        break;
                    case "DON":
                        note.Description = "<p>Thay đổi trạng thái dự án thành: <strong>Đóng</strong></p>";
                        break;
                    case "HUY":
                        note.Description = "<p>Thay đổi trạng thái dự án thành: <strong>Hủy</strong></p>";
                        break;
                    case "HTH":
                        note.Description = "<p>Thay đổi trạng thái dự án thành: <strong>Hoàn thành</strong></p>";
                        break;
                }
            }
            context.Note.Add(note);

            // Update ngày thay đổi vào trường LastChangeActivityDate bảng Project
            var project = context.Project.FirstOrDefault(x => x.ProjectId == projectId);
            if (project != null)
            {
                project.LastChangeActivityDate = DateTime.Now;
                context.Project.Update(project);
            }
            context.SaveChanges();
        }

        private void DeleteProject(Guid projectId)
        {
            var projectContractMappings = context.ProjectContractMapping.Where(p => p.ProjectId == projectId).ToList();
            context.ProjectContractMapping.RemoveRange(projectContractMappings);

            var projectEmployeeMappings = context.ProjectEmployeeMapping.Where(p => p.ProjectId == projectId).ToList();
            context.ProjectEmployeeMapping.RemoveRange(projectEmployeeMappings);

            var projectResources = context.ProjectResource.Where(p => p.ProjectId == projectId).ToList();
            context.ProjectResource.RemoveRange(projectResources);

            var projectScopes = context.ProjectScope.Where(p => p.ProjectId == projectId).ToList();
            var projectScopeIds = projectScopes.Select(ps => ps.ProjectScopeId).ToList();
            context.ProjectScope.RemoveRange(projectScopes);

            var projectTasks = context.Task.Where(p => p.ProjectScopeId != null && projectScopeIds.Contains((Guid)p.ProjectScopeId)).ToList();
            context.Task.RemoveRange(projectTasks);


            var projectTargets = context.ProjectObjective.Where(p => p.ProjectId == projectId).ToList();
            context.ProjectObjective.RemoveRange(projectTargets);

            var project = context.Project.Where(p => p.ProjectId == projectId).ToList();
            context.Project.RemoveRange(project);
            context.SaveChanges();
        }

        public CreateProjectResult CreateProject(CreateProjectParameter parameter)
        {
            try
            {
                var listProjectCode = context.Project.Select(x => x.ProjectCode).ToList();
                var duplicateCode = false;

                if (listProjectCode.Count > 0)
                {
                    listProjectCode.ForEach(x =>
                    {
                        if (x.Equals(parameter.Project.ProjectCode))
                        {
                            duplicateCode = true;
                        }
                    });
                }

                if (duplicateCode)
                {
                    return new CreateProjectResult()
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = "Mã dự án đã tồn tại",
                    };
                }

                // kiểm tra nếu có tag <img> thì báo lỗi
                if (parameter.Project.Description != null)
                {
                    if (parameter.Project.Description.Contains("<img"))
                    {
                        return new CreateProjectResult()
                        {
                            StatusCode = HttpStatusCode.ExpectationFailed,
                            MessageCode = "Mô tả không được chưa ảnh",
                        };
                    }
                }

                parameter.Project.ProjectId = Guid.NewGuid();

                //Lưu quản lý cấp cao
                var ListEmployeeSm = parameter.Project.EmployeeSM.ToList();
                if (ListEmployeeSm?.Count > 0)
                {
                    ListEmployeeSm.ForEach(item =>
                    {
                        var projectEmployeeManagement = new Entities.ProjectEmployeeMapping()
                        {
                            ProjectResourceMappingId = Guid.NewGuid(),
                            EmployeeId = item,
                            ProjectId = parameter.Project.ProjectId,
                            Type = 0 // Quản lý cấp cao
                        };

                        context.ProjectEmployeeMapping.Add(projectEmployeeManagement);
                    });
                }

                //Lưu sub quản lý 
                var lstEmpSub = parameter.Project.EmployeeSub.ToList();
                if (lstEmpSub?.Count > 0)
                {
                    lstEmpSub.ForEach(item =>
                    {
                        var projectEmpPM = new Entities.ProjectEmployeeMapping()
                        {
                            ProjectResourceMappingId = Guid.NewGuid(),
                            EmployeeId = item,
                            ProjectId = parameter.Project.ProjectId,
                            Type = 1 // Sub quản lý 
                        };

                        context.ProjectEmployeeMapping.Add(projectEmpPM);
                    });
                }

                parameter.Project.CreateBy = parameter.UserId;
                parameter.Project.CreateDate = DateTime.Now;
                parameter.Project.UpdateBy = parameter.UserId;
                parameter.Project.UpdateDate = DateTime.Now;
                // Update ngày thay đổi vào trường LastChangeActivityDate bảng Project
                parameter.Project.LastChangeActivityDate = DateTime.Now;

                context.Project.Add(parameter.Project.ToEntity());

                #region Tạo thư mục lưu file cho dự án

                var projectCodeName = $"{parameter.Project.ProjectCode} - {parameter.Project.ProjectName}";
                var folderRootProject = context.Folder.FirstOrDefault(x => x.FolderType == "QLDA");
                var webRootPath = iHostingEnvironment.WebRootPath + "\\";

                if (folderRootProject != null)
                {
                    var folderRootProjectUrl = Path.Combine(webRootPath, ConvertFolderUrl(folderRootProject.Url));

                    if (Directory.Exists(folderRootProjectUrl))
                    {
                        #region folder gốc theo code và tên dự án

                        var folder = new Folder()
                        {
                            FolderId = Guid.NewGuid(),
                            Active = true,
                            CreatedById = parameter.UserId,
                            CreatedDate = DateTime.Now,
                            FolderLevel = folderRootProject.FolderLevel + 1,
                            IsDelete = false,
                            Name = projectCodeName,
                            ParentId = folderRootProject.FolderId,
                            Url = folderRootProject.Url + @"\" + projectCodeName,
                            FolderType = parameter.Project.ProjectCode.ToUpper(),
                            ObjectId = parameter.Project.ProjectId,
                        };

                        var folderName = ConvertFolderUrl(folder.Url);
                        var newPath = Path.Combine(webRootPath, folderName);
                        if (!Directory.Exists(newPath))
                        {
                            context.Folder.Add(folder);
                            Directory.CreateDirectory(newPath);
                        }

                        #endregion

                        #region folder cho tài liệu công việc

                        var folderTask = new Folder()
                        {
                            FolderId = Guid.NewGuid(),
                            Active = true,
                            CreatedById = parameter.UserId,
                            CreatedDate = DateTime.Now,
                            FolderLevel = folder.FolderLevel + 1,
                            IsDelete = false,
                            Name = $"{parameter.Project.ProjectCode} - Công việc",
                            ParentId = folder.FolderId,
                            FolderType = $"{parameter.Project.ProjectCode.ToUpper()}_TASK_FILE",
                            ObjectId = parameter.Project.ProjectId,
                        };
                        folderTask.Url = folder.Url + @"\" + folderTask.Name;
                        var taskFolderUrl = ConvertFolderUrl(folderTask.Url);
                        string newPathTask = Path.Combine(webRootPath, taskFolderUrl);

                        if (!Directory.Exists(newPathTask))
                        {
                            context.Folder.Add(folderTask);
                            Directory.CreateDirectory(newPathTask);
                        }

                        #endregion

                        // #region folder cho tài liệu ghi chú project
                        //
                        // var folderNote = new Folder()
                        // {
                        //     FolderId = Guid.NewGuid(),
                        //     Active = true,
                        //     CreatedById = parameter.UserId,
                        //     CreatedDate = DateTime.Now,
                        //     FolderLevel = folder.FolderLevel + 1,
                        //     IsDelete = false,
                        //     Name = $"{parameter.Project.ProjectCode} - Thông tin chung",
                        //     ParentId = folder.FolderId,
                        //     FolderType = $"{parameter.Project.ProjectCode.ToUpper()}_DETAIL_NOTE",
                        //     ObjectId = parameter.Project.ProjectId,
                        // };
                        // folderNote.Url = folder.Url + @"\" + folderNote.Name;
                        // var noteFolderUrl = ConvertFolderUrl(folderNote.Url);
                        // string newPathNote = Path.Combine(webRootPath, noteFolderUrl);
                        //
                        // if (!Directory.Exists(newPathNote))
                        // {
                        //     context.Folder.Add(folderNote);
                        //     Directory.CreateDirectory(newPathNote);
                        // }
                        //
                        // #endregion

                        #region folder cho tài liệu dự án

                        var folderFile = new Folder()
                        {
                            FolderId = Guid.NewGuid(),
                            Active = true,
                            CreatedById = parameter.UserId,
                            CreatedDate = DateTime.Now,
                            FolderLevel = folder.FolderLevel + 1,
                            IsDelete = false,
                            Name = $"{parameter.Project.ProjectCode} - Tài liệu",
                            ParentId = folder.FolderId,
                            FolderType = $"{parameter.Project.ProjectCode.ToUpper()}_PROJECT_FILE",
                            ObjectId = parameter.Project.ProjectId,
                        };
                        folderFile.Url = folder.Url + @"\" + folderFile.Name;
                        var projectFolderUrl = ConvertFolderUrl(folderFile.Url);
                        var newPathProject = Path.Combine(webRootPath, projectFolderUrl);

                        if (!Directory.Exists(newPathProject))
                        {
                            context.Folder.Add(folderFile);
                            Directory.CreateDirectory(newPathProject);
                        }

                        #endregion
                    }
                    else
                    {
                        return new CreateProjectResult()
                        {
                            StatusCode = HttpStatusCode.ExpectationFailed,
                            MessageCode = "Bạn phải cấu hình thư mục để lưu file"
                        };
                    }
                }

                #endregion



                context.SaveChanges();

                #region Thông báo

                NotificationHelper.AccessNotification(context, TypeModel.Project, "CRE", new Product(), parameter.Project.ToEntity(), true);

                #endregion

                #region Thêm 1 hạng mục dự án
                // Tạo phạm vi công việc mặc định
                // Chọn HĐ liên quan nhưng hợp đồng không gắn với báo giá 
                var contractNotQuote = context.Contract.Where(x => (x.QuoteId == null || x.QuoteId == Guid.Empty) && x.ContractId == parameter.Project.ContractId).ToList();
                // Chọn HĐ gắn với báo giá nhưng báo giá báo giá không có giá trị tại tab Phạm vi công việc
                int countContract = 0;
                var contractQuote = context.Contract.FirstOrDefault(x => (x.QuoteId != null && x.QuoteId != Guid.Empty) && x.ContractId == parameter.Project.ContractId);
                if (contractQuote != null)
                {
                    countContract = context.QuoteScope.Where(x => x.QuoteId == contractQuote.QuoteId).ToList().Count();
                }

                using (var dbcxtransaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var firstProjectScopeId = Guid.NewGuid();
                        var projectScope = new ProjectScope()
                        {
                            ProjectScopeId = firstProjectScopeId,
                            ProjectScopeCode = string.Empty,
                            ProjectScopeName = parameter.Project.ProjectName,
                            Description = "",
                            ParentId = null,
                            ProjectId = parameter.Project.ProjectId,
                            CreateDate = DateTime.Now,
                            CreateBy = parameter.UserId,
                            Level = 0
                        };
                        context.ProjectScope.Add(projectScope);

                        if (countContract > 0)
                        {
                            var contract = context.Contract.FirstOrDefault(x => x.ContractId == parameter.Project.ContractId);
                            // Danh sách Phạm vi công việc (Ngoại trừ note đầu tiên)
                            var scopes = context.QuoteScope.Where(x => x.QuoteId == contract.QuoteId).OrderBy(x => x.Level).ToList();

                            List<QuoteScope> lstNewQuoteScopes = new List<QuoteScope>();
                            lstNewQuoteScopes = scopes.Select(q => new QuoteScope()
                            {
                                ScopeId = q.ScopeId,
                                QuoteId = q.QuoteId,
                                Tt = q.Tt,
                                Category = q.Category,
                                Description = q.Description,
                                CreatedDate = q.CreatedDate,
                                CreatedById = q.CreatedById,
                                ParentId = q.ParentId,
                                Level = q.Level
                            }).ToList();

                            List<CloneProjectScopeModel> lstNewOldId = new List<CloneProjectScopeModel>();
                            // Vòng lặp qua các level
                            for (int i = 0; i < lstNewQuoteScopes.Count(); i++)
                            {
                                if (i == 0)
                                {
                                    var newScopeId = Guid.NewGuid();
                                    lstNewQuoteScopes.Where(x => x.Level == i).ToList().ForEach(qout =>
                                    {
                                        // Thêm vào list
                                        lstNewOldId.Add(new CloneProjectScopeModel
                                        {
                                            NewScopeId = newScopeId,
                                            OldScopeId = qout.ScopeId
                                        });

                                        qout.ScopeId = newScopeId;
                                        qout.ParentId = qout.ParentId;

                                    });
                                }
                                else
                                {
                                    lstNewQuoteScopes.Where(x => x.Level == i).ToList().ForEach(qout =>
                                    {
                                        var newScopeId = Guid.NewGuid();
                                        // Thêm vào list
                                        lstNewOldId.Add(new CloneProjectScopeModel
                                        {
                                            NewScopeId = newScopeId,
                                            OldScopeId = (Guid)qout.ScopeId
                                        });

                                        qout.ScopeId = newScopeId;
                                        qout.ParentId = lstNewOldId.FirstOrDefault(a => a.OldScopeId == qout.ParentId).NewScopeId;
                                    });
                                }
                            }

                            // Xóa note cha đầu tiên
                            lstNewQuoteScopes.RemoveAt(0);
                            // Cập nhập lại parentId cho level 1 là projectScopeId của note mới được tạo                        
                            lstNewQuoteScopes.ForEach(item =>
                            {
                                if (item.Level == 1)
                                    item.ParentId = firstProjectScopeId;
                            });

                            lstProjectScope = lstNewQuoteScopes.Select(q => new ProjectScope()
                            {
                                ProjectScopeId = q.ScopeId,
                                ProjectId = parameter.Project.ProjectId,
                                ProjectScopeCode = q.Tt,
                                ProjectScopeName = q.Category,
                                Description = q.Description,
                                CreateDate = DateTime.Now,
                                CreateBy = parameter.UserId,
                                ParentId = q.ParentId,
                                Level = q.Level
                            }).ToList();

                            if (lstProjectScope.Count() > 0)
                            {
                                context.ProjectScope.AddRange(lstProjectScope);
                            }
                        }

                        context.SaveChanges();
                        dbcxtransaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        dbcxtransaction.Rollback();
                    }
                }


                #endregion

                //Thêm danh sach mục tiêu
                List<ProjectObjective> listprojectTarget = new List<ProjectObjective>();
                parameter.ListProjectTarget.ForEach(item =>
                {
                    var target = new ProjectObjective();

                    target.ProjectObjectId = Guid.NewGuid();
                    target.OrderNumber = item.OrderNumber;
                    target.ProjectObjectName = item.ProjectObjectName;
                    target.ProjectObjectType = item.ProjectObjectType;
                    target.ProjectObjectUnit = item.ProjectObjectUnit;
                    target.ProjectObjectDescription = item.ProjectObjectDescription;
                    target.ProjectObjectValue = item.ProjectObjectValue;
                    target.ProjectId = parameter.Project.ProjectId;
                    listprojectTarget.Add(target);
                });
                context.ProjectObjective.AddRange(listprojectTarget);
                context.SaveChanges();

                return new CreateProjectResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Tạo dự án thành công.",
                    ProjectId = parameter.Project.ProjectId
                };
            }
            catch (Exception e)
            {
                return new CreateProjectResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message,

                };
            }
        }
        private class QuoteScopeModel
        {
            public Guid ScopeId { get; set; }
            public string Tt { get; set; }
            public string Category { get; set; }
            public string Description { get; set; }
            public Guid? QuoteId { get; set; }
            public Guid? TenantId { get; set; }
            public int? Level { get; set; }
            public Guid? ParentId { get; set; }
            public DateTime? CreatedDate { get; set; }
            public Guid? CreatedById { get; set; }
        }

        private class CloneProjectScopeModel
        {
            public Guid NewScopeId { get; set; }
            public Guid? OldScopeId { get; set; }
        }
        private List<ProjectScope> lstProjectScope = new List<ProjectScope>();
        //private List<ProjectScope> lstNewProjectScope = new List<ProjectScope>();
        //public void RecursiveQuoteScope(Guid? scopeId, Guid? newScopeId)
        //{
        //    var lstQuote = lstProjectScope.Where(x => x.ParentId == scopeId).ToList();
        //    lstQuote?.ForEach(item =>
        //    {
        //        List<ProjectScope> hasChild = new List<ProjectScope>();
        //        hasChild = new List<ProjectScope>(lstProjectScope.Where(x => x.ParentId == item.ProjectScopeId).ToList());

        //        if (hasChild.Count() > 0)
        //        {
        //            hasChild.ForEach(x =>
        //            {
        //                x.ProjectScopeId = Guid.NewGuid();
        //                x.ParentId = newScopeId;
        //                lstQuoteNewScope.Add(x);
        //            });
        //        }
        //        else
        //        {
        //            item.ProjectScopeId = Guid.NewGuid();
        //            item.ParentId = newScopeId;
        //            lstQuoteNewScope.Add(item);
        //        }
        //    });
        //}

        public GetMasterUpdateProjectResult GetMasterUpdateProjectCreate(GetMasterUpdateProjectParameter parameter)
        {
            try
            {
                var user = context.User.FirstOrDefault(c => c.UserId == parameter.UserId);
                int pageSize = 10;
                int pageIndex = 1;
                int totalRecordsNote = 0;
                if (user == null)
                {
                    return new GetMasterUpdateProjectResult
                    {
                        MessageCode = "Nhân viên không tồn tại trong hệ thống",
                        StatusCode = HttpStatusCode.ExpectationFailed
                    };
                }
                var employee = context.Employee.FirstOrDefault(c => c.EmployeeId == user.EmployeeId);
                if (employee == null)
                {
                    return new GetMasterUpdateProjectResult
                    {
                        MessageCode = "Nhân viên không tồn tại trong hệ thống",
                        StatusCode = HttpStatusCode.ExpectationFailed
                    };
                }

                var listEmployee = new List<EmployeeEntityModel>();
                var listCustomer = new List<CustomerEntityModel>();
                var listContract = new List<ContractEntityModel>();
                var listProjectType = new List<CategoryEntityModel>();
                var listProjectScope = new List<CategoryEntityModel>();
                var listProjectStatus = new List<CategoryEntityModel>();
                var listTargetType = new List<CategoryEntityModel>();
                var listTargetUnit = new List<CategoryEntityModel>();

                // chỉ lấy khách hàng định danh
                var HDOStatusId = context.Category.FirstOrDefault(f => f.CategoryCode == "HDO")?.CategoryId;

                var listAllCustomer = context.Customer.Where(x => (x.Active == true) && (x.StatusId == HDOStatusId)).ToList();

                var listCategoryType = context.CategoryType.Where(x => x.Active == true).ToList();
                var listCategory = context.Category.Where(x => x.Active == true).ToList();
                var listAllEmployee = context.Employee.ToList();
                var listAllContract = context.Contract.Where(x => x.Active == true).ToList();

                string webRootPath = iHostingEnvironment.WebRootPath + "\\"; ;


                var employeeSM = context.ProjectEmployeeMapping.Where(pe => pe.ProjectId == parameter.ProjectId && pe.Type == 0).Select(p => p.EmployeeId).ToList();
                var employeeSub = context.ProjectEmployeeMapping.Where(pe => pe.ProjectId == parameter.ProjectId && pe.Type == 1).Select(p => p.EmployeeId).ToList();

                var project = context.Project.Where(x => x.ProjectId == parameter.ProjectId).Select(y => new ProjectEntityModel
                {
                    ProjectId = y.ProjectId,
                    ProjectStartDate = y.ProjectStartDate,
                    ProjectEndDate = y.ProjectEndDate,
                    ActualStartDate = y.ActualStartDate,
                    ActualEndDate = y.ActualEndDate,
                    ProjectManagerId = y.ProjectManagerId,
                    ContractId = y.ContractId,
                    ProjectName = y.ProjectName,
                    ProjectCode = y.ProjectCode,
                    BudgetVnd = y.BudgetVnd,
                    BudgetUsd = y.BudgetUsd,
                    BudgetNgayCong = y.BudgetNgayCong,
                    CustomerId = y.CustomerId,
                    Description = y.Description,
                    ProjectSize = y.ProjectSize,
                    ProjectType = y.ProjectType,
                    ProjectStatus = y.ProjectStatus,
                    IncludeWeekend = y.IncludeWeekend,
                    Priority = y.Priority,
                    EmployeeSM = employeeSM,
                    EmployeeSub = employeeSub,
                    GiaBanTheoGio = y.GiaBanTheoGio,
                    ProjectStatusPlan = y.ProjectStatusPlan,
                    NgayKyNghiemThu = y.NgayKyNghiemThu
                }).FirstOrDefault();

                if (project == null)
                {
                    return new GetMasterUpdateProjectResult()
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = "Dự án không tồn tại trên hệ thống"
                    };
                }

                #region Phân quyền
                /*
                 * PM, Quản lý cấp cao: Toàn quyền vs project
                 * SubPm: Không được sửa thông tin chung - vẫn được sửa các mục liên quan của dự án
                 * Người tạo nhưng ko có vai trò j trong dự án - chỉ đc sửa thông tin chung - ko được sửa các mục liên quan
                 * Nguồn lực của dự án - Không được quyền chỉnh sửa thông tin chung của dự án
                 */
                var role = string.Empty;
                var position = context.Position.FirstOrDefault(c => c.PositionId == employee.PositionId);
                if (position.PositionCode == "GD")
                {
                    // là giám đốc nên có toàn quyền
                    role = "GD";
                }
                else
                {
                    var projectEmployeeMapping = context.ProjectEmployeeMapping.FirstOrDefault(c => c.ProjectId == project.ProjectId && c.Type == 0);
                    if (project.ProjectManagerId == employee.EmployeeId || (projectEmployeeMapping != null && projectEmployeeMapping.EmployeeId == employee.EmployeeId))
                    {
                        // Là quản lý va quản lý cấp cao
                        role = "PM";
                    }
                    else
                    {
                        var projectSubPm = context.ProjectEmployeeMapping.FirstOrDefault(c => c.ProjectId == project.ProjectId && c.Type == 1 && c.EmployeeId == employee.EmployeeId);
                        if (projectSubPm != null)
                        {
                            role = "SUBPM";
                        }
                        else
                        {
                            var listReousreId = context.ProjectResource.Where(c => c.ObjectId == employee.EmployeeId && c.ProjectId == project.ProjectId).ToList();
                            role = listReousreId.Count != 0 ? "RESOURCE" : "CREATER";
                        }
                    }
                }
                #endregion

                project.ProjectStatusCode = listCategory.FirstOrDefault(x => x.CategoryId == project.ProjectStatus)?
                    .CategoryCode;

                #region Lấy List Customer

                var listEmployeeId = listEmployee.Select(x => x.EmployeeId).ToList();
                var categoryTypeTHA = context.CategoryType.FirstOrDefault(ct => ct.Active == true && ct.CategoryTypeCode == "THA");
                var categoryNew = context.Category.FirstOrDefault(c =>
                    c.Active == true && c.CategoryCode == "MOI" && c.CategoryTypeId == categoryTypeTHA.CategoryTypeId);
                var categoryHDO = context.Category.FirstOrDefault(c =>
                    c.Active == true && c.CategoryCode == "HDO" && c.CategoryTypeId == categoryTypeTHA.CategoryTypeId);

                #region Lấy danh sách nhân viên

                var listOrganization = context.Organization.ToList();

                listEmployee = listAllEmployee.Select(y =>
                           new EmployeeEntityModel
                           {
                               EmployeeId = y.EmployeeId,
                               EmployeeCode = y.EmployeeCode,
                               EmployeeName = y.EmployeeName,
                               EmployeeCodeName = y.EmployeeCode + " - " + y.EmployeeName,
                               OrganizationId = y.OrganizationId,
                               IsManager = y.IsManager,
                               Active = y.Active
                           }).ToList();

                listEmployee.ForEach(item =>
                {
                    var empOrganization = listOrganization.FirstOrDefault(x => x.OrganizationId == item.OrganizationId);
                    if (empOrganization != null)
                    {
                        item.OrganizationLevel = empOrganization.Level;
                    }

                });

                #endregion

                //if (listEmployeeId.Count > 0)
                //{
                var listUserId = context.User.Where(x => listEmployeeId.Contains(x.EmployeeId))
                        .Select(y => y.UserId);
                listCustomer = listAllCustomer.Select(y => new CustomerEntityModel
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
                    MaximumDebtDays = y.MaximumDebtDays,
                    MaximumDebtValue = y.MaximumDebtValue,
                    CustomerCodeName = y.CustomerCode + " - " + y.CustomerName,
                }).ToList();
                //}

                #endregion

                #region Lấy List trạng thái dự án

                listContract = listAllContract
                  .Select(y => new ContractEntityModel
                  {
                      ContractId = y.ContractId,
                      QuoteId = y.QuoteId,
                      CustomerId = y.CustomerId,
                      ContractCode = y.ContractCode,
                      ContractTypeId = y.ContractTypeId,
                      EmployeeId = y.EmployeeId,
                      MainContractId = y.MainContractId,
                      ContractNote = y.ContractNote,
                      ContractDescription = y.ContractDescription,
                      ValueContract = y.ValueContract,
                      PaymentMethodId = y.PaymentMethodId,
                      BankAccountId = y.BankAccountId,
                      EffectiveDate = y.EffectiveDate,
                      Active = y.Active,
                      CreatedById = y.CreatedById,
                      CreatedDate = y.CreatedDate,
                      UpdatedById = y.UpdatedById,
                      UpdatedDate = y.UpdatedDate,
                      TenantId = y.TenantId,
                      DiscountType = y.DiscountType,
                      DiscountValue = y.DiscountValue,
                      Amount = y.Amount,
                      StatusId = y.StatusId,
                      ListDetail = null,
                      ContractName = y.ContractName,
                      ContractCodeName = GetContractCodeName(y.ContractCode, y.ContractName),
                  }).ToList();

                #endregion

                #region Lấy List trạng thái dự án

                var categoryTypeStatus = listCategoryType.FirstOrDefault(x => x.CategoryTypeCode == "DAT").CategoryTypeId;
                listProjectStatus = listCategory.Where(x => x.Active == true && x.CategoryTypeId == categoryTypeStatus)
                    .Select(y => new CategoryEntityModel
                    {
                        CategoryId = y.CategoryId,
                        CategoryCode = y.CategoryCode,
                        CategoryName = y.CategoryName
                    }).ToList();

                #endregion

                #region Lấy List Quy mô dự án

                var categoryTypeScope = listCategoryType.FirstOrDefault(x => x.CategoryTypeCode == "PSC").CategoryTypeId;
                listProjectScope = listCategory.Where(x => x.Active == true && x.CategoryTypeId == categoryTypeScope)
                    .Select(y => new CategoryEntityModel
                    {
                        CategoryId = y.CategoryId,
                        CategoryCode = y.CategoryCode,
                        CategoryName = y.CategoryName
                    }).ToList();

                #endregion

                #region Lấy List Loai dự án

                var categoryTypeProject = listCategoryType.FirstOrDefault(x => x.CategoryTypeCode == "LDA").CategoryTypeId;
                listProjectType = listCategory.Where(x => x.Active == true && x.CategoryTypeId == categoryTypeProject)
                    .Select(y => new CategoryEntityModel
                    {
                        CategoryId = y.CategoryId,
                        CategoryCode = y.CategoryCode,
                        CategoryName = y.CategoryName
                    }).ToList();

                #endregion

                #region Lấy List mục tiêu dự án

                var categoryTargetType = listCategoryType.FirstOrDefault(x => x.CategoryTypeCode == "LMT").CategoryTypeId;
                listTargetType = listCategory.Where(x => x.Active == true && x.CategoryTypeId == categoryTargetType)
                    .Select(y => new CategoryEntityModel
                    {
                        CategoryId = y.CategoryId,
                        CategoryCode = y.CategoryCode,
                        CategoryName = y.CategoryName
                    }).ToList();

                #endregion

                #region Lấy List đơn vị mục tiêu

                var categoryTargetUnit = listCategoryType.FirstOrDefault(x => x.CategoryTypeCode == "LDV").CategoryTypeId;
                listTargetUnit = listCategory.Where(x => x.Active == true && x.CategoryTypeId == categoryTargetUnit)
                    .Select(y => new CategoryEntityModel
                    {
                        CategoryId = y.CategoryId,
                        CategoryCode = y.CategoryCode,
                        CategoryName = y.CategoryName
                    }).ToList();

                #endregion

                #region Lấy List mục tiêu dự án

                var listProjectTargets = context.ProjectObjective.Where(x => x.ProjectId == parameter.ProjectId)
                           .Select(y => new ProjectTargetEntityModel
                           {
                               ProjectId = y.ProjectId,
                               ProjectObjectDescription = y.ProjectObjectDescription,
                               ProjectObjectId = y.ProjectObjectId,
                               ProjectObjectName = y.ProjectObjectName,
                               ProjectObjectType = y.ProjectObjectType,
                               ProjectObjectUnit = y.ProjectObjectUnit,
                               TargetTypeDisplay = listTargetType.FirstOrDefault(t => t.CategoryId == y.ProjectObjectType).CategoryName,
                               TargetUnitDisplay = listTargetUnit.FirstOrDefault(t => t.CategoryId == y.ProjectObjectUnit).CategoryName,
                               ProjectObjectValue = y.ProjectObjectValue,
                               OrderNumber = y.OrderNumber
                           }).OrderBy(z => z.OrderNumber).ToList();

                #endregion

                #region Lấy list note (ghi chú)

                var folderType = $"{project.ProjectCode}_PROJECT_FILE";

                var folderUrl = context.Folder.FirstOrDefault(x => x.FolderType == folderType)?.Url;

                var listNote = context.Note
                    .Where(x => x.ObjectId == parameter.ProjectId && x.ObjectType == "PROJECT" && x.Active == true)
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
                    var listNoteDocument = context.NoteDocument.Where(x => listNoteId.Contains(x.NoteId)).Select(
                        y => new NoteDocumentEntityModel
                        {
                            DocumentName = y.DocumentName,
                            DocumentSize = y.DocumentSize,
                            DocumentUrl = y.DocumentUrl,
                            CreatedById = y.CreatedById,
                            CreatedDate = y.CreatedDate,
                            UpdatedById = y.UpdatedById,
                            UpdatedDate = y.UpdatedDate,
                            NoteDocumentId = y.NoteDocumentId,
                            NoteId = y.NoteId
                        }
                    ).ToList();

                    var listFileInFolder = context.FileInFolder.Where(x => listNoteId.Contains((Guid)x.ObjectId))
                        .ToList();

                    listFileInFolder.ForEach(item =>
                    {
                        var file = new NoteDocumentEntityModel
                        {
                            DocumentName = item.FileName.Substring(0, item.FileName.LastIndexOf("_")),
                            DocumentSize = item.Size,
                            CreatedById = item.CreatedById,
                            CreatedDate = item.CreatedDate,
                            UpdatedById = item.UpdatedById,
                            UpdatedDate = item.UpdatedDate,
                            NoteDocumentId = item.FileInFolderId,
                            NoteId = (Guid)item.ObjectId
                        };

                        var fileName = $"{item.FileName}.{item.FileExtension}";
                        var folderName = ConvertFolderUrl(folderUrl);

                        file.DocumentUrl = Path.Combine(webRootPath, folderName, fileName);

                        listNoteDocument.Add(file);
                    });

                    listNote.ForEach(item =>
                    {
                        var _user = listUser.FirstOrDefault(x => x.UserId == item.CreatedById);
                        if (_user != null)
                        {
                            var _employee = _listAllEmployee.FirstOrDefault(x => x.EmployeeId == _user.EmployeeId);
                            item.ResponsibleName = _employee.EmployeeName;
                            item.NoteDocList = listNoteDocument.Where(x => x.NoteId == item.NoteId)
                                .OrderBy(z => z.UpdatedDate).ToList();
                        }
                    });

                    // Sắp xếp lại listnote
                    listNote = listNote.OrderByDescending(x => x.CreatedDate).ToList();
                    totalRecordsNote = listNote.Count;

                    listNote = listNote
                        .Skip(pageSize * (pageIndex - 1))
                        .Take(pageSize).ToList();
                }

                #endregion

                #region kiểm tra xem có folder default của dự án chưa

                var listFolder = context.Folder
                    .Where(x => x.ObjectId == project.ProjectId && x.FolderType.Contains(project.ProjectCode)).ToList();

                var projectCodeName = $"{project.ProjectCode} - {project.ProjectName}";
                var folderRootProject = context.Folder.FirstOrDefault(x => x.FolderType == "QLDA");


                if (listFolder.Count > 0)
                {
                    listFolder.ForEach(item =>
                    {
                        var path = Path.Combine(webRootPath, item.Url);
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                    });
                }
                else
                {

                    #region Tạo thư mục lưu file cho dự án



                    if (folderRootProject != null)
                    {
                        #region folder gốc theo code và tên dự án

                        var folder = new Folder()
                        {
                            FolderId = Guid.NewGuid(),
                            Active = true,
                            CreatedById = parameter.UserId,
                            CreatedDate = DateTime.Now,
                            FolderLevel = folderRootProject.FolderLevel + 1,
                            IsDelete = false,
                            Name = projectCodeName,
                            ParentId = folderRootProject.FolderId,
                            Url = folderRootProject.Url + @"\" + projectCodeName,
                            FolderType = project.ProjectCode.ToUpper(),
                            ObjectId = project.ProjectId,
                        };

                        var folderName = ConvertFolderUrl(folder.Url);
                        var newPath = Path.Combine(webRootPath, folderName);
                        if (!Directory.Exists(newPath))
                        {
                            context.Folder.Add(folder);
                            context.SaveChanges();
                            Directory.CreateDirectory(newPath);
                        }

                        #endregion

                        #region folder cho tài liệu công việc

                        var folderTask = new Folder()
                        {
                            FolderId = Guid.NewGuid(),
                            Active = true,
                            CreatedById = parameter.UserId,
                            CreatedDate = DateTime.Now,
                            FolderLevel = folder.FolderLevel + 1,
                            IsDelete = false,
                            Name = $"{project.ProjectCode} - Công việc",
                            ParentId = folder.FolderId,
                            FolderType = $"{project.ProjectCode.ToUpper()}_TASK_FILE",
                            ObjectId = project.ProjectId,
                        };
                        folderTask.Url = folder.Url + @"\" + folderTask.Name;
                        var taskFolderUrl = ConvertFolderUrl(folderTask.Url);
                        string newPathTask = Path.Combine(webRootPath, taskFolderUrl);

                        if (!Directory.Exists(newPathTask))
                        {
                            context.Folder.Add(folderTask);
                            context.SaveChanges();
                            Directory.CreateDirectory(newPathTask);
                        }

                        #endregion

                        // #region folder cho tài liệu ghi chú project
                        //
                        // var folderNote = new Folder()
                        // {
                        //     FolderId = Guid.NewGuid(),
                        //     Active = true,
                        //     CreatedById = parameter.UserId,
                        //     CreatedDate = DateTime.Now,
                        //     FolderLevel = folder.FolderLevel + 1,
                        //     IsDelete = false,
                        //     Name = $"{project.ProjectCode} - Thông tin chung",
                        //     ParentId = folder.FolderId,
                        //     FolderType = $"{project.ProjectCode.ToUpper()}_DETAIL_NOTE",
                        //     ObjectId = project.ProjectId,
                        // };
                        // folderNote.Url = folder.Url + @"\" + folderNote.Name;
                        // var noteFolderUrl = ConvertFolderUrl(folderNote.Url);
                        // string newPathNote = Path.Combine(webRootPath, noteFolderUrl);
                        //
                        // if (!Directory.Exists(newPathNote))
                        // {
                        //     context.Folder.Add(folderNote);
                        //     context.SaveChanges();
                        //     Directory.CreateDirectory(newPathNote);
                        // }
                        //
                        // #endregion

                        #region folder cho tài liệu dự án

                        var folderFile = new Folder()
                        {
                            FolderId = Guid.NewGuid(),
                            Active = true,
                            CreatedById = parameter.UserId,
                            CreatedDate = DateTime.Now,
                            UpdatedById = parameter.UserId,
                            UpdatedDate = DateTime.Now,
                            FolderLevel = folder.FolderLevel + 1,
                            IsDelete = false,
                            Name = $"{project.ProjectCode} - Tài liệu",
                            ParentId = folder.FolderId,
                            FolderType = $"{project.ProjectCode.ToUpper()}_PROJECT_FILE",
                            ObjectId = project.ProjectId,
                        };
                        folderFile.Url = folder.Url + @"\" + folderFile.Name;
                        var projectFolderUrl = ConvertFolderUrl(folderFile.Url);
                        var newPathProject = Path.Combine(webRootPath, projectFolderUrl);

                        if (!Directory.Exists(newPathProject))
                        {
                            context.Folder.Add(folderFile);
                            context.SaveChanges();
                            Directory.CreateDirectory(newPathProject);
                        }

                        #endregion
                    }

                    #endregion

                }


                #endregion

                var statusId = context.Category.FirstOrDefault(x => x.CategoryCode == "MOI").CategoryId;
                // Kiểm tra xem dự án có tồn tại ít nhất 1 task nào có trạng thái khác trạng thái mới k
                var has = context.Task.FirstOrDefault(x => x.ProjectId == parameter.ProjectId && x.Status != statusId);

                #region list dự án theo phân quyền user

                var listAllProject = context.Project.ToList();

                if (user != null)
                {
                    if (employee != null)
                    {
                        var positionEmp = context.Position.FirstOrDefault(c => c.PositionId == employee.PositionId);
                        if (positionEmp != null && positionEmp.PositionCode == "GD")
                        {
                            var isRoot = context.Organization.FirstOrDefault(c => c.OrganizationId == employee.OrganizationId).ParentId == null;
                            if (!isRoot)
                            {
                                // Giám đốc được set đơn vị cao nhất trong tổ chức - Get All
                                // Lấy những bản ghi là quản lý, quản lý cấp cao, subPM - trong nguồn lực
                                // Những dự án có trong nguồn lực
                                var listProjectFollowResourceId = context.ProjectResource.Where(c => c.ObjectId == employee.EmployeeId).Select(m => m.ProjectId).ToList();
                                // Những dự án là quản lý, quản lý cấp cao, đồng quản lý
                                var listProjectFollowManagerId = context.ProjectEmployeeMapping.Where(c => c.EmployeeId == employee.EmployeeId).Select(c => c.ProjectId).ToList();

                                var listId = new List<Guid>();
                                listId.AddRange(listProjectFollowResourceId);
                                listId.AddRange(listProjectFollowManagerId);

                                listAllProject = listAllProject.Where(c => listId.Contains(c.ProjectId) || c.ProjectManagerId == employee.EmployeeId || c.CreateBy == user.UserId).ToList();
                            }
                        }
                        else
                        {
                            // Những dự án có trong nguồn lực
                            var listProjectFollowResourceId = context.ProjectResource.Where(c => c.ObjectId == employee.EmployeeId).Select(m => m.ProjectId).ToList();
                            // Những dự án là quản lý, quản lý cấp cao, đồng quản lý
                            var listProjectFollowManagerId = context.ProjectEmployeeMapping.Where(c => c.EmployeeId == employee.EmployeeId).Select(c => c.ProjectId).ToList();

                            var listId = new List<Guid>();
                            listId.AddRange(listProjectFollowResourceId);
                            listId.AddRange(listProjectFollowManagerId);

                            listAllProject = listAllProject.Where(c => listId.Contains(c.ProjectId) || c.ProjectManagerId == employee.EmployeeId || c.CreateBy == user.UserId).ToList();
                        }
                    }

                }

                var listProject = listAllProject
                        .Select(m => new ProjectEntityModel
                        {
                            ProjectId = m.ProjectId,
                            ProjectCode = m.ProjectCode,
                            ProjectName = m.ProjectName
                        }).ToList();

                #endregion

                #region Hiển thị giá bán theo giờ theo phân quyền Quản lý và Quản lý cấp cao

                var isShowGiaBanTheoGio = false;

                //Nếu người đăng nhập là quản lý thì
                if (project.ProjectManagerId == employee.EmployeeId)
                {
                    isShowGiaBanTheoGio = true;
                }
                //Ngược lại
                else
                {
                    var exists = project.EmployeeSM.FirstOrDefault(x => x == employee.EmployeeId);

                    //Nếu người đăng nhập là quản lý cấp cao thì
                    if (exists != null && exists != Guid.Empty)
                    {
                        isShowGiaBanTheoGio = true;
                    }
                }

                #endregion

                return new GetMasterUpdateProjectResult
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "",
                    ListEmployee = listEmployee,
                    ListCustomer = listCustomer,
                    ListContract = listContract,
                    ListProjectType = listProjectType,
                    ListProjectScope = listProjectScope,
                    ListProjectStatus = listProjectStatus,
                    ListTargetType = listTargetType,
                    ListTargetUnit = listTargetUnit,
                    ListProjectTarget = listProjectTargets,
                    ListNote = listNote,
                    Project = project,
                    Role = role,
                    TotalRecordsNote = totalRecordsNote,
                    HasTaskInProgress = has != null ? true : false,
                    ListProject = listProject,
                    IsShowGiaBanTheoGio = isShowGiaBanTheoGio
                };
            }
            catch(Exception e)
            {
                return new GetMasterUpdateProjectResult
                {
                    MessageCode = e.Message,
                    StatusCode = HttpStatusCode.ExpectationFailed
                };
            }
        }

        public UpdateProjectResult UpdateProject(UpdateProjectParameter parameter)
        {
            try
            {
                var listAllUser = context.User.ToList();
                var listProjectCode = context.Project.Where(x => x.ProjectId != parameter.Project.ProjectId)
                    .Select(y => y.ProjectCode).ToList();
                var duplicateCode = 0;
                string noteDescription = "";

                if (listProjectCode.Count > 0)
                {
                    listProjectCode.ForEach(x =>
                    {
                        if (x.Equals(parameter.Project.ProjectCode))
                        {
                            duplicateCode++;
                        }
                    });
                }

                if (duplicateCode > 0)
                {
                    return new UpdateProjectResult()
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = "Mã dự án đã tồn tại",
                    };
                }

                //common
                var today = DateTime.Now;
                var listAllCategory = context.Category.ToList();
                var categoryTypeId = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "DAT")?.CategoryTypeId;
                var statusMoi =
                    listAllCategory.FirstOrDefault(x => x.CategoryTypeId == categoryTypeId && x.CategoryCode == "MOI")?.CategoryId;
                var statusDTK =
                    listAllCategory.FirstOrDefault(x => x.CategoryTypeId == categoryTypeId && x.CategoryCode == "DTK")?.CategoryId;
                var statusHTH =
                    listAllCategory.FirstOrDefault(x => x.CategoryTypeId == categoryTypeId && x.CategoryCode == "HTH")?.CategoryId;
                var statusDON =
                    listAllCategory.FirstOrDefault(x => x.CategoryTypeId == categoryTypeId && x.CategoryCode == "DON")?.CategoryId;
                var statusHUY =
                    listAllCategory.FirstOrDefault(x => x.CategoryTypeId == categoryTypeId && x.CategoryCode == "HUY")?.CategoryId;
                var statusTDU =
                    listAllCategory.FirstOrDefault(x => x.CategoryTypeId == categoryTypeId && x.CategoryCode == "TDU")?.CategoryId;

                //Lưu quản lý cấp cao
                var listEmployeeSm = context.ProjectEmployeeMapping
                    .Where(pe => pe.ProjectId == parameter.Project.ProjectId && pe.Type == 0).ToList();
                var listEmployeeSmIds = listEmployeeSm.Select(pe => pe.EmployeeId).ToList();
                var listEmployeeSmEntityModel = parameter.Project.EmployeeSM.ToList();
                if (listEmployeeSmEntityModel.Except(listEmployeeSmIds).Any() || 
                    listEmployeeSmIds.Except(listEmployeeSmEntityModel).Any())
                {
                    context.ProjectEmployeeMapping.RemoveRange(listEmployeeSm);
                    if (listEmployeeSmEntityModel?.Count > 0)
                    {
                        listEmployeeSmEntityModel.ForEach(item =>
                        {
                            context.ProjectEmployeeMapping.Add(new Entities.ProjectEmployeeMapping()
                            {
                                ProjectResourceMappingId = Guid.NewGuid(),
                                EmployeeId = item,
                                ProjectId = parameter.Project.ProjectId,
                                Type = 0 // Quản lý cấp cao
                            });
                        });
                    }
                }

                //Lưu sub quản lý 
                var listEmployeeSub = context.ProjectEmployeeMapping
                    .Where(pe => pe.ProjectId == parameter.Project.ProjectId && pe.Type == 1).ToList();
                var listEmployeeSubIds = listEmployeeSub.Select(pe => pe.EmployeeId).ToList();
                var listEmployeeSubEntityModel = parameter.Project.EmployeeSub.ToList();
                if (listEmployeeSubEntityModel.Except(listEmployeeSubIds).Any() || 
                    listEmployeeSubIds.Except(listEmployeeSubEntityModel).Any())
                {
                    context.ProjectEmployeeMapping.RemoveRange(listEmployeeSub);
                    if (listEmployeeSubEntityModel?.Count > 0)
                    {
                        listEmployeeSubEntityModel.ForEach(item =>
                        {
                            context.ProjectEmployeeMapping.Add(new Entities.ProjectEmployeeMapping()
                            {
                                ProjectResourceMappingId = Guid.NewGuid(),
                                EmployeeId = item,
                                ProjectId = parameter.Project.ProjectId,
                                Type = 1 // Sub quản lý 
                            });
                        });
                    }
                }
                //Lưu project
                var projectEntity = context.Project.FirstOrDefault(p => p.ProjectId == parameter.Project.ProjectId);
                var oldStatusProjectCode =
                    context.Category.FirstOrDefault(x => x.CategoryId == projectEntity.ProjectStatus)?.CategoryCode;
                var project = parameter.Project;
                if (projectEntity != null)
                {
                    var user = listAllUser.FirstOrDefault(x => x.UserId == parameter.UserId);

                    #region Update note nếu cập nhật trạng thái

                    if (projectEntity.ProjectStatus != project.ProjectStatus)
                    {
                        var stt = listAllCategory.FirstOrDefault(x =>
                            x.CategoryTypeId == categoryTypeId && x.CategoryId == project.ProjectStatus);
                        if (user != null)
                        {
                            if (stt != null)
                            {
                                noteDescription = "<p>- <strong>" + user.UserName + "</strong> thay đổi trạng thái của dự án sang " + stt.CategoryName + "</p>";
                            }
                        }
                        else
                        {
                            if (stt != null)
                            {
                                noteDescription = "<p>- Thay đổi trạng thái của dự án sang " + stt.CategoryName + "</p>";
                            }
                        }

                    }

                    #endregion

                    #region Update note nếu cập nhật ngân sách dự án

                    if (projectEntity.BudgetVnd != project.BudgetVnd)
                    {
                        if (user != null)
                        {
                            noteDescription += "<p>- <strong>" + user.UserName +
                                               "</strong> đã cập nhật ngân sách của dự án là " + string.Format(project.BudgetVnd.ToString(), "C") +
                                               " VND</p>";
                        }
                        else
                        {
                            noteDescription += "<p>- Cập nhật ngân sách của dự án là " + string.Format(project.BudgetVnd.ToString(), "C", new CultureInfo("vi-VN")) + " VND</p>";
                        }
                    }

                    if (projectEntity.BudgetUsd != project.BudgetUsd)
                    {
                        if (user != null)
                        {
                            noteDescription += "<p>- <strong>" + user.UserName +
                                               "</strong> đã cập nhật ngân sách của dự án là " + string.Format(project.BudgetUsd.ToString(), "C", new CultureInfo("vi-VN")) +
                                               " USD</p>";
                        }
                        else
                        {
                            noteDescription += "<p>- Cập nhật ngân sách của dự án là " + string.Format(project.BudgetUsd.ToString(), "C", new CultureInfo("vi-VN")) + " USD</p>";
                        }
                    }

                    if (projectEntity.BudgetNgayCong != project.BudgetNgayCong)
                    {
                        if (user != null)
                        {
                            noteDescription += "<p>- <strong>" + user.UserName +
                                               "</strong> đã cập nhật ngân sách của dự án là " + string.Format(project.BudgetNgayCong.ToString(), "C", new CultureInfo("vi-VN")) +
                                               " ngày công</p>";
                        }
                        else
                        {
                            noteDescription += "<p>- Cập nhật ngân sách của dự án là " + string.Format(project.BudgetNgayCong.ToString(), "C", new CultureInfo("vi-VN")) + " ngày công</p>";
                        }
                    }
                    #endregion

                    #region Update note nếu cập ngật thời gian dự kiến

                    var oldProjectStartDate =
                        projectEntity.ProjectStartDate;
                    var oldProjectEndDate = projectEntity.ProjectEndDate;

                    var newProjectStartDate = project.ProjectStartDate;
                    var newProjectEndDate = project.ProjectEndDate;



                    if (user != null)
                    {
                        if (newProjectStartDate != null && oldProjectStartDate != null)
                        {
                            if (newProjectStartDate.Value.Date != oldProjectStartDate.Value.Date)
                            {
                                noteDescription += "<p>- <strong>" + user.UserName + "</strong>" + " thay đổi ngày bắt đầu dự kiến từ " +
                                                   "<strong>" + oldProjectStartDate.Value.Date.ToString("dd/MM/yyyy") + "</strong>" + " sang ngày " +
                                                   "<strong>" + newProjectStartDate.Value.Date.ToString("dd/MM/yyyy") + "</strong></p>";
                            }

                        }
                        else if (newProjectStartDate != null && oldProjectStartDate == null)
                        {
                            noteDescription += "<p>- <strong>" + user.UserName + "</strong>" + " cập nhật ngày bắt đầu dự kiến thành " +
                                              "<strong>" + newProjectStartDate.Value.Date.ToString("dd/MM/yyyy") + "</strong></p>";
                        }
                        else if (newProjectStartDate == null && oldProjectStartDate != null)
                        {
                            noteDescription += "<p>- <strong>" + user.UserName + "</strong>" + " đã xóa ngày bắt đầu dự kiến.</p>";
                        }

                    }
                    else
                    {
                        if (newProjectStartDate != null && oldProjectStartDate != null)
                        {
                            if (newProjectStartDate != null && oldProjectStartDate != null)
                            {
                                noteDescription += "<p>- Thay đổi ngày bắt đầu dự kiến từ " +
                                                   "<strong>" + oldProjectStartDate.Value.Date.ToString("dd/MM/yyyy") + "</strong>" + " sang ngày " +
                                                   "<strong>" + newProjectStartDate.Value.Date.ToString("dd/MM/yyyy") + "</strong></p>";
                            }
                        }
                        else if (newProjectStartDate != null && oldProjectStartDate == null)
                        {
                            noteDescription += "<p>- Cập nhật ngày bắt đầu dự kiến thành " +
                                              "<strong>" + newProjectStartDate.Value.Date.ToString("dd/MM/yyyy") + "</strong></p>";
                        }
                        else if (newProjectStartDate == null && oldProjectStartDate != null)
                        {
                            noteDescription += "<p>- Xóa ngày bắt đầu dự kiến</p>";
                        }
                    }



                    if (user != null)
                    {
                        if (newProjectEndDate != null && oldProjectEndDate != null)
                        {
                            if (newProjectEndDate.Value.Date != oldProjectEndDate.Value.Date)
                            {
                                noteDescription += "<p>- <strong>" + user.UserName + "</strong>" + " thay đổi ngày kết thúc dự kiến từ " +
                                                   "<strong>" + oldProjectEndDate.Value.Date.ToString("dd/MM/yyyy") + "</strong>" + " sang ngày " +
                                                   "<strong>" + newProjectEndDate.Value.Date.ToString("dd/MM/yyyy") + "</strong></p>";
                            }
                        }
                        else if (newProjectEndDate != null && oldProjectEndDate == null)
                        {
                            noteDescription += "<p>- <strong>" + user.UserName + "</strong>" + " cập nhật ngày kết thúc dự kiến thành " +
                                               "<strong>" + newProjectEndDate.Value.Date.ToString("dd/MM/yyyy") + "</strong></p>";
                        }
                        else if (newProjectEndDate == null && oldProjectEndDate != null)
                        {
                            noteDescription += " -<p><strong>" + user.UserName + "</strong>" + " đã xóa ngày kết thúc dự kiến.</p>";
                        }

                    }
                    else
                    {
                        if (newProjectEndDate != null && oldProjectEndDate != null)
                        {
                            if (newProjectEndDate.Value.Date != oldProjectEndDate.Value.Date)
                            {
                                noteDescription += "<p>- Thay đổi ngày kết thúc dự kiến từ " +
                                                   "<strong>" + oldProjectEndDate.Value.Date.ToString("dd/MM/yyyy") + "</strong>" + " sang ngày " +
                                                   "<strong>" + newProjectEndDate.Value.Date.ToString("dd/MM/yyyy") + "</strong></p>";
                            }
                        }
                        else if (newProjectEndDate != null && oldProjectEndDate == null)
                        {
                            noteDescription += "<p>- Cập nhật ngày kết thúc dự kiến thành " +
                                               "<strong>" + newProjectEndDate.Value.Date.ToString("dd/MM/yyyy") + "</strong></p>";
                        }
                        else if (newProjectEndDate == null && oldProjectEndDate != null)
                        {
                            noteDescription += "- <p>Xóa ngày kết thúc dự kiến.</p>";
                        }
                    }


                    #endregion

                    #region Update note nếu cập ngật thời gian thực tế

                    var oldStartDate =
                        projectEntity.ActualStartDate;
                    var oldEndDate = projectEntity.ActualEndDate;

                    var newStartDate = project.ActualStartDate;
                    var newEndDate = project.ActualEndDate;

                    if (user != null)
                    {
                        if (newStartDate != null && oldStartDate != null)
                        {
                            if (newStartDate.Value.Date != oldStartDate.Value.Date)
                            {
                                noteDescription += "<p>- <strong>" + user.UserName + "</strong>" + " thay đổi ngày bắt đầu thực tế từ " +
                                                   "<strong>" + oldStartDate.Value.Date.ToString("dd/MM/yyyy") + "</strong>" + " sang ngày " +
                                                   "<strong>" + newStartDate.Value.Date.ToString("dd/MM/yyyy") + "</strong></p>";
                            }
                        }
                        else if (newStartDate != null && oldStartDate == null)
                        {
                            noteDescription += "<p>- <strong>" + user.UserName + "</strong>" + " cập nhật ngày bắt đầu thực tế thành " +
                                              "<strong>" + newStartDate.Value.Date.ToString("dd/MM/yyyy") + "</strong></p>";
                        }
                        else if (newStartDate == null && oldStartDate != null)
                        {
                            noteDescription += "<p>- <strong>" + user.UserName + "</strong>" + " đã xóa ngày bắt đầu thực tế.</p>";
                        }

                    }
                    else
                    {
                        if (newStartDate != null && oldStartDate != null)
                        {
                            if (newStartDate.Value.Date != oldStartDate.Value.Date)
                            {
                                noteDescription += "<p>- Thay đổi ngày bắt đầu dự thực tế " +
                                                   "<strong>" + oldStartDate.Value.Date.ToString("dd/MM/yyyy") + "</strong>" + " sang ngày " +
                                                   "<strong>" + newStartDate.Value.Date.ToString("dd/MM/yyyy") + "</strong></p>";
                            }
                        }
                        else if (newStartDate != null && oldStartDate == null)
                        {
                            noteDescription += "<p>- Cập nhật ngày bắt đầu thực tế thành " +
                                              "<strong>" + newStartDate.Value.Date.ToString("dd/MM/yyyy") + "</strong></p>";
                        }
                        else if (newStartDate == null && oldStartDate != null)
                        {
                            noteDescription += "<p>- Xóa ngày bắt đầu dự kiến.</p>";
                        }
                    }



                    if (user != null)
                    {
                        if (newEndDate != null && oldEndDate != null)
                        {
                            if (newEndDate.Value.Date != oldEndDate.Value.Date)
                            {
                                noteDescription += "<p>- <strong>" + user.UserName + "</strong>" + " thay đổi ngày kết thúc thực tế từ " +
                                                   "<strong>" + oldEndDate.Value.Date.ToString("dd/MM/yyyy") + "</strong>" + " sang ngày " +
                                                   "<strong>" + newEndDate.Value.Date.ToString("dd/MM/yyyy") + "</strong></p>";
                            }
                        }
                        else if (newEndDate != null && oldEndDate == null)
                        {
                            noteDescription += "<p>- <strong>" + user.UserName + "</strong>" + " cập nhật ngày kết thúc thực tế thành " +
                                              "<strong>" + newEndDate.Value.Date.ToString("dd/MM/yyyy") + "</strong></p>";
                        }
                        else if (newEndDate == null && oldEndDate != null)
                        {
                            noteDescription += "<p>- <strong>" + user.UserName + "</strong>" + " đã xóa ngày kết thúc thực tế.</p>";
                        }

                    }
                    else
                    {
                        if (newEndDate != null && oldEndDate != null)
                        {
                            if (newEndDate.Value.Date != oldEndDate.Value.Date)
                            {
                                noteDescription += "<p>- Thay đổi ngày kết thúc thực tế từ " +
                                                   "<strong>" + oldEndDate.Value.Date.ToString("dd/MM/yyyy") + "</strong>" + " sang ngày " +
                                                   "<strong>" + newEndDate.Value.Date.ToString("dd/MM/yyyy") + "</strong></p>";
                            }
                        }
                        else if (newEndDate != null && oldEndDate == null)
                        {
                            noteDescription += "<p>- Cập nhật ngày kết thúc thực tế thành " +
                                              "<strong>" + newEndDate.Value.Date.ToString("dd/MM/yyyy") + "</strong></p>";
                        }
                        else if (newEndDate == null && oldEndDate != null)
                        {
                            noteDescription += "<p>- Xóa ngày kết thúc thực tế.</p>";
                        }
                    }


                    #endregion

                    #region Update note nếu cập nhật mục tiêu dự án

                    var _newListDetail =
                        parameter.ListProjectTarget.OrderBy(x => x.OrderNumber).ToList();
                    var _oldListDetail =
                        context.ProjectObjective.Where(co => co.ProjectId == parameter.Project.ProjectId).OrderBy(x => x.OrderNumber).ToList();

                    CompareLogic compareLogic = new CompareLogic();
                    var listIgnorFieldDetail = new List<string> { "ProjectObjectId", "ProjectId", "TenantId", "OrderNumber" };
                    compareLogic.Config.MembersToIgnore = listIgnorFieldDetail;
                    ComparisonResult detailCompare = compareLogic.Compare(_oldListDetail, _newListDetail);
                    if (!detailCompare.AreEqual)
                    {
                        if (user != null)
                        {

                            // Cập nhật mục tiêu
                            _newListDetail.ForEach(itemNew =>
                            {
                                _oldListDetail.ForEach(itemOld =>
                                {
                                    if (itemNew.OrderNumber == itemOld.OrderNumber)
                                    {
                                        #region Kiểm tra thay đổi trên object

                                        CompareLogic compareLogicObject = new CompareLogic();
                                        var listIgnorField = new List<string> { "ProjectObjectId", "ProjectId", "TenantId", "OrderNumber" };
                                        compareLogicObject.Config.MembersToIgnore = listIgnorField;
                                        ComparisonResult compare = compareLogicObject.Compare(itemOld, itemNew);

                                        //Nếu có thay đổi
                                        if (!compare.AreEqual)
                                        {
                                            noteDescription += "<p>- <strong>" + user.UserName + "</strong>" + " thay đổi mục tiêu dự án: " + itemNew.ProjectObjectName + "</p>";
                                        }

                                        #endregion
                                    }
                                });
                            });


                            // thêm mục tiêu
                            if (_newListDetail.Count > _oldListDetail.Count)
                            {
                                _newListDetail.ForEach(itemNew =>
                                {
                                    if (itemNew.OrderNumber > _oldListDetail.Count)
                                    {
                                        noteDescription += "<p>- <strong>" + user.UserName + "</strong>" + " thêm mục tiêu dự án: " + itemNew.ProjectObjectName + "</p>";
                                    }
                                });
                            }

                            // Xóa mục tiêu
                            if (_newListDetail.Count < _oldListDetail.Count)
                            {
                                var listOrder = new List<int?>();
                                int count = 0;
                                _oldListDetail.ForEach(itemOld =>
                                {
                                    _newListDetail.ForEach(itemNew =>
                                    {
                                        if (itemOld.OrderNumber == itemNew.OrderNumber)
                                        {
                                            listOrder.Add(itemNew.OrderNumber);
                                            count++;
                                        }
                                    });
                                });

                                if (count > 0)
                                {
                                    _oldListDetail.ForEach(itemOld =>
                                    {
                                        if (!listOrder.Contains(itemOld.OrderNumber))
                                        {
                                            noteDescription += "<p>- <strong>" + user.UserName + "</strong>" + " Xóa mục tiêu dự án: " + itemOld.ProjectObjectName + "</p>";
                                        }
                                    });
                                }
                            }
                        }
                    }

                    #endregion

                    // cap nhat trang thai du an truoc khi set

                    #region Logic khi nguoi dung thay doi ngay thuc te

                    // nguoi dung cap nhat ngay bat dau thuc te tai trang thai moi cua project
                    if (projectEntity.ProjectStatus == statusMoi)
                    {
                        if (project.ActualStartDate != null && projectEntity.ActualStartDate == null)
                        {
                            if (project.ActualStartDate.Value.Date <= today.Date)
                            {
                                projectEntity.ProjectStatus = statusDTK;
                            }
                        }
                        else if (project.ActualStartDate != null && projectEntity.ActualStartDate != null && projectEntity.ActualStartDate.Value.Date != project.ActualStartDate.Value.Date)
                        {
                            if (project.ActualStartDate.Value.Date <= today.Date)
                            {
                                projectEntity.ProjectStatus = statusDTK;
                            }
                        }
                    }
                    // update trang thai theo nguoi dung nhap
                    else
                    {
                        projectEntity.ProjectStatus = project.ProjectStatus;
                    }

                    // nguoi dung cap nhat ngay ket thuc thuc te tai trang thai dang trien khai hoac hoan thanh cua project
                    if (projectEntity.ProjectStatus == statusDTK || projectEntity.ProjectStatus == statusHTH)
                    {
                        if (project.ActualEndDate != null && projectEntity.ActualEndDate == null)
                        {
                            if (project.ActualEndDate.Value.Date <= today.Date)
                            {
                                projectEntity.ProjectStatus = statusDON;
                            }
                        }
                        else if (project.ActualEndDate != null && projectEntity.ActualEndDate != null && projectEntity.ActualEndDate.Value.Date != project.ActualEndDate.Value.Date)
                        {
                            if (project.ActualEndDate.Value.Date <= today.Date)
                            {
                                projectEntity.ProjectStatus = statusDON;
                            }
                        }
                    }
                    // update trang thai theo nguoi dung nhap
                    else
                    {
                        projectEntity.ProjectStatus = project.ProjectStatus;
                    }

                    #endregion

                    /* Nếu thay đổi Hợp đồng or Khách hàng thì check:
                     *  TH1: Có hạng mục và có task => Không cho lưu => Cảnh báo => Tách hàm
                     *  TH2: Có hạng mục - Có KH và HĐ có Phạm vi công việc => Copy phạm vi công việc sang
                     *  TH3  Có hạng mục - Có KH hoặc HĐ mà không có Phạm vi công việc => Tạo node mặc định là tên dự án
                     *  TH4: Có hạng mục - Không có task - Không có HĐ hay gì gì => Tạo node mặc định là tên dự án
                     * */

                    // Tạo phạm vi công việc mặc định
                    // Chọn HĐ liên quan nhưng hợp đồng không gắn với báo giá 
                    var contractNotQuote = context.Contract.Where(x =>
                            (x.QuoteId == null || x.QuoteId == Guid.Empty) &&
                            x.ContractId == parameter.Project.ContractId)
                        .ToList();
                    // Chọn HĐ gắn với báo giá nhưng báo giá báo giá không có giá trị tại tab Phạm vi công việc
                    int countContract = 0;
                    var contractQuote = context.Contract.FirstOrDefault(x =>
                        (x.QuoteId != null && x.QuoteId != Guid.Empty) && x.ContractId == parameter.Project.ContractId);
                    if (contractQuote != null)
                    {
                        countContract = context.QuoteScope.Where(x => x.QuoteId == contractQuote.QuoteId).ToList().Count();
                    }
                    var projectScope = context.ProjectScope.Where(x => x.ProjectId == projectEntity.ProjectId).ToList();

                    #region Nếu thay đổi khách hàng HOẶC HỢP ĐÔNG => XÓA PROJECTSCOPE - trừ note đầu tiên
                    if (projectEntity.CustomerId != project.CustomerId || projectEntity.ContractId != project.ContractId)
                    {
                        var projectScopeRemove = projectScope.Where(x => x.ParentId != null).ToList();
                        if (projectScopeRemove.Count > 0)
                        {
                            context.ProjectScope.RemoveRange(projectScopeRemove);
                            context.SaveChanges();
                        }
                    }
                    #endregion

                    // Lấy ra node chính của project scope của dự án

                    #region Nếu thay đổi hợp đồng           
                    if (countContract > 0 && (projectEntity.CustomerId != project.CustomerId || projectEntity.ContractId != project.ContractId))
                    {
                        // Id của hạng mục đầu tiên
                        var firstProjectScopeId = projectScope.FirstOrDefault(x => x.ParentId == null).ProjectScopeId;

                        var contract = context.Contract.FirstOrDefault(x => x.ContractId == parameter.Project.ContractId);
                        // Danh sách Phạm vi công việc (Ngoại trừ note đầu tiên)
                        var scopes = context.QuoteScope.Where(x => x.QuoteId == contract.QuoteId).OrderBy(x => x.Level).ToList();

                        List<QuoteScope> lstNewQuoteScopes = new List<QuoteScope>();
                        lstNewQuoteScopes = scopes.Select(q => new QuoteScope()
                        {
                            ScopeId = q.ScopeId,
                            QuoteId = q.QuoteId,
                            Tt = q.Tt,
                            Category = q.Category,
                            Description = q.Description,
                            CreatedDate = q.CreatedDate,
                            CreatedById = q.CreatedById,
                            ParentId = q.ParentId,
                            Level = q.Level
                        }).ToList();

                        List<CloneProjectScopeModel> lstNewOldId = new List<CloneProjectScopeModel>();
                        // Vòng lặp qua các level
                        for (int i = 0; i < lstNewQuoteScopes.Count(); i++)
                        {
                            if (i == 0)
                            {
                                var newScopeId = Guid.NewGuid();
                                lstNewQuoteScopes.Where(x => x.Level == i).ToList().ForEach(qout =>
                                {
                                    // Thêm vào list
                                    lstNewOldId.Add(new CloneProjectScopeModel
                                    {
                                        NewScopeId = newScopeId,
                                        OldScopeId = qout.ScopeId
                                    });

                                    qout.ScopeId = newScopeId;
                                    qout.ParentId = qout.ParentId;

                                });
                            }
                            else
                            {
                                lstNewQuoteScopes.Where(x => x.Level == i).ToList().ForEach(qout =>
                                {
                                    var newScopeId = Guid.NewGuid();
                                    // Thêm vào list
                                    lstNewOldId.Add(new CloneProjectScopeModel
                                    {
                                        NewScopeId = newScopeId,
                                        OldScopeId = (Guid)qout.ScopeId
                                    });

                                    qout.ScopeId = newScopeId;
                                    qout.ParentId = lstNewOldId.FirstOrDefault(a => a.OldScopeId == qout.ParentId).NewScopeId;
                                });
                            }
                        }

                        // Xóa note đầu tiên
                        lstNewQuoteScopes.RemoveAt(0);
                        // Cập nhập lại parentId cho level 1 là projectScopeId của note mới được tạo                        
                        lstNewQuoteScopes.ForEach(item =>
                        {
                            if (item.Level == 1)
                                item.ParentId = firstProjectScopeId;
                        });

                        lstProjectScope = lstNewQuoteScopes.Select(q => new ProjectScope()
                        {
                            ProjectScopeId = q.ScopeId,
                            ProjectId = parameter.Project.ProjectId,
                            ProjectScopeCode = q.Tt,
                            ProjectScopeName = q.Category,
                            Description = q.Description,
                            CreateDate = DateTime.Now,
                            CreateBy = parameter.UserId,
                            ParentId = q.ParentId,
                            Level = q.Level
                        }).ToList();

                        if (lstProjectScope.Count() > 0)
                        {
                            context.ProjectScope.AddRange(lstProjectScope);
                        }
                    }


                    #endregion

                    projectEntity.ProjectCode = project.ProjectCode;
                    projectEntity.ProjectName = project.ProjectName;
                    projectEntity.CustomerId = project.CustomerId;
                    projectEntity.ContractId = project.ContractId;
                    projectEntity.ProjectManagerId = project.ProjectManagerId;
                    projectEntity.Description = project.Description;
                    projectEntity.ProjectType = project.ProjectType;
                    projectEntity.ProjectSize = project.ProjectSize;
                    projectEntity.BudgetVnd = project.BudgetVnd;
                    projectEntity.BudgetUsd = project.BudgetUsd;
                    projectEntity.BudgetNgayCong = project.BudgetNgayCong;
                    projectEntity.Priority = project.Priority;
                    projectEntity.ProjectStartDate = project.ProjectStartDate;
                    projectEntity.ProjectEndDate = project.ProjectEndDate;
                    projectEntity.ActualStartDate = project.ActualStartDate;
                    projectEntity.ActualEndDate = project.ActualEndDate;
                    projectEntity.IncludeWeekend = project.IncludeWeekend;
                    projectEntity.GiaBanTheoGio = project.GiaBanTheoGio;
                    projectEntity.ProjectStatusPlan = project.ProjectStatusPlan;
                    projectEntity.NgayKyNghiemThu = project.NgayKyNghiemThu;

                    projectEntity.UpdateBy = parameter.UserId;
                    projectEntity.UpdateDate = DateTime.Now;
                    projectEntity.LastChangeActivityDate = DateTime.Now;

                    #region Nếu thay đổi trạng thái

                    if (projectEntity.ProjectStatus == statusDTK)
                    {
                        if (projectEntity.ActualStartDate == null)
                        {
                            projectEntity.ActualStartDate = DateTime.Now;
                        }
                    }

                    if (projectEntity.ProjectStatus == statusDON)
                    {
                        if (projectEntity.ActualEndDate == null)
                        {
                            projectEntity.ActualEndDate = DateTime.Now;
                        }
                    }

                    #endregion
                }

                context.Update(projectEntity);
                context.SaveChanges();

                #region Cập nhật dòng thười gian

                if (noteDescription != null && noteDescription != "")
                {
                    Note note = new Note
                    {
                        NoteId = Guid.NewGuid(),
                        Type = "SYS",
                        ObjectId = project.ProjectId,
                        ObjectType = "Project",
                        Active = true,
                        Description = noteDescription,
                        CreatedById = parameter.UserId,
                        CreatedDate = DateTime.Now,
                        NoteTitle = "đã thêm ghi chú"
                    };

                    context.Note.Add(note);
                    context.SaveChanges();
                }

                #endregion

                #region Thông báo

                NotificationHelper.AccessNotification(context, TypeModel.ProjectDetail, "UPD", new Project(),
                    projectEntity, true);

                #endregion

                //Thêm danh sach mục tiêu
                var projectTargets = context.ProjectObjective.Where(pe => pe.ProjectId == parameter.Project.ProjectId).ToList();
                context.ProjectObjective.RemoveRange(projectTargets);
                List<ProjectObjective> listprojectTarget = new List<ProjectObjective>();
                parameter.ListProjectTarget.ForEach(item =>
                {
                    var target = new ProjectObjective();

                    target.ProjectObjectId = Guid.NewGuid();
                    target.OrderNumber = item.OrderNumber;
                    target.ProjectObjectName = item.ProjectObjectName;
                    target.ProjectObjectType = item.ProjectObjectType;
                    target.ProjectObjectUnit = item.ProjectObjectUnit;
                    target.ProjectObjectDescription = item.ProjectObjectDescription;
                    target.ProjectObjectValue = item.ProjectObjectValue;
                    target.ProjectId = parameter.Project.ProjectId;
                    listprojectTarget.Add(target);
                });
                context.ProjectObjective.AddRange(listprojectTarget);
                context.SaveChanges();

                return new UpdateProjectResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Cập nhật dự án thành công.",
                    ProjectId = parameter.Project.ProjectId
                };
            }
            catch(Exception e)
            {
                return new UpdateProjectResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }         
        }

        public GetProjectScopeResult GetProjectScope(GetProjectScopeParameter parameter)
        {
            try
            {
                var listNote = new List<NoteEntityModel>();
                var projectScopes = context.ProjectScope.Where(x => x.ProjectId == parameter.ProjectId).ToList();
                var projectScopeGuids = projectScopes.Select(ps => ps.ProjectScopeId).ToList();
                var tasks = context.Task.Where(x => projectScopeGuids.Contains((Guid)x.ProjectScopeId)).ToList();
                int pageSize = 10;
                int pageIndex = 1;
                int totalRecordsNote = 0;

                #region Trạng thái công việc
                var listCategoryTypeCodes = new List<string> { "DAT", "PSC", "LDA", "CVT", "NCNL", "TTCV" };
                var listCategory = context.Category.Where(x => listCategoryTypeCodes.Contains(x.CategoryType.CategoryTypeCode) && x.Active == true).Select(y =>
                                   new CategoryEntityModel
                                   {
                                       CategoryId = y.CategoryId,
                                       CategoryName = y.CategoryName,
                                       CategoryCode = y.CategoryCode,
                                       CategoryTypeId = Guid.Empty,
                                       CreatedById = Guid.Empty,
                                       CategoryTypeCode = y.CategoryType.CategoryTypeCode,
                                       CountCategoryById = 0
                                   }).ToList();

                var listStatus = listCategory.Where(c => c.CategoryTypeCode == "TTCV").ToList();
                #endregion

                #region Thông tin dự án 
                var project = context.Project.Where(x => x.ProjectId == parameter.ProjectId).Select(y => new ProjectEntityModel
                {
                    ProjectId = y.ProjectId,
                    ProjectStartDate = y.ProjectStartDate,
                    ProjectEndDate = y.ProjectEndDate,
                    ActualStartDate = y.ActualStartDate,
                    ActualEndDate = y.ActualEndDate,
                    ProjectManagerId = y.ProjectManagerId,
                    ContractId = y.ContractId,
                    ProjectName = y.ProjectName,
                    ProjectCode = y.ProjectCode,
                    BudgetVnd = y.BudgetVnd,
                    BudgetUsd = y.BudgetUsd,
                    BudgetNgayCong = y.BudgetNgayCong,
                    // Butget = y.ButgetType == 1 ? y.Butget : 0,
                    // ButgetType = y.ButgetType,
                    CustomerId = y.CustomerId,
                    Description = y.Description,
                    ProjectSize = y.ProjectSize,
                    ProjectType = y.ProjectType,
                    ProjectStatus = y.ProjectStatus,
                    IncludeWeekend = y.IncludeWeekend,
                    Priority = y.Priority,
                }).FirstOrDefault();
                project.ProjectTypeName = listCategory.FirstOrDefault(c => c.CategoryId == project.ProjectType)?.CategoryName;
                project.ProjectStatusName = listCategory.FirstOrDefault(c => c.CategoryId == project.ProjectStatus)?.CategoryName;
                project.ProjectStatusCode = listCategory.FirstOrDefault(c => c.CategoryId == project.ProjectStatus)?.CategoryCode;
                if (project.Priority == 1) project.PriorityName = "Thấp";
                else if (project.Priority == 2) project.PriorityName = "Trung bình";
                else project.PriorityName = "Cao";

                TaskDAO _taskDao = new TaskDAO(context, iHostingEnvironment);
                _taskDao.CaculatorProjectTask(parameter.ProjectId, out decimal projectComplete, out decimal totalEstimateHour);

                project.TaskComplate = projectComplete;
                project.EstimateCompleteTime = totalEstimateHour;
                if (project == null)
                {
                    return new GetProjectScopeResult()
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = "Dự án không tồn tại trên hệ thống"
                    };
                }
                #endregion

                #region Danh sách scope thuộc dự án
                var listProjectScope = projectScopes?.Select(p => new ProjectScopeModel()
                {
                    ProjectScopeId = p.ProjectScopeId,
                    Description = p.Description,
                    ResourceType = p.ResourceType,
                    ProjectScopeName = p.ProjectScopeName,
                    ProjectScopeCode = p.ProjectScopeCode,
                    TenantId = p.TenantId,
                    ParentId = p.ParentId,
                    ProjectId = p.ProjectId,
                    Level = p.Level == null ? 0 : p.Level
                }).ToList();

                listProjectScope?.ForEach(item =>
                {
                    // danh sách các task thuộc hạng mục
                    var listTask = tasks.Where(x => x.ProjectScopeId == item.ProjectScopeId).ToList();
                    var listTaskId = listTask.Select(x => x.TaskId).ToList();
                    var listTaskRe = context.TaskResourceMapping
                        .Where(x => x.IsPersonInCharge == true && listTaskId.Contains(x.TaskId))
                        .Select(a => a.ResourceId).ToList();
                    var listProjectRe = context.ProjectResource.Where(x => listTaskRe.Contains(x.ProjectResourceId))
                        .Select(a => a.ObjectId).ToList();
                    var lstEmp = context.Employee.Where(e => listProjectRe.Contains(e.EmployeeId))
                        .Select(em => em.EmployeeCode + '-' + em.EmployeeName).ToList();
                    var listResourceScope = new List<string>();
                    lstEmp.ForEach(emp => { listResourceScope.Add(lstEmp.Count > 0 ? emp : string.Empty); });
                    item.ListEmployee = listResourceScope;

                    // Danh sách task thuộc note và task thuộc con của note đấy
                    List<Guid> lstNode = new List<Guid>();
                    lstNode.Add(item.ProjectScopeId);
                    var listScopeId = listProjectScope.Where(x => x.ParentId == item.ProjectScopeId)
                        .Select(x => x.ProjectScopeId).ToList();
                    while (listScopeId.Count() != 0)
                    {
                        lstNode.AddRange(listScopeId);
                        listScopeId = listProjectScope
                            .Where(x => x.ParentId != null && listScopeId.Contains(x.ParentId.Value))
                            ?.Select(x => x.ProjectScopeId).ToList();
                    }

                    // Danh sách công việc thuộc gói
                    var lstTaskOfScope = tasks.Where(x => lstNode.Contains((Guid) x.ProjectScopeId)).ToList();

                    // Nếu tất cả là mới HOẶC tồn tại 1 trạng thái mới => MỚI
                    if (lstTaskOfScope.FirstOrDefault(x =>
                            x.Status == listStatus.Select(a => new {a.CategoryId, a.CategoryCode})
                                .Where(s => s.CategoryCode == "NEW").FirstOrDefault().CategoryId) != null && (
                            lstTaskOfScope.FindAll(x =>
                                x.Status == listStatus.Select(a => new {a.CategoryId, a.CategoryCode})
                                    .Where(s => s.CategoryCode == "NEW").FirstOrDefault().CategoryId).Count() ==
                            lstTaskOfScope.Count()
                            || lstTaskOfScope.FindAll(x =>
                                x.Status == listStatus.Select(a => new {a.CategoryId, a.CategoryCode})
                                    .Where(s => s.CategoryCode == "NEW").FirstOrDefault().CategoryId).Count() > 0))
                    {
                        //item.StyleClass = "node-NEW";
                    }
                    else
                    {
                        // Nếu tồn tại 1 trạng thái đang thực hiện => ĐANG THỰC HIỆN
                        if (lstTaskOfScope.FirstOrDefault(x =>
                                x.Status == listStatus.Select(a => new {a.CategoryId, a.CategoryCode})
                                    .Where(s => s.CategoryCode == "DL").FirstOrDefault().CategoryId) != null)
                        {
                            item.StyleClass = "node-DL";
                        }
                        // Nếu tồn tại 1 task hoàn thành (các task còn lại là đóng) => HOÀN THÀNH
                        // Nếu tất cả task là hoàn thành => HOÀN THÀNH
                        else
                        {
                            if (lstTaskOfScope.FirstOrDefault(x =>
                                    x.Status == listStatus.Select(a => new {a.CategoryId, a.CategoryCode})
                                        .Where(s => s.CategoryCode == "HT").FirstOrDefault().CategoryId) != null
                                && (
                                    (
                                        lstTaskOfScope.FindAll(x =>
                                                x.Status == listStatus.Select(a => new {a.CategoryId, a.CategoryCode})
                                                    .Where(s => s.CategoryCode == "HT").FirstOrDefault().CategoryId)
                                            .Count() < lstTaskOfScope.Count()
                                        && lstTaskOfScope.FindAll(x =>
                                                x.Status == listStatus.Select(a => new {a.CategoryId, a.CategoryCode})
                                                    .Where(s => s.CategoryCode == "NEW").FirstOrDefault().CategoryId)
                                            .ToList().Count() == 0
                                        && lstTaskOfScope.FindAll(x =>
                                                x.Status == listStatus.Select(a => new {a.CategoryId, a.CategoryCode})
                                                    .Where(s => s.CategoryCode == "DL").FirstOrDefault().CategoryId)
                                            .ToList().Count() == 0)
                                    || (lstTaskOfScope.FindAll(x =>
                                                x.Status == listStatus.Select(a => new {a.CategoryId, a.CategoryCode})
                                                    .Where(s => s.CategoryCode == "HT").FirstOrDefault().CategoryId)
                                            .Count() == lstTaskOfScope.Count())
                                ))
                            {
                                item.StyleClass = "node-HT";
                            }
                            else
                                // Nếu tất cả task là đóng => ĐÓNG
                            if (lstTaskOfScope.FirstOrDefault(x =>
                                    x.Status == listStatus.Select(a => new {a.CategoryId, a.CategoryCode})
                                        .Where(s => s.CategoryCode == "CLOSE").FirstOrDefault().CategoryId) != null &&
                                lstTaskOfScope.FindAll(x =>
                                    x.Status == listStatus.Select(a => new {a.CategoryId, a.CategoryCode})
                                        .Where(s => s.CategoryCode == "CLOSE").FirstOrDefault().CategoryId).Count() ==
                                lstTaskOfScope.Count())
                            {
                                item.StyleClass = "node-CLOSE";
                            }
                        }
                    }
                });
                // Format lại số thứ tự 1 - 1.1 - 1.2 cho danh sách hạng mục
                listProjectScope = SetTTChildren(listProjectScope);

                #endregion

                #region Dánh sách task thuộc dự án - tab danh sách công việc
                var listProjectTask = tasks.Select(p => new TaskEntityModel()
                {
                    ProjectScopeId = p.ProjectScopeId,
                    Description = p.Description,
                    TaskCode = p.TaskCode,
                    TaskId = p.TaskId,
                    TaskName = p.TaskName,
                    PlanStartTime = p.PlanStartTime,
                    PlanEndTime = p.PlanEndTime,
                    Status = p.Status,
                    ActualEndTime = p.ActualEndTime,
                    ActualStartTime = p.ActualStartTime,
                    ActualHour = p.ActualHour,
                    EstimateCost = p.EstimateCost,
                    EstimateHour = p.EstimateHour,
                    TaskComplate = p.TaskComplate,
                    ProjectScopeName = listProjectScope.FirstOrDefault(s => s.ProjectScopeId == p.ProjectScopeId) != null ? listProjectScope.FirstOrDefault(s => s.ProjectScopeId == p.ProjectScopeId).ProjectScopeCode + ". " + listProjectScope.FirstOrDefault(s => s.ProjectScopeId == p.ProjectScopeId).ProjectScopeName : "",

                }).OrderByDescending(x => x.CreateDate).ToList();
                // Danh sách bộ lọc gói công việc            
                listProjectTask.ForEach(task =>
                {
                    var status = listStatus.FirstOrDefault(c => c.CategoryId == task.Status);
                    task.StatusName = status?.CategoryName;
                    switch (status.CategoryCode)
                    {
                        case "NEW":
                            task.BackgroundColorForStatus = "#0F62FE";
                            task.IsDelete = true;
                            break;
                        case "DL":
                            task.BackgroundColorForStatus = "#FFC000";
                            task.IsDelete = false;
                            break;
                        case "HT":
                            task.BackgroundColorForStatus = "#63B646";
                            task.IsDelete = false;
                            break;
                        case "CLOSE":
                            task.BackgroundColorForStatus = "#9C00FF";
                            task.IsDelete = false;
                            break;
                    }

                    var category = listCategory.FirstOrDefault(c => c.CategoryId == task.Status);
                    if (category != null)
                    {
                        task.StatusName = category.CategoryName;
                        task.CanEdit = category.CategoryCode != "CLOSE" ? true : false;
                    }
                    // danh sách các task thuộc hạng mục
                    var listTaskRe = context.TaskResourceMapping.Where(x => x.IsPersonInCharge == true && x.TaskId == task.TaskId).Select(a => a.ResourceId).ToList();
                    var listProjectRe = context.ProjectResource.Where(x => listTaskRe.Contains(x.ProjectResourceId)).Select(a => a.ObjectId).ToList();
                    var employees = context.Employee.Where(e => listProjectRe.Contains(e.EmployeeId)).Select(em => em.EmployeeCode + '-' + em.EmployeeName).ToList();
                    task.Employee = employees.Count > 0 ? employees.Aggregate((a, x) => a + ", " + x) : string.Empty;
                });

                #endregion

                #region Lấy danh sách nhà cung cấp
                var listVendor = context.Vendor.Select(y =>
                           new VendorEntityModel
                           {
                               VendorCode = y.VendorCode,
                               VendorName = y.VendorName,
                               VendorId = y.VendorId,
                               Active = y.Active
                           }).ToList();
                #endregion

                #region Danh sách nguồn
                var listResource = listCategory.Where(c => c.CategoryTypeCode == "NCNL").ToList();
                #endregion

                #region get list notes
                //var lstTaskId = context.Task.Where(x => x.ProjectId == parameter.ProjectId).Select(a => a.TaskId).ToList();
                listNote = context.Note.Where(w => w.Active == true && w.ObjectType == "PROSCOPE" && w.ObjectId == parameter.ProjectId).Select(w => new NoteEntityModel
                {
                    NoteId = w.NoteId,
                    Description = w.Description,
                    Type = w.Type,
                    ObjectId = w.ObjectId,
                    ObjectType = w.ObjectType,
                    NoteTitle = w.NoteTitle,
                    CreatedById = w.CreatedById,
                    CreatedDate = w.CreatedDate,
                    UpdatedById = w.UpdatedById,
                    UpdatedDate = w.UpdatedDate,
                    NoteDocList = new List<NoteDocumentEntityModel>()
                }).ToList();

                //lấy tên người tạo, người chỉnh sửa cho note
                listNote.ForEach(note =>
                {
                    var empId = context.User.FirstOrDefault(f => f.UserId == note.CreatedById).EmployeeId;
                    var contact = context.Contact.FirstOrDefault(f => f.ObjectType == "EMP" && f.ObjectId == empId);
                    if (contact != null)
                    {
                        note.ResponsibleName = contact.FirstName + " " + contact.LastName;
                    }
                });

                // Sắp xếp lại listnote
                listNote = listNote.OrderByDescending(x => x.CreatedDate).ToList();

                totalRecordsNote = listNote.Count;

                listNote = listNote
                    .Skip(pageSize * (pageIndex - 1))
                    .Take(pageSize).ToList();
                #endregion

                #region list dự án theo phân quyền user

                var listAllProject = context.Project.ToList();

                var user = context.User.FirstOrDefault(c => c.UserId == parameter.UserId);

                if (user != null)
                {
                    var employee = context.Employee.FirstOrDefault(c => c.EmployeeId == user.EmployeeId);

                    if (employee != null)
                    {
                        var positionEmp = context.Position.FirstOrDefault(c => c.PositionId == employee.PositionId);
                        if (positionEmp != null && positionEmp.PositionCode == "GD")
                        {
                            var isRoot = context.Organization.FirstOrDefault(c => c.OrganizationId == employee.OrganizationId).ParentId == null;
                            if (!isRoot)
                            {
                                // Giám đốc được set đơn vị cao nhất trong tổ chức - Get All
                                // Lấy những bản ghi là quản lý, quản lý cấp cao, subPM - trong nguồn lực
                                // Những dự án có trong nguồn lực
                                var listProjectFollowResourceId = context.ProjectResource.Where(c => c.ObjectId == employee.EmployeeId).Select(m => m.ProjectId).ToList();
                                // Những dự án là quản lý, quản lý cấp cao, đồng quản lý
                                var listProjectFollowManagerId = context.ProjectEmployeeMapping.Where(c => c.EmployeeId == employee.EmployeeId).Select(c => c.ProjectId).ToList();

                                var listId = new List<Guid>();
                                listId.AddRange(listProjectFollowResourceId);
                                listId.AddRange(listProjectFollowManagerId);

                                listAllProject = listAllProject.Where(c => listId.Contains(c.ProjectId) || c.ProjectManagerId == employee.EmployeeId || c.CreateBy == user.UserId).ToList();
                            }
                        }
                        else
                        {
                            // Những dự án có trong nguồn lực
                            var listProjectFollowResourceId = context.ProjectResource.Where(c => c.ObjectId == employee.EmployeeId).Select(m => m.ProjectId).ToList();
                            // Những dự án là quản lý, quản lý cấp cao, đồng quản lý
                            var listProjectFollowManagerId = context.ProjectEmployeeMapping.Where(c => c.EmployeeId == employee.EmployeeId).Select(c => c.ProjectId).ToList();

                            var listId = new List<Guid>();
                            listId.AddRange(listProjectFollowResourceId);
                            listId.AddRange(listProjectFollowManagerId);

                            listAllProject = listAllProject.Where(c => listId.Contains(c.ProjectId) || c.ProjectManagerId == employee.EmployeeId || c.CreateBy == user.UserId).ToList();
                        }
                    }
                }

                var listProject = listAllProject
                        .Select(m => new ProjectEntityModel
                        {
                            ProjectId = m.ProjectId,
                            ProjectCode = m.ProjectCode,
                            ProjectName = m.ProjectName,
                            ProjectStatusPlan = m.ProjectStatusPlan,
                        }).ToList();

                #endregion

                return new GetProjectScopeResult
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "",
                    Project = project,
                    ListProjectScope = listProjectScope,
                    ListProjectTask = listProjectTask,
                    ListVendor = listVendor,
                    ListResource = listResource,
                    ListStatus = listStatus,
                    ListNote = listNote,
                    TotalRecordsNote = totalRecordsNote,
                    listProject = listProject,
                };
            }
            catch(Exception e)
            {
                return new GetProjectScopeResult
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    Message = e.Message,
                   
                };
            }
            
        }

        public SearchNoteResult PagingProjectNote(SearchNoteParameter parameter)
        {
            try
            {
                // common
                var listAllNote = context.Note.ToList();
                int pageSize = parameter.PageSize.Value;
                int pageIndex = parameter.PageIndex.Value;
                int totalRecordsNote = 0;
                var listNote = new List<NoteEntityModel>();

                listNote = context.Note.Where(w => w.Active == true && w.ObjectType == parameter.ScreenName && w.ObjectId == parameter.ProjectId).Select(w => new NoteEntityModel
                {
                    NoteId = w.NoteId,
                    Description = w.Description,
                    Type = w.Type,
                    ObjectId = w.ObjectId,
                    ObjectType = w.ObjectType,
                    NoteTitle = w.NoteTitle,
                    CreatedById = w.CreatedById,
                    CreatedDate = w.CreatedDate,
                    UpdatedById = w.UpdatedById,
                    UpdatedDate = w.UpdatedDate,
                    NoteDocList = new List<NoteDocumentEntityModel>()
                }).ToList();

                //lấy tên người tạo, người chỉnh sửa cho note
                listNote.ForEach(note =>
                {
                    var empId = context.User.FirstOrDefault(f => f.UserId == note.CreatedById).EmployeeId;
                    var contact = context.Contact.FirstOrDefault(f => f.ObjectType == "EMP" && f.ObjectId == empId);
                    if (contact != null)
                    {
                        note.ResponsibleName = contact.FirstName + " " + contact.LastName;
                    }
                });

                // Sắp xếp lại listnote
                listNote = listNote.OrderByDescending(x => x.CreatedDate).ToList();
                totalRecordsNote = listNote.Count;

                listNote = listNote
                    .Skip(pageSize * pageIndex)
                    .Take(pageSize).ToList();

                return new SearchNoteResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success",
                    TotalRecordsNote = totalRecordsNote,
                    NoteList = listNote,
                    NoteEntityList=listNote
                };
            }
            catch (Exception e)
            {
                return new SearchNoteResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }


        private List<ProjectScopeModel> SetTTChildren(List<ProjectScopeModel> listQuoteScope)
        {
            listQuoteScope.Where(l => l.ParentId == null).ToList().ForEach(item =>
            {
                item.ProjectScopeCode = "";
                var list = listQuoteScope.Where(l => l.ParentId == item.ProjectScopeId).OrderBy(o => o.ProjectScopeCode);
                int index = 1;
                foreach (var item1 in list)
                {
                    item1.ProjectScopeCode = index.ToString();
                    lstScopeNew.Add(item1);
                    var list2 = listQuoteScope.Where(l => l.ParentId == item1.ProjectScopeId).OrderBy(o => o.ProjectScopeCode);
                    int index2 = 1;
                    SetTTChildrenLoop(list2.ToList(), listQuoteScope, index2, item1.ProjectScopeCode, item1.Level.Value);
                    index++;
                }
                lstScopeNew.Add(item);
            });

            return lstScopeNew.OrderBy(o => o.ProjectScopeCode).ToList();
        }


        private void SetTTChildrenLoop(List<ProjectScopeModel> lstChildrenScope, List<ProjectScopeModel> listQuoteScopeDefault, int index, string projectScopeCode, int level)
        {
            foreach (var item1 in lstChildrenScope)
            {
                if (level != item1.Level)
                {
                    index = 1;
                    level = item1.Level.Value;
                }
                item1.ProjectScopeCode = projectScopeCode + "." + index;
                listQuoteScopeDefault.Find(x => x.ProjectScopeId == item1.ProjectScopeId).ProjectScopeCode = item1.ProjectScopeCode;
                lstScopeNew.Add(item1);
                var list2 = listQuoteScopeDefault.Where(l => l.ParentId == item1.ProjectScopeId).OrderBy(o => o.ProjectScopeCode);
                SetTTChildrenLoop(list2.ToList(), listQuoteScopeDefault, index, item1.ProjectScopeCode, item1.Level.Value);
                index++;
            }
        }

        public UpdateProjectScopeResult UpdateProjectScope(UpdateProjectScopeParameter parameter)
        {
            try
            {
                var guidId = Guid.NewGuid();
                ProjectScopeMapping map = new ProjectScopeMapping();
                var projectScope = context.ProjectScope.FirstOrDefault(s => s.ProjectScopeId == parameter.ProjectScope.ProjectScopeId);
                var scopeDesOld = projectScope == null || projectScope.ProjectScopeName == null ? null : projectScope.ProjectScopeName;
                // Cập nhật hạng mục
                if (projectScope != null)
                {
                    projectScope.ProjectScopeName = parameter.ProjectScope.ProjectScopeName;
                    projectScope.Description = parameter.ProjectScope.Description;
                    projectScope.UpdateBy = parameter.UserId;
                    projectScope.ResourceType = parameter.ProjectScope.ResourceType;
                    projectScope.UpdateDate = DateTime.Now;
                    context.ProjectScope.Update(projectScope);
                    #region Thông báo

                    NotificationHelper.AccessNotification(context, TypeModel.ProjectScope, "UPD", new ProjectScope(),
                        projectScope, true);

                    #endregion
                    if (scopeDesOld == parameter.ProjectScope.ProjectScopeName && projectScope != null)
                        scopeDesOld = string.Empty;
                    CreateProjectScopeNoteStatus(parameter.ProjectScope.ProjectId, parameter.UserId, parameter.ProjectScope.ProjectScopeName, "EDT", scopeDesOld);
                    //if (parameter.ProjectScope.IsSendMail == true)
                    //{
                    //    // Send Mail
                    //}
                }
                else
                {
                    ProjectScope obj = new ProjectScope();
                    obj.ProjectScopeId = guidId;
                    obj.ResourceType = parameter.ProjectScope.ResourceType;
                    obj.ProjectScopeCode = parameter.ProjectScope.ProjectScopeCode;
                    obj.ProjectScopeName = parameter.ProjectScope.ProjectScopeName;
                    obj.Description = parameter.ProjectScope.Description;
                    obj.ParentId = parameter.ProjectScope.ParentId;
                    obj.ProjectId = (Guid)parameter.ProjectScope.ProjectId;
                    obj.CreateBy = parameter.UserId;
                    obj.Level = parameter.ProjectScope.Level;
                    obj.CreateDate = DateTime.Now;

                    context.ProjectScope.Add(obj);

                    #region Thông báo

                    NotificationHelper.AccessNotification(context, TypeModel.ProjectScope, "CRE",
                        new ProjectScope(), obj, true);

                    #endregion

                    if (parameter.ProjectScope.ListTask.Count() > 0)
                    {
                        parameter.ProjectScope.ListTask.ForEach(taskId =>
                            {
                                var task = context.Task.FirstOrDefault(x => x.TaskId == new Guid(taskId));
                                if (task != null)
                                {
                                    task.ProjectScopeId = guidId;
                                }
                            });
                    }
                    CreateProjectScopeNoteStatus(parameter.ProjectScope.ProjectId, parameter.UserId, parameter.ProjectScope.ProjectScopeName, "ADD");
                    //if (oldTask.IsSendMail == true)
                    //{
                    //    // Send Mail
                    //}
                }
                // Update ngày thay đổi vào trường LastChangeActivityDate bảng Project
                var project = context.Project.FirstOrDefault(x => x.ProjectId == parameter.ProjectScope.ProjectId);
                if (project != null)
                {
                    project.LastChangeActivityDate = DateTime.Now;
                    context.Project.Update(project);
                }

                context.SaveChanges();

                var projectScopes = context.ProjectScope.Where(x => x.ProjectId == parameter.ProjectScope.ProjectId).ToList();
                var projectScopeGuids = projectScopes.Select(ps => ps.ProjectScopeId).ToList();
                var tasks = context.Task.Where(x => projectScopeGuids.Contains((Guid)x.ProjectScopeId)).ToList();
                var listProjectScope = context.ProjectScope.Where(x => x.ProjectId == parameter.ProjectScope.ProjectId).Select(p => new ProjectScopeModel()
                {
                    ProjectScopeId = p.ProjectScopeId,
                    Description = p.Description,
                    ResourceType = p.ResourceType,
                    ProjectScopeName = p.ProjectScopeName,
                    ProjectScopeCode = p.ProjectScopeCode,
                    TenantId = p.TenantId,
                    ParentId = p.ParentId,
                    Level = p.Level
                }).ToList();
                listProjectScope.ForEach(item =>
                {
                    // danh sách các task thuộc hạng mục
                    var listTask = tasks.Where(x => x.ProjectScopeId == item.ProjectScopeId).Select(x => x.TaskId).ToList();
                    var listTaskRe = context.TaskResourceMapping.Where(x => x.IsPersonInCharge == true && listTask.Contains(x.TaskId)).Select(a => a.ResourceId).ToList();
                    var listProjectRe = context.ProjectResource.Where(x => listTaskRe.Contains(x.ProjectResourceId)).Select(a => a.ObjectId).ToList();
                    var lstEmp = context.Employee.Where(e => listProjectRe.Contains(e.EmployeeId)).Select(em => em.EmployeeCode + '-' + em.EmployeeName).ToList();
                    var listResourceScope = new List<string>();
                    lstEmp.ForEach(emp =>
                    {
                        listResourceScope.Add(lstEmp.Count > 0 ? emp : string.Empty);
                    });
                    item.ListEmployee = listResourceScope;
                });
                listProjectScope = SetTTChildren(listProjectScope);

                #region get list notes

                var lstTaskId = context.Task.Where(x => x.ProjectId == parameter.ProjectScope.ProjectId).Select(a => a.TaskId).ToList();
                var listNote = new List<NoteEntityModel>();
                listNote = context.Note.Where(w => w.Active == true && (w.ObjectType == "PROSCOPE" && w.ObjectId == parameter.ProjectScope.ProjectId) || lstTaskId.Contains(w.ObjectId)).Select(w => new NoteEntityModel
                {
                    NoteId = w.NoteId,
                    Description = w.Description,
                    Type = w.Type,
                    ObjectId = w.ObjectId,
                    ObjectType = w.ObjectType,
                    NoteTitle = w.NoteTitle,
                    CreatedById = w.CreatedById,
                    CreatedDate = w.CreatedDate,
                    UpdatedById = w.UpdatedById,
                    UpdatedDate = w.UpdatedDate,
                    NoteDocList = new List<NoteDocumentEntityModel>()
                }).ToList();

                //lấy tên người tạo, người chỉnh sửa cho note
                listNote.ForEach(note =>
                {
                    var empId = context.User.FirstOrDefault(f => f.UserId == note.CreatedById).EmployeeId;
                    var contact = context.Contact.FirstOrDefault(f => f.ObjectType == "EMP" && f.ObjectId == empId);
                    if (contact != null)
                    {
                        note.ResponsibleName = contact.FirstName + " " + contact.LastName;
                    }
                });

                // Sắp xếp lại listnote
                listNote = listNote.OrderByDescending(x => x.CreatedDate).ToList();

                #endregion
                return new UpdateProjectScopeResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Cập nhật hạng mục thành công.",
                    ListProjectScope = listProjectScope,
                    ListNote = listNote
                };
            }
            catch (Exception ex)
            {
                return new UpdateProjectScopeResult
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = ex.ToString()
                };
            }
        }


        public GetProjectResourceResult GetProjectResource(GetProjectResourceParameter parameter)
        {
            try
            {
                var listNote = new List<NoteEntityModel>();
                int totalRecordsNote = 0;
                int pageSize = 10;
                int pageIndex = 1;

                #region Get Status
                //  "PSC", "CVT",  
                var listCategoryTypeCodes = new List<string> { "DAT", "LDA", "TTCV", "NCNL", "VTN", "LNL", "NNT" };
                var listCategory = context.Category.Where(x => listCategoryTypeCodes.Contains(x.CategoryType.CategoryTypeCode) && x.Active == true).Select(y =>
                                   new CategoryEntityModel
                                   {
                                       CategoryId = y.CategoryId,
                                       CategoryName = y.CategoryName,
                                       CategoryCode = y.CategoryCode,
                                       CategoryTypeId = Guid.Empty,
                                       CreatedById = Guid.Empty,
                                       CategoryTypeCode = y.CategoryType.CategoryTypeCode,
                                       CountCategoryById = 0
                                   }).ToList();
                #endregion

                #region Thông tin dự án
                var project = context.Project.Where(x => x.ProjectId == parameter.ProjectId).Select(y => new ProjectEntityModel
                {
                    ProjectId = y.ProjectId,
                    ProjectStartDate = y.ProjectStartDate,
                    ProjectEndDate = y.ProjectEndDate,
                    ActualStartDate = y.ActualStartDate,
                    ActualEndDate = y.ActualEndDate,
                    ProjectManagerId = y.ProjectManagerId,
                    ContractId = y.ContractId,
                    ProjectName = y.ProjectName,
                    ProjectCode = y.ProjectCode,
                    BudgetVnd = y.BudgetVnd,
                    BudgetUsd = y.BudgetUsd,
                    BudgetNgayCong = y.BudgetNgayCong,
                    // Butget = y.ButgetType == 1 ? y.Butget : 0,
                    // ButgetType = y.ButgetType,
                    CustomerId = y.CustomerId,
                    Description = y.Description,
                    ProjectSize = y.ProjectSize,
                    ProjectType = y.ProjectType,
                    ProjectStatus = y.ProjectStatus,
                    IncludeWeekend = y.IncludeWeekend,
                    Priority = y.Priority,
                }).FirstOrDefault();

                project.ProjectTypeName = listCategory.FirstOrDefault(c => c.CategoryId == project.ProjectType)?.CategoryName;
                project.ProjectStatusName = listCategory.FirstOrDefault(c => c.CategoryId == project.ProjectStatus)?.CategoryName;
                project.ProjectStatusCode = listCategory.FirstOrDefault(c => c.CategoryId == project.ProjectStatus)?.CategoryCode;
                if (project.Priority == 1) project.PriorityName = "Thấp";
                else if (project.Priority == 2) project.PriorityName = "Trung bình";
                else project.PriorityName = "Cao";
                TaskDAO _taskDao = new TaskDAO(context, iHostingEnvironment);
                _taskDao.CaculatorProjectTask(parameter.ProjectId, out decimal projectComplete, out decimal totalEstimateHour);

                project.TaskComplate = projectComplete;
                project.EstimateCompleteTime = totalEstimateHour;

                if (project == null)
                {
                    return new GetProjectResourceResult()
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = "Dự án không tồn tại trên hệ thống"
                    };
                }
                #endregion

                #region Danh sách nguồn lực

                var listProjectResource = context.ProjectResource.Where(x => x.ProjectId == parameter.ProjectId).Select(p => new ProjectResourceEntityModel()
                {
                    ProjectResourceId = p.ProjectResourceId,
                    TenantId = p.TenantId,
                    ResourceType = p.ResourceType,
                    IsCreateVendor = p.IsCreateVendor,
                    ResourceRole = p.ResourceRole,
                    ObjectId = p.ObjectId,
                    Allowcation = p.Allowcation,
                    StartTime = p.StartTime,
                    EndTime = p.EndTime,
                    CreateDate = p.CreateDate,
                    EmployeeRole = p.EmployeeRole,
                    IsOverload = p.IsOverload,
                    IncludeWeekend = p.IncludeWeekend
                }).OrderBy(x => x.CreateDate).ToList();

                var listResourceIds = listProjectResource.Select(r => r.ObjectId).ToList();
                var employees = context.Employee.Where(e => listResourceIds.Contains(e.EmployeeId)).Select(a => new EmployeeEntityModel
                {
                    EmployeeId = a.EmployeeId,
                    EmployeeName = a.EmployeeName
                }).ToList();

                var vendors = context.Vendor.Where(e => listResourceIds.Contains(e.VendorId)).Select(a => new VendorEntityModel
                {
                    VendorId = a.VendorId,
                    VendorName = a.VendorName
                }).ToList();

                int index = 1;
                int indexEx = 1;

                listProjectResource.ForEach(resource =>
                {
                    var obj = new CheckAllowcateProjectResourceParameter();
                    obj.ResourceId = resource.ObjectId == null ? Guid.Empty : (Guid)resource.ObjectId;
                    obj.FromDate = resource.StartTime;
                    obj.ToDate = resource.EndTime;
                    obj.Allowcation = resource.Allowcation == null ? 0 : (int)resource.Allowcation;
                    obj.ProjectResourceId = resource.ProjectResourceId;

                    // Check xem khoảng thời gian của nguồn lực tổng thời gian phân bổ là bao nhiêu
                    int totalAllowcation = CheckAllowcateProjectResource(obj).TotalAllowcation;

                    switch (totalAllowcation < 85 ? "Low" :
                          (85 <= totalAllowcation && totalAllowcation <= 100 ? "Mid" : "Hight"))
                    {
                        case "Low":
                            resource.BackgroundColorForStatus = "#fdff00";
                            break;
                        case "Mid":
                            resource.BackgroundColorForStatus = "#9aee81";
                            break;
                        case "Hight":
                            resource.BackgroundColorForStatus = "#ff5959";
                            break;
                    }

                    // Lây thông tin ngày nghỉ cố định

                    // Lấy thông tin ngày nghỉ được phê duyệt
                    var tempType = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "LXU");
                    var absentPermissionId = context.Category.FirstOrDefault(ct => ct.CategoryCode.Trim() == "NP" &&
                                                                                   ct.CategoryTypeId ==
                                                                                   tempType.CategoryTypeId)?.CategoryId;
                    var absentWithoutPermissionId =
                        context.Category.FirstOrDefault(ct => ct.CategoryCode.Trim() == "NKL")?.CategoryId;
                    var _empRequest = (from empR in context.EmployeeRequest
                        join stt in context.Category on empR.StatusId equals stt.CategoryId
                        where empR.OfferEmployeeId == resource.ObjectId &&
                              empR.RequestDate.Value.Year == DateTime.Now.Year && stt.CategoryCode.Trim() == "Approved"
                        select empR).OrderByDescending(o => o.RequestDate).ToList();

                    double amountAbsentWithPermission = 0;
                    double amountAbsentWithoutPermission = 0;

                    // Danh sách ngày nghỉ
                    var lstDayOfRequest = new List<DateTime>();
                    _empRequest.ForEach(request =>
                    {
                        for (var date = request.StartDate;
                            (date <= request.EnDate && date >= resource.StartTime && date <= resource.EndTime);
                            date = date.Value.AddDays(1))
                        {
                            lstDayOfRequest.Add((DateTime) date);
                        }
                    });

                    _empRequest.ForEach(empR =>
                    {
                        for (var date = empR.StartDate;
                            (date <= empR.EnDate && date >= resource.StartTime && date <= resource.EndTime);
                            date = date.Value.AddDays(1))
                        {
                            if (empR.TypeRequest == absentPermissionId)
                            {
                                if (empR.StartTypeTime == empR.EndTypeTime)
                                {
                                    amountAbsentWithPermission = amountAbsentWithPermission + 0.5;
                                }
                                else
                                {
                                    amountAbsentWithPermission = amountAbsentWithPermission + 1;
                                }
                            }

                            if (empR.TypeRequest == absentWithoutPermissionId)
                            {
                                if (empR.StartTypeTime == empR.EndTypeTime)
                                {
                                    amountAbsentWithoutPermission = amountAbsentWithoutPermission + 0.5;
                                }
                                else
                                {
                                    amountAbsentWithoutPermission = amountAbsentWithoutPermission + 1;
                                }
                            }
                        }
                    });

                    // Nội bộ hay thuê ngoài
                    resource.ResourceRoleName = listCategory.FirstOrDefault(r => r.CategoryId == resource.ResourceRole)?.CategoryCode;

                    //Nội bộ
                    if (listCategory.FirstOrDefault(r => r.CategoryId == resource.ResourceRole)?.CategoryCode == "NB")
                    {
                        // Loại nguồn lực
                        resource.ResourceTypeName = listCategory.FirstOrDefault(r => r.CategoryId == resource.ResourceType)?.CategoryName;
                        //Tên nhân viên 
                        resource.NameResource = employees.FirstOrDefault(r => r.EmployeeId == resource.ObjectId)?.EmployeeName;
                        // Vai trò
                        resource.EmployeeRoleName = listCategory.FirstOrDefault(r => r.CategoryId == resource.EmployeeRole)?.CategoryName;
                        // Ngày công
                        if (resource.EndTime != null && resource.StartTime != null)
                        {
                            TimeSpan ts = (DateTime)resource.EndTime - (DateTime)resource.StartTime;
                            var numberWeeken = TotalHoliday(resource.StartTime.Value, resource.EndTime.Value,
                                resource.IncludeWeekend ?? false);
                            resource.WorkDay =
                                ((ts.TotalDays + 1 - amountAbsentWithPermission - amountAbsentWithoutPermission -
                                  numberWeeken) * (resource.Allowcation == null ? 0 : resource.Allowcation)) / 100;
                        }
                        resource.Stt = index;
                        index = index + 1;
                    }
                    //Thuê ngoài
                    else
                    {
                        // Nguon luc thue ngoai
                        #region Danh sách người liên hệ nhà thầu

                        var listVendorId = context.Vendor.Where(x => x.VendorId == resource.ObjectId).Select(a => a.VendorId);

                        var listProjectVendor = context.ProjectVendor
                            .Where(x => x.VendorId == resource.ObjectId && x.ProjectId == parameter.ProjectId).Select(
                                a => new ProjectVendorEntityModel
                                {
                                    ProjectId = a.ProjectId,
                                    ContactId = a.ContactId,
                                    PaymentMethodId = a.PaymentMethodId.Value,
                                    ProjectVendorId = a.ProjectVendorId,
                                    ProjectResourceId = a.ProjectResourceId,
                                    VendorId = a.VendorId
                                }).ToList();

                        var listContact = context.Contact.Where(x => (x.ObjectType == ObjectType.VENDORCONTACT
                                                                      || x.ObjectType == ObjectType.VENDOR
                                                                     ) && listVendorId.Contains(x.ObjectId)).Select(a =>
                            new ContactEntityModel
                            {
                                ContactId = a.ContactId,
                                ObjectId = a.ObjectId,
                                ObjectType = a.ObjectType,
                                FirstName = a.FirstName,
                                LastName = a.LastName,
                                ProvinceId = a.ProvinceId,
                                DistrictId = a.DistrictId,
                                WardId = a.WardId,
                                Gender = a.Gender,
                                Phone = a.Phone,
                                Email = a.Email,
                                Address = a.Address,
                            }).ToList();

                        var listProvinceEntity = context.Province.ToList();
                        var listDistrictEntity = context.District.ToList();
                        var listWardEntity = context.Ward.ToList();
                        listContact.ForEach(contact =>
                        {
                            var listAddress = new List<string>();
                            if (!string.IsNullOrWhiteSpace(contact.Address))
                            {
                                listAddress.Add(contact.Address);
                            }
                            if (contact.WardId != null)
                            {
                                var _ward = listWardEntity.FirstOrDefault(f => f.WardId == contact.WardId);
                                var _wardText = _ward.WardType + " " + _ward.WardName;
                                listAddress.Add(_wardText);
                            }
                            if (contact.DistrictId != null)
                            {
                                var _district =
                                    listDistrictEntity.FirstOrDefault(f => f.DistrictId == contact.DistrictId);
                                var _districtText = _district.DistrictType + " " + _district.DistrictName;
                                listAddress.Add(_districtText);
                            }
                            if (contact.ProvinceId != null)
                            {
                                var _province =
                                    listProvinceEntity.FirstOrDefault(f => f.ProvinceId == contact.ProvinceId);
                                var _provincetext = _province.ProvinceType + " " + _province.ProvinceName;
                                listAddress.Add(_provincetext);
                            }
                            contact.Address = String.Join(", ", listAddress);
                        });
                        if (listContact.Count == 0)
                            resource.ListContact = new List<ContactEntityModel>();
                        else
                            resource.ListContact = listContact;

                        var vendor = context.Vendor.Where(x => x.VendorId == resource.ObjectId).Select(a => new VendorEntityModel
                        {
                            VendorId = a.VendorId,
                            VendorCode = a.VendorCode,
                            VendorName = a.VendorName,
                            VendorGroupId = a.VendorGroupId,
                            PaymentId = a.PaymentId,
                        }).FirstOrDefault();

                        resource.Vendor = vendor;
                        resource.ListProjectVendor = listProjectVendor;

                        #endregion

                        // Tên nhóm nhà thầu
                        resource.ResourceTypeName = listCategory.FirstOrDefault(r => r.CategoryId == resource.ResourceType)?.CategoryName;
                        //Tên nhà thầu
                        resource.NameResource = vendors.FirstOrDefault(r => r.VendorId == resource.ObjectId)?.VendorName;
                        // Ngày công
                        if (resource.EndTime != null && resource.StartTime != null)
                        {
                            TimeSpan ts = (DateTime)resource.EndTime - (DateTime)resource.StartTime;
                            var numberWeeken = TotalHoliday(resource.StartTime.Value, resource.EndTime.Value, resource.IncludeWeekend ?? false);
                            resource.WorkDay = ((ts.TotalDays + 1 - amountAbsentWithPermission - amountAbsentWithoutPermission - numberWeeken) * (resource.Allowcation == null ? 0 : resource.Allowcation)) / 100;
                        }
                        resource.Stt = indexEx;
                        indexEx = indexEx + 1;
                    }
                });

                #endregion

                #region List phương thức thanh toán
                var paymentMethodCategoryTypeId = context.CategoryType
                    .FirstOrDefault(x => x.CategoryTypeCode == "PTO" && x.Active == true)?.CategoryTypeId;
                var listPaymentMethod = context.Category
                    .Where(x => x.CategoryTypeId == paymentMethodCategoryTypeId && x.Active == true).Select(y =>
                        new CategoryEntityModel
                        {
                            CategoryId = y.CategoryId,
                            CategoryName = y.CategoryName,
                            CategoryCode = y.CategoryCode,
                            IsDefault = y.IsDefauld,
                        }).ToList();

                #endregion

                #region Dánh sách task thuộc dự án
                var tasks = context.Task.Where(x => x.ProjectId == project.ProjectId).ToList();
                var listProjectTask = tasks.Select(p => new TaskEntityModel()
                {
                    ProjectScopeId = p.ProjectScopeId,
                    TaskId = p.TaskId,
                    ActualHour = p.ActualHour,
                    EstimateCost = p.EstimateCost,
                    EstimateHour = p.EstimateHour,
                    TaskComplate = p.TaskComplate,
                }).ToList();

                #endregion

                #region get list notes
                if (project != null)
                {
                    listNote = context.Note.Where(w => w.Active == true && w.ObjectType == "PRORESOURCE" && w.ObjectId == project.ProjectId).Select(w => new NoteEntityModel
                    {
                        NoteId = w.NoteId,
                        Description = w.Description,
                        Type = w.Type,
                        ObjectId = w.ObjectId,
                        ObjectType = w.ObjectType,
                        NoteTitle = w.NoteTitle,
                        CreatedById = w.CreatedById,
                        CreatedDate = w.CreatedDate,
                        UpdatedById = w.UpdatedById,
                        UpdatedDate = w.UpdatedDate,
                        NoteDocList = new List<NoteDocumentEntityModel>()
                        //context.NoteDocument.Where(ws => ws.NoteId == w.NoteId && ws.Active == true).Select(s => new NoteDocumentEntityModel
                        //{
                        //    NoteDocumentId = s.NoteDocumentId,
                        //    NoteId = s.NoteId,
                        //    DocumentName = s.DocumentName,
                        //    DocumentSize = s.DocumentSize,
                        //    DocumentUrl = s.DocumentUrl,
                        //}).ToList() ?? new List<NoteDocumentEntityModel>()
                    }).ToList();

                    //lấy tên người tạo, người chỉnh sửa cho note
                    listNote.ForEach(note =>
                    {
                        var empId = context.User.FirstOrDefault(f => f.UserId == note.CreatedById).EmployeeId;
                        var contact = context.Contact.FirstOrDefault(f => f.ObjectType == "EMP" && f.ObjectId == empId);
                        if (contact != null)
                        {
                            note.ResponsibleName = contact.FirstName + " " + contact.LastName;
                        }
                    });
                    // Sắp xếp lại listnote
                    listNote = listNote.OrderByDescending(x => x.CreatedDate).ToList();

                    totalRecordsNote = listNote.Count;

                    listNote = listNote
                        .Skip(pageSize * (pageIndex - 1))
                        .Take(pageSize).ToList();
                }
                #endregion

                #region list dự án theo phân quyền user

                var listAllProject = context.Project.ToList();

                var user = context.User.FirstOrDefault(c => c.UserId == parameter.UserId);

                if (user != null)
                {
                    var employee = context.Employee.FirstOrDefault(c => c.EmployeeId == user.EmployeeId);

                    if (employee != null)
                    {
                        var positionEmp = context.Position.FirstOrDefault(c => c.PositionId == employee.PositionId);
                        if (positionEmp != null && positionEmp.PositionCode == "GD")
                        {
                            var isRoot = context.Organization.FirstOrDefault(c => c.OrganizationId == employee.OrganizationId).ParentId == null;
                            if (!isRoot)
                            {
                                // Giám đốc được set đơn vị cao nhất trong tổ chức - Get All
                                // Lấy những bản ghi là quản lý, quản lý cấp cao, subPM - trong nguồn lực
                                // Những dự án có trong nguồn lực
                                var listProjectFollowResourceId = context.ProjectResource.Where(c => c.ObjectId == employee.EmployeeId).Select(m => m.ProjectId).ToList();
                                // Những dự án là quản lý, quản lý cấp cao, đồng quản lý
                                var listProjectFollowManagerId = context.ProjectEmployeeMapping.Where(c => c.EmployeeId == employee.EmployeeId).Select(c => c.ProjectId).ToList();

                                var listId = new List<Guid>();
                                listId.AddRange(listProjectFollowResourceId);
                                listId.AddRange(listProjectFollowManagerId);

                                listAllProject = listAllProject.Where(c => listId.Contains(c.ProjectId) || c.ProjectManagerId == employee.EmployeeId || c.CreateBy == user.UserId).ToList();
                            }
                        }
                        else
                        {
                            // Những dự án có trong nguồn lực
                            var listProjectFollowResourceId = context.ProjectResource.Where(c => c.ObjectId == employee.EmployeeId).Select(m => m.ProjectId).ToList();
                            // Những dự án là quản lý, quản lý cấp cao, đồng quản lý
                            var listProjectFollowManagerId = context.ProjectEmployeeMapping.Where(c => c.EmployeeId == employee.EmployeeId).Select(c => c.ProjectId).ToList();

                            var listId = new List<Guid>();
                            listId.AddRange(listProjectFollowResourceId);
                            listId.AddRange(listProjectFollowManagerId);

                            listAllProject = listAllProject.Where(c => listId.Contains(c.ProjectId) || c.ProjectManagerId == employee.EmployeeId || c.CreateBy == user.UserId).ToList();
                        }
                    }

                }

                var listProject = listAllProject
                        .Select(m => new ProjectEntityModel
                        {
                            ProjectId = m.ProjectId,
                            ProjectCode = m.ProjectCode,
                            ProjectName = m.ProjectName
                        }).ToList();

                #endregion

                return new GetProjectResourceResult
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "",
                    Project = project,
                    ListProjectTask = listProjectTask,
                    ListProjectResource = listProjectResource,
                    ListPaymentMethod = listPaymentMethod,
                    ListNote = listNote,
                    TotalRecordsNote = totalRecordsNote,
                    listProject = listProject,
                };
            }
            catch(Exception e)
            {
                return new GetProjectResourceResult
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
            
        }

        public GetStatusResourceProjectResult GetStatusResourceProject(GetStatusResourceProjectParameter parameter)
        {
            try
            {
                #region Chi tiết nguồn lực
                var projectResource = new ProjectResourceEntityModel();
                if (parameter.ProjectResourceId != null)
                {
                    projectResource = context.ProjectResource.Where(x => x.ProjectResourceId == parameter.ProjectResourceId).Select(x => new ProjectResourceEntityModel
                    {
                        ProjectResourceId = x.ProjectResourceId,
                        ProjectId = x.ProjectId,
                        ObjectId = x.ObjectId,
                        ResourceType = x.ResourceType,
                        ResourceRole = x.ResourceRole,
                        StartTime = x.StartTime,
                        EndTime = x.EndTime,
                        IsCreateVendor = x.IsCreateVendor,
                        EmployeeRole = x.EmployeeRole,
                        Allowcation = x.Allowcation,
                        IncludeWeekend = x.IncludeWeekend ?? false,
                        ChiPhiTheoGio = x.ChiPhiTheoGio,
                        CreateBy = x.CreateBy,
                        CreateDate = x.CreateDate,
                        UpdateBy = x.UpdateBy,
                        UpdateDate = x.UpdateDate
                    }).FirstOrDefault();
                }
                #endregion

                #region Get Status
                var listCategoryTypeCodes = new List<string> { "LNL", "VTN", "NNT", "NCNL" };
                var listCategory = context.Category.Where(x => listCategoryTypeCodes.Contains(x.CategoryType.CategoryTypeCode) && x.Active == true).Select(y =>
                                   new CategoryEntityModel
                                   {
                                       CategoryId = y.CategoryId,
                                       CategoryName = y.CategoryName,
                                       CategoryCode = y.CategoryCode,
                                       CategoryTypeId = Guid.Empty,
                                       CreatedById = Guid.Empty,
                                       CategoryTypeCode = y.CategoryType.CategoryTypeCode,
                                       CountCategoryById = 0
                                   }).ToList();
                var listResourceType = listCategory?.Where(c => c.CategoryTypeCode == "LNL").ToList();
                var listResourceRole = listCategory?.Where(c => c.CategoryTypeCode == "VTN").ToList();
                var listVendorGroup = listCategory?.Where(c => c.CategoryTypeCode == "NNT").ToList();
                var listResourceSource = listCategory?.Where(c => c.CategoryTypeCode == "NCNL").ToList();
                #endregion

                #region Lấy danh sách nhân viên
                var lstEmp = context.Employee.ToList();
                var listEmployeeActive = lstEmp.Where(x => x.Active == true).Select(y =>
                           new EmployeeEntityModel
                           {
                               EmployeeId = y.EmployeeId,
                               EmployeeCode = y.EmployeeCode,
                               EmployeeName = y.EmployeeName,
                               EmployeeCodeName = y.EmployeeCode + " - " + y.EmployeeName,
                               OrganizationId = y.OrganizationId,
                               IsManager = y.IsManager,
                               Active = y.Active
                           }).ToList();
                // Nhân viên đã được thêm vào nguồn lực nội bộ - Không phân biệt nghỉ việc hay chưa            
                var employeeIdOfResource = context.ProjectResource.FirstOrDefault(x => x.ProjectResourceId == parameter.ProjectResourceId)?.ObjectId;
                var emp = lstEmp.Where(x => x.EmployeeId == employeeIdOfResource).Select(y =>
                            new EmployeeEntityModel
                            {
                                EmployeeId = y.EmployeeId,
                                EmployeeCode = y.EmployeeCode,
                                EmployeeName = y.EmployeeName,
                                EmployeeCodeName = y.EmployeeCode + " - " + y.EmployeeName,
                                OrganizationId = y.OrganizationId,
                                IsManager = y.IsManager,
                                Active = y.Active
                            }).ToList();

                // Danh sách distinct giữa list NV và NV được phân bổ vào nguồn lực
                var listEmployee = listEmployeeActive.Concat(emp).Distinct().ToList();

                #endregion

                #region Lấy danh sách máy móc 

                #endregion

                #region Lấy danh sách nhà cung cấp
                var listVendor = context.Vendor.Select(y =>
                           new VendorEntityModel
                           {
                               VendorCode = y.VendorCode,
                               VendorName = y.VendorName,
                               VendorId = y.VendorId,
                               Active = y.Active
                           }).ToList();
                var lstExtAccount = context.ExternalUser.ToList();

                listVendor.ForEach(vendor =>
                {
                    vendor.ExitsAccount = lstExtAccount.FirstOrDefault(x => x.ObjectId == vendor.VendorId) != null
                        ? true
                        : false;
                });
                #endregion

                return new GetStatusResourceProjectResult
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "",
                    ListResourceType = listResourceType,
                    ListResourceRole = listResourceRole,
                    ListVendorGroup = listVendorGroup,
                    ListEmployee = listEmployee,
                    ListVendor = listVendor,
                    ListResourceSource = listResourceSource,
                    ProjectResource = projectResource
                };
            }
            catch(Exception e)
            {
                return new GetStatusResourceProjectResult
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message                  
                };
            }       
        }

        public CreateOrUpdateProjectResourceResult CreateOrUpdateProjectResource(CreateOrUpdateProjectResourceParameter parameter)
        {
            try
            {
                var projectResource = parameter.ProjectResource;

                var oldResource = context.ProjectResource.FirstOrDefault(x => x.ProjectResourceId == projectResource.ProjectResourceId);
                var SendEmailEntityModel = new DataAccess.Models.Email.SendEmailEntityModel();
                var projectResourceId = Guid.Empty;
                var empName = context.Employee.FirstOrDefault(x => x.EmployeeId == projectResource.ObjectId) != null
                    ? context.Employee.FirstOrDefault(x => x.EmployeeId == projectResource.ObjectId).EmployeeName
                    : context.Vendor.FirstOrDefault(x => x.VendorId == projectResource.ObjectId).VendorName;
                Note note = new Note();
                var user = context.User.FirstOrDefault(x => x.UserId == parameter.UserId);
                var projectResourceIdRespone = Guid.Empty;

                using (var dbcxtransaction = context.Database.BeginTransaction())
                {
                    // Cập nhập nguồn lực
                    if (oldResource != null)
                    {
                        #region Get Status
                        var listCategoryTypeCodes = new List<string> { "LNL", "VTN", "NNT", "NCNL" };
                        var listCategory = context.Category.Where(x => listCategoryTypeCodes.Contains(x.CategoryType.CategoryTypeCode) && x.Active == true).Select(y =>
                                           new CategoryEntityModel
                                           {
                                               CategoryId = y.CategoryId,
                                               CategoryName = y.CategoryName,
                                               CategoryCode = y.CategoryCode,
                                               CategoryTypeId = Guid.Empty,
                                               CreatedById = Guid.Empty,
                                               CategoryTypeCode = y.CategoryType.CategoryTypeCode,
                                               CountCategoryById = 0
                                           }).ToList();
                        var listResourceType = listCategory?.Where(c => c.CategoryTypeCode == "LNL").ToList();
                        var listResourceRole = listCategory?.Where(c => c.CategoryTypeCode == "VTN").ToList();
                        var listVendorGroup = listCategory?.Where(c => c.CategoryTypeCode == "NNT").ToList();
                        var listResourceSource = listCategory?.Where(c => c.CategoryTypeCode == "NCNL").ToList();

                        #endregion

                        // Thêm note cho nguồn lực
                        note = new Note
                        {
                            NoteId = Guid.NewGuid(),
                            ObjectType = "PRORESOURCE",
                            ObjectId = parameter.ProjectResource.ProjectId,
                            Type = "SYS",
                            Active = true,
                            CreatedById = parameter.UserId,
                            CreatedDate = DateTime.Now,
                            NoteTitle = "Đã thêm ghi chú",
                            Description = string.Empty
                        };
                        string noteDescription = "";

                        var resourceType = listResourceSource.FirstOrDefault(x => oldResource.ResourceRole == x.CategoryId);
                        // Nguon luc nội bộ
                        if (resourceType.CategoryCode == "NB")
                        {
                            // Thay đổi vai trò
                            if (oldResource != null && projectResource.EmployeeRole != oldResource.EmployeeRole)
                            {
                                if (listResourceRole.FirstOrDefault(x => x.CategoryId == oldResource.EmployeeRole) == null)
                                {
                                    noteDescription += "<p>- <strong>" + user.UserName + "</strong>" + " đã thay đổi Vai trò của nguồn lực " + empName
                                   + " sang " + "<strong>" + listResourceRole.FirstOrDefault(x => x.CategoryId == parameter.ProjectResource.EmployeeRole).CategoryName + "</strong></p>";
                                }
                                else
                                    noteDescription += "<p>- <strong>" + user.UserName + "</strong>" + " đã thay đổi Vai trò của nguồn lực " + empName + " từ "
                                        + "<strong>" + listResourceRole.FirstOrDefault(x => x.CategoryId == oldResource.EmployeeRole).CategoryName + "</strong>"
                                        + " sang " + "<strong>" + listResourceRole.FirstOrDefault(x => x.CategoryId == parameter.ProjectResource.EmployeeRole).CategoryName + "</strong></p>";

                            }
                            // Ngày bắt đầu dự kiến
                            if (oldResource != null && projectResource.StartTime != oldResource.StartTime)
                            {
                                noteDescription += "<p>- <strong>" + user.UserName + "</strong>"
                                    + " đã thay đổi Ngày bắt đầu dự kiến của nguồn lực " + empName + " từ "
                                    + "<strong>" + $"{oldResource.StartTime:dd/MM/yyyy}" + "</strong>" + " sang "
                                    + "<strong>" + $"{projectResource.StartTime:dd/MM/yyyy}" + "</strong></p>";
                            }
                            //  Ngày kết thúc dự kiến
                            if (oldResource != null && projectResource.EndTime != oldResource.EndTime)
                            {
                                noteDescription += "<p>- <strong>" + user.UserName + "</strong>" + " đã thay đổi Ngày kết thúc dự kiến của nguồn lực " + empName + " từ "
                                      + "<strong>" + $"{oldResource.EndTime:dd/MM/yyyy}" + "</strong>" + " sang "
                                      + "<strong>" + $"{projectResource.EndTime:dd/MM/yyyy}" + "</strong></p>";
                            }
                            //  đã thay đổi % phân bổ của nguồn lực
                            if (oldResource != null && projectResource.Allowcation != oldResource.Allowcation)
                            {
                                noteDescription += "<p>- <strong>" + user.UserName + "</strong>" + " đã thay đổi % phân bổ của nguồn lực " + empName + " từ "
                                     + "<strong>" + oldResource.Allowcation + "</strong>" + "% sang "
                                     + "<strong>" + projectResource.Allowcation + "%</strong></p>";
                            }

                            //  đã bỏ tính cuối tuần cho nguồn lực
                            if (oldResource != null && projectResource.IncludeWeekend != oldResource.IncludeWeekend)
                            {
                                if ((bool)projectResource.IncludeWeekend == false)
                                    noteDescription += "<p>- <strong>" + user.UserName + "</strong>" + " đã bỏ tính cuối tuần cho nguồn lực " + empName + "</p>";
                                else
                                    noteDescription += "<p>- <strong>" + user.UserName + "</strong>" + " đã thêm tính cuối tuần cho nguồn lực " + empName + "</p>";
                            }
                        }
                        else if (resourceType.CategoryCode == "TN")
                        {
                            // [Mã nhân viên] đã chỉnh sửa nguồn lực[Tên nguồn lực]
                            noteDescription += "<p><strong>" + user.UserName + "</strong>" + " đã chỉnh sửa nguồn lực " + empName + "</p>";
                        }
                        note.Description = noteDescription;

                        context.Note.Add(note);

                        projectResourceIdRespone = projectResource.ProjectResourceId;
                        projectResourceId = projectResource.ProjectResourceId;
                        oldResource.IsCreateVendor = projectResource.IsCreateVendor;
                        oldResource.IsOverload = projectResource.IsOverload;
                        oldResource.ResourceType = projectResource.ResourceType;
                        oldResource.EmployeeRole = projectResource.EmployeeRole == null || projectResource.EmployeeRole == Guid.Empty ? Guid.Empty : projectResource.EmployeeRole;
                        oldResource.ObjectId = projectResource.ObjectId.Value;
                        oldResource.StartTime = projectResource.StartTime;
                        oldResource.EndTime = projectResource.EndTime;
                        oldResource.Allowcation = projectResource.Allowcation.Value;
                        oldResource.IncludeWeekend = projectResource.IncludeWeekend;
                        oldResource.ChiPhiTheoGio = projectResource.ChiPhiTheoGio;
                        oldResource.UpdateBy = parameter.UserId;
                        oldResource.UpdateDate = DateTime.Now;
                        context.ProjectResource.Update(oldResource);

                        #region Thông báo
                        NotificationHelper.AccessNotification(context, TypeModel.ProjectResource, "UPD", new ProjectResource(),
                            oldResource, true);
                        #endregion
                    }
                    // Tạo mới nguồn lực
                    else
                    {
                        var proResourceId = Guid.NewGuid();
                        projectResourceIdRespone = proResourceId;
                        projectResourceId = proResourceId;
                        projectResource.IsOverload = projectResource.IsOverload;
                        projectResource.IncludeWeekend = projectResource.IncludeWeekend;
                        projectResource.ProjectResourceId = proResourceId;
                        projectResource.EmployeeRole =
                            projectResource.EmployeeRole == null || projectResource.EmployeeRole == Guid.Empty
                                ? Guid.Empty
                                : projectResource.EmployeeRole;
                        projectResource.CreateBy = parameter.UserId;
                        projectResource.CreateDate = DateTime.Now;

                        context.ProjectResource.Add(projectResource.ToEntity());
                        // Tạo note thêm nguồn lực                    
                        note = new Note
                        {
                            NoteId = Guid.NewGuid(),
                            ObjectType = "PRORESOURCE",
                            ObjectId = parameter.ProjectResource.ProjectId,
                            Type = "SYS",
                            Active = true,
                            CreatedById = parameter.UserId,
                            CreatedDate = DateTime.Now,
                            Description = "<p><strong>" + user.UserName + "</strong>" + " đã thêm nguồn lực " + empName + " vào dự án" + "</p>",
                            NoteTitle = "Đã thêm ghi chú"
                        };
                        context.Note.Add(note);

                        #region Thông báo
                        NotificationHelper.AccessNotification(context, TypeModel.ProjectResource, "CRE",
                            new ProjectResource(), projectResource.ToEntity(), true);
                        #endregion

                        // Copy thông tin Vendor sang bảng ProjectVendor
                        var vendor = context.Vendor.FirstOrDefault(x => x.VendorId == projectResource.ObjectId);
                        if (vendor != null)
                        {
                            var contact = context.Contact.FirstOrDefault(x => x.ObjectId == vendor.VendorId && x.ObjectType == "VEN_CON");
                            if (contact != null)
                            {
                                ProjectVendor projectVendor = new ProjectVendor
                                {
                                    ProjectVendorId = Guid.NewGuid(),
                                    ProjectResourceId = proResourceId,
                                    VendorId = vendor.VendorId,
                                    ProjectId = projectResource.ProjectId,
                                    ContactId = contact == null ? Guid.Empty : contact.ContactId,
                                    PaymentMethodId = vendor.PaymentId,
                                    CreatedById = parameter.UserId,
                                    CreatedDate = DateTime.Now
                                };
                                context.ProjectVendor.Add(projectVendor);
                            }
                        }
                    }

                    #region Tạo account cho nguồn lực thuê ngoài
                    if (projectResource.IsCreateVendor == true)
                    {
                        var vendor = context.Vendor.FirstOrDefault(x => x.VendorId == projectResource.ObjectId);
                        var contact = context.Contact.FirstOrDefault(x => (x.ObjectType == ObjectType.VENDORCONTACT || x.ObjectType == ObjectType.VENDOR) && vendor.VendorId == x.ObjectId);

                        // Kiểm tra và tạo tài khoản cho đối tác
                        var exUser = context.ExternalUser.FirstOrDefault(x => x.ObjectId == contact.ContactId);
                        var passDefault = context.SystemParameter.FirstOrDefault(w => w.SystemKey == "DefaultUserPassword").SystemValueString;
                        // Không tồn tại tài khoản của nhà thầu
                        if (exUser == null)
                        {
                            try
                            {
                                var extUser = new ExternalUser
                                {
                                    ExternalUserId = Guid.NewGuid(),
                                    UserName = vendor.VendorCode,
                                    Password = AuthUtil.GetHashingPassword(passDefault),
                                    ObjectId = vendor.VendorId,
                                    Disabled = false,
                                    Active = true,
                                    CreatedById = parameter.UserId,
                                    CreatedDate = DateTime.Now,
                                    UpdatedById = null,
                                    UpdatedDate = null,
                                    EmployeeId = Guid.Parse("78816DA4-561B-EC11-80D1-005056990AE8")
                                };
                                context.ExternalUser.Add(extUser);

                                var extUserModel = context.ExternalUser.FirstOrDefault(e => e.ObjectId == parameter.ProjectResource.ObjectId);
                                if (extUserModel == null)
                                {
                                    // hardcode
                                    var role = context.Role.FirstOrDefault(e => e.RoleId == Guid.Parse("62B3120C-4602-4902-80ED-01E7B83B7250"));
                                    if (role == null)
                                    {
                                        return new CreateOrUpdateProjectResourceResult
                                        {
                                            StatusCode = HttpStatusCode.ExpectationFailed,
                                            MessageCode = "Không tồn tại Nhóm quyền này trên hệ thống"
                                        };
                                    }
                                    var listUserRoleOld = context.UserRole.Where(e => e.UserId == extUser.ExternalUserId).ToList();
                                    if (listUserRoleOld.Count > 0)
                                    {
                                        context.UserRole.RemoveRange(listUserRoleOld);
                                    }

                                    //Add lại role cho user
                                    //Hiện tại chỉ là 1:1
                                    UserRole userRole = new UserRole();
                                    userRole.UserRoleId = Guid.NewGuid();
                                    userRole.UserId = extUser.ExternalUserId;
                                    userRole.RoleId = role.RoleId;
                                    context.UserRole.Add(userRole);

                                    // Phân quyền cho tk nhà thầu được vào những chức năng nào -- tạo scripts


                                    #region Get Employee Infor to send email   

                                    SendEmailEntityModel.EmployeeName = contact.FirstName;
                                    SendEmailEntityModel.UserName = contact.Email;
                                    SendEmailEntityModel.UserPassword = passDefault;
                                    SendEmailEntityModel.ListSendToEmail.Add(contact.Email);

                                    var configEntity = context.SystemParameter.ToList();

                                    var emailTempCategoryTypeId = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "TMPE")
                                        .CategoryTypeId;
                                    var listEmailTempType =
                                        context.Category.Where(x => x.CategoryTypeId == emailTempCategoryTypeId).ToList();

                                    var emailCategoryId = listEmailTempType.FirstOrDefault(w => w.CategoryCode == "TNV")
                                        .CategoryId;

                                    var emailTemplate =
                                        context.EmailTemplate.FirstOrDefault(w =>
                                            w.Active && w.EmailTemplateTypeId == emailCategoryId);

                                    var subject = ReplaceTokenForContent(context, extUser, emailTemplate.EmailTemplateTitle,
                                        configEntity);
                                    var content = ReplaceTokenForContent(context, extUser, emailTemplate.EmailTemplateContent,
                                        configEntity);

                                    // Tạm thời comment
                                    //Emailer.SendEmail(context, SendEmailEntityModel.ListSendToEmail, new List<string>(), subject, content);
                                    #endregion
                                }
                            }
                            catch (Exception e)
                            {
                                return new CreateOrUpdateProjectResourceResult
                                {
                                    MessageCode = e.Message,
                                    StatusCode = HttpStatusCode.ExpectationFailed,
                                };
                            }
                        }

                    }
                    #endregion

                    // Update ngày thay đổi vào trường LastChangeActivityDate bảng Project
                    var project = context.Project.FirstOrDefault(x => x.ProjectId == parameter.ProjectResource.ProjectId);
                    if (project != null)
                    {
                        project.LastChangeActivityDate = DateTime.Now;
                        context.Project.Update(project);
                    }
                    context.SaveChanges();
                    dbcxtransaction.Commit();

                    #region Check việc Overload nguồn lực - Nguồn lực nội bộ

                    var category = context.Category.FirstOrDefault(x => x.CategoryType.CategoryTypeCode == "NCNL" && x.Active == true && x.CategoryId == projectResource.ResourceType && x.CategoryCode == "NB");
                    if (category != null)
                    {
                        var lstProResource = context.ProjectResource.Where(x => x.ObjectId == projectResource.ObjectId && x.ProjectResourceId != projectResourceId && x.ResourceRole == category.CategoryId).ToList();
                        lstProResource.ForEach(item =>
                        {
                            var obj = new CheckAllowcateProjectResourceParameter();
                            obj.ResourceId = item.ObjectId == null ? Guid.Empty : (Guid)item.ObjectId;
                            obj.FromDate = item.StartTime;
                            obj.ToDate = item.EndTime;
                            obj.Allowcation = item.Allowcation == null ? 0 : (int)item.Allowcation;
                            obj.ProjectResourceId = item.ProjectResourceId;
                            var updateResource = context.ProjectResource.FirstOrDefault(x => x.ProjectResourceId == item.ProjectResourceId);

                            if (CheckAllowcateProjectResource(obj).Status)
                                updateResource.IsOverload = false;

                            updateResource.UpdateDate = DateTime.Now;
                            context.ProjectResource.Update(updateResource);
                        });
                    }

                    #endregion
                }
                return new CreateOrUpdateProjectResourceResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Cập nhật hạng mục thành công.",
                    SendEmailEntityModel = SendEmailEntityModel,
                    ProjectResourceId = projectResourceIdRespone
                };
            }
            catch(Exception e)
            {
                return new CreateOrUpdateProjectResourceResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message              
                };
            }
            
        }
        private static string ReplaceTokenForContent(TNTN8Context context, object model, string emailContent, List<SystemParameter> configEntity)
        {
            var result = emailContent;
            var defaultPass = context.SystemParameter.FirstOrDefault(w => w.SystemKey == "DefaultUserPassword").SystemValueString;

            #region Common Token

            const string Logo = "[LOGO]";
            const string UserName = "[USER_NAME]";
            const string UserPass = "[USER_PASS]";
            const string VendorName = "[EMP_NAME]";
            const string Url_Login = "[ACCESS_SYSTEM]";

            #endregion

            var _model = model as ExternalUser;

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

            if (result.Contains(UserName) && _model.UserName != null)
            {
                result = result.Replace(UserName, _model.UserName);
            }

            if (result.Contains(UserPass) && _model.Password != null)
            {
                result = result.Replace(UserPass, defaultPass);
            }

            if (result.Contains(VendorName))
            {
                var vendorName = context.Vendor.FirstOrDefault(x => x.VendorId == _model.ObjectId)?.VendorName;
                if (!string.IsNullOrEmpty(vendorName))
                {
                    result = result.Replace(VendorName, _model.UserName);
                }
                else
                {
                    result = result.Replace(vendorName, "");
                }
            }

            if (result.Contains(Url_Login))
            {
                var Domain = configEntity.FirstOrDefault(w => w.SystemKey == "Domain").SystemValueString;
                var loginLink = Domain + @"/login?returnUrl=%2Fhome";

                if (!String.IsNullOrEmpty(loginLink))
                {
                    result = result.Replace(Url_Login, loginLink);
                }
            }
            return result;
        }

        public GetMasterMilestoneResult GetMasterMilestone(GetMasterMilestoneParameter parameter)
        {
            var taskNote = context.Note.Where(x => x.ObjectType == "TAS").ToList();
            var listCategoryTypeCodes = new List<string> { "DAT", "PSC", "LDA", "CVT" };
            var listCategory = context.Category.Where(x => listCategoryTypeCodes.Contains(x.CategoryType.CategoryTypeCode) && x.Active == true).Select(y =>
                          new CategoryEntityModel
                          {
                              CategoryId = y.CategoryId,
                              CategoryName = y.CategoryName,
                              CategoryCode = y.CategoryCode,
                              CategoryTypeId = Guid.Empty,
                              CreatedById = Guid.Empty,
                              CategoryTypeCode = y.CategoryType.CategoryTypeCode,
                              CountCategoryById = 0
                          }).ToList();

            #region Danh sách công việc
            var listTask = context.Task.Where(t => t.ProjectId == parameter.ProjectId).Select(x =>
                      new ProjectTaskEntityModel
                      {
                          ProjectId = x.ProjectId,
                          TaskName = x.TaskName,
                          CountNote = taskNote == null ? 0 : taskNote.Where(t => t.ObjectId == x.TaskId).Count(),
                          CreateBy = x.CreateBy,
                          CreateDate = x.CreateDate,
                          CompleteRate = x.TaskComplate,
                          EstimateHour = x.EstimateHour,
                          MilestonesId = x.MilestonesId,
                          StatusName = listCategory.FirstOrDefault(c => c.CategoryId == x.Status).CategoryName
                      }).ToList();
            #endregion

            #region Thông tin dự án


            var project = context.Project.Select(y =>
                             new ProjectEntityModel
                             {
                                 ProjectId = y.ProjectId,
                                 ProjectName = y.ProjectName,
                                 Priority = y.Priority,
                                 ProjectStartDate = y.ProjectStartDate,
                                 ProjectEndDate = y.ProjectEndDate,
                                 ActualEndDate = y.ActualEndDate,
                                 ActualStartDate = y.ActualStartDate,
                             }).Where(x => x.ProjectId == parameter.ProjectId).FirstOrDefault();
            // Mức độ ưu tiên
            if (project.Priority == 1) project.PriorityName = "Thấp";
            else if (project.Priority == 2) project.PriorityName = "Trung bình";
            else project.PriorityName = "Cao";

            // Loại dự án
            project.ProjectTypeName = listCategory.FirstOrDefault(c => c.CategoryId == project.ProjectType)?.CategoryName;
            //Trạng thái dự án
            project.ProjectStatusName = listCategory.FirstOrDefault(c => c.CategoryId == project.ProjectStatus)?.CategoryName;
            #endregion

            #region Danh sách mốc dự án
            // Trạng thái công viêc
            var listMilestone = context.ProjectMilestone.Select(y =>
                             new ProjectMilestoneEntityModel
                             {
                                 ProjectMilestonesId = y.ProjectMilestonesId,
                                 Name = y.Name,
                                 Status = y.Status,
                                 EndTime = y.EndTime,
                                 ProjectId = y.ProjectId,
                                 CreateBy = y.CreateBy,
                                 CreateDate = y.CreateDate,
                                 UpdateDate = y.UpdateDate
                             }).Where(x => x.ProjectId == parameter.ProjectId).ToList();
            #endregion

            return new GetMasterMilestoneResult
            {
                Status = true,
                Message = "",
                ListMilestone = listMilestone,
                Project = project,
                ListTask = listTask
            };
        }

        public UpdateProjectMilestoneResult CreateOrUpdateProjectMilestone(UpdateProjectMilestoneParameter parameter)
        {
            try 
            {
                var milestone = new ProjectMilestone();
                parameter.ProjectMilestone.ProjectMilestonesId = Guid.NewGuid();
                //parameter.ProjectMilestone.Status = DateTime.Now;
                //switch (parameter.Note.Type)
                //{
                //    case "ADD":
                //        parameter.Note.NoteTitle = "đã thêm ghi chú";
                //        if (parameter.FileList != null)
                //        {
                //            parameter.Note.NoteTitle = "đã thêm tài liệu";
                //            CreatedNoteDocument(parameter.Note.NoteId, parameter.LeadId.Value, parameter.UserId, parameter.FileList);
                //        }
                //        break;
                //    case "NEW":
                //        parameter.Note.NoteTitle = "đã tạo";
                //        break;
                //    case "EDT":
                //        parameter.Note.Description = "";
                //        break;
                //    case "UNF":
                //        break;
                //    default:
                //        parameter.Note.NoteTitle = "";
                //        break;
                //}

                //context.Note.Add(parameter.Note);
                context.SaveChanges();

                //context.SaveChanges();

                return new UpdateProjectMilestoneResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Cập nhật mốc dự án thành công."
                };
            }
            catch(Exception e)
            {
                return new UpdateProjectMilestoneResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }        
        }

        public GetAllTaskByProjectScopeIdResult GetAllTaskByProjectScopeId(GetAllTaskByProjectIdParameter parameter)
        {
            try
            {
                var listCategoryTypeCodes = new List<string> { "LCV", "TTCV" };
                var listCategory = context.Category.Where(x => listCategoryTypeCodes.Contains(x.CategoryType.CategoryTypeCode) && x.Active == true).Select(y =>
                              new CategoryEntityModel
                              {
                                  CategoryId = y.CategoryId,
                                  CategoryName = y.CategoryName,
                                  CategoryCode = y.CategoryCode,
                                  CategoryTypeId = Guid.Empty,
                                  CreatedById = Guid.Empty,
                                  CategoryTypeCode = y.CategoryType.CategoryTypeCode,
                                  CountCategoryById = 0
                              }).ToList();
                var listTask = context.Task.Where(t => t.ProjectId == parameter.ProjectId && t.ProjectScopeId == parameter.ProjectScopeId).Select(x =>
                        new TaskEntityModel
                        {
                            ProjectId = x.ProjectId,
                            ProjectScopeId = x.ProjectScopeId,
                            TaskId = x.TaskId,
                            TaskCode = x.TaskCode,
                            TaskName = x.TaskName,
                            StatusName = listCategory.FirstOrDefault(c => c.CategoryId == x.Status).CategoryName,
                            TaskTyeName = listCategory.FirstOrDefault(c => c.CategoryId == x.TaskTypeId).CategoryName,
                        }).ToList();
                listTask.ForEach(task =>
                {
                    // danh sách các task thuộc hạng mục
                    var listTaskRe = context.TaskResourceMapping.Where(x => x.IsPersonInCharge == true && x.TaskId == task.TaskId).Select(a => a.ResourceId).ToList();
                    var listProjectRe = context.ProjectResource.Where(x => listTaskRe.Contains(x.ProjectResourceId)).Select(a => a.ObjectId).ToList();
                    var employees = context.Employee.Where(e => listProjectRe.Contains(e.EmployeeId)).Select(em => em.EmployeeCode + '-' + em.EmployeeName).ToList();
                    task.Employee = employees.Count > 0 ? employees.Aggregate((a, x) => a + ", " + x) : string.Empty;
                });
                return new GetAllTaskByProjectScopeIdResult
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "",
                    ListTask = listTask
                };
            }
            catch(Exception e)
            {
                return new GetAllTaskByProjectScopeIdResult
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message,
                };
            }          
        }

        public DeleteProjectResourceResult DeleteProjectResource(DeleteProjectResourceParameter parameter)
        {
            using (var dbcxtransaction = context.Database.BeginTransaction())
            {
                try
                {
                    var resource = context.ProjectResource.FirstOrDefault(c => c.ProjectResourceId == parameter.ProjectResourceId);

                    var user = context.User.FirstOrDefault(x => x.UserId == parameter.UserId);
                    var lstEmployee = context.Employee.ToList();
                    var resourceName = lstEmployee.FirstOrDefault(x => x.EmployeeId == resource.ObjectId) != null
                        ? lstEmployee.FirstOrDefault(x => x.EmployeeId == resource.ObjectId).EmployeeName
                        : context.Vendor.FirstOrDefault(x => x.VendorId == resource.ObjectId).VendorName;
                    var empName = lstEmployee.FirstOrDefault(x => x.EmployeeId == user.EmployeeId).EmployeeName;
                    // Check nguồn lực chưa được găn với task nào thì mới cho phép xóa
                    var isTaskResource = context.TaskResourceMapping.FirstOrDefault(x => x.ResourceId == resource.ProjectResourceId);
                    if (resource == null || isTaskResource != null)
                    {
                        return new DeleteProjectResourceResult
                        {
                            StatusCode = HttpStatusCode.ExpectationFailed,
                            MessageCode = CommonMessage.ProjectResource.DELETE_FAIL
                        };
                    }
                    else
                    {

                        context.ProjectResource.Remove(resource);

                        Note note = new Note
                        {
                            NoteId = Guid.NewGuid(),
                            ObjectType = "PRORESOURCE",
                            ObjectId = resource.ProjectId,
                            Type = "SYS",
                            Active = true,
                            CreatedById = parameter.UserId,
                            CreatedDate = DateTime.Now,
                            NoteTitle = "Đã thêm ghi chú",
                            Description = "<p><strong>" + empName + "</strong>" + " đã xóa nguồn lực " + resourceName + "</p>"

                        };
                        context.Note.Add(note);
                        // Update ngày thay đổi vào trường LastChangeActivityDate bảng Project
                        var project = context.Project.FirstOrDefault(x => x.ProjectId == resource.ProjectId);
                        if (project != null)
                        {
                            project.LastChangeActivityDate = DateTime.Now;
                            context.Project.Update(project);
                        }
                        context.SaveChanges();
                        dbcxtransaction.Commit();
                    }
                    return new DeleteProjectResourceResult
                    {
                        StatusCode = HttpStatusCode.OK,
                        MessageCode = CommonMessage.ProjectResource.DELETE_SUCCESS
                    };
                }
                catch (Exception ex)
                {
                    dbcxtransaction.Rollback();
                    return new DeleteProjectResourceResult
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = ex.Message
                    };
                }
            }

        }
        public UpdateProjectVendorResult UpdateProjectVendorResource(UpdateProjectVendorParameter parameter)
        {
            try
            {
                if (parameter.ProjectVendor.ProjectVendorId == Guid.Empty)
                {
                    return new UpdateProjectVendorResult()
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = "Nguồn lực không có trên hệ thống",
                    };
                }
                else
                {
                    var projectVendor = context.ProjectVendor.FirstOrDefault(s =>
                        s.ProjectVendorId == parameter.ProjectVendor.ProjectVendorId);
                    if (projectVendor != null)
                    {
                        projectVendor.ContactId = parameter.ProjectVendor.ContactId;
                        projectVendor.PaymentMethodId = parameter.ProjectVendor.PaymentMethodId;
                        projectVendor.UpdatedById = parameter.UserId;
                        projectVendor.UpdatedDate = DateTime.Now;
                        context.ProjectVendor.Update(projectVendor);
                        // Update ngày thay đổi vào trường LastChangeActivityDate bảng Project
                        var project = context.Project.FirstOrDefault(x => x.ProjectId == parameter.ProjectVendor.ProjectId);
                        if (project != null)
                        {
                            project.LastChangeActivityDate = DateTime.Now;
                            context.Project.Update(project);
                        }
                    }
                    else
                    {
                        return new UpdateProjectVendorResult()
                        {
                            StatusCode = HttpStatusCode.ExpectationFailed,
                            MessageCode = "Nguồn lực không có trên hệ thống",
                        };
                    }

                    context.SaveChanges();
                    return new UpdateProjectVendorResult()
                    {
                        StatusCode = HttpStatusCode.OK,
                        MessageCode = "Cập nhật nhà thầu thành công.",
                    };
                }
            }
            catch (Exception ex)
            {
                return new UpdateProjectVendorResult
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = ex.ToString()
                };
            }
        }

        public DeleteProjectScopeResult DeleteProjectScope(DeleteProjectScopeParameter parameter)
        {
            try
            {
                var proScope = context.ProjectScope.FirstOrDefault(c => c.ProjectScopeId == parameter.ProjectScopeId);
                if (proScope == null)
                {
                    return new DeleteProjectScopeResult()
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = "Hạng mục không tồn tại trên hệ thống"
                    };
                }

                context.ProjectScope.Remove(proScope);

                // Update ngày thay đổi vào trường LastChangeActivityDate bảng Project
                var project = context.Project.FirstOrDefault(x => x.ProjectId == parameter.ProjectId);
                if (project != null)
                {
                    project.LastChangeActivityDate = DateTime.Now;
                    context.Project.Update(project);
                }
                context.SaveChanges();

                CreateProjectScopeNoteStatus(parameter.ProjectId, parameter.UserId, proScope.ProjectScopeName, "DEL");

                var projectScopes = context.ProjectScope.Where(x => x.ProjectId == parameter.ProjectId).ToList();
                var listProjectScope = projectScopes.Select(p => new ProjectScopeModel()
                {
                    ProjectScopeId = p.ProjectScopeId,
                    Description = p.Description,
                    ResourceType = p.ResourceType,
                    ProjectScopeName = p.ProjectScopeName,
                    ProjectScopeCode = p.ProjectScopeCode,
                    TenantId = p.TenantId,
                    ParentId = p.ParentId,
                    ProjectId = p.ProjectId,
                    Level = p.Level
                }).ToList();

                var projectScopeGuids = projectScopes.Select(ps => ps.ProjectScopeId).ToList();
                var tasks = context.Task.Where(x => projectScopeGuids.Contains((Guid)x.ProjectScopeId)).ToList();

                listProjectScope.ForEach(item =>
                {
                    // danh sách các task thuộc hạng mục
                    var listTask = tasks.Where(x => x.ProjectScopeId == item.ProjectScopeId).Select(x => x.TaskId)
                        .ToList();
                    var listTaskRe = context.TaskResourceMapping
                        .Where(x => x.IsPersonInCharge == true && listTask.Contains(x.TaskId)).Select(a => a.ResourceId)
                        .ToList();
                    var listProjectRe = context.ProjectResource.Where(x => listTaskRe.Contains(x.ProjectResourceId))
                        .Select(a => a.ObjectId).ToList();
                    var lstEmp = context.Employee.Where(e => listProjectRe.Contains(e.EmployeeId))
                        .Select(em => em.EmployeeName).ToList();
                    var listResourceScope = new List<string>();
                    lstEmp.ForEach(emp => { listResourceScope.Add(lstEmp.Count > 0 ? emp : string.Empty); });
                    item.ListEmployee = listResourceScope;
                });

                listProjectScope = SetTTChildren(listProjectScope);

                #region get list notes

                var listNote = new List<NoteEntityModel>();
                listNote = context.Note.Where(w => w.Active == true && w.ObjectType == "PROSCOPE" && w.ObjectId == parameter.ProjectId).Select(w => new NoteEntityModel
                {
                    NoteId = w.NoteId,
                    Description = w.Description,
                    Type = w.Type,
                    ObjectId = w.ObjectId,
                    ObjectType = w.ObjectType,
                    NoteTitle = w.NoteTitle,
                    CreatedById = w.CreatedById,
                    CreatedDate = w.CreatedDate,
                    UpdatedById = w.UpdatedById,
                    UpdatedDate = w.UpdatedDate,
                    NoteDocList = new List<NoteDocumentEntityModel>()
                }).ToList();

                //lấy tên người tạo, người chỉnh sửa cho note
                listNote.ForEach(note =>
                {
                    var empId = context.User.FirstOrDefault(f => f.UserId == note.CreatedById).EmployeeId;
                    var contact = context.Contact.FirstOrDefault(f => f.ObjectType == "EMP" && f.ObjectId == empId);
                    if (contact != null)
                    {
                        note.ResponsibleName = contact.FirstName + " " + contact.LastName;
                    }
                });

                // Sắp xếp lại listnote
                listNote = listNote.OrderByDescending(x => x.CreatedDate).ToList();

                #endregion

                return new DeleteProjectScopeResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = CommonMessage.ProjectScope.DELETE_SUCCESS,
                    ListProjectScope = listProjectScope,
                    ListNote = listNote
                };
            }
            catch (Exception e)
            {
                return new DeleteProjectScopeResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public CheckAllowcateProjectResourceResult CheckAllowcateProjectResource(CheckAllowcateProjectResourceParameter parameter, bool isCreateOrUpdet = true)
        {
            try
            {
                var categoryId = context.Category.FirstOrDefault(x => x.CategoryCode == "NB" && x.Active == true).CategoryId;
                /* Lấy thông tin các dự án mà nguồn lực tham gia trong khoảng thời gian.
                Ngày bắt đầu nguồn lực hoặc ngày kết thúc nguồn lực thuộc các khoảng bắt đầu và kết thúc của các dự án sử dụng nguồn lực
                */
                var lstProjectResource = new List<ProjectResource>();
                //if (isCreateOrUpdet)
                // Danh sach nguồn lực - Ngoại trừ nguồn lực được truyền vào (đối với trường hợp Thêm mới hoặc chỉnh sửa)
                lstProjectResource = context.ProjectResource.Where(x => x.ProjectResourceId != parameter.ProjectResourceId && x.ResourceRole == categoryId
                && x.ObjectId == parameter.ResourceId && ((x.StartTime <= parameter.FromDate && parameter.FromDate <= x.EndTime)
                      || (x.StartTime <= parameter.ToDate && parameter.ToDate <= x.EndTime))).ToList();
                //else
                //    // Danh sach phan bo nguon luc do trong ca du an
                //    lstProjectResource = context.ProjectResource.Where(x => x.ResourceRole == categoryId && x.ObjectId == parameter.ResourceId).ToList();
                int totalPercent = 0;

                totalPercent = lstProjectResource.Sum(x => x.Allowcation);


                //int totalPercent = context.ProjectResource.Where(x => x.ProjectResourceId != parameter.ProjectResourceId &&
                //x.ResourceRole == categoryId &&
                //x.ObjectId == parameter.ResourceId &&
                //((x.StartTime >= parameter.FromDate && x.EndTime <= parameter.ToDate)
                //|| (x.EndTime >= parameter.FromDate && x.EndTime <= parameter.ToDate) ||
                //    (x.StartTime >= parameter.FromDate && x.StartTime <= parameter.ToDate)
                //)).Sum(x => x.Allowcation);

                if ((parameter.Allowcation + totalPercent) > 100)
                {
                    return new CheckAllowcateProjectResourceResult
                    {
                        TotalAllowcation = parameter.Allowcation + totalPercent,
                        MessageCode = "Phân bổ nguồn lực quá 100%",
                        StatusCode = HttpStatusCode.ExpectationFailed,
                    };
                }
                else
                    return new CheckAllowcateProjectResourceResult
                    {
                        TotalAllowcation = parameter.Allowcation + totalPercent,
                        MessageCode = "Phân bổ nguồn lực thành công",
                        StatusCode = HttpStatusCode.OK,
                    };
            }
            catch (Exception ex)
            {
                return new CheckAllowcateProjectResourceResult
                {
                    TotalAllowcation = 0,
                    MessageCode = ex.Message,
                    StatusCode = HttpStatusCode.ExpectationFailed,
                };
            }
        }

        public GetMasterProjectDocumentResult GetMasterProjectDocument(GetMasterProjectDocumentParameter parameter)
        {
            try
            {
                var listAllTask = context.Task.ToList();
                var listAllUser = context.User.ToList();
                var listAllEmp = context.Employee.ToList();
                var totalSize = 0M;

                var commonListCategory = context.Category.Where(c => c.Active == true)
                    .Select(m => new CategoryEntityModel
                    {
                        CategoryId = m.CategoryId,
                        CategoryCode = m.CategoryCode,
                        CategoryName = m.CategoryName,
                        CategoryTypeId = m.CategoryTypeId
                    }).ToList();

                var project = context.Project.Where(x => x.ProjectId == parameter.ProjectId)
                    .Select(y => new ProjectEntityModel
                    {
                        ProjectId = y.ProjectId,
                        ProjectStartDate = y.ProjectStartDate,
                        ProjectEndDate = y.ProjectEndDate,
                        ActualStartDate = y.ActualStartDate,
                        ActualEndDate = y.ActualEndDate,
                        ProjectManagerId = y.ProjectManagerId,
                        ContractId = y.ContractId,
                        ProjectName = y.ProjectName,
                        ProjectCode = y.ProjectCode,
                        BudgetVnd = y.BudgetVnd,
                        BudgetUsd = y.BudgetUsd,
                        BudgetNgayCong = y.BudgetNgayCong,
                        CustomerId = y.CustomerId,
                        Description = y.Description,
                        ProjectSize = y.ProjectSize,
                        ProjectType = y.ProjectType,
                        ProjectStatus = y.ProjectStatus,
                        IncludeWeekend = y.IncludeWeekend,
                        Priority = y.Priority,
                    }).FirstOrDefault();

                if (project == null)
                {
                    return new GetMasterProjectDocumentResult()
                    {
                        MessageCode = "Dự án không tồn tại trong hệ thống",
                        StatusCode = HttpStatusCode.ExpectationFailed
                    };
                }

                project.ProjectTypeName = commonListCategory.FirstOrDefault(c => c.CategoryId == project.ProjectType)?.CategoryName;
                project.ProjectStatusName = commonListCategory.FirstOrDefault(c => c.CategoryId == project.ProjectStatus)?.CategoryName;
                project.ProjectStatusCode = commonListCategory.FirstOrDefault(c => c.CategoryId == project.ProjectStatus)?.CategoryCode;
                switch (project.Priority)
                {
                    case 1:
                        project.PriorityName = "Thấp";
                        break;
                    case 2:
                        project.PriorityName = "Trung bình";
                        break;
                    case 3:
                        project.PriorityName = "Cao";
                        break;
                    default: break;
                }

                CalculatorProjectTask(parameter.ProjectId, out decimal projectComplete, out decimal totalEstimateHour);

                #region Lấy dánh sách folder thuộc dụ án

                var folderRootProject = context.Folder.FirstOrDefault(x => x.FolderType == "QLDA");

                var webRootPath = iHostingEnvironment.WebRootPath + "\\";

                if (folderRootProject == null)
                {
                    return new GetMasterProjectDocumentResult()
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = "Thư mục upload không tồn tại",
                    };
                }

                var listProjectFolder = context.Folder.Where(x =>
                    x.FolderType.Contains(project.ProjectCode) && x.ObjectId == parameter.ProjectId && !x.FolderType.Contains("TASK_FILE")).ToList();

                if (listProjectFolder.Count > 0)
                {
                    listProjectFolder.ForEach(item =>
                    {
                        var path = Path.Combine(webRootPath, item.Url);
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                    });
                }
                else
                {
                    #region Tạo thư mục lưu file cho dự án

                    var projectCodeName = $"{project.ProjectCode} - {project.ProjectName}";

                    var folderRootProjectUrl = Path.Combine(webRootPath, ConvertFolderUrl(folderRootProject.Url));

                    if (Directory.Exists(folderRootProjectUrl))
                    {
                        #region folder gốc theo code và tên dự án

                        var folder = new Folder()
                        {
                            FolderId = Guid.NewGuid(),
                            Active = true,
                            CreatedById = parameter.UserId,
                            CreatedDate = DateTime.Now,
                            UpdatedById = parameter.UserId,
                            UpdatedDate = DateTime.Now,
                            FolderLevel = folderRootProject.FolderLevel + 1,
                            IsDelete = false,
                            Name = projectCodeName,
                            ParentId = folderRootProject.FolderId,
                            Url = folderRootProject.Url + @"\" + projectCodeName,
                            FolderType = project.ProjectCode.ToUpper(),
                            ObjectId = project.ProjectId,
                        };

                        var folderName = ConvertFolderUrl(folder.Url);
                        var newPath = Path.Combine(webRootPath, folderName);
                        if (!Directory.Exists(newPath))
                        {
                            context.Folder.Add(folder);
                            context.SaveChanges();
                            Directory.CreateDirectory(newPath);
                        }

                        #endregion

                        #region folder cho tài liệu công việc

                        var folderTask = new Folder()
                        {
                            FolderId = Guid.NewGuid(),
                            Active = true,
                            CreatedById = parameter.UserId,
                            CreatedDate = DateTime.Now,
                            UpdatedById = parameter.UserId,
                            UpdatedDate = DateTime.Now,
                            FolderLevel = folder.FolderLevel + 1,
                            IsDelete = false,
                            Name = $"{project.ProjectCode} - Công việc",
                            ParentId = folder.FolderId,
                            FolderType = $"{project.ProjectCode.ToUpper()}_TASK_FILE",
                            ObjectId = project.ProjectId,
                        };
                        folderTask.Url = folder.Url + @"\" + folderTask.Name;
                        var taskFolderUrl = ConvertFolderUrl(folderTask.Url);
                        string newPathTask = Path.Combine(webRootPath, taskFolderUrl);

                        if (!Directory.Exists(newPathTask))
                        {
                            context.Folder.Add(folderTask);
                            context.SaveChanges();
                            Directory.CreateDirectory(newPathTask);
                        }

                        #endregion

                        // #region folder cho tài liệu ghi chú project
                        //
                        // // var folderNote = new Folder()
                        // // {
                        // //     FolderId = Guid.NewGuid(),
                        // //     Active = true,
                        // //     CreatedById = parameter.UserId,
                        // //     CreatedDate = DateTime.Now,
                        // //     UpdatedById = parameter.UserId,
                        // //     UpdatedDate = DateTime.Now,
                        // //     FolderLevel = folder.FolderLevel + 1,
                        // //     IsDelete = false,
                        // //     Name = $"{project.ProjectCode} - Thông tin chung",
                        // //     ParentId = folder.FolderId,
                        // //     FolderType = $"{project.ProjectCode.ToUpper()}_DETAIL_NOTE",
                        // //     ObjectId = project.ProjectId,
                        // // };
                        // // folderNote.Url = folder.Url + @"\" + folderNote.Name;
                        // // var noteFolderUrl = ConvertFolderUrl(folderNote.Url);
                        // // string newPathNote = Path.Combine(webRootPath, noteFolderUrl);
                        // //
                        // // if (!Directory.Exists(newPathNote))
                        // // {
                        // //     context.Folder.Add(folderNote);
                        // //     context.SaveChanges();
                        // //     Directory.CreateDirectory(newPathNote);
                        // }
                        //
                        // #endregion

                        #region folder cho tài liệu dự án

                        var folderFile = new Folder()
                        {
                            FolderId = Guid.NewGuid(),
                            Active = true,
                            CreatedById = parameter.UserId,
                            CreatedDate = DateTime.Now,
                            UpdatedById = parameter.UserId,
                            UpdatedDate = DateTime.Now,
                            FolderLevel = folder.FolderLevel + 1,
                            IsDelete = false,
                            Name = $"{project.ProjectCode} - Tài liệu",
                            ParentId = folder.FolderId,
                            FolderType = $"{project.ProjectCode.ToUpper()}_PROJECT_FILE",
                            ObjectId = project.ProjectId,
                        };
                        folderFile.Url = folder.Url + @"\" + folderFile.Name;
                        var projectFolderUrl = ConvertFolderUrl(folderFile.Url);
                        var newPathProject = Path.Combine(webRootPath, projectFolderUrl);

                        if (!Directory.Exists(newPathProject))
                        {
                            context.Folder.Add(folderFile);
                            context.SaveChanges();
                            Directory.CreateDirectory(newPathProject);
                        }

                        #endregion
                    }
                    else
                    {
                        return new GetMasterProjectDocumentResult()
                        {
                            StatusCode = HttpStatusCode.ExpectationFailed,
                            MessageCode = "Bạn phải cấu hình thư mục để lưu file",
                        };
                    }

                    #endregion
                }


                var listFolder = listProjectFolder.Where(x => x.Active && x.FolderType.Contains(project.ProjectCode.ToUpper()) && x.ObjectId == parameter.ProjectId)
                    .Select(y => new FolderEntityModel
                    {
                        FolderId = y.FolderId,
                        ParentId = y.ParentId,
                        Name = y.Name,
                        Url = y.Url,
                        IsDelete = y.IsDelete,
                        Active = y.Active,
                        FolderType = y.FolderType,
                        FolderLevel = y.FolderLevel,
                        ObjectId = y.ObjectId,
                    })
                    .OrderBy(z => z.Url)
                    .ToList();


                listFolder.ForEach(item =>
                {
                    item.HasChild = listFolder.FirstOrDefault(x => x.ParentId == item.FolderId) != null;
                });
                listFolder.ForEach(item =>
                {
                    item.FileNumber = GetAllFile(item.FolderId, listFolder, context.FileInFolder.ToList()).Count;
                });

                #endregion


                var listDocument = new List<NoteDocumentEntityModel>();

                var listfolderId = listProjectFolder.Select(x => x.FolderId).ToList();

                #region Lấy danh sách note file

                var listAllProjectNoteId =
                    context.Note.Where(x => x.ObjectId == parameter.ProjectId && x.ObjectType == "Project")
                        .Select(y => y.NoteId)
                        .ToList();

                var listNoteDocument = context.FileInFolder
                    .Where(x => x.ObjectId != null && listAllProjectNoteId.Contains((Guid)x.ObjectId) && x.ObjectType == "NOTE" && listfolderId.Contains(x.FolderId)).ToList();

                listNoteDocument.ForEach(item =>
                {
                    var file = new NoteDocumentEntityModel
                    {
                        NoteDocumentId = item.FileInFolderId,
                        NoteId = (Guid)item.ObjectId,
                        DocumentSize = item.Size,
                        ObjectType = item.ObjectType,
                        FolderId = item.FolderId,
                        UpdatedDate = item.UpdatedDate,
                        Active = item.Active,
                    };

                    var folderUrl = context.Folder.FirstOrDefault(x => x.FolderId == item.FolderId)?.Url;
                    var fileName = $"{item.FileName}.{item.FileExtension}";

                    file.DocumentUrl = Path.Combine(webRootPath, folderUrl, fileName);
                    file.DocumentName = fileName;

                    listDocument.Add(file);

                    totalSize += Convert.ToDecimal(item.Size);
                });

                // var listNoteDocument =
                //     context.NoteDocument.Where(x => listAllProjectNoteId.Contains(x.NoteId))
                //         .Select(y => new NoteDocumentEntityModel
                //         {
                //             NoteDocumentId = y.NoteDocumentId,
                //             NoteId = y.NoteId,
                //             DocumentName = y.DocumentName,
                //             DocumentSize = y.DocumentSize,
                //             DocumentUrl = y.DocumentUrl,
                //             UpdatedDate = y.UpdatedDate,
                //             Active = y.Active
                //         })
                //         .OrderBy(z => z.UpdatedDate)
                //         .ToList();
                //
                // listNoteDocument.ForEach(item =>
                // {
                //     totalSize += Convert.ToDecimal(item.DocumentSize);
                // });

                #endregion

                #region Lấy danh sách task file

                var listTaskDocument = new List<TaskDocumentEntityModel>();

                var listTaskId = listAllTask.Where(x => x.ProjectId == parameter.ProjectId)
                    .Select(y => y.TaskId).ToList();

                var listFileInFolderDocument =
                    context.FileInFolder.Where(x => listTaskId.Contains((Guid)x.ObjectId) && x.ObjectType == "TASK")
                        .OrderBy(z => z.CreatedDate)
                        .ToList();

                listFileInFolderDocument.ForEach(item =>
                {
                    var file = new TaskDocumentEntityModel()
                    {
                        TaskDocumentId = item.FileInFolderId,
                        TaskId = (Guid)item.ObjectId,
                        DocumentSize = item.Size,
                        ObjectType = item.ObjectType,
                        FolderId = item.FolderId,
                        CreatedDate = item.CreatedDate,
                        CreatedById = item.CreatedById,
                        Active = item.Active,
                    };

                    var folderUrl = context.Folder.FirstOrDefault(x => x.FolderId == item.FolderId)?.Url;
                    var fileName = $"{item.FileName}.{item.FileExtension}";

                    file.DocumentUrl = Path.Combine(webRootPath, folderUrl, fileName);
                    file.DocumentName = fileName;

                    listTaskDocument.Add(file);
                });

                listTaskDocument.ForEach(item =>
                {
                    var taskCode = listAllTask.FirstOrDefault(x => x.TaskId == item.TaskId)?.TaskCode;
                    var taskName = listAllTask.FirstOrDefault(x => x.TaskId == item.TaskId)?.TaskName;
                    item.TaskCodeName = $"{taskCode ?? ""}: {taskName ?? ""}";

                    var empId = listAllUser.FirstOrDefault(x => x.UserId == item.CreatedById)?.EmployeeId;
                    var empCode = listAllEmp.FirstOrDefault(x => x.EmployeeId == empId)?.EmployeeCode;
                    var empName = listAllEmp.FirstOrDefault(x => x.EmployeeId == empId)?.EmployeeName;
                    item.CreateByName = $"{empCode ?? ""} - {empName ?? ""}";

                    totalSize += Convert.ToDecimal(item.DocumentSize);
                });

                #endregion

                #region lấy danh sách các file còn lại

                var listFileInFolder = context.FileInFolder
                    .Where(x => x.ObjectId == parameter.ProjectId && x.ObjectType == "QLDA" && listfolderId.Contains(x.FolderId))
                    .ToList();

                listFileInFolder.ForEach(item =>
                {
                    var file = new NoteDocumentEntityModel
                    {
                        NoteDocumentId = item.FileInFolderId,
                        ObjectId = (Guid)item.ObjectId,
                        ObjectType = item.ObjectType,
                        FolderId = item.FolderId,
                        DocumentSize = item.Size,
                        UpdatedDate = item.UpdatedDate,
                        Active = item.Active,
                        CreatedById = item.CreatedById,
                    };

                    var folderUrl = context.Folder.FirstOrDefault(x => x.FolderId == item.FolderId)?.Url;
                    var fileName = $"{item.FileName}.{item.FileExtension}";

                    file.DocumentUrl = Path.Combine(webRootPath, folderUrl, fileName);
                    file.DocumentName = fileName;

                    var empId = listAllUser.FirstOrDefault(x => x.UserId == item.CreatedById)?.EmployeeId;
                    var empCode = listAllEmp.FirstOrDefault(x => x.EmployeeId == empId)?.EmployeeCode;
                    var empName = listAllEmp.FirstOrDefault(x => x.EmployeeId == empId)?.EmployeeName;
                    file.CreateByName = $"{empCode ?? ""} - {empName ?? ""}";

                    listDocument.Add(file);

                    totalSize += Convert.ToDecimal(item.Size);
                });
                #endregion

                #region list dự án theo phân quyền user

                var listAllProject = context.Project.ToList();

                var user = context.User.FirstOrDefault(c => c.UserId == parameter.UserId);

                if (user != null)
                {
                    var employee = context.Employee.FirstOrDefault(c => c.EmployeeId == user.EmployeeId);

                    if (employee != null)
                    {
                        var positionEmp = context.Position.FirstOrDefault(c => c.PositionId == employee.PositionId);
                        if (positionEmp != null && positionEmp.PositionCode == "GD")
                        {
                            var isRoot = context.Organization.FirstOrDefault(c => c.OrganizationId == employee.OrganizationId).ParentId == null;
                            if (!isRoot)
                            {
                                // Giám đốc được set đơn vị cao nhất trong tổ chức - Get All
                                // Lấy những bản ghi là quản lý, quản lý cấp cao, subPM - trong nguồn lực
                                // Những dự án có trong nguồn lực
                                var listProjectFollowResourceId = context.ProjectResource.Where(c => c.ObjectId == employee.EmployeeId).Select(m => m.ProjectId).ToList();
                                // Những dự án là quản lý, quản lý cấp cao, đồng quản lý
                                var listProjectFollowManagerId = context.ProjectEmployeeMapping.Where(c => c.EmployeeId == employee.EmployeeId).Select(c => c.ProjectId).ToList();

                                var listId = new List<Guid>();
                                listId.AddRange(listProjectFollowResourceId);
                                listId.AddRange(listProjectFollowManagerId);

                                listAllProject = listAllProject.Where(c => listId.Contains(c.ProjectId) || c.ProjectManagerId == employee.EmployeeId || c.CreateBy == user.UserId).ToList();
                            }
                        }
                        else
                        {
                            // Những dự án có trong nguồn lực
                            var listProjectFollowResourceId = context.ProjectResource.Where(c => c.ObjectId == employee.EmployeeId).Select(m => m.ProjectId).ToList();
                            // Những dự án là quản lý, quản lý cấp cao, đồng quản lý
                            var listProjectFollowManagerId = context.ProjectEmployeeMapping.Where(c => c.EmployeeId == employee.EmployeeId).Select(c => c.ProjectId).ToList();

                            var listId = new List<Guid>();
                            listId.AddRange(listProjectFollowResourceId);
                            listId.AddRange(listProjectFollowManagerId);

                            listAllProject = listAllProject.Where(c => listId.Contains(c.ProjectId) || c.ProjectManagerId == employee.EmployeeId || c.CreateBy == user.UserId).ToList();
                        }
                    }

                }

                var listProject = listAllProject
                        .Select(m => new ProjectEntityModel
                        {
                            ProjectId = m.ProjectId,
                            ProjectCode = m.ProjectCode,
                            ProjectName = m.ProjectName
                        }).ToList();

                #endregion

                return new GetMasterProjectDocumentResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "successful",
                    ListTaskDocument = listTaskDocument,
                    ListDocument = listDocument,
                    ListFolders = listFolder,
                    Project = project,
                    ProjectTaskComplete = projectComplete,
                    TotalEstimateHour = totalEstimateHour,
                    TotalSize = totalSize,
                    ListProject = listProject
                };
            }
            catch (Exception e)
            {
                return new GetMasterProjectDocumentResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        private List<FileInFolderEntityModel> GetAllFile(Guid folderId, List<FolderEntityModel> listCommonFolders, List<FileInFolder> listCommonFile)
        {
            var listResult = new List<FileInFolderEntityModel>();

            var listFile = listCommonFile.Where(x => x.FolderId == folderId).ToList();

            listFile.ForEach(item =>
            {
                var fileInFolder = new FileInFolderEntityModel
                {
                    FileInFolderId = item.FileInFolderId,
                    FolderId = item.FolderId,
                    FileName = item.FileName,
                    ObjectId = item.ObjectId,
                    ObjectType = item.ObjectType,
                    FileExtension = item.FileExtension,
                    Size = item.Size,
                    Active = item.Active,
                    CreatedById = item.CreatedById,
                    CreatedDate = item.CreatedDate,
                    UpdatedById = item.UpdatedById,
                    UpdatedDate = item.UpdatedDate
                };

                listResult.Add(fileInFolder);
            });

            var folder = listCommonFolders.FirstOrDefault(x => x.FolderId == folderId);

            if (folder != null && folder.HasChild)
            {
                var listFolderChild = listCommonFolders.Where(x => x.ParentId == folderId).ToList();

                listFolderChild.ForEach(item =>
                {
                    listResult.AddRange(GetAllFile(item.FolderId, listCommonFolders, listCommonFile));
                });
            }

            return listResult;
        }

        public void CalculatorProjectTask(Guid projectId, out decimal projectComplete, out decimal totalEstimateHour)
        {
            var listTask = context.Task.Where(c => c.ProjectId == projectId).ToList();
            var total = 0M;
            var taskComplete = 0M;
            listTask.ForEach(item =>
            {
                taskComplete += (item?.TaskComplate ?? 0) / 100 * (item?.EstimateHour ?? 0);
                total += item?.EstimateHour ?? 0;
            });
            totalEstimateHour = total;

            projectComplete = total != 0 ? taskComplete / total * 100 : 0M;
        }

        private void CreateProjectScopeNoteStatus(Guid objectId, Guid userId, string nameOfCategory = null, string type = null, string scopeDesOld = null)
        {
            Note note = new Note
            {
                NoteId = Guid.NewGuid(),
                ObjectType = "PROSCOPE",
                ObjectId = objectId,
                Type = "SYS",
                Active = true,
                CreatedById = userId,
                CreatedDate = DateTime.Now,
                NoteTitle = "Đã thêm ghi chú"
            };

            var user = context.User.FirstOrDefault(x => x.UserId == userId);
            if (user != null)
            {
                switch (type)
                {
                    case "ADD":
                        note.Description = "<p><strong>" + user.UserName + "</strong>" + " đã thêm hạng mục " + nameOfCategory + " vào dự án" + "</p>";
                        break;
                    case "EDT":
                        note.Description = "<p><strong>" + user.UserName + "</strong>" + " đã chỉnh sửa hạng mục " + nameOfCategory + "</p>";
                        break;
                    case "DEL":
                        note.Description = "<p><strong>" + user.UserName + "</strong>" + " đã xóa hạng mục " + nameOfCategory + "</p>";
                        break;
                }
            }
            context.Note.Add(note);
            // Update ngày thay đổi vào trường LastChangeActivityDate bảng Project
            var project = context.Project.FirstOrDefault(x => x.ProjectId == objectId);
            if (project != null)
            {
                project.LastChangeActivityDate = DateTime.Now;
                context.Project.Update(project);
            }
            context.SaveChanges();
        }

        private int TotalHoliday(DateTime startDate, DateTime endDate, bool includeWeekend)
        {
            int total = 0;
            while (startDate != endDate)
            {
                if (includeWeekend == false)
                {
                    if (startDate.DayOfWeek == DayOfWeek.Saturday || startDate.DayOfWeek == DayOfWeek.Sunday)
                    {
                        total += 1;
                    }
                }

                startDate = startDate.AddDays(1);
            }

            if (endDate.DayOfWeek == DayOfWeek.Saturday || endDate.DayOfWeek == DayOfWeek.Sunday)
            {
                if (includeWeekend == false)
                {
                    total += 1;
                }
            }

            return total;
        }

        public GetPermissionResult GetPermission(GetPermissionParameter parameter)
        {
            try
            {
                var project = context.Project.FirstOrDefault(c => c.ProjectId == parameter.ProjectId);
                if (project == null)
                {
                    return new GetPermissionResult
                    {
                        MessageCode = "Dự án không tồn tại trong hệ thống",
                        StatusCode = HttpStatusCode.ExpectationFailed
                    };
                }
                var user = context.User.FirstOrDefault(c => c.UserId == parameter.UserId);
                if (user == null)
                {
                    return new GetPermissionResult
                    {
                        MessageCode = "Nhân viên không tồn tại trong hệ thống",
                        StatusCode = HttpStatusCode.ExpectationFailed
                    };
                }
                var employee = context.Employee.FirstOrDefault(c => c.EmployeeId == user.EmployeeId);
                if (employee == null)
                {
                    return new GetPermissionResult
                    {
                        MessageCode = "Nhân viên không tồn tại trong hệ thống",
                        StatusCode = HttpStatusCode.ExpectationFailed
                    };
                }
                var listTextActionResource = new List<string>();

                #region Phân quyền

                var position = context.Position.FirstOrDefault(c => c.PositionId == employee.PositionId);
                if (position != null && ( position.PositionCode == "GD" || position.PositionCode == "GM"))
                {
                    listTextActionResource = GetActionResource("TQDA");
                }
                else
                {
                    var isManager = context.ProjectEmployeeMapping.Where(c =>
                                            employee.EmployeeId == c.EmployeeId && c.ProjectId == project.ProjectId)
                                        .Count() != 0;

                    if (project.ProjectManagerId != employee.EmployeeId && !isManager)
                    {
                        var isResource = context.ProjectResource.Where(c =>
                                                 c.ObjectId == employee.EmployeeId && c.ProjectId == project.ProjectId)
                                             .Count() != 0;
                        if (isResource)
                        {
                            listTextActionResource = GetActionResource("NVDA");
                        }
                        else
                        {
                            listTextActionResource = GetActionResource("NTKTNL");
                        }
                    }
                    else
                    {
                        listTextActionResource = GetActionResource("TQDA");
                    }
                }

                #endregion

                var permissionStr = string.Join(',', listTextActionResource);
                return new GetPermissionResult
                {
                    MessageCode = "Success",
                    StatusCode = HttpStatusCode.OK,
                    ListPermission = listTextActionResource,
                    PermissionStr = permissionStr
                };
            }
            catch (Exception ex)
            {
                return new GetPermissionResult
                {
                    MessageCode = ex.Message,
                    StatusCode = HttpStatusCode.ExpectationFailed
                };
            }
        }

        private List<string> GetActionResource(string code)
        {
            var role = context.Role.FirstOrDefault(c => c.RoleValue == code);
            var listActionResourceId = context.RoleAndPermission.Where(c => c.RoleId == role.RoleId)
                .Select(m => m.ActionResourceId).ToList();
            var lst = context.ActionResource.Where(e => listActionResourceId.Contains(e.ActionResourceId))
                .Select(x => x.ActionResource1).ToList();
            return lst;
        }

        public GetMasterDataProjectMilestoneResult GetMasterDataProjectMilestone(GetMasterDataProjectMilestoneParameter parameter)
        {
            try
            {
                int totalRecordsNote = 0;
                int pageSize = 10;
                int pageIndex = 1;
                var user = context.User.FirstOrDefault(c => c.UserId == parameter.UserId);
                if (user == null)
                {
                    return new GetMasterDataProjectMilestoneResult
                    {
                        MessageCode = "Nhân viên không tồn tại trong hệ thống",
                        StatusCode = HttpStatusCode.ExpectationFailed
                    };
                }

                var employee = context.Employee.FirstOrDefault(c => c.EmployeeId == user.EmployeeId);
                if (employee == null)
                {
                    return new GetMasterDataProjectMilestoneResult
                    {
                        MessageCode = "Nhân viên không tồn tại trong hệ thống",
                        StatusCode = HttpStatusCode.ExpectationFailed
                    };
                }

                var project = context.Project.Where(x => x.ProjectId == parameter.ProjectId).Select(y => new ProjectEntityModel
                {
                    ProjectId = y.ProjectId,
                    ProjectStartDate = y.ProjectStartDate,
                    ProjectEndDate = y.ProjectEndDate,
                    ActualStartDate = y.ActualStartDate,
                    ActualEndDate = y.ActualEndDate,
                    ProjectManagerId = y.ProjectManagerId,
                    ContractId = y.ContractId,
                    ProjectName = y.ProjectName,
                    ProjectCode = y.ProjectCode,
                    BudgetVnd = y.BudgetVnd,
                    BudgetUsd = y.BudgetUsd,
                    BudgetNgayCong = y.BudgetNgayCong,
                    CustomerId = y.CustomerId,
                    Description = y.Description,
                    ProjectSize = y.ProjectSize,
                    ProjectType = y.ProjectType,
                    ProjectStatus = y.ProjectStatus,
                    IncludeWeekend = y.IncludeWeekend,
                    Priority = y.Priority,
                }).FirstOrDefault();

                if (project == null)
                {
                    return new GetMasterDataProjectMilestoneResult
                    {
                        MessageCode = "Dự án không tồn tại trong hệ thống",
                        StatusCode = HttpStatusCode.ExpectationFailed
                    };
                }

                var commonListCategory = context.Category.Where(c => c.Active == true)
                  .Select(m => new CategoryEntityModel
                  {
                      CategoryId = m.CategoryId,
                      CategoryCode = m.CategoryCode,
                      CategoryName = m.CategoryName,
                      CategoryTypeId = m.CategoryTypeId
                  }).ToList();

                project.ProjectTypeName = commonListCategory.FirstOrDefault(c => c.CategoryId == project.ProjectType)?.CategoryName;
                project.ProjectStatusName = commonListCategory.FirstOrDefault(c => c.CategoryId == project.ProjectStatus)?.CategoryName;
                project.ProjectStatusCode = commonListCategory.FirstOrDefault(c => c.CategoryId == project.ProjectStatus)?.CategoryCode;
                switch (project.Priority)
                {
                    case 1:
                        project.PriorityName = "Thấp";
                        break;
                    case 2:
                        project.PriorityName = "Trung bình";
                        break;
                    case 3:
                        project.PriorityName = "Cao";
                        break;
                    default: break;
                }
                CaculatorProjectTask(parameter.ProjectId, out decimal projectComplete, out decimal totalEstimateHour);
                var listUser = context.User.ToList();

                #region Trạng thái công việc
                var typeStatusTaskId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "TTCV")?.CategoryTypeId;
                var listStatus = commonListCategory.Where(c => c.CategoryTypeId == typeStatusTaskId).ToList();
                var inProgressStatusId = listStatus.FirstOrDefault(c => c.CategoryCode == "DL")?.CategoryId;
                var closeStatusId = listStatus.FirstOrDefault(c => c.CategoryCode == "CLOSE")?.CategoryId;
                #endregion

                var listAllTaskOfProject = context.Task.Where(c => c.ProjectId == parameter.ProjectId)
                    .Select(m => new TaskEntityModel
                    {
                        TaskId = m.TaskId,
                        TaskCode = m.TaskCode,
                        TaskName = m.TaskName,
                        TaskTypeId = m.TaskTypeId,
                        Status = m.Status,
                        Priority = m.Priority,
                        PlanEndTime = m.PlanEndTime,
                        MilestonesId = m.MilestonesId,
                        CreateBy = m.CreateBy,
                        CreateDate = m.CreateDate,
                        TaskComplate = m.TaskComplate,
                        EstimateHour = m.EstimateHour
                    }).ToList();

                var listAllMilestoneOfProject = context.ProjectMilestone.Where(c => c.ProjectId == parameter.ProjectId)
                    .Select(m => new ProjectMilestoneEntityModel
                    {
                        ProjectMilestonesId = m.ProjectMilestonesId,
                        ProjectId = m.ProjectId,
                        Name = m.Name,
                        StartTime = m.StartTime,
                        EndTime = m.EndTime,
                        Status = m.Status,
                        Description = m.Description,
                        CreateBy = m.CreateBy,
                        CreateDate = m.CreateDate,
                        UpdateBy = m.UpdateBy,
                        UpdateDate = m.UpdateDate
                    }).ToList();

                listAllMilestoneOfProject.ForEach(item =>
                {
                    var listTaskOfMileStone = listAllTaskOfProject.Where(c => c.MilestonesId == item.ProjectMilestonesId).ToList();
                    item.TaskNumber = listTaskOfMileStone.Count();
                    item.TaskInProgressNumber = listTaskOfMileStone.Where(c => c.Status == inProgressStatusId).Count();
                    item.TaskCloseNumber = listTaskOfMileStone.Where(c => c.Status == closeStatusId).Count();
                    item.CreateByName = listUser.FirstOrDefault(c => c.UserId == item.CreateBy)?.UserName ?? string.Empty;
                    item.ProjectMilestoneComplete = CaculatorProjectMilestoneTask(listTaskOfMileStone, item.ProjectMilestonesId);
                    var dayOfWeek = item.UpdateDate?.DayOfWeek;
                    switch (dayOfWeek)
                    {
                        case DayOfWeek.Monday:
                            item.UpdateTimeStr = $"thứ 2, ngày {item.UpdateDate:dd/MM/yyyy}";
                            break;
                        case DayOfWeek.Tuesday:
                            item.UpdateTimeStr = $"thứ 3, ngày {item.UpdateDate:dd/MM/yyyy}";
                            break;
                        case DayOfWeek.Wednesday:
                            item.UpdateTimeStr = $"thứ 4, ngày {item.UpdateDate:dd/MM/yyyy}";
                            break;
                        case DayOfWeek.Thursday:
                            item.UpdateTimeStr = $"thứ 5, ngày {item.UpdateDate:dd/MM/yyyy}";
                            break;
                        case DayOfWeek.Friday:
                            item.UpdateTimeStr = $"thứ 6, ngày {item.UpdateDate:dd/MM/yyyy}";
                            break;
                        case DayOfWeek.Saturday:
                            item.UpdateTimeStr = $"thứ 7, ngày {item.UpdateDate:dd/MM/yyyy}";
                            break;
                        case DayOfWeek.Sunday:
                            item.UpdateTimeStr = $"chủ nhật, ngày {item.UpdateDate:dd/MM/yyyy}";
                            break;
                    }

                });

                var lstMilestoneInProgress = listAllMilestoneOfProject.Where(c => c.Status == MilestoneStatus.InProgress).OrderBy(c => c.EndTime).ToList();
                var lstMilestoneComplete = listAllMilestoneOfProject.Where(c => c.Status == MilestoneStatus.Complete).OrderBy(c => c.EndTime).ToList();

                #region get list notes
                var listNote = context.Note.Where(w => w.Active == true && w.ObjectId == parameter.ProjectId && w.ObjectType == "MILESTONE").Select(w => new NoteEntityModel
                {
                    NoteId = w.NoteId,
                    Description = w.Description,
                    Type = w.Type,
                    ObjectId = w.ObjectId,
                    ObjectType = w.ObjectType,
                    NoteTitle = w.NoteTitle,
                    CreatedById = w.CreatedById,
                    CreatedDate = w.CreatedDate,
                    UpdatedById = w.UpdatedById,
                    UpdatedDate = w.UpdatedDate,
                    NoteDocList = context.NoteDocument.Where(ws => ws.NoteId == w.NoteId && ws.Active == true).Select(s => new NoteDocumentEntityModel
                    {
                        NoteDocumentId = s.NoteDocumentId,
                        NoteId = s.NoteId,
                        DocumentName = s.DocumentName,
                        DocumentSize = s.DocumentSize,
                        DocumentUrl = s.DocumentUrl,
                    }).ToList() ?? new List<NoteDocumentEntityModel>()
                }).OrderByDescending(c => c.CreatedDate).ToList();

                //lấy tên người tạo, người chỉnh sửa cho note
                listNote.ForEach(note =>
                {
                    var empId = context.User.FirstOrDefault(f => f.UserId == note.CreatedById).EmployeeId;
                    var contact = context.Contact.FirstOrDefault(f => f.ObjectType == "EMP" && f.ObjectId == empId);
                    if (contact != null)
                    {
                        note.ResponsibleName = contact.FirstName + " " + contact.LastName;
                    }
                });
                // Sắp xếp lại listnote
                listNote = listNote.OrderByDescending(x => x.CreatedDate).ToList();
                totalRecordsNote = listNote.Count;

                listNote = listNote
                    .Skip(pageSize * (pageIndex - 1))
                    .Take(pageSize).ToList();
                #endregion

                #region list dự án theo phân quyền user

                var listAllProject = context.Project.ToList();

                if (user != null)
                {
                    if (employee != null)
                    {
                        var positionEmp = context.Position.FirstOrDefault(c => c.PositionId == employee.PositionId);
                        if (positionEmp != null && positionEmp.PositionCode == "GD")
                        {
                            var isRoot = context.Organization.FirstOrDefault(c => c.OrganizationId == employee.OrganizationId).ParentId == null;
                            if (!isRoot)
                            {
                                // Giám đốc được set đơn vị cao nhất trong tổ chức - Get All
                                // Lấy những bản ghi là quản lý, quản lý cấp cao, subPM - trong nguồn lực
                                // Những dự án có trong nguồn lực
                                var listProjectFollowResourceId = context.ProjectResource.Where(c => c.ObjectId == employee.EmployeeId).Select(m => m.ProjectId).ToList();
                                // Những dự án là quản lý, quản lý cấp cao, đồng quản lý
                                var listProjectFollowManagerId = context.ProjectEmployeeMapping.Where(c => c.EmployeeId == employee.EmployeeId).Select(c => c.ProjectId).ToList();

                                var listId = new List<Guid>();
                                listId.AddRange(listProjectFollowResourceId);
                                listId.AddRange(listProjectFollowManagerId);

                                listAllProject = listAllProject.Where(c => listId.Contains(c.ProjectId) || c.ProjectManagerId == employee.EmployeeId || c.CreateBy == user.UserId).ToList();
                            }
                        }
                        else
                        {
                            // Những dự án có trong nguồn lực
                            var listProjectFollowResourceId = context.ProjectResource.Where(c => c.ObjectId == employee.EmployeeId).Select(m => m.ProjectId).ToList();
                            // Những dự án là quản lý, quản lý cấp cao, đồng quản lý
                            var listProjectFollowManagerId = context.ProjectEmployeeMapping.Where(c => c.EmployeeId == employee.EmployeeId).Select(c => c.ProjectId).ToList();

                            var listId = new List<Guid>();
                            listId.AddRange(listProjectFollowResourceId);
                            listId.AddRange(listProjectFollowManagerId);

                            listAllProject = listAllProject.Where(c => listId.Contains(c.ProjectId) || c.ProjectManagerId == employee.EmployeeId || c.CreateBy == user.UserId).ToList();
                        }
                    }

                }

                var listProject = listAllProject
                        .Select(m => new ProjectEntityModel
                        {
                            ProjectId = m.ProjectId,
                            ProjectCode = m.ProjectCode,
                            ProjectName = m.ProjectName,
                            ProjectStatusPlan = m.ProjectStatusPlan,
                        }).ToList();

                #endregion

                return new GetMasterDataProjectMilestoneResult
                {
                    MessageCode = "Success",
                    StatusCode = HttpStatusCode.OK,
                    ListProjectMilestoneInProgress = lstMilestoneInProgress,
                    ListProjectMilestoneComplete = lstMilestoneComplete,
                    TotalEstimateHour = totalEstimateHour,
                    ProjectTaskComplete = projectComplete,
                    Project = project,
                    ListNote = listNote,
                    TotalRecordsNote = totalRecordsNote,
                    ListProject = listProject,
                };
            }
            catch (Exception ex)
            {
                return new GetMasterDataProjectMilestoneResult
                {
                    MessageCode = ex.Message,
                    StatusCode = HttpStatusCode.ExpectationFailed
                };
            }
        }

        public void CaculatorProjectTask(Guid projectId, out decimal projectComplete, out decimal totalEstimateHour)
        {
            var listTask = context.Task.Where(c => c.ProjectId == projectId).ToList();
            var total = 0M;
            var taskComplete = 0M;
            listTask.ForEach(item =>
            {
                taskComplete += (item?.TaskComplate ?? 0) / 100 * (item?.EstimateHour ?? 0);
                total += item?.EstimateHour ?? 0;
            });
            totalEstimateHour = total;

            projectComplete = total != 0 ? taskComplete / total * 100 : 0M;
        }

        public decimal CaculatorProjectMilestoneTask(List<TaskEntityModel> listTask, Guid projetMilestoneId)
        {
            var total = 0M;
            var taskComplete = 0M;
            listTask.ForEach(item =>
            {
                taskComplete += (item?.TaskComplate ?? 0) / 100 * (item?.EstimateHour ?? 0);
                total += item?.EstimateHour ?? 0;
            });

            return total != 0 ? taskComplete / total * 100 : 0M;
        }

        public GetMasterDataCreateOrUpdateMilestoneResult GetMasterDataCreateOrUpdateMilestone(GetMasterDataCreateOrUpdateMilestoneParameter parameter)
        {
            try
            {
                var user = context.User.FirstOrDefault(c => c.UserId == parameter.UserId);
                if (user == null)
                {
                    return new GetMasterDataCreateOrUpdateMilestoneResult
                    {
                        MessageCode = "Nhân viên không tồn tại trong hệ thống",
                        StatusCode = HttpStatusCode.ExpectationFailed
                    };
                }

                var employee = context.Employee.FirstOrDefault(c => c.EmployeeId == user.EmployeeId);
                if (employee == null)
                {
                    return new GetMasterDataCreateOrUpdateMilestoneResult
                    {
                        MessageCode = "Nhân viên không tồn tại trong hệ thống",
                        StatusCode = HttpStatusCode.ExpectationFailed
                    };
                }

                var listProject = context.Project.Select(m => new ProjectEntityModel
                {
                    ProjectId = m.ProjectId,
                    ProjectCode = m.ProjectCode,
                    ProjectName = m.ProjectName,
                    ProjectStartDate = m.ProjectStartDate,
                    ProjectEndDate = m.ProjectEndDate
                }).ToList();
                var milestone = new ProjectMilestoneEntityModel();

                if (parameter.ProjectMilestoneId != null && parameter.ProjectMilestoneId != Guid.Empty)
                {
                    milestone = context.ProjectMilestone.Where(c => c.ProjectMilestonesId == parameter.ProjectMilestoneId)
                        .Select(m => new ProjectMilestoneEntityModel
                        {
                            ProjectMilestonesId = m.ProjectMilestonesId,
                            ProjectId = m.ProjectId,
                            StartTime = m.StartTime,
                            EndTime = m.EndTime,
                            Description = m.Description,
                            Name = m.Name,
                        }).FirstOrDefault();
                }


                return new GetMasterDataCreateOrUpdateMilestoneResult
                {
                    MessageCode = "Success",
                    StatusCode = HttpStatusCode.OK,
                    ListProject = listProject,
                    ProjectMilestone = milestone,
                };
            }
            catch (Exception ex)
            {
                return new GetMasterDataCreateOrUpdateMilestoneResult
                {
                    MessageCode = ex.Message,
                    StatusCode = HttpStatusCode.ExpectationFailed
                };
            }
        }

        public CreateOrUpdateMilestoneResult CreateOrUpdateMilestone(CreateOrUpdateMilestoneParameter parameter)
        {
            try
            {
                var user = context.User.FirstOrDefault(c => c.UserId == parameter.UserId);
                if (user == null)
                {
                    return new CreateOrUpdateMilestoneResult
                    {
                        MessageCode = "Nhân viên không tồn tại trong hệ thống",
                        StatusCode = HttpStatusCode.ExpectationFailed
                    };
                }

                var employee = context.Employee.FirstOrDefault(c => c.EmployeeId == user.EmployeeId);
                if (employee == null)
                {
                    return new CreateOrUpdateMilestoneResult
                    {
                        MessageCode = "Nhân viên không tồn tại trong hệ thống",
                        StatusCode = HttpStatusCode.ExpectationFailed

                    };
                }
                Note note = new Note();
                if (parameter.ProjectMilestone.ProjectMilestonesId == null || parameter.ProjectMilestone.ProjectMilestonesId == Guid.Empty)
                {
                    var milestone = new ProjectMilestone
                    {
                        ProjectMilestonesId = Guid.NewGuid(),
                        ProjectId = parameter.ProjectMilestone.ProjectId,
                        EndTime = parameter.ProjectMilestone.EndTime,
                        Name = parameter.ProjectMilestone.Name,
                        Description = parameter.ProjectMilestone.Description,
                        Status = MilestoneStatus.InProgress,
                        CreateBy = parameter.UserId,
                        CreateDate = DateTime.Now,
                        UpdateBy = parameter.UserId,
                        UpdateDate = DateTime.Now,
                        StartTime = null
                    };
                    context.ProjectMilestone.Add(milestone);

                    note = new Note
                    {
                        NoteId = Guid.NewGuid(),
                        ObjectType = "MILESTONE",
                        ObjectId = parameter.ProjectMilestone.ProjectId,
                        Type = "SYS",
                        Active = true,
                        CreatedById = parameter.UserId,
                        CreatedDate = DateTime.Now,
                        NoteTitle = "Đã thêm ghi chú",
                        Description = "<p><strong>" + user.UserName + "</strong>" + " đã thêm mốc " + parameter.ProjectMilestone.Name + " vào dự án" + "</p>",
                    };
                }
                else
                {
                    var oldMilestone = context.ProjectMilestone.FirstOrDefault(c => c.ProjectMilestonesId == parameter.ProjectMilestone.ProjectMilestonesId);
                    if (oldMilestone == null)
                    {
                        return new CreateOrUpdateMilestoneResult
                        {
                            MessageCode = "Mốc dự án không tồn tại trong hệ thống",
                            StatusCode = HttpStatusCode.ExpectationFailed
                        };
                    };

                    //oldMilestone.ProjectId = parameter.ProjectMilestone.ProjectId;
                    oldMilestone.Name = parameter.ProjectMilestone.Name;
                    oldMilestone.EndTime = parameter.ProjectMilestone.EndTime;
                    oldMilestone.Description = parameter.ProjectMilestone.Description;

                    oldMilestone.UpdateBy = parameter.UserId;
                    oldMilestone.UpdateDate = DateTime.Now;

                    context.ProjectMilestone.Update(oldMilestone);

                    note = new Note
                    {
                        NoteId = Guid.NewGuid(),
                        ObjectType = "MILESTONE",
                        ObjectId = parameter.ProjectMilestone.ProjectId,
                        Type = "SYS",
                        Active = true,
                        CreatedById = parameter.UserId,
                        CreatedDate = DateTime.Now,
                        NoteTitle = "Đã thêm ghi chú",
                        Description = "<p><strong>" + user.UserName + "</strong>" + " đã chỉnh sửa mốc " + parameter.ProjectMilestone.Name + " của dự án" + "</p>",
                    };

                }
                context.Note.Add(note);

                // Update ngày thay đổi vào trường LastChangeActivityDate bảng Project
                var project = context.Project.FirstOrDefault(x => x.ProjectId == parameter.ProjectMilestone.ProjectId);
                if (project != null)
                {
                    project.LastChangeActivityDate = DateTime.Now;
                    context.Project.Update(project);
                }
                context.SaveChanges();

                return new CreateOrUpdateMilestoneResult
                {
                    MessageCode = "Success",
                    StatusCode = HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new CreateOrUpdateMilestoneResult
                {
                    MessageCode = ex.Message,
                    StatusCode = HttpStatusCode.ExpectationFailed
                };
            }
        }

        public UpdateStatusProjectMilestoneResult UpdateStatusProjectMilestone(UpdateStatusProjectMilestoneParameter parameter)
        {
            try
            {
                var user = context.User.FirstOrDefault(c => c.UserId == parameter.UserId);
                if (user == null)
                {
                    return new UpdateStatusProjectMilestoneResult
                    {
                        MessageCode = "Nhân viên không tồn tại trong hệ thống",
                        StatusCode = HttpStatusCode.ExpectationFailed
                    };
                }

                var employee = context.Employee.FirstOrDefault(c => c.EmployeeId == user.EmployeeId);
                if (employee == null)
                {
                    return new UpdateStatusProjectMilestoneResult
                    {
                        MessageCode = "Nhân viên không tồn tại trong hệ thống",
                        StatusCode = HttpStatusCode.ExpectationFailed

                    };
                }

                var projectMilstone = context.ProjectMilestone.FirstOrDefault(c => c.ProjectMilestonesId == parameter.ProjectMilestoneId);
                if (projectMilstone == null)
                {
                    return new UpdateStatusProjectMilestoneResult
                    {
                        MessageCode = "Mốc dự án không tồn tại trong hệ thống",
                        StatusCode = HttpStatusCode.ExpectationFailed
                    };
                }
                // Update ngày thay đổi vào trường LastChangeActivityDate bảng Project
                var project = context.Project.FirstOrDefault(x => x.ProjectId == projectMilstone.ProjectId);
                if (project != null)
                {
                    project.LastChangeActivityDate = DateTime.Now;
                    context.Project.Update(project);
                }
                Note note = new Note();
                // Đóng mốc dự án
                if (parameter.Type == 0)
                {
                    projectMilstone.Status = MilestoneStatus.Complete;
                    projectMilstone.UpdateBy = parameter.UserId;
                    projectMilstone.UpdateDate = DateTime.Now;
                    context.ProjectMilestone.Update(projectMilstone);
                    context.SaveChanges();
                }
                // Mở lại mốc dự án 
                else if (parameter.Type == 1)
                {
                    projectMilstone.Status = MilestoneStatus.InProgress;
                    projectMilstone.UpdateBy = parameter.UserId;
                    projectMilstone.UpdateDate = DateTime.Now;
                    context.ProjectMilestone.Update(projectMilstone);
                    context.SaveChanges();
                }
                // Xóa mốc dự án
                else if (parameter.Type == 2)
                {
                    using (var transaction = context.Database.BeginTransaction())
                    {
                        var listAllTaskOfMilstone = context.Task.Where(c => c.MilestonesId == projectMilstone.ProjectMilestonesId).ToList();
                        listAllTaskOfMilstone.ForEach(item =>
                        {
                            item.MilestonesId = null;
                        });
                        context.Task.UpdateRange(listAllTaskOfMilstone);
                        context.ProjectMilestone.Remove(projectMilstone);

                        note = new Note
                        {
                            NoteId = Guid.NewGuid(),
                            ObjectType = "MILESTONE",
                            ObjectId = projectMilstone.ProjectId,
                            Type = "SYS",
                            Active = true,
                            CreatedById = parameter.UserId,
                            CreatedDate = DateTime.Now,
                            NoteTitle = "Đã thêm ghi chú",
                            Description = "<p><strong>" + user.UserName + "</strong>" + " đã xóa mốc " + projectMilstone.Name + " của dự án" + "</p>",
                        };

                        context.Note.Add(note);
                        context.SaveChanges();
                        transaction.Commit();
                    };
                }

                return new UpdateStatusProjectMilestoneResult
                {
                    MessageCode = "Sucess",
                    StatusCode = HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new UpdateStatusProjectMilestoneResult
                {
                    MessageCode = ex.Message,
                    StatusCode = HttpStatusCode.ExpectationFailed,
                };
            }
        }

        public GetMasterDataAddOrRemoveTaskToMilestoneResult GetMasterDataAddOrRemoveTaskToMilestone(GetMasterDataAddOrRemoveTaskToMilestoneParameter parameter)
        {
            try
            {
                var user = context.User.FirstOrDefault(c => c.UserId == parameter.UserId);
                if (user == null)
                {
                    return new GetMasterDataAddOrRemoveTaskToMilestoneResult
                    {
                        MessageCode = "Nhân viên không tồn tại trong hệ thống",
                        StatusCode = HttpStatusCode.ExpectationFailed
                    };
                }

                var employee = context.Employee.FirstOrDefault(c => c.EmployeeId == user.EmployeeId);
                if (employee == null)
                {
                    return new GetMasterDataAddOrRemoveTaskToMilestoneResult
                    {
                        MessageCode = "Nhân viên không tồn tại trong hệ thống",
                        StatusCode = HttpStatusCode.ExpectationFailed

                    };
                }

                #region Trạng thái công việc
                var typeStatusTaskId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "TTCV")?.CategoryTypeId;
                var listStatus = context.Category.Where(c => c.CategoryTypeId == typeStatusTaskId).ToList();
                var newStatusId = listStatus.FirstOrDefault(c => c.CategoryCode == "NEW")?.CategoryId;
                var inProgressStatusId = listStatus.FirstOrDefault(c => c.CategoryCode == "DL")?.CategoryId;
                #endregion

                #region Loại Công Việc
                var typeTaskId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "LCV")?.CategoryTypeId;
                var listTaskType = context.Category.Where(c => c.CategoryTypeId == typeTaskId).ToList();
                #endregion

                var listAllTask = context.Task.Where(c => c.ProjectId == parameter.ProjectId)
                        .Select(m => new TaskEntityModel
                        {
                            TaskId = m.TaskId,
                            TaskCode = m.TaskCode,
                            TaskName = m.TaskName,
                            TaskTypeId = m.TaskTypeId,
                            Status = m.Status,
                            Priority = m.Priority,
                            PlanEndTime = m.PlanEndTime,
                            MilestonesId = m.MilestonesId
                        }).ToList();

                listAllTask.ForEach(item =>
                {
                    var status = listStatus.FirstOrDefault(c => c.CategoryId == item.Status);
                    item.StatusName = status?.CategoryName;
                    switch (status.CategoryCode)
                    {
                        case "NEW":
                            item.BackgroundColorForStatus = "#0F62FE";
                            item.IsDelete = true;
                            break;
                        case "DL":
                            item.BackgroundColorForStatus = "#FFC000";
                            item.IsDelete = false;
                            break;
                        case "HT":
                            item.BackgroundColorForStatus = "#63B646";
                            item.IsDelete = false;
                            break;
                        case "CLOSE":
                            item.BackgroundColorForStatus = "#9C00FF";
                            item.IsDelete = false;
                            break;
                    }
                    item.TaskTypeName = listTaskType.FirstOrDefault(c => c.CategoryId == item.TaskTypeId)?.CategoryName;
                    switch (item.Priority)
                    {
                        case 0:
                            item.PriorityName = "Thấp";
                            break;
                        case 1:
                            item.PriorityName = "Trung bình";
                            break;
                        case 2:
                            item.PriorityName = "Cao";
                            break;
                        default: break;
                    }

                    if (item.PlanEndTime != null && item.PlanEndTime.Value.Date > DateTime.Now.Date)
                    {
                        item.PlanEndTimeStr = item.PlanEndTime.Value.ToString("dd/MM/yyyy");
                        item.ColorPlanEndTimeStr = "#333333";
                    }
                    else if (item.PlanEndTime != null && item.PlanEndTime.Value.Date < DateTime.Now.Date)
                    {
                        var number = DateTime.Now.Date.Subtract(item.PlanEndTime.Value.Date).TotalDays;
                        item.PlanEndTimeStr = number == 1 ? "Hôm qua" : item.PlanEndTime.Value.ToString("dd/MM/yyyy");
                        item.ColorPlanEndTimeStr = "#323232";
                    }
                    else if (item.PlanEndTime != null && item.PlanEndTime.Value.Date == DateTime.Now.Date)
                    {
                        item.PlanEndTimeStr = "Hôm nay";
                        item.ColorPlanEndTimeStr = "#FD1B3E";
                    }
                });

                var listTask = new List<TaskEntityModel>();
                // Thêm công việc vào mốc
                if (parameter.Type == 0)
                {
                    listTask = listAllTask.Where(c => c.MilestonesId == null && (c.Status == newStatusId || c.Status == inProgressStatusId)).ToList();
                }
                // Xóa công việc ra khỏi mốc
                else if (parameter.Type == 1)
                {
                    listTask = listAllTask.Where(c => c.MilestonesId == parameter.ProjectMilestoneId).ToList();
                }

                return new GetMasterDataAddOrRemoveTaskToMilestoneResult
                {
                    MessageCode = "Sucess",
                    ListTask = listTask,
                    StatusCode = HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new GetMasterDataAddOrRemoveTaskToMilestoneResult
                {
                    MessageCode = ex.Message,
                    StatusCode = HttpStatusCode.ExpectationFailed
                };
            }
        }

        public AddOrRemoveTaskMilestoneResult AddOrRemoveTaskMilestone(AddOrRemoveTaskMilestoneParameter parameter)
        {
            try
            {
                var user = context.User.FirstOrDefault(c => c.UserId == parameter.UserId);
                if (user == null)
                {
                    return new AddOrRemoveTaskMilestoneResult
                    {
                        MessageCode = "Nhân viên không tồn tại trong hệ thống",
                        StatusCode = HttpStatusCode.ExpectationFailed
                    };
                }

                var employee = context.Employee.FirstOrDefault(c => c.EmployeeId == user.EmployeeId);
                if (employee == null)
                {
                    return new AddOrRemoveTaskMilestoneResult
                    {
                        MessageCode = "Nhân viên không tồn tại trong hệ thống",
                        StatusCode = HttpStatusCode.ExpectationFailed

                    };
                }

                var projectMilestone = context.ProjectMilestone.FirstOrDefault(c => c.ProjectMilestonesId == parameter.ProjectMilestoneId);
                if (projectMilestone == null)
                {
                    return new AddOrRemoveTaskMilestoneResult
                    {
                        MessageCode = "Mốc dự án không tồn tại trong hệ thống",
                        StatusCode = HttpStatusCode.ExpectationFailed

                    };
                }
                // Update ngày thay đổi vào trường LastChangeActivityDate bảng Project
                var project = context.Project.FirstOrDefault(x => x.ProjectId == projectMilestone.ProjectId);
                if (project != null)
                {
                    project.LastChangeActivityDate = DateTime.Now;
                    context.Project.Update(project);
                }
                // Thêm công việc vào mốc dự án
                if (parameter.Type == 0)
                {
                    var listTask = context.Task.Where(c => parameter.ListTaskId.Contains(c.TaskId)).ToList();
                    listTask.ForEach(item =>
                    {
                        item.MilestonesId = parameter.ProjectMilestoneId;
                    });

                    projectMilestone.UpdateBy = parameter.UserId;
                    projectMilestone.UpdateDate = DateTime.Now;
                    context.ProjectMilestone.Update(projectMilestone);
                    context.Task.UpdateRange(listTask);
                    context.SaveChanges();
                }
                // Xóa công việc khỏi mốc dự án
                else if (parameter.Type == 1)
                {
                    var listTask = context.Task.Where(c => parameter.ListTaskId.Contains(c.TaskId)).ToList();
                    listTask.ForEach(item =>
                    {
                        item.MilestonesId = null;
                    });
                    projectMilestone.UpdateBy = parameter.UserId;
                    projectMilestone.UpdateDate = DateTime.Now;
                    context.Task.UpdateRange(listTask);
                    context.ProjectMilestone.Update(projectMilestone);
                    context.SaveChanges();
                }
                // cập nhập ngày update của mốc dự án
                projectMilestone.UpdateBy = parameter.UserId;
                projectMilestone.UpdateDate = DateTime.Now;
                context.ProjectMilestone.Update(projectMilestone);
                context.SaveChanges();

                return new AddOrRemoveTaskMilestoneResult
                {
                    MessageCode = "Success",
                    StatusCode = HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new AddOrRemoveTaskMilestoneResult
                {
                    MessageCode = ex.Message,
                    StatusCode = HttpStatusCode.ExpectationFailed
                };
            }
        }

        public GetDataMilestoneByIdResult GetDataMilestoneById(GetDataMilestoneByIdParameter parameter)
        {
            try
            {
                var user = context.User.FirstOrDefault(c => c.UserId == parameter.UserId);
                if (user == null)
                {
                    return new GetDataMilestoneByIdResult
                    {
                        MessageCode = "Nhân viên không tồn tại trong hệ thống",
                        StatusCode = HttpStatusCode.ExpectationFailed
                    };
                }

                var employee = context.Employee.FirstOrDefault(c => c.EmployeeId == user.EmployeeId);
                if (employee == null)
                {
                    return new GetDataMilestoneByIdResult
                    {
                        MessageCode = "Nhân viên không tồn tại trong hệ thống",
                        StatusCode = HttpStatusCode.ExpectationFailed

                    };
                }

                var projectMilestone = context.ProjectMilestone.Where(c => c.ProjectMilestonesId == parameter.ProjectMilestoneId)
                    .Select(m => new ProjectMilestoneEntityModel
                    {
                        ProjectMilestonesId = m.ProjectMilestonesId,
                        ProjectId = m.ProjectId,
                        Name = m.Name,
                        EndTime = m.EndTime,
                        DelayNumber = DateTime.Now.Date.Subtract(m.EndTime.Value.Date).TotalDays,
                        ProjectMilestoneComplete = 0M,
                    }).FirstOrDefault();
                if (projectMilestone == null)
                {
                    return new GetDataMilestoneByIdResult
                    {
                        MessageCode = "Mốc dự án không tồn tại trong hệ thống",
                        StatusCode = HttpStatusCode.ExpectationFailed
                    };
                }

                #region Trạng thái công việc
                var typeStatusTaskId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "TTCV")?.CategoryTypeId;
                var listStatus = context.Category.Where(c => c.CategoryTypeId == typeStatusTaskId)
                    .Select(m => new CategoryEntityModel
                    {
                        CategoryId = m.CategoryId,
                        CategoryCode = m.CategoryCode,
                        CategoryName = m.CategoryName
                    }).ToList();

                var newStatusId = listStatus.FirstOrDefault(c => c.CategoryCode == "NEW")?.CategoryId;
                var inProgressStatusId = listStatus.FirstOrDefault(c => c.CategoryCode == "DL")?.CategoryId;
                var completeStatusId = listStatus.FirstOrDefault(c => c.CategoryCode == "HT")?.CategoryId;
                var closeStatusId = listStatus.FirstOrDefault(c => c.CategoryCode == "CLOSE")?.CategoryId;
                #endregion

                #region Loại Công Việc
                var typeTaskId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "LCV")?.CategoryTypeId;
                var listTaskType = context.Category.Where(c => c.CategoryTypeId == typeTaskId).ToList();
                #endregion

                var listNote = context.Note.Where(c => c.ObjectType == "TAS").ToList();

                #region Nguồn lực
                var commonListEmployee = context.Employee.ToList();
                var typeRoleResource = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "NCNL")?.CategoryTypeId;
                var resourceNoiBoId = context.Category.FirstOrDefault(c => c.CategoryTypeId == typeRoleResource && c.CategoryCode == "NB")?.CategoryId;

                var typeResourceId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "LNL")?.CategoryTypeId;
                var resourceNhanLucId = context.Category.FirstOrDefault(c => c.CategoryTypeId == typeResourceId && c.CategoryCode == "NLC")?.CategoryId;

                var typeVaiTroNguonLucId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "VTN")?.CategoryTypeId;
                var listVaiTroNguonLuc = context.Category.Where(c => c.CategoryTypeId == typeVaiTroNguonLucId).ToList();

                var listResource = context.ProjectResource.Where(c => c.ProjectId == parameter.ProjectId && (c.ResourceRole == resourceNoiBoId || c.ResourceType == resourceNhanLucId))
                    .Select(m => new ProjectResourceEntityModel
                    {
                        ProjectResourceId = m.ProjectResourceId,
                        ProjectId = m.ProjectId,
                        ResourceRole = m.ResourceRole,
                        EmployeeRole = m.EmployeeRole,
                        ObjectId = m.ObjectId,
                        NameResource = string.Empty,
                        StartTime = m.StartTime,
                        EndTime = m.EndTime,

                    }).ToList();

                listResource.ForEach(item =>
                {
                    if (item.ObjectId != Guid.Empty)
                    {
                        var emp = commonListEmployee.FirstOrDefault(c => c.EmployeeId == item.ObjectId);
                        item.NameResource = $"{emp?.EmployeeCode}";
                        item.IsActive = emp?.Active;
                    }
                    item.EmployeeRoleName = listVaiTroNguonLuc.FirstOrDefault(c => c.CategoryId == item.EmployeeRole)?.CategoryName ?? string.Empty;
                });

                var listTaskResourceMapping = context.TaskResourceMapping.ToList();

                var listObjectId = context.ProjectResource.Where(c => c.ProjectId == parameter.ProjectId && c.ResourceRole == resourceNoiBoId)
                    .Select(m => new ProjectResourceEntityModel
                    {
                        ProjectResourceId = m.ProjectResourceId,
                        ProjectId = m.ProjectId,
                        ResourceRole = m.ResourceRole,
                        ObjectId = m.ObjectId,
                        NameResource = string.Empty,
                    }).Select(m => m.ObjectId).ToList();

                var listEmployee = context.Employee.Where(c => c.Active == true && listObjectId.Contains(c.EmployeeId))
                   .Select(m => new EmployeeEntityModel
                   {
                       EmployeeId = m.EmployeeId,
                       EmployeeCode = m.EmployeeCode,
                       EmployeeName = m.EmployeeName
                   }).ToList();
                #endregion

                var listAllTaskOfMilestone = context.Task.Where(c => c.MilestonesId == parameter.ProjectMilestoneId)
                    .Select(m => new TaskEntityModel
                    {
                        TaskId = m.TaskId,
                        TaskCode = m.TaskCode,
                        TaskName = m.TaskName,
                        TaskTypeId = m.TaskTypeId,
                        Status = m.Status,
                        Priority = m.Priority,
                        PlanEndTime = m.PlanEndTime,
                        MilestonesId = m.MilestonesId,
                        NoteNumber = 0,
                        PersionInChargedName = string.Empty,
                        TaskComplate = m.TaskComplate,
                        EstimateHour = m.EstimateHour,
                        ActualStartTime = m.ActualStartTime,
                        ActualEndTime = m.ActualEndTime
                    }).ToList();

                listAllTaskOfMilestone.ForEach(item =>
                {
                    item.NoteNumber = listNote.Where(c => c.ObjectId == item.TaskId).Count();
                    var listResourceId = listTaskResourceMapping.Where(c => c.TaskId == item.TaskId && c.IsPersonInCharge == true).Select(m => m.ResourceId).ToList();
                    item.IsHavePic = listResourceId.Count() != 0;
                    item.PersionInChargedName = string.Join(", ", listResource.Where(c => listResourceId.Contains(c.ProjectResourceId)).Select(m => m.NameResource).ToList());
                });

                var listTaskNew = listAllTaskOfMilestone.Where(c => c.Status == newStatusId).ToList();
                var listTaskInProgress = listAllTaskOfMilestone.Where(c => c.Status == inProgressStatusId).ToList();
                var listTaskComplete = listAllTaskOfMilestone.Where(c => c.Status == completeStatusId).ToList();
                var listTaskClose = listAllTaskOfMilestone.Where(c => c.Status == closeStatusId).ToList();

                projectMilestone.ProjectMilestoneComplete = CaculatorProjectMilestoneTask(listAllTaskOfMilestone, parameter.ProjectId);

                var contain = listEmployee.FirstOrDefault(c => c.EmployeeId == employee.EmployeeId);
                var isContainResource = contain != null;

                return new GetDataMilestoneByIdResult
                {
                    MessageCode = "Success",
                    StatusCode = HttpStatusCode.OK,
                    ListTaskNew = listTaskNew,
                    ListTaskInProgress = listTaskInProgress,
                    ListTaskComplete = listTaskComplete,
                    ListTaskClose = listTaskClose,
                    ListStatus = listStatus,
                    IsContainResource = isContainResource,
                    ProjectMilestone = projectMilestone
                };
            }
            catch (Exception ex)
            {
                return new GetDataMilestoneByIdResult
                {
                    MessageCode = ex.Message,
                    StatusCode = HttpStatusCode.ExpectationFailed
                };
            }
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

        public GetCloneProjectScopeResult GetMasterDataListCloneProjectScope()
        {
            try
            {
                var listAllProjectScope = new List<ProjectScopeModel>();
                var listAllProject = context.Project.Select(y => new ProjectEntityModel
                {
                    ProjectId = y.ProjectId,
                    ProjectCode = y.ProjectCode,
                    ProjectName = y.ProjectName,
                    ProjectStatus = y.ProjectStatus,
                }).OrderByDescending(z => z.UpdateDate).ToList();

                listAllProject.ForEach(project =>
                {
                    var projectScopes = context.ProjectScope.Where(x => x.ProjectId == project.ProjectId).ToList();
                    var listProjectScope = projectScopes.Select(p => new ProjectScopeModel()
                    {
                        ProjectScopeId = p.ProjectScopeId,
                        Description = p.Description,
                        ResourceType = p.ResourceType,
                        ProjectScopeName = p.ProjectScopeName,
                        ProjectScopeCode = p.ProjectScopeCode,
                        TenantId = p.TenantId,
                        ParentId = p.ParentId,
                        ProjectId = p.ProjectId,
                        Level=p.Level
                    }).ToList();
                    listProjectScope = SetTTChildren(listProjectScope);

                    listAllProjectScope.AddRange(listProjectScope);
                });

                return new GetCloneProjectScopeResult
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "",
                    ListProject = listAllProject,
                    ListProjectScope = listAllProjectScope
                };
            }
            catch(Exception e)
            {
                return new GetCloneProjectScopeResult
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message,
                };
            }          
        }


        public CloneProjectScopeResult CloneProjectScope(GetCloneProjectScopeParameter parameter)
        {
            using (var dbcxtransaction = context.Database.BeginTransaction())
            {
                try
                {
                    // Danh sách hạng mục của dự án hiện tại
                    var lstToProjectScope = context.ProjectScope.Where(x => x.ProjectId == parameter.OldProjectId).ToList();
                    // Note đầu tiên của hạng mục dự án hiện tại
                    var firstProjectScopeId = lstToProjectScope.FirstOrDefault(x => x.Level == 0).ProjectScopeId;

                    // Danh sách hạng mục muốn copy sang dự án hiện tại 
                    var projectScopesFrom = context.ProjectScope.Where(x => x.ProjectId == parameter.NewProjectId).ToList();

                    List<ProjectScope> lstFromProjectScopes = new List<ProjectScope>();
                    lstFromProjectScopes = projectScopesFrom.Select(q => new ProjectScope()
                    {
                        ProjectScopeId = q.ProjectScopeId,
                        ProjectId = parameter.OldProjectId,
                        ProjectScopeCode = q.ProjectScopeCode,
                        ProjectScopeName = q.ProjectScopeName,
                        Description = q.Description,
                        CreateDate = q.CreateDate,
                        CreateBy = q.CreateBy,
                        ParentId = q.ParentId,
                        Level = q.Level
                    }).OrderBy(x => x.Level).ToList();

                    List<CloneProjectScopeModel> lstNewOldId = new List<CloneProjectScopeModel>();
                    // Vòng lặp qua các level
                    for (int i = 0; i < lstFromProjectScopes.Count(); i++)
                    {
                        if (i == 0)
                        {
                            var newScopeId = Guid.NewGuid();
                            lstFromProjectScopes.Where(x => x.Level == i).ToList().ForEach(qout =>
                            {
                                // Thêm vào list
                                lstNewOldId.Add(new CloneProjectScopeModel
                                {
                                    NewScopeId = newScopeId,
                                    OldScopeId = qout.ProjectScopeId
                                });

                                qout.ProjectScopeId = newScopeId;
                                qout.ParentId = qout.ParentId;

                            });
                        }
                        else
                        {
                            lstFromProjectScopes.Where(x => x.Level == i).ToList().ForEach(newProScope =>
                            {
                                var newScopeId = Guid.NewGuid();
                                // Thêm vào list
                                lstNewOldId.Add(new CloneProjectScopeModel
                                {
                                    NewScopeId = newScopeId,
                                    OldScopeId = newProScope.ProjectScopeId
                                });

                                newProScope.ProjectScopeId = newScopeId;
                                newProScope.ParentId = lstNewOldId.FirstOrDefault(a => a.OldScopeId == newProScope.ParentId).NewScopeId;
                            });
                        }
                    }

                    // Xóa note đầu tiên
                    lstFromProjectScopes.RemoveAll(x => x.Level == 0);
                    // Cập nhập lại parentId cho level 1 là projectScopeId của dự án hiện tại                      
                    lstFromProjectScopes.ForEach(item =>
                    {
                        if (item.Level == 1)
                            item.ParentId = firstProjectScopeId;
                    });

                    if (lstFromProjectScopes.Count() > 0)
                    {
                        var oldProjectScopeRemove = lstToProjectScope.Where(x => x.Level > 0).ToList();
                        if (oldProjectScopeRemove.Count > 0)
                        {
                            // Xóa hết hạng mục con của dự án CŨ trước khi copy
                            context.ProjectScope.RemoveRange(oldProjectScopeRemove);
                            context.SaveChanges();
                        }
                        // Thêm hạng mục từ dự án mới vào dự án hiện tại
                        context.ProjectScope.AddRange(lstFromProjectScopes);
                    }
                    // Update ngày thay đổi vào trường LastChangeActivityDate bảng Project
                    var project = context.Project.FirstOrDefault(x => x.ProjectId == parameter.NewProjectId);
                    if (project != null)
                    {
                        project.LastChangeActivityDate = DateTime.Now;
                        context.Project.Update(project);
                    }
                    context.SaveChanges();
                    dbcxtransaction.Commit();
                }
                catch (Exception ex)
                {
                    dbcxtransaction.Rollback();
                    return new CloneProjectScopeResult
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = "Copy hạng mục không thành công",
                    };
                }
            }

            return new CloneProjectScopeResult
            {
                StatusCode = HttpStatusCode.OK,
                MessageCode = "Copy hạng mục thành công",
            };
        }

        public GetMasterDataCommonDashboardProjectResult GetMasterDataCommonDashboardProject(GetMasterDataCommonDashboardProjectParameter parameter)
        {
            try
            {
                var totalEE = 0M;
                var user = context.User.FirstOrDefault(c => c.UserId == parameter.UserId);
                if (user == null)
                {
                    return new GetMasterDataCommonDashboardProjectResult
                    {
                        MessageCode = "Nhân viên không tồn tại trong hệ thống",
                        StatusCode = HttpStatusCode.ExpectationFailed
                    };
                }

                var employee = context.Employee.FirstOrDefault(c => c.EmployeeId == user.EmployeeId);
                if (employee == null)
                {
                    return new GetMasterDataCommonDashboardProjectResult
                    {
                        MessageCode = "Nhân viên không tồn tại trong hệ thống",
                        StatusCode = HttpStatusCode.ExpectationFailed
                    };
                }

                #region Dự án
                var project = context.Project.Where(x => x.ProjectId == parameter.ProjectId).Select(y => new ProjectEntityModel
                {
                    ProjectId = y.ProjectId,
                    ProjectStartDate = y.ProjectStartDate,
                    ProjectEndDate = y.ProjectEndDate,
                    ActualStartDate = y.ActualStartDate,
                    ActualEndDate = y.ActualEndDate,
                    ProjectManagerId = y.ProjectManagerId,
                    ContractId = y.ContractId,
                    ProjectName = y.ProjectName,
                    ProjectCode = y.ProjectCode,
                    BudgetVnd = y.BudgetVnd,
                    BudgetUsd = y.BudgetUsd,
                    BudgetNgayCong = y.BudgetNgayCong,
                    CustomerId = y.CustomerId,
                    Description = y.Description,
                    ProjectSize = y.ProjectSize,
                    ProjectType = y.ProjectType,
                    ProjectStatus = y.ProjectStatus,
                    IncludeWeekend = y.IncludeWeekend,
                    Priority = y.Priority,
                }).FirstOrDefault();

                if (project == null)
                {
                    return new GetMasterDataCommonDashboardProjectResult
                    {
                        MessageCode = "Dự án không tồn tại trong hệ thống",
                        StatusCode = HttpStatusCode.ExpectationFailed
                    };
                }
                CaculatorProjectTask(parameter.ProjectId, out decimal projectComplete, out decimal totalEstimateHour);
                #endregion

                var listAllEmployee = context.Employee.ToList();

                #region Trạng thái công việc
                var typeStatusTaskId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "TTCV")?.CategoryTypeId;
                var listStatus = context.Category.Where(c => c.CategoryTypeId == typeStatusTaskId).ToList();
                #endregion

                #region Loại Công Việc
                var typeTaskId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "LCV")?.CategoryTypeId;
                var listTaskType = context.Category.Where(c => c.CategoryTypeId == typeTaskId).ToList();
                #endregion

                #region Trạng thái công việc

                var taskStatusTypeId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "TTCV")?.CategoryTypeId;
                var taskNewAndWorkId = context.Category.Where(c =>
                        c.CategoryTypeId == taskStatusTypeId && (c.CategoryCode == "NEW" || c.CategoryCode == "DL"))
                    .Select(y => y.CategoryId)
                    .ToList();

                #endregion

                // Công việc của dự án
                var listAllTaskOfProject = context.Task.Where(c => c.ProjectId == parameter.ProjectId)
                  .Select(m => new TaskEntityModel
                  {
                      TaskId = m.TaskId,
                      TaskCode = m.TaskCode,
                      TaskName = m.TaskName,
                      PlanStartTime = m.PlanStartTime,
                      PlanEndTime = m.PlanEndTime,
                      EstimateHour = m.EstimateHour,
                      ActualStartTime = m.ActualStartTime,
                      ActualEndTime = m.ActualEndTime,
                      ActualHour = m.ActualHour,
                      Priority = m.Priority,
                      Status = m.Status,
                      TaskTypeId = m.TaskTypeId,
                      BackgroundColorForStatus = string.Empty,
                      CreateBy = m.CreateBy,
                      IncludeWeekend = m.IncludeWeekend,
                  }).ToList();

                var listAllTaskOfProjectId = listAllTaskOfProject.Select(m => m.TaskId).ToList();
                var totalHouseUsed = context.TimeSheet.Where(c => listAllTaskOfProjectId.Contains(c.TaskId)).Sum(m => m.SpentHour);

                // Lấy nguồn lực dự án
                var listAllTaskMapping = context.TaskResourceMapping.ToList();
                var listResourceOfProject = context.ProjectResource.Where(c => c.ProjectId == parameter.ProjectId)
                    .Select(p => new ProjectResourceEntityModel
                    {
                        ProjectResourceId = p.ProjectResourceId,
                        TenantId = p.TenantId,
                        ResourceType = p.ResourceType,
                        IsCreateVendor = p.IsCreateVendor,
                        ResourceRole = p.ResourceRole,
                        ObjectId = p.ObjectId,
                        Allowcation = p.Allowcation,
                        StartTime = p.StartTime,
                        EndTime = p.EndTime,
                        CreateDate = p.CreateDate,
                        EmployeeRole = p.EmployeeRole,
                        IsOverload = p.IsOverload,
                        IncludeWeekend = p.IncludeWeekend,
                        TotalMoney = 0,
                        ChiPhiTheoGio = p.ChiPhiTheoGio //Thuê ngoài
                    }).ToList();

                #region Phân quyền
                var isManager = true;
                var position = context.Position.FirstOrDefault(c => c.PositionId == employee.PositionId);
                if (position.PositionCode != "GD")
                {
                    var projectMananger = context.ProjectEmployeeMapping.FirstOrDefault(c => c.ProjectId == project.ProjectId && c.Type == 1 && c.EmployeeId == employee.EmployeeId);
                    if (projectMananger == null && project.ProjectManagerId != employee.EmployeeId)
                    {
                        var subPm = context.ProjectEmployeeMapping.FirstOrDefault(c => c.ProjectId == project.ProjectId && c.Type == 0 && c.EmployeeId == employee.EmployeeId);
                        // Là nhân viên 
                        if (subPm == null)
                        {
                            isManager = false;
                            var listReourceId = context.ProjectResource.Where(c => c.ProjectId == project.ProjectId && c.ObjectId == employee.EmployeeId).Select(m => m.ProjectResourceId).ToList();
                            var listTaskId = context.TaskResourceMapping.Where(c => listReourceId.Contains(c.ResourceId)).Select(m => m.TaskId).ToList();
                            listAllTaskOfProject = listAllTaskOfProject.Where(c => listTaskId.Contains(c.TaskId) || c.CreateBy == parameter.UserId).ToList();
                        }
                    }
                }
                #endregion

                listAllTaskOfProject = GetPropertyOfListTask(listAllTaskOfProject, listStatus, listTaskType, listAllTaskMapping, listResourceOfProject, listAllEmployee);

                var completeStatusId = listStatus.FirstOrDefault(c => c.CategoryCode == "HT")?.CategoryId;
                var listStatusNew = listStatus.Where(c => c.CategoryCode == "NEW" || c.CategoryCode == "DL").Select(m => m.CategoryId).ToList();
                var listStatusComplete = listStatus.Where(c => c.CategoryCode == "HT" || c.CategoryCode == "CLOSE").Select(m => m.CategoryId).ToList();

                /*Công việc quá hạn: 
                     + Công việc có ngày hoàn thành thực tế > ngày kết thúc dự kiến
                     + Công việc chưa hoàn thành( Mới, Đang thực hiện) nhưng ngày hiện tại > ngày kết thúc dự kiến.
                 */
                var listTaskOverdue = listAllTaskOfProject.Where(c =>
                        (c.PlanEndTime.Value.Date < DateTime.Now.Date && listStatusNew.Contains(c.Status.Value)) ||
                        (c.ActualEndTime != null && c.ActualEndTime.Value.Date > c.PlanEndTime.Value.Date &&
                         listStatusComplete.Contains(c.Status.Value)))
                    .OrderByDescending(c => c.Priority).ThenByDescending(c => c.CreateDate).ToList();

                listTaskOverdue.ForEach(item =>
                {
                    switch (listStatus.FirstOrDefault(x => x.CategoryId == item.Status)?.CategoryCode)
                    {
                        case "NEW":
                            item.Order = 1;
                            break;
                        case "DL":
                            item.Order = 2;
                            break;
                        case "HT":
                            item.Order = 3;
                            break;
                        case "CLOSE":
                            item.Order = 4;
                            break;
                    }
                });

                listTaskOverdue = listTaskOverdue.OrderByDescending(c => c.PlanEndTime).ThenBy(c => c.Order)
                    .ToList();

                /*Công việc cần phải hoàn thành trong ngày : 
                 * Ngày kết thúc dự kiến hoàn thành == ngày hôm nay.
                 */

                var resourceId = context.ProjectResource
                    .Where(x => x.ObjectId == employee.EmployeeId && x.ProjectId == parameter.ProjectId)
                    .Select(y => y.ProjectResourceId)
                    .ToList();

                var listTaskOfResourceId = context.TaskResourceMapping.Where(x => resourceId.Contains(x.ResourceId))
                    .Select(y => y.TaskId)
                    .ToList();

                var listTaskCompleteInDay = listAllTaskOfProject
                    .Where(c => c.PlanEndTime != null && c.Status != null &&
                                c.PlanEndTime.Value.Date == DateTime.Now.Date &&
                                listTaskOfResourceId.Contains(c.TaskId) && taskNewAndWorkId.Contains((Guid) c.Status))
                    .ToList();

                listTaskCompleteInDay.ForEach(item =>
                {
                    switch (listStatus.FirstOrDefault(x => x.CategoryId == item.Status)?.CategoryCode)
                    {
                        case "NEW":
                            item.Order = 1;
                            break;
                        case "DL":
                            item.Order = 2;
                            break;
                        //case "HT":
                        //    item.Order = 3;
                        //    break;
                        //case "CLOSE":
                        //    item.Order = 4;
                        //    break;
                    }
                });

                listTaskCompleteInDay = listTaskCompleteInDay.OrderByDescending(c => c.Priority).ThenBy(c => c.Order).ToList();

                var listEmployee = new List<EmployeeEntityModel>();
                var listChartBudget = new List<ChartBudget>();
                var listResourceId = listResourceOfProject.Select(m => m.ProjectResourceId).ToList();
                var listAllTaskMappingResource = context.TaskResourceMapping
                    .Where(c => listResourceId.Contains(c.ResourceId)).ToList();

                var listResourceOfProjectId = listResourceOfProject.Select(m => m.ObjectId).ToList();

                listEmployee = listAllEmployee.Where(c => listResourceOfProjectId.Contains(c.EmployeeId))
                    .Select(m => new EmployeeEntityModel
                    {
                        EmployeeId = m.EmployeeId,
                        EmployeeCode = m.EmployeeCode,
                        EmployeeName = m.EmployeeName,
                        EmployeeCodeName = $"{m.EmployeeCode ?? string.Empty} - {m.EmployeeName ?? string.Empty}",

                    }).ToList();

                listEmployee.ForEach(item =>
                {
                    var lstReId = listResourceOfProject.Where(c => c.ObjectId == item.EmployeeId)
                        .Select(m => m.ProjectResourceId).ToList();
                    item.ListTaskId = listAllTaskMappingResource.Where(c => lstReId.Contains(c.ResourceId))
                        .Select(m => m.TaskId).ToList();
                });

                #region Ngân sách dự án

                var loaiNguonLuc = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "NCNL");
                var listLoaiNguonLuc = context.Category.Where(x => x.CategoryTypeId == loaiNguonLuc.CategoryTypeId)
                    .ToList();
                var noiBo = listLoaiNguonLuc.FirstOrDefault(x => x.CategoryCode == "NB")?.CategoryId;
                var thueNgoai = listLoaiNguonLuc.FirstOrDefault(x => x.CategoryCode == "TN")?.CategoryId;

                double totalWorkDays = 0;
                var nowDate = DateTime.Now;
                double tongNgayCongThucTe = 0;

                listResourceOfProject.ForEach(resource =>
                {
                    #region Tính ngày công và Ngày công thực tế

                    if (resource.EndTime != null && resource.StartTime != null && resource.ResourceRole == noiBo)
                    {
                        TimeSpan ts = (DateTime)resource.EndTime - (DateTime)resource.StartTime;
                        var tongNgayThu7_ChuNhat = 0;
                        var tinhCuoiTuan = resource.IncludeWeekend ?? false;
                        TinhNgayLamViecHelper.GetWeekendDaysBetween(resource.StartTime.Value.Date,
                            resource.EndTime.Value.Date, tinhCuoiTuan, out int thu7, out int chuNhat);

                        tongNgayThu7_ChuNhat = thu7 + chuNhat;

                        resource.WorkDay = (ts.TotalDays + 1 - tongNgayThu7_ChuNhat) *
                                           (resource.Allowcation == null ? 0 : resource.Allowcation) / 100;

                        #region Tính số ngày làm việc thực tế đến thời điểm hiện tại

                        //Nếu ngày hiện tại nằm trong khoảng phân bổ của nguồn lực
                        if (nowDate.Date >= resource.StartTime.Value.Date && nowDate.Date <= resource.EndTime.Value.Date)
                        {
                            TimeSpan tongNgayLamViec = nowDate - (DateTime)resource.StartTime;
                            var tongNgayCuoiTuan = 0;
                            TinhNgayLamViecHelper.GetWeekendDaysBetween(resource.StartTime.Value.Date,
                                nowDate, tinhCuoiTuan, out int _thu7, out int _chuNhat);

                            tongNgayCuoiTuan = _thu7 + _chuNhat;

                            resource.WorkDayActual = (tongNgayLamViec.TotalDays + 1 - tongNgayCuoiTuan) *
                                                     (resource.Allowcation == null ? 0 : resource.Allowcation) / 100;
                            tongNgayCongThucTe += (resource.WorkDayActual ?? 0);
                        }

                        //Nếu ngày hiện tại lớn hơn ngày cuối cùng phân bổ của nguồn lực
                        if (nowDate.Date > resource.EndTime.Value.Date)
                        {
                            resource.WorkDayActual = resource.WorkDay;
                            tongNgayCongThucTe += (resource.WorkDay ?? 0);
                        }

                        #endregion
                    }
                    else
                    {
                        resource.WorkDay = 0;
                    }

                    #endregion

                    #region Tính lương theo giờ

                    var emp = listAllEmployee.FirstOrDefault(c => c.EmployeeId == resource.ObjectId);

                    //Nếu loại nguồn lực là Nội bộ
                    if (emp != null && resource.ResourceRole == noiBo)
                    {
                        resource.ChiPhiTheoGio = emp.ChiPhiTheoGio;
                    }

                    #endregion

                    if (resource.WorkDay != null && resource.ResourceRole == noiBo) 
                        totalWorkDays += (double) resource.WorkDay;
                });

                if (project.BudgetNgayCong != null && totalWorkDays != 0)
                {
                    totalEE = (decimal)((project.BudgetNgayCong / (decimal?)totalWorkDays) * 100);
                }

                var totalSalary = listResourceOfProject.Sum(c => (c.ChiPhiTheoGio * (decimal)(c.WorkDay ?? 0) * 8));
                var totalHour = listResourceOfProject.Sum(c => (c.WorkDay ?? 0) * 8);
                var avengredSalary = totalHour == 0 ? 0 : (totalSalary / (decimal)totalHour);
                var toltaMoney = (avengredSalary * (project.BudgetNgayCong ?? 0) * 8) + (project.BudgetVnd ?? 0) +
                                 ((project.BudgetUsd ?? 0) * 23000);

                var tongLuongThucTe = listResourceOfProject.Sum(c => c.ChiPhiTheoGio * (decimal)(c.WorkDayActual ?? 0) * 8);
                //var tongNgayLamViecThucTe = listResourceOfProject.Sum(c => (c.WorkDayActual ?? 0) * 8);
                //var luongTrungBinhThucTe =
                //    tongNgayLamViecThucTe == 0 ? 0 : (tongLuongThucTe / (decimal) tongNgayLamViecThucTe);
                //var nganSachThucTe = (luongTrungBinhThucTe * (decimal) tongNgayCongThucTe * 8);

                listChartBudget = new List<ChartBudget>
                {
                    new ChartBudget
                    {
                        BudgetName = "Ngân sách dự kiến",
                        BudgetValue = Math.Round(toltaMoney, 0),
                    },
                    new ChartBudget
                    {
                        BudgetName = "Ngân sách thực tế",
                        BudgetValue = Math.Round(tongLuongThucTe, 0),
                    }
                };

                #endregion

                #region list dự án theo phân quyền user

                var listAllProject = context.Project.ToList();

                if (user != null)
                {
                    if (employee != null)
                    {
                        var positionEmp = context.Position.FirstOrDefault(c => c.PositionId == employee.PositionId);
                        if (positionEmp != null && positionEmp.PositionCode == "GD")
                        {
                            var isRoot = context.Organization
                                             .FirstOrDefault(c => c.OrganizationId == employee.OrganizationId)
                                             .ParentId == null;
                            if (!isRoot)
                            {
                                // Giám đốc được set đơn vị cao nhất trong tổ chức - Get All
                                // Lấy những bản ghi là quản lý, quản lý cấp cao, subPM - trong nguồn lực
                                // Những dự án có trong nguồn lực
                                var listProjectFollowResourceId = context.ProjectResource
                                    .Where(c => c.ObjectId == employee.EmployeeId).Select(m => m.ProjectId).ToList();
                                // Những dự án là quản lý, quản lý cấp cao, đồng quản lý
                                var listProjectFollowManagerId = context.ProjectEmployeeMapping
                                    .Where(c => c.EmployeeId == employee.EmployeeId).Select(c => c.ProjectId).ToList();

                                var listId = new List<Guid>();
                                listId.AddRange(listProjectFollowResourceId);
                                listId.AddRange(listProjectFollowManagerId);

                                listAllProject = listAllProject.Where(c =>
                                    listId.Contains(c.ProjectId) || c.ProjectManagerId == employee.EmployeeId ||
                                    c.CreateBy == user.UserId).ToList();
                            }
                        }
                        else
                        {
                            // Những dự án có trong nguồn lực
                            var listProjectFollowResourceId = context.ProjectResource
                                .Where(c => c.ObjectId == employee.EmployeeId).Select(m => m.ProjectId).ToList();
                            // Những dự án là quản lý, quản lý cấp cao, đồng quản lý
                            var listProjectFollowManagerId = context.ProjectEmployeeMapping
                                .Where(c => c.EmployeeId == employee.EmployeeId).Select(c => c.ProjectId).ToList();

                            var listId = new List<Guid>();
                            listId.AddRange(listProjectFollowResourceId);
                            listId.AddRange(listProjectFollowManagerId);

                            listAllProject = listAllProject.Where(c =>
                                listId.Contains(c.ProjectId) || c.ProjectManagerId == employee.EmployeeId ||
                                c.CreateBy == user.UserId).ToList();
                        }
                    }
                }

                var listProject = listAllProject
                    .Select(m => new ProjectEntityModel
                    {
                        ProjectId = m.ProjectId,
                        ProjectCode = m.ProjectCode,
                        ProjectName = m.ProjectName
                    }).ToList();

                #endregion

                return new GetMasterDataCommonDashboardProjectResult
                {
                    MessageCode = "Success",
                    StatusCode = HttpStatusCode.OK,
                    IsManager = isManager,
                    ProjectTaskComplete = projectComplete,
                    TotalEstimateHour = totalEstimateHour,
                    TotalHourUsed = totalHouseUsed ?? 0,
                    Project = project,
                    ListTaskOverdue = listTaskOverdue,
                    ListProject = listProject,
                    ListEmployee = listEmployee,
                    ListAllTask = listAllTaskOfProject,
                    ListChartBudget = listChartBudget,
                    ListTaskComplete = listTaskCompleteInDay,
                    TotalEE = totalEE,
                };
            }
            catch (Exception ex)
            {
                return new GetMasterDataCommonDashboardProjectResult
                {
                    MessageCode = ex.Message,
                    StatusCode = HttpStatusCode.ExpectationFailed
                };
            }
        }

        private List<TaskEntityModel> GetPropertyOfListTask(List<TaskEntityModel> listTask, List<Category> listStatus, List<Category> listTaskType, List<TaskResourceMapping> listAllTaskMapping,
           List<ProjectResourceEntityModel> listResourceOfProject, List<Employee> listAllEmployee)
        {
            listTask.ForEach(item =>
            {
                var status = listStatus.FirstOrDefault(c => c.CategoryId == item.Status);
                item.StatusName = status?.CategoryName;
                item.StatusCode = status?.CategoryCode;
                switch (status.CategoryCode)
                {
                    case "NEW":
                        item.BackgroundColorForStatus = "#0F62FE";
                        item.IsDelete = true;
                        break;
                    case "DL":
                        item.BackgroundColorForStatus = "#FFC000";
                        item.IsDelete = false;
                        break;
                    case "HT":
                        item.BackgroundColorForStatus = "#63B646";
                        item.IsDelete = false;
                        break;
                    case "CLOSE":
                        item.BackgroundColorForStatus = "#9C00FF";
                        item.IsDelete = false;
                        break;
                }
                var taskType = listTaskType.FirstOrDefault(c => c.CategoryId == item.TaskTypeId);
                item.TaskTypeCode = taskType?.CategoryCode;
                item.TaskTypeName = taskType?.CategoryName;
                switch (item.Priority)
                {
                    case 0:
                        item.PriorityName = "Thấp";
                        break;
                    case 1:
                        item.PriorityName = "Trung bình";
                        break;
                    case 2:
                        item.PriorityName = "Cao";
                        break;
                    default: break;
                }

                var listResourceId = listAllTaskMapping.Where(c => c.TaskId == item.TaskId && c.IsPersonInCharge == true).Select(m => m.ResourceId).ToList();
                // Có người phụ trách
                if (listResourceId.Count != 0)
                {
                    item.IsHavePic = true;
                    var listObjectId = listResourceOfProject.Where(c => listResourceId.Contains(c.ProjectResourceId)).Select(m => m.ObjectId).ToList();
                    var listEmployeeName = listAllEmployee.Where(c => listObjectId.Contains(c.EmployeeId)).Select(m => m.EmployeeName).ToList();
                    item.PersionInChargedName = string.Join(", ", listEmployeeName);
                }
                // Không có người phụ trách
                else
                {
                    item.IsHavePic = false;
                }

                if (item.PlanEndTime != null && item.PlanEndTime.Value.Date > DateTime.Now.Date)
                {
                    item.PlanEndTimeStr = item.PlanEndTime.Value.ToString("dd/MM/yyyy");
                    item.ColorPlanEndTimeStr = "#333333";
                }
                else if (item.PlanEndTime != null && item.PlanEndTime.Value.Date < DateTime.Now.Date)
                {
                    var number = DateTime.Now.Date.Subtract(item.PlanEndTime.Value.Date).TotalDays;
                    item.PlanEndTimeStr = number == 1 ? "Hôm qua" : item.PlanEndTime.Value.ToString("dd/MM/yyyy");
                    item.ColorPlanEndTimeStr = "#fd1b1b";
                }
                else if (item.PlanEndTime != null && item.PlanEndTime.Value.Date == DateTime.Now.Date)
                {
                    item.PlanEndTimeStr = "Hôm nay";
                    item.ColorPlanEndTimeStr = "#FD1B3E";
                }

                if (item.ActualEndTime != null && item.ActualEndTime.Value.Date > DateTime.Now.Date)
                {
                    item.ActualEndTimeStr = item.ActualEndTime.Value.ToString("dd/MM/yyyy");
                    item.ColorActualEndTimeStr = "#333333";
                }
                else if (item.ActualEndTime != null && item.ActualEndTime.Value.Date < DateTime.Now.Date)
                {
                    var number = DateTime.Now.Date.Subtract(item.ActualEndTime.Value.Date).TotalDays;
                    item.ActualEndTimeStr = number == 1 ? "Hôm qua" : item.ActualEndTime.Value.ToString("dd/MM/yyyy");
                    item.ColorActualEndTimeStr = "#fd1b1b";
                }
                else if (item.ActualEndTime != null && item.ActualEndTime.Value.Date == DateTime.Now.Date)
                {
                    item.ActualEndTimeStr = "Hôm nay";
                    item.ColorActualEndTimeStr = "#FD1B3E";
                }

                if (item.CreateDate > DateTime.Now.Date)
                {
                    var dayOfWeek = GetDayOfWeek(item.CreateDate);
                    item.CreateDateStr = $"{dayOfWeek} - {item.CreateDate:dd/MM/yyyy}";
                }
                else if (item.CreateDate < DateTime.Now.Date)
                {
                    var dayOfWeek = GetDayOfWeek(item.CreateDate);
                    item.CreateDateStr = $"{dayOfWeek} - {item.CreateDate:dd/MM/yyyy}";
                }
                else if (item.CreateDate == DateTime.Now.Date)
                {
                    var dayOfWeek = GetDayOfWeek(item.CreateDate);
                    item.CreateDateStr = $"{dayOfWeek} - Hôm nay";
                }
            });

            return listTask;
        }

        private string GetDayOfWeek(DateTime date)
        {
            var str = string.Empty;
            var dayOfWeek = date.DayOfWeek;
            switch (dayOfWeek)
            {
                case DayOfWeek.Monday:
                    str = "Thứ 2";
                    break;
                case DayOfWeek.Tuesday:
                    str = "Thứ 3";
                    break;
                case DayOfWeek.Wednesday:
                    str = "Thứ 4";
                    break;
                case DayOfWeek.Thursday:
                    str = "Thứ 5";
                    break;
                case DayOfWeek.Friday:
                    str = "Thứ 6";
                    break;
                case DayOfWeek.Saturday:
                    str = "Thứ 7";
                    break;
                case DayOfWeek.Sunday:
                    str = "Chủ nhật";
                    break;
            }
            return str;
        }

        public GetDataDashboardProjectFollowManagerResult GetDataDashboardProjectFollowManager(GetDataDashboardProjectFollowManagerParameter parameter)
        {
            try
            {
                #region Trạng thái công việc
                var typeStatusTaskId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "TTCV")?.CategoryTypeId;
                var listStatus = context.Category.Where(c => c.CategoryTypeId == typeStatusTaskId).ToList();
                #endregion

                #region Loại Công Việc
                var typeTaskId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "LCV")?.CategoryTypeId;
                var listTaskType = context.Category.Where(c => c.CategoryTypeId == typeTaskId).ToList();
                #endregion

                #region Nguồn của nguồn lực

                var typeResourceRoleId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "NCNL")?.CategoryTypeId;
                var listResourceRole = context.Category.Where(c => c.CategoryTypeId == typeResourceRoleId).ToList();

                var typeResourceTypeId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "LNL")?.CategoryTypeId;
                var listResourceType = context.Category.Where(c => c.CategoryTypeId == typeResourceTypeId).ToList();
                var nhanLuc = listResourceType.FirstOrDefault(x => x.CategoryCode == "NLC");

                #endregion

                #region Loại nguồn lực

                var listCategoryTypeCodes = new List<string> { "NCNL" };
                var listCategory = context.Category.Where(x =>
                    listCategoryTypeCodes.Contains(x.CategoryType.CategoryTypeCode) && x.Active == true).Select(y =>
                    new CategoryEntityModel
                    {
                        CategoryId = y.CategoryId,
                        CategoryName = y.CategoryName,
                        CategoryCode = y.CategoryCode,
                        CategoryTypeId = Guid.Empty,
                        CreatedById = Guid.Empty,
                        CategoryTypeCode = y.CategoryType.CategoryTypeCode,
                        CountCategoryById = 0
                    }).ToList();
                var listResourceSource = listCategory?.Where(c => c.CategoryTypeCode == "NCNL").ToList();
                var resourceType = listResourceSource.FirstOrDefault(x =>
                    listResourceSource.Select(a => x.CategoryTypeId).Contains(x.CategoryTypeId) &&
                    x.CategoryCode == "NB");

                #endregion

                #region Trạng thái đề xuất xin nghỉ

                var loaiDeXuat = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "LXU");
                var statusNghiPhep = context.Category.FirstOrDefault(ct => ct.CategoryCode.Trim() == "NP" &&
                                                                           ct.CategoryTypeId ==
                                                                           loaiDeXuat.CategoryTypeId)?.CategoryId;
                var statusNghiKhongLuong = context.Category.FirstOrDefault(ct => ct.CategoryCode.Trim() == "NKL" &&
                                                                                 ct.CategoryTypeId ==
                                                                                 loaiDeXuat.CategoryTypeId)?.CategoryId;

                #endregion

                #region Trạng thái time sheet

                var timeSheetType = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "TTKTG");
                var listTimeSheetStatus = context.Category.Where(x => x.CategoryTypeId == timeSheetType.CategoryTypeId)
                    .ToList();
                var statusTimeSheetDaPheDuyet = listTimeSheetStatus.FirstOrDefault(x => x.CategoryCode == "DPD")?.CategoryId;

                #endregion

                #region Lấy thời gian 

                var currentDay = DateTime.Now;
                var firstDayInWeek = DateTime.Now.Date;
                while (firstDayInWeek.DayOfWeek != DayOfWeek.Monday)
                {
                    firstDayInWeek = firstDayInWeek.AddDays(-1);
                }
                var lastDayInWeek = firstDayInWeek.AddDays(6);

                var firstDateInMonth = new DateTime(currentDay.Year, currentDay.Month, 1);
                var lastDateInMonth = firstDateInMonth.AddMonths(1).AddDays(-1);

                int year = DateTime.Now.Year;
                DateTime firstDateInYear = new DateTime(year, 1, 1);
                DateTime lastDateInYear = new DateTime(year, 12, 31);

                #endregion

                var listAllTask = context.Task.Where(c => c.ProjectId == parameter.ProjectId)
                    .Select(m => new TaskEntityModel
                    {
                        TaskId = m.TaskId,
                        Status = m.Status,
                        TaskTypeId = m.TaskTypeId,
                        PlanEndTime = m.PlanEndTime,
                        ActualEndTime = m.ActualEndTime,
                        CreateDate = m.CreateDate,
                        UpdateDate = m.UpdateDate
                    }).ToList();

                switch (parameter.Mode)
                {
                    case "Day":
                        listAllTask = listAllTask.Where(c =>
                            (c.CreateDate.Date == currentDay.Date) ||
                            (c.UpdateDate != null && c.UpdateDate.Value.Date == currentDay.Date)).ToList();
                        break;
                    case "Week":
                        listAllTask = listAllTask.Where(c =>
                            (firstDayInWeek.Date <= c.CreateDate.Date && c.CreateDate.Date <= lastDayInWeek.Date) ||
                            (c.UpdateDate != null && firstDayInWeek.Date <= c.UpdateDate.Value.Date &&
                             c.UpdateDate.Value.Date <= lastDayInWeek.Date)).ToList();
                        break;
                    case "Month":
                        listAllTask = listAllTask.Where(c =>
                            (c.CreateDate.Month == currentDay.Month && c.CreateDate.Year == currentDay.Year) ||
                            (c.UpdateDate != null && c.UpdateDate.Value.Month == currentDay.Month &&
                             c.UpdateDate.Value.Year == currentDay.Year)).ToList();
                        break;
                    case "Year":
                        listAllTask = listAllTask.Where(c =>
                            (c.CreateDate.Year == currentDay.Year) ||
                            (c.UpdateDate != null && c.UpdateDate.Value.Year == currentDay.Year)).ToList();
                        break;
                    default:
                        break;
                }

                listAllTask.ForEach(item =>
                {
                    var status = listStatus.FirstOrDefault(c => c.CategoryId == item.Status);
                    item.StatusCode = status?.CategoryCode;
                    item.StatusName = status?.CategoryName;

                    var taskType = listTaskType.FirstOrDefault(c => c.CategoryId == item.TaskTypeId);
                    item.TaskTypeCode = taskType?.CategoryCode;
                    item.TaskTypeName = taskType?.CategoryName;

                    switch (status.CategoryCode)
                    {
                        case "NEW":
                            item.BackgroundColorForStatus = "#0F62FE";
                            break;
                        case "DL":
                            item.BackgroundColorForStatus = "#FFC000";
                            break;
                        case "HT":
                            item.BackgroundColorForStatus = "#63B646";
                            break;
                        case "CLOSE":
                            item.BackgroundColorForStatus = "#9C00FF";
                            break;
                    }

                    item.Overdue = GetTypeOverdue(item.PlanEndTime, item.ActualEndTime, item.StatusCode);
                });
                var totalTask = listAllTask.Count();
                var listAllTaskId = listAllTask.Select(m => m.TaskId).ToList();

                #region Tổng quan tình trạng công việc
                var listChartFollowStatus = listAllTask.GroupBy(c => c.Status)
                    .Select(m => new ChartTaskFollowStatus
                    {
                        CategoryId = m.Key.Value,
                        CategoryCode = m.First().StatusCode,
                        CategoryName = m.First().StatusName,
                        CountTask = m.Count(),
                        Color = m.First().BackgroundColorForStatus
                    }).ToList();

                listChartFollowStatus.ForEach(item =>
                {
                    item.PercentValue = ((float)item.CountTask / totalTask * 100).ToString("0");
                });
                #endregion

                #region Phân loại công việc

                var listChartFollowTaskType = listAllTask.GroupBy(c => c.TaskTypeId)
                    .Select(m => new CharFollowTaskType
                    {
                        CategoryId = m.Key,
                        CategoryCode = m.First()?.TaskTypeCode,
                        CategoryName = m.First()?.TaskTypeName,
                        CountTask = m.Count(),
                    }).ToList();

                listChartFollowTaskType.ForEach(item =>
                {
                    if (item.CategoryId == null)
                    {
                        item.CategoryName = "Chưa phân loại";
                    }
                });

                #endregion

                #region Tiến độ công việc theo nhân viên

                var listChartTaskFollowResource = new List<ChartTaskFollowResource>();
                // Lấy nguồn lực nội bộ của dự án
                var listResource = context.ProjectResource.Where(c =>
                        c.ProjectId == parameter.ProjectId && c.ResourceRole == resourceType.CategoryId &&
                        c.StartTime != null && c.EndTime != null)
                    .Select(p => new ProjectResourceEntityModel()
                    {
                        ProjectResourceId = p.ProjectResourceId,
                        ResourceType = p.ResourceType,
                        IsCreateVendor = p.IsCreateVendor,
                        ResourceRole = p.ResourceRole,
                        ObjectId = p.ObjectId,
                        Allowcation = p.Allowcation,
                        StartTime = p.StartTime,
                        EndTime = p.EndTime,
                        CreateDate = p.CreateDate,
                        EmployeeRole = p.EmployeeRole,
                        IsOverload = p.IsOverload,
                        IncludeWeekend = p.IncludeWeekend
                    }).ToList();

                var listReourceId = listResource.Select(m => m.ProjectResourceId).ToList();

                // Lấy tất cả nhân viên trong dự án
                var listAllEmployeeIdOfProject = listResource.Select(m => m.ObjectId).ToList();
                var listAllEmployeeOfProject = context.Employee.Where(c => listAllEmployeeIdOfProject.Contains(c.EmployeeId))
                    .Select(m => new
                    {
                        m.EmployeeId,
                        m.EmployeeCode,
                        m.EmployeeName
                    }).ToList();

                // Lấy tất cả các công việc map vs nguồn lực và trạng thái của của công việc
                var listTaskMappingFollowResourceProject = context.TaskResourceMapping.Where(c => listReourceId.Contains(c.ResourceId) && listAllTaskId.Contains(c.TaskId))
                    .Select(m => new
                    {
                        m.TaskId,
                        m.ResourceId,
                        StatusTaskCode = listAllTask.FirstOrDefault(c => c.TaskId == m.TaskId).StatusCode,
                    }).ToList();

                // Đếm số lượng công việc hoàn thành và chưa hoàn thành đối vs từng nguồn lực
                listResource.ForEach(item =>
                {
                    var chartModel = new ChartTaskFollowResource
                    {
                        ResouceId = item.ProjectResourceId,
                        EmployeeId = item.ObjectId,
                        CountTaskComplete = listTaskMappingFollowResourceProject.Where(c =>
                            c.ResourceId == item.ProjectResourceId &&
                            (c.StatusTaskCode == "HT" || c.StatusTaskCode == "CLOSE")).Count(),
                        CountTaskNotComplete = listTaskMappingFollowResourceProject.Where(c =>
                            c.ResourceId == item.ProjectResourceId &&
                            (c.StatusTaskCode == "NEW" || c.StatusTaskCode == "DL")).Count()
                    };
                    listChartTaskFollowResource.Add(chartModel);
                });

                // Group by số lượng công việc theo Id nhân viên ( 1 nhân viên => 2 nguồn lực)
                var listGroupChartTaskFollowResource = listChartTaskFollowResource.GroupBy(c => c.EmployeeId)
                    .Select(m => new ChartTaskFollowResource
                    {
                        EmployeeId = m.Key,
                        CountTaskComplete = m.Sum(c => c.CountTaskComplete),
                        CountTaskNotComplete = m.Sum(c => c.CountTaskNotComplete)
                    }).ToList();

                listGroupChartTaskFollowResource.ForEach(item =>
                {
                    var employee = listAllEmployeeOfProject.FirstOrDefault(c => c.EmployeeId == item.EmployeeId);
                    item.EmployeeCodeName = $"{employee?.EmployeeCode ?? string.Empty}";
                    item.Total = item.CountTaskComplete + item.CountTaskNotComplete;
                });

                listGroupChartTaskFollowResource =
                    listGroupChartTaskFollowResource.OrderByDescending(c => c.Total).ToList();

                #endregion

                #region Tiến độ công việc trong dự án
                var listChartFollowTime = listAllTask.GroupBy(c => c.Overdue)
                   .Select(m => new ChartTaskFollowTime
                   {
                       TimeCode = m.Key,
                       CountTask = m.Count()
                   }).ToList();

                listChartFollowTime.ForEach(item =>
                {
                    switch (item.TimeCode)
                    {
                        case 0:
                            item.Color = "#E30B0B";
                            item.TimeName = "Quá hạn";
                            break;
                        case 1:
                            item.Color = "#70B603";
                            item.TimeName = "Đúng hạn";
                            break;
                        case 2:
                            item.Color = "#8080FF";
                            item.TimeName = "Trước hạn";
                            break;
                    }

                    item.PercentValue = ((float)item.CountTask / totalTask * 100).ToString("0");
                });
                #endregion

                #region Thời gian nhàn rỗi của nhân viên

                var listForEmp = new List<ChartTimeFollowResource>();

                var listDeXuatXinNghi = (from empR in context.EmployeeRequest
                    join status in context.Category on empR.StatusId equals status.CategoryId
                    where listAllEmployeeIdOfProject.Contains(empR.OfferEmployeeId) &&
                          status.CategoryCode.Trim() == "Approved"
                    select empR).OrderByDescending(o => o.RequestDate).ToList();

                var listAllTimeSheet = context.TimeSheet.Where(c => listAllTaskId.Contains(c.TaskId) &&
                                                                    c.Status == statusTimeSheetDaPheDuyet)
                    .ToList();
                var listTimeSheetIdForProject = listAllTimeSheet.Select(y => y.TimeSheetId).ToList();
                var listPhanBo = new List<ProjectResourceEntityModel>();

                //Ngày
                if (parameter.Mode == "Day")
                {
                    #region Lấy thời gian sử dụng thực tế theo ngày hiện tại

                    var listTimeSheetDetail = context.TimeSheetDetail.Where(y => y.Date != null && 
                                                                                 y.Date.Value.Date == currentDay.Date)
                        .ToList();

                    var listTimeSheetId = listTimeSheetDetail.Select(y => y.TimeSheetId).Distinct().ToList();
                    var listTimeSheet = context.TimeSheet.Where(x => listTimeSheetId.Contains(x.TimeSheetId) &&
                                                                     listTimeSheetIdForProject.Contains(x.TimeSheetId))
                        .ToList();
                    var listEmpIdForTimeSheet = listTimeSheet.Select(y => y.PersonInChargeId).ToList();
                    var listEmpForTimeSheet = context.Employee.Where(x => listEmpIdForTimeSheet.Contains(x.EmployeeId))
                        .Select(y => new {y.EmployeeId, y.EmployeeCode, y.EmployeeName})
                        .ToList();

                    var listActualHourForEmp = new List<ChartTimeFollowResource>();
                    listEmpForTimeSheet.ForEach(item =>
                    {
                        var _listTimeSheetId = listTimeSheet.Where(x => x.PersonInChargeId == item.EmployeeId)
                            .Select(y => y.TimeSheetId).ToList();
                        var _listTimeSheetDetail = listTimeSheetDetail
                            .Where(x => _listTimeSheetId.Contains(x.TimeSheetId)).ToList();

                        var totalActualHour = _listTimeSheetDetail.Sum(s => (s.SpentHour ?? 0));

                        var newEmp = new ChartTimeFollowResource();
                        newEmp.EmployeeId = item.EmployeeId;
                        newEmp.EmployeeCodeName = item.EmployeeCode + " - " + item.EmployeeName;
                        newEmp.TotalHour = 0;
                        newEmp.HourNotUsed = 0;
                        newEmp.HourUsed = totalActualHour;

                        listActualHourForEmp.Add(newEmp);
                    });

                    #endregion

                    #region Lấy thời gian được phân bổ trung bình theo ngày của nhân viên được phân bổ trong dự án

                    var listPlanHourForEmp = new List<ChartTimeFollowResource>();
                    listResource.ForEach(nguonLuc =>
                    {
                        //Lấy các nguồn lực có khoảng phân bổ chứa ngày hiện tại
                        if (nguonLuc.StartTime.Value.Date <= currentDay.Date &&
                            nguonLuc.EndTime.Value.Date >= currentDay.Date)
                        {
                            var listDeXuatXinNghiTheoNhanVien =
                                listDeXuatXinNghi.Where(x => x.OfferEmployeeId == nguonLuc.ObjectId).ToList();

                            TinhNgayLamViecHelper.GetSoNgayNghiPhep(
                                nguonLuc.StartTime.Value, nguonLuc.EndTime.Value,
                                statusNghiPhep.Value, statusNghiKhongLuong.Value,
                                listDeXuatXinNghiTheoNhanVien,
                                out decimal soNgayNghiPhep, out decimal soNgayNghiKhongLuong);

                            TinhNgayLamViecHelper.GetWeekendDaysBetween(nguonLuc.StartTime.Value,
                                nguonLuc.EndTime.Value,
                                nguonLuc.IncludeWeekend ?? false, out int soNgayThuBay, out int soNgayChuNhat);
                            var tongSoNgayCuoiTuan = soNgayThuBay + soNgayChuNhat;

                            decimal tongSoNgayNghiPhep = soNgayNghiPhep + soNgayNghiKhongLuong;

                            decimal TongSoNgay =
                                (decimal) (nguonLuc.EndTime.Value.Date - nguonLuc.StartTime.Value.Date).TotalDays + 1 -
                                tongSoNgayNghiPhep - tongSoNgayCuoiTuan;

                            var emp = listAllEmployeeOfProject.FirstOrDefault(x => x.EmployeeId == nguonLuc.ObjectId);

                            var newEmp = new ChartTimeFollowResource();
                            newEmp.EmployeeId = emp.EmployeeId;
                            newEmp.EmployeeCodeName = emp.EmployeeCode + emp.EmployeeName;
                            newEmp.HourNotUsed = 0;
                            newEmp.HourUsed = 0;
                            newEmp.TotalHour =
                                Math.Round((TongSoNgay * (nguonLuc.Allowcation ?? 0) / (TongSoNgay * 100)) * 8, 2);

                            listPlanHourForEmp.Add(newEmp);
                        }
                    });

                    //Nhóm lại theo nhân viên
                    listPlanHourForEmp = listPlanHourForEmp.GroupBy(g => new {g.EmployeeId, g.EmployeeCodeName})
                        .Select(y => new ChartTimeFollowResource
                        {
                            EmployeeId = y.Key.EmployeeId,
                            EmployeeCodeName = y.Key.EmployeeCodeName,
                            HourNotUsed = 0,
                            HourUsed = 0,
                            TotalHour = y.Sum(s => s.TotalHour)
                        }).ToList();

                    #endregion

                    #region Gộp 2 list nhân viên
                    
                    listForEmp.AddRange(listActualHourForEmp);
                    listForEmp.AddRange(listPlanHourForEmp);

                    listForEmp = listForEmp.GroupBy(g => new {g.EmployeeId, g.EmployeeCodeName})
                        .Select(y => new ChartTimeFollowResource
                        {
                            EmployeeId = y.Key.EmployeeId,
                            EmployeeCodeName = y.Key.EmployeeCodeName,
                            HourNotUsed = y.Sum(s => s.TotalHour) - y.Sum(s => s.HourUsed),
                            HourUsed = y.Sum(s => s.HourUsed),
                            TotalHour = y.Sum(s => s.TotalHour)
                        }).ToList();

                    #endregion
                }
                //Tuần
                else if (parameter.Mode == "Week")
                {
                    #region Lấy thời gian sử dụng thực tế theo tuần hiện tại

                    var listTimeSheetDetail = context.TimeSheetDetail.Where(y => y.Date != null &&
                                                                                 y.Date.Value.Date >= firstDayInWeek.Date &&
                                                                                 y.Date.Value.Date <= lastDayInWeek.Date)
                        .ToList();

                    var listTimeSheetId = listTimeSheetDetail.Select(y => y.TimeSheetId).Distinct().ToList();
                    var listTimeSheet = context.TimeSheet.Where(x => listTimeSheetId.Contains(x.TimeSheetId) &&
                                                                     listTimeSheetIdForProject.Contains(x.TimeSheetId))
                        .ToList();
                    var listEmpIdForTimeSheet = listTimeSheet.Select(y => y.PersonInChargeId).ToList();
                    var listEmpForTimeSheet = context.Employee.Where(x => listEmpIdForTimeSheet.Contains(x.EmployeeId))
                        .Select(y => new { y.EmployeeId, y.EmployeeCode, y.EmployeeName })
                        .ToList();

                    var listActualHourForEmp = new List<ChartTimeFollowResource>();
                    listEmpForTimeSheet.ForEach(item =>
                    {
                        var _listTimeSheetId = listTimeSheet.Where(x => x.PersonInChargeId == item.EmployeeId)
                            .Select(y => y.TimeSheetId).ToList();
                        var _listTimeSheetDetail = listTimeSheetDetail
                            .Where(x => _listTimeSheetId.Contains(x.TimeSheetId)).ToList();

                        var totalActualHour = _listTimeSheetDetail.Sum(s => (s.SpentHour ?? 0));

                        var newEmp = new ChartTimeFollowResource();
                        newEmp.EmployeeId = item.EmployeeId;
                        newEmp.EmployeeCodeName = item.EmployeeCode + " - " + item.EmployeeName;
                        newEmp.TotalHour = 0;
                        newEmp.HourNotUsed = 0;
                        newEmp.HourUsed = totalActualHour;

                        listActualHourForEmp.Add(newEmp);
                    });

                    #endregion

                    #region Lấy thời gian được phân bổ trung bình theo ngày của nhân viên được phân bổ trong dự án

                    var listPlanHourForEmp = new List<ChartTimeFollowResource>();
                    listResource.ForEach(nguonLuc =>
                    {
                        //Lấy các nguồn lực có khoảng phân bổ nằm trong tuần hiện tại
                        GetFirstAndLastDateForBeetwenTime(firstDayInWeek, lastDayInWeek, nguonLuc.StartTime.Value,
                            nguonLuc.EndTime.Value, out DateTime? startTime, out DateTime? endTime);

                        if (startTime != null && endTime != null)
                        {
                            var listDeXuatXinNghiTheoNhanVien =
                                listDeXuatXinNghi.Where(x => x.OfferEmployeeId == nguonLuc.ObjectId).ToList();

                            TinhNgayLamViecHelper.GetSoNgayNghiPhep(
                                startTime.Value, endTime.Value,
                                statusNghiPhep.Value, statusNghiKhongLuong.Value,
                                listDeXuatXinNghiTheoNhanVien,
                                out decimal soNgayNghiPhep, out decimal soNgayNghiKhongLuong);

                            TinhNgayLamViecHelper.GetWeekendDaysBetween(startTime.Value,
                                endTime.Value,
                                nguonLuc.IncludeWeekend ?? false, out int soNgayThuBay, out int soNgayChuNhat);
                            var tongSoNgayCuoiTuan = soNgayThuBay + soNgayChuNhat;

                            decimal tongSoNgayNghiPhep = soNgayNghiPhep + soNgayNghiKhongLuong;

                            decimal TongSoNgay =
                                (decimal)(endTime.Value.Date - startTime.Value.Date).TotalDays + 1 -
                                tongSoNgayNghiPhep - tongSoNgayCuoiTuan;

                            var emp = listAllEmployeeOfProject.FirstOrDefault(x => x.EmployeeId == nguonLuc.ObjectId);

                            var newEmp = new ChartTimeFollowResource();
                            newEmp.EmployeeId = emp.EmployeeId;
                            newEmp.EmployeeCodeName = emp.EmployeeCode + " - " + emp.EmployeeName;
                            newEmp.HourNotUsed = 0;
                            newEmp.HourUsed = 0;
                            newEmp.TotalHour =
                                Math.Round((TongSoNgay * (nguonLuc.Allowcation ?? 0) / (TongSoNgay * 100)) * 8, 2);

                            listPlanHourForEmp.Add(newEmp);
                        }
                    });

                    //Nhóm lại theo nhân viên
                    listPlanHourForEmp = listPlanHourForEmp.GroupBy(g => new { g.EmployeeId, g.EmployeeCodeName })
                        .Select(y => new ChartTimeFollowResource
                        {
                            EmployeeId = y.Key.EmployeeId,
                            EmployeeCodeName = y.Key.EmployeeCodeName,
                            HourNotUsed = 0,
                            HourUsed = 0,
                            TotalHour = y.Sum(s => s.TotalHour)
                        }).ToList();

                    #endregion

                    #region Gộp 2 list nhân viên

                    listForEmp.AddRange(listActualHourForEmp);
                    listForEmp.AddRange(listPlanHourForEmp);

                    listForEmp = listForEmp.GroupBy(g => new { g.EmployeeId, g.EmployeeCodeName })
                        .Select(y => new ChartTimeFollowResource
                        {
                            EmployeeId = y.Key.EmployeeId,
                            EmployeeCodeName = y.Key.EmployeeCodeName,
                            HourNotUsed = y.Sum(s => s.TotalHour) - y.Sum(s => s.HourUsed),
                            HourUsed = y.Sum(s => s.HourUsed),
                            TotalHour = y.Sum(s => s.TotalHour)
                        }).ToList();

                    #endregion
                }
                //Tháng
                else if (parameter.Mode == "Month")
                {
                    #region Lấy thời gian sử dụng thực tế theo tuần hiện tại

                    var listTimeSheetDetail = context.TimeSheetDetail.Where(y => y.Date != null &&
                                                                                 y.Date.Value.Date >= firstDateInMonth.Date &&
                                                                                 y.Date.Value.Date <= lastDateInMonth.Date)
                        .ToList();

                    var listTimeSheetId = listTimeSheetDetail.Select(y => y.TimeSheetId).Distinct().ToList();
                    var listTimeSheet = context.TimeSheet.Where(x => listTimeSheetId.Contains(x.TimeSheetId) &&
                                                                     listTimeSheetIdForProject.Contains(x.TimeSheetId))
                        .ToList();
                    var listEmpIdForTimeSheet = listTimeSheet.Select(y => y.PersonInChargeId).ToList();
                    var listEmpForTimeSheet = context.Employee.Where(x => listEmpIdForTimeSheet.Contains(x.EmployeeId))
                        .Select(y => new { y.EmployeeId, y.EmployeeCode, y.EmployeeName })
                        .ToList();

                    var listActualHourForEmp = new List<ChartTimeFollowResource>();
                    listEmpForTimeSheet.ForEach(item =>
                    {
                        var _listTimeSheetId = listTimeSheet.Where(x => x.PersonInChargeId == item.EmployeeId)
                            .Select(y => y.TimeSheetId).ToList();
                        var _listTimeSheetDetail = listTimeSheetDetail
                            .Where(x => _listTimeSheetId.Contains(x.TimeSheetId)).ToList();

                        var totalActualHour = _listTimeSheetDetail.Sum(s => (s.SpentHour ?? 0));

                        var newEmp = new ChartTimeFollowResource();
                        newEmp.EmployeeId = item.EmployeeId;
                        newEmp.EmployeeCodeName = item.EmployeeCode + " - " + item.EmployeeName;
                        newEmp.TotalHour = 0;
                        newEmp.HourNotUsed = 0;
                        newEmp.HourUsed = totalActualHour;

                        listActualHourForEmp.Add(newEmp);
                    });

                    #endregion

                    #region Lấy thời gian được phân bổ trung bình theo ngày của nhân viên được phân bổ trong dự án

                    var listPlanHourForEmp = new List<ChartTimeFollowResource>();
                    listResource.ForEach(nguonLuc =>
                    {
                        //Lấy các nguồn lực có khoảng phân bổ nằm trong tháng hiện tại
                        GetFirstAndLastDateForBeetwenTime(firstDateInMonth, lastDateInMonth, nguonLuc.StartTime.Value,
                            nguonLuc.EndTime.Value, out DateTime? startTime, out DateTime? endTime);

                        if (startTime != null && endTime != null)
                        {
                            var listDeXuatXinNghiTheoNhanVien =
                                listDeXuatXinNghi.Where(x => x.OfferEmployeeId == nguonLuc.ObjectId).ToList();

                            TinhNgayLamViecHelper.GetSoNgayNghiPhep(
                                startTime.Value, endTime.Value,
                                statusNghiPhep.Value, statusNghiKhongLuong.Value,
                                listDeXuatXinNghiTheoNhanVien,
                                out decimal soNgayNghiPhep, out decimal soNgayNghiKhongLuong);

                            TinhNgayLamViecHelper.GetWeekendDaysBetween(startTime.Value,
                                endTime.Value,
                                nguonLuc.IncludeWeekend ?? false, out int soNgayThuBay, out int soNgayChuNhat);
                            var tongSoNgayCuoiTuan = soNgayThuBay + soNgayChuNhat;

                            decimal tongSoNgayNghiPhep = soNgayNghiPhep + soNgayNghiKhongLuong;

                            decimal TongSoNgay =
                                (decimal)(endTime.Value.Date - startTime.Value.Date).TotalDays + 1 -
                                tongSoNgayNghiPhep - tongSoNgayCuoiTuan;

                            var emp = listAllEmployeeOfProject.FirstOrDefault(x => x.EmployeeId == nguonLuc.ObjectId);

                            var newEmp = new ChartTimeFollowResource();
                            newEmp.EmployeeId = emp.EmployeeId;
                            newEmp.EmployeeCodeName = emp.EmployeeCode + " - " + emp.EmployeeName;
                            newEmp.HourNotUsed = 0;
                            newEmp.HourUsed = 0;
                            newEmp.TotalHour =
                                Math.Round((TongSoNgay * (nguonLuc.Allowcation ?? 0) / (TongSoNgay * 100)) * 8, 2);

                            listPlanHourForEmp.Add(newEmp);
                        }
                    });

                    //Nhóm lại theo nhân viên
                    listPlanHourForEmp = listPlanHourForEmp.GroupBy(g => new { g.EmployeeId, g.EmployeeCodeName })
                        .Select(y => new ChartTimeFollowResource
                        {
                            EmployeeId = y.Key.EmployeeId,
                            EmployeeCodeName = y.Key.EmployeeCodeName,
                            HourNotUsed = 0,
                            HourUsed = 0,
                            TotalHour = y.Sum(s => s.TotalHour)
                        }).ToList();

                    #endregion

                    #region Gộp 2 list nhân viên

                    listForEmp.AddRange(listActualHourForEmp);
                    listForEmp.AddRange(listPlanHourForEmp);

                    listForEmp = listForEmp.GroupBy(g => new { g.EmployeeId, g.EmployeeCodeName })
                        .Select(y => new ChartTimeFollowResource
                        {
                            EmployeeId = y.Key.EmployeeId,
                            EmployeeCodeName = y.Key.EmployeeCodeName,
                            HourNotUsed = y.Sum(s => s.TotalHour) - y.Sum(s => s.HourUsed),
                            HourUsed = y.Sum(s => s.HourUsed),
                            TotalHour = y.Sum(s => s.TotalHour)
                        }).ToList();

                    #endregion
                }
                //Năm
                else if (parameter.Mode == "Year")
                {
                    #region Lấy thời gian sử dụng thực tế theo tuần hiện tại

                    var listTimeSheetDetail = context.TimeSheetDetail.Where(y => y.Date != null &&
                                                                                 y.Date.Value.Date >= firstDateInYear.Date &&
                                                                                 y.Date.Value.Date <= lastDateInYear.Date)
                        .ToList();

                    var listTimeSheetId = listTimeSheetDetail.Select(y => y.TimeSheetId).Distinct().ToList();
                    var listTimeSheet = context.TimeSheet.Where(x => listTimeSheetId.Contains(x.TimeSheetId) &&
                                                                     listTimeSheetIdForProject.Contains(x.TimeSheetId))
                        .ToList();
                    var listEmpIdForTimeSheet = listTimeSheet.Select(y => y.PersonInChargeId).ToList();
                    var listEmpForTimeSheet = context.Employee.Where(x => listEmpIdForTimeSheet.Contains(x.EmployeeId))
                        .Select(y => new { y.EmployeeId, y.EmployeeCode, y.EmployeeName })
                        .ToList();

                    var listActualHourForEmp = new List<ChartTimeFollowResource>();
                    listEmpForTimeSheet.ForEach(item =>
                    {
                        var _listTimeSheetId = listTimeSheet.Where(x => x.PersonInChargeId == item.EmployeeId)
                            .Select(y => y.TimeSheetId).ToList();
                        var _listTimeSheetDetail = listTimeSheetDetail
                            .Where(x => _listTimeSheetId.Contains(x.TimeSheetId)).ToList();

                        var totalActualHour = _listTimeSheetDetail.Sum(s => (s.SpentHour ?? 0));

                        var newEmp = new ChartTimeFollowResource();
                        newEmp.EmployeeId = item.EmployeeId;
                        newEmp.EmployeeCodeName = item.EmployeeCode + " - " + item.EmployeeName;
                        newEmp.TotalHour = 0;
                        newEmp.HourNotUsed = 0;
                        newEmp.HourUsed = totalActualHour;

                        listActualHourForEmp.Add(newEmp);
                    });

                    #endregion

                    #region Lấy thời gian được phân bổ trung bình theo ngày của nhân viên được phân bổ trong dự án

                    var listPlanHourForEmp = new List<ChartTimeFollowResource>();
                    listResource.ForEach(nguonLuc =>
                    {
                        //Lấy các nguồn lực có khoảng phân bổ nằm trong tháng hiện tại
                        GetFirstAndLastDateForBeetwenTime(firstDateInYear, lastDateInYear, nguonLuc.StartTime.Value,
                            nguonLuc.EndTime.Value, out DateTime? startTime, out DateTime? endTime);

                        if (startTime != null && endTime != null)
                        {
                            var listDeXuatXinNghiTheoNhanVien =
                                listDeXuatXinNghi.Where(x => x.OfferEmployeeId == nguonLuc.ObjectId).ToList();

                            TinhNgayLamViecHelper.GetSoNgayNghiPhep(
                                startTime.Value, endTime.Value,
                                statusNghiPhep.Value, statusNghiKhongLuong.Value,
                                listDeXuatXinNghiTheoNhanVien,
                                out decimal soNgayNghiPhep, out decimal soNgayNghiKhongLuong);

                            TinhNgayLamViecHelper.GetWeekendDaysBetween(startTime.Value,
                                endTime.Value,
                                nguonLuc.IncludeWeekend ?? false, out int soNgayThuBay, out int soNgayChuNhat);
                            var tongSoNgayCuoiTuan = soNgayThuBay + soNgayChuNhat;

                            decimal tongSoNgayNghiPhep = soNgayNghiPhep + soNgayNghiKhongLuong;

                            decimal TongSoNgay =
                                (decimal)(endTime.Value.Date - startTime.Value.Date).TotalDays + 1 -
                                tongSoNgayNghiPhep - tongSoNgayCuoiTuan;

                            var emp = listAllEmployeeOfProject.FirstOrDefault(x => x.EmployeeId == nguonLuc.ObjectId);

                            var newEmp = new ChartTimeFollowResource();
                            newEmp.EmployeeId = emp.EmployeeId;
                            newEmp.EmployeeCodeName = emp.EmployeeCode + " - " + emp.EmployeeName;
                            newEmp.HourNotUsed = 0;
                            newEmp.HourUsed = 0;
                            newEmp.TotalHour =
                                Math.Round((TongSoNgay * (nguonLuc.Allowcation ?? 0) / (TongSoNgay * 100)) * 8, 2);

                            listPlanHourForEmp.Add(newEmp);
                        }
                    });

                    //Nhóm lại theo nhân viên
                    listPlanHourForEmp = listPlanHourForEmp.GroupBy(g => new { g.EmployeeId, g.EmployeeCodeName })
                        .Select(y => new ChartTimeFollowResource
                        {
                            EmployeeId = y.Key.EmployeeId,
                            EmployeeCodeName = y.Key.EmployeeCodeName,
                            HourNotUsed = 0,
                            HourUsed = 0,
                            TotalHour = y.Sum(s => s.TotalHour)
                        }).ToList();

                    #endregion

                    #region Gộp 2 list nhân viên

                    listForEmp.AddRange(listActualHourForEmp);
                    listForEmp.AddRange(listPlanHourForEmp);

                    listForEmp = listForEmp.GroupBy(g => new { g.EmployeeId, g.EmployeeCodeName })
                        .Select(y => new ChartTimeFollowResource
                        {
                            EmployeeId = y.Key.EmployeeId,
                            EmployeeCodeName = y.Key.EmployeeCodeName,
                            HourNotUsed = y.Sum(s => s.TotalHour) - y.Sum(s => s.HourUsed),
                            HourUsed = y.Sum(s => s.HourUsed),
                            TotalHour = y.Sum(s => s.TotalHour)
                        }).ToList();

                    #endregion
                }

                listForEmp = listForEmp.OrderByDescending(c => c.HourNotUsed).ToList();

                #region Giang comment: Code không đúng yêu cầu

                //listResource.ForEach(resource =>
                //{
                //    // Lấy thông tin ngày nghỉ được phê duyệt
                //    var absentPermissionId = context.Category.FirstOrDefault(ct => ct.CategoryCode.Trim() == "NP")
                //        ?.CategoryId;
                //    var absentWithoutPermissionId =
                //        context.Category.FirstOrDefault(ct => ct.CategoryCode.Trim() == "NKL")?.CategoryId;
                //    var _empRequest = (from empR in context.EmployeeRequest
                //                       join stt in context.Category on empR.StatusId equals stt.CategoryId
                //                       where empR.OfferEmployeeId == resource.ObjectId &&
                //                             empR.RequestDate.Value.Year == DateTime.Now.Year && stt.CategoryCode.Trim() == "Approved"
                //                       select empR).OrderByDescending(o => o.RequestDate).ToList();
                //    double amountAbsentWithPermission = 0;
                //    double amountAbsentWithoutPermission = 0;

                //    // Danh sách ngày nghỉ
                //    _empRequest.ForEach(empR =>
                //    {
                //        for (var date = empR.StartDate;
                //            (date <= empR.EnDate && date >= resource.StartTime && date <= resource.EndTime);
                //            date = date.Value.AddDays(1))
                //        {
                //            if (empR.TypeRequest == absentPermissionId)
                //            {
                //                if (empR.StartTypeTime == empR.EndTypeTime)
                //                {
                //                    amountAbsentWithPermission += 0.5;
                //                }
                //                else
                //                {
                //                    amountAbsentWithPermission++;
                //                }
                //            }

                //            if (empR.TypeRequest == absentWithoutPermissionId)
                //            {
                //                if (empR.StartTypeTime == empR.EndTypeTime)
                //                {
                //                    amountAbsentWithoutPermission += 0.5;
                //                }
                //                else
                //                {
                //                    amountAbsentWithoutPermission++;
                //                }
                //            }
                //        }
                //    });

                //    // Nội bộ hay thuê ngoài
                //    var resourceRole = listResourceRole.FirstOrDefault(r => r.CategoryId == resource.ResourceRole);
                //    if (resourceRole?.CategoryCode == "NB")
                //    {
                //        // Ngày công
                //        if (resource.EndTime != null && resource.StartTime != null)
                //        {
                //            TimeSpan ts = (DateTime)resource.EndTime - (DateTime)resource.StartTime;
                //            var numberWeeken = TotalHoliday(resource.StartTime.Value, resource.EndTime.Value,
                //                resource.IncludeWeekend ?? false);
                //            resource.WorkDay =
                //                ((ts.TotalDays + 1 - amountAbsentWithPermission - amountAbsentWithoutPermission -
                //                  numberWeeken) * (resource.Allowcation == null ? 0 : resource.Allowcation)) / 100;
                //        }
                //    }
                //});

                //var listChartTimeFollowResource = listResource.GroupBy(c => c.ObjectId)
                //    .Select(m => new ChartTimeFollowResource
                //    {
                //        EmployeeId = m.Key.Value,
                //        EmployeeCodeName = listAllEmployeeOfProject.FirstOrDefault(c => c.EmployeeId == m.Key)
                //            .EmployeeCode,
                //        // TotalHour = (decimal)(m.First().WorkDay ?? 0) * 8,
                //        TotalHour = (decimal)m.Sum(c => c.WorkDay ?? 0) * 8,
                //        HourUsed = listAllTimeSheet == null || listAllTimeSheet.Count() == 0
                //            ? 0
                //            : listAllTimeSheet.Where(c => c.PersonInChargeId == m.Key).Sum(c => (c.SpentHour ?? 0.0M)),
                //        HourNotUsed = 0.0M,
                //    }).ToList();

                //listChartTimeFollowResource.ForEach(item =>
                //{
                //    item.HourNotUsed = item.TotalHour - item.HourUsed;
                //});

                //listChartTimeFollowResource = listChartTimeFollowResource.OrderByDescending(c => c.HourNotUsed).ToList();

                #endregion

                #endregion

                return new GetDataDashboardProjectFollowManagerResult
                {
                    MessageCode = "Success",
                    StatusCode = HttpStatusCode.OK,
                    ListTaskFollowStatus = listChartFollowStatus,
                    ListTaskFollowTime = listChartFollowTime,
                    ListTaskFollowTaskType = listChartFollowTaskType,
                    ListTaskFollowResource = listGroupChartTaskFollowResource,
                    ListChartTimeFollowResource = listForEmp
                };
            }
            catch (Exception ex)
            {
                return new GetDataDashboardProjectFollowManagerResult
                {
                    MessageCode = ex.Message,
                    StatusCode = HttpStatusCode.ExpectationFailed
                };
            }
        }

        public GetDataDashboardProjectFollowEmployeeResult getDataDashboardProjectFollowEmployee(
            GetDataDashboardProjectFollowEmployeeParameter parameter)
        {
            try
            {
                var employeeId = context.User.FirstOrDefault(x => x.UserId == parameter.UserId)?.EmployeeId;

                var projectResourceId = context.ProjectResource.FirstOrDefault(x =>
                    x.ProjectId == parameter.ProjectId && x.ObjectId == employeeId)?.ProjectResourceId;

                var listTaskId = context.TaskResourceMapping.Where(x => x.ResourceId == projectResourceId)
                    .Select(y => y.TaskId).ToList();

                #region Trạng thái công việc

                var typeStatusTaskId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "TTCV")?.CategoryTypeId;
                var listStatus = context.Category.Where(c => c.CategoryTypeId == typeStatusTaskId).ToList();

                #endregion

                #region Loại Công Việc

                var typeTaskId = context.CategoryType.FirstOrDefault(c => c.CategoryTypeCode == "LCV")?.CategoryTypeId;
                var listTaskType = context.Category.Where(c => c.CategoryTypeId == typeTaskId).ToList();

                #endregion

                #region Lấy thời gian 

                var currentDay = DateTime.Now;
                var firstDayInWeek = DateTime.Now.Date;
                while (firstDayInWeek.DayOfWeek != DayOfWeek.Monday)
                {
                    firstDayInWeek = firstDayInWeek.AddDays(-1);
                }
                var lastDayOfWeek = firstDayInWeek.AddDays(6);

                #endregion

                var listAllTask = context.Task.Where(c => c.ProjectId == parameter.ProjectId && listTaskId.Contains(c.TaskId))
                    .Select(m => new TaskEntityModel
                    {
                        TaskId = m.TaskId,
                        Status = m.Status,
                        TaskTypeId = m.TaskTypeId,
                        PlanEndTime = m.PlanEndTime,
                        ActualEndTime = m.ActualEndTime,
                        CreateDate = m.CreateDate,
                        UpdateDate = m.UpdateDate
                    }).ToList();

                var listProjectResource = context.ProjectResource.Where(x => x.ObjectId == employeeId)
                    .Select(m => new ProjectResourceEntityModel
                    {
                        ProjectResourceId = m.ProjectResourceId,
                        ObjectId = m.ObjectId,
                        ProjectId = m.ProjectId,
                        StartTime = m.StartTime,
                        EndTime = m.EndTime,
                        Allowcation = m.Allowcation,
                    }).ToList();

                switch (parameter.Mode)
                {
                    case "Day":

                        listAllTask = listAllTask.Where(c =>
                            (c.CreateDate.Date == currentDay.Date) ||
                            (c.UpdateDate != null && c.UpdateDate.Value.Date == currentDay.Date)).ToList();

                        listProjectResource = listProjectResource.Where(c =>
                            (c.StartTime != null && c.EndTime != null && c.StartTime.Value.Date <= currentDay.Date &&
                             currentDay.Date <= c.EndTime.Value.Date)).ToList();

                        break;
                    case "Week":

                        listAllTask = listAllTask.Where(c =>
                            (firstDayInWeek.Date <= c.CreateDate.Date && c.CreateDate.Date <= lastDayOfWeek.Date) ||
                            (c.UpdateDate != null && firstDayInWeek.Date <= c.UpdateDate.Value.Date &&
                             c.UpdateDate.Value.Date <= lastDayOfWeek.Date)).ToList();

                        listProjectResource = listProjectResource.Where(c =>
                                ((c.StartTime != null && c.EndTime != null &&
                                  ((c.StartTime.Value.Date <= firstDayInWeek.Date &&
                                    firstDayInWeek.Date <= c.EndTime.Value.Date) ||
                                   (firstDayInWeek.Date <= c.StartTime.Value.Date &&
                                    c.StartTime.Value.Date <= lastDayOfWeek.Date))) &&
                                 (c.StartTime != null && c.EndTime != null && ((c.StartTime.Value.Date <=
                                     lastDayOfWeek.Date && lastDayOfWeek.Date <= c.EndTime.Value.Date) || (firstDayInWeek.Date <=
                                     c.EndTime.Value.Date && c.EndTime.Value.Date <= lastDayOfWeek.Date)))))
                            .ToList();

                        break;
                    case "Month":

                        listAllTask = listAllTask.Where(c =>
                            (c.CreateDate.Month == currentDay.Month && c.CreateDate.Year == currentDay.Year) ||
                            (c.UpdateDate != null && c.UpdateDate.Value.Month == currentDay.Month &&
                             c.UpdateDate.Value.Year == currentDay.Year)).ToList();

                        listProjectResource = listProjectResource.Where(c =>
                                (c.StartTime != null && c.EndTime != null && c.StartTime.Value.Month <= currentDay.Month && currentDay.Month <= c.EndTime.Value.Month))
                            .ToList();

                        break;
                    case "Year":
                        listAllTask = listAllTask.Where(c =>
                            (c.CreateDate.Year == currentDay.Year) ||
                            (c.UpdateDate != null && c.UpdateDate.Value.Year == currentDay.Year)).ToList();

                        listProjectResource = listProjectResource.Where(c =>
                                (c.StartTime != null && c.EndTime != null && c.StartTime.Value.Year <= currentDay.Year && currentDay.Year <= c.EndTime.Value.Year))
                            .ToList();

                        break;
                    default:
                        break;
                }

                listAllTask.ForEach(item =>
                {
                    var status = listStatus.FirstOrDefault(c => c.CategoryId == item.Status);
                    item.StatusCode = status?.CategoryCode;
                    item.StatusName = status?.CategoryName;

                    var taskType = listTaskType.FirstOrDefault(c => c.CategoryId == item.TaskTypeId);
                    item.TaskTypeCode = taskType?.CategoryCode;
                    item.TaskTypeName = taskType?.CategoryName;

                    switch (status.CategoryCode)
                    {
                        case "NEW":
                            item.BackgroundColorForStatus = "#0F62FE";
                            break;
                        case "DL":
                            item.BackgroundColorForStatus = "#FFC000";
                            break;
                        case "HT":
                            item.BackgroundColorForStatus = "#63B646";
                            break;
                        case "CLOSE":
                            item.BackgroundColorForStatus = "#9C00FF";
                            break;
                    }

                    item.Overdue = GetTypeOverdue(item.PlanEndTime, item.ActualEndTime, item.StatusCode);
                });
                var totalTask = listAllTask.Count();
                var listAllTaskId = listAllTask.Select(m => m.TaskId).ToList();


                #region Tiến độ công việc trong dự án

                var listChartFollowTime = listAllTask.GroupBy(c => c.Overdue)
                    .Select(m => new ChartTaskFollowTime
                    {
                        TimeCode = m.Key,
                        CountTask = m.Count()
                    }).ToList();

                listChartFollowTime.ForEach(item =>
                {
                    switch (item.TimeCode)
                    {
                        case 0:
                            item.Color = "#E30B0B";
                            item.TimeName = "Quá hạn";
                            break;
                        case 1:
                            item.Color = "#70B603";
                            item.TimeName = "Đúng hạn";
                            break;
                        case 2:
                            item.Color = "#8080FF";
                            item.TimeName = "Trước hạn";
                            break;
                    }

                    item.PercentValue = ((float)item.CountTask / totalTask * 100).ToString("0");
                });

                #endregion


                #region Các dự án được giao

                listProjectResource.ForEach(item =>
                {
                    item.ProjectName = context.Project.FirstOrDefault(x => x.ProjectId == item.ProjectId)?.ProjectName;
                    item.ProjectCode = context.Project.FirstOrDefault(x => x.ProjectId == item.ProjectId)?.ProjectCode;
                });

                var listProjectFollowResource = listProjectResource.GroupBy(c => c.ProjectCode)
                    .Select(item => new ChartProjectFollowResource
                    {
                        Allowcation = item.Sum(x => x.Allowcation),
                        // ProjectName = item.ProjectName,
                        ProjectCode = item.Key,
                    }).ToList();

                #endregion

                return new GetDataDashboardProjectFollowEmployeeResult
                {
                    MessageCode = "Success",
                    StatusCode = HttpStatusCode.OK,
                    ListTaskFollowTime = listChartFollowTime,
                    ListProjectFollowResource = listProjectFollowResource,
                };
            }
            catch (Exception e)
            {
                return new GetDataDashboardProjectFollowEmployeeResult
                {
                    MessageCode = e.Message,
                    StatusCode = HttpStatusCode.ExpectationFailed,
                };
            }
        }

        /// <summary>
        /// Kiểm tra tiến độ công việc trong dự án
        /// </summary>
        /// <param name="planEndTime"> Thời gian kết dự kiến </param>
        /// <param name="actualEndTime"> Thời gian kết thúc thức tế </param>
        /// <param name="statusCode"> Status code của dự án </param>
        /// <returns> 0 - Quá hạn </returns>
        /// <returns> 1 - Đúng hạn </returns>
        /// <returns> 2 - Hoàn thành trước hạn </returns>
        /// <returns> -1 - Không nằm trong ba th trên </returns>
        private int GetTypeOverdue(DateTime? planEndTime, DateTime? actualEndTime, string statusCode)
        {
            var listStatusCodeInProgress = new List<string> { "NEW", "DL" };
            var listStatusCodeResolved = new List<string> { "HT", "CLOSE" };

            if ((actualEndTime != null && planEndTime != null && actualEndTime.Value.Date > planEndTime.Value.Date && listStatusCodeResolved.Contains(statusCode)) ||
                (planEndTime != null && planEndTime.Value.Date < DateTime.Now.Date && listStatusCodeInProgress.Contains(statusCode)))
            {
                return 0;
            }
            else if ((actualEndTime != null && planEndTime != null && listStatusCodeResolved.Contains(statusCode) && actualEndTime.Value.Date == planEndTime.Value.Date) ||
                     (planEndTime != null && planEndTime.Value.Date >= DateTime.Now.Date && listStatusCodeInProgress.Contains(statusCode)))
            {
                return 1;
            }
            else if (actualEndTime != null && planEndTime != null && listStatusCodeResolved.Contains(statusCode) && actualEndTime.Value.Date < planEndTime.Value.Date)
            {
                return 2;
            }
            else
            {
                return -1;
            }
        }

        public GetDataEVNProjectDashboardResult GetGetDataEVNProjectDashboard(GetDataEVNProjectDashboardParameter parameter)
        {
            try
            {
                var project = context.Project.FirstOrDefault(c => c.ProjectId == parameter.ProjectId);
                if (project == null)
                {
                    return new GetDataEVNProjectDashboardResult
                    {
                        MessageCode = "Dự án không tồn tại trong hệ thống",
                        StatusCode = HttpStatusCode.ExpectationFailed
                    };
                }

                var listProjectCost = context.ProjectCostReport.Where(c => c.ProjectId == parameter.ProjectId)
                    .OrderByDescending(y => y.Date).ToList();

                var listChartEvn = new List<ChartEvn>();
                switch (parameter.Mode)
                {
                    case "Day":
                        listChartEvn = listProjectCost
                            .Select(m => new ChartEvn
                            {
                                DateStr = m.Date.ToString("dd/MM/yyyy"),
                                AC = listProjectCost.Where(x => x.Date.Date <= m.Date.Date).Sum(s => s.Ac),
                                PV = m.Pv,
                                EV = m.Ev,
                                DateOrder = m.Date.Date
                            }).OrderBy(c => c.DateOrder).ToList();
                        break;
                    case "Week":
                        listChartEvn = listProjectCost
                            .GroupBy(g => new
                            {
                                Year = g.Date.Year,
                                GetWeekOfYear = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(g.Date.Date,
                                    CalendarWeekRule.FirstDay, DayOfWeek.Monday)
                            }).Select(m => new ChartEvn
                            {
                                DateStr = $"Tuần {m.Key.GetWeekOfYear} - {m.Key.Year}",
                                AC = listProjectCost.Where(x =>
                                        x.Date.Date <= m.OrderByDescending(z => z.Date.Date).FirstOrDefault().Date.Date)
                                    .Sum(s => s.Ac),
                                PV = m.OrderByDescending(z => z.Date.Date).FirstOrDefault().Pv,
                                EV = m.OrderByDescending(z => z.Date.Date).FirstOrDefault().Ev,
                                Year = m.Key.Year,
                                WeekOfYear = m.Key.GetWeekOfYear
                            }).OrderBy(z => z.Year).ThenBy(z => z.WeekOfYear).ToList();
                        break;
                    case "Month":
                        listChartEvn = listProjectCost.GroupBy(c => new { c.Date.Year, c.Date.Month })
                            .Select(m => new ChartEvn
                            {
                                DateStr = $"Tháng {m.Key.Month} - {m.Key.Year}",
                                AC = listProjectCost.Where(x =>
                                        x.Date.Date <= m.OrderByDescending(z => z.Date.Date).FirstOrDefault().Date.Date)
                                    .Sum(s => s.Ac),
                                PV = m.OrderByDescending(z => z.Date.Date).FirstOrDefault().Pv,
                                EV = m.OrderByDescending(z => z.Date.Date).FirstOrDefault().Ev,
                                DateOrder = new DateTime(m.Key.Year, m.Key.Month, 1)
                            }).OrderBy(c => c.DateOrder).ToList();
                        break;
                    case "Year":
                        listChartEvn = listProjectCost.GroupBy(c => c.Date.Year)
                            .Select(m => new ChartEvn
                            {
                                DateStr = $"Năm {m.Key}",
                                AC = listProjectCost.Where(x =>
                                        x.Date.Date <= m.OrderByDescending(z => z.Date.Date).FirstOrDefault().Date.Date)
                                    .Sum(s => s.Ac),
                                PV = m.OrderByDescending(z => z.Date.Date).FirstOrDefault().Pv,
                                EV = m.OrderByDescending(z => z.Date.Date).FirstOrDefault().Ev,
                                SortOrder = m.Key
                            }).OrderBy(c => c.SortOrder).ToList();
                        break;
                    default:
                        break;
                }

                var lstChartPerformanceCost = new List<PerformanceCost>();
                listChartEvn.ForEach(item =>
                {
                    var performanceCost = new PerformanceCost
                    {
                        DateStr = item.DateStr,
                        CPI = item.AC != 0 ? Math.Round((item.EV ?? 0) / (item.AC ?? 0), 2) : 0,
                        SPI = item.PV != 0 ? Math.Round((item.EV ?? 0) / (item.PV ?? 0), 2) : 0,
                    };
                    //if (performanceCost.CPI > performanceCost.SPI)
                    //{
                    //    performanceCost.MaxYAxis = performanceCost.CPI + 5;
                    //}
                    //else
                    //{
                    //    performanceCost.MaxYAxis = performanceCost.SPI + 5;
                    //}
                    lstChartPerformanceCost.Add(performanceCost);
                });

                return new GetDataEVNProjectDashboardResult
                {
                    MessageCode = "Success",
                    StatusCode = HttpStatusCode.OK,
                    ListChartEvn = listChartEvn,
                    ListPerformanceCost = lstChartPerformanceCost
                };
            }
            catch (Exception ex)
            {
                return new GetDataEVNProjectDashboardResult
                {
                    MessageCode = ex.Message,
                    StatusCode = HttpStatusCode.ExpectationFailed
                };
            }
        }

        public SynchronizedEvnResult SynchronizedEvn(SynchronizedEvnParameter parameter)
        {
            try
            {
                #region Common Data
                var commonListCategoryType = context.CategoryType.ToList();
                var commonListCategory = context.Category.ToList();

                var typeStatusProjectId = commonListCategoryType.FirstOrDefault(c => c.CategoryTypeCode == "DAT").CategoryTypeId;
                // Tất cả các trạng thái khác Đóng
                var listStatusProjectId = commonListCategory.Where(c => typeStatusProjectId == c.CategoryTypeId && c.CategoryCode != "DON").Select(m => m.CategoryId).ToList();

                #region Trạng thái công việc
                var typeStatusTaskId = commonListCategoryType.First(c => c.CategoryTypeCode == "TTCV")?.CategoryTypeId;
                var listTaskStatus = commonListCategory.Where(c => typeStatusTaskId == c.CategoryTypeId).ToList();
                #endregion

                // Lấy tất cả dự án của trạng thái khác Đóng
                var listAllProject = context.Project.Where(c => listStatusProjectId.Contains(c.ProjectStatus.Value)).ToList();

                var listAllTask = context.Task.ToList();
                var listAllEmployee = context.Employee.ToList();
                var listAllProjectResource = context.ProjectResource.ToList();
                var listAllTaskResourceMapping = context.TaskResourceMapping.ToList();

                var currentDate = DateTime.Now.Date;

                // Đề xuất xin nghỉ - Nghỉ Phép
                var absentPermissionId = commonListCategory.FirstOrDefault(ct => ct.CategoryCode.Trim() == "NP")?.CategoryId;
                // Đề xuất xin nghỉ - Nghỉ Không Phép
                var absentWithoutPermissionId = commonListCategory.FirstOrDefault(ct => ct.CategoryCode.Trim() == "NKL")?.CategoryId;

                var caSangId = commonListCategory.FirstOrDefault(c => c.CategoryCode == "SAN")?.CategoryId;
                var caChieuId = commonListCategory.FirstOrDefault(c => c.CategoryCode == "CHI")?.CategoryId;

                var typeTrangThaiDeXuatPheDuyet = commonListCategoryType.FirstOrDefault(c => c.CategoryTypeCode == "DDU")?.CategoryTypeId;
                var daPheDuyetDeXuatId = commonListCategory.FirstOrDefault(c => typeTrangThaiDeXuatPheDuyet == c.CategoryTypeId && c.CategoryCode == "Approved")?.CategoryId;
                #endregion

                var listProjectCostReport = new List<ProjectCostReport>();

                listAllProject.ForEach(item =>
                {
                    var listTaskOfProject = listAllTask.Where(c => c.ProjectId == item.ProjectId).
                        Select(m => new TaskEntityModel
                        {
                            TaskId = m.TaskId,
                            EstimateHour = m.EstimateHour,
                            ActualHour = m.ActualHour,
                            Description = m.Description,
                            Status = m.Status,
                            IncludeWeekend = m.IncludeWeekend,
                            TaskComplate = m.TaskComplate,
                            ProjectId = m.ProjectId,
                            StatusCode = listTaskStatus.FirstOrDefault(c => c.CategoryId == m.Status)?.CategoryCode
                        }).ToList();

                    var listResourceOfProject = listAllProjectResource.Where(c => c.ProjectId == item.ProjectId)
                        .Select(p => new ProjectResourceEntityModel()
                        {
                            ProjectResourceId = p.ProjectResourceId,
                            TenantId = p.TenantId,
                            ResourceType = p.ResourceType,
                            IsCreateVendor = p.IsCreateVendor,
                            ResourceRole = p.ResourceRole,
                            ObjectId = p.ObjectId,
                            Allowcation = p.Allowcation,
                            StartTime = p.StartTime,
                            EndTime = p.EndTime,
                            CreateDate = p.CreateDate,
                            EmployeeRole = p.EmployeeRole,
                            IsOverload = p.IsOverload,
                            IncludeWeekend = p.IncludeWeekend,
                        }).ToList();


                    var listResourceOfProjectId = listResourceOfProject.Select(m => m.ProjectResourceId).ToList();
                    var listObjectIdOfProject = listResourceOfProject.Select(m => m.ObjectId).ToList();

                    var listEmployeeRequest = context.EmployeeRequest.Where(c => listObjectIdOfProject.Contains(c.OfferEmployeeId) && daPheDuyetDeXuatId == c.StatusId.Value).ToList();

                    // Tính số ngày nghỉ
                    // listResourceOfProject.ForEach(resource =>
                    // {
                    //     double amountAbsentWithoutPermission = 0;
                    //     if (resource.EndTime != null && resource.StartTime != null)
                    //     {
                    //         TimeSpan ts = (DateTime)resource.EndTime - (DateTime)resource.StartTime;
                    //         var numberWeeken = TotalHoliday(resource.StartTime.Value, resource.EndTime.Value, resource.IncludeWeekend ?? false);
                    //
                    //         var listEmployeeRequetsOfResource = listEmployeeRequest.Where(c => c.OfferEmployeeId == resource.ObjectId).ToList();
                    //
                    //         listEmployeeRequetsOfResource.ForEach(empRe =>
                    //         {
                    //             if (absentWithoutPermissionId == empRe.TypeRequest.Value && resource.EndTime.Value.Date <= currentDate)
                    //             {
                    //                 for (var date = empRe.StartDate.Value.Date; (date <= empRe.EnDate.Value.Date && date >= resource.StartTime.Value.Date
                    //                 && date <= DateTime.Now.Date && date <= resource.EndTime.Value.Date); date = date.AddDays(1))
                    //                 {
                    //
                    //                     if (empRe.StartTypeTime == empRe.EndTypeTime)
                    //                     {
                    //                         amountAbsentWithoutPermission += 0.5;
                    //                     }
                    //                     else
                    //                     {
                    //                         amountAbsentWithoutPermission++;
                    //                     }
                    //                 }
                    //             }
                    //         });
                    //
                    //         resource.WorkDay = (ts.TotalDays + 1 - amountAbsentWithoutPermission - numberWeeken) * (resource.Allowcation == null ? 0 : resource.Allowcation) / 100;
                    //     }
                    //     else
                    //     {
                    //         resource.WorkDay = 0;
                    //     }
                    // });

                    var listEmployeeOfProject = listAllEmployee.Where(c => listObjectIdOfProject.Contains(c.EmployeeId)).ToList();

                    #region Tính AC (theo nhân viên)

                    // var listTaskMappingResource = listAllTaskResourceMapping.Where(c => listResourceOfProjectId.Contains(c.ResourceId))
                    //     .Select(m => new
                    //     {
                    //         m.TaskId,
                    //         m.ResourceId,
                    //         EmployeeId = listResourceOfProject.FirstOrDefault(c => c.ProjectResourceId == m.ResourceId)?.ObjectId,
                    //         listResourceOfProject.FirstOrDefault(c => c.ProjectResourceId == m.ResourceId)?.AllowcateUntilNow,
                    //         listTaskOfProject.FirstOrDefault(c => c.TaskId == m.TaskId)?.EstimateHour,
                    //     }).ToList();
                    //
                    // var listGroupByTaskMappingResource = listTaskMappingResource.GroupBy(c => c.EmployeeId)
                    //      .Select(m => new
                    //      {
                    //          EmployeeId = m.Key,
                    //          TotalEstimateHour = m.Sum(c => c.EstimateHour),
                    //          TotalWorkDay = m.Sum(c => c.WorkDay),
                    //          listEmployeeOfProject.FirstOrDefault(c => c.EmployeeId == m.Key).ChiPhiTheoGio,
                    //      }).ToList();

                    listResourceOfProject.ForEach(resource =>
                    {
                        var chiPhiTheoGio = listEmployeeOfProject
                            .FirstOrDefault(c => c.EmployeeId == resource.ObjectId)?.ChiPhiTheoGio;
                        if (chiPhiTheoGio !=
                            null)
                            resource.ChiPhiTheoGio = (decimal)chiPhiTheoGio;
                        if (resource.StartTime != null)
                        {
                            var ts = DateTime.Now.Date - (DateTime)resource.StartTime.Value.Date;
                            resource.ThoiGianPhanBoDenHienTai = (decimal)(((ts.TotalDays + 1) * 8 * resource.Allowcation) / 100);
                        }
                        else
                        {
                            resource.ThoiGianPhanBoDenHienTai = 0;
                        }
                    });


                    var ac = listResourceOfProject.Sum(c => c.ThoiGianPhanBoDenHienTai * (decimal)c.ChiPhiTheoGio);

                    #endregion

                    #region Tính PV, EV

                    listTaskOfProject.ForEach(task =>
                    {
                        switch (task.StatusCode)
                        {
                            case "NEW":
                                task.PVOfTask = 0;
                                break;
                            case "DL":
                                task.PVOfTask = 0.5 * (double)(task.EstimateHour ?? 0) * (double)item.GiaBanTheoGio;
                                break;
                            default:
                                task.PVOfTask = (double)(task.EstimateHour ?? 0) * (double)item.GiaBanTheoGio;
                                break;
                        }
                        task.EVOfTask = (double)(task.TaskComplate ?? 0) / 100 * (double)(task.EstimateHour ?? 0) * (double)item.GiaBanTheoGio;
                    });

                    var pv = listTaskOfProject.Sum(c => c.PVOfTask);
                    var ev = listTaskOfProject.Sum(c => c.EVOfTask);

                    #endregion

                    var projectCostResport = new ProjectCostReport
                    {
                        ProjectCostReportId = Guid.NewGuid(),
                        ProjectId = item.ProjectId,
                        Date = DateTime.Now,
                        Ac = (decimal)ac,
                        Pv = (decimal)pv,
                        Ev = (decimal)ev,
                        Active = true
                    };

                    listProjectCostReport.Add(projectCostResport);
                });

                using (var beginTransaction = context.Database.BeginTransaction())
                {
                    var listAllProjectId = listAllProject.Select(m => m.ProjectId).ToList();
                    var lstDelProjectCostReport = context.ProjectCostReport.Where(c => listAllProjectId.Contains(c.ProjectId) && c.Date.Date == DateTime.Now.Date).ToList();

                    context.ProjectCostReport.RemoveRange(lstDelProjectCostReport);
                    context.ProjectCostReport.AddRange(listProjectCostReport);
                    context.SaveChanges();
                    beginTransaction.Commit();
                }

                return new SynchronizedEvnResult
                {
                    MessageCode = "Success",
                    StatusCode = HttpStatusCode.OK,
                };
            }
            catch (Exception ex)
            {
                return new SynchronizedEvnResult
                {
                    MessageCode = ex.Message,
                    StatusCode = HttpStatusCode.ExpectationFailed
                };
            }
        }

        public GetMasterDataProjectInformationResult GetMasterDataProjectInformation(GetMasterDataProjectInformationParameter parameter)
        {
            try
            {

                #region Thông tin dự án

                var project = context.Project.Where(x => x.ProjectId == parameter.ProjectId).Select(y => new ProjectEntityModel
                {
                    ProjectId = y.ProjectId,
                    ProjectStartDate = y.ProjectStartDate,
                    ProjectEndDate = y.ProjectEndDate,
                    ActualStartDate = y.ActualStartDate,
                    ActualEndDate = y.ActualEndDate,
                    ProjectManagerId = y.ProjectManagerId,
                    ContractId = y.ContractId,
                    ProjectName = y.ProjectName,
                    ProjectCode = y.ProjectCode,
                    BudgetVnd = y.BudgetVnd,
                    BudgetUsd = y.BudgetUsd,
                    BudgetNgayCong = y.BudgetNgayCong,
                    CustomerId = y.CustomerId,
                    Description = y.Description,
                    ProjectSize = y.ProjectSize,
                    ProjectType = y.ProjectType,
                    ProjectStatus = y.ProjectStatus,
                    IncludeWeekend = y.IncludeWeekend,
                    Priority = y.Priority,
                }).FirstOrDefault();

                if (project == null)
                {
                    return new GetMasterDataProjectInformationResult
                    {
                        MessageCode = "Dự án không tồn tại trong hệ thống",
                        StatusCode = HttpStatusCode.ExpectationFailed
                    };
                }

                project.ProjectTypeName = context.Category.FirstOrDefault(c => c.CategoryId == project.ProjectType)?.CategoryName;
                project.ProjectStatusName = context.Category.FirstOrDefault(c => c.CategoryId == project.ProjectStatus)?.CategoryName;
                project.ProjectStatusCode = context.Category.FirstOrDefault(c => c.CategoryId == project.ProjectStatus)?.CategoryCode;
                switch (project.Priority)
                {
                    case 1:
                        project.PriorityName = "Thấp";
                        break;
                    case 2:
                        project.PriorityName = "Trung bình";
                        break;
                    case 3:
                        project.PriorityName = "Cao";
                        break;
                    default: break;
                }

                #endregion

                #region Tính % hoàn thành dự án và Thời gian dự kiến thực hiện

                CaculatorProjectTask(parameter.ProjectId, out decimal projectComplete, out decimal totalEstimateHour);

                #endregion

                #region Thời gian đã sử dụng

                var listAllTaskId = context.Task.Where(c => c.ProjectId == parameter.ProjectId).Select(m => m.TaskId).ToList();
                var totalHouseUsed = context.TimeSheet.Where(c => listAllTaskId.Contains(c.TaskId)).Sum(c => c.SpentHour);

                #endregion

                #region Tính Hiệu quả sử dụng nguồn lực(EE)

                var listProjectResource = context.ProjectResource.Where(x => x.ProjectId == parameter.ProjectId).Select(p => new ProjectResourceEntityModel()
                {
                    ProjectResourceId = p.ProjectResourceId,
                    TenantId = p.TenantId,
                    ResourceType = p.ResourceType,
                    IsCreateVendor = p.IsCreateVendor,
                    ResourceRole = p.ResourceRole,
                    ObjectId = p.ObjectId,
                    Allowcation = p.Allowcation,
                    StartTime = p.StartTime,
                    EndTime = p.EndTime,
                    CreateDate = p.CreateDate,
                    EmployeeRole = p.EmployeeRole,
                    IsOverload = p.IsOverload,
                    IncludeWeekend = p.IncludeWeekend
                }).OrderBy(x => x.CreateDate).ToList();

                double totalWorkDays = 0;

                listProjectResource.ForEach(resource =>
                {
                    if (context.Category.FirstOrDefault(r => r.CategoryId == resource.ResourceRole)?.CategoryCode == "NB")
                    {
                        // Lấy thông tin ngày nghỉ được phê duyệt
                        var absentPermissionId = context.Category.FirstOrDefault(ct => ct.CategoryCode.Trim() == "NP")?.CategoryId;
                        var absentWithoutPermissionId = context.Category.FirstOrDefault(ct => ct.CategoryCode.Trim() == "NKL")?.CategoryId;
                        var _empRequest = (from empR in context.EmployeeRequest
                                           join stt in context.Category on empR.StatusId equals stt.CategoryId
                                           where empR.OfferEmployeeId == resource.ObjectId && empR.RequestDate.Value.Year == DateTime.Now.Year && stt.CategoryCode.Trim() == "Approved"
                                           select empR).OrderByDescending(o => o.RequestDate).ToList();
                        double amountAbsentWithPermission = 0;
                        double amountAbsentWithoutPermission = 0;
                        // Danh sách ngày nghỉ
                        var lstDayOfRequest = new List<DateTime>();
                        _empRequest.ForEach(request =>
                        {
                            for (var date = request.StartDate; (date <= request.EnDate && date >= resource.StartTime && date <= resource.EndTime); date = date.Value.AddDays(1))
                            {
                                lstDayOfRequest.Add((DateTime)date);
                            }
                        });

                        _empRequest.ForEach(empR =>
                        {
                            for (var date = empR.StartDate; (date <= empR.EnDate && date >= resource.StartTime && date <= resource.EndTime); date = date.Value.AddDays(1))
                            {
                                if (empR.TypeRequest == absentPermissionId)
                                {
                                    if (empR.StartTypeTime == empR.EndTypeTime)
                                    {
                                        amountAbsentWithPermission = amountAbsentWithPermission + 0.5;
                                    }
                                    else
                                    {
                                        amountAbsentWithPermission = amountAbsentWithPermission + 1;
                                    }
                                }
                                if (empR.TypeRequest == absentWithoutPermissionId)
                                {
                                    if (empR.StartTypeTime == empR.EndTypeTime)
                                    {
                                        amountAbsentWithoutPermission = amountAbsentWithoutPermission + 0.5;
                                    }
                                    else
                                    {
                                        amountAbsentWithoutPermission = amountAbsentWithoutPermission + 1;
                                    }
                                }
                            }
                        });

                        // Ngày công
                        if (resource.EndTime != null && resource.StartTime != null)
                        {
                            TimeSpan ts = (DateTime)resource.EndTime - (DateTime)resource.StartTime;
                            var numberWeeken = TotalHoliday(resource.StartTime.Value, resource.EndTime.Value, resource.IncludeWeekend ?? false);
                            resource.WorkDay = (((ts.TotalDays + 1 - amountAbsentWithPermission - amountAbsentWithoutPermission - numberWeeken) * (resource.Allowcation ?? 0)) / 100);
                        }

                        if (resource.WorkDay != null) totalWorkDays += (double)resource.WorkDay;
                    }
                });

                var totalEE = 0M;

                if (project.BudgetNgayCong != null && totalWorkDays != 0)
                {
                    totalEE = (decimal)((project.BudgetNgayCong / (decimal?)totalWorkDays) * 100);
                }

                #endregion

                return new GetMasterDataProjectInformationResult
                {
                    Project = project,
                    ProjectTaskComplete = projectComplete,
                    TotalEstimateHour = totalEstimateHour,
                    TotalHourUsed = totalHouseUsed.Value,
                    TotalEE = totalEE,
                    MessageCode = "Success",
                    StatusCode = HttpStatusCode.OK,
                };
            }
            catch (Exception e)
            {
                return new GetMasterDataProjectInformationResult
                {
                    MessageCode = e.Message,
                    StatusCode = HttpStatusCode.ExpectationFailed,
                };
            }
        }

        public GetBaoCaoSuDungNguonLucResult GetBaoCaoSuDungNguonLuc(GetBaoCaoSuDungNguonLucParameter parameter)
        {
            try
            {
                var nowDate = DateTime.Now;

                #region Dữ liệu danh mục cần dùng

                var categoryTypeStatus = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "DAT");

                if (categoryTypeStatus == null)
                {
                    return new GetBaoCaoSuDungNguonLucResult()
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = "Loại dữ liệu danh mục [Trạng thái dự án] không tồn tại"
                    };
                }

                var listStatus = context.Category.Where(x => x.CategoryTypeId == categoryTypeStatus.CategoryTypeId)
                    .ToList();
                var listStausAccept = listStatus.Where(x => x.CategoryCode == "MOI" ||
                                                            x.CategoryCode == "DTK" ||
                                                            x.CategoryCode == "HTH" ||
                                                            x.CategoryCode == "TDU").ToList();
                var listStatusAcceptId = listStausAccept.Select(y => y.CategoryId).ToList();

                var categoryTypeLoaiNguonLuc = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "LNL");

                if (categoryTypeLoaiNguonLuc == null)
                {
                    return new GetBaoCaoSuDungNguonLucResult()
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = "Loại dữ liệu danh mục [Loại nguồn lực] không tồn tại"
                    };
                }

                var listLoaiNguonLuc = context.Category
                    .Where(x => x.CategoryTypeId == categoryTypeLoaiNguonLuc.CategoryTypeId).ToList();
                var nhanLuc = listLoaiNguonLuc.FirstOrDefault(x => x.CategoryCode == "NLC");

                if (nhanLuc == null)
                {
                    return new GetBaoCaoSuDungNguonLucResult()
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = "Dữ liệu danh mục [Nhân lực] (thuộc Loại nhân lực) không tồn tại"
                    };
                }

                var loaiVaiTroNguonLuc = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "VTN");

                if (loaiVaiTroNguonLuc == null)
                {
                    return new GetBaoCaoSuDungNguonLucResult()
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = "Loại dữ liệu danh mục [Vai trò nguồn lực] không tồn tại"
                    };
                }

                var listVaiTroNguonLuc = context.Category
                    .Where(x => x.CategoryTypeId == loaiVaiTroNguonLuc.CategoryTypeId).ToList();


                #endregion

                #region Báo cáo sử dụng nguồn nhân lực

                //Nếu Tháng hoặc Năm không tồn tại thì lấy giá trị mặc định là Tháng = tháng hiện tại và Năm = năm hiện tại
                if (parameter.Thang == null || parameter.Nam == null)
                {
                    parameter.Thang = nowDate.Month;
                    parameter.Nam = nowDate.Year;
                }

                var listProject = context.Project
                    .Where(x => parameter.ListProjectId.Count == 0 || parameter.ListProjectId.Contains(x.ProjectId))
                    .OrderBy(z => z.ProjectName).ToList();
                var listProjectId = listProject.Select(y => y.ProjectId).ToList();

                //Lấy list nguồn lực là nhân lực và có thời gian phân bổ trong tháng hiện tại
                var listResource = context.ProjectResource.Where(x => listProjectId.Contains(x.ProjectId) &&
                                                                      x.ResourceType == nhanLuc.CategoryId).ToList();

                //Lọc ra những dự án có nguồn lực được phân bổ ở tháng được chọn
                var listProjectForSelectedTime = new List<Project>();
                var listResourceIdForProject = new List<Guid>();
                listProject.ForEach(item =>
                {
                    var _listResourceId = listResource.Where(x => x.ProjectId == item.ProjectId &&
                                                                  x.StartTime != null &&
                                                                  x.EndTime != null &&
                                                                  CheckCurrentMonth((int) parameter.Thang.Value,
                                                                      (int) parameter.Nam.Value,
                                                                      x.StartTime.Value,
                                                                      x.EndTime.Value) == true)
                        .Select(y => y.ObjectId).Distinct().ToList();

                    if (_listResourceId.Count > 0)
                    {
                        listProjectForSelectedTime.Add(item);
                        listResourceIdForProject.AddRange(_listResourceId);
                    }
                });

                var listProjectIdForSelectedTime = listProjectForSelectedTime.Select(x => x.ProjectId).ToList();
                listResourceIdForProject = listResourceIdForProject.Distinct().ToList();

                //Lấy list nguồn lực là nhân lực và có thời gian phân bổ trong tháng hiện tại
                var listResourceForSelectedTime = listResource
                    .Where(x => listProjectIdForSelectedTime.Contains(x.ProjectId) &&
                                listResourceIdForProject.Contains(x.ObjectId)).ToList();
                var listNhanLucId = listResourceForSelectedTime.Select(y => y.ObjectId).Distinct().ToList();
                var listNhanVienDaThamGiaDuAn = context.Employee.Where(x => listNhanLucId.Contains(x.EmployeeId))
                    .OrderBy(z => z.EmployeeCode)
                    .ToList();

                #region Nhân viên ngoài phân bổ

                var listPosition = context.Position.ToList();
                var listOrganization = context.Organization.ToList();

                var listNhanVienKhongThamGiaDuAn = context.Employee.Where(x => !listNhanLucId.Contains(x.EmployeeId) && x.Active == true)
                    .Select(y => new EmployeeEntityModel
                    {
                        EmployeeId = y.EmployeeId,
                        EmployeeCode = y.EmployeeCode,
                        EmployeeName = y.EmployeeName,
                        EmployeeCodeName = y.EmployeeCode + " - " + y.EmployeeName,
                        PositionId = y.PositionId,
                        OrganizationId = y.OrganizationId
                    }).OrderBy(z => z.EmployeeCode).ToList();

                listNhanVienKhongThamGiaDuAn.ForEach(item =>
                {
                    var phongBan = listOrganization.FirstOrDefault(x => x.OrganizationId == item.OrganizationId);
                    item.OrganizationName = phongBan?.OrganizationName;

                    var chucVu = listPosition.FirstOrDefault(x => x.PositionId == item.PositionId);
                    item.PositionName = chucVu?.PositionName;
                });

                #endregion

                #region Tổng số ngày làm việc trong tháng được chọn

                var startDate = new DateTime((int)parameter.Nam, (int)parameter.Thang, 1);
                var endDate = startDate.AddMonths(1).AddDays(-1);
                decimal TongSoNgayTrongThang = (decimal)(endDate.Date - startDate.Date).TotalDays + 1;
                TinhNgayLamViecHelper.GetWeekendDaysBetween(startDate, endDate,
                    false,
                    out int tongSoNgayThuBay,
                    out int tongSoNgayChuNhat);

                var tongNgayLamViecTrongThang = TongSoNgayTrongThang - tongSoNgayThuBay - tongSoNgayChuNhat;

                #endregion

                var listData = new List<List<DataRowModel>>();
                
                var listTong = new List<TongBaoCaoSuDungNguonLuc>();

                int indexRow = 0;
                listNhanVienDaThamGiaDuAn.ForEach(item =>
                {
                    indexRow++;
                    var _listDataRow = new List<DataRowModel>();

                    var dataRow1 = new DataRowModel();
                    dataRow1.ColumnKey = "stt";
                    dataRow1.ColumnValue = indexRow.ToString();
                    dataRow1.Width = "80px";
                    dataRow1.TextAlign = "center";
                    _listDataRow.Add(dataRow1);

                    var dataRow2 = new DataRowModel();
                    dataRow2.ColumnKey = "objectName";
                    dataRow2.ColumnValue = item.EmployeeCode + " - " + item.EmployeeName;
                    dataRow2.Width = "170px";
                    dataRow2.TextAlign = "left";
                    _listDataRow.Add(dataRow2);

                    var dataRow3 = new DataRowModel();
                    dataRow3.ColumnKey = "soNgayLamViecTrongThang";
                    dataRow3.ColumnValue = tongNgayLamViecTrongThang.ToString();
                    dataRow3.ValueType = ValueTypeEnum.NUMBER;
                    dataRow3.Width = "150px";
                    dataRow3.TextAlign = "right";
                    _listDataRow.Add(dataRow3);

                    int dynamicIndex = 0;
                    int tongPhanTramPhanBo = 0;
                    decimal tongSoNgayThucTeThamGiaCuaTatCaDuAn = 0;
                    listProjectForSelectedTime.ForEach(project =>
                    {
                        var _dataRow0 = new DataRowModel();
                        var _dataRow1 = new DataRowModel();
                        var _dataRow2 = new DataRowModel();

                        var tong = new TongBaoCaoSuDungNguonLuc();
                        tong.ProjectId = project.ProjectId;

                        //Kiểm tra nguồn lực có nằm trong dự án và trong tháng đc chọn hay không?
                        var _nguonLuc = listResourceForSelectedTime.Where(x =>
                            x.ProjectId == project.ProjectId && x.ObjectId == item.EmployeeId &&
                            x.StartTime != null &&
                            x.EndTime != null &&
                            CheckCurrentMonth((int) parameter.Thang.Value,
                                (int) parameter.Nam.Value,
                                x.StartTime.Value,
                                x.EndTime.Value) == true).ToList();
                        
                        //Nếu nguồn lực không nằm trong dự án
                        if (_nguonLuc.Count == 0)
                        {
                            _dataRow0.ColumnKey = "vaiTro" + dynamicIndex.ToString();
                            _dataRow0.ColumnValue = null;
                            _dataRow0.Width = "80px";
                            _dataRow0.TextAlign = "left";

                            _dataRow1.ColumnKey = "phanBo" + dynamicIndex.ToString();
                            _dataRow1.ColumnValue = null;
                            _dataRow1.ValueType = ValueTypeEnum.NUMBER;
                            _dataRow1.Width = "80px";
                            _dataRow1.TextAlign = "right";

                            _dataRow2.ColumnKey = "soNgay" + dynamicIndex.ToString();
                            _dataRow2.ColumnValue = null;
                            _dataRow2.ValueType = ValueTypeEnum.NUMBER;
                            _dataRow2.Width = "80px";
                            _dataRow2.TextAlign = "right";

                            tong.PhanBo = 0;
                            tong.SoNgay = 0;
                        }
                        //Nếu nguồn lực nằm trong dự án
                        else
                        {
                            var listVaiTroId = _nguonLuc.Select(y => y.EmployeeRole).Distinct().ToList();
                            var listVaiTroTrongDuAn = listVaiTroNguonLuc.Where(x => listVaiTroId.Contains(x.CategoryId))
                                .Select(y => y.CategoryCode).OrderBy(z => z).ToList();

                            _dataRow0.ColumnKey = "vaiTro" + dynamicIndex.ToString();
                            _dataRow0.ColumnValue = String.Join(", ", listVaiTroTrongDuAn);
                            _dataRow0.Width = "80px";
                            _dataRow0.TextAlign = "left";

                            //Tính số ngày thực tế tham gia và Tổng phân bổ trong tháng được chọn
                            decimal tongSoNgayThucTeThamGia = 0;
                            decimal tongPhanBo = 0;
                            _nguonLuc.ForEach(_nhanLuc =>
                            {
                                decimal ngayLamViecTrongThang = NgayLamViecTrongThangDaChon((int)parameter.Thang,
                                    (int)parameter.Nam, _nhanLuc.StartTime.Value, _nhanLuc.EndTime.Value);

                                tongSoNgayThucTeThamGia += Math.Round(ngayLamViecTrongThang * _nhanLuc.Allowcation / 100, 2);

                                tongPhanBo +=
                                    Math.Round(
                                        (ngayLamViecTrongThang * _nhanLuc.Allowcation) / tongNgayLamViecTrongThang,
                                        0);
                            });

                            _dataRow1.ColumnKey = "phanBo" + dynamicIndex.ToString();
                            _dataRow1.ColumnValue = tongPhanBo.ToString();
                            _dataRow1.ValueType = ValueTypeEnum.NUMBER;
                            _dataRow1.Width = "80px";
                            _dataRow1.TextAlign = "right";

                            _dataRow2.ColumnKey = "soNgay" + dynamicIndex.ToString();
                            _dataRow2.ColumnValue = tongSoNgayThucTeThamGia.ToString();
                            _dataRow2.ValueType = ValueTypeEnum.NUMBER;
                            _dataRow2.Width = "80px";
                            _dataRow2.TextAlign = "right";

                            tongPhanTramPhanBo += (int) tongPhanBo;
                            tongSoNgayThucTeThamGiaCuaTatCaDuAn += tongSoNgayThucTeThamGia;

                            tong.PhanBo = (int) tongPhanBo;
                            tong.SoNgay = tongSoNgayThucTeThamGia;
                        }

                        _listDataRow.Add(_dataRow0);
                        _listDataRow.Add(_dataRow1);
                        _listDataRow.Add(_dataRow2);

                        listTong.Add(tong);

                        dynamicIndex++;
                    });

                    var _dataRowTong1 = new DataRowModel();
                    _dataRowTong1.ColumnKey = "phanBo" + dynamicIndex.ToString();
                    _dataRowTong1.ColumnValue = tongPhanTramPhanBo.ToString();
                    _dataRowTong1.ValueType = ValueTypeEnum.NUMBER;
                    _dataRowTong1.Width = "80px";
                    _dataRowTong1.TextAlign = "right";

                    var _dataRowTong2 = new DataRowModel();
                    _dataRowTong2.ColumnKey = "soNgay" + dynamicIndex.ToString();
                    _dataRowTong2.ColumnValue = tongSoNgayThucTeThamGiaCuaTatCaDuAn.ToString();
                    _dataRowTong2.ValueType = ValueTypeEnum.NUMBER;
                    _dataRowTong2.Width = "80px";
                    _dataRowTong2.TextAlign = "right";

                    _listDataRow.Add(_dataRowTong1);
                    _listDataRow.Add(_dataRowTong2);

                    listData.Add(_listDataRow);
                });

                var listHeaderRow1 = new List<DataHeaderModel>()
                {
                    new DataHeaderModel()
                    {
                        ColumnKey = "stt",
                        ColumnValue = "STT",
                        TextAlign = "center",
                        Width = "80px",
                        Rowspan = 2,
                        Colspan = 0
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "objectName",
                        ColumnValue = "Nhân viên",
                        TextAlign = "center",
                        Width = "170px",
                        Rowspan = 2,
                        Colspan = 0
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "soNgayLamViecTrongThang",
                        ColumnValue = "Số ngày làm việc trong tháng",
                        TextAlign = "center",
                        Width = "150px",
                        Rowspan = 2,
                        Colspan = 0
                    }
                };

                var listHeaderRow2 = new List<DataHeaderModel>();

                if (listProjectForSelectedTime.Count > 0)
                {
                    for (int i = 0; i < listProjectForSelectedTime.Count; i++)
                    {
                        var project = listProjectForSelectedTime[i];

                        var header = new DataHeaderModel();

                        header.ColumnValue = project.ProjectName;
                        header.TextAlign = "center";
                        header.Width = "240px";
                        header.Colspan = 3;
                        header.Rowspan = 0;

                        listHeaderRow1.Add(header);

                        var headerRow2_1 = new DataHeaderModel();
                        headerRow2_1.ColumnValue = "Vai trò";
                        headerRow2_1.TextAlign = "center";
                        headerRow2_1.Width = "80px";

                        var headerRow2_2 = new DataHeaderModel();
                        headerRow2_2.ColumnValue = "% Phân bổ";
                        headerRow2_2.TextAlign = "center";
                        headerRow2_2.Width = "80px";

                        var headerRow2_3 = new DataHeaderModel();
                        headerRow2_3.ColumnValue = "Số ngày phân bổ thực tế";
                        headerRow2_3.TextAlign = "center";
                        headerRow2_3.Width = "80px";

                        listHeaderRow2.Add(headerRow2_1);
                        listHeaderRow2.Add(headerRow2_2);
                        listHeaderRow2.Add(headerRow2_3);
                    }
                }

                var headerTong = new DataHeaderModel();

                headerTong.ColumnValue = "Tổng";
                headerTong.TextAlign = "center";
                headerTong.Width = "160px";
                headerTong.Colspan = 2;
                headerTong.Rowspan = 0;

                listHeaderRow1.Add(headerTong);

                var headerTongRow2_2 = new DataHeaderModel();
                headerTongRow2_2.ColumnValue = "% Phân bổ";
                headerTongRow2_2.TextAlign = "center";
                headerTongRow2_2.Width = "80px";

                var headerTongRow2_3 = new DataHeaderModel();
                headerTongRow2_3.ColumnValue = "Số ngày thực tế tham gia";
                headerTongRow2_3.TextAlign = "center";
                headerTongRow2_3.Width = "80px";

                listHeaderRow2.Add(headerTongRow2_2);
                listHeaderRow2.Add(headerTongRow2_3);

                #region footer

                var listDataFooter = new List<DataHeaderModel>()
                {
                    new DataHeaderModel()
                    {
                        ColumnValue = "Tổng số",
                        TextAlign = "center",
                        Width = "400px",
                    }
                };

                var listGroupTong = listTong.GroupBy(g => g.ProjectId).Select(y => new
                {
                    ProjectId = y.Key,
                    TongPhanBo = y.Sum(s => s.PhanBo),
                    TongSoNgay = y.Sum(s => s.SoNgay)
                }).ToList();

                listGroupTong.ForEach(item =>
                {
                    var emptyFooter = new DataHeaderModel()
                    {
                        ColumnValue = "",
                        TextAlign = "center",
                        Width = "80px"
                    };

                    var tongPhanBoFooter = new DataHeaderModel()
                    {
                        ColumnValue = item.TongPhanBo.ToString(),
                        ValueType = ValueTypeEnum.NUMBER,
                        TextAlign = "right",
                        Width = "80px"
                    };

                    var tongSoNgayFooter = new DataHeaderModel()
                    {
                        ColumnValue = item.TongSoNgay.ToString(),
                        ValueType = ValueTypeEnum.NUMBER,
                        TextAlign = "right",
                        Width = "80px"
                    };

                    listDataFooter.Add(emptyFooter);
                    listDataFooter.Add(tongPhanBoFooter);
                    listDataFooter.Add(tongSoNgayFooter);
                });

                var _tongPhanBoFooter = new DataHeaderModel()
                {
                    ColumnValue = null,
                    ValueType = ValueTypeEnum.NUMBER,
                    TextAlign = "right",
                    Width = "80px"
                };

                var _tongSoNgayFooter = new DataHeaderModel()
                {
                    ColumnValue = listGroupTong.Sum(s => s.TongSoNgay).ToString(),
                    ValueType = ValueTypeEnum.NUMBER,
                    TextAlign = "right",
                    Width = "80px"
                };

                listDataFooter.Add(_tongPhanBoFooter);
                listDataFooter.Add(_tongSoNgayFooter);

                #endregion

                #endregion

                return new GetBaoCaoSuDungNguonLucResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success",
                    ListData = listData,
                    ListHeaderRow1 = listHeaderRow1,
                    ListHeaderRow2 = listHeaderRow2,
                    ListDataFooter = listDataFooter,
                    ListNhanVienKhongThamGiaDuAn = listNhanVienKhongThamGiaDuAn
                };
            }
            catch (Exception e)
            {
                return new GetBaoCaoSuDungNguonLucResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetMasterDataBaoCaoSuDungNguonLucResult GetMasterDataBaoCaoSuDungNguonLuc(
            GetMasterDataBaoCaoSuDungNguonLucParameter parameter)
        {
            try
            {
                var listProject = context.Project.Select(y => new ProjectEntityModel
                {
                    ProjectId = y.ProjectId,
                    ProjectName =  y.ProjectName
                }).OrderBy(z => z.ProjectName).ToList();

                return new GetMasterDataBaoCaoSuDungNguonLucResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success",
                    ListProject = listProject
                };
            }
            catch (Exception e)
            {
                return new GetMasterDataBaoCaoSuDungNguonLucResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetBaoCaoTongHopCacDuAnResult GetBaoCaoTongHopCacDuAn(GetBaoCaoTongHopCacDuAnParameter parameter)
        {
            try
            {
                #region Dữ liệu danh mục cần dùng

                var categoryTypeStatus = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "DAT");

                if (categoryTypeStatus == null)
                {
                    return new GetBaoCaoTongHopCacDuAnResult()
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = "Loại dữ liệu danh mục [Trạng thái dự án] không tồn tại"
                    };
                }

                var listStatus = context.Category.Where(x => x.CategoryTypeId == categoryTypeStatus.CategoryTypeId)
                    .ToList();
                var listStausAccept = listStatus.Where(x => x.CategoryCode == "MOI" ||
                                                            x.CategoryCode == "DTK" ||
                                                            x.CategoryCode == "HTH" ||
                                                            x.CategoryCode == "TDU").ToList();
                var listStatusAcceptId = listStausAccept.Select(y => y.CategoryId).ToList();

                var categoryTypeLoaiNguonLuc = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "LNL");

                if (categoryTypeLoaiNguonLuc == null)
                {
                    return new GetBaoCaoTongHopCacDuAnResult()
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = "Loại dữ liệu danh mục [Loại nguồn lực] không tồn tại"
                    };
                }

                var listLoaiNguonLuc = context.Category
                    .Where(x => x.CategoryTypeId == categoryTypeLoaiNguonLuc.CategoryTypeId).ToList();
                var nhanLuc = listLoaiNguonLuc.FirstOrDefault(x => x.CategoryCode == "NLC");

                if (nhanLuc == null)
                {
                    return new GetBaoCaoTongHopCacDuAnResult()
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = "Dữ liệu danh mục [Nhân lực] (thuộc Loại nhân lực) không tồn tại"
                    };
                }

                var loaiDeXuat = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "LXU");
                var statusNghiPhep = context.Category.FirstOrDefault(ct => ct.CategoryCode.Trim() == "NP" &&
                                                                           ct.CategoryTypeId ==
                                                                           loaiDeXuat.CategoryTypeId)?.CategoryId;
                var statusNghiKhongLuong = context.Category.FirstOrDefault(ct => ct.CategoryCode.Trim() == "NKL" &&
                                                                                 ct.CategoryTypeId ==
                                                                                 loaiDeXuat.CategoryTypeId)?.CategoryId;

                #endregion

                #region Báo cáo tổng hợp các dự án

                var listProjectPipeline_Moi = new List<ProjectPipelineModel>();
                var listProjectPipeline_DangThucHien = new List<ProjectPipelineModel>();
                var listProjectPipeline_HoanThanh = new List<ProjectPipelineModel>();
                var listProjectPipeline_TamDung = new List<ProjectPipelineModel>();

                var listProjectPipeline = new List<ProjectPipelineModel>();

                var listProject = context.Project.Where(x => listStatusAcceptId.Contains(x.ProjectStatus.Value))
                    .ToList();
                var listProjectId = listProject.Select(y => y.ProjectId).ToList();
                var listTask = context.Task.Where(x => listProjectId.Contains(x.ProjectId)).ToList();
                //Chỉ lấy nguồn lực là Nhân lực
                var listNguonLuc = context.ProjectResource.Where(x => listProjectId.Contains(x.ProjectId) &&
                                                                      x.ResourceType == nhanLuc.CategoryId &&
                                                                      x.StartTime != null &&
                                                                      x.EndTime != null).ToList();
                var listNguonLucId = listNguonLuc.Select(y => y.ObjectId).Distinct().ToList();

                var listDeXuatXinNghi = (from empR in context.EmployeeRequest
                                   join status in context.Category on empR.StatusId equals status.CategoryId
                                   where listNguonLucId.Contains(empR.OfferEmployeeId) &&
                                         status.CategoryCode.Trim() == "Approved"
                                   select empR).OrderByDescending(o => o.RequestDate).ToList();

                listProject.ForEach(item =>
                {
                    var status = listStatus.FirstOrDefault(x => x.CategoryId == item.ProjectStatus);

                    var listNguonLucForProject = listNguonLuc.Where(x => x.ProjectId == item.ProjectId).ToList();

                    decimal ngayCongThucTe = 0;
                    listNguonLucForProject.ForEach(nguonLuc =>
                    {
                        var listDeXuatXinNghiTheoNhanVien =
                            listDeXuatXinNghi.Where(x => x.OfferEmployeeId == nguonLuc.ObjectId).ToList();

                        TinhNgayLamViecHelper.GetWeekendDaysBetween(nguonLuc.StartTime.Value, nguonLuc.EndTime.Value,
                            nguonLuc.IncludeWeekend ?? false, out int soNgayThuBay, out int soNgayChuNhat);
                        var tongSoNgayCuoiTuan = soNgayThuBay + soNgayChuNhat;

                        TinhNgayLamViecHelper.GetSoNgayNghiPhep(
                            nguonLuc.StartTime.Value, nguonLuc.EndTime.Value, 
                            statusNghiPhep.Value, statusNghiKhongLuong.Value,
                            listDeXuatXinNghiTheoNhanVien,
                            out decimal soNgayNghiPhep, out decimal soNgayNghiKhongLuong);

                        decimal tongSoNgayNghiPhep = soNgayNghiPhep + soNgayNghiKhongLuong;

                        decimal TongSoNgay =
                            (decimal) (nguonLuc.EndTime.Value.Date - nguonLuc.StartTime.Value.Date).TotalDays + 1;

                        ngayCongThucTe += (TongSoNgay - tongSoNgayCuoiTuan - tongSoNgayNghiPhep) *
                                          nguonLuc.Allowcation / 100;
                    });

                    decimal hieuQuaSuDungNguonLuc = 0;
                    if (ngayCongThucTe != 0)
                    {
                        hieuQuaSuDungNguonLuc = Math.Round((item.BudgetNgayCong ?? 0) / ngayCongThucTe * 100, 0);
                    }

                    var listTaskForProject = listTask.Where(x => x.ProjectId == item.ProjectId).ToList();

                    TienDoHoanThanhDuAn(listTaskForProject, out decimal tienDoHoanThanh, out decimal totalEstimateHour);

                    var projectLine = new ProjectPipelineModel();
                    projectLine.ProjectName = item.ProjectName;
                    projectLine.TrangThaiDuAn = status?.CategoryName;
                    projectLine.TrangThaiCode = status?.CategoryCode;
                    projectLine.NgayBatDauDuKien = item.ProjectStartDate;
                    projectLine.NgayKetThucDuKien = item.ProjectEndDate;
                    projectLine.NgayBatDauThucTe = item.ActualStartDate;
                    projectLine.NgayKetThucThucTe = item.ActualEndDate;
                    projectLine.NgayKyBienBanNghiemThu = item.NgayKyNghiemThu;
                    projectLine.NgayCongTheoNganSach = item.BudgetNgayCong ?? 0;
                    projectLine.VndTheoNganSach = item.BudgetVnd ?? 0;
                    projectLine.UsdTheoNganSach = item.BudgetUsd ?? 0;
                    projectLine.NgayCongTheoThucTe = ngayCongThucTe;
                    projectLine.VndTheoThucTe = 0;
                    projectLine.UsdTheoThucTe = 0;
                    projectLine.HieuQuaSuDungNguonLuc = hieuQuaSuDungNguonLuc;
                    projectLine.TienDo = Math.Round(tienDoHoanThanh, 0);
                    projectLine.MucDoUuTienCode = item.Priority ?? 0;

                    if (projectLine.TrangThaiCode == "MOI")
                    {
                        listProjectPipeline_Moi.Add(projectLine);
                    }
                    if (projectLine.TrangThaiCode == "DTK")
                    {
                        listProjectPipeline_DangThucHien.Add(projectLine);
                    }
                    if (projectLine.TrangThaiCode == "HTH")
                    {
                        listProjectPipeline_HoanThanh.Add(projectLine);
                    }
                    if (projectLine.TrangThaiCode == "TDU")
                    {
                        listProjectPipeline_TamDung.Add(projectLine);
                    }
                });

                listProjectPipeline_Moi = listProjectPipeline_Moi
                    .OrderByDescending(x => x.MucDoUuTienCode)
                    .ThenBy(z => z.NgayKetThucDuKien).ToList();

                listProjectPipeline_DangThucHien = listProjectPipeline_DangThucHien
                    .OrderByDescending(x => x.MucDoUuTienCode)
                    .ThenBy(z => z.NgayKetThucDuKien).ToList();

                listProjectPipeline_HoanThanh = listProjectPipeline_HoanThanh
                    .OrderByDescending(x => x.MucDoUuTienCode)
                    .ThenBy(z => z.NgayKetThucDuKien).ToList();

                listProjectPipeline_TamDung = listProjectPipeline_TamDung
                    .OrderByDescending(x => x.MucDoUuTienCode)
                    .ThenBy(z => z.NgayKetThucDuKien).ToList();

                listProjectPipeline.AddRange(listProjectPipeline_Moi);
                listProjectPipeline.AddRange(listProjectPipeline_DangThucHien);
                listProjectPipeline.AddRange(listProjectPipeline_HoanThanh);
                listProjectPipeline.AddRange(listProjectPipeline_TamDung);

                var stt = 0;
                listProjectPipeline.ForEach(item =>
                {
                    stt++;
                    item.Stt = stt;
                });

                #endregion

                return new GetBaoCaoTongHopCacDuAnResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success",
                    ListProjectPipeline = listProjectPipeline
                };
            }
            catch (Exception e)
            {
                return new GetBaoCaoTongHopCacDuAnResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetThoiGianUocLuongHangMucResult GetThoiGianUocLuongHangMuc(GetThoiGianUocLuongHangMucParameter parameter)
        {
            try
            {
                var hangMuc = context.ProjectScope.FirstOrDefault(x => x.ProjectScopeId == parameter.ProjectScopeId);
                if (hangMuc == null)
                {
                    return new GetThoiGianUocLuongHangMucResult()
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = "Hạng mục không tồn tại trên hệ thống",
                    };
                }

                var listHangMuc = context.ProjectScope.ToList();
                var listHangMucConId = GetProjectScopeChild(listHangMuc, hangMuc.ProjectScopeId, new List<Guid>());
                listHangMucConId.Add(hangMuc.ProjectScopeId);
                var listTask = context.Task.Where(x => x.ProjectScopeId != null &&
                                                       listHangMucConId.Contains(x.ProjectScopeId.Value)).ToList();

                //Lấy tổng Số giờ dự kiến thực hiện của tất cả các task thuộc hạng mục
                decimal thoiGianUocLuong = listTask.Sum(s => (s.EstimateHour ?? 0));

                return new GetThoiGianUocLuongHangMucResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success",
                    ThoiGianUocLuong = thoiGianUocLuong
                };
            }
            catch (Exception e)
            {
                return new GetThoiGianUocLuongHangMucResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetPhanBoTheoNguonLucResult GetPhanBoTheoNguonLuc(GetPhanBoTheoNguonLucParameter parameter)
        {
            try
            {
                var listPhanBoNguonLuc = new List<PhanBoNguonLucModel>();

                var categoryType = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "NCNL");
                var listCategory = context.Category.Where(x => x.CategoryTypeId == categoryType.CategoryTypeId)
                    .ToList();
                var noiBo = listCategory.FirstOrDefault(x => x.CategoryCode == "NB");

                var _categoryType = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "LNL");
                var _listCategory = context.Category.Where(x => x.CategoryTypeId == _categoryType.CategoryTypeId)
                    .ToList();
                var nhanLuc = _listCategory.FirstOrDefault(x => x.CategoryCode == "NLC");

                var listProjectResource = context.ProjectResource.Where(x => x.ObjectId == parameter.ObjectId &&
                                                                             x.ResourceRole == noiBo.CategoryId &&
                                                                             x.ResourceType == nhanLuc.CategoryId)
                    .OrderByDescending(z => z.CreateDate).ToList();

                var listProjectId = listProjectResource.Select(y => y.ProjectId).Distinct().ToList();
                var listProject = context.Project.Where(x => listProjectId.Contains(x.ProjectId)).ToList();

                var emp = context.Employee.FirstOrDefault(x => x.EmployeeId == parameter.ObjectId);

                int stt = 0;
                listProjectResource.ForEach(item =>
                {
                    stt++;
                    var phanBoNguonLuc = new PhanBoNguonLucModel();
                    phanBoNguonLuc.Stt = stt;
                    phanBoNguonLuc.ProjectName =
                        listProject.FirstOrDefault(x => x.ProjectId == item.ProjectId)?.ProjectName;
                    phanBoNguonLuc.PhanTram = item.Allowcation;

                    listPhanBoNguonLuc.Add(phanBoNguonLuc);
                });

                return new GetPhanBoTheoNguonLucResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success",
                    EmployeeCodeName = emp?.EmployeeCode + " - " + emp?.EmployeeName,
                    ListPhanBoNguonLuc = listPhanBoNguonLuc
                };
            }
            catch (Exception e)
            {
                return new GetPhanBoTheoNguonLucResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        private void TienDoHoanThanhDuAn(List<Task> listTask, out decimal projectComplete, out decimal totalEstimateHour)
        {
            var total = 0M;
            var taskComplete = 0M;
            listTask.ForEach(item =>
            {
                taskComplete += (item?.TaskComplate ?? 0) / 100 * (item?.EstimateHour ?? 0);
                total += item?.EstimateHour ?? 0;
            });

            totalEstimateHour = total;
            projectComplete = total != 0 ? taskComplete / total * 100 : 0M;
        }

        /* Kiểm tra tháng được chọn có nằm trong một khoảng thời gian xác định nào đó hay không */
        private bool CheckCurrentMonth(int month, int year, DateTime startDate, DateTime endDate)
        {
            bool result = false;

            int monthStartDate = startDate.Month;
            int yearStartDate = startDate.Year;
            int monthEndDate = endDate.Month;
            int yearEndDate = endDate.Year;

            if (year < yearStartDate)
            {
                return false;
            }

            if (year > yearEndDate)
            {
                return false;
            }

            if (year >= yearStartDate && year <= yearEndDate)
            {
                var between1 = (year - yearStartDate) * 12 + month - monthStartDate;
                var between2 = (yearEndDate - year) * 12 + monthEndDate - month;

                if (between1 < 0 || between2 < 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }

            return result;
        }

        /* Tính số ngày làm việc trong tháng đã chọn */
        private int NgayLamViecTrongThangDaChon(int month, int year, DateTime startDate, DateTime endDate)
        {
            //Vì tháng được chọn chắc chắn nằm trong khoảng thời gian nên:
            int result = 0;

            int monthStartDate = startDate.Month;
            int yearStartDate = startDate.Year;
            int monthEndDate = endDate.Month;
            int yearEndDate = endDate.Year;

            var between = (year - yearStartDate) * 12 + month - monthStartDate;

            //Nếu tháng được chọn = tháng bắt đầu làm việc thì
            if (between == 0)
            {
                //Ngày cuối cùng của tháng bắt đầu làm việc
                var lastDate = new DateTime(yearStartDate, monthStartDate, 1).AddMonths(1).AddDays(-1);

                //Nếu ngày cuối cùng của tháng bắt đầu làm việc lớn hơn ngày cuối cùng của tháng làm việc cuối cùng
                if (lastDate.Date > endDate.Date)
                {
                    //Tính tổng số ngày thứ 7 và CN trong khoảng thời gian
                    TinhNgayLamViecHelper.GetWeekendDaysBetween(startDate, endDate,
                        false,
                        out int tongSoNgayThuBay,
                        out int tongSoNgayChuNhat);

                    //Số ngày làm việc = Ngày cuối cùng của tháng bắt đầu làm việc - Ngày bắt đầu làm việc - Tổng số ngày thứ 7 - Tổng số ngày Chủ nhật
                    result = (int)(endDate.Date - startDate.Date).TotalDays + 1 - tongSoNgayThuBay - tongSoNgayChuNhat;
                }
                //Ngược lại
                else
                {
                    //Tính tổng số ngày thứ 7 và CN trong khoảng thời gian
                    TinhNgayLamViecHelper.GetWeekendDaysBetween(startDate, lastDate,
                        false,
                        out int tongSoNgayThuBay,
                        out int tongSoNgayChuNhat);

                    //Số ngày làm việc = Ngày cuối cùng của tháng bắt đầu làm việc - Ngày bắt đầu làm việc - Tổng số ngày thứ 7 - Tổng số ngày Chủ nhật
                    result = (int)(lastDate.Date - startDate.Date).TotalDays + 1 - tongSoNgayThuBay - tongSoNgayChuNhat;
                }
            }
            //Nếu tháng được chọn > tháng bắt đầu làm việc thì
            else
            {
                //Nếu tháng được chọn là tháng làm việc cuối cùng
                if (month == monthEndDate && year == yearEndDate)
                {
                    //Ngày đầu tiên của tháng làm việc cuối cùng
                    var firstDate = new DateTime(yearEndDate, monthEndDate, 1);

                    //Tính tổng số ngày thứ 7 và CN trong khoảng thời gian
                    TinhNgayLamViecHelper.GetWeekendDaysBetween(firstDate, endDate,
                        false,
                        out int tongSoNgayThuBay,
                        out int tongSoNgayChuNhat);

                    //Số ngày làm việc = Ngày cuối cùng của tháng làm việc cuối cùng - Ngày bắt đầu làm việc - Tổng số ngày thứ 7 - Tổng số ngày Chủ nhật
                    result = (int)(endDate.Date - firstDate.Date).TotalDays + 1 - tongSoNgayThuBay - tongSoNgayChuNhat;
                }
                //Nếu tháng được chọn không phải tháng làm việc cuối cùng
                else
                {
                    //Ngày đầu tiên của tháng được chọn
                    var firstDate = new DateTime(year, month, 1);

                    //Ngày cuối cùng của tháng được chọn
                    var lastDate = new DateTime(year, month, 1).AddMonths(1).AddDays(-1);

                    //Tính tổng số ngày thứ 7 và CN trong khoảng thời gian
                    TinhNgayLamViecHelper.GetWeekendDaysBetween(firstDate, lastDate,
                        false,
                        out int tongSoNgayThuBay,
                        out int tongSoNgayChuNhat);

                    //Số ngày làm việc = Ngày cuối cùng của tháng được chọn - Ngày bắt đầu làm việc - Tổng số ngày thứ 7 - Tổng số ngày Chủ nhật
                    result = (int)(lastDate.Date - firstDate.Date).TotalDays + 1 - tongSoNgayThuBay - tongSoNgayChuNhat;
                }
            }

            return result;
        }

        /* Lấy ra Ngày bắt đầu và Ngày kết thúc nằm trong khoảng thời gian được chọn */
        private void GetFirstAndLastDateForBeetwenTime(
            DateTime firstDate, DateTime lastDate, 
            DateTime startTimeInput, DateTime endTimeInput, 
            out DateTime? startTimeOutput, out DateTime? endTimeOutput)
        {
            startTimeOutput = null;
            endTimeOutput = null;

            #region Lấy chốt chặn đầu cuối

            if (lastDate.Date >= endTimeInput.Date)
            {
                endTimeOutput = endTimeInput.Date;

                if (firstDate.Date > endTimeInput.Date)
                {
                    return;
                }

                if (firstDate.Date >= startTimeInput.Date && firstDate.Date <= endTimeInput.Date)
                {
                    startTimeOutput = firstDate.Date;
                }
                else if (firstDate.Date < startTimeInput.Date)
                {
                    startTimeOutput = startTimeInput.Date;
                }
            }
            else if (lastDate.Date < endTimeInput.Date && lastDate.Date >= startTimeInput.Date)
            {
                endTimeOutput = lastDate.Date;

                if (firstDate.Date >= startTimeInput.Date)
                {
                    startTimeOutput = firstDate.Date;
                }
                else
                {
                    startTimeOutput = startTimeInput.Date;
                }
            }

            #endregion
        }

        /* Lấy tất cả id hạng mục con */
        private List<Guid> GetProjectScopeChild(List<ProjectScope> listProjectScope, Guid id, List<Guid> list)
        {
            var listChild = listProjectScope.Where(o => o.ParentId == id).ToList();
            listChild.ForEach(item =>
            {
                list.Add(item.ProjectScopeId);
                GetProjectScopeChild(listProjectScope, item.ProjectScopeId, list);
            });

            return list;
        }
    }

    public class TongBaoCaoSuDungNguonLuc
    {
        public Guid ProjectId { get; set; }
        public int PhanBo { get; set; }
        public decimal SoNgay { get; set; }
    }
}
