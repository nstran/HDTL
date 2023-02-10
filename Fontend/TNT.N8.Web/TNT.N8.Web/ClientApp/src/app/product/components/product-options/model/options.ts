export class OptionsEntityModel {
    id: string;
    categoryId: string;
    name: string;
    description: string;
    note: string;
    enableNumber: boolean | null;
    isShowQuantity: boolean | null;
    createdById: string | null;
    createdDate: string | null;
    updatedById: string | null;
    updatedDate: string | null;
    tenantId: string | null;
    price: number | null;
    productId: string;
    parentId: null | string;
    vat: number;
}
export class SearchOptionTreeResult {
    listOptions: ListOptions[];
    statusCode: number;
    messageCode: null;
}

export class ListOptions {
    categoryName: string;
    optionName: string;
    price: number;
    optionId: string;
    parentId: null | string;
    hasChild: boolean;
    vat: number;
    listOptions: ListOptions[];
}
export class CreateOrUpdateOptionsResult {
    statusCode: number;
    messageCode: string;
    optionsEntityModel: OptionsEntityModel;
}
export class OptionsEntityTable {
    statusCode: number;
    messageCode: string;
    optionsEntityModel: OptionsEntityModel;
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
}
export class AttributeConfigurationModel {
    id: string;
    dataType: number;
    categoryId: string;
    createdById: string;
    createdDate: string;
    updatedById: string;
    updatedDate: string;
    tenantId: string;
    objectType: number;
    objectId: string;
}
export class CreateOrUpdateOptionsModel {
    optionsEntityModel: OptionsEntityModel;
    attributeConfigurationModel: AttributeConfigurationModel;
    milestoneConfigurationEntityModel: MilestoneConfigurationEntityModel

}