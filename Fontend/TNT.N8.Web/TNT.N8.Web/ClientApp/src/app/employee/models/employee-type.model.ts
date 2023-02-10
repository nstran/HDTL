export class GetBenefitTypeModel {
    getListBenefitType: GetListBenefitType[]
    statusCode: number;
    messageCode: string;
}

export class GetListBenefitType {
    name: string
    value: number
    key: any
}