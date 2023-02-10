using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.QuyTrinh;

namespace TN.TNM.BusinessLogic.Messages.Requests.QuyTrinh
{
    public class DeleteQuyTrinhRequest : BaseRequest<DeleteQuyTrinhParameter>
    {
        public Guid Id { get; set; }
        public override DeleteQuyTrinhParameter ToParameter()
        {
            return new DeleteQuyTrinhParameter()
            {
                UserId = UserId,
                Id = Id
            };
        }
    }
}
