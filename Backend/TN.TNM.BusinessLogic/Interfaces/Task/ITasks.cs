using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Messages.Requests.Task;
using TN.TNM.BusinessLogic.Messages.Responses.Task;

namespace TN.TNM.BusinessLogic.Interfaces.Task
{
    public interface ITasks
    {
        GetMasterDataCreateOrUpdateTaskResponse GetMasterDataCreateOrUpdateTask(GetMasterDataCreateOrUpdateTaskRequest request);
        GetProjectScopeByProjectIdResponse GetProjectScopeByProjectId(GetProjectScopeByProjectIdRequest request);
        CreateOrUpdateTaskResponse CreateOrUpdateTask(CreateOrUpdateTaskRequest request);
        GetMasterDataTimeSheetResponse GetMasterDataTimeSheet(GetMasterDataTimeSheetRequest request);
        GetMasterDataSearchTaskResponse GetMasterDataSearchTask(GetMasterDataSearchTaskRequest request);
        SearchTaskResponse SearchTask(SearchTaskRequest request);
        CreateOrUpdateTimeSheetResponse CreateOrUpdateTimeSheet(CreateOrUpdateTimeSheetRequest request);
        ChangeStatusTaskResponse ChangeStatusTask(ChangeStatusTaskRequest request);
        DeleteTaskResponse DeleteTask(DeleteTaskRequest request);
        SearchTaskFromProjectScopeResponse SearchTaskFromProjectScope(SearchTaskFromProjectScopeRequest request);
        GetMasterDataCreateConstraintResponse GetMasterDataCreateConstraint(GetMasterDataCreateConstraintRequest request);
        CreateConstraintTaskResponse CreateConstraintTask(CreateConstraintTaskRequest request);
        UpdateRequiredConstrantResponse UpdateRequiredConstrant(UpdateRequiredConstrantRequest request);
        DeleteTaskConstraintResponse DeleteTaskConstraint(DeleteTaskConstraintRequest request);
        GetMasterDataSearchTimeSheetResponse GetMasterDataSearchTimeSheet(GetMasterDataSearchTimeSheetRequest request);
        SearchTimeSheetResponse SearchTimeSheet(SearchTimeSheetRequest request);
        UpdateStatusTimeSheetResponse UpdateStatusTimeSheet(UpdateStatusTimeSheetRequest request);
        AcceptOrRejectByDayResponse AcceptOrRejectByDay(AcceptOrRejectByDayRequest request);
    }
}
