using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TN.TNM.BusinessLogic.Interfaces.Order;
using TN.TNM.BusinessLogic.Messages.Requests.Order;
using TN.TNM.BusinessLogic.Messages.Responses.Order;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.Order;
using TN.TNM.DataAccess.Messages.Results.Order;

namespace TN.TNM.Api.Controllers
{
    public class CustomerOrderController : Controller
    {
        private readonly ICustomerOrder _iCustomerOrder;
        private readonly ICustomerOrderDataAccess _iCustomerOrderDataAccess;
        public CustomerOrderController (ICustomerOrder iCustomerOrder, ICustomerOrderDataAccess iCustomerOrderDataAccess)
        {
            this._iCustomerOrder = iCustomerOrder;
            this._iCustomerOrderDataAccess = iCustomerOrderDataAccess;
        }

        /// <summary>
        /// Get All Order
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/order/getAllOrder")]
        [Authorize(Policy = "Member")]
        public GetAllCustomerOrderResult GetAllCustomerOrder([FromBody]GetAllCustomerOrderParameter request)
        {
            return this._iCustomerOrderDataAccess.GetAllCustomerOrder(request);
        }
        /// <summary>
        /// Create Customer Order
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/order/createCustomerOrder")]
        [Authorize(Policy = "Member")]
        public CreateCustomerOrderResult CreateCustomerOrder([FromBody]CreateCustomerOrderParameter request)
        {
            return this._iCustomerOrderDataAccess.CreateCustomerOrder(request);
        }
        /// <summary>
        /// Update Customer Order
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/order/updateCustomerOrder")]
        [Authorize(Policy = "Member")]
        public UpdateCustomerOrderResult UpdateCustomerOrder([FromBody]UpdateCustomerOrderParameter request)
        {
            return this._iCustomerOrderDataAccess.UpdateCustomerOrder(request);
        }
        /// <summary>
        /// Get Customer Order By ID
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/order/getCustomerOrderByID")]
        [Authorize(Policy = "Member")]
        public GetCustomerOrderByIDResult GetCustomerOrderByID([FromBody]GetCustomerOrderByIDParameter request)
        {
            return this._iCustomerOrderDataAccess.GetCustomerOrderByID(request);
        }
        /// <summary>
        /// Export Pdf Customer Order
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/order/exportPdfCustomerOrder")]
        [Authorize(Policy = "Member")]
        public ExportCustomerOrderPDFResult ExportPdfCustomerOrder([FromBody]ExportCustomerOrderPDFParameter request)
        {
            return this._iCustomerOrderDataAccess.ExportPdfCustomerOrder(request);
        }

        /// <summary>
        /// Get Customer Order By Seller
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/order/getCustomerOrderBySeller")]
        [Authorize(Policy = "Member")]
        public GetCustomerOrderBySellerResult GetCustomerOrderBySeller([FromBody]GetCustomerOrderBySellerParameter request)
        {
            return this._iCustomerOrderDataAccess.GetCustomerOrderBySeller(request);
        }

        /// <summary>
        /// Get Employee List By OrganizationId
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/order/getEmployeeListByOrganizationId")]
        [Authorize(Policy = "Member")]
        public GetEmployeeListByOrganizationIdResult GetEmployeeListByOrganizationId([FromBody]GetEmployeeListByOrganizationIdParameter request)
        {
            return this._iCustomerOrderDataAccess.GetEmployeeListByOrganizationId(request);
        }

        /// <summary>
        /// Get Months List of revenue between months
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/order/getMonthsList")]
        [Authorize(Policy = "Member")]
        public GetMonthsListResult GetMonthsList([FromBody]GetMonthsListParameter request)
        {
            return this._iCustomerOrderDataAccess.GetMonthsList(request);
        }

