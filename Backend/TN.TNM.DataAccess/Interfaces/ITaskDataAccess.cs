using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Task;
using TN.TNM.DataAccess.Messages.Results.Task;

namespace TN.TNM.DataAccess.Interfaces
{
    public interface ITaskDataAccess
    {
        GetMasterDataCreateOrUpdateTaskResult GetMasterDataCreateOrUpdateTask(GetMasterDataCreateOrUpdateTaskParameter parameter);
        GetProjectScopeByProjectIdResult GetProjectScopeByProjectId(GetProjectScopeByProjectIdParameter parameter);
        CreateOrUpdateTaskResult CreateOrUpdateTask(CreateOrUpdateTaskParameter parameter);
        GetMasterDataTimeSheetResult GetMasterDataTimeSheet(GetMasterDataTimeSheetParameter parameter);
        GetMasterDataSearchTaskResult GetMasterDataSearchTask(GetMasterDataSearchTaskParameter parameter);
        SearchTaskResult SearchTask(SearchTaskParameter parameter);
        CreateOrUpdateTimeSheetResult CreateOrUpdateTimeSheet(CreateOrUpdateTimeSheetParameter parameter);
        ChangeStatusTaskResult ChangeStatusTask(ChangeStatusTaskParameter parameter);
        DeleteTaskResult DeleteTask(DeleteTaskParameter parameter);
       
        SearchTaskResult SearchTaskFromProjectScope(SearchTaskParameter parameter);
        GetMasterDataCreateConstraintResult GetMasterDataCreateConstraint(GetMasterDataCreateConstraintParameter parameter);
        CreateConstraintTaskResult CreateConstraintTask(CreateConstraintTaskParameter parameter);
        UpdateRequiredConstrantResult UpdateRequiredConstrant(UpdateRequiredConstrantParameter parameter);
        DeleteTaskConstraintResult DeleteTaskConstraint(DeleteTaskConstraintParameter paramter);
        GetMasterDataSearchTimeSheetResult GetMasterDataSearchTimeSheet(GetMasterDataSearchTimeSheetParameter parameter);
        SearchTimeSheetResult SearchTimeSheet(SearchTimeSheetParameter parameter);

        UpdateStatusTimeSheetResult UpdateStatusTimeSheet(UpdateStatusTimeSheetParameter parameter);
        AcceptOrRejectByDayResult AcceptOrRejectByDay(AcceptOrRejectByDayParameter parameter);
        GetMasterDataCreateRelateTaskResult GetMasterDataCreateRelateTask(GetMasterDataCreateRelateTaskParameter parameter);
        CreateRelateTaskResult CreateRelateTask(CreateRelateTaskParameter parameter);
        DeleteRelateTaskResult DeleteRelateTask(DeleteRelateTaskParameter parameter);
    }
}
