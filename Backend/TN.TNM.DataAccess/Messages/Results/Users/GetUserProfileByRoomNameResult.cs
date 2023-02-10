using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Results.Users
{
    public class GetUserProfileByRoomNameResult : BaseResult
    {
        public List<GetUserProfileResult> ListUserProfileResult { get; set; }
    }
}
