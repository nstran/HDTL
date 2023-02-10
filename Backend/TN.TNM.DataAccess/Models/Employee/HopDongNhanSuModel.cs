using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Models.Employee
{
    public class HopDongNhanSuModel : BaseModel<HopDongNhanSu>
    {
        public int? HopDongNhanSuId { get; set; }
        public Guid LoaiHopDongId { get; set; }
        public string SoHopDong { get; set; }
        public string SoPhuLuc { get; set; }
        public DateTime NgayKyHopDong { get; set; }
        public DateTime NgayBatDauLamViec { get; set; }
        public DateTime? NgayKetThucHopDong { get; set; }
        public Guid PositionId { get; set; }
        public decimal MucLuong { get; set; }
        public Guid EmployeeId { get; set; }
        public string EmployeeCode { get; set; }

        /* Các trường bổ sung thêm thông tin */
        public string LoaiHopDong { get; set; }
        public string PositionName { get; set; }
        public bool? IsPhatSinhKyLuong { get; set; }

        public HopDongNhanSuModel() { }

        //Map từ Entity => Model
        public HopDongNhanSuModel(HopDongNhanSu entity)
        {
            Mapper(entity, this);
        }

        //Map từ Model => Entity
        public override HopDongNhanSu ToEntity()
        {
            var entity = new HopDongNhanSu();
            Mapper(this, entity);
            return entity;
        }
    }
}