        /// <summary>
        /// Get Product Category Group By Level
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/order/getProductCategoryGroupByLevel")]
        [Authorize(Policy = "Member")]
        public GetProductCategoryGroupByLevelResult GetProductCategoryGroupByLevel([FromBody]GetProductCategoryGroupByLevelParameter request)
        {
            return this._iCustomerOrderDataAccess.GetProductCategoryGroupByLevel(request);
        }

        /// <summary>
        /// Get Product Category Group By Manager
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/order/getProductCategoryGroupByManager")]
        [Authorize(Policy = "Member")]
        public GetProductCategoryGroupByManagerResult GetProductCategoryGroupByManager([FromBody] GetProductCategoryGroupByManagerParameter request)
        {
            return this._iCustomerOrderDataAccess.GetProductCategoryGroupByManager(request);
        }

        //
        [HttpPost]
        [Route("api/order/getMasterDataOrderSearch")]
        [Authorize(Policy = "Member")]
        public GetMasterDataOrderSearchResult GetMasterDataOrderSearch([FromBody] GetMasterDataOrderSearchParameter request)
        {
            return this._iCustomerOrderDataAccess.GetMasterDataOrderSearch(request);
        }

        //
        [HttpPost]
        [Route("api/order/searchOrder")]
        [Authorize(Policy = "Member")]
        public SearchOrderResult SearchOrder([FromBody] SearchOrderParameter request)
        {
            return this._iCustomerOrderDataAccess.SearchOrder(request);
        }

        //
        [HttpPost]
        [Route("api/order/getMasterDataOrderCreate")]
        [Authorize(Policy = "Member")]
        public GetMasterDataOrderCreateResult GetMasterDataOrderCreate([FromBody]GetMasterDataOrderCreateParameter request)
        {   var  reponse = this._iCustomerOrderDataAccess.GetMasterDataOrderCreate(request);
            return reponse;
        }

        //
        [HttpPost]
        [Route("api/order/getMasterDataOrderDetailDialog")]
        [Authorize(Policy = "Member")]
        public GetMasterDataOrderDetailDialogResult GetMasterDataOrderDetailDialog([FromBody] GetMasterDataOrderDetailDialogParameter request)
        {
            return this._iCustomerOrderDataAccess.GetMasterDataOrderDetailDialog(request);
        }

        //
        [HttpPost]
        [Route("api/order/getVendorByProductId")]
        [Authorize(Policy = "Member")]
        public GetVendorByProductIdResult GetVendorByProductId([FromBody]GetVendorByProductIdParameter request)
        {
            return this._iCustomerOrderDataAccess.GetVendorByProductId(request);
        }

        //
        [HttpPost]
        [Route("api/order/getMasterDataOrderDetail")]
        [Authorize(Policy = "Member")]
        public GetMasterDataOrderDetailResult GetMasterDataOrderDetail([FromBody]GetMasterDataOrderDetailParameter request)
        {
            return this._iCustomerOrderDataAccess.GetMasterDataOrderDetail(request);
        }

        //
        [HttpPost]
        [Route("api/order/deleteOrder")]
        [Authorize(Policy = "Member")]
        public DeleteOrderResult DeleteOrder([FromBody]DeleteOrderParameter request)
        {
            return this._iCustomerOrderDataAccess.DeleteOrder(request);
        }

        //
        [HttpPost]
        [Route("api/order/getDataDashboardHome")]
        [Authorize(Policy = "Member")]
        public GetDataDashboardHomeResult GetDataDashboardHome([FromBody]GetDataDashboardHomeParameter request)
        {
            return this._iCustomerOrderDataAccess.GetDataDashboardHome(request);
        }

        //
        [HttpPost]
        [Route("api/order/checkReceiptOrderHistory")]
        [Authorize(Policy = "Member")]
        public CheckReceiptOrderHistoryResult CheckReceiptOrderHistory([FromBody]CheckReceiptOrderHistoryParameter request)
        {
            return this._iCustomerOrderDataAccess.CheckReceiptOrderHistory(request);
        }

