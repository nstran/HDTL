import { Component, OnInit, ChangeDetectorRef, ViewChild } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { ProjectService } from '../../services/project.service';
import { MessageService, ConfirmationService } from 'primeng/api';
import { ProjectModel } from '../../models/project.model';
import { DatePipe } from '@angular/common';
import { DialogService } from 'primeng/dynamicdialog';
import { ProjectResourceModel } from '../../models/ProjectResource.model';
import { AddProjectResourceDialogComponent } from '../add-project-resource-dialog/add-project-resource-dialog.component';
import { ContactModel, contactModel } from '../../../../../src/app/shared/models/contact.model';
import { ProjectTaskModel } from '../../models/ProjectTask.model';
import { VendorModel } from '../../../../app/vendor/models/vendor.model';
import { ProjectVendorModel } from '../../models/ProjectVendor.model';
import { NoteService } from '../../../shared/services/note.service';
import { NoteDocumentModel } from '../../../shared/models/note-document.model';
import { NoteModel } from '../../../shared/models/note.model';
import * as $ from 'jquery';
import { Paginator } from 'primeng/paginator';
import { GetPermission } from '../../../shared/permission/get-permission';
import { timeStamp } from 'console';

class Project {
  projectId: string;
  projectCode: string;
  projectName: string;
  projectCodeName: string;
}

interface Employee {
  EmployeeId: string;
  EmployeeName: string;
}

interface Category {
  categoryId: string;
  categoryCode: string;
  categoryName: string;
  isDefault: boolean;
}


class NoteDocument {
  active: boolean;
  base64Url: string;
  createdById: string;
  createdDate: Date;
  documentName: string;
  documentSize: string;
  documentUrl: string;
  noteDocumentId: string;
  noteId: string;
  updatedById: string;
  updatedDate: Date;
}
class Note {
  active: boolean;
  createdById: string;
  createdDate: Date;
  description: string;
  noteDocList: Array<NoteDocument>;
  noteId: string;
  noteTitle: string;
  objectId: string;
  objectType: string;
  responsibleAvatar: string;
  responsibleName: string;
  type: string;
  updatedById: string;
  updatedDate: Date;
  constructor() {
    this.noteDocList = [];
  }
}


class provinceModel {
  provinceId: string;
  provinceName: string;
  provinceType: string;
  provinceCode: string;
}

class districtModel {
  districtId: string;
  districtName: string;
  districtType: string;
  districtCode: string;
  provinceId: string;
}

class wardModel {
  wardId: string;
  wardCode: string;
  wardType: string;
  wardName: string;
  districtId: string;
}
@Component({
  selector: 'app-project-resource',
  templateUrl: './project-resource.component.html',
  styleUrls: ['./project-resource.component.css'],
  providers: [
    DatePipe
  ]
})

export class ProjectResourceComponent implements OnInit {

  constructor(
    private projectService: ProjectService,
    private router: Router,
    private noteService: NoteService,
    private getPermission: GetPermission,
    private dialogService: DialogService,
    private confirmationService: ConfirmationService,
    private messageService: MessageService,
    private changeDetectorRef: ChangeDetectorRef,
    private route: ActivatedRoute) {
  }

  listProjectTitle: Array<Project> = [];
  selectedProject: Project = null;

  loading: boolean = false;
  projectId: string = 'DCB9271B-E84D-4C56-8145-63C16653505B';
  selectedColumns: any[];
  selectedColumnsExternal: any[];
  cols: any[];
  isInternal: boolean = true;
  selectedItem: any;
  selectedItemExternal: any;
  listProjectResources: Array<ProjectResourceModel> = [];
  listProjectResourcesInternal: Array<ProjectResourceModel> = [];
  listProjectResourcesExternal: Array<ProjectResourceModel> = [];

  listProjectResourcesInternalShow: Array<ProjectResourceModel> = [];
  listProjectResourcesExternalShow: Array<ProjectResourceModel> = [];


  listNguonLucThueNgoai: Array<Employee> = [];
  listNguonLucNoiBo: Array<Employee> = [];

  project: ProjectModel = new ProjectModel();
  resourceProjectForm: FormGroup;
  inforVendorForm: FormGroup;
  selectProjectResourceId: string; // Chon nha thau tren grid
  projectVendorId: string;
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  isShow: boolean = true;


  SelectNVNB = [];//list nhân viên nội bộ
  SelectNVTN = [];//list nhân viên thuê ngoài
  HienTaiNB = false;
  HienTaiTN = false;

