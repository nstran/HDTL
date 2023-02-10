export class ListCategoryAttributeModel {
    listCategoryAttributes: ListCategoryAttribute[];
    statusCode: number;
    messageCode: string;
}

export class ListCategoryAttribute {
    id: string;
    categoryName: string;
    dataType: string;
    dataTypeValue: number;
    categoryId: string;
}
export class AttributeConfigureResult {
    statusCode: number;
    messageCode: string;
}