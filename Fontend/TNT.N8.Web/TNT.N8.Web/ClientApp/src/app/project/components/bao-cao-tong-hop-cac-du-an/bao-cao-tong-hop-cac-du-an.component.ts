import { Component, OnInit, ViewChild, ChangeDetectorRef } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { GetPermission } from '../../../shared/permission/get-permission';
import { MessageService, ConfirmationService } from 'primeng/api';
import { Table } from 'primeng/table';
import { ProjectService } from '../../services/project.service';

@Component({
  selector: 'app-bao-cao-tong-hop-cac-du-an',
  templateUrl: './bao-cao-tong-hop-cac-du-an.component.html',
  styleUrls: ['./bao-cao-tong-hop-cac-du-an.component.css']
})
export class BaoCaoTongHopCacDuAnComponent implements OnInit {
  auth = JSON.parse(localStorage.getItem("auth"));
  loading: boolean = false;
  projectId: string = null;

  cols: Array<any> = [];

  listProjectPipeline: Array<any> = [];
  selectedRow: any = null;

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private messageService: MessageService,
    private projectService: ProjectService,
  ) { }

  ngOnInit(): void {
    this.initTable();
    this.getBaoCaoTongHopCacDuAn();
  }

  initTable() {
    this.cols = [
      { field: 'stt', header: 'STT', textAlign: 'center', width: '80px'},
      { field: 'projectName', header: 'Tên dự án/Khách hàng', textAlign: 'left', width: '170px'},
      { field: 'ngayBatDauDuKien', header: 'Ngày bắt đầu dự kiến', textAlign: 'center', width: '160px'},
      { field: 'ngayKetThucDuKien', header: 'Ngày dự kiến kết thúc', textAlign: 'center', width: '160px'},
      { field: 'ngayBatDauThucTe', header: 'Ngày bắt đầu thực tế', textAlign: 'center', width: '160px'},
      { field: 'ngayKetThucThucTe', header: 'Ngày kết thúc thực tế', textAlign: 'center', width: '160px'},
      { field: 'ngayKyBienBanNghiemThu', header: 'Ngày ký BBNT', textAlign: 'center', width: '120px'},
      { field: 'ngayCongTheoNganSach', header: 'Ngày công', textAlign: 'right', width: '150px'},
      { field: 'vndTheoNganSach', header: 'VND', textAlign: 'right', width: '150px'},
      { field: 'usdTheoNganSach', header: 'USD', textAlign: 'right', width: '150px'},
      { field: 'ngayCongTheoThucTe', header: 'Ngày công', textAlign: 'right', width: '150px'},
      { field: 'vndTheoThucTe', header: 'VND', textAlign: 'right', width: '150px'},
      { field: 'usdTheoThucTe', header: 'USD', textAlign: 'right', width: '150px'},
      { field: 'hieuQuaSuDungNguonLuc', header: 'Hiệu quả sử dụng nguồn lực', textAlign: 'center', width: '140px'},
      { field: 'tienDo', header: 'Tiến độ', textAlign: 'center', width: '140px'},
      { field: 'trangThaiDuAn', header: 'Trạng thái dự án', textAlign: 'left', width: '140px'},
      { field: 'cacVanDeHienTai', header: 'Các vấn đề hiện tại của dự án', textAlign: 'left', width: '210px'},
      { field: 'ruiRo', header: 'Rủi ro', textAlign: 'left', width: '140px'},
      { field: 'baiHoc', header: 'Bài học', textAlign: 'left', width: '140px'},
      { field: 'ghiChu', header: 'Ghi chú', textAlign: 'left', width: '140px'},
    ];
  }

  async getBaoCaoTongHopCacDuAn() {
    this.loading = true;
    let result: any = await this.projectService.getBaoCaoTongHopCacDuAn();
    this.loading = false;

    if (result.statusCode != 200) {
      this.showMessage('error', result.messageCode);
      return;
    }

    this.listProjectPipeline = result.listProjectPipeline;
  }

  showMessage(severity: string, detail: string) {
    this.messageService.add({ severity: severity, summary: 'Thông báo', detail: detail });
  }
}