  vendorNameControl: FormControl; // Tên nhà thầu
  contactControl: FormControl; // Người liên hệ
  paymentMethodControl: FormControl;  //Phương thức thanh toán
  listPaymentMethod: Array<Category> = []; //list Phương thức thanh toán
  listContact: Array<ContactModel> = []; // Danh sách người liên hệ
  listContactById: Array<contactModel> = []; // Danh sách người liên hệ
  vendorModel: VendorModel = new VendorModel();
  contactPerson: contactModel = new contactModel();
  paymentId: string;
  isContractor: boolean = false;
  totalPercent: string = "0"; // Tiến độ dự án
  totalEstimateTime: number = 0;
  listProjectTasks: Array<ProjectTaskModel> = [];

  listProvince: Array<provinceModel> = [];
  listDistrict: Array<districtModel> = [];
  listCurrentDistrict: Array<districtModel> = [];
  listWard: Array<wardModel> = [];
  listCurrentWard: Array<wardModel> = [];
  fullAddress: string = "";
  auth: any = JSON.parse(localStorage.getItem("auth"));
  userPermission: any = localStorage.getItem("UserPermission").split(',');
  selectVendor: any;
  selectProjectResource: any;
  isEdit: boolean = false;

  /*NOTE*/
  noteContent: string = '';
  noteId: string;
  listUpdateNoteDocument: Array<NoteDocument> = [];
  isEditNote: boolean = false;
  defaultAvatar: string = '/assets/images/no-avatar.png';
  noteHistory: Array<Note> = [];
  /*END*/
  strAcceptFile: string = 'image video audio .zip .rar .pdf .xls .xlsx .doc .docx .ppt .pptx .txt';
  uploadedFiles: any[] = [];
  listNoteDocumentModel: Array<NoteDocumentModel> = [];

  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  defaultNumberType = this.systemParameterList.find((systemParameter: { systemKey: string; }) => systemParameter.systemKey == "DefaultNumberType").systemValueString;
  defaultLimitedFileSize = Number(this.systemParameterList.find(systemParameter => systemParameter.systemKey == "LimitedFileSize").systemValueString) * 1024 * 1024;
  isSendEmailControl: FormControl;
  totalWorkdays: number = 0;
  //actionDelete: boolean = true;
  actionEditNote: boolean = true;
  // actionEdit: boolean = true;

  totalRecordsNote: number = 0;
  @ViewChild('paginator', { static: true }) paginator: Paginator;

  displayPhanBo: boolean = false;
  employeeCodeName: string = null;
  listPhanBoNguonLuc: Array<any> = [];
  tongPhanTramRanhRoi: number = 0;
  listPermissionResourceActive: string = localStorage.getItem("ListPermissionResource");

  async ngOnInit() {
    this.loading = true;
    this.initForm();
    this.setTable();
    this.route.params.subscribe(async params => {
      this.projectId = params['projectId'];
      let resource = "pro/project/resource/";
      let permission: any = await this.getPermission.getPermissionLocal(this.listPermissionResourceActive, resource);
      if (permission.status == false) {
        this.router.navigate(['/home']);
      }
      else {
        let listCurrentActionResource = permission.listCurrentActionResource;
        // if (listCurrentActionResource.indexOf("edit") == -1) {
        //   this.actionEdit = false;
        // }
        // if (listCurrentActionResource.indexOf("delete") == -1) {
        //   this.actionDelete = false;
        // }
        if (listCurrentActionResource.indexOf("editnote") == -1) {
          this.actionEditNote = false;
        }
        this.getMasterData();
      }
    });
  }

  initForm() {
    this.paymentMethodControl = new FormControl(null);
    this.contactControl = new FormControl(null);
    this.vendorNameControl = new FormControl(null);
    this.isSendEmailControl = new FormControl(false);

    this.resourceProjectForm = new FormGroup({
      isSendEmailControl: this.isSendEmailControl
    });
    this.inforVendorForm = new FormGroup({
      paymentMethodControl: this.paymentMethodControl,
      contactControl: this.contactControl,
      vendorNameControl: this.vendorNameControl,

    });
  }

