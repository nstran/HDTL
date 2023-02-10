
import { map } from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { HoSoCongTacModel } from '../../models/ho-so-cong-tac';
import { DeNghiTamHoanUngModel } from '../../models/de-nghi-tam-hoan-ung';
import { DeXuatCongTacModel } from '../../models/de-xuat-cong-tac';
import { DeXuatCongTacChiTietModel } from '../../models/de-xuat-cong-tac-chi-tiet';

class FileUploadModel {
  FileInFolder: FileInFolder;
  FileSave: File;
}

class FileInFolder {
  fileInFolderId: string;
  folderId: string;
  fileName: string;
  objectId: string;
  objectNumber: number;
  objectType: string;
  size: string;
  active: boolean;
  fileExtension: string;
  createdById: string;
  createdDate: Date;
}

@Injectable()
export class QuanLyCongTacService {

  constructor(private httpClient: HttpClient) { }
  userId: string = JSON.parse(localStorage.getItem("auth")).UserId;

  getMasterDataHoSoCTForm() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/getMasterDataHoSoCTForm';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  createOrUpdateHoSoCT(hoSoCT: HoSoCongTacModel, folderType: string, listDocumentFile: Array<FileUploadModel>, userId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/createOrUpdateHoSoCT';
    let formData: FormData = new FormData();

    formData.append("UserId", userId);
    formData.append("FolderType", folderType);
    formData.append('HoSoCongTac[HoSoCongTacId]', hoSoCT.hoSoCongTacId.toString());
    formData.append('HoSoCongTac[DeXuatCongTacId]', hoSoCT.deXuatCongTacId.toString());
    formData.append('HoSoCongTac[KetQua]', hoSoCT.ketQuaCT.toString());

    var indexFile = 0;
    for (var file of listDocumentFile) {
      formData.append("ListFile[" + indexFile + "].FileInFolder.FileInFolderId", file.FileInFolder.fileInFolderId);
      formData.append("ListFile[" + indexFile + "].FileInFolder.FolderId", file.FileInFolder.folderId);
      formData.append("ListFile[" + indexFile + "].FileInFolder.FileName", file.FileInFolder.fileName);
      formData.append("ListFile[" + indexFile + "].FileInFolder.ObjectNumber", file.FileInFolder.objectNumber.toString());
      formData.append("ListFile[" + indexFile + "].FileInFolder.ObjectType", file.FileInFolder.objectType);
      formData.append("ListFile[" + indexFile + "].FileInFolder.Size", file.FileInFolder.size);
      formData.append("ListFile[" + indexFile + "].FileInFolder.FileExtension", file.FileInFolder.fileExtension);
      formData.append("ListFile[" + indexFile + "].FileSave", file.FileSave);
      indexFile++;
    }

    return this.httpClient.post(url, formData).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  xoaHoSoCongTac(hoSoCongTacId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/xoaHoSoCongTac';
    return this.httpClient.post(url, { HoSoCongTacId: hoSoCongTacId }).pipe(
      map((response: Response) => {
        return <any>response;
      }));
  }

