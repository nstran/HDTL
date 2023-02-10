import { map } from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { AdditionalInformationModel } from "../../shared/models/additional-information.model";
import { Quote } from "../models/quote.model";
import { QuoteDetail } from "../models/quote-detail.model";
import { QuoteCostDetail } from '../models/quote-cost-detail.model';
import { PromotionObjectApply } from '../../promotion/models/promotion-object-apply.model';
import { QuotePlanModel } from '../models/quote-plan.model';
import { QuoteScopeModel } from '../models/QuoteScopeModel';
import { QuotePaymentTerm } from '../models/quote-payment-term.model';


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
}

@Injectable()
export class QuoteService {

  constructor(private httpClient: HttpClient) { }

  userId: string = JSON.parse(localStorage.getItem("auth")).UserId;

  CreateQuote(quote: Quote,
    quoteDetail: Array<QuoteDetail>,
    typeAccount: number,
    fileList: Array<any>,
    listAdditionalInformation: Array<AdditionalInformationModel>,
    quoteCostDetail: Array<QuoteCostDetail>,
    isClone: boolean,
    quoteIdClone: string,
    listParticipant: Array<string>,
    listPromotionObjectApply: Array<PromotionObjectApply>,
    listQuotePlan: Array<QuotePlanModel>,
    listQuoteScope: Array<QuoteScopeModel>,
    listQuotePaymentTerm: Array<QuotePaymentTerm>) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/quote/createQuote';
    return this.httpClient.post(url, {
      Quote: quote,
      QuoteDetail: quoteDetail,
      QuoteCostDetail: quoteCostDetail,
      TypeAccount: typeAccount,
      QuoteDocument: fileList,
      isClone: isClone,
      QuoteIdClone: quoteIdClone,
      ListAdditionalInformation: listAdditionalInformation,
      ListParticipant: listParticipant,
      ListPromotionObjectApply: listPromotionObjectApply,
      QuotePlans: listQuotePlan,
      //ListQuoteScope: listQuoteScope
      ListQuotePaymentTerm: listQuotePaymentTerm,
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  createScope(
    scopeId: string,
    category: string,
    description: string,
    level: Number,
    parentId: string,
    quoteId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/quote/createScope";
    return this.httpClient.post(url, {
      ScopeId: scopeId, Category: category, Description: description,
      Level: level, ParentId: parentId, QuoteId: quoteId,
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  deleteScope(
    scopeId: string) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/quote/deleteScope";
    return this.httpClient.post(url, {
      ScopeId: scopeId,
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  UpdateQuote(quote: Quote, quoteDetail: Array<QuoteDetail>, typeAccount: number, fileList: Array<any>) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/quote/updateQuote';
    return this.httpClient.post(url, {
      Quote: quote,
      QuoteDetail: quoteDetail,
      TypeAccount: typeAccount,
      FileList: fileList,
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  GetQuoteByID(quoteId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/quote/getQuoteByID';
    return this.httpClient.post(url, {
      QuoteId: quoteId,
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  GetAllQuote(quoteCode: string, productCode: string, quoteStatusId: Array<string>) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/quote/getAllQuote';

    return this.httpClient.post(url,
      {
        QuoteCode: quoteCode,
        ProductCode: productCode,
        QuoteStatusId: quoteStatusId,
      }).pipe(
        map((response: Response) => {
          return <any>response;
        }));
  }

  GetTop3QuotesOverdue(personInChangeId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/quote/getTop3QuotesOverdue';

    return this.httpClient.post(url,
      {
        PersonInChangeId: personInChangeId
      }).pipe(
        map((response: Response) => {
          return <any>response;
        }));
  }

  GetTop3WeekQuotesOverdue(personInChangeId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/quote/getTop3WeekQuotesOverdue';

    return this.httpClient.post(url,
      {
        PersonInChangeId: personInChangeId
      }).pipe(
        map((response: Response) => {
          return <any>response;
        }));
  }

  GetTop3PotentialCustomers(personInChangeId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/quote/getTop3PotentialCustomers';

    return this.httpClient.post(url,
      {
        PersonInChangeId: personInChangeId
      }).pipe(
        map((response: Response) => {
          return <any>response;
        }));
  }

  GetTotalAmountQuote(personInChangeId: string, month: number, year: number) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/quote/getTotalAmountQuote';

    return this.httpClient.post(url,
      {
        PersonInChangeId: personInChangeId,
        MonthQuote: month,
        YearQuote: year
      }).pipe(
        map((response: Response) => {
          return <any>response;
        }));
  }
  GetDashBoardQuote(personInChangeId: string, month: number, year: number) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/quote/getDashBoardQuote';

    return this.httpClient.post(url,
      {
        PersonInChangeId: personInChangeId,
        MonthQuote: month,
        YearQuote: year
      }).pipe(
        map((response: Response) => {
          return <any>response;
        }));
  }

  getDataQuoteToPieChart(month: number, year: number) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/quote/getDataQuoteToPieChart';

    return this.httpClient.post(url,
      {
        MonthQuote: month,
        YearQuote: year
      }).pipe(
        map((response: Response) => {
          return <any>response;
        }));
  }

  updateActiveQuote(quoteId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/quote/updateActiveQuote';

    return this.httpClient.post(url,
      {
        QuoteId: quoteId,
        UserId: this.userId
      }).pipe(
        map((response: Response) => {
          return <any>response;
        }));
  }

  searchQuote(quoteCode: string, startDate: Date, endDate: Date, listStatusQuote: Array<string>,
    isOutOfDate: boolean, isCompleteInWeek: boolean, quoteName: string, isPotentialCustomer: boolean, isCustomer: boolean, listEmpCreateId: Array<string>, listNhanVienPhuTrach: Array<string>, listSeller: Array<string>) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/quote/searchQuote';

    return this.httpClient.post(url,
      {
        QuoteCode: quoteCode,
        StartDate: startDate,
        EndDate: endDate,
        ListStatusQuote: listStatusQuote,
        IsOutOfDate: isOutOfDate,
        QuoteName: quoteName,
        IsCompleteInWeek: isCompleteInWeek,
        IsPotentialCustomer: isPotentialCustomer,
        IsCustomer: isCustomer,
        ListEmpCreateId: listEmpCreateId,
        PersonInChargeId: listNhanVienPhuTrach,
        SellerId:listSeller
      }).pipe(
        map((response: Response) => {
          return <any>response;
        }));
  }

  searchQuoteApproval(quoteCode: string, startDate: Date, endDate: Date, listStatusQuote: Array<string>,
    isOutOfDate: boolean, isCompleteInWeek: boolean) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/quote/searchQuoteApproval';

    return this.httpClient.post(url,
      {
        QuoteCode: quoteCode,
        StartDate: startDate,
        EndDate: endDate,
        ListStatusQuote: listStatusQuote,
        IsOutOfDate: isOutOfDate,
        IsCompleteInWeek: isCompleteInWeek
      }).pipe(
        map((response: Response) => {
          return <any>response;
        }));
  }

  getDataCreateUpdateQuote(quoteId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/quote/getDataCreateUpdateQuote';

    return this.httpClient.post(url, { QuoteId: quoteId }).pipe(
      map((response: Response) => {
        return <any>response;
      }));
  }

  getEmployeeSale(listEmp: any[], employeeId: string, OldEmployeeId: any) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/quote/getEmployeeSale';

    return this.httpClient.post(url, { ListEmployeeByAccount: listEmp, EmployeeId: employeeId , OldEmployeeId : OldEmployeeId}).pipe(
      map((response: Response) => {
        return <any>response;
      }));
  }

  downloadTemplateProduct() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/quote/downloadTemplateProduct';

    return this.httpClient.post(url, {}).pipe(
      map((response: Response) => {
        return <any>response;
      }));
  }

  getMasterDataCreateCost(userId) {
    const url = localStorage.getItem('ApiEndPoint') + "/api/quote/getMasterDataCreateCost";
    return this.httpClient.post(url, { UserId: userId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getDataQuoteAddEditProductDialog() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/quote/getDataQuoteAddEditProductDialog';

    return this.httpClient.post(url, {}).pipe(
      map((response: Response) => {
        return <any>response;
      }));
  }

  getDataQuoteAddEditProductDialogAsync() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/quote/getDataQuoteAddEditProductDialog';

    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {})
        .toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getVendorByProductId(productId: string, soLuong: number, customerGroupId?: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/quote/getVendorByProductId';

    return this.httpClient.post(url, { ProductId: productId, SoLuong: soLuong, CustomerGroupId: customerGroupId }).pipe(
      map((response: Response) => {
        return <any>response;
      }));
  }

  updateStatusQuote(quoteId: string, userId: string, objectType: string, amountPriceInitial: number, amountPriceProfit: number, customerOrderAmountAfterDiscount: number, totalAmountDiscount: number) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/quote/updateStatusQuote';

    return this.httpClient.post(url, {
      QuoteId: quoteId,
      UserId: userId,
      ObjectType: objectType,
      AmountPriceInitial: amountPriceInitial,
      AmountPriceProfit: amountPriceProfit,
      CustomerOrderAmountAfterDiscount: customerOrderAmountAfterDiscount,
      TotalAmountDiscount: totalAmountDiscount,
    }).pipe(
      map((response: Response) => {
        return <any>response;
      }));
  }

  sendEmailCustomerQuote(lstEmail: string[], lstEmailCC: string[], lstEmailBcc: string[], titleEmail: string, contentEmail: string, quoteId: string, file: File[], userId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/quote/sendEmailCustomerQuote';
    let formData: FormData = new FormData();

    for (let i = 0; i < lstEmail.length; i++) {
      formData.append("ListEmail", lstEmail[i]);
    }

    for (let i = 0; i < lstEmailCC.length; i++) {
      formData.append("ListEmailCC", lstEmailCC[i]);
    }

    for (let i = 0; i < lstEmailBcc.length; i++) {
      formData.append("ListEmailBcc", lstEmailBcc[i]);
    }

    formData.append("TitleEmail", titleEmail);

    formData.append("ContentEmail", contentEmail);

    formData.append("QuoteId", quoteId);
    formData.append("UserId", userId);

    for (let i = 0; i < file.length; i++) {
      formData.append("ListFormFile", file[i]);
    }

    return this.httpClient.post(url, formData).pipe(
      map((response: Response) => {
        return <any>response;
      }));
  }

  approvalOrRejectQuote(listQuoteId: string[], isApproval: boolean, userId: string, description: string, rejectReason: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/quote/approvalOrRejectQuote';

    return this.httpClient.post(url, { ListQuoteId: listQuoteId, IsApproval: isApproval, UserId: userId, Description: description, RejectReason: rejectReason }).pipe(
      map((response: Response) => {
        return <any>response;
      }));
  }

  getDataExportExcelQuote(quoteId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/quote/getDataExportExcelQuouter';
    return this.httpClient.post(url, { QuoteId: quoteId }).pipe(
      map((response: Response) => {
        return <any>response;
      }));
  }

  getMasterDataCreateQuote(objectId: string, objectType: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/quote/getMasterDataCreateQuote';

    return this.httpClient.post(url, { ObjectId: objectId, ObjectType: objectType }).pipe(
      map((response: Response) => {
        return <any>response;
      }));
  }

  getEmployeeByPersonInCharge(employeeId: string, OldEmployeeId: any) {
    let url = localStorage.getItem('ApiEndPoint') + "/api/quote/getEmployeeByPersonInCharge";

    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, { EmployeeId: employeeId, OldEmployeeId : OldEmployeeId })
        .toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getMasterDataUpdateQuote(quoteId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/quote/getMasterDataUpdateQuote';

    return this.httpClient.post(url, { QuoteId: quoteId }).pipe(
      map((response: Response) => {
        return <any>response;
      }));
  }

  UploadQuoteDocument(quoteId: string, fileList: Array<any>,) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/quote/uploadQuoteDocument';
    return this.httpClient.post(url, {
      QuoteId: quoteId,
      FileList: fileList,
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getMasterDataSearchQuote() {
    let url = localStorage.getItem('ApiEndPoint') + "/api/quote/getMasterDataSearchQuote";
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {})
        .toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }
}
