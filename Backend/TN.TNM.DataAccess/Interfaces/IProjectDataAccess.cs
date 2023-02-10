using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Note;
using TN.TNM.DataAccess.Messages.Parameters.Project;
using TN.TNM.DataAccess.Messages.Parameters.Task;
using TN.TNM.DataAccess.Messages.Results.Note;
using TN.TNM.DataAccess.Messages.Results.Project;
using TN.TNM.DataAccess.Messages.Results.Task;

namespace TN.TNM.DataAccess.Interfaces
{
    public interface IProjectDataAccess
    {
        GetMasterProjectResult GetMasterProjectCreate(GetMasterProjectParameter parameter);
        SearchProjectResult SearchProject(SearchProjectParameter parameter);
        CreateProjectResult CreateProject(CreateProjectParameter parameter);
        UpdateProjectStatusResult UpdateProjectStatus(UpdateProjectStatusParameter parametter);
        GetMasterUpdateProjectResult GetMasterUpdateProjectCreate(GetMasterUpdateProjectParameter parameter);
        UpdateProjectResult UpdateProject(UpdateProjectParameter parameter);
        GetProjectScopeResult GetProjectScope(GetProjectScopeParameter parameter);
        UpdateProjectScopeResult UpdateProjectScope(UpdateProjectScopeParameter parameter);
        GetProjectResourceResult GetProjectResource(GetProjectResourceParameter parameter);
        GetStatusResourceProjectResult GetStatusResourceProject(GetStatusResourceProjectParameter parameter);
        CreateOrUpdateProjectResourceResult CreateOrUpdateProjectResource(CreateOrUpdateProjectResourceParameter parameter);
        GetMasterMilestoneResult GetMasterMilestone(GetMasterMilestoneParameter parameter);
        //GetAllTaskByProjectIdResult GetAllTaskForMilestone(GetAllTaskByProjectIdParameter request);
        UpdateProjectMilestoneResult CreateOrUpdateProjectMilestone(UpdateProjectMilestoneParameter request);
        GetAllTaskByProjectScopeIdResult GetAllTaskByProjectScopeId(GetAllTaskByProjectIdParameter request);
        //CreateProjectScopeResult CreateNewProjectScope(UpdateProjectScopeParameter parameter);
        DeleteProjectResourceResult DeleteProjectResource(DeleteProjectResourceParameter request);
        UpdateProjectVendorResult UpdateProjectVendorResource(UpdateProjectVendorParameter request);
        DeleteProjectScopeResult DeleteProjectScope(DeleteProjectScopeParameter request);
        CheckAllowcateProjectResourceResult CheckAllowcateProjectResource(CheckAllowcateProjectResourceParameter request, bool isCreateOrUpdet = true);
        GetPermissionResult GetPermission(GetPermissionParameter parameter);
        GetMasterDataProjectMilestoneResult GetMasterDataProjectMilestone(GetMasterDataProjectMilestoneParameter parameter);
        GetMasterDataCreateOrUpdateMilestoneResult GetMasterDataCreateOrUpdateMilestone(GetMasterDataCreateOrUpdateMilestoneParameter parameter);
        CreateOrUpdateMilestoneResult CreateOrUpdateMilestone(CreateOrUpdateMilestoneParameter parameter);
        UpdateStatusProjectMilestoneResult UpdateStatusProjectMilestone(UpdateStatusProjectMilestoneParameter parameter);
        GetMasterDataAddOrRemoveTaskToMilestoneResult GetMasterDataAddOrRemoveTaskToMilestone(GetMasterDataAddOrRemoveTaskToMilestoneParameter parameter);
        AddOrRemoveTaskMilestoneResult AddOrRemoveTaskMilestone(AddOrRemoveTaskMilestoneParameter parameter);
        GetDataMilestoneByIdResult GetDataMilestoneById(GetDataMilestoneByIdParameter parameter);
        GetMasterProjectDocumentResult GetMasterProjectDocument(GetMasterProjectDocumentParameter parameter);
        GetCloneProjectScopeResult GetMasterDataListCloneProjectScope();
        CloneProjectScopeResult CloneProjectScope(GetCloneProjectScopeParameter request);
        SearchNoteResult PagingProjectNote(SearchNoteParameter parameter);
        GetMasterDataCommonDashboardProjectResult GetMasterDataCommonDashboardProject(GetMasterDataCommonDashboardProjectParameter parameter);
        GetDataDashboardProjectFollowManagerResult GetDataDashboardProjectFollowManager(GetDataDashboardProjectFollowManagerParameter parameter);
        GetDataDashboardProjectFollowEmployeeResult getDataDashboardProjectFollowEmployee(GetDataDashboardProjectFollowEmployeeParameter parameter);
        GetDataEVNProjectDashboardResult GetGetDataEVNProjectDashboard(GetDataEVNProjectDashboardParameter parameter);
        SynchronizedEvnResult SynchronizedEvn(SynchronizedEvnParameter parameter);
        GetMasterDataProjectInformationResult GetMasterDataProjectInformation(GetMasterDataProjectInformationParameter parameter);
        GetBaoCaoSuDungNguonLucResult GetBaoCaoSuDungNguonLuc(GetBaoCaoSuDungNguonLucParameter parameter);

        GetMasterDataBaoCaoSuDungNguonLucResult GetMasterDataBaoCaoSuDungNguonLuc(
            GetMasterDataBaoCaoSuDungNguonLucParameter parameter);
        GetBaoCaoTongHopCacDuAnResult GetBaoCaoTongHopCacDuAn(GetBaoCaoTongHopCacDuAnParameter parameter);
        GetThoiGianUocLuongHangMucResult GetThoiGianUocLuongHangMuc(GetThoiGianUocLuongHangMucParameter parameter);
        GetPhanBoTheoNguonLucResult GetPhanBoTheoNguonLuc(GetPhanBoTheoNguonLucParameter parameter);
    }
}
