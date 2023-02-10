using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using TN.TNM.BusinessLogic.Interfaces.Project;
using TN.TNM.BusinessLogic.Messages.Requests.Note;
using TN.TNM.BusinessLogic.Messages.Requests.Project;
using TN.TNM.BusinessLogic.Messages.Requests.Task;
using TN.TNM.BusinessLogic.Messages.Responses.Note;
using TN.TNM.BusinessLogic.Messages.Responses.Project;
using TN.TNM.BusinessLogic.Messages.Responses.Task;
using TN.TNM.BusinessLogic.Models.Category;
using TN.TNM.BusinessLogic.Models.Contract;
using TN.TNM.BusinessLogic.Models.Customer;
using TN.TNM.BusinessLogic.Models.Employee;
using TN.TNM.BusinessLogic.Models.Project;
using TN.TNM.BusinessLogic.Models.Vendor;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.Note;

namespace TN.TNM.BusinessLogic.Factories.Project
{
    public class ProjectFactory : BaseFactory, IProject
    {
        private IProjectDataAccess iProjectDataAccess;

        public ProjectFactory(IProjectDataAccess _iProjectDataAccess, ILogger<ProjectFactory> _logger)
        {
            this.iProjectDataAccess = _iProjectDataAccess;
            this.logger = _logger;
        }

        public CreateProjectResponse CreateProject(CreateProjectRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iProjectDataAccess.CreateProject(parameter);
                var response = new CreateProjectResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,

                    ProjectId = result.ProjectId
                };

                return response;
            }
            catch (Exception e)
            {
                return new CreateProjectResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetMasterProjectResponse GetMasterProjectCreate(GetMasterProjectRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iProjectDataAccess.GetMasterProjectCreate(parameter);
                var response = new GetMasterProjectResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,

                    ListEmployee = new List<EmployeeModel>(),
                    ListCustomer = new List<CustomerModel>(),
                    ListContract = new List<ContractModel>(),
                    ListProjectType = new List<CategoryModel>(),
                    ListProjectScope = new List<CategoryModel>(),
                    ListProjectStatus = new List<CategoryModel>(),
                    ListTargetType = new List<CategoryModel>(),
                    ListTargetUnit = new List<CategoryModel>(),
                };

                result.ListProjectStatus.ForEach(item =>
                {
                    response.ListProjectStatus.Add(new CategoryModel(item));
                });
                result.ListProjectScope.ForEach(item =>
                {
                    response.ListProjectScope.Add(new CategoryModel(item));
                });

                result.ListProjectType.ForEach(item =>
                {
                    response.ListProjectType.Add(new CategoryModel(item));
                });

                result.ListTargetType.ForEach(item =>
                {
                    response.ListTargetType.Add(new CategoryModel(item));
                });
                result.ListTargetUnit.ForEach(item =>
                {
                    response.ListTargetUnit.Add(new CategoryModel(item));
                });

                result.ListContract.ForEach(item =>
                {
                    var obj = new ContractModel(item);
                    obj.ListDetail = item.ListDetail;
                    obj.ListCostDetail = item.ListCostDetail;
                    response.ListContract.Add(obj);
                });

                result.ListEmployee.ForEach(item =>
                {
                    response.ListEmployee.Add(new EmployeeModel(item));
                });

                result.ListCustomer.ForEach(item =>
                {
                    response.ListCustomer.Add(new CustomerModel(item));
                });


                return response;
            }
            catch (Exception e)
            {
                return new GetMasterProjectResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }


