using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.QuyTrinh;

namespace TN.TNM.BusinessLogic.Messages.Requests.QuyTrinh
{
    public class GuiPheDuyetRequest : BaseRequest<GuiPheDuyetParameter>
    {
        public Guid? ObjectId { get; set; }
        public int DoiTuongApDung { get; set; }
        public int? ObjectNumber { get; set; }
        public override GuiPheDuyetParameter ToParameter()
        {
            return new GuiPheDuyetParameter()
            {
                UserId = UserId,
                ObjectId = ObjectId,
                DoiTuongApDung = DoiTuongApDung,
                ObjectNumber = ObjectNumber
            };
        }
    }
}
