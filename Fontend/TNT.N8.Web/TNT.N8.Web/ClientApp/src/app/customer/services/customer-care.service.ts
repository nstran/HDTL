
import { map } from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { CustomerCareModel } from "../models/customer-care.model";
import { CustomerCareFeedBack } from "../models/customer-care-feed-back.model";
import { QueueModel } from '../models/queue.model';

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

@Injectable()
export class CustomerCareService {

  constructor(private httpClient: HttpClient) { }
  userId: string = JSON.parse(localStorage.getItem("auth")).UserId;

  createCustomerCare(
    customerCare: CustomerCareModel,
    customerId: Array<string>,
    listTypeCustomer: Array<string>,
    queryFilter: string,
    files: File[],
    listSelectedTinhTrangEmail: Array<number>
  ) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/customerCare/createCustomerCare";
    return this.httpClient.post(url, {
      customerCare: customerCare,
      customerId: customerId,
      listTypeCustomer: listTypeCustomer,
      queryFilter: queryFilter,
      listSelectedTinhTrangEmail: listSelectedTinhTrangEmail,
      ListFormFile: files
    }).pipe(
      map((response: Response) => {
        return response;
      }));


    // let formData: FormData = new FormData();
    // formData.append("UserId", userId);
    // formData.append("QueryFilter", queryFilter);

    // formData.append("CustomerCare.CustomerCareId", customerCare.CustomerCareId);
    // formData.append("CustomerCare.CustomerCareCode", customerCare.CustomerCareCode);
    // formData.append("CustomerCare.EmployeeCharge", customerCare.EmployeeCharge);
    // formData.append("CustomerCare.EffecttiveFromDate", customerCare.EffecttiveFromDate == null ? null : customerCare.EffecttiveFromDate.toUTCString());
    // formData.append("CustomerCare.EffecttiveToDate", customerCare.EffecttiveToDate == null ? null : customerCare.EffecttiveToDate.toUTCString());
    // formData.append("CustomerCare.CustomerCareTitle", customerCare.CustomerCareTitle);
    // formData.append("CustomerCare.CustomerCareContent", customerCare.CustomerCareContent);
    // formData.append("CustomerCare.CustomerCareContactType", customerCare.CustomerCareContactType);
    // formData.append("CustomerCare.CustomerCareContentSMS", customerCare.CustomerCareContentSMS);
    // formData.append("CustomerCare.ExpectedAmount", customerCare.ExpectedAmount == null ? null : customerCare.ExpectedAmount.toString());
    // formData.append("CustomerCare.IsSendNow", customerCare.IsSendNow ? "true" : "false");
    // formData.append("CustomerCare.IsEvent", customerCare.IsEvent ? "true" : "false");
    // formData.append("CustomerCare.IsEvent", customerCare.IsEvent ? "true" : "false");
    // formData.append("CustomerCare.SendDate", customerCare.SendDate == null ? null : customerCare.SendDate.toUTCString());
    // formData.append("CustomerCare.SendHour", customerCare.SendHour == null ? null : customerCare.SendHour.toUTCString());
    // formData.append("CustomerCare.CustomerCareEvent", customerCare.CustomerCareEvent);
    // formData.append("CustomerCare.CustomerCareEventHour", customerCare.CustomerCareEventHour == null ? null : customerCare.CustomerCareEventHour.toUTCString());
    // formData.append("CustomerCare.CustomerCareContentEmail", customerCare.CustomerCareContentEmail);
    // formData.append("CustomerCare.IsSendEmailNow", customerCare.IsSendEmailNow ? "true" : "false");
    // formData.append("CustomerCare.SendEmailDate", customerCare.SendEmailDate == null ? null : customerCare.SendEmailDate.toUTCString());
    // formData.append("CustomerCare.SendEmailHour", customerCare.SendEmailHour == null ? null : customerCare.SendEmailHour.toUTCString());
    // formData.append("CustomerCare.CustomerCareVoucher", customerCare.CustomerCareVoucher);
    // formData.append("CustomerCare.DiscountAmount", customerCare.DiscountAmount == null ? null : customerCare.DiscountAmount.toString());
    // formData.append("CustomerCare.PercentDiscountAmount", customerCare.PercentDiscountAmount == null ? null : customerCare.PercentDiscountAmount.toString());
    // formData.append("CustomerCare.GiftCustomerType1", customerCare.GiftCustomerType1 == null ? null : customerCare.GiftCustomerType1.toString());
    // formData.append("CustomerCare.GiftTypeId1", customerCare.GiftTypeId1);
    // formData.append("CustomerCare.GiftTotal1", customerCare.GiftTotal1 == null ? null : customerCare.GiftTotal1.toString());
    // formData.append("CustomerCare.GiftCustomerType2", customerCare.GiftCustomerType2 == null ? null : customerCare.GiftCustomerType2.toString());
    // formData.append("CustomerCare.GiftTypeId2", customerCare.GiftTypeId2);
    // formData.append("CustomerCare.GiftTotal2", customerCare.GiftTotal2 == null ? null : customerCare.GiftTotal2.toString());
    // formData.append("CustomerCare.CustomerCareType", customerCare.CustomerCareType == null ? null : customerCare.CustomerCareType.toString());
    // formData.append("CustomerCare.StatusId", customerCare.StatusId);
    // formData.append("CustomerCare.StatusCode", customerCare.StatusCode);
    // formData.append("CustomerCare.StatusCode", customerCare.StatusCode);
    // formData.append("CustomerCare.CreateById", customerCare.CreateById);
    // formData.append("CustomerCare.CustomerCareContactTypeName", customerCare.CustomerCareContactTypeName);
    // formData.append("CustomerCare.StatusName", customerCare.StatusName);
    // formData.append("CustomerCare.EmployeeChargeName", customerCare.EmployeeChargeName);
    // formData.append("CustomerCare.IsEditStatus", customerCare.IsEditStatus ? "true" : "false");
    // formData.append("CustomerCare.TypeCustomer", customerCare.TypeCustomer == null ? null : customerCare.TypeCustomer.toString());
    // formData.append("CustomerCare.IsSentMail", customerCare.IsSentMail ? "true" : "false");
    // formData.append("CustomerCare.IsFilterSendMailCondition", customerCare.IsFilterSendMailCondition ? "true" : "false");


