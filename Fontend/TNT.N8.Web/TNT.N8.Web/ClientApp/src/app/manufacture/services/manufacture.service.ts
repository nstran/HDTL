import { Injectable } from '@angular/core';
import { HttpClient } from "@angular/common/http";
import { map } from 'rxjs/operators';

interface ProductOrderWorkflow {
  productOrderWorkflowId: string;
  code: string;
  name: string;
  isDefault: boolean;
  description: string;
  active: boolean;
  createdDate: Date;
  createdById: string;
}

class TechniqueRequest {
  techniqueRequestId: string;
  parentId: string;
  organizationId: string;
  techniqueName: string;
  description: string;
  active: boolean;
  createdDate: Date;
  createdById: string;
  techniqueRequestCode: string;
}

class OrderTechniqueMapping {
  orderTechniqueMappingId: string;
  productOrderWorkflowId: string;
  techniqueRequestId: string;
  techniqueOrder: number;
  rate: any;
  isDefault: boolean;
  createdDate: Date;
  createdById: string;
  techniqueName: string;
}

class ProductionOrder {
  productionOrderId: string;
  productionOrderCode: string;
  orderId: string;
  customerName: string;
  orderCustomerCode: string;
  placeOfDelivery: string;
  receivedDate: Date;
  startDate: Date;
  endDate: Date;
  note: string;
  noteTechnique: string;
  especially: boolean;
  statusId: string;
  createdDate: Date;
  createdById: string;
  customerNumber: string;
  parentId:string;
}

class Technique {
  techniqueRequestId: string;
  parentId: string;
  organizationId: string;
  techniqueName: string;
  description: string;
  active: boolean;
  createdDate: Date;
  createdById: string;
  techniqueOrder: number;
  techniqueValue: any;
  parentName: string;
  rate: number;
}

class TechniqueRequestMaping {
  productionOrderMappingId: string;
  index: string;
  parentId: string;
  productionOrderId: string;
  productId: string;
  productName: string;
  productColor: string;
  productColorCode: string;
  productThickness: number;
  productLength: number;
  productWidth: number;
  quantity: number;
  totalArea: number;
  techniqueDescription: string;
  statusId: string;
  createdDate: Date;
  createdById: string;
  productOrderWorkflowName: string;
  productCode:string;
  productOrderWorkflowId: string;
  listTechnique: Array<Technique>;
  parentIndex:string;
  borehole:number;
  hole: number;
  productGroupCode :string;
  description: string;// Mô tả lỗi
  nameNest: string;  // Tên tổ
  productionOrderHistoryId: string // Id bản lịch sử
  techniqueOrder:number // Order tiến trình lỗi
  originalId: string; // Tiến trình gây lỗi
  grind: number;  //Mài
  stt: number;  //Số thứ tự
  note: string;
}
class TotalProductionOrder {
  totalProductionOrderId: string;
  code: string;
  periodId: string;
  startDate: Date;
  createdDate: Date;
  createdById: string;
}

class ProductionOrderHistory {
  productionOrderHistoryId: string;
  totalProductionOrderId: string;
  parentId: string;
  parentType: boolean;
  productionOrderId: string;
  productionOrderMappingId: string;
  techniqueRequestId: string;
  calculatorType: boolean;
  isError: boolean;
  isErrorPre: boolean;
  originalId: string;
  description: string;
  quantityUnitErr: number;
  isParent: string;
  isSubParent: string;
  idChildrent: string;
  parentPartId: string;
  parentExtendId: string;
  remainQuantity: number;
  createdDate: Date;
  createdById: string;
}

class RememberItem {
  rememberItemId: string;
  productionOrderId: string;
  productionOrderMappingId: string;
  quantity: number;
  description: string;
  isCheck: boolean;
  createdDate: Date;
  createdById: string;
}

class ItemInfor {
  productionOrderMappingId: string;
  productionOrderId: string;
  productName: string;
  productColor: string;
  productThickness: any;
  productLength: number;
  productWidth: number;
  quantity: number;
  totalArea: number;
  hole: number;
  borehole: number;
  techniqueDescription: string;
  statusId: string;
  statusCode: string;
  statusName: string;
  productOrderWorkflowId: string;
  productOrderWorkflowName: string;
  productGroupCode: string;
  parentPartId: string;
  isAddItem: boolean;
  isCreated: boolean;
  isOriginal: boolean;
}

@Injectable()
export class ManufactureService {

  constructor(private httpClient: HttpClient) { }

