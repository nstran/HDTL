export class ProjectMilestoneModel {
  projectMilestonesId: string;
  name: string;
  description: string;
  status: string;
  endTime: Date;
  projectId: string;
  createdById: string;
  createdDate: Date;
  updateBy: string;
  updateDate: Date;
  delayNumber: number;
  taskNumber: number;
  taskInProgressNumber: number;
  taskCloseNumber: number;
  createByName: string;
  isShow: boolean;
  index: number;
  dayOfWeek: number;

  constructor() {
    this.projectMilestonesId = '00000000-0000-0000-0000-000000000000';
    this.projectId = '00000000-0000-0000-0000-000000000000';
    this.name = '';
    this.description = null;
    this.status = null;
    this.endTime = null;
    this.createdById = '00000000-0000-0000-0000-000000000000';
    this.createdDate = new Date();
    this.updateBy = null;
    this.updateDate = null;
    this.taskNumber = 0;
    this.taskInProgressNumber = 0;
    this.taskCloseNumber = 0;
    this.createByName = '';
    this.isShow = false;
    this.index = 0;
    this.dayOfWeek = 2;
  }
}
