using TN.TNM.DataAccess.Messages.Parameters.Manufacture;
using TN.TNM.DataAccess.Messages.Results.Manufacture;

namespace TN.TNM.DataAccess.Interfaces
{
    public interface IManufactureDataAccess
    {
        GetMasterDataCreateProductOrderWorkflowResult GetMasterDataCreateProductOrderWorkflow(
            GetMasterDataCreateProductOrderWorkflowParameter parameter);

        CreateProductOrderWorkflowResult CreateProductOrderWorkflow(CreateProductOrderWorkflowParameter parameter);

        CheckIsDefaultProductOrderWorkflowResult CheckIsDefaultProductOrderWorkflow(
            CheckIsDefaultProductOrderWorkflowParameter parameter);

        GetProductOrderWorkflowByIdResult GetProductOrderWorkflowById(GetProductOrderWorkflowByIdParameter parameter);
        UpdateProductOrderWorkflowResult UpdateProductOrderWorkflow(UpdateProductOrderWorkflowParameter parameter);

        GetMasterDataSearchProductOrderWorkflowResult GetMasterDataSearchProductOrderWorkflow(
            GetMasterDataSearchProductOrderWorkflowParameter parameter);

        SearchProductOrderWorkflowResult SearchProductOrderWorkflow(SearchProductOrderWorkflowParameter parameter);

        UpdateProductOrderWorkflowDefaultResult UpdateProductOrderWorkflowDefault(
            UpdateProductOrderWorkflowDefaultParameter parameter);

        UpdateProductOrderWorkflowActiveResult UpdateProductOrderWorkflowActive(
            UpdateProductOrderWorkflowActiveParameter parameter);

        GetMasterDataCreateTechniqueRequestResult GetMasterDataCreateTechniqueRequest(
            GetMasterDataCreateTechniqueRequestParameter parameter);

        CreateTechniqueRequestResult CreateTechniqueRequest(CreateTechniqueRequestParameter parameter);

        GetMasterDataSearchTechniqueRequestResult GetMasterDataSearchTechniqueRequest(
            GetMasterDataSearchTechniqueRequestParameter parameter);

        SearchTechniqueRequestResult SearchTechniqueRequest(SearchTechniqueRequestParameter parameter);

        GetMasterDataCreateProductionOrderResult GetMasterDataCreateProductionOrder(
            GetMasterDataCreateProductionOrderParameter parameter);

        GetMasterDataCreateTotalProductionOrderResult GetMasterDataCreateTotalProductionOrder(
            GetMasterDataCreateTotalProductionOrderParameter parameter);

        GetMasterDataProductionOrderDetailResult GetMasterDataProductionOrderDetail(
            GetMasterDataProductionOrderDetailParameter parameter);

        GetAllProductionOrderResult GetAllProductionOrder(GetAllProductionOrderParameter parameter);

        GetMasterDataListSearchProductionOrderResult GetMasterDataListSearchProductionOrder(
            GetMasterDataListSearchProductionOrderParameter parameter);

        GetTechniqueRequestByIdResult GetTechniqueRequestById(GetTechniqueRequestByIdParameter parameter);
        UpdateTechniqueRequestResult UpdateTechniqueRequest(UpdateTechniqueRequestParameter parameter);

        UpdateProductionOrderEspeciallyResult UpdateProductionOrderEspecially(
            UpdateProductionOrderEspeciallyParameter parameter);

        CreateTotalProductionOrderResult CreateTotalProductionOrder(CreateTotalProductionOrderParameter parameter);
        GetTotalProductionOrderByIdResult GetTotalProductionOrderById(GetTotalProductionOrderByIdParameter parameter);
        UpdateTotalProductionOrderResult UpdateTotalProductionOrder(UpdateTotalProductionOrderParameter parameter);

        GetMasterDataAddProductionOrderDialogResult GetMasterDataAddProductionOrderDialog(
            GetMasterDataAddProductionOrderDialogParameter parameter);

        GetMasterDataSearchTotalProductionOrderResult GetMasterDataSearchTotalProductionOrder(
            GetMasterDataSearchTotalProductionOrderParameter parameter);

        SearchTotalProductionOrderResult SearchTotalProductionOrder(SearchTotalProductionOrderParameter parameter);
        GetTrackProductionResult GetTrackProduction(GetTrackProductionParameter parameter);

        CreateProductionOrderResult CreateProductionOrder(CreateProductionOrderParameter parameter);
        PlusItemResult PlusItem(PlusItemParameter parameter);
        MinusItemResult MinusItem(MinusItemParameter parameter);

