using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.FireBase
{
    public class FireBaseEntityModel
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string RoomName { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid UpdatedById { get; set; }
        public DateTime UpdatedDate { get; set; }
        public Guid TenantId { get; set; }
        public Guid? OtherId { get; set; }
    }
}
