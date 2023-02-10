using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Parameters.Users
{
    public class GetUserProfileByRoomNameParameter : BaseParameter
    {
        public List<string> ListRoomName { get; set; }
    }
}
