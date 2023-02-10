using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TN.TNM.BusinessLogic.Interfaces.Manufacture;
using TN.TNM.BusinessLogic.Messages.Requests.Manufacture;
using TN.TNM.BusinessLogic.Messages.Responses.Manufacture;

namespace TN.TNM.Api.Controllers
{
    public class ManufactureController : Controller
    {
        private readonly IManufacture iManufacture;
        public ManufactureController(IManufacture _iManufacture)
        {
            this.iManufacture = _iManufacture;
        }

        [HttpPost]
        [Route("api/manufacture/getMasterDataCreateProductOrderWorkflow")]
        [Authorize(Policy = "Member")]
        public GetMasterDataCreateProductOrderWorkflowResponse GetMasterDataCreateProductOrderWorkflow([FromBody]GetMasterDataCreateProductOrderWorkflowRequest request)
        {
            return this.iManufacture.GetMasterDataCreateProductOrderWorkflow(request);
        }

        [HttpPost]
        [Route("api/manufacture/createProductOrderWorkflow")]
        [Authorize(Policy = "Member")]
        public CreateProductOrderWorkflowResponse CreateProductOrderWorkflow([FromBody]CreateProductOrderWorkflowRequest request)
        {
            return this.iManufacture.CreateProductOrderWorkflow(request);
        }

        [HttpPost]
        [Route("api/manufacture/checkIsDefaultProductOrderWorkflow")]
        [Authorize(Policy = "Member")]
        public CheckIsDefaultProductOrderWorkflowResponse CheckIsDefaultProductOrderWorkflow([FromBody]CheckIsDefaultProductOrderWorkflowRequest request)
        {
            return this.iManufacture.CheckIsDefaultProductOrderWorkflow(request);
        }

        [HttpPost]
        [Route("api/manufacture/getProductOrderWorkflowById")]
        [Authorize(Policy = "Member")]
        public GetProductOrderWorkflowByIdResponse GetProductOrderWorkflowById([FromBody]GetProductOrderWorkflowByIdRequest request)
        {
            return this.iManufacture.GetProductOrderWorkflowById(request);
        }

        [HttpPost]
        [Route("api/manufacture/updateProductOrderWorkflow")]
        [Authorize(Policy = "Member")]
        public UpdateProductOrderWorkflowResponse UpdateProductOrderWorkflow([FromBody]UpdateProductOrderWorkflowRequest request)
        {
            return this.iManufacture.UpdateProductOrderWorkflow(request);
        }

        [HttpPost]
        [Route("api/manufacture/getMasterDataSearchProductOrderWorkflow")]
        [Authorize(Policy = "Member")]
        public GetMasterDataSearchProductOrderWorkflowResponse GetMasterDataSearchProductOrderWorkflow([FromBody]GetMasterDataSearchProductOrderWorkflowRequest request)
        {
            return this.iManufacture.GetMasterDataSearchProductOrderWorkflow(request);
        }

        [HttpPost]
        [Route("api/manufacture/searchProductOrderWorkflow")]
        [Authorize(Policy = "Member")]
        public SearchProductOrderWorkflowResponse SearchProductOrderWorkflow([FromBody]SearchProductOrderWorkflowRequest request)
        {
            return this.iManufacture.SearchProductOrderWorkflow(request);
        }

        [HttpPost]
        [Route("api/manufacture/updateProductOrderWorkflowDefault")]
        [Authorize(Policy = "Member")]
        public UpdateProductOrderWorkflowDefaultResponse UpdateProductOrderWorkflowDefault([FromBody]UpdateProductOrderWorkflowDefaultRequest request)
        {
            return this.iManufacture.UpdateProductOrderWorkflowDefault(request);
        }

        [HttpPost]
        [Route("api/manufacture/updateProductOrderWorkflowActive")]
        [Authorize(Policy = "Member")]
        public UpdateProductOrderWorkflowActiveResponse UpdateProductOrderWorkflowActive([FromBody]UpdateProductOrderWorkflowActiveRequest request)
        {
            return this.iManufacture.UpdateProductOrderWorkflowActive(request);
        }

