using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TN.TNM.BusinessLogic.Interfaces.WareHouse;
using TN.TNM.BusinessLogic.Messages.Requests.WareHouse;
using TN.TNM.BusinessLogic.Messages.Responses.WareHouse;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.WareHouse;
using TN.TNM.DataAccess.Messages.Results.WareHouse;

namespace TN.TNM.Api.Controllers
{
    public class WareHouseController : Controller
    {
        private readonly IWareHouse iWareHouse;
        private readonly IWareHouseDataAccess iWareHouseDataAccess;
        public WareHouseController(IWareHouse _iWareHouse, IWareHouseDataAccess _iwareHouseDataAccess)
        {
            this.iWareHouse = _iWareHouse;
            this.iWareHouseDataAccess = _iwareHouseDataAccess;
        }

        /// <summary>
        /// Create/Update WareHouse
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/warehouse/createUpdateWareHouse")]
        [Authorize(Policy = "Member")]
        public CreateUpdateWareHouseResult CreateUpdateWareHouse([FromBody]CreateUpdateWareHouseParameter request)
        {
            return iWareHouseDataAccess.CreateUpdateWareHouse(request);
        }

        /// <summary>
        /// Search WareHouse
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/warehouse/searchWareHouse")]
        [Authorize(Policy = "Member")]
        public SearchWareHouseResult SearchWareHouse([FromBody]SearchWareHouseParameter request)
        {
            return iWareHouseDataAccess.SearchWareHouse(request);
        }


        /// <summary>
        /// GetWareHouseCha
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/warehouse/getWareHouseCha")]
        [Authorize(Policy = "Member")]
        public GetWareHouseChaResult GetWareHouseCha([FromBody]GetWareHouseChaParameter request)
        {
            return iWareHouseDataAccess.GetWareHouseCha(request);
        }

        /// <summary>
        /// GetVendorOrderByVendorId
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/warehouse/getVendorOrderByVendorId")]
        [Authorize(Policy = "Member")]
        public GetVendorOrderByVendorIdResult GetVendorOrderByVendorId([FromBody]GetVendorOrderByVendorIdParameter request)
        {
            return iWareHouseDataAccess.GetVendorOrderByVendorId(request);
        }

        /// <summary>
        /// GetVendorOrderDetailByVenderOrderId
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/warehouse/getVendorOrderDetailByVenderOrderId")]
        [Authorize(Policy = "Member")]
        public GetVendorOrderDetailByVenderOrderIdResult GetVendorOrderDetailByVenderOrderId([FromBody]GetVendorOrderDetailByVenderOrderIdParameter request)
        {
            return iWareHouseDataAccess.GetVendorOrderDetailByVenderOrderId(request);
        }

        /// <summary>
        /// DownloadTemplateSerial
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/warehouse/downloadTemplateSerial")]
        [Authorize(Policy = "Member")]
        public DownloadTemplateSerialResult DownloadTemplateSerial([FromBody]DownloadTemplateSerialParameter request)
        {
            return iWareHouseDataAccess.DownloadTemplateSerial(request);
        }

        /// <summary>
        /// CreateOrUpdateInventoryVoucher
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/warehouse/createOrUpdateInventoryVoucher")]
        [Authorize(Policy = "Member")]
        public CreateOrUpdateInventoryVoucherResult CreateOrUpdateInventoryVoucher(CreateOrUpdateInventoryVoucherParameter request)
        {
                return iWareHouseDataAccess.CreateOrUpdateInventoryVoucher(request);
        }

