using System;
using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Folder;
using TN.TNM.BusinessLogic.Models.Note;
using TN.TNM.BusinessLogic.Models.PurchaseOrderStatus;
using TN.TNM.DataAccess.Databases.DAO;
using TN.TNM.DataAccess.Models.Vendor;
using TN.TNM.DataAccess.Models.WareHouse;

namespace TN.TNM.BusinessLogic.Messages.Responses.Vendor
{
    public class GetDataEditVendorOrderResponse: BaseResponse
    {
        public DataAccess.Models.Vendor.VendorOrderEntityModel VendorOrderById { get; set; }
        public List<DataAccess.Models.Vendor.VendorOrderDetailEntityModel> ListVendorOrderDetailById { get; set; }
        public List<Models.Category.CategoryModel> ListPaymentMethod { get; set; }
        public List<PurchaseOrderStatusModel> ListOrderStatus { get; set; }
        public List<DataAccess.Models.Employee.EmployeeEntityModel> ListEmployeeModel { get; set; }
        public List<Models.BankAccount.BankAccountModel> ListBankAccount { get; set; }
        public List<DataAccess.Models.Vendor.VendorCreateOrderEntityModel> VendorCreateOrderModel { get; set; }
        public List<DataAccess.Models.ProcurementRequest.ProcurementRequestEntityModel> ListProcurementRequest { get; set; }
        //public List<ContractEntityModel> ListContract { get; set; }
        public List<WareHouseEntityModel> ListWareHouse { get; set; }
        public List<Guid?> ListProcurementRequestId { get; set; }
        public List<VendorOrderCostDetailEntityModel> ListVendorOrderCostDetail { get; set; }
        public List<NoteModel> ListNote { get; set; }

        public List<FileInFolderModel> ListFile { get; set; }

        public List<SoDuSanPhamTrongKho> ListSucChuaSanPhamTrongKho { get; set; }

        public List<InventoryReceivingVoucherEntityModel> ListPhieuNhapKho { get; set; }
    }
}
