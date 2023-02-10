import { CacBuocQuyTrinh } from './cac-buoc-quy-trinh.model';

export class CauHinhQuyTrinh {
  id: string;
  soTienTu: any;
  tenCauHinh: string;
  quyTrinh: string;
  quyTrinhId: string;
  listCacBuocQuyTrinh: Array<CacBuocQuyTrinh>;

  //virtual field
  error: boolean;

  constructor() {
    this.tenCauHinh = null;
    this.quyTrinh = null;
    this.listCacBuocQuyTrinh = [];
    this.error = false;
  }
}