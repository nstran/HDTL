using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Models.Employee
{
    public partial class CauHinhChecklistEntityModel : BaseModel<CauHinhChecklist>
    {
        public int? CauHinhChecklistId { get; set; }
        public string TenTaiLieu { get; set; }
        public bool Active { get; set; }
        public DateTime CreatedDate { get; set; }


        public CauHinhChecklistEntityModel()
        {

        }

        //Map từ Entity => Model
        public CauHinhChecklistEntityModel(CauHinhChecklist entity)
        {
            Mapper(entity, this);
        }

        //Map từ Model => Entity
        public override CauHinhChecklist ToEntity()
        {
            var entity = new CauHinhChecklist();
            Mapper(this, entity);
            return entity;
        }
    }
}
