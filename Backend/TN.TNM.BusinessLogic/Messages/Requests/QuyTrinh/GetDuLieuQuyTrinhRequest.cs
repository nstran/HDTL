using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.QuyTrinh;

namespace TN.TNM.BusinessLogic.Messages.Requests.QuyTrinh
{
    public class GetDuLieuQuyTrinhRequest : BaseRequest<GetDuLieuQuyTrinhParameter>
    {
        public Guid ObjectId { get; set; }
        public int DoiTuongApDung { get; set; }
        public override GetDuLieuQuyTrinhParameter ToParameter()
        {
            return new GetDuLieuQuyTrinhParameter()
            {
                UserId = UserId,
                ObjectId = ObjectId,
                DoiTuongApDung = DoiTuongApDung
            };
        }
    }
}
