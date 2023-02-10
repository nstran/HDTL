import { Gunzip } from "zlib";
import { FileUpload } from 'primeng/fileupload';

export class employeeInterface {
  employeeId: number;
  employeeName: string;
  employeeCode: string;
  positionId: string;
  positionName: string;
  organizationId: string;
  organizationName: string;
  dateOfBirth: Date;
  identity: string;  
  lyDo: string;  
  trangThai: number;
}
 
export class deXuatCongTacModel {
  deXuatCongTacId: number;
  tenDeXuat: string;
  loaiDeXuat: number;
  ngayDeXuat: Date;
  nguoiDeXuatId: string;
  positionId: string;
  organizationId: string;
  donVi: string
  diaDiem: string;
  phuongTien: string;
  nhiemVu: string;
  thoiGianBatDau: Date;
  thoiGianKetThuc: Date;  
  trangThaiDeXuat: number;  
  thoigian: string;
}
export class chiTietDeXuatCongTacModel {
  deXuatCongTacId: number;
  idEmployee: string;  
  tenNhanVien: string;
  maNhanVien: string;
  phongBan: string;
  chucVu: string;
  ngaySinh: Date;
  identity: string;
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

export class  Note {
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

export class  FileInFolder {
  fileInFolderId: string;
  folderId: string;  
  fileName: string;
  objectId: string;  
  objectType: string;
  objectNumber: number; 
  size: string;  
  active: boolean;
  fileExtension: string;
  createdById: string;
  createdByName: string;                      
  createdDate: Date;
  updatedById: string ;
  updatedDate: Date;
  fileUrl: string;
  fileFullName: string;
  uploadByName: string;
}

export class  FileUploadModel {
  FileInFolder: FileInFolder;
  FileSave: File;
}

export class  FileNameExists {
  oldFileName: string;
  newFileName: string;
}
export class NoteModel {
  NoteId: string;
  NoteTitle: string;
  Description: string;
  Type: string;
  ObjectId: string;
  ObjectNumber: number;
  ObjectType: string;
  Active: Boolean;
  CreatedById: string;
  CreatedDate: Date;
  UpdatedById: string;
  UpdatedDate: Date;
  constructor() { }
}
export class NoteDocumentModel {
  NoteDocumentId: string;
  NoteId: string;
  DocumentName: string;
  DocumentSize: string;
  DocumentUrl: string;
  DocumentType: string;
  Base64Url: string;
  Active: boolean;
  CreatedById: string;
  CreatedDate: Date;
  UpdatedById: string;
  UpdatedDate: Date;
  constructor() { 
      this.NoteDocumentId = '00000000-0000-0000-0000-000000000000',
      this.NoteId = '00000000-0000-0000-0000-000000000000',
      this.DocumentName = '',
      this.DocumentSize = '',
      this.DocumentUrl = '',
      this.DocumentType = '',
      this.Base64Url = '',
      this.Active = true,
      this.CreatedById = '00000000-0000-0000-0000-000000000000',
      this.CreatedDate = new Date(),
      this.UpdatedById = null,
      this.UpdatedDate = null
  }
}