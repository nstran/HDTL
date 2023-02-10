export class DeXuatChucVuModel {
  deXuatThayDoiChucVuId:  number;
  tenDeXuat: string;
  ngayDeXuat: Date;
  nguoiDeXuatId: string;
  taiLieuLienQuanId: string;
  trangThai: number;
  createdById: string;
  createdDate: Date;
  updatedById: string;
  updatedDate: Date;
  ngayApDung: Date;
  active: boolean;


  constructor() {
    this.deXuatThayDoiChucVuId =0,
    this.tenDeXuat = "",
    this.ngayDeXuat = new Date();
    this.nguoiDeXuatId = "";
    this.taiLieuLienQuanId = null;;
    this.trangThai = 1;
    this.createdById = null;;
    this.createdDate = new Date();
    this.updatedById =null;;
    this.updatedDate = new Date();
    this.ngayApDung = new Date();
    this.active = true;
  }
}


export class NhanVienDeXuatThayDoiChucVu {
  nhanVienDeXuatThayDoiChucVuId: number;
  deXuatThayDoiChucVuId: number;
  employeeId: string;
  chucVuDeXuatId: string;
  chucVuHienTaiId: string;
  lyDoDeXuat: string;
  trangThai: number;
  active: boolean;

  createdById: string;
  createdDate: Date;
  updatedById: string;
  updatedDate: Date;




  constructor() {
    this.nhanVienDeXuatThayDoiChucVuId =0,
    this.deXuatThayDoiChucVuId =  0;
    this.employeeId =  null;
    this.chucVuDeXuatId =  null;
    this.chucVuHienTaiId =  null;
    this.trangThai = 1;
    this.lyDoDeXuat = "";
    this.createdById =  null;
    this.createdDate = new Date();
    this.updatedById =  null;
    this.updatedDate =new Date();
    this.active = true;
  }
}