        [HttpPost]
        [Route("api/manufacture/getMasterDataCreateTechniqueRequest")]
        [Authorize(Policy = "Member")]
        public GetMasterDataCreateTechniqueRequestResponse GetMasterDataCreateTechniqueRequest([FromBody]GetMasterDataCreateTechniqueRequestRequest request)
        {
            return this.iManufacture.GetMasterDataCreateTechniqueRequest(request);
        }

        [HttpPost]
        [Route("api/manufacture/createTechniqueRequest")]
        [Authorize(Policy = "Member")]
        public CreateTechniqueRequestResponse CreateTechniqueRequest([FromBody]CreateTechniqueRequestRequest request)
        {
            return this.iManufacture.CreateTechniqueRequest(request);
        }

        [HttpPost]
        [Route("api/manufacture/getMasterDataSearchTechniqueRequest")]
        [Authorize(Policy = "Member")]
        public GetMasterDataSearchTechniqueRequestResponse GetMasterDataSearchTechniqueRequest([FromBody]GetMasterDataSearchTechniqueRequestRequest request)
        {
            return this.iManufacture.GetMasterDataSearchTechniqueRequest(request);
        }

        [HttpPost]
        [Route("api/manufacture/searchTechniqueRequest")]
        [Authorize(Policy = "Member")]
        public SearchTechniqueRequestResponse SearchTechniqueRequest([FromBody]SearchTechniqueRequestRequest request)
        {
            return this.iManufacture.SearchTechniqueRequest(request);
        }

        [HttpPost]
        [Route("api/manufacture/getMasterDataCreateProductionOrder")]
        [Authorize(Policy = "Member")]
        public GetMasterDataCreateProductionOrderResponse GetMasterDataCreateProductionOrder([FromBody]GetMasterDataCreateProductionOrderRequest request)
        {
            return this.iManufacture.GetMasterDataCreateProductionOrder(request);
        }

        [HttpPost]
        [Route("api/manufacture/getMasterDataCreateTotalProductionOrder")]
        [Authorize(Policy = "Member")]
        public GetMasterDataCreateTotalProductionOrderResponse GetMasterDataCreateTotalProductionOrder([FromBody]GetMasterDataCreateTotalProductionOrderRequest request)
        {
            return this.iManufacture.GetMasterDataCreateTotalProductionOrder(request);
        }

        [HttpPost]
        [Route("api/manufacture/getMasterDataProductionOrderDetail")]
        [Authorize(Policy = "Member")]
        public GetMasterDataProductionOrderDetailResponse GetMasterDataProductionOrderDetail([FromBody]GetMasterDataProductionOrderDetailRequest request)
        {
            return this.iManufacture.GetMasterDataProductionOrderDetail(request);
        }

        [HttpPost]
        [Route("api/manufacture/getAllProductionOrder")]
        [Authorize(Policy = "Member")]
        public GetAllProductionOrderResponse GetAllProductionOrder([FromBody]GetAllProductionOrderRequest request)
        {
            return this.iManufacture.GetAllProductionOrder(request);
        }

        [HttpPost]
        [Route("api/manufacture/getTechniqueRequestById")]
        [Authorize(Policy = "Member")]
        public GetTechniqueRequestByIdResponse GetTechniqueRequestById([FromBody]GetTechniqueRequestByIdRequest request)
        {
            return this.iManufacture.GetTechniqueRequestById(request);
        }

        [HttpPost]
        [Route("api/manufacture/updateTechniqueRequest")]
        [Authorize(Policy = "Member")]
        public UpdateTechniqueRequestResponse UpdateTechniqueRequest([FromBody]UpdateTechniqueRequestRequest request)
        {
            return this.iManufacture.UpdateTechniqueRequest(request);
        }

        [HttpPost]
        [Route("api/manufacture/getMasterDataListSearchProductionOrder")]
        [Authorize(Policy = "Member")]
        public GetMasterDataListSearchProductionOrderResponse GetMasterDataListSearchProductionOrder([FromBody]GetMasterDataListSearchProductionOrderRequest request)
        {
            return this.iManufacture.GetMasterDataListSearchProductionOrder(request);
        }

