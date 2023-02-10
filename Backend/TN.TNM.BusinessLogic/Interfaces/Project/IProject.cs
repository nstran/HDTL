using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Messages.Requests.Note;
using TN.TNM.BusinessLogic.Messages.Requests.Project;
using TN.TNM.BusinessLogic.Messages.Requests.Task;
using TN.TNM.BusinessLogic.Messages.Responses.Note;
using TN.TNM.BusinessLogic.Messages.Responses.Project;
using TN.TNM.BusinessLogic.Messages.Responses.Task;

namespace TN.TNM.BusinessLogic.Interfaces.Project
{
    public interface IProject
    {
        GetMasterProjectResponse GetMasterProjectCreate(GetMasterProjectRequest request);
        SearchProjectResponse SearchProject(SearchProjectRequest request);
        CreateProjectResponse CreateProject(CreateProjectRequest request);
        UpdateProjectStatusResponse UpdateProjectStatus(UpdateProjectStatusRequest request);
        GetMasterUpdateProjectResponse GetMasterUpdateProjectCreate(GetMasterUpdateProjectRequest request);
        UpdateProjectResponse UpdateProject(UpdateProjectRequest request);
        GetProjectScopeResponse GetProjectScope(GetProjectScopeRequest request);
        UpdateProjectScopeResponse UpdateProjectScope(UpdateProjectScopeRequest request);
        GetProjectResourceResponse GetProjectResource(GetProjectResourceRequest request);
        GetStatusResourceProjectResponse GetStatusResourceProject(GetStatusResourceProjectRequest request);
        CreateOrUpdateProjectResourceResponse CreateOrUpdateProjectResource(CreateOrUpdateProjectResourceRequest request);
        GetMasterMilestoneResponse GetMasterMilestone(GetMasterMilestoneRequest request);
        //GetAllTaskByProjectIdResponse GetAllTaskForMilestone(GetAllTaskByProjectIdRequest request);
        UpdateProjectMilestoneResponse CreateOrUpdateProjectMilestone(UpdateProjectMilestoneRequest request);
        GetAllTaskByProjectIdResponse GetAllTaskByProjectScopeId(GetAllTaskByProjectScopeIdRequest request);
        //CreateProjectScopeResponse CreateNewProjectScope(UpdateProjectScopeRequest request);
        DeleteProjectResourceResponse DeleteProjectResource(DeleteProjectResourceRequest request);
        UpdateProjectVendorResponse UpdateProjectVendorResource( UpdateProjectVendorRequest request);
        DeleteProjectScopeResponse DeleteProjectScope(DeleteProjectScopeRequest request);
        CheckAllowcateProjectResourceResponse CheckAllowcateProjectResourceResult(CheckAllowcateProjectResourceRequest request);
        GetPermissionResponse GetPermission(GetPermissionRequest request);
        GetMasterDataProjectMilestoneResponse GetMasterDataProjectMilestone(GetMasterDataProjectMilestoneRequest request);
        GetMasterDataCreateOrUpdateMilestoneResponse GetMasterDataCreateOrUpdateMilestone(GetMasterDataCreateOrUpdateMilestoneRequest request);
        CreateOrUpdateMilestoneResponse CreateOrUpdateMilestone(CreateOrUpdateMilestoneRequest request);
        UpdateStatusProjectMilestoneResponse UpdateStatusProjectMilestone(UpdateStatusProjectMilestoneRequest request);
        GetMasterDataAddOrRemoveTaskToMilestoneResponse GetMasterDataAddOrRemoveTaskToMilestone(GetMasterDataAddOrRemoveTaskToMilestoneRequest request);
        AddOrRemoveTaskMilestoneResponse AddOrRemoveTask(AddOrRemoveTaskMilestoneRequest request);
        GetDataMilestoneByIdResponse GetDataMilestoneById(GetDataMilestoneByIdRequest request);

        GetMasterProjectDocumentResponse GetMasterProjectDocument(GetMasterProjectDocumentRequest request);
        GetCloneProjectScopeResponse GetMasterDataListCloneProjectScope();
        CloneProjectScopeResponse CloneProjectScope(GetCloneProjecScopetRequest request);
        SearchNoteResponse PagingProjectNote( SearchNoteRequest request);
        GetMasterDataCommonDashboardProjectResponse GetMasterDataCommonDashboardProject(GetMasterDataCommonDashboardProjectRequest request);
        
        GetDataDashboardProjectFollowManagerResponse GetDataDashboardProjectFollowManager(GetDataDashboardProjectFollowManagerRequest request);

        GetDataDashboardProjectFollowEmployeeResponse GetDataDashboardProjectFollowEmployee(GetDataDashboardProjectFollowEmployeeRequest request);

        GetDataEVNProjectDashboardResponse GetDataEVNProjectDashboard(GetDataEVNProjectDashboardRequest request);
        SynchronizedEvnResponse SynchronizedEvn(SynchronizedEvnRequest request);

        GetMasterDataProjectInformationResponse getMasterDataProjectInformation(GetMasterDataProjectInformationRequest request);
    }
}
