import { ChangeDetectorRef, Component, Injector, OnInit } from '@angular/core';
import { MobileAppConfigurationEntityModel, PaymentMethodConfigure } from '../../models/mobile-app-configuraton.models';
import { AbstractBase } from '../../../shared/abstract-base.component';
import { MobileAppConfigurationService } from '../../services/mobile-app-configuration.service';
import { tap } from 'rxjs/operators';
import { CategoryEntityModel } from '../../../../../src/app/product/models/product.model';

@Component({
  selector: 'app-mobile-app-configuration',
  templateUrl: './mobile-app-configuration.component.html',
  styleUrls: ['./mobile-app-configuration.component.css']
})
export class MobileAppConfigurationComponent extends AbstractBase implements OnInit {
  mobileAppConfiguration: MobileAppConfigurationEntityModel = new MobileAppConfigurationEntityModel();
  loading: boolean = false;
  introScreenColor: string;
  ratioImageIntro: string = '1:1';
  base64ImageIntro: string | ArrayBuffer;
  base64ImageLoginAndResterScreen: string | ArrayBuffer;
  base64IconLogin: string | ArrayBuffer;
  base64PaymentScreenIconTransfer: string | ArrayBuffer;
  base64PaymentScreenIconVNPAY: string | ArrayBuffer;
  base64ImageNotice: string | ArrayBuffer;
  isImage: boolean = true;
  selectedColumns2 = [];
  listPayMentCategory: Array<CategoryEntityModel> = [];
  listPayMent: Array<PaymentMethodConfigure> = [];

  constructor(
    injector: Injector,
    public _mobileAppConfigurationService: MobileAppConfigurationService,
    public changeDetector: ChangeDetectorRef
  ) {
    super(injector)
  }

  ngOnInit(): void {
    this.takeMobileAppConfiguration();

    this.selectedColumns2 = [
      { field: 'categoryName', header: 'Loại hình thức', width: '80px', textAlign: 'left', color: '#f44336' },
      { field: 'content', header: 'Nội dung', width: '250px', textAlign: 'left', color: '#f44336' },
      { field: 'action', header: 'Thao tác', width: '80px', textAlign: 'center', color: '#f44336' },
    ];
  }

  //#region Image
  async uploadImageIntro(event: { files: File[] }): Promise<void> {
    this.base64ImageIntro = await this.getBase64ImageFromURL(event);
    this.mobileAppConfiguration.introduceImageOrVideo = this.base64ImageIntro.toString();
  }

  removeImageIntro(): void {
    this.base64ImageIntro = undefined;
    this.mobileAppConfiguration.introduceImageOrVideo = undefined;
  }

  async uploadImageLoginAndResterScreen(event: { files: File[] }): Promise<void> {
    this.base64ImageLoginAndResterScreen = await this.getBase64ImageFromURL(event);
    this.mobileAppConfiguration.loginAndRegisterScreenImage = this.base64ImageLoginAndResterScreen.toString();
  }

  removeImageLoginAndResterScreen(): void {
    this.base64ImageLoginAndResterScreen = undefined;
    this.mobileAppConfiguration.loginAndRegisterScreenImage = undefined;
  }

  async uploadIconLogin(event: { files: File[] }): Promise<void> {
    this.base64IconLogin = await this.getBase64ImageFromURL(event);
    this.mobileAppConfiguration.loginScreenIcon = this.base64IconLogin.toString();
  }

  removeIconLogin(): void {
    this.base64IconLogin = undefined;
    this.mobileAppConfiguration.loginScreenIcon = undefined;
  }

  async uploadIconPaymentScreenTransfer(event: { files: File[] }): Promise<void> {
    this.base64PaymentScreenIconTransfer = await this.getBase64ImageFromURL(event);
    this.mobileAppConfiguration.paymentScreenIconTransfer = this.base64PaymentScreenIconTransfer.toString();
  }

  removeIconPaymentScreenTransfer(): void {
    this.base64PaymentScreenIconTransfer = undefined;
    this.mobileAppConfiguration.paymentScreenIconTransfer = undefined;
  }

  async uploadIconPaymentScreenVNPAY(event: { files: File[] }): Promise<void> {
    this.base64PaymentScreenIconVNPAY = await this.getBase64ImageFromURL(event);
    this.mobileAppConfiguration.paymentScreenIconVnpay = this.base64PaymentScreenIconVNPAY.toString();
  }

  removeIconPaymentScreenVNPAY(): void {
    this.base64PaymentScreenIconVNPAY = undefined;
    this.mobileAppConfiguration.paymentScreenIconVnpay = undefined;
  }

