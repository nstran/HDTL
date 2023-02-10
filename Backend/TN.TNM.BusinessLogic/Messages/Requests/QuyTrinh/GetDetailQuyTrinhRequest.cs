using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.QuyTrinh;

namespace TN.TNM.BusinessLogic.Messages.Requests.QuyTrinh
{
    public class GetDetailQuyTrinhRequest : BaseRequest<GetDetailQuyTrinhParameter>
    {
        public Guid Id { get; set; }
        public override GetDetailQuyTrinhParameter ToParameter()
        {
            return new GetDetailQuyTrinhParameter()
            {
                UserId = UserId,
                Id = Id
            };
        }
    }
}
