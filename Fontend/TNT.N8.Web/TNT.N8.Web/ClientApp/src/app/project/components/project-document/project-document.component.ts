import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Location } from '@angular/common';

// MODEL
import { ProjectModel } from '../../models/project.model';

// SERVICE
import { ProjectService } from '../../services/project.service';
import { ImageUploadService } from '../../../shared/services/imageupload.service';
import { GetPermission } from '../../../shared/permission/get-permission';

// PRIMENG
import { MessageService, TreeNode } from 'primeng/api';
import { ConfirmationService } from 'primeng/api';
import { Item } from '@syncfusion/ej2-navigations';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ForderConfigurationService } from '../../.././admin/components/folder-configuration/services/folder-configuration.service';
import { FileUpload } from 'primeng/fileupload';

class Project {
  projectId: string;
  projectCode: string;
  projectName: string;
  projectCodeName: string;
}

class FileUploadModel {
  FileInFolder: FileInFolder;
  FileSave: File;
}

class Folder {
  folderId: string;
  parentId: string;
  name: string;
  url: string;
  isDelete: boolean;
  active: boolean;
  hasChild: boolean;
  folderType: string;
  folderLevel: number;
  listFile: Array<FileInFolder>;
  fileNumber: number;
}

class FileInFolder {
  fileInFolderId: string;
  folderId: string;
  fileName: string;
  objectId: string;
  objectType: string;
  size: string;
  active: boolean;
  fileExtension: string;
  createdById: string;
  createdDate: Date;
  createByName: string;
}

class Document {
  noteDocumentId: string;
  projectId: string;
  documentName: string;
  createByName: string;
  documentSize: string;
  documentUrl: string;
  documentType: string;
  documentExtension: string;
  documentNameWithoutExtension: string;
  updatedDate: Date;
  active: boolean;
  objectId: string;
  objectType: string;
  folderId: string;
  noteId: string;
  fileNumber: number;
}

class TaskDocument {
  taskDocumentId: string;
  taskId: string;
  documentOrder: number;
  documentNameWithoutExtension: string;
  documentName: string;
  documentSize: string;
  documentUrl: string;
  documentExtension: string;
  active: boolean;
  createdDate: Date;
  createdById: string;
  createByName: string;
  taskCodeName: string;
  updatedDate: Date;
  updatedById: string;
  objectId: string;
  objectType: string;
  folderId: string;
}


@Component({
  selector: 'app-project-document',
  templateUrl: './project-document.component.html',
  styleUrls: ['./project-document.component.css']
})
export class ProjectDocumentComponent implements OnInit {

  listProjectTitle: Array<Project> = [];
  selectedProject: Project = null;

  loading: boolean = false;

  auth = JSON.parse(localStorage.getItem("auth"));
  systemParameterList = JSON.parse(localStorage.getItem('systemParameterList'));
  defaultLimitedFileSize = Number(this.systemParameterList.find(systemParameter => systemParameter.systemKey == "LimitedFileSize").systemValueString) * 1024 * 1024;
  strAcceptFile: string = 'image video audio .zip .rar .pdf .xls .xlsx .doc .docx .ppt .pptx .txt';

  /*Check user permission*/
  listPermissionResource: string = localStorage.getItem("ListPermissionResource");
  filterGlobal: string;
  emptyGuid: string = '00000000-0000-0000-0000-000000000000';
  projectId: string = '00000000-0000-0000-0000-000000000000';

  project: ProjectModel = null;
  totalPercent: number = 0; // Tiến độ dự án
  totalEstimateTime: string = '0';
  totalSize: number = 0;
  rowGroupMetadata: any;
  projectCodeName: string = '';
  SelectFolderName: string = '';
  isContainResource: boolean = false;
  isSelectFolder: boolean = false;
  isSelectTaskDocument: boolean = false;
  isSelectProjectDocument: boolean = true;
  showDialogAddFolder: boolean = false;
  showDialogAddFile: boolean = false;
  isSave: boolean = true;

