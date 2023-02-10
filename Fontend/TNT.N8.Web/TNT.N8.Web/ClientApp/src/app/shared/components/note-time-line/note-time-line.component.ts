import { Component, OnInit, Input, ViewChild, ChangeDetectorRef, SimpleChanges } from '@angular/core';
import { FileUpload } from 'primeng';
import { MessageService, ConfirmationService } from 'primeng/api';
import { NoteDocumentModel } from '../../models/note-document.model';
import { NoteService } from '../../services/note.service';
import { ImageUploadService } from '../../services/imageupload.service';
import { DataService } from '../../services/data.service';
import { Paginator } from 'primeng/paginator';

class NoteModel {
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

class Note {
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
  constructor() {
    this.noteDocList = [];
  }
}

class NoteDocument {
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

class FileUploadModel {
  FileInFolder: FileInFolder;
  FileSave: File;
}

class FileInFolder {
  fileInFolderId: string;
  folderId: string;
  fileFullName: string;
  fileName: string;
  objectId: string;
  objectNumber: number;
  fileUrl: string;
  objectType: string;
  size: string;
  active: boolean;
  fileExtension: string;
  createdById: string;
  createdDate: Date;
  uploadByName: string;
}

@Component({
  selector: 'app-note-time-line',
  templateUrl: './note-time-line.component.html',
  styleUrls: ['./note-time-line.component.css']
})
export class NoteTimeLineComponent implements OnInit {
  @Input() viewNote: boolean = false;
  @Input() viewTimeline: boolean = true;
  @Input() objectType: string = null;
  @Input() objectId: string = null;
  @Input() objectNumber: number = null;
  @Input() actionAdd: boolean = false;
  @Input() actionEdit: boolean = false;
  @Input() actionDelete: boolean = false;
  @Input() refresh: number = 0;
  @Input() showPaginator: boolean = true;
  @Input() pageSize: number = 10;
  @Input() pageIndex: number = 0;
  @Input() isReportPoint: boolean = false;
  @Input() statusOrderAction: number = 1;

  loading: boolean = false;

  emptyGuid = '00000000-0000-0000-0000-000000000000';
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));

  @ViewChild('paginator') paginator: Paginator

  //Note
  noteHistory: Array<Note> = [];
  totalRecordsNote: number = 0;

  listNoteDocumentModel: Array<NoteDocumentModel> = [];
  listUpdateNoteDocument: Array<NoteDocument> = [];
  isAprovalQuote: boolean = false;
  noteId: string = null;
  noteContent: string = '';
  isEditNote: boolean = false;
  defaultAvatar: string = '/assets/images/no-avatar.png';
  uploadedNoteFiles: any[] = [];
  @ViewChild('fileNoteUpload') fileNoteUpload: FileUpload;
  defaultLimitedFileSize = Number(this.systemParameterList.find(systemParameter => systemParameter.systemKey == "LimitedFileSize").systemValueString) * 1024 * 1024;
  strAcceptFile: string = 'image video audio .zip .rar .pdf .xls .xlsx .doc .docx .ppt .pptx .txt';
  colsNoteFile = [];
  first: number = 0;

  constructor(
    private imageService: ImageUploadService,
    private noteService: NoteService,
    private messageService: MessageService,
    private ref: ChangeDetectorRef,
    private confirmationService: ConfirmationService,
    private dataService: DataService
  ) { }

  ngOnInit(): void {
    this.colsNoteFile = [
      { field: 'documentName', header: 'Tên tài liệu', width: '50%', textAlign: 'left', type: 'string' },
      { field: 'documentSize', header: 'Kích thước', width: '50%', textAlign: 'right', type: 'number' },
      { field: 'createdDate', header: 'Ngày tạo', width: '50%', textAlign: 'right', type: 'date' },
    ];

    this.dataService.currentMessage.subscribe(message => {
      this.getListNote();
      this.ref.detectChanges();
    });
  }

  /* Nếu Input có thay đổi */
  ngOnChanges(changes: SimpleChanges) {
    if (this.objectNumber || this.objectId) {
      this.getListNote();
      this.ref.detectChanges();
    }
  }

  async getListNote() {
    this.loading = true;
    let result: any = await this.noteService.getListNote(this.objectType, this.objectId, this.objectNumber, this.pageSize, this.pageIndex);
    this.loading = false;

    if (result.statusCode != 200) {
      this.showMessage('error', 'Lấy thông tin không thành công');
      return;
    }

    this.noteHistory = result.listNote;
    this.totalRecordsNote = result.totalRecordsNote;
  }

  paginate(event) {
    this.pageIndex = event.page;
    this.first = event.first;
    this.getListNote();
    this.ref.detectChanges();
  }

