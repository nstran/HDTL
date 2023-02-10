using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.FireBase;

namespace TN.TNM.DataAccess.Messages.Results.FireBase
{
    public class TakeDataFireBaseByUserIdResult : BaseResult
    {
        public List<FireBaseEntityModel> ListFireBaseEntityModel { get; set; }
    }
}
