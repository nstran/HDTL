using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models;

namespace TN.TNM.DataAccess.Messages.Results.Admin.Email
{
    public class SearchEmailConfigMasterdataResult: BaseResult
    {
        public List<CategoryEntityModel> ListEmailType { get; set; }
        public List<CategoryEntityModel> ListEmailStatus { get; set; }
    }
}
