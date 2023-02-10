export class YeuCauCapPhatModel {
  yeuCauCapPhatTaiSanId: number;
  maYeuCau: string;
  ngayDeXuat: Date;
  nguoiDeXuatId: string;
  nguoiDeXuat: string;
  trangThai: number;

  createdById: string;
  createdDate: Date;
  updatedById: string;
  updatedDate: Date;
  constructor() {
    this.yeuCauCapPhatTaiSanId = 0;
    this.maYeuCau = '';
    this.nguoiDeXuatId = '00000000-0000-0000-0000-000000000000';
    this.trangThai = 0;
    this.ngayDeXuat = new Date();
    this.createdById = '00000000-0000-0000-0000-000000000000';
    this.createdDate = new Date();
    this.updatedById = null;
    this.updatedDate = null;
  }
}
