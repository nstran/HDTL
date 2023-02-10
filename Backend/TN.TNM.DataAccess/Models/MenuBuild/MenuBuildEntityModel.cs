using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.MenuBuild
{
    public class MenuBuildEntityModel : BaseModel<DataAccess.Databases.Entities.MenuBuild>
    {
        public Guid? MenuBuildId { get; set; }
        public Guid? ParentId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string CodeParent { get; set; }
        public short Level { get; set; }
        public string Path { get; set; }
        public string NameIcon { get; set; }
        public short IndexOrder { get; set; }
        public bool? IsPageDetail { get; set; }
        public bool? IsShow { get; set; }

        public MenuBuildEntityModel()
        {
        }

        public MenuBuildEntityModel(DataAccess.Databases.Entities.MenuBuild entity)
        {
            Mapper(entity, this);
        }

        public override DataAccess.Databases.Entities.MenuBuild ToEntity()
        {
            var entity = new DataAccess.Databases.Entities.MenuBuild();
            Mapper(this, entity);
            return entity;
        }
    }
}
