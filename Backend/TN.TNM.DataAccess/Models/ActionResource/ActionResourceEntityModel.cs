using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.ActionResource
{
    public class ActionResourceEntityModel : BaseModel<Databases.Entities.ActionResource>
    {
        public Guid ActionResourceId { get; set; }
        public string ActionResource1 { get; set; }
        public string Description { get; set; }

        public ActionResourceEntityModel() { }

        public ActionResourceEntityModel(Databases.Entities.ActionResource entity) : base(entity)
        {
            Mapper(this, entity);
        }

        public override Databases.Entities.ActionResource ToEntity()
        {
            //Code tien xu ly model truoc khi day vao DB
            var entity = new Databases.Entities.ActionResource();
            Mapper(this, entity);
            return entity;
        }
    }
}
