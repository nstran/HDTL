export class MucHuongTheoNgayNghi {
  mucHuongTheoNgayNghiId: number;
  troCapId: number;
  mucHuongPhanTram: any;
  loaiNgayNghi: number;
  soNgayTu: any
  soNgayDen: any;

  /* Virtual Field */
  id: number;
  cachTinh: string;

  constructor() {
    this.mucHuongTheoNgayNghiId = null;
    this.troCapId = null;
    this.mucHuongPhanTram = 0;
    this.loaiNgayNghi = null;
    this.soNgayTu = null;
    this.soNgayDen = null;
    this.id = 0;
  }
}