import { Component, Injector, OnInit } from '@angular/core';
import { tap } from 'rxjs/operators';
import { AbstractBase } from '../../../shared/abstract-base.component';
import { AttrConfigure, CategoryEntityModel, CreateOrUpdateServicePacketParameter, EmployeeEntityModel, NewTreeNode, NotificationConfigurationEntityModel, OptionsEntityModel, PermissionConfigurationEntityModel, ProductCategoryEntityModel, ProvinceEntityModel,RoleEntityModel,ServicePacketAttributeEntityModel, ServicePacketEntityModel, ServicePacketImageEntityModel, ServicePacketMappingOptionsEntityModel } from '../../models/product.model';
import { ProductService } from '../../services/product.service';
import { ActivatedRoute, Router } from '@angular/router';
import { FormGroup, FormControl, Validators, FormBuilder, FormArray } from '@angular/forms';
import { CauHinhQuyTrinh } from '../../../../../src/app/admin/models/cau-hinh-quy-trinh.model';
import { PhongBanTrongCacBuocQuyTrinh } from '../../../../../src/app/admin/models/phong-ban-trong-cac-buoc-quy-trinh.model';
import { DialogService } from 'primeng/dynamicdialog';
import { ChonNhieuDvDialogComponent } from '../../../shared/components/chon-nhieu-dv-dialog/chon-nhieu-dv-dialog.component';
import { CacBuocQuyTrinh } from '../../../../../src/app/admin/models/cac-buoc-quy-trinh.model';
import { OrganizationService } from '../../../shared/services/organization.service';
import { GetPermission } from '../../../../../src/app/shared/permission/get-permission';

@Component({
  selector: 'app-create-or-edit-product-packet',
  templateUrl: './create-or-edit-product-packet.component.html',
  styleUrls: ['./create-or-edit-product-packet.component.css'],
  providers: [DialogService]

})
export class CreateOrEditProductPacketComponent extends AbstractBase implements OnInit {
  servicePacketEntityModel: ServicePacketEntityModel = new ServicePacketEntityModel();
  //Obj add thuộc tính
  servicePacketAttributeEntityModelAdd: ServicePacketAttributeEntityModel = new ServicePacketAttributeEntityModel();

  //List cấu hình thuộc tính
  listServicePacketAttributeEntityModel: ServicePacketAttributeEntityModel[] = [];

  //MasterData 
  //List loại gói dịch vụ
  listProductCategoryEntityModel: ProductCategoryEntityModel[] = [];
  //List kiểu dữ liệu thuộc tính
  listDataTypeAttr: AttrConfigure[] = [];
  //List tên thuộc tính
  listAttrCategory: CategoryEntityModel[] = [];
  //List quyền thuộc tính
  listRoleServicePacket: RoleEntityModel[] = [];
  // List cấu hình quyền
  listServicePacketConfigurationPermissionModel : PermissionConfigurationEntityModel[] = [];
  //List Province
  listProvince : ProvinceEntityModel[];
  //List các bước select step
  listStepServicePacketSelect :PermissionConfigurationEntityModel[];

  productCategoryEntityModel: ProductCategoryEntityModel;
  loading: boolean = false;
  showModalAttr: boolean = false;
  showModalOption: boolean = false;
  showModalNoitiConfig: boolean = false;
  showErrorProductCategoryEntityModel : boolean = false;
  showErrorProvinceEntityModel : boolean = false;
  colsAttr: any[];
  colsRole: any[];
  colsNotiConfig: any[];
  rows = 10;
  servicePacketId: string;
  provinceEntityModel : ProvinceEntityModel;
  newStepServicePacket: PermissionConfigurationEntityModel;
  lastStep: number;
  oldStep : number;
  lastSortOrder: number;
  base64BackgroundImage : string | ArrayBuffer;
  base64MainImage: string | ArrayBuffer;
  base64Icon: string | ArrayBuffer;
  listOptionTreeNode: NewTreeNode[] = [];
  listOption: ServicePacketMappingOptionsEntityModel[] = [];
  listOptionEntityModel: OptionsEntityModel[] = [];
  colOptions: any[];
  servicePacketMappingOptionsEntityModel: ServicePacketMappingOptionsEntityModel = new ServicePacketMappingOptionsEntityModel();
  servicePacketImageEntityModel : ServicePacketImageEntityModel = new ServicePacketImageEntityModel();
  isLastOption : boolean = false;
  optionModelService : OptionsEntityModel;
  showErrorOptionModelService : boolean = false;
  editServicePMOption : boolean = false;
  showIsLastOption : boolean = false;
  servicePrice : number;
  serviceDescription: string;
  ratioMainImage : string = '2:1';
  ratioBackgroundImage : string = '2:1';
  ratioIconImage : string = '1:1';
  listEmpWithRole: EmployeeEntityModel[];
  listNotificationConfigurationModel: NotificationConfigurationEntityModel[] = [];
  notificationConfigurationEntityModel = new NotificationConfigurationEntityModel();
  isShowButtonActive: boolean = false;

