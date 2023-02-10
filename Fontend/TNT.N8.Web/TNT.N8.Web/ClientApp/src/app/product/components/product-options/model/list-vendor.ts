export class VendorModel {
    statusCode : number;
    messageCode : string;
    vendorList: VendorListModel[];
}

export class VendorListModel {
    vendorId: string;
    vendorName: string
    vendorCode: string;
    vendorGroupId: string;
    vendorGroupName: string;
    active: boolean;
    contactId: string;
    countVendorInformation: number;
    createdById: string;
    createdDate: string;
    paymentId: string;
    totalPayableValue: number;
    totalPurchaseValue: number;
    canDelete: boolean;
    address: string;
    email: string;
    phoneNumber: string
    listoptionId: Array<string>;
}
export class VendorGroupModel {
    categoryId: string;
    categoryName: string;
    categoryCode: string;
  }