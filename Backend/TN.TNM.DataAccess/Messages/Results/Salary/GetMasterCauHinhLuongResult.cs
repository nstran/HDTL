using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Helper;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.CauHinhNghiLe;
using TN.TNM.DataAccess.Models.CauHinhOtMođel;
using TN.TNM.DataAccess.Models.CauHinhThue;
using TN.TNM.DataAccess.Models.ChamCong;
using TN.TNM.DataAccess.Models.GiamTru;
using TN.TNM.DataAccess.Models.Salary;

namespace TN.TNM.DataAccess.Messages.Results.Salary
{
    public class GetMasterCauHinhLuongResult : BaseResult
    {
        public List<TrangThaiGeneral> ListLoaiCaLamViec { get; set; }
        public List<TrangThaiGeneral> ListNgayLamViecTrongTuan { get; set; }
        public List<CaLamViecModel> ListCaLamViec { get; set; }
        public List<TrangThaiGeneral> ListLoaiNghiLe { get; set; }
        public List<CauHinhNghiLeModel> ListCauHinhNghiLe { get; set; }
        public List<CategoryEntityModel> ListLoaiOt { get; set; }
        public List<CauHinhOtModel> ListCauHinhOt { get; set; }
        public List<TrangThaiGeneral> ListLoaiGiamTru { get; set; }
        public List<CauHinhGiamTruModel> ListCauHinhGiamTru { get; set; }
        public KinhPhiCongDoanModel KinhPhiCongDoan { get; set; }
        public List<TrangThaiGeneral> ListTokenTinhLuong { get; set; }
        public CongThucTinhLuongModel CongThucTinhLuong { get; set; }
        public CauHinhChamCongOtModel CauHinhChamCongOt { get; set; }
        public CauHinhOtCaNgayModel CauHinhOtCaNgay { get; set; }
        public List<CauHinhThueTncnModel> ListCauHinhThueTncn { get; set; }
    }
}