  // danh sach tai lieu
  uploadedFiles: any[] = [];
  listDocument: Array<Document> = [];
  listDocumentDelete: Array<Document> = [];
  listNoteDocument: Array<Document> = [];
  listNoteDocumentDelete: Array<Document> = [];
  listTaskDocument: Array<TaskDocument> = [];
  listTaskDocumentDelete: Array<TaskDocument> = [];
  listFolders: Array<Folder> = [];
  listFoldersRoot: Array<Folder> = [];
  // listFoldersChild: Array<Folder> = [];
  folders: TreeNode[] = [];
  selectedForderNode: TreeNode;
  currentExpandNote: TreeNode;
  selectedForder: Folder = {
    folderId: this.emptyGuid,
    active: false,
    folderLevel: 0,
    folderType: '',
    isDelete: false,
    hasChild: false,
    listFile: [],
    name: '',
    fileNumber: 0,
    parentId: this.emptyGuid,
    url: ''
  };

  // list column
  cols: any[];
  colsTask: any[];
  selectedColumns: any[];
  selectedColumnsTask: any[];

  // Form Group
  addFolderFormGroup: FormGroup;
  parentFolderControl: FormControl;
  folderNameControl: FormControl;

  addFileFormGroup: FormGroup;
  parentFolderOfFileControl: FormControl;
  fileNameControl: FormControl;

  @ViewChild('fileUpload') fileUpload: FileUpload;

  constructor(
    private getPermission: GetPermission,
    private route: ActivatedRoute,
    private projectService: ProjectService,
    private messageService: MessageService,
    private imageService: ImageUploadService,
    private confirmationService: ConfirmationService,
    private folderService: ForderConfigurationService,
    private location: Location,
    private router: Router,
  ) { }

  ngOnInit(): void {
    this.initForm();
    this.initTable();

    this.route.params.subscribe(params => {
      this.projectId = params['projectId'];
      this.getMasterData();
    });
  }

  getMasterData() {
    this.loading = true;
    this.projectService.getMasterProjectDocument(this.projectId).subscribe(response => {
      let result: any = response;
      this.loading = false;
      if (result.statusCode === 200) {
        this.listProjectTitle = result.listProject;
        this.listProjectTitle.forEach(item => {
          item.projectCodeName = `[${item.projectCode}]: ${item.projectName}`;
        });
        this.selectedProject = this.listProjectTitle.find(c => c.projectId == this.projectId);
        this.listDocument = [];
        this.folders = [];
        this.project = result.project;
        this.projectCodeName = `${this.project.projectCode} - ${this.project.projectName}`;
        this.totalSize = result.totalSize;

        this.totalPercent = result.projectTaskComplete.toFixed(2);

        this.totalEstimateTime = result.totalEstimateHour;

        this.listDocument = result.listDocument;

        this.listTaskDocument = result.listTaskDocument;

        this.listFolders = result.listFolders;

        this.listFoldersRoot = this.listFolders.filter(x => x.folderLevel == 2);
        // this.listFoldersChild = this.listFolders.filter(x => x.folderLevel != 2);

        this.listDocument.forEach(item => {
          item.documentExtension = item.documentName.substring(item.documentName.lastIndexOf('.') + 1);
          item.documentNameWithoutExtension = item.documentName.substring(0, item.documentName.lastIndexOf('_')).toLocaleLowerCase();
          item.documentType = this.classifyTypeDocument(item.documentExtension);
          item.fileNumber = CountFileByFolderId(this.listDocument, item.folderId);
        })

        this.listTaskDocument.forEach(item => {
          item.documentExtension = item.documentName.substring(item.documentName.lastIndexOf('.') + 1);
          item.documentNameWithoutExtension = item.documentName.substring(0, item.documentName.lastIndexOf('_')).toLocaleLowerCase();
        })

        this.listFoldersRoot.forEach(item => {
          let folder: TreeNode = {
            label: item.name,
            data: item,
            expandedIcon: "fas fa-folder-open",
            collapsedIcon: "fas fa-folder",
            key: item.folderId,
            children: this.mapDataTreeNode(this.listFolders, item),
          };
          folder.expanded = true;
          this.folders.push(folder);
        });
      } else {
        this.clearToast();
        this.showToast('error', 'Thông báo', result.messageCode);
      }
    });
  }

