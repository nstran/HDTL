import { Component, OnInit } from '@angular/core';
import { DialogService } from 'primeng/dynamicdialog';
import { MessageService, ConfirmationService } from "primeng/api";
import { TreeNode } from 'primeng/api';

import { EmployeeService } from "../../../services/employee.service";

import { CauHinhBaoHiemLoftCareModel } from "../../../models/cauhinh-baohiem.model";

import { ThemNamCauHinhComponent } from '../them-nam-cau-hinh/them-nam-cau-hinh.component';
import { ThemNhomBaoHiemComponent } from '../them-nhom-bao-hiem/them-nhom-bao-hiem.component';
import { ThemQuyenLoiBaoHiemComponent } from '../them-quyen-loi-bao-hiem/them-quyen-loi-bao-hiem.component';

@Component({
  selector: 'app-bao-hiem-loftcare',
  templateUrl: './bao-hiem-loftcare.component.html',
  styleUrls: ['./bao-hiem-loftcare.component.css']
})
export class BaoHiemLoftcareComponent implements OnInit {
  loading: boolean = false;

  listCauHinhBaoHiemLoftCare: Array<CauHinhBaoHiemLoftCareModel> = [];
  listPosition: Array<any> = [];
  listDonVi: Array<any> = [];
  listDoiTuong: Array<any> = [];

  cauHinhBaoHiemLoftCare: TreeNode[];

  constructor(
    public dialogService: DialogService,
    private employeeService: EmployeeService,
    private messageService: MessageService,
    private confirmationService: ConfirmationService
  ) { }

  ngOnInit(): void {
    this.getCauHinhBaoHiemLoftCareById();
  }

  // Lấy dữ liệu ban đầu
  async getCauHinhBaoHiemLoftCareById() {
    this.loading = true;
    let result: any = await this.employeeService.getCauHinhBaoHiemLoftCareById();
    this.loading = false;

    if (result.statusCode != 200) {
      this.showMessage("error", result.messageCode);
      return;
    }

    this.listCauHinhBaoHiemLoftCare = result.listCauHinhBaoHiemLoftCare;
    this.listPosition = result.listPosition;
    this.listDonVi = result.listDonVi;
    this.listDoiTuong = result.listDoiTuong;

    this.setData();
  }

  setData() {
    let dataTable = [];
    this.listCauHinhBaoHiemLoftCare.forEach(item => {
      let node = {
        data: {
          name: item.namCauHinh,
          id: item.cauHinhBaoHiemLoftCareId,
          value: item
        },
        children: []
      }
      if (item.listNhomBaoHiemLoftCare.length > 0) {
        item.listNhomBaoHiemLoftCare.forEach(item2 => {
          let nodeLevel2 = {
            data: {
              name: item2.tenNhom,
              id: item2.nhomBaoHiemLoftCareId,
              value: item2
            },
            children: []
          }
          if (item2.listChucVuBaoHiemLoftCare.length > 0) {
            item2.listChucVuBaoHiemLoftCare.forEach(cv => {
              let chucVu = this.listPosition.find(x => x.positionId == cv.positionId);
              if(cv.soNamKinhNghiem > 0) {
                cv.positionName = chucVu?.positionName + ' - từ ' + cv.soNamKinhNghiem + ' năm kinh nghiệm';
              } else {
                cv.positionName = chucVu?.positionName;
              }
              
            });
            let lstChucVuName = item2.listChucVuBaoHiemLoftCare.map(x => x.positionName);
            nodeLevel2.data.name = nodeLevel2.data.name + " (" + lstChucVuName + ")"
          }
          if (item.listQuyenLoiBaoHiemLoftCare.length > 0) {
            let lstQuyenLoi = item.listQuyenLoiBaoHiemLoftCare.filter(x => x.nhomBaoHiemLoftCareId == item2.nhomBaoHiemLoftCareId);
            lstQuyenLoi.forEach(item3 => {
              let nodeLevel3 = {
                data: {
                  name: item3.tenQuyenLoi,
                  id: item3.quyenLoiBaoHiemLoftCareId,
                  value: item3
                },
                children: []
              }
              nodeLevel2.children = [...nodeLevel2.children, nodeLevel3]
            })
          }
          node.children = [...node.children, nodeLevel2]
        })
      }
      dataTable = [...dataTable, node];
    })

    this.cauHinhBaoHiemLoftCare = dataTable;
  }

  addNamCauHinh() {
    let ref = this.dialogService.open(ThemNamCauHinhComponent, {
      data: {
        isAdd: true
      },
      header: 'Năm cấu hình bảo hiểm',
      width: '690px',
      baseZIndex: 1030,
      contentStyle: {
        "min-height": "150px",
        "max-height": "500px",
        "overflow": "auto"
      }
    });

    ref.onClose.subscribe((result: any) => {
      if (result && result.status)
        this.getCauHinhBaoHiemLoftCareById();
    });
  }

