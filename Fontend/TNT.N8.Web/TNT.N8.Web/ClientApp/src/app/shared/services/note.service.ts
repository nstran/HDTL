
import { map } from 'rxjs/operators';
import { Injectable, Pipe } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { NoteModel } from '../models/note.model';
import { NoteDocumentModel } from '../models/note-document.model';
// tslint:disable-next-line:use-pipe-transform-interface

class FileInFolder {
  fileInFolderId: string;
  folderId: string;
  fileName: string;
  objectId: string;
  objectType: string;
  size: string;
  active: boolean;
  fileExtension: string;
}

class FileUploadModel {
  FileInFolder: FileInFolder;
  FileSave: File;
}

@Pipe({ name: 'NoteService' })

@Injectable()
export class NoteService {
  constructor(private httpClient: HttpClient) { }
  userId: string = JSON.parse(localStorage.getItem("auth")).UserId;

  // Create note
  createNote(noteModel: NoteModel, leadId: string, fileList: File[], userId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/note/createNote';
    let formData: FormData = new FormData();
    if (fileList !== null) {
      for (var i = 0; i < fileList.length; i++) {
        formData.append("fileList", fileList[i]);
      }
    }

    formData.append("Note[Description]", noteModel.Description);
    formData.append("Note[Type]", noteModel.Type);
    formData.append('Note[NoteTitle]', noteModel.NoteTitle);
    formData.append('LeadId', leadId);
    formData.append('UserId', userId);

    return this.httpClient.post(url, formData).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  // Disable note
  disableNote(noteId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/note/disableNote';
    return this.httpClient.post(url, { NoteId: noteId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  createNoteAndNoteDocument(leadId: string, fileList: File[], userId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/note/createNoteAndNoteDocument';
    let formData: FormData = new FormData();
    for (var i = 0; i < fileList.length; i++) {
      formData.append('fileList', fileList[i]);
    }

    formData.append('LeadId', leadId);
    formData.append('UserId', userId);

    return this.httpClient.post(url, formData).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  editNoteById(noteId: string, noteDescription: string, fileList: File[], leadId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/note/editNoteById';
    const formData: FormData = new FormData();
    for (let i = 0; i < fileList.length; i++) {
      formData.append('FileList', fileList[i]);
    }
    formData.append('LeadId', leadId);
    formData.append('NoteId', noteId);
    formData.append('NoteDescription', noteDescription);

    return this.httpClient.post(url, formData).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  searchNote(keyword: string, fromDate: Date, toDate: Date, leadId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/note/searchNote';
    return this.httpClient.post(url, { Keyword: keyword, FromDate: fromDate, ToDate: toDate, LeadId: leadId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  createNoteForCustomerDetail(noteModel: NoteModel, listNoteDocumentModel: Array<NoteDocumentModel>) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/note/createNoteForCustomerDetail';
    return this.httpClient.post(url, { Note: noteModel, ListNoteDocument: listNoteDocumentModel }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  createNoteForLeadDetail(noteModel: NoteModel, listNoteDocumentModel: Array<NoteDocumentModel>) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/note/createNoteForLeadDetail';
    return this.httpClient.post(url, { Note: noteModel, ListNoteDocument: listNoteDocumentModel }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  createNoteForOrderDetail(noteModel: NoteModel, listNoteDocumentModel: Array<NoteDocumentModel>) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/note/createNoteForOrderDetail';
    return this.httpClient.post(url, { Note: noteModel, ListNoteDocument: listNoteDocumentModel }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  createNoteForQuoteDetail(noteModel: NoteModel, listNoteDocumentModel: Array<NoteDocumentModel>) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/note/createNoteForQuoteDetail';
    return this.httpClient.post(url, { Note: noteModel, ListNoteDocument: listNoteDocumentModel }).pipe(
      map((response: Response) => {
        return response;
      }));
  }
  createNoteForSaleBiddingDetail(noteModel: NoteModel, listNoteDocumentModel: Array<NoteDocumentModel>) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/note/createNoteForSaleBiddingDetail';
    return this.httpClient.post(url, { Note: noteModel, ListNoteDocument: listNoteDocumentModel }).pipe(
      map((response: Response) => {
        return response;
      }));
  }
  createNoteForBillSaleDetail(noteModel: NoteModel, listNoteDocumentModel: Array<NoteDocumentModel>) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/note/createNoteForBillSaleDetail';
    return this.httpClient.post(url, { Note: noteModel, ListNoteDocument: listNoteDocumentModel }).pipe(
      map((response: Response) => {
        return response;
      }));
  }
  createNoteForContract(noteModel: NoteModel, listNoteDocumentModel: Array<NoteDocumentModel>, objType: string, description: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/note/createNoteForContract';
    return this.httpClient.post(url,
      {
        Note: noteModel,
        ListNoteDocument: listNoteDocumentModel,
        ObjType: objType,
        DescriptionReject: description
      }).pipe(
        map((response: Response) => {
          return response;
        }));
  }

  // createNoteForProjectDetail(noteModel: NoteModel, listNoteDocumentModel: Array<NoteDocumentModel>) {
  //   const url = localStorage.getItem('ApiEndPoint') + '/api/note/createNoteForProjectDetail';
  //   return this.httpClient.post(url, { Note: noteModel, ListNoteDocument: listNoteDocumentModel }).pipe(
  //     map((response: Response) => {
  //       return response;
  //     }));
  // }

  createNoteForProjectDetail(noteModel: NoteModel, folderType: string, listNoteDocumentModel: Array<FileUploadModel>) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/note/createNoteForProjectDetail';
    let formData: FormData = new FormData();

    formData.append("UserId", this.userId);
    formData.append("FolderType", folderType);

    formData.append('Note[NoteId]', noteModel.NoteId);
    formData.append('Note[Description]', noteModel.Description);
    formData.append('Note[Type]', noteModel.Type);
    formData.append('Note[ObjectId]', noteModel.ObjectId);
    formData.append('Note[ObjectType]', noteModel.ObjectType);
    formData.append('Note[NoteTitle]', noteModel.NoteTitle);
    formData.append('Note[CreatedById]', noteModel.CreatedById);
    formData.append('Note[CreatedDate]', null);

    var index = 0;
    for (var pair of listNoteDocumentModel) {
      formData.append("ListNoteDocument[" + index + "].FileInFolder.FileInFolderId", pair.FileInFolder.fileInFolderId);
      formData.append("ListNoteDocument[" + index + "].FileInFolder.FolderId", pair.FileInFolder.folderId);
      formData.append("ListNoteDocument[" + index + "].FileInFolder.FileName", pair.FileInFolder.fileName);
      formData.append("ListNoteDocument[" + index + "].FileInFolder.ObjectId", pair.FileInFolder.objectId);
      formData.append("ListNoteDocument[" + index + "].FileInFolder.ObjectType", pair.FileInFolder.objectType);
      formData.append("ListNoteDocument[" + index + "].FileInFolder.Size", pair.FileInFolder.size);
      formData.append("ListNoteDocument[" + index + "].FileInFolder.FileExtension", pair.FileInFolder.fileExtension);
      formData.append("ListNoteDocument[" + index + "].FileSave", pair.FileSave);
      index++;
    }

    return this.httpClient.post(url, formData).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  deleteNoteDocument(noteId: string, noteDocumentId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/note/deleteNoteDocument';
    return this.httpClient.post(url, { NoteId: noteId, NoteDocumentId: noteDocumentId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  createNoteForObject(noteModel: NoteModel, listFile: Array<FileUploadModel>, folderType: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/note/createNoteForObject';
    let formData: FormData = new FormData();

    formData.append("UserId", this.userId);

    formData.append('Note[NoteId]', noteModel.NoteId);
    formData.append('Note[Description]', noteModel.Description);
    formData.append('Note[Type]', noteModel.Type);
    formData.append('Note[ObjectId]', noteModel.ObjectId);
    formData.append('Note[ObjectType]', noteModel.ObjectType);
    formData.append('Note[NoteTitle]', noteModel.NoteTitle);
    formData.append('Note[CreatedById]', noteModel.CreatedById);
    formData.append('Note[CreatedDate]', null);

    formData.append("FolderType", folderType);

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

    return this.httpClient.post(url, formData).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  createNoteForProjectResource(noteModel: NoteModel) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/note/createNoteForProjectResource';
    return this.httpClient.post(url, {
      Note: noteModel,
      PageSize: 10,
      PageIndex: 1
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  createNoteForProductionOrderDetail(noteModel: NoteModel, listNoteDocumentModel: Array<NoteDocumentModel>) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/note/createNoteForProductionOrderDetail';
    return this.httpClient.post(url, { Note: noteModel, ListNoteDocument: listNoteDocumentModel }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  createNoteForProjectScope(noteModel: NoteModel) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/note/createNoteForProjectScope';
    return this.httpClient.post(url, { Note: noteModel }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  createNoteTask(noteModel: NoteModel) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/note/createNoteTask';
    return this.httpClient.post(url, { Note: noteModel }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  createNoteMilestone(noteModel: NoteModel, isSendEmail: boolean) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/note/createNoteMilestone';
    return this.httpClient.post(url,
      {
        Note: noteModel,
        IsSendEmail: isSendEmail
      }
    ).pipe(
      map((response: Response) => {
        return response;
      }));
  }


  createNoteForAllRecruitmentCampaign(noteModel: NoteModel, objType: string, listNoteDocumentModel: Array<FileUploadModel>) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/note/createNoteForAllRecruitmentCampaign';
    let formData: FormData = new FormData();

    formData.append("UserId", this.userId);
    formData.append("ObjType", objType);

    formData.append('Note[NoteId]', noteModel.NoteId);
    formData.append('Note[Description]', noteModel.Description);
    formData.append('Note[Type]', noteModel.Type);
    formData.append('Note[ObjectId]', noteModel.ObjectId);
    formData.append('Note[ObjectType]', noteModel.ObjectType);
    formData.append('Note[NoteTitle]', noteModel.NoteTitle);
    formData.append('Note[CreatedById]', noteModel.CreatedById);
    formData.append('Note[CreatedDate]', null);

    var index = 0;
    for (var pair of listNoteDocumentModel) {
      formData.append("ListNoteDocument[" + index + "].FileInFolder.FileInFolderId", pair.FileInFolder.fileInFolderId);
      formData.append("ListNoteDocument[" + index + "].FileInFolder.FolderId", pair.FileInFolder.folderId);
      formData.append("ListNoteDocument[" + index + "].FileInFolder.FileName", pair.FileInFolder.fileName);
      formData.append("ListNoteDocument[" + index + "].FileInFolder.ObjectId", pair.FileInFolder.objectId);
      formData.append("ListNoteDocument[" + index + "].FileInFolder.ObjectType", pair.FileInFolder.objectType);
      formData.append("ListNoteDocument[" + index + "].FileInFolder.Size", pair.FileInFolder.size);
      formData.append("ListNoteDocument[" + index + "].FileInFolder.FileExtension", pair.FileInFolder.fileExtension);
      formData.append("ListNoteDocument[" + index + "].FileSave", pair.FileSave);
      index++;
    }

    return this.httpClient.post(url, formData).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getListNote(ObjectType: string, ObjectId: string, ObjectNumber: number, PageSize: number, PageIndex: number) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/note/getListNote';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        ObjectType,
        ObjectId,
        ObjectNumber,
        PageSize,
        PageIndex
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  themMoiGhiChu(Note: any, ObjectType: string, ListFile: any) {
    let formData: FormData = new FormData();

    formData.append("UserId", this.userId);
    formData.append("ObjectType", ObjectType);

    formData.append('Note[NoteId]', Note.NoteId);
    formData.append('Note[Description]', Note.Description);
    formData.append('Note[Type]', Note.Type);
    formData.append('Note[ObjectId]', Note.ObjectId ? Note.ObjectId.toString() : null);
    formData.append('Note[ObjectNumber]', Note.ObjectNumber != null ? Note.ObjectNumber.toString() : null);
    formData.append('Note[ObjectType]', Note.ObjectType);
    formData.append('Note[NoteTitle]', Note.NoteTitle);
    formData.append('Note[CreatedById]', Note.CreatedById);
    formData.append('Note[CreatedDate]', null);

    var index = 0;
    for (var pair of ListFile) {
      formData.append("ListFile[" + index + "].FileInFolder.FileInFolderId", pair.FileInFolder.fileInFolderId);
      formData.append("ListFile[" + index + "].FileInFolder.FolderId", pair.FileInFolder.folderId);
      formData.append("ListFile[" + index + "].FileInFolder.FileName", pair.FileInFolder.fileName);
      formData.append("ListFile[" + index + "].FileInFolder.ObjectId", pair.FileInFolder.objectId);
      formData.append("ListFile[" + index + "].FileInFolder.ObjectNumber", pair.FileInFolder.objectNumber != null ? pair.FileInFolder.objectNumber.toString() : null);
      formData.append("ListFile[" + index + "].FileInFolder.ObjectType", pair.FileInFolder.objectType);
      formData.append("ListFile[" + index + "].FileInFolder.Size", pair.FileInFolder.size);
      formData.append("ListFile[" + index + "].FileInFolder.Active", 'true');
      formData.append("ListFile[" + index + "].FileInFolder.FileExtension", pair.FileInFolder.fileExtension);
      formData.append("ListFile[" + index + "].FileInFolder.CreatedById", this.userId);
      formData.append("ListFile[" + index + "].FileSave", pair.FileSave);
      index++;
    }

    const url = localStorage.getItem('ApiEndPoint') + '/api/note/themMoiGhiChu';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, formData).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  xoaGhiChu(NoteId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/note/xoaGhiChu';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        NoteId,
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  xoaFileGhiChu(FileInFolderId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/note/xoaFileGhiChu';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        FileInFolderId,
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }
}
