using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.QuyTrinh;

namespace TN.TNM.DataAccess.Messages.Results.QuyTrinh
{
    public class GetDuLieuQuyTrinhResult : BaseResult
    {
        public List<DuLieuQuyTrinhModel> ListDuLieuQuyTrinh { get; set; }
    }
}
