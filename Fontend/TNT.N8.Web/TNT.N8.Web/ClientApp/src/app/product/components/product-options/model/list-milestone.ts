import internal from "events";

export class ListMilestoneConfigurationModel {
    listMilestoneConfiguration: MilestoneConfigurationEntityModel[];
    statusCode: number;
    messageCode: string;
}

export class MilestoneConfigurationEntityModel {
    id: string;
    scopeReport: string;
    name: string;
    clientView: boolean | null;
    deadline: string | null;
    status: string;
    content: string;
    employeeId: string | null;
    workingOrder: number | null;
    note: string;
    createdById: string;
    createdDate: string;
    updatedById: string;
    updatedDate: string;
    tenantId: string;
    optionId: string;
    sortOrder: number;
}
export class CreateOrUpdateMilestoneConfigurationResult {
    statusCode: number;
    messageCode: string;
}