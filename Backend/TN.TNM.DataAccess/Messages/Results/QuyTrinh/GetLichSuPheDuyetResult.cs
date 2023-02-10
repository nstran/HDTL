using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.QuyTrinh;

namespace TN.TNM.DataAccess.Messages.Results.QuyTrinh
{
    public class GetLichSuPheDuyetResult : BaseResult
    {
        public List<LichSuPheDuyetModel> ListLichSuPheDuyet { get; set; }
    }
}
