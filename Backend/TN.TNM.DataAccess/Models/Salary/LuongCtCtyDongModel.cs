using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Models.Salary
{
    public class LuongCtCtyDongModel : BaseModel<LuongCtCtyDong>
    {
        public int LuongCtCtyDongId { get; set; }
        public int KyLuongId { get; set; }
        public Guid EmployeeId { get; set; }
        public Guid OrganizationId { get; set; }
        public Guid PositionId { get; set; }
        public decimal BaseBhxh { get; set; }
        public decimal Bhxh { get; set; }
        public decimal Bhyt { get; set; }
        public decimal Bhtn { get; set; }
        public decimal Bhtnnn { get; set; }
        public decimal KinhPhiCongDoan { get; set; }
        public decimal Other { get; set; }
        public decimal FundOct { get; set; }

        /* Virtual Field */
        public string EmployeeCode { get; set; }
        public string OrganizationCode { get; set; }
        public string SubCode1 { get; set; }
        public string EmployeeName { get; set; }
        public string PositionName { get; set; }
        public decimal Sum { get; set; }

        public LuongCtCtyDongModel()
        {

        }

        //Map từ Entity => Model
        public LuongCtCtyDongModel(LuongCtCtyDong entity)
        {
            Sum = entity.Bhxh + entity.Bhyt + entity.Bhtn + entity.Bhtnnn + entity.KinhPhiCongDoan + entity.Other +
                  entity.FundOct;

            Mapper(entity, this);
        }

        //Map từ Model => Entity
        public override LuongCtCtyDong ToEntity()
        {
            var entity = new LuongCtCtyDong();
            Mapper(this, entity);
            return entity;
        }
    }
}