        /// <summary>
        /// Remove WareHouse
        /// </summary>
        /// <param name="request">Remove WareHouse</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/warehouse/removeWareHouse")]
        [Authorize(Policy = "Member")]
        public RemoveWareHouseResult RemoveWareHouse([FromBody]RemoveWareHouseParameter request)
        {
            return iWareHouseDataAccess.RemoveWareHouse(request);
        }
        /// <summary>
        /// GetInventoryReceivingVoucherByIdResponse
        /// </summary>
        /// <param name="request">Id</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/warehouse/getInventoryReceivingVoucherById")]
        [Authorize(Policy = "Member")]
        public GetInventoryReceivingVoucherByIdResult GetInventoryReceivingVoucherById([FromBody]GetInventoryReceivingVoucherByIdParameter request)
        {
            return iWareHouseDataAccess.GetInventoryReceivingVoucherById(request);
        }
        /// <summary>
        /// GetInventoryReceivingVoucherByIdResponse
        /// </summary>
        /// <param name="request">Id</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/warehouse/getListInventoryReceivingVoucher")]
        [Authorize(Policy = "Member")]
        public GetListInventoryReceivingVoucherResult GetListInventoryReceivingVoucher([FromBody]GetListInventoryReceivingVoucherParameter request)
        {
            return iWareHouseDataAccess.GetListInventoryReceivingVoucher(request);
        }
        /// <summary>
        /// GetListCustomerOrderByIdCustomerId
        /// </summary>
        /// <param name="request">Id</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/warehouse/getListCustomerOrderByIdCustomerId")]
        [Authorize(Policy = "Member")]
        public GetListCustomerOrderByIdCustomerIdResult GetListCustomerOrderByIdCustomerId([FromBody]GetListCustomerOrderByIdCustomerIdParameter request)
        {
            return iWareHouseDataAccess.GetListCustomerOrderByIdCustomerId(request);
        }

        /// <summary>
        /// GetCustomerOrderDetailByCustomerOrderId
        /// </summary>
        /// <param name="request">Id</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/warehouse/getCustomerOrderDetailByCustomerOrderId")]
        [Authorize(Policy = "Member")]
        public GetCustomerOrderDetailByCustomerOrderIdResult GetCustomerOrderDetailByCustomerOrderId([FromBody]GetCustomerOrderDetailByCustomerOrderIdParameter request)
        {
            return iWareHouseDataAccess.GetCustomerOrderDetailByCustomerOrderId(request);
        }
        /// <summary>
        /// CheckQuantityActualReceivingVoucher
        /// </summary>
        /// <param name="request">Id</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/warehouse/checkQuantityActualReceivingVoucher")]
        [Authorize(Policy = "Member")]
        public CheckQuantityActualReceivingVoucherResult CheckQuantityActualReceivingVoucher([FromBody]CheckQuantityActualReceivingVoucherParameter request)
        {
            return iWareHouseDataAccess.CheckQuantityActualReceivingVoucher(request);
        }
        /// <summary>
        /// FilterVendor
        /// </summary>
        /// <param name="request">Id</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/warehouse/filterVendor")]
        [Authorize(Policy = "Member")]
        public FilterVendorResult FilterVendor([FromBody]FilterVendorParameter request)
        {
            return iWareHouseDataAccess.FilterVendor(request);
        }
        /// <summary>
        /// FilterCustomer
        /// </summary>
        /// <param name="request">Id</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/warehouse/filterCustomer")]
        [Authorize(Policy = "Member")]
        public FilterCustomerResult FilterCustomer([FromBody]FilterCustomerParameter request)
        {
            return iWareHouseDataAccess.FilterCustomer(request);
        }

        /// <summary>
        /// ChangeStatusInventoryReceivingVoucher
        /// </summary>
        /// <param name="request">Id</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/warehouse/changeStatusInventoryReceivingVoucher")]
        [Authorize(Policy = "Member")]
        public ChangeStatusInventoryReceivingVoucherResult ChangeStatusInventoryReceivingVoucher([FromBody]ChangeStatusInventoryReceivingVoucherParameter request)
        {
            return iWareHouseDataAccess.ChangeStatusInventoryReceivingVoucher(request);
        }

