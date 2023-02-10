using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Models.Cost
{
    public class CostEntityModel:BaseModel<DataAccess.Databases.Entities.Cost>
    {
        public Guid? CostId { get; set; }
        public string CostCode { get; set; }
        public string CostName { get; set; }
        public string CostCodeName { get; set; }
        public Guid? OrganizationId { get; set; }
        public Guid? StatusId { get; set; }
        public bool Active { get; set; }
        public Guid? CreatedById { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }
        public string StatusName { get; set; }
        public string OrganizationName { get; set; }
        public DateTime? NgayHieuLuc { get; set; }
        public DateTime? NgayHetHan { get; set; }
        public decimal? DonGia { get; set; }
        public decimal? SoLuongToiThieu { get; set; }

        public CostEntityModel(Databases.Entities.Cost cost)
        {
            Mapper(cost, this);
        }
        public CostEntityModel()
        {

        }

        public override Databases.Entities.Cost ToEntity()
        {
            //Code tiền xử lý model trước khí đẩy vào DB
            var entity = new DataAccess.Databases.Entities.Cost();
            Mapper(this, entity);
            return entity;
        }
    }
}