        public SearchProjectResponse SearchProject(SearchProjectRequest request)
        {
            try
            {
                this.logger.LogInformation("Search Project");
                var parameter = request.ToParameter();
                var result = iProjectDataAccess.SearchProject(parameter);

                var response = new SearchProjectResponse
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = result.Message,
                    ListProject = new List<ProjectModel>(),
                    ListStatus = new List<CategoryModel>(),
                    ListProjectType = new List<CategoryModel>(),
                    ListEmployee = new List<EmployeeModel>(),
                };
                result.ListProject?.ForEach(or =>
                {
                    response.ListProject.Add(new ProjectModel(or));
                });
                result.ListStatus?.ForEach(item =>
                {
                    response.ListStatus.Add(new CategoryModel(item));
                });
                result.ListProjectType?.ForEach(item =>
                {
                    response.ListProjectType?.Add(new CategoryModel(item));
                });
                result.ListEmployee?.ForEach(item =>
                {
                    response.ListEmployee.Add(new EmployeeModel(item));
                });
                return response;
            }
            catch (Exception e)
            {
                return new SearchProjectResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public UpdateProjectStatusResponse UpdateProjectStatus(UpdateProjectStatusRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iProjectDataAccess.UpdateProjectStatus(parameter);
                var response = new UpdateProjectStatusResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    StatusId = result.StatusId,
                    MessageCode = result.Message,
                };

                return response;
            }
            catch (Exception e)
            {
                return new UpdateProjectStatusResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetMasterUpdateProjectResponse GetMasterUpdateProjectCreate(GetMasterUpdateProjectRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iProjectDataAccess.GetMasterUpdateProjectCreate(parameter);
                var response = new GetMasterUpdateProjectResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    Project = new ProjectModel(result.Project),
                    ListEmployee = new List<EmployeeModel>(),
                    ListCustomer = new List<CustomerModel>(),
                    ListContract = new List<ContractModel>(),
                    ListProjectType = new List<CategoryModel>(),
                    ListProjectScope = new List<CategoryModel>(),
                    ListProjectStatus = new List<CategoryModel>(),
                    ListTargetType = new List<CategoryModel>(),
                    ListTargetUnit = new List<CategoryModel>(),
                    ListProjectTarget = new List<ProjectTargetModel>(),
                    Role = result.Role,
                    ListNote = result.ListNote,
                    Notes = result.Notes,
                    TotalRecordsNote = result.TotalRecordsNote,
                    HasTaskInProgress = result.HasTaskInProgress,
                    ListProject = result.ListProject,
                };
                response.Project.EmployeeSM = result.Project.EmployeeSM;
                response.Project.EmployeeSub = result.Project.EmployeeSub;
                result.ListProjectStatus.ForEach(item =>
                {
                    response.ListProjectStatus.Add(new CategoryModel(item));
                });
                result.ListProjectScope.ForEach(item =>
                {
                    response.ListProjectScope.Add(new CategoryModel(item));
                });

                result.ListProjectType.ForEach(item =>
                {
                    response.ListProjectType.Add(new CategoryModel(item));
                });

                result.ListTargetType.ForEach(item =>
                {
                    response.ListTargetType.Add(new CategoryModel(item));
                });
                result.ListTargetUnit.ForEach(item =>
                {
                    response.ListTargetUnit.Add(new CategoryModel(item));
                });

                result.ListContract.ForEach(item =>
                {
                    var obj = new ContractModel(item);
                    obj.ListDetail = item.ListDetail;
                    obj.ListCostDetail = item.ListCostDetail;
                    response.ListContract.Add(obj);
                });

                result.ListEmployee.ForEach(item =>
                {
                    response.ListEmployee.Add(new EmployeeModel(item));
                });

                result.ListCustomer.ForEach(item =>
                {
                    response.ListCustomer.Add(new CustomerModel(item));
                });
                result.ListProjectTarget.ForEach(item =>
                {
                    response.ListProjectTarget.Add(new ProjectTargetModel(item));
                });
                return response;
            }
            catch (Exception e)
            {
                return new GetMasterUpdateProjectResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public UpdateProjectResponse UpdateProject(UpdateProjectRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iProjectDataAccess.UpdateProject(parameter);
                var response = new UpdateProjectResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,

                    ProjectId = result.ProjectId
                };

                return response;
            }
            catch (Exception e)
            {
                return new UpdateProjectResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetProjectScopeResponse GetProjectScope(GetProjectScopeRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iProjectDataAccess.GetProjectScope(parameter);
                var response = new GetProjectScopeResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    Project = new ProjectModel(result.Project),
                    ListProjectScope = new List<Models.Project.ProjectScopeModel>(),
                    ListProjectTask = new List<TaskModel>(),
                    ListVendor = new List<VendorModel>(),
                    ListResource = new List<CategoryModel>(),
                    ListStatus = new List<CategoryModel>(),
                    ListNote = result.ListNote,
                    TotalRecordsNote = result.TotalRecordsNote,
                    ListResourceSope = new List<string>(),
                    listProject = result.listProject,
                };
                result.ListProjectScope.ForEach(item =>
                {
                    Models.Project.ProjectScopeModel obj = new Models.Project.ProjectScopeModel(item);
                    obj.ListEmployee = item.ListEmployee;
                    response.ListProjectScope.Add(obj);
                });
                result.ListProjectTask.ForEach(item =>
                {
                    response.ListProjectTask.Add(new TaskModel(item));
                });
                result.ListVendor.ForEach(item =>
                {
                    response.ListVendor.Add(new VendorModel(item));
                });
                result.ListResource.ForEach(item =>
                {
                    response.ListResource.Add(new CategoryModel(item));
                });
                result.ListStatus.ForEach(item =>
                {
                    response.ListStatus.Add(new CategoryModel(item));
                });

                return response;
            }
            catch (Exception e)
            {
                return new GetProjectScopeResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public UpdateProjectScopeResponse UpdateProjectScope(UpdateProjectScopeRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iProjectDataAccess.UpdateProjectScope(parameter);
                var response = new UpdateProjectScopeResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    ListProjectScope = new List<Models.Project.ProjectScopeModel>(),
                    ListNote = result.ListNote
                };
                if (result.ListProjectScope != null)
                {
                    result.ListProjectScope.ForEach(r =>
                    {
                        Models.Project.ProjectScopeModel obj = new Models.Project.ProjectScopeModel(r);
                        obj.ListEmployee = r.ListEmployee;
                        response.ListProjectScope.Add(obj);
                    }
               );
                }

                return response;
            }
            catch (Exception e)
            {
                return new UpdateProjectScopeResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetProjectResourceResponse GetProjectResource(GetProjectResourceRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iProjectDataAccess.GetProjectResource(parameter);
                var response = new GetProjectResourceResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    Project = new ProjectModel(result.Project),
                    ListProjectResource = new List<ProjectResourceModel>(),
                    ListPaymentMethod = result.ListPaymentMethod,
                    ListProjectTask = result.ListProjectTask,
                    ListNote = result.ListNote,
                    TotalRecordsNote = result.TotalRecordsNote,
                    listProject = result.listProject,
                };
                result.ListProjectResource.ForEach(r =>
                  {
                      ProjectResourceModel obj = new ProjectResourceModel(r);
                      obj.ListContact = r.ListContact;
                      obj.ListProjectVendor = r.ListProjectVendor;
                      obj.Vendor = r.Vendor;
                      response.ListProjectResource.Add(obj);
                  }
                );
                return response;
            }
            catch (Exception e)
            {
                return new GetProjectResourceResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetStatusResourceProjectResponse GetStatusResourceProject(GetStatusResourceProjectRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iProjectDataAccess.GetStatusResourceProject(parameter);
                var response = new GetStatusResourceProjectResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    ListEmployee = new List<EmployeeModel>(),
                    ListResourceRole = new List<CategoryModel>(),
                    ListResourceType = new List<CategoryModel>(),
                    ListVendor = new List<VendorModel>(),
                    ListVendorGroup = new List<CategoryModel>(),
                    ListResourceSource = new List<CategoryModel>(),
                    ListMachine = new List<CategoryModel>(),
                    ListOther = new List<CategoryModel>(),
                    ProjectResource = result.ProjectResource
                };
                result.ListEmployee.ForEach(e =>
                {
                    response.ListEmployee.Add(new EmployeeModel(e));
                });
                result.ListResourceRole.ForEach(e =>
                {
                    response.ListResourceRole.Add(new CategoryModel(e));
                });
                result.ListResourceType.ForEach(e =>
                {
                    response.ListResourceType.Add(new CategoryModel(e));
                });
                result.ListVendor.ForEach(e =>
                {
                    response.ListVendor.Add(new VendorModel(e));
                });
                result.ListVendorGroup.ForEach(e =>
                {
                    response.ListVendorGroup.Add(new CategoryModel(e));
                });
                result.ListResourceSource.ForEach(e =>
                {
                    response.ListResourceSource.Add(new CategoryModel(e));
                });
                if (result.ListMachine != null)
                {
                    result.ListMachine.ForEach(e =>
                    {
                        response.ListMachine.Add(new CategoryModel(e));
                    });
                }

                if (result.ListOther != null)
                {
                    result.ListOther.ForEach(e =>
                {
                    response.ListOther.Add(new CategoryModel(e));
                });
                }
                return response;
            }
            catch (Exception e)
            {
                return new GetStatusResourceProjectResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public CreateOrUpdateProjectResourceResponse CreateOrUpdateProjectResource(CreateOrUpdateProjectResourceRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iProjectDataAccess.CreateOrUpdateProjectResource(parameter);
                var response = new CreateOrUpdateProjectResourceResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    SendEmailEntityModel = result.SendEmailEntityModel,
                    ProjectResourceId = result.ProjectResourceId
                };

                return response;
            }
            catch (Exception e)
            {
                return new CreateOrUpdateProjectResourceResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetMasterMilestoneResponse GetMasterMilestone(GetMasterMilestoneRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iProjectDataAccess.GetMasterMilestone(parameter);
                var response = new GetMasterMilestoneResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    Project = new ProjectModel(),
                    ListMilestone = new List<ProjectMilestoneModel>(),
                };

                response.Project = new ProjectModel(result.Project);
                response.ListMilestone.Add(new ProjectMilestoneModel());
                result.ListMilestone.ForEach(mile =>
                {
                    response.ListMilestone.Add(new ProjectMilestoneModel(mile));
                });
                return response;
            }
            catch (Exception e)
            {
                return new GetMasterMilestoneResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        //GetAllTaskByProjectIdResponse GetAllTaskForMilestone(GetAllTaskByProjectIdRequest request)
        //{
        //    try
        //    {
        //        var parameter = request.ToParameter();
        //        var result = iProjectDataAccess.GetAllTaskForMilestone(parameter);
        //        var response = new GetAllTaskByProjectIdResponse()
        //        {
        //            StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
        //            MessageCode = result.Message,
        //            ListTask = new List<TaskEntityModel>(),
        //        };
        //        response.ListTask = result.ListTask;             
        //        return response;
        //    }
        //    catch (Exception e)
        //    {
        //        return new GetAllTaskByProjectIdResponse()
        //        {
        //            StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
        //            MessageCode = e.Message
        //        };
        //    }
        //}

        public UpdateProjectMilestoneResponse CreateOrUpdateProjectMilestone(UpdateProjectMilestoneRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iProjectDataAccess.CreateOrUpdateProjectMilestone(parameter);
                var response = new UpdateProjectMilestoneResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                };

                return response;
            }
            catch (Exception e)
            {
                return new UpdateProjectMilestoneResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetAllTaskByProjectIdResponse GetAllTaskByProjectScopeId(GetAllTaskByProjectScopeIdRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iProjectDataAccess.GetAllTaskByProjectScopeId(parameter);
                var response = new GetAllTaskByProjectIdResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    ListTask = new List<TaskModel>(),
                };
                result.ListTask.ForEach(task =>
                {
                    response.ListTask.Add(new TaskModel(task));
                });
                return response;
            }
            catch (Exception e)
            {
                return new GetAllTaskByProjectIdResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        //public CreateProjectScopeResponse CreateNewProjectScope(UpdateProjectScopeRequest request)
        //{
        //    try
        //    {
        //        var parameter = request.ToParameter();
        //        var result = iProjectDataAccess.CreateNewProjectScope(parameter);
        //        var response = new CreateProjectScopeResponse()
        //        {
        //            StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
        //            MessageCode = result.Message,
        //            ListProjectScope = new List<Models.Project.ProjectScopeModel>()
        //        };
        //        result.ListProjectScope.ForEach(scope =>
        //        {
        //            response.ListProjectScope.Add(new Models.Project.ProjectScopeModel(scope));
        //        });

        //        return response;
        //    }
        //    catch (Exception e)
        //    {
        //        return new CreateProjectScopeResponse()
        //        {
        //            StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
        //            MessageCode = e.Message
        //        };
        //    }
        //}

        public DeleteProjectResourceResponse DeleteProjectResource(DeleteProjectResourceRequest request)
        {
            try
            {
                this.logger.LogInformation("Delete ProjectResource");
                var parameter = request.ToParameter();
                var result = iProjectDataAccess.DeleteProjectResource(parameter);
                return new DeleteProjectResourceResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message
                };
            }
            catch (Exception ex)
            {
                return new DeleteProjectResourceResponse
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = ex.Message
                };
            }
        }
        public UpdateProjectVendorResponse UpdateProjectVendorResource(UpdateProjectVendorRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iProjectDataAccess.UpdateProjectVendorResource(parameter);
                var response = new UpdateProjectVendorResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                };

                return response;
            }
            catch (Exception e)
            {
                return new UpdateProjectVendorResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public DeleteProjectScopeResponse DeleteProjectScope(DeleteProjectScopeRequest request)
        {
            try
            {
                this.logger.LogInformation("Delete ProjectScope");
                var parameter = request.ToParameter();
                var result = iProjectDataAccess.DeleteProjectScope(parameter);
                return new DeleteProjectScopeResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    ListProjectScope = result.ListProjectScope,
                    ListNote = result.ListNote
                };
            }
            catch (Exception ex)
            {
                return new DeleteProjectScopeResponse
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = ex.Message
                };
            }
        }
        public CheckAllowcateProjectResourceResponse CheckAllowcateProjectResourceResult(CheckAllowcateProjectResourceRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iProjectDataAccess.CheckAllowcateProjectResource(parameter);
                if (result.TotalAllowcation > 100)
                {
                    return new CheckAllowcateProjectResourceResponse()
                    {
                        TotalAllowcation = result.TotalAllowcation,
                        StatusCode = System.Net.HttpStatusCode.OK,
                        MessageCode = result.Message,
                    };
                }
                else
                {
                    return new CheckAllowcateProjectResourceResponse()
                    {
                        TotalAllowcation = result.TotalAllowcation,
                        StatusCode = System.Net.HttpStatusCode.NotFound,
                        MessageCode = result.Message,
                    };
                }
            }
            catch (Exception e)
            {
                return new CheckAllowcateProjectResourceResponse
                {
                    MessageCode = e.Message,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public GetPermissionResponse GetPermission(GetPermissionRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iProjectDataAccess.GetPermission(parameter);

                return new GetPermissionResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.NotFound,
                    MessageCode = result.Message,
                    PermissionStr = result.PermissionStr,
                    ListPermission = result.ListPermission
                };
            }
            catch (Exception e)
            {
                return new GetPermissionResponse
                {
                    MessageCode = e.Message,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public GetMasterDataProjectMilestoneResponse GetMasterDataProjectMilestone(GetMasterDataProjectMilestoneRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iProjectDataAccess.GetMasterDataProjectMilestone(parameter);

                return new GetMasterDataProjectMilestoneResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.NotFound,
                    MessageCode = result.Message,
                    ListProjectMilestoneInProgress = result.ListProjectMilestoneInProgress,
                    ListProjectMilestoneComplete = result.ListProjectMilestoneComplete,
                    ProjectTaskComplete = result.ProjectTaskComplete,
                    TotalEstimateHour = result.TotalEstimateHour,
                    Project = result.Project,
                    ListNote = result.ListNote,
                    TotalRecordsNote = result.TotalRecordsNote,
                    ListProject = result.ListProject,
                };
            }
            catch (Exception e)
            {
                return new GetMasterDataProjectMilestoneResponse
                {
                    MessageCode = e.Message,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public GetMasterDataCreateOrUpdateMilestoneResponse GetMasterDataCreateOrUpdateMilestone(GetMasterDataCreateOrUpdateMilestoneRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iProjectDataAccess.GetMasterDataCreateOrUpdateMilestone(parameter);

                return new GetMasterDataCreateOrUpdateMilestoneResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.NotFound,
                    MessageCode = result.Message,
                    ListProject = result.ListProject,
                    ProjectMilestone = result.ProjectMilestone
                };
            }
            catch (Exception ex)
            {
                return new GetMasterDataCreateOrUpdateMilestoneResponse
                {
                    MessageCode = ex.Message,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };
            }
        }

        public CreateOrUpdateMilestoneResponse CreateOrUpdateMilestone(CreateOrUpdateMilestoneRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iProjectDataAccess.CreateOrUpdateMilestone(parameter);

                return new CreateOrUpdateMilestoneResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.NotFound,
                    MessageCode = result.Message,
                };
            }
            catch (Exception ex)
            {
                return new CreateOrUpdateMilestoneResponse
                {
                    MessageCode = ex.Message,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };
            }
        }

        public UpdateStatusProjectMilestoneResponse UpdateStatusProjectMilestone(UpdateStatusProjectMilestoneRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iProjectDataAccess.UpdateStatusProjectMilestone(parameter);

                return new UpdateStatusProjectMilestoneResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.NotFound,
                    MessageCode = result.Message,
                };
            }
            catch (Exception ex)
            {
                return new UpdateStatusProjectMilestoneResponse
                {
                    MessageCode = ex.Message,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };
            }
        }

        public GetMasterDataAddOrRemoveTaskToMilestoneResponse GetMasterDataAddOrRemoveTaskToMilestone(GetMasterDataAddOrRemoveTaskToMilestoneRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iProjectDataAccess.GetMasterDataAddOrRemoveTaskToMilestone(parameter);

                return new GetMasterDataAddOrRemoveTaskToMilestoneResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.NotFound,
                    MessageCode = result.Message,
                    ListTask = result.ListTask
                };
            }
            catch (Exception ex)
            {
                return new GetMasterDataAddOrRemoveTaskToMilestoneResponse
                {
                    MessageCode = ex.Message,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };
            }
        }

        public AddOrRemoveTaskMilestoneResponse AddOrRemoveTask(AddOrRemoveTaskMilestoneRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iProjectDataAccess.AddOrRemoveTaskMilestone(parameter);

                return new AddOrRemoveTaskMilestoneResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.NotFound,
                    MessageCode = result.Message,
                };
            }
            catch (Exception ex)
            {
                return new AddOrRemoveTaskMilestoneResponse
                {
                    MessageCode = ex.Message,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };
            }
        }

        public GetDataMilestoneByIdResponse GetDataMilestoneById(GetDataMilestoneByIdRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iProjectDataAccess.GetDataMilestoneById(parameter);

                return new GetDataMilestoneByIdResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.NotFound,
                    MessageCode = result.Message,
                    ListTaskClose = result.ListTaskClose,
                    ListTaskComplete = result.ListTaskComplete,
                    ListTaskNew = result.ListTaskNew,
                    ListTaskInProgress = result.ListTaskInProgress,
                    ListStatus = result.ListStatus,
                    IsContainResource = result.IsContainResource,
                    ProjectMilestone = result.ProjectMilestone
                };
            }
            catch (Exception ex)
            {
                return new GetDataMilestoneByIdResponse
                {
                    MessageCode = ex.Message,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };
            }
        }

        public GetMasterProjectDocumentResponse GetMasterProjectDocument(GetMasterProjectDocumentRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iProjectDataAccess.GetMasterProjectDocument(parameter);
                var response = new GetMasterProjectDocumentResponse()
                {
                    StatusCode = result.Status
                        ? System.Net.HttpStatusCode.OK
                        : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    ListTaskDocument = result.ListTaskDocument,
                    ListDocument = result.ListDocument,
                    ListFolders = result.ListFolders,
                    Project = result.Project,
                    TotalEstimateHour = result.TotalEstimateHour,
                    ProjectTaskComplete = result.ProjectTaskComplete,
                    TotalSize = result.TotalSize,
                    ListProject = result.ListProject,
                };

                return response;
            }
            catch (Exception e)
            {
                return new GetMasterProjectDocumentResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public GetCloneProjectScopeResponse GetMasterDataListCloneProjectScope()
        {
            try
            {
                var result = iProjectDataAccess.GetMasterDataListCloneProjectScope();

                var response = new GetCloneProjectScopeResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    ListProject = result.ListProject,
                    ListProjectScope = new List<Models.Project.ProjectScopeModel>(),
                };
                result.ListProjectScope.ForEach(item =>
                {
                    Models.Project.ProjectScopeModel obj = new Models.Project.ProjectScopeModel(item);
                    obj.ListEmployee = item.ListEmployee;
                    response.ListProjectScope.Add(obj);
                });

                return response;
            }
            catch (Exception ex)
            {
                return new GetCloneProjectScopeResponse
                {
                    MessageCode = ex.Message,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };
            }
        }

        public CloneProjectScopeResponse CloneProjectScope(GetCloneProjecScopetRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iProjectDataAccess.CloneProjectScope(parameter);

                return new CloneProjectScopeResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.NotFound,
                    MessageCode = result.Message,
                };
            }
            catch (Exception ex)
            {
                return new CloneProjectScopeResponse
                {
                    MessageCode = ex.Message,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };
            }
        }

        public SearchNoteResponse PagingProjectNote(SearchNoteRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iProjectDataAccess.PagingProjectNote(parameter);
                var response = new SearchNoteResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    NoteEntityList = result.NoteList,
                    TotalRecordsNote = result.TotalRecordsNote
                };

                return response;
            }
            catch (Exception e)
            {
                return new SearchNoteResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetMasterDataCommonDashboardProjectResponse GetMasterDataCommonDashboardProject(GetMasterDataCommonDashboardProjectRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iProjectDataAccess.GetMasterDataCommonDashboardProject(parameter);
                var response = new GetMasterDataCommonDashboardProjectResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    IsManager = result.IsManager,
                    ListTaskComplete = result.ListTaskComplete,
                    ListTaskOverdue = result.ListTaskOverdue,
                    Project = result.Project,
                    ProjectTaskComplete = result.ProjectTaskComplete,
                    TotalEstimateHour = result.TotalEstimateHour,
                    TotalHourUsed = result.TotalHourUsed,
                    ListProject = result.ListProject,
                    ListAllTask = result.ListAllTask,
                    ListEmployee = result.ListEmployee,
                    ListChartBudget = result.ListChartBudget,
                    TotalEE = result.TotalEE,
                };

                return response;
            }
            catch (Exception e)
            {
                return new GetMasterDataCommonDashboardProjectResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetDataDashboardProjectFollowManagerResponse GetDataDashboardProjectFollowManager(GetDataDashboardProjectFollowManagerRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iProjectDataAccess.GetDataDashboardProjectFollowManager(parameter);
                var response = new GetDataDashboardProjectFollowManagerResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    ListTaskFollowStatus = result.ListTaskFollowStatus,
                    ListTaskFollowTime = result.ListTaskFollowTime,
                    ListTaskFollowTaskType = result.ListTaskFollowTaskType,
                    ListTaskFollowResource = result.ListTaskFollowResource,
                    ListChartTimeFollowResource = result.ListChartTimeFollowResource
                };

                return response;
            }
            catch (Exception e)
            {
                return new GetDataDashboardProjectFollowManagerResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.BadRequest,
                    MessageCode = e.Message
                };
            }
        }

        public GetDataDashboardProjectFollowEmployeeResponse GetDataDashboardProjectFollowEmployee(
            GetDataDashboardProjectFollowEmployeeRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iProjectDataAccess.getDataDashboardProjectFollowEmployee(parameter);
                var response = new GetDataDashboardProjectFollowEmployeeResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    ListTaskFollowTime = result.ListTaskFollowTime,
                    ListProjectFollowResource = result.ListProjectFollowResource,
                };
                return response;
            }
            catch (Exception e)
            {
                return new GetDataDashboardProjectFollowEmployeeResponse
                {
                    StatusCode = System.Net.HttpStatusCode.BadRequest,
                    MessageCode = e.Message,
                };
            }
        }


        public GetDataEVNProjectDashboardResponse GetDataEVNProjectDashboard(GetDataEVNProjectDashboardRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iProjectDataAccess.GetGetDataEVNProjectDashboard(parameter);
                var response = new GetDataEVNProjectDashboardResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    ListChartEvn = result.ListChartEvn,
                    ListPerformanceCost = result.ListPerformanceCost
                };

                return response;
            }
            catch (Exception ex)
            {
                return new GetDataEVNProjectDashboardResponse
                {
                    MessageCode = ex.Message,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };
            }
        }

        public SynchronizedEvnResponse SynchronizedEvn(SynchronizedEvnRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iProjectDataAccess.SynchronizedEvn(parameter);
                var response = new SynchronizedEvnResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                };

                return response;
            }
            catch (Exception ex)
            {
                return new SynchronizedEvnResponse
                {
                    MessageCode = ex.Message,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };
            }
        }

        public GetMasterDataProjectInformationResponse getMasterDataProjectInformation(GetMasterDataProjectInformationRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iProjectDataAccess.GetMasterDataProjectInformation(parameter);
                var response = new GetMasterDataProjectInformationResponse()
                {
                    Project = result.Project,
                    ProjectTaskComplete = result.ProjectTaskComplete,
                    TotalEstimateHour = result.TotalEstimateHour,
                    TotalHourUsed = result.TotalHourUsed,
                    TotalEE = result.TotalEE,
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                };

                return response;
            }
            catch (Exception e)
            {
                return new GetMasterDataProjectInformationResponse()
                {
                    MessageCode = e.Message,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };
            }
        }
    }
}