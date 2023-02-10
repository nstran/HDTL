import { map } from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { CustomerOrderDetailExten } from '../../../../src/app/order/models/customer-order-detail.model';

@Injectable({
  providedIn: 'root'
})
export class QuyTrinhService {
  constructor(private httpClient: HttpClient) { }

  checkTrangThaiQuyTrinh(DoiTuongApDung: number, Id: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/quytrinh/checkTrangThaiQuyTrinh';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        DoiTuongApDung: DoiTuongApDung,
        Id: Id
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  createQuyTrinh(quyTrinh: any, listCauHinhQuyTrinh: Array<any>) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/quytrinh/createQuyTrinh';
    return this.httpClient.post(url, {
      QuyTrinh: quyTrinh,
      ListCauHinhQuyTrinh: listCauHinhQuyTrinh
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  searchQuyTrinh(
    listEmployeeId: Array<string>,
    tenQuyTrinh: string,
    maQuyTrinh: string,
    createdDateFrom: Date,
    createdDateTo: Date,
    listTrangThai: Array<boolean>
  ) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/quytrinh/searchQuyTrinh';
    return this.httpClient.post(url, {
      listEmployeeId: listEmployeeId,
      tenQuyTrinh: tenQuyTrinh,
      maQuyTrinh: maQuyTrinh,
      createdDateFrom: createdDateFrom,
      createdDateTo: createdDateTo,
      listTrangThai: listTrangThai
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getMasterDataSearchQuyTrinh() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/quytrinh/getMasterDataSearchQuyTrinh';
    return this.httpClient.post(url, {}).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getDetailQuyTrinh(Id: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/quytrinh/getDetailQuyTrinh';
    return this.httpClient.post(url, { Id: Id }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  updateQuyTrinh(quyTrinh: any, listCauHinhQuyTrinh: Array<any>, isResetDoiTuong: boolean = null) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/quytrinh/updateQuyTrinh';
    return this.httpClient.post(url, {
      QuyTrinh: quyTrinh,
      ListCauHinhQuyTrinh: listCauHinhQuyTrinh,
      IsResetDoiTuong: isResetDoiTuong
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  deleteQuyTrinh(Id: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/quytrinh/deleteQuyTrinh';
    return this.httpClient.post(url, {
      Id: Id,
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  guiPheDuyet(ObjectId: string, DoiTuongApDung: number, ObjectNumber: number = 0, servicePacketId: string = null) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/quytrinh/guiPheDuyet';
    return this.httpClient.post(url, {
      ObjectId: ObjectId,
      DoiTuongApDung: DoiTuongApDung,
      ObjectNumber: ObjectNumber,
      ServicePacketId: servicePacketId
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  huyYeuCauPheDuyet(ObjectId: string, DoiTuongApDung: number, ObjectNumber: number = 0, servicePacketId: string = null) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/quytrinh/huyYeuCauPheDuyet';
    return this.httpClient.post(url, {
      ObjectId: ObjectId,
      DoiTuongApDung: DoiTuongApDung,
      ObjectNumber: ObjectNumber,
      ServicePacketId: servicePacketId
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  pheDuyet(ObjectId: string, DoiTuongApDung: number, Mota: string, ObjectNumber: number = 0, servicePacketId: string = null) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/quytrinh/pheDuyet';
    return this.httpClient.post(url, {
      ObjectId: ObjectId,
      DoiTuongApDung: DoiTuongApDung,
      Mota: Mota,
      ObjectNumber: ObjectNumber,
      ServicePacketId: servicePacketId,
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }


  tuChoi(ObjectId: string, DoiTuongApDung: number, Mota: string, ObjectNumber: number = 0,  servicePacketId: string = null) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/quytrinh/tuChoi';
    return this.httpClient.post(url, {
      ObjectId: ObjectId,
      DoiTuongApDung: DoiTuongApDung,
      Mota: Mota,
      ObjectNumber: ObjectNumber,
      ServicePacketId: servicePacketId
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getLichSuPheDuyet(ObjectId: string, DoiTuongApDung: number) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/quytrinh/getLichSuPheDuyet';
    return this.httpClient.post(url, {
      ObjectId: ObjectId,
      DoiTuongApDung: DoiTuongApDung
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getDuLieuQuyTrinh(ObjectId: string, DoiTuongApDung: number, ObjectNumber: number = 0,servicePacketId: string = null) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/quytrinh/getDuLieuQuyTrinh';
    return this.httpClient.post(url, {
      ObjectId: ObjectId,
      DoiTuongApDung: DoiTuongApDung,
      ObjectNumber: ObjectNumber,
      ServicePacketId: servicePacketId
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }
  getMasterDataCreateQuyTrinh() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/quytrinh/getMasterDataCreateQuyTrinh';
    return this.httpClient.post(url, {

    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  checkUpdateQuyTrinh(quyTrinh: any, listCauHinhQuyTrinh: Array<any>) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/quytrinh/checkUpdateQuyTrinh';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        QuyTrinh: quyTrinh,
        ListCauHinhQuyTrinh: listCauHinhQuyTrinh
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  updateCustomerOrderDetailExtend(orderId: string, listOrderDetailExten: Array<CustomerOrderDetailExten>) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/order/updateCustomerOrderDetailExtend';
    return this.httpClient.post(url, {
      OrderId: orderId,
      ListOrderDetailExten: listOrderDetailExten,
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }
}
