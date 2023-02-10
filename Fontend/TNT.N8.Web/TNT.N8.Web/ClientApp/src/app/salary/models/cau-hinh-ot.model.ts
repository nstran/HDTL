export class CauHinhOt {
  cauHinhOtId: number;
  loaiOtId: string; 
  tyLeHuong: number;

  /* Virtual Field */
  stt: number;
  tenLoaiOt: string;

  constructor() {
    this.cauHinhOtId = null;
    this.loaiOtId = null;
    this.tyLeHuong = null;
    this.tenLoaiOt = null;
    this.stt = null;
  }
}