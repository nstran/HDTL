using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Messages.Requests.Manufacture;
using TN.TNM.BusinessLogic.Messages.Responses.Manufacture;

namespace TN.TNM.BusinessLogic.Interfaces.Manufacture
{
    public interface IManufacture
    {
        GetMasterDataCreateProductOrderWorkflowResponse GetMasterDataCreateProductOrderWorkflow(
            GetMasterDataCreateProductOrderWorkflowRequest request);

        CreateProductOrderWorkflowResponse CreateProductOrderWorkflow(CreateProductOrderWorkflowRequest request);

        CheckIsDefaultProductOrderWorkflowResponse CheckIsDefaultProductOrderWorkflow(
            CheckIsDefaultProductOrderWorkflowRequest request);

        GetProductOrderWorkflowByIdResponse GetProductOrderWorkflowById(GetProductOrderWorkflowByIdRequest request);
        UpdateProductOrderWorkflowResponse UpdateProductOrderWorkflow(UpdateProductOrderWorkflowRequest request);

        GetMasterDataSearchProductOrderWorkflowResponse GetMasterDataSearchProductOrderWorkflow(
            GetMasterDataSearchProductOrderWorkflowRequest request);

        SearchProductOrderWorkflowResponse SearchProductOrderWorkflow(SearchProductOrderWorkflowRequest request);

        UpdateProductOrderWorkflowDefaultResponse UpdateProductOrderWorkflowDefault(
            UpdateProductOrderWorkflowDefaultRequest request);

        UpdateProductOrderWorkflowActiveResponse UpdateProductOrderWorkflowActive(
            UpdateProductOrderWorkflowActiveRequest request);

        GetMasterDataCreateTechniqueRequestResponse GetMasterDataCreateTechniqueRequest(
            GetMasterDataCreateTechniqueRequestRequest request);

        CreateTechniqueRequestResponse CreateTechniqueRequest(CreateTechniqueRequestRequest request);

        GetMasterDataSearchTechniqueRequestResponse GetMasterDataSearchTechniqueRequest(
            GetMasterDataSearchTechniqueRequestRequest request);

        SearchTechniqueRequestResponse SearchTechniqueRequest(SearchTechniqueRequestRequest request);

        GetMasterDataCreateProductionOrderResponse GetMasterDataCreateProductionOrder(
            GetMasterDataCreateProductionOrderRequest request);

        GetMasterDataCreateTotalProductionOrderResponse GetMasterDataCreateTotalProductionOrder(
            GetMasterDataCreateTotalProductionOrderRequest request);

        GetMasterDataProductionOrderDetailResponse GetMasterDataProductionOrderDetail(
            GetMasterDataProductionOrderDetailRequest request);

        GetAllProductionOrderResponse GetAllProductionOrder(GetAllProductionOrderRequest request);

        GetTechniqueRequestByIdResponse GetTechniqueRequestById(GetTechniqueRequestByIdRequest request);
        UpdateTechniqueRequestResponse UpdateTechniqueRequest(UpdateTechniqueRequestRequest request);

        GetMasterDataListSearchProductionOrderResponse GetMasterDataListSearchProductionOrder(
            GetMasterDataListSearchProductionOrderRequest request);

        UpdateProductionOrderEspeciallyResponse UpdateProductionOrderEspecially(
            UpdateProductionOrderEspeciallyRequest request);

        CreateTotalProductionOrderResponse CreateTotalProductionOrder(CreateTotalProductionOrderRequest request);
        GetTotalProductionOrderByIdResponse GetTotalProductionOrderById(GetTotalProductionOrderByIdRequest request);
        UpdateTotalProductionOrderResponse UpdateTotalProductionOrder(UpdateTotalProductionOrderRequest request);

        GetMasterDataAddProductionOrderDialogResponse GetMasterDataAddProductionOrderDialog(
            GetMasterDataAddProductionOrderDialogRequest request);

        GetMasterDataSearchTotalProductionOrderResponse GetMasterDataSearchTotalProductionOrder(
            GetMasterDataSearchTotalProductionOrderRequest request);

        SearchTotalProductionOrderResponse SearchTotalProductionOrder(SearchTotalProductionOrderRequest request);
        GetTrackProductionResponse GetTrackProduction(GetTrackProductionRequest request);

        CreateProductionOrderResponse CreateProductionOrder(CreateProductionOrderRequest request);