        UpdateProductionOrderResult UpdateProductionOrder(UpdateProductionOrderParameter parameter);

        UpdateStatusItemStopResult UpdateStatusItemStop(UpdateStatusItemStopParameter parameter);

        UpdateStatusItemCancelResult UpdateStatusItemCancel(UpdateStatusItemCancelParameter parameter);

        UpdateStatusItemWorkingResult UpdateStatusItemWorking(UpdateStatusItemWorkingParameter parameter);

        UpdateItemInProductionResult UpdateItemInProduction(UpdateItemInProductionParameter parameter);

        GetDataReportQuanlityControlResult GetDataReportQuanlityControl(GetDataReportQuanlityControlParameter parameter);
        SearchQuanlityControlReportResult SearchQuanlityControlReport(SearchQuanlityControlReportParameter parameter);

        DeleteItemInProductionResult DeleteItemInProduction(DeleteItemInProductionParameter parameter);

        CreateItemInProductionResult CreateItemInProduction(CreateItemInProductionParameter parameter);

        UpdateWorkFlowForProductionOrderResult UpdateWorkFlowForProductionOrder(UpdateWorkFlowForProductionOrderParameter parameter);

        GetMasterDataListItemDialogResult GetMasterDataListItemDialog(GetMasterDataListItemDialogParameter parameter);
        UpdateProductionOrderHistoryNoteQcAndErroTypeResult UpdateProductionOrderHistoryNoteQcAndErroType(UpdateProductionOrderHistoryNoteQcAndErroTypeParameter parameter);
        ExportManufactureReportResult ExportManufactureReport(ExportManufactureReportParameter parameter);
        GetDataReportManufactureResult GetDataReportManufacture(GetDataReportManufactureParameter parameter);
        GetMasterDataDialogListProductOrderResult GetMasterDataDialogListProductOrder(GetMasterDataDialogListProductOrderParameter parameter);
        CreateProductionOrderAdditionalResult CreateProductionOrderAdditional(CreateProductionOrderAdditionalParameter parameter);
        GetReportManuFactureByDayResult GetReportManuFactureByDay(GetReportManuFactureByDayParameter parameter);
        GetReportManuFactureByMonthResult GetReportManuFactureByMonth(GetReportManuFactureByMonthParameter parameter);
        GetReportManuFactureByYearResult GetReportManuFactureByYear(GetReportManuFactureByYearParameter parameter);
        GetReportManuFactureByMonthAdditionalResult GetReportManuFactureByMonthAdditional(GetReportManuFactureByMonthAdditionalParameter parameter);
        GetDataExportTrackProductionResult GetDataExportTrackProduction(GetDataExportTrackProductionParameter parameter);
        GetListChildrentItemResult GetListChildrentItem(GetListChildrentItemParameter parameter);
        GetDataDasboardManufactureResult GetDataDasboardManufacture(GetDataDasboardManufactureParameter paramter);

        GetMasterDataTrackProductionResult
            GetMasterDataTrackProduction(GetMasterDataTrackProductionParameter parameter);

        CreateRememberItemResult CreateRememberItem(CreateRememberItemParameter parameter);
        GetTrackProductionReportResult GetTrackProductionReport(GetTrackProductionReportParameter parameter);

        GetMasterDataViewRememberItemDialogResult GetMasterDataViewRememberItemDialog(
            GetMasterDataViewRememberItemDialogParameter parameter);

        UpdateRememberItemResult UpdateRememberItem(UpdateRememberItemParameter parameter);

        CreateAllBTPResult CreateAllBTP(CreateAllBTPParameter parameter);
        UpdateProductionOrderNoteResult UpdateProductionOrderNote(UpdateProductionOrderNoteParameter parameter);
        PlusListItemResult PlusListItem(PlusListItemParameter parameter);

        ChangeProductGroupCodeByItemResult
            ChangeProductGroupCodeByItem(ChangeProductGroupCodeByItemParameter parameter);

        ChangeGroupCodeForListItemResult ChangeGroupCodeForListItem(ChangeGroupCodeForListItemParameter parameter);
        MinusQuantityForItemResult MinusQuantityForItem(MinusQuantityForItemParameter parameter);
        GetListItemChangeResult GetListItemChange(GetListItemChangeParameter parameter);
        SaveCatHaResult SaveCatHa(SaveCatHaParameter parameter);
        GetListWorkflowByIdResult GetListWorkflowById(GetListWorkflowByIdParameter parameter);
    }
}
