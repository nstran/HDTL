using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.QuyTrinh;

namespace TN.TNM.BusinessLogic.Messages.Requests.QuyTrinh
{
    public class PheDuyetRequest : BaseRequest<PheDuyetParameter>
    {
        public Guid ObjectId { get; set; }
        public int DoiTuongApDung { get; set; }
        public string Mota { get; set; }
        public override PheDuyetParameter ToParameter()
        {
            return new PheDuyetParameter()
            {
                UserId = UserId,
                DoiTuongApDung = DoiTuongApDung,
                ObjectId = ObjectId,
                Mota = Mota
            };
        }
    }
}
