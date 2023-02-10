using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TN.TNM.BusinessLogic.Interfaces.Project;
using TN.TNM.BusinessLogic.Messages.Requests.Note;
using TN.TNM.BusinessLogic.Messages.Requests.Project;
using TN.TNM.BusinessLogic.Messages.Requests.Task;
using TN.TNM.BusinessLogic.Messages.Responses.Note;
using TN.TNM.BusinessLogic.Messages.Responses.Project;
using TN.TNM.BusinessLogic.Messages.Responses.Task;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.Note;
using TN.TNM.DataAccess.Messages.Parameters.Project;
using TN.TNM.DataAccess.Messages.Parameters.Task;
using TN.TNM.DataAccess.Messages.Results.Note;
using TN.TNM.DataAccess.Messages.Results.Project;
using TN.TNM.DataAccess.Messages.Results.Task;

namespace TN.TNM.Api.Controllers
{
    public class ProjectController : Controller
    {
        public readonly IProject iProject;
        public readonly IProjectDataAccess iProjectDataAccess;

        public ProjectController(IProject _iProject, IProjectDataAccess _iProjectDataAccess)
        {
            this.iProject = _iProject;
            this.iProjectDataAccess = _iProjectDataAccess;
        }
        [HttpPost]
        [Route("api/project/getMasterProjectCreate")]
        [Authorize(Policy = "Member")]
        public GetMasterProjectResult GetMasterProjectCreate([FromBody] GetMasterProjectParameter request)
        {
            return this.iProjectDataAccess.GetMasterProjectCreate(request);
        }

        [HttpPost]
        [Route("api/project/searchProject")]
        [Authorize(Policy = "Member")]
        public SearchProjectResult SearchProject([FromBody] SearchProjectParameter request)
        {
          return this.iProjectDataAccess.SearchProject(request);
        }

        [HttpPost]
        [Route("api/project/createProject")]
        [Authorize(Policy = "Member")]
        public CreateProjectResult CreateProject([FromBody] CreateProjectParameter request)
        {
            return iProjectDataAccess.CreateProject(request);
        }

        [HttpPost]
        [Route("api/project/updateProjectStatus")]
        [Authorize(Policy = "Member")]
        public UpdateProjectStatusResult UpdateProjectStatus([FromBody] UpdateProjectStatusParameter request)
        {
            return this.iProjectDataAccess.UpdateProjectStatus(request);
        }

        [HttpPost]
        [Route("api/project/getMasterUpdateProjectCreate")]
        [Authorize(Policy = "Member")]
        public GetMasterUpdateProjectResult GetMasterUpdateProjectCreate([FromBody] GetMasterUpdateProjectParameter request)
        {
            return this.iProjectDataAccess.GetMasterUpdateProjectCreate(request);
        }

        [HttpPost]
        [Route("api/project/updateProject")]
        [Authorize(Policy = "Member")]
        public UpdateProjectResult UpdateProject([FromBody] UpdateProjectParameter request)
        {
            return iProjectDataAccess.UpdateProject(request);
        }

        [HttpPost]
        [Route("api/project/getProjectScope")]
        [Authorize(Policy = "Member")]
        public GetProjectScopeResult GetProjectScope([FromBody] GetProjectScopeParameter request)
        {
            return this.iProjectDataAccess.GetProjectScope(request);
        }

        //[HttpPost]
        //[Route("api/project/updateProjectScope")]
        //[Authorize(Policy = "Member")]
        //public UpdateProjectScopeResponse UpdateProjectScope([FromBody] UpdateProjectScopeRequest request)
        //{
        //    return iProject.UpdateProjectScope(request);
        //}
        [HttpPost]
        [Route("api/project/updateProjectScope")]
        [Authorize(Policy = "Member")]
        public UpdateProjectScopeResult UpdateProjectScope([FromBody] UpdateProjectScopeParameter request)
        {
            return iProjectDataAccess.UpdateProjectScope(request);
        }

        [HttpPost]
        [Route("api/project/getProjectResource")]
        [Authorize(Policy = "Member")]
        public GetProjectResourceResult GetProjectResource([FromBody] GetProjectResourceParameter request)
        {
            return this.iProjectDataAccess.GetProjectResource(request);
        }

