using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.QuyTrinh;

namespace TN.TNM.BusinessLogic.Messages.Requests.QuyTrinh
{
    public class GetLichSuPheDuyetRequest : BaseRequest<GetLichSuPheDuyetParameter>
    {
        public Guid ObjectId { get; set; }
        public int DoiTuongApDung { get; set; }
        public override GetLichSuPheDuyetParameter ToParameter()
        {
            return new GetLichSuPheDuyetParameter()
            {
                UserId = UserId,
                ObjectId = ObjectId,
                DoiTuongApDung = DoiTuongApDung
            };
        }
    }
}