  async refreshPage(isEditNode) {
    //thêm mới Note thì reset về page 1
    if (!isEditNode) {
      this.pageIndex = 0;
      this.first = 0;
    }

    await this.getListNote();
    this.ref.detectChanges();
  }

  convertFileSize(size: string) {
    let tempSize = parseFloat(size);
    if (tempSize < 1024 * 1024) {
      return true;
    } else {
      return false;
    }
  }

  /*Kiểm tra noteText > 250 ký tự hoặc noteDocument > 3 thì ẩn đi một phần nội dung*/
  tooLong(note): boolean {
    if (note.noteDocList.length > 3) return true;
    var des = $.parseHTML(note.description);
    var count = 0;
    for (var i = 0; i < des.length; i++) {
      count += des[i].textContent.length;
      if (count > 250) return true;
    }
    return false;
  }

  /* Event thêm file dược chọn vào list file note */
  handleNoteFile(event, uploader: FileUpload) {
    for (let file of event.files) {
      let size: number = file.size;
      let type: string = file.type;

      if (size <= this.defaultLimitedFileSize) {
        if (type.indexOf('/') != -1) {
          type = type.slice(0, type.indexOf('/'));
        }
        if (this.strAcceptFile.includes(type) && type != "") {
          this.uploadedNoteFiles.push(file);
        } else {
          let subType = file.name.slice(file.name.lastIndexOf('.'));
          if (this.strAcceptFile.includes(subType)) {
            this.uploadedNoteFiles.push(file);
          }
        }
      }
    }
  }

  /*Event khi click xóa từng file */
  removeNoteFile(event) {
    let index = this.uploadedNoteFiles.indexOf(event.file);
    this.uploadedNoteFiles.splice(index, 1);
  }
  /*End*/

  /*Event khi click xóa toàn bộ file */
  clearAllNoteFile() {
    this.uploadedNoteFiles = [];
  }

  deleteNoteFile(rowData) {
    this.confirmationService.confirm({
      message: 'Bạn chắc chắn muốn xóa file này?',
      accept: async () => {
        this.loading = true;
        let result: any = await this.noteService.xoaFileGhiChu(rowData.noteDocumentId);
        this.loading = false;

        if (result.statusCode != 200) {
          this.showMessage('error', result.messageCode);
          return;
        }

        this.listUpdateNoteDocument = this.listUpdateNoteDocument.filter(x => x.noteDocumentId != rowData.noteDocumentId);
        this.showMessage('success', 'Xóa file thành công');
      }
    });
  }

  cancelNote() {
    this.confirmationService.confirm({
      message: 'Bạn có chắc muốn hủy ghi chú này?',
      accept: () => {
        this.noteId = null;
        this.noteContent = null;
        this.uploadedNoteFiles = [];
        if (this.fileNoteUpload) {
          this.fileNoteUpload.clear();
        }
        this.listUpdateNoteDocument = [];
        this.isEditNote = false;
      }
    });
  }

  /*Lưu file và ghi chú vào Db*/
  async saveNote() {
    if (
      (this.noteContent == null || this.noteContent.trim() == '') &&
      (this.uploadedNoteFiles == null || this.uploadedNoteFiles.length == 0)
    ) {
      this.showMessage('error', "Vui lòng nhập thông tin báo cáo!");
      return;
    }

    this.listNoteDocumentModel = [];

    let listFileUploadModel: Array<FileUploadModel> = [];
    this.uploadedNoteFiles.forEach(item => {
      let fileUpload: FileUploadModel = new FileUploadModel();

      fileUpload.FileSave = item;

      fileUpload.FileInFolder = new FileInFolder();

      fileUpload.FileInFolder.fileInFolderId = this.emptyGuid;
      fileUpload.FileInFolder.folderId = this.emptyGuid;

      fileUpload.FileInFolder.active = true;
      let index = item.name.lastIndexOf(".");
      let name = item.name.substring(0, index);
      fileUpload.FileInFolder.fileName = name;
      fileUpload.FileInFolder.fileExtension = item.name.substring(index + 1);
      fileUpload.FileInFolder.size = item.size;
      fileUpload.FileInFolder.objectType = this.objectType;
      fileUpload.FileInFolder.objectId = this.objectId ?? this.emptyGuid;
      fileUpload.FileInFolder.objectNumber = this.objectNumber;

      listFileUploadModel.push(fileUpload);
    });
    let noteModel = new NoteModel();

    /*Tạo mới ghi chú*/
    if (!this.noteId) {
      noteModel.NoteId = this.emptyGuid;
      noteModel.Description = this.noteContent != null ? this.noteContent.trim() : "";
      noteModel.Type = 'ADD';
      noteModel.ObjectId = this.objectId ?? this.emptyGuid;
      noteModel.ObjectNumber = this.objectNumber ?? null;
      noteModel.ObjectType = this.objectType;
      noteModel.NoteTitle = 'đã thêm ghi chú';
      noteModel.Active = true;
      noteModel.CreatedById = this.emptyGuid;
      noteModel.CreatedDate = new Date();
    }
    /*Update ghi chú*/
    else {
      noteModel.NoteId = this.noteId;
      noteModel.Description = this.noteContent != null ? this.noteContent.trim() : "";
      noteModel.Type = 'EDT';
      noteModel.ObjectId = this.objectId ?? this.emptyGuid;
      noteModel.ObjectNumber = this.objectNumber ?? null;
      noteModel.ObjectType = this.objectType;
      noteModel.NoteTitle = 'đã thêm cập nhật';
      noteModel.Active = true;
      noteModel.CreatedById = this.emptyGuid;
      noteModel.CreatedDate = new Date();
    }

    this.noteHistory = [];

    this.loading = true;
    let result: any = await this.noteService.themMoiGhiChu(noteModel, this.objectType, listFileUploadModel);
    this.loading = false;

    if (result.statusCode != 200) {
      this.showMessage('error', result.messageCode);
      return;
    }

    /*Reshow Time Line*/
    await this.refreshPage(this.isEditNote); //isEditNote:false - quay lại page 1

    this.uploadedNoteFiles = [];
    if (this.fileNoteUpload) {
      this.fileNoteUpload.clear();  //Xóa toàn bộ file trong control
    }
    this.noteContent = null;
    this.noteId = null;
    this.isEditNote = false;
    this.listUpdateNoteDocument = [];

    this.showMessage('success', result.messageCode);
  }

