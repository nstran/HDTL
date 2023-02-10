using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Models.Salary
{
    public class TroCapModel : BaseModel<TroCap>
    {
        public int? TroCapId { get; set; }
        public int TypeId { get; set; }
        public Guid LoaiTroCapId { get; set; }
        public decimal MucTroCap { get; set; }

        /* Virtual Field */
        public string LoaiTroCap { get; set; }
        public string ChucVuText { get; set; }
        public string LoaiHopDongText { get; set; }
        public string DieuKienHuongText { get; set; }

        public List<TroCapChucVuMappingModel> ListChucVu { get; set; }
        public List<TroCapLoaiHopDongMappingModel> ListLoaiHopDong { get; set; }
        public List<TroCapDieuKienHuongMappingModel> ListDieuKienHuong { get; set; }
        public List<MucHuongTheoNgayNghiModel> ListMucHuongTheoNgayNghi { get; set; }
        public List<MucHuongDmvsModel> ListMucHuongDmvs { get; set; }

        public TroCapModel()
        {
            ListChucVu = new List<TroCapChucVuMappingModel>();
            ListLoaiHopDong = new List<TroCapLoaiHopDongMappingModel>();
            ListDieuKienHuong = new List<TroCapDieuKienHuongMappingModel>();
            ListMucHuongTheoNgayNghi = new List<MucHuongTheoNgayNghiModel>();
            ListMucHuongDmvs = new List<MucHuongDmvsModel>();
        }

        //Map từ Entity => Model
        public TroCapModel(TroCap entity)
        {
            Mapper(entity, this);
        }

        //Map từ Model => Entity
        public override TroCap ToEntity()
        {
            var entity = new TroCap();
            Mapper(this, entity);
            return entity;
        }
    }
}