        /// <summary>
        /// DeleteInventoryReceivingVoucher
        /// </summary>
        /// <param name="request">Id</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/warehouse/deleteInventoryReceivingVoucher")]
        [Authorize(Policy = "Member")]
        public DeleteInventoryReceivingVoucherResult DeleteInventoryReceivingVoucher([FromBody]DeleteInventoryReceivingVoucherParameter request)
        {
            return iWareHouseDataAccess.DeleteInventoryReceivingVoucher(request);
        }
        /// <summary>
        /// InventoryDeliveryVoucherFilterVendorOrder
        /// </summary>
        /// <param name="request">Id</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/warehouse/inventoryDeliveryVoucherFilterVendorOrder")]
        [Authorize(Policy = "Member")]
        public InventoryDeliveryVoucherFilterVendorOrderResult InventoryDeliveryVoucherFilterVendorOrder([FromBody]InventoryDeliveryVoucherFilterVendorOrderParameter request)
        {
            return iWareHouseDataAccess.InventoryDeliveryVoucherFilterVendorOrder(request);
        }
        /// <summary>
        /// InventoryDeliveryVoucherFilterCustomerOrder
        /// </summary>
        /// <param name="request">Id</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/warehouse/inventoryDeliveryVoucherFilterCustomerOrder")]
        [Authorize(Policy = "Member")]
        public InventoryDeliveryVoucherFilterCustomerOrderResult InventoryDeliveryVoucherFilterCustomerOrder([FromBody]InventoryDeliveryVoucherFilterCustomerOrderParameter request)
        {
            return iWareHouseDataAccess.InventoryDeliveryVoucherFilterCustomerOrder(request);
        }
        /// <summary>
        /// GetTop10WarehouseFromReceivingVoucher
        /// </summary>
        /// <param name="request">Id</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/warehouse/getTop10WarehouseFromReceivingVoucher")]
        [Authorize(Policy = "Member")]
        public GetTop10WarehouseFromReceivingVoucherResult GetTop10WarehouseFromReceivingVoucher([FromBody]GetTop10WarehouseFromReceivingVoucherParameter request)
        {
            return iWareHouseDataAccess.GetTop10WarehouseFromReceivingVoucher(request);
        }
        /// <summary>
        /// GetSerial
        /// </summary>
        /// <param name="request">Id</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/warehouse/getSerial")]
        [Authorize(Policy = "Member")]
        public GetSerialResult GetSerial([FromBody]GetSerialParameter request)
        {
            return iWareHouseDataAccess.GetSerial(request);
        }
        /// <summary>
        /// CreateUpdateInventoryDeliveryVoucher
        /// </summary>
        /// <param name="request">Id</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/warehouse/createUpdateInventoryDeliveryVoucher")]
        [Authorize(Policy = "Member")]
        public CreateUpdateInventoryDeliveryVoucherResult CreateUpdateInventoryDeliveryVoucher(CreateUpdateInventoryDeliveryVoucherParameter request)
        {
            return iWareHouseDataAccess.CreateUpdateInventoryDeliveryVoucher(request);
        }

        /// <summary>
        /// GetInventoryDeliveryVoucherById
        /// </summary>
        /// <param name="request">Id</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/warehouse/getInventoryDeliveryVoucherById")]
        [Authorize(Policy = "Member")]
        public GetInventoryDeliveryVoucherByIdResult GetInventoryDeliveryVoucherById([FromBody]GetInventoryDeliveryVoucherByIdParameter request)
        {
            return iWareHouseDataAccess.GetInventoryDeliveryVoucherById(request);
        }
        /// <summary>
        /// DeleteInventoryDeliveryVoucher
        /// </summary>
        /// <param name="request">Id</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/warehouse/deleteInventoryDeliveryVoucher")]
        [Authorize(Policy = "Member")]
        public DeleteInventoryDeliveryVoucherResult DeleteInventoryDeliveryVoucher([FromBody]DeleteInventoryDeliveryVoucherParameter request)
        {
            return iWareHouseDataAccess.DeleteInventoryDeliveryVoucher(request);
        }

