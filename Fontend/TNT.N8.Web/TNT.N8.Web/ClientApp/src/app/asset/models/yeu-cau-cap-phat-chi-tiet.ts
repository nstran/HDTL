export class YeuCauCapPhatChiTietModel {
  yeuCauCapPhatTaiSanChiTietId: number;
  taiSanId: number;
  maTaiSan: string;
  yeuCauCapPhatTaiSanId: number;
  loaiTaiSanId: string;
  loaiTaiSan: string;

  moTa: string;
  nguoiSuDungId: string;
  nhanVienYeuCauId: string;
  maNV: string;
  tenNhanVien: string;
  phongBan: string;
  viTriLamViec: string;
  mucDichSuDungId: string;
  mucDichSuDung: string;
  ngayBatDau: Date;
  ngayBatDauString: string;
  ngayKetThuc: Date;
  ngayKetThucString: string;
  lyDo: string;
  trangThai: number;
  soLuong: number;
  soLuongPheDuyet: number;
  error: boolean;

  createdById: string;
  createdDate: Date;
  updatedById: string;
  updatedDate: Date;

  employeeName: string;
  organizationName: string;
  positionName: string;
  parentPartId: number;
  totalChild: number;
  isUpdate: boolean;
  capPhatTaiSanId: number;

  constructor() {
    this.yeuCauCapPhatTaiSanChiTietId = 0;
    this.taiSanId = 0;
    this.maTaiSan = '';
    this.yeuCauCapPhatTaiSanId = 0;
    this.loaiTaiSanId = '00000000-0000-0000-0000-000000000000';
    this.moTa = '';
    this.maNV = '';
    this.nguoiSuDungId = '00000000-0000-0000-0000-000000000000';
    this.nhanVienYeuCauId = '00000000-0000-0000-0000-000000000000';
    this.mucDichSuDungId = '00000000-0000-0000-0000-000000000000';
    this.ngayBatDau = new Date();
    this.ngayKetThuc = new Date();
    this.lyDo = '';
    this.trangThai = 0;
    this.soLuong = 0;
    this.soLuongPheDuyet = 0;
    this.error = false;
    this.createdById = '00000000-0000-0000-0000-000000000000';
    this.createdDate = new Date();
    this.updatedById = null;
    this.updatedDate = null;
  }
}
