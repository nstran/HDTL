using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.QuyTrinh;

namespace TN.TNM.BusinessLogic.Messages.Responses.QuyTrinh
{
    public class GetDetailQuyTrinhResponse : BaseResponse
    {
        public QuyTrinhModel QuyTrinh { get; set; }
        public List<CauHinhQuyTrinhModel> ListCauHinhQuyTrinh { get; set; }
    }
}