        [HttpPost]
        [Route("api/project/getStatusResourceProject")]
        [Authorize(Policy = "Member")]
        public GetStatusResourceProjectResult GetStatusResourceProject([FromBody] GetStatusResourceProjectParameter request)
        {
            return this.iProjectDataAccess.GetStatusResourceProject(request);
        }


        //[HttpPost]
        //[Route("api/project/getMasterMilestone")]
        //[Authorize(Policy = "Member")]
        //public GetMasterMilestoneResponse GetMasterMilestone([FromBody] GetMasterMilestoneRequest request)
        //{
        //    return iProject.GetMasterMilestone(request);
        //}

        //[HttpPost]
        //[Route("api/project/getAllTaskForMilestone")]
        //[Authorize(Policy = "Member")]
        //public GetAllTaskByProjectIdResponse GetAllTaskForMilestone(GetAllTaskByProjectIdRequest request)
        //{
        //    return iProject.GetAllTaskForMilestone(request);
        //}

        [HttpPost]
        [Route("api/project/createOrUpdateProjectMilestone")]
        [Authorize(Policy = "Member")]
        public UpdateProjectMilestoneResult CreateOrUpdateProjectMilestone(UpdateProjectMilestoneParameter request)
        {
            return iProjectDataAccess.CreateOrUpdateProjectMilestone(request);
        }

        [HttpPost]
        [Route("api/project/getAllTaskByProjectScopeId")]
        [Authorize(Policy = "Member")]
        public GetAllTaskByProjectScopeIdResult GetAllTaskByProjectScopeId([FromBody] GetAllTaskByProjectIdParameter request)
        {
            return iProjectDataAccess.GetAllTaskByProjectScopeId(request);
        }

        //[HttpPost]
        //[Route("api/project/createNewProjectScope")]
        //[Authorize(Policy = "Member")]
        //public CreateProjectScopeResponse CreateNewProjectScope(UpdateProjectScopeRequest request)
        //{
        //    return iProject.CreateNewProjectScope(request);
        //}


        [HttpPost]
        [Route("api/project/createOrUpdateProjectResource")]
        [Authorize(Policy = "Member")]
        public CreateOrUpdateProjectResourceResult CreateOrUpdateProjectResource ([FromBody] CreateOrUpdateProjectResourceParameter request)
        {
            return this.iProjectDataAccess.CreateOrUpdateProjectResource(request);
        }

        [HttpPost]
        [Route("api/project/deleteProjectResource")]
        [Authorize(Policy = "Member")]
        public DeleteProjectResourceResult DeleteProjectResource([FromBody] DeleteProjectResourceParameter request)
        {
            return this.iProjectDataAccess.DeleteProjectResource(request);
        }

        [HttpPost]
        [Route("api/project/updateProjectVendorResource")]
        [Authorize(Policy = "Member")]
        public UpdateProjectVendorResult UpdateProjectVendorResource([FromBody] UpdateProjectVendorParameter request)
        {
             return this.iProjectDataAccess.UpdateProjectVendorResource(request);
        }

        [HttpPost]
        [Route("api/project/deleteProjectScope")]
        [Authorize(Policy = "Member")]
        public DeleteProjectScopeResult DeleteProjectScope([FromBody] DeleteProjectScopeParameter request)
        {
            return this.iProjectDataAccess.DeleteProjectScope(request);
        }

        [HttpPost]
        [Route("api/project/checkAllowcateProjectResource")]
        [Authorize(Policy = "Member")]
        public CheckAllowcateProjectResourceResult CheckAllowcateProjectResourceResult([FromBody] CheckAllowcateProjectResourceParameter request)
        {
            return this.iProjectDataAccess.CheckAllowcateProjectResource(request);
        }

        [HttpPost]
        [Route("api/project/getPermission")]
        [Authorize(Policy = "Member")]
        public GetPermissionResult GetPermission([FromBody] GetPermissionParameter request)
        {
            return this.iProjectDataAccess.GetPermission(request);
        }