  getMasterDataCreateProductOrderWorkflow() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/manufacture/getMasterDataCreateProductOrderWorkflow';
    return this.httpClient.post(url, {}).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  createProductOrderWorkflow(productOrderWorkflow: ProductOrderWorkflow) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/manufacture/createProductOrderWorkflow';
    return this.httpClient.post(url, { ProductOrderWorkflow: productOrderWorkflow }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  checkIsDefaultProductOrderWorkflow() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/manufacture/checkIsDefaultProductOrderWorkflow';
    return this.httpClient.post(url, {}).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getProductOrderWorkflowById(productOrderWorkflowId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/manufacture/getProductOrderWorkflowById';
    return this.httpClient.post(url, { ProductOrderWorkflowId: productOrderWorkflowId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  updateProductOrderWorkflow(productOrderWorkflow: ProductOrderWorkflow, listOrderTechniqueMapping: Array<OrderTechniqueMapping>) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/manufacture/updateProductOrderWorkflow';
    return this.httpClient.post(url, {
      ProductOrderWorkflow: productOrderWorkflow,
      ListOrderTechniqueMapping: listOrderTechniqueMapping
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getMasterDataSearchProductOrderWorkflow() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/manufacture/getMasterDataSearchProductOrderWorkflow';
    return this.httpClient.post(url, {}).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  searchProductOrderWorkflow(code: string, name: string, active: boolean, description: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/manufacture/searchProductOrderWorkflow';
    return this.httpClient.post(url, {
      Code: code,
      Name: name,
      Active: active,
      Description: description
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  updateProductOrderWorkflowDefault(productOrderWorkflowId: string, isDefault: boolean) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/manufacture/updateProductOrderWorkflowDefault';
    return this.httpClient.post(url, {
      ProductOrderWorkflowId: productOrderWorkflowId,
      IsDefault: isDefault
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  updateProductOrderWorkflowActive(productOrderWorkflowId: string, active: boolean) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/manufacture/updateProductOrderWorkflowActive';
    return this.httpClient.post(url, {
      ProductOrderWorkflowId: productOrderWorkflowId,
      Active: active
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getMasterDataCreateTechniqueRequest() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/manufacture/getMasterDataCreateTechniqueRequest';
    return this.httpClient.post(url, {}).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  createTechniqueRequest(techniqueRequest: TechniqueRequest) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/manufacture/createTechniqueRequest';
    return this.httpClient.post(url, { TechniqueRequest: techniqueRequest }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getMasterDataSearchTechniqueRequest() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/manufacture/getMasterDataSearchTechniqueRequest';
    return this.httpClient.post(url, {}).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  searchTechniqueRequest(techniqueName: string, organizationId: string, parentId: string, description: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/manufacture/searchTechniqueRequest';
    return this.httpClient.post(url, {
      TechniqueName: techniqueName,
      OrganizationId: organizationId,
      ParentId: parentId,
      Description: description
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getMasterDataCreateProductionOrder(orderId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/manufacture/getMasterDataCreateProductionOrder';
    return this.httpClient.post(url, {
      OrderId: orderId
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getMasterDataProductionOrderDetail(productionOrderId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/manufacture/getMasterDataProductionOrderDetail';
    return this.httpClient.post(url, {
      ProductionOrderId: productionOrderId
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getAllProductionOrder(productionOrderCode: string, customerName: string, totalProductionOrderCode: string,
    startDate: Date, endDate: Date, listStatusId: Array<string>, techniqueDescription: string, 
    type:boolean,listOrgan:Array<string>, isError: boolean) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/manufacture/getAllProductionOrder';
    return this.httpClient.post(url, {
      ProductionOrderCode: productionOrderCode,
      CustomerName: customerName,
      TotalProductionOrderCode: totalProductionOrderCode,
      StartDate: startDate,
      EndDate: endDate,
      ListStatusId: listStatusId,
      TechniqueDescription: techniqueDescription,
      Type:type,
      ListOrgan: listOrgan,
      IsError: isError
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getMasterDataCreateTotalProductionOrder() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/manufacture/getMasterDataCreateTotalProductionOrder';
    return this.httpClient.post(url, {}).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getTechniqueRequestById(techniqueRequestId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/manufacture/getTechniqueRequestById';
    return this.httpClient.post(url, { TechniqueRequestId: techniqueRequestId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  updateTechniqueRequest(techniqueRequest: TechniqueRequest) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/manufacture/updateTechniqueRequest';
    return this.httpClient.post(url, { TechniqueRequest: techniqueRequest }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getMasterDataListSearchProductionOrder() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/manufacture/getMasterDataListSearchProductionOrder';
    return this.httpClient.post(url, {}).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  updateProductionOrderEspecially(productionOrderId: string, especially: boolean) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/manufacture/updateProductionOrderEspecially';
    return this.httpClient.post(url, {
      ProductionOrderId:productionOrderId,
      Especially: especially
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  createTotalProductionOrder(totalProductionOrder: TotalProductionOrder, listProductionOrderId: Array<string>) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/manufacture/createTotalProductionOrder';
    return this.httpClient.post(url, {
      TotalProductionOrder: totalProductionOrder,
      ListProductionOrderId: listProductionOrderId
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getTotalProductionOrderById(totalProductionOrderId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/manufacture/getTotalProductionOrderById';
    return this.httpClient.post(url, { TotalProductionOrderId: totalProductionOrderId }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  updateTotalProductionOrder(totalProductionOrder: TotalProductionOrder, listProductionOrderId: Array<string>, oldStatusCodeFe: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/manufacture/updateTotalProductionOrder';
    return this.httpClient.post(url, {
      TotalProductionOrder: totalProductionOrder,
      ListProductionOrderId: listProductionOrderId,
      OldStatusCodeFe: oldStatusCodeFe
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getMasterDataAddProductionOrderDialog(listIgnore: Array<string>) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/manufacture/getMasterDataAddProductionOrderDialog';
    return this.httpClient.post(url, {
      ListIgnore: listIgnore
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getMasterDataSearchTotalProductionOrder() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/manufacture/getMasterDataSearchTotalProductionOrder';
    return this.httpClient.post(url, {}).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  searchTotalProductionOrder(code: string, startDate: Date, totalQuantity: number, 
    totalArea: number, minEndDate: Date, maxEndDate: Date, listStatusId: Array<string>, firstNumber: number, rows: number) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/manufacture/searchTotalProductionOrder';
    return this.httpClient.post(url, {
      Code: code,
      StartDate: startDate,
      TotalQuantity: totalQuantity,
      TotalArea: totalArea,
      MinEndDate: minEndDate,
      MaxEndDate: maxEndDate,
      ListStatusId: listStatusId,
      FirstNumber: firstNumber,
      Rows: rows
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getTrackProduction(listProductionOrderId: Array<string>, endDate: Date, productThickness: number, productName: string, 
    fromDate: Date, toDate: Date, listStatusItem: Array<string>, 
    selectedProductLengthValue: number, selectedProductWidthValue: number,
    firstNumber: number, rows: number) {

    const url = localStorage.getItem('ApiEndPoint') + '/api/manufacture/getTrackProduction';
    return this.httpClient.post(url, { 
      ListProductionOrderId: listProductionOrderId,
      EndDate: endDate,
      ProductThickness: productThickness,
      ProductName: productName,
      FromDate: fromDate,
      ToDate: toDate,
      ListStatusItem: listStatusItem,
      ProductLength: selectedProductLengthValue,
      ProductWidth: selectedProductWidthValue,
      FirstNumber: firstNumber,
      Rows: rows
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getTrackProductionAsync() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/manufacture/getTrackProduction';
    return new Promise((resolve, reject) => {
      return this.httpClient.post(url, { }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  createProductionOrder(productionOrder:ProductionOrder,listProduct:Array<TechniqueRequestMaping>,listProductChildren:Array<TechniqueRequestMaping>,listProductChildrenChildren:Array<TechniqueRequestMaping>) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/manufacture/createProductionOrder';
    return this.httpClient.post(url, {
      ProductionOrder : productionOrder,
      ListProduct: listProduct,
      ListProductChildren: listProductChildren,
      ListProductChildrenChildren: listProductChildrenChildren
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  plusItem(productionOrderHistory: ProductionOrderHistory) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/manufacture/plusItem';
    return this.httpClient.post(url, {
      ProductionOrderHistory: productionOrderHistory
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  minusItem(productionOrderHistory: ProductionOrderHistory, listItemId: Array<string>) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/manufacture/minusItem';
    return this.httpClient.post(url, {
      ProductionOrderHistory: productionOrderHistory,
      ListItemId: listItemId
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  updateStatusItemStop(productionOrderMappingId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/manufacture/updateStatusItemStop';
    return this.httpClient.post(url, {
      ProductionOrderMappingId: productionOrderMappingId
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  updateStatusItemCancel(productionOrderMappingId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/manufacture/updateStatusItemCancel';
    return this.httpClient.post(url, {
      ProductionOrderMappingId: productionOrderMappingId
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  updateProductionOrder(productionOrder:ProductionOrder) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/manufacture/updateProductionOrder';
    return this.httpClient.post(url, {
      ProductionOrder : productionOrder
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  updateStatusItemWorking(productionOrderMappingId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/manufacture/updateStatusItemWorking';
    return this.httpClient.post(url, {
      ProductionOrderMappingId : productionOrderMappingId
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getDataReportQuanlityControl() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/manufacture/getDataReportQuanlityControl';
    return new Promise ((resolve, reject) => {
      return this.httpClient.post(url, {}).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  searchQuanlityControlReport(listTechniqueRequestId: Array<string>, listThicknessOptionId: Array<number>, fromDate: Date, toDate: Date) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/manufacture/searchQuanlityControlReport';
    return new Promise ((resolve, reject) => {
      return this.httpClient.post(url, {
        ListTechniqueRequestId: listTechniqueRequestId,
        ListThicknessOptionId: listThicknessOptionId,
        FromDate: fromDate,
        ToDate:  toDate
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  exportManufactureReport(fromDate: Date, toDate: Date) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/manufacture/exportManufactureReport';
    return new Promise ((resolve, reject) => {
      return this.httpClient.post(url, {
        FromDate: fromDate,
        ToDate:  toDate
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  updateItemInProduction(productionOrderMapping: TechniqueRequestMaping) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/manufacture/updateItemInProduction';
    return this.httpClient.post(url, {
      ProductItem : productionOrderMapping
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }
  
  deleteItemInProduction(productionOrderMappingId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/manufacture/deleteItemInProduction';
    return this.httpClient.post(url, {
      ProductionOrderMappingId : productionOrderMappingId
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }
  
  createItemInProduction(productionOrderMapping: TechniqueRequestMaping) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/manufacture/createItemInProduction';
    return this.httpClient.post(url, {
      ProductItem : productionOrderMapping
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }
  
  updateWorkFlowForProductionOrder(listProductItem: Array<TechniqueRequestMaping>) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/manufacture/updateWorkFlowForProductionOrder';
    return this.httpClient.post(url, {
      ListProductItem : listProductItem
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getMasterDataListItemDialog(productionOrderId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/manufacture/getMasterDataListItemDialog';
    return this.httpClient.post(url, {
      ProductionOrderId: productionOrderId
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }
  
  updateProductionOrderHistoryNoteQcAndErroType(productionOrderHistoryId: string, noteQc: string, errorType: string, isUpdateNoteQc: boolean, isUpdateErrorType: boolean) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/manufacture/updateProductionOrderHistoryNoteQcAndErroType';
    return new Promise ((resolve, reject) => {
      return this.httpClient.post(url, {
        ProductionOrderHistoryId : productionOrderHistoryId,
        NoteQc : noteQc,
        ErrorType : errorType,
        IsUpdateNoteQc : isUpdateNoteQc,
        IsUpdateErrorType : isUpdateErrorType
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getMasterDataDialogListProductOrder(productionOrderId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/manufacture/getMasterDataDialogListProductOrder';
    return this.httpClient.post(url, {
      ProductionOrderId: productionOrderId
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  createProductionOrderAdditional(listProduct: Array<TechniqueRequestMaping>) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/manufacture/createProductionOrderAdditional';
    return this.httpClient.post(url, {
      ListProduct: listProduct
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getDataReportManufacture() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/manufacture/getDataReportManufacture';
    return new Promise ((resolve, reject) => {
      return this.httpClient.post(url, {}).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getReportManuFactureByDay(techniqueRequestId: string,  shift: number,fromDate: Date, toDate: Date) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/manufacture/getReportManuFactureByDay';
    return new Promise ((resolve, reject) => {
      return this.httpClient.post(url, {
        TechniqueRequestId: techniqueRequestId,
        Shift: shift,
        FromDate: fromDate,
        ToDate: toDate,
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getReportManuFactureByMonth(techniqueRequestId: string, fromDate: Date, toDate: Date) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/manufacture/getReportManuFactureByMonth';
    return new Promise ((resolve, reject) => {
      return this.httpClient.post(url, {
        TechniqueRequestId: techniqueRequestId,
        FromDate: fromDate,
        ToDate: toDate,
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getReportManuFactureByMonthAdditional(techniqueRequestId: string, fromDate: Date, toDate: Date) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/manufacture/getReportManuFactureByMonthAdditional';
    return new Promise ((resolve, reject) => {
      return this.httpClient.post(url, {
        TechniqueRequestId: techniqueRequestId,
        FromDate: fromDate,
        ToDate: toDate,
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getReportManuFactureByYear(techniqueRequestId: string, fromDate: Date, toDate: Date) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/manufacture/getReportManuFactureByYear';
    return new Promise ((resolve, reject) => {
      return this.httpClient.post(url, {
        TechniqueRequestId: techniqueRequestId,
        FromDate: fromDate,
        ToDate: toDate,
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getDataExportTrackProduction(organizationCode: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/manufacture/getDataExportTrackProduction';
    return new Promise ((resolve, reject) => {
      return this.httpClient.post(url, { OrganizationCode: organizationCode}).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getListChildrentItem(productionOrderMappingId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/manufacture/getListChildrentItem';
    return this.httpClient.post(url, {
      ProductionOrderMappingId: productionOrderMappingId
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getTrackProductionReport(organizationCode: string, listProductionOrderId: Array<string>) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/manufacture/getTrackProductionReport';
    return new Promise ((resolve, reject) => {
      return this.httpClient.post(url, {
        OrganizationCode: organizationCode,
        ListProductionOrderId: listProductionOrderId
      }).toPromise()
        .then((response: Response) => {
          resolve(response);
        });
    });
  }

  getMasterDataTrackProduction() {
    const url = localStorage.getItem('ApiEndPoint') + '/api/manufacture/getMasterDataTrackProduction';
    return this.httpClient.post(url, {
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  createRememberItem(rememberItem: RememberItem) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/manufacture/createRememberItem';
    return this.httpClient.post(url, {
      RememberItem: rememberItem
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getMasterDataViewRememberItemDialog(productionOrderCode: string, productName: string, 
      productThickness: number, fromDate: Date, toDate: Date) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/manufacture/getMasterDataViewRememberItemDialog';
    return this.httpClient.post(url, {
      ProductionOrderCode: productionOrderCode,
      ProductName: productName,
      ProductThickness: productThickness,
      FromDate: fromDate,
      ToDate: toDate
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  updateRememberItem(rememberItem: RememberItem) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/manufacture/updateRememberItem';
    return this.httpClient.post(url, {
      RememberItem: rememberItem
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  createAllBTP(productionOrderId: string,btp:TechniqueRequestMaping,listBTP1: Array<TechniqueRequestMaping>,listBTP2: Array<TechniqueRequestMaping>) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/manufacture/createAllBTP';
    return this.httpClient.post(url, {
      ProductionOrderId: productionOrderId,
      ListBTP1:listBTP1,
      ListBTP2:listBTP2,
      BTP:btp
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  updateProductionOrderNote(productionOrderId: string, note: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/manufacture/updateProductionOrderNote';
    return this.httpClient.post(url, {
      ProductionOrderId: productionOrderId,
      Note: note
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  plusListItem(listItem: Array<ProductionOrderHistory>) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/manufacture/plusListItem';
    return this.httpClient.post(url, {
      ListItem: listItem
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  changeProductGroupCodeByItem(productionOrderMappingId: string, productGroupCode: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/manufacture/changeProductGroupCodeByItem';
    return this.httpClient.post(url, {
      ProductionOrderMappingId: productionOrderMappingId,
      ProductGroupCode: productGroupCode
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  changeGroupCodeForListItem(productionOrderId: string, code_11: string, code_111: string, code_112: string, 
    code_12: string, code_121: string, code_122: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/manufacture/changeGroupCodeForListItem';
    return this.httpClient.post(url, {
      ProductionOrderId: productionOrderId,
      Code_11: code_11,
      Code_111: code_111,
      Code_112: code_112,
      Code_12: code_12,
      Code_121: code_121,
      Code_122: code_122
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  minusQuantityForItem(productionOrderMappingId: string, minusType: number, quantity: number) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/manufacture/minusQuantityForItem';
    return this.httpClient.post(url, {
      ProductionOrderMappingId: productionOrderMappingId,
      MinusType: minusType,
      Quantity: quantity
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getListItemChange(productionOrderId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/manufacture/getListItemChange';
    return this.httpClient.post(url, {
      ProductionOrderId: productionOrderId
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  saveCatHa(listItemInfor: Array<ItemInfor>) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/manufacture/saveCatHa';
    return this.httpClient.post(url, {
      ListItemInfor: listItemInfor
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }

  getListWorkflowById(productOrderWorkflowId: string) {
    const url = localStorage.getItem('ApiEndPoint') + '/api/manufacture/getListWorkflowById';
    return this.httpClient.post(url, {
      ProductOrderWorkflowId: productOrderWorkflowId
    }).pipe(
      map((response: Response) => {
        return response;
      }));
  }
}