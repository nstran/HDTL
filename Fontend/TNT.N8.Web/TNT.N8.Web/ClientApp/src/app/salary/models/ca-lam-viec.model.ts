import { CaLamViecChiTiet } from "./ca-lam-viec-chi-tiet.model";

export class CaLamViec {
  caLamViecId: number;
  loaiCaLamViecId: number;
  gioVao: any;
  gioRa: any;
  thoiGianKetThucCa: any;

  /* Virtual Field */
  tenLoaiCaLamViec: string;
  listCaLamViecChiTiet: Array<CaLamViecChiTiet>;
  ngayLamViecTrongTuanText: string;

  constructor() {
    this.caLamViecId = null;
    this.loaiCaLamViecId = null;
    this.gioVao = null;
    this.gioRa = null;

    this.tenLoaiCaLamViec = null;
    this.thoiGianKetThucCa = [];
    this.ngayLamViecTrongTuanText = null;
  }
}