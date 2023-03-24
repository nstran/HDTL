import { CategoryEntityModel } from "../../../../src/app/product/models/product.model";

export class MobileAppConfigurationEntityModel {
    id: string;
    introduceColor: string;
    introduceImageOrVideo: string;
    introduceSologan: string;
    loginAndRegisterScreenImage: string;
    loginScreenColor: string;
    loginScreenIcon: string;
    paymentScreenIconVnpay: string;
    isPaymentScreenIconVnpay: boolean | null;
    paymentScreenContentVnpay: string;
    orderNotificationImage: string;
    createdById: string;
    updatedById: string;
    tenantId: string;
    createdDate: string | null;
    updatedDate: string | null;
    paymentScreenIconTransfer: string;
    isPaymentScreenIconTransfer: boolean | null;
    paymentScreenContentTransfer: string;
}

export class TakeMobileAppConfigurationResult {
    mobileAppConfigurationEntityModel : MobileAppConfigurationEntityModel;
    listPayMentCategory : Array<CategoryEntityModel>;
    listPayMent : Array<PaymentMethodConfigure>;
    listAdvertisementConfigurationEntityModel : AdvertisementConfigurationEntityModel[];
}

export class CreateOrEditMobileAppConfigurationResult {
    statusCode: number;
    messageCode: string;
}

export class CreateOrEditMobileAppConfigurationParameter {
    mobileAppConfigurationEntityModel : MobileAppConfigurationEntityModel;
}

export class PaymentMethodConfigure {
    id: string;
    categoryId: string;
    categoryName: string;
    categoryCode: string;
    categoryObject: any;
    content: string;
    status: number;
    edit: boolean;
    createdById: string;
    createdDate: string;
    updatedById: string;
    updatedDate: string;
    tenantId: string;
}

export class AdvertisementConfigurationEntityModel {
    id: string | null;
    image: string;
    title: string;
    content: string;
    sortOrder: number | null;
    edit: boolean;
}
