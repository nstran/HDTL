using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Admin.Organization;

namespace TN.TNM.BusinessLogic.Messages.Requests.Admin.Organization
{
    public class DeleteNhanVienThuocDonViRequest : BaseRequest<DeleteNhanVienThuocDonViParameter>
    {
        public Guid Id { get; set; }
        public override DeleteNhanVienThuocDonViParameter ToParameter()
        {
            return new DeleteNhanVienThuocDonViParameter()
            {
                UserId = UserId,
                Id = Id
            };
        }
    }
}
