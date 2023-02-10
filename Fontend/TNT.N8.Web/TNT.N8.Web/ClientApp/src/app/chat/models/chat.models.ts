export class TakeDataFireBaseByUserIdResult {
    listFireBaseEntityModel : FireBaseEntityModel[];
}

export class FireBaseEntityModel {
    id: string;
    userId: string;
    userName: string;
    roomName: string;
    createdById: string;
    createdDate: string;
    updatedById: string;
    updatedDate: string;
    tenantId: string;
    otherName : string;
}

export class TakeListEmployeeByUserNameParameter {
    listEmployeeEntityModel : EmployeeEntityModel[]
}

export class EmployeeEntityModel {
    userId: string | null;
    userName: string | null;
    employeeId: string | null;
    employeeName: string | null;
    avatarUrl: string | null;
}

export class CreateDataFireBaseResult {
    statusCode: number;
    message: string;
    roomname: string;
}