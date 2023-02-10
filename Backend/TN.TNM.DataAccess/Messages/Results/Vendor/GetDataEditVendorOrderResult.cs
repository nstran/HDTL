using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.DAO;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.BankAccount;
using TN.TNM.DataAccess.Models.Folder;
using TN.TNM.DataAccess.Models.Note;
using TN.TNM.DataAccess.Models.ProcurementRequest;
using TN.TNM.DataAccess.Models.PurchaseOrderStatus;
using TN.TNM.DataAccess.Models.Vendor;
using TN.TNM.DataAccess.Models.WareHouse;

namespace TN.TNM.DataAccess.Messages.Results.Vendor
{
    public class GetDataEditVendorOrderResult: BaseResult
    {
        public Models.Vendor.VendorOrderEntityModel VendorOrderById { get; set; }
        public List<Models.Vendor.VendorOrderDetailEntityModel> ListVendorOrderDetailById { get; set; }
        public List<CategoryEntityModel> ListPaymentMethod { get; set; }
        public List<PurchaseOrderStatusEntityModel> ListOrderStatus { get; set; }
        public List<BankAccountEntityModel> ListBankAccount { get; set; }
        public List<Models.Employee.EmployeeEntityModel> ListEmployeeModel { get; set; }
        public List<Models.Vendor.VendorCreateOrderEntityModel> VendorCreateOrderModel { get; set; }
        public List<ProcurementRequestEntityModel> ListProcurementRequest { get; set; }
        //public List<ContractEntityModel> ListContract { get; set; }
        public List<WareHouseEntityModel> ListWareHouse { get; set; }
        public List<Guid?> ListProcurementRequestId { get; set; }
        public List<VendorOrderCostDetailEntityModel> ListVendorOrderCostDetail { get; set; }
        public List<NoteEntityModel> ListNote { get; set; }

        public List<FileInFolderEntityModel> ListFile { get; set; }

        public List<SoDuSanPhamTrongKho> ListSucChuaSanPhamTrongKho { get; set; }

        public List<InventoryReceivingVoucherEntityModel> ListPhieuNhapKho { get; set; }
    }
}
