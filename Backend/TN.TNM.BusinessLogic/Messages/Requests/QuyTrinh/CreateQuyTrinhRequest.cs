using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.QuyTrinh;
using TN.TNM.DataAccess.Models.QuyTrinh;

namespace TN.TNM.BusinessLogic.Messages.Requests.QuyTrinh
{
    public class CreateQuyTrinhRequest : BaseRequest<CreateQuyTrinhParameter>
    {
        public QuyTrinhModel QuyTrinh { get; set; }
        public List<CauHinhQuyTrinhModel> ListCauHinhQuyTrinh { get; set; }
        public override CreateQuyTrinhParameter ToParameter()
        {
            return new CreateQuyTrinhParameter()
            {
                UserId = UserId,
                QuyTrinh = QuyTrinh,
                ListCauHinhQuyTrinh = ListCauHinhQuyTrinh
            };
        }
    }
}