  async getMasterData() {
    this.loading = true;
    let result: any = await this.projectService.getProjectResource(this.projectId);
    this.loading = false;
    if (result.statusCode == 200) {
      this.project = result.project;

      this.listProjectTitle = result.listProject;
      this.listProjectTitle.forEach(item => {
        item.projectCodeName = `[${item.projectCode}]: ${item.projectName}`;
      });
      this.selectedProject = this.listProjectTitle.find(c => c.projectId == this.projectId);

      this.listProjectResources = result.listProjectResource || [];
      this.listProjectResourcesInternal = this.listProjectResources.filter(x => x.resourceRoleName == "NB");
      this.listProjectResourcesExternal = this.listProjectResources.filter(x => x.resourceRoleName == "TN");

      //Danh sách nội bộ và thuê ngoài để hiển thị
      this.listProjectResourcesInternalShow = this.listProjectResourcesInternal;
      this.listProjectResourcesExternalShow = this.listProjectResourcesExternal;

      let nguonLucNoiBo = [];
      let nguonLucThueNgoai = [];

      this.listProjectResourcesInternal.forEach(x => {
        nguonLucNoiBo.push({ EmployeeId: x.objectId, EmployeeName: x.nameResource })
      });

      this.listProjectResourcesExternal.forEach(x => {
        nguonLucThueNgoai.push({ EmployeeId: x.objectId, EmployeeName: x.nameResource })
      });

      this.listNguonLucNoiBo = nguonLucNoiBo.filter((v, i, a) => a.indexOf(v) === i);
      this.listNguonLucThueNgoai = nguonLucThueNgoai.filter((v, i, a) => a.indexOf(v) === i);

      this.listPaymentMethod = result.listPaymentMethod; //list Phương thức thanh toán
      this.listProjectTasks = result.listProjectTask;

      this.totalRecordsNote = result.totalRecordsNote;
      this.noteHistory = result.listNote;
      this.handleNoteContent();
      this.isSendEmailControl.setValue(true);
      this.totalPercent = this.project.taskComplate.toFixed(2);
      this.totalEstimateTime = this.project.estimateCompleteTime;
      this.totalWorkdays = 0;
      this.listProjectResourcesInternal.forEach(a => this.totalWorkdays += a.workDay);
      this.totalWorkdays = Number(this.totalWorkdays.toFixed(2));
      this.loading = false;
      this.inforVendorForm.markAsUntouched();
    } else {
      let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
      this.showMessage(msg);
    }
  }

  cancel() {
    this.confirmationService.confirm({
      message: 'Bạn có muốn lưu các thay đổi?',
      accept: () => {
        // Chuyển về thông tin dự án
        this.router.navigate(['/project/detail', { 'projectId': this.projectId }]);
      }
    });
  }

  // Thay đổi người liên hệ
  changeContact(event: any) {
    if (event.value) {
      var contactId = event.value.contactId;
      this.contactPerson = this.listContactById.filter(x => x.contactId == contactId)[0];
    }
    else
      this.contactPerson = new contactModel();
    this.inforVendorForm.markAsTouched();
  }

  changePayment() {
    this.inforVendorForm.markAsTouched();
  }

  // Xem chi tiết Nhà thầu
  async goToDetailExternalResource(projectResourceId: any): Promise<void> {
    // Check thay doi data
    if (!this.inforVendorForm.touched || !this.isContractor) {
      this.isEdit = false;
      this.isContractor = true;
      this.loading = true;
      this.selectProjectResourceId = projectResourceId;
      await this.reloadDataProjectResource(projectResourceId);
    }
    else {
      this.confirmationService.confirm({
        header: "Xác nhận",
        message: 'Bạn có muốn lưu các thay đổi?',
        accept: () => {
          this.selectProjectResourceId = projectResourceId;
          this.saveVendor();
        },
        reject: () => {
          this.isEdit = false;
          this.inforVendorForm.markAsUntouched();
          this.goToDetailExternalResource(projectResourceId);
        }
      });
    }
  }

