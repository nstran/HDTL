using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.QuyTrinh;

namespace TN.TNM.BusinessLogic.Messages.Requests.QuyTrinh
{
    public class TuChoiRequest : BaseRequest<TuChoiParameter>
    {
        public Guid ObjectId { get; set; }
        public int DoiTuongApDung { get; set; }
        public string Mota { get; set; }
        public override TuChoiParameter ToParameter()
        {
            return new TuChoiParameter()
            {
                UserId = UserId,
                ObjectId = ObjectId,
                DoiTuongApDung = DoiTuongApDung,
                Mota = Mota
            };
        }
    }
}