  /*Event Mở rộng/Thu gọn nội dung của ghi chú*/
  toggle_note_label: string = 'Mở rộng';
  trigger_node(nodeid: string, event) {
    // noteContent
    let shortcontent_ = $('#' + nodeid).find('.short-contents');
    let fullcontent_ = $('#' + nodeid).find('.full-contents');
    if (shortcontent_.css("display") === "none") {
      fullcontent_.css("display", "none");
      shortcontent_.css("display", "-webkit-box");
    } else {
      fullcontent_.css("display", "block");
      shortcontent_.css("display", "none");
    }

    let curr = $(event.target);

    if (curr.attr('class').indexOf('pi-chevron-right') != -1) {
      this.toggle_note_label = 'Thu gọn';
      curr.removeClass('pi-chevron-right');
      curr.addClass('pi-chevron-down');
    } else {
      this.toggle_note_label = 'Mở rộng';
      curr.removeClass('pi-chevron-down');
      curr.addClass('pi-chevron-right');
    }
  }
  /*End */

  /*Event Sửa ghi chú*/
  onClickEditNote(noteId: string, noteDes: string) {
    this.noteContent = noteDes;
    this.noteId = noteId;
    this.listUpdateNoteDocument = this.noteHistory.find(x => x.noteId == this.noteId).noteDocList;
    this.isEditNote = true;
  }
  /*End*/

  /*Event Xóa ghi chú*/
  onClickDeleteNote(noteId: string) {
    this.confirmationService.confirm({
      message: 'Bạn chắc chắn muốn xóa ghi chú này?',
      accept: async () => {
        this.loading = true;
        let result: any = await this.noteService.xoaGhiChu(noteId);
        this.loading = false;

        if (result.statusCode != 200) {
          this.showMessage('error', result.messageCode);
          return;
        }

        this.refreshPage(false); //quay lại page 1
        this.showMessage('success', result.messageCode);
      }
    });
  }

  openItem(name, url) {
    this.imageService.downloadFile(name, url).subscribe(response => {
      var result = <any>response;
      var binaryString = atob(result.fileAsBase64);
      var fileType = result.fileType;

      var binaryLen = binaryString.length;
      var bytes = new Uint8Array(binaryLen);
      for (var idx = 0; idx < binaryLen; idx++) {
        var ascii = binaryString.charCodeAt(idx);
        bytes[idx] = ascii;
      }
      var file = new Blob([bytes], { type: fileType });
      if ((window.navigator as any) && (window.navigator as any).msSaveOrOpenBlob) {
        (window.navigator as any).msSaveOrOpenBlob(file);
      } else {
        var fileURL = URL.createObjectURL(file);
        if (fileType.indexOf('image') !== -1) {
          window.open(fileURL);
        } else {
          var anchor = document.createElement("a");
          anchor.download = name;
          anchor.href = fileURL;
          anchor.click();
        }
      }
    }, error => { });
  }

  showMessage(severity: string, detail: string) {
    this.messageService.add({ severity: severity, summary: 'Thông báo:', detail: detail });
  }


}