  async reloadDataProjectResource(projectResourceId: any, isDelete: boolean = false) {
    if (isDelete) {
      this.resetDataExternalResource();
    }
    else {
      try {
        this.disableControls();
        this.selectProjectResource = this.listProjectResourcesExternal.filter(x => x.projectResourceId == projectResourceId)[0];
        this.vendorNameControl.setValue(this.selectProjectResource.nameResource);
        // Thông tin chi tiết nhà thầu vs thông tin thanh toán - người liên hệ
        this.selectVendor = this.selectProjectResource.listProjectVendor.filter(x => x.projectResourceId == projectResourceId).length !== 0 ? this.selectProjectResource.listProjectVendor.filter(x => x.projectResourceId == projectResourceId) : [];

        // filter danh sách liên hệ của nhà thầu
        this.listContactById = this.listProjectResources.filter(x => x.resourceRoleName == "TN").find(x => x.objectId == this.selectProjectResource.objectId).listContact.filter(a => a.objectType == "VEN_CON");
        if (this.listContactById.length > 0) {
          // Không có trong bảng projectVendor => Lấy trường đầu tiên của danh sách người liên hệ
          if (this.selectProjectResource.listContact.filter(x => x.contactId == this.selectVendor[0].contactId).length == 0) {
            this.contactPerson = this.listContactById.filter(x => x.objectType == "VEN_CON") != null ? this.listContactById.filter(x => x.objectType == "VEN_CON")[0] : new contactModel();
            this.inforVendorForm.controls['contactControl'].setValue(this.contactPerson);
            this.inforVendorForm.controls['paymentMethodControl'].setValue(this.listPaymentMethod[0]);
          }
          else {
            let toSelectContact = this.listContactById.find(c => c.contactId === this.selectVendor[0].contactId && c.objectType == "VEN_CON");
            this.inforVendorForm.controls['contactControl'].setValue(toSelectContact);
            this.contactPerson = toSelectContact == undefined ? new contactModel() : toSelectContact;
          }
        }
        else {
          // Nhà thau khong co lien he
          this.contactPerson = new contactModel();
          this.inforVendorForm.controls['contactControl'].setValue(null);
        }

        let toSelectPaymentMethod = this.listPaymentMethod.find(c => c.categoryId === this.selectVendor[0].paymentMethodId);
        this.inforVendorForm.controls['paymentMethodControl'].setValue(toSelectPaymentMethod);

        var vendorModel = this.listProjectResources.filter(x => x.resourceRoleName == "TN").find(x => x.objectId == this.selectProjectResource.objectId).listContact.filter(a =>
          a.objectType == "VEN" &&
          a.objectId == this.selectProjectResource.objectId)[0];

        this.vendorModel = vendorModel == undefined || vendorModel == null ? new VendorModel() : vendorModel;
        if (vendorModel == undefined) {
          this.fullAddress = '';
        }
        else
          this.fullAddress = this.fullAddressBuilder(vendorModel.address, vendorModel.provinceId, vendorModel.districtId, vendorModel.wardId);

      } catch (error) {
        this.loading = false;
      }
      this.loading = false;
    }
  }
  editVendor() {
    this.isEdit = true;
    this.enableControls();
  }
  // disable control Hạng mục
  disableControls() {
    this.inforVendorForm.get('vendorNameControl').disable();
    this.inforVendorForm.get('contactControl').disable();
    this.inforVendorForm.get('paymentMethodControl').disable();
  }
  // enable control Hạng mục
  enableControls() {
    this.inforVendorForm.get('paymentMethodControl').enable();
    this.inforVendorForm.get('contactControl').enable();
  }

  fullAddressBuilder(address, provinceId, districtId, wardId): string {
    let arr: Array<string> = [];
    if (address) arr = [...arr, address];
    let _ward = this.listWard.find(e => e.wardId == wardId);
    if (_ward) arr = [...arr, `${_ward.wardType} ${_ward.wardName}`];
    let _district = this.listDistrict.find(e => e.districtId == districtId);
    if (_district) arr = [...arr, `${_district.districtType} ${_district.districtName}`];
    let _province = this.listProvince.find(e => e.provinceId == provinceId);
    if (_province) arr = [...arr, `${_province.provinceType} ${_province.provinceName}`];
    return arr.join(', ');
  }


  setTable() {

    // Bảng tab Nội bộ
    this.selectedColumns = [
      { field: 'stt', header: 'STT', width: '5%', textAlign: 'center', color: '#f44336' },
      { field: 'resourceTypeName', header: 'Loại nguồn lực', width: '10%', textAlign: 'left', color: '#f44336' },
      { field: 'nameResource', header: 'Tên nguồn lực', width: '25%', textAlign: 'left', color: '#f44336' },
      { field: 'employeeRoleName', header: 'Vai trò', width: '10%', textAlign: 'left', color: '#f44336' },
      { field: 'startTime', header: 'Bắt đầu', width: '10%', textAlign: 'right', color: '#f44336' },
      { field: 'endTime', header: 'Kết thúc', width: '10%', textAlign: 'right', color: '#f44336' },
      { field: 'allowcation', header: '% Phân bổ', width: '10%', textAlign: 'center', color: '#f44336' },
      { field: 'workDay', header: 'Số ngày phân bổ', width: '10%', textAlign: 'right', color: '#f44336' }
    ];

    // Tab bảng thuê ngoài
    this.selectedColumnsExternal = [
      { field: 'resourceTypeName', header: 'Nhóm nhà thầu', width: '20%', textAlign: 'left', color: '#f44336' },
      { field: 'startTime', header: 'Bắt đầu', width: '10%', textAlign: 'right', color: '#f44336' },
      { field: 'endTime', header: 'Kết thúc', width: '10%', textAlign: 'right', color: '#f44336' },
      { field: 'allowcation', header: '% Phân bổ', width: '10%', textAlign: 'center', color: '#f44336' },
      { field: 'workDay', header: 'Số ngày phân bổ', width: '10%', textAlign: 'right', color: '#f44336' },
    ];

    //Phân bổ nguồn lực các dự án
    this.cols = [
      { field: 'stt', header: 'STT', width: '10%', textAlign: 'center' },
      { field: 'projectName', header: 'Tên dự án', width: '60%', textAlign: 'left' },
      { field: 'phanTram', header: '% Phân bổ', width: '30%', textAlign: 'right' },
    ];
  }

