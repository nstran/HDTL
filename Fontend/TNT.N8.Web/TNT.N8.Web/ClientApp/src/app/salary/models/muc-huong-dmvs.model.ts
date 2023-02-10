export class MucHuongDmvs {
  mucHuongDmvsId: number;
  troCapId: number;
  hinhThucTru: number;
  mucTru: any;
  soLanTu: any;
  soLanDen: any;

  /* Virtual Field */
  id: number;
  hinhThucTruText: string;
  cachTinh: string;

  constructor() {
    this.mucHuongDmvsId = null;
    this.troCapId = null;
    this.hinhThucTru = null;
    this.mucTru = null;
    this.soLanTu = null;
    this.soLanDen = null;
    this.id = 0;
    this.hinhThucTruText = null;
    this.cachTinh = null;
  }
}