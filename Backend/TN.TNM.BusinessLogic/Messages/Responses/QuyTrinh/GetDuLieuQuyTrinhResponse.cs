using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.QuyTrinh;

namespace TN.TNM.BusinessLogic.Messages.Responses.QuyTrinh
{
    public class GetDuLieuQuyTrinhResponse : BaseResponse
    {
        public List<DuLieuQuyTrinhModel> ListDuLieuQuyTrinh { get; set; }
    }
}
