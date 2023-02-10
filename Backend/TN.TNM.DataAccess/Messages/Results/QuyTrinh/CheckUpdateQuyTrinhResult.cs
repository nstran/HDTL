using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Results.QuyTrinh
{
    public class CheckUpdateQuyTrinhResult : BaseResult
    {
        public bool IsResetDoiTuong { get; set; }
        public List<string> ListDoiTuong { get; set; }
    }
}
