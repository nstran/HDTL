using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Models
{
    public class PermissionEntityModel : BaseModel<Databases.Entities.Permission>
    {
        public Guid? PermissionId { get; set; }
        public string PermissionCode { get; set; }
        public string PermissionName { get; set; }
        public Guid? ParentId { get; set; }
        public string PermissionDescription { get; set; }
        public string Type { get; set; }
        public string IconClass { get; set; }
        public bool? Active { get; set; }
        public List<PermissionEntityModel> PermissionChildList { get; set; }
        public int NumberOfUserHasPermission { get; set; }
        public short? Sort { get; set; }
        public PermissionEntityModel() { }
        public PermissionEntityModel(Databases.Entities.Permission entity)
        {
            Mapper(entity, this);
        }

        public override Databases.Entities.Permission ToEntity()
        {
            var entity = new Databases.Entities.Permission();
            Mapper(this, entity);
            return entity;
        }
    }
}
