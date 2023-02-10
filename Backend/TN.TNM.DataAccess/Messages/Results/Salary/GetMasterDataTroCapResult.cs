using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Helper;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Salary;

namespace TN.TNM.DataAccess.Messages.Results.Salary
{
    public class GetMasterDataTroCapResult : BaseResult
    {
        public List<TroCapModel> ListTroCapCoDinh { get; set; }
        public TroCapModel TroCapTheoChuyenCanNgayCong { get; set; }
        public TroCapModel TroCapTheoChuyenCanDmvs { get; set; }
        public List<TroCapModel> ListTroCapKhac { get; set; }
        public List<CategoryEntityModel> ListLoaiTroCapCoDinh { get; set; }
        public List<CategoryEntityModel> ListLoaiTroCapChuyenCanNgayCong { get; set; }
        public List<CategoryEntityModel> ListLoaiTroCapChuyenCanDmvs { get; set; }
        public List<CategoryEntityModel> ListLoaiTroCapKhac { get; set; }
        public List<PositionModel> ListPosition { get; set; }
        public List<CategoryEntityModel> ListLoaiHopDong { get; set; }
        public List<CategoryEntityModel> ListDieuKienHuong { get; set; }
        public List<TrangThaiGeneral> ListLoaiNgayNghi { get; set; }
        public List<TrangThaiGeneral> ListHinhThucTru { get; set; }
    }
}
