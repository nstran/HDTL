export class DeXuatCongTacModel {
  deXuatCongTacId: number;
  tenDeXuat: string;
  ngayDeXuat: Date;
  nguoiDeXuatId: string;
  nguoiDeXuat: string;
  trangThaiDeXuat: number;
  donVi: string;
  diaDiem: string;
  phuongTien: string;
  nhiemVu: string;
  thoiGianBatDau: Date;
  thoiGianKetThuc: Date;
  thoiGianCT: string;
  createdById: string;
  createdDate: Date;
  updatedById: string;
  updatedDate: Date;

  maDeXuat: string;

  constructor() {
    this.deXuatCongTacId = 0;
    this.tenDeXuat = '';
    this.diaDiem = '';
    this.nguoiDeXuatId = '00000000-0000-0000-0000-000000000000';
    this.trangThaiDeXuat = 0;
    this.donVi = '';
    this.phuongTien = '';
    this.nhiemVu = '';
    this.thoiGianKetThuc = new Date();
    this.thoiGianBatDau = new Date();

    this.ngayDeXuat = new Date();
    this.createdDate = new Date();
    this.updatedById = null;
    this.updatedDate = null;
  }
}
