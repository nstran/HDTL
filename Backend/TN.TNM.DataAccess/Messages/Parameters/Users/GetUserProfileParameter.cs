using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Users
{
    public class GetUserProfileParameter : BaseParameter
    {
        public Guid? Id { get; set; }
    }
}
