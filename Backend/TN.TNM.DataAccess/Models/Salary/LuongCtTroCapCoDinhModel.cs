using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Models.Salary
{
    public class LuongCtTroCapCoDinhModel : BaseModel<LuongCtTroCapCoDinh>
    {
        public int LuongCtTroCapCoDinhId { get; set; }
        public int KyLuongId { get; set; }
        public Guid EmployeeId { get; set; }
        public Guid OrganizationId { get; set; }
        public Guid PositionId { get; set; }
        public Guid? LoaiHopDongId { get; set; }

        /* Virtual Field */
        public string EmployeeCode { get; set; }
        public string OrganizationCode { get; set; }
        public string SubCode1 { get; set; }
        public string EmployeeName { get; set; }
        public string PositionName { get; set; }

        public LuongCtTroCapCoDinhModel()
        {

        }

        //Map từ Entity => Model
        public LuongCtTroCapCoDinhModel(LuongCtTroCapCoDinh entity)
        {
            Mapper(entity, this);
        }

        //Map từ Model => Entity
        public override LuongCtTroCapCoDinh ToEntity()
        {
            var entity = new LuongCtTroCapCoDinh();
            Mapper(this, entity);
            return entity;
        }
    }
}
