using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Address;
using TN.TNM.DataAccess.Models.Asset;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.DataAccess.Messages.Results.Asset
{
    public class GetAllAssetListResult : BaseResult
    {
        public List<AssetEntityModel> ListAsset { get; set; }
        public CompanyConfigEntityModel CompanyConfig { get; set; }
        public List<EmployeeEntityModel> ListEmployee { get; set; }
        public List<CategoryEntityModel> ListLoaiTaiSan { get; set; }
        public List<ProvinceEntityModel> ListKhuVuc { get; set; }
    }
}
