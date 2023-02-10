import { share, map } from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { TaiSanModel } from '../models/taisan.model';
import { PhanBoTaiSanModel } from '../models/phanbotaisan.model';
import { ThuHoiTaiSanModel } from '../models/thu-hoi-tai-san.model';
import { YeuCauCapPhatChiTietModel } from '../models/yeu-cau-cap-phat-chi-tiet';
import { YeuCauCapPhatModel } from '../models/yeu-cau-cap-phat';

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
export class AssetService {
  constructor(private httpClient: HttpClient) { }
  refreshLead: boolean = false;
  userId: string = JSON.parse(localStorage.getItem("auth")).UserId;

  // getMasterDataAssetForm() {
  //   const url = localStorage.getItem('ApiEndPoint') + '/api/asset/getMasterDataAssetForm';
  //   return this.httpClient.post(url, {
  //   }).pipe(map((response: Response) => {
  //     return response;
  //   }));
  // }

  getMasterDataAssetForm() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/asset/getMasterDataAssetForm';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  createOrUpdateAsset(taiSan: TaiSanModel, userId: string, isQuick: boolean = false) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/asset/createOrUpdateAsset';
    return this.httpClient.post(url,
      {
        TaiSan: taiSan,
        UserId: userId,
        IsQuick: isQuick
      }).pipe(map((response: Response) => {
        return response;
      }));
  }

  deleteAsset(assetId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/asset/deleteAsset';
    return this.httpClient.post(url, { TaiSanId: assetId }).pipe(
      map((response: Response) => {
        return <any>response;
      }));
  }

  getAllAssetList() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/asset/getAllAssetList';
    return this.httpClient.post(url, {}).pipe(
      map((response: Response) => {
        return <any>response;
      }));
  }

  searchAssetAsync(assetName: string, assetCode: string, listTrangThai: Array<number>, listEmpId: Array<string>, listLoaiTSId: Array<string>, ngayTinhKH: any, khuVucId: any) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/asset/getAllAssetList";
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        MaTaiSan: assetName,
        TenTaiSan: assetCode,
        ListTrangThai: listTrangThai,
        ListEmployee: listEmpId,
        ListLoaiTS: listLoaiTSId,
        NgayTinhKhauHao: ngayTinhKH,
        ListProvinceId: khuVucId
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  taoPhanBoTaiSan(lstPhanBoModel: any, userId: string) {

    const url = localStorage.getItem('ApiEndPoint') + '/api/asset/taoPhanBoTaiSan';
    return this.httpClient.post(url,
      {
        ListPhanBo: lstPhanBoModel,
        UserId: userId,
      }).pipe(map((response: Response) => {
        return response;
      }));
  }

  getMasterDataPhanBoTSForm() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/asset/getMasterDataPhanBoTSForm';
    return this.httpClient.post(url, {
    }).pipe(map((response: Response) => {
      return response;
    }));
  }

  getDataAssetDetail(taiSanId: number, userId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/asset/getDataAssetDetail';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        TaiSanId: taiSanId,
        UserId: userId
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getMasterDataThuHoiTSForm() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/asset/getMasterDataThuHoiTSForm';
    return this.httpClient.post(url, {
    }).pipe(map((response: Response) => {
      return response;
    }));
  }

  taoThuHoiTaiSan(lstThuHoiModel: any, userId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/asset/taoThuHoiTaiSan';
    return this.httpClient.post(url,
      {
        ListPhanBo: lstThuHoiModel, UserId: userId
      }).pipe(map((response: Response) => {
        return response;
      }));
  }

  downloadTemplateAsset(phanLoai: number) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/asset/downloadTemplateAsset';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        PhanLoai: phanLoai
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  deleteBaoDuong(baoDuongId: number) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/asset/deleteBaoDuong";
    return new Promise((resolve, rejecr) => {
      return this.httpClient.post(url, {
        BaoDuongId: baoDuongId,
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  createOrUpdateBaoDuong(baoDuong: any) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/asset/createOrUpdateBaoDuong";
    return new Promise((resolve, rejecr) => {
      return this.httpClient.post(url, {
        BaoDuong: baoDuong,
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

  //#region YÊU CẦU CẤP PHÁT
  createOrYeuCauCapPhat(yeuCau: YeuCauCapPhatModel, lstYeuCauChiTiet: Array<YeuCauCapPhatChiTietModel>, folderType: string, listDocumentFile: Array<FileUploadModel>, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/asset/createOrYeuCauCapPhat';
    let formData: FormData = new FormData();

    formData.append("UserId", userId);
    formData.append("FolderType", folderType);
    formData.append('YeuCauCapPhatTaiSan[YeuCauCapPhatTaiSanId]', yeuCau.yeuCauCapPhatTaiSanId.toString());
    formData.append('YeuCauCapPhatTaiSan[MaYeuCau]', yeuCau.maYeuCau);
    formData.append('YeuCauCapPhatTaiSan[NgayDeXuat]', yeuCau.ngayDeXuat == null ? null : yeuCau.ngayDeXuat.toUTCString());
    formData.append('YeuCauCapPhatTaiSan[NguoiDeXuatId]', yeuCau.nguoiDeXuatId);
    formData.append('YeuCauCapPhatTaiSan[TrangThai]', yeuCau.trangThai.toString());
    formData.append('YeuCauCapPhatTaiSan[CreatedById]', yeuCau.createdById);
    formData.append('YeuCauCapPhatTaiSan[CreatedDate]', yeuCau.createdDate == null ? null : yeuCau.createdDate.toUTCString());

    var index = 0;
    for (var chiTiet of lstYeuCauChiTiet) {
      formData.append("ListYeuCauCapPhatTaiSanChiTiet[" + index + "].YeuCauCapPhatTaiSanChiTietId", chiTiet.yeuCauCapPhatTaiSanChiTietId.toString());
      formData.append("ListYeuCauCapPhatTaiSanChiTiet[" + index + "].TaiSanId", chiTiet.taiSanId.toString());
      formData.append("ListYeuCauCapPhatTaiSanChiTiet[" + index + "].YeuCauCapPhatTaiSanId", chiTiet.yeuCauCapPhatTaiSanId.toString());
      formData.append("ListYeuCauCapPhatTaiSanChiTiet[" + index + "].LoaiTaiSanId", chiTiet.loaiTaiSanId.toString());
      formData.append("ListYeuCauCapPhatTaiSanChiTiet[" + index + "].MoTa", chiTiet.moTa == null ? "" : chiTiet.moTa);
      formData.append("ListYeuCauCapPhatTaiSanChiTiet[" + index + "].SoLuong", chiTiet.soLuong.toString());
      formData.append("ListYeuCauCapPhatTaiSanChiTiet[" + index + "].SoLuongPheDuyet", chiTiet.soLuongPheDuyet.toString());
      formData.append("ListYeuCauCapPhatTaiSanChiTiet[" + index + "].NhanVienYeuCauId", chiTiet.nhanVienYeuCauId.toString());
      formData.append("ListYeuCauCapPhatTaiSanChiTiet[" + index + "].MucDichSuDungId", chiTiet.mucDichSuDungId.toString());
      formData.append("ListYeuCauCapPhatTaiSanChiTiet[" + index + "].NgayBatDau", chiTiet.ngayBatDau == null ? null : chiTiet.ngayBatDau.toUTCString());
      formData.append("ListYeuCauCapPhatTaiSanChiTiet[" + index + "].NgayKetThuc", chiTiet.ngayKetThuc == null ? null : chiTiet.ngayKetThuc.toUTCString());

      formData.append("ListYeuCauCapPhatTaiSanChiTiet[" + index + "].LyDo", chiTiet.lyDo == null ? "" : chiTiet.lyDo);
      formData.append("ListYeuCauCapPhatTaiSanChiTiet[" + index + "].TrangThai", chiTiet.trangThai == null ? '0' : chiTiet.trangThai.toString());
      formData.append("ListYeuCauCapPhatTaiSanChiTiet[" + index + "].CreatedById", chiTiet.createdById);
      formData.append("ListYeuCauCapPhatTaiSanChiTiet[" + index + "].CreatedDate", chiTiet.createdDate == null ? null : chiTiet.createdDate.toUTCString());
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

    return this.httpClient.post(url, formData).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getAllYeuCauCapPhatTSList() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/asset/getAllYeuCauCapPhatTSList';
    return this.httpClient.post(url, {}).pipe(
      map((response: Response) => {
        return <any>response;
      }));
  }
  getMasterDataYeuCauCapPhatForm() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/asset/getMasterDataYeuCauCapPhatForm';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  searchYeuCauCapPhatAsync(maYC: string, listEmpId: Array<string>, trangThai: number) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/asset/getAllYeuCauCapPhatTSList";
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        MaYeuCau: maYC,
        ListEmployee: listEmpId,
        TrangThai: trangThai,
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  xoaYeuCauCapPhat(yeuCauId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/asset/xoaYeuCauCapPhat';
    return this.httpClient.post(url, { YeuCauCapPhatTaiSanId: yeuCauId }).pipe(
      map((response: Response) => {
        return <any>response;
      }));
  }

  getDataYeuCauCapPhatDetail(yeuCauId: number, userId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/asset/getDataYeuCauCapPhatDetail';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        YeuCauCapPhatTaiSanId: yeuCauId,
        UserId: userId
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  deleteChiTietYeuCauCapPhat(id: number) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/asset/deleteChiTietYeuCauCapPhat";
    return new Promise((resolve, rejecr) => {
      return this.httpClient.post(url, {
        YeuCauCapPhatTaiSanChiTietId: id,
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  createOrUpdateChiTietYeuCauCapPhat(chiTiet: any, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/asset/createOrUpdateChiTietYeuCauCapPhat";
    return new Promise((resolve, rejecr) => {
      return this.httpClient.post(url, {
        YeuCauCapPhatTaiSanChiTiet: chiTiet,
        UserId: userId
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  datVeMoiYeuCauCapPhatTS(yeuCauId: number) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/asset/datVeMoiYeuCauCapPhatTS';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        YeuCauCapPhatTaiSanId: yeuCauId
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  capNhapTrangThaiYeuCauCapPhat(yeuCauId: number, type: number) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/asset/capNhapTrangThaiYeuCauCapPhat';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        YeuCauCapPhatTaiSanId: yeuCauId,
        Type: type
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }
  baoCaoPhanBo(listEmployeeId: Array<string>, listOrganizationId: Array<string>, listPhanLoaiTaiSanId: Array<string>, listHienTrangTaiSan: Array<number>,
  ) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/asset/baoCaoPhanBo";
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        ListEmployeeId: listEmployeeId,
        ListOrganizationId: listOrganizationId,
        ListPhanLoaiTaiSanId: listPhanLoaiTaiSanId,
        ListHienTrangTaiSan: listHienTrangTaiSan,
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }
  baoCaoKhauHao(thoigianKhauHaoDen: Date, listPhanLoaiTaiSanId: Array<string>, listHienTrangTaiSan: Array<number>,
  ) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/asset/baoCaoKhauHao";
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        ThoiGianKhauHaoDen: thoigianKhauHaoDen,
        ListPhanLoaiTaiSanId: listPhanLoaiTaiSanId,
        ListHienTrangTaiSan: listHienTrangTaiSan,
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getMasterDataBaoCaoPhanBo() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/asset/getMasterDataBaoCaoPhanBo';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getMasterImportAsset() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/asset/getMasterImportAsset';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {}).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  importAsset(listAsset) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/asset/importAsset';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        ListTaiSan:listAsset,
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  downloadTemplateImportAsset() {
    let url = localStorage.getItem('ApiEndPoint') + '/api/asset/downloadTemplateImportAsset';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url,{}).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  taoDotKiemKe(TenDotKiemKe: string, NgayBatDau: any, NgayKetThuc: any, dotKiemKeId = null) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/asset/taoDotKiemKe';
    return this.httpClient.post(url, { 
      TenDotKiemKe: TenDotKiemKe,
      NgayBatDau: NgayBatDau,
      NgayKetThuc: NgayKetThuc,
      DotKiemKeId: dotKiemKeId,
    }).pipe(
      map((response: Response) => {
        return <any>response;
      }));
  }

  dotKiemKeSearch(TenDotKiemKe: string, NgayBatDau: any, NgayKetThuc: any, listTrangThai : any) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/asset/dotKiemKeSearch';
    return this.httpClient.post(url, { 
      TenDotKiemKe: TenDotKiemKe,
      NgayBatDau: NgayBatDau,
      NgayKetThuc: NgayKetThuc,
      ListTrangThai: listTrangThai,
    }).pipe(
      map((response: Response) => {
        return <any>response;
      }));
  }

  
  deleteDotKiemKe(dotKiemKeId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/asset/deleteDotKiemKe';
    return this.httpClient.post(url, { DotKiemKeId: dotKiemKeId }).pipe(
      map((response: Response) => {
        return <any>response;
      }));
  }

  
  dotKiemKeDetail(dotKiemKeId, provincecId, phanLoaiTaiSanId, hienTrangTaiSan, ngayKiemKe, nguoiKiemKeId) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/asset/dotKiemKeDetail';
    return this.httpClient.post(url, { 
      DotKiemKeId: dotKiemKeId,
      ProvincecId: provincecId,
      PhanLoaiTaiSanId: phanLoaiTaiSanId,
      HienTrangTaiSan: hienTrangTaiSan,
      NgayKiemKe: ngayKiemKe,
      NguoiKiemKeId: nguoiKiemKeId,
    }).pipe(
      map((response: Response) => {
        return <any>response;
      }));
  }
}
