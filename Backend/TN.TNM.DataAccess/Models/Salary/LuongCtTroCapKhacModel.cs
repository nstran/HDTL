using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Models.Salary
{
    public class LuongCtTroCapKhacModel : BaseModel<LuongCtTroCapKhac>
    {
        public int LuongCtTroCapKhacId { get; set; }
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

        public LuongCtTroCapKhacModel()
        {

        }

        //Map từ Entity => Model
        public LuongCtTroCapKhacModel(LuongCtTroCapKhac entity)
        {
            Mapper(entity, this);
        }

        //Map từ Model => Entity
        public override LuongCtTroCapKhac ToEntity()
        {
            var entity = new LuongCtTroCapKhac();
            Mapper(this, entity);
            return entity;
        }
    }
}
