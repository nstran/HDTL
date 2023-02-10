export class DeNghiTamHoanUngModel {
  deNghiTamHoanUngId: number;
  hoSoCongTacId: number;
  maDeNghi: string;
  ngayDeNghi: Date;
  tienTamUng: number;
  tongTienThanhToan: number;
  nguoiThuHuongId: string;
  lyDo: string;
  trangThai: number;
  loaiDeNghi: number; // 0 Tạm ứng --- 1 Hoàn ứng
  active: boolean;
  createdById: string;
  createdDate: Date;
  updatedById: string;
  updatedDate: Date;

  constructor() {
    this.deNghiTamHoanUngId = 0;
    this.hoSoCongTacId = 0;
    this.maDeNghi = '';
    this.tienTamUng = 0;
    this.tongTienThanhToan = 0;
    this.nguoiThuHuongId = '00000000-0000-0000-0000-000000000000';
    this.active = false;
    this.ngayDeNghi = new Date();
    this.lyDo = '';
    this.createdDate = new Date();
    this.updatedById = null;
    this.updatedDate = null;
    this.loaiDeNghi = 0;
  }
}