  async uploadImageNotice(event: { files: File[] }): Promise<void> {
    this.base64ImageNotice = await this.getBase64ImageFromURL(event);
    this.mobileAppConfiguration.orderNotificationImage = this.base64ImageNotice.toString();
  }

  removeImageNotice(): void {
    this.base64ImageNotice = undefined;
    this.mobileAppConfiguration.orderNotificationImage = undefined;
  }

  chosePaymentMethod(data, rowData) {
    rowData.categoryName = data.categoryName;
    rowData.categoryId = data.categoryId;
  }

  getBase64ImageFromURL(event: { files: File[] }): Promise<string | ArrayBuffer | null> {
    return new Promise(resolve => {
      let file = event.files[0];
      let reader = new FileReader();
      reader.readAsDataURL(file);
      reader.onload = function () {
        resolve(reader.result);
      };
    });
  }
  //#endregion

  takeMobileAppConfiguration(): void {
    this.loading = true;
    this._mobileAppConfigurationService.takeMobileAppConfiguration()
      .pipe(tap(() => this.loading = false))
      .subscribe(result => {
        if (result.mobileAppConfigurationEntityModel) {
          this.mobileAppConfiguration = result.mobileAppConfigurationEntityModel;
          this.listPayMentCategory = result.listPayMentCategory.length > 0 ? result.listPayMentCategory : [];
          this.listPayMent = result.listPayMent.length > 0 ? result.listPayMent : [];
        }
      })
  }

  save(): void {
    this.loading = true;
    this._mobileAppConfigurationService.createOrEditMobileAppConfiguration(this.mobileAppConfiguration)
      .pipe(tap(() => this.loading = false))
      .subscribe(result => {
        if (result.statusCode == 200) {
          this.showToast('success', 'Thông báo', 'Lưu thành công');
          this.takeMobileAppConfiguration();
        } else {
          this.showToast('error', 'Thông báo', 'Lưu thất bại');
        }
      })
  }

  ngAfterContentChecked(): void {
    this.changeDetector.detectChanges();
  }

  onRowEditInitChild(rowData: PaymentMethodConfigure): void {
    rowData.edit = !rowData.edit;
  }

  addPaymentMethod(): void {
    var newOrderExten = new PaymentMethodConfigure();
    if(this.listPayMentCategory.length > 0){
      newOrderExten.categoryObject = this.listPayMentCategory[0];
      newOrderExten.categoryId = this.listPayMentCategory[0].categoryId;
      newOrderExten.categoryName = this.listPayMentCategory[0].categoryName;
      this.listPayMent.push(newOrderExten);
    }
  }

  async onRowRemoveChild(rowData: PaymentMethodConfigure) {
    if(!rowData.id){
      this.listPayMent = this.listPayMent.filter(e => e != rowData);
      return;
    }
    this.confirmationService.confirm({
      message: `Bạn có chắc chắn xóa dòng này?`,
      accept: async () => {
        this._mobileAppConfigurationService.deletePaymentMethod(rowData)
          .pipe(tap(() => this.loading = false))
          .subscribe(result => {
            if (result.statusCode == 200) {
              this.showToast('success', 'Thông báo', 'Xóa thành công');
              this.listPayMent = this.listPayMent.filter(e => e != rowData);
            } else {
              this.showToast('error', 'Thông báo', result.messageCode);
            }
          })
      }
    });
  }

  /** Xử lý row con */
  onRowEditSaveChild(rowData: PaymentMethodConfigure): void {
    if (!rowData.categoryId || rowData.categoryId == '' || !rowData.content || rowData.content == '') {
      let msg = { severity: 'error', summary: 'Thông báo:', detail: 'Hãy nhập đầy đủ thông tin!' };
      this.showMessage(msg);
      return;
    }
    this.loading = true;
    this._mobileAppConfigurationService.createOrUpdatePaymentMethod(rowData)
      .pipe(tap(() => this.loading = false))
      .subscribe(result => {
        if (result.statusCode == 200) {
          this.showToast('success', 'Thông báo', 'Lưu thành công');
          this.listPayMent = this.listPayMent.filter(e => e != rowData);
          result.payment.edit = false;
          this.listPayMent.push(result.payment);
        } else {
          this.showToast('error', 'Thông báo', result.messageCode);
          rowData.edit = false;
        }
      })
  }

  onRowEditCancelChild(rowData: PaymentMethodConfigure): void {
    rowData.edit = !rowData.edit;
  }

  addAdvertisement(): void {
    
  }

}