  //Cấu hình phê duyệt
  cauHinhQuyTrinh = new CauHinhQuyTrinh();

  formTenCauHinh: FormGroup;
  tenCauHinhControl: FormControl;

  form: FormGroup = this.fb.group({
    quyTrinh: this.fb.array([]),
  });;

  //Danh sách quản lý dịch vụ
  listManager: EmployeeEntityModel[];


  get quyTrinh() {
    return this.form.get('quyTrinh') as FormArray;
  }

  listLoaiPheDuyet = [
    // { name: 'Phê duyệt trưởng bộ phận', value: 1 },
    { name: 'Phòng ban phê duyệt', value: 2 },
    { name: 'Phê duyệt quản lý gói dịch vụ', value: 3 },
  ];

  listOrganization: Array<any> = [];

  constructor(
    injector : Injector,
    private _productService: ProductService,
    private _activatedRoute: ActivatedRoute,
    private fb: FormBuilder,
    private dialogService: DialogService,
    private _router: Router,
    private organizationService: OrganizationService,
    private getPermission: GetPermission,
    private router: Router,

  ) {
    super(injector);
   }

   async ngOnInit() {
    this.setTable();

    let resource = "sal/product/product-packet-createOrUpdate";
    let permission: any = await this.getPermission.getPermission(resource);

    if(permission.listCurrentActionResource.includes("action")){
      this.isShowButtonActive = true;
    }

    if (permission.status == false) {
      this.router.navigate(['/home']);
    }

    this.getMasterDataServicePacket();

    this.tenCauHinhControl = new FormControl(null);

    this.formTenCauHinh = new FormGroup({
      tenCauHinhControl: this.tenCauHinhControl
    });

    this.form = this.fb.group({
      quyTrinh: this.fb.array([]),
    });

  }

  setTable(): void {
    this.colsAttr = [
      { field: 'categoryName', header: 'Tên thuộc tính', textAlign: "center" },
      { field: 'dataTypeName', header: 'Loại giá trị', textAlign: "center" },
    ];

    this.colsRole = [
      { field: 'stepName', header: 'Tên bước', textAlign: "center" },
      { field: 'roleId', header: 'Nhóm quyền thực hiện', textAlign: "center" },
      { field: 'listEmp', header: 'Nhân viên thực hiện', textAlign: "center" },
    ];

    this.colOptions = [
      { field: 'AttrName', header: 'Tên thuộc tính', width: '50%', textAlign: 'left', color: '#f44336' },
      { field: 'AttrValue', header: 'Giá trị', width: '50%', textAlign: 'left', color: '#f44336' },
    ];

    this.colsNotiConfig = [
      { field: 'categoryName', header: 'Sự kiện', width: '30%', textAlign: 'left', color: '#f44336' },
      { field: 'notificationRecipient', header: 'Người nhận thông báo', width: '70%', textAlign: 'left', color: '#f44336' }
    ]
  }
  
  closeModalAttr(): void {
    this.showModalAttr = false;
  }
  
  openModalAttr(): void {
    this.showModalAttr = true;
  }

  closeModalOption(): void {
    this.showModalOption = false;
  }

  getMasterDataServicePacket(): void {
    this.loading = true;
    this._productService.getMasterDataCreateServicePacket()
      .pipe(
        tap(() => {
          this.loading = false;
        })
      )
      .subscribe(result => {
        this.listProductCategoryEntityModel = result.listProductCategoryEntityModel;
        this.listDataTypeAttr = result.listDataTypeAttr;
        this.listAttrCategory = result.listAttrCategory;
        this.listRoleServicePacket = result.listRoleServicePacket;
        this.listProvince = result.listProvince;
        this.listStepServicePacketSelect = result.listStepServicePacketSelect;
        this.listServicePacketConfigurationPermissionModel = result.listServicePacketConfigurationPermissionModel.sort((a,b) => {return a.stepId - b.stepId});
        this.listOptionEntityModel = result.listOptionEntityModel;
        this.listEmpWithRole = result.listEmpWithRole;
        this.listNotificationConfigurationModel = result.listNotificationConfigurationEntityModel.sort((a,b) => {return a.sortOrder - b.sortOrder});

        this.listNotificationConfigurationModel.forEach(item => {
          item.listEmployeeEntityModel = [];
        });
        
        this._activatedRoute.params.subscribe(params => {
          this.servicePacketId = params['id'];
          if(this.servicePacketId){
            this.getServicePacketById(this.servicePacketId);
            this.getServicePacketOptionByServicePacketId(this.servicePacketId);
          }
        });
      })
  }

