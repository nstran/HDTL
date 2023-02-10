using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Models.Salary
{
    public class LuongCtDieuKienTroCapKhacModel : BaseModel<LuongCtDieuKienTroCapKhac>
    {
        public int LuongCtDieuKienTroCapKhacId { get; set; }
        public int LuongCtTroCapKhacId { get; set; }
        public int LuongCtLoaiTroCapKhacId { get; set; }
        public int KyLuongId { get; set; }
        public Guid DieuKienHuongId { get; set; }

        /* Virtual Field */
        public bool DuDieuKien { get; set; }
        public string DieuKienHuong { get; set; }

        public LuongCtDieuKienTroCapKhacModel()
        {

        }

        //Map từ Entity => Model
        public LuongCtDieuKienTroCapKhacModel(LuongCtDieuKienTroCapKhac entity)
        {
            Mapper(entity, this);
        }

        //Map từ Model => Entity
        public override LuongCtDieuKienTroCapKhac ToEntity()
        {
            var entity = new LuongCtDieuKienTroCapKhac();
            Mapper(this, entity);
            return entity;
        }
    }
}
