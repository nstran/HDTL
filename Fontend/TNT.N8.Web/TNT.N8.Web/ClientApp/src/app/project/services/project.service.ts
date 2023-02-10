import { map } from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ProjectModel } from '../models/project.model';
import { ProjectResourceModel } from '../models/ProjectResource.model';
import { TaskDocument, TaskModel } from '../models/ProjectTask.model';
import { TimeSheet } from '../models/timeSheet.model';
import { ProjectScopeModel } from '../models/ProjectScope.model';
import { ProjectVendorModel } from '../models/ProjectVendor.model';
import { TaskContraint } from '../models/TaskConstraint.model';
import { ProjectMilestoneModel } from '../models/milestone.model';
import { TaskRelate } from '../models/TaskRelate.model';

class Document {
  noteDocumentId: string;
  projectId: string;
  documentName: string;
  documentSize: string;
  documentUrl: string;
  documentType: string;
  documentExtension: string;
  documentNameWithoutExtension: string;
  updatedDate: Date;
  active: boolean;
  objectId: string;
  objectType: string;
  folderId: string;
}

class FileUploadModel {
  FileInFolder: FileInFolder;
  FileSave: File;
}

class TaskDocumentEntity {
  taskDocumentId: string;
  taskId: string;
  documentName: string;
  documentSize: string;
  documentUrl: string;
  active: boolean;
  createdDate: Date;
  createdById: string;
  createByName: string;
  updatedDate: Date;
  updatedById: string;
}

class Folder {
  folderId: string;
  parentId: string;
  name: string;
  url: string;
  isDelete: boolean;
  active: boolean;
  hasChild: boolean;
  folderType: string;
  folderLevel: number;
  listFile: Array<FileInFolder>;
  fileNumber: number;
}

class FileInFolder {
  fileInFolderId: string;
  folderId: string;
  fileName: string;
  objectId: string;
  objectType: string;
  size: string;
  active: boolean;
  fileExtension: string;
}

@Injectable()
export class ProjectService {

  constructor(private httpClient: HttpClient) { }

  userId: string = JSON.parse(localStorage.getItem("auth")).UserId;


