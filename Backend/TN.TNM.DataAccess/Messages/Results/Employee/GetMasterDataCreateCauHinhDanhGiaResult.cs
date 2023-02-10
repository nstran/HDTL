using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.DataAccess.Messages.Results.Employee
{
    public class GetMasterDataCreateCauHinhDanhGiaResult: BaseResult
    {
        public List<CategoryEntityModel> ListMucDanhGia { get; set; }
        public List<MucDanhGiaDanhGiaMappingEntityModel> ListThangDiemDanhGia { get; set; }
        public List<QuyLuongEntityModel> ListQuyLuong { get; set; }
    }
}
