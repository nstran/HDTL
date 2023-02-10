export class KeHoachOtModel {
  KeHoachOtId: number;
  LoaiOtId: string;
  TenKeHoach: string;
  NgayDeXuat: Date;
  NguoiDeXuatId: string;
  DiaDiem: string;
  NgayBatDau: Date;
  NgayKetThuc: Date;
  GioBatDau: Date;
  GioKetThuc: Date;
  LyDo: string;
  GhiChu: string;
  TrangThai: number;
  Active: boolean;
  HanDangKy: Date;
  HanPheDuyetKeHoach: Date;
  HanPheDuyetDangKy: Date;
  LoaiCaId: number;
}

export class NoteDocument {
  active: boolean;
  base64Url: string;
  createdById: string;
  createdDate: Date;
  documentName: string;
  documentSize: string;
  documentUrl: string;
  noteDocumentId: string;
  noteId: string;
  updatedById: string;
  updatedDate: Date;
}

export class Note {
  active: boolean;
  createdById: string;
  createdDate: Date;
  description: string;
  noteDocList: Array<NoteDocument>;
  noteId: string;
  noteTitle: string;
  objectId: string;
  objectType: string;
  responsibleAvatar: string;
  responsibleName: string;
  type: string;
  updatedById: string;
  updatedDate: Date;
}

export class FileInFolder {
  fileInFolderId: string;
  folderId: string;
  fileFullName: string;
  fileName: string;
  objectId: string;
  fileUrl: string;
  objectType: string;
  size: string;
  active: boolean;
  fileExtension: string;
  createdById: string;
  createdDate: Date;
  uploadByName: string;
}

export class FileUploadModel {
  FileInFolder: FileInFolder;
  FileSave: File;
}

export class FileNameExists {
  oldFileName: string;
  newFileName: string;
}