    // for (var i = 0; i < customerId.length; i++){
    //   formData.append("CustomerId", customerId[i]);
    // }

    // for (var i = 0; i < listTypeCustomer.length; i++){
    //   formData.append("ListTypeCustomer", listTypeCustomer[i]);
    // }

    // for (let i = 0; i < files.length; i++) {
    //   formData.append("ListFormFile", files[i]);
    // }

    // return this.httpClient.post(url, formData).pipe(
    //   map((response: Response) => {
    //     return response;
    //   }));
  }

  updateCustomerCare(
    customerCare: CustomerCareModel,
    customerId: Array<string>,
    queryFilter: string,
    files: File[],
    listSelectedTinhTrangEmail: Array<number>
  ) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/customerCare/updateCustomerCare";
    return this.httpClient.post(url, {
      customerCare: customerCare,
      customerId: customerId,
      queryFilter: queryFilter,
      listSelectedTinhTrangEmail: listSelectedTinhTrangEmail,
      ListFormFile: files
    }).pipe(
      map((response: Response) => {
        return response;
      }));

    // let formData: FormData = new FormData();
    // formData.append("UserId", userId);
    // formData.append("QueryFilter", queryFilter);

    // formData.append("CustomerCare.CustomerCareId", customerCare.CustomerCareId);
    // formData.append("CustomerCare.CustomerCareCode", customerCare.CustomerCareCode);
    // formData.append("CustomerCare.EmployeeCharge", customerCare.EmployeeCharge);
    // formData.append("CustomerCare.EffecttiveFromDate", customerCare.EffecttiveFromDate == null ? null : customerCare.EffecttiveFromDate.toUTCString());
    // formData.append("CustomerCare.EffecttiveToDate", customerCare.EffecttiveToDate == null ? null : customerCare.EffecttiveToDate.toUTCString());
    // formData.append("CustomerCare.CustomerCareTitle", customerCare.CustomerCareTitle);
    // formData.append("CustomerCare.CustomerCareContent", customerCare.CustomerCareContent);
    // formData.append("CustomerCare.CustomerCareContactType", customerCare.CustomerCareContactType);
    // formData.append("CustomerCare.CustomerCareContentSMS", customerCare.CustomerCareContentSMS);
    // formData.append("CustomerCare.ExpectedAmount", customerCare.ExpectedAmount == null ? null : customerCare.ExpectedAmount.toString());
    // formData.append("CustomerCare.IsSendNow", customerCare.IsSendNow ? "true" : "false");
    // formData.append("CustomerCare.IsEvent", customerCare.IsEvent ? "true" : "false");
    // formData.append("CustomerCare.IsEvent", customerCare.IsEvent ? "true" : "false");
    // formData.append("CustomerCare.SendDate", customerCare.SendDate == null ? null : customerCare.SendDate.toUTCString());
    // formData.append("CustomerCare.SendHour", customerCare.SendHour == null ? null : customerCare.SendHour.toUTCString());
    // formData.append("CustomerCare.CustomerCareEvent", customerCare.CustomerCareEvent);
    // formData.append("CustomerCare.CustomerCareEventHour", customerCare.CustomerCareEventHour == null ? null : customerCare.CustomerCareEventHour.toUTCString());
    // formData.append("CustomerCare.CustomerCareContentEmail", customerCare.CustomerCareContentEmail);
    // formData.append("CustomerCare.IsSendEmailNow", customerCare.IsSendEmailNow ? "true" : "false");
    // formData.append("CustomerCare.SendEmailDate", customerCare.SendEmailDate == null ? null : customerCare.SendEmailDate.toUTCString());
    // formData.append("CustomerCare.SendEmailHour", customerCare.SendEmailHour == null ? null : customerCare.SendEmailHour.toUTCString());
    // formData.append("CustomerCare.CustomerCareVoucher", customerCare.CustomerCareVoucher);
    // formData.append("CustomerCare.DiscountAmount", customerCare.DiscountAmount == null ? null : customerCare.DiscountAmount.toString());
    // formData.append("CustomerCare.PercentDiscountAmount", customerCare.PercentDiscountAmount == null ? null : customerCare.PercentDiscountAmount.toString());
    // formData.append("CustomerCare.GiftCustomerType1", customerCare.GiftCustomerType1 == null ? null : customerCare.GiftCustomerType1.toString());
    // formData.append("CustomerCare.GiftTypeId1", customerCare.GiftTypeId1);
    // formData.append("CustomerCare.GiftTotal1", customerCare.GiftTotal1 == null ? null : customerCare.GiftTotal1.toString());
    // formData.append("CustomerCare.GiftCustomerType2", customerCare.GiftCustomerType2 == null ? null : customerCare.GiftCustomerType2.toString());
    // formData.append("CustomerCare.GiftTypeId2", customerCare.GiftTypeId2);
    // formData.append("CustomerCare.GiftTotal2", customerCare.GiftTotal2 == null ? null : customerCare.GiftTotal2.toString());
    // formData.append("CustomerCare.CustomerCareType", customerCare.CustomerCareType == null ? null : customerCare.CustomerCareType.toString());
    // formData.append("CustomerCare.StatusId", customerCare.StatusId);
    // formData.append("CustomerCare.StatusCode", customerCare.StatusCode);
    // formData.append("CustomerCare.StatusCode", customerCare.StatusCode);
    // formData.append("CustomerCare.CreateById", customerCare.CreateById);
    // formData.append("CustomerCare.CustomerCareContactTypeName", customerCare.CustomerCareContactTypeName);
    // formData.append("CustomerCare.StatusName", customerCare.StatusName);
    // formData.append("CustomerCare.EmployeeChargeName", customerCare.EmployeeChargeName);
    // formData.append("CustomerCare.IsEditStatus", customerCare.IsEditStatus ? "true" : "false");
    // formData.append("CustomerCare.TypeCustomer", customerCare.TypeCustomer == null ? null : customerCare.TypeCustomer.toString());

    // for (var i = 0; i < customerId.length; i++){
    //   formData.append("CustomerId", customerId[i]);
    // }

    // for (let i = 0; i < files.length; i++) {
    //   formData.append("ListFormFile", files[i]);
    // }

    // return this.httpClient.post(url, formData).pipe(
    //   map((response: Response) => {
    //     return response;
    //   }));
  }
  getCustomerCareById(customerCareId: string, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/customerCare/getCustomerCareById";

    return this.httpClient.post(url, { CustomerCareId: customerCareId, UserId: userId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getTotalInteractive(month: number, year: number) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/customerCare/getTotalInteractive";

    return this.httpClient.post(url, { Month: month, Year: year }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getCustomerCareActive() {
    let url = localStorage.getItem('ApiEndPoint') + "/api/customerCare/getCustomerCareActive";

    return this.httpClient.post(url, {}).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getCharCustomerCS() {
    let url = localStorage.getItem('ApiEndPoint') + "/api/customerCare/getCharCustomerCS";

    return this.httpClient.post(url, {}).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getCustomerNewCS() {
    let url = localStorage.getItem('ApiEndPoint') + "/api/customerCare/getCustomerNewCS";

    return this.httpClient.post(url, {}).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getCustomerBirthDay() {
    let url = localStorage.getItem('ApiEndPoint') + "/api/customerCare/getCustomerBirthDay";

    return this.httpClient.post(url, {}).pipe(
      map((response: Response) => {
        return response;
      }));
  }
  createCustomerCareFeedBack(customerCareFeedBack: CustomerCareFeedBack, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/customerCare/createCustomerCareFeedBack";

    return this.httpClient.post(url, { CustomerCareFeedBack: customerCareFeedBack, UserId: userId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }
  updateCustomerCareFeedBack(customerCareFeedBack: CustomerCareFeedBack, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/customerCare/updateCustomerCareFeedBack";

    return this.httpClient.post(url, { CustomerCareFeedBack: customerCareFeedBack, UserId: userId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  filterCustomer(
    sqlQuery: string,
    typeCustomer: Array<any>,
    isSendEmail: boolean,
    listSelectedTinhTrangEmail: Array<number>,
    userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/customerCare/filterCustomer";

    return this.httpClient.post(url, {
      SqlQuery: sqlQuery,
      CustomerStatusCode: typeCustomer,
      IsSendEmail: isSendEmail,
      ListSelectedTinhTrangEmail: listSelectedTinhTrangEmail,
      UserId: userId
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getCustomerCareByIdAsync(customerCareId: string, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/customerCare/getCustomerCareById";
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        CustomerCareId: customerCareId,
        UserId: userId
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }
  searchCustomerCare(fromDate: Date, toDate: Date, customerCareTitle: string, picName: Array<string>, status: Array<string>, customerCareContent: string, programType: Array<number>, userId: string, customerCareCode: string, typeCusCare: Array<string>) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/customerCare/searchCustomerCare";

    return this.httpClient.post(url, { FromDate: fromDate, ToDate: toDate, CustomerCareTitle: customerCareTitle, PicName: picName, Status: status, CustomerCareContent: customerCareContent, ProgramType: programType, UserId: userId, CustomerCareCode: customerCareCode, ListTypeCusCareId: typeCusCare }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  updateStatusCustomerCareCustomer(customerCareCustomerId: string, statusId: string, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/customerCare/updateStatusCustomerCareCustomerById";

    return this.httpClient.post(url, { CustomerCareCustomerId: customerCareCustomerId, StatusId: statusId, UserId: userId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  updateStatusCustomerCareCustomerAsync(customerCareCustomerId: string, statusId: string, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/customerCare/updateStatusCustomerCareCustomerById";

    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, { CustomerCareCustomerId: customerCareCustomerId, StatusId: statusId, UserId: userId }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getTimeLineCustomerCareByCustomerId(customerId: string, first_day: Date, last_day: Date) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/customerCare/getTimeLineCustomerCareByCustomerId";

    return this.httpClient.post(url, { CustomerId: customerId, First_day: first_day, Last_day: last_day }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getTimeLineCustomerCareByCustomerIdAsync(customerId: string, first_day: Date, last_day: Date) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/customerCare/getTimeLineCustomerCareByCustomerId";
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, { CustomerId: customerId, First_day: first_day, Last_day: last_day }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getCustomerCareFeedBackByCusIdAndCusCareId(customerId: string, customerCareId: string, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/customerCare/getCustomerCareFeedBackByCusIdAndCusCareId";

    return this.httpClient.post(url, { CustomerId: customerId, CustomerCareId: customerCareId, UserId: userId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  sendQuickEmail(queue: QueueModel, attchement: Array<FileUploadModel>, folderType: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/customerCare/sendQuickEmail";

    let formData: FormData = new FormData();

    formData.append("FolderType", folderType);
    formData.append("UserId", this.userId);

    formData.append("Queue.QueueId", queue.QueueId);
    formData.append("Queue.FromTo", queue.FromTo);
    formData.append("Queue.SendTo", queue.SendTo);
    formData.append("Queue.SendContent", queue.SendContent);
    formData.append("Queue.Title", queue.Title);
    formData.append("Queue.Method", queue.Method);
    formData.append("Queue.IsSend", queue.IsSend ? "true" : "false");
    formData.append("Queue.SenDate", queue.SenDate);
    formData.append("Queue.CustomerCareId", queue.CustomerCareId);
    formData.append("Queue.CustomerId", queue.CustomerId);
    formData.append("Queue.StatusId", queue.StatusId);
    formData.append("Queue.CreateDate", queue.CreateDate);
    formData.append("Queue.CreateById", queue.CreateById);
    formData.append("Queue.Bcc", queue.SendToBCC);
    formData.append("Queue.Cc", queue.SendToCC);

    var index = 0;
    for (var pair of attchement) {
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

  sendQuickSMS(queue: QueueModel) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/customerCare/sendQuickSMS";

    return this.httpClient.post(url, { Queue: queue }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  sendQuickGift(title: string, giftCustomerType1: number, giftTypeId1: string, giftTotal1: number,
    giftCustomerType2: number, giftTypeId2: string, giftTotal2: number, customerId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/customerCare/sendQuickGift";

    return this.httpClient.post(url,
      {
        Title: title,
        GiftCustomerType1: giftCustomerType1,
        GiftTypeId1: giftTypeId1,
        GiftTotal1: giftTotal1,
        GiftCustomerType2: giftCustomerType2,
        GiftTypeId2: giftTypeId2,
        GiftTotal2: giftTotal2,
        CustomerId: customerId
      }).pipe(
        map((response: Response) => {
          return response;
        }));
  }

  updateStatusCustomerCare(customerCareId: string, statusId: string, isSendNow: boolean, sendDate: Date, sendHour: Date, typeCusCareCode: string, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/customerCare/updateStatusCustomerCare";
    return this.httpClient.post(url,
      {
        CustomerCareId: customerCareId,
        StatusId: statusId,
        IsSendNow: isSendNow,
        SendDate: sendDate,
        SendHour: sendHour,
        TypeCusCareCode: typeCusCareCode,
        UserId: userId
      }).pipe(
        map((response: Response) => {
          return response;
        }));
  }

  getMasterDataCustomerCareList(userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/customerCare/getMasterDataCustomerCareList";
    return this.httpClient.post(url, { UserId: userId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getMasterDataCustomerCareListAsync(userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/customerCare/getMasterDataCustomerCareList";
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, { UserId: userId }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  updateStatusCusCare(userId: string, customerCareId: string, statusId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/customerCare/updateStatusCusCare";
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, { UserId: userId, CustomerCareId: customerCareId, StatusId: statusId }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  removeCustomerMeeting(customerMeetingId: string, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/customerCare/removeCustomerMeeting";
    return this.httpClient.post(url, { CustomerMeetingId: customerMeetingId, UserId: userId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

}