        //
        [HttpPost]
        [Route("api/order/checkBeforCreateOrUpdateOrder")]
        [Authorize(Policy = "Member")]
        public CheckBeforCreateOrUpdateOrderResult CheckBeforCreateOrUpdateOrder([FromBody]CheckBeforCreateOrUpdateOrderParameter request)
        {
            return this._iCustomerOrderDataAccess.CheckBeforCreateOrUpdateOrder(request);
        }

        //
        [HttpPost]
        [Route("api/order/updateStatusOrder")]
        [Authorize(Policy = "Member")]
        public UpdateCustomerOrderResult UpdateStatusOrder([FromBody]UpdateStatusOrderParameter request)
        {
            return this._iCustomerOrderDataAccess.UpdateStatusOrder(request);
        }

        //
        [HttpPost]
        [Route("api/order/searchProfitAccordingCustomers")]
        [Authorize(Policy = "Member")]
        public ProfitAccordingCustomersResult SearchProfitAccordingCustomers([FromBody]ProfitAccordingCustomersParameter request)
        {
            return this._iCustomerOrderDataAccess.SearchProfitAccordingCustomers(request);
        }

        //
        [HttpPost]
        [Route("api/order/getMasterDataOrderServiceCreate")]
        [Authorize(Policy = "Member")]
        public GetMasterDataOrderServiceCreateResult GetMasterDataOrderServiceCreate([FromBody]GetMasterDataOrderServiceCreateParameter request)
        {
            return this._iCustomerOrderDataAccess.GetMasterDataOrderServiceCreate(request);
        }

        //
        [HttpPost]
        [Route("api/order/createOrderService")]
        [Authorize(Policy = "Member")]
        public CreateOrderServiceResult CreateOrderService([FromBody]CreateOrderServiceParameter request)
        {
            return this._iCustomerOrderDataAccess.CreateOrderService(request);
        }

        //
        [HttpPost]
        [Route("api/order/getMasterDataPayOrderService")]
        [Authorize(Policy = "Member")]
        public GetMasterDataPayOrderServiceResult GetMasterDataPayOrderService([FromBody]GetMasterDataPayOrderServiceParameter request)
        {
            return this._iCustomerOrderDataAccess.GetMasterDataPayOrderService(request);
        }

        //
        [HttpPost]
        [Route("api/order/getListOrderByLocalPoint")]
        [Authorize(Policy = "Member")]
        public GetListOrderByLocalPointResult GetListOrderByLocalPoint([FromBody]GetListOrderByLocalPointParameter request)
        {
            return this._iCustomerOrderDataAccess.GetListOrderByLocalPoint(request);
        }

        //
        [HttpPost]
        [Route("api/order/payOrderByLocalPoint")]
        [Authorize(Policy = "Member")]
        public PayOrderByLocalPointResult PayOrderByLocalPoint([FromBody]PayOrderByLocalPointParameter request)
        {
            return this._iCustomerOrderDataAccess.PayOrderByLocalPoint(request);
        }

        //
        [HttpPost]
        [Route("api/order/checkExistsCustomerByPhone")]
        [Authorize(Policy = "Member")]
        public CheckExistsCustomerByPhoneResult CheckExistsCustomerByPhone([FromBody]CheckExistsCustomerByPhoneParameter request)
        {
            return this._iCustomerOrderDataAccess.CheckExistsCustomerByPhone(request);
        }

        //
        [HttpPost]
        [Route("api/order/refreshLocalPoint")]
        [Authorize(Policy = "Member")]
        public RefreshLocalPointResult RefreshLocalPoint([FromBody]RefreshLocalPointParameter request)
        {
            return this._iCustomerOrderDataAccess.RefreshLocalPoint(request);
        }

        //
        [HttpPost]
        [Route("api/order/getLocalPointByLocalAddress")]
        [Authorize(Policy = "Member")]
        public GetLocalPointByLocalAddressResult GetLocalPointByLocalAddress([FromBody]GetLocalPointByLocalAddressParameter request)
        {
            return this._iCustomerOrderDataAccess.GetLocalPointByLocalAddress(request);
        }

