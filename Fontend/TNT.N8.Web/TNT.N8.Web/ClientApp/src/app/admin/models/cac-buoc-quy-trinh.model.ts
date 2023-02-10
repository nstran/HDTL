import { PhongBanTrongCacBuocQuyTrinh } from './phong-ban-trong-cac-buoc-quy-trinh.model';

export class CacBuocQuyTrinh {
  id: string;
  stt: number;
  loaiPheDuyet: number;
  cauHinhQuyTrinhId: string;
  listPhongBanTrongCacBuocQuyTrinh: Array<PhongBanTrongCacBuocQuyTrinh>;

  constructor() {
    this.listPhongBanTrongCacBuocQuyTrinh = [];
  }
}