  // Thêm mới nguồn lực
  async addProjectResource(isInternal: any) {

    let ref = this.dialogService.open(AddProjectResourceDialogComponent, {
      data: {
        projectId: this.projectId,
        isInternal: isInternal,
        projectStartDate: this.project.projectStartDate,
        projectEndDate: this.project.projectEndDate
      },
      header: isInternal ? 'Thêm nguồn lực nội bộ' : 'Thêm nguồn lực thuê ngoài',
      width: isInternal ? '70%' : '60%',
      baseZIndex: 999,
      contentStyle: {
        "min-height": "440px",
        "max-height": "1000px",
        "overflow-x": "hidden",
        "overflow": "true",
        "overflow-y": "auto"
      }
    });

    ref.onClose.subscribe(async (result: any) => {
      if (result) {
        if (result.status === true) {
          await this.getMasterData();

          // Lua chon luon nguon luc moi them
          this.goToDetailExternalResource(result.projectResourceId);
          let msg = { severity: 'success', summary: 'Thông báo:', detail: 'Thêm mới nguồn lực thành công' };
          this.showMessage(msg);
        } else {
          let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
          this.showMessage(msg);
        }
        this.loading = false;
      }
    });
  }

  // Cập nhập thông tin nhà thầu
  async saveVendor(): Promise<void> {

    if (this.inforVendorForm.touched) {
      let vendorModel = new ProjectVendorModel();
      vendorModel.paymentMethodId = this.inforVendorForm.get('paymentMethodControl').value.categoryId;
      vendorModel.contactId = this.inforVendorForm.get('contactControl').value !== undefined &&
        this.inforVendorForm.get('contactControl').value !== null ? this.inforVendorForm.get('contactControl').value.contactId : this.emptyGuid;
      vendorModel.projectId = this.projectId;
      vendorModel.vendorId = this.selectVendor[0].vendorId;
      if (this.selectVendor.length == 0) {
        vendorModel.projectVendorId = this.emptyGuid;
      }
      else {
        vendorModel.projectVendorId = this.selectVendor[0].projectVendorId;
      }

      let result: any = await this.projectService.updateProjectVendorResource(vendorModel, this.auth.UserId);

      if (result.statusCode === 202 || result.statusCode === 200) {
        await this.getMasterData();
        this.inforVendorForm.markAsUntouched();
        this.goToDetailExternalResource(this.selectProjectResourceId);
        this.isEdit = false;
        this.clearToast();
        this.showToast('success', 'Thông báo', 'Cập nhật thông tin nhà thầu thành công');
        this.loading = false;
      }

    }

  }

  /* Sửa nguồn lực */
  showEditProjectResourceDialog(rowData: any, isInternal: any) {

    let ref = this.dialogService.open(AddProjectResourceDialogComponent, {
      data: {
        projectId: this.projectId,
        isInternal: isInternal,
        projectResourceId: rowData.projectResourceId,
        projectStartDate: this.project.projectStartDate,
        projectEndDate: this.project.projectEndDate
      },
      header: 'Thông tin nguồn lực',
      width: '60%',
      baseZIndex: 999,
      contentStyle: {
        "min-height": "350px",
        "max-height": "1000px",
        "overflow-x": "hidden",
        "overflow": "true",
        "overflow-y": "auto"
      }
    });

    ref.onClose.subscribe(async (result: any) => {
      if (result) {
        if (result.status === true) {
          let msg = { severity: 'success', summary: 'Thông báo:', detail: 'Cập nhập nguồn lực thành công' };
          this.showMessage(msg);
          await this.getMasterData();
          this.inforVendorForm.markAsUntouched();
          this.goToDetailExternalResource(this.selectProjectResourceId);
        }
        else {
          let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
          this.showMessage(msg);
        }
        this.loading = false;
      }
    });
  }
  async editProjectResource(rowData: any, isInternal: any) {

    if (this.inforVendorForm.touched) {
      this.confirmationService.confirm({
        header: "Xác nhận",
        message: 'Bạn có muốn lưu các thay đổi?',
        accept: async () => {
          await this.saveVendor();
          this.showEditProjectResourceDialog(rowData, isInternal);
        },
        reject: () => {
          this.isEdit = false;
          this.inforVendorForm.markAsUntouched();
          this.goToDetailExternalResource(rowData.projectResourceId);
          this.showEditProjectResourceDialog(rowData, isInternal);
        }
      });
    }
    else
      this.showEditProjectResourceDialog(rowData, isInternal);

  }

