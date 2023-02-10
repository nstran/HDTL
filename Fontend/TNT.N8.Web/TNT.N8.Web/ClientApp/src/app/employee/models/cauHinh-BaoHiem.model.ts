export class CauHinhBaoHiemModel {
  cauHinhBaoHiemId: number;
  loaiDong: number;
  mucDong: number;
  mucDongToiDa: number;
  mucLuongCoSo: number;
  tiLePhanBoMucDongBhxhcuaNld: number;
  tiLePhanBoMucDongBhxhcuaNsdld: number;
  tiLePhanBoMucDongBhytcuaNld: number;
  tiLePhanBoMucDongBhytcuaNsdld: number;
  tiLePhanBoMucDongBhtncuaNld: number;
  tiLePhanBoMucDongBhtncuaNsdld: number;
  tiLePhanBoMucDongBhtnnncuaNld: number;
  tiLePhanBoMucDongBhtnnncuaNsdld: number;

  constructor() {
    this.cauHinhBaoHiemId = null,
    this.loaiDong = 1;
    this.mucDong = 0;
    this.mucDongToiDa = 0;
    this.mucLuongCoSo = 0;
    this.tiLePhanBoMucDongBhxhcuaNld = 0;
    this.tiLePhanBoMucDongBhxhcuaNsdld = 0;
    this.tiLePhanBoMucDongBhytcuaNld = 0;
    this.tiLePhanBoMucDongBhytcuaNsdld = 0;
    this.tiLePhanBoMucDongBhtncuaNld = 0;
    this.tiLePhanBoMucDongBhtncuaNsdld = 0;
    this.tiLePhanBoMucDongBhtnnncuaNld = 0;
    this.tiLePhanBoMucDongBhtnnncuaNsdld = 0;
  }
}

export class CauHinhBaoHiemLoftCareModel {
  cauHinhBaoHiemLoftCareId?: number;
  namCauHinh?: string;
  nhomBaoHiemLoftCare?: NhomBaoHiemLoftCareModel;
  quyenLoiBaoHiemLoftCare?: QuyenLoiBaoHiemLoftCareModel;
  listNhomBaoHiemLoftCare?: Array<NhomBaoHiemLoftCareModel>;
  listQuyenLoiBaoHiemLoftCare?: Array<QuyenLoiBaoHiemLoftCareModel>;

  //virtual field
  index?: number;

  constructor() {
    this.cauHinhBaoHiemLoftCareId = null;
    this.namCauHinh = null;
    this.nhomBaoHiemLoftCare = null;
    this.quyenLoiBaoHiemLoftCare = null;
    this.listNhomBaoHiemLoftCare = [];
    this.listQuyenLoiBaoHiemLoftCare = [];

    this.index = 0;
  }
}

export class NhomBaoHiemLoftCareModel {
  nhomBaoHiemLoftCareId?: number;
  cauHinhBaoHiemLoftCareId: number;
  tenNhom: string;

  listChucVuBaoHiemLoftCare?: Array<ChucVuBaoHiemLoftCareModel>;

  constructor() {
    this.nhomBaoHiemLoftCareId = null;
    this.cauHinhBaoHiemLoftCareId = null;
    this.tenNhom = null;
    
    this.listChucVuBaoHiemLoftCare = [];
  }
}

export class QuyenLoiBaoHiemLoftCareModel {
  quyenLoiBaoHiemLoftCareId?: number;
  nhomBaoHiemLoftCareId?: number;
  tenQuyenLoi?: string;
  
  listMucHuongBaoHiemLoftCare?: Array<MucHuongBaoHiemLoftCareModel>;

  constructor() {
    this.quyenLoiBaoHiemLoftCareId = null;
    this.nhomBaoHiemLoftCareId = null;
    this.tenQuyenLoi = null;
    this.listMucHuongBaoHiemLoftCare = [];
  }
}

export class ChucVuBaoHiemLoftCareModel {
  chucVuBaoHiemLoftCareId: number;
  nhomBaoHiemLoftCareId: number;
  positionId: number;
  soNamKinhNghiem: number;

  //virtual field
  positionName: string;

  constructor() {
    this.chucVuBaoHiemLoftCareId = null;
    this.nhomBaoHiemLoftCareId = null;
    this.positionId = null;
    this.soNamKinhNghiem = 0;
    this.positionName = null;
  }
}

export class MucHuongBaoHiemLoftCareModel {
  id: number;
  quyenLoiBaoHiemLoftCareId: number;
  doiTuongHuong: number; //1: Bản thân, 2: Người thân
  mucHuong: number;
  donVi: number;
  lePhi: number;
  phiCoDinh: number;
  phiTheoLuong: number;
  mucGiam: number;

  //virtual field
  tenDoiTuong: string;
  selectedDonVi: any;

  constructor() {
    this.id = null;
    this.quyenLoiBaoHiemLoftCareId = null;
    this.doiTuongHuong = null;
    this.mucHuong = 0;
    this.donVi = null;
    this.lePhi = 0;
    this.phiCoDinh = 0;
    this.phiTheoLuong = 0;
    this.mucGiam = 0;

    this.tenDoiTuong = null;
    this.selectedDonVi = null;
  }
}