  initTable() {
    this.cols = [
      { field: 'documentNameWithoutExtension', header: 'Tên tài liệu', textAlign: 'left', display: 'table-cell', width: '250px' },
      { field: 'updatedDate', header: 'Ngày tải lên', textAlign: 'center', display: 'table-cell', width: '150px' },
      { field: 'createByName', header: 'Người tải lên', textAlign: 'left', display: 'table-cell', width: '100px' },
      { field: 'documentType', header: 'Loại file', textAlign: 'left', display: 'table-cell', width: '100px' },
      { field: 'documentSize', header: 'Kích thước', textAlign: 'right', display: 'table-cell', width: '100px' },
    ];
    this.selectedColumns = this.cols;

    this.colsTask = [
      { field: 'documentOrder', header: 'STT', textAlign: 'center', display: 'table-cell', width: '20px' },
      { field: 'documentNameWithoutExtension', header: 'Tên tài liệu', textAlign: 'left', display: 'table-cell', width: '160px' },
      { field: 'createdDate', header: 'Ngày tải lên', textAlign: 'center', display: 'table-cell', width: '60px' },
      { field: 'createByName', header: 'Người tải lên', textAlign: 'left', display: 'table-cell', width: '100px' },
      { field: 'taskCodeName', header: 'Công việc liên quan', textAlign: 'left', display: 'table-cell', width: '200px' },
    ];
    this.selectedColumnsTask = this.colsTask;
  }

  initForm() {
    this.parentFolderControl = new FormControl('', Validators.required);
    this.folderNameControl = new FormControl('', [Validators.required, forbiddenSpaceText]);
    this.parentFolderOfFileControl = new FormControl('');

    this.addFolderFormGroup = new FormGroup({
      parentFolderControl: this.parentFolderControl,
      folderNameControl: this.folderNameControl,
    });

    this.addFileFormGroup = new FormGroup({
      parentFolderOfFileControl: this.parentFolderOfFileControl,
    });

  }

  cancel() {
    if (!this.isSave) {
      this.confirmationService.confirm({
        message: `Các thay đổi sẽ không được lưu lại. Hành động này không thể được hoàn tác, bạn có chắc chắn muốn thoát?`,
        accept: () => {
          // this.location.back();
          this.router.navigate(['/project/detail', { projectId: this.projectId }]);
        }
      });
    }
    else {
      this.router.navigate(['/project/detail', { projectId: this.projectId }]);
    }
  }

  update() {
    this.loading = true;

    // // xao file vat ly task
    // this.listTaskDocumentDelete.forEach(item => {
    //   this.projectService.deleteFileForOptionAsync('Task', item.documentName, this.projectCodeName);
    // });

    // // xao file vat ly Note
    // this.listNoteDocumentDelete.forEach(item => {
    //   this.projectService.deleteFileForOptionAsync('Note', item.documentName, this.projectCodeName);
    // });

    // xoa file trong DB
    this.projectService.UpdateProjectDocument(this.projectId, this.listTaskDocumentDelete, this.listDocumentDelete).subscribe(response => {
      let result = <any>response;
      this.loading = false;
      if (result.statusCode == 200) {
        // this.getMasterData();
        this.isSave = true;
        this.isSelectTaskDocument = this.isSelectTaskDocument;
        this.isSelectProjectDocument = this.isSelectProjectDocument;
        this.selectedForderNode = this.selectedForderNode;

        if (this.selectedForderNode != null) {
          // let node = this
          this.listDocument = [];
          this.loadFileByFolderId(this.selectedForderNode.data.folderId, this.selectedForderNode.data.folderType, this.projectId);
        }

        this.folders = [];

        this.listFolders = result.listFolders;

        this.listFoldersRoot = this.listFolders.filter(x => x.folderLevel == 2);
        // this.listFoldersChild = this.listFolders.filter(x => x.folderLevel != 2);

        this.listFoldersRoot.forEach(item => {
          let folder: TreeNode = {
            label: item.name,
            data: item,
            expandedIcon: "fas fa-folder-open",
            collapsedIcon: "fas fa-folder",
            key: item.folderId,
            children: this.mapDataTreeNode(this.listFolders, item),
          };
          folder.expanded = true;
          this.folders.push(folder);
        });

        this.listTaskDocumentDelete = [];
        this.listDocumentDelete = [];
        this.clearToast();
        this.showToast('success', 'Thông báo', 'Cập nhật tài liệu thành công')
      } else {
        this.clearToast();
        this.showToast('error', 'Thông báo', result.messageCode)
      }
    });
  }

