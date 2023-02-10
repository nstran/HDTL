export class ThuHoiTaiSanModel {
  capPhatTaiSanId: number;
  loaiTaiSanPhanBo: number;
  taiSanId: number;
  nguoiSuDungId: string;
  mucDichSuDungId: string;
  ngayBatDau: Date;
  ngayKetThuc: Date;
  ngayBatDauString: string;
  ngayKetThucString: string;
  loaiCapPhat: number;
  nguoiCapPhatId: string;
  lyDo: string;
  createdById: string;
  createdDate: Date;
  updatedById: string;
  updatedDate: Date;
  tenTaiSan: string;
  employeeName: string;
  employeeCode: string;
  organizationName: string;
  positionName: string;
  mucDichSuDung: string;
  constructor() {
    this.capPhatTaiSanId = 0;
    this.loaiTaiSanPhanBo = 0;
    this.taiSanId = 0;
    this.nguoiSuDungId = '00000000-0000-0000-0000-000000000000';
    this.mucDichSuDungId = '00000000-0000-0000-0000-000000000000';
    this.ngayBatDau = new Date();
    this.ngayKetThuc = new Date();
    this.nguoiCapPhatId = '00000000-0000-0000-0000-000000000000';
    this.lyDo = '';
    this.loaiCapPhat = 0;
    this.createdById = '00000000-0000-0000-0000-000000000000';
    this.createdDate = new Date();
    this.updatedById = null;
    this.updatedDate = null;
  }
}