        [HttpPost]
        [Route("api/manufacture/updateProductionOrderEspecially")]
        [Authorize(Policy = "Member")]
        public UpdateProductionOrderEspeciallyResponse UpdateProductionOrderEspecially([FromBody]UpdateProductionOrderEspeciallyRequest request)
        {
            return this.iManufacture.UpdateProductionOrderEspecially(request);
        }

        [HttpPost]
        [Route("api/manufacture/createTotalProductionOrder")]
        [Authorize(Policy = "Member")]
        public CreateTotalProductionOrderResponse CreateTotalProductionOrder([FromBody]CreateTotalProductionOrderRequest request)
        {
            return this.iManufacture.CreateTotalProductionOrder(request);
        }

        [HttpPost]
        [Route("api/manufacture/getTotalProductionOrderById")]
        [Authorize(Policy = "Member")]
        public GetTotalProductionOrderByIdResponse GetTotalProductionOrderById([FromBody]GetTotalProductionOrderByIdRequest request)
        {
            return this.iManufacture.GetTotalProductionOrderById(request);
        }

        [HttpPost]
        [Route("api/manufacture/updateTotalProductionOrder")]
        [Authorize(Policy = "Member")]
        public UpdateTotalProductionOrderResponse UpdateTotalProductionOrder([FromBody]UpdateTotalProductionOrderRequest request)
        {
            return this.iManufacture.UpdateTotalProductionOrder(request);
        }

        [HttpPost]
        [Route("api/manufacture/getMasterDataAddProductionOrderDialog")]
        [Authorize(Policy = "Member")]
        public GetMasterDataAddProductionOrderDialogResponse GetMasterDataAddProductionOrderDialog([FromBody]GetMasterDataAddProductionOrderDialogRequest request)
        {
            return this.iManufacture.GetMasterDataAddProductionOrderDialog(request);
        }

        [HttpPost]
        [Route("api/manufacture/getMasterDataSearchTotalProductionOrder")]
        [Authorize(Policy = "Member")]
        public GetMasterDataSearchTotalProductionOrderResponse GetMasterDataSearchTotalProductionOrder([FromBody]GetMasterDataSearchTotalProductionOrderRequest request)
        {
            return this.iManufacture.GetMasterDataSearchTotalProductionOrder(request);
        }

        [HttpPost]
        [Route("api/manufacture/searchTotalProductionOrder")]
        [Authorize(Policy = "Member")]
        public SearchTotalProductionOrderResponse SearchTotalProductionOrder([FromBody]SearchTotalProductionOrderRequest request)
        {
            return this.iManufacture.SearchTotalProductionOrder(request);
        }

        [HttpPost]
        [Route("api/manufacture/getTrackProduction")]
        [Authorize(Policy = "Member")]
        public GetTrackProductionResponse GetTrackProduction([FromBody]GetTrackProductionRequest request)
        {
            return this.iManufacture.GetTrackProduction(request);
        }

        [HttpPost]
        [Route("api/manufacture/createProductionOrder")]
        [Authorize(Policy = "Member")]
        public CreateProductionOrderResponse CreateProductionOrder([FromBody]CreateProductionOrderRequest request)
        {
            return this.iManufacture.CreateProductionOrder(request);
        }

        [HttpPost]
        [Route("api/manufacture/updateStatusItemStop")]
        [Authorize(Policy = "Member")]
        public UpdateStatusItemStopResponse UpdateStatusItemStop([FromBody]UpdateStatusItemStopRequest request)
        {
            return this.iManufacture.UpdateStatusItemStop(request);
        }

        [HttpPost]
        [Route("api/manufacture/updateStatusItemCancel")]
        [Authorize(Policy = "Member")]
        public UpdateStatusItemCancelResponse UpdateStatusItemCancel([FromBody]UpdateStatusItemCancelRequest request)
        {
            return this.iManufacture.UpdateStatusItemCancel(request);
        }

