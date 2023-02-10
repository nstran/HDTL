using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Models.Salary
{
    public class LuongCtThuNhapTinhThueModel : BaseModel<LuongCtThuNhapTinhThue>
    {
        public int LuongCtThuNhapTinhThueId { get; set; }
        public int KyLuongId { get; set; }
        public Guid EmployeeId { get; set; }
        public Guid OrganizationId { get; set; }
        public Guid PositionId { get; set; }
        public decimal NetToGross { get; set; }
        public decimal Month13 { get; set; }
        public decimal Gift { get; set; }
        public decimal Other { get; set; }

        /* Virtual Field */
        public string EmployeeCode { get; set; }
        public string OrganizationCode { get; set; }
        public string SubCode1 { get; set; }
        public string EmployeeName { get; set; }
        public string PositionName { get; set; }
        public decimal Sum { get; set; }

        public LuongCtThuNhapTinhThueModel()
        {

        }

        //Map từ Entity => Model
        public LuongCtThuNhapTinhThueModel(LuongCtThuNhapTinhThue entity)
        {
            Sum = entity.NetToGross + entity.Month13 + entity.Gift + entity.Other;

            Mapper(entity, this);
        }

        //Map từ Model => Entity
        public override LuongCtThuNhapTinhThue ToEntity()
        {
            var entity = new LuongCtThuNhapTinhThue();
            Mapper(this, entity);
            return entity;
        }
    }
}
