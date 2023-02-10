import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class SalaryService {

  constructor(private httpClient: HttpClient) { }

  getMasterCauHinhLuong() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/salary/getMasterCauHinhLuong';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {}).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  createOrUpdateCaLamViec(caLamViec: any) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/salary/createOrUpdateCaLamViec';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        CaLamViec: caLamViec
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  deleteCauHinhCaLamViec(caLamViecId: number) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/salary/deleteCauHinhCaLamViec";
    return new Promise((resolve, rejecr) => {
      return this.httpClient.post(url, {
        caLamViecId: caLamViecId,
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  createOrUpdateCauHinhNghiLe(CauHinhNghiLe: any) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/salary/createOrUpdateCauHinhNghiLe";
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        CauHinhNghiLe: CauHinhNghiLe
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  deleteCauHinhNghiLe(nghiLeId: number) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/salary/deleteCauHinhNghiLe";
    return new Promise((resolve, rejecr) => {
      return this.httpClient.post(url, {
        nghiLeId: nghiLeId,
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  createOrUpdateCauHinhOt(CauHinhOt: any) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/salary/createOrUpdateCauHinhOt";
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        CauHinhOt: CauHinhOt
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  deleteCauHinhOt(CauHinhOtId: number) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/salary/deleteCauHinhOt";
    return new Promise((resolve, rejecr) => {
      return this.httpClient.post(url, {
        CauHinhOtId: CauHinhOtId,
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  createOrUpdateCauHinhGiamTru(CauHinhGiamTru: any) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/salary/createOrUpdateCauHinhGiamTru";
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        CauHinhGiamTru: CauHinhGiamTru
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  deleteCauHinhGiamTru(CauHinhGiamTruId: number) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/salary/deleteCauHinhGiamTru";
    return new Promise((resolve, rejecr) => {
      return this.httpClient.post(url, {
        CauHinhGiamTruId: CauHinhGiamTruId,
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  createOrUpdateKinhPhi(KinhPhiCongDoan: any) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/salary/createOrUpdateKinhPhi";
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        KinhPhiCongDoan: KinhPhiCongDoan
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  createOrUpdateCongThucTinhLuong(CongThucTinhLuong: any) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/salary/createOrUpdateCongThucTinhLuong';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        CongThucTinhLuong: CongThucTinhLuong
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getTkDiMuonVeSom(tuNgay: any, denNgay: any, listEmployeeId: any, isShowOption: any) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/salary/getTkDiMuonVeSom';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        tuNgay: tuNgay,
        denNgay: denNgay,
        listEmployeeId: listEmployeeId,
        isShowOption: isShowOption
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  importChamCong(ListData: any) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/salary/importChamCong';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        ListData: ListData
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getMasterThongKeChamCong() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/salary/getMasterThongKeChamCong';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {

      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  createOrUpdateChamCong(TypeUpdate: string, TypeField: number, ListEmployeeId: Array<string>, NgayChamCong: Date, ThoiGian: string, 
    MaKyHieu: number) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/salary/createOrUpdateChamCong';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        TypeUpdate: TypeUpdate,
        TypeField: TypeField,
        ListEmployeeId: ListEmployeeId,
        NgayChamCong: NgayChamCong,
        ThoiGian: ThoiGian,
        MaKyHieu: MaKyHieu
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  createOrUpdateCauHinhChamCongOt(cauHinhChamCongOt: any) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/salary/createOrUpdateCauHinhChamCongOt';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        cauHinhChamCongOt: cauHinhChamCongOt
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getTkThoiGianOt(tuNgay: any, denNgay: any, listEmployeeId: any, isShowOption: any) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/salary/getTkThoiGianOt';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        tuNgay: tuNgay,
        denNgay: denNgay,
        listEmployeeId: listEmployeeId,
        isShowOption: isShowOption
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  createOrUpdateCauHinhOtCaNgay(cauHinhOtCaNgay: any) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/salary/createOrUpdateCauHinhOtCaNgay';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        cauHinhOtCaNgay: cauHinhOtCaNgay
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  createOrUpdateCauHinhThueTncn(CauHinhThueTncn: any) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/salary/createOrUpdateCauHinhThueTncn';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        CauHinhThueTncn: CauHinhThueTncn
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  deleteCauHinhThueTncn(CauHinhThueTncnId: number) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/salary/deleteCauHinhThueTncn";
    return new Promise((resolve, rejecr) => {
      return this.httpClient.post(url, {
        CauHinhThueTncnId: CauHinhThueTncnId,
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getChamCongOtByEmpId(tuNgay: any, denNgay: any, employeeId: any) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/salary/getChamCongOtByEmpId';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        tuNgay: tuNgay,
        denNgay: denNgay,
        employeeId: employeeId,
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  createOrUpdateChamCongOt(EmployeeId: string, LoaiOtId: string, NgayChamCong: Date, Type: string, ThoiGian: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/salary/createOrUpdateChamCongOt';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        EmployeeId: EmployeeId,
        LoaiOtId: LoaiOtId,
        NgayChamCong: NgayChamCong,
        Type: Type,
        ThoiGian: ThoiGian
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getMasterDataTroCap() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/salary/getMasterDataTroCap';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  createOrUpdateTroCap(TroCap: any) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/salary/createOrUpdateTroCap';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        TroCap
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  deleteTroCap(TypeId: number, ObjectId: number) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/salary/deleteTroCap';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        TypeId,
        ObjectId
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getListKyLuong(tenKyLuong: string, listTrangThai: Array<number>) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/salary/getListKyLuong';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        tenKyLuong,
        listTrangThai
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  createOrUpdateKyLuong(kyLuong: any) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/salary/createOrUpdateKyLuong';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        kyLuong,
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  deleteKyLuong(KyLuongId: number) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/salary/deleteKyLuong';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        KyLuongId,
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  datVeMoiKyLuong(KyLuongId: number) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/salary/datVeMoiKyLuong';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        KyLuongId,
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }
  
  getKyLuongById(KyLuongId: number) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/salary/getKyLuongById';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        KyLuongId,
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  saveThuNhapTinhThue(ListLuongCtThuNhapTinhThue: any) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/salary/saveThuNhapTinhThue';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        ListLuongCtThuNhapTinhThue,
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  saveBaoHiem(ListLuongCtBaoHiem: any) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/salary/saveBaoHiem';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        ListLuongCtBaoHiem,
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  saveGiamTruTruocThue(ListLuongCtGiamTruTruocThue: any) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/salary/saveGiamTruTruocThue';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        ListLuongCtGiamTruTruocThue,
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  saveGiamTruSauThue(ListLuongCtGiamTruSauThue: any) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/salary/saveGiamTruSauThue';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        ListLuongCtGiamTruSauThue,
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  saveHoanLaiSauThue(ListLuongCtHoanLaiSauThue: any) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/salary/saveHoanLaiSauThue';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        ListLuongCtHoanLaiSauThue,
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  saveCtyDong(ListLuongCtCtyDong: any) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/salary/saveCtyDong';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        ListLuongCtCtyDong,
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  saveOther(ListLuongCtOther: any) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/salary/saveOther';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        ListLuongCtOther,
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getListDieuKienTroCapCoDinh(LuongCtLoaiTroCapCoDinhId: number) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/salary/getListDieuKienTroCapCoDinh';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        LuongCtLoaiTroCapCoDinhId,
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  updateDieuKienTroCapCoDinh(LuongCtDieuKienTroCapCoDinh: any) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/salary/updateDieuKienTroCapCoDinh';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        LuongCtDieuKienTroCapCoDinh,
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  saveLuongCtTroCapKhac(KyLuongId: number, ListLoaiTroCapKhacId: Array<string>) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/salary/saveLuongCtTroCapKhac';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        KyLuongId,
        ListLoaiTroCapKhacId
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getListDieuKienTroCapKhac(LuongCtLoaiTroCapKhacId: number) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/salary/getListDieuKienTroCapKhac';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        LuongCtLoaiTroCapKhacId,
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  updateDieuKienTroCapKhac(EmployeeId: string, LuongCtDieuKienTroCapKhac: any) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/salary/updateDieuKienTroCapKhac';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        EmployeeId,
        LuongCtDieuKienTroCapKhac,
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  updateMucTroCapKhac(MucTroCap: number, LuongCtLoaiTroCapKhacId: number) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/salary/updateMucTroCapKhac';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        MucTroCap,
        LuongCtLoaiTroCapKhacId,
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  importTroCapKhac(ListData: any, KyLuongId: number) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/salary/importTroCapKhac';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        ListData,
        KyLuongId,
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  importLuongChiTiet(ListData: any) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/salary/importLuongChiTiet';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        ListData,
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  hoanThanhKyLuong(KyLuongId: number) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/salary/hoanThanhKyLuong';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        KyLuongId,
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getListPhieuLuong(ListKyLuongId: any, ListEmployeeId: any) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/salary/getListPhieuLuong';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        ListKyLuongId,
        ListEmployeeId
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getPhieuLuongById(PhieuLuongId: number) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/salary/getPhieuLuongById';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        PhieuLuongId,
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  exportBaoCaoKyLuong(KyLuongId: number) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/salary/exportBaoCaoKyLuong';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        KyLuongId,
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  capNhatBangLuong(KyLuongId: number) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/salary/capNhatBangLuong';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        KyLuongId,
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  exportExcelKyLuongMealAllowance(number: any) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/salary/exportExcelKyLuongMealAllowance';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        number: number
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }
}