        /// <summary>
        /// ChangeStatusInventoryDeliveryVoucher
        /// </summary>
        /// <param name="request">Id</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/warehouse/changeStatusInventoryDeliveryVoucher")]
        [Authorize(Policy = "Member")]
        public ChangeStatusInventoryDeliveryVoucherResult ChangeStatusInventoryDeliveryVoucherRequest([FromBody]ChangeStatusInventoryDeliveryVoucherParameter request)
        {
            return iWareHouseDataAccess.ChangeStatusInventoryDeliveryVoucher(request);
        }
        /// <summary>
        /// FilterCustomerInInventoryDeliveryVoucher
        /// </summary>
        /// <param name="request">Id</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/warehouse/filterCustomerInInventoryDeliveryVoucher")]
        [Authorize(Policy = "Member")]
        public FilterCustomerInInventoryDeliveryVoucherResult FilterCustomerInInventoryDeliveryVoucher([FromBody]FilterCustomerInInventoryDeliveryVoucherParameter request)
        {
            return iWareHouseDataAccess.FilterCustomerInInventoryDeliveryVoucher(request);
        }
        /// <summary>
        /// searchInventoryDeliveryVoucher
        /// </summary>
        /// <param name="request">Id</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/warehouse/searchInventoryDeliveryVoucher")]
        [Authorize(Policy = "Member")]
        public SearchInventoryDeliveryVoucherResult SearchInventoryDeliveryVoucher([FromBody]SearchInventoryDeliveryVoucherParameter request)
        {
            return iWareHouseDataAccess.SearchInventoryDeliveryVoucher(request);
        }
        /// <summary>
        /// FilterProduct
        /// </summary>
        /// <param name="request">Id</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/warehouse/filterProduct")]
        [Authorize(Policy = "Member")]
        public FilterProductResult FilterProduct([FromBody]FilterProductParameter request)
        {
            return iWareHouseDataAccess.FilterProduct(request);
        }
        /// <summary>
        /// GetProductNameAndProductCode
        /// </summary>
        /// <param name="request">Id</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/warehouse/getProductNameAndProductCode")]
        [Authorize(Policy = "Member")]
        public GetProductNameAndProductCodeResult GetProductNameAndProductCode([FromBody]GetProductNameAndProductCodeParameter request)
        {
            return iWareHouseDataAccess.GetProductNameAndProductCode(request);
        }
        /// <summary>
        /// GetVendorInvenoryReceiving
        /// </summary>
        /// <param name="request">Id</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/warehouse/getVendorInvenoryReceiving")]
        [Authorize(Policy = "Member")]
        public GetVendorInvenoryReceivingResult GetVendorInvenoryReceiving([FromBody]GetVendorInvenoryReceivingParameter request)
        {
            return iWareHouseDataAccess.GetVendorInvenoryReceiving(request);
        }

        /// <summary>
        /// GetCustomerDelivery
        /// </summary>
        /// <param name="request">Id</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/warehouse/getCustomerDelivery")]
        [Authorize(Policy = "Member")]
        public GetCustomerDeliveryResult GetCustomerDelivery([FromBody]GetCustomerDeliveryParameter request)
        {
            return iWareHouseDataAccess.GetCustomerDelivery(request);
        }
        /// <summary>
        /// InStockReport
        /// </summary>
        /// <param name="request">Id</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/warehouse/inStockReport")]
        [Authorize(Policy = "Member")]
        public InStockReportResult InStockReport([FromBody]InStockReportParameter request)
        {
            return iWareHouseDataAccess.InStockReport(request);
        }

        /// <summary>
        /// Create Update Warehouse Masterdata
        /// </summary>
        /// <param name="request">Id</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/warehouse/createUpdateWarehouseMasterdata")]
        [Authorize(Policy = "Member")]
        public CreateUpdateWarehouseMasterdataResult CreateUpdateWarehouseMasterdata([FromBody]CreateUpdateWarehouseMasterdataParameter request)
        {
            return iWareHouseDataAccess.CreateUpdateWarehouseMasterdata(request);
        }

        //
        [HttpPost]
        [Route("api/warehouse/getMasterDataSearchInStockReport")]
        [Authorize(Policy = "Member")]
        public GetMasterDataSearchInStockReportResult GetMasterDataSearchInStockReport(
            [FromBody]GetMasterDataSearchInStockReportParameter request)
        {
            return iWareHouseDataAccess.GetMasterDataSearchInStockReport(request);
        }

        //
        [HttpPost]
        [Route("api/warehouse/searchInStockReport")]
        [Authorize(Policy = "Member")]
        public SearchInStockReportResult SearchInStockReport([FromBody] SearchInStockReportParameter request)
        {
            return iWareHouseDataAccess.SearchInStockReport(request);
        }

        //
        [HttpPost]
        [Route("api/warehouse/getMasterDataPhieuNhapKho")]
        [Authorize(Policy = "Member")]
        public GetMasterDataPhieuNhapKhoResult GetMasterDataPhieuNhapKho([FromBody]GetMasterDataPhieuNhapKhoParameter request)
        {
            return iWareHouseDataAccess.GetMasterDataPhieuNhapKho(request);
        }

