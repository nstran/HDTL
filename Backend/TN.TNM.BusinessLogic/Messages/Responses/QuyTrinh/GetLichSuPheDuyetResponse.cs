using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.QuyTrinh;

namespace TN.TNM.BusinessLogic.Messages.Responses.QuyTrinh
{
    public class GetLichSuPheDuyetResponse : BaseResponse
    {
        public List<LichSuPheDuyetModel> ListLichSuPheDuyet { get; set; }
    }
}
