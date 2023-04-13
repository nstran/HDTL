using System.Threading.Tasks;
using TN.TNM.DataAccess.Messages.Parameters.Order;
using TN.TNM.DataAccess.Messages.Results.Order;

namespace TN.TNM.DataAccess.Interfaces
{
    public interface ICustomerOrderDataAccess
    {
        GetAllCustomerOrderResult GetAllCustomerOrder(GetAllCustomerOrderParameter parameter);
        CreateCustomerOrderResult CreateCustomerOrder(CreateCustomerOrderParameter parameter);
        UpdateCustomerOrderResult UpdateCustomerOrder(UpdateCustomerOrderParameter parameter);
        GetCustomerOrderByIDResult GetCustomerOrderByID(GetCustomerOrderByIDParameter parameter);
        ExportCustomerOrderPDFResult ExportPdfCustomerOrder(ExportCustomerOrderPDFParameter parameter);
        GetCustomerOrderBySellerResult GetCustomerOrderBySeller(GetCustomerOrderBySellerParameter parameter);
        GetEmployeeListByOrganizationIdResult GetEmployeeListByOrganizationId(GetEmployeeListByOrganizationIdParameter parameter);
        GetMonthsListResult GetMonthsList(GetMonthsListParameter parameter);
        GetProductCategoryGroupByLevelResult GetProductCategoryGroupByLevel(GetProductCategoryGroupByLevelParameter parameter);
        GetProductCategoryGroupByManagerResult GetProductCategoryGroupByManager(GetProductCategoryGroupByManagerParameter parameter);
        GetMasterDataOrderSearchResult GetMasterDataOrderSearch(GetMasterDataOrderSearchParameter parameter);
        SearchOrderResult SearchOrder(SearchOrderParameter parameter);
        GetMasterDataOrderCreateResult GetMasterDataOrderCreate(GetMasterDataOrderCreateParameter parameter);

        GetMasterDataOrderDetailDialogResult GetMasterDataOrderDetailDialog(
            GetMasterDataOrderDetailDialogParameter parameter);

        GetVendorByProductIdResult GetVendorByProductId(GetVendorByProductIdParameter parameter);

        GetMasterDataOrderDetailResult GetMasterDataOrderDetail(GetMasterDataOrderDetailParameter parameter);
        DeleteOrderResult DeleteOrder(DeleteOrderParameter parameter);
        GetDataDashboardHomeResult GetDataDashboardHome(GetDataDashboardHomeParameter parameter);
        CheckReceiptOrderHistoryResult CheckReceiptOrderHistory(CheckReceiptOrderHistoryParameter parameter);
        CheckBeforCreateOrUpdateOrderResult CheckBeforCreateOrUpdateOrder(CheckBeforCreateOrUpdateOrderParameter parameter);
        UpdateCustomerOrderResult UpdateStatusOrder(UpdateStatusOrderParameter parameter);
        ProfitAccordingCustomersResult SearchProfitAccordingCustomers(ProfitAccordingCustomersParameter parameter);

        GetMasterDataOrderServiceCreateResult GetMasterDataOrderServiceCreate(
            GetMasterDataOrderServiceCreateParameter parameter);

        CreateOrderServiceResult CreateOrderService(CreateOrderServiceParameter parameter);

        GetMasterDataPayOrderServiceResult
            GetMasterDataPayOrderService(GetMasterDataPayOrderServiceParameter parameter);

