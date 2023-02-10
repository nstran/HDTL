import { Component, OnInit, Input, ViewChild } from '@angular/core';
import { MessageService } from 'primeng/api';
import { DialogService } from 'primeng/dynamicdialog';
import { Table } from 'primeng/table';
import { ConfirmationService } from 'primeng/api';
import { GetPermission } from '../../../../shared/permission/get-permission';
import { EmployeeService } from './../../../services/employee.service';
import { ThemMoiCauhinhChecklistComponent } from '../them-moi-cauhinh-checklist/them-moi-cauhinh-checklist.component';
import { Router, ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-list-cauhinh-checklist',
  templateUrl: './list-cauhinh-checklist.component.html',
  styleUrls: ['./list-cauhinh-checklist.component.css']
})
export class ListCauhinhChecklistComponent implements OnInit {
  loading: boolean = false;

  cols: Array<any> = [];
  listCauHinh: Array<any> = [];

  isShowButton: boolean = false;

  actionAdd: boolean = false;
  actionEdit: boolean = false;
  actionDelete: boolean = false;

  @ViewChild('myTable') myTable: Table;

  constructor(
    public dialogService: DialogService,
    private messageService: MessageService,
    private employeeService: EmployeeService,
    public confirmationService: ConfirmationService,
    private getPermission: GetPermission,
    private router: Router,
  ) { }

  async ngOnInit() {
    this.initTable();
    await this._getPermission();
    this.getListCauHinhChecklist();
  }

  initTable() {
    this.cols = [
      { field: 'stt', header: '#', textAlign: 'center', colWith: '10vw' },
      { field: 'tenTaiLieu', header: 'Tên tài liệu', textAlign: 'left', colWith: '' },
      { field: 'active', header: 'Trạng thái', textAlign: 'center', colWith: '20vw' },
      { field: 'action', header: 'Thao tác', textAlign: 'center', colWith: '150px' }
    ];
  }

  async _getPermission() {
    let resource = "hrm/employee/cauhinh-checklist/";
    let permission: any = await this.getPermission.getPermission(resource);

    if (permission.status == false) { 
      this.router.navigate(["/home"]);
      return;
    }

    if (permission.listCurrentActionResource.indexOf("add") != -1) {
      this.actionAdd = true;
    }

    if (permission.listCurrentActionResource.indexOf("edit") != -1) {
      this.actionEdit = true;
    }

    if (permission.listCurrentActionResource.indexOf("delete") != -1) {
      this.actionDelete = true;
    }
  }

  async getListCauHinhChecklist() {
    this.loading = true;
    let result: any = await this.employeeService.getListCauHinhChecklist();
    this.loading = false;

    if (result.statusCode == 200) {
      this.listCauHinh = result.listCauHinhChecklist;
      this.isShowButton = result.isShowButton;
    }
    else {
      this.showMessage('error', result.messageCode);
    }
  }

  addTaiLieu(rowData = null) {
    let isAdd: boolean = true;
    if (rowData) {
      isAdd = false;
    }
    let ref = this.dialogService.open(ThemMoiCauhinhChecklistComponent, {
      data: {
        isAdd: isAdd,
        taiLieu: rowData
      },
      header: isAdd ? 'Thêm mới tài liệu' : 'Chỉnh sửa tài liệu',
      width: '690px',
      baseZIndex: 1030,
      contentStyle: {
        "min-height": "250px",
        "max-height": "500px",
        "overflow": "auto"
      }
    });

    ref.onClose.subscribe((result: any) => {
      this.getListCauHinhChecklist();
    });
  }

  deleteTaiLieu(data) {
    this.confirmationService.confirm({
      message: 'Bạn có chắc chắn muốn xóa, dữ liệu sẽ không thể hoàn tác?',
      accept: async () => {
        try {
          this.loading = true;
          let result: any = await this.employeeService.deleteCauHinhChecklistById(data.cauHinhChecklistId);
          this.loading = false;

          if (result.statusCode != 200) {
            this.showMessage('error', result.messageCode);
            return;
          }
          this.showMessage('success', "Xóa dữ liệu thành công");
          this.getListCauHinhChecklist();
        } catch (error) {
          this.showMessage('error', error);
        }
      }
    });
  }

  showMessage(severity: string, detail: string) {
    let msg = { severity: severity, summary: 'Thông báo:', detail: detail };
    this.messageService.add(msg);
  }

}
