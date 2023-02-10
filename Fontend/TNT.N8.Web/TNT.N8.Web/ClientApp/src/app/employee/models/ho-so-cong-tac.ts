export class HoSoCongTacModel {
  hoSoCongTacId: number;
  deXuatCongTacId: number;
  maHoSoCongTac: string;
  deXuatCongTac: string;
  donViDen: string;
  diaDiem: string;
  phuongTien: string;
  nhiemVu: string;
  ngayBatDau: Date;
  ngayKetThuc: Date;
  active: boolean;
  createdById: string;
  createdDate: Date;
  updatedById: string;
  updatedDate: Date;
  trangThai: number;
  ketQuaCT: string;
  trangThaiText: string;

  constructor() {
    this.hoSoCongTacId = 0;
    this.deXuatCongTacId = 0;
    this.maHoSoCongTac = '';
    this.active = false;
    this.donViDen = '';
    this.diaDiem = '';
    this.phuongTien = '';
    this.nhiemVu = '';
    this.ngayBatDau = new Date();
    this.ngayKetThuc = new Date();

    this.ketQuaCT = '';
    this.createdDate = new Date();
    this.updatedById = null;
    this.updatedDate = null;
    this.trangThaiText = null;
  }
}