        GetListOrderByLocalPointResult GetListOrderByLocalPoint(GetListOrderByLocalPointParameter parameter);
        PayOrderByLocalPointResult PayOrderByLocalPoint(PayOrderByLocalPointParameter parameter);
        CheckExistsCustomerByPhoneResult CheckExistsCustomerByPhone(CheckExistsCustomerByPhoneParameter parameter);
        RefreshLocalPointResult RefreshLocalPoint(RefreshLocalPointParameter parameter);
        GetLocalPointByLocalAddressResult GetLocalPointByLocalAddress(GetLocalPointByLocalAddressParameter parameter);
        GetDataSearchTopReVenueResult GetDataSearchTopReVenue(GetDataSearchTopReVenueParameter parameter);
        SearchTopReVenueResult SearchTopReVenue(SearchTopReVenueParameter parameter);
        GetDataSearchRevenueProductResult GetDataSearchRevenueProduct(GetDataSearchRevenueProductParameter parameter);
        SearchRevenueProductResult SearchRevenueProduct(SearchRevenueProductParameter parameter);
        GetListOrderDetailByOrderResult GetListOrderDetailByOrder(GetListOrderDetailByOrderParameter parameter);
        GetListProductWasOrderResult GetListProductWasOrder(GetListProductWasOrderParameter parameter);
        UpdateCustomerServiceResult UpdateCustomerService(UpdateCustomerServiceParameter parameter);
        GetDataProfitByCustomerResult GetDataProfitByCustomer(GetDataProfitByCustomerParameter parameter);
        SearchProfitCustomerResult SearchProfitCustomer(SearchProfitCustomerParameter parameter);
        GetInventoryNumberResult GetInventoryNumber(GetInventoryNumberParameter parameter);
        CheckTonKhoSanPhamResult CheckTonKhoSanPham(CheckTonKhoSanPhamParameter parameter);
        UpdateCustomerOrderTonKhoResult UpdateCustomerOrderTonKho(UpdateCustomerOrderTonKhoParameter parameter);
        Task<SearchOptionOfPacketServiceResult> SearchOptionOfPacketService(SearchOptionOfPacketServiceParameter parameter);
        Task<ChangeStatusCustomerOrderResult> ChangeStatusCustomerOrder(ChangeStatusCustomerOrderParameter parameter);
        GetMasterDataCreateOrderActionResult GetMasterDataCreateOrderAction(GetMasterDataCreateOrderActionParameter parameter);
        ChangeCustomerOrderResult ChangeCustomerOrder(ChangeCustomerOrderParameter parameter);
        GetVendorAndEmployeeOfPacketResult GetVendorAndEmployeeOfPacket(GetVendorAndEmployeeOfPacketParameter parameter);
        CreateOrUpdateCustomerOrderActionResult CreateOrUpdateCustomerOrderAction(CreateOrUpdateCustomerOrderActionParameter parameter);
        GetMasterDataOrderActionDetailResult GetMasterDataOrderActionDetail(GetMasterDataOrderActionDetailParameter parameter);
        CreateOrUpdateCustomerReportPointResult CreateOrUpdateCustomerReportPoint(CreateOrUpdateCustomerReportPointParameter parameter);
        GetOptionAndEmpOfOrderActionResult GetOptionAndEmpOfOrderAction(GetOptionAndEmpOfOrderActionParameter parameter);
        ChangeStatusReportPointResult ChangeStatusReportPoint(ChangeStatusReportPointParameter parameter);
        GetMasterDataCreateOrderProcessResult GetMasterDataCreateOrderProcess(GetMasterDataCreateOrderProcessParameter parameter);
        CreateOrUpdateCustomerOrderProcessResult CreateOrUpdateCustomerOrderProcess(CreateOrUpdateCustomerOrderProcessParameter parameter);
        GetDetailOrderProcessResult GetDetailOrderProcess(GetDetailOrderProcessParameter parameter);
        UpdateOrderProcessResult UpdateOrderProcess(UpdateOrderProcessParameter parameter);
        GetListOrderOfCusResult GetListOrderOfCus(GetListOrderOfCusParameter parameter);
        RatingOrderResult RatingOrder(RatingOrderParameter parameter);
        DeleteOrderOptionByCusResult DeleteOrderOptionByCus(DeleteOrderOptionByCusParameter parameter);
        UpdateCustomerOrderDetailExtendResult UpdateCustomerOrderDetailExtend(UpdateCustomerOrderDetailExtendParameter parameter);
        CheckTaskWithReportPointExtendResult CheckTaskWithReportPointExtend(CheckTaskWithReportPointExtendParameter parameter);
        DeleteOrderResult DeleteCustomerOrder(DeleteOrderParameter parameter);
        Task<Messages.Results.Employee.TakeListEvaluateResult> TakeListEvaluateForObjectId(Messages.Parameters.Employee.TakeListEvaluateParameter parameter);
    }
}
