using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.QuyTrinh;
using TN.TNM.DataAccess.Models.QuyTrinh;

namespace TN.TNM.BusinessLogic.Messages.Requests.QuyTrinh
{
    public class UpdateQuyTrinhRequest : BaseRequest<UpdateQuyTrinhParameter>
    {
        public QuyTrinhModel QuyTrinh { get; set; }
        public List<CauHinhQuyTrinhModel> ListCauHinhQuyTrinh { get; set; }
        public override UpdateQuyTrinhParameter ToParameter()
        {
            return new UpdateQuyTrinhParameter()
            {
                UserId = UserId,
                QuyTrinh = QuyTrinh,
                ListCauHinhQuyTrinh = ListCauHinhQuyTrinh
            };
        }
    }
}
