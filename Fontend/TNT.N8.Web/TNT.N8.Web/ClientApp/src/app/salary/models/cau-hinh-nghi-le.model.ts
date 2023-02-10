import { CauHinhNghiLeChiTiet } from "./cau-hinh-nghi-le-chi-tiet.model";

export class CauHinhNghiLe {
  nghiLeId: number;
  soNam: number;

  /* Virtual Field */
  listCauHinhNghiLeChiTiet: Array<CauHinhNghiLeChiTiet>;

  constructor() {
    this.nghiLeId = null;
    this.soNam = null;

    this.listCauHinhNghiLeChiTiet = [];
  }
}