  getServicePacketById(servicePacketId : string): void {
    this.loading = true;
    this._productService.getServicePacketById(servicePacketId)
      .pipe(tap(() => this.loading = false))
      .subscribe(result => {
        (this.form.get('quyTrinh') as FormArray).clear();
        this.cauHinhQuyTrinh = new CauHinhQuyTrinh();
        this.cauHinhQuyTrinh = result.cauHinhQuyTrinh ?? new CauHinhQuyTrinh();

        this.organizationService.getAllOrganization().subscribe(response => {
          let result = <any>response;
          this.listOrganization = result.listAll;
          this.mapDataToForm(this.cauHinhQuyTrinh);
        });
        

        this.listManager = this.listEmpWithRole.filter(x => result.listManager.indexOf(x.employeeId) != -1);

        this.listServicePacketConfigurationPermissionModel = result.servicePacketEntityModel.listPermissionConfigurationEntityModel;
        this.listRoleServicePacket = result.servicePacketEntityModel.listRoleServicePacket;
        this.servicePacketEntityModel = result.servicePacketEntityModel;
        this.productCategoryEntityModel = this.listProductCategoryEntityModel.find(x => x.productCategoryId == result.servicePacketEntityModel.productCategoryId);
        this.listServicePacketAttributeEntityModel = result.servicePacketEntityModel.listServicePacketAttributeEntityModel;
        this.listNotificationConfigurationModel = result.servicePacketEntityModel.listNotificationConfigurationEntityModel.sort((a,b) => {return a.sortOrder - b.sortOrder});
        this.listNotificationConfigurationModel.forEach(x => {
          x = this.customerNotificationRecipient(x);
        })
        if(result.servicePacketImageEntityModel){
          this.base64MainImage = result.servicePacketImageEntityModel.mainImage;
          this.base64BackgroundImage = result.servicePacketImageEntityModel.backgroundImage;
          this.base64Icon = result.servicePacketImageEntityModel.icon;
          this.servicePacketImageEntityModel = result.servicePacketImageEntityModel;
        }

        if(this.listProvince.find(x => x.provinceId == result.servicePacketEntityModel.provinceId)){
          this.provinceEntityModel = this.listProvince.find(x => x.provinceId == result.servicePacketEntityModel.provinceId);
        }

        this.listServicePacketAttributeEntityModel.forEach(x => {
          if(this.listDataTypeAttr.find(y => y.value == x.objectType)){
            x.dataTypeName = this.listDataTypeAttr.find(y => y.value == x.dataType).name;
          }
        })
        this.listServicePacketConfigurationPermissionModel.forEach(x => {
          let stepObj = this.listServicePacketConfigurationPermissionModel.find(y => y.stepId == x.stepId);
          if(stepObj){
            if(this.listRoleServicePacket.find(i => i.roleId == stepObj.roleId)){
              x.objectRole = this.listRoleServicePacket.find(i => i.roleId == stepObj.roleId);
            };
          };
          let stepServicePacketConfiguration = this.listServicePacketConfigurationPermissionModel.find(y => y.stepId == x.stepId);
          if(stepServicePacketConfiguration){
            let listEmployeeId = stepServicePacketConfiguration.listEmployeeEntityModel.map(i => i.employeeId);
            x.listEmployeeEntityModel = this.listEmpWithRole.filter(x => listEmployeeId.indexOf(x.employeeId) != -1);
            x.listEmployeeEntityModelByRoleId = this.listEmpWithRole.filter(y => y.roleId == x.roleId);
          }
          if(x.isEdit == true){
            x.stepServicePacketSelect = this.listStepServicePacketSelect.find(y => y.categoryId == x.categoryId);
          }
        });
        this.listServicePacketConfigurationPermissionModel = this.listServicePacketConfigurationPermissionModel.sort((a, b) => (a.stepId > b.stepId) ? 1 : -1);
      })
  }

  //#region File
  async uploadMainImage(event : {files : File[]}): Promise<void>{
    this.base64MainImage = await this.getBase64ImageFromURL(event);
    this.servicePacketImageEntityModel.mainImage = this.base64MainImage.toString();
  }

  async uploadBackgroundImage(event : {files : File[]}): Promise<void> {
    this.base64BackgroundImage = await this.getBase64ImageFromURL(event);
    this.servicePacketImageEntityModel.backgroundImage = this.base64BackgroundImage.toString();
  }

  async uploadIcon(event : {files : File[]}): Promise<void> {
    this.base64Icon = await this.getBase64ImageFromURL(event);
    this.servicePacketImageEntityModel.icon = this.base64Icon.toString();
  }

  removeMainImage(): void {
    this.base64MainImage = undefined;
    this.servicePacketImageEntityModel.mainImage = undefined;
  }

  removeBackgroundImage(): void {
    this.base64BackgroundImage = undefined;
    this.servicePacketImageEntityModel.backgroundImage = undefined;
  }

  removeIcon(): void {
    this.base64Icon = undefined;
    this.servicePacketImageEntityModel.icon = undefined;
  }

