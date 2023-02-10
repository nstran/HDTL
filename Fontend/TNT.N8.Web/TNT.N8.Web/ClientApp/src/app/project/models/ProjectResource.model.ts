export class ProjectResourceModel {
  projectResourceId: string;
  projectId: string;
  resourceType: string;
  resourceTypeName: string;
  nameResource: string;
  resourceName: string;
  scopeName: string;
  resourceRole: string;
  resourceRoleName: string;
  employeeRole: string;
  employeeRoleName: string;
  statusName: string;
  allowcation: number;
  startTime: Date;
  endTime: Date;
  isCreateVendor: boolean;
  totalDays: number;
  objectId: string;
  isPic: boolean;
  isCheck: boolean;
  isOverload: boolean;
  hours: number;
  workDay: number;
  listContact: Array<any>;
  vendor: any;
  listProjectVendor: Array<any>;
  backgroundColorForStatus: string;
  includeWeekend: boolean;
  isActive: boolean;
  chiPhiTheoGio: number;

  constructor() {
    this.isPic = false;
    this.isCheck = false;
    this.includeWeekend = false;
    this.chiPhiTheoGio = 0;
  }
}