  searchHoSoCongTac(hoSo: string = null, listEmpId: Array<string> = [], trangThai: number = 0) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/employee/getAllHoSoCongTacList";
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        MaHoSo: hoSo,
        ListEmployee: listEmpId,
        TrangThai: trangThai,
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getAllHoSoCongTacList() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/getAllHoSoCongTacList';
    return this.httpClient.post(url, {}).pipe(
      map((response: Response) => {
        return <any>response;
      }));
  }

  getDataHoSoCTDetail(hoSoId: number, userId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/getDataHoSoCTDetail';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        Id: hoSoId,
        UserId: userId
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  uploadFile(folderType: string, listFile: Array<FileUploadModel>, objectNumber: number = 0) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/asset/uploadFile';

    let formData: FormData = new FormData();
    formData.append("FolderType", folderType);
    formData.append("ObjectNumber", objectNumber.toString());
    formData.append("UserId", this.userId);
    var index = 0;
    for (var pair of listFile) {
      formData.append("ListFile[" + index + "].FileInFolder.FileInFolderId", pair.FileInFolder.fileInFolderId);
      formData.append("ListFile[" + index + "].FileInFolder.FolderId", pair.FileInFolder.folderId);
      formData.append("ListFile[" + index + "].FileInFolder.FileName", pair.FileInFolder.fileName);
      formData.append("ListFile[" + index + "].FileInFolder.ObjectId", pair.FileInFolder.objectId);
      formData.append("ListFile[" + index + "].FileInFolder.ObjectType", pair.FileInFolder.objectType);
      formData.append("ListFile[" + index + "].FileInFolder.Size", pair.FileInFolder.size);
      formData.append("ListFile[" + index + "].FileInFolder.FileExtension", pair.FileInFolder.fileExtension);
      formData.append("ListFile[" + index + "].FileSave", pair.FileSave);
      index++;
    }
    return this.httpClient.post(url,
      formData
    ).pipe(map((response: Response) => {
      return response;
    }));
  }

  deleteFile(fileInFolderId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/contract/deleteFile';
    return this.httpClient.post(url, {
      FileInFolderId: fileInFolderId
    }).pipe(map((response: Response) => {
      return response;
    }));
  }

  createOrUpdateDeNghiTamHoanUng(deNghi: DeNghiTamHoanUngModel, listNoiDungTT: Array<any>, folderType: string, listDocumentFile: Array<FileUploadModel>, userId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/createOrUpdateDeNghiTamHoanUng';
    let formData: FormData = new FormData();

    formData.append("UserId", userId);
    formData.append("FolderType", folderType);
    formData.append('DeNghiTamHoanUng[DeNghiTamHoanUngId]', deNghi.deNghiTamHoanUngId.toString());
    formData.append('DeNghiTamHoanUng[HoSoCongTacId]', deNghi.hoSoCongTacId.toString());
    formData.append('DeNghiTamHoanUng[NgayDeNghi]', deNghi.ngayDeNghi == null ? null : deNghi.ngayDeNghi.toUTCString());
    formData.append('DeNghiTamHoanUng[LyDo]', deNghi.lyDo.toString());
    formData.append('DeNghiTamHoanUng[TienTamUng]', deNghi.tienTamUng.toString());
    formData.append('DeNghiTamHoanUng[TongTienThanhToan]', deNghi.tongTienThanhToan.toString());
    formData.append('DeNghiTamHoanUng[NguoiThuHuongId]', deNghi.nguoiThuHuongId.toString());
    formData.append('DeNghiTamHoanUng[LoaiDeNghi]', deNghi.loaiDeNghi.toString());

    var index = 0;
    // Hoàn ứng
    if (deNghi.loaiDeNghi == 1) {
      for (var chiTiet of listNoiDungTT) {
        formData.append("ListNoiDungTT[" + index + "].DeNghiTamHoanUngChiTietId", '');
        formData.append("ListNoiDungTT[" + index + "].DeNghiTamHoanUngId", deNghi.deNghiTamHoanUngId.toString());
        formData.append("ListNoiDungTT[" + index + "].NgayThang", chiTiet.ngayThang == null ? null : chiTiet.ngayThang.toUTCString());
        formData.append("ListNoiDungTT[" + index + "].SoHoaDon", chiTiet.soHoaDon);
        formData.append("ListNoiDungTT[" + index + "].NoiDung", chiTiet.noiDung);
        formData.append("ListNoiDungTT[" + index + "].HinhThucThanhToan", chiTiet.hinhThucThanhToan);
        formData.append("ListNoiDungTT[" + index + "].VanChuyenXm", chiTiet.vanChuyenXm.toString());
        formData.append("ListNoiDungTT[" + index + "].TienDonHnnb", chiTiet.tienDonHnnb.toString());
        formData.append("ListNoiDungTT[" + index + "].TienDonDn", chiTiet.tienDonDn.toString());
        formData.append("ListNoiDungTT[" + index + "].KhachSan", chiTiet.khachSan.toString());
        formData.append("ListNoiDungTT[" + index + "].ChiPhiKhac", chiTiet.chiPhiKhac.toString());
        formData.append("ListNoiDungTT[" + index + "].GhiChu", chiTiet.ghiChu);
        formData.append("ListNoiDungTT[" + index + "].Vat", chiTiet.vat.toString());
        formData.append("ListNoiDungTT[" + index + "].DinhKemCt", chiTiet.dinhKemCt);
        index++;
      }
    }
    // Tạm ứng
    else {
      for (var chiTiet of listNoiDungTT) {
        formData.append("ListNoiDungTT[" + index + "].DeNghiTamHoanUngChiTietId", chiTiet.id.toString());
        formData.append("ListNoiDungTT[" + index + "].DeNghiTamHoanUngId", '');
        formData.append("ListNoiDungTT[" + index + "].NoiDung", chiTiet.noiDung);
        formData.append("ListNoiDungTT[" + index + "].TongTienTruocVAT", chiTiet.tongTienTruocVat.toString());
        formData.append("ListNoiDungTT[" + index + "].Vat", chiTiet.vat.toString());
        formData.append("ListNoiDungTT[" + index + "].TienSauVat", chiTiet.tienSauVat.toString());
        formData.append("ListNoiDungTT[" + index + "].ParentId", chiTiet.parentId == null ? null : chiTiet.parentId.toString());
        formData.append("ListNoiDungTT[" + index + "].Level", chiTiet.level.toString());
        index++;
      }
    }

    var indexFile = 0;
    for (var file of listDocumentFile) {
      formData.append("ListFile[" + indexFile + "].FileInFolder.FileInFolderId", file.FileInFolder.fileInFolderId);
      formData.append("ListFile[" + indexFile + "].FileInFolder.FolderId", file.FileInFolder.folderId);
      formData.append("ListFile[" + indexFile + "].FileInFolder.FileName", file.FileInFolder.fileName);
      formData.append("ListFile[" + indexFile + "].FileInFolder.ObjectNumber", file.FileInFolder.objectNumber.toString());
      formData.append("ListFile[" + indexFile + "].FileInFolder.ObjectType", file.FileInFolder.objectType);
      formData.append("ListFile[" + indexFile + "].FileInFolder.Size", file.FileInFolder.size);
      formData.append("ListFile[" + indexFile + "].FileInFolder.FileExtension", file.FileInFolder.fileExtension);
      formData.append("ListFile[" + indexFile + "].FileSave", file.FileSave);
      indexFile++;
    }

    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, formData).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getMasterDataDeNghiForm() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/getMasterDataDeNghiForm';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getDataDeNghiDetailForm(deNghiId: number) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/getDataDeNghiDetailForm';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        Id: deNghiId
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  updateOrCreateDeNghiTamHoanUngChiTiet(rowData: any, userId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/updateDeNghiTamHoanUngChiTiet';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        DeNghiTamHoanUngChiTiet: rowData,
        UserId: userId
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  deleteDeNghiTamHoanUngChiTiet(deNghiCTId: number) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/employee/deleteDeNghiTamHoanUngChiTiet";
    return new Promise((resolve, rejecr) => {
      return this.httpClient.post(url, {
        DeNghiTamHoanUngChiTietId: deNghiCTId,
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  datVeMoiDeNghiTamHoanUng(deNghiId: number, DoiTuongApDung: number) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/datVeMoiDeNghiTamHoanUng';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        DeNghiTamHoanUngId: deNghiId,
        DoiTuongApDung: DoiTuongApDung
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  xoaDeNghiTamHoanUng(deNghiTamHoanUngId: number) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/xoaDeNghiTamHoanUng';
    return this.httpClient.post(url, { DeNghiTamHoanUngId: deNghiTamHoanUngId }).pipe(
      map((response: Response) => {
        return <any>response;
      }));
  }

  getAllDenghiTamUngList(loaiDeNghi: number) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/getAllDenghiTamUngList';
    return this.httpClient.post(url, {
      LoaiDeNghi: loaiDeNghi
    }).pipe(
      map((response: Response) => {
        return <any>response;
      }));
  }

  searchDeNghiTamUng(maDenghi: string = null, listEmpId: Array<string> = [], trangThai: number = 0, loaiDeNghi: number = 0) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/employee/getAllDenghiTamUngList";
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        MaDenghi: maDenghi,
        ListEmployee: listEmpId,
        TrangThai: trangThai,
        LoaiDeNghi: loaiDeNghi
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  createOrUpdateDeXuatCT(dexuatCT: DeXuatCongTacModel, lstDeXuatChiTiet: Array<DeXuatCongTacChiTietModel>, folderType: string, listDocumentFile: Array<FileUploadModel>, userId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/createOrUpdateDeXuatCT';
    let formData: FormData = new FormData();
    formData.append("UserId", userId);
    formData.append("FolderType", folderType);
    formData.append('DeXuatCongTac[DeXuatCongTacId]', dexuatCT.deXuatCongTacId.toString());
    formData.append('DeXuatCongTac[TenDeXuat]', dexuatCT.tenDeXuat == null ? '' : dexuatCT.tenDeXuat);
    formData.append('DeXuatCongTac[NguoiDeXuatId]', dexuatCT.nguoiDeXuatId.toString());
    formData.append('DeXuatCongTac[NgayDeXuat]', dexuatCT.ngayDeXuat == null ? null : dexuatCT.ngayDeXuat.toUTCString());
    formData.append('DeXuatCongTac[DonVi]', dexuatCT.donVi == null ? '' : dexuatCT.donVi);
    formData.append('DeXuatCongTac[DiaDiem]', dexuatCT.diaDiem == null ? '' : dexuatCT.diaDiem);
    formData.append('DeXuatCongTac[PhuongTien]', dexuatCT.phuongTien == null ? '' : dexuatCT.phuongTien);
    formData.append('DeXuatCongTac[NhiemVu]', dexuatCT.nhiemVu == null ? '' : dexuatCT.nhiemVu);
    formData.append('DeXuatCongTac[ThoiGianBatDau]', dexuatCT.thoiGianBatDau == null ? null : dexuatCT.thoiGianBatDau.toUTCString());
    formData.append('DeXuatCongTac[ThoiGianKetThuc]', dexuatCT.thoiGianKetThuc == null ? null : dexuatCT.thoiGianKetThuc.toUTCString());

    var index = 0;
    for (var chiTiet of lstDeXuatChiTiet) {
      formData.append("ListChiTietDeXuatCongTac[" + index + "].ChiTietDeXuatCongTacId", chiTiet.chiTietDeXuatCongTacId.toString());
      formData.append("ListChiTietDeXuatCongTac[" + index + "].DeXuatCongTacId", chiTiet.deXuatCongTacId.toString());
      formData.append("ListChiTietDeXuatCongTac[" + index + "].EmployeeId", chiTiet.employeeId.toString());
      formData.append("ListChiTietDeXuatCongTac[" + index + "].LyDo", chiTiet.lyDo == null ? "" : chiTiet.lyDo);
      formData.append("ListChiTietDeXuatCongTac[" + index + "].CreatedById", chiTiet.createdById);
      formData.append("ListChiTietDeXuatCongTac[" + index + "].CreatedDate", chiTiet.createdDate == null ? null : chiTiet.createdDate.toUTCString());
      index++;
    }

    var indexFile = 0;
    for (var file of listDocumentFile) {
      formData.append("ListFile[" + indexFile + "].FileInFolder.FileInFolderId", file.FileInFolder.fileInFolderId);
      formData.append("ListFile[" + indexFile + "].FileInFolder.FolderId", file.FileInFolder.folderId);
      formData.append("ListFile[" + indexFile + "].FileInFolder.FileName", file.FileInFolder.fileName);
      formData.append("ListFile[" + indexFile + "].FileInFolder.ObjectNumber", file.FileInFolder.objectNumber.toString());
      formData.append("ListFile[" + indexFile + "].FileInFolder.ObjectType", file.FileInFolder.objectType);
      formData.append("ListFile[" + indexFile + "].FileInFolder.Size", file.FileInFolder.size);
      formData.append("ListFile[" + indexFile + "].FileInFolder.FileExtension", file.FileInFolder.fileExtension);
      formData.append("ListFile[" + indexFile + "].FileSave", file.FileSave);
      indexFile++;
    }

    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, formData).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getAllDeXuatCongTac() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/getAllDeXuatCongTac';
    return this.httpClient.post(url, {}).pipe(
      map((response: Response) => {
        return <any>response;
      }));
  }

  searchDeXuatCongTacAsync(maDX: string, listEmpId: Array<string>, trangThai: number) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/employee/getAllDeXuatCongTac";
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        MaDeXuat: maDX,
        ListEmployee: listEmpId,
        TrangThai: trangThai,
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  xoaDeXuatCongTac(deXuatId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/xoaDeXuatCongTac';
    return this.httpClient.post(url, { DeXuatCongTacId: deXuatId }).pipe(
      map((response: Response) => {
        return <any>response;
      }));
  }

  getMasterDeXuatCongTacDetail(deXuatCongTacId: any) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/employee/getMasterDeXuatCongTacDetail";
    return new Promise((resolve, rejecr) => {
      return this.httpClient.post(url, {
        DeXuatCongTacId: deXuatCongTacId,
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  datVeMoiDeXuatCongTac(deXuatId: number) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/employee/datVeMoiDeXuatCongTac';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        DeXuatCongTacId: deXuatId
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  xoaDeXuatCongTacChiTiet(id: number) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/employee/xoaDeXuatCongTacChiTiet";
    return new Promise((resolve, rejecr) => {
      return this.httpClient.post(url, {
        ChiTietDeXuatCongTacId: id,
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  createOrUpdateChiTietDeXuatCongTac(chiTiet: any, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/employee/createOrUpdateChiTietDeXuatCongTac";
    return new Promise((resolve, rejecr) => {
      return this.httpClient.post(url, {
        DeXuatCongTacChiTiet: chiTiet,
        UserId: userId
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  hoanThanhHoSoCongTac(hoSoCongTacId: any) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/employee/hoanThanhHoSoCongTac";
    return new Promise((resolve, rejecr) => {
      return this.httpClient.post(url, {
        HoSoCongTacId: hoSoCongTacId,
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }
}