        [HttpPost]
        [Route("api/order/getDataSearchTopReVenue")]
        [Authorize(Policy = "Member")]
        public GetDataSearchTopReVenueResult GetDataSearchTopReVenue([FromBody]GetDataSearchTopReVenueParameter request)
        {
            return this._iCustomerOrderDataAccess.GetDataSearchTopReVenue(request);
        }

        [HttpPost]
        [Route("api/order/searchTopReVenue")]
        [Authorize(Policy = "Member")]
        public SearchTopReVenueResult SearchTopReVenue([FromBody]SearchTopReVenueParameter request)
        {
            return this._iCustomerOrderDataAccess.SearchTopReVenue(request);
        }

        [HttpPost]
        [Route("api/order/getDataSearchRevenueProduct")]
        [Authorize(Policy = "Member")]
        public GetDataSearchRevenueProductResult GetDataSearchRevenueProduct([FromBody]GetDataSearchRevenueProductParameter request)
        {
            return this._iCustomerOrderDataAccess.GetDataSearchRevenueProduct(request);
        }

        [HttpPost]
        [Route("api/order/searchRevenueProduct")]
        [Authorize(Policy = "Member")]
        public SearchRevenueProductResult SearchRevenueProduct([FromBody]SearchRevenueProductParameter request)
        {
            return this._iCustomerOrderDataAccess.SearchRevenueProduct(request);
        }

        //
        [HttpPost]
        [Route("api/order/getListOrderDetailByOrder")]
        [Authorize(Policy = "Member")]
        public GetListOrderDetailByOrderResult GetListOrderDetailByOrder([FromBody]GetListOrderDetailByOrderParameter request)
        {
            return this._iCustomerOrderDataAccess.GetListOrderDetailByOrder(request);
        }

        //
        [HttpPost]
        [Route("api/order/getListProductWasOrder")]
        [Authorize(Policy = "Member")]
        public GetListProductWasOrderResult GetListProductWasOrder([FromBody]GetListProductWasOrderParameter request)
        {
            return this._iCustomerOrderDataAccess.GetListProductWasOrder(request);
        }

        //
        [HttpPost]
        [Route("api/order/updateCustomerService")]
        [Authorize(Policy = "Member")]
        public UpdateCustomerServiceResult UpdateCustomerService([FromBody]UpdateCustomerServiceParameter request)
        {
            return this._iCustomerOrderDataAccess.UpdateCustomerService(request);
        }

        [HttpPost]
        [Route("api/order/getDataProfitByCustomer")]
        [Authorize(Policy = "Member")]
        public GetDataProfitByCustomerResult GetDataProfitByCustomer([FromBody]GetDataProfitByCustomerParameter request)
        {
            return this._iCustomerOrderDataAccess.GetDataProfitByCustomer(request);
        }

        [HttpPost]
        [Route("api/order/searchProfitCustomer")]
        [Authorize(Policy = "Member")]
        public SearchProfitCustomerResult SearchProfitCustomer([FromBody]SearchProfitCustomerParameter request)
        {
            return this._iCustomerOrderDataAccess.SearchProfitCustomer(request);
        }

        [HttpPost]
        [Route("api/order/getInventoryNumber")]
        [Authorize(Policy = "Member")]
        public GetInventoryNumberResult GetInventoryNumber([FromBody] GetInventoryNumberParameter request)
        {
            return this._iCustomerOrderDataAccess.GetInventoryNumber(request);
        }

        [HttpPost]
        [Route("api/order/CheckTonKhoSanPham")]
        [Authorize(Policy = "Member")]
        public CheckTonKhoSanPhamResult CheckTonKhoSanPham([FromBody] CheckTonKhoSanPhamParameter request)
        {
            return this._iCustomerOrderDataAccess.CheckTonKhoSanPham(request);
        }
        [HttpPost]
        [Route("api/order/UpdateCustomerOrderTonKho")]
        [Authorize(Policy = "Member")]
        public UpdateCustomerOrderTonKhoResult UpdateCustomerOrderTonKho([FromBody] UpdateCustomerOrderTonKhoParameter request)
        {
            return this._iCustomerOrderDataAccess.UpdateCustomerOrderTonKho(request);
        }

