using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.Employee;
using TN.TNM.DataAccess.Models.ProcurementRequest;
using TN.TNM.DataAccess.Models.Product;
using TN.TNM.DataAccess.Models.PurchaseOrderStatus;
using TN.TNM.DataAccess.Models.Vendor;

namespace TN.TNM.DataAccess.Messages.Results.Vendor
{
    public class GetMasterDataSearchVendorOrderResult : BaseResult
    {
        public List<VendorEntityModel> ListVendor { get; set; }
        public List<PurchaseOrderStatusEntityModel> ListOrderStatus { get; set; }
        public List<EmployeeEntityModel> ListEmployee { get; set; }
        public List<ProcurementRequestEntityModel> ListProcurementRequest { get; set; }
        public List<ProductEntityModel> ListProduct { get; set; }
        public DataAccess.Models.CompanyConfigEntityModel CompanyConfig { get; set; }
    }
}
