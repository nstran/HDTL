using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Models.Salary
{
    public class LuongCtDieuKienTroCapCoDinhModel : BaseModel<LuongCtDieuKienTroCapCoDinh>
    {
        public int LuongCtDieuKienTroCapCoDinhId { get; set; }
        public int LuongCtTroCapCoDinhId { get; set; }
        public int LuongCtLoaiTroCapCoDinhId { get; set; }
        public int KyLuongId { get; set; }
        public Guid DieuKienHuongId { get; set; }
        public bool DuDieuKien { get; set; }

        /* Virtual Field */
        public string DieuKienHuong { get; set; }

        public LuongCtDieuKienTroCapCoDinhModel()
        {

        }

        //Map từ Entity => Model
        public LuongCtDieuKienTroCapCoDinhModel(LuongCtDieuKienTroCapCoDinh entity)
        {
            Mapper(entity, this);
        }

        //Map từ Model => Entity
        public override LuongCtDieuKienTroCapCoDinh ToEntity()
        {
            var entity = new LuongCtDieuKienTroCapCoDinh();
            Mapper(this, entity);
            return entity;
        }
    }
}
