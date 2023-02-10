using System;

namespace TN.TNM.DataAccess.Models.BankAccount
{
    public class BankAccountEntityModel : BaseModel<Databases.Entities.BankAccount>
    {
        public Guid? BankAccountId { get; set; }
        public Guid ObjectId { get; set; }
        public string ObjectType { get; set; }
        public string AccountNumber { get; set; }
        public string BankName { get; set; }
        public string BankDetail { get; set; }
        public string BranchName { get; set; }
        public string AccountName { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? Active { get; set; }

        public string LabelShow { get; set; }

        public BankAccountEntityModel()
        {
        }

        public BankAccountEntityModel(Databases.Entities.BankAccount entity)
        {
            Mapper(entity, this);
        }

        public override Databases.Entities.BankAccount ToEntity()
        {
            var entity = new Databases.Entities.BankAccount();
            Mapper(this, entity);
            return entity;
        }
    }
}
