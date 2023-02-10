export class BaoHiemXaHoiModel {
  soSoBhxh: string;
  maTheBhyt: string;
  mucDongBhxh?: number;
  tyLeDongBhxh?: number;
  tyLeDongBhyt?: number;
  tyLeDongBhtn?: number;
  tyLeDongBhtnnn?: number;
}

export class BaoHiemLoftCareModel {
  maTheBhLoftCare: string;
  nhomBhLoftCare?: string;
}

export class ThongTinThueVaGiamTruModel {
  maSoThueCaNhan: string;
  soNguoiDangKyPhuThuoc: number;
  // thangNopDangKyGiamTru: Date;
  listDoiTuongPhuThuocId?: Array<string>;
}