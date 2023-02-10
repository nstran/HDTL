using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Options;

namespace TN.TNM.DataAccess.Messages.Results.Admin.Product
{
    public class TakeListOptionResult : BaseResult
    {
        public List<OptionsEntityModel> ListOption { get; set; }
    }
}
