using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Models.Salary
{
    public class LuongCtBaoHiemModel : BaseModel<LuongCtBaoHiem>
    {
        public int LuongCtBaoHiemId { get; set; }
        public int KyLuongId { get; set; }
        public Guid EmployeeId { get; set; }
        public Guid OrganizationId { get; set; }
        public Guid PositionId { get; set; }
        public decimal BaseBhxh { get; set; }
        public decimal Bhxh { get; set; }
        public decimal Bhyt { get; set; }
        public decimal Bhtn { get; set; }
        public decimal Bhtnnn { get; set; }
        public decimal Other { get; set; }

        /* Virtual Field */
        public string EmployeeCode { get; set; }
        public string OrganizationCode { get; set; }
        public string SubCode1 { get; set; }
        public string EmployeeName { get; set; }
        public string PositionName { get; set; }
        public decimal Sum { get; set; }

        public LuongCtBaoHiemModel()
        {

        }

        //Map từ Entity => Model
        public LuongCtBaoHiemModel(LuongCtBaoHiem entity)
        {
            Sum = entity.Bhxh + entity.Bhyt + entity.Bhtn + entity.Bhtnnn + entity.Other;

            Mapper(entity, this);
        }

        //Map từ Model => Entity
        public override LuongCtBaoHiem ToEntity()
        {
            var entity = new LuongCtBaoHiem();
            Mapper(this, entity);
            return entity;
        }
    }
}
