using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.QuyTrinh;

namespace TN.TNM.BusinessLogic.Messages.Responses.QuyTrinh
{
    public class SearchQuyTrinhResponse : BaseResponse
    {
        public List<QuyTrinhModel> ListQuyTrinh { get; set; }
    }
}