        UpdateProductionOrderResponse UpdateProductionOrder(UpdateProductionOrderRequest request);

        UpdateStatusItemStopResponse UpdateStatusItemStop(UpdateStatusItemStopRequest request);

        UpdateStatusItemCancelResponse UpdateStatusItemCancel (UpdateStatusItemCancelRequest request);

        UpdateStatusItemWorkingResponse UpdateStatusItemWorking(UpdateStatusItemWorkingRequest request);
        PlusItemResponse PlusItem(PlusItemRequest request);
        MinusItemResponse MinusItem(MinusItemRequest request);

        UpdateItemInProductionResponse UpdateItemInProduction(UpdateItemInProductionRequest request);

        GetDataReportQuanlityControlResponse GetDataReportQuanlityControl(GetDataReportQuanlityControlRequest request);
        SearchQuanlityControlReportResponse SearchQuanlityControlReport(SearchQuanlityControlReportRequest request);

        DeleteItemInProductionResponse DeleteItemInProduction(DeleteItemInProductionRequest request);

        CreateItemInProductionResponse CreateItemInProduction(CreateItemInProductionRequest request);

        UpdateWorkFlowForProductionOrderResponse UpdateWorkFlowForProductionOrder(UpdateWorkFlowForProductionOrderRequest request);

        GetMasterDataListItemDialogResponse GetMasterDataListItemDialog(GetMasterDataListItemDialogRequest request);
        UpdateProductionOrderHistoryNoteQcAndErroTypeResponse UpdateProductionOrderHistoryNoteQcAndErroType(UpdateProductionOrderHistoryNoteQcAndErroTypeRequest request);
        ExportManufactureReportResponse ExportManufactureReport(ExportManufactureReportRequest request);

        GetMasterDataDialogListProductOrderResponse GetMasterDataDialogListProductOrder(GetMasterDataDialogListProductOrderRequest request);

        CreateProductionOrderAdditionalResponse CreateProductionOrderAdditional(CreateProductionOrderAdditionalRequest request);
        GetDataReportManufactureResponse GetDataReportManufacture(GetDataReportManufactureRequest request);
        GetReportManuFactureByDayResponse GetReportManuFactureByDay(GetReportManuFactureByDayRequest request);
        GetReportManuFactureByMonthResponse GetReportManuFactureByMonth(GetReportManuFactureByMonthRequest request);
        GetReportManuFactureByYearResponse GetReportManuFactureByYear(GetReportManuFactureByYearRequest request);
        GetReportManuFactureByMonthAdditionalResponse GetReportManuFactureByMonthAdditional(GetReportManuFactureByMonthAdditionalRequest request);
        GetDataExportTrackProductionResponse GetDataExportTrackProduction(GetDataExportTrackProductionRequest request);
        GetListChildrentItemResponse GetListChildrentItem(GetListChildrentItemRequest request);
        GetDataDasboardManufactureResponse GetDataDasboardManufacture(GetDataDasboardManufactureRequest request);

        GetMasterDataTrackProductionResponse GetMasterDataTrackProduction(GetMasterDataTrackProductionRequest request);
        CreateRememberItemResponse CreateRememberItem(CreateRememberItemRequest request);
        GetTrackProductionReportResponse GetTrackProductionReport(GetTrackProductionReportRequest request);

        GetMasterDataViewRememberItemDialogResponse GetMasterDataViewRememberItemDialog(
            GetMasterDataViewRememberItemDialogRequest request);

        UpdateRememberItemResponse UpdateRememberItem(UpdateRememberItemRequest request);

        CreateAllBTPResponse CreateAllBTP(CreateAllBTPRequest request);
        UpdateProductionOrderNoteResponse UpdateProductionOrderNote(UpdateProductionOrderNoteRequest request);
        PlusListItemResponse PlusListItem(PlusListItemRequest request);

        ChangeProductGroupCodeByItemResponse ChangeProductGroupCodeByItem(ChangeProductGroupCodeByItemRequest request);
        MinusQuantityForItemResponse MinusQuantityForItem(MinusQuantityForItemRequest request);
        GetListItemChangeResponse GetListItemChange(GetListItemChangeRequest request);
        SaveCatHaResponse SaveCatHa(SaveCatHaRequest request);
        GetListWorkflowByIdResponse GetListWorkflowById(GetListWorkflowByIdRequest request);
    }
}