        //
        [HttpPost]
        [Route("api/warehouse/getDanhSachSanPhamCuaPhieu")]
        [Authorize(Policy = "Member")]
        public GetDanhSachSanPhamCuaPhieuResult GetDanhSachSanPhamCuaPhieu([FromBody] GetDanhSachSanPhamCuaPhieuParameter request)
        {
            return iWareHouseDataAccess.GetDanhSachSanPhamCuaPhieu(request);
        }

        //
        [HttpPost]
        [Route("api/warehouse/getDanhSachKhoCon")]
        [Authorize(Policy = "Member")]
        public GetDanhSachKhoConResult GetDanhSachKhoCon([FromBody]GetDanhSachKhoConParameter request)
        {
            return iWareHouseDataAccess.GetDanhSachKhoCon(request);
        }

        //
        [HttpPost]
        [Route("api/warehouse/createItemInventoryReport")]
        [Authorize(Policy = "Member")]
        public CreateItemInventoryReportResult CreateItemInventoryReport([FromBody] CreateItemInventoryReportParameter request)
        {
            return iWareHouseDataAccess.CreateItemInventoryReport(request);
        }

        //
        [HttpPost]
        [Route("api/warehouse/updateItemInventoryReport")]
        [Authorize(Policy = "Member")]
        public UpdateItemInventoryReportResult UpdateItemInventoryReport([FromBody]UpdateItemInventoryReportParameter request)
        {
            return iWareHouseDataAccess.UpdateItemInventoryReport(request);
        }

        //
        [HttpPost]
        [Route("api/warehouse/createUpdateSerial")]
        [Authorize(Policy = "Member")]
        public CreateUpdateSerialResult CreateUpdateSerial([FromBody]CreateUpdateSerialParameter request)
        {
            return iWareHouseDataAccess.CreateUpdateSerial(request);
        }

        //
        [HttpPost]
        [Route("api/warehouse/deleteItemInventoryReport")]
        [Authorize(Policy = "Member")]
        public DeleteItemInventoryReportResult DeleteItemInventoryReport([FromBody]DeleteItemInventoryReportParameter request)
        {
            return iWareHouseDataAccess.DeleteItemInventoryReport(request);
        }

        //
        [HttpPost]
        [Route("api/warehouse/getSoGTCuaSanPhamTheoKho")]
        [Authorize(Policy = "Member")]
        public GetSoGTCuaSanPhamTheoKhoResult GetSoGTCuaSanPhamTheoKho([FromBody]GetSoGTCuaSanPhamTheoKhoParameter request)
        {
            return iWareHouseDataAccess.GetSoGTCuaSanPhamTheoKho(request);
        }

        //
        [HttpPost]
        [Route("api/warehouse/createPhieuNhapKho")]
        [Authorize(Policy = "Member")]
        public CreatePhieuNhapKhoResult CreatePhieuNhapKho(CreatePhieuNhapKhoParameter request)
        {
            return iWareHouseDataAccess.CreatePhieuNhapKho(request);
        }

        //
        [HttpPost]
        [Route("api/warehouse/getDetailPhieuNhapKho")]
        [Authorize(Policy = "Member")]
        public GetDetailPhieuNhapKhoResult GetDetailPhieuNhapKho([FromBody]GetDetailPhieuNhapKhoParameter request)
        {
            return iWareHouseDataAccess.GetDetailPhieuNhapKho(request);
        }

        //
        [HttpPost]
        [Route("api/warehouse/suaPhieuNhapKho")]
        [Authorize(Policy = "Member")]
        public SuaPhieuNhapKhoResult SuaPhieuNhapKho([FromBody]SuaPhieuNhapKhoParameter request)
        {
            return iWareHouseDataAccess.SuaPhieuNhapKho(request);
        }

        //
        [HttpPost]
        [Route("api/warehouse/kiemTraKhaDungPhieuNhapKho")]
        [Authorize(Policy = "Member")]
        public KiemTraKhaDungPhieuNhapKhoResult KiemTraKhaDungPhieuNhapKho([FromBody] KiemTraKhaDungPhieuNhapKhoParameter request)
        {
            return iWareHouseDataAccess.KiemTraKhaDungPhieuNhapKho(request);
        }