        [HttpPost]
        [Route("api/project/getMasterDataProjectMilestone")]
        [Authorize(Policy = "Member")]
        public GetMasterDataProjectMilestoneResult GetMasterDataProjectMilestone([FromBody] GetMasterDataProjectMilestoneParameter request)
        {
            return this.iProjectDataAccess.GetMasterDataProjectMilestone(request);
        }

        [HttpPost]
        [Route("api/project/getMasterDataCreateOrUpdateMilestone")]
        [Authorize(Policy = "Member")]
        public GetMasterDataCreateOrUpdateMilestoneResult GetMasterDataCreateOrUpdateMilestone([FromBody] GetMasterDataCreateOrUpdateMilestoneParameter request)
        {
            return this.iProjectDataAccess.GetMasterDataCreateOrUpdateMilestone(request);
        }

        [HttpPost]
        [Route("api/project/createOrUpdateMilestone")]
        [Authorize(Policy = "Member")]
        public CreateOrUpdateMilestoneResult CreateOrUpdateMilestone([FromBody] CreateOrUpdateMilestoneParameter request)
        {
            return this.iProjectDataAccess.CreateOrUpdateMilestone(request);
        }

        [HttpPost]
        [Route("api/project/updateStatusProjectMilestone")]
        [Authorize(Policy = "Member")]
        public UpdateStatusProjectMilestoneResult UpdateStatusProjectMilestone([FromBody] UpdateStatusProjectMilestoneParameter request)
        {
            return this.iProjectDataAccess.UpdateStatusProjectMilestone(request);
        }

        [HttpPost]
        [Route("api/project/getMasterDataAddOrRemoveTaskToMilestone")]
        [Authorize(Policy = "Member")]
        public GetMasterDataAddOrRemoveTaskToMilestoneResult GetMasterDataAddOrRemoveTaskToMilestone([FromBody] GetMasterDataAddOrRemoveTaskToMilestoneParameter request)
        {
            return this.iProjectDataAccess.GetMasterDataAddOrRemoveTaskToMilestone(request);
        }

        [HttpPost]
        [Route("api/project/addOrRemoveTaskMilestone")]
        [Authorize(Policy = "Member")]
        public AddOrRemoveTaskMilestoneResult AddOrRemoveTaskMilestone([FromBody] AddOrRemoveTaskMilestoneParameter request)
        {
            return this.iProjectDataAccess.AddOrRemoveTaskMilestone(request);
        }

        [HttpPost]
        [Route("api/project/getDataMilestoneById")]
        [Authorize(Policy = "Member")]
        public GetDataMilestoneByIdResult GetDataMilestoneById([FromBody] GetDataMilestoneByIdParameter request)
        {
            return this.iProjectDataAccess.GetDataMilestoneById(request);
        }

        [HttpPost]
        [Route("api/project/getMasterProjectDocument")]
        [Authorize(Policy = "Member")]
        public GetMasterProjectDocumentResult GetMasterProjectDocument([FromBody] GetMasterProjectDocumentParameter request)
        {
            return this.iProjectDataAccess.GetMasterProjectDocument(request);
        }

        [HttpPost]
        [Route("api/project/getMasterDataListCloneProjectScope")]
        [Authorize(Policy = "Member")]
        public GetCloneProjectScopeResult GetMasterDataListCloneProjectScope()
        {
            return this.iProjectDataAccess.GetMasterDataListCloneProjectScope();
        }

        [HttpPost]
        [Route("api/project/cloneProjectScope")]
        [Authorize(Policy = "Member")]
        public CloneProjectScopeResult CloneProjectScope([FromBody] GetCloneProjectScopeParameter request)
        {
            return this.iProjectDataAccess.CloneProjectScope(request);
        }

        [HttpPost]
        [Route("api/project/pagingProjectNote")]
        [Authorize(Policy = "Member")]
        public SearchNoteResult PagingProjectNote([FromBody] SearchNoteParameter request)
        {
            return this.iProjectDataAccess.PagingProjectNote(request);
        }

        [HttpPost]
        [Route("api/project/getMasterDataCommonDashboardProject")]
        [Authorize(Policy = "Member")]
        public GetMasterDataCommonDashboardProjectResult GetMasterDataCommonDashboardProject([FromBody] GetMasterDataCommonDashboardProjectParameter request)
        {
            return this.iProjectDataAccess.GetMasterDataCommonDashboardProject(request);
        }