  /* Xóa nguồn lực */
  delete_ProjectResource(rowData: any) {
    this.confirmationService.confirm({
      header: "Xác nhận",
      message: 'Bạn có chắc muốn xóa?',
      accept: () => {
        this.deleteResource(rowData.projectResourceId);
      }
    })
  }

  async deleteResource(projectResourceId: any) {
    this.loading = true;
    await this.projectService.deleteProjectResource(projectResourceId).subscribe(async response => {
      let result = <any>response;
      if (result.statusCode === 200 || result.statusCode === 202) {
        await this.getMasterData();
        this.isEdit = false;
        this.inforVendorForm.markAsUntouched();
        this.changeDetectorRef.detectChanges();
        this.isContractor = false;
        let msg = { severity: 'success', summary: 'Thông báo:', detail: 'Xóa nguồn lực thành công!' };
        this.showMessage(msg);
      } else {
        let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
        this.showMessage(msg);
      }
      this.loading = false;
    });
  }
  /* end*/

  resetDataExternalResource() {
    this.vendorNameControl.setValue(null);
    this.contactControl.setValue(null);
    this.paymentMethodControl.setValue(null);
    this.listContactById = []
    this.paymentId = null;
    this.vendorModel = new VendorModel();
    this.fullAddress = "";
    this.contactPerson = new contactModel();
  }

  onRowSelect(dataRow) {
  }
  showMessage(msg: any) {
    this.messageService.add(msg);
  }
  clearToast() {
    this.messageService.clear();
  }
  showToast(severity: string, summary: string, detail: string) {
    this.messageService.add({ severity: severity, summary: summary, detail: detail });
  }



  /*Event thay đổi nội dung ghi chú*/
  currentTextChange: string = '';
  changeNoteContent(event) {
    let htmlValue = event.htmlValue;
    this.currentTextChange = event.textValue;
  }

  /*Xử lý và hiển thị lại nội dung ghi chú*/
  handleNoteContent() {

    this.noteHistory.forEach(element => {
      setTimeout(() => {
        let count = 0;
        if (element.description == null) {
          element.description = "";
        }

        let des = $.parseHTML(element.description);
        let newTextContent = '';
        for (let i = 0; i < des.length; i++) {
          count += des[i].textContent.length;
          newTextContent += des[i].textContent;
        }

        if (count > 250) {
          newTextContent = newTextContent.substr(0, 250) + '<b>...</b>';
          $('#' + element.noteId).find('.short-content').append($.parseHTML(newTextContent));
        } else {
          $('#' + element.noteId).find('.short-content').append($.parseHTML(element.description));
        }

        $('#' + element.noteId).find('.full-content').append($.parseHTML(element.description));
      }, 1000);
    });
  }
  /*End*/

  /*Event Sửa ghi chú*/
  onClickEditNote(noteId: string, noteDes: string) {
    this.noteContent = noteDes;
    this.noteId = noteId;
    this.listUpdateNoteDocument = [];//this.noteHistory.find(x => x.noteId == this.noteId).noteDocList;
    this.isEditNote = true;
  }
  /*End*/

  /*Event Xóa ghi chú*/
  onClickDeleteNote(noteId: string) {
    this.confirmationService.confirm({
      message: 'Bạn chắc chắn muốn xóa ghi chú này?',
      accept: () => {
        this.loading = true;
        this.noteService.disableNote(noteId).subscribe(response => {
          let result: any = response;
          this.loading = false;
          if (result.statusCode == 200) {
            let note = this.noteHistory.find(x => x.noteId == noteId);
            let index = this.noteHistory.lastIndexOf(note);
            this.noteHistory.splice(index, 1);
            this.clearToast();
            this.showToast('success', 'Thông báo', 'Xóa ghi chú thành công');
          } else {
            this.clearToast();
            this.showToast('error', 'Thông báo', result.messageCode);
          }
        });
      }
    });
  }
  /*End*/

