using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TN.TNM.BusinessLogic.Interfaces.Task;
using TN.TNM.BusinessLogic.Messages.Requests.Task;
using TN.TNM.BusinessLogic.Messages.Responses.Task;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.Task;
using TN.TNM.DataAccess.Messages.Results.Task;

namespace TN.TNM.Api.Controllers
{
    public class TaskController : Controller
    {
        private readonly ITasks _iTask;
        private readonly ITaskDataAccess _iTaskDataAccess;
        public TaskController(ITasks iTask, ITaskDataAccess iTaskDataAccess)
        {
            this._iTask = iTask;
            this._iTaskDataAccess = iTaskDataAccess;
        }

        [HttpPost]
        [Route("api/task/getMasterDataCreateOrUpdateTask")]
        [Authorize(Policy = "Member")]
        public GetMasterDataCreateOrUpdateTaskResult GetMasterDataCreateOrUpdateTask([FromBody] GetMasterDataCreateOrUpdateTaskParameter request)
        {
            return this._iTaskDataAccess.GetMasterDataCreateOrUpdateTask(request);
        }

        [HttpPost]
        [Route("api/task/getProjectScopeByProjectId")]
        [Authorize(Policy = "Member")]
        public GetProjectScopeByProjectIdResult GetProjectScopeByProjectId([FromBody]GetProjectScopeByProjectIdParameter request)
        {
            return this._iTaskDataAccess.GetProjectScopeByProjectId(request);
        }

        // [HttpPost]
        // [Route("api/task/createOrUpdateTask")]
        // [Authorize(Policy = "Member")]
        // public CreateOrUpdateTaskResponse CreateOrUpdateTask([FromBody] CreateOrUpdateTaskRequest request)
        // {
        //     return this._iTask.CreateOrUpdateTask(request);
        // }
        
        [HttpPost]
        [Route("api/task/createOrUpdateTask")]
        [Authorize(Policy = "Member")]
        public CreateOrUpdateTaskResult CreateOrUpdateTask([FromForm] CreateOrUpdateTaskParameter request)
        {
            return this._iTaskDataAccess.CreateOrUpdateTask(request);
        }


        [HttpPost]
        [Route("api/task/getMasterDataTimeSheet")]
        [Authorize(Policy = "Member")]
        public GetMasterDataTimeSheetResult GetMasterDataTimeSheet([FromBody] GetMasterDataTimeSheetParameter request)
        {
            return this._iTaskDataAccess.GetMasterDataTimeSheet(request);
        }

        [HttpPost]
        [Route("api/task/getMasterDataSearchTask")]
        [Authorize(Policy = "Member")]
        public GetMasterDataSearchTaskResult GetMasterDataSearchTask([FromBody] GetMasterDataSearchTaskParameter request)
        {
            return this._iTaskDataAccess.GetMasterDataSearchTask(request);
        }

        [HttpPost]
        [Route("api/task/searchTask")]
        [Authorize(Policy = "Member")]
        public SearchTaskResult SearchTask([FromBody] SearchTaskParameter request)
        {
            return this._iTaskDataAccess.SearchTask(request);
        }

        [HttpPost]
        [Route("api/task/createOrUpdateTimeSheet")]
        [Authorize(Policy = "Member")]
        public CreateOrUpdateTimeSheetResult CreateOrUpdateTimeSheet([FromBody] CreateOrUpdateTimeSheetParameter request)
        {
            return this._iTaskDataAccess.CreateOrUpdateTimeSheet(request);
        }

        [HttpPost]
        [Route("api/task/changeStatusTask")]
        [Authorize(Policy = "Member")]
        public ChangeStatusTaskResult ChangeStatusTask([FromBody] ChangeStatusTaskParameter request)
        {
            return this._iTaskDataAccess.ChangeStatusTask(request);
        }

