export class CaLamViecChiTiet {
  caLamViecChiTietId: number;
  caLamViecId: number;
  ngayTrongTuan: number;

  /* Virtual Field */
  ngayTrongTuanText: string; 

  constructor() {
    this.caLamViecChiTietId = null;
    this.caLamViecId = null;
    this.ngayTrongTuan = null;

    this.ngayTrongTuanText = null;
  }
}