  /*Kiểm tra noteText > 250 ký tự hoặc noteDocument > 3 thì ẩn đi một phần nội dung*/
  tooLong(note): boolean {
    if (note.noteDocList.length > 3) return true;
    var des = $.parseHTML(note.description);
    var count = 0;
    for (var i = 0; i < des.length; i++) {
      count += des[i].textContent.length;
      if (count > 250) return true;
    }
    return false;
  }

  /*Event Mở rộng/Thu gọn nội dung của ghi chú*/
  toggle_note_label: string = 'Mở rộng';
  trigger_node(nodeid: string, event) {
    // noteContent
    let shortcontent_ = $('#' + nodeid).find('.short-content');
    let fullcontent_ = $('#' + nodeid).find('.full-content');
    if (shortcontent_.css("display") === "none") {
      fullcontent_.css("display", "none");
      shortcontent_.css("display", "block");
    } else {
      fullcontent_.css("display", "block");
      shortcontent_.css("display", "none");
    }

    let curr = $(event.target);

    if (curr.attr('class').indexOf('pi-chevron-right') != -1) {
      this.toggle_note_label = 'Thu gọn';
      curr.removeClass('pi-chevron-right');
      curr.addClass('pi-chevron-down');
    } else {
      this.toggle_note_label = 'Mở rộng';
      curr.removeClass('pi-chevron-down');
      curr.addClass('pi-chevron-right');
    }
  }
  /*End */

  /*Hủy sửa ghi chú*/
  cancelNote() {
    this.confirmationService.confirm({
      message: 'Bạn chắc chắn muốn hủy ghi chú này?',
      accept: () => {
        this.noteId = null;
        this.noteContent = null;
        this.uploadedFiles = [];
        // if (this.fileUpload) {
        //   this.fileUpload.clear();  //Xóa toàn bộ file trong control
        // }
        this.listUpdateNoteDocument = [];
        this.isEditNote = false;
      }
    });
  }

  /*Lưu file và ghi chú vào Db*/
  async saveNote() {

    this.loading = true;
    this.listNoteDocumentModel = [];
    let noteModel = new NoteModel();
    if (!this.noteId) {
      /*Tạo mới ghi chú*/
      noteModel.NoteId = this.emptyGuid;
      noteModel.Description = this.noteContent != null ? this.noteContent.trim() : "";
      noteModel.Type = 'NEW';
      noteModel.ObjectId = this.projectId;
      noteModel.ObjectType = 'PRORESOURCE';
      noteModel.NoteTitle = 'đã thêm ghi chú';
      noteModel.Active = true;
      noteModel.CreatedById = this.emptyGuid;
      noteModel.CreatedDate = new Date();
    } else {
      /*Update ghi chú*/
      noteModel.NoteId = this.noteId;
      noteModel.Description = this.noteContent != null ? this.noteContent.trim() : "";
      noteModel.Type = 'EDT';
      noteModel.ObjectId = this.projectId;
      noteModel.ObjectType = 'PRORESOURCE';
      noteModel.NoteTitle = 'đã sửa ghi chú';
      noteModel.Active = true;
      noteModel.CreatedById = this.emptyGuid;
      noteModel.CreatedDate = new Date();
    }
    this.noteService.createNoteForProjectResource(noteModel).subscribe(response => {
      let result: any = response;
      this.loading = false;
      if (result.statusCode == 200) {
        this.uploadedFiles = [];
        this.noteContent = null;
        this.listUpdateNoteDocument = [];
        this.noteId = null;
        this.isEditNote = false;
        this.paginator.changePage(0);
        this.clearToast();
        this.showToast('success', 'Thông báo', "Lưu ghi chú thành công!");
      } else {
        this.clearToast();
        this.showToast('error', 'Thông báo', result.messageCode);
      }
    });
  }

  async createNoteAfterEdit(type: string, noteTitle: string, noteBody: string) {
    let noteModel = new NoteModel();
    noteModel.NoteId = this.emptyGuid;
    noteModel.Description = noteBody;
    noteModel.Type = 'ADD';
    noteModel.ObjectId = this.projectId;
    noteModel.ObjectType = 'PRORESOURCE';
    noteModel.NoteTitle = noteTitle;
    noteModel.Active = true;
    noteModel.CreatedById = this.emptyGuid;
    noteModel.CreatedDate = new Date();
    this.noteService.createNoteForProjectResource(noteModel).subscribe(response => {
      let result: any = response;
      if (result.statusCode == 200) {
        this.uploadedFiles = [];
        // if (this.fileUpload) {
        //   this.fileUpload.clear();  //Xóa toàn bộ file trong control
        // }
        this.noteContent = null;
        this.listUpdateNoteDocument = [];
        this.noteId = null;
        this.isEditNote = false;
        /*Reshow Time Line */

        this.noteHistory = result.listNote;
        this.handleNoteContent();
      } else {
        this.clearToast();
        this.showToast('error', 'Thông báo', result.messageCode);
      }
    });
  }

