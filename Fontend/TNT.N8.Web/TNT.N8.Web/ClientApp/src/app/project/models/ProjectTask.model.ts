export class ProjectTaskModel {
  taskId: string;
  projectId: string;
  projectScopeId: string;
  taskCode: string;
  taskName: string;
  scopeName: string;
  employee: string;
  statusName: string;
  taskComplate: number;
  startTime: Date;
  endTime: Date;
  hour: number;
  actualStartTime: Date;
  actualEndTime: Date;
  actualHour: number;
  description: string;
  status: string;
  priority: number;
  milestonesId: string;
  includeWeekend: boolean;
  createBy: string;
  createDate: Date;
  updateBy: string;
  updateDate: Date;
  isSendMail: boolean;
  estimateHour: number;
  backgroundColorForStatus: string;
  constructor() {
    this.taskId = '00000000-0000-0000-0000-000000000000';
    this.projectId = '00000000-0000-0000-0000-000000000000';
    this.projectScopeId = '00000000-0000-0000-0000-000000000000';
    this.taskCode = null;
    this.taskName = null;
    this.startTime = null;
    this.endTime = null;
    this.hour = 0;
    this.actualStartTime = null;
    this.actualEndTime = null;
    this.actualHour = 0;
    this.description = null;
    this.status = '00000000-0000-0000-0000-000000000000';
    this.priority = 0;
    this.milestonesId = '00000000-0000-0000-0000-000000000000';
    this.includeWeekend = false;
    this.createBy = '00000000-0000-0000-0000-000000000000';
    this.createDate = null;
    this.updateBy = null;
    this.updateDate = null;
    this.isSendMail = true;
  }
}

export class TaskModel {
  taskId: string;
  projectId: string;
  projectScopeId: string;
  projectScopeName: string;
  taskCode: string;
  taskName: string;
  planStartTime: Date;
  planEndTime: Date;
  estimateHour: number;
  estimateCost: number;
  actualStartTime: Date;
  actualEndTime: Date;
  actualHour: number;
  description: string;
  status: string;
  priority: number;
  milestonesId: string;
  includeWeekend: boolean;
  createBy: string;
  createDate: Date;
  updateBy: string;
  updateDate: Date;
  isSendMail: boolean;
  taskComplate: number;
  taskTypeId: string;
  timeType: string;
  taskTypeName: string;
  priorityName: string;
  persionInChargedName: string;
  statusName: string;
  planEndTimeStr: string;
  colorPlanEndTimeStr: string;
  actualEndTimeStr: string;
  coloractualEndTimeStr: string;
  isDelete: boolean;
  isHavePic: boolean;
  noteNumber: number;
  createDateStr: string;
  listTaskResource: Array<TaskResourceMapping>;
  isCreate: boolean;
  backgroundColorForStatus: string;
  statusCode: string;
  icon: string;
  updateDateStr: string;
  soLanMoLai: number;

  index: number;

  constructor() {
    this.taskId = '00000000-0000-0000-0000-000000000000';
    this.projectId = '00000000-0000-0000-0000-000000000000';
    this.projectScopeId = '00000000-0000-0000-0000-000000000000';
    this.projectScopeName = null;
    this.taskCode = null;
    this.taskName = null;
    this.planStartTime = null;
    this.planEndTime = null;
    this.estimateHour = 0;
    this.estimateCost = 0;
    this.actualStartTime = null;
    this.actualEndTime = null;
    this.actualHour = 0;
    this.description = null;
    this.status = '00000000-0000-0000-0000-000000000000';
    this.priority = 0;
    this.milestonesId = '00000000-0000-0000-0000-000000000000';
    this.includeWeekend = false;
    this.isSendMail = false;
    this.taskComplate = 0;
    this.createBy = '00000000-0000-0000-0000-000000000000';
    this.createDate = new Date();
    this.updateBy = null;
    this.updateDate = null;
    this.taskTypeId = '00000000-0000-0000-0000-000000000000';
    this.timeType = '';
    this.planEndTimeStr = '';
    this.colorPlanEndTimeStr = "";
    this.isDelete = false;
    this.isHavePic = true;
    this.noteNumber = 0;
    this.createDateStr = '';
    this.listTaskResource = [];
    this.isCreate = false;
    this.index = 0;
    this.backgroundColorForStatus = '';
    this.updateDateStr = '';
    this.soLanMoLai= null;
  }
}

export class TaskResourceMapping {
  taskResourceMappingId: string;
  taskId: string;
  resourceId: string;
  hours: number;
  isPersonInCharge: boolean;
  isChecker: boolean;

  constructor() {
    this.taskResourceMappingId = '00000000-0000-0000-0000-000000000000';
    this.taskId = '00000000-0000-0000-0000-000000000000';
    this.resourceId = '00000000-0000-0000-0000-000000000000';
    this.hours = 0;
    this.isPersonInCharge = false;
    this.isChecker = false;
  }

}

export class TaskDocument {
  taskDocumentId: string;
  taskId: string;
  documentName: string;
  documentSize: string;
  documentUrl: string;
  active: boolean;
  createdById: string;
  createdDate: Date;
  updatedById: string;
  updatedDate: Date;
  uploadByName: string;
  createByName: string;

  constructor() {
    this.taskDocumentId = '00000000-0000-0000-0000-000000000000';
    this.taskId = '00000000-0000-0000-0000-000000000000';
    this.documentName = '';
    this.documentSize = '';
    this.documentUrl = '';
    this.active = true;
    this.createdById = JSON.parse(localStorage.getItem('auth')).UserId;
    this.createdDate = new Date();
    this.updatedById = JSON.parse(localStorage.getItem('auth')).UserId;
    this.updatedDate = new Date();
    this.uploadByName = '';
    this.createByName = '';
  }
}