        [HttpPost]
        [Route("api/order/SearchOptionOfPacketService")]
        [AllowAnonymous]
        public Task<SearchOptionOfPacketServiceResult> SearchOptionOfPacketService([FromBody] SearchOptionOfPacketServiceParameter request)
        {
            return this._iCustomerOrderDataAccess.SearchOptionOfPacketService(request);
        }

        [HttpPost]
        [Route("api/order/changeStatusCustomerOrder")]
        [Authorize(Policy = "Member")]
        public Task<ChangeStatusCustomerOrderResult> ChangeStatusCustomerOrder([FromBody] ChangeStatusCustomerOrderParameter request)
        {
            return this._iCustomerOrderDataAccess.ChangeStatusCustomerOrder(request);
        }

        [HttpPost]
        [Route("api/order/getMasterDataCreateOrderAction")]
        [Authorize(Policy = "Member")]
        public GetMasterDataCreateOrderActionResult GetMasterDataCreateOrderAction([FromBody] GetMasterDataCreateOrderActionParameter request)
        {
            return this._iCustomerOrderDataAccess.GetMasterDataCreateOrderAction(request);
        }

        [HttpPost]
        [Route("api/order/changeCustomerOrder")]
        [Authorize(Policy = "Member")]
        public ChangeCustomerOrderResult ChangeCustomerOrder([FromBody] ChangeCustomerOrderParameter request)
        {
            return this._iCustomerOrderDataAccess.ChangeCustomerOrder(request);
        }

        [HttpPost]
        [Route("api/order/getVendorAndEmployeeOfPacket")]
        [Authorize(Policy = "Member")]
        public GetVendorAndEmployeeOfPacketResult GetVendorAndEmployeeOfPacket([FromBody] GetVendorAndEmployeeOfPacketParameter request)
        {
            return this._iCustomerOrderDataAccess.GetVendorAndEmployeeOfPacket(request);
        }

        [HttpPost]
        [Route("api/order/createOrUpdateCustomerOrderAction")]
        [Authorize(Policy = "Member")]
        public CreateOrUpdateCustomerOrderActionResult CreateOrUpdateCustomerOrderAction([FromBody] CreateOrUpdateCustomerOrderActionParameter request)
        {
            return this._iCustomerOrderDataAccess.CreateOrUpdateCustomerOrderAction(request);
        }

        [HttpPost]
        [Route("api/order/getMasterDataOrderActionDetail")]
        [Authorize(Policy = "Member")]
        public GetMasterDataOrderActionDetailResult GetMasterDataOrderActionDetail([FromBody] GetMasterDataOrderActionDetailParameter request)
        {
            return this._iCustomerOrderDataAccess.GetMasterDataOrderActionDetail(request);
        }

        [HttpPost]
        [Route("api/order/createOrUpdateCustomerReportPoint")]
        [Authorize(Policy = "Member")]
        public CreateOrUpdateCustomerReportPointResult CreateOrUpdateCustomerReportPoint([FromBody] CreateOrUpdateCustomerReportPointParameter request)
        {
            return this._iCustomerOrderDataAccess.CreateOrUpdateCustomerReportPoint(request);
        }

        [HttpPost]
        [Route("api/order/getOptionAndEmpOfOrderAction")]
        [Authorize(Policy = "Member")]
        public GetOptionAndEmpOfOrderActionResult GetOptionAndEmpOfOrderAction([FromBody] GetOptionAndEmpOfOrderActionParameter request)
        {
            return this._iCustomerOrderDataAccess.GetOptionAndEmpOfOrderAction(request);
        }

