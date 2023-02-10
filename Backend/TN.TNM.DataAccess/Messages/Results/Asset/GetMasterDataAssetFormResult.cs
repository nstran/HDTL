using System.Collections.Generic;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Address;
using TN.TNM.DataAccess.Models.BankAccount;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.DataAccess.Messages.Results.Asset
{
    public class GetMasterDataAssetFormResult : BaseResult
    {
        public List<CategoryEntityModel> ListPhanLoaiTS { get; set; }
        public List<CategoryEntityModel> ListDonVi { get; set; }
        public List<CategoryEntityModel> ListHienTrangTS { get; set; }
        public List<CategoryEntityModel> ListNuocSX { get; set; }
        public List<CategoryEntityModel> ListHangSX { get; set; }
        public List<EmployeeEntityModel> ListEmployee { get; set; }
        public List<CategoryEntityModel> ListMucDichSuDung { get; set; }
        public List<ProvinceEntityModel> ListProvinceModel { get; set; }
        public List<string> ListMaTS { get; set; }
    }
}
