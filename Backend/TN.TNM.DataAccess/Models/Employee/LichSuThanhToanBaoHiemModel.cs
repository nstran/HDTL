using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Models.Employee
{
    public class LichSuThanhToanBaoHiemModel : BaseModel<LichSuThanhToanBaoHiem>
    {
        public int? LichSuThanhToanBaoHiemId { get; set; }
        public Guid EmployeeId { get; set; }
        public DateTime? NgayThanhToan { get; set; }
        public int? LoaiBaoHiem { get; set; }
        public string LoaiBaoHiemName { get; set; }
        public decimal? SoTienThanhToan { get; set; }
        public string GhiChu { get; set; }

        public LichSuThanhToanBaoHiemModel() { }

        //Map từ Entity => Model
        public LichSuThanhToanBaoHiemModel(LichSuThanhToanBaoHiem entity)
        {
            Mapper(entity, this);
        }

        //Map từ Model => Entity
        public override LichSuThanhToanBaoHiem ToEntity()
        {
            var entity = new LichSuThanhToanBaoHiem();
            Mapper(this, entity);
            return entity;
        }
    }
}
