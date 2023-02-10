export class TimeSheet {
    timeSheetId: string;
    taskId: string;
    personInChargeId: string;
    date: Date;
    spentHour: number;
    note: string;
    createdDate: Date;
    createdById: string;
    updatedDate: Date;
    updatedById: string;
    status: string;
    timeType: string;
    toDate: Date;
    fromDate: Date;
    listTimeSheetDetail: Array<TimeSheetDetail>;
    createByName: string;
    updateByName: string;
    isAdminApproval: boolean;

    personInChargeName: string;
    taskCodeName: string;
    weekName: string;
    statusCode: string;
    statusName: string;
    timeTypeName: string;
    monday: number;
    tuesday: number;
    wednesday: number;
    thursday: number;
    friday: number;
    saturday: number;
    sunday: number;

    listNote: Array<any>;

    constructor() {
        this.timeSheetId = '00000000-0000-0000-0000-000000000000';
        this.taskId = '00000000-0000-0000-0000-000000000000';
        this.personInChargeId = '00000000-0000-0000-0000-000000000000';
        this.date = null;
        this.spentHour = 0;
        this.note = '';
        this.createdDate = new Date();
        this.createdById = '00000000-0000-0000-0000-000000000000';
        this.updatedDate = null;
        this.updatedById = '00000000-0000-0000-0000-000000000000';
        this.status = '00000000-0000-0000-0000-000000000000';
        this.timeType = '00000000-0000-0000-0000-000000000000';
        this.toDate = null;
        this.fromDate = null;
        this.isAdminApproval = false;

        this.personInChargeName = '';
        this.taskCodeName = '';
        this.weekName = '';
        this.statusCode = '';
        this.statusName = '';
        this.timeTypeName = '';
        this.monday = 0;
        this.tuesday = 0;
        this.wednesday = 0;
        this.thursday = 0;
        this.friday = 0;
        this.saturday = 0;
        this.sunday = 0;

        this.listTimeSheetDetail = [];
        this.listNote = [];
    }
}

export class TimeSheetDetail {
    timeSheetDetailId: string;
    timeSheetId: string;
    date: Date;
    dayOfWeek: string;
    spentHour: number;
    status: number;

    constructor(note: string, date: Date) {
        this.timeSheetDetailId = '00000000-0000-0000-0000-000000000000';
        this.timeSheetId = '00000000-0000-0000-0000-000000000000';
        this.date = date;
        this.spentHour = 0;
        this.status = 0;
        this.dayOfWeek = note;
    }
}

