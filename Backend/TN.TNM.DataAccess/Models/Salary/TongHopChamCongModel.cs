using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Models.Salary
{
    public class TongHopChamCongModel : BaseModel<TongHopChamCong>
    {
        public int TongHopChamCongId { get; set; }
        public int KyLuongId { get; set; }
        public Guid EmployeeId { get; set; }
        public Guid OrganizationId { get; set; }
        public Guid PositionId { get; set; }
        public decimal NgayLamViecThucTe { get; set; }
        public decimal CongTac { get; set; }
        public decimal DaoTaoHoiThao { get; set; }
        public decimal NghiPhep { get; set; }
        public decimal NghiLe { get; set; }
        public decimal NghiCheDo { get; set; }
        public decimal NghiHuongLuongKhac { get; set; }
        public decimal NghiBu { get; set; }
        public decimal NghiHuongBhxh { get; set; }
        public decimal NghiKhongPhep { get; set; }
        public decimal NghiKhongLuong { get; set; }
        public decimal TongNgayDmvs { get; set; }
        public decimal SoLanTruChuyenCan { get; set; }

        /* Virtual Field */
        public string EmployeeCode { get; set; }
        public string OrganizationCode { get; set; }
        public string SubCode1 { get; set; }
        public string EmployeeName { get; set; }
        public decimal TongSoNgayKhongTinhLuong { get; set; }
        public decimal TongSoNgayTinhLuong { get; set; }
        public decimal TongNgayNghiTinhTroCapChuyenCan { get; set; }

        public TongHopChamCongModel()
        {

        }

        //Map từ Entity => Model
        public TongHopChamCongModel(TongHopChamCong entity)
        {
            TongSoNgayKhongTinhLuong = entity.NghiKhongLuong + entity.NghiKhongPhep + entity.NghiHuongBhxh +
                                       entity.TongNgayDmvs;

            TongSoNgayTinhLuong = entity.NgayLamViecThucTe + entity.CongTac + entity.DaoTaoHoiThao + entity.NghiPhep +
                                  entity.NghiLe + entity.NghiBu + entity.NghiCheDo +
                                  entity.NghiHuongLuongKhac - entity.TongNgayDmvs;

            TongNgayNghiTinhTroCapChuyenCan = entity.NghiPhep + entity.NghiKhongLuong + entity.NghiKhongPhep;

            Mapper(entity, this);
        }

        //Map từ Model => Entity
        public override TongHopChamCong ToEntity()
        {
            var entity = new TongHopChamCong();
            Mapper(this, entity);
            return entity;
        }
    }
}