        [HttpPost]
        [Route("api/task/deleteTask")]
        [Authorize(Policy = "Member")]
        public DeleteTaskResult DeleteTask([FromBody] DeleteTaskParameter request)
        {
            return this._iTaskDataAccess.DeleteTask(request);
        }

   
        [HttpPost]
        [Route("api/task/searchTaskFromProjectScope")]
        [Authorize(Policy = "Member")]
        public SearchTaskResult SearchTaskFromProjectScope([FromBody] SearchTaskParameter request)
        {
            return this._iTaskDataAccess.SearchTaskFromProjectScope(request);
        }

        [HttpPost]
        [Route("api/task/getMasterDataCreateConstraint")]
        [Authorize(Policy = "Member")]
        public GetMasterDataCreateConstraintResult GetMasterDataCreateConstraint([FromBody] GetMasterDataCreateConstraintParameter request)
        {
            return this._iTaskDataAccess.GetMasterDataCreateConstraint(request);
        }

        [HttpPost]
        [Route("api/task/createConstraintTask")]
        [Authorize(Policy = "Member")]
        public CreateConstraintTaskResult CreateConstraintTask([FromBody] CreateConstraintTaskParameter request)
        {
            return this._iTaskDataAccess.CreateConstraintTask(request);
        }

        [HttpPost]
        [Route("api/task/updateRequiredConstrant")]
        [Authorize(Policy = "Member")]
        public UpdateRequiredConstrantResult UpdateRequiredConstrant([FromBody] UpdateRequiredConstrantParameter request)
        {
            return this._iTaskDataAccess.UpdateRequiredConstrant(request);
        }


        [HttpPost]
        [Route("api/task/deleteTaskConstraint")]
        [Authorize(Policy = "Member")]
        public DeleteTaskConstraintResult DeleteTaskConstraint([FromBody] DeleteTaskConstraintParameter request)
        {
            return this._iTaskDataAccess.DeleteTaskConstraint(request);
        }
         
        [HttpPost]
        [Route("api/task/deleteRelateTask")]
        [Authorize(Policy = "Member")]
        public DeleteRelateTaskResult DeleteRelateTask([FromBody] DeleteRelateTaskParameter request)
        {
            return this._iTaskDataAccess.DeleteRelateTask(request);
        }


        [HttpPost]
        [Route("api/task/getMasterDataSearchTimeSheet")]
        [Authorize(Policy = "Member")]
        public GetMasterDataSearchTimeSheetResult GetMasterDataSearchTimeSheet([FromBody] GetMasterDataSearchTimeSheetParameter request)
        {
            return this._iTaskDataAccess.GetMasterDataSearchTimeSheet(request);
        }

        [HttpPost]
        [Route("api/task/searchTimeSheet")]
        [Authorize(Policy = "Member")]
        public SearchTimeSheetResult SearchTimeSheet([FromBody] SearchTimeSheetParameter request)
        {
            return this._iTaskDataAccess.SearchTimeSheet(request);
        }


        [HttpPost]
        [Route("api/task/updateStatusTimeSheet")]
        [Authorize(Policy = "Member")]
        public UpdateStatusTimeSheetResult UpdateStatusTimeSheet([FromBody] UpdateStatusTimeSheetParameter request)
        {
            return this._iTaskDataAccess.UpdateStatusTimeSheet(request);
        }

        [HttpPost]
        [Route("api/task/acceptOrRejectByDay")]
        [Authorize(Policy = "Member")]
        public AcceptOrRejectByDayResult AcceptOrRejectByDay([FromBody] AcceptOrRejectByDayParameter request)
        {
            return this._iTaskDataAccess.AcceptOrRejectByDay(request);
        }

        [HttpPost]
        [Route("api/task/GetMasterDataCreateRelateTask")]
        [Authorize(Policy = "Member")]
        public GetMasterDataCreateRelateTaskResult GetMasterDataCreateRelateTask([FromBody] GetMasterDataCreateRelateTaskParameter request)
        {
            return this._iTaskDataAccess.GetMasterDataCreateRelateTask(request);
        }



        [HttpPost]
        [Route("api/task/CreateRelateTask")]
        [Authorize(Policy = "Member")]
        public CreateRelateTaskResult CreateRelateTask([FromBody] CreateRelateTaskParameter request)
        {
            return this._iTaskDataAccess.CreateRelateTask(request);
        }
    }
}
