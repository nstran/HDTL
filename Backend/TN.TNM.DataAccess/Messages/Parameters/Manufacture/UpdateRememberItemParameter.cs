using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Manufacture;

namespace TN.TNM.DataAccess.Messages.Parameters.Manufacture
{
    public class UpdateRememberItemParameter : BaseParameter
    {
        public RememberItemEntityModel RememberItem { get; set; }
    }
}
