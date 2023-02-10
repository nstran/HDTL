//model dùng để build template excel
export class columnModel {
  column1: string;
  column2: string;
  constructor() { }
}

export class companyConfigModel {
  companyName: string;
  companyAddress: string;
  phone: string;
  email: string;
  website: string;
  constructor() {
    this.companyName = '';
    this.companyAddress = '';
    this.phone = '';
    this.email = '';
    this.website = '';
  }
}

export class exportExcelModel {
  maTaiSan: string;
  tenTaiSan: string;
  ngayVaoSo: string;
  hienTrangTaiSanString: string;
  constructor() { }
}

export class BaoCaoPhanBoModel {
  maTaiSan: string;
  tenTaiSan: string;
  phanLoaiTaiSan: string;
  hienTrangTaiSanString: string;
  ngayVaoSo: string;
  maNhanVien: string;
  tenNhanVien: string;
  phongBan: string;
  viTriLamViec: string;
  moTa: string;
}

export class baoCaoKhauHaoModel {
  maTaiSan: string;
  tenTaiSan: string;
  loaiTaiSanStr: string;
  hienTrangTaiSanStr: string;
  ngayVaoSo: string;
  giaTriNguyenGia: number;
  giaTriTinhKhauHao: number;
  tiLeKhauHaoTheoThang: number;
  tiLeKhauHaoTheoNam: number;
  giaTriKhauHaoTheoThang: number;
  giaTriKhauHaoTheoNam: number;
  thoiGianKhauHaoDen: Date;
  thoiDiemKTKhauHao: string;
  giaTriKhauHaoLuyKe: number;
  giaTriConLai: number;
}