  getBase64ImageFromURL(event: {files : File[]}): Promise<string | ArrayBuffer | null> {
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

  changeProductCategory(event: ProductCategoryEntityModel): void {
    this.servicePacketEntityModel.productCategoryId = event.productCategoryId;
  }

  changeProvince(event: ProvinceEntityModel): void {
    this.servicePacketEntityModel.provinceId = event.provinceId;
  }

  changeAttrName(event : CategoryEntityModel): void {
    this.servicePacketAttributeEntityModelAdd.categoryId = event.categoryId;
    this.servicePacketAttributeEntityModelAdd.categoryName = event.categoryName;
  }

  changeAttrDataType(event: AttrConfigure): void {
    this.servicePacketAttributeEntityModelAdd.dataType = event.value;
    this.servicePacketAttributeEntityModelAdd.dataTypeName = event.name;
  }

  selectRole(event : PermissionConfigurationEntityModel, stepObject : PermissionConfigurationEntityModel){
    stepObject.categoryId = event.categoryId;
    stepObject.categoryName = event.categoryName;
  }

  deleteRole(event: PermissionConfigurationEntityModel): void {
    this.listServicePacketConfigurationPermissionModel = this.listServicePacketConfigurationPermissionModel.filter(x => x.stepId != event.stepId);
    this.setStepIdAgain();
  }

  changeRole(event: RoleEntityModel, step: PermissionConfigurationEntityModel): void {
    let index = this.listServicePacketConfigurationPermissionModel.indexOf(this.listServicePacketConfigurationPermissionModel.find(x => x.stepId == step.stepId));
    this.listServicePacketConfigurationPermissionModel[index].roleId = event.roleId;
    this.listServicePacketConfigurationPermissionModel[index].stepId = step.stepId;
    this.listServicePacketConfigurationPermissionModel[index].categoryId = step.categoryId;
    this.getListEmployeeByRoleId(event.roleId, step);
  }

  getListEmployeeByRoleId(roleId : string, step: PermissionConfigurationEntityModel): void {
    this.loading = true;
    this._productService.getListEmployeeByRoleId(roleId)
      .pipe(tap(() => {this.loading = false}))
      .subscribe(result => {
        step.listEmployeeEntityModelByRoleId = result.listEmployeeEntityModel
        step.listEmployeeEntityModel = []
      })
  }

  changeStep(step : number, stepObject: PermissionConfigurationEntityModel): void {
    let count = 0;
    this.oldStep = stepObject.stepId;
    if(step){
      this.listServicePacketConfigurationPermissionModel.splice(step - 1, 0 , this.listServicePacketConfigurationPermissionModel.splice(this.oldStep - 1, 1)[0]);
      this.listServicePacketConfigurationPermissionModel.forEach(x => {
        count += 1;
        x.stepId = count;
      });
      
      this.listServicePacketConfigurationPermissionModel = this.listServicePacketConfigurationPermissionModel.sort((a, b) => (a.stepId > b.stepId) ? 1 : -1);
      this.oldStep = step;
    };
  }

  setStepIdAgain(): void {
    let count = 0;
    this.listServicePacketConfigurationPermissionModel.forEach(x => {
      count += 1;
      x.stepId = count;
    });
  }

  addStepServicePacket(): void {
    this.lastStep = this.listServicePacketConfigurationPermissionModel[this.listServicePacketConfigurationPermissionModel.length - 1] != undefined ? this.listServicePacketConfigurationPermissionModel[this.listServicePacketConfigurationPermissionModel.length - 1].stepId : 0;
    this.newStepServicePacket = new PermissionConfigurationEntityModel({
      id: '00000000-0000-0000-0000-000000000000',
      stepId: this.lastStep +=1,
      roleId: '',
      servicePacketId: null,
      employeeId: "00000000-0000-0000-0000-000000000000",
      listEmployeeEntityModel: [],
      listEmployeeEntityModelByRoleId: [],
      createdById: "00000000-0000-0000-0000-000000000000",
      createdDate: "0001-01-01T00:00:00",
      updatedById: "00000000-0000-0000-0000-000000000000",
      updatedDate: "0001-01-01T00:00:00",
      tenantId: '00000000-0000-0000-0000-000000000000',
      categoryId : '',
      categoryName : '',
      categoryCode : '',
      isEdit : true,
      objectRole : null,
      stepServicePacketSelect : null
    });
    this.listServicePacketConfigurationPermissionModel.push(this.newStepServicePacket);
  }
  
  selectEmp(event: EmployeeEntityModel[], step: PermissionConfigurationEntityModel): void {
    let index = this.listServicePacketConfigurationPermissionModel.indexOf(this.listServicePacketConfigurationPermissionModel.find(x => x.stepId == step.stepId));
    this.listServicePacketConfigurationPermissionModel[index].listEmployeeEntityModel = event;
    this.listServicePacketConfigurationPermissionModel[index].stepId = step.stepId;
  }

  addAttr(): void {
    if (this.servicePacketAttributeEntityModelAdd.categoryId == null || this.servicePacketAttributeEntityModelAdd.dataType == null) {
      this.showToast("error", "Thông báo", "Vui lòng nhập đủ thông tin!");
      return;
    }
    var checkExitsAttr = this.listServicePacketAttributeEntityModel.find(x => x.categoryId == this.servicePacketAttributeEntityModelAdd.categoryId);
    if (checkExitsAttr) {
      this.showToast("error", "Thông báo", "Thuộc tính này đã được thêm!");
      return;
    }

    let newAttr = new ServicePacketAttributeEntityModel();
    newAttr.dataType = this.servicePacketAttributeEntityModelAdd.dataType;
    newAttr.dataTypeName = this.servicePacketAttributeEntityModelAdd.dataTypeName;
    newAttr.categoryId = this.servicePacketAttributeEntityModelAdd.categoryId;
    newAttr.categoryName = this.servicePacketAttributeEntityModelAdd.categoryName;
    this.listServicePacketAttributeEntityModel.push(newAttr);
  }

  deleteAttr(rowData: ServicePacketAttributeEntityModel): void {
    this.listServicePacketAttributeEntityModel = this.listServicePacketAttributeEntityModel.filter(x => x != rowData);
  }

  checkValidateServicePacket(): boolean {
    if(this.productCategoryEntityModel == undefined){
      return this.showErrorProductCategoryEntityModel = true;
    } else {
      return this.showErrorProductCategoryEntityModel = false;
    }
  }

  checkValidateProvince(): boolean {
    if(this.provinceEntityModel == undefined){
      return this.showErrorProvinceEntityModel = true;
    } else {
      return this.showErrorProvinceEntityModel = false;
    }
  }

  async getServicePacketOptionByServicePacketId(servicePacketId : string){
    this.loading = true;
    let result: any = await this._productService.searchOptionOfPacketService(servicePacketId);
    this.loading = false;
    if (result.statusCode == 200) {
      this.listOption = result.listOption;
      this.colOptions = [{ field: 'nameCustom', header: 'Tên dịch vụ' }];

      this.listOptionTreeNode = [];
      let listDataRoot = this.listOption.filter((x) => x.parentId == null);
      let level = 0;

      await listDataRoot.forEach((item) => {
        let number = "1";
        let listChild = this.mapDataTreeNode(item, level, item.name);
        let nodeRoot: NewTreeNode = {
          id : item.id,
          data: item,
          path: item.name,
          key: item.id,
          number: number,
          leaf: listChild.length == 0,
          children: listChild,
          margin: (40 + 16 * level).toString() + "px",
          optionId : item.optionId,
          sortOrder : item.sortOrder,
          price : item.price
        };
        nodeRoot.expanded = true;
        this.listOptionTreeNode.push(nodeRoot);
      });
      this.listOptionTreeNode = [...this.listOptionTreeNode.sort((a, b) => (a.sortOrder > b.sortOrder) ? 1 : -1)];
      console.log(this.listOptionTreeNode)
    }
    else if (result.statusCode !== 200) {
      let msg = { key: 'popup', severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
      this.showMessage(msg);
    }
  }

  mapDataTreeNode(option: ServicePacketMappingOptionsEntityModel, level: number, path: string): Array<NewTreeNode> {
    level++;
    let result: Array<NewTreeNode> = [];
    let listChildren = this.listOption.filter((x) => x.parentId == option.id);
    listChildren.forEach((item) => {
      let newPath = path + " ---> " + item.name;
      let dataRoot = item;
      let listChildrenNode: Array<NewTreeNode> = [];
      if (this.listOption.find(x => x.parentId == item.id)) {
        listChildrenNode = this.mapDataTreeNode(item, level, newPath);
      }
      let number = "1";
      let node: NewTreeNode = {
        id : item.id,
        data: dataRoot,
        key: item.id,
        number: number,
        path: newPath,
        leaf: listChildrenNode.length == 0,
        children: listChildrenNode,
        margin: (40 + 16 * level).toString() + "px",
        optionId : item.optionId,
        sortOrder : item.sortOrder,
        price : item.price
      };
      node.expanded = true;
      result.push(node);
    });
    return result.sort((a, b) => (a.sortOrder > b.sortOrder) ? 1 : -1);
  }

  editOption(event, type: string): void {
    this.showModalOption = true;
    this.showIsLastOption = true;
    if(event.node.data.optionId){
      this.optionModelService = this.listOptionEntityModel.find(x => x.id == event.node.data.optionId);
      this.servicePacketMappingOptionsEntityModel.optionId = event.node.data.optionId;
      this.isLastOption = true;
    } else {
      this.isLastOption = false;
      this.optionModelService = undefined;
    }
    if(type == 'edit'){
      if(event.parent == null && event.node.children.length > 0){
        this.showIsLastOption = false;
        this.isLastOption = false;
      }
      this.editServicePMOption = true;
      this.serviceDescription = event.node.data.description;
      this.servicePrice = event.node.data.price;
      this.servicePacketMappingOptionsEntityModel.name = event.node.data.name;
      this.servicePacketMappingOptionsEntityModel.id = event.node.data.id;
      this.servicePacketMappingOptionsEntityModel.parentId = event.node.data.parentId;
      this.servicePacketMappingOptionsEntityModel.sortOrder = event.node.data.sortOrder;
    }
  }

  addOption(event, type: string): void {
    if(type == 'add'){
      this.showIsLastOption = true;
      this.editServicePMOption = false;
      if(event && event.price){
        return this.showToast('error', 'Thông báo', 'Không thể thêm tùy chọn con');
      }
      this.optionModelService = undefined;
      this.servicePacketMappingOptionsEntityModel = new ServicePacketMappingOptionsEntityModel();
      this.showModalOption = true;
      if(event){
        this.servicePacketMappingOptionsEntityModel.parentId = event.id;
        if(this.listOptionTreeNode.find(x => x.key == event.id) && this.listOptionTreeNode.find(x => x.key == event.id).children.length > 0){
          let listChildByParentId: any  = this.listOptionTreeNode.find(x => x.key == event.id).children;
          this.lastSortOrder = listChildByParentId[listChildByParentId.length - 1] != undefined ? listChildByParentId[listChildByParentId.length - 1].sortOrder : 0;
          this.lastSortOrder +=1;
        } else {
          this.lastSortOrder = 1;
        }
      } else {
        this.lastSortOrder = this.listOptionTreeNode[this.listOptionTreeNode.length - 1] != undefined ? this.listOptionTreeNode[this.listOptionTreeNode.length - 1].sortOrder : 0;
        this.lastSortOrder +=1;
        this.isLastOption = false;
      }
    }
    this.servicePacketMappingOptionsEntityModel.sortOrder = this.lastSortOrder;
  }

  checkValidateOption(): boolean {
    if(this.optionModelService == undefined){
      return this.showErrorOptionModelService = true;
    } else {
      return this.showErrorOptionModelService = false;
    }
  }

  createServicePacketMappingOption(): void {
    if(!this.checkValidateOption() || this.servicePacketMappingOptionsEntityModel.name){
      this.loading = true;
      this.servicePacketMappingOptionsEntityModel.servicePacketId = this.servicePacketId;
      if(!this.isLastOption){
        this.servicePacketMappingOptionsEntityModel.optionId = null;
      }
      this._productService.createServicePacketMappingOption(this.servicePacketMappingOptionsEntityModel)
        .pipe(tap(() => {
          this.loading = false; 
        }))
        .subscribe(result => {
          if (result.statusCode == 200) {
            this.showToast('success', 'Thông báo', 'Lưu thành công');
            this.getServicePacketOptionByServicePacketId(this.servicePacketId);
            this.closeModalOption();
          } else {
            this.showToast('error', 'Thông báo', result.message);
          }
        })
    }
  }

  changeOption(option: OptionsEntityModel): void {
    console.log(option);
    this.optionModelService = option;
    this.servicePacketMappingOptionsEntityModel.optionId = option.id;
    this.servicePacketMappingOptionsEntityModel.name = option.name;
    this.servicePrice = option.price;
    this.serviceDescription = option.description;
  }

  deleteOption(optionId : string): void {
    this.confirmationService.confirm({
      message: 'Bạn có chắc chắn muốn xóa?',
      accept: () => {
        this.loading = true;
        this._productService.deleteServicePacketMappingOption(optionId)
        .pipe(tap(() => {
          this.loading = false; 
        }))
        .subscribe(response => {
          if (response.statusCode === 202 || response.statusCode === 200) {
            let mgs = { severity: 'success', summary: 'Thông báo', detail: 'Xóa thành công' };
            this.getMasterDataServicePacket();
            this.showMessage(mgs);
          } else {
            let mgs = { severity: 'error', summary: 'Thông báo', detail: response.message };
            this.showMessage(mgs);
          }
        });
      }
    });
  }

  customerNotificationRecipient(notificationConfigurationEntityModel: NotificationConfigurationEntityModel): NotificationConfigurationEntityModel {
    notificationConfigurationEntityModel.notificationRecipient = '';
    if(notificationConfigurationEntityModel.serviceManagement){
      notificationConfigurationEntityModel.notificationRecipient += 'Quản lý dịch vụ, ';
    }
    if(notificationConfigurationEntityModel.serviceSupporter){
      notificationConfigurationEntityModel.notificationRecipient += 'Người tạo phiếu hỗ trợ dịch vụ, ';
    }
    if(notificationConfigurationEntityModel.serviceRequestMaker){
      notificationConfigurationEntityModel.notificationRecipient += 'Người tạo phiếu yêu cầu, ';
    }
    if(notificationConfigurationEntityModel.reporter){
      notificationConfigurationEntityModel.notificationRecipient += 'Nhân viên báo cáo, ';
    }
    if(notificationConfigurationEntityModel.supporter){
      notificationConfigurationEntityModel.notificationRecipient += 'Nhân viên hỗ trợ, ';
    }
    if(notificationConfigurationEntityModel.listEmployeeEntityModel.length > 0){
      notificationConfigurationEntityModel.listEmployeeEntityModel.forEach(x => {
        notificationConfigurationEntityModel.notificationRecipient += this.listEmpWithRole.find(y => y.employeeId == x.employeeId).employeeName + ', ';
      })
    }

    return notificationConfigurationEntityModel;
  }

  showModalNotiConfig(event: NotificationConfigurationEntityModel): void {
    this.notificationConfigurationEntityModel = new NotificationConfigurationEntityModel();
    this.notificationConfigurationEntityModel = event;
    this.notificationConfigurationEntityModel.notificationRecipient = '';
    let listEmployeeId = event.listEmployeeEntityModel.map(i => i.employeeId);
    this.notificationConfigurationEntityModel.listEmployeeEntityModel = this.listEmpWithRole.filter(x => listEmployeeId.indexOf(x.employeeId) != -1);
    this.notificationConfigurationEntityModel = this.customerNotificationRecipient(this.notificationConfigurationEntityModel)
    this.showModalNoitiConfig = true;
  }

  saveNotiConfig(): void {
    let index = this.listNotificationConfigurationModel.indexOf(this.listNotificationConfigurationModel.find(x => x.categoryId == this.notificationConfigurationEntityModel.categoryId));
    this.listNotificationConfigurationModel[index] = this.customerNotificationRecipient(this.notificationConfigurationEntityModel);
    this.showModalNoitiConfig = false;
  }

  save(): void {
    if (!this.formTenCauHinh.valid) {
      if (!this.tenCauHinhControl.valid)
        this.tenCauHinhControl.markAsTouched();
      return;
    }
    
    if (this.quyTrinh.length == 0) {
      this.showToast('warn', 'Thông báo','Bạn cần thêm quy trình');
      return;
    }

    if (!this.quyTrinh.valid) {
      this.quyTrinh.controls.forEach(abstractControl => {
        if (!abstractControl.valid) {
          abstractControl.get('loaiPheDuyet').markAsTouched();
        }
      });
      return;
    }
    
    //Kiểm tra nếu Loại phê duyệt là Phòng ban phê duyệt hoặc Phòng ban xác nhận => Đã chọn phòng ban chưa?
    let error = false;
    this.quyTrinh.controls.forEach(abstractControl => {
      if (abstractControl.get('loaiPheDuyet').value.value == 2) {
        let listPhongBanId = abstractControl.get('phongBanId').value;
        if (listPhongBanId.length == 0) {
          error = true;
        }
      }
    });

    if (error) {
      this.showToast('warn', 'Thông báo', 'Bạn chưa chọn phòng ban phê duyệt');
      return;
    }
    
    let cauHinhQuyTrinh = this.mapDataToModel(this.cauHinhQuyTrinh);


    if(!this.checkValidateServicePacket() && !this.checkValidateProvince()){
      if(this.listServicePacketConfigurationPermissionModel.some(x => x.listEmployeeEntityModel == undefined || x.listEmployeeEntityModel == null) ||
        this.listServicePacketConfigurationPermissionModel.some(x => x.roleId == undefined || x.roleId == null)
        )
      {
        this.showToast('error', 'Thông báo', 'Vui lòng điền đầy đủ cấu hình phân quyền') 
      } 
      else 
      {
        if(!this.listManager || this.listManager.length == 0){
          this.showToast('error', 'Thông báo', 'Vui lòng chọn quản lý dịch vụ cho gói') 
          return;
        }
        let listManagerId = this.listManager.map(x => x.employeeId);
        
        this.loading = true;
        this.setStepIdAgain();
        let createOrEditProductParameter = new CreateOrUpdateServicePacketParameter();
        createOrEditProductParameter.servicePacketEntityModel = this.servicePacketEntityModel;
        createOrEditProductParameter.listServicePacketAttributeEntityModel = this.listServicePacketAttributeEntityModel;
        createOrEditProductParameter.listServicePacketConfigurationPermissionModel = this.listServicePacketConfigurationPermissionModel;
        createOrEditProductParameter.servicePacketImageEntityModel = this.servicePacketImageEntityModel;
        createOrEditProductParameter.listNotificationConfigurationModel = this.listNotificationConfigurationModel;
        createOrEditProductParameter.cauHinhQuyTrinh = cauHinhQuyTrinh;
        createOrEditProductParameter.listManagerId = listManagerId;

        this._productService.createOrUpdateServicePacket(createOrEditProductParameter)
        .pipe(tap(() => {
          this.loading = false; 
        }))
        .subscribe(result => {
          if (result.statusCode == 200) {
          this.showToast('success', 'Thông báo', 'Lưu thành công');

          //Thêm cấu hình phân quyền

          this._router.navigate(['/product/list-product-packet']);
          this.getMasterDataServicePacket();
          } else {
            this.showToast('error', 'Thông báo', 'Lưu thất bại');
          }
        })
      }
    }
  }

  //Quy trình phê duyệt
  themBuoc() {
    this.quyTrinh.push(this.addForm());
  }

  xoaBuoc(index: number) {
    this.quyTrinh.removeAt(index);
  }

  addForm() {
    return this.fb.group({
      loaiPheDuyet: [null, Validators.required],
      phongBan: [{value: [], disabled: true}],
      phongBanId: [[]]
    });
  }

  /* Thay đổi loại phê duyệt */
  changeLoaiPheDuyet(index: number) {
    this.quyTrinh.controls[index].get('phongBanId').setValue([]);
    this.quyTrinh.controls[index].get('phongBan').setValue([]);
  }


  /* Chọn đơn vị */
  openPopup(index: number) {
    let mode = 2;
    let loaiPheDuyet = this.quyTrinh.controls[index].get('loaiPheDuyet').value.value;
    if (loaiPheDuyet == 2) {
      mode = 1;
    }
    
    let listSelectedId = this.quyTrinh.controls[index].get('phongBanId').value;
    let selectedId = listSelectedId.length == 1 ? listSelectedId[0] : null;
    
    let ref = this.dialogService.open(ChonNhieuDvDialogComponent, {
      data: {
        mode: mode, //1: Multiple choice, 2: Single choice
        listSelectedId: listSelectedId,
        selectedId: selectedId
      },
      header: 'Chọn đơn vị',
      width: '40%',
      baseZIndex: 10001,
      contentStyle: {
        "min-height": "350px",
        "max-height": "500px",
        "overflow": "auto"
      }
    });

    ref.onClose.subscribe((result: any) => {
      if (result) {
        if (result?.length > 0) {
          this.quyTrinh.controls[index].get('phongBanId').setValue(result.map(x => x.organizationId));
          this.quyTrinh.controls[index].get('phongBan').setValue(result.map(x => x.organizationName))
        }
        else {
          this.quyTrinh.controls[index].get('phongBanId').setValue([]);
          this.quyTrinh.controls[index].get('phongBan').setValue([]);
        }
      }
    });
  }


  mapDataToModel(cauHinhQuyTrinh: CauHinhQuyTrinh) {
    cauHinhQuyTrinh.listCacBuocQuyTrinh = [];

    let listQuyTrinh: Array<string> = [];
    this.quyTrinh.controls.forEach((abstractControl, index) => {
      //Phê duyệt trưởng bộ phận
      if (abstractControl.get('loaiPheDuyet').value.value == 1) {
        listQuyTrinh.push('Trưởng bộ phận');
      }
      //Phòng ban phê duyệt hoặc Phòng ban xác nhận
      else {
        let listPhongBan: Array<string> = abstractControl.get('phongBan').value;
        listQuyTrinh.push(listPhongBan.join(", "));
      }

      let buoc = new CacBuocQuyTrinh();
      buoc.stt = index + 1;
      buoc.loaiPheDuyet = abstractControl.get('loaiPheDuyet').value.value;
      
      let listCacPhongBanTrongBuoc: Array<string> = abstractControl.get('phongBanId').value;
      listCacPhongBanTrongBuoc.forEach(phongBanId => {
        let phongBan = new PhongBanTrongCacBuocQuyTrinh();
        phongBan.organizationId = phongBanId;

        buoc.listPhongBanTrongCacBuocQuyTrinh.push(phongBan);
      });

      cauHinhQuyTrinh.listCacBuocQuyTrinh.push(buoc);
    });

    cauHinhQuyTrinh.quyTrinh = listQuyTrinh.join(" --> ");

    return cauHinhQuyTrinh;
  }

  mapDataToForm(cauHinhQuyTrinh: CauHinhQuyTrinh) {
    if(cauHinhQuyTrinh != null){
      this.tenCauHinhControl.setValue("");

      //Show lại các bước
      cauHinhQuyTrinh.listCacBuocQuyTrinh.forEach(buoc => {
        let newForm = this.addForm();
  
        let loaiPheDuyet = this.listLoaiPheDuyet.find(x => x.value == buoc.loaiPheDuyet);
        newForm.get('loaiPheDuyet').setValue(loaiPheDuyet);
  
        //Phòng ban phê duyệt hoặc Phòng ban xác nhận
        if (loaiPheDuyet.value != 1) {
          let listPhongBan: Array<string> = []; //chip
          let listPhongBanId: Array<string> = []; //Id
          buoc.listPhongBanTrongCacBuocQuyTrinh.forEach(phongBan => {
            let _phongBan = this.listOrganization.find(x => x.organizationId == phongBan.organizationId);
            listPhongBan.push(_phongBan.organizationName);
            listPhongBanId.push(_phongBan.organizationId);
          });
  
          newForm.get('phongBan').setValue(listPhongBan);
          newForm.get('phongBanId').setValue(listPhongBanId);
        }
  
        this.quyTrinh.push(newForm);
      });
    }
   
  }

}