  changeProjectStatus(
    projectId: string, status) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/project/updateProjectStatus";
    return this.httpClient.post(url, {
      projectId: projectId, Status: status
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  searchProject(projectCode: string, projectName: string, projectStartS: Date, projectStartE: Date, projectEndS: Date, projectEndE: Date,
    actualStartS: Date, actualStartE: Date, actualEndS: Date, actualEndE: Date, listStatusProject: Array<string>, projectType: Array<string>, listEmployee: Array<string>,
    estimateCompleteTimeS: number, estimateCompleteTimeE: number, completeRateS: number, completeRateE: number) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/project/searchProject';

    return this.httpClient.post(url,
      {
        ProjectCode: projectCode,
        ProjectStartS: projectStartS,
        ProjectStartE: projectStartE,
        ProjectEndS: projectEndS,
        ProjectEndE: projectEndE,
        ActualStartS: actualStartS,
        ActualStartE: actualStartE,
        ActualEndS: actualEndS,
        ActualEndE: actualEndE,
        ListStatusProject: listStatusProject,
        ListProjectType: projectType,
        ListEmployee: listEmployee,
        ProjectName: projectName,
        EstimateCompleteTimeS: estimateCompleteTimeS,
        EstimateCompleteTimeE: estimateCompleteTimeE,
        CompleteRateS: completeRateS,
        CompleteRateE: completeRateE,
      }).pipe(
        map((response: Response) => {
          return <any>response;
        }));
  }


  getMasterProjectCreate() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/project/getMasterProjectCreate';
    return this.httpClient.post(url, {}).pipe(
      map((response: Response) => {
        return response;
      }));
  }
  getMasterUpdateProjectCreate(projectId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/project/getMasterUpdateProjectCreate';
    return this.httpClient.post(url, { ProjectId: projectId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  createProject(project: ProjectModel, listProjectTarget: Array<any>, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/project/createProject';
    return this.httpClient.post(url, { Project: project, ListProjectTarget: listProjectTarget, UserId: userId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }
  updateProject(project: ProjectModel, listProjectTarget: Array<any>, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/project/updateProject';
    return this.httpClient.post(url, { Project: project, ListProjectTarget: listProjectTarget, UserId: userId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getProjectScope(projectId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/project/getProjectScope';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url,
        {
          ProjectId: projectId,
          UserId: this.userId
        }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  deleteProjectScope(
    projectScopeId: string, projectId: string, parentId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/project/deleteProjectScope";
    return this.httpClient.post(url, {
      ProjectScopeId: projectScopeId,
      ProjectId: projectId,
      ParentId: parentId
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  // updateProjectScope(listProjectScope: Array<any>, userId: string) {
  //   let url = localStorage.getItem('ApiEndPoint') + '/api/project/updateProjectScope';
  //   return this.httpClient.post(url, { ListProjectScope: listProjectScope, UserId: userId }).pipe(
  //     map((response: Response) => {
  //       return response;
  //     }));
  // }
  updateProjectScope(projectScope: ProjectScopeModel, userId: string, parentId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/project/updateProjectScope';
    return this.httpClient.post(url, { ProjectScope: projectScope, UserId: userId, ParentId: parentId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getProjectResource(projectId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/project/getProjectResource';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        ProjectId: projectId,
        UserId: this.userId
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getStatusResourceProject(projectId: string, projectResourceId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/project/getStatusResourceProject';
    return this.httpClient.post(url, { ProjectId: projectId, ProjectResourceId: projectResourceId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }
  createOrUpdateProjectResource(projectResource: ProjectResourceModel, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/project/createOrUpdateProjectResource';
    return this.httpClient.post(url, { ProjectResource: projectResource, UserId: userId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getMasterDataCreateOrUpdateTask(projectId: string, taskId: string, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/task/getMasterDataCreateOrUpdateTask';
    return this.httpClient.post(url, { ProjectId: projectId, TaskId: taskId, UserId: userId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getProjectScopeByProjectId(projectId: string, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/task/getProjectScopeByProjectId';
    return this.httpClient.post(url, { ProjectId: projectId, UserId: userId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  // createOrUpdateTask(task: TaskModel, listResource: Array<ProjectResourceModel>, listTaskDocument: Array<TaskDocument>, userId: string) {
  //   let url = localStorage.getItem('ApiEndPoint') + '/api/task/createOrUpdateTask';
  //   return this.httpClient.post(url, {
  //     Task: task,
  //     ListProjectResource: listResource,
  //     ListTaskDocument: listTaskDocument,
  //     UserId: userId
  //   }).pipe(
  //     map((response: Response) => {
  //       return response;
  //     }));
  // }


  createOrUpdateTask(task: TaskModel, folderType: string, listResource: Array<ProjectResourceModel>, 
                    listTaskDocument: Array<FileUploadModel>, listTaskDocumentDelete: Array<TaskDocumentEntity>, 
                    userId: string, listRelateTask: Array<TaskRelate>) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/task/createOrUpdateTask';
    let formData: FormData = new FormData();

    formData.append("UserId", userId);
    formData.append("FolderType", folderType);

    formData.append('Task[TaskId]', task.taskId);
    formData.append('Task[ProjectId]', task.projectId);
    formData.append('Task[ProjectScopeId]', task.projectScopeId);
    formData.append('Task[TaskCode]', task.taskCode);
    formData.append('Task[TaskName]', task.taskName);
    formData.append('Task[PlanStartTime]', task.planStartTime == null ? null : task.planStartTime.toUTCString());
    formData.append('Task[PlanEndTime]', task.planEndTime == null ? null : task.planEndTime.toUTCString());
    formData.append('Task[EstimateHour]', task.estimateHour == null ? null : task.estimateHour.toString());
    formData.append('Task[EstimateCost]', task.estimateCost == null ? null : task.estimateCost.toString());
    formData.append('Task[ActualStartTime]', task.actualStartTime == null ? null : task.actualStartTime.toUTCString());
    formData.append('Task[ActualEndTime]', task.actualEndTime == null ? null : task.actualEndTime.toUTCString());
    formData.append('Task[ActualHour]', task.actualHour == null ? null : task.actualHour.toString());
    formData.append('Task[Description]', task.description == null ? "" : task.description.toString());
    formData.append('Task[Status]', task.status);
    formData.append('Task[Priority]', task.priority.toString());
    formData.append('Task[MilestonesId]', task.milestonesId);
    formData.append('Task[IncludeWeekend]', task.includeWeekend ? "true" : "false");
    formData.append('Task[IsSendMail]', task.isSendMail ? "true" : "false");
    formData.append('Task[TaskComplate]', task.taskComplate == null ? null : task.taskComplate.toString());
    formData.append('Task[TaskTypeId]', task.taskTypeId);
    formData.append('Task[TimeType]', task.timeType);
    formData.append('Task[CreateBy]', task.createBy);
    formData.append('Task[CreateDate]', task.createDate == null ? null : task.createDate.toUTCString());



    var index = 0;
    for (var listRelateTaskItem of listRelateTask) {
      formData.append("ListTaskRelate[" + index + "].RelateTaskMappingId", listRelateTaskItem.RelateTaskMappingId);
      formData.append("ListTaskRelate[" + index + "].RelateTaskId", listRelateTaskItem.relateTaskId);
      formData.append("ListTaskRelate[" + index + "].ProjectId", listRelateTaskItem.ProjectId);
      formData.append("ListTaskRelate[" + index + "].createdById", listRelateTaskItem.createdById);
      index++;
    }


    var index = 0;
    for (var pair of listTaskDocument) {
      formData.append("ListTaskDocument[" + index + "].FileInFolder.FileInFolderId", pair.FileInFolder.fileInFolderId);
      formData.append("ListTaskDocument[" + index + "].FileInFolder.FolderId", pair.FileInFolder.folderId);
      formData.append("ListTaskDocument[" + index + "].FileInFolder.FileName", pair.FileInFolder.fileName);
      formData.append("ListTaskDocument[" + index + "].FileInFolder.ObjectId", pair.FileInFolder.objectId);
      formData.append("ListTaskDocument[" + index + "].FileInFolder.ObjectType", pair.FileInFolder.objectType);
      formData.append("ListTaskDocument[" + index + "].FileInFolder.Size", pair.FileInFolder.size);
      formData.append("ListTaskDocument[" + index + "].FileInFolder.FileExtension", pair.FileInFolder.fileExtension);
      formData.append("ListTaskDocument[" + index + "].FileSave", pair.FileSave);
      index++;
    }

    var index = 0;
    for (var file of listTaskDocumentDelete) {
      formData.append("ListTaskDocumentDelete[" + index + "].Active", file.active ? "true" : "false");
      formData.append("ListTaskDocumentDelete[" + index + "].DocumentUrl", file.documentUrl);
      formData.append("ListTaskDocumentDelete[" + index + "].DocumentSize", file.documentSize);
      formData.append("ListTaskDocumentDelete[" + index + "].DocumentName", file.documentName);
      formData.append("ListTaskDocumentDelete[" + index + "].TaskId", file.taskId);
      formData.append("ListTaskDocumentDelete[" + index + "].TaskDocumentId", file.taskDocumentId);
      formData.append("ListTaskDocumentDelete[" + index + "].CreatedDate", file.createdDate == null ? null : new Date(file.createdDate).toUTCString());
      formData.append("ListTaskDocumentDelete[" + index + "].CreatedById", file.createdById);
      formData.append("ListTaskDocumentDelete[" + index + "].CreateByName", file.createByName);
      index++;
    }

    index = 0;
    for (var item of listResource) {
      formData.append("ListProjectResource[" + index + "].EmployeeRole", item.employeeRole.toString());
      formData.append("ListProjectResource[" + index + "].ResourceRoleName", item.employeeRoleName);
      formData.append("ListProjectResource[" + index + "].EndTime", item.endTime == null ? null : item.endTime.toUTCString());
      formData.append("ListProjectResource[" + index + "].StartTime", item.startTime == null ? null : item.startTime.toUTCString());
      formData.append("ListProjectResource[" + index + "].Hours", item.hours.toString());
      formData.append("ListProjectResource[" + index + "].IncludeWeekend", item.includeWeekend ? "true" : "false");
      formData.append("ListProjectResource[" + index + "].IsActive", item.isActive ? "true" : "false");
      formData.append("ListProjectResource[" + index + "].IsCheck", item.isCheck ? "true" : "false");
      formData.append("ListProjectResource[" + index + "].IsCreateVendor", item.isCreateVendor ? "true" : "false");
      formData.append("ListProjectResource[" + index + "].IsOverload", item.isOverload ? "true" : "false");
      formData.append("ListProjectResource[" + index + "].IsPic", item.isPic ? "true" : "false");
      formData.append("ListProjectResource[" + index + "].NameResource", item.nameResource);
      formData.append("ListProjectResource[" + index + "].ObjectId", item.objectId);
      formData.append("ListProjectResource[" + index + "].ProjectId", item.projectId);
      formData.append("ListProjectResource[" + index + "].ProjectResourceId", item.projectResourceId);
      formData.append("ListProjectResource[" + index + "].ResourceName", item.resourceName);
      formData.append("ListProjectResource[" + index + "].ResourceRole", item.resourceRole);
      formData.append("ListProjectResource[" + index + "].ResourceType", item.resourceType);
      index++;
    }

    return this.httpClient.post(url, formData).pipe(
      map((response: Response) => {
        return response;
      }));
  }


  getMasterMilestone(projectId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/project/getMasterMilestone';
    return this.httpClient.post(url, { ProjectId: projectId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getAllTaskForMilestone(projectId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/project/getAllTaskForMilestone';
    return this.httpClient.post(url, { ProjectId: projectId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  createOrUpdateProjectMilestone(
    milestonIdId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/project/createOrUpdateProjectMilestone";
    return this.httpClient.post(url, {
      MilestonIdId: milestonIdId,
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getAllTaskByProjectScopeId(projectId: string, projectScopeId: string, userId: string) {

    const url = localStorage.getItem('ApiEndPoint') + '/api/project/getAllTaskByProjectScopeId';
    return this.httpClient.post(url, { ProjectId: projectId, ProjectScopeId: projectScopeId, UserId: userId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  createNewProjectScope(listProjectScope: Array<any>, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/project/createNewProjectScope';
    return this.httpClient.post(url, { ListProjectScope: listProjectScope, UserId: userId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getMasterDataTimeSheet(projectId: string, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/task/getMasterDataTimeSheet';
    return this.httpClient.post(url, {
      ProjectId: projectId,
      UserId: userId
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  // deleteTask(
  //   taskId: string) {
  //   let url = localStorage.getItem('ApiEndPoint') + "/api/task/deleteTask";
  //   return this.httpClient.post(url, {
  //     TaskId: taskId,
  //   }).pipe(
  //     map((response: Response) => {
  //       return response;
  //     }));
  // }

  searchTaskFromProjectScope(projectId: string, listStatusId: Array<string>, listWorkpackageId: Array<string>,
    listEmployeeId: Array<string>, fromStartDate: Date, toStartDate: Date, fromEndDate: Date, toEndDate: Date, fromPercent: number, toPercent: number, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/task/searchTaskFromProjectScope';

    return this.httpClient.post(url, {
      ProjectId: projectId,
      ListWorkpackageId: listWorkpackageId,
      ListStatusId: listStatusId,
      ListEmployeeId: listEmployeeId,
      FromDate: fromStartDate,
      ToDate: toStartDate,
      FromEndDate: fromEndDate,
      ToEndDate: toEndDate,
      FromPercent: fromPercent,
      ToPercent: toPercent,
      UserId: userId
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }


  getMasterDataSearchTask(projectId: string, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/task/getMasterDataSearchTask';
    return this.httpClient.post(url, {
      ProjectId: projectId,
      UserId: userId
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  searchTask(projectId: string, listTaskTypeId: Array<string>, listStatusId: Array<string>, listPriority: Array<number>,
    listEmployeeId: Array<string>, listCreatedId: Array<string>, fromDate: Date, toDate: Date, type: string, optionStatus: string, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/task/searchTask';
    return this.httpClient.post(url, {
      ProjectId: projectId,
      ListTaskTypeId: listTaskTypeId,
      ListStatusId: listStatusId,
      ListPriority: listPriority,
      ListEmployeeId: listEmployeeId,
      ListCreatedId: listCreatedId,
      FromDate: fromDate,
      ToDate: toDate,
      Type: type,
      OptionStatus: optionStatus,
      UserId: userId
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  createOrUpdateTimeSheet(timeSheet: TimeSheet, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/task/createOrUpdateTimeSheet';
    return this.httpClient.post(url, {
      TimeSheet: timeSheet,
      UserId: userId
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  changeStatusTask(taskId: string, statusId: string, type: number, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/task/changeStatusTask';
    return this.httpClient.post(url, {
      TaskId: taskId,
      StatusId: statusId,
      Type: type,
      UserId: userId
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  deleteTask(taskId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/task/deleteTask';
    return this.httpClient.post(url, {
      TaskId: taskId
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  deleteProjectResource(
    projectResourceId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/project/deleteProjectResource";
    return this.httpClient.post(url, {
      ProjectResourceId: projectResourceId,
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }


  updateProjectVendorResource(projectVendor: ProjectVendorModel, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/project/updateProjectVendorResource";
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        ProjectVendor: projectVendor,
        UserId: userId
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  checkAllowcateProjectResource(resourceId: string, fromDate: Date, toDate: Date, allowcation: number, projectResourceId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/project/checkAllowcateProjectResource';
    return this.httpClient.post(url, {
      ResourceId: resourceId,
      FromDate: fromDate,
      ToDate: toDate,
      Allowcation: allowcation,
      ProjectResourceId: projectResourceId
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getMasterDataCreateConstraint(taskId: string, projectId: string, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/task/getMasterDataCreateConstraint';
    return this.httpClient.post(url, {
      ProjectId: projectId,
      TaskId: taskId,
      UserId: userId
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }


  GetMasterDataCreateRelateTask(taskId: string, projectId: string, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/task/GetMasterDataCreateRelateTask';
    return this.httpClient.post(url, {
      ProjectId: projectId,
      TaskId: taskId,
      UserId: userId
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }



  createConstraintTask(taskConstraint: TaskContraint, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/task/createConstraintTask';
    return this.httpClient.post(url, {
      TaskConstraint: taskConstraint,
      UserId: userId
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  updateRequiredConstrant(taskConstraintId: string, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/task/updateRequiredConstrant';
    return this.httpClient.post(url, {
      TaskConstraintId: taskConstraintId,
      UserId: userId
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  deleteTaskConstraint(taskConstraintId: string, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/task/deleteTaskConstraint';
    return this.httpClient.post(url, {
      TaskConstraintId: taskConstraintId,
      UserId: userId
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  deleteRelateTask(RelateTaskMappingId: string, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/task/deleteRelateTask';
    return this.httpClient.post(url, {
      RelateTaskMappingId: RelateTaskMappingId,
      UserId: userId
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getPermission(projectId: string, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/project/getPermission';
    return this.httpClient.post(url, {
      ProjectId: projectId,
      UserId: userId
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getMasterDataProjectMilestone(projectId: string, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/project/getMasterDataProjectMilestone';
    return this.httpClient.post(url, {
      ProjectId: projectId,
      UserId: userId
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getMasterDataCreateOrUpdateMilestone(projectMilestoneId: string, projectId: string, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/project/getMasterDataCreateOrUpdateMilestone';
    return this.httpClient.post(url, {
      ProjectMilestoneId: projectMilestoneId,
      ProjectId: projectId,
      UserId: userId
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  createOrUpdateMilestone(projectMilestone: ProjectMilestoneModel, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/project/createOrUpdateMilestone';
    return this.httpClient.post(url, {
      ProjectMilestone: projectMilestone,
      UserId: userId
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  updateStatusProjectMilestone(projectMilestoneId: string, type: number, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/project/updateStatusProjectMilestone';
    return this.httpClient.post(url, {
      ProjectMilestoneId: projectMilestoneId,
      Type: type,
      UserId: userId
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getMasterDataAddOrRemoveTaskToMilestone(projectMilestoneId: string, projectId: string, type: number, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/project/getMasterDataAddOrRemoveTaskToMilestone';
    return this.httpClient.post(url, {
      ProjectMilestoneId: projectMilestoneId,
      ProjectId: projectId,
      Type: type,
      UserId: userId
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  addOrRemoveTaskMilestone(listTaskId: Array<string>, projectMilestoneId: string, type: number, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/project/addOrRemoveTaskMilestone';
    return this.httpClient.post(url, {
      ListTaskId: listTaskId,
      ProjectMilestoneId: projectMilestoneId,
      Type: type,
      UserId: userId
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getDataMilestoneById(projectId: string, projectMilestoneId: string, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/project/getDataMilestoneById';
    return this.httpClient.post(url, {
      ProjectId: projectId,
      ProjectMilestoneId: projectMilestoneId,
      UserId: userId
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  // getAllProjectNoteDocument(projectId: string) {
  //   let url = localStorage.getItem('ApiEndPoint') + "/api/document/getAllProjectNoteDocument";
  //   return this.httpClient.post(url, {
  //     projectId: projectId,
  //   }).pipe(
  //     map((response: Response) => {
  //       return response;
  //     }));
  // }

  getMasterProjectDocument(projectId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/project/getMasterProjectDocument";
    return this.httpClient.post(url, {
      projectId: projectId,
      userId: this.userId,
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  UpdateProjectDocument(projectId: string, listTaskDocument: Array<TaskDocumentEntity>, listDocument: Array<Document>) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/document/updateProjectDocument";
    return this.httpClient.post(url, {
      projectId: projectId,
      listTaskDocument: listTaskDocument,
      listDocument: listDocument,
      userId: this.userId,
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  createFolder(folder: Folder, folderName: string, projectCode: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/document/createFolder';
    return this.httpClient.post(url, {
      FolderParent: folder,
      FolderName: folderName,
      ProjectCode: projectCode,
    }).pipe(map((response: Response) => {
      return response;
    }));
  }

  uploadFileForOptionAsync(fileList: File[], option: string, projectCodeName: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/document/uploadFileForOption';
    let formData: FormData = new FormData();
    formData.append('Option', option);
    formData.append('ProjectCodeName', projectCodeName);
    formData.append('UserId', this.userId);

    for (var i = 0; i < fileList.length; i++) {
      formData.append('fileList', fileList[i]);
    }

    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, formData).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  deleteFileForOptionAsync(option: string, fileName: string, projectCodeName: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/document/deleteFileForOption';
    let formData: FormData = new FormData();
    formData.append('Option', option);
    formData.append('FileName', fileName);
    formData.append('ProjectCodeName', projectCodeName);
    formData.append('UserId', this.userId);

    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, formData).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  uploadFileDocument(folderId: string, folderType: string, listFile: Array<FileUploadModel>, objectId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/document/uploadFileDocument';

    let formData: FormData = new FormData();
    formData.append("FolderId", folderId);
    formData.append("FolderType", folderType);
    formData.append("ObjectId", objectId);
    formData.append("UserId", this.userId);
    var index = 0;
    for (var pair of listFile) {
      formData.append("ListFile[" + index + "].FileInFolder.FileInFolderId", pair.FileInFolder.fileInFolderId);
      formData.append("ListFile[" + index + "].FileInFolder.FolderId", pair.FileInFolder.folderId);
      formData.append("ListFile[" + index + "].FileInFolder.FileName", pair.FileInFolder.fileName);
      formData.append("ListFile[" + index + "].FileInFolder.ObjectId", pair.FileInFolder.objectId);
      formData.append("ListFile[" + index + "].FileInFolder.ObjectType", pair.FileInFolder.objectType);
      formData.append("ListFile[" + index + "].FileInFolder.Size", pair.FileInFolder.size);
      formData.append("ListFile[" + index + "].FileInFolder.FileExtension", pair.FileInFolder.fileExtension);
      formData.append("ListFile[" + index + "].FileSave", pair.FileSave);
      index++;
    }
    return this.httpClient.post(url,
      formData
    ).pipe(map((response: Response) => {
      return response;
    }));
  }

  loadFileByFolder(folderId: string, folderType: string, objectId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/document/loadFileByFolder';
    return this.httpClient.post(url, {
      FolderId: folderId,
      FolderType: folderType,
      ObjectId: objectId,
      UserId: this.userId,
    }).pipe(map((response: Response) => {
      return response;
    }));
  }

  getMasterDataSearchTimeSheet(projectId: string, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/task/getMasterDataSearchTimeSheet';
    return this.httpClient.post(url, {
      ProjectId: projectId,
      UserId: userId
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getMasterDataListCloneProjectScope() {
    let url = localStorage.getItem('ApiEndPoint') + '/api/project/getMasterDataListCloneProjectScope';
    return this.httpClient.post(url, {}).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  cloneProjectScope(newProjectId: string, oldProjectId: string, userId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/project/cloneProjectScope';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        NewProjectId: newProjectId,
        OldProjectId: oldProjectId,
        UserId: userId
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  searchTimeSheet(projectId: string, fromDate: Date, toDate: Date, listStatusId: Array<string>, listTimeTypeId: Array<string>, listPersionInChargedId: Array<string>,
    isShowAll: boolean, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/task/searchTimeSheet';
    return this.httpClient.post(url, {
      ProjectId: projectId,
      FromDate: fromDate,
      ToDate: toDate,
      ListStatusId: listStatusId,
      ListTimeTypeId: listTimeTypeId,
      ListPersionInChargedId: listPersionInChargedId,
      IsShowAll: isShowAll,
      UserId: userId
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  updateStatusTimeSheet(listTimeSheetId: Array<string>, type: string, description: string, userId: string, timeSheet: TimeSheet) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/task/updateStatusTimeSheet';
    return this.httpClient.post(url, {
      ListTimeSheetId: listTimeSheetId,
      Type: type,
      Description: description,
      UserId: userId,
      TimeSheet: timeSheet,
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  pagingProjectNote(projectId: string, pageSize: number, pageIndex: number, screenName: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/project/pagingProjectNote';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url,
        {
          ProjectId: projectId,
          PageSize: pageSize,
          PageIndex: pageIndex,
          ScreenName: screenName
        }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getMasterDataCommonDashboardProject(projectId: string, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/project/getMasterDataCommonDashboardProject';
    return this.httpClient.post(url, {
      ProjectId: projectId,
      UserId: userId
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getDataDashboardProjectFollowManager(projectId: string, mode: string, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/project/getDataDashboardProjectFollowManager';
    return this.httpClient.post(url, {
      ProjectId: projectId,
      Mode: mode,
      UserId: userId
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getDataDashboardProjectFollowEmployee(projectId: string, mode: string, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/project/getDataDashboardProjectFollowEmployee';
    return this.httpClient.post(url, {
      ProjectId: projectId,
      Mode: mode,
      UserId: userId
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getDataEVNProjectDashboard(projectId: string, mode: string, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/project/getDataEVNProjectDashboard';
    return this.httpClient.post(url, {
      ProjectId: projectId,
      Mode: mode,
      UserId: userId
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  synchronizedEvn() {
    let url = localStorage.getItem('ApiEndPoint') + '/api/project/synchronizedEvn';
    return this.httpClient.post(url, {
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getMasterDataProjectInformation(projectId: string, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/project/getMasterDataProjectInformation';
    return this.httpClient.post(url, {
      ProjectId: projectId,
      UserId: userId
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getBaoCaoSuDungNguonLuc(thang: number, nam: number, listProjectId: Array<string>) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/project/getBaoCaoSuDungNguonLuc';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        Thang: thang,
        Nam: nam,
        ListProjectId: listProjectId
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getMasterDataBaoCaoSuDungNguonLuc() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/project/getMasterDataBaoCaoSuDungNguonLuc';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {

      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getBaoCaoTongHopCacDuAn() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/project/getBaoCaoTongHopCacDuAn';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {

      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  acceptOrRejectByDay(id: string, check: boolean) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/task/acceptOrRejectByDay';
    return this.httpClient.post(url, {
      TimeSheetDetail: id,
      Check: check
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getThoiGianUocLuongHangMuc(projectScopeId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/project/getThoiGianUocLuongHangMuc';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url,
        {
          ProjectScopeId: projectScopeId,
        }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getPhanBoTheoNguonLuc(objectId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/project/getPhanBoTheoNguonLuc';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url,
        {
          objectId: objectId,
        }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  createRelateTask(TaskRelate: TaskRelate, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/task/createRelateTask';
    return this.httpClient.post(url, {
      TaskRelate: TaskRelate,
      UserId: userId
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

}