        [HttpPost]
        [Route("api/order/changeStatusReportPoint")]
        [Authorize(Policy = "Member")]
        public ChangeStatusReportPointResult ChangeStatusReportPoint([FromBody] ChangeStatusReportPointParameter request)
        {
            return this._iCustomerOrderDataAccess.ChangeStatusReportPoint(request);
        }

        [HttpPost]
        [Route("api/order/getMasterDataCreateOrderProcess")]
        [Authorize(Policy = "Member")]
        public GetMasterDataCreateOrderProcessResult GetMasterDataCreateOrderProcess([FromBody] GetMasterDataCreateOrderProcessParameter request)
        {
            return this._iCustomerOrderDataAccess.GetMasterDataCreateOrderProcess(request);
        }

        [HttpPost]
        [Route("api/order/createOrUpdateCustomerOrderProcess")]
        [Authorize(Policy = "Member")]
        public CreateOrUpdateCustomerOrderProcessResult CreateOrUpdateCustomerOrderProcess([FromBody] CreateOrUpdateCustomerOrderProcessParameter request)
        {
            return this._iCustomerOrderDataAccess.CreateOrUpdateCustomerOrderProcess(request);
        }

        [HttpPost]
        [Route("api/order/getDetailOrderProcess")]
        [Authorize(Policy = "Member")]
        public GetDetailOrderProcessResult GetDetailOrderProcess([FromBody] GetDetailOrderProcessParameter request)
        {
            return this._iCustomerOrderDataAccess.GetDetailOrderProcess(request);
        }

        [HttpPost]
        [Route("api/order/updateOrderProcess")]
        [Authorize(Policy = "Member")]
        public UpdateOrderProcessResult UpdateOrderProcess([FromBody] UpdateOrderProcessParameter request)
        {
            return this._iCustomerOrderDataAccess.UpdateOrderProcess(request);
        }

        [HttpPost]
        [Route("api/order/getListOrderOfCus")]
        [Authorize(Policy = "Member")]
        public GetListOrderOfCusResult GetListOrderOfCus([FromBody] GetListOrderOfCusParameter request)
        {
            return this._iCustomerOrderDataAccess.GetListOrderOfCus(request);
        }

        [HttpPost]
        [Route("api/order/ratingOrder")]
        [Authorize(Policy = "Member")]
        public RatingOrderResult RatingOrder([FromBody] RatingOrderParameter request)
        {
            return this._iCustomerOrderDataAccess.RatingOrder(request);
        }

        [HttpPost]
        [Route("api/order/deleteOrderOptionByCus")]
        [Authorize(Policy = "Member")]
        public DeleteOrderOptionByCusResult DeleteOrderOptionByCus([FromBody] DeleteOrderOptionByCusParameter request)
        {
            return this._iCustomerOrderDataAccess.DeleteOrderOptionByCus(request);
        }

        [HttpPost]
        [Route("api/order/updateCustomerOrderDetailExtend")]
        [Authorize(Policy = "Member")]
        public UpdateCustomerOrderDetailExtendResult UpdateCustomerOrderDetailExtend([FromBody] UpdateCustomerOrderDetailExtendParameter request)
        {
            return this._iCustomerOrderDataAccess.UpdateCustomerOrderDetailExtend(request);
        }

        [HttpPost]
        [Route("api/order/checkTaskWithReportPointExtend")]
        [Authorize(Policy = "Member")]
        public CheckTaskWithReportPointExtendResult CheckTaskWithReportPointExtend([FromBody] CheckTaskWithReportPointExtendParameter request)
        {
            return this._iCustomerOrderDataAccess.CheckTaskWithReportPointExtend(request);
        }


        [HttpPost]
        [Route("api/order/DeleteCustomerOrder")]
        [Authorize(Policy = "Member")]
        public DeleteOrderResult DeleteCustomerOrder([FromBody] DeleteOrderParameter request)
        {
            return this._iCustomerOrderDataAccess.DeleteCustomerOrder(request);
        }

    }
}