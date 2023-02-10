using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Manufacture;

namespace TN.TNM.BusinessLogic.Messages.Responses.Manufacture
{
    public class GetMasterDataViewRememberItemDialogResponse : BaseResponse
    {
        public List<RememberItemModel> ListRememberItem { get; set; }
    }
}
