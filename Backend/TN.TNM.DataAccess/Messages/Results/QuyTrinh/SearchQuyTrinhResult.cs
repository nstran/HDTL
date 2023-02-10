using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.QuyTrinh;

namespace TN.TNM.DataAccess.Messages.Results.QuyTrinh
{
    public class SearchQuyTrinhResult : BaseResult
    {
        public List<QuyTrinhModel> ListQuyTrinh { get; set; }
    }
}
