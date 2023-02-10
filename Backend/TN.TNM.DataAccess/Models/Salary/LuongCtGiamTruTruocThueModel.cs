using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Models.Salary
{
    public class LuongCtGiamTruTruocThueModel : BaseModel<LuongCtGiamTruTruocThue>
    {
        public int LuongCtGiamTruTruocThueId { get; set; }
        public int KyLuongId { get; set; }
        public Guid EmployeeId { get; set; }
        public Guid OrganizationId { get; set; }
        public Guid PositionId { get; set; }
        public decimal GiamTruCaNhan { get; set; }
        public decimal GiamTruNguoiPhuThuoc { get; set; }
        public int SoNguoiPhuThuoc { get; set; }
        public decimal TienDongBaoHiem { get; set; }
        public decimal GiamTruKhac { get; set; }

        /* Virtual Field */
        public string EmployeeCode { get; set; }
        public string OrganizationCode { get; set; }
        public string SubCode1 { get; set; }
        public string EmployeeName { get; set; }
        public string PositionName { get; set; }
        public decimal Sum { get; set; }

        public LuongCtGiamTruTruocThueModel()
        {

        }

        //Map từ Entity => Model
        public LuongCtGiamTruTruocThueModel(LuongCtGiamTruTruocThue entity)
        {
            Sum = entity.GiamTruCaNhan + entity.GiamTruNguoiPhuThuoc + entity.TienDongBaoHiem + entity.GiamTruKhac;

            Mapper(entity, this);
        }

        //Map từ Model => Entity
        public override LuongCtGiamTruTruocThue ToEntity()
        {
            var entity = new LuongCtGiamTruTruocThue();
            Mapper(this, entity);
            return entity;
        }
    }
}