  convertFileSize(size: string) {
    let tempSize = parseFloat(size);
    if (tempSize < 1024 * 1024) {
      return true;
    } else {
      return false;
    }
  }

  classifyTypeOfFile(rowData: Document) {
    switch (rowData.documentExtension.toLocaleLowerCase()) {
      case 'docx':
      case 'doc':
        return 'word';
        break;
      case 'pdf':
        return 'pdf';
        break;
      case 'txt':
        return 'text';
        break;
      case 'png':
      case 'jpg':
      case 'jpeg':
      case 'ico':
        return 'image'
        break;
      case 'pps':
      case 'ppt':
      case 'pptx':
        return 'powerpoint'
        break;
      case 'xls':
      case 'xlsm':
      case 'xlsx':
        return 'excel'
      case 'rar':
        return 'rar'
        break;
      case 'mp4':
        return 'video'
        break;
      case 'folder':
        return 'folder'
        break;
      default:
        return 'other';
        break;
    }
  }

  classifyTypeDocument(extension: string) {
    let documentType = '';
    switch (extension.toLocaleLowerCase()) {
      case 'docx':
      case 'doc':
        documentType = 'Word';
        break;
      case 'pdf':
        documentType = 'Pdf';
        break;
      case 'txt':
        documentType = 'Text';
        break;
      case 'png':
      case 'jpg':
      case 'jpeg':
        documentType = 'Image'
        break;
      case 'ico':
        documentType = 'Icon'
        break;
      case 'pps':
      case 'ppt':
      case 'pptx':
        documentType = 'Powerpoint'
        break;
      case 'xls':
      case 'xlsm':
      case 'xlsx':
        documentType = 'Excel'
        break;
      case 'rar':
        documentType = 'RAR'
        break;
      case 'mp4':
        documentType = 'Video'
        break;
      case 'folder':
        documentType = 'Folder'
        break;
      default:
        documentType = 'Other';
        break;
    }
    return documentType;
  }

  /*Event khi download 1 file đã lưu trên server*/
  downloadFile(fileInfor: Document) {

    this.imageService.downloadFile(fileInfor.documentNameWithoutExtension, fileInfor.documentUrl).subscribe(response => {
      var result = <any>response;
      if (result.statusCode == 200) {
        var binaryString = atob(result.fileAsBase64);
        var fileType = result.fileType;
        var name = fileInfor.documentNameWithoutExtension;

        var binaryLen = binaryString.length;
        var bytes = new Uint8Array(binaryLen);
        for (var idx = 0; idx < binaryLen; idx++) {
          var ascii = binaryString.charCodeAt(idx);
          bytes[idx] = ascii;
        }
        var file = new Blob([bytes], { type: fileType });
        if (window.navigator && window.navigator.msSaveOrOpenBlob) {
          window.navigator.msSaveOrOpenBlob(file);
        } else {
          var fileURL = URL.createObjectURL(file);
          if (fileType.indexOf('image') !== -1) {
            window.open(fileURL);
          } else {
            var anchor = document.createElement("a");
            anchor.download = name + result.extension;
            anchor.href = fileURL;
            anchor.click();
          }
        }
      } else {
        this.clearToast();
        this.showToast('error', 'Thông báo', result.messageCode)
      }
    });
  }

  /*Event khi xóa 1 file đã lưu trên server*/
  deleteTaskFile(file: TaskDocument) {
    this.confirmationService.confirm({
      message: 'Bạn chắc chắn muốn xóa?',
      accept: () => {
        this.isSave = false;
        let taskDocument = this.listTaskDocument.find(x => x.taskDocumentId == file.taskDocumentId);
        let index = this.listTaskDocument.indexOf(file);
        this.listTaskDocument.splice(index, 1);
        this.listTaskDocumentDelete.push(taskDocument);
        this.clearToast();
        this.showToast('success', 'Thông báo', 'Xóa file thành công, hành động của bạn sẽ được lưu lại khi bạn click button "Cập nhật."')
      }
    });
  }

