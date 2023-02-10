using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.QuyTrinh;

namespace TN.TNM.BusinessLogic.Messages.Requests.QuyTrinh
{
    public class CheckTrangThaiQuyTrinhRequest : BaseRequest<CheckTrangThaiQuyTrinhParameter>
    {
        public int DoiTuongApDung { get; set; }
        public Guid? Id { get; set; }
        public override CheckTrangThaiQuyTrinhParameter ToParameter()
        {
            return new CheckTrangThaiQuyTrinhParameter()
            {
                UserId = UserId,
                DoiTuongApDung = DoiTuongApDung,
                Id = Id
            };
        }
    }
}
