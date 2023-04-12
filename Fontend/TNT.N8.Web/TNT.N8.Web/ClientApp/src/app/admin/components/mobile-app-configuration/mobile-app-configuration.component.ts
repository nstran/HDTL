import { ChangeDetectorRef, Component, Injector, OnInit } from '@angular/core';
import { AdvertisementConfigurationEntityModel, MobileAppConfigurationEntityModel, MobileAppConfigurationImage, PaymentMethodConfigure } from '../../models/mobile-app-configuraton.models';
import { AbstractBase } from '../../../shared/abstract-base.component';
import { MobileAppConfigurationService } from '../../services/mobile-app-configuration.service';
import { tap } from 'rxjs/operators';
import { CategoryEntityModel } from '../../../../../src/app/product/models/product.model';
import { GetPermission } from '../../../../../src/app/shared/permission/get-permission';
import { Router } from '@angular/router';

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
  columnAdvertisement = [];
  listPayMentCategory: Array<CategoryEntityModel> = [];
  listPayMent: Array<PaymentMethodConfigure> = [];
  listAdvertisementConfigurationEntityModel : AdvertisementConfigurationEntityModel[] = [];
  images: MobileAppConfigurationImage[] = [];

  constructor(
    injector: Injector,
    private _mobileAppConfigurationService: MobileAppConfigurationService,
    private changeDetector: ChangeDetectorRef,
    private getPermission: GetPermission,
    private router: Router
    ) {
    super(injector)
  }

  async ngOnInit() {
    this.takeMobileAppConfiguration();
    this.selectedColumns2 = [
      { field: 'categoryName', header: 'Loại hình thức', width: '80px', textAlign: 'left', color: '#f44336' },
      { field: 'content', header: 'Nội dung', width: '250px', textAlign: 'left', color: '#f44336' },
      { field: 'action', header: 'Thao tác', width: '80px', textAlign: 'center', color: '#f44336' },
    ];

    this.columnAdvertisement = [
      { field: 'sortOrder', header: 'STT', width: '30px', textAlign: 'right', color: '#f44336' },
      { field: 'image', header: 'Ảnh đại diện', width: '120px', textAlign: 'left', color: '#f44336' },
      { field: 'title', header: 'Tiêu đề', width: '120px', textAlign: 'left', color: '#f44336' },
      { field: 'content', header: 'Nội dung', width: '120px', textAlign: 'center', color: '#f44336' },
      { field: 'action', header: 'Thao tác', width: '30px', textAlign: 'center', color: '#f44336' }
    ];

    let resource = "admin/mobile-app-config";
    let permission: any = await this.getPermission.getPermission(resource);
    if (permission.status == false) {
      let mgs = { severity: 'warn', summary: 'Thông báo:', detail: 'Bạn không có quyền truy cập vào đường dẫn này vui lòng quay lại trang chủ' };
      this.showMessage(mgs);
      this.router.navigate(['/home']);
    }
  }

  //#region Image
  getFileList(): File[] {
    let files: File[] = [];
    this.images.forEach((image, index) => {
      let file = this.convertDataURItoFile(image.source, image.imageName, image.imageType);
      files.push(file)
    });
    return files;
  }

  convertDataURItoFile(dataURI: string, fileName: string, type: string): File {
    var byteString = atob(dataURI.split(',')[1]);
    var mimeString = dataURI.split(',')[0].split(':')[1].split(';')[0]
    var ab = new ArrayBuffer(byteString.length);
    var ia = new Uint8Array(ab);
    for (var i = 0; i < byteString.length; i++) {
      ia[i] = byteString.charCodeAt(i);
    }
    var blob: any = new Blob([ia], { type: mimeString });
    blob.lastModifiedDate = new Date();
    blob.name = fileName;
    let file = new File([blob], fileName, { type: type })
    return file;
  }

  readerFile(files: File[]): Promise<string>{
    return new Promise(resolve => {
      let fileName = "";
      let file = files[0];
      let reader = new FileReader();
      reader.readAsDataURL(file);
      reader.onload  = (reader: any) => {
        let newImage = new MobileAppConfigurationImage();
        newImage.source = reader.target.result; //base64
        newImage.imageSize = file.size;
        newImage.imageName = file.name;
        fileName = file.name;
        newImage.imageType = file.type;
        newImage.title = '';
        newImage.alt = '';
        var existImage = this.images.find(x => x.imageName == file.name);
        if(!existImage){
          this.images = [...this.images, newImage];
        }
        resolve(fileName);
      };
    });
  }

  async uploadImageIntro(event: { files: File[] }): Promise<void> {
    this.base64ImageIntro = await this.getBase64ImageFromURL(event);
    this.mobileAppConfiguration.introduceImageOrVideo = this.base64ImageIntro.toString();
    this.mobileAppConfiguration.introduceImageOrVideoName = await this.readerFile(event.files);
  }

  removeImageIntro(): void {
    this.base64ImageIntro = undefined;
    this.mobileAppConfiguration.introduceImageOrVideo = undefined;
  }

  async uploadImageLoginAndResterScreen(event: { files: File[] }): Promise<void> {
    this.base64ImageLoginAndResterScreen = await this.getBase64ImageFromURL(event);
    this.mobileAppConfiguration.loginAndRegisterScreenImage = this.base64ImageLoginAndResterScreen.toString();
    this.mobileAppConfiguration.loginAndRegisterScreenImageName = await this.readerFile(event.files);
  }

  removeImageLoginAndResterScreen(): void {
    this.base64ImageLoginAndResterScreen = undefined;
    this.mobileAppConfiguration.loginAndRegisterScreenImage = undefined;
  }

  async uploadIconLogin(event: { files: File[] }): Promise<void> {
    this.base64IconLogin = await this.getBase64ImageFromURL(event);
    this.mobileAppConfiguration.loginScreenIcon = this.base64IconLogin.toString();
    this.mobileAppConfiguration.loginScreenIconName = await this.readerFile(event.files);
  }

  removeIconLogin(): void {
    this.base64IconLogin = undefined;
    this.mobileAppConfiguration.loginScreenIcon = undefined;
  }

  async uploadIconPaymentScreenTransfer(event: { files: File[] }): Promise<void> {
    this.base64PaymentScreenIconTransfer = await this.getBase64ImageFromURL(event);
    this.mobileAppConfiguration.paymentScreenIconTransfer = this.base64PaymentScreenIconTransfer.toString();
    this.mobileAppConfiguration.paymentScreenIconTransferName = await this.readerFile(event.files);
  }

  removeIconPaymentScreenTransfer(): void {
    this.base64PaymentScreenIconTransfer = undefined;
    this.mobileAppConfiguration.paymentScreenIconTransfer = undefined;
  }

  async uploadIconPaymentScreenVNPAY(event: { files: File[] }): Promise<void> {
    this.base64PaymentScreenIconVNPAY = await this.getBase64ImageFromURL(event);
    this.mobileAppConfiguration.paymentScreenIconVnpay = this.base64PaymentScreenIconVNPAY.toString();
    this.mobileAppConfiguration.paymentScreenIconTransferName = await this.readerFile(event.files);
  }

  removeIconPaymentScreenVNPAY(): void {
    this.base64PaymentScreenIconVNPAY = undefined;
    this.mobileAppConfiguration.paymentScreenIconVnpay = undefined;
  }

  async uploadImageNotice(event: { files: File[] }): Promise<void> {
    this.base64ImageNotice = await this.getBase64ImageFromURL(event);
    this.mobileAppConfiguration.orderNotificationImage = this.base64ImageNotice.toString();
    this.mobileAppConfiguration.orderNotificationImageName = await this.readerFile(event.files);
  }

  removeImageNotice(): void {
    this.base64ImageNotice = undefined;
    this.mobileAppConfiguration.orderNotificationImage = undefined;
  }

  chosePaymentMethod(data: CategoryEntityModel, rowData): void {
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

  getFile(source: string, fileName: string) {
    if(source && fileName){
      let newImage = new MobileAppConfigurationImage();
      newImage.source = source; //base64
      newImage.imageName = fileName;
      newImage.imageType = fileName.split('.').pop();
      this.images = [...this.images, newImage];
    }
  }

  //#endregion

  takeMobileAppConfiguration(): void {
    this.loading = true;
    this._mobileAppConfigurationService.takeMobileAppConfiguration()
      .pipe(tap(() => this.loading = false))
      .subscribe(result => {
        if (result.mobileAppConfigurationEntityModel) {
          this.mobileAppConfiguration = result.mobileAppConfigurationEntityModel;
          this.getFile(this.mobileAppConfiguration.introduceImageOrVideo, this.mobileAppConfiguration.introduceImageOrVideoName);
          this.getFile(this.mobileAppConfiguration.loginAndRegisterScreenImage, this.mobileAppConfiguration.loginAndRegisterScreenImageName);
          this.getFile(this.mobileAppConfiguration.loginScreenIcon, this.mobileAppConfiguration.loginScreenIconName);
          this.getFile(this.mobileAppConfiguration.orderNotificationImage, this.mobileAppConfiguration.orderNotificationImageName);
          this.getFile(this.mobileAppConfiguration.paymentScreenIconTransfer, this.mobileAppConfiguration.paymentScreenIconTransferName);
          this.listAdvertisementConfigurationEntityModel = result.listAdvertisementConfigurationEntityModel;
          this.listPayMentCategory = result.listPayMentCategory.length > 0 ? result.listPayMentCategory : [];
          this.listPayMent = result.listPayMent ? result.listPayMent.length > 0 ? result.listPayMent : [] : [];
        }
      })
  }

  async save(): Promise<void> {
    this.loading = true;
    let fileList: File[] = this.getFileList();
    let uploadResult: any = await this._mobileAppConfigurationService.uploadMobileAppConfigurationImage(fileList);
    if(uploadResult){
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
    } else {
      this.loading = false;
    }
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

  async onRowRemoveChild(rowData: PaymentMethodConfigure): Promise<void> {
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

  async uploadImageAdvertisement(event: { files: File[] }, index: number): Promise<void> {
    this.listAdvertisementConfigurationEntityModel[index].image = await (await this.getBase64ImageFromURL(event)).toString();
  }

  removeImageAdvertisement(index: number): void {
    this.listAdvertisementConfigurationEntityModel[index].image = undefined;
  }

  addAdvertisement(): void {
    let advertisementConfigurationEntityModel = new AdvertisementConfigurationEntityModel();
    advertisementConfigurationEntityModel.image = "";
    advertisementConfigurationEntityModel.title = "";
    advertisementConfigurationEntityModel.content = "";
    advertisementConfigurationEntityModel.sortOrder = null;
    advertisementConfigurationEntityModel.edit = false;
    this.listAdvertisementConfigurationEntityModel.push(advertisementConfigurationEntityModel);
  }

  /** Xử lý row con */
  onRowEditSaveAdvertisement(rowData: AdvertisementConfigurationEntityModel): void {
    if (!rowData.title && !rowData.content) {
      let msg = { severity: 'error', summary: 'Thông báo:', detail: 'Hãy nhập đầy đủ thông tin!' };
      this.showMessage(msg);
      return;
    }
    this.loading = false;
    this._mobileAppConfigurationService.createOrEditAdvertisementConfiguration(rowData)
      .pipe(tap(() => this.loading = false))
      .subscribe(result => {
        if (result.statusCode == 200) {
          this.showToast('success', 'Thông báo', 'Lưu thành công');
          rowData.edit = false;
        } else {
          this.showToast('error', 'Thông báo', result.messageCode);
          rowData.edit = false;
        }
      })
  }

  onRowEditAdvertisementInitChild(rowData: AdvertisementConfigurationEntityModel): void {
    rowData.edit = !rowData.edit;
  }

  onRowDeleteAdvertisement(rowData: AdvertisementConfigurationEntityModel): void {
    if(!rowData.id){
      this.listAdvertisementConfigurationEntityModel = this.listAdvertisementConfigurationEntityModel.filter(e => e != rowData);
      return;
    }
    this.confirmationService.confirm({
      message: `Bạn có chắc chắn xóa dòng này?`,
      accept: async () => {
        this._mobileAppConfigurationService.deleteAdvertisementConfiguration(rowData.id)
          .pipe(tap(() => this.loading = false))
          .subscribe(result => {
            if (result.statusCode == 200) {
              this.showToast('success', 'Thông báo', 'Xóa thành công');
              this.listAdvertisementConfigurationEntityModel = this.listAdvertisementConfigurationEntityModel.filter(e => e != rowData);
            } else {
              this.showToast('error', 'Thông báo', result.messageCode);
            }
          })
      }
    });
  }

}
