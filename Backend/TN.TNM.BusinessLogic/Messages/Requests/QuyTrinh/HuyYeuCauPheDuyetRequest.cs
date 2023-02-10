using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.QuyTrinh;

namespace TN.TNM.BusinessLogic.Messages.Requests.QuyTrinh
{
    public class HuyYeuCauPheDuyetRequest : BaseRequest<HuyYeuCauPheDuyetParameter>
    {
        public Guid ObjectId { get; set; }
        public int DoiTuongApDung { get; set; }

        public override HuyYeuCauPheDuyetParameter ToParameter()
        {
            return new HuyYeuCauPheDuyetParameter()
            {
                UserId = UserId,
                ObjectId = ObjectId,
                DoiTuongApDung = DoiTuongApDung
            };
        }
    }
}
