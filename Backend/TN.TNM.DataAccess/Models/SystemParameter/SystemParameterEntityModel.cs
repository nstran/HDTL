using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.SystemParameter
{
    public class SystemParameterEntityModel : BaseModel<Databases.Entities.SystemParameter>
    {
        public Guid SystemParameterId { get; set; }
        public string SystemKey { get; set; }
        public bool? SystemValue { get; set; }
        public string SystemValueString { get; set; }
        public string SystemDescription { get; set; }
        public bool? Active { get; set; }

        public Guid? TenantId { get; set; }
        public string SystemGroupCode { get; set; }
        public string SystemGroupDesc { get; set; }
        public string Description { get; set; }
        public SystemParameterEntityModel() { }

        public SystemParameterEntityModel(Databases.Entities.SystemParameter entity) : base(entity)
        {
            Mapper(this, entity);
        }

        public override Databases.Entities.SystemParameter ToEntity()
        {
            //Code tien xu ly model truoc khi day vao DB
            var entity = new Databases.Entities.SystemParameter();
            Mapper(this, entity);
            return entity;
        }
    }
}
