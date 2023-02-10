using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Asset;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.Employee;
namespace TN.TNM.DataAccess.Messages.Results.Asset
{
    public class BaoCaoKhauHaoResult : BaseResult
    {
        public List<BaoCaoKhauHaoEntityModel> ListAsset { get; set; }
        public CompanyConfigEntityModel CompanyConfig { get; set; }
        public List<CategoryEntityModel> ListPhanLoaiTaiSan { get; set; }     
        //public List<> ListHienTrangTaiSan { get; set; }
   
    }
}
