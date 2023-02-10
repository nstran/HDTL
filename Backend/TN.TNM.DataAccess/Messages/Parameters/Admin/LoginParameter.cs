using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.User;

namespace TN.TNM.DataAccess.Messages.Parameters.Admin
{
    public class LoginParameter : BaseParameter
    {
        public UserEntityModel User { get; set; }
    }
}
