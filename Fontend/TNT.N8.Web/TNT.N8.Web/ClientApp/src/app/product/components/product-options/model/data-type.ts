import { OptionsEntityModel } from "./options";

export class GetGiaTriThuocTinhModel {
    getGiaTriThuocTinh: GetGiaTriThuocTinh[]
    statusCode: number;
    messageCode: string;
}

export class GetGiaTriThuocTinh {
    name: string
    value: number
    key: any
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
    isUsing: boolean | null;
    objectId: string;
}
export class CreateOptionAndAttrResult {
    statusCode: number;
    messageCode: string;
}

export class CreateOptionAndAttrModel {
    attributeConfigurationModel: AttributeConfigurationModel;
    optionsEntityModel: OptionsEntityModel
}