        //
        [HttpPost]
        [Route("api/warehouse/danhDauCanLamPhieuNhapKho")]
        [Authorize(Policy = "Member")]
        public DanhDauCanLamPhieuNhapKhoResult DanhDauCanLamPhieuNhapKho([FromBody]DanhDauCanLamPhieuNhapKhoParameter request)
        {
            return iWareHouseDataAccess.DanhDauCanLamPhieuNhapKho(request);
        }

        //
        [HttpPost]
        [Route("api/warehouse/nhanBanPhieuNhapKho")]
        [Authorize(Policy = "Member")]
        public NhanBanPhieuNhapKhoResult NhanBanPhieuNhapKho([FromBody] NhanBanPhieuNhapKhoParameter request)
        {
            return iWareHouseDataAccess.NhanBanPhieuNhapKho(request);
        }

        //
        [HttpPost]
        [Route("api/warehouse/xoaPhieuNhapKho")]
        [Authorize(Policy = "Member")]
        public XoaPhieuNhapKhoResult XoaPhieuNhapKho([FromBody]XoaPhieuNhapKhoParameter request)
        {
            return iWareHouseDataAccess.XoaPhieuNhapKho(request);
        }

        //
        [HttpPost]
        [Route("api/warehouse/huyPhieuNhapKho")]
        [Authorize(Policy = "Member")]
        public HuyPhieuNhapKhoResult HuyPhieuNhapKho([FromBody] HuyPhieuNhapKhoParameter request)
        {
            return iWareHouseDataAccess.HuyPhieuNhapKho(request);
        }

        //
        [HttpPost]
        [Route("api/warehouse/khongGiuPhanPhieuNhapKho")]
        [Authorize(Policy = "Member")]
        public KhongGiuPhanPhieuNhapKhoResult KhongGiuPhanPhieuNhapKho([FromBody] KhongGiuPhanPhieuNhapKhoParameter request)
        {
            return iWareHouseDataAccess.KhongGiuPhanPhieuNhapKho(request);
        }

        //
        [HttpPost]
        [Route("api/warehouse/kiemTraNhapKho")]
        [Authorize(Policy = "Member")]
        public KiemTraNhapKhoResult KiemTraNhapKho([FromBody] KiemTraNhapKhoParameter request)
        {
            return iWareHouseDataAccess.KiemTraNhapKho(request);
        }

        //
        [HttpPost]
        [Route("api/warehouse/datVeNhapPhieuNhapKho")]
        [Authorize(Policy = "Member")]
        public DatVeNhapPhieuNhapKhoResult DatVeNhapPhieuNhapKho([FromBody] DatVeNhapPhieuNhapKhoParameter request)
        {
            return iWareHouseDataAccess.DatVeNhapPhieuNhapKho(request);
        }

        //
        [HttpPost]
        [Route("api/warehouse/getListProductPhieuNhapKho")]
        [Authorize(Policy = "Member")]
        public GetListProductPhieuNhapKhoResult GetListProductPhieuNhapKho([FromBody] GetListProductPhieuNhapKhoParameter request)
        {
            return iWareHouseDataAccess.GetListProductPhieuNhapKho(request);
        }

        //
        [HttpPost]
        [Route("api/warehouse/getMasterDataListPhieuNhapKho")]
        [Authorize(Policy = "Member")]
        public GetMasterDataListPhieuNhapKhoResult GetMasterDataListPhieuNhapKho([FromBody] GetMasterDataListPhieuNhapKhoParameter request)
        {
            return iWareHouseDataAccess.GetMasterDataListPhieuNhapKho(request);
        }

        //
        [HttpPost]
        [Route("api/warehouse/searchListPhieuNhapKho")]
        [Authorize(Policy = "Member")]
        public SearchListPhieuNhapKhoResult SearchListPhieuNhapKho([FromBody] SearchListPhieuNhapKhoParameter request)
        {
            return iWareHouseDataAccess.SearchListPhieuNhapKho(request);
        }
    }
}