        [HttpPost]
        [Route("api/manufacture/updateProductionOrder")]
        [Authorize(Policy = "Member")]
        public UpdateProductionOrderResponse UpdateProductionOrder([FromBody]UpdateProductionOrderRequest request)
        {
            return this.iManufacture.UpdateProductionOrder(request);
        }

        [HttpPost]
        [Route("api/manufacture/updateStatusItemWorking")]
        [Authorize(Policy = "Member")]
        public UpdateStatusItemWorkingResponse UpdateStatusItemWorking([FromBody]UpdateStatusItemWorkingRequest request)
        {
            return this.iManufacture.UpdateStatusItemWorking(request);
        }

        [HttpPost]
        [Route("api/manufacture/plusItem")]
        [Authorize(Policy = "Member")]
        public PlusItemResponse PlusItem([FromBody]PlusItemRequest request)
        {
            return this.iManufacture.PlusItem(request);
        }

        [HttpPost]
        [Route("api/manufacture/minusItem")]
        [Authorize(Policy = "Member")]
        public MinusItemResponse MinusItem([FromBody]MinusItemRequest request)
        {
            return this.iManufacture.MinusItem(request);
        }

        [HttpPost]
        [Route("api/manufacture/updateItemInProduction")]
        [Authorize(Policy = "Member")]
        public UpdateItemInProductionResponse UpdateItemInProduction([FromBody]UpdateItemInProductionRequest request)
        {
            return this.iManufacture.UpdateItemInProduction(request);
        }

        [HttpPost]
        [Route("api/manufacture/getDataReportQuanlityControl")]
        [Authorize(Policy = "Member")]
        public GetDataReportQuanlityControlResponse GetDataReportQuanlityControl([FromBody]GetDataReportQuanlityControlRequest request)
        {
            return this.iManufacture.GetDataReportQuanlityControl(request);
        }

        [HttpPost]
        [Route("api/manufacture/searchQuanlityControlReport")]
        [Authorize(Policy = "Member")]
        public SearchQuanlityControlReportResponse SearchQuanlityControlReport([FromBody]SearchQuanlityControlReportRequest request)
        {
            return this.iManufacture.SearchQuanlityControlReport(request);
        }

        [HttpPost]
        [Route("api/manufacture/deleteItemInProduction")]
        [Authorize(Policy = "Member")]
        public DeleteItemInProductionResponse DeleteItemInProduction([FromBody]DeleteItemInProductionRequest request)
        {
            return this.iManufacture.DeleteItemInProduction(request);
        }

        [HttpPost]
        [Route("api/manufacture/createItemInProduction")]
        [Authorize(Policy = "Member")]
        public CreateItemInProductionResponse CreateItemInProduction([FromBody]CreateItemInProductionRequest request)
        {
            return this.iManufacture.CreateItemInProduction(request);
        }

        [HttpPost]
        [Route("api/manufacture/updateWorkFlowForProductionOrder")]
        [Authorize(Policy = "Member")]
        public UpdateWorkFlowForProductionOrderResponse UpdateWorkFlowForProductionOrder([FromBody]UpdateWorkFlowForProductionOrderRequest request)
        {
            return this.iManufacture.UpdateWorkFlowForProductionOrder(request);
        }

        [HttpPost]
        [Route("api/manufacture/getMasterDataListItemDialog")]
        [Authorize(Policy = "Member")]
        public GetMasterDataListItemDialogResponse GetMasterDataListItemDialog([FromBody]GetMasterDataListItemDialogRequest request)
        {
            return this.iManufacture.GetMasterDataListItemDialog(request);
        }

        [HttpPost]
        [Route("api/manufacture/updateProductionOrderHistoryNoteQcAndErroType")]
        [Authorize(Policy = "Member")]
        public UpdateProductionOrderHistoryNoteQcAndErroTypeResponse UpdateProductionOrderHistoryNoteQcAndErroType([FromBody]UpdateProductionOrderHistoryNoteQcAndErroTypeRequest request)
        {
            return this.iManufacture.UpdateProductionOrderHistoryNoteQcAndErroType(request);
        }

