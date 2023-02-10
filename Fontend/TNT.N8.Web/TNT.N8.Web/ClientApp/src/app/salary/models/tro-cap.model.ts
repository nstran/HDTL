import { MucHuongDmvs } from "./muc-huong-dmvs.model";
import { MucHuongTheoNgayNghi } from "./muc-huong-theo-ngay-nghi.model";
import { TroCapChucVuMapping } from "./tro-cap-chuc-vu-mapping.model";
import { TroCapDieuKienHuongMapping } from "./tro-cap-dieu-kien-huong-mapping.model";
import { TroCapLoaiHopDongMapping } from "./tro-cap-loai-hop-dong-mapping.model";

export class TroCap {
  troCapId: number;
  typeId: number; 
  loaiTroCapId: string;
  mucTroCap: any;

  /* Virtual Field */
  loaiTroCap: string;
  chucVuText: string;
  loaiHopDongText: string; 
  dieuKienHuongText: string; 

  listChucVu: Array<TroCapChucVuMapping>;
  listLoaiHopDong: Array<TroCapLoaiHopDongMapping>;
  listDieuKienHuong: Array<TroCapDieuKienHuongMapping>;
  listMucHuongTheoNgayNghi: Array<MucHuongTheoNgayNghi>;
  listMucHuongDmvs: Array<MucHuongDmvs>;

  constructor() {
    this.troCapId = null;
    this.typeId = null;
    this.loaiTroCapId = null;
    this.mucTroCap = 0;
    this.listChucVu = [];
    this.listLoaiHopDong = [];
    this.listDieuKienHuong = [];
    this.listMucHuongTheoNgayNghi = [];
    this.listMucHuongDmvs = [];
  }
}