  /*Event khi xóa 1 file đã lưu trên server*/
  deleteNoteFile(file: Document) {
    this.confirmationService.confirm({
      message: 'Bạn chắc chắn muốn xóa?',
      accept: () => {
        this.isSave = false;

        let noteDocument = this.listDocument.find(x => x.noteDocumentId == file.noteDocumentId);
        let index = this.listDocument.indexOf(file);

        // this.folders = [];

        // this.listFolders.forEach(item => {
        //   if (item.folderId == file.folderId) {
        //     item.fileNumber -= 1;
        //   }
        // })

        // this.listFoldersRoot = this.listFolders.filter(x => x.folderLevel == 2);
        // // this.listFoldersChild = this.listFolders.filter(x => x.folderLevel != 2);

        // this.listFoldersRoot.forEach(item => {
        //   let folder: TreeNode = {
        //     label: item.name,
        //     data: item,
        //     expandedIcon: "fas fa-folder-open",
        //     collapsedIcon: "fas fa-folder",
        //     key: item.folderId,
        //     children: this.mapDataTreeNode(this.listFolders, item),
        //   };
        //   folder.expanded = true;
        //   this.folders.push(folder);
        // });

        this.listDocument.splice(index, 1);
        this.listDocumentDelete.push(noteDocument);
        this.clearToast();
        this.showToast('success', 'Thông báo', 'Xóa file thành công, hành động của bạn sẽ được lưu lại khi bạn click button "Cập nhật."')
      }
    });
  }

  nodeExpand(event) {
    this.currentExpandNote = event;
  }

  nodeSelect(event: TreeNode) {

    this.listDocument = [];
    this.isSelectTaskDocument = false;
    this.isSelectProjectDocument = true;
    this.isSelectFolder = true;
    this.SelectFolderName = event.label;
    this.selectedForder.isDelete = this.selectedForderNode.data.isDelete;
    this.loading = true;
    this.loadFileByFolderId(this.selectedForderNode.data.folderId, this.selectedForderNode.data.folderType, this.projectId);
  }

  loadFileByFolderId(folderId: string, folderType: string, projectId: string) {

    this.projectService.loadFileByFolder(folderId, folderType, projectId).subscribe(response => {
      let result: any = response;
      this.loading = false;
      if (result.statusCode == 200) {
        this.uploadedFiles = [];
        this.listDocument = [];
        if (this.fileUpload) {
          this.fileUpload.clear();  //Xóa toàn bộ file trong control
        }

        let listFolderDocument = result.listFileInFolder;
        listFolderDocument.forEach(item => {
          let file = new Document();
          file.noteDocumentId = item.fileInFolderId;
          file.documentName = item.fileFullName;
          file.documentSize = item.size;
          file.updatedDate = item.updatedDate;
          file.documentUrl = item.fileUrl;
          file.documentExtension = item.fileExtension;
          file.documentNameWithoutExtension = item.fileName.substring(0, item.fileFullName.lastIndexOf('_')).toLocaleLowerCase();
          file.documentType = this.classifyTypeDocument(item.fileExtension);
          file.objectId = item.objectId;
          file.folderId = item.folderId;
          file.objectType = item.objectType;
          file.createByName = item.createdByName;
          this.listDocument.push(file)
        });


      } else {
        this.clearToast();
        this.showToast('error', 'Thôngg bóa', result.messageCode);
      }
    });
  }

  nodeUnselect(event) {
    this.isSelectFolder = false;
    this.selectedForderNode = null;
  }

  showDialog() {
    this.showDialogAddFolder = true;
    if (this.selectedForderNode) {
      this.parentFolderControl.setValue(this.selectedForderNode.data);
    }
    else {
      let folderRoot: Folder = {
        active: true,
        folderId: this.emptyGuid,
        folderLevel: 0,
        folderType: `${this.project.projectCode} - ${this.project.projectName}`,
        url: '',
        hasChild: false,
        isDelete: false,
        parentId: null,
        name: 'Danh mục tài liệu dự án',
        listFile: [],
        fileNumber: 0
      }
      this.listFolders.push(folderRoot);
      this.parentFolderControl.setValue(folderRoot);
    }

  }

