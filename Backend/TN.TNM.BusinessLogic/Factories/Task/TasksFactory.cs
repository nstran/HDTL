using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Interfaces.Task;
using TN.TNM.BusinessLogic.Messages.Requests.Task;
using TN.TNM.BusinessLogic.Messages.Responses.Task;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.Task;
using TN.TNM.DataAccess.Messages.Results.Task;

namespace TN.TNM.BusinessLogic.Factories.Task
{
    public class TasksFactory : BaseFactory, ITasks
    {
        private ITaskDataAccess _iTaskDataAccess;

        public TasksFactory(ITaskDataAccess iTaskDataAccess, ILogger<TasksFactory> _logger)
        {
            _iTaskDataAccess = iTaskDataAccess;
            this.logger = _logger;
        }

        public CreateOrUpdateTaskResponse CreateOrUpdateTask(CreateOrUpdateTaskRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = _iTaskDataAccess.CreateOrUpdateTask(parameter);
                var response = new CreateOrUpdateTaskResponse()
                {
                    TaskId = result.TaskId,
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                };

                return response;
            }
            catch (Exception e)
            {
                return new CreateOrUpdateTaskResponse()
                {
                    MessageCode = e.Message,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public GetMasterDataCreateOrUpdateTaskResponse GetMasterDataCreateOrUpdateTask(GetMasterDataCreateOrUpdateTaskRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = _iTaskDataAccess.GetMasterDataCreateOrUpdateTask(parameter);
                var response = new GetMasterDataCreateOrUpdateTaskResponse()
                {
                    ListStatus = result.ListStatus,
                    ListTaskType = result.ListTaskType,
                    ListMilestone = result.ListMilestone,
                    ListProjectResource = result.ListProjectResource,
                    ListTaskDocument = result.ListTaskDocument,
                    Project = result.Project,
                    ListNote = result.ListNote,
                    Task = result.Task,
                    Scope = result.Scope,
                    ListTaskConstraintBefore = result.ListTaskConstraintBefore,
                    ListTaskConstraintAfter = result.ListTaskConstraintAfter,
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    TotalRecordsNote = result.TotalRecordsNote,
                    listProject = result.listProject,
                    IsManager = result.IsManager,
                    IsPresident = result.IsPresident,
                };

                return response;
            }
            catch (Exception e)
            {
                return new GetMasterDataCreateOrUpdateTaskResponse()
                {
                    MessageCode = e.Message,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public GetProjectScopeByProjectIdResponse GetProjectScopeByProjectId(GetProjectScopeByProjectIdRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = _iTaskDataAccess.GetProjectScopeByProjectId(parameter);
                var response = new GetProjectScopeByProjectIdResponse()
                {
                    ListProjectScrope = result.ListProjectScrope,
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                };

                return response;
            }
            catch (Exception e)
            {
                return new GetProjectScopeByProjectIdResponse()
                {
                    MessageCode = e.Message,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public GetMasterDataTimeSheetResponse GetMasterDataTimeSheet(GetMasterDataTimeSheetRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = _iTaskDataAccess.GetMasterDataTimeSheet(parameter);
                var response = new GetMasterDataTimeSheetResponse()
                {
                    ListTimeType = result.ListTimeType,
                    ListStatus = result.ListStatus,
                    ListTask = result.ListTask,
                    Employee = result.Employee,
                    Project = result.Project,
                    FromDate = result.FromDate,
                    ToDate = result.ToDate,
                    ListTimeSheet = result.ListTimeSheet,
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                };

                return response;
            }
            catch (Exception e)
            {
                return new GetMasterDataTimeSheetResponse()
                {
                    MessageCode = e.Message,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public GetMasterDataSearchTaskResponse GetMasterDataSearchTask(GetMasterDataSearchTaskRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = _iTaskDataAccess.GetMasterDataSearchTask(parameter);
                var response = new GetMasterDataSearchTaskResponse()
                {
                    ListTaskType = result.ListTaskType,
                    ListStatus = result.ListStatus,
                    ListPersionInCharged = result.ListPersionInCharged,
                    InforExportExcel = result.InforExportExcel,
                    Project = result.Project,
                    ProjectTaskComplete = result.ProjectTaskComplete,
                    TotalEstimateHour = result.TotalEstimateHour,
                    Employee = result.Employee,
                    IsContainResource = result.IsContainResource,
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    listProject = result.listProject,
                };

                return response;
            }
            catch (Exception e)
            {
                return new GetMasterDataSearchTaskResponse()
                {
                    MessageCode = e.Message,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public SearchTaskResponse SearchTask(SearchTaskRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = _iTaskDataAccess.SearchTask(parameter);
                var response = new SearchTaskResponse()
                {
                    ListTask = result.ListTask,
                    NumberTaskNew = result.NumberTaskNew,
                    NumberTaskDL = result.NumberTaskDL,
                    NumberTaskHT = result.NumberTaskHT,
                    NumberTaskClose = result.NumberTaskClose,
                    NumberTotalTask = result.NumberTotalTask,
                    ProjectTaskComplete = result.ProjectTaskComplete,
                    TotalEstimateHour = result.TotalEstimateHour,
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                };

                return response;
            }
            catch (Exception e)
            {
                return new SearchTaskResponse()
                {
                    MessageCode = e.Message,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public CreateOrUpdateTimeSheetResponse CreateOrUpdateTimeSheet(CreateOrUpdateTimeSheetRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = _iTaskDataAccess.CreateOrUpdateTimeSheet(parameter);
                var response = new CreateOrUpdateTimeSheetResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    TimeSheetId = result.TimeSheetId,
                };

                return response;
            }
            catch (Exception e)
            {
                return new CreateOrUpdateTimeSheetResponse()
                {
                    MessageCode = e.Message,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };
            }
        }

        public ChangeStatusTaskResponse ChangeStatusTask(ChangeStatusTaskRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = _iTaskDataAccess.ChangeStatusTask(parameter);
                var response = new ChangeStatusTaskResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                };

                return response;
            }
            catch (Exception e)
            {
                return new ChangeStatusTaskResponse()
                {
                    MessageCode = e.Message,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };
            }
        }

        public DeleteTaskResponse DeleteTask(DeleteTaskRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = _iTaskDataAccess.DeleteTask(parameter);
                var response = new DeleteTaskResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                };

                return response;
            }
            catch (Exception e)
            {
                return new DeleteTaskResponse()
                {
                    MessageCode = e.Message,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };
            }
        }

        public SearchTaskFromProjectScopeResponse SearchTaskFromProjectScope(SearchTaskFromProjectScopeRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = _iTaskDataAccess.SearchTaskFromProjectScope(parameter);
                var response = new SearchTaskFromProjectScopeResponse()
                {
                    ListProjectScrope = result.ListTask,
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                };

                return response;
            }
            catch (Exception e)
            {
                return new SearchTaskFromProjectScopeResponse()
                {
                    MessageCode = e.Message,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public GetMasterDataCreateConstraintResponse GetMasterDataCreateConstraint(GetMasterDataCreateConstraintRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = _iTaskDataAccess.GetMasterDataCreateConstraint(parameter);
                var response = new GetMasterDataCreateConstraintResponse()
                {
                    ListConstraint = result.ListConstraint,
                    ListTask = result.ListTask,
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                };

                return response;
            }
            catch (Exception e)
            {
                return new GetMasterDataCreateConstraintResponse()
                {
                    MessageCode = e.Message,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public CreateConstraintTaskResponse CreateConstraintTask(CreateConstraintTaskRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = _iTaskDataAccess.CreateConstraintTask(parameter);
                var response = new CreateConstraintTaskResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                };

                return response;
            }
            catch (Exception e)
            {
                return new CreateConstraintTaskResponse()
                {
                    MessageCode = e.Message,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };
            }
        }

        public UpdateRequiredConstrantResponse UpdateRequiredConstrant(UpdateRequiredConstrantRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = _iTaskDataAccess.UpdateRequiredConstrant(parameter);
                var response = new UpdateRequiredConstrantResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                };

                return response;
            }
            catch (Exception e)
            {
                return new UpdateRequiredConstrantResponse()
                {
                    MessageCode = e.Message,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };
            }
        }

        public DeleteTaskConstraintResponse DeleteTaskConstraint(DeleteTaskConstraintRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = _iTaskDataAccess.DeleteTaskConstraint(parameter);
                var response = new DeleteTaskConstraintResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                };

                return response;
            }
            catch (Exception e)
            {
                return new DeleteTaskConstraintResponse()
                {
                    MessageCode = e.Message,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };
            }
        }

        public GetMasterDataSearchTimeSheetResponse GetMasterDataSearchTimeSheet(GetMasterDataSearchTimeSheetRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = _iTaskDataAccess.GetMasterDataSearchTimeSheet(parameter);
                var response = new GetMasterDataSearchTimeSheetResponse()
                {
                    ListEmployee = result.ListEmployee,
                    ListStatus = result.ListStatus,
                    ListTimeStyle = result.ListTimeStyle,
                    Project = result.Project,
                    InforExportExcel = result.InforExportExcel,
                    ProjectTaskComplete = result.ProjectTaskComplete,
                    TotalEstimateHour = result.TotalEstimateHour,
                    FromDate = result.FromDate,
                    ToDate = result.ToDate,
                    IsRoot = result.IsRoot,
                    TotalHourUsed = result.TotalHourUsed,
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    ListProject = result.ListProject,
                };

                return response;
            }
            catch (Exception e)
            {
                return new GetMasterDataSearchTimeSheetResponse()
                {
                    MessageCode = e.Message,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };
            }
        }

        public SearchTimeSheetResponse SearchTimeSheet(SearchTimeSheetRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = _iTaskDataAccess.SearchTimeSheet(parameter);
                var response = new SearchTimeSheetResponse()
                {
                    ListTimeSheet = result.ListTimeSheet,
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                };

                return response;
            }
            catch (Exception e)
            {
                return new SearchTimeSheetResponse()
                {
                    MessageCode = e.Message,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };
            }
        }

        public UpdateStatusTimeSheetResponse UpdateStatusTimeSheet(UpdateStatusTimeSheetRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = _iTaskDataAccess.UpdateStatusTimeSheet(parameter);
                var response = new UpdateStatusTimeSheetResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                };

                return response;
            }
            catch (Exception e)
            {
                return new UpdateStatusTimeSheetResponse()
                {
                    MessageCode = e.Message,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };
            }
        }


        public AcceptOrRejectByDayResponse AcceptOrRejectByDay(AcceptOrRejectByDayRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = _iTaskDataAccess.AcceptOrRejectByDay(parameter);
                var response = new AcceptOrRejectByDayResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                };

                return response;
            }
            catch (Exception e)
            {
                return new AcceptOrRejectByDayResponse()
                {
                    MessageCode = e.Message,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };
            }
        }
    }
}
