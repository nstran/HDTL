using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Models.Salary
{
    public class LuongCtOtherModel : BaseModel<LuongCtOther>
    {
        public int LuongCtOtherId { get; set; }
        public int KyLuongId { get; set; }
        public Guid EmployeeId { get; set; }
        public Guid OrganizationId { get; set; }
        public Guid PositionId { get; set; }
        public decimal KhoanBuTruThangTruoc { get; set; }
        public decimal TroCapKhacKhongTinhThue { get; set; }
        public decimal KhauTruHoanLaiTruocThue { get; set; }
        public decimal LuongTamUng { get; set; }

        /* Virtual Field */
        public string EmployeeCode { get; set; }
        public string OrganizationCode { get; set; }
        public string SubCode1 { get; set; }
        public string EmployeeName { get; set; }
        public string PositionName { get; set; }
        public decimal Sum { get; set; }

        public LuongCtOtherModel()
        {

        }

        //Map từ Entity => Model
        public LuongCtOtherModel(LuongCtOther entity)
        {
            Sum = entity.KhoanBuTruThangTruoc + entity.TroCapKhacKhongTinhThue + entity.KhauTruHoanLaiTruocThue;

            Mapper(entity, this);
        }

        //Map từ Model => Entity
        public override LuongCtOther ToEntity()
        {
            var entity = new LuongCtOther();
            Mapper(this, entity);
            return entity;
        }
    }
}