        [HttpPost]
        [Route("api/manufacture/exportManufactureReport")]
        [Authorize(Policy = "Member")]
        public ExportManufactureReportResponse ExportManufactureReport([FromBody]ExportManufactureReportRequest request)
        {
            return this.iManufacture.ExportManufactureReport(request);
        }

        [HttpPost]
        [Route("api/manufacture/getMasterDataDialogListProductOrder")]
        [Authorize(Policy = "Member")]
        public GetMasterDataDialogListProductOrderResponse GetMasterDataDialogListProductOrder([FromBody]GetMasterDataDialogListProductOrderRequest request)
        {
            return this.iManufacture.GetMasterDataDialogListProductOrder(request);
        }

        [HttpPost]
        [Route("api/manufacture/createProductionOrderAdditional")]
        [Authorize(Policy = "Member")]
        public CreateProductionOrderAdditionalResponse CreateProductionOrderAdditional([FromBody]CreateProductionOrderAdditionalRequest request)
        {
            return this.iManufacture.CreateProductionOrderAdditional(request);
        }

        [HttpPost]
        [Route("api/manufacture/getDataReportManufacture")]
        [Authorize(Policy = "Member")]
        public GetDataReportManufactureResponse GetDataReportManufacture([FromBody]GetDataReportManufactureRequest request)
        {
            return this.iManufacture.GetDataReportManufacture(request);
        }

        [HttpPost]
        [Route("api/manufacture/getReportManuFactureByDay")]
        [Authorize(Policy = "Member")]
        public GetReportManuFactureByDayResponse GetReportManuFactureByDay([FromBody]GetReportManuFactureByDayRequest request)
        {
            return this.iManufacture.GetReportManuFactureByDay(request);
        }

        [HttpPost]
        [Route("api/manufacture/getReportManuFactureByMonth")]
        [Authorize(Policy = "Member")]
        public GetReportManuFactureByMonthResponse GetReportManuFactureByMonth([FromBody]GetReportManuFactureByMonthRequest request)
        {
            return this.iManufacture.GetReportManuFactureByMonth(request);
        }

        [HttpPost]
        [Route("api/manufacture/getReportManuFactureByMonthAdditional")]
        [Authorize(Policy = "Member")]
        public GetReportManuFactureByMonthAdditionalResponse GetReportManuFactureByMonthAdditional([FromBody]GetReportManuFactureByMonthAdditionalRequest request)
        {
            return this.iManufacture.GetReportManuFactureByMonthAdditional(request);
        }

        [HttpPost]
        [Route("api/manufacture/getReportManuFactureByYear")]
        [Authorize(Policy = "Member")]
        public GetReportManuFactureByYearResponse GetReportManuFactureByYear([FromBody]GetReportManuFactureByYearRequest request)
        {
            return this.iManufacture.GetReportManuFactureByYear(request);
        }

        [HttpPost]
        [Route("api/manufacture/getDataExportTrackProduction")]
        [Authorize(Policy = "Member")]
        public GetDataExportTrackProductionResponse GetDataExportTrackProduction([FromBody]GetDataExportTrackProductionRequest request)
        {
            return this.iManufacture.GetDataExportTrackProduction(request);
        }
        
        [HttpPost]
        [Route("api/manufacture/getListChildrentItem")]
        [Authorize(Policy = "Member")]
        public GetListChildrentItemResponse GetListChildrentItem([FromBody]GetListChildrentItemRequest request)
        {
            return this.iManufacture.GetListChildrentItem(request);
        }

        [HttpPost]
        [Route("api/manufacture/getDataDasboardManufacture")]
        [Authorize(Policy = "Member")]
        public GetDataDasboardManufactureResponse GetDataDasboardManufacture([FromBody]GetDataDasboardManufactureRequest request)
        {
            return this.iManufacture.GetDataDasboardManufacture(request);
        }
        
        [HttpPost]
        [Route("api/manufacture/getMasterDataTrackProduction")]
        [Authorize(Policy = "Member")]
        public GetMasterDataTrackProductionResponse GetMasterDataTrackProduction([FromBody]GetMasterDataTrackProductionRequest request)
        {
            return this.iManufacture.GetMasterDataTrackProduction(request);
        }
        