  showProjectInfor() {
    this.isShow = !this.isShow;
    if (this.isShow) {
      window.scrollTo(0, 0)
    }
  }

  async paginate(event) {
    //event.first = Index of the first record
    //event.rows = Number of rows to display in new page
    //event.page = Index of the new page
    //event.pageCount = Total number of pages

    let result: any = await this.projectService.pagingProjectNote(this.projectId, event.rows, event.page, "PRORESOURCE");
    if (result.statusCode == 200) {
      this.noteHistory = result.noteEntityList;
      this.totalRecordsNote = result.totalRecordsNote
      await this.handleNoteContent();
    }
  }

  onChangeProject(projectId: string) {
    this.router.navigate(['/project/resource', { 'projectId': projectId }]);
  }

  async getPhanBoNguonLuc(data: any) {
    let result: any = await this.projectService.getPhanBoTheoNguonLuc(data.objectId);
    if (result.statusCode != 200) {
      let msg = { severity: 'error', summary: 'Thông báo:', detail: result.messageCode };
      this.showMessage(msg);
      return;
    }

    this.employeeCodeName = result.employeeCodeName;
    this.listPhanBoNguonLuc = result.listPhanBoNguonLuc;
    this.tongPhanTramRanhRoi = 100 - this.listPhanBoNguonLuc.map(x => x.phanTram).reduce((a, b) => a + b, 0);
    this.displayPhanBo = true;
  }


  ChangeThoiGian(event, type) {

    var toDay = new Date();
    if (type == "NB") {
      this.HienTaiNB = event.checked;
      if (event.checked) {
        this.listProjectResourcesInternalShow = this.listProjectResourcesInternal.filter(x => new Date(x.endTime) >= toDay);
      } else {
        this.listProjectResourcesInternalShow = this.listProjectResourcesInternal;
      }
      if (this.SelectNVNB.length > 0) this.listProjectResourcesInternalShow = this.listProjectResourcesInternalShow.filter(x => this.SelectNVNB.includes(x.objectId));
      this.tinhWorDay(this.listProjectResourcesInternalShow);
    } else {
      this.HienTaiTN = event.checked;
      if (event.checked) {
        this.listProjectResourcesExternalShow = this.listProjectResourcesExternal.filter(x => new Date(x.endTime) >= toDay);
      } else {
        this.listProjectResourcesExternalShow = this.listProjectResourcesExternal;
      }
      if (this.SelectNVTN.length > 0) this.listProjectResourcesExternalShow = this.listProjectResourcesExternalShow.filter(x => this.SelectNVTN.includes(x.objectId));
      this.tinhWorDay(this.listProjectResourcesExternalShow);
    }
  }
  changeSelectNV(event, type) {
    var toDay = new Date();
    //Lấy list employeeId
    let listEmpId = event.value.map(x => x.EmployeeId);
    if (type == "NB") {
      this.SelectNVNB = listEmpId;//list nhân viên nội bộ
      if (listEmpId.length > 0) {
        this.listProjectResourcesInternalShow = this.listProjectResourcesInternal.filter(x => listEmpId.includes(x.objectId));
      } else {
        this.listProjectResourcesInternalShow = this.listProjectResourcesInternal;
      }
      if (this.HienTaiNB) this.listProjectResourcesInternalShow = this.listProjectResourcesInternalShow.filter(x => new Date(x.endTime) >= toDay);
      this.tinhWorDay(this.listProjectResourcesInternalShow);
    } else {
      this.SelectNVTN = listEmpId;//list nhân viên thuê ngoài
      if (listEmpId.length > 0) {
        this.listProjectResourcesExternalShow = this.listProjectResourcesExternal.filter(x => listEmpId.includes(x.objectId));
      } else {
        this.listProjectResourcesExternalShow = this.listProjectResourcesExternal;
      }

      if (this.HienTaiTN) this.listProjectResourcesExternalShow = this.listProjectResourcesExternalShow.filter(x => new Date(x.endTime) >= toDay);
      this.tinhWorDay(this.listProjectResourcesExternalShow);
    }
  }

  tinhWorDay(list) {
    this.totalWorkdays = 0;
    list.forEach(a => this.totalWorkdays += a.workDay);
    this.totalWorkdays = Number(this.totalWorkdays.toFixed(2));
  }
}
