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
}

export class NotificationFireBase {
    content: string | null;
    status : boolean | null;
    url : string;
    orderId : string | null;
    date: string | null
    employeeId: string | null;
}