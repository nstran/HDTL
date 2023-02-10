using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.DataAccess.Messages.Results.Employee
{
    public class GetMasterDataTaoKyDanhGiaResult: BaseResult
    {
        public List<PositionModel> ListChucVu { get; set; }
        public List<OrganizationEntityModel> ListPhongBan { get; set; }
        public List<PhieuDanhGia> ListPhieuDanhGia { get; set; }
        public Decimal? SoTienQuyLuongConLai { get; set; }
        
    }
}