  addNode(rowNode) {
    if (rowNode.level == 1) {
      let ref = this.dialogService.open(ThemQuyenLoiBaoHiemComponent, {
        data: {
          isAdd: true,
          listDonVi: this.listDonVi,
          listDoiTuong: this.listDoiTuong,
          nhomBaoHiemLoftCareId: rowNode.node.data.id
        },
        header: 'Chi tiết thông tin bảo hiểm',
        width: '1000px',
        baseZIndex: 1030,
        contentStyle: {
          "min-height": "150px",
          "max-height": "500px",
          "overflow": "auto"
        }
      });

      ref.onClose.subscribe((result: any) => {
        if (result && result.status)
          this.getCauHinhBaoHiemLoftCareById();
      });
    } else {
      let ref = this.dialogService.open(ThemNhomBaoHiemComponent, {
        data: {
          isAdd: true,
          listPosition: this.listPosition,
          cauHinhBaoHiemLoftCareId: rowNode.node.data.id
        },
        header: 'Đối tượng hưởng bảo hiểm',
        width: '690px',
        baseZIndex: 1030,
        contentStyle: {
          "min-height": "350px",
          "max-height": "500px",
          "overflow": "auto"
        }
      });

      ref.onClose.subscribe((result: any) => {
        if (result && result.status)
          this.getCauHinhBaoHiemLoftCareById();
      });
    }
  }

  editNode(rowData, rowNode) {
    if (rowNode.level == 0) {
      let ref = this.dialogService.open(ThemNamCauHinhComponent, {
        data: {
          isAdd: false,
          cauHinhBaoHiem: rowData.value
        },
        header: 'Năm cấu hình bảo hiểm',
        width: '690px',
        baseZIndex: 1030,
        contentStyle: {
          "min-height": "150px",
          "max-height": "500px",
          "overflow": "auto"
        }
      });

      ref.onClose.subscribe((result: any) => {
        if (result && result.status)
          this.getCauHinhBaoHiemLoftCareById();
      });
      
    } else if (rowNode.level == 1) {
      let ref = this.dialogService.open(ThemNhomBaoHiemComponent, {
        data: {
          isAdd: false,
          listPosition: this.listPosition,
          cauHinhBaoHiemLoftCareId: rowNode.node.data.id,
          nhomBaoHiem: rowData.value
        },
        header: 'Đối tượng hưởng bảo hiểm',
        width: '690px',
        baseZIndex: 1030,
        contentStyle: {
          "min-height": "350px",
          "max-height": "500px",
          "overflow": "auto"
        }
      });

      ref.onClose.subscribe((result: any) => {
        if (result && result.status)
          this.getCauHinhBaoHiemLoftCareById();
      });
    } else {
      let ref = this.dialogService.open(ThemQuyenLoiBaoHiemComponent, {
        data: {
          isAdd: false,
          listDonVi: this.listDonVi,
          listDoiTuong: this.listDoiTuong,
          nhomBaoHiemLoftCareId: rowNode.node.data.id,
          quyenLoiBaoHiem: rowData.value
        },
        header: 'Chi tiết thông tin bảo hiểm',
        width: '1000px',
        baseZIndex: 1030,
        contentStyle: {
          "min-height": "150px",
          "max-height": "500px",
          "overflow": "auto"
        }
      });

      ref.onClose.subscribe((result: any) => {
        if (result && result.status)
          this.getCauHinhBaoHiemLoftCareById();
      });
    }
  }

  deleteNode(rowData, rowNode) {
    let type = null;
    let id = rowData.id;
    if(rowNode.level == 0) {
      type = 1;
    } else if(rowNode.level == 1) {
      type = 2;
    } else {
      type = 3;
    }
    this.confirmationService.confirm({
      message: 'Bạn có chắc chắn muốn xóa, dữ liệu sẽ không thể hoàn tác?',
      accept: async () => {
        this.loading = true;
        let result: any = await this.employeeService.deleteCauHinhBaoHiemLoftCare(type, id);
        this.loading = false;

        if (result.statusCode != 200) {
          this.showMessage("error", result.messageCode);
          return;
        }

        this.getCauHinhBaoHiemLoftCareById();
        this.showMessage("success", result.messageCode);
      }
    });
  }

  showMessage(severity: string, detail: string) {
    let msg = { severity: severity, summary: 'Thông báo:', detail: detail };
    this.messageService.add(msg);
  }

}