        [HttpPost]
        [Route("api/manufacture/createRememberItem")]
        [Authorize(Policy = "Member")]
        public CreateRememberItemResponse CreateRememberItem([FromBody]CreateRememberItemRequest request)
        {
            return this.iManufacture.CreateRememberItem(request);
        }

        [HttpPost]
        [Route("api/manufacture/getTrackProductionReport")]
        [Authorize(Policy = "Member")]
        public GetTrackProductionReportResponse GetTrackProductionReport([FromBody]GetTrackProductionReportRequest request)
        {
            return this.iManufacture.GetTrackProductionReport(request);
        }
        
        [HttpPost]
        [Route("api/manufacture/getMasterDataViewRememberItemDialog")]
        [Authorize(Policy = "Member")]
        public GetMasterDataViewRememberItemDialogResponse GetMasterDataViewRememberItemDialog([FromBody]GetMasterDataViewRememberItemDialogRequest request)
        {
            return this.iManufacture.GetMasterDataViewRememberItemDialog(request);
        }
        
        [HttpPost]
        [Route("api/manufacture/updateRememberItem")]
        [Authorize(Policy = "Member")]
        public UpdateRememberItemResponse UpdateRememberItem([FromBody]UpdateRememberItemRequest request)
        {
            return this.iManufacture.UpdateRememberItem(request);
        }

        [HttpPost]
        [Route("api/manufacture/createAllBTP")]
        [Authorize(Policy = "Member")]
        public CreateAllBTPResponse CreateAllBTP([FromBody]CreateAllBTPRequest request)
        {
            return this.iManufacture.CreateAllBTP(request);
        }

        [HttpPost]
        [Route("api/manufacture/updateProductionOrderNote")]
        [Authorize(Policy = "Member")]
        public UpdateProductionOrderNoteResponse UpdateProductionOrderNote([FromBody]UpdateProductionOrderNoteRequest request)
        {
            return this.iManufacture.UpdateProductionOrderNote(request);
        }
        
        [HttpPost]
        [Route("api/manufacture/plusListItem")]
        [Authorize(Policy = "Member")]
        public PlusListItemResponse PlusListItem([FromBody]PlusListItemRequest request)
        {
            return this.iManufacture.PlusListItem(request);
        }
        
        [HttpPost]
        [Route("api/manufacture/changeProductGroupCodeByItem")]
        [Authorize(Policy = "Member")]
        public ChangeProductGroupCodeByItemResponse ChangeProductGroupCodeByItem([FromBody]ChangeProductGroupCodeByItemRequest request)
        {
            return this.iManufacture.ChangeProductGroupCodeByItem(request);
        }
        
        [HttpPost]
        [Route("api/manufacture/minusQuantityForItem")]
        [Authorize(Policy = "Member")]
        public MinusQuantityForItemResponse MinusQuantityForItem([FromBody]MinusQuantityForItemRequest request)
        {
            return this.iManufacture.MinusQuantityForItem(request);
        }
        
        [HttpPost]
        [Route("api/manufacture/getListItemChange")]
        [Authorize(Policy = "Member")]
        public GetListItemChangeResponse GetListItemChange([FromBody]GetListItemChangeRequest request)
        {
            return this.iManufacture.GetListItemChange(request);
        }
        
        [HttpPost]
        [Route("api/manufacture/saveCatHa")]
        [Authorize(Policy = "Member")]
        public SaveCatHaResponse SaveCatHa([FromBody]SaveCatHaRequest request)
        {
            return this.iManufacture.SaveCatHa(request);
        }
        
        [HttpPost]
        [Route("api/manufacture/getListWorkflowById")]
        [Authorize(Policy = "Member")]
        public GetListWorkflowByIdResponse GetListWorkflowById([FromBody]GetListWorkflowByIdRequest request)
        {
            return this.iManufacture.GetListWorkflowById(request);
        }
    }
}