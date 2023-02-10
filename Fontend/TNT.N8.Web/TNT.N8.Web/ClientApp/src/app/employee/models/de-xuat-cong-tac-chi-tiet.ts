export class DeXuatCongTacChiTietModel {
  chiTietDeXuatCongTacId: number;
  employeeId: string;
  deXuatCongTacId: number;
  tenNhanVien: string;
  maNhanVien: string;
  phongBan: string;
  positionName: string;
  viTriLamViec: string;
  dateOfBirth: Date;
  identity: string;
  lyDo: string;
  error: boolean;
  isUpdate: boolean;
  createdById: string;
  createdDate: Date;
  updatedById: string;
  updatedDate: Date;

  constructor() {
    this.chiTietDeXuatCongTacId = 0;
    this.deXuatCongTacId = 0;
    this.employeeId = '00000000-0000-0000-0000-000000000000';
    this.lyDo = '';
    this.error = false;
    this.createdById = '00000000-0000-0000-0000-000000000000';
    this.createdDate = new Date();
    this.updatedById = null;
    this.updatedDate = null;
  }
}
