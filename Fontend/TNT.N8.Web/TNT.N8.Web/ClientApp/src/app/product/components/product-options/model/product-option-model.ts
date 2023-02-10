export class ProductOptionModel {
    statusCode : number;
    messageCode : string;
    listOptions : ListOptionModel[];
}

export class ListOptionModel {
    id:             string;
    type:           string;
    name:           string;
    description:    string;
    note:           string;
    enableNumber:   boolean;
    isShowQuantity: boolean;
    createdById:    string;
    createdDate:    string;
    updatedById:    string;
    updatedDate:    string;
    tenantId:       string;
    parentId:       string;
    price: number;
}
export class OptionByIdResult {
    statusCode : number;
    messageCode : string;
}
export class OptionByIdTable {
    statusCode : number;
    messageCode : string;
    options:        null;
    listOptions:    null;
    listOptionById: OptionByIdModel;
}
export class OptionByIdModel {
    id: string;
    categoryId: string;
    name: string;
    description: string;
    price:             number;
}
// export interface Test {
//     options:        null;
//     listOptions:    null;
//     listOptionById: ListOptionByID;
//     status:         boolean;
//     message:        null;
//     notes:          null;
//     statusCode:     number;
//     messageCode:    null;
// }

// export interface ListOptionByID {
//     optionId:          string;
//     optionName:        string;
//     optionDescription: string;
//     price:             number;
//     categoryName:      string;
//     categoryCode:      string;
//     categoryTypeName:  string;
//     categoryTypeCode:  string;
// }


