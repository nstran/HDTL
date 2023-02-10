export class TaskRelate {
    RelateTaskMappingId: string;
    relateTaskId: string;
    ProjectId: string;
    TaskId: string;
   

    createdDate: Date;
    createdById: string;
    updatedDate: Date;
    updatedById: string;

    taskCode: string;
    taskName: string;
    expectedEndDate: Date;
    expectedStartDate: Date;
    status: string;
    statusName: string;

    statusCode: string;
    backgroundColorForStatus: string;

    
    constructor() {
        this.RelateTaskMappingId = '00000000-0000-0000-0000-000000000000';
        this.relateTaskId = '00000000-0000-0000-0000-000000000000';
        this.ProjectId = '00000000-0000-0000-0000-000000000000';   
        this.TaskId = '00000000-0000-0000-0000-000000000000';       
        this.createdById = '00000000-0000-0000-0000-000000000000';
        this.createdDate = new Date();
        this.updatedById = null;
        this.updatedDate = null;
    }
}