  showFileDialog() {
    this.showDialogAddFile = true;
    this.parentFolderOfFileControl.setValue(this.listFolders.find(x => x.folderId == this.selectedForderNode.data.folderId));
    this.parentFolderOfFileControl.disable();
  }

  showConfirm() {
    this.showDialogAddFolder = true;
  }

  addFolder() {

    if (!this.addFolderFormGroup.valid) {
      Object.keys(this.addFolderFormGroup.controls).forEach(key => {
        if (!this.addFolderFormGroup.controls[key].valid) {
          this.addFolderFormGroup.controls[key].markAsTouched();
        }
      });
    } else if (this.folderNameControl.value.length > 225) {
      this.clearToast();
      this.showToast('error', 'Thông báo', 'Tên thư mục không quá 225 ký tự');
    } else {
      this.loading = true;
      this.showDialogAddFolder = false;
      if (this.selectedForderNode) {
        let folder: Folder = this.parentFolderControl.value;
        let folderName = this.folderNameControl.value.replace(/\s+/g, ' ').trim();
        this.loading = true;
        this.projectService.createFolder(folder, folderName, this.project.projectCode).subscribe(response => {
          let result: any = response;
          this.loading = false;
          if (result.statusCode == 200) {
            this.listFoldersRoot = [];
            this.folders = []
            this.getMasterData();
            this.folderNameControl.reset();
            this.clearToast();
            this.showToast('success', 'Thông báo', 'Tạo thư mục thành công');
          } else {
            this.clearToast();
            this.showToast('error', 'Thông báo', result.messageCode);
          }
        });
      }
    }
  }

  addFile() {
    if (this.uploadedFiles.length > 0) {
      this.loading = true;
      this.showDialogAddFile = false;
      this.listDocument = [];
      let folder: Folder = this.parentFolderOfFileControl.value;
      let listFileUploadModel: Array<FileUploadModel> = [];
      this.uploadedFiles.forEach(item => {
        let fileUpload: FileUploadModel = new FileUploadModel();
        fileUpload.FileInFolder = new FileInFolder();
        fileUpload.FileInFolder.active = true;
        let index = item.name.lastIndexOf(".");
        let name = item.name.substring(0, index);
        fileUpload.FileInFolder.fileName = name;
        fileUpload.FileInFolder.fileExtension = item.name.substring(index + 1);
        fileUpload.FileInFolder.size = item.size;
        fileUpload.FileInFolder.objectId = this.projectId;
        fileUpload.FileInFolder.objectType = 'QLDA';
        fileUpload.FileSave = item;
        listFileUploadModel.push(fileUpload);
      });
      this.loading = true;
      this.projectService.uploadFileDocument(folder.folderId, folder.folderType, listFileUploadModel, this.projectId).subscribe(response => {
        let result: any = response;
        this.loading = false;
        if (result.statusCode == 200) {
          this.uploadedFiles = [];
          this.listDocument = [];
          this.folders = [];

          if (this.fileUpload) {
            this.fileUpload.clear();  //Xóa toàn bộ file trong control
          }

          let listFolderDocument = result.listFileInFolder;

          listFolderDocument.forEach(item => {
            let file = new Document();
            file.noteDocumentId = item.fileInFolderId;
            file.documentName = item.fileName;
            file.createByName = item.createdByName;
            file.documentSize = item.size;
            file.updatedDate = item.updatedDate;
            file.documentUrl = item.fileUrl;
            file.documentExtension = item.fileExtension;
            file.documentNameWithoutExtension = item.fileFullName.substring(0, item.fileFullName.lastIndexOf('_')).toLocaleLowerCase();
            file.documentType = this.classifyTypeDocument(item.fileExtension);
            this.listDocument.push(file)
          });

          this.totalSize += result.totalSize;

          this.listFolders = result.listFolders;

          this.listFoldersRoot = this.listFolders.filter(x => x.folderLevel == 2);
          // this.listFoldersChild = this.listFolders.filter(x => x.folderLevel != 2);

          this.listFoldersRoot.forEach(item => {
            let folder: TreeNode = {
              label: item.name,
              data: item,
              expandedIcon: "fas fa-folder-open",
              collapsedIcon: "fas fa-folder",
              key: item.folderId,
              children: this.mapDataTreeNode(this.listFolders, item),
            };
            folder.expanded = true;
            this.folders.push(folder);
          });
          this.clearToast();
          this.showToast('success', 'Thông báo', 'Thêm file thành công')
        } else {
          this.clearToast();
          this.showToast('error', 'Thôngg bóa', result.messageCode);
        }
      });
    }
    else {
      this.clearToast();
      this.showToast('error', 'Thôngg bóa', "Vui lòng chọn file cần tải lên");
    }
  }

