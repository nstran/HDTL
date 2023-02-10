using System.Collections.Generic;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Asset;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.DataAccess.Messages.Results.Asset
{
    public class GetMasterDataPhanBoTSFormResult : BaseResult
    {
        public List<CategoryEntityModel> ListLoaiTSPB { get; set; }
        public List<CategoryEntityModel> ListMucDichSuDung { get; set; }
        public List<EmployeeEntityModel> ListEmployee { get; set; }        
        public List<AssetEntityModel> ListTaiSan { get; set; }
        public List<CategoryEntityModel> ListDonVi { get; set; }
        public List<AssetEntityModel> ListTaiSanChuaPhanBo { get; set; }
    }
}
