import { CategoryEntityModel } from "../../../../src/app/product/models/product.model";

export class MobileAppConfigurationEntityModel {
    id: string | null;
    introduceColor: string;
    introduceImageOrVideo: string;
    introduceImageOrVideoName: string;
    introduceSologan: string;
    loginAndRegisterScreenImage: string;
    loginAndRegisterScreenImageName: string;
    loginScreenColor: string;
    loginScreenIcon: string;
    loginScreenIconName: string;
    paymentScreenIconVnpay: string;
    isPaymentScreenIconVnpay: boolean | null;
    paymentScreenContentVnpay: string;
    orderNotificationImage: string;
    orderNotificationImageName: string;
    createdById: string | null;
    updatedById: string | null;
    tenantId: string | null;
    createdDate: string | null;
    updatedDate: string | null;
    paymentScreenIconTransfer: string;
    paymentScreenIconTransferName: string;
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
    imageName: string;
    title: string;
    content: string;
    sortOrder: number | null;
    edit: boolean;
}

export class MobileAppConfigurationImage {
    source: string | null;
    imageSize: number;
    imageName: string;
    imageType: string;
    imageUrl: string;
    alt: string | null;
    title: string | null;
  }