  closeDialog() {
    this.showDialogAddFolder = false;
    this.showDialogAddFile = false;
    this.folderNameControl.reset();
  }

  showToast(severity: string, summary: string, detail: string) {
    this.messageService.add({ severity: severity, summary: summary, detail: detail });
  }

  clearToast() {
    this.messageService.clear();
  }

  mapDataTreeNode(listFolderCommon: Array<Folder>, folder: Folder): Array<TreeNode> {
    let result: Array<TreeNode> = [];
    let listChildren = listFolderCommon.filter(x => x.parentId == folder.folderId);
    listChildren.forEach(item => {
      let dataRoot: Folder = new Folder();
      dataRoot = item;
      let listChildrenNode: Array<TreeNode> = [];
      if (item.hasChild) {
        listChildrenNode = this.mapDataTreeNode(listFolderCommon, item);
      }
      let node: TreeNode = {
        data: dataRoot,
        children: listChildrenNode,
        expandedIcon: "fas fa-folder-open",
        collapsedIcon: "fas fa-folder",
        label: dataRoot.name,
        key: dataRoot.folderId,
      }
      result.push(node);
    });
    return result;
  }

  /*Event Lưu các file được chọn*/
  handleFile(event, uploader: FileUpload) {
    for (let file of event.files) {
      let size: number = file.size;
      let type: string = file.type;
      if (size <= this.defaultLimitedFileSize) {
        if (type.indexOf('/') != -1) {
          type = type.slice(0, type.indexOf('/'));
        }
        if (this.strAcceptFile.includes(type) && type != "") {
          this.uploadedFiles.push(file);
        } else {
          let subType = file.name.slice(file.name.lastIndexOf('.'));
          if (this.strAcceptFile.includes(subType)) {
            this.uploadedFiles.push(file);
          }
        }
      }
    }
  }

  /*Event Khi click xóa từng file*/
  removeFile(event) {
    let index = this.uploadedFiles.indexOf(event.file);
    this.uploadedFiles.splice(index, 1);
  }

  /*Event Khi click xóa toàn bộ file*/
  clearAllFile() {
    this.uploadedFiles = [];
  }

  handleChange(event) {

    this.isSelectTaskDocument = !this.isSelectTaskDocument;
    this.isSelectProjectDocument = !this.isSelectProjectDocument;
    this.isSelectFolder = false;
    this.selectedForderNode = null;
    this.selectedForder = {
      folderId: this.emptyGuid,
      active: false,
      folderLevel: 0,
      folderType: '',
      isDelete: false,
      hasChild: false,
      listFile: [],
      name: '',
      fileNumber: 0,
      parentId: this.emptyGuid,
      url: ''
    };
    this.loadFileByFolderId(this.folders[0].data.folderId, this.folders[0].data.folderType, this.projectId);
  }

  onChangeProject(projectId: string) {
    this.router.navigate(['/project/document', { 'projectId': projectId }]);
  }
}

function forbiddenSpaceText(control: FormControl) {
  let text = control.value;
  if (text && text.trim() == "") {
    return {
      forbiddenSpaceText: {
        parsedDomain: text
      }
    }
  }
  return null;
}

function CountFileByFolderId(listDocument: Array<Document>, folderId: string) {
  let number = listDocument.filter(x => x.folderId == folderId).length;
  return number;
}