        [HttpPost]
        [Route("api/project/getDataDashboardProjectFollowManager")]
        [Authorize(Policy = "Member")]
        public GetDataDashboardProjectFollowManagerResult GetDataDashboardProjectFollowManager([FromBody] GetDataDashboardProjectFollowManagerParameter request)
        {
            return this.iProjectDataAccess.GetDataDashboardProjectFollowManager(request);
        }

        [HttpPost]
        [Route("api/project/getDataDashboardProjectFollowEmployee")]
        [Authorize(Policy = "Member")]
        public GetDataDashboardProjectFollowEmployeeResult GetDataDashboardProjectFollowEmployee([FromBody] GetDataDashboardProjectFollowEmployeeParameter request)
        {
            return this.iProjectDataAccess.getDataDashboardProjectFollowEmployee(request);
        }

        [HttpPost]
        [Route("api/project/getDataEVNProjectDashboard")]
        [Authorize(Policy = "Member")]
        public GetDataEVNProjectDashboardResult GetDataEVNProjectDashboard([FromBody] GetDataEVNProjectDashboardParameter request)
        {
            return this.iProjectDataAccess.GetGetDataEVNProjectDashboard(request);
        }

        [HttpPost]
        [Route("api/project/synchronizedEvn")]
        [Authorize(Policy = "Member")]
        public SynchronizedEvnResult SynchronizedEvn([FromBody] SynchronizedEvnParameter request)
        {
            return this.iProjectDataAccess.SynchronizedEvn(request);
        }

        [HttpPost]
        [Route("api/project/getMasterDataProjectInformation")]
        [Authorize(Policy = "Member")]
        public GetMasterDataProjectInformationResult GetMasterDataProjectInformation([FromBody] GetMasterDataProjectInformationParameter request)
        {
            return this.iProjectDataAccess.GetMasterDataProjectInformation(request);
        }

        [HttpPost]
        [Route("api/project/getBaoCaoSuDungNguonLuc")]
        [Authorize(Policy = "Member")]
        public GetBaoCaoSuDungNguonLucResult GetBaoCaoSuDungNguonLuc([FromBody] GetBaoCaoSuDungNguonLucParameter request)
        {
            return this.iProjectDataAccess.GetBaoCaoSuDungNguonLuc(request);
        }
        
        [HttpPost]
        [Route("api/project/getMasterDataBaoCaoSuDungNguonLuc")]
        [Authorize(Policy = "Member")]
        public GetMasterDataBaoCaoSuDungNguonLucResult GetMasterDataBaoCaoSuDungNguonLuc([FromBody] GetMasterDataBaoCaoSuDungNguonLucParameter request)
        {
            return this.iProjectDataAccess.GetMasterDataBaoCaoSuDungNguonLuc(request);
        }

        [HttpPost]
        [Route("api/project/getBaoCaoTongHopCacDuAn")]
        [Authorize(Policy = "Member")]
        public GetBaoCaoTongHopCacDuAnResult GetBaoCaoTongHopCacDuAn([FromBody] GetBaoCaoTongHopCacDuAnParameter request)
        {
            return this.iProjectDataAccess.GetBaoCaoTongHopCacDuAn(request);
        }

        //
        [HttpPost]
        [Route("api/project/getThoiGianUocLuongHangMuc")]
        [Authorize(Policy = "Member")]
        public GetThoiGianUocLuongHangMucResult GetThoiGianUocLuongHangMuc([FromBody] GetThoiGianUocLuongHangMucParameter request)
        {
            return this.iProjectDataAccess.GetThoiGianUocLuongHangMuc(request);
        }

        //
        [HttpPost]
        [Route("api/project/getPhanBoTheoNguonLuc")]
        [Authorize(Policy = "Member")]
        public GetPhanBoTheoNguonLucResult GetPhanBoTheoNguonLuc([FromBody] GetPhanBoTheoNguonLucParameter request)
        {
            return this.iProjectDataAccess.GetPhanBoTheoNguonLuc(request);
        }
    }
}
