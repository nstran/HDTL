export class TaskContraint {
    taskConstraintId: string;
    taskId: string;
    parentId: string;
    projectId: string;
    constraintType: string;
    constraingRequired: boolean;
    delayTime: number;
    createdDate: Date;
    createdById: string;
    updatedDate: Date;
    updatedById: string;

    taskCode: string;
    taskName: string;
    planStartTime: Date;
    planEndTime: Date;
    status: string;
    statusName: string;
    
    constructor() {
        this.taskConstraintId = '00000000-0000-0000-0000-000000000000';
        this.taskId = '00000000-0000-0000-0000-000000000000';
        this.parentId = '00000000-0000-0000-0000-000000000000';
        this.projectId = '00000000-0000-0000-0000-000000000000';
        this.constraintType = '00000000-0000-0000-0000-000000000000';
        this.constraingRequired = false;
        this.delayTime = 0;
        this.createdById = '00000000-0000-0000-0000-000000000000';
        this.createdDate = new Date();
        this.updatedById = null;
        this.updatedDate = null;
    }
}