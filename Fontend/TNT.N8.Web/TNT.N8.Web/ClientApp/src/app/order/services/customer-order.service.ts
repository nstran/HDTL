
import { map } from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { CustomerOrder, CustomerOrderTaskEntityModel, ReportPointEntityModel } from '../models/customer-order.model';
import { CustomerOrderDetail, CustomerOrderDetailExten, CustomerOrderExtension } from '../models/customer-order-detail.model';
import { ContactModel } from '../../shared/models/contact.model';
import { OrderCostDetail } from '../models/customer-order-cost-detail.model';
import { CustomList } from '../components/create/create.component';
import { OrderProcessEntityModel } from '../../../../src/app/product/models/product.model';
import { CustomerModel } from '../components/orderProcessList/orderProcessList.component';
@Injectable()
export class CustomerOrderService {

  constructor(private httpClient: HttpClient) { }

  userId: string = JSON.parse(localStorage.getItem("auth")).UserId;

  CreateCustomerOrder(
    orderId: string,
    vat: CustomList,
    discountType: number,
    discountValue: number,
    cusOrder: CustomerOrder,
    listCustomerDetail: Array<CustomerOrderDetail>,
    listAttrPackAndOption: Array<CustomerOrderExtension>,
    listOrderExten: Array<CustomerOrderDetailExten>,
    orderProcessId: string,
    servicePacketId: string,
  ) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/order/createCustomerOrder';
    return this.httpClient.post(url, {
      OrderId: orderId,
      Vat: vat,
      DiscountType: discountType,
      DiscountValue: discountValue,
      CusOrder: cusOrder,
      ListCustomerDetail: listCustomerDetail,
      ListAttrPackAndOption: listAttrPackAndOption,
      ListOrderDetailExten: listOrderExten,
      OrderProcessId: orderProcessId,
      ServicePacketId: servicePacketId
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }



  UpdateCustomerOrder(customerOrder: CustomerOrder, customerOrderDetail: Array<CustomerOrderDetail>, listOrderCostDetailModel: Array<OrderCostDetail>, typeAccount: number) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/order/updateCustomerOrder';
    return this.httpClient.post(url, {
      CustomerOrder: customerOrder,
      CustomerOrderDetail: customerOrderDetail,
      OrderCostDetail: listOrderCostDetailModel,
      TypeAccount: typeAccount,
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }
  checkBeforCreateOrUpdateOrder(CustomerId: string, MaxDebt: number, AmountOrder: number) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/order/checkBeforCreateOrUpdateOrder';
    return this.httpClient.post(url, {
      AmountOrder: AmountOrder,
      CustomerId: CustomerId,
      MaxDebt: MaxDebt,
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  updateStatusOrder(customerOrderId: string, objectType: string, userId: string, description: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/order/updateStatusOrder';
    return this.httpClient.post(url, {
      CustomerOrderId: customerOrderId,
      ObjectType: objectType,
      Description: description,
      UserId: userId
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  GetCustomerOrderByID(customerOrderId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/order/getCustomerOrderByID';
    return this.httpClient.post(url, {
      CustomerOrderId: customerOrderId,
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  GetCustomerOrderByIDAsync(customerOrderId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/order/getCustomerOrderByID';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        CustomerOrderId: customerOrderId,
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  ExportPdfCustomerOrder(customerOrderId: string, userId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/order/exportPdfCustomerOrder';
    return this.httpClient.post(url, {
      CustomerOrderId: customerOrderId,
      UserId: userId,
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  sendEmailCustomerOrder(orderId: string, userId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/email/sendEmailCustomerOrder';
    return this.httpClient.post(url, {
      OrderId: orderId,
      UserId: userId,
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getMasterDataOrderSearch(isOrderAction, isOrderProcess: boolean = false) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/order/getMasterDataOrderSearch';
    return this.httpClient.post(url, {
      UserId: this.userId,
      IsOrderAction: isOrderAction,
      IsOrderProcess: isOrderProcess,
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  searchOrder(listPacketIdSelected: Array<string>, listCreatorIdSelected: Array<string>,
    listStatusSelected: Array<number>, fromDate: Date, toDate: Date, listCusId: Array<string>, IsOrderAction: boolean,
    IsOrderProcess: boolean = false, listCusSelected: Array<string> = [], listProductCateSelected: Array<string> = []
  ) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/order/searchOrder';
    return this.httpClient.post(url, {
      ListPacketIdSelected: listPacketIdSelected,
      ListCreatorIdSelected: listCreatorIdSelected,
      ListStatusSelected: listStatusSelected,
      FromDate: fromDate,
      ToDate: toDate,
      ListCusId: listCusId,
      IsOrderAction: IsOrderAction,
      IsOrderProcess: IsOrderProcess,
      ListCusSelected: listCusSelected,
      ListProductCateSelected: listProductCateSelected,
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  searchProfitAccordingCustomers(orderCode: string, customerName: string, listStatusId: Array<string>, fromDate: Date, toDate: Date, productId: string, quoteId: string, seller: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/order/searchProfitAccordingCustomers';
    return this.httpClient.post(url, {
      OrderCode: orderCode,
      CustomerName: customerName,
      ListStatusId: listStatusId,
      FromDate: fromDate,
      ToDate: toDate,
      ProductId: productId,
      QuoteId: quoteId,
      Seller: seller
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getMasterDataOrderCreate(createObjectId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/order/getMasterDataOrderCreate';
    return this.httpClient.post(url, {
      CreateObjectId: createObjectId
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getMasterDataOrderDetailDialog() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/order/getMasterDataOrderDetailDialog';
    return this.httpClient.post(url, {}).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getMasterDataOrderDetailDialogAsync() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/order/getMasterDataOrderDetailDialog';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {}).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getVendorByProductId(productId: string, customerGroupId: string, orderDate: Date) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/order/getVendorByProductId';

    return this.httpClient.post(url, {
      ProductId: productId,
      CustomerGroupId: customerGroupId,
      OrderDate: orderDate,
    }).pipe(
      map((response: Response) => {
        return <any>response;
      }));
  }

  getMasterDataOrderDetail(orderId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/order/getMasterDataOrderDetail';
    return this.httpClient.post(url, { OrderId: orderId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  deleteOrder(orderId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/order/deleteOrder';
    return this.httpClient.post(url, { OrderId: orderId, UserId: this.userId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  checkReceiptOrderHistory(orderId: string, moneyOrder: number) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/order/checkReceiptOrderHistory';
    return this.httpClient.post(url, {
      OrderId: orderId,
      MoneyOrder: moneyOrder
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getMasterDataOrderServiceCreate() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/order/getMasterDataOrderServiceCreate';
    return this.httpClient.post(url, {}).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  createOrderService(customerOrder: CustomerOrder, listCustomerOrderDetail: Array<CustomerOrderDetail>, listLocalPointId: Array<string>) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/order/createOrderService';
    return this.httpClient.post(url, {
      CustomerOrder: customerOrder,
      ListCustomerOrderDetail: listCustomerOrderDetail,
      ListLocalPointId: listLocalPointId,
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getMasterDataPayOrderService() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/order/getMasterDataPayOrderService';
    return this.httpClient.post(url, {}).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getListOrderByLocalPoint(localPointId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/order/getListOrderByLocalPoint';
    return this.httpClient.post(url, { LocalPointId: localPointId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  payOrderByLocalPoint(listOrderId: Array<string>, customerId: string, customerName: string, customerPhone: string,
    discountType: boolean, discountValue: number, point: number, payPoint: number) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/order/payOrderByLocalPoint';
    return this.httpClient.post(url, {
      ListOrderId: listOrderId,
      CustomerId: customerId,
      CustomerName: customerName,
      CustomerPhone: customerPhone,
      DiscountType: discountType,
      DiscountValue: discountValue,
      Point: point,
      PayPoint: payPoint
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  checkExistsCustomerByPhone(customerPhone: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/order/checkExistsCustomerByPhone';
    return this.httpClient.post(url, { CustomerPhone: customerPhone }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  refreshLocalPoint() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/order/refreshLocalPoint';
    return this.httpClient.post(url, {}).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getLocalPointByLocalAddress(localAddressId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/order/getLocalPointByLocalAddress';
    return this.httpClient.post(url, { LocalAddressId: localAddressId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getListOrderDetailByOrder(orderId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/order/getListOrderDetailByOrder';
    return this.httpClient.post(url, { OrderId: orderId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getListProductWasOrder(localPointId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/order/getListProductWasOrder';
    return this.httpClient.post(url, { LocalPointId: localPointId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  updateCustomerService(orderId: string, listCustomerOrderDetail: Array<CustomerOrderDetail>) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/order/updateCustomerService';
    return this.httpClient.post(url, { OrderId: orderId, ListCustomerOrderDetail: listCustomerOrderDetail }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getDataProfitByCustomer(userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/order/getDataProfitByCustomer';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        UserId: userId
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  searchProfitCustomer(filterData: any, userId: string) {
    let url = localStorage.getItem('ApiEndPoint') + '/api/order/searchProfitCustomer';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        ...filterData,
        UserId: userId
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getInventoryNumber(wareHouseId: string, productId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/order/getInventoryNumber';
    return this.httpClient.post(url, {
      WareHouseId: wareHouseId,
      ProductId: productId,
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }
  CheckTonKhoSanPham(orderId: string, customerOrderDetail: Array<CustomerOrderDetail>) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/order/CheckTonKhoSanPham';
    return this.httpClient.post(url, {
      OrderId: orderId,
      CustomerOrderDetail: customerOrderDetail,
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }
  UpdateCustomerOrderTonKho(customerOrder: CustomerOrder, customerOrderDetail: Array<CustomerOrderDetail>, listOrderCostDetailModel:
    Array<OrderCostDetail>, typeAccount: number, statusType: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/order/UpdateCustomerOrderTonKho';
    return this.httpClient.post(url, {
      CustomerOrder: customerOrder,
      CustomerOrderDetail: customerOrderDetail,
      OrderCostDetail: listOrderCostDetailModel,
      TypeAccount: typeAccount,
      StatusType: statusType
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  searchOptionOfPacketService(packetServiceId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/order/searchOptionOfPacketService';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, {
        PacketServiceId: packetServiceId
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  changeStatusCustomerOrder(
    orderId: string,
    statusOrder: number
  ) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/order/changeStatusCustomerOrder';
    return this.httpClient.post(url, {
      OrderId: orderId,
      StatusOrder: statusOrder,
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getMasterDataCreateOrderAction() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/order/getMasterDataCreateOrderAction';
    return this.httpClient.post(url, {}).pipe(
      map((response: Response) => {
        return response;
      }));
  }


  changeCustomerOrder(orderId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/order/changeCustomerOrder';
    return this.httpClient.post(url, { OrderId: orderId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getVendorAndEmployeeOfPacket(packetServiceId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/order/getVendorAndEmployeeOfPacket';
    return this.httpClient.post(url, {
      PacketServiceId: packetServiceId
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  createOrUpdateCustomerOrderAction(customerOrderActionId: string, customerOrderId: string, listSettingEmpToTask: Array<CustomerOrderTaskEntityModel>, statusOrderAction: number) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/order/createOrUpdateCustomerOrderAction';
    return this.httpClient.post(url, {
      customerOrderActionId: customerOrderActionId,
      customerOrderId: customerOrderId,
      listSettingEmpToTask: listSettingEmpToTask,
      StatusOrderAction: statusOrderAction,
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getMasterDataOrderActionDetail(orderActionId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/order/getMasterDataOrderActionDetail';
    return this.httpClient.post(url, { OrderActionId: orderActionId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getOptionAndEmpOfOrderAction(orderActionId: string, servicePacketMappingOptionsId) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/order/getOptionAndEmpOfOrderAction';
    return this.httpClient.post(url, { OrderActionId: orderActionId, ServicePacketMappingOptionsId: servicePacketMappingOptionsId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  createOrUpdateCustomerReportPoint(reportPoint: ReportPointEntityModel, orderActionId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/order/createOrUpdateCustomerReportPoint';
    return this.httpClient.post(url, {
      ReportPoint: reportPoint,
      OrderActionId: orderActionId,
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }


  changeStatusReportPoint(reportPointId: string, status: Number) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/order/changeStatusReportPoint';
    return this.httpClient.post(url, {
      ReportPointId: reportPointId,
      Status: status,
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getMasterDataCreateOrderProcess() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/order/getMasterDataCreateOrderProcess';
    return this.httpClient.post(url, {}).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  createOrUpdateCustomerOrderProcess(orderProcess: OrderProcessEntityModel) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/order/createOrUpdateCustomerOrderProcess';
    return this.httpClient.post(url, {
      OrderProcess: orderProcess,
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getDetailOrderProcess(id: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/order/getDetailOrderProcess';
    return this.httpClient.post(url, {
      Id: id
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  updateOrderProcess(orderProcessDetailId: string, orderProcessId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/order/updateOrderProcess';
    return this.httpClient.post(url, {
      OrderProcessDetailId: orderProcessDetailId,
      OrderProcessId: orderProcessId,
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  checkTaskWithReportPointExtend(customerOrderTaskId: string, listEmpId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/order/checkTaskWithReportPointExtend';
    return this.httpClient.post(url, {
      CustomerOrderTaskId: customerOrderTaskId,
      ListEmpId: listEmpId,
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  deleteCustomerOrder(orderId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/order/deleteCustomerOrder';
    return this.httpClient.post(url, {
      OrderId: orderId
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

}
